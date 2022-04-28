using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.DeviceInterface
{
    /// <summary>
    /// 设备状态获取
    /// </summary>
    public interface IEquipmentStatus
    {
        /// 获取检定装置状态
        /// <summary>
        /// 获取检定装置状态
        /// </summary>
        string GetEquipmentStatus();
    }
}
