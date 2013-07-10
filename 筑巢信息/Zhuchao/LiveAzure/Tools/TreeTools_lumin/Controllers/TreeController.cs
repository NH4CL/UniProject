using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveAzure.Models;
using LiveAzure.Utility;
using TreeTools_lumin.Models;
using LiveAzure.Models.General;
using LiveAzure.Models.Product;
using LiveAzure.Models.Member;
using MVC.Controls;
using MVC.Controls.Grid;
using LiveAzure.Models.Shipping;
using LiveAzure.Models.Finance;
using LiveAzure.Models.Order;

namespace TreeTools_lumin.Controllers
{
    public class TreeController : BaseController
    {
        // 
        // GET: /Tree

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PartialTest()
        {
            return PartialView();
        }

        public ActionResult MutipleTree() 
        {
            return View();
        }

        public ActionResult TestPartialTree() 
        {
            return View();
        }

        public ActionResult TestJqGrid() 
        {
            return View();
        }

        public ActionResult OrderConsignee(bool bSelectAddress, Guid? memberAddressGid) 
        {
            OrderInformation oNewOrderInfo = new OrderInformation();

            if (bSelectAddress == true)
            {
                Guid addressGid = (Guid)memberAddressGid;
                MemberAddress oMemberAddress = dbEntity.MemberAddresses.Include("User").Include("Location").Where(p => p.Deleted == false && p.Gid == memberAddressGid).FirstOrDefault();

                if (oMemberAddress != null)
                {
                    oNewOrderInfo.Consignee = oMemberAddress.User.LastName + " " + oMemberAddress.User.FirstName;
                    oNewOrderInfo.Location = oMemberAddress.Location;
                    oNewOrderInfo.aLocation = oMemberAddress.aLocation;
                    oNewOrderInfo.FullAddress = oMemberAddress.FullAddress;
                    oNewOrderInfo.PostCode = oMemberAddress.PostCode;
                    oNewOrderInfo.Telephone = oMemberAddress.CellPhone;
                    oNewOrderInfo.Email = oMemberAddress.Email;
                }
            }

            return View(oNewOrderInfo);
        }

        public ActionResult OrderUserIndex() 
        {
            return View();
        }
        /// <summary>
        /// 获取用户地址信息
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListOrderUser(SearchModel searchModel) 
        {
            Guid userOrgId = Guid.Parse("5478ce0e-0ada-e011-be99-4437e63336bd");

            IQueryable<MemberAddress> oMemberAddress = dbEntity.MemberAddresses.Include("User").Where(p => p.Deleted == false && p.User.OrgID == userOrgId).AsQueryable();

            GridColumnModelList<MemberAddress> columns = new GridColumnModelList<MemberAddress>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.User.LoginName);
            columns.Add(p => p.User.LastName);
            columns.Add(p => p.User.FirstName);
            columns.Add(p => p.Code);
            columns.Add(p => p.FullAddress);

            GridData gridData = oMemberAddress.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        
        public string TreeGeneration()
        {
            //string strTreeJson = "";

            //strTreeJson += "{\"name\":\"root\", \"id\":\"root\", \"checked\": false, \"isParent\":true, \"parentId\":null, \"open\":true , \"nodes\":[";

            var progList = dbEntity.GeneralRegions.Where(o => o.Parent == null && o.Deleted == false).ToList();

            int iProgCount = progList.Count();

            List<LiveTreeNode> list = new List<LiveTreeNode>();

            foreach (var item in progList)
            {
                LiveTreeNode treeNode = new LiveTreeNode();
                treeNode.id = item.Gid.ToString();
                treeNode.name = item.FullName;
                treeNode.icon = "";
                treeNode.iconClose = "";
                treeNode.iconOpen = "";
                treeNode.nodeChecked = false;

                treeNode.nodes = new List<LiveTreeNode>();

                list.Add(treeNode);
            }

            string strTreeJson = CreateTree(list);

            return strTreeJson;

        }

        public string TreeExpand(Guid id)
        {
            string strTreeJson = "";

            //当展开root节点的时候
            if (id.ToString().Equals("00000000-0000-0000-0000-000000000000"))
            {
                var progList = dbEntity.GeneralRegions.Where(o => o.Parent == null && o.Deleted == false).ToList();

                int iProgCount = progList.Count();

                List<LiveTreeNode> list = new List<LiveTreeNode>();

                foreach (var item in progList)
                {
                    if (item.Deleted == false)
                    {
                        LiveTreeNode treeNode = new LiveTreeNode();
                        treeNode.id = item.Gid.ToString();
                        treeNode.name = item.FullName;
                        treeNode.icon = "";
                        treeNode.iconClose = "";
                        treeNode.iconOpen = "";
                        treeNode.nodeChecked = false;
                        
                        treeNode.nodes = new List<LiveTreeNode>();

                        list.Add(treeNode);
                    }
                }

                strTreeJson = CreateTreeJson(list, "");
            }
            else                                                          //非root节点展开的时候，回传的gid不为空
            {
                GeneralRegion progChildList = dbEntity.GeneralRegions.Include("ChildItems").Where(p => p.Gid == id).Single();

                List<LiveTreeNode> list = new List<LiveTreeNode>();

                foreach (var item in progChildList.ChildItems)
                {
                    if (item.Deleted == false)
                    {
                        LiveTreeNode treeNode = new LiveTreeNode();
                        treeNode.id = item.Gid.ToString();
                        treeNode.name = item.FullName;
                        treeNode.icon = "";
                        treeNode.iconClose = "";
                        treeNode.iconOpen = "";
                        treeNode.nodeChecked = false;
                        
                        treeNode.nodes = new List<LiveTreeNode>();

                        list.Add(treeNode);
                    }
                }

                strTreeJson = CreateTreeJson(list, "");
            }

            return "[" + strTreeJson + "]";
        }

