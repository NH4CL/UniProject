using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC.Controls;
using MVC.Controls.Grid;
using LiveAzure.BLL;
using LiveAzure.Models;
using LiveAzure.Models.General;
using LiveAzure.Resource.Stage;

namespace LiveAzure.Stage.Controllers
{
    public class ProgramController : BaseController
    {
        private static Guid progNodeGidGrid; //表格获取程序结点的Guid
        private static Guid progNodeGidEdit; //用于编辑的程序结点的Guid

        public ActionResult Index()
        {
            // 权限验证
            string strProgramCode = Request.RequestContext.RouteData.Values["Controller"].ToString() +
                Request.RequestContext.RouteData.Values["Action"].ToString();
            if (!base.Permission(strProgramCode))
                return RedirectToAction("ErrorPage", "Home", new { LiveAzure.Resource.Common.NoPermission });
            return View();
        }

        /// <summary>
        /// 部分视图 树 页面
        /// </summary>
        /// <returns>树视图</returns>
        public ViewResult Tree()
        {
            ViewBag.EnableEdit = (base.GetProgramNode("EnableEdit") == "1") ? true : false;
            return View();
        }

        /// <summary>
        /// 异步加载 生成树数据
        /// </summary>
        /// <returns>返回生成树的json数据</returns>
        public string TreeLoad()
        {
            return CreateTree(ListTreeNode(Guid.Empty));
        }

        /// <summary>
        /// 异步展开树节点
        /// </summary>
        /// <param name="id">展开树节点的guid</param>
        /// <returns>返回展开节点的json字符串</returns>
        public string TreeExpand(Guid id)
        {
            List<LiveTreeNode> nodes = ListTreeNode(id);
            return nodes.ToJsonString();
        }

        private List<LiveTreeNode> ListTreeNode(Guid id)
        {
            Guid? gNodeID = null;
            // 当展开root节点的时候
            if (id != Guid.Empty)
                gNodeID = id;
            List<GeneralProgram> oUserPrograms = (from g in dbEntity.GeneralPrograms
                                                  where g.Deleted == false && (gNodeID == null ? g.aParent == null : g.aParent == gNodeID)
                                                  orderby g.Sorting descending
                                                  select g).ToList();
            List<LiveTreeNode> list = new List<LiveTreeNode>();
            foreach (var item in oUserPrograms)
            {
                LiveTreeNode treeNode = new LiveTreeNode();
                treeNode.id = item.Gid.ToString();
                treeNode.name = item.Name.GetResource(CurrentSession.Culture);
                treeNode.icon = "";
                treeNode.iconClose = "";
                treeNode.iconOpen = "";
                treeNode.nodeChecked = false;
                if (item.Terminal == true)
                    treeNode.isParent = false;
                else
                    treeNode.isParent = true;
                treeNode.progUrl = item.ProgUrl;
                treeNode.nodes = new List<LiveTreeNode>();
                list.Add(treeNode);
            }
            return list;
        }

