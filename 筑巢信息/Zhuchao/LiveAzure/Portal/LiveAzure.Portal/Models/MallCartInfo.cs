using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveAzure.Portal.Models
{
    public class MallCartInfo
    {
        //组织Gid
        public Guid orgGid { set; get; }

        //购物车组织信息
        public string organizationName { set; get; }

        //商品总数
        public decimal productCount { get; set; }

        //赠送的积分
        public int backPoint { get; set; }

        //总计金额
        public decimal salePriceSum { get; set; }

        //市场价的总和
        public decimal marketPriceSum { get; set; }

        //市场价和销售价的差额
        public decimal priceLower { get; set; }
    }
}