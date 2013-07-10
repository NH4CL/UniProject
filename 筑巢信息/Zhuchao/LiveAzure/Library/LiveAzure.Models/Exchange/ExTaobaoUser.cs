using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LiveAzure.Models.Exchange
{
    /// <summary>
    /// 淘宝用户表
    /// </summary>
    public class ExTaobaoUser : LiveAzure.Models.ModelBase
    {
        public int CompareTo(object obj)
        {
            ExTaobaoUser oUser = (ExTaobaoUser)obj;

            return 0;
        }

        public long user_id { get; set; }
        public string uid { get; set; }
        public string nick { get; set; }
        public string sex { get; set; }
        public Guid? buyer_credit { get; set; }
        public Guid? seller_credit { get; set; }
        public DateTime? created { get; set; }
        public DateTime? last_visit { get; set; }
        public DateTime? birthday { get; set; }
        public string type { get; set; }
        public bool has_more_pic { get; set; }
        public long item_img_num { get; set; }
        public long item_img_size { get; set; }
        public long prop_img_num { get; set; }
        public long prop_img_size { get; set; }
        public string auto_repost { get; set; }
        public string promoted_type { get; set; }
        public string status { get; set; }
        public string alipay_bind { get; set; }
        public bool consumer_protection { get; set; }
        public string alipay_account { get; set; }
        public string alipay_no { get; set; }
        public string avatar { get; set; }
        public bool liangpin { get; set; }
        public bool sign_food_seller_promise { get; set; }
        public bool has_shop { get; set; }
        public bool is_lightning_consignment { get; set; }
        public string vip_info { get; set; }
        public string email { get; set; }
        public bool magazine_subscribe { get; set; }
        public string vertical_market { get; set; }
        public bool online_gaming { get; set; }

        [ForeignKey("buyer_credit")]
        public virtual ExTaobaoUserCredit BuyerCredit { get; set; }

        [ForeignKey("seller_credit")]
        public virtual ExTaobaoUserCredit SellerCredit { get; set; }
    }
}
