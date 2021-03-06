using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frontier.MeterVerification.DeviceCommon;
using Frontier.MeterVerification.DeviceInterface;
using VerificationEquipment.Commons;

namespace Frontier.MeterVerification.KLDevice
{
    /// <summary>
    /// 标准表
    /// </summary>
    public class CLStdMeter : Comm2018Device, IConnect, IStdMeter, IMonitor
    {
        private object stdmeterObject = new object();
        /// <summary>
        /// 重试次数
        /// </summary>
        public static int RETRYTIEMS = 3;
        /// <summary>
        /// 标准表控制端口
        /// </summary>
        private int m_MeterPort = 0;

        public override void Config(List<CommSerialPortSettings> serialPortSettings, List<CommSocketSettings> socketSettings)
        {
            string strSetting;
            strSetting = string.Format("{0},{1},{2},{3}"
                , serialPortSettings[0].BaudRate
                , serialPortSettings[0].Parity
                , serialPortSettings[0].DataBits
                , serialPortSettings[0].StopBits);
            m_MeterPort = serialPortSettings[0].CommPortNumber;
            RegisterPort(m_MeterPort, socketSettings[0].Port2, strSetting, socketSettings[0].IP, 2000, 200);

        }

        #region IConnect 成员

        public void Connected(VerificationEquipment.Commons.MeterPosition[] meterPositions)
        {
            this.Open();
        }

        public void Closed()
        {
            this.Close();
        }

        #endregion

        #region IStdMeter 成员

