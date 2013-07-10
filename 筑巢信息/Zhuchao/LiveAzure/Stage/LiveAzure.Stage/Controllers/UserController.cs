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
using UserResource = LiveAzure.Resource.Model.Member.MemberUser;
using System.Globalization;

namespace LiveAzure.Stage.Controllers
{
    public class UserController : BaseController
    {
        #region 初始化
        //全局变量
        public static Guid gUserId;//被编辑的用户ID
        public static Guid OrgID;//全局组织ID
        public static string gSearchStr;//?
        public static Guid gLevelOrgId;//?
        public static bool isFromOrder = false;//标志位 是否是订单管理中选择用户进入
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

        #region 用户管理

        /// <summary>
        /// 用户管理首页
        /// </summary>
        /// <param name="OrganizationFromOrder">来自订单的组织</param>
        /// <returns></returns>
        public ActionResult Index(Guid? ParentOrgID = null, bool? FromOrder = false)//凤凰添加来源参数FromOrder
        {
            Guid? userID = CurrentSession.UserID;
            gSearchStr = "";
            isFromOrder = (bool)FromOrder;
            if (userID != null)
            {
                //获取用户所属组织
                var userOrgId = dbEntity.MemberUsers.Where(p => p.Deleted == false && p.Gid == userID).Select(p => p.OrgID).Single();
                OrgID = userOrgId;
            }
            //新建页面model
            MemberUser oViewModel = new MemberUser();
            //组织下拉框
            ViewBag.organization = GetSupportOrganizations();
            if (ParentOrgID != null)//有选中的组织
            {
                //ViewBag.IsFromOrg = true;
                oViewModel.OrgID = ParentOrgID.Value;
                //组织下拉框为ParentOrgID的组织
                OrgID = ParentOrgID.Value;//设置全局OrgID
                SetCurrentPath();//当前程序路径
            }
            //if (FromOrder == null)
            //{
            //    ViewBag.fromOrder = false;
            //}
            //else
            //{
            //    isFromOrder = FromOrder;
            //    ViewBag.fromOrder = true;
            //}
            ViewBag.fromOrder = FromOrder;
            return View(oViewModel);
        }
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public ActionResult listUser(SearchModel searchModel)
        {
            IQueryable<MemberUser> users = (from o in dbEntity.MemberUsers.Include("Role").Include("Channel")
                                            where (o.Deleted == false && o.OrgID == OrgID && o.Ustatus == 1)
                                            select o).AsQueryable();
            //搜索功能
            if (!String.IsNullOrEmpty(gSearchStr))
            {
                users = users.Where(s => s.Role.Name.Matter.ToUpper().Contains(gSearchStr.ToUpper())
                                        || s.Channel.FullName.Matter.ToUpper().Contains(gSearchStr.ToUpper())
                                        || s.LoginName.ToUpper().Contains(gSearchStr.ToUpper())
                                        || s.DisplayName.ToUpper().Contains(gSearchStr.ToUpper()));
            }
            GridColumnModelList<MemberUser> columns = new GridColumnModelList<MemberUser>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Role.Name.Matter);
            columns.Add(p => p.Channel.FullName.Matter);
            columns.Add(p => p.LoginName);
            columns.Add(p => p.DisplayName);
            columns.Add(p => p.GenderName).SetName("Gender");
            columns.Add(p => p.Email);

            GridData gridData = users.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 角色管理


        /// <summary>
        /// 角色定义
        /// </summary>
        /// <returns></returns>
        public ActionResult MemberRole(Guid? ParentOrgID=null)
        {
            Guid? userID = CurrentSession.UserID;
            if (userID != null)
            {
                //获取用户所属组织
                var userOrgId = dbEntity.MemberUsers.Where(p => p.Deleted == false && p.Gid == userID).Select(p => p.OrgID).Single();
                OrgID = userOrgId;
            }
            MemberRole oViewModel = new MemberRole();
            //生成组织下拉框
            ViewBag.organization = GetSupportOrganizations();
            if (ParentOrgID != null)//有选中的组织
            {
                //ViewBag.IsFromOrg = true;
                oViewModel.OrgID = ParentOrgID.Value;
                //组织下拉框为ParentOrgID的组织
                OrgID = ParentOrgID.Value;//设置全局OrgID
                SetCurrentPath();//当前程序路径
            }
            return View(oViewModel);
        }
        #endregion

