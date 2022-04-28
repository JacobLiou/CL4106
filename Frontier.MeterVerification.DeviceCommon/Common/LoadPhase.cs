using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.DeviceCommon
{
    /// <summary>
    /// 负载相别,None代表合元，暂时先按南自的标准定义Value
    /// </summary>
    public enum LoadPhase : byte
    {
        A = 1,
        B = 2,
        C = 4,
        AC = 5,
        None = 7
    }
}
