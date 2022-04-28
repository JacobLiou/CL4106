using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
namespace Frontier.MeterVerification.KLDevice
{

    /// <summary>
    /// 与台体的通讯连接
    /// </summary>
    internal class Connection
    {
        private object objSendLock = new object();

        /// <summary>
        /// 连接对象
        /// </summary>
        Comm2018Device connection = null;


        /// <summary>
        /// 初始化为UDP连接，并打开连接
        /// </summary>
        /// <param name="remoteIp">远程服务器IP</param>
        /// <param name="remotePort">远程服务器端口</param>
        /// <param name="localPort">本地监听端口</param>
        /// <param name="MaxWaitMSecond">指示最大等待时间</param>
        /// <param name="WaitSecondsPerByte">单字节最大等等时间</param>
        public Connection(IPAddress remoteIp, int remotePort, int localPort, int BindPort, int MaxWaitMSecond, int WaitSecondsPerByte)
        {
            connection = new Comm2018Device();
            connection.UDPClient(remoteIp.ToString(), localPort, BindPort, remotePort);
            connection.MaxWaitSeconds = MaxWaitMSecond;
            connection.WaitSecondsPerByte = WaitSecondsPerByte;
            //connection.Open();
        }
        

        /// <summary>
        /// 发送并且接收返回数据
        /// </summary>
        /// <param name="sendPack">发送数据包</param>
        /// <param name="recvPack">接收数据包</param>
        /// <returns></returns>
        public bool Send(SendPacket sendPack, RecvPacket recvPack)
        {

            //接到的数据存放在recvPack中，通过检测
            byte[] vData = sendPack.GetPacketData();
            if (vData == null)
                return false;
            Console.WriteLine("SendData:({0}) {1}", sendPack.GetPacketName(), BitConverter.ToString(vData, 0));
            if (!Send(ref vData, sendPack.IsNeedReturn))
            {
                return false;
            }
            if (sendPack.IsNeedReturn && vData.Length < 1)
            {
                Console.WriteLine("不需要回复数据");
                return false;
            }
            if (sendPack.IsNeedReturn && recvPack != null)
            {
                Console.WriteLine("收到数据{0}", BitConverter.ToString(vData, 0));
                bool ret = recvPack.ParsePacket(vData);
                return ret;
            }
            else
            {
                Console.WriteLine("无回复数据");
                return true;
            }
        }

        /// <summary>
        /// 更新端口对应的COMM口波特率参数
        /// </summary>
        /// <param name="szBlt">要更新的波特率</param>
        /// <returns>更新是否成功</returns>
        public bool UpdatePortSetting(string szBlt)
        {
            if (connection != null) connection.UpdateBltSetting(szBlt);
            return true;
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="vData"></param>
        /// <param name="isNeedReturn"></param>
        /// <returns></returns>
        public bool Send(ref byte[] vData, bool isNeedReturn)
        {
            if (connection == null) return false;
            lock (objSendLock)
            {
                if (connection != null)
                {
                    if (!connection.SendData(ref vData, isNeedReturn))
                        return false;
                }
                if (isNeedReturn && vData.Length < 1)
                {
                    return false;
                }
                return true;
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="vData"></param>
        /// <param name="isNeedReturn"></param>
        /// <returns></returns>
        public bool Send(ref byte[] vData)
        {
            if (connection == null) return false;
            lock (objSendLock)
            {
                if (connection != null)
                {
                    if (!connection.SendCmd(ref vData))
                        return false;
                }
                if (vData.Length < 1)
                {
                    return false;
                }
                return true;
            }
        }
        public bool Close()
        {
            if (connection == null) return true;
             connection.Close();
             return true;
        }
    }
}