        /// <summary>
        /// 删除树节点
        /// </summary>
        /// <param name="id">选中树节点的guid</param>
        public void TreeRemove(Guid id)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                GeneralProgram oProgram = dbEntity.GeneralPrograms.Include("ChildItems").Where(p => p.Gid == id).Single();
                oProgram.Deleted = true;
                int rChildCount = oProgram.ChildItems.ToList().Count;
                if (rChildCount > 0)
                {
                    DeleteChild(oProgram.ChildItems.ToList<GeneralProgram>());
                }
                else
                {
                    DeleteChildItem(oProgram.ProgramNodes.ToList<GeneralProgNode>());
                }
                dbEntity.SaveChanges();
            }
        }

        /// <summary>
        /// 递归删除program的子项
        /// </summary>
        /// <param name="list">program子项的集合</param>
        public void DeleteChild(List<GeneralProgram> list)
        {
            foreach (var item in list)
            {
                if (item.ChildItems.Count > 0)
                {
                    item.Deleted = true;
                    DeleteChild(item.ChildItems.ToList<GeneralProgram>());
                }
                else
                {
                    item.Deleted = true;
                    DeleteChildItem(item.ProgramNodes.ToList<GeneralProgNode>());
                }
            }
        }

        /// <summary>
        /// 删除program子项的Item
        /// </summary>
        public void DeleteChildItem(List<GeneralProgNode> list)
        {
            foreach (var item in list)
            {
                GeneralProgNode oProgramNodeItem = dbEntity.GeneralProgNodes.Include("Name").Include("Optional").Where(p => p.Gid == item.Gid).Single();
                if (oProgramNodeItem.Deleted == false)
                {
                    oProgramNodeItem.Deleted = true;
                    oProgramNodeItem.Name.Deleted = true;
                    if (oProgramNodeItem.InputMode == 1)
                    {
                        oProgramNodeItem.Optional.Deleted = true;
                    }
                }
            }
        }

        /// <summary>
        /// 编辑程序
        /// </summary>
        /// <param name="id">被选中的程序节点的Guid</param>
        /// <returns>编辑程序节点视图</returns>
        public ActionResult ProgramEdit(Guid? id)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                if (id.Equals(Guid.Empty) || id.Equals(null))
                    id = progNodeGidEdit;
                try
                {
                    var oProgram = dbEntity.GeneralPrograms.Include("Parent").Include("Name").Where(s => s.Gid == id && s.Deleted==false).Single();
                    oProgram.Name = RefreshResource(ModelEnum.ResourceType.STRING, oProgram.Name);
                    ViewBag.TerminalList = SelectEnumList(oProgram.Terminal);
                    ViewBag.ShowList = SelectEnumList(oProgram.Show);
                    if (oProgram.Parent == null)
                        ViewBag.ParentName = "Root";
                    else
                        ViewBag.ParentName = oProgram.Parent.Name.GetResource(CurrentSession.Culture);
                    ViewBag.EnableEdit = (base.GetProgramNode("EnableEdit") == "1") ? true : false;
                    ViewBag.Culture = CurrentSession.Culture;
                    return View(oProgram);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
                }
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }

        /// <summary>
        /// 保存编辑程序
        /// </summary>
        /// <param name="gid">被选中的程序节点的Guid</param>
        /// <param name="programNodeCollection">程序节点内容</param>
        /// <returns>重定向到树视图</returns>
        public ActionResult ProgramEditSave(GeneralProgram program)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                try
                {
                    var oProgram = dbEntity.GeneralPrograms.Where(s => s.Gid == program.Gid).Single();
                    oProgram.Code = program.Code;
                    oProgram.ProgUrl = program.ProgUrl;
                    oProgram.Name.SetResource(ModelEnum.ResourceType.STRING, program.Name);
                    oProgram.Sorting = program.Sorting;
                    oProgram.Terminal = program.Terminal;
                    program.Show = program.Show;
                    dbEntity.SaveChanges();
                    return RedirectToAction("Tree");
                }
                catch (Exception ex)
                {
                    return RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
                }
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }

        /// <summary>
        /// 添加程序
        /// </summary>
        /// <param name="id">被选中的程序节点的Guid</param>
        /// <returns>添加程序节点视图</returns>
        public ActionResult ProgramNew(Guid id)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                GeneralProgram oProgram = new GeneralProgram { Name = NewResource(ModelEnum.ResourceType.STRING) };
                string strParentName = "root";
                if (id.Equals(Guid.Empty) || id.Equals(null))
                {
                    oProgram.aParent = null;
                    oProgram.Parent = null;
                }
                else
                {
                    oProgram.aParent = id;
                    oProgram.Parent = dbEntity.GeneralPrograms.Where(p => p.Gid == id).Single();
                    strParentName = oProgram.Parent.Name.GetResource(CurrentSession.Culture);
                }
                ViewBag.TerminalList = SelectEnumList(oProgram.Terminal);
                ViewBag.ShowList = SelectEnumList(oProgram.Show);
                ViewBag.ParentName = strParentName;
                ViewBag.EnableEdit = (base.GetProgramNode("EnableEdit") == "1") ? true : false;
                return View(oProgram);
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }

        /// <summary>
        /// 添加程序节点
        /// </summary>
        /// <param name="parentId">被选中的程序节点的Guid</param>
        /// <param name="programNodeCollection">要添加的程序节点的内容</param>
        /// <returns>树视图</returns>
        public ActionResult ProgramNewSave(Guid? parentId, GeneralProgram program)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                GeneralProgram oProgram = new GeneralProgram
                {
                    aParent = parentId,
                    Code = program.Code,
                    ProgUrl = program.ProgUrl,
                    Terminal = program.Terminal,
                    Show = program.Show,
                    Sorting = program.Sorting,
                    Name = new GeneralResource(ModelEnum.ResourceType.STRING, program.Name)
                };
                dbEntity.GeneralPrograms.Add(oProgram);
                dbEntity.SaveChanges();
                progNodeGidEdit = oProgram.Gid;
                return RedirectToAction("Tree");
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }

        /// <summary>
        /// 程序功能节点页面
        /// </summary>
        /// <returns>程序功能节点视图</returns>
        //public ActionResult ProgramItemList()
        //{
        //    return View();
        //}

        /// <summary>
        /// 跳转到节点子项列表页面
        /// </summary>
        /// <param name="id">程序节点Guid</param>
        /// <returns>程序功能节点视图</returns>
        public ActionResult NodeIndex(Guid? id)
        {
            ViewBag.modelList = SelectEnumList(false);
            var oGeneralProgNode = dbEntity.GeneralProgNodes.Include("Name").Include("Optional").Where(p => p.ProgID == id && p.Deleted == false);
            if (id.HasValue)
            {
                ViewBag.progNodeGid = id;
                progNodeGidGrid = id.Value;
            }
            else
            {
                ViewBag.progNodeGid = progNodeGidGrid;
            }
            return View("NodeList");
        }

        /// <summary>
        /// grid显示函数
        /// </summary>
        /// <param name="searchModel">grid搜索模型</param>
        /// <returns>程序功能节点</returns>
        public ActionResult ListNodes(SearchModel searchModel)
        {
            IQueryable<GeneralProgNode> oPrograms = dbEntity.GeneralProgNodes.Include("Name").Where(p => p.Deleted == false && p.ProgID == progNodeGidGrid).AsQueryable();
            GridColumnModelList<GeneralProgNode> columns = new GridColumnModelList<GeneralProgNode>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Code);
            columns.Add(p => p.Name.GetResource(CurrentSession.Culture)).SetName("Name.Matter");
            columns.Add(p => (p.Optional == null) ? "": p.Optional.GetResource(CurrentSession.Culture)).SetName("Optional.Matter");
            GridData gridData = oPrograms.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 返回程序节点
        /// </summary>
        /// <returns>程序功能节点列表</returns>
        [HttpPost]
        public ActionResult NodeDelete()
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                Guid itemId = Guid.Parse(Request.Form["deleteItemId"]);
                GeneralProgNode oGeneralProgNode = dbEntity.GeneralProgNodes.Where(p => p.Gid == itemId).FirstOrDefault();
                if (oGeneralProgNode != null)
                {
                    oGeneralProgNode.Deleted = true;
                    dbEntity.SaveChanges();
                    progNodeGidGrid = oGeneralProgNode.ProgID;
                }
                return RedirectToAction("NodeIndex", new { id = oGeneralProgNode.ProgID });
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }

        /// <summary>
        /// 添加程序功能节点页面
        /// </summary>
        /// <returns>添加程序功能节点视图</returns>
        //public ActionResult NodeAdd()
        //{
        //    return View();
        //}

        /// <summary>
        /// 跳转添加子项
        /// </summary>
        /// <param name="progNodeGid">选中程序节点的Guid</param>
        /// <returns>程序功能节点列表</returns>
        public ActionResult NodeAdd(Guid progNodeGid)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                GeneralProgNode oGeneralProgNode = new GeneralProgNode
                {
                    Name = NewResource(ModelEnum.ResourceType.STRING),
                    Optional = NewResource(ModelEnum.ResourceType.STRING)
                };
                ViewBag.InputModeList = SelectEnumList(typeof(ModelEnum.OptionalInputMode), 1);
                oGeneralProgNode.ProgID = progNodeGid;
                return View(oGeneralProgNode);
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }

        /// <summary>
        /// 保存新建program node子项信息
        /// </summary>
        /// <param name="formCollection">程序功能节点内容</param>
        /// <returns>重定向到程序功能节点列表</returns>
        public ActionResult NodeAddSave(GeneralProgNode model)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                GeneralProgNode oGeneralProgNode = new GeneralProgNode
                {
                    ProgID = model.ProgID,
                    Code = model.Code,
                    InputMode = model.InputMode,
                    Name = new GeneralResource(ModelEnum.ResourceType.STRING, model.Name)
                };
                if (oGeneralProgNode.InputMode == (byte)ModelEnum.OptionalInputMode.COMBOBOX)
                    oGeneralProgNode.Optional = new GeneralResource(ModelEnum.ResourceType.STRING, model.Optional);
                dbEntity.GeneralProgNodes.Add(oGeneralProgNode);
                dbEntity.SaveChanges();
                return RedirectToAction("NodeIndex", new { id = model.ProgID });
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }



        /// <summary>
        /// 程序功能节点编辑
        /// </summary>
        /// <returns>编辑程序功能节点视图</returns>
        //public ViewResult ProgramItemEdit()
        //{
        //    return View();
        //}

        /// <summary>
        /// 编辑程序功能节点
        /// </summary>
        /// <param name="progItemGid">程序功能节点Guid</param>
        /// <returns>程序功能节点列表</returns>
        public ActionResult NodeEdit(Guid progItemGid)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                ViewBag.InputModeList = SelectEnumList(typeof(ModelEnum.OptionalInputMode), 1);
                GeneralProgNode oGeneralProgNode = dbEntity.GeneralProgNodes.Include("Name").Include("Optional").Where(p => p.Gid == progItemGid && p.Deleted == false).Single();
                oGeneralProgNode.Name = RefreshResource(ModelEnum.ResourceType.STRING, oGeneralProgNode.Name);
                oGeneralProgNode.Optional = RefreshResource(ModelEnum.ResourceType.STRING, oGeneralProgNode.Optional);
                return View(oGeneralProgNode);
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }

        /// <summary>
        /// 保存编辑program node子项信息
        /// </summary>
        /// <param name="formCollection">程序功能节点内容</param>       
        public void NodeEditSave(GeneralProgNode model)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                GeneralProgNode oGeneralProgNode = dbEntity.GeneralProgNodes.Where(p => p.Gid == model.Gid).Single();
                oGeneralProgNode.Code = model.Code;
                oGeneralProgNode.Name.SetResource(ModelEnum.ResourceType.STRING, model.Name);
                if (oGeneralProgNode.InputMode == (byte)ModelEnum.OptionalInputMode.TEXTBOX)
                {
                    oGeneralProgNode.InputMode = model.InputMode;
                    if (model.InputMode == (byte)ModelEnum.OptionalInputMode.COMBOBOX)
                        oGeneralProgNode.Optional = new GeneralResource(ModelEnum.ResourceType.STRING, model.Optional);
                }
                else
                {
                    oGeneralProgNode.InputMode = model.InputMode;
                    if (oGeneralProgNode.InputMode == (byte)ModelEnum.OptionalInputMode.COMBOBOX)
                        oGeneralProgNode.Optional.SetResource(ModelEnum.ResourceType.STRING, model.Optional);
                    else
                        oGeneralProgNode.Optional.Deleted = true;
                }
                dbEntity.SaveChanges();
            }
        }




        /// <summary>
        /// 删除节点子项
        /// </summary>
        /// <param name="progItemGid">程序功能节点的Guid</param>
        /// <returns>程序功能节点列表</returns>
        //public ActionResult DeleteProgItem(Guid progItemGid)
        //{
        //    GeneralProgNode oGeneralProgNode = dbEntity.GeneralProgNodes.Where(p => p.Gid == progItemGid).Single();
        //    oGeneralProgNode.Deleted = true;
        //    dbEntity.SaveChanges();           
        //    return View("ProgramItemList");
        //}


    }
}
