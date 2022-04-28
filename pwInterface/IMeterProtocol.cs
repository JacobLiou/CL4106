/****************************************************************************

    被检表协议接口类
    刘伟 2009-10

*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
namespace pwInterface
{
    public delegate void Dge_EventRxFrame(int intCom,string str_Frame);
    public delegate void Dge_EventTxFrame(int intCom, string str_Frame);

    public delegate void DisposeRxEvent(int intCom, string str_Frame);
    public delegate void DisposeTxEvent(int intCom, string str_Frame);


    /// <summary>
    /// 被校表通信协议接口
    /// </summary>
    public interface IMeterProtocol
    {

        /// <summary>
        /// 上行485数据事件
        /// </summary>
        event Dge_EventRxFrame OnEventRxFrame;
        /// <summary>
        /// 下行485数据事件
        /// </summary>
        event Dge_EventTxFrame OnEventTxFrame;

        /// <summary>
        /// 通信串口
        /// </summary>
        ISerialport ComPort { get;set;}

        /// <summary>
        /// 波特率
        /// </summary>
        string Setting { get;set;}

        /// <summary>
        /// 表地址
        /// </summary>
        string Address { get;set;}

        /// <summary>
        /// 写操作密码
        /// </summary>
        string Password { get;set;}

        /// <summary>
        /// 密码等级
        /// </summary>
        byte PasswordClass { get; set; }

        /// <summary>
        /// 操作员代码
        /// </summary>
        string UserID { get;set;}

        /// <summary>
        /// 密码验证类型，0＝无密码认证 1＝密码放在数据域中方式 2＝A型表密码认证方式 3＝B型表密码认证方式
        /// </summary>
        int VerifyPasswordType { get;set;}

        /// <summary>
        /// 写操作时，数据域是否包含写密码,true=要，false=不用
        /// </summary>
        bool DataFieldPassword { get;set;}


        /// <summary>
        /// 等待数据到达最大时间
        /// </summary>
        int WaitDataRevTime { get; set; }

        /// <summary>
        /// 数据间隔最大时间
        /// </summary>
        int IntervalTime { get; set; }


        /// <summary>
        /// 操作失败信息
        /// </summary>
        string LostMessage { get;}

        /// <summary>
        /// 返回帧
        /// </summary>
        string RxFrame { get;}

        /// <summary>
        /// 下发帧
        /// </summary>
        string TxFrame { get;}

        /// <summary>
        /// 下发帧的唤醒符个数
        /// </summary>
        int FECount { set;}

        ///// <summary>
        ///// 帧起始字节
        ///// </summary>
        //int ZenStrat { set;}
        
        ///// <summary>
        ///// 帧长度起始字节
        ///// </summary>
        //int ZenLenStart { set;}              

        ///// <summary>
        ///// 帧长度字节数
        ///// </summary>
        //int ZenLenByte { set;}

        /// <summary>
        /// 发送接收的数据帧，数据域减0x33
        /// </summary>
        bool ZendStringDel0x33 { set; }

        /// <summary>
        /// 通讯失败重试次数
        /// </summary>
        byte iRepeatTimes { set; }


        /// <summary>
        /// 通讯完成后是否关闭端口
        /// </summary>
        bool  bClosComm { set; }

        /// <summary>
        /// 下载打包参数时是否被外部中断
        /// </summary>
        bool BreakDown{ set; }

        /// <summary>
        /// 是否科陆红外
        /// </summary>
        bool ClouHw { set; }

        /// <summary>
        /// 功耗版ID
        /// </summary>
        string RxID
        {
            get;
            set;
        }

        /// <summary>
        /// 发送数据帧，返回接收数据帧
        /// </summary>
        /// <param name="RxFrame">发送帧</param>
        /// <param name="TxFrame">接收帧</param>
        /// <param name="cData">接收数据域</param>
        /// <returns></returns>
        bool SendDLT645RxFrame(string RxFrame, ref string TxFrame, ref string cData);


        /// <summary>
        /// 读取数据（数据型，数据项）
        /// </summary>
        /// <param name="str_OBIS">标识符,2个字节</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="int_Dot">小数位</param>
        /// <param name="sng_Value">返回数据</param>
        /// <returns></returns>
        bool ReadData(string str_OBIS, int int_Len, int int_Dot, ref Single sng_Value);

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="bCmd">控制码</param>
        /// <param name="cAddr">表地址</param>
        /// <param name="bDataLen">数据长度</param>
        /// <param name="cData">数据域数据</param>
        /// <param name="bExtend">是否有后续数据</param>
        /// <returns></returns>
        bool SendDLT645Command(byte bCmd, string cAddr, byte bDataLen, ref string cData, ref bool bExtend);


        /// <summary>
        /// 命令链路
        /// </summary>
        /// <param name="bCmd">控制码</param>
        /// <param name="cAddr">表地址</param>
        /// <param name="bDataLen">数据长度</param>
        /// <param name="cData">数据域数据</param>
        /// <param name="byt_RevDataF">数据帧</param>
        /// <param name="bExtend">是否有后续数据</param>
        /// <returns></returns>
        bool SendDLT645Command(byte bCmd, string cAddr, byte bDataLen, ref string cData, ref byte[] byt_RevDataF, ref bool bExtend);

        /// <summary>
        /// 读取数据（字符型，数据项）
        /// </summary>
        /// <param name="str_OBIS">标识符,2个字节</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="str_Value">返回数据</param>
        /// <returns></returns>
        bool ReadData(string str_OBIS, int int_Len, ref string str_Value);

        /// <summary>
        /// 写数据(字符型，数据项)
        /// </summary>
        /// <param name="str_OBIS">标识符,两个字节</param>
        /// <param name="int_Len">数据长度(块中每项字节数)</param>
        /// <param name="str_Value">写入数据</param>
        /// <returns></returns>
        bool WriteData(string str_OBIS, int int_Len, string str_Value);


        /// <summary>
        /// 写数据(数据型，数据项)
        /// </summary>
        /// <param name="str_OBIS">标识符</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="int_Dot">小数位</param>
        /// <param name="bln_Reverse">解释方式，true=高低位对调，false=高低位正常</param>
        /// <param name="sng_Value">写入数据</param>
        /// <returns></returns>
        bool WriteData( string str_OBIS, int int_Len, int int_Dot, Single sng_Value);

        /// <summary>
        /// 写命令(BYTE型，数据项)
        /// </summary>
        /// <param name="byt_Cmd">命令字,1个字节</param>
        /// <param name="int_Len">写入参数数据长度(块中每项字节数)</param>
        /// <param name="str_Value">写入数据</param>
        /// <returns></returns>
        bool WriteData(byte byte_Cmd, int int_Len, string str_Value);

        /// <summary>
        /// 读通信地址
        /// </summary>
        /// <param name="str_Value">返回表地址</param>
        /// <returns></returns>
        bool ReadMeterAddress(ref string str_Value);

        /// <summary>
        /// 写通信地址
        /// </summary>
        /// <param name="str_Value">写入表地址</param>
        /// <returns></returns>
        bool WriteMeterAddress(string str_Value);


        /// <summary>
        /// 读取生产编号
        /// </summary>
        /// <param name="str_Value">返回生产编号</param>
        /// <returns></returns>
        bool ReadScbh(ref string str_Value);

        /// <summary>
        /// 读取电表底度
        /// </summary>
        /// <param name="str_Value">电表底度</param>
        /// <returns></returns>
        bool ReadEnergy(ref string str_Value);


        /// <summary>
        /// 读取软件版本号
        /// </summary>
        /// <param name="str_Value">返回软件版本号</param>
        /// <returns></returns>
        bool ReadVer(ref string str_Value);


        /// <summary>
        /// 分相供电测试:读取三相电压数据块
        /// </summary>
        /// <param name="str_Value">返回三相电压数据块</param>
        /// <returns></returns>
        bool ReadSinglePhaseTest(ref string str_Value);


        /// <summary>
        /// 交流采样测试:读电压0201FF00，电流0202FF00，有功功率0203FF00数据块
        /// </summary>
        /// <param name="str_Value">返回电压，电流，有功功率数据块</param>
        /// <returns></returns>
        bool ReadACSamplingTest(ref string str_Value);


        /// <summary>
        /// 系统清零
        /// </summary>
        /// <returns></returns>
        bool SysClear();

        /// <summary>
        /// 设置高频脉冲放大倍数(用于误差高频检定)
        /// </summary>
        /// <param name="BS"></param>
        /// <returns></returns>
        bool HighFrequencyPulse(int BS);


        /// <summary>
        /// 整机自检
        /// </summary>
        /// <param name="M1">Bit0;0=单板测试  1=整机测试;  Bit1 0=三相四 1=三相三 </param>
        /// <param name="M2">0：外控磁保持  1：内控磁保持   2：外控电保持</param>
        /// <param name="str_Value">整机测试结果，4个字节，每个bit位代表一个检测项目，包含32个，为1时代表有故障，0时代表合格</param>
        /// <returns></returns>
        bool SelfCheck(int M1, int M2, ref string str_Value);

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

        /// <summary>
        /// 读取功耗
        /// </summary>
        /// <param name="flt_ActiveP">基波有功功率</param>
        /// <returns></returns>
        bool ReadPower(ref float flt_ActiveP);

        /// <summary>
        /// 打包参数下载
        /// </summary>
        /// <param name="_DownParaItemOne">下载帧列表</param>
        /// <param name="AllAddress">统一通信地址</param>
        /// <returns></returns>
        bool DownPara(List<MeterDownParaItem> _DownParaItemOne);

        /// <summary>
        /// 初始化表参数
        /// </summary>
        /// <param name="str_Value">下发参数</param>
        /// <returns></returns>
        bool InitMeterPara(string str_Value);

        /// <summary>
        /// 反转字节字符串
        /// </summary>
        /// <param name="sData"></param>
        /// <returns></returns>
        string BackString(string sData);


        /// <summary>
        /// 海外校表协议组帧通用 --框架---IEC62056_21组帧  协议框架
        /// </summary>
        /// <param name="Addr">地址</param>
        /// <param name="i_APCU">校表协议应用命令单元</param>
        /// <param name="ArrayList_APDU">校表协议应用数据单元</param>
        /// <param name="fpByteList"></param>
        /// <param name="str_LostMessage"></param>
        /// <returns></returns>
        bool MakeFrame_Measure_Calibrate(string Addr, int i_APCU, ArrayList ArrayList_APDU, ref ArrayList fpByteList, ref string str_LostMessage);


        /// <summary>
        /// 海外校准专用
        /// </summary>
        /// <param name="i_APCU">校表协议应用命令单元</param>
        /// <param name="ArrayList_APDU">校表协议应用数据单元</param>
        /// <returns></returns>
        bool SendAdjustData(int i_APCU, ArrayList ArrayList_APDU);

    }
}
