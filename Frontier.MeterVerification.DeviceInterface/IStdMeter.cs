using System;
using System.Collections.Generic;

using System.Text;
using VerificationEquipment.Commons;
using Frontier.MeterVerification.DeviceCommon;

namespace Frontier.MeterVerification.DeviceInterface
{
    /// <summary>
    /// 标准表
    /// </summary>
    public interface IStdMeter
    {
        /// <summary>
        /// 设置标准表接线方式、电能指示
        /// </summary>
        /// <param name="wiringMode">接线方式</param>
        /// <param name="pulse">脉冲类型</param>
        /// <returns></returns>
        bool SetStdMeterWiringMode(VerificationElementType elementType, WiringMode wiringMode, Pulse pulse);
        
        /// <summary>
        /// 设置标准表常数
        /// </summary>
        /// <param name="voltage"></param>
        /// <param name="current"></param>
        /// <param name="wiringMode"></param>
        /// <returns></returns>
        bool SetStdMeterConst(float voltage, float current, WiringMode wiringMode);
       
        /// <summary>
        /// 获取标准表电能
        /// </summary>
        /// <returns></returns>
        float GetStdMeterEnergy();

        /// <summary>
        /// 读取标准表参数
        /// </summary>
         object ReadStdMeterParam(); 

        /// <summary>
        /// 设置标准表档位
        /// </summary>
        /// <param name="elementType">试验项</param>
        /// <param name="voltage">电压</param>
        /// <param name="current">电流</param>
        /// <returns></returns>
        //bool SetStdMeterGrade(VerificationElementType elementType, float voltage, float current);
        /// <summary>
        /// 获取标准表脉冲个数
        /// </summary>
        /// <returns></returns>
        //int GetStdMeterPulseCount();

        /// <summary>
        /// 停止标准表
        /// </summary>
        /// <returns></returns>
        bool StopStdMeter();
    }
}
