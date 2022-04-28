using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Net;
using System.IO;
using System.Web;
using System.Web.Services;
using System.DirectoryServices.ActiveDirectory;
using System.Threading;
/* OrBit的C#插件示例的EXE版本
 * 采用VS-2008-SP1编写
 * 版本号V11.0
 * 本插件适用于OrBit-Browser V11　或以上的版本
 * 提供了各种必要的接口(属性、函数(方法\过程))，开发者可以在此基础上进行改编或完善
 * 由:深圳OrBit Systems Inc. OrBit Team提供
 * 发布日期 2012年9月3日
 * 最后修改 2011年10月23日
 */

namespace CL4100
{
    public partial class pw_Main : Form
    {
        #region ////以下定义OrBit插件与浏览器进行交互的接口变量

        //基本接口变量
        public string PlugInCommand;  //插件的命令ID
        public string PlugInName; //插件的名字(能随语言ID改变)
        public string LanguageId; //语言ID(0-英,1-简,2-繁...8)
        public string ParameterString; //参数字串
        public string RightString; //权限字串(1-Z)
        public string OrBitUserId; //OrBit用户ID
        public string OrBitUserName; //用户名
        public string ApplicationName; //应用系统
        public string ResourceId; //资源(电脑)ID
        public string ResourceName; //资源名
        public bool IsExitQuery; //在插件窗体退出是询问是否要退出，用于提醒数据状态已改变。
        public string UserTicket; //经浏览器认证后的加密票，调用某些WCF服务时需要使用

        //单独调试数据库应用时需用到参数
        public string DatabaseServer; //数据库服务器
        public string DatabaseName;//数据库名
        public string DatabaseUser;//数据库用户
        public string DatabasePassword; //密码

        //各服务器的地址
        public string WcfServerUrl; // WCF或Webservice服务的路径
        public string DocumentServerURL; //文档服务器URL
        public string PluginServerURL;//插件服务器URL
        public string RptReportServerURL; //水晶报表服务器URL

        //回传给浏览器的元对象信息
        public string MetaDataName = "No metadata"; //元对象名
        public string MetaDataVersion = "0.00"; //元对象版本
        public string UIMappingEngineVersion = "0.00"; //UIMapping版本号

        //事件日志类型枚举--1.普通事件，2警告，3错误，4严重错误 ,5表字段变更 ,6其它
        public enum EventLogType { Normal = 1, Warning = 2, Error = 3, FatalError = 4, TableChanged = 5, Other = 6 };

        #endregion

        #region ////以下定义OrBit插件与浏览器交互的接口函数

        /// <summary>
        /// 执行浏览器的命令
        /// </summary>
        /// <param name="command">命令ID</param>
        public void RunCommand(string command)
        {
            try
            {
                Type type = this.ParentForm.GetType();
                //调用没有返回值的方法
                Object obj = this.ParentForm;
                type.InvokeMember("RunCommand", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, obj, new object[] { command });
            }
            catch
            {

            }
        }
        /// <summary>
        /// 将键值写入OrBit的注册表
        /// </summary>
        /// <param name="ApplicationName">应用系统名</param>
        /// <param name="KeyName">键名</param>
        /// <param name="KeyValue">键值</param>
        public void WriteRegistryKey(string ApplicationName, string KeyName, string KeyValue)
        {
            try
            {
                Type type = this.ParentForm.GetType();
                //调用没有返回值的方法
                Object obj = this.ParentForm;
                type.InvokeMember("GetSetRegistryInfomation", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, obj, new object[] { ApplicationName + "." + KeyName, KeyValue, false });
            }
            catch
            {

            }
        }
        /// <summary>
        /// 从OrBit的注册表读取键值
        /// </summary>
        /// <param name="ApplicationName"></param>
        /// <param name="KeyName"></param>
        /// <returns></returns>
        public string ReadRegistryKey(string ApplicationName, string KeyName)
        {  //读取注册表

            try
            {
                Type type = this.ParentForm.GetType();
                //调用没有返回值的方法
                Object obj = this.ParentForm;

                return (string)type.InvokeMember("GetSetRegistryInfomation", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, obj, new object[] { ApplicationName + "." + KeyName, "", true });
            }
            catch
            {
                return "";
            }

        }

        /// <summary>
        /// 采用OrBit-MES的WCF服务器上传文件
        public string UploadFile(string Filename, string SafeFileName, string UploadFilePath)
        {

            try
            {
                Type type = this.ParentForm.GetType();
                //调用没有返回值的方法
                Object obj = this.ParentForm;

                return (string)type.InvokeMember("UploadFile", BindingFlags.InvokeMethod | BindingFlags.Public |
                    BindingFlags.Instance, null, obj, new object[] { Filename, SafeFileName, UploadFilePath });
            }
            catch
            {
                return "";
            }

        }

        /// <summary>
        /// 在内置浏览器中打开一个WebURL
        /// </summary>
        /// <param name="URL"></param>
        public void OpenWebUrl(string URL)
        {
            try
            {
                //用反射的方法直接调用浏览器来浏览文件                
                Type type = this.ParentForm.GetType();
                Object obj = this.ParentForm;
                type.InvokeMember("OpenIEURL", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, obj, new object[] { URL });
            }
            catch
            {

            }
        }

