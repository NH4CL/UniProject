using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LiveAzure.Models.Order
{
    /// <summary>
    /// 订单处理日志，可显示在页面个客户查看
    /// </summary>
    /// <see cref="OrderInformation"/>
    public class OrderProcess:LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，订单ID
        /// </summary>
        [Required(ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Order.OrderProcess),
            ErrorMessageResourceName="OrderIDRequired")]
        public Guid OrderID { get; set; }

        /// <summary>
        /// 代码，可空
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderProcess),
            ErrorMessageResourceName = "CodeLong")]
        public string Code { get; set; }

        /// <summary>
        /// 是否显示在官网
        /// </summary>
        public bool Show { get; set; }

        /// <summary>
        /// 事件内容，下单默认语言，从资源文件中读取
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderProcess),
            ErrorMessageResourceName = "MatterLong")]
        public string Matter { get; set; }

        /// <summary>
        /// 订单主键表
        /// </summary>
        [ForeignKey("OrderID")]
        public virtual OrderInformation Order { get; set; }
    }
}
