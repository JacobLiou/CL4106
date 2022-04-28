using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Diagnostics;
using pwInterface;
using pwFunction;
using pwFunction.pwConst;
using VerificationEquipment.Commons;
//using Frontier.MeterVerification.DeviceCommon;
//using Frontier.MeterVerification.KLDevice;
using Frontier.MeterVerification.Communication;
namespace CL4100
{
    public class Adapter
    {
        #region ------------静态成员-------------
        private static MeterEquipment m_ComAdpater=null;
        private static ClAmMeterController.CMultiController m_485Control = null;
        private static ClAmMeterController.pwMeterProtocolInfo m_ProtocolInfo = null;

        private static int m_ChancelCount = 12;                             //表位数
        private static int m_reTryTime = 3;                                 //源重试次数
        private static bool[] m_bln_Selected = new bool[BwCount];           //选中操作
        private static stPrjParameter m_stPrjParameter;                     //项目参数结构
        private static string m_DelDirectory = Application.StartupPath      //写文件目录(数据帧)
                    + pwFunction.pwConst.Variable.CONST_COMMUNICATIONDATA
                    + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month .ToString() + "-" + DateTime.Now.Day.ToString();

        private static int m_DLHL = 0;                                      //电流回路

        
        private static System.Timers.Timer StdParmTimer = null;//定时读取标准表信息
        private static int m_StepSinglePhaseTest;//分相供电测试步数，处理数据事件事件用
        private static CheckPoint m_checkPoint;//检定点,误差检定用
        public static bool isConnected;
        private static string[] m_SinglePhaseTestData=new string[BwCount];

        

        /// <summary>
        /// 设备控制单元
        /// 静态设计，所有检定器共享
        /// </summary>   
        public static MeterEquipment g_ComAdpater
        {
            set { m_ComAdpater = value; }
            get { return m_ComAdpater; }
        }


        /// <summary>
        /// 485控制单元，
        /// </summary>
        public static ClAmMeterController.CMultiController g_485Control
        {
            set { m_485Control = value; }
            get { return m_485Control; }
        }

        /// <summary>
        /// 协议信息
        /// </summary>
        public static ClAmMeterController.pwMeterProtocolInfo g_ProtocolInfo
        {
            set { m_ProtocolInfo = value; }
            get { return m_ProtocolInfo; }
        }

        /// <summary>
        /// 挂表架表位数量
        /// </summary>
        public static int BwCount
        {
            get { return pwFunction.pwConst.GlobalUnit.GetConfig(pwFunction.pwConst.Variable.CTC_BWCOUNT, 12); }
        }

        /// <summary>
        /// 485通道数量
        /// </summary>
        public static int ChancelCount
        {
            get { return Adapter.m_ChancelCount; }
            set { Adapter.m_ChancelCount = value; }
        }

        /// <summary>
        /// 操作重试次数
        /// </summary>
        public static int RetryTime
        {
            get { return m_reTryTime; }
        }

        /// <summary>
        /// 是否要检
        /// </summary>
        public static bool[] blnSelected
        {
            get { return Adapter.m_bln_Selected; }
            set
            {
                Adapter.m_bln_Selected = value;
                m_485Control.blnSelected = value;
            }
        }

        /// <summary>
        /// 结束
        /// </summary>
        /// <returns></returns>
        public static void Stop()
        {
            //m_ComAdpater.Link(false);
            m_ComAdpater.IsStop = true;
            m_485Control.IsStop = true;

        }

        public static WiringMode Wiringmode
        {
            get
            {
                if (   GlobalUnit.g_Meter.MInfo.Clfs == enmClfs.三相三线有功 
                    || GlobalUnit.g_Meter.MInfo.Clfs == enmClfs.三相三线无功)
                {
                    return WiringMode.三相三线;
                }
                else if( GlobalUnit.g_Meter.MInfo.Clfs == enmClfs.二相三线)
                {
                    return WiringMode.二相三线;
                }
                return WiringMode.三相四线;
            }
            set
            {
            }
        }

        #endregion

        #region ------------事件-----------------
        public delegate void ElapsedEventHandler(object sender, System.Timers.ElapsedEventArgs e);

        public static event OnEventReadStdInfo OnReadStaInfo;//读标准表事件

        public static event DelegateEventResuleChange OnEventResuleChange;      //结果改变事件--->灯炮改变

        public static event DelegateEventItemChange OnEventItemChange;          //项目改变---->显示改变

        //public static event DelegateEventTxmChange OnEvenResuleTxtChange;
        #endregion

        #region ------------构造函数-------------
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bwCount">挂表架表位数量</param>
        /// <param name="ReTeyTime">操作失败重试次数</param>
        public Adapter()
        {

            //初始化设备控制单元
            m_ComAdpater = new MeterEquipment();

            #region 通迅485
            m_485Control = new ClAmMeterController.CMultiController(BwCount);
            m_485Control.OnEventMultiControlle += new DelegateEventMultiController(OnEventMultiControllerh);
            m_485Control.OnEventTxFrame += new Dge_EventTxFrame(OnEventCMultiControllerBwTxFrame);
            m_485Control.OnEventRxFrame += new Dge_EventRxFrame(OnEventCMultiControllerBwRxFrame);
            #endregion

            m_ProtocolInfo = new ClAmMeterController.pwMeterProtocolInfo();

            StdParmTimer = new System.Timers.Timer(1000);
            StdParmTimer.Enabled = false;
            StdParmTimer.Elapsed += new System.Timers.ElapsedEventHandler(InspectStdParmTimer_Elapsed);

        }

         ~Adapter()
        {
            //m_ComAdpater = null;
            //m_AdjustErrorAdpater = null;
            //m_AdjustClockAdpater = null;
            //m_ErrorAdpater = null;
            //m_ClockAdpater = null;
            //m_485Control = null;
            //m_ProtocolInfo = null;
        }
        #endregion

        #region ------------公共方法-------------

        #region 联机
        public static void Link()
        {
            m_ComAdpater.InitConnect(BwCount);
            isConnected = true;
        }

        #endregion
        #region 脱机
        public static void LinkOff()
        {
            m_ComAdpater.Close();
        }


        #endregion
        #region 升源
        public static void PowerOnOnlyU()
        {
            m_ComAdpater.ExecuteOnlyUpPower();
            //m_ComAdpater.ExecutFenXiangPower(GlobalUnit.g_Meter.MInfo.Ub, GlobalUnit.g_Meter.MInfo.Ub, GlobalUnit.g_Meter.MInfo.Ub, Wiringmode, 1f, true, Pulse.正向有功);
            GlobalUnit.g_PowerStatus = enmPowerStatus.只输出电压;
        }

        #endregion
        #region 关源
        public static void PowerOff()
        {
            m_ComAdpater.ExecutDownPower();
            GlobalUnit.g_PowerStatus = enmPowerStatus.空闲;
        }

        #endregion

        #endregion

        #region ------------方案作业------------

        /// <summary>
        /// 开始
        /// </summary>
        /// <returns></returns>
        private static void Start()
        {
            m_ComAdpater.IsStop = false;
            m_485Control.IsStop = false;

        }

