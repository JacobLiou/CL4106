using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.DeviceCommon
{
    /// <summary>
    /// 通信端口异常
    /// </summary>
    public class PortException : Exception
    {
        public PortException(string message)
            : base(message)
        {
        }

        public PortException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
