using System;
using System.Collections.Generic;
using System.Text;

namespace VerificationEquipment.Commons
{
    /// <summary>
    /// 表位翻转状态
    /// </summary>
    public enum MeterPositionReverseStatus
    {
        倾斜状态 = 0,
        直立状态,
        翻转未到位状态,
        故障状态
    }
}
