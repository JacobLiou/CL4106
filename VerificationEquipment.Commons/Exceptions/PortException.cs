using System;
using System.Collections.Generic;
using System.Text;

namespace VerificationEquipment.Commons
{
    /// <summary>
    /// 端口操作失败
    /// </summary>
    public class PortException : ApplicationException
    {
        public PortException()
        {
        }

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
