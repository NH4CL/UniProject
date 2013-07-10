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
    public class ExTaobaoRefund : LiveAzure.Models.ModelBase
    {
        public ExTaobaoRefund()
        {
            TaobaoRefundReminds = new List<ExTaobaoRefundRemind>();
        }

        public Guid TopOrderID { get; set; }
        public string shipping_type { get; set; }
        public long refund_id { get; set; }
        public long tid { get; set; }
        public long oid { get; set; }
        public string alipay_no { get; set; }
        public string total_fee { get; set; }
        public string buyer_nick { get; set; }
        public string seller_nick { get; set; }
        public DateTime? created { get; set; }
        public DateTime? modified { get; set; }
        public string order_status { get; set; }
        public string status { get; set; }
        public string good_status { get; set; }
        public bool has_good_return { get; set; }
        public string refund_fee { get; set; }
        public string payment { get; set; }
        public string reason { get; set; }
        public string desc { get; set; }
        public string iid { get; set; }
        public string title { get; set; }
        public string price { get; set; }
        public long num { get; set; }
        public DateTime? good_return_time { get; set; }
        public string company_name { get; set; }
        public string sid { get; set; }
        public string address { get; set; }
        public long num_iid { get; set; }

        [ForeignKey("TopOrderID")]
        public ExTaobaoOrder TaobaoOrder { get; set; }

        [InverseProperty("ExTaobaoRefund")]
        public virtual ICollection<ExTaobaoRefundRemind> TaobaoRefundReminds { get; set; }
    }
}
