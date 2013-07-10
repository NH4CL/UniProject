using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LiveAzure.Models.Shipping
{
    /// <summary>
    /// 运输公司
    /// </summary>
    public class ShippingInformation : LiveAzure.Models.Member.OrganizationBase
    {
        /// <summary>
        /// 构造函数，设置类型
        /// </summary>
        public ShippingInformation()
        {
            this.Otype = (byte)ModelEnum.OrganizationType.SHIPPER;
        }

        /// <summary>
        /// 从表内容，运输公司支持的区域和标准费率定义
        /// </summary>
        [InverseProperty("Shipping")]
        public virtual ICollection<ShippingArea> Areas { get; set; }

        /// <summary>
        /// 从表内容，面单模板，包括文字和背景图片等
        /// </summary>
        [InverseProperty("Shipping")]
        public virtual ICollection<ShippingEnvelope> Envelopes { get; set; }
    }
}
