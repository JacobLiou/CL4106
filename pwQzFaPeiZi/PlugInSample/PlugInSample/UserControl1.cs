using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;
using System.Xml.Linq;
using System.IO;




/* OrBit的C#插件示例
 * 采用VS-2008-SP1编写
 * 版本号V10.40
 * 本插件适用于OrBit-Browser V10.20　或以上的版本
 * 提供了各种必要的接口(属性、函数(方法\过程))，开发者可以在此基础上进行改编或完善
 * 由:深圳OrBit Systems Inc. OrBit Team提供
 * 发布日期 2009年1月1日
 * 最后修改 2011年10月23日
 */


namespace CL4101_QZ_GW
{
    public partial class UserControl1 : UserControl
    {  //类初始化

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
        public static UserControl1 PUC;

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
                if (this.Parent.Name.ToString() != "FormPlugIn")
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

        #region
        /// <summary>
        /// 插件入口,由.NET自动生成
        /// </summary>
        public UserControl1()
        { 
            InitializeComponent();　//插件控件布局(.NET的默认过程)
            initializeVariable(); //插件变量初始化 
            PUC = this;
        }

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

            // WcfServerUrl = "http://localhost/WCFService";
            //DocumentServerURL = ""; //文档服务器URL
            //PluginServerURL = "http://henryx61/Plug-in/";//插件服务器URL
            // RptReportServerURL = "http://henryx61/RptExamples/"; //水晶报表服务器URL     

            WcfServerUrl = "http://10.98.99.6:800/browserWCFService/BrowserService.svc";
            DocumentServerURL = "http://10.98.99.6:800/SOPDemo"; //文档服务器URL
            PluginServerURL = "http://10.98.99.6:800/OrBitPlugins/";//插件服务器URL
            RptReportServerURL = "http://10.98.99.6:800/OrBitReports/"; //水晶报表服务器URL

            UserTicket = "";
            IsExitQuery = false;
            InitializeMeterParasPanel();
        }

     

        /// <summary>
        /// PluginUnload函数为OrBit浏览器关闭窗体时触发的过程,可以用于示范各种对象
        /// 此函数不可改名或删除
        /// 但里面内容允许修改。
        /// </summary>
        public void PluginUnload()
        {
        }

        //演示如何退出插件
        private void tsbtnExit_Click(object sender, EventArgs e)
        {
            this.ParentForm.Close();

        }  
        #endregion

        #region
        public string XMLFileName;
        public static string NowTimeString = System.DateTime.Now.ToString("yyyyMMddHHmmss");
        public string PathServer = @"http://10.98.99.6:800/browserWCFService/DataService.svc";

        private void mainForm_Load(object sender, EventArgs e)
        {

        }

        private void InitializeMeterParasPanel()
        {
            string configFileMath = Application.StartupPath;

            #region 初始化配置文件
            if (!File.Exists(configFileMath + "\\JFrontloadingtest.ini"))
            {
                StreamWriter sw = new StreamWriter(configFileMath + "\\JFrontloadingtest.ini");
                sw.Write("[Parameters]\r\nMaxI=60\r\nBasicI=5,10\r\nConst=1200,1600\r\nPLYS=0.5,1.0,2.5,3.0,5.0\r\nPath=http://10.98.99.6:800/browserWCFService/DataService.svc");
                sw.Close();
                sw.Dispose();
            }
            #endregion

            #region 导入配置文件
            IniFile.FilePath = configFileMath + @"\JFrontloadingtest.ini";
            #endregion

            #region 服务器地址
            try
            {
                PathServer = IniFile.IniReadValue("Parameters", "Path");
            }
            catch
            {
                PathServer = @"http://10.98.99.6:800/browserWCFService/DataService.svc";
            }
            #endregion

            InitTree();
            #region 默认选项
            cmbReadPowerProtcol.SelectedIndex = 1;//读电能量的默认协议“DLT645_2007”
            ProductNOParamXYCmb.SelectedIndex = 0;//读生产编号的默认协议“DLT645_1997”
            cmbProcolWCJZ.SelectedIndex = 0;//校准误差的默认协议“DLT645_1997”
            cbmjzfangfa.SelectedIndex = 0;//校准误差的默认功率校准“功率校准”          
            cmbjzxdl.SelectedIndex = 0;//校准误差的默认标准小电流选择“0.2Ib”
            cmbTimeCheckProcol.SelectedIndex = 0;//时钟误差的默认协议“DLT645_1997”          
            cmbM1Bit0.SelectedIndex = 0;
            cmbM1Bit1.SelectedIndex = 0;
            cmbM2.SelectedIndex = 0;
            cmbhwProcol.SelectedIndex = 1;
            cmbIniParam.SelectedIndex = 0;//参数初始化
            #endregion

        }

        private void InitTree()
        {
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add("读生产编号",);
            treeView1.Nodes.Add("读软件版本号");
            treeView1.Nodes.Add("第2路485口通讯测试");
            treeView1.Nodes.Add("整机自检功能");
            treeView1.Nodes.Add("时钟误差校准");
            treeView1.Nodes.Add("时钟设置");
            treeView1.Nodes.Add("计量误差校准");
            treeView1.Nodes.Add("整机功耗测试");
            treeView1.Nodes.Add("温度校准");
            treeView1.Nodes.Add("始化表参数");
            treeView1.Nodes.Add("整机功耗测试");

        }

        private void AddNewItem(ComboBox cmb, string iniFile, string section, string key)
        {
            AddNewParameter newFrm = new AddNewParameter(iniFile, section, key, cmb);
            if (DialogResult.Yes == newFrm.ShowDialog())
            {
                string value = newFrm.Value;
                cmb.Items.Add(value);
                cmb.Text = value;
            }
            else
            {
                cmb.SelectedIndex = -1;
            }
        }



        private void btnDelFunctionCheckSchema_Click(object sender, EventArgs e)
        {
            if (functionCheckSchemaCmb.Text == "")
            {
                MessageBox.Show("请选择要删除的方案", "提示");
                return;
            }
            if (DialogResult.Yes == MessageBox.Show("您确定要删除此方案吗?", "提示", MessageBoxButtons.YesNo))
            {
                #region 删除方案文件
                string path = IniFile.FilePath;
                string inputpath = path.Substring(0, path.LastIndexOf("\\"));
                string inputFile = inputpath + "\\WorkPlan_" + functionCheckSchemaCmb.Text.Trim() + ".xml";
                if (File.Exists(inputFile))
                {
                    File.Delete(inputFile);
                }
                #endregion
                string value = functionCheckSchemaCmb.Text;
                string[] oldValue = IniFile.IniReadValue("Parameters", "方案").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                string newStrValue = "";
                for (int i = 0; i < oldValue.Length - 1; i++)
                {
                    if (value != oldValue[i].ToString())
                    {
                        newStrValue += (oldValue[i] + ",");
                    }
                }
                if (value != oldValue[oldValue.Length - 1])
                {
                    newStrValue += oldValue[oldValue.Length - 1];
                }
                else
                {
                    if (newStrValue.Trim() != "")
                    {
                        newStrValue = newStrValue.Substring(0, newStrValue.Length - 1);
                    }
                }
                IniFile.IniWriteValue("Parameters", "方案", newStrValue);

            }

        }

        private void btnSaveFunctionCheckSchema_Click(object sender, EventArgs e)
        {
            SaveFile("0");//新增方案
            //BindShemaName();
        }

