using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frontier.MeterVerification.DeviceCommon;
using Frontier.MeterVerification.DeviceInterface;
using VerificationEquipment.Commons;

namespace Frontier.MeterVerification.KLDevice
{
    public class CLPower : Comm2018Device, IConnect, IPower
    {
        private object powerObject = new object();
        /// <summary>
        /// 电表挂表信息
        /// </summary>
        private MeterPosition[] meterPositions;
        /// <summary>
        /// 
        /// </summary>
        private static float _Voltage = 0.0f;
       
        /// <summary>
        /// 重试次数
        /// </summary>
        public static int RETRYTIEMS = 3;
        /// <summary>
        /// 源控制端口
        /// </summary>
        private int m_PowerPort = 0;


        public override void Config(List<CommSerialPortSettings> serialPortSettings, List<CommSocketSettings> socketSettings)
        {
            string strSetting = string.Format("{0},{1},{2},{3}", serialPortSettings[0].BaudRate, serialPortSettings[0].Parity, serialPortSettings[0].DataBits, serialPortSettings[0].StopBits);
            m_PowerPort = serialPortSettings[0].CommPortNumber;
            RegisterPort(m_PowerPort, socketSettings[0].Port2, strSetting, socketSettings[0].IP, 2000, 200);

        }

        #region IConnect 成员

        public void Connected(VerificationEquipment.Commons.MeterPosition[] meterPositions)
        {
            this.meterPositions = meterPositions;
            this.Open();
        }

        public void Closed()
        {
            this.Close();
        }

        #endregion

        #region IPower 成员

        public bool SetWiringMode(VerificationEquipment.Commons.WiringMode wiringMode, VerificationEquipment.Commons.Pulse pulse)
        {
            return true;
        }

        public bool SetRange(float voltage, float current)
        {
            return true;
        }

        public bool SetFreq(float acFreq)
        {
            return true;
        }

        public bool SetLoadPhase(VerificationEquipment.Commons.WiringMode wiringMode, float factor, bool capacitive, VerificationEquipment.Commons.Pulse pulse, LoadPhase loadPhase)
        {
            return true;
        }

        public bool SetVerificationPulseType(VerificationElementType elementType)
        {
            return true;
        }

        public bool RaiseVoltage(float voltage, LoadPhase loadPhase, VerificationEquipment.Commons.WiringMode wiringMode, float factor, bool capacitive, VerificationEquipment.Commons.Pulse pulse)
        {
            UIPara tagUI = new UIPara();
            PhiPara tagP = new PhiPara();
            CL309_RequestPowerOnPacket rcpower = new CL309_RequestPowerOnPacket();
            tagUI.Ua = voltage;
            tagUI.Ub = voltage;
            tagUI.Uc = voltage;
            tagUI.Ia = 0;
            tagUI.Ib = 0;
            tagUI.Ic = 0;

            tagP.PhiIa = 0;
            tagP.PhiIb = 120;
            tagP.PhiIc = 240;
            tagP.PhiUa = 0;
            tagP.PhiUb = 120;
            tagP.PhiUc = 240;
            Cus_PowerYuanJiang phase;
            switch (loadPhase)
            {
                case LoadPhase.None:
                case LoadPhase.AC:
                    phase = Cus_PowerYuanJiang.H;
                    break;
                case LoadPhase.A:
                    phase = Cus_PowerYuanJiang.A;
                    break;
                case LoadPhase.B:
                    phase = Cus_PowerYuanJiang.B;
                    break;
                case LoadPhase.C:
                    phase = Cus_PowerYuanJiang.C;
                    break;
                default:
                    phase = Cus_PowerYuanJiang.H;
                    break;
            }

            rcpower.SetPara(tagUI, tagP, phase, "1.0", 50, 0, 63, false, false, false, false);
            Cl309_RequestPowerOnReplyPacket recv2 = new Cl309_RequestPowerOnReplyPacket();
            _Voltage = voltage;
            if (!SendPacketWithRetry(m_PowerPort, rcpower, recv2))
            {
                return false;
            }

            return true;
        }

