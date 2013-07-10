using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Product;

namespace LiveAzure.Models.Warehouse
{
    /// <summary>
    /// 库位/货位对照表
    /// </summary>
    /// <see cref="ProductInfoItem"/>
    /// <see cref="WarehouseShelf"/>
    public class WarehouseSkuShelf : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，仓库ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseSkuShelf),
            ErrorMessageResourceName = "WhIDRequired")]
        public Guid WhID { get; set; }

        /// <summary>
        /// 主键表ID，SKU ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseSkuShelf),
            ErrorMessageResourceName = "SkuIDRequired")]
        public Guid SkuID { get; set; }

        /// <summary>
        /// 主键表ID，库位ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseSkuShelf),
            ErrorMessageResourceName = "ShelfIDRequired")]
        public Guid ShelfID { get; set; }

        /// <summary>
        /// 批次号，从采购单中复制，用于FIFO
        /// </summary>
        public int TrackLot { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// 锁定数量
        /// </summary>
        [Column(TypeName = "money")]
        public decimal LockQty { get; set; }

        /// <summary>
        /// 保值期到，复制自采购单，可修改
        /// </summary>
        public DateTimeOffset? Guarantee { get; set; }

        /// <summary>
        /// 仓库主键表
        /// </summary>
        [ForeignKey("WhID")]
        public virtual WarehouseInformation Warehouse { get; set; }

        /// <summary>
        /// 主键表,Sku
        /// </summary>
        [ForeignKey("SkuID")]
        public virtual ProductInfoItem SkuItem { get; set; }

        /// <summary>
        /// 主键表,库位
        /// </summary>
        [ForeignKey("ShelfID")]
        public virtual WarehouseShelf Shelf { get; set; }
    }
}
