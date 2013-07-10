using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;

namespace LiveAzure.Models.General
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-28         -->
    /// <summary>
    /// 错误报告提交与处理
    /// </summary>
    public class GeneralErrorReport : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 事件代码，自动生成
        /// </summary>
        //[Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralErrorReport),
        //    ErrorMessageResourceName = "CodeRequired")]
        //[StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralErrorReport),
        //    ErrorMessageResourceName = "CodeLong")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Code { get; set; }

        /// <summary>
        /// 提交人
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralErrorReport),
            ErrorMessageResourceName = "UserIDRequired")]
        public Guid UserID { get; set; }

        /// <summary>
        /// 程序ID
        /// </summary>
        public Guid? ProgID { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralErrorReport),
            ErrorMessageResourceName = "TitleLong")]
        public string Title { get; set; }

        /// <summary>
        /// 内容描述
        /// </summary>
        public string Matter { get; set; }

        /// <summary>
        /// 关键词
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralErrorReport),
            ErrorMessageResourceName = "KeywordLong")]
        public string Keyword { get; set; }

        /// <summary>
        /// 附件01，用户提交的截图等，多图用Word文件记录
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralErrorReport),
            ErrorMessageResourceName = "Attached01Long")]
        public string Attached01 { get; set; }

        /// <summary>
        /// 附件02，程序员提交的截图等
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralErrorReport),
            ErrorMessageResourceName = "Attached02Long")]
        public string Attached02 { get; set; }

        /// <summary>
        /// 处理程序员
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralErrorReport),
            ErrorMessageResourceName = "ProgrammerLong")]
        public string Programmer { get; set; }

        /// <summary>
        /// 结果 0未处理 1修改成功 2不成功 3部分修正 4不预修正
        /// </summary>
        public byte Estatus { get; set; }

        /// <summary>
        /// 修改结果描述
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralErrorReport),
            ErrorMessageResourceName = "CommentLong")]
        public string Comment { get; set; }

        /// <summary>
        /// 技术参考，例如如何修复的，涉及的程序等
        /// </summary>
        [StringLength(512, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralErrorReport),
            ErrorMessageResourceName = "TechnologyLong")]
        public string Technology { get; set; }

        /// <summary>
        /// 主键表，用户
        /// </summary>
        [ForeignKey("UserID")]
        public virtual MemberUser User { get; set; }

        /// <summary>
        /// 主键表，程序
        /// </summary>
        [ForeignKey("ProgID")]
        public virtual GeneralProgram Program { get; set; }
    }
}
