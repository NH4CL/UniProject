using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LiveAzure.Models.Exchange
{
    /// <summary>
    /// 淘宝订单评价中间表
    /// </summary>
    public class ExTaobaoTradeRate : LiveAzure.Models.ModelBase
    {
        public Guid TopOrderID { get; set; }
        public bool valid_score { get; set; }
        public long tid { get; set; }
        public long oid { get; set; }
        public string role { get; set; }
        public string nick { get; set; }
        public string result { get; set; }
        public DateTime? created { get; set; }
        public string rated_nick { get; set; }
        public string item_title { get; set; }
        public string item_price { get; set; }
        public string content { get; set; }
        public string reply { get; set; }

        [ForeignKey("TopOrderID")]
        public ExTaobaoOrder TaobaoOrder { get; set; }
    }
}
