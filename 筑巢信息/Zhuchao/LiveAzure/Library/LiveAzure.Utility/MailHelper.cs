using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text.RegularExpressions;

namespace LiveAzure.Utility
{
    /// <summary>
    /// 使用SMTP发送电子邮件
    /// </summary>
    public static class MailHelper
    {
        /// <summary>
        /// 验证EMail(true:成功 false:失败)
        /// </summary>
        /// <param name="fString">处理的字符串</param>
        public static bool CheckEmail(string fString)
        {
            return Regex.IsMatch(fString, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="sReceipt">单个收件人</param>
        /// <param name="sSubject">主题</param>
        /// <param name="sBody">内容</param>
        /// <returns>返回代码 0没收件人/发件人; -1收件人地址不正确; 1成功</returns>
        public static int SendMail(string sReceipt, string sSubject, string sBody)
        {
            List<string> oList = new List<string>();
            oList.Add(sReceipt);
            return SendMail(ConfigHelper.SmtpConfig.Sender, ConfigHelper.SmtpConfig.From, oList, sSubject, sBody, null);
        }

        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="oReceiptList">多个收件人</param>
        /// <param name="sSubject">主题</param>
        /// <param name="sBody">内容</param>
        /// <returns>返回代码 0没收件人/发件人; -1收件人地址不正确; 1成功</returns>
        public static int SendMail(List<string> oReceiptList, string sSubject, string sBody)
        {
            return SendMail(ConfigHelper.SmtpConfig.Sender, ConfigHelper.SmtpConfig.From, oReceiptList, sSubject, sBody, null);
        }

        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="oReceiptList">收件人列表</param>
        /// <param name="sSubject">主题</param>
        /// <param name="sBody">内容</param>
        /// <param name="oAttachmentList">附件列表</param>
        /// <returns>返回代码 0没收件人/发件人; -1收件人地址不正确; 1成功</returns>
        public static int SendMail(List<string> oReceiptList, string sSubject, string sBody, List<string> oAttachmentList)
        {
            return SendMail(ConfigHelper.SmtpConfig.Sender, ConfigHelper.SmtpConfig.From, oReceiptList, sSubject, sBody, oAttachmentList);
        }
        
        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="sSendName">发送者显示名称</param>
        /// <param name="sFromEmail">发件人地址</param>
        /// <param name="sReceipt">收件人</param>
        /// <param name="sSubject">主题</param>
        /// <param name="sBody">内容</param>
        /// <returns>返回代码 0没收件人/发件人; -1收件人地址不正确; 1成功</returns>
        public static int SendMail(string sSendName, string sFromEmail, string sReceipt, string sSubject, string sBody)
        {
            List<string> oList = new List<string>();
            oList.Add(sReceipt);
            return SendMail(sSendName, sFromEmail, oList, sSubject, sBody, null);
        }

        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="sSendName">发送者显示名称</param>
        /// <param name="sFromEmail">发件人地址</param>
        /// <param name="oReceiptList">收件人列表</param>
        /// <param name="sSubject">主题</param>
        /// <param name="sBody">内容</param>
        /// <param name="oAttachmentList">附件列表</param>
        /// <returns>返回代码 0没收件人/发件人; -1收件人地址不正确; 1成功</returns>
        public static int SendMail(string sSendName, string sFromEmail, List<string> oReceiptList, string sSubject, string sBody, List<string> oAttachmentList)
        {
            if (String.IsNullOrEmpty(sFromEmail) || (oReceiptList == null) || (oReceiptList.Count == 0) ||
                String.IsNullOrEmpty(sSubject) || String.IsNullOrEmpty(sBody))
                return 0;
            try
            {
                // 邮件主体
                MailMessage oMessage = new MailMessage();
                if (String.IsNullOrEmpty(sSendName))
                    oMessage.From = new MailAddress(sFromEmail);
                else
                    oMessage.From = new MailAddress(sFromEmail, sSendName);
                foreach (string sOneReceipt in oReceiptList)
                {
                    if (CheckEmail(sOneReceipt))
                        oMessage.To.Add(sOneReceipt);
                    else
                        return -1;
                }
                oMessage.Subject = sSubject;
                oMessage.SubjectEncoding = Encoding.UTF8;
                oMessage.Body = sBody;
                oMessage.BodyEncoding = Encoding.UTF8;
                oMessage.IsBodyHtml = true;
                oMessage.Priority = MailPriority.Normal;
                // 附件
                if (oAttachmentList != null)
                {
                    foreach (string sOneFileName in oAttachmentList)
                    {
                        Attachment oAttachFile = new Attachment(sOneFileName, MediaTypeNames.Application.Octet);
                        // ContentDisposition oDisposition = oAttachFile.ContentDisposition;
                        // oDisposition.CreationDate = System.IO.File.GetCreationTime(sOneFileName);
                        // oDisposition.ModificationDate = System.IO.File.GetLastWriteTime(sOneFileName);
                        // oDisposition.ReadDate = System.IO.File.GetLastAccessTime(sOneFileName);
                        oMessage.Attachments.Add(oAttachFile);
                    }
                }
                // 发送
                SmtpClient oSmtpClient = new SmtpClient();
                oSmtpClient.Host = ConfigHelper.SmtpConfig.Host;
                oSmtpClient.Credentials = new NetworkCredential(ConfigHelper.SmtpConfig.Username, ConfigHelper.SmtpConfig.Password);
                oSmtpClient.Port = ConfigHelper.SmtpConfig.Port;
                oSmtpClient.EnableSsl = ConfigHelper.SmtpConfig.EnableSsl;
                oSmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                oSmtpClient.Send(oMessage);
                // 清理垃圾
                foreach (Attachment oAttachFile in oMessage.Attachments)
                    oAttachFile.Dispose();
                oMessage.Dispose();
                oSmtpClient.Dispose();
            }
            catch
            {
                return 0;
            }
            return 1;
        }
    }
}
