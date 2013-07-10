using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;

namespace LiveAzure.Models.Mall
{
    /// <summary>
    /// 商城热门关键词
    /// 每次搜索都记录在MallHotItem表中，并汇总到MallHotWord表中
    /// </summary>
    /// <see cref="MallHotItem"/>
    public class MallHotWord : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID,组织ID
        /// </summary>
        public Guid? OrgID { get; set; }

        /// <summary>
        /// 主键表ID,渠道ID
        /// </summary>
        [Required]
        public Guid ChlID { get; set; }

        /// <summary>
        /// 关键字内容，即客户搜索内容
        /// </summary>
        [Required]
        [StringLength(128)]
        public string Keyword { get; set; }

        /// <summary>
        /// 近似值
        /// </summary>
        [StringLength(128)]
        public string Approx { get; set; }

        /// <summary>
        /// 是否显示0；不显示1显示
        /// </summary>
        public bool Show { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int KeyWeight { get; set; }

        /// <summary>
        /// 总计搜索次数
        /// </summary>
        public int SearchCount { get; set; }

        /// <summary>
        /// 主键表，渠道
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
