using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.DeviceCommon
{
    /// <summary>
    /// 误差数据对象
    /// </summary>
    public class ErrorData
    {
        public ErrorData()
        { 
        }

        public ErrorData(int meterNo, int sampleIndex, string errorValue)
        {
            MeterNo = meterNo;
            SampleIndex = sampleIndex;
            ErrorValue = errorValue;
        }
        /// <summary>
        /// 表位号
        /// </summary>
        public int MeterNo { get; set; }

        /// <summary>
        /// 第几次采样
        /// </summary>
        public int SampleIndex { get; set; }

        /// <summary>
        /// 误差值
        /// </summary>
        public string ErrorValue { get; set; }
    }
}
