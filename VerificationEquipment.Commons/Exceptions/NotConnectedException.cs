using System;
using System.Collections.Generic;
using System.Text;

namespace VerificationEquipment.Commons
{
    /// <summary>
    /// 硬件设备未联机
    /// </summary>
    public class NotConnectedException : ApplicationException
    {
        public NotConnectedException()
        {
        }

        public NotConnectedException(string message)
            : base(message)
        {
        }

        public NotConnectedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
