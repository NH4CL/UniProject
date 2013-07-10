using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;
using LiveAzure.Models.General;


namespace LiveAzure.Models.Warehouse
{
    /// <summary>
    /// 入库单
    /// </summary>
    /// <see cref="WarehouseInformation"/>
    /// <see cref="WarehouseInItem"/>
    public class WarehouseStockIn : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，仓库ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseStockIn),
            ErrorMessageResourceName = "WhIDRequired")]
        public Guid WhID { get; set; }

        /// <summary>
        /// 入库单号，可自动生成
        /// </summary>
        //[Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseStockIn),
        //     ErrorMessageResourceName = "CodeRequired")]
        //[StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseStockIn),
        //    ErrorMessageResourceName = "CodeLong")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Code { get; set; }

        /// <summary>
        /// 状态，0未确认 1已确认
        /// </summary>
        /// <see cref="ModelEnum.InStatus"/>
        public byte Istatus { get; set; }

        /// <summary>
        /// 入库类型，大货、退货、盘盈、调整入库，移入库
        /// </summary>
        public Guid? InType { get; set; }

        /// <summary>
        /// 单据类型
        /// </summary>
        /// <see cref="ModelEnum.NoteType"/>
        public byte RefType { get; set; }

        /// <summary>
        /// 关联单据，采购单或销售退单的订单号，可空
        /// </summary>
        public Guid? RefID { get; set; }

        /// <summary>
        /// 总数量，简单相加
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Total { get; set; }

        /// <summary>
        /// 入库单打印次数，0表示未打印
        /// </summary>
        public int PrintInSheet { get; set; }

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
        /// 入库类型主键表
        /// </summary>
        [ForeignKey("InType")]
        public virtual GeneralStandardCategory StockInType { get; set; }

        /// <summary>
        /// 从表内容，入库单明细
        /// </summary>
        [InverseProperty("StockIn")]
        public virtual ICollection<WarehouseInItem> StockInItems { get; set; }

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
        public List<ListItem> InStatusList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.StockInStatus), this.Istatus); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string InStatusName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.StockInStatus), this.Istatus); }
        }
    }
}
