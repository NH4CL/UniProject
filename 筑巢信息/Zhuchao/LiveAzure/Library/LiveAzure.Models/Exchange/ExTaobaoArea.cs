using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveAzure.Models.Exchange
{
    public class ExTaobaoArea : LiveAzure.Models.ModelBase
    {
        public Guid? RegionID { get; set; }
        public long id { get; set; }
        public long type { get; set; }
        public string name { get; set; }
        public long parent_id { get; set; }
        public string zip { get; set; }
    }
}
