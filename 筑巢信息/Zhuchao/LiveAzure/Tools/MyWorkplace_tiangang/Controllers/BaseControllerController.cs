using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Globalization;
using LiveAzure.Utility;
using LiveAzure.Models;

namespace LiveAzure.Stage.Controllers
{
    /// <summary>
    /// 控制器基类，所有控制器继承自该类
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// 数据库连接，全局变量
        /// </summary>
        public LiveEntities dbEntity = new LiveEntities(ConfigHelper.LiveConnection.Connection);
        /// <summary>
        /// Session
        /// </summary>
        public SessionData oSessionData = new SessionData();
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
            string cultureName = null;
            // Attempt to read the culture cookie from Request
            HttpCookie cultureCookie = Request.Cookies["_culture"];
            if (cultureCookie != null)
                cultureName = cultureCookie.Value;
            else
                cultureName = Request.UserLanguages[0]; // obtain it from HTTP header AcceptLanguages
            // Validate culture name
            cultureName = CultureHelper.GetValidCulture(cultureName); // This is safe
            // Modify current thread's culture            
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureName);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(cultureName);
            base.ExecuteCore();
        }

        /// <summary>
        /// Session Name from GeneralConfig
        /// </summary>
        /// <returns></returns>
        private string GetSessionName()
        {
            return "SessionName";
        }

        /// <summary>
        /// 获取/设置Session数据
        /// </summary>
        public SessionData CurrentSession
        {
            get
            {
                object obj = Session[GetSessionName()];
                if (obj == null)
                    return new SessionData();
                else
                    return (SessionData)obj;
            }
            set
            {
                Session.Clear();
                Session.Add(GetSessionName(), value);
            }
        }

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

    }
}