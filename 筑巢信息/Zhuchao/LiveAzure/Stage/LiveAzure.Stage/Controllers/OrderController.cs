using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveAzure.Models;
using LiveAzure.Models.Order;
using LiveAzure.Models.Member;
using LiveAzure.Models.Finance;
using LiveAzure.Models.General;
using LiveAzure.Models.Warehouse;
using MVC.Controls.Grid;
using MVC.Controls;
using LiveAzure.Models.Product;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Web.UI.WebControls;
using System.Collections;

using LiveAzure.BLL;
using LiveAzure.Models.Shipping;

namespace LiveAzure.Stage.Controllers
{
    public class OrderController : BaseController
    {
        #region 初始化
        public WarehouseBLL warehouseBll;
        public OrderBLL orderBll;
        private static bool IsEditing = false; //是否是在编辑
        private static Guid OrganizationGuidGeneralOrder;//手动产生订单所属的组织
        private static Guid ChannelGuidGeneralOrder;//手动产生订单所属的渠道
        private static Guid WarehouseGuidSelected;//选中的仓库
        private static Guid ClientGuid; //下单用户的guid
        private static OrderInformation oNewOrder = new OrderInformation();//新生成的订单对象，未保存
        private static List<OrderItem> listNewOrderItem = new List<OrderItem>();//新添加商品列表
        private static List<OrderShipping> listNewOrderShipping = new List<OrderShipping>();//新配送列表
        private static ShippingInformation oNewShipper = new ShippingInformation();//承运商
        private static bool isBackList = false;//是否还原原始订单中商品，用于撤销更改订单商品全局变量
        private static string searchKey="";//订单中搜索关键字
        private static string searchType = "";//搜索类型
        //public EventBLL eventBll;
        //用于判断页面是否点击订单变更按钮
        private static bool bChangeOrder = false;//当前是否是变更状态
        private static bool bOrderBaseInfoEdit = false;//当前是否在变更订单基础信息
        private static bool bOrderItemInfoEdit = false;//当前是否在变更订单产品信息
        private static bool bOrderFeeInfoEdit = false;//当前是否在变更订单价格信息
        private static bool bSaveEditOrderInfo = false;//判断是否保存进数据库
        
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            string strControllerName = (string)requestContext.RouteData.Values["Controller"];
            string strActionName = (string)requestContext.RouteData.Values["Action"];
            string strCurrenAction = strControllerName + "." + strActionName;
            base.Initialize(requestContext);
            oEventBLL.WriteEvent("Initialize Load",
                ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, strCurrenAction, CurrentSession.UserID);
        }
        //
        // GET: /Order/
        public OrderController() 
        {
            warehouseBll = new WarehouseBLL(dbEntity);
            orderBll = new OrderBLL(dbEntity);
        }
        #endregion 初始化

