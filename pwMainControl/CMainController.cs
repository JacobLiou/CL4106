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

namespace pwMainControl
{   
    
    public class CMainController 
    {
        #region ------------------私有变量---------------------
        private int m_int_BwCount = 1;                      //表位数
        private static ISerialport[] S_ISP_COMLIST;         //端口例表  防止创建相同的端口
        private IMainControlProtocol[] m_iap_MProtocol;     //主控通信协议
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
        private bool m_bln_Link;                            //联机是否成功

        private bool m_bln_Stop = false;                    //停止

        private Mutex m_mut_Mutex = new Mutex(false, "CL4101_02_CMultiController");//同步基元，

        private string str_files = Directory.GetCurrentDirectory() + "\\ClMainControlComPort.xml";

        #endregion

        #region------------------构造函数---------------------
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="p_int_BwCount">表位数</param>
        public CMainController()
        {
            //this.m_int_BwCount = p_int_BwCount;
            this.m_str_MeterLostMessage = new string[m_int_BwCount];
            this.m_bln_Selected = new bool[m_int_BwCount];
            this.m_bln_HaveSameAddress = true;

            if (!System.IO.File.Exists(str_files))                      //没有文件
            {
                CreateXMLFile(str_files);                               //创建默认文件
            }

            if (CreateIClass(str_files))
            {
                this.m_bln_Enabled = true;                              //如果创建成功，则打上标志
            }
            
            this.m_bln_Return = new bool[this.m_int_ChannelCount];

            this.RefreshProtocol();

        }

