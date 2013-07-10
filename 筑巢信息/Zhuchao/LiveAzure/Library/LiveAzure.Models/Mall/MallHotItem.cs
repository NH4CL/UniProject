using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;

namespace LiveAzure.Models.Mall
{
    /// <summary>
    /// 热门关键字明细
    /// 每次搜索都记录在MallHotItem表中，并汇总到MallHotWord表中
    /// </summary>
    /// <see cref="MallHotWord"/>
    public class MallHotItem : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，组织ID
        /// </summary>
        public Guid? OrgID { get; set; }

        /// <summary>
        /// 主键表ID，渠道ID
        /// </summary>
        [Required]
        public Guid ChlID { get; set; }

        /// <summary>
        /// 关键字内容，即客户搜索内容等
        /// </summary>
        [Required]
        [StringLength(128)]
        public string Keyword { get; set; }

        /// <summary>
        /// 搜索来源URL
        /// </summary>
        [StringLength(256)]
        public string Source { get; set; }

        /// <summary>
        /// 主键表，组织
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }

        /// <summary>
        /// 主键表，渠道
        /// </summary>
        [ForeignKey("ChlID")]
        public virtual MemberChannel Channel { get; set; }
    }
}
