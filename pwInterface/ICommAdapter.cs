using System;
using System.Collections.Generic;
using System.Text;

namespace pwInterface
{
    /// <summary>
    /// 台体控制通信接口
    /// 用于台体各种设备控制
    /// </summary>
    public interface ICommAdapter
    {
        /// <summary>
        /// 表位数
        /// </summary>
        int BWCount { get;set;}

        /// <summary>
        /// 对象是否正常,如果为false，则可从LostMessage中读取失败信息
        /// </summary>
        bool Enabled { get;}
        
        /// <summary>
        /// 失败信息
        /// </summary>
        string LostMessage { get;}

        /// <summary>
        /// 停止当前任务
        /// </summary>
        /// <returns></returns>
        bool StopTask{get;set;}

        /// <summary>
        /// 联机
        /// </summary>
        /// <returns></returns>
        bool Link();

        /// <summary>
        /// 联机
        /// </summary>
        /// <param name="IsCL191BCL188L">是否带有CL191B及CL188L，True有，false没有</param>
        /// <returns></returns>
        bool Link(bool IsCL191BCL188L);


        /// <summary>
        /// 脱机
        /// </summary>
        /// <returns></returns>
        bool Unlink();



        /// <summary>
        /// 设置测试点(三相不平衡电压电流)
        /// </summary>
        /// <param name="ecs_Clfs">测量方式</param>
        /// <param name="sng_U">额定电压</param>
        /// <param name="sng_xUa">A相输出额定电压倍数</param>
        /// <param name="sng_xUb">B相输出额定电压倍数</param>
        /// <param name="sng_xUc">C相输出额定电压倍数</param>
        /// <param name="sng_I">额定电流</param>
        /// <param name="sng_Imax">最大电流</param>
        /// <param name="eit_IType">输出电流基准类型</param>
        /// <param name="sng_xIa">A相输出电流（是I还是IMax倍数則根据eit_IType决定）</param>
        /// <param name="sng_xIb">B相输出电流（是I还是IMax倍数則根据eit_IType决定）</param>
        /// <param name="sng_xIc">C相输出电流（是I还是IMax倍数則根据eit_IType决定）</param>
        /// <param name="eet_Element">元件</param>
        /// <param name="str_Glys">功率因数，负数表示反向，正数据表示正向</param>
        /// <param name="sng_Freq">频率</param>
        /// <param name="bln_NXX">是否逆相序 true=逆相序,false=正相序</param>
        /// <returns></returns>
        bool SetTestPoint(enmClfs ecs_Clfs, Single sng_U, Single sng_xUa, Single sng_xUb, Single sng_xUc,
                          Single sng_I, Single sng_Imax, enmIType eit_IType, Single sng_xIa, Single sng_xIb,
                          Single sng_xIc, enmElement eet_Element, string str_Glys, Single sng_Freq, bool bln_NXX);

        /// <summary>
        /// 设置测试点(三相不平衡电压电流、任意角度)
        /// </summary>
        /// <param name="int_Clfs">测量方式，0=PT4，1=PT3，2=QT4，3=QT3</param>
        /// <param name="sng_U">额定电压</param>
        /// <param name="sng_xUa">A相输出额定电压倍数</param>
        /// <param name="sng_xUb">B相输出额定电压倍数</param>
        /// <param name="sng_xUc">C相输出额定电压倍数</param>
        /// <param name="sng_I">额定电流</param>
        /// <param name="sng_Imax">最大电流</param>
        /// <param name="eit_IType">输出电流基准类型</param>
        /// <param name="sng_xIa">A相输出电流（是I还是IMax倍数則根据int_IType决定）</param>
        /// <param name="sng_xIb">B相输出电流（是I还是IMax倍数則根据int_IType决定）</param>
        /// <param name="sng_xIc">C相输出电流（是I还是IMax倍数則根据int_IType决定）</param>
        /// <param name="eet_Element">元件</param>
        /// <param name="sng_UaPhi">A相电压角度</param>
        /// <param name="sng_UbPhi">B相电压角度</param>
        /// <param name="sng_UcPhi">C相电压角度</param>
        /// <param name="sng_IaPhi">A相电流角度</param>
        /// <param name="sng_IbPhi">B相电流角度</param>
        /// <param name="sng_IcPhi">C相电流角度</param>
        /// <param name="sng_Freq">频率</param>
        /// <param name="bln_NXX">是否逆相序 true=逆相序,false=正相序</param>
        /// <returns></returns>
        bool SetTestPoint(enmClfs ecs_Clfs, Single sng_U, Single sng_xUa, Single sng_xUb, Single sng_xUc,
                          Single sng_I, Single sng_Imax, enmIType eit_IType, Single sng_xIa, Single sng_xIb,
                          Single sng_xIc, enmElement eet_Element, Single sng_UaPhi, Single sng_UbPhi, Single sng_UcPhi,
                           Single sng_IaPhi, Single sng_IbPhi, Single sng_IcPhi, Single sng_Freq);





