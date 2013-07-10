using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using LiveAzure.Utility;
using LiveAzure.Models;
using LiveAzure.Models.General;
using LiveAzure.Models.Order;

namespace LiveAzure.BLL
{
    /// <summary>
    /// 日志处理类
    /// </summary>
    public class EventBLL : IDisposable
    {
        private LiveEntities dbEntity;                     // 数据库连接
        private string strLogFile;                         // 默认日志文件
        private bool bIsDebug;                             // 是否调试状态

        /// <summary>
        /// 构造函数，无数据库连接，日志不写入数据库
        /// </summary>
        public EventBLL()
        {
            this.dbEntity = null;
            SetEventFile();
        }

        /// <summary>
        /// 构造函数，从调用处传数据库连接参数
        /// </summary>
        /// <param name="entity">数据库连接实体</param>
        public EventBLL(LiveEntities entity)
        {
            this.dbEntity = entity;
            SetEventFile();
        }

        private void SetEventFile()
        {
            try
            {
                string sPrefix = Utility.ConfigHelper.GlobalConst.GetSetting("LogPrefix");
                string sIsDebug = Utility.ConfigHelper.GlobalConst.GetSetting("IsDebug");
                if (String.IsNullOrEmpty(sPrefix)) sPrefix = @"C:\Temp\stage";
                if (String.IsNullOrEmpty(sIsDebug)) sIsDebug = "false";
                strLogFile = sPrefix + DateTimeOffset.Now.ToString("yyyyMM") + @".log";
                bIsDebug = Boolean.Parse(sIsDebug);
            }
            catch
            {
                strLogFile = Path.GetTempPath() + @"stage" + DateTimeOffset.Now.ToString("yyyyMM") + @".log";
                bIsDebug = false;
            }
        }

        #region 析构
        /// <summary>
        /// 析构函数，调用虚拟的Dispose方法 
        /// </summary>
        ~EventBLL()
        {
            Dispose(false);
        }

        /// <summary>
        /// 调用虚拟的Dispose方法。禁止Finalization（终结操作） 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
        #endregion

        #region GeneralAction日志处理
        /// <summary>
        /// 记录普通日志到文件中
        /// </summary>
        /// <param name="msg">日志内容（中文）</param>
        public void WriteEvent(string msg)
        {
            WriteEvent(msg, ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, "", null, 0, null);
        }

        /// <summary>
        /// 记录普通日志到文件中
        /// </summary>
        /// <param name="msg">日志内容（中文）</param>
        /// <param name="classname">控制器的名称</param>
        /// <param name="userID">用户名，登陆名，GUID均可，可空</param>
        public void WriteEvent(string msg, string classname, Guid? userID = null)
        {
            WriteEvent(msg, ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, classname, userID, 0, null);
        }

        /// <summary>
        /// 普通日志写入文件
        /// </summary>
        /// <param name="msg">日志内容（中文）</param>
        /// <param name="level">日志级别</param>
        /// <param name="source">日志来源，模块标识</param>
        /// <param name="classname">类名称</param>
        /// <param name="userID">用户GUID</param>
        public void WriteEvent(string msg, ModelEnum.ActionLevel level, ModelEnum.ActionSource source, string classname, Guid? userID = null)
        {
            WriteEvent(msg, level, source, classname, userID, 0, null);
        }

        /// <summary>
        /// 根据记录级别，日志写到文件或数据库中
        /// </summary>
        /// <param name="msg">日志内容（中文）</param>
        /// <param name="level">日志级别</param>
        /// <param name="source">日志来源，模块标识</param>
        /// <param name="classname">类名称</param>
        /// <param name="userID">用户GUID</param>
        /// <param name="refType">关联单据类型，0订单号</param>
        /// <param name="refID">关联单据号</param>
        public void WriteEvent(string msg, ModelEnum.ActionLevel level, ModelEnum.ActionSource source,
            string classname, Guid? userID, ModelEnum.NoteType? refType, Guid? refID = null)
        {
            WriteEvent(msg, level, source, classname, userID, refType, refID, false);
        }

