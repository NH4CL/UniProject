using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyWorkplace_tiangang.Controllers;
using LiveAzure.Models;
using LiveAzure.Utility;
using LiveAzure.Models.General;

namespace MyWorkplace_tiangang.Controllers
{
    public class StandardCategoryController : Controller
    {
        public LiveEntities dbEntity = new LiveEntities(ConfigHelper.LiveConnection.Connection);

        public ActionResult Index()
        {
            var oStandardCategories = dbEntity.GeneralStandardCategorys.Where(g => g.Parent == null && g.Deleted == false).ToList();

            ViewBag.standardCategoryList = getStandardCategoryTypeSelectlist();

            return View(oStandardCategories);
        }

        public ActionResult ChangeTree()
        {

            return PartialView();
        }

        public List<SelectListItem> getStandardCategoryTypeSelectlist()
        {
            var oStandardCategoryTypeName = Enum.GetNames(typeof(LiveAzure.Models.ModelEnum.StandardCategoryType));
            var oStandardCategoryTypevalues = (LiveAzure.Models.ModelEnum.StandardCategoryType[])Enum.GetValues(typeof(LiveAzure.Models.ModelEnum.StandardCategoryType));
            List<SelectListItem> oStandardCategoryTypeList = new List<SelectListItem>();
            int nStandardCategoryTypeCount = oStandardCategoryTypeName.Count();
            for (int i = 0; i < nStandardCategoryTypeCount; i++)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = oStandardCategoryTypeName[i],
                    Value = ((byte)(oStandardCategoryTypevalues[i])).ToString()
                };
                oStandardCategoryTypeList.Add(item);
            }
            return oStandardCategoryTypeList;
        }
    }
}
