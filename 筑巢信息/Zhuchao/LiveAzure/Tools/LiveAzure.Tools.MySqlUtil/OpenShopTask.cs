using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LiveAzure.Utility;
using MySql.Data.MySqlClient;

namespace LiveAzure.Tools.MySqlUtil
{
    public static class OpenShopTask
    {
        private static string strExecuteFile;
        private static string strEventFile = "";
        private static string[] arguments;
        private static bool bOutputConsole = true;

        public static void Run()
        {
            arguments = Environment.GetCommandLineArgs();
            strExecuteFile = Path.GetFileName(arguments[0]);
            try
            {
                if (arguments.Length < 3)
                {
                    ParamError();
                    return;
                }
                string sCommand = arguments[2].ToUpper();
                if (sCommand == "HELP")
                {
                    ShowHelp();
                    return;
                }
                for (int i = 3; i < arguments.Length; i++)
                {
                    string sParam = arguments[i].ToUpper();
                    switch (sParam)
                    {
                        case "-E":
                            strEventFile = arguments[++i];
                            break;
                        default:
                            break;
                    }
                }

                EventLog("任务启动");
                switch (sCommand)
                {
                    case "MESSAGE":
                        SendPending();
                        break;
                    default:
                        EventLog(string.Format("不支持该命令，请查看帮助 {0}", sCommand));
                        break;
                }
            }
            catch (Exception ex)
            {
                EventLog(string.Format("出现错误 {0}", ex.Message));
            }
        }

        /// <summary>
        /// 发送OpenShop中的手机短信
        /// </summary>
        private static void SendPending()
        {
            string strMyString = ConfigHelper.ConnectionString.MySqlString;
            if (String.IsNullOrEmpty(strMyString))
            {
                EventLog(string.Format("MySQL 连接字符串无效"));
                return;
            }
            try
            {
                MySqlHelper oMySqlHelper = new MySqlHelper(strMyString);
                EucpHelper oEucpHelper = new EucpHelper();
                int nCounter = 0;

                // 发送手机短信
                string sPending = "SELECT * FROM system_message_queue WHERE status = 1";
                Dictionary<int, int> oSendList = new Dictionary<int, int>();
                using (MySqlDataReader oDataReader = oMySqlHelper.ExecuteReader(sPending))
                {
                    if (oDataReader.HasRows)
                    {
                        while (oDataReader.Read())
                        {
                            int nMsgID = (int)oDataReader["id"];
                            string strMsgType = oDataReader["msg_type"].ToString().Trim();
                            string strMobile = oDataReader["target_ids"].ToString();
                            string strContent = oDataReader["msg_content"].ToString();
                            string strMsgName = oDataReader["msg_name"].ToString();
                            string strOrderSn = oDataReader["order_sn"].ToString();
                            int nResult = -1;
                            if (strMsgType == "1")             // 发送邮件
                            {
                                nResult = MailHelper.SendMail(ConfigHelper.SmtpConfig.Sender, ConfigHelper.SmtpConfig.From,
                                    strMobile, "筑巢家居商城订单号：" + strOrderSn, strContent);
                            }
                            else if (strMsgType == "2")        // 发送短信
                            {
                                strContent = strContent.Replace("<p>", "");
                                strContent = strContent.Replace("</p>", "");
                                nResult = oEucpHelper.SendSms(strMobile, strContent);
                            }
                            oSendList.Add(nMsgID, nResult);
                            nCounter++;
                        }
                    }
                    oDataReader.Close();
                }

                // 检测余额
                string strBalance = oEucpHelper.GetBalance().ToString();

                // 更新已发送状态
                foreach (int nKey in oSendList.Keys)
                {
                    string strUpdateStatus = "UPDATE system_message_queue SET status = 3, msg_desc = '" + oSendList[nKey].ToString() + ":" + strBalance + "', send_time = NOW() WHERE id = " + nKey.ToString();
                    oMySqlHelper.ExecuteNonQuery(strUpdateStatus);
                }
                EventLog(string.Format("已发送{0}条手机短信/邮件", nCounter));
                nCounter = 0;

                // 接收手机短信
                List<object> oGetList = oEucpHelper.ReceiveSms();
                foreach (Dictionary<string, object> oMessage in oGetList)
                {
                    string strInsertMessage = "INSERT INTO system_message_receive (mobileNumber, smsContent, sentTime, channelNumber) " +
                        " VALUES ('" + oMessage["Mobile"].ToString() + "', '" + oMessage["Content"].ToString() + "', '" +
                        oMessage["SentTime"].ToString() + "', '" + oMessage["Channel"].ToString() + "')";
                    oMySqlHelper.ExecuteNonQuery(strInsertMessage);
                    nCounter++;
                }
                EventLog(string.Format("已接收{0}条手机短信", nCounter));
            }
            catch (Exception ex)
            {
                EventLog(string.Format("SendPending 出现错误 {0}", ex.Message));
            }
        }

        private static void ParamError()
        {
            EventLog(string.Format("参数错误，{0} -OpenShop help", strExecuteFile));
        }

        private static void EventLog(string sLogMsg)
        {
            if ((strEventFile != null) && (strEventFile != ""))
            {
                try
                {
                    StreamWriter fsEventLog = new StreamWriter(strEventFile, true, Encoding.Default);
                    fsEventLog.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + sLogMsg);
                    fsEventLog.Flush();
                    fsEventLog.Close();
                }
                catch { }
            }
            if (bOutputConsole)
                Console.WriteLine(sLogMsg);
        }

        private static void ShowHelp()
        {
            Console.WriteLine("{0}  -OpenShop 版本 1.0.2011.0601", strExecuteFile);
            Console.WriteLine("OpenShop 定时处理程序");
            Console.WriteLine("");
            Console.WriteLine("{0} -OpenShop message [-q] [-e eventlog]", strExecuteFile);
            Console.WriteLine("");
            Console.WriteLine("  command 已实现的命令");
            Console.WriteLine("          message     发送system_message_queue表中的手机短信和邮件");
            Console.WriteLine("");
            Console.WriteLine("  -q 安静模式，不输出结果");
            Console.WriteLine("  -e 事件日志文件名称");
            Console.WriteLine("");
            Console.WriteLine("注：长参数可以使用双引号");
            Console.WriteLine("例如：{0} -OpenShop message -e C://Temp//sms.log");
            Console.WriteLine("");
            Console.WriteLine("(c) Copyright 2011, by 伯鉴");
        }
    }
}
