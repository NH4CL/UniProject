using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.General;

namespace LiveAzure.Models.Member
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-06         -->
    /// <summary>
    /// 组织定义主表，包括运营组织、供应商、仓库、运输商等
    /// </summary>
    /// <see cref="OrganizationBase"/>
    /// <see cref="MemberUser"/>
    /// <see cref="MemberRole"/>
    /// <see cref="MemberChannel"/>
    /// <see cref="MemberOrgChannel"/>
    public class MemberOrganization : LiveAzure.Models.Member.OrganizationBase
    {
        /// <summary>
        /// 构造函数，设置类型
        /// </summary>
        public MemberOrganization()
        {
            this.Otype = (byte)ModelEnum.OrganizationType.CORPORATION;
        }

        /// <summary>
        /// 从表内容，角色
        /// </summary>
        [InverseProperty("Organization")]
        public virtual ICollection<MemberRole> Roles { get; set; }

        /// <summary>
        /// 从表内容，用户
        /// </summary>
        [InverseProperty("Organization")]
        public virtual ICollection<MemberUser> Users { get; set; }

        /// <summary>
        /// 从表内容，组织支持的语言文化
        /// </summary>
        [InverseProperty("Organization")]
        public virtual ICollection<MemberOrgCulture> Cultures { get; set; }

        /// <summary>
        /// 从表内容，组织自定义属性
        /// </summary>
        [InverseProperty("Organization")]
        public virtual ICollection<MemberOrgAttribute> Attributes { get; set; }
    }
}
