using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LiveAzure.Models.General
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-19         -->
    /// <summary>
    /// 权限控制模板，可以与某用户的权限互相复制
    /// </summary>
    /// <see cref="MemberPrivilege"/>
    /// <see cref="GeneralPrivItem"/>
    public class GeneralPrivTemplate : LiveAzure.Models.ModelBase
    {

        // TODO 与某用户的权限互相复制


        /// <summary>
        /// 代码或名称
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralPrivTemplate),
            ErrorMessageResourceName = "CodeRequired")]
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralPrivTemplate),
            ErrorMessageResourceName = "CodeLong")]
        public string Code { get; set; }

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
        /// 从表内容，用户权限
        /// </summary>
        [InverseProperty("Template")]
        public virtual ICollection<GeneralPrivItem> TemplateItems { get; set; }
    }
}
