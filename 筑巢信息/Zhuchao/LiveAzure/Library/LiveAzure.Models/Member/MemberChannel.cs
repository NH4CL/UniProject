using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LiveAzure.Models.Member
{
    /// <summary>
    /// 渠道，继承自组织
    /// </summary>
    /// <see cref="OrganizationBase"/>
    /// <see cref="MemberOrganization"/>
    /// <see cref="MemberOrgChannel"/>
    public class MemberChannel : LiveAzure.Models.Member.OrganizationBase
    {
        /// <summary>
        /// 构造函数，设置类型
        /// </summary>
        public MemberChannel()
        {
            this.Otype = (byte)ModelEnum.OrganizationType.CHANNEL;
        }

        /// <summary>
        /// 从表内容，支持的渠道
        /// </summary>
        [InverseProperty("Channel")]
        public virtual ICollection<MemberOrgChannel> Organizations { get; set; }
    }
}
