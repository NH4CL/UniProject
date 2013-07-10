using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LiveAzure.Models.General
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-07         -->
    /// <summary>
    /// 国家、区域
    /// </summary>
    public class GeneralRegion : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 代码，国家唯一，但地区不一定唯一，程序中不需要判断
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralRegion),
            ErrorMessageResourceName = "CodeLong")]
        public string Code { get; set; }

        /// <summary>
        /// 上级地区
        /// </summary>
        [Column("Parent")]
        public Guid? aParent { get; set; }

        /// <summary>
        /// 地区全名称，母语
        /// </summary>
        [StringLength(512, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralRegion),
            ErrorMessageResourceName = "FullNameLong")]
        public string FullName { get; set; }

        /// <summary>
        /// 地区简称，母语
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralRegion),
            ErrorMessageResourceName = "ShortNameLong")]
        public string ShortName { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        [StringLength(20, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.General.GeneralRegion),
            ErrorMessageResourceName = "PostCodeLong")]
        public string PostCode { get; set; }

        /// <summary>
        /// 地址映射，淘宝
        /// </summary>
        public string Map01 { get; set; }

        /// <summary>
        /// 地址映射，备用
        /// </summary>
        public string Map02 { get; set; }

        /// <summary>
        /// 地址映射，备用
        /// </summary>
        public string Map03 { get; set; }

        /// <summary>
        /// 地址映射，备用
        /// </summary>
        public string Map04 { get; set; }

        /// <summary>
        /// 地址映射，备用
        /// </summary>
        public string Map05 { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sorting { get; set; }

        /// <summary>
        /// 地区层级
        /// </summary>
        public int RegionLevel { get; set; }

        /// <summary>
        /// 统计分类01
        /// </summary>
        public int Statistics01 { get; set; }

        /// <summary>
        /// 统计分类02
        /// </summary>
        public int Statistics02 { get; set; }

        /// <summary>
        /// 统计分类03
        /// </summary>
        public int Statistics03 { get; set; }

        /// <summary>
        /// 统计分类04
        /// </summary>
        public int Statistics04 { get; set; }

        /// <summary>
        /// 统计分类05
        /// </summary>
        public int Statistics05 { get; set; }

        /// <summary>
        /// ChildItems计数，不能直接使用ChildItems，因为没有检测删除项
        /// </summary>
        [NotMapped]
        public int ChildCount
        {
            get
            {
                int i = 0;
                if (ChildItems != null)
                    i = ChildItems.Count(c => !c.Deleted);
                return i;
            }
        }

        /// <summary>
        /// 父项内容
        /// </summary>
        [ForeignKey("aParent")]
        public virtual GeneralRegion Parent { get; set; }

        /// <summary>
        /// 子项内容
        /// </summary>
        [InverseProperty("Parent")]
        public virtual ICollection<GeneralRegion> ChildItems { get; set; }
    }
}
