using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveAzure.BLL;
using LiveAzure.Models;
using LiveAzure.Models.Product;
using LiveAzure.Models.Member;
using MVC.Controls;
using MVC.Controls.Grid;
using LiveAzure.Models.General;
using LiveAzure.Models.Shipping;
using LiveAzure.Models.Finance;
using System.IO;
//using System.Text.RegularExpressions;

namespace LiveAzure.Stage.Controllers
{
    public class ProductController : BaseController
    {
        #region 初始化
        private static Guid organizationGuid = Guid.Empty;         //pu所属的组织
        private static Guid selectedProductGuid = Guid.Empty;      //页面选择的PU
        private static Guid selectedPuOnSaleGuid = Guid.Empty;     //上架PU
        private static Guid selectedProductSKUGuid = Guid.Empty;   //页面选择的SKU 
        private static Guid selectedProductOnUnitPrice = Guid.Empty;//选择的上架计量单位
        private static Guid selectedSKUOnsaleGuid = Guid.Empty;     //上架SKU的Guid
        private static bool isFromProductOnSale = false;  //标志是否是从产品列表点的上架
        private static string SearchKey;//搜索关键字
        private static Guid Category;//分类搜索
        public ProductBLL productBLL;
        public DataTransferBLL dataTransferBLL;
        string SpaceMark = "--";

        /// <summary>
        /// 构造函数
        /// </summary>
        public ProductController()
        {
            productBLL = new ProductBLL(dbEntity);
            dataTransferBLL = new DataTransferBLL(dbEntity);
        }

        /// <summary>
        /// 控制器初始化
        /// </summary>
        /// <param name="requestContext"></param>
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            string strControllerName = (string)requestContext.RouteData.Values["Controller"];
            string strActionName = (string)requestContext.RouteData.Values["Action"];
            string strCurrenAction = strControllerName + "." + strActionName;
            base.Initialize(requestContext);
            oEventBLL.WriteEvent("Initialize Load",
                ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, strCurrenAction, CurrentSession.UserID);
        }
        #endregion 初始化



        #region 商品列表
        /// <summary>
        /// 产品基本信息-PU 首页
        /// </summary>
        /// <returns>产品基本信息页面</returns>        
        public ActionResult Index()
        {                       
            // 权限验证
            if (!base.CheckPrivilege())
                return RedirectToAction("ErrorPage", "Home", new { LiveAzure.Resource.Common.NoPermission });
            ViewBag.organizationList = GetSupportOrganizations();//组织下拉框 默认选中当前用户组织
            ProductInformation oViewProductInfomation = new ProductInformation();
            //if (organizationGuid == null || organizationGuid== Guid.Empty)//首次进来默认全局组织为当前用户组织
            organizationGuid = CurrentSession.OrganizationGID;//首次进来默认全局组织为当前用户组织
            oViewProductInfomation.OrgID = organizationGuid;
            SearchKey = null;
            Category = Guid.Empty;
            return View(oViewProductInfomation);
            //if (Guid.Empty == organizationGuid)
            //{
            //    ViewBag.organizationList = GetSupportOrganizations();//orgnization downlist
            //    organizationGuid = CurrentSession.OrganizationGID;
            //}
            //else
            //{
            //    List<MemberOrganization> oUserOrganization = new List<MemberOrganization>();
            //    Guid UserID = (Guid)CurrentSession.UserID;
            //    if (CurrentSession.IsAdmin)//是管理员 则有所有组织权限
            //    {
            //        oUserOrganization = dbEntity.MemberOrganizations
            //                            .Where(o => o.Deleted == false && o.Otype == (byte)ModelEnum.OrganizationType.CORPORATION)
            //                            .OrderByDescending(o => o.Sorting)
            //                            .ToList();
            //    }
            //    else//非管理员
            //    {
            //        try
            //        {
            //            byte IsValde = dbEntity.MemberPrivileges.Where(p => p.UserID == CurrentSession.UserID && p.Ptype == (byte)ModelEnum.UserPrivType.ORGANIZATION).FirstOrDefault().Pstatus;
            //            if (IsValde == 1)//如果是启用状态，则将权限表中查得的组织添加到列表
            //            {
            //                oUserOrganization = (from pi in dbEntity.MemberPrivItems.AsEnumerable()
            //                                     join p in dbEntity.MemberPrivileges.AsEnumerable() on pi.PrivID equals p.Gid
            //                                     join org in dbEntity.MemberOrganizations.AsEnumerable() on pi.RefID equals org.Gid
            //                                     where pi.Deleted == false && p.UserID == UserID && p.Ptype == (byte)ModelEnum.UserPrivType.ORGANIZATION
            //                                     orderby org.Sorting descending
            //                                     select org).ToList();
            //            }
            //            //将自己所属组织加到列表
            //            MemberOrganization currentOrg = dbEntity.MemberOrganizations.Where(o => o.Deleted == false && o.Gid == CurrentSession.OrganizationGID).FirstOrDefault();
            //            oUserOrganization.Add(currentOrg);
            //        }
            //        catch (Exception e)//
            //        {
            //            oEventBLL.WriteEvent(e.ToString());
            //        }
            //    }
            //    List<SelectListItem> ogranizationList = new List<SelectListItem>();
            //    foreach (MemberOrganization item in oUserOrganization)
            //    {
            //        if (item.Gid == organizationGuid)//默认选中自己所属组织
            //            ogranizationList.Add(new SelectListItem { Text = item.FullName.GetResource(CurrentSession.Culture), Value = item.Gid.ToString(), Selected = true });
            //        else
            //            ogranizationList.Add(new SelectListItem { Text = item.FullName.GetResource(CurrentSession.Culture), Value = item.Gid.ToString(), Selected = false });
            //    }
            //    ViewBag.organizationList = ogranizationList;
            //}
            
            ////获取用户的默认组织            
            //MemberUser currentUser = dbEntity.MemberUsers.Where(p => p.Gid == (Guid)CurrentSession.UserID).FirstOrDefault();
            //organizationGuid = currentUser.OrgID;
            ////获取当前用户所属的所有组织
            //if (currentUser.LoginName == "admin")
            //{
            //    List<MemberOrganization> productOrganization = dbEntity.MemberOrganizations.Where(p => p.Deleted == false).ToList();
            //    List<SelectListItem> modelList = new List<SelectListItem>();
            //    foreach (MemberOrganization item in productOrganization)
            //    {
            //        modelList.Add(new SelectListItem { Text = item.FullName.GetResource(CurrentSession.Culture), Value = item.Gid.ToString() });
            //    }
            //    ViewBag.organizationList = modelList;
            //}
            //else
            //{
            //    List<MemberUser> oUser = dbEntity.MemberUsers.Where(p => p.Gid == (Guid)CurrentSession.UserID).ToList();
            //    List<SelectListItem> modelList = new List<SelectListItem>();
            //    foreach (MemberUser item in oUser)
            //    {
            //        MemberOrganization productOrganization = dbEntity.MemberOrganizations.Where(p => p.Deleted == false && p.Gid == item.OrgID).Single();
            //        modelList.Add(new SelectListItem { Text = productOrganization.FullName.GetResource(CurrentSession.Culture), Value = productOrganization.Gid.ToString() });
            //    }
            //    ViewBag.organizationList = modelList;
            //}
              
        }

