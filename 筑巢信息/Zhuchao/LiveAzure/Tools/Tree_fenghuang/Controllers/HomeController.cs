using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveAzure.Models.General;
using LiveAzure.Models;
using LiveAzure.Utility;
using MVC.Controls.Grid;
using MVC.Controls;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Tree_fenghuang.Controllers
{
    public class HomeController :Controller
    {
        private LiveEntities dbEntity = new LiveEntities(ConfigHelper.LiveConnection.Connection);

        public ActionResult Index()
        {
            ViewBag.Message = "欢迎使用 ASP.NET MVC!";
            IpTest test = new IpTest();
            ViewBag.test = IpTest.SystemCheck();
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult ProgItemNode()
        {
            List<SelectListItem> modelList = new List<SelectListItem>();
            modelList.Add(new SelectListItem { Text = LiveAzure.Resource.Stage.ProgramController.inputBox, Value = "0" });
            modelList.Add(new SelectListItem { Text = LiveAzure.Resource.Stage.ProgramController.dropdownListBox, Value = "1" });
            ViewBag.modelList = modelList;
            GeneralProgNode addProgItemNode = new GeneralProgNode();
            return View(new GeneralProgNode());
        }

        public ActionResult ListProgs(SearchModel searchModel) 
        {
            IQueryable<GeneralProgNode> oPrograms = dbEntity.GeneralProgNodes.Include("Name").Where(p => p.Deleted == false).AsQueryable();
            int i = 2052;
            GridColumnModelList<GeneralProgNode> columns = new GridColumnModelList<GeneralProgNode>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Name.GetResource(i)).SetName("Name.Matter");
            columns.Add(p => p.Code);

            GridData gridData = oPrograms.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

       
    }

    public class IpTest : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //获取浏览器版本号
        public string getBrowser()
        {
            string browsers;
            HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
            string aa = bc.Browser.ToString();
            string bb = bc.Version.ToString();
            browsers = aa + bb;
            return browsers;
        }

        //获取客户端IP地址
        public string getIP()
        {
            string result = String.Empty;
            result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; 
            }
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.UserHostAddress;
                
            }
            if (null == result || result == String.Empty)
            {
                return "0.0.0.0";
            }
            return result;
        }

        //获取操作系统版本号
        public static string SystemCheck()
        {
            string Agent = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];

            if (Agent.IndexOf("NT 4.0") > 0)
            {
                return "Windows NT ";
            }
            else if (Agent.IndexOf("NT 5.0") > 0)
            {
                return "Windows 2000";
            }
            else if (Agent.IndexOf("NT 5.1") > 0)
            {
                return "Windows XP";
            }
            else if (Agent.IndexOf("NT 5.2") > 0)
            {
                return "Windows 2003";
            }
            else if (Agent.IndexOf("NT 6.0") > 0)
            {
                return "Windows Vista";
            }
            else if (Agent.IndexOf("WindowsCE") > 0)
            {
                return "Windows CE";
            }
            else if (Agent.IndexOf("NT") > 0)
            {
                return "Windows NT ";
            }
            else if (Agent.IndexOf("9x") > 0)
            {
                return "Windows ME";
            }
            else if (Agent.IndexOf("98") > 0)
            {
                return "Windows 98";
            }
            else if (Agent.IndexOf("95") > 0)
            {
                return "Windows 95";
            }
            else if (Agent.IndexOf("Win32") > 0)
            {
                return "Win32";
            }
            else if (Agent.IndexOf("Linux") > 0)
            {
                return "Linux";
            }
            else if (Agent.IndexOf("SunOS") > 0)
            {
                return "SunOS";
            }
            else if (Agent.IndexOf("Mac") > 0)
            {
                return "Mac";
            }
            else if (Agent.IndexOf("Linux") > 0)
            {
                return "Linux";
            }
            else if (Agent.IndexOf("Windows") > 0)
            {
                return "Windows";
            }
            return "未知类型";
        }
    }
}
