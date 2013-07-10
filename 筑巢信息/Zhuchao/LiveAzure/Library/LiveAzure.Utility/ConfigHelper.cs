using System;
using System.Collections.Generic;
using System.Globalization;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;

namespace LiveAzure.Utility
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-06         -->
    /// <summary>
    /// 从xml中读取配置文件
    /// </summary>
    public static class ConfigHelper
    {
        #region 数据库连接

        /// <summary>
        /// 全局数据库连接
        /// </summary>
        public static class LiveConnection
        {
            private static DbConnection _Connection;

            /// <summary>
            /// 获取主数据库连接对象
            /// </summary>
            public static DbConnection Connection
            {
                get
                {
                    if (_Connection == null)
                        _Connection = new SqlConnection(LiveConnection.MainSqlString);
                    else if (String.IsNullOrEmpty(_Connection.ConnectionString))
                        _Connection = new SqlConnection(LiveConnection.MainSqlString);
                    return _Connection;
                }
            }

            /// <summary>
            /// 百胜OpenShop使用的MySQL连接字符串
            /// </summary>
            public static string MySqlString
            {
                get
                {
                    string strConfig = System.Configuration.ConfigurationManager.AppSettings["MySqlString"];
                    if (String.IsNullOrEmpty(strConfig))
                        return "";
                    else
                        return CommonHelper.DecryptDES(strConfig, EncodeKey.DefaultKey);
                }
            }

            /// <summary>
            /// SQL Server 数据库连接字符串，主数据库
            /// </summary>
            public static string MainSqlString
            {
                get
                {
                    string strConfig = System.Configuration.ConfigurationManager.AppSettings["SqlMainString"];
                    if (String.IsNullOrEmpty(strConfig))
                        return "";
                    else
                        return CommonHelper.DecryptDES(strConfig, EncodeKey.DefaultKey);
                }
            }

            /// <summary>
            /// SQL Server 数据库连接字符串，Web网站数据库
            /// </summary>
            public static string WebSqlString
            {
                get
                {
                    string strConfig = System.Configuration.ConfigurationManager.AppSettings["SqlWebString"];
                    if (String.IsNullOrEmpty(strConfig))
                        return "";
                    else
                        return CommonHelper.DecryptDES(strConfig, EncodeKey.DefaultKey);
                }
            }
        }

        #endregion

        #region 通用系统常量

        /// <summary>
        /// 定义全局通用常量，均为只读
        /// </summary>
        public static class GlobalConst
        {
            /// <summary>
            /// 是否调试状态
            /// </summary>
            public static bool IsDebug
            {
                get
                {
                    string strConfig = System.Configuration.ConfigurationManager.AppSettings["IsDebug"];
                    bool bResult = false;
                    if (!String.IsNullOrEmpty(strConfig))
                        bool.TryParse(strConfig, out bResult);
                    return bResult;
                }
            }

            /// <summary>
            /// 读取指定的xml配置参数
            /// </summary>
            /// <param name="sAddKey">xml中的key值</param>
            /// <returns>xml的值</returns>
            public static string GetSetting(string sAddKey)
            {
                return System.Configuration.ConfigurationManager.AppSettings[sAddKey];
            }

        }

        #endregion

        #region 加密密钥

        /// <summary>
        /// 通用DEC加密密钥，要求必须为8位数字
        /// </summary>
        public static class EncodeKey
        {
            /// <summary>
            /// 获取默认的DEC加密密钥
            /// </summary>
            public static string DefaultKey
            {
                get
                {
                    string strConfig = System.Configuration.ConfigurationManager.AppSettings["EncodeKeyIndex"];
                    int nResult = 0;
                    if (!String.IsNullOrEmpty(strConfig))
                        int.TryParse(strConfig, out nResult);
                    return EncryptKeys[nResult];
                }
            }

            /// <summary>
            /// 通用DEC加密密钥，要求必须为8位数字
            /// </summary>
            public static string[] EncryptKeys = { "20100101", "20091225", "20110530", "20101218" };
        }

        #endregion

        #region SMTP 配置

        /// <summary>
        /// SMTP 配置文件
        /// </summary>
        public static class SmtpConfig
        {
            /// <summary>
            /// SMTP Host
            /// </summary>
            public static string Host
            {
                get
                {
                    string strConfig = System.Configuration.ConfigurationManager.AppSettings["SmtpHost"];
                    if (String.IsNullOrEmpty(strConfig))
                        return "mail.zhuchao.com";
                    else
                        return strConfig;
                }
            }

            /// <summary>
            /// SMTP Username
            /// </summary>
            public static string Username
            {
                get
                {
                    string strConfig = System.Configuration.ConfigurationManager.AppSettings["SmtpUsername"];
                    if (String.IsNullOrEmpty(strConfig))
                        return "service@zhuchao.com";
                    else
                        return strConfig;
                }
            }

            /// <summary>
            /// SMTP Password 需要加/解密
            /// </summary>
            public static string Password
            {
                get
                {
                    string strConfig = System.Configuration.ConfigurationManager.AppSettings["SmtpPassword"];
                    if (String.IsNullOrEmpty(strConfig))
                        return "";
                    else
                        return CommonHelper.DecryptDES(strConfig, EncodeKey.DefaultKey);
                }
            }

            /// <summary>
            /// SMTP Port
            /// </summary>
            public static int Port
            {
                get
                {
                    string strConfig = System.Configuration.ConfigurationManager.AppSettings["SmtpPort"];
                    int nResult = 25;
                    if (!String.IsNullOrEmpty(strConfig))
                        int.TryParse(strConfig, out nResult);
                    return nResult;
                }
            }

            /// <summary>
            /// SMTP EnableSsl
            /// </summary>
            public static bool EnableSsl
            {
                get
                {
                    string strConfig = System.Configuration.ConfigurationManager.AppSettings["SmtpEnableSsl"];
                    bool bResult = false;
                    if (!String.IsNullOrEmpty(strConfig))
                        bool.TryParse(strConfig, out bResult);
                    return bResult;
                }
            }

            /// <summary>
            /// SMTP Sender's Name
            /// </summary>
            public static string Sender
            {
                get
                {
                    string strConfig = System.Configuration.ConfigurationManager.AppSettings["SmtpSender"];
                    if (String.IsNullOrEmpty(strConfig))
                        return "筑巢家居商城";
                    else
                        return strConfig;
                }
            }

            /// <summary>
            /// SMTP From Email
            /// </summary>
            public static string From
            {
                get
                {
                    string strConfig = System.Configuration.ConfigurationManager.AppSettings["SmtpFrom"];
                    if (String.IsNullOrEmpty(strConfig))
                        return "service@zhuchao.com";
                    else
                        return strConfig;
                }
            }
        }

        #endregion

        #region 亿美软通短信参数

        /// <summary>
        /// 亿美软通短信参数设置
        /// </summary>
        public static class EucpConfig
        {
            /// <summary>
            /// 亿美软通序列号
            /// </summary>
            public static string SerialNumber
            {
                get
                {
                    string strConfig = System.Configuration.ConfigurationManager.AppSettings["EucpSerialNumber"];
                    if (String.IsNullOrEmpty(strConfig))
                        return "3SDK-EMS-0130-LHWNL";
                    else
                        return strConfig;
                }
            }

            /// <summary>
            /// 亿美软通密码，需要加/解密
            /// </summary>
            public static string Password
            {
                get
                {
                    string strConfig = System.Configuration.ConfigurationManager.AppSettings["EucpPassword"];
                    if (String.IsNullOrEmpty(strConfig))
                        return "";
                    else
                        return CommonHelper.DecryptDES(strConfig, EncodeKey.DefaultKey);
                }
            }
        }

        #endregion
    }

}
