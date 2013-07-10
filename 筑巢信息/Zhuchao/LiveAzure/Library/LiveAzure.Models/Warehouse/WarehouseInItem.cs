using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Product;

namespace LiveAzure.Models.Warehouse
{
    /// <summary>
    /// 入库单明细，货架为实际货架
    /// 准备入库单时，可设置货架为缓存区，确认入库单时，必须指定准确的货架
    /// </summary>
    /// <see cref="WarehouseStockIn"/>
    /// <see cref="ProductInfoItem"/>
    /// <see cref="WarehouseShelf"/>
    public class WarehouseInItem : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，入库单号
        /// </summary>
        [Required(ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Warehouse.WarehouseInItem),
            ErrorMessageResourceName="InIDRequired")]
        public Guid InID { get; set; }

        /// <summary>
        /// 主键表ID，SKU ID
        /// </summary>
        [Required(ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Warehouse.WarehouseInItem),
            ErrorMessageResourceName="SkuIDRequired")]
        public Guid SkuID { get; set; }

        /// <summary>
        /// 主键表ID，货位ID，一货多位时，必须录入多条记录
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseInItem),
            ErrorMessageResourceName = "ShelfID")]
        public Guid ShelfID { get; set; }

        /// <summary>
        /// 批次号，复制采购单
        /// </summary>
        public int TrackLot { get; set; }

        /// <summary>
        /// 数量，需更新Ledger
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// 保值期到，复制自采购单，可修改
        /// </summary>
        public DateTimeOffset? Guarantee { get; set; }

        /// <summary>
        /// 已生成条码数
        /// </summary>
        public int GenBarcode { get; set; }

        /// <summary>
        /// 主键表,入库单
        /// </summary>
        [ForeignKey("InID")]
        public virtual WarehouseStockIn StockIn { get; set; }

        /// <summary>
        /// 主键表,Sku
        /// </summary>
        [ForeignKey("SkuID")]
        public virtual ProductInfoItem SkuItem { get; set; }

        /// <summary>
        /// 主键表,货位
        /// </summary>
        [ForeignKey("ShelfID")]
        public virtual WarehouseShelf Shelf { get; set; }
    }
}
