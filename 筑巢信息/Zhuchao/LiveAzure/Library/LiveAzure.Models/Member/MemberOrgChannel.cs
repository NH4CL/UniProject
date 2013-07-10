using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LiveAzure.Models.Member
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-07         -->
    /// <summary>
    /// 支持的渠道
    /// </summary>
    /// <see cref="MemberOrganization"/>
    /// <see cref="MemberChannel"/>
    public class MemberOrgChannel : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，组织ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberOrgChannel),
            ErrorMessageResourceName = "OrgIDRequired")]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 主键表ID，渠道ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberOrgChannel),
            ErrorMessageResourceName = "ChlIDRequired")]
        public Guid ChlID { get; set; }

        /// <summary>
        /// 状态，0不支持 1支持
        /// </summary>
        /// <see cref="ModelEnum.GenericStatus"/>
        public byte Cstatus { get; set; }

        /// <summary>
        /// 渠道配置文件，例如淘宝url等
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberOrgChannel),
            ErrorMessageResourceName = "RemoteUrlLong")]
        public string RemoteUrl { get; set; }

        /// <summary>
        /// 渠道配置文件，例如淘宝appkey等
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberOrgChannel),
            ErrorMessageResourceName = "ConfigKeyLong")]
        public string ConfigKey { get; set; }

        /// <summary>
        /// 渠道配置文件，例如淘宝密钥等
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberOrgChannel),
            ErrorMessageResourceName = "SecretKeyLong")]
        public string SecretKey { get; set; }

        /// <summary>
        /// 渠道配置文件，例如淘宝SessionKey等
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberOrgChannel),
            ErrorMessageResourceName = "SessionKeyLong")]
        public string SessionKey { get; set; }

        /// <summary>
        /// 组织主键表
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual OrganizationBase Organization { get; set; }

        /// <summary>
        /// 组织渠道表
        /// </summary>
        [ForeignKey("ChlID")]
        public virtual MemberChannel Channel { get; set; }

        //add by tianyou 2011.10.08
        /// <summary>
        /// 组织支持的渠道的状态列表
        /// </summary>
        [NotMapped]
        public List<ListItem> ChannelStatusList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.ChannelStatus), this.Cstatus); }
        }

        /// <summary>
        /// 组织支持的渠道状态的名称
        /// </summary>
        [NotMapped]
        public string ChannelStatusName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.ChannelStatus), this.Cstatus); }
        }
    }
}
