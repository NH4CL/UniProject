using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveAzure.Models;
using LiveAzure.Resource.Stage;
using LiveAzure.Stage.Controllers;
using LiveAzure.Models.General;
using MVC.Controls;
using MVC.Controls.Grid;
using LiveAzure.Models.Member;
using System.Data;

namespace LiveAzure.Stage.Controllers
{
    public class CategoryController : BaseController
    {
        public static byte? nStandardCategoryType;
        public static byte? nPrivateCategoryType;
        public static Guid? oStandardCagegoryGid;
        public static Guid? oPrivateCagegoryGid;
        public static Guid? oPrivateCategoryOrg;
        
        /// <summary>
        /// 分类Tab页 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取用户所属组织
        /// </summary>
        /// <returns></returns>
        public Guid GetOrganization()
        {
            MemberUser oUser = dbEntity.MemberUsers.Find(CurrentSession.UserID);
            return (oUser.OrgID);
        }

        /// <summary>
        /// 标准分类页面
        /// </summary>
        /// <returns></returns>
        public ActionResult StandardCategoryIndex()
        {
            nStandardCategoryType = null;
            ViewBag.standardCategoryList = GetStandardCategoryTypeSelectlist();

            return View();
        }

        /// <summary>
        /// 标准分类列表
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public ActionResult StandardCategoryListTable(Guid? categoryId)
        {
            oStandardCagegoryGid = categoryId;
            return PartialView();
        }

        /// <summary>
        /// 标准分类树
        /// </summary>
        /// <param name="standardCategoryType"></param>
        /// <returns></returns>
        public ActionResult StandardCategoryTree(byte? standardCategoryType)
        {
            if (standardCategoryType != null)
            {
                nStandardCategoryType = standardCategoryType;
            }
            return View();

        }

