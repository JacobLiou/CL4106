using System;
using System.Collections.Generic;

using System.Text;
using VerificationEquipment.Commons;

namespace Frontier.MeterVerification.DeviceInterface
{
    /// 控制设备翻转
    /// <summary>
    /// 控制设备翻转
    /// </summary>
    public interface IControlReversalMotor
    {
        /// 控制设备翻转操作
        /// <summary>
        /// 控制设备翻转操作
        /// </summary>
        /// <param name="isReversal">true表示正转为检定状态，false表示反转</param>
        /// <param name="results">翻转结论</param>
        /// <param name="resultDescriptions">翻转结论描述</param>
        void EquipmentReversal(bool isReversal, out bool[] results, out string[] resultDescriptions);

        /// 单独某一行进行翻转过程
        /// <summary>
        /// 单独某一行进行翻转过程
        /// </summary>
        /// <param name="rowIndex">行索引，从1开始（1：最上一行）</param>
        /// <param name="IsReversal">true表示正转为检定状态，false表示反转</param>
        /// <returns></returns>
        bool EquipmentReversal(int rowIndex, bool isReversal);

        /// <summary>
        /// 表位手工翻转
        /// </summary>
        /// <param name="isReversal">true表示翻转成检定状态，false表示翻转成挂表状态</param>
        /// <param name="results">表位翻转情况，true表示翻转成功，false表示翻转失败</param>
        /// <param name="resultDescriptions">电能表表位翻转情况描述，数组长度与result一致，对应每一个表位的翻转情况</param>
        void EquipmentManualReversal(int meterPositionCount, bool isReversal, out bool[] results, out string[] resultDescriptions);

        /// <summary>
        /// 单个表位手工翻转
        /// </summary>
        /// <param name="meterPostion">表位号</param>
        /// <param name="isReversal">true表示翻转成检定状态，false表示翻转成挂表状态</param>
        /// <param name="results">表位翻转情况，true表示翻转成功，false表示翻转失败</param>
        /// <param name="resultDescriptions">电能表表位翻转情况描述，数组长度与result一致，对应每一个表位的翻转情况</param>
        void EquipmentMeterPositionManualReversal(int meterPositionCount, int[] meterPosition, bool isReversal, out bool[] results, out string[] resultDescriptions);

        /// <summary>
        /// 获取检定装置表位翻转状态
        /// </summary>
        /// <param name="meterNo">返回表位列表，对应检定台表位</param>
        /// <param name="reverseStatus">返回表位翻转状态</param>
        /// <returns></returns>
        void GetEquipmentReversalStatus(int meterPositionCount, out int[] meterNo, out MeterPositionReverseStatus[] reverseStatus);
    }
}
