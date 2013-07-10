using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;


namespace LiveAzure.Models.General
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-07         -->
    /// <summary>
    /// 计量单位
    /// </summary>
    public class GeneralMeasureUnit : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 类别 0计件 1重量 2体积 3容积 4面积 5长度 6货币
        /// </summary>
        /// <see cref="ModelEnum.MeasureUnit"/>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralMeasureUnit),
            ErrorMessageResourceName = "UtypeRequired")]
        public byte Utype { get; set; }

        /// <summary>
        /// 代码，类别内唯一
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralMeasureUnit),
            ErrorMessageResourceName = "CodeRequired")]
        [StringLength(20, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralMeasureUnit),
            ErrorMessageResourceName = "CodeLong")]
        public string Code { get; set; }
        
        /// <summary>
        /// 名称
        /// </summary>
        [Column("Name")]
        public Guid? aName { get; set; }

        /// <summary>
        /// 名称主键表
        /// </summary>
        [ForeignKey("aName")]
        public virtual GeneralResource Name { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> TypeList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.MeasureUnit), this.Utype); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string TypeName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.MeasureUnit), this.Utype); }
        }
    }
}
