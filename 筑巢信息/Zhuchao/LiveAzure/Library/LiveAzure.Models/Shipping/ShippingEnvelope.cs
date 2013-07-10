using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using LiveAzure.Models.General;


namespace LiveAzure.Models.Shipping
{
    /// <summary>
    /// 运输公司面单，运输一个公司使用多个版本的面单
    /// </summary>
    /// <see cref="ShippingInformation"/>
    public class ShippingEnvelope : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，运输公司ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Shipping.ShippingEnvelope),
            ErrorMessageResourceName = "ShipIDRequired")]
        public Guid ShipID { get; set; }

        /// <summary>
        /// 面单代码
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Shipping.ShippingEnvelope),
            ErrorMessageResourceName = "CodeRequired")]
        [StringLength(20, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Shipping.ShippingEnvelope),
            ErrorMessageResourceName = "CodeLong")]
        public string Code { get; set; }

        ///// <summary>
        ///// 支持的语言
        ///// </summary>
        //public int Culture { get; set; }

        ///// <summary>
        ///// 语言的本地名称
        ///// </summary>
        //[NotMapped]
        //public string CultureName
        //{
        //    get
        //    {
        //        string strCultureName = "NONE";
        //        try
        //        {
        //            CultureInfo oCulture = new CultureInfo(this.Culture);
        //            strCultureName = oCulture.NativeName;
        //        }
        //        catch { }
        //        return strCultureName;
        //    }
        //}

        /// <summary>
        /// 状态 0无效 1默认值 2其他版本
        /// </summary>
        /// <see cref="ModelEnum.EnvelopeStatus"/>
        public byte Estatus { get; set; }

        /// <summary>
        /// 默认语言简单描述
        /// </summary>
        [StringLength(512, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Shipping.ShippingEnvelope),
            ErrorMessageResourceName = "MatterLong")]
        public string Matter { get; set; }

        /// <summary>
        /// 面单模板，包括文字和背景图片等
        /// </summary>
        [Column("Template")]
        public Guid? aTemplate { get; set; }

        /// <summary>
        /// 运输公司主键表
        /// </summary>
        [ForeignKey("ShipID")]
        public virtual ShippingInformation Shipping { get; set; }

        /// <summary>
        /// 主键表，面单模板，包括文字和背景图片等
        /// </summary>
        [ForeignKey("aTemplate")]
        public virtual GeneralLargeObject Template { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> EnvelopeStatusList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.EnvelopeStatus), this.Estatus); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string EnvelopeStatusName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.EnvelopeStatus), this.Estatus); }
        }
    }
}