        /// <summary>
        /// 根据记录级别，日志写到文件或数据库中
        /// </summary>
        /// <param name="msg">日志内容（中文）</param>
        /// <param name="level">日志级别</param>
        /// <param name="source">日志来源，模块标识</param>
        /// <param name="classname">类名称</param>
        /// <param name="userID">用户GUID</param>
        /// <param name="refType">关联单据类型，0订单号</param>
        /// <param name="refID">关联单据号</param>
        /// <param name="bForceToDb">将日期强制写入数据库</param>
        public void WriteEvent(string msg, ModelEnum.ActionLevel level, ModelEnum.ActionSource source,
            string classname, Guid? userID, ModelEnum.NoteType? refType, Guid? refID, bool bForceToDb = false)
        {
            string sMessage = "";
            string sRefMsg;
            string sLevelMsg = "[INFO],[WARN],[ERROR]";
            string[] sLevelList = sLevelMsg.Split(',');
            byte nLevel = (byte)level;
            if (nLevel < sLevelList.Length)
                sMessage = sLevelList[nLevel];
            try
            {
                sRefMsg = (refID == null) ? "" : String.Format("[0][1]", refType.ToString(), refID);
                sMessage += String.Format("[{0}][{1}][{2}][{3}][{4}][{5}]", DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    source.ToString(), classname, userID, sRefMsg, msg);
                StreamWriter oLogWriter = new StreamWriter(strLogFile, true);
                oLogWriter.WriteLine(sMessage);
                oLogWriter.Close();
                oLogWriter.Dispose();
                // 严重错误，写入数据库
                if ((bForceToDb) || ((level == ModelEnum.ActionLevel.ERROR) && (dbEntity != null)))
                {
                    GeneralAction oAction = new GeneralAction { Grade = (byte)level, Source = (byte)source, ClassName = classname, UserID = userID, RefType = (byte)refType, RefID = refID, Matter = msg };
                    dbEntity.GeneralActions.Add(oAction);
                    dbEntity.SaveChanges();
                }
                if (bIsDebug) Debug.WriteLine("{0}, {1}", this.ToString(), sMessage);
            }
            catch (Exception ex)
            {
                try
                {
                    sMessage = String.Format("[ERROR][{0}][{1}][{2}][{3}][][{4}]", DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        ModelEnum.ActionSource.SYSTEM, this.ToString(), userID, ex.Message);
                    StreamWriter oLogWriter = new StreamWriter(strLogFile, true);
                    oLogWriter.WriteLine(sMessage);
                    oLogWriter.Close();
                    oLogWriter.Dispose();
                    if (bIsDebug) Debug.WriteLine("{0}, {1}", this.ToString(), sMessage);
                }
                catch { }
            }
        }
        #endregion

        /// <summary>
        /// 待办事项日志
        /// </summary>
        /// <param name="msg">事项内容</param>
        /// <param name="todo">类别</param>
        /// <param name="jumpUrl">跳转地址</param>
        public void WriteTodoEvent(string msg, ModelEnum.TodoType todo, string jumpUrl)
        {
            try
            {
                if (dbEntity != null)
                {
                    GeneralTodoList oTodo = new GeneralTodoList { Ttype = (byte)todo, Title = msg, JumpUrl = jumpUrl };
                    dbEntity.GeneralTodoLists.Add(oTodo);
                    dbEntity.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                WriteEvent(ex.Message, this.ToString());
            }
        }

        /// <summary>
        /// 订单处理日志，供客户查阅
        /// </summary>
        /// <param name="msg">日志内容（可空）</param>
        /// <param name="resource">资源代码（用于多语言资源）</param>
        /// <param name="classname">类名</param>
        /// <param name="show">是否显示到客户端</param>
        /// <param name="orderID">订单ID</param>
        /// <param name="userID">用户ID</param>
        public void WriteOrderEvent(string msg, string resource, string classname, bool show, Guid orderID, Guid? userID)
        {
            string sMessage = msg;
            WriteEvent(sMessage, ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.ORDER,
                classname, userID, ModelEnum.NoteType.ORDER, orderID, true);
            if (show)
            {
                if (!String.IsNullOrEmpty(resource))
                {
                    try
                    {
                        sMessage = Resource.Model.Order.OrderInformation.ResourceManager.GetString(resource);
                    }
                    catch { }
                }
                OrderProcess oProcess = new OrderProcess
                {
                    OrderID = orderID,
                    Code = resource,
                    Show = true,
                    Matter = sMessage
                };
                dbEntity.OrderProcesses.Add(oProcess);
                dbEntity.SaveChanges();
            }
        }

        /// <summary>
        /// 采购单处理日志
        /// </summary>
        /// <param name="msg">日志内容（可空）</param>
        /// <param name="resource">代码（用于多语言资源）</param>
        /// <param name="classname">类名</param>
        /// <param name="show">是否显示到客户端</param>
        /// <param name="orderID">订单ID</param>
        /// <param name="userID">用户ID</param>
        public void WritePurchaseEvent(string msg, string resource, string classname, bool show, Guid purchaseID, Guid? userID)
        {
            string sMessage = msg;
            if (!String.IsNullOrEmpty(resource))
            {
                try
                {
                    sMessage = Resource.Model.Order.OrderInformation.ResourceManager.GetString(resource);
                }
                catch { }
            }
            WriteEvent(sMessage, ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.PURCHASE,
                classname, userID, ModelEnum.NoteType.PURCHASE, purchaseID, true);
        }

    }
}
