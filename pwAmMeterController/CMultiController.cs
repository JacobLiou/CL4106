/****************************************************************************

    多路控制多功能表通信操作
    刘伟 2012-08

*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using pwInterface;
using System.Reflection;
using System.Data;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Xml;
using pwClassLibrary;
using System.Windows.Forms;
namespace ClAmMeterController
{

    public class CMultiController
    {
        #region ------------------私有变量---------------------
        private int m_int_BwCount = 24;                     //表位数
        private pwMeterProtocolInfo[] m_cam_ProtocolInfo;   //被检表协议参数(所有表一样,否则需要使用数组)
        private static ISerialport[] S_ISP_COMLIST;         //端口例表  防止创建相同的端口
        private IMeterProtocol[] m_iap_MProtocol;           //通信协议
        private ISerialport[] m_isp_Comport;                //串口485
        private bool[] m_bln_Selected;                      //选中操作
        private int[][] m_int_ChannelMeterList;             //用于串口所带的表位
        private int m_int_ChannelCount;                     //串口通道数
        private object m_obj_LockObject = new object();     //用于锁定的
        private string m_str_LostMessage = "";              //操作失败信息
        private string[] m_str_MeterLostMessage;            //各位表操作失败信息
        private bool m_bln_Enabled;                         //创建成功
        private bool m_bln_HaveSameAddress;                 //有表位的电能表地址相同
        private string m_Setting;                           //波特率
        private bool[] m_bln_Return;                        //已经返回操作了

        private bool m_bln_Stop = false;                    //停止

        private Mutex m_mut_Mutex = new Mutex(false, "CL4101_02_CMultiController");//同步基元，

        #endregion

        #region------------------构造函数---------------------
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="p_int_BwCount">表位数</param>
        public CMultiController(int p_int_BwCount)
        {
            this.m_int_BwCount = p_int_BwCount;
            this.m_str_MeterLostMessage = new string[p_int_BwCount];
            this.m_cam_ProtocolInfo = new pwMeterProtocolInfo[p_int_BwCount];
            this.m_bln_Selected = new bool[p_int_BwCount];
            this.m_bln_HaveSameAddress = true;

            for (int int_Inc = 0; int_Inc < p_int_BwCount; int_Inc++)
            {
                this.m_cam_ProtocolInfo[int_Inc] = new pwMeterProtocolInfo();
                this.m_cam_ProtocolInfo[int_Inc].Setting = m_Setting;
            }

            string str_files = Directory.GetCurrentDirectory() + "\\ClMeterComPort.xml";

            if (!System.IO.File.Exists(str_files))                      //没有文件
            {
                CreateXMLFile(str_files);                               //创建默认文件
            }

            if (CreateIClass(str_files))
            {
                this.m_bln_Enabled = true;                              //如果创建成功，则打上标志
            }

            this.m_bln_Return = new bool[this.m_int_ChannelCount];
            this.RefreshProtocol(this.m_cam_ProtocolInfo);

        }

        ~CMultiController()
        {
            try
            {
                this.m_mut_Mutex.ReleaseMutex();
            }
            catch
            {
            }
        }

        #endregion

        #region ------------------私有函数---------------------

        /// <summary>
        /// 创建XML文件
        /// </summary>
        /// <param name="str_File"></param>
        private void CreateXMLFile(string str_File)
        {
            DataTable dat_XML = new DataTable("extension");
            string[] str_Name = new string[] { "UserID", "Index", "Interface", "Dllfile", "Class", "Parameter", "setting", "CParameter", "Channel" };
            for (int int_Inc = 0; int_Inc < str_Name.Length; int_Inc++)
            {
                DataColumn dac_Clum = new DataColumn(str_Name[int_Inc]);
                dac_Clum.DataType = System.Type.GetType("System.String");
                dac_Clum.DefaultValue = "";
                dat_XML.Columns.Add(dac_Clum);
            }

            for (int int_Inc = 0; int_Inc < m_int_BwCount; int_Inc++)
            {
                DataRow dar_Row = dat_XML.NewRow();
                dar_Row[str_Name[0]] = "COM";
                dar_Row[str_Name[1]] = Convert.ToString(int_Inc + 1);
                dar_Row[str_Name[2]] = "ISerialport";
                dar_Row[str_Name[3]] = "pwComPorts";
                dar_Row[str_Name[4]] = "CCL20181";
                dar_Row[str_Name[5]] = Convert.ToString(int_Inc + 1) + ",193.168.18.1:10003:20000";
                dar_Row[str_Name[6]] = "9600,e,8,1";
                dar_Row[str_Name[7]] = Convert.ToString(int_Inc + 1) + "/" + m_int_BwCount.ToString();
                dar_Row[str_Name[8]] = "";
                dat_XML.Rows.Add(dar_Row);
            }
            dat_XML.AcceptChanges();
            dat_XML.WriteXml(str_File);
        }


        /// <summary>
        /// 根据配置文件创建各设备通信类
        /// </summary>
        /// <param name="str_XmlFile">配置文件</param>
        private bool CreateIClass(string str_XmlFile)
        {
            try
            {
                DataSet dst_XmlConfig = new DataSet();
                dst_XmlConfig.ReadXml(str_XmlFile);

                DataRow[] daw_cRowArry = dst_XmlConfig.Tables["extension"].Select("UserID='COM'");
                if (daw_cRowArry == null)
                {
                    this.m_str_LostMessage = "找不到配置信息！";
                    return false;         //找不到配置信息
                }

                this.m_int_ChannelCount = daw_cRowArry.Length;

                this.m_isp_Comport = new ISerialport[this.m_int_ChannelCount];
                this.m_int_ChannelMeterList = new int[this.m_int_ChannelCount][];

                int int_Index = 0;
                foreach (DataRow daw_cRow in daw_cRowArry)
                {

                    string str_Dllfiles = daw_cRow["Dllfile"].ToString() + ".dll";
                    string str_Class = daw_cRow["Class"].ToString();
                    string str_Para = daw_cRow["Parameter"].ToString();
                    string str_Setting = daw_cRow["setting"].ToString();
                    m_Setting = str_Setting;
                    string str_CPara = daw_cRow["CParameter"].ToString();
                    int int_Channel = 0;

                    if (daw_cRow["Channel"] != null && daw_cRow["Channel"].ToString() != "")
                        int_Channel = Convert.ToInt16(daw_cRow["Channel"].ToString());

                    string[] str_files = Directory.GetFiles(Directory.GetCurrentDirectory(), str_Dllfiles);

                    if (str_files.Length == 0)
                    {
                        this.m_str_LostMessage = "指定的串口DLL文件！";
                        return false;         //找不到配置信息
                    }

                    this.m_isp_Comport[int_Index] = CreateSerialComClass(str_files[0], str_Class, str_Para);
                    if (this.m_isp_Comport[int_Index] == null)
                    {
                        continue;
                    }
                    this.m_isp_Comport[int_Index].PortOpen(str_Para);
                    this.m_isp_Comport[int_Index].Setting = str_Setting;
                    this.m_isp_Comport[int_Index].Channel = int_Channel;
                    //this.m_int_ChannelMeterList[int_Index] = GetChannelList(str_CPara);

                    int_Index++;
                }
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }


        /// <summary>
        /// 创建通信对象
        /// </summary>
        /// <param name="str_DLLFile"></param>
        /// <param name="str_Class"></param>
        /// <param name="str_Para"></param>
        /// <returns></returns>
        private ISerialport CreateSerialComClass(string str_DLLFile, string str_Class, string str_Para)
        {
            try
            {
                if (S_ISP_COMLIST != null)
                {
                    for (int int_Inc = 0; int_Inc < S_ISP_COMLIST.Length; int_Inc++)
                    {
                        if (S_ISP_COMLIST[int_Inc].ComPort.ToString() + "," + S_ISP_COMLIST[int_Inc].IP == str_Para)
                        {
                            return S_ISP_COMLIST[int_Inc];
                        }
                    }
                }

                Type typ_Object = GetICType(str_DLLFile, "ISerialport", str_Class);
                if (typ_Object != null)
                {

                    if (S_ISP_COMLIST == null)
                    {
                        S_ISP_COMLIST = new ISerialport[1];
                    }
                    else
                    {
                        Array.Resize(ref S_ISP_COMLIST, S_ISP_COMLIST.Length + 1);
                    }
                    S_ISP_COMLIST[S_ISP_COMLIST.Length - 1] = (ISerialport)Activator.CreateInstance(typ_Object);
                    return S_ISP_COMLIST[S_ISP_COMLIST.Length - 1];
                }

                //this.m_str_LostMessage = "找不到指定配置端口类！";
                return null;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return null;
            }
        }

        /// <summary>
        /// 根据配置文件关闭各设备通信类端口
        /// </summary>
        /// <param name="str_XmlFile">配置文件</param>
        /// <returns></returns>
        public bool ClosePort(string str_XmlFile)
        {
            try
            {
                DataSet dst_XmlConfig = new DataSet();
                dst_XmlConfig.ReadXml(str_XmlFile);

                DataRow[] daw_cRowArry = dst_XmlConfig.Tables["extension"].Select("UserID='COM'");
                if (daw_cRowArry == null)
                {
                    this.m_str_LostMessage = "找不到配置信息！";
                    return false;         //找不到配置信息
                }
                this.m_int_ChannelCount = daw_cRowArry.Length;

                this.m_isp_Comport = new ISerialport[this.m_int_ChannelCount];
                this.m_int_ChannelMeterList = new int[this.m_int_ChannelCount][];

                int int_Index = 0;
                foreach (DataRow daw_cRow in daw_cRowArry)
                {
                    string str_Dllfiles = daw_cRow["Dllfile"].ToString() + ".dll";
                    string str_Class = daw_cRow["Class"].ToString();
                    string str_Para = daw_cRow["Parameter"].ToString();
                    string str_Setting = daw_cRow["setting"].ToString();
                    string str_CPara = daw_cRow["CParameter"].ToString();
                    int int_Channel = 0;

                    if (daw_cRow["Channel"] != null && daw_cRow["Channel"].ToString() != "")
                        int_Channel = Convert.ToInt16(daw_cRow["Channel"].ToString());

                    string[] str_files = Directory.GetFiles(Directory.GetCurrentDirectory(), str_Dllfiles);

                    if (str_files.Length == 0)
                    {
                        this.m_str_LostMessage = "指定的串口DLL文件！";
                        return false;         //找不到配置信息
                    }
                    this.m_isp_Comport[int_Index] = CreateSerialComClass(str_files[0], str_Class, str_Para);
                    if (this.m_isp_Comport[int_Index] == null)
                    {
                        continue;
                    }
                    this.m_isp_Comport[int_Index].PortClose();
                    int_Index++;
                }
                return true;

            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 根据文件，接口和类，提取类型号
        /// </summary>
        /// <param name="str_DLLFile">DLL文件</param>
        /// <param name="str_Interface">接口名</param>
        /// <param name="str_Class">类名</param>
        /// <returns></returns>
        private Type GetICType(string str_DLLFile, string str_Interface, string str_Class)
        {
            try
            {
                Assembly aby_DllFile = Assembly.LoadFile(str_DLLFile);      //动态加载文件
                if (aby_DllFile != null)                            //
                {
                    Type[] tpe_Types = aby_DllFile.GetTypes();        //取出当前.DLL所有类
                    for (int int_Inb = 0; int_Inb < tpe_Types.Length; int_Inb++)     //遍历当前.DLL文件中的所有类是存在为ClassName的名称
                    {
                        if (tpe_Types[int_Inb].Name == str_Class)        //存在为ClassName的名称
                        {
                            Type[] tpe_ITypes = tpe_Types[int_Inb].GetInterfaces();    //取出当前类的所有继承的接口
                            for (int int_Ina = 0; int_Ina < tpe_ITypes.Length; int_Ina++)        //判断这个类是否继承str_Interface这个接口
                            {
                                if (tpe_ITypes[int_Ina].Name == str_Interface) return tpe_Types[int_Inb];     //是继承于str_Interface接口,才是要找的
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return null;
            }
        }

        /// <summary>
        /// 刷新各表位协议
        /// </summary>
        /// <param name="cam_MeterInfo">表协议</param>
        private void RefreshProtocol(pwMeterProtocolInfo[] cam_MeterInfo)
        {
            int int_Count = cam_MeterInfo.Length;
            this.m_iap_MProtocol = new IMeterProtocol[int_Count];
            for (int int_Inc = 0; int_Inc < int_Count; int_Inc++)
            {
                string[] str_files = Directory.GetFiles(Directory.GetCurrentDirectory(), cam_MeterInfo[int_Inc].DllFile + ".dll");
                if (str_files.Length > 0)
                {
                    Type typ_Object = GetICType(str_files[0], "IMeterProtocol", cam_MeterInfo[int_Inc].ClassName);
                    if (typ_Object != null)
                    {
                        this.m_iap_MProtocol[int_Inc] = (IMeterProtocol)Activator.CreateInstance(typ_Object);
                        this.m_iap_MProtocol[int_Inc].Address = cam_MeterInfo[int_Inc].Address;
                        this.m_iap_MProtocol[int_Inc].Setting = cam_MeterInfo[int_Inc].Setting;//是不是这里07协议波特率设置错误 2015 1218 还有就是 300 e 7 1 // 9600 e 8 1 
                        this.m_iap_MProtocol[int_Inc].UserID = cam_MeterInfo[int_Inc].UserID;
                        this.m_iap_MProtocol[int_Inc].VerifyPasswordType = cam_MeterInfo[int_Inc].VerifyPasswordType;
                        this.m_iap_MProtocol[int_Inc].FECount = cam_MeterInfo[int_Inc].FECount;


                        //数据上行事件
                        m_iap_MProtocol[int_Inc].OnEventRxFrame += new Dge_EventRxFrame(CMultiController_OnEventRxFrame);
                        //数据下行事件
                        m_iap_MProtocol[int_Inc].OnEventTxFrame += new Dge_EventTxFrame(CMultiController_OnEventTxFrame);
                    }
                    else
                    {
                        this.m_str_MeterLostMessage[int_Inc] = "找不到配置的协议类";
                        this.m_iap_MProtocol[int_Inc] = null;
                    }
                }
                else
                {
                    this.m_str_MeterLostMessage[int_Inc] = "找不到协议动态库";
                    this.m_iap_MProtocol[int_Inc] = null;
                }
            }
        }


        /// <summary>
        /// 刷新各表位协议
        /// </summary>
        /// <param name="cam_MeterInfo">表协议</param>
        public void RefreshProtocol(pwMeterProtocolInfo cam_MeterInfo)
        {
            int int_Count = m_int_BwCount;
            this.m_iap_MProtocol = new IMeterProtocol[int_Count];
            for (int int_Inc = 0; int_Inc < int_Count; int_Inc++)
            {
                string[] str_files = Directory.GetFiles(Directory.GetCurrentDirectory(), cam_MeterInfo.DllFile + ".dll");
                if (str_files.Length > 0)
                {
                    Type typ_Object = GetICType(str_files[0], "IMeterProtocol", cam_MeterInfo.ClassName);
                    if (typ_Object != null)
                    {
                        this.m_iap_MProtocol[int_Inc] = (IMeterProtocol)Activator.CreateInstance(typ_Object);
                        this.m_iap_MProtocol[int_Inc].Address = cam_MeterInfo.Address;
                        this.m_iap_MProtocol[int_Inc].Setting = cam_MeterInfo.Setting;
                        this.m_iap_MProtocol[int_Inc].UserID = cam_MeterInfo.UserID;
                        this.m_iap_MProtocol[int_Inc].VerifyPasswordType = cam_MeterInfo.VerifyPasswordType;
                        this.m_iap_MProtocol[int_Inc].FECount = cam_MeterInfo.FECount;
                        this.m_iap_MProtocol[int_Inc].WaitDataRevTime = cam_MeterInfo.WaitDataRevTime;
                        this.m_iap_MProtocol[int_Inc].Password = cam_MeterInfo.WritePassword;
                        this.m_iap_MProtocol[int_Inc].PasswordClass = Convert.ToByte(cam_MeterInfo.WritePswClass);

                        //数据上行事件
                        m_iap_MProtocol[int_Inc].OnEventRxFrame += new Dge_EventRxFrame(CMultiController_OnEventRxFrame);
                        //数据下行事件
                        m_iap_MProtocol[int_Inc].OnEventTxFrame += new Dge_EventTxFrame(CMultiController_OnEventTxFrame);
                    }
                    else
                    {
                        this.m_str_MeterLostMessage[int_Inc] = "找不到配置的协议类";
                        this.m_iap_MProtocol[int_Inc] = null;
                    }
                }
                else
                {
                    this.m_str_MeterLostMessage[int_Inc] = "找不到协议动态库";
                    this.m_iap_MProtocol[int_Inc] = null;
                }
            }
        }


        /// <summary>
        /// 数据发送事件
        /// </summary>
        /// <param name="str_Frame"></param>
        private void CMultiController_OnEventTxFrame(int intCom, string str_Frame)
        {
            if (OnEventRxFrame != null)
                OnEventTxFrame(intCom, str_Frame);
        }
        /// <summary>
        /// 数据接收事件
        /// </summary>
        /// <param name="str_Frame"></param>
        private void CMultiController_OnEventRxFrame(int intCom, string str_Frame)
        {
            if (OnEventRxFrame != null)
            {
                OnEventRxFrame(intCom, str_Frame);
            }
            //throw new Exception("The method or operation is not implemented.");
        }


        /// <summary>
        /// 多线程处理
        /// </summary>
        /// <param name="ctp_TPara">线程传输参数</param>
        /// <param name="ptt_ParaThreadStart">委托函数</param>
        /// <returns></returns>
        private bool MultiThreadingDispose(CThreadParameter[] ctp_TPara, ParameterizedThreadStart ptt_ParaThreadStart)
        {
            try
            {
                Thread[] thd_DoingThread = new Thread[0];       //创建线程
                int[] int_NO = new int[0];
                for (int int_Inc = 0; int_Inc < ctp_TPara.Length; int_Inc++)  //线程对象
                {
                    if (this.m_bln_Selected[int_Inc])
                    {
                        Array.Resize(ref thd_DoingThread, thd_DoingThread.Length + 1);
                        Array.Resize(ref int_NO, int_NO.Length + 1);
                        int_NO[int_NO.Length - 1] = int_Inc;
                        thd_DoingThread[thd_DoingThread.Length - 1] = new Thread(ptt_ParaThreadStart);
                        thd_DoingThread[thd_DoingThread.Length - 1].Name = "ClChannel" + int_Inc.ToString();
                        thd_DoingThread[thd_DoingThread.Length - 1].IsBackground = true;
                    }
                }

                for (int int_Inc = 0; int_Inc < thd_DoingThread.Length; int_Inc++)
                {
                    if (this.m_bln_Selected[int_NO[int_Inc]])//int_Inc
                    {
                        thd_DoingThread[int_Inc].Start(ctp_TPara[int_NO[int_Inc]]);              //运行线程
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        #endregion

        #region ------------------继承接口---------------------


        #region -------------------事件说明-------------------
        /// <summary>
        /// 数据接收事件
        /// </summary>
        public event Dge_EventRxFrame OnEventRxFrame;
        /// <summary>
        /// 数据下发事件
        /// </summary>
        public event Dge_EventTxFrame OnEventTxFrame;

        /// <summary>
        /// 表操作事件
        /// </summary>
        public event DelegateEventMultiController OnEventMultiControlle;

        ///// <summary>
        ///// 表操作事件（返回Bool值）
        ///// </summary>
        //public event DelegateEventResuleChange OnEventResuleChange;
        #endregion

        #region -------------------属性-------------------

        /// <summary>
        /// 表位数
        /// </summary>
        public int BWCount
        {
            get
            {
                return this.m_int_BwCount;
            }
            set
            {
                this.m_int_BwCount = value;
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <returns></returns>
        public bool IsStop
        {
            set
            {
                this.m_bln_Stop = value;

                for (int i = 0; i < BWCount; i++)
                    if (m_iap_MProtocol[i] != null) m_iap_MProtocol[i].BreakDown = value;
            }
        }

        /// <summary>
        /// 操作失败信息
        /// </summary>
        public string LostMessage
        {
            get { return this.m_str_LostMessage; }
        }

        /// <summary>
        /// 表位通信操作失败信息
        /// </summary>
        public string[] MeterLostMessage
        {
            get { return this.m_str_MeterLostMessage; }
        }

        /// <summary>
        /// 各位是要操作
        /// </summary>
        public bool[] blnSelected
        {
            get { return this.m_bln_Selected; }
            set { this.m_bln_Selected = value; }
        }

        /// <summary>
        /// 各位要操作是否完成
        /// </summary>
        public bool[] blnReturn
        {
            get { return this.m_bln_Return; }
            //set { this.m_bln_Return = value; }
        }

        /// <summary>
        /// 被检表通信速率（用于红外通信时暂时保存表485的通信字）
        /// </summary>
        public string str485Setting
        {
            //set { this.m_Setting = value; }
            get { return this.m_Setting; }
        }



        #endregion

        #endregion

        #region ------------------线程函数---------------------

        #region -------------------读生产编号＿多表位同时处理-------------------

        /// <summary>
        /// 读生产编号
        /// </summary>
        /// <param name="str_Address">各表位写入地址</param>
        /// <returns></returns>
        public bool ReadScbh(string strCode, string tablename)
        {
            try
            {
                CThreadParameter[] ctp_TPara = new CThreadParameter[this.m_int_ChannelCount];       //线程参数
                ParameterizedThreadStart ptt_ParaThreadStart = new ParameterizedThreadStart(this.ThreadReadScbh);  //委托函数
                for (int int_Inc = 0; int_Inc < this.m_int_ChannelCount; int_Inc++)
                {
                    ctp_TPara[int_Inc] = new CThreadParameter();                    //线程参数
                    ctp_TPara[int_Inc].int_BwNo = int_Inc;//1拖1
                    ctp_TPara[int_Inc].int_ChannelNo = int_Inc;
                    ctp_TPara[int_Inc].str_Parameter1 = strCode;
                    ctp_TPara[int_Inc].str_Parameter2 = tablename;
                    this.m_bln_Return[int_Inc] = false;
                }
                bool bln_Result = MultiThreadingDispose(ctp_TPara, ptt_ParaThreadStart);
                return bln_Result;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 读生产编号
        /// </summary>
        /// <param name="str_Address">各表位写入地址</param>
        /// <returns></returns>
        public bool ReadScbh(string strCode)
        {
            try
            {
                CThreadParameter[] ctp_TPara = new CThreadParameter[this.m_int_ChannelCount];       //线程参数
                ParameterizedThreadStart ptt_ParaThreadStart = new ParameterizedThreadStart(this.ThreadReadScbh);  //委托函数
                for (int int_Inc = 0; int_Inc < this.m_int_ChannelCount; int_Inc++)
                {
                    ctp_TPara[int_Inc] = new CThreadParameter();                    //线程参数
                    ctp_TPara[int_Inc].int_BwNo = int_Inc;//1拖1
                    ctp_TPara[int_Inc].int_ChannelNo = int_Inc;
                    ctp_TPara[int_Inc].str_Parameter1 = strCode;
                    this.m_bln_Return[int_Inc] = false;
                }
                bool bln_Result = MultiThreadingDispose(ctp_TPara, ptt_ParaThreadStart);
                return bln_Result;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 线程控制某通道进行读生产编号
        /// </summary>
        /// <param name="obj_Para">线程传参数</param>
        private void ThreadReadScbh(object obj_Para)
        {
            CThreadParameter ctp_ThreadPara = (CThreadParameter)obj_Para;
            int int_Index = ctp_ThreadPara.int_ChannelNo;
            this.m_bln_Return[int_Index] = false;
            string  int_Index2 = ctp_ThreadPara.str_Parameter2;
           
            //string strReadScbhReturn = ctp_ThreadPara.int_Parameter2.ToString ();

            string strReadScbhReturn = "";

            int int_BwNo = ctp_ThreadPara.int_BwNo;
            try
            {
                if (this.m_bln_Selected[int_BwNo])
                {
                    this.m_iap_MProtocol[int_BwNo].ComPort = this.m_isp_Comport[int_Index];
                    //bool bln_Result = this.m_iap_MProtocol[int_BwNo].ReadScbh(ref strReadScbhReturn);
                    string strinfo = "";
                    bool bln_Result = true;
                    if (int_Index2 == "2")
                    {
                        strReadScbhReturn = ctp_ThreadPara.str_Parameter1;

                        bln_Result = this.m_iap_MProtocol[int_BwNo].ReadScbh(ref strReadScbhReturn);
                       
                        //strReadScbhReturn = Sstring.ASCIIEncodingToString(strReadScbhReturn);


                        if (bln_Result && ctp_ThreadPara.str_Parameter1=="212121")
                        {
                            #region 生产编号 统一处理

                            if (strReadScbhReturn.Length > 24)  //if (sRxData.Length > iLen * 2)
                            {
                                strReadScbhReturn = strReadScbhReturn.Substring(0, 24);
                            }

                            ////返回数据帧 实例：
                            ////<-FE-68-AA-AA-AA-AA-AA-AA-68-83-10-23-32-B3-81-67-7B-64-63-63-63-63-69-63-66-64-38-88-16

                            #region   ASCII 数据转换
                                string stmp = "";
                                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                                for (int j = 0; j < strReadScbhReturn.Length / 2; j++)
                                {
                                    byte[] byteArray = new byte[] { Convert.ToByte(strReadScbhReturn.Substring(2 * j, 2), 16) };
                                    stmp += asciiEncoding.GetString(byteArray);
                                }
                                strReadScbhReturn = stmp.Replace("\0", "");
                            
                            #endregion
 
                            #endregion
                        }
                        if (!bln_Result)
                        {
                            strinfo = "读生产编号失败";
                            if (strReadScbhReturn == "000000000000000000000000" || strReadScbhReturn == "FFFFFFFFFFFFFFFFFFFFFFFF")
                            {
                                strinfo = "生产编号全为0或F";
                                bln_Result = false;
                            }
                        }
                         
                    }
                    else
                    {                        
                            strReadScbhReturn = "99";
                       
                    }
                    //strReadScbhReturn = Sstring.ASCIIEncodingToString(strReadScbhReturn);
                    if (!bln_Result) this.m_str_MeterLostMessage[int_BwNo] = this.m_iap_MProtocol[int_BwNo].LostMessage;
                    if (this.OnEventMultiControlle != null) this.OnEventMultiControlle(enmMeterPrjID.RS485读生产编号, int_BwNo, bln_Result, bln_Result ? strReadScbhReturn : strinfo);

                }
                else
                    this.m_bln_Return[int_Index] = true;
            }
            catch (Exception e)
            {
                this.m_str_MeterLostMessage[int_BwNo] = int_Index.ToString() + "通信号出错:" + e.ToString();
                if (this.OnEventMultiControlle != null) this.OnEventMultiControlle(enmMeterPrjID.RS485读生产编号, int_BwNo, false, "读生产编号通信出错");
            }
            finally
            {
                this.m_bln_Return[int_Index] = true;
            }
        }
        #endregion

        #region -------------------系统清零＿多表位同时处理-------------------

        /// <summary>
        /// 系统清零
        /// </summary>
        /// <returns></returns>
        public bool SysClear()
        {
            try
            {
                CThreadParameter[] ctp_TPara = new CThreadParameter[this.m_int_ChannelCount];       //线程参数
                ParameterizedThreadStart ptt_ParaThreadStart = new ParameterizedThreadStart(this.ThreadSysClear);  //委托函数
                for (int int_Inc = 0; int_Inc < this.m_int_ChannelCount; int_Inc++)
                {
                    ctp_TPara[int_Inc] = new CThreadParameter();                    //线程参数
                    ctp_TPara[int_Inc].int_BwNo = int_Inc;//1拖1，
                    ctp_TPara[int_Inc].int_ChannelNo = int_Inc;
                    this.m_bln_Return[int_Inc] = false;
                }
                bool bln_Result = MultiThreadingDispose(ctp_TPara, ptt_ParaThreadStart);
                return bln_Result;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }

        /// <summary>
        /// 线程控制某通道某表位进行通信
        /// </summary>
        /// <param name="obj_Para">线程传参数,通道号及表位号</param>
        private void ThreadSysClear(object obj_Para)
        {
            CThreadParameter ctp_ThreadPara = (CThreadParameter)obj_Para;
            int int_Index = ctp_ThreadPara.int_ChannelNo;
            this.m_bln_Return[int_Index] = false;
            int int_BwNo = ctp_ThreadPara.int_BwNo;
            try
            {
                if (this.m_bln_Selected[int_BwNo])
                {
                    System.Windows.Forms.Application.DoEvents();
                    this.m_iap_MProtocol[int_BwNo].ComPort = this.m_isp_Comport[int_Index];
                    bool bln_Result = this.m_iap_MProtocol[int_BwNo].SysClear();//("04CC1000", 1, "55");
                    if (!bln_Result)
                        this.m_str_MeterLostMessage[int_BwNo] = this.m_iap_MProtocol[int_BwNo].LostMessage;
                    if (this.OnEventMultiControlle != null) this.OnEventMultiControlle(enmMeterPrjID.系统清零, int_BwNo, bln_Result, bln_Result ? "" : "系统清零失败");
                }
                else
                    this.m_bln_Return[int_Index] = true;
            }
            catch (Exception e)
            {
                this.m_str_MeterLostMessage[int_BwNo] = int_Index.ToString() + "通信号出错:" + e.ToString();
                if (this.OnEventMultiControlle != null) this.OnEventMultiControlle(enmMeterPrjID.系统清零, int_BwNo, false, "系统清零通信出错");
            }
            finally
            {
                this.m_bln_Return[int_Index] = true;
            }
        }
        #endregion

        #region -------------------打包参数下载＿多表位同时处理-------------------

        /// <summary>
        /// 打包参数下载
        /// </summary>
        /// <param name="str_Address">各表位写入地址</param>
        /// <returns></returns>
        public bool DownPara(List<MeterDownParaItem> _DownParaItem, string _DownParaAddress)
        {
            try
            {
                CThreadParameter[] ctp_TPara = new CThreadParameter[this.m_int_ChannelCount];       //线程参数
                ParameterizedThreadStart ptt_ParaThreadStart = new ParameterizedThreadStart(this.ThreadDownPara);  //委托函数
                for (int int_Inc = 0; int_Inc < this.m_int_ChannelCount; int_Inc++)
                {
                    ctp_TPara[int_Inc] = new CThreadParameter();                    //线程参数
                    ctp_TPara[int_Inc].int_BwNo = int_Inc;//1拖1，
                    ctp_TPara[int_Inc].int_ChannelNo = int_Inc;
                    ctp_TPara[int_Inc].DownParaItem = _DownParaItem;
                    ctp_TPara[int_Inc].str_Parameter1 = _DownParaAddress;
                    this.m_bln_Return[int_Inc] = false;
                }
                bool bln_Result = MultiThreadingDispose(ctp_TPara, ptt_ParaThreadStart);
                return bln_Result;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 线程控制某通道进行通信
        /// </summary>
        /// <param name="obj_Para">线程传参数</param>
        private void ThreadDownPara(object obj_Para)
        {
            CThreadParameter ctp_ThreadPara = (CThreadParameter)obj_Para;
            int int_Index = ctp_ThreadPara.int_ChannelNo;
            this.m_bln_Return[int_Index] = false;
            string str_Address = ctp_ThreadPara.str_Parameter1;
            int int_BwNo = ctp_ThreadPara.int_BwNo;
            try
            {
                if (this.m_bln_Selected[int_BwNo])
                {
                    this.m_iap_MProtocol[int_BwNo].ComPort = this.m_isp_Comport[int_Index];
                    bool bln_Result = this.m_iap_MProtocol[int_BwNo].DownPara(ctp_ThreadPara.DownParaItem);
                    if (!bln_Result) this.m_str_MeterLostMessage[int_BwNo] = this.m_iap_MProtocol[int_BwNo].LostMessage;
                    if (this.OnEventMultiControlle != null) this.OnEventMultiControlle(enmMeterPrjID.打包参数下载, int_BwNo, bln_Result, bln_Result ? "" : "打包参数下载失败");
                }
                else
                    this.m_bln_Return[int_Index] = true;
            }
            catch (Exception e)
            {
                this.m_str_MeterLostMessage[int_BwNo] = int_Index.ToString() + "通信号出错:" + e.ToString();
                if (this.OnEventMultiControlle != null) this.OnEventMultiControlle(enmMeterPrjID.打包参数下载, int_BwNo, false, "打包参数下载通信出错");
            }
            finally
            {
                this.m_bln_Return[int_Index] = true;
            }
        }
        #endregion

        #region -------------------高频检定＿多表位同时处理-------------------

        /// <summary>
        /// 高频检定
        /// </summary>
        /// <returns></returns>
        public bool HighFrequencyPulse(int intBs)
        {
            try
            {
                Thread.Sleep(5000);//等待源稳定
                CThreadParameter[] ctp_TPara = new CThreadParameter[this.m_int_ChannelCount];       //线程参数
                ParameterizedThreadStart ptt_ParaThreadStart = new ParameterizedThreadStart(this.ThreadSHighFrequencyPulse);  //委托函数
                for (int int_Inc = 0; int_Inc < this.m_int_ChannelCount; int_Inc++)
                {
                    ctp_TPara[int_Inc] = new CThreadParameter();                    //线程参数
                    ctp_TPara[int_Inc].int_BwNo = int_Inc;//1拖1，
                    ctp_TPara[int_Inc].int_ChannelNo = int_Inc;
                    ctp_TPara[int_Inc].int_Parameter1 = intBs;//放大常数5倍
                    this.m_bln_Return[int_Inc] = false;
                }
                bool bln_Result = MultiThreadingDispose(ctp_TPara, ptt_ParaThreadStart);
                return bln_Result;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }

        /// <summary>
        /// 线程控制某通道某表位进行通信
        /// </summary>
        /// <param name="obj_Para">线程传参数,通道号及表位号</param>
        private void ThreadSHighFrequencyPulse(object obj_Para)
        {
            CThreadParameter ctp_ThreadPara = (CThreadParameter)obj_Para;
            int int_Index = ctp_ThreadPara.int_ChannelNo;
            int int_Bs = ctp_ThreadPara.int_Parameter1;
            this.m_bln_Return[int_Index] = false;
            int int_BwNo = ctp_ThreadPara.int_BwNo;
            try
            {
                if (this.m_bln_Selected[int_BwNo])
                {
                    System.Windows.Forms.Application.DoEvents();
                    this.m_iap_MProtocol[int_BwNo].ComPort = this.m_isp_Comport[int_Index];
                    bool bln_Result = this.m_iap_MProtocol[int_BwNo].HighFrequencyPulse(int_Bs);//WriteData("04CC0509", 1, int_Bs);
                    if (!bln_Result)
                        this.m_str_MeterLostMessage[int_BwNo] = this.m_iap_MProtocol[int_BwNo].LostMessage;
                    if (this.OnEventMultiControlle != null) this.OnEventMultiControlle(enmMeterPrjID.误差检定, int_BwNo, bln_Result, bln_Result ? "" : "设置高频检定参数失败");
                }
                else
                    this.m_bln_Return[int_Index] = true;
            }
            catch (Exception e)
            {
                this.m_str_MeterLostMessage[int_BwNo] = int_Index.ToString() + "通信号出错:" + e.ToString();
                if (this.OnEventMultiControlle != null) this.OnEventMultiControlle(enmMeterPrjID.误差检定, int_BwNo, false, "设置高频检定参数通信出错");
            }
            finally
            {
                this.m_bln_Return[int_Index] = true;
            }
        }
        #endregion


        #region -------------------读电能量＿多表位同时处理-------------------

        /// <summary>
        /// 读电能量
        /// </summary>
        /// <param name="str_Address">各表位写入地址</param>
        /// <returns></returns>
        public bool ReadEnergy(string strCode)
        {
            try
            {
                CThreadParameter[] ctp_TPara = new CThreadParameter[this.m_int_ChannelCount];       //线程参数
                ParameterizedThreadStart ptt_ParaThreadStart = new ParameterizedThreadStart(this.ThreadReadEnergy);  //委托函数
                for (int int_Inc = 0; int_Inc < this.m_int_ChannelCount; int_Inc++)
                {
                    ctp_TPara[int_Inc] = new CThreadParameter();                    //线程参数
                    ctp_TPara[int_Inc].int_BwNo = int_Inc;//1拖1
                    ctp_TPara[int_Inc].int_ChannelNo = int_Inc;
                    ctp_TPara[int_Inc].str_Parameter1 = strCode;
                    this.m_bln_Return[int_Inc] = false;
                }
                bool bln_Result = MultiThreadingDispose(ctp_TPara, ptt_ParaThreadStart);
                return bln_Result;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 线程控制某通道进行读生产编号
        /// </summary>
        /// <param name="obj_Para">线程传参数</param>
        private void ThreadReadEnergy(object obj_Para)
        {
            CThreadParameter ctp_ThreadPara = (CThreadParameter)obj_Para;
            int int_Index = ctp_ThreadPara.int_ChannelNo;
            this.m_bln_Return[int_Index] = false;
            float strReadReturn = 0f;
            int int_BwNo = ctp_ThreadPara.int_BwNo;
            try
            {
                if (this.m_bln_Selected[int_BwNo])
                {
                    this.m_iap_MProtocol[int_BwNo].ComPort = this.m_isp_Comport[int_Index];
                    bool bln_Result = this.m_iap_MProtocol[int_BwNo].ReadData(ctp_ThreadPara.str_Parameter1, 4, 2, ref strReadReturn);
                    if (!bln_Result) this.m_str_MeterLostMessage[int_BwNo] = this.m_iap_MProtocol[int_BwNo].LostMessage;
                    if (this.OnEventMultiControlle != null) this.OnEventMultiControlle(enmMeterPrjID.读电能表底度, int_BwNo, bln_Result, bln_Result ? strReadReturn.ToString() : "读电能量失败");

                }
                else
                    this.m_bln_Return[int_Index] = true;
            }
            catch (Exception e)
            {
                this.m_str_MeterLostMessage[int_BwNo] = int_Index.ToString() + "通信号出错:" + e.ToString();
                if (this.OnEventMultiControlle != null) this.OnEventMultiControlle(enmMeterPrjID.读电能表底度, int_BwNo, false, "读电能量通信出错");
            }
            finally
            {
                this.m_bln_Return[int_Index] = true;
            }
        }
        #endregion


        #region -------------------分相供电测试＿多表位同时处理-------------------

        /// <summary>
        /// 分相供电测试
        /// </summary>
        /// <param name="strCode">参数</param>
        /// <returns></returns>
        public bool ReadSinglePhaseTest(string strCode)
        {
            try
            {
                CThreadParameter[] ctp_TPara = new CThreadParameter[this.m_int_ChannelCount];       //线程参数
                ParameterizedThreadStart ptt_ParaThreadStart = new ParameterizedThreadStart(this.ThreadReadSinglePhaseTest);  //委托函数
                for (int int_Inc = 0; int_Inc < this.m_int_ChannelCount; int_Inc++)
                {
                    ctp_TPara[int_Inc] = new CThreadParameter();                    //线程参数
                    ctp_TPara[int_Inc].int_BwNo = int_Inc;//1拖1
                    ctp_TPara[int_Inc].int_ChannelNo = int_Inc;
                    ctp_TPara[int_Inc].str_Parameter1 = strCode;
                    this.m_bln_Return[int_Inc] = false;
                }
                bool bln_Result = MultiThreadingDispose(ctp_TPara, ptt_ParaThreadStart);
                return bln_Result;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 线程控制某通道进行读生产编号
        /// </summary>
        /// <param name="obj_Para">线程传参数</param>
        private void ThreadReadSinglePhaseTest(object obj_Para)
        {
            CThreadParameter ctp_ThreadPara = (CThreadParameter)obj_Para;
            int int_Index = ctp_ThreadPara.int_ChannelNo;
            this.m_bln_Return[int_Index] = false;
            string strReadReturn = "212121";
            int int_BwNo = ctp_ThreadPara.int_BwNo;
            try
            {
                if (this.m_bln_Selected[int_BwNo])
                {
                    this.m_iap_MProtocol[int_BwNo].ComPort = this.m_isp_Comport[int_Index];
                    bool bln_Result = this.m_iap_MProtocol[int_BwNo].ReadSinglePhaseTest(ref strReadReturn);
                    if (!bln_Result) this.m_str_MeterLostMessage[int_BwNo] = this.m_iap_MProtocol[int_BwNo].LostMessage;
                    if (this.OnEventMultiControlle != null) this.OnEventMultiControlle(enmMeterPrjID.分相供电测试, int_BwNo, bln_Result, bln_Result ? strReadReturn.ToString() : "分相供电测试失败");

                }
                else
                    this.m_bln_Return[int_Index] = true;
            }
            catch (Exception e)
            {
                this.m_str_MeterLostMessage[int_BwNo] = int_Index.ToString() + "通信号出错:" + e.ToString();
                if (this.OnEventMultiControlle != null) this.OnEventMultiControlle(enmMeterPrjID.分相供电测试, int_BwNo, false, "分相供电测试通信出错");
            }
            finally
            {
                this.m_bln_Return[int_Index] = true;
            }
        }
        #endregion

        #region -------------------交流采样测试＿多表位同时处理-------------------

        /// <summary>
        /// 交流采样测试
        /// </summary>
        /// <param name="strCode">参数</param>
        /// <returns></returns>
        public bool ReadACSamplingTest(string strCode)
        {
            try
            {
                Thread.Sleep(2000);
                CThreadParameter[] ctp_TPara = new CThreadParameter[this.m_int_ChannelCount];       //线程参数
                ParameterizedThreadStart ptt_ParaThreadStart = new ParameterizedThreadStart(this.ThreadReadACSamplingTest);  //委托函数
                for (int int_Inc = 0; int_Inc < this.m_int_ChannelCount; int_Inc++)
                {
                    ctp_TPara[int_Inc] = new CThreadParameter();                    //线程参数
                    ctp_TPara[int_Inc].int_BwNo = int_Inc;//1拖1
                    ctp_TPara[int_Inc].int_ChannelNo = int_Inc;
                    ctp_TPara[int_Inc].str_Parameter1 = strCode;
                    this.m_bln_Return[int_Inc] = false;
                }
                bool bln_Result = MultiThreadingDispose(ctp_TPara, ptt_ParaThreadStart);
                return bln_Result;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 线程控制某通道进行读生产编号
        /// </summary>
        /// <param name="obj_Para">线程传参数</param>
        private void ThreadReadACSamplingTest(object obj_Para)
        {
            CThreadParameter ctp_ThreadPara = (CThreadParameter)obj_Para;
            int int_Index = ctp_ThreadPara.int_ChannelNo;
            this.m_bln_Return[int_Index] = false;
            string strReadReturn = "";
            int int_BwNo = ctp_ThreadPara.int_BwNo;
            try
            {
                if (this.m_bln_Selected[int_BwNo])
                {
                    this.m_iap_MProtocol[int_BwNo].ComPort = this.m_isp_Comport[int_Index];
                    bool bln_Result = this.m_iap_MProtocol[int_BwNo].ReadACSamplingTest(ref strReadReturn);
                    if (!bln_Result) this.m_str_MeterLostMessage[int_BwNo] = this.m_iap_MProtocol[int_BwNo].LostMessage;
                    if (this.OnEventMultiControlle != null)
                        this.OnEventMultiControlle(enmMeterPrjID.交流采样测试, int_BwNo, bln_Result, bln_Result ? strReadReturn.ToString() : "交流采样测试失败");

                }
                else
                    this.m_bln_Return[int_Index] = true;
            }
            catch (Exception e)
            {
                this.m_str_MeterLostMessage[int_BwNo] = int_Index.ToString() + "通信号出错:" + e.ToString();
                if (this.OnEventMultiControlle != null)
                    this.OnEventMultiControlle(enmMeterPrjID.交流采样测试, int_BwNo, false, "交流采样测试通信出错");
            }
            finally
            {
                this.m_bln_Return[int_Index] = true;
            }
        }
        #endregion

        #endregion

    }





}
