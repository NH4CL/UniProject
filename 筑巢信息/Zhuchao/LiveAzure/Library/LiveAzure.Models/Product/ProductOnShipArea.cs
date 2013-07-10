using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Finance;
using LiveAzure.Models.Shipping;
using LiveAzure.Models.General;

namespace LiveAzure.Models.Product
{
    /// <summary>
    /// 上架商品承运商支持的地区
    /// </summary>
    /// <see cref="ProductOnShipping"/>
    /// <see cref="ShippingArea"/>
    public class ProductOnShipArea : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，上架商品承运商
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductOnShipArea),
            ErrorMessageResourceName = "OnShipRequired")]
        public Guid OnShip { get; set; }

        ///// <summary>
        ///// 主键表ID，上架商品承运地区
        ///// </summary>
        //[Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductOnShipArea),
        //    ErrorMessageResourceName = "ShipAreaRequired")]
        //public Guid ShipArea { get; set; }

        /// <summary>
        /// 主键表ID，区域ID，可以是任意层级
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductOnShipArea),
            ErrorMessageResourceName = "RegionIDRequired")]
        public Guid RegionID { get; set; }

        /// <summary>
        /// 包含所有末级区域，查询使用递归算法，如果设置了末级，则立即将所有下级记录清除
        /// </summary>
        public bool Terminal { get; set; }

        /// <summary>
        /// 上架商品承运商主键表
        /// </summary>
        [ForeignKey("OnShip")]
        public virtual ProductOnShipping OnShipping { get; set; }

        ///// <summary>
        ///// 上架商品承运地区主键表
        ///// </summary>
        //[ForeignKey("ShipArea")]
        //public virtual ShippingArea ShippingArea { get; set; }

        /// <summary>
        /// 地区主键表
        /// </summary>
        [ForeignKey("RegionID")]
        public virtual GeneralRegion Region { get; set; }
    }
}
