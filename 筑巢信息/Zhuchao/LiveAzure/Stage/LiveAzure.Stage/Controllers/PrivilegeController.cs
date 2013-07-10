/******************************************PrivilegeController********************************************/
//说明：用户授权控制器，用于系统后台管理Stage
//功能：对组织用户进行授权 包括程序授权、组织授权、程序授权、渠道授权、仓库授权、产品类别授权、供应商类别授权
//操作权限：超级管理员拥有最高权限，其他用户根据程序授权和程序功能节点授权判断
//最后修改日期：2011-10-19
/*********************************************************************************************************/
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
using LiveAzure.BLL;

namespace LiveAzure.Stage.Controllers
{
    public class PrivilegeController : BaseController
    {
        #region 初始化
        //全局变量
        public static Guid gUserID;//被授权的用户ID
        public static Guid gOrgID;//全局组织ID
        public static Guid ProgramID;
        public static Guid ProgNodeID;//程序节点ID
        public static string gSearchStr;
 
        /// <summary>
        /// 控制器初始化
        /// </summary>
        /// <param name="requestContext"></param>
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            oEventBLL.WriteEvent("Initialize Load",
                ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString(), CurrentSession.UserID);
        }
        #endregion

        #region 授权首页
        /// <summary>
        /// 授权首页
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public ActionResult Index()
        {
            // 权限验证
            if (!base.CheckPrivilege())
                RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            gUserID = new Guid();//初始全局变量，被授权用户ID
            Guid? userID = CurrentSession.UserID;
            if (gOrgID == Guid.Empty || gOrgID == null)//
            {
                //获取组织
                MemberUser oUser = dbEntity.MemberUsers.Where(u => u.Deleted == false && u.Gid == CurrentSession.UserID).Single();
                gOrgID = oUser.OrgID;
            }
            //组织下拉框
            ViewBag.organizationList = GetSupportOrganizations();
            return View();
        }
        /// <summary>
        /// 查找用户列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchUser(Guid orgId, string searchStr)
        {
            gSearchStr = searchStr;
            gOrgID = orgId;
            ViewBag.orgid = orgId;
            ViewBag.str = searchStr;
            return View();
        }
        /// <summary>
        /// 查找符合的用户
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListSearchUser(SearchModel searchModel, string str,Guid? orgID=null)
        {
            List<MemberUser> memberUsers = dbEntity.MemberUsers.Include("Role").Where(s => s.Deleted == false && s.OrgID == orgID).ToList();
            if (!String.IsNullOrEmpty(str))
            {
                memberUsers = memberUsers.Where(s => s.LoginName!=null? s.LoginName.ToUpper().Contains(str.ToUpper()):false ||
                    s.LastName != null ? s.LastName.ToUpper().Contains(str.ToUpper()) :false||
                    s.FirstName != null ? s.FirstName.ToUpper().Contains(str.ToUpper()) : false||
                    s.Email != null? s.Email.ToUpper().Contains(str.ToUpper()) :false||
                    s.NickName !=null? s.NickName.ToUpper().Contains(str.ToUpper()) :false
                    ).ToList();
            }
            IQueryable<MemberUser> result = memberUsers.AsQueryable();
            GridColumnModelList<MemberUser> columns = new GridColumnModelList<MemberUser>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.LoginName);
            columns.Add(p => p.Organization.FullName.GetResource(CurrentSession.Culture)).SetName("OrgName");
            columns.Add(p => p.Role.Name.GetResource(CurrentSession.Culture)).SetName("RoleName");
            columns.Add(p => p.Email);
            columns.Add(p => p.Birthday);

            GridData gridData = result.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 授权模板
        /// <summary>
        /// 保存授权模板页
        /// </summary>
        /// <param name="UserID"></param>
        public ActionResult SavePrivilegeTemplatePage(Guid UserID)
        {
            if (!base.CheckPrivilege("EnableEdit"))//权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            GeneralPrivTemplate oNewModel = new GeneralPrivTemplate();
            ViewBag.UserID = UserID;
            return View(oNewModel);
        }
        /// <summary>
        /// 是否存在模板
        /// </summary>
        /// <returns></returns>
        public bool HasTemplateBefore(string Code)
        {
            return dbEntity.GeneralPrivTemplates.Any(temp => temp.Deleted == false && temp.Code == Code);
        }
        /// <summary>
        /// 保存授权模板
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="TemplatePage"></param>
        public void SavePrivilegeTemplate(FormCollection formCollection, GeneralPrivTemplate oBackModel)
        {
            Guid UserID = Guid.Parse(formCollection["UserID"].ToString());
            List<MemberPrivilege> oMemberPrivilegeList = dbEntity.MemberPrivileges.Where(p => p.UserID == UserID && p.Deleted == false).ToList();
            List<GeneralPrivTemplate> oOldTemplateList = dbEntity.GeneralPrivTemplates.Where(temp => temp.Code == oBackModel.Code).ToList();
            if (oOldTemplateList.Count > 0)
            {//如果原来存在模板， 则先删除模板中所有权限
                foreach (GeneralPrivTemplate item in oOldTemplateList)
                {
                    item.Deleted = true;
                    List<GeneralPrivItem> oOldTemplateItemList = dbEntity.GeneralPrivItems.Where(p => p.PrivID == item.Gid && p.Deleted==false).ToList();
                    if (oOldTemplateItemList.Count > 0)
                    {
                        foreach (GeneralPrivItem privitem in oOldTemplateItemList)
                        {
                            privitem.Deleted = true;
                        }
                    }
                }
                dbEntity.SaveChanges();
            }
            foreach (MemberPrivilege oMemberPrivilege in oMemberPrivilegeList)
            {
                GeneralPrivTemplate oOldGeneralPrivTemplate = oOldTemplateList.Where(temp => temp.Ptype == oMemberPrivilege.Ptype).FirstOrDefault();
                bool bHasBefore = oOldGeneralPrivTemplate == null ? false : true;
                if (bHasBefore)
                {//之前有模板 则更新原来模板
                    oOldGeneralPrivTemplate.Deleted = false;
                    oOldGeneralPrivTemplate.Pstatus = oMemberPrivilege.Pstatus;
                    oOldGeneralPrivTemplate.Remark = oBackModel.Remark;
                    List<MemberPrivItem> oMemberPrivItemList = dbEntity.MemberPrivItems.Where(p => p.PrivID == oMemberPrivilege.Gid&&p.Deleted==false).ToList();
                    foreach (MemberPrivItem oMemberPrivItem in oMemberPrivItemList)
                    {
                        GeneralPrivItem oOldGeneralPrivItem = dbEntity.GeneralPrivItems.Where(p => p.PrivID == oOldGeneralPrivTemplate.Gid && p.RefID == oMemberPrivItem.RefID).FirstOrDefault();
                        if (oOldGeneralPrivItem != null)
                        {//原来有GeneralPrivItem 更新
                            oOldGeneralPrivItem.Deleted = false;
                            oOldGeneralPrivItem.NodeCode = oMemberPrivItem.NodeCode;
                            oOldGeneralPrivItem.NodeValue = oMemberPrivItem.NodeValue;
                            oOldGeneralPrivItem.Remark = oMemberPrivItem.Remark;
                        }
                        else
                        {//没有 则添加
                            GeneralPrivItem oNewPrivItem = new GeneralPrivItem
                            {
                                PrivID = oOldGeneralPrivTemplate.Gid,
                                RefID = oMemberPrivItem.RefID,
                                NodeCode = oMemberPrivItem.NodeCode,
                                NodeValue = oMemberPrivItem.NodeValue,
                                Remark = oMemberPrivItem.Remark
                            };
                            dbEntity.GeneralPrivItems.Add(oNewPrivItem);
                        }
                    }
                }
                else
                {//添加
                    GeneralPrivTemplate oNewPriv = new GeneralPrivTemplate
                    {
                        Code = oBackModel.Code,
                        Ptype = oMemberPrivilege.Ptype,
                        Pstatus = oMemberPrivilege.Pstatus,
                        Remark = oBackModel.Remark
                    };
                    dbEntity.GeneralPrivTemplates.Add(oNewPriv);
                    dbEntity.SaveChanges();
                    List<MemberPrivItem> oMemberPrivItemList = dbEntity.MemberPrivItems.Where(p => p.PrivID == oMemberPrivilege.Gid&&p.Deleted==false).ToList();
                    foreach (MemberPrivItem oMemberPrivItem in oMemberPrivItemList)
                    {
                        GeneralPrivItem oNewPrivItem = new GeneralPrivItem
                        {
                            PrivID = oNewPriv.Gid,
                            RefID = oMemberPrivItem.RefID,
                            NodeCode = oMemberPrivItem.NodeCode,
                            NodeValue = oMemberPrivItem.NodeValue,
                            Remark = oMemberPrivItem.Remark
                        };
                        dbEntity.GeneralPrivItems.Add(oNewPrivItem);
                    }
                }
            }
            dbEntity.SaveChanges();
        }
        /// <summary>
        /// 模板选择页
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public ActionResult ChoosePrivilegeTemplatePage(Guid UserID)
        {
            if (!base.CheckPrivilege("EnableEdit"))//权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            List<SelectListItem> oTemplateList = new List<SelectListItem>();
            List<string> oGeneralPrivTemplateList = dbEntity.GeneralPrivTemplates.Where(p => p.Deleted == false).Select(p=>p.Code).Distinct().ToList();
            for (int i = 0; i < oGeneralPrivTemplateList.Count;i++ )
            {
                oTemplateList.Add(new SelectListItem { Text = oGeneralPrivTemplateList.ElementAt(i), Value = oGeneralPrivTemplateList.ElementAt(i) });
            }
            ViewBag.Template = oTemplateList;
            MemberUser oUser = dbEntity.MemberUsers.Where(u => u.Deleted == false && u.Gid == UserID).FirstOrDefault();
            if (oUser != null)
            {
                ViewBag.UserName = oUser.DisplayName;
                ViewBag.UserID = oUser.Gid;
            }
            return View();
        }
        /// <summary>
        /// 用模板对用户授权
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="SelectTemplateCode"></param>
        /// <returns></returns>
        public bool SetPrivilegeByTemplate(Guid UserID, string SelectTemplateCode)
        {
            if (UserID == null || String.IsNullOrEmpty(SelectTemplateCode) || SelectTemplateCode == "null")
                return false;
            List<GeneralPrivTemplate> GeneralPrivTemplateList = dbEntity.GeneralPrivTemplates.Where(g => g.Deleted == false && g.Code == SelectTemplateCode).ToList();
            //查找用户本身的权限 并且都做删除操作
            List<MemberPrivilege> oOldMemberPrivilegeList = dbEntity.MemberPrivileges.Where(m => m.UserID == UserID).ToList();
            if (oOldMemberPrivilegeList.Count > 0)
            {
                foreach (MemberPrivilege memberPriv in oOldMemberPrivilegeList)
                {
                    memberPriv.Deleted = true;
                    List<MemberPrivItem> oOldMemberPrivItemList = dbEntity.MemberPrivItems.Where(p => p.Deleted == false && p.PrivID == memberPriv.Gid).ToList();
                    if (oOldMemberPrivItemList.Count > 0)
                    {
                        foreach (MemberPrivItem memberPrivItem in oOldMemberPrivItemList)
                        {
                            memberPrivItem.Deleted = true;
                        }
                    }
                }
            }
            dbEntity.SaveChanges();
            //COPY 若原来有该权限 delete恢复 若没有 则添加
            foreach (GeneralPrivTemplate oCopyPrivTemplate in GeneralPrivTemplateList)
            {
                MemberPrivilege oOldMemberPriv = oOldMemberPrivilegeList.Where(m => m.Ptype == oCopyPrivTemplate.Ptype).FirstOrDefault();
                bool bHasBefore = oOldMemberPriv == null ? false : true;
                if (bHasBefore)//之前有权限，则恢复数据
                {
                    oOldMemberPriv.Deleted = false;
                    oOldMemberPriv.Pstatus = oCopyPrivTemplate.Pstatus;
                    oOldMemberPriv.Remark = oCopyPrivTemplate.Remark;
                    List<GeneralPrivItem> oCopyGeneralPrivItemList = dbEntity.GeneralPrivItems.Where(g => g.Deleted == false && g.PrivID == oCopyPrivTemplate.Gid).ToList();//待COPY的子表
                    foreach (GeneralPrivItem oCopyGeneralPrivItem in oCopyGeneralPrivItemList)
                    {
                        MemberPrivItem oOldMemberPrivItem = dbEntity.MemberPrivItems.Where(m => m.PrivID == oOldMemberPriv.Gid && m.RefID == oCopyGeneralPrivItem.RefID).FirstOrDefault();
                        if (oOldMemberPrivItem != null)//子表 之前有 则恢复
                        {
                            oOldMemberPrivItem.Deleted = false;
                            oOldMemberPrivItem.NodeCode = oCopyGeneralPrivItem.NodeCode;
                            oOldMemberPrivItem.NodeValue = oCopyGeneralPrivItem.NodeValue;
                            oOldMemberPrivItem.Remark = oCopyGeneralPrivItem.Remark;
                        }
                        else//子表之前没有 则添加
                        {
                            MemberPrivItem oNewPrivItem = new MemberPrivItem
                            {
                                PrivID = oOldMemberPriv.Gid,
                                RefID = oCopyGeneralPrivItem.RefID,
                                NodeCode = oCopyGeneralPrivItem.NodeCode,
                                NodeValue = oCopyGeneralPrivItem.NodeValue,
                                Remark = oCopyGeneralPrivItem.Remark
                            };
                            dbEntity.MemberPrivItems.Add(oNewPrivItem);
                        }
                    }
                }
                else//之前没有，则添加
                {
                    MemberPrivilege oNewPriv = new MemberPrivilege
                    {
                        UserID = UserID,
                        Ptype = oCopyPrivTemplate.Ptype,
                        Pstatus = oCopyPrivTemplate.Pstatus,
                        Remark = oCopyPrivTemplate.Remark
                    };
                    dbEntity.MemberPrivileges.Add(oNewPriv);
                    dbEntity.SaveChanges();
                    List<GeneralPrivItem> oCopyGeneralPrivItemList = dbEntity.GeneralPrivItems.Where(g => g.Deleted == false && g.PrivID == oCopyPrivTemplate.Gid).ToList();
                    foreach (GeneralPrivItem oCopyGeneralPrivItem in oCopyGeneralPrivItemList)
                    {
                        MemberPrivItem oNewPrivItem = new MemberPrivItem { 
                            PrivID = oNewPriv.Gid,
                            RefID = oCopyGeneralPrivItem.RefID,
                            NodeCode = oCopyGeneralPrivItem.NodeCode,
                            NodeValue = oCopyGeneralPrivItem.NodeValue,
                            Remark = oCopyGeneralPrivItem.Remark
                        };
                        dbEntity.MemberPrivItems.Add(oNewPrivItem);
                    }
                }
            }
            dbEntity.SaveChanges();
            return true;
        }
        #endregion

