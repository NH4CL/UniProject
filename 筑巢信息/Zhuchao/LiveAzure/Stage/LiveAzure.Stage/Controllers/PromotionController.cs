using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC.Controls;
using LiveAzure.Models;
using LiveAzure.Models.Member;
using LiveAzure.Models.Order;
using MVC.Controls.Grid;
using System.Globalization;
using LiveAzure.Models.Product;
using System.Collections;

namespace LiveAzure.Stage.Controllers
{
    public class PromotionController : BaseController
    {
        //
        // GET: /Promotion/ 
        public static int nCodeLenth = 16;
        public static int nPasswordLenth = 16;

        private static Guid promotionOrgGid = new Guid();
        private static Guid promotionChlGid = new Guid();
        //添加或编辑促销信息时的全局变量，默认是添加状态
        private static bool bEdit = false;
        //是否覆盖已存在的促销方案
        private static bool bCoverExist = false;
        //全局促销Gid
        private static Guid globlePromotionGid = new Guid();
        //添加或编辑促销关系的全局变量，默认是添加状态
        private static bool bMutexEdit = false;
        private static bool bCoverMutexExist = false;
        private static bool bCoverProductExist = false;
        //全局促销互斥Gid
        private static Guid globleMutexGid = new Guid();

        #region 首页以及列表页面
        public ActionResult Index()
        {
            globlePromotionGid = new Guid();
            globleMutexGid = new Guid();
            bEdit = false;
            bCoverExist = false;
            bMutexEdit = false;
            bCoverMutexExist = false;
            bCoverProductExist = false;

            Guid currentUserGid = (Guid)CurrentSession.UserID;
            List<SelectListItem> oOrgList = new List<SelectListItem>();
            List<SelectListItem> oChlList = new List<SelectListItem>();
            if (promotionOrgGid.Equals(Guid.Empty))
            {
                if (CurrentSession.IsAdmin)
                {
                    //当前用户是admin
                    List<MemberOrganization> oMemberOrg = dbEntity.MemberOrganizations.Include("FullName").Where(p => p.Deleted == false).ToList();
                    for (int i = 0; i < oMemberOrg.Count; i++)
                    {
                        SelectListItem item = new SelectListItem { Text = oMemberOrg.ElementAt(i).FullName.GetResource(CurrentSession.Culture), Value = oMemberOrg.ElementAt(i).Gid.ToString() };
                        oOrgList.Add(item);
                    }
                    ViewBag.oOrgList = oOrgList;
                    Guid currentOrgGid = Guid.Parse(oOrgList.ElementAt(0).Value);
                    promotionOrgGid = Guid.Parse(oOrgList.ElementAt(0).Value);
                    List<MemberOrgChannel> oMemberChannel = dbEntity.MemberOrgChannels.Include("Channel").Where(p => p.OrgID == currentOrgGid && p.Deleted == false).ToList();
                    for (int i = 0; i < oMemberChannel.Count; i++)
                    {
                        SelectListItem item = new SelectListItem { Text = oMemberChannel.ElementAt(i).Channel.FullName.GetResource(CurrentSession.Culture), Value = oMemberChannel.ElementAt(i).Channel.Gid.ToString() };
                        oChlList.Add(item);
                    }
                    ViewBag.oChlList = oChlList;
                    promotionChlGid = Guid.Parse(oChlList.ElementAt(0).Value);
                }
                else
                {
                    oOrgList = GetPrivilegedOrgs(currentUserGid);
                    ViewBag.oOrgList = oOrgList;
                    promotionOrgGid = Guid.Parse(oOrgList.ElementAt(0).Value);
                    oChlList = GetPrivilegedChls(promotionOrgGid, currentUserGid);
                    ViewBag.oChlList = oChlList;
                    promotionChlGid = Guid.Parse(oChlList.ElementAt(0).Value);
                }
            }
            else 
            {
                if (CurrentSession.IsAdmin)
                {
                    //当前用户是admin
                    List<MemberOrganization> oMemberOrg = dbEntity.MemberOrganizations.Include("FullName").Where(p => p.Deleted == false).ToList();
                    for (int i = 0; i < oMemberOrg.Count; i++)
                    {
                        SelectListItem item = new SelectListItem { Text = oMemberOrg.ElementAt(i).FullName.GetResource(CurrentSession.Culture), Value = oMemberOrg.ElementAt(i).Gid.ToString() };
                        if (oMemberOrg.ElementAt(i).Gid == promotionOrgGid)
                        {
                            item.Selected = true;
                        }
                        oOrgList.Add(item);
                    }
                    ViewBag.oOrgList = oOrgList;
                    List<MemberOrgChannel> oMemberChannel = dbEntity.MemberOrgChannels.Include("Channel").Where(p => p.OrgID == promotionOrgGid && p.Deleted == false).ToList();
                    for (int i = 0; i < oMemberChannel.Count; i++)
                    {
                        SelectListItem item = new SelectListItem { Text = oMemberChannel.ElementAt(i).Channel.FullName.GetResource(CurrentSession.Culture), Value = oMemberChannel.ElementAt(i).Channel.Gid.ToString() };
                        oChlList.Add(item);
                    }
                    ViewBag.oChlList = oChlList;
                    promotionChlGid = Guid.Parse(oChlList.ElementAt(0).Value);
                }
                else
                {
                    oOrgList = GetPrivilegedOrgs(currentUserGid);
                    for (int i = 0; i < oOrgList.Count; i++)
                    {
                        if (oOrgList.ElementAt(i).Value == promotionOrgGid.ToString())
                        {
                            oOrgList.ElementAt(i).Selected = true;
                            break;
                        }
                    }
                    ViewBag.oOrgList = oOrgList;
                    oChlList = GetPrivilegedChls(promotionOrgGid, currentUserGid);
                    ViewBag.oChlList = oChlList;
                    promotionChlGid = Guid.Parse(oChlList.ElementAt(0).Value);
                }
            }

            return View();
        }

