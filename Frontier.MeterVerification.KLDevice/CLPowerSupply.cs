using System;
using System.Collections.Generic;

using System.Text;
using VerificationEquipment.Commons;
using Frontier.MeterVerification.DeviceCommon;
using Frontier.MeterVerification.DeviceInterface;

namespace Frontier.MeterVerification.KLDevice
{
    /// <summary>
    /// 多功能控制板
    /// </summary>
    public class CLPowerSupply : Comm2018Device, IConnect, IPowerSupply
    {
        /// <summary>
        /// 电表挂表信息
        /// </summary>
        private MeterPosition[] meterPositions;

        /// <summary>
        /// 继电器控制端口
        /// </summary>
        private int m_SwitchPort = 0;

        public override void Config(List<CommSerialPortSettings> serialPortSettings, List<CommSocketSettings> socketSettings)
        {
            string strSetting = string.Format("{0},{1},{2},{3}", serialPortSettings[0].BaudRate, serialPortSettings[0].Parity, serialPortSettings[0].DataBits, serialPortSettings[0].StopBits);
            m_SwitchPort = serialPortSettings[0].CommPortNumber + socketSettings[0].Port1;
            RegisterPort(m_SwitchPort, socketSettings[0].Port2, strSetting, socketSettings[0].IP, socketSettings[0].Port1, 1500, 200);
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

        #region IPowerSupply 成员

        /// <summary>
        /// 供电类型，耐压供电、载波供电、普通供电
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        public bool SetPowerSupplyType(VerificationElementType elementType)
        {
            CL2029D_RequestSetSwitchPacket cl2029d = new CL2029D_RequestSetSwitchPacket();
            CL2029D_RequestSetSwitchReplyPacket cl2029drec = new CL2029D_RequestSetSwitchReplyPacket();


            //耐压供电
            if (elementType == VerificationElementType.耐压试验)
            {
                cl2029d.SetPara(1, 0);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
                cl2029d.SetPara(3, 0);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
                cl2029d.SetPara(4, 0);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
                cl2029d.SetPara(5, 0);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
                cl2029d.SetPara(6, 0);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
                cl2029d.SetPara(7, 0);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
                cl2029d.SetPara(8, 1);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
                cl2029d.SetPara(9, 1);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
                //cl2029d.SetPara(10, 1);
                //if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                //{
                //    return false;
                //}
                //cl2029d.SetPara(11, 0);
                //if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                //{
                //    return false;
                //}
            }
            //载波供电
            else if (elementType == VerificationElementType.载波试验)
            {
                cl2029d.SetPara(1, 0);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
                cl2029d.SetPara(3, 0);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
                cl2029d.SetPara(5, 0);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
                cl2029d.SetPara(6, 0);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
                cl2029d.SetPara(7, 0);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
                cl2029d.SetPara(8, 0);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                    return false;
                cl2029d.SetPara(9, 0);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
                cl2029d.SetPara(10, 0);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
                cl2029d.SetPara(11, 1);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
            }
            else //普通供电
            {
                cl2029d.SetPara(1, 1);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
                cl2029d.SetPara(3, 1);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
                cl2029d.SetPara(4, 1);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
                cl2029d.SetPara(5, 1);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
                cl2029d.SetPara(6, 1);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
                cl2029d.SetPara(7, 1);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
                cl2029d.SetPara(8, 0);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
                cl2029d.SetPara(9, 0);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
                cl2029d.SetPara(10, 0);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
                cl2029d.SetPara(11, 0);
                if (!SendData(m_SwitchPort, cl2029d, cl2029drec))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}
