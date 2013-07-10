using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LiveAzure.Models.Purchase
{
    /// <summary>
    /// 供应商
    /// </summary>
    public class PurchaseSupplier : LiveAzure.Models.Member.OrganizationBase
    {
        /// <summary>
        /// 构造函数，设置类型
        /// </summary>
        public PurchaseSupplier()
        {
            this.Otype = (byte)ModelEnum.OrganizationType.SUPPLIER;
        }

        /// <summary>
        /// 从表内容,采购单
        /// </summary>
        [InverseProperty("Supplier")]
        public virtual ICollection<PurchaseInformation> Purchase { get; set; }
    }
}
