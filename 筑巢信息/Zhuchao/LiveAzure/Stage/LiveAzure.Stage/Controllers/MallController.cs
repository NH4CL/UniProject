using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveAzure.Models;
using LiveAzure.Models.Member;
using LiveAzure.Models.Mall;
using LiveAzure.Models.General;
using LiveAzure.Models.Product;
using MVC.Controls;
using MVC.Controls.Grid;

namespace LiveAzure.Stage.Controllers
{
    public class MallController : BaseController
    {
        //
        // GET: /Mall/
        private static Guid gOrgId;//全局变量组织ID
        private static Guid gCategoryId;//文章分类ID
        private static Guid gChlId;//渠道
        /// <summary>
        /// MallArtPosition入口
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //生成组织下拉框
            Guid? userID = CurrentSession.UserID;
            if (CurrentSession.IsAdmin == true)
            {
                MemberUser oMemberUser = (from n in dbEntity.MemberUsers where (n.Deleted == false && n.Gid == userID) select n).Single();
                Guid gUserOrgID = oMemberUser.OrgID;
                var oMemberOrganizationList =(from o in dbEntity.MemberOrganizations.Include("FullName") where(o.Deleted==false&&o.Otype==0) select o).ToList();
                List<SelectListItem> list = new List<SelectListItem>();
                foreach (var item in oMemberOrganizationList)
                {
                    SelectListItem sitem = new SelectListItem();
                    sitem.Text = item.FullName.GetResource(CurrentSession.Culture);
                    sitem.Value = item.Gid.ToString();
                    if (item.Gid == gUserOrgID) sitem.Selected = true;
                    list.Add(sitem);
                }
                ViewBag.list = list;

                gOrgId = gUserOrgID;
            }
            else
            {
                if (userID != null)
                {
                    // 该用户有权限管理的组织，生成下拉框
                    MemberPrivilege oMemberPrivilege = (from o in dbEntity.MemberPrivileges where (o.Deleted == false && o.Ptype == 2) select o).SingleOrDefault();
                    List<SelectListItem> list = new List<SelectListItem>();
                    if (userID != null)
                    {
                        //有查看多个组织的权限
                        if (oMemberPrivilege != null)
                        {
                            Guid gPrivID = oMemberPrivilege.Gid;
                            var oMemberPrivItem = (from i in dbEntity.MemberPrivItems where (i.Deleted == false && i.PrivID == gPrivID) select i).ToList();
                            foreach (var PrivItem in oMemberPrivItem)
                            {
                                Guid gOrgID = (Guid)PrivItem.RefID;
                                MemberOrganization oMemberOrganization = (from m in dbEntity.MemberOrganizations.Include("FullName") where (m.Deleted == false && m.Gid == gOrgID) select m).Single();
                                SelectListItem item = new SelectListItem();
                                item.Text = oMemberOrganization.FullName.GetResource(CurrentSession.Culture);
                                item.Value = gOrgID.ToString();
                                list.Add(item);
                            }
                            //用户的默认组织不包含在MemberPrivItem表中，单独添加，并且默认为选中
                            MemberUser oMemberUser = (from n in dbEntity.MemberUsers where (n.Deleted == false && n.Gid == userID) select n).Single();
                            Guid gUserOrgID = oMemberUser.OrgID;
                            MemberOrganization oUserMemberOrganization = (from s in dbEntity.MemberOrganizations.Include("FullName") where (s.Deleted == false && s.Gid == gUserOrgID) select s).Single();
                            SelectListItem Sitem = new SelectListItem();
                            Sitem.Text = oUserMemberOrganization.FullName.GetResource(CurrentSession.Culture);
                            Sitem.Value = gUserOrgID.ToString();
                            Sitem.Selected = true;
                            list.Add(Sitem);

                            ViewBag.list = list;

                            gOrgId = gUserOrgID;
                        }
                        //只有查看自己所属组织的权限
                        else
                        {
                            MemberUser oMemberUser = (from n in dbEntity.MemberUsers where (n.Deleted == false && n.Gid == userID) select n).Single();
                            Guid gUserOrgID = oMemberUser.OrgID;
                            MemberOrganization oUserMemberOrganization = (from s in dbEntity.MemberOrganizations.Include("FullName") where (s.Deleted == false && s.Gid == gUserOrgID) select s).Single();
                            SelectListItem Sitem = new SelectListItem();
                            Sitem.Text = oUserMemberOrganization.FullName.GetResource(CurrentSession.Culture);
                            Sitem.Value = gUserOrgID.ToString();
                            Sitem.Selected = true;
                            list.Add(Sitem);

                            ViewBag.list = list;

                            gOrgId = gUserOrgID;
                        }
                    }
                }
            }
            return View();
        }
        /// <summary>
        /// 获得组织ID
        /// </summary>
        /// <param name="id">组织ID</param>
        public void ArtPosition(Guid id)
        {
           gOrgId = id;
        }
        /// <summary>
        /// 所选择组织下的所有广告位列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ArtPositionList(SearchModel searchModel)
        {
            IQueryable<MallArtPosition> oMallArtPosition = (from o in dbEntity.MallArtPositions where (o.Deleted == false && o.OrgID == gOrgId) select o).AsQueryable();
            GridColumnModelList<MallArtPosition> columns = new GridColumnModelList<MallArtPosition>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p=>p.Code).SetCaption(@LiveAzure.Resource.Model.Mall.MallArtPosition.Code);
            columns.Add(p=>p.Name.Matter).SetCaption(@LiveAzure.Resource.Model.Mall.MallArtPosition.Name);
            columns.Add(p=>p.Width).SetCaption(@LiveAzure.Resource.Model.Mall.MallArtPosition.Width);
            columns.Add(p => p.Height).SetCaption(@LiveAzure.Resource.Model.Mall.MallArtPosition.Height);
            columns.Add(p => p.Matter).SetCaption(@LiveAzure.Resource.Model.Mall.MallArtPosition.Matter);
            GridData gridData = oMallArtPosition.ToGridData(searchModel,columns);
            return Json(gridData,JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 添加或者编辑广告位
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ArtPositionAddOrEdit(Guid? id)
        {
            MallArtPosition oMallArtPosition = new MallArtPosition();
            //添加新的广告位置
            if (id == null)
            {
                oMallArtPosition.Name = NewResource(ModelEnum.ResourceType.STRING,gOrgId);
                ViewBag.ShowList = SelectEnumList(oMallArtPosition.Show);
            }
            //编辑原有广告位置
            else
            {
                oMallArtPosition = (from o in dbEntity.MallArtPositions where(o.Gid==id&&o.Deleted ==false)select o).SingleOrDefault();
                if (oMallArtPosition != null)
                {
                    oMallArtPosition.Name = RefreshResource(ModelEnum.ResourceType.STRING, oMallArtPosition.Name, gOrgId);
                    ViewBag.ShowList = SelectEnumList(oMallArtPosition.Show);
                }
            }
            return View("ArtPositionAddOrEdit",oMallArtPosition);
        }
        /// <summary>
        /// 保存添加或编辑的广告位
        /// </summary>
        /// <param name="mallArtPosition"></param>
        /// <returns></returns>
        public string ArtPositionSave(MallArtPosition mallArtPosition)
        {
            Guid gId = mallArtPosition.Gid;
            //保存新添加的广告位
            if (gId.Equals(Guid.Empty) || gId.Equals(null))
            {
                string strCode = mallArtPosition.Code;
                bool flag = false;
                var ArtPositionList = (from o in dbEntity.MallArtPositions.Include("Name") where (o.OrgID == gOrgId) select o).ToList();
                foreach (var item in ArtPositionList)
                {
                    if (item.Code == strCode && item.Deleted == false)
                    {
                        flag = true;
                        return "error";
                    }
                    else if (item.Code == strCode && item.Deleted == true)
                    {
                        item.Deleted = false;
                        item.Name.SetResource(ModelEnum.ResourceType.STRING, mallArtPosition.Name);
                        item.OrgID = gOrgId;
                        item.Code = mallArtPosition.Code;
                        item.Width = mallArtPosition.Width;
                        item.Height = mallArtPosition.Height;
                        item.Matter = mallArtPosition.Matter;
                        item.Show = mallArtPosition.Show;
                        dbEntity.SaveChanges();
                        flag = true;
                        return "success";
                    }
                }
                if (flag == false)
                {
                    MallArtPosition oMallArtPosition = new MallArtPosition
                    {
                        Name = new GeneralResource(ModelEnum.ResourceType.STRING, mallArtPosition.Name)
                    };
                    oMallArtPosition.OrgID = gOrgId;
                    oMallArtPosition.Show = mallArtPosition.Show;
                    oMallArtPosition.Code = mallArtPosition.Code;
                    oMallArtPosition.Width = mallArtPosition.Width;
                    oMallArtPosition.Height = mallArtPosition.Height;
                    oMallArtPosition.Matter = mallArtPosition.Matter;
                    dbEntity.MallArtPositions.Add(oMallArtPosition);
                    dbEntity.SaveChanges();
                }
            }
            //保存编辑后的广告位
            else
            {
                MallArtPosition oMallArtPosition = (from o in dbEntity.MallArtPositions.Include("Name") where (o.Deleted == false && o.Gid == gId) select o).SingleOrDefault();
                if (oMallArtPosition != null)
                {
                    oMallArtPosition.Name.SetResource(ModelEnum.ResourceType.STRING, mallArtPosition.Name);
                    oMallArtPosition.OrgID = mallArtPosition.OrgID;
                    oMallArtPosition.Code = mallArtPosition.Code;
                    oMallArtPosition.Width = mallArtPosition.Width;
                    oMallArtPosition.Height = mallArtPosition.Height;
                    oMallArtPosition.Matter = mallArtPosition.Matter;
                    oMallArtPosition.Show = mallArtPosition.Show;
                    dbEntity.SaveChanges();

                }
            }

            return "success";
        }
        /// <summary>
        /// 删除选中的广告位
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ArtPositionDelete(Guid id)
        {
            MallArtPosition oMallArtPosition = (from o in dbEntity.MallArtPositions where (o.Deleted == false && o.Gid == id) select o).SingleOrDefault();
            if (oMallArtPosition != null)
            {
                oMallArtPosition.Deleted = true;
                dbEntity.SaveChanges();
            }
            return RedirectToAction("Index");
        }



        /// <summary>
        /// 文章内容定义
        /// </summary>
        /// <returns></returns>
        public ActionResult ArticleIndex()
        {
            //生成组织下拉框
            Guid? userID = CurrentSession.UserID;
            if (userID != null)
            {
                //该用户为超级管理员
                if (CurrentSession.IsAdmin == true)
                {
                    var oMemberOrganization = (from o in dbEntity.MemberOrganizations where (o.Deleted == false && o.Otype == 0) select o).ToList();
                    MemberUser oMemberUser = (from i in dbEntity.MemberUsers where (i.Gid == userID && i.Deleted == false) select i).SingleOrDefault();
                    Guid gUserOrg = oMemberUser.OrgID;
                    List<SelectListItem> list = new List<SelectListItem>();
                    foreach (var item in oMemberOrganization)
                    {
                        SelectListItem sitem = new SelectListItem
                        {
                            Text = item.FullName.GetResource(CurrentSession.Culture),
                            Value = item.Gid.ToString()
                        };
                        if (item.Gid == gUserOrg) sitem.Selected = true;

                        list.Add(sitem);
                    }
                    ViewBag.list = list;

                    gOrgId = gUserOrg;
                }
                //该用户为普通用户
                else
                {
                    // 该用户有权限管理的组织，生成下拉框
                    MemberPrivilege oMemberPrivilege = (from o in dbEntity.MemberPrivileges where (o.Deleted == false && o.Ptype == 2) select o).SingleOrDefault();
                    List<SelectListItem> list = new List<SelectListItem>();
                    //有权查看多个组织
                    if (oMemberPrivilege != null)
                    {
                        Guid gPrivID = oMemberPrivilege.Gid;
                        var oMemberPrivItem = (from i in dbEntity.MemberPrivItems where (i.Deleted == false && i.PrivID == gPrivID) select i).ToList();
                        foreach (var PrivItem in oMemberPrivItem)
                        {
                            Guid gOrgID = (Guid)PrivItem.RefID;
                            MemberOrganization oMemberOrganization = (from m in dbEntity.MemberOrganizations.Include("FullName") where (m.Deleted == false && m.Gid == gOrgID) select m).Single();
                            SelectListItem item = new SelectListItem();
                            item.Text = oMemberOrganization.FullName.GetResource(CurrentSession.Culture);
                            item.Value = gOrgID.ToString();

                            list.Add(item);
                        }
                        //用户的默认组织不包含在MemberPrivItem表中，单独添加，并且默认为选中
                        MemberUser oMemberUser = (from n in dbEntity.MemberUsers where (n.Deleted == false && n.Gid == userID) select n).Single();
                        Guid gUserOrgID = oMemberUser.OrgID;
                        MemberOrganization oUserMemberOrganization = (from s in dbEntity.MemberOrganizations.Include("FullName") where (s.Deleted == false && s.Gid == gUserOrgID) select s).Single();
                        SelectListItem Sitem = new SelectListItem();
                        Sitem.Text = oUserMemberOrganization.FullName.GetResource(CurrentSession.Culture);
                        Sitem.Value = gUserOrgID.ToString();
                        Sitem.Selected = true;
                        list.Add(Sitem);

                        ViewBag.list = list;

                        gOrgId = gUserOrgID;
                    }
                    //只有查看自己本组织的权限
                    else
                    {
                        MemberUser oMemberUser = (from n in dbEntity.MemberUsers where (n.Deleted == false && n.Gid == userID) select n).Single();
                        Guid gUserOrgID = oMemberUser.OrgID;
                        MemberOrganization oUserMemberOrganization = (from s in dbEntity.MemberOrganizations.Include("FullName") where (s.Deleted == false && s.Gid == gUserOrgID) select s).Single();
                        SelectListItem Sitem = new SelectListItem();
                        Sitem.Text = oUserMemberOrganization.FullName.GetResource(CurrentSession.Culture);
                        Sitem.Value = gUserOrgID.ToString();
                        Sitem.Selected = true;
                        list.Add(Sitem);

                        ViewBag.list = list;

                        gOrgId = gUserOrgID;
                    }
                }
            }
            return View();
        }
        /// <summary>
        /// 分类树结构
        /// </summary>
        /// <returns></returns>
        public string CategoryTreeLoad()
        {
            //私有分类表中读出文章分类
            var oPrivateCategory =(from o in dbEntity.GeneralPrivateCategorys.Include("Name").Include("ChildItems") where(o.OrgID==gOrgId && o.Ctype==7&&o.Parent==null &&o.Deleted ==false)select o).ToList();
            List<LiveTreeNode> list = new List<LiveTreeNode>();
            foreach (var item in oPrivateCategory)
            {
                LiveTreeNode node = new LiveTreeNode();
                node.id = item.Gid.ToString();
                node.name = item.Name.GetResource(CurrentSession.Culture);
                if (item.ChildItems.Count > 0) node.isParent = true;
                else node.isParent = false;
                node.nodes = new List<LiveTreeNode>();
                list.Add(node);
            }
            string strTreeJson = CreateTree(list);
            return strTreeJson;
        }
        /// <summary>
        /// 分类树展开
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string CategoryTreeExpand(Guid id)
        {
            List<LiveTreeNode> list = new List<LiveTreeNode>();
            //根节点展开
            if (id.Equals(Guid.Empty))
            {
                var oPrivateCategory = (from o in dbEntity.GeneralPrivateCategorys.Include("Name").Include("ChildItems") where (o.OrgID == gOrgId && o.Ctype == 7 && o.Parent == null && o.Deleted == false) select o).ToList();

                foreach (var item in oPrivateCategory)
                {
                    LiveTreeNode node = new LiveTreeNode();
                    node.id = item.Gid.ToString();
                    node.name = item.Name.GetResource(CurrentSession.Culture);
                    if (item.ChildItems.Count > 0) node.isParent = true;
                    else node.isParent = false;
                    node.nodes = new List<LiveTreeNode>();
                    list.Add(node);
                }

            }
            else
            {
                var oPrivateCategory = (from o in dbEntity.GeneralPrivateCategorys.Include("Name").Include("ChildItems") where (o.OrgID == gOrgId && o.Ctype == 7 && o.aParent == id && o.Deleted == false) select o).ToList();
                foreach (var item in oPrivateCategory)
                {
                    LiveTreeNode node = new LiveTreeNode();
                    node.id = item.Gid.ToString();
                    node.name = item.Name.GetResource(CurrentSession.Culture);
                    if (item.ChildItems.Count > 0) node.isParent = true;
                    else node.isParent = false;
                    node.nodes = new List<LiveTreeNode>();
                    list.Add(node);
                }
            }

            return list.ToJsonString();
        }
        /// <summary>
        /// 某一分类下的文章
        /// </summary>
        /// <param name="id">分类ID</param>
        /// <returns></returns>
        public ActionResult ArticleList(Guid id)
        {
            gCategoryId = id;
            return View();
        }
        /// <summary>
        /// 文章列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ArticleGridList(SearchModel searchModel)
        {
            IQueryable<MallArticle> oMallArticles = (from o in dbEntity.MallArticles where (o.Deleted == false && o.Acategory == gCategoryId && o.OrgID == gOrgId) select o).AsQueryable();
            int n =oMallArticles.ToList().Count;
            GridColumnModelList<MallArticle> ArticleColumns = new GridColumnModelList<MallArticle>();
            ArticleColumns.Add(cs => cs.Gid).SetAsPrimaryKey();
            ArticleColumns.Add(cs => cs.Code).SetCaption(@LiveAzure.Resource.Model.Mall.MallArticle.Code);
            ArticleColumns.Add(cs => cs.ArticleType.Name.Matter).SetCaption(@LiveAzure.Resource.Model.Mall.MallArticle.Atype);
            ArticleColumns.Add(cs => cs.UserName).SetCaption(@LiveAzure.Resource.Model.Mall.MallArticle.UserID);
            ArticleColumns.Add(cs => cs.Title.Matter).SetCaption(@LiveAzure.Resource.Model.Mall.MallArticle.Title);
            //foreach (var item in oMallArticles)
            //{
            //    MallArticle oMallArticle = item.Parent;
            //    if (oMallArticle != null)
            //    {
            //        ArticleColumns.Add(cs => cs.Parent.Code);
            //    }
            //    else ArticleColumns.Add(null);
            //}
            
            GridData gridData = oMallArticles.ToGridData(searchModel,ArticleColumns);
            return Json(gridData,JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 添加或者编辑文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult ArticleAddOrEdit(Guid? id)
        {
            //MallArticle oMallArticle = new MallArticle();
            
            List<SelectListItem> list = new List<SelectListItem>();
            //添加新的Article
            if (id == null)
            {
                var oPrivateCategory = (from o in dbEntity.GeneralPrivateCategorys where (o.Deleted == false && o.Ctype == 6 && o.OrgID == gOrgId) select o).ToList();
                
                MallArticle oMallArticle = new MallArticle { Matter=NewLargeObject(gOrgId) };
                oMallArticle.Title = NewResource(ModelEnum.ResourceType.STRING,gOrgId);
                //文章类型下拉列表
                foreach (var item in oPrivateCategory)
                {
                    SelectListItem sitem = new SelectListItem();
                    sitem.Text = item.Name.GetResource(CurrentSession.Culture);
                    sitem.Value = item.Gid.ToString();

                    list.Add(sitem);
                }
                ViewBag.list = list;

                List<SelectListItem> AstatusList = GetSelectList(oMallArticle.ArticleStatusList);
                ViewBag.AstatusList = AstatusList;
                return View(oMallArticle);
            }
            //编辑已有的Article
            else
            {
                MallArticle oMallArticle = (from o in dbEntity.MallArticles where (o.Deleted == false && o.Gid == id) select o).SingleOrDefault();
                oMallArticle.Matter = RefreshLargeObject(oMallArticle.Matter,gOrgId);
                oMallArticle.Title = RefreshResource(ModelEnum.ResourceType.STRING,oMallArticle.Title);
                if (oMallArticle != null)
                {
                    var oPrivateCategory = (from o in dbEntity.GeneralPrivateCategorys where (o.Deleted == false && o.Ctype == 6 && o.OrgID == gOrgId) select o).ToList();

                    foreach (var item in oPrivateCategory)
                    {
                        SelectListItem sitem = new SelectListItem();
                        sitem.Text = item.Name.GetResource(CurrentSession.Culture);
                        sitem.Value = item.Gid.ToString();
                        if (oMallArticle.Atype == item.Gid) sitem.Selected = true;
                        list.Add(sitem);
                    }
                    ViewBag.list = list;
                }
                List<SelectListItem> AstatusList = GetSelectList(oMallArticle.ArticleStatusList);
                ViewBag.AstatusList = AstatusList;
                return View(oMallArticle);
            }
            
        }
        /// <summary>
        /// 对某一文章跟帖
        /// </summary>
        /// <param name="id">父文章的ID</param>
        /// <returns></returns>
        public ActionResult ArticleReplay(Guid id)
        {
            //文章类型下拉框
            var oPrivateCategory = (from o in dbEntity.GeneralPrivateCategorys where (o.Deleted == false && o.Ctype == 6 && o.OrgID == gOrgId) select o).ToList();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in oPrivateCategory)
            {
                SelectListItem sitem = new SelectListItem();
                sitem.Text = item.Name.GetResource(CurrentSession.Culture);
                sitem.Value = item.Gid.ToString();

                list.Add(sitem);
            }
            ViewBag.list = list;

            MallArticle oMallArticle = new MallArticle();
            oMallArticle.Matter = NewLargeObject(gOrgId);
            oMallArticle.Title = NewResource(ModelEnum.ResourceType.STRING,gOrgId);
            MallArticle oParentMallArticle = (from o in dbEntity.MallArticles where (o.Deleted == false && o.Gid == id) select o).SingleOrDefault();
            if (oParentMallArticle != null)
            {
                oMallArticle.aParent = id;
                oMallArticle.Parent = oParentMallArticle;
                oMallArticle.ProdID = oParentMallArticle.ProdID;
                oMallArticle.Product = oParentMallArticle.Product;
            }
            else
            {
                oMallArticle.aParent = null;
                oMallArticle.ProdID = null;
            }
            List<SelectListItem> AstatusList = GetSelectList(oMallArticle.ArticleStatusList);
            ViewBag.AstatusList = AstatusList;
            return View("ArticleReplay", oMallArticle);
        }
        /// <summary>
        /// 保存新增或者编辑后的文章
        /// </summary>
        /// <param name="mallArticle"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public string ArticleSave(MallArticle mallArticle)
        {
            Guid gArticleID = mallArticle.Gid;
            MallArticle oMallArticle = new MallArticle();
            //保存新增加的文章
            if (gArticleID.Equals(Guid.Empty))
            {
                string strCode = mallArticle.Code;
                var oArticleCode = (from o in dbEntity.MallArticles where (o.Deleted == false && o.OrgID == gOrgId && o.Acategory == gCategoryId) select o).ToList();
                bool flag = false;
                //判断Code是否唯一
                foreach (var item in oArticleCode)
                {
                    //存在相同Code的文章时，不进行保存，提示用户
                    if (item.Code == strCode && item.Deleted == false)
                    {
                        flag = true;
                        return "error";
                    }
                    //存在相同Code的文章，但文章已删除，恢复
                    else if (item.Code == strCode && item.Deleted == true)
                    {
                        item.Deleted = false;
                        item.OrgID = gOrgId;
                        item.Acategory = gCategoryId;
                        item.Code = mallArticle.Code;
                        item.Atype = mallArticle.Atype;
                        item.Astatus = mallArticle.Astatus;
                        string strUserName = mallArticle.UserName;
                        //填写的用户名不为空
                        if (strUserName != null)
                        {

                            Guid gUserID = (from o in dbEntity.MemberUsers where (o.Deleted == false && o.OrgID == gOrgId && o.LoginName == strUserName) select o.Gid).SingleOrDefault();
                            //用户登陆名错误，不存在为此用户名的用户
                            if (gUserID == null)
                            {
                                return "UserName Error";
                            }
                            else
                            {
                                item.UserID = gUserID;
                                item.UserName = mallArticle.UserName;
                            }
                        }
                        //用户名为空时，保存为游客
                        else
                        {
                            item.UserName = "游客";
                            item.UserID = null;
                        }
                        //用户输入的产品代码
                        string strProductCode = mallArticle.Product.Code;
                        if (strProductCode != null)
                        {
                            //有组织和代码唯一确定产品（组织为前面选择传入值？？？）
                            ProductInformation oProductInfo = (from p in dbEntity.ProductInformations where (p.OrgID == gOrgId && p.Code == strProductCode && p.Deleted == false) select p).SingleOrDefault();
                            //代码输入正确，得到产品的具体信息
                            if (oProductInfo != null)
                            {
                                oMallArticle.ProdID = oProductInfo.Gid;
                                oMallArticle.Product = oProductInfo;
                            }
                            //代码输入不正确，没有符合条件的产品,提示错误
                            else
                            {
                                return "ProductCode Error";
                            }
                        }
                        //用户输入的父文章的代码
                        string strParentCode = mallArticle.Parent.Code;
                        if (strProductCode != null)
                        {
                            //有组织和代码唯一确定文章
                            MallArticle oParentArt = (from o in dbEntity.MallArticles where (o.OrgID == gOrgId && o.Code == strParentCode && o.Deleted == false) select o).SingleOrDefault();
                            //文章代码输入正确，进行保存
                            if (oParentArt != null)
                            {
                                oMallArticle.aParent = oParentArt.Gid;
                                oMallArticle.Parent = oParentArt;
                            }
                            //文章代码不正确，提示错误
                            else
                            {
                                return "ParentCode Error";
                            }
                        }
                        oMallArticle.Title = new GeneralResource(ModelEnum.ResourceType.STRING,mallArticle.Title);
                        oMallArticle.Matter = new GeneralLargeObject(mallArticle.Matter);
                        item.Remark = mallArticle.Remark;

                        dbEntity.SaveChanges();
                        flag = true;
                    }
                }
                //不存在相同代码时
                if (flag == false)
                {
                    oMallArticle.OrgID = gOrgId;
                    oMallArticle.Acategory = gCategoryId;
                    oMallArticle.Code = mallArticle.Code;
                    oMallArticle.Atype = mallArticle.Atype;
                    oMallArticle.Astatus = mallArticle.Astatus;

                    string strUserName = mallArticle.UserName;
                    if (strUserName != null)
                    {

                        Guid gUserID = (from o in dbEntity.MemberUsers where (o.Deleted == false && o.OrgID == gOrgId && o.LoginName == strUserName) select o.Gid).SingleOrDefault();
                        //用户登陆名输入错误，提示错误
                        if (gUserID == null)
                        {
                            return "UserName Error";
                        }
                        else
                        {
                            oMallArticle.UserID = gUserID;
                            oMallArticle.UserName = mallArticle.UserName;
                        }
                    }
                    //用户登陆名为空
                    else
                    {
                        oMallArticle.UserName = "游客";
                        oMallArticle.UserID = null;
                    }

                    //用户输入的产品代码
                    string strProductCode = mallArticle.Product.Code;
                    if (strProductCode != null)
                    {
                        //有组织和代码唯一确定产品（组织为前面选择传入值？？？）
                        ProductInformation oProductInfo = (from p in dbEntity.ProductInformations where (p.OrgID == gOrgId && p.Code == strProductCode && p.Deleted == false) select p).SingleOrDefault();
                        //代码输入正确，得到产品的具体信息
                        if (oProductInfo != null)
                        {
                            oMallArticle.ProdID = oProductInfo.Gid;
                            oMallArticle.Product = oProductInfo;
                        }
                        //代码输入不正确，没有符合条件的产品,提示错误
                        else
                        {
                            return "ProductCode Error";
                        }
                    }
                    //用户输入的父文章的代码
                    string strParentCode = mallArticle.Parent.Code;
                    if (strParentCode != null)
                    {
                        //有组织和代码唯一确定文章
                        MallArticle oParentArt = (from o in dbEntity.MallArticles where (o.OrgID == gOrgId && o.Code == strParentCode && o.Deleted == false) select o).SingleOrDefault();
                        //文章代码输入正确，进行保存
                        if (oParentArt != null)
                        {
                            oMallArticle.aParent = oParentArt.Gid;
                            oMallArticle.Parent = oParentArt;
                        }
                        //文章代码不正确，提示错误
                        else
                        {
                            return "ParentCode Error";
                        }
                    }
                    oMallArticle.Title = new GeneralResource(ModelEnum.ResourceType.STRING,mallArticle.Title);
                    oMallArticle.Matter =new GeneralLargeObject( mallArticle.Matter);
                    oMallArticle.Remark = mallArticle.Remark;
                    dbEntity.MallArticles.Add(oMallArticle);
                    dbEntity.SaveChanges();

                    flag = false;

                }
            }
            //保存编辑的文章
            else
            {
                oMallArticle = (from o in dbEntity.MallArticles where (o.Deleted == false && o.Gid == gArticleID) select o).SingleOrDefault();
                if (oMallArticle != null)
                {
                    oMallArticle.Code = mallArticle.Code;
                    oMallArticle.Atype = mallArticle.Atype;
                    oMallArticle.OrgID = gOrgId;
                    oMallArticle.Acategory = mallArticle.Acategory;
                    oMallArticle.Astatus = mallArticle.Astatus;
                    string strUserName = mallArticle.UserName;
                    if (strUserName != "游客")
                    {
                        oMallArticle.UserName = mallArticle.UserName;
                        Guid gUserID = (from o in dbEntity.MemberUsers where (o.Deleted == false && o.OrgID == gOrgId && o.LoginName == strUserName) select o.Gid).SingleOrDefault();
                        oMallArticle.UserID = gUserID;
                    }
                    else
                    {
                        oMallArticle.UserName = "游客";
                        oMallArticle.UserID = null;
                    }
                    //用户输入的产品代码
                    if (mallArticle.Product != null)
                    {
                        string strProductCode = mallArticle.Product.Code;
                        if (strProductCode != null)
                        {
                            //有组织和代码唯一确定产品（组织为前面选择传入值？？？）
                            ProductInformation oProductInfo = (from p in dbEntity.ProductInformations where (p.OrgID == gOrgId && p.Code == strProductCode && p.Deleted == false) select p).SingleOrDefault();
                            //代码输入正确，得到产品的具体信息
                            if (oProductInfo != null)
                            {
                                oMallArticle.ProdID = oProductInfo.Gid;
                                oMallArticle.Product = oProductInfo;
                            }
                            //代码输入不正确，没有符合条件的产品,提示错误
                            else
                            {
                                return "ProductCode Error";
                            }
                        }
                    }
                    //用户输入的父文章的代码
                    if (mallArticle.Parent != null)
                    {
                        string strParentCode = mallArticle.Parent.Code;
                        if (strParentCode != null)
                        {
                            //有组织和代码唯一确定文章
                            MallArticle oParentArt = (from o in dbEntity.MallArticles where (o.OrgID == gOrgId && o.Code == strParentCode && o.Deleted == false) select o).SingleOrDefault();
                            //文章代码输入正确，进行保存
                            if (oParentArt != null)
                            {
                                oMallArticle.aParent = oParentArt.Gid;
                                oMallArticle.Parent = oParentArt;
                            }
                            //文章代码不正确，提示错误
                            else
                            {
                                return "ParentCode Error";
                            }
                        }
                    }
                    oMallArticle.Title = mallArticle.Title;
                    oMallArticle.Matter.SetLargeObject( mallArticle.Matter);
                    oMallArticle.Remark = mallArticle.Remark;
                    dbEntity.SaveChanges();

                }
            }
            return "success";
        }
        /// <summary>
        /// 删除选中的文章
        /// </summary>
        /// <param name="id">选中文章的ID</param>
        /// <returns></returns>
        public ActionResult ArticleDelete(Guid id)
        {
            MallArticle oMallArticle = (from o in dbEntity.MallArticles where (o.Deleted == false && o.Gid == id) select o).SingleOrDefault();
            if (oMallArticle != null)
            {
                oMallArticle.Deleted = true;
                dbEntity.SaveChanges();
            }
            return RedirectToAction("ArticleIndex");
        }



        /// <summary>
        /// 发布文章
        /// </summary>
        /// <returns></returns>
        public ActionResult ArticlePublish()
        {
            return View();
        }
        public ActionResult ArticlePublishList(SearchModel searchModel)
        {
            IQueryable<MallArtPublish> oMallArticles = (from o in dbEntity.MallArtPublishes where (o.Deleted == false) select o).AsQueryable();
            CultureInfo culture = new CultureInfo(CurrentSession.Culture);
            GridColumnModelList<MallArtPublish> ArtPublishColumns = new GridColumnModelList<MallArtPublish>();
            ArtPublishColumns.Add(cs => cs.Gid).SetAsPrimaryKey();
            ArtPublishColumns.Add(cs => cs.Article.Code).SetCaption(@LiveAzure.Resource.Model.Mall.MallArtPublish.ArtID);
            ArtPublishColumns.Add(cs => cs.Channel.Code).SetCaption(@LiveAzure.Resource.Model.Mall.MallArtPublish.ChlID);
            ArtPublishColumns.Add(cs => cs.Position.Code).SetCaption(@LiveAzure.Resource.Model.Mall.MallArtPublish.PosID);
            ArtPublishColumns.Add(cs => cs.Sorting).SetCaption(@LiveAzure.Resource.Model.Mall.MallArtPublish.Sorting);
            ArtPublishColumns.Add(cs => cs.Show).SetCaption(@LiveAzure.Resource.Model.Mall.MallArtPublish.Show);
            ArtPublishColumns.Add(cs => cs.TotalRank).SetCaption(@LiveAzure.Resource.Model.Mall.MallArtPublish.TotalRank);
            ArtPublishColumns.Add(cs => cs.MatterRank).SetCaption(@LiveAzure.Resource.Model.Mall.MallArtPublish.MatterRank);
            ArtPublishColumns.Add(cs => cs.LayoutRank).SetCaption(@LiveAzure.Resource.Model.Mall.MallArtPublish.LayoutRank);
            ArtPublishColumns.Add(cs => cs.ComfortRank).SetCaption(@LiveAzure.Resource.Model.Mall.MallArtPublish.ComfortRank);
            ArtPublishColumns.Add(cs => cs.StartTime == null ? "" : cs.StartTime.Value.ToString(culture.DateTimeFormat.ShortDatePattern)).SetCaption(@LiveAzure.Resource.Model.Mall.MallArtPublish.StartTime).SetName("StartTime");
            ArtPublishColumns.Add(cs => cs.EndTime == null ? "" : cs.EndTime.Value.ToString(culture.DateTimeFormat.ShortDatePattern)).SetCaption(@LiveAzure.Resource.Model.Mall.MallArtPublish.EndTime).SetName("EndTime");

            GridData gridData = oMallArticles.ToGridData(searchModel, ArtPublishColumns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 新增或者编辑文章发布
        /// </summary>
        /// <param name="id">要编辑的文章发布ID</param>
        /// <returns></returns>
        public ActionResult ArtPublishAddOrEdit(Guid? id)
        {
            MallArtPublish oMallArtPublish = new MallArtPublish();
            //新建文章发布
            if (id.Equals(Guid.Empty) || id == null)
            {
                //若为超级管理员则有权限读取所有文章、渠道、文章位子
                if (CurrentSession.IsAdmin == true)
                {
                    //文章选择下拉框
                    var oArticleList = (from o in dbEntity.MallArticles where (o.Deleted == false) select o).ToList();
                    List<SelectListItem> ArticleList = new List<SelectListItem>();
                    foreach (var item in oArticleList)
                    {
                        SelectListItem aitem = new SelectListItem();
                        aitem.Text = item.Code;
                        aitem.Value = item.Gid.ToString();

                        ArticleList.Add(aitem);
                    }
                    ViewBag.ArticleList = ArticleList;
                    //渠道选择下拉框
                    var oChannelList = (from m in dbEntity.MemberChannels where (m.Deleted == false) select m).ToList();
                    List<SelectListItem> ChannelList = new List<SelectListItem>();
                    foreach (var item in oChannelList)
                    {
                        SelectListItem citem = new SelectListItem();
                        citem.Text = item.Code;
                        citem.Value = item.Gid.ToString();

                        ChannelList.Add(citem);
                    }
                    ViewBag.ChannelList = ChannelList;
                    //文章位置选择下拉框
                    var oPositionList = (from n in dbEntity.MallArtPositions where (n.Deleted == false) select n).ToList();
                    List<SelectListItem> PositionList = new List<SelectListItem>();
                    foreach (var item in oPositionList)
                    {
                        SelectListItem pitem = new SelectListItem();
                        pitem.Text = item.Code;
                        pitem.Value = item.Gid.ToString();

                        PositionList.Add(pitem);
                    }
                    ViewBag.PositionList = PositionList;
                    //是否显示
                    List<SelectListItem> ShowList = SelectEnumList(oMallArtPublish.Show);

                    ViewData["StartTime"] = "";
                    ViewData["EndTime"] = "";
                }
                //若为普通用户
                else
                {
                    Guid gUserID = (Guid)CurrentSession.UserID;
                    //如果用户ID不为空，首先读出该用户有权限查看的组织，其次读出所有有权限查看组织下的文章和文章位置
                    if (!gUserID.Equals(Guid.Empty))
                    {
                        MemberPrivilege oMemberPrivilege = (from m in dbEntity.MemberPrivileges where (m.Deleted == false && m.UserID == gUserID && m.Ptype == 2) select m).SingleOrDefault();
                        //该用户只有权限查看自己所属组织
                        if (oMemberPrivilege == null)
                        {
                            MemberUser oMemberUser = (from o in dbEntity.MemberUsers where (o.Gid == gUserID && o.Deleted == false) select o).SingleOrDefault();
                            if (oMemberUser != null)
                            {
                                Guid gOrgID = oMemberUser.OrgID;
                                //文章选择下拉框
                                var oArticle = (from p in dbEntity.MallArticles where (p.OrgID == gOrgID && p.Deleted == false) select p).ToList();
                                List<SelectListItem> ArticleList = new List<SelectListItem>();
                                foreach (var item in oArticle)
                                {
                                    SelectListItem aitem = new SelectListItem();
                                    aitem.Text = item.Code;
                                    aitem.Value = item.Gid.ToString();

                                    ArticleList.Add(aitem);
                                }
                                ViewBag.ArticleList = ArticleList;

                                //文章位置选择下拉框
                                var oPosition = (from q in dbEntity.MallArtPositions where (q.OrgID == gOrgID && q.Deleted == false) select q).ToList();
                                List<SelectListItem> PositionList = new List<SelectListItem>();
                                foreach (var item in oPosition)
                                {
                                    SelectListItem pitem = new SelectListItem();
                                    pitem.Text = item.Code;
                                    pitem.Value = item.Gid.ToString();

                                    PositionList.Add(pitem);
                                }
                                ViewBag.PositionList = PositionList;
                            }
                        }
                        //该用户能够查看多个组织
                        else
                        {
                            Guid PrivId = oMemberPrivilege.Gid;
                            List<SelectListItem> ArticleList = new List<SelectListItem>();
                            List<SelectListItem> PositionList = new List<SelectListItem>();
                            //授权组织
                            var oMemPrivItem = (from o in dbEntity.MemberPrivItems where (o.PrivID == PrivId && o.Deleted == false) select o).ToList();
                            //读取每个组织下的文章和文章位置
                            foreach (var item in oMemPrivItem)
                            {
                                Guid gOrg = (Guid)item.RefID;
                                var oArticle = (from p in dbEntity.MallArticles where (p.OrgID == gOrg && p.Deleted == false) select p).ToList();

                                foreach (var aitem in oArticle)
                                {
                                    SelectListItem saitem = new SelectListItem();
                                    saitem.Text = aitem.Code;
                                    saitem.Value = aitem.Gid.ToString();

                                    ArticleList.Add(saitem);
                                }

                                var oPosition = (from q in dbEntity.MallArtPositions where (q.OrgID == gOrg && q.Deleted == false) select q).ToList();
                                foreach (var pitem in oPosition)
                                {
                                    SelectListItem spitem = new SelectListItem();
                                    spitem.Text = pitem.Code;
                                    spitem.Value = pitem.Gid.ToString();

                                    PositionList.Add(spitem);
                                }
                            }
                            //该用户默认的组织
                            MemberUser oMemberUser = (from o in dbEntity.MemberUsers where (o.Gid == gUserID && o.Deleted == false) select o).SingleOrDefault();
                            if (oMemberUser != null)
                            {
                                Guid gOrgID = oMemberUser.OrgID;
                                //文章选择下拉框
                                var oArticle = (from p in dbEntity.MallArticles where (p.OrgID == gOrgID && p.Deleted == false) select p).ToList();
                                foreach (var item in oArticle)
                                {
                                    SelectListItem aitem = new SelectListItem();
                                    aitem.Text = item.Code;
                                    aitem.Value = item.Gid.ToString();

                                    ArticleList.Add(aitem);
                                }
                                ViewBag.ArticleList = ArticleList;

                                //文章位置选择下拉框
                                var oPosition = (from q in dbEntity.MallArtPositions where (q.OrgID == gOrgID && q.Deleted == false) select q).ToList();
                                foreach (var item in oPosition)
                                {
                                    SelectListItem pitem = new SelectListItem();
                                    pitem.Text = item.Code;
                                    pitem.Value = item.Gid.ToString();

                                    PositionList.Add(pitem);
                                }
                                ViewBag.PositionList = PositionList;
                            }
                        }
                    }
                    //该用户有权查看的渠道
                    MemberPrivilege oMemPrivilege = (from n in dbEntity.MemberPrivileges where (n.Ptype == 3 && n.UserID == gUserID && n.Deleted == false) select n).SingleOrDefault();
                    List<SelectListItem> ChannelList = new List<SelectListItem>();
                    //只有权查看自己所属的渠道
                    if (oMemPrivilege == null)
                    {
                        MemberUser oMemberUser = (from o in dbEntity.MemberUsers where (o.Gid == gUserID && o.Deleted == false) select o).SingleOrDefault();
                        Guid gChannelID = oMemberUser.ChlID;
                        MemberChannel oMemberChannel = (from p in dbEntity.MemberChannels where(p.Gid==gChannelID &&p.Deleted==false)select p).SingleOrDefault();
                        if(oMemberChannel!=null)
                        {
                            SelectListItem citem = new SelectListItem();
                            citem.Text = oMemberChannel.Code;
                            citem.Value = oMemberChannel.Gid.ToString();

                            ChannelList.Add(citem);
                        }
                    }
                    else
                    {
                        Guid gPrivId = oMemPrivilege.Gid;
                        var oMemPrivItem = (from o in dbEntity.MemberPrivItems where(o.PrivID==gPrivId&&o.Deleted==false)select o).ToList();
                        foreach (var item in oMemPrivItem)
                        {
                            Guid ChannelID =(Guid)item.RefID;
                            MemberChannel oMemberChannel = (from p in dbEntity.MemberChannels where (p.Gid == ChannelID && p.Deleted == false) select p).SingleOrDefault();
                            if (oMemberChannel != null)
                            {
                                SelectListItem citem = new SelectListItem();
                                citem.Text = oMemberChannel.Code;
                                citem.Value = oMemberChannel.Gid.ToString();

                                ChannelList.Add(citem);
                            }
                        }
                        MemberUser oMemberUser = (from o in dbEntity.MemberUsers where (o.Gid == gUserID && o.Deleted == false) select o).SingleOrDefault();
                        Guid gChannelID = oMemberUser.ChlID;
                        MemberChannel oMemrChannel = (from p in dbEntity.MemberChannels where (p.Gid == gChannelID && p.Deleted == false) select p).SingleOrDefault();
                        if (oMemrChannel != null)
                        {
                            SelectListItem citem = new SelectListItem();
                            citem.Text = oMemrChannel.Code;
                            citem.Value = oMemrChannel.Gid.ToString();

                            ChannelList.Add(citem);
                        }
                    }
                    ViewBag.ChannelList = ChannelList;

                    ViewBag.ShowList = SelectEnumList(oMallArtPublish.Show);

                    ViewData["StartTime"] = "";
                    ViewData["EndTime"] = "";
                }
            }
            //编辑某一文章发布 
            else
            {
                oMallArtPublish = (from o in dbEntity.MallArtPublishes where (o.Deleted == false && o.Gid == id) select o).SingleOrDefault();
                if (oMallArtPublish != null)
                {
                    //若为超级管理员则有权限读取所有文章、渠道、文章位子
                    if (CurrentSession.IsAdmin == true)
                    {
                        //文章选择下拉框
                        var oArticleList = (from o in dbEntity.MallArticles where (o.Deleted == false) select o).ToList();
                        List<SelectListItem> ArticleList = new List<SelectListItem>();
                        foreach (var item in oArticleList)
                        {
                            SelectListItem aitem = new SelectListItem();
                            aitem.Text = item.Code;
                            aitem.Value = item.Gid.ToString();
                            if (item.Gid == oMallArtPublish.ArtID) aitem.Selected = true;

                            ArticleList.Add(aitem);
                        }
                        ViewBag.ArticleList = ArticleList;
                        //渠道选择下拉框
                        var oChannelList = (from m in dbEntity.MemberChannels where (m.Deleted == false) select m).ToList();
                        List<SelectListItem> ChannelList = new List<SelectListItem>();
                        foreach (var item in oChannelList)
                        {
                            SelectListItem citem = new SelectListItem();
                            citem.Text = item.Code;
                            citem.Value = item.Gid.ToString();
                            if (item.Gid == oMallArtPublish.ChlID) citem.Selected = true;

                            ChannelList.Add(citem);
                        }
                        ViewBag.ChannelList = ChannelList;
                        //文章位置选择下拉框
                        var oPositionList = (from n in dbEntity.MallArtPositions where (n.Deleted == false) select n).ToList();
                        List<SelectListItem> PositionList = new List<SelectListItem>();
                        foreach (var item in oPositionList)
                        {
                            SelectListItem pitem = new SelectListItem();
                            pitem.Text = item.Code;
                            pitem.Value = item.Gid.ToString();
                            if (item.Gid == oMallArtPublish.PosID) pitem.Selected = true;

                            PositionList.Add(pitem);
                        }
                        ViewBag.PositionList = PositionList;
                        //是否显示
                        List<SelectListItem> ShowList = SelectEnumList(oMallArtPublish.Show);

                        CultureInfo culture = new CultureInfo(CurrentSession.Culture);
                        ViewData["StartTime"] = oMallArtPublish.StartTime.Value.ToString(culture.DateTimeFormat.ShortDatePattern);

                        ViewData["EndTime"] = oMallArtPublish.EndTime.Value.ToString(culture.DateTimeFormat.ShortDatePattern);
                    }
                    //若为普通用户
                    else
                    {
                        Guid gUserID = (Guid)CurrentSession.UserID;
                        //如果用户ID不为空，首先读出该用户有权限查看的组织，其次读出所有有权限查看组织下的文章和文章位置
                        if (!gUserID.Equals(Guid.Empty))
                        {
                            MemberPrivilege oMemberPrivilege = (from m in dbEntity.MemberPrivileges where (m.Deleted == false && m.UserID == gUserID && m.Ptype == 2) select m).SingleOrDefault();
                            //该用户只有权限查看自己所属组织
                            if (oMemberPrivilege == null)
                            {
                                MemberUser oMemberUser = (from o in dbEntity.MemberUsers where (o.Gid == gUserID && o.Deleted == false) select o).SingleOrDefault();
                                if (oMemberUser != null)
                                {
                                    Guid gOrgID = oMemberUser.OrgID;
                                    //文章选择下拉框
                                    var oArticle = (from p in dbEntity.MallArticles where (p.OrgID == gOrgID && p.Deleted == false) select p).ToList();
                                    List<SelectListItem> ArticleList = new List<SelectListItem>();
                                    foreach (var item in oArticle)
                                    {
                                        SelectListItem aitem = new SelectListItem();
                                        aitem.Text = item.Code;
                                        aitem.Value = item.Gid.ToString();
                                        if (item.Gid == oMallArtPublish.ArtID) aitem.Selected = true;

                                        ArticleList.Add(aitem);
                                    }
                                    ViewBag.ArticleList = ArticleList;

                                    //文章位置选择下拉框
                                    var oPosition = (from q in dbEntity.MallArtPositions where (q.OrgID == gOrgID && q.Deleted == false) select q).ToList();
                                    List<SelectListItem> PositionList = new List<SelectListItem>();
                                    foreach (var item in oPosition)
                                    {
                                        SelectListItem pitem = new SelectListItem();
                                        pitem.Text = item.Code;
                                        pitem.Value = item.Gid.ToString();
                                        if (item.Gid == oMallArtPublish.PosID) pitem.Selected = true;
                                        PositionList.Add(pitem);
                                    }
                                    ViewBag.PositionList = PositionList;
                                }
                            }
                            //该用户能够查看多个组织
                            else
                            {
                                Guid PrivId = oMemberPrivilege.Gid;
                                List<SelectListItem> ArticleList = new List<SelectListItem>();
                                List<SelectListItem> PositionList = new List<SelectListItem>();
                                //授权组织
                                var oMemPrivItem = (from o in dbEntity.MemberPrivItems where (o.PrivID == PrivId && o.Deleted == false) select o).ToList();
                                //读取每个组织下的文章和文章位置
                                foreach (var item in oMemPrivItem)
                                {
                                    Guid gOrg = (Guid)item.RefID;
                                    var oArticle = (from p in dbEntity.MallArticles where (p.OrgID == gOrg && p.Deleted == false) select p).ToList();

                                    foreach (var aitem in oArticle)
                                    {
                                        SelectListItem saitem = new SelectListItem();
                                        saitem.Text = aitem.Code;
                                        saitem.Value = aitem.Gid.ToString();
                                        if (aitem.Gid == oMallArtPublish.ArtID) saitem.Selected = true;
                                        ArticleList.Add(saitem);
                                    }

                                    var oPosition = (from q in dbEntity.MallArtPositions where (q.OrgID == gOrg && q.Deleted == false) select q).ToList();
                                    foreach (var pitem in oPosition)
                                    {
                                        SelectListItem spitem = new SelectListItem();
                                        spitem.Text = pitem.Code;
                                        spitem.Value = pitem.Gid.ToString();
                                        if (pitem.Gid == oMallArtPublish.PosID) spitem.Selected = true;

                                        PositionList.Add(spitem);
                                    }
                                }
                                //该用户默认的组织
                                MemberUser oMemberUser = (from o in dbEntity.MemberUsers where (o.Gid == gUserID && o.Deleted == false) select o).SingleOrDefault();
                                if (oMemberUser != null)
                                {
                                    Guid gOrgID = oMemberUser.OrgID;
                                    //文章选择下拉框
                                    var oArticle = (from p in dbEntity.MallArticles where (p.OrgID == gOrgID && p.Deleted == false) select p).ToList();
                                    foreach (var item in oArticle)
                                    {
                                        SelectListItem aitem = new SelectListItem();
                                        aitem.Text = item.Code;
                                        aitem.Value = item.Gid.ToString();
                                        if (item.Gid == oMallArtPublish.ArtID) aitem.Selected = true;

                                        ArticleList.Add(aitem);
                                    }
                                    ViewBag.ArticleList = ArticleList;

                                    //文章位置选择下拉框
                                    var oPosition = (from q in dbEntity.MallArtPositions where (q.OrgID == gOrgID && q.Deleted == false) select q).ToList();
                                    foreach (var item in oPosition)
                                    {
                                        SelectListItem pitem = new SelectListItem();
                                        pitem.Text = item.Code;
                                        pitem.Value = item.Gid.ToString();
                                        if (item.Gid == oMallArtPublish.PosID) pitem.Selected = true;

                                        PositionList.Add(pitem);
                                    }
                                    ViewBag.PositionList = PositionList;
                                }
                            }
                        }
                        //该用户有权查看的渠道
                        MemberPrivilege oMemPrivilege = (from n in dbEntity.MemberPrivileges where (n.Ptype == 3 && n.UserID == gUserID && n.Deleted == false) select n).SingleOrDefault();
                        List<SelectListItem> ChannelList = new List<SelectListItem>();
                        //只有权查看自己所属的渠道
                        if (oMemPrivilege == null)
                        {
                            MemberUser oMemberUser = (from o in dbEntity.MemberUsers where (o.Gid == gUserID && o.Deleted == false) select o).SingleOrDefault();
                            Guid gChannelID = oMemberUser.ChlID;
                            MemberChannel oMemberChannel = (from p in dbEntity.MemberChannels where (p.Gid == gChannelID && p.Deleted == false) select p).SingleOrDefault();
                            if (oMemberChannel != null)
                            {
                                SelectListItem citem = new SelectListItem();
                                citem.Text = oMemberChannel.Code;
                                citem.Value = oMemberChannel.Gid.ToString();
                                if (oMemberChannel.Gid == oMallArtPublish.ChlID) citem.Selected = true;

                                ChannelList.Add(citem);
                            }
                        }
                        //授权查看多个渠道
                        else
                        {
                            Guid gPrivId = oMemPrivilege.Gid;
                            var oMemPrivItem = (from o in dbEntity.MemberPrivItems where (o.PrivID == gPrivId && o.Deleted == false) select o).ToList();
                            foreach (var item in oMemPrivItem)
                            {
                                Guid ChannelID = (Guid)item.RefID;
                                MemberChannel oMemberChannel = (from p in dbEntity.MemberChannels where (p.Gid == ChannelID && p.Deleted == false) select p).SingleOrDefault();
                                if (oMemberChannel != null)
                                {
                                    SelectListItem citem = new SelectListItem();
                                    citem.Text = oMemberChannel.Code;
                                    citem.Value = oMemberChannel.Gid.ToString();
                                    if (oMemberChannel.Gid == oMallArtPublish.ChlID) citem.Selected = true;

                                    ChannelList.Add(citem);
                                }
                            }
                            MemberUser oMemberUser = (from o in dbEntity.MemberUsers where (o.Gid == gUserID && o.Deleted == false) select o).SingleOrDefault();
                            Guid gChannelID = oMemberUser.ChlID;
                            MemberChannel oMemChannel = (from p in dbEntity.MemberChannels where (p.Gid == gChannelID && p.Deleted == false) select p).SingleOrDefault();
                            if (oMemChannel != null)
                            {
                                SelectListItem citem = new SelectListItem();
                                citem.Text = oMemChannel.Code;
                                citem.Value = oMemChannel.Gid.ToString();
                                if (oMemChannel.Gid == oMallArtPublish.ChlID) citem.Selected = true;

                                ChannelList.Add(citem);
                            }
                        }
                        ViewBag.ChannelList = ChannelList;

                        ViewBag.ShowList = SelectEnumList(oMallArtPublish.Show);

                        CultureInfo culture = new CultureInfo(CurrentSession.Culture);
                        ViewData["StartTime"] = oMallArtPublish.StartTime.Value.ToString(culture.DateTimeFormat.ShortDatePattern);

                        ViewData["EndTime"] = oMallArtPublish.EndTime.Value.ToString(culture.DateTimeFormat.ShortDatePattern);
                    }
                }
            }
            return View("ArtPublishAddOrEdit",oMallArtPublish);
        }
        /// <summary>
        /// 保存新增或者编辑的文章
        /// </summary>
        /// <param name="mallArtPublish"></param>
        /// <returns></returns>
        public string ArtPublishSave(MallArtPublish mallArtPublish, FormCollection formCollection)
        {
            MallArtPublish oMallArtPublish = new MallArtPublish();
            Guid gArtPublish = mallArtPublish.Gid;
            if (gArtPublish.Equals(Guid.Empty))
            {
                oMallArtPublish.ArtID = mallArtPublish.ArtID;
                oMallArtPublish.ChlID = mallArtPublish.ChlID;
                oMallArtPublish.PosID = mallArtPublish.PosID;
                oMallArtPublish.Sorting = mallArtPublish.Sorting;
                oMallArtPublish.Show = mallArtPublish.Show;
                oMallArtPublish.TotalRank = mallArtPublish.TotalRank;
                oMallArtPublish.MatterRank = mallArtPublish.MatterRank;
                oMallArtPublish.LayoutRank = mallArtPublish.LayoutRank;
                oMallArtPublish.ComfortRank = mallArtPublish.ComfortRank;

                string startTime = formCollection["startTime"];
                if (startTime != "")
                {
                    DateTimeOffset DStartTime = DateTimeOffset.Parse(startTime);
                    oMallArtPublish.StartTime = DStartTime;
                }
                else
                {
                    oMallArtPublish.StartTime =DateTimeOffset.Now;
                }
                
                string endTime = formCollection["endTime"];
                if (endTime != "")
                {
                    DateTimeOffset DEndTime = DateTimeOffset.Parse(endTime);
                    oMallArtPublish.EndTime = DEndTime;
                }
                else
                {
                    oMallArtPublish.EndTime = DateTimeOffset.Now;
                }
                oMallArtPublish.Remark = mallArtPublish.Remark;

                dbEntity.MallArtPublishes.Add(oMallArtPublish);
                dbEntity.SaveChanges();
            }
            else
            {
                oMallArtPublish = (from o in dbEntity.MallArtPublishes where (o.Gid == gArtPublish && o.Deleted == false) select o).SingleOrDefault();
                if (oMallArtPublish != null)
                {
                    oMallArtPublish.ArtID = mallArtPublish.ArtID;
                    oMallArtPublish.ChlID = mallArtPublish.ChlID;
                    oMallArtPublish.PosID = mallArtPublish.PosID;
                    oMallArtPublish.Sorting = mallArtPublish.Sorting;
                    oMallArtPublish.Show = mallArtPublish.Show;
                    oMallArtPublish.TotalRank = mallArtPublish.TotalRank;
                    oMallArtPublish.MatterRank = mallArtPublish.MatterRank;
                    oMallArtPublish.LayoutRank = mallArtPublish.LayoutRank;
                    oMallArtPublish.ComfortRank = mallArtPublish.ComfortRank;

                    string startTime = formCollection["startTime"];
                    if (startTime != "")
                    {
                        oMallArtPublish.StartTime = DateTimeOffset.Parse(startTime);
                    }
                    else
                    {
                        oMallArtPublish.StartTime = DateTimeOffset.Now;
                    }

                    string endTime = formCollection["endTime"];
                    if (endTime != "")
                    {
                        oMallArtPublish.EndTime = DateTimeOffset.Parse(endTime);
                    }
                    else
                    {
                        oMallArtPublish.EndTime = DateTimeOffset.Now;
                    }
                    oMallArtPublish.Remark = mallArtPublish.Remark;
                    
                    dbEntity.SaveChanges();
                }
            }
            return "success";
        }
        /// <summary>
        /// 删除选中行文章发布信息
        /// </summary>
        /// <param name="id">选中行的ID</param>
        /// <returns></returns>
        public ActionResult ArtPublishDelete(Guid id)
        {
            MallArtPublish oMallArtPublish = (from o in dbEntity.MallArtPublishes where (o.Gid == id && o.Deleted == false) select o).SingleOrDefault();
            if (oMallArtPublish != null)
            {
                oMallArtPublish.Deleted = true;
                dbEntity.SaveChanges();
            }
            return RedirectToAction("ArticlePublish");
        }




        /// <summary>
        /// 禁用IP
        /// </summary>
        /// <returns></returns>
        public ActionResult DisableIP(Guid? id)
        {
            Guid gUser = (Guid)CurrentSession.UserID;
            MemberUser oMemberUser = (from p in dbEntity.MemberUsers where (p.Gid == gUser && p.Deleted == false) select p).SingleOrDefault();
            //该用户默认组织Guid
            Guid gOrg = oMemberUser.OrgID;
            Guid gChannel = oMemberUser.ChlID;
            //首次加载
            if (id==null)
            {
                //首次加载全局变量组织ID为用户默认所属组织,渠道ID为用户默认所属渠道
                gOrgId = oMemberUser.OrgID;
                gChlId = oMemberUser.ChlID;
                //当前用户为Admin
                if (CurrentSession.IsAdmin == true)
                {
                    //Admin能查看所有组织
                    var oMemberOrganization = (from o in dbEntity.MemberOrganizations where (o.Deleted == false && o.Otype == 0) select o).ToList();
                    List<SelectListItem> OrgList = new List<SelectListItem>();
                    foreach (var item in oMemberOrganization)
                    {
                        SelectListItem SelItem = new SelectListItem();
                        SelItem.Text = item.FullName.GetResource(CurrentSession.Culture);
                        SelItem.Value = item.Gid.ToString();
                        if (item.Gid == gOrgId) SelItem.Selected = true;
                        OrgList.Add(SelItem);
                    }
                    ViewBag.OrgList = OrgList;

                    //该组织下的渠道
                    var oChannel = (from q in dbEntity.MemberOrgChannels where (q.OrgID == gOrgId && q.Deleted == false) select q).ToList();
                    List<SelectListItem> ChlList = new List<SelectListItem>();
                    foreach (var item in oChannel)
                    {
                        Guid gChl = item.ChlID;
                        MemberChannel oMemberChannel = (from o in dbEntity.MemberChannels where (o.Gid == gChl && o.Deleted == false) select o).Single();
                        SelectListItem SelItem = new SelectListItem();
                        SelItem.Text = oMemberChannel.FullName.GetResource(CurrentSession.Culture);
                        SelItem.Value = gChl.ToString();
                        if (gChl == gChannel) SelItem.Selected = true;
                        ChlList.Add(SelItem);
                    }
                    ViewBag.ChlList = ChlList;
                }
                else
                {
                    gOrgId = new Guid(GetPrivilegeOrgs(gUser).ElementAt(0).Value);
                    gChlId = new Guid(GetPrivilegeChls(gUser).ElementAt(0).Value);
                    List<SelectListItem> OrgList = GetPrivilegeOrgs(gUser);
                    List<SelectListItem> ChlList = GetPrivilegeChls(gUser);
                    ViewBag.OrgList = OrgList;
                    ViewBag.ChlList = ChlList;
                }

            }
            //改变组织选项
            else
            {
                List<SelectListItem> OrgList = new List<SelectListItem>();
                if (CurrentSession.IsAdmin == true)
                {
                    List<MemberOrganization> oMemberOrganizations = (from o in dbEntity.MemberOrganizations where (o.Otype == 0 && o.Deleted == false) select o).ToList();
                    foreach (var item in oMemberOrganizations)
                    {
                        SelectListItem SelItem = new SelectListItem { Text = item.FullName.GetResource(CurrentSession.Culture), Value = item.Gid.ToString() };
                        if (item.Gid == id) SelItem.Selected = true;

                        OrgList.Add(SelItem);
                    }

                    ViewBag.OrgList = OrgList;

                    List<SelectListItem> ChlList = new List<SelectListItem>();
                    List<MemberOrgChannel> oMemberOrgChannels = (from o in dbEntity.MemberOrgChannels where (o.OrgID == id && o.Deleted == false) select o).ToList();
                    foreach (var item in oMemberOrgChannels)
                    {
                        Guid ChlID = item.ChlID;
                        MemberChannel oMemberChannel = (from o in dbEntity.MemberChannels where (o.Gid == ChlID && o.Deleted == false) select o).Single();
                        SelectListItem SelItem = new SelectListItem { Text = oMemberChannel.FullName.GetResource(CurrentSession.Culture), Value = ChlID.ToString() };
                        ChlList.Add(SelItem);
                    }
                    ViewBag.ChlList = ChlList;
                    gChlId = new Guid(ChlList.ElementAt(0).Value);
                }
                else
                {
                    ViewBag.OrgList = GetPrivilegeOrgs(gUser);
                    ViewBag.ChlList = GetPrivilegeChls(gUser);
                    gChlId = new Guid(GetPrivilegeChls(gUser).ElementAt(0).Value);
                }
            }
            return View();
        }
        /// <summary>
        /// 获得用户授权组织
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns>返回组织列表</returns>
        public List<SelectListItem> GetPrivilegeOrgs(Guid userID)
        {
            
            MemberUser oMemberUser = (from p in dbEntity.MemberUsers where (p.Gid == userID && p.Deleted == false) select p).SingleOrDefault();
            Guid gOrg = oMemberUser.OrgID;//该用户默认组织ID
            Guid gChannel = oMemberUser.ChlID;//默认渠道ID
            MemberPrivilege oMemPriv = (from o in dbEntity.MemberPrivileges where (o.UserID == userID && o.Deleted == false && o.Ptype == 2) select o).SingleOrDefault();
            List<SelectListItem> OrgList = new List<SelectListItem>();
            //该用户有权限查看多个组织
            if (oMemPriv != null)
            {
                Guid gPrivID = oMemPriv.Gid;
                var oMemPrivItem = (from p in dbEntity.MemberPrivItems where (p.PrivID == gPrivID && p.Deleted == false) select p).ToList();
                foreach (var item in oMemPrivItem)
                {
                    Guid gMemOrganization = (Guid)item.RefID;
                    MemberOrganization oMemberOrganization = (from o in dbEntity.MemberOrganizations where (o.Gid == gMemOrganization && o.Deleted == false) select o).Single();

                    SelectListItem Selitem = new SelectListItem();
                    Selitem.Text = oMemberOrganization.FullName.GetResource(CurrentSession.Culture);
                    Selitem.Value = gMemOrganization.ToString();
                    if (gMemOrganization == gOrgId) Selitem.Selected = true;

                    OrgList.Add(Selitem);

                }
                //默认组织
                MemberOrganization oMemOrg = (from o in dbEntity.MemberOrganizations where (o.Gid == gOrgId && o.Deleted == false) select o).Single();
                SelectListItem listitem = new SelectListItem();
                listitem.Text = oMemOrg.FullName.GetResource(CurrentSession.Culture);
                listitem.Value = oMemOrg.Gid.ToString();
                if (oMemOrg.Gid == gOrgId) listitem.Selected = true;
                OrgList.Add(listitem);
            }
            //该用户只能查看自己的默认所属组织
            else
            {
                MemberOrganization oMemberOrg = (from p in dbEntity.MemberOrganizations where (p.Gid == gOrg && p.Deleted == false) select p).Single();

                SelectListItem Selitem = new SelectListItem();
                Selitem.Text = oMemberOrg.FullName.GetResource(CurrentSession.Culture);
                Selitem.Value = gOrg.ToString();
                Selitem.Selected = true;
            }
            
            return OrgList;
        }
        /// <summary>
        /// 用户授权渠道
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<SelectListItem> GetPrivilegeChls(Guid userID)
        {
            List<SelectListItem> ChlList = new List<SelectListItem>();

            //所选组织下的渠道
            var oMemberChl = (from p in dbEntity.MemberOrgChannels.Include("Channel") where (p.OrgID == gOrgId && p.Deleted == false) select p).ToList();
            //该用户授权的渠道
            Guid gPrivChl = (from q in dbEntity.MemberPrivileges where (q.UserID == userID && q.Ptype == 3 && q.Deleted == false) select q.Gid).SingleOrDefault();
            //如果没有渠道授权，则该用户能够查看该组织下的所有渠道
            if (gPrivChl == null)
            {
                foreach (var Chlitem in oMemberChl)
                {
                    Guid gChl = Chlitem.Gid;
                    MemberChannel oMemberChannel = (from o in dbEntity.MemberChannels where (o.Gid == gChl && o.Deleted == false) select o).Single();

                    SelectListItem ChlSelitem = new SelectListItem();
                    ChlSelitem.Text = oMemberChannel.FullName.GetResource(CurrentSession.Culture);
                    ChlSelitem.Value = gChl.ToString();

                    ChlList.Add(ChlSelitem);
                }

            }
            //对组织内的渠道进行授权  
            else
            {
                //授权的渠道
                var oPrivChlItem = (from m in dbEntity.MemberPrivItems where (m.PrivID == gPrivChl && m.Deleted == false) select m).ToList();
                foreach (var Chlitem in oPrivChlItem)
                {
                    Guid gChlID = (Guid)Chlitem.RefID;
                    MemberOrgChannel oMemberOrgChannel = (from o in dbEntity.MemberOrgChannels where (o.OrgID == gOrgId && o.ChlID == gChlID && o.Deleted == false) select o).SingleOrDefault();

                    //属于该组织的授权渠道
                    if (oMemberChl.Contains(oMemberOrgChannel))
                    {
                        MemberChannel oMemberChannel = (from o in dbEntity.MemberChannels where (o.Gid == gChlID && o.Deleted == false) select o).Single();

                        SelectListItem ChlSelitem = new SelectListItem();
                        ChlSelitem.Text = oMemberChannel.FullName.GetResource(CurrentSession.Culture);
                        ChlSelitem.Value = gChlID.ToString();
                        ChlList.Add(ChlSelitem);
                    }
                }
            }
            return ChlList;
        }
        public ActionResult DisableIPList()
        {
            return View();
        }
        public ActionResult DisableIPListDetail(SearchModel searchModel)
        {
            IQueryable<MallDisabledIp> oMallDisableIP = (from o in dbEntity.MallDisabledIps where (o.Deleted == false && o.OrgID == gOrgId && o.ChlID == gChlId) select o).AsQueryable();

            int n = oMallDisableIP.ToList().Count;
            GridColumnModelList<MallDisabledIp> columns = new GridColumnModelList<MallDisabledIp>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.IpAddress).SetCaption(@LiveAzure.Resource.Model.Mall.MallDisabledIp.IpAddress);
            columns.Add(p => p.Channel.FullName.GetResource(CurrentSession.Culture)).SetName("Channel").SetCaption(@LiveAzure.Resource.Model.Mall.MallDisabledIp.ChlID);
            columns.Add(p => p.Organization.FullName.GetResource(CurrentSession.Culture)).SetName("Organization").SetCaption(@LiveAzure.Resource.Model.Mall.MallDisabledIp.OrgID);

            GridData gridData = oMallDisableIP.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        
        //获得下拉框的组织ID
        public void GetOrgID(Guid id)
        {
            gOrgId = id;
        }
        public void GetChannelID(Guid id)
        {
            gChlId = id;
        }
        public ActionResult DisableIPAddOrEdit(Guid? id)
        {
            MallDisabledIp oMallDisableIP = new MallDisabledIp();
            //添加新的禁用IP
            if (id == null)
            {
                oMallDisableIP.OrgID = gOrgId;
                MemberOrganization oMemberOrganization = (from o in dbEntity.MemberOrganizations where (o.Gid == gOrgId && o.Deleted == false) select o).Single();
                oMallDisableIP.Organization = oMemberOrganization;

                oMallDisableIP.ChlID = gChlId;
                MemberChannel oMemberChannel = (from o in dbEntity.MemberChannels where (o.Gid == gChlId && o.Deleted == false) select o).Single();
                oMallDisableIP.Channel = oMemberChannel;

                List<SelectListItem> StatusList = GetSelectList(oMallDisableIP.GenericStatusList);
                ViewBag.StatusList=StatusList;
            }
            else
            {
                oMallDisableIP = (from o in dbEntity.MallDisabledIps where (o.Gid == id && o.Deleted == false) select o).Single();
                List<SelectListItem> StatusList = GetSelectList(oMallDisableIP.GenericStatusList);
                ViewBag.StatusList = StatusList;
            }
            return View("DisableIPAddOrEdit",oMallDisableIP);
        }
        /// <summary>
        /// 保存新增或者编辑的禁用IP地址
        /// </summary>
        /// <param name="mallDisableIP"></param>
        /// <returns></returns>
        public string  DisableIPSave(MallDisabledIp mallDisableIP)
        {
            string strIP = mallDisableIP.IpAddress;
            var oMallDisableIP = (from o in dbEntity.MallDisabledIps where (o.OrgID == gOrgId && o.ChlID == gChlId) select o).ToList();
            foreach (var item in oMallDisableIP)
            {
                if (item.IpAddress==strIP&&item.Deleted==false)
                {
                    return "ERROR";
                }
                else if (item.IpAddress == strIP && item.Deleted == true)
                {
                    item.Deleted = false;
                    dbEntity.SaveChanges();
                    return "Success";
                }
            }
            MallDisabledIp oDisableIP = new MallDisabledIp();
            oDisableIP.OrgID = gOrgId;
            oDisableIP.ChlID = gChlId;
            oDisableIP.IpAddress = mallDisableIP.IpAddress;
            oDisableIP.Remark = mallDisableIP.Remark;
            dbEntity.MallDisabledIps.Add(oDisableIP);
            dbEntity.SaveChanges();
            return "Success";
        }
        public ActionResult DisableIPDelete(Guid? id)
        {
            MallDisabledIp oMallDisableIP = (from o in dbEntity.MallDisabledIps where (o.Gid == id && o.Deleted == false) select o).Single();
            oMallDisableIP.Deleted = true;
            dbEntity.SaveChanges();
            return RedirectToAction("DisableIP");
        }

        /// <summary>
        /// 敏感词
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SensitiveWord(Guid? id)
        {
            Guid gUser =(Guid) CurrentSession.UserID;
            MemberUser oMemberUser = (from o in dbEntity.MemberUsers where (o.Gid == gUser && o.Deleted == false) select o).Single();
            
            List<SelectListItem> OrgList = new List<SelectListItem>();
            List<SelectListItem> ChlList = new List<SelectListItem>();
            //首次加载
            if (gOrgId == null)
            {
                gOrgId = oMemberUser.OrgID;
                gChlId = oMemberUser.ChlID;
                if (CurrentSession.IsAdmin == true)
                {
                    List<MemberOrganization> oMemberOrganizations = (from o in dbEntity.MemberOrganizations where (o.Deleted == false && o.Otype == 0) select o).ToList();
                    foreach (var item in oMemberOrganizations)
                    {
                        SelectListItem SelItem = new SelectListItem { Text=item.FullName.GetResource(CurrentSession.Culture),Value=item.Gid.ToString()};
                        if (item.Gid == gOrgId) SelItem.Selected = true;

                        OrgList.Add(SelItem);
                    }

                    List<MemberOrgChannel> oMemberOrgChannels = (from o in dbEntity.MemberOrgChannels where (o.Deleted == false && o.OrgID == gOrgId) select o).ToList();
                    foreach (var item in oMemberOrgChannels)
                    {
                        Guid gChannelID = item.ChlID;
                        MemberChannel oMemberChannel = (from o in dbEntity.MemberChannels where (o.Deleted == false && o.Gid == gChannelID) select o).Single();
                        SelectListItem SelItem = new SelectListItem { Text=oMemberChannel.FullName.GetResource(CurrentSession.Culture),Value=gChannelID.ToString()};
                        if(gChannelID==gChlId) SelItem.Selected=true;
                        ChlList.Add(SelItem);
                    }
                }
                else
                {
                    gOrgId = new Guid(GetPrivilegeOrgs(gUser).ElementAt(0).Value);
                    gChlId = new Guid(GetPrivilegeChls(gUser).ElementAt(0).Value);
                    ViewBag.OrgList = GetPrivilegeOrgs(gUser); ;
                    ViewBag.ChlList = GetPrivilegeChls(gUser);
                }
            }
            //更换组织
            else
            {
                if (CurrentSession.IsAdmin == true)
                {
                    List<MemberOrganization> oMemberOrganizations = (from o in dbEntity.MemberOrganizations where (o.Otype == 0 && o.Deleted == false) select o).ToList();
                    foreach (var item in oMemberOrganizations)
                    {
                        SelectListItem SelItem = new SelectListItem { Text = item.FullName.GetResource(CurrentSession.Culture), Value = item.Gid.ToString() };
                        if (item.Gid == id) SelItem.Selected = true;

                        OrgList.Add(SelItem);
                    }

                    ViewBag.OrgList = OrgList;

                    List<MemberOrgChannel> oMemberOrgChannels = (from o in dbEntity.MemberOrgChannels where (o.OrgID == id && o.Deleted == false) select o).ToList();
                    foreach (var item in oMemberOrgChannels)
                    {
                        Guid ChlID = item.ChlID;
                        MemberChannel oMemberChannel = (from o in dbEntity.MemberChannels where (o.Gid == ChlID && o.Deleted == false) select o).Single();
                        SelectListItem SelItem = new SelectListItem { Text = oMemberChannel.FullName.GetResource(CurrentSession.Culture), Value = ChlID.ToString() };
                        ChlList.Add(SelItem);
                    }
                    ViewBag.ChlList = ChlList;
                    if (oMemberOrgChannels.Count > 0)
                    {
                        gChlId = new Guid(ChlList.ElementAt(0).Value);
                    }
                }
                else
                {
                    ViewBag.OrgList = GetPrivilegeOrgs(gUser);
                    ViewBag.ChlList = GetPrivilegeChls(gUser);
                    if (GetPrivilegeChls(gUser).Count > 0)
                    {
                        gChlId = new Guid(GetPrivilegeChls(gUser).ElementAt(0).Value);
                    }
                }
            }
            ViewBag.OrgList = OrgList;
            ViewBag.ChlList = ChlList;
            return View();
        }

        public ActionResult SensitiveWordList(SearchModel searchModel)
        {
            IQueryable<MallSensitiveWord> oMallSensitiveWord = (from o in dbEntity.MallSensitiveWords where (o.Deleted == false && o.OrgID == gOrgId && o.ChlID == gChlId) select o).AsQueryable();

            GridColumnModelList<MallSensitiveWord> columns = new GridColumnModelList<MallSensitiveWord>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Keyword).SetCaption(@LiveAzure.Resource.Model.Mall.MallSensitiveWord.Keyword);
            columns.Add(p => p.Channel.FullName.GetResource(CurrentSession.Culture)).SetName("Channel").SetCaption(@LiveAzure.Resource.Model.Mall.MallSensitiveWord.ChlID);
            columns.Add(p => p.Organization.FullName.GetResource(CurrentSession.Culture)).SetName("Organization").SetCaption(@LiveAzure.Resource.Model.Mall.MallSensitiveWord.OrgID);

            GridData gridData = oMallSensitiveWord.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SensitiveWordAddOrEdit(Guid? id)
        {
            MallSensitiveWord oMallSensitiveWord = new MallSensitiveWord();
            //添加新的禁用IP
            if (id == null)
            {
                oMallSensitiveWord.OrgID = gOrgId;
                MemberOrganization oMemberOrganization = (from o in dbEntity.MemberOrganizations where (o.Gid == gOrgId && o.Deleted == false) select o).Single();
                oMallSensitiveWord.Organization = oMemberOrganization;

                oMallSensitiveWord.ChlID = gChlId;
                MemberChannel oMemberChannel = (from o in dbEntity.MemberChannels where (o.Gid == gChlId && o.Deleted == false) select o).Single();
                oMallSensitiveWord.Channel = oMemberChannel;

                List<SelectListItem> StatusList = GetSelectList(oMallSensitiveWord.GenericStatusList);
            }
            else
            {
                oMallSensitiveWord = (from o in dbEntity.MallSensitiveWords where (o.Gid == id && o.Deleted == false) select o).Single();
                List<SelectListItem> StatusList = GetSelectList(oMallSensitiveWord.GenericStatusList);

                ViewBag.StatusList = StatusList;
            }
            return View("SensitiveWordAddOrEdit", oMallSensitiveWord);
        }
        public string SensitiveWordSave(MallSensitiveWord mallSensitiveWord)
        {
            string Keyword = mallSensitiveWord.Keyword;
            var oMallSensitiveWord = (from o in dbEntity.MallSensitiveWords where (o.OrgID == gOrgId && o.ChlID == gChlId) select o).ToList();
            foreach (var item in oMallSensitiveWord)
            {
                if (item.Keyword == Keyword && item.Deleted == false)
                {
                    return "ERROR";
                }
                else if (item.Keyword == Keyword && item.Deleted == true)
                {
                    item.Deleted = false;
                    dbEntity.SaveChanges();
                    return "Success";
                }
            }
            MallSensitiveWord oSensitiveWord = new MallSensitiveWord();
            oSensitiveWord.OrgID = gOrgId;
            oSensitiveWord.ChlID = gChlId;
            oSensitiveWord.Keyword = oSensitiveWord.Keyword;
            oSensitiveWord.Remark = oSensitiveWord.Remark;
            dbEntity.MallSensitiveWords.Add(oSensitiveWord);
            dbEntity.SaveChanges();
            return "Success";
        }
        public ActionResult SensitiveWordDelete(Guid? id)
        {
            MallSensitiveWord oMallSensitiveWord = (from o in dbEntity.MallSensitiveWords where (o.Gid == id && o.Deleted == false) select o).Single();
            oMallSensitiveWord.Deleted = true;
            dbEntity.SaveChanges();
            return View("DisableIP");
        }
        public ActionResult Click()
        {
            return View();
        }
        public ActionResult Setting()
        {
            return View();
        }
    }
}
