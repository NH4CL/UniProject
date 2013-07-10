using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LiveAzure.Models.Exchange
{
    /// <summary>
    /// 淘宝订单SKU项中间表
    /// </summary>
    public class ExTaobaoOrdItem : LiveAzure.Models.ModelBase
    {
        public Guid TopOrderID { get; set; }
        public string total_fee { get; set; }
        public string discount_fee { get; set; }
        public string adjust_fee { get; set; }
        public string payment { get; set; }
        public DateTime? modified { get; set; }
        public long item_meal_id { get; set; }
        public string status { get; set; }
        public long refund_id { get; set; }
        public string iid { get; set; }
        public string sku_id { get; set; }
        public string sku_properties_name { get; set; }
        public string item_meal_name { get; set; }
        public long num { get; set; }
        public string title { get; set; }
        public string price { get; set; }
        public string pic_path { get; set; }
        public string seller_nick { get; set; }
        public string buyer_nick { get; set; }
        public string refund_status { get; set; }
        public long oid { get; set; }
        public string outer_iid { get; set; }
        public string outer_sku_id { get; set; }
        public string snapshot_url { get; set; }
        public string snapshot { get; set; }
        public DateTime? timeout_action_time { get; set; }
        public bool buyer_rate { get; set; }
        public bool seller_rate { get; set; }
        public string seller_type { get; set; }
        public long num_iid { get; set; }
        public long cid { get; set; }
        public bool is_oversold { get; set; }

        [ForeignKey("TopOrderID")]
        public ExTaobaoOrder TaobaoOrder { get; set; }
    }
}
