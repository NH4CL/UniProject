using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveAzure.Models.Exchange
{
    /// <summary>
    /// 淘宝用户信用等级
    /// </summary>
    public class ExTaobaoUserCredit : LiveAzure.Models.ModelBase
    {
        public long level { get; set; }
        public long score { get; set; }
        public long total_num { get; set; }
        public long good_num { get; set; }
    }
}
