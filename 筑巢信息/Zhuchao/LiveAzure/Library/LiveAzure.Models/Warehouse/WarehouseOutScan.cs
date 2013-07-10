using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Product;

namespace LiveAzure.Models.Warehouse
{
    /// <summary>
    /// 出库扫描记录
    /// 出库拣货单上打印预计的库位条码，扫描出库时必须输入库位号
    /// </summary>
    /// <see cref="WarehouseStockOut"/>
    /// <see cref="ProductInfoItem"/>
    public class WarehouseOutScan : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，出库单号
        /// </summary>
        [Required(ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Warehouse.WarehouseOutSacn),
            ErrorMessageResourceName="OutIDRequired")]
        public Guid OutID { get; set; }

        /// <summary>
        /// 实际拣货/扫描人 (冗余字段)
        /// </summary>
        public Guid? TakeMan { get; set; }

        /// <summary>
        /// 条码号
        /// </summary>
        [StringLength(50,ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Warehouse.WarehouseOutSacn),
            ErrorMessageResourceName="BarcodeLong")]
        public string Barcode { get; set; }

        /// <summary>
        /// 主键表ID，SKU
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseOutSacn),
            ErrorMessageResourceName = "SkuIDRequired")]
        public Guid SkuID { get; set; }

        /// <summary>
        /// 数量，需要更新Ledger
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// 主键表，出库单号
        /// </summary>
        [ForeignKey("OutID")]
        public virtual WarehouseStockOut StockOut { get; set; }

        /// <summary>
        /// 主键表，SKU
        /// </summary>
        [ForeignKey("SkuID")]
        public virtual ProductInfoItem SkuItem { get; set; }
    }
}
