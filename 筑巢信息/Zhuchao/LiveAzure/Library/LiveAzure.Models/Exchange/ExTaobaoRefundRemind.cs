using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LiveAzure.Models.Exchange
{
    /// <summary>
    /// 淘宝订单退款中间表
    /// </summary>
    public class ExTaobaoRefundRemind : LiveAzure.Models.ModelBase
    {
        public Guid RefundID { get; set; }
        public long remind_type { get; set; }
        public bool exist_timeout { get; set; }
        public DateTime? timeout { get; set; }

        [ForeignKey("RefundID")]
        public ExTaobaoRefund ExTaobaoRefund { get; set; }
    }
}
