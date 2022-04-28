using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frontier.MeterVerification.DeviceCommon;
using Frontier.MeterVerification.DeviceInterface;
using VerificationEquipment.Commons;
namespace Frontier.MeterVerification.KLDevice
{
    public class CLErrorPlate : Comm2018Device, IConnect, IWcfk, ITemperature, IControlPressMotor, IResistanceWcfk, IEquipmentStatus, IMeterPositionStatus, ICalcTime, IControlResistancePower
    {
        private object[] ErrorPlateObject = new object[0];
        /// <summary>
        /// 检定类型
        /// </summary>
        private Cus_CheckType _CheckType = Cus_CheckType.电能误差;
        /// <summary>
        /// 多功能误差板通道号
        /// </summary>
        private Cus_DgnWcChannelNo _DgnWcChannelNo = Cus_DgnWcChannelNo.电能误差;
        /// <summary>
        /// 
        /// </summary>
        private Cus_GyGyType _GyGyType = Cus_GyGyType.共阴;
        /// <summary>
        /// 光点头类型
        /// </summary>
        private Cus_PulseType _PulseType = Cus_PulseType.脉冲盒;
        /// <summary>
        /// 电表误差通道
        /// </summary>
        private Cus_MeterWcChannelNo _MeterWcChannelNo;
        /// <summary>
        /// 误差板端口
        /// </summary>
        private int[] m_arrErrorPort = new int[0];
        /// <summary>
        /// 
        /// </summary>
        private bool[] bSelectBw;

        private bool[] bDefaultSelectBw;
        /// <summary>
        /// 表圈数
        /// </summary>
        private int _intMeterCircle = 1;
        /// <summary>
        /// 
        /// </summary>
        private MeterPosition[] _meterPositions;
        /// <summary>
        /// 重试次数
        /// </summary>
        public static int RETRYTIEMS = 2;

        private object locker = new object();

        Meter _meter = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialPortSettings"></param>
        /// <param name="socketSettings"></param>
        public override void Config(List<CommSerialPortSettings> serialPortSettings, List<CommSocketSettings> socketSettings)
        {
            int intInc = 0;
            string strSetting;
            m_arrErrorPort = new int[serialPortSettings.Count];
            for (intInc = 0; intInc < serialPortSettings.Count; intInc++)
            {
                strSetting = string.Format("{0},{1},{2},{3}", serialPortSettings[intInc].BaudRate, serialPortSettings[intInc].Parity, serialPortSettings[intInc].DataBits, serialPortSettings[intInc].StopBits);
                m_arrErrorPort[intInc] = serialPortSettings[intInc].CommPortNumber + socketSettings[0].Port1;
                RegisterPort(m_arrErrorPort[intInc], socketSettings[0].Port2, strSetting, socketSettings[0].IP, socketSettings[0].Port1, 500, 60);
            }
        }

        #region IWcfk 成员

        /// <summary>
        /// 初始化误差板
        /// </summary>
        /// <returns></returns>
        public bool InitWcfk()
        {
            if (GlobalUnit.DriverTypes[3] == '0')
            {
                return CL188L_INIT();
            }
            else if (GlobalUnit.DriverTypes[3] == '1')
            {
                return CL188E_INIT();
            }
            else if (GlobalUnit.DriverTypes[3] == '2')
            {
                return CL188G_INIT();
            }
            else
            {
                return CL188G_INIT();
            }
        }

        private bool CL188G_INIT()
        {
            throw new NotImplementedException();
        }

        private bool CL188E_INIT()
        {
            CL188_RequestStartStopPacket rstop = new CL188_RequestStartStopPacket();
            rstop.ControlTypes = CL188_RequestStartStopPacket.ControlType.停止当前功能;

            CL188_RequestReadStatusReplyPacket recv = new CL188_RequestReadStatusReplyPacket();
            for (int j = 0; j < m_arrErrorPort.Length; j++)
            {
                if (getErrorStates(j))
                {
                    //for (int o = 0; o < _meterPositions.Length; o++)
                    {
                        rstop.Pos = 0xFF;//Convert.ToByte(o+1);
                        if (!SendPacketWithRetry(m_arrErrorPort[j], rstop, recv,true))
                        {
                            //return false;
                        }
                    }
                }
            }

            System.Threading.Thread.Sleep(200);
            return true;

        }

        private bool CL188L_INIT()
        {
            //清除表位状态
            CL188L_RequestClearBwStatusPacket cl188l = new CL188L_RequestClearBwStatusPacket();
            CL188L_RequestClearBwStatusReplayPacket cl188lrec = new CL188L_RequestClearBwStatusReplayPacket();
            for (int j = 0; j < m_arrErrorPort.Length; j++)
            {
                if (getErrorStates(j))
                {
                    cl188l.Pos = 0;
                    cl188l.ChannelNo = j;
                    cl188l.ChannelNum = m_arrErrorPort.Length;
                    cl188l.SetPara(bSelectBw, true);
                    if (!SendPacketWithRetry(m_arrErrorPort[j], cl188l, cl188lrec,true))
                    {
                        //return false;
                    }
                }
            }
            //电压电流隔离控制
            CL188L_RequestSeperateBwControlPacket cl188ls = new CL188L_RequestSeperateBwControlPacket();
            CL188L_RequestSeperateBwControlReplayPacket cl188srec = new CL188L_RequestSeperateBwControlReplayPacket();
            for (int j = 0; j < m_arrErrorPort.Length; j++)
            {
                cl188ls.Pos = 0;
                cl188ls.ChannelNo = j;
                cl188ls.ChannelNum = m_arrErrorPort.Length;
                cl188ls.SetPara(j, bSelectBw, false);
                if (!SendPacketWithRetry(m_arrErrorPort[j], cl188ls, cl188srec,true))
                {
                    //return false;
                }

                System.Threading.Thread.Sleep(500);
                cl188ls.SetPara(j, bSelectBw, true);
                if (!SendPacketWithRetry(m_arrErrorPort[j], cl188ls, cl188srec, true))
                {
                    //return false;
                }
            }


            return true;
        }
        /// <summary>
        /// 判断当前误差板是否挂表
        /// </summary>
        /// <param name="errorNo"></param>
        /// <returns></returns>
        public bool getErrorStates(int errorNo)
        {
            bool ErrorStates = false;

            int oneErrorNum = 15;//一排所挂表数目 单项四排每排15个表位，三相两排每排10个表位
            if (m_arrErrorPort.Length == 2)
            {
                oneErrorNum = 10;
            }
            for (int i = 0; i < bSelectBw.Length; i++)
            {
                if (i >= errorNo * oneErrorNum && i < (errorNo + 1) * oneErrorNum)
                {
                    if (bSelectBw[i])
                    { ErrorStates = true; continue; }
                }
            }
            return ErrorStates;
        }


        



        /// <summary>
        /// 设置检定类型
        /// </summary>
        /// <param name="elementType">检定类型</param>
        /// <returns></returns>
        public bool SetVerificationType(VerificationElementType elementType)
        {
            _CheckType = GetCheckType(elementType);
            return true;
        }

        
        


