using System;
using System.Collections.Generic;

using System.Text;
using VerificationEquipment.Commons;

namespace Frontier.MeterVerification.DeviceInterface
{
    /// 控制设备压接操作
    /// <summary>
    /// 控制设备压接操作
    /// </summary>
    public interface IControlPressMotor
    {
        /// 控制设备进行压接操作
        /// <summary>
        /// 控制设备进行压接操作
        /// </summary>
        /// <param name="isPress">true表示压接，false表示松开压接</param>
        /// <param name="results">压接结论</param>
        /// <param name="resultDescriptions">压接结论描述</param>
        void EquipmentPress(bool isPress, out bool[] results, out string[] resultDescriptions);

        /// 单个表位压接、松开过程 
        /// <summary>
        /// 单个表位压接、松开过程 
        /// </summary>
        /// <param name="meterIndex">表位号，从1开始</param>
        /// <param name="isPress">true：压接；false：松开</param>
        /// <returns></returns>
        bool EquipmentPress(int meterIndex, bool isPress);

        /// <summary>
        /// 表位手工压接
        /// </summary>
        /// <param name="isPress">true表示进行电能表压接，false表示松开压接</param>
        /// <param name="results">每一个表位的压接情况，true表示压接成功，false表示压接失败</param>
        /// <param name="resultDescriptions">电能表压接情况描述，数组长度与result一致，对应每一个表位的压接情况描述</param>
        void EquipmentManualPress(int meterPositionCount, bool isPress, out bool[] results, out string[] resultDescriptions);

        /// <summary>
        /// 单个表位手工压接
        /// </summary>
        /// <param name="meterPostion">表位号</param>
        /// <param name="isPress">true表示进行电能表压接，false表示松开压接</param>
        /// <param name="results">每一个表位的压接情况，true表示压接成功，false表示压接失败</param>
        /// <param name="resultDescriptions">电能表压接情况描述，数组长度与result一致，对应每一个表位的压接情况描述</param>
        void EquipmentMeterPositionManualPress(int meterPositionCount ,int[] meterPosition, bool isPress, out bool[] results, out string[] resultDescriptions);

        /// <summary>
        /// 获取检定装置表位压接状态
        /// </summary>
        /// <param name="meterNo">返回表位列表，对应检定台表位</param>
        /// <param name="pressStatus">返回表位压接状态</param>
        /// <returns></returns>
        void GetEquipmentPressStatus(int meterPositionCount, out int[] meterNo, out MeterPositionPressStatus[] pressStatus);

        /// <summary>
        /// 重压接
        /// </summary>
        /// <param name="meterPositions">表位</param>
        /// <param name="results">结论</param>
        /// <param name="resultDescriptions">结论描述</param>
        void EquipmentMeterPositionSecondPress(int[] meterPositions, out bool[] results, out string[] resultDescriptions);
    }
}
