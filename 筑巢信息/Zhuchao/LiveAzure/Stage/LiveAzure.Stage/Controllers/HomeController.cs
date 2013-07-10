using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Collections;
using MVC.Controls.Grid;
using MVC.Controls;
using LiveAzure.Utility;
using LiveAzure.Models;
using LiveAzure.Models.General;
using LiveAzure.Models.Member;
using LiveAzure.BLL;

namespace LiveAzure.Stage.Controllers
{
    public class HomeController : BaseController
    {
        
        #region 初始化数据
        /// <summary>
        /// 控制器初始化
        /// </summary>
        /// <param name="requestContext"></param>
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
        }
        #endregion

        #region 登陆登出
        /// <summary>
        /// 初始登陆页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            CookieData oCookieData = CurrentCookie;//获取cookie
            if (oCookieData == null)//cookie为空则直接返回登陆页面
                return View();
            if (oCookieData.Remember == 2)//cookie.Remember=2 则直接自动登陆并记录session
            {
                MemberUser user = (from u in dbEntity.MemberUsers.Include("Role")
                                   where u.Deleted == false && u.Ustatus == (byte)ModelEnum.UserStatus.VALID
                                         && u.Gid == CurrentCookie.UserID
                                   select u).FirstOrDefault();
                if (user == null)
                    return View();

                //判断用户上次登陆时间是否一致
                string strLastLoginTime = CommonHelper.EncryptDES(user.LastLoginTime.ToString(), user.SaltKey);
                if (strLastLoginTime != oCookieData.LastLoginTime)
                    return View();
                DateTimeOffset tdLastLoginTime = DateTimeOffset.Parse(DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                user.LastLoginTime = tdLastLoginTime;//记录用户登陆时间
                dbEntity.SaveChanges();
                oCookieData.LastLoginTime = CommonHelper.EncryptDES(tdLastLoginTime.ToString(), user.SaltKey);//加密保存最后登陆时间
                if (user.Role.Code == "Supervisor")
                    CurrentSession = new SessionData(user.Gid, true, true, user.Culture.Culture, oGeneralBLL.GetDefaultCurrency(user.OrgID), user.OrgID);
                else
                    CurrentSession = new SessionData(user.Gid, false, true, user.Culture.Culture, oGeneralBLL.GetDefaultCurrency(user.OrgID), user.OrgID);
                CurrentCookie = oCookieData;
                //记录用户登录日志
                oEventBLL.WriteEvent("用户：" + user.LoginName + "登陆");
                return RedirectToAction("HomePage");
            }
            else if (oCookieData.Remember == 1)//若session.Remember=1 则返回登陆页面并显示cookie中上次登陆的用户名
            {
                MemberUser user = oGeneralBLL.getUser((Guid)oCookieData.UserID);
                if (user != null)
                    ViewBag.userLoginName = user.LoginName;
            }
            return View();
        }

        /// <summary>
        /// 验证用户登陆，并记录Session
        /// </summary>
        /// <param name="strUserName">输入的登陆名</param>
        /// <param name="strPassCode">输入的登陆密码明文</param>
        /// <returns>0:用户名密码为空;1:用户名不存在;2:用户密码错误;3:非内部用户;4:成功登陆</returns>
        public byte checkUser(string strUserName, string strPassCode, bool rememberLoginName = false, bool rememberUser = false)
        {
            byte nResult = 4;
            //如果接收的用户名和密码为空，返回0
            if (String.IsNullOrEmpty(strUserName) || String.IsNullOrEmpty(strPassCode))
            {
                nResult = 0;
            }
            else
            {
                //创建一个User实例
                MemberUser user = new MemberUser();
                //验证用户名
                try
                {
                    user = (from u in dbEntity.MemberUsers.Include("Role")
                            where u.Deleted == false && u.Ustatus == (byte)ModelEnum.UserStatus.VALID
                                  && u.LoginName == strUserName
                            select u).Single();
                }
                catch (Exception)
                {
                    user = null;
                    nResult = 1;
                }
                if (user != null)
                {
                    string _passcode = CommonHelper.EncryptDES(strPassCode, user.SaltKey);//加密密码密文
                    if (user.Passcode != _passcode)//验证密码密文
                    {
                        nResult = 2;
                    }
                    else if (!oGeneralBLL.IsInternal(user))
                    {
                        nResult = 3;
                    }
                    else
                    {
                        //登陆成功,记录session
                        if (user.Role.Code == "Supervisor")
                            CurrentSession = new SessionData(user.Gid, true, true, user.Culture.Culture, oGeneralBLL.GetDefaultCurrency(user.OrgID),user.OrgID);
                        else
                            CurrentSession = new SessionData(user.Gid, false, true, user.Culture.Culture, oGeneralBLL.GetDefaultCurrency(user.OrgID),user.OrgID);

                        //记录cookie
                        CookieData oCookieData = new CookieData();
                        DateTimeOffset tdLastLoginTime = DateTimeOffset.Parse(DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        user.LastLoginTime = tdLastLoginTime;//记录用户登陆时间
                        dbEntity.SaveChanges();
                        oCookieData.LastLoginTime = CommonHelper.EncryptDES(tdLastLoginTime.ToString(), user.SaltKey);//加密保存最后登陆时间
                        oCookieData.UserID = user.Gid;
                        byte isRemember = 0;
                        if (rememberUser == true)
                            isRemember = 2;
                        else if (rememberLoginName == true)
                            isRemember = 1;
                        oCookieData.Remember = isRemember;
                        CurrentCookie = oCookieData;
                        //记录用户登录日志
                        oEventBLL.WriteEvent("用户：" + user.LoginName + "登陆");
                    }
                }
            }
            return nResult;
        }

        /// <summary>
        /// 用户注销
        /// </summary>
        /// <returns></returns>
        public ActionResult Logoff()
        {
            //记录用户登录日志
            oEventBLL.WriteEvent("用户：" + CurrentSession.UserID + "注销");
            CurrentSession = new SessionData();
            CurrentCookie = new CookieData();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Error Page
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <returns></returns>
        public ActionResult ErrorPage(string message)
        {
            ViewBag.ErrorMessage = message;
            return View();
        }

        /// <summary>
        /// 后台Home页面
        /// </summary>
        /// <returns></returns>
        public ActionResult HomePage()
        {
            // 权限验证
            string strProgramCode = Request.RequestContext.RouteData.Values["Controller"].ToString() +
                Request.RequestContext.RouteData.Values["Action"].ToString();
            if (!base.Permission(strProgramCode))
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });

            MemberUser user = oGeneralBLL.getUser(CurrentSession.UserID);
            Session["userLoginName"] = user.LoginName;
            Session["systemCulture"] = oGeneralBLL.GetSupportCultures();
            if (CurrentSession.IsAdmin)
                Session["userOrganizationCulture"] = Session["systemCulture"];
            else
                Session["userOrganizationCulture"] = oGeneralBLL.GetSupportCultures(user.OrgID);
            return View();
        }

