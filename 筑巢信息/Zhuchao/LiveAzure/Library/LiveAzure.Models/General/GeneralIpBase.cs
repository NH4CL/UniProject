using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LiveAzure.Models.General
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-07         -->
    /// <summary>
    /// IP地址库，仅适用中国
    /// </summary>
    public class GeneralIpBase : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 起始地址
        /// </summary>
        public long IpFrom { get; set; }

        /// <summary>
        /// 终止地址
        /// </summary>
        public long IpTo { get; set; }

        /// <summary>
        /// 国家
        /// </summary>
        [StringLength(512)]
        public string Country { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        [StringLength(128)]
        public string Province { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        [StringLength(256)]
        public string City { get; set; }

        /// <summary>
        /// ISP
        /// </summary>
        [StringLength(256)]
        public string Isp { get; set; }

        /// <summary>
        /// 源
        /// </summary>
        [StringLength(512)]
        public string Source { get; set; }
    }
}
