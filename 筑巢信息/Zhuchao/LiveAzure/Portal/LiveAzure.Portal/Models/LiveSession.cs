using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LiveAzure.Models.Member;

namespace LiveAzure.Portal.Models
{
    public class LiveSession
    {
        public const string LOGINNAME = "LoginName";
        public static Guid userID;//登陆用户的ID
        public static string channelCode = "Web01";//渠道代码
        public static Guid channelID;
        public static string currencyCode = "¥";//货币代码
        public static Guid currencyID;
        public static int Culture = 2052;
        public static string LoginName
        {
            get
            {
                if (HttpContext.Current.Session[LOGINNAME] != null)
                    return HttpContext.Current.Session[LOGINNAME].ToString();
                else
                    return "UserNoExist";
            }
            set
            {
                HttpContext.Current.Session[LOGINNAME] = value;
            }
        }
        public static bool IsLoggedIn
        {
            get
            {
                object obj = HttpContext.Current.Session[LOGINNAME];
                return obj != null;
            }
        }
        /// <summary>
        /// 登陆到Session
        /// </summary>
        /// <param name="loginName">登陆名</param>
        public static void Login(string loginName)
        {
            LoginName = loginName;            
        }
        public static void UserID(Guid userId,Guid cId,Guid mId){
            userID = userId;
            channelID = cId;
            currencyID = mId;
        }
        /// <summary>
        /// 注销到Session
        /// </summary>
        public static void Logoff()
        {
            LoginName = null;
        }
    }
}