using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC.Controls;
using MVC.Controls.Grid;
using LiveAzure.Models.Finance;
using LiveAzure.Models;
using LiveAzure.Utility;
using System.Data;
using LiveAzure.BLL;
using LiveAzure.Models.Member;
using System.Text;
using LiveAzure.Models.Order;
using LiveAzure.Models.General;
using LiveAzure.Models.Purchase;
using System.Globalization;

namespace LiveAzure.Stage.Controllers
{
    public class FinanceController : BaseController
    {
        public static string orderCode;         //订单号
        public static Guid? organizationId;         //组织ID
        public static Guid paymentGid;              //应付款Gid
        public static byte? refType;         //关联单据类型
        public static string refBillCode;       //关联单据号

        #region PayType
        /// <summary>
        /// PayType列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // 权限验证
            string strProgramCode = Request.RequestContext.RouteData.Values["Controller"].ToString() +
                Request.RequestContext.RouteData.Values["Action"].ToString();
            if (!base.Permission(strProgramCode))
                return RedirectToAction("ErrorPage", "Home", new { LiveAzure.Resource.Common.NoPermission });
            return View();
        }

        /// <summary>
        /// 添加支付方式页面
        /// </summary>
        /// <returns></returns>
        public ActionResult AddPayType()
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                FinancePayType oPayType = new FinancePayType
                {
                    Name = NewResource(ModelEnum.ResourceType.STRING)
                };
                ViewBag.organization = GetOrganizationList();
                ViewBag.isCod = SelectEnumList(oPayType.IsCod);
                ViewBag.isOnline = SelectEnumList(oPayType.IsOnline);
                ViewBag.payStatus = GetPayState(oPayType);

