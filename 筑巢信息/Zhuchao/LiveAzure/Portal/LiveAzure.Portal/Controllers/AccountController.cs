using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveAzure.Models.Member;
using LiveAzure.Utility;
using LiveAzure.Portal.Models;
using LiveAzure.Models.Order;
using LiveAzure.Models.Mall;
using LiveAzure.Models.General;
using LiveAzure.Models;


namespace LiveAzure.Portal.Controllers
{
    public class AccountController : BaseController
    {
        //
        // GET: /Account/
        private static string currencyCash = "RMB";//当前货币
        private static int addressNumber = 5;//用户可以建的地址数目
        private static Guid addressSelect;//编辑的地址
        private static Guid defaultValue=Guid.Empty;
        private static Guid? townSelect = null;
        #region 分页信息
        private static int pageSize=2;//每页显示n条信息
        private static int pageStart = 1;//从第n页开始显示
        private static int pages;//总页数 
        private static string tableNow;//当前使用的表
        public string page(int pNow,int pPages,string functionName)
        {
            string rString="";
            //第一页
            if (1 == pNow)
            {
                rString += "<font color=#cccccc >第一页</font>|";
            }
            else
            {
                rString = rString + "<a href=\"@Url.Content(\"/Account/"+functionName+"?pNow=1\")\">第一页</a>|";
            }
            if (1 == pNow)
            {
                rString += "<font color=#cccccc >上一页</font>";
            }
            else
            {
                rString = rString + "<a href=\"@Url.Content(\"/Account/" + functionName + "?pNow=" + (pNow - 1) + "\")\">上一页</a>";
            }
            //当前页
            if (0 == pPages)
                pNow = 0;
            rString += pNow;
            if (pNow < pPages)
            {
                rString = rString + "<a href=\"@Url.Content(\"/Account/" + functionName + "?pNow=" + (pNow + 1) + "\")\">下一页</a>";
            }
            else
            {
                rString += "<font color=#cccccc >下一页</font>";
            }
            //最后一页
            if (pPages == pNow)
            {
                rString += "|<font color=#cccccc >最后一页</font>";
            }
            else
            {
                rString = rString + "|<a href=\"@Url.Content(\"/Account/" + functionName + "?pNow=" + pPages + "\")\">最后一页</a>";
            }
            rString = rString + "总"+pages+"页";
            return rString;
        }
        public ViewResult Pages()
        {            
            ViewBag.page = 1;
            ViewBag.total = pages;
            return View();
        }
        public ViewResult First(int pNow)
        {  
            ViewBag.page = 1;
            ViewBag.total = pages;
            return View("Pages");
        }
        public ViewResult Tail(int pNow)
        {            
            ViewBag.page = pages;
            ViewBag.total = pages;
            return View("Pages");
        }
        public ViewResult Last(int pNow)
        {
            if (pNow - 1 < 1)
            {
                ViewBag.page = 1;
            }
            else
            {
                ViewBag.page = pNow - 1;
            }
            ViewBag.total = pages;
            return View("Pages");
        }
        public ViewResult Next(int pNow)
        {
            if (pNow + 1 > pages)
            {
                ViewBag.page = pages;
            }
            else
            {
                ViewBag.page = pNow + 1;
            }
            ViewBag.total = pages;
            return View("Pages");
        }
        #endregion
        public ActionResult Index()
        {
            string isLogin = LiveSession.LoginName;//判断是否登陆
            if (isLogin == "UserNoExist")
            {
                return RedirectToAction("LoginOrRegister");
            }
            return View();
        }
        /// <summary>
        /// 注册和登录显示在同一个页面
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginOrRegister()
        {
            return View();
        }
        #region 我的订单
        public ActionResult MyOrder(int pNow=1)
        {           
          
            return View();
        }
        public ActionResult orderTable(string type,int pNow = 1)
        {
            List<OrderInformation> userOrderListTotal = dbEntity.OrderInformations.Include("User").Where(p => p.Deleted == false && p.User.LoginName == LiveSession.LoginName).OrderByDescending(p => p.CreateTime).ToList();
            decimal listNumber = (decimal)userOrderListTotal.Count / (decimal)pageSize;
            pages = (int)Math.Ceiling(listNumber);
            List<OrderInformation> userOrderList=null;
            if (type == "f")//第一页
            {
                pageStart = 0;
                userOrderList = dbEntity.OrderInformations.Include("User").Where(p => p.Deleted == false && p.User.LoginName == LiveSession.LoginName).OrderByDescending(p => p.CreateTime).Skip(pageStart * pageSize).Take(pageSize).ToList();
            }
            else if (type == "l")//上一页
            {
                pageStart = pNow - 2;
                userOrderList = dbEntity.OrderInformations.Include("User").Where(p => p.Deleted == false && p.User.LoginName == LiveSession.LoginName).OrderByDescending(p => p.CreateTime).Skip(pageStart * pageSize).Take(pageSize).ToList();
            }
            else if (type == "n")//下一页
            {
                pageStart = pNow;
                userOrderList = dbEntity.OrderInformations.Include("User").Where(p => p.Deleted == false && p.User.LoginName == LiveSession.LoginName).OrderByDescending(p => p.CreateTime).Skip(pageStart * pageSize).Take(pageSize).ToList();
            }
            else if (type == "t")//最后一页
            {
                pageStart = pages-1;
                userOrderList = dbEntity.OrderInformations.Include("User").Where(p => p.Deleted == false && p.User.LoginName == LiveSession.LoginName).OrderByDescending(p => p.CreateTime).Skip(pageStart * pageSize).Take(pageSize).ToList();
            }
            return View(userOrderList);
        }
        #endregion

