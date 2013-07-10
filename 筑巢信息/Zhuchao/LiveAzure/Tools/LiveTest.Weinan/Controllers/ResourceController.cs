using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using LiveAzure.Models;
using LiveAzure.Models.General;
using LiveAzure.Utility;
using MVC.Controls;
using MVC.Controls.Grid;
using LiveAzure.BLL;

namespace LiveTest.Weinan.Controllers
{
    public class ResourceController : BaseController
    {
        private static Guid? resGid;

        #region 权限
        public static string strSaltKey = CommonHelper.RandomNumber(8);
        #endregion 权限

        #region 加密解密参数
        /// <summary>
        /// 参数分隔符
        /// </summary>
        public const char cSeparator = ',';
        /// <summary>
        /// 加密资源编辑器参数
        /// </summary>
        /// <param name="sResId">资源Guid</param>
        /// <param name="sSaltKey">8位加密密钥</param>
        /// <param name="bAddButton">允许添加</param>
        /// <param name="bEditButton">允许修改</param>
        /// <param name="bDeleteButton">允许删除</param>
        /// <returns>加密后的参数</returns>
        public static string EncryptResourceEditorParam(string sResId, string sSaltKey,
            bool bAddButton = false, bool bEditButton = false, bool bDeleteButton = false)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(sResId);
            strBuilder.Append(cSeparator);
            strBuilder.Append(bAddButton ? 1 : 0);
            strBuilder.Append(cSeparator);
            strBuilder.Append(bEditButton ? 1 : 0);
            strBuilder.Append(cSeparator);
            strBuilder.Append(bDeleteButton ? 1 : 0);
            string sOriginalParam = strBuilder.ToString();
            string sEncryptedParam = CommonHelper.EncryptDES(sOriginalParam, sSaltKey );
            return sEncryptedParam;
        }

        /// <summary>
        /// 解密资源编辑器参数
        /// </summary>
        /// <param name="sEncryptedParam">加密后的参数</param>
        /// <param name="sSaltKey">8位加密密钥</param>
        /// <param name="sResId">用于输出资源Guid的变量</param>
        /// <param name="bAddButton">用于输出允许添加的变量</param>
        /// <param name="bEditButton">用于输出允许修改的变量</param>
        /// <param name="bDeleteButton">用于输出允许删除的变量</param>
        /// <returns>是否成功</returns>
        public static bool DecryptResourceEditorParam(string sEncryptedParam, string sSaltKey,
            out string sResId, out bool bAddButton, out bool bEditButton, out bool bDeleteButton)
        {
            string sOriginalParam = CommonHelper.DecryptDES(sEncryptedParam, sSaltKey);
            string[] parameters = sOriginalParam.Split(new char[] { cSeparator });

            //Guid解密后长度不对即认为解密失败，以失败论处
            //此处可能还需要更加精确的处理，暂时略过
            if (parameters[0].Length != 36)
            {
                sResId = null;
                bAddButton = false;
                bEditButton = false;
                bDeleteButton = false;
                return false;
                //解密失败
            }
            else
            {
                sResId = parameters[0];
                bAddButton = (parameters[1] == "1");
                bEditButton = (parameters[2] == "1");
                bDeleteButton = (parameters[3] == "1");
                return true;
                //解密成功
            }
        }

        #endregion 加密解密参数

        /// <summary>
        /// 首页，呈现一些测试方法，最终将被舍弃
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        #region MatterEditor

        /// <summary>
        /// 测试方法，最终将被舍弃
        /// </summary>
        /// <returns></returns>
        public ActionResult TestEditMatter()
        {
            GeneralResource res = new GeneralResource
            {
                Rtype = (byte)ModelEnum.ResourceType.STRING,
                Culture = 2052,
                Matter = "我是中文名字"
            };
            GeneralResItem resitem1 = new GeneralResItem
            {
                Culture = 1036,
                Matter = "我是法国名字（PS:我不会法文，囧）",
                Resource = res
            };
            GeneralResItem resitem2 = new GeneralResItem
            {
                Culture = 1031,
                Matter = "It's an English name.",
                Resource = res
            };
            dbEntity.GeneralResources.Add(res);
            dbEntity.GeneralResItems.Add(resitem1);
            dbEntity.GeneralResItems.Add(resitem2);
            dbEntity.SaveChanges();

            return View("TestEditMatter", res);
        }

