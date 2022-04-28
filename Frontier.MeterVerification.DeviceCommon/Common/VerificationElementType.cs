using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.DeviceCommon
{
    public enum VerificationElementType
    {
        /// 电能误差试验
        /// </summary>
        基本误差试验 = 07,
        /// <summary>
        /// 日计时误差试验
        /// </summary>
        日计时误差试验 = 08,
        /// <summary>
        /// 需量周期误差试验
        /// </summary>
        需量周期误差试验 = 09,
        /// <summary>
        /// 电能走字试验
        /// </summary>
        电能走字试验 = 10,
        /// <summary>
        /// 电表潜动试验
        /// </summary>
        电表潜动试验 = 11,
        /// <summary>
        /// 时段投切
        /// </summary>
        时段投切 = 12,
        /// <summary>
        /// 电表启动试验
        /// </summary>
        电表启动试验,
        /// <summary>
        /// 常数校核试验
        /// </summary>
        常数校核试验,
        /// <summary>
        /// 定数走字试验
        /// </summary>
        定数走字试验,
        /// <summary>
        /// 电量清零
        /// </summary>
        电量清零,
        /// <summary>
        /// 需量清零
        /// </summary>
        需量清零,
        /// <summary>
        /// 读电能底数
        /// </summary>
        读电能底数,
        /// <summary>
        /// 读需量底数
        /// </summary>
        读需量底数,
        /// <summary>
        /// 读取表地址
        /// </summary>
        读取表地址,
        /// <summary>
        /// 读生产编号
        /// </summary>
        读取表生产编号,

        /// <summary>
        /// 潜动日计时同时执行试验
        /// </summary>
        潜动日计时试验,

        翻转,
        压接,
        耐压试验,
        载波试验
    }
}
