/****************************************************************************

    被检表协议接口类
    刘伟 2009-10

*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace pwInterface
{

    /// <summary>
    /// 被校表通信协议接口
    /// </summary>
    public interface IMainControlProtocol
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
        /// 主控板事件
        /// </summary>
        event DelegateEventMainControl OnEventMainControl;

        /// <summary>
        /// 通信串口
        /// </summary>
        ISerialport ComPort { get;set;}

        /// <summary>
        /// 波特率
        /// </summary>
        string Setting { get;set;}



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
        /// 命令链路
        /// </summary>
        /// <param name="bCmd">控制码</param>
        /// <param name="cAddr">表地址</param>
        /// <param name="bDataLen">数据长度</param>
        /// <param name="cData">数据域数据</param>
        /// <param name="bExtend">是否有后续数据</param>
        /// <returns></returns>
        bool SendDLT645Command(byte bCmd, string cAddr, byte bDataLen, ref string cData);


        #region ---------应用方法----------
        /// <summary>
        /// 升源(F930,F950)
        /// </summary>
        /// <param name="Typ_Cmd">Typ_Cmd为00=表示220V给A组电表供电上电，01=表示154V给A组电表供电上电，02=表示220V给B组电表供电上电，03=表示154V给B组电表供电上电</param>
        /// <returns></returns>
        bool SetPowerOn(int Typ_Cmd);

        /// <summary>
        /// 继电器切换电源给电表供电(F930)
        /// </summary>
        /// <param name="Typ_Cmd">Typ_Cmd为00=表示220V给A组电表供电，01=表示154V给A组电表供电，02=表示220V给B组电表供电，03=表示154V给B组电表供电</param>
        /// <returns></returns>
        bool SetJDQChange(int Typ_Cmd);

        /// <summary>
        /// 给表位上电断电(F950)
        /// </summary>
        /// <param name="Typ_Cmd">Typ_Cmd为00=表示给A组表位断电，01=表示给A组表位上电，02=表示给B组表位断电，03=表示给B组表位上电</param>
        /// <returns></returns>
        bool SetPowerOnOff(int Typ_Cmd);

        /// <summary>
        /// 启动、复位停止(F980)
        /// </summary>
        /// <param name="Typ_Cmd">Typ_Cmd为00=表示A组复位停止，01=表示A组启动，02=表示B组复位停止，03=表示B组启动</param>
        /// <returns></returns>
        bool SetStartStopCmd(int Typ_Cmd);

        /// <summary>
        /// 气缸控制 --上、下(F913)
        /// </summary>
        /// <param name="Typ_Cmd">Typ_Cmd为00=表示A组气缸下降，01=表示A组气缸上升，04=表示A组气缸停止，02=表示B组气缸下降，03=表示B组气缸上升 05=表示B组气缸停止，</param>
        /// <returns></returns>
        bool SetStartStopQG(int Typ_Cmd);

        /// <summary>
        /// 设置测试状态(F90F)
        /// </summary>
        /// <param name="Typ_Cmd">XX为00时表示A组测试完毕，为01时表示A组测试中，为02时表示B组测试完毕，为03时表示B组测试中</param>
        /// <returns></returns>
        bool SetTestStatusCmd(int Typ_Cmd);


        /// <summary>
        /// 选择测试继电器(F907)
        /// </summary>
        /// <param name="Typ_Cmd">Typ_Cmd为00时表示选择测试内置继电器，为01时表示选择测试外置继电器，为02时表示外置隔离继电器</param>
        /// <returns></returns>
        bool SelectJDQ(int Typ_Cmd);

        /// <summary>
        /// A组电流校淮切换(F931)
        /// </summary>
        /// <param name="Typ_Cmd">Typ_Cmd为为00=表示大电流校准，01=表示小电流校准</param>
        /// <returns></returns>
        bool AdjustCmdA(int Typ_Cmd);

        /// <summary>
        /// B组电流校淮切换(F931)
        /// </summary>
        /// <param name="Typ_Cmd">Typ_Cmd为为00=表示大电流校准，01=表示小电流校准</param>
        /// <returns></returns>
        bool AdjustCmdB(int Typ_Cmd);


        /// <summary>
        /// 读A组继电器状态(F908)
        /// </summary>
        /// <param name="str_Valu">返回值XX表示读到的A组继电器状态，01表示1#表位拉闸故障，02表示2#表位拉闸故障，04表示3#表位拉闸故障， 03表示1#2#表位拉闸故障，05表示1#3#表位拉闸故障，06表示2#3#表位拉闸故障，07表示1#2#3#表位拉闸故障，00表示无故障</param>
        /// <returns></returns>
        bool ReadJDQA(ref string str_Value);


        /// <summary>
        /// 读B组继电器状态(F909)
        /// </summary>
        /// <param name="str_Valu">返回值XX表示读到的B组继电器状态，08表示4#表位拉闸故障，10表示5#表位拉闸故障，20表示6#表位拉闸故障，18表示4#、5#表位拉闸故障，28表示4#、6#表位拉闸故障，30表示5#、6#表位拉闸故障，38表示4#、5#、6#表位拉闸故障，00表示无故障</param>
        /// <returns></returns>
        bool ReadJDQB(ref string str_Value);



        /// <summary>
        /// 设置A组表位故障灯(F90D)
        /// </summary>
        /// <param name="Typ_Cmd">XX表示A组故障灯状态，01表示1#表位故障灯亮，02表示2#表位故障灯亮，04表示3#表位故障灯亮， 03表示1#2#表位故障灯亮，05表示1#3#表位故障灯亮，06表示2#3#表位故障灯亮，07表示1#2#3#表位故障灯亮，00表示无故障</param>
        /// <returns></returns>
        bool SetGzdengA(int Typ_Cmd);

        /// <summary>
        /// 设置B组表位故障灯(F90E)
        /// </summary>
        /// <param name="Typ_Cmd">XX表示B组故障灯状态，08表示4#表位故障灯亮，10表示5#表位故障灯亮，20表示6#表位故障灯亮，18表示4#、5#表位故障灯亮，28表示4#、6#表位故障灯亮，30表示5#、6#表位故障灯亮，38表示4#、5#、6#表位故障灯亮，00表示无故障</param>
        /// <returns></returns>
        bool SetGzdengB(int Typ_Cmd);

        /// <summary>
        /// 整机自检开始命令
        /// </summary>
        /// <param name="Typ_Cmd">0为A组，1为B组</param>
        /// <returns></returns>
        bool SetSelfCheckStart(int Typ_Cmd);


        /// <summary>
        /// 整机自检结束命令
        /// </summary>
        /// <param name="Typ_Cmd">0为A组，1为B组</param>
        /// <param name="cDataStr">继电器故障状态，bit0~2为三个继电器的故障状态，0正常，1故障</param>
        /// <returns></returns>
        bool SetSelfCheckEnd(int Typ_Cmd, ref string cDataStr);

        #endregion

        /// <summary>
        /// 反转字节字符串
        /// </summary>
        /// <param name="sData"></param>
        /// <returns></returns>
        string BackString(string sData);



    }
}
