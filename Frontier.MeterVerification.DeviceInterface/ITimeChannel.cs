using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.DeviceInterface
{
    /// <summary>
    /// 时基源
    /// </summary>
    public interface ITimeChannel
    {
    /// <summary>
    /// 设置时钟通道
    /// </summary>
    /// <param name="stdPulseType">通道类型0=时钟1=电能</param>
    /// <returns></returns>
        bool SetTimeChannel(int stdPulseType);
    }
}
