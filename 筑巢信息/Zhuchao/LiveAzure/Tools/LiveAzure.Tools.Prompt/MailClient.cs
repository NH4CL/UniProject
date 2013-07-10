using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Net.Mail;
using System.Net;
using System.Net.Mime;

namespace LiveAzure.Tools.Prompt
{
    public class MailClient
    {
        private static string strExecuteFile, strEventFile, strXmlFile;
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
                        case "-X":
                            strXmlFile = arguments[++i];
                            break;
                        case "-Q":
                            bOutputConsole = false;
                            break;
                        default:
                            break;
                    }
                }

                EventLog("任务启动");
                switch (sCommand)
                {
                    case "SEND":
                        if (strXmlFile == null)
                            ParamError();
                        else if (CheckXmlFormat())
                            SendMail();
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

        /// <summary>初步检查XML格式是否正确</summary>
        /// <returns></returns>
        private static bool CheckXmlFormat()
        {
            bool bResult = true;
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(strXmlFile);
                XmlNode xmlNodeRoot = xmlDocument.SelectSingleNode("root");
                foreach (XmlNode xmlNodeMail in xmlNodeRoot.ChildNodes)
                {
                    XmlElement xmlElementMail = (XmlElement)xmlNodeMail;
                    string sSmtp = xmlElementMail.GetAttribute("smtp");
                    string sBody = xmlElementMail.GetAttribute("body");
                    strEventFile = xmlElementMail.GetAttribute("log");
                    if ((sSmtp == null) || (sSmtp == "") || (sBody == null) || (sBody == ""))
                    {
                        bResult = false;
                        break;
                    }
                    if (!File.Exists(sBody))
                    {
                        bResult = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog(string.Format("检查Xml格式错误 {0}, {1}", ex.Message, strXmlFile));
                bResult = false;
            }
            return bResult;
        }

        private static void SendMail()
        {
            XmlDocument xmlDocument = new XmlDocument();
            int nCount = 1;
            EventLog("开始发送邮件");
            try
            {
                xmlDocument.Load(strXmlFile);
                XmlNode xmlNodeRoot = xmlDocument.SelectSingleNode("root");
                foreach (XmlNode xmlNodeMail in xmlNodeRoot.ChildNodes)
                {
                    XmlElement xmlElementMail = (XmlElement)xmlNodeMail;
                    // 服务器配置
                    string sSmtp = xmlElementMail.GetAttribute("smtp");
                    int nPort = 25;
                    int.TryParse(xmlElementMail.GetAttribute("port"), out nPort);
                    bool bSsl = false;
                    bool.TryParse(xmlElementMail.GetAttribute("ssl"), out bSsl);
                    string sUsername = xmlElementMail.GetAttribute("username");
                    string sPassword = xmlElementMail.GetAttribute("password");
                    string sBodyFile = xmlElementMail.GetAttribute("body");
                    strEventFile = xmlElementMail.GetAttribute("log");
                    // 建立SMTP对象
                    SmtpClient oSmtpClient = new SmtpClient();
                    oSmtpClient.Host = sSmtp;
                    oSmtpClient.Credentials = new NetworkCredential(sUsername, sPassword);
                    oSmtpClient.Port = nPort;
                    oSmtpClient.EnableSsl = bSsl;
                    // 邮件内容字符串
                    StreamReader oBodyReader = new StreamReader(sBodyFile, System.Text.Encoding.UTF8);
                    string sBodySource = oBodyReader.ReadToEnd();
                    // 逐个发送邮件
                    foreach (XmlNode xmlNodeSend in xmlNodeMail.ChildNodes)
                    {
                        XmlElement xmlElementSend = (XmlElement)xmlNodeSend;
                        string sFromMail = xmlElementSend.GetAttribute("from");
                        string sToEmail = xmlElementSend.GetAttribute("to");
                        string sDisplayName = xmlElementSend.GetAttribute("displayname");
                        string sSubject = xmlElementSend.GetAttribute("subject");
                        // 替换标识 replace_0 的内容 替换 body 中的 {0}，以此类推，最多10个替换
                        string sBodyMessage = sBodySource;
                        for (int i = 0; i < 10; i++)
                        {
                            string sReplace = xmlElementSend.GetAttribute("replace_" + i);
                            if ((sReplace != null) && (sReplace != ""))
                                sBodyMessage = sBodyMessage.Replace("{" + i + "}", sReplace);
                        }
                        // 建立MailMessage对象
                        MailMessage oMessage = new MailMessage(new MailAddress(sFromMail, sDisplayName), new MailAddress(sToEmail));
                        oMessage.Subject = sSubject;
                        oMessage.SubjectEncoding = Encoding.UTF8;
                        oMessage.Body = sBodyMessage;
                        oMessage.BodyEncoding = Encoding.UTF8;
                        oMessage.IsBodyHtml = true;
                        oMessage.Priority = MailPriority.High;
                        // oMessage.Headers.Add("Disposition-Notification-To", sFromMail);  // 回执
                        // oMessage.Headers.Add("ReturnReceipt", "1");   // 针对 Lotus Domino Server，插入回执头
                        // 添加附件，最多10个
                        for (int i = 0; i < 10; i++)
                        {
                            string sFileName = xmlElementSend.GetAttribute("attached_" + i);
                            if ((sFileName != null) && (sFileName != ""))
                            {
                                if (File.Exists(sFileName))
                                {
                                    Attachment oAttachFile = new Attachment(sFileName, MediaTypeNames.Application.Octet);
                                    oMessage.Attachments.Add(oAttachFile);
                                }
                            }
                        }
                        try
                        {
                            oSmtpClient.Send(oMessage);
                            EventLog(string.Format("发送成功 From: {0} To: {1}", sFromMail, sToEmail));
                        }
                        catch (SmtpException ex)
                        {
                            EventLog(string.Format("发送失败 From: {0} To: {1}，原因 {2}", sFromMail, sToEmail, ex.Message));
                        }
                        nCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog(string.Format("批处理错误 {0}, {1}, 行号：{2}", ex.Message, strXmlFile, nCount));
            }
            EventLog("发送邮件结束");
        }

        private static void ParamError()
        {
            EventLog(string.Format("参数错误，{0} -MAIL help", strExecuteFile));
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
            Console.WriteLine("{0}  -MAIL 版本 1.0.11.0109", strExecuteFile);
            Console.WriteLine("群发邮件客户端命令", strExecuteFile);
            Console.WriteLine("");
            Console.WriteLine("{0} -MAIL command [-q] -x xmlfile", strExecuteFile);
            Console.WriteLine("");
            Console.WriteLine("  command 已实现命令");
            Console.WriteLine("          send     根据xml中的配置群发邮件，必须-x参数");
            Console.WriteLine("");
            Console.WriteLine("  -x Xml配置文件名，示例如下：");
            Console.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            Console.WriteLine("<root>");
            Console.WriteLine("  <mail smtp=\"smtp.zhuchao.com\" port=\"25\" ssl=\"false\" username=\"service@zhuchao.com\" password=\"****\" body=\"C:\\Temp\\mail\\body.html\" log=\"C:\\Temp\\mail\\sending.log\">");
            Console.WriteLine("    <send from=\"service@zhuchao.com\" to=\"bojian@zhuchao.com\" displayname=\"筑巢家居网\" subject=\"Hello World\" replace_0=\"serialnumber1\" replace_1=\"password1\" attached_0=\"C:\\Temp\\mail\\Book1.xls\"></send>");
            Console.WriteLine("    <send from=\"service@zhuchao.com\" to=\"tangxh@zhuchao.com\" displayname=\"筑巢家居网\" subject=\"Hello World\" replace_0=\"serialnumber2\" replace_1=\"password2\"></send>");
            Console.WriteLine("    <send from=\"service@zhuchao.com\" to=\"tmxli@163.com\" displayname=\"筑巢家居网\" subject=\"Hello World\" replace_0=\"serialnumber3\" replace_1=\"password3\"></send>");
            Console.WriteLine("    <send from=\"service@zhuchao.com\" to=\"tmxli@hotmail.com\" displayname=\"筑巢家居网\" subject=\"Hello World\" replace_0=\"serialnumber4\" replace_1=\"password4\" attached_0=\"C:\\Temp\\mail\\Book1.xls\"></send>");
            Console.WriteLine("  </mail>");
            Console.WriteLine("</root>");
            Console.WriteLine("");
            Console.WriteLine("例如：{0} -MAIL send -x C:/Temp/file.xml", strExecuteFile);
            Console.WriteLine("");
            Console.WriteLine("(c) Copyright 2011, by 伯鉴");
        }
    }
}
