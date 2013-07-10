using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using LiveAzure.BLL;
using LiveAzure.Models;
using LiveAzure.Utility;

namespace LiveAzure.Daemon
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-10-24         -->
    /// <summary>
    /// 守护进程
    /// </summary>
    public partial class StageDaemon : ServiceBase
    {

        private int nDelayTime = 10;                       // 延迟启动
        private enum LiveSemaphore { GREED, RED };         // 信号灯
        private LiveSemaphore oSemaphore;                  // 冲突标识

        public StageDaemon()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 服务启动时执行一次
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            StageTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            int nValue;
            if (Int32.TryParse(ConfigHelper.GlobalConst.GetSetting("DelayTime"), out nValue))
                nDelayTime = nValue;
            if (Int32.TryParse(ConfigHelper.GlobalConst.GetSetting("Interval"), out nValue))
                StageTimer.Interval = nValue * 1000;
            EventBLL oEventBLL = new EventBLL();
            oEventBLL.WriteEvent(String.Format("OnStart 服务启动，延迟{0}周期启动连接", nDelayTime), this.ToString());
            StageTimer.Enabled = true;
            oSemaphore = LiveSemaphore.GREED;              // 设置绿灯
        }

        protected override void OnStop()
        {
            EventBLL oEventBLL = new EventBLL();
            oEventBLL.WriteEvent("OnStop 服务停止", this.ToString());
        }

        /// <summary>
        /// 时间触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            if (oSemaphore == LiveSemaphore.RED)           // 红灯冲突，终止执行
                return;
            if (nDelayTime > 0)                            // 延迟一个周期
            {
                nDelayTime--;
                return;
            }

            // 开始执行事件
            oSemaphore = LiveSemaphore.RED;
            LiveEntities dbEntity = new LiveEntities(ConfigHelper.LiveConnection.Connection);
            EventBLL oEventBLL = new EventBLL(dbEntity);
            try
            {
                oEventBLL.WriteEvent("OnTimedEvent 执行事件", this.ToString());

                // 淘宝接口同步
                Section.TaobaoDaemon oTaobao = new Section.TaobaoDaemon(dbEntity, oEventBLL);
                oTaobao.Main();

                // 消息服务
                Section.MessageDaemon oMessage = new Section.MessageDaemon(dbEntity, oEventBLL);
                oMessage.Main();

                // 同步分布式数据库
                Section.SynchroDaemon oSynchro = new Section.SynchroDaemon(dbEntity, oEventBLL);
                oSynchro.Main();

                oEventBLL.WriteEvent("OnTimedEvent 执行结束", this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent("OnTimedEvent " + ex.Message,
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            finally
            {
                try
                {
                    dbEntity.Dispose();
                }
                catch { }
            }
            oSemaphore = LiveSemaphore.GREED;
        }

    }
}
