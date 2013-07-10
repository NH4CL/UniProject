using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;

namespace LiveAzure.Models.Union
{
    /// <summary>
    /// 向上层分成比例
    /// </summary>
    /// <see cref="MemberOrganization"/>
    /// <see cref="MemberChannel"/>
    /// <see cref="MemberRole"/>
    public class UnionLevelPercent : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，组织ID
        /// </summary>
        [Required]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public Guid RoleID { get; set; }

        /// <summary>
        /// 向上的层级，数字越大表示离本级越高，直到顶级
        /// </summary>
        public int BackLevel { get; set; }

        /// <summary>
        /// 状态 0无效 1有效
        /// Enum 待定
        /// </summary>
        public byte Ustatus { get; set; }

        /// <summary>
        /// 向上分层比例(有上线)
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Percentage { get; set; }

        /// <summary>
        /// 向上分层比例(无上级)
        /// </summary>
        [Column(TypeName = "money")]
        public decimal PercentTop { get; set; }

        /// <summary>
        /// 该级是否允许提现
        /// </summary>
        public bool Cashier { get; set; }

        /// <summary>
        /// 组织主键表
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }

        /// <summary>
        /// 角色主键表
        /// </summary>
        [ForeignKey("RoleID")]
        public virtual MemberRole Role { get; set; }
    }
}