        public void TreeRemove(Guid id)
        {
            //Guid gid = id;

            //GeneralProgram progChildList = dbEntity.GeneralPrograms.Include("ChildItems").Where(p => p.Gid == id).Single();

            //progChildList.Deleted = true;

            //DeleteChild(progChildList.ChildItems.ToList<GeneralProgram>());

            //dbEntity.SaveChanges();

        }

        public void DeleteChild(List<GeneralProgram> list)
        {
            int iListCount = list.Count;

            for (int i = 0; i < iListCount; i++)
            {
                if (list.ElementAt(i).ChildItems.Count > 0)
                {
                    list.ElementAt(i).Deleted = true;
                    DeleteChild(list.ElementAt(i).ChildItems.ToList<GeneralProgram>());
                }
                else
                {
                    list.ElementAt(i).Deleted = true;
                }
            }

        }

        public void UpdateTreeView(string name, Guid id)
        {
            Guid gid = id;

            GeneralProgram progChildList = dbEntity.GeneralPrograms.Where(p => p.Gid == id).Single();

            progChildList.Name.Matter = name;

            dbEntity.SaveChanges();

        }

        public ActionResult ProgramNodeEdit(Guid id)
        {
            //判断是否为root节点，如果是root则parentid应该为空
            if (!id.ToString().Equals("00000000-0000-0000-0000-000000000000"))
            {
                ViewBag.Parent = dbEntity.GeneralPrograms.Single(item => item.Gid == id);
                SelectListItem item1 = new SelectListItem();
                item1.Text = "是";
                item1.Value = "1";
                SelectListItem item0 = new SelectListItem();
                item0.Text = "否";
                item0.Value = "0";
                List<SelectListItem> items = new List<SelectListItem> { item0, item1 };
                ViewBag.Program = items;
            }

            return View();
            //parentGid = id;
            //return View();
        }

        public ActionResult SaveProgramNode()
        {
            //RedirectToAction("indexTest", "Organization");
            return View("indexTest");
        }

        protected override void Dispose(bool disposing)
        {
            dbEntity.Dispose();
            base.Dispose(disposing);
        }

        [HttpPost]
        public ActionResult RegionEdit(Guid id) 
        {
            GeneralRegion oGeneralRegion = new GeneralRegion();

            oGeneralRegion = dbEntity.GeneralRegions.Where(p=>p.Gid == id).Single();

            return PartialView("PartialTreeEdit", oGeneralRegion);
        }

        public ActionResult PartialTreeEdit()
        {
            return PartialView();
        }

        /// <summary>
        /// 编辑地区节点信息
        /// </summary>
        /// <param name="id">修改地区的Guid</param>
        /// <param name="regionFormCollection">表单集合</param>
        /// <returns></returns>
        public ActionResult EditRegion(Guid id, FormCollection regionFormCollection)
        {
            GeneralRegion oGeneralRegion = new GeneralRegion();

            oGeneralRegion = dbEntity.GeneralRegions.Where(p => p.Gid == id).Single();

            oGeneralRegion.Code = regionFormCollection["Code"];
            oGeneralRegion.FullName = regionFormCollection["FullName"];
            oGeneralRegion.ShortName = regionFormCollection["ShortName"];
            oGeneralRegion.Sorting = Int32.Parse(regionFormCollection["Sorting"]);
            oGeneralRegion.RegionLevel = Int32.Parse(regionFormCollection["RegionLevel"]);
            oGeneralRegion.Remark = regionFormCollection["Remark"];

            dbEntity.SaveChanges();

            return null;
            //return RedirectToAction("Index");
            //return RedirectToAction("PartialTest");
            
        }

        public string TestMutipleTreeGeneration() 
        {
            var progList = dbEntity.GeneralRegions.Where(o => o.Parent == null && o.Deleted == false).ToList();

            int iProgCount = progList.Count();

            List<LiveTreeNode> list = new List<LiveTreeNode>();

            foreach (var item in progList)
            {
                LiveTreeNode treeNode = new LiveTreeNode();
                treeNode.id = item.Gid.ToString();
                treeNode.name = item.FullName;
                treeNode.icon = "";
                treeNode.iconClose = "";
                treeNode.iconOpen = "";
                treeNode.nodeChecked = false;

                treeNode.nodes = new List<LiveTreeNode>();

                list.Add(treeNode);
            }

            //strTreeJson = strTreeJson.Substring(0, strTreeJson.Length - 1);

            //strTreeJson += "]}";

            //return "[" + strTreeJson + "]";
            string strTreeJson = CreateTree(list);

            return strTreeJson;
        }