        /// <summary>
        /// 等待返回
        /// </summary>
        private static void WaitAllReturn()
        {
            bool bolBreak = true;
            while (true)
            {

                bolBreak = true;
                for (int i = 0; i < GlobalUnit.g_BW; i++)
                {
                    if (m_485Control.blnSelected[i])
                    {
                        Application.DoEvents();
                        if (m_485Control.blnReturn[i] == false) //如果有一个没有返回
                        {
                            bolBreak = false;
                            break;
                        }
                    }
                }
                if (bolBreak) break;
                Application.DoEvents();
                Thread.Sleep(5);

                if (GlobalUnit.ForceVerifyStop || GlobalUnit.g_Status == enmStatus.停止)//外部中止＝＝退出
                {
                    GlobalUnit.g_MsgControl.OutMessage("检定停止，操作完毕", false);
                    break;
                }
                Application.DoEvents();
                Thread.Sleep(5);

            }
            return;
        }

        /// <summary>
        /// 是否可以继续作业，有要检表
        /// </summary>
        /// <returns></returns>
        private static bool bIsContinue()
        {
            #region
            bool bIsCheck = false;
            for (int i = 0; i < GlobalUnit.g_BW; i++)
            {
                if (GlobalUnit.g_Meter.MData[i].bolIsCheck)
                {
                    bIsCheck = true;
                    break;
                }
            }
            if (!bIsCheck) GlobalUnit.g_MsgControl.OutMessage("没有选中任何检定表", false);
            return bIsCheck;
            #endregion
        }

        #region 刷新项目参数
        /// <summary>
        /// 加载项目参数｜加载默认参数
        /// </summary>
        /// <returns></returns>
        private static void RefreshPrjParameter(enmMeterPrjID enm_PrjID, string str_PrjParameter)
        {

            m_stPrjParameter = new stPrjParameter();
            string _PrjParameter ;
            string[] _PrjParameterArray;
            string[] _CurPrjParameterArray;
            try
            {

                switch (enm_PrjID)
                {
                    case enmMeterPrjID.RS485读生产编号:
                        #region 读生产编号
                        _PrjParameter = "DLT645_1997|FFF9|12|0||";//协议|标识编码|长度|小数点|下发参数
                        _PrjParameterArray= _PrjParameter.Split('|');
                        _CurPrjParameterArray = str_PrjParameter.Split('|');
                        if (_CurPrjParameterArray.Length <= 0)//== 0 //_PrjParameterArray.Length
                        {
                            GlobalUnit.g_MsgControl.OutMessage("当前检定项目方案参数错误，正在加载默认参数", false);
                            #region 加载默认参数
                            m_stPrjParameter.str_XyName = "DLT645_1997";
                            m_stPrjParameter.str_Code = "FFF9";
                            m_stPrjParameter.int_Len = 12;
                            m_stPrjParameter.int_Dot = 0;
                            m_stPrjParameter.str_SendParameter = "";
                            #endregion
                        }
                        else
                        {
                            #region 加载方案参数
                            m_stPrjParameter.str_XyName = _CurPrjParameterArray[0];
                            m_stPrjParameter.str_Code = _CurPrjParameterArray[1];
                            m_stPrjParameter.int_Len = Convert.ToInt32(_CurPrjParameterArray[2]);
                            m_stPrjParameter.int_Dot = 0;// Convert.ToInt32(_CurPrjParameterArray[3]);
                            m_stPrjParameter.str_SendParameter = "";// _CurPrjParameterArray[4];
                            #endregion

                        }
                        break;
                        #endregion

                    case enmMeterPrjID.误差检定:
                        #region 误差检定
                        _PrjParameter = "1";//高频倍数
                        _PrjParameterArray = _PrjParameter.Split('|');
                        _CurPrjParameterArray = GlobalUnit.g_Plan.cWcjd.PrjParameter.Split('|');
                        if (_CurPrjParameterArray.Length <= 0)
                        {
                            #region 加载默认参数
                            m_stPrjParameter.int_HighFrequencyPulseBS = 1;
                            #endregion
                        }
                        else
                        {
                            #region 加载方案参数
                            m_stPrjParameter.int_HighFrequencyPulseBS = Convert.ToInt32(_CurPrjParameterArray[0]);
                            #endregion

                        }

                        break;
                        #endregion

                    case enmMeterPrjID.日计时误差检定:
                        #region 多功能检定
                        _PrjParameter = "5000000|1|10";
                        _PrjParameterArray = _PrjParameter.Split('|');
                        _CurPrjParameterArray = GlobalUnit.g_Plan.cDgnSy.PrjParameter.Split('|');
                        if (_CurPrjParameterArray.Length <= 0)
                        {
                            m_stPrjParameter.lng_TimePulseConst = 0x4c4b40L;
                            m_stPrjParameter.int_TimePulsePL = 1;
                            m_stPrjParameter.int_TimePulseQs = 1;
                        }
                        else
                        {
                            m_stPrjParameter.lng_TimePulseConst = Convert.ToInt64(_CurPrjParameterArray[0]);
                            m_stPrjParameter.int_TimePulsePL = Convert.ToInt32(_CurPrjParameterArray[1]);
                            m_stPrjParameter.int_TimePulseQs = Convert.ToInt32(_CurPrjParameterArray[2]);
                        }
                        break;
                        #endregion

                    case enmMeterPrjID.分相供电测试:
                        #region 分相供电测试
                        _PrjParameter = "5|5|5";
                        _PrjParameterArray = _PrjParameter.Split('|');
                        _CurPrjParameterArray = str_PrjParameter.Split('|');
                        if (_CurPrjParameterArray.Length <= 0)
                        {
                            GlobalUnit.g_MsgControl.OutMessage("当前检定项目方案参数错误，正在加载默认参数", false);
                            m_stPrjParameter.flt_UMaxSinglePhaseTest = 5f;
                        }
                        else
                        {
                            m_stPrjParameter.flt_UMaxSinglePhaseTest = Convert.ToSingle(_CurPrjParameterArray[0]);
                        }
                        break;
                        #endregion

                    case enmMeterPrjID.交流采样测试:
                        #region 交流采样测试
                        _PrjParameter = "0.4|0.4|0.4";
                        _PrjParameterArray = _PrjParameter.Split('|');
                        _CurPrjParameterArray = str_PrjParameter.Split('|');
                        if (_PrjParameterArray.Length <= 0)
                        {
                            GlobalUnit.g_MsgControl.OutMessage("当前检定项目方案参数错误，正在加载默认参数", false);
                            m_stPrjParameter.flt_WcACSamplingTest[0] = 0.4f;
                            m_stPrjParameter.flt_WcACSamplingTest[1] = 0.4f;
                            m_stPrjParameter.flt_WcACSamplingTest[2] = 0.4f;
                        }
                        else
                        {
                            m_stPrjParameter.flt_WcACSamplingTest[0] = Convert.ToSingle(_CurPrjParameterArray[0]);
                            m_stPrjParameter.flt_WcACSamplingTest[1] = Convert.ToSingle(_CurPrjParameterArray[1]);
                            m_stPrjParameter.flt_WcACSamplingTest[2] = Convert.ToSingle(_CurPrjParameterArray[2]);
                        }
                        return;
                        #endregion

                    case enmMeterPrjID.读电能表底度:
                        #region 读电能表底度
                        _PrjParameter = "DLT645_2007|00010000|4|2||50|0.1f";//协议|标识编码|长度|小数点|下发参数
                        _PrjParameterArray = _PrjParameter.Split('|');
                        _CurPrjParameterArray = str_PrjParameter.Split('|');
                        if (_CurPrjParameterArray.Length <= 0)//== 0 
                        {
                            GlobalUnit.g_MsgControl.OutMessage("当前检定项目方案参数错误，正在加载默认参数", false);
                            #region 加载默认参数
                            m_stPrjParameter.str_XyName = "DLT645_2007";
                            m_stPrjParameter.str_Code = "00010000";
                            m_stPrjParameter.int_Len = 4;
                            m_stPrjParameter.int_Dot = 2;
                            m_stPrjParameter.str_SendParameter = "";
                            m_stPrjParameter.flt_EnergyMax = 50;
                            m_stPrjParameter.flt_EnergyMin = 0.1f;
                            #endregion
                        }
                        else
                        {
                            #region 加载方案参数
                            m_stPrjParameter.str_XyName = _CurPrjParameterArray[0];
                            m_stPrjParameter.str_Code = _CurPrjParameterArray[1];
                            m_stPrjParameter.int_Len = Convert.ToInt32(_CurPrjParameterArray[2]);
                            m_stPrjParameter.int_Dot = Convert.ToInt32(_CurPrjParameterArray[3]);
                            m_stPrjParameter.str_SendParameter = _CurPrjParameterArray[4];
                            m_stPrjParameter.flt_EnergyMax = Convert.ToSingle(_CurPrjParameterArray[5]);
                            m_stPrjParameter.flt_EnergyMin =Convert.ToSingle(_CurPrjParameterArray[6]);
                            #endregion

                        }
                        break;
                        #endregion

                    case enmMeterPrjID.打包参数下载:
                        #region 打包参数下载
                        _PrjParameter = "DLT645_2007|111111111111|51";///协议|标识编码|方案地址|总数
                        _PrjParameterArray = _PrjParameter.Split('|');
                        _CurPrjParameterArray = str_PrjParameter.Split('|');
                        if (_CurPrjParameterArray.Length <= 0)
                        {
                            #region 加载默认参数
                            m_stPrjParameter.str_XyName = "DLT645_2007";
                            m_stPrjParameter.str_Address = "111111111111";
                            m_stPrjParameter.int_Count = 1;
                            #endregion
                        }
                        else
                        {
                            #region 加载方案参数
                            m_stPrjParameter.str_XyName = _CurPrjParameterArray[0];
                            m_stPrjParameter.str_Address = _CurPrjParameterArray[1];
                            m_stPrjParameter.int_Count = Convert.ToInt32(_CurPrjParameterArray[2]); ;
                            #endregion

                        }
                        break;
                        #endregion

                    case enmMeterPrjID.系统清零:
                        #region 系统清零
                        _PrjParameter = "DLT645_2007|04CC1000|1|0|55|";//协议|标识编码|长度|参数
                        _PrjParameterArray = _PrjParameter.Split('|');
                        _CurPrjParameterArray = str_PrjParameter.Split('|');
                        if (_CurPrjParameterArray.Length <= 0)
                        {
                            #region 加载默认参数
                            m_stPrjParameter.str_XyName = "DLT645_2007";
                            m_stPrjParameter.str_Code = "04CC1000";
                            m_stPrjParameter.int_Len = 1;
                            m_stPrjParameter.int_Dot = 0;
                            m_stPrjParameter.str_SendParameter = "55";
                            #endregion
                        }
                        else
                        {
                            #region 加载方案参数
                            m_stPrjParameter.str_XyName = _CurPrjParameterArray[0];
                            m_stPrjParameter.str_Code = _CurPrjParameterArray[1];
                            m_stPrjParameter.int_Len = Convert.ToInt32(_CurPrjParameterArray[2]);
                            m_stPrjParameter.int_Dot = Convert.ToInt32(_CurPrjParameterArray[3]);
                            m_stPrjParameter.str_SendParameter = _CurPrjParameterArray[4];
                            #endregion

                        }
                        break;
                        #endregion

                }
            }
            catch
            {
                GlobalUnit.g_Status = enmStatus.停止;
                return;
            }
        }


