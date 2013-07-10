using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using MySql.Data.MySqlClient;

namespace LiveAzure.Tools.MySqlUtil
{
    #region 数据库操作封装类
    
    /// <summary>
    /// 仅用于处理MySQL数据库，暂不移植到Utility中
    /// </summary>
    public class MySqlHelper
    {
        private MySqlConnection  oConnection;
        private MySqlCommand oCommand;
        private ArrayList sOutParm;

        /// <summary>
        /// 数据库操作对象
        /// </summary>
        /// <param name="sConnectionString">数据库连接字符串</param>
        public MySqlHelper(string sConnectionString)
        {
            oConnection = new MySqlConnection();
            oConnection.ConnectionString = sConnectionString;
            oCommand = new MySqlCommand();
            oCommand.Connection = oConnection;
            sOutParm = new ArrayList();
        }

        /// <summary>
        /// 返回DataSet
        /// </summary>
        /// <param name="sCommandText">Sql文本</param>
        /// <returns></returns>
        public DataSet ExecuteDataset(string sCommandText)
        {
            DataSet ds = new DataSet();
            MySqlDataAdapter objAdapter = new MySqlDataAdapter();
            oCommand.CommandText = sCommandText;
            oCommand.CommandType = CommandType.Text;
            objAdapter.SelectCommand = oCommand;
            try
            {
                oConnection.Open();
                objAdapter.Fill(ds);
            }
            catch (Exception ex)
            {
                HandleExceptions(ex);
            }
            finally
            {
                oConnection.Close();
                oCommand.Parameters.Clear();
            }
            return ds;
        }

        /// <summary>
        /// 返回DataTable
        /// </summary>
        /// <param name="sCommandText">Sql文本</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string sCommandText)
        {
            DataTable oDataTable = new DataTable();
            DataSet ds = new DataSet();
            MySqlDataAdapter objAdapter = new MySqlDataAdapter();
            oCommand.CommandText = sCommandText;
            oCommand.CommandType = CommandType.Text;
            objAdapter.SelectCommand = oCommand;
            try
            {
                oConnection.Open();
                objAdapter.Fill(ds);
            }
            catch (Exception ex)
            {
                HandleExceptions(ex);
            }
            finally
            {
                oConnection.Close();
                oCommand.Parameters.Clear();
            }
            if (ds.Tables.Count > 0)
            {
                oDataTable = ds.Tables[0];
            }
            return oDataTable;
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行
        /// </summary>
        /// <param name="sCommandText">Sql文本</param>
        /// <returns></returns>
        public object ExecuteScalar(string sCommandText)
        {
            object obj = null;
            oCommand.CommandText = sCommandText;
            oCommand.CommandType = CommandType.Text;
            try
            {
                oConnection.Open();
                obj = oCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                HandleExceptions(ex);
            }
            finally
            {
                oConnection.Close();
                oCommand.Parameters.Clear();
            }
            return obj;
        }

        /// <summary>
        /// 返回SqlDataReader,注意在使用后请关闭本对象
        /// </summary>
        /// <param name="sCommandText">Sql文本</param>
        /// <returns></returns>
        public MySqlDataReader ExecuteReader(string sCommandText)
        {
            MySqlDataReader oReader = null;
            oCommand.CommandText = sCommandText;
            oCommand.CommandType = CommandType.Text;
            try
            {
                oConnection.Open();
                oReader = oCommand.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                try
                {
                    oReader.Close();
                }
                catch
                {
                    oConnection.Close();
                }
                HandleExceptions(ex);
            }
            finally
            {
                oCommand.Parameters.Clear();
            }
            return oReader;
        }

        /// <summary>
        /// 返回SqlDataReader(SingleRow),注意在使用后请关闭本对象
        /// </summary>
        /// <param name="sCommandText">Sql文本</param>
        /// <returns></returns>
        public MySqlDataReader ExecuteReaderSingleRow(string sCommandText)
        {
            MySqlDataReader oReader = null;
            oCommand.CommandText = sCommandText;
            oCommand.CommandType = CommandType.Text;
            try
            {
                oConnection.Open();
                oReader = oCommand.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleRow);
            }
            catch (Exception ex)
            {
                try
                {
                    oReader.Close();
                }
                catch
                {
                    oConnection.Close();
                }
                HandleExceptions(ex);
            }
            finally
            {
                oCommand.Parameters.Clear();
            }
            return oReader;
        }

        /// <summary>
        /// 执行ExecuteNonQuery
        /// </summary>
        /// <param name="sCommandText">Sql文本</param>
        /// <returns></returns>
        public void ExecuteNonQuery(string sCommandText)
        {
            oCommand.CommandText = sCommandText;
            oCommand.CommandType = CommandType.Text;
            try
            {
                oConnection.Open();
                oCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                HandleExceptions(ex);
            }
            finally
            {
                oConnection.Close();
                oCommand.Parameters.Clear();
            }
        }

