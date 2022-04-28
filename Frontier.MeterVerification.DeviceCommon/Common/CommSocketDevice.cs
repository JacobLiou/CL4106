using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using VerificationEquipment.Commons;

namespace Frontier.MeterVerification.DeviceCommon
{
    public class CommSocketDevice : CommPortDevice
    {
        private Socket socket;
        private List<byte> dataList = new List<byte>();
        protected FrameAnalyzer analyzer = null;
        private object receiveLock = new object();
        private CommSocketSettings _commPortSettings = null;

        public bool IsAbort { get; set; }

        public event EventHandler<DataReceivedArgs> DataReceived;

        private void OnDataReceived(byte[] data)
        {
            if (DataReceived != null)
            {
                DataReceived(this, new DataReceivedArgs(data));
            }
        }

        /// <summary>
        /// 端口初始化
        /// </summary>
        /// <param name="commSocketSettings"></param>
        public void Config(CommSocketSettings commSocketSettings)
        {
            if (commSocketSettings != null)
            {
                _commPortSettings = commSocketSettings;
                if (socket != null)
                {
                    //socket.Dispose();
                    socket = null;
                }
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            else
            {
                if (socket != null)
                {
                    //socket.Dispose();
                    socket = null;
                }
                throw new PortException("未对设备进行串口参数设置");
            }
        }

        public override void Config(List<CommSerialPortSettings> serialPortSettings, List<CommSocketSettings> socketPortSettings)
        {
            foreach (var item in serialPortSettings)
            {
                //Config(item);//串口通信参数设置
            }

            foreach (var item in socketPortSettings)
            {
                this.Config(item);//网口设置
            }
        }

        public override void Open()
        {
            try
            {
                if (socket == null)
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
                if (!socket.Connected)
                {
                    socket.Connect(_commPortSettings.IP, _commPortSettings.Port1);
                    Thread thread = new Thread(new ThreadStart(ReceivePortData));
                    thread.IsBackground = true;
                    thread.Start();
                }
            }
            catch (Exception e)
            {
                if (socket != null)
                {
                    //socket.Dispose();
                    socket = null;
                }
                throw new PortException("打开设备网口失败", e);
            }
        }

        public override void Close()
        {
            try
            {
                if (socket.Connected)
                {
                    socket.Close();
                }
            }
            catch (Exception e)
            {
                throw new PortException("关闭设备网口失败", e);
            }
            finally
            {
                if (socket != null)
                {
                    //socket.Dispose();
                    socket = null;
                }
            }
        }

        protected override void Send(byte[] buffer)
        {
            if (buffer != null && buffer.Length > 0)
            {
                try
                {
                    if (!socket.Connected)
                    {
                        Open();
                    }
                    socket.Send(buffer);
                }
                catch (Exception e)
                {
                    throw new PortException(_commPortSettings.IP + " 发送数据失败。", e);
                }
            }
        }

        protected override void Send(List<byte> buffer)
        {
            Send(buffer.ToArray());
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
            lock (receiveLock)
            {
                //报文分析对象
                FrameAnalyzer analyzer = new FrameAnalyzer { Anaylzer = messageAnalyzer };
                try
                {
                    this.analyzer = analyzer;
                    DateTime startTime = DateTime.Now;

                    while (true)
                    {
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

        /// <summary>
        /// 接收端口发送回来的数据
        /// </summary>
        public void ReceivePortData()
        {
            while (true)
            {
                if (!socket.Connected)
                {
                    Open();
                    Thread.Sleep(3000);
                }
                else
                {
                    if (socket.Poll(500, System.Net.Sockets.SelectMode.SelectRead))
                    {
                        if (socket.Available > 0)
                        {
                            byte[] buf = new byte[socket.ReceiveBufferSize];
                            int rec = socket.Receive(buf, buf.Length, 0);
                            if (rec > 0)
                            {
                                byte[] data = new byte[rec];
                                Array.Copy(buf, data, rec);
                                // 解析报文
                                AnalyzeMessage(data.ToList());
                                OnDataReceived(buf);
                            }
                        }
                        else
                        {
                            Open();
                            Thread.Sleep(3000);
                        }
                    }
                    else
                    {
                        Open();
                        Thread.Sleep(3000);
                    }
                }
            }
        }


    }
}