        #region 订单首页列表
        /// <summary>
        /// 订单首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // 权限验证
            if (!base.CheckPrivilege())
                RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            oNewOrder = new OrderInformation();
            return View();
        }
        /// <summary>
        /// 订单列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderList()
        {
            searchKey = "";//置空搜索关键字          
            searchType = "";
            return View();
        }
        #region 搜索
        /// <summary>
        /// search page
        /// </summary>
        /// <returns></returns>
        public PartialViewResult Search()
        {
            /*----------organization downlist----------*/
            if (Guid.Empty == OrganizationGuidGeneralOrder)
            {
                ViewBag.Org = GetSupportOrganizations();//orgnization downlist
                OrganizationGuidGeneralOrder = CurrentSession.OrganizationGID;
            }
            else
            {
                List<MemberOrganization> oUserOrganization = new List<MemberOrganization>();
                Guid UserID = (Guid)CurrentSession.UserID;
                if (CurrentSession.IsAdmin)//是管理员 则有所有组织权限
                {
                    oUserOrganization = dbEntity.MemberOrganizations
                                        .Where(o => o.Deleted == false && o.Otype == (byte)ModelEnum.OrganizationType.CORPORATION)
                                        .OrderByDescending(o => o.Sorting)
                                        .ToList();
                }
                else//非管理员
                {
                    try
                    {
                        byte IsValde = dbEntity.MemberPrivileges.Where(p => p.UserID == CurrentSession.UserID && p.Ptype == (byte)ModelEnum.UserPrivType.ORGANIZATION).FirstOrDefault().Pstatus;
                        if (IsValde == 1)//如果是启用状态，则将权限表中查得的组织添加到列表
                        {
                            oUserOrganization = (from pi in dbEntity.MemberPrivItems.AsEnumerable()
                                                 join p in dbEntity.MemberPrivileges.AsEnumerable() on pi.PrivID equals p.Gid
                                                 join org in dbEntity.MemberOrganizations.AsEnumerable() on pi.RefID equals org.Gid
                                                 where pi.Deleted == false && p.UserID == UserID && p.Ptype == (byte)ModelEnum.UserPrivType.ORGANIZATION
                                                 orderby org.Sorting descending
                                                 select org).ToList();
                        }
                        //将自己所属组织加到列表
                        MemberOrganization currentOrg = dbEntity.MemberOrganizations.Where(o => o.Deleted == false && o.Gid == CurrentSession.OrganizationGID).FirstOrDefault();
                        oUserOrganization.Add(currentOrg);
                    }
                    catch (Exception e)//
                    {
                        oEventBLL.WriteEvent(e.ToString());
                    }
                }
                List<SelectListItem> ogranizationList = new List<SelectListItem>();
                foreach (MemberOrganization item in oUserOrganization)
                {
                    if (item.Gid == OrganizationGuidGeneralOrder)//默认选中自己所属组织
                        ogranizationList.Add(new SelectListItem { Text = item.FullName.GetResource(CurrentSession.Culture), Value = item.Gid.ToString(), Selected = true });
                    else
                        ogranizationList.Add(new SelectListItem { Text = item.FullName.GetResource(CurrentSession.Culture), Value = item.Gid.ToString(), Selected = false });
                }
                ViewBag.Org = ogranizationList;
            }
            /*---------channel downlist-----------*/
            List<MemberOrgChannel> oChannel = dbEntity.MemberOrgChannels.Where(p => p.Deleted == false && p.OrgID == OrganizationGuidGeneralOrder).ToList();
            if (oChannel != null)
            {
                ChannelGuidGeneralOrder = oChannel.Select(p => p.ChlID).FirstOrDefault();
                List<SelectListItem> ChannelList = new List<SelectListItem>();
                foreach (MemberOrgChannel item in oChannel)
                {
                    if (item.ChlID == ChannelGuidGeneralOrder)
                        ChannelList.Add(new SelectListItem { Text = item.Channel.FullName.GetResource(CurrentSession.Culture), Value = item.ChlID.ToString(), Selected = true });
                    else
                        ChannelList.Add(new SelectListItem { Text = item.Channel.FullName.GetResource(CurrentSession.Culture), Value = item.ChlID.ToString(), Selected = false });
                }
                ViewBag.Channel = ChannelList;
            }
            else
            {
                //return error page
                RedirectToAction("ErrorPage", "Home", new { message = "Sorry you should have channel" });
            }
            /*-----------warehouse downlist -------------*/
            List<WarehouseInformation> oWarehouse = dbEntity.WarehouseInformations.Where(p => p.Deleted == false && p.aParent == OrganizationGuidGeneralOrder).ToList();
            if (oWarehouse != null)
            {
                WarehouseGuidSelected = oWarehouse.Select(p => p.Gid).FirstOrDefault();
                List<SelectListItem> WarehouseList = new List<SelectListItem>();
                foreach (WarehouseInformation item1 in oWarehouse)
                {
                    if (item1.Gid == WarehouseGuidSelected)
                        WarehouseList.Add(new SelectListItem { Text = item1.FullName.GetResource(CurrentSession.Culture), Value = item1.Gid.ToString(), Selected = true });
                    else
                        WarehouseList.Add(new SelectListItem { Text = item1.FullName.GetResource(CurrentSession.Culture), Value = item1.Gid.ToString(), Selected = false });
                }
                ViewBag.Warehouse = WarehouseList;
            }
            else
            {
                //return error page
                RedirectToAction("ErrorPage", "Home", new { message = "Sorry you should have warehouse" });
            }
            /*---------------search downlist--------------*/
            List<SelectListItem> SearchList = new List<SelectListItem>();
            SearchList.Add(new SelectListItem { Text = @LiveAzure.Resource.Stage.OrderController.OrderCode, Value = "OrderCode", Selected = true });
            SearchList.Add(new SelectListItem { Text = @LiveAzure.Resource.Stage.OrderController.Nick, Value = "Nick", Selected = false });
            SearchList.Add(new SelectListItem { Text = @LiveAzure.Resource.Stage.OrderController.SKUCode, Value = "SKUCode", Selected = false });
            SearchList.Add(new SelectListItem { Text = @LiveAzure.Resource.Stage.OrderController.Telephone, Value = "Tel", Selected = false });
            ViewBag.Search = SearchList;
            return PartialView();
        }

        /// <summary>
        /// 组织改变
        /// </summary>
        /// <param name="Org"></param>
        public ActionResult OrgChange(Guid Org)
        {
            OrganizationGuidGeneralOrder = Org;
            searchKey = "";
            searchType = "";
            return RedirectToAction("Search");
        }
        public void ChannelChange(Guid Channel)
        {
            ChannelGuidGeneralOrder = Channel;
        }
        /// <summary>
        /// 订单搜索
        /// </summary>
        /// <param name="organization">订单gid</param>
        /// <param name="channel">渠道gid</param>
        /// <param name="warehouse">仓库gid</param>
        /// <param name="stype">搜索类型</param>
        /// <param name="skey">搜索关键字</param>
        public void SearchKey(Guid organization, Guid channel, Guid warehouse, string stype, string skey)
        {
            OrganizationGuidGeneralOrder = organization;
            ChannelGuidGeneralOrder = channel;
            WarehouseGuidSelected = warehouse;
            searchType = stype;
            searchKey = skey;
        }
        #endregion
        /// <summary>
        /// 订单列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListOrderInformation(SearchModel searchModel)
        {
            IQueryable<OrderInformation> oOrderInfomation = null;
            /*判断是否进行了搜索*/
            if (searchKey != "")
            {
                oOrderInfomation = dbEntity.OrderInformations.Include("Organization").Include("Channel").Include("User").Include("OrderItems").Where(p => p.Deleted == false && p.OrgID == OrganizationGuidGeneralOrder && p.ChlID == ChannelGuidGeneralOrder && p.WhID == WarehouseGuidSelected).AsQueryable();
                switch (searchType)
                {
                    case "OrderCode":
                        oOrderInfomation = oOrderInfomation.Where(p => p.Code.Contains(searchKey));
                        break;
                    case "Nick":
                        oOrderInfomation = oOrderInfomation.Where(p => p.User.NickName.Contains(searchKey));
                        break;
                    case "SKUCode":
                        {
                            List<Guid> gidList = dbEntity.ProductInfoItems.Where(p => p.Deleted == false && p.Code.Contains(searchKey)).Select(p => p.Gid).ToList();
                            oOrderInfomation = (from oi in dbEntity.OrderInformations.AsEnumerable()
                                                join oii in dbEntity.OrderItems.AsEnumerable() on oi.Gid equals oii.OrderID
                                                join pii in dbEntity.ProductInfoItems.AsEnumerable() on oii.SkuID equals pii.Gid
                                                where pii.Code.Contains(searchKey) && pii.OrgID == OrganizationGuidGeneralOrder && oi.OrgID == OrganizationGuidGeneralOrder
                                                select oi).AsQueryable();
                        }
                        break;
                    case "Tel":
                        oOrderInfomation = oOrderInfomation.Where(p => p.Telephone.Contains(searchKey) || p.Mobile.Contains(searchKey));
                        break;
                    default:
                        RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no error" });
                        break;
                }
            }
            else
            {
                oOrderInfomation = dbEntity.OrderInformations.Include("Organization").Include("Channel").Include("User").Where(p => p.Deleted == false && p.OrgID == OrganizationGuidGeneralOrder && p.ChlID == ChannelGuidGeneralOrder && p.WhID == WarehouseGuidSelected).AsQueryable();
            }
            CultureInfo culture = new CultureInfo(CurrentSession.Culture);
            GridColumnModelList<OrderInformation> columns = new GridColumnModelList<OrderInformation>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(x => x.Code);
            //columns.Add(x => x.Organization.FullName.Matter).SetName("Organization");
            //columns.Add(x => x.Channel.FullName.Matter).SetName("Channel");
            columns.Add(x => x.User.DisplayName);
            columns.Add(x => x.Consignee);
            columns.Add(x => x.CreateTime == null ? "" : x.CreateTime.Value.ToString(culture.DateTimeFormat.ShortDatePattern)).SetName("CreateTime");
            columns.Add(x => x.ExecuteAmount);
            columns.Add(x => x.SaleAmount);
            columns.Add(x => x.Pieces).SetName("Pieces");
            columns.Add(x => x.TotalPaid);
            columns.Add(x => x.OrderAmount);
            columns.Add(x => x.PostComment);
            columns.Add(x => x.LockStatusName);
            columns.Add(x => x.OrderStatusName);
            columns.Add(x => x.PayStatusName);
            columns.Add(x => x.HangStatusName);

            GridData gridData = oOrderInfomation.ToGridData(searchModel, columns);

            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        #endregion 订单首页列表

        #region 订单添加

        /// <summary>
        /// 添加新订单
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderWithoutUser()
        {
            oNewOrder = new OrderInformation();
            oNewOrder.OrgID = OrganizationGuidGeneralOrder;
            oNewOrder.ChlID = ChannelGuidGeneralOrder;
            oNewOrder.WhID = WarehouseGuidSelected;
            oNewOrder.DocVersion = 0;
            oNewOrder.LinkCode = "";
            oNewOrder.UserID = Guid.Empty;
            listNewOrderItem = new List<OrderItem>();//情况之前变量
            listNewOrderShipping = new List<OrderShipping>();//清空之前变量
            IsEditing = false;//添加状态
            
            return RedirectToAction("OrderAdd");
        }
        public ActionResult OrderWithUser(Guid? UserGuid)
        {
            ClientGuid = (Guid)UserGuid;
            MemberUser oUser = dbEntity.MemberUsers.Where(p => p.Deleted == false && p.Gid == UserGuid).FirstOrDefault();
            //OrganizationGuidGeneralOrder = oUser.OrgID;
            //ChannelGuidGeneralOrder = oUser.ChlID;
            //oNewOrder.OrgID = OrganizationGuidGeneralOrder;
            //oNewOrder.ChlID = ChannelGuidGeneralOrder;
            oNewOrder.DocVersion = 0;
            oNewOrder.LinkCode = "";
            oNewOrder.User = oUser;
            oNewOrder.UserID = (Guid)UserGuid;
            
            return RedirectToAction("OrderAdd");
        }
        /// <summary>
        /// 手动添加订单,没有选择用户
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderAdd()
        {
            List<SelectListItem> currency = new List<SelectListItem>();
            List<MemberOrgCulture> oCurrency = dbEntity.MemberOrgCultures.Include("Currency").Where(p => p.Deleted == false && p.OrgID == OrganizationGuidGeneralOrder && p.Ctype == 1).OrderBy(p => p.Sorting).ToList();
            foreach (MemberOrgCulture item in oCurrency)
            {
                currency.Add(new SelectListItem { Text = item.Currency.Name.GetResource(CurrentSession.Culture), Value = item.aCurrency.ToString() });
            }
            ViewBag.Currency = currency;
            //陆旻添加，设置组织默认货币
            oNewOrder.aCurrency = oCurrency.ElementAt(0).aCurrency;
            List<FinancePayType> oPayType = dbEntity.FinancePayTypes.Where(p => p.Deleted == false && p.OrgID == OrganizationGuidGeneralOrder).ToList();
            List<SelectListItem> PayType = new List<SelectListItem>();
            foreach (FinancePayType item in oPayType)
            {
                PayType.Add(new SelectListItem { Text = item.Name.GetResource(CurrentSession.Culture), Value = item.Gid.ToString() });
            }
            ViewBag.PayMode = PayType;
            ViewBag.TransList = base.GetSelectList(oNewOrder.TransTypeList);
            
            MemberOrganization org = dbEntity.MemberOrganizations.Where(p => p.Deleted == false && p.Gid == OrganizationGuidGeneralOrder).FirstOrDefault();
            MemberChannel chan = dbEntity.MemberChannels.Where(p => p.Deleted == false && p.Gid == ChannelGuidGeneralOrder).FirstOrDefault();
            
            ViewBag.organization = org.FullName.GetResource(CurrentSession.Culture);
            ViewBag.channel = chan.FullName.GetResource(CurrentSession.Culture);

            oNewOrder.OrgID = OrganizationGuidGeneralOrder;
            oNewOrder.ChlID = ChannelGuidGeneralOrder;
            
            return View(oNewOrder);
        }
    /// <summary>
    /// 订单编辑
    /// </summary>
    /// <returns></returns>    
        public ActionResult OrderEdit(){
            //1.锁定|解锁 2.确认|取消确认 3.挂起|解挂 4.排单|未排单 5.通知收款 6.已付款|未付款 7结算 8.沟通 9.做费
            
            //CurrentSession.IsAdmin.Equals(true) && oNewOrder.TransType.Equals(0) && oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(0)
            //订单修改人登陆
            if (CurrentSession.UserID == oNewOrder.LastModifiedBy || CurrentSession.IsAdmin.Equals(true))
            {
                if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(0))
                {
                    //订单初始状态 10 0000 0000 0000    *2000
                    ViewBag.status = "2000";
                }
                if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(0))
                {
                    //订单锁定状态|订单解挂状态 01 1010 0000 0011    *1A03
                    ViewBag.status = "1A03";
                }                
                if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(5) && oNewOrder.PayStatus.Equals(0))
                {
                    //订单取消状态 01 0000 0000 0010    *1002
                    ViewBag.status = "1002";
                }
                if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(5) && oNewOrder.PayStatus.Equals(0))
                {
                    //订单取消*解锁状态 10 0000 0000 0000    *2000
                    ViewBag.status = "2000";
                }
                if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(1) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(0))
                {
                    //订单挂起状态 01 0001 0000 0010    *1102
                    ViewBag.status = "1102";
                }
                if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(1) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(0))
                {
                    //订单挂起*解锁状态 10 0000 0000 0000    *2000
                    ViewBag.status = "2000";
                }
                if (oNewOrder.TransType.Equals(0)||oNewOrder.TransType.Equals(2))
                {                  
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(1) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单确定状态*款到发货 01 0100 0011 0010    *1432
                        ViewBag.status = "1432";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(1) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单确定*解锁状态*款到发货 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(1) && oNewOrder.PayStatus.Equals(2))
                    {
                        //订单通知收款状态*款到发货 01 0100 0000 1010    *140A
                        ViewBag.status = "140A";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(1) && oNewOrder.PayStatus.Equals(2))
                    {
                        //订单通知收款*解锁状态*款到发货 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(2))
                    {
                        //订单通知收款*未确定状态*款到发货 01 1010 0000 0011    *1A03
                        ViewBag.status = "1A03";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(2))
                    {
                        //订单通知收款*未确定*解锁状态*款到发货10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(5) && oNewOrder.PayStatus.Equals(2))
                    {
                        //订单通知收款*未确定*取消状态*款到发货 01 0000 0000 0010    *1002
                        ViewBag.status = "1002";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(5) && oNewOrder.PayStatus.Equals(2))
                    {
                        //订单通知收款*未确定*取消*未锁定状态*款到发货 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(1) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(2))
                    {
                        //订单通知收款*未确定*挂起状态*款到发货 01 0001 0000 0010    *1102
                        ViewBag.status = "1102";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(1) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(2))
                    {
                        //订单通知收款*未确定*挂起*解锁状态*款到发货 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(1) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单未付款状态*款到发货 01 0100 0011 0010    *1432
                        ViewBag.status = "1432";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(1) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单未付款状态*款到发货 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单未付款*未确定状态*款到发货 01 1010 0000 0011    *1A03
                        ViewBag.status = "1A03";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单未付款*未确定*解锁状态*款到发货 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(5) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单未付款*未确定*取消状态*款到发货 01 0000 0000 0010    *1002
                        ViewBag.status = "1002";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(5) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单未付款*未确定*取消状态*未锁定*款到发货 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(1) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单未付款*未确定*挂起状态*款到发货 01 0001 0000 0010    *1102
                        ViewBag.status = "1102";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(1) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单未付款*未确定*挂起*解锁状态*款到发货 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(1) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已付款*未排单状态*款到发货 01 0100 1000 1010    *148A
                        ViewBag.status = "148A";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(1) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已付款*未排单*解锁状态*款到发货 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已付款*未排单*未确定状态*款到发货 01 1010 0000 0011    *1A03
                        ViewBag.status = "1A03";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已付款*未排单*未确定*解锁状态*款到发货 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(5) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已付款*未排单*未确定*取消状态*款到发货 01 0000 0000 1110    *100E
                        ViewBag.status = "100E";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(5) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已付款*未排单*未确定*取消*解锁状态*款到发货 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(1) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已付款*未排单*未确定*挂起状态*款到发货 01 0001 0000 0010    *1102
                        ViewBag.status = "1102";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(1) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已付款*未排单*未确定*挂起*解锁状态*款到发货 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(2) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已排单状态*款到发货 01 0100 0100 1110    *144E
                        ViewBag.status = "144E";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(2) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已排单*解锁状态*款到发货 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(3) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已发货*款到发货 01 0000 0000 1111    *100F
                        ViewBag.status = "100F";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(3) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已发货*款到发货 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(3) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单已发货*未付款*款到发货 01 0000 0001 0111    *1017
                        ViewBag.status = "1017";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(3) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单已发货*未付款*未锁定*款到发货 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(4) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已结算已锁定状态*款到发货 01 0000 0000 0010    *1002
                        ViewBag.status = "1002";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(4) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已结算未锁定状态*款到发货 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                }
                else //货到付款
                {
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(1) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单确定状态 01 0100 1000 0010    *1482
                        ViewBag.status = "1482";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(1) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单确定*解锁状态 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(2) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单已排单状态 01 0100 0111 0010    *1472
                        ViewBag.status = "1472";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(2) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单已排单*解锁状态 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(2) && oNewOrder.PayStatus.Equals(2))
                    {
                        //订单通知收款状态 01 0100 0100 1010    *144A
                        ViewBag.status = "144A";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(2) && oNewOrder.PayStatus.Equals(2))
                    {
                        //订单通知收款*解锁状态 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(1) && oNewOrder.PayStatus.Equals(2))
                    {
                        //订单通知收款*未排单状态 01 0100 1000 0010    *1482
                        ViewBag.status = "1482";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(1) && oNewOrder.PayStatus.Equals(2))
                    {
                        //订单通知收款*未排单*解锁状态 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(2))
                    {
                        //订单通知收款*未确定状态 01 1010 0000 0011    *1A03
                        ViewBag.status = "1A03";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(2))
                    {
                        //订单通知收款*未确定*解锁状态 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(5) && oNewOrder.PayStatus.Equals(2))
                    {
                        //订单通知收款*未确定*取消状态 01 0000 0000 0010    *1002
                        ViewBag.status = "1002";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(5) && oNewOrder.PayStatus.Equals(2))
                    {
                        //订单通知收款*未确定*取消*解锁状态 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(1) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(2))
                    {
                        //订单通知收款*未确定*挂起状态 01 0001 0000 0010    *1102
                        ViewBag.status = "1102";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(1) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(2))
                    {
                        //订单通知收款*未确定*挂起*解锁状态 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(2) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单未付款状态 01 0100 0111 0010    *1472
                        ViewBag.status = "1472";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(2) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单未付款*解锁状态 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(1) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单未付款*未排单状态 01 0100 1000 0010    *1482
                        ViewBag.status = "1482";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(1) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单未付款*未排单*解锁状态 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单未付款*未排单*未确定状态 01 1010 0000 0011    *1A03
                        ViewBag.status = "1A03";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单未付款*未排单*未确定*解锁状态 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(5) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单未付款*未排单*未确定*取消状态 01 0000 0000 0010    *1002
                        ViewBag.status = "1002";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(5) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单未付款*未排单*未确定*取消*未锁定状态 10 0000 0000 0010    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(1) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单未付款*未排单*未确定*挂起状态 01 0001 0000 0010    *1102
                        ViewBag.status = "1102";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(1) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单未付款*未排单*未确定*挂起*解锁状态 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(3) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单已发货*未付款状态 01 0000 0000 1011    *100B
                        ViewBag.status = "100B";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(3) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单已发货*未付款*未锁定状态 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(5) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单已退货*未付款状态 01 0000 0000 1010    *100A
                        ViewBag.status = "100A";
                    } 
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(5) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单已退货*未付款*未锁定状态 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(2) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已付款状态 01 0100 0100 1110    *144E
                        ViewBag.status = "144E";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(2) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已付款*解锁状态 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(1) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已付款*未排单状态 01 0100 1000 0010    *1482
                        ViewBag.status = "1482";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(1) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已付款*未排单*解锁状态 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已付款*未排单*未确定状态 01 1010 0000 0011    *1A03
                        ViewBag.status = "1A03";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已付款*未排单*未确定*解锁状态 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(5) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已付款*未排单*未确定*取消状态 01 0000 0001 0110    *1015
                        ViewBag.status = "1015";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(5) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已付款*未排单*未确定*取消*解锁状态 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(1) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已付款*未排单*未确定*挂起状态 01 0001 0000 0010    *1102
                        ViewBag.status = "1102";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(1) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已付款*未排单*未确定*挂起*解锁状态 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(3) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已付款*已发货*已锁定状态 01 000 0000 1111    *100F
                        ViewBag.status = "100F";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(3) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已付款*已发货*未锁定状态 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.Locking.Equals(1) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(4) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已结算状态已锁定 01 000 0000 0010    *1002
                        ViewBag.status = "1002";
                    }
                    if (oNewOrder.Locking.Equals(0) && oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(4) && oNewOrder.PayStatus.Equals(3))
                    {
                        //订单已结算状态未锁定 10 000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                }
            }
            else  //非订单修改人登陆，  前台生成的订单  
            {
                if (oNewOrder.Locking.Equals(1))
                {
                    //订单锁定状态 0 0000 0000 0000     *0000
                    ViewBag.status = "0000";
                }
                else
                {
                    if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单初始状态 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }                                                            
                    if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(5) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单取消*解锁状态 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }                    
                    if (oNewOrder.Hanged.Equals(1) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(0))
                    {
                        //订单挂起*解锁状态 10 0000 0000 0000    *2000
                        ViewBag.status = "2000";
                    }
                    if (oNewOrder.TransType.Equals(0) || oNewOrder.TransType.Equals(2))
                    {                        
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(1) && oNewOrder.PayStatus.Equals(0))
                        {
                            //订单确定*解锁状态*款到发货 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(1) && oNewOrder.PayStatus.Equals(2))
                        {
                            //订单通知收款*解锁状态*款到发货 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(2))
                        {
                            //订单通知收款*未确定*解锁状态*款到发货10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(5) && oNewOrder.PayStatus.Equals(2))
                        {
                            //订单通知收款*未确定*取消*未锁定状态*款到发货 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(1) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(2))
                        {
                            //订单通知收款*未确定*挂起*解锁状态*款到发货 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(1) && oNewOrder.PayStatus.Equals(0))
                        {
                            //订单未付款状态*款到发货 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(0))
                        {
                            //订单未付款*未确定*解锁状态*款到发货 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(5) && oNewOrder.PayStatus.Equals(0))
                        {
                            //订单未付款*未确定*取消状态*未锁定*款到发货 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(1) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(0))
                        {
                            //订单未付款*未确定*挂起*解锁状态*款到发货 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(1) && oNewOrder.PayStatus.Equals(3))
                        {
                            //订单已付款*未排单*解锁状态*款到发货 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(3))
                        {
                            //订单已付款*未排单*未确定*解锁状态*款到发货 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(5) && oNewOrder.PayStatus.Equals(3))
                        {
                            //订单已付款*未排单*未确定*取消*解锁状态*款到发货 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(1) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(3))
                        {
                            //订单已付款*未排单*未确定*挂起*解锁状态*款到发货 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(2) && oNewOrder.PayStatus.Equals(3))
                        {
                            //订单已排单*解锁状态*款到发货 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(3) && oNewOrder.PayStatus.Equals(3))
                        {
                            //订单已发货*解锁状态*款到发货 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(3) && oNewOrder.PayStatus.Equals(0))
                        {
                            //订单已发货*未付款*解锁状态*款到发货 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        } 
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(4) && oNewOrder.PayStatus.Equals(3))
                        {
                            //订单已结算未锁定状态*款到发货 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }
                    }
                    else //货到付款
                    {                        
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(1) && oNewOrder.PayStatus.Equals(0))
                        {
                            //订单确定*解锁状态 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(2) && oNewOrder.PayStatus.Equals(0))
                        {
                            //订单已排单*解锁状态 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(2) && oNewOrder.PayStatus.Equals(2))
                        {
                            //订单通知收款*解锁状态 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(1) && oNewOrder.PayStatus.Equals(2))
                        {
                            //订单通知收款*未排单*解锁状态 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(2))
                        {
                            //订单通知收款*未确定*解锁状态 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(5) && oNewOrder.PayStatus.Equals(2))
                        {
                            //订单通知收款*未确定*取消*解锁状态 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(1) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(2))
                        {
                            //订单通知收款*未确定*挂起*解锁状态 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                       
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(2) && oNewOrder.PayStatus.Equals(0))
                        {
                            //订单未付款*解锁状态 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(1) && oNewOrder.PayStatus.Equals(0))
                        {
                            //订单未付款*未排单*解锁状态 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(0))
                        {
                            //订单未付款*未排单*未确定*解锁状态 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(5) && oNewOrder.PayStatus.Equals(0))
                        {
                            //订单未付款*未排单*未确定*取消*未锁定状态 10 0000 0000 0010    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(1) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(0))
                        {
                            //订单未付款*未排单*未确定*挂起*解锁状态 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(3) && oNewOrder.PayStatus.Equals(0))
                        {
                            //订单已发货*解锁状态 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(2) && oNewOrder.PayStatus.Equals(3))
                        {
                            //订单已付款*解锁状态 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(1) && oNewOrder.PayStatus.Equals(3))
                        {
                            //订单已付款*未排单*解锁状态 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(3))
                        {
                            //订单已付款*未排单*未确定*解锁状态 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(5) && oNewOrder.PayStatus.Equals(3))
                        {
                            //订单已付款*已退货*解锁状态 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(5) && oNewOrder.PayStatus.Equals(3))
                        {
                            //订单已付款*未排单*未确定*取消*解锁状态 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }                        
                        if (oNewOrder.Hanged.Equals(1) && oNewOrder.Ostatus.Equals(0) && oNewOrder.PayStatus.Equals(3))
                        {
                            //订单已付款*未排单*未确定*挂起*解锁状态 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(3) && oNewOrder.PayStatus.Equals(3))
                        {
                            //订单已付款*已退货*解锁状态 10 0000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }
                        if (oNewOrder.Hanged.Equals(0) && oNewOrder.Ostatus.Equals(4) && oNewOrder.PayStatus.Equals(3))
                        {
                            //订单已结算状态未锁定 10 000 0000 0000    *2000
                            ViewBag.status = "2000";
                        }
                    }
                    
                }
                                
            }
            //bChangeOrder = false;
            bOrderBaseInfoEdit = false;
            bOrderItemInfoEdit = false;
            bOrderFeeInfoEdit = false;
            bSaveEditOrderInfo = false;
            //判断 取消-退货 状态转换
            CancelOrReturn();
            return View(oNewOrder);
        }
        /// <summary>
        /// 判断 取消-退货 状态转换
        /// </summary>
        public void CancelOrReturn()
        {
            List<WarehouseStockOut> oChange = dbEntity.WarehouseStockOuts.Where(p => p.Deleted == false && p.RefType == (byte)ModelEnum.NoteType.ORDER && p.RefID == oNewOrder.Gid).ToList();
            if (oChange != null)
            {
                foreach (WarehouseStockOut item in oChange)
                {
                    if (item.Ostatus == 2)
                    {
                        ViewBag.change = true;
                        break;
                    }
                    else
                    {
                        ViewBag.change = false;
                    }
                }
            }
            else
            {
                ViewBag.change = false;
            }
        }
        /// <summary>
        /// 订单挂起原因弹出页面
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderHangedAlert()
        {
            List<SelectListItem> modelList = new List<SelectListItem>();
            modelList.Add(new SelectListItem { Text = LiveAzure.Resource.Stage.OrderController.CHANGE, Value = "1", Selected = true });
            modelList.Add(new SelectListItem { Text = LiveAzure.Resource.Stage.OrderController.REJECT, Value = "2", Selected = false });
            modelList.Add(new SelectListItem { Text = LiveAzure.Resource.Stage.OrderController.NONE, Value = "0", Selected = false });
            ViewBag.HtypeList = modelList;
            return View();
        }
        /// <summary>
        /// 沟通页面
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderTalkAlert()
        {
            return View();
        }
        /// <summary>
        /// 操作日志显示
        /// </summary>
        /// <returns></returns>
        public ActionResult LogAction()
        {            
            return View();
        }
        /// <summary>
        /// 日志列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListGeneralAction(SearchModel searchModel)
        {
            searchModel.SortOrder = "desc";
            IQueryable<GeneralAction> oAction = dbEntity.GeneralActions.Include("User").Where(p => p.RefID == oNewOrder.Gid).AsQueryable().OrderByDescending(p=>p.LastModifyTime);
            CultureInfo culture = new CultureInfo(CurrentSession.Culture);
            GridColumnModelList<GeneralAction> columns = new GridColumnModelList<GeneralAction>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.UserID==Guid.Empty ? "" : p.User.LoginName).SetName("Name");
            columns.Add(p => p.LastModifyTime.Value.ToString(culture.DateTimeFormat.FullDateTimePattern)).SetName("Time");
            columns.Add(p => p.Matter);
            
            GridData gridData = oAction.ToGridData(searchModel, columns);
            
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 由订单当前状态选择编辑页面的按钮显示状态
        /// </summary>
        /// <param name="sStatus">订单状态</param>
        /// <param name="TransType">交易方式</param>
        /// <returns>按钮状态代码</returns>
        public string OrderStatusSelect(string sStatus, string TransType)
        {
            string returnString="0000";
            if (TransType == "0" || TransType == "2")
            {
                switch (sStatus)
                {
                    case "0000":
                        returnString = "2000";
                        break;
                    case "1000":
                        returnString = "1A03";
                        break;
                    case "1050":
                        returnString = "1002";
                        break;
                    case "0050":
                        returnString = "2000";
                        break;
                    case "1100":
                        returnString = "1102";
                        break;
                    case "0100":
                        returnString = "2000";
                        break;
                    case "1010":
                        returnString = "1432";
                        break;
                    case "0010":
                        returnString = "2000";
                        break;
                    case "1012":
                        returnString = "140A";
                        break;
                    case "0012":
                        returnString = "2000";
                        break;
                    case "1002":
                        returnString = "1A03";
                        break;
                    case "0002":
                        returnString = "2000";
                        break;
                    case "1052":
                        returnString = "1002";
                        break;
                    case "0052":
                        returnString = "2000";
                        break;
                    case "1102":
                        returnString = "1102";
                        break;
                    case "0102":
                        returnString = "2000";
                        break;
                    case "1013":
                        returnString = "148A";
                        break;
                    case "0013":
                        returnString = "2000";
                        break;
                    case "1003":
                        returnString = "1A03";
                        break;
                    case "0003":
                        returnString = "2000";
                        break;
                    case "1053":
                        returnString = "100F";
                        break;
                    case "0053":
                        returnString = "2000";
                        break;
                    case "1103":
                        returnString = "1102";
                        break;
                    case "0103":
                        returnString = "2000";
                        break;
                    case "1023":
                        returnString = "144E";
                        break;
                    case "0023":
                        returnString = "2000";
                        break;
                    case "1033":
                        returnString = "100F";
                        break;
                    case "0033":
                        returnString = "2000";
                        break;
                    case "1030":
                        returnString = "1017";
                        break;
                    case "0030":
                        returnString = "2000";
                        break;
                    case "1043":
                        returnString = "1002";
                        break;
                    case "0043":
                        returnString = "2000";
                        break;
                    default:
                        returnString = "0000";
                        break;
                }
            }
            else
            {
                switch (sStatus)
                {
                    case "0000":
                        returnString = "2000";
                        break;
                    case "1000":
                        returnString = "1A03";
                        break;
                    case "1050":
                        returnString = "100A";
                        break;
                    case "0050":
                        returnString = "2000";
                        break;
                    case "1100":
                        returnString = "1102";
                        break;
                    case "0100":
                        returnString = "2000";
                        break;
                    case "1030":
                        returnString = "100B";
                        break;
                    case "0030":
                        returnString = "2000";
                        break;
                    case "1010":
                        returnString = "1482";
                        break;
                    case "0010":
                        returnString = "2000";
                        break;
                    case "1020":
                        returnString = "1472";
                        break;
                    case "0020":
                        returnString = "2000";
                        break;
                    case "1022":
                        returnString = "144A";
                        break;
                    case "0022":
                        returnString = "2000";
                        break;
                    case "1012":
                        returnString = "1482";
                        break;
                    case "0012":
                        returnString = "2000";
                        break;
                    case "1002":
                        returnString = "1A03";
                        break;
                    case "0002":
                        returnString = "2000";
                        break;
                    case "1052":
                        returnString = "1002";
                        break;
                    case "0052":
                        returnString = "2000";
                        break;
                    case "1102":
                        returnString = "1102";
                        break;
                    case "0102":
                        returnString = "2000";
                        break;
                    case "1023":
                        returnString = "144E";
                        break;
                    case "0023":
                        returnString = "2000";
                        break;
                    case "1013":
                        returnString = "1482";
                        break;
                    case "0013":
                        returnString = "2000";
                        break;
                    case "1003":
                        returnString = "1A03";
                        break;
                    case "0003":
                        returnString = "2000";
                        break;
                    case "1053":
                        returnString = "1015";
                        break;
                    case "0053":
                        returnString = "2000";
                        break;
                    case "1103":
                        returnString = "1102";
                        break;
                    case "0103":
                        returnString = "2000";
                        break;
                    case "1033":
                        returnString = "100F";
                        break;
                    case "1043":
                        returnString = "1002";                       
                        break;
                    case "0033":
                        returnString = "2000";
                        break;
                    case "0043":
                        returnString = "2000";
                        break;
                    default:
                        returnString = "0000";
                        break;
                }
            }            
            return returnString;
        }
        /// <summary>
        /// 查看选择的订单信息
        /// </summary>
        /// <param name="orderGid"></param>
        /// <returns></returns>
        public string SetViewOrderInfo(Guid orderGid) 
        {
            //查询出查看的订单信息
            OrderInformation oViewOrder = dbEntity.OrderInformations.Include("Organization").Include("Channel").Include("User").Include("Warehouse").Include("PayType").Include("Currency").Include("Location").Include("OrderItems").Include("OrderProcesses").Include("OrderAttributes").Include("OrderHistories").Where(p => p.Gid == orderGid && p.Deleted == false).FirstOrDefault();
            //设置订单页面编辑状态
            if (oViewOrder.Hanged == (byte)ModelEnum.HangStatus.HANGED)
                bChangeOrder = true;
            else
                bChangeOrder = false;
            if (oViewOrder.Equals(null))
            {
                return "failure";
            }
            oNewOrder = oViewOrder;
            listNewOrderItem.Clear();//清空缓存区商品列表
            foreach (OrderItem item in oNewOrder.OrderItems)
            {
                OrderItem obj = dbEntity.OrderItems.Include("OnSkuItem").Include("SkuItem").Where(o => o.Gid == item.Gid && o.Deleted==false).FirstOrDefault();
                if (obj != null)
                    listNewOrderItem.Add(obj);
            }
            listNewOrderShipping.Clear();//清空缓存区承运商列表
            oNewShipper = new ShippingInformation();
            //-----------------------------------------------------------------edit by 2011/10/24 手工订单编辑可以选择该组织所有支持的承运商
            //foreach (OrderShipping ship in oNewOrder.OrderShippings)
            //{
            //    OrderShipping obj = dbEntity.OrderShippings.Include("Shipper").Where(o => o.Gid == ship.Gid && o.Deleted == false).FirstOrDefault();
            //    if (obj != null)
            //    {
            //        if (obj.Candidate)//将原MODEL中选中的承运商赋给oNewShipper
            //            oNewShipper = obj.Shipper;
            //        listNewOrderShipping.Add(obj);
            //    }
            //}
            List<ShippingInformation> listOrgShippers = dbEntity.ShippingInformations.Where(s => s.Deleted == false && s.aParent == oNewOrder.OrgID).ToList();//组织支持的承运商
            foreach (ShippingInformation ship in listOrgShippers)
            {
                OrderShipping oShip = new OrderShipping {  OrderID = oNewOrder.Gid,ShipID = ship.Gid};
                listNewOrderShipping.Add(oShip);
            }
            foreach (OrderShipping ship in oNewOrder.OrderShippings)
            {
                OrderShipping obj = dbEntity.OrderShippings.Include("Shipper").Where(o => o.Gid == ship.Gid && o.Deleted == false).FirstOrDefault();
                if (obj != null)
                {
                    if (obj.Candidate)//将原MODEL中选中的承运商赋给oNewShipper
                        oNewShipper = obj.Shipper;
                }
            }
            //---------------------------------------------------------------------------------------------------------------------------------
            return "success";
        }
        /// <summary>
        /// 设置订单结算货币
        /// </summary>
        /// <param name="currencyId"></param>
        public void SetOrderCurrency(Guid currencyId)
        {
            Guid testGid = (Guid)oNewOrder.aCurrency;
            oNewOrder.aCurrency = currencyId;
        }
        /// <summary>
        /// 保存新建的订单
        /// </summary>
        /// <returns></returns>
        public string SaveNewOrder(OrderInformation oBackOrder,FormCollection formCollection) 
        {
            //oOrderBLL.TestRegion();
            //需要根据组织Gid，渠道Gid以及地区Gid找出最佳的仓库
            if (oNewOrder.OrgID.Equals(null)) 
            {
                return "error";
            }
            if (oNewOrder.ChlID.Equals(null))
            {
                return "error";
            }
            if (oBackOrder.aLocation.Equals(null)) 
            {
                return "error";
            }
            //计算出最佳仓库
            var bestWhID = dbEntity.Database.SqlQuery<Guid>("SELECT dbo.fn_FindBestWarehouse({0}, {1}, {2})", oNewOrder.OrgID, oNewOrder.ChlID, oNewOrder.aLocation).FirstOrDefault();
            if (bestWhID != null)
            {
                oNewOrder.WhID = (Guid)bestWhID;
            }
            else
            {
                return "error";
            }
            //如果订单没有商品，返回
            if (listNewOrderItem.Count <= 0)
            {
                return "error";
            }
            //如果没有最佳承运商，返回
            if (formCollection["shippingListSelect"] == "" || formCollection["shippingListSelect"] ==string.Empty)
            {
                return "error";            
            }

            //保存订单的基本信息
            oNewOrder.User = null;
            oNewOrder.aLocation = oBackOrder.aLocation;
            oNewOrder.LinkCode = oBackOrder.LinkCode;
            oNewOrder.aCurrency = oBackOrder.aCurrency;
            oNewOrder.PayID = oBackOrder.PayID;
            oNewOrder.TransType = oBackOrder.TransType;
            oNewOrder.PayNote = oBackOrder.PayNote;
            oNewOrder.LeaveWord = oBackOrder.LeaveWord;
            oNewOrder.PostComment = oBackOrder.PostComment;
            oNewOrder.Consignee = oBackOrder.Consignee;
            oNewOrder.FullAddress = oBackOrder.FullAddress;
            oNewOrder.PostCode = oBackOrder.PostCode;
            oNewOrder.Telephone = oBackOrder.Telephone;
            oNewOrder.Mobile = oBackOrder.Mobile;
            oNewOrder.Email = oBackOrder.Email;
            oNewOrder.BuildingSign = oBackOrder.BuildingSign;
            oNewOrder.BestDelivery = oBackOrder.BestDelivery;
            //先保存订单信息
            dbEntity.OrderInformations.Add(oNewOrder);
            dbEntity.SaveChanges();

            //保存订单的产品
            decimal Pieces = 0;//定义商品件数
            foreach (OrderItem o in listNewOrderItem)
            {
                Pieces += Math.Round(o.Quantity, o.SkuItem.Percision);//订单商品件数简单相加
                o.SkuItem = null;
                o.OrderID = oNewOrder.Gid;
                dbEntity.OrderItems.Add(o);
            }
            oNewOrder.Pieces = Pieces;//订单商品件数赋值
            dbEntity.SaveChanges();
            //保存订单的承运商
            if (formCollection["shippingListSelect"] != null)//如果有承运商
            {
                Guid shipperID = Guid.Parse(formCollection["shippingListSelect"]);
                foreach (OrderShipping ship in listNewOrderShipping)
                {
                    if (ship.ShipID == shipperID)
                    {
                        ship.Ostatus = (byte)ModelEnum.ShippingCheck.PASSED;//设置选中的承运商
                        ship.Candidate = true;
                    }
                    dbEntity.OrderShippings.Add(ship);
                }
            }
            else
            { 
                //没有承运商
            }
            dbEntity.SaveChanges();
            return "success";
        }
        #endregion 


        #region 订单添加

        #region 收货人信息
        /// <summary>
        /// 设置选中的收货人信息
        /// </summary>
        /// <param name="bSelectAddress"></param>
        /// <param name="memberAddressGid"></param>
        /// <returns></returns>
        public ActionResult OrderConsigneeInfo(bool? bSelectAddress, Guid? memberAddressGid) 
        {
            if (bSelectAddress == true)
            {
                Guid addressGid = (Guid)memberAddressGid;
                MemberAddress oMemberAddress = dbEntity.MemberAddresses.Include("User").Include("Location").Where(p => p.Deleted == false && p.Gid == memberAddressGid).FirstOrDefault();
                
                if (oMemberAddress != null)
                {
                    oNewOrder.Consignee = oMemberAddress.DisplayName;
                    oNewOrder.aLocation = oMemberAddress.aLocation;
                    oNewOrder.FullAddress = oMemberAddress.FullAddress;
                    oNewOrder.PostCode = oMemberAddress.PostCode;
                    oNewOrder.Telephone = oMemberAddress.HomePhone;
                    oNewOrder.Mobile = oMemberAddress.CellPhone;
                    oNewOrder.Email = oMemberAddress.Email;
                }
            }

            return RedirectToAction("OrderAdd");
        }

        public ActionResult OrderEditConsigneeInfo(bool? bSelectAddress, Guid? memberAddressGid)
        {
            if (bSelectAddress == true)
            {
                Guid addressGid = (Guid)memberAddressGid;
                MemberAddress oMemberAddress = dbEntity.MemberAddresses.Include("User").Include("Location").Where(p => p.Deleted == false && p.Gid == memberAddressGid).FirstOrDefault();

                if (oMemberAddress != null)
                {
                    oNewOrder.Consignee = oMemberAddress.DisplayName;
                    oNewOrder.aLocation = oMemberAddress.aLocation;
                    oNewOrder.FullAddress = oMemberAddress.FullAddress;
                    oNewOrder.PostCode = oMemberAddress.PostCode;
                    oNewOrder.Telephone = oMemberAddress.HomePhone;
                    oNewOrder.Mobile = oMemberAddress.CellPhone;
                    oNewOrder.Email = oMemberAddress.Email;
                }
            }
            return null;
        }

        public ActionResult ListOrderUser(SearchModel searchModel) 
        {
            Guid userOrgId = OrganizationGuidGeneralOrder;
            Guid userGid = oNewOrder.UserID;
            if (userGid.Equals(null) || userGid.Equals(Guid.Empty))
            {
                return null;
            }
            IQueryable<MemberAddress> oMemberAddress = dbEntity.MemberAddresses.Include("User").Where(p => p.Deleted == false && p.UserID == userGid).AsQueryable();

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

        public string OrderLocationChange(Guid locationGid)
        {
            oNewOrder.aLocation = locationGid;
            if (oNewOrder.OrgID.Equals(null))
            {
                return "error";
            }
            if (oNewOrder.ChlID.Equals(null))
            {
                return "error";
            }
            if (oNewOrder.aLocation.Equals(null))
            {
                return "error";
            }
            //计算出最佳仓库
            var bestWhID = dbEntity.Database.SqlQuery<Guid>("SELECT dbo.fn_FindBestWarehouse({0}, {1}, {2})", oNewOrder.OrgID, oNewOrder.ChlID, oNewOrder.aLocation).FirstOrDefault();
            if (bestWhID != null)
            {
                oNewOrder.WhID = (Guid)bestWhID;
            }
            else
            {
                return "error";
            }

            return "success"; 
        }

        #endregion

        #region 选择商品
        /// <summary>
        /// 订单添加的商品列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult OrderItemList(SearchModel searchModel)
        {
            IQueryable<OrderItem> listOrderItem = listNewOrderItem.AsQueryable();
            GridColumnModelList<OrderItem> columns = new GridColumnModelList<OrderItem>();
            columns.Add(p => p.OnSkuID).SetAsPrimaryKey().SetHidden(true);
            columns.Add(p => p.Name).SetCaption(@LiveAzure.Resource.Model.Order.OrderItem.Name);
            columns.Add(p => p.SkuItem.Code).SetCaption(@LiveAzure.Resource.Model.Product.ProductOnItem.SkuItem);
            columns.Add(p => p.SkuItem.Barcode).SetCaption(@LiveAzure.Resource.Model.Product.ProductInfoItem.Barcode);
            columns.Add(p => p.ExecutePrice).SetCaption(@LiveAzure.Resource.Model.Order.OrderItem.ExecutePrice);
            columns.Add(p => p.SalePrice).SetCaption(@LiveAzure.Resource.Model.Order.OrderItem.SalePrice);
            columns.Add(p => p.Quantity).SetCaption(@LiveAzure.Resource.Model.Order.OrderItem.Quantity);
            columns.Add(p => getStdUnit(p.SkuItem.StdUnit)).SetName("StdUnit");
            columns.Add(p => p.ExecutePrice * p.Quantity).SetName("Amount").SetCaption("小计");
            GridData gridData = listOrderItem.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取SKU标准计量单位
        /// </summary>
        /// <returns></returns>
        public string getStdUnit(Guid StdUnit)
        {
            string result = "";
            GeneralMeasureUnit oMeasureUnit = dbEntity.GeneralMeasureUnits.Where(g => g.Gid == StdUnit && g.Utype == (byte)ModelEnum.MeasureUnit.PIECE).FirstOrDefault();
            if(oMeasureUnit != null)
                result = oMeasureUnit.Name.GetResource(CurrentSession.Culture);
            return result;
        }
        /// <summary>
        /// 判断是否可添加OrderItem
        /// </summary>
        /// <returns></returns>
        public string CanAddOrderItem()
        {
            if (oNewOrder.OrgID == null || oNewOrder.OrgID == Guid.Empty)
                return "Org_NULL";
            else return "Success";
        }

        /// <summary>
        /// 订单商品添加页面
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderItemAddPage()
        {
            //搜索字段的下拉框
            List<SelectListItem> listSearchProperty = new List<SelectListItem>();
            Dictionary<string, string> dictionaryProperty = new Dictionary<string, string>();
            dictionaryProperty.Add(LiveAzure.Resource.Model.Product.ProductInfoItem.Code,"Code");
            dictionaryProperty.Add(LiveAzure.Resource.Model.Product.ProductOnItem.FullName,"FullName");
            for (int i = 0; i < dictionaryProperty.Count; i++)
            {
                listSearchProperty.Add(new SelectListItem { Text = dictionaryProperty.ElementAt(i).Key, Value = dictionaryProperty.ElementAt(i).Value });
            }
            ViewBag.SearchProperty = listSearchProperty;
            //搜索条件的下拉框
            List<SelectListItem> listSearchMode = new List<SelectListItem>();
            Dictionary<string, string> dictionaryModes = new Dictionary<string, string>();
            dictionaryModes.Add("以...开头", "0");
            dictionaryModes.Add("包含", "1");
            dictionaryModes.Add("以...结尾", "2");
            for (int i = 0; i < dictionaryModes.Count; i++)
            {
                listSearchMode.Add(new SelectListItem { Text = dictionaryModes.ElementAt(i).Key, Value = dictionaryModes.ElementAt(i).Value });
            }
            ViewBag.SearchMode = listSearchMode;
            return View();
        }

        /// <summary>
        /// 搜索产品
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="orderSearchMode">搜索的条件</param>
        /// <param name="orderSearchValue">搜索的值</param>
        /// <param name="orderSearchProperty">搜索的字段</param>
        /// <param name="orderSearchRelationship">多条件时的关系</param>
        /// <returns></returns>
        public ActionResult OrderItemSearchList(SearchModel searchModel, string orderSearchMode, string orderSearchValue, string orderSearchProperty, string orderSearchRelationship)
        {
            if (orderSearchMode == null)
            { return null; }
            Guid organizationId = oNewOrder.OrgID;
            Guid channleId = oNewOrder.ChlID;
            string[] OrderSearchMode = orderSearchMode.Split('|');
            string[] OrderSearchValue = orderSearchValue.Split('|');
            string[] OrderSearchProperty = orderSearchProperty.Split('|');
            string[] OrderSearchRelationship = orderSearchRelationship.Split('|');
            List<ProductOnItem> listSearchSku_0 = new List<ProductOnItem>();
            List<ProductOnItem> listSearchSku_1 = new List<ProductOnItem>(); 
            List<ProductOnItem> listSearchSku_2 = new List<ProductOnItem>();
            List<ProductOnItem> result_1 = new List<ProductOnItem>();
            List<ProductOnItem> result_2 = new List<ProductOnItem>();
            ArrayList listSearchSku = new ArrayList{listSearchSku_0,listSearchSku_1,listSearchSku_2};
            //先列出该组织，该渠道下的 商品
            List<ProductOnItem> list = dbEntity.ProductOnItems.Where(p => p.SkuItem.OrgID == organizationId && p.OnSale.ChlID == channleId && p.Deleted == false).ToList();
//            string sSqlLine = @"SELECT * FROM ProductOnItem poi
//                                     JOIN ProductItem pi ON pi.Gid = poi.SkuID
//                                     JOIN viewResourceMatter rm ON poi.FullName = rm.Gid
//                                     WHERE rm.Matter LIKE '%'
//
//                                 ";
//            List<ProductOnItem> list2 = dbEntity.Database.SqlQuery<ProductOnItem>(sSqlLine);
                for (var i = 0; i <= 2; i++)
                {
                    if (i > 0)
                    {
                        if (OrderSearchRelationship[i-1] == "undefined") continue;
                    }
                    switch (OrderSearchProperty[i])
                    {
                        case "Code":
                            if (OrderSearchMode[i] == "0") listSearchSku[i] = (from p in list
                                                                          where p.SkuItem.Code.StartsWith(OrderSearchValue[i])
                                                                          //&& p.SkuItem.OrgID == organizationId 
                                                                          //&& p.OnSale.ChlID == channleId
                                                                          //&& p.Deleted == false
                                                                       select p).ToList();
                            if (OrderSearchMode[i] == "1") listSearchSku[i] = (from p in list
                                                                          where p.SkuItem.Code.Contains(OrderSearchValue[i])
                                                                          //&& p.SkuItem.OrgID == organizationId
                                                                          //&& p.OnSale.ChlID == channleId
                                                                          //&& p.Deleted == false
                                                                       select p).ToList();
                            if (OrderSearchMode[i] == "2") listSearchSku[i] = (from p in list
                                                                          where p.SkuItem.Code.EndsWith(OrderSearchValue[i])
                                                                          //&& p.SkuItem.OrgID == organizationId
                                                                          //&& p.OnSale.ChlID == channleId
                                                                          //&& p.Deleted == false
                                                                       select p).ToList();
                            break;
                        case "FullName":
                            if (OrderSearchMode[i] == "0") listSearchSku[i] = (from p in list
                                                                          where p.FullName.GetResource(CurrentSession.Culture).StartsWith(OrderSearchValue[i])
                                                                          //&& p.SkuItem.OrgID == organizationId
                                                                          //&& p.OnSale.ChlID == channleId
                                                                          //&& p.Deleted == false
                                                                       select p).ToList();
                            if (OrderSearchMode[i] == "1") listSearchSku[i] = (from p in list
                                                                          where p.FullName.GetResource(CurrentSession.Culture).Contains(OrderSearchValue[i])
                                                                          //&& p.SkuItem.OrgID == organizationId
                                                                          //&& p.OnSale.ChlID == channleId
                                                                          //&& p.Deleted == false
                                                                       select p).ToList();
                            if (OrderSearchMode[i] == "2") listSearchSku[i] = (from p in list
                                                                          where p.FullName.GetResource(CurrentSession.Culture).EndsWith(OrderSearchValue[i])
                                                                          //&& p.SkuItem.OrgID == organizationId
                                                                          //&& p.OnSale.ChlID == channleId
                                                                          //&& p.Deleted == false
                                                                       select p).ToList();
                            break;
                    }
                }
                listSearchSku_0 = (List<ProductOnItem>)listSearchSku[0];
                listSearchSku_1 = (List<ProductOnItem>)listSearchSku[1];
                listSearchSku_2 = (List<ProductOnItem>)listSearchSku[2];
                if (OrderSearchRelationship[0] != "undefined")
                {
                    if (OrderSearchRelationship[0] == "AND")// 取交集
                    {
                        foreach (var item in listSearchSku_0)
                        {
                            if (listSearchSku_1.Contains(item))
                                result_1.Add(item);
                        }
                    }
                    else//取并集
                    {
                        result_1 = listSearchSku_0;
                        foreach (var item in listSearchSku_1)
                        {
                            if (!listSearchSku_0.Contains(item))
                                result_1.Add(item);
                        }
                    }
                }
                else
                {
                    result_1 = listSearchSku_0;
                }
                if (OrderSearchRelationship[1] != "undefined")
                {
                    if (OrderSearchRelationship[1] == "AND")
                    {
                        foreach (var item in result_1)
                        {
                            if (listSearchSku_2.Contains(item))
                                result_2.Add(item);
                        }
                    }
                    else
                    {
                        result_2 = result_1;
                        foreach (var item in listSearchSku_2)
                        {
                            if (!result_1.Contains(item))
                                result_2.Add(item);
                        }
                    }
                }
                else
                {
                    result_2 = result_1;
                }
            IQueryable<ProductOnItem> Result = result_2.AsQueryable();
            GridColumnModelList<ProductOnItem> columns = new GridColumnModelList<ProductOnItem>();
            columns.Add(p => p.Gid).SetAsPrimaryKey().SetHidden(true);
            columns.Add(p => p.FullName.GetResource(CurrentSession.Culture)).SetName("FullName.Matter");
            columns.Add(p => p.SkuItem.Code);
            GridData gridData = Result.ToGridData(searchModel, columns);
            return Json(gridData,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加选中的商品，并返回该ONSKU详细信息
        /// </summary>
        /// <param name="OnSkuId"></param>
        /// <returns></returns>
        public ActionResult OrderItemtoAdd(Guid OnSkuId)
        {
            ViewBag.Isediting = IsEditing.ToString();//add by 2011/10/20 tianyou
            OrderItem oOrderItem = new OrderItem();
            ProductOnItem oOnSku = dbEntity.ProductOnItems.Include("SkuItem").Where(p => p.Gid == OnSkuId && p.Deleted == false).FirstOrDefault();
            oOrderItem.OnSkuID = OnSkuId;
            oOrderItem.SkuID = oOnSku.SkuID;
            //找到标准计量单位的价格
            ProductOnUnitPrice oUnitPrice = dbEntity.ProductOnUnitPrices.Where(u => u.OnSkuID == OnSkuId && u.aShowUnit == oOnSku.SkuItem.StdUnit).FirstOrDefault();
            oOrderItem.MarketPrice = oUnitPrice.MarketPrice.GetResource(oNewOrder.aCurrency.Value);
            oOrderItem.SalePrice = oUnitPrice.SalePrice.GetResource(oNewOrder.aCurrency.Value);
            oOrderItem.ExecutePrice = oOrderItem.SalePrice;
            oOrderItem.Name = oOnSku.FullName.GetResource(CurrentSession.Culture);
            ViewBag.StdUnit = getStdUnit(oOnSku.SkuItem.StdUnit);
            return View(oOrderItem);
        }

        /// <summary>
        /// 商品添加，保存到缓存listNewOrderItem
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool OrderItemAdd(OrderItem model)
        {
            if (listNewOrderItem.Any(li => li.OnSkuID == model.OnSkuID))
            {//已经添加过的OnSku,则数量相加，执行价格按新设置的价格算，待发货数相加
                listNewOrderItem.Find(li => li.OnSkuID == model.OnSkuID).Quantity += model.Quantity;
                listNewOrderItem.Find(li => li.OnSkuID == model.OnSkuID).ExecutePrice = model.ExecutePrice;
                listNewOrderItem.Find(li => li.OnSkuID == model.OnSkuID).TobeShip += model.Quantity;
            }
            else
            {
                model.SkuItem = dbEntity.ProductInfoItems.Find(model.SkuID);
                model.TobeShip =model.Quantity;
                listNewOrderItem.Add(model);
            }
            return true;
        }

        /// <summary>
        /// 商品编辑，保存到缓存listNewOrderItem
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool OrderItemEdit(OrderItem model)
        {
            //将缓存区的OrderItem删除
            OrderIremRemove(model.OnSkuID);
            //添加新的OrderItem
            model.SkuItem = dbEntity.ProductInfoItems.Find(model.SkuID);
            model.TobeShip = model.Quantity;
            listNewOrderItem.Add(model);
            return true;
        }

        /// <summary>
        /// 从缓存列表listNewOrderItem删除商品的添加
        /// </summary>
        /// <param name="OrderItemRemoveID">所选中的待删除OrderItem的ID</param>
        /// <returns></returns>
        public bool OrderIremRemove(Guid ONSKUID)
        {
            OrderItem oItemtoRemove = listNewOrderItem.Where(o => o.OnSkuID == ONSKUID).FirstOrDefault();
            if (oItemtoRemove != null)
            {
                listNewOrderItem.Remove(oItemtoRemove);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// 编辑缓存列表listNewOrderItem中的商品
        /// </summary>
        /// <param name="OrderItemRemoveID">所选中的待编辑的OrderItem的ID</param>
        /// <returns></returns>
        public ActionResult OrderItemtoEdit(Guid ONSKUID)
        {
            ViewBag.IsEditing = IsEditing;
            OrderItem oEditModel = listNewOrderItem.Where(o => o.OnSkuID == ONSKUID).FirstOrDefault();
            return View(oEditModel);
        }
        #endregion

        #region 订单操作日志

        public ActionResult OrderProcessList() 
        {
            return View();
        }

        public ActionResult ListOrderProcess(SearchModel searchModel) 
        {
            //需要换成全局的订单
            Guid orderGid = Guid.Parse("e1c5e9ad-73de-e011-bcc4-4437e63336bd");
            CultureInfo culture = new CultureInfo(CurrentSession.Culture);
            IQueryable<OrderProcess> oOrderProcess = dbEntity.OrderProcesses.Include("Order").Where(p => p.Deleted == false && p.OrderID == orderGid).AsQueryable();

            GridColumnModelList<OrderProcess> columns = new GridColumnModelList<OrderProcess>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Code);
            columns.Add(p => p.Order.Code);
            columns.Add(p => p.Matter);
            columns.Add(p => p.LastModifiedBy == null ? "" : dbEntity.MemberUsers.Find(p.LastModifiedBy).LoginName).SetName("Modifier");
            columns.Add(p => p.LastModifyTime == null ? "" : p.LastModifyTime.Value.ToString(culture.DateTimeFormat.ShortDatePattern)).SetName("ModifyTime");
            columns.Add(p => p.Remark);

            GridData gridData = oOrderProcess.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 承运商信息

        //public ActionResult OrderSupportShippingList() 
        //{
        //    return View();
        //}
        ///// <summary>
        ///// 编辑订单时候显示的承运商列表
        ///// </summary>
        ///// <param name="searchModel"></param>
        ///// <returns></returns>
        //public ActionResult ListOrderSupportShipping(SearchModel searchModel) 
        //{
        //    //需要对应全局的订单Guid
        //    Guid orderGid = oNewOrder.Gid;

        //    IQueryable<OrderShipping> oOrderShipping = dbEntity.OrderShippings.Include("Order").Include("Shipper").Where(p => p.Deleted == false && p.OrderID == orderGid).AsQueryable();
            
        //    GridColumnModelList<OrderShipping> columns = new GridColumnModelList<OrderShipping>();
        //    columns.Add(p => p.Gid).SetAsPrimaryKey();
        //    columns.Add(p => p.Shipper.FullName.GetResource(CurrentSession.Culture)).SetName("ShipperName");
        //    columns.Add(p => p.ShipWeight);
        //    columns.Add(p => p.ShippingCheckName).SetName("ShipStatus");
        //    columns.Add(p => p.Candidate);
        //    columns.Add(p => p.Remark);

        //    GridData gridData = oOrderShipping.ToGridData(searchModel, columns);
            
        //    return Json(gridData, JsonRequestBehavior.AllowGet);

        //}

        /// <summary>
        /// 添加订单时列出支持的承运商
        /// </summary>
        /// <returns></returns>
        public ActionResult ShippingListShow()
        {
            OrderBLL oOrderBLL = new OrderBLL(this.dbEntity);//新建oOrderBLL对象
            List<ShippingInformation> oShippingList = new List<ShippingInformation>();
            List<SelectListItem> shippinglist = new List<SelectListItem>();
            if (oNewOrder.WhID != null && oNewOrder.aLocation != null && listNewOrderItem.Count>=1)
            {
                //-----------edit by 2011/10/24 需求:手动编辑订单是可以选择所有组织支持的承运商
                //获取支持的承运商列表
                //oShippingList = oOrderBLL.GetSupportShippings(listNewOrderItem, (Guid)oNewOrder.WhID, (Guid)oNewOrder.aLocation);
                //获取组织支持的所有承运商
                oShippingList = dbEntity.ShippingInformations.Where(s => s.aParent == oNewOrder.OrgID && s.Deleted == false).ToList();
                //------------------------------------------------------------------------------
                
                listNewOrderShipping.Clear();//每次都清除之前的承运商列表，以保证该列表永远是最新的-----BUG修改09.25
                if (oShippingList.Count > 0)
                {
                    for (int i = 0; i < oShippingList.Count; i++)
                    {
                        OrderShipping oNewOrderShipping = new OrderShipping
                        {
                            ShipID = oShippingList.ElementAt(i).Gid,
                            Order = oNewOrder
                        };
                        listNewOrderShipping.Add(oNewOrderShipping);
                        shippinglist.Add(new SelectListItem { Text = oShippingList.ElementAt(i).FullName.GetResource(CurrentSession.Culture), Value = oShippingList.ElementAt(i).Gid.ToString() });
                    }
                    oNewShipper = oShippingList.ElementAt(0);//默认选中权重最大的，即通过排序查找出的列表的第一个
                    foreach (var reg in oOrderBLL.GetFullRegions((Guid)oNewOrder.aLocation))
                    {
                        if (oNewShipper.Areas != null)//支持区域不为空
                        {
                            foreach (var area in oNewShipper.Areas)
                            {
                                if (area.RegionID == reg.Gid)
                                {
                                    oNewOrder.ResidenceFee = area.Residential == null? 0:area.Residential.GetResource((Guid)oNewOrder.aCurrency);//收取的到门费
                                    oNewOrder.LiftGateFee = area.LiftGate == null? 0:area.LiftGate.GetResource((Guid)oNewOrder.aCurrency);//收取的上楼费
                                    oNewOrder.InstallFee = area.Installation == null? 0:area.Installation.GetResource((Guid)oNewOrder.aCurrency);//收取的安装费
                                }
                            }
                        }
                    }
                }
                else//未找到符合的承运商
                {
                    oNewShipper = new ShippingInformation();
                }
            }
            ViewBag.shippinglist = shippinglist;
            
            return View();
        }

        /// <summary>
        /// 前台页面改变所选的承运商
        /// </summary>
        /// <param name="shipperID">传回的承运商ID</param>
        public void ChangeShipper(Guid shipperID)
        {
            oNewShipper = dbEntity.ShippingInformations.Find(shipperID);
        }
        #endregion

        #region 价格信息
        /// <summary>
        /// 显示价格信息
        /// </summary>
        /// <returns></returns>
        public ActionResult CashInformation()
        {
            //将执行价格算出
            setAmount();
            oNewOrder.TotalFee = oNewOrder.ExecuteAmount + oNewOrder.ShippingFee + oNewOrder.TaxFee + oNewOrder.Insurance + oNewOrder.PaymentFee + oNewOrder.PackingFee + oNewOrder.ResidenceFee + oNewOrder.LiftGateFee
                + oNewOrder.InstallFee + oNewOrder.OtherFee;
            oNewOrder.OrderAmount = oNewOrder.TotalFee - oNewOrder.TotalPaid + oNewOrder.Differ;
            return View(oNewOrder);
        }
        /// <summary>
        /// 执行价价格总计/商品总金额/商品件数
        /// </summary>
        public void setAmount()
        {
            oNewOrder.ExecuteAmount = 0;//清空
            oNewOrder.SaleAmount = 0;
            foreach (var li in listNewOrderItem)
            {
                oNewOrder.ExecuteAmount += li.Quantity * li.ExecutePrice;
                oNewOrder.SaleAmount += li.Quantity * li.SalePrice;
            }
        }
        public ActionResult setPriceInformation(OrderInformation viewmodel)
        {
            oNewOrder.ShippingFee = viewmodel.ShippingFee;
            oNewOrder.TaxFee = viewmodel.TaxFee;
            oNewOrder.Insurance = viewmodel.Insurance;
            oNewOrder.PaymentFee = viewmodel.PaymentFee;
            oNewOrder.PackingFee = viewmodel.PackingFee;
            oNewOrder.ResidenceFee = viewmodel.ResidenceFee;
            oNewOrder.LiftGateFee = viewmodel.LiftGateFee;
            oNewOrder.InstallFee = viewmodel.InstallFee;
            oNewOrder.OtherFee = viewmodel.OtherFee;
            return RedirectToAction("PriceMessageDisplayPage", "Order");
        }

        /// <summary>
        /// 订单价格显示页
        /// </summary>
        /// <returns></returns>
        public ActionResult PriceMessageDisplayPage()
        {

            return View(oNewOrder);
        }

        /// <summary>
        /// 订单价格编辑页
        /// </summary>
        /// <returns></returns>
        public ActionResult PriceMessageEditPage()
        {
            return View(oNewOrder);
        }
        #endregion

        #endregion

        #region 订单状态修改
        /// <summary>
        /// 修改订单锁定状态
        /// </summary>
        /// <param name="bLocking">锁定状态</param>
        /// <returns></returns>
        public string UpdateOrderLocking(bool bLocking)
        {
            string content;
            string Status;            
            //判断订单是否已被他人锁定
            OrderInformation oNewOther = dbEntity.OrderInformations.Where(p=>p.Gid==oNewOrder.Gid && p.LastModifiedBy!=oNewOrder.LastModifiedBy).FirstOrDefault();
            OrderInformation oNew = dbEntity.OrderInformations.Where(p => p.Deleted.Equals(false) && p.Gid == oNewOrder.Gid).Single();
            if (oNewOther != null)//订单被他人锁定
            {
                Status="2000";//返回初始状态
            }
            else  //订单未被他人锁定
            {
                //根据传入的订单锁定值修改订单的状态
                if (bLocking == true)
                {
                    oNew.Locking = (byte)ModelEnum.LockStatus.LOCKED;
                    content = @LiveAzure.Resource.Common.LockStatus.Split(',')[1];
                }
                else
                {
                    oNew.Locking = (byte)ModelEnum.LockStatus.UNLOCK;
                    content = @LiveAzure.Resource.Common.LockStatus.Split(',')[0];
                }                
                dbEntity.SaveChanges();
                oNewOrder = dbEntity.OrderInformations.Where(p => p.Deleted.Equals(false) && p.Gid == oNewOrder.Gid).Single();
                string sStatus = oNewOrder.Locking.ToString() + oNewOrder.Hanged.ToString() + oNewOrder.Ostatus.ToString() + oNewOrder.PayStatus.ToString();
                Status = OrderStatusSelect(sStatus, oNewOrder.TransType.ToString());
                //写操作日志
                oEventBLL.WriteOrderEvent(content, "", this.ToString(), false, oNewOrder.Gid,CurrentSession.UserID);
            }
            //判断 取消-退货 状态转换
            CancelOrReturn();
            return Status;        
        }        
        /// <summary>
        /// 修改订单挂起状态
        /// </summary>
        /// <param name="bHanged">挂起状态</param>
        /// <returns></returns>
        public string UpdateOrderHanged(bool bHanged)
        {
            string content;
            OrderInformation oNew = dbEntity.OrderInformations.Where(p => p.Deleted.Equals(false) && p.Gid == oNewOrder.Gid).Single();
            //根据传入的订单挂起状态修改状态
            if (bHanged == true)
            {
                oNew.Hanged = (byte)ModelEnum.HangStatus.HANGED;
                content = @LiveAzure.Resource.Common.HangStatus.Split(',')[1];
            }
            else
            {
                oNew.Hanged = (byte)ModelEnum.HangStatus.NONE;
                content = @LiveAzure.Resource.Common.HangStatus.Split(',')[0];
            }
            dbEntity.SaveChanges();
            oNewOrder = dbEntity.OrderInformations.Where(p => p.Deleted.Equals(false) && p.Gid == oNewOrder.Gid).Single();
            string sStatus = oNewOrder.Locking.ToString() + oNewOrder.Hanged.ToString() + oNewOrder.Ostatus.ToString() + oNewOrder.PayStatus.ToString();
            string Status = OrderStatusSelect(sStatus, oNewOrder.TransType.ToString());
            //写操作日志
            oEventBLL.WriteOrderEvent(content, "", this.ToString(), false, oNewOrder.Gid, CurrentSession.UserID);
            //判断 取消-退货 状态转换
            CancelOrReturn();
            return Status;
        }
        /// <summary>
        /// 修改订单付款状态
        /// </summary>
        /// <param name="bPayStatus">支付状态</param>
        /// <returns></returns>
        public string UpdateOrderPayStatus(int nPayStatus)
        {
            string content;
            OrderInformation oNew = dbEntity.OrderInformations.Where(p => p.Deleted.Equals(false) && p.Gid == oNewOrder.Gid).Single();
            //根据传入的付款状态修改订单的付款状态
            switch (nPayStatus)
            {
                case 0:
                    oNew.PayStatus = (byte)ModelEnum.PayStatus.NONE;
                    content = @LiveAzure.Resource.Common.PayStatus.Split(',')[0];
                    break;
                case 1:
                    oNew.PayStatus = (byte)ModelEnum.PayStatus.ONPAYMENT;
                    content = @LiveAzure.Resource.Common.PayStatus.Split(',')[1];
                    break;
                case 2:
                    oNew.PayStatus = (byte)ModelEnum.PayStatus.NOTICE;
                    oNew.NoticeTime = DateTimeOffset.Now;
                    content = @LiveAzure.Resource.Common.PayStatus.Split(',')[2];
                    break;
                case 3:
                    oNew.PayStatus = (byte)ModelEnum.PayStatus.PAID;
                    oNew.PaidTime = DateTimeOffset.Now;
                    content = @LiveAzure.Resource.Common.PayStatus.Split(',')[3];
                    break;
                default:
                    oNew.PayStatus = 6;
                    content = "状态错误-error";   
                    break;
            }
            dbEntity.SaveChanges();
            oNewOrder = dbEntity.OrderInformations.Where(p => p.Deleted.Equals(false) && p.Gid == oNewOrder.Gid).Single();
            string sStatus = oNewOrder.Locking.ToString() + oNewOrder.Hanged.ToString() + oNewOrder.Ostatus.ToString() + oNewOrder.PayStatus.ToString();
            string Status = OrderStatusSelect(sStatus, oNewOrder.TransType.ToString());
            //写操作日志
            oEventBLL.WriteOrderEvent(content, "", this.ToString(), false, oNewOrder.Gid, CurrentSession.UserID);
            //判断 取消-退货 状态转换
            CancelOrReturn();
            return Status;
        }
        /// <summary>
        /// 修改订单状态
        /// </summary>
        /// <param name="nStatus">订单状态</param>
        /// <returns></returns>
        public string UpdateOrderStatus(int nStatus)
        {
            //根据传入的订单状态信息修改状态
            //订单状态包括 0待确认 1已确认 2已排单 3已发货 4已结算 5已取消
            //从已确认状态到已排单状态，需要自动生成未确认的出库单；从已排单到未排单的状态，则需要删除相应的出库单。
            //取消状态对应页面订单作废按钮
            string content;
            Guid stockOut;
            OrderInformation oNew = dbEntity.OrderInformations.Where(p => p.Deleted.Equals(false) && p.Gid == oNewOrder.Gid).Single();
            //根据传入的订单确定值修改订单的状态
            switch (nStatus)
            {
                case 0:
                    oNew.Ostatus = (byte)ModelEnum.OrderStatus.NONE;                    
                    content = @LiveAzure.Resource.Common.OrderStatus.Split(',')[0];
                    break;
                case 1:
                    if (oNew.Ostatus == (byte)ModelEnum.OrderStatus.DELIVERIED)
                    {
                        content = "订单已发货，不能返回未排单";
                    }
                    else
                    {
                        if (oNew.Ostatus == (byte)ModelEnum.OrderStatus.ARRANGED)//判断是否从排单状态而来
                        {
                            oNew.Ostatus = (byte)ModelEnum.OrderStatus.CONFIRMED;
                            content = "订单未排单";
                            InventoryByOrder(oNew);
                            //做废出库单
                            DisableWarehouseStockOut(oNew);
                        }
                        else//订单从未确定状态而来
                        {
                            oNew.Ostatus = (byte)ModelEnum.OrderStatus.CONFIRMED;
                            oNew.ConfirmTime = DateTimeOffset.Now;
                            content = @LiveAzure.Resource.Common.OrderStatus.Split(',')[1];
                        }
                    }
                    break;
                case 2:
                    if (oNew.TransType == (byte)ModelEnum.TransType.CASH || oNew.TransType == (byte)ModelEnum.TransType.SECURED)//判断订单是否款到发货
                    {
                        if (!oNew.OrderAmount.Equals(0))
                        {
                            content = "订单未付款";
                            break;
                        }
                    }
                    //货到付款情况
                    int result = warehouseBll.GenerateStockOutFromOrder(oNew.Gid, oNew.UserID, out stockOut);
                    //int result = 0;
                    if (result.Equals(0))
                    {
                        oNew.Ostatus = (byte)ModelEnum.OrderStatus.ARRANGED;
                        oNew.ArrangeTime = DateTimeOffset.Now;
                        content = @LiveAzure.Resource.Common.OrderStatus.Split(',')[2];
                    }
                    else
                    {
                        content = "订单生成出库单失败:"+result.ToString();
                    }
                    break;
                case 4:
                    if (oNew.Ostatus == (byte)ModelEnum.OrderStatus.DELIVERIED && oNew.OrderAmount.Equals(0))
                    {
                        oNew.Ostatus = (byte)ModelEnum.OrderStatus.CLOSED;
                        oNew.ClosedTime = DateTimeOffset.Now;
                        content = @LiveAzure.Resource.Common.OrderStatus.Split(',')[4];
                    }
                    else
                    {
                        content = "未付款|未发货*订单结算失败";
                    }
                    break;
                case 5:
                    oNew.Ostatus = (byte)ModelEnum.OrderStatus.CANCELLED;

                    UpdateOrderHistory(oNew);
                    oNew.Deleted = true;
                    if (oNew.TotalPaid > 0)
                    {
                        RefundFromOrder(oNew);//生成退款单
                    }
                    if (oNew.Ostatus.Equals(3))
                    {
                        content = "退货";
                        ReturnedPurchase(oNew);//生成退货单
                    }
                    else
                    {
                        content = "订单取消";
                    }
                    break;
                default:
                    oNew.Ostatus = 6;
                    content = "操作错误-error";
                    break;
            }
            dbEntity.SaveChanges();
            oNewOrder = dbEntity.OrderInformations.Where(p => p.Deleted.Equals(false) && p.Gid == oNewOrder.Gid).Single();
            string sStatus = oNewOrder.Locking.ToString() + oNewOrder.Hanged.ToString() + oNewOrder.Ostatus.ToString() + oNewOrder.PayStatus.ToString();
            string Status = OrderStatusSelect(sStatus, oNewOrder.TransType.ToString());
            //写操作日志
            oEventBLL.WriteOrderEvent(content, "", this.ToString(), false, oNewOrder.Gid, CurrentSession.UserID);
            //判断 取消-退货 状态转换
            CancelOrReturn();
            InventoryByOrder(oNew);//按SKU核算
            return Status;
        }
        /// <summary>
        /// 更新沟通
        /// </summary>
        /// <returns></returns>
        public string UpdateOrderTalk(string content)
        {
            content = content + @LiveAzure.Resource.Stage.OrderController.TalkWithSelf;
            oNewOrder = dbEntity.OrderInformations.Where(p => p.Deleted.Equals(false) && p.Gid == oNewOrder.Gid).Single();
            string sStatus = oNewOrder.Locking.ToString() + oNewOrder.Hanged.ToString() + oNewOrder.Ostatus.ToString() + oNewOrder.PayStatus.ToString();
            string Status = OrderStatusSelect(sStatus, oNewOrder.TransType.ToString());
            oEventBLL.WriteOrderEvent(content, "", this.ToString(), false, oNewOrder.Gid, CurrentSession.UserID);
            //判断 取消-退货 状态转换
            CancelOrReturn();
            return Status;
        }
        /// <summary>
        /// 取消订单加入历史订单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool UpdateOrderHistory(OrderInformation order)
        {
            OrderHistory oNewOrderHistory = new OrderHistory
            {
                OrderID = order.Gid,
                DocVersion = order.DocVersion,
                Htype = 0,//?
                Reason = "退货",
                RefRefund = null,//?
                RefStockIn = null,//?
                WhID = order.WhID,
                LinkCode = order.LinkCode,
                Ostatus = order.Ostatus,
                Locking = order.Locking,
                PayStatus = order.PayStatus,
                Hanged = order.Hanged,
                HangReason = order.HangReason,
                ReleaseTime = order.ReleaseTime,
                TransType = order.TransType,
                PayID = order.PayID,
                PayNote = order.PayNote,
                Pieces = order.Pieces,
                Currency = order.aCurrency,
                SaleAmount = order.SaleAmount,
                ExecuteAmount = order.ExecuteAmount,
                ShippingFee = order.ShippingFee,
                TaxFee = order.TaxFee,
                Insurance = order.Insurance,
                PaymentFee = order.PaymentFee,
                PackingFee = order.PackingFee,
                ResidenceFee = order.ResidenceFee,
                LiftGateFee = order.LiftGateFee,
                InstallFee = order.InstallFee,
                OtherFee = order.OtherFee,
                TotalFee = order.TotalFee,
                UsePoint = order.UsePoint,
                PointPay = order.PointPay,
                CouponPay = order.CouponPay,
                BounsPay = order.BounsPay,
                MoneyPaid = order.MoneyPaid,
                TotalPaid = order.TotalPaid,
                OrderAmount = order.OrderAmount,
                Differ = order.Differ,
                MergeFrom = order.MergeFrom,
                SplitFrom = order.SplitFrom,
                GetPoint = order.GetPoint,
                ConfirmTime = order.ConfirmTime,
                PaidTime = order.PaidTime,
                ArrangeTime = order.ArrangeTime,
                NoticeTime = order.NoticeTime,
                ClosedTime = order.ClosedTime,
                Consignee = order.Consignee,
                Location = order.aLocation,
                FullAddress = order.FullAddress,
                PostCode = order.PostCode,
                Telephone = order.Telephone,
                Mobile = order.Mobile,
                Email = order.Email,
                ErrorAddress = order.ErrorAddress,
                BestDelivery = order.BestDelivery,
                BuildingSign = order.BuildingSign,
                PostComment = order.PostComment,
                LeaveWord = order.LeaveWord,
                IpAddress = order.IpAddress,
                AdvID = order.AdvID,
                Remark = order.Remark
            };
            dbEntity.OrderHistories.Add(oNewOrderHistory);            
            dbEntity.SaveChanges();
            List<OrderItem> OrderItemList = dbEntity.OrderItems.Where(p => p.OrderID == oNewOrder.Gid && p.Deleted == false).ToList();
            foreach(OrderItem item in OrderItemList){
                item.Deleted = true;
                OrderHisItem oNewOrderHisItem = new OrderHisItem
                {
                    OrderHisID = oNewOrderHistory.Gid,
                    OnSkuID = item.OnSkuID,
                    SkuID = item.SkuID,
                    Name = item.Name,
                    Quantity = item.Quantity,
                    MarketPrice = item.MarketPrice,
                    SalePrice = item.SalePrice,
                    ExecutePrice = item.ExecutePrice,
                    SkuPoint = item.SkuPoint,
                    Remark = item.Remark
                };
                dbEntity.OrderHisItems.Add(oNewOrderHisItem);
                dbEntity.SaveChanges();
            }
            return true;
        }
        /// <summary>
        /// 生成退款
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool RefundFromOrder(OrderInformation order)
        {
            if (order.TotalPaid-order.MoneyPaid>0)
            {
                orderBll.ReturnMoney(order);
            }            
            if (order.MoneyPaid > 0)
            {
                FinancePayment oFinancePayment = new FinancePayment
                {
                    OrgID=order.OrgID,
                    Ptype=(from gpc in dbEntity.GeneralPrivateCategorys
                          where gpc.Deleted.Equals(false) && gpc.Ctype==(byte)ModelEnum.PrivateCategoryType.PAY_TYPE
                          select gpc.Gid).FirstOrDefault(),
                    PayTo=1,
                    Pstatus=0,
                    RefType=(byte)ModelEnum.NoteType.ORDER,
                    RefID=order.Gid,
                    Reason="退",
                    Currency=order.Currency,
                    Amount=order.MoneyPaid,
                    Remark=order.Remark
                };
                dbEntity.FinancePayments.Add(oFinancePayment);
                dbEntity.SaveChanges();
            }
            return true;
        }
        /// <summary>
        /// 生成退货单**等待完美
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool ReturnedPurchase(OrderInformation order)
        {            
            ////生成入库单
            //WarehouseStockIn oStockIn = new WarehouseStockIn
            //{
            //    WhID = (Guid)order.WhID,
            //    Istatus = 0,
            //    InType = dbEntity.GeneralStandardCategorys.Include("Name").Where(p => p.Deleted == false && p.Ctype == 4 && p.Code == "ReturnIn").Select(p => p.Gid).FirstOrDefault(),
            //    RefID = order.Gid,
            //    PrintInSheet = 0,
            //    Prepared = CurrentSession.UserID
            //};
            //dbEntity.WarehouseStockIns.Add(oStockIn);
            //dbEntity.SaveChanges();
            Guid InType = dbEntity.GeneralStandardCategorys.Include("Name").Where(p => p.Deleted == false && p.Ctype == 4 && p.Code == "ReturnIn").Select(p => p.Gid).FirstOrDefault();
            byte RefType = (byte)0;//0:订单号;1:采购单号;
            //跳转到入库单详情页面  StockInAdd(Guid? whID = null,Guid? inType = null ,byte? refType = null, Guid? refID = null)
            RedirectToAction("StockInAdd", "Warehouse",new {order.WhID,InType,RefType,order.Gid});  
            return true;
            
        }
        /// <summary>
        /// 做费出库单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool DisableWarehouseStockOut(OrderInformation order)
        {
            WarehouseStockOut oWarehouseStockOut = dbEntity.WarehouseStockOuts.Where(p => p.Deleted.Equals(false) && p.RefID == oNewOrder.Gid).FirstOrDefault();
            dbEntity.SaveChanges();
            int result = warehouseBll.StockInDiscard(oWarehouseStockOut.Gid, CurrentSession.UserID);
            
            return true;
        }
        
        public bool CanUnhange()
        {
            if (bChangeOrder == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 根据订单更新库存总帐
        /// </summary>
        /// <param name="order"></param>
        public void InventoryByOrder(OrderInformation order)
        {
            List<OrderItem> oGoods = dbEntity.OrderItems.Where(p => p.Deleted == false&&p.OrderID==order.Gid).ToList();
            foreach (OrderItem item in oGoods)
            {
                warehouseBll.InventoryByWarehouseSku((Guid)order.WhID, item.SkuID);
            }
        }
        
        #endregion

        #region 订单编辑

        #region 订单编辑信息页面
        /// <summary>
        /// 订单信息编辑页面
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderEditInfo() 
        {            
            //设置页面每块信息为显示状态还是编辑状态
            ViewBag.bChangeOrder = bChangeOrder;
            ViewBag.bOrderBaseInfoEdit = bOrderBaseInfoEdit;
            ViewBag.bOrderItemInfoEdit = bOrderItemInfoEdit;
            ViewBag.bOrderFeeInfoEdit = bOrderFeeInfoEdit;
            ViewBag.bSaveEditOrderInfo = bSaveEditOrderInfo;
            IsEditing = true;//正在添加
            Guid editOrderOrgID = oNewOrder.OrgID;
            Guid editOrderChID = oNewOrder.ChlID;
            List<SelectListItem> currency = new List<SelectListItem>();
            List<MemberOrgCulture> oCurrency = dbEntity.MemberOrgCultures.Include("Currency").Where(p => p.Deleted == false && p.OrgID == editOrderOrgID && p.Ctype == 1).OrderBy(p => p.Sorting).ToList();
            foreach (MemberOrgCulture item in oCurrency)
            {
                currency.Add(new SelectListItem { Text = item.Currency.Name.GetResource(CurrentSession.Culture), Value = item.aCurrency.ToString() });
            }
            //设置页面用户名称
            Guid userGid = oNewOrder.UserID;
            MemberUser oUser = dbEntity.MemberUsers.Where(p=>p.Gid == userGid && p.Deleted == false).FirstOrDefault();
            ViewBag.UserDisplayName = oUser.DisplayName;
            //设置页面上的结算货币
            ViewBag.Currency = currency;
            List<FinancePayType> oPayType = dbEntity.FinancePayTypes.Where(p => p.Deleted == false && p.OrgID == editOrderOrgID).ToList();
            List<SelectListItem> PayType = new List<SelectListItem>();
            foreach (FinancePayType item in oPayType)
            {
                PayType.Add(new SelectListItem { Text = item.Name.GetResource(CurrentSession.Culture), Value = item.Gid.ToString() });
            }
            //设置页面上的支付方式
            ViewBag.PayMode = PayType;
            ViewBag.TransList = base.GetSelectList(oNewOrder.TransTypeList);
            MemberOrganization orderOrg = dbEntity.MemberOrganizations.Include("ShortName").Where(p => p.Deleted == false && p.Gid == editOrderOrgID).FirstOrDefault();
            //设置页面上显示的组织
            ViewBag.organization = orderOrg.ShortName.GetResource(CurrentSession.Culture);
            MemberChannel orderCh = dbEntity.MemberChannels.Include("ShortName").Where(p => p.Deleted == false && p.Gid == editOrderChID).FirstOrDefault();
            //设置页面上显示的渠道
            ViewBag.channel = orderCh.ShortName.GetResource(CurrentSession.Culture);
            if (!oNewOrder.aCurrency.Equals(null))
            {
                Guid orderCurrencyGid = (Guid)oNewOrder.aCurrency;
                GeneralMeasureUnit orderCurrency = dbEntity.GeneralMeasureUnits.Include("Name").Where(p => p.Deleted == false && p.Gid == orderCurrencyGid).FirstOrDefault();
                oNewOrder.Currency = orderCurrency;
                ViewBag.orderCurrency = orderCurrency.Name.GetResource(CurrentSession.Culture);
            }
            if (!oNewOrder.PayID.Equals(null))
            {
                Guid orderPayTypeGid = (Guid)oNewOrder.PayID;
                FinancePayType orderPayType = dbEntity.FinancePayTypes.Include("Name").Where(p => p.Deleted == false && p.Gid == orderPayTypeGid).FirstOrDefault();
                oNewOrder.PayType = orderPayType;
                ViewBag.orderPayType = orderPayType.Name.GetResource(CurrentSession.Culture);
            }
            //给订单的location重新赋值
            Guid currentALocation = (Guid)oNewOrder.aLocation;
            GeneralRegion currentLocation = dbEntity.GeneralRegions.Where(p => p.Gid == currentALocation && p.Deleted == false).FirstOrDefault();

            oNewOrder.Location = currentLocation;
            if (bOrderBaseInfoEdit == false)
            {
                List<GeneralRegion> listRegion = orderBll.GetFullRegions(currentALocation);
                string regionFullName = "";
                for (int i = listRegion.Count - 1; i >= 0 ; i--)
                {
                    regionFullName = regionFullName + listRegion.ElementAt(i).FullName + "-";
                }
                regionFullName = regionFullName.Remove(regionFullName.Length - 1, 1);
                ViewBag.fullRegionName = regionFullName;
            }

            //shipper信息
            List<SelectListItem> editShippingList = new List<SelectListItem>();
            foreach (OrderShipping ship in listNewOrderShipping)
            {
                ShippingInformation obj = dbEntity.ShippingInformations.Find(ship.ShipID);
                if (obj.Gid == oNewShipper.Gid)//如果是已经选中的承运商 则显示还是选中的
                {
                    editShippingList.Add(new SelectListItem { Value = obj.Gid.ToString(), Text = obj.FullName.GetResource(CurrentSession.Culture), Selected = true });
                }
                else
                {
                    editShippingList.Add(new SelectListItem { Value = obj.Gid.ToString(), Text = obj.FullName.GetResource(CurrentSession.Culture), Selected = false });
                }
            }
            ViewBag.editShippingList = editShippingList;

            //生成价格信息
            oNewOrder.TotalFee = oNewOrder.ExecuteAmount + oNewOrder.ShippingFee + oNewOrder.TaxFee + oNewOrder.Insurance + oNewOrder.PaymentFee + oNewOrder.PackingFee + oNewOrder.ResidenceFee + oNewOrder.LiftGateFee
                + oNewOrder.InstallFee + oNewOrder.OtherFee;
            oNewOrder.OrderAmount = oNewOrder.TotalFee - oNewOrder.TotalPaid;
            return View(oNewOrder);

        }
        /// <summary>
        /// 设置编辑信息的全局变量，设置基本信息编辑
        /// </summary>
        public void SetOrderBaseInfoEdit()
        {
            bOrderBaseInfoEdit = true;
            bOrderItemInfoEdit = false;
            bOrderFeeInfoEdit = false;
        }
        /// <summary>
        /// 设置产品信息编辑
        /// </summary>
        public void SetOrderItemInfoEdit()
        {
            bOrderBaseInfoEdit = false;
            bOrderItemInfoEdit = true;
            bOrderFeeInfoEdit = false;
        }
        /// <summary>
        /// 设置费用信息编辑
        /// </summary>
        public void SetOrderFeeInfoEdit()
        {
            bOrderBaseInfoEdit = false;
            bOrderItemInfoEdit = false;
            bOrderFeeInfoEdit = true;
        }
        /// <summary>
        /// 设置保存变更的全局变量
        /// </summary>
        public void SetSaveEditOrderInfo() 
        {
            bSaveEditOrderInfo = true;
        }
        /// <summary>
        /// 保存页面修改信息，通过编辑的标志位来判断保存不同的信息
        /// </summary>
        /// <param name="oBackOrder"></param>
        /// <returns></returns>
        public ActionResult SaveEditOrderInfo(OrderInformation oBackOrder)
        {
            if (bChangeOrder == true)
            {
                if (bOrderBaseInfoEdit == true && bOrderItemInfoEdit == false && bOrderFeeInfoEdit == false)
                {
                    oNewOrder.LinkCode = oBackOrder.LinkCode;
                    oNewOrder.PayID = oBackOrder.PayID;
                    oNewOrder.aCurrency = oBackOrder.aCurrency;
                    oNewOrder.TransType = oBackOrder.TransType;
                    oNewOrder.LeaveWord = oBackOrder.LeaveWord;
                    oNewOrder.PostComment = oBackOrder.PostComment;
                    oNewOrder.PayNote = oBackOrder.PayNote;

                    oNewOrder.Consignee = oBackOrder.Consignee;
                    oNewOrder.aLocation = oBackOrder.aLocation;
                    var bestWhID = dbEntity.Database.SqlQuery<Guid>("SELECT dbo.fn_FindBestWarehouse({0}, {1}, {2})", oNewOrder.OrgID, oNewOrder.ChlID, oNewOrder.aLocation).FirstOrDefault();
                    if (bestWhID != null)
                    {
                        oNewOrder.WhID = (Guid)bestWhID;
                    }
                    oNewOrder.FullAddress = oBackOrder.FullAddress;
                    oNewOrder.PostCode = oBackOrder.PostCode;
                    oNewOrder.Telephone = oBackOrder.Telephone;
                    oNewOrder.Mobile = oBackOrder.Mobile;
                    oNewOrder.Email = oBackOrder.Email;
                    oNewOrder.BestDelivery = oBackOrder.BestDelivery;
                    oNewOrder.BuildingSign = oBackOrder.BuildingSign;
                    EditShippingListShow();//更新承运商列表信息
                    bOrderBaseInfoEdit = false;
                }
                else if (bOrderBaseInfoEdit == false && bOrderItemInfoEdit == true && bOrderFeeInfoEdit == false)
                {
                    //保存商品信息进入全局变量
                    //oNewOrder.OrderItems.Clear();
                    List<OrderItem> newList = new List<OrderItem>();
                    foreach (OrderItem item in listNewOrderItem)
                    {
                        OrderItem newObj = new OrderItem();
                        newObj.OrderID = item.OrderID;
                        newObj.OnSkuID = item.OnSkuID;
                        newObj.SkuID = item.SkuID;
                        newObj.Name = item.Name;
                        newObj.Quantity = item.Quantity;
                        newObj.TobeShip = item.TobeShip;
                        newObj.Shipped = item.Shipped;
                        newObj.BeReturn = item.BeReturn;
                        newObj.Returned = item.Returned;
                        newObj.MarketPrice = item.MarketPrice;
                        newObj.SalePrice = item.SalePrice;
                        newObj.ExecutePrice = item.ExecutePrice;
                        newObj.SkuPoint = item.SkuPoint;
                        newObj.Remark = item.Remark;
                        //oNewOrder.OrderItems.Add(item);
                        newList.Add(newObj);
                    }
                    oNewOrder.OrderItems = newList;
                    setAmount();
                    EditShippingListShow();//更新承运商列表信息
                    bOrderItemInfoEdit = false;
                }
                else if (bOrderBaseInfoEdit == false && bOrderItemInfoEdit == false && bOrderFeeInfoEdit == true)
                {
                    //保存费用信息进入全局变量
                    oNewOrder.ShippingFee = oBackOrder.ShippingFee;
                    oNewOrder.TaxFee = oBackOrder.TaxFee;
                    oNewOrder.Insurance = oBackOrder.Insurance;
                    oNewOrder.PaymentFee = oBackOrder.PaymentFee;
                    oNewOrder.PackingFee = oBackOrder.PackingFee;
                    oNewOrder.ResidenceFee = oBackOrder.ResidenceFee;
                    oNewOrder.LiftGateFee = oBackOrder.LiftGateFee;
                    oNewOrder.InstallFee = oBackOrder.InstallFee;
                    oNewOrder.OtherFee = oBackOrder.OtherFee;
                    bOrderFeeInfoEdit = false;
                }
                
                if(bSaveEditOrderInfo == true)
                {
                    Guid currentEditOrderGid = oNewOrder.Gid;
                    OrderInformation oCurrentEditOrder = dbEntity.OrderInformations.Where(p => p.Gid == currentEditOrderGid && p.Deleted == false).FirstOrDefault();
                    //订单基本信息保存
                    oCurrentEditOrder.DocVersion = oCurrentEditOrder.DocVersion + 1;
                    oCurrentEditOrder.LinkCode = oNewOrder.LinkCode;
                    oCurrentEditOrder.PayID = oNewOrder.PayID;
                    oCurrentEditOrder.WhID = oNewOrder.WhID;
                    oCurrentEditOrder.aCurrency = oNewOrder.aCurrency;
                    oCurrentEditOrder.TransType = oNewOrder.TransType;
                    oCurrentEditOrder.LeaveWord = oNewOrder.LeaveWord;
                    oCurrentEditOrder.PostComment = oNewOrder.PostComment;
                    oCurrentEditOrder.PayNote = oNewOrder.PayNote;

                    oCurrentEditOrder.Consignee = oNewOrder.Consignee;
                    oCurrentEditOrder.aLocation = oNewOrder.aLocation;
                    oCurrentEditOrder.FullAddress = oNewOrder.FullAddress;
                    oCurrentEditOrder.PostCode = oNewOrder.PostCode;
                    oCurrentEditOrder.Telephone = oNewOrder.Telephone;
                    oCurrentEditOrder.Mobile = oNewOrder.Mobile;
                    oCurrentEditOrder.Email = oNewOrder.Email;
                    oCurrentEditOrder.BestDelivery = oNewOrder.BestDelivery;
                    oCurrentEditOrder.BuildingSign = oNewOrder.BuildingSign;

                    //保存商品信息
                    //保存订单的产品
                    decimal Pieces = 0;//商品件数
                    List<OrderItem> hasBeforeOrderItems = dbEntity.OrderInformations.Find(oNewOrder.Gid).OrderItems.ToList();
                    foreach (OrderItem oldOrderItem in hasBeforeOrderItems)
                    {
                        oldOrderItem.Deleted = true;
                    }
                    foreach (OrderItem newOrderItem in listNewOrderItem)
                    {
                        Pieces += Math.Round(newOrderItem.Quantity,newOrderItem.SkuItem.Percision);//简单相加商品件数
                        newOrderItem.OrderID = oNewOrder.Gid;
                        OrderItem obj = hasBeforeOrderItems.Where(o => o.OrderID == newOrderItem.OrderID && o.OnSkuID == newOrderItem.OnSkuID).FirstOrDefault();
                        if (obj != null)//本身有该索引对应的记录
                        {
                            obj.Quantity = newOrderItem.Quantity;
                            obj.ExecutePrice = newOrderItem.ExecutePrice;
                            obj.TobeShip = newOrderItem.TobeShip;
                            obj.Deleted = false;
                        }
                        else
                        {
                            OrderItem newObj = new OrderItem();
                            newObj.OrderID = newOrderItem.OrderID;
                            newObj.OnSkuID = newOrderItem.OnSkuID;
                            newObj.SkuID = newOrderItem.SkuID;
                            newObj.Name = newOrderItem.Name;
                            newObj.Quantity = newOrderItem.Quantity;
                            newObj.TobeShip = newOrderItem.TobeShip;
                            newObj.Shipped = newOrderItem.Shipped;
                            newObj.BeReturn = newOrderItem.BeReturn;
                            newObj.Returned = newOrderItem.Returned;
                            newObj.MarketPrice = newOrderItem.MarketPrice;
                            newObj.SalePrice = newOrderItem.SalePrice;
                            newObj.ExecutePrice = newOrderItem.ExecutePrice;
                            newObj.SkuPoint = newOrderItem.SkuPoint;
                            newObj.Remark = newOrderItem.Remark;
                            dbEntity.OrderItems.Add(newObj);
                        }
                        oCurrentEditOrder.Pieces = Pieces;//商品件数赋值
                    }
                    //dbEntity.SaveChanges();
                    //保存承运商信息
                    List<OrderShipping> hasBeforeOrderShippings = dbEntity.OrderShippings.Where(o => o.OrderID == oNewOrder.Gid).ToList();//获取原数据库的所有该订单下对应的OrderShipping
                    foreach (OrderShipping oldOrderShip in hasBeforeOrderShippings)
                    {
                        oldOrderShip.Deleted = true;//全部变成删除
                        oldOrderShip.Candidate = false;
                    }
                    foreach (OrderShipping newOrderShip in listNewOrderShipping)
                    {
                        newOrderShip.OrderID = oNewOrder.Gid;
                        OrderShipping obj = hasBeforeOrderShippings.Where(o => o.OrderID == newOrderShip.OrderID && o.ShipID == newOrderShip.ShipID).FirstOrDefault();
                        if (obj==null)
                        {
                            if (newOrderShip.ShipID == oNewShipper.Gid)
                                newOrderShip.Candidate = true;
                            OrderShipping newObj = new OrderShipping();
                            newObj.OrderID = newOrderShip.OrderID;
                            newObj.ShipID = newOrderShip.ShipID;
                            newObj.Ostatus = newOrderShip.Ostatus;
                            newObj.ShipWeight = newOrderShip.ShipWeight;
                            newObj.Candidate = newOrderShip.Candidate;
                            newObj.Remark = newOrderShip.Remark;
                            dbEntity.OrderShippings.Add(newObj);
                        }
                        else
                        {//若OrderShipping已经有该索引的记录
                            if (obj.ShipID == oNewShipper.Gid)
                                obj.Candidate = true;
                            obj.Deleted = false;//变成可用状态
                        }
                    }
                    //dbEntity.SaveChanges();

                    //保存价格信息
                    oCurrentEditOrder.SaleAmount = oNewOrder.SaleAmount;
                    oCurrentEditOrder.ExecuteAmount = oNewOrder.ExecuteAmount;
                    oCurrentEditOrder.ShippingFee = oNewOrder.ShippingFee;
                    oCurrentEditOrder.TaxFee = oNewOrder.TaxFee;
                    oCurrentEditOrder.Insurance = oNewOrder.Insurance;
                    oCurrentEditOrder.PaymentFee = oNewOrder.PaymentFee;
                    oCurrentEditOrder.PackingFee = oNewOrder.PackingFee;
                    oCurrentEditOrder.ResidenceFee = oNewOrder.ResidenceFee;
                    oCurrentEditOrder.LiftGateFee = oNewOrder.LiftGateFee;
                    oCurrentEditOrder.InstallFee = oNewOrder.InstallFee;
                    oCurrentEditOrder.OtherFee = oNewOrder.OtherFee;
                    


                    //保存全局的信息
                    dbEntity.SaveChanges();

                    bOrderBaseInfoEdit = false;
                    bOrderItemInfoEdit = false;
                    bOrderFeeInfoEdit = false;
                    bChangeOrder = false;
                    bSaveEditOrderInfo = false;
                }

            }

            return null;
        }
        /// <summary>
        /// 编辑订单时修改用户的地址
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListEditOrderUser(SearchModel searchModel) 
        {
            Guid userOrgId = oNewOrder.OrgID;
            Guid userGid = oNewOrder.UserID;
            if (userGid.Equals(null) || userGid.Equals(Guid.Empty))
            {
                return null;
            }
            IQueryable<MemberAddress> oMemberAddress = dbEntity.MemberAddresses.Include("User").Where(p => p.Deleted == false && p.UserID == userGid).AsQueryable();

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

        /// <summary>
        /// 将订单保存入历史表，修改订单版本号
        /// </summary>
        /// <param name="oOldOrder"></param>
        /// <param name="nHangType"></param>
        /// <param name="strReason"></param>
        /// <returns></returns>
        public ActionResult SaveOldOrderVersion(byte nHangType, string strReason)
        {
            //将订单保存入历史表，改变订单的版本号
            OrderHistory oOrderHistory = new OrderHistory();
            OrderInformation oOldOrder = oNewOrder;
            oOrderHistory.OrderID = oOldOrder.Gid;
            oOrderHistory.DocVersion = oOldOrder.DocVersion;
            oOrderHistory.Htype = nHangType;
            oOrderHistory.Reason = strReason;
            //关联退款单，需要根据更改原因来设置，可以使用Guid？类型的参数传入函数
            oOrderHistory.RefRefund = null;
            //关联入库单，需要根据更改原因来设置，可以使用Guid？类型的参数传入函数
            oOrderHistory.RefStockIn = null;
            //仓库字段
            oOrderHistory.WhID = oOldOrder.WhID;

            oOrderHistory.LinkCode = oOldOrder.LinkCode;
            oOrderHistory.Ostatus = oOldOrder.Ostatus;
            oOrderHistory.Locking = oOldOrder.Locking;
            oOrderHistory.PayStatus = oOldOrder.PayStatus;
            oOrderHistory.Hanged = oOldOrder.Hanged;
            oOrderHistory.HangReason = oOldOrder.HangReason;
            oOrderHistory.ReleaseTime = oOldOrder.ReleaseTime;
            oOrderHistory.TransType = oOldOrder.TransType;
            if (!oOldOrder.PayID.Equals(null))
            {
                oOrderHistory.PayID = (Guid)oOldOrder.PayID;
            }
            oOrderHistory.PayNote = oOldOrder.PayNote;
            oOrderHistory.Pieces = oOldOrder.Pieces;
            if (!oOldOrder.aCurrency.Equals(null))
            {
                oOrderHistory.Currency = (Guid)oOldOrder.aCurrency;
            }
            oOrderHistory.SaleAmount = oOldOrder.SaleAmount;
            oOrderHistory.ExecuteAmount = oOldOrder.ExecuteAmount;
            oOrderHistory.ShippingFee = oOldOrder.ShippingFee;
            oOrderHistory.TaxFee = oOldOrder.TaxFee;
            oOrderHistory.Insurance = oOldOrder.Insurance;
            oOrderHistory.PaymentFee = oOldOrder.PaymentFee;
            oOrderHistory.PackingFee = oOldOrder.PackingFee;
            oOrderHistory.ResidenceFee = oOldOrder.ResidenceFee;
            oOrderHistory.LiftGateFee = oOldOrder.LiftGateFee;
            oOrderHistory.InstallFee = oOldOrder.InstallFee;
            oOrderHistory.OtherFee = oOldOrder.OtherFee;
            oOrderHistory.TotalFee = oOldOrder.TotalFee;
            oOrderHistory.UsePoint = oOldOrder.UsePoint;
            oOrderHistory.PointPay = oOldOrder.PointPay;
            oOrderHistory.CouponPay = oOldOrder.CouponPay;
            oOrderHistory.BounsPay = oOldOrder.BounsPay;
            oOrderHistory.MoneyPaid = oOldOrder.MoneyPaid;
            oOrderHistory.TotalPaid = oOldOrder.TotalPaid;
            oOrderHistory.OrderAmount = oOldOrder.OrderAmount;
            oOrderHistory.Differ = oOldOrder.Differ;
            oOrderHistory.MergeFrom = oOldOrder.MergeFrom;
            oOrderHistory.SplitFrom = oOldOrder.SplitFrom;
            oOrderHistory.GetPoint = oOldOrder.GetPoint;
            oOrderHistory.ConfirmTime = oOldOrder.ConfirmTime;
            oOrderHistory.PaidTime = oOldOrder.PaidTime;
            oOrderHistory.ArrangeTime = oOldOrder.ArrangeTime;
            oOrderHistory.NoticeTime = oOldOrder.NoticeTime;
            oOrderHistory.ClosedTime = oOldOrder.ClosedTime;
            oOrderHistory.Consignee = oOldOrder.Consignee;
            if (!oOldOrder.aLocation.Equals(null))
            {
                oOrderHistory.Location = (Guid)oOldOrder.aLocation;
            }
            oOrderHistory.FullAddress = oOldOrder.FullAddress;
            oOrderHistory.PostCode = oOldOrder.PostCode;
            oOrderHistory.Telephone = oOldOrder.Telephone;
            oOrderHistory.Mobile = oOldOrder.Mobile;
            oOrderHistory.Email = oOldOrder.Email;
            oOrderHistory.ErrorAddress = oOldOrder.ErrorAddress;
            oOrderHistory.BestDelivery = oOldOrder.BestDelivery;
            oOrderHistory.BuildingSign = oOldOrder.BuildingSign;
            oOrderHistory.PostComment = oOldOrder.PostComment;
            oOrderHistory.LeaveWord = oOldOrder.LeaveWord;
            oOrderHistory.IpAddress = oOldOrder.IpAddress;
            if (!oOldOrder.AdvID.Equals(null))
            {
                oOrderHistory.AdvID = (Guid)oOldOrder.AdvID;
            }
            oOrderHistory.Remark = oOldOrder.Remark;

            dbEntity.OrderHistories.Add(oOrderHistory);
            dbEntity.SaveChanges();
            //当前变更订单的guid
            Guid currentOrderGid = oOldOrder.Gid;
            //将订单对应的商品信息保存进历史表
            List<OrderItem> listOrderItemInCurrentOrder = dbEntity.OrderItems.Include("Order").Include("OnSkuItem").Include("SkuItem").Where(p => p.Gid == currentOrderGid && p.Deleted == false).ToList();
            for (int i = 0; i < listOrderItemInCurrentOrder.Count; i++)
            {
                OrderHisItem oNewHisOrderItem = new OrderHisItem();
                oNewHisOrderItem.OrderHisID = oOrderHistory.Gid;
                oNewHisOrderItem.OnSkuID = listOrderItemInCurrentOrder.ElementAt(i).OnSkuID;
                oNewHisOrderItem.SkuID = listOrderItemInCurrentOrder.ElementAt(i).SkuID;
                oNewHisOrderItem.Name = listOrderItemInCurrentOrder.ElementAt(i).Name;
                oNewHisOrderItem.Quantity = listOrderItemInCurrentOrder.ElementAt(i).Quantity;
                oNewHisOrderItem.MarketPrice = listOrderItemInCurrentOrder.ElementAt(i).MarketPrice;
                oNewHisOrderItem.SalePrice = listOrderItemInCurrentOrder.ElementAt(i).SalePrice;
                oNewHisOrderItem.ExecutePrice = listOrderItemInCurrentOrder.ElementAt(i).ExecutePrice;
                oNewHisOrderItem.SkuPoint = listOrderItemInCurrentOrder.ElementAt(i).SkuPoint;
                oNewHisOrderItem.Remark = listOrderItemInCurrentOrder.ElementAt(i).Remark;

                dbEntity.OrderHisItems.Add(oNewHisOrderItem);
                dbEntity.SaveChanges();
            }

            bChangeOrder = true;
            return null;
        }

        /// <summary>
        /// 撤销对订单编辑的所有变更
        /// </summary>
        /// <returns></returns>
        public ActionResult DiscardAllChange()
        {
            //全局model 还原
            SetViewOrderInfo(oNewOrder.Gid);
            bOrderBaseInfoEdit = false;
            bOrderItemInfoEdit = false;
            bOrderFeeInfoEdit = false;
            bChangeOrder = false;
            bSaveEditOrderInfo = false;
            return RedirectToAction("OrderEditInfo", "Order");
        }

        #endregion

        #region 商品编辑
        /// <summary>
        /// 订单编辑页面的产品列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult OrderItemEditList(SearchModel searchModel)
        {
            if (isBackList == true)
            {//如果是还原状态 则全局变量重新加载次
                isBackList = false;
                OrderInformation oOldOrder = dbEntity.OrderInformations.Include("OrderItems").Where(o => o.Gid == oNewOrder.Gid).FirstOrDefault();
                foreach (var item in oOldOrder.OrderItems)
                {
                    listNewOrderItem.Add(item);
                }
            }
            IQueryable<OrderItem> listOrderItem = listNewOrderItem.AsQueryable();
            GridColumnModelList<OrderItem> columns = new GridColumnModelList<OrderItem>();
            columns.Add(p => p.OnSkuID).SetAsPrimaryKey().SetHidden(true);
            columns.Add(p => p.Name).SetCaption(@LiveAzure.Resource.Model.Order.OrderItem.Name);
            columns.Add(p => p.SkuItem.Code).SetCaption(@LiveAzure.Resource.Model.Product.ProductOnItem.SkuItem);
            columns.Add(p => p.SkuItem.Barcode).SetCaption(@LiveAzure.Resource.Model.Product.ProductInfoItem.Barcode);
            columns.Add(p => p.ExecutePrice).SetCaption(@LiveAzure.Resource.Model.Order.OrderItem.ExecutePrice);
            columns.Add(p => p.SalePrice).SetCaption(@LiveAzure.Resource.Model.Order.OrderItem.SalePrice);
            columns.Add(p => p.Quantity).SetCaption(@LiveAzure.Resource.Model.Order.OrderItem.Quantity);
            columns.Add(p => getStdUnit(p.SkuItem.StdUnit)).SetName("StdUnit");
            columns.Add(p => Math.Round(p.Shipped,p.SkuItem.Percision) + "(" + Math.Round(p.TobeShip,p.SkuItem.Percision) + ")").SetName("ShipMessage");
            columns.Add(p => Math.Round(p.Returned,p.SkuItem.Percision) + "(" + Math.Round(p.BeReturn,p.SkuItem.Percision) + ")").SetName("ReturnMessage");
            
            columns.Add(p => p.ExecutePrice * p.Quantity).SetName("Amount").SetCaption("小计");
            GridData gridData = listOrderItem.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 撤销对全局变量listNewOrderItem的更改
        /// </summary>
        public ActionResult BacklistNewOrderItem()
        {
            listNewOrderItem.Clear();
            isBackList = true;
            return RedirectToAction("OrderEditInfo", "Order");
        }

        /// <summary>
        /// 编辑订单时列出支持的承运商
        /// </summary>
        /// <returns></returns>
        public void EditShippingListShow()
        {
            OrderBLL oOrderBLL = new OrderBLL(this.dbEntity);//新建oOrderBLL对象
            oNewShipper = dbEntity.ShippingInformations.Find(oNewShipper.Gid);
            List<ShippingInformation> oShippingList = new List<ShippingInformation>();
            List<SelectListItem> shippinglist = new List<SelectListItem>();
            if (oNewOrder.WhID != null && oNewOrder.aLocation != null && listNewOrderItem.Count >= 1)
            {
                //-----------edit by 2011/10/24 需求:手动编辑订单是可以选择所有组织支持的承运商
                //获取支持的承运商列表
                //oShippingList = oOrderBLL.GetSupportShippings(listNewOrderItem, (Guid)oNewOrder.WhID, (Guid)oNewOrder.aLocation);
                //获取组织支持的所有承运商
                oShippingList = dbEntity.ShippingInformations.Where(s => s.aParent == oNewOrder.OrgID && s.Deleted == false).ToList();
                //------------------------------------------------------------------------------

                listNewOrderShipping.Clear();//每次都清除之前的承运商列表，以保证该列表永远是最新的-----BUG修改09.25
                if (oShippingList.Count >= 1)//如果查出承运商
                {
                    for (int i = 0; i < oShippingList.Count; i++)
                    {
                        OrderShipping oNewOrderShipping = new OrderShipping
                        {
                            ShipID = oShippingList.ElementAt(i).Gid,
                            Order = oNewOrder
                        };
                        listNewOrderShipping.Add(oNewOrderShipping);
                    }
                    if (oNewShipper == null)//如果之前没有承运商的
                    {
                        oNewShipper = oShippingList.ElementAt(0);
                    }
                    foreach (var reg in oOrderBLL.GetFullRegions((Guid)oNewOrder.aLocation))
                    {
                        if (oNewShipper.Areas != null)//地区不为空
                        {
                            foreach (var area in oNewShipper.Areas)
                            {
                                if (area.RegionID == reg.Gid)
                                {
                                    oNewOrder.ResidenceFee = area.Residential == null ? 0 : area.Residential.GetResource((Guid)oNewOrder.aCurrency);//收取的到门费
                                    oNewOrder.LiftGateFee = area.LiftGate == null ? 0 : area.LiftGate.GetResource((Guid)oNewOrder.aCurrency);//收取的上楼费
                                    oNewOrder.InstallFee = area.Installation == null ? 0 : area.Installation.GetResource((Guid)oNewOrder.aCurrency);//收取的安装费
                                }
                            }
                        }
                    }
                }
                else//未找到符合的承运商
                {
                    oNewShipper = new ShippingInformation();
                }
            }
            else//错误：仓库ID 地区ID 不能为空 且 订单商品列表不为空
            { 
            
            }
        }
        #endregion

        #endregion

        #region 订单合并
        public ActionResult Merge()
        {
            return View();
        }
        #endregion 订单合并


        #region 订单拆分
        public ActionResult Split()
        {
            return View();
        }
        #endregion 订单拆分


        #region 订单策略
        public ActionResult Policy()
        {
            return View();
        }
        #endregion 订单策略
    }

}
