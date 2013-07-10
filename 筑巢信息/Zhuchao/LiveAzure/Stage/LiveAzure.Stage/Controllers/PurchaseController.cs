using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveAzure.Models.Purchase;
using MVC.Controls.Grid;
using MVC.Controls;
using System.Globalization;
using LiveAzure.Models.General;
using LiveAzure.Models.Product;
using LiveAzure.Models.Member;
using LiveAzure.Models.Warehouse;
using LiveAzure.Models.Finance;
using LiveAzure.Models;
using System.Collections;
using LiveAzure.BLL;

namespace LiveAzure.Stage.Controllers
{
    public class PurchaseController : BaseController
    {
        //
        // GET: /Purchase/
        public static Guid gOrgPurId;//采购单的所属组织的ID
        public static Guid OrgIdSelect;//当前选中的组织
        private static Guid WarehouseSelected=Guid.Empty;//选中的仓库
        public static Guid gPurId;//采购单号
        public static Guid gHisId;
        public static int nLocking = -1;
        public static int gPstatus = -1;
        public static int gHstatus = -1;
        public static string gSearchStr  = "";
        public static int nAll = 1;//是否是搜索结果--1：不是
        public static int nCalmode;//计算方式
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrgId">级联组织</param>
        /// <returns></returns>
        public ActionResult Index(Guid? OrgId = null)
        {
            // 权限验证
            string strProgramCode = Request.RequestContext.RouteData.Values["Controller"].ToString() +
                Request.RequestContext.RouteData.Values["Action"].ToString();
            if (!base.Permission(strProgramCode))
                return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });

            gPurId = new Guid("00000000-0000-0000-0000-000000000000");
            nLocking = -1;//锁定状态
            gPstatus = -1;//确定状态
            gHstatus = -1;//挂起状态
            gSearchStr  = "";
            nAll = 1;
            Guid? userID = CurrentSession.UserID;
            //生成组织下拉列表
            if (userID != null)
            {
                //获取用户所属组织---一个用户可能属于多个组织
                var userOrgId = dbEntity.MemberUsers.Where(p => p.Deleted == false && p.Gid == userID).Select(p => p.OrgID).FirstOrDefault();
                gOrgPurId = userOrgId;
                OrgIdSelect = OrgId == null ? userOrgId : (Guid)OrgId;//赋值所选中的组织
                List<SelectListItem> memOrgs = new List<SelectListItem>();

                var memOrg = dbEntity.MemberOrganizations.Where(s => s.Deleted == false && s.Gid == userOrgId).FirstOrDefault();
                SelectListItem item = new SelectListItem();
                item.Value = userOrgId.ToString();
                item.Text = memOrg.FullName.GetResource(CurrentSession.Culture);
                item.Selected = OrgId == null ? true : false;//默认用户所属组织为已选中值
                memOrgs.Add(item);

                //获取当前登录用户所授权的组织,如果是admin，则所有组织都被授权
                var nowuser = dbEntity.MemberUsers.Where(p => p.Deleted == false && p.Gid == userID).Single();
                if (nowuser.Role.Code == "Supervisor")
                {
                    var allmem = dbEntity.MemberOrganizations.Where(s => s.Deleted == false).ToList();
                    foreach (var item1 in allmem)
                    {
                        if (item1.Gid == userOrgId)
                        {
                            continue;
                        }
                        item = new SelectListItem();
                        if (item1.Gid == OrgId)
                        {
                            item.Selected = true;
                        }
                        item.Value = item1.Gid.ToString();
                        item.Text = item1.FullName.GetResource(CurrentSession.Culture);
                        memOrgs.Add(item);
                    }
                }
                else
                {
                    var memPrivOrg = dbEntity.MemberPrivileges.Where(p => p.Deleted == false && p.UserID == userID && p.Ptype == 2 && p.Pstatus == 1).FirstOrDefault();
                    if (memPrivOrg != null)
                    {
                        var privOrgs = dbEntity.MemberPrivItems.Where(p => p.Deleted == false && p.PrivID == memPrivOrg.Gid).Select(p => p.RefID).ToList();
                        foreach (Guid gid in privOrgs)
                        {
                            if (gid == userOrgId)
                            {
                                continue;
                            }
                            memOrg = dbEntity.MemberOrganizations.Where(s => s.Deleted == false && s.Gid == gid).Single();
                            item = new SelectListItem();
                            if (gid == OrgId)
                                item.Selected = true;
                            item.Value = gid.ToString();
                            item.Text = memOrg.FullName.GetResource(CurrentSession.Culture);
                            memOrgs.Add(item);
                        }
                    }
                }             
                ViewData["orgCode"] = memOrgs;
            }
            //生成仓库列表
            List<WarehouseInformation> warehouses = dbEntity.WarehouseInformations.Where(p => p.Deleted == false && p.Otype == (byte)ModelEnum.OrganizationType.WAREHOUSE && p.aParent == (OrgId==null? gOrgPurId:OrgId)).ToList();
            if (WarehouseSelected == null || WarehouseSelected == Guid.Empty)
            {                
                    WarehouseSelected = warehouses.Select(p => p.Gid).FirstOrDefault();                
            }            
            List<SelectListItem> warehouseList = new List<SelectListItem>();
            foreach (WarehouseInformation item in warehouses)
            {
                if (item.Gid == WarehouseSelected)
                    warehouseList.Add(new SelectListItem { Text = item.FullName.GetResource(CurrentSession.Culture), Value = item.Gid.ToString(), Selected = true });
                else
                    warehouseList.Add(new SelectListItem { Text = item.FullName.GetResource(CurrentSession.Culture), Value = item.Gid.ToString(), Selected = false });
            }
            ViewBag.WarehouseList = warehouseList;
            //生成Locking状态下拉表
            PurchaseInformation purchaseInfo = new PurchaseInformation();
            List<SelectListItem> plockinglist = GetSelectList(purchaseInfo.LockStatusList);
            ViewBag.plockinglist = plockinglist;