        private void btnEditSchema_Click(object sender, EventArgs e)
        {
            SaveFile("1");//修改方案
            // BindShemaName();
        }
        public void SaveFile(string path)
        {
            #region
            //string PathServer = WcfServerUrl.Replace("BrowserService", "DataService");
            //XMLFileName = functionCheckSchemaCmb.Text.Trim();
            //if (path == "1" || path == "0")
            //{
            //if (functionCheckSchemaCmb.Text == "")
            //{
            //    MessageBox.Show("请填写方案名称", "提示");
            //    return;
            //  }
            //}
            //else
            //{
            //    try
            //    {
            //        XMLFileName = path.Substring(path.LastIndexOf("\\") + 1, path.LastIndexOf(".") - path.LastIndexOf("\\") - 1);
            //    }
            //    catch
            //    {

            //    }
            //}
            #endregion

            #region 方案名称不能重复
            //if (path == "0")//新增的时候方案名不能重复，工单号不能存在多个方案
            //{
            //    string sql = "select MOName from CLTestSchema where MOName='" + txtModel.Text.Trim() + "' and SchemaType='20' and SchemaName='" + lbProductName.Text.Trim() + "' and SchemaNameNew='" + functionCheckSchemaCmb.Text.Trim() + "'";
            //    DataSet ds = GetDataSetWithSQLString(sql);
            //    if (ds.Tables[0].Rows.Count != 0)
            //    {
            //        MessageBox.Show("方案名称重复", "提示");
            //        return;
            //    }
            //    sql = "select MOName from CLTestSchema where MOName='" + txtModel.Text.Trim() + "' and SchemaType='20'";
            //    ds = GetDataSetWithSQLString(sql);
            //    if (ds.Tables[0].Rows.Count != 0)
            //    {
            //        MessageBox.Show("此表型号存在方案", "提示");
            //        return;
            //    }
            //}
            #endregion

            #region 是否检定
            cbkProduct.Checked = true;//产品信息        


            string wcjzstr = "";
            string wcjz16 = "00";
            if (cbkWCCalibration.Checked == true)
            {

                #region 步骤⑦
                if (cb7.Checked == true)
                {
                    wcjzstr += "1";
                }
                else
                {
                    wcjzstr += "0";
                }
                #endregion
                #region 步骤⑥
                if (cb6.Checked == true)
                {
                    wcjzstr += "1";
                }
                else
                {
                    wcjzstr += "0";
                }
                #endregion
                #region 步骤⑤
                if (cb5.Checked == true)
                {
                    wcjzstr += "1";
                }
                else
                {
                    wcjzstr += "0";
                }
                #endregion
                #region 步骤④
                if (cb4.Checked == true)
                {
                    wcjzstr += "1";
                }
                else
                {
                    wcjzstr += "0";
                }
                #endregion
                #region 步骤③
                if (cb3.Checked == true)
                {
                    wcjzstr += "1";
                }
                else
                {
                    wcjzstr += "0";
                }
                #endregion
                #region 步骤②
                if (cb2.Checked == true)
                {
                    wcjzstr += "1";
                }
                else
                {
                    wcjzstr += "0";
                }
                #endregion
                #region 步骤①
                if (cb1.Checked == true)
                {
                    wcjzstr += "1";
                }
                else
                {
                    wcjzstr += "0";
                }
                #endregion
                wcjz16 = Convert.ToString(Convert.ToInt32(wcjzstr, 2), 16).ToUpper();
            }
            #endregion

            #region M1
            string M1Bitstr = "";//M1Bit0+M1Bit1
            string M1Bit0str = cmbM1Bit0.SelectedIndex.ToString();
            string M1Bit1str = cmbM1Bit1.SelectedIndex.ToString();
            if (M1Bit0str == "0" && M1Bit1str == "0")
            {
                M1Bitstr = "0";
            }
            if (M1Bit0str == "0" && M1Bit1str == "1")
            {
                M1Bitstr = "1";
            }
            if (M1Bit0str == "1" && M1Bit1str == "0")
            {
                M1Bitstr = "2";
            }
            if (M1Bit0str == "1" && M1Bit1str == "1")
            {
                M1Bitstr = "3";
            }
            #endregion
            try
            {
                XMLFileSave p = new XMLFileSave
                {
                    #region 数据库链接
                    DatabaseServer1 = DatabaseServer,
                    DatabaseName1 = DatabaseName,
                    DatabasePassword1 = DatabasePassword,
                    DatabaseUser1 = DatabaseUser,
                    #endregion

                    #region 产品信息
                    SchemaDescription = txtSchemaDescription.Text.Trim(),//方案描述
                    SchemaNameNew = functionCheckSchemaCmb.Text.Trim(),//方案号
                    ProductName = lbProductName.Text.Trim(),//成品料号
                    Model = txtModel.Text,//表型号
                    ModelDiscription = txtModelDiscription.Text,//型号描述
                    TestType = txtTestType.Text,//测量方式
                    BasicV = txtBasicV.Text,//基本电压
                    BasicI = txtBasicI.Text,//基本电流
                    MaxI = txtMaxI.Text,//最大电流
                    Const = txtConst.Text,//常数
                    Level = txtLevel.Text,//等级
                    HZ = txtHZ.Text,//频率
                    DuanZi = txtDuanZi.Text,//端子类型
                    gongyinggongyang = txtgongyinggongyang.Text,//共阴共阳
                    MaiChong = txtMaiChong.Text,//脉冲类型
                    JiDianQi = txtJiDianQi.Text,//继电器类型
                    RevSoftNo = txtRevSoftNo.Text,//软件版本号                  
                    #endregion

                    #region 读电能量
                    valueddny = (cbkReadPower.Checked == true) ? "true" : "false",//是否要检
                    ProjectNameReadPower = txtReadPowerName.Text.Trim(),//项目名称
                    ProcolReadPower = cmbReadPowerProtcol.Text.Trim(),//协议
                    IdentityNOReadPower = txtReadPowerIdentityNO.Text.Trim(),//标识编码  
                    DataLengthReadPower = txtReadPowerDataLength.Text.Trim(),//数据长度
                    PointReadPower = txtReadPowerPoint.Text.Trim(),//小数点
                    SendNumberReadPower = txtReadPowerSendParameter.Text.Trim(),//下发参数 

                    #endregion

                    #region 读生产编号信息
                    //valuedscbh = (cbkReadProctNO.Checked == true) ? "true" : "false",//是否要检
                    valuedscbh = "true",//是否要检 默认生产编号信息为必选项
                    ProjectNameProductNumber = txtProjectNameProductNumber.Text.Trim(),//项目名称
                    ProductNOParamXY = ProductNOParamXYCmb.Text.Trim(),//协议
                    IdentityNO = txtIdentityNO.Text.Trim().ToUpper(),//标识编码  
                    DataLengthReadNO = txtProductNODataLength.Text.Trim(),//数据长度
                    PointReadNO = txtProductNOPoint.Text.Trim(),//小数点
                    SendNumberReadNO = txtProductNOSendParameter.Text.Trim(),//下发参数 

                    #endregion

                    #region  误差校准信息
                    valuewcjz = (cbkWCCalibration.Checked == true) ? "true" : "false",//是否要检
                    wcjzProjectName = txtWCCalibration.Text.Trim(),//项目名称
                    ProcolWCJZ = cmbProcolWCJZ.Text.Trim(),//协议
                    IdentityWCJZ = txtIdentityWCJZ.Text.Trim(),//标识编码
                    wcjzfangfa = cbmjzfangfa.Text.Trim(),//校准方法                  
                    wcjzbzxdl = cmbjzxdl.Text.Trim(),//标准小电流       
                    // wcjzinifile = txtIniFile.Text.Trim(),//标准ini文件 
                    wcjzbz = wcjz16,
                    #region 误差校准参数 18个
                    Param1 = txtParam1.Text.Trim(),
                    Param2 = txtParam2.Text.Trim(),
                    Param3 = txtParam3.Text.Trim(),
                    Param4 = txtParam4.Text.Trim(),
                    Param5 = txtParam5.Text.Trim(),
                    Param6 = txtParam6.Text.Trim(),
                    Param7 = txtParam7.Text.Trim(),
                    Param8 = txtParam8.Text.Trim(),
                    Param9 = txtParam9.Text.Trim(),
                    Param10 = txtParam10.Text.Trim(),
                    Param11 = txtParam11.Text.Trim(),
                    Param12 = txtParam12.Text.Trim(),
                    Param13 = txtParam13.Text.Trim(),
                    Param14 = txtParam14.Text.Trim(),
                    Param15 = txtParam15.Text.Trim(),
                    Param16 = txtParam16.Text.Trim(),
                    Param17 = txtParam17.Text.Trim(),
                    Param18 = txtParam18.Text.Trim(),
                    ParamName1 = lblParam1.Text.Trim(),
                    ParamName2 = lblParam2.Text.Trim(),
                    ParamName3 = lblParam3.Text.Trim(),
                    ParamName4 = lblParam4.Text.Trim(),
                    ParamName5 = lblParam5.Text.Trim(),
                    ParamName6 = lblParam6.Text.Trim(),
                    ParamName7 = lblParam7.Text.Trim(),
                    ParamName8 = lblParam8.Text.Trim(),
                    ParamName9 = lblParam9.Text.Trim(),
                    ParamName10 = lblParam10.Text.Trim(),
                    ParamName11 = lblParam11.Text.Trim(),
                    ParamName12 = lblParam12.Text.Trim(),
                    ParamName13 = lblParam13.Text.Trim(),
                    ParamName14 = lblParam14.Text.Trim(),
                    ParamName15 = lblParam15.Text.Trim(),
                    ParamName16 = lblParam16.Text.Trim(),
                    ParamName17 = lblParam17.Text.Trim(),
                    ParamName18 = lblParam18.Text.Trim(),
                    #endregion
                    #endregion

                    #region 时钟校准信息
                    valueszjz = (cbkTimeCalibration.Checked == true) ? "true" : "false",//是否要检
                    szjzProjectName = txtszjzProjectName.Text.Trim(),//项目名称
                    TimeCheckProcol = cmbTimeCheckProcol.Text.Trim(),//协议
                    TimeIdentity = txtTimeIdentity.Text.Trim(),//标识编码
                    TimeDataLength = txtTimeDateLength.Text.Trim(),//数据长度
                    TimePoint = txtTimePoint.Text.Trim(),//小数点
                    TimeSend = txtTimeSend.Text.Trim(),//下发数 
                    #endregion

                    #region 红外读版本号
                    valuehw = (cbkhw.Checked == true) ? "true" : "false",//是否要检
                    hwProjectName = txthwProjectName.Text.Trim(),//项目名称
                    hwProcol = cmbhwProcol.Text.Trim(),//协议
                    hwIdentity = txthwIdentity.Text.Trim(),//标识编码  
                    hwDataLength = txthwDataLength.Text.Trim(),//数据长度
                    hwPoint = txthwPoint.Text.Trim(),//小数点
                    hwSend = txthwSend.Text.Trim(),//下发参数 
                    #endregion

                    #region 读整机功耗
                    valuegh = (cbkgh.Checked == true) ? "true" : "false",//是否要检
                    ghProjectName = txtgh.Text.Trim(),//项目名称
                    ghPmin = txtghPmin.Text.Trim(),//功率最小值
                    ghPmax = txtghPmax.Text.Trim(),//功率最大值
                    #endregion

                    #region 整机自检
                    valuezj = (cbkzj.Checked == true) ? "true" : "false",//是否要检
                    zjProjectName = txtzjProjectName.Text.Trim(),//项目名称
                    M1Bit = M1Bitstr,//M1
                    M2 = cmbM2.SelectedIndex.ToString(),//M2
                    fourBit = txtfourBit.Text.Trim(),//测试需判断项4个字节
                    #endregion

                    #region 初始化参数
                    valueIniParam = (chkIniParam.Checked == true) ? "true" : "false",//是否要检
                    ProjectNameIniParam = txtIniParam.Text.Trim(),//项目名称
                    ProcolIniParam = cmbIniParam.Text.Trim(),//协议
                    IdentityNOIniParam = txtIdentityNOIniParam.Text.Trim().ToUpper(),//标识编码  
                    DataLengthIniParam = txtDataLengthIniParam.Text.Trim(),//数据长度
                    PointIniParam = txtPointIniParam.Text.Trim(),//小数点
                    SendNumberIniParam = txtSendParamIniParam.Text.Trim(),//下发参数 
                    #endregion
                };
                p.Save(path, OrBitUserName);
                if (path == "0")
                {
                    MessageBox.Show("新增成功", "提示");
                }
                else if (path == "1")
                {
                    MessageBox.Show("修改成功", "提示");
                    // MessageBox.Show("修改成功", "提示",MessageBoxButtons.YesNo);
                }
                else
                {
                    MessageBox.Show("保存成功", "提示");
                }
            }
            catch (Exception E)
            {
                MessageBox.Show("保存失败" + E.Message, "提示");
            }
        }
        private void cbkParameter_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cbk = (CheckBox)sender;
            foreach (TabPage item in tabControl2.Controls.OfType<TabPage>())
            {
                item.Visible = false;
                if (item.Tag.ToString() == cbk.Tag.ToString())
                {
                    item.Visible = true;
                    tabControl2.SelectedIndex = Convert.ToInt32(cbk.Tag.ToString());
                    break;
                }
            }
        }

        #region 判断是否为数字
        public bool IsNumber(string strNum)
        {
            bool flag = true;
            int flgcount = 0;
            string NumAll = "0123456789.";
            for (int i = 0; i < strNum.Length; i++)
            {
                if (strNum[i].ToString() == ".")
                {
                    flgcount++;
                }
                if (!NumAll.Contains(strNum[i].ToString()))
                {
                    flag = false;
                    break;
                }
            }
            //if (strNum == "")
            //{
            //    flag = false;
            //}
            if (flgcount > 1)
            {
                flag = false;
            }
            return flag;
        }
        #endregion

        #region 判断是否为整数
        public bool isNumeric(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                int chr = Convert.ToInt32(str[i]);
                if (chr < 48 || chr > 57)
                    return false;
            }
            return true;
        }
        #endregion

        #region 判断是否是16进制数
        public bool Is16Number(string strNum)
        {
            bool flag = true;
            string NumAll = "0123456789ABCDEFabcdef";
            for (int i = 0; i < strNum.Length; i++)
            {
                if (!NumAll.Contains(strNum[i].ToString()))
                {
                    flag = false;
                    break;
                }
            }
            return flag;
        }
        #endregion


        #region  作业方案
        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl2.SelectedIndex == 0)//产品信息
            {

                cbkProduct.BackColor = System.Drawing.Color.DarkGray;
            }
            else
            {
                cbkProduct.BackColor = System.Drawing.Color.WhiteSmoke;
            }

            if (tabControl2.SelectedIndex == 1)//读电能量
            {

                cbkReadPower.BackColor = System.Drawing.Color.DarkGray;
            }
            else
            {
                cbkReadPower.BackColor = System.Drawing.Color.WhiteSmoke;
            }

            if (tabControl2.SelectedIndex == 2)//读生产编号
            {

                cbkReadProctNO.BackColor = System.Drawing.Color.DarkGray;
            }
            else
            {
                cbkReadProctNO.BackColor = System.Drawing.Color.WhiteSmoke;
            }
            if (tabControl2.SelectedIndex == 3) //误差校准
            {

                cbkWCCalibration.BackColor = System.Drawing.Color.DarkGray;
            }
            else
            {
                cbkWCCalibration.BackColor = System.Drawing.Color.WhiteSmoke;
            }

            if (tabControl2.SelectedIndex == 4)//时钟校准
            {

                cbkTimeCalibration.BackColor = System.Drawing.Color.DarkGray;
            }
            else
            {
                cbkTimeCalibration.BackColor = System.Drawing.Color.WhiteSmoke;
            }

            if (tabControl2.SelectedIndex == 5)//误差检定信息(1回路)
            {

                cbkhw.BackColor = System.Drawing.Color.DarkGray;
            }
            else
            {
                cbkhw.BackColor = System.Drawing.Color.WhiteSmoke;
            }

            if (tabControl2.SelectedIndex == 6)//多功能试验
            {

                cbkgh.BackColor = System.Drawing.Color.DarkGray;
            }
            else
            {
                cbkgh.BackColor = System.Drawing.Color.WhiteSmoke;
            }

            if (tabControl2.SelectedIndex == 7)//打包参数下载
            {
                cbkzj.BackColor = System.Drawing.Color.DarkGray;
            }
            else
            {
                cbkzj.BackColor = System.Drawing.Color.WhiteSmoke;
            }
            if (tabControl2.SelectedIndex == 8)//打包参数下载
            {
                chkIniParam.BackColor = System.Drawing.Color.DarkGray;
            }
            else
            {
                chkIniParam.BackColor = System.Drawing.Color.WhiteSmoke;
            }
        }
        #endregion

        #region 系统清零验证


        private void ProductNOParamXYCmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ProductNOParamXYCmb.SelectedIndex == 0)
            {
                txtIdentityNO.MaxLength = 4;
            }
            else
            {
                txtIdentityNO.MaxLength = 8;
            }
            if (txtIdentityNO.Text.Length > txtIdentityNO.MaxLength)
            {
                txtIdentityNO.Text = "";
            }
        }

        private void cmbReadPowerProtcol_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbReadPowerProtcol.SelectedIndex == 0)
            {
                txtReadPowerIdentityNO.MaxLength = 4;
            }
            else
            {
                txtReadPowerIdentityNO.MaxLength = 8;
            }
            if (txtReadPowerIdentityNO.Text.Length > txtReadPowerIdentityNO.MaxLength)
            {
                txtReadPowerIdentityNO.Text = "";
            }
        }
        #endregion

        #region 读生产编号数据验证
        private void txtIdentityNO_TextChanged(object sender, EventArgs e)
        {
            if (!Is16Number(txtIdentityNO.Text.Trim()))
            {
                MessageBox.Show("标识编码:" + txtIdentityNO.Text + "不是十六进制数", "提示");
                txtIdentityNO.Text = "";
            }
        }

        private void txtProductNODataLength_TextChanged(object sender, EventArgs e)
        {
            if (!isNumeric(txtProductNODataLength.Text.Trim()))
            {
                MessageBox.Show("数据长度:" + txtProductNODataLength.Text + "不是整数", "提示");
                txtProductNODataLength.Text = "";
            }
        }

        private void txtProductNOPoint_TextChanged(object sender, EventArgs e)
        {
            if (!isNumeric(txtProductNOPoint.Text.Trim()))
            {
                MessageBox.Show("小数点:" + txtProductNOPoint.Text + "不是整数", "提示");
                txtProductNOPoint.Text = "";
            }
        }

        private void txtProductNOSendParameter_TextChanged(object sender, EventArgs e)
        {
            if (!isNumeric(txtProductNOSendParameter.Text.Trim()))
            {
                MessageBox.Show("下发参数:" + txtProductNOSendParameter.Text + "不是整数", "提示");
                txtProductNOSendParameter.Text = "";
            }
        }
        #endregion

        #region 读电能量输入验证
        private void txtReadPowerIdentityNO_TextChanged(object sender, EventArgs e)
        {
            if (!Is16Number(txtReadPowerIdentityNO.Text.Trim()))
            {
                MessageBox.Show("标识编码:" + txtReadPowerIdentityNO.Text + "不是十六进制数", "提示");
                txtReadPowerIdentityNO.Text = "";
            }
        }

        private void txtReadPowerDataLength_TextChanged(object sender, EventArgs e)
        {
            if (!isNumeric(txtReadPowerDataLength.Text.Trim()))
            {
                MessageBox.Show("数据长度:" + txtReadPowerDataLength.Text + "不是整数", "提示");
                txtReadPowerDataLength.Text = "";
            }
        }

        private void txtReadPowerPoint_TextChanged(object sender, EventArgs e)
        {
            if (!isNumeric(txtReadPowerPoint.Text.Trim()))
            {
                MessageBox.Show("小数点:" + txtReadPowerPoint.Text + "不是整数", "提示");
                txtReadPowerPoint.Text = "";
            }
        }

        private void txtReadPowerSendParameter_TextChanged(object sender, EventArgs e)
        {
            if (!isNumeric(txtReadPowerSendParameter.Text.Trim()))
            {
                MessageBox.Show("下发参数:" + txtReadPowerSendParameter.Text + "不是整数", "提示");
                txtReadPowerSendParameter.Text = "";
            }
        }
        #endregion

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.Filter = "Config Files|*.xml|txt Files|*.txt|All Files|*.*";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {

                string FileNamestr = fileDialog.FileName.Substring(fileDialog.FileName.LastIndexOf("\\") + 1, fileDialog.FileName.LastIndexOf(".") - fileDialog.FileName.LastIndexOf("\\") - 1);
                functionCheckSchemaCmb.Text = FileNamestr;
                IniFile.FilePath = fileDialog.FileName;
                try
                {
                    ViewSchem(fileDialog.FileName);
                }
                catch
                {

                }
            }
        }

        #region 导入方案
        public void ViewSchem(string path)
        {
            XDocument d = XDocument.Load(path);

            #region 表型号
            XElement WorkInformation = d.Root.Elements("Work").Where(el => el.Attribute("Description").Value == "工单信息").First();
            XElement elementtxtModel = WorkInformation.Elements("Work").Where(el => el.Attribute("ID").Value == "ProductsModel").First(); //表型号
            txtModel.Text = elementtxtModel.Attribute("Value").Value;//表型号
            #endregion

            #region 产品信息
            try
            {
                XElement ProductInformation = d.Root.Elements("Products").Where(el => el.Attribute("Description").Value == "产品信息").First();
                //XElement elementtxtModel = ProductInformation.Elements("Products").Where(el => el.Attribute("ID").Value == "Model").First(); //表型号
                //XElement elementtxtModelDiscription = ProductInformation.Elements("Products").Where(el => el.Attribute("ID").Value == "ModelDiscription").First(); //型号描述
                XElement elementtxtTestType = ProductInformation.Elements("Products").Where(el => el.Attribute("ID").Value == "Clfs").First(); //测量方式
                XElement elementtxtBasicV = ProductInformation.Elements("Products").Where(el => el.Attribute("ID").Value == "Ub").First();//基本电压
                XElement elementtxtBasicI = ProductInformation.Elements("Products").Where(el => el.Attribute("ID").Value == "Ib").First();//基本电流
                XElement elementtxtMaxI = ProductInformation.Elements("Products").Where(el => el.Attribute("ID").Value == "IMax").First();//最大电流
                XElement elementtxtConst = ProductInformation.Elements("Products").Where(el => el.Attribute("ID").Value == "Constant").First();//常数
                XElement elementtxtLevel = ProductInformation.Elements("Products").Where(el => el.Attribute("ID").Value == "DJ").First();//等级
                XElement elementtxtHZ = ProductInformation.Elements("Products").Where(el => el.Attribute("ID").Value == "PL").First();//频率
                XElement elementcbmdzleixing = ProductInformation.Elements("Products").Where(el => el.Attribute("ID").Value == "DzType").First();//端子类型
                XElement elementtxtgongyinggongyang = ProductInformation.Elements("Products").Where(el => el.Attribute("ID").Value == "GYGY").First();//共阴共阳
                XElement elementtxtMaiChong = ProductInformation.Elements("Products").Where(el => el.Attribute("ID").Value == "PulseType").First();//脉冲类型
                XElement elementtxtJiDianQi = ProductInformation.Elements("Products").Where(el => el.Attribute("ID").Value == "JDQType").First();//继电器类型
                XElement elementtxtRevSoftNo = ProductInformation.Elements("Products").Where(el => el.Attribute("ID").Value == "SoftVer").First();//软件版本号
                //txtModel.Text = elementtxtModel.Attribute("Value").Value;//表型号
                //txtModelDiscription.Text = elementtxtModelDiscription.Attribute("Value").Value;//型号描述
                txtTestType.Text = elementtxtTestType.Attribute("Value").Value;//测量方式
                txtBasicV.Text = elementtxtBasicV.Attribute("Value").Value.Replace("V", "");//电压
                txtBasicI.Text = elementtxtBasicI.Attribute("Value").Value.Replace("A", "");//电流
                txtMaxI.Text = elementtxtMaxI.Attribute("Value").Value.Replace("A", "");//Max电流
                txtConst.Text = elementtxtConst.Attribute("Value").Value;//常数
                txtLevel.Text = elementtxtLevel.Attribute("Value").Value;//等级
                txtHZ.Text = elementtxtHZ.Attribute("Value").Value; //频率
                txtDuanZi.Text = elementcbmdzleixing.Attribute("Value").Value; //端子类型
                txtgongyinggongyang.Text = elementtxtgongyinggongyang.Attribute("Value").Value; //共阴共阳
                txtMaiChong.Text = elementtxtMaiChong.Attribute("Value").Value; //脉冲类型
                txtJiDianQi.Text = elementtxtJiDianQi.Attribute("Value").Value; //继电器类型
                txtRevSoftNo.Text = elementtxtRevSoftNo.Attribute("Value").Value; //软件版本号
            }
            catch
            {

            }
            #endregion

            #region 读电能量
            try
            {
                XElement ReadPower = d.Root.Elements("Plan_ReadEnergy").Where(el => el.Attribute("Description").Value == "读电能量").First();
                XElement elementcmbReadPowerProtcol = ReadPower.Elements("Plan_ReadEnergy").Where(el => el.Attribute("ID").Value == "ProductsName").First(); //项目参数
                XElement elementcbkReadPower = ReadPower.Elements("Plan_ReadEnergy").Where(el => el.Attribute("ID").Value == "IsCheck").First();//是否检验
                cbkReadPower.Checked = elementcbkReadPower.Attribute("Value").Value == "true" ? true : false;
                if (cbkReadPower.Checked == true)
                {
                    string[] ReadPowerParams = elementcmbReadPowerProtcol.Attribute("Value").Value.Split('|');
                    cmbReadPowerProtcol.Text = ReadPowerParams[0];//协议
                    txtReadPowerIdentityNO.Text = ReadPowerParams[1];//标识编码
                    txtReadPowerDataLength.Text = ReadPowerParams[2];//数据长度
                    txtReadPowerPoint.Text = ReadPowerParams[3];//小数点
                    txtReadPowerSendParameter.Text = ReadPowerParams[4];//下发参数                   
                }
            }
            catch
            {

            }
            #endregion

            #region 读生产编号信息
            try
            {
                XElement ReadProductNO = d.Root.Elements("Plan_ReadScbh").Where(el => el.Attribute("Description").Value == "读生产编号信息").First();
                XElement elementcmbReadProductNO = ReadProductNO.Elements("Plan_ReadScbh").Where(el => el.Attribute("ID").Value == "ProductsName").First(); //项目参数
                XElement elementcbkReadProctNO = ReadProductNO.Elements("Plan_ReadScbh").Where(el => el.Attribute("ID").Value == "IsCheck").First();//是否检验
                cbkReadProctNO.Checked = elementcbkReadProctNO.Attribute("Value").Value == "true" ? true : false;
                if (cbkReadProctNO.Checked == true)
                {
                    string[] ReadProductNOParams = elementcmbReadProductNO.Attribute("Value").Value.Split('|');
                    ProductNOParamXYCmb.Text = ReadProductNOParams[0];//协议
                    txtIdentityNO.Text = ReadProductNOParams[1];//标识编码
                    txtProductNODataLength.Text = ReadProductNOParams[2];//数据长度
                    txtProductNOPoint.Text = ReadProductNOParams[3];//小数点
                    txtProductNOSendParameter.Text = ReadProductNOParams[4];//下发参数
                }
            }
            catch
            {

            }
            #endregion

            #region 误差校准信息
            try
            {
                XElement Wcjz = d.Root.Elements("Plan_AdjustError").Where(el => el.Attribute("Description").Value == "校准误差").First();
                XElement elementWCCalibration = Wcjz.Elements("Plan_AdjustError").Where(el => el.Attribute("ID").Value == "ProductsName").First(); //项目参数
                XElement elementcbkWCCalibration = Wcjz.Elements("Plan_AdjustError").Where(el => el.Attribute("ID").Value == "IsCheck").First();//是否检验
                cbkWCCalibration.Checked = elementcbkWCCalibration.Attribute("Value").Value == "true" ? true : false;
                if (cbkWCCalibration.Checked == true)
                {
                    string[] WCCalibrationParams = elementWCCalibration.Attribute("Value").Value.Split('|');
                    cmbProcolWCJZ.Text = WCCalibrationParams[0];//协议
                    txtIdentityWCJZ.Text = WCCalibrationParams[1];//标识编码
                    cbmjzfangfa.Text = WCCalibrationParams[2];//校准方法;
                    // cbmdzleixing.SelectedIndex = WCCalibrationParams[3] == "国网表" ? 0 : 1;//端子型号
                    cmbjzxdl.Text = WCCalibrationParams[3];//标准小电流选择 
                    //  txtIniFile.Text = elementWCCalibration.Attribute("IniFile").Value;//校准INI文件
                    #region 步骤
                    string wcjz16 = WCCalibrationParams[4];
                    string wcjz2 = Convert.ToString(Convert.ToInt32(wcjz16, 16), 2).PadLeft(7, '0');
                    #region 步骤⑦
                    if (wcjz2[0] == '0')
                    {
                        cb7.Checked = false;
                    }
                    else
                    {
                        cb7.Checked = true;
                    }
                    #endregion
                    #region 步骤⑥
                    if (wcjz2[1] == '0')
                    {
                        cb6.Checked = false;
                    }
                    else
                    {
                        cb6.Checked = true;
                    }
                    #endregion
                    #region 步骤⑤
                    if (wcjz2[2] == '0')
                    {
                        cb5.Checked = false;
                    }
                    else
                    {
                        cb5.Checked = true;
                    }
                    #endregion
                    #region 步骤④
                    if (wcjz2[3] == '0')
                    {
                        cb4.Checked = false;
                    }
                    else
                    {
                        cb4.Checked = true;
                    }
                    #endregion
                    #region 步骤③
                    if (wcjz2[4] == '0')
                    {
                        cb3.Checked = false;
                    }
                    else
                    {
                        cb3.Checked = true;
                    }
                    #endregion
                    #region 步骤②
                    if (wcjz2[5] == '0')
                    {
                        cb2.Checked = false;
                    }
                    else
                    {
                        cb2.Checked = true;
                    }
                    #endregion
                    #region 步骤①
                    if (wcjz2[6] == '0')
                    {
                        cb1.Checked = false;
                    }
                    else
                    {
                        cb1.Checked = true;
                    }
                    #endregion
                    #endregion
                    XElement WcjzItem = d.Root.Elements("Plan_AdjustErrorPara_Item").Where(el => el.Attribute("Description").Value == "校准误差参数文件").First();
                    txtParam1.Text = WcjzItem.Elements("Plan_AdjustErrorPara_Item").Where(el => el.Attribute("ParaID").Value == "1").First().Attribute("ParaValue").Value;
                    txtParam2.Text = WcjzItem.Elements("Plan_AdjustErrorPara_Item").Where(el => el.Attribute("ParaID").Value == "2").First().Attribute("ParaValue").Value;
                    txtParam3.Text = WcjzItem.Elements("Plan_AdjustErrorPara_Item").Where(el => el.Attribute("ParaID").Value == "3").First().Attribute("ParaValue").Value;
                    txtParam4.Text = WcjzItem.Elements("Plan_AdjustErrorPara_Item").Where(el => el.Attribute("ParaID").Value == "4").First().Attribute("ParaValue").Value;
                    txtParam5.Text = WcjzItem.Elements("Plan_AdjustErrorPara_Item").Where(el => el.Attribute("ParaID").Value == "5").First().Attribute("ParaValue").Value;
                    txtParam6.Text = WcjzItem.Elements("Plan_AdjustErrorPara_Item").Where(el => el.Attribute("ParaID").Value == "6").First().Attribute("ParaValue").Value;
                    txtParam7.Text = WcjzItem.Elements("Plan_AdjustErrorPara_Item").Where(el => el.Attribute("ParaID").Value == "7").First().Attribute("ParaValue").Value;
                    txtParam8.Text = WcjzItem.Elements("Plan_AdjustErrorPara_Item").Where(el => el.Attribute("ParaID").Value == "8").First().Attribute("ParaValue").Value;
                    txtParam9.Text = WcjzItem.Elements("Plan_AdjustErrorPara_Item").Where(el => el.Attribute("ParaID").Value == "9").First().Attribute("ParaValue").Value;
                    txtParam10.Text = WcjzItem.Elements("Plan_AdjustErrorPara_Item").Where(el => el.Attribute("ParaID").Value == "10").First().Attribute("ParaValue").Value;
                    txtParam11.Text = WcjzItem.Elements("Plan_AdjustErrorPara_Item").Where(el => el.Attribute("ParaID").Value == "11").First().Attribute("ParaValue").Value;
                    txtParam12.Text = WcjzItem.Elements("Plan_AdjustErrorPara_Item").Where(el => el.Attribute("ParaID").Value == "12").First().Attribute("ParaValue").Value;
                    txtParam13.Text = WcjzItem.Elements("Plan_AdjustErrorPara_Item").Where(el => el.Attribute("ParaID").Value == "13").First().Attribute("ParaValue").Value;
                    txtParam14.Text = WcjzItem.Elements("Plan_AdjustErrorPara_Item").Where(el => el.Attribute("ParaID").Value == "14").First().Attribute("ParaValue").Value;
                    txtParam15.Text = WcjzItem.Elements("Plan_AdjustErrorPara_Item").Where(el => el.Attribute("ParaID").Value == "15").First().Attribute("ParaValue").Value;
                    txtParam16.Text = WcjzItem.Elements("Plan_AdjustErrorPara_Item").Where(el => el.Attribute("ParaID").Value == "16").First().Attribute("ParaValue").Value;
                    txtParam17.Text = WcjzItem.Elements("Plan_AdjustErrorPara_Item").Where(el => el.Attribute("ParaID").Value == "17").First().Attribute("ParaValue").Value;
                    txtParam18.Text = WcjzItem.Elements("Plan_AdjustErrorPara_Item").Where(el => el.Attribute("ParaID").Value == "18").First().Attribute("ParaValue").Value;
                }
            }
            catch
            { }
            #endregion

            #region 时钟校准
            try
            {
                XElement Szjz = d.Root.Elements("Plan_AdjustClock").Where(el => el.Attribute("Description").Value == "校准时钟").First();
                XElement elementTimeCalibration = Szjz.Elements("Plan_AdjustClock").Where(el => el.Attribute("ID").Value == "ProductsName").First();
                XElement elementcbkTimeCalibration = Szjz.Elements("Plan_AdjustClock").Where(el => el.Attribute("ID").Value == "IsCheck").First();//是否检验
                cbkTimeCalibration.Checked = elementcbkTimeCalibration.Attribute("Value").Value == "true" ? true : false;
                if (cbkTimeCalibration.Checked == true)
                {
                    string[] AdjustClockParams = elementTimeCalibration.Attribute("Value").Value.Split('|');
                    cmbTimeCheckProcol.Text = AdjustClockParams[0];//协议
                    txtTimeIdentity.Text = AdjustClockParams[1];//标识编码
                    txtTimeDateLength.Text = AdjustClockParams[2];//数据长度
                    txtTimePoint.Text = AdjustClockParams[3];//小数点
                    txtTimeSend.Text = AdjustClockParams[4];//下发参数
                }
            }
            catch
            {
            }
            #endregion

            #region RS485读版本号
            try
            {
                XElement hw = d.Root.Elements("Plan_ReadVer").Where(el => el.Attribute("Description").Value == "RS485读版本号").First();
                XElement elementcmbhw = hw.Elements("Plan_ReadVer").Where(el => el.Attribute("ID").Value == "ProductsName").First(); //项目参数
                XElement elementcbkhw = hw.Elements("Plan_ReadVer").Where(el => el.Attribute("ID").Value == "IsCheck").First();//是否检验
                cbkhw.Checked = elementcbkhw.Attribute("Value").Value == "true" ? true : false;
                if (cbkhw.Checked == true)
                {
                    string[] hwParams = elementcmbhw.Attribute("Value").Value.Split('|');
                    cmbhwProcol.Text = hwParams[0];//协议
                    txthwIdentity.Text = hwParams[1];//标识编码
                    txthwDataLength.Text = hwParams[2];//数据长度
                    txthwPoint.Text = hwParams[3];//小数点
                    txthwSend.Text = hwParams[4];//下发参数
                }
            }
            catch
            {

            }
            #endregion

            #region 读整机功耗
            try
            {
                XElement gh = d.Root.Elements("Plan_ReadPower").Where(el => el.Attribute("Description").Value == "读整机功耗").First();
                XElement elementcmbgh = gh.Elements("Plan_ReadPower").Where(el => el.Attribute("ID").Value == "ProductsName").First(); //项目参数
                XElement elementcbkgh = gh.Elements("Plan_ReadPower").Where(el => el.Attribute("ID").Value == "IsCheck").First();//是否检验
                cbkgh.Checked = elementcbkgh.Attribute("Value").Value == "true" ? true : false;
                if (cbkgh.Checked == true)
                {
                    string[] hwParams = elementcmbgh.Attribute("Value").Value.Split('|');
                    txtghPmin.Text = hwParams[0];//功率最小值
                    txtghPmax.Text = hwParams[1];//功率最大值
                }
            }
            catch
            {

            }


            #endregion

            #region 整机自检
            try
            {
                XElement zj = d.Root.Elements("Plan_SelfCheck").Where(el => el.Attribute("Description").Value == "整机自检").First();
                XElement elementcmbzj = zj.Elements("Plan_SelfCheck").Where(el => el.Attribute("ID").Value == "ProductsName").First(); //项目参数
                XElement elementcbkzj = zj.Elements("Plan_SelfCheck").Where(el => el.Attribute("ID").Value == "IsCheck").First();//是否检验
                cbkzj.Checked = elementcbkzj.Attribute("Value").Value == "true" ? true : false;
                if (cbkzj.Checked == true)
                {
                    string[] hwParams = elementcmbzj.Attribute("Value").Value.Split('|');
                    string strM1 = hwParams[0];
                    string strM2 = hwParams[1];
                    switch (strM2)
                    {
                        case "0": cmbM2.Text = "外控磁保持";
                            break;
                        case "1": cmbM2.Text = "内控磁保持";
                            break;
                        case "2": cmbM2.Text = "外控电保持";
                            break;
                        default:
                            break;
                    }
                    txtfourBit.Text = hwParams[2];
                    if (strM1 == "0")
                    {
                        cmbM1Bit0.Text = "单板测试";
                        cmbM1Bit1.Text = "三相四";
                    }
                    if (strM1 == "1")
                    {
                        cmbM1Bit0.Text = "单板测试";
                        cmbM1Bit1.Text = "三相三";
                    }
                    if (strM1 == "2")
                    {
                        cmbM1Bit0.Text = "整机测试";
                        cmbM1Bit1.Text = "三相四";
                    }
                    if (strM1 == "3")
                    {
                        cmbM1Bit0.Text = "整机测试";
                        cmbM1Bit1.Text = "三相三";
                    }
                }
            }
            catch
            {

            }
            #endregion

            #region 初始化参数
            try
            {
                XElement ReadProductNO = d.Root.Elements("Plan_InitPara").Where(el => el.Attribute("Description").Value == "初始化参数").First();
                XElement elementcmbIniParam = ReadProductNO.Elements("Plan_InitPara").Where(el => el.Attribute("ID").Value == "ProductsName").First(); //项目参数
                XElement elementchkIniParam = ReadProductNO.Elements("Plan_InitPara").Where(el => el.Attribute("ID").Value == "IsCheck").First();//是否检验
                chkIniParam.Checked = elementchkIniParam.Attribute("Value").Value == "true" ? true : false;
                if (chkIniParam.Checked == true)
                {
                    string[] IniParamParams = elementcmbIniParam.Attribute("Value").Value.Split('|');
                    cmbIniParam.Text = IniParamParams[0];//协议
                    txtIdentityNOIniParam.Text = IniParamParams[1];//标识编码
                    txtDataLengthIniParam.Text = IniParamParams[2];//数据长度
                    txtPointIniParam.Text = IniParamParams[3];//小数点
                    txtSendParamIniParam.Text = IniParamParams[4];//下发参数
                }
            }
            catch
            {

            }
            #endregion
            cbkProduct.Checked = true;
            tabControl2.SelectedIndex = 0;
        }
        #endregion



        private void btnChooseiniFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.Filter = "Config Files|*.ini|txt Files|*.txt|All Files|*.*";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                //txtIniFile.Text = string.Empty;
                IniFile.FilePath = fileDialog.FileName;
                try
                {
                    txtParam1.Text = IniFile.IniReadValue("AdjustParaMeter_GW", "1");
                    txtParam2.Text = IniFile.IniReadValue("AdjustParaMeter_GW", "2");
                    txtParam3.Text = IniFile.IniReadValue("AdjustParaMeter_GW", "3");
                    txtParam4.Text = IniFile.IniReadValue("AdjustParaMeter_GW", "4");
                    txtParam5.Text = IniFile.IniReadValue("AdjustParaMeter_GW", "5");
                    txtParam6.Text = IniFile.IniReadValue("AdjustParaMeter_GW", "6");
                    txtParam7.Text = IniFile.IniReadValue("AdjustParaMeter_GW", "7");
                    txtParam8.Text = IniFile.IniReadValue("AdjustParaMeter_GW", "8");
                    txtParam9.Text = IniFile.IniReadValue("AdjustParaMeter_GW", "9");
                    txtParam10.Text = IniFile.IniReadValue("AdjustParaMeter_GW", "10");
                    txtParam11.Text = IniFile.IniReadValue("AdjustParaMeter_GW", "11");
                    txtParam12.Text = IniFile.IniReadValue("AdjustParaMeter_GW", "12");
                    txtParam13.Text = IniFile.IniReadValue("AdjustParaMeter_GW", "13");
                    txtParam14.Text = IniFile.IniReadValue("AdjustParaMeter_GW", "14");
                    txtParam15.Text = IniFile.IniReadValue("AdjustParaMeter_GW", "15");
                    txtParam16.Text = IniFile.IniReadValue("AdjustParaMeter_GW", "16");
                    txtParam17.Text = IniFile.IniReadValue("AdjustParaMeter_GW", "17");
                    txtParam18.Text = IniFile.IniReadValue("AdjustParaMeter_GW", "18");
                    cbkWCCalibration.Checked = true;
                }
                catch
                {

                }
            }
        }





        private void txtTimeIdentity_TextChanged(object sender, EventArgs e)
        {
            if (!Is16Number(txtTimeIdentity.Text.Trim()))
            {
                MessageBox.Show("标识编码:" + txtTimeIdentity.Text + "不是十六进制数", "提示");
                txtTimeIdentity.Text = "";
            }
        }

        private void txtIdentityWCJZ_TextChanged(object sender, EventArgs e)
        {
            if (!Is16Number(txtIdentityWCJZ.Text.Trim()))
            {
                MessageBox.Show("标识编码:" + txtIdentityWCJZ.Text + "不是十六进制数", "提示");
                txtIdentityWCJZ.Text = "";
            }
        }

        private void btnOtherSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "xml文件|*.xml";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string path = dlg.FileName;
                SaveFile(path);//方案另存为,传入参数为另存为的地址
                //BindShemaName();
            }
        }
        #endregion

        private void functionCheckSchemaCmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            string path = Application.StartupPath + "\\" + functionCheckSchemaCmb.Text.Trim() + ".xml";
            string sql = "";
            DataTable ShemaDT = new DataTable();
            try
            {
                //sql = "select FileXML,MoName,SchemaDescription from CLTestSchema where SchemaNameNew='" + functionCheckSchemaCmb.Text.Trim() + "' and SchemaType='20' and MOName='" + txtModel.Text.Trim() + "' and SchemaName='" + lbProductName.Text.Trim() + "'";

                sql = "exec GetShemaDT @SchemaNameNew='" + functionCheckSchemaCmb.Text.Trim() + "',@SchemaType='20',@MOName='" + txtModel.Text.Trim() + "',@SchemaName='" + lbProductName.Text.Trim() + "',@SchemaArea='国网'";
                ShemaDT = GetDataSetWithSQLString(sql).Tables[0];
            }
            catch
            {

            }

            if (ShemaDT.Rows.Count != 0)//如果方案存在 开始加载方案
            {
                txtSchemaDescription.Text = ShemaDT.Rows[0][2].ToString().Trim();
                StreamWriter sw = new StreamWriter(path);
                sw.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" + ShemaDT.Rows[0]["FileXML"].ToString().Trim());
                sw.Close();
                sw.Dispose();
                ViewSchem(path);

            }
            else
            {
                cbkReadPower.Checked = false;
                //cbkReadProctNO.Checked = false;
                cbkWCCalibration.Checked = false;
                cbkTimeCalibration.Checked = false;
                cbkhw.Checked = false;
                cbkgh.Checked = false;
                cbkzj.Checked = false;
                txtRevSoftNo.Text = txtSchemaDescription.Text = String.Empty;
            }
            tabControl2.SelectedIndex = 0;
            btnSave.Focus();
            //}
            //else
            //{
            //    txtTestType.Text = txtBasicV.Text = txtBasicI.Text = txtMaxI.Text = txtConst.Text = txtLevel.Text = txtHZ.Text = txtDuanZi.Text = txtMaiChong.Text = txtJiDianQi.Text = txtRevSoftNo.Text = txtgongyinggongyang.Text = String.Empty;
            //}

        }
        private void cmbhwProcol_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbhwProcol.SelectedIndex == 0)
            {
                txthwIdentity.MaxLength = 4;
            }
            else
            {
                txthwIdentity.MaxLength = 8;
            }
            if (txthwIdentity.Text.Length > txthwIdentity.MaxLength)
            {
                txthwIdentity.Text = "";
            }
        }

        private void cmbProcolWCJZ_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProcolWCJZ.SelectedIndex == 0)
            {
                txtIdentityWCJZ.MaxLength = 4;
            }
            else
            {
                txtIdentityWCJZ.MaxLength = 8;
            }
            if (txtIdentityWCJZ.Text.Length > txtIdentityWCJZ.MaxLength)
            {
                txtIdentityWCJZ.Text = "";
            }
        }
        private void txtTimeDateLength_TextChanged(object sender, EventArgs e)
        {
            if (!isNumeric(txtTimeDateLength.Text.Trim()))
            {
                MessageBox.Show("数据长度:" + txtTimeDateLength.Text + "不是整数", "提示");
                txtTimeDateLength.Text = "";
            }
        }

        private void txtTimePoint_TextChanged(object sender, EventArgs e)
        {
            if (!isNumeric(txtTimePoint.Text.Trim()))
            {
                MessageBox.Show("小数点:" + txtTimePoint.Text + "不是整数", "提示");
                txtTimePoint.Text = "";
            }
        }

        private void txtTimeSend_TextChanged(object sender, EventArgs e)
        {
            //if (!isNumeric(txtTimeSend.Text.Trim()))
            //{
            //    MessageBox.Show("小数点:" + txtTimeSend.Text + "不是整数", "提示");
            //    txtTimeSend.Text = "";
            //}
        }

        private void txthwIdentity_TextChanged(object sender, EventArgs e)
        {
            if (!Is16Number(txthwIdentity.Text.Trim()))
            {
                MessageBox.Show("标识编码:" + txthwIdentity.Text + "不是十六进制数", "提示");
                txthwIdentity.Text = "";
            }
        }

        private void txthwDataLength_TextChanged(object sender, EventArgs e)
        {
            if (!isNumeric(txthwDataLength.Text.Trim()))
            {
                MessageBox.Show("数据长度:" + txthwDataLength.Text + "不是整数", "提示");
                txthwDataLength.Text = "";
            }
        }

        private void txthwPoint_TextChanged(object sender, EventArgs e)
        {
            if (!isNumeric(txthwPoint.Text.Trim()))
            {
                MessageBox.Show("小数点:" + txthwPoint.Text + "不是整数", "提示");
                txthwPoint.Text = "";
            }
        }

        private void txtghPmin_TextChanged(object sender, EventArgs e)
        {
            if (!IsNumber(txtghPmin.Text.Trim()))
            {
                MessageBox.Show("功率最小值:" + txtghPmin.Text + "不是数字", "提示");
                txtghPmin.Text = "";
            }
        }

        private void txtghPmax_TextChanged(object sender, EventArgs e)
        {
            if (!IsNumber(txtghPmax.Text.Trim()))
            {
                MessageBox.Show("功率最大值:" + txtghPmax.Text + "不是数字", "提示");
                txtghPmax.Text = "";
            }
        }

        private void txtfourBit_TextChanged(object sender, EventArgs e)
        {
            string fourbyte = txtfourBit.Text.Trim();
            if (!Is16Number(fourbyte))
            {
                MessageBox.Show(fourbyte + "不是十六进制数", "提示");
                txtfourBit.Text = "";
                return;
            }
            if (fourbyte.Length > 8)
            {
                //MessageBox.Show("输入值不能超过8位数", "提示");
                txtfourBit.Text = fourbyte.Substring(0, 8);
                return;
            }
            if (txtfourBit.Text.Trim() != "")
            {
                txtfourBit.Text = fourbyte.PadLeft(8, '0');
                string strfourByte = Convert.ToString(Convert.ToInt32(txtfourBit.Text, 16), 2).PadLeft(32, '0');
                if (strfourByte.Length == 32)
                {
                    if (lab1.Text == "0")
                    {
                        lab1.Text = "1";
                        ViewLushu(strfourByte);
                        lab1.Text = "0";
                    }
                }
            }
        }

        private void UserControl_Load(object sender, EventArgs e)
        {
            //BindShemaName();
            BindModel();

        }
        private void BindModel()
        {
            string sql = "exec GetModelNameForProductType @ProductFamilyName='电能表'";
            DataTable ModelNameDT = GetDataSetWithSQLString(sql).Tables[0];
            if (lbModel.Items.Count != 0)
            {
                lbModel.Items.Clear();
            }
            for (int i = 0; i < ModelNameDT.Rows.Count; i++)
            {
                lbModel.Items.Add(ModelNameDT.Rows[i][0].ToString().Trim());
            }
        }

        private void lbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                txtModel.Text = lbModel.Text.ToString().Trim();//表型号   
                string sql = "select ProductModelDescription from ProductModelRoot where ProductModelName='" + txtModel.Text + "'";
                DataTable ModelDiscription = GetDataSetWithSQLString(sql).Tables[0];
                if (ModelDiscription.Rows.Count != 0)
                {
                    txtModelDiscription.Text = ModelDiscription.Rows[0][0].ToString().Trim();//型号描述
                }
                else
                {
                    txtModelDiscription.Text = string.Empty;//型号描述
                } 
                sql = "exec GetProductName @ProductModelName='" + lbModel.Text.Trim() + "'";
                DataTable ProductNameDT = GetDataSetWithSQLString(sql).Tables[0];
                if (lbProductName.Items.Count != 0)
                {
                    lbProductName.Items.Clear();
                }
                for (int i = 0; i < ProductNameDT.Rows.Count; i++)
                {
                    lbProductName.Items.Add(ProductNameDT.Rows[i][1].ToString().Trim());
                }
                if (functionCheckSchemaCmb.Items.Count != 0)
                {
                    functionCheckSchemaCmb.Items.Clear();
                    functionCheckSchemaCmb.Text = "";
                    txtSchemaDescription.Text = "";
                } 
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        private void lbProductName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string sql = "exec GetProductName @ProductModelName='" + lbModel.Text.Trim() + "'";
                DataTable ProductNameDT = GetDataSetWithSQLString(sql).Tables[0];
                string ProductId = ProductNameDT.Rows[0][0].ToString(); 
                sql = "select * from ProductModel where ProductModelRootID in (select ProductModelRootID from ProductModelRoot where ProductModelName='" + txtModel.Text.Trim() + "') and ProductId='" + ProductId + "'";
                //通过表型号和成品料号取出物料中的信息
                DataTable ModelDT = GetDataSetWithSQLString(sql).Tables[0];
                if (ModelDT.Rows.Count != 0)//是否进行过表型号的维护
                {
                    txtTestType.Text = ModelDT.Rows[0]["TestWay"].ToString();//测量方式
                    txtBasicV.Text = ModelDT.Rows[0]["Voltage"].ToString().Replace("V", "");//基本电压
                    try
                    {
                        string I = ModelDT.Rows[0]["ICurrent"].ToString();
                        txtBasicI.Text = I.Substring(0, I.IndexOf("-")).Replace("A", "");//基本电流
                        txtMaxI.Text = I.Substring(I.IndexOf("-") + 1).Replace("A", "");//最大电流
                    }
                    catch
                    {

                    }
                    txtConst.Text = ModelDT.Rows[0]["Constant"].ToString();//常数
                    txtLevel.Text = ModelDT.Rows[0]["Level"].ToString();//等级
                    txtHZ.Text = ModelDT.Rows[0]["Rate"].ToString();//频率
                    txtDuanZi.Text = ModelDT.Rows[0]["TerminalType"].ToString();//端子类型
                    txtMaiChong.Text = ModelDT.Rows[0]["PulseType"].ToString();//脉冲类型
                    txtJiDianQi.Text = ModelDT.Rows[0]["RelayType"].ToString();//继电器类型
                    txtgongyinggongyang.Text = ModelDT.Rows[0]["GygyType"].ToString();//共阴共阳  

                    BindShemaName();
                }
                else//产品信息赋空
                {
                    txtTestType.Text = txtBasicV.Text = txtBasicI.Text = txtMaxI.Text = txtConst.Text = txtLevel.Text = txtHZ.Text = txtDuanZi.Text = txtMaiChong.Text = txtJiDianQi.Text = txtgongyinggongyang.Text = txtRevSoftNo.Text = txtSchemaDescription.Text = functionCheckSchemaCmb.Text = String.Empty;
                    ///
                    functionCheckSchemaCmb.DataSource = null;
                    cbkReadPower.Checked = false;
                    //cbkReadProctNO.Checked = false;
                    cbkWCCalibration.Checked = false;
                    cbkTimeCalibration.Checked = false;
                    cbkhw.Checked = false;
                    cbkgh.Checked = false;
                    cbkzj.Checked = false;
                    chkIniParam.Checked = false;
                    if (functionCheckSchemaCmb.Items.Count != 0)
                    {
                        functionCheckSchemaCmb.Items.Clear();
                    }
                }
                tabControl2.SelectedIndex = 0;
                btnSave.Focus(); 
            }
            catch
            {

            }
        }




        private void btnSave_Click(object sender, EventArgs e)
        {
            if (lbProductName.SelectedIndex == -1)
            {
                MessageBox.Show("请选择需要配置方案的成品料号！", "提示");
                return;
            }
            DataTable ModifySchemaDT = new DataTable();
            string sql = "exec CheckYesNoModifySchema @username='" + OrBitUserName.Trim() + "'";
            ModifySchemaDT = GetDataSetWithSQLString(sql).Tables[0];
            string Mess = ModifySchemaDT.Rows[0][0].ToString().Trim();
            if (Mess != "")
            {
                MessageBox.Show(Mess, "提示");
                return;
            }
            if (functionCheckSchemaCmb.Text == "")
            {
                MessageBox.Show("请填写方案名称", "提示");
                return;
            }
            //sql = "select MOName from CLTestSchema where MOName='" + txtModel.Text.Trim() + "' and SchemaType='20' and SchemaName='" + lbProductName.Text.Trim() + "' and SchemaNameNew='"+functionCheckSchemaCmb.Text.Trim()+"'";
            sql = "exec GetShemaDT @SchemaNameNew='" + functionCheckSchemaCmb.Text.Trim() + "',@SchemaType='20',@MOName='" + txtModel.Text.Trim() + "',@SchemaName='" + lbProductName.Text.Trim() + "',@SchemaArea='国网'";
            DataSet ds = GetDataSetWithSQLString(sql);
            if (ds.Tables[0].Rows.Count != 0)
            {
                SaveFile("1");//修改方案
                BindShemaName();
            }
            else
            {
                SaveFile("0");//新增方案
                BindShemaName();
            }
        }

        private void BindShemaName()
        {
            #region 方案名称绑定
            DataTable SchemaNameDT = new DataTable();
            string sql = "select SchemaNameNew,SchemaDescription from CLTestSchema where SchemaType='20' and MOName='" + txtModel.Text.Trim() + "' and SchemaName='" + lbProductName.Text.Trim() + "' and SchemaArea='国网' order by SchemaConfigureDate desc";
            SchemaNameDT = GetDataSetWithSQLString(sql).Tables[0];//
            if (functionCheckSchemaCmb.Items.Count != 0)
            {
                functionCheckSchemaCmb.Items.Clear();
            }
            if (SchemaNameDT.Rows.Count != 0)//是否存在方案
            {
                for (int i = 0; i < SchemaNameDT.Rows.Count; i++)
                {
                    functionCheckSchemaCmb.Items.Add(SchemaNameDT.Rows[i][0].ToString());
                }
                functionCheckSchemaCmb.SelectedIndex = 0;
                txtSchemaDescription.Text = SchemaNameDT.Rows[0][1].ToString().Trim();
            }
            else
            {
                functionCheckSchemaCmb.Text = "";
                txtSchemaDescription.Text = "";
                cbkReadPower.Checked = false;
                cbkWCCalibration.Checked = false;
                cbkTimeCalibration.Checked = false;
                cbkhw.Checked = false;
                cbkgh.Checked = false;
                cbkzj.Checked = false;
                chkIniParam.Checked = false;
                txtRevSoftNo.Text = String.Empty;
            }
            #endregion
        }







        private void chb_CheckedChanged(object sender, EventArgs e)
        {
            if (lab1.Text == "0")
            {
                lab1.Text = "1";
                txtfourBit.Text = GetFourByte();
                lab1.Text = "0";
            }
        }
        private string GetFourByte()
        {
            string strFourByte = "";
            #region 第32路
            if (chb32.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第31路
            if (chb31.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第30路
            if (chb30.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第29路
            if (chb29.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第28路
            if (chb28.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第27路
            if (chb27.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第26路
            if (chb26.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第25路
            if (chb25.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第24路
            if (chb24.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第23路
            if (chb23.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第22路
            if (chb22.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第21路
            if (chb21.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第20路
            if (chb20.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第19路
            if (chb19.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第18路
            if (chb18.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第17路
            if (chb17.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第16路
            if (chb16.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第15路
            if (chb15.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第14路
            if (chb14.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第13路
            if (chb13.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第12路
            if (chb12.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第11路
            if (chb11.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第10路
            if (chb10.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第9路
            if (chb9.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第8路
            if (chb8.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第7路
            if (chb7.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第6路
            if (chb6.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第5路
            if (chb5.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第4路
            if (chb4.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第3路
            if (chb3.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第2路
            if (chb2.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            #region 第1路
            if (chb1.Checked)
            {
                strFourByte += "1";
            }
            else
            {
                strFourByte += "0";
            }
            #endregion
            // MessageBox.Show(strFourByte);
            strFourByte = Convert.ToString(Convert.ToInt32(strFourByte, 2), 16).ToUpper().PadLeft(8, '0');
            return strFourByte;
        }

        private void ViewLushu(string strFourByte32)
        {
            #region 第32路
            if (strFourByte32[0] == '1')
            {
                chb32.Checked = true;
            }
            else
            {
                chb32.Checked = false;
            }
            #endregion
            #region 第31路
            if (strFourByte32[1] == '1')
            {
                chb31.Checked = true;
            }
            else
            {
                chb31.Checked = false;
            }
            #endregion
            #region 第30路
            if (strFourByte32[2] == '1')
            {
                chb30.Checked = true;
            }
            else
            {
                chb30.Checked = false;
            }
            #endregion
            #region 第29路
            if (strFourByte32[3] == '1')
            {
                chb29.Checked = true;
            }
            else
            {
                chb29.Checked = false;
            }
            #endregion
            #region 第28路
            if (strFourByte32[4] == '1')
            {
                chb28.Checked = true;
            }
            else
            {
                chb28.Checked = false;
            }
            #endregion
            #region 第27路
            if (strFourByte32[5] == '1')
            {
                chb27.Checked = true;
            }
            else
            {
                chb27.Checked = false;
            }
            #endregion
            #region 第26路
            if (strFourByte32[6] == '1')
            {
                chb26.Checked = true;
            }
            else
            {
                chb26.Checked = false;
            }
            #endregion
            #region 第25路
            if (strFourByte32[7] == '1')
            {
                chb25.Checked = true;
            }
            else
            {
                chb25.Checked = false;
            }
            #endregion
            #region 第24路
            if (strFourByte32[8] == '1')
            {
                chb24.Checked = true;
            }
            else
            {
                chb24.Checked = false;
            }
            #endregion
            #region 第23路
            if (strFourByte32[9] == '1')
            {
                chb23.Checked = true;
            }
            else
            {
                chb23.Checked = false;
            }
            #endregion
            #region 第22路
            if (strFourByte32[10] == '1')
            {
                chb22.Checked = true;
            }
            else
            {
                chb22.Checked = false;
            }
            #endregion
            #region 第21路
            if (strFourByte32[11] == '1')
            {
                chb21.Checked = true;
            }
            else
            {
                chb21.Checked = false;
            }
            #endregion
            #region 第20路
            if (strFourByte32[12] == '1')
            {
                chb20.Checked = true;
            }
            else
            {
                chb20.Checked = false;
            }
            #endregion
            #region 第19路
            if (strFourByte32[13] == '1')
            {
                chb19.Checked = true;
            }
            else
            {
                chb19.Checked = false;
            }
            #endregion
            #region 第18路
            if (strFourByte32[14] == '1')
            {
                chb18.Checked = true;
            }
            else
            {
                chb18.Checked = false;
            }
            #endregion
            #region 第17路
            if (strFourByte32[15] == '1')
            {
                chb17.Checked = true;
            }
            else
            {
                chb17.Checked = false;
            }
            #endregion
            #region 第16路
            if (strFourByte32[16] == '1')
            {
                chb16.Checked = true;
            }
            else
            {
                chb16.Checked = false;
            }
            #endregion
            #region 第15路
            if (strFourByte32[17] == '1')
            {
                chb15.Checked = true;
            }
            else
            {
                chb15.Checked = false;
            }
            #endregion
            #region 第14路
            if (strFourByte32[18] == '1')
            {
                chb14.Checked = true;
            }
            else
            {
                chb14.Checked = false;
            }
            #endregion
            #region 第13路
            if (strFourByte32[19] == '1')
            {
                chb13.Checked = true;
            }
            else
            {
                chb13.Checked = false;
            }
            #endregion
            #region 第12路
            if (strFourByte32[20] == '1')
            {
                chb12.Checked = true;
            }
            else
            {
                chb12.Checked = false;
            }
            #endregion
            #region 第11路
            if (strFourByte32[21] == '1')
            {
                chb11.Checked = true;
            }
            else
            {
                chb11.Checked = false;
            }
            #endregion
            #region 第10路
            if (strFourByte32[22] == '1')
            {
                chb10.Checked = true;
            }
            else
            {
                chb10.Checked = false;
            }
            #endregion
            #region 第9路
            if (strFourByte32[23] == '1')
            {
                chb9.Checked = true;
            }
            else
            {
                chb9.Checked = false;
            }
            #endregion
            #region 第8路
            if (strFourByte32[24] == '1')
            {
                chb8.Checked = true;
            }
            else
            {
                chb8.Checked = false;
            }
            #endregion
            #region 第7路
            if (strFourByte32[25] == '1')
            {
                chb7.Checked = true;
            }
            else
            {
                chb7.Checked = false;
            }
            #endregion
            #region 第6路
            if (strFourByte32[26] == '1')
            {
                chb6.Checked = true;
            }
            else
            {
                chb6.Checked = false;
            }
            #endregion
            #region 第5路
            if (strFourByte32[27] == '1')
            {
                chb5.Checked = true;
            }
            else
            {
                chb5.Checked = false;
            }
            #endregion
            #region 第4路
            if (strFourByte32[28] == '1')
            {
                chb4.Checked = true;
            }
            else
            {
                chb4.Checked = false;
            }
            #endregion
            #region 第3路
            if (strFourByte32[29] == '1')
            {
                chb3.Checked = true;
            }
            else
            {
                chb3.Checked = false;
            }
            #endregion
            #region 第2路
            if (strFourByte32[30] == '1')
            {
                chb2.Checked = true;
            }
            else
            {
                chb2.Checked = false;
            }
            #endregion
            #region 第1路
            if (strFourByte32[31] == '1')
            {
                chb1.Checked = true;
            }
            else
            {
                chb1.Checked = false;
            }
            #endregion
        }

        private void btnCoppy_Click(object sender, EventArgs e)
        {
            CoppyShema cop = new CoppyShema();
            cop.ShowDialog();
        }
    }
}
