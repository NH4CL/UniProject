using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;

namespace LiveAzure.Models.Mall
{
    /// <summary>
    /// 商业敏感词，或正则表达式
    /// </summary>
    public class MallSensitiveWord : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，组织ID
        /// </summary>
        public Guid? OrgID { get; set; }

        /// <summary>
        /// 主键表ID，渠道ID
        /// </summary>
        [Required]
        public Guid ChlID { get; set; }

        /// <summary>
        /// 敏感词
        /// </summary>
        [Required]
        [StringLength(128)]
        public string Keyword { get; set; }

        /// <summary>
        /// 是否禁用 0禁用 1解除禁用
        /// </summary>
        public byte Wstatus { get; set; }

        /// <summary>
        /// 组织主键表
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }

        /// <summary>
        /// 渠道主键表
        /// </summary>
        [ForeignKey("ChlID")]
        public virtual MemberChannel Channel { get; set; }

        /// <summary>
        /// 获取敏感词是否禁用列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> GenericStatusList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.GenericStatus), this.Wstatus); }
        }

        /// <summary>
        /// 某个是否禁用类型的名称
        /// </summary>
        [NotMapped]
        public string GenericStatusName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.GenericStatus), this.Wstatus); }
        }
    }
}
