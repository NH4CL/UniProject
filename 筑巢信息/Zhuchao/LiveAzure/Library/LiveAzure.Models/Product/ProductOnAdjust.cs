using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.General;

namespace LiveAzure.Models.Product
{
    /// <summary>
    /// 商品调价定时器，按SKU调整
    /// </summary>
    /// <see cref="ProductOnSale"/>
    /// <see cref="ProductOnItem"/>
    public class ProductOnAdjust : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ProductOnAdjust()
        {
            this.StartTime = DateTimeOffset.Now;
        }

        /// <summary>
        /// 主键表ID，上架商品 PU ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductOnAdjust),
            ErrorMessageResourceName = "OnSaleIDRequired")]
        public Guid OnSaleID { get; set; }

        /// <summary>
        /// 主键表ID，上架商品SKU ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductOnAdjust),
            ErrorMessageResourceName = "OnSkuIDRequired")]
        public Guid OnSkuID { get; set; }

        /// <summary>
        /// 状态 0待审批 1审批通过 2驳回/废弃 3已发布
        /// </summary>
        /// <see cref="ModelEnum.AdjustStatus"/>
        public byte Astatus { get; set; }

        /// <summary>
        /// 制表人
        /// </summary>
        public Guid? PreparedBy { get; set; }

        /// <summary>
        /// 审批人
        /// </summary>
        public Guid? ApprovedBy { get; set; }

        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTimeOffset? ApprovedTime { get; set; }

        /// <summary>
        /// 计划生效时间，精确到10分钟
        /// </summary>
        public DateTimeOffset? StartTime { get; set; }

        /// <summary>
        /// 失效复原时间
        /// </summary>
        public DateTimeOffset? RestoreTime { get; set; }

        /// <summary>
        /// 失效是否需要复原
        /// </summary>
        public bool BeRestore { get; set; }

        /// <summary>
        /// 活动数量
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// 原实际销售价
        /// </summary>
        [Column("SalePriceOld")]
        public Guid? aSalePriceOld { get; set; }

        /// <summary>
        /// 原使用积分购买的积分值
        /// </summary>
        public int UseScoreOld { get; set; }

        /// <summary>
        /// 原使用积分抵扣的金额
        /// </summary>
        [Column("ScoreDeductOld")]
        public Guid? aScoreDeductOld { get; set; }

        /// <summary>
        /// 原购买成功，获取积分数，xml中配置n天后生效
        /// </summary>
        public int GetScoreOld { get; set; }

        /// <summary>
        /// 新实际销售价
        /// </summary>
        [Column("SalePriceNew")]
        public Guid? aSalePriceNew { get; set; }

        /// <summary>
        /// 新使用积分购买的积分值
        /// </summary>
        public int UseScoreNew { get; set; }

        /// <summary>
        /// 新使用积分抵扣的金额
        /// </summary>
        [Column("ScoreDeductNew")]
        public Guid? aScoreDeductNew { get; set; }

        /// <summary>
        /// 新购买成功，获取积分数，xml中配置n天后生效
        /// </summary>
        public int GetScoreNew { get; set; }

        /// <summary>
        /// 上架商品主键表
        /// </summary>
        [ForeignKey("OnSaleID")]
        public virtual ProductOnSale OnSale { get; set; }

        /// <summary>
        /// 上架商品SKU主键表
        /// </summary>
        [ForeignKey("OnSkuID")]
        public virtual ProductOnItem OnSkuItem { get; set; }

        /// <summary>
        /// 原实际销售价主键表
        /// </summary>
        [ForeignKey("aSalePriceOld")]
        public virtual GeneralResource SalePriceOld { get; set; }

        /// <summary>
        /// 原使用积分抵扣的金额主键表
        /// </summary>
        [ForeignKey("aScoreDeductOld")]
        public virtual GeneralResource ScoreDeductOld { get; set; }

        /// <summary>
        /// 新实际销售价主键表
        /// </summary>
        [ForeignKey("aSalePriceNew")]
        public virtual GeneralResource SalePriceNew { get; set; }

        /// <summary>
        /// 新使用积分抵扣的金额主键表
        /// </summary>
        [ForeignKey("aScoreDeductNew")]
        public virtual GeneralResource ScoreDeductNew { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> AdjustStatusList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.AdjustStatus), this.Astatus); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string AdjustStatusName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.AdjustStatus), this.Astatus); }
        }
    }
}