        /// <summary>
        /// 将信息送往浏览器状态条
        /// </summary>
        /// <param name="Message">信息</param>
        public void SendToStatusBar(string Message)
        {
            try
            {
                Type type = this.ParentForm.GetType();
                //调用没有返回值的方法
                Object obj = this.ParentForm;
                type.InvokeMember("SendToStatusBar", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, obj, new object[] { Message });
            }
            catch
            {

            }
        }

        /// <summary>
        /// 更改浏览器进度条
        /// </summary>
        /// <param name="Maximum">最大值</param>
        /// <param name="Value">当前值</param>        
        public void ChangeProgressBar(int Maximum, int Value)
        {
            try
            {
                Type type = this.ParentForm.GetType();
                //调用没有返回值的方法
                Object obj = this.ParentForm;
                type.InvokeMember("ChangeProgressBar", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, obj, new object[] { Maximum, Value });
            }
            catch
            {

            }
        }

        /// <summary>
        /// 写入事件日志-重载1
        /// </summary>
        /// <param name="eventLogType">1.普通事件，2警告，3错误，4严重错误 ,5表字段变更 ,6其它</param>
        /// <param name="eventLog">事件具体内容</param>
        public void WriteToEventLog(string eventLogType, string eventLog)
        {
            if (eventLogType == "1") WriteToEventLog(EventLogType.Normal, eventLog);
            if (eventLogType == "2") WriteToEventLog(EventLogType.Warning, eventLog);
            if (eventLogType == "3") WriteToEventLog(EventLogType.Error, eventLog);
            if (eventLogType == "4") WriteToEventLog(EventLogType.FatalError, eventLog);
            if (eventLogType == "5") WriteToEventLog(EventLogType.TableChanged, eventLog);
            if (eventLogType == "6") WriteToEventLog(EventLogType.Other, eventLog);
        }
        /// <summary>
        /// 写入事件日志-原型
        /// </summary>
        /// <param name="eventLogType">1.普通事件，2警告，3错误，4严重错误 ,5表字段变更 ,6其它</param>
        /// <param name="eventLog">事件具体内容</param>
        public void WriteToEventLog(EventLogType eventLogType, string eventLog)
        {
            try
            {
                Type type = this.ParentForm.GetType();
                //调用没有返回值的方法
                Object obj = this.ParentForm;
                type.InvokeMember("WriteToEventLog", BindingFlags.InvokeMethod | BindingFlags.Public |
                                BindingFlags.Instance, null, obj, new object[] { (int)eventLogType, this.ParentForm.Text, eventLog });
            }
            catch
            {

            }
        }

