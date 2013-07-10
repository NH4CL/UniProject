using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Product;

namespace LiveAzure.Models.Warehouse
{
    /// <summary>
    /// 出库记录明细
    /// </summary>
    /// <see cref="WarehouseStockOut"/>
    /// <see cref="WarehouseShelf"/>
    /// <see cref="ProductInfoItem"/>
    public class WarehouseOutItem : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，出库单号
        /// </summary>
        [Required(ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Warehouse.WarehouseOutItem),
            ErrorMessageResourceName="OutIDRequired")]
        public Guid OutID { get; set; }

        /// <summary>
        /// 主键表ID，SKU ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseOutItem),
            ErrorMessageResourceName = "SkuIDRequired")]
        public Guid SkuID { get; set; }

        /// <summary>
        /// 主键表ID，实际货位，一货多位时，必须录入多条记录
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseOutItem),
            ErrorMessageResourceName = "ShelfIDRequired")]
        public Guid ShelfID { get; set; }

        /// <summary>
        /// 批次号，考虑FIFO
        /// </summary>
        public int TrackLot { get; set; }

        /// <summary>
        /// 数量，需更新Ledger
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// 主键表，出库单
        /// </summary>
        [ForeignKey("OutID")]
        public virtual WarehouseStockOut StockOut { get; set; }

        /// <summary>
        /// SKU主键表
        /// </summary>
        [ForeignKey("SkuID")]
        public virtual ProductInfoItem SkuItem { get; set; }

        /// <summary>
        /// 主键表，实际货位，一货多位时，必须录入多条记录
        /// </summary>
        [ForeignKey("ShelfID")]
        public virtual WarehouseShelf Shelf { get; set; }
    }
}
