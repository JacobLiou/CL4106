/****************************************************************************

    CL2018-1����
    ��ΰ 2009-10

*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using pwInterface;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace pwComPorts
{

    public class CCL20181 :ISerialport 
    {

        delegate void ReceiveDelegate(Socket SOT);                              //���ڽ����ϲ��·�����Ϣ
        delegate void DisposeEventDelegete(byte[] bData, int iLen);             //�����յ��ϲ��·�����Ϣ

        private static IPEndPoint[] RomteEndPint;
        


        /// <summary>
        /// ��ʼ�󶨶˿ں�
        /// </summary>
        public int StartBindPort = 20000;

        private IPEndPoint m_ipt_RomtePoint;

        private IPEndPoint m_ipt_ComBindPoint;
        private IPEndPoint m_ipt_BtlBindPoint;

        
        private Socket m_skt_Cl20181Com;
        private Socket m_skt_Cl20181Btl;

        private Byte[] m_byt_RevData = new Byte[1024];
        private int m_int_RevLen;
        private bool m_bln_ComState = false;

        private int m_int_Com;
        private string m_str_Setting ="38400,n,8,1";
        private string m_str_IP = "193.168.18.1:10003:20000";

        private int m_int_Channel = 0;

        private string m_str_LostMessage = "";  //ʧ����ʾ��Ϣ

        public event RevEventDelegete DataReceive;
        
        
        public CCL20181()
        {
        }

        ~CCL20181()
        {
            try
            {
                m_skt_Cl20181Com.Close();
                m_skt_Cl20181Btl.Close();
                
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
            }
        }


        private void IniParameter(string str_RomteIP, int int_Com)
        {
            try
            {
                this.m_int_Com = int_Com;

                string[] str_Para = str_RomteIP.Split(new char[] { ':' });

                string str_MyIP = "193.168.18.1";
                int int_RomtePort = 10003;
                this.StartBindPort = 20000;
                if (str_Para.Length > 0)
                {
                    IPAddress ipa_Tmp;
                    if (IPAddress.TryParse(str_Para[0], out ipa_Tmp))
                    {
                        str_MyIP = str_Para[0];
                    }
                }
                if (str_Para.Length >= 3)
                {
                    int_RomtePort = Convert.ToInt16(str_Para[1]);
                    this.StartBindPort = Convert.ToInt16(str_Para[2]);
                }

                this.m_str_IP = str_MyIP + ":" + int_RomtePort.ToString() + ":" + this.StartBindPort.ToString();
                
                this.m_ipt_RomtePoint = CreateRomteIPEndPoint(str_MyIP, int_RomtePort);
                this.m_bln_ComState = CreateCom(int_Com);

                #region ��ʼ���˿ڲ�����
                string str_Data = "init " + m_str_Setting.Replace(',', ' ');
                byte[] byt_Data = ASCIIEncoding.ASCII.GetBytes(str_Data);
                this.m_skt_Cl20181Btl.SendTo(byt_Data, this.m_ipt_RomtePoint);
                #endregion

            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                this.m_bln_ComState = false;
                
                //�˿ڲ����ڣ���˿���ʹ��ʱ�������pseilwe2010-01-27
            }
        }

        private bool CreateCom(int int_Com)
        {

            try
            {
                if (this.m_ipt_ComBindPoint != null) this.m_ipt_ComBindPoint = null;
                if (this.m_ipt_BtlBindPoint != null) this.m_ipt_BtlBindPoint = null;

                if (this.m_skt_Cl20181Btl != null)
                {
                    this.m_skt_Cl20181Btl.Close();
                    this.m_skt_Cl20181Btl = null;
                }
                if (this.m_skt_Cl20181Com != null)
                {
                    this.m_skt_Cl20181Com.Close();
                    this.m_skt_Cl20181Com = null;
                }

                IPAddress[] ipa_BindIP = GetAddressList(this.m_ipt_RomtePoint.Address.ToString());

                if (ipa_BindIP == null || ipa_BindIP.Length <= 0)
                {
                    this.m_str_LostMessage = "�ڱ������Ҳ�������������ӵ�IP";
                    return false;
                }

                this.m_ipt_ComBindPoint = new IPEndPoint(ipa_BindIP[0], StartBindPort + (int_Com - 1) * 2);��
                this.m_ipt_BtlBindPoint = new IPEndPoint(ipa_BindIP[0], StartBindPort + (int_Com - 1) * 2 + 1);
                

                this.m_skt_Cl20181Com = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                this.m_skt_Cl20181Com.Bind(this.m_ipt_ComBindPoint);
               
          
                this.m_skt_Cl20181Btl = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                this.m_skt_Cl20181Btl.Bind(this.m_ipt_BtlBindPoint);


                AsyReceive(this.m_skt_Cl20181Com);

                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                this.m_bln_ComState = false;
                return false;
            }

        }




        /// <summary>
        /// ȡ���������ͬһ���ε�IP
        /// </summary>
        /// <param name="str_IP">������IP</param>
        /// <returns></returns>
        private IPAddress[] GetAddressList(string str_IP)
        {
            try
            {
                IPAddress[] ipa_AddrList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                IPAddress[] ipa_GetAddrList = new IPAddress[0];
                string[] str_IpPara = str_IP.Split(new char[] { '.' });
                for (int int_Inc = 0; int_Inc < ipa_AddrList.Length; int_Inc++)
                {
                    string str_TmpIp = ipa_AddrList[int_Inc].ToString();
                    string[] str_TmpIpPara = str_TmpIp.Split(new char[] { '.' });
                    if (str_IpPara[0] == str_TmpIpPara[0] && str_IpPara[1] == str_TmpIpPara[1]
                        && str_IpPara[2] == str_TmpIpPara[2])
                    {
                        if (ipa_GetAddrList == null)
                        {
                            ipa_GetAddrList = new IPAddress[1];
                            ipa_GetAddrList[0] = ipa_AddrList[int_Inc];
                        }
                        else
                        {
                            int int_Len = ipa_GetAddrList.Length;
                            Array.Resize(ref ipa_GetAddrList, int_Len + 1);
                            ipa_GetAddrList[int_Len] = ipa_AddrList[int_Inc];
                        }
                    }
                }
                return ipa_GetAddrList;
            }
            catch (Exception e)
            {
                this.m_bln_ComState = false;
                this.m_str_LostMessage = e.ToString();
                return null;
            }
        }

        /// <summary>
        /// ����Զ�����ӽڵ�
        /// </summary>
        /// <param name="str_RomteIP">Զ�̵�ַ</param>
        /// <param name="int_RomtePort">Զ�̶˿�</param>
        /// <returns></returns>
        private IPEndPoint CreateRomteIPEndPoint(string str_RomteIP, int int_RomtePort)
        {
            try
            {
                if (RomteEndPint == null)               //��һ������Զ�̷�����
                {
                    RomteEndPint = new IPEndPoint[1];
                    RomteEndPint[0] = new IPEndPoint(IPAddress.Parse(str_RomteIP), int_RomtePort);
                    return RomteEndPint[0];
                }
                else
                {
                    for (int int_Inc = 0; int_Inc < RomteEndPint.Length; int_Inc++)
                    {
                        //�����Ƿ��и���������һ����
                        //if (RomteEndPint[int_Inc].Address.Address == IPAddress.Parse(str_RomteIP).Address && RomteEndPint[int_Inc].Port == int_RomtePort)
                        if (RomteEndPint[int_Inc].ToString() == str_RomteIP && RomteEndPint[int_Inc].Port == int_RomtePort)
                            return RomteEndPint[int_Inc];
                    }
                    //û�ҵ�������
                    int int_Len = RomteEndPint.Length;
                    Array.Resize(ref RomteEndPint, int_Len + 1);
                    RomteEndPint[int_Len] = new IPEndPoint(IPAddress.Parse(str_RomteIP), int_RomtePort);
                    return RomteEndPint[int_Len];
                }
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                this.m_bln_ComState = false;
                return null;
            }
        }



        private bool SetSetting(string str_Setting)
        {
            try
            {
                if (this.m_bln_ComState)
                {
                    string str_Data = "init " + str_Setting.Replace(',', ' ');
                    byte[] byt_Data = ASCIIEncoding.ASCII.GetBytes(str_Data);
                    this.m_skt_Cl20181Btl.SendTo(byt_Data, this.m_ipt_RomtePoint);

                    return true;
                }
                else
                {
                    this.m_bln_ComState = CreateCom(this.m_int_Com);
                    if (this.m_bln_ComState)
                    {
                        string str_Data = "init " + str_Setting.Replace(',', ' ');
                        byte[] byt_Data = ASCIIEncoding.ASCII.GetBytes(str_Data);
                        this.m_skt_Cl20181Btl.SendTo(byt_Data, this.m_ipt_RomtePoint);
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                this.m_bln_ComState = false;
                return false;
            }
        }


        /// <summary>
        /// �첽�����ϲ��·�ָ��
        /// </summary>
        /// <param name="SOT">���ն���</param>
        private void AsyReceive(Socket SOT)
        {
            try
            {
                this.m_int_RevLen = 0;
                Array.Clear(this.m_byt_RevData, 0, 1024);
                SOT.BeginReceive(this.m_byt_RevData, 0, 1024, SocketFlags.None,
                                     new AsyncCallback(CallReceive), SOT);
            }
            catch (Exception ex)
            {
                this.m_str_LostMessage = ex.ToString();
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="iar"></param>
        private void CallReceive(IAsyncResult iar)
        {
            Socket soc = (Socket)iar.AsyncState;
            try
            {
                this.m_int_RevLen = soc.EndReceive(iar);
                if (this.m_int_RevLen > 0)
                {
                    DisposeEventDelegete DispCmd = new DisposeEventDelegete(AsyDisposeCommand);  //ί�д���ָ��
                    DispCmd(this.m_byt_RevData, this.m_int_RevLen);
                }
                ReceiveDelegate LoopReceive = new ReceiveDelegate(AsyReceive);                   //ί�н�����һ��ָ��
                LoopReceive(this.m_skt_Cl20181Com);
            }
            catch 
            {
                ReceiveDelegate LoopReceive = new ReceiveDelegate(AsyReceive);                   //ί�н�����һ��ָ��
                LoopReceive(this.m_skt_Cl20181Com);
            }
        }


        
        private void AsyDisposeCommand(byte[] bData, int iLen)
        {
            byte[] byt_MyRevData = new byte[iLen];
            try
            {
                Array.Copy(bData, byt_MyRevData, iLen);
                //Console.WriteLine("COM" + this.m_int_Com.ToString() +",Len:"+ iLen.ToString()+ " Rx:" + BitConverter.ToString(byt_MyRevData));
                if (DataReceive != null) DataReceive(byt_MyRevData);
            }
            catch (Exception ex)
            {
                this.m_str_LostMessage = ex.ToString();
            }
        }





        #region ISerialport ��Ա


        /// <summary>
        /// ͨ��ѡ����Ҫ���ڿ���CL2011�豸�ģ���Чֵ1-4,
        /// ����PC���ڵ�RTSEnable��DTREnable��4�����
        /// CL2018-1������Ч��
        /// </summary>
        public int Channel
        {
            get { return this.m_int_Channel; }
            set { this.m_int_Channel = value; }
        }



        public bool State
        {
            get { return this.m_bln_ComState; }
        }


        public int ComPort
        {
            get
            {
                return this.m_int_Com;
            }
            set
            {
                int int_Com = value;
                if (this.m_int_Com != int_Com)
                {
                    this.m_bln_ComState = CreateCom(int_Com);
                    if (this.m_bln_ComState) this.m_int_Com = int_Com;
                }
            }
        }

        public int ComType
        {
            get { return 1; }
        }

        public string Setting
        {
            get
            {
                return this.m_str_Setting;
            }
            set
            {
                string str_Data = value;
                if (this.m_str_Setting != str_Data )//|| !this.m_bln_ComState
                {
                    if (this.SetSetting(str_Data))
                        if (this.m_bln_ComState)
                            this.m_str_Setting = str_Data;
                }
            }
        }

        public string IP
        {
            get { return this.m_str_IP; }
        }


        /// <summary>
        /// �򿪶˿�
        /// </summary>
        /// <param name="str_Para">�˿ڲ�������ʽ���˿ں�,IP:Զ��Port��:����ʼPort��  ע������PC�˿ں����������ʡ��</param>
        /// <returns></returns>
        public bool PortOpen(string str_Para)
        {
            if (this.m_bln_ComState)
            {
                this.m_str_LostMessage = "�˿��Ѿ����ˡ�";
                return false;
            }
            string[] str_Data = str_Para.Split(new char[] { ',' });
            if (str_Data.Length == 2)
            {
                IniParameter(str_Data[1], Convert.ToInt16(str_Data[0]));
                return this.m_bln_ComState;
            }
            else
            {
                this.m_str_LostMessage = "�˿����ò���������Ҫ��";
                return false;
            }

        }

        public bool PortClose()
        {
            try
            {
                if (this.m_bln_ComState)
                {
                    this.m_skt_Cl20181Btl.Close();
                    this.m_skt_Cl20181Com.Close();
                    this.m_bln_ComState = false;
                }
                return true;
            }
            catch 
            {
                return false;
            }
        }

        public bool SendData(byte[] byt_Data)
        {
            try
            {
                if (!this.m_bln_ComState)
                {
                    this.m_bln_ComState = CreateCom(this.m_int_Com);
                    if (!this.m_bln_ComState)
                        return false;
                }
                this.m_skt_Cl20181Com.SendTo(byt_Data, this.m_ipt_RomtePoint);
                return true;
            }
            catch(System.Exception ErrSend)
            {
                this.m_str_LostMessage = ErrSend.Message;
                return false ;
            }
        }

        public string LostMessage
        {
            get { return this.m_str_LostMessage; }
        }


        #endregion
    }

}

