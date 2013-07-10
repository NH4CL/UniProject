using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;


namespace LiveAzure.Models.General
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-07         -->
    /// <summary>
    /// 操作日志
    /// </summary>
    public class GeneralAction : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 事件ID，预定义的整数，可重复
        /// </summary>
        public int ActID { get; set; }

        /// <summary>
        /// 来源 日志产生模块 0系统 1会员 2产品 3采购 4仓库 5订单 6承运 7商城 8联盟 9知识库 10财务 11绩效 12接口 13呼叫中心
        /// </summary>
        /// <see cref="ModelEnum.ActionSource"/>
        public byte Source { get; set; }

        /// <summary>
        /// 日志级别 0操作 1警告 2错误
        /// </summary>
        /// <see cref="ModelEnum.ActionLevel"/>
        public byte Grade { get; set; }

        /// <summary>
        /// 用户名ID
        /// </summary>
        public Guid? UserID { get; set; }

        /// <summary>
        /// 类名称
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 类别，备用
        /// </summary>
        public byte Atype { get; set; }

        /// <summary>
        /// 单据类型
        /// </summary>
        /// <see cref="ModelEnum.NoteType"/>
        public byte RefType { get; set; }

        /// <summary>
        /// 关联单据号，可对应订单号等
        /// </summary>
        public Guid? RefID { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [StringLength(512)]
        public string Matter { get; set; }

        /// <summary>
        /// 关键词
        /// </summary>
        [StringLength(128)]
        public string Keyword { get; set; }

        /// <summary>
        /// 用户主键表
        /// </summary>
        [ForeignKey("UserID")]
        public virtual MemberUser User { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> RefTypeList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.NoteType), this.RefType); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string RefTypeName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.NoteType), this.RefType); }
        }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> ActionLevelList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.ActionLevel), this.Grade); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string ActionLevelName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.ActionLevel), this.Grade); }
        }
 
        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> ActionSourceList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.ActionSource), this.Source); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string ActionSourceName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.ActionSource), this.Source); }
        }
    }
}
 