        #region 用户信息显示
        /// <summary>
        /// 授权用户信息页
        /// </summary>
        /// <returns></returns>
        public ActionResult UserInfomation()
        {
            if (!base.CheckPrivilege("EnableEdit"))//权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            var memberUser = dbEntity.MemberUsers.Include("Channel").Include("Role").Where(s => s.Deleted == false && s.Gid == gUserID).SingleOrDefault();
            ViewBag.channelName = memberUser.Channel.FullName.GetResource(CurrentSession.Culture);
            ViewBag.RoleName = memberUser.Role.Name.GetResource(CurrentSession.Culture);
            return View(memberUser);
        }
        /// <summary>
        /// Tab页
        /// </summary>
        /// <param name="EditUserID">授权用户ID</param>
        /// <returns></returns>
        public ActionResult TabPage(Guid? EditUserID=null)
        {
            if(!base.CheckPrivilege("EnableEdit"))//权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            gUserID = (Guid)EditUserID;
            return View();
        }
        #endregion

        #region 组织授权
        /// <summary>
        /// 用户组织授权
        /// </summary>
        /// <returns></returns>
        public ActionResult PrivOrganization(string orgSearchString = null, Guid? selectedOrg = null)
        {
            if (!base.CheckPrivilege("EnableEdit"))//权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            if (selectedOrg == null)
            {
                gSearchStr = orgSearchString;
                //在数据库中查找是否已存在该用户组织授权
                MemberPrivilege memberPrivilege = dbEntity.MemberPrivileges.Where(o => o.UserID == gUserID && o.Deleted == false && o.Ptype == (byte)ModelEnum.UserPrivType.ORGANIZATION).FirstOrDefault();
                if (memberPrivilege != null)
                {
                    ViewBag.pStatus = memberPrivilege.Pstatus;
                }
                else
                    ViewBag.pStatus = -1;
                ViewBag.orgSearchStr = orgSearchString;
            }
            else//保存
            {
                //在数据库中查找是否已存在该用户组织授权
                MemberPrivilege memberPrivilege = dbEntity.MemberPrivileges.Where(o => o.UserID == gUserID && o.Ptype == (byte)ModelEnum.UserPrivType.ORGANIZATION).FirstOrDefault();
                //如果已存在，则对该用户组织授权项进行更新
                if (memberPrivilege != null)
                {
                    //更新组织授权项
                    memberPrivilege.Deleted = false;
                    memberPrivilege.Pstatus = (byte)ModelEnum.GenericStatus.VALID;
                    //在MemberPrivItem中，查找已添加的项
                    MemberPrivItem memberPrivItem = dbEntity.MemberPrivItems.Where(o => o.PrivID == memberPrivilege.Gid && o.RefID == selectedOrg).FirstOrDefault();
                    if (memberPrivItem != null)
                    {
                        memberPrivItem.Deleted = false;
                    }
                    else  //添加新增的项
                    {
                        MemberPrivItem oNewMemberPrivItem = new MemberPrivItem();
                        oNewMemberPrivItem.PrivID = memberPrivilege.Gid;
                        oNewMemberPrivItem.RefID = selectedOrg;
                        dbEntity.MemberPrivItems.Add(oNewMemberPrivItem);
                    }
                    dbEntity.SaveChanges();
                }
                //如果不存在，则新建组织授权，并添加组织授权项
                else
                {
                    //新建组织授权
                    memberPrivilege = new MemberPrivilege();
                    memberPrivilege.UserID = gUserID;
                    memberPrivilege.Ptype = 2;
                    memberPrivilege.Pstatus = 1;
                    dbEntity.MemberPrivileges.Add(memberPrivilege);
                    dbEntity.SaveChanges();
                    //添加授权项
                    MemberPrivItem memberPrivItem = new MemberPrivItem();
                    memberPrivItem.PrivID = memberPrivilege.Gid;
                    memberPrivItem.RefID = selectedOrg;
                    dbEntity.MemberPrivItems.Add(memberPrivItem);
                    dbEntity.SaveChanges();
                }
            }
            return View();
        }