        //更新一个由客户端传回的记录集
        public bool UpdateDataSetBySQL(DataSet DataSetWithSQL, string SQLString)
        {
            try
            {
                if (this.Parent.Name.ToString() != "FormPlugIn")
                {　//在插件调试环境下运行时，用ADO.NET直连 
                    string ConnectionString = "Data Source=" + DatabaseServer +
                            ";Initial Catalog=" + DatabaseName +
                            ";password=" + DatabasePassword +
                            ";Persist Security Info=True;User ID=" + DatabaseUser;
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {

                        conn.ConnectionString = ConnectionString;
                        conn.Open();
                        SqlDataAdapter da = new SqlDataAdapter(SQLString, conn);
                        SqlCommandBuilder objCommandBuilder = new SqlCommandBuilder(da);
                        da.Update(DataSetWithSQL.Tables[0]);
                        conn.Close();
                        return true;
                    }
                }
                else
                {  //在浏览器下运行时，直接调用浏览器的RunStoredProcedure,

                    Type type = this.ParentForm.GetType();
                    Object obj = this.ParentForm;
                    bool resultR = true;
                    Object[] myArgs = new object[] { DataSetWithSQL, SQLString };
                    resultR = (bool)type.InvokeMember("UpdateDataSetBySQL", BindingFlags.InvokeMethod | BindingFlags.Public |
                                    BindingFlags.Instance, null, obj, myArgs);
                    return resultR;

                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString(), this.ParentForm.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        //创建一个用于执行存储过程的结构
        public DataSet CreateIOParameterDataSet()
        {
            DataSet ds = new DataSet();//声明一个DataSet对象
            ds.Tables.Add(new DataTable()); //添加一个表
            ds.Tables[0].Columns.Add(new DataColumn("FieldName"));
            ds.Tables[0].Columns.Add(new DataColumn("FieldType"));
            ds.Tables[0].Columns.Add(new DataColumn("IsOutput"));
            ds.Tables[0].Columns.Add(new DataColumn("FieldValue"));

            return ds;
        }

        //创建执行存储过程的结构记录集的输入输出参数
        public void CreateIOParameter(DataSet IOParameterDataSet, string FieldName, string FieldType, bool IsOutput, string FieldValue)
        {

            if (FieldType.ToUpper() != "Bit".ToUpper() &&
                FieldType.ToUpper() != "DateTime".ToUpper() &&
                FieldType.ToUpper() != "Decimal".ToUpper() &&
                FieldType.ToUpper() != "Int".ToUpper() &&
                FieldType.ToUpper() != "NVarChar".ToUpper())
                FieldType = "NVarChar";

            bool isUpdate = false;
            for (int i = 0; i < IOParameterDataSet.Tables[0].Rows.Count; i++)
            {
                if (IOParameterDataSet.Tables[0].Rows[i]["FieldName"].ToString().Trim().ToUpper() == FieldName.Trim().ToUpper())
                {
                    IOParameterDataSet.Tables[0].Rows[i]["FieldType"] = FieldType.ToUpper();
                    IOParameterDataSet.Tables[0].Rows[i]["IsOutput"] = IsOutput;
                    IOParameterDataSet.Tables[0].Rows[i]["FieldValue"] = FieldValue;
                    isUpdate = true;
                    break;
                }

            }
            if (isUpdate == false)
            {
                DataRow dr = IOParameterDataSet.Tables[0].NewRow();//添加一个新行对象
                dr["FieldName"] = FieldName;
                dr["FieldType"] = FieldType;
                dr["IsOutput"] = IsOutput;
                dr["FieldValue"] = FieldValue;
                IOParameterDataSet.Tables[0].Rows.Add(dr);  //将新添加的行添加到表中            
            }
            IOParameterDataSet.AcceptChanges();
        }

        //取得输入输出参数的值
        public string GetIOParameterValue(DataSet IOParameterDataSet, string FieldName)
        {
            for (int i = 0; i < IOParameterDataSet.Tables[0].Rows.Count; i++)
            {
                if (IOParameterDataSet.Tables[0].Rows[i]["FieldName"].ToString().Trim().ToUpper() == FieldName.Trim().ToUpper())
                {
                    return IOParameterDataSet.Tables[0].Rows[i]["FieldValue"].ToString();
                }

            }
            return "";
        }

        //更新一个由客户端传回的记录集
        public DataSet ExecSP(string SpName, DataSet IOParameterDataSet, out int Return_value, out  DataSet IOParameterDataSetReturn)
        {
            Return_value = -1;
            IOParameterDataSetReturn = null;
            try
            {
                if (this.Parent.Name.ToString() != "FormPlugIn")
                {　//在插件调试环境下运行时，用ADO.NET直连 
                    DataSet ds = execSPInner(SpName, IOParameterDataSet, out Return_value);
                    IOParameterDataSetReturn = IOParameterDataSet;
                    return ds;
                }
                else
                {  //在浏览器下运行时，直接调用浏览器的RunStoredProcedure,

                    Type type = this.ParentForm.GetType();
                    Object obj = this.ParentForm;
                    DataSet resultR = new DataSet();
                    Object[] myArgs = new object[] { SpName, IOParameterDataSet, Return_value, IOParameterDataSetReturn };
                    resultR = (DataSet)type.InvokeMember("ExecSP", BindingFlags.InvokeMethod | BindingFlags.Public |
                                    BindingFlags.Instance, null, obj, myArgs);
                    IOParameterDataSetReturn = (DataSet)myArgs[3];
                    Return_value = (int)myArgs[2];
                    return resultR;

                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString(), this.ParentForm.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }


        //纯DataSet版本的存储过程执行接口
        private DataSet execSPInner(string SpName, DataSet IOParameterDataSet, out int Return_value)
        {
            SqlCommand cmdDs = new SqlCommand();
            DataSet ReturnDataSet = new DataSet();
            Return_value = 0;
            //将DataSet转化为SqlCommand
            if (IOParameterDataSet != null && IOParameterDataSet.Tables.Count > 0 && IOParameterDataSet.Tables[0].Columns.Count > 0)
            {
                cmdDs.CommandText = SpName;
                for (int i = 0; i < IOParameterDataSet.Tables[0].Rows.Count; i++)
                {
                    bool isOutput = false;
                    string paraName = IOParameterDataSet.Tables[0].Rows[i]["FieldName"].ToString();

                    if (IOParameterDataSet.Tables[0].Rows[i]["IsOutput"].ToString().ToUpper().Trim() == "TRUE")
                    {
                        isOutput = true;
                    }

                    string dataType = IOParameterDataSet.Tables[0].Rows[i]["FieldType"].ToString();

                    if (dataType.ToUpper() == "Bit".ToUpper())
                        cmdDs.Parameters.Add(new SqlParameter(paraName, SqlDbType.Bit));
                    if (dataType.ToUpper() == "DateTime".ToUpper())
                        cmdDs.Parameters.Add(new SqlParameter(paraName, SqlDbType.DateTime));
                    if (dataType.ToUpper() == "Decimal".ToUpper())
                        cmdDs.Parameters.Add(new SqlParameter(paraName, SqlDbType.Decimal));
                    if (dataType.ToUpper() == "Int".ToUpper())
                        cmdDs.Parameters.Add(new SqlParameter(paraName, SqlDbType.Int));
                    if (dataType.ToUpper() == "NVarChar".ToUpper())
                        cmdDs.Parameters.Add(new SqlParameter(paraName, SqlDbType.NVarChar, 1000));


                    cmdDs.Parameters[paraName].Value = IOParameterDataSet.Tables[0].Rows[i]["FieldValue"];
                    if (isOutput == true) cmdDs.Parameters[paraName].Direction = ParameterDirection.Output;


                }

                cmdDs.Parameters.Add("RETURN_VALUE", SqlDbType.Int);
                cmdDs.Parameters["RETURN_VALUE"].Direction = ParameterDirection.ReturnValue;


                // 执行SqlCommand

                string ConnectionString = "Data Source=" + DatabaseServer +
                    ";Initial Catalog=" + DatabaseName +
                    ";password=" + DatabasePassword +
                    ";Persist Security Info=True;User ID=" + DatabaseUser;

                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = ConnectionString;
                conn.Open();

                cmdDs.CommandTimeout = conn.ConnectionTimeout;
                cmdDs.Connection = conn;
                cmdDs.CommandType = CommandType.StoredProcedure;


                DataSet ds = new DataSet("WCFSQLDataSet");
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmdDs;
                adapter.Fill(ds, "WCFSQLDataSet");
                ReturnDataSet = ds;
                conn.Close();

                //将返回值类型的参数送回到DataSet
                for (int i = 0; i < cmdDs.Parameters.Count; i++)
                {
                    if (cmdDs.Parameters[i].Direction.ToString() == "Output")
                    {
                        for (int ii = 0; ii < IOParameterDataSet.Tables[0].Rows.Count; ii++)
                        {

                            if (IOParameterDataSet.Tables[0].Rows[ii]["IsOutput"].ToString().ToUpper().Trim() == "TRUE" &&
                                IOParameterDataSet.Tables[0].Rows[ii]["FieldName"].ToString().ToUpper() == cmdDs.Parameters[i].ToString().ToUpper())
                            {
                                IOParameterDataSet.Tables[0].Rows[ii]["FieldValue"] = cmdDs.Parameters[i].Value;
                            }
                        }

                    }

                    if (cmdDs.Parameters[i].Direction.ToString() == "ReturnValue")
                    {
                        Return_value = (int)cmdDs.Parameters["RETURN_VALUE"].Value;
                    }
                }
                return ReturnDataSet;
            }

            return null;
        }


        /// <summary>
        /// 通用执行存储过程程序.
        /// </summary>
        /// <param name="SQLCmd">传入的SqlCommand对象</param>
        /// <param name="ReturnDataSet">执行存储过程后返回的数据集</param>
        /// <param name="ReturnValue">执行存储过程的返回值</param>
        /// <returns>将SQLCmd执行后的参数刷新并传回，主要返回存储过程中的out参数</returns>
        public SqlCommand RunStoredProcedure(SqlCommand SQLCmd, out DataSet ReturnDataSet, out int ReturnValue)
        {
            ReturnValue = 0;
            try
            {
                if (this.Parent.Name.ToString() != "FormPlugIn")
                {　//在插件调试环境下运行时，用ADO.NET直连 
                    string ConnectionString = "Data Source=" + DatabaseServer +
                            ";Initial Catalog=" + DatabaseName +
                            ";password=" + DatabasePassword +
                            ";Persist Security Info=True;User ID=" + DatabaseUser;
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        conn.Open();
                        SQLCmd.Connection = conn;
                        SQLCmd.CommandType = CommandType.StoredProcedure;
                        SQLCmd.CommandTimeout = conn.ConnectionTimeout;
                        SQLCmd.Parameters.Add("RETURN_VALUE", SqlDbType.Int);
                        SQLCmd.Parameters["RETURN_VALUE"].Direction = ParameterDirection.ReturnValue;

                        SqlDataAdapter adapter = new SqlDataAdapter();
                        adapter.SelectCommand = SQLCmd;

                        DataSet ds = new DataSet("WCFSQLDataSet");
                        adapter.Fill(ds, "WCFSQLDataSet");

                        ReturnDataSet = ds;
                        conn.Close();
                        ReturnValue = (int)SQLCmd.Parameters["RETURN_VALUE"].Value;
                        return SQLCmd;
                    }
                }
                else
                {  //在浏览器下运行时，直接调用浏览器的RunStoredProcedure,

                    Type type = this.ParentForm.GetType();
                    Object obj = this.ParentForm;

                    SqlCommand cmd = new SqlCommand();
                    DataSet rds = new DataSet();

                    ReturnDataSet = null;

                    Object[] myArgs = new object[] { SQLCmd, ReturnDataSet, ReturnValue };
                    cmd = (SqlCommand)type.InvokeMember("RunStoredProcedure", BindingFlags.InvokeMethod | BindingFlags.Public |
                                    BindingFlags.Instance, null, obj, myArgs);
                    ReturnValue = (int)myArgs[2];
                    ReturnDataSet = (DataSet)myArgs[1];
                    return cmd;

                }
            }
            catch (Exception er)
            {
                ReturnDataSet = null;
                MessageBox.Show(er.ToString(), this.ParentForm.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

        }

        /// <summary>
        /// 通用执行存储过程程序，并支侍向存储过程上传数据集,需SQL-Server 2008对表值类型传入参数的支持
        /// </summary>
        /// <param name="SQLCmd">传入的SqlCommand对象</param>
        /// <param name="UserDataSet">用于上传数据的UserDataSet</param>
        /// <param name="ReturnDataSet">执行存储过程后返回的数据集</param>
        /// <param name="ReturnValue">执行存储过程的返回值</param>
        /// <returns>将SQLCmd执行后的参数刷新并传回，主要返回存储过程中的out参数</returns>
        public SqlCommand RunSPUploadDataSet(SqlCommand SQLCmd, DataSet UserDataSet, out DataSet ReturnDataSet, out int ReturnValue)
        {
            ReturnValue = 0;
            try
            {
                if (this.Parent.Name.ToString() != "FormPlugIn")
                {　//在插件调试环境下运行时，用ADO.NET直连 
                    string ConnectionString = "Data Source=" + DatabaseServer +
                            ";Initial Catalog=" + DatabaseName +
                            ";password=" + DatabasePassword +
                            ";Persist Security Info=True;User ID=" + DatabaseUser;
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        conn.Open();
                        SQLCmd.Connection = conn;
                        SQLCmd.CommandType = CommandType.StoredProcedure;
                        SQLCmd.CommandTimeout = conn.ConnectionTimeout;
                        SQLCmd.Parameters.Add("RETURN_VALUE", SqlDbType.Int);
                        SQLCmd.Parameters["RETURN_VALUE"].Direction = ParameterDirection.ReturnValue;

                        //处理一次性从客户端传过来的DataSet，并将它的Table名作为参数名使用
                        if (UserDataSet != null && UserDataSet.Tables.Count > 0)
                        {
                            for (int k = 0; k < UserDataSet.Tables.Count; k++)
                            {
                                string tableName = UserDataSet.Tables[k].TableName.ToString().Trim();
                                if (tableName != "")
                                {
                                    SQLCmd.Parameters.AddWithValue("@" + tableName, UserDataSet.Tables[k]);
                                }
                            }
                        }

                        SqlDataAdapter adapter = new SqlDataAdapter();
                        adapter.SelectCommand = SQLCmd;

                        DataSet ds = new DataSet("WCFSQLDataSet");
                        adapter.Fill(ds, "WCFSQLDataSet");

                        ReturnDataSet = ds;
                        conn.Close();
                        ReturnValue = (int)SQLCmd.Parameters["RETURN_VALUE"].Value;
                        return SQLCmd;
                    }
                }
                else
                {  //在浏览器下运行时，直接调用浏览器的RunStoredProcedure,

                    Type type = this.ParentForm.GetType();
                    Object obj = this.ParentForm;

                    SqlCommand cmd = new SqlCommand();
                    DataSet rds = new DataSet();

                    ReturnDataSet = null;

                    Object[] myArgs = new object[] { SQLCmd, UserDataSet, ReturnDataSet, ReturnValue };
                    cmd = (SqlCommand)type.InvokeMember("RunSPUploadDataSet", BindingFlags.InvokeMethod | BindingFlags.Public |
                                    BindingFlags.Instance, null, obj, myArgs);
                    ReturnValue = (int)myArgs[3];
                    ReturnDataSet = (DataSet)myArgs[2];
                    return cmd;

                }
            }
            catch (Exception er)
            {
                ReturnDataSet = null;
                MessageBox.Show(er.ToString(), this.ParentForm.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

        }

        /// <summary>
        /// //为插件提供可以任意一个WCF的地址上直接调用任意存储过程的接口,并支持上传数据集UserDataSet。
        /// </summary>
        /// <param name="WCFUrl">一个任意的WCFUrl地址，需要完整的写出*.svc</param>
        /// <param name="SQLCmd">传入的SqlCommand对象</param>
        /// <param name="UserDataSet">用于上传数据的UserDataSet</param>
        /// <param name="ReturnDataSet">执行存储过程后返回的数据集</param>
        /// <param name="ReturnValue">执行存储过程的返回值</param>
        /// <returns>将SQLCmd执行后的参数刷新并传回，主要返回存储过程中的out参数</returns>
        public SqlCommand RunSPUploadDataSetByWCFUrl(string WCFUrl, SqlCommand SQLCmd, DataSet UserDataSet, out DataSet ReturnDataSet, out int ReturnValue)
        {
            ReturnValue = 0;
            try
            {
                if (this.Parent.Name.ToString() != "FormPlugIn")
                {　//在插件调试环境下运行时，用ADO.NET直连 
                    MessageBox.Show("本函数必须在OrBit浏览器环境下执行！");
                    ReturnDataSet = null;
                    return null;

                }
                else
                {  //在浏览器下运行时，直接调用浏览器的RunStoredProcedure,

                    Type type = this.ParentForm.GetType();
                    Object obj = this.ParentForm;

                    SqlCommand cmd = new SqlCommand();
                    DataSet rds = new DataSet();

                    ReturnDataSet = null;

                    Object[] myArgs = new object[] { WCFUrl, SQLCmd, UserDataSet, ReturnDataSet, ReturnValue };
                    cmd = (SqlCommand)type.InvokeMember("RunSPUploadDataSetByWCFUrl", BindingFlags.InvokeMethod | BindingFlags.Public |
                                    BindingFlags.Instance, null, obj, myArgs);
                    ReturnValue = (int)myArgs[4];
                    ReturnDataSet = (DataSet)myArgs[3];
                    return cmd;

                }
            }
            catch (Exception er)
            {
                ReturnDataSet = null;
                MessageBox.Show(er.ToString(), this.ParentForm.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

        }


        /// <summary>
        /// 执行一个指定的SQL字串，并从OLAP服务器上返回一个记录集
        /// 在浏览器下执行时，直接调用浏览器的WCF服务器来传送记录集
        /// </summary>
        /// <param name="SQLString">SQL字串</param>
        /// <returns>返回的记录集</returns>
        public DataSet GetDataSetWithSQLString(string SQLString)
        {
            try
            {

                //if (this.Parent == null || this.Parent.Name.ToString() != "pw_Main")//
#if DEBUG
                {　//在插件调试环境下运行时，用ADO.NET直连 
                    string ConnectionString = "Data Source=" + DatabaseServer +
                                        ";Initial Catalog=" + DatabaseName +
                                        ";password=" + DatabasePassword +
                                        ";Persist Security Info=True;User ID=" + DatabaseUser;
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        conn.Open();
                        SqlCommand comm = new SqlCommand();
                        comm.Connection = conn;
                        comm.CommandText = SQLString;

                        comm.CommandType = CommandType.Text;
                        comm.CommandTimeout = conn.ConnectionTimeout;

                        DataSet ds = new DataSet("SQLDataSet");
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        adapter.SelectCommand = comm;
                        adapter.Fill(ds, "SQLDataSet");

                        conn.Close();
                        return ds;
                    }
                }
#else
                {　 //在浏览器下运行时，直接调用浏览器的GetDataSetWithSQLString,
                    //通过WCF服务器返回记录集

                    Type type = this.ParentForm.GetType();
                    Object obj = this.ParentForm;
                    DataSet ds = new DataSet("SQLDataSet");
                    ds = (DataSet)type.InvokeMember("GetDataSetWithSQLString", BindingFlags.InvokeMethod | BindingFlags.Public |
                                    BindingFlags.Instance, null, obj, new object[] { SQLString });
                    return ds;

                }
#endif
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }


        // GetUIText提供了几种形式的重载，以方便使用

        /// <summary>
        /// 返回控件或提示信息不同语种的内容-重载4
        /// </summary>
        /// <param name="module">模块名</param>
        /// <returns></returns>
        public String GetUIText(string module)
        {
            string s = GetUIText("[Public Text]", module, "", "").Trim();
            if (s == string.Empty)
            {
                return module;
            }
            else
            {
                return s;
            }

        }
        /// <summary>
        /// 返回控件或提示信息不同语种的内容-重载3
        /// </summary>
        /// <param name="module">模块名</param>
        /// <param name="defaultText">默认文本,必须是英文</param>
        /// <returns></returns>
        public string GetUIText(string module, string defaultText)
        {
            string s = GetUIText("[Public Text]", module, "", defaultText).Trim();
            if (s == string.Empty)
            {
                return defaultText;
            }
            else
            {
                return s;
            }
        }
        /// <summary>
        /// 返回控件或提示信息不同语种的内容-重载2
        /// </summary>
        /// <param name="module">模块名</param>
        /// <param name="control">控件对象</param>
        /// <returns></returns>
        public string GetUIText(string module, Control control)
        {
            string s = GetUIText(this.ParentForm.Text, module, control.Name, control.Text);
            if (s == string.Empty)
            {
                return control.Text;
            }
            else
            {
                return s;
            }

        }
        /// <summary>
        /// 返回控件或提示信息不同语种的内容-重载1
        /// </summary>
        /// <param name="module">模块名</param>
        /// <param name="controlName">控件名</param>
        /// <param name="defaultText">默认文本,必须是英文</param>
        /// <returns></returns>
        public string GetUIText(string module, string controlName, string defaultText)
        {
            string s = GetUIText(this.ParentForm.Text, controlName, defaultText);
            if (s == string.Empty)
            {
                return defaultText;
            }
            else
            {
                return s;
            }
        }

        /// <summary>
        /// 返回控件或提示信息不同语种的内容-基本型式
        /// </summary>
        /// <param name="owner">所有者</param>
        /// <param name="module">模块名</param>
        /// <param name="controlName">控件名</param>
        /// <param name="defaultText">默认文本,必须是英文</param>
        /// <returns></returns>
        public string GetUIText(string owner, string module, string controlName, string defaultText)
        {
            try
            {
                Type type = this.ParentForm.GetType();
                //调用没有返回值的方法
                Object obj = this.ParentForm;
                string s = (string)type.InvokeMember("GetUIText", BindingFlags.InvokeMethod | BindingFlags.Public |
                                BindingFlags.Instance, null, obj, new object[] { owner, module, controlName, defaultText });
                if (s != null)
                {
                    return s;
                }
                else
                {
                    return defaultText;
                }
            }
            catch
            {
                return defaultText;
            }

        }


        #endregion

        #region 下载文件列表
        private string m_PrjDirectory = "CL4102_12\\";//项目目录，方便拓展
        private string[] m_DLLFileArray = new string[] 
                        {   

                            "CL4102_12//Frontier.MeterVerification.Communication.dll",
                            "CL4102_12//Frontier.MeterVerification.DeviceCommon.dll",
                            "CL4102_12//Frontier.MeterVerification.DeviceInterface.dll",
                            "CL4102_12//Frontier.MeterVerification.KLDevice.dll",
                            "CL4102_12//VerificationEquipment.Commons.dll",

                            "CL4102_12//pwAmMeterController.dll",
                            "CL4102_12//pwClassLibrary.dll", 
                            "CL4102_12//pwComPorts.dll",
                            "CL4102_12//pwFunction.dll",
                            "CL4102_12//pwInterface.dll", 

                            "CL4102_12//pwMainControl.dll",
                            "CL4102_12//pwMeterProtocol.dll",
                            "CL4102_12//pwResource.dll",
                            "CL4102_12//CL4102_12.exe",

                            "CL4102_12//pwCollapseDataGridView.dll",
                            "CL4102_12//OrBitADCService.dll",
                            "CL4102_12//Interop.ADOX.dll",
                            "CL4102_12//PropertyGridEx.dll", 
                            "CL4102_12//VBFloat.dll",
                            "CL4102_12//CL4101.ico",
                            "CL4102_12//DeviceConfig.txt",
                            "CL4102_12//xIb.xml",
                            "CL4102_12//GLYS.xml", 
                            "CL4102_12//ClMeterComPort.xml",
                            "CL4102_12//white.gif",
                            "CL4102_12//wait2.gif",
                            "CL4102_12//wait.gif",
                            "CL4102_12//red.gif",
                            "CL4102_12//blue.gif",
                            //"CL4101_12/123.txt",
                            "CL4102_12//System//System.xml",                            
                            "CL4102_12//WorkPlan//WorkPlan.xml",
                            "CL4102_12//Comm", 
                        };

        #endregion

        private ListView m_listView = new ListView();                       //



        /// <summary>
        /// 本私有函数对插件各接口变量进行初始化，赋予默认值
        /// 调试环境下这些值不变，通过浏览器执行时，
        /// 这些变量将会根据系统环境被重新赋值。
        /// </summary>
        private void initializeVariable()
        {
            PlugInCommand = "MYPGN";
            PlugInName = "我的插件";
            LanguageId = "0";  //(0-英,1-简,2-繁...8)
            ParameterString = "";
            RightString = "(0)";

            OrBitUserId = "DEVUSER";
            OrBitUserName = "调试者";
            ApplicationName = "DEBUG";
            ResourceId = "RES0000XXX";
            ResourceName = "YourPC";

            //这里需要根据实际的数据库环境进行改写
            DatabaseServer = "HenryX200";
            DatabaseName = "OrBitX";
            DatabaseUser = "SA";
            DatabasePassword = "sasasa";

            WcfServerUrl = "http://localhost/WCFService";
            DocumentServerURL = ""; //文档服务器URL
            PluginServerURL = "http://10.98.99.6:800//OrBitPlugins";//插件服务器URL
            RptReportServerURL = "http://henryx61/RptExamples/"; //水晶报表服务器URL

            UserTicket = "";
            IsExitQuery = false;
        }

        public pw_Main()
        {
            InitializeComponent();
            initializeVariable(); //插件变量初始化

            label1.Text = "更新下载提示：" + "\r\n" + "\r\n";
            label1.Text += "1：完全退出MES系统。" + "\r\n";
            label1.Text += "2：登入MES系统。" + "\r\n";
            label1.Text += "3：更新下载。" + "\r\n";
            label1.Text += "4：打开配置或测试程序。" + "\r\n";
        }


        private void tsbtnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsExitQuery == true)
            {
                if (MessageBox.Show(
                         GetUIText("Do you want to exist?")
                         , this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private static DataTable UDatatable()
        {
            DataTable dt = new DataTable("dt");
            DataColumn[] dtc = new DataColumn[6];
            dtc[0] = new DataColumn("_LsId", System.Type.GetType("System.String"));
            dtc[1] = new DataColumn("Product", System.Type.GetType("System.String"));
            dtc[2] = new DataColumn("Base", System.Type.GetType("System.String"));
            dtc[3] = new DataColumn("NGReason", System.Type.GetType("System.String"));
            dtc[4] = new DataColumn("Qty", System.Type.GetType("System.String"));
            dtc[5] = new DataColumn("A", System.Type.GetType("System.String"));
            dt.Columns.AddRange(dtc); return dt;
        }


        private void Form1_Shown(object sender, EventArgs e)
        {
            // PluginServerURL = "http://10.98.99.6:800//OrBitPlugins";           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {

                bool bolDown = false;
                listView1.Items.Clear();
                progressBar1.Minimum = 0;
                progressBar1.Maximum = m_DLLFileArray.Length;
                progressBar1.Visible = true;
                for (int i = 0; i < m_DLLFileArray.Length; i++)
                {
                    if (chk_DownDLL.Checked && i >= 14) break;

                    progressBar1.Value = i;
                    string tmp_DLLFile = m_DLLFileArray[i].Replace("//", "\\");
                    tmp_DLLFile = tmp_DLLFile.Replace(m_PrjDirectory, "");
                    string tmp_Directory = System.Windows.Forms.Application.StartupPath + "\\" + tmp_DLLFile;
                    if (tmp_Directory.IndexOf("System\\") > 0 || tmp_Directory.IndexOf("WorkPlan\\") > 0)
                    {
                        tmp_Directory = tmp_Directory.Substring(0, tmp_Directory.LastIndexOf('\\'));
                        if (!Directory.Exists(tmp_Directory)) Directory.CreateDirectory(tmp_Directory);
                    }
                    if (tmp_Directory.IndexOf("\\Comm") > 0)
                    {
                        // tmp_Directory = tmp_Directory.Substring(0, tmp_Directory.LastIndexOf('\\'));
                        if (!Directory.Exists(tmp_Directory)) Directory.CreateDirectory(tmp_Directory);
                    }

                    if (m_DLLFileArray[i].Contains("."))
                    {
                        bolDown = pwDownLoadFile(PluginServerURL + "//" + m_DLLFileArray[i], System.Windows.Forms.Application.StartupPath + "\\" + tmp_DLLFile);
                    }
                    else
                    {
                        if (Directory.Exists(tmp_Directory))
                        {
                            bolDown = true;
                        }
                        else
                        {
                            bolDown = false;
                        }
                    }
                    ListViewItem lv = new ListViewItem(Convert.ToString(i + 1));
                    lv.SubItems.Add(m_DLLFileArray[i]);
                    lv.SubItems.Add(DateTime.Now.ToShortTimeString());
                    lv.SubItems.Add(bolDown ? "成功" : "失败");
                    listView1.Items.Add(lv);
                    listView1.Items[listView1.Items.Count - 1].EnsureVisible();
                    //if (bolDown == false)
                    //{
                    //    listView1.Items[listView1.Items.Count - 1].SubItems[3].ForeColor = System.Drawing.Color.Red;
                    //}
                    Application.DoEvents();
                    listView1.Refresh();
                    Thread.Sleep(50);

                    if (m_DLLFileArray[i] == "CL4102_12//DeviceConfig.txt")
                    {
                        string oldname = System.Windows.Forms.Application.StartupPath + "\\" + tmp_DLLFile;
                        string newname = oldname.Split('.')[0] + ".ini";
                        if (File.Exists(newname)) File.Delete(newname);
                        File.Copy(oldname, newname);
                        File.Delete(oldname);
                    }
                }
                progressBar1.Value = progressBar1.Maximum;
                progressBar1.Visible = false;
            }
            catch (SystemException errror)
            {
                MessageBox.Show(errror.Message);
            }
        }

        /// <summary>
        /// 从服务器下载文件到本地
        /// </summary>
        /// <param name="url">服务器路径</param>
        /// <param name="FileName">本地路径</param>
        /// <returns></returns>
        public bool pwDownLoadFile(String url, String FileName)
        {
            try
            {
                string md5S = GetMd5Hash(FileName);

                FileStream outputStream = new FileStream(FileName, FileMode.OpenOrCreate);
                WebRequest request = WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream httpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 1024*10;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = httpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = httpStream.Read(buffer, 0, bufferSize);
                }
                //if (outputStream.Length != cl)
                //{
                //    MessageBox.Show(FileName + "文件大小不符，文件下载失败！");
                //    httpStream.Close();
                //    outputStream.Close();
                //    response.Close();
                //    return false;
                //}
                //else
                //{
                    httpStream.Close();
                    outputStream.Close();
                    response.Close();
                    return true;
                //}

            }
            catch (Exception ex)
            {
                MessageBox.Show("文件下载失败错误为" + ex.Message.ToString(), "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            
        }

        private void chk_DownDLL_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_DownDLL.Checked == true)
            {
                chk_DownALL.Checked = false;
            }
            else
            {
                chk_DownALL.Checked = true;
            }
        }

        private void chk_DownALL_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_DownALL.Checked == true)
            {
                chk_DownDLL.Checked = false;
            }
            else
            {
                chk_DownDLL.Checked = true;
            }
        }
        private string GetMd5Hash(string pathName)
        {
            string strResult = "";
            string strHashData = "";
            byte[] arrbytHashValue;

            System.IO.FileStream oFileStream = null;

            System.Security.Cryptography.MD5CryptoServiceProvider oMD5Hasher =
                new System.Security.Cryptography.MD5CryptoServiceProvider();

            try
            {
                oFileStream = new System.IO.FileStream(pathName, System.IO.FileMode.Open,
                                                       System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);

                arrbytHashValue = oMD5Hasher.ComputeHash(oFileStream); //计算指定Stream 对象的哈希值

                oFileStream.Close();

                //由以连字符分隔的十六进制对构成的String，其中每一对表示value 中对应的元素；例如“F-2C-4A”

                strHashData = System.BitConverter.ToString(arrbytHashValue);

                //替换-
                strHashData = strHashData.Replace("-", "");

                strResult = strHashData;
            }

            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return strResult;
        }


    }
}
