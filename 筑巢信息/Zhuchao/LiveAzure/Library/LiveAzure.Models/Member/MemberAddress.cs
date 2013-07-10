using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Utility;
using LiveAzure.Models.General;

namespace LiveAzure.Models.Member
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-08         -->
    /// <summary>
    /// 用户配送地址
    /// </summary>
    /// <see cref="MemberUser"/>
    public class MemberAddress : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，用户ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberAddress),
            ErrorMessageResourceName = "UserIDRequired")]
        public Guid UserID { get; set; }

        /// <summary>
        /// 代码
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberAddress),
            ErrorMessageResourceName = "CodeRequired")]
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberAddress),
            ErrorMessageResourceName = "CodeLong")]
        public string Code { get; set; }

        /// <summary>
        /// 是否默认地址
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 名
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberAddress),
            ErrorMessageResourceName = "FirstNameLong")]
        public string FirstName { get; set; }

        /// <summary>
        /// 姓
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberAddress),
            ErrorMessageResourceName = "LastNameLong")]
        public string LastName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberAddress),
            ErrorMessageResourceName = "DisplayNameLong")]
        public string DisplayName { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        [Column("Location")]
        public Guid? aLocation { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        [StringLength(512, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberAddress),
            ErrorMessageResourceName = "FullAddressLong")]
        public string FullAddress { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        [StringLength(20, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberAddress),
            ErrorMessageResourceName = "PostCodeLong")]
        public string PostCode { get; set; }

        /// <summary>
        /// 手机，至少需要一个电话
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberAddress),
            ErrorMessageResourceName = "CellPhoneLong")]
        public string CellPhone { get; set; }

        /// <summary>
        /// 工作电话
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberAddress),
            ErrorMessageResourceName = "WordPhoneLong")]
        public string WorkPhone { get; set; }

        /// <summary>
        /// 工作传真
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberAddress),
            ErrorMessageResourceName = "WorkFaxLong")]
        public string WorkFax { get; set; }

        /// <summary>
        /// 家庭电话
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberAddress),
            ErrorMessageResourceName = "HomePhoneLong")]
        public string HomePhone { get; set; }

        /// <summary>
        /// 邮件地址
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberAddress),
            ErrorMessageResourceName = "EmailLong")]
        public string Email { get; set; }

        /// <summary>
        /// 主键表，用户
        /// </summary>
        [ForeignKey("UserID")]
        public virtual MemberUser User { get; set; }

        /// <summary>
        /// 主键表，城市
        /// </summary>
        [ForeignKey("aLocation")]
        public virtual GeneralRegion Location { get; set; }
    }
}
