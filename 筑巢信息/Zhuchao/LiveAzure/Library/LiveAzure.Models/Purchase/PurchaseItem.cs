using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Product;

namespace LiveAzure.Models.Purchase
{
    /// <summary>
    /// 采购单明细，程序中允许输入金额和数量，反算单价
    /// </summary>
    /// <see cref="PurchaseInformation"/>
    /// <see cref="ProductInfoItem"/>
    public class PurchaseItem : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，采购单ID
        /// </summary>
        [Required(ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Purchase.PurchaseItem),
            ErrorMessageResourceName="PurIDRequired")]
        public Guid PurID { get; set; }

        /// <summary>
        /// 主键表ID，SKU ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Purchase.PurchaseItem),
            ErrorMessageResourceName = "SkuIDRequired")]
        public Guid SkuID { get; set; }

        /// <summary>
        /// 采购数量
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// 已入库数量
        /// </summary>
        [Column(TypeName = "money")]
        public decimal InQty { get; set; }

        /// <summary>
        /// 采购价格，货币在主键表中定义
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        /// <summary>
        /// 分摊单件税额
        /// </summary>
        [Column(TypeName = "money")]
        public decimal TaxFee { get; set; }
        
        /// <summary>
        /// 分摊单件运费
        /// </summary>
        [Column(TypeName = "money")]
        public decimal ShipFee { get; set; }
        
        /// <summary>
        /// 单件成本，平均到WarehouseLedger中
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(TypeName = "money")]
        public decimal AvgCost { get; set; }

        /// <summary>
        /// 小计金额
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 保质期
        /// </summary>
        public DateTimeOffset? Guarantee { get; set; }

        /// <summary>
        /// 采购单主键表
        /// </summary>
        [ForeignKey("PurID")]
        public virtual PurchaseInformation Purchase { get; set; }

        /// <summary>
        /// SKU主键表
        /// </summary>
        [ForeignKey("SkuID")]
        public virtual ProductInfoItem SkuItem { get; set; }
    }
}
