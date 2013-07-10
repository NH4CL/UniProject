using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;
using LiveAzure.Models.Shipping;

namespace LiveAzure.Models.Warehouse
{
    /// <summary>
    /// 出库运输记录
    /// </summary>
    /// <see cref="WarehouseStockOut"/>
    /// <see cref="ShippingInformation"/>
    public class WarehouseOutDelivery : LiveAzure.Models.ModelBase
    {
        public WarehouseOutDelivery()
        {
            this.SendTime = DateTimeOffset.Now;
        }

        /// <summary>
        /// 主键表ID，出库单号
        /// </summary>
        [Required(ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Warehouse.WarehouseOutDelivery),
            ErrorMessageResourceName="OutIDRequired")]
        public Guid OutID { get; set; }

        /// <summary>
        /// 主键表ID，运输公司
        /// 按订单自动匹配承运商，并按正则表达式规范面单号
        /// </summary>
         [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseOutDelivery),
            ErrorMessageResourceName = "ShipIDRequired")]
        public Guid ShipID { get; set; }

        /// <summary>
        /// 面单号，同一运输公司的面单是唯一的
        /// </summary>
         [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseOutDelivery),
            ErrorMessageResourceName = "EnvelopeRequired")]
        [StringLength(50,ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Warehouse.WarehouseOutDelivery),
            ErrorMessageResourceName="EnvelopeLong")]
        public string Envelope { get; set; }

        /// <summary>
        /// 包裹重量或体积
        /// </summary>
        [Column(TypeName = "money")]
        public decimal PackWeight { get; set; }

        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTimeOffset? SendTime { get; set; }

        /// <summary>
        /// 运输跟踪记录，从运输公司的接口导入
        /// </summary>
        public string TraceRoute { get; set; }

        /// <summary>
        /// 预计到达时间
        /// </summary>
        public DateTimeOffset? ETA { get; set; }

        /// <summary>
        /// 计价
        /// </summary>
        [Column(TypeName = "money")]
        public decimal ShipPrice { get; set; }

        /// <summary>
        /// 核销
        /// </summary>
        [Column(TypeName = "money")]
        public decimal ClosePrice { get; set; }

        /// <summary>
        /// 出库单主键表
        /// </summary>
        [ForeignKey("OutID")]
        public virtual WarehouseStockOut StockOut { get; set; }

        /// <summary>
        /// 运输公司主键表
        /// </summary>
        [ForeignKey("ShipID")]
        public virtual ShippingInformation Shipper { get; set; }
    }
}
