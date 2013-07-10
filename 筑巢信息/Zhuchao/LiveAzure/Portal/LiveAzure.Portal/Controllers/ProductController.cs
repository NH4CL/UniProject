using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveAzure.Models;
using LiveAzure.Models.Product;

namespace LiveAzure.Portal.Controllers
{
    public class ProductController : BaseController
    {
        //
        // GET: /Product/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List()
        {
            return View();
        }
        public ActionResult Left()
        {
            return View();
        }
        public IQueryable<ProductOnSale> SearchProductOnSale(string keywords)
        {
            string strKeyWord = keywords.Trim();
            var SearchProduct =( from o in dbEntity.ProductOnSales.Include("Name").Include("Matter") where (o.Deleted == false && o.Name.Matter.Contains("strKeyWord")||o.Matter.CLOB.Contains("strKeyWord")) select o).AsQueryable();
            return SearchProduct;
        }
        public ActionResult Detail()
        {
            return View();
        }
        public ActionResult Tab()
        {
            return View();
        }
        
        public ActionResult ListShowStyle0()
        {
            return View();
        }
        public ActionResult ListShowStyle1()
        {
            return View();
        }
        public ActionResult ListShowStyle2()
        {
            return View();
        }

    }
}
