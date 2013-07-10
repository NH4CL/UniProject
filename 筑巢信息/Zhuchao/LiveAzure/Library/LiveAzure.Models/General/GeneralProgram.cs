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
    /// 程序定义
    /// </summary>
    /// <see cref="GeneralProgNode"/>
    public class GeneralProgram : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public GeneralProgram()
        {
            this.Terminal = false;
            this.Show = true;
        }

        /// <summary>
        /// 按代码查询程序功能节点的内容
        /// </summary>
        /// <param name="sCode">节点代码</param>
        /// <returns>程序功能节点</returns>
        public GeneralProgNode FindProgramNode(string sCode)
        {
            GeneralProgNode oNode = null;
            if (this.ProgramNodes != null)
                oNode = this.ProgramNodes.FirstOrDefault(n => n.Code == sCode);
            return oNode;
        }

        /// <summary>
        /// 程序代码，ControllerView名称
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralProgram),
            ErrorMessageResourceName = "CodeRequired")]
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralProgram),
            ErrorMessageResourceName = "CodeLong")]
        public string Code { get; set; }

        /// <summary>
        /// 上级程序，菜单层级
        /// </summary>
        [Column("Parent")]
        public Guid? aParent { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Column("Name")]
        public Guid? aName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sorting { get; set; }

        /// <summary>
        /// 程序URL地址
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralProgram),
            ErrorMessageResourceName = "ProgUrlLong")]
        public string ProgUrl { get; set; }

        /// <summary>
        /// 是否末级
        /// </summary>
        public bool Terminal { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool Show { get; set; }

        /// <summary>
        /// 父项内容
        /// </summary>
        [ForeignKey("aParent")]
        public virtual GeneralProgram Parent { get; set; }

        /// <summary>
        /// 子项内容
        /// </summary>
        [InverseProperty("Parent")]
        public virtual ICollection<GeneralProgram> ChildItems { get; set; }

        /// <summary>
        /// 名称主键表
        /// </summary>
        [ForeignKey("aName")]
        public virtual GeneralResource Name { get; set; }

        /// <summary>
        /// 从表内容，选项内容
        /// </summary>
        [InverseProperty("Program")]
        public virtual ICollection<GeneralProgNode> ProgramNodes { get; set; }
    }
}