        //------------------------------------------------↑整理过 ↓未整理---------------------------------------------------


        public ActionResult UserDefination()
        {
            return View();
        }

        public string cultureText(int culid)
        {
            CultureInfo a = new CultureInfo(culid);
            return a.NativeName;
        }

        public ActionResult UserInformation()
        {
            //生成“用户状态”下拉框
            MemberUser omemuser = (dbEntity.MemberUsers.Where(o => o.Deleted == false)).FirstOrDefault();
            List<SelectListItem> ouserstatuslist = GetSelectList(omemuser.UserStatusList);
            ViewBag.ouserstatuslist = ouserstatuslist;


            //生成“性别“下拉框
            List<SelectListItem> ousergenderlist = GetSelectList(omemuser.GenderList);
            ViewBag.ousergenderlist = ousergenderlist;


            //生成”该运营商支持渠道“的下拉框
            List<SelectListItem> ochalist = new List<SelectListItem>();
            var memberorgcha = (from o in dbEntity.MemberOrgChannels 
                                where (o.Deleted == false && o.OrgID == OrgID)
                                select o).ToList();
            foreach (var item in memberorgcha)
            {
                SelectListItem item1 = new SelectListItem
                {
                    Text = item.Channel.FullName.Matter,
                    Value = item.ChlID.ToString()
                };
                ochalist.Add(item1);
            }
            ViewBag.ochalist = ochalist;

            //生成"该运营商支持语言"的下拉框
            List<SelectListItem> ocullist = new List<SelectListItem>();
            var memberorgcul = (from o in dbEntity.MemberOrgCultures
                                where (o.Deleted == false && o.OrgID == OrgID && o.Ctype == 0)
                                select o).ToList();
            foreach (var item in memberorgcul)
            {
                SelectListItem item1 = new SelectListItem
                {
                    Text = cultureText (item.Culture.Culture),
                    Value = item.Culture.Gid.ToString()
                };
                ocullist.Add(item1);
            }
            ViewBag.ocullist = ocullist;

            //生成"该运营商支持角色"的下拉框
            List<SelectListItem> orolelist = new List<SelectListItem>();
            var memberorgrole = (from o in dbEntity.MemberRoles
                                where (o.Deleted == false && o.OrgID == OrgID )
                                select o).ToList();
            foreach (var item in memberorgrole)
            {
                SelectListItem item1 = new SelectListItem
                {
                    Text = item.Name.Matter,
                    Value = item.Gid.ToString()
                };
                orolelist.Add(item1);
            }
            ViewBag.orolelist = orolelist;


            if (gUserId.ToString().Equals("00000000-0000-0000-0000-000000000000"))
            {
                return View();
            }
            else
            {
                var memberuser = dbEntity.MemberUsers.Include("Organization").Include("Role").Include("Channel").Include("Manager").Include("Culture").
                                 Where(o => o.Gid == gUserId && o.OrgID == OrgID && o.Deleted == false).Single();
                return View(memberuser);
            }                       
        }






