using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using JqForMvcTest_tianyou.Models;
using LiveAzure.Models;
using LiveAzure.Models.General;
using LiveAzure.Models.Member;
using LiveAzure.Utility;
using MVC.Controls;
using MVC.Controls.Grid;

namespace JqForMvcTest_tianyou.Controllers
{
    public class RendererController : BaseController
    {
        // private LiveEntities db = new LiveEntities(ConfigHelper.LiveConnection.Connection);

        //
        // GET: /Renderers/

        public ActionResult Renderers()
        {
            return View();
        }

        //Create grid
        public ActionResult ListProgs(SearchModel searchModel)
        {
            IQueryable<GeneralProgram> oPrograms = dbEntity.GeneralPrograms.Include("Name").Where( p => p.Deleted ==false ).AsQueryable();
            int i = 2052;
            GridColumnModelList<GeneralProgram> columns = new GridColumnModelList<GeneralProgram>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Name.GetResource(i)).SetName("Name.Matter");
            columns.Add(p => p.Code);
            columns.Add(p => p.ProgUrl);

            GridData gridData = oPrograms.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteProg(GeneralProgram prog)
        {
            GeneralProgram p = dbEntity.GeneralPrograms.Single(item => item.Gid == prog.Gid);
            p.Deleted = true;
            dbEntity.Entry(p).State = EntityState.Modified;
            dbEntity.SaveChanges();
            return Json(GridResponse.CreateSuccess(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditProg(GeneralProgram prog)
        {
            if (prog.Gid.ToString() != new Guid().ToString())
            {
                GeneralProgram p = dbEntity.GeneralPrograms.Single(item => item.Gid == prog.Gid);
                p.Name.Matter = prog.Name.Matter;
                p.ProgUrl = prog.ProgUrl;
                dbEntity.Entry(p).State = EntityState.Modified;
                dbEntity.SaveChanges();
            }
            else
            {
                GeneralProgram oProgram = new GeneralProgram();
                GeneralResource oResource = new GeneralResource();
                oResource.Matter = prog.Name.Matter;
                oResource.Culture = 2052;
                dbEntity.Entry(oResource).State = EntityState.Added;
                dbEntity.SaveChanges();

                oProgram.Name = oResource;
                oProgram.ProgUrl = prog.ProgUrl;
                oProgram.Code = prog.Code;
                dbEntity.Entry(oProgram).State = EntityState.Added;
                dbEntity.SaveChanges();
            }
            return Json(GridResponse.Create(true), JsonRequestBehavior.AllowGet);
        }
    }
}
