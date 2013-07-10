using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;

namespace LiveAzure.Models.Order
{
    /// <summary>
    /// 拆单策略，例如分仓拆单，缺货拆单等
    /// </summary>
    /// <see cref="MemberOrganization"/>
    /// <see cref="MemberChannel"/>
    public class OrderSplitPolicy : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，组织ID
        /// </summary>
        [Required(ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Order.OrderSplitPolicy),
            ErrorMessageResourceName="OrgIDRequired")]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 主键表ID，渠道ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderSplitPolicy),
            ErrorMessageResourceName = "ChlID")]
        public Guid ChlID { get; set; }

        /// <summary>
        /// 自动挂起，优先即最高，即商品分组、缺货、运输等问题都需要挂起
        /// </summary>
        public bool SetHanged { get; set; }

        /// <summary>
        /// 商品分组是否自动拆分，例如大小件，参考prod.group
        /// </summary>
        public bool ProdGroup { get; set; }

        /// <summary>
        /// 仓库缺货，是否自动拆单
        /// </summary>
        public bool ProdShort { get; set; }

        /// <summary>
        /// 商品分属不同仓库，是否自动拆单，同仓库缺货
        /// </summary>
        public bool Warehouse { get; set; }

        /// <summary>
        /// 运输无法到达(只针对最高权重)，是否自动拆单
        /// </summary>
        public bool ShipLimit { get; set; }

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
    }
}
