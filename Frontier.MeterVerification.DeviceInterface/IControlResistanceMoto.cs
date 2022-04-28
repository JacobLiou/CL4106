using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.DeviceInterface
{
    /// <summary>
    /// 电机模块耐压开关
    /// 【三辉专用】
    /// </summary>
    public interface IControlResistanceMoto
    {
        /// <summary>
        /// 设置电机为耐压模式或者退出耐压模式
        /// </summary>
        /// <param name="isResistance">false：退出耐压 true :进入耐压</param>
        /// <returns></returns>
        bool SetMotoContorlToResistance(bool isResistance);
    }
}
