using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.General;

namespace LiveAzure.Models.Member
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-07         -->
    /// <summary>
    /// 角色
    /// </summary>
    /// <see cref="MemberOrganization"/>
    /// <see cref="MemberUser"/>
    public class MemberRole : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，组织ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberRole),
            ErrorMessageResourceName = "OrgIDRequired")]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 代码，组织内唯一
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberRole),
            ErrorMessageResourceName = "CodeRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberRole),
            ErrorMessageResourceName = "CodeLong")]
        public string Code { get; set; }

        /// <summary>
        /// 角色类型
        /// </summary>
        public Guid? Rtype { get; set; }

        /// <summary>
        /// 上级角色
        /// </summary>
        [Column("Parent")]
        public Guid? aParent { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Column("Name")]
        public Guid? aName { get; set; }

        /// <summary>
        /// 组织主键表
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }

        /// <summary>
        /// 分类主键表
        /// </summary>
        [ForeignKey("Rtype")]
        public virtual GeneralStandardCategory RoleType { get; set; }

        /// <summary>
        /// 父项内容
        /// </summary>
        [ForeignKey("aParent")]
        public virtual MemberRole Parent { get; set; }

        /// <summary>
        /// 子项内容
        /// </summary>
        [InverseProperty("Parent")]
        public virtual ICollection<MemberRole> ChildItems { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [ForeignKey("aName")]
        public virtual GeneralResource Name { get; set; }

        /// <summary>
        /// 从表内容，用户
        /// </summary>
        [InverseProperty("Role")]
        public virtual ICollection<MemberUser> Users { get; set; }
    }
}