        #endregion

        #region 刷新协议
        /// <summary>
        ///  刷新协议
        /// </summary>
        /// <param name="strXyName">协议名称</param>
        /// <returns></returns>
        private static bool RefreshProtocol(string strXyName)
        {
            if (GlobalUnit.g_PowerStatus != enmPowerStatus.只输出电压) Adapter.PowerOnOnlyU();
            m_ProtocolInfo = new ClAmMeterController.pwMeterProtocolInfo(strXyName, m_485Control.str485Setting);
            m_485Control.RefreshProtocol(m_ProtocolInfo);
            return true;
        }
        private static bool RefreshProtocol(string strXyName, enmPowerStatus enmPStatus)
        {
            if (!(GlobalUnit.g_PowerStatus == enmPStatus || GlobalUnit.g_PowerStatus == enmPowerStatus.只输出电压)) Adapter.PowerOnOnlyU();
            m_ProtocolInfo = new ClAmMeterController.pwMeterProtocolInfo(strXyName, m_485Control.str485Setting);
            m_485Control.RefreshProtocol(m_ProtocolInfo);
            return true;
        }

        #endregion

        #region 事件处理
        /// <summary>
        /// 数据发送事件
        /// </summary>
        /// <param name="str_Frame"></param>
        private static void OnEventCMultiControllerBwTxFrame(int intCom, string str_Frame)
        {
            //写文件处理
            object  obj = intCom.ToString() + "|" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ":"
                        + DateTime.Now.Millisecond.ToString("D3") + "->" + str_Frame;
            //pwClassLibrary.pwProtocol.ThreadPool.QueueUserWorkItem(new WaitCallback(SaveCommunicationData), obj);
            ThreadPool.QueueUserWorkItem(new WaitCallback(SaveCommunicationData), obj);
        }
        /// <summary>
        /// 数据接收事件
        /// </summary>
        /// <param name="str_Frame"></param>
        private static void OnEventCMultiControllerBwRxFrame(int intCom, string str_Frame)
        {
            //写文件处理
            object obj = intCom.ToString() + "|" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ":"
                        + DateTime.Now.Millisecond.ToString("D3") + "<-" + str_Frame;
            //pwClassLibrary.pwProtocol.ThreadPool.QueueUserWorkItem(new WaitCallback(SaveCommunicationData), obj);
            ThreadPool.QueueUserWorkItem(new WaitCallback(SaveCommunicationData), obj);

        }

        private static object Locked = new object();
        /// <summary>
        /// 保存数据帧到文件
        /// </summary>
        /// <param name="obj"></param>
        private static void SaveCommunicationData(object obj)
        {
            try
            {
                lock (Locked)
                {
                    string [] objArray=obj.ToString().Split('|');
                    int intCom = Convert.ToInt32(objArray[0].ToString());
                    string str_Frame = objArray[1].ToString();

                    #region 写入文件
                    string sfilename =m_DelDirectory + "\\" + intCom.ToString("D2")+"_CommunicationData.txt";
                    StreamWriter sw = new StreamWriter(@sfilename, true, System.Text.Encoding.Unicode);
                    sw.WriteLine(str_Frame);
                    sw.Close();
                    #endregion
                }
            }
            catch (Exception e)
            {
                return;
            }
        }