            //生成PStatus状态下拉列表
            //PurchaseInformation purchaseInfo = (dbEntity.PurchaseInformations.Where(o => o.Deleted == false)).FirstOrDefault();         
            List<SelectListItem> pstatuslist = GetSelectList(purchaseInfo.PurchaseStatusList);
            ViewBag.pstatuslist = pstatuslist;

            //生成Hanged状态下拉列表
            List<SelectListItem> hstatuslist = GetSelectList(purchaseInfo.HangStatusList);
            ViewBag.hstatuslist = hstatuslist;
            //允许制表
            ViewBag.EnablePrepare =(base.GetProgramNode("EnablePrepare") == "1") ? true : false;
            return View();
        }
        
        public string searchPurchase(Guid orgId, int pstatus, int hstatus, string searchStr, int plocking)
        {
            nLocking = plocking;
            gOrgPurId = orgId;
            gPstatus = pstatus;
            gHstatus = hstatus;
            gSearchStr = searchStr;
            nAll = 0;
            return "success";
        }

        public ActionResult listPur(SearchModel searchModel)
        {
            IQueryable<PurchaseInformation> purs = dbEntity.PurchaseInformations.Include("Organization").Include("Warehouse").Include("Supplier").Include("PurchaseType").Include("Currency")
                .Where(p => p.Deleted == false && p.OrgID == OrgIdSelect).AsQueryable();
            if (nAll == 1)
            {

            }
            else
            {
                if (nLocking != -1)
                {
                    purs = purs.Where(p => p.Locking == nLocking);
                }
                if (gPstatus != -1)
                {
                    purs = purs.Where(p => p.Pstatus == gPstatus);
                }
                if (gHstatus != -1)
                {
                    purs = purs.Where(p => p.Hanged == gHstatus);
                }
                if (!String.IsNullOrEmpty(gSearchStr))
                {
                    purs = purs.Where(s => s.Warehouse.FullName.Matter.ToUpper().Contains(gSearchStr.ToUpper()) ||
                        s.Supplier.FullName.Matter.ToUpper().Contains(gSearchStr.ToUpper()) ||
                        s.Code.ToUpper().Contains(gSearchStr.ToUpper())
                        );
                }
            }
           
            GridColumnModelList<PurchaseInformation> columnspur = new GridColumnModelList<PurchaseInformation>();
            columnspur.Add(p => p.Gid).SetAsPrimaryKey();
            columnspur.Add(p => p.Warehouse.ShortName.GetResource(CurrentSession.Culture)).SetName("Warehouse");
            columnspur.Add(p => p.Supplier.FullName.GetResource(CurrentSession.Culture)).SetName("Supplier");
            columnspur.Add(p => p.Code);
            columnspur.Add(p => p.LockStatusName);
            columnspur.Add(p => p.PurchaseStatusName);
            columnspur.Add(p => p.HangStatusName);
            GridData gridDataCha = purs.ToGridData(searchModel, columnspur);
            return Json(gridDataCha, JsonRequestBehavior.AllowGet);
        }

        //添加订单页面Get方法
        public ActionResult AddPurchaseDetail()
        {
            CultureInfo culture = new CultureInfo(CurrentSession.Culture);
            ViewBag.datePattern = culture.DateTimeFormat.ShortDatePattern;

            //货币下拉框
            List<SelectListItem> listorgunit = new List<SelectListItem>();
            var orgunit = dbEntity.MemberOrgCultures.Where(p => p.Deleted == false && p.Ctype == 1 && p.OrgID == OrgIdSelect).ToList();
            foreach (var i in orgunit)
            {
                SelectListItem item1 = new SelectListItem
                {
                    Text = i.Currency.Name.Matter,
                    Value = i.aCurrency.ToString()
                };
                listorgunit.Add(item1);
            }
            ViewBag.listorgunit = listorgunit;

            //供应商下拉框
            List<SelectListItem> osupplierlist = new List<SelectListItem>();
            var orgsupplier = dbEntity.PurchaseSuppliers.Where(p => p.Deleted == false && p.Otype == 3 && p.aParent == OrgIdSelect).ToList();
            foreach (var itemsupplier in orgsupplier)
            {
                SelectListItem item1 = new SelectListItem
                {
                    Text = itemsupplier.ShortName.Matter,
                    Value = itemsupplier.Gid.ToString() 
                };
                osupplierlist.Add(item1);
            }
            ViewBag.osupplierlist = osupplierlist;

            //仓库下拉框
            List<SelectListItem> owarehouselist = new List<SelectListItem>();
            var orgwarehouse = dbEntity.WarehouseInformations.Where(p => p.Deleted == false && p.Otype == 2 && p.aParent == OrgIdSelect).ToList();
            foreach (var itemwarehouse in orgwarehouse)
            {
                SelectListItem item1 = new SelectListItem
                {
                    Text = itemwarehouse.ShortName.Matter,
                    Value = itemwarehouse.Gid.ToString()
                };
                owarehouselist.Add(item1);
            }
            ViewBag.owarehouselist = owarehouselist;

            //订单状态下拉框
            PurchaseInformation opur = new PurchaseInformation();
            List<SelectListItem> ostatuslist = GetSelectList(opur.PurchaseStatusList);
            ViewBag.ostatuslist = ostatuslist;


            //挂起状态下拉框
            List<SelectListItem> ohangstatuslist = GetSelectList(opur.HangStatusList);
            ViewBag.ohangstatuslist = ohangstatuslist;

            //金额结算方式下拉框
            List<SelectListItem> ocalmodelist = GetSelectList(opur.CalcModeList);
            ViewBag.ocalmodelist = ocalmodelist;
            
            //采购类型下拉框
            List<GeneralPrivateCategory> oPurchases = dbEntity.GeneralPrivateCategorys.Where(p => p.Deleted == false && p.OrgID == OrgIdSelect && p.Ctype == (byte)ModelEnum.PrivateCategoryType.PURCHASE).ToList(); 
            List<SelectListItem> PurchaseTypeList = new List<SelectListItem>();
            foreach (GeneralPrivateCategory item in oPurchases)
            {
                PurchaseTypeList.Add(new SelectListItem { Text = item.Code, Value = item.Gid.ToString() });
            }
            ViewBag.purchaseTypeList = PurchaseTypeList;
            return View();
        }



        
        public ActionResult SetPurId()
        { 
            return View();
        }

        //传递id到达EditPurchaseDetail页面设置全局变量
        [HttpPost]
        public string setDoPurId(string strId)
        {
            gPurId = new Guid(strId);
            return "success";
        }


        //编辑订单页面方法
        public ActionResult EditPurchaseDetail()
        {
            var pur = dbEntity.PurchaseInformations.Include("Currency").Include("PurchaseType").Include("Supplier").Include("Warehouse").Include("Organization").
                        Where(o => o.Gid == gPurId && o.Deleted == false).Single();
            CultureInfo culture = new CultureInfo(CurrentSession.Culture);
            ViewBag.datePattern = culture.DateTimeFormat.ShortDatePattern;
            //ViewBag.datePattern = "yy-mm-dd";
            //货币下拉框
            List<SelectListItem> listorgunit = new List<SelectListItem>();
            var orgunit = dbEntity.MemberOrgCultures.Where(p => p.Deleted == false && p.Ctype == 1 && p.OrgID == gOrgPurId).ToList();
            foreach (var i in orgunit)
            {
                SelectListItem item1 = new SelectListItem
                {
                    Text = i.Currency.Name.Matter,
                    Value = i.aCurrency.ToString()
                };
                listorgunit.Add(item1);
            }
            ViewBag.listorgunit = listorgunit;
            //供应商下拉框
            List<SelectListItem> osupplierlist = new List<SelectListItem>();
            SelectListItem itemS = new SelectListItem
            {
                Text = pur.Supplier.FullName.GetResource(CurrentSession.Culture),
                Value = pur.aSupplier.ToString(),
                Selected = true
            };
            osupplierlist.Add(itemS);
            ViewBag.osupplierlist = osupplierlist;
            //供应商下拉框
            //List<SelectListItem> osupplierlist = new List<SelectListItem>();
            //var orgsupplier = dbEntity.PurchaseSuppliers.Where(p => p.Deleted == false && p.Otype == 3 && p.aParent == gOrgPurId).ToList();
            //foreach (var itemsupplier in orgsupplier)
            //{
            //    if (itemsupplier.Gid == pur.aSupplier)
            //    {
            //        SelectListItem item1 = new SelectListItem
            //        {
            //            Text = itemsupplier.ShortName.Matter,
            //            Value = itemsupplier.Gid.ToString(),
            //            Selected = true

            //        };
            //        osupplierlist.Add(item1);
            //    }
            //    else
            //    {
            //        SelectListItem item1 = new SelectListItem
            //        {
            //            Text = itemsupplier.ShortName.Matter,
            //            Value = itemsupplier.Gid.ToString(),
            //            Selected = false

            //        };
            //        osupplierlist.Add(item1);
            //    }
            //}
            //ViewBag.osupplierlist = osupplierlist;
            //仓库下拉框
            ////仓库下拉框
            List<SelectListItem> owarehouselist = new List<SelectListItem>();
            owarehouselist.Add(new SelectListItem { Text = pur.Warehouse.FullName.GetResource(CurrentSession.Culture), Value = pur.WhID.ToString(), Selected = true });
            ViewBag.owarehouselist = owarehouselist;
            //List<SelectListItem> owarehouselist = new List<SelectListItem>();
            //var orgwarehouse = dbEntity.WarehouseInformations.Where(p => p.Deleted == false && p.Otype == 2 && p.aParent == gOrgPurId).ToList();
            //foreach (var itemwarehouse in orgwarehouse)
            //{
            //    SelectListItem item1 = new SelectListItem
            //    {
            //        Text = itemwarehouse.ShortName.Matter,
            //        Value = itemwarehouse.Gid.ToString()
            //    };
            //    owarehouselist.Add(item1);
            //}
            
            //ViewBag.owarehouselist = owarehouselist;

            //订单状态下拉框
            //PurchaseInformation opur = (dbEntity.PurchaseInformations.Where(o => o.Deleted == false)).FirstOrDefault();
            List<SelectListItem> ostatuslist = GetSelectList(pur.PurchaseStatusList);            
            ViewBag.ostatuslist = ostatuslist;

            //挂起状态下拉框
            List<SelectListItem> ohangstatuslist = GetSelectList(pur.HangStatusList);
            ViewBag.ohangstatuslist = ohangstatuslist;

            //金额结算方式下拉框
            List<SelectListItem> ocalmodelist = GetSelectList(pur.CalcModeList);
            ViewBag.ocalmodelist = ocalmodelist;

            //var pur = dbEntity.PurchaseInformations.Include("Currency").Include("PurchaseType").Include("Supplier").Include("Warehouse").Include("Organization").
            //            Where(o => o.Gid == gPurId && o.Deleted == false).Single();
            nCalmode = pur.CalcMode;
            return View(pur);
        }

        //列表：PurchaseItem
        public ActionResult listPurchaseItem(SearchModel searchModel)
        {
            IQueryable<PurchaseItem> puritems = dbEntity.PurchaseItems.Include("Purchase").Include("SkuItem").Where(p => p.Deleted == false && p.Purchase.Deleted == false && p.Purchase.OrgID == gOrgPurId && p.SkuItem.Deleted == false && p.SkuItem.OrgID == gOrgPurId && p.PurID == gPurId).AsQueryable();
            GridColumnModelList<PurchaseItem> columnspuritem = new GridColumnModelList<PurchaseItem>();
            columnspuritem.Add(p => p.Gid).SetAsPrimaryKey().SetName("Gid");
            columnspuritem.Add(p => p.SkuItem.Product.Name.Matter).SetName("Name");
            columnspuritem.Add(p => p.SkuItem.Code).SetName("Code");
            columnspuritem.Add(p => p.SkuItem.Barcode).SetName("Barcode");
            columnspuritem.Add(p => p.Price);
            columnspuritem.Add(p => p.Quantity);
            columnspuritem.Add(p => p.InQty);
            columnspuritem.Add(p => p.Amount);            
            GridData gridDataCha = puritems.ToGridData(searchModel, columnspuritem);
            return Json(gridDataCha, JsonRequestBehavior.AllowGet);
        }

        //列表：Sku
        public ActionResult listSku(SearchModel searchModel)
        {
            IQueryable<ProductInfoItem> skuitems = dbEntity.ProductInfoItems.Include("Product").Include("Organization").
                                                    Where(p => p.OrgID == gOrgPurId).AsQueryable();
            //搜索功能
            if (!String.IsNullOrEmpty(gSearchStr))
            {
                skuitems = skuitems.Where(s => s.Code.ToUpper().Contains(gSearchStr.ToUpper())
                                        || s.Barcode.ToUpper().Contains(gSearchStr.ToUpper()));
            }
            GridColumnModelList<ProductInfoItem> columnsskuitem = new GridColumnModelList<ProductInfoItem>();
            columnsskuitem.Add(p => p.Gid).SetAsPrimaryKey();
            columnsskuitem.Add(p => p.Product.Name.Matter).SetName("Name");
            columnsskuitem.Add(p => p.Code);
            columnsskuitem.Add(p => p.Barcode);
            GridData gridDataCha = skuitems.ToGridData(searchModel, columnsskuitem);
            return Json(gridDataCha, JsonRequestBehavior.AllowGet);
        }


        //设置搜索Sku字符串
        public string setSearchString(string searchStr)
        {
            gSearchStr = searchStr;
            return "success";
        }

        [HttpPost]
        public ActionResult PurchaseDetail(PurchaseInformation purinfo)
        {
            PurchaseInformation newpurinfo;
            if (gPurId.ToString().Equals("00000000-0000-0000-0000-000000000000"))
            {
                CultureInfo culture = new CultureInfo(CurrentSession.Culture);
                ViewBag.datePattern = culture.DateTimeFormat.ShortDatePattern;
                //culture.DateTimeFormat.ShortDatePattern
                newpurinfo = new PurchaseInformation();
                newpurinfo.OrgID = OrgIdSelect;
                newpurinfo.WhID = purinfo.WhID;
                newpurinfo.Pstatus = purinfo.Pstatus;
                //newpurinfo.Ptype = purinfo.Ptype;    //采购类型暂时缺少
                newpurinfo.aSupplier = purinfo.aSupplier;
                newpurinfo.Discount = purinfo.Discount;
                newpurinfo.aCurrency = purinfo.aCurrency;
                newpurinfo.CalcMode = purinfo.CalcMode;
                //newpurinfo.Quantity = purinfo.Quantity;
                //newpurinfo.Amount = purinfo.Amount;
                newpurinfo.Brief = purinfo.Brief;
                newpurinfo.DocVersion = 0;
                nCalmode = purinfo.CalcMode;
                newpurinfo.etd = purinfo.etd;
                newpurinfo.atd = purinfo.atd;
                newpurinfo.eta = purinfo.eta;
                newpurinfo.ata = purinfo.ata;
                newpurinfo.PortDate = purinfo.PortDate;
                newpurinfo.Discount = purinfo.Discount;
                //newpurinfo.etd = DateTimeOffset.Parse(purinfo.etd.Value.ToString(ViewBag.datePattern));
                //newpurinfo.atd = DateTimeOffset.Parse(purinfo.atd.Value.ToString(ViewBag.datePattern));
                //newpurinfo.eta = DateTimeOffset.Parse(purinfo.eta.Value.ToString(ViewBag.datePattern));
                //newpurinfo.ata = DateTimeOffset.Parse(purinfo.ata.Value.ToString(ViewBag.datePattern));
                //newpurinfo.PortDate = DateTimeOffset.Parse(purinfo.PortDate.Value.ToString(ViewBag.datePattern));
                dbEntity.PurchaseInformations.Add(newpurinfo);
                dbEntity.SaveChanges();
                gPurId = newpurinfo.Gid;
                return RedirectToAction("EditPurchaseDetail");
            }
            else
            {
                newpurinfo = dbEntity.PurchaseInformations.Include("Currency").Include("PurchaseType").Include("Supplier").Include("Warehouse").Include("Organization").
                            Where(o => o.Gid == gPurId && o.Deleted == false).Single();
                CultureInfo culture = new CultureInfo(CurrentSession.Culture);
                ViewBag.datePattern = culture.DateTimeFormat.ShortDatePattern;
                //culture.DateTimeFormat.ShortDatePattern
                
                newpurinfo.aCurrency = purinfo.aCurrency;
                newpurinfo.Pstatus = purinfo.Pstatus;          
                newpurinfo.Discount = purinfo.Discount;          
                newpurinfo.CalcMode = purinfo.CalcMode;
                newpurinfo.Quantity = purinfo.Quantity;
                newpurinfo.Amount = purinfo.Amount;
                newpurinfo.Brief = purinfo.Brief;
                newpurinfo.DocVersion =  newpurinfo.DocVersion + 1;
                newpurinfo.Hanged = purinfo.Hanged;
                nCalmode = purinfo.CalcMode;
                //string stretd = purinfo.etd.Value.ToString("yyyy/M/d");
                //DateTimeOffset dateetd = DateTimeOffset.Parse(stretd);
                newpurinfo.etd = purinfo.etd;
                newpurinfo.atd = purinfo.atd;
                newpurinfo.eta = purinfo.eta;
                newpurinfo.ata = purinfo.ata;
                newpurinfo.PortDate = purinfo.PortDate;
                newpurinfo.Discount = purinfo.Discount;
                //newpurinfo.etd = purinfo.etd.Value.ToString(ViewBag.datePattern);
                //newpurinfo.atd = DateTimeOffset.Parse( purinfo.atd.Value.ToString(ViewBag.datePattern) );
                //newpurinfo.eta = DateTimeOffset.Parse( purinfo.eta.Value.ToString(ViewBag.datePattern) );
                //newpurinfo.ata = DateTimeOffset.Parse( purinfo.ata.Value.ToString(ViewBag.datePattern) );
                //newpurinfo.PortDate = DateTimeOffset.Parse( purinfo.PortDate.Value.ToString(ViewBag.datePattern) );
                dbEntity.SaveChanges();

                return RedirectToAction("EditPurchaseDetail");
            }
        }

        //添加purItem页面
        [HttpPost]
        public ActionResult AddpurItem()
        {
            ViewBag.calMode = nCalmode;
            return View();
        }


        //保存PurchaseItem表格操作，对应editUrl
        public string savePurchaseItemGrid()
        {
            return "success";
        }


        [HttpPost]
        public string savePurchaseItem(string savecode, string saveprice1, string savequantity1, string saveamount1)
        {
            decimal saveprice;
            decimal savequantity;
            decimal saveamount;
            if (saveprice1 != "")
                saveprice = decimal.Parse(saveprice1);
            else
                saveprice = 0;

            if (savequantity1 != "")
                savequantity = decimal.Parse(savequantity1);
            else
                savequantity = 0;
            
            if (saveamount1 != "")
                saveamount = decimal.Parse(saveamount1);
            else
                saveamount = 0;


            var querySku = dbEntity.ProductInfoItems.Where(p => p.Code == savecode && p.OrgID == gOrgPurId && p.Deleted == false).SingleOrDefault();
            PurchaseItem queryPuritem ;
            if (querySku != null)
            {
                Guid gsku = querySku.Gid;
                queryPuritem = dbEntity.PurchaseItems.Include("Purchase").Include("SkuItem").Where(p => p.SkuID == gsku && p.PurID == gPurId).SingleOrDefault();
                if (queryPuritem != null)
                {
                    if (nCalmode == 0)
                    {
                        queryPuritem.Amount = saveamount;
                        queryPuritem.Quantity = savequantity;
                        queryPuritem.Price = saveamount/savequantity;
                    }
                    else if (nCalmode == 1)
                    {
                        queryPuritem.Quantity = savequantity;
                        queryPuritem.Price = saveprice;
                        queryPuritem.Amount = savequantity * saveprice;
                    }                    
                }
                else
                {
                    queryPuritem = new PurchaseItem();
                    queryPuritem.PurID = gPurId;
                    queryPuritem.SkuID = gsku;
                    if (nCalmode == 0)
                    {
                        queryPuritem.Amount = saveamount;
                        queryPuritem.Quantity = savequantity;
                        queryPuritem.Price = saveamount / savequantity;
                    }
                    else if (nCalmode == 1)
                    {
                        queryPuritem.Quantity = savequantity;
                        queryPuritem.Price = saveprice;
                        queryPuritem.Amount = savequantity * saveprice;
                    }
                    dbEntity.PurchaseItems.Add(queryPuritem);
                }                
               
                dbEntity.SaveChanges();
            }
            return "success";
        }
        

        [HttpPost]
        public string setZero()
        {
            gPurId = new Guid("00000000-0000-0000-0000-000000000000");
            nLocking = -1;
            gPstatus = -1;
            gHstatus = -1;
            gSearchStr = "";
            nAll = 1;
            return "success";
        }


        //作废
        [HttpPost]
        public string doDelete()
        {   
            PurchaseInformation purinfo = dbEntity.PurchaseInformations.
                                          Where(o => o.Gid == gPurId && o.Deleted == false).SingleOrDefault();
            purinfo.Deleted = true;
            var purinfoitem = dbEntity.PurchaseItems.Where(o => o.Deleted == false && o.PurID == gPurId).ToList();
            if (purinfoitem.Count != 0)
            {
                foreach (var item in purinfoitem)
                {
                    item.Deleted = true;
                }
            }
            var stockin = dbEntity.WarehouseStockIns.Where(o => o.Deleted == false && o.RefID == gPurId && o.RefType == 1).ToList();
            if (stockin.Count != 0)
            {
                return "与入库单关联,不能删除";
            }
            else
            {
                dbEntity.SaveChanges();
                return "success";
            }
            
        }

        //确认
        public string setConfirm()
        {
            PurchaseInformation purinfo = dbEntity.PurchaseInformations.
                                          Where(o => o.Gid == gPurId && o.Deleted == false).SingleOrDefault();
            purinfo.Pstatus = 1;
            purinfo.Locking = 0;
            dbEntity.SaveChanges();
            return "success";
        }

        //取消确认
        public int setUnconfirm()
        {
            PurchaseInformation purinfo = dbEntity.PurchaseInformations.
                                          Where(o => o.Gid == gPurId && o.Deleted == false).SingleOrDefault();
            Guid? userid = CurrentSession.UserID;
            if (purinfo.Locking == 0)
            {
                purinfo.LastModifiedBy = userid;
                purinfo.Pstatus = 0;
                purinfo.Locking = 1;                
                dbEntity.SaveChanges();
                return 1;
            }
            else
            {
                return 0;
            }    
        }

        //挂起
        public string setHang()
        {
            PurchaseInformation purinfo = dbEntity.PurchaseInformations.Include("Currency").Include("PurchaseType").Include("Supplier").Include("Warehouse").Include("Organization").
                                              Where(o => o.Gid == gPurId && o.Deleted == false).SingleOrDefault();
            purinfo.Hanged = 1;
            PurchaseHistory purhis = dbEntity.PurchaseHistories.Where(o => o.PurID == gPurId && o.DocVersion == purinfo.DocVersion).SingleOrDefault();
            if (purhis == null)
            {
                PurchaseHistory newpurinfo = new PurchaseHistory();
                newpurinfo.PurID = gPurId;
                newpurinfo.DocVersion = purinfo.DocVersion;
                newpurinfo.WhID = purinfo.WhID;
                newpurinfo.Supplier = purinfo.aSupplier;
                newpurinfo.Pstatus = purinfo.Pstatus;
                newpurinfo.Hanged = purinfo.Hanged;
                newpurinfo.Locking = purinfo.Locking;
                newpurinfo.TrackLot = purinfo.TrackLot;
                newpurinfo.Quantity = purinfo.Quantity;
                newpurinfo.Amount = purinfo.Amount;
                newpurinfo.Brief = purinfo.Brief;
                newpurinfo.etd = purinfo.etd;
                newpurinfo.atd = purinfo.atd;
                newpurinfo.eta = purinfo.eta;
                newpurinfo.ata = purinfo.ata;
                newpurinfo.PortDate = purinfo.PortDate;
                newpurinfo.Discount = purinfo.Discount;
                dbEntity.PurchaseHistories.Add(newpurinfo);
                dbEntity.SaveChanges();
                Guid newhisid = newpurinfo.Gid;
                var purinfoitem = dbEntity.PurchaseItems.Include("Purchase").Include("SkuItem").
                                  Where(o => o.Deleted == false && o.PurID == gPurId).ToList();
                if (purinfoitem != null)
                {
                    foreach (var item in purinfoitem)
                    {
                        PurchaseHisItem newpurinfoitem = new PurchaseHisItem();
                        newpurinfoitem.PurHisID = newhisid;
                        newpurinfoitem.SkuID = item.SkuID;
                        newpurinfoitem.Quantity = item.Quantity;
                        newpurinfoitem.InQty = item.InQty;
                        newpurinfoitem.Price = item.Price;
                        newpurinfoitem.Amount = item.Amount;
                        dbEntity.PurchaseHisItems.Add(newpurinfoitem);
                    }
                }
            }
         
            dbEntity.SaveChanges();
            return "success";
        }
        
        //结算
        public string setClose()
        {
            PurchaseInformation purinfo = dbEntity.PurchaseInformations.
                                          Where(o => o.Gid == gPurId && o.Deleted == false).SingleOrDefault();
            purinfo.Pstatus = 2;
            dbEntity.SaveChanges();
            return "success";
        }

        //锁定
        public int setLock()
        {
            PurchaseInformation purinfo = dbEntity.PurchaseInformations.
                                          Where(o => o.Gid == gPurId && o.Deleted == false).SingleOrDefault();
            Guid? userid = CurrentSession.UserID;
            if (purinfo.Locking == 0 || purinfo.LastModifiedBy == userid)
            {
                purinfo.LastModifiedBy = userid;
                purinfo.Locking = 1;
                dbEntity.SaveChanges();
                return 1;
            }
            else
            {
                return 0;
            }      
        }

        //解锁
        public string setUnlock()
        {
            PurchaseInformation purinfo = dbEntity.PurchaseInformations.
                                          Where(o => o.Gid == gPurId && o.Deleted == false).SingleOrDefault();
            purinfo.Locking = 0;
            dbEntity.SaveChanges();
            return "success";
        }

        //改变金额计算方式
        public string setCalmode(int calmode)
        {
            nCalmode = calmode;
            var purinfoitem = dbEntity.PurchaseItems.Include("Purchase").Include("SkuItem").
                              Where(o => o.Deleted == false && o.PurID == gPurId).ToList();
            foreach (var item in purinfoitem)
            {
                if (nCalmode == 0)
                {
                    item.Price = item.Amount / item.Quantity;   
                }
                else if (nCalmode == 1)
                {
                    item.Amount = item.Price * item.Quantity;
                }
            }
            dbEntity.SaveChanges();
            return "success";
        }

        //历史数据入口
        public ActionResult PurchaseHistory()
        {
            return View();
        }

        public ActionResult listPurchaseHistory(SearchModel searchModel)
        {
            IQueryable<PurchaseHistory> purhistorys = dbEntity.PurchaseHistories.Include("Purchase").Include("HistoryItems").Where(p => p.Deleted == false && p.PurID == gPurId).AsQueryable();
            GridColumnModelList<PurchaseHistory> columnspur = new GridColumnModelList<PurchaseHistory>();
            columnspur.Add(p => p.Gid).SetAsPrimaryKey();
            columnspur.Add(p => p.DocVersion);
            columnspur.Add(p => p.Quantity);
            columnspur.Add(p => p.Amount);
            GridData gridDataCha = purhistorys.ToGridData(searchModel, columnspur);
            return Json(gridDataCha, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PurchaseHistoryDefination()
        {
            return View();
        }


        public string setHistoryId(string strId)
        {
            gHisId = new Guid(strId);
            return "success";
        }


        //历史数据细节
        public ActionResult PurchaseHistoryDetail()
        {
            CultureInfo culture = new CultureInfo(CurrentSession.Culture);
            ViewBag.datePattern = culture.DateTimeFormat.ShortDatePattern;
            //ViewBag.datePattern = "yy-mm-dd";

            //货币下拉框
            List<SelectListItem> listorgunit = new List<SelectListItem>();
            var orgunit = dbEntity.MemberOrgCultures.Where(p => p.Deleted == false && p.Ctype == 1 && p.OrgID == gOrgPurId).ToList();
            foreach (var i in orgunit)
            {
                SelectListItem item1 = new SelectListItem
                {
                    Text = i.Currency.Name.Matter,
                    Value = i.aCurrency.ToString()
                };
                listorgunit.Add(item1);
            }
            ViewBag.listorgunit = listorgunit;

            //供应商下拉框
            List<SelectListItem> osupplierlist = new List<SelectListItem>();
            var orgsupplier = dbEntity.PurchaseSuppliers.Where(p => p.Deleted == false && p.Otype == 3 && p.aParent == gOrgPurId).ToList();
            foreach (var itemsupplier in orgsupplier)
            {
                SelectListItem item1 = new SelectListItem
                {
                    Text = itemsupplier.ShortName.Matter,
                    Value = itemsupplier.Gid.ToString()
                };
                osupplierlist.Add(item1);
            }
            ViewBag.osupplierlist = osupplierlist;

            //仓库下拉框
            List<SelectListItem> owarehouselist = new List<SelectListItem>();
            var orgwarehouse = dbEntity.WarehouseInformations.Where(p => p.Deleted == false && p.Otype == 2 && p.aParent == gOrgPurId).ToList();
            foreach (var itemwarehouse in orgwarehouse)
            {
                SelectListItem item1 = new SelectListItem
                {
                    Text = itemwarehouse.ShortName.Matter,
                    Value = itemwarehouse.Gid.ToString()
                };
                owarehouselist.Add(item1);
            }

            ViewBag.owarehouselist = owarehouselist;


            //订单状态下拉框
            PurchaseInformation opur = dbEntity.PurchaseInformations.FirstOrDefault();
            List<SelectListItem> ostatuslist = GetSelectList(opur.PurchaseStatusList);
            ViewBag.ostatuslist = ostatuslist;


            //挂起状态下拉框
            List<SelectListItem> ohangstatuslist = GetSelectList(opur.HangStatusList);
            ViewBag.ohangstatuslist = ohangstatuslist;


            //金额结算方式下拉框
            List<SelectListItem> ocalmodelist = GetSelectList(opur.CalcModeList);
            ViewBag.ocalmodelist = ocalmodelist;

            PurchaseHistory purhis = dbEntity.PurchaseHistories.Include("Purchase").Include("HistoryItems").Where(p => p.Deleted == false && p.Gid == gHisId).SingleOrDefault();
            return View(purhis);
        }


        public ActionResult listPurchaseHistoryItem(SearchModel searchModel)
        {
            IQueryable<PurchaseHisItem> purhistorys = dbEntity.PurchaseHisItems.Include("History").Where(p => p.Deleted == false && p.PurHisID == gHisId).AsQueryable();
            GridColumnModelList<PurchaseHisItem> columnspur = new GridColumnModelList<PurchaseHisItem>();
            columnspur.Add(p => p.Gid).SetAsPrimaryKey();
            columnspur.Add(p => p.Quantity);
            columnspur.Add(p => p.Amount);
            GridData gridDataCha = purhistorys.ToGridData(searchModel, columnspur);
            return Json(gridDataCha, JsonRequestBehavior.AllowGet);
        }



        public string readPara()
        {
            PurchaseInformation purinfo = dbEntity.PurchaseInformations.Include("Currency").Include("PurchaseType").Include("Supplier").Include("Warehouse").Include("Organization").
                                              Where(o => o.Gid == gPurId && o.Deleted == false).SingleOrDefault();
            int otherlocking;
            int mylocking = purinfo.Locking;
            int mypstatus = purinfo.Pstatus;
            int myhanged = purinfo.Hanged;
            string strreturn = "";
            Guid? userid = CurrentSession.UserID;
            //获取超级管理员
            MemberUser currentUser = dbEntity.MemberUsers.Include("Role").Where(p=>p.Deleted==false&&p.Gid==CurrentSession.UserID&&p.Ustatus==(byte)ModelEnum.UserStatus.VALID).FirstOrDefault();
            if (purinfo.Locking == 0 || purinfo.LastModifiedBy == userid || currentUser.Role.Code == "Supervisor")
            {
                otherlocking = 0;
            }
            else
            {
                otherlocking = 1;
            }
            strreturn = strreturn + otherlocking + "," + mylocking + "," + mypstatus + "," + myhanged;
            return strreturn;
        }


        public string showAllPur()
        {
            nAll = 1;
            return "success";
        }

        public string outWare()
        {
            Guid? userid = CurrentSession.UserID;
            Guid gInWareId;
            WarehouseBLL oWarehouseBLL = new WarehouseBLL(dbEntity);
            int nResult = oWarehouseBLL.GenerateStockInFromPurchase(gPurId, userid, out gInWareId);
            if (nResult == 0)
            {
                string sInCode = (from s in dbEntity.WarehouseStockIns
                                  where s.Gid == gInWareId
                                  select s.Code).FirstOrDefault();

            }
            else
            {

            }
            return "success";
        }
        #region 财物记录
        /// <summary>
        /// 采购订单付款记录历史
        /// </summary>
        /// <returns></returns>
        public ActionResult PurchaseFinance()
        {
            return View();
        }
        /// <summary>
        /// 采购订单付款记录历史列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListPurchaseFinanceHistory(SearchModel searchModel)
        {
            CultureInfo culture = new CultureInfo(CurrentSession.Culture);
            IQueryable<FinancePayment> PurchaseFinance = dbEntity.FinancePayments.Include("Orgnization").Include("Currency").Where(p => p.Deleted == false && p.RefID == gPurId).AsQueryable();
            GridColumnModelList<FinancePayment> columns = new GridColumnModelList<FinancePayment>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Organization.FullName.GetResource(CurrentSession.Culture)).SetName("Orgnization");
            columns.Add(p => p.Code);
            columns.Add(p => p.PayTo);
            columns.Add(p => p.Pstatus);
            columns.Add(p => p.RefTypeName);
            columns.Add(p => p.Reason);
            columns.Add(p => p.PayDate.Value.ToString(culture.DateTimeFormat.ShortDatePattern)).SetName("Paydate");
            columns.Add(p => p.Currency.Code);
            columns.Add(p => p.Amount);
            GridData gridData = PurchaseFinance.ToGridData(searchModel, columns);

            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 质检
        /// <summary>
        /// 质检
        /// </summary>
        /// <returns></returns>
        public ActionResult Quality()
        {
            return View();
        }
        #endregion
    }
}
