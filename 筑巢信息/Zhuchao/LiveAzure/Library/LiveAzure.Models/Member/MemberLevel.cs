using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.General;

namespace LiveAzure.Models.Member
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-08         -->
    /// <summary>
    /// 用户级别定义
    /// </summary>
    /// <see cref="MemberUser"/>
    /// <see cref="ProductOnLevelDiscount"/>
    public class MemberLevel : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 组织ID
        /// </summary>
        public Guid OrgID { get; set; }

        /// <summary>
        /// 级别代码
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberLevel),
            ErrorMessageResourceName = "CodeLong")]
        public string Code { get; set; }

        /// <summary>
        /// 级别名称
        /// </summary>
        [Column("Name")]
        public Guid? aName { get; set; }

        /// <summary>
        /// 级别，数值越大级别越高
        /// </summary>
        public byte Mlevel { get; set; }

        /// <summary>
        /// 标准折扣
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// 组织主键表
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }
    
        /// <summary>
        /// 级别名称
        /// </summary>
        [ForeignKey("aName")]
        public virtual GeneralResource Name { get; set; }
    }
}
