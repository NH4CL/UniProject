using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Globalization;
using LiveAzure.Utility;
using LiveAzure.BLL;
using LiveAzure.Models;
using LiveAzure.Models.General;
using LiveAzure.Models.Member;

namespace LiveAzure.Stage.Controllers
{
    /// <summary>
    /// 控制器基类，所有控制器继承自该类
    /// </summary>
    public partial class BaseController : Controller
    {
        #region 全局变量
        
        public LiveEntities dbEntity;                     // 数据库连接，全局变量
        public EventBLL oEventBLL;                        // 事件记录工具
        public GeneralBLL oGeneralBLL;                    // 通用业务逻辑
        public static Dictionary<string, string> oProgramNodes = new Dictionary<string, string>();  // 程序节点，功能权限

        #endregion

        #region Controller 核心重载

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseController()
        {
            dbEntity = new LiveEntities(ConfigHelper.LiveConnection.Connection);
            oEventBLL = new EventBLL(dbEntity);
            oGeneralBLL = new GeneralBLL(dbEntity);
        }
        
        /// <summary>
        /// 释放数据库连接
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            dbEntity.Dispose();
            base.Dispose(disposing);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // Is it View ?
            ViewResultBase view = filterContext.Result as ViewResultBase;
            if (view == null) return;
            string cultureName = CultureHelper.GetValidCulture(Thread.CurrentThread.CurrentCulture.Name); // e.g. "en-US" // filterContext.HttpContext.Request.UserLanguages[0]; // needs validation return "en-us" as default
            
            // Is it default culture? exit
            if (cultureName == CultureHelper.GetDefaultCulture()) return;

            // Are views implemented separately for this culture?  if not exit
            if (!CultureHelper.IsViewSeparate(cultureName)) return;

            string viewName = view.ViewName;

            int i = 0;
            if (string.IsNullOrEmpty(viewName))
                viewName = filterContext.RouteData.Values["action"] + "." + cultureName; // Index.en-US
            else if ((i = viewName.IndexOf('.')) > 0)
            {
                // contains . like "Index.cshtml"                
                viewName = viewName.Substring(0, i + 1) + cultureName + viewName.Substring(i);
            }
            else
                viewName += "." + cultureName; // e.g. "Index" ==> "Index.en-Us"

            view.ViewName = viewName;
            filterContext.Controller.ViewBag._culture = "." + cultureName;
            base.OnActionExecuted(filterContext);
        }

