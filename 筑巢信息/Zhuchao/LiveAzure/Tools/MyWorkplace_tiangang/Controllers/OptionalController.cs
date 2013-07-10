using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveAzure.Models.General;
using MVC.Controls;
using MVC.Controls.Grid;
using LiveAzure.Models;
using LiveAzure.Utility;
using LiveAzure.Models.Member;
using System.Web.Services;
using System.Data;
using LiveAzure.Stage.Controllers;

namespace MyWorkplace_tiangang.Controllers
{
    public class OptionalController : BaseController
    {
        //private LiveEntities dbEntity = new LiveEntities(ConfigHelper.LiveConnection.Connection);

        static GeneralOptional optionals = new GeneralOptional();
        
        /// <summary>
        /// 编辑属性页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //从session里读出用户名，查出其所属组织
            string loginName = "admin";
            MemberUser user = dbEntity.MemberUsers.Where(u => u.LoginName == loginName).Single();

            //调用方法生成页面所需的下拉框列表
            ViewBag.optionalList = getOptionalTypeSelectlist();
            ViewBag.optionalInputMode = getInputModeSelectlist();

            //新建GeneralOptional对象，将其所属组织赋值
            GeneralOptional optional = new GeneralOptional
            {
                OrgID = user.OrgID
            };
            return View(optional);
        }

        /// <summary>
        /// (POST)保存页面传回的GeneralOptional数据
        /// </summary>
        /// <param name="optional">页面传回的GeneralOptional对象</param>
        /// <returns>Optional/SetOptionalItems</returns>
        [HttpPost]
        public ActionResult Index(GeneralOptional optional)
        {
            optionals = optional;
            dbEntity.GeneralOptionals.Add(optional);
            dbEntity.SaveChanges();
            if (optional.InputMode == (byte)ModelEnum.OptionalInputMode.COMBOBOX)
            {
                return RedirectToAction("SetOptionalItems", "Optional");
            }
            else
            {
                return RedirectToAction("Index","Home");
            }
        }

        /// <summary>
        /// 属性选项设置页
        /// </summary>
        /// <returns></returns>
        public ActionResult SetOptionalItems()
        {
            return View();
        }
        
        /// <summary>
        /// (POST)保存属性选项
        /// </summary>
        /// <param name="oOptionalItem">页面传回的GeneralOptItem对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetOptionalItems(GeneralOptItem oOptionalItem)
        {
            //为oOptionalItem对象的主表字段赋值，值为Index方法中保存的GeneralOptional对象的Gid
            oOptionalItem.OptID = optionals.Gid;
            dbEntity.GeneralOptItems.Add(oOptionalItem);
            dbEntity.SaveChanges();
            return View();
        }

        /// <summary>
        /// 根据页面传回的数据产生相对应的参考ID下拉框列表
        /// </summary>
        /// <param name="optionalType">页面传回的选项类型</param>
        /// <returns></returns>
        public ActionResult ChangeRefIDList(byte optionalType)
        {
            List<SelectListItem> refIDList = new List<SelectListItem>();

            //获取页面数据，显示不同的下拉框的值
            switch (optionalType)
            {
                case ((byte)ModelEnum.OptionalType.USER):

                    break;
                case ((byte)ModelEnum.OptionalType.GENERIC):
                    {
                        foreach (MemberUser member in dbEntity.MemberUsers)
                        {
                            string code = member.Gid.ToString();
                            SelectListItem item = new SelectListItem
                            {
                                Text = member.FirstName,
                                Value = code
                            };
                            refIDList.Add(item);
                        }
                        break;
                    }
                case ((byte)ModelEnum.OptionalType.ORDER):
                    {
                        foreach (GeneralOptional opt in dbEntity.GeneralOptionals)
                        {
                            SelectListItem item = new SelectListItem
                            {
                                Text = opt.Code,
                                Value = opt.Gid.ToString()
                            };
                            refIDList.Add(item);
                        }
                        break;
                    }
                case ((byte)ModelEnum.OptionalType.ORGANIZATION):
                    {
                        foreach (MemberOrganization org in dbEntity.MemberOrganizations)
                        {
                            SelectListItem item = new SelectListItem
                            {
                                Text = org.Code,
                                Value = org.Gid.ToString()
                            };
                            refIDList.Add(item);
                        }
                        break;
                    }
                case ((byte)ModelEnum.OptionalType.PRODUCT):
                default:
                    {
                        foreach (MemberRole role in dbEntity.MemberRoles)
                        {
                            SelectListItem item = new SelectListItem
                            {
                                Text = role.Code,
                                Value = role.Gid.ToString()
                            };
                            refIDList.Add(item);
                        }
                        break;
                    }
            }
            ViewBag.refIDList = refIDList;
            return PartialView();
        }

