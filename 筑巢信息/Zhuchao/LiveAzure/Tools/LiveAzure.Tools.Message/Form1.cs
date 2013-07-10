using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using LiveAzure.Utility;

namespace LiveAzure.Tools.Message
{
    public partial class frmShortMessage : Form
    {

        private Excel.Application objRecordExcel;
        private Excel.Workbooks objRecordBooks;
        private Excel.Workbook objRecordBook;
        private Excel.Sheets objRecordSheets;
        private Excel.Worksheet objRecordSheet1;
        private Excel.Worksheet objRecordSheet2;

        private bool bDirty = true;
        private EucpHelper objEucpHelper;

        private enum MessageDirection { Send, Receive, Balance };

        public frmShortMessage()
        {
            InitializeComponent();
        }

        public void init()
        {
            // 从注册表中读取配置文件
            RegistryKey regMobile = Registry.LocalMachine.OpenSubKey("SOFTWARE\\LiveAzure\\Mobile", false);
            bool bHasConfig = false;
            if (regMobile != null)
            {
                txtSerialNumber.Text = regMobile.GetValue("SerialNumber").ToString();
                txtPassword.Text = regMobile.GetValue("Password").ToString();
                txtSessionKey.Text = regMobile.GetValue("SessionKey").ToString();
                txtStorage.Text = regMobile.GetValue("StorageFile").ToString();
                bHasConfig = true;
            }
            else
            {
                txtStorage.Text = Application.StartupPath + @"\SmsDataBase.xls";
            }
            lblSingleMessage.Text = "";
            lblExcelMessage.Text = "";
            lblGeneralMessage.Text = "";
            lblViewMessage.Text = "";
            if (bHasConfig)
                btnGetMessage_Click(this, null);
            else
            {
                tabControl1.SelectedIndex = 3;
                lblViewMessage.Text = "请先完善配置信息，才能使用";
            }
            PrepareRecord();   // 准备记录文件
        }

        private void frmShortMessage_Load(object sender, EventArgs e)
        {
            // do nothing
        }

        private void frmShortMessage_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 保存配置到注册表中
            try
            {
                RegistryKey regMobile = Registry.LocalMachine.OpenSubKey("SOFTWARE\\LiveAzure\\Mobile", true);
                if (regMobile == null)
                    regMobile = Registry.LocalMachine.CreateSubKey("SOFTWARE\\LiveAzure\\Mobile");
                regMobile.SetValue("SerialNumber", txtSerialNumber.Text);
                regMobile.SetValue("Password", txtPassword.Text);
                regMobile.SetValue("SessionKey", txtSessionKey.Text);
                regMobile.SetValue("StorageFile", txtStorage.Text);
                // 保存记录文件
                // SaveRecord();
                objRecordBook.Close();
                objRecordBooks.Close();
                objRecordExcel.Quit();
                GC.Collect();
            }
            catch { }
        }

        private void txtSerialNumber_TextChanged(object sender, EventArgs e)
        {
            bDirty = true;
        }

        private void txtContent_TextChanged(object sender, EventArgs e)
        {
            if ((sender as TextBox).Tag.ToString() == "SingleContent")
            {
                int nByteLength = Encoding.GetEncoding("gb2312").GetBytes(txtContent.Text).Length;
                if (nByteLength < 970)
                {
                    lblSingleMessage.Text = "短信长度：" + nByteLength.ToString() + " 字节";
                    btnSendSingle.Enabled = true;
                }
                else
                {
                    lblSingleMessage.Text = "短信长度：" + nByteLength.ToString() + " 字节，超过970字节无法发送";
                    btnSendSingle.Enabled = false;
                }
            }
            else if ((sender as TextBox).Tag.ToString() == "GeneralContent")
            {
                int nByteLength = Encoding.GetEncoding("gb2312").GetBytes(txtGeneralContent.Text).Length;
                if (nByteLength < 970)
                {
                    lblGeneralMessage.Text = "短信长度：" + nByteLength.ToString() + " 字节";
                    btnGeneralSend.Enabled = true;
                }
                else
                {
                    lblGeneralMessage.Text = "短信长度：" + nByteLength.ToString() + " 字节，超过970字节无法发送";
                    btnGeneralSend.Enabled = false;
                }
            }
        }

