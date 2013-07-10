using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;

namespace LiveAzure.Models.Union
{
    /// <summary>
    /// 其他分成比例
    /// </summary>
    /// <see cref="MemberOrganization"/>
    /// <see cref="MemberChannel"/>
    /// <remarks>废弃该表，使用Public角色代替</remarks>
    [Obsolete]
    public class UnionFixedPercent : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，组织ID
        /// </summary>
        [Required]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 主键表ID，渠道ID
        /// </summary>
        [Required]
        public Guid ChlID { get; set; }

        /// <summary>
        /// 销售类型，同prod.saletype
        /// </summary>
        public byte SaleType { get; set; }

        /// <summary>
        /// 类型 0CPS分成 1代客下单分成
        /// </summary>
        public byte Ftype { get; set; }

        /// <summary>
        /// 状态 0无效 1有效
        /// Enum 待定
        /// </summary>
        public byte Fstatus { get; set; }

        /// <summary>
        /// 比例
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Percentage { get; set; }

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
        /// 渠道主键表
        /// </summary>
        [ForeignKey("ChlID")]
        public virtual MemberChannel Channel { get; set; }
    }
}
