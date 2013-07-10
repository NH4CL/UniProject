using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.General;
using LiveAzure.Models.Member;

namespace LiveAzure.Models.Mall
{
    /// <summary>
    /// 友情链接
    /// </summary>
    public class MallFriendLink : LiveAzure.Models.ModelBase
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
        /// 代码
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        /// <summary>
        /// 主键表ID，友情链接的名称
        /// </summary>
        [Column("Name")]
        public Guid? aName { get; set; }

        /// <summary>
        /// 友情链接的地址
        /// </summary>
        [StringLength(256)]
        public string LinkURL { get; set; }

        /// <summary>
        /// 友情链接的logo 是一个文件路径
        /// </summary>
        [StringLength(256)]
        public string LinkLogo { get; set; }

        /// <summary>
        /// 友情链接是否显示 0不显示 1显示
        /// </summary>
        public bool Show { get; set; }

        /// <summary>
        /// 友情链接排序
        /// </summary>
        public int Sorting { get; set; }

        /// <summary>
        /// 主键表，组织
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }

        /// <summary>
        /// 主键表，渠道
        /// </summary>
        [ForeignKey("ChlID")]
        public virtual MemberChannel Channel { get; set; }

        /// <summary>
        /// 主键表，名称
        /// </summary>
        [ForeignKey("aName")]
        public virtual GeneralResource Name { get; set; }
    }
}