        /// <summary>
        /// 设置测试点(统一电压电流角度)
        /// </summary>
        /// <param name="int_Clfs">测量方式，0=PT4，1=PT3，2=QT4，3=QT3</param>
        /// <param name="sng_U">额定电压</param>
        /// <param name="sng_xU">输出额定电压倍数</param>
        /// <param name="sng_I">额定电流</param>
        /// <param name="sng_Imax">最大电流</param>
        /// <param name="eit_IType">输出电流基准类型，0=额定电流，1=最大电流</param>
        /// <param name="sng_xI">输出电流（是I还是IMax倍数則根据int_IType决定）</param>
        /// <param name="eet_Element">元件</param>
        /// <param name="str_Glys">功率因数，负数表示反向，正数据表示正向</param>
        /// <param name="sng_Freq">频率</param>
        /// <param name="bln_NXX">是否逆相序 true=逆相序,false=正相序</param>
        /// <returns></returns>
        bool SetTestPoint(enmClfs ecs_Clfs, Single sng_U, Single sng_xU, Single sng_I, Single sng_Imax,
                          enmIType eit_IType, Single sng_xI, enmElement eet_Element, string str_Glys, Single sng_Freq, bool bln_NXX);



        /// <summary>
        /// 设置测试点(三相统一电压电流、任意角度)
        /// </summary>
        /// <param name="int_Clfs">测量方式，0=PT4，1=PT3，2=QT4，3=QT3</param>
        /// <param name="sng_U">额定电压</param>
        /// <param name="sng_xU">输出额定电压倍数</param>
        /// <param name="sng_I">额定电流</param>
        /// <param name="sng_Imax">最大电流</param>
        /// <param name="eit_IType">输出电流基准类型，0=额定电流，1=最大电流</param>
        /// <param name="sng_xI">输出电流（是I还是IMax倍数則根据int_IType决定）</param>
        /// <param name="eet_Element">元件</param>
        /// <param name="sng_UaPhi">A相电压角度</param>
        /// <param name="sng_UbPhi">B相电压角度</param>
        /// <param name="sng_UcPhi">C相电压角度</param>
        /// <param name="sng_IaPhi">A相电流角度</param>
        /// <param name="sng_IbPhi">B相电流角度</param>
        /// <param name="sng_IcPhi">C相电流角度</param>
        /// <param name="sng_Freq">频率</param>
        /// <param name="bln_NXX">是否逆相序 true=逆相序,false=正相序</param>
        /// <returns></returns>
        bool SetTestPoint(enmClfs ecs_Clfs, Single sng_U, Single sng_xU, Single sng_I, Single sng_Imax,
                          enmIType eit_IType, Single sng_xI, enmElement eet_Element, Single sng_UaPhi, Single sng_UbPhi, Single sng_UcPhi,
                           Single sng_IaPhi, Single sng_IbPhi, Single sng_IcPhi, Single sng_Freq);


        /// <summary>
        /// 设置测试点
        /// </summary>
        /// <param name="int_Clfs">测量方式</param>
        /// <param name="sng_U">输出电压</param>
        /// <param name="sng_I">输出电流</param>
        /// <param name="eet_Element">元件</param>
        /// <param name="str_Glys">功率因数，负数表示反向，正数据表示正向</param>
        /// <param name="sng_Freq">频率</param>
        /// <returns></returns>
        bool SetTestPoint(enmClfs ecs_Clfs, Single sng_U, Single sng_I, enmElement eet_Element, string str_Glys, Single sng_Freq);





        /// <summary>
        /// 关源
        /// </summary>
        /// <returns></returns>
        bool PowerOff();



