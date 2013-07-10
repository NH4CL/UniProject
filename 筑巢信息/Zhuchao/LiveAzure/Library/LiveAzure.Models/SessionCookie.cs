using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveAzure.Utility;

namespace LiveAzure.Models
{
    /// <summary>
    /// 存入Session的对象
    /// </summary>
    public class SessionData
    {
        /// <summary>
        /// 构造函数，生成随机密钥
        /// </summary>
        public SessionData()
        {
            this.aSaltKey = Utility.CommonHelper.RandomNumber(8);
            this.IsAdmin = false;
            this.IsLogin = false;
            this.ChannelCode = "Web01";
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="isAdmin">管理员身份</param>
        /// <param name="isLogin">是否登录</param>
        /// <param name="culture">语言</param>
        /// <param name="currency">货币</param>
        public SessionData(Guid userID, bool isAdmin, bool isLogin, int culture, Guid currency, Guid OrganizationID)
        {
            this.aSaltKey = Utility.CommonHelper.RandomNumber(8);
            this.UserID = userID;
            this.IsAdmin = isAdmin;
            this.IsLogin = isLogin;
            this.Culture = culture;
            this.Currency = currency;
            this.OrganizationGID = OrganizationID;
        }

        /// <summary>
        /// Session自己的名称
        /// </summary>
        public string SessionName { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid? UserID { get; set; }

        /// <summary>
        /// 当前语言
        /// </summary>
        public int Culture { get; set; }

        /// <summary>
        /// 当前货币
        /// </summary>
        /// <see cref="GeneralMeasureUnit"/>
        public Guid? Currency { get; set; }

        /// <summary>
        /// 是否超级管理员 0否 1是
        /// </summary>
        public bool IsAdmin { get; set; }
        
        /// <summary>
        /// 判断是否已登录
        /// </summary>
        public bool IsLogin { get; set; }

        /// <summary>
        /// 前台渠道代码，硬代码
        /// </summary>
        public string ChannelCode { get; set; }

        /// <summary>
        /// 前台渠道Guid
        /// </summary>
        public Guid ChannelGid { get; set; }

        /// <summary>
        /// 用户所属组织ID
        /// </summary>
        public Guid OrganizationGID { get; set; }
        /// <summary>
        /// 密钥
        /// </summary>
        private string aSaltKey;
        public string SaltKey
        {
            get { return aSaltKey; }
        }
    }

    /// <summary>
    /// Cookie数据结构
    /// </summary>
    public class CookieData
    {
        /// <summary>
        /// Cookie自己的名称
        /// </summary>
        public string CookieName { get; set; }

        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid? UserID { get; set; }

        /// <summary>
        /// 0不记住 1记住用户名 2记住用户名和登陆时间(自动登陆)
        /// </summary>
        public byte Remember { get; set; }

        /// <summary>
        /// 用User.SaltKey加密后的上次登陆时间，格式为yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string LastLoginTime { get; set; }
    }
}
