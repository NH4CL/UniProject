using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;

namespace LiveAzure.Models.General
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-07         -->
    /// <summary>
    /// 消息模板
    /// </summary>
    /// <see cref="GeneralMessagePending"/>
    /// <see cref="GeneralMessageReceive"/>
    public class GeneralMessageTemplate : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 组织
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralMessageTemplate),
            ErrorMessageResourceName = "OrgIDRequired")]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 代码，组织内唯一
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralMessageTemplate),
            ErrorMessageResourceName = "CodeRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralMessageTemplate),
            ErrorMessageResourceName = "CodeLong")]
        public string Code { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        [Column("Name")]
        public Guid? aName{get;set;}

        /// <summary>
        /// 模板内容
        /// </summary>
        [Column("Matter")]
        public Guid? aMatter { get; set; }

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

        /// <summary>
        /// 内容主键表
        /// </summary>
        [ForeignKey("aMatter")]
        public virtual GeneralLargeObject Matter { get; set; }
    }
}