        private void btnBrowseFile_Click(object sender, EventArgs e)
        {
            // 打开Excel文件
            if ((sender as Button).Tag.ToString() == "SendFile")           // 打开批量发送文件
            {
                OpenFileDialog oOpenExcel = new OpenFileDialog();
                oOpenExcel.Filter = "Excel 2003文件(*.xls)|*.xls|Excel 2007文件(*.xlsx)|*.xlsx|所有文件(*.*)|*.*";
                oOpenExcel.Title = "打开批量发送文件";
                if (oOpenExcel.ShowDialog() == DialogResult.OK)
                    txtExcelFile.Text = oOpenExcel.FileName;
            }
            else if ((sender as Button).Tag.ToString() == "GeneralSend")           // 统一群发
            {
                OpenFileDialog oOpenExcel = new OpenFileDialog();
                oOpenExcel.Filter = "Excel 2003文件(*.xls)|*.xls|Excel 2007文件(*.xlsx)|*.xlsx|所有文件(*.*)|*.*";
                oOpenExcel.Title = "打开批量发送地址文件";
                if (oOpenExcel.ShowDialog() == DialogResult.OK)
                    txtGeneralExcel.Text = oOpenExcel.FileName;
            }
            else if ((sender as Button).Tag.ToString() == "StoreFile")     // 打开存储文件
            {
                SaveFileDialog oSaveExcel = new SaveFileDialog();
                oSaveExcel.Title = "保存收件箱和发件箱文件";
                oSaveExcel.Filter = "Excel 2003文件(*.xls)|*.xls|Excel 2007文件(*.xlsx)|*.xlsx|所有文件(*.*)|*.*";
                if (oSaveExcel.ShowDialog() == DialogResult.OK)
                {
                    txtStorage.Text = oSaveExcel.FileName;
                    PrepareRecord();
                }
            }
        }

        private void btnSendSingle_Click(object sender, EventArgs e)
        {
            // 单条发送
            btnSendSingle.Enabled = false;
            lblSingleMessage.Text = "正在发送...";
            this.Refresh();
            int nReturn = -1;
            if (this.Connect() && !String.IsNullOrEmpty(txtReceiver.Text) && !String.IsNullOrEmpty(txtContent.Text))
            {
                nReturn = objEucpHelper.SendSms(txtReceiver.Text.Trim(), txtContent.Text.Trim());
                txtBalance.Text = objEucpHelper.GetBalance().ToString();
                Recording(MessageDirection.Send, txtReceiver.Text.Trim(), txtContent.Text.Trim(), DateTime.Now, nReturn);
                Recording(MessageDirection.Balance, "", txtBalance.Text, DateTime.Now, 100);
            }
            switch (nReturn)
            {
                case -1:
                    lblSingleMessage.Text = "配置参数有误";
                    break;
                case 0:
                    lblSingleMessage.Text = "发送成功";
                    break;
                case 17:
                    lblSingleMessage.Text = "发送失败";
                    break;
                case 18:
                    lblSingleMessage.Text = "发送定时信息失败";
                    break;
                case 101:
                    lblSingleMessage.Text = "客户端网络故障";
                    break;
                case 307:
                    lblSingleMessage.Text = "目标电话号码不符合规则";
                    break;
                default:
                    lblSingleMessage.Text = "无法确定是否发送成功";
                    break;
            }
            SaveRecord();
            btnSendSingle.Enabled = true;
        }

