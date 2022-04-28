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
using System.Data.SqlClient;
using System.Reflection;
using System.IO;
using System.Net;
using System.Collections;
using System.Xml.Linq;
using pwClassLibrary;
using pwInterface;


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
        public static pw_Main PUC;
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
        #endregion

        /// <summary>
        /// 本私有函数对插件各接口变量进行初始化，赋予默认值
        /// 调试环境下这些值不变，通过浏览器执行时，
        /// 这些变量将会根据系统环境被重新赋值。
        /// </summary>
        private void initializeVariableOrBit()
        {
            //放到MES插件服务器上是需要在Release下生成
#if DEBUG
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

            WcfServerUrl = "http://10.98.99.6:800/browserWCFService/BrowserService.svc";//"http://localhost/WCFService";//
            DocumentServerURL = ""; //文档服务器URL
            PluginServerURL = "http://henryx61/Plug-in/";//插件服务器URL
            RptReportServerURL = "http://henryx61/RptExamples/"; //水晶报表服务器URL 
            UserTicket = "";
            IsExitQuery = false;
#endif
        }

        //public string XMLFileName;
        //public static string NowTimeString = System.DateTime.Now.ToString("yyyyMMddHHmmss");
        //public string PathServer = @"http://10.98.99.6:800/browserWCFService/DataService.svc";

        private  string m_strMeterModel = "";       //表型号   
        private string m_strProductId = "";         //料号   
        private string m_ShemaName = "";            //方案名称
        private string m_PathXMLPlan = "";          //方案路径
        private string m_ShemaType = "30";          //方案类型，10-单板，20-前装，30-检定，40-后装
        private string m_SchemaArea = "海外";     //产品类型，国网，国网三相，海外，自动化

        #region 测试项目列表
        private string[] m_PlanList = { 
                     "产品信息", 
                     "读生产编号信息",
                     "误差检定信息",
                     "日计时误差检定",                     
                     "分相供电测试",
                     "交流采样测试",
                     "读电能表底度",
                     "打包参数下载信息",
                     "系统清零信息",
                     "误差检定点信息", 
                     "打包参数下载项信息"};
        private string[] m_PlanListName = { 
                     "Products", 
                     "Plan_ReadScbh",
                     "Plan_Wcjd",
                     "Plan_DgnSy",
                     "Plan_SinglePhaseTest",
                     "Plan_ACSamplingTest",
                     "Plan_ReadEnergy",
                     "Plan_DownPara",
                     "Plan_SysClear",
                     "Plan_Wcjd_Point", 
                     "Plan_DownPara_Item"};

        #endregion

        public static event DelegateEventProductChange OnEventProductChange;//料号改变事件
        public static event DelegateEventSchemaDataSave OnEventSchemaDataSave;//方案保存事件
        public static event DelegateEventSchemaDataLoad OnEventSchemaDataLoad;//方案调用事件

        #region UserControl
        
        //
        //private CL4100.Plan3Phase.Plan_ReadVerUserControl _Plan_ReadVerUserControl = new Plan3Phase.Plan_ReadVerUserControl();
        //private CL4100.Plan3Phase.Plan_Comm2TestUserControl _Plan_Comm2TestUserControl = new Plan3Phase.Plan_Comm2TestUserControl();
        //private CL4100.Plan3Phase.Plan_SelfCheckUserControl _Plan_SelfCheckUserControl = new Plan3Phase.Plan_SelfCheckUserControl();
        //private CL4100.Plan3Phase.Plan_AdjustClockUserControl _Plan_AdjustClockUserControl = new Plan3Phase.Plan_AdjustClockUserControl();
        //private CL4100.Plan3Phase.Plan_SetDateTimeUserControl _Plan_SetDateTimeUserControl = new Plan3Phase.Plan_SetDateTimeUserControl();
        //private CL4100.Plan3Phase.Plan_AdjustErrorUserControl _Plan_AdjustErrorUserControl = new Plan3Phase.Plan_AdjustErrorUserControl();
        //private CL4100.Plan3Phase.Plan_ReadPowerUserControl _Plan_ReadPowerUserControl = new Plan3Phase.Plan_ReadPowerUserControl();
        //private CL4100.Plan3Phase.Plan_SetWdUserControl _Plan_SetWdUserControl = new Plan3Phase.Plan_SetWdUserControl();
        //private CL4100.Plan3Phase.Plan_InitParaUserControl _Plan_InitParaUserControl = new Plan3Phase.Plan_InitParaUserControl();

        private CL4100.Plan3Phase.Plan_ProductsUserControl _ProductsUserControl = new Plan3Phase.Plan_ProductsUserControl();//产品信息
        private CL4100.Plan3Phase.Plan_ReadScbhUserControl _Plan_ReadScbhUserControl = new Plan3Phase.Plan_ReadScbhUserControl();//读生产编号
        private CL4100.Plan3Phase.Plan_WcjdUserControl _Plan_WcjdUserControl = new Plan3Phase.Plan_WcjdUserControl();//误差检定
        private CL4100.Plan3Phase.Plan_DgnSyUserControl _Plan_DgnSyUserControl = new Plan3Phase.Plan_DgnSyUserControl();//日计时误差检定
        private CL4100.Plan3Phase.Plan_FxgdUserControl _Plan_FxgdUserControl = new Plan3Phase.Plan_FxgdUserControl();//分相供电测试
        private CL4100.Plan3Phase.Plan_JcjdUserControl _Plan_JcjdUserControl = new Plan3Phase.Plan_JcjdUserControl();//交流采样测试
        private CL4100.Plan3Phase.Plan_ReadEnergyUserControl _Plan_ReadEnergyUserControl = new Plan3Phase.Plan_ReadEnergyUserControl();//读电能表底度
        private CL4100.Plan3Phase.Plan_DownParaUserControl _Plan_DownParaUserControl = new Plan3Phase.Plan_DownParaUserControl();//打包参数下载
        private CL4100.Plan3Phase.Plan_SysClearUserControl _Plan_SysClearUserControl = new Plan3Phase.Plan_SysClearUserControl();//系统清零信息
       
        #endregion

        public pw_Main()
        {
            InitializeComponent();
            PUC = this;
        }

        private void InitializeMeterParasPanel()
        {
            //string configFileMath = Application.StartupPath;

            //#region 初始化配置文件
            //if (!File.Exists(configFileMath + "\\JFrontloadingtest.ini"))
            //{
            //    StreamWriter sw = new StreamWriter(configFileMath + "\\JFrontloadingtest.ini");
            //    sw.Write("[Parameters]\r\nMaxI=60\r\nBasicI=5,10\r\nConst=1200,1600\r\nPLYS=0.5,1.0,2.5,3.0,5.0\r\nPath=http://10.98.99.6:800/browserWCFService/DataService.svc");
            //    sw.Close();
            //    sw.Dispose();
            //}
            //#endregion

            //#region 导入配置文件
            //string strIniFile = configFileMath + @"\JFrontloadingtest.ini";
            //#endregion

            //#region 服务器地址
            //PathServer = pwClassLibrary.pwFile.File.ReadInIString(strIniFile, "Parameters", "Path", @"http://10.98.99.6:800/browserWCFService/DataService.svc");
            //#endregion
        }

        private void pw_Main_Load(object sender, EventArgs e)
        {
            initializeVariableOrBit();
            //InitializeMeterParasPanel();
            InitializePlantreeView();
            InitializeUserControl();
            BindModel();
            textBox1.Text = WcfServerUrl + "**" + DatabaseServer ;
           
        }

        private void BindModel()
        {
            try
            {
               lbModel.Items.Clear();
               string sql = "exec GetModelNameForProductType @ProductFamilyName=" + m_SchemaArea ;
                DataTable ModelNameDT = GetDataSetWithSQLString(sql).Tables[0];
                for (int i = 0; i < ModelNameDT.Rows.Count; i++)
                {
                    lbModel.Items.Add(ModelNameDT.Rows[i][0].ToString().Trim());
                }
                if (lbModel.Items.Count > 0 && lbModel.SelectedItems.Count ==0) lbModel.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void lbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                m_strMeterModel = lbModel.Text.ToString().Trim();//表型号   
                BindProductName(m_strMeterModel);//显示料号列表
                CParam.ModelName = m_strMeterModel;//表型号  
                //string sql = "select ProductModelDescription from ProductModelRoot where ProductModelName='" + strMeterModel + "'";
                //DataTable ModelDiscription = GetDataSetWithSQLString(sql).Tables[0];
                //if (ModelDiscription.Rows.Count != 0)
                //{
                //    txtModelDiscription.Text = ModelDiscription.Rows[0][0].ToString().Trim();//型号描述
                //}
                //else
                //{
                //    txtModelDiscription.Text = string.Empty;//型号描述
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BindProductName(string _strMeterModel)
        {
            try
            {
                lbProductName.Items.Clear();
                FaNamelist.Items.Clear();
                txtSchemaDescription.Text = "";
                string sql = "exec GetProductName @ProductModelName='" + _strMeterModel + "'";
                DataTable ProductNameDT = GetDataSetWithSQLString(sql).Tables[0];
                for (int i = 0; i < ProductNameDT.Rows.Count; i++)
                {
                    lbProductName.Items.Add(ProductNameDT.Rows[i][1].ToString().Trim());
                }
                if (lbProductName.Items.Count > 0 && lbProductName.SelectedItems.Count==0) lbProductName.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void lbProductName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                m_strProductId = lbProductName.Text.Trim();//料号
                CParam.ModelName = m_strMeterModel;//表型号  
                CParam.ProductName = m_strProductId;//物料编码 
                string sql = "exec GetProductName @ProductModelName='" + m_strMeterModel + "'";
                DataTable ProductNameDT = GetDataSetWithSQLString(sql).Tables[0];
                string ProductId = ProductNameDT.Rows[0][0].ToString();
                sql = "select * from ProductModel where ProductModelRootID in (select ProductModelRootID from ProductModelRoot where ProductModelName='" + m_strMeterModel + "') and ProductId='" + ProductId + "'";
                //通过表型号和成品料号取出物料中的信息
                DataTable ModelDT = GetDataSetWithSQLString(sql).Tables[0];
                if (ModelDT.Rows.Count != 0)//是否进行过表型号的维护
                {
                    string _ProductPara="";
                    _ProductPara = ModelDT.Rows[0]["TestWay"].ToString()+"|";//测量方式
                    _ProductPara += ModelDT.Rows[0]["Voltage"].ToString().Replace("V", "") + "|";//基本电压
                    try
                    {
                        string I = ModelDT.Rows[0]["ICurrent"].ToString();
                        _ProductPara += I.Substring(0, I.IndexOf("-")).Replace("A", "") + "|";//基本电流
                        _ProductPara += I.Substring(I.IndexOf("-") + 1).Replace("A", "") + "|";//最大电流
                    }
                    catch
                    {

                    }
                    _ProductPara += ModelDT.Rows[0]["Constant"].ToString() + "|";//常数剑逆苍穹
                    _ProductPara += ModelDT.Rows[0]["Level"].ToString() + "|";//等级
                    _ProductPara += ModelDT.Rows[0]["Rate"].ToString() + "|";//频率
                    _ProductPara += ModelDT.Rows[0]["TerminalType"].ToString() + "|";//端子类型
                    _ProductPara += ModelDT.Rows[0]["GygyType"].ToString() + "|";//共阴共阳  
                    _ProductPara += ModelDT.Rows[0]["PulseType"].ToString() + "|";//脉冲类型
                    _ProductPara += ModelDT.Rows[0]["RelayType"].ToString() + "|";//继电器类型
                    //_ProductPara += "" + "|";//软件版本号
                    _ProductPara += m_strMeterModel + "|";//表型号

                    if (OnEventProductChange != null) OnEventProductChange(_ProductPara);

                    BindShemaName(m_strProductId);
                }
                else//产品信息赋空
                {

                }
            }
            catch
            {

            }
        }

        private void BindShemaName(string _strProductId)
        {
            try
            {
                #region 方案名称绑定
                FaNamelist.Items.Clear();
                txtSchemaDescription.Text = "";

                DataTable SchemaNameDT = new DataTable();
                string sql = "select SchemaNameNew,SchemaDescription from CLTestSchema where SchemaType='" + m_ShemaType 
                    + "' and MOName='" + m_strMeterModel 
                    + "' and SchemaName='" + _strProductId 
                    + "' and SchemaArea='"+ m_SchemaArea
                    + "' order by SchemaConfigureDate desc";
                SchemaNameDT = GetDataSetWithSQLString(sql).Tables[0];//
                for (int i = 0; i < SchemaNameDT.Rows.Count; i++)
                {
                    FaNamelist.Items.Add(SchemaNameDT.Rows[i][0].ToString());
                }
                if (FaNamelist.Items.Count > 0 && FaNamelist.SelectedItems.Count ==0)
                    FaNamelist.SelectedIndex = 0;
                else 
                {
                    string tmp_PathXMLPlan = Application.StartupPath + "\\WorkPlan.xml";//默认方案
                    BindSchemTreeView(tmp_PathXMLPlan);
                    lab_Info.Text = "当前型号料号没有配置三相检定方案，默认引用方案：" + getFileName(tmp_PathXMLPlan); 

                }
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void FaNamelist_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSchemaDescription.Text = "";
            m_ShemaName = FaNamelist.Text.ToString().Trim();//方案名称
            CParam.FanName = m_ShemaName;//方案名称 
            m_PathXMLPlan = Application.StartupPath + "\\" + m_ShemaName + ".xml";
            string sql = "";
            DataTable ShemaDT = new DataTable();
            try
            {
                sql = "exec GetShemaDT @SchemaNameNew='" + m_ShemaName 
                    + "',@SchemaType='" + m_ShemaType 
                    + "',@MOName='" + m_strMeterModel 
                    + "',@SchemaName='" + m_strProductId 
                    + "',@SchemaArea='"+ m_SchemaArea + "'" ;
                    
                ShemaDT = GetDataSetWithSQLString(sql).Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show("方案不存在，请配置方案" + ex.Message);
            }

            if (ShemaDT.Rows.Count != 0)//如果方案存在 开始加载方案
            {
                txtSchemaDescription.Text = ShemaDT.Rows[0][2].ToString().Trim();
                StreamWriter sw = new StreamWriter(m_PathXMLPlan);
                sw.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" + ShemaDT.Rows[0]["FileXML"].ToString().Trim());
                sw.Close();
                sw.Dispose();

                //m_PathXMLPlan = Application.StartupPath + "\\WorkPlan.xml";//调试时用

                BindSchemTreeView(m_PathXMLPlan);//导入方案
                lab_Info.Text = "当前方案名称：" + getFileName(m_PathXMLPlan); 

            }
            else
            {
                #region 默认是否测试选择项目及参数

                #endregion
            }

        }

        public void BindSchemTreeView(string path)
        {
            if (!File.Exists(path))
            {
                PlantreeView.Enabled = false;
                lab_Info.Text = "该型号料号表方案不存在，请先建设型号，料号，再配置方案";
                //MessageBox.Show("该型号料号表方案不存在，请先建设型号，料号，再配置方案", "提示", MessageBoxButtons.OK);
                return;
            }
            PlantreeView.Enabled = true;
            bool[] _NodeChecked = new bool[m_PlanListName.Length-2];
            XDocument d = XDocument.Load(path);
            PlantreeView.Nodes[0].Checked = true;
            if (OnEventSchemaDataLoad != null) OnEventSchemaDataLoad(0, path);//m_PathXMLPlan
            for (int i = 1; i < m_PlanListName.Length-2; i++)
            {
                try
                {
                    XElement hw = d.Root.Elements(m_PlanListName[i]).Where(el => el.Attribute("Description").Value == m_PlanList[i]).First();
                    XElement elementcbkhw = hw.Elements(m_PlanListName[i]).Where(el => el.Attribute("ID").Value == "IsCheck").First();//是否检验
                    _NodeChecked[i] = elementcbkhw.Attribute("Value").Value.ToUpper() == "TRUE" ? true : false;
                    PlantreeView.Nodes[i].Checked=_NodeChecked[i] ;
                    if (OnEventSchemaDataLoad != null) OnEventSchemaDataLoad(i, path);//m_PathXMLPlan
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                    continue;
                }
            }

        }

        private void InitializePlantreeView()
        {
            PlantreeView.Nodes.Clear();

            for (int i = 0; i < m_PlanList.Length-2; i++)
            {
                PlantreeView.Nodes.Add(m_PlanList[i]);
            }
        }

        private void InitializeUserControl()
        {
            splitContainer2.Panel2.Controls.Add(_ProductsUserControl);
            _ProductsUserControl.Dock = DockStyle.Fill;
            _ProductsUserControl.Visible = true;
            _ProductsUserControl.BringToFront();

            splitContainer2.Panel2.Controls.Add(_Plan_ReadEnergyUserControl);
            _Plan_ReadEnergyUserControl.Dock = DockStyle.Fill;
            _Plan_ReadEnergyUserControl.Visible = true;
            _Plan_ReadEnergyUserControl.BringToFront();

            splitContainer2.Panel2.Controls.Add(_Plan_ReadScbhUserControl);
            _Plan_ReadScbhUserControl.Dock = DockStyle.Fill;
            _Plan_ReadScbhUserControl.Visible = true;
            _Plan_ReadScbhUserControl.BringToFront();

            splitContainer2.Panel2.Controls.Add(_Plan_WcjdUserControl);
            _Plan_WcjdUserControl.Dock = DockStyle.Fill;
            _Plan_WcjdUserControl.Visible = true;
            _Plan_WcjdUserControl.BringToFront();

            splitContainer2.Panel2.Controls.Add(_Plan_JcjdUserControl);
            _Plan_JcjdUserControl.Dock = DockStyle.Fill;
            _Plan_JcjdUserControl.Visible = true;
            _Plan_JcjdUserControl.BringToFront();

            splitContainer2.Panel2.Controls.Add(_Plan_FxgdUserControl);
            _Plan_FxgdUserControl.Dock = DockStyle.Fill;
            _Plan_FxgdUserControl.Visible = true;
            _Plan_FxgdUserControl.BringToFront();

            splitContainer2.Panel2.Controls.Add(_Plan_DgnSyUserControl);
            _Plan_DgnSyUserControl.Dock = DockStyle.Fill;
            _Plan_DgnSyUserControl.Visible = true;
            _Plan_DgnSyUserControl.BringToFront();

            splitContainer2.Panel2.Controls.Add(_Plan_DownParaUserControl);
            _Plan_DownParaUserControl.Dock = DockStyle.Fill;
            _Plan_DownParaUserControl.Visible = true;
            _Plan_DownParaUserControl.BringToFront();

            splitContainer2.Panel2.Controls.Add(_Plan_SysClearUserControl);
            _Plan_SysClearUserControl.Dock = DockStyle.Fill;
            _Plan_SysClearUserControl.Visible = true;
            _Plan_SysClearUserControl.BringToFront(); 

            //splitContainer2.Panel2.Controls.Add(_Plan_ReadScbhUserControl);
            //_Plan_ReadScbhUserControl.Dock = DockStyle.Fill;
            //_Plan_ReadScbhUserControl.Visible = false;

            //splitContainer2.Panel2.Controls.Add(_Plan_ReadVerUserControl);
            //_Plan_ReadVerUserControl.Dock = DockStyle.Fill;
            //_Plan_ReadVerUserControl.Visible = false;

            //splitContainer2.Panel2.Controls.Add(_Plan_Comm2TestUserControl);
            //_Plan_Comm2TestUserControl.Dock = DockStyle.Fill;
            //_Plan_Comm2TestUserControl.Visible = false;

            //splitContainer2.Panel2.Controls.Add(_Plan_SelfCheckUserControl);
            //_Plan_SelfCheckUserControl.Dock = DockStyle.Fill;
            //_Plan_SelfCheckUserControl.Visible = false;

            //splitContainer2.Panel2.Controls.Add(_Plan_AdjustClockUserControl);
            //_Plan_AdjustClockUserControl.Dock = DockStyle.Fill;
            //_Plan_AdjustClockUserControl.Visible = false;

            //splitContainer2.Panel2.Controls.Add(_Plan_SetDateTimeUserControl);
            //_Plan_SetDateTimeUserControl.Dock = DockStyle.Fill;
            //_Plan_SetDateTimeUserControl.Visible = false;

            //splitContainer2.Panel2.Controls.Add(_Plan_AdjustErrorUserControl);
            //_Plan_AdjustErrorUserControl.Dock = DockStyle.Fill;
            //_Plan_AdjustErrorUserControl.Visible = false;

            //splitContainer2.Panel2.Controls.Add(_Plan_ReadPowerUserControl);
            //_Plan_ReadPowerUserControl.Dock = DockStyle.Fill;
            //_Plan_ReadPowerUserControl.Visible = false;

            //splitContainer2.Panel2.Controls.Add(_Plan_SetWdUserControl);
            //_Plan_SetWdUserControl.Dock = DockStyle.Fill;
            //_Plan_SetWdUserControl.Visible = false;

            //splitContainer2.Panel2.Controls.Add(_Plan_InitParaUserControl);
            //_Plan_InitParaUserControl.Dock = DockStyle.Fill;
            //_Plan_InitParaUserControl.Visible = false;


        }

        private void PlantreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {

            switch (e.Node.Index)
            {
                case 0://"产品信息":
                    _ProductsUserControl.BringToFront();
                    _ProductsUserControl.Dock = DockStyle.Fill;
                    _ProductsUserControl.Visible = true;
                    break;
                case 1://"读生产编号信息":
                    _Plan_ReadScbhUserControl.BringToFront();
                    _Plan_ReadScbhUserControl.Dock = DockStyle.Fill;
                    _Plan_ReadScbhUserControl.Visible = true;
                    break;
                case 2://"误差检定":
                    _Plan_WcjdUserControl.BringToFront();
                    _Plan_WcjdUserControl.Dock = DockStyle.Fill;
                    _Plan_WcjdUserControl.Visible = true;
                    break;
                case 3://"日计时误差检定"":
                    _Plan_DgnSyUserControl.BringToFront();
                    _Plan_DgnSyUserControl.Dock = DockStyle.Fill;
                    _Plan_DgnSyUserControl.Visible = true;
                    break;
                case 4://"分向供电测试":
                    _Plan_FxgdUserControl.BringToFront();
                    _Plan_FxgdUserControl.Dock = DockStyle.Fill;
                    _Plan_FxgdUserControl.Visible = true;
                    break;
                case 5://"交采精度检定":
                    _Plan_JcjdUserControl.BringToFront();
                    _Plan_JcjdUserControl.Dock = DockStyle.Fill;
                    _Plan_JcjdUserControl.Visible = true;
                    break;
                case 6://"读电能表底度":
                    _Plan_ReadEnergyUserControl.BringToFront();
                    _Plan_ReadEnergyUserControl.Dock = DockStyle.Fill;
                    _Plan_ReadEnergyUserControl.Visible = true;
                    break;
                case 7://"打包参数下载":
                    _Plan_DownParaUserControl.BringToFront();
                    _Plan_DownParaUserControl.Dock = DockStyle.Fill;
                    _Plan_DownParaUserControl.Visible = true;
                    break;
                case 8://"系统清零信息":
                    _Plan_SysClearUserControl.BringToFront();
                    _Plan_SysClearUserControl.Dock = DockStyle.Fill;
                    _Plan_SysClearUserControl.Visible = true;
                    break; 
            }
        }

        private void PlantreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (e.Node.Index)
            {
                case 0://"产品信息":
                    _ProductsUserControl.BringToFront();
                    _ProductsUserControl.Dock = DockStyle.Fill;
                    _ProductsUserControl.Visible = true;
                    break;               
                case 1://"读生产编号信息":
                    _Plan_ReadScbhUserControl.BringToFront();
                    _Plan_ReadScbhUserControl.Dock = DockStyle.Fill;
                    _Plan_ReadScbhUserControl.Visible = true;
                    break; 
                case 2://"误差检定":
                    _Plan_WcjdUserControl.BringToFront();
                    _Plan_WcjdUserControl.Dock = DockStyle.Fill;
                    _Plan_WcjdUserControl.Visible = true;
                    break;
                case 3://"日计时误差检定"":
                    _Plan_DgnSyUserControl.BringToFront();
                    _Plan_DgnSyUserControl.Dock = DockStyle.Fill;
                    _Plan_DgnSyUserControl.Visible = true;
                    break;
                case 4://"分向供电测试":
                    _Plan_FxgdUserControl.BringToFront();
                    _Plan_FxgdUserControl.Dock = DockStyle.Fill;
                    _Plan_FxgdUserControl.Visible = true;
                    break;
                case 5://"交采精度检定":
                    _Plan_JcjdUserControl.BringToFront();
                    _Plan_JcjdUserControl.Dock = DockStyle.Fill;
                    _Plan_JcjdUserControl.Visible = true;
                    break; 
                case 6://"读电能表底度":
                    _Plan_ReadEnergyUserControl.BringToFront();
                    _Plan_ReadEnergyUserControl.Dock = DockStyle.Fill;
                    _Plan_ReadEnergyUserControl.Visible = true;
                    break; 
                case 7://"打包参数下载":
                    _Plan_DownParaUserControl.BringToFront();
                    _Plan_DownParaUserControl.Dock = DockStyle.Fill;
                    _Plan_DownParaUserControl.Visible = true;
                    break;
                case 8://"系统清零信息":
                    _Plan_SysClearUserControl.BringToFront();
                    _Plan_SysClearUserControl.Dock = DockStyle.Fill;
                    _Plan_SysClearUserControl.Visible = true;
                    break; 
            } 
        }


        private void but_Add_Click(object sender, EventArgs e)
        {
            if (lbModel.SelectedIndex == -1 )
            {
                MessageBox.Show("请选择需要配置方案的表型号！", "提示");
                return;
            }
            if ( lbProductName.SelectedIndex == -1)
            {
                MessageBox.Show("请选择需要配置方案的成品料号！", "提示");
                return;
            }

            if (false == bolModifySchema())
            {
                return;
            }

            InputBoxResult inputbox = InputBox.Show("请输入方案名称:", "新增方案", "三相表检定测试方案");
            string strInput = inputbox.Text.Trim();
            if (strInput == "") return;

            string strSQL = "exec GetShemaDT @SchemaNameNew='" + strInput
                + "',@SchemaType='" + m_ShemaType 
                + "',@MOName='" + m_strMeterModel
                + "',@SchemaName='" + m_strProductId
                + "',@SchemaArea='" + m_SchemaArea + "'";
                

            DataSet ds = GetDataSetWithSQLString(strSQL);

            if (ds.Tables[0].Rows.Count != 0)//修改方案
            {
                MessageBox.Show("该方案已经存在!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else//新增方案
            {
                string tmp_PathXMLPlan = Application.StartupPath + "\\WorkPlan.xml";//默认方案

                if (File.Exists(tmp_PathXMLPlan))
                {
                    File.Delete(tmp_PathXMLPlan);
                }
                StreamWriter sw = new StreamWriter(tmp_PathXMLPlan);
                string FileXmlStr = "<WorkPlan>"

                    + "<Work Description=\"工单信息\">"
                    + "<Work ID=\"WorkSN\" Name=\"工单号\" Value=\"默认工单号\" /> "
                    + "<Work ID=\"CustomerName\" Name=\"客户名称\" Value=\"\" />"
                    + "<Work ID=\"ProductsName\" Name=\"产品名称\" Value=\"\" />"
                    + "<Work ID=\"ProductsSN\" Name=\"产品编号\" Value=\"\" />"
                    + "<Work ID=\"ProductsModel\" Name=\"产品型号\" Value=\"\" />"
                    + "</Work>"

                    + "<Products Description=\"产品信息\">"
                    + "<Products ID=\"Clfs\" Name=\"测量方式\" Value=\"三相四\" />"
                    + "<Products ID=\"Ub\" Name=\"电压\" Value=\"220V\" />"
                    + "<Products ID=\"Ib\" Name=\"电流\" Value=\"1.5A\" />"
                    + "<Products ID=\"IMax\" Name=\"电流\" Value=\"6A\" />"
                    + "<Products ID=\"Constant\" Name=\"常数\" Value=\"5000\" />"
                    + "<Products ID=\"DJ\" Name=\"等级\" Value=\"0.5\" />"
                    + "<Products ID=\"PL\" Name=\"频率\" Value=\"50\" />"
                    + "<Products ID=\"DzType\" Name=\"端子类型\" Value=\"国网端子\" />"
                    + "<Products ID=\"GYGY\" Name=\"共阴共阳\" Value=\"共阴\" />"
                    + "<Products ID=\"PulseType\" Name=\"脉冲类型\" Value=\"脉冲盒\" />"
                    + "<Products ID=\"JDQType\" Name=\"继电器类型\" Value=\"内置继电器\" />"
                    + " <Products ID=\"SoftVer\" Name=\"软件版本号\" Value=\"3E0091050400630020130115\" />"
                    + "</Products>"

                    + "<Plan_ReadScbh Description=\"读生产编号信息\">"
                    + "<Plan_ReadScbh ID=\"IsCheck\" Name=\"是否要检\" Value=\"True\" />"
                    + "<Plan_ReadScbh ID=\"CustomerName\" Name=\"项目名称\" Value=\"读生产编号信息\" />"
                    + "<Plan_ReadScbh ID=\"ProductsName\" Name=\"项目参数\" Value=\"DLT645_1997|FFF9|6|0||\" />"
                    + "</Plan_ReadScbh>"

                    + "<Plan_Wcjd Description=\"误差检定信息\">"
                    + "<Plan_Wcjd ID=\"IsCheck\" Name=\"是否要检\" Value=\"True\" />"
                    + "<Plan_Wcjd ID=\"CustomerName\" Name=\"项目名称\" Value=\"误差检定信息\" />"
                    + "<Plan_Wcjd ID=\"ProductsName\" Name=\"项目参数\" Value=\"4\" />"
                    + "</Plan_Wcjd>"

                    + "<Plan_DgnSy Description=\"日计时误差检定\">"
                    + "<Plan_DgnSy ID=\"IsCheck\" Name=\"是否要检\" Value=\"true\" /> "
                    + "<Plan_DgnSy ID=\"CustomerName\" Name=\"项目名称\" Value=\"日计时误差检定\" /> "
                    + "<Plan_DgnSy ID=\"ProductsName\" Name=\"项目参数\" Value=\"5000000|1|1\" /> "
                    + "</Plan_DgnSy>"

                    + "<Plan_SinglePhaseTest Description=\"分相供电测试\">"
                    + " <Plan_SinglePhaseTest ID=\"IsCheck\" Name=\"是否要检\" Value=\"true\" /> "
                    + "<Plan_SinglePhaseTest ID=\"CustomerName\" Name=\"项目名称\" Value=\"分相供电测试\" /> "
                    + "<Plan_SinglePhaseTest ID=\"ProductsName\" Name=\"项目参数\" Value=\"5\" /> "
                    + "</Plan_SinglePhaseTest>"

                    + "<Plan_ACSamplingTest Description=\"交流采样测试\">"
                    + "<Plan_ACSamplingTest ID=\"IsCheck\" Name=\"是否要检\" Value=\"true\" /> "
                    + "<Plan_ACSamplingTest ID=\"CustomerName\" Name=\"项目名称\" Value=\"交流采样测试\" /> "
                    + "<Plan_ACSamplingTest ID=\"ProductsName\" Name=\"项目参数\" Value=\"0.4|0.4|0.4\" /> "
                    + "</Plan_ACSamplingTest>"

                    + "<Plan_ReadEnergy Description=\"读电能表底度\">"
                    + "<Plan_ReadEnergy ID=\"IsCheck\" Name=\"是否要检\" Value=\"True\" />"
                    + "<Plan_ReadEnergy ID=\"CustomerName\" Name=\"项目名称\" Value=\"读电能量\" />"
                    + "<Plan_ReadEnergy ID=\"ProductsName\" Name=\"项目参数\" Value=\"DLT645_1997|00000000|4|2||1|5\" />"
                    + "</Plan_ReadEnergy>"

                    + "<Plan_DownPara Description=\"打包参数下载信息\">"
                    + "<Plan_DownPara ID=\"IsCheck\" Name=\"是否要检\" Value=\"true\" /> "
                    + "<Plan_DownPara ID=\"CustomerName\" Name=\"项目名称\" Value=\"打包参数下载\" /> "
                    + "<Plan_DownPara ID=\"ProductsName\" Name=\"项目参数\" Value=\"DLT645_2007|111111111111|5\" /> "
                    + "</Plan_DownPara>"


                    + "<Plan_SysClear Description=\"系统清零信息\">"
                    + "<Plan_SysClear ID=\"IsCheck\" Name=\"是否要检\" Value=\"true\" />"
                    + "<Plan_SysClear ID=\"CustomerName\" Name=\"项目名称\" Value=\"系统清零\" /> "
                    + "<Plan_SysClear ID=\"ProductsName\" Name=\"项目参数\" Value=\"DLT645_2007|04CC1000|1|0|55|\" /> "
                    + "</Plan_SysClear>"

                    + "<Plan_Wcjd_Point Description=\"误差检定点信息\" />"
                    + "<Plan_DownPara_Item Description=\"打包参数下载项信息\"/>"

                    + "</WorkPlan>";
                sw.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + FileXmlStr);
                sw.Close();
                sw.Dispose();

                XDocument document = XDocument.Load(tmp_PathXMLPlan);
                AddToDbServer(m_strMeterModel, m_strProductId, document, txtSchemaDescription.Text, strInput);//新增方案
                m_PathXMLPlan = Application.StartupPath + "\\" + strInput + ".xml";//新增方案
                BindShemaName(m_strProductId);
                BindSchemTreeView(m_PathXMLPlan);//导入方案
                lab_Info.Text = "新增方案成功,新增方案名称：" + getFileName(strInput);
            }
        }

        private bool  bolModifySchema()
        {
            //判断是否有全权限新增修改删除方案
            try
            {
                DataTable ModifySchemaDT = new DataTable();
                string sql = "exec CheckYesNoModifySchema @username='" + OrBitUserName.Trim() + "'";
                ModifySchemaDT = GetDataSetWithSQLString(sql).Tables[0];
                string Mess = ModifySchemaDT.Rows[0][0].ToString().Trim();
                if (Mess != "")
                {
                    MessageBox.Show(Mess, "提示");
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void AddToDbServer(string Model, string ProductName, XDocument document, string SchemaDescription, string SchemaNameNew)
        {
            #region 保存数据
            string SQLString = @"exec SaveCLTestSchemaNew @SchemaNOParam='" + Model + "-" + m_ShemaType
                + "',@SchemaTypeParam='" + m_ShemaType 
                + "',@MONameParam='" + Model
                + "',@SchemaNameParam='" + ProductName
                + "',@FileXMLParam='" + document.ToString()
                + "',@SchemaConfigureUserParam='" + OrBitUserName
                + "',@SchemaConfigureDateParam='" + System.DateTime.Now.ToString()
                + "',@SchemaDescriptionParam='" + SchemaDescription
                + "',@SchemaNameNew='" + SchemaNameNew 
                + "',@SchemaArea='" + m_SchemaArea + "'";
                
                
            GetDataSetWithSQLString(SQLString);
            #endregion

            #region 保存历史数据
            SQLString = @"exec SaveCLTestSchemaHistoricalRecords @SchemaNOParam='" + Model + "-" + m_ShemaType
                + "',@SchemaTypeParam='" + m_ShemaType 
                + "',@MONameParam='" + Model 
                + "',@SchemaNameParam='" + ProductName 
                + "',@FileXMLParam='" + document.ToString() 
                + "',@SchemaConfigureUserParam='" + OrBitUserName
                + "',@SchemaConfigureDateParam='" + System.DateTime.Now.ToString() 
                + "',@SchemaDescriptionParam='" + SchemaDescription
                + "',@SchemaNameNew='" + SchemaNameNew 
                + "',@SchemaArea='" + m_SchemaArea + "'";
                
            GetDataSetWithSQLString(SQLString);
            #endregion

        }

        private void UpdateToDbServer(string Model, string ProductName, XDocument document, string SchemaDescription, string SchemaNameNew)
        {
            #region 修改数据
            string SQLString = @"update CLTestSchema set FileXML='" + document.ToString()
                + "',SchemaConfigureUser='" + OrBitUserName
                + "',SchemaConfigureDate='" + System.DateTime.Now.ToString()
                + "',SchemaDescription='" + SchemaDescription
                + "' where MOName='" + Model
                + "' and SchemaType='" + m_ShemaType
                + "' and SchemaNameNew='" + SchemaNameNew
                + "' and SchemaName='" + ProductName
                + "' and SchemaArea='" + m_SchemaArea + "'";


            GetDataSetWithSQLString(SQLString);
            #endregion

            #region 保存历史数据
            SQLString = @"exec SaveCLTestSchemaHistoricalRecords @SchemaNOParam='" + Model + "-" + m_ShemaType
                + "',@SchemaTypeParam='" + m_ShemaType
                + "',@MONameParam='" + Model
                + "',@SchemaNameParam='" + ProductName
                + "',@FileXMLParam='" + document.ToString()
                + "',@SchemaConfigureUserParam='" + OrBitUserName
                + "',@SchemaConfigureDateParam='" + System.DateTime.Now.ToString()
                + "',@SchemaDescriptionParam='" + SchemaDescription
                + "',@SchemaNameNew='" + SchemaNameNew 
                + "',@SchemaArea='" + m_SchemaArea + "'";
                
            GetDataSetWithSQLString(SQLString);
            #endregion
        }

        #region 清除临时表中的误差点数据
        private void DeletePoint()
        {
            string sql = @"DELETE FROM JDPointTempNew where datetimePoint<'" + System.DateTime.Now.AddHours(-10) + "'";
            GetDataSetWithSQLString(sql);
        }
        #endregion


        private void DeleteToDbServer(string Model, string ProductName,  string SchemaNameNew)
        {
            #region 删除历史数据
            string SQLString = "Delete from CLTestSchema where SchemaNameNew='" + SchemaNameNew                
                + "' and SchemaType='" + m_ShemaType 
                + "' and MOName='" + Model
                + "' and SchemaName='" + ProductName
                + "' and SchemaArea='" + m_SchemaArea + "'"; 
            GetDataSetWithSQLString(SQLString);
            #endregion

            #region 删除临时表中的误差点数据
            SQLString = @"DELETE FROM JDPointTempNew where SchemaName='" + SchemaNameNew
                + "' and SchemaType='" + m_ShemaType
                + "' and Model='" + Model
                + "' and ProductName='" + ProductName
                + "' and SchemaArea='" + m_SchemaArea + "'";
            GetDataSetWithSQLString(SQLString);
            #endregion
        }



        private void but_Save_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < m_PlanListName.Length-2; i++)//
            {
                if (OnEventSchemaDataSave != null) OnEventSchemaDataSave(i, PlantreeView.Nodes[i].Checked , m_PathXMLPlan);//
            }

            XDocument document = XDocument.Load(m_PathXMLPlan);
            UpdateToDbServer(m_strMeterModel, m_strProductId, document, txtSchemaDescription.Text, m_ShemaName);//修改方案
            DeletePoint();
            lab_Info.Text = "修改方案成功,被修改方案名称：" + getFileName(m_PathXMLPlan); 

        }


        private void but_Delete_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("你确定要删除该方案吗？","删除",MessageBoxButtons.YesNo)==DialogResult.Yes)
            {
                DeleteToDbServer(m_strMeterModel, m_strProductId, m_ShemaName);//删除方案
                BindShemaName(m_strProductId);
                lab_Info.Text = "删除方案成功,被删除方案名称：" + getFileName(m_ShemaName); 
            }

        }

        /// <summary>
        /// 解析文件路径返回文件名称
        /// </summary>
        /// <param name="sPath"></param>
        /// <returns></returns>
        private string getFileName(string sPath)
        {
            return sPath.Split('\\')[sPath.Split('\\').Length - 1]; 
        }

        private void btnCoppy_Click(object sender, EventArgs e)
        {
            CoppyShema cop = new CoppyShema();
            cop.ShowDialog();
            BindShemaName(m_strProductId);

        }

    }
}
