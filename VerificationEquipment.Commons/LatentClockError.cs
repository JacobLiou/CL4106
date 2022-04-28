using System;
using System.Collections.Generic;

using System.Text;

namespace VerificationEquipment.Commons
{
    /// <summary>
    /// 潜动、日计时试验
    /// </summary>
    public class LatentClockError
    {
        /// <summary>
        /// 电压
        /// </summary>
        public float U;

        /// <summary>
        /// 交流电频率(Hz)
        /// </summary>
        public float AcFreq;

        /// <summary>
        /// 潜动时间
        /// </summary>
        public int LatentSecond;

        /// <summary>
        /// 时钟频率
        /// </summary>
        public float clockFreq;

        /// <summary>
        /// 日计时采样时间
        /// </summary>
        public int SampleSecond;

        /// <summary>
        /// 采样次数
        /// </summary>
        public int count;

        /// <summary>
        /// 是否执行日计时试验
        /// </summary>
        public bool isExectorClockError;
    }
}