        /// <summary>
        /// 自动添加前缀
        /// </summary>
        /// <param name="userInput">用户输入的数据</param>
        /// <param name="resultCount">加入前缀后的数据</param>
        /// <returns></returns>
        public ActionResult AutoAddPrefix(string userInput, int resultCount)
        {
            List<AutoPrefix> autoPrefix = new List<AutoPrefix>();
            autoPrefix.Add(new AutoPrefix() { id = 1, text = "pref" + userInput, value = "pref" + userInput });
            return Json(autoPrefix);
        }

        /// <summary>
        /// 定义前缀
        /// </summary>
        public class AutoPrefix
        {
            public string text { get; set; }
            public string value { get; set; }
            public int id { get; set; }
        }

        /// <summary>
        /// 获取属性输入模式下拉框列表
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> getInputModeSelectlist()
        {
            var oInputnames = Enum.GetNames(typeof(LiveAzure.Models.ModelEnum.OptionalInputMode));
            var oInputvalues = (LiveAzure.Models.ModelEnum.OptionalType[])Enum.GetValues(typeof(LiveAzure.Models.ModelEnum.OptionalInputMode));
            List<SelectListItem> oInputModeList = new List<SelectListItem>();
            int oinputcount = oInputnames.Count();
            for (int i = 0; i < oinputcount; i++)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = oInputnames[i],
                    Value = ((byte)(oInputvalues[i])).ToString()
                };
                oInputModeList.Add(item);
            }
            return oInputModeList;
        }

        /// <summary>
        /// 获取属性类型下拉框列表
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> getOptionalTypeSelectlist()
        {
            //读取枚举类型OptionType的Name和value
            var otypenames = Enum.GetNames(typeof(LiveAzure.Models.ModelEnum.OptionalType));
            var otypevalues = (LiveAzure.Models.ModelEnum.OptionalType[])Enum.GetValues(typeof(LiveAzure.Models.ModelEnum.OptionalType));

            List<SelectListItem> oTypeSelectlist = new List<SelectListItem>();

            int otypecount = otypenames.Count();
            for (int i = 0; i < otypecount; i++)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = otypenames[i],
                    Value = ((byte)(otypevalues[i])).ToString()
                };
                oTypeSelectlist.Add(item);
            }
            return oTypeSelectlist;
        }

        /// <summary>
        /// 从数据库读取Index方法保存的对象所对应的下拉框选项
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult OptionalItemList(SearchModel searchModel)
        {
            IQueryable<GeneralOptItem> oGeneralOptionalItem =  dbEntity.GeneralOptItems.Include("Name").Where(g=>g.OptID==optionals.Gid).Where(g => g.Deleted == false).AsQueryable();
            //int i = 2052;

            GridColumnModelList<GeneralOptItem> columns = new GridColumnModelList<GeneralOptItem>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Code);
            columns.Add(p => p.Name.Matter).SetName("Name.Matter");
            columns.Add(p => p.Sorting);

            GridData gridData = oGeneralOptionalItem.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除选中的GeneralOptItem对象
        /// </summary>
        /// <param name="OptionalItempost"></param>
        /// <returns></returns>
        public ActionResult DeleteOptionalItem(GeneralOptItem OptionalItempost)
        {
            GeneralOptItem otherOptItem = dbEntity.GeneralOptItems.Where(g => g.Gid == OptionalItempost.Gid).Single();

            otherOptItem.Deleted = true;
            dbEntity.Entry(otherOptItem).State = EntityState.Modified;
            dbEntity.SaveChanges();
            return Json(GridResponse.CreateSuccess(), JsonRequestBehavior.AllowGet);
            
        }
    }
}
