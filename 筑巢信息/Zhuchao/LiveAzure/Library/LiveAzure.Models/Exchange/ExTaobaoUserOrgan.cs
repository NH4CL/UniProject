using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;

namespace LiveAzure.Models.Exchange
{
    public class ExTaobaoUserOrgan : LiveAzure.Models.ModelBase
    {
        public Guid OrgID { get; set; }
        public Guid ChlID { get; set; }
        public Guid TopUser { get; set; }

        [ForeignKey("OrgID")]
        public MemberOrganization Organization { get; set; }
        
        [ForeignKey("ChlID")]
        public MemberChannel Channel { get; set; }

        [ForeignKey("TopUser")]
        public ExTaobaoUser TaobaoUser { get; set; }
    }
}