        public ActionResult IndexContent()
        {
            return View();
        }
        #region 抵用券
        /// <summary>
        /// 抵用券
        /// </summary>
        /// <param name="pNow">当前页</param>
        /// <returns>数据列表</returns>
        public ActionResult MyCoupon(int pNow=1)
        {
            List<MemberPoint> pointListTotal = dbEntity.MemberPoints.Include("User").Include("Coupon").Where(p => p.Deleted == false && p.User.LoginName == LiveSession.LoginName&&p.Currency==null?true:(p.Coupon.Currency.Code==currencyCash)).OrderByDescending(p => p.CreateTime).ToList();
            decimal listNumber = (decimal)pointListTotal.Count / (decimal)pageSize;
            pages = (int)Math.Ceiling(listNumber);
            pageStart = pNow - 1;
            List<MemberPoint> pointList = dbEntity.MemberPoints.Include("User").Include("Coupon").Where(p => p.Deleted == false && p.User.LoginName == LiveSession.LoginName && p.Currency == null ? true : (p.Coupon.Currency.Code == currencyCash)).OrderByDescending(p => p.CreateTime).Skip(pageStart).Take(pageSize).ToList();
            List<MyCoupons> couponList = new List<MyCoupons>();
            foreach (MemberPoint item in pointList)
            {
                string code="";
                switch(item.RefType){
                    case 0:
                        {
                            code = dbEntity.OrderInformations.Where(p=>p.Deleted==false&&p.Gid==item.RefID).Select(p=>p.Code).FirstOrDefault();
                            break;
                        }
                    case 1:
                        {
                            code = dbEntity.PurchaseInformations.Where(p=>p.Deleted==false&&p.Gid==item.RefID).Select(p=>p.Code).FirstOrDefault();
                            break;
                        }
                    case 2:
                        {
                            code = dbEntity.WarehouseStockOuts.Where(p=>p.Deleted==false&&p.Gid==item.RefID).Select(p=>p.Code).FirstOrDefault();
                            break;
                        }
                    case 3:
                        {
                            code = dbEntity.ShippingEnvelopes.Where(p=>p.Deleted==false&&p.Gid==item.RefID).Select(p=>p.Code).FirstOrDefault();
                            break;
                        }
                    case 4:
                        {
                            code = dbEntity.FinancePayments.Where(p=>p.Deleted==false&&p.Gid==item.RefID).Select(p=>p.Code).FirstOrDefault();
                            break;
                        }
                    case 5:
                        {
                            code = dbEntity.OrderInformations.Where(p=>p.Deleted==false&&p.Gid==item.RefID).Select(p=>p.Code).FirstOrDefault();
                            break;
                        }
                    case 6:
                        {
                            code = dbEntity.WarehouseMovings.Where(p=>p.Deleted==false&&p.Gid==item.RefID).Select(p=>p.Code).FirstOrDefault();
                            break;
                        }
                    default:
                        {
                            code="";
                            break;
                        }
                }
                
                MyCoupons coup = new MyCoupons
                {
                    cCode = item.Coupon.Code,
                    Ptype = item.Ptype,
                    Pstatus = item.Pstatus,
                    Score = item.Score,
                    Remain = item.Remain,
                    Amount = item.Amount,
                    Balance = item.Balance,
                    StartTime = item.StartTime,
                    EndTime = item.EndTime,
                    Reason = item.Reason,
                    oCode = code,
                    MinCharge = item.MinCharge,
                    Cashier = item.Cashier,
                    OnceUse = item.OnceUse,
                };
                couponList.Add(coup);
            }
            ViewBag.pageDown = page(pNow, pages, "MyCoupon");
            return View(couponList);
        }
        
