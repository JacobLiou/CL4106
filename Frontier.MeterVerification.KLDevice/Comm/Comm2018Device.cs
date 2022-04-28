using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Microsoft.Win32;
using System.Threading;
using Frontier.MeterVerification.DeviceCommon;
using VerificationEquipment.Commons;

namespace Frontier.MeterVerification.KLDevice
{
    public class Comm2018Device : CommPortDevice, IConnection
    {

        private List<byte> mydataList = new List<byte>();

        public string ConnectName
        {
            get
            {
                return Point.ToString();
            }
            set
            {
            }
        }

        public int MaxWaitSeconds
        {
            get;
            set;
        }

        public int WaitSecondsPerByte
        {
            get;
            set;
        }

        /// <summary>
        /// 更新232串口波特率
        /// </summary>
        /// <param name="szSetting"></param>
        /// <returns></returns>
        public bool UpdateBltSetting(string szSetting)
        {
            if (szBlt == szSetting) return true;//与上次相同，则不用初始化
            szBlt = szSetting;
            int settingPort = UdpBindPort + 1;          
            UdpClient settingClient = new UdpClient(settingPort);
            try
            {
                settingClient.Connect(Point);
                string str_Data = "init " + szBlt.Replace(',', ' ');
                byte[] byt_Data = ASCIIEncoding.ASCII.GetBytes(str_Data);
                int sendlen = settingClient.Send(byt_Data, byt_Data.Length);
                return sendlen == byt_Data.Length;
            }
            catch { }
            finally
            {
            }
            return false;
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="vData"></param>
        /// <param name="IsReturn"></param>
        /// <returns></returns>
        public bool SendData(ref byte[] vData, bool IsReturn)
        {
            try
            {
                lock (this)
                {
                    Client = new UdpClient();
                    Client.Client.Bind(this.localPoint);
                }
                Client.Connect(Point);
            }
            catch (Exception e)
            {
                return false;
            }
            Client.Send(vData, vData.Length);
            Console.WriteLine(UdpBindPort.ToString());
            Console.WriteLine("┏SendData:{0}", BitConverter.ToString(vData));
            if (!IsReturn)
            {
                Console.WriteLine("┗本包不需要回复");
                Client.Close();
                return true;
            }

            byte[] BytReceived = new byte[0];
            bool IsReveive = false;     //标志是否返回
            List<byte> RevItems = new List<byte>();     //接收的数据集合
            DateTime Dt;            //等待时间变量
            Dt = DateTime.Now;
            while (TimeSub(DateTime.Now, Dt) < MaxWaitSeconds)
            {
                Thread.Sleep(1);
                try
                {
                    if (Client.Available > 0)
                    {
                        BytReceived = Client.Receive(ref Point);
                        IsReveive = true;
                        break;
                    }
                }
                catch
                {
                    Client.Close();
                    return false;
                }
            }

            if (!IsReveive)
            {
                vData = new byte[0];
            }
            else
            {
                RevItems.AddRange(BytReceived);
                mydataList.AddRange(BytReceived);
                Dt = DateTime.Now;
                while (TimeSub(DateTime.Now, Dt) < WaitSecondsPerByte)
                {
                    if (Client.Available > 0)
                    {
                        BytReceived = Client.Receive(ref Point);
                        RevItems.AddRange(BytReceived);
                        mydataList.AddRange(BytReceived);
                        Dt = DateTime.Now;
                    }
                }
                vData = RevItems.ToArray();
            }
            Console.WriteLine("┗RecvData:{0}", BitConverter.ToString(vData));
            Client.Close();
            return true;
        }


        private long TimeSub(DateTime Time1, DateTime Time2)
        {
            TimeSpan tsSub = Time1.Subtract(Time2);
            return tsSub.Hours * 60 * 60 * 1000 + tsSub.Minutes * 60 * 1000 + tsSub.Seconds * 1000 + tsSub.Milliseconds;
        }
        public new bool Close()
        {
            try
            {
                Client.Close();
                return true;
            }
            catch (Exception e)
            {
                return false;
                throw new DeviceCommon.PortException("关闭设备网口失败", e);
            }
            finally
            {
                if (Client != null)
                {
                    Client = null;

                }
            }
        }

        protected override void Send(byte[] buffer)
        {
            try
            {
                lock (this)
                {
                    Client = new UdpClient();
                    Client.Client.Bind(this.localPoint);
                }
                Client.Connect(Point);

                Client.Send(buffer, buffer.Length);
                Console.WriteLine(UdpBindPort.ToString());
                Console.WriteLine("┏SendData:{0}", BitConverter.ToString(buffer));
            }
            catch (Exception e)
            {
            }


        }

        protected override void Send(List<byte> buffer)
        {
            byte[] sendbuffer = buffer.ToArray();
            SendData(ref sendbuffer, true);
        }

        protected override void AnalyzeMessage(List<byte> dataList)
        {
            MessageAnalyzeEventArgs messageAnalyzer = new MessageAnalyzeEventArgs();
            messageAnalyzer.Result = false;

            try
            {
                // 对报文进行预处理
                // 移除不匹配的报文头字节
                // 可以根据报文长度进行报文数据进行提取
                // 对公用的报文判断进行处理
                byte[] datas = PretreatMessage(dataList);

                if (datas != null)
                {
                    if (this.analyzer != null)
                    {
                        try
                        {
                            analyzer.Anaylzer(datas, messageAnalyzer);
                        }
                        catch (Exception ex)
                        {
                            analyzer.Exception = ex;
                        }
                        analyzer.Success = messageAnalyzer.Success;
                        analyzer.Result = messageAnalyzer.Result;
                    }
                }
            }
            catch (Exception e)
            {
                if (this.analyzer != null)
                {
                    this.analyzer.Exception = e;
                }
            }
        }

        protected override T Receive<T>(int timeout, MessageAnalyzerDelegate messageAnalyzer)
        {
            //报文分析对象
            FrameAnalyzer analyzer = new FrameAnalyzer { Anaylzer = messageAnalyzer };

            lock (receiveLock)
            {
                try
                {
                    this.analyzer = analyzer;
                    DateTime startTime = DateTime.Now;

                    while (true)
                    {
                        if (this.mydataList.Count >= 1)
                        {
                            // 解析报文
                            AnalyzeMessage(this.mydataList);

                        }
                        if (timeout > 0 && (DateTime.Now - startTime).TotalMilliseconds > timeout)
                        {
                            throw new TimeoutException("接收数据超时!");
                        }

                        if (analyzer.Exception != null)
                        {
                            throw analyzer.Exception;
                        }
                        else if (analyzer.Success)
                        {
                            if (analyzer.Result == null)
                                return default(T);

                            return (T)analyzer.Result;
                        }

                        System.Threading.Thread.Sleep(50);
                    }
                }
                finally
                {
                    this.analyzer = null;
                }
            }
        }
        private int UdpBindPort;
        private UdpClient Client;
        private string szBlt = "1200,e,8,1";
        private IPEndPoint Point = new IPEndPoint(IPAddress.Parse("193.168.18.1"), 10003);
        private IPEndPoint localPoint = null;
        protected FrameAnalyzer analyzer = null;
        private object receiveLock = new object();
        public static int SendType = 1;

        public void UDPClient(string Ip, int BindPort, int RemotePort)
        {
            Point.Address = IPAddress.Parse(Ip);
            Point.Port = RemotePort;
            UdpBindPort = LocalPortTo2011Port(BindPort);//转换端口成2018端口
            localPoint = new IPEndPoint(IPAddress.Parse(GetSubnetworkIP("193.168.18.1")), UdpBindPort);
        }

        /// <summary>
        /// 本地通道转换成2018端口:20000 + 2 * (port - 1);
        /// 数据端口，设置端口在数据端口的基础上+1
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        private int LocalPortTo2011Port(int port)
        {
            return 20000 + 2 * (port - 1);
        }
        public void UDPClient(string Ip, int BindPort, int BasePort, int RemotePort)
        {
            Point.Address = IPAddress.Parse(Ip);
            Point.Port = RemotePort;
            UdpBindPort = BasePort + 2 * (BindPort - BasePort - 1);//转换端口成2018端口
            localPoint = new IPEndPoint(IPAddress.Parse(GetSubnetworkIP(Ip)), UdpBindPort);
        }

        private static string GetSubnetworkIP(string targetIP)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\services\Tcpip\Parameters\Interfaces", RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.ReadKey);
            uint iTarget = IPToUint(targetIP);
            foreach (string keyName in key.GetSubKeyNames())
            {
                try
                {
                    RegistryKey tmpKey = key.OpenSubKey(keyName);
                    string[] ip = tmpKey.GetValue("IPAddress") as string[];
                    if (ip == null) continue;
                    string[] subnet = tmpKey.GetValue("SubnetMask") as string[];
                    for (int i = 0; i < ip.Length; i++)
                    {
                        IPAddress local = IPAddress.Parse(ip[i]);
                        if (local.IsIPv6SiteLocal)
                            continue;

                        uint iIP = IPToUint(ip[i]);
                        uint iSub = IPToUint(subnet[i]);

                        if ((iIP & iSub) == (iTarget & iSub))
                        {
                            return ip[i];
                        }
                    }
                }
                catch
                {
                }
            }
            return "127.0.0.1";

        }

