using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.DeviceCommon
{
    /// <summary>
    /// 电能表投切脉冲数
    /// </summary>
    public class PulseValue
    {
        public PulseValue()
        {
        }

        public PulseValue(int meterIndex, int count, DateTime dateTime)
        {
            MeterIndex = meterIndex;
            Count = count;
            DateTime = dateTime;
        }
        /// <summary>
        /// 表位号
        /// </summary>
        public int MeterIndex { get; set; }

        /// <summary>
        /// 表位脉冲个数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 脉冲发生时间
        /// </summary>
        public DateTime DateTime { get; set; }
    }
}