        /// <summary>
        /// 组织授权列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListPrivOrganization(SearchModel searchModel)
        {
            Guid UserID = gUserID;
            Guid PrivID = dbEntity.MemberPrivileges.Where(o => o.UserID == UserID &&o.Ptype==(byte)ModelEnum.UserPrivType.ORGANIZATION&& o.Deleted == false).Select(o => o.Gid).FirstOrDefault();
            IQueryable<MemberOrganization> organs = (from o in dbEntity.MemberOrganizations
                                                     from p in dbEntity.MemberPrivItems
                                                     where (o.Deleted == false && o.Otype == (byte)ModelEnum.OrganizationType.CORPORATION && p.Deleted == false && p.PrivID == PrivID && o.Gid == p.RefID)
                                                     select o).AsQueryable();
            GridColumnModelList<MemberOrganization> columns = new GridColumnModelList<MemberOrganization>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Code);
            columns.Add(p => p.FullName.GetResource(CurrentSession.Culture)).SetName("FullName");

            GridData gridData = organs.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 组织列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListmemOrganization(SearchModel searchModel)
        {
            Guid UserID = (Guid)CurrentSession.UserID;
            List<MemberOrganization> ResultList = new List<MemberOrganization>();
            if (CurrentSession.IsAdmin)//如果登陆用户是超级管路员，则可以授权所有组织
            {
                ResultList = dbEntity.MemberOrganizations.Where(o => o.Otype == (byte)ModelEnum.OrganizationType.CORPORATION && o.Deleted == false).ToList();
            }
            else//如果不是管理员，则只可以授权该登陆用户自己有权限的组织
            {
                MemberPrivilege OrgPrivilege = dbEntity.MemberPrivileges.Where(o => o.UserID == UserID && o.Ptype == (byte)ModelEnum.UserPrivType.ORGANIZATION && o.Deleted == false && o.Pstatus == (byte)ModelEnum.GenericStatus.VALID).FirstOrDefault();
                if (OrgPrivilege != null)//如果没有启用或者没有授权组织，则只授权自己所属组织
                {
                    ResultList = (from o in dbEntity.MemberOrganizations
                                  join pitem in dbEntity.MemberPrivItems on o.Gid equals pitem.RefID
                                  where (o.Deleted == false && o.Otype == (byte)ModelEnum.OrganizationType.CORPORATION && pitem.Deleted == false && pitem.PrivID == OrgPrivilege.Gid)
                                  select o).ToList();
                }
                MemberOrganization UserOrg = dbEntity.MemberOrganizations.Where(o => o.Gid == CurrentSession.OrganizationGID && o.Deleted == false).FirstOrDefault();
                ResultList.Add(UserOrg);
            }
            IQueryable<MemberOrganization> Rseult = ResultList.AsQueryable();
            //搜索功能
            if (!String.IsNullOrEmpty(gSearchStr))
            {
                Rseult = Rseult.Where(s => s.Code.ToUpper().Contains(gSearchStr.ToUpper())
                                        || (s.FullName == null|| s.FullName.GetResource(CurrentSession.Culture) == null)? false: s.FullName.GetResource(CurrentSession.Culture).ToUpper().Contains(gSearchStr.ToUpper()));
            }
            GridColumnModelList<MemberOrganization> columns = new GridColumnModelList<MemberOrganization>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Code);
            columns.Add(p => p.FullName.GetResource(CurrentSession.Culture)).SetName("FullName");
            GridData gridData = Rseult.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 逻辑删除组织授权数据
        /// </summary>
        /// <param name="Gid">组织ID</param>
        /// <returns></returns>
        public string DeletePriOrg(Guid Gid)
        {
            MemberPrivItem MemberPrivItem = (from p in dbEntity.MemberPrivileges.AsEnumerable()
                                            join item in dbEntity.MemberPrivItems.AsEnumerable() on p.Gid equals item.PrivID
                                            where p.Deleted == false && item.Deleted == false && p.Ptype == (byte)ModelEnum.UserPrivType.ORGANIZATION && p.UserID == gUserID && item.RefID == Gid
                                            select item).Single();
            MemberPrivItem.Deleted = true;
            dbEntity.SaveChanges();
            return "success!";
        }

        /// <summary>
        /// 添加组织授权
        /// </summary>
        /// <returns></returns>
        public ActionResult PrivOrganizationAdd(string orgSearchString)
        {
            gSearchStr = orgSearchString;
            return View();
        }

        /// <summary>
        /// 检查所添加的授权组织是否是被授权用户自己所属组织
        /// </summary>
        /// <returns></returns>
        public bool IsUserOwnOrg(Guid orgID)
        {
            //获取被授权用户所属组织
            if (orgID == GetUserOrganizationID(gUserID))//如果添加的授权组织是用户自己所属组织，则不操作，因为用户本身就有自己组织权限
            {
                return true;
            }
            else
                return false;
        }
        #endregion

        #region 渠道授权
        /// <summary>
        /// 渠道授权
        /// </summary>
        /// <param name="chaSearchString"></param>
        /// <param name="selectedOrg"></param>
        /// <returns></returns>
        public ActionResult PrivChannel(string chaSearchString = null, Guid? selectedOrg = null)
        {
            if (!base.CheckPrivilege("EnableEdit"))//权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            if (selectedOrg == null)
            {
                gSearchStr = chaSearchString;
                //在数据库中查找是否已存在该用户渠道授权
                MemberPrivilege memberPrivilege = dbEntity.MemberPrivileges.Where(o => o.UserID == gUserID && o.Deleted == false && o.Ptype == (byte)ModelEnum.UserPrivType.CHANNEL).FirstOrDefault();
                if (memberPrivilege != null)
                {
                    ViewBag.pStatusC = memberPrivilege.Pstatus;
                }
                else
                    ViewBag.pStatusC = -1;

                ViewBag.chaSearchStr = chaSearchString;
            }
            else//保存
            {
                //在数据库中查找是否已存在该用户渠道授权
                MemberPrivilege memberPrivilege = dbEntity.MemberPrivileges.Where(o => o.UserID == gUserID && o.Ptype == (byte)ModelEnum.UserPrivType.CHANNEL).FirstOrDefault();
                //如果已存在，则对该用户渠道授权项进行更新
                if (memberPrivilege != null)
                {
                    //更新渠道授权项
                    memberPrivilege.Deleted = false;
                    memberPrivilege.Pstatus = (byte)ModelEnum.GenericStatus.VALID;
                    //在MemberPrivItem中，查找已添加的项
                    MemberPrivItem memberPrivItem = dbEntity.MemberPrivItems.Where(o => o.PrivID == memberPrivilege.Gid && o.RefID == selectedOrg).FirstOrDefault();
                    if (memberPrivItem != null)
                    {
                        memberPrivItem.Deleted = false;
                    }
                    else  //添加新增的项
                    {
                        MemberPrivItem oNewMemberPrivItem = new MemberPrivItem();
                        oNewMemberPrivItem.PrivID = memberPrivilege.Gid;
                        oNewMemberPrivItem.RefID = selectedOrg;
                        dbEntity.MemberPrivItems.Add(oNewMemberPrivItem);
                    }
                    dbEntity.SaveChanges();
                }
                //如果不存在，则新建渠道授权，并添加渠道授权项
                else
                {
                    //新建渠道授权
                    memberPrivilege = new MemberPrivilege();
                    memberPrivilege.UserID = gUserID;
                    memberPrivilege.Ptype = 3;
                    memberPrivilege.Pstatus = 1;
                    dbEntity.MemberPrivileges.Add(memberPrivilege);
                    dbEntity.SaveChanges();
                    //添加渠道授权项
                    MemberPrivItem memberPrivItem = new MemberPrivItem();
                    memberPrivItem.PrivID = memberPrivilege.Gid;
                    memberPrivItem.RefID = selectedOrg;
                    dbEntity.MemberPrivItems.Add(memberPrivItem);
                    dbEntity.SaveChanges();
                }
            }
            return View();
        }

        /// <summary>
        /// 渠道授权列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListPrivChannel(SearchModel searchModel)
        {
            Guid UserID = gUserID;
            Guid PrivID = dbEntity.MemberPrivileges.Where(o => o.UserID == UserID && o.Ptype == (byte)ModelEnum.UserPrivType.CHANNEL && o.Deleted == false).Select(o => o.Gid).FirstOrDefault();
            IQueryable<MemberChannel> organs = (from o in dbEntity.MemberChannels
                                                     from p in dbEntity.MemberPrivItems
                                                where (o.Deleted == false && o.Otype == (byte)ModelEnum.OrganizationType.CHANNEL && p.Deleted == false && p.PrivID == PrivID && o.Gid == p.RefID)
                                                     select o).AsQueryable();
            GridColumnModelList<MemberChannel> columns = new GridColumnModelList<MemberChannel>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Code);
            columns.Add(p => p.ShortName.Matter);

