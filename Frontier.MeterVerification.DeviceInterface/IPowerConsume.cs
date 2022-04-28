using System;
using System.Collections.Generic;

using System.Text;
using Frontier.MeterVerification.DeviceCommon;

namespace Frontier.MeterVerification.DeviceInterface
{
    /// <summary>
    /// 功耗接口
    /// </summary>
    public interface IPowerConsume
    {
        /// <summary>
        /// 获取功耗
        /// </summary>
        /// <param name="meterIndex">表位数组</param>
        /// <param name="powerConsume">功耗</param>
        /// <returns></returns>
        bool ReadPowerConsume(int[] meterIndex, out PowerConsume[] powerConsume);
    }
}
