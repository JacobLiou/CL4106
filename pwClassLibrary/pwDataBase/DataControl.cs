using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace pwClassLibrary.DataBase
{
    public class DataControl
    {
        /// <summary>
        /// ACCESS数据库连接字符串常数
        /// </summary>
        const string CONST_ACCESS = "Provider=Microsoft.Jet.OLEDB.4.0;Persist Security Info=True;Jet OLEDB:DataBase Password=;Data Source=";
        /// <summary>
        /// 数据库连接对象
        /// </summary>
        protected OleDbConnection _Con;
        /// <summary>
        /// 数据库记录集对象
        /// </summary>
        private OleDbDataReader _Reader;
        /// <summary>
        /// Access数据库路径
        /// </summary>
        private string _sTrAccessPath="";
        /// <summary>
        /// SQL服务器IP地址
        /// </summary>
        private string _Ip="";
        /// <summary>
        /// SQL用户名
        /// </summary>
        private string _User="";
        /// <summary>
        /// SQL密码
        /// </summary>
        private string _Pwd="";
        /// <summary>
        /// SQL数据库名称
        /// </summary>
        const string _DbName = "ClouDnt";

        private bool _Connection = false;

        /// <summary>
        /// 检查数据是否连接（只读）
        /// </summary>
        public bool Connection
        {
            get 
            {
                return _Connection;
            }
        }

        /// <summary>
        /// 无参数构造，默认ACCESS路径为程序目录下DATABASE文件夹
        /// </summary>
        /// 
        public DataControl()
        { 
            _sTrAccessPath=System.Windows.Forms.Application.StartupPath + "\\Database\\ClouDnbInfo.mdb";
            if (!System.IO.File.Exists(_sTrAccessPath))
            {
                CreateMdb DataMdb = new CreateMdb(_sTrAccessPath);
                DataMdb.CreateDataDb();
                DataMdb = null;
            }
            OpenDB();
        }
        /// <summary>
        /// 一个参数构造，设置ACCESS路径
        /// </summary>
        /// <param name="SavePath"></param>
        public DataControl(string SavePath)
        { 
            _sTrAccessPath=SavePath;
            OpenDB();
        }
        /// <summary>
        /// 三个参数构造，设置服务器IP地址
        /// </summary>
        /// <param name="Ip"></param>
        /// <param name="User"></param>
        /// <param name="Pwd"></param>
        public DataControl(string Ip,string User,string Pwd)
        {
            _Ip=Ip;
            _User=User;
            _Pwd=Pwd;
            OpenDB();
        }
        ~DataControl()
        {
            CloseDB();
        }


        public OleDbConnection Con
        {
            get
            {
                return _Con;
            }
        }


        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <returns></returns>
        private void OpenDB()
        {
            string ConString = "";
            if (_sTrAccessPath == "")
                ConString = "Provider=SQLOLEDB.1;Password=" + _Pwd
                                + ";Persist Security Info=True;User ID="
                                + _User + ";Initial Catalog="
                                + _DbName + ";Data Source=" + _Ip;
            else
            {
                ConString = CONST_ACCESS + _sTrAccessPath;
            }
            if (_Con != null)
            {
                _Con.Close();
                _Con.Dispose();
                OleDbConnection.ReleaseObjectPool();
            }

            _Con = new OleDbConnection(ConString);
            try
            {
                _Con.Open();
                _Connection=true;
            }
            catch
            {
                _Con.Dispose();
                OleDbConnection.ReleaseObjectPool();
            }
        }
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        protected void CloseDB()
        {

             OleDbConnection.ReleaseObjectPool();

        }
        /// <summary>
        /// 将数据写入数据库
        /// </summary>
        /// <param name="InsertSQLString">写入数据库的SQL语句</param>
        /// <returns></returns>
        public bool SaveData(List<string> InsertSQLString,out string ErrString)
        {
            OleDbCommand _Com = new OleDbCommand();    //声明一个命令对象
            OleDbTransaction _Tx = _Con.BeginTransaction();     //实例化一个事务对象
            _Com.Connection=_Con;       
            _Com.Transaction = _Tx;    //引用事务
            try
            {
                DateTime startTime = DateTime.Now;
                for (int int_i = 0; int_i < InsertSQLString.Count; int_i++)
                {
                    string _SqlString = "";
                    if (_sTrAccessPath == "")
                        _SqlString = InsertSQLString[int_i].Replace("#", "'");//如果是SQL数据库则需要将#替换为'
                    else
                        _SqlString = InsertSQLString[int_i].ToString();
                    _Com.CommandText = _SqlString;//插入数据
                    _Com.ExecuteNonQuery(); //执行
                  
                }

               // Console.WriteLine(Comm.Function.DateTimes.DateDiff(startTime) );

                _Tx.Commit();  //提交事务
                _Tx.Dispose();
                _Com.Dispose();
                ErrString = "";
                return true;
            }
            catch (OleDbException e)
            {
                _Tx.Rollback();   //回滚事务
                _Tx.Dispose();
                _Com.Dispose();
                ErrString = e.Message;
                return false;
            }
            catch (Exception ex)
            {
                _Tx.Rollback();
                _Tx.Dispose();
                _Com.Dispose();
                ErrString = ex.Message;
                return false;
            }

            finally
            {

            }
        }
        /// <summary>
        /// 读取最大自动编号并且+1
        /// </summary>
        /// <param name="ErrString">输出错误编号</param>
        /// <returns></returns>
        public long ReadMaxAutoID(out string ErrString)
        {
            OleDbCommand _Com = _Con.CreateCommand();
            _Com.CommandText = "Insert into CreateIDTable(sTrTmpValue) Values ('')";
            _Com.ExecuteNonQuery();
            _Com.CommandText = "Select MAX(Lng_AutoID) AS ID From CreateIDTable";
            _Reader = _Com.ExecuteReader();
            _Reader.Read();
            long _Return = long.Parse(_Reader["ID"].ToString());
            _Reader.Close();
            _Reader = null;
            _Com=null;
            ErrString = "";
            return _Return;
        }

    }
}
