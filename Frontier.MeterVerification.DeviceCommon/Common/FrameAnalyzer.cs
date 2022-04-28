using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.DeviceCommon
{
    /// <summary>
    /// 报文分析对象
    /// </summary>
    public class FrameAnalyzer
    {
        /// <summary>
        /// 报文解析代理
        /// </summary>
        public MessageAnalyzerDelegate Anaylzer { get; set; }

        /// <summary>
        /// 报文数据解析是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 对报文数据解析后得到的结果
        /// </summary>
        public object Result { get; set; }

        public Exception Exception { get; set; }
    }
}
