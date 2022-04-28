using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.DeviceCommon
{
    public class DataReceivedArgs : EventArgs
    {
        public DataReceivedArgs(byte[] data)
        {
            this.Data = data;
        }
        public byte[] Data;
    }
}
