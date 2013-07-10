using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.General;


namespace LiveAzure.Models.Order
{
    /// <summary>
    /// 券，包括抵用券和现金券
    /// </summary>
    /// <see cref="PromotionInformation"/>
    public class PromotionCoupon:LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PromotionCoupon()
        {
            this.StartTime = DateTimeOffset.Now;
            this.EndTime = DateTimeOffset.Now;
        }

        /// <summary>
        /// 主键表ID，促销ID
        /// </summary>
        [Required (ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Order.PromotionCoupon),
            ErrorMessageResourceName="PromIDRequired")]
        public Guid PromID { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
       [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.PromotionCoupon),
            ErrorMessageResourceName = "CodeRequired")]
        [StringLength(64,ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Order.PromotionCoupon),
            ErrorMessageResourceName="CodeLong")]
        public string Code { get; set; }

        /// <summary>
        /// 密码，明文密码
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.PromotionCoupon),
            ErrorMessageResourceName = "PasscodeRequired")]
        [StringLength(64, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.PromotionCoupon),
            ErrorMessageResourceName = "PasscodeLong")]
        public string Passcode { get; set; }

        /// <summary>
        /// 状态 0无效 1有效
        /// </summary>
        /// <see cref="ModelEnum.CouponStatus"/>
        public byte Cstatus { get; set; }

        /// <summary>
        /// 货币，不同货币的积分/现金不能混合使用
        /// </summary>
        [Column("Currency")]
        public Guid? aCurrency { get; set; }

        /// <summary>
        /// 获取的现金(抵用券or现金券)
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

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
        /// 促销方案主键表
        /// </summary>
        [ForeignKey("PromID")]
        public virtual PromotionInformation Promotion { get; set; }

        /// <summary>
        /// 货币主键表
        /// </summary>
        [ForeignKey("aCurrency")]
        public virtual GeneralMeasureUnit Currency { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> CouponStatusList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.CouponStatus), this.Cstatus); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string CouponStatusName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.CouponStatus), this.Cstatus); }
        }
    }
}
