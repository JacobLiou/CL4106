using System;
using System.Collections.Generic;
using System.Text;

namespace VerificationEquipment.Commons
{
    /// <summary>
    /// 表位信息
    /// </summary>
    public class MeterPosition
    {
        private int meterIndex;
        private Meter meter;
        private bool isVerify;

        /// <summary>
        /// 表位，从1开始编号
        /// </summary>
        public int MeterIndex
        {
            get
            {
                return meterIndex;
            }
            set
            {
                meterIndex = value;
            }
        }

        /// <summary>
        /// 电能表对象，如果表位没有挂表，此属性为null
        /// </summary>
        public Meter Meter
        {
            get
            {
                return meter;
            }
            set
            {
                meter = value;
            }
        }

        /// <summary>
        /// 是否检定
        /// </summary>
        public bool IsVerify
        {
            get
            {
                return isVerify;
            }
            set
            {
                isVerify = value;
            }
        }
    }
}
