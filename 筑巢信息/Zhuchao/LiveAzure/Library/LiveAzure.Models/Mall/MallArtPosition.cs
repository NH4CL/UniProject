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
    /// 商城文章/广告位置
    /// </summary>
    /// <see cref="MeberOrganization"/>
    /// <see cref="MallArticle"/>
    /// <see cref="MallArtPublish"/>
    public class MallArtPosition : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，所属组织ID
        /// </summary>
        [Required]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 位置代码
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        /// <summary>
        /// 主键表ID，名称
        /// </summary>
        [Column("Name")]
        public Guid? aName { get; set; }

        /// <summary>
        /// 简单描述，默认语言
        /// </summary>
        [StringLength(256)]
        public string Matter { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool Show { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 主键表ID，代码模板
        /// </summary>
        [Column("Template")]
        public Guid? aTemplate { get; set; }

        /// <summary>
        /// 主键表，组织
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }

        /// <summary>
        /// 主键表，位置名称
        /// </summary>
        [ForeignKey("aName")]
        public virtual GeneralResource Name { get; set; }

        /// <summary>
        /// 主键表，模板
        /// </summary>
        [ForeignKey("aTemplate")]
        public virtual GeneralLargeObject Template { get; set; }

        /// <summary>
        /// 从表内容，文章的发布渠道
        /// </summary>
        [InverseProperty("Position")]
        public virtual ICollection<MallArtPublish> ArtPublish { get; set; }
    }
}
