using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;

namespace LiveAzure.Models.Purchase
{
    /// <summary>
    /// 质检表
    /// </summary>
    /// <see cref="MemberOrganization"/>
    /// <see cref="PurchaseInformation"/>
    /// <see cref="PurchaseInspItem"/>
    public class PurchaseInspection : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PurchaseInspection()
        {
            this.InspDate = DateTimeOffset.Now;
        }

        /// <summary>
        /// 主键表ID，组织ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Purchase.PurchaseInspection),
            ErrorMessageResourceName = "OrgIDRequired")]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 主键表ID，采购单号
        /// </summary>
        [Column("PurID")]
        public Guid? PurID { get; set; }

        /// <summary>
        /// 质检单号
        /// </summary>
        //[Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Purchase.PurchaseInspection),
        //    ErrorMessageResourceName = "CodeRequired")]
        //[StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Purchase.PurchaseInspection),
        //    ErrorMessageResourceName = "CodeLong")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Code { get; set; }

        /// <summary>
        /// 质检员
        /// </summary>
        [StringLength(50, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Purchase.PurchaseInspection),
            ErrorMessageResourceName = "InspectorLong")]
        public string Inspector { get; set; }

        /// <summary>
        /// 检验日期
        /// </summary>
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset? InspDate { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Total { get; set; }

        /// <summary>
        /// 是否通过
        /// </summary>
        public bool Passed { get; set; }

        /// <summary>
        /// 问题原因描述，默认语言
        /// </summary>
        [StringLength(512, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Purchase.PurchaseInspection),
            ErrorMessageResourceName = "BriefLong")]
        public string Brief { get; set; }

        /// <summary>
        /// 组织主键表
        /// </summary>
        [ForeignKey("OrgID")]
        public virtual MemberOrganization Organization { get; set; }

        /// <summary>
        /// 采购单主键表
        /// </summary>
        [ForeignKey("PurID")]
        public virtual PurchaseInformation Purchase { get; set; }

        /// <summary>
        /// 从表内容，质检明细
        /// </summary>
        [InverseProperty("Inspection")]
        public virtual ICollection<PurchaseInspItem> InspectionItems { get; set; }
    }
}
