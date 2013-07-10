using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace LiveAzure.Models.Synchro
{
    /// <summary>
    /// 系统同步表的数据访问层
    /// </summary>
    public class SynchroEntities : DbContext
    {
        public DbSet<SynchroTimestamp> SynchroTimestamps { get; set; }
    }
}
