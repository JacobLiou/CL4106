using System;
using System.Collections.Generic;

using System.Text;
using Frontier.MeterVerification.DeviceCommon;

namespace Frontier.MeterVerification.DeviceInterface
{
    /// <summary>
    /// 科陆多功能控制板
    /// </summary>
    public interface IPowerSupply
    {
        /// <summary>
        /// 供电类型，耐压供电、载波供电、普通供电
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        bool SetPowerSupplyType(VerificationElementType elementType);
    }
}
