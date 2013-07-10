using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LiveAzure.Models.Product
{
    /// <summary>
    /// 商品图片，按SKU
    /// </summary>
    /// <see cref="ProductInformation"/>
    /// <see cref="ProductInfoItem"/>
    public class ProductGallery : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，PU
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductGallery),
            ErrorMessageResourceName = "ProdIDRequired")]
        public Guid ProdID { get; set; }

        /// <summary>
        /// 主键表ID，SKU
        /// </summary>
        public Guid? SkuID { get; set; }

        /// <summary>
        /// 图片类型，0PU图 1SKU图
        /// </summary>
        /// <see cref=" ModelEnum.PictureType"/>
        public byte Gtype { get; set; }

        /// <summary>
        /// 图片简要说明，默认语言，可空
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductGallery),
            ErrorMessageResourceName = "CodeLong")]
        public string Code { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sorting { get; set; }

        /// <summary>
        /// 放大图片完整路径
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductGallery),
            ErrorMessageResourceName = "EnlargeLong")]
        public string Enlarge { get; set; }

        /// <summary>
        /// 正常图片完整路径
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductGallery),
            ErrorMessageResourceName = "ThumburlLong")]
        public string Thumburl { get; set; }

        /// <summary>
        /// 缩略图片完整路径
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductGallery),
            ErrorMessageResourceName = "ThumbnailLong")]
        public string Thumbnail { get; set; }

        /// <summary>
        /// 产品(PU)主键表
        /// </summary>
        [ForeignKey("ProdID")]
        public virtual ProductInformation Product { get; set; }

        /// <summary>
        /// 产品(SKU)主键表
        /// </summary>
        [ForeignKey("SkuID")]
        public virtual ProductInfoItem SkuItem { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> PictureTypeList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.PictureType), this.Gtype); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string PictureTypeName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.PictureType), this.Gtype); }
        }
    }
}
