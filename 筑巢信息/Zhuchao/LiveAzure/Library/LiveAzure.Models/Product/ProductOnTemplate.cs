using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;
using LiveAzure.Models.General;

namespace LiveAzure.Models.Product
{
    /// <summary>
    /// 商品上架模板
    /// </summary>
    /// <see cref="MemberOrganization"/>
    /// <seealso cref="ProductOnSale"/>
    /// <seealso cref="ProductOnShipping"/>
    /// <seealso cref="ProductOnPayment"/>
    public class ProductOnTemplate : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，组织ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductOnTemplate),
            ErrorMessageResourceName = "OrgIDRequired")]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 代码，组织内唯一
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductOnTemplate),
            ErrorMessageResourceName = "CodeRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductOnTemplate),
            ErrorMessageResourceName = "CodeLong")]
        public string Code { get; set; }

        /// <summary>
        /// 主键表ID，模板名称
        /// </summary>
        [Column("Name")]
        public Guid? aName { get; set; }

        /// <summary>
        /// 运输策略，特定格式保存
        /// </summary>
        public string ShipPolicy { get; set; }

        /// <summary>
        /// 支付策略
        /// </summary>
        public string PayPolicy { get; set; }

        /// <summary>
        /// 关联商品
        /// </summary>
        public string Relation { get; set; }

        /// <summary>
        /// 会员等级折扣
        /// </summary>
        public string LevelDiscount { get; set; }

        /// <summary>
        /// 组织主键表
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }

        /// <summary>
        /// 名称主键表
        /// </summary>
        [ForeignKey("aName")]
        public virtual GeneralResource Name { get; set; }
    }
}