        public string GetColumnSettings() 
        {
            string strSettings = "";
            string strColumnNames = "";
            Guid skuGid = Guid.Parse("0205d095-9fd3-e011-9465-4437e63336bd");// Guid();
            Guid orgId = Guid.Parse("0d8d18b8-e4d1-e011-b92d-4437e63336bd");//new Guid();

            GeneralMeasureUnit oGeneralMesureUnit = new GeneralMeasureUnit();

            //取出所有的单位，给下拉框使用
            List<GeneralMeasureUnit> listMeasureUnit = dbEntity.GeneralMeasureUnits.Include("Name").Where(p => p.Utype != 6).ToList();

            List<SelectListItem> oMeasureUnitList = new List<SelectListItem>();

            int nMeasureCount = listMeasureUnit.Count;

            for (int i = 0; i < nMeasureCount; i++)
            {
                oMeasureUnitList.Add(new SelectListItem { Text = listMeasureUnit.ElementAt(i).Name.Matter, Value = listMeasureUnit.ElementAt(i).Gid.ToString() });
            }

            //取出对应上架SKU的标准单位，来自productinfoitem
            ProductInfoItem oProductInfoItem = dbEntity.ProductInfoItems.Include("StandardUnit").Where(p => p.Gid == skuGid).FirstOrDefault();

            string strSkuStdUnit = oProductInfoItem.StandardUnit.Name.Matter;

            //取出组织所对应的货币，取自MemberOrgCulture
            List<MemberOrgCulture> oMemberOrgCulture = dbEntity.MemberOrgCultures.Include("Currency").Where(p => p.OrgID == orgId && p.Ctype == 1).ToList();

            //根据取出的货币数，生成货币的columnmodel
            List<JqGridColumns> listColumnModel = new List<JqGridColumns>();

            int nMemberCultureListCount = oMemberOrgCulture.Count;

            for (int i = 0; i < nMemberCultureListCount; i++)
            {
                JqGridColumns oColumnModel = new JqGridColumns();
                oColumnModel.name = oMemberOrgCulture.ElementAt(i).Currency.Name.Matter;
                oColumnModel.index = oMemberOrgCulture.ElementAt(i).Currency.Name.Matter;
                oColumnModel.width = "80";

                listColumnModel.Add(oColumnModel);
            }

            //除了自动生成的货币列之外，还有必然存在的三列
            //显示计量单位，即销售的计量单位
            JqGridColumns standardColumn = new JqGridColumns();
            standardColumn.name = "ShowUnit";
            standardColumn.index = "ShowUnit";
            standardColumn.width = "80";

            listColumnModel.Add(standardColumn);

            //转换比率
            JqGridColumns unitRadioColumn = new JqGridColumns();
            unitRadioColumn.name = "unitRadio";
            unitRadioColumn.index = "unitRadio";
            unitRadioColumn.width = "80";

            listColumnModel.Add(unitRadioColumn);

            //计量精度
            JqGridColumns percisionColumn = new JqGridColumns();
            percisionColumn.name = "Percision";
            percisionColumn.index = "Percision";
            percisionColumn.width = "80";

            listColumnModel.Add(percisionColumn);

            //=======================================================================================================
            for (int i=0;i<listColumnModel.Count;i++)
            {
                JqGridColumns currentColumn = listColumnModel.ElementAt(i);
                strSettings += "{ \"name\": \"" + currentColumn.name + "\",\"index\": \"" + currentColumn.index + "\",\"width\": \"" + currentColumn.width + "\"},";
                strColumnNames += "\"" + currentColumn.name + "\",";
            }
            strSettings = "[" + "{ \"name\": \" \",\"index\": \" \",\"width\": \"80\"}," + strSettings.Substring(0, strSettings.Length - 1) + "]" + "!" + "[" + "\" \"," + strColumnNames.Substring(0, strColumnNames.Length - 1) + "]";

            return strSettings;
        }

        public string GetGridJson() 
        {
            string test = "[{ id: \"1\", invdate: \"2007-10-01\", name: \"test\", note: \"note\", amount: \"200.00\", tax: \"10.00\", total: \"210.00\" }]";

            return test;
        }

        //==============================================================================================================

        public ActionResult TestOnsaleShipping()
        {
            return View();
        }

        public ActionResult TestPayment() 
        {
            return View();
        }

        public ActionResult TestPaymentAddOrEdit(bool bAddOrEdit) 
        {
            ProductOnPayment oNewProductOnsalePayment = new ProductOnPayment();
            List<FinancePayType> listFinancePayType = dbEntity.FinancePayTypes.Include("Name").Where(p => p.Deleted == false && p.OrgID == orgOnsaleGid).ToList();
            int nPaymentCount = listFinancePayType.Count;
            List<SelectListItem> oPaymentList = new List<SelectListItem>();
            for (int i = 0; i < nPaymentCount; i++)
            {
                oPaymentList.Add(new SelectListItem { Text = listFinancePayType.ElementAt(i).Name.Matter, Value = listFinancePayType.ElementAt(i).Gid.ToString() });
            }

            if (bAddOrEdit == false)
            {
                oNewProductOnsalePayment = dbEntity.ProductOnPayments.Include("OnSale").Include("PayType").Where(p => p.OnSaleID == productOnSaleGid && p.Deleted == false).FirstOrDefault();
            }
            else 
            {
                ProductOnSale oCurrentProductOnsale = dbEntity.ProductOnSales.Where(p => p.Gid == productOnSaleGid && p.Deleted == false).FirstOrDefault();
                oNewProductOnsalePayment.OnSale = oCurrentProductOnsale;
                oNewProductOnsalePayment.OnSaleID = oCurrentProductOnsale.Gid;
            }

            ViewBag.oPaytypeList = oPaymentList;
            ViewBag.bAddOrEdit = bAddOrEdit;

            return View(oNewProductOnsalePayment);
        }

