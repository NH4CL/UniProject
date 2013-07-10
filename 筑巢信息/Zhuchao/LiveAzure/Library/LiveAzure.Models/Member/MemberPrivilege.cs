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
    /// 权限控制，按组织，渠道，程序，程序功能
    /// 类型为组织/渠道/仓库/程序，则值为组织的GUID，表示有权限进入该组织/渠道/仓库/程序
    /// 类型为程序功能，则节点功能代码为GeneralProgNode.Code，节点功能授权值为GeneralProgNode.Optional.Key
    /// 每个程序打开时，在GeneralProgram检测this.ProgID->Permission，读取NodeCode和NodeValue到Dictionary中备用
    /// </summary>
    /// <see cref="MemberUser"/>
    /// <see cref="GeneralPrivTemplate"/>
    public class MemberPrivilege : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，积分ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberPrivilege),
            ErrorMessageResourceName = "UserIDRequired")]
        public Guid UserID { get; set; }

        /// <summary>
        /// 类型xml定义 0程序 1程序功能 2组织 3渠道 4仓库 5商品类别私有 6供应商类别 7...
        /// </summary>
        /// <see cref="ModelEnum.UserPrivType"/>
        public byte Ptype { get; set; }

        /// <summary>
        /// 状态 (0无效 1启用)
        /// </summary>
        public byte Pstatus { get; set; }

        /// <summary>
        /// 用户主键表
        /// </summary>
        [ForeignKey("UserID")]
        public virtual MemberUser User { get; set; }

        /// <summary>
        /// 从表内容，用户权限
        /// </summary>
        [InverseProperty("Privilege")]
        public virtual ICollection<MemberPrivItem> PrivilegeItems { get; set; }
        
        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> PrivilegeTypeList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.UserPrivType), this.Ptype); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string PrivilegeTypeName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.UserPrivType), this.Ptype); }
        }
    }
}
