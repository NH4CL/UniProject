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
    /// 私有分类，仅组织内部使用
    /// </summary>
    /// <seealso cref="GeneralStandardCategory"/>
    public class GeneralPrivateCategory : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 组织
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralPrivateCategory),
            ErrorMessageResourceName = "OrgIDRequired")]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 类别 0商品私有分类 1仓库分类 2运输公司分类 3采购类型 4供应商类型...
        /// </summary>
        /// <see cref="ModelEnum.PrivateCategoryType"/>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralPrivateCategory),
            ErrorMessageResourceName = "CtypeRequired")]
        public byte Ctype { get; set; }
        
        /// <summary>
        /// 代码，组织和类别内唯一
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralPrivateCategory),
            ErrorMessageResourceName = "CodeRequired")]
        [StringLength(20, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralPrivateCategory),
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
        /// 是否显示
        /// </summary>
        public bool Show { get; set; }

        /// <summary>
        /// 默认标准(最小)计量单位，即入库计量单位
        /// </summary>
        public Guid? StdUnit { get; set; }

        /// <summary>
        /// 是否显示保质期
        /// </summary>
        public bool ShowGuarantee { get; set; }

        /// <summary>
        /// 组织主键表
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }
                
        /// <summary>
        /// 父项内容
        /// </summary>
        [ForeignKey("aParent")]
        public virtual GeneralPrivateCategory Parent { get; set; }

        /// <summary>
        /// 子项内容
        /// </summary>
        [InverseProperty("Parent")]
        public virtual ICollection<GeneralPrivateCategory> ChildItems { get; set; }

        /// <summary>
        /// 名称主键表
        /// </summary>
        [ForeignKey("aName")]
        public virtual GeneralResource Name { get; set; }

        /// <summary>
        /// 计量单位主键表
        /// </summary>
        [ForeignKey("StdUnit")]
        public virtual GeneralMeasureUnit StandardUnit { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> CategoryTypeList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.PrivateCategoryType), this.Ctype); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string CategoryTypeName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.PrivateCategoryType), this.Ctype); }
        }
    }
}
