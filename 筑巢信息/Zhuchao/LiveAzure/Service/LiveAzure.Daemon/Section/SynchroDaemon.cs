using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveAzure.Models;
using LiveAzure.BLL;

namespace LiveAzure.Daemon.Section
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-10-24         -->
    /// <summary>
    /// 分布式数据库，系统同步进程
    /// </summary>
    public class SynchroDaemon : DaemonBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="entity">数据库连接</param>
        /// <param name="eventBLL">事件记录器</param>
        public SynchroDaemon(LiveEntities entity, EventBLL eventBLL) : base(entity, eventBLL) { }

        public void Main()
        {
        }
    }
}
