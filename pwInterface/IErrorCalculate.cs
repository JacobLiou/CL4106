/****************************************************************************

    误差版接口类
    刘伟 2009-10

*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace pwInterface
{   
    /// <summary>
    /// 误差计算板通信协议接口
    /// </summary>
    public interface IErrorCalculate
    {
        #region 基本成员

        /// <summary>
        /// 误差计算板地址
        /// </summary>
        string ID { get;set;}

        /// <summary>
        /// 误差计算板串口
        /// </summary>
        ISerialport ComPort { get;set;}


        /// <summary>
        /// 通道选择，主要用于控制CL2011设备的，有效值1-4,
        /// 控制PC串口的RTSEnable和DTREnable的4种组合
        /// CL2018-1串口无效。
        /// </summary>
        int Channel { get;set;}


        /// <summary>
        /// 特波率
        /// </summary>
        string Setting { set;}

        /// <summary>
        /// 失败信息
        /// </summary>
        string LostMessage { get;}

        /// <summary>
        /// 设置误差板表位号
        /// </summary>
        /// <param name="int_List">此管理的表位误差板列表</param>
        /// <returns></returns>
        bool SetECListTable(int[] int_List);

        /// <summary>
        /// 自动适应标准表端口号
        /// </summary>
        /// <param name="mySerialPort">端口数组</param>
        /// <returns>true 适应成功，false 适应失败</returns>
        int AdaptCom(ISerialport[] mySerialPort);


        /// <summary>
        /// 误差板联机
        /// </summary>
        /// <param name="bln_Result">联机结论</param>
        /// <returns>true 联机成功,false 联机失败</returns>
        bool Link(ref bool[] bln_Result);

        /// <summary>
        /// 误差板联机
        /// </summary>
        /// <param name="int_Num">表位</param>
        /// <param name="bln_Result">联机结论</param>
        /// <returns></returns>
        bool Link(int int_Num);
        #endregion

        #region F1H：	设置电能误差检定时脉冲参数：常数、圈数


        /// <summary>
        /// 设置被检表脉冲常数和检定圈数
        /// </summary>
        /// <param name="lAmMeterPulseConst">被检表脉冲常数</param>
        /// <param name="iPulseCount">检定圈数</param>
        /// <param name="lStdPulseConst">标准表当前常数，需要从标准表读取</param>
        /// <param name="fStdP">标准表当前功率，需要从标准表读取</param>
        /// <param name="iAmMeterPulseBS">被检表脉冲放大系数（-128~127），默认为1不放大也不缩小1</param>
        /// <returns></returns>
        bool SetDnWcrPara(long lAmMeterPulseConst, long iPulseCount, long lStdPulseConst, float fStrandMeterP, byte iAmMeterPulseBS);
        #endregion

        #region F3H：	设置日计时误差检定时钟频率及需量周期误差检定时间

        /// <summary>
        /// 日计时误差参数设置：频率，圈数
        /// </summary>
        /// <param name="sglMeterHz">被检表时间频率</param>
        /// <param name="iPulse">脉冲数</param>
        /// <returns></returns>
        bool SetTimePara( float fMeterHz,int iPulse);

        /// <summary>
        /// 日计时误差参数设置：表位号，常数，频率，圈数
        /// </summary>
        /// <param name="int_Num">表位号：广播标志(0xFFH)，偶位(0xEEH)，奇位(0xDDH)</param>
        /// <param name="lStdTimeConst">标准时钟脉冲常数</param>
        /// <param name="flt_MeterHz">被检时钟频率</param>
        /// <param name="int_Pulse">被检脉冲个数</param>
        /// <returns></returns>
        bool SetTimePara(int iNum, long lStdTimeConst, float fMeterHz, int iPulse);
        #endregion

        #region A7H：	选择被检脉冲通道及检定类型

        /// <summary>
        /// 选择脉冲通道
        /// </summary>
        /// <param name="iPulseChannel">脉冲通道,0=P+,1=P-,2=Q+,3=Q-,4=需量,5=时钟</param>
        /// <param name="iChannelType">通道类型,0=脉冲盒,1=光电头</param>
        /// <param name="iPulseType">脉冲类型,0=共阴,1=共阳</param>
        /// <returns></returns>
        bool SelectPulseChannel(int[] iPulseChannel, int[] iChannelType, int[] iPulseType);

        /// <summary>
        /// 选择脉冲通道
        /// </summary>
        /// <param name="iNum">表位</param>
        /// <param name="iPulseChannel">脉冲通道,0=P+,1=P-,2=Q+,3=Q-,4=需量,5=时钟</param>
        /// <param name="iChannelType">通道类型,0=脉冲盒,1=光电头</param>
        /// <param name="iPulseType">脉冲类型,0=共阴,1=共阳</param>
        /// <returns></returns>
        bool SelectPulseChannel(int iNum, int iPulseChannel, int iChannelType, int iPulseType);
        #endregion

        #region ACH：	设置表位开关

        /// <summary>
        /// 控制表位通信口开关
        /// </summary>
        /// <param name="int_Num">表位号(255所有表位)</param>
        /// <param name="bln_Open">是否打开，true=打开，flase=关闭</param>
        /// <returns></returns>
        bool SetCommSwitch(int int_Num, bool bln_Open);
        #endregion

        #region ADH：	选择被表脉冲端子类型
        /// <summary>
        /// 选择被表脉冲端子类型：0=国网端子，1=南网端子
        /// </summary>
        /// <param name="iPulseDzType">0=国网端子，1=南网端子</param>
        /// <returns></returns>
        bool SetMeterPulseDzType(int iPulseDzType);
        #endregion


        #region AFH：	用于双回路检定时，选择其中的某一路作为电流的输出回路
        /// <summary>
        /// 回路切换
        /// </summary>
        /// <param name="int_DL_type">电流回路： 0一回路 1二回路</param>
        /// <returns></returns>
        bool SetDLSwitch(int int_DL_type);
        #endregion

        #region B1H：	启动计算功能指令

        /// <summary>
        /// 启动误差板计算功能 带任务类型参数
        /// </summary>
        /// <param name="byt_TaskType">检定类型：0x00电能误差、0x01需量误差、0x02日计时误差、0x03走字计数、0x04对标、0x05预付费功能检定</param>
        /// <returns></returns>
        bool StartCalculate(int byt_TaskType);
        #endregion

        #region B2H：	停止检定功能指令

        /// <summary>
        /// 停止误差板计算功能
        /// </summary>
        /// <param name="byt_TaskType">检定类型：0x00电能误差、0x01需量误差、0x02日计时误差、0x03走字计数、0x04对标、0x05预付费功能检定</param>
        /// <returns></returns>
        bool StopCalculate(int byt_TaskType);
        #endregion


        #region C2H：	清除表位状态
        /// <summary>
        /// 清理接线故障状态，
        /// </summary>
        /// 状态类型分为四种：接线故障状态01、预付费跳闸状态02、报警信号状态03、对标状态04
        /// <returns></returns>
        bool ClearBwState(int int_StateType);
        #endregion

        #region C0H：	读取表位各类型误差及各种状态

        #region 读误差板数据

        /// <summary>
        /// 读取所有表位误差板数据
        /// </summary>
        /// <param name="byt_TaskType">检定类型：0x00电能误差、0x01需量误差、0x02日计时误差、0x03走字计数、0x04对标、0x05预付费功能检定</param>
        /// <param name="bln_Result">对应表位是否获得数据</param>
        /// <param name="int_Time">对应表位的误差次数</param>
        /// <param name="str_Data">对应表位的误差值</param>
        /// <returns></returns>
        bool ReadData(int byt_TaskType, ref bool[] bln_Result, ref int[] int_Time, ref string[] str_Data);

        /// <summary>
        /// 读取单一表位误差板数据
        /// </summary>
        /// <param name="byt_TaskType">检定类型：0x00电能误差、0x01需量误差、0x02日计时误差、0x03走字计数、0x04对标、0x05预付费功能检定</param>
        /// <param name="int_Num">表位号(255所有表位)FF 全表位，0xEE,偶数，0xDD奇数</param>
        /// <param name="int_Time">误差次数</param>
        /// <param name="str_Data">误差值</param>
        /// <returns></returns>
        bool ReadData(int byt_TaskType, int int_Num, ref bool bln_Result, ref int int_Times, ref string str_Data);
        #endregion

        #region 读误差板状态
        /// <summary>
        /// 获得误差板状态信息 接线故障状态（Bit0）、预付费跳闸状态（Bit1）、报警信号状态（Bit2）、对标状态（Bit3）
        /// 一个字节（八位）
        /// 接线故障状态（Bit0）、预付费跳闸状态（Bit1）、报警信号状态（Bit2）、
        /// 对标状态（Bit3）的参数分别由一个字节中的Bit0、Bit1、Bit2、Bit3标示，为1则表示该表位有故障/跳闸/报警/对标完成，
        /// 为0则表示正常/正常/正常/未完成对标。
        /// </summary>不足八位，高位补零    by Zhoujl
        /// <param name="bln_Result">是否获得状态</param>
        /// <param name="str_Data">对应表位的状态</param>
        /// <returns></returns>  
        bool ReadData(ref bool[] bln_Result, ref string[] str_Data);
        #endregion


        #endregion


        #region C3H：	读取表位前10次各类型误差及当前各种状态
        /// <summary>
        /// 读取10次误差板数据(读十个)
        /// </summary>
        /// <param name="byt_TaskType">检定类型：0x00电能误差、0x01需量误差、0x02日计时误差、0x03走字计数、0x04对标、0x05预付费功能检定</param>
        /// <param name="bln_Result">是否获得数据</param>
        /// <param name="int_Sum">对应表位的误差次数</param>
        /// <param name="str_Data">对应表位的误差值</param>
        /// <returns></returns>
        bool ReadDataTenTime(int byt_TaskType, ref bool[] bln_Result, ref int[] int_Sum, ref string[] str_Data);
        #endregion





        #region 扩展命令
        /// <summary>
        /// 执行其它扩展指令(有返回指令)
        /// </summary>
        /// <param name="byt_Cmd">命令字</param>
        /// <param name="byt_Data">数据</param>
        /// <param name="bln_RevResult">返回结论</param>
        /// <param name="byt_RevData">返回数据</param>
        /// <param name="int_Scend">等待时间</param>
        /// <returns></returns>
        bool ExeOtherCmd(byte byt_Cmd, byte[][] byt_Data, ref bool[] bln_RevResult, ref byte[][] byt_RevData, int int_Scend);




        /// <summary>
        /// 执行其它扩展指令(单表位,有返回指令)
        /// </summary>
        /// <param name="int_Num">表位</param>
        /// <param name="byt_Cmd">命令字</param>
        /// <param name="byt_Data">数据</param>
        /// <param name="byt_RevData">返回指令</param>
        ///  <param name="int_Scend">等待时间</param>
        /// <returns></returns>
        bool ExeOtherCmd(int int_Num, byte byt_Cmd, byte[] byt_Data, ref byte[] byt_RevData, int int_Scend);

        /// <summary>
        /// 执行其它扩展指令(无返回指令)
        /// </summary>
        /// <param name="byt_Cmd">命令字</param>
        /// <param name="byt_Data">数据</param>
        /// <param name="int_Scend">间隔时间</param>
        /// <returns></returns>
        bool ExeOtherCmd(byte byt_Cmd, byte[] byt_Data, int int_Scend);


       


        /// <summary>
        /// 执行其它扩展指令(单表位,无返回指令)
        /// </summary>
        /// <param name="int_Num">表位</param>
        /// <param name="byt_Cmd">命令字</param>
        /// <param name="byt_Data">数据</param>
        /// <param name="int_Scend">间隔时间</param>
        /// <returns></returns>
        bool ExeOtherCmd(int int_Num, byte byt_Cmd, byte[] byt_Data, int int_Scend);
        #endregion




    }
}
