using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.DeviceInterface
{
    /// <summary>
    /// 耐压设备电源控制箱，控制耐压设备开关机
    /// </summary>
    public interface IControlResistancePower
    {
        /// 遥控打开耐压设备
        /// <summary>
        /// 遥控打开耐压设备
        /// </summary>
        /// <returns></returns>
        bool RemoteOpenResistanceEquipment();

        /// 遥控关闭耐压设备
        /// <summary>
        /// 遥控关闭耐压设备
        /// </summary>
        /// <returns></returns>
        bool RemoteCloseResistanceEquipment();
    }
}
