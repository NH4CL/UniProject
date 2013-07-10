using System;
using System.Text;
using System.IO;
using LiveAzure.Utility;

namespace LiveAzure.Tools.Prompt
{
    public class FtpClient
    {
        private static string strExecuteFile;
        private static string[] arguments;
        private static string strHost, strUsername, strPassword, strRemote, strLocal, strSkipFiles = "", strEventFile = "";
        private static bool bEnableSsl = false;
        private static bool bOutputConsole = true;
        private static bool bOverwrite = false;
        private static bool bTimeNewOnly = false;
        private static bool bIsFolder = false;

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
                        case "-H":
                            strHost = arguments[++i];
                            break;
                        case "-U":
                            strUsername = arguments[++i];
                            break;
                        case "-P":
                            strPassword = arguments[++i];
                            break;
                        case "-S":
                            bEnableSsl = true;
                            break;
                        case "-R":
                            strRemote = arguments[++i];
                            break;
                        case "-L":
                            strLocal = arguments[++i];
                            break;
                        case "-E":
                            strEventFile = arguments[++i];
                            break;
                        case "-Q":
                            bOutputConsole = false;
                            break;
                        case "-O":
                            bOverwrite = true;
                            break;
                        case "-T":
                            bTimeNewOnly = true;
                            break;
                        case "-F":
                            bIsFolder = true;
                            break;
                        case "-K":
                            strSkipFiles = arguments[++i];
                            break;
                        default:
                            break;
                    }
                }
                if (strRemote == null)
                    strRemote = "/";
                if (strUsername == null)
                    strUsername = "anonymouse";
                if (strPassword == null)
                    strPassword = "anonymouse@someone.com";

                EventLog("任务启动");
                switch (sCommand)
                {
                    case "LIST":
                        if (strHost == null)
                            ParamError();
                        else
                        {
                            FtpHelper oFtpHelper = new FtpHelper(strHost, bEnableSsl, bOverwrite, bTimeNewOnly,
                                strUsername, strPassword, strEventFile, bOutputConsole, strSkipFiles);
                            string[] strFileList = oFtpHelper.GetFileDetailList(strRemote);
                            foreach (string s in strFileList) EventLog(s);
                        }
                        break;
                    case "DOWNLOAD":
                        if (strHost == null)
                            ParamError();
                        else
                        {
                            if ((strLocal == null) || (strLocal == ""))
                                strLocal = Directory.GetCurrentDirectory();
                            string sNewName = Path.GetFileName(strRemote);
                            if (sNewName == "")
                                sNewName = Path.GetFileName(strRemote.Substring(0, strRemote.Length - 1));
                            if (bIsFolder)
                                strLocal = strLocal + "/";                 // 下载文件夹(虚拟目录)，存入当前目录
                            else
                                strLocal = strLocal + "/" + sNewName;       // 下载文件，连接文件名称
                            FtpHelper oFtpHelper = new FtpHelper(strHost, bEnableSsl, bOverwrite, bTimeNewOnly,
                                strUsername, strPassword, strEventFile, bOutputConsole, strSkipFiles);
                            if (bIsFolder)
                            {
                                DirectoryInfo oNewFolder = new DirectoryInfo(strLocal);
                                oNewFolder.Create();
                                oFtpHelper.DownloadFolder(strRemote, strLocal);
                            }
                            else
                            {
                                oFtpHelper.DownloadFile(strRemote, strLocal);
                            }
                        }
                        break;
                    case "UPLOAD":
                        if ((strHost == null) || (strLocal == null))
                            ParamError();
                        else
                        {
                            string sNewName = Path.GetFileName(strLocal);
                            if (sNewName == "")
                                sNewName = Path.GetFileName(strLocal.Substring(0, strLocal.Length - 1));
                            strRemote = strRemote + "/" + sNewName;
                            FtpHelper oFtpHelper = new FtpHelper(strHost, bEnableSsl, bOverwrite, bTimeNewOnly,
                                strUsername, strPassword, strEventFile, bOutputConsole, strSkipFiles);
                            if (bIsFolder)
                            {
                                oFtpHelper.CreateFolder(strRemote, true);
                                oFtpHelper.UploadFolder(strLocal, strRemote);
                            }
                            else
                            {
                                oFtpHelper.UploadFile(strLocal, strRemote);
                            }
                        }
                        break;
                    case "DELETE":
                        if (strHost == null)
                            ParamError();
                        else
                        {
                            FtpHelper oFtpHelper = new FtpHelper(strHost, bEnableSsl, bOverwrite, bTimeNewOnly,
                                strUsername, strPassword, strEventFile, bOutputConsole, strSkipFiles);
                            if (bIsFolder)
                                oFtpHelper.RemoveFolder(strRemote);
                            else
                                oFtpHelper.DeleteFile(strRemote);
                        }
                        break;
                    case "MKDIR":
                        if (strHost == null)
                            ParamError();
                        else
                        {
                            FtpHelper oFtpHelper = new FtpHelper(strHost, bEnableSsl, bOverwrite, bTimeNewOnly,
                                strUsername, strPassword, strEventFile, bOutputConsole, strSkipFiles);
                            oFtpHelper.CreateFolder(strRemote);
                        }
                        break;
                    case "SCRIPT":
                        if (strLocal == null)
                            ParamError();
                        else
                        {
                            FtpHelper oFtpHelper = new FtpHelper(strEventFile, bOutputConsole);
                            oFtpHelper.BatchProcess(strLocal);
                        }
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

        private static void ParamError()
        {
            EventLog(string.Format("参数错误，{0} -FTP help", strExecuteFile));
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
            Console.WriteLine("{0}  -FTP 版本 1.2.11.0402", strExecuteFile);
            Console.WriteLine("支持SSL的FTP客户端命令，可代替Windows的ftp.exe程序", strExecuteFile);
            Console.WriteLine("  目前仅支持MS-DOS文件格式，且FTP日期格式为MM-dd-yyyy  hh:mmtt，例如03-31-2011  05:19PM");
            Console.WriteLine("");
            Console.WriteLine("{0} -FTP command [-h host] [-u username] [-p password] [-s] [-q] [-o] [-f] [-t] [-r remote] [-l local] [-e eventlog] [-k skipfiles]", strExecuteFile);
            Console.WriteLine("");
            Console.WriteLine("  command 已实现的命令");
            Console.WriteLine("          download 下载文件或文件夹，必须-h、-r参数");
            Console.WriteLine("                   如果没有-l参数，则下载在当前文件夹");
            Console.WriteLine("                   如果有-f，则下载整个文件夹");
            Console.WriteLine("          upload   上传文件或文件夹，必须-h,-r,-l参数，且-r为完整路径包含文件名");
            Console.WriteLine("                   如果有-f，则上传整个文件夹");
            Console.WriteLine("          list     列出文件夹的内容，必须-h、-r参数，且-r为完整路径");
            Console.WriteLine("          delete   删除FTP服务器上的文件，必须-h、-r参数");
            Console.WriteLine("                   如果有-f，则删除整个文件夹，包括文件夹中的文件");
            Console.WriteLine("          mkdir    在FTP服务器上的创建文件夹，必须-h、-r参数");
            Console.WriteLine("          script   使用xml脚本上传或下载，必须-l参数为xml文件");
            Console.WriteLine("");
            Console.WriteLine("  -h 主机名，不需要ftp://前缀");
            Console.WriteLine("  -u 用户名");
            Console.WriteLine("  -p 密码");
            Console.WriteLine("  -s 使用SSL加密协议");
            Console.WriteLine("  -o 无条件覆盖已经存在的文件，比-t优先级高，即忽略-t");
            Console.WriteLine("  -t 有条件覆盖，检查文件日期，仅复制和覆盖新的和不存在的文件，一般与-f配合");
            Console.WriteLine("     仅支持DOWNLOAD，暂时不支持UPLOAD命令");
            Console.WriteLine("  -f 上传或下载文件夹，否则操作文件");
            Console.WriteLine("  -q 安静模式，不输出结果");
            Console.WriteLine("  -r 远程文件夹或文件名称，主机名后的完整路径，上传时必须包含文件名称");
            Console.WriteLine("  -e 事件日志文件名称");
            Console.WriteLine("  -k 排除的文件夹或文件名，多个排除使用分号;分隔");
            Console.WriteLine("  -l 本地文件名称，下载或上传文件名；如果命令为script，示例：scripts.xml，");
            Console.WriteLine("       script仅支持下载和上传");
            Console.WriteLine("     <?xml version=\"1.0\" encoding=\"utf-8\"?>");
            Console.WriteLine("     <root>");
            Console.WriteLine("       <ftp host=\"ftp.domain.com\" username=\"bojian\" password=\"****\" ssl=\"ture\"");
            Console.WriteLine("             overwrite=\"false\" command=\"download\">");
            Console.WriteLine("         <files localfolder=\"C:\\Temp\" remotefolder=\"/Shared/Temp\" skipfiles=\"/Shared/Temp\"></files>");
            Console.WriteLine("         <files>");
            Console.WriteLine("           <remote>/temp/file2.txt</remote>");
            Console.WriteLine("           <local>C:/temp/file2.txt</local>");
            Console.WriteLine("         </files>");
            Console.WriteLine("       </ftp>");
            Console.WriteLine("     </root>");
            Console.WriteLine("");
            Console.WriteLine("注：长参数可以使用双引号");
            Console.WriteLine("例如：{0} -FTP download -h ftp.domain.com -u bojian");
            Console.WriteLine("               -p **** -s -r /Shared/file.txt -l C:/Temp/file.txt", strExecuteFile);
            Console.WriteLine("      {0} -FTP upload -h ftp.domain.com -u bojian");
            Console.WriteLine("               -p **** -s -r /Shared/file.txt -l C:/Temp/file.txt", strExecuteFile);
            Console.WriteLine("      {0} -FTP list -h ftp.domain.com -u bojian");
            Console.WriteLine("               -p **** -s -r /Shared/", strExecuteFile);
            Console.WriteLine("      {0} -FTP script -l batch.xml", strExecuteFile);
            Console.WriteLine("");
            Console.WriteLine("(c) Copyright 2011, by 伯鉴");
        }
    }
}
