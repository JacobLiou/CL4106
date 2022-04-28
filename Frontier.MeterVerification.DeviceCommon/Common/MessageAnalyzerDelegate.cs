using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.DeviceCommon
{
    /// <summary>
    /// 报文解析代理
    /// </summary>
    /// <param name="datas">接收到的报文</param>
    /// <param name="e">报文解析结果</param>
    public delegate void MessageAnalyzerDelegate(byte[] datas, MessageAnalyzeEventArgs e);

    /// <summary>
    /// 报文解析
    /// </summary>
    public class MessageAnalyzeEventArgs : EventArgs
    {
        /// <summary>
        /// 报文数据解析是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 对数据解析后生成的结果
        /// </summary>
        public object Result { get; set; }
    }
}
