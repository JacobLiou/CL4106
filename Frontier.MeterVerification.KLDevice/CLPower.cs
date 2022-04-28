using System;
using System.Collections.Generic;

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
            m_PowerPort = serialPortSettings[0].CommPortNumber + socketSettings[0].Port1;
            RegisterPort(m_PowerPort, socketSettings[0].Port2, strSetting, socketSettings[0].IP, socketSettings[0].Port1, 1000, 200);

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

        #region IPower 成员

        public bool SetWiringMode(VerificationEquipment.Commons.WiringMode wiringMode, VerificationEquipment.Commons.Pulse pulse)
        {
            

            CL3115_RequestSetStdMeterLinkTypePacket rc3115 = new CL3115_RequestSetStdMeterLinkTypePacket();
            CL3115_RequestSetStdMeterLinkTypeReplayPacket recv3115 = new CL3115_RequestSetStdMeterLinkTypeReplayPacket();

            
            //rc3115.SetPara(

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
        /// <summary>
        /// 升起单相电压
        /// </summary>
        /// <param name="Avoltage"></param>
        /// <param name="Bvoltage"></param>
        /// <param name="Cvoltage"></param>
        /// <param name="loadPhase"></param>
        /// <param name="wiringMode"></param>
        /// <param name="factor"></param>
        /// <param name="capacitive"></param>
        /// <param name="pulse"></param>
        /// <returns></returns>
       public bool RaiseVoltage(float Avoltage, float Bvoltage, float Cvoltage, LoadPhase loadPhase, WiringMode wiringMode, float factor, bool capacitive, Pulse pulse)
       {
           if (GlobalUnit.DriverTypes[1] == '0')
           {
               return RaiseVoltageOne309(Avoltage, Bvoltage, Cvoltage, loadPhase, wiringMode, factor, capacitive, pulse);
           }
           else if (GlobalUnit.DriverTypes[1] == '1')
           {
               return RaiseVoltageOne303(Avoltage, Bvoltage, Cvoltage, loadPhase, wiringMode, factor, capacitive, pulse);
           }
           else
           {
           }
           return true;
       }

       /// <summary>
       /// 309升源
       /// </summary>
       /// <param name="voltage"></param>
       /// <param name="loadPhase"></param>
       /// <param name="wiringMode"></param>
       /// <param name="factor"></param>
       /// <param name="capacitive"></param>
       /// <param name="pulse"></param>
       /// <returns></returns>
       private bool RaiseVoltageOne309(float Avoltage, float Bvoltage, float Cvoltage, LoadPhase loadPhase, VerificationEquipment.Commons.WiringMode wiringMode, float factor, bool capacitive, VerificationEquipment.Commons.Pulse pulse)
       {
           UIPara tagUI = new UIPara();
           PhiPara tagP = new PhiPara();
           CL309_RequestPowerOnPacket rcpower = new CL309_RequestPowerOnPacket();
           tagUI.Ua = Avoltage;
           tagUI.Ub = Bvoltage;
           tagUI.Uc = Cvoltage;
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
           string StrGlys = factor != 1.0
               ? string.Format("{0}{1}", factor, capacitive ? "C" : "L")
               : factor.ToString();

           rcpower.SetPara(tagUI, tagP, phase, StrGlys, 50, getiClfs(wiringMode, pulse), 63, false, false, false, false);
           Cl309_RequestPowerOnReplyPacket recv2 = new Cl309_RequestPowerOnReplyPacket();
           //_Voltage = voltage;
           if (!SendPacketWithRetry(m_PowerPort, rcpower, recv2))
           {
               return false;
           }
           return true;
       }

       private bool RaiseVoltageOne303(float Avoltage, float Bvoltage, float Cvoltage, LoadPhase loadPhase, VerificationEquipment.Commons.WiringMode wiringMode, float factor, bool capacitive, VerificationEquipment.Commons.Pulse pulse)
       {
           CL303_RequestSetPowerOnPacket0x30.UPara upra = new CL303_RequestSetPowerOnPacket0x30.UPara();
           CL303_RequestSetPowerOnPacket0x30.PhiPara ppra = new CL303_RequestSetPowerOnPacket0x30.PhiPara();
           CL303_RequestSetPowerOnPacket0x30 rcper = new CL303_RequestSetPowerOnPacket0x30();
           upra.Ua = Avoltage;
           upra.Ub = Bvoltage;
           upra.Uc = Cvoltage;
           upra.Ia = 0;
           upra.Ib = 0;
           upra.Ic = 0;
           ppra.PhiIa = 0;
           ppra.PhiIb = 240;
           ppra.PhiIc = 120;
           ppra.PhiUa = 0;
           ppra.PhiUb = 240;
           ppra.PhiUc = 120;

           //脉冲类型
           byte clfs = 0;
           switch (wiringMode)
           {
               case WiringMode.三相四线:
                   if (pulse == Pulse.正向有功 || pulse == Pulse.反向有功)
                   {
                       clfs = 0;
                   }
                   else
                   {
                       clfs = 1;
                   }
                   break;
               case WiringMode.三相三线:
                   if (pulse == Pulse.正向有功 || pulse == Pulse.反向有功)
                   {
                       clfs = 2;
                   }
                   else
                   {
                       clfs = 3;
                   }

                   break;
               case WiringMode.单相:
                   clfs = 7;
                   break;

           }

           float phi = 0;
           
           string StrGlys = factor != 1.0
               ? string.Format("{0}{1}", factor, capacitive ? "C" : "L")
               : factor.ToString();
           switch (StrGlys)
           {
               case "1.0":
               case "1":
                   phi = 0;
                   break;
               case "0.5L":
                   phi = 60;
                   break;
               case "0.5C":
                   phi = 300;
                   break;
               case "0.8L":
                   phi = 36.8F;
                   break;
               case "0.8C":
                   phi = 323.2F;
                   break;
               case "0.25L":
                   phi = 75.5F;
                   break;
               case "0.25C":
                   phi = 284.5F;
                   break;

           }

           rcper.Freq = 50.0f;
           rcper.OpenXieBo = false;
           rcper.SetPara(clfs, upra, ppra);

           doResult(rcper, m_PowerPort);
           return true;
       }
        
        /// <summary>
        /// 升源
        /// </summary>
        /// <param name="voltage">电压</param>
        /// <param name="loadPhase">电流</param>
        /// <param name="wiringMode">接线方式</param>
        /// <returns></returns>
        public bool RaiseVoltage(float voltage, LoadPhase loadPhase, VerificationEquipment.Commons.WiringMode wiringMode, float factor, bool capacitive, VerificationEquipment.Commons.Pulse pulse)
        {
            if (GlobalUnit.DriverTypes[1] == '0')
            {
               return RaiseVoltage309(voltage, loadPhase, wiringMode, factor, capacitive, pulse);
            }
            else if (GlobalUnit.DriverTypes[1] == '1')
            {
                return RaiseVoltage303(voltage, loadPhase, wiringMode, factor, capacitive, pulse);
            }
            else
            {
            }

            return true;
        }
        /// <summary>
        /// 309升源
        /// </summary>
        /// <param name="voltage"></param>
        /// <param name="loadPhase"></param>
        /// <param name="wiringMode"></param>
        /// <param name="factor"></param>
        /// <param name="capacitive"></param>
        /// <param name="pulse"></param>
        /// <returns></returns>
        private bool RaiseVoltage309(float voltage, LoadPhase loadPhase, VerificationEquipment.Commons.WiringMode wiringMode, float factor, bool capacitive, VerificationEquipment.Commons.Pulse pulse)
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
            string StrGlys = factor != 1.0
                ? string.Format("{0}{1}", factor, capacitive ? "C" : "L")
                : factor.ToString();

            rcpower.SetPara(tagUI, tagP, phase, StrGlys, 50, getiClfs(wiringMode, pulse), 63, false, false, false, false);
            Cl309_RequestPowerOnReplyPacket recv2 = new Cl309_RequestPowerOnReplyPacket();
            _Voltage = voltage;
            if (!SendPacketWithRetry(m_PowerPort, rcpower, recv2))
            {
                return false;
            }
            return true;
        }

        private bool RaiseVoltage303(float voltage, LoadPhase loadPhase, VerificationEquipment.Commons.WiringMode wiringMode, float factor, bool capacitive, VerificationEquipment.Commons.Pulse pulse)
        {
            CL303_RequestSetPowerOnPacket0x35 rcpower = new CL303_RequestSetPowerOnPacket0x35();
            CL303_RequestSetPowerOnPacket0x30.UPara upra = new CL303_RequestSetPowerOnPacket0x30.UPara();
            upra.Ua = voltage;
            upra.Ub = voltage;
            upra.Uc = voltage;
            if (wiringMode == WiringMode.二相三线)
            {
                wiringMode = WiringMode.三相四线;
                upra.Ub = 0;
            }
            CL303_RequestSetPowerOnPacket0x30.PhiPara ppra = new CL303_RequestSetPowerOnPacket0x30.PhiPara();
            ppra.PhiIa = 0;
            ppra.PhiIb = 120;
            ppra.PhiIc = 240;
            ppra.PhiUa = 0;
            ppra.PhiUb = 120;
            ppra.PhiUc = 240;
            byte byt_XWKG = 63;

         

            Cus_PowerYuanJiang phase;
            switch (loadPhase)
            {
                case LoadPhase.A:
                    phase = Cus_PowerYuanJiang.A;
                    byt_XWKG &= 0xf;

                    break;
                case LoadPhase.B:
                    phase = Cus_PowerYuanJiang.B;
                    byt_XWKG &= 0x17;
                    break;
                case LoadPhase.C:
                    phase = Cus_PowerYuanJiang.C;
                    byt_XWKG &= 0x27;
                    break;
                default:
                    phase = Cus_PowerYuanJiang.H;
                    upra.Ia = 0;
                    upra.Ib = 0;
                    upra.Ic = 0;
                    ppra.PhiIa = 0;
                    ppra.PhiIb = 120;
                    ppra.PhiIc = 240;
                    break;
            }
            rcpower.m_YuanJian = phase;
            byte clfs = 0;
            switch (wiringMode)
            {
                case WiringMode.三相四线:
                    if (pulse == Pulse.正向有功 || pulse == Pulse.反向有功)
                    {
                        clfs = 0;
                    }
                    else
                    {
                        clfs = 1;
                    }
                    break;
                case WiringMode.三相三线:
                    if (pulse == Pulse.正向有功 || pulse == Pulse.反向有功)
                    {
                        clfs = 2;
                    }
                    else
                    {
                        clfs = 3;
                    }

                    break;
                case WiringMode.单相:
                    clfs = 7;
                    break;

            }


            float phi = 0;
            string StrGlys = factor != 1.0
                ? string.Format("{0}{1}", factor, capacitive ? "C" : "L")
                : factor.ToString();
            switch (StrGlys)
            {
                case "1.0":
                    phi = 0;
                    break;
                case "0.5L":
                    phi = 60;
                    break;
                case "0.5C":
                    phi = 300;
                    break;
                case "0.8L":
                    phi = 36.8F;
                    break;
                case "0.8C":
                    phi = 323.2F;
                    break;
                case "0.25L":
                    phi = 75.5F;
                    break;
                case "0.25C":
                    phi = 284.5F;
                    break;

            }

            //if (loadPhase == LoadPhase.None)
            //{
                //为了电量寄存器计算 所以在合元时我们通过计算得到功率因数
            phi = GetPhiGlys((int)clfs, StrGlys, phase, pulse);     //根据测试方式和功率因数计算角度
  
            
     

            rcpower.SetPara(clfs, byt_XWKG, false, voltage, 0, false, phi);

            if (!doResult(rcpower, m_PowerPort))
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// 计算角度
        /// </summary>
        /// <param name="int_Clfs">测量方式</param>
        /// <param name="str_Glys">功率因数</param>
        /// <returns></returns>
        private Single GetPhiGlys(int int_Clfs, string str_Glys, Cus_PowerYuanJiang phase, Pulse pulse)
        {
            string str_CL = str_Glys.ToUpper().Substring(str_Glys.Length - 1, 1);
            Double dbl_XS = 0;
            if (str_CL == "C" || str_CL == "L")
                dbl_XS = Convert.ToDouble(str_Glys.Substring(0, str_Glys.Length - 1));
            else
                dbl_XS = Convert.ToDouble(str_Glys);
            Double dbl_Phase;
            if (int_Clfs == 1 || int_Clfs == 3 || int_Clfs == 6)
                dbl_Phase = Math.Asin(Math.Abs(dbl_XS));                              //无功计算
            else
                dbl_Phase = Math.Acos(Math.Abs(dbl_XS));                              //有功计算

            dbl_Phase = dbl_Phase * 180 / Math.PI;      //角度换算
            if (dbl_XS < 0) dbl_Phase = 180 + dbl_Phase;         //反向
            if (str_CL == "C") dbl_Phase = 360 - dbl_Phase;
            if (dbl_Phase < 0) dbl_Phase = 360 + dbl_Phase;
            if (pulse.ToString().Contains("反"))
            {
                dbl_Phase += 180;
            }
            if (int_Clfs == 2 || int_Clfs == 3)
            {
                if (phase == Cus_PowerYuanJiang.A)
                {
                    dbl_Phase -= 30;
                  //  dbl_Phase = 180 + dbl_Phase;
                }
                if (phase == Cus_PowerYuanJiang.C)
                {
                    dbl_Phase += 30;
                }
                dbl_Phase += 360;
            }

            dbl_Phase %= 360;
  




            return Convert.ToSingle(dbl_Phase);
        }
        /// <summary>
        /// 计算角度
        /// </summary>
        /// <param name="p_int_Clfs">测量方式</param>
        /// <param name="p_str_Glys">功率因素名</param>
        /// <param name="p_eet_Element">合分元</param>
        /// <returns></returns>
        private float GetPhiGlys(int p_int_Clfs, string p_str_Glys, LoadPhase p_eet_Element)
        {
            string str_GlysName = p_str_Glys.Replace("-", "").Replace("+", "");
            string str_GlysF = p_str_Glys.Contains("-") ? "-" : "+";
            string str_ClfsCode = "";
            string str_ElementCode = "";

            switch (p_int_Clfs)
            {
                case 0:
                    str_ClfsCode = "00";
                    break;
                case 1:
                    str_ClfsCode = "01";
                    break;
                case 2:
                    str_ClfsCode = "10";
                    break;
                case 3:
                    str_ClfsCode = "11";
                    break;
                case 4:
                    str_ClfsCode = "21";
                    break;
                case 5:
                    str_ClfsCode = "31";
                    break;
                case 6:
                    str_ClfsCode = "41";
                    break;
                case 7:
                    str_ClfsCode = "51";
                    break;
                default:
                    str_ClfsCode = "00";
                    break;
            }
            switch (p_eet_Element)
            {
                case LoadPhase.None:
                    str_ElementCode = "1";
                    break;
                case LoadPhase.A:
                    str_ElementCode = "2";
                    break;
                case LoadPhase.B:
                    str_ElementCode = "3";
                    break;
                case LoadPhase.C:
                    str_ElementCode = "4";
                    break;
                default:
                    str_ElementCode = "1";
                    break;
            }

            //csGlys gly_Instance = new csGlys();
            //gly_Instance.Load();
            //string str_JD = gly_Instance.getJiaoDu(str_GlysName)[str_ClfsCode + str_ElementCode];
            //Single sng_JD = Single.Parse(str_JD);
            //if (str_GlysF.Equals("-"))
            //{
            //    sng_JD += 180;
            //    sng_JD %= 360;
            //}
            return 0;
        }

        public int getiClfs(WiringMode wir, Pulse pulse)
        {
            //    单相 = 1,
            //三相三线 = 2,
            //三相四线 = 3,
            int pulseType = 0;
            if (wir == WiringMode.单相)
            { pulseType = 7; }
            else if (wir == WiringMode.三相三线)
            {
                if (Convert.ToInt32(pulse) <= 2)
                { pulseType = 2; }
                else
                { pulseType = 3; }
            }
            else if (wir == WiringMode.三相四线)
            {
                if (Convert.ToInt32(pulse) <= 2)
                { pulseType = 0; }
                else
                { pulseType = 1; }
            }
            return pulseType;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        /// <param name="loadPhase"></param>
        /// <param name="wiringMode"></param>
        /// <param name="pulse"></param>
        /// <returns></returns>
        public bool RaiseCurrent(float current, LoadPhase loadPhase, WiringMode wiringMode, float factor, bool capacitive, VerificationEquipment.Commons.Pulse pulse)
        {
            UIPara tagUI = new UIPara();
            PhiPara tagP = new PhiPara();
            CL309_RequestPowerOnPacket rcpower = new CL309_RequestPowerOnPacket();
            tagUI.Ua = _Voltage;
            tagUI.Ub = _Voltage;
            tagUI.Uc = _Voltage;



            tagP.PhiUa = 0;
            tagP.PhiUb = 120;
            tagP.PhiUc = 240;
            //by -zjl
            byte byt_XWKG = 63;
            if (wiringMode >= WiringMode.三相三线)
            {
                byt_XWKG &= 0x2D;   //三相三线 去掉B相       
            }

            Cus_PowerYuanJiang phase;
            switch (loadPhase)
            {
                case LoadPhase.None:
                case LoadPhase.AC:
                    phase = Cus_PowerYuanJiang.H;
                    tagUI.Ia = current;
                    tagUI.Ib = current;
                    tagUI.Ic = current;
                    tagP.PhiIa = 0;
                    tagP.PhiIb = 120;
                    tagP.PhiIc = 240;
                    break;
                case LoadPhase.A:
                    phase = Cus_PowerYuanJiang.A;
                    byt_XWKG &= 0xf;
                    tagUI.Ia = current;
                    tagUI.Ib = 0;
                    tagUI.Ic = 0;

                    tagP.PhiIa = 0;
                    tagP.PhiIb = 0;
                    tagP.PhiIc = 0;
                    break;
                case LoadPhase.B:
                    phase = Cus_PowerYuanJiang.B;
                    byt_XWKG &= 0x17;
                    tagUI.Ia = 0;
                    tagUI.Ib = current;
                    tagUI.Ic = 0;

                    tagP.PhiIa = 0;
                    tagP.PhiIb = 0;
                    tagP.PhiIc = 0;
                    break;
                case LoadPhase.C:
                    phase = Cus_PowerYuanJiang.C;
                    byt_XWKG &= 0x27;
                    tagUI.Ia = 0;
                    tagUI.Ib = 0;
                    tagUI.Ic = current;

                    tagP.PhiIa = 0;
                    tagP.PhiIb = 0;
                    tagP.PhiIc = 0;
                    break;
                default:
                    phase = Cus_PowerYuanJiang.H;
                    break;
            }
            string StrGlys = factor != 1.0
                ? string.Format("{0}{1}", factor, capacitive ? "C" : "L")
                : factor.ToString();

            rcpower.SetPara(tagUI, tagP, phase, StrGlys, 50, getiClfs(wiringMode, pulse), byt_XWKG, false, false, false, false);
            Cl309_RequestPowerOnReplyPacket recv2 = new Cl309_RequestPowerOnReplyPacket();
            if (!SendPacketWithRetry(m_PowerPort, rcpower, recv2))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 控源指令
        /// </summary>
        /// <param name="UA"></param>
        /// <param name="IA"></param>
        /// <param name="Afactor"></param>
        /// <param name="Acapacitive"></param>
        /// <param name="UB"></param>
        /// <param name="IB"></param>
        /// <param name="Bfactor"></param>
        /// <param name="Bcapacitive"></param>
        /// <param name="UC"></param>
        /// <param name="IC"></param>
        /// <param name="Cfactor"></param>
        /// <param name="Ccapacitive"></param>
        public void RaiseVoltageCurrentForJcjd(float UA, float IA, float Afactor, bool Acapacitive, float UB, float IB, float Bfactor, bool Bcapacitive,
            float UC, float IC, float Cfactor, bool Ccapacitive, VerificationEquipment.Commons.WiringMode wiringMode, Pulse pulse)
        {
            if (GlobalUnit.DriverTypes[1] == '0')
            {
                RaiseVoltageCurrentForCl309Jcjd(UA, IA, Afactor, Acapacitive, UB, IB, Bfactor, Bcapacitive,
                    UC, IC, Cfactor, Ccapacitive,wiringMode,pulse);
            }
            else if (GlobalUnit.DriverTypes[1] == '1')
            {
                RaiseVoltageCurrentForCl303V2Jcjd(UA, IA, Afactor, Acapacitive, UB, IB, Bfactor, Bcapacitive,
                    UC, IC, Cfactor, Ccapacitive, wiringMode, pulse);
            }
        }
        /// <summary>
        /// 控309源 单向控源
        /// </summary>
        /// <param name="UA"></param>
        /// <param name="IA"></param>
        /// <param name="Afactor"></param>
        /// <param name="Acapacitive"></param>
        /// <param name="UB"></param>
        /// <param name="IB"></param>
        /// <param name="Bfactor"></param>
        /// <param name="Bcapacitive"></param>
        /// <param name="UC"></param>
        /// <param name="IC"></param>
        /// <param name="Cfactor"></param>
        /// <param name="Ccapacitive"></param>
        private void RaiseVoltageCurrentForCl309Jcjd(float UA, float IA, float Afactor, bool Acapacitive, float UB, float IB, float Bfactor, bool Bcapacitive,
            float UC, float IC, float Cfactor, bool Ccapacitive, VerificationEquipment.Commons.WiringMode wiringMode, Pulse pulse)
        {
            UIPara tagUI = new UIPara();
            PhiPara tagP = new PhiPara();
            CL309_RequestPowerOnPacket rcpower = new CL309_RequestPowerOnPacket();
            tagUI.Ua = UA;
            tagUI.Ub = UB;
            tagUI.Uc = UC;
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

            phase = Cus_PowerYuanJiang.H;


            string StrGlys = "1.0";
            rcpower.SetPara(tagUI, tagP, phase, StrGlys, 50, getiClfs(wiringMode, pulse), 63, false, false, false, false);
            Cl309_RequestPowerOnReplyPacket recv2 = new Cl309_RequestPowerOnReplyPacket();
            //
            SendPacketWithRetry(m_PowerPort, rcpower, recv2);
           
                
        }
        /// <summary>
        /// 控303V2源
        /// </summary>
        /// <param name="UA">A相电压</param>
        /// <param name="IA">A相电流</param>
        /// <param name="Afactor">A相功率因数</param>
        /// <param name="Acapacitive">A相感性OR容性</param>
        /// <param name="UB"></param>
        /// <param name="IB"></param>
        /// <param name="Bfactor"></param>
        /// <param name="Bcapacitive"></param>
        /// <param name="UC"></param>
        /// <param name="IC"></param>
        /// <param name="Cfactor"></param>
        /// <param name="Ccapacitive"></param>
        private void RaiseVoltageCurrentForCl303V2Jcjd(float UA, float IA, float Afactor, bool Acapacitive, float UB, float IB, float Bfactor, bool Bcapacitive,
            float UC, float IC, float Cfactor, bool Ccapacitive, VerificationEquipment.Commons.WiringMode wiringMode, Pulse pulse)
        {
            
            CL303_RequestSetPowerOnPacket0x30.UPara upra = new CL303_RequestSetPowerOnPacket0x30.UPara();
            CL303_RequestSetPowerOnPacket0x30.PhiPara ppra = new CL303_RequestSetPowerOnPacket0x30.PhiPara();
            CL303_RequestSetPowerOnPacket0x30 rcper = new CL303_RequestSetPowerOnPacket0x30();


            upra.Ua = UA;
            upra.Ub = UB;
            upra.Uc = UC;
            upra.Ia = IA;
            upra.Ib = IB;
            upra.Ic = IC;
            ppra.PhiIa = 0;
            ppra.PhiIb = 240;
            ppra.PhiIc = 120;
            ppra.PhiUa = 0;
            ppra.PhiUb = 240;
            ppra.PhiUc = 120;

            //脉冲类型
            byte clfs = 0;
            switch (wiringMode)
            {
                case WiringMode.三相四线:
                    if (pulse == Pulse.正向有功 || pulse == Pulse.反向有功)
                    {
                        clfs = 0;
                    }
                    else
                    {
                        clfs = 1;
                    }
                    break;
                case WiringMode.三相三线:
                    if (pulse == Pulse.正向有功 || pulse == Pulse.反向有功)
                    {
                        clfs = 2;
                    }
                    else
                    {
                        clfs = 3;
                    }

                    break;
                case WiringMode.单相:
                    clfs = 7;
                    break;

            }

            float phi = 0;
            //A相相位
            string StrGlys = Afactor != 1.0
                ? string.Format("{0}{1}", Afactor, Acapacitive ? "C" : "L")
                : Afactor.ToString();
            switch (StrGlys)
            {
                case "1.0":
                case "1":
                    phi = 0;
                    break;
                case "0.5L":
                    phi = 60;
                    break;
                case "0.5C":
                    phi = 300;
                    break;
                case "0.8L":
                    phi = 36.8F;
                    break;
                case "0.8C":
                    phi = 323.2F;
                    break;
                case "0.25L":
                    phi = 75.5F;
                    break;
                case "0.25C":
                    phi = 284.5F;
                    break;

            }
            ppra.PhiIa = phi;

            //b相相位
             StrGlys = Bfactor != 1.0
                ? string.Format("{0}{1}", Bfactor, Bcapacitive ? "C" : "L")
                : Bfactor.ToString();
            switch (StrGlys)
            {
                case "1.0":
                case "1":
                    phi = 0;
                    break;
                case "0.5L":
                    phi = 60;
                    break;
                case "0.5C":
                    phi = 300;
                    break;
                case "0.8L":
                    phi = 36.8F;
                    break;
                case "0.8C":
                    phi = 323.2F;
                    break;
                case "0.25L":
                    phi = 75.5F;
                    break;
                case "0.25C":
                    phi = 284.5F;
                    break;

            }
            ppra.PhiIb = phi;

            //C相相位
            StrGlys = Bfactor != 1.0
               ? string.Format("{0}{1}", Bfactor, Bcapacitive ? "C" : "L")
               : Bfactor.ToString();
            switch (StrGlys)
            {
                case "1.0":
                case "1":
                    phi = 0;
                    break;
                case "0.5L":
                    phi = 60;
                    break;
                case "0.5C":
                    phi = 300;
                    break;
                case "0.8L":
                    phi = 36.8F;
                    break;
                case "0.8C":
                    phi = 323.2F;
                    break;
                case "0.25L":
                    phi = 75.5F;
                    break;
                case "0.25C":
                    phi = 284.5F;
                    break;

            }
            ppra.PhiIc = phi;

            rcper.SetPara(clfs, upra, ppra);

            doResult(rcper, m_PowerPort);
        }

        /// <summary>
        /// 控源指令  
        /// </summary>
        /// <param name="voltage"></param>
        /// <param name="current"></param>
        /// <param name="loadPhase"></param>
        /// <param name="wiringMode"></param>
        /// <param name="factor"></param>
        /// <param name="capacitive"></param>
        /// <param name="pulse"></param>
        /// <returns></returns>
        public bool RaiseVotageCurrent(float voltage, float current, LoadPhase loadPhase, VerificationEquipment.Commons.WiringMode wiringMode, float factor, bool capacitive, Pulse pulse)
        {
            bool rlt = false;
            if (GlobalUnit.DriverTypes[1] == '0')
            {
                rlt = CL309_VC(voltage, current, loadPhase, wiringMode, factor, capacitive, pulse);
            }
            else if (GlobalUnit.DriverTypes[1] == '1')
            {
                rlt = CL303_VC(voltage, current, loadPhase, wiringMode, factor, capacitive, pulse);
            }
            return rlt;
        }

        private bool CL303_VC(float voltage, float current, LoadPhase loadPhase, WiringMode wiringMode, float factor, bool capacitive, Pulse pulse)
        {
            
            CL303_RequestSetPowerOnPacket0x35 rcpower = new CL303_RequestSetPowerOnPacket0x35();
            CL303_RequestSetPowerOnPacket0x30.UPara upra=new CL303_RequestSetPowerOnPacket0x30.UPara();
            upra.Ua = voltage;
            upra.Ub = voltage;
            upra.Uc = voltage;

            WiringMode wirTmp =wiringMode; 
            if (wiringMode == WiringMode.二相三线)
            {
                wiringMode = WiringMode.三相四线;
                upra.Ub = 0;
            }
            CL303_RequestSetPowerOnPacket0x30.PhiPara ppra=new CL303_RequestSetPowerOnPacket0x30.PhiPara();
            ppra.PhiIa = 0;
            ppra.PhiIb = 240;
            ppra.PhiIc = 120;
            ppra.PhiUa = 0;
            ppra.PhiUb = 240;
            ppra.PhiUc = 120;
            byte byt_XWKG = 63;
            Cus_PowerYuanJiang phase;
            switch (loadPhase)
            {
                case LoadPhase.A:
                    phase = Cus_PowerYuanJiang.A;
                    byt_XWKG &= 0xf;
                    
                    break;
                case LoadPhase.B:
                    phase = Cus_PowerYuanJiang.B;
                    byt_XWKG &= 0x17;
                    break;
                case LoadPhase.C:
                    phase = Cus_PowerYuanJiang.C;
                    byt_XWKG &= 0x27;
                    break;
                default:
                    phase = Cus_PowerYuanJiang.H;
                    upra.Ia = current;
                    upra.Ib = current;
                    upra.Ic = current;
                    ppra.PhiIa = 0;
                    ppra.PhiIb = 240;
                    ppra.PhiIc = 120;

                    if (wirTmp == WiringMode.二相三线)
                    {
                        upra.Ib = 0;
                    }
                    break;
            }
            rcpower.m_YuanJian = phase;
            byte clfs=0;
            switch (wiringMode)
            {
                case WiringMode.三相四线:
                    if (pulse == Pulse.正向有功 || pulse == Pulse.反向有功)
                    {
                        clfs = 0;
                    }
                    else
                    {
                        clfs = 1;
                    }
                    break;
                case WiringMode.三相三线:
                    if (pulse == Pulse.正向有功 || pulse == Pulse.反向有功)
                    {
                        clfs = 2;
                    }
                    else
                    {
                        clfs = 3;
                    }
                    
                    break;
                case WiringMode.单相:
                    clfs = 7;
                    break;
                
            }
            
            
            float phi=0;
            string StrGlys = factor != 1.0
                ? string.Format("{0}{1}", factor, capacitive ? "C" : "L")
                : factor.ToString();
            switch (StrGlys)
            {
                case "1.0":
                case "1":
                    phi = 0;
                    break;
                case "0.5L":
                    phi = 60;
                    break;
                case "0.5C":
                    phi = 300;
                    break;
                case "0.8L":
                    phi = 36.8F;
                    break;
                case "0.8C":
                    phi = 323.2F;
                    break;
                case "0.25L":
                    phi = 75.5F;
                    break;
                case "0.25C":
                    phi = 284.5F;
                    break;

            }
            //phase;pulse
            phi = GetPhiGlys((int)clfs, StrGlys, phase, pulse);     //根据测试方式和功率因数计算角度
             
            rcpower.SetPara(clfs, byt_XWKG, false, voltage, current, false, phi);

            if (!doResult(rcpower, m_PowerPort))
            {
                return false;
            }

            return true;
        }

        private bool CL309_VC(float voltage, float current, LoadPhase loadPhase, VerificationEquipment.Commons.WiringMode wiringMode, float factor, bool capacitive, Pulse pulse)
        {
            UIPara tagUI = new UIPara();
            PhiPara tagP = new PhiPara();
            CL309_RequestPowerOnPacket rcpower = new CL309_RequestPowerOnPacket();
            tagUI.Ua = voltage;
            tagUI.Ub = voltage;
            tagUI.Uc = voltage;

            tagP.PhiIa = 0;
            tagP.PhiIb = 120;
            tagP.PhiIc = 240;
            tagP.PhiUa = 0;
            tagP.PhiUb = 120;
            tagP.PhiUc = 240;

            byte byt_XWKG = 63;
            if (wiringMode >= WiringMode.三相三线)
            {
                byt_XWKG &= 0x2D;   //三相三线 去掉B相       
            }

            Cus_PowerYuanJiang phase;
            switch (loadPhase)
            {
                case LoadPhase.A:
                    phase = Cus_PowerYuanJiang.A;
                    byt_XWKG &= 0xf;
                    tagUI.Ia = current;
                    tagUI.Ib = 0;
                    tagUI.Ic = 0;

                    tagP.PhiIa = 0;
                    tagP.PhiIb = 0;
                    tagP.PhiIc = 0;
                    break;
                case LoadPhase.B:
                    phase = Cus_PowerYuanJiang.B;
                    byt_XWKG &= 0x17;
                    tagUI.Ia = 0;
                    tagUI.Ib = current;
                    tagUI.Ic = 0;

                    tagP.PhiIa = 0;
                    tagP.PhiIb = 0;
                    tagP.PhiIc = 0;
                    break;
                case LoadPhase.C:
                    phase = Cus_PowerYuanJiang.C;
                    byt_XWKG &= 0x27;
                    tagUI.Ia = 0;
                    tagUI.Ib = 0;
                    tagUI.Ic = current;

                    tagP.PhiIa = 0;
                    tagP.PhiIb = 0;
                    tagP.PhiIc = 0;
                    break;
                default:
                    phase = Cus_PowerYuanJiang.H;
                    tagUI.Ia = current;
                    tagUI.Ib = current;
                    tagUI.Ic = current;
                    tagP.PhiIa = 0;
                    tagP.PhiIb = 120;
                    tagP.PhiIc = 240;
                    break;
            }

            string StrGlys = factor != 1.0
                ? string.Format("{0}{1}", factor, capacitive ? "C" : "L")
                : factor.ToString();

            rcpower.SetPara(tagUI, tagP, phase, StrGlys, 50, getiClfs(wiringMode, pulse), byt_XWKG, false, false, false, false);
            Cl309_RequestPowerOnReplyPacket recv2 = new Cl309_RequestPowerOnReplyPacket();
            if (!SendPacketWithRetry(m_PowerPort, rcpower, recv2))
            {
                return false;
            }


            return true;
        }


        /// <summary>
        ///  停止源
        /// </summary>
        /// <returns></returns>
        public bool StopVerification()
        {
            if (GlobalUnit.DriverTypes[1] == '0')
            {//309
                return StopCl309Verification();
            }
            else if (GlobalUnit.DriverTypes[1] == '1')
            {//303
                Stop303Verification();
            }
            else
            {
            }

            return true;
        }

        /// <summary>
        /// 停止309源操作
        /// </summary>
        /// <returns></returns>
        private bool StopCl309Verification()
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
        /// <summary>
        /// 停止303源
        /// </summary>
        /// <returns></returns>
        private bool Stop303Verification()
        {
            /*
            CL303_RequestSetPowerOnPacket0x30 rc = new CL303_RequestSetPowerOnPacket0x30();
            CL303_RequestReadPowerStateReplyPacket recv = new CL303_RequestReadPowerStateReplyPacket();

            CL303_RequestSetPowerOnPacket0x30.UPara Ui = new CL303_RequestSetPowerOnPacket0x30.UPara();
            CL303_RequestSetPowerOnPacket0x30.PhiPara Phi = new CL303_RequestSetPowerOnPacket0x30.PhiPara();
            Ui.Ua = 0;
            Ui.Ub = 0;
            Ui.Uc = 0;
            Ui.Ia = 0;
            Ui.Ib = 0;
            Ui.Ib = 0;
            Ui.Ic = 0;

            Phi.PhiUa = 0;
            Phi.PhiUb = 0;
            Phi.PhiUc = 0;
            Phi.PhiIa = 0;
            Phi.PhiIb = 0;
            Phi.PhiIc = 0;
            rc.SetPara(Convert.ToByte( 7), Ui, Phi);

             * */
            CL303_RequestSetPowerOffPacket rc = new CL303_RequestSetPowerOffPacket();
            CL303_RequestReadPowerStateReplyPacket recv = new CL303_RequestReadPowerStateReplyPacket();
            if (!SendPacketWithRetry(m_PowerPort, rc, recv))
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
        /// <summary>
        /// 发送返回CLNormalRequestResultReplayPacket的数据包并返回结果
        /// </summary>
        /// <param name="rac"></param>
        /// <param name="port"></param>
        /// <param name="ename"></param>
        /// <returns></returns>
        private bool doResult(SendPacket rac, int port)
        {
            bool result = true;
            
            RecvPacket rcback = new CLNormalRequestResultReplayPacket();
            result = this.SendPacketWithRetry(port, rac, rcback);
            if (result)
            {
                if (((CLNormalRequestResultReplayPacket)rcback).ReplayResult
                    != CLNormalRequestResultReplayPacket.ReplayCode.Ok)
                {
                    result = false;
                    
                }
                
            }
            
                
            return result;
        }
    }
}
