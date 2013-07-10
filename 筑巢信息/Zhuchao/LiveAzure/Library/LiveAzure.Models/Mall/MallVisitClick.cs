using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using LiveAzure.Models.Member;
using LiveAzure.Models.Product;
using LiveAzure.Models.General;

namespace LiveAzure.Models.Mall
{
    /// <summary>
    /// 商城页面访问，简单记录，用户行为分析
    /// </summary>
    public class MallVisitClick : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，组织ID
        /// </summary>
        public Guid? OrgID { get; set; }

        /// <summary>
        /// 主键表ID，渠道ID
        /// </summary>
        [Required]
        public Guid ChlID { get; set; }

        /// <summary>
        /// 主键ID,用户ID,可空表示游客
        /// </summary>
        public Guid? UserID { get; set; }

        /// <summary>
        /// 主键表ID，商品ID，可空
        /// </summary
        public Guid? OnSaleID { get; set; }

        /// <summary>
        /// 广告链接标识
        /// </summary>
        public Guid? AdvID { get; set; }

        /// <summary>
        /// 点击地址
        /// </summary>
        [StringLength(256)]
        public string ClickUrl { get; set; }

        /// <summary>
        /// 点击来源
        /// </summary>
        [StringLength(256)]
        public string PreUrl { get; set; }

        /// <summary>
        /// Session
        /// </summary>
        [Required]
        [StringLength(50)]
        public string SessionID { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        [StringLength(20)]
        public string IpAddress { get; set; }

        /// <summary>
        /// 当前语言
        /// </summary>
        public int Culture { get; set; }

        /// <summary>
        /// 语言的本地名称
        /// </summary>
        [NotMapped]
        public string CultureName
        {
            get
            {
                string strCultureName = "NONE";
                try
                {
                    CultureInfo oCulture = new CultureInfo(this.Culture);
                    strCultureName = oCulture.NativeName;
                }
                catch { }
                return strCultureName;
            }
        }

        /// <summary>
        /// 当前货币
        /// </summary>
        [Column("Currency")]
        public Guid? aCurrency { get; set; }

        /// <summary>
        /// 浏览器及其版本
        /// </summary>
        [StringLength(30)]
        public string Browser { get; set; }

        /// <summary>
        /// 操作系统
        /// </summary>
        [StringLength(30)]
        public string WebSystem { get; set; }

        /// <summary>
        /// 生成记录的计算机名（服务器）
        /// </summary>
        [StringLength(20)]
        public string Response { get; set; }

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
        /// 上架商品主键表
        /// </summary>
        [ForeignKey("OnSaleID")]
        public virtual ProductOnSale OnSale { get; set; }

        ///// <summary>
        ///// 广告主键表
        ///// </summary>
        //[ForeignKey("AdvID")]
        //public virtual UnionAdvertising Advertising { get; set; }

        /// <summary>
        /// 货币主键表
        /// </summary>
        [ForeignKey("aCurrency")]
        public virtual GeneralMeasureUnit Currency { get; set; }
    }
}
