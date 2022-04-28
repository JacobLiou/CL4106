using System;
using System.Collections.Generic;
using System.Text;
using pwInterface;
using System.Data;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Threading;
using pwFunction.pwGlysModel;

namespace pwCommAdapter
{
    /// <summary>
    /// 装置控制类，主要非手动控制器控制模式,如V80、N1系列、N6系列等,不包括等电位
    /// 如尼6系列硬件
    /// </summary>
    public class ClCommV7 : ICommAdapter
    {
        private static ISerialport[] S_ISP_COMLIST;    //端口例表
        private IPower m_ipr_Power;             //程控源 
        private IStdMeter m_isr_StdMeter;       //标准表
        private IStdTime m_ise_StdTime;         //时基源
        private IErrorCalculate[] m_iee_ErrCal; //误差板
        private int[][] m_int_ErrCalListTable;  //多路控制误差板列表
        private string m_str_LostMessage = "";  //失败原因
        private Stopwatch sth_SpaceTicker = new Stopwatch(); //记时时钟

        //***********************
        //private bool m_bln_StdAuto = true;          //自动档，手动档
        //private long m_lng_StdConst = 4000000;        //CL1115电能脉冲常数

        //private float m_lng_StdCurP = 0f;             //CL1115当前功率
        //private int m_int_StdFPXS = 1;              //CL1115分频系数


        //private int m_int_AmClfs = 0;               //测试方式
        //private long m_lng_AmConst = 16000;         //被检表常数
        //private long m_lng_AmPulse = 2;             //圈数
        //private int m_int_AmMeterPulseBS;           //被检表脉冲常数缩放倍数
        //private long m_lng_StdTimeConst = 500000;     //CL191标准时钟脉冲常数
        //private float m_int_AmTimeHz = 1f;            //被检时钟频率
        //private int m_int_AmTimePulse = 10;           //被检脉冲个数

        //private enmTaskType m_ett_TaskType = enmTaskType.电能误差;
        //private int m_int_188PulseChannel;           //脉冲通道,0=P+,1=P-,2=Q+,3=Q-,4=需量,5=时钟
        //private int m_int_188ChannelType;            //通道类型,0=脉冲盒,1=光电头
        //private int m_int_188PulseType;              //脉冲类型,0=共阳,1=共阴


        //*************************


        private bool[] m_bln_LinkResult = new bool[4];          //接机情况
        private bool[] m_bln_ErrCalLinkResult;                  //误差板接机情况
        private int m_int_BWCount = 24;                         //24表位

        private Single[] sng_CurU = new Single[3];              //当前电压
        private Single[] sng_CurI = new Single[3];              //当前电流

        private bool m_bln_Enabled;                 //对象是否正常
        private bool m_bln_StopTask = false;        //停止当前任务

        private string m_str_files = Directory.GetCurrentDirectory() + "\\ClPlugins.xml";

        //操作锁
        private object objStopTaskLock = new object();

        public ClCommV7(int int_BWCount)
        {
            try
            {
                this.m_int_BWCount = int_BWCount;
                this.m_bln_ErrCalLinkResult = new bool[int_BWCount];
                if (!System.IO.File.Exists(m_str_files))      //没有文件
                    CreateXMLFile(m_str_files);                //创建默认文
                if (CreateIClass(m_str_files)) this.m_bln_Enabled = true;      //如果创建成功，则打上标志
            }
            catch (Exception e)
            {
                this.m_bln_Enabled = false;
                this.m_str_LostMessage = e.ToString();
            }
        }

        #region -------------------私有函数---------------------

        /// <summary>
        /// 创建XML文件
        /// </summary>
        /// <param name="str_File"></param>
        private void CreateXMLFile(string str_File)
        {
            DataTable dat_XML = new DataTable("extension");
            string[] str_Name = new string[] { "UserID", "Interface", "Dllfile", "Class", "ComDllfile",
                                               "ComClass", "Parameter","setting" ,"CParameter","Channel"};
            for (int int_Inc = 0; int_Inc < 10; int_Inc++)
            {
                DataColumn dac_Clum = new DataColumn(str_Name[int_Inc]);
                dac_Clum.DataType = System.Type.GetType("System.String");
                dac_Clum.DefaultValue = "";
                dat_XML.Columns.Add(dac_Clum);
            }

            string[][] str_Value = new string[][] { 
            new string []{"ClCommV7","IPower","pwCommAdapter","CCL109","pwComPorts","CCL20181","33,193.168.18.1:10003:20000","38400,n,8,1","",""},
            new string []{"ClCommV7","IStdMeter","pwCommAdapter","CCL1115","pwComPorts","CCL20181","30,193.168.18.1:10003:20000","38400,n,8,1","",""},
            new string []{"ClCommV7","IErrorCalculate","pwCommAdapter","CCL188L","pwComPorts","CCL20181","31,193.168.18.1:10003:20000","38400,n,8,1","1/1",""},
            new string []{"ClCommV7","IStdTime","pwCommAdapter","CCL191","pwComPorts","CCL20181","32,193.168.18.1:10003:20000","2400,n,8,1","",""}
            };

            for (int int_Inc = 0; int_Inc < str_Value.Length; int_Inc++)
            {
                DataRow dar_Row = dat_XML.NewRow();
                for (int int_Inb = 0; int_Inb < str_Value[int_Inc].Length; int_Inb++)
                    dar_Row[str_Name[int_Inb]] = str_Value[int_Inc][int_Inb];
                dat_XML.Rows.Add(dar_Row);
            }

            dat_XML.AcceptChanges();
            dat_XML.WriteXml(str_File);
        }