        public ActionResult StandardCategoryList(SearchModel searchModel)
        {
            try
            {
                List<GeneralStandardCategory> standardCategoryList = new List<GeneralStandardCategory>();
                if (oStandardCagegoryGid == Guid.Empty || oStandardCagegoryGid == null)
                {
                    standardCategoryList = dbEntity.GeneralStandardCategorys.Include("Name").Where(u => u.aParent == null && u.Ctype == nStandardCategoryType && u.Deleted == false).ToList();
                }
                else
                {
                    standardCategoryList = dbEntity.GeneralStandardCategorys.Include("Name").Where(u => u.aParent == oStandardCagegoryGid && u.Deleted == false).ToList();
                }
                IQueryable<GeneralStandardCategory> standardCategorys = standardCategoryList.AsQueryable();
                GridColumnModelList<GeneralStandardCategory> columns = new GridColumnModelList<GeneralStandardCategory>();
                columns.Add(p => p.Gid).SetAsPrimaryKey();
                columns.Add(p => p.Parent == null ? " " : p.Parent.Name.GetResource(CurrentSession.Culture)).SetName("Parent");
                columns.Add(p => p.CategoryTypeName);
                columns.Add(p => p.Code);
                columns.Add(p => p.Name.GetResource(CurrentSession.Culture)).SetName("Name");
                columns.Add(p => p.Sorting);
                GridData gridData = standardCategorys.ToGridData(searchModel, columns);
                return Json(gridData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
            }
        }

        /// <summary>
        /// 返回生成树的json数据
        /// </summary>
        /// <returns></returns>
        public string StandardCategoryTreeLoad()
        {
            return CreateTree(StandardCategoryListTreeNode(null));
        }

        /// <summary>
        /// 异步展开树节点，返回展开节点的json字符串
        /// </summary>
        /// <param name="id">展开树节点的guid</param>
        /// <returns></returns>
        public string StandardCategoryTreeExpand(Guid id)
        {
            List<LiveTreeNode> nodes = StandardCategoryListTreeNode(id);
            return nodes.ToJsonString();
        }

        private List<LiveTreeNode> StandardCategoryListTreeNode(Guid? id )
        {
            List<GeneralStandardCategory> Category = (from g in dbEntity.GeneralStandardCategorys
                                                      where g.Deleted == false && g.Ctype == nStandardCategoryType
                                                      orderby g.Sorting descending
                                                      select g).ToList();
            List<GeneralStandardCategory> oCategory = (from c in Category
                                                       where id == null ? c.aParent == null : c.aParent == id
                                                       select c).ToList();

            List<LiveTreeNode> list = new List<LiveTreeNode>();
            foreach (var item in oCategory)
            {
                LiveTreeNode treeNode = new LiveTreeNode();
                treeNode.id = item.Gid.ToString();
                treeNode.name = (item.Name == null) ? "" : item.Name.GetResource(CurrentSession.Culture);
                treeNode.icon = "";
                treeNode.iconClose = "";
                treeNode.iconOpen = "";
                treeNode.nodeChecked = false;
                treeNode.isParent = true;
                treeNode.nodes = new List<LiveTreeNode>();
                list.Add(treeNode);
            }
            return list;
        }

        /// <summary>
        /// 删除树节点
        /// </summary>
        /// <param name="id">选中树节点的guid</param>
        public ActionResult RemoveStandardCategory(Guid id)
        {
            GeneralStandardCategory standardCategoryChildList = dbEntity.GeneralStandardCategorys.Include("ChildItems").Where(p => p.Gid == id).Single();
            int nChildItemCount=standardCategoryChildList.ChildItems.Count;
            standardCategoryChildList.Deleted = true;

            if (nChildItemCount > 0) 
                StandardCategoryDeleteChild(standardCategoryChildList.ChildItems.ToList<GeneralStandardCategory>());

            dbEntity.SaveChanges();
            return RedirectToAction("StandardCategoryTree");
        }

        /// <summary>
        /// 递归删除子项
        /// </summary>
        /// <param name="list">子项的集合</param>
        public void StandardCategoryDeleteChild(List<GeneralStandardCategory> list)
        {
            int nListCount = list.Count;

            for (int i = 0; i < nListCount; i++)
            {
                if (list.ElementAt(i).ChildItems.Count > 0)
                {
                    list.ElementAt(i).Deleted = true;
                    StandardCategoryDeleteChild(list.ElementAt(i).ChildItems.ToList<GeneralStandardCategory>());
                }
                else
                {
                    list.ElementAt(i).Deleted = true;
                }
            }

        }

        /// <summary>
        /// 编辑标准分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult StandardCategoryNodeEdit(Guid oGid)
        {
            var standardCategoryForEdit = dbEntity.GeneralStandardCategorys.Include("Parent").Include("Name").Where(s => s.Gid == oGid).Single();
            Guid orgGid = GetOrganization();
            standardCategoryForEdit.Name = RefreshResource(ModelEnum.ResourceType.STRING, standardCategoryForEdit.Name, orgGid);
            return PartialView(standardCategoryForEdit);
        }

        /// <summary>
        /// 保存编辑的标准分类
        /// </summary>
        /// <param name="gid"></param>
        /// <param name="standardCategoryNodeCollection"></param>
        /// <returns></returns>
        public ActionResult SaveStandardCategoryNode(GeneralStandardCategory editStandardCategory)
        {
            var oldStandardCategory = dbEntity.GeneralStandardCategorys.Include("Name").Include("Parent").Where(s => s.Gid == editStandardCategory.Gid).Single();
            oldStandardCategory.Code = editStandardCategory.Code;
            oldStandardCategory.Name.SetResource(ModelEnum.ResourceType.STRING, editStandardCategory.Name);
            oldStandardCategory.Sorting = editStandardCategory.Sorting;

            if (ModelState.IsValid)
            {
                dbEntity.Entry(oldStandardCategory).State = EntityState.Modified;
                dbEntity.SaveChanges();
            }

            return RedirectToAction("StandardCategoryListTable", new { categoryId = oldStandardCategory.aParent });
        }

        /// <summary>
        /// 添加标准分类页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult StandardCategoryNodeAdd(byte categoryType)
        {
            var oParent = dbEntity.GeneralStandardCategorys.Where(g => g.Gid == oStandardCagegoryGid).SingleOrDefault();
            Guid orgGid = GetOrganization();
            if (oParent != null)
            {
                GeneralStandardCategory oGeneralStandardCategory = new GeneralStandardCategory
                {
                    Ctype = categoryType,
                    aParent = oStandardCagegoryGid,
                    Name = NewResource(ModelEnum.ResourceType.STRING,orgGid),
                    Parent = oParent
                };
                return View(oGeneralStandardCategory);
            }
            else
            {
                GeneralStandardCategory oGeneralStandardCategory = new GeneralStandardCategory
                {
                    Ctype = categoryType,
                    Name = NewResource(ModelEnum.ResourceType.STRING,orgGid)
                };
                return View(oGeneralStandardCategory);
            }
        }

        /// <summary>
        /// 保存添加的标准分类
        /// </summary>
        /// <param name="parentId">父类ID</param>
        /// <param name="StandardCategoryNodeCollection"></param>
        /// <returns></returns>
        public ActionResult AddStandardCategoryNode(GeneralStandardCategory newGeneralCategory)
        {
            GeneralStandardCategory oGeneralStandardCategory = new GeneralStandardCategory
            {
                Ctype = newGeneralCategory.Ctype,
                Code = newGeneralCategory.Code,
                aParent = newGeneralCategory.aParent,
                Name = new GeneralResource(ModelEnum.ResourceType.STRING, newGeneralCategory.Name),
                Sorting = newGeneralCategory.Sorting
            };
            Guid? parentId = newGeneralCategory.aParent;
            dbEntity.GeneralStandardCategorys.Add(oGeneralStandardCategory);
            dbEntity.SaveChanges();
            return RedirectToAction("StandardCategoryListTable", new { categoryId = parentId });
        }

        /// <summary>
        /// 标准分类类型下拉框列表
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetStandardCategoryTypeSelectlist()
        {
            List<SelectListItem> oStandardCategoryTypeList = new List<SelectListItem>();
            List<ListItem> StandardCategoryTypeList = new GeneralStandardCategory().CategoryTypeList;
            foreach (var item in StandardCategoryTypeList)
            {
                oStandardCategoryTypeList.Add(new SelectListItem { Value = item.Value, Text = item.Text });
            }
            return oStandardCategoryTypeList;
        }


//------------------------------------------------------- PrivateCategory --------------------------------------------------------------------------------------------------//

        /// <summary>
        /// 私有分类页面
        /// </summary>
        /// <returns></returns>
        public ActionResult PrivateCategoryIndex()
        {
            nPrivateCategoryType = null;
            ViewBag.privateCategoryList = GetPrivateCategoryTypeSelectlist();
            ViewBag.orgnizations = GetOrganizationList();
            return View();
        }

        /// <summary>
        /// 私有分类列表
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public ActionResult PrivateCategoryListTable(Guid? categoryId)
        {
            oPrivateCagegoryGid = categoryId;
            return PartialView();
        }

        /// <summary>
        /// 私有分类树
        /// </summary>
        /// <param name="privateCategoryType"></param>
        /// <param name="privateCategoryOrg"></param>
        /// <returns></returns>
        public ActionResult PrivateCategoryTree(byte? privateCategoryType, Guid? privateCategoryOrg)
        {
            if (privateCategoryType != null)
            {
                nPrivateCategoryType = privateCategoryType;
            }
            if (privateCategoryOrg != null)
            {
                oPrivateCategoryOrg = privateCategoryOrg;
            }
            return View();
        }

        /// <summary>
        /// 返回私有分类Grid列表Json数据
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult PrivateCategoryList(SearchModel searchModel)
        {
            try
            {
                Guid orgId = GetOrganization();
                List<GeneralPrivateCategory> privateCategoryList = new List<GeneralPrivateCategory>();
                if (oPrivateCagegoryGid == Guid.Empty || oPrivateCagegoryGid == null)
                {
                    privateCategoryList = dbEntity.GeneralPrivateCategorys.Include("Name").Where(u => u.aParent == null && u.Ctype == nPrivateCategoryType && u.Deleted == false).ToList();
                }
                else
                {
                    privateCategoryList = dbEntity.GeneralPrivateCategorys.Include("Name").Where(u => u.aParent == oPrivateCagegoryGid && u.Deleted == false).ToList();
                }
                IQueryable<GeneralPrivateCategory> privateCategorys = (from p in privateCategoryList
                                                                       where oPrivateCategoryOrg == null ? p.OrgID == orgId : p.OrgID == oPrivateCategoryOrg
                                                                       select p).AsQueryable();
                    
                GridColumnModelList<GeneralPrivateCategory> columns = new GridColumnModelList<GeneralPrivateCategory>();
                columns.Add(p => p.Gid).SetAsPrimaryKey();
                columns.Add(p => (p.Parent) == null ? " " : p.Parent.Name.GetResource(CurrentSession.Culture)).SetName("Parent");
                columns.Add(p => p.CategoryTypeName);
                columns.Add(p => p.Code);
                columns.Add(p => p.Name.GetResource(CurrentSession.Culture)).SetName("Name");
                columns.Add(p => p.Sorting);
                GridData gridData = privateCategorys.ToGridData(searchModel, columns);
                return Json(gridData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
            }
        }

        /// <summary>
        /// 返回生成树的json数据
        /// </summary>
        /// <returns></returns>
        public string PrivateCategoryTreeLoad()
        {
            return CreateTree(PrivateCategoryListTreeNode(null));
        }

        public string PrivateCategoryMenuLoad(byte? privateCategoryType)
        {
            if (privateCategoryType != null)
                nPrivateCategoryType = privateCategoryType;
            return PrivateCategoryTreeLoad();
        }

        /// <summary>
        /// 异步展开树节点，返回展开节点的json字符串
        /// </summary>
        /// <param name="id">展开树节点的guid</param>
        /// <returns></returns>
        public string PrivateCategoryTreeExpand(Guid id)
        {
            List<LiveTreeNode> nodes = PrivateCategoryListTreeNode(id);
            return nodes.ToJsonString();
        }

        public string PrivateCategoryMenuExpand(Guid id, byte? privateCategoryType)
        {
            if (privateCategoryType != null)
                nPrivateCategoryType = privateCategoryType;
            return PrivateCategoryTreeExpand(id);
        }

        private List<LiveTreeNode> PrivateCategoryListTreeNode(Guid? id)
        {
            Guid orgID = GetOrganization();
            List<GeneralPrivateCategory> oCategoryByType = (from g in dbEntity.GeneralPrivateCategorys
                                                      where g.Deleted == false && g.Ctype == nPrivateCategoryType && id == null ? g.aParent == null : g.aParent == id 
                                                      orderby g.Sorting descending
                                                      select g).ToList();
            List<GeneralPrivateCategory> oCategory = (from o in oCategoryByType
                                                      where oPrivateCategoryOrg == null ? o.OrgID == orgID : o.OrgID == oPrivateCategoryOrg
                                                      select o).ToList();

            List<LiveTreeNode> list = new List<LiveTreeNode>();
            foreach (var item in oCategory)
            {
                LiveTreeNode treeNode = new LiveTreeNode()
                {
                    id = item.Gid.ToString(),
                    name = item.Name.GetResource(CurrentSession.Culture),
                    icon = "",
                    iconClose = "",
                    iconOpen = "",
                    nodeChecked = false,
                    isParent = true,
                    nodes = new List<LiveTreeNode>()
                };
                list.Add(treeNode);
            }
            return list;
        }

        /// <summary>
        /// 删除树节点
        /// </summary>
        /// <param name="id">选中树节点的guid</param>
        public ActionResult RemovePrivateCategory(Guid id)
        {
            GeneralPrivateCategory privateCategoryChildList = dbEntity.GeneralPrivateCategorys.Include("ChildItems").Where(p => p.Gid == id).Single();
            int nChildItemCount = privateCategoryChildList.ChildItems.Count;
            privateCategoryChildList.Deleted = true;

            if (nChildItemCount > 0)
                PrivateCategoryDeleteChild(privateCategoryChildList.ChildItems.ToList<GeneralPrivateCategory>());

            dbEntity.SaveChanges();
            return RedirectToAction("PrivateCategoryTree");
        }

        /// <summary>
        /// 递归删除privateCategory的子项
        /// </summary>
        /// <param name="list">program子项的集合</param>
        public void PrivateCategoryDeleteChild(List<GeneralPrivateCategory> list)
        {
            int nListCount = list.Count;

            for (int i = 0; i < nListCount; i++)
            {
                if (list.ElementAt(i).ChildItems.Count > 0)
                {
                    list.ElementAt(i).Deleted = true;
                    PrivateCategoryDeleteChild(list.ElementAt(i).ChildItems.ToList<GeneralPrivateCategory>());
                }
                else
                {
                    list.ElementAt(i).Deleted = true;
                }
            }

        }

        /// <summary>
        /// 编辑私有分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PrivateCategoryNodeEdit(Guid oGid)
        {
            var privateCategoryForEdit = dbEntity.GeneralPrivateCategorys.Include("Parent").Include("Name").Where(s => s.Gid == oGid).Single();
            Guid orgId = GetOrganization();
            Guid orgGid = (oPrivateCategoryOrg == null) ? orgId : (Guid)oPrivateCategoryOrg;
            privateCategoryForEdit.Name = RefreshResource(ModelEnum.ResourceType.STRING, privateCategoryForEdit.Name, orgGid);
            ViewBag.IsShow = SelectEnumList(privateCategoryForEdit.Show);
            ViewBag.UnitList = GetUnitList();
            return PartialView(privateCategoryForEdit);
        }

        /// <summary>
        /// 保存编辑的私有分类
        /// </summary>
        /// <param name="gid"></param>
        /// <param name="standardCategoryNodeCollection"></param>
        /// <returns></returns>
        public ActionResult SavePrivateCategoryNode(GeneralPrivateCategory editPrivateCategory)
        {
            var oldPrivateCategory = dbEntity.GeneralPrivateCategorys.Include("Name").Include("Parent").Where(s => s.Gid == editPrivateCategory.Gid).Single();
            oldPrivateCategory.Code = editPrivateCategory.Code;
            oldPrivateCategory.Name.SetResource(ModelEnum.ResourceType.STRING, editPrivateCategory.Name);
            oldPrivateCategory.Sorting = editPrivateCategory.Sorting;

            if (ModelState.IsValid)
            {
                dbEntity.Entry(oldPrivateCategory).State = EntityState.Modified;
                dbEntity.SaveChanges();
            }

            return RedirectToAction("PrivateCategoryListTable", new { categoryId = oldPrivateCategory.aParent });
        }

        /// <summary>
        /// 添加私有分类页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PrivateCategoryNodeAdd(byte categoryType)
        {
            var oParent = dbEntity.GeneralPrivateCategorys.Where(g => g.Gid == oPrivateCagegoryGid).SingleOrDefault();
            Guid orgId = GetOrganization();
            Guid orgGid = (oPrivateCategoryOrg == null) ? orgId : (Guid)oPrivateCategoryOrg;
            if (oParent != null)
            {
                GeneralPrivateCategory oGeneralPrivateCategory = new GeneralPrivateCategory
                {
                    Ctype = categoryType,
                    aParent = oPrivateCagegoryGid,
                    Name = NewResource(ModelEnum.ResourceType.STRING,orgGid),
                    Parent = oParent
                };
                ViewBag.IsShow = SelectEnumList(oGeneralPrivateCategory.Show);
                ViewBag.UnitList = GetUnitList();
                return View(oGeneralPrivateCategory);
            }
            else
            {
                GeneralPrivateCategory oGeneralPrivateCategory = new GeneralPrivateCategory
                {
                    Ctype = categoryType,
                    Name = NewResource(ModelEnum.ResourceType.STRING, orgGid)
                };
                ViewBag.IsShow = SelectEnumList(oGeneralPrivateCategory.Show);
                ViewBag.UnitList = GetUnitList();
                return View(oGeneralPrivateCategory);
            }
        }

        /// <summary>
        /// 保存添加的私有分类
        /// </summary>
        /// <param name="parentId">父类ID</param>
        /// <param name="StandardCategoryNodeCollection"></param>
        /// <returns></returns>
        public ActionResult AddPrivateCategoryNode(GeneralPrivateCategory newGeneralCategory)
        {
            GeneralPrivateCategory oGeneralPrivateCategory = new GeneralPrivateCategory
            {
                Ctype = newGeneralCategory.Ctype,
                Code = newGeneralCategory.Code,
                aParent = newGeneralCategory.aParent,
                Name = new GeneralResource(ModelEnum.ResourceType.STRING, newGeneralCategory.Name),
                Sorting = newGeneralCategory.Sorting,
                OrgID = GetOrganization()
            };
            Guid? parentId = newGeneralCategory.aParent;
            dbEntity.GeneralPrivateCategorys.Add(oGeneralPrivateCategory);
            dbEntity.SaveChanges();
            return RedirectToAction("PrivateCategoryListTable", new { categoryId = parentId });
        }

        public List<SelectListItem> GetUnitList()
        {
            List<SelectListItem> oList = new List<SelectListItem>();
            var oUnitList = dbEntity.GeneralMeasureUnits.Where(m => m.Deleted == false).ToList();
            foreach (GeneralMeasureUnit item in oUnitList)
            {
                oList.Add(new SelectListItem { Text = item.Name.GetResource(CurrentSession.Culture), Value = item.Gid.ToString() });
            }
            return oList;
        }

        public List<SelectListItem> GetPrivateCategoryTypeSelectlist()
        {
            List<SelectListItem> oPrivateCategoryTypeList = new List<SelectListItem>();
            List<ListItem> oPrivateCategorys = new GeneralPrivateCategory().CategoryTypeList;
            foreach (var item in oPrivateCategorys)
            {
                oPrivateCategoryTypeList.Add(new SelectListItem { Value = item.Value, Text = item.Text });
            }
            return oPrivateCategoryTypeList;
        }

        public List<SelectListItem> GetOrganizationList()
        {
            List<SelectListItem> oOrganizationList = new List<SelectListItem>();
            var OrganizationList = dbEntity.MemberOrganizations.Where(o => o.Deleted == false).ToList();
            foreach (var item in OrganizationList)
            {
                oOrganizationList.Add(new SelectListItem { Value = item.Gid.ToString(), Text = item.FullName.GetResource(CurrentSession.Culture) });
            }
            return oOrganizationList;
        }
    }
}
