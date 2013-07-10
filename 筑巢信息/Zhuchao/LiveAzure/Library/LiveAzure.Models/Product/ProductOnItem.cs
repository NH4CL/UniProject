using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.General;

namespace LiveAzure.Models.Product
{
    /// <summary>
    /// 商品上架明细
    /// </summary>
    /// <see cref="ProductOnSale"/>
    /// <see cref="ProductInfoItem"/>
    /// <see cref="ProductOnUnitPrice"/>
    public class ProductOnItem : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，上架商品ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductOnItem),
            ErrorMessageResourceName = "OnSaleIDRequired")]
        public Guid OnSaleID { get; set; }

        /// <summary>
        /// 主键表ID，Sku ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductOnItem),
            ErrorMessageResourceName = "SkuIDRequired")]
        public Guid SkuID { get; set; }

        /// <summary>
        /// 主键表ID，SKU的完整名称
        /// </summary>
        [Column("FullName")]
        public Guid? aFullName { get; set; }

        /// <summary>
        /// 主键表ID，产品简称，小图片的提示文字
        /// </summary>
        [Column("ShortName")]
        public Guid? aShortName { get; set; }

        /// <summary>
        /// 排序，在onsale.gid内的排序
        /// </summary>
        public int Sorting { get; set; }

        /// <summary>
        /// 套装数量，例如一个PU中有两个相同的SKU同时销售
        /// </summary>
        [Column(TypeName = "money")]
        public decimal SetQuantity { get; set; }

        /// <summary>
        /// 上架数量，最大销售数量，0表示强制提示缺货，-1表示按实际库存销售
        /// MaxQuantity, OnTheWay, Overflow 优先级，待定？
        /// </summary>
        [Column(TypeName = "money")]
        public decimal MaxQuantity { get; set; }

        /// <summary>
        /// 是否允许预售在途数量
        /// </summary>
        public bool OnTheWay { get; set; }

        /// <summary>
        /// 是否允许超卖
        /// </summary>
        public bool Overflow { get; set; }

        /// <summary>
        /// 关联商品标识 0普通 1主商品 2从商品,保留小数 3从商品,四舍五入 4从商品,进位法 5从商品,舍去法
        /// </summary>
        /// <see cref="ModelEnum.OnSaleDependence"/>
        public byte DependTag { get; set; }

        /// <summary>
        /// 与主商品的数量比例
        /// </summary>
        [Column(TypeName = "money")]
        public decimal DependRate { get; set; }
        
        /// <summary>
        /// 使用积分购买的积分值
        /// </summary>
        public int UseScore { get; set; }

        /// <summary>
        /// 使用积分抵扣的金额
        /// </summary>
        [Column("ScoreDeduct")]
        public Guid? aScoreDeduct { get; set; }

        /// <summary>
        /// 购买成功，获取积分数，xml中配置n天后生效
        /// </summary>
        public int GetScore { get; set; }

        /// <summary>
        /// 上架商品主键表
        /// </summary>
        [ForeignKey("OnSaleID")]
        public virtual ProductOnSale OnSale { get; set; }

        /// <summary>
        /// Sku主键表
        /// </summary>
        [ForeignKey("SkuID")]
        public virtual ProductInfoItem SkuItem { get; set; }

        /// <summary>
        /// SKU完整名称主键表
        /// </summary>
        [ForeignKey("aFullName")]
        public virtual GeneralResource FullName { get; set; }

        /// <summary>
        /// 产品简称主键表
        /// </summary>
        [ForeignKey("aShortName")]
        public virtual GeneralResource ShortName { get; set; }

        /// <summary>
        /// 允许使用积分抵扣主键表
        /// </summary>
        [ForeignKey("aScoreDeduct")]
        public virtual GeneralResource ScoreDeduct { get; set; }

        /// <summary>
        /// 从表内容，上架商品OnSku的计量单位
        /// </summary>
        [InverseProperty("OnSkuItem")]
        public virtual ICollection<ProductOnUnitPrice> OnSkuPrices { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> DependenceList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.OnSaleDependence), this.DependTag); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string DependenceName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.OnSaleDependence), this.DependTag); }
        }
    }
}
