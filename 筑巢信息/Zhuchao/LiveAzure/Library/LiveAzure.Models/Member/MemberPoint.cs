using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.General;
using LiveAzure.Models.Order;

namespace LiveAzure.Models.Member
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-08         -->
    /// <summary>
    /// 用户积分
    /// </summary>
    /// <see cref="MemberUser"/>
    /// <see cref="MemberUsePoint"/>
    public class MemberPoint : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MemberPoint()
        {
            this.StartTime = DateTimeOffset.Now;
            this.EndTime = DateTimeOffset.Now;
        }

        /// <summary>
        /// 主键表ID，组织ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberPoint),
            ErrorMessageResourceName = "OrgIDRequired")]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 主键表ID，用户ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberPoint),
            ErrorMessageResourceName = "UserIDRequired")]
        public Guid UserID { get; set; }

        /// <summary>
        /// 类别xml定义 0积分 1现金余额 2抵用券 3联盟返点 4CPS返点 5代销返点
        /// </summary>
        /// <see cref="ModelEnum.PointType"/>
        public byte Ptype { get; set; }

        /// <summary>
        /// 是否有效 0无效 1有效 2已使用
        /// </summary>
        /// <see cref="ModelEnum.PointStatus"/>
        public byte Pstatus { get; set; }

        /// <summary>
        /// 原因说明，可空
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberPoint),
            ErrorMessageResourceName = "ReasonLong")]
        public string Reason { get; set; }

        /// <summary>
        /// 促销方案，可空
        /// </summary>
        public Guid? PromID { get; set; }

        /// <summary>
        /// 单据类型
        /// </summary>
        /// <see cref="ModelEnum.NoteType"/>
        public byte RefType { get; set; }

        /// <summary>
        /// 获取积分的关联单据号，主要是订单号
        /// </summary>
        public Guid? RefID { get; set; }

        /// <summary>
        /// 获取的积分值
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// 积分余额
        /// </summary>
        public int Remain { get; set; }

        /// <summary>
        /// 券ID，可空
        /// </summary>
        public Guid? CouponID { get; set; }

        /// <summary>
        /// 货币，不同货币的积分/现金不能混合使用
        /// </summary>
        [Column("Currency")]
        public Guid? aCurrency { get; set; }

        /// <summary>
        /// 获取的现金(抵用券or现金券)
        /// </summary>
        [Column(TypeName="money")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 现金余额
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Balance { get; set; }

        /// <summary>
        /// 最低消费抵用
        /// </summary>
        [Column(TypeName = "money")]
        public decimal MinCharge { get; set; }
        
        /// <summary>
        /// 是否允许提现，提现扣税线下实现
        /// </summary>
        public bool Cashier { get; set; }

        /// <summary>
        /// 是否一次性使用，使用完后，无论是否有余额，立即失效
        /// </summary>
        public bool OnceUse { get; set; }

        /// <summary>
        /// 是否可以多券同时使用
        /// </summary>
        public bool Together { get; set; }

        /// <summary>
        /// 有效起始时间，起始时需要刷新金额
        /// </summary>
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset? StartTime { get; set; }

        /// <summary>
        /// 有效结束时间
        /// </summary>
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset? EndTime { get; set; }

        /// <summary>
        /// 组织主键表
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }

        /// <summary>
        /// 用户主键表
        /// </summary>
        [ForeignKey("UserID")]
        public virtual MemberUser User { get; set; }

        /// <summary>
        /// 促销主键表
        /// </summary>
        [ForeignKey("PromID")]
        public virtual PromotionInformation Promotion { get; set; }

        /// <summary>
        /// 促销主键表
        /// </summary>
        [ForeignKey("CouponID")]
        public virtual PromotionCoupon Coupon { get; set; }

        /// <summary>
        /// 货币单位
        /// </summary>
        [ForeignKey("aCurrency")]
        public virtual GeneralMeasureUnit Currency { get; set; }

        /// <summary>
        /// 从表内容，积分使用情况
        /// </summary>
        [InverseProperty("Point")]
        public virtual ICollection<MemberUsePoint> PointUses { get; set; }

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
        public List<ListItem> PointTypeList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.PointType), this.Ptype); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string PointTypeName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.PointType), this.Ptype); }
        }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> PointStatusList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.PointStatus), this.Pstatus); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string PointStatusName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.PointStatus), this.Pstatus); }
        }
    }
}
