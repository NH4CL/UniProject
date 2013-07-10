using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Finance;

namespace LiveAzure.Models.Product
{
    /// <summary>
    /// 上架商品支持的付款方式
    /// </summary>
    /// <see cref="ProductOnSale"/>
    /// <see cref="FinancePayType"/>
    /// <seealso cref="ProductOnTemplate"/>
    public class ProductOnPayment : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，上架商品ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductOnPayment),
            ErrorMessageResourceName = "OnSaleIDRequired")]
        public Guid OnSaleID { get; set; }

        /// <summary>
        /// 主键表ID，支付方式ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductOnPayment),
            ErrorMessageResourceName = "PayIDRequired")]
        public Guid PayID { get; set; }

        /// <summary>
        /// 上架商品主键表
        /// </summary>
        [ForeignKey("OnSaleID")]
        public virtual ProductOnSale OnSale { get; set; }

        /// <summary>
        /// 支付方式主键表
        /// </summary>
        [ForeignKey("PayID")]
        public virtual FinancePayType PayType { get; set; }
    }
}
