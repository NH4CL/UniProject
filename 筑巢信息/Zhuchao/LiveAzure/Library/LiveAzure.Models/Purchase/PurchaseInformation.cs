using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;
using LiveAzure.Models.General;
using LiveAzure.Models.Warehouse;

namespace LiveAzure.Models.Purchase
{
    /// <summary>
    /// 采购单
    /// </summary>
    /// <see cref="PurchaseItem"/>
    /// <see cref="PurchaseSupplier"/>
    /// <see cref="MemberOrganization"/>
    public class PurchaseInformation : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，组织ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Purchase.PurchaseInformation),
            ErrorMessageResourceName = "OrgIDRequired")]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 主键表ID,仓库ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Purchase.PurchaseInformation),
            ErrorMessageResourceName = "WhIDRequired")]
        public Guid WhID { get; set; }

        /// <summary>
        /// 采购单号，组织内唯一，由触发器自动生成
        /// </summary>
        //[Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Purchase.PurchaseInformation),
        //    ErrorMessageResourceName = "CodeRequired")]
        //[StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Purchase.PurchaseInformation),
        //    ErrorMessageResourceName = "CodeLong")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Code { get; set; }

        /// <summary>
        /// 状态 0未确定 1已确认 2已结算
        /// </summary>
        /// <see cref="ModelEnum.PurchaseStatus"/>
        public byte Pstatus { get; set; }

        /// <summary>
        /// 锁定状态 0解锁 1锁定 最后修改人即锁定人，锁定后其他人不能修改，管理员可强制解锁
        /// </summary>
        /// <see cref="ModelEnum.LockStatus"/>
        public byte Locking { get; set; }

        /// <summary>
        /// 挂起状态 0未挂起 1挂起（问题单） 2变更过程中，页面按钮修订时产生新版本，并设置变更过程中，在修订完成之前不能做任何动作
        /// </summary>
        /// <see cref="ModelEnum.HangStatus"/>
        public byte Hanged { get; set; }

        /// <summary>
        /// 主键表ID，供应商ID
        /// </summary>
        [Required]
        [Column("Supplier")]
        public Guid? aSupplier { get; set; }

        /// <summary>
        /// 主键表ID，采购类型ID，包括工厂采购，代理商采购，供应商主动补货等
        /// </summary>
        public Guid? Ptype { get; set; }

        /// <summary>
        /// 批次号，由触发器自动生成
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int TrackLot { get; set; }

        /// <summary>
        /// 当前版本
        /// </summary>
        public int DocVersion { get; set; }

        /// <summary>
        /// 折扣信息
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Discount { get; set; }

        /// <summary>
        /// 主键表ID，采购货币
        /// </summary>
        [Column("Currency")]
        public Guid? aCurrency { get; set; }

        /// <summary>
        /// 金额计算方式 0从总价计算单价 1从单价计算总价
        /// </summary>
        /// <see cref="ModelEnum.CalculateMode"/>
        public byte CalcMode { get; set; }

        /// <summary>
        /// 总数量，数量简单相加
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// 总金额，金额简单相加
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 总税额
        /// </summary>
        [Column(TypeName = "money")]
        public decimal TaxFee { get; set; }

        /// <summary>
        /// 总运费
        /// </summary>
        [Column(TypeName = "money")]
        public decimal ShipFee { get; set; }

        /// <summary>
        /// 已支付给供应商金额
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Paid { get; set; }

        /// <summary>
        /// 预计发货日期
        /// </summary>
        public DateTimeOffset? etd { get; set; }

        /// <summary>
        /// 实际发货日期
        /// </summary>
        public DateTimeOffset? atd { get; set; }

        /// <summary>
        /// 预计到达日期
        /// </summary>
        public DateTimeOffset? eta { get; set; }

        /// <summary>
        /// 实际到达日期
        /// </summary>
        public DateTimeOffset? ata { get; set; }

        /// <summary>
        /// 中间港预计到达日期
        /// </summary>
        public DateTimeOffset? PortDate { get; set; }

        /// <summary>
        /// 简单描述
        /// </summary>
        [StringLength(512, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Purchase.PurchaseInformation),
            ErrorMessageResourceName = "BriefLong")]
        public string Brief { get; set; }

        /// <summary>
        /// 组织主键表
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }

        /// <summary>
        /// 仓库主键表
        /// </summary>
        [ForeignKey("WhID")]
        public virtual WarehouseInformation Warehouse { get; set; }

        /// <summary>
        /// 供应商主键表
        /// </summary>
        [ForeignKey("aSupplier")]
        public virtual PurchaseSupplier Supplier { get; set; }

        /// <summary>
        /// 采购类型主键表
        /// </summary>
        [ForeignKey("Ptype")]
        public virtual GeneralPrivateCategory PurchaseType { get; set; }

        /// <summary>
        /// 采购货币主键表
        /// </summary>
        [ForeignKey("aCurrency")]
        public virtual GeneralMeasureUnit Currency { get; set; }

        /// <summary>
        /// 从表内容，采购单明细
        /// </summary>
        [InverseProperty("Purchase")]
        public virtual ICollection<PurchaseItem> PurchaseItems { get; set; }

        /// <summary>
        /// 从表内容，采购单变更记录
        /// </summary>
        [InverseProperty("Purchase")]
        public virtual ICollection<PurchaseHistory> Histories { get; set; }

        /// <summary>
        /// 从表内容，质检表
        /// </summary>
        [InverseProperty("Purchase")]
        public virtual ICollection<PurchaseInspection> Inspections { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> PurchaseStatusList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.PurchaseStatus), this.Pstatus); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string PurchaseStatusName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.PurchaseStatus), this.Pstatus); }
        }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> LockStatusList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.LockStatus), this.Locking); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string LockStatusName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.LockStatus), this.Locking); }
        }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> HangStatusList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.HangStatus), this.Hanged); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string HangStatusName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.HangStatus), this.Hanged); }
        }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> CalcModeList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.CalculateMode), this.CalcMode); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string CalcModeName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.CalculateMode), this.CalcMode); }
        }
    }
}
