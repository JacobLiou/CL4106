using System;
using System.Collections.Generic;
using System.Text;

namespace VerificationEquipment.Commons
{
    /// <summary>
    /// 设备无应答异常
    /// </summary>
    public class NoResponseException : ApplicationException
    {
        public NoResponseException()
        {
        }

        public NoResponseException(string message)
            : base(message)
        {
        }

        public NoResponseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
