using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LiveAzure.Tools.Prompt
{
    class Program
    {
        static void Main(string[] args)
        {

            string[] arguments = Environment.GetCommandLineArgs();
            string strExeFile = Path.GetFileName(arguments[0]);
            if (args.Length < 1)
            {
                Console.WriteLine("命令行参数不正确");
                Console.WriteLine("{0} -FTP help", strExeFile);
                Console.WriteLine("{0} -MAIL help", strExeFile);
                // Console.WriteLine("{0} -SMS help", strExeFile);
            }
            else
            {
                string strServiceID = arguments[1].ToUpper();
                switch (strServiceID)
                {
                    case "-FTP":
                        FtpClient.Run();
                        break;
                    case "-MAIL":
                        MailClient.Run();
                        break;
                    case "-SMS":
                        Console.WriteLine("{0} -SMS 尚未完成", strExeFile);
                        break;
                    case "-HELP":
                        Console.WriteLine("帮助说明");
                        Console.WriteLine("{0} -FTP help", strExeFile);
                        Console.WriteLine("{0} -MAIL help", strExeFile);
                        Console.WriteLine("{0} -SMS help", strExeFile);
                        break;
                    default:
                        Console.WriteLine("不支持该命令 {0}", strServiceID);
                        Console.WriteLine("{0} -FTP help", strExeFile);
                        Console.WriteLine("{0} -MAIL help", strExeFile);
                        // Console.WriteLine("{0} -SMS help", strExeFile);
                        break;
                }
            }
        }
    }
}
