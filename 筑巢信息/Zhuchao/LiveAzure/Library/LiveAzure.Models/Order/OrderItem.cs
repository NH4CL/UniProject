using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Product;

namespace LiveAzure.Models.Order
{
    /// <summary>
    /// 订单商品表
    /// </summary>
    /// <see cref="OrderInformation"/>
    /// <see cref="ProductOnItem"/>
    public class OrderItem : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，订单ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderItem),
            ErrorMessageResourceName = "OrderIDRequired")]
        public Guid OrderID { get; set; }

        /// <summary>
        /// 主键表ID，上架SKU ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderItem),
            ErrorMessageResourceName = "OnSkuIDRequired")]
        public Guid OnSkuID { get; set; }

        /// <summary>
        /// 主键表ID，产品SKU ID，冗余字段
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderItem),
            ErrorMessageResourceName = "SkuIDRequired")]
        public Guid SkuID { get; set; }

        /// <summary>
        /// 购买时的商品名称，购买时的语言，仅供参考
        /// </summary>
        [StringLength(512, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderItem),
            ErrorMessageResourceName = "NameLong")]
        public string Name { get; set; }

        /// <summary>
        /// 数量，多次购买合并
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// 待发货数
        /// </summary>
        public decimal TobeShip { get; set; }

        /// <summary>
        /// 已发货数
        /// </summary>
        public decimal Shipped { get; set; }

        /// <summary>
        /// 待退货数
        /// </summary>
        public decimal BeReturn { get; set; }

        /// <summary>
        /// 已退货数
        /// </summary>
        public decimal Returned { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        [Column(TypeName = "money")]
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// 销售价
        /// </summary>
        [Column(TypeName = "money")]
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 实际执行价
        /// </summary>
        [Column(TypeName = "money")]
        public decimal ExecutePrice { get; set; }

        /// <summary>
        /// 备用 SKU消费积分，积分按实际支付现金
        /// </summary>
        public int SkuPoint { get; set; }

        /// <summary>
        /// 主键表，订单表
        /// </summary>
        [ForeignKey("OrderID")]
        public virtual OrderInformation Order { get; set; }

        /// <summary>
        /// 主键表，上架SKU
        /// </summary>
        [ForeignKey("OnSkuID")]
        public virtual ProductOnItem OnSkuItem { get; set; }

        /// <summary>
        /// 主键表，产品SKU
        /// </summary>
        [ForeignKey("SkuID")]
        public virtual ProductInfoItem SkuItem { get; set; }
    }
}
