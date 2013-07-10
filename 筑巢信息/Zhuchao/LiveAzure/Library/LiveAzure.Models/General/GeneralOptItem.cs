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
    /// 动态属性，自定义属性下拉框选项内容
    /// </summary>
    /// <see cref="GeneralOptional"/>
    public class GeneralOptItem : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，选项ID
        /// </summary>
        public Guid OptID { get; set; }

        /// <summary>
        /// 选项代码
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralOptItem),
            ErrorMessageResourceName = "CodeRequired")]
        [StringLength(20, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralOptItem),
            ErrorMessageResourceName = "CodeLong")]
        public string Code { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int Sorting { get; set; }
        
        /// <summary>
        /// 选项名称
        /// </summary>
        [Column("Name")]
        public Guid? aName { get; set; }

        /// <summary>
        /// 主键表
        /// </summary>
        [ForeignKey("OptID")]
        public virtual GeneralOptional Optional { get; set; }

        /// <summary>
        /// 主键表，选项名称
        /// </summary>
        [ForeignKey("aName")]
        public virtual GeneralResource Name { get; set; }
    }
}
