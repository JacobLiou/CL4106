using System;
using System.Collections.Generic;

using System.Text;
using VerificationEquipment.Commons;
using Frontier.MeterVerification.DeviceCommon;
using Frontier.MeterVerification.DeviceInterface;

namespace Frontier.MeterVerification.KLDevice
{
    /// <summary>
    /// 耐压仪
    /// </summary>
    public class CLResistance : Comm2018Device, IConnect, IResistance, IControlResistanceMoto
    {

        private MeterPosition[] _meterPosition;
        /// <summary>
        /// 耐压仪控制端口
        /// </summary>
        private int m_WishStandPort = 0;

        public override void Config(List<CommSerialPortSettings> serialPortSettings, List<CommSocketSettings> socketSettings)
        {
            string strSetting = string.Format("{0},{1},{2},{3}", serialPortSettings[0].BaudRate, serialPortSettings[0].Parity, serialPortSettings[0].DataBits, serialPortSettings[0].StopBits);
            m_WishStandPort = serialPortSettings[0].CommPortNumber + socketSettings[0].Port1;
            RegisterPort(m_WishStandPort, socketSettings[0].Port2, strSetting, socketSettings[0].IP, socketSettings[0].Port1, 1000, 200);

        }


        #region IConnect 成员

        public void Connected(int meterCount)
        {
            this.Open();
        }

        public void Connected(VerificationEquipment.Commons.MeterPosition[] meterPositions)
        {
            _meterPosition = meterPositions;
        }

        public void Closed()
        {
            this.Close();
        }

        #endregion

        #region IResistance 成员

        /// <summary>
        /// 初始化耐压设备
        /// </summary>
        /// <returns></returns>
        public bool InitResistancee()
        {
            return true;
        }
        /// <summary>
        /// 设置耐压方式
        /// </summary>
        /// <param name="resistanceType">耐压方式，对外壳、辅助端子等</param>
        /// <returns></returns>
        public bool SetResistanceType(string resistanceType)
        {
            return true;
        }
        /// <summary>
        /// 设置漏电流限制
        /// </summary>
        /// <param name="resistanceI">5mA</param>
        /// <returns></returns>
        public bool SetResistanceIRangle(float resistanceI)
        {
            CL2038A_RequestSetThresholdValuePacket cl2038Packet = new CL2038A_RequestSetThresholdValuePacket();
            cl2038Packet.SetPara(resistanceI);
            CL2038A_RequestSetThresholdValueReplyPacket cl2038abrecv = new CL2038A_RequestSetThresholdValueReplyPacket();
            if (SendData(m_WishStandPort, cl2038Packet, cl2038abrecv))
            { return true; }
            else
            { return false; }
        }
        /// <summary>
        /// 设置漏电流
        /// </summary>
        /// <param name="resistanceI">漏电流，3mA，5mA</param>
        /// <returns></returns>
        public bool SetResistanceI(float resistanceI)
        {
            CL2038A_RequestSetThresholdValuePacket cl2038Packet = new CL2038A_RequestSetThresholdValuePacket();
            cl2038Packet.SetPara(resistanceI);
            CL2038A_RequestSetThresholdValueReplyPacket cl2038abrecv = new CL2038A_RequestSetThresholdValueReplyPacket();
            if (SendData(m_WishStandPort, cl2038Packet, cl2038abrecv))
            { return true; }
            else
            { return false; }
        }
        /// <summary>
        /// 设置电压
        /// </summary>
        /// <param name="resistanceU">电压值，2kv，4kv</param>
        /// <returns></returns>
        public bool SetResistanceU(float resistanceU)
        {
            CL2038A_RequestSetVoltagePacket cl2038Packet = new CL2038A_RequestSetVoltagePacket();
            cl2038Packet.SetPara(resistanceU * 1000);
            CL2038A_RequestSetVoltageReplyPacket cl2038abrecv = new CL2038A_RequestSetVoltageReplyPacket();
            if (SendData(m_WishStandPort, cl2038Packet, cl2038abrecv))
            { return true; }
            else
            { return false; }
        }
        /// <summary>
        /// 设置耐压测试时间
        /// </summary>
        /// <param name="second">耐压时间（秒）</param>
        /// <returns></returns>
        public bool SetResistanceTime(int second)
        {
            CL2038A_RequestSetTimePacket cl2038Packet = new CL2038A_RequestSetTimePacket();
            cl2038Packet.SetPara(second);
            CL2038A_RequestSetTimeReplyPacket cl2038abrecv = new CL2038A_RequestSetTimeReplyPacket();
            if (SendData(m_WishStandPort, cl2038Packet, cl2038abrecv))
            { return true; }
            else
            { return false; }
        }
        /// <summary>
        /// 开始耐压
        /// </summary>
        /// <returns></returns>
        public bool StartResistance()
        {
            CL2038A_RequestControlPacket cl2038Packet = new CL2038A_RequestControlPacket();
            cl2038Packet.SetPara(true);
            CL2038A_RequestControlReplyPacket cl2038abrecv = new CL2038A_RequestControlReplyPacket();
            if (SendData(m_WishStandPort, cl2038Packet, cl2038abrecv))
            { return true; }
            else
            { return false; }
        }

        /// <summary>
        /// 停止耐压
        /// </summary>
        /// <returns></returns>
        public bool StopResistance()
        {
            CL2038A_RequestControlPacket cl2038Packet = new CL2038A_RequestControlPacket();
            cl2038Packet.SetPara(false);
            CL2038A_RequestControlReplyPacket cl2038abrecv = new CL2038A_RequestControlReplyPacket();
            if (SendData(m_WishStandPort, cl2038Packet, cl2038abrecv))
            { return true; }
            else
            { return false; }
        }
        /// <summary>
        ///  耐压仪复位状态
        /// </summary>
        /// <returns></returns>
        public bool RequestReset()
        {
            CL2038A_RequestResetStatusPacket cl2038Packet = new CL2038A_RequestResetStatusPacket();
            CL2038A_RequestResetStatusReplyPacket cl2038abrecv = new CL2038A_RequestResetStatusReplyPacket();
            if (SendData(m_WishStandPort, cl2038Packet, cl2038abrecv))
            { return true; }
            else
            { return false; }
        }
        #endregion

        #region IControlResistanceMoto

        public bool SetMotoContorlToResistance(bool isResistance)
        {
            return true;
        }

        #endregion
    }
}