        /// <summary>
        /// 设置脉冲通道
        /// </summary>
        /// <param name="elementType">检定类型</param>
        /// <param name="pulse">脉冲方式</param>
        /// <returns></returns>
        public bool SetPulseChannel(VerificationElementType elementType, Pulse pulse)
        {
            if (GlobalUnit.DriverTypes[3] == '0')
            {
                return Set188LPulseChannel(elementType, pulse);
            }
            else if (GlobalUnit.DriverTypes[3] == '1')
            {
                return Set188EPulseChannel(elementType, pulse);
            }
            else if (GlobalUnit.DriverTypes[3] == '2')
            {
                return Set188GPulseChannel(elementType, pulse);
            }
            else
            {
                return Set188HPulseChannel(elementType, pulse);
            }

            return true;
        }
        /// <summary>
        /// 设置188L 脉冲通道
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="pulse"></param>
        /// <returns></returns>
        private bool Set188LPulseChannel(VerificationElementType elementType, Pulse pulse)
        {
            switch (elementType)
            {
                case VerificationElementType.日计时误差试验:
                case VerificationElementType.时段投切:
                    _DgnWcChannelNo = Cus_DgnWcChannelNo.日计时脉冲;
                    break;
                case VerificationElementType.需量周期误差试验:
                    _DgnWcChannelNo = Cus_DgnWcChannelNo.需量脉冲;
                    break;
                default:
                    _DgnWcChannelNo = Cus_DgnWcChannelNo.电能误差;
                    break;
            }

            _CheckType = GetCheckType(elementType);
            _MeterWcChannelNo = GetMeterWcChannelNo(pulse);
            CL188L_RequestSelectPulseChannelAndCheckTypePacket rc = new CL188L_RequestSelectPulseChannelAndCheckTypePacket();
            CL188L_RequestSelectPulseChannelAndCheckTypeReplayPacket recv = new CL188L_RequestSelectPulseChannelAndCheckTypeReplayPacket();
            for (int j = 0; j < m_arrErrorPort.Length; j++)
            {
                if (getErrorStates(j))
                {
                    rc.Pos = 0;
                    rc.ChannelNo = j;
                    rc.ChannelNum = m_arrErrorPort.Length;
                    rc.SetPara(bSelectBw, _MeterWcChannelNo, _PulseType, _GyGyType, _DgnWcChannelNo, _CheckType);

                    if (!SendPacketWithRetry(m_arrErrorPort[j], rc, recv, true))
                    {
                        //return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 设置188E通道
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="pulse"></param>
        /// <returns></returns>
        private bool Set188EPulseChannel(VerificationElementType elementType, Pulse pulse)
        {
            //

            switch (elementType)
            {
                case VerificationElementType.日计时误差试验:
                case VerificationElementType.时段投切:
                    //Cus_CheckType
                    _DgnWcChannelNo = Cus_DgnWcChannelNo.日计时脉冲;
                    break;
                case VerificationElementType.需量周期误差试验:
                    _DgnWcChannelNo = Cus_DgnWcChannelNo.需量脉冲;
                    break;
                default:
                    _DgnWcChannelNo = Cus_DgnWcChannelNo.电能误差;
                    break;
            }
            _CheckType = GetCheckType(elementType);
            
            CL188_RequestSelectPulseChannelPacket rc = new CL188_RequestSelectPulseChannelPacket();
            CL188ERecvPacket recv = new CL188ERecvPacket();

            ////启动
            //CL188_RequestStartStopPacket rc48 = new CL188_RequestStartStopPacket();
            //rc48.Pos = 0xFF;
            //rc48.ControlTypes = CL188_RequestStartStopPacket.ControlType.启动当前功能;
            //if (!SendPacketWithRetry(m_arrErrorPort[0], rc48, recv))
            //{
            //    //return false;
            //}
            //System.Threading.Thread.Sleep(120);

            //38
            CL188_RequestSetDuiSheBiaoPacket rcsb = new CL188_RequestSetDuiSheBiaoPacket();
            rcsb.SetPara(false);
            if (!SendPacketWithRetry(m_arrErrorPort[0], rcsb, recv, true))
            {
                //return false;
            }
            System.Threading.Thread.Sleep(120);
            int iCheckType = (int)pulse;
            if (GlobalUnit.isPushBox == 0) //脉冲盒
            {
                if (iCheckType <= 2)
                { iCheckType = 1; }
                if (iCheckType == 3)
                { iCheckType = 2; }
                if (iCheckType == 4)
                { iCheckType = 2; }
            }
            //else
            //{
            //    if (iCheckType <= 2)
            //    { iCheckType = 1; }
            //    if (iCheckType == 3)
            //    { iCheckType = 3; }
            //    if (iCheckType == 4)
            //    { iCheckType = 4; }
            //}


            
            if (_CheckType == Cus_CheckType.日计时误差)
            {
                CL188_RequestSetStdDataPacket rc55 = new CL188_RequestSetStdDataPacket();
                SendPacketWithRetry(m_arrErrorPort[0], rc55, recv, true);

                System.Threading.Thread.Sleep(120);
                CL188_RequestSetStdMeterConstPacket rcl188e = new CL188_RequestSetStdMeterConstPacket();

                rcl188e.SetPara(6000000, 1);//先固定常数
                if (!SendPacketWithRetry(m_arrErrorPort[0], rcl188e, recv, true))
                {
                }
                //Set188EConst(400);
                System.Threading.Thread.Sleep(120);
                CL188_RequestStartStopPacket rc49 = new CL188_RequestStartStopPacket();
                rc49.ControlTypes = CL188_RequestStartStopPacket.ControlType.停止当前功能;
                rc49.Pos = 0xFF;
                if (!SendPacketWithRetry(m_arrErrorPort[0], rc49, recv, true))
                {
                    //return false;
                }
                iCheckType = 6;
                System.Threading.Thread.Sleep(120);
            }


            //误差通道
            _MeterWcChannelNo = GetMeterWcChannelNo(pulse);


            int o = 0;
            for (int j = 0; j < m_arrErrorPort.Length; j++)
            {
                if (getErrorStates(j))
                {
                    for (o = 0; o < _meterPositions.Length; o++)
                    {
                        
                        //              表位号                共阳 0共阴 1         脉冲（光电头1、脉冲盒0）   //脉冲通道
                        rc.SetPara(Convert.ToByte(o + 1), Convert.ToByte(1), Convert.ToByte(_PulseType), Convert.ToByte(iCheckType));

                        if (!SendPacketWithRetry(m_arrErrorPort[j], rc, recv, true))
                        {
                            //return false;
                        }
                        System.Threading.Thread.Sleep(80);
                    }

                    //for (o = 0; o < _meterPositions.Length; o++)
                    //{
                    //    //              表位号                共阳 0共阴 1         脉冲（光电头1、脉冲盒0）   //脉冲通道
                    //    rc.SetPara(Convert.ToByte(o + 1), Convert.ToByte(1), Convert.ToByte(_PulseType), Convert.ToByte(iCheckType));

                    //    if (!SendPacketWithRetry(m_arrErrorPort[j], rc, recv))
                    //    {
                    //        //return false;
                    //    }
                    //    System.Threading.Thread.Sleep(120);
                    //}
                }
            }

            CL188_RequestSetTaskTypePacket rc188e = new CL188_RequestSetTaskTypePacket();
            CL188ERecvPacket recv188e = new CL188ERecvPacket();

            CL188_RequestSetTaskTypePacket.TaskType taskType = CL188_RequestSetTaskTypePacket.TaskType.电能误差;
            switch (_CheckType)
            {
                case Cus_CheckType.电能误差:
                    taskType = CL188_RequestSetTaskTypePacket.TaskType.电能误差;
                    break;
                case Cus_CheckType.日计时误差:
                    taskType = CL188_RequestSetTaskTypePacket.TaskType.时钟日误差;
                    break;
                default:
                    break;

            }
            rc188e.SetPara(0xFF, taskType);
            System.Threading.Thread.Sleep(100);
            if (!SendPacketWithRetry(m_arrErrorPort[0], rc188e, recv188e, true))
            {
                //return false;
            }
            

            return true;
        }

        /// <summary>
        /// 设置188G通道
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="pulse"></param>
        /// <returns></returns>
        private bool Set188GPulseChannel(VerificationElementType elementType, Pulse pulse)
        {
            //
            return true;
        }

        /// <summary>
        /// 设置188H通道
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="pulse"></param>
        /// <returns></returns>
        private bool Set188HPulseChannel(VerificationElementType elementType, Pulse pulse)
        {
            //
            return true;
        }

        /// <summary>
        /// 设置校验圈数
        /// </summary>
        /// <param name="circle">校验圈数</param>
        /// <returns></returns>
        public bool SetCircle(int circle)
        {
            _intMeterCircle = circle;
            return true;
        }

        /// <summary>
        /// 设置日计时时间
        /// </summary>
        /// <param name="second">时间，单位（秒）</param>
        /// <returns></returns>
        public bool SetClockErrorTime(int second, VerificationElementType elementType)
        {
            if (GlobalUnit.DriverTypes[3] == '0')
            {
               return  Set188LClockErrorTime(second, elementType);
            }
            else if (GlobalUnit.DriverTypes[3] == '1')
            {
                return Set188EClockErrorTime(second,elementType);
            }
            else if (GlobalUnit.DriverTypes[3] == '2')
            {
            }
            else if (GlobalUnit.DriverTypes[3] == '3')
            {
            }
            else
            {
            }

            return true;
        }
        /// <summary>
        /// 设置误差板188L时间
        /// </summary>
        /// <param name="second">秒</param>
        /// <param name="elementType"></param>
        /// <returns></returns>
        private bool Set188LClockErrorTime(int second, VerificationElementType elementType)
        {
            int pulseTime = elementType == VerificationElementType.日计时误差试验 ? second : (second * 60 * 1000);
            CL188L_RequestSetTimePluseNumAndXLTimePacket cl188l = new CL188L_RequestSetTimePluseNumAndXLTimePacket();
            CL188L_RequestSetTimePluseNumAndXLTimeReplayPacket cl188lrecv = new CL188L_RequestSetTimePluseNumAndXLTimeReplayPacket();
            for (int j = 0; j < m_arrErrorPort.Length; j++)
            {
                if (getErrorStates(j))
                {
                    cl188l.Pos = 0;
                    cl188l.ChannelNo = j;
                    cl188l.ChannelNum = m_arrErrorPort.Length;

                    cl188l.SetPara(bSelectBw, 500000, pulseTime, second);
                    if (!SendPacketWithRetry(m_arrErrorPort[j], cl188l, cl188lrecv, true))
                    {
                        //return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 设置脉冲个数
        /// </summary>
        /// <param name="second"></param>
        /// <param name="elementType"></param>
        /// <returns></returns>
        private bool Set188EClockErrorTime(int second, VerificationElementType elementType)
        {  
            //45指令
            CL188_RequestSetclockfreqPacket clocrc = new CL188_RequestSetclockfreqPacket();
            CL188ERecvPacket recv = new CL188ERecvPacket();
            clocrc.SetPara(50000000);
            if (!SendPacketWithRetry(m_arrErrorPort[0], clocrc, recv, true))
            {
                //return false;
            }
            //
            System.Threading.Thread.Sleep(100);
            CL188_RequestSetDayTimePacket dayTimerc = new CL188_RequestSetDayTimePacket();
            //44指令
            for (int o = 0; o < _meterPositions.Length; o++)
            {
                //                 表位通道             被检脉冲个数
                dayTimerc.SetPara(Convert.ToByte(o + 1), second);
                if (!SendPacketWithRetry(m_arrErrorPort[0], dayTimerc, recv, false))
                {
                    //return false;
                }
                System.Threading.Thread.Sleep(10);
            }

            CL188_RequestSetTaskTypePacket rc188e = new CL188_RequestSetTaskTypePacket();
            CL188ERecvPacket recv188e = new CL188ERecvPacket();
            //(int)_CheckType;
            //int.Parse(_CheckType.ToString());
            CL188_RequestSetTaskTypePacket.TaskType taskType = CL188_RequestSetTaskTypePacket.TaskType.电能误差;
            switch (_CheckType)
            {
                case Cus_CheckType.电能误差:
                    taskType = CL188_RequestSetTaskTypePacket.TaskType.电能误差;
                    break;
                case Cus_CheckType.日计时误差:
                    taskType = CL188_RequestSetTaskTypePacket.TaskType.时钟日误差;
                    break;
                default:
                    break;

            }
            rc188e.SetPara(0xFF, taskType);

            if (!SendPacketWithRetry(m_arrErrorPort[0], rc188e, recv188e, true))
            {
                //return false;
            }

            return true;
        }
        /// <summary>
        /// 设置电能表常数
        /// </summary>
        /// <param name="meterConst"></param>
        /// <returns></returns>
        public bool SetMeterConst(int meterConst)
        {

            if (GlobalUnit.DriverTypes[3] == '0')
            {
                return true;
                //return Set188LConst(meterConst,220,);
            }
            else if (GlobalUnit.DriverTypes[3] == '1')
            {
                return true;
                //return Set188LConst(meterConst);
                //return Set188EConst(meterConst);
            }
            else if (GlobalUnit.DriverTypes[3] == '2')
            {
                return CL188G_INIT();
            }
            else
            {
                return CL188G_INIT();
            }




        }
        /// <summary>
        /// 设置188L脉冲常数
        /// </summary>
        /// <param name="meterConst"></param>
        /// <returns></returns>
        private bool Set188LConst(int meterConst,float voltage, float current)
        {
            int _lStdMeterConst = 4000000;
            _lStdMeterConst=SetMeterBCS3115(voltage,current);
            CL188L_RequestSetPulseParaPacket ccpc = new CL188L_RequestSetPulseParaPacket();
            CL188L_RequestSetPulseParaReplayPacket ccpcrecv = new CL188L_RequestSetPulseParaReplayPacket();
            for (int j = 0; j < m_arrErrorPort.Length; j++)
            {
                if (getErrorStates(j))
                {
                    long lngtmpStdPulsePl = GetStdPulsePl(_lStdMeterConst, GlobalUnit.g_StrandMeterP[0] > 0 ? GlobalUnit.g_StrandMeterP[0] : 100);
                    ccpc.Pos = 0;
                    ccpc.ChannelNo = j;
                    ccpc.ChannelNum = m_arrErrorPort.Length;
                    ccpc.SetPara(bSelectBw, _lStdMeterConst, (int)lngtmpStdPulsePl, 1, meterConst, _intMeterCircle, 1);
                    ccpc.BwStatus = bSelectBw;
                    if (!SendPacketWithRetry(m_arrErrorPort[j], ccpc, ccpcrecv, true))
                    {
                        //return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 设置脉冲常数
        /// </summary>
        /// <param name="meterConst"></param>
        /// <returns></returns>
        public bool SetMeterConst(int meterConst, float voltage, float current)
        {
            if (GlobalUnit.DriverTypes[3] == '0')
            {
                return Set188LConst(meterConst,voltage,current);
            }
            else if (GlobalUnit.DriverTypes[3] == '1')
            {
                return Set188EConst(meterConst,voltage,current);
            }
            else if (GlobalUnit.DriverTypes[3] == '2')
            {
                return CL188G_INIT();
            }
            else
            {
                return CL188G_INIT();
            }

        }


        /// 判断是否直接查标准表常数  //add 周江林 添加 自动标准表常数
        /// </summary>
        /// <param name="lngConst">标准表常数</param>
        /// <param name="sng_U">电压</param>
        /// <param name="sng_I">电流</param>
        /// <returns></returns>
        public int SetMeterBCS3115(float sng_U, float sng_I)
        {
            int g_StrandMeterConst = 4000000;
            if (sng_U <= 70)
            {
                if (sng_I <= 1 * 1.1)
                {
                    g_StrandMeterConst = 2000000000;
                }
                else if (sng_I > 1 * 1.1 && sng_I <= 2 * 1.1)
                {
                    g_StrandMeterConst = 1600000000;
                }
                else if (sng_I > 2 * 1.1 && sng_I <= 5 * 1.1)
                {
                    g_StrandMeterConst = 640000000;
                }
                else if (sng_I > 5 * 1.1 && sng_I <= 10 * 1.1)
                {
                    g_StrandMeterConst = 320000000;
                }
                else if (sng_I > 10 * 1.1 && sng_I <= 20 * 1.1)
                {
                    g_StrandMeterConst = 160000000;
                }
                else if (sng_I > 20 * 1.1 && sng_I <= 50 * 1.1)
                {
                    g_StrandMeterConst = 64000000;
                }
                else if (sng_I >= 50 * 1.1)
                {
                    g_StrandMeterConst = 32000000;
                }

            }
            else if (sng_U > 74 && sng_U <= 110)
            {
                if (sng_I <= 0.5 * 1.1)
                {
                    g_StrandMeterConst = 2000000000;
                }
                else if (sng_I > 0.5 * 1.1 && sng_I <= 1 * 1.1)
                {
                    g_StrandMeterConst = 1600000000;
                }
                else if (sng_I > 1 * 1.1 && sng_I <= 2 * 1.1)
                {
                    g_StrandMeterConst = 800000000;
                }
                else if (sng_I > 2 * 1.1 && sng_I <= 5 * 1.1)
                {
                    g_StrandMeterConst = 320000000;
                }
                else if (sng_I > 5 * 1.1 && sng_I <= 10 * 1.1)
                {
                    g_StrandMeterConst = 160000000;
                }
                else if (sng_I > 10 * 1.1 && sng_I <= 20 * 1.1)
                {
                    g_StrandMeterConst = 80000000;
                }
                else if (sng_I > 20 * 1.1 && sng_I <= 50 * 1.1)
                {
                    g_StrandMeterConst = 32000000;
                }
                else if (sng_I >= 50 * 1.1)
                {
                    g_StrandMeterConst = 16000000;
                }
            }
            else if (sng_U > 130 && sng_U <= 250)
            {
                if (sng_I <= 0.2 * 1.1)
                {
                    g_StrandMeterConst = 1000000000;
                }
                else if (sng_I > 0.2 * 1.1 && sng_I <= 0.5 * 1.1)
                {
                    g_StrandMeterConst = 800000000;
                }
                else if (sng_I > 0.5 * 1.1 && sng_I <= 1 * 1.1)
                {
                    g_StrandMeterConst = 400000000;
                }
                else if (sng_I > 1 * 1.1 && sng_I <= 2 * 1.1)
                {
                    g_StrandMeterConst = 200000000;
                }
                else if (sng_I > 2 * 1.1 && sng_I <= 5 * 1.1)
                {
                    g_StrandMeterConst = 80000000;
                }
                else if (sng_I > 5 * 1.1 && sng_I <= 10 * 1.1)
                {
                    g_StrandMeterConst = 40000000;
                }
                else if (sng_I > 10 * 1.1 && sng_I <= 20 * 1.1)
                {
                    g_StrandMeterConst = 20000000;
                }
                else if (sng_I > 20 * 1.1 && sng_I <= 50 * 1.1)
                {
                    g_StrandMeterConst = 8000000;
                }
                else if (sng_I >= 50 * 1.1)
                {
                    //2014-03-28按周江林要求由8000000改为4000000
                    //2014-04-02按周江林要求由4000000改为480000
                    g_StrandMeterConst = 480000;
                }
                else
                { g_StrandMeterConst = 4000000; }

            }
            else if (sng_U > 480)
            {
                if (sng_I <= 0.2 * 1.1)
                {
                    g_StrandMeterConst = 2000000000;
                }
                else if (sng_I > 0.2 * 1.1 && sng_I <= 0.5 * 1.1)
                {
                    g_StrandMeterConst = 800000000;
                }
                else if (sng_I > 0.5 * 1.1 && sng_I <= 1 * 1.1)
                {
                    g_StrandMeterConst = 400000000;
                }
                else if (sng_I > 1 * 1.1 && sng_I <= 2 * 1.1)
                {
                    g_StrandMeterConst = 200000000;
                }
                else if (sng_I > 2 * 1.1 && sng_I <= 5 * 1.1)
                {
                    g_StrandMeterConst = 80000000;
                }
                else if (sng_I > 5 * 1.1 && sng_I <= 10 * 1.1)
                {
                    g_StrandMeterConst = 40000000;
                }
                else if (sng_I > 10 * 1.1 && sng_I <= 20 * 1.1)
                {
                    g_StrandMeterConst = 20000000;
                }
                else if (sng_I > 20 * 1.1 && sng_I <= 50 * 1.1)
                {
                    g_StrandMeterConst = 8000000;
                }
                else if (sng_I >= 50 * 1.1)
                {
                    g_StrandMeterConst = 4000000;
                }
                else
                { g_StrandMeterConst = 4000000; }
                //g_StrandMeterConst = ;
            }
            return g_StrandMeterConst;
        }

        private bool Set188EConst(int meterConst, float voltage, float current)
        {
            CL188_RequestSetMeterConstAndVerifyCirclePacket rc = new CL188_RequestSetMeterConstAndVerifyCirclePacket();
            CL188_RequestReadStatusReplyPacket recv = new CL188_RequestReadStatusReplyPacket();

            //System.Threading.Thread.Sleep(100);
            CL188_RequestSetStdMeterConstPacket rcl188e = new CL188_RequestSetStdMeterConstPacket();

            if (GlobalUnit.DriverTypes[0] == '0')
            {
                int c=SetMeterBCS3115(voltage,current);
                int s = meterConst;
                rcl188e.SetPara(c, 1);//先固定常数
            }
            else
            {
                int ConstUnit = 0;
                if (voltage <= 60)
                {
                    if (current <= 1)
                    {
                        ConstUnit = (int)(1.2 * 100000000);
                    }
                    else if (current <= 5)
                    {
                        ConstUnit = (int)(2.4 * 10000000);
                    }
                    else if (current <= 10)
                    {
                        ConstUnit = (int)(1.2 * 10000000);
                    }
                    else if (current <= 50)
                    {
                        ConstUnit = (int)(2.4 * 1000000);
                    }
                    else if (current <= 100)
                    {
                        ConstUnit = (int)(1.2 * 1000000);
                    }
                }
                else if (voltage <= 100)
                {
                    if (current <= 1)
                    {
                        ConstUnit = (int)(6 * 10000000);
                    }
                    else if (current <= 5)
                    {
                        ConstUnit = (int)(1.2 * 10000000);
                    }
                    else if (current <= 10)
                    {
                        ConstUnit = (int)(6 * 1000000);
                    }
                    else if (current <= 50)
                    {
                        ConstUnit = (int)(1.2 * 1000000);
                    }
                    else if (current <= 100)
                    {
                        ConstUnit = (int)(6 * 100000);
                    }
                }
                else if (voltage <= 220)
                {
                    if (current <= 1)
                    {
                        ConstUnit = (int)(3 * 10000000);
                    }
                    else if (current <= 5)
                    {
                        ConstUnit = (int)(6 * 1000000);
                    }
                    else if (current <= 10)
                    {
                        ConstUnit = (int)(3 * 1000000);
                    }
                    else if (current <= 50)
                    {
                        ConstUnit = (int)(6 * 100000);
                    }
                    else if (current <= 100)
                    {
                        ConstUnit = (int)(3 * 100000);
                    }
                }
                else if (voltage <= 380)
                {
                    if (current <= 1)
                    {
                        ConstUnit = (int)(1.5 * 10000000);
                    }
                    else if (current <= 5)
                    {
                        ConstUnit = (int)(3 * 1000000);
                    }
                    else if (current <= 10)
                    {
                        ConstUnit = (int)(1.5 * 1000000);
                    }
                    else if (current <= 50)
                    {
                        ConstUnit = (int)(3 * 100000);
                    }
                    else if (current <= 100)
                    {
                        ConstUnit = (int)(1.5 * 100000);
                    }
                }
                else if (voltage <= 1000)
                {
                    if (current <= 1)
                    {
                        ConstUnit = (int)(6 * 1000000);
                    }
                    else if (current <= 5)
                    {
                        ConstUnit = (int)(1.2 * 1000000);
                    }
                    else if (current <= 10)
                    {
                        ConstUnit = (int)(6 * 100000);
                    }
                    else if (current <= 50)
                    {
                        ConstUnit = (int)(1.2 * 100000);
                    }
                    else if (current <= 100)
                    {
                        ConstUnit = (int)(6 * 10000);
                    }
                }
                rcl188e.SetPara(ConstUnit, 1);//先固定常数
            }
            System.Threading.Thread.Sleep(100);
            if (!SendPacketWithRetry(m_arrErrorPort[0], rcl188e, recv, true))
            {
            }
            System.Threading.Thread.Sleep(100);
            CL188_RequestSetStdDataPacket rcset188e = new CL188_RequestSetStdDataPacket();
            if (!SendPacketWithRetry(m_arrErrorPort[0], rcset188e, recv, false))
            {
                //return false;
            }
            System.Threading.Thread.Sleep(200);
            int[] metercons = { meterConst, meterConst, meterConst };//被检定表常数
            int[] metercircl = { _intMeterCircle, _intMeterCircle, _intMeterCircle };//圈数
            for (int j = 0; j < m_arrErrorPort.Length; j++)
            {
                if (getErrorStates(j))
                {
                    //rc.SetPara(meterConst, _intMeterCircle);
                    for (int o = 0; o < 2; o++)
                    {
                        rc.SetPara(Convert.ToByte(o), metercons, metercircl);
                        if (!SendPacketWithRetry(m_arrErrorPort[j], rc, recv, true))
                        {
                        }
                        System.Threading.Thread.Sleep(100);
                        //rc.SetPara(Convert.ToByte(o), metercons, metercircl);
                        //if (!SendPacketWithRetry(m_arrErrorPort[j], rc, recv, true))
                        //{
                        //}
                        //System.Threading.Thread.Sleep(100);
                    }
                    for (int o = 0; o < 4; o++)
                    {
                        rc.SetPara(Convert.ToByte(o), metercons, metercircl);
                        if (!SendPacketWithRetry(m_arrErrorPort[j], rc, recv, true))
                        {
                        }
                        //rc.SetPara(Convert.ToByte(o), metercons, metercircl);
                        //if (!SendPacketWithRetry(m_arrErrorPort[j], rc, recv, true))
                        //{
                        //}
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }

            return true;
        }
        /// <summary>
        /// 设置188G脉冲常数
        /// </summary>
        /// <param name="meterConst"></param>
        /// <returns></returns>
        private bool Set188GConst(int meterConst)
        {
            return true;
        }

        /// <summary>
        /// 设置188H脉冲常数
        /// </summary>
        /// <param name="meterConst"></param>
        /// <returns></returns>
        private bool Set188HConst(int meterConst)
        {
            return true;
        }

        /// <summary>
        /// 计算标准脉冲频率
        /// </summary>
        /// <param name="StdPulse"></param>
        /// <param name="_CurP"></param>
        /// <returns></returns>
        private long GetStdPulsePl(long StdPulse, float _CurP)
        {//标准脉冲常数/（3600*1000/P）
            long lngStdPl;
            lngStdPl = (StdPulse / (3600 * 1000)) * (long)_CurP;
            return lngStdPl;
        }
        /// <summary>
        /// 发送报文
        /// </summary>
        /// <param name="port"></param>
        /// <param name="sp"></param>
        /// <param name="rp"></param>
        /// <returns></returns>
        private bool SendPacketWithRetry(int port, SendPacket sp, RecvPacket rp,bool isReturn)
        {
            lock (ErrorPlateObject)
            {
                for (int i = 0; i < RETRYTIEMS; i++)
                {
                    if (this.SendData(port, sp, rp) == true || !isReturn)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 开始执行试验
        /// </summary>
        /// <param name="elementType">检定类型</param>
        /// <returns></returns>
        public bool StartVerification(VerificationElementType elementType)
        {
            if (GlobalUnit.DriverTypes[3] == '0')
            {
                return Start188LVerification(elementType);
            }
            else if (GlobalUnit.DriverTypes[3] == '1')
            {
                return Start188EVerification(elementType);
            }
            else if (GlobalUnit.DriverTypes[3] == '2')
            {
            }
            else if (GlobalUnit.DriverTypes[4] == '3')
            {
            }
            else
            {
            }

            return true;
        }
        /// <summary>
        /// 设定启动误差板
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        private bool Start188LVerification(VerificationElementType elementType)
        {
            if (elementType != VerificationElementType.耐压试验)
            {
                RequestSetWishStandStatus(0);//复位误差板到非耐压状态
            }
            //启动误差板
            CL188L_RequestStartPCFunctionPacket rc = new CL188L_RequestStartPCFunctionPacket();
            CL188L_RequestStartPCFunctionReplayPacket recv = new CL188L_RequestStartPCFunctionReplayPacket();
            for (int i = 0; i < m_arrErrorPort.Length; i++)
            {
                if (getErrorStates(i))
                {
                    rc.Pos = 0;
                    rc.ChannelNo = i;
                    rc.ChannelNum = m_arrErrorPort.Length;
                    rc.SetPara(bSelectBw, GetCheckType(elementType));

                    if (!SendPacketWithRetry(m_arrErrorPort[i], rc, recv, true))
                    {
                        //return false;
                    }
                }
            }
            return true;
        }

        private bool Start188EVerification(VerificationElementType elementType)
        {
            /*
            CL188_RequestStartStopPacket rc = new CL188_RequestStartStopPacket();
            rc.ControlTypes = CL188_RequestStartStopPacket.ControlType.启动当前功能;

            CL188_RequestReadStatusReplyPacket recv = new CL188_RequestReadStatusReplyPacket();
            for (int j = 0; j < m_arrErrorPort.Length; j++)
            {
                if (getErrorStates(j))
                {
                    //_CheckType
                    rc.Pos = 0xFF;//Convert.ToByte(j);
                    if (!SendPacketWithRetry(m_arrErrorPort[j], rc, recv))
                    {
                        //return false;
                    }
                }
            }
             * */
            return true;
        }

        /// <summary>
        /// 设置脉冲方式
        /// </summary>
        /// <param name="pulse">脉冲类型</param>
        /// <returns></returns>
        public bool SetPulseType(Pulse pulse)
        {
            _MeterWcChannelNo = GetMeterWcChannelNo(pulse);
            return true;
        }
        /// <summary>
        /// 获取表位误差数据
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <returns></returns>
        public ErrorData ReadErrorData(int meterIndex, int sampleIndex)
        {
             ErrorData errData = new ErrorData();
            if (GlobalUnit.DriverTypes[3] == '0')
            {
                return Read188LErrorData(meterIndex, sampleIndex);
            }
            else if (GlobalUnit.DriverTypes[3] == '1')
            {
                return Read188EErrorData(meterIndex, sampleIndex);    
            }
            else if (GlobalUnit.DriverTypes[3] == '2')
            {
                return Read188GErrorData(meterIndex, sampleIndex); 
            }
            else if (GlobalUnit.DriverTypes[4] == '3')
            {
                return Read188HErrorData(meterIndex, sampleIndex); 
            }
            else
            {
            }
            return errData;
        }
        /// <summary>
        /// 188L误差板读数据
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="sampleIndex"></param>
        /// <returns></returns>
        private ErrorData Read188LErrorData(int meterIndex, int sampleIndex)
        {
            ErrorData errData = new ErrorData();

            CL188L_RequestReadBwWcAndStatusPacket rc = new CL188L_RequestReadBwWcAndStatusPacket((Cus_WuchaType)GlobalUnit.g_CurTestType);
            CL188L_RequestReadBwWcAndStatusReplyPacket rcback = new CL188L_RequestReadBwWcAndStatusReplyPacket();//m_curTaskType, this.currentWorkFlow);
            rc.Pos = meterIndex;

            int line = (meterIndex - 1) / getOneLineLoads(_meterPositions.Length);
            rc.ChannelNo = line;
            rc.ChannelNum = m_arrErrorPort.Length;
            rc.BwStatus = bSelectBw;

            if (SendPacketWithRetry(m_arrErrorPort[line], rc, rcback, true))
            {
                errData.ErrorValue = rcback.wcData;
                errData.SampleIndex = rcback.wcNum;
                errData.MeterNo = meterIndex;
            }

            return errData;

        }
        /// <summary>
        /// 188E读取误差
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="sampleIndex"></param>
        /// <returns></returns>
        private  ErrorData Read188EErrorData(int meterIndex, int sampleIndex)
        {

            ErrorData errData = new ErrorData();


            CL188_RequestReadVerifyDataPacket rc = new CL188_RequestReadVerifyDataPacket();
            CL188_RequestReadVerifyDataReplayPacket rcback = new CL188_RequestReadVerifyDataReplayPacket(enmTaskType.电能误差, WorkFlow.基本误差);

            rc.Pos = Convert.ToByte(meterIndex);

            //int line = meterIndex - 1;
            int line = (meterIndex - 1) / getOneLineLoads(_meterPositions.Length);
            if (SendPacketWithRetry(m_arrErrorPort[line], rc, rcback, true))
            {
                errData.ErrorValue = rcback.Data.ToString("f4");
                errData.SampleIndex = rcback.WcTimes;
                errData.MeterNo = meterIndex;
            }

            return errData;
        }
        /// <summary>
        /// 188G读取误差数据
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="sampleIndex"></param>
        /// <returns></returns>
        private ErrorData Read188GErrorData(int meterIndex, int sampleIndex)
        {
            ErrorData errData = new ErrorData();
            //先空着、之后再加上需要代码
            return errData;
        }
        /// <summary>
        /// 188H读取误差数据
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="sampleIndex"></param>
        /// <returns></returns>
        private ErrorData Read188HErrorData(int meterIndex, int sampleIndex)
        {
            ErrorData errData = new ErrorData();
            //先空着、之后再加上需要代码
            return errData;
        }

        /// <summary>
        /// 读取启动、潜动脉冲
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <returns></returns>
        public PulseValue ReadPulse(int meterIndex)
        {
            PulseValue pulseValue = new PulseValue();
            CL188L_RequestReadBwWcAndStatusPacket rc = new CL188L_RequestReadBwWcAndStatusPacket((Cus_WuchaType)GlobalUnit.g_CurTestType);
            CL188L_RequestReadBwWcAndStatusReplyPacket rcback = new CL188L_RequestReadBwWcAndStatusReplyPacket();//m_curTaskType, this.currentWorkFlow);
            rc.Pos = meterIndex;

            int line = (meterIndex - 1) / getOneLineLoads(_meterPositions.Length);
            rc.ChannelNo = line;
            rc.ChannelNum = m_arrErrorPort.Length;
            rc.BwStatus = bSelectBw;

            if (SendPacketWithRetry(m_arrErrorPort[line], rc, rcback, true))
            {
                pulseValue.Count = Convert.ToInt16(rcback.wcData);
                pulseValue.MeterIndex = meterIndex;
            }
            return pulseValue;
        }

        /// <summary>
        /// 获取多个表位误差数据
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <returns></returns>
        public Dictionary<int, ErrorData> ReadErrorData(int[] meterIndex, int sampleIndex)
        {
            Dictionary<int, ErrorData> errData = new Dictionary<int, ErrorData>();

            stError[] tagError = new stError[meterIndex.Length];
            ReadWcbManager readWcb = new ReadWcbManager();
            int wcbPerCount = getOneLineLoads(_meterPositions.Length);
            readWcb.WcbChannelCount = m_arrErrorPort.Length;
            readWcb.WcbPerChannelBwCount = wcbPerCount;
            readWcb.BwStatus = bSelectBw;
            readWcb.portNum = m_arrErrorPort;
            readWcb.m_curTaskType = (enmTaskType)GlobalUnit.g_CurTestType;
            readWcb.Start();
            readWcb.WaitAllThreaWorkDone();
            tagError = readWcb.tagError;

            return errData;
        }
        #endregion

        #region ITemperature 成员

        /// <summary>
        /// 获取表位温度
        /// </summary>
        /// <param name="meterIndex">表位数组</param>
        /// <param name="temperature">温度</param>
        public void GetTemperature(int[] meterIndex, out float[] temperature)
        {
            temperature = new float[meterIndex.Length];

            CL188L_RequestReadBwTemperaturePacket cl188 = new CL188L_RequestReadBwTemperaturePacket();
            CL188L_RequestReadBwTemperatureReplyPacket cl188rec = new CL188L_RequestReadBwTemperatureReplyPacket();

            for (int i = 0; i < m_arrErrorPort.Length; i++)
            {
                cl188.Pos = 0;
                cl188.ChannelNo = i;
                cl188.ChannelNum = m_arrErrorPort.Length;
                cl188.BwStatus = bSelectBw;

                if (!SendPacketWithRetry(m_arrErrorPort[i], cl188, cl188rec, true))
                {

                }
            }
        }

        #endregion

        #region IConnect 成员

        public void Connected(int meterCount)
        {
            this.Open();
        }

        public void Connected(VerificationEquipment.Commons.MeterPosition[] meterPositions)
        {
            _meterPositions = meterPositions;
            _meter = meterPositions.First(item => item.Meter != null && item.IsVerify).Meter;
            bSelectBw = new bool[_meterPositions.Length];
            bDefaultSelectBw = new bool[_meterPositions.Length];
            for (int i = 0; i < bSelectBw.Length; i++)
            {
                bSelectBw[i] = _meterPositions[i].IsVerify;
                bDefaultSelectBw[i] = true;
            }
        }

        public void Closed()
        {
            this.Close();
        }

        #endregion

        #region IControPressMotor 成员
        /// 控制设备进行压接操作
        /// <summary>
        /// 控制设备进行压接操作
        /// </summary>
        /// <param name="isPress">true表示压接，false表示松开压接</param>
        /// <param name="results">压接结论</param>
        /// <param name="resultDescriptions">压接结论描述</param>
        public void EquipmentPress(bool isPress, out bool[] results, out string[] resultDescriptions)
        {
            results = new bool[_meterPositions.Length];
            resultDescriptions = new string[_meterPositions.Length];
            int intPress = isPress ? 1 : 0;
            string strPress = isPress ? "压接" : "松开";
            bool bResult = true;
            //压接电机
            CL188L_RequestSetElectromotorPacket cl188 = new CL188L_RequestSetElectromotorPacket();
            CL188L_RequestSetElectromotorReplayPacket cl188rec = new CL188L_RequestSetElectromotorReplayPacket();
            for (int j = 0; j < m_arrErrorPort.Length; j++)
            {
                cl188.Pos = 0;
                cl188.ChannelNo = j;
                cl188.ChannelNum = m_arrErrorPort.Length;
                cl188.SetPara(bDefaultSelectBw, intPress);
                if (!SendPacketWithRetry(m_arrErrorPort[j], cl188, cl188rec, true))
                {
                    bResult = false;
                    break;
                }
            }
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = bResult;
                if (bResult)
                {
                    resultDescriptions[i] = string.Format("{0}成功", strPress);
                }
                else
                {
                    resultDescriptions[i] = string.Format("{0}失败", strPress);
                }
            }
            MeterPositionPressStatus[] meterPressStaus = new MeterPositionPressStatus[bDefaultSelectBw.Length];
            int[] meterBwcount = new int[bDefaultSelectBw.Length];
            for (int i = 0; i < bDefaultSelectBw.Length; i++)
            {
                if (bDefaultSelectBw[i])
                {
                    meterBwcount[i] = i + 1;
                }
            }
            bool reBool = true;
            int startTime = Environment.TickCount;
            while (reBool)
            {
                GetEquipmentPressStatus(meterBwcount, meterPressStaus);
                for (int i = 0; i < meterPressStaus.Length; i++)
                {
                    if (bSelectBw[i])
                    {
                        if (isPress)
                        {
                            if (meterPressStaus[i] == MeterPositionPressStatus.已压接)
                            {
                                reBool = false;
                            }
                            else
                            {
                                reBool = true;
                                break;
                            }
                        }
                        else
                        {
                            if (meterPressStaus[i] == MeterPositionPressStatus.未压接)
                            {
                                reBool = false;
                            }
                            else
                            {
                                reBool = true;
                                break;
                            }
                        }
                    }
                }
                if ((Environment.TickCount - startTime) / 1000 > 10)
                {
                    reBool = false;
                    break;
                }
            }
        }

        /// 单个表位压接、松开过程 
        /// <summary>
        /// 单个表位压接、松开过程 
        /// </summary>
        /// <param name="meterIndex">表位号，从1开始</param>
        /// <param name="isPress">true：压接；false：松开</param>
        /// <returns></returns>
        public bool EquipmentPress(int meterIndex, bool isPress)
        {
            bool bResult = true;
            int intPress = isPress ? 1 : 0;
            int iChannelNo = (meterIndex - 1) / getOneLineLoads(_meterPositions.Length);
            CL188L_RequestSetElectromotorPacket cl188 = new CL188L_RequestSetElectromotorPacket();
            cl188.Pos = 0;
            cl188.ChannelNo = iChannelNo;
            cl188.ChannelNum = m_arrErrorPort.Length;
            cl188.SetPara(bDefaultSelectBw, intPress);
            CL188L_RequestSetElectromotorReplayPacket cl188rec = new CL188L_RequestSetElectromotorReplayPacket();
            if (!SendPacketWithRetry(m_arrErrorPort[iChannelNo], cl188, cl188rec, true))
            {
                bResult = false;
            }
            return bResult;
        }
        #endregion

        #region IResistanceWcfk 成员

        /// <summary>
        /// 读取耐压结论
        /// </summary>
        /// <param name="meterIndex">表位数组</param>
        /// <param name="result">结论</param>
        /// <param name="resultDescription">结论描述</param>
        /// <returns></returns>
        public bool ReadResistanceResult(int[] meterIndex, out bool[] result, out string[] resultDescription)
        {
            result = new bool[_meterPositions.Length];
            resultDescription = new string[_meterPositions.Length];
            for (int i = 0; i < meterIndex.Length; i++)
            {
                ErrorData errorData = this.ReadErrorData(meterIndex[i], 1);
                if (errorData != null
                    && errorData.SampleIndex > 0)
                {
                    result[meterIndex[i] - 1] = false;
                }
                else
                {
                    result[meterIndex[i] - 1] = true;
                }
            }

            return true;
        }

        /// <summary>
        /// 设置漏电流限制
        /// </summary>
        /// <param name="resistanceI">5mA</param>
        /// <returns></returns>
        public bool SetResistanceIWcfkRangle(float resistanceI)
        {
            CL188L_RequestSetWishStandCurrentLimitPacket rc = new CL188L_RequestSetWishStandCurrentLimitPacket();
            CL188L_RequestSetWishStandCurrentLimitReplayPacket recv = new CL188L_RequestSetWishStandCurrentLimitReplayPacket();
            for (int i = 0; i < m_arrErrorPort.Length; i++)
            {
                if (getErrorStates(i))
                {
                    rc.Pos = 0;
                    rc.ChannelNo = i;
                    rc.ChannelNum = m_arrErrorPort.Length;
                    rc.SetPara(bSelectBw, resistanceI);

                    if (!SendPacketWithRetry(m_arrErrorPort[i], rc, recv, true))
                    {
                        //return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 设置误差板耐压状态功能
        /// </summary>
        /// <param name="WishStandStatustype"> 状态类型 0，复位状态；1，控制耐压继电器闭合状态；</param>
        /// <returns></returns>
        private bool RequestSetWishStandStatus(int WishStandStatustype)
        {

            CL188L_RequestSetWishStandStatusPacket rc = new CL188L_RequestSetWishStandStatusPacket();
            CL188L_RequestSetWishStandStatusReplayPacket recv = new CL188L_RequestSetWishStandStatusReplayPacket();
            for (int i = 0; i < m_arrErrorPort.Length; i++)
            {
                if (getErrorStates(i))
                {
                    rc.Pos = 0;
                    rc.ChannelNo = i;
                    rc.ChannelNum = m_arrErrorPort.Length;
                    rc.SetPara(bSelectBw, WishStandStatustype);

                    if (!SendPacketWithRetry(m_arrErrorPort[i], rc, recv, true))
                    {
                        //return false;
                    }
                }
            }
            return true;
        }
        #endregion

        #region IStatus 成员
        /// 获取表位状态
        /// <summary>
        /// 获取表位状态
        /// </summary>
        /// <param name="meterNo"></param>
        /// <param name="status"></param>
        public void GetCurrentMeterNoStatus(out int[] meterNo, out string[] status)
        {
            int bwCount = 60;
            if (m_arrErrorPort.Length == 2)
            { bwCount = 20; }


            meterNo = new int[bwCount];
            status = new string[bwCount];

            stError[] tagError = new stError[bwCount];
            ReadWcbManager readWcb = new ReadWcbManager();
            int wcbPerCount = getOneLineLoads(bwCount);
            readWcb.WcbChannelCount = m_arrErrorPort.Length;
            readWcb.WcbPerChannelBwCount = wcbPerCount;

            bool[] defaultPress = new bool[bwCount];
            for (int i = 0; i < bwCount; i++)
            {
                defaultPress[i] = true;
            }

            readWcb.BwStatus = defaultPress;
            readWcb.portNum = m_arrErrorPort;
            readWcb.m_curTaskType = (enmTaskType)GlobalUnit.g_CurTestType;
            readWcb.Start();
            readWcb.WaitAllThreaWorkDone();
            tagError = readWcb.tagError;

            for (int j = 0; j < tagError.Length; j++)
            {
                meterNo[j] = j + 1;
                if (tagError[j].statusTypeIsOn_HaveMeter)// 是否挂表
                {
                    status[j] = "01";
                }
                else
                {
                    status[j] = "00";
                }
            }
        }

        /// 获取检定装置状态
        /// <summary>
        /// 获取检定装置状态
        /// </summary>
        public string GetEquipmentStatus()
        {
            string strEquipmentStatus = "";

            return strEquipmentStatus;
        }
        #endregion

        /// <summary>
        /// 获取误差每路负载
        /// </summary>
        /// <returns></returns>
        private int getOneLineLoads(int meterPositionCount)
        {
            int nNum = meterPositionCount / m_arrErrorPort.Length;
            return nNum;
        }
        /// <summary>
        /// 获取电表误差板通道号
        /// </summary>
        /// <param name="pulse"></param>
        /// <returns></returns>
        private Cus_MeterWcChannelNo GetMeterWcChannelNo(Pulse pulse)
        {
            Cus_MeterWcChannelNo MeterWcChannelNo;
            switch (pulse)
            {
                case Pulse.正向有功:
                    MeterWcChannelNo = Cus_MeterWcChannelNo.正向有功;
                    break;
                case Pulse.正向无功:
                    MeterWcChannelNo = Cus_MeterWcChannelNo.正向无功;
                    break;
                case Pulse.反向有功:
                    MeterWcChannelNo = Cus_MeterWcChannelNo.反向有功;
                    break;
                case Pulse.反向无功:
                    MeterWcChannelNo = Cus_MeterWcChannelNo.反向无功;
                    break;
                default:
                    MeterWcChannelNo = Cus_MeterWcChannelNo.正向有功;
                    break;
            }
            return MeterWcChannelNo;
        }
        /// <summary>
        /// 获取检定类型
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        private Cus_CheckType GetCheckType(VerificationElementType elementType)
        {
            Cus_CheckType CheckType;
            switch (elementType)
            {
                case VerificationElementType.时段投切:
                    CheckType = Cus_CheckType.多功能脉冲计数试验;
                    GlobalUnit.g_CurTestType = 7;
                    break;
                case VerificationElementType.电能走字试验:
                case VerificationElementType.常数校核试验:
                case VerificationElementType.定数走字试验:
                case VerificationElementType.电表启动试验:
                case VerificationElementType.电表潜动试验:
                    CheckType = Cus_CheckType.走字计数;
                    GlobalUnit.g_CurTestType = 3;
                    break;
                case VerificationElementType.日计时误差试验:
                    CheckType = Cus_CheckType.日计时误差;
                    GlobalUnit.g_CurTestType = 2;
                    break;
                case VerificationElementType.需量周期误差试验:
                    CheckType = Cus_CheckType.需量误差;
                    GlobalUnit.g_CurTestType = 1;
                    break;
                case VerificationElementType.耐压试验:
                    CheckType = Cus_CheckType.耐压实验;
                    GlobalUnit.g_CurTestType = 6;
                    break;
                default:
                    CheckType = Cus_CheckType.电能误差;
                    GlobalUnit.g_CurTestType = 0;
                    break;
            }           
            return CheckType;
        }
        /// <summary>
        /// 发送停止误差板
        /// </summary>
        /// <returns></returns>
        public bool StopWcfk()
        {
            if (GlobalUnit.DriverTypes[3] == '0')
            {
                return Stop188LWcfk();
            }
            else if (GlobalUnit.DriverTypes[3] == '1')
            {
                return Stop188EWcfk();
            }
            else if (GlobalUnit.DriverTypes[3] == '2')
            {
                return Stop188GWcfk();
            }
            else if (GlobalUnit.DriverTypes[3] == '3')
            {
                return Stop188HWcfk();
            }
            else
            {
            }
            return true;
        }
        /// <summary>
        /// 停止188L误差板
        /// </summary>
        /// <returns></returns>
        private bool Stop188LWcfk()
        {
            CL188L_RequestStopPCFunctionPacket rc = new CL188L_RequestStopPCFunctionPacket();
            CL188L_RequestStopPCFunctionReplayPacket recv = new CL188L_RequestStopPCFunctionReplayPacket();
            for (int j = 0; j < m_arrErrorPort.Length; j++)
            {
                rc.Pos = 0;
                rc.ChannelNo = j;
                rc.ChannelNum = m_arrErrorPort.Length;
                rc.SetParam(bSelectBw, _CheckType);

                if (!SendPacketWithRetry(m_arrErrorPort[j], rc, recv, true))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 停止188E误差板
        /// </summary>
        /// <returns></returns>
        private bool Stop188EWcfk()
        {
            CL188_RequestStartStopPacket rc = new CL188_RequestStartStopPacket();
            rc.ControlTypes = CL188_RequestStartStopPacket.ControlType.停止当前功能;

            CL188_RequestReadStatusReplyPacket recv = new CL188_RequestReadStatusReplyPacket();
            for (int j = 0; j < m_arrErrorPort.Length; j++)
            {
                if (getErrorStates(j))
                {
                    rc.Pos = Convert.ToByte(j);
                    if (!SendPacketWithRetry(m_arrErrorPort[j], rc, recv, true))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 停止188G误差板
        /// </summary>
        /// <returns></returns>
        private bool Stop188GWcfk()
        {
            return true;
        }

        /// <summary>
        /// 停止188H误差板
        /// </summary>
        /// <returns></returns>
        private bool Stop188HWcfk()
        {
            return true;
        }

        public void GetEquipmentPressStatus(int[] meterIndex, MeterPositionPressStatus[] pressStatus)
        {
            lock (locker)
            {
                stError[] tagError = new stError[meterIndex.Length];
                ReadWcbManager readWcb = new ReadWcbManager();
                int wcbPerCount = getOneLineLoads(_meterPositions.Length);
                readWcb.WcbChannelCount = m_arrErrorPort.Length;
                readWcb.WcbPerChannelBwCount = wcbPerCount;
                readWcb.BwStatus = bDefaultSelectBw;
                readWcb.portNum = m_arrErrorPort;
                readWcb.m_curTaskType = (enmTaskType)GlobalUnit.g_CurTestType;
                readWcb.Start();
                readWcb.WaitAllThreaWorkDone();
                tagError = readWcb.tagError;

                for (int j = 0; j < tagError.Length; j++)
                {
                    if (tagError[j].statusTypeIsOn_PressDownLimt)
                    {
                        pressStatus[j] = MeterPositionPressStatus.已压接;
                    }
                    else if (tagError[j].statusTypeIsOn_PressUpLimit)
                    {
                        pressStatus[j] = MeterPositionPressStatus.未压接;
                    }
                    else
                    {
                        pressStatus[j] = MeterPositionPressStatus.压接未到位;
                    }
                }
            }
        }

        public bool StartWcfk()
        {
            if (GlobalUnit.DriverTypes[3] == '0')
            {
                return Start188LWcfk();
            }
            else if (GlobalUnit.DriverTypes[3] == '1')
            {
                return Start188EWcfk();
            }
            else if (GlobalUnit.DriverTypes[3] == '2')
            {
            }
            else if (GlobalUnit.DriverTypes[3] == '3')
            {
            }
            return true;
        }
        /// <summary>
        /// 启动188L误差板
        /// </summary>
        /// <returns></returns>
        private bool Start188LWcfk()
        {
            CL188L_RequestStartPCFunctionPacket rc = new CL188L_RequestStartPCFunctionPacket();
            CL188L_RequestStartPCFunctionReplayPacket recv = new CL188L_RequestStartPCFunctionReplayPacket();

            for (int j = 0; j < m_arrErrorPort.Length; j++)
            {
                if (getErrorStates(j))
                {
                    rc.Pos = j;
                    rc.SetPara(rc.BwStatus, Cus_CheckType.电能误差);
                    if (!SendPacketWithRetry(m_arrErrorPort[j], rc, recv, true))
                    {
                        //return false;
                    }
                }
            }


            return true;
        }

        /// <summary>
        /// 启动188E误差板
        /// </summary>
        /// <returns></returns>
        private bool Start188EWcfk()
        {
            CL188_RequestStartStopPacket rc = new CL188_RequestStartStopPacket();
            rc.ControlTypes =  CL188_RequestStartStopPacket.ControlType.启动当前功能;
            
            CL188_RequestReadStatusReplyPacket recv = new CL188_RequestReadStatusReplyPacket();
            for (int j = 0; j < m_arrErrorPort.Length; j++)
            {
                if (getErrorStates(j))
                {
                    //_CheckType
                    rc.Pos = 0xFF;//Convert.ToByte(j);
                    if (!SendPacketWithRetry(m_arrErrorPort[j], rc, recv, true))
                    {
                        //return false;
                    }
                }
            }
            return true;
        }
        public int[] GetMeterPulseCount(int[] meterIndex)
        {
            throw new NotImplementedException();
        }

        #region IControlPressMotor 成员


        public void EquipmentManualPress(int meterPositionCount, bool isPress, out bool[] results, out string[] resultDescriptions)
        {
            results = new bool[meterPositionCount];
            resultDescriptions = new string[meterPositionCount];
            int intPress = isPress ? 1 : 0;
            string strPress = isPress ? "压接" : "松开";
            bool bResult = true;
            //压接电机
            CL188L_RequestSetElectromotorPacket cl188 = new CL188L_RequestSetElectromotorPacket();
            CL188L_RequestSetElectromotorReplayPacket cl188rec = new CL188L_RequestSetElectromotorReplayPacket();
            for (int j = 0; j < m_arrErrorPort.Length; j++)
            {
                cl188.Pos = 0;
                cl188.ChannelNo = j;
                cl188.ChannelNum = m_arrErrorPort.Length;
                bool[] defaultPress = new bool[meterPositionCount];
                for (int i = 0; i < meterPositionCount; i++)
                {
                    defaultPress[i] = true;
                }
                cl188.SetPara(defaultPress, intPress);
                if (!SendPacketWithRetry(m_arrErrorPort[j], cl188, cl188rec, true))
                {
                    bResult = false;
                    break;
                }
            }
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = bResult;
                if (bResult)
                {
                    resultDescriptions[i] = string.Format("{0}成功", strPress);
                }
                else
                {
                    resultDescriptions[i] = string.Format("{0}失败", strPress);
                }
            }
            System.Threading.Thread.Sleep(5000);
        }

        public void GetEquipmentPressStatus(int meterPositionCount, out int[] meterNo, out MeterPositionPressStatus[] pressStatus)
        {
            stError[] tagError = new stError[meterPositionCount];
            meterNo = new int[meterPositionCount];
            pressStatus = new MeterPositionPressStatus[meterPositionCount];
            ReadWcbManager readWcb = new ReadWcbManager();
            int wcbPerCount = getOneLineLoads(meterPositionCount);
            readWcb.WcbChannelCount = m_arrErrorPort.Length;
            readWcb.WcbPerChannelBwCount = wcbPerCount;
            readWcb.BwStatus = new bool[meterPositionCount];
            for (int i = 0; i < meterPositionCount; i++)
            {
                readWcb.BwStatus[i] = true;
            }
            readWcb.portNum = m_arrErrorPort;
            readWcb.m_curTaskType = (enmTaskType)GlobalUnit.g_CurTestType;
            readWcb.Start();
            readWcb.WaitAllThreaWorkDone();
            tagError = readWcb.tagError;

            for (int j = 0; j < tagError.Length; j++)
            {
                meterNo[j] = j + 1;

                if (tagError[j].statusTypeIsOn_PressDownLimt)
                {
                    pressStatus[j] = MeterPositionPressStatus.已压接;
                }
                else if (tagError[j].statusTypeIsOn_PressUpLimit)
                {
                    pressStatus[j] = MeterPositionPressStatus.未压接;
                }
                else
                {
                    pressStatus[j] = MeterPositionPressStatus.压接未到位;
                }
            }
        }

        #endregion

        #region ICalcTime 成员

        int SteadyTime = 10;    //系统稳定时间(秒)
        float PowerDelay = 0.5f;//源通讯延时时间(秒)
        float StopDelay = 2f;   //停止检定延时时间(秒)

        /// <summary>
        /// 预热时间
        /// </summary>
        /// <param name="U">预热电压，单位V</param>
        /// <param name="I">预热电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="second">预热时间，单位秒</param>
        /// <returns>预热预计时间,单位秒</returns>
        public float CalcWarmUpTime(float U, float I, float acFreq, int second)
        {
            // 检定时间 + 系统稳定时间 + 源通讯延时时间 + 停止检定延时时间
            return (float)(second + SteadyTime + PowerDelay * 2 + StopDelay);
        }

        /// <summary>
        /// 合元
        /// </summary>
        /// <param name="U"></param>
        /// <param name="I"></param>
        /// <param name="factor"></param>
        /// <param name="capacitive"></param>
        /// <param name="acFreq"></param>
        /// <param name="pulse"></param>
        /// <param name="circle"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public float CalcBasicErrorTime(float U, float I, float factor, bool capacitive, float acFreq, string pulse, int circle, int count, out float verifyTime)
        {
            float clactimes = 0;
            float glys = 0f;
            float wiringNum = _meter.WiringMode == WiringMode.单相 ? 1f : 3f;
            if (factor != 1)
                glys = 0.5f;
            else
                glys = 1;
            clactimes = (circle * 3600000) / ((U * I * glys * wiringNum) * _meter.Const);
            verifyTime = clactimes + BasicErrorTimeOffset(clactimes);
            // 检定时间 + 系统稳定时间 + 源通讯延时时间 + 停止检定延时时间
            return (verifyTime + 2) * count + SteadyTime + PowerDelay * 10 + StopDelay;
        }

        /// <summary>
        /// 基本误差时间补偿
        /// </summary>
        /// <param name="everyTurnTime"></param>
        /// <returns></returns>
        private float BasicErrorTimeOffset(float everyTurnTime)
        {
            if(everyTurnTime<=3)
            { return (float)(everyTurnTime * 8); }
            else if (everyTurnTime <= 5)
            {
                return (float)(everyTurnTime * 4);
            }
            else if (everyTurnTime <= 10)
            {
                return (float)(everyTurnTime * 3);
            }
            else if (everyTurnTime <= 20)
            {
                return (float)(everyTurnTime * 2);
            }
            else if (everyTurnTime <= 50)
            {
                return (float)(everyTurnTime * 1);
            }
            else
            {
                return (float)(everyTurnTime * 0.8);
            }
        }

        /// <summary>
        /// 分元
        /// </summary>
        /// <param name="U"></param>
        /// <param name="I"></param>
        /// <param name="acFreq"></param>
        /// <param name="phase"></param>
        /// <param name="factor"></param>
        /// <param name="capacitive"></param>
        /// <param name="pulse"></param>
        /// <param name="circle"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public float CalcBasicErrorTime(float U, float I, float acFreq, string phase, float factor, bool capacitive, Pulse pulse, int circle, int count, out float verifyTime)
        {
            float clactimes = 0;
            float glys = 0f;
            float wiringNum = 1f;
            if (factor != 1)
                glys = 0.5f;
            else
                glys = 1;
            clactimes = (circle * 3600000) / ((U * I * glys * wiringNum) * _meter.Const);
            verifyTime = clactimes + BasicErrorTimeOffset(clactimes);
            // 检定时间 + 系统稳定时间 + 源通讯延时时间 + 停止检定延时时间
            return (verifyTime + 2) * count + SteadyTime + PowerDelay * 10 + StopDelay;
        }

        public float CalcStartupTime(float U, float I, float acFreq, int second)
        {
            // 检定时间 + 系统稳定时间 + 源通讯延时时间 + 停止检定延时时间
            return second + SteadyTime + PowerDelay * 6 + StopDelay;
        }

        public float CalcLatentTime(float U, float acFreq, int second)
        {
            // 检定时间 + 系统稳定时间 + 源通讯延时时间 + 停止检定延时时间
            return second + SteadyTime + PowerDelay * 6 + StopDelay;
        }

        public float CalcClockErrorTime(float U, float acFreq, float clockFreq, int second, int count, out float verifyTime)
        {
            // 检定时间 + 系统稳定时间 + 源通讯延时时间 + 停止检定延时时间
            verifyTime = second + Convert.ToInt32(second * (0.2));
            return verifyTime * count + SteadyTime + PowerDelay * 5 + StopDelay;
        }

        public float CalcEnergyReadingTime(float U, float I, float acFreq, float factor, bool capacitive, Pulse pulse, Phase phase, float energy, out float verifyTime)
        {
            float clactimes = (energy / (U * I)) * 3600000;
            verifyTime = clactimes;
            // 检定时间 + 系统稳定时间 + 源通讯延时时间 + 停止检定延时时间 + 读写报文补偿时间
            return clactimes + SteadyTime + PowerDelay * 4 + StopDelay + 10;
        }

        public float CalcDemandTime(float U, float I, float acFreq, out float verifyTime)
        {
            // 检定时间 + 系统稳定时间 + 源通讯延时时间
            verifyTime = 17 * 60;
            return 17 * 60 + SteadyTime + PowerDelay * 3;
        }

        public float CalcSwitchChangeTime(float U, float I, float acFreq, out float verifyTime)
        {
            if (_meter.WiringMode == WiringMode.单相)   // 单相
            {
                // 检定时间 + 系统稳定时间 + 源通讯延时时间 + 停止检定延时时间 + 读写报文补偿时间
                verifyTime = 120;
                return (float)(120 + SteadyTime + PowerDelay * 5 + StopDelay + 5);
            }
            else                            // 三相
            {
                // 检定时间 + 系统稳定时间 + 源通讯延时时间 + 停止检定延时时间 + 读写报文补偿时间
                verifyTime = 30 * 5;
                return (float)(30 * 5 + SteadyTime + PowerDelay * 5 + StopDelay + 7);   //默认按照5个时段来算
            }
        }

        public float CalcGetEnergyReadingTime(float U, float acFreq, Pulse pulse)
        {
            // 检定时间 + 系统稳定时间 + 源通讯延时时间
            return 10 + SteadyTime + PowerDelay * 2;
        }

        public float CalcGetDemandReadingTime(float U, float acFreq, Pulse pulse)
        {
            // 检定时间 + 系统稳定时间 + 源通讯延时时间
            return 30 + SteadyTime + PowerDelay * 2;
        }

        public float CalcReadMeterAddressTime(float U, float acFreq)
        {
            // 检定时间 + 系统稳定时间 + 源通讯延时时间
            return 30 + SteadyTime + PowerDelay * 2;
        }

        public float CalcHoldPowerTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }

        public float CalcHoldPowerCommandTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }

        public float CalcReleasePowerCommandTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }

        public float CalcRemoteOpenAccountTime(float U, float acFreq, double money, int count)
        {
            throw new NotImplementedException();
        }

        public float CalcRemoteChangeAccountTime(float U, float acFreq, double money, int count)
        {
            throw new NotImplementedException();
        }

        public float CalcRemoteSecretKeyUpdateTime(float U, float acFreq, bool isRemote)
        {
            throw new NotImplementedException();
        }

        public float CalcRemoteParameterUpdateTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }

        public float CalcRemoteDataReturnTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }

        public float CalcRemoteKnifeSwitchTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }

        public float CalcRemoteOpenSwitchCommandTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }

        public float CalcRemoteCloseSwitchCommandTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }

        public float CalcAlarmTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }

        public float CalcClearEnergyTime(float U, float acFreq)
        {
            // 检定时间 + 系统稳定时间 + 源通讯延时时间
            return 50 + SteadyTime + PowerDelay * 2;
        }

        public float CalcClearDemandTime(float U, float acFreq)
        {
            // 检定时间 + 系统稳定时间 + 源通讯延时时间
            return 50 + SteadyTime + PowerDelay * 2;
        }

        public float CalcTimeSyncTime(float U, float acFreq)
        {
            // 检定时间 + 系统稳定时间 + 源通讯延时时间
            return 30 + SteadyTime + PowerDelay * 2;
        }

        public float CalcChangePasswordTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }

        public float CalcCommunicationTestTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }

        public float CalcEventRecordTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }

        public float CalcPublicKeyAuthenticationTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }

        public float CalcPrivateKeyAuthenticationTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }

        public float CalcFreezeByTimeTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }

        public float CalcFreezeByInstantaneousTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }

        public float CalcFreezeByDayTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }

        public float CalcFreezeByHourTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }

        public float CalcCarrieWaveReturnTime(float U, float acFreq, string dataID)
        {
            throw new NotImplementedException();
        }

        public float CalcCarrieWaveReliabilityTime(float U, float acFreq, string dataID, int times)
        {
            throw new NotImplementedException();
        }

        public float CalcCarrieWaveSuccessRateTime(float U, float acFreq, string dataID, int times, int interval)
        {
            throw new NotImplementedException();
        }

        public float CalcProtocolConsistencyTime(float U, float acFreq, string dataID, int length, int digital, bool readWrite, string content)
        {
            throw new NotImplementedException();
        }

        public float CalcConnectionCheckTime(float U, float I, float acFreq)
        {
            throw new NotImplementedException();
        }

        public float CalcResistanceTime(float resistanceU, float resistanceI, int resistanceTime, string resistanceType)
        {
            return resistanceTime + SteadyTime * 5 + PowerDelay * 11 + StopDelay;
        }

        #endregion

        public void CurrentTest(float current, out bool[] results)
        {
            results = new bool[this._meterPositions.Length];

            stError[] tagError = new stError[20];
            ReadWcbManager readWcb = new ReadWcbManager();
            int wcbPerCount = getOneLineLoads(_meterPositions.Length);
            readWcb.WcbChannelCount = m_arrErrorPort.Length;
            readWcb.WcbPerChannelBwCount = wcbPerCount;
            readWcb.BwStatus = bDefaultSelectBw;
            readWcb.portNum = m_arrErrorPort;
            readWcb.m_curTaskType = (enmTaskType)GlobalUnit.g_CurTestType;
            readWcb.Start();
            readWcb.WaitAllThreaWorkDone();
            tagError = readWcb.tagError;

            for (int j = 0; j < tagError.Length; j++)
            {
                if (!tagError[j].statusTypeIsOnErr_Jxgz)
                {
                    results[j] = true;
                }
                else
                {
                    results[j] = false;
                }
            }
        }


        public void EquipmentMeterPositionManualPress(int meterPositionCount, int[] meterPosition, bool isPress, out bool[] results, out string[] resultDescriptions)
        {
            results = new bool[meterPosition.Length];
            resultDescriptions = new string[meterPosition.Length];
            for (int BwCount = 0; BwCount < meterPosition.Length; BwCount++)
            {
                bool bResult = true;

                int intPress = isPress ? 1 : 0;
                string strPress = isPress ? "压接" : "松开";
                int iChannelNo = (meterPosition[BwCount] - 1) / getOneLineLoads(meterPositionCount);
                CL188L_RequestSetElectromotorPacket cl188 = new CL188L_RequestSetElectromotorPacket();
                cl188.Pos = 0;
                cl188.ChannelNo = iChannelNo;
                cl188.ChannelNum = m_arrErrorPort.Length;
                bool[] defaultPress = new bool[meterPositionCount];
                for (int i = 0; i < meterPositionCount; i++)
                {
                    if (i == (meterPosition[BwCount] - 1))
                    {
                        defaultPress[i] = true;
                    }
                    else
                    { defaultPress[i] = false; }
                }
                cl188.SetPara(defaultPress, intPress);
                CL188L_RequestSetElectromotorReplayPacket cl188rec = new CL188L_RequestSetElectromotorReplayPacket();
                if (!SendPacketWithRetry(m_arrErrorPort[iChannelNo], cl188, cl188rec, true))
                {
                    bResult = false;
                }
                results[BwCount] = bResult;
                if (bResult)
                {
                    resultDescriptions[BwCount] = string.Format("{0}成功", strPress);
                }
                else
                {
                    resultDescriptions[BwCount] = string.Format("{0}失败", strPress);
                }
            }

        }

        /// <summary>
        /// 设备二次压接
        /// </summary>
        /// <param name="meterPositions"></param>
        /// <param name="results"></param>
        /// <param name="resultDescriptions"></param>
        public void EquipmentMeterPositionSecondPress(int[] meterPositions, out bool[] results, out string[] resultDescriptions)
        {
            this.EquipmentMeterPositionManualPress(this._meterPositions.Length, meterPositions, false, out results, out resultDescriptions);
            System.Threading.Thread.Sleep(1000);
            this.EquipmentMeterPositionManualPress(this._meterPositions.Length, meterPositions, true, out results, out resultDescriptions);
        }

        public void LatentStart()
        {
            throw new NotImplementedException();
        }

        public PulseValue ReadLatentPulse(int meterIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 断接
        /// </summary>
        /// <param name="meterPositions"></param>
        /// <param name="results"></param>
        public void CurrentShout(int[] meterPositions, out bool[] results)
        {
            results = new bool[meterPositions.Length];
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = true;
            }
            return;
        }

        #region IControlResistancePower

        public bool RemoteOpenResistanceEquipment()
        {
            return RequestSetWishStandStatus(1);
        }

        public bool RemoteCloseResistanceEquipment()
        {
            return RequestSetWishStandStatus(0);
        }

        #endregion
    }
}
