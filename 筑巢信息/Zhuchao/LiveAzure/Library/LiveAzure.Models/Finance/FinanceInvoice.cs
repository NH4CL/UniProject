using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Order;

namespace LiveAzure.Models.Finance
{
    /// <summary>
    /// 发票信息
    /// </summary>
    public class FinanceInvoice : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 订单号
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Finance.FinanceInvoice),
            ErrorMessageResourceName = "OrderIDRequired")]
        public Guid OrderID { get; set; }

        /// <summary>
        /// 发票号，订单内唯一
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Finance.FinanceInvoice),
            ErrorMessageResourceName = "CodeRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Finance.FinanceInvoice),
            ErrorMessageResourceName = "CodeLong")]
        public string Code { get; set; }

        /// <summary>
        /// 发票抬头
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 发票内容
        /// </summary>
        public string Matter { get; set; }

        /// <summary>
        /// 状态 0未开票 1已开票
        /// </summary>
        public byte Istatus { get; set; }

        /// <summary>
        /// 发票金额
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 配送情况，例如挂号信号码
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Finance.FinanceInvoice),
            ErrorMessageResourceName = "SendNoteLong")]
        public string SendNote { get; set; }

        /// <summary>
        /// 订单主键表
        /// </summary>
        [ForeignKey("OrderID")]
        public virtual OrderInformation OrderInfo { get; set; }
    }
}
