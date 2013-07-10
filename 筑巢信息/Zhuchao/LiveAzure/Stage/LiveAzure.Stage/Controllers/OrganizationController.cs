using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveAzure.Models.General;
using LiveAzure.Utility;
using LiveAzure.Models.Member;
using LiveAzure.Models.Warehouse;
using LiveAzure.Models.Purchase;
using LiveAzure.Models.Shipping;
using System.Web.Helpers;
using LiveAzure.Resource.Stage;
using System.Collections;
using LiveAzure.Models;
using MVC.Controls;
using MVC.Controls.Grid;
using System.Globalization;
using System.Data;
using System.Text;

namespace LiveAzure.Stage.Controllers
{
    public class OrganizationController : BaseController
    {
        #region 初始化

        //全局变量
        private static MemberOrganization _oNewMemberOrganization;//全局变量 待添加的组织
        private static List<MemberOrgCulture> _listOrganizationCulture;//全局变量 待添加的组织支持的文化列表，包括组织的语言和组织的货币
        private static bool _isAdding = false;//全局变量 是否是添加状态 用于区别是编辑还是添加
        public static int retype;//？？？？
        public static Guid gOrgId;//全局变量 组织ID？

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

        #region 组织管理

        /// <summary>
        /// 组织管理首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // 权限验证
            if (!base.CheckPrivilege())
                RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            return View();
        }
        /// <summary>
        /// 组织管理首页中组织GRID
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult List(SearchModel searchModel)
        {
            IQueryable<MemberOrganization> organs = dbEntity.MemberOrganizations.Include("FullName").Include("ShortName").Where(p => p.Deleted == false && p.Otype == (byte)ModelEnum.OrganizationType.CORPORATION).AsQueryable();
            GridColumnModelList<MemberOrganization> columns = new GridColumnModelList<MemberOrganization>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Code);
            columns.Add(p => (p.FullName == null) ? " " : p.FullName.GetResource(CurrentSession.Culture)).SetName("FullName");
            columns.Add(p => (p.ShortName == null) ? " " : p.ShortName.GetResource(CurrentSession.Culture)).SetName("ShortName");
            columns.Add(p => p.OrganStatusName);
            columns.Add(p => p.Sorting);
            GridData gridData = organs.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 添加组织时，先初始化缓存对象
        /// </summary>
        public string AddNewOrganization()
        {
            if (!base.CheckPrivilege("EnableEdit"))//权限验证
                return "NoPrivilege";
            _oNewMemberOrganization = new MemberOrganization();
            _listOrganizationCulture = new List<MemberOrgCulture>();
            _isAdding = true;
            return "Success";
        }
        /// <summary>
        /// 核对组织代码
        /// </summary>
        /// <param name="organizationCode"></param>
        /// <returns></returns>
        public ActionResult CheckOrgnizationCode(string organizationCode)
        {
            MemberOrganization oOrganization = dbEntity.MemberOrganizations.Where(o => o.Code == organizationCode).SingleOrDefault();
            if (oOrganization != null || organizationCode == "")
            {
                ViewBag.checkResult = false;
            }
            else
            {
                _oNewMemberOrganization.Code = organizationCode;
                _oNewMemberOrganization.Otype = (byte)ModelEnum.OrganizationType.CORPORATION;
                ViewBag.checkResult = true;
                ViewBag.orgCode = organizationCode;
            }
            return PartialView();
        }
        /// <summary>
        /// 检测是否可添加组织
        /// </summary>
        /// <returns></returns>
        public bool CheckSaveCode(string organizationCode)
        {
            MemberOrganization oOrganization = dbEntity.MemberOrganizations.Where(o => o.Code == organizationCode).SingleOrDefault();
            if (oOrganization != null || organizationCode == "")
                return false;
            else
                return true;
        }
        /// <summary>
        /// 组织添加编辑时的语言文化列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult OrgCultureGridList(SearchModel searchModel)
        {
            List<MemberOrgCulture> oOrgCultures = new List<MemberOrgCulture>();
            if (_isAdding)
            {
                oOrgCultures = _listOrganizationCulture.Where(c => c.Ctype == (byte)ModelEnum.CultureType.LANGUAGE).ToList();
            }
            else
            {
                oOrgCultures = dbEntity.MemberOrgCultures.Include("Culture").Where(c => c.Deleted == false && c.OrgID == gOrgId && c.Ctype == (byte)ModelEnum.CultureType.LANGUAGE).ToList();
            }
            IQueryable<MemberOrgCulture> oListOrgCultures = oOrgCultures.AsQueryable();
            GridColumnModelList<MemberOrgCulture> columns = new GridColumnModelList<MemberOrgCulture>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.aCulture);
            columns.Add(p => dbEntity.GeneralCultureUnits.Find(p.aCulture).CultureName).SetName("Culture");
            columns.Add(p => p.Sorting);
            columns.Add(p => p.Remark);
            GridData gridData = oListOrgCultures.ToGridData(searchModel, columns);

            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 组织添加编辑时的货币列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult OrgUnitGridList(SearchModel searchModel)
        {
            List<MemberOrgCulture> oOrgUnits = new List<MemberOrgCulture>();
            if (_isAdding)
            {
                oOrgUnits = _listOrganizationCulture.Where(c => c.Ctype == (byte)ModelEnum.CultureType.CURRENCY).ToList();
            }
            else
            {
                oOrgUnits = dbEntity.MemberOrgCultures.Include("Currency").Where(c => c.Deleted == false && c.OrgID == gOrgId && c.Ctype == (byte)ModelEnum.CultureType.CURRENCY).ToList();
            }
            IQueryable<MemberOrgCulture> oListOrgUnits = oOrgUnits.AsQueryable();
            GridColumnModelList<MemberOrgCulture> columns = new GridColumnModelList<MemberOrgCulture>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.aCurrency);
            columns.Add(p => dbEntity.GeneralMeasureUnits.Find(p.aCurrency).Name.GetResource(CurrentSession.Culture)).SetName("Currency");
            columns.Add(p => p.Sorting);
            columns.Add(p => p.Remark);
            GridData gridData = oListOrgUnits.ToGridData(searchModel, columns);

            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 返回当前是否添加了组织语言
        /// </summary>
        /// <returns></returns>
        public bool HasAddCulture()
        {
            if (_listOrganizationCulture.Where(item => item.aCulture != null).ToList().Count > 0)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 删除组织
        /// </summary>
        /// <param name="oOrganizationGid"></param>
        public string RemoveOrganization(Guid oOrganizationGid)
        {
            if (!base.CheckPrivilege("EnableDelete"))//权限验证
                return "NoPrivilege";
            MemberOrganization oOrganization = dbEntity.MemberOrganizations.Where(o => o.Gid == oOrganizationGid).Single();
            oOrganization.Deleted = true;
            if (ModelState.IsValid)
            {
                dbEntity.Entry(oOrganization).State = EntityState.Modified;
                dbEntity.SaveChanges();
            }
            return "Success";
        }
        /// <summary>
        /// 编辑组织文化
        /// </summary>
        /// <param name="oCultureGid"></param>
        /// <returns></returns>
        public ActionResult EditOrgCulturePage(Guid? oCultureGid, Guid? aCulture, Guid? aCurrency)
        {
            MemberOrgCulture oCulture = new MemberOrgCulture();
            if (_isAdding)//添加时 保存到缓存区
            {
                if (aCulture != null)
                {
                    oCulture = _listOrganizationCulture.Where(c => c.aCulture == aCulture).FirstOrDefault();
                    ViewBag.editCultureName = dbEntity.GeneralCultureUnits.Find(aCulture).CultureName;
                }
                else
                {
                    oCulture = _listOrganizationCulture.Where(c => c.aCurrency == aCurrency).FirstOrDefault();
                    ViewBag.editCultureName = dbEntity.GeneralMeasureUnits.Find(aCurrency).Name.GetResource(CurrentSession.Culture);
                }
            }
            else
            {
                oCulture = dbEntity.MemberOrgCultures.Include("Culture").Include("Currency").Where(c => c.Gid == oCultureGid).Single();
                ViewBag.editCultureType = oCulture.Ctype;
                if (oCulture.Ctype == (byte)ModelEnum.CultureType.LANGUAGE)
                    ViewBag.editCultureName = oCulture.Culture.CultureName;
                else
                    ViewBag.editCultureName = oCulture.Currency.Name.GetResource(CurrentSession.Culture);
            }
            return PartialView(oCulture);
        }
        /// <summary>
        /// 保存文化/货币到缓存区
        /// </summary>
        /// <param name="oOrgCulture"></param>
        public void SaveNewOrgCulture(MemberOrgCulture oOrgCulture)
        {
            if (_isAdding)//添加时 保存到缓存区
            {
                MemberOrgCulture oOldOrgCulture = null;
                if (oOrgCulture.aCulture != null && oOrgCulture.aCurrency == null)//添加的是语言
                    oOldOrgCulture = _listOrganizationCulture.Where(c => c.aCulture == oOrgCulture.aCulture && c.Ctype == oOrgCulture.Ctype).FirstOrDefault();
                else if (oOrgCulture.aCulture == null && oOrgCulture.aCurrency != null)//添加的是货币
                    oOldOrgCulture = _listOrganizationCulture.Where(c => c.aCurrency == oOrgCulture.aCurrency && c.Ctype == oOrgCulture.Ctype).FirstOrDefault();
                if (oOldOrgCulture != null)
                {
                    oOldOrgCulture.Sorting = oOrgCulture.Sorting;
                    oOldOrgCulture.Remark = oOrgCulture.Remark;
                }
                else
                {
                    _listOrganizationCulture.Add(oOrgCulture);
                }
            }
            else//编辑时 直接保存到数据库
            {
                MemberOrganization oOrganization = dbEntity.MemberOrganizations.Where(o => o.Gid == gOrgId).Single();
                oOrgCulture.Organization = oOrganization;
                dbEntity.MemberOrgCultures.Add(oOrgCulture);
                dbEntity.SaveChanges();
            }
        }
        /// <summary>
        /// 添加组织到数据库，即保存全局变量_oNewMemberOrganization
        /// </summary>
        public bool saveNewOrganization(string organizationCode)
        {
            _oNewMemberOrganization.Code = organizationCode;
            _oNewMemberOrganization.Otype = (byte)ModelEnum.OrganizationType.CORPORATION;
            dbEntity.MemberOrganizations.Add(_oNewMemberOrganization);
            dbEntity.SaveChanges();
            foreach (MemberOrgCulture orgculture in _listOrganizationCulture)
            {
                orgculture.OrgID = _oNewMemberOrganization.Gid;
                dbEntity.MemberOrgCultures.Add(orgculture);
                dbEntity.SaveChanges();
                _oNewMemberOrganization.FullName = NewResource(ModelEnum.ResourceType.STRING, _oNewMemberOrganization.Gid);
                _oNewMemberOrganization.ShortName = NewResource(ModelEnum.ResourceType.STRING, _oNewMemberOrganization.Gid);
                dbEntity.SaveChanges();
            }
            gOrgId = _oNewMemberOrganization.Gid;
            _isAdding = false;
            return true;
        }
        /// <summary>
        /// 获取文化列表
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetOrgCultureList()
        {
            List<SelectListItem> oList = new List<SelectListItem>();
            var oCultures = dbEntity.GeneralCultureUnits.Where(o => o.Deleted == false).ToList();
            var oOrgCultures = (from m in dbEntity.MemberOrgCultures
                                where m.Deleted == false && m.OrgID == gOrgId && m.Ctype == (byte)ModelEnum.CultureType.LANGUAGE
                                select m.Culture).ToList();
            var Lists = oCultures.Except(oOrgCultures);
            foreach (GeneralCultureUnit item in Lists)
            {
                oList.Add(new SelectListItem { Text = item.CultureName, Value = item.Gid.ToString() });
            }
            return oList;
        }
        /// <summary>
        /// 获取货币列表
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetOrgUnitList()
        {
            List<SelectListItem> oList = new List<SelectListItem>();
            var oUnits = (from o in dbEntity.GeneralMeasureUnits
                          where o.Deleted == false && o.Utype == (byte)ModelEnum.MeasureUnit.CURRENCY
                          select o).ToList();
            var oOrgUnits = (from m in dbEntity.MemberOrgCultures
                             where m.Deleted == false && m.OrgID == gOrgId && m.Ctype == (byte)ModelEnum.CultureType.CURRENCY
                             select m.Currency).ToList();
            var Lists = oUnits.Except(oOrgUnits);

            foreach (var item in Lists)
            {
                oList.Add(new SelectListItem { Text = item.Name.GetResource(CurrentSession.Culture), Value = item.Gid.ToString() });
            }
            return oList;
        }
        /// <summary>
        /// 保存所编辑的文化/语言
        /// </summary>
        /// <param name="oOrgCulture"></param>
        public void SaveEditOrgCulture(MemberOrgCulture oOrgCulture)
        {
            MemberOrgCulture oldCulture = new MemberOrgCulture();
            if (_isAdding)
            {
                if (oOrgCulture.aCulture != null)
                {
                    oldCulture = _listOrganizationCulture.Where(c => c.aCulture == oOrgCulture.aCulture).FirstOrDefault();
                }
                else
                {
                    oldCulture = _listOrganizationCulture.Where(c => c.aCurrency == oOrgCulture.aCurrency).FirstOrDefault();
                }
                if (oldCulture != null)
                {
                    oldCulture.Sorting = oOrgCulture.Sorting;
                    oldCulture.Remark = oOrgCulture.Remark;
                }
            }
            else
            {
                oldCulture = dbEntity.MemberOrgCultures.Include("Culture").Include("Currency").Where(c => c.Gid == oOrgCulture.Gid).Single();
                if (oldCulture.Ctype == (byte)ModelEnum.CultureType.CURRENCY)
                    oldCulture.Culture = oOrgCulture.Culture;
                else
                    oldCulture.Currency = oOrgCulture.Currency;
                oldCulture.Sorting = oOrgCulture.Sorting;
                oldCulture.Remark = oOrgCulture.Remark;
                if (ModelState.IsValid)
                {
                    dbEntity.Entry(oldCulture).State = EntityState.Modified;
                    dbEntity.SaveChanges();
                }
            }

        }
        /// <summary>
        /// 删除文化 语言/货币
        /// </summary>
        /// <param name="oCultureGid"></param>
        public void RemoveOrgCulture(Guid? oCultureGid, Guid? aCulture, Guid? aCurrency)
        {
            MemberOrgCulture oOrgCulture = new MemberOrgCulture();
            if (_isAdding)//添加时
            {
                if (aCulture != null)
                {
                    oOrgCulture = _listOrganizationCulture.Where(c => c.aCulture == aCulture).FirstOrDefault();
                    _listOrganizationCulture.Remove(oOrgCulture);
                }
                else
                {
                    oOrgCulture = _listOrganizationCulture.Where(c => c.aCurrency == aCurrency).FirstOrDefault();
                    _listOrganizationCulture.Remove(oOrgCulture);
                }
            }
            else//编辑时
            {
                oOrgCulture = dbEntity.MemberOrgCultures.Where(c => c.Gid == oCultureGid).Single();
                oOrgCulture.Deleted = true;
                if (ModelState.IsValid)
                {
                    dbEntity.Entry(oOrgCulture).State = EntityState.Modified;
                    dbEntity.SaveChanges();
                }
            }

        }
        /// <summary>
        /// 组织文化页面
        /// </summary>
        /// <returns></returns>
        public ActionResult OrganizationCultureAdd()
        {
            if (gOrgId == null || gOrgId == Guid.Empty)
                ViewBag.notHasCode = true;
            else
            {
                ViewBag.notHasCode = false;
                ViewBag.modeOrgCode = dbEntity.MemberOrganizations.Find(gOrgId).Code;
            }
            return View();
        }
        /// <summary>
        /// 组织的文化/货币 添加页面
        /// </summary>
        /// <param name="ctype"></param>
        /// <returns></returns>
        public ActionResult AddOrgCulturePage(byte ctype)
        {
            if (ctype == (byte)ModelEnum.CultureType.CURRENCY)
            {
                ViewBag.selectList = GetOrgUnitList();
                ViewBag.culturetype = 1;
            }
            else
            {
                ViewBag.selectList = GetOrgCultureList();
                ViewBag.culturetype = 0;
            }
            MemberOrgCulture oOrgCulture = new MemberOrgCulture
            {
                Ctype = ctype
            };
            return PartialView(oOrgCulture);
        }
        #endregion

        #region 渠道

        /// <summary>
        /// 渠道首页
        /// </summary>
        /// <returns></returns>
        public ActionResult ChannelIndex()
        {
            // 权限验证
            if (!base.CheckPrivilege())
                RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            return View();
        }
        /// <summary>
        /// 渠道列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="orgSelect">组织选择</param>
        /// <returns></returns>
        public ActionResult ListChannel(SearchModel searchModel)
        {
            IQueryable<MemberChannel> listChannels = (from m in dbEntity.MemberChannels.Include("FullName")
                                                      where m.Deleted == false
                                                      select m).ToList().AsQueryable();
            GridColumnModelList<MemberChannel> columnsChannel = new GridColumnModelList<MemberChannel>();
            columnsChannel.Add(p => p.Gid).SetAsPrimaryKey();
            columnsChannel.Add(p => p.Code);
            columnsChannel.Add(p => p.FullName.Matter);
            columnsChannel.Add(p => p.Ostatus);
            columnsChannel.Add(p => p.Sorting);
            GridData gridDataCha = listChannels.ToGridData(searchModel, columnsChannel);
            return Json(gridDataCha, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 删除渠道
        /// </summary>
        /// <param name="strdeleid"></param>
        /// <returns></returns>
        [HttpPost]
        public string deleteChannel(Guid strdeleid)
        {
            // 权限验证
            if (!base.CheckPrivilege("EnableDelete"))
                return "NoPrivilege";
            var delecha = (from o in dbEntity.MemberChannels
                           where (o.Gid == strdeleid && o.Deleted == false)
                           select o).Single();
            delecha.Deleted = true; 
            dbEntity.SaveChanges();
            return "success!";
        }

        #region 组织管理TAB页中的渠道
        /// <summary>
        /// 组织管理TAB页中的渠道管理页
        /// </summary>
        /// <returns></returns>
        public ViewResult OrgChannel()
        {
            MemberOrganization oMemberOrganization = dbEntity.MemberOrganizations.Where(o=>o.Gid == gOrgId).FirstOrDefault();
            if (oMemberOrganization != null)
                ViewBag.OrganizationName = oMemberOrganization.FullName.GetResource(CurrentSession.Culture);
            else
                ViewBag.OrganizationName = null;
            ViewBag.privChannelSecretKey = base.GetProgramNode("ChannelSecretKey");
            return View();
        }
        /// <summary>
        /// 组织管理TAB页中的渠道管理添加页
        /// </summary>
        /// <returns></returns>
        public ActionResult AddOrgChannelPage()
        {
            MemberOrgChannel oViewModel = new MemberOrgChannel();
            ViewBag.OrgChannelStatusSelectList = GetChannelStatusList();//状态列表下拉框
            ViewBag.OrgChannelSelectList = GetOrgChannelList();//获取有权限的渠道列表
            return View(oViewModel);
        }
        /// <summary>
        /// 组织管理TAB页中的渠道管理编辑页
        /// </summary>
        /// <returns></returns>
        public ActionResult EditOrgChannelPage(Guid oChannelGid)
        {
            MemberOrgChannel oOrgChannel = dbEntity.MemberOrgChannels.Include("Channel").Where(o => o.Gid == oChannelGid).Single();
            ViewBag.OrgChannelSelectListForEdit = GetOrgChannelList();
            ViewBag.OrgChannelStatusSelectListForEdit = GetChannelStatusList();
            ViewBag.ChannelFullName = oOrgChannel.Channel.FullName.GetResource(CurrentSession.Culture);
            return View(oOrgChannel);
        }
        /// <summary>
        /// 组织管理TAB页中的渠道保存添加新渠道
        /// </summary>
        /// <param name="oOrgChannel"></param>
        public void SaveNewOrgChannel(MemberOrgChannel oBackModel)
        {
            MemberOrgChannel oOldMemberOrgChannel = dbEntity.MemberOrgChannels.Where(o=>o.ChlID == oBackModel.ChlID && o.OrgID == gOrgId).FirstOrDefault();
            if(oOldMemberOrgChannel != null)//索引重复，则编辑
            {
                oOldMemberOrgChannel.Deleted = false;
                oOldMemberOrgChannel.Cstatus = oBackModel.Cstatus;
                oOldMemberOrgChannel.RemoteUrl = oBackModel.RemoteUrl;
                oOldMemberOrgChannel.ConfigKey = oBackModel.ConfigKey;
                oOldMemberOrgChannel.SecretKey = oBackModel.SecretKey;
                oOldMemberOrgChannel.SessionKey = oBackModel.SessionKey;
                oOldMemberOrgChannel.Remark = oBackModel.Remark;
            }
            else//不重复 则添加新的
            {
                oBackModel.OrgID = gOrgId;
                dbEntity.MemberOrgChannels.Add(oBackModel);
            }
            dbEntity.SaveChanges();
        }
        /// <summary>
        /// 组织管理TAB页中的渠道保存编辑后渠道
        /// </summary>
        /// <param name="oOrgChannel"></param>
        public void SaveEditOrgChannel(MemberOrgChannel oBackModel)
        {
            MemberOrgChannel oldOrgChannel = dbEntity.MemberOrgChannels.Include("Channel").Where(o => o.Gid == oBackModel.Gid).Single();
            oldOrgChannel.Cstatus = oBackModel.Cstatus;
            oldOrgChannel.RemoteUrl = oBackModel.RemoteUrl;
            oldOrgChannel.ConfigKey = oBackModel.ConfigKey;
            oldOrgChannel.SecretKey = oBackModel.SecretKey;
            oldOrgChannel.SessionKey = oBackModel.SessionKey;
            oldOrgChannel.Remark = oBackModel.Remark;
            if (ModelState.IsValid)
            {
                dbEntity.Entry(oldOrgChannel).State = EntityState.Modified;
                dbEntity.SaveChanges();
            }
        }
        /// <summary>
        /// 组织管理TAB页中的渠道，删除渠道
        /// </summary>
        /// <param name="oChannelGid"></param>
        public void RemoveOrgChannel(Guid oChannelGid)
        {
            MemberOrgChannel oldOrgChannel = dbEntity.MemberOrgChannels.Include("Channel").Where(o => o.Gid == oChannelGid).Single();
            oldOrgChannel.Deleted = true;

            if (ModelState.IsValid)
            {
                dbEntity.Entry(oldOrgChannel).State = EntityState.Modified;
                dbEntity.SaveChanges();
            }
        }
        /// <summary>
        /// 组织管理TAB页中的渠道列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult OrgChannelList(SearchModel searchModel)
        {
            IQueryable<MemberOrgChannel> oOrgChannels = dbEntity.MemberOrgChannels.Include("Channel").Where(c => c.Deleted == false && c.OrgID == gOrgId).AsQueryable();
            GridColumnModelList<MemberOrgChannel> columns = new GridColumnModelList<MemberOrgChannel>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => (p.Channel == null) ? "" : p.Channel.FullName.GetResource(CurrentSession.Culture)).SetName("Channel");
            columns.Add(p => p.ChannelStatusName).SetName("Cstatus");
            string privChannelSecretKey = "0";
            privChannelSecretKey = base.GetProgramNode("ChannelSecretKey");
            if (privChannelSecretKey == "1")
            {
                columns.Add(p => p.RemoteUrl).SetName("RemoteUrl");
                columns.Add(p => p.ConfigKey).SetName("ConfigKey");
                columns.Add(p => p.SecretKey).SetName("SecretKey");
                columns.Add(p => p.SessionKey).SetName("SessionKey");
            }
            columns.Add(p => p.Remark).SetName("Remark");
            GridData gridData = oOrgChannels.ToGridData(searchModel, columns);

            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取渠道列表
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetOrgChannelList()
        {
            List<SelectListItem> oList = new List<SelectListItem>();
            //用户有权限的渠道列表
            List<MemberChannel> oChannels = new List<MemberChannel>();
            if (CurrentSession.IsAdmin)
            {
                oChannels = dbEntity.MemberChannels.Where(c => c.Deleted == false).ToList();
            }
            else
            {
                oChannels = (from p in Permission(ModelEnum.UserPrivType.CHANNEL)
                             join chl in dbEntity.MemberChannels on p equals chl.Gid
                             where chl.Deleted == false
                             select chl).ToList();
            }
            foreach (MemberChannel item in oChannels)
            {
                oList.Add(new SelectListItem { Text = item.FullName.GetResource(CurrentSession.Culture), Value = item.Gid.ToString() });
            }
            return oList;
        }
        /// <summary>
        /// 获取当前状态
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetChannelStatusList()
        {
            List<SelectListItem> oList = base.SelectEnumList(typeof(ModelEnum.OrganizationStatus), new MemberOrgChannel().Cstatus);
            return oList;
        } 
        #endregion

        #endregion

        #region 供应商

        /// <summary>
        /// 供应商首页
        /// </summary>
        /// <returns></returns>
        public ActionResult SupplierIndex(Guid? ParentOrgID = null)
        {
            // 权限验证
            if (!base.CheckPrivilege())
                RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            retype = 1;
            Guid? userID = CurrentSession.UserID;
            if (userID != null)
            {
                //获取用户所属组织
                var sessionOrgId = dbEntity.MemberUsers.Where(p => p.Deleted == false && p.Gid == userID).Select(p => p.OrgID).Single();
                gOrgId = sessionOrgId;
            }
            //新建页面model
            PurchaseSupplier oViewModel = new PurchaseSupplier();
            //组织下拉框
            ViewBag.organization = GetSupportOrganizations();
            if (ParentOrgID != null)//有选中的组织
            {
                oViewModel.aParent = ParentOrgID.Value;
                gOrgId = ParentOrgID.Value;//设置全局gOrgId
                SetCurrentPath();//当前程序路径
            }
            return View(oViewModel);
        }
        /// <summary>
        /// 组织首页中的供应商列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parentid"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListSupplier(Guid? id, Guid? parentid, SearchModel searchModel)
        {
            if (gOrgId != null)
            {
                string sid = gOrgId.ToString();
                parentid = new Guid(sid);
            }
            IQueryable<PurchaseSupplier> suppliers = dbEntity.PurchaseSuppliers.Include("FullName").Include("ShortName").Where(p => p.Deleted == false && p.aParent == parentid).AsQueryable();
            GridColumnModelList<PurchaseSupplier> columnsSupp = new GridColumnModelList<PurchaseSupplier>();
            columnsSupp.Add(p => p.Gid).SetAsPrimaryKey();
            columnsSupp.Add(p => p.Code);
            columnsSupp.Add(p => p.FullName.Matter);
            columnsSupp.Add(p => p.ShortName.Matter);
            columnsSupp.Add(p => p.Ostatus);
            columnsSupp.Add(p => p.Sorting);
            GridData gridDataSupp = suppliers.ToGridData(searchModel, columnsSupp);
            return Json(gridDataSupp, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 删除供应商
        /// </summary>
        /// <param name="strdeleid"></param>
        /// <param name="parentid"></param>
        /// <returns></returns>
        [HttpPost]
        public string deleteSupp(Guid strdeleid, Guid? parentid)
        {

            if (gOrgId != null)
            {
                string sid = gOrgId.ToString();
                parentid = new Guid(sid);
            }
            var delesupp = (from o in dbEntity.PurchaseSuppliers
                            where (o.Gid == strdeleid && o.aParent == parentid && o.Deleted == false)
                            select o).Single();
            delesupp.Deleted = true;
            dbEntity.SaveChanges();
            return "success!";
        }
        #endregion

        #region 仓库
        /// <summary>
        /// 仓库首页
        /// </summary>
        /// <returns></returns>
        public ActionResult WarehouseIndex(Guid? ParentOrgID=null)
        {
            // 权限验证
            if (!base.CheckPrivilege())
                RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            retype = 1;
            Guid? userID = CurrentSession.UserID;
            if (userID != null)
            {
                //获取用户所属组织
                var sessionOrgId = dbEntity.MemberUsers.Where(p => p.Deleted == false && p.Gid == userID).Select(p => p.OrgID).Single();
                gOrgId = sessionOrgId;
            }
            //新建页面model
            WarehouseInformation oViewModel = new WarehouseInformation();
            //组织下拉框
            ViewBag.organization = GetSupportOrganizations();
            if (ParentOrgID != null)//有选中的组织
            {
                //ViewBag.IsFromOrg = true;
                oViewModel.aParent = ParentOrgID.Value;
                //组织下拉框为ParentOrgID的组织
                //MemberOrganization oParentOrg = dbEntity.MemberOrganizations.Where(o=>o.Gid == ParentOrgID && o.Deleted == false).FirstOrDefault();
                //ViewData["orgCode"] = new List<SelectListItem> { new SelectListItem { Text = oParentOrg.FullName.GetResource(CurrentSession.Culture), Value = oParentOrg.Gid.ToString() } };
                gOrgId = ParentOrgID.Value;//设置全局gOrgId
                SetCurrentPath();//当前程序路径
            }
            return View(oViewModel);
        }
        /// <summary>
        /// 组织首页中的仓库列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parentid"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListWarehouse(Guid? id, Guid? parentid, SearchModel searchModel)
        {
            if (gOrgId != null)
            {
                string sid = gOrgId.ToString();
                parentid = new Guid(sid);
            }
            IQueryable<WarehouseInformation> wareHouses = dbEntity.WarehouseInformations.Include("FullName").Include("ShortName").Where(p => p.Deleted == false && p.aParent == parentid).AsQueryable();
            GridColumnModelList<WarehouseInformation> columnsWare = new GridColumnModelList<WarehouseInformation>();
            columnsWare.Add(p => p.Gid).SetAsPrimaryKey();
            columnsWare.Add(p => p.Code);
            columnsWare.Add(p => p.FullName.Matter);
            columnsWare.Add(p => p.ShortName.Matter);
            columnsWare.Add(p => p.Ostatus);
            columnsWare.Add(p => p.Sorting);
            GridData gridDataWare = wareHouses.ToGridData(searchModel, columnsWare);
            return Json(gridDataWare, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 删除仓库
        /// </summary>
        /// <param name="strdeleid"></param>
        /// <param name="parentid"></param>
        /// <returns></returns>
        [HttpPost]
        public string deleteWare(Guid strdeleid, Guid? parentid)
        {
            if (gOrgId != null)
            {
                string sid = gOrgId.ToString();
                parentid = new Guid(sid);
            }
            var deleware = (from o in dbEntity.WarehouseInformations
                            where (o.Gid == strdeleid && o.aParent == parentid && o.Deleted == false)
                            select o).Single();
            deleware.Deleted = true;
            dbEntity.SaveChanges();
            return "success!";
        }

        #endregion

        #region 承运商
        /// <summary>
        /// 承运商首页
        /// </summary>
        /// <returns></returns>
        public ActionResult ShippingIndex(Guid? ParentOrgID = null)
        {
            // 权限验证
            if (!base.CheckPrivilege())
                RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });

            retype = 1;
            Guid? userID = CurrentSession.UserID;
            if (userID != null)
            {
                //获取用户所属组织
                var sessionOrgId = dbEntity.MemberUsers.Where(p => p.Deleted == false && p.Gid == userID).Select(p => p.OrgID).Single();
                gOrgId = sessionOrgId;
            }
            //新建页面model
            ShippingInformation oViewModel = new ShippingInformation();
            //组织下拉框
            ViewBag.organization = GetSupportOrganizations();
            if (ParentOrgID != null)//有选中的组织
            {
                //ViewBag.IsFromOrg = true;
                oViewModel.aParent = ParentOrgID.Value;
                //组织下拉框为ParentOrgID的组织
                //MemberOrganization oParentOrg = dbEntity.MemberOrganizations.Where(o=>o.Gid == ParentOrgID && o.Deleted == false).FirstOrDefault();
                //ViewData["orgCode"] = new List<SelectListItem> { new SelectListItem { Text = oParentOrg.FullName.GetResource(CurrentSession.Culture), Value = oParentOrg.Gid.ToString() } };
                gOrgId = ParentOrgID.Value;//设置全局gOrgId
                SetCurrentPath();//当前程序路径
            }
            return View(oViewModel);
        }
        /// <summary>
        /// 组织首页中的承运商列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parentid"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListShipping(Guid? id, Guid? parentid, SearchModel searchModel)
        {
            if (gOrgId != null)
            {
                string sid = gOrgId.ToString();
                parentid = new Guid(sid);
            }
            IQueryable<ShippingInformation> shippings = dbEntity.ShippingInformations.Include("FullName").Include("ShortName").Where(p => p.Deleted == false && p.aParent == parentid).AsQueryable();
            GridColumnModelList<ShippingInformation> columnsShip = new GridColumnModelList<ShippingInformation>();
            columnsShip.Add(p => p.Gid).SetAsPrimaryKey();
            columnsShip.Add(p => p.Code);
            columnsShip.Add(p => p.FullName.Matter);
            columnsShip.Add(p => p.ShortName.Matter);
            columnsShip.Add(p => p.Ostatus);
            columnsShip.Add(p => p.Sorting);
            GridData gridDataShip = shippings.ToGridData(searchModel, columnsShip);
            return Json(gridDataShip, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 删除承运商
        /// </summary>
        /// <param name="strdeleid"></param>
        /// <param name="parentid"></param>
        /// <returns></returns>
        [HttpPost]
        public string deleteShip(Guid strdeleid, Guid? parentid)
        {
            if (gOrgId != null)
            {
                string sid = gOrgId.ToString();
                parentid = new Guid(sid);
            }
            var deleship = (from o in dbEntity.ShippingInformations
                            where (o.Gid == strdeleid && o.aParent == parentid && o.Deleted == false)
                            select o).Single();
            deleship.Deleted = true;
            dbEntity.SaveChanges();
            return "success!";
        }
        #endregion

        #region 公用方法
        /// <summary>
        /// Tab页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Tabpage(Guid? id)
        {
            if (id != null)
            {
                gOrgId = (Guid)id;
            }
            else
            {
                gOrgId = Guid.Empty;
            }
            return View("TabPage");
        }
        /// <summary>
        /// 改变页面上面组织的下拉框
        /// </summary>
        public void changeOrg(Guid currentid)
        {
            if (currentid != null)
                gOrgId = currentid;
        }
        /// <summary>
        /// 改变正在添加的状态为False
        /// </summary>
        public string setAddingFalse()
        {
            if (!base.CheckPrivilege("EnableEdit"))//权限验证
                return "NoPrivilege";
            _isAdding = false;
            return "Success";
        }
        /// <summary>
        /// 返回当前是否是添加状态
        /// </summary>
        /// <returns></returns>
        public bool IsAdding()
        {
            return _isAdding;
        }
        /// <summary>
        /// 组织详细介绍页面.Html编辑器
        /// </summary>
        /// <param name="OrgType">页面传递的标志位，组织类型</param>
        /// <param name="Gid">组织的Gid</param>
        /// <returns></returns>
        public ActionResult OrgIntroduction(string OrgType, Guid Gid)
        {
            OrganizationBase oOrganizationModel = new OrganizationBase();
            ViewBag.OrgType = OrgType;
            switch (OrgType)//不同的组织类型
            {
                case "organization": oOrganizationModel = dbEntity.MemberOrganizations.Include("FullName").Include("ShortName").Include("Parent").Where(o => o.Gid == Gid).FirstOrDefault();
                    if (oOrganizationModel.aIntroduction != null)
                        oOrganizationModel.Introduction = RefreshLargeObject(oOrganizationModel.Introduction, Gid);
                    else
                        oOrganizationModel.Introduction = NewLargeObject(Gid);
                    break;
                case "channel": oOrganizationModel = dbEntity.MemberChannels.Include("FullName").Include("ShortName").Where(o => o.Gid == Gid).FirstOrDefault();
                    if (oOrganizationModel.aIntroduction != null)
                        oOrganizationModel.Introduction = RefreshLargeObject(oOrganizationModel.Introduction);
                    else
                        oOrganizationModel.Introduction = NewLargeObject();
                    break;
                case "warehouse": oOrganizationModel = dbEntity.WarehouseInformations.Include("FullName").Include("ShortName").Where(o => o.Gid == Gid).FirstOrDefault();
                    if (oOrganizationModel.aIntroduction != null)
                        oOrganizationModel.Introduction = RefreshLargeObject(oOrganizationModel.Introduction, oOrganizationModel.aParent);
                    else
                        oOrganizationModel.Introduction = NewLargeObject(oOrganizationModel.aParent);
                    break;
                case "supplier": oOrganizationModel = dbEntity.PurchaseSuppliers.Include("FullName").Include("ShortName").Where(o => o.Gid == Gid).FirstOrDefault();
                    if (oOrganizationModel.aIntroduction != null)
                        oOrganizationModel.Introduction = RefreshLargeObject(oOrganizationModel.Introduction, oOrganizationModel.aParent);
                    else
                        oOrganizationModel.Introduction = NewLargeObject(oOrganizationModel.aParent);
                    break;
                case "shipper": oOrganizationModel = dbEntity.ShippingInformations.Include("FullName").Include("ShortName").Where(o => o.Gid == Gid).FirstOrDefault();
                    if (oOrganizationModel.aIntroduction != null)
                        oOrganizationModel.Introduction = RefreshLargeObject(oOrganizationModel.Introduction, oOrganizationModel.aParent);
                    else
                        oOrganizationModel.Introduction = NewLargeObject(oOrganizationModel.aParent);
                    break;
            }
            try//由于节点的编辑功能代码都是EnableEdit 因此可以放在此处做验证权限，若代码不同，则需要单独对某种CASE做权限判断
            {
                ViewBag.privEnableEdit = CurrentSession.IsAdmin ? "1" : oProgramNodes["EnableEdit"];
            }
            catch (KeyNotFoundException)//捕捉到oProgramNodes没有Key 说明没有授权该节点
            {
                ViewBag.privEnableEdit = "0";
            }
            return View(oOrganizationModel);
        }
        /// <summary>
        /// 保存组织详情
        /// </summary>
        /// <param name="oBackModel"></param>
        [ValidateInput(false)]
        public string saveOrgIntroduction(OrganizationBase oBackModel)
        {
            if (!base.CheckPrivilege("EnableEdit"))//权限验证
                return "NoPrivilege";
            if (oBackModel.Gid != Guid.Empty)//编辑保存
            {
                OrganizationBase oOldOrganization = new OrganizationBase();
                switch (oBackModel.Otype)
                {
                    case 0: oOldOrganization = dbEntity.MemberOrganizations.Include("FullName").Include("ShortName").Where(o => o.Gid == oBackModel.Gid).FirstOrDefault();
                        if (oOldOrganization.aIntroduction != null)
                            oOldOrganization.Introduction = RefreshLargeObject(oOldOrganization.Introduction, oOldOrganization.Gid);
                        else
                            oOldOrganization.Introduction = NewLargeObject(oOldOrganization.Gid);
                        break;
                    case 1: oOldOrganization = dbEntity.MemberChannels.Include("FullName").Include("ShortName").Where(c => c.Gid == oBackModel.Gid).FirstOrDefault();
                        if (oOldOrganization.aIntroduction != null)
                            oOldOrganization.Introduction = RefreshLargeObject(oOldOrganization.Introduction);
                        else
                            oOldOrganization.Introduction = NewLargeObject();
                        break;
                    case 2: oOldOrganization = dbEntity.WarehouseInformations.Include("FullName").Include("ShortName").Where(c => c.Gid == oBackModel.Gid).FirstOrDefault();
                        if (oOldOrganization.aIntroduction != null)
                            oOldOrganization.Introduction = RefreshLargeObject(oOldOrganization.Introduction, oOldOrganization.aParent);
                        else
                            oOldOrganization.Introduction = NewLargeObject(oOldOrganization.aParent);
                        break;
                    case 3: oOldOrganization = dbEntity.PurchaseSuppliers.Include("FullName").Include("ShortName").Where(c => c.Gid == oBackModel.Gid).FirstOrDefault();
                        if (oOldOrganization.aIntroduction != null)
                            oOldOrganization.Introduction = RefreshLargeObject(oOldOrganization.Introduction, oOldOrganization.aParent);
                        else
                            oOldOrganization.Introduction = NewLargeObject(oOldOrganization.aParent);
                        break;
                    case 4: oOldOrganization = dbEntity.ShippingInformations.Include("FullName").Include("ShortName").Where(c => c.Gid == oBackModel.Gid).FirstOrDefault();
                        if (oOldOrganization.aIntroduction != null)
                            oOldOrganization.Introduction = RefreshLargeObject(oOldOrganization.Introduction, oOldOrganization.aParent);
                        else
                            oOldOrganization.Introduction = NewLargeObject(oOldOrganization.aParent);
                        break;
                }
                oOldOrganization.Introduction = oBackModel.Introduction;
            }
            dbEntity.SaveChanges();
            return "Success";
        }
        /// <summary>
        /// 添加和编辑组织
        /// </summary>
        /// <returns></returns>
        public ActionResult OrgDetail(string OrgType, Guid? Gid, Guid? ParentOrgID)
        {
            // 权限验证
            if (!base.CheckPrivilege("EnableEdit"))
                RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            //初始化组织
            OrganizationBase oOrganizationModel = new OrganizationBase { FullName = NewResource(ModelEnum.ResourceType.STRING, ParentOrgID), ShortName = NewResource(ModelEnum.ResourceType.STRING, ParentOrgID) };
            ViewBag.OrgType = OrgType;//用于页面控制
            if (ParentOrgID != null)
            {
                ViewBag.ParentName = dbEntity.MemberOrganizations.Where(o => o.Gid == ParentOrgID && o.Deleted == false).FirstOrDefault().FullName.GetResource(CurrentSession.Culture);
                oOrganizationModel.aParent = ParentOrgID;
            }
            else
                ViewBag.ParentName = "";
            //扩展类型下拉框
            List<GeneralStandardCategory> ExTypeList = new List<GeneralStandardCategory>();
            if (Gid != null || OrgType == "organization")//是编辑该组织
            {
                switch (OrgType)
                {
                    case "organization": Gid = gOrgId;
                        oOrganizationModel = dbEntity.MemberOrganizations.Include("FullName").Include("ShortName").Include("Parent").Where(o => o.Gid == Gid).FirstOrDefault();
                        oOrganizationModel.FullName = RefreshResource(ModelEnum.ResourceType.STRING, oOrganizationModel.FullName, oOrganizationModel.Gid);
                        oOrganizationModel.ShortName = RefreshResource(ModelEnum.ResourceType.STRING, oOrganizationModel.ShortName, oOrganizationModel.Gid);
                        ExTypeList = dbEntity.GeneralStandardCategorys.Include("Name").Where(c => c.Ctype == (byte)ModelEnum.StandardCategoryType.ORGANIZATION).ToList();
                        break;
                    case "channel": oOrganizationModel = dbEntity.MemberChannels.Include("FullName").Include("ShortName").Where(o => o.Gid == Gid).FirstOrDefault();
                        oOrganizationModel.FullName = RefreshResource(ModelEnum.ResourceType.STRING, oOrganizationModel.FullName);
                        oOrganizationModel.ShortName = RefreshResource(ModelEnum.ResourceType.STRING, oOrganizationModel.ShortName);
                        ExTypeList = dbEntity.GeneralStandardCategorys.Include("Name").Where(c => c.Ctype == (byte)ModelEnum.StandardCategoryType.CHANNEL).ToList();
                        break;
                    case "warehouse": oOrganizationModel = dbEntity.WarehouseInformations.Include("FullName").Include("ShortName").Where(o => o.Gid == Gid).FirstOrDefault();
                        oOrganizationModel.FullName = RefreshResource(ModelEnum.ResourceType.STRING, oOrganizationModel.FullName, oOrganizationModel.Parent.Gid);
                        oOrganizationModel.ShortName = RefreshResource(ModelEnum.ResourceType.STRING, oOrganizationModel.ShortName, oOrganizationModel.Parent.Gid);
                        break;
                    case "supplier": oOrganizationModel = dbEntity.PurchaseSuppliers.Include("FullName").Include("ShortName").Where(o => o.Gid == Gid).FirstOrDefault();
                        oOrganizationModel.FullName = RefreshResource(ModelEnum.ResourceType.STRING, oOrganizationModel.FullName, oOrganizationModel.Parent.Gid);
                        oOrganizationModel.ShortName = RefreshResource(ModelEnum.ResourceType.STRING, oOrganizationModel.ShortName, oOrganizationModel.Parent.Gid);
                        break;
                    case "shipper": oOrganizationModel = dbEntity.ShippingInformations.Include("FullName").Include("ShortName").Where(o => o.Gid == Gid).FirstOrDefault();
                        oOrganizationModel.FullName = RefreshResource(ModelEnum.ResourceType.STRING, oOrganizationModel.FullName, oOrganizationModel.Parent.Gid);
                        oOrganizationModel.ShortName = RefreshResource(ModelEnum.ResourceType.STRING, oOrganizationModel.ShortName, oOrganizationModel.Parent.Gid);
                        break;
                }
                ViewBag.IsAdding = false;
            }
            else//添加组织
            {
                switch (OrgType)
                {
                    case "organization": oOrganizationModel.Otype = (byte)ModelEnum.OrganizationType.CORPORATION;
                        ExTypeList = dbEntity.GeneralStandardCategorys.Include("Name").Where(c => c.Ctype == (byte)ModelEnum.StandardCategoryType.ORGANIZATION).ToList();
                        break;
                    case "channel": oOrganizationModel.Otype = (byte)ModelEnum.OrganizationType.CHANNEL;
                        ExTypeList = dbEntity.GeneralStandardCategorys.Include("Name").Where(c => c.Ctype == (byte)ModelEnum.StandardCategoryType.CHANNEL).ToList();
                        break;
                    case "warehouse": oOrganizationModel.Otype = (byte)ModelEnum.OrganizationType.WAREHOUSE; break;
                    case "supplier": oOrganizationModel.Otype = (byte)ModelEnum.OrganizationType.SUPPLIER; break;
                    case "shipper": oOrganizationModel.Otype = (byte)ModelEnum.OrganizationType.SHIPPER; break;
                }
                ViewBag.IsAdding = true;
            }
            //生成“状态”下拉框
            List<SelectListItem> organizationstatuslist = GetSelectList(oOrganizationModel.OrganStatusList);
            ViewBag.organizationstatuslist = organizationstatuslist;
            //生成“类型”下拉框
            List<SelectListItem> organizationtypelist = GetSelectList(oOrganizationModel.OrganTypeList);
            ViewBag.organizationtypelist = organizationtypelist;
            //扩展类型下拉框
            List<SelectListItem> lExType = new List<SelectListItem>();
            lExType.Add(new SelectListItem { Text = null, Value = null });
            for (int i = 0; i < ExTypeList.Count; i++)
            {
                lExType.Add(new SelectListItem { Text = ExTypeList.ElementAt(i).Name.GetResource(CurrentSession.Culture), Value = ExTypeList.ElementAt(i).Gid.ToString() });
            }
            ViewBag.lExType = lExType;
            //是否是末级
            List<SelectListItem> IsTerminal = SelectEnumList(false);
            ViewBag.oIsTerminal = IsTerminal;

            return View(oOrganizationModel);
        }
        /// <summary>
        /// 保存组织
        /// </summary>
        /// <param name="oOrganization"></param>
        public void SaveEditOrgDetail(OrganizationBase oBackModel)
        {
            if (oBackModel.Gid != Guid.Empty)//编辑保存
            {
                OrganizationBase oOldOrganization = new OrganizationBase { FullName = NewResource(ModelEnum.ResourceType.STRING, oBackModel.aParent), ShortName = NewResource(ModelEnum.ResourceType.STRING, oBackModel.aParent) };
                switch (oBackModel.Otype)
                {
                    case 0: oOldOrganization = dbEntity.MemberOrganizations.Include("FullName").Include("ShortName").Where(o => o.Gid == oBackModel.Gid).FirstOrDefault(); break;
                    case 1: oOldOrganization = dbEntity.MemberChannels.Include("FullName").Include("ShortName").Where(c => c.Gid == oBackModel.Gid).FirstOrDefault(); break;
                    case 2: oOldOrganization = dbEntity.WarehouseInformations.Include("FullName").Include("ShortName").Where(c => c.Gid == oBackModel.Gid).FirstOrDefault(); break;
                    case 3: oOldOrganization = dbEntity.PurchaseSuppliers.Include("FullName").Include("ShortName").Where(c => c.Gid == oBackModel.Gid).FirstOrDefault(); break;
                    case 4: oOldOrganization = dbEntity.ShippingInformations.Include("FullName").Include("ShortName").Where(c => c.Gid == oBackModel.Gid).FirstOrDefault(); break;
                }
                oOldOrganization.FullName.SetResource(ModelEnum.ResourceType.STRING, oBackModel.FullName);
                oOldOrganization.ShortName.SetResource(ModelEnum.ResourceType.STRING, oBackModel.ShortName);
                oOldOrganization.Code = oBackModel.Code;
                oOldOrganization.ExCode = oBackModel.ExCode;
                oOldOrganization.Ostatus = oBackModel.Ostatus;
                oOldOrganization.Otype = oBackModel.Otype;
                oOldOrganization.ExType = oBackModel.ExType;
                oOldOrganization.Terminal = oBackModel.Terminal;
                oOldOrganization.aLocation = oBackModel.aLocation;
                oOldOrganization.FullAddress = oBackModel.FullAddress;
                oOldOrganization.PostCode = oBackModel.PostCode;
                oOldOrganization.Contact = oBackModel.Contact;
                oOldOrganization.CellPhone = oBackModel.CellPhone;
                oOldOrganization.WorkPhone = oBackModel.WorkPhone;
                oOldOrganization.WorkFax = oBackModel.WorkFax;
                oOldOrganization.Email = oBackModel.Email;
                oOldOrganization.HomeUrl = oBackModel.HomeUrl;
                oOldOrganization.Sorting = oBackModel.Sorting;
                oOldOrganization.Brief = oBackModel.Brief;
                oOldOrganization.Remark = oBackModel.Remark;
            }
            else//添加保存
            {
                switch (oBackModel.Otype)
                {
                    case 0: break;
                    case 1: MemberChannel oNewMemberChannel = dbEntity.MemberChannels.Include("FullName").Include("ShortName").Where(o => o.Code == oBackModel.Code).FirstOrDefault();
                        if (oNewMemberChannel != null)//原来存在,则是编辑
                        {
                            oNewMemberChannel.Deleted = false;
                            oNewMemberChannel.FullName.SetResource(ModelEnum.ResourceType.STRING, oBackModel.FullName);
                            oNewMemberChannel.ShortName.SetResource(ModelEnum.ResourceType.STRING, oBackModel.ShortName);
                            //oNewMemberChannel.Code = oBackModel.Code;
                            oNewMemberChannel.ExCode = oBackModel.ExCode;
                            oNewMemberChannel.Ostatus = oBackModel.Ostatus;
                            //oNewMemberChannel.Otype = oBackModel.Otype;
                            oNewMemberChannel.ExType = oBackModel.ExType;
                            oNewMemberChannel.Terminal = oBackModel.Terminal;
                            oNewMemberChannel.aLocation = oBackModel.aLocation;
                            oNewMemberChannel.FullAddress = oBackModel.FullAddress;
                            oNewMemberChannel.PostCode = oBackModel.PostCode;
                            oNewMemberChannel.Contact = oBackModel.Contact;
                            oNewMemberChannel.CellPhone = oBackModel.CellPhone;
                            oNewMemberChannel.WorkPhone = oBackModel.WorkPhone;
                            oNewMemberChannel.WorkFax = oBackModel.WorkFax;
                            oNewMemberChannel.Email = oBackModel.Email;
                            oNewMemberChannel.HomeUrl = oBackModel.HomeUrl;
                            oNewMemberChannel.Sorting = oBackModel.Sorting;
                            oNewMemberChannel.Brief = oBackModel.Brief;
                            oNewMemberChannel.Remark = oBackModel.Remark;
                        }
                        else 
                        {
                            oNewMemberChannel = new MemberChannel { FullName = NewResource(ModelEnum.ResourceType.STRING), ShortName = NewResource(ModelEnum.ResourceType.STRING) };
                            oNewMemberChannel.FullName.SetResource(ModelEnum.ResourceType.STRING, oBackModel.FullName);
                            oNewMemberChannel.ShortName.SetResource(ModelEnum.ResourceType.STRING, oBackModel.ShortName);
                            oNewMemberChannel.Code = oBackModel.Code;
                            oNewMemberChannel.ExCode = oBackModel.ExCode;
                            oNewMemberChannel.Ostatus = oBackModel.Ostatus;
                            oNewMemberChannel.Otype = oBackModel.Otype;
                            oNewMemberChannel.ExType = oBackModel.ExType;
                            oNewMemberChannel.Terminal = oBackModel.Terminal;
                            oNewMemberChannel.aLocation = oBackModel.aLocation;
                            oNewMemberChannel.FullAddress = oBackModel.FullAddress;
                            oNewMemberChannel.PostCode = oBackModel.PostCode;
                            oNewMemberChannel.Contact = oBackModel.Contact;
                            oNewMemberChannel.CellPhone = oBackModel.CellPhone;
                            oNewMemberChannel.WorkPhone = oBackModel.WorkPhone;
                            oNewMemberChannel.WorkFax = oBackModel.WorkFax;
                            oNewMemberChannel.Email = oBackModel.Email;
                            oNewMemberChannel.HomeUrl = oBackModel.HomeUrl;
                            oNewMemberChannel.Sorting = oBackModel.Sorting;
                            oNewMemberChannel.Brief = oBackModel.Brief;
                            oNewMemberChannel.Remark = oBackModel.Remark;
                            dbEntity.MemberChannels.Add(oNewMemberChannel); 
                        }
                        break;
                    case 2: WarehouseInformation oNewWarehouse = dbEntity.WarehouseInformations.Include("FullName").Include("ShortName").Where(w => w.Code == oBackModel.Code).FirstOrDefault();
                        if (oNewWarehouse != null)
                        {
                            oNewWarehouse.Deleted = false;
                            oNewWarehouse.FullName.SetResource(ModelEnum.ResourceType.STRING, oBackModel.FullName);
                            oNewWarehouse.ShortName.SetResource(ModelEnum.ResourceType.STRING, oBackModel.ShortName);
                            //oNewWarehouse.aParent = oBackModel.aParent;
                            //oNewWarehouse.Code = oBackModel.Code;
                            oNewWarehouse.ExCode = oBackModel.ExCode;
                            oNewWarehouse.Ostatus = oBackModel.Ostatus;
                            //oNewWarehouse.Otype = oBackModel.Otype;
                            oNewWarehouse.ExType = oBackModel.ExType;
                            oNewWarehouse.Terminal = oBackModel.Terminal;
                            oNewWarehouse.aLocation = oBackModel.aLocation;
                            oNewWarehouse.FullAddress = oBackModel.FullAddress;
                            oNewWarehouse.PostCode = oBackModel.PostCode;
                            oNewWarehouse.Contact = oBackModel.Contact;
                            oNewWarehouse.CellPhone = oBackModel.CellPhone;
                            oNewWarehouse.WorkPhone = oBackModel.WorkPhone;
                            oNewWarehouse.WorkFax = oBackModel.WorkFax;
                            oNewWarehouse.Email = oBackModel.Email;
                            oNewWarehouse.HomeUrl = oBackModel.HomeUrl;
                            oNewWarehouse.Sorting = oBackModel.Sorting;
                            oNewWarehouse.Brief = oBackModel.Brief;
                            oNewWarehouse.Remark = oBackModel.Remark;
                        }
                        else
                        {
                            oNewWarehouse = new WarehouseInformation { FullName = NewResource(ModelEnum.ResourceType.STRING, oBackModel.aParent), ShortName = NewResource(ModelEnum.ResourceType.STRING, oBackModel.aParent) };
                            oNewWarehouse.FullName.SetResource(ModelEnum.ResourceType.STRING, oBackModel.FullName);
                            oNewWarehouse.ShortName.SetResource(ModelEnum.ResourceType.STRING, oBackModel.ShortName);
                            oNewWarehouse.aParent = oBackModel.aParent;
                            oNewWarehouse.Code = oBackModel.Code;
                            oNewWarehouse.ExCode = oBackModel.ExCode;
                            oNewWarehouse.Ostatus = oBackModel.Ostatus;
                            oNewWarehouse.Otype = oBackModel.Otype;
                            oNewWarehouse.ExType = oBackModel.ExType;
                            oNewWarehouse.Terminal = oBackModel.Terminal;
                            oNewWarehouse.aLocation = oBackModel.aLocation;
                            oNewWarehouse.FullAddress = oBackModel.FullAddress;
                            oNewWarehouse.PostCode = oBackModel.PostCode;
                            oNewWarehouse.Contact = oBackModel.Contact;
                            oNewWarehouse.CellPhone = oBackModel.CellPhone;
                            oNewWarehouse.WorkPhone = oBackModel.WorkPhone;
                            oNewWarehouse.WorkFax = oBackModel.WorkFax;
                            oNewWarehouse.Email = oBackModel.Email;
                            oNewWarehouse.HomeUrl = oBackModel.HomeUrl;
                            oNewWarehouse.Sorting = oBackModel.Sorting;
                            oNewWarehouse.Brief = oBackModel.Brief;
                            oNewWarehouse.Remark = oBackModel.Remark;
                            dbEntity.WarehouseInformations.Add(oNewWarehouse);
                        }
                        break;
                    case 3: PurchaseSupplier oNewPurchaseSupplier = dbEntity.PurchaseSuppliers.Include("FullName").Include("ShortName").Where(s => s.Code == oBackModel.Code).FirstOrDefault();
                        if (oNewPurchaseSupplier != null)
                        {
                            oNewPurchaseSupplier.Deleted = false;
                            oNewPurchaseSupplier.FullName.SetResource(ModelEnum.ResourceType.STRING, oBackModel.FullName);
                            oNewPurchaseSupplier.ShortName.SetResource(ModelEnum.ResourceType.STRING, oBackModel.ShortName);
                            //oNewPurchaseSupplier.aParent = oBackModel.aParent;
                            //oNewPurchaseSupplier.Code = oBackModel.Code;
                            oNewPurchaseSupplier.ExCode = oBackModel.ExCode;
                            oNewPurchaseSupplier.Ostatus = oBackModel.Ostatus;
                            //oNewPurchaseSupplier.Otype = oBackModel.Otype;
                            oNewPurchaseSupplier.ExType = oBackModel.ExType;
                            oNewPurchaseSupplier.Terminal = oBackModel.Terminal;
                            oNewPurchaseSupplier.aLocation = oBackModel.aLocation;
                            oNewPurchaseSupplier.FullAddress = oBackModel.FullAddress;
                            oNewPurchaseSupplier.PostCode = oBackModel.PostCode;
                            oNewPurchaseSupplier.Contact = oBackModel.Contact;
                            oNewPurchaseSupplier.CellPhone = oBackModel.CellPhone;
                            oNewPurchaseSupplier.WorkPhone = oBackModel.WorkPhone;
                            oNewPurchaseSupplier.WorkFax = oBackModel.WorkFax;
                            oNewPurchaseSupplier.Email = oBackModel.Email;
                            oNewPurchaseSupplier.HomeUrl = oBackModel.HomeUrl;
                            oNewPurchaseSupplier.Sorting = oBackModel.Sorting;
                            oNewPurchaseSupplier.Brief = oBackModel.Brief;
                            oNewPurchaseSupplier.Remark = oBackModel.Remark;
                        }
                        else
                        {
                            oNewPurchaseSupplier = new PurchaseSupplier { FullName = NewResource(ModelEnum.ResourceType.STRING, oBackModel.aParent), ShortName = NewResource(ModelEnum.ResourceType.STRING, oBackModel.aParent) };
                            oNewPurchaseSupplier.FullName.SetResource(ModelEnum.ResourceType.STRING, oBackModel.FullName);
                            oNewPurchaseSupplier.ShortName.SetResource(ModelEnum.ResourceType.STRING, oBackModel.ShortName);
                            oNewPurchaseSupplier.aParent = oBackModel.aParent;
                            oNewPurchaseSupplier.Code = oBackModel.Code;
                            oNewPurchaseSupplier.ExCode = oBackModel.ExCode;
                            oNewPurchaseSupplier.Ostatus = oBackModel.Ostatus;
                            oNewPurchaseSupplier.Otype = oBackModel.Otype;
                            oNewPurchaseSupplier.ExType = oBackModel.ExType;
                            oNewPurchaseSupplier.Terminal = oBackModel.Terminal;
                            oNewPurchaseSupplier.aLocation = oBackModel.aLocation;
                            oNewPurchaseSupplier.FullAddress = oBackModel.FullAddress;
                            oNewPurchaseSupplier.PostCode = oBackModel.PostCode;
                            oNewPurchaseSupplier.Contact = oBackModel.Contact;
                            oNewPurchaseSupplier.CellPhone = oBackModel.CellPhone;
                            oNewPurchaseSupplier.WorkPhone = oBackModel.WorkPhone;
                            oNewPurchaseSupplier.WorkFax = oBackModel.WorkFax;
                            oNewPurchaseSupplier.Email = oBackModel.Email;
                            oNewPurchaseSupplier.HomeUrl = oBackModel.HomeUrl;
                            oNewPurchaseSupplier.Sorting = oBackModel.Sorting;
                            oNewPurchaseSupplier.Brief = oBackModel.Brief;
                            oNewPurchaseSupplier.Remark = oBackModel.Remark;
                            dbEntity.PurchaseSuppliers.Add(oNewPurchaseSupplier); 
                        }
                        break;
                    case 4: ShippingInformation oNewShippingInformations = dbEntity.ShippingInformations.Include("FullName").Include("ShortName").Where(s => s.Code == oBackModel.Code).FirstOrDefault();
                        if (oNewShippingInformations != null)
                        {
                            oNewShippingInformations.Deleted = false;
                            oNewShippingInformations.FullName.SetResource(ModelEnum.ResourceType.STRING, oBackModel.FullName);
                            oNewShippingInformations.ShortName.SetResource(ModelEnum.ResourceType.STRING, oBackModel.ShortName);
                            //oNewShippingInformations.aParent = oBackModel.aParent;
                            //oNewShippingInformations.Code = oBackModel.Code;
                            oNewShippingInformations.ExCode = oBackModel.ExCode;
                            oNewShippingInformations.Ostatus = oBackModel.Ostatus;
                            //oNewShippingInformations.Otype = oBackModel.Otype;
                            oNewShippingInformations.ExType = oBackModel.ExType;
                            oNewShippingInformations.Terminal = oBackModel.Terminal;
                            oNewShippingInformations.aLocation = oBackModel.aLocation;
                            oNewShippingInformations.FullAddress = oBackModel.FullAddress;
                            oNewShippingInformations.PostCode = oBackModel.PostCode;
                            oNewShippingInformations.Contact = oBackModel.Contact;
                            oNewShippingInformations.CellPhone = oBackModel.CellPhone;
                            oNewShippingInformations.WorkPhone = oBackModel.WorkPhone;
                            oNewShippingInformations.WorkFax = oBackModel.WorkFax;
                            oNewShippingInformations.Email = oBackModel.Email;
                            oNewShippingInformations.HomeUrl = oBackModel.HomeUrl;
                            oNewShippingInformations.Sorting = oBackModel.Sorting;
                            oNewShippingInformations.Brief = oBackModel.Brief;
                            oNewShippingInformations.Remark = oBackModel.Remark;
                        }
                        else
                        {
                            oNewShippingInformations = new ShippingInformation { FullName = NewResource(ModelEnum.ResourceType.STRING, oBackModel.aParent), ShortName = NewResource(ModelEnum.ResourceType.STRING, oBackModel.aParent) };
                            oNewShippingInformations.FullName.SetResource(ModelEnum.ResourceType.STRING, oBackModel.FullName);
                            oNewShippingInformations.ShortName.SetResource(ModelEnum.ResourceType.STRING, oBackModel.ShortName);
                            oNewShippingInformations.aParent = oBackModel.aParent;
                            oNewShippingInformations.Code = oBackModel.Code;
                            oNewShippingInformations.ExCode = oBackModel.ExCode;
                            oNewShippingInformations.Ostatus = oBackModel.Ostatus;
                            oNewShippingInformations.Otype = oBackModel.Otype;
                            oNewShippingInformations.ExType = oBackModel.ExType;
                            oNewShippingInformations.Terminal = oBackModel.Terminal;
                            oNewShippingInformations.aLocation = oBackModel.aLocation;
                            oNewShippingInformations.FullAddress = oBackModel.FullAddress;
                            oNewShippingInformations.PostCode = oBackModel.PostCode;
                            oNewShippingInformations.Contact = oBackModel.Contact;
                            oNewShippingInformations.CellPhone = oBackModel.CellPhone;
                            oNewShippingInformations.WorkPhone = oBackModel.WorkPhone;
                            oNewShippingInformations.WorkFax = oBackModel.WorkFax;
                            oNewShippingInformations.Email = oBackModel.Email;
                            oNewShippingInformations.HomeUrl = oBackModel.HomeUrl;
                            oNewShippingInformations.Sorting = oBackModel.Sorting;
                            oNewShippingInformations.Brief = oBackModel.Brief;
                            oNewShippingInformations.Remark = oBackModel.Remark;
                            dbEntity.ShippingInformations.Add(oNewShippingInformations);
                        }
                        break;
                }
            }
            dbEntity.SaveChanges();
        }
        /// <summary>
        /// 检查编辑或者添加的Code是否呗使用
        /// </summary>
        /// <param name="Code">待检查的Code</param>
        /// <returns></returns>
        public bool CodeCheck(string Code)
        {
            return (dbEntity.MemberOrganizations.Any(o => o.Code == Code && o.Deleted == false) ||
                dbEntity.MemberChannels.Any(o => o.Code == Code && o.Deleted == false) ||
                dbEntity.WarehouseInformations.Any(o => o.Code == Code && o.Deleted == false) ||
                dbEntity.PurchaseSuppliers.Any(o => o.Code == Code && o.Deleted == false) ||
                dbEntity.ShippingInformations.Any(o => o.Code == Code && o.Deleted == false));
        }

        #endregion

        #region 属性?
        /// <summary>
        /// ？？？？
        /// </summary>
        /// <param name="culid"></param>
        /// <returns></returns>
        public string cultureText(int culid)
        {
            CultureInfo a = new CultureInfo(culid);
            return a.NativeName;
        }

        /// <summary>
        /// ？？？？
        /// </summary>
        /// <param name="strdeleid"></param>
        /// <returns></returns>
        [HttpPost]
        public string deleteOrg(Guid strdeleid)
        {
            int oType = (int)Session["oType"];
            if (oType == 0)
            {
                var deleorg = (from o in dbEntity.MemberOrganizations
                               where (o.Gid == strdeleid && o.Deleted == false)
                               select o).Single();
                deleorg.Deleted = true;
            }
            else if (oType == 1)
            {
               var deleorg = (from o in dbEntity.MemberChannels
                           where (o.Gid == strdeleid && o.Deleted == false)
                           select o).Single();
               deleorg.Deleted = true;
            }
            dbEntity.SaveChanges();

            return "success!";
        }


        /// <summary>
        /// 逻辑删除数据?????
        /// </summary>
        /// <param name="Gid"></param>
        /// <returns></returns>
        public ActionResult Delete(Guid Gid)
        {
            var memorganization = (from o in dbEntity.MemberOrganizations
                                   where o.Gid == Gid
                                   select o).Single();
            memorganization.Deleted = true;

            dbEntity.SaveChanges();
            
            return RedirectToAction("Index");
        }



        /// <summary>
        /// 逻辑删除属性表格数据???
        /// </summary>
        /// <param name="Gid"></param>
        /// <returns></returns>
        public ActionResult DeleteAttGrid(Guid Gid)
        {
            var attri = (from o in dbEntity.MemberOrgAttributes
                                   where o.Gid == Gid
                                   select o).Single();
            attri.Deleted = true;

            dbEntity.SaveChanges();

            return RedirectToAction("OrganizationDefination");
        }
       
        /// <summary>
        /// ??????
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult ListAttribute(SearchModel searchModel)
        {
            Guid id = new Guid();
            IQueryable<MemberOrgAttribute> memorgattr;
            if (Session["sessionId"] != null)
            {
                string sid = Session["sessionId"].ToString();
                id = new Guid(sid);
                memorgattr = dbEntity.MemberOrgAttributes.Where(p => p.OrgID == id && p.Deleted == false).AsQueryable();
            }
            else
            {
                memorgattr = dbEntity.MemberOrgAttributes.ToList().AsQueryable();
            }
            GridColumnModelList<MemberOrgAttribute> columns = new GridColumnModelList<MemberOrgAttribute>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Optional.Name.Matter);
            //columns.Add(p => p.OptionalResult.Name.Matter);
            columns.Add(p => p.Matter);

            GridData gridData = memorgattr.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }


        //[HttpPost]
        //public ViewResult OrgChannel(Guid[] selectedChannel)
        //{
        //    //MemberOrganization memberOrganization;
        //    //此时编辑状态
        //    if (selectedChannel != null)
        //    {
        //        if (Session["sessionOrgId"] != null)
        //        {
        //            string sid = Session["sessionOrgId"].ToString();
        //            Guid id = new Guid(sid);
        //            //memberOrganization = (from o in dbEntity.MemberOrganizations
        //            //                      where (o.Gid == id && o.Deleted == false)
        //            //                      select o).Single();
        //            //判断memberOrgChannel是否存在该组织的渠道记录
        //            var memOrgChas = dbEntity.MemberOrgChannels.Where(o => o.OrgID == id).ToList();
        //            foreach (var item in memOrgChas)
        //            {
        //                item.Deleted = true;
        //            }
        //            foreach (Guid gid in selectedChannel)
        //            {
        //                bool find = false;
        //                foreach (var item in memOrgChas)
        //                {
        //                    if (item.ChlID == gid)
        //                    {
        //                        find = true;
        //                        if (item.Deleted == true)
        //                            item.Deleted = false;
        //                        break;
        //                    }
        //                }
        //                if (find == false)
        //                {
        //                    MemberOrgChannel item = new MemberOrgChannel();
        //                    item.OrgID = id;
        //                    item.ChlID = gid;
        //                    dbEntity.MemberOrgChannels.Add(item);
        //                }
        //            }
                 
        //            dbEntity.SaveChanges();
        //            ViewBag.addChaMessage = "添加成功！";
        //        }
        //        else
        //        {
        //            ViewBag.addChaMessage = "请先进入组织详情填写页面！";
        //        }
        //    }
           
        //    return View("Index");
        //}

        /// <summary>
        /// 添加组织属性--从数据库获取下拉框内容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult OrgAttribute(Guid? id)
        {
            //var option = from p in dbEntity.GeneralOptionals.Include("Name")
            //             where (p.OrgID == new Guid("2cb5565e-c8c7-e011-87f7-00218660bc3a") && p.Otype == 1)
            //             select new { Gid = p.Gid, aName = p.Name.Matter };

            //var optionResult = from p in dbEntity.GeneralOptItems.Include("Name").Include("OptID")
            //                   where (p.Optional.OrgID == new Guid("2cb5565e-c8c7-e011-87f7-00218660bc3a") && p.Optional.Otype == 1
            //                   && p.Optional.InputMode == 1 && p.OptID == p.Optional.Gid)
            //                   select new { Gid = p.Gid, aName = p.Name.Matter };
            ////      var memberOrgAttributes = from p in db.MemberOrgAttributes
            ////                               where (p.OrgID == id )
            ////                               select p;
            //ViewData["OptID"] = new SelectList(option, "Gid", "aName");
            //ViewData["OptResult"] = new SelectList(optionResult, "Gid", "aName");
            ////      ViewBag.memberOrgAttribute = new List<MemberOrgAttribute>(memberOrgAttribute);
            return View();
        }

        /// <summary>
        /// 添加组织属性--保存至数据库MemberOrgAttribute
        /// </summary>
        /// <param name="optitemCount"></param>
        /// <param name="memberOrganization"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OrgAttribute(int? optitemCount,Guid? id)
        {
            //MemberOrganization memberOrganization;
            //if (Session["sessionId"] != null)
            //{
            //    string sid = Session["sessionId"].ToString();
            //    id = new Guid(sid);
            //    memberOrganization = (from m in dbEntity.MemberOrganizations
            //                          where (m.Gid == id)
            //                          select m).Single();
            //}
            //else
            //    memberOrganization = new MemberOrganization();
           
            //if (!ModelState.IsValid)
            //{
            //    ICollection<MemberOrgAttribute> memberOrgAttributes = new List<MemberOrgAttribute>();
            //    for (int i = 0; i < optitemCount; i++)
            //    {
            //        MemberOrgAttribute memberOrgAttribute = new MemberOrgAttribute();
            //        memberOrgAttribute.OrgID = memberOrganization.Gid;
            //        //            memberOrgAttribute.OptID = memberOrganization.Attributes.ElementAt(i).OptID;
            //        //            memberOrgAttribute.OptResult = memberOrganization.Attributes.ElementAt(i).OptResult;
            //        //             memberOrgAttribute.Matter  = memberOrganization.Attributes.ElementAt(i).Matter ;
            //        memberOrgAttribute.OptID = new Guid(Request.Form["OptID" + i]);
            //        memberOrgAttribute.OptResult = new Guid(Request.Form["OptResult" + i]);
            //        memberOrgAttribute.Matter = Request.Form["Matter" + i];
            //        memberOrgAttributes.Add(memberOrgAttribute);
            //    }
            //    memberOrganization.Attributes = memberOrgAttributes;
            //    if (Session["sessionId"] == null)
            //    {
            //        dbEntity.MemberOrganizations.Add(memberOrganization);
            //    }
            //    dbEntity.SaveChanges();
            //    return RedirectToAction("Index");
            //}
            return View();
        }
        public ActionResult RegionTree()
        {
            return View();
        }

        ///// <summary>
        ///// 获取属性编辑输入模式
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public string getInputMode(Guid id)
        //{
        //    var inputMode = (from i in dbEntity.GeneralOptionals
        //                     where (i.Gid == id)
        //                     select i.InputMode).Single();
        //    string json = inputMode.ToString();
        //    return json;
        //}
#endregion

    }
    
}
