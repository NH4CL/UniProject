using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;


namespace LiveAzure.Models.General
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-08-10         -->
    /// <summary>
    /// 待办事项，包括后台运行程序的紧急日志等
    /// </summary>
    public class GeneralTodoList : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 事件代码，自动生成
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Code { get; set; }

        /// <summary>
        /// 类别 0订单待确认 1订单待发货 2转淘宝订单提示 3淘宝发货提示
        /// </summary>
        /// <see cref="ModelEnum.TodoType"/>
        public byte Ttype { get; set; }

        /// <summary>
        /// 结果 0未处理(显示) 1已处理(不显示)
        /// </summary>
        public byte Tstatus { get; set; }

        /// <summary>
        /// 标题，内容
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralTodoList),
            ErrorMessageResourceName = "TitleLong")]
        public string Title { get; set; }

        /// <summary>
        /// 关键词
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralTodoList),
            ErrorMessageResourceName = "KeywordLong")]
        public string Keyword { get; set; }

        /// <summary>
        /// 技术参考，例如如何修复的，涉及的程序等
        /// </summary>
        [StringLength(512, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralTodoList),
            ErrorMessageResourceName = "TechnologyLong")]
        public string Technology { get; set; }
        
        /// <summary>
        /// 跳转地址
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralTodoList),
            ErrorMessageResourceName = "JumpUrlLong")]
        public string JumpUrl { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> TodoTypeList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.TodoType), this.Ttype); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string TodoTypeName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.TodoType), this.Ttype); }
        }
    }
}