        #endregion

        #region 结果事件处理
        private static object LockedA = new object();

        /// <summary>
        /// 结果处理
        /// </summary>
        /// <param name="enm_PrjID"></param>
        /// <param name="int_BwNo"></param>
        /// <param name="bln_Result"></param>
        /// <param name="str_Value"></param>
        private static void OnEventMultiControllerh(enmMeterPrjID enm_PrjID, int int_BwNo, bool bln_Result, string str_Value)
        {
            lock (LockedA)
            {

                #region 判断前工序是否OK，生产编码是否在范围
                if (enm_PrjID == enmMeterPrjID.RS485读生产编号)
                {
                    
                    GlobalUnit.g_Meter.MData[int_BwNo].bolSaveData = true;

                    if (str_Value == "99") //  扫描进条码
                    {
                        str_Value = GlobalUnit.g_Meter.MData[int_BwNo].chrTxm;
                    }

                    if (bln_Result)
                    {
                        str_Value = str_Value.Replace("'", "0");//如果有单引号则替换成
                       
                        pw_Main codefileform = new pw_Main();
                        string Errorinfo = "";
                        bool bolCheckScbh = codefileform.Sql_CheckLotSpecificationDoMethod(str_Value, ref Errorinfo);

                        if (!bolCheckScbh)
                        {
                            GlobalUnit.g_Meter.MData[int_BwNo].bolSaveData = false;
                            bln_Result = false;
                            str_Value = Errorinfo;
                        }
                    }
                    else
                    {
                        GlobalUnit.g_Meter.MData[int_BwNo].bolSaveData = false ;
                    }
                }
                #endregion

                #region 处理总结论
                if (!bln_Result)
                {
                    GlobalUnit.g_Meter.MData[int_BwNo].bolResult = false;
                    GlobalUnit.g_Meter.MData[int_BwNo].chrResult = Variable.CTG_BuHeGe;
                    GlobalUnit.g_Meter.MData[int_BwNo].chrRexplain = str_Value;
                }
                else
                {
                    GlobalUnit.g_Meter.MData[int_BwNo].bolResult = true;
                    GlobalUnit.g_Meter.MData[int_BwNo].chrResult = Variable.CTG_HeGe;

                    #region 再把重要数据写COPY到主干
                    switch (enm_PrjID)
                    {
                        case enmMeterPrjID.RS485读生产编号:
                            GlobalUnit.g_Meter.MData[int_BwNo].chrScbh = str_Value;
                            break;
                        case enmMeterPrjID.误差检定:
                            break;
                        case enmMeterPrjID.日计时误差检定 :
                            break;
                        case enmMeterPrjID.分相供电测试:
                            #region
                            if (m_StepSinglePhaseTest == 0)
                            {
                                m_SinglePhaseTestData[int_BwNo] = "A相：" + GlobalUnit.g_Meter.MInfo.Ub * 0.7;// str_Value;//伪造
                            }
                            else
                            {
                                string[] strArray;
                                IntPtr ptr;
                                if (m_StepSinglePhaseTest == 1)
                                {
                                    (strArray = m_SinglePhaseTestData)[(int)(ptr = (IntPtr)int_BwNo)] = strArray[(int)ptr] + " B相：" + GlobalUnit.g_Meter.MInfo.Ub * 0.7;// str_Value;//伪造
                                }
                                else if (m_StepSinglePhaseTest == 2)
                                {
                                    (strArray = m_SinglePhaseTestData)[(int)(ptr = (IntPtr)int_BwNo)] = strArray[(int)ptr] + " C相：" + GlobalUnit.g_Meter.MInfo.Ub * 0.7;// str_Value;//伪造
                                }
                            }
                            bln_Result = CheckedSinglePhaseTest(str_Value, m_stPrjParameter.flt_UMaxSinglePhaseTest, m_StepSinglePhaseTest);
                            if (m_StepSinglePhaseTest >= 2)
                            {
                                str_Value = m_SinglePhaseTestData[int_BwNo];
                            }
                            #endregion
                            break;
                        case enmMeterPrjID.交流采样测试:
                            bln_Result = CheckedACSamplingTest(str_Value, m_stPrjParameter.flt_WcACSamplingTest);

                            break;
                        case enmMeterPrjID.读电能表底度:
                            #region
                            float flt_EnergyJDdd = 0f;
                            GlobalUnit.g_Meter.MData[int_BwNo].sngEnergy = 0f;
                            try
                            {
                                GlobalUnit.g_Meter.MData[int_BwNo].sngEnergy = Convert.ToSingle(str_Value);
                                flt_EnergyJDdd = Convert.ToSingle(str_Value);

                                if (flt_EnergyJDdd < m_stPrjParameter.flt_EnergyMin || flt_EnergyJDdd > m_stPrjParameter.flt_EnergyMax)
                                {
                                    bln_Result = false;
                                    //str_Value = "老化走字电量不对";
                                }
                            }
                            catch
                            {

                            }
                            #endregion
                            break;
                        case enmMeterPrjID.打包参数下载:
                            break;
                        case enmMeterPrjID.系统清零:
                            break;
                    }
                    #endregion
                }
                #endregion


                #region 处理数据
                DataResultBasic dataResule;
                dataResule = new DataResultBasic();
                dataResule.Me_PrjID = Convert.ToString((int)enm_PrjID);
                dataResule.Me_Bw = int_BwNo;
                dataResule.Me_PrjName = enm_PrjID.ToString() + "结论";
                dataResule.Me_Result = bln_Result ? Variable.CTG_HeGe : Variable.CTG_BuHeGe;
                dataResule.Me_Value = str_Value;
                if (GlobalUnit.g_Meter.MData[int_BwNo].MeterResults.ContainsKey(((int)enm_PrjID).ToString()))
                {
                    GlobalUnit.g_Meter.MData[int_BwNo].MeterResults.Remove(((int)enm_PrjID).ToString());
                }
                GlobalUnit.g_Meter.MData[int_BwNo].MeterResults.Add(Convert.ToString((int)enm_PrjID), dataResule);
                //===
                GlobalUnit.g_Meter.MData[int_BwNo].bolAlreadyTest = true;
                #endregion


                //处理显示：改变显示指标灯泡
                if (OnEventResuleChange != null) OnEventResuleChange(int_BwNo, bln_Result);
            }
        }