        /// <summary>
        /// 待办事项
        /// </summary>
        /// <returns></returns>
        public ActionResult HomePageIndex()
        {
            return View();
        }
        #endregion

        #region 方法
        /// <summary>
        /// 设置语言
        /// </summary>
        /// <param name="LCID">语言</param>
        /// <returns></returns>
        public ActionResult SetCulture(int LCID)
        {
            SessionData oSessionData = CurrentSession;
            oSessionData.Culture = LCID;
            CurrentSession = oSessionData;
            return RedirectToAction("HomePage");
        }

        /// <summary>
        /// 获取当前路径
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        public bool getCurrentPath(Guid programId)
        {
            bool result = false;
            if (programId != null)
            {
                Session["CurrentPath"] = oGeneralBLL.getPath(programId, CurrentSession.Culture);
                result = true;
            }
            return result;
        }

        public ActionResult ProgramShortCutList()
        {
            //根据当前用户查找到ProgramShortCutList
            List<MemberUserShortcut> listUserShortcut = dbEntity.MemberUserShortcuts.Include("Program").Where(u => u.Deleted == false && u.UserID == CurrentSession.UserID && u.Stype == 0).ToList();
            ViewBag.CurrentCul = CurrentSession.Culture;
            //返回这个modelList到页面 页面循环遍历后 按顺序列出
            return PartialView(listUserShortcut);
        }
        #endregion

        #region 导航树
        /// <summary>
        /// 返回生成树的json数据
        /// </summary>
        /// <returns></returns>
        public string TreeLoad()
        {
            return CreateTree(ListTreeNode(Guid.Empty));
        }

        /// <summary>
        /// 异步展开树节点，返回展开节点的json字符串
        /// </summary>
        /// <param name="id">展开树节点的guid</param>
        /// <returns></returns>
        public string TreeExpand(Guid id)
        {
            List<LiveTreeNode> nodes = ListTreeNode(id);
            return nodes.ToJsonString();
        }

        private List<LiveTreeNode> ListTreeNode(Guid id)
        {
            Guid? gNodeID = null;
            // 当展开root节点的时候
            if (id != Guid.Empty)
                gNodeID = id;

            List<GeneralProgram> oUserPrograms;
            if (CurrentSession.IsAdmin)
            {
                oUserPrograms = (from g in dbEntity.GeneralPrograms
                                 where g.Deleted == false && (gNodeID == null ? g.aParent == null : g.aParent == gNodeID)
                                 orderby g.Sorting descending
                                 select g).ToList();
            }
            else
            {
                List<Guid> fnFindProgs = dbEntity.Database.SqlQuery<Guid>("SELECT Gid FROM dbo.fn_FindFullPrograms({0})", CurrentSession.UserID).ToList();
                oUserPrograms = (from g in dbEntity.GeneralPrograms
                                 join p in fnFindProgs on g.Gid equals p
                                 where (gNodeID == null ? g.aParent == null : g.aParent == gNodeID)
                                 orderby g.Sorting descending
                                 select g).ToList();
            }
            List<LiveTreeNode> list = new List<LiveTreeNode>();
            foreach (var item in oUserPrograms)
            {
                LiveTreeNode treeNode = new LiveTreeNode();
                treeNode.id = item.Gid.ToString();
                treeNode.name = item.Name.GetResource(CurrentSession.Culture);
                treeNode.icon = "";
                treeNode.iconClose = "";
                treeNode.iconOpen = "";
                treeNode.nodeChecked = false;
                if (item.Terminal == true)
                    treeNode.isParent = false;
                else
                    treeNode.isParent = true;
                treeNode.progUrl = item.ProgUrl;
                treeNode.nodes = new List<LiveTreeNode>();
                list.Add(treeNode);
            }
            return list;
        }

        #endregion
    }
}