            GridData gridData = organs.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 渠道列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListmemChannel(SearchModel searchModel)
        {
            Guid UserID = (Guid)CurrentSession.UserID;
            IQueryable<MemberChannel> ResultList = new List<MemberChannel>().AsQueryable();
            if (CurrentSession.IsAdmin)//如果登陆用户是超级管路员，则可以授权所有渠道
            {
                ResultList = dbEntity.MemberChannels.Where(o => o.Otype == (byte)ModelEnum.OrganizationType.CHANNEL && o.Deleted == false).ToList().AsQueryable();
            }
            else//如果不是管理员，则只可以授权该登陆用户自己有权限的渠道
            {
                MemberPrivilege OrgPrivilege = dbEntity.MemberPrivileges.Where(o => o.UserID == UserID && o.Ptype == (byte)ModelEnum.UserPrivType.CHANNEL && o.Deleted == false && o.Pstatus == (byte)ModelEnum.GenericStatus.VALID).FirstOrDefault();
                if (OrgPrivilege != null)//如果有启用且有授权渠道
                {
                    ResultList = (from o in dbEntity.MemberChannels
                                  join pitem in dbEntity.MemberPrivItems on o.Gid equals pitem.RefID
                                  where (o.Deleted == false && o.Otype == (byte)ModelEnum.OrganizationType.CHANNEL && pitem.Deleted == false && pitem.PrivID == OrgPrivilege.Gid)
                                  select o).ToList().AsQueryable();
                }
            }
            //搜索功能
            if (!String.IsNullOrEmpty(gSearchStr))
            {
                ResultList = ResultList.Where(s => s.Code.ToUpper().Contains(gSearchStr.ToUpper())
                                        || (s.FullName == null || s.FullName.GetResource(CurrentSession.Culture) == null) ? false : s.FullName.GetResource(CurrentSession.Culture).ToUpper().Contains(gSearchStr.ToUpper()));
            }
            GridColumnModelList<MemberChannel> columns = new GridColumnModelList<MemberChannel>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Code);
            columns.Add(p => p.ShortName.Matter);
            GridData gridData = ResultList.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 逻辑删除渠道授权数据
        /// </summary>
        /// <param name="Gid"></param>
        /// <returns></returns>
        public string DeletePriCha(Guid Gid)
        {
            MemberPrivItem MemberPrivItem = (from p in dbEntity.MemberPrivileges.AsEnumerable()
                                             join item in dbEntity.MemberPrivItems.AsEnumerable() on p.Gid equals item.PrivID
                                             where p.Deleted == false && item.Deleted == false && p.Ptype == (byte)ModelEnum.UserPrivType.CHANNEL && p.UserID == gUserID && item.RefID == Gid
                                             select item).Single();
            MemberPrivItem.Deleted = true;

            dbEntity.SaveChanges();

            return "success!";
        }

        /// <summary>
        /// 添加渠道权限
        /// </summary>
        /// <param name="chaSearchString"></param>
        /// <returns></returns>
        public ActionResult PrivChannelAdd(string chaSearchString)
        {
            gSearchStr = chaSearchString;
            return View();
        }
        #endregion

        #region 仓库授权
        /// <summary>
        /// 仓库授权列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListPrivWarehouse(SearchModel searchModel)
        {
            Guid UserID = gUserID;
            Guid PrivID = dbEntity.MemberPrivileges.Where(o => o.UserID == UserID && o.Ptype == 4 && o.Deleted == false).Select(o => o.Gid).FirstOrDefault();
            IQueryable<WarehouseInformation> organs = (from o in dbEntity.WarehouseInformations
                                                from p in dbEntity.MemberPrivItems
                                                where (o.Deleted == false && o.Otype == 2 && p.Deleted == false && p.PrivID == PrivID && o.Gid == p.RefID)
                                                select o).AsQueryable();
            GridColumnModelList<WarehouseInformation> columns = new GridColumnModelList<WarehouseInformation>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Code);
            columns.Add(p => p.ShortName.Matter);

            GridData gridData = organs.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 仓库列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListmemWarehouse(SearchModel searchModel)
        {
            Guid UserID = (Guid)CurrentSession.UserID;
            IQueryable<WarehouseInformation> ResultList = new List<WarehouseInformation>().AsQueryable();
            if (CurrentSession.IsAdmin)//如果登陆用户是超级管路员，则可以授权所有仓库
            {
                ResultList = dbEntity.WarehouseInformations.Where(o => o.Otype == (byte)ModelEnum.OrganizationType.WAREHOUSE && o.Deleted == false).ToList().AsQueryable();
            }
            else//如果不是管理员，则只可以授权该登陆用户自己有权限的仓库
            {
                MemberPrivilege OrgPrivilege = dbEntity.MemberPrivileges.Where(o => o.UserID == UserID && o.Ptype == (byte)ModelEnum.UserPrivType.WAREHOUSE && o.Deleted == false && o.Pstatus == (byte)ModelEnum.GenericStatus.VALID).FirstOrDefault();
                if (OrgPrivilege != null)//如果有启用且有授权仓库
                {
                    ResultList = (from o in dbEntity.WarehouseInformations
                                  join pitem in dbEntity.MemberPrivItems on o.Gid equals pitem.RefID
                                  where (o.Deleted == false && o.Otype == (byte)ModelEnum.OrganizationType.WAREHOUSE && pitem.Deleted == false && pitem.PrivID == OrgPrivilege.Gid)
                                  select o).ToList().AsQueryable();
                }
            }
            //搜索功能
            if (!String.IsNullOrEmpty(gSearchStr))
            {
                ResultList = ResultList.Where(s => s.Code.ToUpper().Contains(gSearchStr.ToUpper())
                                        || (s.FullName == null || s.FullName.GetResource(CurrentSession.Culture) == null )? false:s.FullName.GetResource(CurrentSession.Culture).ToUpper().Contains(gSearchStr.ToUpper()));
            }
            GridColumnModelList<WarehouseInformation> columns = new GridColumnModelList<WarehouseInformation>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Code);
            columns.Add(p => p.ShortName.Matter);

            GridData gridData = ResultList.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 逻辑删除仓库授权数据
        /// </summary>
        /// <param name="Gid"></param>
        /// <returns></returns>
        public string DeletePriWare(Guid Gid)
        {
            MemberPrivItem MemberPrivItem = (from p in dbEntity.MemberPrivileges.AsEnumerable()
                                             join item in dbEntity.MemberPrivItems.AsEnumerable() on p.Gid equals item.PrivID
                                             where p.Deleted == false && item.Deleted == false && p.Ptype == (byte)ModelEnum.UserPrivType.WAREHOUSE && p.UserID == gUserID && item.RefID == Gid
                                             select item).Single();
            MemberPrivItem.Deleted = true;
            dbEntity.SaveChanges();
            return "success!";
        }

        /// <summary>
        /// 用户仓库授权
        /// </summary>
        /// <returns></returns>
        public ActionResult PrivWarehouse(string wareSearchString = null, Guid? selectedOrg=null)
        {
            if (!base.CheckPrivilege("EnableEdit"))//权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            if (selectedOrg == null)
            {
                gSearchStr = wareSearchString;
                //在数据库中查找是否已存在该用户仓库授权
                MemberPrivilege memberPrivilege = dbEntity.MemberPrivileges.Where(o => o.UserID == gUserID && o.Deleted == false && o.Ptype == (byte)ModelEnum.UserPrivType.WAREHOUSE).FirstOrDefault();
                if (memberPrivilege != null)
                {
                    ViewBag.pStatusW = memberPrivilege.Pstatus;
                }
                else
                    ViewBag.pStatusW = -1;
                ViewBag.wareSearchStr = wareSearchString;
            }
            else//保存仓库授权信息
            {
                //在数据库中查找是否已存在该用户渠道授权
                MemberPrivilege memberPrivilege = dbEntity.MemberPrivileges.Where(o => o.UserID == gUserID && o.Ptype == (byte)ModelEnum.UserPrivType.WAREHOUSE).FirstOrDefault();
                //如果已存在，则对该用户渠道授权项进行更新
                if (memberPrivilege != null)
                {
                    //更新组织授权项
                    memberPrivilege.Deleted = false;
                    memberPrivilege.Pstatus = (byte)ModelEnum.GenericStatus.VALID;
                    //在MemberPrivItem中，查找已添加的项
                    MemberPrivItem memberPrivItem = dbEntity.MemberPrivItems.Where(o => o.PrivID == memberPrivilege.Gid && o.RefID == selectedOrg).FirstOrDefault();
                    if (memberPrivItem != null)
                    {
                        memberPrivItem.Deleted = false;
                    }
                    else  //添加新增的项
                    {
                        MemberPrivItem oNewMemberPrivItem = new MemberPrivItem();
                        oNewMemberPrivItem.PrivID = memberPrivilege.Gid;
                        oNewMemberPrivItem.RefID = selectedOrg;
                        dbEntity.MemberPrivItems.Add(oNewMemberPrivItem);
                    }
                    dbEntity.SaveChanges();
                }
                //如果不存在，则新建组织授权，并添加组织授权项
                else
                {
                    //新建组织授权
                    memberPrivilege = new MemberPrivilege();
                    memberPrivilege.UserID = gUserID;
                    memberPrivilege.Ptype = 4;
                    memberPrivilege.Pstatus = 1;
                    dbEntity.MemberPrivileges.Add(memberPrivilege);
                    dbEntity.SaveChanges();
                    //添加组织授权项
                    MemberPrivItem memberPrivItem = new MemberPrivItem();
                    memberPrivItem.PrivID = memberPrivilege.Gid;
                    memberPrivItem.RefID = selectedOrg;
                    dbEntity.MemberPrivItems.Add(memberPrivItem);
                    dbEntity.SaveChanges();
                }
            }
            return View();
        }

        public ActionResult PrivWarehouseAdd(string wareSearchString)
        {
            gSearchStr = wareSearchString;
            return View();
        }
        #endregion

        #region 程序授权