        private static bool CheckedACSamplingTest(string _str_Value, float[] _WcACSamplingTest)
        {
            try
            {
                int num;
                bool flag = false;
                string[] strArray = _str_Value.Split(new char[] { '|' });
                float[] numArray = new float[9];
                float[] numArray2 = new float[9];
                float[] numArray3 = new float[9];
                numArray2[0] = GlobalUnit.g_StdMeter.Ua;
                numArray2[1] = GlobalUnit.g_StdMeter.Ub;
                numArray2[2] = GlobalUnit.g_StdMeter.Uc;
                numArray2[3] = GlobalUnit.g_StdMeter.Ia;
                numArray2[4] = GlobalUnit.g_StdMeter.Ib;
                numArray2[5] = GlobalUnit.g_StdMeter.Ic;
                numArray2[6] = Math.Abs(GlobalUnit.g_StdMeter.Pa) / 1000f;
                numArray2[7] = Math.Abs(GlobalUnit.g_StdMeter.Pb) / 1000f;
                numArray2[8] = Math.Abs(GlobalUnit.g_StdMeter.Pc) / 1000f;
                for (num = 0; num < 9; num++)
                {
                    numArray[num] = Math.Abs(Convert.ToSingle(strArray[num]));
                    numArray3[num] = _WcACSamplingTest[num / 3];
                }
                for (num = 0; num < numArray.Length; num++)
                {
                    if (Wiringmode == WiringMode.三相三线)
                    {
                        if (num == 1 || num == 4 || num == 7) continue;
                    }
                    flag = checkWc(numArray[num], numArray2[num], numArray3[num]);
                    if (!flag)
                    {
                        break;
                    }
                }
                return flag;
            }
            catch
            {
                return false;
            }
        }

        private static bool checkWc(float _curValue, float _baseValue, float _baseWcx)
        {
            return (Math.Abs((float)(((_curValue - _baseValue) / _baseValue) * 100f)) < _baseWcx);
        }



        private static bool CheckedSinglePhaseTest(string _str_Value, float flt_UMaxSinglePhaseTest, int int_Step)
        {
            ////string[] strArray = _str_Value.Split(new char[] { '|' });//这里不能取出生产编号，而是三相电压
            ////string[] strArray = new string[]{}(GlobalUnit.g_Meter.MInfo.Ub * 0.7).ToString().Split(new char[] { '|' });
            //float[] numArray = new float[3];
            //for (int i = 0; i < 3; i++)
            //{
            //    //numArray[i] = Math.Abs(Convert.ToSingle(strArray[i]));
            //    numArray[i] = 0.0f;//伪造
            //}
            //switch (int_Step)
            //{
            //    case 0:
            //        return ((numArray[1] < flt_UMaxSinglePhaseTest) && (numArray[2] < flt_UMaxSinglePhaseTest));

            //    case 1:
            //        return ((numArray[0] < flt_UMaxSinglePhaseTest) && (numArray[2] < flt_UMaxSinglePhaseTest));

            //    case 2:
            //        return ((numArray[0] < flt_UMaxSinglePhaseTest) && (numArray[1] < flt_UMaxSinglePhaseTest));
            //}
            return true;//伪造  假定永远没有误差 三相电压数据永远是对的 因为没读出来 只是通信
        }


        #endregion


        #region 检定方案
        public static void Verify()
        {
            Adapter.Start();
            StdParmTimer.Enabled = true;
            InstallMeter();

            PowerOnOnlyU();
            System.Threading.Thread.Sleep(8000);
            if (GlobalUnit.g_Plan.cReadScbh.IsCheck)
            {
                if (GlobalUnit.ForceVerifyStop || GlobalUnit.g_Status == enmStatus.停止) return;
                if (!ReadScbh()) return;
            }


            if (GlobalUnit.g_Plan.cWcjd.IsCheck)
            {
                if (GlobalUnit.ForceVerifyStop || GlobalUnit.g_Status == enmStatus.停止) return;
                if (!Wcjd()) return;
            }

            if (GlobalUnit.g_Plan.cDgnSy.IsCheck)
            {
                if (GlobalUnit.ForceVerifyStop || GlobalUnit.g_Status == enmStatus.停止) return;
                if (!Dgn_Clock()) return;
            }

            if (GlobalUnit.g_Plan.cSinglePhaseTest.IsCheck)
            {
                if (GlobalUnit.ForceVerifyStop || GlobalUnit.g_Status == enmStatus.停止) return;
                if (!SinglePhaseTest()) return;
            }

            if (GlobalUnit.g_Plan.cACSamplingTest.IsCheck)
            {
                if (GlobalUnit.ForceVerifyStop || GlobalUnit.g_Status == enmStatus.停止) return;
                if (!ACSamplingTest()) return;
            }

            if (GlobalUnit.g_Plan.cReadEnergy.IsCheck || GlobalUnit.g_Plan.cDownPara.IsCheck || GlobalUnit.g_Plan.cSysClear.IsCheck)
                Adapter.PowerOnOnlyU();//如果后面还需做功能，就只输出电压

            if (GlobalUnit.g_Plan.cReadEnergy.IsCheck)
            {
                if (GlobalUnit.ForceVerifyStop || GlobalUnit.g_Status == enmStatus.停止) return;
                if (!ReadEnergy()) return;
            }

            if (GlobalUnit.g_Plan.cDownPara.IsCheck)
            {
                if (GlobalUnit.ForceVerifyStop || GlobalUnit.g_Status == enmStatus.停止) return;
                if (!DownPara()) return;
            }
            if (GlobalUnit.g_Plan.cSysClear.IsCheck)
            {
                if (GlobalUnit.ForceVerifyStop || GlobalUnit.g_Status == enmStatus.停止) return;
                if (!SysClear()) return;
            }
        }
        private static void InstallMeter()
        {
            MeterPosition[] meterPositions = new MeterPosition[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                MeterPosition position = new MeterPosition();
                position.IsVerify = GlobalUnit.g_Meter.MData[i].bolIsCheck;
                position.MeterIndex = i + 1;
                position.Meter = new Meter();
                position.Meter.Address = "";
                position.Meter.AssetNo = "";
                position.Meter.BaudRate = "";
                position.Meter.CarrieWaveType = CarrieWaveType.东软;
                position.Meter.ConnectFromIT = false;
                position.Meter.Const = GlobalUnit.g_Meter.MInfo.Constant;
                position.Meter.Current = GlobalUnit.g_Meter.MInfo.Ib.ToString();
                position.Meter.Level = GlobalUnit.g_Meter.MInfo.DJ.ToString();
                position.Meter.Protocal = new Protocal();
                position.Meter.Rp_Const = GlobalUnit.g_Meter.MInfo.Constant_wg;
                position.Meter.Rp_Level = GlobalUnit.g_Meter.MInfo.DJ_wg.ToString();
                position.Meter.SaveTime = "0100";
                position.Meter.Voltage = GlobalUnit.g_Meter.MInfo.Ub.ToString();
                position.Meter.WiringMode = Wiringmode;
                meterPositions[i] = position;
            }
            m_ComAdpater.InstallMeter(meterPositions);
            m_ComAdpater.Connect();
        }


        #endregion