        [HttpPost]
        public ActionResult UserInformation(MemberUser memberuser)
        {
            //生成“用户状态”下拉框
            MemberUser omemuser = (dbEntity.MemberUsers.Where(o => o.Deleted == false)).FirstOrDefault();
            List<SelectListItem> ouserstatuslist = GetSelectList(omemuser.UserStatusList);
            ViewBag.ouserstatuslist = ouserstatuslist;


            //生成“性别“下拉框
            List<SelectListItem> ousergenderlist = GetSelectList(omemuser.GenderList);
            ViewBag.ousergenderlist = ousergenderlist;

            MemberUser newmemberuser;
            if (gUserId.ToString().Equals("00000000-0000-0000-0000-000000000000"))
            {
                newmemberuser = new MemberUser();
                newmemberuser.OrgID = OrgID;
                newmemberuser.RoleID = new Guid("1e3630f8-cace-e011-a4cf-00218660bc3a");
                newmemberuser.ChlID = new Guid("133630f8-cace-e011-a4cf-00218660bc3a");
                newmemberuser.aCulture = new Guid("f53530f8-cace-e011-a4cf-00218660bc3a");
                newmemberuser.LoginName = memberuser.LoginName;
                newmemberuser.Ustatus = memberuser.Ustatus;
                newmemberuser.NickName = memberuser.NickName;
                newmemberuser.LastName = memberuser.LastName;
                newmemberuser.FirstName = memberuser.FirstName;
                newmemberuser.DisplayName = memberuser.DisplayName;
                newmemberuser.aPasscode = "testUser";
                newmemberuser.SaltKey = "47010147";
                newmemberuser.Title = memberuser.Title;
                newmemberuser.Gender = memberuser.Gender;
                newmemberuser.HeadPic = memberuser.HeadPic;
                newmemberuser.UserSign = memberuser.UserSign;
                newmemberuser.Brief = memberuser.Brief;
                newmemberuser.Birthday = memberuser.Birthday;
                newmemberuser.Email = memberuser.Email;
                //保存渠道
                newmemberuser.ChlID = memberuser.ChlID;
                newmemberuser.aCulture = memberuser.aCulture;
                newmemberuser.RoleID = memberuser.RoleID;
                dbEntity.MemberUsers.Add(newmemberuser);
                dbEntity.SaveChanges();
                ViewBag.location = "1";
                ViewBag.staticGuid = newmemberuser.Gid;
                return View("UserDefination");
            }
            else
            {
                newmemberuser = (from o in dbEntity.MemberUsers
                                 where (o.OrgID == OrgID && o.Gid == gUserId)
                                 select o).Single();
                //newmemberuser.RoleID = new Guid("72a3b1a2-1fce-e011-a16f-00218660bc3a");
                //newmemberuser.ChlID = new Guid("76a3b1a2-1fce-e011-a16f-00218660bc3a");
                //newmemberuser.aCulture = new Guid("58a3b1a2-1fce-e011-a16f-00218660bc3a");
                newmemberuser.LoginName = memberuser.LoginName;
                newmemberuser.Ustatus = memberuser.Ustatus;
                newmemberuser.NickName = memberuser.NickName;
                newmemberuser.LastName = memberuser.LastName;
                newmemberuser.FirstName = memberuser.FirstName;
                newmemberuser.DisplayName = memberuser.DisplayName;
                newmemberuser.aPasscode = "testUser";
                newmemberuser.SaltKey = "47010147";
                newmemberuser.Title = memberuser.Title;
                newmemberuser.Gender = memberuser.Gender;
                newmemberuser.HeadPic = memberuser.HeadPic;
                newmemberuser.UserSign = memberuser.UserSign;
                newmemberuser.Brief = memberuser.Brief;
                newmemberuser.Birthday = memberuser.Birthday;
                newmemberuser.Email = memberuser.Email;
                newmemberuser.ChlID = memberuser.ChlID;
                newmemberuser.aCulture = memberuser.aCulture;
                newmemberuser.RoleID = memberuser.RoleID;
                dbEntity.SaveChanges();
                ViewBag.location = "1";
                ViewBag.staticGuid = newmemberuser.Gid;
                return View("UserDefination");
            }      
        }


        public ActionResult listAddress(SearchModel searchModel)
        {
            IQueryable<MemberAddress> addresses = (from o in dbEntity.MemberAddresses.Include("User").Include("Location")
                                                    where ( o.Deleted == false && o.UserID == gUserId)
                                                    select o).AsQueryable();
            GridColumnModelList<MemberAddress> columns = new GridColumnModelList<MemberAddress>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Code);
            columns.Add(p => p.DisplayName);
            //columns.Add(p => p.Location.ShortName);
            columns.Add(p => p.FullAddress);
            columns.Add(p => p.PostCode);
            columns.Add(p => p.CellPhone);
            //columns.Add(p => p.WorkPhone);
            columns.Add(p => p.WorkFax);
            columns.Add(p => p.HomePhone);
            columns.Add(p => p.Email);

