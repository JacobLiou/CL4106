using System;
using System.Collections.Generic;

using System.Text;
using Frontier.MeterVerification.DeviceCommon;
using Frontier.MeterVerification.DeviceInterface;
using VerificationEquipment.Commons;

namespace Frontier.MeterVerification.KLDevice
{
    /// <summary>
    /// 功耗板
    /// </summary>
    public class CLPowerConsumePlate : Comm2018Device, IConnect, IPowerConsume
    {
        /// <summary>
        /// 功耗板板端口
        /// </summary>
        private int[] m_arrPowerConsumePort = new int[0];

        /// <summary>
        /// 配置端口
        /// </summary>
        /// <param name="serialPortSettings"></param>
        /// <param name="socketSettings"></param>
        public override void Config(List<CommSerialPortSettings> serialPortSettings, List<CommSocketSettings> socketSettings)
        {
            int intInc = 0;
            string strSetting;
            m_arrPowerConsumePort = new int[socketSettings.Count];
            for (intInc = 0; intInc < serialPortSettings.Count; intInc++)
            {
                strSetting = string.Format("{0},{1},{2},{3}", serialPortSettings[intInc].BaudRate, serialPortSettings[intInc].Parity, serialPortSettings[intInc].DataBits, serialPortSettings[intInc].StopBits);
                m_arrPowerConsumePort[intInc] = serialPortSettings[intInc].CommPortNumber;
                RegisterPort(m_arrPowerConsumePort[intInc], socketSettings[intInc].Port2, strSetting, socketSettings[0].IP, socketSettings[0].Port1, 1000, 200);
            }
        }

        #region IPowerConsume 成员
        /// <summary>
        /// 获取功耗
        /// </summary>
        /// <param name="meterIndex">表位数组</param>
        /// <param name="powerConsume">功耗</param>
        /// <returns></returns>
        public bool ReadPowerConsume(int[] meterIndex, out PowerConsume[] powerConsume)
        {
            PowerConsume[] _power = new PowerConsume[meterIndex.Length];
            powerConsume = _power;
            return true;
        }
        #endregion

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

    }
}
