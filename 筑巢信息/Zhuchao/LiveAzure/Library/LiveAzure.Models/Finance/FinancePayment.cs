using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;
using LiveAzure.Models.General;

namespace LiveAzure.Models.Finance
{
    /// <summary>
    /// 应付账
    /// </summary>
    public class FinancePayment : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public FinancePayment()
        {
            this.PayDate = DateTimeOffset.Now;
        }

        /// <summary>
        /// 主键表ID，所属组织
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Finance.FinancePayment),
            ErrorMessageResourceName = "OrgIDRequired")]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 代码，组织内唯一
        /// </summary>
        //[Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Finance.FinancePayment),
        //    ErrorMessageResourceName = "CodeRequired")]
        //[StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Finance.FinancePayment),
        //    ErrorMessageResourceName = "CodeLong")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Code { get; set; }

        /// <summary>
        /// 原因
        /// </summary>
        public Guid? Ptype { get; set; }

        /// <summary>
        /// 方向 0到积分 1到现金 2到个人银行帐 2到公司银行账
        /// </summary>
        /// <see cref="ModelEnum.PayDirection"/>
        public byte PayTo { get; set; }

        /// <summary>
        /// 状态 0待付 1已退 2已结算
        /// </summary>
        /// <see cref="ModelEnum.FinanceStatus"/>
        public byte Pstatus { get; set; }

        /// <summary>
        /// 单据类型
        /// </summary>
        /// <see cref="ModelEnum.NoteType"/>
        public byte RefType { get; set; }

        /// <summary>
        /// 订单号，采购单号, 积分ID
        /// </summary>
        public Guid RefID { get; set; }

        /// <summary>
        /// 原因说明，银行帐号等
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Finance.FinancePayment),
            ErrorMessageResourceName = "ReasonLong")]
        public string Reason { get; set; }

        /// <summary>
        /// 支付日期
        /// </summary>
        public DateTimeOffset? PayDate { get; set; }

        /// <summary>
        /// 货币
        /// </summary>
        [Column("Currency")]
        public Guid? aCurrency { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 组织主键表
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }

        /// <summary>
        /// 支付类型主键表
        /// </summary>
        [ForeignKey("Ptype")]
        public virtual GeneralPrivateCategory PaymentType { get; set; }

        /// <summary>
        /// 货币主键表
        /// </summary>
        [ForeignKey("aCurrency")]
        public virtual GeneralMeasureUnit Currency { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> RefTypeList
        {
            get
            {
                return base.SelectEnumList(typeof(ModelEnum.NoteType), this.RefType);
            }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string RefTypeName
        {
            get
            {
                return base.SelectEnumName(typeof(ModelEnum.NoteType), this.RefType);
            }
        }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> PayToList
        {
            get
            {
                return base.SelectEnumList(typeof(ModelEnum.PayDirection), this.PayTo);
            }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string PayToName
        {
            get
            {
                return base.SelectEnumName(typeof(ModelEnum.PayDirection), this.PayTo);
            }
        }

        /// <summary>
        /// 获取枚举类型的列表值，用于MVC WEB页面
        /// </summary>
        [NotMapped]
        public List<ListItem> PayStatusList
        {
            get
            {
                return base.SelectEnumList(typeof(ModelEnum.FinanceStatus), this.Pstatus);
            }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string PayStatusName
        {
            get
            {
                return base.SelectEnumName(typeof(ModelEnum.FinanceStatus), this.Pstatus);
            }
        }
    }
}
