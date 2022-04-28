using System;
using System.Collections.Generic;
using System.Text;

namespace ClInterface
{



    public delegate void Dge_EventRxFrame(string str_Frame);
    public delegate void Dge_EventTxFrame(string str_Frame);

    public delegate void DisposeRxEvent(string str_Frame);
    public delegate void DisposeTxEvent(string str_Frame);


    /// <summary>
    /// 多功能表协议接口
    /// </summary>
    public interface IAmMeterProtocol
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
        string WritePassword { get;set;}

        /// <summary>
        /// 需量清零密码
        /// </summary>
        string ClearDemandPassword { get;set;}

        /// <summary>
        /// 电量清零密码
        /// </summary>
        string ClearEnergyPassword { get;set;}

        
        /// <summary>
        /// 操作员代码
        /// </summary>
        string UserID { get;set;}

        /// <summary>
        /// 密码验证类型
        /// </summary>
        int VerifyPasswordType { get;set;}

        /// <summary>
        /// 写操作时，数据域是否包含写密码,true=要，false=不用
        /// </summary>
        bool DataFieldPassword { get;set;}

        /// <summary>
        /// 块写操作时数据域后是否加AA
        /// </summary>
        bool BlockAddAA { get;set;}

        /// <summary>
        /// 费率排序类型  如：峰平谷尖=1234 尖峰平谷=4123
        /// </summary>
        string TariffOrderType { get;set;}

        /// <summary>
        /// 日期格式,默认:YYMMDDHHFFSS
        /// </summary>
        string DateTimeFormat { get;set;}

        /// <summary>
        /// 星期天的序号，默认:0
        /// </summary>
        int SundayIndex { get;set;}

        /// <summary>
        /// 配置文件
        /// </summary>
        string ConfigFile { get;set;}



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
        int FECount { get;set;}
            


        /// <summary>
        /// 通信测试
        /// </summary>
        /// <param name="int_Type"></param>
        /// <returns></returns>
        bool ComTest(int int_Type);



        /// <summary>
        /// 广播校时
        /// </summary>
        /// <param name="int_Type">广播校时类型</param>
        /// <param name="str_DateTime">日期时间</param>
        /// <returns></returns>
        bool BroadcastTime(int int_Type, string str_DateTime);



        /// <summary>
        /// 读取电量(分费率读取)
        /// </summary>
        /// <param name="int_Type">读取类型</param>
        /// <param name="ept_DirectType">功率类型，0=P+ 1=P- 2=Q+ 3=Q- 4=Q1 5=Q2 6=Q3 6=Q4</param>
        /// <param name="ett_TariffType">费率类型，0=总，1=峰，2=平，3=谷，4=尖</param>
        /// <param name="sng_Energy">返回电量</param>
        /// <returns></returns>
        bool ReadEnergy(int int_Type, enmPDirectType ept_DirectType, enmTariffType ett_TariffType, ref Single sng_Energy);

        /// <summary>
        /// 读取电量(所有费率读取)
        /// </summary>
        /// <param name="int_Type">读取类型</param>
        /// <param name="ept_DirectType">功率类型，0=P+ 1=P- 2=Q+ 3=Q- 4=Q1 5=Q2 6=Q3 6=Q4</param>
        /// <param name="sng_Energy">返回电量</param>
        /// <returns></returns>
        bool ReadEnergy(int int_Type, enmPDirectType ept_DirectType, ref Single[] sng_Energy);

        /// <summary>
        /// 读取需量(分费率读取)
        /// </summary>
        /// <param name="int_Type">读取类型</param>
        /// <param name="ept_DirectType">功率类型，0=P+ 1=P- 2=Q+ 3=Q- 4=Q1 5=Q2 6=Q3 6=Q4</param>
        /// <param name="ett_TariffType">费率类型，0=总，1=峰，2=平，3=谷，4=尖</param>
        /// <param name="sng_Demand">返回需量</param>
        /// <returns></returns>
        bool ReadDemand(int int_Type, enmPDirectType ept_DirectType, enmTariffType ett_TariffType, ref Single sng_Demand);

        /// <summary>
        /// 读取需量(所有费率读取)
        /// </summary>
        /// <param name="int_Type">读取类型</param>
        /// <param name="ept_DirectType">功率类型，0=P+ 1=P- 2=Q+ 3=Q- 4=Q1 5=Q2 6=Q3 6=Q4</param>
        /// <param name="sng_Demand">返回需量</param>
        /// <returns></returns>
        bool ReadDemand(int int_Type, enmPDirectType ept_DirectType, ref Single[] sng_Demand);

        /// <summary>
        /// 读日期时间
        /// </summary>
        /// <param name="int_Type">读取类型</param>
        /// <param name="str_DateTime">返回时间</param>
        /// <returns></returns>
        bool ReadDateTime(int int_Type, ref string str_DateTime);


        /// <summary>
        /// 读地址
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="str_Address">返回地址</param>
        /// <returns></returns>
        bool ReadAddress(int int_Type, ref string str_Address);

        /// <summary>
        /// 读取时段
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="str_PTime">返回时段</param>
        /// <returns></returns>
        bool ReadPeriodTime(int int_Type,ref string [] str_PTime);