        /// <summary>
        /// 券绑定
        /// </summary>
        /// <param name="code">帐号</param>
        /// <param name="pwd">密码</param>
        /// <returns>绑定结果</returns>
        public bool CouponBind(string code,string pwd)
        {
            PromotionCoupon coupon = dbEntity.PromotionCoupons.Where(p => p.Deleted == false && p.Cstatus == 1 && p.Code == code && p.Passcode == pwd).FirstOrDefault();
            if (null == coupon)
            {
                return false;
            }
            MemberPoint oPoint = new MemberPoint
            {
                UserID = dbEntity.MemberUsers.Where(p => p.Deleted == false && p.LoginName == LiveSession.LoginName).Select(p => p.Gid).FirstOrDefault(),
                Ptype = 2,//抵用券
                Pstatus = 1,//有效
                Reason = "券绑定",
                PromID = coupon.PromID,
                CouponID = coupon.Gid,
                aCurrency = coupon.aCurrency,
                Amount = coupon.Amount,
                Balance = coupon.Amount,
                MinCharge = coupon.MinCharge,
                Cashier = coupon.Cashier,
                OnceUse = coupon.OnceUse,
                StartTime = coupon.StartTime,
                EndTime = coupon.EndTime,
                Remark = coupon.Remark,
            };
            dbEntity.MemberPoints.Add(oPoint);
            dbEntity.SaveChanges();
            return true;
        }
        #endregion
        #region 我的评论 未完成
        public ActionResult MyComment(int pNow=1)
        {
            List<MallArticle> articleListTotal = dbEntity.MallArticles.Where(p => p.Deleted == false && p.User.LoginName == LiveSession.LoginName).ToList();
            decimal listNumber = (decimal)articleListTotal.Count / (decimal)pageSize;
            pages = (int)Math.Ceiling(listNumber);
            pageStart = pNow - 1;
            List<MallArticle> articleList = dbEntity.MallArticles.Where(p => p.Deleted == false && p.User.LoginName == LiveSession.LoginName).OrderByDescending(p => p.CreateTime).Skip(pageStart).Take(pageSize).ToList();
            ViewBag.pageDown = page(pNow, pages, "MyComment");
            return View(articleList);
            
        }
        public ActionResult MyCommentLog(int pNow = 1)
        {
            return View();
        }
        public ActionResult Comment()
        {
            return View();
        }
        public ActionResult MyCommentIndex()
        {
            return View();
        }
        #endregion
        #region 订阅
        public ActionResult MySubscribe()
        {
            MemberSubscribe subscribe = dbEntity.MemberSubscribes.Include("User").Where(p => p.Deleted == false && p.User.LoginName == LiveSession.LoginName).FirstOrDefault();
            if (null == subscribe)
            {
                subscribe = new MemberSubscribe
                {
                    UserID = LiveSession.userID,
                    ShortMessage = true,
                    Email = true,
                    Bulletin = false,
                    OrderConfirm = false,
                    OrderDelivery = true,
                    ShortSupply = true,
                    Promotion = false,
                    Discount = false,
                    UnionNotice = false
                };
                dbEntity.MemberSubscribes.Add(subscribe);
                dbEntity.SaveChanges();
            }
            return View(subscribe);
        }
        #endregion
        #region 积分
        public ActionResult MyCredits(int pNow=1)
        {
            List<int> pointList = dbEntity.MemberPoints.Include("User").Where(p => p.Deleted == false && p.User.LoginName == LiveSession.LoginName && p.Ptype == 0).Select(p => p.Remain).ToList();
            if (null == pointList)
                ViewBag.pointTotal = 0;
            else
                ViewBag.pointTotal = pointList.Sum();
            /*-----log-----*/
            List<MemberUsePoint> logListTotal = (from a in dbEntity.MemberUsePoints
                                            join b in dbEntity.MemberPoints on a.PointID equals b.Gid
                                            where a.Deleted == false && b.Deleted == false && b.User.LoginName == LiveSession.LoginName && b.Ptype == 0
                                            select a).ToList();
            decimal listNumber;
            if (null == logListTotal)
            {
                listNumber = (decimal)0.0 / (decimal)pageSize;
            }
            else
            {
                listNumber = (decimal)logListTotal.Count / (decimal)pageSize;
            }
            pages = (int)Math.Ceiling(listNumber);
            pageStart = pNow - 1;
            List<MemberUsePoint> logList = (from a in dbEntity.MemberUsePoints
                                            join b in dbEntity.MemberPoints on a.PointID equals b.Gid
                                            where a.Deleted == false && b.Deleted == false && b.User.LoginName == LiveSession.LoginName && b.Ptype == 0
                                            select a).OrderByDescending(p=>p.CreateTime).Skip(pageStart).Take(pageStart).ToList();
            ViewBag.pageDown = page(pNow, pages, "MyCredits");
            return View(logList);
        }
        #endregion
        public ActionResult Left()
        {
            return View();
        }
        #region 个人信息
        public ActionResult Information()
        {
            MemberUser user = dbEntity.MemberUsers.Where(p => p.Deleted == false && p.LoginName == LiveSession.LoginName).FirstOrDefault();
            List<MemberUserAttribute> attributeList = dbEntity.MemberUserAttributes.Where(p => p.Deleted == false && p.UserID == LiveSession.userID).ToList();
            
            foreach (MemberUserAttribute item in attributeList)
            {
                if (item.Optional.Code == "IDE")
                {
                    ViewBag.identity = item.Matter;
                }
                else if (item.Optional.Code == "QQ")
                {
                    ViewBag.qq = item.Matter;
                }
                else if (item.Optional.Code == "MSN")
                {
                    ViewBag.msn = item.Matter;
                }
                else if (item.Optional.Code == "TRUENAME")
                {
                    ViewBag.trueName = item.Matter;
                }
            }
            
            return View(user);
        }
        public ActionResult SaveInformation(FormCollection information)
        {
            MemberUser user = dbEntity.MemberUsers.Where(p => p.Deleted == false && p.LoginName == LiveSession.LoginName).FirstOrDefault();
            user.NickName = information["nickName"];
            user.Gender =Convert.ToByte(information["sex"]);
            user.Birthday =DateTimeOffset.Parse(information["birthday"]);
            user.CellPhone = information["cphone"];
            user.Telephone = information["phone"];
            List<MemberUserAttribute> attributeList = dbEntity.MemberUserAttributes.Where(p => p.Deleted == false && p.UserID == LiveSession.userID).ToList();

            foreach (MemberUserAttribute item in attributeList)
            {
                if (item.Optional.Code == "IDE")
                {
                    item.Matter = information["userIDNumber"];
                }
                else if (item.Optional.Code == "QQ")
                {
                    item.Matter = information["QQ"];
                }
                else if (item.Optional.Code == "MSN")
                {
                    item.Matter = information["MSN"];
                }
                else if (item.Optional.Code == "TRUENAME")
                {
                    item.Matter = information["userName"];
                }
            }
            return RedirectToAction("Information");
        }
        #endregion
        #region 收货地址 
        public ActionResult Address()
        {
            List<MemberAddress> addressList = dbEntity.MemberAddresses.Include("Location").Where(p => p.Deleted == false && p.UserID == LiveSession.userID).ToList();
            ViewBag.addNum = addressList.Count;
            return View(addressList);
        }

