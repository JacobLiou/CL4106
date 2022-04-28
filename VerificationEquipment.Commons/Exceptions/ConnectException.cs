using System;
using System.Collections.Generic;
using System.Text;

namespace VerificationEquipment.Commons
{
    /// <summary>
    /// 联机
    /// </summary>
    public class ConnectException : ApplicationException
    {
        public ConnectException()
        {
        }

        public ConnectException(string message)
            : base(message)
        {
        }

        public ConnectException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
