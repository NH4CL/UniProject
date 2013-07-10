using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data;
using System.Text;
using System.Globalization;
using MVC.Controls;
using MVC.Controls.Grid;
using LiveAzure.Models;
using LiveAzure.Models.General;
using LiveAzure.Models.Member;

namespace LiveAzure.Stage.Controllers
{
    public class ConfigController : BaseController
    {

        public static byte mtype;   //计量单位的类型

        /// <summary>
        /// 用户所属组织，从Session里读出
        /// </summary>
        /// <returns></returns>
        public Guid GetOrganization()
        {
            MemberUser oUser = dbEntity.MemberUsers.Find(CurrentSession.UserID);
            return (oUser.OrgID);
        }

        #region Config
        /// <summary>
        /// Config 页面Action
        /// </summary>
        /// <returns>Config 页面</returns>
        public ActionResult Index()
        {
            // 权限验证
            string strProgramCode = Request.RequestContext.RouteData.Values["Controller"].ToString() +
                Request.RequestContext.RouteData.Values["Action"].ToString();
            if (!base.Permission(strProgramCode))
                return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
            ViewBag.EnableEdit = (base.GetProgramNode("EnableEdit") == "1") ? true : false;

            try
            {
                List<GeneralConfig> configs = dbEntity.GeneralConfigs.Where(item => item.Parent == null).ToList();
                List<SelectListItem> list = new List<SelectListItem>();
                foreach (GeneralConfig config in configs)
                    list.Add(new SelectListItem { Text = config.Code, Value = config.Gid.ToString() });
                list.ElementAt(0).Selected = true;
                return View(list);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
            }
        }