            GridData gridData = addresses.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);

        }



        public ActionResult UserAddress()
        {
            if (gUserId.ToString().Equals("00000000-0000-0000-0000-000000000000"))
            {
                return null;
            }
            return View();
        }

      
        [HttpPost]
        public ActionResult UserAddress(MemberAddress memberaddress)
        {
            MemberAddress newmemberaddress ;
            if (memberaddress.Gid.ToString().Equals("00000000-0000-0000-0000-000000000000"))
            {
                newmemberaddress = new MemberAddress();
                newmemberaddress.UserID =(Guid)gUserId;
                newmemberaddress.aLocation = memberaddress.aLocation;
                newmemberaddress.Code = memberaddress.Code;
                newmemberaddress.DisplayName = memberaddress.DisplayName;
                newmemberaddress.FullAddress = memberaddress.FullAddress;
                newmemberaddress.PostCode = memberaddress.PostCode;
                newmemberaddress.CellPhone = memberaddress.CellPhone;
                newmemberaddress.WorkPhone = memberaddress.WorkPhone;
                newmemberaddress.WorkFax = memberaddress.WorkFax;
                newmemberaddress.HomePhone = memberaddress.HomePhone;
                newmemberaddress.Email = memberaddress.Email;
                dbEntity.MemberAddresses.Add(newmemberaddress);
                dbEntity.SaveChanges();
                ViewBag.staticGuid = (Guid)gUserId;
            }
            else
            {
                newmemberaddress = (from o in dbEntity.MemberAddresses
                                    where (o.Gid == memberaddress.Gid && o.Deleted == false)
                                    select o).Single();
                newmemberaddress.Code = memberaddress.Code;
                newmemberaddress.DisplayName = memberaddress.DisplayName;
                newmemberaddress.FullAddress = memberaddress.FullAddress;
                newmemberaddress.PostCode = memberaddress.PostCode;
                newmemberaddress.CellPhone = memberaddress.CellPhone;
                newmemberaddress.WorkPhone = memberaddress.WorkPhone;
                newmemberaddress.WorkFax = memberaddress.WorkFax;
                newmemberaddress.HomePhone = memberaddress.HomePhone;
                newmemberaddress.Email = memberaddress.Email;
                newmemberaddress.aLocation = memberaddress.aLocation;
                dbEntity.SaveChanges();
                ViewBag.staticGuid = (Guid)gUserId;
            }
            if (isFromOrder)
                return RedirectToAction("OrderWithUser", "Order", new { UserGuid = gUserId });
            else
                return View("UserDefination");
  
        }

        public ActionResult UserAttribute()
        {
            return View();
        }
        public ActionResult UserSubscribe()
        {
            return View();
        }
        public ActionResult UserEvent()
        {
            return View();
        }



        public string IndexTest1(Guid currentid, string searchStr)
        {
            gSearchStr = searchStr;
            OrgID = currentid;
            gLevelOrgId = currentid;
            return "success";
        }




        public ActionResult GetUserTabpage(string strId)
        {
            if (strId != null)
            {
                gUserId = new Guid(strId);
            }
            else
            {
                gUserId = new Guid("00000000-0000-0000-0000-000000000000");
            }
            //Session["sessionorgotype"] = strOtype;
            return PartialView("UserTabpage");
        }

        public ActionResult testGrid()
        {
            return View();
        }
        public ActionResult listTestGrid(SearchModel searchModel)
        {
            IQueryable<MemberUser> users = (from o in dbEntity.MemberUsers.Include("Role").Include("Channel")
                                            where (o.Deleted == false && o.OrgID == new Guid("093630f8-cace-e011-a4cf-00218660bc3a") && o.Ustatus == 1)
                                            select o).AsQueryable();
            GridColumnModelList<MemberUser> columns = new GridColumnModelList<MemberUser>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Role.Name.Matter);
            columns.Add(p => p.Channel.FullName.Matter);
            columns.Add(p => p.LoginName);
            columns.Add(p => p.DisplayName);
            columns.Add(p => p.GenderName).SetName("Gender");
            columns.Add(p => p.Email);

            GridData gridData = users.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult editTestGrid(MemberUser memberuser)
        {
            IQueryable<MemberUser> users = (from o in dbEntity.MemberUsers.Include("Role").Include("Channel")
                                            where (o.Deleted == false && o.OrgID == new Guid("093630f8-cace-e011-a4cf-00218660bc3a") && o.Ustatus == 1)
                                            select o).AsQueryable();
            return View("testGrid");
        }


        #region Level
        /// <summary>
        /// 级别入口
        /// </summary>
        /// <returns></returns> 
        public ActionResult Level()
        {
            Guid? userID = CurrentSession.UserID;
            if (userID != null)
            {
                //获取用户所属组织
                var userOrgId = dbEntity.MemberUsers.Where(p => p.Deleted == false && p.Gid == userID).Select(p => p.OrgID).Single();
                OrgID = userOrgId;
                gLevelOrgId = userOrgId;
                List<SelectListItem> memOrgs = new List<SelectListItem>();

                var memOrg = dbEntity.MemberOrganizations.Where(s => s.Deleted == false && s.Gid == userOrgId).Single();
                SelectListItem item = new SelectListItem();
                item.Value = userOrgId.ToString();
                item.Text = memOrg.FullName.Matter;
                item.Selected = true;//默认用户所属组织为已选中值
                memOrgs.Add(item);

                //获取当前登录用户所授权的组织
                var memPrivOrg = dbEntity.MemberPrivileges.Where(p => p.Deleted == false && p.UserID == userID && p.Ptype == 2 && p.Pstatus == 1).FirstOrDefault();
                if (memPrivOrg != null)
                {
                    var privOrgs = dbEntity.MemberPrivItems.Where(p => p.Deleted == false && p.PrivID == memPrivOrg.Gid).Select(p => p.RefID).ToList();
                    foreach (Guid? gid in privOrgs)
                    {
                        if (gid!=null)
                        {
                            if (gid == userOrgId)
                            {
                                continue;
                            }
                            memOrg = dbEntity.MemberOrganizations.Where(s => s.Deleted == false && s.Gid == gid).Single();
                            item = new SelectListItem();
                            item.Value = gid.ToString();
                            item.Text = memOrg.FullName.Matter;
                            memOrgs.Add(item);
                        }
                    }
                }
                ViewData["orgCode"] = memOrgs;
            }
            return View();
        }

        /// <summary>
        /// 级别列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult listLevel(SearchModel searchModel)
        {
            IQueryable<MemberLevel> levels = (from o in dbEntity.MemberLevels.Include("Name")
                                            where (o.Deleted == false && o.OrgID == OrgID)
                                            select o).AsQueryable();
            
            GridColumnModelList<MemberLevel> columns = new GridColumnModelList<MemberLevel>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Code);
            columns.Add(p => p.Name.Matter);
            columns.Add(p => p.Mlevel);
            columns.Add(p => p.Discount);

            GridData gridData = levels.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        public string DeleteLevel(Guid Gid)
        {
            var memLevel = (from o in dbEntity.MemberLevels
                             where (o.Gid == Gid && o.Deleted == false)
                             select o).Single();
            memLevel.Deleted = true;

            dbEntity.SaveChanges();

            return "success!";
        }

        public ActionResult LevelAdd()
        {

            MemberLevel memLevel = new MemberLevel { Name = NewResource(ModelEnum.ResourceType.STRING, OrgID) };
            ViewBag.exist = 0;
            return View("LevelEdit", memLevel);    
        }

        public ActionResult LevelEdit(Guid uGid)
        {
            MemberLevel memLevel = dbEntity.MemberLevels.Include("Name").Where(u => u.Gid == uGid).Single();
            memLevel.Name = RefreshResource(ModelEnum.ResourceType.STRING, memLevel.Name, OrgID);
            ViewBag.exist = 1;
            return View("LevelEdit", memLevel);  
        }

        public ActionResult LevelSave(MemberLevel model)
        {

            MemberLevel memLevel = (from u in dbEntity.MemberLevels
                                    where u.Deleted==false&&u.OrgID == gLevelOrgId && u.Code == model.Code
                                    select u).SingleOrDefault();
            if (memLevel == null)
            {
                memLevel = new MemberLevel();
                memLevel.Name = new GeneralResource(ModelEnum.ResourceType.STRING, model.Name);
                dbEntity.MemberLevels.Add(memLevel);
            }
            else
            {
                memLevel.Name.SetResource(ModelEnum.ResourceType.STRING, model.Name);
            }
            memLevel.Code = model.Code;
            memLevel.OrgID = gLevelOrgId;
            memLevel.Mlevel = model.Mlevel;
            memLevel.Discount = model.Discount;
            dbEntity.SaveChanges();
            return View("LevelEdit");        
        }
        
        #endregion


        #region Role

        /// <summary>
        /// 生成角色 树
        /// </summary>
        /// <returns></returns>
        public ActionResult MemberRoleTree()
        {
            return View();
        }





        /// <summary>
        /// 改变全局变量OrgID的值
        /// </summary>
        /// <param name="id">前台传入的组织ID值</param>
        public void GetID(Guid id)
        {
            OrgID = id;
        }

        /// <summary>
        /// 加载角色树
        /// </summary>
        /// <returns></returns>
        public string RoleTreeLoad()
        {
            string strTreeJson = "";
            //首次加载的默认组织的角色树
            if (OrgID.Equals(Guid.Empty))
            {
                Guid guserId = (Guid)CurrentSession.UserID;
                //MemberUser表中读出该用户的默认组织，MemberRole表中读出该组织的角色
                var RoleList = (from o in dbEntity.MemberUsers
                                from p in dbEntity.MemberRoles
                                where (p.OrgID == o.OrgID && o.Gid == guserId && o.Deleted == false & p.Deleted == false && p.Parent == null) select p).ToList();
                List<LiveTreeNode> list = new List<LiveTreeNode>();
                foreach (var item in RoleList)
                {
                    LiveTreeNode node = new LiveTreeNode();
                    node.id = item.Gid.ToString();
                    node.name = string.Concat(item.Name.GetResource(CurrentSession.Culture), "(" + item.Code + ")");
                    if (item.ChildItems.Count > 0) node.isParent = true;
                    else node.isParent = false;
                    node.nodes = new List<LiveTreeNode>();
                    if (item.Code == "Internal"||item.Code=="Public") node.progUrl = "0";
                    else node.progUrl = "a";
                    list.Add(node);
                }
                strTreeJson = CreateTree(list);
            }
            //用户选择其他组织
            else
            {
                var RoleList = (from p in dbEntity.MemberRoles.Include("Name").Include("ChildItems") where (p.OrgID == OrgID && p.Parent == null && p.Deleted == false) select p).ToList();
                List<LiveTreeNode> list = new List<LiveTreeNode>();
                foreach (var item in RoleList)
                {
                    LiveTreeNode node = new LiveTreeNode();
                    node.id = item.Gid.ToString();
                    node.name = string.Concat(item.Name.GetResource(CurrentSession.Culture), "(" + item.Code + ")");
                    if (item.ChildItems.Count > 0) node.isParent = true;
                    else node.isParent = false;
                    if (item.Code == "Internal"||item.Code=="Public") node.progUrl = "0";
                    else node.progUrl = "a";
                    node.nodes = new List<LiveTreeNode>();
                    list.Add(node);
                }
                strTreeJson = CreateTree(list);
            }
            return strTreeJson;
        }

        /// <summary>
        /// 异步展开树
        /// </summary>
        /// <param name="id">展开节点的id</param>
        /// <returns></returns>
        public string RoleTreeExpand(Guid id)
        {
            //bool flag = false;
            ////判断id是组织的id还是角色的id
            //Guid guserId = (Guid)CurrentSession.UserID;
            ////该用户的默认组织的ID
            //Guid gId = (from o in dbEntity.MemberUsers where (o.Gid == guserId && o.Deleted == false) select o.OrgID).Single();
            //var gOrgIDList = (from o in dbEntity.MemberOrganizations
            //                  where (o.Deleted == false)
            //                  select o).ToList();
            ////所有的组织ID组成一个数组
            //Guid[] gIdArray = new Guid[gOrgIDList.Count + 1];
            //for (int i = 0; i < gOrgIDList.Count; i++)
            //{
            //    gIdArray[i] = (Guid)gOrgIDList[i].Gid;
            //}
            //gIdArray[gOrgIDList.Count] = gId;
            ////判断前台传入的ID是否是组织ID
            //for (int i = 0; i < gIdArray.Length; i++)
            //{
            //    if (id == gIdArray[i])
            //    {
            //        flag = true;
            //        break;
            //    }
            //}
            List<LiveTreeNode> list = new List<LiveTreeNode>();
            ////站开点为root
            if (id.Equals(Guid.Empty))
            {
                var RoleList = (from o in dbEntity.MemberRoles.Include("Name").Include("ChildItems") where (o.OrgID == id && o.Deleted == false&&o.Parent==null) select o).ToList();
                foreach (var item in RoleList)
                {
                    LiveTreeNode node = new LiveTreeNode();
                    node.id = item.Gid.ToString();
                    node.name = string.Concat(item.Name.GetResource(CurrentSession.Culture), "(" + item.Code + ")");
                    if (item.Code == "Internal"||item.Code=="Public") node.progUrl = "0";
                    else node.progUrl = "a";
                    if (item.ChildItems.Count > 0) node.isParent = true;
                    else node.isParent = false;
                    node.nodes = new List<LiveTreeNode>();
                    list.Add(node);
                }
            }
            //展开点为角色
            else
            {
                MemberRole Role = (from o in dbEntity.MemberRoles.Include("Name").Include("ChildItems") where (o.Gid == id && o.Deleted == false) select o).Single();
                foreach (var item in Role.ChildItems)
                {
                    if (item.Deleted == false)
                    {
                        LiveTreeNode node = new LiveTreeNode();
                        node.id = item.Gid.ToString();
                        node.name = string.Concat(item.Name.GetResource(CurrentSession.Culture), "(" + item.Code + ")");
                        if (item.Code == "Supervisor") node.progUrl = "1";
                        else node.progUrl = "a";
                        if (item.ChildItems.Count > 0) node.isParent = true;
                        else node.isParent = false;
                        node.nodes = new List<LiveTreeNode>();
                        list.Add(node);
                    }
                }
            }
            return list.ToJsonString();
        }


        /// <summary>
        /// 右键 添加角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddNewRole(Guid id)
        {
            MemberRole oMemberRole = new MemberRole
            {
                Name = NewResource(ModelEnum.ResourceType.STRING,OrgID)
            };
            if (id.Equals(Guid.Empty))
            {
                oMemberRole.aParent = null;
            }
            else
            {
                MemberRole oParentRole = (from o in dbEntity.MemberRoles where (o.Gid == id) select o).Single();
                oMemberRole.aParent = id;
                oMemberRole.Parent = oParentRole;
            }
            var oRoleType = (from o in dbEntity.GeneralStandardCategorys.Include("Name") where (o.Deleted == false && o.Ctype == 2) select o).ToList();
            int noRoleType = oRoleType.Count;
            List<SelectListItem> list = new List<SelectListItem>();
            for (int i = 0; i < noRoleType; i++)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = oRoleType[i].Name.GetResource(CurrentSession.Culture),
                    Value = oRoleType[i].Gid.ToString()

                };
                list.Add(item);
            }
            ViewBag.olist = list;
            return View("MemberRoleAdd",oMemberRole);
        }

        /// <summary>
        /// 保存添加的角色到数据库
        /// </summary>
        /// <param name="memberRole"></param>
        /// <returns></returns>
        [HttpPost]
        public string AddRole(MemberRole memberRole)
        {
            int nCulture = CurrentSession.Culture;
            Guid guserId = (Guid)CurrentSession.UserID;
            string strcode = memberRole.Code;
            //检验角色代码是否重复
            bool flag = false;
            var Role = (from o in dbEntity.MemberRoles where (o.OrgID == OrgID) select o).ToList();
            foreach (var item in Role)
            {
                if (strcode == item.Code&&item.Deleted == false)
                {
                    flag = true;
                    return "Code Error";
                }
                else if (strcode == item.Code && item.Deleted == true)
                {
                    item.Deleted = false;
                    MemberRole oMemberRole = new MemberRole {Name=new GeneralResource(ModelEnum.ResourceType.STRING,memberRole.Name) };
                    oMemberRole.Rtype = memberRole.Rtype;
                    oMemberRole.OrgID = memberRole.OrgID;
                    oMemberRole.Code = memberRole.Code;
                    oMemberRole.aParent = memberRole.aParent;
                    oMemberRole.Remark = memberRole.Remark;
                    dbEntity.SaveChanges();
                    return "success";
                }
            }
            if (flag == false)
            {
                MemberRole oMemberRole = new MemberRole
                {
                    Name = new GeneralResource(ModelEnum.ResourceType.STRING, memberRole.Name)
                };
                oMemberRole.Rtype = memberRole.Rtype;
                oMemberRole.Remark = memberRole.Remark;
                oMemberRole.OrgID = OrgID;
                oMemberRole.Code = memberRole.Code;
                oMemberRole.aParent = memberRole.aParent;

                dbEntity.MemberRoles.Add(oMemberRole);

                dbEntity.SaveChanges();

                flag = false;
            }
            return "success";
        }

        /// <summary>
        /// 右键编辑角色
        /// </summary>
        /// <param name="id">要编辑角色的id</param>
        /// <returns></returns>
        public ActionResult RoleEdit(Guid id)
        {
            MemberRole oMemberRole = new MemberRole();
            oMemberRole = dbEntity.MemberRoles.Include("Parent").Where(o => o.Gid == id && o.Deleted == false).Single();
            oMemberRole.Name = RefreshResource(ModelEnum.ResourceType.STRING, oMemberRole.Name);
            var oRoleType = (from o in dbEntity.GeneralStandardCategorys.Include("Name") where (o.Deleted == false && o.Ctype == 2) select o).ToList();
            int noRoleType = oRoleType.Count;
            List<SelectListItem> list = new List<SelectListItem>();
            for (int i = 0; i < noRoleType; i++)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = oRoleType[i].Name.GetResource(CurrentSession.Culture),
                    Value = oRoleType[i].Gid.ToString()

                };
                list.Add(item);
            }
            ViewBag.olist = list;
            return View("MemberRoleEdit",oMemberRole);
        }

        /// <summary>
        /// 保存编辑后的节点
        /// </summary>
        /// <param name="memberrole"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditRole(MemberRole memberrole)
        {
            Guid guserId = (Guid)CurrentSession.UserID;
            MemberRole oMem = (from o in dbEntity.MemberRoles where (o.Gid == memberrole.Gid && o.Deleted == false) select o).Single();
            oMem.Code = memberrole.Code;
            oMem.Rtype = memberrole.Rtype;
            oMem.Remark = memberrole.Remark;
            if (OrgID.Equals(Guid.Empty))
            {
                Guid gorgID = (from o in dbEntity.MemberUsers where (o.Gid == guserId && o.Deleted == false) select o.OrgID).Single();
                oMem.OrgID = gorgID;
            }
            else oMem.OrgID = OrgID;
            if (memberrole.aName != null)
            {
                oMem.Name.SetResource(ModelEnum.ResourceType.STRING, memberrole.Name);
            }
            dbEntity.SaveChanges();
            return RedirectToAction("MemberRole");
       
        }

        /// <summary>
        /// 删除选中节点
        /// </summary>
        /// <param name="id">删除节点的id</param>
        public void DelTreeNode(Guid id)
        {
            MemberRole oMemberRole = (from o in dbEntity.MemberRoles.Include("ChildItems") where (o.Gid == id && o.Deleted == false) select o).Single();
            oMemberRole.Deleted = true;
            DelChildRole(oMemberRole.ChildItems.ToList<MemberRole>());
            dbEntity.SaveChanges();
        }
        
        /// <summary>
        /// 递归删除树节点
        /// </summary>
        /// <param name="list"></param>
        public void DelChildRole(List<MemberRole> list)
        {
            int nlist = list.Count;
            for (int i = 0; i < nlist; i++)
            {
                if (list[i].ChildItems.Count > 0)
                {
                    list[i].Deleted = true;
                    DelChildRole(list[i].ChildItems.ToList<MemberRole>());
                }
                else
                    list[i].Deleted = true;
            }
        }
        #endregion

        #region 用户积分
        public ActionResult Point()
        {
            return View();
        }
        #endregion 用户积分
    }
}
