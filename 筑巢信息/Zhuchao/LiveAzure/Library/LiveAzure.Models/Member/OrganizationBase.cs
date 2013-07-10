using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.General;

namespace LiveAzure.Models.Member
{
    /// <summary>
    /// 组织基类，分别用于运营商，渠道，仓库，供应商，承运商等
    /// </summary>
    /// <see cref="MemberOrganization"/>
    /// <see cref="MemberChannel"/>
    /// <see cref="WarehouseInformation"/>
    /// <see cref="PurchaseSupplier"/>
    /// <see cref="ShippingInformation"/>
    [Table("MemberOrganization")]
    public class OrganizationBase : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 组织代码，全局唯一
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.OrganizationBase),
            ErrorMessageResourceName = "CodeRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.OrganizationBase),
            ErrorMessageResourceName = "CodeLong")]
        public string Code { get; set; }

        /// <summary>
        /// 扩展代码，例如运输公司需要符合淘宝代码规则
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.OrganizationBase),
            ErrorMessageResourceName = "ExCodeLong")]
        public string ExCode { get; set; }

        /// <summary>
        /// 组织状态，0无效 1有效
        /// </summary>
        /// <see cref="ModelEnum.OrganizationStatus"/>
        public byte Ostatus { get; set; }

        /// <summary>
        /// 组织类型，在xml中定义名称，0运营商、1渠道、2仓库、3供应商、4运输公司
        /// </summary>
        /// <see cref="ModelEnum.OrganizationType"/>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.OrganizationBase),
            ErrorMessageResourceName = "OtypeRequired")]
        public byte Otype { get; set; }

        /// <summary>
        /// 扩展类型，例如运输公司类型，备用，可空
        /// </summary>
        public Guid? ExType { get; set; }

        /// <summary>
        /// 上级组织
        /// </summary>
        [Column("Parent")]
        public Guid? aParent { get; set; }

        /// <summary>
        /// 是否末级
        /// </summary>
        public bool Terminal { get; set; }

        /// <summary>
        /// 全名称
        /// </summary>
        [Column("FullName")]
        public Guid? aFullName { get; set; }

        /// <summary>
        /// 短名称
        /// </summary>
        [Column("ShortName")]
        public Guid? aShortName { get; set; }

        /// <summary>
        /// 城市（第0or1or2级均可）
        /// </summary>
        [Column("Location")]
        public Guid? aLocation { get; set; }

        /// <summary>
        /// 详细地址，母语描述
        /// </summary>
        [StringLength(512, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.OrganizationBase),
            ErrorMessageResourceName = "FullAddressLong")]
        public string FullAddress { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        [StringLength(20, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.OrganizationBase),
            ErrorMessageResourceName = "PostCodeLong")]
        public string PostCode { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.OrganizationBase),
            ErrorMessageResourceName = "ContactLong")]
        public string Contact { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.OrganizationBase),
            ErrorMessageResourceName = "CellPhoneLong")]
        public string CellPhone { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.OrganizationBase),
            ErrorMessageResourceName = "WorkPhoneLong")]
        public string WorkPhone { get; set; }

        /// <summary>
        /// 传真
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.OrganizationBase),
            ErrorMessageResourceName = "WorkFaxLong")]
        public string WorkFax { get; set; }

        /// <summary>
        /// 邮件地址
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.OrganizationBase),
            ErrorMessageResourceName = "EmailLong")]
        public string Email { get; set; }

        /// <summary>
        /// 公司主页地址
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.OrganizationBase),
            ErrorMessageResourceName = "HomeUrlLong")]
        public string HomeUrl { get; set; }

        /// <summary>
        /// 推荐排序值
        /// </summary>
        public int Sorting { get; set; }

        /// <summary>
        /// 默认语言简介
        /// </summary>
        [StringLength(512, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.OrganizationBase),
            ErrorMessageResourceName = "BriefLong")]
        public string Brief { get; set; }

        /// <summary>
        /// 详细介绍
        /// </summary>
        [Column("Introduction")]
        public Guid? aIntroduction { get; set; }

        /// <summary>
        /// 产品PU码规则，正则表达式
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.OrganizationBase),
            ErrorMessageResourceName = "ProdCodePolicyLong")]
        public string ProdCodePolicy { get; set; }

        /// <summary>
        /// 产品SKU码正则表达式
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.OrganizationBase),
            ErrorMessageResourceName = "SkuCodePolicyLong")]
        public string SkuCodePolicy { get; set; }

        /// <summary>
        /// 产品条码正则表达式
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.OrganizationBase),
            ErrorMessageResourceName = "BarcodePolicyLong")]
        public string BarcodePolicy { get; set; }
        
        /// <summary>
        /// 标准分类
        /// </summary>
        [ForeignKey("ExType")]
        public virtual GeneralStandardCategory ExtendType { get; set; }

        /// <summary>
        /// 上级组织
        /// </summary>
        [ForeignKey("aParent")]
        public virtual OrganizationBase Parent { get; set; }

        /// <summary>
        /// 子项内容
        /// </summary>
        [InverseProperty("Parent")]
        public virtual ICollection<OrganizationBase> ChildItems { get; set; }

        /// <summary>
        /// 组织完整名称
        /// </summary>
        [ForeignKey("aFullName")]
        public virtual GeneralResource FullName { get; set; }

        /// <summary>
        /// 组织短名称
        /// </summary>
        [ForeignKey("aShortName")]
        public virtual GeneralResource ShortName { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        [ForeignKey("aLocation")]
        public virtual GeneralRegion Location { get; set; }

        /// <summary>
        /// 组织详细介绍
        /// </summary>
        [ForeignKey("aIntroduction")]
        public virtual GeneralLargeObject Introduction { get; set; }

        /// <summary>
        /// 从表内容,组织/仓库支持的渠道，对供应商、承运商无效
        /// </summary>
        [InverseProperty("Organization")]
        public virtual ICollection<MemberOrgChannel> Channels { get; set; }

        /// <summary>
        /// 获取组织类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> OrganTypeList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.OrganizationType), this.Otype); }
        }

        /// <summary>
        /// 某个组织类型的名称
        /// </summary>
        [NotMapped]
        public string OrganTypeName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.OrganizationType), this.Otype); }
        }

        /// <summary>
        /// 获取组织类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> OrganStatusList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.OrganizationStatus), this.Ostatus); }
        }

        /// <summary>
        /// 某个组织类型的名称
        /// </summary>
        [NotMapped]
        public string OrganStatusName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.OrganizationStatus), this.Ostatus); }
        }
    }
}