        /// <summary>
        /// 弹出编辑多语言资源窗口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult EditMatter(string param)
        {
            string resId;
            bool bEnableAdd = false;
            bool bEnableEdit = false;
            bool bEnableDelete = false;
            bool m = DecryptResourceEditorParam(param, strSaltKey, out resId, out bEnableAdd, out bEnableEdit, out bEnableDelete);
            if (m)
            {
                resGid = new Guid(resId);
                GeneralResource resource = (from i in dbEntity.GeneralResources.Include("ResourceItems")
                                            where i.Gid == resGid
                                               && i.Deleted == false
                                            select i).Single();
                ViewBag.CultureSelectList = GetAvailableCultureSelectList();
                ViewBag.bEnableAdd = bEnableAdd;
                ViewBag.bEnableEdit = bEnableEdit;
                ViewBag.bEnableDelete = bEnableDelete;
                return View("MatterEditor", resource);
            }
            else
            {
                return View("Error");
            }
        }

        public ActionResult Error()
        {
            return View();
        }
        /// <summary>
        /// 用于列举多语言资源编辑窗口的列表项
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public JsonResult ListMatter(SearchModel searchModel)
        {
            GeneralResource resource = (from i in dbEntity.GeneralResources.Include("ResourceItems")
                                        where i.Gid == resGid
                                           && i.Deleted == false
                                        select i).Single();
            List<GeneralResItem> items = (from i in resource.ResourceItems
                                          where i.Deleted == false
                                          select i).ToList();
            var model = items.AsQueryable();
            GridData data = model.ToGridData(searchModel, Columns.MatterColumns);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 用于编辑
        /// </summary>
        /// <param name="newModel"></param>
        /// <returns></returns>
        public JsonResult UpdateMatter(GeneralResItem newModel)
        {
            if (ModelState.IsValid == false)
            {
                return Json(GridResponse.Create(ModelState), JsonRequestBehavior.AllowGet);
            }

            GeneralResItem item = (from i in dbEntity.GeneralResItems
                                   where i.Gid == newModel.Gid
                                      && i.Deleted == false
                                   select i).Single();
            if (item != null)
            {
                item.Matter = newModel.Matter;
                dbEntity.SaveChanges();
            }
            return Json(GridResponse.Create(true), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddMatter()
        {
            int newCultureCode = Convert.ToInt32(Request.Form["NewCulture"]);
            GeneralResItem oldItem = (from i in dbEntity.GeneralResItems
                                where i.ResID == resGid
                                   && i.Culture == newCultureCode
                                   && i.Deleted == false
                                select i).FirstOrDefault();
            if (oldItem == null)
            {
                GeneralResItem item = new GeneralResItem
                {
                    ResID = resGid.Value,
                    Culture = newCultureCode
                };
                dbEntity.GeneralResItems.Add(item);
                dbEntity.SaveChanges();
            }
            return RedirectToAction("EditMatter", new { param = Request.QueryString["param"]});
        }

        public JsonResult DeleteMatter(GeneralResItem deletedModel)
        {
            if (deletedModel.Gid != Guid.Empty)
            {
                GeneralResItem item = (from i in dbEntity.GeneralResItems
                                       where i.Gid == deletedModel.Gid
                                          && i.Deleted == false
                                       select i).FirstOrDefault();
                if (item != null)
                {
                    item.Deleted = true;
                    dbEntity.SaveChanges();
                }
            }
            return Json(GridResponse.CreateSuccess(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取多语言下拉框选择项列表
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetAvailableCultureSelectList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            int mainCulture = (from i in dbEntity.GeneralResources
                                        where i.Gid == resGid
                                           && i.Deleted == false
                                        select i.Culture).Single();
            IQueryable<int> selectedCultures = from i in dbEntity.GeneralResItems
                                               where i.Deleted == false
                                                  && i.ResID == resGid
                                               select i.Culture;
            IQueryable<int> allCultures = from c in dbEntity.GeneralCultureUnits
                                          where c.Deleted == false
                                          select c.Culture;
            IQueryable<int> availableCultures = from a in allCultures
                                                where !selectedCultures.Contains(a)
                                                   && a != mainCulture
                                                select a;
            foreach (int culture in availableCultures)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = new CultureInfo(culture).NativeName,
                    Value = culture.ToString()
                };
                list.Add(item);
            }
            return list;
        }

        #endregion MatterEditor

        #region CurrencyEditor

        /// <summary>
        /// 测试方法，最终将被舍弃
        /// </summary>
        /// <returns></returns>
        public ActionResult TestEditCash()
        {
            ///添加货币名称资源(人民币)
            GeneralResource cashName1_CN = new GeneralResource { Rtype = (byte)ModelEnum.ResourceType.STRING, Culture = 2052, Matter = "人民币" };
            dbEntity.GeneralResources.Add(cashName1_CN);
            GeneralResItem cashName1_EN = new GeneralResItem { Culture = 1033, Matter = "Chinese Yuan", Resource = cashName1_CN };
            dbEntity.GeneralResItems.Add(cashName1_EN);
            GeneralResItem cashName1_FR = new GeneralResItem { Culture = 1036, Matter = "~~~Chinese Yuan~~法语~", Resource = cashName1_CN };
            dbEntity.GeneralResItems.Add(cashName1_FR);
            dbEntity.SaveChanges();

            ///添加货币名称资源(美元)
            GeneralResource cashName2_CN = new GeneralResource { Rtype = (byte)ModelEnum.ResourceType.STRING, Culture = 2052, Matter = "美元" };
            dbEntity.GeneralResources.Add(cashName2_CN);
            GeneralResItem cashName2_EN = new GeneralResItem { Culture = 1033, Matter = "Dollar", Resource = cashName2_CN };
            dbEntity.GeneralResItems.Add(cashName2_EN);
            GeneralResItem cashName2_FR = new GeneralResItem { Culture = 1036, Matter = "~~~Dollar~~法语~", Resource = cashName2_CN };
            dbEntity.GeneralResItems.Add(cashName2_FR);
            dbEntity.SaveChanges();


            //添加货币单位
            GeneralMeasureUnit unitCNY = new GeneralMeasureUnit
            {
                Utype = (byte)ModelEnum.MeasureUnit.CURRENCY,
                Code = "CNYdcced5",
                Name = cashName1_CN
            };
            dbEntity.GeneralMeasureUnits.Add(unitCNY);
            GeneralMeasureUnit unitUSD = new GeneralMeasureUnit
            {
                Utype = (byte)ModelEnum.MeasureUnit.CURRENCY,
                Code = "USDd5555",
                Name = cashName2_CN
            };
            dbEntity.GeneralMeasureUnits.Add(unitUSD);
            dbEntity.SaveChanges();

            //添加金额资源
            GeneralResource res = new GeneralResource
            {
                Rtype = (byte)ModelEnum.ResourceType.MONEY,
                Currency = unitCNY.Gid,
                Cash = 10.0m
            };
            GeneralResItem resItem = new GeneralResItem
            {
                Resource = res,
                Currency = unitUSD.Gid
            };
            dbEntity.GeneralResources.Add(res);
            dbEntity.GeneralResItems.Add(resItem);
            dbEntity.SaveChanges();

            return View("TestEditCash", res);
        }

        public ActionResult EditCash(string param)
        {
            string resId;
            bool bEnableAdd = false;
            bool bEnableEdit = false;
            bool bEnableDelete = false;
            bool m = DecryptResourceEditorParam(param, strSaltKey, out resId, out bEnableAdd, out bEnableEdit, out bEnableDelete);
            if (m)
            {
                resGid = new Guid(resId);
                GeneralResource resource = (from i in dbEntity.GeneralResources.Include("ResourceItems")
                                            where i.Gid == resGid
                                               && i.Deleted == false
                                            select i).Single();
                GeneralMeasureUnit unit = (from u in dbEntity.GeneralMeasureUnits
                                           where u.Gid == resource.Currency
                                              && u.Deleted == false
                                           select u).Single();
                ViewBag.MainCurrencyName = GetMatterValue(unit.Name);
                //ViewBag.CurrencySelectList = GetAvailableCurrencySelectList();
                ViewBag.CurrencySelectList = new List<SelectListItem>();
                ViewBag.bEnableAdd = bEnableAdd;
                ViewBag.bEnableEdit = bEnableEdit;
                ViewBag.bEnableDelete = bEnableDelete;
                return View("CashEditor", resource);
            }
            else
            {
                return View("Error");
            }
        }

        /// <summary>
        /// 用于列举多货币资源编辑窗口的列表项
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public JsonResult ListCash(SearchModel searchModel)
        {
            GeneralResource resource = (from i in dbEntity.GeneralResources.Include("ResourceItems")
                                        where i.Gid == resGid
                                           && i.Deleted == false
                                        select i).Single();
            List<GeneralResItem> items = (from i in resource.ResourceItems
                                          where i.Deleted == false
                                          select i).ToList();
            var model = items.AsQueryable();
            GridData data = model.ToGridData(searchModel, Columns.CashColumns);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 用于编辑
        /// </summary>
        /// <param name="newModel"></param>
        /// <returns></returns>
        public JsonResult UpdateCash(GeneralResItem newModel)
        {
            if (ModelState.IsValid == false)
            {
                return Json(GridResponse.Create(ModelState), JsonRequestBehavior.AllowGet);
            }

            GeneralResItem item = (from i in dbEntity.GeneralResItems
                                   where i.Gid == newModel.Gid
                                      && i.Deleted == false
                                   select i).Single();
            if (item != null)
            {
                item.Cash = newModel.Cash;
                dbEntity.SaveChanges();
            }
            return Json(GridResponse.Create(true), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddCash()
        {
            Guid newCurrencyID = new Guid(Request.Form["NewCurrency"]);
            GeneralResItem oldItem = (from i in dbEntity.GeneralResItems
                                      where i.ResID == resGid
                                         && i.Currency == newCurrencyID
                                         && i.Deleted == false
                                      select i).FirstOrDefault();
            if (oldItem == null)
            {
                GeneralResItem item = new GeneralResItem
                {
                    ResID = resGid.Value,
                    Currency = newCurrencyID
                };
                dbEntity.GeneralResItems.Add(item);
                dbEntity.SaveChanges();
            }
            return RedirectToAction("EditCash", new { param = Request.QueryString["param"] });
        }

        public JsonResult DeleteCash(GeneralResItem deletedModel)
        {
            if (deletedModel.Gid != Guid.Empty)
            {
                GeneralResItem item = (from i in dbEntity.GeneralResItems
                                       where i.Gid == deletedModel.Gid
                                          && i.Deleted == false
                                       select i).FirstOrDefault();
                if (item != null)
                {
                    item.Deleted = true;
                    dbEntity.SaveChanges();
                }
            }
            return Json(GridResponse.CreateSuccess(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取可用多货币输入下拉框
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetAvailableCurrencySelectList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            GeneralResource resource = (from i in dbEntity.GeneralResources.Include("ResourceItems")
                                        where i.Gid == resGid
                                           && i.Deleted == false
                                        select i).Single();
            GeneralMeasureUnit mainCurrency = (from u in dbEntity.GeneralMeasureUnits
                                 where u.Gid == resource.Currency
                                    && u.Deleted == false
                                 select u).Single();

            IQueryable<GeneralMeasureUnit> allCurrencys = from u in dbEntity.GeneralMeasureUnits
                                            where u.Deleted == false
                                               && u.Utype == (byte)ModelEnum.MeasureUnit.CURRENCY
                                            select u;
            IQueryable<Guid> selectedCurrencys = from r in dbEntity.GeneralResItems
                                                 where r.Deleted == false
                                                    && r.ResID == resGid
                                                 select r.Currency.Value;
            IQueryable<GeneralMeasureUnit> availableCurrencys = from c in allCurrencys
                                                                where c != mainCurrency
                                                                   && !selectedCurrencys.Contains(c.Gid)
                                                                select c;
            foreach (GeneralMeasureUnit unit in availableCurrencys)
            {
                list.Add(new SelectListItem
                {
                    Text = GetMatterValue(unit.Name),
                    Value = unit.Gid.ToString()
                });
            }
            return list;
        }
        /// <summary>
        /// 获取GeneralResource字符串的当前语言值
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public string GetMatterValue(GeneralResource res)
        {
            int currentCulture = CurrentSession.Culture;
            string matter = string.Empty;
            if (currentCulture == res.Culture)
                matter = res.Matter;
            else
            {
                foreach (GeneralResItem item in res.ResourceItems)
                {
                    if (currentCulture == item.Culture)
                    {
                        matter = item.Matter;
                        break;
                    }
                }
            }
            return matter;
        }

        /// <summary>
        /// 获取所有货币单位对应表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCurrencyUnits()
        {
            StringBuilder sb = new StringBuilder();
            IQueryable<GeneralMeasureUnit> allCurrencys = from u in dbEntity.GeneralMeasureUnits.Include("Name")
                                                          where u.Deleted == false
                                                             && u.Utype == (byte)ModelEnum.MeasureUnit.CURRENCY
                                                          select u;
            foreach (GeneralMeasureUnit unit in allCurrencys)
            {
                sb.Append(unit.Gid.ToString());
                sb.Append(':');
                sb.Append(GetMatterValue(unit.Name));
                sb.Append(";");
            }
            return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
        }

        #endregion CurrencyEditor

        public static class Columns
        {
            #region MatterEditor
            private static GridColumnModelList<GeneralResItem> _MatterColumns = GetMatterColumns();

            public static GridColumnModelList<GeneralResItem> MatterColumns
            {
                get
                {
                    return _MatterColumns;
                }
            }

            private static GridColumnModelList<GeneralResItem> GetMatterColumns()
            {
                GridColumnModelList<GeneralResItem> columns = new GridColumnModelList<GeneralResItem>();
                columns.Add(i => i.Gid)
                    .SetAsPrimaryKey()
                    .SetHidden(true);
                columns.Add(i => i.Culture)
                    .SetHidden(true);
                columns.Add(i => i.CultureName)
                    .SetWidth("300")
                    .SetEditable(false)
                    .SetCaption("国家地区");
                columns.Add(i => i.Matter)
                    .SetWidth("300")
                    .SetCaption("值");
                return columns;
            }
            #endregion MatterEditor

            #region CashEditor
            private static GridColumnModelList<GeneralResItem> _CashColumns = GetCashColumns();

            public static GridColumnModelList<GeneralResItem> CashColumns
            {
                get
                {
                    return _CashColumns;
                }
            }

            private static GridColumnModelList<GeneralResItem> GetCashColumns()
            {

                GridColumnModelList<GeneralResItem> columns = new GridColumnModelList<GeneralResItem>();
                columns.Add(i => i.Gid)
                    .SetAsPrimaryKey()
                    .SetHidden(true);
                columns.Add(i => i.Culture)
                    .SetHidden(true);
                columns.Add(i => i.Currency)
                    .SetWidth("300")
                    //.SetColumnRenderer(new ComboColumnRenderer("/Resource/GetCurrencyUnits"))
                    .SetEditable(false)
                    .SetCaption("货币");
                columns.Add(i => i.Cash)
                    .SetWidth("300")
                    .SetCaption("值");
                return columns;
            }

            #endregion CashEditor
        }
    }
}
