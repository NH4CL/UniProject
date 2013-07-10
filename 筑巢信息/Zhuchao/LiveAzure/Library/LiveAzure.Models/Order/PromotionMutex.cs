using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LiveAzure.Models.Order
{
    /// <summary>
    /// 促销互斥关系，不设置表示互容
    /// </summary>
    /// <see cref="PromotionInformation"/>
    public class PromotionMutex : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，促销ID
        /// </summary>
        [Required(ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Order.PromotionMutex),
            ErrorMessageResourceName="PromIDRequired")]
        public Guid PromID { get; set; }

        /// <summary>
        /// 主键表ID，互斥ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.PromotionMutex),
            ErrorMessageResourceName = "MutexIDRequired")]
        public Guid MutexID { get; set; }

        /// <summary>
        /// 关系，0互斥 1互容
        /// </summary>
        /// <see cref="ModelEnum.PromotionRelation"/>
        public byte Relation { get; set; }

        /// <summary>
        /// 主键表，促销方案
        /// </summary>
        [ForeignKey("PromID")]
        public virtual PromotionInformation Promotion { get; set; }

        /// <summary>
        /// 主键表，互斥的促销方案
        /// </summary>
        [ForeignKey("MutexID")]
        public virtual PromotionInformation Mutex { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> RelationList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.PromotionRelation), this.Relation); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string RelationName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.PromotionRelation), this.Relation); }
        }
    }
}
