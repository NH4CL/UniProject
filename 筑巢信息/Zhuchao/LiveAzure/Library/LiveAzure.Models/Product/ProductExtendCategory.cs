using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.General;

namespace LiveAzure.Models.Product
{
    /// <summary>
    /// 扩展分类
    /// </summary>
    /// <see cref="ProductInformation"/>
    /// <see cref="GeneralPrivateCategory"/>
    public class ProductExtendCategory : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，产品ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductExtendCategory),
            ErrorMessageResourceName = "ProgIDRequired")]
        public Guid ProdID { get; set; }

        /// <summary>
        /// 主键表ID，私有分类
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductExtendCategory),
            ErrorMessageResourceName = "PrvCatIDRequired")]
        public Guid PrvCatID { get; set; }

        /// <summary>
        /// 默认值，页面显示的分类信息
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 产品主键表
        /// </summary>
        [ForeignKey("ProdID")]
        public virtual ProductInformation Product { get; set; }

        /// <summary>
        /// 私有分类主键表
        /// </summary>
        [ForeignKey("PrvCatID")]
        public virtual GeneralPrivateCategory PrivateCategory { get; set; }
    }
}
