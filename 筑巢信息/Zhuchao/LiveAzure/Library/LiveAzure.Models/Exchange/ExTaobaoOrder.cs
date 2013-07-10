using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;
using LiveAzure.Models.Order;

namespace LiveAzure.Models.Exchange
{
    /// <summary>
    /// 淘宝订单中间表
    /// </summary>
    public class ExTaobaoOrder : LiveAzure.Models.ModelBase
    {
        public ExTaobaoOrder()
        {
            TaobaoOrderItems = new List<ExTaobaoOrdItem>();
            TaobaoPromotions = new List<ExTaobaoPromotion>();
            TaobaoTradeRates = new List<ExTaobaoTradeRate>();
            TaobaoRefunds = new List<ExTaobaoRefund>();
        }
        public Guid OrgID { get; set; }
        public Guid ChlID { get; set; }
        public Guid? OrderID { get; set; }
        public bool Transfered { get; set; }
        public bool Changed { get; set; }
        public DateTime? end_time { get; set; }
        public string buyer_message { get; set; }
        public string shipping_type { get; set; }
        public string buyer_cod_fee { get; set; }
        public string seller_cod_fee { get; set; }
        public string express_agency_fee { get; set; }
        public string alipay_warn_msg { get; set; }
        public string status { get; set; }
        public string buyer_memo { get; set; }
        public string seller_memo { get; set; }
        public DateTime? modified { get; set; }
        public long buyer_flag { get; set; }
        public long seller_flag { get; set; }
        public string trade_from { get; set; }
        public string seller_nick { get; set; }
        public string buyer_nick { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public DateTime? created { get; set; }
        public string iid { get; set; }
        public string price { get; set; }
        public string pic_path { get; set; }
        public long num { get; set; }
        public long tid { get; set; }
        public string alipay_no { get; set; }
        public string payment { get; set; }
        public string discount_fee { get; set; }
        public string adjust_fee { get; set; }
        public string snapshot_url { get; set; }
        public string snapshot { get; set; }
        public bool seller_rate { get; set; }
        public bool buyer_rate { get; set; }
        public string trade_memo { get; set; }
        public DateTime? pay_time { get; set; }
        public long buyer_obtain_point_fee { get; set; }
        public long point_fee { get; set; }
        public long real_point_fee { get; set; }
        public string total_fee { get; set; }
        public string post_fee { get; set; }
        public string buyer_alipay_no { get; set; }
        public string receiver_name { get; set; }
        public string receiver_state { get; set; }
        public string receiver_city { get; set; }
        public string receiver_district { get; set; }
        public string receiver_address { get; set; }
        public string receiver_zip { get; set; }
        public string receiver_mobile { get; set; }
        public string receiver_phone { get; set; }
        public DateTime? consign_time { get; set; }
        public string buyer_email { get; set; }
        public string commission_fee { get; set; }
        public string seller_alipay_no { get; set; }
        public string seller_mobile { get; set; }
        public string seller_phone { get; set; }
        public string seller_name { get; set; }
        public string seller_email { get; set; }
        public string available_confirm_fee { get; set; }
        public bool has_post_fee { get; set; }
        public string received_payment { get; set; }
        public string cod_fee { get; set; }
        public string cod_status { get; set; }
        public DateTime? timeout_action_time { get; set; }
        public bool is_3D { get; set; }
        public long num_iid { get; set; }
        public string promotion { get; set; }
        public string invoice_name { get; set; }
        public string alipay_url { get; set; }

        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }

        [ForeignKey("ChlID")]
        public virtual MemberChannel Channel { get; set; }

        [ForeignKey("OrderID")]
        public virtual OrderInformation Order { get; set; }

        [InverseProperty("TaobaoOrder")]
        public virtual ICollection<ExTaobaoOrdItem> TaobaoOrderItems { get; set; }

        [InverseProperty("TaobaoOrder")]
        public virtual ICollection<ExTaobaoPromotion> TaobaoPromotions { get; set; }

        [InverseProperty("TaobaoOrder")]
        public virtual ICollection<ExTaobaoTradeRate> TaobaoTradeRates { get; set; }

        [InverseProperty("TaobaoOrder")]
        public virtual ICollection<ExTaobaoRefund> TaobaoRefunds { get; set; }
    }
}
