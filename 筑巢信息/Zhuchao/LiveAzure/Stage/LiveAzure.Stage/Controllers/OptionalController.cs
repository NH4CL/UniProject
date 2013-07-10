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
using System.Text;
using LiveAzure.BLL;

namespace LiveAzure.Stage.Controllers
{
    public class OptionalController : BaseController
    {
        static Guid optid;
        static Guid? orgId; //组织ID;

        /// <summary>
        /// 用户所属组织，从Session里读出
        /// </summary>
        /// <returns></returns>
        public Guid GetOrganization()
        {
            MemberUser oUser = dbEntity.MemberUsers.Find(CurrentSession.UserID);
            return (oUser.OrgID);
        }
        
        /// <summary>
        /// 属性首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.OrgSelectList = GetOrganizationList();
            return View();
        }

        /// <summary>
        /// 保存添加的属性
        /// </summary>
        /// <param name="optional">页面传回的GeneralOptional对象</param>
        /// <returns>Optional/SetOptionalItems</returns>
        public void SaveNewOptional(GeneralOptional optional)
        {
            try
            {
                GeneralOptional oOptional = new GeneralOptional
                {
                    RefID = optional.RefID,
                    Otype = optional.Otype,
                    Code = optional.Code,
                    Sorting = optional.Sorting,
                    Name = new GeneralResource(ModelEnum.ResourceType.STRING, optional.Name),
                    InputMode = optional.InputMode,
                    OrgID = optional.OrgID,
                    Remark = optional.Remark
                };
                dbEntity.GeneralOptionals.Add(oOptional);
                dbEntity.SaveChanges();
            }
            catch (Exception ex)
            {
                RedirectToAction("ErrorPage", "Home", new { message = ex.Message });
            }
        }

        public ActionResult AddNewOptional()
        {
            ViewBag.optionalTypeList = getOptionalTypeSelectlist();
            ViewBag.optionalInputMode = getInputModeSelectlist();
            ViewBag.OrgSelectList = GetOrganizationList();
            ViewBag.otitle = LiveAzure.Resource.Stage.OptionalController.AddOptional;
            Guid orgGid = GetOrganization();
            GeneralOptional oOptional = new GeneralOptional
            {
                Name = NewResource(ModelEnum.ResourceType.STRING, orgGid)
            };
            return PartialView(oOptional);
        }

        public ActionResult OptionalListTable()
        {
            return PartialView();
        }

        public ActionResult EditOptional(Guid oGid)
        {
            ViewBag.editOptionalTypeList = getOptionalTypeSelectlist();
            ViewBag.editOptionalInputMode = getInputModeSelectlist();
            GeneralOptional oOptional = dbEntity.GeneralOptionals.Where(g => g.Gid == oGid).Single();
            Guid orgGid = GetOrganization();
            oOptional.Name = RefreshResource(ModelEnum.ResourceType.STRING, oOptional.Name, orgGid);
            ViewBag.oType = oOptional.Otype;
            return PartialView(oOptional);
        }

        /// <summary>
        /// 更新Optional
        /// </summary>
        /// <returns></returns>
        public void SaveOptional(GeneralOptional newOptional)
        {

            GeneralOptional oldOptional = dbEntity.GeneralOptionals.Include("Name").Where(g => g.Gid == newOptional.Gid).Single();
            oldOptional.Otype = newOptional.Otype;
            oldOptional.Code = newOptional.Code;
            oldOptional.RefID = newOptional.RefID;
            oldOptional.Sorting = newOptional.Sorting;
            oldOptional.Name.SetResource(ModelEnum.ResourceType.STRING, newOptional.Name);
            oldOptional.InputMode = newOptional.InputMode;
            oldOptional.Remark = newOptional.Remark;

            if (ModelState.IsValid)
            {
                dbEntity.Entry(oldOptional).State = EntityState.Modified;
                dbEntity.SaveChanges();
            }
        }

        public ActionResult OptionalItemListTable(Guid optGid)
        {
            GeneralOptional oOptional = dbEntity.GeneralOptionals.Where(o => o.Gid == optGid).Single();
            ViewBag.optionalName = oOptional.Name.GetResource(CurrentSession.Culture);
            optid = optGid;
            return PartialView();
        }

        public ActionResult AddOptionalItem(Guid optGid)
        {
            Guid orgGid = GetOrganization();
            GeneralOptItem oOptionalItem = new GeneralOptItem
            {
                OptID = optGid,
                Name = NewResource(ModelEnum.ResourceType.STRING, orgGid)
            };
            optid = optGid;
            GeneralOptional oOptional = dbEntity.GeneralOptionals.Where(o => o.Gid == optGid).Single();
            ViewBag.optionalName = oOptional.Name.GetResource(CurrentSession.Culture);
            return View(oOptionalItem);
        }

        /// <summary>
        /// 保存属性选项
        /// </summary>
        /// <returns>选项列表</returns>
        public void SaveOptionalItem(GeneralOptItem OptionalItem)
        {
            GeneralOptItem oOptionalItem = new GeneralOptItem
            {
                OptID = OptionalItem.OptID,
                Code = OptionalItem.Code,
                Sorting = OptionalItem.Sorting,
                Name = new GeneralResource(ModelEnum.ResourceType.STRING, OptionalItem.Name),
                Remark = OptionalItem.Remark
            };

            dbEntity.GeneralOptItems.Add(oOptionalItem);
            dbEntity.SaveChanges();
        }