        public ActionResult AddressAdd()
        {
            
            MemberAddress address = new MemberAddress
            {
                UserID = LiveSession.userID,
                IsDefault = true,
            };                       

            return View(address);
        }

        public ActionResult AddressAddSave(FormCollection address)
        {
            
            MemberAddress add = new MemberAddress();
            add.UserID = LiveSession.userID;
            add.Code = address["code"];
            if ("true" == address["isdefault"])
                add.IsDefault = true;
            else
                add.IsDefault = false;
            add.DisplayName = address["consignee"];
            add.aLocation =Guid.Parse(address["Select3"]);
            add.FullAddress = address["address"];
            add.PostCode = address["zipcode"];
            add.CellPhone = address["mobile"];
            add.HomePhone = address["tel"];
            add.Email = address["email"];
            dbEntity.MemberAddresses.Add(add);
            dbEntity.SaveChanges();
            return RedirectToAction("Address");
        }

        public ActionResult Select(Guid? pro, Guid? cit)
        {
            Guid china = dbEntity.GeneralRegions.Where(p => p.Deleted == false && p.Code == "CHN").Select(p => p.Gid).FirstOrDefault();
            ViewBag.province = RegionSelect(china,pro).Values.FirstOrDefault();
            Guid province=Guid.Empty;
            if (pro == null)
            {
                province = RegionSelect(china,pro).Keys.FirstOrDefault();
            }
            else
            {
                province =(Guid)pro;
            }
            ViewBag.city = RegionSelect(province,cit).Values.FirstOrDefault();
            Guid city = Guid.Empty;
            if (cit == null)
            {
                city = RegionSelect(province, cit).Keys.FirstOrDefault();
            }
            else
            {
                city = (Guid)cit;
            }
            ViewBag.town = RegionSelect(city,townSelect).Values.FirstOrDefault();
            townSelect = null;
            return View();
        }
        
