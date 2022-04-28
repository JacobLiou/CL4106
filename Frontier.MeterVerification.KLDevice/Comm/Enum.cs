using System;
using System.Collections.Generic;
using System.Text;

namespace Frontier.MeterVerification.KLDevice
{
    /// <summary>
    /// 接收侦结果
    /// </summary>
    public enum RecvResult
    {
        /// <summary>
        ///  未解析过侦
        /// </summary>
        None,

        /// <summary>
        /// 未知
        /// </summary>
        Unknow,

        /// <summary>
        /// 无此命令
        /// </summary>
        NOCOMMAND,

        /// <summary>
        /// 正确
        /// </summary>
        OK,

        /// <summary>
        /// 帧结构错误
        /// </summary>
        FrameError,

        /// <summary>
        /// 校验错误
        /// </summary>
        CSError,

        /// <summary>
        /// 数据区域逻辑错误
        /// </summary>
        DataError
    }
    /// <summary>
    /// 电压电流相别
    /// </summary>
    public enum enmUIXXType
    {
        A相电压 = 0,
        B相电压 = 1,
        C相电压 = 2,
        A相电流 = 3,
        B相电流 = 4,
        C相电流 = 5
    }
    /// <summary>
    /// 电源错误等级
    /// </summary>
    [Serializable]
    public enum CusPowerErrorLevel
    {
        /// <summary>
        /// 提示信息，
        /// </summary>
        提示,

        /// <summary>
        /// 警告信息
        /// </summary>
        警告,

        /// <summary>
        /// 错误，服务端应该停止检定工作
        /// </summary>
        错误
    }
   
    /// <summary>
    /// 误差板当前状态
    /// </summary>
    public enum Cus_WuchaType
    {
        电能误差 = 0,
        需量周期误差 = 1,
        日计时误差 = 2,
        脉冲个数 = 3,
        对标状态 = 4
    }
    
    /// <summary>
    /// 电压跌落试验类型
    /// </summary>
    public enum Cus_VolFallOffType
    {

        电压跌落和短时中断 = 0,
        电压逐渐变化 = 1,
        逐渐关机和启动 = 2
    }
    public enum Cus_TestType
    {
        /// <summary>
        /// 错误类型
        /// </summary>
        CHECK_INVALID = 0,
        /// <summary>
        /// 首检
        /// </summary>
        CHECK_FIRST = 1,

        /// <summary>
        /// 周期检定
        /// </summary>
        CHECK_CYCLE = 2,
        /// <summary>
        /// 验收检定
        /// </summary>
        CHECK_ACCEPT = 3,

        /// <summary>
        /// 自定义
        /// </summary>
        CHECK_OTHER = 4
    }
    /// <summary>
    /// 标准脉冲类型
    /// </summary>
    public enum Cus_StdPulseType
    {
        标准时钟脉冲 = 0x00,
        标准电能脉冲 = 0xFF,
    }
    /// <summary>
    /// 标准表界面指示
    /// </summary>
    public enum Cus_StdMeterScreen
    {
        谐波柱图界面 = 0x09,
        谐波列表界面 = 0x0A,
        波形界面 = 0x0B,
        清除设置界面 = 0xFE
    }
    /// <summary>
    /// 电流档位
    /// </summary>
    public enum Cus_StdMeterIDangWei
    {
        档位100A = 0,
        档位50A = 0x01,
        档位20A = 0x02,
        档位10A = 0x03,
        档位5A = 0x04,
        档位2A = 0x05,
        档位1A = 0x06,
        档位05A = 0x07,
        档位02A = 0x08,
        档位01A = 0x09,
        档位005A = 0x0a,
        档位002A = 0x0b,
        档位001A = 0x0c
    }

    /// <summary>
    /// 电压档位
    /// </summary>
    public enum Cus_StdMeterVDangWei
    {
        档位480V = 1,
        档位240V = 2,
        档位120V = 3,
        档位60V = 4
    }
    /// <summary>
    /// 台体的对色标功能
    /// </summary>
    public enum Cus_SeiBiaoType
    {
        不支持,
        电压,
        电流
    }
    /// <summary>
    /// 脉冲接口类型
    /// </summary>
    public enum enmPulseComType
    {
        脉冲盒 = 0,
        光电头 = 1
    }
    /// <summary>
    /// 电能误差通道号
    /// </summary>
    public enum Cus_MeterWcChannelNo
    {
        正向有功 = 0,
        反向有功 = 1,
        正向无功 = 2,
        反向无功 = 3,
        需量 = 4,
        时钟 = 5
    }

    /// <summary>
    /// 多功能误差通道号
    /// </summary>
    public enum Cus_DgnWcChannelNo
    {
        电能误差 = 0,
        日计时脉冲 = 1,
        需量脉冲 = 2
    }

    /// <summary>
    /// 检定类型
    /// </summary>
    public enum Cus_CheckType
    {
        电能误差 = 0,
        需量误差 = 1,
        日计时误差 = 2,
        走字计数 = 3,
        对标 = 4,
        预付费功能检定 = 5,
        耐压实验 = 6,
        多功能脉冲计数试验 = 7

    }

