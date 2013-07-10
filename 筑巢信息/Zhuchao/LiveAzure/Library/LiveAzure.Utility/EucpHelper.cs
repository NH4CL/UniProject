using System;
using System.Collections.Generic;

namespace LiveAzure.Utility
{
    /// <summary>
    /// 手机短信平台，仅支持亿美软通EUCP WebService协议
    /// </summary>
    /// <example>
    ///     EucpHelper oEucpHelper = new EucpHelper("3SDK-EMS-0130-LHWNL", "890526");
    ///     int nResult = oEucpHelper.SendSms("13800000000", "短信内容，最多970字节");
    /// </example>
    public class EucpHelper
    {

        private string SerialNumber { get; set; }
        private string Password { get; set; }

        private EucpService.SDKService objEucpService;

        /// <summary>
        /// 构造函数，亿美软通的序列号和密码从配置文件中读取
        /// </summary>
        public EucpHelper()
        {
            SerialNumber = ConfigHelper.EucpConfig.SerialNumber;   // 从配置文件中读取加密串
            Password = ConfigHelper.EucpConfig.Password;
            objEucpService = new EucpService.SDKService();
        }

        /// <summary>
        /// 构造函数，需提供亿美软通的序列号和密码
        /// </summary>
        /// <param name="pSerialNumber">序列号</param>
        /// <param name="pPassword">密码</param>
        public EucpHelper(string pSerialNumber, string pPassword)
        {
            SerialNumber = pSerialNumber.Trim();
            Password = pPassword.Trim();
            objEucpService = new EucpService.SDKService();
        }

        /// <summary>
        /// 析构函数，清理对象
        /// </summary>
        ~EucpHelper()
        {
            objEucpService.Dispose();
            objEucpService = null;
            GC.Collect();
        }

        /// <summary>
        /// 发送手机短信
        /// </summary>
        /// <param name="pReceivers">接收人手机号，可用逗号分隔最多20个手机号</param>
        /// <param name="pContent">短信内容，长短信最多970个字节，超出部分可能被舍弃</param>
        /// <returns>返回值，0表示发送成功</returns>
        public int SendSms(string pReceivers, string pContent)
        {
            return objEucpService.sendSMS(this.SerialNumber, this.Password, null,
                pReceivers.Trim().Split(new char[] { ',' }), pContent.Trim(), null, "GBK", 5);
        }

        /// <summary>
        /// 接收短信
        /// </summary>
        /// <returns>列表包含多条短信，用Dictionary存储每条短信</returns>
        /// <example>
        ///     foreach (Dictionary<string, object> oMessage in oList)
        ///     {
        ///         (DateTime)oMessage["SentTime"]
        ///         oMessage["Mobile"].ToString()
        ///         oMessage["Content"].ToString()
        ///     }
        /// </example>
        public List<object> ReceiveSms()
        {
            List<object> oResult = new List<object>();
            EucpService.mo[] listMo = objEucpService.getMO(this.SerialNumber, this.Password);
            if (listMo != null)
            {
                foreach (EucpService.mo model in listMo)
                {
                    Dictionary<string, object> oMessage = new Dictionary<string, object>();
                    DateTimeOffset dtSentTime = DateTimeOffset.Now;
                    DateTimeOffset.TryParseExact(model.sentTime, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out dtSentTime);
                    oMessage.Add("SentTime", dtSentTime);
                    oMessage.Add("Mobile", model.mobileNumber);
                    oMessage.Add("Content", model.smsContent);
                    oMessage.Add("Channel", model.channelnumber);
                    oResult.Add(oMessage);
                }
            }
            return oResult;
        }

        /// <summary>
        /// 获取余额
        /// </summary>
        /// <returns>余额值</returns>
        public double GetBalance()
        {
            return objEucpService.getBalance(this.SerialNumber, this.Password);
        }

        /// <summary>
        /// 获取每条短信的价格，长短信按每70个字节作为一条计价
        /// </summary>
        /// <returns>单价</returns>
        public double GetEachFee()
        {
            return objEucpService.getEachFee(this.SerialNumber, this.Password);
        }

        /// <summary>
        /// 充值
        /// </summary>
        /// <param name="pCardNumber">充值卡号</param>
        /// <param name="pCardPassword">充值卡密码</param>
        /// <returns></returns>
        public int ChargeUp(string pCardNumber, string pCardPassword)
        {
            return objEucpService.chargeUp(this.SerialNumber, this.Password, pCardNumber, pCardPassword);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="pOldPassword">原密码</param>
        /// <param name="pNewPassword">新密码</param>
        /// <returns>返回值，0表示修改成功</returns>
        public int ChangePassword(string pOldPassword, string pNewPassword)
        {
            int nResult = -1;
            if (pOldPassword == this.Password)
            {
                nResult = objEucpService.serialPwdUpd(this.SerialNumber, pNewPassword, this.Password, pNewPassword);
                if (nResult == 0)
                    this.Password = pNewPassword;
            }
            return nResult;
        }

        /// <summary>
        /// 注册企业信息，只在开通账号时使用一次即可
        /// </summary>
        /// <param name="pCompanyName">公司名称</param>
        /// <param name="pContactPerson">联系人姓名</param>
        /// <param name="pContactPhone">联系电话</param>
        /// <param name="pContactMobile">联系手机</param>
        /// <param name="pContactEmail">联系邮件</param>
        /// <param name="pContactFax">联系传真</param>
        /// <param name="pContactAddress">联系地址</param>
        /// <param name="pContactPostcode">邮政编码</param>
        /// <returns>返回值，0表示成功</returns>
        public int RegistDetailInfo(string pCompanyName,string pContactPerson, string pContactPhone, string pContactMobile,
            string pContactEmail, string pContactFax, string pContactAddress, string pContactPostcode)
        {
            return objEucpService.registDetailInfo(this.SerialNumber, this.Password, pCompanyName, pContactPerson,
                pContactPhone,pContactMobile,pContactEmail,pContactFax,pContactAddress,pContactPostcode);
        }
        
        /// <summary>
        /// 注册序列号，只在开通账号时使用一次即可
        /// </summary>
        /// <returns>返回值，0表示成功</returns>
        public int RegistSerialNumber()
        {
            return objEucpService.registEx(this.SerialNumber, this.Password, this.Password);
        }
    }
}
