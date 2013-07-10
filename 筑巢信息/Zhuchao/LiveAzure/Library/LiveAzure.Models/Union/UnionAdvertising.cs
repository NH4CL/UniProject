using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LiveAzure.Models.Union
{
    /// <summary>
    /// 广告内容和代码
    /// </summary>
    public class UnionAdvertising:LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 组织ID
        /// </summary>
        [Required]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 渠道ID
        /// </summary>
        [Required]
        public Guid ChlID { get; set; }

        /// <summary>
        /// 状态 0无效 1有效
        /// Enum 待定
        /// </summary>
        public byte Fstatus { get; set; }
    }
}
