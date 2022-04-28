using System;
using System.Collections.Generic;

using System.Text;
using VerificationEquipment.Commons;

namespace Frontier.MeterVerification.DeviceCommon
{
    /// <summary>
    /// 费率
    /// </summary>
    public class MeterRates
    {
        /// <summary>
        /// 时段
        /// </summary>
        public Phase Period;
        /// <summary>
        /// 时段开始时间
        /// </summary>
        public string PeriodTime;
        /// <summary>
        /// 时段默认时间
        /// </summary>
        public string DefaultTime;
        /// <summary>
        /// 第几套
        /// </summary>
        public int Series;
    }
}
