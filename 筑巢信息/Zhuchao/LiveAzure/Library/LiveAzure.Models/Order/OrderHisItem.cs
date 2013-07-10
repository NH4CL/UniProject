using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LiveAzure.Models.Order
{
    /// <summary>
    /// 订单变更商品表
    /// </summary>
    /// <see cref="OrderHistory"/>
    public class OrderHisItem : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，变更单ID
        /// </summary>
        [Required (ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Order.OrderHisItem),
            ErrorMessageResourceName="OrderHisIDRequired")]
        public Guid OrderHisID { get; set; }

        /// <summary>
        /// SKU
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderHisItem),
            ErrorMessageResourceName = "OnSkuIDRequired")]
        public Guid OnSkuID { get; set; }

        /// <summary>
        /// 主键表ID，产品SKU ID，冗余字段
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderHisItem),
            ErrorMessageResourceName = "SkuIDRequired")]
        public Guid SkuID { get; set; }

        /// <summary>
        /// 购买时的商品名称，购买时的语言，仅供参考
        /// </summary>
        [StringLength(512, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderHisItem),
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
        /// 待发货数
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
        /// 主键表，变更单
        /// </summary>
        [ForeignKey("OrderHisID")]
        public virtual OrderHistory OrderHistory { get; set; }
    }
}
