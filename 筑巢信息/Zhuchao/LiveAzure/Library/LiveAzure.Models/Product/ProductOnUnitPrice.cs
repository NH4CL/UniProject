using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;
using LiveAzure.Models.General;

namespace LiveAzure.Models.Product
{
    /// <summary>
    /// 产品上架－SKU计量单位
    /// </summary>
    /// <see cref="ProductOnItem"/>
    public class ProductOnUnitPrice : LiveAzure.Models.ModelBase
    {
        public ProductOnUnitPrice()
        {
            this.UnitRatio = 1;
            this.Percision = 0;
        }

        /// <summary>
        /// 主键ID，上架OnSkuID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductOnUnitPrice),
            ErrorMessageResourceName = "OnSkuIDRequired")]
        public Guid OnSkuID { get; set; }

        /// <summary>
        /// 显示计量单位，即销售的计量单位
        /// </summary>
        [Required]
        [Column("ShowUnit")]
        public Guid aShowUnit { get; set; }

        /// <summary>
        /// 默认值，用于类似淘宝等，没有指定计量单位，则使用默认值的计量单位
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 到标准单位sku.stdunit转换比率
        /// </summary>
        [Column(TypeName = "money")]
        public decimal UnitRatio { get; set; }

        /// <summary>
        /// 计量精度，0整数 2保留两位小数
        /// </summary>
        public byte Percision { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        [Column("MarketPrice")]
        public Guid? aMarketPrice { get; set; }

        /// <summary>
        /// 销售价
        /// </summary>
        [Column("SalePrice")]
        public Guid? aSalePrice { get; set; }

        /// <summary>
        /// 上架OnSku主键表
        /// </summary>
        [ForeignKey("OnSkuID")]
        public virtual ProductOnItem OnSkuItem { get; set; }

        /// <summary>
        /// 计量单位主键表
        /// </summary>
        [ForeignKey("aShowUnit")]
        public virtual GeneralMeasureUnit ShowUnit { get; set; }

        /// <summary>
        /// 市场价主键表
        /// </summary>
        [ForeignKey("aMarketPrice")]
        public virtual GeneralResource MarketPrice { get; set; }

        /// <summary>
        /// 销售价主键表
        /// </summary>
        [ForeignKey("aSalePrice")]
        public virtual GeneralResource SalePrice { get; set; }
    }
}
