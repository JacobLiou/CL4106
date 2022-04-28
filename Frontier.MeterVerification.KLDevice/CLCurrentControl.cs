using System;
using System.Collections.Generic;

using System.Text;
using VerificationEquipment.Commons;
using Frontier.MeterVerification.DeviceCommon;
using Frontier.MeterVerification.DeviceInterface;

namespace Frontier.MeterVerification.KLDevice
{
    /// <summary>
    /// CT档位控制器
    /// </summary>
    public class CLCurrentControl : Comm2018Device, IConnect, ICurrentControl
    {
        /// <summary>
        /// 电表挂表信息
        /// </summary>
        private MeterPosition[] meterPositions;
        /// <summary>
        /// 源控制端口
        /// </summary>
        private int m_CTControlPort = 0;

        public override void Config(List<CommSerialPortSettings> serialPortSettings, List<CommSocketSettings> socketSettings)
        {
            string strSetting = string.Format("{0},{1},{2},{3}", serialPortSettings[0].BaudRate, serialPortSettings[0].Parity, serialPortSettings[0].DataBits, serialPortSettings[0].StopBits);
            m_CTControlPort = serialPortSettings[0].CommPortNumber + socketSettings[0].Port1;
            RegisterPort(m_CTControlPort, socketSettings[0].Port2, strSetting, socketSettings[0].IP, socketSettings[0].Port1, 1000, 200);
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

        #region ICurrentControl 成员
        /// <summary>
        /// 三相装置需设置CT档位控制器
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public bool ISetCurrentControl(float current)
        {
            bool bResult = false;
            //清除过载信号
            CL2030_RequestClearOverPacket ClearOver = new CL2030_RequestClearOverPacket();
            CL2030_RequestClearOverReplyPacket CleaOverRec = new CL2030_RequestClearOverReplyPacket();
            if (SendData(m_CTControlPort, ClearOver, CleaOverRec))
            {
                bResult = true;
            }
            else
            {
                bResult = false;
            }
            //设置CT接线方式
            CL2030_RequestSetLinkTypePacket SetLink = new CL2030_RequestSetLinkTypePacket();
            CL2030_RequestSetLinkTypeReplyPacket SetLinkRec = new CL2030_RequestSetLinkTypeReplyPacket();
            if (SendData(m_CTControlPort, SetLink, SetLinkRec) && bResult)
            {
                bResult = true;
            }
            else
            {
                bResult = false;
            }
            //设置CT档位
            CL2030_RequestSwitchPacket Switch = new CL2030_RequestSwitchPacket();
            CL2030_RequestSwitchReplyPacket SwitchRec = new CL2030_RequestSwitchReplyPacket();
            Switch.SetPara(current);
            if (SendData(m_CTControlPort, Switch, SwitchRec) && bResult)
            {
                bResult = true;
            }
            else
            {
                bResult = false;
            }

            return bResult;
        }
        #endregion

        public bool SetCurrentControl(float current)
        {
            bool bResult = false;
            //清除过载信号
            CL2030_RequestClearOverPacket ClearOver = new CL2030_RequestClearOverPacket();
            CL2030_RequestClearOverReplyPacket CleaOverRec = new CL2030_RequestClearOverReplyPacket();
            if (SendData(m_CTControlPort, ClearOver, CleaOverRec))
            {
                bResult = true;
            }
            else
            {
                bResult = false;
            }
            //设置CT接线方式
            CL2030_RequestSetLinkTypePacket SetLink = new CL2030_RequestSetLinkTypePacket();
            CL2030_RequestSetLinkTypeReplyPacket SetLinkRec = new CL2030_RequestSetLinkTypeReplyPacket();
            if (SendData(m_CTControlPort, SetLink, SetLinkRec) && bResult)
            {
                bResult = true;
            }
            else
            {
                bResult = false;
            }
            //设置CT档位
            CL2030_RequestSwitchPacket Switch = new CL2030_RequestSwitchPacket();
            CL2030_RequestSwitchReplyPacket SwitchRec = new CL2030_RequestSwitchReplyPacket();
            Switch.SetPara(current);
            if (SendData(m_CTControlPort, Switch, SwitchRec) && bResult)
            {
                bResult = true;
            }
            else
            {
                bResult = false;
            }

            return bResult;
        }
    }
}