        /// <summary>
        /// 获取属性输入模式下拉框列表
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> getInputModeSelectlist()
        {
            List<ListItem> inputModeList = new GeneralOptional().InputModeList;
            List<SelectListItem> oInputModeList = new List<SelectListItem>();
            foreach (ListItem item in inputModeList)
            {
                oInputModeList.Add(new SelectListItem { Value = item.Value, Text = item.Text });
            }
            
            return oInputModeList;
        }

        /// <summary>
        /// 获取属性类型下拉框列表
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> getOptionalTypeSelectlist()
        {
            List<ListItem> oTypelist = new GeneralOptional().OptionalTypeList;
            List<SelectListItem> oTypeSelectlist=new List<SelectListItem>();
            foreach (ListItem item in oTypelist)
            {
                oTypeSelectlist.Add(new SelectListItem { Value = item.Value, Text = item.Text });
            }
            return oTypeSelectlist;
        }

        /// <summary>
        /// GeneralOptionalItems Grid列表
        /// </summary>
        /// <param name="optionalSearchModel"></param>
        /// <returns></returns>
        public ActionResult OptionalItemList(SearchModel optionalSearchModel)
        {
            IQueryable<GeneralOptItem> oGeneralOptionalItem =  dbEntity.GeneralOptItems.Include("Name").Where(g=>g.OptID==optid && g.Deleted == false).AsQueryable();

            GridColumnModelList<GeneralOptItem> columns = new GridColumnModelList<GeneralOptItem>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Optional.Name.GetResource(CurrentSession.Culture)).SetName("Optional");
            columns.Add(p => p.Name.GetResource(CurrentSession.Culture)).SetName("Name");
            columns.Add(p => p.Code);
            columns.Add(p => p.Sorting);

            GridData optionalGridData = oGeneralOptionalItem.ToGridData(optionalSearchModel, columns);
            return Json(optionalGridData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GeneralOptional Grid列表
        /// </summary>
        /// <param name="optionalItemSearchModel"></param>
        /// <returns></returns>
        public ActionResult OptionalList(SearchModel optionalItemSearchModel)
        {
            Guid OrgID=GetOrganization();
            IQueryable<GeneralOptional> oGeneralOptional = dbEntity.GeneralOptionals.Include("Name").Where(g => g.Deleted == false && g.OrgID == ((orgId == null) ? OrgID : orgId)).AsQueryable();

            GridColumnModelList<GeneralOptional> columns = new GridColumnModelList<GeneralOptional>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Otype);
            columns.Add(p => p.Code);
            columns.Add(p => p.Name.GetResource(CurrentSession.Culture)).SetName("Name");
            columns.Add(p => p.Sorting);
            columns.Add(p => p.InputMode);

            GridData optionalItemGridData = oGeneralOptional.ToGridData(optionalItemSearchModel, columns);
            return Json(optionalItemGridData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除页面选中的GeneralOptItem对象
        /// </summary>
        /// <param name="optItemGid">页面选中对象的GID</param>
        /// <returns></returns>
        public void RemoveOptionalItems(Guid optItemGid)
        {
            GeneralOptItem otherOptItem = dbEntity.GeneralOptItems.Where(g => g.Gid == optItemGid).Single();
            otherOptItem.Deleted = true;
            dbEntity.Entry(otherOptItem).State = EntityState.Modified;
            dbEntity.SaveChanges();
        }

        /// <summary>
        /// 删除页面选中的GeneralOptional对象
        /// </summary>
        /// <param name="Optionalpost">页面选中的GeneralOptional对象</param>
        /// <returns></returns>
        public void RemoveOptional(Guid optionalGid)
        {
            GeneralOptional otherOptional = dbEntity.GeneralOptionals.Where(g => g.Gid == optionalGid).Single();
            var otherOptionalItems = dbEntity.GeneralOptItems.Where(o => o.OptID == optionalGid).ToList();
            otherOptional.Deleted = true;

            //删除所有对应的下拉框选项
            foreach (GeneralOptItem item in otherOptionalItems)
            {
                item.Deleted = true;
                dbEntity.Entry(item).State = EntityState.Modified;
            }
            dbEntity.Entry(otherOptional).State = EntityState.Modified;
            dbEntity.SaveChanges();
        }

        public ActionResult GetOptionalType()
        {
            StringBuilder sb = new StringBuilder();
            List<ListItem> list = new GeneralOptional().OptionalTypeList;

            foreach (ListItem OType in list)
                sb.Append(OType.Value + ":" + OType.Text + ";");

            return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInputMode()
        {
            StringBuilder sb = new StringBuilder();
            List<ListItem> list = new GeneralOptional().InputModeList;

            foreach (ListItem InputMode in list)
                sb.Append(InputMode.Value + ":" + InputMode.Text + ";");

            return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
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

        public void ChangeOrganization(Guid orgGid)
        {
            orgId = orgGid;
        }
    }
}