        /// <summary>
        /// 程序授权
        /// </summary>
        /// <returns></returns>
        public ActionResult PrivProgram(Guid? id=null)
        {
            if (!base.CheckPrivilege("EnableEdit"))//权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            if (id == null)
            {
                int nstatus;
                MemberPrivilege oPri = (from o in dbEntity.MemberPrivileges where (o.UserID == gUserID && o.Deleted == false && o.Ptype == (byte)ModelEnum.UserPrivType.PROGRAM) select o).FirstOrDefault();
                if (oPri != null)
                {
                    nstatus = oPri.Pstatus;
                }
                else nstatus = -1;
                ViewBag.status = nstatus;
            }
            else//保存授权程序
            {
                Guid gPrivID = new Guid();
                Guid gid = (Guid)id;
                MemberPrivilege oMemberPrivilege = (from o in dbEntity.MemberPrivileges where (o.Ptype == 0 && o.UserID == gUserID) select o).FirstOrDefault();
                //已存在对程序授权的主记录
                if (oMemberPrivilege != null && oMemberPrivilege.Deleted == false)
                {
                    if (oMemberPrivilege.Pstatus == 0)
                    {
                        oMemberPrivilege.Pstatus = 1;
                        dbEntity.SaveChanges();
                    }
                    gPrivID = oMemberPrivilege.Gid;
                }
                if (oMemberPrivilege != null && oMemberPrivilege.Deleted == true)
                {
                    if (oMemberPrivilege.Pstatus == 0) oMemberPrivilege.Pstatus = 1;
                    oMemberPrivilege.Deleted = false;
                    dbEntity.SaveChanges();
                    gPrivID = oMemberPrivilege.Gid;
                }
                if (oMemberPrivilege == null)
                {
                    MemberPrivilege oMemPriv = new MemberPrivilege();
                    oMemPriv.Ptype = 0;
                    oMemPriv.UserID = gUserID;
                    oMemPriv.Pstatus = 1;
                    dbEntity.MemberPrivileges.Add(oMemPriv);
                    dbEntity.SaveChanges();
                    gPrivID = oMemPriv.Gid;
                }
                //是否存在对该程序的授权记录
                MemberPrivItem oMemberPrivItem = (from o in dbEntity.MemberPrivItems where (o.PrivID == gPrivID && o.RefID == gid) select o).FirstOrDefault();
                if (oMemberPrivItem != null && oMemberPrivItem.Deleted == true)
                {
                    oMemberPrivItem.Deleted = false;
                    dbEntity.SaveChanges();
                }
                if (oMemberPrivItem == null)
                {
                    MemberPrivItem oMemPrivItem = new MemberPrivItem();
                    oMemPrivItem.PrivID = gPrivID;
                    oMemPrivItem.RefID = gid;
                    dbEntity.MemberPrivItems.Add(oMemPrivItem);
                    dbEntity.SaveChanges();
                }

                //如果该程序Terminal=true，保存程序节点授权主记录
                GeneralProgram oGeneralProgram = (from o in dbEntity.GeneralPrograms where (o.Gid == gid && o.Deleted == false) select o).SingleOrDefault();
                if (oGeneralProgram != null)
                {
                    if (oGeneralProgram.Terminal == true)
                    {
                        MemberPrivilege oMemPrivilege = (from o in dbEntity.MemberPrivileges where (o.Ptype == 1 && o.UserID == gUserID) select o).FirstOrDefault();
                        if (oMemPrivilege == null)
                        {
                            MemberPrivilege oNodeMemberPrivilege = new MemberPrivilege();
                            oNodeMemberPrivilege.UserID = gUserID;
                            oNodeMemberPrivilege.Ptype = 1;
                            oNodeMemberPrivilege.Pstatus = 1;
                            dbEntity.MemberPrivileges.Add(oNodeMemberPrivilege);
                            dbEntity.SaveChanges();
                        }
                        if (oMemPrivilege != null && oMemPrivilege.Deleted == true)
                        {
                            if (oMemPrivilege.Pstatus == 0) oMemPrivilege.Pstatus = 1;
                            oMemPrivilege.Deleted = false;
                            dbEntity.SaveChanges();
                        }
                    }
                }
                return RedirectToAction("PrivProgram");
            }
            return View();
        }

        /// <summary>
        /// 加载程序树形结构
        /// </summary>
        /// <returns></returns>
        public string PrivProgramLoad()
        {
            //首次加载的，父节点为空的程序
            var oProgramList = (from o in dbEntity.GeneralPrograms.Include("ChildItems") where (o.Parent == null && o.Deleted == false) select o).ToList();
            Guid gPriID = (from o in dbEntity.MemberPrivileges where (o.Ptype == 0 && o.Deleted == false && o.UserID == gUserID && o.Pstatus == (byte)ModelEnum.GenericStatus.VALID) select o.Gid).FirstOrDefault();
            //已授权的程序
            var oRefList = (from p in dbEntity.MemberPrivItems where (p.PrivID == gPriID && p.Deleted == false) select p).ToList();
            int noRefList = oRefList.Count;
            bool flag = false;
            List<LiveTreeNode> list = new List<LiveTreeNode>();

            foreach (var item in oProgramList)
            {
                for (int i = 0; i < noRefList; i++)
                {
                    if (item.Gid == oRefList[i].RefID)
                    {
                        flag = true;
                        break;
                    }
                }
                LiveTreeNode treeNode = new LiveTreeNode();
                treeNode.id = item.Gid.ToString();
                treeNode.name = item.Name.GetResource(CurrentSession.Culture);
                if (flag == false) treeNode.nodeChecked = false;
                else treeNode.nodeChecked = true;
                if (item.Terminal==false) treeNode.isParent = true;
                else treeNode.isParent = false;
                
                treeNode.nodes = new List<LiveTreeNode>();

                list.Add(treeNode);
                flag = false;
            }
            string strTreeJson = CreateTree(list);

            return strTreeJson;
        }

        /// <summary>
        /// 展开树节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string PrivProgramExpand(Guid id)
        {
            List<LiveTreeNode> list = new List<LiveTreeNode>();
            //Guid userID = new Guid("12db0a77-92c4-e011-afef-002269c9aa85");
            //当展开root节点的时候
            if (id.Equals(Guid.Empty))
            {
                var oProgramList = dbEntity.GeneralPrograms.Where(o => o.Parent == null && o.Deleted == false).ToList();
                Guid gPriID = (from o in dbEntity.MemberPrivileges where (o.Ptype == 0 && o.Deleted == false && o.UserID == gUserID) select o.Gid).FirstOrDefault();
                var oRefList = (from p in dbEntity.MemberPrivItems where (p.PrivID == gPriID && p.Deleted == false) select p).ToList();
                int noRefList = oRefList.Count;
                bool flag = false;


                foreach (var item in oProgramList)
                {
                    for (int i = 0; i < noRefList; i++)
                    {
                        if (item.Gid == oRefList[i].RefID)
                        {
                            flag = true;
                            break;
                        }
                    }

                    LiveTreeNode treeNode = new LiveTreeNode();
                    treeNode.id = item.Gid.ToString();
                    treeNode.name = item.Name.GetResource(CurrentSession.Culture);
                    if (flag == false)
                        treeNode.nodeChecked = false;
                    else treeNode.nodeChecked = true;
                    if (item.Terminal == true)
                    {
                        treeNode.isParent = false;
                    }
                    else
                    {
                        treeNode.isParent = true;
                    }
                    treeNode.nodes = new List<LiveTreeNode>();

                    list.Add(treeNode);
                    flag = false;
                }
            }
            else
            {
                //非root节点展开的时候，回传的gid不为空
                GeneralProgram oProgramChildList = dbEntity.GeneralPrograms.Include("ChildItems").Where(p => p.Gid == id && p.Deleted == false).Single();
                Guid gPriID = (from o in dbEntity.MemberPrivileges where (o.Ptype == 0 && o.Deleted == false && o.UserID == gUserID) select o.Gid).FirstOrDefault();
                var oRefList = (from p in dbEntity.MemberPrivItems where (p.PrivID == gPriID && p.Deleted == false) select p).ToList();
                int noRefList = oRefList.Count;
                bool flag = false;
                foreach (var item in oProgramChildList.ChildItems)
                {
                    for (int i = 0; i < noRefList; i++)
                    {
                        if (item.Gid == oRefList[i].RefID)
                        {
                            flag = true;
                            break;
                        }
                    }
                    LiveTreeNode treeNode = new LiveTreeNode();
                    treeNode.id = item.Gid.ToString();
                    treeNode.name = item.Name.GetResource(CurrentSession.Culture);
                    if (flag == false) treeNode.nodeChecked = false;
                    else treeNode.nodeChecked = true;
                    if (item.Terminal == true)
                    {
                        treeNode.isParent = false;
                    }
                    else
                    {
                        treeNode.isParent = true;
                    }
                    treeNode.nodes = new List<LiveTreeNode>();

                    list.Add(treeNode);
                    flag = false;

                }
            }

            return list.ToJsonString();

        }

        /// <summary>
        /// checkbox取消时，删除数据库中保存的程序数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteProgram(string id)
        {
            Guid gid = new Guid(id);
            MemberPrivItem oMemberPrivItem = (from o in dbEntity.MemberPrivItems
                                              from p in dbEntity.MemberPrivileges
                                             where (o.RefID == gid &&o.PrivID == p.Gid && p.Ptype ==0 &&p.Deleted ==false &&p.UserID==gUserID)
                                             select o).Single();
            oMemberPrivItem.Deleted = true;
            //删除授权的程序节点
            GeneralProgram oGeneralProgram = (from o in dbEntity.GeneralPrograms where (o.Gid == gid && o.Deleted == false)select o).SingleOrDefault();
            if (oGeneralProgram != null)
            {
                //只有末级程序才有程序节点
                if (oGeneralProgram.Terminal == true)
                {
                    //程序授权时也对程序节点授权主记录做保存
                    MemberPrivilege oMemPrivilege = (from o in dbEntity.MemberPrivileges where (o.Ptype == 1 && o.UserID == gUserID)select o).FirstOrDefault();
                    if (oMemPrivilege != null)
                    {
                        Guid guid = oMemPrivilege.Gid;
                        List<MemberPrivItem> oMemPrivItem = (from p in dbEntity.MemberPrivileges 
                                           join pitem in dbEntity.MemberPrivItems on p.Gid equals pitem.PrivID
                                           join node in dbEntity.GeneralProgNodes on pitem.RefID equals node.Gid
                                           where p.Gid == oMemPrivilege.Gid && node.ProgID == oGeneralProgram.Gid
                                           select pitem).ToList() ;
                        foreach (var item in oMemPrivItem)
                        {
                            item.Deleted = true;
                        }
                    }
                }
                dbEntity.SaveChanges();
            }
            return RedirectToAction("PrivProgram");
        }

