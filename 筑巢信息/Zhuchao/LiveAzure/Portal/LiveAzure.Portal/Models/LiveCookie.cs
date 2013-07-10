using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveAzure.Portal.Models
{
    public class LiveCookie
    {
        public const string LOGIN = "Login";
        public const string AUTO = "Auto";
        public const string SAVE = "Save";
        public const string LOGIN_NAME = "LoginName";
        public const string PASSCODE = "Passcode";

        public static bool IsSaveLoginName
        {
            get
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[LOGIN];
                if (cookie == null)
                    return false;
                return cookie[SAVE] == "True";
            }
            set
            {
                if (!value)
                    LoginName = string.Empty;
                HttpContext.Current.Response.Cookies[LOGIN][SAVE] = value.ToString();
            }
        }
        /// <summary>
        /// 是否自动登陆
        /// </summary>
        public static bool IsAutoLogin
        {
            get
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[LOGIN];
                if (cookie == null)
                    return false;
                return cookie[AUTO] == "True";
            }
            set
            {
                if (value)
                {
                    HttpContext.Current.Response.Cookies[LOGIN][AUTO] = "True";
                }
                else
                {
                    Passcode = string.Empty;
                    Expires = DateTime.Now.AddDays(-1);
                }
            }
        }
        /// <summary>
        /// 登陆名
        /// </summary>
        public static string LoginName
        {
            get
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[LOGIN];
                if (cookie == null)
                    return string.Empty;
                return cookie[LOGIN_NAME];
            }
            set
            {
                HttpContext.Current.Response.Cookies[LOGIN][LOGIN_NAME] = value;
            }
        }
        /// <summary>
        /// 密码密文
        /// </summary>
        public static string Passcode
        {
            get
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[LOGIN];
                if (cookie == null)
                    return string.Empty;
                return cookie[PASSCODE];
            }
            set
            {
                HttpContext.Current.Response.Cookies[LOGIN][PASSCODE] = value;
            }
        }

        public static DateTime Expires
        {
            get
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[LOGIN];
                if (cookie == null)
                    return DateTime.Now;
                return cookie.Expires;
            }
            set
            {
                HttpContext.Current.Response.Cookies[LOGIN].Expires = value;
            }
        }
        /// <summary>
        /// 设置自动登陆
        /// </summary>
        /// <param name="loginName">登陆名</param>
        /// <param name="password">密码</param>
        /// <param name="expireDays">到期天数</param>
        public static void SetAutoLogin(string loginName, string passcode, int expireDays = 14)
        {
            IsAutoLogin = true;
            IsSaveLoginName = true;
            LoginName = loginName;
            Passcode = passcode;
            Expires = DateTime.Now.AddDays(expireDays);
        }
        public static void SetSaveName(string loginName,int expireDays = 14)
        {
            LoginName = loginName;            
            Passcode = string.Empty;
            IsSaveLoginName = true;
            Expires = DateTime.Now.AddDays(expireDays);
        }
        public static void ClearAutoLogin()
        {
            IsAutoLogin = false;
        }
        public static void ClearSaveName()
        {
            IsSaveLoginName = false;
        }
    }
}