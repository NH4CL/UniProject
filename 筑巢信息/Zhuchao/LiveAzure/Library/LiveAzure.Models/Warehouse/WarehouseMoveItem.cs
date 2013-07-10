using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Product;

namespace LiveAzure.Models.Warehouse
{
    /// <summary>
    /// 移库单/移货位单明细表
    /// </summary>
    /// <see cref="WarehouseMoving"/>
    /// <see cref="WarehouseShelf"/>
    /// <see cref="ProductInfoItem"/>
    public class WarehouseMoveItem : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，移库单号
        /// </summary>
        [Required(ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Warehouse.WarehouseMoveItem),
            ErrorMessageResourceName="MoveIDRequired")]
        public Guid MoveID { get; set; }

        /// <summary>
        /// 主键表ID，SKU ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseMoveItem),
            ErrorMessageResourceName = "SkuIDRequired")]
        public Guid SkuID { get; set; }

        /// <summary>
        /// 原货架
        /// </summary>
        public Guid? OldShelf { get; set; }

        /// <summary>
        /// 新货架，计划新货架，或缓存区
        /// </summary>
        public Guid? NewShelf { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public int TrackLot { get; set; }

        /// <summary>
        /// 主键表，移库单
        /// </summary>
        [ForeignKey("MoveID")]
        public virtual WarehouseMoving Moving { get; set; }

        /// <summary>
        /// 主键表，Sku
        /// </summary>
        [ForeignKey("SkuID")]
        public virtual ProductInfoItem SkuItem { get; set; }

        /// <summary>
        /// 主键表，原货架
        /// </summary>
        [ForeignKey("OldShelf")]
        public virtual WarehouseShelf ShelfOld { get; set; }

        /// <summary>
        /// 主键表，新货架
        /// </summary>
        [ForeignKey("OldShelf")]
        public virtual WarehouseShelf ShelfNew { get; set; }
    }
}
