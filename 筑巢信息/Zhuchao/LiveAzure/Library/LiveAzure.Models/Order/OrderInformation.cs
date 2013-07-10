using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;
using LiveAzure.Models.Finance;
using LiveAzure.Models.General;
using LiveAzure.Models.Shipping;
using LiveAzure.Models.Warehouse;


namespace LiveAzure.Models.Order
{
    /// <summary>
    /// 订单表
    /// </summary>
    /// <see cref="OrderItem"/>
    /// <see cref="MemberOrganization"/>
    /// <see cref="MemberChannel"/>
    /// <see cref="MemberUser"/>
    /// <see cref="FinancePayType"/>
    /// <see cref="GeneralMeasureUnit"/>
    /// <see cref="GeneralRegion"/>
    /// <see cref="UnionAdvertising"/>
    /// <see cref="OrderHistory"/>
    public class OrderInformation : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public OrderInformation()
        {
            this.OrderItems = new List<OrderItem>();
        }

        /// <summary>
        /// 主键表ID，组织ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderInformation),
            ErrorMessageResourceName = "OrgIDRequired")]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 主键表ID，来源渠道ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderInformation),
            ErrorMessageResourceName = "ChlIDRequired")]
        public Guid ChlID { get; set; }

        /// <summary>
        /// 仓库ID，可空则在触发器中匹配仓库
        /// </summary>
        public Guid? WhID { get; set; }

        /// <summary>
        /// 主键表ID，用户ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderInformation),
            ErrorMessageResourceName = "UserIDRequired")]
        public Guid UserID { get; set; }

        /// <summary>
        /// 订单号，每个数据库有不同的前缀，全局唯一
        /// 由触发器自动生成
        /// </summary>
        //[Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderInformation),
        //    ErrorMessageResourceName = "CodeRequired")]
        //[StringLength(50,ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Order.OrderInformation),
        //    ErrorMessageResourceName="CodeLong")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Code { get; set; }

        /// <summary>
        /// 外连单号，例如淘宝单号，团购单号等
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderInformation),
            ErrorMessageResourceName = "LinkCodeLong")]
        public string LinkCode { get; set; }

        /// <summary>
        /// 订单状态，0待确认 1已确认 2已排单 3已发货 4已结算
        /// 取消订单，复制到历史记录后，set Deleted = 1
        /// </summary>
        /// <see cref="ModelEnum.OrderStatus"/>
        public byte Ostatus { get; set; }

        /// <summary>
        /// 锁定状态 0解锁 1锁定 最后修改人即锁定人，锁定后其他人不能修改，管理员可强制解锁
        /// </summary>
        /// <see cref="ModelEnum.LockStatus"/>
        public byte Locking { get; set; }

        /// <summary>
        /// 付款状态 0未付款 1付款中/货到付款 2通知收款 3已付款
        /// </summary>
        /// <see cref="ModelEnum.PayStatus"/>
        public byte PayStatus { get; set; }

        /// <summary>
        /// 挂起状态 0未挂起 1挂起（问题单） 2变更过程中，页面按钮修订时产生新版本，并设置变更过程中，在修订完成之前不能做任何动作
        /// </summary>
        /// <see cref="ModelEnum.HangStatus"/>
        public byte Hanged { get; set; }

        /// <summary>
        /// 变更原因，退款中，退货中，换货中，取消中等，一旦退货入库，即清除或移到History中
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderInformation),
            ErrorMessageResourceName = "HangReasonLong")]
        public string HangReason { get; set; }

        /// <summary>
        /// 自动解挂时间
        /// </summary>
        public DateTimeOffset? ReleaseTime { get; set; }

        /// <summary>
        /// 交易类型 0款到发货 1货到付款 2担保交易
        /// </summary>
        /// <see cref="ModelEnum.TransType"/>
        public byte TransType { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public int DocVersion { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public Guid? PayID { get; set; }

        /// <summary>
        /// 付款备注，例如支付宝流水号
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderInformation),
            ErrorMessageResourceName = "PayNoteLong")]
        public string PayNote { get; set; }

        /// <summary>
        /// 商品件数，简单相加
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Pieces { get; set; }

        /// <summary>
        /// 结算货币
        /// </summary>
        [Column("Currency")]
        public Guid? aCurrency { get; set; }

        /// <summary>
        /// 商品总金额
        /// </summary>
        [Column(TypeName = "money")]
        public decimal SaleAmount { get; set; }

        /// <summary>
        /// 商品执行价
        /// </summary>
        [Column(TypeName = "money")]
        public decimal ExecuteAmount { get; set; }

        /// <summary>
        /// 收取的运费，到自提点
        /// </summary>
        [Column(TypeName = "money")]
        public decimal ShippingFee { get; set; }

        /// <summary>
        /// 税
        /// </summary>
        [Column(TypeName = "money")]
        public decimal TaxFee { get; set; }

        /// <summary>
        /// 收取的保价费
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Insurance { get; set; }

        /// <summary>
        /// 收取的支付手续费
        /// </summary>
        [Column(TypeName = "money")]
        public decimal PaymentFee { get; set; }

        /// <summary>
        /// 收取的包装费
        /// </summary>
        [Column(TypeName = "money")]
        public decimal PackingFee { get; set; }

        /// <summary>
        /// 收取的到门费
        /// </summary>
        [Column(TypeName = "money")]
        public decimal ResidenceFee { get; set; }

        /// <summary>
        /// 收取的上楼费
        /// </summary>
        [Column(TypeName = "money")]
        public decimal LiftGateFee { get; set; }

        /// <summary>
        /// 收取的安装费
        /// </summary>
        [Column(TypeName = "money")]
        public decimal InstallFee { get; set; }

        /// <summary>
        /// 收取的其它费用
        /// </summary>
        [Column(TypeName = "money")]
        public decimal OtherFee { get; set; }

        /// <summary>
        /// 订单应付款
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(TypeName = "money")]
        public decimal TotalFee { get; set; }

        /// <summary>
        /// 使用积分
        /// </summary>
        public int UsePoint { get; set; }

        /// <summary>
        /// 积分抵扣金额
        /// </summary>
        [Column(TypeName = "money")]
        public decimal PointPay { get; set; }

        /// <summary>
        /// 券支付，允许同货币的现金余额、抵用券、现金券、CPS返点等混合支付
        /// </summary>
        [Column(TypeName = "money")]
        public decimal CouponPay { get; set; }

        /// <summary>
        /// 红包支付金额，主要用于淘宝和其他渠道
        /// </summary>
        [Column(TypeName = "money")]
        public decimal BounsPay { get; set; }

        /// <summary>
        /// 折扣抵消费用
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Discount { get; set; }

        /// <summary>
        /// 现金支付费用，可能是网上支付等
        /// </summary>
        [Column(TypeName = "money")]
        public decimal MoneyPaid { get; set; }

        /// <summary>
        /// 已支付的全部费用
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(TypeName = "money")]
        public decimal TotalPaid { get; set; }

        /// <summary>
        /// 应付总计费用
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(TypeName = "money")]
        public decimal OrderAmount { get; set; }

        /// <summary>
        /// 误差，合并和拆分可能价格不一致，产生小数点误差
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Differ { get; set; }

        /// <summary>
        /// 合并，原始订单ID列表，一次最多能合并15个订单，原始订单移到History中，guid,guid,...
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderInformation),
            ErrorMessageResourceName = "MergeFormLong")]
        public string MergeFrom { get; set; }

        /// <summary>
        /// 拆分，原始订单ID，原始订单移到History中
        /// </summary>
        public Guid? SplitFrom { get; set; }

        /// <summary>
        /// 获得积分，方案1：即MoneyPaid 方案2：按SKU给的积分
        /// </summary>
        public int GetPoint { get; set; }

        /// <summary>
        /// 确认时间，多次确认记录最后一次
        /// </summary>
        public DateTimeOffset? ConfirmTime { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTimeOffset? PaidTime { get; set; }

        /// <summary>
        /// 排单时间，即生成出库单时间
        /// </summary>
        public DateTimeOffset? ArrangeTime { get; set; }

        /// <summary>
        /// 通知收款时间
        /// </summary>
        public DateTimeOffset? NoticeTime { get; set; }

        /// <summary>
        /// 结算时间
        /// </summary>
        public DateTimeOffset? ClosedTime { get; set; }

        /// <summary>
        /// 收货人 LastName + FirstName
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderInformation),
            ErrorMessageResourceName = "ConsigneeLong")]
        public string Consignee { get; set; }

        /// <summary>
        /// 城市（第0/1/2级均可）
        /// </summary>
        [Column("Location")]
        public Guid? aLocation { get; set; }

        /// <summary>
        /// 详细配送地址，如选择已有地址，则复制
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderInformation),
            ErrorMessageResourceName = "FullAddressLong")]
        public string FullAddress { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderInformation),
            ErrorMessageResourceName = "PostCodeLong")]
        public string PostCode { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderInformation),
            ErrorMessageResourceName = "TelephoneLong")]
        public string Telephone { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderInformation),
            ErrorMessageResourceName = "MobileLong")]
        public string Mobile { get; set; }

        /// <summary>
        /// 邮件地址
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderInformation),
            ErrorMessageResourceName = "EmailLong")]
        public string Email { get; set; }

        /// <summary>
        /// 地址有误
        /// </summary>
        public bool ErrorAddress { get; set; }

        /// <summary>
        /// 最佳送货时间说明
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderInformation),
            ErrorMessageResourceName = "BestDeliveryLong")]
        public string BestDelivery { get; set; }

        /// <summary>
        /// 收货人地址的标志性建筑
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderInformation),
            ErrorMessageResourceName = "BuildingSignLong")]
        public string BuildingSign { get; set; }

        /// <summary>
        /// 订单留言，由客户提交订单时填写
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderInformation),
            ErrorMessageResourceName = "PostCommentLong")]
        public string PostComment { get; set; }

        /// <summary>
        /// 商家给客户的留言
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderInformation),
            ErrorMessageResourceName = "LeaveWordLong")]
        public string LeaveWord { get; set; }

        /// <summary>
        /// 下单的IP地址
        /// </summary>
        [StringLength(20, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderInformation),
            ErrorMessageResourceName = "IpAddressLong")]
        public string IpAddress { get; set; }

        /// <summary>
        /// 主键表ID,广告ID
        /// </summary>
        public Guid? AdvID { get; set; }

        /// <summary>
        /// 组织主键表
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }

        /// <summary>
        /// 渠道主键表
        /// </summary>
        [ForeignKey("ChlID")]
        public virtual MemberChannel Channel { get; set; }

        /// <summary>
        /// 用户主键表
        /// </summary>
        [ForeignKey("UserID")]
        public virtual MemberUser User { get; set; }

        /// <summary>
        /// 仓库主键表
        /// </summary>
        [ForeignKey("WhID")]
        public virtual WarehouseInformation Warehouse { get; set; }

        /// <summary>
        /// 支付方式主键表
        /// </summary>
        [ForeignKey("PayID")]
        public virtual FinancePayType PayType { get; set; }

        /// <summary>
        /// 主键表,结算货币
        /// </summary>
        [ForeignKey("aCurrency")]
        public virtual GeneralMeasureUnit Currency { get; set; }

        /// <summary>
        /// 城市主键表
        /// </summary>
        [ForeignKey("aLocation")]
        public virtual GeneralRegion Location { get; set; }

        ///// <summary>
        ///// 广告主键表
        ///// </summary>
        //[ForeignKey("AdvID")]
        //public virtual UnionAdvertising Advertising { get; set; }

        /// <summary>
        /// 从表内容,订单商品表
        /// </summary>
        [InverseProperty("Order")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }

        /// <summary>
        /// 从表内容,订单承运商表
        /// </summary>
        [InverseProperty("Order")]
        public virtual ICollection<OrderShipping> OrderShippings { get; set; }

        /// <summary>
        /// 从表内容,订单处理日志，可显示在页面个客户查看
        /// </summary>
        [InverseProperty("Order")]
        public virtual ICollection<OrderProcess> OrderProcesses { get; set; }

        /// <summary>
        /// 从表内容,订单自定义属性
        /// </summary>
        [InverseProperty("Order")]
        public virtual ICollection<OrderAttribute> OrderAttributes { get; set; }

        /// <summary>
        /// 从表内容,订单变更表
        /// </summary>
        [InverseProperty("Order")]
        public virtual ICollection<OrderHistory> OrderHistories { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> OrderStatusList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.OrderStatus), this.Ostatus); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string OrderStatusName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.OrderStatus), this.Ostatus); }
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
        public List<ListItem> PayStatusList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.PayStatus), this.PayStatus); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string PayStatusName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.PayStatus), this.PayStatus); }
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
        public List<ListItem> TransTypeList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.TransType), this.TransType); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string TransTypeName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.TransType), this.TransType); }
        }
    }
}
