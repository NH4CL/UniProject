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
    /// 产品基础信息
    /// </summary>
    /// <see cref="MemberOrganization"/>
    /// <see cref="ProductInfoItem"/>
    public class ProductInformation : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键ID，所属组织ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductInformation),
            ErrorMessageResourceName = "OrgIDRequired")]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 产品代码，PU码，组织内唯一
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductInformation),
            ErrorMessageResourceName = "CodeRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductInformation),
            ErrorMessageResourceName = "CodeLong")]
        public string Code { get; set; }

        /// <summary>
        /// 主键ID,商品名称
        /// </summary>
        [Column("Name")]
        public Guid? aName { get; set; }

        /// <summary>
        /// 标准分类
        /// </summary>
        public Guid? StdCatID { get; set; }

        /// <summary>
        /// 拆单分组标志，不同组的商品可能需要拆单，或变问题单，直接程序中定义，或xml中定义？
        /// </summary>
        /// <see cref="ModelEnum.SplitBlock"/>
        public byte Block { get; set; }

        /// <summary>
        /// 产品模式 0:pu-sku模式，1:pu-配件模式
        /// </summary>
        /// <see cref="ModelEnum.ProductMode"/>
        public byte Mode { get; set; }

        /// <summary>
        /// 商品主图路径，PU列表的图片
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductInformation),
            ErrorMessageResourceName = "PictureLong")]
        public string Picture { get; set; }

        /// <summary>
        /// 商品简单描述
        /// </summary>
        [Column("Brief")]
        public Guid? aBrief { get; set; }

        /// <summary>
        /// 商品详细描述
        /// </summary>
        [Column("Matter")]
        public Guid? aMatter { get; set; }

        /// <summary>
        /// 起订量
        /// </summary>
        [Column(TypeName = "money")]
        public decimal MinQuantity { get; set; }

        /// <summary>
        /// 生产周期（天）
        /// </summary>
        public int ProductionCycle { get; set; }

        /// <summary>
        /// 默认保质期（天）
        /// </summary>
        public int GuaranteeDays { get; set; }

        /// <summary>
        /// 关键词，供搜索引擎使用
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductInformation),
            ErrorMessageResourceName = "KeywordsLong")]
        public string Keywords { get; set; }

        /// <summary>
        /// 联盟备用 销售类型，0直营、1联营 2代销
        /// </summary>
        /// <remarks>废弃，使用Public角色代替</remarks>
        [Obsolete]
        public byte SaleType { get; set; }

        /// <summary>
        /// 组织主键表
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }

        /// <summary>
        /// 名称主键表
        /// </summary>
        [ForeignKey("aName")]
        public virtual GeneralResource Name { get; set; }

        /// <summary>
        /// 分类主键表
        /// </summary>
        [ForeignKey("StdCatID")]
        public virtual GeneralStandardCategory StandardCategory { get; set; }

        /// <summary>
        /// 商品简单描述
        /// </summary>
        [ForeignKey("aBrief")]
        public virtual GeneralResource Brief { get; set; }

        /// <summary>
        /// 商品详细描述
        /// </summary>
        [ForeignKey("aMatter")]
        public virtual GeneralLargeObject Matter { get; set; }

        /// <summary>
        /// 从表内容，SKU
        /// </summary>
        [InverseProperty("Product")]
        public virtual ICollection<ProductInfoItem> SkuItems { get; set; }

        /// <summary>
        /// 从表内容，扩展分类
        /// </summary>
        [InverseProperty("Product")]
        public virtual ICollection<ProductExtendCategory> ExtendCategories { get; set; }

        /// <summary>
        /// 从表内容，扩展属性
        /// </summary>
        [InverseProperty("Product")]
        public virtual ICollection<ProductExtendAttribute> ExtendAttributes { get; set; }

        /// <summary>
        /// 从表内容，PU商品图片
        /// </summary>
        [InverseProperty("Product")]
        public virtual ICollection<ProductGallery> Galleries { get; set; }

        /// <summary>
        /// 从表内容，上架商品，可以统计上架次数
        /// </summary>
        [InverseProperty("Product")]
        public virtual ICollection<ProductOnSale> OnSales { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> ProductModeList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.ProductMode), this.Mode); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string ProductModeName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.ProductMode), this.Mode); }
        }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> BlockList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.SplitBlock), this.Block); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string BlockName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.SplitBlock), this.Block); }
        }
    }
}
