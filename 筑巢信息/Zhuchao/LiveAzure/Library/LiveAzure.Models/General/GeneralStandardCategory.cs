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
    /// 标准分类，全局
    /// </summary>
    /// <seealso cref="GeneralPrivateCategory"/>
    public class GeneralStandardCategory : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 类别 0组织分类 1渠道类型 3商品全局标准分类
        /// </summary>
        /// <see cref="ModelEnum.StandardCategoryType"/>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralStandardCategory),
            ErrorMessageResourceName = "CtypeRequired")]
        public byte Ctype { get; set; }
        
        /// <summary>
        /// 代码，类别内唯一
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralStandardCategory),
            ErrorMessageResourceName = "CodeRequired")]
        [StringLength(20, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralStandardCategory),
            ErrorMessageResourceName = "CodeLong")]
        public string Code { get; set; }
        
        /// <summary>
        /// 上级分类
        /// </summary>
        [Column("Parent")]
        public Guid? aParent { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        [Column("Name")]
        public Guid? aName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sorting { get; set; }

        /// <summary>
        /// 父项内容
        /// </summary>
        [ForeignKey("aParent")]
        public virtual GeneralStandardCategory Parent { get; set; }

        /// <summary>
        /// 子项内容
        /// </summary>
        [InverseProperty("Parent")]
        public virtual ICollection<GeneralStandardCategory> ChildItems { get; set; }

        ///// <summary>
        ///// 子项内容
        ///// </summary>
        //[InverseProperty("ExtendType")]
        //public virtual ICollection<GeneralStandardCategory> ExtendTypes { get; set; }

        /// <summary>
        /// 名称主键表
        /// </summary>
        [ForeignKey("aName")]
        public virtual GeneralResource Name { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> CategoryTypeList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.StandardCategoryType), this.Ctype); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string CategoryTypeName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.StandardCategoryType), this.Ctype); }
        }
    }
}
