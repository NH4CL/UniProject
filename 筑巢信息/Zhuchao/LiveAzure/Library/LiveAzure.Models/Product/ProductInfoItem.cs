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
    /// 产品基础信息－SKU
    /// </summary>
    /// <see cref="ProductInformation"/>
    /// <see cref="ProductGallery"/>
    /// <see cref="ProductOnItem"/>
    public class ProductInfoItem : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键ID，组织ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductInfoItem),
            ErrorMessageResourceName = "OrgIDRequired")]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductInfoItem),
            ErrorMessageResourceName = "ProdIDRequired")]
        public Guid ProdID { get; set; }

        /// <summary>
        /// SKU代码，组织内唯一
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductInfoItem),
            ErrorMessageResourceName = "CodeRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductInfoItem),
            ErrorMessageResourceName = "CodeLong")]
        public string Code { get; set; }

        /// <summary>
        /// 条码，组织内唯一
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductInfoItem),
            ErrorMessageResourceName = "BarcodeLong")]
        public string Barcode { get; set; }

        /// <summary>
        /// 自定义编码，在XML中定义编码的名称
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductInfoItem),
            ErrorMessageResourceName = "CodeEx1Long")]
        public string CodeEx1 { get; set; }

        /// <summary>
        /// 自定义编码
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductInfoItem),
            ErrorMessageResourceName = "CodeEx2Long")]
        public string CodeEx2 { get; set; }

        /// <summary>
        /// 自定义编码
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductInfoItem),
            ErrorMessageResourceName = "CodeEx3Long")]
        public string CodeEx3 { get; set; }

        /// <summary>
        /// SKU的完整名称
        /// </summary>
        [Column("FullName")]
        public Guid? aFullName { get; set; }

        /// <summary>
        /// 产品简称，小图片的提示文字
        /// </summary>
        [Column("ShortName")]
        public Guid? aShortName { get; set; }

        /// <summary>
        /// 标准计量单位，入/出库时的计量单位
        /// </summary>
        public Guid StdUnit { get; set; }

        /// <summary>
        /// 规格说明
        /// </summary>
        [Column("Specification")]
        public Guid? aSpecification { get; set; }

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
        /// 建议单价
        /// </summary>
        [Column("SuggestPrice")]
        public Guid? aSuggestPrice { get; set; }

        /// <summary>
        /// 最低售价，预警价格
        /// </summary>
        [Column("LowestPrice")]
        public Guid? aLowestPrice { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        [Column(TypeName = "money")]
        public decimal GrossWeight { get; set; }

        /// <summary>
        /// 净重
        /// </summary>
        [Column(TypeName = "money")]
        public decimal NetWeight { get; set; }

        /// <summary>
        /// 毛体积
        /// </summary>
        [Column(TypeName = "money")]
        public decimal GrossVolume { get; set; }

        /// <summary>
        /// 净体积
        /// </summary>
        [Column(TypeName = "money")]
        public decimal NetVolume { get; set; }

        /// <summary>
        /// 表面积
        /// </summary>
        [Column(TypeName = "money")]
        public decimal NetArea { get; set; }

        /// <summary>
        /// 计件
        /// </summary>
        public int NetPiece { get; set; }

        /// <summary>
        /// 组织主键表
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }

        /// <summary>
        /// 产品主键表
        /// </summary>
        [ForeignKey("ProdID")]
        public virtual ProductInformation Product { get; set; }

        /// <summary>
        /// 产品主键表
        /// </summary>
        [ForeignKey("aFullName")]
        public virtual GeneralResource FullName { get; set; }

        /// <summary>
        /// 产品主键表
        /// </summary>
        [ForeignKey("aShortName")]
        public virtual GeneralResource ShortName { get; set; }

        /// <summary>
        /// 计量单位主键表
        /// </summary>
        [ForeignKey("StdUnit")]
        public virtual GeneralMeasureUnit StandardUnit { get; set; }

        /// <summary>
        /// 规则主键表
        /// </summary>
        [ForeignKey("aSpecification")]
        public virtual GeneralResource Specification { get; set; }

        /// <summary>
        /// 市场价主键表
        /// </summary>
        [ForeignKey("aMarketPrice")]
        public virtual GeneralResource MarketPrice { get; set; }

        /// <summary>
        /// 建议价主键表
        /// </summary>
        [ForeignKey("aSuggestPrice")]
        public virtual GeneralResource SuggestPrice { get; set; }

        /// <summary>
        /// 最低售价，预警价主键表
        /// </summary>
        [ForeignKey("aLowestPrice")]
        public virtual GeneralResource LowestPrice { get; set; }

        /// <summary>
        /// 从表内容，SKU商品图片
        /// </summary>
        [InverseProperty("SkuItem")]
        public virtual ICollection<ProductGallery> Galleries { get; set; }

        /// <summary>
        /// 从表内容，OnSku上架商品
        /// </summary>
        [InverseProperty("SkuItem")]
        public virtual ICollection<ProductOnItem> OnSkuItems { get; set; }
    }
}