    /// <summary>
    /// 光电头选择位
    /// </summary>
    public enum Cus_PulseType
    {
        脉冲盒 = 0,
        光电头 = 1
    }
    /// <summary>
    /// 功率元件
    /// </summary>
    public enum Cus_PowerYuanJiang
    {
        /// <summary>
        /// 错误的、未赋值的
        /// 
        /// </summary>
        Error = 0,

        /// <summary>
        /// 合元
        /// </summary>
        H = 1,

        /// <summary>
        /// A元
        /// </summary>
        A = 2,

        /// <summary>
        /// B元
        /// </summary>
        B = 3,

        /// <summary>
        /// C元
        /// </summary>
        C = 4,
    }
    #region 功率方向
    /// <summary>
    /// 功率方向
    /// </summary>
    public enum Cus_PowerFangXiang
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
    #endregion
    /// <summary>
    /// 表类型：电子式感应式
    /// </summary>
    public enum Cus_MeterType_DianziOrGanYing
    {
        /// <summary>
        /// 电子式
        /// </summary>
        DianZiShi = 1,

        /// <summary>
        /// 感应式
        /// </summary>
        GanYingShi = 2
    }
    
    /// <summary>
    /// 通讯选择
    /// </summary>
    public enum Cus_LightSelect
    {
        一对一模式485通讯 = 0,
        奇数表位485通讯 = 1,
        偶数表位485通讯 = 2,
        一对一模式红外通讯 = 3,
        奇数表位红外通讯 = 4,
        偶数表位红外通讯 = 5,
        切换到485总线 = 6       //电科院协议用
    }
    /// <summary>
    /// 警示灯类型
    /// </summary>
    public enum Cus_LightType
    {
        关灯 = 0,
        红灯 = 1,
        黄灯 = 2,
        绿灯 = 4
    }
    /// <summary>
    /// 电流档位
    /// </summary>
    public enum Cus_IChannelType
    {
        /// <summary>
        /// 100A档位
        /// </summary>
        档位100A = 0x01,

        /// <summary>
        /// 2A档位
        /// </summary>
        档位2A = 0x02
    }
    /// <summary>
    /// 共阳共阴
    /// </summary>
    public enum Cus_GyGyType
    {
        共阴 = 0,
        共阳 = 1,
    }
    /// <summary>
    /// 费率
    /// </summary>
    public enum Cus_FeiLv
    {
        /// <summary>
        /// 总
        /// </summary>
        总 = 0,

        /// <summary>
        /// 尖
        /// </summary>
        尖 = 1,

        /// <summary>
        /// 锋
        /// </summary>
        峰 = 2,

        /// <summary>
        /// 平
        /// </summary>
        平 = 3,

        /// <summary>
        /// 谷
        /// </summary>
        谷 = 4
    }   


    /// <summary>
    /// 测量方式
    /// 
    /// 三相四线有功 = 0,
    ///   三相四线无功 = 1,
    ///   三相三线有功 = 2,
    ///   三相三线无功 = 3,
    ///   二元件跨相90 = 4,
    ///   二元件跨相60 = 5,
    ///   三元件跨相90 = 6,
    /// </summary>
    public enum Cus_Clfs
    {
        /// <summary>
        /// 测量方式-三相四线
        /// </summary>
        三相四线 = 0,
        /// <summary>
        /// 测量方式-三相三线
        /// </summary>
        三相三线 = 1,
        /// <summary>
        /// 测量方式-二元件跨相90
        /// </summary>
        二元件跨相90 = 2,
        /// <summary>
        /// 测量方式-二元件跨相60
        /// </summary>
        二元件跨相60 = 3,
        /// <summary>
        /// 测量方式-三元件跨相90
        /// </summary>
        三元件跨相90 = 4,
        /// <summary>
        /// 测量方式-单相
        /// </summary>
        单相 = 5
    }
    /// <summary>
    /// 检定状态
    /// </summary>
    [Flags]
    public enum Cus_CheckStaute
    {
        未赋值的 = 0,
        检定 = 1,
        停止检定 = 2,
        调表 = 4,
        单步检定 = 8,
        录入完成 = 16
    }

    /// <summary>
    /// 电流的输出回路
    /// </summary>
    public enum Cus_BothIRoadType
    {
        第一个电流回路 = 0,
        第二个电流回路 = 1,
    }
    /// <summary>
    /// 电表电压回路选择
    /// </summary>
    public enum Cus_BothVRoadType
    {
        直接接入式 = 0,
        互感器接入式 = 1,
        本表位无电表接入 = 2
    }
    /// <summary>
    /// 试验类型
    /// </summary>
    public enum enmTaskType
    {
        电能误差 = 0,
        需量周期 = 1,
        时钟日误差 = 2,
        脉冲计数 = 3,
        对标 = 4,
        走字 = 5,
        设置预付费试验 = 6,
        设置底度对齐 = 7
    }
    /// <summary>
    /// 标准脉冲类型
    /// </summary>
    enum enmStdPulseType
    {
        标准时钟脉冲 = 0,
        标准电能脉冲 = 1
    }
}
