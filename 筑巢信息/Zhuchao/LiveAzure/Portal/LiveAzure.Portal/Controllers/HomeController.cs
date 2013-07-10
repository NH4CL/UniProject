using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveAzure.Models.General;
using LiveAzure.Models.Mall;
using LiveAzure.Models.Product;
using LiveAzure.Models.Member;
using LiveAzure.Models;
using LiveAzure.Utility;

namespace LiveAzure.Portal.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            //GetTopAdvertise();
            //NewProduct();
            //ProductClass();
            return View();
        }
        ///// <summary>
        ///// 首页滚动图片广告
        ///// </summary>
        //public void GetTopAdvertise()
        //{
        //    string strTopAd = Utility.ConfigHelper.GlobalConst.GetSetting("IndexScrollCMSCode");
        //    MemberOrganization oMemberOrganization = (from o in dbEntity.MemberOrganizations where (o.Deleted == false && o.Code == "Zhuchao") select o).Single();
        //    Guid OrgID = oMemberOrganization.Gid;//组织ID
        //    MallArtPosition oMallArtPosition = (from o in dbEntity.MallArtPositions where (o.Deleted == false && o.Code == "Top_Advertise") select o).SingleOrDefault();
        //    Guid PosID = oMallArtPosition.Gid;//文章的大小
        //    MallArticle oMallArticle = new MallArticle();
        //    Guid ArtID;
        //    Guid ChlID = CurrentSession.ChannelGid;
        //    if (strTopAd.Equals(string.Empty))
        //    {
        //        //文章代码为空时，根据位置和渠道，按照时间倒序排列，取第一个显示
        //        MallArtPublish oTopAdvertise = (from o in dbEntity.MallArtPublishes where (o.Deleted == false && o.Show == true && o.PosID == PosID && o.ChlID == ChlID) orderby o.CreateTime descending select o).FirstOrDefault();
        //        ArtID = oTopAdvertise.ArtID;//文章ID
        //        oMallArticle = (from o in dbEntity.MallArticles where (o.Deleted == false && o.Gid == ArtID) select o).FirstOrDefault();
        //        ViewBag.Article = oMallArticle.Matter.CLOB;
        //    }
        //    else
        //    {
        //        oMallArticle = (from o in dbEntity.MallArticles where (o.Deleted == false && o.OrgID == OrgID && o.Code == strTopAd) select o).Single();
        //        ViewBag.Article = oMallArticle.Matter.CLOB;
        //    }
        //    //oMallArticle = (from o in dbEntity.MallArticles where (o.Deleted == false && o.OrgID == OrgID && o.Code == "Top_Advertise") select o).Single();
        //    //ViewBag.Article = oMallArticle.Matter.CLOB;
        //}
        /// <summary>
        /// 推荐新品列表
        /// </summary>
        /// <returns></returns>
        //public List<ProductOnSale> NewProduct()
        //{
        //    //Guid ChlID = CurrentSession.ChannelGid;
        //    //List<ProductOnSale> NewProductList = new List<ProductOnSale>();
        //    //NewProductList = (from o in dbEntity.ProductOnSales.Include("Product") where (o.Deleted == false&&o.ChlID==ChlID) orderby o.SortingNew descending select o).Take(10).ToList();
        //    //ViewBag.NewProduct = NewProductList;
        //    return NewProductList;
        //}
        /// <summary>
        /// 产品分类
        /// </summary>
        /// <returns></returns>
        //public List<ProductOnSale> ProductClass()
        //{
        //    string strProductCat = Utility.ConfigHelper.GlobalConst.GetSetting("IndexShowCatCode");
        //    string[] strArr = strProductCat.Split(',');
        //    List<ProductOnSale> ProductClassList =new List<ProductOnSale>();
        //    for (int i = 0; i < strArr.Length; i++)
        //    {
        //        MemberOrganization oMemberOrganization = (from o in dbEntity.MemberOrganizations where (o.Deleted == false && o.Code == "Zhuchao") select o).Single();
        //        Guid OrgID = oMemberOrganization.Gid;//组织ID
        //        string strCode = strArr[i];
        //        MallArticle oMallArticle = (from o in dbEntity.MallArticles where (o.Code == strCode && o.OrgID == OrgID && o.Deleted == false) select o).SingleOrDefault();
        //        if (oMallArticle != null)
        //        {
        //            ViewBag.CMS = oMallArticle.Matter.CLOB;
        //        }
        //        ProductClassList = (from o in dbEntity.ProductOnSales.Include("Product")
        //                            join m in dbEntity.ProductExtendCategories.Include("PrivateCategory") on o.ProdID equals m.ProdID
        //                            where (m.PrivateCategory.Code == strCode)
        //                            orderby o.SortingPush descending
        //                            select o).Take(8).ToList();
        //        ViewBag.ProductClass = ProductClassList;
        //    }
                
            
        //    return ProductClassList;

        //}
        public ActionResult Help()
        {
            return View();
        }
    }
}
