/****************************************************************************

    CL2018-1控制
    刘伟 2009-10

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

        delegate void ReceiveDelegate(Socket SOT);                              //用于接收上层下发的信息
        delegate void DisposeEventDelegete(byte[] bData, int iLen);             //处理收到上层下发的信息

        private static IPEndPoint[] RomteEndPint;
        


        /// <summary>
        /// 起始绑定端口号
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

        private string m_str_LostMessage = "";  //失败提示信息

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

                #region 初始化端口波特率
                string str_Data = "init " + m_str_Setting.Replace(',', ' ');
                byte[] byt_Data = ASCIIEncoding.ASCII.GetBytes(str_Data);
                this.m_skt_Cl20181Btl.SendTo(byt_Data, this.m_ipt_RomtePoint);
                #endregion

            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                this.m_bln_ComState = false;
                
                //端口不存在，或端口在使用时，会出错，pseilwe2010-01-27
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
                    this.m_str_LostMessage = "在本脑上找不到与服务器连接的IP";
                    return false;
                }

                this.m_ipt_ComBindPoint = new IPEndPoint(ipa_BindIP[0], StartBindPort + (int_Com - 1) * 2);　
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
        /// 取出与服务器同一网段的IP
        /// </summary>
        /// <param name="str_IP">服务器IP</param>
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
        /// 创建远程连接节点
        /// </summary>
        /// <param name="str_RomteIP">远程地址</param>
        /// <param name="int_RomtePort">远程端口</param>
        /// <returns></returns>
        private IPEndPoint CreateRomteIPEndPoint(string str_RomteIP, int int_RomtePort)
        {
            try
            {
                if (RomteEndPint == null)               //第一次设置远程服务器
                {
                    RomteEndPint = new IPEndPoint[1];
                    RomteEndPint[0] = new IPEndPoint(IPAddress.Parse(str_RomteIP), int_RomtePort);
                    return RomteEndPint[0];
                }
                else
                {
                    for (int int_Inc = 0; int_Inc < RomteEndPint.Length; int_Inc++)
                    {
                        //查找是否有跟本次设置一样的
                        //if (RomteEndPint[int_Inc].Address.Address == IPAddress.Parse(str_RomteIP).Address && RomteEndPint[int_Inc].Port == int_RomtePort)
                        if (RomteEndPint[int_Inc].ToString() == str_RomteIP && RomteEndPint[int_Inc].Port == int_RomtePort)
                            return RomteEndPint[int_Inc];
                    }
                    //没找到则新增
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
        /// 异步接收上层下发指令
        /// </summary>
        /// <param name="SOT">接收对象</param>
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
        /// 接收数据
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
                    DisposeEventDelegete DispCmd = new DisposeEventDelegete(AsyDisposeCommand);  //委托处理指令
                    DispCmd(this.m_byt_RevData, this.m_int_RevLen);
                }
                ReceiveDelegate LoopReceive = new ReceiveDelegate(AsyReceive);                   //委托接收下一条指令
                LoopReceive(this.m_skt_Cl20181Com);
            }
            catch 
            {
                ReceiveDelegate LoopReceive = new ReceiveDelegate(AsyReceive);                   //委托接收下一条指令
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





        #region ISerialport 成员


        /// <summary>
        /// 通道选择，主要用于控制CL2011设备的，有效值1-4,
        /// 控制PC串口的RTSEnable和DTREnable的4种组合
        /// CL2018-1串口无效。
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
        /// 打开端口
        /// </summary>
        /// <param name="str_Para">端口参数，格式：端口号,IP:远程Port号:绑定起始Port号  注：如是PC端口后面参数可以省略</param>
        /// <returns></returns>
        public bool PortOpen(string str_Para)
        {
            if (this.m_bln_ComState)
            {
                this.m_str_LostMessage = "端口已经打开了。";
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
                this.m_str_LostMessage = "端口设置参数不符合要求。";
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

