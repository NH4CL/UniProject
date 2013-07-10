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
    /// 消息服务守护进程
    /// </summary>
    public class MessageDaemon : DaemonBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="entity">数据库连接</param>
        /// <param name="eventBLL">事件记录器</param>
        public MessageDaemon(LiveEntities entity, EventBLL eventBLL) : base(entity, eventBLL) { }

        /// <summary>
        /// 进程主程序
        /// </summary>
        public void Main()
        {
            MessageBLL oMessageBLL = new MessageBLL(this.dbEntity);

            // 任何时间发送邮件
            oMessageBLL.SendPendingMail();

            // 仅在早上9点到晚上8点发送手机短信
            int nMorning = 9;
            int nNignt = 20;
            try
            {
                string[] sTimer = Utility.ConfigHelper.GlobalConst.GetSetting("SmsWorkTime").Split(',');
                Int32.TryParse(sTimer[0], out nMorning);
                Int32.TryParse(sTimer[1], out nNignt);
            }
            catch { }
            int nHour = DateTime.Now.Hour;
            if (nHour >= nMorning && nHour < nNignt)
                oMessageBLL.SendPendingSms();

            // 接收手机短信
            oMessageBLL.ReceiveSms();
        }
    }
}
