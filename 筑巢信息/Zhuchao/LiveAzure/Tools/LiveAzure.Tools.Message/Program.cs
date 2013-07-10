using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LiveAzure.Tools.Message
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        
        // private static ApplicationContext context;
        
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            frmSplash objSplash = new frmSplash();
            objSplash.Show();
            objSplash.Refresh();

            frmShortMessage objMain = new frmShortMessage();
            objMain.init();

            objSplash.Close();
            Application.Run(objMain);
        }
    }
}
