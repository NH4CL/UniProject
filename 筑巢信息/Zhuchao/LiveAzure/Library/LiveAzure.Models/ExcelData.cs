using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace LiveAzure.Models
{
    /// <!--刘鑫-->
    /// <summary>
    /// Excel数据操作类
    /// </summary>
    public class ExcelData
    {
        private DataTable _ExcelTable;

        /// <summary>
        /// 获取DataTable
        /// </summary>
        public DataTable ExcelTable
        {
            get { return _ExcelTable; }
        }
        
        /// <summary>
        /// 初始化ExcelData
        /// </summary>
        /// <param name="strExcelFilePath">文件完整路径</param>
        /// <param name="strSheetName">工作簿名称</param>
        public ExcelData(string strExcelFilePath, string strSheetName = "Sheet1")
        {
            ExcelToDataTable(strExcelFilePath, strSheetName);
        }

        /// <summary>
        /// 初始化ExcelData
        /// </summary>
        /// <param name="strExcelFilePath">文件完整路径</param>
        /// <param name="strSheetName">工作簿名称</param>
        private void ExcelToDataTable(string strExcelFilePath, string strSheetName = "Sheet1")
        {
            OleDbConnectionStringBuilder con = new OleDbConnectionStringBuilder
            {
                Provider = "Microsoft.ACE.OLEDB.12.0",
                DataSource = strExcelFilePath
            };
            con.Add("Extended Properties", "Excel 12.0;HDR=YES");
            string strCon = con.ConnectionString;
            string strExcel = string.Format("select * from [{0}$]", strSheetName);
            DataSet ds = new DataSet();

            using (OleDbConnection conn = new OleDbConnection(strCon))
            {
                conn.Open();
                OleDbDataAdapter adapter = new OleDbDataAdapter(strExcel, strCon);
                adapter.Fill(ds, strSheetName);
                conn.Close();
            }
            _ExcelTable = ds.Tables[strSheetName];
        }
    }
}
