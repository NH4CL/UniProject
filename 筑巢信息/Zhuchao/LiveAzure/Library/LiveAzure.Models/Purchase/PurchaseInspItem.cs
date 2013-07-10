using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Product;

namespace LiveAzure.Models.Purchase
{
    /// <summary>
    /// 质检明细表
    /// </summary>
    /// <see cref="PurchaseInspection"/>
    /// <see cref="ProductInfoItem"/>
    public class PurchaseInspItem : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，质检单ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Purchase.PurchaseInspItem),
            ErrorMessageResourceName = "InspID")]
        public Guid InspID { get; set; }

        /// <summary>
        /// 主键表ID，
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Purchase.PurchaseInspItem),
            ErrorMessageResourceName = "SkuID")]
        public Guid SkuID { get; set; }

        /// <summary>
        /// 采购数量
        /// </summary>
        [Column(TypeName = "money")]
        public decimal TotalQty { get; set; }

        /// <summary>
        /// 检验数量
        /// </summary>
        [Column(TypeName = "money")]
        public decimal InspQty { get; set; }

        /// <summary>
        /// 合格数量
        /// </summary>
        [Column(TypeName = "money")]
        public decimal PassQty { get; set; }

        /// <summary>
        /// 检验比例
        /// </summary>
        [Column(TypeName = "money")]
        public decimal InspRate { get; set; }

        /// <summary>
        /// 合格比例
        /// </summary>
        [Column(TypeName = "money")]
        public decimal PassRate { get; set; }

        /// <summary>
        /// 质检单主键表
        /// </summary>
        [ForeignKey("InspID")]
        public virtual PurchaseInspection Inspection { get; set; }

        /// <summary>
        /// SKU主键表
        /// </summary>
        [ForeignKey("SkuID")]
        public virtual ProductInfoItem SkuItem { get; set; }
    }
}
