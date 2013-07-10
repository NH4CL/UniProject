using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.General;

namespace LiveAzure.Models.Member
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-13         -->
    /// <summary>
    /// 组织支持的语言文化，货币
    /// </summary>
    /// <see cref="MemberOrganization"/>
    /// <see cref="GeneralCultureUnit"/>
    /// <see cref="GeneralMeasureUnit"/>
    public class MemberOrgCulture : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，组织ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberOrgCulture),
            ErrorMessageResourceName = "OrgIDRequired")]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 支持的类型 0语言 1货币
        /// </summary>
        /// <see cref="ModelEnum.CultureType"/>
        public byte Ctype { get; set; }
        
        /// <summary>
        /// 语言文化
        /// </summary>
        [Column("Culture")]
        public Guid? aCulture { get; set; }

        /// <summary>
        /// 支持的货币
        /// </summary>
        [Column("Currency")]
        public Guid? aCurrency { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sorting { get; set; }

        /// <summary>
        /// 组织主键表
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }

        /// <summary>
        /// 语言文化主键表
        /// </summary>
        [ForeignKey("aCulture")]
        public virtual GeneralCultureUnit Culture { get; set; }

        /// <summary>
        /// 货币主键表
        /// </summary>
        [ForeignKey("aCurrency")]
        public virtual GeneralMeasureUnit Currency { get; set; }
        
        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> CultureTypeList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.CultureType), this.Ctype); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string CultureTypeName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.CultureType), this.Ctype); }
        }
    }
}
