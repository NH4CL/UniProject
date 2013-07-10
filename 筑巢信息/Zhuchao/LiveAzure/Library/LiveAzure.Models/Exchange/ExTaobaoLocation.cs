using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LiveAzure.Models.Exchange
{
    public class ExTaobaoLocation : LiveAzure.Models.ModelBase
    {
        public Guid TopUser { get; set; }
        public string zip { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string district { get; set; }

        [ForeignKey("TopUser")]
        public ExTaobaoUser TaobaoUser { get; set; }
    }
}
