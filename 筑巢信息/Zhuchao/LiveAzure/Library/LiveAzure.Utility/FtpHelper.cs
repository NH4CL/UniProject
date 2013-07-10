using System;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Xml;
using System.Globalization;

namespace LiveAzure.Utility
{
    public class FtpHelper
    {
        private string strFtpHost, strFtpUsername, strFtpPassword, strEventFile = "";
        private string[] strFtpSkipFiles;

        private bool bFtpConsole = false;
        private bool bFtpEnableSsl = false;
        private bool bFtpOverwrite = false;
        private bool bFtpTimeNew = false;
        private FtpWebRequest oFtpRequest;

        public FtpHelper()
        {
        }

        /// <summary>构造函数，FTP登录参数</summary>
        /// <param name="sEventFile">日志文件</param>
        /// <param name="bShowConsole">在Console中输出</param>
        public FtpHelper(string sEventFile, bool bShowConsole)
        {
            this.strEventFile = sEventFile;
            this.bFtpConsole = bShowConsole;
        }

        /// <summary>构造函数，FTP登录参数</summary>
        /// <param name="sHost">FTP主机名或IP地址</param>
        /// <param name="bEnabledSsl">是否使用SSL协议</param>
        /// <param name="bOverwrite">自动覆盖已存在的文件</param>
        /// <param name="bTimeNew">检查文件日期，仅复制新的和不存在的文件</param>
        /// <param name="sUsername">FTP用户名</param>
        /// <param name="sPassword">FTP密码</param>
        /// <param name="sEventFile">日志文件</param>
        /// <param name="bShowConsole">在Console中输出</param>
        /// <param name="sSkipFiles">排除的文件夹或文件</param>
        public FtpHelper(string sHost, bool bEnabledSsl, bool bOverwrite, bool bTimeNew,
            string sUsername, string sPassword, string sEventFile, bool bShowConsole, string sSkipFiles)
        {
            SetParameters(sHost, bEnabledSsl, bOverwrite, bTimeNew, sUsername, sPassword, sEventFile, bShowConsole, sSkipFiles);
        }

        /// <summary>FTP登录参数</summary>
        /// <param name="sHost">FTP主机名或IP地址</param>
        /// <param name="bEnabledSsl">是否使用SSL协议</param>
        /// <param name="bOverwrite">自动覆盖已存在的文件</param>
        /// <param name="bTimeNew">检查文件日期，仅复制新的和不存在的文件</param>
        /// <param name="sUsername">FTP用户名</param>
        /// <param name="sPassword">FTP密码</param>
        /// <param name="sEventFile">日志文件</param>
        /// <param name="bShowConsole">在Console中直接输出</param>
        /// <param name="sSkipFiles">排除的文件夹或文件</param>
        public void SetParameters(string sHost, bool bEnabledSsl, bool bOverwrite, bool bTimeNew,
            string sUsername, string sPassword, string sEventFile, bool bShowConsole, string sSkipFiles)
        {
            this.strFtpHost = sHost;
            this.bFtpEnableSsl = bEnabledSsl;
            this.bFtpOverwrite = bOverwrite;
            this.bFtpTimeNew = bTimeNew;
            this.strFtpUsername = sUsername;
            this.strFtpPassword = sPassword;
            this.strEventFile = sEventFile;
            this.bFtpConsole = bShowConsole;
            this.strFtpSkipFiles = sSkipFiles.Split(';');
        }

        /// <summary>默认参数连接</summary>
        /// <param name="sRemotePath">远程文件或文件夹</param>
        private void Connect(string sRemotePath)
        {
            oFtpRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(sRemotePath));
            oFtpRequest.UseBinary = true;
            oFtpRequest.EnableSsl = bFtpEnableSsl;
            oFtpRequest.Credentials = new NetworkCredential(strFtpUsername, strFtpPassword);
            if (bFtpEnableSsl)
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
        }

