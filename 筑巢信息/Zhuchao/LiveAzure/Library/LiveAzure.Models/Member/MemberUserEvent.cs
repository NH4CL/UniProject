using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.General;


namespace LiveAzure.Models.Member
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-08         -->
    /// <summary>
    /// 用户事件，例如打电话联系，发DM时间等信息
    /// </summary>
    /// <see cref="MemberUser"/>
    public class MemberUserEvent : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，积分ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUserEvent),
            ErrorMessageResourceName = "UserIDRequired")]
        public Guid UserID { get; set; }

        /// <summary>
        /// 事件类型 0电话呼入 1电话呼出 2发DM 3在线沟通
        /// </summary>
        /// <see cref="ModelEnum.UserEventType"/>
        public byte Etype { get; set; }

        /// <summary>
        /// 再次提醒日期，到期后无限提醒，直到取消为止
        /// </summary>
        public DateTimeOffset? Reminder { get; set; }

        /// <summary>
        /// 呼入电话号，在线沟通ID等
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUserEvent),
            ErrorMessageResourceName = "SourceLong")]
        public string Source { get; set; }

        /// <summary>
        /// 接待人，接待人ID号等
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUserEvent),
            ErrorMessageResourceName = "DestinationLong")]
        public string Destination { get; set; }

        /// <summary>
        /// 内容描述，例如电话主要内容，录用文件位置，DM版本，在线沟通内容等
        /// </summary>
        [StringLength(512, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUserEvent),
            ErrorMessageResourceName = "MatterLong")]
        public string Matter { get; set; }

        /// <summary>
        /// 单据类型
        /// </summary>
        /// <see cref="ModelEnum.NoteType"/>
        public byte RefType { get; set; }

        /// <summary>
        /// 参考订单号
        /// </summary>
        public Guid? RefID { get; set; }
        
        /// <summary>
        /// 用户主键表
        /// </summary>
        [ForeignKey("UserID")]
        public virtual MemberUser User { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> RefTypeList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.NoteType), this.RefType); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string RefTypeName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.NoteType), this.RefType); }
        }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> EventTypeList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.UserEventType), this.Etype); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string EventTypeName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.UserEventType), this.Etype); }
        }
    }
}
