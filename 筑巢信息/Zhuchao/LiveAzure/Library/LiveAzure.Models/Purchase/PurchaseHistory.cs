using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LiveAzure.Models.Purchase
{
    /// <summary>
    /// 采购单变更记录
    /// </summary>
    /// <see cref="PurchaseInformation"/>
    /// <see cref="PurchaseHisItem"/>
    public class PurchaseHistory : LiveAzure.Models.ModelBase
    {
        #region 构造函数，版本复制

        /// <summary>
        /// 构造函数
        /// </summary>
        public PurchaseHistory()
        {
        }

        /// <summary>
        /// 复制采购单成老版本
        /// </summary>
        public PurchaseHistory(PurchaseInformation oPurchase)
        {
            // TODO
        }

        #endregion

        /// <summary>
        /// 主键表ID，原始单据号
        /// </summary>
        [Required(ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Purchase.PurchaseHistory),
            ErrorMessageResourceName="PurIDRequired")]
        public Guid PurID { get; set; }

        /// <summary>
        /// 历史版本
        /// </summary>
        [Required(ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Purchase.PurchaseHistory),
            ErrorMessageResourceName="DocVersionRequired")]
        public int DocVersion { get; set; }

        /// <summary>
        /// 变更原因，用xml显示下拉菜单
        /// </summary>
        /// <see cref="ModelEnum.HistoryType"/>
        public byte Htype { get; set; }

        /// <summary>
        /// 变更原因描述
        /// </summary>
        [StringLength(256,ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Purchase.PurchaseHistory),
            ErrorMessageResourceName="ReasonLong")]
        public string Reason { get; set; }

        /// <summary>
        /// 仓库
        /// </summary>
        public Guid? WhID { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public byte Pstatus { get; set; }

        /// <summary>
        /// 锁定状态 0解锁 1锁定 最后修改人即锁定人，锁定后其他人不能修改，管理员可强制解锁
        /// </summary>
        /// <see cref="ModelEnum.LockStatus"/>
        public byte Locking { get; set; }

        /// <summary>
        /// 挂起状态 0未挂起 1挂起
        /// </summary>
        public byte Hanged { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public Guid? Supplier { get; set; }

        /// <summary>
        /// 采购类型
        /// </summary>
        public Guid? Ptype { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public int TrackLot { get; set; }

        /// <summary>
        /// 折扣信息
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Discount { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// 总金额
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
        /// 已付款
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
        [StringLength(512,ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Purchase.PurchaseHistory),
            ErrorMessageResourceName="BriefLong")]
        public string Brief { get; set; }

        /// <summary>
        /// 原始单据主键表
        /// </summary>
        [ForeignKey("PurID")]
        public virtual PurchaseInformation Purchase { get; set; }

        /// <summary>
        /// 从表内容，采购单明细，老版本
        /// </summary>
        [InverseProperty("History")]
        public virtual ICollection<PurchaseHisItem> HistoryItems { get; set; }
    }
}
