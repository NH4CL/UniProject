using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Diagnostics;

namespace LiveAzure.Tools.MySqlUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            // 不允许多次运行
            string strModuleName=Process.GetCurrentProcess().MainModule.ModuleName;
            string strPathName = Path.GetFileNameWithoutExtension(strModuleName);
            Process[] objProcess = Process.GetProcessesByName(strPathName);
            if (objProcess.Length > 1)
            {
                Console.WriteLine("系统已经在运行中 ");
                Console.WriteLine("Process ID: " + Process.GetCurrentProcess().Id.ToString());
                Console.WriteLine("Module Name: " + Process.GetCurrentProcess().MainModule.ModuleName);
                return;
            }
            // 处理程序
            string[] arguments = Environment.GetCommandLineArgs();
            string strExeFile = Path.GetFileName(arguments[0]);
            if (args.Length < 1)
            {
                Console.WriteLine("命令行参数不正确");
                Console.WriteLine("{0} -OpenShop help", strExeFile);
            }
            else
            {
                string strServiceID = arguments[1].ToUpper();
                switch (strServiceID)
                {
                    case "-OPENSHOP":
                        OpenShopTask.Run();
                        break;
                    case "-HELP":
                        Console.WriteLine("帮助说明");
                        Console.WriteLine("{0} -OpenShop help", strExeFile);
                        break;
                    default:
                        Console.WriteLine("不支持该命令 {0}", strServiceID);
                        Console.WriteLine("{0} -OpenShop help", strExeFile);
                        break;
                }
            }
        }
    }
}
