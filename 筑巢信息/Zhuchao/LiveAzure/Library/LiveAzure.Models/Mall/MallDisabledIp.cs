using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;

namespace LiveAzure.Models.Mall
{
    /// <summary>
    /// 禁用ip地址
    /// </summary>
    public class MallDisabledIp : LiveAzure.Models.ModelBase
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
        /// 需要禁用的IP地址
        /// </summary>
        [Required]
        [StringLength(50)]
        public string IpAddress { get; set; }

        /// <summary>
        /// 是否禁用0禁用1解除禁用
        /// </summary>
        public byte Istatus { get; set; }

        /// <summary>
        /// 主键表，渠道
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }

        /// <summary>
        /// 主键表，渠道
        /// </summary>
        [ForeignKey("ChlID")]
        public virtual MemberChannel Channel { get; set; }

        /// <summary>
        /// 获取IP是否禁用列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> GenericStatusList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.GenericStatus), this.Istatus); }
        }

        /// <summary>
        /// 某个是否禁用类型的名称
        /// </summary>
        [NotMapped]
        public string GenericStatusName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.GenericStatus), this.Istatus); }
        }
    }
}
