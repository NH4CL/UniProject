using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveAzure.Models;
using LiveAzure.Utility;
using LiveAzure.Models.General;

namespace LiveAzure.BLL
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-10-24         -->
    /// <summary>
    /// 消息服务类
    /// </summary>
    public class MessageBLL : BaseBLL
    {
        /// <summary>
        /// 构造函数，必须传入数据库连接参数
        /// </summary>
        /// <param name="entity">数据库连接参数</param>
        public MessageBLL(LiveEntities entity) : base(entity) { }

        #region 手机短信

        /// <summary>
        /// 发送手机短信
        /// </summary>
        /// <param name="pReceivers">接收人手机号，可用逗号分隔最多20个手机号</param>
        /// <param name="pContent">短信内容，长短信最多970个字节，超出部分可能被舍弃</param>
        /// <returns>返回值，0表示发送成功</returns>
        public int SendSms(string pReceivers, string pContent)
        {
            EucpHelper oEucpHelper = new EucpHelper(ConfigHelper.EucpConfig.SerialNumber, ConfigHelper.EucpConfig.Password);
            int nResult = oEucpHelper.SendSms(pReceivers, pContent);
            return nResult;
        }

        /// <summary>
        /// 批量发送待发送短信，由定时器在特定时间段内（白天）调用
        /// </summary>
        /// <returns>发送短信数量</returns>
        public int SendPendingSms()
        {
            var oPending = (from p in dbEntity.GeneralMessagePendings
                            where p.Deleted == false
                                  && p.Mtype == (byte)ModelEnum.MessageType.SMS
                                  && p.Mstatus == (byte)ModelEnum.MessageStatus.PENDING
                            select p).ToList();
            EucpHelper oEucpHelper = new EucpHelper(ConfigHelper.EucpConfig.SerialNumber, ConfigHelper.EucpConfig.Password);
            int nCount = 0;
            foreach (var oMessage in oPending)
            {
                if (!String.IsNullOrEmpty(oMessage.Recipient) && !String.IsNullOrEmpty(oMessage.Matter))
                {
                    int nResult = oEucpHelper.SendSms(oMessage.Recipient, oMessage.Matter);
                    if (nResult == 0)
                    {
                        oMessage.Mstatus = (byte)ModelEnum.MessageStatus.SENDSUCCESS;
                        oMessage.SentTime = DateTimeOffset.Now;
                    }
                    else
                    {
                        oMessage.Mstatus = (byte)ModelEnum.MessageStatus.SENDFAILED;
                        oMessage.SentTime = DateTimeOffset.Now;
                        oMessage.Remark = String.Format("{0} Send Failed: {1}", oMessage.Remark, nResult);
                    }
                    nCount++;
                }
            }
            dbEntity.SaveChanges();
            return nCount;
        }

        /// <summary>
        /// 接收短信
        /// </summary>
        /// <returns>接收到的短信数量</returns>
        public int ReceiveSms()
        {
            EucpHelper oEucpHelper = new EucpHelper(ConfigHelper.EucpConfig.SerialNumber, ConfigHelper.EucpConfig.Password);
            List<object> oList = oEucpHelper.ReceiveSms();
            int nCount = 0;
            foreach (Dictionary<string, object> oMessage in oList)
            {
                GeneralMessageReceive oReceive = new GeneralMessageReceive
                {
                    SendFrom = oMessage["Mobile"].ToString(),
                    Matter = oMessage["Content"].ToString(),
                    SentTime = (DateTimeOffset)oMessage["SentTime"],
                    GetFrom = oMessage["Channel"].ToString()
                };
                dbEntity.GeneralMessageReceives.Add(oReceive);
                dbEntity.SaveChanges();
                nCount++;
            }
            return nCount;
        }

        #endregion

        #region 邮件

        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="sReceipt">收件人（仅一个地址）</param>
        /// <param name="sSubject">主题</param>
        /// <param name="sBody">内容</param>
        /// <returns>返回代码 0没收件人/发件人; -1收件人地址不正确; 1成功</returns>
        public int SendMail(string sReceipt, string sSubject, string sBody)
        {
            List<string> oList = new List<string>();
            oList.Add(sReceipt);
            return Utility.MailHelper.SendMail(oList, sSubject, sBody);
        }

        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="oReceiptList">收件人列表</param>
        /// <param name="sSubject">主题</param>
        /// <param name="sBody">内容</param>
        /// <returns>返回代码 0没收件人/发件人; -1收件人地址不正确; 1成功</returns>
        public int SendMail(List<string> oReceiptList, string sSubject, string sBody)
        {
            return Utility.MailHelper.SendMail(oReceiptList, sSubject, sBody, null);
        }

        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="oReceiptList">收件人列表</param>
        /// <param name="sSubject">主题</param>
        /// <param name="sBody">内容</param>
        /// <param name="oAttachmentList">附件列表</param>
        /// <returns>返回代码 0没收件人/发件人; -1收件人地址不正确; 1成功</returns>
        public int SendMail(List<string> oReceiptList, string sSubject, string sBody, List<string> oAttachmentList)
        {
            return Utility.MailHelper.SendMail(oReceiptList, sSubject, sBody, oAttachmentList);
        }

        /// <summary>
        /// 批量发送待发送邮件，由定时器调用
        /// </summary>
        /// <returns>发送邮件数量</returns>
        public int SendPendingMail()
        {
            var oPending = (from p in dbEntity.GeneralMessagePendings
                            where p.Deleted == false
                                  && p.Mtype == (byte)ModelEnum.MessageType.EMAIL
                                  && p.Mstatus == (byte)ModelEnum.MessageStatus.PENDING
                            select p).ToList();
            int nCount = 0;
            foreach (var oMessage in oPending)
            {
                if (!String.IsNullOrEmpty(oMessage.Recipient) && !String.IsNullOrEmpty(oMessage.Matter))
                {
                    int nResult = this.SendMail(oMessage.Recipient, oMessage.Title, oMessage.Matter);
                    if (nResult == 1)
                    {
                        oMessage.Mstatus = (byte)ModelEnum.MessageStatus.SENDSUCCESS;
                        oMessage.SentTime = DateTimeOffset.Now;
                    }
                    else
                    {
                        oMessage.Mstatus = (byte)ModelEnum.MessageStatus.SENDFAILED;
                        oMessage.SentTime = DateTimeOffset.Now;
                        oMessage.Remark = String.Format("{0} Send Failed: {1}", oMessage.Remark, nResult);
                    }
                    nCount++;
                }
            }
            dbEntity.SaveChanges();
            return nCount;
        }

        #endregion

    }
}