        public bool SetStdMeterWiringMode(VerificationElementType elementType, WiringMode wiringMode, Pulse pulse)
        {
            Cus_Clfs clfs;
            Cus_PowerFangXiang glfx;
            bool bAuto = false;
            if (wiringMode == VerificationEquipment.Commons.WiringMode.三相四线)
            {
                clfs = Cus_Clfs.三相四线;
            }
            else if (wiringMode == VerificationEquipment.Commons.WiringMode.三相三线)
            {
                clfs = Cus_Clfs.三相三线;
            }
            else if (wiringMode == VerificationEquipment.Commons.WiringMode.单相)
            {
                clfs = Cus_Clfs.单相;
            }
            else
            {
                clfs = Cus_Clfs.三相四线;
            }
            if (elementType == VerificationElementType.常数校核试验
                || elementType == VerificationElementType.电能走字试验
                || elementType == VerificationElementType.定数走字试验)
            {
                bAuto = false;
            }
            else
            {
                bAuto = true;
            }

            glfx = (Cus_PowerFangXiang)pulse;
            CL3115_RequestSetParaPacket CL3115para = new CL3115_RequestSetParaPacket();
            CL3115para.SetPara(clfs, glfx, bAuto);
            CL3115_RequestSetParaReplayPacket CL3115pararecv = new CL3115_RequestSetParaReplayPacket();
            if (SendData(m_MeterPort, CL3115para, CL3115pararecv))
            {
                if (CL3115pararecv.ReciveResult != RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            // 启动标准表
            StartStdMeter(elementType);

            return true;
        }

        /// <summary>
        /// 设置标准表档位
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="voltage"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        public bool SetStdMeterGrade(VerificationElementType elementType, float voltage, float current)
        {
            if (elementType != VerificationElementType.常数校核试验
                && elementType == VerificationElementType.电能走字试验
                && elementType == VerificationElementType.定数走字试验)
            {
                return true;
            }
            Cus_StdMeterVDangWei Dvoltage = new Cus_StdMeterVDangWei();
            Cus_StdMeterIDangWei Icurrent = new Cus_StdMeterIDangWei();
            // 电压档位
            if (voltage > 240)
            {
                Dvoltage = Cus_StdMeterVDangWei.档位480V;
            }
            else if (voltage > 120)
            {
                Dvoltage = Cus_StdMeterVDangWei.档位240V;
            }
            else if (voltage > 60)
            {
                Dvoltage = Cus_StdMeterVDangWei.档位120V;
            }
            else
            {
                Dvoltage = Cus_StdMeterVDangWei.档位60V;
            }

            // 电流档位
            if (current > 50)
            {
                Icurrent = Cus_StdMeterIDangWei.档位100A;
            }
            else if (current > 20)
            {
                Icurrent = Cus_StdMeterIDangWei.档位50A;
            }
            else if (current > 10)
            {
                Icurrent = Cus_StdMeterIDangWei.档位20A;
            }
            else if (current > 5)
            {
                Icurrent = Cus_StdMeterIDangWei.档位10A;
            }
            else if (current > 2)
            {
                Icurrent = Cus_StdMeterIDangWei.档位5A;
            }
            else if (current > 1)
            {
                Icurrent = Cus_StdMeterIDangWei.档位2A;
            }
            else if (current > 0.5)
            {
                Icurrent = Cus_StdMeterIDangWei.档位1A;
            }
            else if (current > 0.2)
            {
                Icurrent = Cus_StdMeterIDangWei.档位05A;
            }
            else if (current > 0.1)
            {
                Icurrent = Cus_StdMeterIDangWei.档位02A;
            }
            else if (current > 0.05)
            {
                Icurrent = Cus_StdMeterIDangWei.档位01A;
            }
            else if (current > 0.02)
            {
                Icurrent = Cus_StdMeterIDangWei.档位005A;
            }
            else if (current > 0.01)
            {
                Icurrent = Cus_StdMeterIDangWei.档位002A;
            }
            else
            {
                Icurrent = Cus_StdMeterIDangWei.档位001A;
            }


            CL3115_RequestSetStdMeterDangWeiPacket cl3115 = new CL3115_RequestSetStdMeterDangWeiPacket(Dvoltage, Icurrent);
            CL3115_RequestSetStdMeterDangWeiReplayPacket cl3115rec = new CL3115_RequestSetStdMeterDangWeiReplayPacket();

            if (SendPacketWithRetry(m_MeterPort, cl3115, cl3115rec)
                && cl3115rec.ReciveResult == RecvResult.OK)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 设置标准表常数
        /// </summary>
        /// <param name="voltage"></param>
        /// <param name="current"></param>
        /// <param name="wiringMode"></param>
        /// <returns></returns>
        public bool SetStdMeterConst(float voltage, float current, WiringMode wiringMode)
        {
            bool bResult = false;
            CL3115_RequestSetStdMeterConstPacket cl3115 = new CL3115_RequestSetStdMeterConstPacket();
            CL3115_RequestSetStdMeterConstReplayPacket cl3115rec = new CL3115_RequestSetStdMeterConstReplayPacket();

            cl3115.SetPara(4000000);
            if (SendPacketWithRetry(m_MeterPort, cl3115, cl3115rec)
                && cl3115rec.ReciveResult == RecvResult.OK)
            {
                bResult = true;
            }
            return bResult;
        }

        /// <summary>
        /// 获取标准表脉冲个数
        /// </summary>
        /// <returns></returns>
        public int GetStdMeterPulseCount()
        {
            int iStdMeterPulseCount = 0;
            CL3115_RequestReadStdMeterTotalPulseNumPacket cl3115 = new CL3115_RequestReadStdMeterTotalPulseNumPacket();
            CL3115_RequestReadStdMeterTotalPulseNumReplayPacket cl3115rec = new CL3115_RequestReadStdMeterTotalPulseNumReplayPacket();

            if (SendPacketWithRetry(m_MeterPort, cl3115, cl3115rec)
                && cl3115rec.ReciveResult == RecvResult.OK)
            {
                iStdMeterPulseCount = Convert.ToInt32(cl3115rec.Pulsenum);
            }
            return iStdMeterPulseCount;
        }

        /// <summary>
        /// 获取标准表电能
        /// </summary>
        /// <returns></returns>
        public float GetStdMeterEnergy()
        {
            float fStdMeterEnergy = 0.0f;
            CL3115_RequestReadStdMeterTotalNumPacket cl3115 = new CL3115_RequestReadStdMeterTotalNumPacket();
            CL3115_RequestReadStdMeterTotalNumReplayPacket cl3115rec = new CL3115_RequestReadStdMeterTotalNumReplayPacket();
            if (SendPacketWithRetry(m_MeterPort, cl3115, cl3115rec))
                fStdMeterEnergy = cl3115rec.MeterTotalNum;
            return fStdMeterEnergy;
        }

        /// <summary>
        /// 停止标准表
        /// </summary>
        /// <returns></returns>
        public bool StopStdMeter()
        {
            CL3115_RequestStartTaskPacket cl3115 = new CL3115_RequestStartTaskPacket();
            CL3115_RequestStartTaskReplyPacket cl3115rec = new CL3115_RequestStartTaskReplyPacket();
            cl3115.SetPara(0);
            if (this.SendPacketWithRetry(m_MeterPort, cl3115, cl3115rec) == false
                || cl3115rec.ReciveResult != RecvResult.OK)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 启动标准表
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        private bool StartStdMeter(VerificationElementType elementType)
        {
            CL3115_RequestStartTaskPacket cl3115 = new CL3115_RequestStartTaskPacket();
            CL3115_RequestStartTaskReplyPacket cl3115rec = new CL3115_RequestStartTaskReplyPacket();

            int typeParam = 1;
            if (elementType == VerificationElementType.电能走字试验
                || elementType == VerificationElementType.定数走字试验)
            {
                typeParam = 2;
            }

            cl3115.SetPara(typeParam);
            if (this.SendPacketWithRetry(m_MeterPort, cl3115, cl3115rec) == false
                || cl3115rec.ReciveResult != RecvResult.OK)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region IMonitor 成员
        /// 读取检定装置当前负载
        /// <summary>
        /// 读取检定装置当前负载
        /// </summary>
        /// <param name="U">电压</param>
        /// <param name="I">电流</param>
        /// <param name="P">有功功率</param>
        /// <param name="Q">无功功率</param>
        /// <param name="angle">角度</param>
        /// <param name="acFreq">频率</param>
        public void GetCurrentLoad(out string[] U, out string[] I, out string[] P, out string[] Q, out string[] angle, out string acFreq)
        {
            int iArrayLength = 3;
            U = new string[iArrayLength];
            I = new string[iArrayLength];
            P = new string[iArrayLength + 1];
            Q = new string[iArrayLength + 1];
            angle = new string[iArrayLength];
            acFreq = "50";
            CL3115_RequestReadStdInfoPacket cl3115 = new CL3115_RequestReadStdInfoPacket();
            CL3115_RequestReadStdInfoReplayPacket cl3115rec = new CL3115_RequestReadStdInfoReplayPacket();
            if (this.SendPacketWithRetry(m_MeterPort, cl3115, cl3115rec))
            {
                U[0] = cl3115rec.PowerInfo.Ua.ToString();
                U[1] = cl3115rec.PowerInfo.Ub.ToString();
                U[2] = cl3115rec.PowerInfo.Uc.ToString();

                I[0] = cl3115rec.PowerInfo.Ia.ToString();
                I[1] = cl3115rec.PowerInfo.Ib.ToString();
                I[2] = cl3115rec.PowerInfo.Ic.ToString();

                P[0] = cl3115rec.PowerInfo.P.ToString();
                P[1] = cl3115rec.PowerInfo.Pa.ToString();
                P[2] = cl3115rec.PowerInfo.Pb.ToString();
                P[3] = cl3115rec.PowerInfo.Pc.ToString();

                Q[0] = cl3115rec.PowerInfo.Q.ToString();
                Q[1] = cl3115rec.PowerInfo.Qa.ToString();
                Q[2] = cl3115rec.PowerInfo.Qb.ToString();
                Q[3] = cl3115rec.PowerInfo.Qc.ToString();

                angle[0] = (cl3115rec.PowerInfo.Phi_Ua - cl3115rec.PowerInfo.Phi_Ia).ToString();
                angle[1] = (cl3115rec.PowerInfo.Phi_Ub - cl3115rec.PowerInfo.Phi_Ib).ToString();
                angle[2] = (cl3115rec.PowerInfo.Phi_Uc - cl3115rec.PowerInfo.Phi_Ic).ToString();

                acFreq = cl3115rec.PowerInfo.Freq.ToString();

                GlobalUnit.g_StdMeterInfo = cl3115rec.PowerInfo;
            }
        }
        /// <summary>
        /// 获取当前负载
        /// </summary>
        /// <returns></returns>
        public MonitorData GetCurrentLoad()
        {
            MonitorData monitordata = new MonitorData();
            int iArrayLength = 3;
            string[] U = new string[iArrayLength];
            string[] I = new string[iArrayLength];
            string[] P = new string[iArrayLength + 1];
            string[] Q = new string[iArrayLength + 1];
            string[] angle = new string[iArrayLength];
            string acFreq = "50";
            CL3115_RequestReadStdInfoPacket cl3115 = new CL3115_RequestReadStdInfoPacket();
            CL3115_RequestReadStdInfoReplayPacket cl3115rec = new CL3115_RequestReadStdInfoReplayPacket();
            if (this.SendPacketWithRetry(m_MeterPort, cl3115, cl3115rec))
            {
                U[0] = cl3115rec.PowerInfo.Ua.ToString();
                U[1] = cl3115rec.PowerInfo.Ub.ToString();
                U[2] = cl3115rec.PowerInfo.Uc.ToString();

                I[0] = cl3115rec.PowerInfo.Ia.ToString();
                I[1] = cl3115rec.PowerInfo.Ib.ToString();
                I[2] = cl3115rec.PowerInfo.Ic.ToString();

                P[0] = cl3115rec.PowerInfo.P.ToString();
                P[1] = cl3115rec.PowerInfo.Pa.ToString();
                P[2] = cl3115rec.PowerInfo.Pb.ToString();
                P[3] = cl3115rec.PowerInfo.Pc.ToString();

                angle[0] = (cl3115rec.PowerInfo.Phi_Ua - cl3115rec.PowerInfo.Phi_Ia).ToString();
                angle[1] = (cl3115rec.PowerInfo.Phi_Ub - cl3115rec.PowerInfo.Phi_Ib).ToString();
                angle[2] = (cl3115rec.PowerInfo.Phi_Uc - cl3115rec.PowerInfo.Phi_Ic).ToString();

                acFreq = cl3115rec.PowerInfo.Freq.ToString();

                monitordata.U = U;
                monitordata.I = I;
                monitordata.P = P;
                monitordata.Q = Q;
                monitordata.Freq = acFreq;
                monitordata.Angle = angle;
            }
            return monitordata;
        }

        #endregion

        #region 私有成员
        private bool SendPacketWithRetry(int port, SendPacket sp, RecvPacket rp)
        {
            //加锁 防止通讯冲突
            lock (stdmeterObject)
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

        #endregion


        public void Connected(int meterPositionCount)
        {
            
        }
    }
}
