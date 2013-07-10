using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;

namespace LiveAzure.Models.Mall
{
    /// <summary>
    /// 文章发布渠道
    /// </summary>
    /// <see cref="MemberOrganization"/>
    /// <see cref="MemberChannel"/>
    /// <see cref="MallArticle"/>
    /// <see cref="MallArtPosition"/>
    public class MallArtPublish : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MallArtPublish()
        {
            this.StartTime = DateTimeOffset.Now;
            this.EndTime = DateTimeOffset.Now;
        }
        /// <summary>
        /// 主键表ID，文章ID
        /// </summary>
        [Required]
        public Guid ArtID { get; set; }

        /// <summary>
        /// 主键表ID,渠道ID
        /// </summary>
        [Required]
        public Guid ChlID { get; set; }

        /// <summary>
        /// 主键表ID,文章位置ID
        /// </summary>
        [Required]
        public Guid PosID { get; set; }

        /// <summary>
        /// 文章的排序
        /// </summary>
        public int Sorting { get; set; }

        /// <summary>
        /// 是否显示 0不显示 1显示
        /// </summary>
        public bool Show { get; set; }

        /// <summary>
        /// 文章的实际平均评分,通过MatterRank、LayoutRank、ComfortRank所得
        /// </summary>
        public int TotalRank { get; set; }

        /// <summary>
        /// 内容评分
        /// </summary>
        public int MatterRank { get; set; }

        /// <summary>
        /// 布局评分
        /// </summary>
        public int LayoutRank { get; set; }

        /// <summary>
        /// 安慰评分
        /// </summary>
        public int ComfortRank { get; set; }

        /// <summary>
        /// 文章的有效开始时间
        /// </summary>
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset? StartTime { get; set; }

        /// <summary>
        /// 文章的有效结束时间
        /// </summary>
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset? EndTime { get; set; }

        /// <summary>
        /// 主键表，文章内容
        /// </summary>
        [ForeignKey("ArtID")]
        public virtual MallArticle Article { get; set; }

        /// <summary>
        /// 主键表，渠道
        /// </summary>
        [ForeignKey("ChlID")]
        public virtual MemberChannel Channel { get; set; }

        /// <summary>
        /// 主键表，位置
        /// </summary>
        [ForeignKey("PosID")]
        public virtual MallArtPosition Position { get; set; }
    }
}
