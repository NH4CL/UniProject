using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;


namespace LiveAzure.Models.Product
{
    /// <summary>
    /// 关联商品
    /// </summary>
    /// <see cref="ProductOnSale"/>
    public class ProductOnRelation : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，上架商品ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Product.ProductOnRelation),
            ErrorMessageResourceName = "OnSaleIDRequired")]
        public Guid OnSaleID { get; set; }

        /// <summary>
        /// 主键表ID，上架商品ID
        /// </summary>
        [Required]
        [Column("OnRelation")]
        public Guid aOnRelation{ get; set; }

        /// <summary>
        /// 关联类型 类型：0推荐 1买过该商品的人还买过 2
        /// </summary>
        /// <see cref="ModelEnum.RelationType"/>
        public byte Rtype { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sorting { get; set; }

        /// <summary>
        /// 上架商品主键表
        /// </summary>
        [ForeignKey("OnSaleID")]
        public virtual ProductOnSale OnSale { get; set; }

        /// <summary>
        /// 上架商品主键表
        /// </summary>
        [ForeignKey("aOnRelation")]
        public virtual ProductOnSale OnRelation { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> RelationTypeList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.RelationType), this.Rtype); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string RelationTypeName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.RelationType), this.Rtype); }
        }
    }
}