        #region 读生产编号
        private static bool ReadScbh()
        {

            if (!bIsContinue()) return false;

            if (OnEventItemChange != null) OnEventItemChange(enmMeterPrjID.RS485读生产编号.ToString());

            RefreshPrjParameter(enmMeterPrjID.RS485读生产编号, GlobalUnit.g_Plan.cReadScbh.PrjParameter);//传入表号

            string StmpXyName = m_stPrjParameter.str_XyName; //"DLT645_1997";
            string StmpXyCode = m_stPrjParameter.str_Code; //"FFF9";


            if (!RefreshProtocol(StmpXyName)) return false;

            #region 判断是否扫描输入
            if (pwFunction.pwConst.GlobalUnit.WcReadTableNo)
            {
                GlobalUnit.g_Meter.MData[0]._readdatecheck = "1";
            }
            else {
                GlobalUnit.g_Meter.MData[0]._readdatecheck = "2";
            }
            #endregion

            m_485Control.ReadScbh(StmpXyCode, GlobalUnit.g_Meter.MData[0]._readdatecheck);//写入表号
            //m_485Control.ReadScbh(StmpXyCode);//写入表号

            WaitAllReturn();//等待



            return true;
        }

        #endregion


        /// <summary>
        ///传递要检定数据
        /// </summary>
        private static void selectMeter()
        {
            bool [] boolSelect=new bool[GlobalUnit.g_Meter.MData.Length];
            for (int i = 0; i < GlobalUnit.g_Meter.MData.Length; i++)
            {
                boolSelect[i] = GlobalUnit.g_Meter.MData[i].bolIsCheck;
            }
                m_ComAdpater.MeterSelcetForUp(boolSelect);
        }
        #region 误差检定
        private static bool Wcjd()
        {
            try
            {
                //selectMeter();
                if (!bIsContinue())
                {
                    return false;
                }
                if (OnEventItemChange != null)
                {
                    OnEventItemChange(enmMeterPrjID.误差检定.ToString());
                }
                RefreshPrjParameter(enmMeterPrjID.误差检定, GlobalUnit.g_Plan.cWcjd.PrjParameter);
                int count = GlobalUnit.g_Plan.cWcPoint._WcPoint.Count;
                bool flag = true;
                bool flag2 = false;
                foreach (StWcPoint point in GlobalUnit.g_Plan.cWcPoint._WcPoint)
                {
                    //if (Convert.ToInt32(point.PrjID) < 10) continue;
                    if (!bIsContinue())
                    {
                        GlobalUnit.g_Status = enmStatus.停止;
                        return false;
                    }
                    if (GlobalUnit.ForceVerifyStop || (GlobalUnit.g_Status == enmStatus.停止))
                    {
                        return false;
                    }
                    InstallMeter();
                    m_checkPoint = point.SetCheckPoint();
                    if (OnEventItemChange != null)
                    {
                        OnEventItemChange(m_checkPoint.PrjName);
                    }
                    float ub = GlobalUnit.g_Meter.MInfo.Ub;
                    float ib = GlobalUnit.g_Meter.MInfo.Ib;
                    int constant = GlobalUnit.g_Meter.MInfo.Constant;
                    if ((m_stPrjParameter.int_HighFrequencyPulseBS > 1) && (m_checkPoint.xIb < 1f))
                    {
                        constant = GlobalUnit.g_Meter.MInfo.Constant *   m_stPrjParameter.int_HighFrequencyPulseBS;
                        if (!flag2)
                        {
                            m_ComAdpater.ExecutDownPower();
                            DelayTime(1000L);
                            PowerOnOnlyU();
                            DelayTime(0x1388L);//5000
                            if (!HighFrequencyPulse(m_stPrjParameter.int_HighFrequencyPulseBS))
                           
                            {
                                GlobalUnit.g_MsgControl.OutMessage("设置高频倍数失败", false);
                                GlobalUnit.g_Status = enmStatus.停止;
                                return false;
                            }
                            GlobalUnit.g_MsgControl.OutMessage("设置高频倍数成功,开始高频检表......", false);
                            flag2 = true;
                            //DelayTime(1000L);
                        }
                    }
                    else if (flag2)
                    {
                        flag2 = false;
                        m_ComAdpater.ExecutDownPower();
                        DelayTime(500L);
                    }
                    float i = m_checkPoint.xIb * ib;
                    string gLYS = m_checkPoint.GLYS;
                    float factor = 1f;//功率因素
                    bool capacitive = true;//容性=true,感性=false
                    if (gLYS.Substring(gLYS.Length - 1, 1) == "L")
                    {//lsx capacitive = false ;
                        capacitive = false ;
                        factor = Convert.ToSingle(gLYS.Substring(0, gLYS.Length - 1));
                    }
                    else if (gLYS.Substring(gLYS.Length - 1, 1) == "C")
                    {//lsx capacitive = true;
                        capacitive = true  ;
                        factor = Convert.ToSingle(gLYS.Substring(0, gLYS.Length - 1));
                    }
                    else
                    {
                        factor = Convert.ToSingle(gLYS);
                    }
                    Pulse gLFX = (Pulse)m_checkPoint.GLFX;
                    enmElement yJ = m_checkPoint.YJ;
                    if (yJ == enmElement.H)
                    {
                        flag = m_ComAdpater.ExecuteBasicError(ub, i, GlobalUnit.g_Meter.MInfo.PL, factor, capacitive, gLFX, constant, m_checkPoint.QS, GlobalUnit.WCTimesPerPoint, m_checkPoint.MaxError, m_checkPoint.MinError, dgtResultErr);
                    }
                    else
                    {
                        string phase = string.Empty;
                        if (yJ == enmElement.A)
                        {
                            phase = "A";
                        }
                        else if (yJ == enmElement.B)
                        {
                            phase = "B";
                        }
                        else if (yJ == enmElement.C)
                        {
                            phase = "C";
                        }
                        else
                        {
                            break;
                        }
                        flag = m_ComAdpater.ExecuteBasicError(ub, i, GlobalUnit.g_Meter.MInfo.PL, phase, factor, capacitive, gLFX, constant, m_checkPoint.QS, GlobalUnit.WCTimesPerPoint, m_checkPoint.MaxError, m_checkPoint.MinError, dgtResultErr);
                    }
                    if (!flag)
                    {
                        break;
                    }
                }
                return flag;
            }
            catch
            {
                return false;
            }
        }