        /// <summary>
        /// ConfigList函数
        /// </summary>
        /// <param name="searchModel">SearchModel</param>
        /// <returns>Json数据</returns>
        public ActionResult ConfigList(SearchModel searchModel, Guid? gid)
        {
            try
            {
                CultureInfo culture = new CultureInfo(CurrentSession.Culture);
                if (!gid.HasValue)
                    gid = dbEntity.GeneralConfigs.Where(item => item.Parent == null).First().Gid;
                IQueryable<GeneralConfig> configs = dbEntity.GeneralConfigs.Where(item => item.Parent.Gid == gid).AsQueryable();
                GridColumnModelList<GeneralConfig> columns = new GridColumnModelList<GeneralConfig>();
                columns.Add(p => p.Gid).SetAsPrimaryKey();
                columns.Add(p => p.Code);
                columns.Add(p => (p.Culture == 0) ? @LiveAzure.Resource.Stage.ConfigController.AllCulture : (new CultureInfo(p.Culture).NativeName)).SetName("Culture");
                columns.Add(p => p.Ctype);
                columns.Add(p => p.IntValue);
                columns.Add(p => p.DecValue);
                columns.Add(p => p.StrValue);
                columns.Add(p => ((p.DateValue == null) ? "" : p.DateValue.Value.ToString(culture.DateTimeFormat.ShortDatePattern))).SetName("DateValue");
                columns.Add(p => p.Remark);
                GridData gridData = configs.ToGridData(searchModel, columns);
                return Json(gridData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
            }
        }

        /// <summary>
        /// 对Config进行编辑的Action
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public ActionResult ConfigEdit(GeneralConfig config)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                try
                {
                    GeneralConfig dbConfig = dbEntity.GeneralConfigs.Single(item => item.Gid == config.Gid);
                    dbConfig.Ctype = config.Ctype;
                    switch (dbConfig.Ctype)
                    {
                        case (byte)ModelEnum.ConfigParamType.INTEGER:
                            dbConfig.IntValue = config.IntValue;
                            dbConfig.DecValue = 0;
                            dbConfig.StrValue = null;
                            dbConfig.DateValue = null;
                            break;
                        case (byte)ModelEnum.ConfigParamType.DECIMAL:
                            dbConfig.DecValue = config.DecValue;
                            dbConfig.IntValue = 0;
                            dbConfig.StrValue = null;
                            dbConfig.DateValue = null;
                            break;
                        case (byte)ModelEnum.ConfigParamType.STRING:
                            dbConfig.StrValue = config.StrValue;
                            dbConfig.IntValue = 0;
                            dbConfig.DecValue = 0;
                            dbConfig.DateValue = null;
                            break;
                        case (byte)ModelEnum.ConfigParamType.DATETIME:
                            dbConfig.StrValue = null;
                            dbConfig.IntValue = 0;
                            dbConfig.DecValue = 0;
                            dbConfig.DateValue = config.DateValue;
                            break;
                    }
                    dbEntity.Entry(dbConfig).State = EntityState.Modified;
                    dbEntity.SaveChanges();
                    return Json(GridResponse.Create(true), JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
                }
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }

        /// <summary>
        /// 得到枚举的下拉表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetConfigTypes()
        {
            StringBuilder sb = new StringBuilder();
            List<ListItem> list = new GeneralConfig().ConfigTypeList;
            // WITHOUT THIS ROW, THE FIRST CITY FAILS IN THE COMBO FOR UNKNOWN REASONS!
            //sb.Append(Guid.Empty.ToString() + ":(none);");
            foreach (ListItem configType in list)
                sb.Append(configType.Value + ":" + configType.Text + ";");
            return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
        }
        #endregion Config

        #region MeasureUnitList
        /// <summary>
        /// MeasureUnit页面Action
        /// </summary>
        /// <returns>返回View</returns>
        public ActionResult MeasureUnit()
        {
            // 权限验证
            string strProgramCode = Request.RequestContext.RouteData.Values["Controller"].ToString() +
                Request.RequestContext.RouteData.Values["Action"].ToString();
            if (!base.Permission(strProgramCode))
                return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
            ViewBag.EnableEdit = (base.GetProgramNode("EnableEdit") == "1") ? true : false;

            ViewBag.unitSelectList = base.GetSelectList((new GeneralMeasureUnit()).TypeList);
            mtype = 0;
            return View();
        }

        /// <summary>
        /// 计量单位列表
        /// </summary>
        /// <returns></returns>
        public ActionResult MeasureUnitListTable()
        {
            return PartialView();
        }

        public void ChangeUnitType(byte uType)
        {
            mtype = uType;
        }

        /// <summary>
        /// MeasureList函数
        /// </summary>
        /// <param name="searchModel">searchModel</param>
        /// <returns>Json数据</returns>
        public ActionResult MeasureUnitList(SearchModel searchModel)
        {
            try
            {
                IQueryable<GeneralMeasureUnit> units = dbEntity.GeneralMeasureUnits.Include("Name").Where(u => u.Deleted == false && u.Utype == mtype).AsQueryable();
                GridColumnModelList<GeneralMeasureUnit> columns = new GridColumnModelList<GeneralMeasureUnit>();
                columns.Add(p => p.Gid).SetAsPrimaryKey();
                columns.Add(p => p.TypeName);
                columns.Add(p => p.Code);
                columns.Add(p => p.Name.GetResource(CurrentSession.Culture)).SetName("Name");
                columns.Add(p => p.Remark);
                GridData gridData = units.ToGridData(searchModel, columns);
                return Json(gridData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
            }
        }

        /// <summary>
        /// 添加的计量单位
        /// </summary>
        /// <param name="oMeasureUnit"></param>
        /// <returns></returns>
        // public ActionResult MeasureUnitAdd(GeneralMeasureUnit oMeasureUnit)
        public ActionResult MeasureUnitAdd()
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                try
                {
                    Guid orgGid = GetOrganization();
                    GeneralMeasureUnit oUnit = new GeneralMeasureUnit
                    {
                        Name = NewResource(ModelEnum.ResourceType.STRING, orgGid),
                        Utype = mtype
                    };
                    ViewBag.UnitTypeList = base.GetSelectList(oUnit.TypeList);
                    return View("MeasureUnitEdit", oUnit);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
                }
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }

        /// <summary>
        /// 编辑计量单位的页面
        /// </summary>
        /// <param name="uGid">页面选中行的Gid</param>
        /// <returns></returns>
        public ActionResult MeasureUnitEdit(Guid uGid)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                try
                {
                    Guid orgGid = GetOrganization();
                    GeneralMeasureUnit oUnit = dbEntity.GeneralMeasureUnits.Include("Name").Where(u => u.Gid == uGid).Single();
                    oUnit.Name = RefreshResource(ModelEnum.ResourceType.STRING, oUnit.Name, orgGid);
                    ViewBag.UnitTypeList = base.GetSelectList(oUnit.TypeList);
                    return View("MeasureUnitEdit", oUnit);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
                }
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }

