using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using pwCollapseDataGridView;
using pwFunction.pwConst;
using pwInterface;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;
using System.Net;
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
        /// 执行一个指定的SQL字串，并返回一个记录集
        /// 在浏览器下执行时，直接调用浏览器的WCF服务器来传送记录集
        /// </summary>
        /// <param name="SQLString">SQL字串</param>
        /// <returns>返回的记录集</returns>
        public DataSet GetDataSetWithSQLString(string SQLString)
        {
            try
            {
                if (this.Parent==null||this.Parent.Name.ToString() != "FormPlugIn")
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
                else
                {　//在浏览器下运行时，直接调用浏览器的GetDataSetWithSQLString,
                    //通过WCF服务器返回记录集

                    Type type = this.ParentForm.GetType();
                    Object obj = this.ParentForm;
                    DataSet ds = new DataSet("SQLDataSet");
                    ds = (DataSet)type.InvokeMember("GetDataSetWithSQLString", BindingFlags.InvokeMethod | BindingFlags.Public |
                                    BindingFlags.Instance, null, obj, new object[] { SQLString });
                    return ds;

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        #endregion

        #region 查询当前生产编号是否在范围，并且前工序是否OK--调用存储过程执行
        public bool Sql_CheckLotSpecificationDoMethod(string S_ProductionID, ref string Errorinfo)
        {
            try
            {
                bool bt = true;

                //@lotsn nvarchar(50)='', --生产编号
                //@Stationname nvarchar(50)=''--测试台体编号
                //@MOName nvarchar(50)=''--工单号

                //string StrSQL = @"exec CL_loadingProduction_CheckLotSpecificationDoMethod " +
                string StrSQL = @"exec CL_loadingProduction_Overseas_ThreePhase_CheckLotSpecificationDoMethod " +

                               "@lotsn='" + S_ProductionID + "'," +                                 //--生产编号
                               "@Stationname='" + GlobalUnit.g_DeskNo + "'," +                      //--台体编号
                               "@MOName='" + GlobalUnit.g_Work.WorkSN + "'";                        //--工单号

                DataSet ds = GetDataSetWithSQLString(StrSQL);

                //if (ds == null || ds.Tables[0].Rows.Count == 0)
                if (ds.Tables.Count == 0)
                {
                    bt = true;//合格，可继续测试
                }
                else if (ds.Tables[0].Rows.Count > 0)
                {
                    bt = false;//不合格，不在范围，或前工序不OK
                    Errorinfo = ds.Tables[0].Rows[0][0].ToString();
                }

                return bt;

            }
            catch
            {
                return false ;
            }
        }
        #endregion

        #region 查询前装读取的电能表底度--调用存储过程执行，-1为没有查询记录
        public bool Sql_Txn_FrontLoadingTestReturnSNG(string S_ProductionID, ref float flt_Energy)
        {
            try
            {
                bool bt = false;

                //@lotsn nvarchar(50)='', --生产编号
                string StrSQL = @"exec Txn_FrontLoadingTestReturnSNG  " +
                               "@chr_Scbh_SN='" + S_ProductionID + "'";                                //--生产编号

                DataSet ds = GetDataSetWithSQLString(StrSQL);

                if (ds != null)
                {
                    DataTable RecDt = ds.Tables[0];
                    if (RecDt.Rows.Count > 0)
                    {
                        flt_Energy =Convert.ToSingle (ds.Tables[0].Rows[0][0].ToString());
                        bt= true;
                    }
                    else 
                    {
                        flt_Energy =0f;
                        bt= false;
                    }
                }
                else
                {
                    flt_Energy = -1f;
                    bt = false;
                }

                return bt;
            }
            catch
            {
                return true;
            }
        }
        #endregion

        #region 查询是否有权限进行系统配置
        public bool bolIsSystemConfigUser()
        {
            try
            {
                DataTable SystemConfigDT = new DataTable();
                string sql = "exec CheckYesNoSystemConfig @username='" + OrBitUserName.Trim() + "'";
                SystemConfigDT = GetDataSetWithSQLString(sql).Tables[0];
                string Mess = SystemConfigDT.Rows[0][0].ToString().Trim();
                if (Mess != "")
                {
                    MessageBox.Show(Mess, "提示");
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion

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
            DatabaseServer = "10.98.99.7";
            DatabaseName = "OrBitXI";
            DatabaseUser = "sa";
            DatabasePassword = "mes123";

            WcfServerUrl = "http://10.98.99.6:800/browserWCFService/BrowserService.svc";//"http://localhost/WCFService";
            DocumentServerURL = ""; //文档服务器URL
            PluginServerURL = "http://henryx61/Plug-in/";//插件服务器URL
            RptReportServerURL = "http://henryx61/RptExamples/"; //水晶报表服务器URL

            UserTicket = "";
            IsExitQuery = false;


        }
    

    }
}