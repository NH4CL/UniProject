using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveAzure.Models.Member;
using LiveAzure.Models;
using LiveAzure.Models.General;
using MVC.Controls;
using MVC.Controls.Grid;
using LiveAzure.Models.Warehouse;

namespace LiveTest.Weinan.Controllers
{
    public class PrivilegeController : BaseController
    {
        //
        // GET: /Privilege/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrivilegeDefinition(MemberPrivilege memberPrivilege)
        {
            return View();
        }

        /// <summary>
        /// 新建用户组织授权
        /// </summary>
        /// <returns></returns>
        public string PrivOrgDefinition(MemberPrivilege memPriv, Guid? userID)
        {
            memPriv.User = dbEntity.MemberUsers.Where(o => o.Gid == userID).FirstOrDefault();
            memPriv.Ptype = 2;
            memPriv.Pstatus = 1;
            //if(Request.Form["Pstatus"]==null)
            //{
            //    memPriv.Pstatus = 0;
            //}
            //else 
            //{
            //    memPriv.Pstatus = 1;
            //}
            if (ModelState.IsValid)
            {
                try
                {
                    dbEntity.MemberPrivileges.Add(memPriv);
                    dbEntity.SaveChanges();
                    return memPriv.Gid.ToString();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException)//数据库中已存在该条记录
                {
                    var id = dbEntity.MemberPrivileges.Where(o => o.UserID==userID&&o.Ptype==2).Select(o=>o.Gid).FirstOrDefault();
                    return id.ToString();
                }
            }
            else {
                return null;
            }
            
        }

        /// <summary>
        /// 验证输入的用户名是否存在
        /// </summary>
        /// <param name="strUserName"></param>
        /// <returns></returns>
        public string checkUser(string strUserName)
        {
            string strUserID = "";
            byte nResult = 4;
            //如果接收的用户名为空，返回0
            if (String.IsNullOrEmpty(strUserName))
            {
                nResult = 0;
            }
            else
            {
                //创建一个User实例
                MemberUser user = new MemberUser();
                //验证用户名
                try
                {
                    user = dbEntity.MemberUsers.Where(
                        c => c.LoginName == strUserName && c.Deleted == false && c.Ustatus == (byte)ModelEnum.UserStatus.VALID)
                        .FirstOrDefault();
                    strUserID = user.Gid.ToString();
                    Session["userID"] = strUserID;
                    //ViewData["userID"] = userID;
                    //if (user == null)
                    //{
                    //    nResult = 0;
                    //}
                }
                catch (Exception)
                {
                    user = null;
                    nResult = 1;
                }
            }
            return strUserID;
        }

        /// <summary>
        /// 组织授权列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListPrivOrganization(SearchModel searchModel)
        {
            IQueryable<MemberOrganization> organs = (from o in dbEntity.MemberOrganizations
                                                     from p in dbEntity.MemberPrivItems
                                                     where (o.Deleted == false && o.Otype == 0 && p.Deleted == false && o.Gid == p.RefID)
                                                     select o).AsQueryable();
            GridColumnModelList<MemberOrganization> columns = new GridColumnModelList<MemberOrganization>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Code);
            columns.Add(p => p.ShortName.Matter);

            GridData gridData = organs.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 用户组织授权
        /// </summary>
        /// <returns></returns>
        public ActionResult PrivOrganization(MemberPrivilege memberPrivilege)
        {
            var memberOrganizations = from o in dbEntity.MemberOrganizations.Include("ShortName")
                                      where (o.Deleted == false && o.Otype == 0)
                                      select new
                                      {
                                          Gid = o.Gid,
                                          ShortName = o.ShortName.Matter
                                      };
            ViewBag.memOrg = new SelectList(memberOrganizations, "Gid", "ShortName");
            return View();
        }

