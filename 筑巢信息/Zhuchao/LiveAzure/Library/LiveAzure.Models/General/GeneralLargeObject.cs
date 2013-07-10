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
    /// <see cref="GeneralLargeItem"/>
    /// <seealso cref="GeneralResource"/>
    public class GeneralLargeObject : LiveAzure.Models.ModelBase
    {
        #region 构造函数

        /// <summary>
        /// 构造函数，如有必要，可重载，设置字段默认值
        /// </summary>
        public GeneralLargeObject()
        {
            this.LargeItems = new List<GeneralLargeItem>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="large">完整资源表</param>
        public GeneralLargeObject(GeneralLargeObject large)
        {
            this.LargeItems = new List<GeneralLargeItem>();
            this.Code = large.Code;
            this.Culture = large.Culture;
            this.CLOB = large.CLOB;
            this.Remark = large.Remark;
            foreach (GeneralLargeItem item in large.LargeItems)
            {
                var resitem = this.LargeItems.FirstOrDefault(i => i.Culture == item.Culture && i.Deleted == false);
                if (resitem == null)
                    this.LargeItems.Add(new GeneralLargeItem { Culture = item.Culture, CLOB = item.CLOB, Remark = item.Remark });
                else
                    resitem.CLOB = item.CLOB;
            }
        }

        /// <summary>
        /// 资源构造函数
        /// </summary>
        /// <param name="args">参数对，必须为int culture, string matter成对出现</param>
        /// <example>
        ///     var o = CreateResource(2052, "中文内容", 1033, "英文内容");
        /// </example>
        public GeneralLargeObject(params object[] args)
        {
            this.LargeItems = new List<GeneralLargeItem>();
            bool bFirstSet = true;
            try
            {
                for (int i = 0; i < args.Count(); i += 2)
                {
                    if (bFirstSet)
                    {
                        this.Culture = (int)args[i];
                        this.CLOB = args[i + 1].ToString();
                        bFirstSet = false;
                    }
                    else
                    {
                        this.LargeItems.Add(new GeneralLargeItem
                        {
                            Culture = (int)args[i],
                            CLOB = args[i + 1].ToString()
                        });
                    }
                }
            }
            catch { }
        }

        #endregion

        #region 应用函数

        /// <summary>
        /// 设置资源，即替代更新，Gid不变
        /// </summary>
        /// <param name="large">完整资源表</param>
        public void SetLargeObject(GeneralLargeObject large)
        {
            this.Code = large.Code;
            this.Culture = large.Culture;
            this.CLOB = large.CLOB;
            this.Remark = large.Remark;
            foreach (GeneralLargeItem item in large.LargeItems)
            {
                var resitem = this.LargeItems.FirstOrDefault(i => i.Culture == item.Culture && i.Deleted == false);
                if (resitem == null)
                    this.LargeItems.Add(new GeneralLargeItem { Culture = item.Culture, CLOB = item.CLOB });
                else
                    resitem.CLOB = item.CLOB;
            }
        }

        /// <summary>
        /// 资源构造函数
        /// </summary>
        /// <param name="args">参数对，必须为int culture, string matter成对出现</param>
        /// <example>
        ///     var o = CreateResource(2052, "中文内容", 1033, "英文内容");
        /// </example>
        public void SetLargeObject(params object[] args)
        {
            SetLargeObject(new GeneralLargeObject(args));
        }

        /// <summary>
        /// 获取大字符型资源内容
        /// </summary>
        /// <param name="nCulture">语言</param>
        /// <returns>资源内容</returns>
        public string GetLargeObject(int nCulture)
        {
            string sDefault = this.CLOB;   // 默认值
            if (this.Culture == nCulture)
                sDefault = this.CLOB;
            else if (this.LargeItems != null)
            {
                GeneralLargeItem oLargeItem = this.LargeItems.Where(i => i.Culture == nCulture && i.Deleted == false).FirstOrDefault();
                if (oLargeItem != null)
                    sDefault = oLargeItem.CLOB;
            }
            return sDefault;
        }

        #endregion

        /// <summary>
        /// 助记码，可空
        /// </summary>
        [StringLength(20, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralLargeObject),
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
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralLargeObject),
            ErrorMessageResourceName = "FileTypeLong")]
        public string FileType { get; set; }

        /// <summary>
        /// 文件存放位置；0:未指定；1:数据库；2:操作系统; 3:网站URL
        /// </summary>
        /// <see cref="ModelEnum.LargeObjectSource"/>
        public byte Source { get; set; }

        /// <summary>
        /// 文件存放操作系统位置或URL地址
        /// </summary>
        [StringLength(512, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralLargeObject),
            ErrorMessageResourceName = "ObjUrlLong")]
        public string ObjUrl { get; set; }

        /// <summary>
        /// 从表内容，其他语言的大对象
        /// </summary>
        [InverseProperty("LargeObject")]
        public virtual ICollection<GeneralLargeItem> LargeItems { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> SourceList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.LargeObjectSource), this.Source); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string SourceName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.LargeObjectSource), this.Source); }
        }
    }
}