       /// <summary>
       /// 程序节点授权
       /// </summary>
       /// <returns></returns>
        public ActionResult PrivProgramNode()
        {
            //读出ProgramID下所有程序节点
            var oProgramList = (from o in dbEntity.GeneralProgNodes.Include("Name")
                               where (o.ProgID == ProgramID && o.Deleted == false)
                               select o).ToList();
            //程序节点以下拉框形式出现
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in oProgramList)
            {
                SelectListItem oitem = new SelectListItem
                {
                    Text = item.Name.GetResource(CurrentSession.Culture),
                    Value = item.Gid.ToString()
                };
                list.Add(oitem);
            }
            ViewBag.NodeList = list;

            return View();
        }

        /// <summary>
        /// 获得程序id
        /// </summary>
        /// <param name="id"></param>
        public void GetProgID(Guid id)
        {
            ProgramID = id;
        }

        /// <summary>
        /// InputMode为1时，生成下拉框
        /// </summary>
        /// <param name="id">Node的Gid</param>
        /// <returns></returns>
        public ActionResult PrivProgramDropList(Guid id)
        {
            //授权的程序节点
             var oNodeItem = (from o in dbEntity.MemberPrivItems
                             where (o.RefID == id && o.Deleted == false)
                             select o).FirstOrDefault();

             //下拉列表的内容
             var Drop = (from o in dbEntity.GeneralProgNodes
                         from p in dbEntity.GeneralResources
                         where (o.Gid == id && o.Deleted == false && o.Optional.Gid == p.Gid && p.Deleted == false)
                         select p).SingleOrDefault();
             if (Drop != null)
             {
                 string matter = Drop.GetResource(CurrentSession.Culture);
                 string[] DropMatter = matter.Split(',');
                 List<SelectListItem> list = new List<SelectListItem>();

                 for (int i = 0; i < DropMatter.Length; i++)
                 {
                     string[] test = DropMatter[i].Split('|');

                     SelectListItem item = new SelectListItem
                     {
                         Text = test[1],
                         Value = test[0]
                     };
                     if (oNodeItem != null && test[0] == oNodeItem.NodeValue) item.Selected = true;
                     list.Add(item);
                 }
                 ViewBag.olist = list;
             }
            return View();
        }

       
        /// <summary>
        /// InputMode=0,生成编辑框
        /// </summary>
        /// <returns></returns>
        public ActionResult PrivProgramEditBox(Guid id)
        {
            var oNodeItem = (from o in dbEntity.MemberPrivItems
                             where (o.RefID == id && o.Deleted == false)
                             select o).FirstOrDefault();
            if (oNodeItem != null)
            {
                ViewData["Text"] = oNodeItem.NodeValue;
            }
            else ViewData["Text"] = "";
            return View();
        }

        /// <summary>
        /// 获得ProgNode的ID
        /// </summary>
        /// <param name="id"></param>
        public void GetNodeID(Guid id)
        {
            ProgNodeID = id;
        }


        /// <summary>
        /// 保存Node的内容到数据库
        /// </summary>
        /// <returns></returns>
        public ActionResult Save(string matter)
        {
            //MemberPrivilege表中授权主记录    
            Guid gPrivID = (from o in dbEntity.MemberPrivileges
                                               where (o.Ptype == 1 && o.Deleted == false && o.UserID == gUserID)
                                               select o.Gid).SingleOrDefault();
            //授权的程序节点
            GeneralProgNode oNode = (from p in dbEntity.GeneralProgNodes
                                    where(p.Gid == ProgNodeID && p.Deleted ==false)
                                    select p).SingleOrDefault ();
            //是否对该程序节点授权过
            MemberPrivItem oMemberPrivItem = (from i in dbEntity.MemberPrivItems
                                              where (i.RefID == ProgNodeID && i.PrivID == gPrivID)
                                              select i).FirstOrDefault();
            //第一次对该程序节点授权
            if (oMemberPrivItem == null)
            {

                MemberPrivItem oMemPrivItem = new MemberPrivItem();
                oMemPrivItem.PrivID = gPrivID;
                oMemPrivItem.RefID = ProgNodeID;
                oMemPrivItem.NodeCode = oNode.Code;
                oMemPrivItem.NodeValue = matter;
                dbEntity.MemberPrivItems.Add(oMemPrivItem);
                dbEntity.SaveChanges();

            }
            //进行授权后，又进行编辑
            else
            {
                oMemberPrivItem.Deleted = false;
                oMemberPrivItem.NodeValue = matter;
                dbEntity.SaveChanges();

            }
            return View("PrivProgramNodeList");
        }

        /// <summary>
        /// 获取Node的InputMode
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string getInputMode(Guid id)
        {
            var inputMode = (from o in dbEntity.GeneralProgNodes
                             where (o.Gid == id)
                             select o.InputMode).Single();
            string json = inputMode.ToString();
            return json;
        }

        /// <summary>
        /// 将授权的程序节点以列表展示
        /// </summary>
        /// <returns></returns>
        public ActionResult PrivProgramNodeList()
        {
            return View();
        }

        /// <summary>
        /// 定义Grid的colModel
        /// </summary>
        public class JqGridColumns
        {
            public string name { get; set; }

            public string index { get; set; }

            public string width { get; set; }

        }
        /// <summary>
        /// 设置Grid的colModel的内容
        /// </summary>
        /// <returns></returns>
        public string GetColumnSettings()
        {
            string strSettings = "";
            string strColumnNames = "";
            List<JqGridColumns> ColumnList = new List<JqGridColumns>();

            JqGridColumns NameColumns = new JqGridColumns();
            NameColumns.name = "NodeName";
            NameColumns.index = "NodeName";
            NameColumns.width = "80";

            ColumnList.Add(NameColumns);

            JqGridColumns CodeColumns = new JqGridColumns();
            CodeColumns.name = "NodeCode";
            CodeColumns.index = "NodeCode";
            CodeColumns.width = "80";

            ColumnList.Add(CodeColumns);

            JqGridColumns ValueColumns = new JqGridColumns();
            ValueColumns.name = "NodeValue";
            ValueColumns.index = "NodeValue";
            ValueColumns.width = "80";

            ColumnList.Add(ValueColumns);
            for (int i = 0; i < 3; i++)
            {
                JqGridColumns currentColumn = ColumnList[i];
                strSettings += "{ \"name\": \"" + currentColumn.name + "\",\"index\": \"" + currentColumn.index + "\",\"width\": \"" + currentColumn.width + "\"},";
                strColumnNames += "\"" + currentColumn.name + "\",";
            }
            strSettings = "[" + strSettings.Substring(0, strSettings.Length - 1) +"]"+ "!" +"["+ strColumnNames.Substring(0, strColumnNames.Length - 1) + "]";
                return strSettings;
        }
        /// <summary>
        /// Grid中显示的数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetGridData()
        {
            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
            
            
            //MemberPrivilege表中的主记录Gid,对程序授权保存时亦对程序节点授权主记录做保存
            Guid gPrivID = (from p in dbEntity.MemberPrivileges
                            where (p.UserID == gUserID && p.Ptype == 1 && p.Deleted == false)
                            select p.Gid).Single();
            //已授权的节点
            var PrivNode = (from o in dbEntity.GeneralProgNodes
                           join p in dbEntity.GeneralPrograms on o.ProgID equals p.Gid
                           join m in dbEntity.MemberPrivItems on o.Gid equals m.RefID
                           where (m.PrivID == gPrivID &&p.Gid==ProgramID&& m.Deleted == false && o.Deleted == false && p.Deleted == false)
                           select o).ToList();
            
           strBuilder.Append('[');

           foreach (var item in PrivNode) 
           {
               int n = 0;
               Guid gid = item.Gid;
               MemberPrivItem oMemberPrivItem = (from o in dbEntity.MemberPrivItems where (o.RefID == gid && o.PrivID == gPrivID && o.Deleted == false) select o).SingleOrDefault();
               if (oMemberPrivItem != null)
               {
                   //生成Json数据
                   strBuilder.Append('{');
                   strBuilder.AppendFormat("NodeName:\"{0}\"", item.Name.GetResource(CurrentSession.Culture));
                   strBuilder.Append(',');
                   strBuilder.AppendFormat("NodeCode:\"{0}\"", item.Code);
                   strBuilder.Append(',');


                   //下拉列表的内容
                   var Drop = (from o in dbEntity.GeneralProgNodes
                               from p in dbEntity.GeneralResources
                               where (o.Gid == gid && o.Deleted == false && o.Optional.Gid == p.Gid && p.Deleted == false)
                               select p).Single();
                   string matter = Drop.GetResource(CurrentSession.Culture);
                   string[] DropMatter = matter.Split(',');

                   for (int i = 0; i < DropMatter.Length; i++)
                   {
                       string[] test = DropMatter[i].Split('|');

                       if (test[0] == oMemberPrivItem.NodeValue)
                       {
                           strBuilder.AppendFormat("NodeValue:\"{0}\"", test[1]);
                       }
                   }

                   strBuilder.Append('}');
                   n++;
                   if (n != PrivNode.Count)
                       strBuilder.Append(',');
               }

           }
            
            strBuilder.Append(']');

            string strJson = strBuilder.ToString();
            return Json(strJson, JsonRequestBehavior.AllowGet);
        
        }

