using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LiveAzure.Models.General
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-07         -->
    /// <summary>
    /// 接收到的手机短信
    /// </summary>
    /// <see cref="GeneralMessageTemplate"/>
    /// <see cref="GeneralMessagePending"/>
    public class GeneralMessageReceive : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public GeneralMessageReceive()
        {
            this.SentTime = DateTimeOffset.Now;
        }

        /// <summary>
        /// 发件人手机号
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralMessageReceive),
            ErrorMessageResourceName = "SendFromLong")]
        public string SendFrom { get; set; }
        
        /// <summary>
        /// 内容
        /// </summary>
        public string Matter { get; set; }
        
        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTimeOffset? SentTime { get; set; }
        
        /// <summary>
        /// 接收通道信息，可空
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralMessageReceive),
            ErrorMessageResourceName = "GetFromLong")]
        public string GetFrom { get; set; }
    }
}
