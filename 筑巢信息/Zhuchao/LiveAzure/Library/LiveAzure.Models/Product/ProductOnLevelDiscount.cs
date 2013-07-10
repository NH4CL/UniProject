using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;

namespace LiveAzure.Models.Product
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-09-06         -->
    /// <summary>
    /// 根据用户级别定义折扣体系
    /// </summary>
    /// <see cref="ProductOnSale"/>
    /// <see cref="MemberLevel"/>
    public class ProductOnLevelDiscount : LiveAzure.Models.ModelBase
    {
        public ProductOnLevelDiscount()
        {
            this.Discount = 1;
        }

        /// <summary>
        /// 主键表ID，上架商品ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductOnLevelDiscount),
            ErrorMessageResourceName = "OnSaleIDRequired")]
        public Guid OnSaleID { get; set; }

        /// <summary>
        /// 用户级别
        /// </summary>
        [Column("UserLevel")]
        public Guid aUserLevel { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// 上架商品主键表
        /// </summary>
        [ForeignKey("OnSaleID")]
        public virtual ProductOnSale OnSale { get; set; }
        
        /// <summary>
        /// 外键，用户级别
        /// </summary>
        [ForeignKey("aUserLevel")]
        public virtual MemberLevel UserLevel { get; set; }
    }
}
