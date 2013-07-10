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
    /// 用户自定义属性
    /// </summary>
    /// <see cref="MemberUser"/>
    public class MemberUserAttribute : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，用户ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUserAttribute),
            ErrorMessageResourceName = "UserIDRequired")]
        public Guid UserID { get; set; }

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
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUserAttribute),
            ErrorMessageResourceName = "MatterLong")]
        public string Matter { get; set; }

        /// <summary>
        /// 用户主键表
        /// </summary>
        [ForeignKey("UserID")]
        public virtual MemberUser User { get; set; }

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