        public bool RaiseCurrent(float current, LoadPhase loadPhase, VerificationEquipment.Commons.WiringMode wiringMode, float factor, bool capacitive, VerificationEquipment.Commons.Pulse pulse)
        {
            UIPara tagUI = new UIPara();
            PhiPara tagP = new PhiPara();
            CL309_RequestPowerOnPacket rcpower = new CL309_RequestPowerOnPacket();
            tagUI.Ua = _Voltage;
            tagUI.Ub = _Voltage;
            tagUI.Uc = _Voltage;
            tagUI.Ia = current;
            tagUI.Ib = current;
            tagUI.Ic = current;

            tagP.PhiIa = 0;
            tagP.PhiIb = 120;
            tagP.PhiIc = 240;
            tagP.PhiUa = 0;
            tagP.PhiUb = 120;
            tagP.PhiUc = 240;

            Cus_PowerYuanJiang phase;
            switch (loadPhase)
            {
                case LoadPhase.None:
                case LoadPhase.AC:
                    phase = Cus_PowerYuanJiang.H;
                    break;
                case LoadPhase.A:
                    phase = Cus_PowerYuanJiang.A;
                    break;
                case LoadPhase.B:
                    phase = Cus_PowerYuanJiang.B;
                    break;
                case LoadPhase.C:
                    phase = Cus_PowerYuanJiang.C;
                    break;
                default:
                    phase = Cus_PowerYuanJiang.H;
                    break;
            }

            rcpower.SetPara(tagUI, tagP, phase, "1.0", 50, 1, 63, false, false, false, false);
            Cl309_RequestPowerOnReplyPacket recv2 = new Cl309_RequestPowerOnReplyPacket();
            if (!SendPacketWithRetry(m_PowerPort, rcpower, recv2))
            {
                return false;
            }
            return true;
        }

        public bool RaiseVotageCurrent(float voltage, float current, LoadPhase loadPhase, VerificationEquipment.Commons.WiringMode wiringMode)
        {
            UIPara tagUI = new UIPara();
            PhiPara tagP = new PhiPara();
            CL309_RequestPowerOnPacket rcpower = new CL309_RequestPowerOnPacket();
            tagUI.Ua = voltage;
            tagUI.Ub = voltage;
            tagUI.Uc = voltage;
            tagUI.Ia = current;
            tagUI.Ib = current;
            tagUI.Ic = current;

            tagP.PhiIa = 0;
            tagP.PhiIb = 120;
            tagP.PhiIc = 240;
            tagP.PhiUa = 0;
            tagP.PhiUb = 120;
            tagP.PhiUc = 240;

            Cus_PowerYuanJiang phase;
            switch (loadPhase)
            {
                case LoadPhase.None:
                case LoadPhase.AC:
                    phase = Cus_PowerYuanJiang.H;
                    break;
                case LoadPhase.A:
                    phase = Cus_PowerYuanJiang.A;
                    break;
                case LoadPhase.B:
                    phase = Cus_PowerYuanJiang.B;
                    break;
                case LoadPhase.C:
                    phase = Cus_PowerYuanJiang.C;
                    break;
                default:
                    phase = Cus_PowerYuanJiang.H;
                    break;
            }

            rcpower.SetPara(tagUI, tagP, phase, "1.0", 50, 0, 63, false, false, false, false);
            Cl309_RequestPowerOnReplyPacket recv2 = new Cl309_RequestPowerOnReplyPacket();
            if (!SendPacketWithRetry(m_PowerPort, rcpower, recv2))
            {
                return false;
            }


            return true;
        }

        public bool StopVerification()
        {
            CL309_RequestPowerOnPacket rcpower = new CL309_RequestPowerOnPacket();
            Cl309_RequestPowerOnReplyPacket recv2 = new Cl309_RequestPowerOnReplyPacket();
            UIPara uipara;
            PhiPara phipara;
            uipara.Ia = 0;
            uipara.Ib = 0;
            uipara.Ic = 0;
            uipara.Ua = 0;
            uipara.Ub = 0;
            uipara.Uc = 0;
            phipara.PhiIa = 0;
            phipara.PhiIb = 0;
            phipara.PhiIc = 0;
            phipara.PhiUa = 0;
            phipara.PhiUb = 0;
            phipara.PhiUc = 0;
            rcpower.SetPara(uipara, phipara, Cus_PowerYuanJiang.H, "1.0", 50, 7, 0x00, false, false, false, false);

            if (!SendPacketWithRetry(m_PowerPort, rcpower, recv2))
            {
                return false;
            }

            return true;
        }
        #endregion

        /// <summary>
        /// 发送报文
        /// </summary>
        /// <param name="port"></param>
        /// <param name="sp"></param>
        /// <param name="rp"></param>
        /// <returns></returns>
        private bool SendPacketWithRetry(int port, SendPacket sp, RecvPacket rp)
        {
            lock (powerObject)
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



        public void Connected(int meterPositionCount)
        {
            
        }
    }
}
