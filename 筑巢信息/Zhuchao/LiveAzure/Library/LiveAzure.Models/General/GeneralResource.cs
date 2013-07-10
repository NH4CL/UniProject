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
    /// <see cref="GeneralResItem"/>
    /// <seealso cref="GeneralLargeObject"/>
    public class GeneralResource : LiveAzure.Models.ModelBase
    {
        #region 构造函数
        
        /// <summary>
        /// 构造函数，如有必要，可重载，设置字段默认值
        /// </summary>
        public GeneralResource()
        {
            this.ResourceItems = new List<GeneralResItem>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="rtype">资源类型</param>
        /// <param name="resource">完整资源表</param>
        public GeneralResource(ModelEnum.ResourceType rtype, GeneralResource resource)
        {
            this.Rtype = (byte)rtype;
            this.ResourceItems = new List<GeneralResItem>();
            if (rtype == ModelEnum.ResourceType.MONEY)
            {
                this.Code = resource.Code;
                this.Currency = resource.Currency;
                this.Cash = resource.Cash;
                this.Remark = resource.Remark;
                foreach (GeneralResItem item in resource.ResourceItems)
                {
                    var resitem = this.ResourceItems.FirstOrDefault(i => i.Currency == item.Currency && i.Deleted == false);
                    if (resitem == null)
                        this.ResourceItems.Add(new GeneralResItem { Code = item.Code, Currency = item.Currency, Cash = item.Cash, Remark = item.Remark });
                    else
                    {
                        resitem.Code = item.Code;
                        resitem.Cash = item.Cash;
                    }
                }
            }
            else
            {
                this.Code = resource.Code;
                this.Culture = resource.Culture;
                this.Matter = resource.Matter;
                this.Remark = resource.Remark;
                foreach (GeneralResItem item in resource.ResourceItems)
                {
                    var resitem = this.ResourceItems.FirstOrDefault(i => i.Culture == item.Culture && i.Deleted == false);
                    if (resitem == null)
                        this.ResourceItems.Add(new GeneralResItem { Code = item.Code, Culture = item.Culture, Matter = item.Matter, Remark = item.Remark });
                    else
                    {
                        resitem.Code = item.Code;
                        resitem.Matter = item.Matter;
                    }
                }
            }
        }
        
        /// <summary>
        /// 资源构造函数
        /// </summary>
        /// <param name="rtype">资源类型</param>
        /// <param name="args">参数对，字符型资源必须为int culture, string matter成对出现
        ///                            金额型资源必须为guid gid, decimal cash成对出现</param>
        /// <example>
        ///     var o = CreateResource(STRING, 2052, "中文内容", 1033, "英文内容");
        /// </example>
        public GeneralResource(ModelEnum.ResourceType rtype, params object[] args)
        {
            this.Rtype = (byte)rtype;
            this.ResourceItems = new List<GeneralResItem>();
            bool bFirstSet = true;
            try
            {
                if (this.Rtype == (byte)ModelEnum.ResourceType.MONEY)
                {
                    for (int i = 0; i < args.Count(); i += 2)
                    {
                        if (bFirstSet)
                        {
                            this.Currency = (Guid?)args[i];
                            this.Cash = (decimal)args[i + 1];
                            bFirstSet = false;
                        }
                        else
                        {
                            this.ResourceItems.Add(new GeneralResItem
                            {
                                Currency = (Guid?)args[i],
                                Cash = (decimal)args[i + 1]
                            });
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < args.Count(); i += 2)
                    {
                        if (bFirstSet)
                        {
                            this.Culture = (int)args[i];
                            this.Matter = args[i + 1].ToString();
                            bFirstSet = false;
                        }
                        else
                        {
                            this.ResourceItems.Add(new GeneralResItem
                            {
                                Culture = (int)args[i],
                                Matter = args[i + 1].ToString()
                            });
                        }
                    }
                }
            }
            catch { }
        }

        #endregion

        #region 应用函数

        /// <summary>
        /// 设置资源，即替代更新
        /// </summary>
        /// <param name="rtype">资源类型</param>
        /// <param name="resource">完整资源表</param>
        public void SetResource(ModelEnum.ResourceType rtype, GeneralResource resource)
        {
            this.Rtype = (byte)rtype;
            if (rtype == ModelEnum.ResourceType.MONEY)
            {
                this.Code = resource.Code;
                this.Currency = resource.Currency;
                this.Cash = resource.Cash;
                foreach (GeneralResItem item in resource.ResourceItems)
                {
                    var resitem = this.ResourceItems.FirstOrDefault(i => i.Currency == item.Currency && i.Deleted == false);
                    if (resitem == null)
                        this.ResourceItems.Add(new GeneralResItem { Code = item.Code, Currency = item.Currency, Cash = item.Cash });
                    else
                    {
                        resitem.Code = item.Code;
                        resitem.Cash = item.Cash;
                    }
                }
            }
            else
            {
                this.Code = resource.Code;
                this.Culture = resource.Culture;
                this.Matter = resource.Matter;
                foreach (GeneralResItem item in resource.ResourceItems)
                {
                    var resitem = this.ResourceItems.FirstOrDefault(i => i.Culture == item.Culture && i.Deleted == false);
                    if (resitem == null)
                        this.ResourceItems.Add(new GeneralResItem { Code = item.Code, Culture = item.Culture, Matter = item.Matter });
                    else
                    {
                        resitem.Code = item.Code;
                        resitem.Matter = item.Matter;
                    }
                }
            }
        }

        /// <summary>
        /// 设置资源，即替代更新
        /// </summary>
        /// <param name="rtype">资源类型</param>
        /// <param name="args">参数对，字符型资源必须为int culture, string matter成对出现
        ///                            金额型资源必须为guid gid, decimal cash成对出现</param>
        /// <example>
        ///     var o = CreateResource(STRING, 2052, "中文内容", 1033, "英文内容");
        /// </example>
        public void SetResource(ModelEnum.ResourceType rtype, params object[] args)
        {
            this.SetResource(rtype, new GeneralResource(rtype, args));
        }
      
        /// <summary>
        /// 获取字符型资源内容
        /// </summary>
        /// <param name="nCulture">语言</param>
        /// <returns>资源内容</returns>
        public string GetResource(int nCulture)
        {
            string sDefault = this.Matter;  // 默认值
            if (this.Culture == nCulture)
                sDefault = this.Matter;
            else if (this.ResourceItems != null)
            {
                GeneralResItem oResItem = this.ResourceItems.Where(i => i.Culture == nCulture && i.Deleted == false).FirstOrDefault();
                if (oResItem != null)
                    sDefault = oResItem.Matter;
            }
            return sDefault;
        }

        /// <summary>
        /// 获取金额型资源文件
        /// </summary>
        /// <param name="sCurrency">货币</param>
        /// <returns>金额，-1表示没有对应的金额</returns>
        public decimal GetResource(Guid sCurrency)
        {
            decimal mDefault = -1m;
            if (this.Currency == sCurrency)
                mDefault = this.Cash;
            else if (ResourceItems != null)
            {
                GeneralResItem oResItem = this.ResourceItems.Where(i => i.Currency == sCurrency && i.Deleted == false).FirstOrDefault();
                if (oResItem != null)
                    mDefault = oResItem.Cash;
            }
            return mDefault;
        }

        /// <summary>
        /// 获取主资源货币单位
        /// </summary>
        /// <param name="entity">数据库连接</param>
        /// <returns>计量单位</returns>
        public GeneralMeasureUnit GetCurrencyUnit(LiveEntities entity)
        {
            var unit = entity.GeneralMeasureUnits.Where(u => u.Gid == this.Currency
                && u.Utype == (byte)ModelEnum.MeasureUnit.CURRENCY
                && u.Deleted == false).FirstOrDefault();
            return unit;
        }

        /// <summary>
        /// 获取指定资源货币单位
        /// </summary>
        /// <param name="entity">数据库连接</param>
        /// <param name="currency">货币</param>
        /// <returns>计量单位</returns>
        public GeneralMeasureUnit GetCurrencyUnit(LiveEntities entity, Guid currency)
        {
            var unit = entity.GeneralMeasureUnits.Where(u => u.Gid == currency
                && u.Utype == (byte)ModelEnum.MeasureUnit.CURRENCY
                && u.Deleted == false).FirstOrDefault();
            return unit;
        }

        #endregion

        /// <summary>
        /// 资源类别，0字符资源，1金额资源
        /// </summary>
        /// <see cref="ModelEnum.ResourceType"/>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralResource),
            ErrorMessageResourceName = "RtypeRequired")]
        public byte Rtype { get; set; }

        /// <summary>
        /// 助记码，可空
        /// </summary>
        [StringLength(20, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralResource),
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
        /// 主语言文本内容
        /// </summary>
        [StringLength(512, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralResource),
            ErrorMessageResourceName = "MatterLong")]
        public string Matter { get; set; }

        /// <summary>
        /// 货币类型，冗余字段，可空，主要是指组织的默认货币
        /// </summary>
        /// <see cref="GeneralMeasureUnit"/>
        public Guid? Currency { get; set; }

        /// <summary>
        /// 组织默认货币的金额数值
        /// </summary>
        [Column(TypeName = "money")] 
        public decimal Cash { get; set; }
        
        /// <summary>
        /// 从表内容，其他语言的资源文本
        /// </summary>
        [InverseProperty("Resource")]
        public virtual ICollection<GeneralResItem> ResourceItems { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> ResourceTypeList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.ResourceType), this.Rtype); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string ResourceTypeName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.ResourceType), this.Rtype); }
        }
    }
}
