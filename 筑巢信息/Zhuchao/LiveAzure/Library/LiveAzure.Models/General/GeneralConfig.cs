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
    /// 系统可变参数，不可变参数存放在xml配置文件中
    /// </summary>
    public class GeneralConfig : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 构造函数，默认为字符型参数
        /// </summary>
        public GeneralConfig()
        {
            this.Ctype = (byte)ModelEnum.ConfigParamType.STRING;
        }

        /// <summary>
        /// 参数代码
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralConfig),
            ErrorMessageResourceName = "CodeRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralConfig),
            ErrorMessageResourceName = "CodeLong")]
        // [Display(ResourceType = typeof(LiveAzure.Resource.Model.General.GeneralConfig), Name = "Code")]
        public string Code { get; set; }

        /// <summary>
        /// 上级配置
        /// </summary>
        [Column("Parent")]
        public Guid? aParent { get; set; }

        /// <summary>
        /// 适用的语言，0表示适用所有语言
        /// </summary>
        // [Display(ResourceType = typeof(LiveAzure.Resource.Model.General.GeneralConfig), Name = "Culture")]
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
        /// 参数数据类型
        /// </summary>
        /// <see cref="ModelEnum.ConfigParamType"/>
        // [Display(ResourceType = typeof(LiveAzure.Resource.Model.General.GeneralConfig), Name = "Ctype")]
        public byte Ctype { get; set; }

        /// <summary>
        /// 整数值
        /// </summary>
        // [Display(ResourceType = typeof(LiveAzure.Resource.Model.General.GeneralConfig), Name = "IntValue")]
        public int IntValue { get; set; }

        /// <summary>
        /// 小数值
        /// </summary>
        [Column(TypeName = "money")]
        public decimal DecValue { get; set; }

        /// <summary>
        /// 字符串值
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralConfig),
            ErrorMessageResourceName = "StrValueLong")]
        // [Display(ResourceType = typeof(LiveAzure.Resource.Model.General.GeneralConfig), Name = "StrValue")]
        public string StrValue { get; set; }

        /// <summary>
        /// 日期时间值
        /// </summary>
        // [Display(ResourceType = typeof(LiveAzure.Resource.Model.General.GeneralConfig), Name = "DateValue")]
        public DateTimeOffset? DateValue { get; set; }

        /// <summary>
        /// 父项内容
        /// </summary>
        [ForeignKey("aParent")]
        public virtual GeneralConfig Parent { get; set; }

        /// <summary>
        /// 子项内容
        /// </summary>
        [InverseProperty("Parent")]
        public virtual ICollection<GeneralConfig> ChildItems { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> ConfigTypeList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.ConfigParamType), this.Ctype); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string ConfigTypeName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.ConfigParamType), this.Ctype); }
        }
    }
}
