using System;
using System.Collections.Generic;

using System.Text;
using VerificationEquipment.Commons;

namespace Frontier.MeterVerification.DeviceInterface
{
    /// 连接操作接口
    /// <summary>
    /// 连接操作接口
    /// </summary>
    public interface IConnect
    {
        /// 联机
        /// <summary>
        /// 联机
        /// </summary>
        void Connected(MeterPosition[] meterPositions);

        /// <summary>
        /// 串口初始化
        /// </summary>
        void Connected(int meterPositionCount);

        /// 关闭联机
        /// <summary>
        /// 关闭联机
        /// </summary>
        void Closed();
    }
}
