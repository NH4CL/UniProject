using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LiveAzure.Models.Synchro
{
    public class SynchroTimestamp : LiveAzure.Models.ModelBase
    {
        /// <summary>
        /// 表名
        /// </summary>
        [Required]
        [StringLength(255)]
        string TableName { get; set; }

        /// <summary>
        /// 最新行版本号
        /// </summary>
        byte[] NowStamp { get; set; }
    }
}
