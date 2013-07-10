using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.General;
using LiveAzure.Models.Member;
using LiveAzure.Models.Product;

namespace LiveAzure.Models.Mall
{
    /// <summary>
    /// 商城文章
    /// </summary>
    /// <see cref="MeberOrganization"/>
    /// <see cref="MallArtPosition"/>
    /// <see cref="MallArtPublish"/>
    public class MallArticle : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键ID,组织ID
        /// </summary>
        [Required]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 代码
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        /// <summary>
        /// 状态 0未审核 1审核通过 2不通过 3不需要审核
        /// </summary>
        /// <see cref="ModelEnum.ArticleStatus"/>
        public byte Astatus { get; set; }

        /// <summary>
        /// 主键ID,文章所属类型
        /// </summary>
        public Guid? Atype { get; set; }

        /// <summary>
        /// 主键ID,分类（目录结构）
        /// </summary>
        [Column("Acategory")]
        public Guid? Acategory { get; set; }

        /// <summary>
        /// 主键ID,文章的父分类ID，用户回贴等
        /// </summary>
        [Column("Parent")]
        public Guid? aParent { get; set; }

        /// <summary>
        /// 主键ID,作者的ID，若为游客则为空
        /// </summary>
        [Column("UserID")]
        public Guid? UserID { get; set; }

        /// <summary>
        /// 用户的名称，若为游客则为匿名
        /// </summary>
        [StringLength(256)]
        public string UserName { get; set; }

        /// <summary>
        /// 主键ID,咨询、评论的商品号
        /// </summary>
        public Guid? ProdID { get; set; }

        /// <summary>
        /// 主键ID,标题
        /// </summary>
        [Column("Title")]
        public Guid? aTitle { get; set; }

        /// <summary>
        /// 主键ID,文章的内容
        /// </summary>
        [Column("Matter")]
        public Guid? aMatter { get; set; }

        /// <summary>
        /// 原著者描述
        /// </summary>
        [StringLength(256)]
        public string Copyright { get; set; }

        /// <summary>
        /// 文章的关键字
        /// </summary>
        [StringLength(256)]
        public string Keywords { get; set; }

        /// <summary>
        /// 主键表，组织
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }

        /// <summary>
        /// 主键表，类型
        /// </summary>
        [ForeignKey("Atype")]
        public virtual GeneralPrivateCategory ArticleType { get; set; }

        /// <summary>
        /// 主键表，分类，目录
        /// </summary>
        [ForeignKey("Acategory")]
        public virtual GeneralPrivateCategory ArticleCategory { get; set; }

        /// <summary>
        /// 父项内容
        /// </summary>
        [ForeignKey("aParent")]
        public virtual MallArticle Parent { get; set; }

        /// <summary>
        /// 子项内容
        /// </summary>
        [InverseProperty("Parent")]
        public virtual ICollection<MallArticle> ChildItems { get; set; }

        /// <summary>
        /// 主键表，用户
        /// </summary>
        [ForeignKey("UserID")]
        public virtual MemberUser User { get; set; }

        /// <summary>
        /// 主键表，产品
        /// </summary>
        [ForeignKey("ProdID")]
        public virtual ProductInformation Product { get; set; }

        /// <summary>
        /// 主键表，文章标题
        /// </summary>
        [ForeignKey("aTitle")]
        public virtual GeneralResource Title { get; set; }

        /// <summary>
        /// 主键表，文章内容
        /// </summary>
        [ForeignKey("aMatter")]
        public virtual GeneralLargeObject Matter { get; set; }

        /// <summary>
        /// 从表内容，文章的发布渠道
        /// </summary>
        [InverseProperty("Article")]
        public virtual ICollection<MallArtPublish> ArtPublish { get; set; }

        /// <summary>
        /// 获取文章状态的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> ArticleStatusList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.ArticleStatus), this.Astatus); }
        }

        /// <summary>
        /// 某个文章状态的名称
        /// </summary>
        [NotMapped]
        public string ArticleStatusName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.ArticleStatus), this.Astatus); }
        }
    }
}
