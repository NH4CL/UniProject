using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.General;
using LiveAzure.Models.Member;

namespace LiveAzure.Models.Finance
{
    /// <summary>
    /// 付款方式定义
    /// </summary>
    public class FinancePayType : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，所属组织
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Finance.FinancePayType),
            ErrorMessageResourceName = "OrgIDRequired")]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 代码，组织内唯一
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Finance.FinancePayType),
            ErrorMessageResourceName = "CodeRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Finance.FinancePayType),
            ErrorMessageResourceName = "CodeLong")]
        public string Code { get; set; }

        /// <summary>
        /// 主键表ID，名称
        /// </summary>
        [Column("Name")]
        public Guid? aName { get; set; }

        /// <summary>
        /// 简单描述，默认语言
        /// </summary>
        [StringLength(512, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Finance.FinancePayType),
            ErrorMessageResourceName = "MatterLong")]
        public string Matter { get; set; }

        /// <summary>
        /// 状态 0无效 1有效
        /// </summary>
        public byte Pstatus { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sorting { get; set; }

        /// <summary>
        /// 是否货到付款
        /// </summary>
        public bool IsCod { get; set; }

        /// <summary>
        /// 是否在线支付
        /// </summary>
        public bool IsOnline { get; set; }

        /// <summary>
        /// 是否担保交易
        /// </summary>
        public bool IsSecured { get; set; }

        /// <summary>
        /// 手续费率
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Fee { get; set; }

        /// <summary>
        /// 配置密钥等
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Finance.FinancePayType),
            ErrorMessageResourceName = "ConfigLong")]
        public string Config { get; set; }

        /// <summary>
        /// 组织主键表
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }

        /// <summary>
        /// 名称主键表
        /// </summary>
        [ForeignKey("aName")]
        public virtual GeneralResource Name { get; set; }
    }
}
