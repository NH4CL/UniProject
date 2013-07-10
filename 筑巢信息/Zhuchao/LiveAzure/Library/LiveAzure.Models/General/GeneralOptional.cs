using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;


namespace LiveAzure.Models.General
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-07         -->
    /// <summary>
    /// 动态属性，自定义属性
    /// </summary>
    /// <see cref="GeneralOptItem"/>
    public class GeneralOptional : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 组织
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralOptional),
            ErrorMessageResourceName = "OrgIDRequired")]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 用途 0通用 1组织 2会员 3产品 4订单 5...
        /// </summary>
        /// <see cref="ModelEnum.OptionalType"/>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralOptional),
            ErrorMessageResourceName = "Otype")]
        public byte Otype { get; set; }
        
        /// <summary>
        /// 代码，组织内唯一
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralOptional),
            ErrorMessageResourceName = "CodeRequired")]
        [StringLength(20, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralOptional),
            ErrorMessageResourceName = "CodeLong")]
        public string Code { get; set; }
        
        /// <summary>
        /// 参考ID，例如产品中的适用类别，空表示适用全部产品
        /// </summary>
        public Guid? RefID { get; set; }
        
        /// <summary>
        /// 排序
        /// </summary>
        public int Sorting { get; set; }
        
        /// <summary>
        /// 属性名称
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
        /// 组织主键表
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }

        /// <summary>
        /// 名称主键表
        /// </summary>
        [ForeignKey("aName")]
        public virtual GeneralResource Name { get; set; }

        /// <summary>
        /// 从表内容，选项内容
        /// </summary>
        [InverseProperty("Optional")]
        public virtual ICollection<GeneralOptItem> OptionalItems { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> OptionalTypeList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.OptionalType), this.Otype); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string OptionalTypeName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.OptionalType), this.Otype); }
        }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> InputModeList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.OptionalInputMode), this.InputMode); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string InputModeName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.OptionalInputMode), this.InputMode); }
        }
    }
}