        public Dictionary<Guid,List<SelectListItem>> RegionSelect(Guid par,Guid? def)
        {            
            List<SelectListItem> rSelect = new List<SelectListItem>();
            List<GeneralRegion> regionList = dbEntity.GeneralRegions.Where(p => p.Deleted == false && p.aParent == par).ToList();
            Guid first = Guid.Empty;
            if (def == null)
            {
                first = regionList.Select(p => p.Gid).FirstOrDefault();//默认选项
            }
            else
            {
                first =(Guid)def;
            }
            foreach (GeneralRegion item in regionList)
            {
                if (item.Gid == first)
                    rSelect.Add(new SelectListItem { Text = item.ShortName, Value = item.Gid.ToString(), Selected = true });
                else
                    rSelect.Add(new SelectListItem { Text = item.ShortName, Value = item.Gid.ToString()});
            }
            Dictionary<Guid, List<SelectListItem>> region = new Dictionary<Guid, List<SelectListItem>>();
            region[first] = rSelect;
            return region;
        }

        public ActionResult AddressEdit(Guid gid)
        {
            addressSelect = gid;
            MemberAddress edit = dbEntity.MemberAddresses.Where(p => p.Gid == gid).FirstOrDefault();
            townSelect = edit.aLocation;
            GeneralRegion town = dbEntity.GeneralRegions.Where(p => p.Deleted == false && p.Gid == townSelect).FirstOrDefault();
            ViewBag.cityID = town.aParent;
            GeneralRegion city = dbEntity.GeneralRegions.Where(p => p.Deleted == false && p.Gid == town.aParent).FirstOrDefault();
            ViewBag.provinceID = city.aParent;
            return View(edit);
        }

        public ActionResult AddressEditSave(FormCollection address)
        {
            MemberAddress add = dbEntity.MemberAddresses.Where(p => p.Deleted == false && p.Gid == addressSelect).FirstOrDefault();            
            add.Code = address["code"];
            if ("true" == address["isdefault"])
                add.IsDefault = true;
            else
                add.IsDefault = false;
            add.DisplayName = address["consignee"];
            add.aLocation = Guid.Parse(address["Select3"]);
            add.FullAddress = address["address"];
            add.PostCode = address["zipcode"];
            add.CellPhone = address["mobile"];
            add.HomePhone = address["tel"];
            add.Email = address["email"];            
            dbEntity.SaveChanges();
            return RedirectToAction("Address");
        }