        /// <summary>
        /// 设置电能误差参数
        /// </summary>
        /// <param name="epd_DzType">端子类型</param>
        /// <param name="ett_TaskType">任务类型</param>
        /// <param name="ect_ChannelNo">通道</param>
        /// <param name="ept_PulseType">脉冲接口</param>
        /// <param name="egt_GyG">共阴共阳</param>
        /// <param name="lng_AmConst">脉冲常数</param>
        /// <param name="lng_PulseTimes">圈数</param>
        /// <param name="iAmMeterPulseBS">倍数</param>
        /// <returns></returns>
        bool SetTaskParameter(enmPulseDzType epd_DzType, enmTaskType ett_TaskType, enmChannelType ect_ChannelNo, enmPulseComType ept_PulseType, enmGyGyType egt_GyG, long lng_AmConst, long lng_PulseTimes, byte iAmMeterPulseBS);

        /// <summary>
        /// 设置日计时误差参数
        /// </summary>
        /// <param name="epd_DzType">端子类型</param>
        /// <param name="ett_TaskType">任务类型</param>
        /// <param name="ect_ChannelNo">通道</param>
        /// <param name="ept_PulseType">脉冲接口</param>
        /// <param name="egt_GyG">共阴共阳</param>
        /// <param name="lng_AmConst">时钟频率</param>
        /// <param name="lng_PulseTimes">圈数</param>
        /// <returns></returns>
        bool SetTaskParameter(enmPulseDzType epd_DzType, enmTaskType ett_TaskType, enmChannelType ect_ChannelNo, enmPulseComType ept_PulseType, enmGyGyType egt_GyG, float sng_TimePL, int lng_PulseTimes);


        /// <summary>
        /// 设置电流回路
        /// </summary>
        /// <param name="int_LoopType">电流回路类型 0=第一回路(默认)，1=第二回路</param>
        /// <returns></returns>
        bool SetCurrentLoop(int int_LoopType);

        /// <summary>
        /// 设置被检表脉冲端子类型：0=国网端子，1=南网端子
        /// </summary>
        /// <param name="iPulseDzType">0=国网端子，1=南网端子</param>
        /// <returns></returns>
        bool SetMeterPulseDzType(int iPulseDzType);


        /// <summary>
        /// 设置各表位的通信开关
        /// </summary>
        /// <param name="int_NO">位表号(1-200)，特殊号：0xFF(255)=广播地址，0xEE(238)=偶数地址，0xDD(221)=奇数地址</param>
        /// <param name="bln_Open">是否打开，ture=打开，false=关闭</param>
        /// <returns></returns>
        bool SetAmmeterCmmSwitch(int int_NO, bool bln_Open);


        /// <summary>
        /// 读取标准数据信息
        /// </summary>
        /// <param name="str_Value">返回标准数据信息</param>
        /// <returns></returns>
        bool ReadStdInfo(ref string[] str_Value);

        /// <summary>
        /// 读取任务检定数据，数据内容根据所设置任务类型不同而不同  指定缓冲区
        /// </summary>
        /// <param name="int_FunNo">功能数据缓冲区号，检定类型：0x00电能误差、0x01需量误差、0x02日计时误差、0x03走字计数、0x04对标、0x05预付费功能检定</param>
        /// <param name="bln_Result">返回读取结果</param>
        /// <param name="str_Data">返回数据</param>
        /// <param name="int_Times">返回次数</param>
        /// <returns></returns>
        bool ReadTaskData(int int_FunNo,ref bool[] bln_Result, ref string[] str_Data, ref int[] int_Times);

        bool StopCalculate(enmTaskType ett_TaskType);

        /// <summary>
        /// 设置CL191通道
        /// </summary>
        /// <param name="iType">0=标准时钟脉冲、1=标准电能脉冲</param>
        /// <returns></returns>
        bool SetStdTimeChannel(int iType);

