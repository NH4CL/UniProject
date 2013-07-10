using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.Member;

namespace LiveAzure.Models.Warehouse
{
    /// <summary>
    /// 盘点记录
    /// </summary>
    /// <see cref="WarehouseInvItem"/>
    /// <see cref="MemberOrganization"/>
    /// <see cref="WarehouseInformation"/>
    public class WarehouseInventory : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键ID，组织ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseInventory),
            ErrorMessageResourceName = "OrgIDRequired")]
        public Guid OrgID { get; set; }

        /// <summary>
        /// 主键ID，仓库ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseInventory),
           ErrorMessageResourceName = "WhIDRequired")]
        public Guid WhID { get; set; }

        /// <summary>
        /// 代码，组织内唯一
        /// </summary>
        //[Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Warehouse.WarehouseInventory),
        //    ErrorMessageResourceName = "CodeRequired")]
        //[StringLength(50,ErrorMessageResourceType=typeof(LiveAzure.Resource.Model.Warehouse.WarehouseInventory),
        //    ErrorMessageResourceName="CodeLong")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Code { get; set; }

        /// <summary>
        /// 状态，0未确认 1已确认 2仓库快照
        /// </summary>
        /// <see cref="ModelEnum.InventoryStatus"/>
        public byte Istatus { get; set; }

        /// <summary>
        /// 数量，简单相加
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// 盘点人
        /// </summary>
        public Guid? Prepared { get; set; }

        /// <summary>
        /// 复核人
        /// </summary>
        public Guid? Approved { get; set; }

        /// <summary>
        /// 复核时间
        /// </summary>
        public DateTimeOffset? ApproveTime { get; set; }

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
        /// 盘点明细
        /// </summary>
        [InverseProperty("Inventory")]
        public virtual ICollection<WarehouseInvItem> InvItems { get; set; }

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
        public List<ListItem> InventoryStatusList
        {
            get { return base.SelectEnumList(typeof(ModelEnum.InventoryStatus), this.Istatus); }
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        [NotMapped]
        public string InventoryStatusName
        {
            get { return base.SelectEnumName(typeof(ModelEnum.InventoryStatus), this.Istatus); }
        }
    }
}
