using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LiveAzure.Models.General;

namespace LiveAzure.Models.Member
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-08         -->
    /// <summary>
    /// 用户订阅
    /// </summary>
    /// <see cref="MemberUser"/>
    public class MemberSubscribe : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 主键表ID，积分ID
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.Member.MemberSubscribe),
            ErrorMessageResourceName = "UserIDRequired")]
        public Guid UserID { get; set; }

        /// <summary>
        /// 短信订阅
        /// </summary>
        public bool ShortMessage { get; set; }
        
        /// <summary>
        /// 邮件订阅
        /// </summary>
        public bool Email { get; set; }
        
        /// <summary>
        /// 简报
        /// </summary>
        public bool Bulletin { get; set; }
        
        /// <summary>
        /// 订单确认提醒
        /// </summary>
        public bool OrderConfirm { get; set; }
        
        /// <summary>
        /// 订单发货提醒
        /// </summary>
        public bool OrderDelivery { get; set; }
        
        /// <summary>
        /// 缺货的到货通知
        /// </summary>
        public bool ShortSupply { get; set; }
        
        /// <summary>
        /// 促销信息
        /// </summary>
        public bool Promotion { get; set; }
        
        /// <summary>
        /// 打折信息
        /// </summary>
        public bool Discount { get; set; }
        
        /// <summary>
        /// 联盟获利通知
        /// </summary>
        public bool UnionNotice { get; set; }

        /// <summary>
        /// 用户主键表
        /// </summary>
        [ForeignKey("UserID")]
        public virtual MemberUser User { get; set; }
    }
}
