using System;
using System.Collections.Generic;

using System.Text;
using System.Drawing;

namespace Frontier.MeterVerification.DeviceInterface
{
    /// 状态灯控制
    /// <summary>
    /// 状态灯控制
    /// </summary>
    public interface ILight
    {
        /// 控制状态灯颜色
        /// <summary>
        /// 控制状态灯颜色
        /// </summary>
        /// <param name="color">颜色</param>
        /// <param name="isFlash">是否闪烁</param>
        void Light(Color color, bool isFlash);
    }
}
