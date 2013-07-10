using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.General;
using LiveAzure.Models.Order;

namespace LiveAzure.Models.Member
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-28         -->
    /// <summary>
    /// 用户快捷按钮
    /// </summary>
    /// <see cref="MemberUser"/>
    public class MemberUserShortcut : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，用户ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUserShortcut),
            ErrorMessageResourceName = "UserIDRequired")]
        public Guid UserID { get; set; }

        /// <summary>
        /// 类型 0程序 1URL
        /// </summary>
        /// Enum 待定
        public byte Stype { get; set; }

        /// <summary>
        /// 程序ID
        /// </summary>
        public Guid? ProgID{get;set;}

        /// <summary>
        /// 跳转URL
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUserShortcut),
            ErrorMessageResourceName = "LinkUrlLong")]
        public string LinkUrl { get; set; }
        
        /// <summary>
        /// 排序
        /// </summary>
        public int Sorting{get;set;}
        
        /// <summary>
        /// 图标地址
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberUserShortcut),
            ErrorMessageResourceName = "IconLong")]
        public string Icon { get; set; }

        /// <summary>
        /// 用户主键表
        /// </summary>
        [ForeignKey("UserID")]
        public virtual MemberUser User { get; set; }

        /// <summary>
        /// 程序主键表
        /// </summary>
        [ForeignKey("ProgID")]
        public virtual GeneralProgram Program { get; set; }
    }
}
