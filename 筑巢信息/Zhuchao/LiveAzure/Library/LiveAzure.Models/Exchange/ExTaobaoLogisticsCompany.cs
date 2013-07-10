using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveAzure.Models.Exchange
{
    public class ExTaobaoLogisticsCompany : LiveAzure.Models.ModelBase
    {
        public long id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string reg_mail_no { get; set; }
    }
}