        ~CMainController()
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
        private void RefreshProtocol()
        {
            this.m_iap_MProtocol = new IMainControlProtocol[m_int_BwCount];
            for (int int_Inc = 0; int_Inc < m_int_BwCount; int_Inc++)
            {
                string[] str_files = Directory.GetFiles(Directory.GetCurrentDirectory(), "pwMainControl.dll");
                if (str_files.Length > 0)
                {
                    Type typ_Object = GetICType(str_files[0], "IMainControlProtocol", "CMainControl");
                    if (typ_Object != null)
                    {
                        this.m_iap_MProtocol[int_Inc] = (IMainControlProtocol)Activator.CreateInstance(typ_Object);
                        this.m_iap_MProtocol[int_Inc].ComPort = this.m_isp_Comport[int_Inc];

                        //this.m_iap_MProtocol[int_Inc].Address = cam_MeterInfo[int_Inc].Address;
                        //this.m_iap_MProtocol[int_Inc].Setting = cam_MeterInfo[int_Inc].Setting;
                        //this.m_iap_MProtocol[int_Inc].UserID = cam_MeterInfo[int_Inc].UserID;
                        //this.m_iap_MProtocol[int_Inc].VerifyPasswordType = cam_MeterInfo[int_Inc].VerifyPasswordType;
                        //this.m_iap_MProtocol[int_Inc].FECount = cam_MeterInfo[int_Inc].FECount;

                        //数据上行事件
                        m_iap_MProtocol[int_Inc].OnEventMainControl += new  DelegateEventMainControl(OnEventMainControllered);

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
                OnEventTxFrame(intCom,str_Frame);
        }
        /// <summary>
        /// 数据接收事件
        /// </summary>
        /// <param name="str_Frame"></param>
        private void CMultiController_OnEventRxFrame(int intCom, string str_Frame)
        {
            if (OnEventRxFrame != null)
            {
                OnEventRxFrame(intCom,str_Frame);
            }
            //throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 主控板事件
        /// </summary>
        /// <param name="Typ_Cmd"></param>
        private void OnEventMainControllered(int Typ_Cmd)
        {
            if (OnEventMainControl != null)
            {
                OnEventMainControl(Typ_Cmd);
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
        /// 主控板事件
        /// </summary>
        public event DelegateEventMainControl OnEventMainControl;

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

                for(int i=0;i<BWCount ;i++)
                    m_iap_MProtocol[i].BreakDown = value;
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

        /// <summary>
        /// 联机成功
        /// </summary>
        public bool blnLink
        {
            get { return this.m_bln_Link; }
        }


        #endregion

        #endregion

        #region ---------后装应用方法----------
        /// <summary>
        /// 升源(F930,F950)
        /// </summary>
        /// <param name="Typ_Cmd">Typ_Cmd为00=表示220V给A组电表供电上电，01=表示154V给A组电表供电上电，02=表示220V给B组电表供电上电，03=表示154V给B组电表供电上电</param>
        /// <returns></returns>
        public bool SetPowerOn(int Typ_Cmd)
        {   //其中XX为00时表示220V给A组电表供电，
            //      为01时表示154V给A组电表供电，
            //      为02时表示220V给B组电表供电，
            //      为03时表示154V给B组电表供电

            return m_iap_MProtocol[0].SetPowerOn(Typ_Cmd);

        }

        /// <summary>
        /// 继电器切换电源给电表供电(F930)
        /// </summary>
        /// <param name="Typ_Cmd">Typ_Cmd为00=表示220V给A组电表供电，01=表示154V给A组电表供电，02=表示220V给B组电表供电，03=表示154V给B组电表供电</param>
        /// <returns></returns>
        public bool SetJDQChange(int Typ_Cmd)
        {   //其中XX为00时表示220V给A组电表供电，
            //      为01时表示154V给A组电表供电，
            //      为02时表示220V给B组电表供电，
            //      为03时表示154V给B组电表供电

            return m_iap_MProtocol[0].SetJDQChange(Typ_Cmd);
        }

        /// <summary>
        /// 给表位上电断电(F950)
        /// </summary>
        /// <param name="Typ_Cmd">Typ_Cmd为00=表示给A组表位断电，01=表示给A组表位上电，02=表示给B组表位断电，03=表示给B组表位上电</param>
        /// <returns></returns>
        public bool SetPowerOnOff(int Typ_Cmd)
        {   //其中XX为00时表示给A组表位断电，
            //      为01时表示给A组表位上电，
            //      为02时表示给B组表位断电，
            //      为03时表示给B组表位上电
            return m_iap_MProtocol[0].SetPowerOnOff(Typ_Cmd);
        }


        #endregion

        #region ------------------应用方法---------------------



        /// <summary>
        /// 启动、复位停止(F980)
        /// </summary>
        /// <param name="Typ_Cmd">Typ_Cmd为00=表示A组复位停止，01=表示A组启动，02=表示B组复位停止，03=表示B组启动</param>
        /// <returns></returns>
        public bool SetStartStopCmd(int Typ_Cmd)
        {
            return m_iap_MProtocol[0].SetStartStopCmd(Typ_Cmd);


        }

        /// <summary>
        /// 气缸控制 --上、下(F913)  SunBoy Add
        /// </summary>
        /// <param name="Typ_Cmd">Typ_Cmd为00=表示A组气缸下降，01=表示A组气缸上升，04=表示A组气缸停止，02=表示B组气缸下降，03=表示B组气缸上升 05=表示B组气缸停止，</param>
        /// <returns></returns>
        public bool SetStartStopQG(int Typ_Cmd)
        {
            return m_iap_MProtocol[0].SetStartStopQG(Typ_Cmd);
        }

        /// <summary>
        /// 设置测试状态(F90F)
        /// </summary>
        /// <param name="Typ_Cmd">XX为00时表示A组测试完毕，为01时表示A组测试中，为02时表示B组测试完毕，为03时表示B组测试中</param>
        /// <returns></returns>
        public bool SetTestStatusCmd(int Typ_Cmd)
        {
                return m_iap_MProtocol[0].SetTestStatusCmd(Typ_Cmd);
        }


        /// <summary>
        /// 选择测试继电器(F907)
        /// </summary>
        /// <param name="Typ_Cmd">Typ_Cmd为00时表示选择测试内置继电器，为01时表示选择测试外置继电器，为02时表示外置隔离继电器</param>
        /// <returns></returns>
        public bool SelectJDQ(int Typ_Cmd)
        {
             m_bln_Link=m_iap_MProtocol[0].SelectJDQ(Typ_Cmd);
             return m_bln_Link;

        }

        /// <summary>
        /// A组电流校淮切换(F931)
        /// </summary>
        /// <param name="Typ_Cmd">Typ_Cmd为为00=表示大电流校准，01=表示小电流校准</param>
        /// <returns></returns>
        public bool AdjustCmdA(int Typ_Cmd)
        {
            return m_iap_MProtocol[0].AdjustCmdA(Typ_Cmd);

        }

        /// <summary>
        /// B组电流校淮切换(F931)
        /// </summary>
        /// <param name="Typ_Cmd">Typ_Cmd为为00=表示大电流校准，01=表示小电流校准</param>
        /// <returns></returns>
        public bool AdjustCmdB(int Typ_Cmd)
        {
            return m_iap_MProtocol[0].AdjustCmdB(Typ_Cmd);


        }


        /// <summary>
        /// 读A组继电器状态(F908)
        /// </summary>
        /// <param name="str_Valu">返回值XX表示读到的A组继电器状态，01表示1#表位拉闸故障，02表示2#表位拉闸故障，04表示3#表位拉闸故障， 03表示1#2#表位拉闸故障，05表示1#3#表位拉闸故障，06表示2#3#表位拉闸故障，07表示1#2#3#表位拉闸故障，00表示无故障</param>
        /// <returns></returns>
        public bool ReadJDQA(ref string str_Value)
        {
            return m_iap_MProtocol[0].ReadJDQA(ref str_Value);


        }

        /// <summary>
        /// 读B组继电器状态(F909)
        /// </summary>
        /// <param name="str_Valu">返回值XX表示读到的B组继电器状态，08表示4#表位拉闸故障，10表示5#表位拉闸故障，20表示6#表位拉闸故障，18表示4#、5#表位拉闸故障，28表示4#、6#表位拉闸故障，30表示5#、6#表位拉闸故障，38表示4#、5#、6#表位拉闸故障，00表示无故障</param>
        /// <returns></returns>
        public bool ReadJDQB(ref string str_Value)
        {
            return m_iap_MProtocol[0].ReadJDQB(ref str_Value);


        }


        /// <summary>
        /// 设置A组表位故障灯(F90D)
        /// </summary>
        /// <param name="Typ_Cmd">XX表示A组故障灯状态，01表示1#表位故障灯亮，02表示2#表位故障灯亮，04表示3#表位故障灯亮， 03表示1#2#表位故障灯亮，05表示1#3#表位故障灯亮，06表示2#3#表位故障灯亮，07表示1#2#3#表位故障灯亮，00表示无故障</param>
        /// <returns></returns>
        public bool SetGzdengA(int Typ_Cmd)
        {
            return m_iap_MProtocol[0].SetGzdengA(Typ_Cmd);


        }

        /// <summary>
        /// 设置B组表位故障灯(F90E)
        /// </summary>
        /// <param name="Typ_Cmd">XX表示B组故障灯状态，08表示4#表位故障灯亮，10表示5#表位故障灯亮，20表示6#表位故障灯亮，18表示4#、5#表位故障灯亮，28表示4#、6#表位故障灯亮，30表示5#、6#表位故障灯亮，38表示4#、5#、6#表位故障灯亮，00表示无故障</param>
        /// <returns></returns>
        public bool SetGzdengB(int Typ_Cmd)
        {
            return m_iap_MProtocol[0].SetGzdengB(Typ_Cmd<<3);


        }

        /// <summary>
        /// 整机自检开始命令
        /// </summary>
        /// <param name="Typ_Cmd">0为A组，1为B组</param>
        /// <returns></returns>
        public bool SetSelfCheckStart(int Typ_Cmd)
        {
            return m_iap_MProtocol[0].SetSelfCheckStart(Typ_Cmd);
        }

        /// <summary>
        /// 整机自检结束命令
        /// </summary>
        /// <param name="Typ_Cmd">0为A组，1为B组</param>
        /// <returns></returns>
        public bool SetSelfCheckEnd(int Typ_Cmd, ref string cDataStr)
        {
            return m_iap_MProtocol[0].SetSelfCheckEnd(Typ_Cmd, ref cDataStr);
        }

        #endregion

    }





}
