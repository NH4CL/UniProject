using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWorkplace_tiangang.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index(string a)
        {
            ViewBag.a = a;
            //Session.Add("tiem", DateTimeOffset.Now);
            //Session.Add("t1iem", DateTimeOffset.Today);
            //Session["dsjk"] = "djksj";
            //Session["ds2"] = "dsdfsdj";
            //Session["d4jk"] = "sdfsd5685";
            return View();
        }

    }
}
