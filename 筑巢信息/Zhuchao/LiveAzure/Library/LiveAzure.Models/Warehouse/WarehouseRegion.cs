using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;
using LiveAzure.Models.General;

namespace LiveAzure.Models.Warehouse
{
    /// <summary>
    /// 仓库支持的配送区域
    /// </summary>
    /// <see cref="WarehouseInformation"/>
    /// <see cref="GeneralRegion"/>
    public class WarehouseRegion : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，仓库ID
        /// </summary>
        [Required(ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Warehouse.WarehouseRegion),
            ErrorMessageResourceName="WhIDRequired")]
        public Guid WhID { get; set; }

        /// <summary>
        /// 主键表ID，区域ID,可以是任意层级
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseRegion),
            ErrorMessageResourceName = "RegionIDRequired")]
        public Guid RegionID { get; set; }

        /// <summary>
        /// 包含所有末级区域，查询使用递归算法，如果设置了末级，则立即将所有下级记录清除
        /// </summary>
        public bool Terminal { get; set; }

        /// <summary>
        /// 仓库主键表
        /// </summary>
        [ForeignKey("WhID")]
        public virtual WarehouseInformation Warehouse { get; set; }

        /// <summary>
        /// 主键表,区域ID,可以是任意层级
        /// </summary>
        [ForeignKey("RegionID")]
        public virtual GeneralRegion Region { get; set; }
    }
}
