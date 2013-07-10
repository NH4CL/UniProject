using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.General;
using LiveAzure.Models.Shipping;


namespace LiveAzure.Models.Order
{
    /// <summary>
    /// 订单多种可选配送方式
    /// </summary>
    /// <see cref="OrderInformation"/>
    public class OrderShipping : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，订单ID
        /// </summary>
        [Required (ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Order.OrderShipping),
            ErrorMessageResourceName="OrderIDRequired")]
        public Guid OrderID { get; set; }

        /// <summary>
        /// 主键表ID，承运商
        /// </summary>
         [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderShipping),
            ErrorMessageResourceName = "ShipIDRequired")]
        public Guid ShipID { get; set; }

        /// <summary>
        /// 权重，大者优先
        /// </summary>
        public int ShipWeight { get; set; }

        /// <summary>
        /// 审单结果 0待审核 1通过 2不通过 3不需要审单
        /// </summary>
        /// <see cref="ModelEnum.ShippingCheck"/>
        public byte Ostatus { get; set; }

        /// <summary>
        /// 最终实际承运商，每单只能有一个选中
        /// </summary>
        public bool Candidate { get; set; }
        
        /// <summary>
        /// 订单主键表
        /// </summary>
        [ForeignKey("OrderID")]
        public virtual OrderInformation Order { get; set; }

        /// <summary>
        /// 承运主键表
        /// </summary>
        [ForeignKey("ShipID")]
        public virtual ShippingInformation Shipper { get; set; }
        
        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> ShippingCheckList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.ShippingCheck), this.Ostatus); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string ShippingCheckName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.ShippingCheck), this.Ostatus); }
        }
    }
}
