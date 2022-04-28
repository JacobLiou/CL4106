using System;
using System.Collections.Generic;

using System.Text;
using Frontier.MeterVerification.DeviceCommon;
using VerificationEquipment.Commons;

namespace Frontier.MeterVerification.DeviceInterface
{
    /// <summary>
    /// 监视表
    /// </summary>
    public interface IMonitor
    {
        /// <summary>
        /// 获取标准表电量
        /// </summary>
        /// <param name="pulse">脉冲方式</param>
        /// <returns></returns>
        string GetStdMeterPower(Pulse pulse);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        MonitorData GetCurrentLoad();

        /// <summary>
        /// 读取检定装置当前负载
        /// </summary>
        /// <param name="U">电压</param>
        /// <param name="I">电流</param>
        /// <param name="P">有功功率</param>
        /// <param name="Q">无功功率</param>
        /// <param name="angle">角度</param>
        /// <param name="acFreq">频率</param>
        void GetCurrentLoad(out string[] U, out string[] I, out string[] P, out string[] Q, out string[] angle, out string acFreq);
    }
}