#endregion

        #region 产品授权
        public ActionResult PrivProductTree()
        {
            return View();
        }
        /// <summary>
        /// 商品分类授权
        /// </summary>
        /// <returns></returns>
        public ActionResult PrivProduct(Guid? id=null)
        {
            if (!base.CheckPrivilege("EnableEdit"))//权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            if (id == null)
            {
                int nstatus;
                MemberPrivilege oMemberPrivilege = (from o in dbEntity.MemberPrivileges where (o.Deleted == false && o.UserID == gUserID && o.Ptype == (byte)ModelEnum.UserPrivType.PRODUCT_CATEGORY) select o).FirstOrDefault();
                if (oMemberPrivilege != null)
                {
                    //获取状态，1启用，0禁用
                    nstatus = (int)oMemberPrivilege.Pstatus;
                }
                else 
                    nstatus = -1;
                ViewBag.status = nstatus;
                //组织下拉框
                ViewData["Orglist"] = GetGuserSupportOrganizations();
            }
            else
            {
                Guid gPrivID = new Guid();
                MemberPrivilege oProPrivID = (from o in dbEntity.MemberPrivileges where (o.UserID == gUserID && o.Ptype == 5) select o).FirstOrDefault();
                //主表中已存在授权记录
                if (oProPrivID != null && oProPrivID.Deleted == false)
                {
                    if (oProPrivID.Pstatus == 0)
                    {
                        oProPrivID.Pstatus = 1;
                        dbEntity.SaveChanges();
                    }
                    gPrivID = oProPrivID.Gid;
                }
                //主表中的授权记录已被逻辑删除，恢复
                if (oProPrivID != null && oProPrivID.Deleted == true)
                {
                    oProPrivID.Deleted = false;
                    oProPrivID.Pstatus = 1;
                    dbEntity.SaveChanges();
                    gPrivID = oProPrivID.Gid;
                }
                //不存在授权记录，添加
                if (oProPrivID == null)
                {
                    MemberPrivilege oMemPriv = new MemberPrivilege();
                    oMemPriv.UserID = gUserID;
                    oMemPriv.Ptype = 5;
                    oMemPriv.Pstatus = 1;
                    dbEntity.MemberPrivileges.Add(oMemPriv);
                    dbEntity.SaveChanges();
                    gPrivID = oMemPriv.Gid;
                }

                MemberPrivItem oProPriItem = (from o in dbEntity.MemberPrivItems where (o.RefID == id && o.PrivID == gPrivID) select o).SingleOrDefault();
                if (oProPriItem != null && oProPriItem.Deleted == true)
                {
                    oProPriItem.Deleted = false;
                    dbEntity.SaveChanges();
                }
                if (oProPriItem == null)
                {
                    MemberPrivItem oProItem = new MemberPrivItem();
                    oProItem.PrivID = gPrivID;
                    oProItem.RefID = id;
                    dbEntity.MemberPrivItems.Add(oProItem);
                    dbEntity.SaveChanges();
                }

                return RedirectToAction("PrivProduct");
            }
            return View();
        }
        /// <summary>
        /// 改变全局变量OrgID的值
        /// </summary>
        /// <param name="id"></param>
        public void GetID(Guid id)
        {
            gOrgID = id;
        }
        /// <summary>
        /// 加载商品授权树
        /// </summary>
        /// <returns></returns>
        public string PrivProductLoad()
        {
            Guid gOrg = (from o in dbEntity.MemberUsers where (o.Gid == gUserID && o.Deleted == false) select o.OrgID).Single();
            List<GeneralPrivateCategory> Category;
            List<Guid?> oRefList;
            List<LiveTreeNode> list = new List<LiveTreeNode>();
            //商品私有分类
            Category = (from o in dbEntity.GeneralPrivateCategorys.Include("Name").Include("ChildItems") where (o.OrgID == gOrgID && o.Parent == null &&o.Ctype==0&& o.Deleted == false) select o).ToList();
            //已经被授权的商品分类
            oRefList = (from o in dbEntity.MemberPrivileges
                        from p in dbEntity.MemberPrivItems
                        where (o.Ptype == 5 && o.Deleted == false && o.Gid == p.PrivID && p.Deleted == false)select p.RefID).ToList();

            foreach (var item in Category)
            {
                LiveTreeNode node = new LiveTreeNode();
                node.name = item.Name.GetResource(CurrentSession.Culture);
                node.id = item.Gid.ToString();
                node.nodeChecked = oRefList.Any(id => id == item.Gid);
                node.isParent = (item.ChildItems.Any(c => !c.Deleted));
                list.Add(node);
            }
            return list.ToJsonString();
        }

        /// <summary>
        /// 展开商品授权树节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string PrivProductExpand(Guid? id=null)
        {
            var ProductList = (from o in dbEntity.GeneralPrivateCategorys.Include("Parent").Include("ChildItems").Include("Name") where (o.Parent.Gid == id && o.Deleted == false) select o).ToList();
            
            var oRefList = (from o in dbEntity.MemberPrivileges
                           from p in dbEntity.MemberPrivItems
                           where (o.Ptype == 5 && o.Deleted == false && o.Gid == p.PrivID && p.Deleted == false) select new{id=p.RefID}).ToList();
            int noRefList = oRefList.Count;
            bool flag = false;
            List<LiveTreeNode> list = new List<LiveTreeNode>();
            foreach (var Proitem in ProductList)
            {
                for (int i = 0; i < noRefList; i++)
                {
                    if (Proitem.Gid == oRefList[i].id)
                    {
                        flag = true;
                        break;
                    }
                }
                LiveTreeNode node = new LiveTreeNode();
                node.name = Proitem.Name.GetResource(CurrentSession.Culture);
                node.id = Proitem.Gid.ToString();
                if (flag == false) node.nodeChecked = false;
                else node.nodeChecked = true;
                if (Proitem.ChildItems.Count > 0) node.isParent = true;
                else node.isParent = false;
                node.nodes = new List<LiveTreeNode>();
                list.Add(node);
                flag = false;
            }
            return list.ToJsonString();
        }

        /// <summary>
        /// checkbox取消时，到数据库中删除已保存节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteProduct(Guid id)
        {
            Guid gPrivID = (from o in dbEntity.MemberPrivileges where (o.UserID == gUserID && o.Deleted == false && o.Ptype == 5) select o.Gid).SingleOrDefault();
            if (gPrivID != null)
            {
                MemberPrivItem oSupItem = (from o in dbEntity.MemberPrivItems where (o.Deleted == false && o.PrivID == gPrivID && o.RefID == id) select o).SingleOrDefault();
                if (oSupItem != null)
                {
                    oSupItem.Deleted = true;
                    dbEntity.SaveChanges();
                }
            }
            return RedirectToAction("PrivProduct");
        }