        /// <summary>
        /// 保存Ptype=2，即授权组织信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PrivOrganization(MemberPrivilege memberPrivilege, int? optitemCount, string prvID)
        {
            Guid id = new Guid(prvID);
            memberPrivilege = (dbEntity.MemberPrivileges.Where(o => o.Gid == id)).Single();
            if (ModelState.IsValid)
            {
                ICollection<MemberPrivItem> memberPrivItems = new List<MemberPrivItem>();
                for (int i = 0; i < optitemCount; i++)
                {
                    MemberPrivItem memberPrivItem = new MemberPrivItem();
                    memberPrivItem.PrivID = memberPrivilege.Gid;
                    memberPrivItem.RefID = new Guid(Request.Form["privOrg" + i]);
                    memberPrivItems.Add(memberPrivItem);
                }
                memberPrivilege.PrivilegeItems = memberPrivItems;

                dbEntity.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        /// <summary>
        /// 新建用户渠道授权
        /// </summary>
        /// <returns></returns>
        public string PrivChanDefinition(MemberPrivilege memPriv, Guid? userID)
        {
            memPriv.User = dbEntity.MemberUsers.Where(o => o.Gid == userID).FirstOrDefault();
            memPriv.Ptype = 3;
            memPriv.Pstatus = 1;
            //if(Request.Form["Pstatus"]==null)
            //{
            //    memPriv.Pstatus = 0;
            //}
            //else 
            //{
            //    memPriv.Pstatus = 1;
            //}
            if (ModelState.IsValid)
            {
                try
                {
                    dbEntity.MemberPrivileges.Add(memPriv);
                    dbEntity.SaveChanges();
                    return memPriv.Gid.ToString();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException)//数据库中已存在该条记录
                {
                    var id = dbEntity.MemberPrivileges.Where(o => o.UserID == userID && o.Ptype == 3).Select(o => o.Gid).FirstOrDefault();
                    return id.ToString();
                }
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// 渠道授权列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListPrivChannel(SearchModel searchModel)
        {
            IQueryable<MemberChannel> organs = (from o in dbEntity.MemberChannels.Include("ShortName")
                                                     from p in dbEntity.MemberPrivItems
                                                     where (o.Deleted == false && o.Otype == 1 && p.Deleted == false && o.Gid == p.RefID)
                                                     select o).AsQueryable();
            GridColumnModelList<MemberChannel> columns = new GridColumnModelList<MemberChannel>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Code);
            columns.Add(p => p.ShortName.Matter);

            GridData gridData = organs.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 用户渠道授权
        /// </summary>
        /// <returns></returns>
        public ActionResult PrivChannel(MemberPrivilege memberPrivilege)
        {
            //选择Otype=1的渠道组织
            var memberChannels = from o in dbEntity.MemberChannels.Include("ShortName")
                                      where (o.Deleted == false && o.Otype == 1)
                                      select new
                                      {
                                          Gid = o.Gid,
                                          ShortName = o.ShortName.Matter
                                      };
            ViewBag.memOrg = new SelectList(memberChannels, "Gid", "ShortName");
            return View();
        }

        /// <summary>
        /// 保存Ptype=3，即授权渠道信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PrivChannel(MemberPrivilege memberPrivilege, int? optitemCount, string prvID)
        {
            Guid id = new Guid(prvID);
            memberPrivilege = (dbEntity.MemberPrivileges.Where(o => o.Gid == id)).Single();
            if (ModelState.IsValid)
            {
                ICollection<MemberPrivItem> memberPrivItems = new List<MemberPrivItem>();
                for (int i = 0; i < optitemCount; i++)
                {
                    MemberPrivItem memberPrivItem = new MemberPrivItem();
                    memberPrivItem.PrivID = memberPrivilege.Gid;
                    memberPrivItem.RefID = new Guid(Request.Form["privOrg" + i]);
                    memberPrivItems.Add(memberPrivItem);
                }
                memberPrivilege.PrivilegeItems = memberPrivItems;

                dbEntity.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        /// <summary>
        /// 新建用户仓库授权
        /// </summary>
        /// <returns></returns>
        public string PrivWareDefinition(MemberPrivilege memPriv, Guid? userID)
        {
            memPriv.User = dbEntity.MemberUsers.Where(o => o.Gid == userID).FirstOrDefault();
            memPriv.Ptype = 4;
            memPriv.Pstatus = 1;
            //if(Request.Form["Pstatus"]==null)
            //{
            //    memPriv.Pstatus = 0;
            //}
            //else 
            //{
            //    memPriv.Pstatus = 1;
            //}
            if (ModelState.IsValid)
            {
                try
                {
                    dbEntity.MemberPrivileges.Add(memPriv);
                    dbEntity.SaveChanges();
                    return memPriv.Gid.ToString();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException)//数据库中已存在该条记录
                {
                    var id = dbEntity.MemberPrivileges.Where(o => o.UserID == userID && o.Ptype == 4).Select(o => o.Gid).FirstOrDefault();
                    return id.ToString();
                }
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// 仓库授权列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListPrivWarehouse(SearchModel searchModel)
        {
            IQueryable<WarehouseInformation> organs = (from o in dbEntity.WarehouseInformations.Include("ShortName")
                                                     from p in dbEntity.MemberPrivItems
                                                     where (o.Deleted == false && o.Otype == 2 && p.Deleted == false && o.Gid == p.RefID)
                                                     select o).AsQueryable();
            GridColumnModelList<WarehouseInformation> columns = new GridColumnModelList<WarehouseInformation>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Code);
            columns.Add(p => p.ShortName.Matter);

            GridData gridData = organs.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 用户仓库授权
        /// </summary>
        /// <returns></returns>
        public ActionResult PrivWarehouse(MemberPrivilege memberPrivilege)
        {
            //选择Otype=2的仓库组织
            var memberWarehouses = from o in dbEntity.WarehouseInformations.Include("ShortName")
                                      where (o.Deleted == false && o.Otype == 2)
                                      select new
                                      {
                                          Gid = o.Gid,
                                          ShortName = o.ShortName.Matter
                                      };
            ViewBag.memOrg = new SelectList(memberWarehouses, "Gid", "ShortName");
            return View();
        }

        /// <summary>
        /// 保存Ptype=4，即授权仓库信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PrivWarehouse(MemberPrivilege memberPrivilege, int? optitemCount, string prvID)
        {
            Guid id = new Guid(prvID);
            memberPrivilege = (dbEntity.MemberPrivileges.Where(o => o.Gid == id)).Single();
            if (ModelState.IsValid)
            {
                ICollection<MemberPrivItem> memberPrivItems = new List<MemberPrivItem>();
                for (int i = 0; i < optitemCount; i++)
                {
                    MemberPrivItem memberPrivItem = new MemberPrivItem();
                    memberPrivItem.PrivID = memberPrivilege.Gid;
                    memberPrivItem.RefID = new Guid(Request.Form["privOrg" + i]);
                    memberPrivItems.Add(memberPrivItem);
                }
                memberPrivilege.PrivilegeItems = memberPrivItems;

                dbEntity.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        

        /// <summary>
        /// 程序&程序节点授权
        /// </summary>
        /// <returns></returns>
        public ActionResult PrivProgram()
        {
            int  nstatus;
            Guid userID = new Guid("12db0a77-92c4-e011-afef-002269c9aa85");
            MemberPrivilege PrivID = (from o in dbEntity.MemberPrivileges
                           where (o.Deleted == false && o.UserID == userID&&o.Ptype==0)
                           select o).FirstOrDefault();
            if (PrivID != null)
            {
                nstatus =(int)PrivID.Pstatus;                
            }
            else nstatus = -1;
            ViewBag.status = nstatus;
            return View();
        }
        /// <summary>
        /// 加载程序树形结构
        /// </summary>
        /// <returns></returns>
        public string PrivProgramLoad()
        {
            var ProgList = from o in dbEntity.GeneralPrograms.Include("Name")
                           where (o.Parent == null && o.Deleted == false)
                           select o;
            var CheckList = (from o in dbEntity.GeneralPrograms
                            from p in dbEntity.MemberPrivItems
                            where (o.Parent == null && o.Deleted == false&& p.Deleted == false&&o.Gid==p.RefID)
                            select  new { Gid=p.RefID }).ToList();
            int nCount = ProgList.ToList().Count;
            int nCheck = CheckList.Count;
            List<LiveTreeNode> list = new List<LiveTreeNode>();
                
                foreach (var item in ProgList)
                {
                    //bool flag = false;
                    LiveTreeNode treeNode = new LiveTreeNode();
                    //for (int i = 0; i < CheckList.Count; i++)
                    //{
                    //    if (item.Gid == CheckList[i].Gid)
                    //    {
                    //        treeNode.nodeChecked = true;
                    //        flag = true;
                    //        break;
                    //    }
                    //}
                    //if(flag == false)
                        treeNode.nodeChecked = false;
                        treeNode.id = item.Gid.ToString();
                        //treeNode.name = item.name.GetResource(CurrentSession.Culture);
                        treeNode.name = item.Name.Matter;
                        
                        treeNode.nodes = new List<LiveTreeNode>();
                        treeNode.progUrl = item.ProgUrl;
                        list.Add(treeNode);
                    
                    
                }
               string strTreeJson = CreateTree(list);                
                       
           return strTreeJson;
        }
        /// <summary>
        /// 展开程序树节点
        /// </summary>
        /// <param name="id">展开节点的id</param>
        /// <returns></returns>
        public string PrivProgramExpand(Guid id)
        {
            string strTreeJson = "";

            //当展开root节点的时候
            if (id.ToString().Equals("00000000-0000-0000-0000-000000000000"))
            {

                var ProgList = from o in dbEntity.GeneralPrograms.Include("Name")
                               where (o.Parent == null && o.Deleted == false)
                               select o;
                var CheckList = (from o in dbEntity.GeneralPrograms
                                 from p in dbEntity.MemberPrivItems
                                 where (o.Parent == null && o.Deleted == false && p.Deleted == false && o.Gid == p.RefID)
                                 select new { Gid = p.RefID }).ToList();
                int nCount = ProgList.ToList().Count;
                int nCheck = CheckList.Count;
                List<LiveTreeNode> list = new List<LiveTreeNode>();

                foreach (var item in ProgList)
                {
                //    bool flag = false;
                    LiveTreeNode treeNode = new LiveTreeNode();
                //    for (int i = 0; i < CheckList.Count; i++)
                //    {
                //        if (item.Gid == CheckList[i].Gid)
                //        {
                //            treeNode.nodeChecked = true;
                //            flag = true;
                //            break;
                //        }
                //    }
                //    if (flag == false)
                        treeNode.nodeChecked = false;
                    treeNode.id = item.Gid.ToString();
                    //treeNode.name = item.name.GetResource(CurrentSession.Culture);
                    treeNode.name = item.Name.Matter;

                    treeNode.nodes = new List<LiveTreeNode>();
                    treeNode.progUrl = item.ProgUrl;
                    list.Add(treeNode);
                }
                strTreeJson = CreateTreeJson(list, "");
            }
            else                                                          //非root节点展开的时候，回传的gid不为空
            {                               
                GeneralProgram Pro = (from o in dbEntity.GeneralPrograms where (o.Gid == id && o.Deleted == false) select o).FirstOrDefault();
                //展开程序节点
                if (Pro != null)
                {
                    //展开叶子程序的功能节点
                    if (Pro.Terminal == true)
                    {
                        //该程序的功能节点
                        var ProgNode = from o in dbEntity.GeneralProgNodes.Include("Name").Include("Optional")
                                       where (o.ProgID == id && o.Deleted == false)
                                       select o;
                        int nProNode = ProgNode.ToList().Count;
                        List<LiveTreeNode> list = new List<LiveTreeNode>();
                        foreach (var Nodeitem in ProgNode)
                        {
                            //bool flag = false;
                            LiveTreeNode treeNode = new LiveTreeNode();
                            //for (int i = 0; i < nPrivProgNode;i++ )
                            //{
                            //    if (Nodeitem.Gid == PrivProgNode[i].Gid)
                            //        treeNode.nodeChecked = true;
                            //    flag = true;
                            //    break;

                            //}
                            //if (flag == false) 
                            treeNode.nodeChecked = false;
                            treeNode.id = Nodeitem.Gid.ToString();
                            treeNode.name = string.Concat(Nodeitem.Name.Matter,
                               "  " + Nodeitem.InputMode.ToString());
                            if (Nodeitem.InputMode == 0)
                                treeNode.progUrl = "";
                            else treeNode.progUrl = "";
                            treeNode.nodes = new List<LiveTreeNode>();
                            list.Add(treeNode);
                        }
                        strTreeJson = CreateTreeJson(list, "");
                    }

                    else
                    {
                        var ProgChildList = from o in dbEntity.GeneralPrograms.Include("Name").Include("Parent")
                                            where (o.Parent.Gid == id && o.Deleted == false)
                                            select o;
                        var CheckList = (from o in dbEntity.GeneralPrograms.Include("Parent")
                                         from p in dbEntity.MemberPrivItems
                                         where (o.Parent.Gid == id && o.Deleted == false && p.Deleted == false && o.Gid == p.RefID)
                                         select new { Gid = p.RefID }).ToList();
                        List<LiveTreeNode> list = new List<LiveTreeNode>();
                        foreach (var item in ProgChildList)
                        {
                            bool flag = false;
                            LiveTreeNode treeNode = new LiveTreeNode();
                            for (int i = 0; i < CheckList.Count; i++)
                            {
                                if (item.Gid == CheckList[i].Gid)
                                {
                                    treeNode.nodeChecked = true;
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag == false)
                                treeNode.nodeChecked = false;
                            treeNode.id = item.Gid.ToString();
                            //treeNode.name = item.name.GetResource(CurrentSession.Culture);
                            treeNode.name = item.Name.Matter;
                            treeNode.nodes = new List<LiveTreeNode>();
                            treeNode.progUrl = item.ProgUrl;
                            list.Add(treeNode);

                        }
                        strTreeJson = CreateTreeJson(list, "");
                    }

                }
                else
                {
                    return null;
                }
            }
            return "[" + strTreeJson + "]";
        }
        /// <summary>
        /// 加载已授权树
        /// </summary>
        /// <returns></returns>
        public string SubProgramLoad()
        {
            string strTreeJson = "";
            Guid userID = new Guid("12db0a77-92c4-e011-afef-002269c9aa85");
            //MemberPrivilege表中选出类型为程序授权的记录
            var PrivID= (from o in dbEntity.MemberPrivileges 
                        where(o.UserID==userID&&o.Deleted==false&&o.Ptype==0)
                        select o).FirstOrDefault();
            //若为空标志为对该用户进行授权，不生成树
            if (PrivID == null) return strTreeJson;
            else
            {
                //MemberPrivItems表中选择所有授权的Program
                var oProgramList = (from o in dbEntity.MemberPrivItems
                                   where (o.PrivID == PrivID.Gid && o.Deleted == false)
                                   select o.RefID).ToList();
                List<LiveTreeNode> list = new List<LiveTreeNode>();
                for (int i = 0; i < oProgramList.Count; i++)
                {
                    Guid oGid = new Guid(oProgramList[i].ToString());
                    
                    var oProgram =( from o in dbEntity.GeneralPrograms
                                    where (o.Gid == oGid && o.Deleted == false)
                                select o).Single();
                    //只加载父亲为空的节点
                    if (oProgram.Parent == null)
                    {
                        LiveTreeNode oTreeNode = new LiveTreeNode();
                        oTreeNode.id = oProgram.Gid.ToString();
                        oTreeNode.name = oProgram.Name.Matter;
                        oTreeNode.nodeChecked= false;
                        oTreeNode.progUrl = oProgram.ProgUrl;
                        oTreeNode.nodes = new List<LiveTreeNode>();
                        list.Add(oTreeNode);
                    }
                }
                strTreeJson = CreateTree(list);
                return strTreeJson;
            }
           
        }
        /// <summary>
        /// 展开已授权树节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string SubProgramExpand(Guid id)
        {
            string strTreeJson = "";
            Guid userID = new Guid("12db0a77-92c4-e011-afef-002269c9aa85");
            //MemberPrivilege表中选出类型为程序授权的记录
            var PrivID = (from o in dbEntity.MemberPrivileges
                          where (o.UserID == userID && o.Deleted == false && o.Ptype == 0)
                          select o).FirstOrDefault();           
            
            List<LiveTreeNode> list = new List<LiveTreeNode>();
            if (id.ToString().Equals("00000000-0000-0000-0000-000000000000"))
            {
                //MemberPrivItems表中选择所有授权的Program
                var oProgramList = (from o in dbEntity.MemberPrivItems
                                    where (o.PrivID == PrivID.Gid && o.Deleted == false)
                                    select o.RefID).ToList();
                for (int i = 0; i < oProgramList.Count; i++)
                {
                    Guid oGid = new Guid(oProgramList[i].ToString());

                    var oProgram = (from o in dbEntity.GeneralPrograms
                                    where (o.Gid == oGid && o.Deleted == false)
                                    select o).Single();
                    
                    //只加载父亲为空的节点
                    if (oProgram.Parent == null)
                    {
                        LiveTreeNode oTreeNode = new LiveTreeNode();
                        oTreeNode.id = oProgram.Gid.ToString();
                        oTreeNode.name = oProgram.Name.Matter;
                        oTreeNode.nodeChecked = false;
                        oTreeNode.progUrl = oProgram.ProgUrl;
                        oTreeNode.nodes = new List<LiveTreeNode>();
                        list.Add(oTreeNode);
                    }
                }
                strTreeJson = CreateTreeJson(list, "");
                return "["+ strTreeJson+"]";

            }
            else
            {
                bool flag=false;
                Guid Pid = PrivID.Gid;
                var oProgram = (from o in dbEntity.GeneralPrograms.Include("Parent")
                                where (o.Parent.Gid == id && o.Deleted == false)
                                select o).ToList();
                var oRef = (from o in dbEntity.MemberPrivItems
                           where (o.PrivID == Pid && o.Deleted ==false)
                           select o).ToList();
                
                for (int i = 0; i < oProgram.Count; i++)
                {
                    for (int j = 0; i < oRef.Count; j++)
                    {
                        if (oProgram[i].Gid == oRef[j].RefID)
                            flag = true;
                        break;
                    }
                    if (flag == false)
                    {
                        LiveTreeNode oTreeNode = new LiveTreeNode();
                        oTreeNode.id = oProgram[i].Gid.ToString();
                        oTreeNode.name = oProgram[i].Name.Matter;
                        oTreeNode.nodeChecked = false;
                        oTreeNode.progUrl = oProgram[i].ProgUrl;
                        oTreeNode.nodes = new List<LiveTreeNode>();
                        list.Add(oTreeNode);
                    }

                    flag = false;
                }
                strTreeJson = CreateTreeJson(list, "");
                return "[" + strTreeJson + "]";
            }
        }
        /// <summary>
        /// 保存程序授权内容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PrivProgram(string result, string open, string Editresult,string Dropresult)
        {            
            string[] gid = result.Split(',');
            string[] openstate = open.Split(',');
            Guid priid ;
            Guid priidnode;
            
            //userID为前台传入值，现在假设已获取
            Guid userID = new Guid("12db0a77-92c4-e011-afef-002269c9aa85");
            MemberPrivilege PrivID = (from o in dbEntity.MemberPrivileges
                                      where (o.UserID == userID && o.Deleted == false && o.Ptype == 0)
                                      select o).FirstOrDefault();
            //保存程序授权，Ptype=0
            if (PrivID != null)
            {
                priid = PrivID.Gid;
            }
            else
            {
                //程序授权Ptype=0，主表中添加
                MemberPrivilege m = new MemberPrivilege();
                m.UserID = userID;
                m.Ptype = 0;
                m.Pstatus = 1;

                dbEntity.MemberPrivileges.Add(m);
                dbEntity.SaveChanges();
                priid= m.Gid;
            }
            MemberPrivilege NodePrivID = (from o in dbEntity.MemberPrivileges
                                      where (o.UserID == userID && o.Deleted == false && o.Ptype == 1)
                                      select o).FirstOrDefault();
            if (NodePrivID != null)
            {
                priidnode = NodePrivID.Gid;
            }
            else
            {
                //程序功能点授权，Ptype=1
                    MemberPrivilege m = new MemberPrivilege();
                    m.UserID = userID;
                    m.Ptype = 1;
                    m.Pstatus = 1;

                    dbEntity.MemberPrivileges.Add(m);
                    dbEntity.SaveChanges();
                    priidnode = m.Gid;
            }

            //保存授权的程序或程序节点
            for (int i = 1; i < gid.Length - 1; i++)
            {
                Guid id = new Guid(gid[i]);
                //判断是否是程序的ID
                var IsProgram = (from o in dbEntity.GeneralPrograms
                                 where (o.Gid == id && o.Deleted == false)
                                 select o).FirstOrDefault();
                //gid[i]为程序节点ID
                if (IsProgram == null)
                {
                    var ItemNode = (from o in dbEntity.MemberPrivItems
                                    where (o.RefID == id && o.Deleted == false)
                                    select o).FirstOrDefault();
                    if (ItemNode == null)
                    {
                        GeneralProgNode ProgNode = (from o in dbEntity.GeneralProgNodes.Include("Name").Include("Optional")
                                                    where (o.Gid == id && o.Deleted == false)
                                                    select o).Single();

                        MemberPrivItem mPriItem = new MemberPrivItem();
                        mPriItem.PrivID = priid;
                        mPriItem.RefID = new Guid(gid[i]);
                        mPriItem.NodeCode = ProgNode.Code;
                        if (ProgNode.InputMode == 0)
                            mPriItem.NodeValue = Editresult;
                        else mPriItem.NodeValue = Dropresult;
                        dbEntity.MemberPrivItems.Add(mPriItem);
                        dbEntity.SaveChanges();
                    }

                }
                //gid[i]是程序ID
                else
                {
                    var ItemProg = (from o in dbEntity.MemberPrivItems
                                    where (o.Deleted == false && o.RefID == id)
                                    select o).FirstOrDefault();
                    if (ItemProg == null)
                    {
                        GeneralProgram Program = (from o in dbEntity.GeneralPrograms
                                                  where (o.Gid == id&&o.Deleted==false)
                                                  select o).Single();
                        //程序为叶子节点
                        if (Program.Terminal == true)
                        {
                            if (openstate[i] == "undefined")
                            {
                                //保存自己
                                MemberPrivItem mPriItem = new MemberPrivItem();
                                mPriItem.PrivID = priid;
                                mPriItem.RefID = new Guid(gid[i]);
                                dbEntity.MemberPrivItems.Add(mPriItem);
                                dbEntity.SaveChanges();
                                //保存所有功能节点
                                var ProgNode = from o in dbEntity.GeneralProgNodes
                                               where (o.ProgID == id && o.Deleted == false)
                                               select o;
                                foreach (var Nodeitem in ProgNode)
                                {
                                    mPriItem.PrivID = priid;
                                    mPriItem.RefID = Nodeitem.Gid;
                                    mPriItem.NodeCode = Nodeitem.Code;
                                    if (Nodeitem.InputMode == 0)
                                        mPriItem.NodeValue = Editresult;
                                    else mPriItem.NodeValue = Dropresult;
                                }
                            }
                        }
                        //程序为非叶子节点
                        else
                        {
                            if (openstate[i] == "undefined")
                            {
                                //判断是否已经存在
                                var item = (from o in dbEntity.MemberPrivItems
                                            where (o.RefID == id && o.PrivID == priid)
                                            select o).FirstOrDefault();

                                if (item == null)
                                {
                                    MemberPrivItem mPriItem = new MemberPrivItem();
                                    mPriItem.PrivID = priid;
                                    mPriItem.RefID = new Guid(gid[i]);
                                    dbEntity.MemberPrivItems.Add(mPriItem);
                                    dbEntity.SaveChanges();
                                }
                                //递归保存孩子节点                    
                                PrivProChild(id, priid);
                            }
                            else
                            {
                                var item = (from o in dbEntity.MemberPrivItems
                                            where (o.RefID == id && o.PrivID == priid)
                                            select o).FirstOrDefault();
                                if (item == null)
                                {
                                    MemberPrivItem mPriItem = new MemberPrivItem();
                                    mPriItem.PrivID = priid;
                                    mPriItem.RefID = new Guid(gid[i]);
                                    dbEntity.MemberPrivItems.Add(mPriItem);
                                    dbEntity.SaveChanges();
                                }
                            }
                        }
                    }

                }
            }
            
           return RedirectToAction("PrivProgram");
        }
        /// <summary>
        /// 递归保存子节点
        /// </summary>
        /// <param name="ID">父亲节点ID</param>
        /// <param name="PrivID"></param>
        public void PrivProChild(Guid ID, Guid PrivID)
        {
            bool flag=false;
            var ProgramList = (from o in dbEntity.GeneralPrograms.Include("Parent")
                          where (o.Parent.Gid == ID &&  o.Deleted == false)
                          select o).ToList();
            var refidlist=(from o in dbEntity.MemberPrivItems where (o.PrivID==PrivID)
                           select o).ToList();
            for (int i = 0; i < ProgramList.Count; i++)
            {
                for (int j = 0; j < refidlist.Count; j++)
                {
                    if (ProgramList[i].Gid == refidlist[j].RefID)
                        flag = true;
                    break;
                }
                if (flag == false)
                {
                    if (ProgramList.ElementAt(i).Terminal == true)
                    {
                        MemberPrivItem mPrivItem = new MemberPrivItem();
                        mPrivItem.PrivID = PrivID;
                        mPrivItem.RefID = ProgramList.ElementAt(i).Gid;
                        dbEntity.MemberPrivItems.Add(mPrivItem);
                        dbEntity.SaveChanges();
                        //是否存在程序功能节点
                        var ProgNode = (from o in dbEntity.GeneralProgNodes
                                       where (o.Deleted == false && o.ProgID == ProgramList[i].Gid)
                                       select o).ToList();
                        if (ProgNode.Count>0)
                        {
                            foreach (var Nodeitem in ProgNode)
                            {
                                MemberPrivItem NodeItem = new MemberPrivItem();
                                NodeItem.PrivID = PrivID;
                                NodeItem.RefID = ProgNode[i].Gid;
                                dbEntity.MemberPrivItems.Add(NodeItem);
                            }
                            dbEntity.SaveChanges();
                        }
                    }
                    else
                    {
                        PrivProChild(ProgramList[i].Gid, PrivID);

                    }
                }
                flag = false;
            }
        }
        /// <summary>
        /// 删除已授权的树节点
        /// </summary>
        /// <param name="result">Gid字符串</param>
        /// <param name="open">树节点打开的状态</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteProgrom(string result, string open)
        {
            string[] gid = result.Split(',');
            string[] openstate = open.Split(',');
            //Guid priid;
            //userID为前台传入值，现在假设已获取
            Guid userID = new Guid("12db0a77-92c4-e011-afef-002269c9aa85");
            Guid PrivID = (from o in dbEntity.MemberPrivileges
                           where (o.Deleted == false && o.UserID == userID && o.Ptype == 0)
                           select o.Gid).Single();
            for(int i=1;i<gid.Length-1;i++)
            {
                 Guid id=new Guid (gid[i]);
                 if (openstate[i] == "undefined")
                 {
                     var IsProgram = (from o in dbEntity.GeneralPrograms
                                      where (o.Deleted == false && o.Gid == id)
                                      select o).FirstOrDefault();
                     //程序节点
                     if (IsProgram == null)
                     {
                         MemberPrivItem Node = (from o in dbEntity.MemberPrivItems
                                                 where (o.Deleted == false && o.RefID == id)
                                                 select o).Single();
                         Node.Deleted = true;
                         dbEntity.SaveChanges();
                     }
                         //程序
                     else
                     {
                         if (IsProgram.Terminal == true)
                         {
                             var ProgNode = from o in dbEntity.GeneralProgNodes
                                            where (o.Deleted == false && o.ProgID == id)
                                            select o;
                             foreach (var item in ProgNode)
                             {
                                 MemberPrivItem PrivNode = (from o in dbEntity.MemberPrivItems
                                                            where (o.RefID == item.Gid && o.Deleted == false)
                                                            select o).Single();
                                 PrivNode.Deleted = true;
                             }
                             dbEntity.SaveChanges();

                         }
                         else
                         {
                             //递归删除孩子程序
                             DeleteChildProg(id,PrivID);
                         }
                     }
                 }
                 else
                 {
                     var oDelProgram = (from o in dbEntity.MemberPrivItems
                                        where (o.RefID == id && o.Deleted == false)
                                        select o).Single();
                     oDelProgram.Deleted = true;
                     dbEntity.SaveChanges();
                 }
            }
            
            return RedirectToAction("PrivProgram");
        }
        /// <summary>
        /// 递归删除子程序
        /// </summary>
        /// <param name="id"></param>
        /// <param name="PrivID"></param>
        public void DeleteChildProg(Guid id,Guid PrivID)
        {
            bool flag = false;
            var ProgramList = (from o in dbEntity.GeneralPrograms.Include("Parent")
                               where (o.Parent.Gid == id && o.Deleted == false)
                               select o).ToList();
            var refidlist = (from o in dbEntity.MemberPrivItems
                             where (o.PrivID == PrivID)
                             select o).ToList();
            for (int i = 0; i < ProgramList.Count; i++)
            {
                for (int j = 0; j < refidlist.Count; j++)
                {
                    if (ProgramList[i].Gid == refidlist[j].RefID)
                        flag = true;
                    break;
                }
                if (flag == false)
                {
                    if (ProgramList.ElementAt(i).Terminal == true)
                    {
                        MemberPrivItem PrivItem =( from o in dbEntity.MemberPrivItems
                                                  where (o.RefID == ProgramList.ElementAt(i).Gid)
                                                  select o).Single();
                        PrivItem.Deleted = true;
                        dbEntity.SaveChanges();
                        //是否存在程序功能节点
                        var ProgNode = (from o in dbEntity.GeneralProgNodes
                                        where (o.Deleted == false && o.ProgID == ProgramList[i].Gid)
                                        select o).ToList();
                        //如果存在功能节点，删除
                        if (ProgNode.Count > 0)
                        {
                            foreach (var Nodeitem in ProgNode)
                            {

                                MemberPrivItem NodePrivItem = (from o in dbEntity.MemberPrivItems
                                                           where (o.RefID == Nodeitem.Gid && o.Deleted == false)
                                                           select o).Single();
                                NodePrivItem.Deleted = true;
                            }
                            dbEntity.SaveChanges();
                        }
                    }
                    else
                    {
                        PrivProChild(ProgramList[i].Gid, PrivID);

                    }
                }
                flag = false;
            }
        }
        /// <summary>
        /// 启用 状态改变
        /// </summary>
        public void DelAllProg()
        {
            Guid userID = new Guid("12db0a77-92c4-e011-afef-002269c9aa85");
            Guid PrivID = (from o in dbEntity.MemberPrivileges
                                      where (o.Deleted == false && o.UserID == userID&&o.Ptype==0)
                                      select o.Gid).FirstOrDefault();
            var PrivItem = from o in dbEntity.MemberPrivItems
                           where (o.PrivID == PrivID && o.Deleted == false)
                           select o;
            foreach (var item in PrivItem)
            {
                item.Deleted = true;
            }
            dbEntity.SaveChanges();
        }
        /// <summary>
        /// 程序节点InputMode=0
        /// </summary>
        /// <returns></returns>
        public ActionResult Editor()
        {
            return View();

        }
        /// <summary>
        /// 程序节点InputMode=1
        /// </summary>
        /// <returns></returns>
        public ActionResult DropDown(Guid id)
        {
            GeneralProgNode DropMatter = (from o in dbEntity.GeneralProgNodes.Include("Optional")
                             where (o.Gid == id&&o.Deleted==false)
                             select o).Single();
            string strMatter = DropMatter.Optional.Matter;
            string[] MatterArray = strMatter.Split(',');
            List<SelectListItem> matterlist = new List<SelectListItem>();
            for (int i = 0; i < MatterArray.Length; i++)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = i.ToString(),
                    Value = (MatterArray[i])
                };
                matterlist.Add(item);
            }
            ViewBag.matterlist = matterlist;
            return View();
        }


        /// <summary>
        /// 商品授权
        /// </summary>
        /// <returns></returns>
        public ActionResult PrivProduct()
        {
            return View();
        }
        /// <summary>
        /// 加载商品树形结构
        /// </summary>
        /// <returns></returns>
        public string PrivProductLoad()
        {
            Guid userID = new Guid("12db0a77-92c4-e011-afef-002269c9aa85");


           Guid PrivID = (from o in dbEntity.MemberPrivileges
                           where (o.UserID == userID && o.Ptype == 2 && o.Deleted == false)
                           select o.Gid).Single();
           var OrgPrivList = (from o in dbEntity.MemberPrivItems
                            where (o.PrivID == PrivID && o.Deleted == false)
                            select o).ToList();
            List<LiveTreeNode> list = new List<LiveTreeNode>();

            foreach (var item in OrgPrivList)
            {
                MemberOrganization org = new MemberOrganization();
                org.Gid = (Guid)item.RefID;
                var Org = (from o in dbEntity.MemberOrganizations.Include("ShortName")
                                  where (o.Gid == org.Gid && o.Deleted == false)
                                  select o).Single();
                LiveTreeNode node = new LiveTreeNode();
                node.name = Org.ShortName.Matter;
                node.id = Org.Gid.ToString();
                node.nodeChecked = false;
                node.nodes = new List<LiveTreeNode>();
                list.Add(node);
            }
            string strTreeJson = CreateTree(list);
            return strTreeJson;

        }
        /// <summary>
        /// 展开树形节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string PrivProductExpand(Guid id)
        {
            string strTreeJson = "";
            int nCount=0;
            Guid userID = new Guid("12db0a77-92c4-e011-afef-002269c9aa85");

            //MemberPrivilege表中选择组织授权的Gid
            Guid PrivID = (from o in dbEntity.MemberPrivileges
                           where (o.UserID == userID && o.Ptype == 2 && o.Deleted == false)
                           select o.Gid).Single();
            //MemberPrivItem表中选择出该用户有权查看的所有组织
            var OrgPrivList = (from o in dbEntity.MemberPrivItems
                               where (o.PrivID == PrivID && o.Deleted == false)
                               select o).ToList();
            //确定参数id是组织id还是商品id
            foreach(var item in OrgPrivList)
                {
                    MemberOrganization org = new MemberOrganization();
                    org.Gid = (Guid)item.RefID;
                    if (id == org.Gid)
                        break;
                    else nCount++;                 
                 }
                 
               //树节点为商品时展开 
           if (nCount==OrgPrivList.Count)
            {
                  var ProductList = (from o in dbEntity.GeneralPrivateCategorys.Include("Parent")
                                       where ( o.Parent.Gid == id && o.Deleted == false)
                                       select o).ToList();
                    List<LiveTreeNode> list = new List<LiveTreeNode>();
                    foreach (var Proitem in ProductList)
                    {
                        LiveTreeNode node = new LiveTreeNode();
                        node.name = Proitem.Name.Matter;
                        node.id = Proitem.Gid.ToString();
                        node.nodeChecked = false;
                        node.nodes = new List<LiveTreeNode>();
                        list.Add(node);
                    }
                    strTreeJson = CreateTreeJson(list, "");
                
            }
               //树节点为组织时展开
           else 
           {
                   
                   

                   var ProductList = (from o in dbEntity.GeneralPrivateCategorys
                                          where(o.OrgID==id && o.Parent ==null&&o.Deleted==false)
                                          select o).ToList();
                   List<LiveTreeNode> list = new List<LiveTreeNode>();
                    foreach (var Proitem in ProductList)
                    {
                        LiveTreeNode node = new LiveTreeNode();
                        node.name = Proitem.Name.Matter;
                        node.id = Proitem.Gid.ToString();
                        node.nodeChecked = false;
                        node.nodes = new List<LiveTreeNode>();
                        list.Add(node);
                    }
                    strTreeJson = CreateTreeJson(list, "");
               }
           
            return "["+strTreeJson+"]";
        }
        /// <summary>
        /// 保存授权商品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PrivProduct(Guid? id)
        {
            Guid userID = new Guid("12db0a77-92c4-e011-afef-002269c9aa85");
            Guid PrivID = (from o in dbEntity.MemberPrivileges
                           where (o.UserID == userID && o.Ptype == 1 && o.Deleted == false)
                           select o.Gid).Single();
            GeneralPrivateCategory Product = (from o in dbEntity.GeneralPrivateCategorys
                                  where (o.Gid == id && o.Deleted == false)
                                  select o).Single();
            MemberPrivItem mPrivItem = new MemberPrivItem();
            int nCount=Product.ChildItems.ToList<GeneralPrivateCategory>().Count;
            if (nCount==0)
            {
                mPrivItem.PrivID = PrivID;
                mPrivItem.RefID = Product.Gid;
                dbEntity.MemberPrivItems.Add(mPrivItem);
                dbEntity.SaveChanges();
            }
            else
            {

                PrivProductChild(Product.ChildItems.ToList<GeneralPrivateCategory>(), PrivID);

            }
            return View();
        }
        public void PrivProductChild(List<GeneralPrivateCategory> list, Guid PrivID)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list.ElementAt(i).ChildItems.Count>0)
                {
                    PrivProductChild(list.ElementAt(i).ChildItems.ToList<GeneralPrivateCategory>(), PrivID);
                }
                else
                {
                    MemberPrivItem mPrivItem = new MemberPrivItem();
                    mPrivItem.PrivID = PrivID;
                    mPrivItem.RefID = list.ElementAt(i).Gid;
                    dbEntity.MemberPrivItems.Add(mPrivItem);
                    dbEntity.SaveChanges();
                }
            }
        }




        /// <summary>
        /// 供应商授权
        /// </summary>
        /// <returns></returns>
        public ActionResult PrivSupplier()
        {
            int nstatus;
            Guid userID = new Guid("12db0a77-92c4-e011-afef-002269c9aa85");
            MemberPrivilege PrivID = (from o in dbEntity.MemberPrivileges
                                      where (o.Deleted == false && o.UserID == userID && o.Ptype == 6)
                                      select o).FirstOrDefault();
            if (PrivID != null)
            {
                nstatus = (int)PrivID.Pstatus;
            }
            else nstatus = -1;
            ViewBag.status = nstatus;
            return View();
        }
        /// <summary>
        /// 加载供应商树形结构
        /// </summary>
        /// <returns></returns>
        public string PrivSupplierLoad()
        {
            var SupplierList = from o in dbEntity.MemberOrganizations.Include("ShortName")
                           where (o.Otype == 3 && o.Parent == null && o.Deleted == false)
                           select o;
            List<LiveTreeNode> list = new List<LiveTreeNode>();
            foreach (var item in SupplierList)
            {
                LiveTreeNode node = new LiveTreeNode();
                node.name = item.ShortName.Matter;
                node.id = item.Gid.ToString();
                node.nodeChecked = false;
                node.nodes = new List<LiveTreeNode>();
                list.Add(node);
            }
            string strTree = CreateTree(list);
            return strTree;
        }
        /// <summary>
        /// 展开供应商树节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string PrivSupplierExpand(Guid id)
        {
            string strTreeJson = "";
            if (id.Equals("00000000-0000-0000-0000-000000000000"))
            {
                var SuppierList = from o in dbEntity.MemberOrganizations.Include("ShortName")
                                  where (o.Parent == null && o.Deleted == false)
                                  select o;
                List<LiveTreeNode> list = new List<LiveTreeNode>();
                foreach (var item in SuppierList)
                {
                    LiveTreeNode node = new LiveTreeNode();
                    node.name = item.ShortName.Matter;
                    node.id = item.Gid.ToString();
                    node.nodeChecked = false;
                    node.nodes = new List<LiveTreeNode>();
                    list.Add(node);
                }
                strTreeJson = CreateTreeJson(list, "");
            }
            else
            {
                var SupplierList = from o in dbEntity.MemberOrganizations.Include("ShortName")
                                   where (o.Parent.Gid == id && o.Deleted == false)
                                   select o;
                List<LiveTreeNode> list = new List<LiveTreeNode>();
                foreach (var item in SupplierList)
                {
                    LiveTreeNode node = new LiveTreeNode();
                    node.name = item.ShortName.Matter;
                    node.id = item.Gid.ToString();
                    node.nodeChecked = false;
                    node.nodes = new List<LiveTreeNode>();
                    list.Add(node);
                }
                strTreeJson = CreateTreeJson(list, "");
            }
            return "[" + strTreeJson + "]";
        }
        /// <summary>
        /// 加载已授权供应商
        /// </summary>
        /// <returns></returns>
        public string SubSupplierLoad()
        {
            string strTreeJson = "";
            Guid userID = new Guid("12db0a77-92c4-e011-afef-002269c9aa85");
            Guid PrivID = (from o in dbEntity.MemberPrivileges
                           where (o.UserID == userID && o.Deleted == false && o.Ptype == 6)
                           select o.Gid).FirstOrDefault();
            if (PrivID == null) return null;
            else
            {
                var oSupItem = (from o in dbEntity.MemberPrivItems
                              where (o.Deleted == false && o.PrivID == PrivID)
                              select o.RefID).ToList();
                List<LiveTreeNode> list = new List<LiveTreeNode>();
                for (int i = 0; i < oSupItem.Count; i++)
                {
                    Guid oGid = new Guid(oSupItem[i].ToString());

                    var oSupplier = (from o in dbEntity.MemberOrganizations
                                    where (o.Gid == oGid && o.Deleted == false)
                                    select o).FirstOrDefault();
                    
                        //只加载父亲为空的节点
                        if (oSupplier.Parent == null)
                        {
                            LiveTreeNode oTreeNode = new LiveTreeNode();
                            oTreeNode.id = oSupplier.Gid.ToString();
                            oTreeNode.name = oSupplier.ShortName.Matter;
                            oTreeNode.nodeChecked = false;
                            oTreeNode.nodes = new List<LiveTreeNode>();
                            list.Add(oTreeNode);
                        }                                       
                }
                strTreeJson = CreateTree(list);
                return strTreeJson;

            }
        }
        /// <summary>
        /// 展开已授权供应商
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string SubSupplierExpand(Guid id)
        {
            string strTreeJson = "";
            Guid userID = new Guid("12db0a77-92c4-e011-afef-002269c9aa85");
            Guid PrivID = (from o in dbEntity.MemberPrivileges
                           where (o.UserID == userID && o.Deleted == false && o.Ptype == 0)
                           select o.Gid).FirstOrDefault();
            if (id.Equals("00000000-0000-0000-0000-000000000000"))
            {
                //所有已授权的供应商
                var oSupItem = (from o in dbEntity.MemberPrivItems
                                where (o.Deleted == false && o.PrivID == PrivID)
                                select o.RefID).ToList();
                List<LiveTreeNode> list = new List<LiveTreeNode>();
                for (int i = 0; i < oSupItem.Count; i++)
                {
                    Guid oGid = new Guid(oSupItem[i].ToString());

                    var oSupplier = (from o in dbEntity.MemberOrganizations
                                     where (o.Gid == oGid && o.Deleted == false)
                                     select o).Single();
                    //只加载父亲为空的节点
                    if (oSupplier.Parent == null)
                    {
                        LiveTreeNode oTreeNode = new LiveTreeNode();
                        oTreeNode.id = oSupplier.Gid.ToString();
                        oTreeNode.name = oSupplier.ShortName.Matter;
                        oTreeNode.nodeChecked = false;
                        oTreeNode.nodes = new List<LiveTreeNode>();
                        list.Add(oTreeNode);
                    }
                }
                strTreeJson = CreateTreeJson(list, "");
            }
            else
            {
                List<LiveTreeNode> list = new List<LiveTreeNode>();
                bool flag = false;
                var oSup = (from o in dbEntity.MemberOrganizations.Include("Parent").Include("ShortName")
                            where (o.Parent.Gid == id && o.Deleted == false)
                            select o).ToList();
                var oRef = (from o in dbEntity.MemberPrivItems
                            where (o.PrivID == PrivID && o.Deleted == false)
                            select o).ToList();
                int noSup = oSup.Count;
                int noRef = oRef.Count;
                for (int i = 0; i < noSup; i++)
                {
                    for (int j = 0; i < noRef; j++)
                    {
                        if (oSup[i].Gid == oRef[j].RefID)
                            flag = true;
                        break;
                    }
                    if (flag == false)
                    {

                        LiveTreeNode oTreeNode = new LiveTreeNode();
                        oTreeNode.id = oSup[i].Gid.ToString();
                        oTreeNode.name = oSup[i].ShortName.Matter;
                        oTreeNode.nodeChecked = false;
                        oTreeNode.nodes = new List<LiveTreeNode>();
                        list.Add(oTreeNode);
                    }

                    flag = false;
                }
                strTreeJson = CreateTreeJson(list, "");                
            }
            return "[" + strTreeJson + "]";
        }
        /// <summary>
        /// 保存供应商授权数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PrivSupplier(string result, string open)
        {
            Guid userID = new Guid("12db0a77-92c4-e011-afef-002269c9aa85");
            Guid PrivID = (from o in dbEntity.MemberPrivileges
                                      where (o.UserID == userID && o.Deleted == false && o.Ptype == 0)
                                      select o.Gid).FirstOrDefault();
            string[] gid = result.Split(',');
            string[] openstate = open.Split(',');
            int ngid=gid.Length;
            for(int i=1;i<ngid-1;i++)
            {
                Guid id = new Guid(gid[i]);
                if (openstate[i] == "undefined")
                {
                    //递归保存
                }
                else
                {
                    MemberPrivItem SupItem = new MemberPrivItem();
                    SupItem.PrivID = PrivID;
                    SupItem.RefID = id;
                    dbEntity.MemberPrivItems.Add(SupItem);
                    dbEntity.SaveChanges();
                }
            }
            return RedirectToAction("PrivSupplier");
        }
        /// <summary>
        /// 递归函数保存叶节点
        /// </summary>
        /// <param name="list"></param>
        /// <param name="PrivID"></param>
        public void PrivSupChild(Guid id,Guid PrivID)
        {
            //取出所有孩子
            var SupChild = from o in dbEntity.MemberOrganizations.Include("Parent")
                           where (o.Otype == 3 && o.Parent.Gid==id && o.Deleted == false)
                           select o;
            int nSupChild = SupChild.ToList().Count;
            for (int i = 0; i < nSupChild; i++)
            {
                if (SupChild.ToList().ElementAt(i).Terminal == false)
                {
                    PrivSupChild(SupChild.ToList().ElementAt(i).Gid, PrivID);
                }
                else
                {
                    Guid  ChildID=SupChild.ToList().ElementAt(i).Gid;
                    var SupChildItem = (from o in dbEntity.MemberPrivItems
                                       where (o.RefID == ChildID)
                                       select o).FirstOrDefault();
                    if (SupChildItem == null)
                    {
                        MemberPrivItem mPrivItem = new MemberPrivItem();
                        mPrivItem.PrivID = PrivID;
                        mPrivItem.RefID = ChildID;
                        dbEntity.MemberPrivItems.Add(mPrivItem);
                        dbEntity.SaveChanges();
                    }
                    else
                    {
                        MemberPrivItem mPrivItem = new MemberPrivItem();
                        mPrivItem.Deleted = false;
                    }
                }
            }
        }
        /// <summary>
        /// 删除树节点
        /// </summary>
        /// <param name="result"></param>
        /// <param name="open"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteSupplier(string result, string open)
        {
             Guid userID = new Guid("12db0a77-92c4-e011-afef-002269c9aa85");
            Guid PrivID = (from o in dbEntity.MemberPrivileges
                                      where (o.UserID == userID && o.Deleted == false && o.Ptype == 0)
                                      select o.Gid).Single();
            string[] gid = result.Split(',');
            string[] openstate = open.Split(',');
            int ngid=gid.Length;
            for (int i = 0; i < ngid; i++)
            {
                Guid id = new Guid(gid[i]);
                if (openstate[i] == "undefined")
                {
                    DelSupChild(id, PrivID);
                }
                else
                {
                    var SupItem = (from o in dbEntity.MemberPrivItems
                                  where (o.RefID == id)
                                  select o).Single();
                    SupItem.Deleted = true;
                }
            }
            return View();
        }
        /// <summary>
        /// 递归删除
        /// </summary>
        /// <param name="id"></param>
        /// <param name="PrivID"></param>
        public void DelSupChild(Guid id, Guid PrivID)
        {
            bool flag = false;
            var SupChild = (from o in dbEntity.MemberOrganizations.Include("Parent")
                           where (o.Deleted == false && o.Parent.Gid == id && o.Otype == 3)
                           select o).ToList();
            var SupItem = (from o in dbEntity.MemberPrivItems
                          where (o.PrivID == PrivID)
                          select o).ToList();
            int nSupChild = SupChild.Count;
            int nSupItem = SupItem.Count;
            for (int i = 0; i < nSupChild; i++)
            {
                for (int j = 0; j < nSupItem; j++)
                {
                    if (SupChild[i].Gid == SupItem[j].RefID)
                        flag = true;
                    break;
                }
                if (flag == false)
                {
                    Guid ChildID = SupChild[i].Gid;
                    MemberPrivItem TheSupItem = (from o in dbEntity.MemberPrivItems
                                                where (o.RefID == ChildID)
                                                select o).Single();
                    TheSupItem.Deleted = true;
                }
                    flag = false;
            }

        }
        /// <summary>
        /// 启用状态改变
        /// </summary>
        /// <returns></returns>        
        public string DelAllSup()
        {
            Guid userID = new Guid("12db0a77-92c4-e011-afef-002269c9aa85");
            Guid PrivID = (from o in dbEntity.MemberPrivileges
                           where (o.Deleted == false && o.UserID == userID && o.Ptype ==6)
                           select o.Gid).FirstOrDefault();
            var PrivItem = from o in dbEntity.MemberPrivItems
                           where (o.PrivID == PrivID && o.Deleted == false)
                           select o;
            foreach (var item in PrivItem)
            {
                item.Deleted = true;
            }
            dbEntity.SaveChanges();
            return null;
        }
       

        protected override void Dispose(bool disposing)
        {
            dbEntity.Dispose();
            base.Dispose(disposing);
        }
    }
}
