/****************************************************************************

    通讯接口类
    刘伟 2009-10

*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace pwInterface
{

    /// <summary>
    /// 收到数据委托， 串口收到数据触发事件，
    /// </summary>
    /// <param name="bData">串口收到数</param>
    public delegate void RevEventDelegete(byte[] bData);

    /// <summary>
    /// 串口接口
    /// </summary>
    public interface ISerialport
    {
        /// <summary>
        /// 端口号
        /// </summary>
        int ComPort { get;set;}

        /// <summary>
        /// 串口收到数据触发事件
        /// </summary>
        event RevEventDelegete DataReceive;

        /// <summary>
        /// 通道选择，主要用于控制CL2011设备的，有效值1-4,
        /// 控制PC串口的RTSEnable和DTREnable的4种组合
        /// CL2018-1串口无效。
        /// </summary>
        int Channel { get;set;}

        /// <summary>
        /// 端口类型 0 PC串口，1 CL2018-1无协议， 2 CL2018-1协议包
        /// </summary>
        int ComType { get;}

        /// <summary>
        /// IP地址，在ComType=0是无效
        /// </summary>
        string IP { get;}

        /// <summary>
        /// 失败信息
        /// </summary>
        string LostMessage { get;}

        /// <summary>
        /// 通信参数
        /// </summary>
        string Setting { get;set;}

        /// <summary>
        /// 端口状态
        /// </summary>
        bool State { get;}

        /// <summary>
        /// 打开端口
        /// </summary>
        /// <param name="str_Para">端口参数，格式：端口号,IP:远程Port号:绑定起始Port号  注：如是PC端口后面参数可以省略</param>
        /// <returns></returns>
        bool PortOpen(string str_Para);

        /// <summary>
        /// 关闭端口
        /// </summary>
        /// <returns></returns>
        bool PortClose();

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="byt_Data">数据</param>
        /// <returns></returns>
        bool SendData(byte [] byt_Data);
       
    }




}
