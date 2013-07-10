using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.IO;
using LiveAzure.Models.General;
using LiveAzure.Models.Member;
using LiveAzure.Models;
using LiveAzure.BLL;

namespace LiveAzure.Stage.Controllers
{
    public class RegionController : BaseController
    {

        private enum NameStyle { FullName, ShortName };

        #region   陆旻添加

        /// <summary>
        /// 区域
        /// </summary>
        /// <param name="successImport"></param>
        /// <returns></returns>
        public ActionResult Index(bool? successImport=false)
        {
            // 权限验证
            string strProgramCode = Request.RequestContext.RouteData.Values["Controller"].ToString() +
                Request.RequestContext.RouteData.Values["Action"].ToString();
            if (!base.Permission(strProgramCode))
                return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });

            ViewBag.EnableEdit = (base.GetProgramNode("EnableEdit") == "1") ? true : false;
            ViewBag.ImportSuccess = successImport;
            return View();
        }

        /// <summary>
        /// 加载地区树，树的名称为全称
        /// </summary>
        /// <returns></returns>
        public string TreeLoad()
        {
            return CreateTree(ListTreeNode(Guid.Empty, NameStyle.FullName));
        }

        /// <summary>
        /// 异步展开树节点的json
        /// </summary>
        /// <param name="id">展开节点的guid</param>
        /// <returns></returns>
        public string TreeExpand(Guid id)
        {
            List<LiveTreeNode> nodes = ListTreeNode(id, NameStyle.FullName);
            return nodes.ToJsonString();
        }


        /// <summary>
        /// 加载地区树，树的名称为简称
        /// </summary>
        /// <returns></returns>
        public string ShortNameTreeLoad()
        {
            return CreateTree(ListTreeNode(Guid.Empty, NameStyle.ShortName));
        }

        /// <summary>
        /// 异步展开树节点的json
        /// </summary>
        /// <param name="id">展开节点的guid</param>
        /// <returns></returns>
        public string ShortNameTreeExpand(Guid id)
        {
            List<LiveTreeNode> nodes = ListTreeNode(id, NameStyle.ShortName);
            return nodes.ToJsonString();
        }
        /// <summary>
        /// 树子结点列表
        /// </summary>
        /// <param name="id">父结点ID</param>
        /// <param name="nameStyle">地区显示类型</param>
        /// <returns>树子结点列表</returns>
        private List<LiveTreeNode> ListTreeNode(Guid id, NameStyle nameStyle)
        {
            Guid? gNodeID = null;
            // 当展开root节点的时候
            if (id != Guid.Empty)
                gNodeID = id;
            List<GeneralRegion> oRegions = (from r in dbEntity.GeneralRegions.Include("ChildItems")
                                            where (gNodeID == null ? r.aParent == null : r.aParent == gNodeID)
                                               && (r.Deleted == false)
                                            orderby r.Sorting descending
                                            select r).ToList();
            Func<GeneralRegion, string> GetName;
            if (nameStyle == NameStyle.FullName)
                GetName = (item => item.FullName);
            else
                GetName = (item => item.ShortName);
            List<LiveTreeNode> list = (from item in oRegions
                                       select new LiveTreeNode
                                       {
                                           id = item.Gid.ToString(),
                                           name = GetName(item),
                                           isParent = (item.ChildCount > 0)
                                       }).ToList();
            return list;
        }

        /// <summary>
        /// 通过删除树节点来删除地区
        /// </summary>
        /// <param name="id">删除结点的id</param>
        public void TreeRemove(Guid id)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                GeneralRegion oGeneralRegion = dbEntity.GeneralRegions.Include("ChildItems").Where(p => p.Gid == id).FirstOrDefault();
                if (oGeneralRegion != null)
                {
                    oGeneralRegion.Deleted = true;
                    DeleteChildRegion(oGeneralRegion.ChildItems.ToList());
                    dbEntity.SaveChanges();
                }
            }
            else
            {
                RedirectToAction("ErrorPage", "Home", new { message = @LiveAzure.Resource.Common.NoPermission });
            }
        }

        /// <summary>
        /// 递归删除地区以及其下属的地区
        /// </summary>
        /// <param name="list">下属地区的列表</param>
        public void DeleteChildRegion(List<GeneralRegion> list)
        {
            int iListCount = list.Count;
            for (int i = 0; i < iListCount; i++)
            {
                //如果存在下属地区，删除该地区，然后调用递归删除下属地区；否则删除该地区;
                if (list.ElementAt(i).ChildItems.Count > 0)
                {
                    list.ElementAt(i).Deleted = true;
                    DeleteChildRegion(list.ElementAt(i).ChildItems.ToList());
                }
                else
                {
                    list.ElementAt(i).Deleted = true;
                }
            }
        }

        /// <summary>
        /// 从区域主页面跳转到添加地区页面
        /// </summary>
        /// <param name="id">新建地区的父节点Guid</param>
        /// <returns></returns>
        public ActionResult AddNewRegion(Guid id)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                GeneralRegion oGeneralRegion = new GeneralRegion();
                if (id.Equals(Guid.Empty) || id.Equals(null))
                {
                    oGeneralRegion.aParent = null;
                }
                else
                {
                    GeneralRegion oParentRegion = dbEntity.GeneralRegions.Where(p => p.Gid == id && p.Deleted == false).FirstOrDefault();
                    oGeneralRegion.aParent = id;
                    oGeneralRegion.Parent = oParentRegion;
                }
                return View("RegionAdd", oGeneralRegion);
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }

        /// <summary>
        /// 将新建的地区添加入数据库，返回地区树形显示界面
        /// </summary>
        /// <param name="regionFormCollection">页面form集合</param>
        /// <returns></returns>
        public ActionResult AddRegion(Guid? parentId, GeneralRegion model)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                var oGeneralRegion = (from r in dbEntity.GeneralRegions
                                      where r.aParent == parentId && r.FullName == model.FullName
                                      select r).FirstOrDefault();
                if (oGeneralRegion == null)
                {
                    oGeneralRegion = new GeneralRegion();
                    dbEntity.GeneralRegions.Add(oGeneralRegion);
                }
                oGeneralRegion.Deleted = false;      // 如果以前删除了，可以恢复；如果重名了，则覆盖
                oGeneralRegion.Code = model.Code;
                oGeneralRegion.FullName = model.FullName;
                oGeneralRegion.ShortName = model.ShortName;
                oGeneralRegion.Sorting = model.Sorting;
                oGeneralRegion.RegionLevel = model.RegionLevel;
                oGeneralRegion.Remark = model.Remark;
                oGeneralRegion.aParent = parentId;
                dbEntity.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }

        /// <summary>
        /// 跳转到编辑
        /// </summary>
        /// <param name="id">需要编辑地区的Guid</param>
        /// <returns></returns>
        public ActionResult RegionEdit(Guid id) 
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                try
                {
                    GeneralRegion oGeneralRegion = new GeneralRegion();
                    oGeneralRegion = dbEntity.GeneralRegions.Include("Parent").Where(p => p.Gid == id).Single();
                    return View("RegionEdit", oGeneralRegion);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
                }
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }

        /// <summary>
        /// 保存修改的地区
        /// </summary>
        /// <param name="id">修改地区的Guid</param>
        /// <returns></returns>
        public ActionResult EditRegion(Guid id, GeneralRegion model)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                GeneralRegion oGeneralRegion = dbEntity.GeneralRegions.Where(p => p.Gid == id).Single();
                if (oGeneralRegion != null)
                {
                    oGeneralRegion.Code = model.Code;
                    oGeneralRegion.FullName = model.FullName;
                    oGeneralRegion.ShortName = model.ShortName;
                    oGeneralRegion.Sorting = model.Sorting;
                    oGeneralRegion.RegionLevel = model.RegionLevel;
                    oGeneralRegion.Remark = model.Remark;
                    dbEntity.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }

        /// <summary>
        /// 导入地区
        /// </summary>
        /// <returns></returns>
        public ActionResult RegionImport()
        {
            if (base.GetProgramNode("EnableEdit") == "1")
                return View();
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }

        /// <summary>
        /// 导入地区，提交后
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public ActionResult ImportRegion()
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                DataTransferBLL oTransfer = new DataTransferBLL(dbEntity);
                HttpPostedFileBase hpfChina = Request.Files["ImportChina"];
                HttpPostedFileBase hpfUSA = Request.Files["ImportUSA"];
                HttpPostedFileBase hpfEurope = Request.Files["ImportEurope"];

                string sLocalFile, sRemoteFile, sExtension, sFullFilePath;
                string sServerPath = HttpContext.Server.MapPath("~/Temp");
                if (!Directory.Exists(sServerPath))
                    Directory.CreateDirectory(sServerPath);
                if (hpfChina != null && hpfChina.ContentLength > 0)
                {
                    sLocalFile = Path.GetFileName(hpfChina.FileName);
                    sExtension = Path.GetExtension(sLocalFile);
                    sRemoteFile = Guid.NewGuid() + sExtension;
                    sFullFilePath = Path.Combine(sServerPath, sRemoteFile);
                    hpfChina.SaveAs(sFullFilePath);
                    oTransfer.ImportChinaRegions(sFullFilePath, "");
                    System.IO.File.Delete(sFullFilePath);  // 删除临时文件
                }

                return RedirectToAction("Index", new {successImport=true});
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }

        #endregion

    }
}
