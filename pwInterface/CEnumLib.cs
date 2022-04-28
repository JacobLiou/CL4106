using System;
using System.Collections.Generic;
using System.Text;


namespace pwInterface
{
    public enum enmDesktype
    {
        单相检定台 = 0,
        单相前装台 = 1,
        单相后装台 = 2,

        三相检定台 = 3,
        三相前装台 = 4,
        三相后装台 = 5,
    }


    #region 功率源输出枚举　
    /// <summary>
    /// 测量方式类型
    /// </summary>
    public enum enmClfs
    {
        三相四线有功 = 0,
        三相四线无功 = 1,
        三相三线有功 = 2,
        三相三线无功 = 3,
        二元件跨相90 = 4,
        二元件跨相60 = 5,
        三元件跨相90 = 6,
        单相 = 7,
        二相三线 = 8,
    }

    /// <summary>
    /// 元件类型
    /// </summary>
    public enum enmElement
    {
        H = 0,
        A = 1,
        B = 2,
        C = 3

    }

    /// <summary>
    /// 功率方向
    /// </summary>
    public enum enmsPowerFangXiang
    {
        /// <summary>
        /// 错误的、未赋值的
        /// </summary>
        Error = 0,

        /// <summary>
        /// 正向有功
        /// </summary>
        正向有功 = 1,

        /// <summary>
        /// 反向有功
        /// </summary>
        反向有功 = 2,

        /// <summary>
        /// 正向无功
        /// </summary>
        正向无功 = 3,

        /// <summary>
        /// 反向无功
        /// </summary>
        反向无功 = 4
    }

    /// <summary>
    /// 有无功
    /// </summary>
    public enum enmYwg
    {
        /// <summary>
        /// 有功
        /// </summary>
        P = 0,
        /// <summary>
        /// 无功
        /// </summary>
        Q = 1,
    }

    /// <summary>
    /// 输出电流基准类型
    /// </summary>
    public enum enmIType
    {
        额定电流 = 0,
        最大电流 = 1

    }

    /// <summary>
    /// CL191B标准脉冲类型
    /// </summary>
    public enum enmStdPulseType
    {
        标准时钟脉冲 = 0,
        标准电能脉冲 = 1
    }
    #endregion


    /// <summary>
    /// 工作任务类型
    /// </summary>
    public enum enmTaskType
    {
        电能误差 = 0,
        需量周期 = 1,
        时钟日误差 = 2,
        脉冲计数 = 3,
        对标 = 4,
    }

    /// <summary>
    /// 脉冲盒脉冲通道类型
    /// </summary>
    public enum enmChannelType
    {
        正向有功 = 0,
        反向有功 = 1,
        正向无功 = 2,
        反向无功 = 3,
        需量周期 = 4,
        时钟脉冲 = 5
    }


    /// <summary>
    /// 端子类型
    /// </summary>
    public enum enmPulseDzType
    {
        国网端子 = 0,
        南网端子 = 1,
    }
    /// <summary>
    /// 继电器类型
    /// </summary>
    public enum enmJDQType
    {
        内置继电器=0,
        外置继电器=1,
        外置隔离继电器=2,
    }


    /// <summary>
    /// 脉冲接口类型(脉冲盒0，光电头1)
    /// </summary>
    public enum enmPulseComType
    {
        脉冲盒 = 0,
        光电头 = 1
    }

    /// <summary>
    /// 共阳共阴
    /// </summary>
    public enum enmGyGyType
    {
        共阴 = 0,
        共阳 = 1,
    }



    /// <summary>
    /// 控制任务类型
    /// </summary>
    public enum enmControlTaskType
    {
        停止 = 0,
        启动 = 1
    }


    /// <summary>
    /// 系统当前状态
    /// </summary>
    public enum enmStatus
    {
        错误=0,
        空闲=1,
        联机=2,
        进行中=3,
        停止=4,
        结束=5,
        脱机=6,
    }
    /// <summary>
    /// 功率源当前状态
    /// </summary>
    public enum enmPowerStatus
    {
        错误 = 0,
        空闲 = 1,
        联机 = 2,
        联机失败=3,
        只输出电压 = 4,
        输出电压电流 = 5,
        升源失败 = 6,
        功率源关闭=7,
        脱机 = 8,
        脱机失败 = 9,

    }



    /// <summary>
    /// 检定项目编号
    /// </summary>
    public enum enmMeterPrjID
    {
        //  误差检定
        //	日计时误差检定
        //	分相供电测试（三相分别单独加电压，读取其他相电压）
        //	实时值测量（测量三相电压、电流、有功功率值）
        //	参数打包下载
        //	系统清零

        错误类型 = 0,

        RS485读生产编号 = 100,

        误差检定 = 200,

        日计时误差检定 = 300,

        分相供电测试 = 400,

        交流采样测试 = 500,

        读电能表底度 = 600,

        打包参数下载 = 700,

        系统清零 = 800,

    }

    /// <summary>
    /// 误差子项目编号
    /// </summary>
    public enum enmMeterWcItemID
    {
        错误类型 = 0,

        误差检定 = 1,

        日计时误差检定 = 2,

        高频检定 = 3,

        偏差检定 = 4,

    }


    /// <summary>
    /// 多功能子项目编号
    /// </summary>
    public enum enmMeterDgnItemID
    {
        错误类型 = 0,

        日计时误差 = 601,

        时段投切误差 = 602,

        需量周期误差 = 603,

    }


    /// <summary>
    /// 表位结论类型
    /// </summary>
    public enum enmMeterResult
    {
        合格 = 0,
        不合格 = 1,
        未检定 = 2,
        检定中 = 3,
        检定错误 = 4,


    }

    public enum enmSelfCheckItem
    {
        BIT0EEPROM=0,
        BIT1外部Flash,
        BIT2铁电存储器,
        BIT3ESAM,
        BIT4计量芯片,
        BIT5时钟芯片,
        BIT6拉闸继电器动作检测,
        BIT7时钟电池电压,
        BIT8抄表电池电压,
        BIT9显示按键上,
        BIT10显示按键中,
        BIT11显示按键下,
        BIT12编程开关,
        BIT13端盖检测,
        BIT14上盖检测,
        BIT15需量清零按键,
        BIT16载波模块忙信号,
        BIT17系统开关,
        BIT18插卡按键,
        BIT19计量A电压通道_单相表火线电压,
        BIT20计量B电压通道,
        BIT21计量C电压通道,
        BIT22计量A电流通道_单相表火线电流,
        BIT23计量B电流通道_单相表零线电流,
        BIT24计量C电流通道,
        未知错误,
    }


}