        /// <summary>
        /// 返回存储过程DataSet
        /// </summary>
        /// <param name="sCommandText">Sql文本</param>
        /// <param name="htParmList">存储过程返回参数</param>
        /// <returns></returns>
        public DataSet SPExecuteDataset(string sCommandText, out Hashtable htParmList)
        {
            htParmList = new Hashtable();
            DataSet oDataSet = new DataSet();
            MySqlDataAdapter oDataAdapter = new MySqlDataAdapter();
            oCommand.CommandText = sCommandText;
            oCommand.CommandType = CommandType.StoredProcedure;
            oDataAdapter.SelectCommand = oCommand;
            try
            {
                oConnection.Open();
                oDataAdapter.Fill(oDataSet);
                foreach (string sKey in sOutParm)
                    htParmList[sKey] = oCommand.Parameters[sKey].Value;
            }
            catch (Exception ex)
            {
                HandleExceptions(ex);
            }
            finally
            {
                oConnection.Close();
                oCommand.Parameters.Clear();
                sOutParm.Clear();
            }
            return oDataSet;
        }

        /// <summary>
        /// 返回存储过程DataTable
        /// </summary>
        /// <param name="sCommandText">Sql文本</param>
        /// <param name="htParmList">存储过程返回参数</param>
        /// <returns></returns>
        public DataTable SPExecuteDataTable(string sCommandText, out Hashtable htParmList)
        {
            htParmList = new Hashtable();
            DataTable oDataTable = new DataTable();
            DataSet oDataSet = new DataSet();
            MySqlDataAdapter oDataAdapter = new MySqlDataAdapter();
            oCommand.CommandText = sCommandText;
            oCommand.CommandType = CommandType.StoredProcedure;
            oDataAdapter.SelectCommand = oCommand;
            try
            {
                oConnection.Open();
                oDataAdapter.Fill(oDataSet);
                foreach (string sKey in sOutParm)
                    htParmList[sKey] = oCommand.Parameters[sKey].Value;
            }
            catch (Exception ex)
            {
                HandleExceptions(ex);
            }
            finally
            {
                oConnection.Close();
                oCommand.Parameters.Clear();
                sOutParm.Clear();
            }
            if (oDataSet.Tables.Count > 0)
                oDataTable = oDataSet.Tables[0];
            return oDataTable;
        }

        /// <summary>
        /// 返回存储过程SqlDataReader,注意在使用后请关闭本对象(不支持返回参数)
        /// </summary>
        /// <param name="sCommandText">Sql文本</param>
        /// <returns></returns>
        public MySqlDataReader SPExecuteReader(string sCommandText)
        {
            MySqlDataReader oDataReader = null;
            oCommand.CommandText = sCommandText;
            oCommand.CommandType = CommandType.StoredProcedure;
            try
            {
                oConnection.Open();
                oDataReader = oCommand.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                try
                {
                    oDataReader.Close();
                }
                catch
                {
                    oConnection.Close();
                }
                HandleExceptions(ex);
            }
            finally
            {
                oCommand.Parameters.Clear();
            }
            return oDataReader;
        }

        /// <summary>
        /// 执行存储过程ExecuteNonQuery
        /// </summary>
        /// <param name="sCommandText">Sql文本</param>
        /// <param name="htParmList">存储过程返回参数</param>
        /// <returns></returns>
        public void SPExecuteNonQuery(string sCommandText, out Hashtable htParmList)
        {
            htParmList = new Hashtable();
            oCommand.CommandText = sCommandText;
            oCommand.CommandType = CommandType.StoredProcedure;
            try
            {
                oConnection.Open();
                oCommand.ExecuteNonQuery();
                foreach (string sKey in sOutParm)
                    htParmList[sKey] = oCommand.Parameters[sKey].Value;
            }
            catch (Exception ex)
            {
                HandleExceptions(ex);
            }
            finally
            {
                oConnection.Close();
                oCommand.Parameters.Clear();
                sOutParm.Clear();
            }
        }

        /// <summary>
        /// 添加Parameter对象
        /// </summary>
        /// <returns></returns>
        public void AddCommandParameter(MySqlParameter oSqlParameter)
        {
            oCommand.Parameters.Add(oSqlParameter);
            if (oSqlParameter.Direction == ParameterDirection.Output)
                sOutParm.Add(oSqlParameter.ParameterName);
        }

        /// <summary>
        /// 异常操作
        /// </summary>
        /// <param name="ex">异常</param>
        private void HandleExceptions(Exception ex)
        {
            throw ex;
        }
    }
    
    #endregion
}