        /* 作废


        /// <summary>
        /// 读标准常数
        /// </summary>
        /// <param name="lng_StdConst">标准常数</param>
        /// <returns></returns>
        bool ReadStdConst(ref long lng_StdConst);

        /// <summary>
        /// 统一选择脉冲通道
        /// </summary>
        /// <param name="ect_ChannelNo">通道号</param>
        /// <param name="ept_PulseType">脉冲接口</param>
        /// <param name="egt_GyGy">脉冲类型</param>
        /// <returns></returns>
        bool SelectPulseChannel(enmChannelType ect_ChannelNo, enmPulseComType ept_PulseType, enmGyGyType egt_GyGy);

        /// <summary>
        /// 统一设置被检表常数和圈数
        /// </summary>
        /// <param name="lng_AmConst">电表常数</param>
        /// <param name="lng_PulseTimes">圈数</param>
        /// <returns></returns>
        bool SetDnWcParameter(long lng_AmConst, long lng_PulseTimes, long lStdPulseConst, float fStrandMeterP, byte iAmMeterPulseBS);

        /// <summary>
        /// 统一设置日计时参数
        /// </summary>
        /// <param name="sng_TimePL">被检表时钟频率</param>
        /// <param name="int_Times">检定圈数</param>
        /// <returns></returns>
        bool SetRjsParameter(Single sng_TimePL, int int_Times);


        /// <summary>
        /// 控制当前任务状态
        /// </summary>
        /// <param name="elt_Type">任务状态,0=停止，1=启动</param>
        /// <returns></returns>
        bool ControlTask(enmControlTaskType  elt_Type);

        /// <summary>
        /// 启动当前计算任务
        /// </summary>
        /// <returns></returns>
        bool StartTask();



        /// <summary>
        /// 读取电压电流的故障状态
        /// </summary>
        /// <param name="str_Result">返回各相电压电流故障结果,数据元素对应表位，内容：bit0表示A相电压 bit1表示A相电流 bit2表示B相电压 
        ///                          bit3表示B相电流 bit4表示C相电压 bit5表示C相电流,</param>
        /// <param name="int_WaitingTime">等待时间</param>
        /// <returns></returns>
        bool ReadIUHitch(ref string [] str_Result,int int_WaitingTime);


        /// <summary>
        /// 设置台体状态灯
        /// </summary>
        /// <param name="esl_LightType">状态灯类型</param>
        /// <returns></returns>
        bool SetStateLight(enmStateLightType esl_LightType);


        /// <summary>
        /// 设置各表位的通信调试开关
        /// </summary>
        /// <param name="int_NO">位表号(1-200)</param>
        /// <param name="bln_Open">是否打开，ture=打开，false=关闭</param>
        /// <returns></returns>
        bool SetAmmeterCmmDebug(int int_NO, bool bln_Open);

        /// <summary>
        /// 表位显示检定结果（灯状态）(适用2036)
        /// </summary>
        /// <param name="srt_Result">检定结果</param>
        /// <returns></returns>
        bool SetAmMeterResult(enmShowResultType[] srt_Result);

        /// <summary>
        /// 表位显示检定结果（灯状态）(适用2036)
        /// </summary>
        /// <param name="int_BwNo"></param>
        /// <param name="srt_Result"></param>
        /// <returns></returns>
        bool SetAmMeterResult(int int_BwNo, enmShowResultType srt_Result);

        /// <summary>
        /// 读控制状态(适用2036)
        /// </summary>
        /// <param name="int_State">控制状态字</param>
        /// <param name="bln_ErrCalResult">误差板联机状态</param>
        /// <returns></returns>
        bool ReadState(ref int[] int_State ,ref bool [] bln_ErrCalResult);

        /// <summary>
        /// 读控制版本(适用2036)
        /// </summary>
        /// <param name="str_Ver">版本号</param>
        /// <returns></returns>
        bool ReadVer(ref string str_Ver);

        /// <summary>
        /// 设置停止对
        /// </summary>
        /// <param name="int_Pulses">标脉冲个数</param>
        /// <returns></returns>
        bool SetStopPulses(int[] int_Pulses);
        /// <summary>
        /// 读GPS日期时间
        /// </summary>
        /// <param name="str_DateTime">日期时间,注：格式为yyyy-mm-dd hh:mm:ss</param>
        /// <returns></returns>
        bool ReadGPSDateTime(ref string str_DateTime);

        /// <summary>
        /// 读取温度湿度
        /// </summary>
        /// <param name="sng_Temp">返回温度值</param>
        /// <param name="sng_Hum">返回湿度值</param>
        /// <returns></returns>
        bool ReadTempHum(ref Single sng_Temp, ref Single sng_Hum);

        /// <summary>
        /// 设置需量参数
        /// </summary>
        /// <param name="lng_SpaceTime">需量脉冲间隔时间</param>
        /// <param name="int_Pulses">需量周期脉冲数</param>
        /// <returns></returns>
        //bool SetXLParameter(long[] lng_SpaceTime, int[] int_Pulses);

        ///// <summary>
        ///// 统一设置需量参数
        ///// </summary>
        ///// <param name="lng_SpaceTime">需量脉冲间隔时间</param>
        ///// <param name="int_Pulses">需量周期脉冲数</param>
        ///// <returns></returns>
        //bool SetXLParameter(long lng_SpaceTime, int int_Pulses);

        /// <summary>
        /// 各相各次谐波开关
        /// </summary>
        /// <param name="int_XSwitch">各相开关，数组元素值：0=不加谐波，1=加谐波，数组各元素：0=A相电压，1=B相电压，2=C相电压，3=A相电流，4=B相电流，5=C相电流</param>
        /// <param name="int_XTSwitch">各相各次开关,数组元素值：0=不加谐波，1=加谐波,数组各元素：各相（0-5），各次（0-64）</param>
        /// <returns></returns>
        bool SetHarmSwitch(int[] int_XSwitch, int[][] int_XTSwitch);

        /// <summary>
        /// 设置各相谐波含量
        /// </summary>
        /// <param name="eui_Type">相别</param>
        /// <param name="sng_Value">64次谐波含量</param>
        /// <param name="sng_Phase">64次谐波相位</param>
        /// <returns></returns>
        bool SetHarmValuePhase(enmUIXXType eui_Type, Single[] sng_Value, Single[] sng_Phase);
        /// <summary>
        /// 电压跌落\短时中断\逐渐变化\逐渐关机和启动
        /// </summary>
        /// <param name="evo_Type">电压操作类型</param>
        /// <returns></returns>
        bool SetVotFalloff(enmVolFallOff  evo_Type);






        /// <summary>
        /// 读取误差及脉冲数(主要用于在标准表计算误差的电能台)
        /// </summary>
        /// <param name="strErr">返回的误差值</param>
        /// <param name="lngTimes">返回的脉冲数</param>
        /// <returns>true 读取成功，false 读取失败</returns>
        bool ReadStdMeterErrorAndPulse(ref string[] strErr, ref long[] lngTimes);

        /// <summary>
        /// 读取标准表累计脉冲数。注意在锁定档位下有效。
        /// </summary>
        /// <param name="lngPulse">返回脉冲数</param>
        /// <returns>true 读取成功，false 读取失败</returns>
        bool ReadStdMeterPulses(ref long lngPulse);

        /// <summary>
        /// 读取表位温度故障（表位接线柱温度过高）
        /// </summary>
        /// <param name="tht_Result">返回故障</param>
        /// <returns></returns>
        bool ReadBwTempHitch(ref enmTempHitchType[] tht_Result);

        /// <summary>
        /// 读取表位温度故障（表位接线柱温度过高）
        /// </summary>
        /// <param name="int_BwIndex">表位号</param>
        /// <param name="int_Result">返回故障</param>
        /// <returns></returns>
        bool ReadBwTempHitch(int int_BwIndex, ref enmTempHitchType int_Result);

        /// <summary>
        /// 读取功耗
        /// </summary>
        /// <param name="int_BwIndex">表位号</param>
        /// <param name="int_Chancel">通道</param>
        /// <param name="flt_U">电压有效值</param>
        /// <param name="flt_I">电流有效值</param>
        /// <param name="flt_ActiveP">有功功率</param>
        /// <param name="flt_ApparentP">视在功率</param>
        /// <returns></returns>
        bool ReadPower(int int_BwIndex, int int_Chancel, ref float flt_U, ref float flt_I, ref float flt_ActiveP, ref float flt_ApparentP);

        ///// <summary>
        ///// 读取对色标状态
        ///// </summary>
        ///// <param name="str_Data"></param>
        ///// <returns></returns>
        //bool ReadStateDSB(ref string[] str_Data);

        ///// <summary>
        ///// 读取标准表累计电量（走字）
        ///// </summary>
        ///// <param name="sng_StdEnergy">返回标准表电量</param>
        ///// <returns></returns>
        //bool ReadStdEnergy(ref Single sng_StdEnergy);



        /// <summary>
        /// 设置80A以上大电流开关
        /// </summary>
        /// <param name="int_SwitchID">电流类型(小于80A 0；大于80A1-48表位开 1；大于80A1-48表位关 2)</param>
        /// <re turns></returns>
        bool SetPower(int int_SwitchID);
        */


    }
}
