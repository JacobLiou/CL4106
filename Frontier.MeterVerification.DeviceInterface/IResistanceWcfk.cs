using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.DeviceInterface
{
    /// <summary>
    /// 耐压设备误差板
    /// </summary>
    public interface IResistanceWcfk
    {
        /// <summary>
        /// 读取耐压结论
        /// </summary>
        /// <param name="meterIndex">表位数组</param>
        /// <param name="result">结论</param>
        /// <param name="resultDescription">结论描述</param>
        /// <returns></returns>
        bool ReadResistanceResult(int[] meterIndex, out bool[] result, out string[] resultDescription);
        /// <summary>
        /// 设置漏电流限制
        /// </summary>
        /// <param name="resistanceI">5mA</param>
        /// <returns></returns>
        bool SetResistanceIWcfkRangle(float resistanceI);
    }
}
