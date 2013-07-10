using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Product;

namespace LiveAzure.Models.Warehouse
{
    /// <summary>
    /// 盘点单明细
    /// </summary>
    /// <see cref="WarehouseInventory"/>
    /// <see cref="WarehouseShelf"/>
    /// <see cref="ProductInfoItem"/>
    public class WarehouseInvItem : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，盘点单号
        /// </summary>
        [Required(ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Warehouse.WarehouseInvItem),
            ErrorMessageResourceName="InvIDRequired")]
        public Guid InvID { get; set; }

        /// <summary>
        /// 主键表ID，SKU ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseInvItem),
            ErrorMessageResourceName = "SkuIDRequired")]
        public Guid SkuID { get; set; }

        /// <summary>
        /// 主键表ID，实际货位
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseInvItem),
            ErrorMessageResourceName = "ShelfIDRequired")]
        public Guid ShelfID { get; set; }

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
        /// 盘点单主键表
        /// </summary>
        [ForeignKey("InvID")]
        public virtual WarehouseInventory Inventory { get; set; }

        /// <summary>
        /// SKU主键表
        /// </summary>
        [ForeignKey("SkuID")]
        public virtual ProductInfoItem SkuItem { get; set; }

        /// <summary>
        /// 主键表，实际盘点货位
        /// </summary>
        [ForeignKey("ShelfID")]
        public virtual WarehouseShelf Shelf { get; set; }
    }
}
