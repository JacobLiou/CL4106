using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.DeviceInterface
{
    /// 电源控制箱
    /// <summary>
    /// 电源控制箱，控制检定设备开关机
    /// </summary>
    public interface IControlEquipmentPower
    {
        /// 遥控打开设备
        /// <summary>
        /// 遥控打开设备
        /// </summary>
        /// <returns></returns>
        bool RemoteOpenEquipment();

        /// 遥控关闭设备
        /// <summary>
        /// 遥控关闭设备
        /// </summary>
        /// <returns></returns>
        bool RemoteCloseEquipment();
    }
}
