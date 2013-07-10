using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LiveAzure.Models.Exchange
{
    /// <summary>
    /// 淘宝订单促销项中间表
    /// </summary>
    public class ExTaobaoPromotion : LiveAzure.Models.ModelBase
    {
        public Guid TopOrderID { get; set; }
        public long id { get; set; }
        public string promotion_name { get; set; }
        public string discount_fee { get; set; }
        public string gift_item_name { get; set; }

        [ForeignKey("TopOrderID")]
        public ExTaobaoOrder TaobaoOrder { get; set; }
    }
}
