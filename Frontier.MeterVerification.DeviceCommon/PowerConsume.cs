using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.DeviceCommon
{
    /// <summary>
    /// 功耗
    /// </summary>
    public class PowerConsume
    {
        /// <summary>
        /// 表位号
        /// </summary>
        public int meterIndex { get; set; }
        /// <summary>
        /// 电压有功功率消耗（W）
        /// </summary>
        public float VoltagePower { get; set; }
        /// <summary>
        /// 电压视在功率（VA）
        /// </summary>
        public float VoltageApparentPower { get; set; }
        /// <summary>
        /// 电流有功功率消耗（VA）
        /// </summary>
        public float CurrentPower { get; set; }
        /// <summary>
        /// 电流视在功率（VA）
        /// </summary>
        public float CurrentApparentPower { get; set; }

    }
}