        private void btnSendExcel_Click(object sender, EventArgs e)
        {
            // 按Excel表格（包括手机号和内容）发送
            btnSendExcel.Enabled = false;
            lblExcelMessage.Text = "正在发送...";
            this.Refresh();
            int nRowsCount = 0;
            int nSuccessCount = 0;
            List<int> oFilterRows = new List<int>();
            List<string> oMobileList = new List<string>();
            if (this.Connect() && !String.IsNullOrEmpty(txtExcelFile.Text) && File.Exists(txtExcelFile.Text))
            {
                Excel.Application objExcel = new Excel.Application();
                Excel.Workbooks objWorkbooks = objExcel.Workbooks;
                Excel.Workbook objWorkbook = objWorkbooks.Open(txtExcelFile.Text);
                Excel.Worksheet objWorksheet = objWorkbook.ActiveSheet;
                nRowsCount = objWorksheet.UsedRange.Rows.Count;
                for (int i = 2; i <= nRowsCount; i++)
                {
                    string strMobiles = objWorksheet.Cells[i, 1].Text;
                    if (!String.IsNullOrEmpty(strMobiles))
                    {
                        if (chkExcelDuplicate.Checked)
                        {
                            if (oMobileList.IndexOf(strMobiles) < 0)
                            {
                                oFilterRows.Add(i);
                                oMobileList.Add(strMobiles);
                            }
                        }
                        else
                        {
                            oFilterRows.Add(i);
                        }
                    }
                }
                foreach (int i in oFilterRows)                // for (int i = 2; i <= nRowsCount; i++)
                {
                    string strMobiles = objWorksheet.Cells[i, 1].Text;
                    string strContent = objWorksheet.Cells[i, 2].Text;
                    if (!String.IsNullOrEmpty(strContent))
                    {
                        int nReturn = objEucpHelper.SendSms(strMobiles.Trim(), strContent.Trim());
                        Recording(MessageDirection.Send, strMobiles, strContent, DateTime.Now, nReturn);
                        nSuccessCount++;
                    }
                    lblExcelMessage.Text = "正在发送 " + (i - 1).ToString() + " of " + (nRowsCount - 1).ToString();
                    this.Refresh();
                }
                objWorkbook.Close();
                objWorkbooks.Close();
                objExcel.Quit();
                objWorkbook = null;
                objWorkbooks = null;
                objExcel = null;
                GC.Collect();
                txtBalance.Text = objEucpHelper.GetBalance().ToString();
                Recording(MessageDirection.Balance, "", txtBalance.Text, DateTime.Now, 100);
                lblExcelMessage.Text = "发送结束 " + nSuccessCount.ToString() + " 条";
            }
            else
            {
                lblExcelMessage.Text = "文件不存在或配置参数有误";
            }
            SaveRecord();
            btnSendExcel.Enabled = true;
        }

