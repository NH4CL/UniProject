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
    /// 移库单/移货位单
    /// </summary>
    /// <see cref="WarehouseMoveItem"/>
    /// <see cref="MemberOrganization"/>
    /// <see cref="WarehouseInformation"/>
    public class WarehouseMoving:LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键ID，组织ID
        /// </summary>
        [Required(ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Warehouse.WarehouseMoving),
            ErrorMessageResourceName="OrgIDRequired")]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 移库单号，组织内唯一
        /// </summary>
        //[Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseMoving),
        //    ErrorMessageResourceName ="CodeRequired")]
        //[StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseMoving),
        //    ErrorMessageResourceName = "CodeLong")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Code { get; set; }

        /// <summary>
        /// 状态，0未确认 1已确认
        /// </summary>
        /// <see cref="ModelEnum.MovingStatus"/>
        public byte Mstatus { get; set; }

        /// <summary>
        /// 移库类型，0移库 1移货位
        /// </summary>
        /// <see cref="ModelEnum.MovingType"/>
        public byte Mtype { get; set; }

        /// <summary>
        /// 主键表ID，原仓库
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseMoving),
            ErrorMessageResourceName = "OldWhIDRequired")]
        public Guid OldWhID { get; set; }

        /// <summary>
        /// 主键表ID，新仓库
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseMoving),
            ErrorMessageResourceName = "NewWhIDRequired")]
        public Guid NewWhID { get; set; }

        /// <summary>
        /// 原因描述，默认语言
        /// </summary>
        [StringLength(256,ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseMoving),
            ErrorMessageResourceName ="ReasonLong")]
        public string Reason { get; set; }

        /// <summary>
        /// 总数量，简单相加
        /// </summary>
        [Column(TypeName="money")]
        public decimal Total { get; set; }

        /// <summary>
        /// 承运商，可空
        /// </summary>
        public Guid? ShipID { get; set; }

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
        /// 组织主键表
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }

        /// <summary>
        /// 主键表，原仓库
        /// </summary>
        [ForeignKey("OldWhID")]
        public virtual WarehouseInformation OldWarehouse { get; set; }

        /// <summary>
        /// 主键表，新仓库
        /// </summary>
        [ForeignKey("NewWhID")]
        public virtual WarehouseInformation NewWarehouse { get; set; }
        /// <!--Angel-->
        /// <summary>
        /// 从表内容，移库明细
        /// </summary>
        [InverseProperty("Moving")]
        public virtual ICollection<WarehouseMoveItem> MoveItems { get; set; }
        /// <summary>
        /// 主键表，承运商
        /// </summary>
        [ForeignKey("ShipID")]
        public virtual ShippingInformation Shipper { get; set; }

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
        public List<ListItem> MoveStatusList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.MovingStatus), this.Mstatus); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string MoveStatusName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.MovingStatus), this.Mstatus); }
        }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> MoveTypeList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.MovingType), this.Mtype); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string MoveTypeName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.MovingType), this.Mtype); }
        }
    }
}