        /// <summary>
        /// 促销方案列表页面
        /// </summary>
        /// <returns></returns>
        public ActionResult PromotionList() 
        {            
            return View();
        }
        /// <summary>
        /// 根据用户GID来取组织的列表
        /// </summary>
        /// <param name="userGid"></param>
        /// <returns></returns>
        public List<SelectListItem> GetPrivilegedOrgs(Guid userGid)
        {
            List<SelectListItem> orgList = new List<SelectListItem>();
            MemberPrivilege oMemberPrivilege = dbEntity.MemberPrivileges.Include("PrivilegeItems").Where(p => p.Deleted == false && p.UserID == userGid && p.Ptype == 2).FirstOrDefault();
            //如果有授权的组织，则将授权组织加入组织下拉列表；否则加入用户默认的组织；
            if (oMemberPrivilege != null)
            {
                for (int i = 0; i < oMemberPrivilege.PrivilegeItems.Count; i++)
                {
                    Guid orgGid = (Guid)oMemberPrivilege.PrivilegeItems.ElementAt(i).RefID;
                    MemberOrganization oOrganization = dbEntity.MemberOrganizations.Include("FullName").Where(p => p.Gid == orgGid && p.Deleted == false).FirstOrDefault();
                    if (oOrganization != null)
                    {
                        SelectListItem item = new SelectListItem { Text = oOrganization.FullName.GetResource(CurrentSession.Culture), Value = oOrganization.Gid.ToString() };
                        orgList.Add(item);
                    }
                }
            }
            else 
            {
                MemberUser oUser = dbEntity.MemberUsers.Include("Organization").Where(p => p.Gid == userGid && p.Deleted == false).FirstOrDefault();
                SelectListItem item = new SelectListItem { Text = oUser.Organization.FullName.GetResource(CurrentSession.Culture), Value = oUser.Organization.Gid.ToString() };
                orgList.Add(item);
            }
            return orgList;
        }
        /// <summary>
        /// 根据用户Guid以及组织Guid查找支持以及授权的渠道
        /// </summary>
        /// <param name="orgGid"></param>
        /// <param name="userGid"></param>
        /// <returns></returns>
        public List<SelectListItem> GetPrivilegedChls(Guid orgGid, Guid userGid)
        {
            List<SelectListItem> chlList = new List<SelectListItem>();
            List<MemberOrgChannel> oMemChannelList = dbEntity.MemberOrgChannels.Include("Channel").Where(p => p.OrgID == orgGid && p.Deleted == false).ToList();
            List<Guid> channelGidList = new List<Guid>();
            //将组织支持的渠道列表转变为Guid列表，方便后面做交集
            for(int j = 0;j<oMemChannelList.Count;j++)
            {
                Guid currentChlGid = oMemChannelList.ElementAt(j).ChlID;
                channelGidList.Add(currentChlGid);
            }
            MemberPrivilege oMemberPrivilege = dbEntity.MemberPrivileges.Include("PrivilegeItems").Where(p => p.Deleted == false && p.UserID == userGid && p.Ptype == 3).FirstOrDefault();
            //如果存在授权的渠道，则将授权的组织所支持的所有授权渠道加入渠道下拉列表；否则加入授权的组织所支持的所有渠道；
            if (oMemberPrivilege != null)
            {
                for (int i = 0; i < oMemberPrivilege.PrivilegeItems.Count; i++)
                {
                    //如果权限的渠道在组织支持的渠道中，则加入渠道下拉列表
                    if (channelGidList.Contains((Guid)oMemberPrivilege.PrivilegeItems.ElementAt(i).RefID))
                    {
                        Guid channelGid = (Guid)oMemberPrivilege.PrivilegeItems.ElementAt(i).RefID;
                        MemberChannel oMemberChannel = dbEntity.MemberChannels.Include("FullName").Where(p => p.Gid == channelGid && p.Deleted == false).FirstOrDefault();
                        if (oMemberChannel != null)
                        {
                            SelectListItem item = new SelectListItem { Text = oMemberChannel.FullName.GetResource(CurrentSession.Culture), Value = oMemberChannel.Gid.ToString() };
                            chlList.Add(item);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < oMemChannelList.Count; i++)
                {
                    SelectListItem item = new SelectListItem { Text = oMemChannelList.ElementAt(i).Channel.FullName.GetResource(CurrentSession.Culture), Value = oMemChannelList.ElementAt(i).ChlID.ToString() };
                    chlList.Add(item);
                }
            }
            return chlList;
        }
        /// <summary>
        /// 设置全局的渠道Guid
        /// </summary>
        /// <param name="chlGid"></param>
        public void ChangeChlGid(Guid chlGid) 
        {
            promotionChlGid = chlGid;
        }
        /// <summary>
        /// 设置全局的组织Guid
        /// </summary>
        /// <param name="orgGid"></param>
        public void ChangeOrgGid(Guid orgGid)
        {
            promotionOrgGid = orgGid;
        }
        /// <summary>
        /// 促销列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListPromotionInfomation(SearchModel searchModel) 
        {
            //获取当前用户的语言
            CultureInfo culture = new CultureInfo(CurrentSession.Culture);
            IQueryable<PromotionInformation> oPromotionInfo = dbEntity.PromotionInformations.Include("Name").Where(p => p.Deleted == false && p.OrgID == promotionOrgGid && p.ChlID == promotionChlGid).AsQueryable();
            GridColumnModelList<PromotionInformation> columns = new GridColumnModelList<PromotionInformation>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Code);
            columns.Add(p => p.Name == null ? "" : p.Name.GetResource(CurrentSession.Culture)).SetName("PromotionName");
            columns.Add(p => p.Matter);
            columns.Add(p => p.Pstatus);
            columns.Add(p => p.IssueType);
            columns.Add(p => p.IssueStart == null ? "" : p.IssueStart.ToString(culture.DateTimeFormat.ShortDatePattern)).SetName("IssueStart");
            columns.Add(p => p.IssueEnd == null ? "" : p.IssueEnd.ToString(culture.DateTimeFormat.ShortDatePattern)).SetName("IssueEnd");
            columns.Add(p => p.StartTime == null ? "" : p.StartTime.ToString(culture.DateTimeFormat.ShortDatePattern)).SetName("StartName");
            columns.Add(p => p.EndTime == null ? "" : p.EndTime.ToString(culture.DateTimeFormat.ShortDatePattern)).SetName("EndTime");
            columns.Add(p => p.EffectDays);

            GridData gridData = oPromotionInfo.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 促销Tab页

        /// <summary>
        /// 将全局的编辑或者添加页面设置为页面操作的状态
        /// </summary>
        /// <param name="bAddOrEdit"></param>
        public void SetbEdit(bool bAddOrEdit, Guid? promotionGid)
        {
            bEdit = bAddOrEdit;
            if (bEdit == true)
            {
                globlePromotionGid = (Guid)promotionGid;
            }
        }
        /// <summary>
        /// 用于判断当前是添加还是编辑状态
        /// </summary>
        /// <returns></returns>
        public bool GetbEdit()
        {
            return bEdit;
        }
        /// <summary>
        /// 促销Tab页面
        /// </summary>
        /// <returns></returns>
        public ActionResult PromotionTab()
        {
            return View();
        }

        #endregion

        #region 促销基本信息编辑

        /// <summary>
        /// 促销信息编辑页面
        /// </summary>
        /// <param name="bEdit"></param>
        /// <returns></returns>
        public ActionResult PromotionInfoEditPage()
        {
            ViewBag.bEdit = bEdit;
            PromotionInformation oNewPromotion;
            CultureInfo culture = new CultureInfo(CurrentSession.Culture);
            //判断是编辑还是添加促销
            if (bEdit == false)
            {
                oNewPromotion = new PromotionInformation { Name = NewResource(ModelEnum.ResourceType.STRING, promotionOrgGid) };
                oNewPromotion.OrgID = promotionOrgGid;
                oNewPromotion.ChlID = promotionChlGid;
                MemberOrganization oCurrentOrg = dbEntity.MemberOrganizations.Include("FullName").Where(p => p.Gid == promotionOrgGid && p.Deleted == false).FirstOrDefault();
                if (oCurrentOrg != null)
                {
                    ViewBag.orgFullName = oCurrentOrg.FullName.GetResource(CurrentSession.Culture);
                }
                else
                {
                    ViewBag.orgFullName = "";
                }
                MemberChannel oCurrentChl = dbEntity.MemberChannels.Include("FullName").Where(p => p.Gid == promotionChlGid && p.Deleted == false).FirstOrDefault();
                if (oCurrentChl != null)
                {
                    ViewBag.chlFullName = oCurrentChl.FullName.GetResource(CurrentSession.Culture);
                }
                else 
                {
                    ViewBag.chlFullName = "";
                }
                ViewData["issueStartTime"] = "";
                ViewData["issueEndTime"] = "";
                ViewData["validateStartTime"] = "";
                ViewData["validateEndTime"] = "";
            }
            else 
            {
                Guid promotionGid = globlePromotionGid;
                oNewPromotion = dbEntity.PromotionInformations.Include("Organization").Include("Channel").Include("Name").Where(p => p.Gid == promotionGid && p.Deleted == false).FirstOrDefault();
                oNewPromotion.Name = RefreshResource(ModelEnum.ResourceType.STRING, oNewPromotion.Name, promotionOrgGid);
                if (oNewPromotion != null)
                {
                    ViewBag.orgFullName = oNewPromotion.Organization.FullName.GetResource(CurrentSession.Culture);
                    ViewBag.chlFullName = oNewPromotion.Channel.FullName.GetResource(CurrentSession.Culture);
                    ViewData["issueStartTime"] = oNewPromotion.IssueStart.ToString(culture.DateTimeFormat.ShortDatePattern);
                    ViewData["issueEndTime"] = oNewPromotion.IssueEnd.ToString(culture.DateTimeFormat.ShortDatePattern);
                    ViewData["validateStartTime"] = oNewPromotion.StartTime.ToString(culture.DateTimeFormat.ShortDatePattern);
                    ViewData["validateEndTime"] = oNewPromotion.EndTime.ToString(culture.DateTimeFormat.ShortDatePattern);
                }
                else 
                {
                    ViewBag.orgFullName = "";
                    ViewBag.chlFullName = "";
                    ViewData["issueStartTime"] = "";
                    ViewData["issueEndTime"] = "";
                    ViewData["validateStartTime"] = "";
                    ViewData["validateEndTime"] = "";
                }
            }

            //促销方案状态
            List<SelectListItem> oPstatusList = new List<SelectListItem>();
            oPstatusList = GetSelectList(oNewPromotion.PromotionStatusList);
            ViewBag.oPstatusList = oPstatusList;

            //促销发放方式
            List<SelectListItem> oIssueTypeList = new List<SelectListItem>();
            oIssueTypeList.Add(new SelectListItem { Text = "线上", Value = "0" });
            oIssueTypeList.Add(new SelectListItem { Text = "线下", Value = "1" });
            ViewBag.oIssueTypeList = oIssueTypeList;

            //促销对应的程序
            List<SelectListItem> oPtypeList = new List<SelectListItem>();
            oPtypeList.Add(new SelectListItem { Text = "程序0", Value = "0" });
            oPtypeList.Add(new SelectListItem { Text = "程序1", Value = "1" });
            oPtypeList.Add(new SelectListItem { Text = "程序2", Value = "2" });
            ViewBag.oPtypeList = oPtypeList;

            return View(oNewPromotion);
        }
        /// <summary>
        /// 编辑或者添加促销信息
        /// </summary>
        /// <param name="backPromotion"></param>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public string SavePromotionInfo(PromotionInformation backPromotion, FormCollection formCollection) 
        {
            Guid currentPromotionGid = backPromotion.Gid;
            PromotionInformation oEditPromotionInfo;
            if (bEdit == false)
            {
                oEditPromotionInfo = new PromotionInformation { Name = NewResource(ModelEnum.ResourceType.STRING, promotionOrgGid) };
                oEditPromotionInfo.Name = backPromotion.Name;
                oEditPromotionInfo.OrgID = backPromotion.OrgID;
                oEditPromotionInfo.ChlID = backPromotion.ChlID;
                string currentCode = formCollection["Code"];
                oEditPromotionInfo.Code = currentCode;
                if (bCoverExist == false)
                {
                    List<PromotionInformation> listPromotion = dbEntity.PromotionInformations.Where(p => p.OrgID == promotionOrgGid && p.Code == currentCode).ToList();
                    if (listPromotion.Count > 0)
                    {
                        //如果查询出来的为存在的，则提示用户是否覆盖；否则将原始数据恢复，然后修改数值。
                        if (listPromotion.ElementAt(0).Deleted == false)
                        {
                            return "exist";
                        }
                        else 
                        {
                            oEditPromotionInfo = listPromotion.ElementAt(0);
                            oEditPromotionInfo.Name.SetResource(ModelEnum.ResourceType.STRING, backPromotion.Name);
                            oEditPromotionInfo.Deleted = false;
                        }
                    }
                }
                else
                {
                    oEditPromotionInfo = dbEntity.PromotionInformations.Where(p => p.OrgID == promotionOrgGid && p.Code == currentCode).FirstOrDefault();
                }
            }
            else
            {
                oEditPromotionInfo = dbEntity.PromotionInformations.Include("Name").Where(p => p.Gid == currentPromotionGid && p.Deleted == false).FirstOrDefault();
                if (oEditPromotionInfo != null)
                {
                    oEditPromotionInfo.Name.SetResource(ModelEnum.ResourceType.STRING, backPromotion.Name);
                }
                else 
                {
                    return "fail";
                }
            }
            oEditPromotionInfo.Matter = backPromotion.Matter;
            oEditPromotionInfo.Pstatus = backPromotion.Pstatus;
            oEditPromotionInfo.IssueType = backPromotion.IssueType;
            oEditPromotionInfo.Sorting = backPromotion.Sorting;
            oEditPromotionInfo.Ptype = backPromotion.Ptype;
            oEditPromotionInfo.ConditionA = backPromotion.ConditionA;
            oEditPromotionInfo.ConditionB = backPromotion.ConditionB;
            oEditPromotionInfo.ConditionC = backPromotion.ConditionC;
            oEditPromotionInfo.ConditionD = backPromotion.ConditionD;
            string test = formCollection["issueStartTime"];
            if (!formCollection["issueStartTime"].Equals(""))
            { 
                oEditPromotionInfo.IssueStart = DateTimeOffset.Parse(formCollection["issueStartTime"]);
            }
            if (!formCollection["issueEndTime"].Equals(""))
            {
                oEditPromotionInfo.IssueEnd = DateTimeOffset.Parse(formCollection["issueEndTime"]);
            }
            if (!formCollection["validateStartTime"].Equals(""))
            {
                oEditPromotionInfo.StartTime = DateTimeOffset.Parse(formCollection["validateStartTime"]);
            }
            if (!formCollection["validateEndTime"].Equals(""))
            {
                oEditPromotionInfo.EndTime = DateTimeOffset.Parse(formCollection["validateEndTime"]);
            }
            oEditPromotionInfo.EffectDays = backPromotion.EffectDays;
            oEditPromotionInfo.Remark = backPromotion.Remark;

            if (bEdit == false && bCoverExist == false)
            {
                dbEntity.PromotionInformations.Add(oEditPromotionInfo);
                dbEntity.SaveChanges();
                globlePromotionGid = oEditPromotionInfo.Gid;
            }
            else 
            {
                dbEntity.SaveChanges();
            }
            //设置不允许覆盖
            bCoverExist = false;
            return "success";
        }
        /// <summary>
        /// 确定要覆盖现有的促销方案
        /// </summary>
        public void SetPromotionCover()
        {
            bCoverExist = true;
        }

        #endregion

        #region 互斥促销信息

        public ActionResult PromotionMutexIndex()
        {
            if (globlePromotionGid.Equals(Guid.Empty))
            {
                return null;
            }
            return View();
        }
        /// <summary>
        /// 互斥促销的首页列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListPromotionMutexInfomation(SearchModel searchModel)
        {
            IQueryable<PromotionMutex> oPromotionMutexInfo = dbEntity.PromotionMutexes.Include("Promotion").Include("Mutex").Where(p => p.Deleted == false && p.PromID == globlePromotionGid || p.MutexID == globlePromotionGid).AsQueryable();
            GridColumnModelList<PromotionMutex> columns = new GridColumnModelList<PromotionMutex>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Promotion.Name.GetResource(CurrentSession.Culture)).SetName("PromotionName");
            columns.Add(p => p.Mutex.Name.GetResource(CurrentSession.Culture)).SetName("MutexName");
            columns.Add(p => p.RelationName).SetName("RelationName");
            columns.Add(p => p.Remark);

            GridData gridData = oPromotionMutexInfo.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 添加编辑互融互斥促销方案页面
        /// </summary>
        /// <returns></returns>
        public ActionResult PromotionMutexEdit() 
        {
            PromotionMutex oNewPromotionMutex;
            List<SelectListItem> oMutexList = new List<SelectListItem>();
            List<SelectListItem> oRelationList = new List<SelectListItem>();
            //页面判断当前编辑或者添加状态变量
            ViewBag.bMutexEdit = bMutexEdit;
            //添加状态下
            if (bMutexEdit == false)
            {
                oNewPromotionMutex = new PromotionMutex();
                oNewPromotionMutex.PromID = globlePromotionGid;
                PromotionInformation currentPromotion = dbEntity.PromotionInformations.Include("Name").Where(p => p.Gid == globlePromotionGid && p.Deleted == false).FirstOrDefault();
                //选出所有有效的促销方案
                List<PromotionInformation> oAllMutexList = dbEntity.PromotionInformations.Include("Name").Where(p => p.Gid != globlePromotionGid && p.Deleted == false && p.Pstatus == 1).ToList();
                for (int i = 0; i < oAllMutexList.Count; i++)
                {
                    oMutexList.Add(new SelectListItem { Text = oAllMutexList.ElementAt(i).Name.GetResource(CurrentSession.Culture), Value = oAllMutexList.ElementAt(i).Gid.ToString() });
                }

                if (currentPromotion != null)
                {
                    ViewBag.promotionName = currentPromotion.Name.GetResource(CurrentSession.Culture);
                }
                else 
                {
                    ViewBag.promotionName = "";
                }
            }
            else 
            {
                oNewPromotionMutex = dbEntity.PromotionMutexes.Include("Promotion").Include("Mutex").Where(p => p.Gid == globleMutexGid && p.Deleted == false).FirstOrDefault();
                ViewBag.promotionName = oNewPromotionMutex.Promotion.Name.GetResource(CurrentSession.Culture);
                ViewBag.mutexName = oNewPromotionMutex.Mutex.Name.GetResource(CurrentSession.Culture);
            }
            //关系列表取自枚举类型
            oRelationList = GetSelectList(oNewPromotionMutex.RelationList);
            ViewBag.oMutexList = oMutexList;
            ViewBag.oRelationList = oRelationList;

            return View(oNewPromotionMutex);
        }
        /// <summary>
        /// 保存互融互斥促销方案
        /// </summary>
        /// <param name="backMutex"></param>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public string SavePromotionMutexRelation(PromotionMutex backMutex, FormCollection formCollection)
        {
            PromotionMutex oNewPromotionMutex;
            //添加互融互斥促销方案
            if (bMutexEdit == false)
            {
                Guid currentPromGid = backMutex.PromID;
                Guid currentMutexGid = backMutex.MutexID;
                //如果不能覆盖，则查找重复的数据，提示用户
                if (bCoverMutexExist == false)
                {
                    oNewPromotionMutex = dbEntity.PromotionMutexes.Include("Promotion").Include("Mutex").Where(p => p.PromID == currentPromGid && p.MutexID == currentMutexGid || (p.MutexID == currentPromGid && p.PromID == currentMutexGid)).FirstOrDefault();
                    if (oNewPromotionMutex != null)
                    {
                        if (oNewPromotionMutex.Deleted == false)
                        {
                            return "exist";
                        }
                        else
                        {
                            oNewPromotionMutex.Deleted = false;
                            oNewPromotionMutex.Relation = backMutex.Relation;
                            oNewPromotionMutex.Remark = backMutex.Remark;
                        }
                    }
                    else 
                    {
                        oNewPromotionMutex = new PromotionMutex();
                        oNewPromotionMutex.PromID = backMutex.PromID;
                        oNewPromotionMutex.MutexID = backMutex.MutexID;
                        oNewPromotionMutex.Relation = backMutex.Relation;
                        oNewPromotionMutex.Remark = backMutex.Remark;
                        dbEntity.PromotionMutexes.Add(oNewPromotionMutex);
                    }
                }
                else     //可以覆盖的话直接覆盖
                {
                    oNewPromotionMutex = dbEntity.PromotionMutexes.Include("Promotion").Include("Mutex").Where(p => p.PromID == currentPromGid && p.MutexID == currentMutexGid || (p.MutexID == currentPromGid && p.PromID == currentMutexGid)).FirstOrDefault();
                    oNewPromotionMutex.Relation = backMutex.Relation;
                    oNewPromotionMutex.Remark = backMutex.Remark;
                }
            }
            else 
            {
                oNewPromotionMutex = dbEntity.PromotionMutexes.Where(p => p.Gid == globleMutexGid && p.Deleted == false).FirstOrDefault();
                if (oNewPromotionMutex != null)
                {
                    oNewPromotionMutex.Relation = backMutex.Relation;
                    oNewPromotionMutex.Remark = backMutex.Remark;
                }
                else 
                {
                    return "fail";
                }
            }

            dbEntity.SaveChanges();
            bCoverMutexExist = false;
            return "success";        
        }
        /// <summary>
        /// 设置添加或者编辑促销互斥的全局变量
        /// </summary>
        /// <param name="bMutexAddOrEdit"></param>
        public void SetMutexEdit(bool bMutexAddOrEdit)
        {
            bMutexEdit = bMutexAddOrEdit;
        }
        /// <summary>
        /// 设置互融互斥的促销方案的可覆盖变量
        /// </summary>
        public void SetMutexCover()
        {
            bCoverMutexExist = true;
        }
        /// <summary>
        /// 设置互斥方案全局变量
        /// </summary>
        /// <param name="gid"></param>
        public void SetMutexGid(Guid gid)
        {
            globleMutexGid = gid;
        }

        #endregion

        #region 促销商品

        public ActionResult PromotionProductIndex()
        {
            return View();
        }
        /// <summary>
        /// 列出对应的促销方案的商品信息
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListPromotionProductInfomation(SearchModel searchModel)
        {
            CultureInfo culture = new CultureInfo(CurrentSession.Culture);
            IQueryable<PromotionProduct> oPromotionProductInfo = dbEntity.PromotionProducts.Include("Promotion").Include("OnSkuItem").Include("Price").Where(p => p.Deleted == false).AsQueryable();
            GridColumnModelList<PromotionProduct> columns = new GridColumnModelList<PromotionProduct>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Promotion.Name.GetResource(CurrentSession.Culture)).SetName("PromotionName");
            columns.Add(p => p.OnSkuItem.SkuItem.Code + "(" + p.OnSkuItem.OnSale.Code + ")").SetName("PromotionProductCode");
            columns.Add(p => p.OnSkuItem.FullName.GetResource(CurrentSession.Culture)).SetName("PromotionProductName");
            columns.Add(p => p.Quantity);
            columns.Add(p => p.Price.Cash).SetName("ProductPrice");
            columns.Add(p => p.Remark);

            GridData gridData = oPromotionProductInfo.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 添加或编辑促销产品
        /// </summary>
        /// <param name="bProductEdit"></param>
        /// <param name="promotionProductGid"></param>
        /// <returns></returns>
        public ActionResult PromotionProductEdit(bool bProductEdit, Guid? promotionProductGid)
        {
            PromotionProduct oNewPromotionProduct;
            PromotionInformation oCurrentPromotion = dbEntity.PromotionInformations.Where(p => p.Gid == globlePromotionGid && p.Deleted == false).FirstOrDefault();
            //添加的时候
            if (bProductEdit == false)
            {
                oNewPromotionProduct = new PromotionProduct { Price = NewResource(ModelEnum.ResourceType.MONEY, oCurrentPromotion.OrgID) };
                oNewPromotionProduct.PromID = oCurrentPromotion.Gid;
                //数量初始值为-1
                oNewPromotionProduct.Quantity = -1;
            }
            else 
            {
                oNewPromotionProduct = dbEntity.PromotionProducts.Include("Promotion").Include("OnSkuItem").Include("Price").Where(p => p.Gid == (Guid)promotionProductGid && p.Deleted == false).FirstOrDefault();
                oNewPromotionProduct.Price = RefreshResource(ModelEnum.ResourceType.MONEY, oNewPromotionProduct.Price, oCurrentPromotion.OrgID);
            }
            ViewBag.bEdit = bProductEdit;
            ViewBag.oPromotionCode = oCurrentPromotion.Code;
            return View(oNewPromotionProduct);
        }
        /// <summary>
        /// 列出促销方案对应的组织所属的上架商品
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListPromotionProductSKUOnsale(SearchModel searchModel) 
        {     
            //查出促销规则对应的上架商品信息
            IQueryable<ProductOnItem> oOnProductInfo = dbEntity.ProductOnItems.Include("OnSale").Include("SkuItem").Include("FullName").Where(p => p.Deleted == false && p.OnSale.OrgID == promotionOrgGid).AsQueryable();
            GridColumnModelList<ProductOnItem> columns = new GridColumnModelList<ProductOnItem>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.OnSale.Code);
            columns.Add(p => p.SkuItem.Code).SetName("SKUCode");
            columns.Add(p => p.FullName.GetResource(CurrentSession.Culture)).SetName("SKUFullName");

            GridData gridData = oOnProductInfo.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PromotionOnSKUList() 
        {
            return View();
        }
        /// <summary>
        /// 将添加的商品信息设置到页面
        /// </summary>
        /// <param name="gid"></param>
        /// <returns></returns>
        public string AddProductToPromotion(Guid gid)
        {
            string backString = "";

            ProductOnItem oProductItem = dbEntity.ProductOnItems.Include("OnSale").Include("SkuItem").Include("FullName").Where(p => p.Deleted == false && p.Gid == gid).FirstOrDefault();
            if (oProductItem != null)
            {
                backString = oProductItem.Gid.ToString() + "|" + oProductItem.FullName.GetResource(CurrentSession.Culture) + "|" + oProductItem.SkuItem.Code + "|" + oProductItem.OnSale.Code;
            }

            return backString;
        }
        /// <summary>
        /// 保存促销相关的上架商品
        /// </summary>
        /// <param name="oBackPromotionProduct"></param>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public string SavePromotionProduct(PromotionProduct oBackPromotionProduct, FormCollection formCollection) 
        {
            PromotionProduct oEditPromotionProduct;
            //上架商品的Gid
            Guid onSKUGid = oBackPromotionProduct.OnSkuItem.Gid;
            if (oBackPromotionProduct.Gid.Equals(Guid.Empty))
            {
                //添加状态
                if (bCoverProductExist == false)
                {
                    oEditPromotionProduct = dbEntity.PromotionProducts.Include("Promotion").Include("OnSkuItem").Include("Price").Where(p => p.PromID == globlePromotionGid && p.OnSkuID == onSKUGid).FirstOrDefault();

                    //如果存在相同的促销以及上架商品Gid
                    if (oEditPromotionProduct != null)
                    {
                        //判断原来数据库中的信息是否已删除
                        if (oEditPromotionProduct.Deleted == false)
                        {
                            return "exist";
                        }
                        else 
                        {
                            //未删除则修改信息，保存
                            oEditPromotionProduct.Deleted = false;
                            oEditPromotionProduct.Quantity = oBackPromotionProduct.Quantity;
                            oEditPromotionProduct.Price.SetResource(ModelEnum.ResourceType.MONEY, oBackPromotionProduct.Price);
                            oEditPromotionProduct.Remark = oBackPromotionProduct.Remark;
                        }
                    }
                    else
                    {
                        //新建促销商品
                        oEditPromotionProduct = new PromotionProduct { Price = NewResource(ModelEnum.ResourceType.MONEY, promotionOrgGid) };
                        oEditPromotionProduct.PromID = globlePromotionGid;
                        oEditPromotionProduct.OnSkuID = onSKUGid;
                        oEditPromotionProduct.Quantity = oBackPromotionProduct.Quantity;
                        oEditPromotionProduct.Price = oBackPromotionProduct.Price;
                        dbEntity.PromotionProducts.Add(oEditPromotionProduct);
                    }
                }
                else 
                {
                    //直接覆盖当前的促销商品信息
                    oEditPromotionProduct = dbEntity.PromotionProducts.Include("Promotion").Include("OnSkuItem").Include("Price").Where(p => p.PromID == globlePromotionGid && p.OnSkuID == onSKUGid).FirstOrDefault();
                    oEditPromotionProduct.Deleted = false;
                    oEditPromotionProduct.Quantity = oBackPromotionProduct.Quantity;
                    oEditPromotionProduct.Price.SetResource(ModelEnum.ResourceType.MONEY, oBackPromotionProduct.Price);
                    oEditPromotionProduct.Remark = oBackPromotionProduct.Remark;
                    bCoverProductExist = false;
                }
            }
            else 
            {
                Guid currentPromotionProductGid = oBackPromotionProduct.Gid;
                //编辑状态
                oEditPromotionProduct = dbEntity.PromotionProducts.Include("Promotion").Include("OnSkuItem").Include("Price").Where(p => p.Gid == currentPromotionProductGid && p.Deleted == false).FirstOrDefault();
                if (oEditPromotionProduct != null)
                {
                    oEditPromotionProduct.Quantity = oBackPromotionProduct.Quantity;
                    oEditPromotionProduct.Price.SetResource(ModelEnum.ResourceType.MONEY, oBackPromotionProduct.Price);
                    oEditPromotionProduct.Remark = oBackPromotionProduct.Remark;
                }
                else
                {
                    return "fail";
                }
            }

            dbEntity.SaveChanges();

            return "success";
        }
        /// <summary>
        /// 修改商品覆盖的全局变量
        /// </summary>
        public void SetbCoverProductExist()
        {
            bCoverProductExist = true;
        }

        #endregion

        #region 促销券信息
        /// <summary>
        /// 促销信息首页
        /// </summary>
        /// <returns></returns>
        public ActionResult PromotionCouponIndex() 
        {
            return View();
        }
        /// <summary>
        /// 促销券的信息列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListPromotionCouponInfomation(SearchModel searchModel)
        {
            CultureInfo culture = new CultureInfo(CurrentSession.Culture);
            IQueryable<PromotionCoupon> oPromotionCoupon = dbEntity.PromotionCoupons.Include("Promotion").Include("Currency").Where(p => p.PromID == globlePromotionGid && p.Deleted == false).AsQueryable();
            GridColumnModelList<PromotionCoupon> columns = new GridColumnModelList<PromotionCoupon>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Promotion.Name.GetResource(CurrentSession.Culture)).SetName("PromotionName");
            columns.Add(p => p.Code);
            columns.Add(p => p.Passcode);
            columns.Add(p => p.CouponStatusName).SetName("Cstatus");
            columns.Add(p => p.Currency.Code).SetName("Currency");
            columns.Add(p => p.Amount);
            columns.Add(p => p.MinCharge);
            columns.Add(p => p.Cashier);
            columns.Add(p => p.OnceUse);
            columns.Add(p => p.StartTime == null ? "" : p.StartTime.Value.ToString(culture.DateTimeFormat.ShortDatePattern)).SetName("StartTime");
            columns.Add(p => p.EndTime == null ? "" : p.EndTime.Value.ToString(culture.DateTimeFormat.ShortDatePattern)).SetName("EndTime");
            columns.Add(p => p.Remark);

            GridData gridData = oPromotionCoupon.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 批量添加促销券的页面
        /// </summary>
        /// <returns></returns>
        public ActionResult PromotionCouponAdd()
        {
            PromotionCoupon oNewPromotionCoupon = new PromotionCoupon();
            List<SelectListItem> oCstatusList = new List<SelectListItem>();
            oCstatusList = GetSelectList(oNewPromotionCoupon.CouponStatusList);
            ViewBag.oCstatusList = oCstatusList;
            List<SelectListItem> oCurrencyList = new List<SelectListItem>();
            List<MemberOrgCulture> listOrgCurrency = dbEntity.MemberOrgCultures.Include("Currency").Where(p => p.OrgID == promotionOrgGid && p.Deleted == false && p.Ctype == 1).ToList();
            for (int i = 0; i < listOrgCurrency.Count; i++)
            {
                SelectListItem item = new SelectListItem { Text = listOrgCurrency.ElementAt(i).Currency.Name.GetResource(CurrentSession.Culture), Value = listOrgCurrency.ElementAt(i).Currency.Gid.ToString() };
                oCurrencyList.Add(item);
            }
            ViewBag.oCurrencyList = oCurrencyList;
            List<SelectListItem> oCashierList = new List<SelectListItem>();
            oCashierList.Add(new SelectListItem { Text = LiveAzure.Resource.Common.Yes, Value = true.ToString() });
            oCashierList.Add(new SelectListItem { Text = LiveAzure.Resource.Common.No, Value = false.ToString() });
            ViewBag.oCashierList = oCashierList;
            List<SelectListItem> oOnceUseList = new List<SelectListItem>();
            oOnceUseList.Add(new SelectListItem { Text = LiveAzure.Resource.Common.Yes, Value = true.ToString() });
            oOnceUseList.Add(new SelectListItem { Text = LiveAzure.Resource.Common.No, Value = false.ToString() });
            ViewBag.oOnceUseList = oOnceUseList;
            return View(oNewPromotionCoupon);
        }
        /// <summary>
        /// 生成指定长度的随机数字字符串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string RndStr(int length)
        {
            Random rnd = new Random();
            string newCode = "";
            int i = 0;
            while (i < length)
            {
                newCode += rnd.Next(0, 9).ToString();
                i++;
            }
            return newCode;
        }
        /// <summary>
        /// 检查随机生成卡号的重复性，返回不重复的卡号
        /// </summary>
        /// <param name="currentCode"></param>
        /// <returns></returns>
        public string GetNewCouponCode(string currentCode)
        {
            string newCouponCode = "";
            List<PromotionCoupon> listCoupon = dbEntity.PromotionCoupons.Where(p => p.PromID == globlePromotionGid && p.Code == currentCode).ToList();
            if (listCoupon.Count > 0)
            {
                currentCode = RndStr(nCodeLenth);
                newCouponCode = GetNewCouponCode(currentCode);
            }
            else 
            {
                newCouponCode = currentCode;
            }

            return newCouponCode;
        }

        public string SavePromotionCouponInfo(PromotionCoupon oBackCoupon, FormCollection formCollection)
        {
            int nCouponCount = Int32.Parse(formCollection["CouponCount"]);
            //根据需要的卡的数量来生成相应的卡号
            for (int i = 0; i < nCouponCount; i++)
            {
                string couponCode = RndStr(nCodeLenth);
                //递归取得不重复的卡号
                couponCode = GetNewCouponCode(couponCode);
                PromotionCoupon oNewPromotionCoupon = new PromotionCoupon();
                oNewPromotionCoupon.PromID = globlePromotionGid;
                oNewPromotionCoupon.Code = couponCode;
                oNewPromotionCoupon.Passcode = RndStr(nPasswordLenth);
                oNewPromotionCoupon.Cstatus = oBackCoupon.Cstatus;
                oNewPromotionCoupon.aCurrency = oBackCoupon.aCurrency;
                oNewPromotionCoupon.Amount = oBackCoupon.Amount;
                oNewPromotionCoupon.MinCharge = oBackCoupon.MinCharge;
                oNewPromotionCoupon.Cashier = oBackCoupon.Cashier;
                oNewPromotionCoupon.OnceUse = oBackCoupon.OnceUse;
                if (!formCollection["StartTime"].Equals(""))
                {
                    oNewPromotionCoupon.StartTime = DateTimeOffset.Parse(formCollection["StartTime"]);
                }
                if (!formCollection["EndTime"].Equals(""))
                {
                    oNewPromotionCoupon.EndTime = DateTimeOffset.Parse(formCollection["EndTime"]);
                }
                oNewPromotionCoupon.Remark = oBackCoupon.Remark;
                dbEntity.PromotionCoupons.Add(oNewPromotionCoupon);
                dbEntity.SaveChanges();
            }
            
            return "success";
        }
        /// <summary>
        /// 作废券信息
        /// </summary>
        /// <param name="couponGid"></param>
        /// <returns></returns>
        public void InvalidCouponInfo(string couponGid)
        {
            string[] couponGidList = couponGid.Split(',');
            for (int i = 0; i < couponGidList.Count(); i++)
            {
                Guid promotionCouponGid = Guid.Parse(couponGidList[i]);
                PromotionCoupon oPromotionCoupon = dbEntity.PromotionCoupons.Where(p => p.Gid == promotionCouponGid && p.Deleted == false).FirstOrDefault();
                if (oPromotionCoupon != null)
                {
                    oPromotionCoupon.Deleted = true;
                    oPromotionCoupon.Cstatus = 0;
                    dbEntity.SaveChanges();
                }
            }
        }

        #endregion

    }
}
