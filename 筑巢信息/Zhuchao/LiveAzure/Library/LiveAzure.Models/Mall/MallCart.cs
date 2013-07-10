using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;
using LiveAzure.Models.Product;
using LiveAzure.Models.General;

namespace LiveAzure.Models.Mall
{
    /// <summary>
    /// 商城购物车
    /// 当多个商品属于多个组织时，应分别形成多个订单，应分别付款，且付到对应组织的指定账户
    /// </summary>
    public class MallCart:LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，组织ID
        /// </summary
        [Column("OrgID")]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 主键表ID，渠道ID
        /// </summary
        [Required]
        [Column("ChlID")]
        public Guid ChlID { get; set; }

        /// <summary>
        /// 主键ID,用户登陆ID，取自session
        /// </summary>
        [Required]
        [Column("UserID")]
        public Guid UserID { get; set; }

        /// <summary>
        /// 主键表ID，上架 ID
        /// </summary>
        [Required]
        [Column("OnSaleID")]
        public Guid OnSaleID { get; set; }

        /// <summary>
        /// 主键表ID，上架SKU ID
        /// </summary>
        [Required]
        [Column("OnSkuID")]
        public Guid OnSkuID { get; set; }

        /// <summary>
        /// 主键表ID，购物车商品类型，common：普通；gift：礼品/赠品；group：套装；integrate：积分换购;second：秒杀
        /// </summary>
        public Guid? Ctype { get; set; }

        /// <summary>
        /// 登陆的sessionid
        /// </summary>
        [StringLength(50)]
        public string WebSession { get; set; }

        /// <summary>
        /// 商品数量，标准计量单位，此数量转换到订单项中
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// 套装数量，仅用于PU-Parts模式，套数变化，则Quantity按比例变化
        /// </summary>
        [Column(TypeName = "money")]
        public decimal SetQty { get; set; }

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
        /// 上架商品SKU主键表
        /// </summary>
        [ForeignKey("OnSaleID")]
        public virtual ProductOnSale OnSale { get; set; }

        /// <summary>
        /// 上架商品SKU主键表
        /// </summary>
        [ForeignKey("OnSkuID")]
        public virtual ProductOnItem OnSkuItem { get; set; }

        /// <summary>
        /// 购物车类型主键表
        /// </summary>
        [ForeignKey("Ctype")]
        public virtual GeneralStandardCategory CartType { get; set; }
    }
}
