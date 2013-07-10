using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LiveAzure.Models.Purchase
{
    /// <summary>
    /// 采购单变更记录明细
    /// </summary>
    /// <see cref="PurchaseHistory"/>
    public class PurchaseHisItem : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，变更单ID
        /// </summary>
        public Guid? PurHisID { get; set; }

        /// <summary>
        /// SKU ID
        /// </summary>
        public Guid? SkuID { get; set; }

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
        /// 采购价格
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
        [Column(TypeName = "money")]
        public decimal AvgCost { get; set; }

        /// <summary>
        /// 小计金额
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 变更单主键表
        /// </summary>
        [ForeignKey("PurHisID")]
        public virtual PurchaseHistory History { get; set; }
    }
}
