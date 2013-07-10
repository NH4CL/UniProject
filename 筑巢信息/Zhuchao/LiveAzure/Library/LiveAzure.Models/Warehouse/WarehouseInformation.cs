using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;

namespace LiveAzure.Models.Warehouse
{
    /// <summary>
    /// 仓库定义
    /// </summary>
    /// <see cref="WarehouseChannel"/>
    /// <see cref="WarehouseShelf"/>
    public class WarehouseInformation : LiveAzure.Models.Member.OrganizationBase
    {
        /// <summary>
        /// 构造函数，设置类型
        /// </summary>
        public WarehouseInformation()
        {
            this.Otype = (byte)ModelEnum.OrganizationType.WAREHOUSE;
        }

        /// <summary>
        /// 从表内容,货架
        /// </summary>
        [InverseProperty("Warehouse")]
        public virtual ICollection<WarehouseShelf> Shelves { get; set; }

        /// <summary>
        /// 从表内容,货架和SKU
        /// </summary>
        [InverseProperty("Warehouse")]
        public virtual ICollection<WarehouseSkuShelf> SkuShelves { get; set; }

        /// <summary>
        /// 从表内容,仓库支持的配送区域
        /// </summary>
        [InverseProperty("Warehouse")]
        public virtual ICollection<WarehouseRegion> Regions { get; set; }

        /// <summary>
        /// 从表内容,仓库支持的运输公司
        /// </summary>
        [InverseProperty("Warehouse")]
        public virtual ICollection<WarehouseShipping> Shippings { get; set; }

        /// <summary>
        /// 从表内容,库存总帐
        /// </summary>
        [InverseProperty("Warehouse")]
        public virtual ICollection<WarehouseLedger> Ledgers { get; set; }

        /// <summary>
        /// 从表内容,入库单
        /// </summary>
        [InverseProperty("Warehouse")]
        public virtual ICollection<WarehouseStockIn> StockIns { get; set; }

        /// <summary>
        /// 从表内容,出库单
        /// </summary>
        [InverseProperty("Warehouse")]
        public virtual ICollection<WarehouseStockOut> StockOuts { get; set; }

        /// <summary>
        /// 从表内容，盘点记录
        /// </summary>
        [InverseProperty("Warehouse")]
        public virtual ICollection<WarehouseInventory> Inventories { get; set; }
    }
}
