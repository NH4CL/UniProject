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

namespace LiveTest.Bojian.Controllers
{
    /// <summary>
    /// 控制器基类，所有控制器继承自该类
    /// </summary>
    public class BaseController : Controller
    {
        #region 全局变量

        public LiveEntities dbEntity;        // 数据库连接，全局变量
        public EventBLL oEventBLL;           // 事件记录工具

        #endregion

        #region Controller 核心重载

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseController()
        {
            dbEntity = new LiveEntities(ConfigHelper.LiveConnection.Connection);
            oEventBLL = new EventBLL(dbEntity);
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
            string cultureName = Thread.CurrentThread.CurrentCulture.Name; // e.g. "en-US" // filterContext.HttpContext.Request.UserLanguages[0]; // needs validation return "en-us" as default            

            // Is it default culture? exit
            if (cultureName == CultureHelper.GetDefaultCulture()) return;

            // Are views implemented separately for this culture?  if not exit
            bool viewImplemented = CultureHelper.IsViewSeparate(cultureName);
            if (viewImplemented == false) return;

            string viewName = view.ViewName;

            //int i = 0;
            //if (string.IsNullOrEmpty(viewName))
            //    viewName = filterContext.RouteData.Values["action"] + "." + cultureName; // Index.en-US
            //else if ((i = viewName.IndexOf('.')) > 0)
            //{
            //    // contains . like "Index.cshtml"                
            //    viewName = viewName.Substring(0, i + 1) + cultureName + viewName.Substring(i);
            //}
            //else
            //    viewName += "." + cultureName; // e.g. "Index" ==> "Index.en-Us"

            view.ViewName = viewName;
            filterContext.Controller.ViewBag._culture = "." + cultureName;
            base.OnActionExecuted(filterContext);
        }

        protected override void ExecuteCore()
        {
            //string cultureName = null;
            // Attempt to read the culture cookie from Request
            //HttpCookie cultureCookie = Request.Cookies["_culture"];
            if (CurrentSession.Culture == 0)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Request.UserLanguages[0]);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(Request.UserLanguages[0]);
            }
            else
            {
                CultureInfo cultureName = new CultureInfo(CurrentSession.Culture);
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureName.Name);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(cultureName.Name);
            }

            //if (cultureCookie != null)
            //    cultureName = cultureCookie.Value;
            //else
            //    cultureName = Request.UserLanguages[0]; // obtain it from HTTP header AcceptLanguages
            //// Validate culture name
            //cultureName = CultureHelper.GetValidCulture(cultureName); // This is safe
            //// Modify current thread's culture            
            //Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureName);
            //Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(cultureName);
            base.ExecuteCore();
        }

        #endregion

        #region 常用工具

        /// <summary>
        /// 将ListItem项转换成SelectListItem（MVC使用）
        /// </summary>
        /// <param name="oItems">ListItem原始项</param>
        /// <returns>SelectListItem项</returns>
        public List<SelectListItem> GetSelectList(List<ListItem> oItems)
        {
            List<SelectListItem> oResult = new List<SelectListItem>();
            if (oItems != null)
                foreach (ListItem oItem in oItems)
                    oResult.Add(new SelectListItem { Selected = oItem.Selected, Text = oItem.Text, Value = oItem.Value });
            return oResult;
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
                string sName = "ZhuchaoSession";
                GeneralConfig oConfig = dbEntity.GeneralConfigs.Where(c => c.Code == "SessionName").FirstOrDefault();
                if (oConfig != null)
                    sName = oConfig.StrValue;
                object obj = Session[sName];
                if (obj == null)
                    return new SessionData();
                else
                    return (SessionData)obj;
            }
            set
            {
                string sName = "ZhuchaoSession";
                GeneralConfig oConfig = dbEntity.GeneralConfigs.Where(c => c.Code == "SessionName").FirstOrDefault();
                if (oConfig != null)
                    sName = oConfig.StrValue;
                value.SessionName = sName;
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
                string sName = "ZhuchaoCookie";
                GeneralConfig oConfig = dbEntity.GeneralConfigs.Where(c => c.Code == "CookieName").FirstOrDefault();
                if (oConfig != null)
                    sName = oConfig.StrValue;
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
                string sName = "ZhuchaoCookie";
                GeneralConfig oConfig = dbEntity.GeneralConfigs.Where(c => c.Code == "CookieName").FirstOrDefault();
                if (oConfig != null)
                    sName = oConfig.StrValue;
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
            string strTreeJson = "{\"id\": \"00000000-0000-0000-0000-000000000000\", \"name\":\"root\", \"progUrl\":\"\", \"icon\":\"\", \"iconClose\":\"\", \"iconOpen\":\"\", \"isParent\":\"true\","
                + "\"checkedCol\":\"nodeChecked\", \"nodeChecked\":\"true\", \"target\":\" \", \"open\":\"true\", \"nodes\":[";

            strTreeJson = CreateTreeJson(list, strTreeJson);

            strTreeJson += "]}";

            return "[" + strTreeJson + "]";
        }

        /// <summary>
        /// 递归生成树节点的JSON字符串
        /// </summary>
        /// <param name="list">treenode的list</param>
        /// <param name="strJson">返回字符串</param>
        /// <returns></returns>
        public string CreateTreeJson(List<LiveTreeNode> list, string strJson)
        {
            string strTreeJson = strJson;

            int nListCount = list.Count;

            for (int i = 0; i < nListCount; i++)
            {
                LiveTreeNode treeNode = list.ElementAt(i);

                strTreeJson += "{\"id\": \"" + treeNode.id + "\", \"name\":\"" + treeNode.name + "\", \"progUrl\":\"" + treeNode.progUrl + "\", \"icon\":\""
                    + treeNode.icon + "\", \"iconClose\":\"" + treeNode.iconClose + "\", \"iconOpen\":\"" + treeNode.iconOpen + "\", \"isParent\":\""
                    + treeNode.isParent.ToString().ToLower() + "\", \"checkedCol\":\"" + treeNode.checkedCol + "\", \"nodeChecked\":\"" + treeNode.nodeChecked.ToString().ToLower()
                    + "\", \"nodes\":[";

                if (treeNode.nodes.Count > 0)
                {
                    strTreeJson = CreateTreeJson(treeNode.nodes, strTreeJson);
                }

                strTreeJson += "]";

                if (i == nListCount - 1)
                {
                    strTreeJson += "}";
                }
                else
                {
                    strTreeJson += "},";
                }
            }

            return strTreeJson;

        }

        #endregion
    }
}
