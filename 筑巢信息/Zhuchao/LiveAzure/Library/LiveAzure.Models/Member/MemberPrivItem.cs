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
    /// 权限控制，按组织，渠道，程序，程序功能等
    /// </summary>
    /// <see cref="MemberPrivilege"/>
    public class MemberPrivItem : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，授权ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberPrivItem),
            ErrorMessageResourceName = "PrivIDRequired")]
        public Guid PrivID { get; set; }

        /// <summary>
        /// 参考ID
        /// </summary>
        public Guid? RefID { get; set; }

        /// <summary>
        /// 节点功能代码，可空
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberPrivItem),
            ErrorMessageResourceName = "NodeCodeLong")]
        public string NodeCode { get; set; }

        /// <summary>
        /// 节点功能授权值，即编辑/下拉框的值
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberPrivItem),
            ErrorMessageResourceName = "NodeValueLong")]
        public string NodeValue { get; set; }

        /// <summary>
        /// 授权主键表
        /// </summary>
        [ForeignKey("PrivID")]
        public virtual MemberPrivilege Privilege { get; set; }
    }
}
