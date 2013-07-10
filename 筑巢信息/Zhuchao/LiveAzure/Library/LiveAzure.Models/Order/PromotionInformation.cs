using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;
using LiveAzure.Models.General;


namespace LiveAzure.Models.Order
{
    /// <summary>
    /// 促销方案
    /// </summary>
    /// <see cref="MemberOrganization"/>
    /// <see cref="MemberChannel"/>
    /// <see cref="PromotionMutex"/>
    public class PromotionInformation : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PromotionInformation()
        {
            this.IssueStart = DateTimeOffset.Now;
            this.IssueEnd = DateTimeOffset.Now;
            this.StartTime = DateTimeOffset.Now;
            this.EndTime = DateTimeOffset.Now;
        }
        /// <summary>
        /// 主键表ID，组织ID
        /// </summary>
        [Required (ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Order.PromotionInformation),
            ErrorMessageResourceName="OrgIDRequired")]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 主键表ID，渠道ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.PromotionInformation),
            ErrorMessageResourceName = "ChlIDRequired")]
        public Guid ChlID { get; set; }

        /// <summary>
        /// 代码，组织内唯一
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.PromotionInformation),
            ErrorMessageResourceName = "CodeRequired")]
        [StringLength(50,ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Order.PromotionInformation),
            ErrorMessageResourceName="CodeLong")]
        public string Code { get; set; }

        /// <summary>
        /// 主键表ID，名称
        /// </summary>
        [Column("Name")]
        public Guid? aName { get; set; }

        /// <summary>
        /// 简单描述，默认语言
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.PromotionInformation),
            ErrorMessageResourceName = "MatterLong")]
        public string Matter { get; set; }

        /// <summary>
        /// 状态 0无效 1有效
        /// </summary>
        /// <see cref="ModelEnum.PromotionStatus"/>
        public byte Pstatus { get; set; }

        /// <summary>
        /// 发放形式
        /// Enum 待定
        /// </summary>
        public byte IssueType { get; set; }

        /// <summary>
        /// 排序，优先级
        /// </summary>
        public int Sorting { get; set; }

        /// <summary>
        /// 促销类型，对应程序算法
        /// Enum 待定
        /// </summary>
        public byte Ptype { get; set; }

        /// <summary>
        /// 条件A，可能是日期、数字或积分值等，可转换
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.PromotionInformation),
            ErrorMessageResourceName = "ConditionALong")]
        public string ConditionA { get; set; }

        /// <summary>
        /// 条件B
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.PromotionInformation),
            ErrorMessageResourceName = "ConditionBLong")]
        public string ConditionB { get; set; }

        /// <summary>
        /// 条件C
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.PromotionInformation),
            ErrorMessageResourceName = "ConditionCLong")]
        public string ConditionC { get; set; }

        /// <summary>
        /// 条件D
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.PromotionInformation),
            ErrorMessageResourceName = "ConditionDLong")]
        public string ConditionD { get; set; }

        /// <summary>
        /// 发放开始时间
        /// </summary>
        public DateTimeOffset IssueStart { get; set; }

        /// <summary>
        /// 发放结束时间
        /// </summary>
        public DateTimeOffset IssueEnd { get; set; }

        /// <summary>
        /// 有效起始时间，起始时需要刷新金额
        /// </summary>
        public DateTimeOffset StartTime { get; set; }

        /// <summary>
        /// 有效结束时间
        /// </summary>
        public DateTimeOffset EndTime { get; set; }

        /// <summary>
        /// 有效天数，用于计算结束时间
        /// </summary>
        public int EffectDays { get; set; }

        /// <summary>
        /// 组织主键表
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }

        /// <summary>
        /// 渠道主键表
        /// </summary>
        [ForeignKey("ChlID")]
        public virtual MemberChannel Channel { get; set; }

        /// <summary>
        /// 名称主键表
        /// </summary>
        [ForeignKey("aName")]
        public virtual GeneralResource Name { get; set; }

        /// <summary>
        /// 从表内容，促销方案
        /// </summary>
        [InverseProperty("Promotion")]
        public virtual ICollection<PromotionMutex> Mutexes { get; set; }

        /// <summary>
        /// 从表内容，促销商品
        /// </summary>
        [InverseProperty("Promotion")]
        public virtual ICollection<PromotionProduct> Products { get; set; }

        /// <summary>
        /// 从表内容，券
        /// </summary>
        [InverseProperty("Promotion")]
        public virtual ICollection<PromotionCoupon> Coupons { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> PromotionStatusList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.PromotionStatus), this.Pstatus); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string PromotionStatusName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.PromotionStatus), this.Pstatus); }
        }
    }
}
