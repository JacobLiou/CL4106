/****************************************************************************

    程控源接口类
    刘伟 2009-10

*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace pwInterface
{
    /// <summary>
    /// 源通信协议接口
    /// </summary>
    public interface IPower
    {
        /// <summary>
        /// 源地址
        /// </summary>
        string ID { get;set;}

        /// <summary>
        /// 源串口
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
        /// 自动适应标准表端口号
        /// </summary>
        /// <param name="mySerialPort">端口数组</param>
        /// <returns>适应成功则返回端口号，失败则返回-1</returns>
        int AdaptCom(ISerialport[] mySerialPort);

        /// <summary>
        /// 源联机
        /// </summary>
        /// <returns>true 联机成功,false 联机失败</returns>
        bool link();

        /// <summary>
        /// 读取源版本
        /// </summary>
        /// <param name="strVer">返回版本号</param>
        /// <returns>true 读取成功，false 读取失败</returns>
        bool ReadVer(ref string strVer);

        /// <summary>
        /// 控制源输出，分相输出
        /// </summary>
        /// <param name="Ua">A相电压</param>
        /// <param name="Ub">B相电压</param>
        /// <param name="Uc">C相电压</param>
        /// <param name="Ia">A相电流</param>
        /// <param name="Ib">B相电流</param>
        /// <param name="Ic">C相电流</param>
        /// <param name="PhiUa">A相电压角度</param>
        /// <param name="PhiUb">B相电压角度</param>
        /// <param name="PhiUc">C相电压角度</param>
        /// <param name="PhiIa">A相电流角度</param>
        /// <param name="PhiIb">B相电流角度</param>
        /// <param name="PhiIc">C相电流角度</param>
        /// <param name="Hz">频率</param>
        /// <param name="iClfs">测试方式 0=PT4,1=QT4,2=P32,3=Q32,4=Q60,5=Q90,6=Q33</param>
        /// <param name="Xwkz">相位控制 XXIcIbIaUcUbUa</param>
        /// <returns>true 输出成功，false 输出失败</returns>
        bool PowerOn(Single Ua, Single Ub, Single Uc,
                     Single Ia, Single Ib, Single Ic,
                     Single PhiUa, Single PhiUb, Single PhiUc,
                     Single PhiIa, Single PhiIb, Single PhiIc,
                     Single Hz, int iClfs, byte Xwkz);

        /// <summary>
        /// 控制源输出，统一输出
        /// </summary>
        /// <param name="U">电压</param>
        /// <param name="I">电流</param>
        /// <param name="Phi">角度(电压电流夹角)</param>
        /// <param name="Hz">频率</param>
        /// <param name="iClfs">测试方式 0=PT4,1=QT4,2=P32,3=Q32,4=Q60,5=Q90,6=Q33,7=P</param>
        /// <param name="Xwkz">相位控制 XXIcIbIaUcUbUa</param>
        /// <returns>true 输出成功，false 输出失败</returns>
        bool PowerOn(Single U, Single I, Single Phi, Single Hz, int iClfs, byte Xwkz);

        /// <summary>
        /// 控源按功率因数输出
        /// </summary>
        /// <param name="U">电压</param>
        /// <param name="I">电流</param>
        /// <param name="Phi">功率因数</param>
        /// <param name="Hz">频率</param>
        /// <param name="iClfs">测试方式 0=PT4,1=QT4,2=P32,3=Q32,4=Q60,5=Q90,6=Q33,7=P</param>
        /// <param name="HABC">元件：0=合元，1＝A元，2＝B元，3＝C元</param>
        /// <returns></returns>
        bool PowerOn(float U, float I, string str_Glys, float Hz, int iClfs, enmElement emt_Element);

        
        /// <summary>
        /// 关源操作
        /// </summary>
        /// <returns>true 关源成功，false 关源失败</returns>
        bool PowerOff();

    }




}
