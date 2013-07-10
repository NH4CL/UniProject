using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LiveAzure.Portal.Models
{
    public class MyCoupons
    {
        //
        // GET: /MyCoupon/

        public string cCode;//卡号
        public Int16 Ptype;
        public Int16 Pstatus;
        public int Score;
        public int Remain;
        public decimal Amount;
        public decimal Balance;
        public DateTimeOffset? StartTime;
        public DateTimeOffset? EndTime;
        public string Reason;
        public string oCode;//关联单据号
        public decimal MinCharge;
        public bool Cashier;
        public bool OnceUse;
    }
}
