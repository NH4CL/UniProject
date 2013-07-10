using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.General;

namespace LiveAzure.Models.Shipping
{
    /// <summary>
    /// 运输公司支持的区域和标准费率定义
    /// </summary>
    /// <see cref="ShippingInformation"/>
    /// <see cref="GeneralRegion"/>
    public class ShippingArea : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，运输公司ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Shipping.ShippingArea),
            ErrorMessageResourceName = "ShipIDRequired")]
        public Guid ShipID { get; set; }

        /// <summary>
        /// 主键表ID，区域ID，可以是任意层级
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Shipping.ShippingArea),
            ErrorMessageResourceName = "RegionIDRequired")]
        public Guid RegionID { get; set; }

        /// <summary>
        /// 包含所有末级区域，查询使用递归算法，如果设置了末级，则立即将所有下级记录清除
        /// </summary>
        public bool Terminal { get; set; }

        /// <summary>
        /// 是否支持货到付款
        /// </summary>
        public bool SupportCod { get; set; }

        /// <summary>
        /// 到门价格 null表示不支持
        /// </summary>
        [Column("Residential")]
        public Guid? aResidential { get; set; }

        /// <summary>
        /// 升降机服务/上楼费
        /// </summary>
        [Column("LiftGate")]
        public Guid? aLiftGate { get; set; }

        /// <summary>
        /// 安装服务
        /// </summary>
        [Column("Installation")]
        public Guid? aInstallation { get; set; }

        /// <summary>
        /// 重量费率
        /// </summary>
        [Column("PriceWeight")]
        public Guid? aPriceWeight { get; set; }

        /// <summary>
        /// 体积费率
        /// </summary>
        [Column("PriceVolume")]
        public Guid? aPriceVolume { get; set; }

        /// <summary>
        /// 计件费率
        /// </summary>
        [Column("PricePiece")]
        public Guid? aPricePiece { get; set; }

        /// <summary>
        /// 运费总价最高值，null表示不封顶
        /// </summary>
        [Column("PriceHigh")]
        public Guid? aPriceHigh { get; set; }

        /// <summary>
        /// 运费总价最低值
        /// </summary>
        [Column("PriceLow")]
        public Guid? aPriceLow { get; set; }

        /// <summary>
        /// 主键表，运输公司
        /// </summary>
        [ForeignKey("ShipID")]
        public virtual ShippingInformation Shipping { get; set; }

        /// <summary>
        /// 区域主键表
        /// </summary>
        [ForeignKey("RegionID")]
        public virtual GeneralRegion Region { get; set; }

        /// <summary>
        /// 到门价格
        /// </summary>
        [ForeignKey("aResidential")]
        public virtual GeneralResource Residential { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ForeignKey("aLiftGate")]
        public virtual GeneralResource LiftGate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ForeignKey("aInstallation")]
        public virtual GeneralResource Installation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ForeignKey("aPriceWeight")]
        public virtual GeneralResource PriceWeight { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ForeignKey("aPriceVolume")]
        public virtual GeneralResource PriceVolume { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ForeignKey("aPricePiece")]
        public virtual GeneralResource PricePiece { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ForeignKey("aPriceHigh")]
        public virtual GeneralResource PriceHigh { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ForeignKey("aPriceLow")]
        public virtual GeneralResource PriceLow { get; set; }
    }
}