        /// <summary>
        /// 读取数据（数据型，数据项）
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="str_ID">标识符,2个字节</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="int_Dot">小数位</param>
        /// <param name="sng_Value">返回数据</param>
        /// <returns></returns>
        bool ReadData(int int_Type, string str_ID, int int_Len, int int_Dot, ref Single sng_Value);

        /// <summary>
        /// 读取数据（字符型，数据项）
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="str_ID">标识符,2个字节</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="str_Value">返回数据</param>
        /// <returns></returns>
        bool ReadData(int int_Type, string str_ID, int int_Len, ref string str_Value);


        /// <summary>
        /// 读取数据（数据型，数据块）
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="str_ID">标识符,2个字节</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="int_Dot">小数位</param>
        /// <param name="sng_Value">返回数据</param>
        /// <returns></returns>
        bool ReadData(int int_Type, string str_ID, int int_Len, int int_Dot, ref Single[] sng_Value);

        /// <summary>
        /// 读取数据（字符型，数据块）
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="str_ID">标识符,2个字节</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="int_Dot">小数位</param>
        /// <param name="sng_Value">返回数据</param>
        /// <returns></returns>
        bool ReadData(int int_Type, string str_ID, int int_Len, ref string[] str_Value);


        

        /// <summary>
        /// 写地址
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="str_Address">写入地址</param>
        /// <returns></returns>
        bool WriteAddress(int int_Type, string str_Address);

        /// <summary>
        /// 写日期时间
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="str_DateTime">日期时间</param>
        /// <returns></returns>
        bool WriteDateTime(int int_Type, string str_DateTime);

        /// <summary>
        /// 写时段
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="str_PTime">时段组</param>
        /// <returns></returns>
        bool WritePeriodTime(int int_Type, string[] str_PTime);



        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="str_ID">标识符</param>
        /// <param name="bln_Reverse">解释方式，true=高低位对调，false=高低位正常</param>
        /// <param name="byt_Value">写入数据</param>
        /// <returns></returns>
        bool WriteData(int int_Type, string str_ID, byte[] byt_Value);

        /// <summary>
        /// 写数据(字符型，数据项)
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="str_ID">标识符,两个字节</param>
        /// <param name="int_Len">数据长度(块中每项字节数)</param>
        /// <param name="str_Value">写入数据</param>
        /// <returns></returns>
        bool WriteData(int int_Type, string str_ID, int int_Len, string str_Value);


        /// <summary>
        /// 写数据(数据型，数据项)
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="str_ID">标识符</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="int_Dot">小数位</param>
        /// <param name="bln_Reverse">解释方式，true=高低位对调，false=高低位正常</param>
        /// <param name="sng_Value">写入数据</param>
        /// <returns></returns>
        bool WriteData(int int_Type, string str_ID, int int_Len, int int_Dot, Single sng_Value);


        /// <summary>
        /// 写数据(字符型，数据块)
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="str_ID">标识符</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="bln_Reverse">解释方式，true=高低位对调，false=高低位正常</param>
        /// <param name="str_Value">写入数据</param>
        /// <returns></returns>
        bool WriteData(int int_Type, string str_ID, int int_Len, string[] str_Value);


        /// <summary>
        /// 写数据(数据型，数据块)
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="str_ID">标识符</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="int_Dot">小数位</param>
        /// <param name="bln_Reverse">解释方式，true=高低位对调，false=高低位正常</param>
        /// <param name="str_Value">写入数据</param>
        /// <returns></returns>
        bool WriteData(int int_Type, string str_ID, int int_Len, int int_Dot, Single[] sng_Value);


        /// <summary>
        /// 清空需量
        /// </summary>
        /// <returns></returns>
        bool ClearDemand(int int_Type);

        /// <summary>
        /// 清空电量
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <returns></returns>
        bool ClearEnergy(int int_Type);

        /// <summary>
        /// 清空事件记录
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="str_ID">事件清零内容</param>
        /// <returns></returns>
        bool ClearEventLog(int int_Type, string str_ID);

        /// <summary>
        /// 设置脉冲端子
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="ecp_PulseType">端子输出脉冲类型</param>
        /// <returns></returns>
        bool SetPulseCom(int int_Type, enmComPulseType ecp_PulseType);


        /// <summary>
        /// 冻结命令
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="str_DateHour">冻结时间，MMDDhhmm(月.日.时.分)</param>
        /// <returns></returns>
        bool FreezeCmd(int int_Type, string str_DateHour);


        /// <summary>
        /// 更改波特率
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="str_Setting">波特率</param>
        /// <returns></returns>
        bool ChangeSetting(int int_Type, string str_Setting);


        /// <summary>
        /// 更改密码
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="int_Class">密码等级，即修改几级的密码</param>
        /// <param name="str_OldPws">旧密码,如果是更高等级修改本等级密码则需加更高等级，原密码则不包含等级</param>
        /// <param name="str_NewPsw">新密码,不包含等级</param>
        /// <returns></returns>
        bool ChangePassword(int int_Type,int int_Class, string str_OldPws, string str_NewPsw);

        



    }
}
