using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;
using LiveAzure.Models.Shipping;

namespace LiveAzure.Models.Warehouse
{
    /// <summary>
    /// 仓库支持的运输公司
    /// </summary>
    /// <see cref="WarehouseInformation"/>
    public class WarehouseShipping : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，仓库ID
        /// </summary>
        [Required(ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Warehouse.WarehouseShipping),
            ErrorMessageResourceName="WhIDRequired")]
        public Guid WhID { get; set; }

        /// <summary>
        /// 主键表ID，运输公司ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseShipping),
            ErrorMessageResourceName = "ShipIDRequired")]
        public Guid ShipID { get; set; }

        /// <summary>
        /// 仓库主键表
        /// </summary>
        [ForeignKey("WhID")]
        public virtual WarehouseInformation Warehouse { get; set; }

        /// <summary>
        /// 主键表,运输公司
        /// </summary>
        [ForeignKey("ShipID")]
        public virtual ShippingInformation Shipper { get; set; }
    }
}
