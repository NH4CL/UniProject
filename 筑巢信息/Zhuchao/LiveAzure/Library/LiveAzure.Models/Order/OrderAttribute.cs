using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.General;

namespace LiveAzure.Models.Order
{
    /// <summary>
    /// 订单自定义属性
    /// </summary>
    /// <see cref="OrderInformation"/>
    public class OrderAttribute : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，订单ID
        /// </summary>
        [Required(ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Order.OrderAttribute),
            ErrorMessageResourceName="OrderIDRequired")]
        public Guid OrderID { get; set; }

        /// <summary>
        /// 主键表ID，属性
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderAttribute),
            ErrorMessageResourceName = "OptIDRequired")]
        public Guid OptID { get; set; }

        /// <summary>
        /// 属性内容，如果是下拉框，则存入optItem.gid，可空
        /// </summary>
        public Guid? OptResult { get; set; }

        /// <summary>
        /// 属性内容，如果是编辑框，则存入值，可空
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderAttribute),
            ErrorMessageResourceName = "MatterLong")]
        public string Matter { get; set; }

        /// <summary>
        /// 订单主键表
        /// </summary>
        [ForeignKey("OrderID")]
        public virtual OrderInformation Order { get; set; }

        /// <summary>
        /// 属性主键表
        /// </summary>
        [ForeignKey("OptID")]
        public virtual GeneralOptional Optional { get; set; }

        /// <summary>
        /// 主键表,属性内容
        /// </summary>
        [ForeignKey("OptResult")]
        public virtual GeneralOptItem OptionalResult { get; set; }
    }
}
