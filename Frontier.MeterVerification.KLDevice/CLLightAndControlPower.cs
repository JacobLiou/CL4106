using System;
using System.Collections.Generic;

using System.Text;
using System.Drawing;
using VerificationEquipment.Commons;
using Frontier.MeterVerification.DeviceCommon;
using Frontier.MeterVerification.DeviceInterface;


namespace Frontier.MeterVerification.KLDevice
{
    /// <summary>
    /// 警示灯、装置电源
    /// </summary>
    public class CLLightAndControlPower : Comm2018Device, IConnect, ILight, IControlEquipmentPower, IControlResistancePower
    {
        private object lightObject = new object();

        private MeterPosition[] meterPositions;
        /// <summary>
        /// 多功能控制端口
        /// </summary>
        private int m_MultiFunctionPort = 0;
        /// <summary>
        /// 重试次数
        /// </summary>
        public static int RETRYTIEMS = 3;

        public override void Config(List<CommSerialPortSettings> serialPortSettings, List<CommSocketSettings> socketSettings)
        {
            string strSetting;
            strSetting = string.Format("{0},{1},{2},{3}", serialPortSettings[0].BaudRate, serialPortSettings[0].Parity, serialPortSettings[0].DataBits, serialPortSettings[0].StopBits);
            m_MultiFunctionPort = serialPortSettings[0].CommPortNumber + socketSettings[0].Port1;
            RegisterPort(m_MultiFunctionPort, socketSettings[0].Port2, strSetting, socketSettings[0].IP, socketSettings[0].Port1, 1000, 200);
        }

        #region IConnect 成员

        public void Connected(int meterCount)
        {
            this.Open();
        }

        public void Connected(VerificationEquipment.Commons.MeterPosition[] meterPositions)
        {
            this.meterPositions = meterPositions;
        }

        public void Closed()
        {
            this.Close();
        }
        #endregion

        /// <summary>
        /// 遥控打开设备
        /// </summary>
        /// <returns></returns>
        public bool RemoteOpenEquipment()
        {
            CL2029B_RequestControlPowerPacket cl2029b = new CL2029B_RequestControlPowerPacket();
            cl2029b.SetPara(true);
            CL2029B_RequestControlPowerReplyPacket cl2029brecv = new CL2029B_RequestControlPowerReplyPacket();
            if (SendPacketWithRetry(m_MultiFunctionPort, cl2029b, cl2029brecv))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 遥控关闭设备
        /// </summary>
        /// <returns></returns>
        public bool RemoteCloseEquipment()
        {
            CL2029B_RequestControlPowerPacket cl2029b = new CL2029B_RequestControlPowerPacket();
            cl2029b.SetPara(false);
            CL2029B_RequestControlPowerReplyPacket cl2029brecv = new CL2029B_RequestControlPowerReplyPacket();
            if (SendPacketWithRetry(m_MultiFunctionPort, cl2029b, cl2029brecv))
                return true;
            else
                return false;
        }        
        
        /// <summary>
        /// 遥控打开耐压设备
        /// </summary>
        /// <returns></returns>
        public bool RemoteOpenResistanceEquipment()
        {
            return true;
        }

        
        /// <summary>
        /// 遥控关闭耐压设备
        /// </summary>
        /// <returns></returns>
        public bool RemoteCloseResistanceEquipment()
        {
            return true;
        }
        /// 控制状态灯颜色
        /// <summary>
        /// 控制状态灯颜色
        /// </summary>
        /// <param name="color">颜色</param>
        /// <param name="isFlash">是否闪烁</param>
        public void Light(Color color, bool isFlash)
        {
            Cus_LightType lightType = Cus_LightType.关灯 ;
            if (color == Color.Yellow)
                lightType = Cus_LightType.黄灯;
            else if (color == Color.Red)
                lightType = Cus_LightType.红灯;
            else if (color == Color.Green)
                lightType = Cus_LightType.绿灯;
            else if (color == Color.Black)
                lightType = Cus_LightType.关灯;
            CL2029B_RequestSetLightPacket cl2019b = new CL2029B_RequestSetLightPacket();
            cl2019b.SetPara((int)lightType);
            Cl2029B_RequestSetLightReplyPacket cl2029brecv = new Cl2029B_RequestSetLightReplyPacket();

            SendPacketWithRetry(m_MultiFunctionPort, cl2019b, cl2029brecv);
        }

        public void GetEquipmentReverseStatus(int[] meterIndex, MeterPositionReverseStatus[] reverseStatus)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 发送报文
        /// </summary>
        /// <param name="port"></param>
        /// <param name="sp"></param>
        /// <param name="rp"></param>
        /// <returns></returns>
        private bool SendPacketWithRetry(int port, SendPacket sp, RecvPacket rp)
        {
            lock (lightObject)
            {
                for (int i = 0; i < RETRYTIEMS; i++)
                {
                    if (this.SendData(port, sp, rp) == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
