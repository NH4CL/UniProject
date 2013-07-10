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
    /// 商品上架－PU
    /// </summary>
    /// <see cref="ProductInformation"/>
    /// <see cref="ProductOnItem"/>
    /// <see cref="ProductOnShipping"/>
    /// <see cref="ProductOnPayment"/>
    /// <see cref="ProductOnRelation"/>
    /// <see cref="MemberOrganization"/>
    public class ProductOnSale : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，PU，每个上架商品必须有一个PU号，用于主分类、属性等等
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductOnSale),
            ErrorMessageResourceName = "ProdIDRequired")]
        public Guid ProdID { get; set; }

        /// <summary>
        /// 组织ID 冗余
        /// </summary>
        public Guid OrgID { get; set; }

        /// <summary>
        /// 主键表ID,渠道ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductOnSale),
            ErrorMessageResourceName = "ChlIDRequired")]
        public Guid ChlID { get; set; }

        /// <summary>
        /// 助记代码，允许一个PU多次上架，以便微调SKU形成新的PU'
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductOnSale),
            ErrorMessageResourceName = "CodeRequired")]
        [StringLength(20, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductOnSale),
            ErrorMessageResourceName = "CodeLong")]
        public string Code { get; set; }

        /// <summary>
        /// 状态 0待审批/下架 1已上架
        /// </summary>
        /// <see cref="ModelEnum.OnSaleStatus"/>
        public byte Ostatus { get; set; }

        /// <summary>
        /// 上架名称，默认为产品名称
        /// </summary>
        [Required]
        [Column("Name")]
        public Guid aName { get; set; }

        /// <summary>
        /// 产品模式 0:pu-sku模式，1:pu-配件模式，按prod默认值
        /// </summary>
        /// <see cref="ModelEnum.ProductMode"/>
        public byte Mode { get; set; }

        /// <summary>
        /// 市场价 模式0，取sku最低，模式1，取sku之和 同货币
        /// </summary>
        [Column("MarketPrice")]
        public Guid? aMarketPrice { get; set; }

        /// <summary>
        /// 销售价 模式0，取sku最低，模式1，取sku之和 同货币
        /// </summary>
        [Column("SalePrice")]
        public Guid? aSalePrice { get; set; }

        /// <summary>
        /// 上架有效期，过期后自动下架
        /// </summary>
        public DateTimeOffset? Validity { get; set; }

        /// <summary>
        /// 是否可拆分退货，仅针对pu-配件模式的拆分退货
        /// </summary>
        public bool CanSplit { get; set; }

        /// <summary>
        /// 主键表，商品简单描述，默认为prod的值
        /// </summary>
        [Column("Brief")]
        public Guid? aBrief { get; set; }

        /// <summary>
        /// 主键表，商品详细描述
        /// </summary>
        [Column("Matter")]
        public Guid? aMatter { get; set; }

        /// <summary>
        /// 商品主图路径，默认为prod的值
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductOnSale),
            ErrorMessageResourceName = "PictureLong")]
        public string Picture { get; set; }

        /// <summary>
        /// 视频地址
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductOnSale),
            ErrorMessageResourceName = "VideoUrlLong")]
        public string VideoUrl { get; set; }

        /// <summary>
        /// 承诺发货天数
        /// </summary>
        public int DeliveryDays { get; set; }

        /// <summary>
        /// 新品排序
        /// </summary>
        public int SortingNew { get; set; }

        /// <summary>
        /// 点击排序
        /// </summary>
        public int SortingClick { get; set; }

        /// <summary>
        /// 热销排序
        /// </summary>
        public int SortingHot { get; set; }

        /// <summary>
        /// 推荐排序
        /// </summary>
        public int SortingPush { get; set; }

        /// <summary>
        /// 备用排序1
        /// </summary>
        public int Sorting01 { get; set; }

        /// <summary>
        /// 备用排序2
        /// </summary>
        public int Sorting02 { get; set; }

        /// <summary>
        /// 备用排序3
        /// </summary>
        public int Sorting03 { get; set; }

        /// <summary>
        /// 产品主键表
        /// </summary>
        [ForeignKey("ProdID")]
        public virtual ProductInformation Product { get; set; }

        /// <summary>
        /// 组织主键表
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }

        /// <summary>
        /// 渠道主键表
        /// </summary>
        [ForeignKey("ChlID")]
        public virtual MemberChannel Channel { get; set; }

        /// <summary>
        /// 名称主键表
        /// </summary>
        [ForeignKey("aName")]
        public virtual GeneralResource Name { get; set; }

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

        /// <summary>
        /// 商品描述主键表
        /// </summary>
        [ForeignKey("aBrief")]
        public virtual GeneralResource Brief { get; set; }

        /// <summary>
        /// 商品详细描述主键表
        /// </summary>
        [ForeignKey("aMatter")]
        public virtual GeneralLargeObject Matter { get; set; }

        /// <summary>
        /// 从表内容，上架商品SKU
        /// </summary>
        [InverseProperty("OnSale")]
        public virtual ICollection<ProductOnItem> OnSkuItems { get; set; }

        /// <summary>
        /// 从表内容，上架商品PU支持的运输公司
        /// </summary>
        [InverseProperty("OnSale")]
        public virtual ICollection<ProductOnShipping> OnShippings { get; set; }

        /// <summary>
        /// 从表内容，上架商品PU支持的支付方式
        /// </summary>
        [InverseProperty("OnSale")]
        public virtual ICollection<ProductOnPayment> OnPayments { get; set; }

        /// <summary>
        /// 从表内容，上架商品PU的关联商品
        /// </summary>
        [InverseProperty("OnSale")]
        public virtual ICollection<ProductOnRelation> OnRelations { get; set; }

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
        public List<ListItem> OnSaleStatusList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.OnSaleStatus), this.Ostatus); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string OnSaleStatusName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.OnSaleStatus), this.Ostatus); }
        }
    }
}
