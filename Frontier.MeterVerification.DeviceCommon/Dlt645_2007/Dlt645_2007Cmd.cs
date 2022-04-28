using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.DeviceCommon
{
    /// <summary>
    /// Dlt645_2007命令
    /// </summary>
    public enum Dlt645_2007Cmd : uint
    {
        /// <summary>
        /// 获取与设置日期
        /// </summary>
        日期 = 0x04000101,
        /// <summary>
        /// 获取与设置时间
        /// </summary>
        时间 = 0x04000102,
        /// <summary>
        /// 正向有功电能
        /// </summary>
        正向有功电能 = 0x0001FF00,
        /// <summary>
        /// 正向无功电能
        /// </summary>
        正向无功电能 = 0x0003FF00,
        /// <summary>
        /// 反向有功电能
        /// </summary>
        反向有功电能 = 0x0002FF00,
        /// <summary>
        /// 反向无功电能
        /// </summary>
        反向无功电能 = 0x0004FF00,
        /// <summary>
        /// 正向有功需量
        /// </summary>
        正向有功需量 = 0x01010000,
        /// <summary>
        /// 正向无功需量
        /// </summary>
        正向无功需量 = 0x01030000,
        /// <summary>
        /// 反向有功需量
        /// </summary>
        反向有功需量 = 0x01020000,
        /// <summary>
        /// 反向无功需量
        /// </summary>
        反向无功需量 = 0x01040000,
        /// <summary>
        /// 最大需量周期
        /// </summary>
        最大需量周期 = 0x04000103,
        /// <summary>
        /// 滑差时间
        /// </summary>
        滑差时间 = 0x04000104,
        /// <summary>
        /// 第一套日时段
        /// </summary>
        第一套日时段 = 0x04010001,
        /// <summary>
        /// 第二套日时段
        /// </summary>
        第二套日时段 = 0x04020001,
        /// <summary>
        /// 运行状态字3
        /// </summary>
        运行状态字3 = 0x04000503,

        /// <summary>
        /// 
        /// </summary>
        当前A相电压 = 0x02010100,
        /// <summary>
        /// 
        /// </summary>
        当前B相电压 = 0x02010200,
        /// <summary>
        /// 
        /// </summary>
        当前C相电压 = 0x02010300,
        /// <summary>
        /// 表厂常数加倍
        /// </summary>
        脉冲常数放大 = 0x04CC0509,
        /// <summary>
        /// 修改密码、清需量
        /// </summary>
        修改密码清需量 = 0x00000000,
        /// <summary>
        /// 身份认证
        /// </summary>
        身份认证 = 0x070000FF,
        /// <summary>
        /// 数据回抄
        /// </summary>
        数据回抄 = 0x078001FF,
        /// <summary>
        /// 控制命令密钥更新
        /// </summary>
        控制命令密钥更新 = 0x070201FF,
        /// <summary>
        /// 参数密钥更新
        /// </summary>
        参数密钥更新 = 0x070202FF,
        /// <summary>
        /// 身份认证密钥更新
        /// </summary>
        身份认证密钥更新 = 0x070203FF,
        /// <summary>
        /// 主控密钥更新
        /// </summary>
        主控密钥更新 = 0x070204FF
    }

    /// <summary>
    /// 控制码类型枚举
    /// </summary>
    public enum Dlt645_2007ControlCode : byte
    {
        /// <summary>
        /// 读表的数据
        /// </summary>
        读数据 = 0x11,
        /// <summary>
        /// 读后续帧
        /// </summary>
        读后续帧 = 0x12,
        /// <summary>
        /// 读通信地址
        /// </summary>
        读通信地址 = 0x13,
        /// <summary>
        /// 写数据
        /// </summary>
        写数据 = 0x14,
        /// <summary>
        /// 安全认证相关
        /// </summary>
        安全认证相关 = 0x03,
        /// <summary>
        /// 广播校时
        /// </summary>
        广播校时 = 0x08,
        /// <summary>
        /// 写通信地址
        /// </summary>
        写通信地址 = 0x15,
        /// <summary>
        /// 冻结数据
        /// </summary>
        冻结数据 = 0x16,
        /// <summary>
        /// 修改波特率
        /// </summary>
        修改波特率 = 0x17,
        /// <summary>
        /// 修改密码
        /// </summary>
        修改密码 = 0x18,
        /// <summary>
        /// 清除最大需量
        /// </summary>
        最大需量清零 = 0x19,
        /// <summary>
        /// 电量清零
        /// </summary>
        电表清零 = 0x1a,
        /// <summary>
        /// 事件清零
        /// </summary>
        事件清零 = 0x1b,
        /// <summary>
        /// 跳合闸、报警、保电
        /// </summary>
        拉合闸报警保电 = 0x1c,
        /// <summary>
        /// 多功能端子输出控制命令MulTerminalOut
        /// </summary>
        多功能端子输出控制命令 = 0x1d
    }

    /// <summary>
    /// 拉合闸报警保电命令枚举
    /// </summary>
    public enum Dlt645_2007OpenAlarm : int
    {
        /// <summary>
        /// 跳闸
        /// </summary>
        跳闸 = 0x1a,
        /// <summary>
        /// 合闸
        /// </summary>
        合闸 = 0x1b,
        /// <summary>
        /// 报警
        /// </summary>
        报警 = 0x2a,
        /// <summary>
        /// 报警解除
        /// </summary>
        报警解除 = 0x2b,
        /// <summary>
        /// 保电
        /// </summary>
        保电 = 0x3a,
        /// <summary>
        /// 保电解除
        /// </summary>
        保电解除 = 0x3b
    }
}