        /// <summary>
        /// 改变组织
        /// </summary>
        /// <param name="orgSelect">所选择的orgID</param>
        /// <param name="isFromProduct">isFromProduct == true? 从productIndex页面进来：从OnSale页面进来</param>
        /// <returns></returns>
        public ActionResult orgSelect(Guid orgSelect)
        {
            organizationGuid = orgSelect;
            return RedirectToAction("Grid");
        }
        public void SetKey(string Key, Guid Cate)
        {
            SearchKey = Key;
            Category = Cate;
        }
        /// <summary>
        /// PU页面跳转到PU上架页面
        /// 设置选中PU，置标志位
        /// </summary>
        /// <param name="Gid"></param>
        /// <param name="FromPU"></param>
        /// <returns></returns>
        public void ProductPUOnSale(Guid Gid)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                selectedProductGuid = Gid;
                isFromProductOnSale = true;
            }
        }
        /// <summary>
        /// 编辑PU
        /// </summary>
        /// <param name="proGidSelect"></param>
        /// <returns></returns>
        public ActionResult editProductInfo(Guid proGidSelect)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                selectedProductGuid = proGidSelect;
                return RedirectToAction("ProductTab");
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }
        /// <summary>
        /// 增加PU
        /// </summary>
        /// <returns></returns>
        public ActionResult addProductInfo()
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                selectedProductGuid = Guid.Empty;
                return RedirectToAction("ProductTab");
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }
        /// <summary>
        /// 删除PU信息---等待完善,需要级联删除pu,sku,puOnsale,skuOnsale
        /// </summary>
        /// <param name="proGidSelect">待删除PU的GUID</param>
        /// <returns></returns>
        public ActionResult deleteProductInfo(Guid proGidSelect)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                selectedProductGuid = Guid.Empty;
                ProductInformation productDelete = dbEntity.ProductInformations.Where(p => p.Gid == proGidSelect).Single();
                productDelete.Deleted = true;
                dbEntity.SaveChanges();
                return RedirectToAction("Grid");
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }
        /// <summary>
        /// 以grid的形式展示产品基本信息
        /// </summary>
        /// <returns>Grid视图</returns>
        public ViewResult Grid()
        {
            return View();
        }
        /// <summary>
        /// Grid内显示数据处理
        /// </summary>
        /// <param name="searchModel">分页</param>
        /// <returns>传递给grid的json数据</returns>
        public ActionResult ListProductInformation(SearchModel searchModel)
        {           
            IQueryable<ProductInformation> oPrograms;
            if (SearchKey == null)
            {
                if (Category == Guid.Empty)
                {
                    oPrograms = dbEntity.ProductInformations.Include("Name").Include("OnSales").Include("StandardCategory").Include("Brief").Include("Matter").Where(p => p.Deleted == false && p.OrgID == organizationGuid).AsQueryable();
                }
                else
                {
                    oPrograms = dbEntity.ProductInformations.Include("Name").Include("OnSales").Include("StandardCategory").Include("Brief").Include("Matter").Where(p => p.Deleted == false && p.OrgID == organizationGuid && p.StdCatID == Category).AsQueryable();
                }
            }
            else
            {
                if (Category == Guid.Empty)
                {
                    oPrograms = dbEntity.ProductInformations.Include("Name").Include("OnSales").Include("StandardCategory").Include("Brief").Include("Matter").Where(p => p.Deleted == false && p.OrgID == organizationGuid && (p.Name.Matter.Contains(SearchKey) || p.Code.Contains(SearchKey))).AsQueryable();
                }
                else
                {
                    oPrograms = dbEntity.ProductInformations.Include("Name").Include("OnSales").Include("StandardCategory").Include("Brief").Include("Matter").Where(p => p.Deleted == false && p.OrgID == organizationGuid && p.StdCatID == Category && (p.Name.Matter.Contains(SearchKey) || p.Code.Contains(SearchKey))).AsQueryable();
                }
            }
            
            //IQueryable<ProductInformation> oPrograms = dbEntity.ProductInformations.Include("Name").Include("Brief").Include("Matter").Where(p => p.Deleted == false).AsQueryable();
            GridColumnModelList<ProductInformation> columns = new GridColumnModelList<ProductInformation>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Code);
            columns.Add(p => p.Name.GetResource(CurrentSession.Culture)).SetName("Name.Matter");
            columns.Add(p => p.StandardCategory.Name.GetResource(CurrentSession.Culture)).SetName("Category.Matter");
            columns.Add(p => p.OnSales.ToList().Count).SetName("OnSaleCount");
            columns.Add(p => p.ProductModeName);
            columns.Add(p => p.Mode == 0 ? PricePuSku(p.Gid) : PricePuCo(p.Gid)).SetName("Price");
            columns.Add(p => p.MinQuantity);
            columns.Add(p => p.ProductionCycle);
            columns.Add(p => p.GuaranteeDays);

            GridData gridData = oPrograms.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        private string PricePuSku(Guid gid)
        {
            string max="0"; string min="0";
            bool oCurrency = dbEntity.ProductInfoItems.Where(p=>p.ProdID==gid&&p.Deleted==false).Select(p=>p.SuggestPrice.Currency).Contains(CurrentSession.Currency);
            if (oCurrency == true)
            {
                 max = dbEntity.ProductInfoItems.Include("SuggestPrice").Where(p => p.ProdID == gid && p.SuggestPrice.Currency == CurrentSession.Currency&&p.Deleted==false).Select(p => p.SuggestPrice.Cash).ToList().Max().ToString();
                 min = dbEntity.ProductInfoItems.Include("SuggestPrice").Where(p => p.ProdID == gid && p.SuggestPrice.Currency == CurrentSession.Currency&&p.Deleted==false).Select(p => p.SuggestPrice.Cash).ToList().Min().ToString();
            }
            string sCode = dbEntity.GeneralMeasureUnits.Where(p => p.Gid == CurrentSession.Currency&&p.Deleted==false).Select(p => p.Code).FirstOrDefault();
            if (max == min)
            {
                return sCode + max;
            }
            else
            {
                return sCode + min + SpaceMark + sCode + max;
            }
        }
        private string PricePuCo(Guid gid)
        {
            decimal price=0;
            bool bCurrency = dbEntity.ProductInfoItems.Where(p => p.ProdID == gid).Select(p => p.SuggestPrice.Currency).Contains(CurrentSession.Currency);
            if (bCurrency == true)
            {
                List<ProductInfoItem> oPrice = dbEntity.ProductInfoItems.Include("SuggestPrice").Where(p => p.ProdID == gid).ToList();
                foreach (ProductInfoItem item in oPrice)
                {
                    price = +item.SuggestPrice.Cash;
                }
            }
            string sCode = dbEntity.GeneralMeasureUnits.Where(p => p.Gid == CurrentSession.Currency).Select(p => p.Code).FirstOrDefault(); 
            return (sCode+price).ToString();
        }
        /// <summary>
        /// 所属组织的SKU信息列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListProductSKU(SearchModel searchModel)
        {
            IQueryable<ProductInfoItem> oPrograms = dbEntity.ProductInfoItems.Where(p => p.Deleted == false && p.OrgID == organizationGuid && p.ProdID == selectedProductGuid).AsQueryable();
            //string MarketPrice = oGeneralBLL.GetMoneyString(oPrograms.);
            GridColumnModelList<ProductInfoItem> columns = new GridColumnModelList<ProductInfoItem>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Code);
            columns.Add(p => p.Barcode);
            columns.Add(p => p.CodeEx1);
            columns.Add(p => p.FullName.GetResource(CurrentSession.Culture)).SetName("FullName.Matter");
            columns.Add(p => p.ShortName.GetResource(CurrentSession.Culture)).SetName("ShortName.Matter");
            columns.Add(p => p.Percision);
            columns.Add(p => p.aMarketPrice.HasValue ? oGeneralBLL.GetMoneyString(p.aMarketPrice.Value, null) : String.Empty).SetName("MarketPrice.Matter");
            columns.Add(p => p.aSuggestPrice.HasValue ? oGeneralBLL.GetMoneyString(p.aSuggestPrice.Value, null) : String.Empty).SetName("SuggestPrice.Matter");
            columns.Add(p => p.aLowestPrice.HasValue ? oGeneralBLL.GetMoneyString(p.aLowestPrice.Value, null) : String.Empty).SetName("LowestPrice.Matter");
            columns.Add(p => p.GrossWeight);
            columns.Add(p => p.NetWeight);
            columns.Add(p => p.GrossVolume);
            columns.Add(p => p.NetVolume);
            columns.Add(p => p.NetPiece);
            columns.Add(p => p.Remark);

            GridData gridData = oPrograms.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 所属组织上架SKU信息列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListProductSKUOnsale(SearchModel searchModel)
        {
            IQueryable<ProductOnItem> oPrograms = dbEntity.ProductOnItems.Where(p => p.Deleted == false && p.OnSaleID == selectedPuOnSaleGuid).AsQueryable();           
            GridColumnModelList<ProductOnItem> columns = new GridColumnModelList<ProductOnItem>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.SkuID);
            columns.Add(p => p.SkuItem.Code).SetName("Code");            
            columns.Add(p => p.FullName.GetResource(CurrentSession.Culture)).SetName("FullName.Matter");
            columns.Add(p => p.ShortName.GetResource(CurrentSession.Culture)).SetName("ShortName.Matter");
            //columns.Add(p => p.Sorting);
            columns.Add(p => p.SkuItem.StandardUnit.Name.GetResource(CurrentSession.Culture)).SetName("SUName");
            columns.Add(p => p.SetQuantity);
            columns.Add(p => PriceSKUOnsale(p.Gid)).SetName("Price");
            //columns.Add(p => p.MaxQuantity);
            //columns.Add(p => p.OnTheWay);
            //columns.Add(p => p.Overflow);
            //columns.Add(p => p.DependTag);
            //columns.Add(p => p.DependRate);
            //columns.Add(p => p.UseScore);
            //columns.Add(p => p.ScoreDeduct.GetResource(CurrentSession.Currency.Value)).SetName("ScoreDeduct");
            columns.Add(p => p.GetScore);
            //columns.Add(p => p.Remark);
            GridData gridData = oPrograms.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 上架产品的价套价格
        /// </summary>
        /// <param name="SKUonsale">上架SKU</param>
        /// <returns></returns>
        public string PriceSKUOnsale(Guid SKUonsale)
        {
            //cash
            List<ProductOnUnitPrice> Unit = dbEntity.ProductOnUnitPrices.Include("SalePrice").Where(p => p.Deleted == false && p.OnSkuID == SKUonsale && p.SalePrice.Currency == CurrentSession.Currency).ToList();
            //code
            string code = dbEntity.GeneralMeasureUnits.Where(p => p.Gid == CurrentSession.Currency).Select(p => p.Code).FirstOrDefault();
            //money and unit
            string sReturn = "";
            foreach (ProductOnUnitPrice item in Unit)
            {
                sReturn=sReturn+item.SalePrice.Cash.ToString()+item.ShowUnit.Name.GetResource(CurrentSession.Culture)+SpaceMark;
            }
            return code+sReturn;
        }
        /// <summary>
        /// SKU页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductSKU()
        {
            return View();
        }
        /// <summary>
        /// 添加SKU
        /// </summary>
        /// <returns></returns>
        public ActionResult AddProductSKU()
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                MemberOrganization regex = dbEntity.MemberOrganizations.Where(p => p.Gid == organizationGuid).FirstOrDefault();               
                ViewBag.regexCode = regex.SkuCodePolicy;
                ViewBag.regexBarcode = regex.BarcodePolicy;
                
                List<GeneralMeasureUnit> oMeasureUnit = dbEntity.GeneralMeasureUnits.ToList();
                List<SelectListItem> modelList = new List<SelectListItem>();
                foreach (GeneralMeasureUnit item in oMeasureUnit)
                {
                    if (item.Utype != 6)
                    {
                        modelList.Add(new SelectListItem { Text = item.Name.GetResource(CurrentSession.Culture), Value = item.Gid.ToString() });
                    }
                }
                ViewBag.modelList = modelList;
                ProductInfoItem oAdd = new ProductInfoItem
                {
                    FullName = NewResource(ModelEnum.ResourceType.STRING, organizationGuid),
                    ShortName = NewResource(ModelEnum.ResourceType.STRING, organizationGuid),
                    Specification = NewResource(ModelEnum.ResourceType.STRING, organizationGuid),
                    MarketPrice = NewResource(ModelEnum.ResourceType.MONEY, organizationGuid),
                    SuggestPrice = NewResource(ModelEnum.ResourceType.MONEY, organizationGuid),
                    LowestPrice = NewResource(ModelEnum.ResourceType.MONEY, organizationGuid),
                };
                //计件默认值为1
                oAdd.NetPiece = 1;
                return View(oAdd);
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }
        /// <summary>
        /// 编辑SKU
        /// </summary>
        /// <param name="ProductSKUGid"></param>
        /// <returns></returns>
        public ActionResult EditProductSKU(Guid ProductSKUGid)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                MemberOrganization regex = dbEntity.MemberOrganizations.Where(p => p.Gid == organizationGuid).FirstOrDefault();                
                ViewBag.regexBarcode = regex.BarcodePolicy;
                selectedProductSKUGuid = ProductSKUGid;
                List<GeneralMeasureUnit> oMeasureUnit = dbEntity.GeneralMeasureUnits.Where(p => p.Utype != 6).ToList();
                List<SelectListItem> modelList = new List<SelectListItem>();
                foreach (GeneralMeasureUnit item in oMeasureUnit)
                {
                    modelList.Add(new SelectListItem { Text = item.Name.GetResource(CurrentSession.Culture), Value = item.Gid.ToString() });
                }
                ViewBag.modelList = modelList;
                ProductInfoItem oEditProductSKU = dbEntity.ProductInfoItems.Where(p => p.Gid == selectedProductSKUGuid).FirstOrDefault();
                oEditProductSKU.FullName = RefreshResource(ModelEnum.ResourceType.STRING, oEditProductSKU.FullName, organizationGuid);
                oEditProductSKU.ShortName = RefreshResource(ModelEnum.ResourceType.STRING, oEditProductSKU.ShortName, organizationGuid);
                oEditProductSKU.Specification = RefreshResource(ModelEnum.ResourceType.STRING, oEditProductSKU.Specification, organizationGuid);
                oEditProductSKU.MarketPrice = RefreshResource(ModelEnum.ResourceType.MONEY, oEditProductSKU.MarketPrice, organizationGuid);
                oEditProductSKU.SuggestPrice = RefreshResource(ModelEnum.ResourceType.MONEY, oEditProductSKU.SuggestPrice, organizationGuid);
                oEditProductSKU.LowestPrice = RefreshResource(ModelEnum.ResourceType.MONEY, oEditProductSKU.LowestPrice, organizationGuid);
                return View(oEditProductSKU);
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }
        /// <summary>
        /// 保存增加的SKU
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public ActionResult SaveAddProductSKU(ProductInfoItem oAdd,FormCollection Form)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {               
                ProductInfoItem oOld = dbEntity.ProductInfoItems.Where(p=>p.OrgID==organizationGuid && p.Code==oAdd.Code).FirstOrDefault();
                if (oOld == null)
                {
                    ProductInfoItem newProductSKU = new ProductInfoItem
                    {
                        OrgID = organizationGuid,
                        ProdID = selectedProductGuid,
                        Code = Form["Code"],
                        Barcode = Form["Barcode"],
                        CodeEx1 = oAdd.CodeEx1,
                        FullName = new GeneralResource(ModelEnum.ResourceType.STRING, oAdd.FullName),
                        ShortName = new GeneralResource(ModelEnum.ResourceType.STRING, oAdd.ShortName),
                        StdUnit = Guid.Parse(Form["StdUnit"]),
                        Specification = new GeneralResource(ModelEnum.ResourceType.STRING, oAdd.Specification),
                        Percision = oAdd.Percision,
                        MarketPrice = new GeneralResource(ModelEnum.ResourceType.MONEY, oAdd.MarketPrice),
                        SuggestPrice = new GeneralResource(ModelEnum.ResourceType.MONEY, oAdd.SuggestPrice),
                        LowestPrice = new GeneralResource(ModelEnum.ResourceType.MONEY, oAdd.LowestPrice),
                        GrossWeight = oAdd.GrossWeight,
                        NetWeight = oAdd.NetWeight,
                        GrossVolume = oAdd.GrossVolume,
                        NetVolume = oAdd.NetVolume,
                        NetPiece = oAdd.NetPiece,
                        Remark = oAdd.Remark
                    };
                    dbEntity.ProductInfoItems.Add(newProductSKU);
                    dbEntity.SaveChanges();
                    selectedProductSKUGuid = newProductSKU.Gid;
                    return RedirectToAction("ProductSKUGrid");
                }
                else
                {
                    oOld.Deleted = false;
                    oOld.Barcode = Form["Barcode"];
                    oOld.CodeEx1 = oAdd.CodeEx1;
                    oOld.FullName.SetResource(ModelEnum.ResourceType.STRING, oAdd.FullName);
                    oOld.ShortName.SetResource(ModelEnum.ResourceType.STRING, oAdd.ShortName);
                    oOld.StdUnit = Guid.Parse(Form["StdUnit"]);
                    oOld.Specification.SetResource(ModelEnum.ResourceType.STRING, oAdd.Specification);
                    oOld.Percision = oAdd.Percision;
                    oOld.MarketPrice.SetResource(ModelEnum.ResourceType.MONEY, oAdd.MarketPrice);
                    oOld.SuggestPrice.SetResource(ModelEnum.ResourceType.MONEY, oAdd.SuggestPrice);
                    oOld.LowestPrice.SetResource(ModelEnum.ResourceType.MONEY, oAdd.LowestPrice);
                    oOld.GrossWeight = oAdd.GrossWeight;
                    oOld.NetWeight = oAdd.NetWeight;
                    oOld.GrossVolume = oAdd.GrossVolume;
                    oOld.NetVolume = oAdd.NetVolume;
                    oOld.NetPiece = oAdd.NetPiece;
                    oOld.Remark = oAdd.Remark;
                    dbEntity.SaveChanges();
                    selectedProductSKUGuid = oOld.Gid;
                    return null;
                }
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }
        /// <summary>
        /// 删除SKU--等待完善，需要级联删除skuOnsale
        /// </summary>
        /// <param name="ProductSKUGuid"></param>
        public void deleteProductSKU(Guid ProductSKUGuid)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                ProductInfoItem deleteProSKU = dbEntity.ProductInfoItems.Where(p => p.Gid == ProductSKUGuid).Single();
                deleteProSKU.Deleted = true;
                dbEntity.SaveChanges();
            }
        }
        /// <summary>
        /// 保存编辑过的SKU
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public ActionResult SaveEditProductSKU(ProductInfoItem oEdit,FormCollection Form)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                ProductInfoItem oProductInfoItem = dbEntity.ProductInfoItems.Where(p => p.Gid == selectedProductSKUGuid).Single();
                oProductInfoItem.Barcode = Form["Barcode"];
                oProductInfoItem.CodeEx1 = oEdit.CodeEx1;
                oProductInfoItem.FullName.SetResource(ModelEnum.ResourceType.STRING, oEdit.FullName);
                oProductInfoItem.ShortName.SetResource(ModelEnum.ResourceType.STRING, oEdit.ShortName);
                oProductInfoItem.StdUnit = Guid.Parse(Form["StdUnit"]);
                oProductInfoItem.Specification.SetResource(ModelEnum.ResourceType.STRING, oEdit.Specification);
                oProductInfoItem.Percision = oEdit.Percision;
                oProductInfoItem.MarketPrice.SetResource(ModelEnum.ResourceType.MONEY, oEdit.MarketPrice);
                oProductInfoItem.SuggestPrice.SetResource(ModelEnum.ResourceType.MONEY, oEdit.SuggestPrice);
                oProductInfoItem.LowestPrice.SetResource(ModelEnum.ResourceType.MONEY, oEdit.LowestPrice);
                oProductInfoItem.GrossWeight = oEdit.GrossWeight;
                oProductInfoItem.NetWeight = oEdit.NetWeight;
                oProductInfoItem.GrossVolume = oEdit.GrossVolume;
                oProductInfoItem.NetVolume = oEdit.NetVolume;
                oProductInfoItem.NetPiece = oEdit.NetPiece;
                oProductInfoItem.Remark = oEdit.Remark;
                dbEntity.SaveChanges();
                return RedirectToAction("ProductSKUGrid");
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }
        /// <summary>
        /// SKU列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductSKUGrid()
        {
            ProductInformation oShowPU = dbEntity.ProductInformations.Where(p => p.Gid == selectedProductGuid && p.Deleted == false).FirstOrDefault();
            ViewBag.SKUName = oShowPU.Name.GetResource(CurrentSession.Culture);
            return View(oShowPU);
        }
        /// <summary>
        /// 上架SKU页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductSKUOnsale()
        {
            return View();
        }
        /// <summary>
        /// 上架SKU列表
        /// </summary>
        /// <returns></returns>
        public ActionResult SKUOnsaleGrid()
        {
            ProductOnSale oShowOnsalePU = dbEntity.ProductOnSales.Where(p => p.Gid == selectedPuOnSaleGuid && p.Deleted == false).FirstOrDefault();
            ViewBag.OnsaleSKUName = oShowOnsalePU.Name.GetResource(CurrentSession.Culture);
            return View(oShowOnsalePU);
        }
        /// <summary>
        /// 增加上架SKU
        /// </summary>
        /// <returns></returns>
        public ActionResult SKUOnsaleAdd()
        {
            //not null 
            //Guid OrgOwnUser = dbEntity.MemberUsers.Where(p => p.Gid == CurrentSession.UserID).Select(p => p.OrgID).FirstOrDefault();
            List<Guid> oSKU = dbEntity.ProductInfoItems.Where(p => p.Deleted == false && p.OrgID == organizationGuid).Select(p=>p.Gid).ToList();
            ViewBag.org = organizationGuid;
            return View(oSKU);
        }
        /// <summary>
        /// 编辑上架的SKU
        /// </summary>
        /// <param name="ProductSKUOnsaleGid"></param>
        /// <returns></returns>
        public ActionResult SKUOnsaleEdit(Guid ProductSKUOnsaleGid)
        {
            ProductOnItem oEdit = dbEntity.ProductOnItems.Include("FullName").Include("ShortName").Include("ScoreDeduct").Where(p => p.Gid == ProductSKUOnsaleGid).Single();
            oEdit.FullName = RefreshResource(ModelEnum.ResourceType.STRING, oEdit.FullName,organizationGuid);
            oEdit.ShortName = RefreshResource(ModelEnum.ResourceType.STRING, oEdit.ShortName,organizationGuid);
            oEdit.ScoreDeduct = RefreshResource(ModelEnum.ResourceType.MONEY, oEdit.ScoreDeduct,organizationGuid);
            ViewBag.OnsaleCode = dbEntity.ProductOnSales.Where(p => p.Deleted == false && p.Gid == oEdit.OnSaleID).FirstOrDefault().Code;
            ViewBag.OnsaleSKUName = dbEntity.ProductOnSales.Where(p => p.Deleted == false && p.Gid == oEdit.OnSaleID).FirstOrDefault().Name.GetResource(CurrentSession.Culture);
            return View(oEdit);
        }
        /// <summary>
        /// 保存编辑过的上架SKU
        /// </summary>
        /// <param name="editSKUOnsaleModel"></param>
        /// <returns></returns>
        public void editSKUOnsaleSave(ProductOnItem editSKUOnsaleModel)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                ProductOnItem oNew = dbEntity.ProductOnItems.Where(p => p.Gid == editSKUOnsaleModel.Gid).Single();
                oNew.FullName.SetResource(ModelEnum.ResourceType.STRING, editSKUOnsaleModel.FullName);
                oNew.ShortName.SetResource(ModelEnum.ResourceType.STRING, editSKUOnsaleModel.ShortName);
                oNew.Sorting = editSKUOnsaleModel.Sorting;
                oNew.SetQuantity = editSKUOnsaleModel.SetQuantity;
                oNew.MaxQuantity = editSKUOnsaleModel.MaxQuantity;
                oNew.OnTheWay = editSKUOnsaleModel.OnTheWay;
                oNew.Overflow = editSKUOnsaleModel.Overflow;
                oNew.DependTag = editSKUOnsaleModel.DependTag;
                oNew.DependRate = editSKUOnsaleModel.DependRate;
                oNew.UseScore = editSKUOnsaleModel.UseScore;
                if (oNew.ScoreDeduct == null)
                    oNew.ScoreDeduct = new GeneralResource(ModelEnum.ResourceType.MONEY, editSKUOnsaleModel.ScoreDeduct);
                else
                    oNew.ScoreDeduct.SetResource(ModelEnum.ResourceType.MONEY, editSKUOnsaleModel.ScoreDeduct);
                oNew.GetScore = editSKUOnsaleModel.GetScore;
                dbEntity.SaveChanges();
            }
        }
        /// <summary>
        /// 展示组织下所有的PU
        /// </summary>
        /// <returns>PU列表</returns>
        public ActionResult ShowPUList()
        {
            ProductOnSale oShowOnsalePU = dbEntity.ProductOnSales.Where(p => p.Gid == selectedPuOnSaleGuid && p.Deleted == false).FirstOrDefault();
            ViewBag.OnsaleSKUName = oShowOnsalePU.Name.GetResource(CurrentSession.Culture);
            return View(oShowOnsalePU);
        }
        /// <summary>
        /// 展示被选中pu下的sku
        /// </summary>
        /// <param name="PUSelected">选中的pu</param>
        /// <returns></returns>
        public ActionResult SKUCheckBox(Guid PUSelected)
        {
            List<ProductInfoItem> oSelect = dbEntity.ProductInfoItems.Where(p => p.ProdID == PUSelected && p.Deleted == false).ToList();
            //List<ProductInfoItem> oSelect = dbEntity.ProductInfoItems.Where(p => p.Deleted == false).ToList();
            ViewBag.SKU = oSelect;
            return View();
        }
        /// <summary>
        /// 上架产品删除
        /// </summary>
        /// <param name="SKUOnsaleGuid">选中的要删除的sku</param>
        public void SKUOnsaleDelete(Guid SKUOnsaleGuid)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                ProductOnItem oDelete = dbEntity.ProductOnItems.Include("FullName").Include("ShortName").Where(p => p.Gid == SKUOnsaleGuid).Single();
                oDelete.FullName.Deleted = true;
                oDelete.ShortName.Deleted = true;
                oDelete.Deleted = true;
                dbEntity.SaveChanges();
            }
        }
        /// <summary>
        /// 保存上架SKU
        /// </summary>
        /// <param name="gid">上架SKU</param>
        /// <returns>上架是否成功</returns>
        public bool SKUOnsaleSave(Guid gid)
        {
            ProductInfoItem oSKU = dbEntity.ProductInfoItems.Include("Organization").Include("FullName").Include("ShortName").Where(p => p.Gid == gid).Single();
            ProductOnItem oSKUOld = dbEntity.ProductOnItems.Where(p => p.SkuID == oSKU.Gid && p.OnSaleID == selectedPuOnSaleGuid).FirstOrDefault();
            if (oSKUOld == null)//没有上过架
            {
                ProductOnItem oSKUOnsale = new ProductOnItem
                {
                    OnSaleID = selectedPuOnSaleGuid,
                    SkuID = oSKU.Gid,
                    FullName = new GeneralResource(ModelEnum.ResourceType.STRING, oSKU.FullName),
                    ShortName = new GeneralResource(ModelEnum.ResourceType.STRING, oSKU.ShortName),
                    ScoreDeduct = NewResource(ModelEnum.ResourceType.MONEY, oSKU.OrgID),
                    SetQuantity = 1,
                    MaxQuantity = -1,
                    DependRate = 1
                };
                dbEntity.ProductOnItems.Add(oSKUOnsale);
                dbEntity.SaveChanges();
                ProductOnUnitPrice MainCultureOnUnitPrice = new ProductOnUnitPrice
                {
                    OnSkuID = oSKUOnsale.Gid,
                    aShowUnit = oSKUOnsale.SkuItem.StdUnit,
                    aMarketPrice = oSKUOnsale.SkuItem.aMarketPrice,
                    aSalePrice = oSKUOnsale.SkuItem.aSuggestPrice,
                    IsDefault = true,
                    UnitRatio = 1
                };
                dbEntity.ProductOnUnitPrices.Add(MainCultureOnUnitPrice);
                dbEntity.SaveChanges();
                BackWritePriceOnsale(oSKUOnsale.Gid);//回填PUOnsale价格
            }
            else        //上过架
            {
                oSKUOld.Deleted = false;
                oSKUOld.SetQuantity = 1;
                dbEntity.SaveChanges();
                BackWritePriceOnsale(oSKUOld.Gid);//回填PUOnsale价格
            }
            return true;
        }
        /// <summary>
        /// 回填上架价格
        /// </summary>
        /// <param name="OnItemGuid">SKUOnSale</param>
        /// <returns></returns>
        public bool BackWritePriceOnsale(Guid OnItemGuid)
        {
            Guid onsale = dbEntity.ProductOnItems.Where(p=>p.Gid==OnItemGuid&&p.Deleted==false).Select(p=>p.OnSaleID).FirstOrDefault();            
            ProductOnSale oChange = dbEntity.ProductOnSales.Include("MarketPrice").Include("SalePrice").Where(p => p.Deleted == false && p.Gid == onsale).FirstOrDefault();
            if (oChange.MarketPrice == null)
            {
                oChange.MarketPrice = NewResource(ModelEnum.ResourceType.MONEY,oChange.OrgID);
                dbEntity.SaveChanges();
            }
            if (oChange.SalePrice == null)
            {
                oChange.SalePrice = NewResource(ModelEnum.ResourceType.MONEY,oChange.OrgID);
                dbEntity.SaveChanges();
            }
            List<ProductOnItem> SKUOnsaleList = dbEntity.ProductOnItems.Where(p => p.Deleted == false && p.OnSaleID == oChange.Gid).ToList();
            List<GeneralMeasureUnit>currencyList = oGeneralBLL.GetSupportCurrencies(oChange.OrgID);
            foreach(GeneralMeasureUnit item in currencyList){

                foreach (ProductOnItem item1 in SKUOnsaleList)
                {
                    ProductOnUnitPrice oBack = dbEntity.ProductOnUnitPrices.Include("MarketPrice").Include("SalePrice").Where(p => p.Deleted == false && p.OnSkuID == item1.Gid && p.MarketPrice.Currency == item.Gid && p.SalePrice.Currency == item.Gid).OrderByDescending(p => p.IsDefault).OrderByDescending(p => p.CreateTime).FirstOrDefault();
                    if (oBack != null)
                    {                        
                        if (oChange.MarketPrice.Cash < oBack.MarketPrice.Cash)
                            oChange.MarketPrice.Cash = oBack.MarketPrice.Cash;
                        if (oChange.SalePrice.Cash < oBack.SalePrice.Cash)
                            oChange.SalePrice.Cash = oBack.SalePrice.Cash;
                    }                    
                    dbEntity.SaveChanges();
                }
            }
            return true;
        }
        /// <summary>
        /// 将页面选中的SKU加入到上架SKU
        /// </summary>
        /// <param name="selectedSKU"></param>
        /// <returns></returns>
        public void SKUOnsaleSave_back(string[] selectedSKU)
        {
            foreach (var item in selectedSKU)
            {
                Guid skuGuid = Guid.Parse(item);    
                ProductInfoItem oSKU = dbEntity.ProductInfoItems.Include("Organization").Include("FullName").Include("ShortName").Where(p => p.Gid == skuGuid).Single();//从SKU表中需要上架的SKU
                ProductOnItem oSKUOld = dbEntity.ProductOnItems.Where(p => p.SkuID == oSKU.Gid && p.OnSaleID == selectedPuOnSaleGuid).FirstOrDefault();//判断SKU是否上过架
                if (oSKUOld == null)//没有上过架
                {
                    ProductOnItem oSKUOnsale = new ProductOnItem
                    {
                        OnSaleID = selectedPuOnSaleGuid,
                        SkuID = oSKU.Gid,
                        FullName = new GeneralResource(ModelEnum.ResourceType.STRING, oSKU.FullName),
                        ShortName = new GeneralResource(ModelEnum.ResourceType.STRING, oSKU.ShortName),
                        ScoreDeduct = NewResource(ModelEnum.ResourceType.MONEY, oSKU.OrgID),
                        SetQuantity = 1,
                        MaxQuantity = -1,
                        DependRate = 1
                    };
                    dbEntity.ProductOnItems.Add(oSKUOnsale);                    
                    dbEntity.SaveChanges();
                    ProductOnUnitPrice MainCultureOnUnitPrice = new ProductOnUnitPrice
                    {
                        OnSkuID = oSKUOnsale.Gid,
                        aShowUnit = oSKUOnsale.SkuItem.StdUnit,
                        aMarketPrice = oSKUOnsale.SkuItem.aMarketPrice,
                        aSalePrice = oSKUOnsale.SkuItem.aSuggestPrice,
                        IsDefault = true,
                        UnitRatio = 1
                    };
                    dbEntity.ProductOnUnitPrices.Add(MainCultureOnUnitPrice);
                    dbEntity.SaveChanges();
                }
                else        //上过架
                {                    
                    oSKUOld.Deleted = false;
                    dbEntity.SaveChanges();
                }
            }
            
        }
        /// <summary>
        /// 关联商品
        /// </summary>
        /// <returns></returns>
        public ActionResult RelationPU()
        {
            ProductOnSale oShowOnsalePU = dbEntity.ProductOnSales.Where(p => p.Gid == selectedPuOnSaleGuid && p.Deleted == false).FirstOrDefault();
            ViewBag.OnsaleSKUName = oShowOnsalePU.Name.GetResource(CurrentSession.Culture);
            return View(oShowOnsalePU);
        }
        /// <summary>
        /// 关联商品列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListRelationPU(SearchModel searchModel)
        {
            IQueryable<ProductOnRelation> oPrograms = dbEntity.ProductOnRelations.Where(p => p.Deleted == false && p.OnSaleID == selectedPuOnSaleGuid).AsQueryable();
            GridColumnModelList<ProductOnRelation> columns = new GridColumnModelList<ProductOnRelation>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.OnRelation.Name.GetResource(CurrentSession.Culture)).SetName("RelationName");
            columns.Add(p => p.Rtype);
            columns.Add(p => p.OnSale.Name.GetResource(CurrentSession.Culture)).SetName("OnSaleName");
            columns.Add(p => p.OnSale.Code);
            //columns.Add(p => p.OnSale.MarketPrice.GetResource(CurrentSession.Currency.Value)).SetName("MarketPrice.Matter");
            //columns.Add(p => p.OnSale.SalePrice.GetResource(CurrentSession.Currency.Value)).SetName("SalePrice.Matter");
            columns.Add(p => p.OnSale.Validity).SetName("Validity");            
            GridData gridData = oPrograms.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 增加关联商品
        /// </summary>
        /// <returns>增加关联商品页</returns>
        //public ActionResult RelationPUAddShow()
        //{
        //    ProductOnSale oNew = new ProductOnSale();
            
        //    return View("RelationPUAdd",oNew);
        //}
        /// <summary>
        /// 关联商品页
        /// </summary>
        /// <returns>关联商品页</returns>
        public ActionResult RelationPUAdd()
        {
            return View();
        }
        /// <summary>
        /// 关联上架商品
        /// </summary>
        /// <returns></returns>
        public ActionResult RelationOnsalePu()
        {
            return View();
        }
        /// <summary>
        /// 上架PU列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListOnsalePU(SearchModel searchModel)
        {
            var oOnSales = from s in dbEntity.ProductOnSales
                           where s.Deleted == false && s.Product.OrgID == organizationGuid
                           select s;
        //var oOnSales2 = from s in dbEntity.ProductOnSales
        //                join p in dbEntity.ProductInformations on s.ProdID equals p.Gid
        //                where s.Deleted == false
        //                        && p.Deleted == false && p.OrgID == organizationGuid
        //                select s;

            GridColumnModelList<ProductOnSale> columns = new GridColumnModelList<ProductOnSale>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Code);
            columns.Add(p => p.Name.GetResource(CurrentSession.Culture)).SetName("Name.Matter");
            GridData gridData = oOnSales.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 删除关联商品
        /// </summary>
        /// <param name="RelationPUGid"></param>
        public void RelationPUDelete(Guid RelationPUGid)
        {
            ProductOnRelation oNew = dbEntity.ProductOnRelations.Where(p => p.Gid == RelationPUGid && p.Deleted == false).FirstOrDefault();
            oNew.Deleted = true;
            dbEntity.SaveChanges();
        }
        /// <summary>
        /// 选择关联商品
        /// </summary>
        /// <param name="GID">选中的PU</param>
        /// <returns></returns>
        public ActionResult RelationPUSelect(Guid GID)
        {
            ProductOnSale oNew = dbEntity.ProductOnSales.Where(p => p.Gid == GID).FirstOrDefault();
            return View("RelationPUAdd",oNew);
        }
        /// <summary>
        /// 保存选中的关联商品
        /// </summary>
        /// <param name="oSave">ProductOnSale oSave</param>
        public void RelationPUSave(Guid pOnRelation, byte pRtype, int pSorting)
        {
            ProductOnRelation oOld = dbEntity.ProductOnRelations.Where(p => p.aOnRelation == pOnRelation && p.OnSaleID==selectedPuOnSaleGuid).FirstOrDefault();
            if (oOld != null)
            {
                if (oOld.Deleted == true)
                    oOld.Deleted = false;
                oOld.Rtype = pRtype;
                oOld.Sorting = pSorting;
            }
            else
            {
                ProductOnRelation oNew = new ProductOnRelation
                {
                    aOnRelation = pOnRelation,
                    OnSaleID = selectedPuOnSaleGuid,
                    Rtype = pRtype,
                    Sorting = pSorting
                };
                dbEntity.ProductOnRelations.Add(oNew);
            }
            dbEntity.SaveChanges();
        }
        #endregion

        #region 陆旻添加，产品基础信息
        /// <summary>
        /// 跳转到基础信息编辑页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductBaseInfo()
        {
            ProductInformation oProductInfo;
            bool bAddOrEdit = true;
            //判断是否为添加新的PU
            if (selectedProductGuid.Equals(Guid.Empty))
            {
                oProductInfo = new ProductInformation { Name = NewResource(ModelEnum.ResourceType.STRING, organizationGuid) };
                bAddOrEdit = true;
            }
            else 
            {
                oProductInfo = dbEntity.ProductInformations.Where(p => p.Gid == selectedProductGuid).Single();
                oProductInfo.Name = RefreshResource(ModelEnum.ResourceType.STRING, oProductInfo.Name, organizationGuid);
                bAddOrEdit = false;
            }
            
            //标准类别下拉框，取自数据库表
            List<SelectListItem> oStdList = new List<SelectListItem>();

            //标准类型3为商品全局标准分类
            List<GeneralStandardCategory> listStd = dbEntity.GeneralStandardCategorys.Where(p => p.Ctype == 3 && p.Deleted == false).ToList();

            int nListCount = listStd.Count;

            for (int i = 0; i < nListCount; i++)
            {
                oStdList.Add(new SelectListItem { Text = listStd.ElementAt(i).Name.GetResource(CurrentSession.Culture), Value = listStd.ElementAt(i).Gid.ToString() });
            }

            //分组下拉框，枚举类型
            List<SelectListItem> oBlockList = new List<SelectListItem>();
            oBlockList = GetSelectList(oProductInfo.BlockList);

            //模式下拉框，枚举类型
            List<SelectListItem> oModeList = new List<SelectListItem>();

            oModeList = GetSelectList(oProductInfo.ProductModeList);

            ////销售类型下拉框，枚举类型
            //List<SelectListItem> oSaleTypeList = new List<SelectListItem>();
            //oSaleTypeList.Add(new SelectListItem { Text = "直营", Value = "0" });

            string strSaltKey = CurrentSession.SaltKey;

            ViewBag.saltKey = strSaltKey;
            ViewBag.oStdList = oStdList;
            ViewBag.oModeList = oModeList;
            ViewBag.oBlockList = oBlockList;
            ViewBag.bAddOrEdit = bAddOrEdit;
            //ViewBag.oSaleTypeList = oSaleTypeList;

            return View(oProductInfo);
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="formCollection"></param>
        public string SaveProductInfo(ProductInformation oProductInfo, FormCollection formCollection) 
        {
            //判断是添加产品还是编辑产品
            bool bNewOrEdit = true;

            ProductInformation oNewProductInfo;

            if (oProductInfo.Gid.Equals(Guid.Empty))
            {
                List<ProductInformation> listPU = dbEntity.ProductInformations.Where(p => p.OrgID == organizationGuid && p.Code == oProductInfo.Code).ToList();
                if (listPU.Count > 0)
                {
                    oNewProductInfo = listPU.ElementAt(0);
                    if (oNewProductInfo.Deleted == false)
                    {
                        return "failure";
                    }
                    else
                    {
                        oNewProductInfo.Deleted = false;
                        oNewProductInfo.Name = oProductInfo.Name;
                        oNewProductInfo.Code = oProductInfo.Code;
                    }
                }
                else
                {
                    oNewProductInfo = new ProductInformation { Name = NewResource(ModelEnum.ResourceType.STRING, organizationGuid) };
                    oNewProductInfo.Name = oProductInfo.Name;
                    oNewProductInfo.Code = formCollection["Code"];
                }
            }
            else 
            {
                oNewProductInfo = dbEntity.ProductInformations.Where(p => p.Gid == selectedProductGuid).Single();
                oNewProductInfo.Name.SetResource(ModelEnum.ResourceType.STRING, oProductInfo.Name);
                bNewOrEdit = false;
            }

            oNewProductInfo.OrgID = organizationGuid;
            oNewProductInfo.StdCatID = Guid.Parse(formCollection["StandardCategory.Name.Matter"]);
            oNewProductInfo.Block = Byte.Parse(formCollection["Block"]);
            oNewProductInfo.Mode = Byte.Parse(formCollection["Mode"]);
            oNewProductInfo.MinQuantity = Decimal.Parse(formCollection["MinQuantity"]);
            oNewProductInfo.ProductionCycle = Int32.Parse(formCollection["ProductionCycle"]);
            oNewProductInfo.GuaranteeDays = Int32.Parse(formCollection["GuaranteeDays"]);
            oNewProductInfo.Keywords = formCollection["Keywords"];
            //oNewProductInfo.SaleType = 0;
            oNewProductInfo.Remark = formCollection["Remark"];
            oNewProductInfo.Picture = oProductInfo.Picture;

            if (bNewOrEdit == true)
            {
                dbEntity.ProductInformations.Add(oNewProductInfo);

                dbEntity.SaveChanges();

                //重新给全局变量赋值
                selectedProductGuid = oNewProductInfo.Gid;
            }
            else 
            {
                dbEntity.SaveChanges();
            }

            return "success";
        }

        public ActionResult ProductTab() 
        {
            return View();
        }

        public ActionResult testindex()
        {
            return View();
        }

        public ActionResult ProductCategoryAttribute() 
        {            
            return View();
        }

        public ActionResult ProductExtendAttributes() 
        {
            List<GeneralOptional> listOptional = dbEntity.GeneralOptionals.Include("Name").Where(p => p.Deleted == false).ToList();

            List<SelectListItem> oPrivateAttributeList = new List<SelectListItem>();

            int nCount = listOptional.Count;

            for (int i = 0; i < nCount; i++)
            {
                oPrivateAttributeList.Add(new SelectListItem { Text = listOptional.ElementAt(i).Name.Matter, Value = listOptional.ElementAt(i).Gid.ToString() });
            }

            ViewBag.oPrivateAttributeList = oPrivateAttributeList;

            ProductInformation oProduct = dbEntity.ProductInformations.Where(p=>p.Gid == selectedProductGuid && p.Deleted == false).FirstOrDefault();
            if(oProduct != null)
            {
                ViewBag.ProductCode = oProduct.Code;
                ViewBag.ProductName = oProduct.Name.GetResource(CurrentSession.Culture);
            }
            return View();
        }

        /// <summary>
        /// 生成扩展分类页面，生成可以选择的私有分类
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductExtendCategories() 
        {
            //List<GeneralPrivateCategory> listPrivateCategory = dbEntity.GeneralPrivateCategorys.Include("Organization").Include("ChildItems").Where(p => p.OrgID == organizationGuid && p.Deleted == false).ToList();
            //List<SelectListItem> oPrivateCategoryList = new List<SelectListItem>();
            //int nCategoryCount = listPrivateCategory.Count;
            //for (int i = 0; i < nCategoryCount; i++)
            //{
            //    oPrivateCategoryList.Add(new SelectListItem { Text = listPrivateCategory.ElementAt(i).Name.GetResource(CurrentSession.Culture), Value = listPrivateCategory.ElementAt(i).Gid.ToString() });
            //}
            //ViewBag.oPrivateCategoryList = oPrivateCategoryList;
            ProductInformation oProduct = dbEntity.ProductInformations.Where(p => p.Gid == selectedProductGuid && p.Deleted == false).FirstOrDefault();
            if (oProduct != null)
            {
                ViewBag.ProductCode = oProduct.Code;
                ViewBag.ProductName = oProduct.Name.GetResource(CurrentSession.Culture);
            }
            ViewBag.OrgID = organizationGuid;
            return View();

        }

        /// <summary>
        /// 返回产品扩展分类的列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ProductCategoryList(SearchModel searchModel) 
        {
            IQueryable<ProductExtendCategory> oProductCategory = dbEntity.ProductExtendCategories.Include("PrivateCategory").Where(p => p.ProdID == selectedProductGuid && p.Deleted == false).AsQueryable();
            GridColumnModelList<ProductExtendCategory> columns = new GridColumnModelList<ProductExtendCategory>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.PrivateCategory.Name.GetResource(CurrentSession.Culture)).SetName("Name.Matter");
            columns.Add(p => p.IsDefault);

            GridData gridData = oProductCategory.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存产品扩展分类
        /// </summary>
        /// <param name="formCollection"></param>
        public void SaveExtendCategory(ProductExtendCategory oBackCategory, FormCollection formCollection) 
        {
            Guid categoryGid = oBackCategory.PrvCatID;

            var existExtendCategory = dbEntity.ProductExtendCategories.Where(p => p.PrvCatID == categoryGid && p.ProdID == selectedProductGuid).ToList();
            //如果新添加的产品分类已经存在，但是deleted是true，则不新添加该分类，而是将deleted置false
            if (existExtendCategory.Count > 0)
            {
                if (existExtendCategory.ElementAt(0).Deleted == true)
                {
                    existExtendCategory.ElementAt(0).Deleted = false;

                    dbEntity.SaveChanges();
                }
            }
            else
            {
                ProductExtendCategory oProductExtendCategory = new ProductExtendCategory();

                oProductExtendCategory.ProdID = selectedProductGuid;

                oProductExtendCategory.PrvCatID = categoryGid;

                dbEntity.ProductExtendCategories.Add(oProductExtendCategory);

                dbEntity.SaveChanges();
            }

        }

        /// <summary>
        /// 删除选中的分类
        /// </summary>
        /// <param name="gid"></param>
        public void DeleteProductCategory(Guid gid) 
        {
            ProductExtendCategory oProductExtendCategory = dbEntity.ProductExtendCategories.Where(p => p.Gid == gid && p.Deleted == false).Single();

            oProductExtendCategory.Deleted = true;

            dbEntity.SaveChanges();

        }

        /// <summary>
        /// 显示扩展分类列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ProductAttributeList(SearchModel searchModel) 
        {
            IQueryable<ProductExtendAttribute> oProductAtrribute = dbEntity.ProductExtendAttributes.Include("Optional").Include("OptionalResult").Where(p => p.ProdID == selectedProductGuid && p.Deleted == false).AsQueryable();
            GridColumnModelList<ProductExtendAttribute> columns = new GridColumnModelList<ProductExtendAttribute>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Optional.Name.GetResource(CurrentSession.Culture)).SetName("Name.Matter");
            columns.Add(p => p.OptResult != null ? p.OptionalResult.Name.GetResource(CurrentSession.Culture) : String.Empty).SetName("OptionalResult");
            columns.Add(p => p.Matter);

            GridData gridData = oProductAtrribute.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProductExtendAttributeOptionalResult() 
        {
            return View();
        }

        /// <summary>
        /// 获取指定optional的下拉框内容
        /// </summary>
        /// <param name="gid">optional的gid</param>
        /// <returns></returns>
        public ActionResult GetGeneralOptionalItem(Guid gid) 
        {
            List<GeneralOptItem> listOptional = dbEntity.GeneralOptItems.Include("Name").Where(p => p.OptID == gid && p.Deleted == false).OrderBy(p => p.Sorting).ToList();

            List<SelectListItem> oOptionalResultList = new List<SelectListItem>();
            
            int nCount = listOptional.Count;

            for (int i = 0; i < nCount; i++)
            {
                oOptionalResultList.Add(new SelectListItem { Text = listOptional.ElementAt(i).Name.GetResource(CurrentSession.Culture), Value = listOptional.ElementAt(i).Gid.ToString() });
            }

            ViewBag.oOptionalResultList = oOptionalResultList;

            return View("ProductExtendAttributeOptionalResult");
        }

        /// <summary>
        /// 获取optional的输入模式
        /// </summary>
        /// <param name="id">optional的gid</param>
        /// <returns></returns>
        public string GetOptionalInputType(Guid id)
        {
            GeneralOptional oGeneralOption = dbEntity.GeneralOptionals.Where(p => p.Gid == id && p.Deleted == false).Single();

            return oGeneralOption.InputMode.ToString();            
        }

        /// <summary>
        /// 保存扩展分类
        /// </summary>
        /// <param name="formCollection"></param>
        public void SaveExtendAttribute(FormCollection formCollection) 
        {
            Guid optItemGid = new Guid();

            bool bInputType = true;

            if (formCollection["productExtendOptionalResult"] != null) 
            {
                optItemGid = Guid.Parse(formCollection["productExtendOptionalResult"]);
                bInputType = false;
            }
              
            Guid optGid = Guid.Parse(formCollection["OptID"]);

            string strOptMatter = formCollection["extendAttributeValue"];

            ProductExtendAttribute oProductExtendAttribute = dbEntity.ProductExtendAttributes.Where(p => p.OptID == optGid && p.ProdID == selectedProductGuid).FirstOrDefault();

            //如果是数据库中没有添加的记录，则新建扩展分类
            if (oProductExtendAttribute == null)
            {
                oProductExtendAttribute = new ProductExtendAttribute();

                oProductExtendAttribute.ProdID = selectedProductGuid;

                oProductExtendAttribute.OptID = optGid;
                //如果是输入框
                if (bInputType == true)
                {
                    oProductExtendAttribute.Matter = strOptMatter;
                }
                else
                {
                    oProductExtendAttribute.OptResult = optItemGid;
                }

                dbEntity.ProductExtendAttributes.Add(oProductExtendAttribute);

                dbEntity.SaveChanges();
            }
            else 
            {
                //如果数据库中原有记录是删除状态，则变为不删除状态
                if (oProductExtendAttribute.Deleted == true)
                {
                    oProductExtendAttribute.Deleted = false;
                }
                //如果是输入框
                if (bInputType == true)
                {
                    oProductExtendAttribute.Matter = strOptMatter;
                }
                else
                {
                    oProductExtendAttribute.OptResult = optItemGid;
                }

                dbEntity.SaveChanges();
            }

        }

        /// <summary>
        /// 删除扩展属性
        /// </summary>
        /// <param name="gid"></param>
        public void DeleteExtendAttribute(Guid gid) 
        {
            ProductExtendAttribute oProductExtendAttribute = dbEntity.ProductExtendAttributes.Where(p => p.Gid == gid).Single();

            oProductExtendAttribute.Deleted = true;

            dbEntity.SaveChanges();
        }

        /// <summary>
        /// 设置默认分类
        /// </summary>
        /// <param name="gid"></param>
        public void SetDefultCategory(Guid gid) 
        {
            ProductExtendCategory oProductExtendCategory = dbEntity.ProductExtendCategories.Where(p => p.Gid == gid && p.Deleted == false).Single();

            if (oProductExtendCategory.IsDefault == false)
            {
                oProductExtendCategory.IsDefault = true;
            }
            else 
            {
                oProductExtendCategory.IsDefault = false;
            }

            dbEntity.SaveChanges();
        }


        #endregion

        #region 产品详细信息
        /// <summary>
        /// 详细信息页
        /// </summary>
        /// <param name="puGID">产品GUID</param>
        /// <returns></returns>
        public ActionResult ProductDetailInfo()
        {
            ProductInformation oProduct = dbEntity.ProductInformations.Where(p => p.Gid == selectedProductGuid && p.Deleted == false).FirstOrDefault();
            if (oProduct != null)
            {
                ViewBag.ProductCode = oProduct.Code;
                ViewBag.ProductName = oProduct.Name.GetResource(CurrentSession.Culture);
            }
            ProductInformation oProductInformation = new ProductInformation { Brief = NewResource(ModelEnum.ResourceType.STRING, organizationGuid) };
            try
            {
                oProductInformation = dbEntity.ProductInformations.Include("Brief").Include("Matter").Where(p => p.Deleted == false && p.Gid == selectedProductGuid).FirstOrDefault();
                oProductInformation.Brief = RefreshResource(ModelEnum.ResourceType.STRING, oProductInformation.Name, organizationGuid);
            }
            catch (Exception)
            {

            }            
            return View(oProductInformation);
        }

        /// <summary>
        /// 保存详情
        /// </summary>
        /// <param name="oProductInfo">产品页面对象</param>
        /// <returns></returns>
        public bool saveProductInfoDetail(ProductInformation model)
        {
            try
            {
                ProductInformation oProductInformation = dbEntity.ProductInformations.Include("Brief").Include("Matter").Where(p => p.Deleted == false && p.Gid == model.Gid).Single();
                oProductInformation.Brief.SetResource(ModelEnum.ResourceType.STRING, model.Brief);
///////////////////////////////////待更改/////////////////////////////////////////////////////
                if (oProductInformation.Matter == null)
                {
                    GeneralLargeObject oClob = new GeneralLargeObject();
                    oClob.CLOB = model.Matter.CLOB;
                    dbEntity.GeneralLargeObjects.Add(oClob);
                    dbEntity.SaveChanges();
                    oProductInformation.aMatter = oClob.Gid;
                }
                else
                {
                    oProductInformation.Matter.CLOB = model.Matter.CLOB;
                }
////////////////////////////////////////////////////////////////////////////////////////////////
                dbEntity.SaveChanges();

            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        #endregion

        #region 上架
        //OnSaleIndex 上架首页（上架程序）
        //ProductOnSaleList PU上架列表
        //ProductOnSaleBaseInfo PU上架基础信息（TAB1）
        //ProductOnSaleUpDate 保存/更新PU上架信息
        //ChoosePu 选择产品来做添加上架信息
        //SetSelectedPuOnSaleGuid 变更当前选择的上架PU.GID
        //ProductOnSaleDetailInfo PU上架详细信息(TAB2)
        //ProductOnUnitPrice 价格套细(TAB4)

        #region PU上架信息
        public bool ChangeFromPU(bool fromPU)
        {
            if (fromPU)
                isFromProductOnSale = true;
            else
                isFromProductOnSale = false;
            return true;
        }

        /// <summary>
        /// 上架首页（上架程序）
        /// </summary>
        /// <returns></returns>
        public ActionResult OnSaleIndex()
        {
            // 权限验证
            string strProgramCode = Request.RequestContext.RouteData.Values["Controller"].ToString() +
                Request.RequestContext.RouteData.Values["Action"].ToString();
            if (!base.Permission(strProgramCode))
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });

            if (organizationGuid == Guid.Empty||organizationGuid==null)//?
            {
                //获取组织
                MemberUser oUser = dbEntity.MemberUsers.Where(u => u.Deleted == false && u.Gid == CurrentSession.UserID).Single();
                organizationGuid = oUser.OrgID;
            }
            ViewBag.organizationList = GetSupportOrganizations();
            ViewBag.isFromProduct = false;
            if (isFromProductOnSale)
                ViewBag.isFromProduct = true;
            return View();
        }

        /// <summary>
        /// PuOnSaleTab页
        /// </summary>
        /// <returns></returns>
        public ActionResult OnSaleTab()
        {
            return View();
        }

        public bool isKeyNull(string key)
        {
            switch (key) {
                case "selectedProductGuid": if (selectedProductGuid == Guid.Empty) return true; else return false;
                case "selectedPuOnSaleGuid": if (selectedPuOnSaleGuid == Guid.Empty) return true; else return false;
            }
            return true;
        }

        /// <summary>
        /// PU上架列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductOnSaleList(SearchModel searchModel, string KeyWord)
        {
           
            //获取产品中上架的商品
            List<ProductOnSale> lProductOnSale = new List<ProductOnSale>();
            if (isFromProductOnSale)//如果是选中product点击上架,则查出该产品所有的上架商品
            {
                try
                {
                    ProductInformation oProduct = dbEntity.ProductInformations.Find(selectedProductGuid);
                    foreach (var productonsale in oProduct.OnSales)
                    {
                        if (productonsale.Deleted == false)
                            lProductOnSale.Add(productonsale);
                    }
                }
                catch (Exception){ }
                isFromProductOnSale = false;
            }
            else
            {
                //获取组织下的所有产品
                List<ProductInformation> listProduct = dbEntity.ProductInformations.Include("OnSales").Where(p => p.Deleted == false && p.OrgID == organizationGuid).ToList();
                foreach (var product in listProduct)
                {
                    foreach (var productonsale in product.OnSales)
                    {
                        if (productonsale.Deleted == false)
                            lProductOnSale.Add(productonsale);
                    }
                }
            }

            if (KeyWord != null)
            {//有搜索条件
                lProductOnSale = (from p in lProductOnSale
                                where p.Code.Contains(KeyWord) || p.Name.GetResource(CurrentSession.Culture).Contains(KeyWord)
                                select p).ToList();
            }

            //获取货币
            //MemberOrganization oOrganization = dbEntity.MemberOrganizations.Include("Cultures").Where(o => o.Deleted == false && o.Gid == organizationGuid).Single();
            //List<Guid> listcurrency = new List<Guid>();
            //foreach (var cul in oOrganization.Cultures)
            //{
            //    if(cul.Ctype == (byte)ModelEnum.CultureType.CURRENCY)
            //        listcurrency.Add(cul.Currency.Gid);
            //}
            
            IQueryable<ProductOnSale> listProductOnSale = lProductOnSale.AsQueryable();
            GridColumnModelList<ProductOnSale> columns = new GridColumnModelList<ProductOnSale>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Channel.FullName.GetResource(CurrentSession.Culture)).SetName("FullName.Matter");
            columns.Add(p => p.Name.GetResource(CurrentSession.Culture)).SetName("Name.Matter");
            columns.Add(p => p.Code);
            columns.Add(p => p.Product.Code);
            columns.Add(p => p.OnSaleStatusName);
            columns.Add(p => p.ProductModeName);
            columns.Add(p => p.Mode == 0 ? PricePuSkuOnsale(p.Gid) : PricePuCoOnsale(p.Gid)).SetName("Price");
            //市场价 销售价 修改为直接从ProductOnUnitPrice里按计量单位，读出价格字符串，如模式0：235-688 最低-最高价格范围, 模式1 还是价格之和 此处不显示
            columns.Add(p => p.CanSplit);
            GridData gridData = listProductOnSale.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        private string PricePuSkuOnsale(Guid gid)
        {
            decimal max = 0; decimal min = 0; string sReturn="";
            List<ProductOnItem> oSKU=dbEntity.ProductOnItems.Where(p => p.OnSaleID == gid && p.Deleted==false).ToList();
            int oSKUNumber = oSKU.Count();
            if (oSKUNumber != 0)
            {
                List<string> oUnit=null;
                foreach (ProductOnItem item1 in oSKU)
                {
                    oUnit = dbEntity.ProductOnUnitPrices.Include("ShowUnit").Where(p => p.OnSkuID == item1.Gid && p.Deleted == false).Select(p => p.ShowUnit.Code).Distinct().ToList();
                }
                foreach (string items in oUnit)
                {
                    foreach (ProductOnItem item in oSKU)
                    {
                        bool oCurrency = dbEntity.ProductOnUnitPrices.Include("SalePrice").Where(p => p.OnSkuID == item.Gid && p.Deleted == false).Select(p => p.SalePrice.Currency).Contains(CurrentSession.Currency);

                        if (oCurrency == true)
                        {
                            List<decimal> mCount = dbEntity.ProductOnUnitPrices.Include("ShowUnit").Include("SalePrice").Where(p => p.OnSkuID == item.Gid && p.ShowUnit.Code == items && p.Deleted == false).Select(p => p.SalePrice.Cash).ToList();
                            if (mCount.Count != 0)
                            {
                                decimal maxCount = mCount.Max();
                                if (maxCount > max)
                                    max = maxCount;
                            }
                            
                            List<decimal> iCount = dbEntity.ProductOnUnitPrices.Include("ShowUnit").Include("SalePrice").Where(p => p.OnSkuID == item.Gid && p.ShowUnit.Code == items && p.Deleted == false).Select(p => p.SalePrice.Cash).ToList();
                            if (iCount.Count != 0)
                            {
                                decimal minCount = iCount.Min();
                                if (min == 0)
                                    min = minCount;
                                if (minCount < min)
                                    min = minCount;
                            }
                            
                        }
                    }
                    string sCode = dbEntity.GeneralMeasureUnits.Where(p => p.Gid == CurrentSession.Currency && p.Deleted == false).Select(p => p.Code).FirstOrDefault();
                    if (max == min)
                    {
                        sReturn = sReturn + sCode + max.ToString()+"/"+items;
                    }
                    else
                    {
                        sReturn = sReturn + sCode + min.ToString() + SpaceMark + sCode + max.ToString() + "/" + items;
                    }
                }
            }
            return sReturn;
        }
        private string PricePuCoOnsale(Guid gid)
        {
            decimal price = 0;string sReturn="0";
            List<ProductOnItem> oSKU = dbEntity.ProductOnItems.Where(p => p.OnSaleID == gid && p.Deleted == false).ToList();
            int oSKUNumber = oSKU.Count();
            if (oSKUNumber != 0)
            {
                List<string> oUnit = null;
                foreach (ProductOnItem item in oSKU)
                {
                    oUnit = dbEntity.ProductOnUnitPrices.Include("ShowUnit").Where(p => p.OnSkuID == item.Gid && p.Deleted == false).Select(p => p.ShowUnit.Code).ToList();
                }
                foreach (string items in oUnit)
                {
                    foreach (ProductOnItem item in oSKU)
                    {
                        bool oCurrency = dbEntity.ProductOnUnitPrices.Include("SalePrice").Where(p => p.OnSkuID == item.Gid && p.Deleted == false).Select(p => p.SalePrice.Currency).Contains(CurrentSession.Currency);

                        if (oCurrency == true)
                        {
                            List<decimal> iMoney = dbEntity.ProductOnUnitPrices.Include("ShowUnit").Include("SalePrice").Where(p => p.OnSkuID == item.Gid && p.ShowUnit.Code == items && p.Deleted == false).Select(p => p.SalePrice.Cash).ToList();
                            foreach (decimal money in iMoney)
                            {
                                price = price + money;
                            }
                        }
                    }
                    string sCode = dbEntity.GeneralMeasureUnits.Where(p => p.Gid == CurrentSession.Currency && p.Deleted == false).Select(p => p.Code).FirstOrDefault();
                    sReturn = sCode + price.ToString()+items+"<br/>";
                }
            }
            return sReturn;
        }
        /// <summary>
        /// PU上架基础信息（TAB1）
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductOnSaleBaseInfo(Guid? GID=null,bool? FromPU= false)
        {
            ProductOnSale oModel = new ProductOnSale { Name = NewResource(ModelEnum.ResourceType.STRING, organizationGuid) };
            ViewBag.isEdit = (GID != null && FromPU == false) == true ? true : false;
            ViewBag.PUName = "";
            ViewBag.PUCode = "";
            //上架渠道下拉框，本组织支持的渠道
            List<SelectListItem> oChlList = new List<SelectListItem>();
            List<MemberOrgChannel> listChl = dbEntity.MemberOrgChannels.Where(c => c.Deleted == false && c.OrgID == organizationGuid).ToList();
            for (int i = 0; i < listChl.Count; i++)
            {
                oChlList.Add(new SelectListItem { Text = listChl.ElementAt(i).Channel.FullName.GetResource(CurrentSession.Culture), Value = listChl.ElementAt(i).ChlID.ToString() });
            }
            ViewBag.orgChannel = oChlList;

            //上架或者下架状态下拉框,枚举
            List<SelectListItem> oStatus = GetSelectList(oModel.OnSaleStatusList);
            ViewBag.PuStatus = oStatus;
            //模式下拉框,枚举
            List<SelectListItem> oMode = GetSelectList(oModel.ProductModeList);
            ViewBag.oMode = oMode;
            //下拉框是否可拆分
            List<SelectListItem> oCanSplit = SelectEnumList(false);
            ViewBag.oCanSplit = oCanSplit;

            if (GID == null && FromPU == false)//增加PU
            {
                return View(oModel);
            }
            else if (GID != null && FromPU == false)//编辑PU
            {
                try
                {
                    oModel = dbEntity.ProductOnSales.Include("Name").Include("Product").Where(p => p.Deleted == false && p.Gid == GID).Single();
                    oModel.Name = RefreshResource(ModelEnum.ResourceType.STRING, oModel.Name, organizationGuid );
                    ViewBag.ChlName = oModel.Channel.FullName.GetResource(CurrentSession.Culture);//2011-9-7
                    ViewBag.PUName = oModel.Product.Name.GetResource(CurrentSession.Culture);
                    ViewBag.PUCode = oModel.Product.Code;
                }
                catch (Exception)
                {
                    //不存在PU
                }
                return View(oModel);
            }
            else//添加产品上架GID为ProductInfoID
            {
                ProductInformation oProduct = dbEntity.ProductInformations.Where(p => p.Deleted == false && GID==null?p.Gid == selectedProductGuid:p.Gid==GID).FirstOrDefault();
                oModel.Product = oProduct;
                oModel.Mode = oProduct.Mode;
                oModel.Name.Matter = oProduct.Name.Matter;
                foreach (var nameres in oProduct.Name.ResourceItems) 
                {
                    var n = oModel.Name.ResourceItems.FirstOrDefault(name => name.Culture == nameres.Culture);
                    if (n != null)
                    {
                        n.Matter = nameres.Matter;
                    }
                }
                oModel.Picture = oProduct.Picture;
                ViewBag.PUName = oProduct.Name.GetResource(CurrentSession.Culture);
                ViewBag.PUCode = oProduct.Code;
                ViewBag.OnsaleSKUName = oModel.Name.GetResource(CurrentSession.Culture);
                return View(oModel);
            }
            
        }

        /// <summary>
        /// 保存/更新PU上架信息
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public bool ProductOnSaleUpDate(ProductOnSale model)
        {
            ProductOnSale oProductOnSale = new ProductOnSale { Name = NewResource(ModelEnum.ResourceType.STRING, organizationGuid) };
            bool isAddNew = dbEntity.ProductOnSales.Where(p=>p.Deleted==false&&p.Gid==model.Gid).FirstOrDefault() == null ? true:false;
            bool hasBefore = dbEntity.ProductOnSales.Where(p => p.ProdID == model.Product.Gid && p.ChlID == model.ChlID && p.Code == model.Code).Count() >= 1 ? true : false;
            if (hasBefore) isAddNew = false;//之前存在 做更新操作
            try
            {
                if (isAddNew)//添加新PU上架信息
                {
                    oProductOnSale.CanSplit = model.CanSplit;
                    oProductOnSale.ChlID = model.ChlID;
                    oProductOnSale.ProdID = model.Product.Gid;
                    oProductOnSale.Code = model.Code;
                    oProductOnSale.DeliveryDays = model.DeliveryDays;
                    oProductOnSale.Picture = model.Picture;
                    oProductOnSale.Mode = model.Mode;
                    oProductOnSale.Ostatus = model.Ostatus;
                    oProductOnSale.Remark = model.Remark;
                    oProductOnSale.SortingClick = model.SortingClick;
                    oProductOnSale.SortingHot = model.SortingHot;
                    oProductOnSale.SortingNew = model.SortingNew;
                    oProductOnSale.SortingPush = model.SortingPush;
                    oProductOnSale.Validity = model.Validity;
                    oProductOnSale.VideoUrl = model.VideoUrl;
                    oProductOnSale.OrgID = organizationGuid;//9-26 BUG--------新增加冗余字段OrgID NOT NULL

                    oProductOnSale.Name=model.Name;
                    dbEntity.ProductOnSales.Add(oProductOnSale);
                    dbEntity.SaveChanges();
                    //自动上架所选PU下面的所有SKU
                    List<ProductInfoItem> oProductInfoItemList = dbEntity.ProductInfoItems.Where(p => p.ProdID == oProductOnSale.ProdID && p.Deleted == false).ToList();
                    foreach (ProductInfoItem item in oProductInfoItemList)
                    {
                        ProductOnItem oNew = new ProductOnItem
                        {
                            OnSaleID = oProductOnSale.Gid,
                            SkuID = item.Gid,
                            aFullName = item.aFullName,
                            aShortName = item.aShortName,
                            Sorting = 0,
                            SetQuantity = 1,
                            MaxQuantity = -1,
                            OnTheWay = false,
                            Overflow = false,
                            DependTag = 0,
                            DependRate = 1
                        };
                        dbEntity.ProductOnItems.Add(oNew);
                        dbEntity.SaveChanges();
                        ProductOnUnitPrice MainCultureOnUnitPrice = new ProductOnUnitPrice
                        {
                            OnSkuID = oNew.Gid,
                            aShowUnit = oNew.SkuItem.StdUnit,
                            aMarketPrice = oNew.SkuItem.aMarketPrice,
                            aSalePrice = oNew.SkuItem.aSuggestPrice,
                            IsDefault = true,
                            UnitRatio = 1
                        };
                        dbEntity.ProductOnUnitPrices.Add(MainCultureOnUnitPrice);
                        dbEntity.SaveChanges();
                    }
                    //变更selectedPuOnSaleGuid
                    selectedPuOnSaleGuid = oProductOnSale.Gid;
                }
                else//更新PU上架信息
                {
                    ProductOnSale oProductOnSaleUpdate = new ProductOnSale { Name = NewResource(ModelEnum.ResourceType.STRING, organizationGuid) };
                    if (!hasBefore)
                    {
                        oProductOnSaleUpdate = dbEntity.ProductOnSales.Include("Name").Where(p => p.Deleted == false && p.Gid == model.Gid).FirstOrDefault();
                        oProductOnSaleUpdate.Name.SetResource(ModelEnum.ResourceType.STRING, model.Name);
                        //oProductOnSaleUpdate.Code = model.Code;
                        //oProductOnSaleUpdate.ChlID = model.ChlID;
                        oProductOnSaleUpdate.Ostatus = model.Ostatus;
                        oProductOnSaleUpdate.Mode = model.Mode;
                        oProductOnSaleUpdate.Validity = model.Validity;
                        oProductOnSaleUpdate.DeliveryDays = model.DeliveryDays;
                        oProductOnSaleUpdate.SortingNew = model.SortingNew;
                        oProductOnSaleUpdate.SortingHot = model.SortingHot;
                        oProductOnSaleUpdate.SortingClick = model.SortingClick;
                        oProductOnSaleUpdate.SortingPush = model.SortingPush;
                        oProductOnSaleUpdate.Picture = model.Picture;
                        oProductOnSaleUpdate.VideoUrl = model.VideoUrl;
                        oProductOnSaleUpdate.Remark = model.Remark;
                    }
                    else
                    {
                        ProductOnSale oProductOnSaleBefore = dbEntity.ProductOnSales.Where(p => p.ProdID == model.Product.Gid && p.ChlID == model.ChlID && p.Code == model.Code).FirstOrDefault();
                        oProductOnSaleBefore.Deleted = false;
                        oProductOnSaleBefore.Name.SetResource(ModelEnum.ResourceType.STRING, model.Name);
                        //oProductOnSaleBefore.Code = model.Code;
                        //oProductOnSaleBefore.ChlID = model.ChlID;
                        oProductOnSaleBefore.Ostatus = model.Ostatus;
                        oProductOnSaleBefore.Mode = model.Mode;
                        oProductOnSaleBefore.Validity = model.Validity;
                        oProductOnSaleBefore.DeliveryDays = model.DeliveryDays;
                        oProductOnSaleBefore.SortingNew = model.SortingNew;
                        oProductOnSaleBefore.SortingHot = model.SortingHot;
                        oProductOnSaleBefore.SortingClick = model.SortingClick;
                        oProductOnSaleBefore.SortingPush = model.SortingPush;
                        oProductOnSaleBefore.Picture = model.Picture;
                        oProductOnSaleBefore.VideoUrl = model.VideoUrl;
                        oProductOnSaleBefore.Remark = model.Remark;
                    }
                    dbEntity.SaveChanges();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool newProductOnLevelDiscount(Guid OnSaleId)
        {
            List<ProductOnLevelDiscount> listPuDiscount = new List<ProductOnLevelDiscount>();
            List<MemberLevel> listMemberLevel = dbEntity.MemberLevels.Where(m => m.Deleted == false && m.OrgID == organizationGuid).ToList();
            if(listMemberLevel.Count > 0)
            {
                foreach(var li in listMemberLevel)
                {
                    
                }
            return true;
            }
            else return false;
        }

        /// <summary>
        /// 
        /// PuOnSale会员等级折扣页
        /// </summary>
        /// <returns></returns>
        public ActionResult ProOnLevelDiscount()
        {
            ProductOnSale oProductOnSale = dbEntity.ProductOnSales.Include("Product").Where(p => p.Gid == selectedPuOnSaleGuid && p.Deleted == false).FirstOrDefault();
            ViewBag.DiscountPUCode = oProductOnSale.Product.Code;
            ViewBag.DiscountPUName = oProductOnSale.Product.Name.GetResource(CurrentSession.Culture);
            return View();
        }

        /// <summary>
        /// PuDiscount列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ProductOnLevelDiscountList(SearchModel searchModel)
        {
            IQueryable<ProductOnLevelDiscount> listPuDiscount = dbEntity.ProductOnLevelDiscounts.Where(p => p.Deleted == false && p.OnSaleID == selectedPuOnSaleGuid && p.UserLevel.OrgID == organizationGuid).OrderBy(m => m.UserLevel.Mlevel).ToList().AsQueryable();
            GridColumnModelList<ProductOnLevelDiscount> columns = new GridColumnModelList<ProductOnLevelDiscount>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.UserLevel.Mlevel);
            columns.Add(p => p.UserLevel.Name.GetResource(CurrentSession.Culture)).SetName("Name.Matter");
            columns.Add(p => p.Discount);
            GridData gridData = listPuDiscount.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// ProductOnLevelDiscount编辑页
        /// </summary>
        /// <param name="ProductOnLevelDiscountGid">选中的ProductOnLevelDiscountGid 若为NULL 则新添加</param>
        /// <returns></returns>
        public ActionResult ProductOnLevelDiscountEdit(Guid? ProductOnLevelDiscountGid=null)
        {
            ProductOnLevelDiscount oProOnLevelDiscount = new ProductOnLevelDiscount();
            if (ProductOnLevelDiscountGid == null)
            {
                //表中已有的折扣
                List<ProductOnLevelDiscount> listPuDiscount = dbEntity.ProductOnLevelDiscounts.Where(p => p.Deleted == false && p.OnSaleID == selectedPuOnSaleGuid && p.UserLevel.OrgID == organizationGuid).ToList();
                //用户等级下拉框，本组织支持的用户等级
                List<SelectListItem> oUserLevelList = new List<SelectListItem>();
                List<MemberLevel> listUserLevel = dbEntity.MemberLevels.Where(m => m.Deleted == false && m.OrgID == organizationGuid).OrderBy(m => m.Mlevel).ToList();
                //当表中已有折扣数和用户等级数量相同 说明都设置过折扣 不能再添加
                if (listPuDiscount.Count == listUserLevel.Count)
                {
                    ViewBag.ErrorMesg = "Can't be Add More";
                    return View("Error");
                }
                bool hasAdd;
                for (int i = 0; i < listUserLevel.Count; i++)
                {
                    hasAdd = false;
                    foreach (var li in listPuDiscount)
                    {
                        if (li.UserLevel.Code == listUserLevel.ElementAt(i).Code)
                            hasAdd = true;
                    }
                    if (!hasAdd)
                        oUserLevelList.Add(new SelectListItem { Text = listUserLevel.ElementAt(i).Name.GetResource(CurrentSession.Culture), Value = listUserLevel.ElementAt(i).Gid.ToString() });
                }
                ViewBag.UserLevel = oUserLevelList;
                //折扣初始值为1
                oProOnLevelDiscount.Discount = 1;
            }
                //编辑
            else
            {
                oProOnLevelDiscount = dbEntity.ProductOnLevelDiscounts.Where(p => p.Gid == ProductOnLevelDiscountGid && p.Deleted == false).FirstOrDefault();//if==null?
                ViewBag.UserLevelName = oProOnLevelDiscount.UserLevel.Name.GetResource(CurrentSession.Culture);
            }
            return View(oProOnLevelDiscount);
        }

        /// <summary>
        /// 添加/更新ProductOnLevelDiscount
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool ProductOnLevelDiscountUpDate(ProductOnLevelDiscount model)
        {
            ProductOnLevelDiscount oProOnLevelDiscount;
            if (model.Gid == Guid.Empty || model.Gid == null)
            {
                oProOnLevelDiscount = new ProductOnLevelDiscount();
                oProOnLevelDiscount.OnSaleID = selectedPuOnSaleGuid;
                oProOnLevelDiscount.aUserLevel = model.aUserLevel;
                oProOnLevelDiscount.Discount = model.Discount;
                dbEntity.ProductOnLevelDiscounts.Add(oProOnLevelDiscount);
            }
            else
            {
                oProOnLevelDiscount = dbEntity.ProductOnLevelDiscounts.Where(p => p.Deleted == false && p.Gid == model.Gid).FirstOrDefault();
                oProOnLevelDiscount.Discount = model.Discount;
            }
            dbEntity.SaveChanges();
            return true;
        }

        /// <summary>
        /// 选择产品来做添加上架信息
        /// </summary>
        /// <returns></returns>
        public ActionResult ChoosePu()
        {
            return View();
        }

        /// <summary>
        /// 变更当前选择的上架PU.GID
        /// </summary>
        /// <param name="GID"></param>
        /// <returns></returns>
        public void SetSelectedPuOnSaleGuid(Guid? GID)
        {
            if(GID == null)
                selectedPuOnSaleGuid = Guid.Empty;
            else
                selectedPuOnSaleGuid = (Guid)GID;
        }

        /// <summary>
        /// 获取当前选择组织的产品列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult PuList(SearchModel searchModel , string KeyWord)
        {
            IQueryable<ProductInformation> lProductList = dbEntity.ProductInformations.Where(p => p.Deleted == false && p.OrgID == organizationGuid).ToList().AsQueryable();
            if (KeyWord!=null)
            {
                lProductList = (from p in lProductList
                               where  p.Code.Contains(KeyWord) || p.Name.GetResource(CurrentSession.Culture).Contains(KeyWord)
                               select p).ToList().AsQueryable();
            }
            GridColumnModelList<ProductInformation> columns = new GridColumnModelList<ProductInformation>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Code);
            columns.Add(p => p.Name.GetResource(CurrentSession.Culture)).SetName("Name.Matter");
            columns.Add(p => p.StandardCategory.Name.GetResource(CurrentSession.Culture)).SetName("Category.Matter");
            columns.Add(p => p.Mode);
            GridData gridData = lProductList.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 上架商品承运商
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductOnsaleShipping()
        {
            ProductOnSale oProductOnSale = dbEntity.ProductOnSales.Include("Product").Where(p => p.Gid == selectedPuOnSaleGuid && p.Deleted == false).FirstOrDefault();
            ViewBag.PaymentPUCode = oProductOnSale.Product.Code;
            ViewBag.PaymentPUName = oProductOnSale.Product.Name.GetResource(CurrentSession.Culture);
            return View();
        }
        /// <summary>
        /// 上架商品支付方式 
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductOnsalePayment()
        {
            ProductOnSale oProductOnSale = dbEntity.ProductOnSales.Include("Product").Where(p => p.Gid == selectedPuOnSaleGuid && p.Deleted == false).FirstOrDefault();
            ViewBag.PaymentPUCode = oProductOnSale.Product.Code;
            ViewBag.PaymentPUName = oProductOnSale.Product.Name.GetResource(CurrentSession.Culture);
            return View();
        }
        /// <summary>
        /// 上架商品支付方式编辑
        /// </summary>
        /// <param name="bAddOrEdit"></param>
        /// <returns></returns>
        public ActionResult ProductOnsalePaymentAddOrEdit(bool bAddOrEdit, Guid? gid)
        {
            ProductOnPayment oNewProductOnsalePayment = new ProductOnPayment();
            List<FinancePayType> listFinancePayType = dbEntity.FinancePayTypes.Include("Name").Where(p => p.Deleted == false && p.OrgID == organizationGuid).ToList();
            int nPaymentCount = listFinancePayType.Count;
            List<SelectListItem> oPaymentList = new List<SelectListItem>();
            for (int i = 0; i < nPaymentCount; i++)
            {
                oPaymentList.Add(new SelectListItem { Text = listFinancePayType.ElementAt(i).Name.Matter, Value = listFinancePayType.ElementAt(i).Gid.ToString() });
            }

            if (bAddOrEdit == false)
            {
                oNewProductOnsalePayment = dbEntity.ProductOnPayments.Include("OnSale").Include("PayType").Where(p => p.Gid == gid && p.Deleted == false).FirstOrDefault();
            }
            else
            {
                ProductOnSale oCurrentProductOnsale = dbEntity.ProductOnSales.Where(p => p.Gid == selectedPuOnSaleGuid && p.Deleted == false).FirstOrDefault();
                oNewProductOnsalePayment.OnSale = oCurrentProductOnsale;
                oNewProductOnsalePayment.OnSaleID = oCurrentProductOnsale.Gid;
            }

            ViewBag.oPaytypeList = oPaymentList;
            ViewBag.bAddOrEdit = bAddOrEdit;

            return View(oNewProductOnsalePayment);
        }

        #endregion

        #region 承运商
        /// <summary>
        /// 查询商品承运商
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListOnsaleShipping(SearchModel searchModel)
        {
            IQueryable<ProductOnShipping> oProductShipping = dbEntity.ProductOnShippings.Include("OnSale").Include("Shipper").Where(p => p.Deleted == false && p.OnSaleID == selectedPuOnSaleGuid).AsQueryable();

            GridColumnModelList<ProductOnShipping> columns = new GridColumnModelList<ProductOnShipping>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Shipper.FullName.Matter);
            columns.Add(p => p.ShipWeight);
            columns.Add(p => p.SolutionName);
            columns.Add(p => p.Condition);
            columns.Add(p => p.Discount);
            columns.Add(p => p.SupportCod);
            columns.Add(p => p.Remark);

            GridData gridData = oProductShipping.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 保存上架商品的承运商信息
        /// </summary>
        /// <param name="oProductShipping"></param>
        /// <param name="formCollection"></param>
        public ActionResult SaveOnsaleShipping(ProductOnShipping oProductShipping, FormCollection formCollection)
        {
            bool bNewOnsaleShipping = false;
            Guid shippingGid;            
            ProductOnShipping oNewProductShipping = new ProductOnShipping();
            //新建上架的商品的承运商
            if (oProductShipping.Gid.Equals(Guid.Empty))
            {

                shippingGid = Guid.Parse(formCollection["Shipper.FullName.Matter"]);

                List<ProductOnShipping> existShipping = dbEntity.ProductOnShippings.Where(p => p.ShipID == shippingGid && p.OnSaleID == selectedPuOnSaleGuid).ToList();
                //如果是删除的承运商信息，则更新后，将deleted变为false
                if (existShipping.Count > 0)
                {
                    oNewProductShipping = existShipping.ElementAt(0);
                    oNewProductShipping.OnSaleID = selectedPuOnSaleGuid;
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
                    oNewProductShipping.OnSaleID = selectedPuOnSaleGuid;
                    oNewProductShipping.ShipWeight = Int32.Parse(formCollection["ShipWeight"]);
                    oNewProductShipping.Solution = Byte.Parse(formCollection["Solution"]);
                    oNewProductShipping.Discount = Decimal.Parse(formCollection["Discount"]);
                    oNewProductShipping.Condition = Decimal.Parse(formCollection["Condition"]);
                    oNewProductShipping.SupportCod = Boolean.Parse(formCollection["SupportCod"]);
                    oNewProductShipping.ShipID = shippingGid;
                    oNewProductShipping.Remark = formCollection["Remark"];
                    dbEntity.ProductOnShippings.Add(oNewProductShipping);
                    bNewOnsaleShipping = true;
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
                oEditProductShipping.Remark = formCollection["Remark"];

            }

            dbEntity.SaveChanges();

            //需要默认添加承运商支持的区域，地区为承运商中Location的递归最上级地区
            if (bNewOnsaleShipping == true)
            {
                shippingGid = Guid.Parse(formCollection["Shipper.FullName.Matter"]);
                ShippingInformation oCurrentShipper = dbEntity.ShippingInformations.Where(p => p.Gid == shippingGid && p.Deleted == false).FirstOrDefault();
                if (oCurrentShipper != null)
                {
                    if (oCurrentShipper.aLocation != null)
                    {
                        Guid location = (Guid)oCurrentShipper.aLocation;
                        List<GeneralRegion> oFullRegionList = new List<GeneralRegion>();
                        List<Guid> oFullRegionGidList = dbEntity.Database.SqlQuery<Guid>("SELECT Gid FROM fn_FindFullRegions({0})", location).ToList();
                        ProductOnShipArea oProductShipArea = new ProductOnShipArea();
                        oProductShipArea.OnShip = oNewProductShipping.Gid;
                        oProductShipArea.RegionID = oFullRegionGidList.ElementAt(oFullRegionGidList.Count - 1);
                        dbEntity.ProductOnShipAreas.Add(oProductShipArea);
                        dbEntity.SaveChanges();
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// 上架商品承运商添加和编辑
        /// </summary>
        /// <param name="bAddorEdit"></param>
        /// <returns></returns>
        public ActionResult ProductOnsaleShippingAddOrEdit(bool bAddorEdit, Guid? gid)
        {
            ProductOnShipping oNewShipping = new ProductOnShipping();

            ProductOnSale oProductOnsale = dbEntity.ProductOnSales.Where(p => p.Deleted == false && p.Gid == selectedPuOnSaleGuid).FirstOrDefault();

            //编辑状态
            if (bAddorEdit == false)
            {
                oNewShipping = dbEntity.ProductOnShippings.Include("OnSale").Include("Shipper").Include("OnShipArea").Where(p => p.Gid == gid && p.Deleted == false).FirstOrDefault();
            }
            else
            {
                oNewShipping.OnSaleID = selectedPuOnSaleGuid;
                oNewShipping.OnSale = oProductOnsale;
                //承运商的折扣默认值为1
                oNewShipping.Discount = 1;
            }

            List<SelectListItem> oShipperList = new List<SelectListItem>();

            List<ShippingInformation> listShipping = dbEntity.ShippingInformations.Where(p => p.Deleted == false && p.aParent == organizationGuid).ToList();

            int nShippingCount = listShipping.Count;

            for (int i = 0; i < nShippingCount; i++)
            {
                oShipperList.Add(new SelectListItem { Text = listShipping.ElementAt(i).FullName.Matter, Value = listShipping.ElementAt(i).Gid.ToString() });
            }

            List<SelectListItem> oSolutionList = new List<SelectListItem>();
            oSolutionList = GetSelectList(oNewShipping.SolutionList);

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
            IQueryable<ProductOnPayment> oProductOnsalePayment = dbEntity.ProductOnPayments.Include("OnSale").Include("PayType").Where(p => p.Deleted == false && p.OnSaleID == selectedPuOnSaleGuid).AsQueryable();

            GridColumnModelList<ProductOnPayment> columns = new GridColumnModelList<ProductOnPayment>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.PayType.Name.GetResource(CurrentSession.Culture)).SetName("PayType");
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
                    oNewProductOnsalePayment.PayID = currentPayID;
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
        //全局记录选择的上架承运商的gid
        private static Guid ProductOnsaleShippingGid = new Guid();
        /// <summary>
        /// 加载完整的地区树
        /// </summary>
        /// <returns></returns>
        public string ShippingRegionTreeLoad()
        {
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
            string strTreeJson = CreateTree(list);
            return strTreeJson;
        }
        /// <summary>
        /// 展开地区树状结构
        /// </summary>
        /// <param name="id">地区的gid</param>
        /// <returns></returns>
        public string ShippingRegionTreeExpand(Guid id)
        {
            List<LiveTreeNode> list = new List<LiveTreeNode>();
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
            }
            return list.ToJsonString();
        }
        /// <summary>
        /// 添加承运商支持的区域
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string AddShippingRegion(Guid id)
        {
            ProductOnShipArea oNewProductShipArea = new ProductOnShipArea();

            List<ProductOnShipArea> listProductShipArea = dbEntity.ProductOnShipAreas.Where(p => p.OnShip == ProductOnsaleShippingGid && p.RegionID == id).ToList();

            bool bSuccess = false;

            if (!id.Equals(Guid.Empty))
            {
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
                    ProductOnShipping oProductOnsaleShipping = dbEntity.ProductOnShippings.Where(p => p.Gid == ProductOnsaleShippingGid && p.Deleted == false).SingleOrDefault();
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
            }
            //如果保存成功则返回success，否则返回failure
            if (bSuccess == true)
            {
                return "success";
            }
            else
            {
                return "failure";
            }
        }
        /// <summary>
        /// 删除承运商支持的地区
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string DeleteShippingRegion(Guid id)
        {
            ProductOnShipArea oProductShipArea = dbEntity.ProductOnShipAreas.Where(p => p.OnShip == ProductOnsaleShippingGid && p.RegionID == id && p.Deleted == false).FirstOrDefault();

            if (oProductShipArea != null)
            {
                oProductShipArea.Deleted = true;

                dbEntity.SaveChanges();
            }

            return "success";
        }
        //承运商支持的区域是否必须属于shiparea表;默认不属于;
        private static bool bInShipArea = false;
        /// <summary>
        /// 设置承运商支持的区域是否必须属于shiparea表
        /// </summary>
        /// <returns></returns>
        public bool SetInShipArea()
        {
            bInShipArea = !bInShipArea;

            return bInShipArea;
        }

        public ActionResult ProductOnshipArea(Guid gid)
        {
            ProductOnsaleShippingGid = gid;

            return View();
        }
        
        #endregion

        #region 价格套细

        public ActionResult PriceUnit(Guid SkuId,Guid SkuOnSaleId)
        {
            selectedProductSKUGuid = SkuId;
            selectedSKUOnsaleGuid = SkuOnSaleId;
            ViewBag.SKUCode = dbEntity.ProductInfoItems.Find(SkuId).Code;
            ViewBag.OnSkuName = dbEntity.ProductOnItems.Find(SkuOnSaleId).FullName.GetResource(CurrentSession.Culture);
            return View();
        }
        public class UnitPrice
        {
            public SessionData currentSession
            {
                get;
                set;
            }
            public List<MemberOrgCulture> memberOrgCulture
            {
                get;
                set;
            }
            public List<ProductOnUnitPrice> ProductOnUnitPrice
            {
                get;
                set;
            }
            public GeneralMeasureUnit StdUnit
            {
                get;
                set;
            }
            public override string ToString()
            {
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                builder.Append('[');
                foreach (var proOnUnitPrice in ProductOnUnitPrice)
                {
                    builder.Append('{');
                    builder.AppendFormat("Gid:\"{0}\"", proOnUnitPrice.Gid);
                    builder.Append(',');
                    builder.AppendFormat("\"{1}\":\"{0}\"", proOnUnitPrice.ShowUnit.Name.GetResource(currentSession.Culture), LiveAzure.Resource.Model.Product.ProductOnUnitPrice.ShowUnit);
                    builder.Append(',');
                        int Currencylength = memberOrgCulture.Count;
                        for (int i = 0; i < Currencylength; i++)
                        {
                            decimal? marketPrice = null;
                            decimal? salePrice = null;
                            Guid PriceItem = memberOrgCulture.ElementAt(i).Currency.Gid;
                            if (proOnUnitPrice.MarketPrice.Currency == PriceItem)
                                marketPrice = proOnUnitPrice.MarketPrice.Cash;
                            else
                            {
                                foreach (var currencyItem in proOnUnitPrice.MarketPrice.ResourceItems)
                                {
                                    if (currencyItem.Currency == PriceItem)
                                        marketPrice = currencyItem.Cash;
                                }
                            }
                            if (proOnUnitPrice.SalePrice.Currency == PriceItem)
                                salePrice = proOnUnitPrice.SalePrice.Cash;
                            else
                            {
                                foreach (var currencyItem in proOnUnitPrice.SalePrice.ResourceItems)
                                {
                                    if (currencyItem.Currency == PriceItem)
                                        salePrice = currencyItem.Cash;
                                }
                            }
                            builder.AppendFormat("\"{0}\":\"{1}|{2}\"",
                                memberOrgCulture.ElementAt(i).Currency.Code+LiveAzure.Resource.Model.Product.ProductOnUnitPrice.MarketPrice+"|"+LiveAzure.Resource.Model.Product.ProductOnUnitPrice.SalePrice,
                                marketPrice == null ? "" : marketPrice.ToString(),
                                salePrice == null ? "" : salePrice.ToString());
                            builder.Append(',');
                    }
                    builder.AppendFormat("\"{1}\":\"{0}\"", StdUnit.Name.GetResource(currentSession.Culture), LiveAzure.Resource.Model.Product.ProductInfoItem.StdUnit);
                    builder.Append(',');
                    builder.AppendFormat("\"{1}\":\"{0}\"", proOnUnitPrice.UnitRatio, LiveAzure.Resource.Model.Product.ProductOnUnitPrice.UnitRatio);
                    builder.Append(',');
                    builder.AppendFormat("\"{1}\":\"{0}\"", proOnUnitPrice.Percision, LiveAzure.Resource.Model.Product.ProductOnUnitPrice.Percision);
                    builder.Append(',');
                    builder.AppendFormat("\"{1}\":\"{0}\"", proOnUnitPrice.IsDefault, LiveAzure.Resource.Model.Product.ProductOnUnitPrice.IsDefault);
                    builder.Append('}');
                    if (proOnUnitPrice.Gid != ProductOnUnitPrice[ProductOnUnitPrice.Count - 1].Gid)
                        builder.Append(',');
                }
                builder.Append(']');
                return builder.ToString();
            }
        }
        public class JqGridColumns
        {
            public string name { get; set; }
            public string index { get; set; }
            public string width { get; set; }
            public string align { get; set; }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        public JsonResult GetGridData()
        {
            Guid skuGid = selectedProductSKUGuid;
            Guid onSkuId = selectedSKUOnsaleGuid;
            UnitPrice PriceData = new UnitPrice();
            PriceData.currentSession = CurrentSession;
            PriceData.memberOrgCulture = dbEntity.MemberOrgCultures.Include("Currency").Where(p => p.OrgID == organizationGuid && p.Ctype == 1 && p.aCurrency != Guid.Empty).OrderBy(p => p.Sorting).ToList();
            PriceData.ProductOnUnitPrice = dbEntity.ProductOnUnitPrices.Where(p => p.Deleted == false && p.OnSkuID == onSkuId).ToList();
            PriceData.StdUnit = dbEntity.ProductInfoItems.Include("StandardUnit").Where(p => p.Deleted == false && p.Gid == skuGid).FirstOrDefault().StandardUnit;
            //return PriceData.ToString();
            return Json(PriceData.ToString(), JsonRequestBehavior.AllowGet);
        }

        public string GetColumnSettings()
        {
            string strSettings = "";
            string strColumnNames = "";
            Guid skuGid = selectedProductSKUGuid;
            Guid orgId = organizationGuid;
            GeneralMeasureUnit oGeneralMesureUnit = new GeneralMeasureUnit();
            //取出组织所对应的货币，取自MemberOrgCulture
            List<MemberOrgCulture> oMemberOrgCulture = dbEntity.MemberOrgCultures.Include("Currency").Where(p => p.OrgID == orgId && p.Ctype == 1 && p.aCurrency != Guid.Empty).OrderBy(p => p.Sorting).ToList();
            //根据取出的货币数，生成货币的columnmodel
            List<JqGridColumns> listColumnModel = new List<JqGridColumns>();
            int nMemberCultureListCount = oMemberOrgCulture.Count;
            for (int i = 0; i < nMemberCultureListCount; i++)
            {
                JqGridColumns oColumnModel = new JqGridColumns();
                oColumnModel.name = oMemberOrgCulture.ElementAt(i).Currency.Code + LiveAzure.Resource.Model.Product.ProductOnUnitPrice.MarketPrice + "|" + LiveAzure.Resource.Model.Product.ProductOnUnitPrice.SalePrice;
                oColumnModel.index = oMemberOrgCulture.ElementAt(i).Currency.Code;
                oColumnModel.width = "80";
                oColumnModel.align = "center";
                listColumnModel.Add(oColumnModel);
            }

            //除了自动生成的货币列之外，还有必然存在的三列 + ISDEFAULT
            //显示计量单位，即销售的计量单位
            JqGridColumns standardColumn = new JqGridColumns();
            standardColumn.name = LiveAzure.Resource.Model.Product.ProductInfoItem.StdUnit;// "StdUnit";
            standardColumn.index = "StdUnit";
            standardColumn.width = "40";
            standardColumn.align = "center";
            listColumnModel.Add(standardColumn);

            //转换比率
            JqGridColumns unitRadioColumn = new JqGridColumns();
            unitRadioColumn.name = LiveAzure.Resource.Model.Product.ProductOnUnitPrice.UnitRatio;//"UnitRatio";
            unitRadioColumn.index = "UnitRatio";
            unitRadioColumn.width = "20";
            unitRadioColumn.align = "right";
            listColumnModel.Add(unitRadioColumn);

            //计量精度
            JqGridColumns percisionColumn = new JqGridColumns();
            percisionColumn.name = LiveAzure.Resource.Model.Product.ProductOnUnitPrice.Percision;//"Percision";
            percisionColumn.index = "Percision";
            percisionColumn.width = "10";
            percisionColumn.align = "right";
            listColumnModel.Add(percisionColumn);

            //IsDefault
            JqGridColumns isDefaultColumn = new JqGridColumns();
            isDefaultColumn.name = LiveAzure.Resource.Model.Product.ProductOnUnitPrice.IsDefault;//"Percision";
            isDefaultColumn.index = "isDefault";
            isDefaultColumn.width = "10";
            isDefaultColumn.align = "center";
            listColumnModel.Add(isDefaultColumn);
            
            //=======================================================================================================
            for (int i = 0; i < listColumnModel.Count; i++)
            {
                JqGridColumns currentColumn = listColumnModel.ElementAt(i);
                strSettings += "{ \"name\": \"" + currentColumn.name + "\",\"index\": \"" + currentColumn.index + "\",\"width\": \"" + currentColumn.width + "\",\"align\": \"" + currentColumn.align + "\"},";
                strColumnNames += "\"" + currentColumn.name + "\",";
            }
            strSettings = "[" + "{ \"name\": \"Gid\",\"index\": \"Gid\",\"width\": \"80\" , \"hidden\":true},{\"name\": \"" + LiveAzure.Resource.Model.Product.ProductOnUnitPrice.ShowUnit + "\",\"index\": \"ShowUnit\",\"width\": \"80\"}," + strSettings.Substring(0, strSettings.Length - 1) + "]" + "!" + "[" + "\"Gid\"," + "\"" + LiveAzure.Resource.Model.Product.ProductOnUnitPrice.ShowUnit + "\"," + strColumnNames.Substring(0, strColumnNames.Length - 1) + "]";

            return strSettings;
        }
        /// <summary>
        /// 进入价套编辑页面
        /// </summary>
        /// <param name="bAddOrEdit"></param>
        /// <param name="unitPriceGid"></param>
        /// <returns></returns>
        public ActionResult ProductOnsalePriceUnitAddOrEdit(bool bAddOrEdit, Guid? unitPriceGid)
        {
            ProductOnItem oProductOnItem = dbEntity.ProductOnItems.Include("OnSale").Include("SkuItem").Include("FullName").Include("ShortName").Where(p => p.Deleted == false && p.Gid == selectedSKUOnsaleGuid).SingleOrDefault();

            ProductOnUnitPrice oNewProductOnsaleUnitPrice;
            List<SelectListItem> oIsDefaultList = new List<SelectListItem>();
            if (bAddOrEdit == true)
            {
                //添加状态
                oNewProductOnsaleUnitPrice = new ProductOnUnitPrice { SalePrice = NewResource(ModelEnum.ResourceType.MONEY, organizationGuid), MarketPrice = NewResource(ModelEnum.ResourceType.MONEY, organizationGuid) };
                
                oNewProductOnsaleUnitPrice.OnSkuID = selectedSKUOnsaleGuid;
                oNewProductOnsaleUnitPrice.OnSkuItem = oProductOnItem;
                oIsDefaultList.Add(new SelectListItem { Text = LiveAzure.Resource.Common.Yes, Value = ModelEnum.YesNo.YES.ToString() });
                oIsDefaultList.Add(new SelectListItem { Text = LiveAzure.Resource.Common.No, Value = ModelEnum.YesNo.NO.ToString() });              
                
            }
            else
            {
                oNewProductOnsaleUnitPrice = dbEntity.ProductOnUnitPrices.Include("OnSkuItem").Include("ShowUnit").Include("MarketPrice").Include("SalePrice").Where(p => p.Deleted == false && p.Gid == unitPriceGid).SingleOrDefault();
                oNewProductOnsaleUnitPrice.SalePrice = RefreshResource(ModelEnum.ResourceType.MONEY, oNewProductOnsaleUnitPrice.SalePrice, organizationGuid);
                oNewProductOnsaleUnitPrice.MarketPrice = RefreshResource(ModelEnum.ResourceType.MONEY, oNewProductOnsaleUnitPrice.MarketPrice, organizationGuid);

                if (oNewProductOnsaleUnitPrice.IsDefault.Equals(ModelEnum.YesNo.YES))
                {
                    oIsDefaultList.Add(new SelectListItem { Text = LiveAzure.Resource.Common.Yes, Value = ModelEnum.YesNo.YES.ToString() });
                    oIsDefaultList.Add(new SelectListItem { Text = LiveAzure.Resource.Common.No, Value = ModelEnum.YesNo.NO.ToString() });
                }
                else 
                {
                    oIsDefaultList.Add(new SelectListItem { Text = LiveAzure.Resource.Common.No, Value = ModelEnum.YesNo.NO.ToString() });
                    oIsDefaultList.Add(new SelectListItem { Text = LiveAzure.Resource.Common.Yes, Value = ModelEnum.YesNo.YES.ToString() });
                }
            }

            List<GeneralMeasureUnit> listMeasureUnit = dbEntity.GeneralMeasureUnits.Include("Name").Where(p => p.Deleted == false && p.Utype != 6).ToList();
            List<SelectListItem> oMeasureUnitList = new List<SelectListItem>();
            for (int i = 0; i < listMeasureUnit.Count; i++)
            {
                oMeasureUnitList.Add(new SelectListItem { Text = listMeasureUnit.ElementAt(i).Name.GetResource(CurrentSession.Culture), Value = listMeasureUnit.ElementAt(i).Gid.ToString() });
            }
            
            
            ViewBag.oIsDefaultList = oIsDefaultList;
            ViewBag.oMeasureUnitList = oMeasureUnitList;
            ViewBag.bAddOrEdit = bAddOrEdit;
            oNewProductOnsaleUnitPrice.OnSkuItem.SkuItem.StandardUnit.Name.Matter = oNewProductOnsaleUnitPrice.OnSkuItem.SkuItem.StandardUnit.Name.GetResource(CurrentSession.Culture);
            ViewBag.StandardUnitGid = oNewProductOnsaleUnitPrice.OnSkuItem.SkuItem.StdUnit;
            
            return View(oNewProductOnsaleUnitPrice);
        }
        //用于记录和判断销售价是否小于最低价
        private static bool bLowerThanLowest = false;
        /// <summary>
        /// 添加上架SKU价套信息
        /// </summary>
        /// <param name="oProductUnitPrice"></param>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public string SaveSKUOnsalePriceUnit(ProductOnUnitPrice oProductUnitPrice, FormCollection formCollection)
        {
            bool bNewOrEdit = true;

            ProductOnUnitPrice oNewProductUnitPrice;
            Guid showUnitGid;
            List<ProductOnUnitPrice> listUnitPrice=new List<ProductOnUnitPrice>();
            
            if (oProductUnitPrice.Gid.Equals(Guid.Empty))
            {
                showUnitGid = Guid.Parse(formCollection["ShowUnit.Name.Matter"]);
                //查询出索引对应的价套信息
                listUnitPrice = dbEntity.ProductOnUnitPrices.Include("OnSkuItem").Include("ShowUnit").Include("MarketPrice").Include("SalePrice").Where(p => p.OnSkuID == oProductUnitPrice.OnSkuID && p.aShowUnit == showUnitGid).ToList();
             
                //新建的时候如果已经存在索引，则将其状态变为未删除状态；否则新建一个价套信息。
                if (listUnitPrice.Count > 0)
                {
                    oNewProductUnitPrice = listUnitPrice.ElementAt(0);
                    oNewProductUnitPrice.SalePrice.SetResource(ModelEnum.ResourceType.MONEY, oProductUnitPrice.SalePrice);
                    oNewProductUnitPrice.MarketPrice.SetResource(ModelEnum.ResourceType.MONEY, oProductUnitPrice.MarketPrice);
                    oNewProductUnitPrice.Deleted = false;
                    bNewOrEdit = false;
                }
                else
                {
                    oNewProductUnitPrice = new ProductOnUnitPrice { SalePrice = NewResource(ModelEnum.ResourceType.MONEY, organizationGuid), MarketPrice = NewResource(ModelEnum.ResourceType.MONEY, organizationGuid) };
                    oNewProductUnitPrice.SalePrice = oProductUnitPrice.SalePrice;
                    oNewProductUnitPrice.MarketPrice = oProductUnitPrice.MarketPrice;
                    oNewProductUnitPrice.aShowUnit = showUnitGid;
                }
            }
            else 
            {
                //查询出需要修改的价套信息
                Guid productOnUnitPriceGid = oProductUnitPrice.Gid;
                oNewProductUnitPrice = dbEntity.ProductOnUnitPrices.Include("OnSkuItem").Include("ShowUnit").Include("MarketPrice").Include("SalePrice").Where(p => p.Deleted == false && p.Gid == productOnUnitPriceGid).SingleOrDefault();
                if (bLowerThanLowest == false)
                {                    
                    if (oNewProductUnitPrice.aShowUnit.Equals(oNewProductUnitPrice.OnSkuItem.SkuItem.StdUnit))
                    {
                        bLowerThanLowest = true;
                        //需要判断销售价是否低于最低价
                        List<MemberOrgCulture> oMemberOrgCulture = dbEntity.MemberOrgCultures.Include("Currency").Where(p => p.OrgID == organizationGuid && p.Ctype == 1 && p.aCurrency != Guid.Empty).OrderBy(p => p.Sorting).ToList();
                        for (int i = 0; i < oMemberOrgCulture.Count; i++)
                        {
                            Guid oCurrency = (Guid)oMemberOrgCulture.ElementAt(i).aCurrency;
                            if (oProductUnitPrice.SalePrice.GetResource(oCurrency) < oNewProductUnitPrice.OnSkuItem.SkuItem.LowestPrice.GetResource(oCurrency))
                            {
                                return "fail";
                            }
                        }
                    }
                }
                else 
                {
                    bLowerThanLowest = false;
                }

                oNewProductUnitPrice.SalePrice.SetResource(ModelEnum.ResourceType.MONEY, oProductUnitPrice.SalePrice);
                oNewProductUnitPrice.MarketPrice.SetResource(ModelEnum.ResourceType.MONEY, oProductUnitPrice.MarketPrice);
                bNewOrEdit = false;
            }

            oNewProductUnitPrice.OnSkuID = oProductUnitPrice.OnSkuID;            
            oNewProductUnitPrice.UnitRatio = oProductUnitPrice.UnitRatio;
            oNewProductUnitPrice.Percision = oProductUnitPrice.Percision;
            string strDefault = formCollection["IsDefault"];
            if (strDefault.Equals("YES"))
            {
                oNewProductUnitPrice.IsDefault = true;
            }
            else 
            {
                oNewProductUnitPrice.IsDefault = false;
            }
            oNewProductUnitPrice.Remark = oProductUnitPrice.Remark;
            ProductOnItem oSKUOnsale = dbEntity.ProductOnItems.Where(p => p.Gid == selectedSKUOnsaleGuid && p.Deleted == false).FirstOrDefault();
            if (bNewOrEdit == true)
            {
                oSKUOnsale.SetQuantity = oSKUOnsale.SetQuantity + 1;//回填价套数
                dbEntity.ProductOnUnitPrices.Add(oNewProductUnitPrice);
                dbEntity.SaveChanges();
                BackWritePriceOnsale(oSKUOnsale.Gid);//回填PUOnsale价格
            }
            else 
            {
                dbEntity.SaveChanges();
                BackWritePriceOnsale(oSKUOnsale.Gid);//回填PUOnsale价格
            }

            return "success";
        }
        
        #endregion

        #region 上架模板

        private static bool bOnTemplateSave = false;

        public ActionResult OnTemplate(bool? successImport=false) 
        {
            ViewBag.ImportSuccess = successImport;
            return View();
        }
        /// <summary>
        /// 显示可以使用的上架模板
        /// </summary>
        /// <param name="productGid"></param>
        /// <returns></returns>
        public ActionResult ProductTemplateOnsale(Guid productGid) 
        {
            ProductOnSale oProductOnsale = new ProductOnSale();
            oProductOnsale.OrgID = organizationGuid;
            oProductOnsale.ProdID = productGid;
            MemberOrganization oMember = dbEntity.MemberOrganizations.Include("FullName").Where(p=>p.Gid == organizationGuid && p.Deleted == false).FirstOrDefault();
            ViewBag.orgName = oMember.FullName.GetResource(CurrentSession.Culture);
            ProductInformation oProduct = dbEntity.ProductInformations.Include("Name").Where(p => p.Gid == productGid && p.Deleted == false).FirstOrDefault();
            ViewBag.productCode = oProduct.Code;
            //上架渠道下拉框，本组织支持的渠道
            List<SelectListItem> oChlList = new List<SelectListItem>();
            List<MemberOrgChannel> listChl = dbEntity.MemberOrgChannels.Where(c => c.Deleted == false && c.OrgID == organizationGuid).ToList();
            for (int i = 0; i < listChl.Count; i++)
            {
                oChlList.Add(new SelectListItem { Text = listChl.ElementAt(i).Channel.FullName.GetResource(CurrentSession.Culture), Value = listChl.ElementAt(i).ChlID.ToString() });
            }
            ViewBag.productChList = oChlList;
            //组织支持的上架商品模板
            List<SelectListItem> TemplateCodeList = new List<SelectListItem>();
            List<ProductOnTemplate> listProductOnTemplate = dbEntity.ProductOnTemplates.Where(p => p.OrgID == organizationGuid && p.Deleted == false).ToList();
            for (int i = 0; i < listProductOnTemplate.Count; i++)
            {
                TemplateCodeList.Add(new SelectListItem { Text = listProductOnTemplate.ElementAt(i).Code, Value = listProductOnTemplate.ElementAt(i).Gid.ToString() });
            }
            ViewBag.TemplateCodeList = TemplateCodeList;

            return View(oProductOnsale);
        }
        /// <summary>
        /// 上架模板列表信息
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListOnsaleTemplate(SearchModel searchModel) 
        {
            Guid userGid = (Guid)CurrentSession.UserID;
            MemberUser oMemberUser = dbEntity.MemberUsers.Where(p => p.Gid == userGid && p.Deleted == false).FirstOrDefault();
            Guid userOrgGid = oMemberUser.OrgID;

            IQueryable<ProductOnTemplate> oProductTemplate = dbEntity.ProductOnTemplates.Include("Name").Where(p => p.Deleted == false && p.OrgID == userOrgGid).AsQueryable();

            GridColumnModelList<ProductOnTemplate> columns = new GridColumnModelList<ProductOnTemplate>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Code);
            columns.Add(p => p.Name == null ? "" : p.Name.GetResource(CurrentSession.Culture)).SetName("TemplateName");
            columns.Add(p => p.ShipPolicy);
            columns.Add(p => p.PayPolicy);
            columns.Add(p => p.Relation);
            columns.Add(p => p.LevelDiscount);
            columns.Add(p => p.Remark);

            GridData gridData = oProductTemplate.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 返回添加模板的页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductOnsaleTemplateAdd() 
        {
            ProductOnTemplate oProuctOnTemplate = new ProductOnTemplate { Name = NewResource(ModelEnum.ResourceType.STRING, organizationGuid) };
            ProductOnSale oProductOnSale = dbEntity.ProductOnSales.Include("Product").Where(p => p.Gid == selectedPuOnSaleGuid && p.Deleted == false).FirstOrDefault();
            bOnTemplateSave = false;
            ViewBag.ProductCode = oProductOnSale.Product.Code;
            ViewBag.ProductOnsaleCode = oProductOnSale.Code;
            return View(oProuctOnTemplate);
        }
        /// <summary>
        /// 设定上架覆盖的变量
        /// </summary>
        public void SetOnTemplateSave() 
        {
            bOnTemplateSave = true;
        }
        /// <summary>
        /// 保存对应的上架PU的上架模板
        /// </summary>
        /// <returns></returns>
        public string  SaveProductOnsaleTemplate(ProductOnTemplate oBackTemplate) 
        {
            ProductOnTemplate oNewProductOnTemplate;
            String strCode = oBackTemplate.Code;
            string strShippingPolicy = "";
            string strPayPolicy = "";
            string strRelation = "";
            string strLevelDiscount = "";
            oNewProductOnTemplate = dbEntity.ProductOnTemplates.Where(p => p.OrgID == organizationGuid && p.Code == strCode).FirstOrDefault();
            if (oNewProductOnTemplate == null)
            {
                oNewProductOnTemplate = new ProductOnTemplate { Name = NewResource(ModelEnum.ResourceType.STRING, organizationGuid) };
                //需要新建上架模板
                oNewProductOnTemplate.OrgID = organizationGuid;
                oNewProductOnTemplate.Code = oBackTemplate.Code;
                oNewProductOnTemplate.Name = oBackTemplate.Name;

                //取出上架商品的承运商信息以及承运商地区信息
                List<ProductOnShipping> oOnsaleShipping = dbEntity.ProductOnShippings.Include("Shipper").Include("OnShipArea").Where(p => p.OnSaleID == selectedPuOnSaleGuid && p.Deleted == false).ToList();
                for (int i = 0; i < oOnsaleShipping.Count; i++)
                {
                    strShippingPolicy = strShippingPolicy + oOnsaleShipping.ElementAt(i).Shipper.Code + "|" + oOnsaleShipping.ElementAt(i).ShipWeight + "|" + oOnsaleShipping.ElementAt(i).Solution + ":";
                    if (oOnsaleShipping.ElementAt(i).OnShipArea.Count == 0)
                    {
                        if (i != oOnsaleShipping.Count - 1)
                        {
                            strShippingPolicy = strShippingPolicy + ";";
                        }
                    }
                    else
                    {
                        for (int j = 0; j < oOnsaleShipping.ElementAt(i).OnShipArea.Count; j++)
                        {
                            if (j == oOnsaleShipping.ElementAt(i).OnShipArea.Count - 1)
                            {
                                //如果是最后一个承运商的最后一个地区代码，则不需要加上分号
                                if (i == oOnsaleShipping.Count - 1)
                                {
                                    strShippingPolicy = strShippingPolicy + oOnsaleShipping.ElementAt(i).OnShipArea.ElementAt(j).RegionID.ToString("N");
                                }
                                else
                                {
                                    strShippingPolicy = strShippingPolicy + oOnsaleShipping.ElementAt(i).OnShipArea.ElementAt(j).RegionID.ToString("N") + ";";
                                }
                            }
                            else
                            {
                                strShippingPolicy = strShippingPolicy + oOnsaleShipping.ElementAt(i).OnShipArea.ElementAt(j).RegionID.ToString("N") + ",";
                            }
                        }
                    }
                }
                oNewProductOnTemplate.ShipPolicy = strShippingPolicy;

                //取出上架商品的支付方式
                List<ProductOnPayment> oOnsalePayment = dbEntity.ProductOnPayments.Include("PayType").Where(p => p.OnSaleID == selectedPuOnSaleGuid && p.Deleted == false).ToList();
                for (int i = 0; i < oOnsalePayment.Count; i++)
                {
                    if (i == oOnsalePayment.Count - 1)
                    {
                        strPayPolicy = strPayPolicy + oOnsalePayment.ElementAt(i).PayType.Code;
                    }
                    else 
                    {
                        strPayPolicy = strPayPolicy + oOnsalePayment.ElementAt(i).PayType.Code + ";";
                    }
                }
                oNewProductOnTemplate.PayPolicy = strPayPolicy;

                //取出关联商品的信息
                //字符串格式使用上架商品的Guid
                List<ProductOnRelation> oOnsaleRelation = dbEntity.ProductOnRelations.Include("OnSale").Where(p => p.OnSaleID == selectedPuOnSaleGuid && p.Deleted == false).ToList();
                for (int i = 0; i < oOnsaleRelation.Count; i++)
                {
                    if (i == oOnsaleRelation.Count - 1)
                    {
                        strRelation = strRelation + oOnsaleRelation.ElementAt(i).OnSale.Gid.ToString("N") + "|" + oOnsaleRelation.ElementAt(i).Rtype.ToString();
                    }
                    else 
                    {
                        strRelation = strRelation + oOnsaleRelation.ElementAt(i).OnSale.Gid.ToString("N") + "|" + oOnsaleRelation.ElementAt(i).Rtype.ToString() + ";";
                    }
                }
                oNewProductOnTemplate.Relation = strRelation;

                //取出会员的折扣信息
                List<ProductOnLevelDiscount> oOnsaleLevelDiscount = dbEntity.ProductOnLevelDiscounts.Include("UserLevel").Where(p => p.OnSaleID == selectedPuOnSaleGuid && p.Deleted == false).ToList();
                for (int i = 0; i < oOnsaleLevelDiscount.Count; i++)
                {
                    if (i == oOnsaleLevelDiscount.Count - 1)
                    {
                        strLevelDiscount = strLevelDiscount + oOnsaleLevelDiscount.ElementAt(i).UserLevel.Code + ":" + oOnsaleLevelDiscount.ElementAt(i).Discount.ToString();
                    }
                    else 
                    {
                        strLevelDiscount = strLevelDiscount + oOnsaleLevelDiscount.ElementAt(i).UserLevel.Code + ":" + oOnsaleLevelDiscount.ElementAt(i).Discount.ToString() + ";";
                    }
                }
                oNewProductOnTemplate.LevelDiscount = strLevelDiscount;

                oNewProductOnTemplate.Remark = oBackTemplate.Remark;

                dbEntity.ProductOnTemplates.Add(oNewProductOnTemplate);

                dbEntity.SaveChanges();

            }
            else 
            {
                //存在原有的上架模板
                if (oNewProductOnTemplate.Deleted == true)
                {
                    //不需要提示用户，直接修改原有被删除的模板
                    oNewProductOnTemplate.Name.SetResource(ModelEnum.ResourceType.STRING, oBackTemplate.Name);
                    oNewProductOnTemplate.Deleted = false;
                    //取出上架商品的承运商信息以及承运商地区信息
                    List<ProductOnShipping> oOnsaleShipping = dbEntity.ProductOnShippings.Include("Shipper").Include("OnShipArea").Where(p => p.OnSaleID == selectedPuOnSaleGuid && p.Deleted == false).ToList();
                    for (int i = 0; i < oOnsaleShipping.Count; i++)
                    {
                        strShippingPolicy = strShippingPolicy + oOnsaleShipping.ElementAt(i).Shipper.Code + "|" + oOnsaleShipping.ElementAt(i).ShipWeight + "|" + oOnsaleShipping.ElementAt(i).Solution + ":";
                        if (oOnsaleShipping.ElementAt(i).OnShipArea.Count == 0)
                        {
                            if (i != oOnsaleShipping.Count - 1)
                            {
                                strShippingPolicy = strShippingPolicy + ";";
                            }
                        }
                        else
                        {
                            for (int j = 0; j < oOnsaleShipping.ElementAt(i).OnShipArea.Count; j++)
                            {
                                if (j == oOnsaleShipping.ElementAt(i).OnShipArea.Count - 1)
                                {
                                    //如果是最后一个承运商的最后一个地区代码，则不需要加上分号
                                    if (i == oOnsaleShipping.Count - 1)
                                    {
                                        strShippingPolicy = strShippingPolicy + oOnsaleShipping.ElementAt(i).OnShipArea.ElementAt(j).RegionID.ToString("N");
                                    }
                                    else
                                    {
                                        strShippingPolicy = strShippingPolicy + oOnsaleShipping.ElementAt(i).OnShipArea.ElementAt(j).RegionID.ToString("N") + ";";
                                    }
                                }
                                else
                                {
                                    strShippingPolicy = strShippingPolicy + oOnsaleShipping.ElementAt(i).OnShipArea.ElementAt(j).RegionID.ToString("N") + ",";
                                }
                            }
                        }
                    }
                    oNewProductOnTemplate.ShipPolicy = strShippingPolicy;

                    //取出上架商品的支付方式
                    List<ProductOnPayment> oOnsalePayment = dbEntity.ProductOnPayments.Include("PayType").Where(p => p.OnSaleID == selectedPuOnSaleGuid && p.Deleted == false).ToList();
                    for (int i = 0; i < oOnsalePayment.Count; i++)
                    {
                        if (i == oOnsalePayment.Count - 1)
                        {
                            strPayPolicy = strPayPolicy + oOnsalePayment.ElementAt(i).PayType.Code;
                        }
                        else
                        {
                            strPayPolicy = strPayPolicy + oOnsalePayment.ElementAt(i).PayType.Code + ";";
                        }
                    }
                    oNewProductOnTemplate.PayPolicy = strPayPolicy;

                    //取出关联商品的信息
                    //字符串格式使用上架商品的Guid
                    List<ProductOnRelation> oOnsaleRelation = dbEntity.ProductOnRelations.Include("OnSale").Where(p => p.OnSaleID == selectedPuOnSaleGuid && p.Deleted == false).ToList();
                    for (int i = 0; i < oOnsaleRelation.Count; i++)
                    {
                        if (i == oOnsaleRelation.Count - 1)
                        {
                            strRelation = strRelation + oOnsaleRelation.ElementAt(i).OnSale.Gid.ToString("N") + "|" + oOnsaleRelation.ElementAt(i).Rtype.ToString();
                        }
                        else
                        {
                            strRelation = strRelation + oOnsaleRelation.ElementAt(i).OnSale.Gid.ToString("N") + "|" + oOnsaleRelation.ElementAt(i).Rtype.ToString() + ";";
                        }
                    }
                    oNewProductOnTemplate.Relation = strRelation;

                    //取出会员的折扣信息
                    List<ProductOnLevelDiscount> oOnsaleLevelDiscount = dbEntity.ProductOnLevelDiscounts.Include("UserLevel").Where(p => p.OnSaleID == selectedPuOnSaleGuid && p.Deleted == false).ToList();
                    for (int i = 0; i < oOnsaleLevelDiscount.Count; i++)
                    {
                        if (i == oOnsaleLevelDiscount.Count - 1)
                        {
                            strLevelDiscount = strLevelDiscount + oOnsaleLevelDiscount.ElementAt(i).UserLevel.Code + ":" + oOnsaleLevelDiscount.ElementAt(i).Discount.ToString();
                        }
                        else
                        {
                            strLevelDiscount = strLevelDiscount + oOnsaleLevelDiscount.ElementAt(i).UserLevel.Code + ":" + oOnsaleLevelDiscount.ElementAt(i).Discount.ToString() + ";";
                        }
                    }
                    oNewProductOnTemplate.LevelDiscount = strLevelDiscount;

                    oNewProductOnTemplate.Remark = oBackTemplate.Remark;

                    dbEntity.SaveChanges();

                }
                else 
                {
                    //需要提示用户模板已存在，页面提示用户是否继续保存，如果是的话，修改全局变量；
                    if (bOnTemplateSave == false)
                    {
                        return "fail";
                    }
                    else 
                    {
                        oNewProductOnTemplate.Name.SetResource(ModelEnum.ResourceType.STRING, oBackTemplate.Name);

                        //取出上架商品的承运商信息以及承运商地区信息
                        List<ProductOnShipping> oOnsaleShipping = dbEntity.ProductOnShippings.Include("Shipper").Include("OnShipArea").Where(p => p.OnSaleID == selectedPuOnSaleGuid && p.Deleted == false).ToList();
                        for (int i = 0; i < oOnsaleShipping.Count; i++)
                        {
                            strShippingPolicy = strShippingPolicy + oOnsaleShipping.ElementAt(i).Shipper.Code + "|" + oOnsaleShipping.ElementAt(i).ShipWeight + "|" + oOnsaleShipping.ElementAt(i).Solution + ":";
                            if (oOnsaleShipping.ElementAt(i).OnShipArea.Count == 0)
                            {
                                if (i != oOnsaleShipping.Count - 1)
                                {
                                    strShippingPolicy = strShippingPolicy + ";";
                                }
                            }
                            else
                            {
                                for (int j = 0; j < oOnsaleShipping.ElementAt(i).OnShipArea.Count; j++)
                                {
                                    if (j == oOnsaleShipping.ElementAt(i).OnShipArea.Count - 1)
                                    {
                                        //如果是最后一个承运商的最后一个地区代码，则不需要加上分号
                                        if (i == oOnsaleShipping.Count - 1)
                                        {
                                            strShippingPolicy = strShippingPolicy + oOnsaleShipping.ElementAt(i).OnShipArea.ElementAt(j).RegionID.ToString("N");
                                        }
                                        else
                                        {
                                            strShippingPolicy = strShippingPolicy + oOnsaleShipping.ElementAt(i).OnShipArea.ElementAt(j).RegionID.ToString("N") + ";";
                                        }
                                    }
                                    else
                                    {
                                        strShippingPolicy = strShippingPolicy + oOnsaleShipping.ElementAt(i).OnShipArea.ElementAt(j).RegionID.ToString("N") + ",";
                                    }
                                }
                            }
                        }
                        oNewProductOnTemplate.ShipPolicy = strShippingPolicy;

                        //取出上架商品的支付方式
                        List<ProductOnPayment> oOnsalePayment = dbEntity.ProductOnPayments.Include("PayType").Where(p => p.OnSaleID == selectedPuOnSaleGuid && p.Deleted == false).ToList();
                        for (int i = 0; i < oOnsalePayment.Count; i++)
                        {
                            if (i == oOnsalePayment.Count - 1)
                            {
                                strPayPolicy = strPayPolicy + oOnsalePayment.ElementAt(i).PayType.Code;
                            }
                            else
                            {
                                strPayPolicy = strPayPolicy + oOnsalePayment.ElementAt(i).PayType.Code + ";";
                            }
                        }
                        oNewProductOnTemplate.PayPolicy = strPayPolicy;

                        //取出关联商品的信息
                        //字符串格式使用上架商品的Guid
                        List<ProductOnRelation> oOnsaleRelation = dbEntity.ProductOnRelations.Include("OnSale").Where(p => p.OnSaleID == selectedPuOnSaleGuid && p.Deleted == false).ToList();
                        for (int i = 0; i < oOnsaleRelation.Count; i++)
                        {
                            if (i == oOnsaleRelation.Count - 1)
                            {
                                strRelation = strRelation + oOnsaleRelation.ElementAt(i).OnSale.Gid.ToString("N") + "|" + oOnsaleRelation.ElementAt(i).Rtype.ToString();
                            }
                            else
                            {
                                strRelation = strRelation + oOnsaleRelation.ElementAt(i).OnSale.Gid.ToString("N") + "|" + oOnsaleRelation.ElementAt(i).Rtype.ToString() + ";";
                            }
                        }
                        oNewProductOnTemplate.Relation = strRelation;

                        //取出会员的折扣信息
                        List<ProductOnLevelDiscount> oOnsaleLevelDiscount = dbEntity.ProductOnLevelDiscounts.Include("UserLevel").Where(p => p.OnSaleID == selectedPuOnSaleGuid && p.Deleted == false).ToList();
                        for (int i = 0; i < oOnsaleLevelDiscount.Count; i++)
                        {
                            if (i == oOnsaleLevelDiscount.Count - 1)
                            {
                                strLevelDiscount = strLevelDiscount + oOnsaleLevelDiscount.ElementAt(i).UserLevel.Code + ":" + oOnsaleLevelDiscount.ElementAt(i).Discount.ToString();
                            }
                            else
                            {
                                strLevelDiscount = strLevelDiscount + oOnsaleLevelDiscount.ElementAt(i).UserLevel.Code + ":" + oOnsaleLevelDiscount.ElementAt(i).Discount.ToString() + ";";
                            }
                        }
                        oNewProductOnTemplate.LevelDiscount = strLevelDiscount;

                        oNewProductOnTemplate.Remark = oBackTemplate.Remark;

                        dbEntity.SaveChanges();

                        bOnTemplateSave = false;
                    }
                }
            }

            return "success";
        }
        /// <summary>
        /// 使用模板对商品上架
        /// </summary>
        /// <param name="oBackOnsale"></param>
        /// <returns></returns>
        public string SaveProductOnsaleUseTemplate(ProductOnSale oBackOnsale, FormCollection formCollection) 
        {
            Guid productGid = oBackOnsale.ProdID;
            ProductInformation oProduct = dbEntity.ProductInformations.Include("SkuItems").Where(p => p.Gid == productGid && p.Deleted == false).FirstOrDefault();
            Guid onSaleChID = oBackOnsale.ChlID;
            MemberChannel oMemberChl = dbEntity.MemberChannels.Where(p => p.Gid == onSaleChID && p.Deleted == false).FirstOrDefault();
            Guid onSaleOrgID = oBackOnsale.OrgID;
            MemberOrganization oMemberOrg = dbEntity.MemberOrganizations.Where(p => p.Gid == onSaleOrgID && p.Deleted == false).FirstOrDefault();
            Guid onSaleTemplateGid = Guid.Parse(formCollection["ProductTemplateCode"]);
            ProductOnTemplate oProductTemplate = dbEntity.ProductOnTemplates.Where(p => p.Gid == onSaleTemplateGid && p.Deleted == false).FirstOrDefault();

            if (oProduct == null || oMemberChl == null || oMemberOrg == null || oProductTemplate == null)
            {
                return "fail";
            }
            else 
            {
                return productBLL.ProductTemplateOnSale(oMemberOrg, oMemberChl, oProductTemplate, oProduct);
            }
        }
        /// <summary>
        /// 通过Excel导入拥有模板的上架商品
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportProductUsingTemplate() 
        {
            HttpPostedFileBase hpfProduct = Request.Files["ImportProduct"];
            string sLocalFile, sRemoteFile, sExtension, sFullFilePath;
            string sServerPath = HttpContext.Server.MapPath("~/Temp");
            if (!Directory.Exists(sServerPath))
                Directory.CreateDirectory(sServerPath);
            if (hpfProduct != null && hpfProduct.ContentLength > 0)
            {
                sLocalFile = Path.GetFileName(hpfProduct.FileName);
                sExtension = Path.GetExtension(sLocalFile);
                sRemoteFile = Guid.NewGuid() + sExtension;
                sFullFilePath = Path.Combine(sServerPath, sRemoteFile);
                hpfProduct.SaveAs(sFullFilePath);
                //oTransfer.ImportChinaRegions(sFullFilePath, "");
                dataTransferBLL.ImportProductOnSale(sFullFilePath, "批量上架");
                System.IO.File.Delete(sFullFilePath);  // 删除临时文件
            }
            return RedirectToAction("OnTemplate", new { successImport = true });
        }

        #endregion

        #endregion

        #region 同步上架商品库存和价格到其他渠道

        /// <summary>
        /// 同步上架商品库存和价格到其他渠道，例如淘宝
        /// </summary>
        /// <param name="sync">上架商品ID</param>
        /// <returns></returns>
        public bool Sync(Guid sync)
        {
            ProductOnSale oOnSale = (from o in dbEntity.ProductOnSales
                                     where o.Gid == sync
                                     select o).FirstOrDefault();
            if (oOnSale != null)
            {
                MemberOrgChannel oOrgChl = (from c in dbEntity.MemberOrgChannels.Include("Channel.ExtendType")
                                            where c.OrgID == oOnSale.OrgID && c.ChlID == oOnSale.ChlID
                                            select c).FirstOrDefault();
                if (oOrgChl.Channel.ExtendType.Code == "Taobao")
                {
                    // 同步淘宝库存和价格
                    LiveAzure.BLL.Taobao.ProductAPI oTopProduct = new LiveAzure.BLL.Taobao.ProductAPI(dbEntity, oOrgChl);
                    oTopProduct.ProductUpdate(oOnSale);
                }
                else if (oOrgChl.Channel.ExtendType.Code == "yihaodian")
                {
                    // TODO 同步一号店库存和价格
                }
                else if (oOrgChl.Channel.ExtendType.Code == "Paipai")
                {
                    // TODO 同步拍拍库存和价格
                }
                else if (oOrgChl.Channel.ExtendType.Code == "Sina")
                {
                    // TODO 同步新浪库存和价格
                }
                else if (oOrgChl.Channel.ExtendType.Code == "tg.com.cn")
                {
                    // TODO 同步齐家库存和价格
                }
                return true;
            }
            return false;
        }

        #endregion

        #region 代码规则

        public ActionResult CodePolicy()
        {
            ViewBag.organization = GetSupportOrganizations();
            return View();
        }

        public ActionResult EditCodePolicy(Guid orgGid)
        {
            var allOrganization = (from d in dbEntity.MemberOrganizations
                                   orderby d.Sorting ascending
                                   select d).ToList();
            Guid defaltOrganization = allOrganization.First().Gid;
            MemberOrganization oOrganizationForEdit = dbEntity.MemberOrganizations.Where(o => o.Deleted == false && orgGid == Guid.Empty ? o.Gid == defaltOrganization : o.Gid == orgGid).Single();
            return PartialView(oOrganizationForEdit);
        }

        public ActionResult SaveCodePolicy(MemberOrganization newOrganization)
        {
            MemberOrganization oldOrganization = dbEntity.MemberOrganizations.Where(o => o.Deleted == false && o.Gid == newOrganization.Gid).Single();
            oldOrganization.ProdCodePolicy = newOrganization.ProdCodePolicy;
            oldOrganization.SkuCodePolicy = newOrganization.SkuCodePolicy;
            oldOrganization.BarcodePolicy = newOrganization.BarcodePolicy;
                dbEntity.Entry(oldOrganization).State = System.Data.EntityState.Modified;
                dbEntity.SaveChanges();
            //Guid orgGid = oldOrganization.Gid;
            return RedirectToAction("EditCodePolicy", new { orgGid = oldOrganization.Gid });
        }

        #endregion

        #region SKU图片处理（By 刘鑫）

        /// <summary>
        /// 图像处理页面
        /// </summary>
        /// <returns></returns>
        public ViewResult Gallery()
        {
            ViewBag.OrgList = GetSupportOrganizations();
            ViewBag.SizeList = GetSelectList(ImageSize.GetSizeInfoList());
            return View();
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="orgID">组织Id</param>
        /// <param name="sizeName">尺寸名称</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GenerateResizedImages(Guid orgID, string sizeName, string code = "")
        {
            bool result;
            MemberOrganization org = dbEntity.MemberOrganizations.Find(orgID);
            if (org == null || org.Deleted)
            {
                //组织代码错误
                result = false;
            }
            else
            {
                //需要修改具体目录
                OrgImage.RootDirURL = "~/IMG";
                OrgImage.RootDirPath = HttpContext.Server.MapPath(OrgImage.RootDirURL);
                ImageSize imageSize = new ImageSize(sizeName);
                int targetHeight = imageSize.Height;
                int targetWidth = imageSize.Width;
                OrgImage orgImage = new OrgImage(org.Code);
                if (!string.IsNullOrEmpty(code))
                {
                    #region 处理单个CODE
                    //处理单个PU/SKU
                    ProductInformation pu = (from p in dbEntity.ProductInformations
                                             where p.Code == code
                                                && p.OrgID == orgID
                                                && !p.Deleted
                                             select p).SingleOrDefault();
                    if (pu != null)
                    {
                        //是PU
                        #region 处理SKU
                        List<ProductInfoItem> skus = (from p in dbEntity.ProductInfoItems
                                                      where p.OrgID == orgID
                                                         && p.ProdID == pu.Gid
                                                         && !p.Deleted
                                                      select p).ToList();
                        foreach (ProductInfoItem sku in skus)
                        {
                            string[] oldPaths = orgImage.GetSkuImagePaths(sku.Code).ToArray();
                            string[] newPaths = orgImage.GetSkuImagePaths(sku.Code, imageSize.Size).ToArray();
                            for (int i = oldPaths.Length - 1; i >= 0; i--)
                            {
                                if (!System.IO.File.Exists(oldPaths[i]))
                                    continue;
                                if (System.IO.File.Exists(newPaths[i]))
                                    System.IO.File.Delete(newPaths[i]);
                                ImageResize.Resize(oldPaths[i], newPaths[i], targetHeight, targetWidth);

                                #region 添加数据库记录
                                ProductGallery pg = (from g in dbEntity.ProductGalleries
                                                     where g.ProdID == pu.Gid
                                                        && g.SkuID == sku.Gid
                                                        && g.Gtype == 1
                                                        && !g.Deleted
                                                     select g).SingleOrDefault();
                                if (pg == null)
                                    pg = new ProductGallery
                                    {
                                        ProdID = pu.Gid,
                                        SkuID = sku.Gid,
                                        Gtype = 1
                                    };
                                switch (imageSize.Size)
                                {
                                    case ImageSizeEnum.Large:
                                        pg.Enlarge = newPaths[i]; break;
                                    case ImageSizeEnum.Middle:
                                        pg.Thumburl = newPaths[i]; break;
                                    case ImageSizeEnum.Small:
                                        pg.Thumbnail = newPaths[i]; break;
                                }
                                if (pg.Gid == null)
                                    dbEntity.ProductGalleries.Add(pg);
                                dbEntity.SaveChanges();
                                #endregion 添加数据库记录
                            }
                        }

                        #endregion 处理SKU

                        #region 处理PU
                        string[] oldPUpaths = orgImage.GetSkuImagePaths(code).ToArray();
                        string[] newPUpaths = orgImage.GetSkuImagePaths(code, imageSize.Size).ToArray();
                        for (int i = oldPUpaths.Length - 1; i >= 0; i--)
                        {
                            if (!System.IO.File.Exists(oldPUpaths[i]))
                                continue;
                            if (System.IO.File.Exists(newPUpaths[i]))
                                System.IO.File.Delete(newPUpaths[i]);
                            ImageResize.Resize(oldPUpaths[i], newPUpaths[i], targetHeight, targetWidth);
                            #region 添加数据库记录
                            ProductGallery pg2 = (from g in dbEntity.ProductGalleries
                                                  where g.ProdID == pu.Gid
                                                     && g.Gtype == 0
                                                     && !g.Deleted
                                                  select g).SingleOrDefault();
                            if (pg2 == null)
                                pg2 = new ProductGallery
                                {
                                    ProdID = pu.Gid,
                                    Gtype = 0
                                };
                            switch (imageSize.Size)
                            {
                                case ImageSizeEnum.Large:
                                    pg2.Enlarge = newPUpaths[i]; break;
                                case ImageSizeEnum.Middle:
                                    pg2.Thumburl = newPUpaths[i];
                                    pu.Picture = newPUpaths[i];
                                    break;
                                case ImageSizeEnum.Small:
                                    pg2.Thumbnail = newPUpaths[i]; break;
                            }
                            if (pg2.Gid == null)
                                dbEntity.ProductGalleries.Add(pg2);
                            dbEntity.SaveChanges();
                            #endregion 添加数据库记录
                        }

                        #endregion 处理PU
                        result = true;
                    }
                    else
                    {
                        ProductInfoItem sku = (from p in dbEntity.ProductInfoItems
                                               where p.Code == code
                                                  && p.OrgID == orgID
                                                  && !p.Deleted
                                               select p).SingleOrDefault();
                        if (sku != null)
                        {
                            //是SKU
                            #region 处理SKU
                            string[] oldPaths = orgImage.GetSkuImagePaths(code).ToArray();
                            string[] newPaths = orgImage.GetSkuImagePaths(code, imageSize.Size).ToArray();
                            for (int i = oldPaths.Length - 1; i >= 0; i--)
                            {
                                if (!System.IO.File.Exists(oldPaths[i]))
                                    continue;
                                if (System.IO.File.Exists(newPaths[i]))
                                    System.IO.File.Delete(newPaths[i]);
                                ImageResize.Resize(oldPaths[i], newPaths[i], targetHeight, targetWidth);
                                #region 添加数据库记录
                                ProductGallery pg3 = (from g in dbEntity.ProductGalleries
                                                      where g.ProdID == sku.ProdID
                                                         && g.SkuID == sku.Gid
                                                         && g.Gtype == 1
                                                         && !g.Deleted
                                                      select g).SingleOrDefault();
                                if (pg3 == null)
                                    pg3 = new ProductGallery
                                    {
                                        ProdID = sku.ProdID,
                                        SkuID = sku.Gid,
                                        Gtype = 1
                                    };
                                switch (imageSize.Size)
                                {
                                    case ImageSizeEnum.Large:
                                        pg3.Enlarge = newPaths[i]; break;
                                    case ImageSizeEnum.Middle:
                                        pg3.Thumburl = newPaths[i]; break;
                                    case ImageSizeEnum.Small:
                                        pg3.Thumbnail = newPaths[i]; break;
                                }
                                if (pg3.Gid == null)
                                    dbEntity.ProductGalleries.Add(pg3);
                                dbEntity.SaveChanges();
                                #endregion 添加数据库记录
                            }
                            #endregion 处理SKU
                            result = true;
                        }
                        else
                        {
                            //代码错误
                            result = false;
                        }
                    }
                    #endregion 处理单个CODE
                }
                else
                {
                    #region 处理所有PU/SKU

                    #region 处理PU
                    IEnumerable<ProductInformation> pus = (from p in dbEntity.ProductInformations
                                                           where p.OrgID == orgID
                                                              && !p.Deleted
                                                           select p).ToList();
                    foreach (ProductInformation pu in pus)
                    {
                        string[] oldPUpaths = orgImage.GetSkuImagePaths(pu.Code).ToArray();
                        string[] newPUpaths = orgImage.GetSkuImagePaths(pu.Code, imageSize.Size).ToArray();
                        for (int i = oldPUpaths.Length - 1; i >= 0; i--)
                        {
                            if (!System.IO.File.Exists(oldPUpaths[i]))
                                continue;
                            if (System.IO.File.Exists(newPUpaths[i]))
                                System.IO.File.Delete(newPUpaths[i]);
                            ImageResize.Resize(oldPUpaths[i], newPUpaths[i], targetHeight, targetWidth);
                            #region 添加数据库记录
                            ProductGallery pg2 = (from g in dbEntity.ProductGalleries
                                                  where g.ProdID == pu.Gid
                                                     && g.Gtype == 0
                                                     && !g.Deleted
                                                  select g).SingleOrDefault();
                            if (pg2 == null)
                                pg2 = new ProductGallery
                                {
                                    ProdID = pu.Gid,
                                    Gtype = 0
                                };
                            switch (imageSize.Size)
                            {
                                case ImageSizeEnum.Large:
                                    pg2.Enlarge = newPUpaths[i]; break;
                                case ImageSizeEnum.Middle:
                                    pg2.Thumburl = newPUpaths[i];
                                    pu.Picture = newPUpaths[i];
                                    break;
                                case ImageSizeEnum.Small:
                                    pg2.Thumbnail = newPUpaths[i]; break;
                            }
                            if (pg2.Gid == null)
                                dbEntity.ProductGalleries.Add(pg2);
                            dbEntity.SaveChanges();
                            #endregion 添加数据库记录
                        } 
                    }
                    #endregion 处理PU
                    #region 处理SKU
                    IEnumerable<ProductInfoItem> skus = (from p in dbEntity.ProductInfoItems
                                                         where p.OrgID == orgID
                                                            && !p.Deleted
                                                         select p).ToList();
                    foreach (ProductInfoItem sku in skus)
                    {
                        string[] oldSKUpaths = orgImage.GetSkuImagePaths(sku.Code).ToArray();
                        string[] newSKUpaths = orgImage.GetSkuImagePaths(sku.Code, imageSize.Size).ToArray();
                        for (int i = oldSKUpaths.Length - 1; i >= 0; i--)
                        {
                            if (!System.IO.File.Exists(oldSKUpaths[i]))
                                continue;
                            if (System.IO.File.Exists(newSKUpaths[i]))
                                System.IO.File.Delete(newSKUpaths[i]);
                            ImageResize.Resize(oldSKUpaths[i], newSKUpaths[i], targetHeight, targetWidth);
                            #region 添加数据库记录
                            ProductGallery pg2 = (from g in dbEntity.ProductGalleries
                                                  where g.ProdID == sku.ProdID
                                                     && g.SkuID == sku.Gid
                                                     && g.Gtype == 1
                                                     && !g.Deleted
                                                  select g).SingleOrDefault();
                            if (pg2 == null)
                                pg2 = new ProductGallery
                                {
                                    ProdID = sku.ProdID,
                                    SkuID = sku.Gid,
                                    Gtype = 1
                                };
                            switch (imageSize.Size)
                            {
                                case ImageSizeEnum.Large:
                                    pg2.Enlarge = newSKUpaths[i]; break;
                                case ImageSizeEnum.Middle:
                                    pg2.Thumburl = newSKUpaths[i]; break;
                                case ImageSizeEnum.Small:
                                    pg2.Thumbnail = newSKUpaths[i]; break;
                            }
                            if (pg2.Gid == null)
                                dbEntity.ProductGalleries.Add(pg2);
                            dbEntity.SaveChanges();
                        }
                            #endregion 添加数据库记录
                    }
                    #endregion 处理SKU
                    result = true;
                    #endregion 处理所有PU/SKU
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion SKU图片处理

        #region 批量导入
        public ActionResult Import()
        {
            return View();
        }
        #endregion 批量导入

        #region 数据验证
        public ActionResult Validation()
        {
            return View();
        }
        #endregion 数据验证


        /// <summary>
        /// 获取组织对象
        /// </summary>
        /// <param name="gid"></param>
        /// <returns></returns>
        private MemberOrganization GetOrganization(Guid gid)
        {
            return dbEntity.MemberOrganizations.Where(p => p.Gid == gid).FirstOrDefault();
        }
    }
}