        /// <summary>
        /// 建默认各设备类,根据V80默认配置
        /// </summary>
        /// <returns></returns>
        private bool CreateIClassDefault()
        {

            try
            {
                string[] str_Interface = new string[] { "IPower", "IStdMeter", "IErrorCalculate", "IStdTime" };
                string[] str_Calss = new string[] { "CCL109", "CCL1115", "CCL188L", "CCL191" };
                string[] str_ComPara = new string[] { "33,193.168.18.1:10003:20000", "30,193.168.18.1:10003:20000", "31,193.168.18.1:10003:20000", "32,193.168.18.1:10003:20000" };
                string[] str_ComSetting = new string[] { "38400,n,8,1", "38400,n,8,1", "38400,n,8,1", "2400,n,8,1" };

                string[] str_files = Directory.GetFiles(Directory.GetCurrentDirectory(), "pwCommAdapter.dll");
                string[] str_Comfiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "pwComPorts.dll");

                if (str_files.Length <= 0)
                {
                    this.m_str_LostMessage = "找不到协议类库！";
                    return false;
                }
                if (str_Comfiles.Length <= 0)
                {
                    this.m_str_LostMessage = "找不到串口类库！";
                    return false;
                }
                for (int int_Inc = 0; int_Inc < 4; int_Inc++)
                {
                    Type typ_Object = GetICType(str_files[0], str_Interface[int_Inc], str_Calss[int_Inc]);
                    ISerialport ist_tmpCom = CreateSerialComClass(str_Comfiles[0], "CCL20181", str_ComPara[int_Inc]);
                    if (ist_tmpCom == null) return false;
                    ist_tmpCom.PortOpen(str_ComPara[int_Inc]);
                    ist_tmpCom.Setting = str_ComSetting[int_Inc];
                    if (!ist_tmpCom.State)
                        this.m_str_LostMessage = ist_tmpCom.LostMessage;

                    switch(str_Interface[int_Inc])
                    {
                        case "IPower" :
                            this.m_ipr_Power = (IPower)Activator.CreateInstance(typ_Object);
                            this.m_ipr_Power.ComPort = ist_tmpCom;
                            this.m_ipr_Power.Setting = str_ComSetting[int_Inc];
                            break ;

                        case "IStdMeter":
                            this.m_isr_StdMeter = (IStdMeter)Activator.CreateInstance(typ_Object);
                            this.m_isr_StdMeter.ComPort = ist_tmpCom;
                            this.m_isr_StdMeter.Setting = str_ComSetting[int_Inc];
                            break ;
                        case "IErrorCalculate"://可以配置多个误差板
                            if (this.m_iee_ErrCal == null)
                            {
                                this.m_iee_ErrCal = new IErrorCalculate[1];
                                this.m_int_ErrCalListTable = new int[1][];
                            }
                            else
                            {
                                Array.Resize(ref  this.m_iee_ErrCal, this.m_iee_ErrCal.Length + 1);
                                Array.Resize(ref  this.m_int_ErrCalListTable, this.m_iee_ErrCal.Length + 1);
                            }
                            this.m_iee_ErrCal[this.m_iee_ErrCal.Length - 1] = (IErrorCalculate)Activator.CreateInstance(typ_Object);
                            this.m_iee_ErrCal[this.m_iee_ErrCal.Length - 1].ComPort = ist_tmpCom;
                            this.m_int_ErrCalListTable[this.m_iee_ErrCal.Length - 1] = GetBWListTable("1/1");
                            this.m_iee_ErrCal[this.m_iee_ErrCal.Length - 1].SetECListTable(this.m_int_ErrCalListTable[this.m_iee_ErrCal.Length - 1]);  //设置当前这一路控制误差板的列表
                            this.m_iee_ErrCal[this.m_iee_ErrCal.Length - 1].Setting = str_ComSetting[int_Inc];
                            break ;
                        case "IStdTime":
                            this.m_ise_StdTime = (IStdTime)Activator.CreateInstance(typ_Object);
                            this.m_ise_StdTime.ComPort = ist_tmpCom;
                            this.m_ise_StdTime.Setting = str_ComSetting[int_Inc];
                            break;
                    }

                }

                if (this.m_ipr_Power == null)
                {
                    this.m_str_LostMessage = "没有创建程控源对象！";
                    return false;
                }

                if (this.m_isr_StdMeter == null)
                {
                    this.m_str_LostMessage = "没有创建标准表对象！";
                    return false;
                }

                if (this.m_ise_StdTime == null)
                {
                    this.m_str_LostMessage = "没有创建时基源对象！";
                    return false;
                }

                if (this.m_iee_ErrCal == null)
                {
                    this.m_str_LostMessage = "没有创建误差板对象！";
                    return false;
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
        /// 根据配置文件创建各设备通信类
        /// </summary>
        /// <param name="str_XmlFile">配置文件</param>
        private bool CreateIClass(string str_XmlFile)
        {
            try
            {
                DataSet dst_XmlConfig = new DataSet();
                dst_XmlConfig.ReadXml(str_XmlFile);


                DataRow[] daw_cRowArry = dst_XmlConfig.Tables["extension"].Select("UserID='ClCommV7'");
                if (daw_cRowArry == null)
                {
                    this.m_str_LostMessage = "找不到配置信息！";
                    return false;         //找不到配置信息
                }

                foreach (DataRow daw_cRow in daw_cRowArry)
                {
                    string str_Dllfiles = daw_cRow["Dllfile"].ToString() + ".dll";
                    string[] str_files = Directory.GetFiles(Directory.GetCurrentDirectory(), str_Dllfiles);
                    if (str_files.Length <= 0)
                    {
                        continue;
                    }
                    string str_Interface = daw_cRow["Interface"].ToString();
                    string str_Class = daw_cRow["Class"].ToString();

                    string str_ComDllfiles = daw_cRow["ComDllfile"].ToString() + ".dll";
                    string[] str_Comfiles = Directory.GetFiles(Directory.GetCurrentDirectory(), str_ComDllfiles);
                    if (str_Comfiles.Length <= 0)
                    {
                        continue;
                    }
                    string str_ComClass = daw_cRow["ComClass"].ToString();
                    string str_Para = daw_cRow["Parameter"].ToString();
                    string str_Setting = daw_cRow["setting"].ToString();
                    int int_Channel = 1;
                    if (daw_cRow["Channel"] == null || daw_cRow["Channel"].ToString() == "")
                        int_Channel = 1;
                    else
                    {
                        int_Channel = Convert.ToInt16(daw_cRow["Channel"].ToString());
                        if (int_Channel < 1 || int_Channel > 4) int_Channel = 1;
                    }

                    ISerialport ist_tmpCom = CreateSerialComClass(str_Comfiles[0], str_ComClass, str_Para);
                    if (ist_tmpCom == null)
                    {
                        continue;
                    }

                    ist_tmpCom.PortOpen(str_Para);
                    ist_tmpCom.Setting = str_Setting;
                    if (!ist_tmpCom.State)
                        this.m_str_LostMessage = ist_tmpCom.LostMessage;

                    Type typ_Object = GetICType(str_files[0], str_Interface, str_Class);

                    switch(str_Interface)
                    {
                        case "IPower":
                            this.m_ipr_Power = (IPower)Activator.CreateInstance(typ_Object);
                            this.m_ipr_Power.ComPort = ist_tmpCom;
                            this.m_ipr_Power.Setting = str_Setting;
                            this.m_ipr_Power.Channel = int_Channel;
                            break ;
                        case "IStdMeter":
                            this.m_isr_StdMeter = (IStdMeter)Activator.CreateInstance(typ_Object);
                            this.m_isr_StdMeter.ComPort = ist_tmpCom;
                            this.m_isr_StdMeter.Setting = str_Setting;
                            this.m_isr_StdMeter.Channel = int_Channel;
                            break ;
                        case "IErrorCalculate"://可以配置多个误差板
                            if (this.m_iee_ErrCal == null)
                            {
                                this.m_iee_ErrCal = new IErrorCalculate[1];
                                this.m_int_ErrCalListTable = new int[1][];
                            }
                            else
                            {
                                Array.Resize(ref  this.m_iee_ErrCal, this.m_iee_ErrCal.Length + 1);
                                Array.Resize(ref  this.m_int_ErrCalListTable, this.m_iee_ErrCal.Length + 1);
                            }
                            string str_CPara = daw_cRow["CParameter"].ToString();
                            this.m_iee_ErrCal[this.m_iee_ErrCal.Length - 1] = (IErrorCalculate)Activator.CreateInstance(typ_Object);
                            this.m_iee_ErrCal[this.m_iee_ErrCal.Length - 1].ComPort = ist_tmpCom;
                            this.m_int_ErrCalListTable[this.m_iee_ErrCal.Length - 1] = GetBWListTable(str_CPara);
                            this.m_iee_ErrCal[this.m_iee_ErrCal.Length - 1].SetECListTable(this.m_int_ErrCalListTable[this.m_iee_ErrCal.Length - 1]);  //设置当前这一路控制误差板的列表
                            this.m_iee_ErrCal[this.m_iee_ErrCal.Length - 1].Setting = str_Setting;
                            this.m_iee_ErrCal[this.m_iee_ErrCal.Length - 1].Channel = int_Channel;
                            break ;
                        case "IStdTime":
                            this.m_ise_StdTime = (IStdTime)Activator.CreateInstance(typ_Object);
                            this.m_ise_StdTime.ComPort = ist_tmpCom;
                            this.m_ise_StdTime.Setting = str_Setting;
                            this.m_ise_StdTime.Channel = int_Channel;
                            break;
                    }
                }

                if (this.m_ipr_Power == null)
                {
                    this.m_str_LostMessage = "没有创建程控源对象！";
                    return false;
                }

                if (this.m_isr_StdMeter == null)
                {
                    this.m_str_LostMessage = "没有创建标准表对象！";
                    return false;
                }

                if (this.m_ise_StdTime == null)
                {
                    this.m_str_LostMessage = "没有创建时基源对象！";
                    return false;
                }

                if (this.m_iee_ErrCal == null)
                {
                    this.m_str_LostMessage = "没有创建误差板对象！";
                    return false;
                }
                this.m_str_LostMessage = "所有对像创建成功！";
                return true;

            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }



        }

        /// <summary>
        /// 取出每个控制误差板的列表
        /// </summary>
        /// <param name="str_CPara">参数 格式：1(当前路数)/3(路数总和)</param>
        /// <returns></returns>
        private int[] GetBWListTable(string str_CPara)
        {
            string[] str_Para = str_CPara.Split(new Char[] { '/' });
            int[] int_BwList;
            if (str_Para.Length >= 2)
            {
                int int_Start = Convert.ToInt16(str_Para[0]);
                int int_Sum = Convert.ToInt16(str_Para[1]);
                if (int_Start <= 0) int_Start = 1;
                if (int_Sum <= 0) int_Sum = 1;

                if ((this.m_int_BWCount % int_Sum > 0) && int_Start == int_Sum)     //如果不能平均，并且是最后一路，则全部在最后一路
                    int_Sum = this.m_int_BWCount / int_Sum + this.m_int_BWCount % int_Sum;
                else
                    int_Sum = this.m_int_BWCount / int_Sum;     //根据总表位数的平均分配

                int_BwList = new int[int_Sum];
                for (int int_Inc = 0; int_Inc < int_Sum; int_Inc++)
                {
                    int_BwList[int_Inc] = (int_Start - 1) * int_Sum + int_Inc + 1;
                }
            }
            else
            {
                int_BwList = new int[this.m_int_BWCount];
                for (int int_Inc = 0; int_Inc < this.m_int_BWCount; int_Inc++)
                {
                    int_BwList[int_Inc] = int_Inc + 1;
                }
            }
            return int_BwList;
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
                        S_ISP_COMLIST = new ISerialport[1];
                    else
                        Array.Resize(ref S_ISP_COMLIST, S_ISP_COMLIST.Length + 1);
                    S_ISP_COMLIST[S_ISP_COMLIST.Length - 1] = (ISerialport)Activator.CreateInstance(typ_Object);
                    return S_ISP_COMLIST[S_ISP_COMLIST.Length - 1];
                }

                this.m_str_LostMessage = "找不到指定配置端口类！";
                return null;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return null;
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
        /// 计算角度
        /// </summary>
        /// <param name="int_Clfs">测量方式</param>
        /// <param name="str_Glys">功率因数</param>
        /// <returns></returns>
        private Single GetPhiGlys(int int_Clfs, string str_Glys)
        {
            string str_CL = str_Glys.ToUpper().Substring(str_Glys.Length - 1, 1);
            Double dbl_XS = 0;
            if (str_CL == "C" || str_CL == "L")
                dbl_XS = Convert.ToDouble(str_Glys.Substring(0, str_Glys.Length - 1));
            else
                dbl_XS = Convert.ToDouble(str_Glys);
            Double dbl_Phase;
            if (int_Clfs == 1 || int_Clfs == 3 || int_Clfs == 6)
                dbl_Phase = Math.Asin(Math.Abs(dbl_XS));                              //无功计算
            else
                dbl_Phase = Math.Acos(Math.Abs(dbl_XS));                              //有功计算

            dbl_Phase = dbl_Phase * 180 / Math.PI;      //角度换算
            if (dbl_XS < 0) dbl_Phase = 180 + dbl_Phase;         //反向
            if (str_CL == "C") dbl_Phase = 360 - dbl_Phase;
            if (dbl_Phase < 0) dbl_Phase = 360 + dbl_Phase;
            dbl_Phase %= 360;
            return Convert.ToSingle(dbl_Phase);
        }

        /// <summary>
        /// 计算角度
        /// </summary>
        /// <param name="p_int_Clfs">测量方式</param>
        /// <param name="p_str_Glys">功率因素名</param>
        /// <param name="p_eet_Element">合分元</param>
        /// <returns></returns>
        private float GetPhiGlys(int p_int_Clfs, string p_str_Glys, enmElement p_eet_Element)
        {
            string str_GlysName = p_str_Glys.Replace("-", "").Replace("+", "");
            string str_GlysF = p_str_Glys.Contains("-") ? "-" : "+";
            string str_ClfsCode = "";
            string str_ElementCode = "";

            switch (p_int_Clfs)
            {
                case 0:
                    str_ClfsCode = "00";
                    break;
                case 1:
                    str_ClfsCode = "01";
                    break;
                case 2:
                    str_ClfsCode = "10";
                    break;
                case 3:
                    str_ClfsCode = "11";
                    break;
                case 4:
                    str_ClfsCode = "21";
                    break;
                case 5:
                    str_ClfsCode = "31";
                    break;
                case 6:
                    str_ClfsCode = "41";
                    break;
                case 7:
                    str_ClfsCode = "51";
                    break;
                default:
                    str_ClfsCode = "00";
                    break;
            }
            switch (p_eet_Element)
            {
                case enmElement.H:
                    str_ElementCode = "1";
                    break;
                case enmElement.A:
                    str_ElementCode = "2";
                    break;
                case enmElement.B:
                    str_ElementCode = "3";
                    break;
                case enmElement.C:
                    str_ElementCode = "4";
                    break;
                default:
                    str_ElementCode = "1";
                    break;
            }

            csGlys gly_Instance = new csGlys();
            gly_Instance.Load();
            string str_JD = gly_Instance.getJiaoDu(str_GlysName)[str_ClfsCode + str_ElementCode];
            Single sng_JD = Single.Parse(str_JD);
            if (str_GlysF.Equals("-"))
            {
                sng_JD += 180;
                sng_JD %= 360;
            }
            return sng_JD;
        }

        /// <summary>
        /// 计算角度 分相计算
        /// </summary>
        /// <param name="int_Clfs">测量方式</param>
        /// <param name="str_Glys">功率因数</param>
        /// <param name="bln_NXX">逆相序</param>
        /// <returns>返回数组，数组元素为各相ABC相电压电流角度</returns>
        private Single[] GetPhiGlys(int int_Clfs, string str_Glys, int int_Element, bool bln_NXX)
        {
            string str_CL = str_Glys.ToUpper().Substring(str_Glys.Length - 1, 1);
            Double dbl_XS = 0;
            if (str_CL == "C" || str_CL == "L")
                dbl_XS = Convert.ToDouble(str_Glys.Substring(0, str_Glys.Length - 1));
            else
                dbl_XS = Convert.ToDouble(str_Glys);
            Double dbl_Phase;

            if (int_Clfs == 1 || int_Clfs == 3 || int_Clfs == 6)
                dbl_Phase = Math.Asin(Math.Abs(dbl_XS));                              //无功计算
            else
                dbl_Phase = Math.Acos(Math.Abs(dbl_XS));                              //有功计算

            dbl_Phase = dbl_Phase * 180 / Math.PI;      //角度换算
            if (dbl_XS < 0) dbl_Phase = 180 + dbl_Phase;         //反向
            if (str_CL == "C") dbl_Phase = 360 - dbl_Phase;
            if (dbl_Phase < 0) dbl_Phase = 360 + dbl_Phase;

            Single sng_UIPhi = Convert.ToSingle(dbl_Phase);
            Single[] sng_Phi = new Single[6];

            if (bln_NXX)
            {
                sng_Phi[0] = 0;         //Ua
                sng_Phi[1] = 240;       //Ub
                sng_Phi[2] = 120;       //Uc
            }
            else
            {
                sng_Phi[0] = 0;         //Ua
                sng_Phi[1] = 120;       //Ub
                sng_Phi[2] = 240;       //Uc
            }


            sng_Phi[3] = sng_Phi[0] + sng_UIPhi;       //Ia
            sng_Phi[4] = sng_Phi[1] + sng_UIPhi;       //Ib
            sng_Phi[5] = sng_Phi[2] + sng_UIPhi;       //Ic

            if (int_Clfs == 2 || int_Clfs == 3)
            {
                sng_Phi[2] += 60;       //Uc
                sng_Phi[3] += 30;       //Ia
                sng_Phi[4] += 30;       //Ib
                sng_Phi[5] += 30;       //Ic
            }

            sng_Phi[3] %= 360;       //Ia
            sng_Phi[4] %= 360;       //Ib
            sng_Phi[5] %= 360;



            //0, 240, 120, 0, 240, 120
            //0, 240, 120, 180, 60, 300
            //0, 240, 120, 30, 270, 150
            //0, 240, 120, 210, 90, 330,

            return sng_Phi;
        }



        #endregion


        #region ------------继承接口成员函数\属性---------------------------

        /// <summary>
        /// 表位数
        /// </summary>
        public int BWCount
        {
            get { return this.m_int_BWCount; }
            set { this.m_int_BWCount = value; }
        }

        /// <summary>
        /// 对象是否正常,如果为false，则可从LostMessage中读取失败信息
        /// </summary>
        public bool Enabled
        {
            get { return this.m_bln_Enabled; }
        }


        /// <summary>
        /// 失败信息
        /// </summary>
        public string LostMessage
        {
            get { return this.m_str_LostMessage; }
        }

        /// <summary>
        /// 停止当前任务
        /// </summary>
        /// <returns></returns>
        public bool StopTask
        {
            get
            {
                return this.m_bln_StopTask;
            }
            set
            {
                lock (objStopTaskLock)
                {
                    //保证一次只能有一个线程修改锁
                    this.m_bln_StopTask = value;
                }
            }
        }

        /// <summary>
        /// 联机
        /// </summary>
        /// <returns></returns>
        public bool Link()
        {
            try
            {
                this.m_bln_LinkResult[0] = this.m_isr_StdMeter.link();
                if (!this.m_bln_LinkResult[0]) this.m_str_LostMessage = this.m_isr_StdMeter.LostMessage;

                this.m_bln_LinkResult[1] = this.m_ipr_Power.link();
                if (!this.m_bln_LinkResult[1]) this.m_str_LostMessage = this.m_ipr_Power.LostMessage;

                //this.m_bln_LinkResult[2] = LinkAllErrCalChannel(ref this.m_bln_ErrCalLinkResult);
                //if (!this.m_bln_LinkResult[2]) this.m_str_LostMessage = this.m_iee_ErrCal[0].LostMessage;
                bool[] bln_RevResult = new bool[m_int_BWCount];    //

                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                {
                    this.m_bln_LinkResult[2] = this.m_iee_ErrCal[int_Inc].Link(ref bln_RevResult);
                    if (!this.m_bln_LinkResult[2])
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        break;
                    }
                    else
                    {
                        this.m_str_LostMessage = string.Format("第{0}块误差板联机失败", int_Inc + 1);

                    }


                }


                this.m_bln_LinkResult[3] = this.m_ise_StdTime.Link();
                if (!this.m_bln_LinkResult[3]) this.m_str_LostMessage = this.m_ise_StdTime.LostMessage;

                return (this.m_bln_LinkResult[0] && this.m_bln_LinkResult[1] && this.m_bln_LinkResult[2] && this.m_bln_LinkResult[3]);

                //return (this.m_bln_LinkResult[0] && this.m_bln_LinkResult[1] && this.m_bln_LinkResult[2]);
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 联机
        /// </summary>
        /// <param name="IsCL191BCL188L">是否带有CL191B及CL188L，True有，false没有</param>
        /// <returns></returns>
        public bool Link(bool IsCL191BCL188L)
        {
            try
            {
                this.m_bln_LinkResult[0] = this.m_isr_StdMeter.link();
                if (!this.m_bln_LinkResult[0]) this.m_str_LostMessage = this.m_isr_StdMeter.LostMessage;

                this.m_bln_LinkResult[1] = this.m_ipr_Power.link();
                if (!this.m_bln_LinkResult[1]) this.m_str_LostMessage = this.m_ipr_Power.LostMessage;

                if (!IsCL191BCL188L) return (this.m_bln_LinkResult[0] && this.m_bln_LinkResult[1]);

                //this.m_bln_LinkResult[2] = LinkAllErrCalChannel(ref this.m_bln_ErrCalLinkResult);
                //if (!this.m_bln_LinkResult[2]) this.m_str_LostMessage = this.m_iee_ErrCal[0].LostMessage;
                bool[] bln_RevResult = new bool[m_int_BWCount];    //

                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                {
                    this.m_bln_LinkResult[2] = this.m_iee_ErrCal[int_Inc].Link(ref bln_RevResult);
                    if (!this.m_bln_LinkResult[2])
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        break;
                    }
                    else
                    {
                        this.m_str_LostMessage = string.Format("第{0}块误差板联机失败", int_Inc + 1);

                    }


                }


                this.m_bln_LinkResult[3] = this.m_ise_StdTime.Link();
                if (!this.m_bln_LinkResult[3]) this.m_str_LostMessage = this.m_ise_StdTime.LostMessage;

                return (this.m_bln_LinkResult[0] && this.m_bln_LinkResult[1] && this.m_bln_LinkResult[2] && this.m_bln_LinkResult[3]);

                //return (this.m_bln_LinkResult[0] && this.m_bln_LinkResult[1] && this.m_bln_LinkResult[2]);
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }


        /// <summary>
        /// 脱机
        /// </summary>
        /// <returns></returns>
        public bool Unlink()
        {
            this.m_bln_LinkResult[0] = false;
            this.m_bln_LinkResult[1] = false;
            this.m_bln_LinkResult[2] = false;
            this.m_bln_LinkResult[3] = false;
            return true;
        }


        #region IPower升源关源

        #region 三相
        /// <summary>
        /// 设置测试点
        /// </summary>
        /// <param name="int_Clfs">测量方式，0=PT4，1=QT4，2=PT3，3=QT3</param>
        /// <param name="sng_U">额定电压</param>
        /// <param name="sng_xUa">A相输出额定电压倍数</param>
        /// <param name="sng_xUb">B相输出额定电压倍数</param>
        /// <param name="sng_xUc">C相输出额定电压倍数</param>
        /// <param name="sng_I">额定电流</param>
        /// <param name="sng_Imax">最大电流</param>
        /// <param name="int_IType">输出电流基准类型，0=额定电流，1=最大电流</param>
        /// <param name="sng_xIa">A相输出电流（是I还是IMax倍数則根据int_IType决定）</param>
        /// <param name="sng_xIb">B相输出电流（是I还是IMax倍数則根据int_IType决定）</param>
        /// <param name="sng_xIc">C相输出电流（是I还是IMax倍数則根据int_IType决定）</param>
        /// <param name="int_Element">元件，0=合元，1=A元，2=B元，3=C元</param>
        /// <param name="str_Glys">功率因数，负数表示反向，正数据表示正向</param>
        /// <param name="sng_Freq">频率</param>
        /// <param name="bln_NXX">是否逆相序 true=逆相序,false=正相序</param>
        /// <returns></returns>
        public bool SetTestPoint(enmClfs ecs_Clfs, float sng_U, float sng_xUa, float sng_xUb, float sng_xUc,
            float sng_I, float sng_Imax, enmIType eit_IType, float sng_xIa, float sng_xIb, float sng_xIc, enmElement eet_Element,
            string str_Glys, float sng_Freq, bool bln_NXX)
        {

            try
            {
                long lng_Ticks = System.DateTime.Now.Ticks;
                bool bln_Result = false;

                byte byt_XWKG = 63;
                if (ecs_Clfs >= enmClfs.三相三线有功 && ecs_Clfs <= enmClfs.三元件跨相90) byt_XWKG &= 0x2D;   //三相三线 去掉B相

                if (eet_Element == enmElement.A)
                    byt_XWKG &= 0xf;                  //去掉BC相
                else if (eet_Element == enmElement.B)
                    byt_XWKG &= 0x17;                  //去掉AC相
                else if (eet_Element == enmElement.C)
                    byt_XWKG &= 0x27;                  //去掉AB相


                //---------------设置标准表参数---
                bln_Result = this.m_isr_StdMeter.SetAmMeterParameter(0x08,0x00,0x01);
                if (!bln_Result)
                {
                    this.m_str_LostMessage = "设置标准表参数失败！";
                    return false;
                }

                Single[] sng_In = new float[3];

                //如果输出三相电压相同并且（三相电流相同或不是合元输出）并且不是逆相序 则用35指令输出，是统调输出
                if ((sng_xUa == sng_xUc && sng_xUa == sng_xUb)
                        && ((sng_xIa == sng_xIc && sng_xIa == sng_xIb) || eet_Element != enmElement.H)
                        && !bln_NXX)
                {
                    if (eit_IType == enmIType.额定电流) //Ib倍数输出
                    {
                        sng_In[0] = sng_I * sng_xIa;
                        sng_In[1] = sng_I * sng_xIa;
                        sng_In[2] = sng_I * sng_xIa;
                    }
                    else                 //IMax倍数输出，
                    {
                        sng_In[0] = sng_Imax * sng_xIa;
                        sng_In[1] = sng_Imax * sng_xIa;
                        sng_In[2] = sng_Imax * sng_xIa;
                    }

                    //Single sng_TmpPhi = GetPhiGlys((int)ecs_Clfs, str_Glys);     //根据测试方式和功率因数计算角度
                    Single sng_TmpPhi = 0f;

                    if (eet_Element == enmElement.H)
                    {
                        //为了电量寄存器计算 所以在合元时我们通过计算得到功率因数
                        sng_TmpPhi = GetPhiGlys((int)ecs_Clfs, str_Glys);     //根据测试方式和功率因数计算角度
                    }
                    else
                    {
                        sng_TmpPhi = GetPhiGlys((int)ecs_Clfs, str_Glys, eet_Element);//根据测试方式、功率因数和合分元计算角度
                    }


                    bln_Result = this.m_ipr_Power.PowerOn(sng_U * sng_xUa, sng_In[0], sng_TmpPhi, sng_Freq, (int)ecs_Clfs,byt_XWKG);//, eet_Element
                }
                else                                    //特殊试验用
                { 
                    if (eit_IType == enmIType.额定电流) //Ib倍数输出
                    {
                        sng_In[0] = sng_I * sng_xIa;
                        sng_In[1] = sng_I * sng_xIb;
                        sng_In[2] = sng_I * sng_xIc;
                    }
                    else                //IMax倍数输出，
                    {
                        sng_In[0] = sng_Imax * sng_xIa;
                        sng_In[1] = sng_Imax * sng_xIb;
                        sng_In[2] = sng_Imax * sng_xIc;
                    }
                    Single sng_TmpPhi = 0f;

                    if (eet_Element == enmElement.H)
                    {
                        //为了电量寄存器计算 所以在合元时我们通过计算得到功率因数
                        sng_TmpPhi = GetPhiGlys((int)ecs_Clfs, str_Glys);     //根据测试方式和功率因数计算角度
                    }
                    else
                    {
                        sng_TmpPhi = GetPhiGlys((int)ecs_Clfs, str_Glys, eet_Element);//根据测试方式、功率因数和合分元计算角度
                    }

                    Single[] sng_PhiXX = GetPhiGlys((int)ecs_Clfs, str_Glys, (int)eet_Element, bln_NXX); //根据测试方式、功率因数、元件、相序计算三相电压电流角度
                    bln_Result = this.m_ipr_Power.PowerOn(sng_U * sng_xUa, sng_U * sng_xUb, sng_U * sng_xUc, sng_In[0],
                                                          sng_In[1], sng_In[2], sng_PhiXX[0], sng_PhiXX[1], sng_PhiXX[2],
                                                          sng_PhiXX[3], sng_PhiXX[4], sng_PhiXX[5], sng_Freq, (int)ecs_Clfs,
                                                          byt_XWKG);
                }
                sng_In.CopyTo(sng_CurI,0);
                sng_CurU[0] = sng_U * sng_xUa;
                sng_CurU[1] = sng_U * sng_xUb;
                sng_CurU[2] = sng_U * sng_xUc;

                this.m_str_LostMessage = this.m_ipr_Power.LostMessage;
                return bln_Result;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }

        /// <summary>
        /// 设置测试点
        /// </summary>
        /// <param name="int_Clfs">测量方式，0=PT4，1=PT3，2=QT4，3=QT3</param>
        /// <param name="sng_U">额定电压</param>
        /// <param name="sng_xU">输出额定电压倍数</param>
        /// <param name="sng_I">额定电流</param>
        /// <param name="sng_Imax">最大电流</param>
        /// <param name="int_IType">输出电流基准类型，0=额定电流，1=最大电流</param>
        /// <param name="sng_xI">输出电流（是I还是IMax倍数則根据int_IType决定）</param>
        /// <param name="int_Element">元件，0=合元，1=A元，2=B元，3=C元</param>
        /// <param name="str_Glys">功率因数，负数表示反向，正数据表示正向</param>
        /// <param name="sng_Freq">频率</param>
        /// <param name="bln_NXX">是否逆相序 true=逆相序,false=正相序</param>
        /// <returns></returns>
        public bool SetTestPoint(enmClfs ecs_Clfs, Single sng_U, Single sng_xU, Single sng_I, Single sng_Imax,
                          enmIType eit_IType, Single sng_xI, enmElement eet_Element, string str_Glys, Single sng_Freq, bool bln_NXX)
        {
            try
            {
                return SetTestPoint(ecs_Clfs, sng_U, sng_xU, sng_xU, sng_xU, sng_I, sng_Imax, eit_IType, sng_xI, sng_xI,
                                    sng_xI, eet_Element, str_Glys, sng_Freq, bln_NXX);

            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 设置测试点(三相不平衡电压电流、任意角度)
        /// </summary>
        /// <param name="int_Clfs">测量方式，0=PT4，1=PT3，2=QT4，3=QT3</param>
        /// <param name="sng_U">额定电压</param>
        /// <param name="sng_xUa">A相输出额定电压倍数</param>
        /// <param name="sng_xUb">B相输出额定电压倍数</param>
        /// <param name="sng_xUc">C相输出额定电压倍数</param>
        /// <param name="sng_I">额定电流</param>
        /// <param name="sng_Imax">最大电流</param>
        /// <param name="int_IType">输出电流基准类型，0=额定电流，1=最大电流</param>
        /// <param name="sng_xIa">A相输出电流（是I还是IMax倍数則根据int_IType决定）</param>
        /// <param name="sng_xIb">B相输出电流（是I还是IMax倍数則根据int_IType决定）</param>
        /// <param name="sng_xIc">C相输出电流（是I还是IMax倍数則根据int_IType决定）</param>
        /// <param name="int_Element">元件，0=合元，1=A元，2=B元，3=C元</param>
        /// <param name="sng_UaPhi">A相电压角度</param>
        /// <param name="sng_UbPhi">B相电压角度</param>
        /// <param name="sng_UcPhi">C相电压角度</param>
        /// <param name="sng_IaPhi">A相电流角度</param>
        /// <param name="sng_IbPhi">B相电流角度</param>
        /// <param name="sng_IcPhi">C相电流角度</param>
        /// <param name="sng_Freq">频率</param>
        /// <returns></returns>
        public bool SetTestPoint(enmClfs ecs_Clfs, Single sng_U, Single sng_xUa, Single sng_xUb, Single sng_xUc,
                          Single sng_I, Single sng_Imax, enmIType eit_IType, Single sng_xIa, Single sng_xIb,
                          Single sng_xIc, enmElement eet_Element, Single sng_UaPhi, Single sng_UbPhi, Single sng_UcPhi,
                           Single sng_IaPhi, Single sng_IbPhi, Single sng_IcPhi, Single sng_Freq)
        {
            try
            {

                bool bln_Result = false;
                byte byt_XWKG = 63;
                if (ecs_Clfs >= enmClfs.三相三线有功 && ecs_Clfs <= enmClfs.三元件跨相90) byt_XWKG &= 0x2D;   //三相三线 去掉B相
                if (eet_Element == enmElement.A)
                    byt_XWKG &= 0xf;                  //去掉BC相
                else if (eet_Element == enmElement.B)
                    byt_XWKG &= 0x17;                  //去掉AC相
                else if (eet_Element == enmElement.C)
                    byt_XWKG &= 0x27;                  //去掉AB相

                //---------------设置标准表参数---
                bln_Result = this.m_isr_StdMeter.SetAmMeterParameter(0x08, 0x00, 0x01);
                if (!bln_Result)
                {
                    this.m_str_LostMessage = "设置标准表参数失败！";
                    return false;
                }

                Single[] sng_In = new float[3];


                if (eit_IType == enmIType.额定电流) //Ib倍数输出
                {
                    sng_In[0] = sng_I * sng_xIa;
                    sng_In[1] = sng_I * sng_xIa;
                    sng_In[2] = sng_I * sng_xIa;
                }
                else                 //IMax倍数输出，
                {
                    sng_In[0] = sng_Imax * sng_xIa;
                    sng_In[1] = sng_Imax * sng_xIa;
                    sng_In[2] = sng_Imax * sng_xIa;
                }


                bln_Result = this.m_ipr_Power.PowerOn(sng_U * sng_xUa, sng_U * sng_xUb, sng_U * sng_xUc, sng_In[0],
                                                      sng_In[1], sng_In[2], sng_UaPhi, sng_UbPhi, sng_UcPhi,
                                                      sng_IaPhi, sng_IbPhi, sng_IcPhi, sng_Freq, (int)ecs_Clfs,
                                                      byt_XWKG);

                sng_In.CopyTo(sng_CurI, 0);
                sng_CurU[0] = sng_U * sng_xUa;
                sng_CurU[1] = sng_U * sng_xUb;
                sng_CurU[2] = sng_U * sng_xUc;

                this.m_str_LostMessage = this.m_ipr_Power.LostMessage;
                return bln_Result;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }




        }

        /// <summary>
        /// 设置测试点(三相统一电压电流、任意角度)
        /// </summary>
        /// <param name="int_Clfs">测量方式，0=PT4，1=PT3，2=QT4，3=QT3</param>
        /// <param name="sng_U">额定电压</param>
        /// <param name="sng_xU">输出额定电压倍数</param>
        /// <param name="sng_I">额定电流</param>
        /// <param name="sng_Imax">最大电流</param>
        /// <param name="int_IType">输出电流基准类型，0=额定电流，1=最大电流</param>
        /// <param name="sng_xI">输出电流（是I还是IMax倍数則根据int_IType决定）</param>
        /// <param name="int_Element">元件，0=合元，1=A元，2=B元，3=C元</param>
        /// <param name="sng_UaPhi">A相电压角度</param>
        /// <param name="sng_UbPhi">B相电压角度</param>
        /// <param name="sng_UcPhi">C相电压角度</param>
        /// <param name="sng_IaPhi">A相电流角度</param>
        /// <param name="sng_IbPhi">B相电流角度</param>
        /// <param name="sng_IcPhi">C相电流角度</param>
        /// <param name="sng_Freq">频率</param>
        /// <returns></returns>
        public bool SetTestPoint(enmClfs ecs_Clfs, Single sng_U, Single sng_xU, Single sng_I, Single sng_Imax,
                          enmIType eit_IType, Single sng_xI, enmElement eet_Element, Single sng_UaPhi, Single sng_UbPhi, Single sng_UcPhi,
                           Single sng_IaPhi, Single sng_IbPhi, Single sng_IcPhi, Single sng_Freq)
        {

            try
            {
                return SetTestPoint(ecs_Clfs, sng_U, sng_xU, sng_xU, sng_xU, sng_I, sng_Imax, eit_IType, sng_xI, sng_xI,
                                    sng_xI, eet_Element, sng_UaPhi, sng_UbPhi, sng_UcPhi, sng_IaPhi, sng_IbPhi, sng_IcPhi, sng_Freq);

            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }
        #endregion

        #region 单相
        /// <summary>
        /// 设置测试点
        /// </summary>
        /// <param name="int_Clfs">测量方式</param>
        /// <param name="sng_U">输出电压</param>
        /// <param name="sng_I">输出电流</param>
        /// <param name="eet_Element">元件</param>
        /// <param name="str_Glys">功率因数，负数表示反向，正数据表示正向</param>
        /// <param name="sng_Freq">频率</param>
        /// <returns></returns>
        public bool SetTestPoint(enmClfs ecs_Clfs, Single sng_U, Single sng_I, enmElement eet_Element, string str_Glys, Single sng_Freq)
        {

            try
            {
                bool bln_Result = false;

                bln_Result = this.m_isr_StdMeter.SetAmMeterParameter(0x08, 0x00, 0x00);
                if (!bln_Result)
                {
                    this.m_str_LostMessage = "设置标准表参数失败！";
                    return false;
                }

                bln_Result = this.m_ipr_Power.PowerOn(sng_U , sng_I,str_Glys, sng_Freq, (int)ecs_Clfs, eet_Element);

                sng_CurI[0] = sng_I;
                sng_CurI[1] = sng_I;
                sng_CurI[2] = sng_I;
                sng_CurU[0] = sng_U;
                sng_CurU[1] = sng_U;
                sng_CurU[2] = sng_U;

                this.m_str_LostMessage = this.m_ipr_Power.LostMessage;
                return bln_Result;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 关源
        /// </summary>
        /// <returns></returns>
        public bool PowerOff()
        {
            try
            {
                bool bln_Result = this.m_ipr_Power.PowerOff();
                this.m_str_LostMessage = this.m_ipr_Power.LostMessage;
                return bln_Result;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }
        #endregion

        #region 设置CL1115、CL188L电能误差检定
        /// <summary>
        /// 设置电能误差参数
        /// </summary>
        /// <param name="ett_TaskType">任务类型</param>
        /// <param name="ect_ChannelNo">通道</param>
        /// <param name="ept_PulseType">脉冲接口</param>
        /// <param name="egt_GyG">共阴共阳</param>
        /// <param name="lng_AmConst">脉冲常数</param>
        /// <param name="lng_PulseTimes">圈数</param>
        /// <param name="iAmMeterPulseBS">倍数</param>
        /// <returns></returns>
        public bool SetTaskParameter(enmPulseDzType epd_DzType, enmTaskType ett_TaskType, enmChannelType ect_ChannelNo, enmPulseComType ept_PulseType, enmGyGyType egt_GyGy, long lng_AmConst, long lng_PulseTimes, byte iAmMeterPulseBS)
        {
            try
            {
                bool bAuto = true ;//自动档，手动档
                long lng_StdConst = 80000000;//脉冲常数

                bool bln_Result = false;

                #region 0.停止误差板
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //由于只发一条指令，则循环下发即可
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].StopCalculate(0x00);//(int)ett_TaskType
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        return false;
                    }
                }

                #endregion

                #region 1.设置191为电能脉冲模式
                bln_Result=this.m_ise_StdTime.SetChannel(1);
                if (!bln_Result) bln_Result = this.m_ise_StdTime.SetChannel(1);
                if (!bln_Result)
                {
                    this.m_str_LostMessage = this.m_ise_StdTime.LostMessage;
                    return false;
                }

                #endregion

                #region 2.获取标准表功率，用来计算标准表脉冲频率，但经测试这个参数好像没有作用，暂注销
                string[] s = new string[50];
                bln_Result = this.m_isr_StdMeter.ReadStdMeterInfo(ref s);
                if (!bln_Result)
                {
                    this.m_str_LostMessage = this.m_isr_StdMeter.LostMessage;
                    return false;
                }
                Single sng_StdP = Convert.ToSingle(s[22]);
                #endregion


                #region 3.获取标准表脉冲常数
                if (bAuto)//如果是自动档，读取标准表脉冲常数
                {
                    bln_Result = this.m_isr_StdMeter.SetStdMeterUclinemode(0x08);
                    if (!bln_Result) bln_Result = this.m_isr_StdMeter.SetStdMeterUclinemode(0x08);
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_isr_StdMeter.LostMessage;
                        return false;
                    }

                    bln_Result = this.m_isr_StdMeter.ReadStdMeterConst(ref lng_StdConst);
                    if (!bln_Result) bln_Result = this.m_isr_StdMeter.ReadStdMeterConst(ref lng_StdConst);
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_isr_StdMeter.LostMessage;
                        return false;
                    }

                    #region 循环查询标准表常数是否正确
                    //sth_SpaceTicker.Reset();
                    //sth_SpaceTicker.Start();
                    //long _lng_StdConst_Select = this.m_isr_StdMeter.SelectStdMeterConst(Convert.ToSingle(s[3]));//按表查询
                    //while (true)
                    //{
                    //    if (Convert.ToInt32(sth_SpaceTicker.ElapsedMilliseconds / 1000) > 10)//设置最长10秒
                    //    {
                    //        bln_Result = false;//设置为失败
                    //        break;//退出
                    //    }
                    //    bln_Result = this.m_isr_StdMeter.ReadStdMeterConst(ref lng_StdConst);
                    //    if (!bln_Result)
                    //    {
                    //        if (_lng_StdConst_Select == lng_StdConst) break;
                    //    }
                    //    else
                    //    {
                    //        continue;
                    //    }

                    //}
                    #endregion

                }
                else//如果是手动档位，则设置标准表常数SetStdMeterConst
                {
                    bln_Result = this.m_isr_StdMeter.SetStdMeterUclinemode(0x88);
                    bln_Result = this.m_isr_StdMeter.SetAmMeterParameter(0x88, 0x00, 0x00);
                    bln_Result = this.m_isr_StdMeter.SetStdMeterConst(lng_StdConst, true);
                }
                if (!bln_Result)
                {
                    this.m_str_LostMessage = this.m_isr_StdMeter.LostMessage;
                    return false;
                }
                #endregion


                #region 4.设置脉冲端子类型SetMeterPulseDzType：端子类型
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //由于只发一条指令，则循环下发即可
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].SetMeterPulseDzType((int)epd_DzType);
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        return false;
                    }
                }
                #endregion


                #region 5.设置选择脉冲通道SelectPulseChannel：通道号,脉冲接口，脉冲类型
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //由于只发一条指令，则循环下发即可
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].SelectPulseChannel(255, (int)ect_ChannelNo, (int)ept_PulseType, (int)egt_GyGy);
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        return false;
                    }
                }
                #endregion

                #region 6.设置被检表参数到误差板参数SetAmMeterPara：被检表的常数和圈数

                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //由于只发一条指令，则循环下发即可
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].SetDnWcrPara(lng_AmConst, lng_PulseTimes, lng_StdConst, sng_StdP, iAmMeterPulseBS);
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        return false;
                    }
                }
                #endregion

                #region 7.启动误差板
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //由于只发一条指令，则循环下发即可
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].StartCalculate(0x00);
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        return false;
                    }
                }
                #endregion

                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }
        #endregion

        #region 设置CL1115、CL188L日计时误差检定
        /// <summary>
        /// 设置日计时误差参数
        /// </summary>
        /// <param name="ett_TaskType">任务类型</param>
        /// <param name="ect_ChannelNo">通道</param>
        /// <param name="ept_PulseType">脉冲接口</param>
        /// <param name="egt_GyG">共阴共阳</param>
        /// <param name="sng_TimePL">时钟频率</param>
        /// <param name="lng_PulseTimes">圈数</param>
        /// <returns></returns>
        public bool SetTaskParameter(enmPulseDzType epd_DzType, enmTaskType ett_TaskType, enmChannelType ect_ChannelNo, enmPulseComType ept_PulseType, enmGyGyType egt_GyGy, float sng_TimePL, int lng_PulseTimes)
        {

            try
            {
                bool bln_Result = false;
                #region 停止误差板
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //由于只发一条指令，则循环下发即可
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].StopCalculate(0x02);//(int)ett_TaskType
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        return false;
                    }
                }

                #endregion

                #region 1.设置191为电能脉冲模式
                this.m_ise_StdTime.SetChannel(0);
                if (!bln_Result)
                {
                    this.m_str_LostMessage = this.m_ise_StdTime.LostMessage;
                    return false;
                }

                #endregion

                #region 2.设置脉冲端子类型SetMeterPulseDzType：端子类型
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //由于只发一条指令，则循环下发即可
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].SetMeterPulseDzType((int)epd_DzType);
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        return false;
                    }
                }
                #endregion


                #region 3.设置选择脉冲通道SelectPulseChannel：通道号,脉冲接口，脉冲类型
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //由于只发一条指令，则循环下发即可
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].SelectPulseChannel(255, (int)ect_ChannelNo, (int)ept_PulseType, (int)egt_GyGy);
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        return false;
                    }
                }
                #endregion


                #region 4.设置被检表参数到误差板参数SetAmMeterPara：被检表的常数和圈数
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //由于只发一条指令，则循环下发即可
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].SetTimePara(sng_TimePL, lng_PulseTimes);
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        return false;
                    }
                }
                #endregion


                #region 5.启动误差板
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //由于只发一条指令，则循环下发即可
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].StartCalculate(0x02);
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        return false;
                    }
                }
                #endregion

                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 设置电流回路
        /// </summary>
        /// <param name="int_LoopType"></param>
        /// <returns></returns>
        public bool SetCurrentLoop(int int_LoopType)
        {
            try
            {
                bool bln_Result = false;
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].SetDLSwitch(int_LoopType);
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        break;
                    }
                }
                return bln_Result;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 设置被检表脉冲端子类型：0=国网端子，1=南网端子
        /// </summary>
        /// <param name="iPulseDzType">0=国网端子，1=南网端子</param>
        /// <returns></returns>
        public bool SetMeterPulseDzType(int iPulseDzType)
        {
            try
            {
                bool bln_Result = false;
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].SetMeterPulseDzType(iPulseDzType);
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        break;
                    }
                }
                return bln_Result;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }


        /// <summary>
        /// 设置各表位的通信开关
        /// </summary>
        /// <param name="int_NO">位表号(1-200)，特殊号：0xFF(255)=广播地址，0xEE(238)=偶数地址，0xDD(221)=奇数地址</param>
        /// <param name="bln_Open">是否打开，ture=打开，false=关闭</param>
        /// <returns></returns>
        public bool SetAmmeterCmmSwitch(int int_NO, bool bln_Open)
        {
            try
            {
                bool bln_Result = false;
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].SetCommSwitch(int_NO, bln_Open);
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        break;
                    }
                }
                return bln_Result;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }

        /// <summary>
        /// 读取标准数据信息
        /// </summary>
        /// <param name="str_Value">返回标准数据信息</param>
        /// <returns></returns>
        public bool ReadStdInfo(ref string[] str_Value)
        {
            try
            {
                bool bln_Result = this.m_isr_StdMeter.ReadStdMeterInfo(ref str_Value);
                this.m_str_LostMessage = this.m_isr_StdMeter.LostMessage;
                return bln_Result;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }


        /// <summary>
        /// 读取任务检定数据，数据内容根据所设置任务类型不同而不同
        /// </summary>
        /// <param name="int_FunNo">功能数据缓冲区号，误差类型包括：电能误差（00）、需量周期误差（01）、日计时误差（02）、脉冲个数（03）、对标状态（04）</param>
        /// <param name="str_Data"></param>
        /// <param name="int_Times"></param>
        /// <returns></returns>
        public bool ReadTaskData(int byt_TaskType, ref bool[] bln_Result, ref string[] str_Data, ref int[] int_Times)
        {

            try
            {
                bool bln_Return = false;
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                {
                    bln_Return = this.m_iee_ErrCal[int_Inc].ReadData(byt_TaskType, ref bln_Result, ref int_Times, ref str_Data);
                    if (!bln_Return)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        break;
                    }
                }
                return bln_Return;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }

        /// <summary>
        /// 设置表标准表档位
        /// </summary>
        /// <param name="sngUn">电压V</param>
        /// <param name="sngIn">电流A</param>
        /// <returns></returns>
        public bool SetUIScale(float sngUn,float sngIn)
        {
            try
            {
                bool bln_Result = this.m_isr_StdMeter.SetUIScale(sngIn, sngIn, sngIn, sngUn, sngUn, sngUn);
                this.m_str_LostMessage = this.m_isr_StdMeter.LostMessage;
                return bln_Result;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 停止当前误差板计算任务
        /// </summary>
        /// <param name="elt_Type">停止 = 0,启动 = 1</param>
        /// <returns></returns>
        public bool StopCalculate(enmTaskType ett_TaskType)
        {
            try
            {
                bool bln_Result = false;

                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].StopCalculate((int)ett_TaskType);
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        break;
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

        /// <summary>
        /// 设置CL191通道
        /// </summary>
        /// <param name="iType">0=标准时钟脉冲、1=标准电能脉冲</param>
        /// <returns></returns>
        public bool SetStdTimeChannel(int iType)
        {
            bool bln_Result = this.m_ise_StdTime.SetChannel(iType);
            if (!bln_Result)
            {
                this.m_str_LostMessage = this.m_ise_StdTime.LostMessage;
                return false;
            }
            else
            {
                return true;
            }

        }

        /*作废

        #region IStdMeter
        /// <summary>
        /// 5.	设置电能参数 81 30 PCID 0e a3 00 09 20 uclinemode 11 usE1type ucE1switch CS
        /// </summary>
        /// <param name="Uclinemode">接线方式：CL1115：自动量程：08H 手动量程：88H</param>
        /// <param name="UsE1type">电能指示：CL1115：总有功电能00H    CL3115：总有功电能00H  总无功电能40H  </param>
        /// <param name="UcE1switch">启动指令： 0：停止计算  1：开始计算电能误差  2：开始计算电能走字</param>
        /// <returns></returns>
        public bool SetAmMeterParameter(byte Uclinemode, byte UsE1type, byte UcE1switch)
        {
            try
            {
                bool bln_Result = this.m_isr_StdMeter.SetAmMeterParameter(Uclinemode, UsE1type, UcE1switch);
                this.m_str_LostMessage = this.m_isr_StdMeter.LostMessage;
                return bln_Result;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }

        /// <summary>
        /// 2.	设置本机常数 81 30 PCID 0d a3 00 04 01 uiLocalnum CS
        /// </summary>
        /// <param name="lStdConst">标准表常数</param>
        /// <returns></returns>
        public bool SetStdMeterConst(long lStdConst)
        {
            try
            {
                bool bln_Result = this.m_isr_StdMeter.SetStdMeterConst(lStdConst, true);
                this.m_str_LostMessage = this.m_isr_StdMeter.LostMessage;
                return bln_Result;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }

        /// <summary>
        /// 读标准常数
        /// </summary>
        /// <param name="lng_StdConst">标准常数</param>
        /// <returns></returns>
        public bool ReadStdConst(ref long lng_StdConst)
        {
            try
            {
                bool bln_Result = this.m_isr_StdMeter.ReadStdMeterConst(ref lng_StdConst);
                this.m_str_LostMessage = this.m_isr_StdMeter.LostMessage;
                return bln_Result;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }


        #endregion

        #region IStdTime
        ///// <summary>
        ///// 读GPS日期时间
        ///// </summary>
        ///// <param name="str_DateTime">日期时间,注：格式为yyyy-mm-dd hh:mm:ss</param>
        ///// <returns></returns>
        //public bool ReadGPSDateTime(ref string str_DateTime)
        //{
        //    try
        //    {
        //        bool bln_Result = this.m_ise_StdTime.ReadGPSTime(ref str_DateTime);

        //        //如果时间为空，则读取系统时间。
        //        if (string.IsNullOrEmpty(str_DateTime))
        //        {
        //            str_DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //        }
        //        this.m_str_LostMessage = this.m_ise_StdTime.LostMessage;
        //        return bln_Result;
        //    }
        //    catch (Exception e)
        //    {
        //        this.m_str_LostMessage = e.ToString();
        //        return false;
        //    }
        //}

        ///// <summary>
        ///// 读取温度湿度
        ///// </summary>
        ///// <param name="sng_Temp">返回温度值</param>
        ///// <param name="sng_Hum">返回湿度值</param>
        ///// <returns></returns>
        //public bool ReadTempHum(ref float sng_Temp, ref float sng_Hum)
        //{
        //    try
        //    {
        //        bool bln_Result = this.m_ise_StdTime.ReadTempHum(ref sng_Temp, ref sng_Hum);
        //        this.m_str_LostMessage = this.m_ise_StdTime.LostMessage;
        //        return bln_Result;
        //    }
        //    catch (Exception e)
        //    {
        //        this.m_str_LostMessage = e.ToString();
        //        return false;
        //    }



        //}
        #endregion


        #region IErrorCalculate
        #region 选择脉冲通道
        /// <summary>
        /// 选择脉冲通道
        /// </summary>
        /// <param name="ect_ChannelNo">通道号</param>
        /// <param name="ept_PulseType">脉冲接口</param>
        /// <param name="egt_GyGy">脉冲类型</param>
        /// <returns></returns>
        public bool SelectPulseChannel(enmChannelType[] ect_ChannelNo, enmPulseComType[] ept_PulseType, enmGyGyType[] egt_GyGy)
        {
            try
            {
                Thread[] thd_Thread = new Thread[this.m_iee_ErrCal.Length];
                sct_Parameter[] sct_Para = new sct_Parameter[this.m_iee_ErrCal.Length];
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                {
                    sct_Para[int_Inc] = new sct_Parameter();
                    sct_Para[int_Inc].int_Arry1 = new int[this.m_int_BWCount];
                    sct_Para[int_Inc].int_Arry2 = new int[this.m_int_BWCount];
                    sct_Para[int_Inc].int_Arry3 = new int[this.m_int_BWCount];

                    for (int int_Inb = 0; int_Inb < this.m_int_BWCount; int_Inb++)
                    {
                        sct_Para[int_Inc].int_Arry1[int_Inb] = (int)ect_ChannelNo[int_Inb];
                        if (((int)ect_ChannelNo[int_Inb]) == 5 || ((int)ect_ChannelNo[int_Inb]) == 6)
                        {
                            sct_Para[int_Inc].int_Arry2[int_Inb] = 1;
                            sct_Para[int_Inc].int_Arry3[int_Inb] = 0;
                        }
                        else
                        {
                            sct_Para[int_Inc].int_Arry2[int_Inb] = (int)ept_PulseType[int_Inb];
                            sct_Para[int_Inc].int_Arry3[int_Inb] = (int)egt_GyGy[int_Inb];
                        }

                    }

                    sct_Para[int_Inc].int_ListNo = int_Inc;
                    thd_Thread[int_Inc] = new Thread(new ParameterizedThreadStart(this.ThreadSelectPulseChannel));
                    thd_Thread[int_Inc].IsBackground = true;
                }

                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //启动多线操作
                    thd_Thread[int_Inc].Start(sct_Para[int_Inc]);

                bool bln_AllOK = false;
                long lng_Ticks = System.DateTime.Now.Ticks;
                while (!bln_AllOK)
                {
                    System.Windows.Forms.Application.DoEvents();
                    bln_AllOK = true;
                    for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                        if (!sct_Para[int_Inc].bln_OptResult) bln_AllOK = false;            //还有没有操作完成
                    System.Threading.Thread.Sleep(5);
                    if (System.DateTime.Now.Ticks - lng_Ticks >= 10000 * this.m_int_BWCount * 300 + 500)        //
                        break;
                }
                bln_AllOK = true;
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                    if (!sct_Para[int_Inc].bln_Result)
                    {
                        bln_AllOK = false;
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        break;

                    }
                return bln_AllOK;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 统一选择脉冲通道
        /// </summary>
        /// <param name="ect_ChannelNo">通道号</param>
        /// <param name="ept_PulseType">脉冲接口</param>
        /// <param name="egt_GyGy">脉冲类型</param>
        /// <returns></returns>
        public bool SelectPulseChannel(enmChannelType ect_ChannelNo, enmPulseComType ept_PulseType, enmGyGyType egt_GyGy)
        {
            try
            {
                bool bln_Result = false;
                //当同时使用脉冲盒和光电头时，多功能一定是脉冲盒
                if (((int)ect_ChannelNo) == 5 || ((int)ect_ChannelNo) == 6)
                {
                    ept_PulseType = (enmPulseComType)1;
                    egt_GyGy = (enmGyGyType)0;
                }
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //由于只发一条指令，则循环下发即可
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].SelectPulseChannel(255, (int)ect_ChannelNo, (int)ept_PulseType, (int)egt_GyGy);
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        break;
                    }
                }
                return bln_Result;

            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 按表位设置脉冲通道
        /// </summary>
        /// <param name="int_BwNo">表位号</param>
        /// <param name="ect_ChannelNo">通道号</param>
        /// <param name="ept_PulseType">脉冲接口</param>
        /// <param name="egt_GyGy">脉冲类型</param>
        /// <returns></returns>
        public bool SelectPulseChannel(int int_BwNo, enmChannelType ect_ChannelNo, enmPulseComType ept_PulseType, enmGyGyType egt_GyGy)
        {
            try
            {
                bool bln_Result = false;
                //当同时使用脉冲盒和光电头时，多功能一定是脉冲盒
                if (((int)ect_ChannelNo) == 5 || ((int)ect_ChannelNo) == 6)
                {
                    ept_PulseType = (enmPulseComType)1;
                    egt_GyGy = (enmGyGyType)0;
                }
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //由于只发一条指令，则循环下发即可
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].SelectPulseChannel(int_BwNo, (int)ect_ChannelNo, (int)ept_PulseType, (int)egt_GyGy);
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        break;
                    }
                }
                return bln_Result;

            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }
        #endregion

        /// <summary>
        /// 设置被检表的常数和圈数
        /// </summary>
        /// <param name="lng_AmConst">电表常数</param>
        /// <param name="lng_PulseTimes">圈数</param>
        /// <returns></returns>
        public bool SetDnWcParameter(long[] lng_AmConst, long[] lng_PulseTimes, long lStdPulseConst, float fStrandMeterP, byte iAmMeterPulseBS)
        {
            try
            {
                this.m_lng_AmConst = lng_AmConst[0];
                this.m_lng_AmPulse = lng_PulseTimes[0];
                if (this.m_iee_ErrCal.Length > 1)           //多路控制误差板操作
                {
                    Thread[] thd_Thread = new Thread[this.m_iee_ErrCal.Length];
                    sct_Parameter[] sct_Para = new sct_Parameter[this.m_iee_ErrCal.Length];
                    for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                    {
                        sct_Para[int_Inc] = new sct_Parameter();
                        sct_Para[int_Inc].lng_Arry1 = lng_AmConst;          //参数
                        sct_Para[int_Inc].lng_Arry2 = lng_PulseTimes;
                        sct_Para[int_Inc].int_ListNo = int_Inc;
                        thd_Thread[int_Inc] = new Thread(new ParameterizedThreadStart(this.ThreadSetAmMeterParameter));
                        thd_Thread[int_Inc].IsBackground = true;
                    }

                    for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //启动多线操作
                        thd_Thread[int_Inc].Start(sct_Para[int_Inc]);

                    bool bln_AllOK = false;
                    long lng_Ticks = System.DateTime.Now.Ticks;
                    while (!bln_AllOK)
                    {
                        System.Windows.Forms.Application.DoEvents();
                        bln_AllOK = true;
                        for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                            if (!sct_Para[int_Inc].bln_OptResult) bln_AllOK = false;            //还有没有操作完成
                        System.Threading.Thread.Sleep(5);
                        if (System.DateTime.Now.Ticks - lng_Ticks >= 10000 * this.m_int_BWCount * 200 + 500)        //
                            break;
                    }
                    bln_AllOK = true;
                    for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                        if (!sct_Para[int_Inc].bln_Result)
                        {
                            this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                            bln_AllOK = false;
                            break;
                        }
                    return bln_AllOK;
                }
                else
                {
                    bool bln_Result = this.m_iee_ErrCal[0].SetDnWcrPara(lng_AmConst[0], lng_PulseTimes[0], lStdPulseConst, fStrandMeterP, iAmMeterPulseBS);
                    this.m_str_LostMessage = this.m_iee_ErrCal[0].LostMessage;
                    return bln_Result;
                }
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 统一设置被检表常数和圈数
        /// </summary>
        /// <param name="lng_AmConst">电表常数</param>
        /// <param name="lng_PulseTimes">圈数</param>
        /// <returns></returns>
        public bool SetDnWcParameter(long lng_AmConst, long lng_PulseTimes, long lStdPulseConst, float fStrandMeterP, byte iAmMeterPulseBS)
        {
            try
            {
                this.m_lng_AmConst = lng_AmConst;
                this.m_lng_AmPulse = lng_PulseTimes;

                bool bln_Result = false;
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].SetDnWcrPara(lng_AmConst, lng_PulseTimes, lStdPulseConst, fStrandMeterP, iAmMeterPulseBS);
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        break;
                    }
                }

                return bln_Result;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }





        /// <summary>
        /// 设置日计时参数
        /// </summary>
        /// <param name="sng_TimePL">被检表时钟频率</param>
        /// <param name="int_Times">检定圈数</param>
        /// <returns></returns>
        public bool SetRjsParameter(float[] sng_TimePL, int[] int_Times)
        {
            try
            {
                Thread[] thd_Thread = new Thread[this.m_iee_ErrCal.Length];
                sct_Parameter[] sct_Para = new sct_Parameter[this.m_iee_ErrCal.Length];
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                {
                    sct_Para[int_Inc] = new sct_Parameter();
                    sct_Para[int_Inc].int_Arry1 = int_Times;
                    sct_Para[int_Inc].flt_Arry1 = sng_TimePL;
                    sct_Para[int_Inc].int_ListNo = int_Inc;
                    thd_Thread[int_Inc] = new Thread(new ParameterizedThreadStart(this.ThreadSetRjsParameter));
                    thd_Thread[int_Inc].IsBackground = true;
                }

                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //启动多线操作
                    thd_Thread[int_Inc].Start(sct_Para[int_Inc]);

                bool bln_AllOK = false;
                long lng_Ticks = System.DateTime.Now.Ticks;
                while (!bln_AllOK)
                {
                    System.Windows.Forms.Application.DoEvents();
                    bln_AllOK = true;
                    for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                        if (!sct_Para[int_Inc].bln_OptResult) bln_AllOK = false;            //还有没有操作完成
                    System.Threading.Thread.Sleep(5);
                    if (System.DateTime.Now.Ticks - lng_Ticks >= 10000 * this.m_int_BWCount * 300 + 500)        //
                        break;
                }
                bln_AllOK = true;
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                    if (!sct_Para[int_Inc].bln_Result)
                    {
                        bln_AllOK = false;
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        break;
                    }
                return bln_AllOK;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 统一设置日计时参数
        /// </summary>
        /// <param name="sng_TimePL">被检表时钟频率</param>
        /// <param name="int_Times">检定圈数</param>
        /// <returns></returns>
        public bool SetRjsParameter(float sng_TimePL, int int_Times)
        {
            try
            {
                bool bln_Result = false;
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].SetTimePara(sng_TimePL,int_Times);
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        break;
                    }
                }
                return bln_Result;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }


        /// <summary>
        /// 控制当前任务状态
        /// </summary>
        /// <param name="elt_Type">任务状态,0=停止，1=启动</param>
        /// <returns></returns>
        public bool ControlTask(enmControlTaskType elt_Type)
        {
            try
            {
                bool bln_Result = false;
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                {

                    if (elt_Type == enmControlTaskType.停止)//停止任务
                    {
                        bln_Result = this.m_iee_ErrCal[0].StopCalculate((int)this.m_ett_TaskType);
                        if (!bln_Result)
                        {
                            this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                            break;
                        }

                    }
                    else//启动
                    {
                        bln_Result = this.m_iee_ErrCal[0].StartCalculate((int)this.m_ett_TaskType);
                        if (!bln_Result)
                        {
                            this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                            break;
                        }

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


        /// <summary>
        /// 启动当前计算任务
        /// </summary>
        /// <returns></returns>
        public bool StartTask()
        {
            try
            {
                bool bln_Result = false;

                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].StartCalculate((int)this.m_ett_TaskType);
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        break;
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



        /// <summary>
        /// 读取电压电流的故障状态
        /// </summary>
        /// <param name="str_Result">返回各相电压电流故障结果</param>
        /// <returns></returns>
        public bool ReadIUHitch(ref string[] str_Result, int int_WaitingTime)
        {
            this.m_str_LostMessage = "不支持自动检测故障状态";
            return false;
        }


        /// <summary>
        /// 设置各表位的通信开关
        /// </summary>
        /// <param name="int_NO">位表号(1-200)，特殊号：0xFF(255)=广播地址，0xEE(238)=偶数地址，0xDD(221)=奇数地址</param>
        /// <param name="bln_Open">是否打开，ture=打开，false=关闭</param>
        /// <returns></returns>
        public bool SetAmmeterCmmSwitch(int int_NO, bool bln_Open)
        {
            try
            {
                Thread[] thd_Thread = new Thread[this.m_iee_ErrCal.Length];
                sct_Parameter[] sct_Para = new sct_Parameter[this.m_iee_ErrCal.Length];
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                {
                    sct_Para[int_Inc] = new sct_Parameter();
                    sct_Para[int_Inc].int_Arry1 = new int[] { int_NO };
                    sct_Para[int_Inc].bln_RevResult = new bool[] { bln_Open };
                    sct_Para[int_Inc].int_ListNo = int_Inc;
                    thd_Thread[int_Inc] = new Thread(new ParameterizedThreadStart(this.ThreadAmmeterCmmSwitch));
                    thd_Thread[int_Inc].IsBackground = true;
                }

                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //启动多线操作
                    thd_Thread[int_Inc].Start(sct_Para[int_Inc]);

                bool bln_AllOK = false;
                long lng_Ticks = System.DateTime.Now.Ticks;
                while (!bln_AllOK)
                {
                    System.Windows.Forms.Application.DoEvents();
                    bln_AllOK = true;
                    for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                        if (!sct_Para[int_Inc].bln_OptResult) bln_AllOK = false;            //还有没有操作完成
                    System.Threading.Thread.Sleep(5);
                    if (System.DateTime.Now.Ticks - lng_Ticks >= 10000 * this.m_int_BWCount * 300 + 500)        //
                        break;
                }
                bln_AllOK = true;
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                    if (!sct_Para[int_Inc].bln_Result)
                    {
                        bln_AllOK = false;
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        break;
                    }
                return bln_AllOK;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 设置各表位的通信调试开关
        /// </summary>
        /// <param name="int_NO">位表号(1-200)</param>
        /// <param name="bln_Open">是否打开，ture=打开，false=关闭</param>
        /// <returns></returns>
        public bool SetAmmeterCmmDebug(int int_NO, bool bln_Open)
        {
            try
            {
                Thread[] thd_Thread = new Thread[this.m_iee_ErrCal.Length];
                sct_Parameter[] sct_Para = new sct_Parameter[this.m_iee_ErrCal.Length];
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                {
                    sct_Para[int_Inc] = new sct_Parameter();
                    sct_Para[int_Inc].int_Arry1 = new int[] { int_NO };
                    sct_Para[int_Inc].bln_RevResult = new bool[] { bln_Open };
                    sct_Para[int_Inc].int_ListNo = int_Inc;
                    thd_Thread[int_Inc] = new Thread(new ParameterizedThreadStart(this.ThreadAmmeterDebugSwitch));
                    thd_Thread[int_Inc].IsBackground = true;
                }

                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //启动多线操作
                    thd_Thread[int_Inc].Start(sct_Para[int_Inc]);

                bool bln_AllOK = false;
                long lng_Ticks = System.DateTime.Now.Ticks;
                while (!bln_AllOK)
                {
                    System.Windows.Forms.Application.DoEvents();
                    bln_AllOK = true;
                    for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                        if (!sct_Para[int_Inc].bln_OptResult) bln_AllOK = false;            //还有没有操作完成
                    System.Threading.Thread.Sleep(5);
                    if (System.DateTime.Now.Ticks - lng_Ticks >= 10000 * this.m_int_BWCount * 300 + 500)        //
                        break;
                }
                bln_AllOK = true;
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                    if (!sct_Para[int_Inc].bln_Result)
                    {
                        bln_AllOK = false;
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        break;
                    }
                return bln_AllOK;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

 
        /// <summary>
        /// 设置台体状态灯
        /// </summary>
        /// <param name="esl_LightType">状态灯类型</param>
        /// <returns></returns>
        public bool SetStateLight(enmStateLightType esl_LightType)
        {
            return false;
        }


        #endregion
        */

        #endregion

        #region --暂作废，以后为加快速度，全部需改成多线程控制--------------------------用于多线控制的误差板相关函数------------------------
        /*
        /// <summary>
        /// 用于多线程控制，传数参数的
        /// </summary>
        class sct_Parameter
        {
            public int int_ListNo;         //操作那一路总线

            public bool bln_OptResult;     //操作结果，是否完成
            public bool bln_Result;        //返回结论,成功与否

            public long[] lng_Arry1;       //Long开数组1
            public long[] lng_Arry2;       //Long开数组2

            public int[] int_Arry1;
            public int[] int_Arry2;
            public int[] int_Arry3;

            public float[] flt_Arry1;

            public string[] str_Arry1;

            public bool[] bln_RevResult;    //

        }


        /// <summary>
        /// 线程设置被检表参数（多路控制误差板）
        /// </summary>
        /// <param name="obj_Para"></param>
        private void ThreadSetAmMeterParameter(object obj_Para)
        {
            sct_Parameter sct_Para = (sct_Parameter)obj_Para;
            try
            {
                sct_Para.bln_OptResult = false;    //标志还没操作完成
                //sct_Para.bln_Result = this.m_iee_ErrCal[sct_Para.int_ListNo].SetAmMeterPulseConstAndCount(sct_Para.lng_Arry1,
                //                                                                                          sct_Para.lng_Arry2);
                sct_Para.bln_OptResult = true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                sct_Para.bln_OptResult = true;
                sct_Para.bln_Result = false;
            }
        }


        /// <summary>
        /// 多路控制误差板联机
        /// </summary>
        /// <param name="bln_Result">返回结论</param>
        /// <returns></returns>
        private bool LinkAllErrCalChannel(ref bool[] bln_Result)
        {
            try
            {

                Thread[] thd_Thread = new Thread[this.m_iee_ErrCal.Length];
                sct_Parameter[] sct_Para = new sct_Parameter[this.m_iee_ErrCal.Length];
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                {
                    sct_Para[int_Inc] = new sct_Parameter();
                    sct_Para[int_Inc].bln_RevResult = new bool[this.m_int_BWCount];
                    sct_Para[int_Inc].int_ListNo = int_Inc;
                    thd_Thread[int_Inc] = new Thread(new ParameterizedThreadStart(this.ThreadLinkAllErrCalChannel));
                    thd_Thread[int_Inc].IsBackground = true;
                }

                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //启动多线操作
                    thd_Thread[int_Inc].Start(sct_Para[int_Inc]);

                bool bln_AllOK = false;
                long lng_Ticks = System.DateTime.Now.Ticks;
                while (!bln_AllOK)
                {
                    System.Windows.Forms.Application.DoEvents();
                    bln_AllOK = true;
                    for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                        if (!sct_Para[int_Inc].bln_OptResult) bln_AllOK = false;            //还有没有操作完成
                    System.Threading.Thread.Sleep(5);
                    if (System.DateTime.Now.Ticks - lng_Ticks >= 10000 * this.m_int_BWCount * 400 + 500)        //
                        break;
                }

                bln_AllOK = false;

                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                {
                    for (int int_Inb = 0; int_Inb < this.m_int_ErrCalListTable[int_Inc].Length; int_Inb++)
                    {
                        bln_Result[this.m_int_ErrCalListTable[int_Inc][int_Inb] - 1] = sct_Para[int_Inc].bln_RevResult[this.m_int_ErrCalListTable[int_Inc][int_Inb] - 1];
                    }
                    if (sct_Para[int_Inc].bln_Result)
                        bln_AllOK = true;
                    else
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                }
                return bln_AllOK;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 线程误差板联机（多路控制误差板）
        /// </summary>
        /// <param name="obj_Para"></param>
        private void ThreadLinkAllErrCalChannel(object obj_Para)
        {
            sct_Parameter sct_Para = (sct_Parameter)obj_Para;
            try
            {
                sct_Para.bln_OptResult = false;    //标志还没操作完成
                sct_Para.bln_Result = this.m_iee_ErrCal[sct_Para.int_ListNo].Link(ref sct_Para.bln_RevResult);
                sct_Para.bln_OptResult = true;      //操作完成
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                sct_Para.bln_OptResult = true;
                sct_Para.bln_Result = false;
            }
        }

        /// <summary>
        /// 线程误差板选择通道（多路控制误差板）
        /// </summary>
        /// <param name="obj_Para"></param>
        private void ThreadSelectPulseChannel(object obj_Para)
        {
            sct_Parameter sct_Para = (sct_Parameter)obj_Para;
            try
            {
                sct_Para.bln_OptResult = false;    //标志还没操作完成
                sct_Para.bln_Result = this.m_iee_ErrCal[sct_Para.int_ListNo].SelectPulseChannel(sct_Para.int_Arry1, sct_Para.int_Arry2,
                                                                                                sct_Para.int_Arry3);
                sct_Para.bln_OptResult = true;      //操作完成
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                sct_Para.bln_OptResult = true;
                sct_Para.bln_Result = false;
            }
        }


        /// <summary>
        /// 线程误差板日计时（多路控制误差板）
        /// </summary>
        /// <param name="obj_Para"></param>
        private void ThreadSetRjsParameter(object obj_Para)
        {
            sct_Parameter sct_Para = (sct_Parameter)obj_Para;
            try
            {
                sct_Para.bln_OptResult = false;    //标志还没操作完成
                sct_Para.bln_Result = this.m_iee_ErrCal[sct_Para.int_ListNo].SetTimePara(sct_Para.flt_Arry1[0],sct_Para.int_Arry1[0]);
                sct_Para.bln_OptResult = true;      //操作完成
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                sct_Para.bln_OptResult = true;
                sct_Para.bln_Result = false;
            }
        }

        /// <summary>
        /// 线程误差板需量（多路控制误差板）
        /// </summary>
        /// <param name="obj_Para"></param>
        private void ThreadReadData(object obj_Para)
        {
            sct_Parameter sct_Para = (sct_Parameter)obj_Para;
            try
            {
                sct_Para.bln_OptResult = false;    //标志还没操作完成
                sct_Para.bln_Result = this.m_iee_ErrCal[sct_Para.int_ListNo].ReadData(0x00,ref sct_Para.bln_RevResult,
                                                                      ref sct_Para.int_Arry1, ref sct_Para.str_Arry1);
                sct_Para.bln_OptResult = true;      //操作完成
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                sct_Para.bln_OptResult = true;
                sct_Para.bln_Result = false;
            }
        }

        /// <summary>
        /// 线程设置表位RS485口通信状态
        /// </summary>
        /// <param name="obj_Para"></param>
        private void ThreadAmmeterCmmSwitch(object obj_Para)
        {
            sct_Parameter sct_Para = (sct_Parameter)obj_Para;
            try
            {
                sct_Para.bln_OptResult = false;    //标志还没操作完成
                int int_Type = 0;
                if (sct_Para.int_Arry1[0] == 238 || sct_Para.int_Arry1[0] == 221)
                {
                    int_Type = (sct_Para.int_Arry1[0] == 238) ? 0 : 1;

                    for (int int_Inc = 0; int_Inc < this.m_int_ErrCalListTable[sct_Para.int_ListNo].Length; int_Inc++)
                        if (this.m_int_ErrCalListTable[sct_Para.int_ListNo][int_Inc] % 2 == int_Type)
                        {
                            sct_Para.bln_Result = this.m_iee_ErrCal[sct_Para.int_ListNo].SetCommSwitch(this.m_int_ErrCalListTable[sct_Para.int_ListNo][int_Inc], sct_Para.bln_RevResult[0]);
                            if (!sct_Para.bln_Result)
                            {
                                this.m_str_LostMessage = this.m_iee_ErrCal[sct_Para.int_ListNo].LostMessage;
                                return;
                            }
                        }
                }
                else
                {
                    sct_Para.bln_Result = true;
                    if (Array.IndexOf(this.m_int_ErrCalListTable[sct_Para.int_ListNo], sct_Para.int_Arry1[0]) >= 0 || sct_Para.int_Arry1[0] == 255)
                        sct_Para.bln_Result = this.m_iee_ErrCal[sct_Para.int_ListNo].SetCommSwitch(sct_Para.int_Arry1[0], sct_Para.bln_RevResult[0]);
                }

                sct_Para.bln_OptResult = true;      //操作完成
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                sct_Para.bln_OptResult = true;
                sct_Para.bln_Result = false;
            }
        }

        /// <summary>
        /// 线程设置表位RS485调试状态
        /// </summary>
        /// <param name="obj_Para"></param>
        private void ThreadAmmeterDebugSwitch(object obj_Para)
        {
            sct_Parameter sct_Para = (sct_Parameter)obj_Para;
            try
            {
                sct_Para.bln_OptResult = false;    //标志还没操作完成
                int int_Type = 0;
                if (sct_Para.int_Arry1[0] == 238 || sct_Para.int_Arry1[0] == 221)
                {
                    int_Type = (sct_Para.int_Arry1[0] == 238) ? 0 : 1;

                    for (int int_Inc = 0; int_Inc < this.m_int_ErrCalListTable[sct_Para.int_ListNo].Length; int_Inc++)
                        if (this.m_int_ErrCalListTable[sct_Para.int_ListNo][int_Inc] % 2 == int_Type)
                        {
                            sct_Para.bln_Result = this.m_iee_ErrCal[sct_Para.int_ListNo].SetCommSwitch(this.m_int_ErrCalListTable[sct_Para.int_ListNo][int_Inc], sct_Para.bln_RevResult[0]);
                            if (!sct_Para.bln_Result)
                            {
                                this.m_str_LostMessage = this.m_iee_ErrCal[sct_Para.int_ListNo].LostMessage;
                                return;
                            }
                        }
                }
                else
                {
                    sct_Para.bln_Result = true;
                    if (Array.IndexOf(this.m_int_ErrCalListTable[sct_Para.int_ListNo], sct_Para.int_Arry1[0]) >= 0 || sct_Para.int_Arry1[0] == 255)
                        sct_Para.bln_Result = this.m_iee_ErrCal[sct_Para.int_ListNo].SetCommSwitch(sct_Para.int_Arry1[0], sct_Para.bln_RevResult[0]);
                }

                sct_Para.bln_OptResult = true;      //操作完成
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                sct_Para.bln_OptResult = true;
                sct_Para.bln_Result = false;
            }
        }
        */

        #endregion

    }
}