        private void btnGeneralSend_Click(object sender, EventArgs e)
        {
            // 按Excel表格（仅手机号）发送
            btnGeneralSend.Enabled = false;
            lblGeneralMessage.Text = "正在发送...";
            this.Refresh();
            int nRowsCount = 0;
            int nSuccessCount = 0;
            List<int> oFilterRows = new List<int>();
            List<string> oMobileList = new List<string>();
            if (this.Connect() && !String.IsNullOrEmpty(txtGeneralExcel.Text) && File.Exists(txtGeneralExcel.Text) &&
                !String.IsNullOrEmpty(txtGeneralContent.Text))
            {
                Excel.Application objExcel = new Excel.Application();
                Excel.Workbooks objWorkbooks = objExcel.Workbooks;
                Excel.Workbook objWorkbook = objWorkbooks.Open(txtGeneralExcel.Text);
                Excel.Worksheet objWorksheet = objWorkbook.ActiveSheet;
                nRowsCount = objWorksheet.UsedRange.Rows.Count;
                for (int i = 2; i <= nRowsCount; i++)
                {
                    string strMobiles = objWorksheet.Cells[i, 1].Text;
                    if (!String.IsNullOrEmpty(strMobiles))
                    {
                        if (chkGeneralDuplicate.Checked)
                        {
                            if (oMobileList.IndexOf(strMobiles) < 0)
                            {
                                oFilterRows.Add(i);
                                oMobileList.Add(strMobiles);
                            }
                        }
                        else
                        {
                            oFilterRows.Add(i);
                        }
                    }
                }
                foreach (int i in oFilterRows)        // for (int i = 2; i <= nRowsCount; i++)
                {
                    string strMobiles = objWorksheet.Cells[i, 1].Text;
                    string strContent = txtGeneralContent.Text.Trim();
                    int nReturn = objEucpHelper.SendSms(strMobiles.Trim(), strContent.Trim());
                    Recording(MessageDirection.Send, strMobiles, strContent, DateTime.Now, nReturn);
                    nSuccessCount++;
                    lblGeneralMessage.Text = "正在发送 " + (i - 1).ToString() + " of " + (nRowsCount - 1).ToString();
                    this.Refresh();
                }
                objWorkbook.Close();
                objWorkbooks.Close();
                objExcel.Quit();
                objWorkbook = null;
                objWorkbooks = null;
                objExcel = null;
                GC.Collect();
                txtBalance.Text = objEucpHelper.GetBalance().ToString();
                Recording(MessageDirection.Balance, "", txtBalance.Text, DateTime.Now, 100);
                lblGeneralMessage.Text = "发送结束 " + nSuccessCount.ToString() + " 条";
            }
            else
            {
                lblGeneralMessage.Text = "文件不存在或配置参数有误";
            }
            SaveRecord();
            btnGeneralSend.Enabled = true;
        }

        private void btnViewData_Click(object sender, EventArgs e)
        {
            if (File.Exists(txtStorage.Text))
            {
                lblViewMessage.Text = "";
                Process.Start(txtStorage.Text);
            }
            else
            {
                lblViewMessage.Text = "数据文件不存在";
            }
        }

        private void btnGetMessage_Click(object sender, EventArgs e)
        {
            // 立即接收短信
            btnGetMessage.Enabled = false;
            lblViewMessage.Text = "正在接收短信...";
            this.Refresh();
            int nGetCount = 0;
            try
            {
                if (this.Connect())
                {
                    List<object> oList = objEucpHelper.ReceiveSms();
                    foreach (Dictionary<string, object> oMessage in oList)
                    {
                        Recording(MessageDirection.Receive, oMessage["Mobile"].ToString() , oMessage["Content"].ToString(), (DateTime)oMessage["SentTime"], 0);
                        nGetCount++;
                    }
                    txtBalance.Text = objEucpHelper.GetBalance().ToString();
                    txtPrice.Text = objEucpHelper.GetEachFee().ToString();
                }
            }
            catch (Exception ex)
            {
                lblViewMessage.Text = ex.Message;
            }
            if (nGetCount > 0)
                lblViewMessage.Text = "收到短信 " + nGetCount.ToString() + " 条";
            else
                lblViewMessage.Text = "";
            btnGetMessage.Enabled = true;
        }

        private bool Connect()
        {
            bool bResult = false;
            if (objEucpHelper == null)
                bDirty = true;
            if (bDirty)
            {
                if (!String.IsNullOrEmpty(txtSerialNumber.Text) && !String.IsNullOrEmpty(txtPassword.Text))
                {
                    try
                    {
                        objEucpHelper = new EucpHelper(txtSerialNumber.Text.Trim(), txtPassword.Text.Trim());
                        if (objEucpHelper != null)
                        {
                            bDirty = false;
                            bResult = true;
                        }
                    }
                    catch
                    {
                        bDirty = true;
                        bResult = false;
                    }
                }
            }
            else
            {
                bResult = true;
            }
            return bResult;
        }

