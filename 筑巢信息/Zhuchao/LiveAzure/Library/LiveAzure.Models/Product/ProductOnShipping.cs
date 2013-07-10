using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;
using LiveAzure.Models.Shipping;

namespace LiveAzure.Models.Product
{
    /// <summary>
    /// 上架商品支持的运输公司，运费策略等
    /// </summary>
    /// <see cref="ProductOnSale"/>
    /// <see cref="ShippingInformation"/>
    /// <seealso cref="ProductOnTemplate"/>
    public class ProductOnShipping : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，上架商品ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductOnShipping),
            ErrorMessageResourceName = "OnSaleIDRequired")]
        public Guid OnSaleID { get; set; }

        /// <summary>
        /// 主键表ID，运输公司ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductOnShipping),
            ErrorMessageResourceName = "ShipIDRequired")]
        public Guid ShipID { get; set; }

        /// <summary>
        /// 计费方案 0按重量 1按体积 2按件
        /// </summary>
        /// <see cref="ModelEnum.BillingType"/>
        public byte Solution { get; set; }

        /// <summary>
        /// 权重，大者优先
        /// </summary>
        public int ShipWeight { get; set; }

        /// <summary>
        /// 小于条件，例如小于kg使用此方式，大于的则到下一个匹配运输公司，0表示无条件
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Condition { get; set; }

        /// <summary>
        /// 对标准资费的打折优惠，允许大于1
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Discount { get; set; }

        /// <summary>
        /// 支持货到付款，与shiparea与运算
        /// </summary>
        public bool SupportCod { get; set; }

        /// <summary>
        /// 上架商品主键表
        /// </summary>
        [ForeignKey("OnSaleID")]
        public virtual ProductOnSale OnSale { get; set; }

        /// <summary>
        /// 主键表，承运人
        /// </summary>
        [ForeignKey("ShipID")]
        public virtual ShippingInformation Shipper { get; set; }

        /// <summary>
        /// 从表内容，可配送地区
        /// </summary>
        [InverseProperty("OnShipping")]
        public virtual ICollection<ProductOnShipArea> OnShipArea { get; set; }

        /// <summary>
        /// 获取计费方案类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> SolutionList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.BillingType), this.Solution); }
        }

        /// <summary>
        /// 某个计费方案类型的名称
        /// </summary>
        [NotMapped]
        public string SolutionName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.BillingType), this.Solution); }
        }
    }
}
