using System;
using System.Collections.Generic;
using System.Text;

namespace VerificationEquipment.Commons
{
    /// <summary>
    /// 打开485端口异常
    /// </summary>
    public class Open485Exception : ApplicationException
    {
        public Open485Exception()
        {
        }

        public Open485Exception(string message)
            : base(message)
        {
        }

        public Open485Exception(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