#endregion

        #region 供应商授权
        /// <summary>
        /// 供应商授权
        /// </summary>
        /// <returns></returns>
        public ActionResult PrivSupplier(Guid? id=null)
        {
            if (!base.CheckPrivilege("EnableEdit"))//权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            if (id == null)
            {
                int nstatus;
                MemberPrivilege PrivID = (from o in dbEntity.MemberPrivileges where (o.Deleted == false && o.UserID == gUserID && o.Ptype == (byte)ModelEnum.UserPrivType.SUPPLIER_CATEGORY) select o).FirstOrDefault();
                if (PrivID != null)
                {
                    nstatus = (int)PrivID.Pstatus;
                }
                else nstatus = -1;
                ViewBag.status = nstatus;
                Guid gUserOrgID = (from o in dbEntity.MemberUsers where (o.Gid == gUserID && o.Deleted == false) select o.OrgID).Single();
                List<SelectListItem> list = new List<SelectListItem>();
                //组织下拉框
                ViewData["Orglist"] = GetGuserSupportOrganizations();
            }
            else
            {
                Guid gPrivID;
                MemberPrivilege PrivID = (from o in dbEntity.MemberPrivileges where (o.UserID == gUserID && o.Ptype == (byte)ModelEnum.UserPrivType.SUPPLIER_CATEGORY) select o).FirstOrDefault();
                //主表中存在授权记录
                if (PrivID != null && PrivID.Deleted == false)
                {
                    if (PrivID.Pstatus == 0)
                    {
                        PrivID.Pstatus = 1;
                        dbEntity.SaveChanges();
                    }
                    gPrivID = PrivID.Gid;

                }
                //主表中的授权记录被删除，恢复记录
                else if (PrivID != null && PrivID.Deleted == true)
                {
                    PrivID.Deleted = false;
                    if (PrivID.Pstatus == 0) PrivID.Pstatus = 1;
                    dbEntity.SaveChanges();
                    gPrivID = PrivID.Gid;
                }
                //主表中不存在授权记录
                else
                {
                    MemberPrivilege oMemPriv = new MemberPrivilege();
                    oMemPriv.UserID = gUserID;
                    oMemPriv.Ptype = 6;
                    oMemPriv.Pstatus = 1;
                    dbEntity.MemberPrivileges.Add(oMemPriv);
                    dbEntity.SaveChanges();
                    gPrivID = oMemPriv.Gid;
                }
                if (!id.Equals(Guid.Empty))
                {
                    MemberPrivItem oSupItem = (from o in dbEntity.MemberPrivItems
                                               where (o.PrivID == gPrivID && o.RefID == id)
                                               select o).FirstOrDefault();
                    //从表中存在这条记录，但已经逻辑删除，恢复记录
                    if (oSupItem != null && oSupItem.Deleted == true)
                    {
                        oSupItem.Deleted = false;
                        dbEntity.SaveChanges();
                    }
                    //不存在记录，添加
                    if (oSupItem == null)
                    {
                        MemberPrivItem oSupItem1 = new MemberPrivItem();
                        oSupItem1.PrivID = gPrivID;
                        oSupItem1.RefID = id;
                        dbEntity.MemberPrivItems.Add(oSupItem1);
                        dbEntity.SaveChanges();
                    }
                }
                return RedirectToAction("PrivSupplier");
            }
            return View();
        }
        public ActionResult PrivSupplierTree()
        {
            return View();
        }
        /// <summary>
        /// 加载供应商树形结构
        /// </summary>
        /// <returns></returns>
        public string PrivSupplierLoad()
        {
            var oSupplierList = (from o in dbEntity.PurchaseSuppliers.Include("FullName").Include("ChildItems") where (o.aParent == gOrgID && o.Deleted == false) select o).ToList();
            int noSupplierList = oSupplierList.Count;

            var oRefIDList = (from o in dbEntity.MemberPrivItems
                              from p in dbEntity.PurchaseSuppliers
                              where (o.RefID == p.Gid && o.Deleted == false && p.Otype == (byte)ModelEnum.OrganizationType.SUPPLIER && p.Deleted == false) select new { gid = o.RefID }).ToList();
            int noRefIDList = oRefIDList.Count;
            List<LiveTreeNode> list = new List<LiveTreeNode>();
            bool flag = false;
            foreach (var item in oSupplierList)
            {
                LiveTreeNode node = new LiveTreeNode();
                //判断节点是否是选中
                for (int i = 0; i < noRefIDList; i++)
                {
                    if (item.Gid == oRefIDList[i].gid)
                    {
                        flag = true;
                        break;
                    }
                }
                node.name = item.FullName.GetResource(CurrentSession.Culture);
                node.id = item.Gid.ToString();
                if (flag == false) node.nodeChecked = false;
                else node.nodeChecked = true;
                if (item.ChildItems.Count > 0) node.isParent = true;
                else node.isParent = false;
                node.nodes = new List<LiveTreeNode>();
                list.Add(node);
                flag = false;
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
            bool flag = false;
            List<LiveTreeNode> list = new List<LiveTreeNode>();
            if (id.Equals(Guid.Empty))
            {
                var oSuppierList =(from o in dbEntity.PurchaseSuppliers.Include("FullName").Include("ChildItems") where (o.Parent == null && o.Deleted == false) select o).ToList();
                int noSupplierList = oSuppierList.Count;
                var oRefIDList = (from o in dbEntity.MemberPrivItems
                                  from p in dbEntity.MemberOrganizations
                                  where (o.RefID == p.Gid && o.Deleted == false && p.Otype == (byte)ModelEnum.OrganizationType.SUPPLIER && p.Deleted == false) select new { gid = o.RefID }).ToList();
                int noRefIDList = oRefIDList.Count;
                foreach (var item in oSuppierList)
                {   
                    LiveTreeNode node = new LiveTreeNode();
                    for (int i = 0; i < noRefIDList; i++)
                    {
                        if (item.Gid == oRefIDList[i].gid)
                        {
                            flag = true;
                            break;
                        }
                    }
                    node.name = item.FullName.GetResource(CurrentSession.Culture);
                    node.id = item.Gid.ToString();
                    if (flag == false) node.nodeChecked = false;
                    else node.nodeChecked = true;
                    if (item.ChildItems.Count > 0) node.isParent = true;
                    else node.isParent = false;
                    node.nodes = new List<LiveTreeNode>();
                    list.Add(node);
                    flag = false;
                }
            }
            else
            {
                
                var SupplierList = (from o in dbEntity.MemberOrganizations.Include("FullName").Include("ChildItems") where (o.Parent.Gid == id && o.Deleted == false) select o).ToList();
                var SupItemList = (from o in dbEntity.MemberPrivItems
                                   from p in dbEntity.MemberOrganizations
                                   where (o.RefID == p.Gid && o.Deleted == false && p.Deleted == false && p.Otype == 3) select new { gid = o.RefID }).ToList();
                int nSupItemList = SupItemList.Count;
                foreach (var item in SupplierList)
                {
                    for (int i = 0; i < nSupItemList; i++)
                    {
                        if (item.Gid == SupItemList[i].gid)
                        {
                            flag = true;
                            break;
                        }
                    }
                    LiveTreeNode node = new LiveTreeNode();
                    node.name = item.FullName.GetResource(CurrentSession.Culture);
                    node.id = item.Gid.ToString();
                    if (flag == true) node.nodeChecked = true;
                    else node.nodeChecked = false;
                    if (item.ChildItems.Count > 0) node.isParent = true;
                    else node.isParent = false;
                    node.nodes = new List<LiveTreeNode>();
                    list.Add(node);
                    flag = false;
                }
            }
            return list.ToJsonString();
        }        

        /// <summary>
        /// 删除树节点
        /// </summary>
        /// <param name="result"></param>
        /// <param name="open"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteSupplier(Guid id)
        {
            Guid gPrivID = (from o in dbEntity.MemberPrivileges where (o.UserID == gUserID && o.Deleted == false && o.Ptype == 6) select o.Gid).SingleOrDefault();
            if (gPrivID != null)
            {
                MemberPrivItem oSupItem = (from o in dbEntity.MemberPrivItems where (o.Deleted == false && o.PrivID == gPrivID && o.RefID == id) select o).SingleOrDefault();
                if (oSupItem != null)
                {
                    oSupItem.Deleted = true;
                    dbEntity.SaveChanges();
                }
            }
            return RedirectToAction("PrivSupplier");
        }

        #endregion

        #region 公用方法

        /// <summary>
        /// 返回正在授权的用户
        /// </summary>
        /// <returns></returns>
        public Guid getUser()
        {
            return gUserID;
        }

        /// <summary>
        /// 启用按钮状态变化时调用
        /// </summary>
        /// <param name="IsCheck">是否启用</param>
        /// <param name="ptype">ptype</param>
        /// <returns></returns>
        public string DelAll(bool IsCheck, byte ptype)
        {
            MemberPrivilege oMemberPrivilege = (from o in dbEntity.MemberPrivileges
                                                where (o.Ptype == ptype && o.UserID == gUserID)
                                                select o).FirstOrDefault();
            if (!IsCheck && oMemberPrivilege != null)
            {
                oMemberPrivilege.Pstatus = (byte)ModelEnum.GenericStatus.NONE;
                dbEntity.SaveChanges();
            }
            else
            {
                if (oMemberPrivilege != null)//如果原来用户在MemberPrivilege中有该Ptype的记录，delete置为false
                {
                    oMemberPrivilege.Pstatus = (byte)ModelEnum.GenericStatus.VALID;
                    oMemberPrivilege.Deleted = false;
                }
                else//如果原来用户在MemberPrivilege中没有该Ptype的记录，则新建
                {
                    MemberPrivilege oNewPriv = new MemberPrivilege {
                        Pstatus = (byte)ModelEnum.GenericStatus.VALID,
                        UserID = gUserID,
                        Ptype = ptype
                    };
                    dbEntity.MemberPrivileges.Add(oNewPriv);
                }
                dbEntity.SaveChanges();
            }
            return null;
        }

        /// <summary>
        /// 获取当前被授权用户的有权限的组织
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetGuserSupportOrganizations()
        {
            List<MemberOrganization> oUserOrganization = new List<MemberOrganization>();
            Guid gUserOrgID = dbEntity.MemberUsers.Find(gUserID).OrgID;
            gOrgID = gUserOrgID;
            try
            {
                byte IsValde = dbEntity.MemberPrivileges.Where(p => p.UserID == gUserID && p.Ptype == (byte)ModelEnum.UserPrivType.ORGANIZATION).FirstOrDefault().Pstatus;
                if (IsValde == 1)//如果是启用状态，则将权限表中查得的组织添加到列表
                {
                    oUserOrganization = (from pi in dbEntity.MemberPrivItems.AsEnumerable()
                                            join p in dbEntity.MemberPrivileges.AsEnumerable() on pi.PrivID equals p.Gid
                                            join org in dbEntity.MemberOrganizations.AsEnumerable() on pi.RefID equals org.Gid
                                         where pi.Deleted == false && p.UserID == gUserID && p.Ptype == (byte)ModelEnum.UserPrivType.ORGANIZATION
                                            orderby org.Sorting descending
                                            select org).ToList();
                }
                //将自己所属组织加到列表
                MemberOrganization currentOrg = null;
                if(gUserOrgID != Guid.Empty || gUserOrgID != null)
                    currentOrg = dbEntity.MemberOrganizations.Where(o => o.Deleted == false && o.Gid == gUserOrgID).FirstOrDefault();
                oUserOrganization.Add(currentOrg);
            }
            catch (Exception e)//
            {
                oEventBLL.WriteEvent(e.ToString());
            }

            List<SelectListItem> ogranizationList = new List<SelectListItem>();
            foreach (MemberOrganization item in oUserOrganization)
            {
                if (item.Gid == gUserOrgID)//默认选中自己所属组织
                    ogranizationList.Add(new SelectListItem { Text = item.FullName.GetResource(CurrentSession.Culture), Value = item.Gid.ToString(), Selected = true });
                else
                    ogranizationList.Add(new SelectListItem { Text = item.FullName.GetResource(CurrentSession.Culture), Value = item.Gid.ToString(), Selected = false });
            }
            return ogranizationList;
        }

        #endregion
    }
}
