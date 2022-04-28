using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.DeviceInterface
{
    /// <summary>
    /// 耐压接口
    /// </summary>
    public interface IResistance
    {
        /// <summary>
        /// 初始化耐压设备
        /// </summary>
        /// <returns></returns>
        bool InitResistancee();
        /// <summary>
        /// 设置耐压方式
        /// </summary>
        /// <param name="resistanceType">耐压方式，对外壳、辅助端子等</param>
        /// <returns></returns>
        bool SetResistanceType(string resistanceType);
        /// <summary>
        /// 设置漏电流限制
        /// </summary>
        /// <param name="resistanceI">5mA</param>
        /// <returns></returns>
        bool SetResistanceIRangle(float resistanceI);
        /// <summary>
        /// 设置漏电流
        /// </summary>
        /// <param name="resistanceI">漏电流，3mA，5mA</param>
        /// <returns></returns>
        bool SetResistanceI(float resistanceI);
        /// <summary>
        /// 设置电压
        /// </summary>
        /// <param name="resistanceU">电压值，2kv，4kv</param>
        /// <returns></returns>
        bool SetResistanceU(float resistanceU);
        /// <summary>
        /// 设置耐压测试时间
        /// </summary>
        /// <param name="second">耐压时间（秒）</param>
        /// <returns></returns>
        bool SetResistanceTime(int second);
        /// <summary>
        /// 开始耐压
        /// </summary>
        /// <returns></returns>
        bool StartResistance();       
        /// <summary>
        /// 停止耐压
        /// </summary>
        /// <returns></returns>
        bool StopResistance();
         /// <summary>
        ///  耐压仪复位状态
        /// </summary>
        /// <returns></returns>
        bool RequestReset();
    }
}
