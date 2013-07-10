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
    /// 程序内的功能定义
    /// </summary>
    /// <see cref="GeneralProgram"/>
    public class GeneralProgNode : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，程序ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralProgNode),
            ErrorMessageResourceName = "ProgIDRequired")]
        public Guid ProgID { get; set; }

        /// <summary>
        /// 程序功能代码
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralProgNode),
            ErrorMessageResourceName = "CodeRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralProgNode),
            ErrorMessageResourceName = "CodeLong")]
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Column("Name")]
        public Guid? aName { get; set; }

        /// <summary>
        /// 输入模式 0编辑框 1下拉框
        /// 输入模式为1下拉框，则必须录入选项内容
        /// </summary>
        /// <see cref="ModelEnum.OptionalInputMode"/>
        public byte InputMode { get; set; }

        /// <summary>
        /// 可选内容，下拉框的内容
        /// </summary>
        [Column("Optional")]
        public Guid? aOptional { get; set; }

        /// <summary>
        /// 主键表
        /// </summary>
        [ForeignKey("ProgID")]
        public virtual GeneralProgram Program { get; set; }

        /// <summary>
        /// 名称主键表
        /// </summary>
        [ForeignKey("aName")]
        public virtual GeneralResource Name { get; set; }

        /// <summary>
        /// 选项内容主键表
        /// </summary>
        [ForeignKey("aOptional")]
        public virtual GeneralResource Optional { get; set; }
    }
}
