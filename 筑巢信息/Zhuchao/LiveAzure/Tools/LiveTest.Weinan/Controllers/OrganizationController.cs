using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveAzure.Models.General;
using LiveAzure.Utility;
using LiveAzure.Models.Member;
using System.Web.Helpers;
using LiveAzure.Resource.Stage;
using System.Collections;
using LiveAzure.Models;
using MVC.Controls;
using MVC.Controls.Grid;

namespace LiveTest.Weinan.Controllers
{
    public class OrganizationController : BaseController
    {
        //创建数据库连接对象db
        //private LiveEntities dbEntity = new LiveEntities(ConfigHelper.LiveConnection.Connection);
        //
        // GET: /Organization/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Index页面中Grid
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult List(SearchModel searchModel)
        {

            //var organs = from o in dbEntity.MemberOrganizations.Include("FullName").Include("ShortName")
            //             where o.Deleted == false
            //             select o;

            //List<MemberOrganization> model = new List<MemberOrganization>();
            //model.AddRange(organs);

            //return Json(model.AsQueryable().ToGridData(searchModel, 
            //    new[] { "Gid", "Code", "FullName", "ShortName", "Ostatus","Otype" }),
            //    JsonRequestBehavior.AllowGet);

            int oType = 1; //oType为上一页面传入值，这里默认为0.

            IQueryable<MemberOrganization> organs = dbEntity.MemberOrganizations.Include("FullName").Include("ShortName").Where(p => p.Deleted == false&&p.Otype==oType).AsQueryable();
            //int i = 2052;
            GridColumnModelList<MemberOrganization> columns = new GridColumnModelList<MemberOrganization>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Code);
            columns.Add(p => p.FullName.Matter);
            columns.Add(p => p.ShortName.Matter);
            columns.Add(p => p.Ostatus);
            columns.Add(p => p.Sorting);

            GridData gridData = organs.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
     

        /// <summary>
        /// 查看和编辑组织详情
        /// </summary>
        /// <returns></returns>
        public ActionResult OrgDetail(Guid? id,MemberOrganization memberOrganization)
        {
            //Guid testid = new Guid();
            string testid1 = Request.Form["id"];

            string ttt = Request.QueryString["id"];
            //生成“状态”下拉框
            if (Session["sessionId"] != null)
            {
                string sid = Session["sessionId"].ToString();
                id = new Guid(sid);
            }  
            var ostatusnames = Enum.GetNames(typeof(LiveAzure.Models.ModelEnum.OrganizationStatus));
            var ostatusvalues = (LiveAzure.Models.ModelEnum.OrganizationStatus[])Enum.GetValues(typeof
                (LiveAzure.Models.ModelEnum.OrganizationStatus));
            List<SelectListItem> ostatuslist = new List<SelectListItem>();
            for (int i = 0; i < 2; i++)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = ostatusnames[i],
                    Value = ((byte)(ostatusvalues[i])).ToString()
                };
                ostatuslist.Add(item);
            }
            ViewBag.ostatuslist = ostatuslist;

