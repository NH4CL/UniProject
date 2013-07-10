using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LiveAzure.Models.General
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-19         -->
    /// <summary>
    /// 权限控制，按组织，渠道，程序，程序功能等
    /// </summary>
    /// <see cref="GeneralPrivTemplate"/>
    public class GeneralPrivItem : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，授权ID
        /// </summary>
        public Guid PrivID { get; set; }

        /// <summary>
        /// 参考ID
        /// </summary>
        public Guid? RefID { get; set; }

        /// <summary>
        /// 节点功能代码，可空
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralPrivItem),
            ErrorMessageResourceName = "NodeCodeLong")]
        public string NodeCode { get; set; }

        /// <summary>
        /// 节点功能授权值，即编辑/下拉框的值
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralPrivItem),
            ErrorMessageResourceName = "NodeValueLong")]
        public string NodeValue { get; set; }

        /// <summary>
        /// 模板主键表
        /// </summary>
        [ForeignKey("PrivID")]
        public virtual GeneralPrivTemplate Template { get; set; }
    }
}
