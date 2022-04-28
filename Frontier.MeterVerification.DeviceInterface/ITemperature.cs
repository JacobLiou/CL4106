using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.DeviceInterface
{
    /// <summary>
    /// 温度
    /// </summary>
    public interface ITemperature
    {
        /// <summary>
        /// 获取表位温度
        /// </summary>
        /// <param name="meterIndex">表位数组</param>
        /// <param name="temperature">温度</param>
        void GetTemperature(int[] meterIndex, out float[] temperature);
    }
}
