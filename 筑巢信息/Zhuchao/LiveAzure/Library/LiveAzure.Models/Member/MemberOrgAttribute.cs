using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.General;

namespace LiveAzure.Models.Member
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-08         -->
    /// <summary>
    /// 组织自定义属性
    /// </summary>
    /// <see cref="MemberOrganization"/>
    public class MemberOrgAttribute : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，组织ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberOrgAttribute),
            ErrorMessageResourceName = "OrgIDRequired")]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 属性
        /// </summary>
        public Guid OptID { get; set; }

        /// <summary>
        /// 属性内容，如果是下拉框，则存入optItem.gid，可空
        /// </summary>
        public Guid? OptResult { get; set; }

        /// <summary>
        /// 属性内容，如果是编辑框，则存入值，可空
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberOrgAttribute),
            ErrorMessageResourceName = "MatterLong")]
        public string Matter { get; set; }

        /// <summary>
        /// 组织主键表
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }

        /// <summary>
        /// 属性主键表
        /// </summary>
        [ForeignKey("OptID")]
        public virtual GeneralOptional Optional { get; set; }

        /// <summary>
        /// 属性内容主键表
        /// </summary>
        [ForeignKey("OptResult")]
        public virtual GeneralOptItem OptionalResult { get; set; }
    }
}
