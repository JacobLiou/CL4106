using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.DeviceInterface
{
    /// 获取状态接口
    /// <summary>
    /// 获取状态接口
    /// </summary>
    public interface IMeterPositionStatus
    {
        /// 获取表位状态
        /// <summary>
        /// 获取表位状态
        /// </summary>
        /// <param name="meterNo"></param>
        /// <param name="status"></param>
        void GetCurrentMeterNoStatus(out int[] meterNo, out string[] status);
    }
}
