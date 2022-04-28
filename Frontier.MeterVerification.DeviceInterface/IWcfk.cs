using System;
using System.Collections.Generic;

using System.Text;
using VerificationEquipment.Commons;
using Frontier.MeterVerification.DeviceCommon;

namespace Frontier.MeterVerification.DeviceInterface
{
    /// <summary>
    /// 误差板接口
    /// </summary>
    public interface IWcfk
    {
        /// <summary>
        /// 初始化误差板
        /// </summary>
        /// <returns></returns>
        bool InitWcfk();

        /// <summary>
        /// 设置检定类型
        /// </summary>
        /// <param name="elementType">检定类型</param>
        /// <returns></returns>
        bool SetVerificationType(VerificationElementType elementType);

        /// <summary>
        /// 设置脉冲通道
        /// </summary>
        /// <param name="elementType">检定类型</param>
        /// <param name="pulse">脉冲方式</param>
        /// <returns></returns>
        bool SetPulseChannel(VerificationElementType elementType, Pulse pulse);

        /// <summary>
        /// 设置校验圈数
        /// </summary>
        /// <param name="circle">校验圈数</param>
        /// <returns></returns>
        bool SetCircle(int circle);

        /// <summary>
        /// 设置日计时时间
        /// </summary>
        /// <param name="second">时间，单位（秒）</param>
        /// <returns></returns>
        bool SetClockErrorTime(int second, VerificationElementType elementType);

        /// <summary>
        /// 设置电能表常数
        /// </summary>
        /// <param name="meterConst"></param>
        /// <returns></returns>
        bool SetMeterConst(int meterConst);

        /// <summary>
        /// 设置误差 板常数
        /// </summary>
        /// <param name="meterConst"></param>
        /// <returns></returns>
        bool SetMeterConst(int meterConst, float voltage, float current);

        /// <summary>
        /// 开始执行试验
        /// </summary>
        /// <param name="elementType">检定类型</param>
        /// <returns></returns>
        bool StartVerification(VerificationElementType elementType);

        /// <summary>
        /// 设置脉冲方式
        /// </summary>
        /// <param name="pulse">脉冲类型</param>
        /// <returns></returns>
        bool SetPulseType(Pulse pulse);

        /// <summary>
        /// 获取表位误差数据
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <returns></returns>
        ErrorData ReadErrorData(int meterIndex, int sampleIndex);

        /// <summary>
        /// 获取多个表位误差数据
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <returns></returns>
        Dictionary<int, ErrorData> ReadErrorData(int[] meterIndex, int sampleIndex);

        /// <summary>
        /// 读取启动、潜动脉冲
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <returns></returns>
        PulseValue ReadPulse(int meterIndex);

        /// <summary>
        /// 启动误差板
        /// </summary>
        /// <returns></returns>
        bool StartWcfk();

        /// <summary>
        /// 获取电能表走字脉冲个数
        /// </summary>
        /// <returns></returns>
        //int[] GetMeterPulseCount(int[] meterIndex);

        /// <summary>
        /// 电流开路检测
        /// </summary>
        /// <param name="current">电流值（基本电流）</param>
        /// <param name="results">返回电流开路检测结论</param>
        void CurrentTest(float current, out bool[] results);

        /// <summary>
        /// 停止误差板
        /// </summary>
        /// <returns></returns>
        bool StopWcfk();

        /// <summary>
        /// 潜动开始
        /// </summary>
        void LatentStart();

        /// <summary>
        /// 读取潜动脉冲
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <returns></returns>
        PulseValue ReadLatentPulse(int meterIndex);

        /// <summary>
        /// 自动开路
        /// </summary>
        /// <param name="meterPositions"></param>
        /// <param name="results"></param>
        void CurrentShout(int[] meterPositions, out bool[] results);
    }
}
