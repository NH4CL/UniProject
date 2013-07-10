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
    /// 用户积分使用情况
    /// </summary>
    /// <see cref="MemberPoint"/>
    public class MemberUsePoint : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，积分ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUsePoint),
            ErrorMessageResourceName = "PointIDRequired")]
        public Guid PointID { get; set; }

        /// <summary>
        /// 是否有效 0无效 1有效 2已使用
        /// </summary>
        /// <see cref="ModelEnum.PointUsed"/>
        public byte Pstatus { get; set; }

        /// <summary>
        /// 单据类型
        /// </summary>
        /// <see cref="ModelEnum.NoteType"/>
        public byte RefType { get; set; }

        /// <summary>
        /// 消费积分的关联单据号，主要是订单号，提现是pay.gid
        /// </summary>
        public Guid? RefID { get; set; }

        /// <summary>
        /// 消费/提现积分的参考号码，例如银行单据号
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberPoint),
            ErrorMessageResourceName = "CommentLong")]
        public string Comment { get; set; }

        /// <summary>
        /// 消费积分值
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// 消费金额
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 积分主键表
        /// </summary>
        [ForeignKey("PointID")]
        public virtual MemberPoint Point { get; set; }

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
        public List<ListItem> PointUsedList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.PointUsed), this.Pstatus); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string PointUsedName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.PointUsed), this.Pstatus); }
        }
    }
}
