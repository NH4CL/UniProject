using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;

namespace LiveAzure.Models.Warehouse
{
    /// <summary>
    /// 库位(货架)定义表
    /// 第一个货架应为缓存区
    /// </summary>
    /// <see cref="WarehouseInformation"/>
    /// <see cref="WarehouseSkuShelf"/>
    public class WarehouseShelf : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，仓库ID
        /// </summary>
        [Required(ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Warehouse.WarehouseShelf),
            ErrorMessageResourceName="WhIDRequired")]
        public Guid WhID { get; set; }

        /// <summary>
        /// 货位代码，仓库内唯一
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseShelf),
            ErrorMessageResourceName = "CodeRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseShelf),
            ErrorMessageResourceName = "CodeLong")]
        public string Code { get; set; }

        /// <summary>
        /// 库位条码，仓库内唯一
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseShelf),
            ErrorMessageResourceName = "BarcodeRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseShelf),
            ErrorMessageResourceName = "BarcodeLong")]
        public string Barcode { get; set; }

        /// <summary>
        /// 保留货位，不参与自动发货
        /// </summary>
        public bool Reserved { get; set; }

        /// <summary>
        /// 名称，默认语言，可空
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseShelf),
            ErrorMessageResourceName = "NameLong")]
        public string Name { get; set; }

        /// <summary>
        /// 简单描述，存放商品类目等，默认语言
        /// </summary>
        [StringLength(512, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseShelf),
            ErrorMessageResourceName = "BriefLong")]
        public string Brief { get; set; }

        /// <summary>
        /// 仓库主键表
        /// </summary>
        [ForeignKey("WhID")]
        public virtual WarehouseInformation Warehouse { get; set; }

        /// <summary>
        /// 从表内容,库位/货位对照表
        /// </summary>
        [InverseProperty("Shelf")]
        public virtual ICollection<WarehouseSkuShelf> ShelfItems { get; set; }

        [NotMapped]
        public decimal ShelfQuantity
        {
            get
            {
                decimal mQuantity = 0m;
                if (this.ShelfItems != null)
                    foreach (var item in this.ShelfItems)
                    {
                        mQuantity += item.Quantity;
                    }
                return mQuantity;
            }
        }
    }
}
