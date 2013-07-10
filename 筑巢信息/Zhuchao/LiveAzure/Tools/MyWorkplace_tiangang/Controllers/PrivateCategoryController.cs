using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveAzure.Models;
using LiveAzure.Models.Member;
using LiveAzure.Resource.Stage;
using LiveAzure.Stage.Controllers;
using LiveAzure.Models.General;
using MVC.Controls;
using MVC.Controls.Grid;

namespace MyWorkplace_tiangang.Controllers
{
    public class PrivateCategoryController : BaseController
    {

        string LogName = "admin";

        public Guid GetOrganization()
        {
            MemberUser oUser = dbEntity.MemberUsers.Where(m => m.LoginName == LogName).Single();
            //MemberOrganization oOrganization = dbEntity.MemberOrganizations.Where(m => m.Gid == oUser.OrgID).Single();
            return (oUser.Gid);
        }
        //
        // GET: /Program/
        static byte nPrivateCategoryType = (byte)ModelEnum.PrivateCategoryType.PRODUCT;

        public ActionResult Index()
        {
            MemberUser oUser = dbEntity.MemberUsers.Where(m => m.LoginName == LogName).Single();
            MemberOrganization oOrganization = dbEntity.MemberOrganizations.Where(m => m.Gid == oUser.OrgID).Single();
            ViewBag.privateCategoryList = getPrivateCategoryTypeSelectlist();
            return View();
        }

        /// <summary>
        /// 返回生成树的json数据
        /// </summary>
        /// <returns></returns>
        public string TreeLoad()
        {
            var privateCategoryList = dbEntity.GeneralPrivateCategorys.Where(o => o.Parent == null && o.Deleted == false && o.Ctype == nPrivateCategoryType).ToList();

            int nPrivateCategoryCount = privateCategoryList.Count();

            List<LiveTreeNode> list = new List<LiveTreeNode>();

            foreach (var item in privateCategoryList)
            {
                LiveTreeNode treeNode = new LiveTreeNode();
                treeNode.id = item.Gid.ToString();
                treeNode.name = item.Name.GetResource(CurrentSession.Culture);
                treeNode.icon = "";
                treeNode.iconClose = "";
                treeNode.iconOpen = "";
                treeNode.nodeChecked = false;
                treeNode.nodeChecked = true;
                treeNode.progUrl = "";
                treeNode.nodes = new List<LiveTreeNode>();

                list.Add(treeNode);
            }

            string strTreeJson = CreateTree(list);

            return strTreeJson;

        }

        /// <summary>
        /// 异步展开树节点，返回展开节点的json字符串
        /// </summary>
        /// <param name="id">展开树节点的guid</param>
        /// <returns></returns>
        public string TreeExpand(Guid id)
        {
            string strTreeJson = "";

            //当展开root节点的时候
            if (id.ToString().Equals("00000000-0000-0000-0000-000000000000"))
            {
                var privateCategoryList = dbEntity.GeneralPrivateCategorys.Where(o => o.Parent == null && o.Deleted == false).ToList();

                int iProgCount = privateCategoryList.Count();

                List<LiveTreeNode> list = new List<LiveTreeNode>();

                foreach (var item in privateCategoryList)
                {
                    if (item.Deleted == false)
                    {
                        LiveTreeNode treeNode = new LiveTreeNode();
                        treeNode.id = item.Gid.ToString();
                        treeNode.name = item.Name.GetResource(CurrentSession.Culture);
                        treeNode.icon = "";
                        treeNode.iconClose = "";
                        treeNode.iconOpen = "";
                        treeNode.nodeChecked = false;
                        treeNode.isParent = false;
                        treeNode.progUrl = "";
                        treeNode.nodes = new List<LiveTreeNode>();

                        list.Add(treeNode);
                    }
                }

                strTreeJson = CreateTreeJson(list, "");
            }
            else                                                          //非root节点展开的时候，回传的gid不为空
            {
                GeneralPrivateCategory privateCategoryChildList = dbEntity.GeneralPrivateCategorys.Include("ChildItems").Where(p => p.Gid == id).Single();

                List<LiveTreeNode> list = new List<LiveTreeNode>();

                foreach (var item in privateCategoryChildList.ChildItems)
                {
                    if (item.Deleted == false)
                    {
                        LiveTreeNode treeNode = new LiveTreeNode();
                        treeNode.id = item.Gid.ToString();
                        treeNode.name = item.Name.GetResource(CurrentSession.Culture);
                        treeNode.icon = "../demo/zTreeStyle/img/edit.png";
                        treeNode.iconClose = "../demo/zTreeStyle/img/edit.png";
                        treeNode.iconOpen = "../demo/zTreeStyle/img/edit.png";
                        treeNode.nodeChecked = false;
                        treeNode.isParent = false;
                        treeNode.progUrl = "";
                        treeNode.nodes = new List<LiveTreeNode>();

                        list.Add(treeNode);
                    }
                }

                strTreeJson = CreateTreeJson(list, "");
            }

            return "[" + strTreeJson + "]";

        }

