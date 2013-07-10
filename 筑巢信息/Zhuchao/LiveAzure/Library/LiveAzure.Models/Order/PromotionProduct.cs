using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Product;
using LiveAzure.Models.General;

namespace LiveAzure.Models.Order
{
    /// <summary>
    /// 促销商品
    /// </summary>
    /// <see cref="PromotionInformation"/>
    /// <see cref="ProductOnItem"/>
    public class PromotionProduct:LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，促销ID
        /// </summary>
        [Required(ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Order.PromotionProduct),
            ErrorMessageResourceName="PromIDRequired")]
        public Guid PromID { get; set; }

        /// <summary>
        /// 主键表ID，SKU
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.PromotionProduct),
            ErrorMessageResourceName = "OnSkuIDRequired")]
        public Guid OnSkuID { get; set; }

        /// <summary>
        /// 数量，-1不限量
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// 主键表ID，物价
        /// </summary>
        [Column("Price")]
        public Guid? aPrice { get; set; }

        /// <summary>
        /// 促销方案主键表
        /// </summary>
        [ForeignKey("PromID")]
        public virtual PromotionInformation Promotion { get; set; }

        /// <summary>
        /// 主键表，SKU
        /// </summary>
        [ForeignKey("OnSkuID")]
        public virtual ProductOnItem OnSkuItem { get; set; }

        /// <summary>
        /// 主键表，特价
        /// </summary>
        [ForeignKey("aPrice")]
        public virtual GeneralResource Price { get; set; }
    }
}
