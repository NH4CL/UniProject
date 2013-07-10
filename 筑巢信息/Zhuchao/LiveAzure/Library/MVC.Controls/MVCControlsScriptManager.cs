using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVC.Controls
{
    public class MVCControlsScriptManager
    {
        internal static readonly string SCRIPT_TEMPLATE = "<script language=\"javascript\" type=\"text/javascript\">{0}</script>";

        private static MVCControlsScriptManager _instance = null;
        private static object _lockKey = new object();

        private bool _useJqCustom = true;
        private string _version = "1.8.2";
        private Dictionary<string, string> _scripts = new Dictionary<string, string>();

        private MVCControlsScriptManager()
        {
        }


        /// <summary>
        /// Whether to use jquery.custom.ui js and css
        /// Defaults to true
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public MVCControlsScriptManager SetUseJqCustom(bool value, string version)
        {
            _version = version;
            _useJqCustom = value;
            return this;
        }

        public static MVCControlsScriptManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockKey)
                    {
                        _instance = new MVCControlsScriptManager();
                    }
                }

                return _instance;
            }
        }


        /// <summary>
        /// Adds the required js and css files
        /// </summary>
        /// <param name="contentRootPath">The relative position of the MVCControls content folder</param>
        /// <param name="scriptRootPath">The relative position of the MVCControls script folder</param>
        /// <returns></returns>
        public System.Web.Mvc.MvcHtmlString RegisterScriptsAndStyles(string contentRootPath, string scriptRootPath)
        {
            string scriptKey = contentRootPath + "@" + scriptRootPath;
            if (_scripts.ContainsKey(scriptKey))
                return new System.Web.Mvc.MvcHtmlString(_scripts[scriptKey]);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<link href=\"" + contentRootPath + "Grid/ui.jqgrid.css\" rel=\"stylesheet\" typ=\"text/css\" />");
            sb.AppendLine("<link href=\"" + contentRootPath + "mvc.controls.css\" rel=\"stylesheet\" type=\"text/css\" />");

            if (_useJqCustom)
            {
                sb.AppendLine("<link href=\"" + contentRootPath + "Grid/ui-lightness/jquery-ui-" + _version + ".custom.css\" rel=\"stylesheet\" type=\"text/css\" />");
                sb.AppendLine("<script src=\"" + scriptRootPath + "jquery-ui-" + _version  + ".custom.min.js\" type=\"text/javascript\"></script>");
            }

            sb.AppendLine("<script src=\"" + scriptRootPath + "Grid/grid.locale-en.js\" type=\"text/javascript\"></script>");
            sb.AppendLine("<script src=\"" + scriptRootPath + "Grid/jquery.jqGrid.min.js\" type=\"text/javascript\"></script>");
            sb.AppendLine("<script src=\"" + scriptRootPath + "Grid/grid.jqueryui.js\" type=\"text/javascript\"></script>");
            sb.AppendLine("<script src=\"" + scriptRootPath + "mvc.controls.manager.js\" type=\"text/javascript\"></script>");

            _scripts[scriptKey] = sb.ToString();

            return new System.Web.Mvc.MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// Adds the required js and css files
        /// Assumes MVCControls scripts are at: ../../Scripts/MVCControls/
        /// Assumes MVCControls styles are at: ../../Content/MVCControls/
        /// </summary>
        /// <returns></returns>
        public System.Web.Mvc.MvcHtmlString RegisterScriptsAndStyles()
        {
            return RegisterScriptsAndStyles("../../Content/MVCControls/", "../../Scripts/MVCControls/");
        }

    }
}