        /// <summary>
        /// 查询商品承运商
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListOnsaleShipping(SearchModel searchModel) 
        {
            IQueryable<ProductOnShipping> oProductShipping = dbEntity.ProductOnShippings.Include("OnSale").Include("Shipper").Where(p => p.Deleted == false && p.OnSaleID == productOnSaleGid).AsQueryable();

            GridColumnModelList<ProductOnShipping> columns = new GridColumnModelList<ProductOnShipping>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Shipper.FullName.Matter);
            columns.Add(p => p.ShipWeight);
            columns.Add(p => p.Solution);
            columns.Add(p => p.Condition);
            columns.Add(p => p.Discount);
            columns.Add(p => p.SupportCod);
            columns.Add(p => p.Remark);

            GridData gridData = oProductShipping.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ListOnsaleShippingSearch(SearchModel searchModel, string testData)
        {
            IQueryable<ProductOnShipping> oProductShipping = dbEntity.ProductOnShippings.Include("OnSale").Include("Shipper").Where(p => p.Deleted == false && p.OnSaleID == productOnSaleGid && p.Shipper.FullName.Matter.Contains(testData)).AsQueryable();

            GridColumnModelList<ProductOnShipping> columns = new GridColumnModelList<ProductOnShipping>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Shipper.FullName.Matter);
            columns.Add(p => p.ShipWeight);
            columns.Add(p => p.Solution);
            columns.Add(p => p.Condition);
            columns.Add(p => p.Discount);
            columns.Add(p => p.SupportCod);
            columns.Add(p => p.Remark);

