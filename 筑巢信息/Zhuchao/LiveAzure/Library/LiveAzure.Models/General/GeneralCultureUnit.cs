using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace LiveAzure.Models.General
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-07         -->
    /// <summary>
    /// 按文化默认计量单位
    /// </summary>
    public class GeneralCultureUnit : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 语言文化
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralCultureUnit),
            ErrorMessageResourceName = "CultureRequired")]
        public int Culture { get; set; }

        /// <summary>
        /// LCID
        /// </summary>
        [NotMapped]
        public int LCID
        {
            get { return this.Culture; }
        }

        /// <summary>
        /// 语言文化类
        /// </summary>
        [NotMapped]
        public CultureInfo CultureInfo
        {
            get { return new CultureInfo(this.Culture); }
        }

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
        /// 计件默认值（个）
        /// </summary>
        [Column("Piece")]
        public Guid? aPiece { get; set; }
        
        /// <summary>
        /// 重量默认值
        /// </summary>
        [Column("Weight")]
        public Guid? aWeight { get; set; }
        
        /// <summary>
        /// 体积默认值
        /// </summary>
        [Column("Volume")]
        public Guid? aVolume { get; set; }
        
        /// <summary>
        /// 容积默认值
        /// </summary>
        [Column("Fluid")]
        public Guid? aFluid { get; set; }
        
        /// <summary>
        /// 面积默认值
        /// </summary>
        [Column("Area")]
        public Guid? aArea { get; set; }
        
        /// <summary>
        /// 长度默认值
        /// </summary>
        [Column("Linear")]
        public Guid? aLinear { get; set; }
        
        /// <summary>
        /// 货币默认值
        /// </summary>
        [Column("Currency")]
        public Guid? aCurrency { get; set; }
        
        /// <summary>
        /// 其他
        /// </summary>
        [Column("Other")]
        public Guid? aOther { get; set; }

        /// <summary>
        /// 计件单位主键表
        /// </summary>
        [ForeignKey("aPiece")]
        public virtual GeneralMeasureUnit Piece { get; set; }

        /// <summary>
        /// 重量单位主键表
        /// </summary>
        [ForeignKey("aWeight")]
        public virtual GeneralMeasureUnit Weight { get; set; }

        /// <summary>
        /// 体积单位主键表
        /// </summary>
        [ForeignKey("aVolume")]
        public virtual GeneralMeasureUnit Volume { get; set; }

        /// <summary>
        /// 容积单位主键表
        /// </summary>
        [ForeignKey("aFluid")]
        public virtual GeneralMeasureUnit Fluid { get; set; }

        /// <summary>
        /// 面积单位主键表
        /// </summary>
        [ForeignKey("aArea")]
        public virtual GeneralMeasureUnit Area { get; set; }

        /// <summary>
        /// 长度单位主键表
        /// </summary>
        [ForeignKey("aLinear")]
        public virtual GeneralMeasureUnit Linear { get; set; }

        /// <summary>
        /// 货币单位主键表
        /// </summary>
        [ForeignKey("aCurrency")]
        public virtual GeneralMeasureUnit Currency { get; set; }
    }
}
