using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;


namespace LiveAzure.Models.General
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-07         -->
    /// <summary>
    /// 消息队列，待发送消息
    /// </summary>
    /// <see cref="GeneralMessageTemplate"/>
    /// <see cref="GeneralMessageReceive"/>
    public class GeneralMessagePending : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public GeneralMessagePending()
        {
            this.Schedule = DateTimeOffset.Now;
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserID { get; set; }

        /// <summary>
        /// 消息类型 0系统短信 1手机短信 2邮件
        /// </summary>
        /// <see cref="ModelEnum.MessageType"/>
        public byte Mtype { get; set; }

        /// <summary>
        /// 消息状态
        /// </summary>
        /// <see cref="ModelEnum.MessageStatus"/>
        public byte Mstatus { get; set; }

        /// <summary>
        /// 收件人姓名
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralMessagePending),
            ErrorMessageResourceName = "NameLong")]
        public string Name { get; set; }

        /// <summary>
        /// 收件人地址，手机号或邮件地址
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralMessagePending),
            ErrorMessageResourceName = "RecipientLong")]
        public string Recipient { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralMessagePending),
            ErrorMessageResourceName = "TitleLong")]
        public string Title { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Matter { get; set; }

        /// <summary>
        /// 管理单据类型 0订单号
        /// </summary>
        /// <see cref="ModelEnum.NoteType"/>
        public byte RefType { get; set; }

        /// <summary>
        /// 关联单据号，可空
        /// </summary>
        public Guid? RefID { get; set; }

        /// <summary>
        /// 定时发送，精确到半小时，考虑使用GMT时间
        /// </summary>
        public DateTimeOffset? Schedule { get; set; }

        /// <summary>
        /// 实际发送时间
        /// </summary>
        public DateTimeOffset? SentTime { get; set; }

        /// <summary>
        /// 用户主键表
        /// </summary>
        [ForeignKey("UserID")]
        public virtual MemberUser User { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> MessageStatusList
        {
            get
            {
                return base.SelectEnumList(typeof(ModelEnum.MessageStatus), this.Mstatus);
            }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string MessageStatusName
        {
            get
            {
                return base.SelectEnumName(typeof(ModelEnum.MessageStatus), this.Mstatus);
            }
        }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> MessageTypeList
        {
            get
            {
                return base.SelectEnumList(typeof(ModelEnum.MessageType), this.Mtype);
            }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string MessageTypeName
        {
            get
            {
                return base.SelectEnumName(typeof(ModelEnum.MessageType), this.Mtype);
            }
        }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> RefTypeList
        {
            get
            {
                return base.SelectEnumList(typeof(ModelEnum.NoteType), this.RefType);
            }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string RefTypeName
        {
            get
            {
                return base.SelectEnumName(typeof(ModelEnum.NoteType), this.RefType);
            }
        }
    }
}
