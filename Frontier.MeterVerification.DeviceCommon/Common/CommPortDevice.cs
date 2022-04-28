using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.DeviceCommon
{
    /// <summary>
    /// 通信基类
    /// </summary>
    public abstract class CommPortDevice
    {
        /// <summary>
        /// 初始化串口、网口
        /// </summary>
        /// <param name="serialPortSettings"></param>
        /// <param name="socketPortSettings"></param>
        public abstract void Config(List<CommSerialPortSettings> serialPortSettings, List<CommSocketSettings> socketPortSettings);

        /// <summary>
        /// 打开
        /// </summary>
        public virtual void Open()
        {
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public virtual void Close()
        {
        }

        /// <summary>
        /// 发送报文
        /// </summary>
        /// <param name="buffer"></param>
        protected virtual void Send(byte[] buffer)
        {
        }

        /// <summary>
        /// 发送报文
        /// </summary>
        /// <param name="buffer"></param>
        protected virtual void Send(List<byte> buffer)
        {
        }

        /// <summary>
        /// 数据报文预处理(去掉报文头等)，
        /// 各设备可以根据自己特性进行解析，在调用前已经确保dataList.Count > 1
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        protected virtual byte[] PretreatMessage(List<byte> dataList)
        {
            return null;
        }

        /// <summary>
        /// 分析报文
        /// </summary>
        /// <param name="dataList"></param>
        protected virtual void AnalyzeMessage(List<byte> dataList)
        {
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="timeout">超时时间(单位:毫秒)</param>
        /// <param name="messageAnalyzer">报文解析代理</param>
        /// <returns></returns>
        protected virtual T Receive<T>(int timeout, MessageAnalyzerDelegate messageAnalyzer)
        {
            return default(T);
        }
    }
}
