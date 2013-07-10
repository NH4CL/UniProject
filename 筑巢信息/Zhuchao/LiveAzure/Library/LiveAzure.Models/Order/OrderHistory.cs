using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LiveAzure.Models.Order
{
    /// <summary>
    /// 订单变更表，复制订单的历史记录
    /// </summary>
    /// <see cref="OrderInformation"/>
    /// <see cref="OrderHisItem"/>
    public class OrderHistory : LiveAzure.Models.ModelBase
    {
        #region 构造函数，版本复制

        /// <summary>
        /// 构造函数
        /// </summary>
        public OrderHistory()
        {
        }

        /// <summary>
        /// 复制采购单成老版本
        /// </summary>
        public OrderHistory(OrderInformation oOrder)
        {
            // TODO
        }

        #endregion

        /// <summary>
        /// 主键表ID，订单ID
        /// </summary>
        [Required(ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Order.OrderHistory),
            ErrorMessageResourceName="OrderIDRequired")]
        public Guid OrderID { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public int DocVersion { get; set; }

        /// <summary>
        /// 变更原因（退货，修改等 xml定义）
        /// </summary>
        /// <see cref="ModelEnum.HistoryType"/>
        public byte Htype { get; set; }

        /// <summary>
        /// 变更原因描述
        /// </summary>
        [StringLength(256,ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Order.OrderHistory),
            ErrorMessageResourceName="ReasonLong")]
        public string Reason { get; set; }

        /// <summary>
        /// 关联退款单
        /// </summary>
        public Guid? RefRefund { get; set; }

        /// <summary>
        /// 关联入库单
        /// </summary>
        public Guid? RefStockIn { get; set; }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public Guid? WhID { get; set; }

        /// <summary>
        /// 外连单号
        /// </summary>
        [StringLength(50)]
        public string LinkCode { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        /// <see cref="ModelEnum.OrderStatus"/>
        public byte Ostatus { get; set; }

        /// <summary>
        /// 锁定状态
        /// </summary>
        /// <see cref="ModelEnum.LockStatus"/>
        public byte Locking { get; set; }

        /// <summary>
        /// 付款状态
        /// </summary>
        /// <see cref="ModelEnum.PayStatus"/>
        public byte PayStatus { get; set; }

        /// <summary>
        /// 挂起状态
        /// </summary>
        /// <see cref="ModelEnum.HangStatus"/>
        public byte Hanged { get; set; }

        /// <summary>
        /// 变更原因
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderHistory),
            ErrorMessageResourceName = "HangReasonLong")]
        public string HangReason { get; set; }

        /// <summary>
        /// 自动解挂时间
        /// </summary>
        public DateTimeOffset? ReleaseTime { get; set; }

        /// <summary>
        /// 交易类型
        /// </summary>
        /// <see cref="ModelEnum.TransType"/>
        public byte TransType { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public Guid? PayID { get; set; }

        /// <summary>
        /// 付款备注，例如支付宝流水号
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderHistory),
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
        public Guid? Currency { get; set; }

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
        [Column(TypeName = "money")]
        public decimal TotalPaid { get; set; }

        /// <summary>
        /// 应付总计费用
        /// </summary>
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
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderHistory),
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
        /// 排单时间，即生成出库时间
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
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderHistory),
            ErrorMessageResourceName = "ConsigneeLong")]
        public string Consignee { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public Guid? Location { get; set; }

        /// <summary>
        /// 详细配送地址，如选择已有地址，则复制
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderHistory),
            ErrorMessageResourceName = "FullAddressLong")]
        public string FullAddress { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderHistory),
            ErrorMessageResourceName = "PostCodeLong")]
        public string PostCode { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderHistory),
            ErrorMessageResourceName = "TelephoneLong")]
        public string Telephone { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderHistory),
            ErrorMessageResourceName = "MobileLong")]
        public string Mobile { get; set; }

        /// <summary>
        /// 邮件地址
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderHistory),
            ErrorMessageResourceName = "EmailLong")]
        public string Email { get; set; }

        /// <summary>
        /// 地址有误
        /// </summary>
        public bool ErrorAddress { get; set; }

        /// <summary>
        /// 最佳送货时间说明
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderHistory),
            ErrorMessageResourceName = "BestDeliveryLong")]
        public string BestDelivery { get; set; }

        /// <summary>
        /// 收货人地址的标志性建筑
        /// </summary>
        [StringLength(128, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderHistory),
            ErrorMessageResourceName = "BuildingSignLong")]
        public string BuildingSign { get; set; }

        /// <summary>
        /// 订单留言，由客户提交订单时填写
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderHistory),
            ErrorMessageResourceName = "PostCommentLong")]
        public string PostComment { get; set; }

        /// <summary>
        /// 商家给客户的留言
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderHistory),
            ErrorMessageResourceName = "LeaveWordLong")]
        public string LeaveWord { get; set; }

        /// <summary>
        /// 下单的IP地址
        /// </summary>
        [StringLength(20, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Order.OrderHistory),
            ErrorMessageResourceName = "IpAddressLong")]
        public string IpAddress { get; set; }

        /// <summary>
        /// 广告ID，用于CPS等
        /// </summary>
        public Guid? AdvID { get; set; }

        /// <summary>
        /// 订单主键表
        /// </summary>
        [ForeignKey("OrderID")]
        public virtual OrderInformation Order { get; set; }

        /// <summary>
        /// 从表内容，订单变更商品表
        /// </summary>
        [InverseProperty("OrderHistory")]
        public virtual ICollection<OrderHisItem> OrderHisItems { get; set; }
    }
}