        private static uint IPToUint(string ipAddress)
        {
            string[] strs = ipAddress.Trim().Split('.');
            byte[] buf = new byte[4];

            for (int i = 0; i < strs.Length; i++)
            {
                buf[i] = byte.Parse(strs[i]);
            }
            Array.Reverse(buf);

            return BitConverter.ToUInt32(buf, 0);
        }

        /// <summary>
        /// 接收端口发送回来的数据
        /// </summary>
        public void ReceivePortData()
        {

            if (Client == null)
                return;
            while (true)
            {
                Thread.Sleep(10);
                if (Client.Client.Poll(-1, System.Net.Sockets.SelectMode.SelectRead))
                {
                    if (Client.Available > 0)
                    {
                        byte[] buf = new byte[0];
                        buf = Client.Receive(ref Point);
                        // 解析报文
                        AnalyzeMessage(buf.ToList());


                    }
                }
            }
        }

        public bool SendCmd(ref byte[] buffer)
        {
            try
            {
                SendData(ref buffer, true);
                return true;
            }
            catch { return false; }
        }


        public override void Config(List<CommSerialPortSettings> serialPortSettings, List<CommSocketSettings> socketSettings)
        {

        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="port"></param>
        /// <param name="sendPacket"></param>
        /// <param name="recvPacket"></param>
        /// <returns></returns>
        public bool SendData(int port, SendPacket sendPacket, RecvPacket recvPacket)
        {
            bool bResult = false;
            string portName = GetPortNameByPortNumber(port);

            bResult = SockPool.Instance.Send(portName, sendPacket, recvPacket);

            return bResult;
        }
        /// <summary>
        /// 注册端口[UDP端口]
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="RemotePort">端口10003,10004</param>
        /// <param name="strSetting">端口参数</param>
        /// <param name="IP">串口服务器IP</param>
        public void RegisterPort(int port, int RemotePort, string strSetting, string IP,int BindPort, int maxWaittime, int waitSencondsPerByte)
        {
            System.Net.IPAddress ipa = System.Net.IPAddress.Parse(IP);
            string portName = GetPortNameByPortNumber(port);
            //注册数据端口
            SockPool.Instance.AddUdpSock(portName, ipa, RemotePort, port,BindPort, maxWaittime, waitSencondsPerByte);
            SockPool.Instance.UpdatePortSetting(portName, strSetting);
        }

        /// <summary>
        /// 根据端口号获取端口名
        /// </summary>
        /// <param name="port">端口号</param>
        /// <returns>端口名</returns>
        public string GetPortNameByPortNumber(int port)
        {
            return string.Format("Port_{0}", port);
        }


        public new bool Open()
        {
            //throw new NotImplementedException();
            return true;
        }
    }
}