        public ActionResult AddressDelete(Guid gid)
        {
            MemberAddress delete = dbEntity.MemberAddresses.Where(p => p.Gid == gid).FirstOrDefault();
            delete.Deleted = true;
            dbEntity.SaveChanges();
            return RedirectToAction("Address");
        }
        #endregion
        #region 密码
        public ActionResult Password()
        {
            return View();
        }
        public bool SavePassword(FormCollection pwd)
        {
            string newPwd = pwd["new_password1"];
            MemberUser user = dbEntity.MemberUsers.Where(p => p.Deleted == false && p.LoginName == LiveSession.LoginName).FirstOrDefault();
            newPwd = CommonHelper.EncryptDES(newPwd, user.SaltKey);
            user.Passcode = newPwd;
            dbEntity.SaveChanges();
            return true;
        }
        #endregion
        #region 我的级别
        public ActionResult MyLevel()
        {
            //MemberLevel level = dbEntity.MemberLevels.Where(p => p.Deleted == false && p.Code == LiveSession.userID).FirstOrDefault();
            return View();
        }
        #endregion
        public ActionResult Invitation()
        {
            return View();
        }
        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            return PartialView();
        }
        /// <summary>
        /// 注册页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            MemberUser model = new MemberUser();
            return PartialView(model);
        }

        #region 用户登录注册
        /// <summary>
        /// 检测用户是否存在
        /// </summary>
        /// <param name="loginName">登陆用户输入的用户名</param>
        [HttpPost]
        public JsonResult CheckUserExist(string loginName)
        {
            bool result;
            if (loginName == "" || loginName.Length > 128)
                result = false;
            else
                result = (from m in dbEntity.MemberUsers
                          where m.LoginName == loginName
                             && !m.Deleted
                          select m).Any();

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 用户登陆
        /// </summary>
        /// <param name="password">密码</param>
        /// <param name="loginName">用户名</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoginUser(string loginName, string password, bool rememberUserName = false, bool autoLogin = false)
        {
            Guid cID = dbEntity.MemberChannels.Where(p => p.Deleted == false && p.Code == LiveSession.channelCode).Select(p => p.Gid).FirstOrDefault();
            Guid mID = dbEntity.GeneralMeasureUnits.Where(p => p.Deleted == false && p.Code == LiveSession.currencyCode).Select(p => p.Gid).FirstOrDefault();
            Guid uID=Guid.Empty;//user guid
            int result = 0;//login success
            if (loginName == "") result = 1; // 用户名不合法
            else if (password == "") result = 2;//password is blank
            else
            {
                try
                {
                    MemberUser user = (from m in dbEntity.MemberUsers
                                       where m.LoginName == loginName
                                          && !m.Deleted
                                       select m).SingleOrDefault();
                    if (user == null) result = 3;//user is not exist
                    else
                    {
                        string strPassCode = CommonHelper.EncryptDES(password, user.SaltKey);//加密密码密文
                        if (user.Passcode != strPassCode) result = 4;//password is error
                        else
                        {
                            if (autoLogin)
                            {
                                LiveCookie.SetAutoLogin(loginName, strPassCode);
                            }
                            else
                            {
                                LiveCookie.ClearAutoLogin();
                                if (rememberUserName)
                                    LiveCookie.SetSaveName(loginName);
                                else
                                    LiveCookie.ClearSaveName();
                            }
                            uID = user.Gid;
                        }
                    }
                }
                catch
                {
                    result = 5;//login is failure
                }

            }
            if (result == 0)
            {
                LiveSession.Login(loginName);
                LiveSession.UserID(uID,cID,mID);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="email">邮箱</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RegisterUser(string loginName, string password, string email)
        {
            int result = 0;
            if (loginName == "" || loginName.Length < 6 || loginName.Length > 128) result = 1; //userName doesn't match the rules
            else if (password == "" || password.Length < 6 || password.Length > 100) result = 2; //password doesn't match the rules
            else if (email == "") result = 3; //email is null
            else
            {
                string saltKey = CommonHelper.RandomString(8);
                MemberUser newUser = new MemberUser
                {
                    LoginName = loginName,
                    SaltKey = saltKey,
                    Passcode = CommonHelper.EncryptDES(password, saltKey),
                    Email = email
                };
                dbEntity.MemberUsers.Add(newUser);
                dbEntity.SaveChanges();
                LiveCookie.SetSaveName(loginName);
            }
            if (result == 0)
            {
                LiveSession.Login(loginName);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion 用户登录注册

        #region 用户首页
        /// <summary>
        /// 列出当前登录用户的交易订单
        /// </summary>
        /// <returns></returns>
        public ActionResult UserOrderList(){
            MemberUser user = GetUser();
            if (user == null) return View("LoginOrRegister");
            IEnumerable<OrderInformation> model = (from o in dbEntity.OrderInformations
                                                   where o.UserID == user.Gid
                                                      && !o.Deleted
                                                   select o);
            return PartialView(model);
        }
        #endregion 用户首页
        #region
        /// <summary>
        /// 得到当前登录用户
        /// </summary>
        /// <returns></returns>
        public MemberUser GetUser()
        {
            MemberUser user = (from u in dbEntity.MemberUsers
                               where u.LoginName == LiveSession.LoginName
                                  && !u.Deleted
                               select u).SingleOrDefault();
            return user;
        }
        #endregion
    }
}
