using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LiveAzure.Portal
{
    public class LiveViewEngine : RazorViewEngine
    {

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            string viewPrefix = Utility.ConfigHelper.GlobalConst.GetSetting("ViewFolder");
            ViewEngineResult result = null;
            var request = controllerContext.HttpContext.Request;
            //This could be replaced with a switch statement as other advanced / device specific views are created
            // Avoid unnecessary checks if this device isn't suspected to be a mobile device
            if (request.Browser.IsMobileDevice)
            {
                if (UserAgentIs(controllerContext, "iPhone"))
                    result = base.FindView(controllerContext, viewPrefix + "iPhone/" + viewName, masterName, useCache);
                else if (UserAgentIs(controllerContext, "MSIEMobile 6"))
                    result = base.FindView(controllerContext, viewPrefix + "MobileIE6/" + viewName, masterName, useCache);
                else if (UserAgentIs(controllerContext, "PocketIE") && request.Browser.MajorVersion >= 4)
                    result = base.FindView(controllerContext, viewPrefix + "PocketIE/" + viewName, masterName, useCache);
                // Fall back to default mobile view if no other mobile view has already been set
                if ((result == null || result.View == null) && request.Browser.IsMobileDevice)
                    result = base.FindView(controllerContext, viewPrefix + "Mobile/" + viewName, masterName, useCache);
            }
            else
            {
                result = base.FindView(controllerContext, viewPrefix + viewName, masterName, useCache);
            }
            //Fall back to desktop view if no other view has been selected
            if (result == null || result.View == null)
                result = base.FindView(controllerContext, viewName, masterName, useCache);
            return result;
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            string viewPrefix = Utility.ConfigHelper.GlobalConst.GetSetting("ViewFolder");
            ViewEngineResult result = null;
            var request = controllerContext.HttpContext.Request;
            //This could be replaced with a switch statement as other advanced / device specific views are created
            // Avoid unnecessary checks if this device isn't suspected to be a mobile device
            if (request.Browser.IsMobileDevice)
            {
                if (UserAgentIs(controllerContext, "iPhone"))
                    result = base.FindPartialView(controllerContext, viewPrefix + "Mobile/iPhone/" + partialViewName, useCache);
                else if (UserAgentIs(controllerContext, "MSIEMobile 6"))
                    result = base.FindPartialView(controllerContext, viewPrefix + "Mobile/MobileIE6/" + partialViewName, useCache);
                else if (UserAgentIs(controllerContext, "PocketIE") && request.Browser.MajorVersion >= 4)
                    result = base.FindPartialView(controllerContext, viewPrefix + "Mobile/PocketIE/" + partialViewName, useCache);
                // Fall back to default mobile view if no other mobile view has already been set
                if ((result == null || result.View == null) && request.Browser.IsMobileDevice)
                    result = base.FindPartialView(controllerContext, viewPrefix + "Mobile/" + partialViewName, useCache);
            }
            else
            {
                result = base.FindPartialView(controllerContext, viewPrefix + partialViewName, useCache);
            }
            //Fall back to desktop view if no other view has been selected
            if (result == null || result.View == null)
                result = base.FindPartialView(controllerContext, partialViewName, useCache);
            return result;
        }

        public bool UserAgentIs(ControllerContext controllerContext, string userAgentToTest)
        {
            return (controllerContext.HttpContext.Request.UserAgent.IndexOf(userAgentToTest, StringComparison.OrdinalIgnoreCase) > 0);
        }
    }
}
