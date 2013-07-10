using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.General;

namespace LiveAzure.Models.Product
{
    /// <summary>
    /// 扩展属性
    /// </summary>
    /// <see cref="ProductInformation"/>
    public class ProductExtendAttribute : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，产品ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductExtendAttribute),
            ErrorMessageResourceName = "ProdIDRequired")]
        public Guid ProdID { get; set; }

        /// <summary>
        /// 主键表ID，属性ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductExtendAttribute),
            ErrorMessageResourceName = "OptIDRequired")]
        public Guid OptID { get; set; }

        /// <summary>
        /// 主键表ID，属性内容,如果是下拉框，则存入optItem.gid，可空
        /// </summary>
        public Guid? OptResult { get; set; }

        /// <summary>
        /// 属性内容，如果是编辑框，则存入值，可空
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductExtendAttribute),
            ErrorMessageResourceName = "MatterLong")]
        public string Matter { get; set; }

        /// <summary>
        /// 产品主键表
        /// </summary>
        [ForeignKey("ProdID")]
        public virtual ProductInformation Product { get; set; }

        /// <summary>
        /// 属性主键表
        /// </summary>
        [ForeignKey("OptID")]
        public virtual GeneralOptional Optional { get; set; }

        /// <summary>
        /// 属性内容主键表
        /// </summary>
        [ForeignKey("OptResult")]
        public virtual GeneralOptItem OptionalResult { get; set; }
    }
}
