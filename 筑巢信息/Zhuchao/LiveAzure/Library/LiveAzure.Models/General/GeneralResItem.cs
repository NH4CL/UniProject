using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace LiveAzure.Models.General
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-01         -->
    /// <summary>
    /// 动态资源文件，用于存放类型产品名称多语言信息，和产品多货币价格等
    /// </summary>
    /// <see cref="GeneralResource"/>
    /// <seealso cref="GeneralLargeItem"/>
    public class GeneralResItem : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，资源ID
        /// </summary>
        public Guid ResID { get; set; }

        /// <summary>
        /// 助记码，可空
        /// </summary>
        [StringLength(20, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralResItem),
            ErrorMessageResourceName = "CodeLong")]
        public string Code { get; set; }

        /// <summary>
        /// 语言
        /// </summary>
        public int Culture { get; set; }

        /// <summary>
        /// 语言的本地名称
        /// </summary>
        [NotMapped]
        public string CultureName
        {
            get
            {
                string strCultureName = "NONE";
                try
                {
                    CultureInfo oCulture = new CultureInfo(this.Culture);
                    strCultureName = oCulture.NativeName;
                }
                catch { }
                return strCultureName;
            }
        }

        /// <summary>
        /// 指定语言的文本内容
        /// </summary>
        [StringLength(512, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralResItem),
            ErrorMessageResourceName = "MatterLong")]
        public string Matter { get; set; }

        /// <summary>
        /// 货币类型
        /// </summary>
        /// <see cref="GeneralMeasureUnit"/>
        public Guid? Currency { get; set; }

        /// <summary>
        /// 组织默认货币的金额数值
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Cash { get; set; }

        /// <summary>
        /// 主键表
        /// </summary>
        [ForeignKey("ResID")]
        public virtual GeneralResource Resource { get; set; }
    }
}
