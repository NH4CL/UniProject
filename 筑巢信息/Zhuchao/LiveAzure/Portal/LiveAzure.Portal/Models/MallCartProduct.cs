using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveAzure.Portal.Models
{
    public class MallCartProduct
    {
        //商品Gid
        public Guid mallCartGid { get; set; }

        //商品图片
        public string productPicture { get; set; }

        //商品名称
        public string productName { get; set; }

        //商品单价
        public decimal productSalePrice { get; set; }

        //商品的数量，转换前的数量
        public decimal productCount { get; set; }

        //商品转换后的标准计量单位下数量
        public decimal productQuantity { get; set; }

        //商品转换后的默认计量单位的数量
        public decimal productFactCount { get; set; }

        //商品的折扣
        public decimal productDiscount { get; set; }

        //商品价格的小计
        public decimal productPriceSum { get; set; }

        //Pu-SKU模式还是Pu-Parts模式
        public byte productMode { get; set; }

        //套装模式下的数量
        public decimal productSetCount { get; set; }

        //默认计量单位
        public string defaultUnit { get; set; }

        //标准计量单位
        public string standardUnit { get; set; }

    }
}