        protected override void ExecuteCore()
        {
            if (CurrentSession.Culture == 0)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Request.UserLanguages[0]);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(Request.UserLanguages[0]);
            }
            else {
            CultureInfo cultureName = new CultureInfo(CurrentSession.Culture);
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureName.Name);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(cultureName.Name);
            }

            base.ExecuteCore();

        }


        /// <summary>
        /// 判断是否登录
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool isHomeController = (string)filterContext.RouteData.Values["Controller"] == "Home";
            bool isLogonAction = ((string)filterContext.RouteData.Values["Action"] == "Index") || ((string)filterContext.RouteData.Values["Action"] == "checkUser");
            if (!(isHomeController && isLogonAction) && !CurrentSession.IsLogin)
            {
                filterContext.Result = RedirectToAction("Index", "Home");
            }
            base.OnActionExecuting(filterContext);
        }

        #endregion

        #region 常用工具

        /// <summary>
        /// 将ListItem项转换成SelectListItem（MVC使用）
        /// </summary>
        /// <param name="oItems">ListItem原始项</param>
        /// <returns>SelectListItem项</returns>
        public List<SelectListItem> GetSelectList(IEnumerable<ListItem> oItems)
        {
            List<SelectListItem> oResult = new List<SelectListItem>();
            if (oItems != null)
                foreach (ListItem oItem in oItems)
                    oResult.Add(new SelectListItem { Selected = oItem.Selected, Text = oItem.Text, Value = oItem.Value });
            return oResult;
        }

        /// <summary>
        /// 枚举类型列表
        /// </summary>
        /// <param name="oEnumType">枚举类型</param>
        /// <param name="nSelectedIndex">选中的项</param>
        /// <returns></returns>
        public List<SelectListItem> SelectEnumList(Type oEnumType, byte nSelectedIndex = 0)
        {
            return GetSelectList(oGeneralBLL.SelectEnumList(oEnumType, nSelectedIndex));
        }

        /// <summary>
        /// 枚举类型ture/false
        /// </summary>
        /// <param name="bSelectedIndex">选中的项</param>
        /// <returns></returns>
        public List<SelectListItem> SelectEnumList(bool bSelectedIndex)
        {
            List<SelectListItem> oResult = new List<SelectListItem>();
            if (bSelectedIndex)
            {
                oResult.Add(new SelectListItem { Selected = false, Text = LiveAzure.Resource.Common.No, Value = "false" });
                oResult.Add(new SelectListItem { Selected = true, Text = LiveAzure.Resource.Common.Yes, Value = "true" });
            }
            else
            {
                oResult.Add(new SelectListItem { Selected = true, Text = LiveAzure.Resource.Common.No, Value = "false" });
                oResult.Add(new SelectListItem { Selected = false, Text = LiveAzure.Resource.Common.Yes, Value = "true" });
            }
            return oResult;
        }

        /// <summary>
        /// 获取用户有权限的组织（给下拉框用）
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSupportOrganizations()
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
                                             join org in dbEntity.MemberOrganizations.AsEnumerable() on pi.RefID equals org.Gid
                                             where pi.Deleted == false
                                                   && pi.Privilege.UserID == UserID
                                                   && pi.Privilege.Ptype == (byte)ModelEnum.UserPrivType.ORGANIZATION
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
                if (item.Gid == CurrentSession.OrganizationGID)//默认选中自己所属组织
                    ogranizationList.Add(new SelectListItem { Text = item.FullName.GetResource(CurrentSession.Culture), Value = item.Gid.ToString(), Selected = true });
                else
                    ogranizationList.Add(new SelectListItem { Text = item.FullName.GetResource(CurrentSession.Culture), Value = item.Gid.ToString(), Selected = false });
            }
            return ogranizationList;
        }

        /// <summary>
        /// 获取指定用户组织ID
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public Guid GetUserOrganizationID(Guid UserID)
        {
            return dbEntity.MemberUsers.Where(u => u.Gid == UserID && u.Deleted == false).FirstOrDefault().OrgID;
        }

        /// <summary>
        /// 获取当前程序路径页面跳转使用
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        public bool SetCurrentPath()
        {
            string strProgramCode = Request.RequestContext.RouteData.Values["Controller"].ToString() +
                Request.RequestContext.RouteData.Values["Action"].ToString();
            Guid programId = dbEntity.GeneralPrograms.Where(p => p.Deleted == false && p.Code == strProgramCode).FirstOrDefault().Gid;
            bool result = false;
            if (programId != null)
            {
                Session["CurrentPath"] = oGeneralBLL.getPath(programId, CurrentSession.Culture);
                result = true;
            }
            return result;
        }
        #endregion

        #region 权限验证

        /// <summary>
        /// 验证程序进入权限
        /// </summary>
        /// <param name="sProgramCode">程序代码</param>
        /// <remarks>
        ///     全局变量oProgNodes，程序功能节点的授权值，例如
        ///     OrderConfirm 1            表示能按订单确认按钮
        ///     OrderArrange 0            表示能按订单排单按钮
        ///     StartDate    2011-08-23   表示某个参数的日期值
        /// </remarks>
        /// <returns>true有权限，false无权限</returns>
        public bool Permission(string sProgramCode)
        {
            oProgramNodes.Clear();
            if (!CurrentSession.UserID.HasValue)
            {
                this.oEventBLL.WriteEvent("Privilege Validation Failed, No User ID in Session",
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, sProgramCode);
                return false;
            }
            Guid gUserID = CurrentSession.UserID.Value;
            this.oEventBLL.WriteEvent("Privilege Validation",
                ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, sProgramCode, gUserID);
            var oUser = (from u in dbEntity.MemberUsers.Include("Role")
                         where u.Gid == gUserID && u.Ustatus == (byte)ModelEnum.UserStatus.VALID && u.Deleted == false
                         select u).FirstOrDefault();
            bool bHasPower = false;
            if (oUser != null)
            {
                if (oUser.Role.Code == "Supervisor")
                {
                    // 超级管理员，不判断权限
                    oProgramNodes.Add(oUser.Role.Code, "1");
                    bHasPower = true;
                }
                else
                {
                    // 一般用户，判断权限
                    var oPrivilege = (from p in dbEntity.MemberPrivileges
                                      join i in dbEntity.MemberPrivItems on p.Gid equals i.PrivID
                                      join g in dbEntity.GeneralPrograms on i.RefID equals g.Gid
                                      where p.Deleted == false && p.UserID == oUser.Gid && p.Ptype == (byte)ModelEnum.UserPrivType.PROGRAM
                                            && i.Deleted == false
                                            && g.Deleted == false && g.Code == sProgramCode
                                      select i).FirstOrDefault();
                    if (oPrivilege != null)
                    {
                        // 程序节点功能权限
                        var oNodes = from p in dbEntity.MemberPrivileges
                                     join i in dbEntity.MemberPrivItems on p.Gid equals i.PrivID
                                     join n in dbEntity.GeneralProgNodes on i.RefID equals n.Gid
                                     join g in dbEntity.GeneralPrograms on n.ProgID equals g.Gid
                                     where p.Deleted == false && p.UserID == oUser.Gid && p.Ptype == (byte)ModelEnum.UserPrivType.PROGRAM_NODE
                                           && i.Deleted == false
                                           && n.Deleted == false
                                           && g.Deleted == false && g.Code == sProgramCode
                                     select i;
                        foreach (var item in oNodes)
                            oProgramNodes.Add(item.NodeCode, item.NodeValue);
                        bHasPower = true;
                    }
                }
            }
            return bHasPower;
        }

        /// <summary>
        /// 根据授权类别，查询用户管辖的GUID，例如组织，渠道，仓库等
        /// </summary>
        /// <param name="privType">授权类型</param>
        /// <returns>对应授权类型的Guid列表，即MemberPrivItem中的RefID值</returns>
        public List<Guid> Permission(ModelEnum.UserPrivType privType)
        {
            List<Guid> oList = new List<Guid>();
            switch (privType)
            {
                case ModelEnum.UserPrivType.ORGANIZATION:
                case ModelEnum.UserPrivType.CHANNEL:
                case ModelEnum.UserPrivType.WAREHOUSE:
                case ModelEnum.UserPrivType.SUPPLIER_CATEGORY:
                    var items = (from p in dbEntity.MemberPrivileges.Include("PrivilegeItems")
                                 where p.UserID == CurrentSession.UserID && p.Deleted == false && p.Ptype == (byte)privType && p.Pstatus == (byte)ModelEnum.GenericStatus.VALID//add by tianyou 2011/10/21 添加判断条件：启用状态
                                 select p).FirstOrDefault();
                    //-----------------------------edit by tianyou 2011/10/15
                    //添加判断----若items为空，则说明用户没有该授权类别的权限或者没有启用
                    if (items != null)//非空 查找子表的RefID
                    {
                        foreach (var item in items.PrivilegeItems)
                            if (item.RefID.HasValue && item.Deleted==false)//
                                oList.Add(item.RefID.Value);
                    }
                    //-----------------------------------------------end edit
                    break;
                case ModelEnum.UserPrivType.PRODUCT_CATEGORY:
                    oList = dbEntity.Database.SqlQuery<Guid>("SELECT Gid FROM dbo.fn_FindFullCategories({0})", CurrentSession.UserID).ToList();
                    break;
            }
            return oList;
        }

        /// <summary>
        /// 查询程序节点的授权值
        /// </summary>
        /// <param name="nodeCode">当前程序的节点代码</param>
        /// <returns>值</returns>
        public string GetProgramNode(string nodeCode)
        {
            string nodeValue = "";
            if (oProgramNodes.ContainsKey("Supervisor"))
                nodeValue = oProgramNodes["Supervisor"];
            else if (oProgramNodes.ContainsKey(nodeCode))
                nodeValue = oProgramNodes[nodeCode];
            return nodeValue;
        }

        /// <summary>
        /// 通用程序权限验证
        /// </summary>
        /// <returns></returns>
        public bool CheckPrivilege()
        {
            string strProgramCode = Request.RequestContext.RouteData.Values["Controller"].ToString() +
                Request.RequestContext.RouteData.Values["Action"].ToString();
            if (!Permission(strProgramCode))
                return false;
            ViewBag.privEnableEdit = GetProgramNode("EnableEdit");//允许编辑
            ViewBag.privEnableDelete = GetProgramNode("EnableDelete");//允许删除/作废
            ViewBag.privEnablePrint = GetProgramNode("EnablePrint");//允许打印
            ViewBag.privEnablePrepare = GetProgramNode("EnablePrepare");//允许制表(编辑)
            ViewBag.privEnableApprove = GetProgramNode("EnableApprove");//允许确认
            ViewBag.privEnableSignFor = GetProgramNode("EnableSignFor");//允许签收
            return true;
        }
        /// <summary>
        /// 程序功能节点权限验证
        /// </summary>
        /// <param name="ProgNode">功能节点代码</param>
        /// <returns></returns>
        public bool CheckPrivilege(string ProgNode)
        {
            if (GetProgramNode(ProgNode) != "1")
                return false;
            else
                return true;
        }

        #endregion

        #region 资源文件预处理

        /// <summary>
        /// 产生多语言的资源文件
        /// </summary>
        /// <param name="rtype">资源类型：字符或金额</param>
        /// <param name="organID">组织，空则表示用系统支持的所有语言</param>
        /// <returns>新资源文件</returns>
        public GeneralResource NewResource(ModelEnum.ResourceType rtype, Guid? organID = null)
        {
            return oGeneralBLL.NewResource(rtype, organID);
        }

        /// <summary>
        /// 更新已经存在的资源文件，包括插入新语言/货币，删除过期的语言/货币等
        /// </summary>
        /// <param name="rtype">资源类型：字符，金额</param>
        /// <param name="resource">原资源文件</param>
        /// <param name="organID">组织ID，空表示用系统支持的语言/货币刷新</param>
        /// <returns>新资源文件</returns>
        public GeneralResource RefreshResource(ModelEnum.ResourceType rtype, GeneralResource resource, Guid? organID = null)
        {
            return oGeneralBLL.RefreshResource(rtype, resource, organID);
        }

        /// <summary>
        /// 产生多语言的大对象资源文件
        /// </summary>
        /// <param name="organID">组织，空则表示用系统支持的所有语言</param>
        /// <returns>新资源文件</returns>
        public GeneralLargeObject NewLargeObject(Guid? organID = null)
        {
            return oGeneralBLL.NewLargeObject(organID);
        }

        /// <summary>
        /// 更新已经存在的大对象资源文件，包括插入新语言，删除过期的语言等
        /// </summary>
        /// <param name="large">原资源文件</param>
        /// <param name="organID">组织ID，空表示用系统支持的语言/货币刷新</param>
        /// <returns>新资源文件</returns>
        public GeneralLargeObject RefreshLargeObject(GeneralLargeObject large, Guid? organID = null)
        {
            return oGeneralBLL.RefreshLargeObject(large, organID);
        }

        #endregion

        #region Session和Cookie操作

        /// <summary>
        /// 获取/设置Session数据
        /// </summary>
        public SessionData CurrentSession
        {
            get
            {
               string sName = (from c in dbEntity.GeneralConfigs
                                where c.Code == "SessionName"
                                select c.StrValue).FirstOrDefault();
                if (sName == null)
                    sName = "ZhuchaoSession";
                object obj = Session[sName];
                if (obj == null)
                    return new SessionData();
                else
                    return (SessionData)obj;
            }
            set
            {
                string sName = (from c in dbEntity.GeneralConfigs
                                where c.Code == "SessionName"
                                select c.StrValue).FirstOrDefault();
                if (sName == null)
                    sName = "ZhuchaoSession";
                value.SessionName = sName;
                if (value.Culture.Equals(0))//设置当前默认货币
                {
                    GeneralCultureUnit cultureunit = dbEntity.GeneralCultureUnits.Where(u => u.Culture == value.Culture).FirstOrDefault();
                    if (cultureunit!=null)
                        value.Currency = cultureunit.aCurrency;
                }
                Session.Clear();
                Session.Add(sName, value);
            }
        }

        /// <summary>
        /// 获取/设置Cookie数据
        /// </summary>
        public CookieData CurrentCookie
        {
            get
            {
                string sName = (from c in dbEntity.GeneralConfigs
                                where c.Code == "CookieName"
                                select c.StrValue).FirstOrDefault();
                if (sName == null)
                    sName = "ZhuchaoCookie";
                if (Request.Cookies[sName] != null)
                {
                    HttpCookie oCookie = Request.Cookies[sName];
                    CookieData cookieData = new CookieData
                    {
                        CookieName = sName
                    };
                    if (oCookie.Values["UserID"] != null && oCookie.Values["UserID"] != "")
                    {
                        cookieData.Remember = Convert.ToByte(oCookie.Values["Remember"]);
                        cookieData.UserID = Guid.Parse(oCookie.Values["UserID"]);
                        cookieData.LastLoginTime = oCookie.Values["LastLoginTime"];
                    }
                    return cookieData;
                }
                return null;
            }
            set
            {
                string sName = (from c in dbEntity.GeneralConfigs
                                where c.Code == "CookieName"
                                select c.StrValue).FirstOrDefault();
                if (sName == null)
                    sName = "ZhuchaoCookie";
                HttpCookie oCookie = Request.Cookies[sName];
                if (oCookie == null)
                {
                    oCookie = new HttpCookie(sName);
                    oCookie.Values["CookieName"] = sName;
                }
                oCookie.Expires = DateTime.Now.AddYears(1);
                oCookie.HttpOnly = false;
                oCookie.Values["UserID"] = value.UserID.ToString();
                oCookie.Values["Remember"] = value.Remember.ToString();
                oCookie.Values["LastLoginTime"] = value.LastLoginTime;
                Response.Cookies.Add(oCookie);
            }
        }

        #endregion

        #region JS树

        /// <summary>
        /// 生成带有root节点的树状结构的Json
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public string CreateTree(List<LiveTreeNode> list)
        {
            List<LiveTreeNode> nodes = new List<LiveTreeNode>
            {
                new LiveTreeNode
                {
                    id = Guid.Empty.ToString(),
                    name = "root",
                    isParent = true,
                    open = true,
                    nodes = list
                }
            };
            return nodes.ToJsonString();
        }
        #endregion

    }
}