            GridData gridData = oProductShipping.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);

        }

        public void SaveOnsaleShipping(ProductOnShipping oProductShipping, FormCollection formCollection) 
        {
            //新建上架的商品的承运商
            if (oProductShipping.Gid.Equals(Guid.Empty))
            {
                ProductOnShipping oNewProductShipping = new ProductOnShipping();

                Guid shippingGid = Guid.Parse(formCollection["Shipper.FullName.Matter"]);

                List<ProductOnShipping> existShipping = dbEntity.ProductOnShippings.Where(p => p.ShipID == shippingGid && p.OnSaleID == productOnSaleGid).ToList();
                //如果是删除的承运商信息，则更新后，将deleted变为false
                if (existShipping.Count > 0)
                {
                    oNewProductShipping = existShipping.ElementAt(0);
                    oNewProductShipping.OnSaleID = productOnSaleGid;
                    oNewProductShipping.ShipWeight = Int32.Parse(formCollection["ShipWeight"]);
                    oNewProductShipping.Solution = Byte.Parse(formCollection["Solution"]);
                    oNewProductShipping.Discount = Decimal.Parse(formCollection["Discount"]);
                    oNewProductShipping.Condition = Decimal.Parse(formCollection["Condition"]);
                    oNewProductShipping.SupportCod = Boolean.Parse(formCollection["SupportCod"]);
                    oNewProductShipping.ShipID = shippingGid;
                    oNewProductShipping.Remark = formCollection["Remark"];
                    oNewProductShipping.Deleted = false;
                }
                else
                {
                    oNewProductShipping.OnSaleID = productOnSaleGid;
                    oNewProductShipping.ShipWeight = Int32.Parse(formCollection["ShipWeight"]);
                    oNewProductShipping.Solution = Byte.Parse(formCollection["Solution"]);
                    oNewProductShipping.Discount = Decimal.Parse(formCollection["Discount"]);
                    oNewProductShipping.Condition = Decimal.Parse(formCollection["Condition"]);
                    oNewProductShipping.SupportCod = Boolean.Parse(formCollection["SupportCod"]);
                    oNewProductShipping.ShipID = shippingGid;
                    oNewProductShipping.Remark = formCollection["Remark"];
                    dbEntity.ProductOnShippings.Add(oNewProductShipping);
                }
            }
            else 
            {
                Guid gid = oProductShipping.Gid;

                ProductOnShipping oEditProductShipping = dbEntity.ProductOnShippings.Where(p => p.Gid == gid).FirstOrDefault();

                oEditProductShipping.ShipWeight = Int32.Parse(formCollection["ShipWeight"]);
                oEditProductShipping.Solution = Byte.Parse(formCollection["Solution"]);
                oEditProductShipping.Discount = Decimal.Parse(formCollection["Discount"]);
                oEditProductShipping.Condition = Decimal.Parse(formCollection["Condition"]);
                oEditProductShipping.SupportCod = Boolean.Parse(formCollection["SupportCod"]);
                //oEditProductShipping.ShipID = Guid.Parse(formCollection["Shipper.FullName.Matter"]);
                oEditProductShipping.Remark = formCollection["Remark"];

            }

            dbEntity.SaveChanges();

        }

        private static Guid productOnSaleGid = Guid.Parse("c7b5fb90-0bd2-e011-b92d-4437e63336bd");
        private static Guid orgOnsaleGid = Guid.Parse("0d8d18b8-e4d1-e011-b92d-4437e63336bd");

        public ActionResult TestOnsaleShippingAddAndEdit(bool bAddorEdit)
        {
            ProductOnShipping oNewShipping = new ProductOnShipping();

            ProductOnSale oProductOnsale = dbEntity.ProductOnSales.Where(p => p.Deleted == false && p.Gid == productOnSaleGid).FirstOrDefault();

            //编辑状态
            if (bAddorEdit == false)
            {
                oNewShipping = dbEntity.ProductOnShippings.Include("OnSale").Include("Shipper").Include("OnShipArea").Where(p => p.OnSaleID == productOnSaleGid && p.Deleted == false).FirstOrDefault();
            }
            else 
            {
                oNewShipping.OnSaleID = productOnSaleGid;
                oNewShipping.OnSale = oProductOnsale;
            }

            List<SelectListItem> oShipperList = new List<SelectListItem>();

            List<ShippingInformation> listShipping = dbEntity.ShippingInformations.Where(p => p.Deleted == false && p.aParent == orgOnsaleGid).ToList();

            int nShippingCount = listShipping.Count;

            for (int i = 0; i < nShippingCount; i++)
            {
                oShipperList.Add(new SelectListItem { Text = listShipping.ElementAt(i).FullName.Matter, Value = listShipping.ElementAt(i).Gid.ToString() });
            }

            List<SelectListItem> oSolutionList = new List<SelectListItem>();
            oSolutionList.Add(new SelectListItem { Text = "按重量", Value = "0" });

            List<SelectListItem> oSupportCodList = new List<SelectListItem>();
            oSupportCodList = SelectEnumList(true);
            
            ViewBag.oShipperList = oShipperList;
            ViewBag.oSolutionList = oSolutionList;
            ViewBag.oSupportCodList = oSupportCodList;
            ViewBag.bAddorEdit = bAddorEdit;

            return View(oNewShipping);
        }

        /// <summary>
        /// 级联删除承运商相关信息
        /// </summary>
        /// <param name="gid"></param>
        public void DeleteOnsaleShipping(Guid gid) 
        {
            ProductOnShipping oProductOnsaleShipping = dbEntity.ProductOnShippings.Where(p => p.Deleted == false && p.Gid == gid).FirstOrDefault();

            oProductOnsaleShipping.Deleted = true;

            //删除相关的承运商区域
            List<ProductOnShipArea> listShipArea = dbEntity.ProductOnShipAreas.Where(p => p.Deleted == false && p.OnShip == gid).ToList();

            int nShipAreaCount = listShipArea.Count;

            for (int i = 0; i < nShipAreaCount; i++)
            {
                listShipArea.ElementAt(i).Deleted = true;
            }

            dbEntity.SaveChanges();

        }

        /// <summary>
        /// 返回付款方式的列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ListOnsalePayment(SearchModel searchModel) 
        {
            Guid testOnsaleGid = Guid.Parse("3f1ef123-9fe2-e011-8166-4437e63336bd");

            IQueryable<ProductOnPayment> oProductOnsalePayment = dbEntity.ProductOnPayments.Include("OnSale").Include("PayType").Where(p => p.Deleted == false && p.OnSaleID == testOnsaleGid).AsQueryable();
            
            GridColumnModelList<ProductOnPayment> columns = new GridColumnModelList<ProductOnPayment>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.OnSale.Name.Matter);
            columns.Add(p => p.Remark);

            GridData gridData = oProductOnsalePayment.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 添加或者更新付款方式
        /// </summary>
        /// <param name="oProductOnsalePayment"></param>
        /// <returns></returns>
        public ActionResult SaveOnsalePayment(ProductOnPayment oProductOnsalePayment)
        {
            ProductOnPayment oNewProductOnsalePayment = new ProductOnPayment();

            //新添加
            if (oProductOnsalePayment.Gid.Equals(Guid.Empty))
            {
                //Guid currentOnsaleID = Guid.Parse(Request.Form["PayType.Name.Matter"]);
                Guid currentPayID = Guid.Parse(Request.Form["PayType.Name.Matter"]);

                List<ProductOnPayment> listProductOnsalePayment = dbEntity.ProductOnPayments.Where(p => p.OnSaleID == oProductOnsalePayment.OnSaleID && p.PayID == currentPayID).ToList();
                //已存在索引
                if (listProductOnsalePayment.Count > 0)
                {
                    oNewProductOnsalePayment = listProductOnsalePayment.ElementAt(0);
                    oNewProductOnsalePayment.Remark = oProductOnsalePayment.Remark;
                }
                else
                {
                    oNewProductOnsalePayment.OnSaleID = oProductOnsalePayment.OnSaleID;
                    oNewProductOnsalePayment.PayID = oProductOnsalePayment.PayID;
                    oNewProductOnsalePayment.Remark = oProductOnsalePayment.Remark;

                    dbEntity.ProductOnPayments.Add(oNewProductOnsalePayment);
                }
            }
            else 
            {
                Guid gid = oProductOnsalePayment.Gid;
                oNewProductOnsalePayment = dbEntity.ProductOnPayments.Where(p => p.Deleted == false && p.Gid == gid).FirstOrDefault();
                oNewProductOnsalePayment.Remark = oProductOnsalePayment.Remark;

            }

            dbEntity.SaveChanges();

            return null;

        }

        //================================承运商的地区树状结构===============================================
        public string ShippingRegionTreeLoad() 
        {
            Guid ProductOnsaleShippingGid = Guid.Parse("4113a18d-47d2-e011-b92d-4437e63336bd");
            //首次加载的树节点
            var oRegion = (from o in dbEntity.GeneralRegions
                           where (o.Parent == null && o.Deleted == false)
                           select o).ToList();
            int nRegion = oRegion.Count;
            //该承运商已经支持的地区
            var oShipRegion = (from o in dbEntity.ProductOnShipAreas
                               where (o.OnShip == ProductOnsaleShippingGid && o.Deleted == false)
                               select o).ToList();
            int nShipRegion = oShipRegion.Count;

            bool flag = false;

            List<LiveTreeNode> list = new List<LiveTreeNode>();
            foreach (var item in oRegion)
            {
                for (int i = 0; i < nShipRegion; i++)
                {
                    if (item.Gid == oShipRegion[i].RegionID)
                    {
                        flag = true;
                        break;
                    }
                }
                LiveTreeNode TreeNode = new LiveTreeNode();
                TreeNode.id = item.Gid.ToString();
                TreeNode.name = item.FullName;
                if (flag == true) TreeNode.nodeChecked = true;
                else TreeNode.nodeChecked = false;
                if (item.ChildItems.Count > 0) TreeNode.isParent = true;
                else TreeNode.isParent = false;
                TreeNode.nodes = new List<LiveTreeNode>();
                list.Add(TreeNode);
                flag = false;
            }
            string strTreeJson = CreateTree(list);
            return strTreeJson;
        }

        public string ShippingRegionTreeExpand(Guid id) 
        {
            Guid ProductOnsaleShippingGid = Guid.Parse("4113a18d-47d2-e011-b92d-4437e63336bd");

            string strTreeJson = "";
            //展开root
            if (id.Equals(Guid.Empty))
            {
                //加载父亲为空的地区
                var oRegion = (from o in dbEntity.GeneralRegions
                               where (o.Parent == null && o.Deleted == false)
                               select o).ToList();
                int nRegion = oRegion.Count;
                //该承运商已经支持的地区
                var oShipRegion = (from o in dbEntity.ProductOnShipAreas
                                   where (o.OnShip == ProductOnsaleShippingGid && o.Deleted == false)
                                   select o).ToList();
                int nShipRegion = oShipRegion.Count;

                bool flag = false;

                List<LiveTreeNode> list = new List<LiveTreeNode>();
                foreach (var item in oRegion)
                {
                    for (int i = 0; i < nShipRegion; i++)
                    {
                        if (item.Gid == oShipRegion[i].RegionID)
                        {
                            flag = true;
                            break;
                        }
                    }
                    LiveTreeNode TreeNode = new LiveTreeNode();
                    TreeNode.id = item.Gid.ToString();
                    TreeNode.name = item.ShortName;
                    if (flag == true)
                    {
                        TreeNode.nodeChecked = true;
                    }
                    else
                    { 
                        TreeNode.nodeChecked = false; 
                    }
                    if (item.ChildItems.Count > 0)
                    {
                        TreeNode.isParent = true;
                    }
                    else
                    { 
                        TreeNode.isParent = false; 
                    }
                    TreeNode.nodes = new List<LiveTreeNode>();
                    list.Add(TreeNode);
                }
                strTreeJson = CreateTreeJson(list, "");
            }
            else
            {
                //展开非root节点
                var oRegion = (from o in dbEntity.GeneralRegions
                               where (o.Parent.Gid == id && o.Deleted == false)
                               select o).ToList();
                //该承运商已经支持的地区
                var oShipRegion = (from o in dbEntity.ProductOnShipAreas
                                   where (o.OnShip == ProductOnsaleShippingGid && o.Deleted == false)
                                   select o).ToList();
                int nShipRegion = oShipRegion.Count;

                bool flag = false;

                List<LiveTreeNode> list = new List<LiveTreeNode>();
                foreach (var item in oRegion)
                {
                    for (int i = 0; i < nShipRegion; i++)
                    {
                        if (item.Gid == oShipRegion[i].RegionID)
                        {
                            flag = true;
                            break;
                        }
                    }
                    LiveTreeNode TreeNode = new LiveTreeNode();
                    TreeNode.id = item.Gid.ToString();
                    TreeNode.name = item.ShortName;
                    if (flag == true)
                    {
                        TreeNode.nodeChecked = true;
                    }
                    else
                    {
                        TreeNode.nodeChecked = false;
                    }
                    if (item.ChildItems.Count > 0)
                    {
                        TreeNode.isParent = true;
                    }
                    else
                    {
                        TreeNode.isParent = false;
                    }
                    TreeNode.nodes = new List<LiveTreeNode>();
                    list.Add(TreeNode);
                    flag = false;
                }
                strTreeJson = CreateTreeJson(list, "");
            }
            return "[" + strTreeJson + "]";
        }

        public string AddShippingRegion(Guid id) 
        {
            Guid ProductOnsaleShippingGid = Guid.Parse("4113a18d-47d2-e011-b92d-4437e63336bd");

            ProductOnShipArea oNewProductShipArea = new ProductOnShipArea();

            List<ProductOnShipArea> listProductShipArea = dbEntity.ProductOnShipAreas.Where(p=>p.OnShip == productOnSaleGid && p.RegionID == id).ToList();

            bool bSuccess = false;

            //需要对一个开关变量进行分析，判断是否位于承运商支持的区域。如果默认是从属于shiparea表，则不属于时不能存入数据库，提示用户
            //否则将数据存入数据库
            if (bInShipArea == false)
            {
                if (listProductShipArea.Count > 0)
                {
                    oNewProductShipArea = listProductShipArea.ElementAt(0);
                    oNewProductShipArea.Deleted = false;
                }
                else
                {
                    oNewProductShipArea.OnShip = ProductOnsaleShippingGid;
                    oNewProductShipArea.RegionID = id;
                    dbEntity.ProductOnShipAreas.Add(oNewProductShipArea);
                }

                dbEntity.SaveChanges();

                bSuccess = true;
            }
            else 
            {
                ProductOnShipping oProductOnsaleShipping = dbEntity.ProductOnShippings.Where(p=>p.Gid == ProductOnsaleShippingGid && p.Deleted == false).SingleOrDefault();
                Guid currentShipGid = oProductOnsaleShipping.ShipID;
                if (dbEntity.ShippingAreas.Where(p => p.Deleted == false && p.RegionID == id && p.ShipID == currentShipGid).ToList().Count > 0)
                {
                    if (listProductShipArea.Count > 0)
                    {
                        oNewProductShipArea = listProductShipArea.ElementAt(0);
                        oNewProductShipArea.Deleted = false;
                    }
                    else
                    {
                        oNewProductShipArea.OnShip = ProductOnsaleShippingGid;
                        oNewProductShipArea.RegionID = id;
                        dbEntity.ProductOnShipAreas.Add(oNewProductShipArea);
                    }

                    dbEntity.SaveChanges();

                    bSuccess = true;
                }
                else
                {
                    //查出该地区的所有父节点区域
                    List<Guid> listRegionParent = new List<Guid>();

                    for (int i = 0; i < listRegionParent.Count; i++)
                    {
                        Guid currentParentRegionID = listRegionParent.ElementAt(i);

                        if (dbEntity.ShippingAreas.Where(p => p.Deleted == false && p.RegionID == currentParentRegionID && p.ShipID == currentShipGid).ToList().Count > 0)
                        {
                            if (listProductShipArea.Count > 0)
                            {
                                oNewProductShipArea = listProductShipArea.ElementAt(0);
                                oNewProductShipArea.Deleted = false;
                            }
                            else
                            {
                                oNewProductShipArea.OnShip = ProductOnsaleShippingGid;
                                oNewProductShipArea.RegionID = id;
                                dbEntity.ProductOnShipAreas.Add(oNewProductShipArea);
                            }

                            dbEntity.SaveChanges();

                            bSuccess = true;

                            break;
                        }
                    }
                }

            }

            if (bSuccess == true)
            {
                return "success";
            }
            else 
            {
                return "failure";
            }
            
        }

        public string DeleteShippingRegion(Guid id)
        {
            Guid ProductOnsaleShippingGid = Guid.Parse("4113a18d-47d2-e011-b92d-4437e63336bd");

            ProductOnShipArea oProductShipArea = dbEntity.ProductOnShipAreas.Where(p => p.OnShip == ProductOnsaleShippingGid && p.RegionID == id && p.Deleted == false).FirstOrDefault();

            if (oProductShipArea != null)
            {
                oProductShipArea.Deleted = true;

                dbEntity.SaveChanges();

            }

            return "success";
        }

        private static bool bInShipArea = true;

        public bool SetInShipArea()
        {
            bInShipArea = !bInShipArea;

            return bInShipArea;
        }

        public ActionResult ProductOnshipArea()
        {
            return View();
        }

        Guid unitPriceGid = new Guid();
        public ActionResult ProductOnsalePriceUnitAddOrEdit(bool bAddOrEdit) 
        {
            ProductOnUnitPrice oNewProductOnsaleUnitPrice;
            if (bAddOrEdit == true)
            {
                //添加状态
                oNewProductOnsaleUnitPrice = new ProductOnUnitPrice { SalePrice = NewResource(ModelEnum.ResourceType.MONEY), MarketPrice = NewResource(ModelEnum.ResourceType.MONEY) };
            }
            else 
            {
                oNewProductOnsaleUnitPrice = dbEntity.ProductOnUnitPrices.Include("OnSkuItem").Include("ShowUnit").Include("MarketPrice").Include("SalePrice").Where(p => p.Deleted == false && p.Gid == unitPriceGid).SingleOrDefault();
                oNewProductOnsaleUnitPrice.SalePrice = RefreshResource(ModelEnum.ResourceType.MONEY, oNewProductOnsaleUnitPrice.SalePrice, orgOnsaleGid);
                oNewProductOnsaleUnitPrice.MarketPrice = RefreshResource(ModelEnum.ResourceType.MONEY, oNewProductOnsaleUnitPrice.MarketPrice, orgOnsaleGid);
            }

            List<GeneralMeasureUnit> listMeasureUnit = dbEntity.GeneralMeasureUnits.Include("Name").Where(p => p.Deleted == false && p.Utype != 6).ToList();
            List<SelectListItem> oMeasureUnitList = new List<SelectListItem>();
            for (int i = 0; i < listMeasureUnit.Count; i++)
            {
                oMeasureUnitList.Add(new SelectListItem { Text = listMeasureUnit.ElementAt(i).Name.Matter, Value = listMeasureUnit.ElementAt(i).Gid.ToString() });
            }

            ViewBag.oMeasureUnitList = oMeasureUnitList;

            return View(oNewProductOnsaleUnitPrice);
        }

        public string TestModelInsert() 
        {
            ProductOnPayment oPayment = new ProductOnPayment();
            Guid testGid = Guid.Parse("c1f410ce-9fe2-e011-8166-4437e63336bd");
            FinancePayType oPayType = dbEntity.FinancePayTypes.Where(p => p.Gid == testGid).FirstOrDefault();
            Guid onSaleGid = Guid.Parse("3f1ef123-9fe2-e011-8166-4437e63336bd");
            oPayment.OnSaleID = onSaleGid;
            oPayment.PayType = oPayType;
            oPayment.PayID = oPayType.Gid;

            dbEntity.ProductOnPayments.Add(oPayment);

            dbEntity.SaveChanges();

            return "success";
        }

        public string TestRegion() 
        {
            //Guid locationGid = Guid.Parse("4a9da834-48e4-e011-b824-4437e63336bd");

            //List<Guid> listRegion = dbEntity.Database.SqlQuery<Guid>("SELECT Gid FROM fn_FindFullRegions({0})", locationGid).ToList();

            //int i = listRegion.Count;

            return "success";
        }

        public ActionResult TestPage() 
        {
            return View();
        }

        public string GetPageContent(int pageNum, int pageCount)
        {
            string strBack = "";
            List<ProductInformation> listProduct = new List<ProductInformation>();
            List<GeneralIpBase> listIpBase = new List<GeneralIpBase>();
                
            //int totalCount = dbEntity.ProductInformations.Count();
            int totalCount = dbEntity.GeneralIpBases.Count();
            int lastCount = totalCount % pageCount;
            int totalPageCount = 0;
            if (lastCount == 0)
            {
                totalPageCount = (totalCount - lastCount) / pageCount;
            }
            else
            {
                totalPageCount = (totalCount - lastCount) / pageCount + 1;
            }

            //当前页不是最后一页，且不是第一页
            if (pageNum < totalPageCount && pageNum > 1)
            {
                //listProduct = dbEntity.ProductInformations.OrderBy(p => p.CreateTime).Skip((pageNum - 1) * pageCount).Take(pageCount).ToList();
                listIpBase = dbEntity.GeneralIpBases.OrderBy(p => p.CreateTime).Skip((pageNum - 1) * pageCount).Take(pageCount).ToList();
                strBack = pageNum.ToString() + "|" + totalPageCount.ToString() + "|";
            }
            else if (pageNum >= totalPageCount) //当前页数大于等于最后一页
            {                
                //listProduct = dbEntity.ProductInformations.OrderBy(p => p.CreateTime).Skip(totalCount - lastCount).Take(lastCount).Where(p => p.Deleted == false).ToList();
                listIpBase = dbEntity.GeneralIpBases.OrderBy(p => p.CreateTime).Skip(totalCount - lastCount).Take(lastCount).Where(p => p.Deleted == false).ToList();
                if (totalPageCount == 0)
                {
                    strBack = "0|0|";
                }
                else
                {
                    strBack = totalPageCount.ToString() + "|" + totalPageCount.ToString() + "|";
                }
            }
            else //当前页数小于等于第一页
            {
                if (totalPageCount == 1 || totalPageCount == 0)
                {
                    //listProduct = dbEntity.ProductInformations.OrderBy(p => p.CreateTime).Skip(0).Take(lastCount).ToList();
                    listIpBase = dbEntity.GeneralIpBases.OrderBy(p => p.CreateTime).Skip(0).Take(lastCount).ToList();
                    if (lastCount == 0)
                    {
                        strBack = "0|0|";
                    }
                    else 
                    {
                        strBack = "1|" + totalPageCount.ToString() + "|";
                    }
                }
                else 
                {
                    //listProduct = dbEntity.ProductInformations.OrderBy(p => p.CreateTime).Skip(0).Take(pageCount).ToList();
                    listIpBase = dbEntity.GeneralIpBases.OrderBy(p => p.CreateTime).Skip(0).Take(pageCount).ToList();
                    strBack = "1|" + totalPageCount.ToString() + "|";
                }
            }
            
            //for (int i = 0; i < listProduct.Count; i++)
            //{
            //    strBack = strBack + listProduct.ElementAt(i).Gid.ToString() + "<br />";
            //}
            for (int i = 0; i < listIpBase.Count; i++)
            {
                strBack = strBack + listIpBase.ElementAt(i).Gid.ToString() + "<br />";
            }

            return strBack;
        }

    }
}
