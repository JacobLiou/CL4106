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
    /// װ�ÿ����࣬��Ҫ���ֶ�����������ģʽ,��V80��N1ϵ�С�N6ϵ�е�,�������ȵ�λ
    /// ����6ϵ��Ӳ��
    /// </summary>
    public class ClCommV7 : ICommAdapter
    {
        private static ISerialport[] S_ISP_COMLIST;    //�˿�����
        private IPower m_ipr_Power;             //�̿�Դ 
        private IStdMeter m_isr_StdMeter;       //��׼��
        private IStdTime m_ise_StdTime;         //ʱ��Դ
        private IErrorCalculate[] m_iee_ErrCal; //����
        private int[][] m_int_ErrCalListTable;  //��·���������б�
        private string m_str_LostMessage = "";  //ʧ��ԭ��
        private Stopwatch sth_SpaceTicker = new Stopwatch(); //��ʱʱ��

        //***********************
        //private bool m_bln_StdAuto = true;          //�Զ������ֶ���
        //private long m_lng_StdConst = 4000000;        //CL1115�������峣��

        //private float m_lng_StdCurP = 0f;             //CL1115��ǰ����
        //private int m_int_StdFPXS = 1;              //CL1115��Ƶϵ��


        //private int m_int_AmClfs = 0;               //���Է�ʽ
        //private long m_lng_AmConst = 16000;         //�������
        //private long m_lng_AmPulse = 2;             //Ȧ��
        //private int m_int_AmMeterPulseBS;           //��������峣�����ű���
        //private long m_lng_StdTimeConst = 500000;     //CL191��׼ʱ�����峣��
        //private float m_int_AmTimeHz = 1f;            //����ʱ��Ƶ��
        //private int m_int_AmTimePulse = 10;           //�����������

        //private enmTaskType m_ett_TaskType = enmTaskType.�������;
        //private int m_int_188PulseChannel;           //����ͨ��,0=P+,1=P-,2=Q+,3=Q-,4=����,5=ʱ��
        //private int m_int_188ChannelType;            //ͨ������,0=�����,1=���ͷ
        //private int m_int_188PulseType;              //��������,0=����,1=����


        //*************************


        private bool[] m_bln_LinkResult = new bool[4];          //�ӻ����
        private bool[] m_bln_ErrCalLinkResult;                  //����ӻ����
        private int m_int_BWCount = 24;                         //24��λ

        private Single[] sng_CurU = new Single[3];              //��ǰ��ѹ
        private Single[] sng_CurI = new Single[3];              //��ǰ����

        private bool m_bln_Enabled;                 //�����Ƿ�����
        private bool m_bln_StopTask = false;        //ֹͣ��ǰ����

        private string m_str_files = Directory.GetCurrentDirectory() + "\\ClPlugins.xml";

        //������
        private object objStopTaskLock = new object();

        public ClCommV7(int int_BWCount)
        {
            try
            {
                this.m_int_BWCount = int_BWCount;
                this.m_bln_ErrCalLinkResult = new bool[int_BWCount];
                if (!System.IO.File.Exists(m_str_files))      //û���ļ�
                    CreateXMLFile(m_str_files);                //����Ĭ����
                if (CreateIClass(m_str_files)) this.m_bln_Enabled = true;      //��������ɹ�������ϱ�־
            }
            catch (Exception e)
            {
                this.m_bln_Enabled = false;
                this.m_str_LostMessage = e.ToString();
            }
        }

        #region -------------------˽�к���---------------------

        /// <summary>
        /// ����XML�ļ�
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
        /// ��Ĭ�ϸ��豸��,����V80Ĭ������
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
                    this.m_str_LostMessage = "�Ҳ���Э����⣡";
                    return false;
                }
                if (str_Comfiles.Length <= 0)
                {
                    this.m_str_LostMessage = "�Ҳ���������⣡";
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
                        case "IErrorCalculate"://�������ö������
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
                            this.m_iee_ErrCal[this.m_iee_ErrCal.Length - 1].SetECListTable(this.m_int_ErrCalListTable[this.m_iee_ErrCal.Length - 1]);  //���õ�ǰ��һ·����������б�
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
                    this.m_str_LostMessage = "û�д����̿�Դ����";
                    return false;
                }

                if (this.m_isr_StdMeter == null)
                {
                    this.m_str_LostMessage = "û�д�����׼�����";
                    return false;
                }

                if (this.m_ise_StdTime == null)
                {
                    this.m_str_LostMessage = "û�д���ʱ��Դ����";
                    return false;
                }

                if (this.m_iee_ErrCal == null)
                {
                    this.m_str_LostMessage = "û�д����������";
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
        /// ���������ļ��������豸ͨ����
        /// </summary>
        /// <param name="str_XmlFile">�����ļ�</param>
        private bool CreateIClass(string str_XmlFile)
        {
            try
            {
                DataSet dst_XmlConfig = new DataSet();
                dst_XmlConfig.ReadXml(str_XmlFile);


                DataRow[] daw_cRowArry = dst_XmlConfig.Tables["extension"].Select("UserID='ClCommV7'");
                if (daw_cRowArry == null)
                {
                    this.m_str_LostMessage = "�Ҳ���������Ϣ��";
                    return false;         //�Ҳ���������Ϣ
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
                        case "IErrorCalculate"://�������ö������
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
                            this.m_iee_ErrCal[this.m_iee_ErrCal.Length - 1].SetECListTable(this.m_int_ErrCalListTable[this.m_iee_ErrCal.Length - 1]);  //���õ�ǰ��һ·����������б�
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
                    this.m_str_LostMessage = "û�д����̿�Դ����";
                    return false;
                }

                if (this.m_isr_StdMeter == null)
                {
                    this.m_str_LostMessage = "û�д�����׼�����";
                    return false;
                }

                if (this.m_ise_StdTime == null)
                {
                    this.m_str_LostMessage = "û�д���ʱ��Դ����";
                    return false;
                }

                if (this.m_iee_ErrCal == null)
                {
                    this.m_str_LostMessage = "û�д����������";
                    return false;
                }
                this.m_str_LostMessage = "���ж��񴴽��ɹ���";
                return true;

            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }



        }

        /// <summary>
        /// ȡ��ÿ������������б�
        /// </summary>
        /// <param name="str_CPara">���� ��ʽ��1(��ǰ·��)/3(·���ܺ�)</param>
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

                if ((this.m_int_BWCount % int_Sum > 0) && int_Start == int_Sum)     //�������ƽ�������������һ·����ȫ�������һ·
                    int_Sum = this.m_int_BWCount / int_Sum + this.m_int_BWCount % int_Sum;
                else
                    int_Sum = this.m_int_BWCount / int_Sum;     //�����ܱ�λ����ƽ������

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
        /// ����ͨ�Ŷ���
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

                this.m_str_LostMessage = "�Ҳ���ָ�����ö˿��࣡";
                return null;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return null;
            }
        }

        /// <summary>
        /// �����ļ����ӿں��࣬��ȡ���ͺ�
        /// </summary>
        /// <param name="str_DLLFile">DLL�ļ�</param>
        /// <param name="str_Interface">�ӿ���</param>
        /// <param name="str_Class">����</param>
        /// <returns></returns>
        private Type GetICType(string str_DLLFile, string str_Interface, string str_Class)
        {

            try
            {
                Assembly aby_DllFile = Assembly.LoadFile(str_DLLFile);      //��̬�����ļ�
                if (aby_DllFile != null)                            //
                {
                    Type[] tpe_Types = aby_DllFile.GetTypes();        //ȡ����ǰ.DLL������
                    for (int int_Inb = 0; int_Inb < tpe_Types.Length; int_Inb++)     //������ǰ.DLL�ļ��е��������Ǵ���ΪClassName������
                    {
                        if (tpe_Types[int_Inb].Name == str_Class)        //����ΪClassName������
                        {
                            Type[] tpe_ITypes = tpe_Types[int_Inb].GetInterfaces();    //ȡ����ǰ������м̳еĽӿ�
                            for (int int_Ina = 0; int_Ina < tpe_ITypes.Length; int_Ina++)        //�ж�������Ƿ�̳�str_Interface����ӿ�
                            {
                                if (tpe_ITypes[int_Ina].Name == str_Interface) return tpe_Types[int_Inb];     //�Ǽ̳���str_Interface�ӿ�,����Ҫ�ҵ�
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
        /// ����Ƕ�
        /// </summary>
        /// <param name="int_Clfs">������ʽ</param>
        /// <param name="str_Glys">��������</param>
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
                dbl_Phase = Math.Asin(Math.Abs(dbl_XS));                              //�޹�����
            else
                dbl_Phase = Math.Acos(Math.Abs(dbl_XS));                              //�й�����

            dbl_Phase = dbl_Phase * 180 / Math.PI;      //�ǶȻ���
            if (dbl_XS < 0) dbl_Phase = 180 + dbl_Phase;         //����
            if (str_CL == "C") dbl_Phase = 360 - dbl_Phase;
            if (dbl_Phase < 0) dbl_Phase = 360 + dbl_Phase;
            dbl_Phase %= 360;
            return Convert.ToSingle(dbl_Phase);
        }

        /// <summary>
        /// ����Ƕ�
        /// </summary>
        /// <param name="p_int_Clfs">������ʽ</param>
        /// <param name="p_str_Glys">����������</param>
        /// <param name="p_eet_Element">�Ϸ�Ԫ</param>
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
        /// ����Ƕ� �������
        /// </summary>
        /// <param name="int_Clfs">������ʽ</param>
        /// <param name="str_Glys">��������</param>
        /// <param name="bln_NXX">������</param>
        /// <returns>�������飬����Ԫ��Ϊ����ABC���ѹ�����Ƕ�</returns>
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
                dbl_Phase = Math.Asin(Math.Abs(dbl_XS));                              //�޹�����
            else
                dbl_Phase = Math.Acos(Math.Abs(dbl_XS));                              //�й�����

            dbl_Phase = dbl_Phase * 180 / Math.PI;      //�ǶȻ���
            if (dbl_XS < 0) dbl_Phase = 180 + dbl_Phase;         //����
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


        #region ------------�̳нӿڳ�Ա����\����---------------------------

        /// <summary>
        /// ��λ��
        /// </summary>
        public int BWCount
        {
            get { return this.m_int_BWCount; }
            set { this.m_int_BWCount = value; }
        }

        /// <summary>
        /// �����Ƿ�����,���Ϊfalse����ɴ�LostMessage�ж�ȡʧ����Ϣ
        /// </summary>
        public bool Enabled
        {
            get { return this.m_bln_Enabled; }
        }


        /// <summary>
        /// ʧ����Ϣ
        /// </summary>
        public string LostMessage
        {
            get { return this.m_str_LostMessage; }
        }

        /// <summary>
        /// ֹͣ��ǰ����
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
                    //��֤һ��ֻ����һ���߳��޸���
                    this.m_bln_StopTask = value;
                }
            }
        }

        /// <summary>
        /// ����
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
                        this.m_str_LostMessage = string.Format("��{0}����������ʧ��", int_Inc + 1);

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
        /// ����
        /// </summary>
        /// <param name="IsCL191BCL188L">�Ƿ����CL191B��CL188L��True�У�falseû��</param>
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
                        this.m_str_LostMessage = string.Format("��{0}����������ʧ��", int_Inc + 1);

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
        /// �ѻ�
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


        #region IPower��Դ��Դ

        #region ����
        /// <summary>
        /// ���ò��Ե�
        /// </summary>
        /// <param name="int_Clfs">������ʽ��0=PT4��1=QT4��2=PT3��3=QT3</param>
        /// <param name="sng_U">���ѹ</param>
        /// <param name="sng_xUa">A��������ѹ����</param>
        /// <param name="sng_xUb">B��������ѹ����</param>
        /// <param name="sng_xUc">C��������ѹ����</param>
        /// <param name="sng_I">�����</param>
        /// <param name="sng_Imax">������</param>
        /// <param name="int_IType">���������׼���ͣ�0=�������1=������</param>
        /// <param name="sng_xIa">A�������������I����IMax�����t����int_IType������</param>
        /// <param name="sng_xIb">B�������������I����IMax�����t����int_IType������</param>
        /// <param name="sng_xIc">C�������������I����IMax�����t����int_IType������</param>
        /// <param name="int_Element">Ԫ����0=��Ԫ��1=AԪ��2=BԪ��3=CԪ</param>
        /// <param name="str_Glys">����������������ʾ���������ݱ�ʾ����</param>
        /// <param name="sng_Freq">Ƶ��</param>
        /// <param name="bln_NXX">�Ƿ������� true=������,false=������</param>
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
                if (ecs_Clfs >= enmClfs.���������й� && ecs_Clfs <= enmClfs.��Ԫ������90) byt_XWKG &= 0x2D;   //�������� ȥ��B��

                if (eet_Element == enmElement.A)
                    byt_XWKG &= 0xf;                  //ȥ��BC��
                else if (eet_Element == enmElement.B)
                    byt_XWKG &= 0x17;                  //ȥ��AC��
                else if (eet_Element == enmElement.C)
                    byt_XWKG &= 0x27;                  //ȥ��AB��


                //---------------���ñ�׼�����---
                bln_Result = this.m_isr_StdMeter.SetAmMeterParameter(0x08,0x00,0x01);
                if (!bln_Result)
                {
                    this.m_str_LostMessage = "���ñ�׼�����ʧ�ܣ�";
                    return false;
                }

                Single[] sng_In = new float[3];

                //�����������ѹ��ͬ���ң����������ͬ���Ǻ�Ԫ��������Ҳ��������� ����35ָ���������ͳ�����
                if ((sng_xUa == sng_xUc && sng_xUa == sng_xUb)
                        && ((sng_xIa == sng_xIc && sng_xIa == sng_xIb) || eet_Element != enmElement.H)
                        && !bln_NXX)
                {
                    if (eit_IType == enmIType.�����) //Ib�������
                    {
                        sng_In[0] = sng_I * sng_xIa;
                        sng_In[1] = sng_I * sng_xIa;
                        sng_In[2] = sng_I * sng_xIa;
                    }
                    else                 //IMax���������
                    {
                        sng_In[0] = sng_Imax * sng_xIa;
                        sng_In[1] = sng_Imax * sng_xIa;
                        sng_In[2] = sng_Imax * sng_xIa;
                    }

                    //Single sng_TmpPhi = GetPhiGlys((int)ecs_Clfs, str_Glys);     //���ݲ��Է�ʽ�͹�����������Ƕ�
                    Single sng_TmpPhi = 0f;

                    if (eet_Element == enmElement.H)
                    {
                        //Ϊ�˵����Ĵ������� �����ں�Ԫʱ����ͨ������õ���������
                        sng_TmpPhi = GetPhiGlys((int)ecs_Clfs, str_Glys);     //���ݲ��Է�ʽ�͹�����������Ƕ�
                    }
                    else
                    {
                        sng_TmpPhi = GetPhiGlys((int)ecs_Clfs, str_Glys, eet_Element);//���ݲ��Է�ʽ�����������ͺϷ�Ԫ����Ƕ�
                    }


                    bln_Result = this.m_ipr_Power.PowerOn(sng_U * sng_xUa, sng_In[0], sng_TmpPhi, sng_Freq, (int)ecs_Clfs,byt_XWKG);//, eet_Element
                }
                else                                    //����������
                { 
                    if (eit_IType == enmIType.�����) //Ib�������
                    {
                        sng_In[0] = sng_I * sng_xIa;
                        sng_In[1] = sng_I * sng_xIb;
                        sng_In[2] = sng_I * sng_xIc;
                    }
                    else                //IMax���������
                    {
                        sng_In[0] = sng_Imax * sng_xIa;
                        sng_In[1] = sng_Imax * sng_xIb;
                        sng_In[2] = sng_Imax * sng_xIc;
                    }
                    Single sng_TmpPhi = 0f;

                    if (eet_Element == enmElement.H)
                    {
                        //Ϊ�˵����Ĵ������� �����ں�Ԫʱ����ͨ������õ���������
                        sng_TmpPhi = GetPhiGlys((int)ecs_Clfs, str_Glys);     //���ݲ��Է�ʽ�͹�����������Ƕ�
                    }
                    else
                    {
                        sng_TmpPhi = GetPhiGlys((int)ecs_Clfs, str_Glys, eet_Element);//���ݲ��Է�ʽ�����������ͺϷ�Ԫ����Ƕ�
                    }

                    Single[] sng_PhiXX = GetPhiGlys((int)ecs_Clfs, str_Glys, (int)eet_Element, bln_NXX); //���ݲ��Է�ʽ������������Ԫ����������������ѹ�����Ƕ�
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
        /// ���ò��Ե�
        /// </summary>
        /// <param name="int_Clfs">������ʽ��0=PT4��1=PT3��2=QT4��3=QT3</param>
        /// <param name="sng_U">���ѹ</param>
        /// <param name="sng_xU">������ѹ����</param>
        /// <param name="sng_I">�����</param>
        /// <param name="sng_Imax">������</param>
        /// <param name="int_IType">���������׼���ͣ�0=�������1=������</param>
        /// <param name="sng_xI">�����������I����IMax�����t����int_IType������</param>
        /// <param name="int_Element">Ԫ����0=��Ԫ��1=AԪ��2=BԪ��3=CԪ</param>
        /// <param name="str_Glys">����������������ʾ���������ݱ�ʾ����</param>
        /// <param name="sng_Freq">Ƶ��</param>
        /// <param name="bln_NXX">�Ƿ������� true=������,false=������</param>
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
        /// ���ò��Ե�(���಻ƽ���ѹ����������Ƕ�)
        /// </summary>
        /// <param name="int_Clfs">������ʽ��0=PT4��1=PT3��2=QT4��3=QT3</param>
        /// <param name="sng_U">���ѹ</param>
        /// <param name="sng_xUa">A��������ѹ����</param>
        /// <param name="sng_xUb">B��������ѹ����</param>
        /// <param name="sng_xUc">C��������ѹ����</param>
        /// <param name="sng_I">�����</param>
        /// <param name="sng_Imax">������</param>
        /// <param name="int_IType">���������׼���ͣ�0=�������1=������</param>
        /// <param name="sng_xIa">A�������������I����IMax�����t����int_IType������</param>
        /// <param name="sng_xIb">B�������������I����IMax�����t����int_IType������</param>
        /// <param name="sng_xIc">C�������������I����IMax�����t����int_IType������</param>
        /// <param name="int_Element">Ԫ����0=��Ԫ��1=AԪ��2=BԪ��3=CԪ</param>
        /// <param name="sng_UaPhi">A���ѹ�Ƕ�</param>
        /// <param name="sng_UbPhi">B���ѹ�Ƕ�</param>
        /// <param name="sng_UcPhi">C���ѹ�Ƕ�</param>
        /// <param name="sng_IaPhi">A������Ƕ�</param>
        /// <param name="sng_IbPhi">B������Ƕ�</param>
        /// <param name="sng_IcPhi">C������Ƕ�</param>
        /// <param name="sng_Freq">Ƶ��</param>
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
                if (ecs_Clfs >= enmClfs.���������й� && ecs_Clfs <= enmClfs.��Ԫ������90) byt_XWKG &= 0x2D;   //�������� ȥ��B��
                if (eet_Element == enmElement.A)
                    byt_XWKG &= 0xf;                  //ȥ��BC��
                else if (eet_Element == enmElement.B)
                    byt_XWKG &= 0x17;                  //ȥ��AC��
                else if (eet_Element == enmElement.C)
                    byt_XWKG &= 0x27;                  //ȥ��AB��

                //---------------���ñ�׼�����---
                bln_Result = this.m_isr_StdMeter.SetAmMeterParameter(0x08, 0x00, 0x01);
                if (!bln_Result)
                {
                    this.m_str_LostMessage = "���ñ�׼�����ʧ�ܣ�";
                    return false;
                }

                Single[] sng_In = new float[3];


                if (eit_IType == enmIType.�����) //Ib�������
                {
                    sng_In[0] = sng_I * sng_xIa;
                    sng_In[1] = sng_I * sng_xIa;
                    sng_In[2] = sng_I * sng_xIa;
                }
                else                 //IMax���������
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
        /// ���ò��Ե�(����ͳһ��ѹ����������Ƕ�)
        /// </summary>
        /// <param name="int_Clfs">������ʽ��0=PT4��1=PT3��2=QT4��3=QT3</param>
        /// <param name="sng_U">���ѹ</param>
        /// <param name="sng_xU">������ѹ����</param>
        /// <param name="sng_I">�����</param>
        /// <param name="sng_Imax">������</param>
        /// <param name="int_IType">���������׼���ͣ�0=�������1=������</param>
        /// <param name="sng_xI">�����������I����IMax�����t����int_IType������</param>
        /// <param name="int_Element">Ԫ����0=��Ԫ��1=AԪ��2=BԪ��3=CԪ</param>
        /// <param name="sng_UaPhi">A���ѹ�Ƕ�</param>
        /// <param name="sng_UbPhi">B���ѹ�Ƕ�</param>
        /// <param name="sng_UcPhi">C���ѹ�Ƕ�</param>
        /// <param name="sng_IaPhi">A������Ƕ�</param>
        /// <param name="sng_IbPhi">B������Ƕ�</param>
        /// <param name="sng_IcPhi">C������Ƕ�</param>
        /// <param name="sng_Freq">Ƶ��</param>
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

        #region ����
        /// <summary>
        /// ���ò��Ե�
        /// </summary>
        /// <param name="int_Clfs">������ʽ</param>
        /// <param name="sng_U">�����ѹ</param>
        /// <param name="sng_I">�������</param>
        /// <param name="eet_Element">Ԫ��</param>
        /// <param name="str_Glys">����������������ʾ���������ݱ�ʾ����</param>
        /// <param name="sng_Freq">Ƶ��</param>
        /// <returns></returns>
        public bool SetTestPoint(enmClfs ecs_Clfs, Single sng_U, Single sng_I, enmElement eet_Element, string str_Glys, Single sng_Freq)
        {

            try
            {
                bool bln_Result = false;

                bln_Result = this.m_isr_StdMeter.SetAmMeterParameter(0x08, 0x00, 0x00);
                if (!bln_Result)
                {
                    this.m_str_LostMessage = "���ñ�׼�����ʧ�ܣ�";
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
        /// ��Դ
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

        #region ����CL1115��CL188L�������춨
        /// <summary>
        /// ���õ���������
        /// </summary>
        /// <param name="ett_TaskType">��������</param>
        /// <param name="ect_ChannelNo">ͨ��</param>
        /// <param name="ept_PulseType">����ӿ�</param>
        /// <param name="egt_GyG">��������</param>
        /// <param name="lng_AmConst">���峣��</param>
        /// <param name="lng_PulseTimes">Ȧ��</param>
        /// <param name="iAmMeterPulseBS">����</param>
        /// <returns></returns>
        public bool SetTaskParameter(enmPulseDzType epd_DzType, enmTaskType ett_TaskType, enmChannelType ect_ChannelNo, enmPulseComType ept_PulseType, enmGyGyType egt_GyGy, long lng_AmConst, long lng_PulseTimes, byte iAmMeterPulseBS)
        {
            try
            {
                bool bAuto = true ;//�Զ������ֶ���
                long lng_StdConst = 80000000;//���峣��

                bool bln_Result = false;

                #region 0.ֹͣ����
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //����ֻ��һ��ָ���ѭ���·�����
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].StopCalculate(0x00);//(int)ett_TaskType
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        return false;
                    }
                }

                #endregion

                #region 1.����191Ϊ��������ģʽ
                bln_Result=this.m_ise_StdTime.SetChannel(1);
                if (!bln_Result) bln_Result = this.m_ise_StdTime.SetChannel(1);
                if (!bln_Result)
                {
                    this.m_str_LostMessage = this.m_ise_StdTime.LostMessage;
                    return false;
                }

                #endregion

                #region 2.��ȡ��׼���ʣ����������׼������Ƶ�ʣ��������������������û�����ã���ע��
                string[] s = new string[50];
                bln_Result = this.m_isr_StdMeter.ReadStdMeterInfo(ref s);
                if (!bln_Result)
                {
                    this.m_str_LostMessage = this.m_isr_StdMeter.LostMessage;
                    return false;
                }
                Single sng_StdP = Convert.ToSingle(s[22]);
                #endregion


                #region 3.��ȡ��׼�����峣��
                if (bAuto)//������Զ�������ȡ��׼�����峣��
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

                    #region ѭ����ѯ��׼�����Ƿ���ȷ
                    //sth_SpaceTicker.Reset();
                    //sth_SpaceTicker.Start();
                    //long _lng_StdConst_Select = this.m_isr_StdMeter.SelectStdMeterConst(Convert.ToSingle(s[3]));//�����ѯ
                    //while (true)
                    //{
                    //    if (Convert.ToInt32(sth_SpaceTicker.ElapsedMilliseconds / 1000) > 10)//�����10��
                    //    {
                    //        bln_Result = false;//����Ϊʧ��
                    //        break;//�˳�
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
                else//������ֶ���λ�������ñ�׼����SetStdMeterConst
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


                #region 4.���������������SetMeterPulseDzType����������
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //����ֻ��һ��ָ���ѭ���·�����
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].SetMeterPulseDzType((int)epd_DzType);
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        return false;
                    }
                }
                #endregion


                #region 5.����ѡ������ͨ��SelectPulseChannel��ͨ����,����ӿڣ���������
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //����ֻ��һ��ָ���ѭ���·�����
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].SelectPulseChannel(255, (int)ect_ChannelNo, (int)ept_PulseType, (int)egt_GyGy);
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        return false;
                    }
                }
                #endregion

                #region 6.���ñ����������������SetAmMeterPara�������ĳ�����Ȧ��

                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //����ֻ��һ��ָ���ѭ���·�����
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].SetDnWcrPara(lng_AmConst, lng_PulseTimes, lng_StdConst, sng_StdP, iAmMeterPulseBS);
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        return false;
                    }
                }
                #endregion

                #region 7.��������
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //����ֻ��һ��ָ���ѭ���·�����
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

        #region ����CL1115��CL188L�ռ�ʱ���춨
        /// <summary>
        /// �����ռ�ʱ������
        /// </summary>
        /// <param name="ett_TaskType">��������</param>
        /// <param name="ect_ChannelNo">ͨ��</param>
        /// <param name="ept_PulseType">����ӿ�</param>
        /// <param name="egt_GyG">��������</param>
        /// <param name="sng_TimePL">ʱ��Ƶ��</param>
        /// <param name="lng_PulseTimes">Ȧ��</param>
        /// <returns></returns>
        public bool SetTaskParameter(enmPulseDzType epd_DzType, enmTaskType ett_TaskType, enmChannelType ect_ChannelNo, enmPulseComType ept_PulseType, enmGyGyType egt_GyGy, float sng_TimePL, int lng_PulseTimes)
        {

            try
            {
                bool bln_Result = false;
                #region ֹͣ����
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //����ֻ��һ��ָ���ѭ���·�����
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].StopCalculate(0x02);//(int)ett_TaskType
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        return false;
                    }
                }

                #endregion

                #region 1.����191Ϊ��������ģʽ
                this.m_ise_StdTime.SetChannel(0);
                if (!bln_Result)
                {
                    this.m_str_LostMessage = this.m_ise_StdTime.LostMessage;
                    return false;
                }

                #endregion

                #region 2.���������������SetMeterPulseDzType����������
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //����ֻ��һ��ָ���ѭ���·�����
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].SetMeterPulseDzType((int)epd_DzType);
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        return false;
                    }
                }
                #endregion


                #region 3.����ѡ������ͨ��SelectPulseChannel��ͨ����,����ӿڣ���������
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //����ֻ��һ��ָ���ѭ���·�����
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].SelectPulseChannel(255, (int)ect_ChannelNo, (int)ept_PulseType, (int)egt_GyGy);
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        return false;
                    }
                }
                #endregion


                #region 4.���ñ����������������SetAmMeterPara�������ĳ�����Ȧ��
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //����ֻ��һ��ָ���ѭ���·�����
                {
                    bln_Result = this.m_iee_ErrCal[int_Inc].SetTimePara(sng_TimePL, lng_PulseTimes);
                    if (!bln_Result)
                    {
                        this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                        return false;
                    }
                }
                #endregion


                #region 5.��������
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //����ֻ��һ��ָ���ѭ���·�����
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
        /// ���õ�����·
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
        /// ���ñ��������������ͣ�0=�������ӣ�1=��������
        /// </summary>
        /// <param name="iPulseDzType">0=�������ӣ�1=��������</param>
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
        /// ���ø���λ��ͨ�ſ���
        /// </summary>
        /// <param name="int_NO">λ���(1-200)������ţ�0xFF(255)=�㲥��ַ��0xEE(238)=ż����ַ��0xDD(221)=������ַ</param>
        /// <param name="bln_Open">�Ƿ�򿪣�ture=�򿪣�false=�ر�</param>
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
        /// ��ȡ��׼������Ϣ
        /// </summary>
        /// <param name="str_Value">���ر�׼������Ϣ</param>
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
        /// ��ȡ����춨���ݣ��������ݸ����������������Ͳ�ͬ����ͬ
        /// </summary>
        /// <param name="int_FunNo">�������ݻ������ţ�������Ͱ�����������00��������������01�����ռ�ʱ��02�������������03�����Ա�״̬��04��</param>
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
        /// ���ñ��׼��λ
        /// </summary>
        /// <param name="sngUn">��ѹV</param>
        /// <param name="sngIn">����A</param>
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
        /// ֹͣ��ǰ�����������
        /// </summary>
        /// <param name="elt_Type">ֹͣ = 0,���� = 1</param>
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
        /// ����CL191ͨ��
        /// </summary>
        /// <param name="iType">0=��׼ʱ�����塢1=��׼��������</param>
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

        /*����

        #region IStdMeter
        /// <summary>
        /// 5.	���õ��ܲ��� 81 30 PCID 0e a3 00 09 20 uclinemode 11 usE1type ucE1switch CS
        /// </summary>
        /// <param name="Uclinemode">���߷�ʽ��CL1115���Զ����̣�08H �ֶ����̣�88H</param>
        /// <param name="UsE1type">����ָʾ��CL1115�����й�����00H    CL3115�����й�����00H  ���޹�����40H  </param>
        /// <param name="UcE1switch">����ָ� 0��ֹͣ����  1����ʼ����������  2����ʼ�����������</param>
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
        /// 2.	���ñ������� 81 30 PCID 0d a3 00 04 01 uiLocalnum CS
        /// </summary>
        /// <param name="lStdConst">��׼����</param>
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
        /// ����׼����
        /// </summary>
        /// <param name="lng_StdConst">��׼����</param>
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
        ///// ��GPS����ʱ��
        ///// </summary>
        ///// <param name="str_DateTime">����ʱ��,ע����ʽΪyyyy-mm-dd hh:mm:ss</param>
        ///// <returns></returns>
        //public bool ReadGPSDateTime(ref string str_DateTime)
        //{
        //    try
        //    {
        //        bool bln_Result = this.m_ise_StdTime.ReadGPSTime(ref str_DateTime);

        //        //���ʱ��Ϊ�գ����ȡϵͳʱ�䡣
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
        ///// ��ȡ�¶�ʪ��
        ///// </summary>
        ///// <param name="sng_Temp">�����¶�ֵ</param>
        ///// <param name="sng_Hum">����ʪ��ֵ</param>
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
        #region ѡ������ͨ��
        /// <summary>
        /// ѡ������ͨ��
        /// </summary>
        /// <param name="ect_ChannelNo">ͨ����</param>
        /// <param name="ept_PulseType">����ӿ�</param>
        /// <param name="egt_GyGy">��������</param>
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

                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //�������߲���
                    thd_Thread[int_Inc].Start(sct_Para[int_Inc]);

                bool bln_AllOK = false;
                long lng_Ticks = System.DateTime.Now.Ticks;
                while (!bln_AllOK)
                {
                    System.Windows.Forms.Application.DoEvents();
                    bln_AllOK = true;
                    for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                        if (!sct_Para[int_Inc].bln_OptResult) bln_AllOK = false;            //����û�в������
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
        /// ͳһѡ������ͨ��
        /// </summary>
        /// <param name="ect_ChannelNo">ͨ����</param>
        /// <param name="ept_PulseType">����ӿ�</param>
        /// <param name="egt_GyGy">��������</param>
        /// <returns></returns>
        public bool SelectPulseChannel(enmChannelType ect_ChannelNo, enmPulseComType ept_PulseType, enmGyGyType egt_GyGy)
        {
            try
            {
                bool bln_Result = false;
                //��ͬʱʹ������к͹��ͷʱ���๦��һ���������
                if (((int)ect_ChannelNo) == 5 || ((int)ect_ChannelNo) == 6)
                {
                    ept_PulseType = (enmPulseComType)1;
                    egt_GyGy = (enmGyGyType)0;
                }
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //����ֻ��һ��ָ���ѭ���·�����
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
        /// ����λ��������ͨ��
        /// </summary>
        /// <param name="int_BwNo">��λ��</param>
        /// <param name="ect_ChannelNo">ͨ����</param>
        /// <param name="ept_PulseType">����ӿ�</param>
        /// <param name="egt_GyGy">��������</param>
        /// <returns></returns>
        public bool SelectPulseChannel(int int_BwNo, enmChannelType ect_ChannelNo, enmPulseComType ept_PulseType, enmGyGyType egt_GyGy)
        {
            try
            {
                bool bln_Result = false;
                //��ͬʱʹ������к͹��ͷʱ���๦��һ���������
                if (((int)ect_ChannelNo) == 5 || ((int)ect_ChannelNo) == 6)
                {
                    ept_PulseType = (enmPulseComType)1;
                    egt_GyGy = (enmGyGyType)0;
                }
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //����ֻ��һ��ָ���ѭ���·�����
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
        /// ���ñ����ĳ�����Ȧ��
        /// </summary>
        /// <param name="lng_AmConst">�����</param>
        /// <param name="lng_PulseTimes">Ȧ��</param>
        /// <returns></returns>
        public bool SetDnWcParameter(long[] lng_AmConst, long[] lng_PulseTimes, long lStdPulseConst, float fStrandMeterP, byte iAmMeterPulseBS)
        {
            try
            {
                this.m_lng_AmConst = lng_AmConst[0];
                this.m_lng_AmPulse = lng_PulseTimes[0];
                if (this.m_iee_ErrCal.Length > 1)           //��·�����������
                {
                    Thread[] thd_Thread = new Thread[this.m_iee_ErrCal.Length];
                    sct_Parameter[] sct_Para = new sct_Parameter[this.m_iee_ErrCal.Length];
                    for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                    {
                        sct_Para[int_Inc] = new sct_Parameter();
                        sct_Para[int_Inc].lng_Arry1 = lng_AmConst;          //����
                        sct_Para[int_Inc].lng_Arry2 = lng_PulseTimes;
                        sct_Para[int_Inc].int_ListNo = int_Inc;
                        thd_Thread[int_Inc] = new Thread(new ParameterizedThreadStart(this.ThreadSetAmMeterParameter));
                        thd_Thread[int_Inc].IsBackground = true;
                    }

                    for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //�������߲���
                        thd_Thread[int_Inc].Start(sct_Para[int_Inc]);

                    bool bln_AllOK = false;
                    long lng_Ticks = System.DateTime.Now.Ticks;
                    while (!bln_AllOK)
                    {
                        System.Windows.Forms.Application.DoEvents();
                        bln_AllOK = true;
                        for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                            if (!sct_Para[int_Inc].bln_OptResult) bln_AllOK = false;            //����û�в������
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
        /// ͳһ���ñ��������Ȧ��
        /// </summary>
        /// <param name="lng_AmConst">�����</param>
        /// <param name="lng_PulseTimes">Ȧ��</param>
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
        /// �����ռ�ʱ����
        /// </summary>
        /// <param name="sng_TimePL">�����ʱ��Ƶ��</param>
        /// <param name="int_Times">�춨Ȧ��</param>
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

                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //�������߲���
                    thd_Thread[int_Inc].Start(sct_Para[int_Inc]);

                bool bln_AllOK = false;
                long lng_Ticks = System.DateTime.Now.Ticks;
                while (!bln_AllOK)
                {
                    System.Windows.Forms.Application.DoEvents();
                    bln_AllOK = true;
                    for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                        if (!sct_Para[int_Inc].bln_OptResult) bln_AllOK = false;            //����û�в������
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
        /// ͳһ�����ռ�ʱ����
        /// </summary>
        /// <param name="sng_TimePL">�����ʱ��Ƶ��</param>
        /// <param name="int_Times">�춨Ȧ��</param>
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
        /// ���Ƶ�ǰ����״̬
        /// </summary>
        /// <param name="elt_Type">����״̬,0=ֹͣ��1=����</param>
        /// <returns></returns>
        public bool ControlTask(enmControlTaskType elt_Type)
        {
            try
            {
                bool bln_Result = false;
                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                {

                    if (elt_Type == enmControlTaskType.ֹͣ)//ֹͣ����
                    {
                        bln_Result = this.m_iee_ErrCal[0].StopCalculate((int)this.m_ett_TaskType);
                        if (!bln_Result)
                        {
                            this.m_str_LostMessage = this.m_iee_ErrCal[int_Inc].LostMessage;
                            break;
                        }

                    }
                    else//����
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
        /// ������ǰ��������
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
        /// ��ȡ��ѹ�����Ĺ���״̬
        /// </summary>
        /// <param name="str_Result">���ظ����ѹ�������Ͻ��</param>
        /// <returns></returns>
        public bool ReadIUHitch(ref string[] str_Result, int int_WaitingTime)
        {
            this.m_str_LostMessage = "��֧���Զ�������״̬";
            return false;
        }


        /// <summary>
        /// ���ø���λ��ͨ�ſ���
        /// </summary>
        /// <param name="int_NO">λ���(1-200)������ţ�0xFF(255)=�㲥��ַ��0xEE(238)=ż����ַ��0xDD(221)=������ַ</param>
        /// <param name="bln_Open">�Ƿ�򿪣�ture=�򿪣�false=�ر�</param>
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

                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //�������߲���
                    thd_Thread[int_Inc].Start(sct_Para[int_Inc]);

                bool bln_AllOK = false;
                long lng_Ticks = System.DateTime.Now.Ticks;
                while (!bln_AllOK)
                {
                    System.Windows.Forms.Application.DoEvents();
                    bln_AllOK = true;
                    for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                        if (!sct_Para[int_Inc].bln_OptResult) bln_AllOK = false;            //����û�в������
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
        /// ���ø���λ��ͨ�ŵ��Կ���
        /// </summary>
        /// <param name="int_NO">λ���(1-200)</param>
        /// <param name="bln_Open">�Ƿ�򿪣�ture=�򿪣�false=�ر�</param>
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

                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //�������߲���
                    thd_Thread[int_Inc].Start(sct_Para[int_Inc]);

                bool bln_AllOK = false;
                long lng_Ticks = System.DateTime.Now.Ticks;
                while (!bln_AllOK)
                {
                    System.Windows.Forms.Application.DoEvents();
                    bln_AllOK = true;
                    for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                        if (!sct_Para[int_Inc].bln_OptResult) bln_AllOK = false;            //����û�в������
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
        /// ����̨��״̬��
        /// </summary>
        /// <param name="esl_LightType">״̬������</param>
        /// <returns></returns>
        public bool SetStateLight(enmStateLightType esl_LightType)
        {
            return false;
        }


        #endregion
        */

        #endregion

        #region --�����ϣ��Ժ�Ϊ�ӿ��ٶȣ�ȫ����ĳɶ��߳̿���--------------------------���ڶ��߿��Ƶ�������غ���------------------------
        /*
        /// <summary>
        /// ���ڶ��߳̿��ƣ�����������
        /// </summary>
        class sct_Parameter
        {
            public int int_ListNo;         //������һ·����

            public bool bln_OptResult;     //����������Ƿ����
            public bool bln_Result;        //���ؽ���,�ɹ����

            public long[] lng_Arry1;       //Long������1
            public long[] lng_Arry2;       //Long������2

            public int[] int_Arry1;
            public int[] int_Arry2;
            public int[] int_Arry3;

            public float[] flt_Arry1;

            public string[] str_Arry1;

            public bool[] bln_RevResult;    //

        }


        /// <summary>
        /// �߳����ñ�����������·�������壩
        /// </summary>
        /// <param name="obj_Para"></param>
        private void ThreadSetAmMeterParameter(object obj_Para)
        {
            sct_Parameter sct_Para = (sct_Parameter)obj_Para;
            try
            {
                sct_Para.bln_OptResult = false;    //��־��û�������
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
        /// ��·������������
        /// </summary>
        /// <param name="bln_Result">���ؽ���</param>
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

                for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)        //�������߲���
                    thd_Thread[int_Inc].Start(sct_Para[int_Inc]);

                bool bln_AllOK = false;
                long lng_Ticks = System.DateTime.Now.Ticks;
                while (!bln_AllOK)
                {
                    System.Windows.Forms.Application.DoEvents();
                    bln_AllOK = true;
                    for (int int_Inc = 0; int_Inc < this.m_iee_ErrCal.Length; int_Inc++)
                        if (!sct_Para[int_Inc].bln_OptResult) bln_AllOK = false;            //����û�в������
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
        /// �߳�������������·�������壩
        /// </summary>
        /// <param name="obj_Para"></param>
        private void ThreadLinkAllErrCalChannel(object obj_Para)
        {
            sct_Parameter sct_Para = (sct_Parameter)obj_Para;
            try
            {
                sct_Para.bln_OptResult = false;    //��־��û�������
                sct_Para.bln_Result = this.m_iee_ErrCal[sct_Para.int_ListNo].Link(ref sct_Para.bln_RevResult);
                sct_Para.bln_OptResult = true;      //�������
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                sct_Para.bln_OptResult = true;
                sct_Para.bln_Result = false;
            }
        }

        /// <summary>
        /// �߳�����ѡ��ͨ������·�������壩
        /// </summary>
        /// <param name="obj_Para"></param>
        private void ThreadSelectPulseChannel(object obj_Para)
        {
            sct_Parameter sct_Para = (sct_Parameter)obj_Para;
            try
            {
                sct_Para.bln_OptResult = false;    //��־��û�������
                sct_Para.bln_Result = this.m_iee_ErrCal[sct_Para.int_ListNo].SelectPulseChannel(sct_Para.int_Arry1, sct_Para.int_Arry2,
                                                                                                sct_Para.int_Arry3);
                sct_Para.bln_OptResult = true;      //�������
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                sct_Para.bln_OptResult = true;
                sct_Para.bln_Result = false;
            }
        }


        /// <summary>
        /// �߳������ռ�ʱ����·�������壩
        /// </summary>
        /// <param name="obj_Para"></param>
        private void ThreadSetRjsParameter(object obj_Para)
        {
            sct_Parameter sct_Para = (sct_Parameter)obj_Para;
            try
            {
                sct_Para.bln_OptResult = false;    //��־��û�������
                sct_Para.bln_Result = this.m_iee_ErrCal[sct_Para.int_ListNo].SetTimePara(sct_Para.flt_Arry1[0],sct_Para.int_Arry1[0]);
                sct_Para.bln_OptResult = true;      //�������
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                sct_Para.bln_OptResult = true;
                sct_Para.bln_Result = false;
            }
        }

        /// <summary>
        /// �߳�������������·�������壩
        /// </summary>
        /// <param name="obj_Para"></param>
        private void ThreadReadData(object obj_Para)
        {
            sct_Parameter sct_Para = (sct_Parameter)obj_Para;
            try
            {
                sct_Para.bln_OptResult = false;    //��־��û�������
                sct_Para.bln_Result = this.m_iee_ErrCal[sct_Para.int_ListNo].ReadData(0x00,ref sct_Para.bln_RevResult,
                                                                      ref sct_Para.int_Arry1, ref sct_Para.str_Arry1);
                sct_Para.bln_OptResult = true;      //�������
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                sct_Para.bln_OptResult = true;
                sct_Para.bln_Result = false;
            }
        }

        /// <summary>
        /// �߳����ñ�λRS485��ͨ��״̬
        /// </summary>
        /// <param name="obj_Para"></param>
        private void ThreadAmmeterCmmSwitch(object obj_Para)
        {
            sct_Parameter sct_Para = (sct_Parameter)obj_Para;
            try
            {
                sct_Para.bln_OptResult = false;    //��־��û�������
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

                sct_Para.bln_OptResult = true;      //�������
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                sct_Para.bln_OptResult = true;
                sct_Para.bln_Result = false;
            }
        }

        /// <summary>
        /// �߳����ñ�λRS485����״̬
        /// </summary>
        /// <param name="obj_Para"></param>
        private void ThreadAmmeterDebugSwitch(object obj_Para)
        {
            sct_Parameter sct_Para = (sct_Parameter)obj_Para;
            try
            {
                sct_Para.bln_OptResult = false;    //��־��û�������
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

                sct_Para.bln_OptResult = true;      //�������
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