        /// <summary>
        /// 保存添加的计量单位
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult MeasureUnitSave(GeneralMeasureUnit model)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                try
                {
                    GeneralMeasureUnit oUnit = (from u in dbEntity.GeneralMeasureUnits
                                                where u.Utype == model.Utype && u.Code == model.Code
                                                select u).FirstOrDefault();
                    if (oUnit == null)
                    {
                        oUnit = new GeneralMeasureUnit();
                        oUnit.Name = new GeneralResource(ModelEnum.ResourceType.STRING, model.Name);
                        dbEntity.GeneralMeasureUnits.Add(oUnit);
                    }
                    else
                    {
                        oUnit.Name.SetResource(ModelEnum.ResourceType.STRING, model.Name);
                    }
                    oUnit.Utype = model.Utype;
                    oUnit.Code = model.Code;
                    dbEntity.SaveChanges();
                    return RedirectToAction("MeasureUnitListTable");
                }
                catch (Exception ex)
                {
                    return RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
                }
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }

        /// <summary>
        /// 删除计量单位
        /// </summary>
        /// <param name="unitGid">页面选中行的Gid</param>
        /// <returns></returns>
        public ActionResult MeasureUnitRemove(Guid unitGid)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                try
                {
                    GeneralMeasureUnit oUnit = dbEntity.GeneralMeasureUnits.Where(o => o.Gid == unitGid).Single();
                    oUnit.Deleted = true;
                    if (ModelState.IsValid)
                    {
                        dbEntity.Entry(oUnit).State = EntityState.Modified;
                        dbEntity.SaveChanges();
                    }
                    return RedirectToAction("MeasureUnitListTable");
                }
                catch (Exception ex)
                {
                    return RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
                }
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }

        /// <summary>
        /// 获取Utype在页面表格中的显示文本
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMeasureUnitTypes()
        {
            StringBuilder sb = new StringBuilder();
            List<ListItem> list = new GeneralMeasureUnit().TypeList;
            foreach (ListItem item in list)
                sb.Append(item.Value + ":" + item.Text + ";");
            return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
        }
        #endregion MeasureUnitList
        
        #region CultureUnit
        /// <summary>
        /// CultureUnit页面Action
        /// </summary>
        /// <returns>返回View</returns>
        public ActionResult Culture()
        {
            // 权限验证
            string strProgramCode = Request.RequestContext.RouteData.Values["Controller"].ToString() +
                Request.RequestContext.RouteData.Values["Action"].ToString();
            if (!base.Permission(strProgramCode))
                return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
            return View();
        }

        /// <summary>
        /// 语言文化列表页面
        /// </summary>
        /// <returns></returns>
        public ActionResult CultureListTable()
        {
            return PartialView();
        }

        public ActionResult ShowCultureName(int cultureId)
        {
            try
            {
                ViewBag.cultureName = new CultureInfo(cultureId).NativeName;
                ViewBag.CheckedResult = 0;
            }
            catch
            {
                ViewBag.CultureName = cultureId + " " + LiveAzure.Resource.Stage.ConfigController.ExistOrNot;
                ViewBag.CheckedResult = 1;
            }
            return PartialView();
        }

        /// <summary>
        /// CultureList函数
        /// </summary>
        /// <param name="searchModel">searchModel</param>
        /// <returns>Json数据</returns>
        public ActionResult CultureList(SearchModel searchModel)
        {
            IQueryable<GeneralCultureUnit> cultures = dbEntity.GeneralCultureUnits.Where(c => c.Deleted == false).AsQueryable();
            GridColumnModelList<GeneralCultureUnit> columns = new GridColumnModelList<GeneralCultureUnit>();
            columns.Add(c => c.Gid).SetAsPrimaryKey();
            columns.Add(c => (c.Culture == null) ? "" : c.CultureName).SetName("Culture");
            //columns.Add(c => (c.Culture == null) ? "" : new CultureInfo(c.Culture).NativeName).SetName("Culture");
            columns.Add(c => (c.Piece == null) ? "" : c.Piece.Name.GetResource(CurrentSession.Culture)).SetName("Piece");
            columns.Add(c => (c.Weight == null) ? "" : c.Weight.Name.GetResource(CurrentSession.Culture)).SetName("Weight");
            columns.Add(c => (c.Volume == null) ? "" : c.Volume.Name.GetResource(CurrentSession.Culture)).SetName("Volume");
            columns.Add(c => (c.Fluid == null) ? "" : c.Fluid.Name.GetResource(CurrentSession.Culture)).SetName("Fluid");
            columns.Add(c => (c.Area == null) ? "" : c.Area.Name.GetResource(CurrentSession.Culture)).SetName("Area");
            columns.Add(c => (c.Linear == null) ? "" : c.Linear.Name.GetResource(CurrentSession.Culture)).SetName("Linear");
            columns.Add(c => (c.Currency == null) ? "" : c.Currency.Name.GetResource(CurrentSession.Culture)).SetName("Currency");
            GridData gridData = cultures.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加的语言文化
        /// </summary>
        /// <returns></returns>
        public ActionResult CultureAdd()
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                try
                {
                    GeneralCultureUnit oCulture = new GeneralCultureUnit();
                    ViewBag.PieceList = GetUnitDropDownList((byte)ModelEnum.MeasureUnit.PIECE);
                    ViewBag.WeightList = GetUnitDropDownList((byte)ModelEnum.MeasureUnit.WEIGHT);
                    ViewBag.VolumeList = GetUnitDropDownList((byte)ModelEnum.MeasureUnit.VOLUME);
                    ViewBag.FluidList = GetUnitDropDownList((byte)ModelEnum.MeasureUnit.FLUID);
                    ViewBag.AreaList = GetUnitDropDownList((byte)ModelEnum.MeasureUnit.AREA);
                    ViewBag.LinearList = GetUnitDropDownList((byte)ModelEnum.MeasureUnit.LINEAR);
                    ViewBag.CurrencyList = GetUnitDropDownList((byte)ModelEnum.MeasureUnit.CURRENCY);
                    return View("CultureEdit", oCulture);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
                }
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }

        /// <summary>
        /// 编辑语言文化页面
        /// </summary>
        /// <param name="unitGid">页面选中行的GID</param>
        /// <returns></returns>
        public ActionResult CultureEdit(Guid unitGid)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                try
                {
                    GeneralCultureUnit oCulture = dbEntity.GeneralCultureUnits
                        .Include("Piece").Include("Weight").Include("Volume").Include("Fluid").Include("Area").Include("Linear").Include("Currency")
                        .Where(g => g.Gid == unitGid).Single();
                    ViewBag.PieceList = GetUnitDropDownList((byte)ModelEnum.MeasureUnit.PIECE);
                    ViewBag.WeightList = GetUnitDropDownList((byte)ModelEnum.MeasureUnit.WEIGHT);
                    ViewBag.VolumeList = GetUnitDropDownList((byte)ModelEnum.MeasureUnit.VOLUME);
                    ViewBag.FluidList = GetUnitDropDownList((byte)ModelEnum.MeasureUnit.FLUID);
                    ViewBag.AreaList = GetUnitDropDownList((byte)ModelEnum.MeasureUnit.AREA);
                    ViewBag.LinearList = GetUnitDropDownList((byte)ModelEnum.MeasureUnit.LINEAR);
                    ViewBag.CurrencyList = GetUnitDropDownList((byte)ModelEnum.MeasureUnit.CURRENCY);
                    return View("CultureEdit", oCulture);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
                }
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }

        /// <summary>
        /// 保存添加的语言文化
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult CultureSave(GeneralCultureUnit model)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                try
                {
                    GeneralCultureUnit oCulture = (from c in dbEntity.GeneralCultureUnits.Include("Piece").Include("Weight").Include("Volume").Include("Fluid").Include("Area").Include("Linear").Include("Currency")
                                                   where c.Culture == model.Culture
                                                   select c).FirstOrDefault();
                    if (oCulture == null)
                    {
                        oCulture = new GeneralCultureUnit();
                        dbEntity.GeneralCultureUnits.Add(oCulture);
                    }
                    oCulture.Deleted = false;     // 恢复删除的项
                    oCulture.Culture = model.Culture;
                    oCulture.aPiece = model.aPiece;
                    oCulture.aWeight = model.aWeight;
                    oCulture.aVolume = model.aVolume;
                    oCulture.aFluid = model.aFluid;
                    oCulture.aArea = model.aArea;
                    oCulture.aLinear = model.aLinear;
                    oCulture.aCurrency = model.aCurrency;
                    oCulture.Remark = model.Remark;
                    dbEntity.SaveChanges();
                    return RedirectToAction("CultureListTable");
                }
                catch (Exception ex)
                {
                    return RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
                }
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }

        /// <summary>
        /// 删除语言文化
        /// </summary>
        /// <param name="uGid"></param>
        /// <returns></returns>
        public ActionResult CultureRemove(Guid uGid)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                try
                {
                    GeneralCultureUnit oCultureUnit = dbEntity.GeneralCultureUnits.Where(u => u.Gid == uGid).Single();
                    oCultureUnit.Deleted = true;
                    if (ModelState.IsValid)
                    {
                        dbEntity.Entry(oCultureUnit).State = EntityState.Modified;
                        dbEntity.SaveChanges();
                    }
                    return RedirectToAction("CultureListTable");
                }
                catch (Exception ex)
                {
                    return RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
                }
            }
            return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
        }

        /// <summary>
        /// 获取计量单位的下拉框列表
        /// </summary>
        /// <param name="unitType">计量单位类型</param>
        /// <returns></returns>
        public List<SelectListItem> GetUnitDropDownList(byte unitType)
        {
            List<SelectListItem> unitList = new List<SelectListItem>();
            try
            {
                var oUnitList = dbEntity.GeneralMeasureUnits.Where(p => p.Utype == unitType && p.Deleted == false).ToList();
                foreach (GeneralMeasureUnit item in oUnitList)
                    unitList.Add(new SelectListItem { Value = item.Gid.ToString(), Text = item.Name.GetResource(CurrentSession.Culture) });
            }
            catch { }
            return unitList;
        }

        #endregion CultureUnit
        
        #region ActionList
        /// <summary>
        /// Action页面Action
        /// </summary>
        /// <returns>返回View</returns>
        public ActionResult Action()
        {
            // 权限验证
            string strProgramCode = Request.RequestContext.RouteData.Values["Controller"].ToString() +
                Request.RequestContext.RouteData.Values["Action"].ToString();
            if (!base.Permission(strProgramCode))
                return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
            return View();
        }
        /// <summary>
        /// ActionList函数
        /// </summary>
        /// <param name="seachModel">searchModel</param>
        /// <returns>返回Json数据</returns>
        public ActionResult ActionList(SearchModel seachModel)
        {
            try
            {
                IQueryable<GeneralAction> actions = dbEntity.GeneralActions.AsQueryable();
                GridColumnModelList<GeneralAction> columns = new GridColumnModelList<GeneralAction>();
                columns.Add(p => p.Gid).SetAsPrimaryKey();
                columns.Add(p => p.ActID);
                columns.Add(p => p.ActionSourceName);
                columns.Add(p => p.ClassName);
                columns.Add(p => p.RefTypeName);
                columns.Add(p => p.RefID);
                columns.Add(p => p.ActionLevelName);
                columns.Add(p => p.Matter);
                columns.Add(p => p.Keyword);
                GridData gridData = actions.ToGridData(seachModel, columns);
                return Json(gridData, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Error");
            }
        }
        #endregion ActionList
        
        #region Message
        /// <summary>
        /// Message 页面的Action
        /// </summary>
        /// <returns>Message</returns>
        public ActionResult Message()
        {
            return View();
        }
        #region MessagePending
        /// <summary>
        /// MessagePending 的Action
        /// </summary>
        /// <returns>MessagePending的部分页面</returns>
        public ActionResult MessagePending()
        {
            return PartialView();
        }
        /// <summary>
        /// MessagePendingList函数
        /// </summary>
        /// <returns>MessagePending的Json数据</returns>
        public ActionResult MessagePendingList(SearchModel searchModel)
        {
            try
            {
                CultureInfo culture = new CultureInfo(CurrentSession.Culture);
                IQueryable<GeneralMessagePending> messagePending = dbEntity.GeneralMessagePendings.AsQueryable();
                GridColumnModelList<GeneralMessagePending> columns = new GridColumnModelList<GeneralMessagePending>();
                columns.Add(p => p.Gid).SetAsPrimaryKey();
                columns.Add(p => p.User.DisplayName).SetName("User");
                columns.Add(p => p.MessageTypeName);
                columns.Add(p => p.MessageStatusName);
                columns.Add(p => p.Name);
                columns.Add(p => p.Recipient);
                columns.Add(p => p.Matter);
                columns.Add(p => p.RefType);
                columns.Add(p => p.RefID);
                columns.Add(p => p.Schedule);
                columns.Add(p => ((p.SentTime==null)? p.SentTime.ToString():((DateTimeOffset)p.SentTime).ToString(culture.DateTimeFormat)));
                GridData gridData = messagePending.ToGridData(searchModel, columns);
                return Json(gridData, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Error");
            }
        }
        #endregion MessagePending
        #region MessageReceive
        /// <summary>
        /// MessageReceive 的页面Action
        /// </summary>
        /// <returns>MessageReceive部分页面</returns>
        public ActionResult MessageReceive()
        {
            return PartialView();
        }
        /// <summary>
        /// MessageReceiveLis函数
        /// </summary>
        /// <param name="searchModel">SearchModel</param>
        /// <returns>返回MessageReceive的Json数据</returns>
        public ActionResult MessageReceiveList(SearchModel searchModel)
        {
            try
            {
                CultureInfo culture = new CultureInfo(CurrentSession.Culture);
                IQueryable<GeneralMessageReceive> messageReceives = dbEntity.GeneralMessageReceives.AsQueryable();
                GridColumnModelList<GeneralMessageReceive> columns = new GridColumnModelList<GeneralMessageReceive>();
                columns.Add(p => p.Gid).SetAsPrimaryKey();
                columns.Add(p => p.SendFrom);
                columns.Add(p => p.Matter);
                columns.Add(p => ((p.SentTime == null)? p.SentTime.ToString():((DateTimeOffset)p.SentTime).ToString(culture.DateTimeFormat)));
                columns.Add(p => p.GetFrom);
                GridData gridData = messageReceives.ToGridData(searchModel, columns);
                return Json(gridData, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Error");
            }
        }
        #endregion MessageReceive
        #region MessageTemplate
        /// <summary>
        /// 消息模板部分页面Action
        /// </summary>
        /// <returns>消息模板部分页面</returns>
        public ActionResult MessageTemplate()
        {
            return PartialView();
        }
        /// <summary>
        /// 消息模板List
        /// </summary>
        /// <param name="searchModel">SearchModel</param>
        /// <returns>Json数据</returns>
        public ActionResult MessageTemplateList(SearchModel searchModel)
        {
            try
            {
                IQueryable<GeneralMessageTemplate> templates = dbEntity.GeneralMessageTemplates.AsQueryable();
                GridColumnModelList<GeneralMessageTemplate> columns = new GridColumnModelList<GeneralMessageTemplate>();
                columns.Add(p => p.Gid).SetAsPrimaryKey();
                columns.Add(p => p.Code);
                columns.Add(p => p.Organization.FullName.GetResource(CurrentSession.Culture)).SetName("Organization");
                columns.Add(p => p.Name.GetResource(CurrentSession.Culture)).SetName("Name");
                GridData gridData = templates.ToGridData(searchModel, columns);
                return Json(gridData, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Error");
            }
        }
        /// <summary>
        /// 消息模板编辑函数页面Action
        /// </summary>
        /// <param name="gid">消息模板的Gid</param>
        /// <returns>消息模板编辑页面</returns>
        public ActionResult MessageTemplateEdit(Guid gid, bool? view)
        {
            if (gid == null)
            {
                RedirectToAction("Error");
            }
            Guid orgGid = dbEntity.MemberUsers.Find(CurrentSession.UserID).OrgID;
            GeneralMessageTemplate template = dbEntity.GeneralMessageTemplates.Single(item=>item.Gid == gid);
            template.Matter = RefreshLargeObject(template.Matter,orgGid);
            //GeneralLargeObject largeObject = template.Matter;
            if(view != null) ViewBag.view = view.ToString();
            return View(template);
        }

        /// <summary>
        /// 消息模板编辑提交函数
        /// </summary>
        /// <param name="largeObject">页面Model</param>
        /// <param name="gid">页面Model的Gid</param>
        /// <returns>Message</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult MessageTemplateEditPost(GeneralMessageTemplate template,Guid gid)
        {
            try
            {
                //GeneralLargeObject o = dbEntity.GeneralLargeObjects.Single(item => item.Gid == gid);
                //o.CLOB.SetLargeObject(o);
                //o.CLOB = largeObject.CLOB;
                GeneralMessageTemplate oTemplate = dbEntity.GeneralMessageTemplates.Single(item => item.Gid == template.Gid);
                oTemplate.Matter.SetLargeObject(template.Matter);
                dbEntity.Entry(oTemplate).State = EntityState.Modified;
                dbEntity.SaveChanges();
                return RedirectToAction("Message");
            }
            catch
            {
                return RedirectToAction("Error");
            }
        }
        #endregion MessageTemplate
        #endregion Message

        #region Shortcut
        /// <summary>
        /// 获取用户ID
        /// </summary>
        /// <returns></returns>
        public Guid GetUserID()
        {
            MemberUser oUser = dbEntity.MemberUsers.Find(CurrentSession.UserID);
            return (oUser.Gid);
        }

        /// <summary>
        /// 用户快捷键页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Shortcut()
        {
            string strProgramCode = Request.RequestContext.RouteData.Values["Controller"].ToString() +
                Request.RequestContext.RouteData.Values["Action"].ToString();
            if (!base.Permission(strProgramCode))
                return RedirectToAction("ErrorPage", "Home", new { message = LiveAzure.Resource.Common.NoPermission });
            
            return View();
        }

        /// <summary>
        /// 网站链接快捷键列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult UrlShortcut()
        {
            ViewBag.EnableEdit = (base.GetProgramNode("EnableEdit") == "1") ? true : false;
            return View();
        }

        /// <summary>
        /// 系统程序快捷键列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult ProgramShortcut()
        {
            ViewBag.EnableEdit = (base.GetProgramNode("EnableEdit") == "1") ? true : false;
            return View();
        }

        public ActionResult AddProgramShortcut()
        {
            //ViewBag.ProgramDropdownList = GetProgramList();
            return PartialView();
        }

        public ActionResult AddUrlShortcut()
        {
            return PartialView();
        }

        public ActionResult EditProgramShortcut(Guid shortcutGid)
        {
            MemberUserShortcut oShortcut = dbEntity.MemberUserShortcuts.Include("Program").Where(s => s.Gid == shortcutGid).Single();
            return PartialView(oShortcut);
        }

        public ActionResult EditUrlShortcut(Guid shortcutGid)
        {
            MemberUserShortcut oShortcut = dbEntity.MemberUserShortcuts.Include("Program").Where(s => s.Gid == shortcutGid).Single();
            return PartialView(oShortcut);
        }

        /// <summary>
        /// 网站链接快捷键列表的GridList
        /// </summary>
        /// <param name="shortSearchModel"></param>
        /// <returns></returns>
        public ActionResult UrlShortcutList(SearchModel shortSearchModel)
        {
            Guid userID = GetUserID();
            var memberUserShortcutUrl = dbEntity.MemberUserShortcuts.Where(m => m.Deleted == false && m.Stype == 1 && m.UserID == userID).AsQueryable();

            GridColumnModelList<MemberUserShortcut> columns = new GridColumnModelList<MemberUserShortcut>();
            columns.Add(m => m.Gid).SetAsPrimaryKey();
            columns.Add(m => m.LinkUrl);
            columns.Add(m => m.Sorting);
            columns.Add(m => m.Icon);

            GridData shortcutGridData = memberUserShortcutUrl.ToGridData(shortSearchModel, columns);
            return Json(shortcutGridData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 系统程序快捷键列表的GridList
        /// </summary>
        /// <param name="shortSearchModel"></param>
        /// <returns></returns>
        public ActionResult ProgramShortcutList(SearchModel shortSearchModel)
        {
            Guid userID = GetUserID();
            var memberUserShortcutUrl = dbEntity.MemberUserShortcuts.Where(m => m.Deleted == false && m.Stype == 0 && m.UserID == userID).AsQueryable();

            GridColumnModelList<MemberUserShortcut> columns = new GridColumnModelList<MemberUserShortcut>();
            columns.Add(m => m.Gid).SetAsPrimaryKey();
            columns.Add(m => m.ProgID);
            columns.Add(m => m.Sorting);
            columns.Add(m => m.Icon);

            GridData shortcutGridData = memberUserShortcutUrl.ToGridData(shortSearchModel, columns);
            return Json(shortcutGridData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 增加快捷键
        /// </summary>
        /// <param name="newShortcut">页面传回的新增快捷键对象</param>
        public void AddShortcut(MemberUserShortcut newShortcut)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                Guid userID = GetUserID();

                //查找用户曾使用过、但被删除的快捷键
                MemberUserShortcut oldShortcut = dbEntity.MemberUserShortcuts.Include("Program").Where(o => o.ProgID == newShortcut.ProgID && o.UserID == userID).SingleOrDefault();

                //如果存在，则将Delete设回false并更新，如果不存在，则保存一个新的MemberUserShortcut对象
                if (oldShortcut != null)
                {
                    oldShortcut.Deleted = false;
                    oldShortcut.Sorting = newShortcut.Sorting;
                    oldShortcut.Icon = newShortcut.Icon;
                    oldShortcut.ProgID = newShortcut.ProgID;
                    oldShortcut.Remark = newShortcut.Remark;
                    if (ModelState.IsValid)
                    {
                        dbEntity.Entry(oldShortcut).State = EntityState.Modified;
                        dbEntity.SaveChanges();
                    }
                }

                else
                {
                    newShortcut.UserID = userID;

                    if (newShortcut.ProgID == null)     //判断传回的SType是URL还是Program
                    {
                        newShortcut.Stype = 1;
                        dbEntity.MemberUserShortcuts.Add(newShortcut);
                        dbEntity.SaveChanges();
                    }
                    else
                    {
                        newShortcut.Stype = 0;
                        dbEntity.MemberUserShortcuts.Add(newShortcut);
                        dbEntity.SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// 编辑用户快捷键
        /// </summary>
        /// <param name="newShortcut"></param>
        /// <returns></returns>
        public void SaveEditShortcut(MemberUserShortcut newShortcut)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                MemberUserShortcut oldShortcut = dbEntity.MemberUserShortcuts.Include("Program").Where(g => g.Gid == newShortcut.Gid).Single();

                if (newShortcut.Icon != null)
                {
                    oldShortcut.LinkUrl = newShortcut.LinkUrl;
                }
                if (newShortcut.ProgID != Guid.Empty)
                {
                    oldShortcut.ProgID = newShortcut.ProgID;
                }
                oldShortcut.Deleted = false;
                oldShortcut.Sorting = newShortcut.Sorting;
                oldShortcut.Icon = newShortcut.Icon;
                oldShortcut.Remark = newShortcut.Remark;

                if (ModelState.IsValid)
                {
                    dbEntity.Entry(oldShortcut).State = EntityState.Modified;
                    dbEntity.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 移除用户快捷键
        /// </summary>
        /// <param name="shortcutGid"></param>
        /// <returns></returns>
        public void RemoveShortcut(Guid shortcutGid)
        {
            if (base.GetProgramNode("EnableEdit") == "1")
            {
                MemberUserShortcut otherShortcut = dbEntity.MemberUserShortcuts.Where(g => g.Gid == shortcutGid).Single();
                otherShortcut.Deleted = true;
                dbEntity.Entry(otherShortcut).State = EntityState.Modified;
                dbEntity.SaveChanges();
            }
        }

        /// <summary>
        /// 将Grid的ProgID列的显示内容设为Program的Name
        /// </summary>
        /// <returns></returns>
        public ActionResult GetProgramText()
        {
            Guid userID = GetUserID();
            var programList = dbEntity.GeneralPrograms.Where(p => p.Deleted == false).ToList();
            //var memberUserShortcutUrl = dbEntity.MemberUserShortcuts.Where(u => u.UserID == userID).ToList();
            List<ListItem> list = new List<ListItem>();
            foreach (var item in programList)
            {
                list.Add(new ListItem { Text = item.Name.GetResource(CurrentSession.Culture), Value = item.Gid.ToString() });
            }

            StringBuilder sb = new StringBuilder();

            foreach (ListItem shortcut in list)
            {
                sb.Append(shortcut.Value + ":" + shortcut.Text + ";");
            }
            
            return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 增加系统程序快捷键时的程序下拉框列表
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetProgramList()
        {
            Guid userID = GetUserID();
            var UserSelectedPrograms = from c in dbEntity.MemberUserShortcuts.Include("Program")
                                       where c.Deleted == false && c.UserID == userID
                                       select c.Program;
            var UserPrograms = dbEntity.GeneralPrograms.ToList();
            var UserSelectList = UserPrograms.Except(UserSelectedPrograms).ToList();
            List<SelectListItem> UserProgramList = new List<SelectListItem>();
            foreach (GeneralProgram ProgramItem in UserSelectList)
            {
                UserProgramList.Add(new SelectListItem { Text = ProgramItem.Name.GetResource(CurrentSession.Culture), Value = ProgramItem.Gid.ToString() });
            }
            return UserProgramList;
        }
        #endregion

        #region ErrorReport

        public ActionResult ErrorReport()
        {
            return View();
        }

        #endregion

        #region ChangePassword

        public ActionResult ChangePassword()
        {
            MemberUser oUser = dbEntity.MemberUsers.Find(CurrentSession.UserID);
            var saltkey = oUser.SaltKey;
            ViewBag.password = LiveAzure.Utility.CommonHelper.DecryptDES(oUser.Passcode, saltkey);
            return View(oUser);
        }

        public void SaveChangedPassword(MemberUser oUser,string newPassword)
        {
            MemberUser oldUser = dbEntity.MemberUsers.Single(o => o.Gid == oUser.Gid);
            oldUser.Passcode = newPassword;

            dbEntity.Entry(oldUser).State = EntityState.Modified;
            dbEntity.SaveChanges();
        }

        #endregion
    }
}