        /// <summary>
        /// 删除树节点
        /// </summary>
        /// <param name="id">选中树节点的guid</param>
        public void TreeRemove(Guid id)
        {
            Guid gid = id;

            GeneralPrivateCategory privateCategoryChildList = dbEntity.GeneralPrivateCategorys.Include("ChildItems").Where(p => p.Gid == id).Single();

            privateCategoryChildList.Deleted = true;

            DeleteChild(privateCategoryChildList.ChildItems.ToList<GeneralPrivateCategory>());

            dbEntity.SaveChanges();

        }

        /// <summary>
        /// 递归删除program的子项
        /// </summary>
        /// <param name="list">program子项的集合</param>
        public void DeleteChild(List<GeneralPrivateCategory> list)
        {
            int nListCount = list.Count;

            for (int i = 0; i < nListCount; i++)
            {
                if (list.ElementAt(i).ChildItems.Count > 0)
                {
                    list.ElementAt(i).Deleted = true;
                    DeleteChild(list.ElementAt(i).ChildItems.ToList<GeneralPrivateCategory>());
                }
                else
                {
                    list.ElementAt(i).Deleted = true;
                }
            }

        }

        /// <summary>
        /// 编辑程序
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ProgramNodeEdit(Guid id)
        {
            var privateCategoryNode = dbEntity.GeneralStandardCategorys.Include("Parent").Where(s => s.Gid == id).Single();
            return PartialView("ProgramNodeEdit", privateCategoryNode);
        }

        /// <summary>
        /// 保存编辑程序
        /// </summary>
        /// <param name="gid"></param>
        /// <param name="privateCategoryNodeCollection"></param>
        /// <returns></returns>
        public ActionResult SavePrivateCategoryNode(Guid gid, FormCollection privateCategoryNodeCollection)
        {
            var selectProgram = dbEntity.GeneralStandardCategorys.Where(s => s.Gid == gid).Single();
            selectProgram.Code = privateCategoryNodeCollection["Code"];
            selectProgram.Name.Matter = privateCategoryNodeCollection["Name.Matter"];
            selectProgram.Sorting = Int32.Parse(privateCategoryNodeCollection["Sorting"]);

            dbEntity.SaveChanges();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 添加程序
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ProgramNodeAdd(Guid id)
        {
            GeneralPrivateCategory oGeneralPrivateCategory = new GeneralPrivateCategory();

            oGeneralPrivateCategory.aParent = id;

            oGeneralPrivateCategory.Parent = dbEntity.GeneralPrivateCategorys.Where(p => p.Gid == id).Single();

            return View(oGeneralPrivateCategory);
        }

        /// <summary>
        /// 添加程序
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="programNodeCollection"></param>
        /// <returns></returns>
        public ActionResult AddProgramNode(Guid parentId, FormCollection programNodeCollection)
        {
            GeneralPrivateCategory oGeneralPrivateCategory = new GeneralPrivateCategory();
            oGeneralPrivateCategory.OrgID = GetOrganization();
            oGeneralPrivateCategory.aParent = parentId;
            oGeneralPrivateCategory.Code = programNodeCollection["Code"];
            oGeneralPrivateCategory.Sorting = Convert.ToInt32(programNodeCollection["Sorting"]);

            GeneralResource oResouce = new GeneralResource();
            oResouce.Code = "4534645";
            oResouce.Culture = 2052;
            oResouce.Matter = programNodeCollection["Name.Matter"];
            dbEntity.GeneralResources.Add(oResouce);

            oGeneralPrivateCategory.Name = oResouce;

            dbEntity.GeneralPrivateCategorys.Add(oGeneralPrivateCategory);

            dbEntity.SaveChanges();

            return RedirectToAction("Index");
        }

        public List<SelectListItem> getPrivateCategoryTypeSelectlist()
        {
            var oPrivateCategoryTypeName = Enum.GetNames(typeof(ModelEnum.PrivateCategoryType));
            var oPrivateCategoryTypevalues = (ModelEnum.PrivateCategoryType[])Enum.GetValues(typeof(ModelEnum.StandardCategoryType));
            List<SelectListItem> oPrivateCategoryTypeList = new List<SelectListItem>();
            int nPrivateCategoryTypeCount = oPrivateCategoryTypeName.Count();
            for (int i = 0; i < nPrivateCategoryTypeCount; i++)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = oPrivateCategoryTypeName[i],
                    Value = ((byte)(oPrivateCategoryTypevalues[i])).ToString()
                };
                oPrivateCategoryTypeList.Add(item);
            }
            return oPrivateCategoryTypeList;
        }

        public ActionResult GetCType(byte privateCategoryType)
        {
            nPrivateCategoryType = privateCategoryType;
            //RedirectToAction("TreeLoad");
            return PartialView();

        }

    }
}