        static VerificationEquipment.Commons.ReturnSampleDataErrDelegate dgtResultErr = new ReturnSampleDataErrDelegate(Up_DataErr);
        private static void Up_DataErr(string[,] val, int[] result)
        {
            try
            {
                int num = val.Length / result.Length;
                for (int i = 0; i < result.Length; i++)
                {
                    if (GlobalUnit.g_Meter.MData[i].bolIsCheck)
                    {
                        MeterErrorItem item = new MeterErrorItem();
                        item.Me_Bw = i + 1;
                        item.Item_PrjID = m_checkPoint.PrjID;
                        item.Item_PrjName = m_checkPoint.PrjName;
                        float num3 = 0f;
                        for (int j = 0; j < num; j++)
                        {
                            if ((val[i, j] == "") || (val[i, j] == null))
                            {
                                item.Wcn[j] = 0f;
                            }
                            else
                            {
                                item.Wcn[j] = Convert.ToSingle(val[i, j]);
                            }
                            num3 += item.Wcn[j];
                        }
                        item.WcPJ = num3 / ((float)num);
                        float errorInt = 0f;
                        string _IntJG = "0.02";//化整间距 = 等级 * 0.1
                        _IntJG = Convert.ToString(GlobalUnit.g_Meter.MInfo.DJ * 0.1f  );
                        //_IntJG = GlobalUnit.g_Meter.MInfo.DJ_wg.ToString();//需要把有无功等级传递过来
                        item.WcHz = float.Parse(MeterErrorItem.ErrorInt(item.WcPJ, "0.2", ref errorInt));
                        item.Item_Result = (result[i] == 1) ? "合格" : "不合格";
                        if (!GlobalUnit.g_Meter.MData[i].MeterErrors.ContainsKey(Convert.ToInt32(item.Item_PrjID).ToString()))
                        {
                            GlobalUnit.g_Meter.MData[i].MeterErrors.Add(Convert.ToInt32(item.Item_PrjID).ToString(), item);
                        }
                        else
                        {
                            foreach (MeterErrorItem item2 in GlobalUnit.g_Meter.MData[i].MeterErrors.Values)
                            {
                                if (item2.Item_PrjID == m_checkPoint.PrjID)
                                {
                                    item2.Item_Result = item.Item_Result;
                                    item2.Wcn = item.Wcn;
                                    item2.WcPJ = item.WcPJ;
                                    item2.WcHz = item.WcHz;
                                }
                            }
                        }
                        bool flag = true;
                        flag = isHeGeOtherWcPoint(GlobalUnit.g_Meter.MData[i].MeterErrors);
                        OnEventMultiControllerh(enmMeterPrjID.误差检定, i, flag, flag ? "" : (m_checkPoint.PrjName + "：" + item.ToString()));
                    }
                }
            }
            catch (SystemException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        private static bool isHeGeOtherWcPoint(Dictionary<string, MeterErrorItem> curMeter)
        {
            foreach (MeterErrorItem item in curMeter.Values)
            {
                if (item.Item_Result != "合格")
                {
                    return false;
                }
            }
            return true;
        }

        private static bool HighFrequencyPulse(int int_HighFrequencyPulseBS)
        {

            GlobalUnit.g_MsgControl.OutMessage("正在执行项目：设置高频检定参数......请稍候", false);

            //RefreshPrjParameter(enmMeterPrjID.高频检定, "DLT645_2007|04CC0509|1|0");

            string StmpXyName =  "DLT645_2007";//m_stPrjParameter.str_XyName;//

            if (!RefreshProtocol(StmpXyName,enmPowerStatus.输出电压电流)) return false;

            m_485Control.HighFrequencyPulse(int_HighFrequencyPulseBS);

            WaitAllReturn();

            return true;
        }

        #endregion

        #region 日计时误差检定
        private static bool Dgn_Clock()
        {
            if (!bIsContinue())
            {
                return false;
            }
            if (OnEventItemChange != null)
            {
                OnEventItemChange(enmMeterPrjID.日计时误差检定.ToString());
            }
            if ((GlobalUnit.g_Status == enmStatus.停止) || GlobalUnit.ForceVerifyStop)
            {
                return false;
            }
            RefreshPrjParameter(enmMeterPrjID.日计时误差检定, GlobalUnit.g_Plan.cDgnSy.PrjParameter);
            InstallMeter();
            if ((GlobalUnit.g_Status == enmStatus.停止) || GlobalUnit.ForceVerifyStop)
            {
                return false;
            }
            m_ComAdpater.ExecuteClockError(GlobalUnit.g_Meter.MInfo.Ub, 50f, 50f, 10, GlobalUnit.WCTimesPerPoint+8, dgtMeterDayErr);
            return true;

        }
        static ReturnSampleDataErrDelegate dgtMeterDayErr = new ReturnSampleDataErrDelegate(Up_MeterDayErr);
        private static void Up_MeterDayErr(string[,] val, int[] reSult)
        {
            int num = val.Length / reSult.Length;
            for (int i = 0; i < reSult.Length; i++)
            {
                if (GlobalUnit.g_Meter.MData[i].bolIsCheck)
                {
                    MeterDgnItem item = new MeterDgnItem();
                    item.Me_Bw = i + 1;
                    item.Item_PrjID = "1";
                    item.Item_PrjName = GlobalUnit.g_Plan.cDgnSy.PrjName;
                    if (reSult[i] == 1)
                    {
                        item.Item_Result = "合格";
                    }
                    else
                    {
                        item.Item_Result = "不合格";
                    }
                    for (int j = 0; j < num; j++)
                    {
                        item.Item_Value = item.Item_Value + val[i, j] + "|";
                    }
                    if (!GlobalUnit.g_Meter.MData[i].MeterDgns.ContainsKey(Convert.ToInt32(item.Item_PrjID).ToString()))
                    {
                        GlobalUnit.g_Meter.MData[i].MeterDgns.Add(Convert.ToInt32(item.Item_PrjID).ToString(), item);
                    }
                    else
                    {
                        foreach (MeterDgnItem item2 in GlobalUnit.g_Meter.MData[i].MeterDgns.Values)
                        {
                            item2.Item_Result = item.Item_Result;
                            item2.Item_Value = item.Item_Value;
                        }
                    }
                    bool flag = reSult[i] == 1;
                    OnEventMultiControllerh(enmMeterPrjID.日计时误差检定, i, flag, flag ? "" : ("日计时误差：" + item.Item_Value));
                }
            }
        }

        #endregion

        #region 分相供电测试
        private static bool SinglePhaseTest()
        {
            if (!bIsContinue())
            {
                return false;
            }
            if (OnEventItemChange != null)
            {
                OnEventItemChange(enmMeterPrjID.分相供电测试.ToString());
            }
            RefreshPrjParameter(enmMeterPrjID.分相供电测试, GlobalUnit.g_Plan.cSinglePhaseTest.PrjParameter);
            //string strXyName = "DLT645_1997";//"DLT645_1997";//
            //string strCode = "21212121";
            string strXyName = m_stPrjParameter.str_XyName; //"DLT645_1997";
            string strCode = "212121";//m_stPrjParameter.str_Code; //"FFF9";
            if (!RefreshProtocol(strXyName))
            {
                return false;
            }
            for (int i = 0; i < 3; i++)
            {
                if (!bIsContinue())
                {
                    GlobalUnit.g_Status = enmStatus.停止;
                    return false;
                }
                if (GlobalUnit.ForceVerifyStop || (GlobalUnit.g_Status == enmStatus.停止))
                {
                    return false;
                }
                if (i == 1 && Wiringmode == WiringMode.三相三线)
                {
                    continue;
                }

                m_StepSinglePhaseTest = i;
                string[] strArray = new string[] { "分相供电测试：A相输出:", "分相供电测试：B相输出:", "分相供电测试：C相输出:" };
                GlobalUnit.g_MsgControl.OutMessage(strArray[i], false);
                float uA = Convert.ToSingle((double)(GlobalUnit.g_Meter.MInfo.Ub * 0.7));
                switch (i)
                {
                    case 0:
                        m_ComAdpater.ExecutFenXiangPower(uA, 0f, 0f, Wiringmode, 1f, true, Pulse.正向有功);
                        break;

                    case 1:
                        m_ComAdpater.ExecutFenXiangPower(0f, uA, 0f, Wiringmode, 1f, true, Pulse.正向有功);
                        break;

                    case 2:
                        m_ComAdpater.ExecutFenXiangPower(0f, 0f, uA, Wiringmode, 1f, true, Pulse.正向有功);
                        break;
                }
                DelayTime(0x1388L);//5000
                m_485Control.ReadSinglePhaseTest(strCode);
                //m_485Control.ReadScbh(strCode);
                //m_485Control.ReadScbh(strCode, GlobalUnit.g_Meter.MData[0]._readdatecheck);//写入表号
                WaitAllReturn();
            }
            return true;

        }


        #endregion

        #region 交流采样测试
        private static bool ACSamplingTest()
        {
            if (!bIsContinue()) return false;
            if (OnEventItemChange != null) OnEventItemChange(enmMeterPrjID.交流采样测试.ToString());

            RefreshPrjParameter(enmMeterPrjID.交流采样测试, GlobalUnit.g_Plan.cACSamplingTest.PrjParameter);

            string strXyName = "DLT645_2007";
            string strCode = "0201FF00";

            if (!RefreshProtocol(strXyName)) return false;

            GlobalUnit.g_MsgControl.OutMessage("交流采样输出", false);
            float[] numArray = new float[] { GlobalUnit.g_Meter.MInfo.Ub * 1.1f, GlobalUnit.g_Meter.MInfo.Ub, GlobalUnit.g_Meter.MInfo.Ub * 0.9f };

            string _Ib01f = Convert.ToSingle(GlobalUnit.g_Meter.MInfo.Ib * 0.10f).ToString("F3");
            float[] numArray2 = new float[] { GlobalUnit.g_Meter.MInfo.IMax, GlobalUnit.g_Meter.MInfo.Ib,Convert.ToSingle(_Ib01f) };
            float[] numArray3 = new float[] { 1f, 0.5f, 0.8f };//1.0, 0.5L, 0.8C
            if (Wiringmode == WiringMode.三相三线)
            {
                numArray[1] = 0f;
                numArray2[1] = 0f;
            }
            //bool capacitive = true;//容性=true,感性=false
            m_ComAdpater.ExecutJcjjdItem(numArray[0], numArray2[0], numArray3[0], true, 
                numArray[1], numArray2[1], numArray3[1], false , 
                numArray[2], numArray2[2], numArray3[2], true , 
                Wiringmode, Pulse.正向有功);
            DelayTime(0x1f40L);//8000
            m_485Control.ReadACSamplingTest(strCode);
            WaitAllReturn();
            return true;

        }

        #endregion

        #region 读电能表底度
        private static bool ReadEnergy()
        {
            if (!bIsContinue()) return false;

            if (OnEventItemChange != null) OnEventItemChange(enmMeterPrjID.读电能表底度.ToString());

            RefreshPrjParameter(enmMeterPrjID.读电能表底度, GlobalUnit.g_Plan.cReadEnergy.PrjParameter);

            string StmpXyName = m_stPrjParameter.str_XyName;// "DLT645_2007";

            string StmpXyCode = m_stPrjParameter.str_Code;// "00010000";

            if (!RefreshProtocol(StmpXyName)) return false;

            m_485Control.ReadEnergy(StmpXyCode);

            WaitAllReturn();



            return true;
        }

        #endregion

        #region 打包参数下载
        private static bool DownPara()
        {
            if (!Adapter.bIsContinue()) return false;
            if (GlobalUnit.g_Plan.cDownParaItem._DownParaItem.Count == 0)
            {
                GlobalUnit.g_MsgControl.OutMessage("打包参数为空", false);
                return false;
            }
            if (OnEventItemChange != null) OnEventItemChange(enmMeterPrjID.打包参数下载.ToString());

            RefreshPrjParameter(enmMeterPrjID.打包参数下载, pwFunction.pwConst.GlobalUnit.g_Plan.cDownPara.PrjParameter);

            string StmpXyName = m_stPrjParameter.str_XyName;

            if (!RefreshProtocol(StmpXyName)) return false;

            #region
            //string[] StmpAddress = new string[GlobalUnit.g_BW];
            //for (int i = 0; i < GlobalUnit.g_BW; i++)
            //{
            //    StmpAddress[i] = m_stPrjParameter.str_Address;
            //}
            //Adapter.g_485Control.WriteMeterAddress(StmpAddress);
            //Adapter.WaitAllReturn();
            //StatusMain_Proc.Value++;
            #endregion


            if (!Adapter.bIsContinue()) return false;
            Adapter.g_485Control.blnSelected = m_bln_Selected;
            Adapter.g_485Control.DownPara(GlobalUnit.g_Plan.cDownParaItem._DownParaItem, m_stPrjParameter.str_Address);

            Adapter.WaitAllReturn();

            return true;
        }

        #endregion

        #region 系统清零
        private static bool SysClear()
        {
            if (!Adapter.bIsContinue()) return false;
            RefreshPrjParameter(enmMeterPrjID.系统清零,GlobalUnit.g_Plan.cSysClear.PrjParameter);

            string StmpXyName = m_stPrjParameter.str_XyName;// "DLT645_2007";

            if (OnEventItemChange != null) OnEventItemChange(enmMeterPrjID.系统清零.ToString());

            if (!RefreshProtocol(StmpXyName)) return false;

            Adapter.g_485Control.SysClear();

            Adapter.WaitAllReturn();
            DelayTime(10000);

            return true;

        }
        #endregion

        public static void DelayTime(long iTime)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Reset();
            stopwatch.Start();
            long elapsedMilliseconds = 0L;
            while (true)
            {
                if ((GlobalUnit.ApplicationIsOver || GlobalUnit.ForceVerifyStop) || (GlobalUnit.g_Status == enmStatus.停止))
                {
                    return;
                }
                elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                Thread.Sleep(5);
                if (elapsedMilliseconds > iTime)
                {
                    return;
                }
                Application.DoEvents();
            }
        }


        public void InspectStdParmTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Frontier.MeterVerification.KLDevice.stStdInfo info = new Frontier.MeterVerification.KLDevice.stStdInfo();
            info = m_ComAdpater.ReadStdParam();
            GlobalUnit.g_StdMeter.Ua = info.Ua;
            GlobalUnit.g_StdMeter.Ub = info.Ub;
            GlobalUnit.g_StdMeter.Uc = info.Uc;
            GlobalUnit.g_StdMeter.Ia = info.Ia;
            GlobalUnit.g_StdMeter.Ib = info.Ib;
            GlobalUnit.g_StdMeter.Ic = info.Ic;
            GlobalUnit.g_StdMeter.P = info.P;
            GlobalUnit.g_StdMeter.Pa = info.Pa;
            GlobalUnit.g_StdMeter.Pb = info.Pb;
            GlobalUnit.g_StdMeter.Pc = info.Pc;
            GlobalUnit.g_StdMeter.Phi_Ia = info.Phi_Ia;
            GlobalUnit.g_StdMeter.Phi_Ib = info.Phi_Ib;
            GlobalUnit.g_StdMeter.Phi_Ic = info.Phi_Ic;
            if (OnReadStaInfo != null)
            {
                OnReadStaInfo(GlobalUnit.g_StdMeter);
            }
        }

 

 

        #endregion
    }

}
