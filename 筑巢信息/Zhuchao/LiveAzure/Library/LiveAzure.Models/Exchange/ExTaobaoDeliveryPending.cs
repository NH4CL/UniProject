using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Order;

namespace LiveAzure.Models.Exchange
{
    public class ExTaobaoDeliveryPending : LiveAzure.Models.ModelBase
    {
        public Guid OrderID { get; set; }

        /// <summary>
        /// 淘宝订单发货类型 0待同步发货 1已同步发货 2不需要同步
        /// </summary>
        /// <see cref="ModelEnum.TaobaoDeliveryStatus"/>
        public byte Dstatus { get; set; }

        public Guid? ShipID { get; set; }
        public string tid { get; set; }
        public string logistics { get; set; }
        public string out_sid { get; set; }
        public string err_msg { get; set; }

        /// <summary>
        /// 主键表，订单表
        /// </summary>
        [ForeignKey("OrderID")]
        public virtual OrderInformation Order { get; set; }
    }
}