        /// <summary>获取FTP文件列表</summary>
        /// <param name="sRemoteFolder">服务器文件夹名</param>
        /// <param name="sRequestMethods">Web Request Method</param>
        /// <param name="bIgnoreError">忽略错误信息</param>
        /// <returns>文件列表数组</returns>
        private string[] GetFileList(string sRemoteFolder, string sRequestMethods, bool bIgnoreError = false)
        {
            StringBuilder sResult = new StringBuilder();
            try
            {
                Connect(sRemoteFolder);
                oFtpRequest.Method = sRequestMethods;

                FtpWebResponse oResponse = (FtpWebResponse)oFtpRequest.GetResponse();
                StreamReader oReader = new StreamReader(oResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                string sLine = oReader.ReadLine();
                while (sLine != null)
                {
                    sResult.Append(sLine);
                    sResult.Append("\n");
                    sLine = oReader.ReadLine();
                }
                oReader.Close();
                oResponse.Close();
                if (sResult.Length > 0)
                    sResult.Remove(sResult.ToString().LastIndexOf('\n'), 1);
            }
            catch (Exception ex)
            {
                if (!bIgnoreError)
                    MessageOutput(string.Format("文件列表错误 {0}, {1}", ex.Message, sRemoteFolder));
            }
            return sResult.ToString().Split('\n'); ;
        }

        /// <summary>获得文件简单信息（仅文件名或文件夹名）</summary>
        /// <param name="sRemoteFolder">FTP文件夹名</param>
        /// <param name="bIgnoreError">忽略错误信息</param>
        /// <returns>文件名列表</returns>
        public string[] GetFileList(string sRemoteFolder, bool bIgnoreError = false)
        {
            return GetFileList("ftp://" + strFtpHost + "/" + sRemoteFolder, WebRequestMethods.Ftp.ListDirectory, bIgnoreError);
        }

        /// <summary>获得文件详细信息</summary>
        /// <param name="sRemoteFolder">FTP文件夹名</param>
        /// <param name="bIgnoreError">忽略错误信息</param>
        /// <returns>文件详细信息列表</returns>
        public string[] GetFileDetailList(string sRemoteFolder, bool bIgnoreError = false)
        {
            return GetFileList("ftp://" + strFtpHost + "/" + sRemoteFolder, WebRequestMethods.Ftp.ListDirectoryDetails, bIgnoreError);
        }

        /// <summary>判断文件是否存在，如果bFtpOverwrite=true OR (bFtpTimeNew=true AND 目标文件较旧)则删除文件</summary>
        /// <param name="sFullPath">完整路径文件名</param>
        /// <param name="bIsFtp">是否判断远程文件</param>
        /// <param name="dtSourceFileTime">源文件日期</param>
        /// <returns>处理后，文件是否存在标识</returns>
        private bool TargetFilePrepare(string sFullPath, bool bIsFtp, DateTimeOffset dtSourceFileTime)
        {
            bool bResult = false;
            try
            {
                if (bIsFtp)
                {
                    Connect(sFullPath);
                    // oFtpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                    oFtpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                    FtpWebResponse oResponse = (FtpWebResponse)oFtpRequest.GetResponse();
                    StreamReader oReader = new StreamReader(oResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                    string sLine = oReader.ReadLine();
                    if (sLine != null)
                    {
                        bResult = true;
                        DateTimeOffset dtTargetFileTime = ParseDateTime(sLine.Substring(0, 19));
                        if (bFtpOverwrite || (bFtpTimeNew && (dtSourceFileTime > dtTargetFileTime)))
                        {
                            Connect(sFullPath);
                            oFtpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                            oResponse = (FtpWebResponse)oFtpRequest.GetResponse();
                            bResult = false;
                        }
                    }
                    oReader.Close();
                    oResponse.Close();
                }
                else
                {
                    if (File.Exists(sFullPath))
                    {
                        bResult = true;
                        DateTimeOffset dtTargetFileTime = File.GetLastWriteTime(sFullPath);
                        if (bFtpOverwrite || (bFtpTimeNew && (dtSourceFileTime > dtTargetFileTime)))
                        {
                            File.Delete(sFullPath);
                            bResult = false;
                        }
                    }
                }
            }
            catch { }
            return bResult;
        }

        /// <summary>上传文件</summary>
        /// <param name="sLocalFile">本地待上传文件名（全路径）</param>
        /// <param name="sRemoteFolder">待上传到FTP的文件夹</param>
        /// <returns>上传是否成功</returns>
        public bool UploadFile(string sLocalFile, string sRemoteFile)
        {
            return UploadFile(sLocalFile, sRemoteFile, DateTimeOffset.Now);
        }

        /// <summary>上传文件</summary>
        /// <param name="sLocalFile">本地待上传文件名（全路径）</param>
        /// <param name="sRemoteFile">待上传到FTP的文件夹，或全路径</param>
        /// <param name="dtSourceFileTime">上传完成后设置文件日期，该功能未完成</param>
        /// <returns>上传是否成功</returns>
        public bool UploadFile(string sLocalFile, string sRemoteFile, DateTimeOffset dtSourceFileTime)
        {
            bool bResult = false;
            FileInfo oFileInfo = new FileInfo(sLocalFile);
            string sFtpUri;
            if ((sRemoteFile == "/") || (Path.GetFileName(sRemoteFile) == ""))
                sFtpUri = "ftp://" + strFtpHost + "/" + sRemoteFile + "/" + oFileInfo.Name;
            else
                sFtpUri = "ftp://" + strFtpHost + "/" + sRemoteFile;

            if (TargetFilePrepare(sFtpUri, true, dtSourceFileTime))
                MessageOutput(string.Format("远程文件已存在, 无法上传 {0}", sRemoteFile));
            else
            {
                Connect(sFtpUri);
                oFtpRequest.KeepAlive = false;
                oFtpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                oFtpRequest.ContentLength = oFileInfo.Length;
                int nBufferSize = 2048;    // 缓冲大小设置为kb
                byte[] btBuffer = new byte[nBufferSize];
                int nContentLength;

                FileStream oLocalStream = oFileInfo.OpenRead();     // 打开一个文件流(System.IO.FileStream) 去读上传的文件
                try
                {
                    Stream oRemoteStream = oFtpRequest.GetRequestStream();     // 把上传的文件写入流
                    nContentLength = oLocalStream.Read(btBuffer, 0, nBufferSize);
                    while (nContentLength != 0)
                    {
                        // 把内容从file stream 写入upload stream
                        oRemoteStream.Write(btBuffer, 0, nContentLength);
                        nContentLength = oLocalStream.Read(btBuffer, 0, nBufferSize);
                    }
                    oRemoteStream.Close();
                    oLocalStream.Close();

                    // TODO: 上传完成后，设置文件日期，FTP不支持设置文件日期

                    MessageOutput(string.Format("上传完成 {0} {1}", sLocalFile, sRemoteFile));
                    bResult = true;
                }
                catch (Exception ex)
                {
                    MessageOutput(string.Format("上传错误 {0}, {1}", ex.Message, sLocalFile));
                }
            }
            return bResult;
        }

        /// <summary>续传文件，未经验证</summary>
        /// <param name="sLocalFile">本地待上传文件名（全路径）</param>
        /// <param name="nDoneSize">已完成上传文件大小</param>
        /// <param name="sRemoteFile">已部分完成的FTP文件名（全路径）</param>
        /// <returns>上传是否成功</returns>
        public bool UploadFile(string sLocalFile, long nDoneSize, string sRemoteFile)
        {
            bool bResult = false;
            string sFtpUri = "ftp://" + strFtpHost + "/" + sRemoteFile;
            FileInfo oFileInfo = new FileInfo(sLocalFile);
            Connect(sFtpUri);
            oFtpRequest.KeepAlive = false;
            oFtpRequest.Method = WebRequestMethods.Ftp.AppendFile;
            oFtpRequest.ContentLength = oFileInfo.Length;
            int nBufferSize = 2048;         // 缓冲大小设置为kb
            byte[] btBuffer = new byte[nBufferSize];
            int nContentLength;
            FileStream oLocalStream = oFileInfo.OpenRead();
            try
            {
                StreamReader oSeekStream = new StreamReader(oLocalStream);
                oLocalStream.Seek(nDoneSize, SeekOrigin.Begin);
                Stream oRemoteStream = oFtpRequest.GetRequestStream();
                nContentLength = oLocalStream.Read(btBuffer, 0, nBufferSize);
                while (nContentLength != 0)
                {
                    oRemoteStream.Write(btBuffer, 0, nContentLength);
                    nContentLength = oLocalStream.Read(btBuffer, 0, nBufferSize);
                }
                oRemoteStream.Close();
                oLocalStream.Close();
                oSeekStream.Close();
                MessageOutput(string.Format("上传完成 {0}", sLocalFile));
                bResult = true;
            }
            catch (Exception ex)
            {
                MessageOutput(string.Format("上传错误 {0}, {1}", ex.Message, sLocalFile));
            }
            return bResult;
        }

        /// <summary>递归上传所有文件和文件夹</summary>
        /// <param name="sRemoteFolder">FTP服务器文件夹名</param>
        /// <param name="sLocalFolder">本地的文件夹名</param>
        /// <returns>上传是否成功</returns>
        public bool UploadFolder(string sLocalFolder, string sRemoteFolder)
        {
            DirectoryInfo oLocalFolder = new DirectoryInfo(sLocalFolder);
            CreateFolder(sRemoteFolder, true);
            // 上传文件
            foreach (FileInfo oNextFile in oLocalFolder.GetFiles())
            {
                string sUploadSource = oNextFile.FullName;
                string sUploadTarget = sRemoteFolder + "/" + oNextFile.Name;
                if (!SkipThisFile(sUploadSource))
                    UploadFile(sUploadSource, sUploadTarget, oNextFile.LastWriteTime);
            }
            // 递归进入子文件夹
            foreach (DirectoryInfo oNextFolder in oLocalFolder.GetDirectories())
            {
                string sUploadSource = oNextFolder.FullName;
                string sUploadTarget = sRemoteFolder + "/" + oNextFolder.Name;
                if (!SkipThisFile(sUploadSource))
                    UploadFolder(sUploadSource, sUploadTarget);
            }
            return true;
        }

        /// <summary>下载单个文件</summary>
        /// <param name="sRemoteFile">FTP服务器文件名（全路径）</param>
        /// <param name="sLocalFile">下载到本地的文件名（全路径）</param>
        /// <returns>下载是否成功</returns>
        public bool DownloadFile(string sRemoteFile, string sLocalFile)
        {
            return DownloadFile(sRemoteFile, sLocalFile, DateTime.Now);
        }

        /// <summary>下载单个文件</summary>
        /// <param name="sRemoteFile">FTP服务器文件名（全路径）</param>
        /// <param name="sLocalFile">下载到本地的文件名（全路径）</param>
        /// <param name="dtSourceFileTime">下载完成后设置文件日期</param>
        /// <returns>下载是否成功</returns>
        public bool DownloadFile(string sRemoteFile, string sLocalFile, DateTime dtSourceFileTime)
        {
            bool bResult = false;
            string sFtpUrl = "ftp://" + strFtpHost + "/" + sRemoteFile;
            if (TargetFilePrepare(sLocalFile, false, dtSourceFileTime))
            {
                if (!bFtpTimeNew)        // 下载新文件时，已存在的文件不提示
                {
                    MessageOutput(string.Format("本地文件已存在, 无法下载 {0}", sLocalFile));
                }
            }
            else
            {
                try
                {
                    Connect(sFtpUrl);
                    oFtpRequest.KeepAlive = false;
                    oFtpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                    FtpWebResponse oResponse = (FtpWebResponse)oFtpRequest.GetResponse();
                    Stream oRemoteStream = oResponse.GetResponseStream();
                    long nContentLength = oResponse.ContentLength;
                    int nBufferSize = 2048;   // kb
                    int nReadCount;
                    byte[] btBuffer = new byte[nBufferSize];
                    nReadCount = oRemoteStream.Read(btBuffer, 0, nBufferSize);
                    FileStream oLocalStream = new FileStream(sLocalFile, FileMode.Create);
                    while (nReadCount > 0)
                    {
                        oLocalStream.Write(btBuffer, 0, nReadCount);
                        nReadCount = oRemoteStream.Read(btBuffer, 0, nBufferSize);
                    }
                    oRemoteStream.Close();
                    oLocalStream.Close();
                    oResponse.Close();
                    // 下载完成后，设置文件日期
                    File.SetCreationTime(sLocalFile, dtSourceFileTime);
                    File.SetLastWriteTime(sLocalFile, dtSourceFileTime);
                    File.SetLastAccessTime(sLocalFile, dtSourceFileTime);
                    MessageOutput(string.Format("下载完成 {0}", sRemoteFile));
                    bResult = true;
                }
                catch (Exception ex)
                {
                    MessageOutput(string.Format("下载错误 {0}, {1}", ex.Message, sRemoteFile));
                }
            }
            return bResult;
        }

        /// <summary>递归下载所有文件和文件夹</summary>
        /// <param name="sRemoteFolder">FTP服务器文件夹名</param>
        /// <param name="sLocalFolder">本地的文件夹名</param>
        /// <returns>下载是否成功</returns>
        public bool DownloadFolder(string sRemoteFolder, string sLocalFolder)
        {
            string[] sFullFileList = GetFileDetailList(sRemoteFolder);
            string[] sOnlyFileList = GetFileList(sRemoteFolder);
            if (sFullFileList.Length != sOnlyFileList.Length)
                MessageOutput(string.Format("文件夹发生变化，无法完成操作 {0}", sRemoteFolder));
            else
            {
                for (int i = 0; i < sFullFileList.Length; i++)
                {
                    // 仅支持MS-DOS文件列表格式   TODO 支持UNIX文件列表格式
                    string sFullName = sFullFileList[i];
                    string sOnlyName = sOnlyFileList[i];
                    if ((sFullName != null) && (sFullName != "") && (sOnlyName != null) && (sOnlyName != ""))
                    {
                        string sDownSource = sRemoteFolder + "/" + sOnlyName;
                        string sDownTarget = sLocalFolder + "/" + sOnlyName;
                        if (!SkipThisFile(sDownSource))
                        {
                            if (sFullName.Contains("<DIR>"))  // 如果是文件夹，则创建文件夹，然后递归
                            {
                                DirectoryInfo oTargetFolder = new DirectoryInfo(sDownTarget);
                                oTargetFolder.Create();       // 自行判断一下是否存在
                                DownloadFolder(sDownSource, sDownTarget);
                            }
                            else                              // 如果是文件，则下载
                            {
                                // 获取源文件日期
                                DateTime dtSourceFileTime = ParseDateTime(sFullName.Substring(0, 19));
                                DownloadFile(sDownSource, sDownTarget, dtSourceFileTime);
                            }
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>判断是否排除指定文件或文件夹</summary>
        /// <param name="sTobeCheck">源文件夹或文件名</param>
        /// <returns>true 排除，false 不排除</returns>
        private bool SkipThisFile(string sTobeCheck)
        {
            bool bResult = false;
            if (this.strFtpSkipFiles != null)
            {
                foreach (string s in this.strFtpSkipFiles)
                {
                    if (s == sTobeCheck) bResult = true;
                }
            }
            return bResult;
        }

        /// <summary>根据xml批量处理</summary>
        /// <param name="sXmlFile">批量处理的xml文件</param>
        /// <returns>是否成功</returns>
        /// <remarks>
        /// <![CDATA[
        /// <?xml version="1.0"?>
        /// <root>
        ///   <ftp host="ftp.zhuchao.com" username="bojian" password="****" ssl="ture" overwrite="false" command="download">
        ///     <files remotefolder="/Shared/Temp" localfolder="C:/Temp"></files>
        ///     <files><remote>/temp/file1.txt</remote><local>C:/temp/file1.txt</local></files>
        ///   </ftp>
        /// </root>
        /// ]]>
        /// </remarks>
        public bool BatchProcess(string sXmlFile)
        {
            bool bResult = false;
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(sXmlFile);
                XmlNode xmlNodeRoot = xmlDocument.SelectSingleNode("root");
                foreach (XmlNode xmlNodeFTP in xmlNodeRoot.ChildNodes)
                {
                    XmlElement xmlElementFTP = (XmlElement)xmlNodeFTP;
                    strFtpHost = xmlElementFTP.GetAttribute("host");
                    strFtpUsername = xmlElementFTP.GetAttribute("username");
                    strFtpPassword = xmlElementFTP.GetAttribute("password");
                    bool.TryParse(xmlElementFTP.GetAttribute("ssl"), out bFtpEnableSsl);
                    bool.TryParse(xmlElementFTP.GetAttribute("overwrite"), out bFtpOverwrite);
                    bool.TryParse(xmlElementFTP.GetAttribute("timenew"), out bFtpTimeNew);
                    string strCommand = xmlElementFTP.GetAttribute("command").ToUpper();

                    foreach (XmlNode xmlNodeFile in xmlNodeFTP.ChildNodes)
                    {
                        XmlElement xmlElementFile = (XmlElement)xmlNodeFile;
                        // 上传或下载文件夹
                        string sRemoteFolder = xmlElementFile.GetAttribute("remotefolder");
                        string sLocalFolder = xmlElementFile.GetAttribute("localfolder");
                        sLocalFolder = sLocalFolder.Replace("/", "\\");
                        // 按指定文件一个一个下载或上传
                        string sRemoteFile = xmlNodeFile.SelectSingleNode("remote").InnerText;
                        string sLocalFile = xmlNodeFile.SelectSingleNode("local").InnerText;
                        sLocalFile = sLocalFile.Replace("/", "\\");
                        if (strCommand == "DOWNLOAD")
                        {
                            if ((sRemoteFolder != "") && (sLocalFolder != ""))
                                DownloadFolder(sRemoteFolder, sLocalFolder);
                            DownloadFile(sRemoteFile, sLocalFile);
                        }
                        else if (strCommand == "UPLOAD")
                        {
                            if ((sRemoteFolder != "") && (sLocalFolder != ""))
                                UploadFolder(sRemoteFolder, sLocalFolder);
                            UploadFile(sLocalFile, sRemoteFile);
                        }
                    }
                }
                bResult = true;
            }
            catch (Exception ex)
            {
                MessageOutput(string.Format("批处理错误 {0}, {1}", ex.Message, sXmlFile));
            }
            return bResult;
        }

        /// <summary>删除文件</summary>
        /// <param name="sRemoteFile">FTP服务器文件名</param>
        /// <param name="bIgnoreError">忽略错误信息</param>
        /// <returns>操作结果</returns>
        public bool DeleteFile(string sRemoteFile, bool bIgnoreError = false)
        {
            bool bResult = false;
            try
            {
                string sFtpUri = "ftp://" + strFtpHost + "/" + sRemoteFile;
                Connect(sFtpUri);
                oFtpRequest.KeepAlive = false;
                oFtpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                FtpWebResponse oResponse = (FtpWebResponse)oFtpRequest.GetResponse();
                oResponse.Close();
                MessageOutput(string.Format("删除完成 {0}", sRemoteFile));
                bResult = true;
            }
            catch (Exception ex)
            {
                if (!bIgnoreError)
                    MessageOutput(string.Format("删除错误 {0}, {1}", ex.Message, sRemoteFile));
            }
            return bResult;
        }

        /// <summary>在FTP服务器上创建目录</summary>
        /// <param name="sRemoteFolder">待创建的目录名</param>
        /// <param name="bIgnoreError">忽略错误信息</param>
        /// <returns>操作结果</returns>
        public bool CreateFolder(string sRemoteFolder, bool bIgnoreError = false)
        {
            bool bResult = false;
            try
            {
                string sFtpUri = "ftp://" + strFtpHost + "/" + sRemoteFolder;
                Connect(sFtpUri);
                oFtpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                FtpWebResponse oResponse = (FtpWebResponse)oFtpRequest.GetResponse();
                oResponse.Close();
                MessageOutput(string.Format("创建文件夹完成 {0}", sRemoteFolder));
                bResult = true;
            }
            catch (Exception ex)
            {
                if (!bIgnoreError)
                    MessageOutput(string.Format("创建文件夹错误 {0}, {1}", ex.Message, sRemoteFolder));
            }
            return bResult;
        }

        /// <summary>删除FTP服务器上的目录，递归删除目录中的文件和子目录</summary>
        /// <param name="sRemoteFolder">待删除的目录名</param>
        /// <param name="bIgnoreError">忽略错误信息</param>
        /// <returns>操作结果</returns>
        public bool RemoveFolder(string sRemoteFolder, bool bIgnoreError = false)
        {
            bool bResult = false;
            try
            {
                string[] sFullFileList = GetFileDetailList(sRemoteFolder, true);
                string[] sOnlyFileList = GetFileList(sRemoteFolder, true);
                if (sFullFileList.Length != sOnlyFileList.Length)
                    MessageOutput(string.Format("文件夹发生变化，无法完成操作 {0}", sRemoteFolder));
                else
                {
                    for (int i = 0; i < sFullFileList.Length; i++)
                    {
                        // 仅支持MS-DOS文件列表格式   TODO 支持UNIX文件列表格式
                        string sFullName = sFullFileList[i];
                        string sOnlyName = sOnlyFileList[i];
                        string sDownSource = sRemoteFolder + "/" + sOnlyName;
                        if ((sFullName != null) && (sFullName != ""))
                        {
                            if (sFullName.Contains("<DIR>"))  // 如果是文件夹，则递归删除
                                RemoveFolder(sDownSource, true);
                            else                              // 如果是文件，直接删除
                                DeleteFile(sDownSource, true);
                        }
                    }
                }
                string sFtpUri = "ftp://" + strFtpHost + "/" + sRemoteFolder;
                Connect(sFtpUri);
                oFtpRequest.Method = WebRequestMethods.Ftp.RemoveDirectory;
                FtpWebResponse oResponse = (FtpWebResponse)oFtpRequest.GetResponse();
                oResponse.Close();
                MessageOutput(string.Format("删除文件夹完成 {0}", sRemoteFolder));
                bResult = true;
            }
            catch (Exception ex)
            {
                if (!bIgnoreError)
                    MessageOutput(string.Format("删除文件夹错误 {0}, {1}", ex.Message, sRemoteFolder));
            }
            return bResult;
        }

        /// <summary>获得FTP服务器上的文件大小</summary>
        /// <param name="sRemoteFile">FTP服务器上的文件</param>
        /// <param name="bIgnoreError">忽略错误信息</param>
        /// <returns>文件大小</returns>
        public long GetFileSize(string sRemoteFile, bool bIgnoreError = false)
        {
            long nFileSize = 0;
            try
            {
                string sFtpUri = "ftp://" + strFtpHost + "/" + sRemoteFile;
                Connect(sFtpUri);
                oFtpRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                FtpWebResponse oResponse = (FtpWebResponse)oFtpRequest.GetResponse();
                nFileSize = oResponse.ContentLength;
                oResponse.Close();
            }
            catch (Exception ex)
            {
                if (bIgnoreError)
                    MessageOutput(string.Format("获取文件大小错误 {0}, {1}", ex.Message, sRemoteFile));
            }
            return nFileSize;
        }

        /// <summary>FTP上文件改名</summary>
        /// <param name="sOldFilename">原文件名</param>
        /// <param name="sNewFilename">新文件名</param>
        /// <param name="bIgnoreError">忽略错误信息</param>
        /// <returns>操作结果</returns>
        public bool Rename(string sOldFilename, string sNewFilename, bool bIgnoreError = false)
        {
            bool bResult = false;
            try
            {
                string sFtpUri = "ftp://" + strFtpHost + "/" + Path.GetFileName(sOldFilename);
                Connect(sFtpUri);
                oFtpRequest.Method = WebRequestMethods.Ftp.Rename;
                oFtpRequest.RenameTo = sNewFilename;
                FtpWebResponse oResponse = (FtpWebResponse)oFtpRequest.GetResponse();
                oResponse.Close();
                MessageOutput(string.Format("改名完成 {0} {1}", sOldFilename, sNewFilename));
                bResult = true;
            }
            catch (Exception ex)
            {
                if (bIgnoreError)
                    MessageOutput(string.Format("改名错误 {0}, {1}", ex.Message, sOldFilename));
            }
            return bResult;
        }

        /// <summary>将FTP的文件日期字符串转换成日期</summary>
        /// <param name="sFtpDateTime">字符串格式的文件日期，例如03-31-2011  05:19PM</param>
        private DateTime ParseDateTime(string sFtpDateTime)
        {
            DateTime dtResult;
            IFormatProvider oCulture = new CultureInfo("en-US", true);
            if (DateTime.TryParseExact(sFtpDateTime, "MM-dd-yyyy  hh:mmtt", oCulture, DateTimeStyles.None, out dtResult))
                return dtResult;
            else
                return DateTime.Now;
        }

        /// <summary>输出日志文件</summary>
        /// <param name="sLogMsg">日志信息</param>
        private void MessageOutput(string sLogMsg)
        {
            if ((strEventFile != null) && (strEventFile != ""))
            {
                try
                {
                    StreamWriter fsEventLog = new StreamWriter(strEventFile, true, Encoding.Default);
                    fsEventLog.WriteLine(DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + sLogMsg);
                    fsEventLog.Flush();
                    fsEventLog.Close();
                }
                catch { }
            }
            if (bFtpConsole)
                Console.WriteLine(sLogMsg);
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
