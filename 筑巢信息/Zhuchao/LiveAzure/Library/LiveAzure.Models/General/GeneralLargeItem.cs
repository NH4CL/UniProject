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
    /// 动态资源文件，用于存放大对象，例如多言的长字符文本
    /// </summary>
    /// <see cref="GeneralLargeObject"/>
    /// <seealso cref="GeneralResItem"/>
    public class GeneralLargeItem : LiveAzure.Models.ModelBase
    {
        #region 构造函数

        /// <summary>
        /// 如有必要，可重载，设置字段默认值
        /// </summary>
        public GeneralLargeItem()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="nCulture">语言</param>
        /// <param name="sCLOB">大字符串</param>
        /// <param name="oLargeObject">主资源文件</param>
        public GeneralLargeItem(short nCulture, string sCLOB, GeneralLargeObject oLargeObject)
        {
            this.Culture = nCulture;
            this.CLOB = sCLOB;
            this.LargeObject = oLargeObject;
        }
        
        #endregion

        /// <summary>
        /// 主键表ID，资源ID
        /// </summary>
        public Guid BlobID { get; set; }

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
        /// 二进制大对象
        /// </summary>
        public byte[] BLOB { get; set; }

        /// <summary>
        /// 大文本对象
        /// </summary>
        public string CLOB { get; set; }

        /// <summary>
        /// 文件类型或扩展名
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralLargeItem),
            ErrorMessageResourceName = "FileTypeLong")]
        public string FileType { get; set; }

        /// <summary>
        /// 文件存放位置；0:数据库；1:操作系统; 2:网站URL
        /// </summary>
        /// <see cref="ModelEnum.LargeObjectSource"/>
        public byte Source { get; set; }

        /// <summary>
        /// 文件存放操作系统位置或URL地址
        /// </summary>
        [StringLength(512, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralLargeItem),
            ErrorMessageResourceName = "ObjUrlLong")]
        public string ObjUrl { get; set; }

        /// <summary>
        /// 主键表
        /// </summary>
        [ForeignKey("BlobID")]
        public virtual GeneralLargeObject LargeObject { get; set; }
    }
}