        private void PrepareRecord()
        {
            objRecordExcel = new Excel.Application();
            objRecordBooks = objRecordExcel.Workbooks;
            if (File.Exists(txtStorage.Text))
                objRecordBook = objRecordBooks.Open(txtStorage.Text);
            else
                objRecordBook = objRecordBooks.Add(true);
            objRecordSheets = objRecordBook.Sheets;
            objRecordSheet1 = objRecordSheets[1];  // 收件箱
            objRecordSheet2 = objRecordSheets[1];  // 已发短信
            bool bSheet1Exists = false, bSheet2Exists = false;
            foreach (Excel.Worksheet objSheet in objRecordSheets)
            {
                if (objSheet.Name == "收件箱")
                {
                    objRecordSheet1 = objSheet;
                    bSheet1Exists = true;
                }
                else if (objSheet.Name == "已发短信")
                {
                    objRecordSheet2 = objSheet;
                    bSheet2Exists = true;
                }
            }
            if (!bSheet1Exists)
            {
                objRecordSheet1 = objRecordSheets.Add();
                objRecordSheet1.Name = "收件箱";
                objRecordSheet1.Cells[1, 1] = "发件人";
                objRecordSheet1.Cells[1, 2] = "短信内容";
                objRecordSheet1.Cells[1, 3] = "发送时间";
            }
            if (!bSheet2Exists)
            {
                objRecordSheet2 = objRecordSheets.Add();
                objRecordSheet2.Name = "已发短信";
                objRecordSheet2.Cells[1, 1] = "收件人";
                objRecordSheet2.Cells[1, 2] = "短信内容";
                objRecordSheet2.Cells[1, 3] = "发送时间";
                objRecordSheet2.Cells[1, 4] = "结果";
                objRecordSheet2.Cells[1, 5] = "余额";
            }
        }

        private void SaveRecord()
        {
            try
            {
                if (File.Exists(txtStorage.Text))
                    objRecordBook.Save();
                else
                    objRecordBook.SaveAs(txtStorage.Text);
            }
            catch { }
        }

        private void Recording(MessageDirection direction, string mobiles, string content, DateTime sendtime, int result)
        {
            if ((objRecordSheet1 != null) && (objRecordSheet2 != null))
            {
                int nRowsCount1 = objRecordSheet1.UsedRange.Rows.Count;
                int nRowsCount2 = objRecordSheet2.UsedRange.Rows.Count;

                switch (direction)
                {
                    case MessageDirection.Receive:
                        objRecordSheet1.Cells[nRowsCount1 + 1, 1] = "'" + mobiles;
                        objRecordSheet1.Cells[nRowsCount1 + 1, 2] = content;
                        objRecordSheet1.Cells[nRowsCount1 + 1, 3] = "'" + sendtime.ToString();
                        objRecordSheet1.Cells[nRowsCount1 + 1, 1].NumberFormatLocal = "@";  // 文本格式
                        objRecordSheet1.Cells[nRowsCount1 + 1, 2].NumberFormatLocal = "@";
                        objRecordSheet1.Cells[nRowsCount1 + 1, 3].NumberFormatLocal = "@";
                        break;
                    case MessageDirection.Send:
                        objRecordSheet2.Cells[nRowsCount2 + 1, 1] = "'" + mobiles;
                        objRecordSheet2.Cells[nRowsCount2 + 1, 2] = content;
                        objRecordSheet2.Cells[nRowsCount2 + 1, 3] = "'" + sendtime.ToString();
                        objRecordSheet2.Cells[nRowsCount2 + 1, 4] = result;
                        objRecordSheet2.Cells[nRowsCount2 + 1, 1].NumberFormatLocal = "@";
                        objRecordSheet2.Cells[nRowsCount2 + 1, 2].NumberFormatLocal = "@";
                        objRecordSheet2.Cells[nRowsCount2 + 1, 3].NumberFormatLocal = "@";
                        break;
                    case MessageDirection.Balance:
                        objRecordSheet2.Cells[nRowsCount2 + 1, 2] = txtSerialNumber.Text;
                        objRecordSheet2.Cells[nRowsCount2 + 1, 5] = content;
                        objRecordSheet2.Cells[nRowsCount2 + 1, 3] = "'" + sendtime.ToString();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
