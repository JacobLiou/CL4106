using System;
using System.Collections.Generic;

using System.Text;
using Frontier.MeterVerification.DeviceCommon;
using Frontier.MeterVerification.DeviceInterface;

namespace Frontier.MeterVerification.KLDevice
{
    /// <summary>
    /// 时基源
    /// </summary>
    public class CLTimeSync : Comm2018Device, IConnect, ITimeChannel
    {
        /// <summary>
        /// 
        /// </summary>
        private object timeSyncObject;
        /// <summary>
        /// 时基源控制端口
        /// </summary>
        private int m_TimeSourcePort = 0;

        public override void Config(List<CommSerialPortSettings> serialPortSettings, List<CommSocketSettings> socketSettings)
        {
            string strSetting;
            strSetting = string.Format("{0},{1},{2},{3}", serialPortSettings[0].BaudRate, serialPortSettings[0].Parity, serialPortSettings[0].DataBits, serialPortSettings[0].StopBits);
            m_TimeSourcePort = serialPortSettings[0].CommPortNumber + socketSettings[0].Port1;
            RegisterPort(m_TimeSourcePort, socketSettings[0].Port2, strSetting, socketSettings[0].IP, socketSettings[0].Port1, 1000, 200);

        }

        #region IConnect 成员

        public void Connected(int meterCount)
        {
            this.Open();
        }

        public void Connected(VerificationEquipment.Commons.MeterPosition[] meterPositions)
        {

        }

        public void Closed()
        {
            this.Close();
        }

        #endregion

        #region ITimeChannel 成员
        /// <summary>
        /// 设置标准脉冲通道
        /// </summary>
        /// <param name="stdPulseType">脉冲通道类型0=标准时钟脉冲，1=标准电能脉冲</param>
        /// <returns></returns>
        public bool SetTimeChannel(int stdPulseType)
        {
            CL191B_RequestSetChannelPacket cl191b = new CL191B_RequestSetChannelPacket();
            cl191b.SetPara((enmStdPulseType)stdPulseType);
            CL191B_RequestSetChannelReplyPacket cl191brecv = new CL191B_RequestSetChannelReplyPacket();
            if (SendData(m_TimeSourcePort, cl191b, cl191brecv))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