            //生成“类型”下拉框
            var otypenames = Enum.GetNames(typeof(LiveAzure.Models.ModelEnum.OrganizationType));
            var otypevalues = (LiveAzure.Models.ModelEnum.OrganizationType[])Enum.GetValues(
                typeof(LiveAzure.Models.ModelEnum.OrganizationType));
            List<SelectListItem> otypelist = new List<SelectListItem>();
            for (int i = 0; i < 5; i++)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = otypenames[i],
                    Value = ((byte)(otypevalues[i])).ToString()
                };
                otypelist.Add(item);
            }
            ViewBag.otypelist = otypelist;

           
            //string ttt = Request.QueryString["sessionId"].ToString();
            if (id == null) 
            {
                ViewData["fullResId"] = new Guid("00000000-0000-0000-0000-000000000000");
                ViewData["shortResId"] = new Guid("00000000-0000-0000-0000-000000000000");
                return View();
            }
            else
            {
                var memorganization = dbEntity.MemberOrganizations.Include("FullName").Include("ShortName").Include("ExtendType").Include("Location").

                                Where(o => o.Gid == id && o.Deleted == false).Single();
                var fullResId = (from f in dbEntity.MemberOrganizations.Include("FullName")
                                where(f.Gid == id && f.Deleted == false)
                                select f.aFullName).Single();
                var shortResId = (from f in dbEntity.MemberOrganizations.Include("ShortName")
                                 where (f.Gid == id && f.Deleted == false)
                                 select f.aShortName).Single();
                ViewData["fullResId"] = fullResId;
                ViewData["shortResId"] = shortResId;
                return View(memorganization);
            }
 
        }

        /// <summary>
        /// 保存组织详情
        /// </summary>
        /// <param name="memorganization"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OrgDetail(MemberOrganization memorganization)
        {
            MemberOrganization newOrg;
            if (!memorganization.Gid.ToString().Equals("00000000-0000-0000-0000-000000000000"))
            {
                newOrg = (from o in dbEntity.MemberOrganizations
                          where (o.Gid == memorganization.Gid && o.Deleted == false)
                          select o).Single();
                newOrg.Code = memorganization.Code;
                newOrg.ExCode = memorganization.ExCode;
                newOrg.Ostatus = memorganization.Ostatus;
                newOrg.Otype = memorganization.Otype;
                //newOrg.ExType = memorganization.ExType;
                //newOrg.ExtendType.Name = memorganization.ExtendType.Name;
                newOrg.ExType = memorganization.ExType;
                newOrg.ExtendType.aName = memorganization.ExtendType.aName;
                newOrg.ExtendType.Name.Matter = memorganization.ExtendType.Name.Matter;
                newOrg.aFullName = memorganization.aFullName;
                newOrg.FullName.Matter = memorganization.FullName.Matter;
                newOrg.aShortName = memorganization.aShortName;
                newOrg.ShortName.Matter = memorganization.ShortName.Matter;
                newOrg.FullAddress = memorganization.FullAddress;
                newOrg.Contact = memorganization.Contact;
                newOrg.WorkPhone = memorganization.WorkPhone;
                newOrg.CellPhone = memorganization.CellPhone;
                newOrg.WorkFax = memorganization.WorkFax;
                newOrg.Email = memorganization.Email;
                newOrg.HomeUrl = memorganization.HomeUrl;
                newOrg.Sorting = memorganization.Sorting;
                newOrg.Brief = memorganization.Brief;
                newOrg.aLocation = memorganization.aLocation;
                dbEntity.Entry(newOrg).State = System.Data.EntityState.Modified;
            }
            else
            {
                GeneralResource res = new GeneralResource
                {
                    Matter = memorganization.ExtendType.Name.Matter
                };
                GeneralStandardCategory sCat = new GeneralStandardCategory
                {
                    Code = "a test",
                    Name = res
                };
                newOrg = new MemberOrganization();
                newOrg.Code = memorganization.Code;
                newOrg.ExCode = memorganization.ExCode;
                newOrg.Ostatus = memorganization.Ostatus;
                newOrg.Otype = memorganization.Otype;
                //newOrg.ExType = memorganization.ExType;
                //newOrg.ExtendType.Name = memorganization.ExtendType.Name;
                //newOrg.ExType = memorganization.ExType;
                //newOrg.ExtendType.aName = memorganization.ExtendType.aName;
                //newOrg.ExtendType.Name.Matter = memorganization.ExtendType.Name.Matter;


                newOrg.ExtendType = sCat;
                
                newOrg.FullName = memorganization.FullName;
                newOrg.ShortName = memorganization.ShortName;



                //newOrg.aFullName = memorganization.aFullName;
                //newOrg.FullName.Matter = memorganization.FullName.Matter;
                //newOrg.aShortName = memorganization.aShortName;
                //newOrg.ShortName.Matter = memorganization.ShortName.Matter;
                newOrg.FullAddress = memorganization.FullAddress;
                newOrg.Contact = memorganization.Contact;
                newOrg.WorkPhone = memorganization.WorkPhone;
                newOrg.CellPhone = memorganization.CellPhone;
                newOrg.WorkFax = memorganization.WorkFax;
                newOrg.Email = memorganization.Email;
                newOrg.HomeUrl = memorganization.HomeUrl;
                newOrg.Sorting = memorganization.Sorting;
                newOrg.Brief = memorganization.Brief;
                newOrg.aLocation = memorganization.aLocation;
                dbEntity.MemberOrganizations.Add(newOrg);
                
            }
            dbEntity.SaveChanges();
 
            return RedirectToAction("Index");
        }
        /// <summary>
        /// 逻辑删除数据
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
        /// 逻辑删除属性表格数据
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
       
        public ActionResult ListAttribute(SearchModel searchModel)
        {
            //Guid id = new Guid();
            //MemberOrgAttribute attributes;
            //if (Session["sessionId"] != null)
            //{
            //    string sid = Session["sessionId"].ToString();
            //    id = new Guid(sid);
            //    attributes = dbEntity.MemberOrgAttributes.Where(o => o.OrgID == id && o.Deleted == false);
            //}
            //else
            //{
            //    attributes = dbEntity.MemberOrgAttributes.Where(o => o.Deleted == false);
            //}
            //List<MemberOrgAttribute> attributeList = new List<MemberOrgAttribute>();
            //attributeList.AddRange(attributes);
            //return Json(attributeList.AsQueryable().ToGridData(searchModel, new[] { "Gid", "OptID", "OptResult", "Matter" }), JsonRequestBehavior.AllowGet);
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
                memorgattr = dbEntity.MemberOrgAttributes.Where(p =>  p.Deleted == false).AsQueryable();
            }
            GridColumnModelList<MemberOrgAttribute> columns = new GridColumnModelList<MemberOrgAttribute>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Optional.Name.Matter);
            columns.Add(p => p.OptionalResult.Name.Matter);
            columns.Add(p => p.Matter);

            GridData gridData = memorgattr.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 创建新组织--第一个页面：选择组织语言
        /// </summary>
        /// <returns></returns>
        public ViewResult AddCulture()
        {

            ViewBag.aCulture = new SelectList(dbEntity.GeneralCultureUnits, "Gid", "Culture");
            return View();
        }

        /// <summary>
        /// 创建新组织--保存已选择的组织语言
        /// </summary>
        /// <param name="memberOrganization"></param>
        /// <param name="selectedCulture"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddCulture(MemberOrganization memberOrganization, Guid[] selectedCulture)
        {

            if (ModelState.IsValid)
            {
                ICollection<MemberOrgCulture> ImemberOrgCulture = new List<MemberOrgCulture>();
                foreach (Guid guid in selectedCulture)
                {
                    MemberOrgCulture item = new MemberOrgCulture();
                    item.OrgID = memberOrganization.Gid;
                    item.Ctype = (byte)ModelEnum.CultureType.LANGUAGE;
                    item.aCulture = guid;
                    ImemberOrgCulture.Add(item);
                }
                memberOrganization.Cultures = ImemberOrgCulture;
                dbEntity.MemberOrganizations.Add(memberOrganization);
                dbEntity.SaveChanges();

                //foreach(var culture in memberOrganization.Cultures)
                //memberOrgCulture.aCulture = memberOrganization.Cultures.Single

                //db.MemberOrgCultures.Add
                return RedirectToAction("AddOrgAttribute", new { id = memberOrganization.Gid });
            }
            return View();
        }

        /// <summary>
        /// 添加渠道
        /// </summary>
        /// <returns></returns>  
        public ViewResult OrgChannel()
        {
            Guid id;
            var oChannel = from o in dbEntity.MemberOrgChannels.Include("Organization").Include("FullName")
                           where o.Organization.Otype == 1
                           select new { Gid = o.Gid, aName = o.Organization.FullName.Matter };
            
            if (Session["sessionId"] != null)
            {
                string sid = Session["sessionId"].ToString();
                id = new Guid(sid);
                var selectmemberorgchannel =  from o in dbEntity.MemberOrgChannels.Include("Organization")
                                              where o.Organization.Otype == 1 && o.OrgID == new Guid("2cb5565e-c8c7-e011-87f7-00218660bc3a")
                                              select o;
                ViewBag.selectchannel = new SelectList(selectmemberorgchannel, "Gid", "");
            }
            else
            {
                ViewBag.selectchannel = new SelectList("", "", "");
            }
            //ViewBag.channel =oChannel;
            ViewBag.testyes = "testyes";
            ViewBag.testno = "testno";
            ViewBag.channel = new SelectList(oChannel, "Gid", "aName");
            
            return View();
        }

        [HttpPost]
        public ViewResult OrgChannel(MemberOrganization memberOrganization, Guid[] selectedChannel)
        {
            ICollection<MemberOrgChannel> oMemOrgChl = new List<MemberOrgChannel>();
            foreach (Guid gid in selectedChannel)
            {

                MemberOrgChannel item = new MemberOrgChannel();
                item.OrgID = memberOrganization.Gid;
                item.ChlID = gid;
                oMemOrgChl.Add(item);
            }
            memberOrganization.Channels = oMemOrgChl;
            dbEntity.MemberOrganizations.Add(memberOrganization);
            dbEntity.SaveChanges();
            return View();
        }
        /// <summary>
        /// 添加组织属性--从数据库获取下拉框内容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult OrgAttribute(Guid? id)
        {
            var option = from p in dbEntity.GeneralOptionals.Include("Name")
                         where (p.OrgID == new Guid("2cb5565e-c8c7-e011-87f7-00218660bc3a") && p.Otype == 1)
                         select new { Gid = p.Gid, aName = p.Name.Matter };

            var optionResult = from p in dbEntity.GeneralOptItems.Include("Name").Include("OptID")
                               where (p.Optional.OrgID == new Guid("2cb5565e-c8c7-e011-87f7-00218660bc3a") && p.Optional.Otype == 1
                               && p.Optional.InputMode == 1 && p.OptID == p.Optional.Gid)
                               select new { Gid = p.Gid, aName = p.Name.Matter };
            //      var memberOrgAttributes = from p in db.MemberOrgAttributes
            //                               where (p.OrgID == id )
            //                               select p;
            ViewData["OptID"] = new SelectList(option, "Gid", "aName");
            ViewData["OptResult"] = new SelectList(optionResult, "Gid", "aName");
            //      ViewBag.memberOrgAttribute = new List<MemberOrgAttribute>(memberOrgAttribute);

            return View();
        }

        /// <summary>
        /// 添加组织属性--保存至数据库MemberOrgAttribute
        /// </summary>
        /// <param name="optitemCount"></param>
        /// <param name="memberOrganization"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OrgAttribute(int? optitemCount,Guid id)
        {
            MemberOrganization memberOrganization;
            if (Session["sessionId"] != null)
            {
                string sid = Session["sessionId"].ToString();
                id = new Guid(sid);
                memberOrganization = (from m in dbEntity.MemberOrganizations
                                      where (m.Gid == id)
                                      select m).Single();
            }
            else
                memberOrganization = new MemberOrganization();
           
            if (!ModelState.IsValid)
            {
                ICollection<MemberOrgAttribute> memberOrgAttributes = new List<MemberOrgAttribute>();
                for (int i = 0; i < optitemCount; i++)
                {
                    MemberOrgAttribute memberOrgAttribute = new MemberOrgAttribute();
                    memberOrgAttribute.OrgID = memberOrganization.Gid;
                    //            memberOrgAttribute.OptID = memberOrganization.Attributes.ElementAt(i).OptID;
                    //            memberOrgAttribute.OptResult = memberOrganization.Attributes.ElementAt(i).OptResult;
                    //             memberOrgAttribute.Matter  = memberOrganization.Attributes.ElementAt(i).Matter ;
                    memberOrgAttribute.OptID = new Guid(Request.Form["OptID" + i]);
                    memberOrgAttribute.OptResult = new Guid(Request.Form["OptResult" + i]);
                    memberOrgAttribute.Matter = Request.Form["Matter" + i];
                    memberOrgAttributes.Add(memberOrgAttribute);
                }
                memberOrganization.Attributes = memberOrgAttributes;
                if (Session["sessionId"] == null)
                {
                    dbEntity.MemberOrganizations.Add(memberOrganization);
                }
                dbEntity.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        public ActionResult RegionTree()
        {
            return View();
        }
        /// <summary>
        /// 加载地区树形结构
        /// </summary>
        /// <returns></returns>
        public string RegionTreeLoad()
        {
            var RegionTreeList = from o in dbEntity.GeneralRegions
                                 where (o.Parent == null && o.Deleted == false)
                                 select o;
            List<LiveTreeNode> list = new List<LiveTreeNode>();
            foreach (var item in RegionTreeList)
            {
                LiveTreeNode node = new LiveTreeNode();
                node.name = item.FullName;
                node.id = item.Gid.ToString();
                node.nodes = new List<LiveTreeNode>();
                list.Add(node);
            }
            string strTree = CreateTree(list);
            return strTree ;
        }

        public string RegionTreeExpand(Guid id)
        {
            string strTree="";
            if (id.Equals("00000000-0000-0000-0000-000000000000"))
            {
                var RegionTreeList = from o in dbEntity.GeneralRegions
                                     where (o.Parent == null && o.Deleted == false)
                                     select o;
                List<LiveTreeNode> list = new List<LiveTreeNode>();
                foreach (var item in RegionTreeList)
                {
                    LiveTreeNode node = new LiveTreeNode();
                    node.name = item.FullName;
                    node.id = item.Gid.ToString();
                    node.nodes = new List<LiveTreeNode>();
                    list.Add(node);
                }
                strTree = CreateTreeJson(list,"");
            }
            else
            {
                var RegionTreeList = from o in dbEntity.GeneralRegions.Include("Parent")
                                     where (o.Parent.Gid == id && o.Deleted == false)
                                     select o;
                List<LiveTreeNode> list = new List<LiveTreeNode>();
                foreach (var item in RegionTreeList)
                {
                    LiveTreeNode node = new LiveTreeNode();
                    node.name = item.FullName;
                    node.id = item.Gid.ToString();
                    node.nodes = new List<LiveTreeNode>();
                    list.Add(node);
                }
                strTree = CreateTreeJson(list, "");
            }
            return "[" + strTree + "]";
        }
        /// <summary>
        /// 获取属性编辑输入模式
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string getInputMode(Guid id)
        {
            var inputMode = (from i in dbEntity.GeneralOptionals
                            where (i.Gid == id)
                            select i.InputMode).Single();
            string json = inputMode.ToString();
            return json;
        }


        public ActionResult OrganizationDefination()
        {
            string ttt = Request.QueryString["id"];
            return View();
        }

        public ActionResult GetTabpage(string strId)
        {
            //string kkk = strId;
            Session["sessionId"] = strId;
            return PartialView("TabPage");
        }

        public ActionResult testGetID(Guid id)
        {
            ViewBag.testID = id;

            return View("TabPage");
        }

        protected override void Dispose(bool disposing)
        {
            dbEntity.Dispose();
            base.Dispose(disposing);
        }
    }
    
}
