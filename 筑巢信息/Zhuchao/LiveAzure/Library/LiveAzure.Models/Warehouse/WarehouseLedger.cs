using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;
using LiveAzure.Models.Product;
using LiveAzure.Models.General;

namespace LiveAzure.Models.Warehouse
{
    /// <summary>
    /// 库存总账 按SKU统计值 = 入库 - 出库
    /// </summary>
    /// <see cref="WarehouseInformation"/>
    /// <see cref="ProductInfoItem"/>
    public class WarehouseLedger : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，仓库ID
        /// </summary>
        [Required (ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Warehouse.WarehouseLedger),
            ErrorMessageResourceName="WhIDRequired")]
        public Guid WhID { get; set; }

        /// <summary>
        /// 主键表ID，SKU ID
        /// </summary>
       [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseLedger),
            ErrorMessageResourceName = "SkuIDRequired")]
        public Guid SkuID { get; set; }

        /// <summary>
        /// 入库总数
        /// </summary>
        [Column(TypeName = "money")]
        public decimal InQty { get; set; }

        /// <summary>
        /// 出库总数,已发货
        /// </summary>
        [Column(TypeName = "money")]
        public decimal OutQty { get; set; }

        /// <summary>
        /// 实际库存数,未发货数 In - Out，由触发器自动计算
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(TypeName = "money")]
        public decimal RealQty { get; set; }

        /// <summary>
        /// 冻结库存
        /// </summary>
        [Column(TypeName = "money")]
        public decimal LockQty { get; set; }

        /// <summary>
        /// 可用销售库存数，由触发器自动计算
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(TypeName = "money")]
        public decimal CanSaleQty { get; set; }

        /// <summary>
        /// 可用现货库存，由触发器自动计算
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(TypeName = "money")]
        public decimal CanDelivery { get; set; }

        /// <summary>
        /// 已留货，未发货；所有有效订单，包括付款和未付款，发货后清零
        /// </summary>
        [Column(TypeName = "money")]
        public decimal TobeDelivery { get; set; }

        /// <summary>
        /// 已排单，未发货；所有已排单订单，发货后清零
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Arranged { get; set; }

        /// <summary>
        /// 预售库存
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Presale { get; set; }

        /// <summary>
        /// 在途库存数，采购单，且未入库
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Ontheway { get; set; }

        /// <summary>
        /// 安全库存，报警值
        /// </summary>
        [Column(TypeName = "money")]
        public decimal SafeQty { get; set; }

        /// <summary>
        /// 最大库存，爆仓值
        /// </summary>
        [Column(TypeName = "money")]
        public decimal MaxQty { get; set; }

        /// <summary>
        /// 平均成本，按采购货币分列
        /// </summary>
        public Guid? AvgCost { get; set; }

        /// <summary>
        /// 仓库主键表
        /// </summary>
        [ForeignKey("WhID")]
        public virtual WarehouseInformation Warehouse { get; set; }

        /// <summary>
        /// 主键表,Sku
        /// </summary>
        [ForeignKey("SkuID")]
        public virtual ProductInfoItem SkuItem { get; set; }

        /// <summary>
        /// 平均成本主键表
        /// </summary>
        [ForeignKey("AvgCost")]
        public virtual GeneralResource AverageCost { get; set; }
    }
}
