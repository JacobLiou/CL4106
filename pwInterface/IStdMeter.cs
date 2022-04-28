/****************************************************************************

    标准表接口类
    刘伟 2009-10

*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace pwInterface
{

    /// <summary>
    /// 标准表通信接口
    /// </summary>
    public interface IStdMeter
    {
        /// <summary>
        /// 标准表地址
        /// </summary>
        string ID { get;set;}


        /// <summary>
        /// 特波率
        /// </summary>
        string Setting { set;}


        /// <summary>
        /// 失败信息
        /// </summary>
        string LostMessage { get;}

        /// <summary>
        /// 标准表串口
        /// </summary>
        ISerialport ComPort { get;set;}

        /// <summary>
        /// 通道选择，主要用于控制CL2011设备的，有效值1-4,
        /// 控制PC串口的RTSEnable和DTREnable的4种组合
        /// CL2018-1串口无效。
        /// </summary>
        int Channel { get;set;}


        /// <summary>
        /// 
        /// </summary>
        bool Enabled { get;set;}


        /// <summary>
        /// 自动适应标准表端口号
        /// </summary>
        /// <param name="mySerialPort">端口数组</param>
        /// <returns>适应成功则返回端口号，失败则返回-1</returns>
        int AdaptCom(ISerialport[] mySerialPort);


        /// <summary>
        /// 标准表联机
        /// </summary>
        /// <returns>true 联机成功,false 联机失败</returns>
        bool link();

        /// <summary>
        /// 读取标准表版本
        /// </summary>
        /// <param name="strVer">返回版本号</param>
        /// <returns>true 读取成功，false 读取失败</returns>
        bool ReadVer(ref string strVer);

        /// <summary>
        /// 1.	设置接线方式 81 30 PCID 0a a3 00 01 20 uclinemode CS
        /// </summary>
        /// <param name="Uclinemode">接线方式：CL1115：自动量程：08H 手动量程：88H</param>
        /// <returns></returns>
        bool SetStdMeterUclinemode(byte Uclinemode);


        /// <summary>
        /// 2.	设置本机常数 81 30 PCID 0d a3 00 04 01 uiLocalnum CS
        /// </summary>
        /// <param name="lStdConst">标准表常数</param>
        /// <param name="bAuto">参数无效，适应接口</param>
        /// <returns></returns>
        bool SetStdMeterConst(long lStdConst, bool bAuto);

        /// <summary>
        /// 3.	设置电能指示 81 30 PCID 0b a3 00 08 01 usE1type CS
        /// </summary>
        /// <param name="int_EngeType">CL1115：总有功电能00H  ；CL3115：总有功电能00H  总无功电能40H  </param>
        /// <returns></returns>
        bool SetStdMeterUsE1type(int int_EngeType);

        /// <summary>
        /// 4.	设置电能计算误差启动开关(启动标准表)
        /// </summary>
        /// <param name="UcE1switch">启动指令 0：停止计算  1：开始计算电能误差  2：开始计算电能走字</param>
        /// <returns></returns>
        bool SetStdMeterUcE1switch(byte UcE1switch);

        /// <summary>
        /// 5.	设置电能参数 81 30 PCID 0e a3 00 09 20 uclinemode 11 usE1type ucE1switch CS
        /// </summary>
        /// <param name="Uclinemode">接线方式：CL1115：自动量程：08H 手动量程：88H</param>
        /// <param name="UsE1type">电能指示：CL1115：总有功电能00H    CL3115：总有功电能00H  总无功电能40H  </param>
        /// <param name="UcE1switch">启动指令： 0：停止计算  1：开始计算电能误差  2：开始计算电能走字</param>
        /// <returns>[true-成功,false-失败]</returns>
        /// <returns></returns>
        bool SetAmMeterParameter(byte Uclinemode, byte UsE1type, byte UcE1switch);


        /// <summary>
        /// 读取标准表测量数据
        /// </summary>
        /// <param name="sPara">返回测量数据，先后顺序按协议格式</param>
        /// <returns>true 读取成功，false 读取失败</returns>
        bool ReadStdMeterInfo(ref string []sPara);

        /// <summary>
        /// 读取标准表标准常数
        /// </summary>
        /// <param name="lngConst">返回标准常数</param>
        /// <returns>true 读取成功，false 读取失败</returns>
        bool ReadStdMeterConst(ref long lngConst);


        /// <summary>
        /// 12.	设置标准表档位
        /// </summary>
        /// <param name="sngIa"></param>
        /// <param name="sngIb"></param>
        /// <param name="sngIc"></param>
        /// <param name="sngUa"></param>
        /// <param name="sngUb"></param>
        /// <param name="sngUc"></param>
        /// <returns></returns>
        bool SetUIScale(float sngIa, float sngIb, float sngIc, float sngUa, float sngUb, float sngUc);

        /// <summary>
        /// 查询标准表常数
        /// </summary>
        /// <param name="sng_I">输出电流</param>
        /// <returns></returns>
        long SelectStdMeterConst(Single sng_I);

        /// <summary>
        /// 设置标准表界面
        /// </summary>
        /// <param name="int_Type"></param>
        /// <returns></returns>
        bool SetLCDMenu(byte int_Type);

        /// <summary>
        /// 退出标准表界面,返回初始界面
        /// </summary>
        /// <returns></returns>
        bool ExitLCDMenu();

        /// <summary>
        /// 启动标准表
        /// </summary>
        /// <returns>true 启动成功，false 启动失败</returns>
        bool RunStdMeter();

        /// <summary>
        /// 扩展指令
        /// </summary>
        /// <param name="byt_Cmd">命令字</param>
        /// <param name="byt_Data">数据域</param>
        /// <param name="byt_RevData">返回数据</param>
        /// <returns>true 操作成功，false 操作失败</returns>
        bool ExtendCommand(byte byt_Cmd, byte[] byt_Data, ref byte[] byt_RevData);


        ///// <summary>
        ///// 读取标准输出档位
        ///// </summary>
        ///// <param name="int_UaRng">A相电压档位</param>
        ///// <param name="int_UbRng">B相电压档位</param>
        ///// <param name="int_UcRng">C相电压档位</param>
        ///// <param name="int_IaRng">A相电流档位</param>
        ///// <param name="int_IbRng">B相电流档位</param>
        ///// <param name="int_IcRng">C相电流档位</param>
        ///// <returns></returns>
        //bool ReadOutRange(ref int int_UaRng, ref int int_UbRng, ref int int_UcRng,
        //                  ref int int_IaRng, ref int int_IbRng, ref int int_IcRng);



        ///// <summary>
        ///// 设置标准表电压档位
        ///// </summary>
        ///// <param name="iUaScale">A相电压档位</param>
        ///// <param name="iUbScale">B相电压档位</param>
        ///// <param name="iUcScale">C相电压档位</param>
        ///// <returns>true 设置成功，false 设置失败</returns>
        //bool SetUScale(int iUaScale,int iUbScale,int iUcScale);

        ///// <summary>
        ///// 设置标准表电压档位
        ///// </summary>
        ///// <param name="sngUa">A相电压幅值</param>
        ///// <param name="sngUb">B相电压幅值</param>
        ///// <param name="sngUc">C相电压幅值</param>
        ///// <returns>true 设置成功，false 设置失败</returns>
        //bool SetUScale(Single sngUa, Single sngUb, Single sngUc);

        ///// <summary>
        ///// 设置标准表电流档位
        ///// </summary>
        ///// <param name="iIaScale">A相电流档位</param>
        ///// <param name="iIbScale">B相电流档位</param>
        ///// <param name="iIcScale">C相电流档位</param>
        ///// <returns>true 设置成功，false 设置失败</returns>
        //bool SetIScale(int iIaScale, int iIbScale, int iIcScale);

        ///// <summary>
        ///// 设置标准表电流档位
        ///// </summary>
        ///// <param name="sngIa">A相电流幅值</param>
        ///// <param name="sngIb">B相电流幅值</param>
        ///// <param name="sngIc">C相电流幅值</param>
        ///// <returns>true 设置成功，false 设置失败</returns>
        //bool SetIScale(Single sngIa, Single sngIb, Single sngIc);

        ///// <summary>
        ///// 读取标准表累计脉冲数。注意在锁定档位下有效。
        ///// </summary>
        ///// <param name="lngPulse">返回脉冲数</param>
        ///// <returns>true 读取成功，false 读取失败</returns>
        //bool ReadStdMeterPulses(ref long lngPulse);

        ///// <summary>
        ///// 读取标准表标准常数 非临界档位直接查表
        ///// </summary>
        ///// <param name="lngConst">常数</param>
        ///// <param name="sng_U">电压</param>
        ///// <param name="sng_I">电流</param>
        ///// <param name="bln_SetXieBo">是否设置谐波</param>
        ///// <returns></returns>
        //bool ReadStdMeterConst(ref long lngConst, Single sng_U, Single sng_I, bool bln_SetXieBo);


        ///// <summary>
        ///// 设置被检表参数
        ///// </summary>
        ///// <param name="lMeterConst">被检表常数</param>
        ///// <param name="lPulseTimes">校验圈数</param>
        ///// <param name="iLX">量限，0=25A,1=100A</param>
        ///// <param name="iClfs">测量方式，0=PT4,1=QT4,2=P32,3=Q32,4=Q90,5=Q60,6=Q33</param>
        ///// <returns>true 设置成功，false 设置失败</returns>
        //bool SetAmMeterParameter(long lMeterConst, long lPulseTimes, int iLX, int iClfs);

    }
}
