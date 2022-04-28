/****************************************************************************

    时基源接口类
    刘伟 2009-10

*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace pwInterface
{
    public interface IStdTime
    {
        //-----------属性--------------------
        
        string ID { get;set;}               //表的ID号
        ISerialport ComPort { get;set;}     //串口


        /// <summary>
        /// 通道选择，主要用于控制CL2011设备的，有效值1-4,
        /// 控制PC串口的RTSEnable和DTREnable的4种组合
        /// CL2018-1串口无效。
        /// </summary>
        int Channel { get;set;}
        
        /// <summary>
        /// 通信参数设置,有多个设备,不同特率可以用/隔开
        /// </summary>
        string Setting { set;}          //

        /// <summary>
        /// 失败信息
        /// </summary>
        string LostMessage { get;}


        //----------方法---------------------
        

        /// <summary>
        /// 联机
        /// </summary>
        /// <returns></returns>
        bool Link();                                                    //联机

        /// <summary>
        /// 设置通道
        /// </summary>
        /// <param name="iType">0=标准时钟脉冲、1=标准电能脉冲</param>
        /// <returns></returns>
        bool SetChannel(int iType);                                     //

        /// <summary>
        /// 读GPS时间
        /// </summary>
        /// <param name="sDateTime"></param>
        /// <returns></returns>
        bool ReadGPSTime(ref string sDateTime);                         //

        /// <summary>
        /// 读温湿度
        /// </summary>
        /// <param name="Temperature"></param>
        /// <param name="Humidity"></param>
        /// <returns></returns>
        bool ReadTempHum(ref float Temperature, ref float Humidity);
        

        ///// <summary>
        /////设置GPS时间,只能在读取不到GPS时间的时候
        ///// </summary>
        ///// <param name="sDateTime">日期时间</param>
        ///// <returns></returns>
        //bool SetGPSTime(string sDateTime);

        ///// <summary>
        ///// 读取功耗大信号电压小信号电流的视在功率
        ///// </summary>
        ///// <param name="sValue">总\A相\B相\C相</param>
        ///// <returns></returns>
        //bool ReadGhBuSiS(ref string[] sValue);

        ///// <summary>
        ///// 读取功耗大信号电压小信号电流的有功功率
        ///// </summary>
        ///// <param name="sValue">总\A相\B相\C相</param>
        ///// <returns></returns>
        //bool ReadGhBuSiP(ref string[] sValue);

        ///// <summary>
        ///// 读取功耗大信号电流小信号电压的视在功率
        ///// </summary>
        ///// <param name="sValue">总\A相\B相\C相</param>
        ///// <returns></returns>
        //bool ReadGhBiSuS(ref string[] sValue);

        ///// <summary>
        ///// 读取功耗小信号电压
        ///// </summary>
        ///// <param name="sValue">A相\B相\C相</param>
        ///// <returns></returns>
        //bool ReadSmallU(ref string[] sValue);

        ///// <summary>
        ///// 读取功耗小信号电流
        ///// </summary>
        ///// <param name="sValue">A相\B相\C相</param>
        ///// <returns></returns>
        //bool ReadSmallI(ref string[] sValue);
        






    }




}
