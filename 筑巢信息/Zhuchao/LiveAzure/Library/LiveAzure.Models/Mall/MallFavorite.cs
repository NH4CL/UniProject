using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;
using LiveAzure.Models.Product;

namespace LiveAzure.Models.Mall
{
    /// <summary>
    /// 商城商品收藏
    /// </summary>
    public class MallFavorite : LiveAzure.Models.ModelBase
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
        /// 主键ID,收藏商品的用户
        /// </summary>
        [Required]
        public Guid UserID { get; set; }

        /// <summary>
        /// 主键表ID，收藏商品的商品货号PU
        /// </summary>
        [Required]
        public Guid OnSaleID { get; set; }

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
        /// 用户主键表
        /// </summary>
        [ForeignKey("UserID")]
        public virtual MemberUser User { get; set; }

        /// <summary>
        /// 商品主键表
        /// </summary>
        [ForeignKey("OnSaleID")]
        public virtual ProductOnSale OnSale { get; set; }
    }
}
