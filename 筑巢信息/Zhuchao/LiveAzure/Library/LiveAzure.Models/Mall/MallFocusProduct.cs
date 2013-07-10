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
    /// 商城商品关注，缺货通知
    /// </summary>
    public class MallFocusProduct : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，组织ID
        /// </summary>
        public Guid? OrgID { get; set; }

        /// <summary>
        /// 主键表ID，渠道ID
        /// </summary>
        [Required]
        public Guid ChlID { get; set; }

        /// <summary>
        /// 主键ID,收藏商品的用户
        /// </summary>
        [Required]
        public Guid UserID { get; set; }

        /// <summary>
        /// 主键表ID，收藏商品的商品货号PU
        /// </summary>
        [Required]
        public Guid OnSaleID { get; set; }

        /// <summary>
        /// 类别 0降价关注 1缺货通知
        /// </summary>
        /// <see cref="ModelEnum.FocusProductType"/>
        public byte Ftype { get; set; }

        /// <summary>
        /// 货币
        /// </summary>
        [Column("Currency")]
        public Guid? aCurrency { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Price { get; set; }

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
        /// 商品主键表
        /// </summary>
        [ForeignKey("OnSaleID")]
        public virtual ProductOnSale OnSale { get; set; }

        /// <summary>
        /// 货币主键表
        /// </summary>
        [ForeignKey("aCurrency")]
        public virtual GeneralMeasureUnit Currency { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> FocusTypeList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.FocusProductType), this.Ftype); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string FocusTypeName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.FocusProductType), this.Ftype); }
        }
    }
}
