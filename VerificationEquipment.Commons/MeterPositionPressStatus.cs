using System;
using System.Collections.Generic;
using System.Text;

namespace VerificationEquipment.Commons
{
    /// <summary>
    /// 表位压接状态
    /// </summary>
    public enum MeterPositionPressStatus
    {
        未压接 = 0,
        已压接,
        压接未到位,
        故障
    }
}
