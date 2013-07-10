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
    /// 服务进程基类
    /// </summary>
    public class DaemonBase
    {
        public LiveEntities dbEntity;                // 数据库连接
        public EventBLL oEventBLL;                   // 事件记录工具

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="entity">数据库连接</param>
        /// <param name="eventBLL">事件记录器</param>
        public DaemonBase(LiveEntities entity, EventBLL eventBLL)
        {
            this.dbEntity = entity;
            this.oEventBLL = eventBLL;
        }
    }
}
