using System;
using System.Collections.Generic;

using System.Text;
using VerificationEquipment.Commons;
using Frontier.MeterVerification.DeviceCommon;

namespace Frontier.MeterVerification.DeviceInterface
{
    /// <summary>
    /// 控制设备进行电压电流操作
    /// </summary>
    public interface IPower
    {
        /// <summary>
        /// 设置接线方式
        /// </summary>
        /// <param name="wiringMode">电能表接线方式</param>
        /// <param name="pulse">脉冲类型</param>
        /// <returns></returns>
        bool SetWiringMode(WiringMode wiringMode, Pulse pulse);

        /// <summary>
        /// 设置电压电流量程
        /// </summary>
        /// <param name="voltage">电压量程</param>
        /// <param name="current">电流量程</param>
        /// <returns></returns>
        bool SetRange(float voltage, float current);

        /// <summary>
        /// 设置交流电频率
        /// </summary>
        /// <param name="acFreq">频率</param>
        /// <returns></returns>
        bool SetFreq(float acFreq);

        /// <summary>
        /// 设置相位
        /// </summary>
        /// <param name="wiringMode">接线方式</param>
        /// <param name="factor">功率因数</param>
        /// <param name="capacitive">感性容性</param>
        /// <param name="pulse">脉冲类型</param>
        /// <param name="loadPhase">相别</param>
        /// <returns></returns>
        bool SetLoadPhase(WiringMode wiringMode, float factor, bool capacitive, Pulse pulse, LoadPhase loadPhase);

        /// <summary>
        /// 设置检定脉冲方式（基本误差、时钟、时段、需量）
        /// </summary>
        /// <param name="elementType">检定类型</param>
        /// <returns></returns>
        bool SetVerificationPulseType(VerificationElementType elementType);

        /// <summary>
        /// 升电压
        /// </summary>
        /// <param name="voltage">电压值</param>
        /// <param name="loadPhase">相别</param>
        /// <param name="wiringMode">接线方式</param>
        /// <returns></returns>
        bool RaiseVoltage(float voltage, LoadPhase loadPhase, WiringMode wiringMode, float factor, bool capacitive, Pulse pulse);

        /// <summary>
        /// 升单相电压
        /// </summary>
        /// <param name="voltage">电压值</param>
        /// <param name="loadPhase">相别</param>
        /// <param name="wiringMode">接线方式</param>
        /// <returns></returns>
        bool RaiseVoltage(float Avoltage, float Bvoltage, float Cvoltage, LoadPhase loadPhase, WiringMode wiringMode, float factor, bool capacitive, Pulse pulse);

        /// <summary>
        /// 升电流
        /// </summary>
        /// <param name="current">电流值</param>
        /// <param name="loadPhase">相别</param>
        /// <param name="wiringMode">接线方式</param>
        /// <returns></returns>
        bool RaiseCurrent(float current, LoadPhase loadPhase, WiringMode wiringMode, float factor, bool capacitive, Pulse pulse);

        /// <summary>
        /// 升电压，升电流
        /// </summary>
        /// <param name="voltage">电压值</param>
        /// <param name="current">电流值</param>
        /// <param name="loadPhase">相别</param>
        /// <param name="wiringMode">接线方式</param>
        /// <returns></returns>
        bool RaiseVotageCurrent(float voltage, float current, LoadPhase loadPhase, WiringMode wiringMode, float factor, bool capacitive, Pulse pulse);

        /// <summary>
        /// 升起不同胡三相电源
        /// </summary>
        /// <param name="UA"></param>
        /// <param name="IA"></param>
        /// <param name="Afactor"></param>
        /// <param name="Acapacitive"></param>
        /// <param name="UB"></param>
        /// <param name="IB"></param>
        /// <param name="Bfactor"></param>
        /// <param name="Bcapacitive"></param>
        /// <param name="UC"></param>
        /// <param name="IC"></param>
        /// <param name="Cfactor"></param>
        /// <param name="Ccapacitive"></param>
        /// <param name="wiringMode"></param>
        /// <param name="pulse"></param>
        void RaiseVoltageCurrentForJcjd(float UA, float IA, float Afactor, bool Acapacitive, float UB, float IB, float Bfactor, bool Bcapacitive,
            float UC, float IC, float Cfactor, bool Ccapacitive, VerificationEquipment.Commons.WiringMode wiringMode, Pulse pulse);

        /// <summary>
        /// 检定完成停止检定
        /// </summary>
        /// <returns></returns>
        bool StopVerification();
    }
}
