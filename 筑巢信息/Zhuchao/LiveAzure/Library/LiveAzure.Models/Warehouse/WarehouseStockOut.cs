using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.General;
using LiveAzure.Models.Member;
using LiveAzure.Models.Shipping;


namespace LiveAzure.Models.Warehouse
{
    /// <summary>
    /// 出库记录 扫描出库，报废出库等
    /// 允许一键出库，则省略出库扫描步骤
    /// </summary>
    /// <see cref="WarehouseInformation"/>
    /// <see cref="ShippingInformation"/>
    /// <see cref="WarehouseOutItem"/>
    public class WarehouseStockOut : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，仓库ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseStockOut),
            ErrorMessageResourceName = "WhIDRequired")]
        public Guid WhID { get; set; }

        /// <summary>
        /// 出库单号，可自动生成
        /// </summary>
        //[Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseStockOut),
        //    ErrorMessageResourceName = "CodeRequired")]
        //[StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseStockOut),
        //    ErrorMessageResourceName = "CodeLong")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Code { get; set; }

        /// <summary>
        /// 状态，0未确认 1拣货/扫描中 2已发货 3已签收
        /// </summary>
        /// <see cref="ModelEnum.StockOutStatus"/>
        public byte Ostatus { get; set; }

        /// <summary>
        /// 出库类型，销售、报废、盘亏、调整出库，采购退货，移出库等
        /// </summary>
        public Guid? OutType { get; set; }

        /// <summary>
        /// 单据类型
        /// </summary>
        /// <see cref="ModelEnum.NoteType"/>
        public byte RefType { get; set; }

        /// <summary>
        /// 关联单据，订单号，原始采购单等，可空
        /// </summary>
        public Guid? RefID { get; set; }

        /// <summary>
        /// 承运商，可空，订单发货默认从订单表中复制，可修改，但需要把结果返回订单表
        /// </summary>
        public Guid? ShipID { get; set; }

        /// <summary>
        /// 拣货员/扫描员
        /// </summary>
        public Guid? TakeMan { get; set; }

        /// <summary>
        /// 分拣员/打包称重
        /// </summary>
        public Guid? SortMan { get; set; }

        /// <summary>
        /// 面单操作员/扫描面单号
        /// </summary>
        public Guid? SendMan { get; set; }

        /// <summary>
        /// 出库单打印次数，0表示未打印
        /// </summary>
        public int PrintOutSheet { get; set; }

        /// <summary>
        /// 面单打印次数，0表示未打印
        /// </summary>
        public int PrintEnvelope { get; set; }

        /// <summary>
        /// 总数量，简单相加
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Total { get; set; }

        /// <summary>
        /// 包裹数
        /// </summary>
        public int Package { get; set; }

        /// <summary>
        /// 制表
        /// </summary>
        public Guid? Prepared { get; set; }

        /// <summary>
        /// 确认
        /// </summary>
        public Guid? Approved { get; set; }

        /// <summary>
        /// 确认时间
        /// </summary>
        public DateTimeOffset? ApproveTime { get; set; }

        /// <summary>
        /// 仓库主键表
        /// </summary>
        [ForeignKey("WhID")]
        public virtual WarehouseInformation Warehouse { get; set; }

        /// <summary>
        /// 出库类型主键表
        /// </summary>
        [ForeignKey("OutType")]
        public virtual GeneralStandardCategory StockOutType { get; set; }

        /// <summary>
        /// 承运商主键表
        /// </summary>
        [ForeignKey("ShipID")]
        public virtual ShippingInformation Shipper { get; set; }

        /// <summary>
        /// 从表内容，出库记录明细
        /// </summary>
        [InverseProperty("StockOut")]
        public virtual ICollection<WarehouseOutItem> StockOutItems { get; set; }

        /// <summary>
        /// 从表内容，出库扫描记录
        /// </summary>
        [InverseProperty("StockOut")]
        public virtual ICollection<WarehouseOutScan> StockOutScans { get; set; }

        /// <summary>
        /// 从表内容，出库运输记录
        /// </summary>
        [InverseProperty("StockOut")]
        public virtual ICollection<WarehouseOutDelivery> StockOutDeliveries { get; set; }

        ///// <summary>
        ///// 制表人
        ///// </summary>
        //[InverseProperty("Prepared")]
        //public virtual MemberUser PreparedUser { get; set; }

        ///// <summary>
        ///// 确认人
        ///// </summary>
        //[InverseProperty("Approved")]
        //public virtual MemberUser ApprovedUser { get; set; }

        ///// <summary>
        ///// 拣货员/扫描员
        ///// </summary>
        //[InverseProperty("TakeMan")]
        //public virtual MemberUser TakeManUser { get; set; }

        ///// <summary>
        ///// 分拣员/打包称重
        ///// </summary>
        //[InverseProperty("SortMan")]
        //public virtual MemberUser SortManUser { get; set; }

        ///// <summary>
        ///// 面单操作员/扫描面单号
        ///// </summary>
        //[InverseProperty("SendMan")]
        //public virtual MemberUser SendManUser { get; set; }

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
        public List<ListItem> OutStatusList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.StockOutStatus), this.Ostatus); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string OutStatusName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.StockOutStatus), this.Ostatus); }
        }
    }
}
