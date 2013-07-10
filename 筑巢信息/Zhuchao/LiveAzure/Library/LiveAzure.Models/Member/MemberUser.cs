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
    /// <!--版本：v1.0 2011-07-07         -->
    /// <summary>
    /// 用户，包括内部用户和公众用户
    /// </summary>
    /// <see cref="MemberOrganization"/>
    /// <see cref="MemberRole"/>
    /// <see cref="MemberChannel"/>
    /// <see cref="MemberAddress"/>
    /// <see cref="GeneralCultureUnit"/>
    public class MemberUser : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MemberUser()
        {
            this.Ustatus = (byte)ModelEnum.UserStatus.VALID;
            this.LastLoginTime = DateTimeOffset.Now;
        }

        /// <summary>
        /// 主键表ID，组织ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUser),
            ErrorMessageResourceName = "OrgIDRequired")]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 主键表ID，角色ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUser),
            ErrorMessageResourceName = "RoleIDRequired")]
        public Guid RoleID { get; set; }

        /// <summary>
        /// 主键表ID，渠道ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUser),
            ErrorMessageResourceName = "ChannelRequired")]
        public Guid ChlID { get; set; }

        /// <summary>
        /// 登陆名，全局唯一
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUser),
            ErrorMessageResourceName = "LoginNameRequired")]
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUser),
            ErrorMessageResourceName = "LoginNameLong")]
        public string LoginName { get; set; }

        /// <summary>
        /// 直接上级，可空
        /// </summary>
        [Column("Manager")]
        public Guid? aManager { get; set; }

        /// <summary>
        /// 外部代码，例如淘宝用户数字ID
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUser),
            ErrorMessageResourceName = "ExCodeLong")]
        public string ExCode { get; set; }

        /// <summary>
        /// 幸运号，相当于Gid，全局唯一，订单符合条件时一次生成，且不可更改
        /// </summary>
        public int LuckyNumber { get; set; }

        /// <summary>
        /// 是否实名认证
        /// </summary>
        public bool Authenticate { get; set; }

        /// <summary>
        /// 状态 0:禁止 1:有效(必须所属组织有效，公众？)
        /// </summary>
        /// <see cref="ModelEnum.UserStatus"/>
        public byte Ustatus { get; set; }

        /// <summary>
        /// 用户级别
        /// </summary>
        [Column("UserLevel")]
        public Guid? aUserLevel { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUser),
            ErrorMessageResourceName = "NickNameLong")]
        public string NickName { get; set; }

        /// <summary>
        /// 名
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUser),
            ErrorMessageResourceName = "FirstNameLong")]
        public string FirstName { get; set; }

        /// <summary>
        /// 姓
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUser),
            ErrorMessageResourceName = "LastNameLong")]
        public string LastName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUser),
            ErrorMessageResourceName = "DisplayNameLong")]
        public string DisplayName { get; set; }

        /// <summary>
        /// 密码密文
        /// </summary>
        [StringLength(256)]
        [Column("Passcode")]
        public string aPasscode { get; set; }

        /// <summary>
        /// 密码密文，明文不能超过100字节
        /// </summary>
        [NotMapped]
        public string Passcode
        {
            get
            {
                return aPasscode;
            }
            set
            {
                if (value.Length > 100)
                    value = value.Substring(0, 100);
                this.SaltKey = CommonHelper.RandomNumber(8);
                this.aPasscode = CommonHelper.EncryptDES(value, this.SaltKey);
            }
        }

        /// <summary>
        /// 密钥，设置密码时随机产生
        /// </summary>
        public string SaltKey { get; set; }

        /// <summary>
        /// 默认语言
        /// </summary>
        [Column("Culture")]
        public Guid? aCulture { get; set; }
        
        /// <summary>
        /// 称呼
        /// </summary>
        [StringLength(10, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUser),
            ErrorMessageResourceName = "TitleLong")]
        public string Title { get; set; }
        
        /// <summary>
        /// 性别 性别 0未知 1男 2女
        /// </summary>
        /// <see cref="ModelEnum.Gender"/>
        public byte Gender { get; set; }
        
        /// <summary>
        /// 头像文件链接，从已有图片中选择
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUser),
            ErrorMessageResourceName = "HeadPicLong")]
        public string HeadPic { get; set; }
        
        /// <summary>
        /// 用户签名
        /// </summary>
        [StringLength(512, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUser),
            ErrorMessageResourceName = "UserSignLong")]
        public string UserSign { get; set; }
        
        /// <summary>
        /// 简介
        /// </summary>
        [StringLength(512, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUser),
            ErrorMessageResourceName = "BriefLong")]
        public string Brief { get; set; }
        
        /// <summary>
        /// 生日
        /// </summary>
        public DateTimeOffset? Birthday { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string CellPhone { get; set; }

        /// <summary>
        /// 邮件地址
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUser),
            ErrorMessageResourceName = "EmailLong")]
        public string Email { get; set; }
        
        /// <summary>
        /// 上次登录时间
        /// </summary>
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset? LastLoginTime { get; set; }
        
        /// <summary>
        /// 上次登录IP地址
        /// </summary>
        [StringLength(20, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUser),
            ErrorMessageResourceName = "LastLoginIPLong")]
        public string LastLoginIP { get; set; }

        /// <summary>
        /// 组织主键表
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }

        /// <summary>
        /// 角色主键表
        /// </summary>
        [ForeignKey("RoleID")]
        public virtual MemberRole Role { get; set; }

        /// <summary>
        /// 渠道主键表
        /// </summary>
        [ForeignKey("ChlID")]
        public virtual MemberChannel Channel { get; set; }

        /// <summary>
        /// 上级
        /// </summary>
        [ForeignKey("aManager")]
        public virtual MemberUser Manager { get; set; }

        /// <summary>
        /// 下级
        /// </summary>
        [InverseProperty("Manager")]
        public virtual ICollection<MemberUser> ChildItems { get; set; }

        /// <summary>
        /// 外键，用户级别
        /// </summary>
        [ForeignKey("aUserLevel")]
        public virtual MemberLevel UserLevel { get; set; }

        /// <summary>
        /// 默认语言文化
        /// </summary>
        [ForeignKey("aCulture")]
        public virtual GeneralCultureUnit Culture { get; set; }

        /// <summary>
        /// 从表内容，配送地址
        /// </summary>
        [InverseProperty("User")]
        public virtual ICollection<MemberAddress> Addresses { get; set; }

        /// <summary>
        /// 从表内容，用户自定义属性
        /// </summary>
        [InverseProperty("User")]
        public virtual ICollection<MemberUserAttribute> Attributes { get; set; }

        /// <summary>
        /// 从表内容，用户积分
        /// </summary>
        [InverseProperty("User")]
        public virtual ICollection<MemberPoint> Points { get; set; }

        ///// <summary>
        ///// 从表内容，用户订阅
        ///// </summary>
        //[ForeignKey("")]
        //public virtual MemberSubscribe UserSubscribe { get; set; }

        /// <summary>
        /// 从表内容，用户事件
        /// </summary>
        [InverseProperty("User")]
        public virtual ICollection<MemberUserEvent> Events { get; set; }

        /// <summary>
        /// 从表内容，用户权限
        /// </summary>
        [InverseProperty("User")]
        public virtual ICollection<MemberPrivilege> Privileges { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> UserStatusList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.UserStatus), this.Ustatus); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string UserStatusName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.UserStatus), this.Ustatus); }
        }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> GenderList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.Gender), this.Gender); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string GenderName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.Gender), this.Gender); }
        }
    }
}