                return PartialView(oPayType);
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }

        /// <summary>
        /// 编辑支付方式页面
        /// </summary>
        /// <param name="fGid"></param>
        /// <returns></returns>
        public ActionResult EditPayType(Guid fGid)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                FinancePayType oPayType = dbEntity.FinancePayTypes.Where(f => f.Gid == fGid).Single();
                oPayType.Name = RefreshResource(ModelEnum.ResourceType.STRING, oPayType.Name);
                ViewBag.editOrganization = GetOrganizationList();
                ViewBag.editIsCod = SelectEnumList(oPayType.IsCod);
                ViewBag.editIsOnline = SelectEnumList(oPayType.IsOnline);
                ViewBag.editPayStatus = GetPayState(oPayType);
                return PartialView(oPayType);
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }

        /// <summary>
        /// 保存添加的支付方式
        /// </summary>
        /// <param name="newPayType"></param>
        public void SaveNewPayType(FinancePayType newPayType)
        {
            try
            {
                dbEntity.FinancePayTypes.Add(newPayType);
                dbEntity.SaveChanges();
            }
            catch (Exception ex)
            {
                RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
            }
        }

        /// <summary>
        /// 保存编辑的支付方式
        /// </summary>
        /// <param name="newPayType"></param>
        public void SavePayType(FinancePayType newPayType)
        {
            try
            {
                FinancePayType oldPayType = dbEntity.FinancePayTypes.Where(f => f.Gid == newPayType.Gid).Single();

                oldPayType.OrgID = newPayType.OrgID;
                oldPayType.Code = newPayType.Code;
                oldPayType.Name.SetResource(ModelEnum.ResourceType.STRING, newPayType.Name);
                oldPayType.Matter = newPayType.Matter;
                oldPayType.Pstatus = newPayType.Pstatus;
                oldPayType.Sorting = newPayType.Sorting;
                oldPayType.IsCod = newPayType.IsCod;
                oldPayType.IsOnline = newPayType.IsOnline;
                oldPayType.Fee = newPayType.Fee;
                oldPayType.Config = newPayType.Config;
                oldPayType.Remark = newPayType.Remark;

                if (ModelState.IsValid)
                {
                    dbEntity.Entry(oldPayType).State = EntityState.Modified;
                    dbEntity.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
            }
        }

        /// <summary>
        /// 移除支付方式
        /// </summary>
        /// <param name="fGid"></param>
        public void RemovePayType(Guid fGid)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                try
                {
                    FinancePayType oPayType = dbEntity.FinancePayTypes.Where(f => f.Gid == fGid).Single();
                    oPayType.Deleted = true;
                    if (ModelState.IsValid)
                    {
                        dbEntity.Entry(oPayType).State = EntityState.Modified;
                        dbEntity.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
                }
            }
            RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }

        /// <summary>
        /// 支付方式GridList
        /// </summary>
        /// <param name="payTypeSearchModel"></param>
        /// <returns></returns>
        public ActionResult PayTypeList(SearchModel payTypeSearchModel)
        {
            IQueryable<FinancePayType> oPayType = dbEntity.FinancePayTypes.Include("Organization").Include("Name").Where(f => f.Deleted == false).AsQueryable();
            GridColumnModelList<FinancePayType> columns = new GridColumnModelList<FinancePayType>();
            columns.Add(f => f.Gid).SetAsPrimaryKey();
            columns.Add(f => f.Organization.FullName.GetResource(CurrentSession.Culture)).SetName("Organization");
            columns.Add(f => f.Code);
            columns.Add(f => f.Name.GetResource(CurrentSession.Culture)).SetName("Name");
            columns.Add(f => f.Matter);
            columns.Add(f => f.Pstatus);
            columns.Add(f => f.Sorting);
            columns.Add(f => f.IsCod);
            columns.Add(f => f.IsOnline);
            columns.Add(f => f.Fee);
            columns.Add(f => f.Config);
            columns.Add(f => f.Remark);

            GridData oPayTypeGridData = oPayType.ToGridData(payTypeSearchModel, columns);
            return Json(oPayTypeGridData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取组织列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetOrganizationList()
        {
            List<SelectListItem> oList = new List<SelectListItem>();
            var oOrganization = dbEntity.MemberOrganizations.Where(m => m.Deleted == false).ToList();
            foreach (MemberOrganization item in oOrganization)
            {
                oList.Add(new SelectListItem { Value = item.Gid.ToString(), Text = item.FullName.GetResource(CurrentSession.Culture) });
            }
            return oList;
        }

        /// <summary>
        /// 获取支付状态列表
        /// </summary>
        /// <param name="oPayType"></param>
        /// <returns></returns>
        private List<SelectListItem> GetPayState(FinancePayType oPayType)
        {
            var oPayState = base.SelectEnumList(typeof(ModelEnum.PointStatus), oPayType.Pstatus);
            return oPayState;
        }

        /// <summary>
        /// 获取支付状态的多语言文本
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPstatus()
        {
            StringBuilder s = new StringBuilder();
            List<SelectListItem> list = base.SelectEnumList(typeof(ModelEnum.PointStatus), new FinancePayType().Pstatus);
            foreach(SelectListItem item in list)
            {
                s.Append(item.Value + ":" + item.Text + ";");
            }
            return Json(s.ToString(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取是否支持货到付款的语言文本
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPayTypeIsCod()
        {
            StringBuilder s = new StringBuilder();
            List<SelectListItem> list = SelectEnumList(new FinancePayType().IsCod);
            foreach (SelectListItem item in list)
            {
                s.Append(item.Value + ":" + item.Text + ";");
            }
            return Json(s.ToString(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取是否支持在线支付的多语言文本
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPayTypeIsOnline()
        {
            StringBuilder s = new StringBuilder();
            List<SelectListItem> list = SelectEnumList(new FinancePayType().IsOnline);
            foreach (SelectListItem item in list)
            {
                s.Append(item.Value + ":" + item.Text + ";");
            }
            return Json(s.ToString(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Invoice

        /// <summary>
        /// 发票列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult Invoice()
        {
            //Guid orderId = Guid.Empty;
            //FinanceInvoice oInvoice = dbEntity.FinanceInvoices.Where(i => i.OrderID == orderId).SingleOrDefault();
            //if (oInvoice != null)
            //    return View(oInvoice);
            //else
                return View();
        }

        /// <summary>
        /// 发票列表
        /// </summary>
        /// <param name="orderCode_Page"></param>
        /// <returns></returns>
        public ActionResult InvoiceListTable(string orderCode_Page)
        {
            orderCode = orderCode_Page;
            return PartialView();
        }

        /// <summary>
        /// GridList
        /// </summary>
        /// <param name="invoiceSearchModel"></param>
        /// <returns></returns>
        public ActionResult InvoiceList(SearchModel invoiceSearchModel)
        {
            IQueryable<FinanceInvoice> oInvoiceList;

            if (orderCode == null)
            {
                oInvoiceList = dbEntity.FinanceInvoices.Where(i => i.Deleted == false).AsQueryable();
            }
            else
            {
                oInvoiceList = (from i in dbEntity.FinanceInvoices
                                where i.OrderInfo.Code.Contains(orderCode) && i.Deleted == false
                                orderby i.OrderInfo.Code descending
                                select i).AsQueryable();
            }

            GridColumnModelList<FinanceInvoice> columns = new GridColumnModelList<FinanceInvoice>();
            columns.Add(m => m.Gid).SetAsPrimaryKey();
            columns.Add(m => (m.OrderInfo == null) ? "" : m.OrderInfo.Code).SetName("OrderInfo");
            columns.Add(m => m.Code);
            columns.Add(m => m.Title);
            columns.Add(m => m.Matter);
            columns.Add(m => m.Istatus);
            columns.Add(m => m.Amount);
            columns.Add(m => m.SendNote);

            GridData invoiceGridData = oInvoiceList.ToGridData(invoiceSearchModel, columns);
            return Json(invoiceGridData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加发票的页面
        /// </summary>
        /// <returns></returns>
        public ActionResult AddInvoice()
        {
            ViewBag.IstatusList = GetIstatus();
            return PartialView();
        }

        /// <summary>
        /// 编辑发票的页面
        /// </summary>
        /// <param name="invoiceGid"></param>
        /// <returns></returns>
        public ActionResult EditInvoice(Guid invoiceGid)
        {
            FinanceInvoice oInvoice = dbEntity.FinanceInvoices.Include("OrderInfo").Where(i => i.Gid == invoiceGid).Single();
            ViewBag.EditIstatusList = GetIstatus();
            return PartialView(oInvoice);
        }

        /// <summary>
        /// 保存添加的发票
        /// </summary>
        /// <param name="NewInvoice"></param>
        public void SaveNewInvoice(FinanceInvoice NewInvoice)
        {
            try
            {
                var orderId = NewInvoice.OrderInfo.Code;
                OrderInformation oOrder = dbEntity.OrderInformations.Where(o => o.Code == orderId).Single();
                NewInvoice.OrderInfo = oOrder;
                dbEntity.FinanceInvoices.Add(NewInvoice);
                dbEntity.SaveChanges();
            }
            catch (Exception ex)
            {
                RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
            }
        }

        /// <summary>
        /// 保存编辑的发票
        /// </summary>
        /// <param name="NewInvoice"></param>
        public void SaveEditInvoice(FinanceInvoice NewInvoice)
        {
            try
            {
                FinanceInvoice oldInvoice = dbEntity.FinanceInvoices.Include("OrderInfo").Where(i => i.Gid == NewInvoice.Gid).Single();
                oldInvoice.OrderInfo.Code = NewInvoice.OrderInfo.Code;
                oldInvoice.Code = NewInvoice.Code;
                oldInvoice.Istatus = NewInvoice.Istatus;
                oldInvoice.Amount = NewInvoice.Amount;
                oldInvoice.SendNote = NewInvoice.SendNote;
                oldInvoice.Remark = NewInvoice.Remark;
                if (ModelState.IsValid)
                {
                    dbEntity.Entry(oldInvoice).State = EntityState.Modified;
                    dbEntity.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
            }
        }

        /// <summary>
        /// 删除发票
        /// </summary>
        /// <param name="invoiceGid"></param>
        public void RemoveInvoice(Guid invoiceGid)
        {
            try
            {
                FinanceInvoice oInvoice = dbEntity.FinanceInvoices.Where(i => i.Gid == invoiceGid).Single();
                oInvoice.Deleted = true;
                if (ModelState.IsValid)
                {
                    dbEntity.Entry(oInvoice).State = EntityState.Modified;
                    dbEntity.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
            }
        }

        /// <summary>
        /// 获取发票状态下拉框列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetIstatus()
        {
            var oIstatusList = base.SelectEnumList(typeof(ModelEnum.InventoryStatus), new FinanceInvoice().Istatus);
            return oIstatusList;
        }

        /// <summary>
        /// 获取发票状态的多语言文本
        /// </summary>
        /// <returns></returns>
        public ActionResult GetIstatusText()
        {
            StringBuilder s = new StringBuilder();
            List<SelectListItem> list = base.SelectEnumList(typeof(ModelEnum.InventoryStatus), new FinanceInvoice().Istatus);
            foreach (SelectListItem item in list)
            {
                s.Append(item.Value + ":" + item.Text + ";");
            }
            return Json(s.ToString(), JsonRequestBehavior.AllowGet);
        }
        #endregion



        #region Payable

        /// <summary>
        /// 应付款列表页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Payable()
        {
            organizationId = null;
            refType = null;
            refBillCode = null;
            ViewBag.organizationSelectList = GetOrganizationList();
            return View();
        }

        /// <summary>
        /// 应付款列表
        /// </summary>
        /// <param name="orgID"></param>
        /// <returns></returns>
        public void PayableListTable(Guid? orgID)
        {
            organizationId = orgID;
        }

        /// <summary>
        /// 添加应付款的页面
        /// </summary>
        /// <returns></returns>
        public ActionResult AddPayable()
        {
            ViewBag.organizationList = GetOrganizationList();
            ViewBag.payTypeList = GetPtypeSelectList();
            ViewBag.payToList = GetPayToSelectList();
            ViewBag.payStatusList = GetPstatusSelectList();
            ViewBag.refTypeList = GetRefTypeSelectList();
            return PartialView();
        }

        /// <summary>
        /// 编辑应付款的页面
        /// </summary>
        /// <param name="PayableGid"></param>
        /// <returns></returns>
        public ActionResult EditPayable(Guid PayableGid)
        {
            FinancePayment oPayment = dbEntity.FinancePayments.Include("Organization").Include("PaymentType").Include("Currency").Where(p => p.Gid == PayableGid).Single();
            OrderInformation oOrder=dbEntity.OrderInformations.Where(o=>o.Gid==oPayment.RefID).Single();
            ViewBag.editOrganizationList = GetOrganizationList();
            ViewBag.editPayTypeList = GetPtypeSelectList();
            ViewBag.editPayToList = GetPayToSelectList();
            ViewBag.editPayStatusList = GetPstatusSelectList();
            ViewBag.editRefTypeList = GetRefTypeSelectList();
            ViewBag.currencyForEdit = oPayment.Currency.Name.GetResource(CurrentSession.Culture);
            ViewBag.defaultvalue = oOrder.Code;
            return PartialView(oPayment);
        }

        /// <summary>
        /// 保存添加的应付款
        /// </summary>
        /// <param name="newPayment"></param>
        /// <param name="RefID">页面传回的订单号</param>
        public void SaveNewPayable(FinancePayment newPayment,string RefID)
        {
            try
            {
                byte ptype = newPayment.RefType;    //单据类型，订单或采购单
                switch (ptype)
                {
                    case (byte)ModelEnum.NoteType.ORDER:
                        {
                            OrderInformation oOrderInfo = dbEntity.OrderInformations.Include("Currency").Where(o => o.Code == RefID).Single();
                            newPayment.RefID = oOrderInfo.Gid;
                            newPayment.Currency = oOrderInfo.Currency;
                            break;
                        }
                    case (byte)ModelEnum.NoteType.PURCHASE:
                        {
                            PurchaseInformation oPurchase = dbEntity.PurchaseInformations.Include("Currency").Where(p => p.Code == RefID).Single();
                            newPayment.RefID = oPurchase.Gid;
                            newPayment.Currency = oPurchase.Currency;
                            break;
                        }
                }
                newPayment.Pstatus = (byte)ModelEnum.FinanceStatus.NONE;
                dbEntity.FinancePayments.Add(newPayment);
                dbEntity.SaveChanges();
            }
            catch (Exception ex)
            {
                RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
            }
        }

        /// <summary>
        /// 保存编辑的应付款
        /// </summary>
        /// <param name="newPayable"></param>
        /// <param name="RefID">页面传回的订单号</param>
        public void SaveEditPayable(FinancePayment newPayable)
        {
            try
            {
                FinancePayment oldPayment = dbEntity.FinancePayments.Include("Organization").Include("PaymentType").Include("Currency").Where(p => p.Gid == newPayable.Gid).Single();
                oldPayment.OrgID = newPayable.OrgID;
                oldPayment.Ptype = newPayable.Ptype;
                oldPayment.PayTo = newPayable.PayTo;
                oldPayment.PayDate = newPayable.PayDate;
                oldPayment.Reason = newPayable.Reason;
                oldPayment.Currency = newPayable.Currency;
                oldPayment.Amount = newPayable.Amount;
                oldPayment.Remark = newPayable.Remark;
                if (ModelState.IsValid)
                {
                    dbEntity.Entry(oldPayment).State = EntityState.Modified;
                    dbEntity.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
            }
        }

        /// <summary>
        /// 删除一个应付款
        /// </summary>
        /// <param name="PayableGid"></param>
        public void RemovePayable(Guid PayableGid)
        {
            try
            {
                FinancePayment oPayment = dbEntity.FinancePayments.Where(p => p.Gid == PayableGid).Single();
                oPayment.Deleted = true;
                if (ModelState.IsValid)
                {
                    dbEntity.Entry(oPayment).State = EntityState.Modified;
                    dbEntity.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
            }
        }

        /// <summary>
        /// 确认应付款
        /// </summary>
        /// <param name="payableGid"></param>
        public void ConfirmPayable(Guid payableGid)
        {
            FinancePayment oPayment = dbEntity.FinancePayments.Where(p => p.Gid == payableGid).Single();
            List<FinancePayment> oPaymentList = dbEntity.FinancePayments.Where(p => p.RefID == oPayment.RefID).ToList();

            oPayment.Pstatus = (byte)ModelEnum.FinanceStatus.CONFIRMED;

            decimal moneyCount = 0;
            foreach (FinancePayment item in oPaymentList)
            {
                moneyCount += item.Amount;
            }

            if (oPayment.RefType == (byte)ModelEnum.NoteType.ORDER)
            {
                OrderInformation oOrder = dbEntity.OrderInformations.Where(o => o.Gid == oPayment.RefID).Single();
                if (moneyCount == oOrder.OrderAmount)
                {
                    oOrder.PayStatus = (byte)ModelEnum.PayStatus.PAID;
                    dbEntity.Entry(oOrder).State = EntityState.Modified;
                    dbEntity.SaveChanges();
                }
            }
            if(oPayment.RefType == (byte)ModelEnum.NoteType.PURCHASE)
            {
                PurchaseInformation oPurchase = dbEntity.PurchaseInformations.Where(o => o.Gid == oPayment.RefID).Single();
                if (moneyCount == oPurchase.Amount)
                {
                    oPurchase.Pstatus = (byte)ModelEnum.PurchaseStatus.CONFIRMED;
                    dbEntity.Entry(oPurchase).State = EntityState.Modified;
                    dbEntity.SaveChanges();
                }
            }
            
            if (ModelState.IsValid)
            {
                dbEntity.Entry(oPayment).State = EntityState.Modified;
                dbEntity.SaveChanges();
            }
        }

        /// <summary>
        /// 侍结算应付款
        /// </summary>
        /// <param name="payableGid"></param>
        /// <returns></returns>
        public ActionResult PayablesToClose(Guid? payableGid)
        {
            ViewBag.refIdList = GetRefTypeSelectList();
            ViewBag.orgList = GetOrganizationList();
            if (payableGid != null)
            {
                paymentGid = (Guid)payableGid;
                FinancePayment oPayment = dbEntity.FinancePayments.Include("Currency").Where(p => p.Gid == payableGid).Single();
                OrderInformation oOrder = dbEntity.OrderInformations.Where(o => o.Gid == oPayment.RefID).SingleOrDefault();
                organizationId = oPayment.OrgID;
                refType = oPayment.RefType;
                refBillCode = oOrder.Code;
                ViewBag.refId = oPayment.RefType;
                ViewBag.refBillCode = oOrder.Code;
                ViewBag.BillPayAmount = oOrder.OrderAmount;
            }
            return PartialView();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgId">组织ID</param>
        /// <param name="reftype">关联单据类型</param>
        /// <param name="billCode">关联单据号</param>
        public void ChangeRefId(Guid? orgId, string reftype, string billCode)
        {
            if (orgId != null)
            {
                organizationId = orgId;
            }
            if (reftype != null)
            {
                if (reftype == "ORDER")
                {
                    refType = (byte)ModelEnum.NoteType.ORDER;
                }
                if (reftype == "PURCHASE")
                {
                    refType = (byte)ModelEnum.NoteType.PURCHASE;
                }
            }
            if (billCode != null)
            {
                if (billCode == "")
                {
                    refBillCode = null;
                }
                else
                {
                    refBillCode = billCode;
                }
            }
        }

        public void ClosePaymentAndSetOrderStatusPaid(Guid paymentGid)
        {
            FinancePayment oPayment = dbEntity.FinancePayments.Where(o => o.Gid == paymentGid).Single();
            List<FinancePayment> oPayments = dbEntity.FinancePayments.Where(o => o.RefID == oPayment.RefID).ToList();
            decimal moneyCount = 0;
            foreach (var item in oPayments)
            {
                moneyCount += item.Amount;
            }

            if (oPayment.RefType == (byte)ModelEnum.NoteType.ORDER)
            { 
                OrderInformation oOrder = dbEntity.OrderInformations.Where(o => o.Gid == oPayment.RefID).Single();
                if (moneyCount == oOrder.OrderAmount)
                {
                    oOrder.Ostatus = (byte)ModelEnum.OrderStatus.CLOSED;
                    dbEntity.Entry(oOrder).State = EntityState.Modified;
                    dbEntity.SaveChanges();

                    foreach (FinancePayment item in oPayments)
                    {
                        item.Pstatus = (byte)ModelEnum.FinanceStatus.CLOSED;
                        if (ModelState.IsValid)
                        {
                            dbEntity.Entry(item).State = EntityState.Modified;
                            dbEntity.SaveChanges();
                        }
                    }
                }
                
            }
            if (oPayment.RefType == (byte)ModelEnum.NoteType.PURCHASE)
            {
                PurchaseInformation oPurchase = dbEntity.PurchaseInformations.Where(o => o.Gid == oPayment.RefID).Single();
                if (moneyCount == oPurchase.Amount)
                {
                    oPurchase.Pstatus = (byte)ModelEnum.PurchaseStatus.CLOSED;
                    dbEntity.Entry(oPurchase).State = EntityState.Modified;
                    dbEntity.SaveChanges();

                    foreach (FinancePayment item in oPayments)
                    {
                        item.Pstatus = (byte)ModelEnum.FinanceStatus.CLOSED;
                        if (ModelState.IsValid)
                        {
                            dbEntity.Entry(item).State = EntityState.Modified;
                            dbEntity.SaveChanges();
                        }
                    }
                }
            }
        }

        public ActionResult PayableToCloseList(SearchModel PayableSearchModel)
        {
            Guid orgGid = CurrentSession.OrganizationGID;
            IQueryable<FinancePayment> oPayable;
            try
            {
                if (refBillCode == null)
                {
                    oPayable = dbEntity.FinancePayments
                        .Where(p => p.Deleted == false 
                            && p.Pstatus == (byte)ModelEnum.FinanceStatus.CONFIRMED
                            && p.OrgID == ((organizationId == null) ? orgGid : organizationId) 
                            && p.RefType == ((refType == null) ? (byte)ModelEnum.NoteType.ORDER : refType))
                            .AsQueryable();
                }
                else
                {
                    if (refType == null || (byte)refType == (byte)ModelEnum.NoteType.ORDER)
                    {
                        OrderInformation oOrder = dbEntity.OrderInformations.Where(o => o.Code == refBillCode).Single();
                        oPayable = dbEntity.FinancePayments
                            .Where(p => p.Deleted == false 
                                && p.OrgID == ((organizationId == null) ? orgGid : organizationId)
                                && p.Pstatus == (byte)ModelEnum.FinanceStatus.CONFIRMED
                                && p.RefType == ((refType == null) ? (byte)ModelEnum.NoteType.ORDER : refType) 
                                && p.RefID == oOrder.Gid)
                                .AsQueryable();
                    }
                    else
                    {
                        PurchaseInformation oPurchase = dbEntity.PurchaseInformations.Where(p => p.Code == refBillCode).Single();
                        oPayable = dbEntity.FinancePayments
                            .Where(p => p.Deleted == false 
                                && p.OrgID == ((organizationId == null) ? orgGid : organizationId)
                                && p.Pstatus == (byte)ModelEnum.FinanceStatus.CONFIRMED
                                && p.RefType == ((refType == null) ? (byte)ModelEnum.NoteType.ORDER : refType) 
                                && p.RefID == oPurchase.Gid)
                                .AsQueryable();
                    }
                }
            }
            catch
            {
                oPayable = (new List<FinancePayment>()).AsQueryable();
            }

            GridColumnModelList<FinancePayment> columns = new GridColumnModelList<FinancePayment>();
            columns.Add(m => m.Gid).SetAsPrimaryKey();
            columns.Add(m => m.Code);
            columns.Add(m => m.PaymentType == null ? "" : m.PaymentType.Name.GetResource(CurrentSession.Culture)).SetName("PaymentType");
            columns.Add(m => m.Pstatus);
            columns.Add(m => m.Currency == null ? "" : m.Currency.Name.GetResource(CurrentSession.Culture)).SetName("Currency");
            columns.Add(m => m.Amount);
            columns.Add(m => Convert.ToDateTime(m.PayDate.ToString()).ToShortDateString()).SetName("PayDate");

            GridData PayableGridData = oPayable.ToGridData(PayableSearchModel, columns);
            return Json(PayableGridData, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 在页面显示单据状态提示
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public ActionResult ShowNotice(string orderId,string refType)
        {
            
            ViewBag.bill = orderId;
            if (refType == "ORDER")
            {
                OrderInformation oOrder = dbEntity.OrderInformations.Where(o => o.Code == orderId).SingleOrDefault();
                if (oOrder != null)
                {
                    if (oOrder.PayStatus == (byte)ModelEnum.PayStatus.PAID)
                    {
                        ViewBag.NoticeContent = orderId + "  " + LiveAzure.Resource.Stage.OrderController.BillInvalid;
                        ViewBag.CheckedResult = 1;
                    }
                    else
                    {
                        ViewBag.NoticeContent = orderId + "  " + LiveAzure.Resource.Stage.OrderController.BillOK;
                        ViewBag.CheckedResult = 0;
                    }
                }
                else
                {
                    ViewBag.NoticeContent = orderId + "  " + LiveAzure.Resource.Stage.OrderController.BillInvalid;
                    ViewBag.CheckedResult = 1;
                }
            }
            else
            {
                PurchaseInformation oPurchase = dbEntity.PurchaseInformations.Where(p => p.Code == orderId).SingleOrDefault();
                if (oPurchase != null)
                {
                    if (oPurchase.Pstatus == (byte)ModelEnum.PurchaseStatus.NONE)
                    {
                        ViewBag.NoticeContent = orderId + "  " + LiveAzure.Resource.Stage.OrderController.BillInvalid;
                        ViewBag.CheckedResult = 1;
                    }
                    else
                    {
                        ViewBag.NoticeContent = orderId + "  " + LiveAzure.Resource.Stage.OrderController.BillOK;
                        ViewBag.CheckedResult = 0;
                    }
                }
                else
                {
                    ViewBag.NoticeContent = orderId + "  " + LiveAzure.Resource.Stage.OrderController.BillInvalid;
                    ViewBag.CheckedResult = 1;
                }
            }
            return PartialView();
        }

        /// <summary>
        /// 显示orderId对应的单据的货币
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public ActionResult ShowCurrency(string orderId,string refType)
        {
            if (refType == "ORDER")
            {
                OrderInformation oOrder = dbEntity.OrderInformations.Where(o => o.Code == orderId).SingleOrDefault();
                if (oOrder != null)
                {
                    ViewBag.currency = oOrder.Currency.Name.GetResource(CurrentSession.Culture);
                }
            }
            else
            {
                PurchaseInformation oPurchase = dbEntity.PurchaseInformations.Where(p => p.Code == orderId).SingleOrDefault();
                if (oPurchase != null)
                {
                    ViewBag.currency = oPurchase.Currency.Name.GetResource(CurrentSession.Culture);
                }
            }
            return PartialView();
        }

        //根据refId获取相应的单据
        public string getTypeOrder(byte type,Guid RefId)
        {
            string Code = "";
            if (type == (byte)ModelEnum.NoteType.ORDER)
            {
                Code = (from a in dbEntity.OrderInformations
                        where a.Deleted == false && a.Gid == RefId
                        select a.Code).FirstOrDefault();
            }
            else
            {
                Code = (from a in dbEntity.PurchaseInformations
                        where a.Deleted == false && a.Gid == RefId
                        select a.Code).FirstOrDefault();
            }
            return Code;
        }

        /// <summary>
        /// GridList
        /// </summary>
        /// <param name="PayableSearchModel"></param>
        /// <returns></returns>
        public ActionResult PayableList(SearchModel PayableSearchModel)
        {
            IQueryable<FinancePayment> oPayable;
            if (organizationId == null)
            {
                oPayable = dbEntity.FinancePayments.Where(p => p.Deleted == false).AsQueryable();
            }
            else
            {
                oPayable = dbEntity.FinancePayments.Where(p => p.Deleted == false && p.OrgID == organizationId).AsQueryable();
            }
            
            GridColumnModelList<FinancePayment> columns = new GridColumnModelList<FinancePayment>();
            columns.Add(m => m.Gid).SetAsPrimaryKey();
            columns.Add(m => m.Organization == null ? "" : m.Organization.FullName.GetResource(CurrentSession.Culture)).SetName("Organization");
            columns.Add(m => m.Code);
            columns.Add(m => m.PaymentType == null ? "" : m.PaymentType.Name.GetResource(CurrentSession.Culture)).SetName("PaymentType");
            columns.Add(m => m.PayTo);
            columns.Add(m => m.Pstatus);
            columns.Add(m => m.RefTypeName);
            columns.Add(m => m.RefID.Equals(Guid.Empty) ? "" : getTypeOrder(m.RefType, m.RefID)).SetName("RefID");
            columns.Add(m => Convert.ToDateTime(m.PayDate.ToString()).ToShortDateString()).SetName("PayDate");
            columns.Add(m => m.Reason);
            columns.Add(m => m.Currency == null ? "" : m.Currency.Name.GetResource(CurrentSession.Culture)).SetName("Currency");
            columns.Add(m => m.Amount);
            columns.Add(m => m.Remark);

            GridData PayableGridData = oPayable.ToGridData(PayableSearchModel, columns);
            return Json(PayableGridData, JsonRequestBehavior.AllowGet);
            
        }

        /// <summary>
        /// 获取退单类型的下拉框列表
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetPtypeSelectList()
        {
            List<SelectListItem> oList = new List<SelectListItem>();
            List<GeneralPrivateCategory> oCategory = dbEntity.GeneralPrivateCategorys.Include("Name").Where(p => p.Deleted == false && p.Ctype == (byte)ModelEnum.PrivateCategoryType.RETURNTYPE).ToList();
            foreach (GeneralPrivateCategory item in oCategory)
            {
                oList.Add(new SelectListItem { Text = item.Name.GetResource(CurrentSession.Culture), Value = item.Gid.ToString() });
            }
            return oList;
        }

        /// <summary>
        /// 获取退款方向的下拉框列表
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetPayToSelectList()
        {
            List<SelectListItem> oList = new List<SelectListItem>();
            List<ListItem> PayToList = new FinancePayment().PayToList;
            foreach (var item in PayToList)
            {
                oList.Add(new SelectListItem { Text = item.Text, Value = item.Value });
            }
            return oList;
        }

        /// <summary>
        /// 获取应付款状态的下拉框列表
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetPstatusSelectList()
        {
            List<SelectListItem> oList = base.SelectEnumList(typeof(ModelEnum.PayStatus), new FinancePayment().Pstatus);
            return oList;
        }

        /// <summary>
        /// 获取订单类型的下拉框列表
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetRefTypeSelectList()
        {
            List<SelectListItem> oList = new List<SelectListItem>();
            string[] sNames = LiveAzure.Resource.Common.ResourceManager.GetString(typeof(ModelEnum.NoteType).Name).Split(',');

            oList.Add(new SelectListItem { Value = ModelEnum.NoteType.ORDER.ToString(), Text = sNames[(int)ModelEnum.NoteType.ORDER] });
            oList.Add(new SelectListItem { Value = ModelEnum.NoteType.PURCHASE.ToString(), Text = sNames[(int)ModelEnum.NoteType.PURCHASE] });
            return oList;
        }

        public ActionResult GetPayToText()
        {
            StringBuilder s = new StringBuilder();
            List<ListItem> list = new FinancePayment().PayToList;
            foreach (ListItem item in list)
            {
                s.Append(item.Value + ":" + item.Text + ";");
            }
            return Json(s.ToString(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取应付款状态的多语言文本
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFinanceStatusText()
        {
            StringBuilder s = new StringBuilder();
            List<ListItem> list = new FinancePayment().PayStatusList;
            foreach (ListItem item in list)
            {
                s.Append(item.Value + ":" + item.Text + ";");
            }
            return Json(s.ToString(), JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
