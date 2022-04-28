using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VerificationEquipment.Commons;
using System.Drawing;
using System.Threading;
using Frontier.MeterVerification.DeviceCommon;
using System.Configuration;
using Frontier.MeterVerification.DeviceInterface;
using Frontier.MeterVerification.KLDevice;
//using Common.Logging;

namespace Frontier.MeterVerification.Communication
{
    public class MeterEquipment : EquipmentBase
    {
        #region 参数定义
        //各接口的定义
        private IControlPressMotor controlPressMotor = null;
        private IControlReversalMotor controlReversalMotor = null;
        private IControlEquipmentPower controlEquipmentPower = null;
        private IControlResistancePower controlResistancePower = null;
        private IControlResistanceMoto controlResistanceMoto = null;
        private IDlt645 dlt645 = null;
        private ILight light = null;
        private IMonitor monitor = null;
        private IPower power = null;
        private IPowerConsume powerConsume = null;
        private IResistance resistance = null;
        private IStdMeter stdMeter = null;
        private IEquipmentStatus equipmentStatus = null;
        private IMeterPositionStatus meterPositionStatus = null;
        private ITemperature temperature = null;
        private ITimeChannel timeChannel = null;
        private IWcfk wcfk = null;
        private IPowerSupply powerSupply = null;
        private ICurrentControl currentControl = null;
        private ICalcTime calcTime = null;
        private IResistanceWcfk resistanceWcfk = null;
        //其他参数
        int SteadyTime = 6;//系统稳定时间(秒)
        private List<IConnect> connects = null;
        private Meter meter;
        private List<int> lstMeterIndex = new List<int>();// 挂表表位
        //private static ILog LOG = LogManager.GetLogger(typeof(MeterEquipment));
        bool Netencryption = true;//true：网络型加密机，false：开发套件。 
        string putdiv = "0000000000000001"; //分散因子
        //ESAM参数 
        string ESAM_IP = "172.17.33.44";
        ushort ESAM_PORT = 8001;
        string ESAM_PWD = "11111111";

        #endregion

        /// 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public MeterEquipment()
        {
            #region 初始化设备
            CreatNewIni();
            //
            INIHelper iniHelper = new INIHelper("DeviceConfig.ini");
            string[] devices = iniHelper.IniReadValue("Main", "Devices").Split(',');
            //string[] devices = ConfigurationManager.AppSettings["Devices"].Split(',');
            CommPortDevice commDevice = null;
            //判定是否初始化表位485端口
            string bs = iniHelper.IniReadValue("EquipSet", "cbx_Rs485");

            for (int i = 0; i < devices.Length; i++)
            {
                List<CommSerialPortSettings> commSerialPortSettings = new List<CommSerialPortSettings>();
                List<CommSocketSettings> commSocketSettings = new List<CommSocketSettings>();
                //实例化设备
                string device = iniHelper.IniReadValue("Main", devices[i]);
                //string device = ConfigurationManager.AppSettings[devices[i]];
                Type type = Type.GetType(device);
                commDevice = (CommPortDevice)type.Assembly.CreateInstance(type.FullName);
                //实例化端口
                string devicePortSetting = iniHelper.IniReadValue("Main", string.Format("{0}_{1}", devices[i], "PortSetting"));
                //string devicePortSetting = ConfigurationManager.AppSettings[string.Format("{0}_{1}", devices[i], "PortSetting")];
                string[] devicePorts = devicePortSetting.Split('|');
                for (int j = 0; j < devicePorts.Length; j++)
                {
                    string[] deviePortSettings = devicePorts[j].Split(',');
                    if (deviePortSettings[0].ToUpper() == "SerialPort".ToUpper())
                    {
                        CommSerialPortSettings commSerialPortSetting = new CommSerialPortSettings();
                        commSerialPortSetting.CommPortNumber = int.Parse(deviePortSettings[1]);
                        commSerialPortSetting.BaudRate = int.Parse(deviePortSettings[2]);
                        commSerialPortSetting.Parity = deviePortSettings[3]; //CommPortParity.None;
                        commSerialPortSetting.DataBits = int.Parse(deviePortSettings[4]);
                        commSerialPortSetting.StopBits = int.Parse(deviePortSettings[5]);
                        commSerialPortSettings.Add(commSerialPortSetting);
                    }
                    else if (deviePortSettings[0].ToUpper() == "Socket".ToUpper())
                    {
                        CommSocketSettings commSocketSetting = new CommSocketSettings();
                        commSocketSetting.IP = deviePortSettings[1];
                        commSocketSetting.Port1 = int.Parse(deviePortSettings[2]);
                        if (deviePortSettings.Length > 3)
                        {
                            commSocketSetting.Port2 = int.Parse(deviePortSettings[3]);
                        }
                        commSocketSettings.Add(commSocketSetting);
                    }
                }
                //
                if (bs == "Close" && devices[i] == "CLDlt645_2007")
                {
                    //不注册485端口
                }
                //if (bs == "Close")
                //{
                //    //不注册485端口
                //}
                else
                {
                    commDevice.Config(commSerialPortSettings, commSocketSettings);
                    //注册设备
                    DeviceManager.RegisterDevice(type, commDevice);
                }
            }
            //增加设备类型选择
            #region 增加自动判定设备类型 zxr 20131104
            //comBox_Std=CL311V2
            //comBox_Power=CL303
            //comBox_TimeBs=CL191B
            //comBox_Error=CL188E
            string stStd = string.Empty;
            string stPower = string.Empty;
            string stTimeBs = string.Empty;
            string stError = string.Empty;
            //标准表
            stStd = iniHelper.IniReadValue("EquipSet", "comBox_Std");
            //功率源
            stPower = iniHelper.IniReadValue("EquipSet", "comBox_Power");
            //时基源
            stTimeBs = iniHelper.IniReadValue("EquipSet", "comBox_TimeBs");
            //误差板
            stError = iniHelper.IniReadValue("EquipSet", "comBox_Error");
            string strIsPush = iniHelper.IniReadValue("EquipSet", "IsPushBox");
            if (String.IsNullOrEmpty(strIsPush))
            { strIsPush = "true"; }
            
            string a = string.Empty;
            string b = string.Empty;
            string c = string.Empty;
            string d = string.Empty;
            //标准表
            if (stStd == "CL3115")
            {
                a = "0";
            }
            else if (stStd == "CL311V2")
            {
                a = "2";
            }
            else
            {
                a = "2";
            }
            //功率源
            if (stPower == "CL309")
            {
                b = "0";
            }
            else if (stPower == "CL303")
            {
                b = "1";
            }
            else
            {
                b = "1";
            }
            //时基源
            c = "0";
            //误差板
            if (stError == "188L")
            {
                d = "0";
            }
            else
            {
                d = "1";
            }
        if (strIsPush=="true")
            {
                Frontier.MeterVerification.KLDevice.GlobalUnit.isPushBox = 0;
            }
            else
            { Frontier.MeterVerification.KLDevice.GlobalUnit.isPushBox = 1; }
            Frontier.MeterVerification.KLDevice.GlobalUnit.DriverTypes = a + b + c + d;
            #endregion

            #endregion
        }

        private  void CreatNewIni()
        {
            INIHelper iniExist = new INIHelper("DeviceConfig.ini");
            if (iniExist.ExistINIFile())
            {
                return;
            }
            else
            {
                string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\DeviceConfig.ini";
                System.IO.StreamWriter swNew = new System.IO.StreamWriter(filePath);
                swNew.Write("装置配置\r\n");
                swNew.Write("[Main]\r\n");
                swNew.Write("Devices=CLPower,CLErrorPlate,CLDlt645_2007,CLStdMeter,CLTimeSync,CLReversalElectromotor,CLResistance,CLPowerSupply,CLCurrentControl,CLLightAndControlPower \r\n");
                swNew.Write("CLPower=Frontier.MeterVerification.KLDevice.CLPower,Frontier.MeterVerification.KLDevice \r\n");
                swNew.Write("CLErrorPlate=Frontier.MeterVerification.KLDevice.CLErrorPlate,Frontier.MeterVerification.KLDevice \r\n");
                swNew.Write("CLDlt645_2007=Frontier.MeterVerification.KLDevice.CLDlt645_2007,Frontier.MeterVerification.KLDevice \r\n");
                swNew.Write("CLStdMeter=Frontier.MeterVerification.KLDevice.CLStdMeter,Frontier.MeterVerification.KLDevice \r\n");
                swNew.Write("CLTimeSync=Frontier.MeterVerification.KLDevice.CLTimeSync,Frontier.MeterVerification.KLDevice \r\n");
                swNew.Write("CLResistance=Frontier.MeterVerification.KLDevice.CLResistance,Frontier.MeterVerification.KLDevice \r\n");
                swNew.Write("CLReversalElectromotor=Frontier.MeterVerification.KLDevice.CLReversalElectromotor,Frontier.MeterVerification.KLDevice \r\n");
                swNew.Write("CLPowerSupply=Frontier.MeterVerification.KLDevice.CLPowerSupply,Frontier.MeterVerification.KLDevice \r\n");
                swNew.Write("CLCurrentControl=Frontier.MeterVerification.KLDevice.CLCurrentControl,Frontier.MeterVerification.KLDevice \r\n");
                swNew.Write("CLLightAndControlPower=Frontier.MeterVerification.KLDevice.CLLightAndControlPower,Frontier.MeterVerification.KLDevice \r\n");
                swNew.Write("CLPower_PortSetting=Socket,193.168.18.1,20000,10003|SerialPort,33,9600,N,8,1 \r\n");
                swNew.Write("CLStdMeter_PortSetting=Socket,193.168.18.1,20000,10003|SerialPort,30,9600,N,8,1 \r\n");
                swNew.Write("CLTimeSync_PortSetting=Socket,193.168.18.1,20000,10003|SerialPort,32,2400,N,8,1 \r\n");
                swNew.Write("CLDlt645_2007_PortSetting=Socket,193.168.18.1,20000,10003|SerialPort,1,2400,E,8,1|SerialPort,2,2400,E,8,1|SerialPort,3,2400,E,8,1|SerialPort,4,2400,E,8,1|SerialPort,5,2400,E,8,1|SerialPort,6,2400,E,8,1|SerialPort,7,2400,E,8,1|SerialPort,8,2400,E,8,1|SerialPort,9,2400,E,8,1|SerialPort,10,2400,E,8,1|SerialPort,11,2400,E,8,1|SerialPort,12,2400,E,8,1 \r\n");
                swNew.Write("CLErrorPlate_PortSetting=Socket,193.168.18.1,20000,10003|SerialPort,31,19200,N,8,1 \r\n");
                swNew.Write("CLPowerSupply_PortSetting=Socket,193.168.18.1,20000,10003|SerialPort,28,38400,N,8,1 \r\n");
                swNew.Write("CLReversalElectromotor_PortSetting=Socket,193.168.18.1,20000,10003|SerialPort,27,38400,N,8,1 \r\n");
                swNew.Write("CLCurrentControl_PortSetting=Socket,193.168.18.1,20000,10003|SerialPort,20,38400,N,8,1 \r\n");
                swNew.Write("CLLightAndControlPower_PortSetting=Socket,193.168.18.1,20000,10003|SerialPort,29,9600,N,8,1 \r\n");
                swNew.Write("CLResistance_PortSetting=Socket,193.168.18.1,20000,10003|SerialPort,17,38400,N,8,1 \r\n");
                swNew.Write("[EquipSet] \r\n");
                swNew.Write("comBox_Std=CL3115 \r\n");
                swNew.Write("comBox_Power=CL303 \r\n");
                swNew.Write("comBox_TimeBs=CL191B \r\n");
                swNew.Write("comBox_Error=CL188E \r\n");
                swNew.Write("cbx_Rs485=Close \r\n");
                swNew.Write("IsPushBox=False \r\n");
                swNew.Write("");
                swNew.Close();
                SetEquipParam();
            }
                //string filePath = AppDomain.CurrentDomain.BaseDirectory + @"confige\ErrorLog.txt";
                //if (!File.Exists(filePath))
        }

        /// 设备联机
        /// <summary>
        /// 设备联机
        /// </summary>
        public override void InitConnect(int meterPositionCount)
        {
            base.InitConnect(meterPositionCount);
            connects = DeviceManager.GetDevices<IConnect>();
            controlPressMotor = DeviceManager.GetDevice<IControlPressMotor>();
            controlReversalMotor = DeviceManager.GetDevice<IControlReversalMotor>();
            equipmentStatus = DeviceManager.GetDevice<IEquipmentStatus>();
            meterPositionStatus = DeviceManager.GetDevice<IMeterPositionStatus>();
            light = DeviceManager.GetDevice<ILight>();
            controlEquipmentPower = DeviceManager.GetDevice<IControlEquipmentPower>();
            resistance = DeviceManager.GetDevice<IResistance>();
            foreach (IConnect connect in connects)
            {
                connect.Connected(meterPositionCount);
            }
        }

        /// 检定初始化
        /// <summary>
        /// 检定初始化
        /// </summary>
        public override void Connect()
        {
            connects = DeviceManager.GetDevices<IConnect>();
            controlResistancePower = DeviceManager.GetDevice<IControlResistancePower>();
            controlResistanceMoto = DeviceManager.GetDevice<IControlResistanceMoto>();
            dlt645 = DeviceManager.GetDevice<IDlt645>();
            monitor = DeviceManager.GetDevice<IMonitor>();
            power = DeviceManager.GetDevice<IPower>();
            powerConsume = DeviceManager.GetDevice<IPowerConsume>();
            stdMeter = DeviceManager.GetDevice<IStdMeter>();
            temperature = DeviceManager.GetDevice<ITemperature>();
            timeChannel = DeviceManager.GetDevice<ITimeChannel>();
            wcfk = DeviceManager.GetDevice<IWcfk>();
            powerSupply = DeviceManager.GetDevice<IPowerSupply>();
            currentControl = DeviceManager.GetDevice<ICurrentControl>();
            calcTime = DeviceManager.GetDevice<ICalcTime>();
            resistanceWcfk = DeviceManager.GetDevice<IResistanceWcfk>();
            meter = MeterPositions.First(item => item.Meter != null && item.IsVerify).Meter;
            foreach (IConnect connect in connects)
            {
                connect.Connected(MeterPositions);
            }

            // 获取挂表表位
            lstMeterIndex.Clear();
            foreach (var item in MeterPositions)
            {
                if (item.IsVerify)
                {
                    lstMeterIndex.Add(item.MeterIndex);
                }
            }

            //设置接线方式  IPower
            this.OnInfo("正在设置接线方式...");
            power.SetWiringMode(meter.WiringMode, Pulse.正向有功);
            Thread.Sleep(500);
        }
        /// <summary>
        /// 设置设备通讯端口
        /// </summary>
        public void SetEquipParam()
        {
            SetEquipParamDlg dg = new SetEquipParamDlg();
            dg.ShowDialog();
        }

        /// 电能表时间
        /// <summary>
        /// 电能表时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="times">返回电能表时间</param>
        /// <returns></returns>
        public override bool GetMeterTime(float U, float acFreq, out string[] times)
        {
            times = new string[base.MeterPositions.Length];
            for (int i = 0; i < times.Length; i++)
            {
                times[i] = DateTime.Now.ToString();
            }
            return true;
        }

        #region 已实现的校验项
        ///基本误差试验，合元
        /// <summary>
        /// 基本误差试验，合元
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="I">电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="factor">功率因数</param>
        /// <param name="capacitive">容性标志，true表示容性，false表示感性</param>
        /// <param name="pulse">脉冲类型，正向有功、反向有功、正向无功、反向无功</param>
        /// <param name="circle">检验圈数</param>
        /// <param name="count">检验次数</param>
        /// <param name="returnSampleDatas">误差结果的回调句柄。根据检验次数，每产生一次误差值，通过该回调句柄上上层系统返回误差值</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        public override bool ExecuteBasicError(float U, float I, float acFreq, float factor, bool capacitive, Pulse pulse, int circle, int count, ReturnSampleDatasDelegate returnSampleDatas)
        {
            bool readLineOver = false;                              //读取当前点结束。
            bool readErrOver = false;                               //校验点执行完成
            int[] lastErrNO = new int[MeterPositions.Length];       //上次误差次数。
            int[] sampleNo = new int[MeterPositions.Length];        //采样次数记录 
            string[] errValue = new string[MeterPositions.Length];  //采样误差数据
            for (int o = 0; o < MeterPositions.Length; o++)
            {
                lastErrNO[o] = 0;
                sampleNo[o] = 0;
                errValue[o] = string.Empty;
            }
            float timeOut;
            calcTime.CalcBasicErrorTime(U, I, factor, capacitive, acFreq, pulse.ToString(), circle, count, out timeOut); //超时时间
            this.IsStop = false;

            #region 1.初始化系统通讯基本模块
            ////1.1 设置接线方式  IPower
            //this.OnInfo("正在设置接线方式...");////源怎么发得接线方式，不理解。
            //power.SetWiringMode(meter.WiringMode, pulse);
            ////Thread.Sleep(500);


            ////1.2 发送电压电流量程信息    IPower
            //this.OnInfo("正在设置电压电流量程...");
            //power.SetRange(U, I);
            //Thread.Sleep(500);

            ////1.3 设置相位信息 IPower
            //this.OnInfo("正在设置相位信息...");
            //power.SetLoadPhase(meter.WiringMode, factor, capacitive, pulse, LoadPhase.None);
            //Thread.Sleep(100);
            #endregion

            #region 2.校验参数设置
            //2.1 初始化误差板状态 IWcfk
            this.OnInfo("正在初始化误差板...");
            wcfk.InitWcfk();

            //2.2 获取检定类型 IWcfk
            this.OnInfo("正在设置检定类型...");
            wcfk.SetVerificationType(VerificationElementType.基本误差试验);

            //2.3 设置检定脉冲方式 IPower
            this.OnInfo("正在设置检定脉冲方式...");
            power.SetVerificationPulseType(VerificationElementType.基本误差试验);
            //Thread.Sleep(500);
            if (IsStop) return false;
            //2.4 设置脉冲方向 IWcfk
            this.OnInfo("正在设置设置脉冲方向...");
            wcfk.SetPulseType(pulse);

            //2.5 设置脉冲通道 IWcfk
            this.OnInfo("正在设置脉冲通道...");
            wcfk.SetPulseChannel(VerificationElementType.基本误差试验, pulse);
            //Thread.Sleep(100);
            if (IsStop) return false;
            //2.6 设置检定圈数 IWcfk
            this.OnInfo("正在设置检定圈数...");
            wcfk.SetCircle(circle);
            if (IsStop) return false;
            //2.7 设置时钟通道
            this.OnInfo("正在设置时钟通道...");
            timeChannel.SetTimeChannel(1);
            if (IsStop) return false;
            //2.8 设置被检表常数 IWcfk
            this.OnInfo("正在设置被检表常数...");

            wcfk.SetMeterConst(pulse == Pulse.反向无功 || pulse == Pulse.正向无功 ? meter.Rp_Const : meter.Const, U, I);

            if (IsStop) return false;
             

            //2.10 设置标准表接线方式
            this.OnInfo("正在设置标准表接线方式...");
            stdMeter.SetStdMeterWiringMode(VerificationElementType.基本误差试验, meter.WiringMode, pulse);
            //找不到311设置方式。。。。。

            //2.12 切换CT档位
            //this.OnInfo("切换CT档位...");
            //currentControl.SetCurrentControl(I);
            //Thread.Sleep(500);

            //2.13 切换多功能控制板 
            //this.OnInfo("切换多功能控制板...");
            //powerSupply.SetPowerSupplyType(VerificationElementType.基本误差试验);
            //Thread.Sleep(500);

            //2.14 发送标准表常数
            this.OnInfo("发送标准表常数...");
            stdMeter.SetStdMeterConst(U, I, meter.WiringMode);

            //2.15 升电压、电流
            this.OnInfo("正在升电压、电流信息...");
            power.RaiseVotageCurrent(U, I, LoadPhase.None, meter.WiringMode, factor, capacitive, pulse);
            ReadStdParam();



            //2.16 延时10秒
            this.OnInfo("延时10秒...");
            Thread.Sleep(SteadyTime * 1000);
            ////增加校验如果检定台电流没有升起，则重新开始设置
            ////增加校验如果检定台电流大于电能表额定最大电流，则进行降电流操作
            #endregion

            #region 3.开始检定并读取误差

            //2.9 启动误差版
            this.OnInfo("正在启动误差版...");
            wcfk.StartWcfk();

            //3.1、 开始检定
            this.OnInfo("开始检定...");
            wcfk.StartVerification(VerificationElementType.基本误差试验);

            //3.2、 读取误差
            this.OnInfo("读取误差...");
            int sampleIndex = 1;
            int startTime = Environment.TickCount;
            while (true)
            {
                if (IsStop)
                {
                    break;
                }
                Thread.Sleep(1000);
                if (sampleIndex <= count)                        //说明该校验点检定还未完成
                {
                    for (int i = 0; i < MeterPositions.Length; i++)  //表位数
                    {
                        if ((MeterPositions[i].IsVerify) && (!IsStop) && string.IsNullOrEmpty(errValue[i]))
                        {
                            OnInfo("正在读取第" + (i + 1) + "表位误差……");
                            ErrorData errorData = wcfk.ReadErrorData(i + 1, sampleIndex);
                            if (errorData != null
                                && lastErrNO[i] != errorData.SampleIndex //sampleIndex//
                                && !string.IsNullOrEmpty(errorData.ErrorValue)
                                && Convert.ToSingle(errorData.ErrorValue) >-1f
                                && Convert.ToSingle(errorData.ErrorValue) < 1f)
                            {
                                sampleNo[i]++;
                                lastErrNO[i] = errorData.SampleIndex;//sampleIndex;//
                                errValue[i] = errorData.ErrorValue;
                            }
                        }
                    }
                }
                //判断是否所有表位都读取成功，必须要将所有选中的表位读取成功，才能回传。
                readLineOver = true;
                if (((Environment.TickCount - startTime) / 1000 + 1) < timeOut + (sampleIndex == 1 ? 30 : 0))
                {
                    for (int i = 0; i < MeterPositions.Length; i++)
                    {
                        if (string.IsNullOrEmpty(errValue[i])
                            && MeterPositions[i].IsVerify)
                        {
                            readLineOver = false;
                            break;
                        }
                    }
                }
                //如果所有表位都读取到数据则进行返回，这儿也可以不用判断，每次采样进行多次返回，这样在界面上可以看到那些表位没有获取到数据
                if (readLineOver)
                {
                    returnSampleDatas(sampleIndex, errValue);
                    errValue = new string[MeterPositions.Length];//发送结束后，在进行重新分配。
                    sampleIndex++;
                    startTime = Environment.TickCount;
                }

                if (sampleIndex > count)                        //表明检定已经完成，返回
                {
                    readErrOver = true;
                    break;
                }
            }
            #endregion

            #region 4.停止检定设置
            //4.1 停止误差版
            this.OnInfo("停止误差版...");
            wcfk.StopWcfk();
            if (IsStop) return false;
            //4.2 停止标准表
            this.OnInfo("停止标准表...");
            stdMeter.StopStdMeter();

            //4.3 设置接线方式  IPower
            this.OnInfo("正在设置接线方式(正向有功)...");
            power.SetWiringMode(meter.WiringMode, Pulse.正向有功);
            //Thread.Sleep(500);

            //4.4 停止检定 Sd2000
            //OnInfo("停止检定...");
            //power.StopVerification();
            #endregion
            
            return readErrOver;
        }


        ///基本误差试验，合元 带误差限，自动判定结论
        /// <summary>
        /// 基本误差试验，合元
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="I">电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="factor">功率因数</param>
        /// <param name="capacitive">容性标志，true表示容性，false表示感性</param>
        /// <param name="pulse">脉冲类型，正向有功、反向有功、正向无功、反向无功</param>
        /// <param name="circle">检验圈数</param>
        /// <param name="count">检验次数</param>
        /// <param name="returnSampleDatas">误差结果的回调句柄。根据检验次数，每产生一次误差值，通过该回调句柄上上层系统返回误差值</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        public override bool ExecuteBasicError(float U, float I, float acFreq, float factor, bool capacitive, Pulse pulse, int meterConst, int circle, int count, float fMax, float fMin, ReturnSampleDataErrDelegate returnSampleDatas)
        {
            bool readLineOver = false;                              //读取当前点结束。
            bool readErrOver = false;                               //校验点执行完成
            int[] lastErrNO = new int[MeterPositions.Length];       //上次误差次数。
            int[] sampleNo = new int[MeterPositions.Length];        //采样次数记录 
            int [] iMeterReSult = new int[MeterPositions.Length];   //标识表位结论。0未检定、1合格、2不合格
            int[] upErrNum = new int[MeterPositions.Length];        //超差次数
            string[,] errValue = new string[MeterPositions.Length,count];  //采样误差数据
            int[] PassCount = new int[MeterPositions.Length];//如果误差过大可以忽略的次数
            for (int o = 0; o < MeterPositions.Length; o++)
            {
                lastErrNO[o] = 0;
                sampleNo[o] = 0;
                upErrNum[o] = 0;
                iMeterReSult[o] = 0;
                PassCount[o] =12;
                //errValue[o] = string.Empty;
            }
            for (int j= 0;j< MeterPositions.Length;j++)
            {
                for(int k = 0;k<count;k++)
                {
                    errValue[j,k] = string.Empty;
                }
            }

            float timeOut;
            calcTime.CalcBasicErrorTime(U, I, factor, capacitive, acFreq, pulse.ToString(), circle, count, out timeOut); //超时时间
            this.IsStop = false;

            

            #region 2.校验参数设置
            //2.1 初始化误差板状态 IWcfk
            this.OnInfo("正在初始化误差板...");
            wcfk.InitWcfk();
            if (IsStop) return false;
            //2.2 获取检定类型 IWcfk
            this.OnInfo("正在设置检定类型...");
            wcfk.SetVerificationType(VerificationElementType.基本误差试验);
            if (IsStop) return false;
             
            //2.4 设置脉冲方向 IWcfk
            this.OnInfo("正在设置设置脉冲方向...");
            wcfk.SetPulseType(pulse);
            if (IsStop) return false;
            //2.5 设置脉冲通道 IWcfk
            this.OnInfo("正在设置脉冲通道...");
            wcfk.SetPulseChannel(VerificationElementType.基本误差试验, pulse);
            
            if (IsStop) return false;
            //2.6 设置检定圈数 IWcfk
            this.OnInfo("正在设置检定圈数...");
            wcfk.SetCircle(circle);
           
            if (IsStop) return false;
            //2.7 设置时钟通道
            this.OnInfo("正在设置时钟通道...");
            timeChannel.SetTimeChannel(1);   
            if (IsStop) return false;
            //2.8 设置被检表常数 IWcfk
            this.OnInfo("正在设置被检表常数...");             
            wcfk.SetMeterConst(meterConst, U, I);           

            //2.10 设置标准表接线方式
            this.OnInfo("正在设置标准表接线方式...");
            stdMeter.SetStdMeterWiringMode(VerificationElementType.基本误差试验, meter.WiringMode, pulse);
             
            //2.14 发送标准表常数
            this.OnInfo("发送标准表常数...");
            stdMeter.SetStdMeterConst(U, I, meter.WiringMode);
            
            //2.15 升电压、电流
            this.OnInfo("正在升电压、电流信息...");
            power.RaiseVotageCurrent(U, I, LoadPhase.None, meter.WiringMode, factor, capacitive, pulse);

            //2.16 延时10秒
            this.OnInfo("延时10秒...");
            Thread.Sleep(SteadyTime * 1000+4000);
            if (IsStop) return false;
            //2.9 启动误差版
            this.OnInfo("正在启动误差版...");
            wcfk.StartWcfk();
            ////增加校验如果检定台电流没有升起，则重新开始设置
            ////增加校验如果检定台电流大于电能表额定最大电流，则进行降电流操作
            #endregion
            if (IsStop) return false;
            #region 3.开始检定并读取误差
            //3.1、 开始检定
            this.OnInfo("开始检定...");
            wcfk.StartVerification(VerificationElementType.基本误差试验);
            if (IsStop) return false;
            //3.2、 读取误差
            this.OnInfo("读取误差...");
            int sampleIndex = 1;
            int startTime = Environment.TickCount;
            while (true)
            {
                if (IsStop)
                {
                    break;
                }
                Thread.Sleep(1000);
                if (sampleIndex <= count)                        //说明该校验点检定还未完成
                {
                    for (int i = 0; i < MeterPositions.Length; i++)  //表位数
                    {
                        if ((MeterPositions[i].IsVerify) && (!IsStop) && string.IsNullOrEmpty(errValue[i,sampleIndex -1]))
                        {
                            OnInfo("正在读取第" + (i + 1) + "表位误差……");
                            ErrorData errorData = new ErrorData();
                            while (true)
                            {
                                errorData = wcfk.ReadErrorData(i + 1, sampleIndex);

                                if (Convert.ToSingle(errorData.ErrorValue) > fMin
                                && Convert.ToSingle(errorData.ErrorValue) < fMax)
                                {
                                    break;
                                }
                                else
                                {
                                    PassCount[i] -= 1;
                                    if (PassCount[i] <= 0)
                                    {
                                        break;
                                    }
                                    System.Threading.Thread.Sleep(1000);
                                    if (IsStop)
                                    {
                                        break;
                                    }
                                }
                            }
                            if (errorData != null
                                && lastErrNO[i] != errorData.SampleIndex //sampleIndex//
                                && !string.IsNullOrEmpty(errorData.ErrorValue)
                                && Convert.ToSingle(errorData.ErrorValue) > -10f
                                && Convert.ToSingle(errorData.ErrorValue) < 10f)//增加判定误差过大不计入误差检测数据内
                                
                            {
                                if (Convert.ToSingle(errorData.ErrorValue) > fMin
                                && Convert.ToSingle(errorData.ErrorValue) < fMax)
                                {
                                    sampleNo[i]++;
                                    lastErrNO[i] = errorData.SampleIndex;//sampleIndex;//
                                    iMeterReSult[i] = 1;
                                    errValue[i, sampleIndex - 1] = errorData.ErrorValue;
                                }
                                else if (upErrNum[i] > 0)
                                {//若超差次数超过三个就认为该表位误差个数不合格
                                    sampleNo[i]++;
                                    lastErrNO[i] = errorData.SampleIndex;//sampleIndex;//
                                    iMeterReSult[i] = 2;
                                    errValue[i, sampleIndex - 1] = errorData.ErrorValue;
                                }
                                else
                                {
                                    errValue[i, sampleIndex - 1] = errorData.ErrorValue;
                                    upErrNum[i]++;
                                }
                            }
                        }
                    }
                }
                //判断是否所有表位都读取成功，必须要将所有选中的表位读取成功，才能回传。
                readLineOver = true;
                if (((Environment.TickCount - startTime) / 1000 + 1) < timeOut + 10 + (sampleIndex == 1 ? 20 : 50))
                {
                    for (int i = 0; i < MeterPositions.Length; i++)
                    {
                        if (string.IsNullOrEmpty(errValue[i,sampleIndex -1])
                            && MeterPositions[i].IsVerify)
                        {
                            readLineOver = false;
                            break;
                        }
                    }
                   
                }
                if (readLineOver)
                {
                    sampleIndex++;
                }
                //如果所有表位都读取到数据则进行返回，这儿也可以不用判断，每次采样进行多次返回，这样在界面上可以看到那些表位没有获取到数据
                if (sampleIndex > count)
                {
                    for (int temp = 0; temp < upErrNum.Length; temp++)
                    {
                        if (upErrNum[temp] > 0)
                        {
                            iMeterReSult[temp] = 2;
                        }
                    }
                    returnSampleDatas(errValue, iMeterReSult);
                    errValue = new string[MeterPositions.Length,count];//发送结束后，在进行重新分配。
                    startTime = Environment.TickCount;
                }

                if (sampleIndex > count)                        //表明检定已经完成，返回
                {
                    readErrOver = true;
                    break;
                }
            }
            #endregion

            #region 4.停止检定设置
            //4.1 停止误差版
            this.OnInfo("停止误差版...");
            wcfk.StopWcfk();

            //4.2 停止标准表
            this.OnInfo("停止标准表...");
            stdMeter.StopStdMeter();

            //4.3 设置接线方式  IPower
            //this.OnInfo("正在设置接线方式(正向有功)...");
            //power.SetWiringMode(meter.WiringMode, Pulse.正向有功);
            //Thread.Sleep(500);

            //4.4 停止检定 Sd2000
            //OnInfo("停止检定...");
            //power.StopVerification();
            #endregion

            return !this.IsStop;
        }


        /// 基本误差，分元
        /// <summary>
        /// 基本误差，分元
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="I">电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="phase">相别，"A"：A(B)相；"B"：B相；"C"：C(B)相。注意三相三线表不作B相试验</param>
        /// <param name="factor">功率因数</param>
        /// <param name="capacitive">容性标志，true表示容性，false表示感性</param>
        /// <param name="pulse">脉冲类型，正向有功、反向有功、正向无功、反向无功</param>
        /// <param name="circle">检验圈数</param>
        /// <param name="count">检验次数</param>
        /// <param name="returnSampleDatas">误差结果的回调句柄。根据检验次数，每产生一次误差值，通过该回调句柄上上层系统返回误差值</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        public override bool ExecuteBasicError(float U, float I, float acFreq, string phase, float factor, bool capacitive, Pulse pulse, int circle, int count, ReturnSampleDatasDelegate returnSampleDatas)
        {
            bool readLineOver = false;                              //读取当前点结束。
            bool readErrOver = false;                               //校验点执行完成
            int[] lastErrNO = new int[MeterPositions.Length];       //上次误差次数。
            int[] sampleNo = new int[MeterPositions.Length];        //采样次数记录 
            string[] errValue = new string[MeterPositions.Length];  //采样误差数据
            float timeOut;
            calcTime.CalcBasicErrorTime(U, I, acFreq, phase, factor, capacitive, pulse, circle, count, out timeOut);//超时时间
            this.IsStop = false;

            #region 1.初始化系统通讯基本模块
            //1.1 设置接线方式  IPower
            //this.OnInfo("正在设置接线方式...");
            //power.SetWiringMode(meter.WiringMode, pulse);
            //Thread.Sleep(500);

            //1.2 发送电压电流量程信息    IPower
            //this.OnInfo("正在设置电压电流量程...");
            //power.SetRange(U, I);
            //Thread.Sleep(500);

            //1.3 设置相位信息 IPower
            //this.OnInfo("正在设置相位信息...");
            //power.SetLoadPhase(meter.WiringMode, factor, capacitive, pulse, (LoadPhase)Enum.Parse(typeof(LoadPhase), phase));
            //Thread.Sleep(500);
            #endregion

            #region 2.校验参数设置
            //2.1 初始化误差板状态 IWcfk
            this.OnInfo("正在初始化误差板...");
            wcfk.InitWcfk();

            //2.2 设置检定类型 IWcfk
            this.OnInfo("正在设置检定类型...");
            wcfk.SetVerificationType(VerificationElementType.基本误差试验);

            //2.3 设置检定脉冲方式 IPower
            this.OnInfo("正在设置检定脉冲方式...");
            power.SetVerificationPulseType(VerificationElementType.基本误差试验);
            Thread.Sleep(500);

            //2.4 设置脉冲方向 IWcfk
            this.OnInfo("正在设置设置脉冲方向...");
            wcfk.SetPulseType(pulse);

            //2.5 设置脉冲通道 IWcfk
            this.OnInfo("正在设置脉冲通道...");
            wcfk.SetPulseChannel(VerificationElementType.基本误差试验, pulse);
            Thread.Sleep(500);

            //2.6 设置检定圈数 IWcfk
            this.OnInfo("正在设置检定圈数...");
            wcfk.SetCircle(circle);

            //2.7 设置时钟通道
            this.OnInfo("正在设置时钟通道...");
            timeChannel.SetTimeChannel(1);

            //2.8 设置被检表常数 IWcfk
            this.OnInfo("正在设置被检表常数...");
            wcfk.SetMeterConst(pulse == Pulse.反向无功 || pulse == Pulse.正向无功 ? meter.Rp_Const : meter.Const,U,I);

            //2.9 启动误差版
            this.OnInfo("正在启动误差版...");
            wcfk.StartWcfk();

            //2.10 设置标准表接线方式
            this.OnInfo("正在设置标准表接线方式...");
            stdMeter.SetStdMeterWiringMode(VerificationElementType.基本误差试验, meter.WiringMode, pulse);

            //2.12 切换CT档位
            this.OnInfo("切换CT档位...");
            currentControl.SetCurrentControl(I);
            Thread.Sleep(500);

            //2.13 切换多功能控制板 
            this.OnInfo("切换多功能控制板...");
            powerSupply.SetPowerSupplyType(VerificationElementType.基本误差试验);
            Thread.Sleep(500);

            //2.14 发送标准表常数
            this.OnInfo("发送标准表常数...");
            stdMeter.SetStdMeterConst(U, I, meter.WiringMode);

            //2.15 升电压、电流
            this.OnInfo("正在升电压、电流信息...");
            power.RaiseVotageCurrent(U, I, (LoadPhase)Enum.Parse(typeof(LoadPhase), phase), meter.WiringMode, factor, capacitive, pulse);

            //2.16 延时10秒
            this.OnInfo("延时10秒...");
            Thread.Sleep(SteadyTime * 1000);
            ////增加校验如果检定台电流没有升起，则重新开始设置
            ////增加校验如果检定台电流大于电能表额定最大电流，则进行降电流操作
            #endregion

            #region 3.开始检定并读取误差
            //3.1、 开始检定
            this.OnInfo("开始检定...");
            wcfk.StartVerification(VerificationElementType.基本误差试验);

            //3.2、 读取误差
            this.OnInfo("读取误差...");
            int sampleIndex = 1;
            int startTime = Environment.TickCount;
            while (true)
            {
                if (IsStop)
                {
                    break;
                }
                Thread.Sleep(1000);
                if (sampleIndex <= count)                        //说明该校验点检定还未完成
                {
                    for (int i = 0; i < MeterPositions.Length; i++)
                    {
                        if ((MeterPositions[i].IsVerify) && (!IsStop) && string.IsNullOrEmpty(errValue[i]))
                        {
                            OnInfo("正在读取第" + (i + 1) + "表位误差……");
                            ErrorData errorData = wcfk.ReadErrorData(i + 1, sampleIndex);
                            if (errorData != null && lastErrNO[i] != errorData.SampleIndex && !string.IsNullOrEmpty(errorData.ErrorValue))
                            {
                                sampleNo[i]++;
                                lastErrNO[i] = errorData.SampleIndex;
                                errValue[i] = errorData.ErrorValue;
                            }
                        }
                    }
                }
                //判断是否所有表位都读取成功，必须要将所有选中的表位读取成功，才能回传。
                readLineOver = true;
                if (((Environment.TickCount - startTime) / 1000 + 1) < timeOut + (sampleIndex == 1 ? 30 : 0))
                {
                    for (int i = 0; i < MeterPositions.Length; i++)
                    {
                        if (string.IsNullOrEmpty(errValue[i])
                            && MeterPositions[i].IsVerify)
                        {
                            readLineOver = false;
                            break;
                        }
                    }
                }
                //如果所有表位都读取到数据则进行返回，这儿也可以不用判断，每次采样进行多次返回，这样在界面上可以看到那些表位没有获取到数据
                if (readLineOver)
                {
                    returnSampleDatas(sampleIndex, errValue);
                    errValue = new string[MeterPositions.Length];//发送结束后，在进行重新分配。
                    sampleIndex++;
                    startTime = Environment.TickCount;
                }

                if (sampleIndex > count)                        //表明检定已经完成，返回
                {
                    readErrOver = true;
                    break;
                }
            }
            #endregion

            #region 4.停止检定设置
            //4.1 停止误差版
            this.OnInfo("停止误差版...");
            wcfk.StopWcfk();

            //4.2 停止标准表
            this.OnInfo("停止标准表...");
            stdMeter.StopStdMeter();

            //4.3 设置接线方式  IPower
            this.OnInfo("正在设置接线方式(正向有功)...");
            power.SetWiringMode(meter.WiringMode, Pulse.正向有功);
            Thread.Sleep(500);

            //4.4 停止检定 Sd2000
            OnInfo("停止检定...");
            power.StopVerification();
            #endregion

            return readErrOver;
        }
        ///基本误差，分元 带误差限，自动判定结论
        /// <summary>
        /// 基本误差，分元
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="I">电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="phase">相别，"A"：A(B)相；"B"：B相；"C"：C(B)相。注意三相三线表不作B相试验</param>
        /// <param name="factor">功率因数</param>
        /// <param name="capacitive">容性标志，true表示容性，false表示感性</param>
        /// <param name="pulse">脉冲类型，正向有功、反向有功、正向无功、反向无功</param>
        /// <param name="circle">检验圈数</param>
        /// <param name="count">检验次数</param>
        /// <param name="fMax">误差上限</param>
        /// <param name="fMin">误差下限</param>
        /// <param name="returnSampleDatas">返回数据</param>
        /// <returns></returns>
        public override bool ExecuteBasicError(float U, float I, float acFreq, string phase, float factor, bool capacitive, Pulse pulse, int meterConst, int circle, int count, float fMax, float fMin, ReturnSampleDataErrDelegate returnSampleDatas)
        {
            bool readLineOver = false;                              //读取当前点结束。
            bool readErrOver = false;                               //校验点执行完成
            int[] lastErrNO = new int[MeterPositions.Length];       //上次误差次数。
            int[] sampleNo = new int[MeterPositions.Length];
             int[] iMeterReSult = new int[MeterPositions.Length];   //标识表位结论。0未检定、1合格、2不合格
            int[] upErrNum = new int[MeterPositions.Length];        //超差次数
            string[,] errValue = new string[MeterPositions.Length, count];  //采样误差数据
            int[] PassCount = new int[MeterPositions.Length];//如果误差过大可以忽略的次数
            for (int o = 0; o < MeterPositions.Length; o++)
            {
                lastErrNO[o] = 0;
                sampleNo[o] = 0;
                upErrNum[o] = 0;
                iMeterReSult[o] = 0;
                PassCount[o] = 12;
                //errValue[o] = string.Empty;
            }            
             
            for (int j = 0; j < MeterPositions.Length; j++)
            {
                for (int k = 0; k < count; k++)
                {
                    errValue[j, k] = string.Empty;
                }
            }
            float timeOut;
            calcTime.CalcBasicErrorTime(U, I, acFreq, phase, factor, capacitive, pulse, circle, count, out timeOut);//超时时间
            this.IsStop = false;

            #region 2.校验参数设置
            //2.1 初始化误差板状态 IWcfk
            this.OnInfo("正在初始化误差板...");
            wcfk.InitWcfk();
            if (IsStop) return false;
            //2.2 获取检定类型 IWcfk
            this.OnInfo("正在设置检定类型...");
            wcfk.SetVerificationType(VerificationElementType.基本误差试验);
            if (IsStop) return false;

            //2.4 设置脉冲方向 IWcfk
            this.OnInfo("正在设置设置脉冲方向...");
            wcfk.SetPulseType(pulse);
            if (IsStop) return false;
            //2.5 设置脉冲通道 IWcfk
            this.OnInfo("正在设置脉冲通道...");
            wcfk.SetPulseChannel(VerificationElementType.基本误差试验, pulse);

            if (IsStop) return false;
            //2.6 设置检定圈数 IWcfk
            this.OnInfo("正在设置检定圈数...");
            wcfk.SetCircle(circle);

            if (IsStop) return false;
            //2.7 设置时钟通道
            this.OnInfo("正在设置时钟通道...");
            timeChannel.SetTimeChannel(1);
            if (IsStop) return false;
            //2.8 设置被检表常数 IWcfk
            this.OnInfo("正在设置被检表常数...");
            wcfk.SetMeterConst(meterConst, U, I);

            //2.10 设置标准表接线方式
            this.OnInfo("正在设置标准表接线方式...");
            stdMeter.SetStdMeterWiringMode(VerificationElementType.基本误差试验, meter.WiringMode, pulse);

            //2.14 发送标准表常数
            this.OnInfo("发送标准表常数...");
            stdMeter.SetStdMeterConst(U, I, meter.WiringMode);
            #endregion
            #region 2.校验参数设置
            //    //2.1 初始化误差板状态 IWcfk
        //this.OnInfo("正在初始化误差板...");
        //    wcfk.InitWcfk();
        //    if (IsStop) return false;
        //    //2.2 设置检定类型 IWcfk
        //    this.OnInfo("正在设置检定类型...");
        //    wcfk.SetVerificationType(VerificationElementType.基本误差试验);
        //    if (IsStop) return false;
             
        //    this.OnInfo("正在设置设置脉冲方向...");
        //    wcfk.SetPulseType(pulse);
        //    //Thread.Sleep(500);
        //    if (IsStop) return false;
        //    //2.5 设置脉冲通道 IWcfk
        //    this.OnInfo("正在设置脉冲通道...");
        //    wcfk.SetPulseChannel(VerificationElementType.基本误差试验, pulse);
        //    //Thread.Sleep(500);
        //    if (IsStop) return false;
        //    //2.6 设置检定圈数 IWcfk
        //    this.OnInfo("正在设置检定圈数...");
        //    wcfk.SetCircle(circle);
        //    //Thread.Sleep(400);
        //    if (IsStop) return false;
        //    //2.7 设置时钟通道
        //    this.OnInfo("正在设置时钟通道...");
        //    timeChannel.SetTimeChannel(1);
        //    //Thread.Sleep(100);
        //    if (IsStop) return false;
        //    //2.8 设置被检表常数 IWcfk
        //    this.OnInfo("正在设置被检表常数...");
        //    wcfk.SetMeterConst(pulse == Pulse.反向无功 || pulse == Pulse.正向无功 ? meter.Rp_Const : meter.Const,U,I);
        //    //Thread.Sleep(200);
        //    if (IsStop) return false;

        //    //2.10 设置标准表接线方式
        //    this.OnInfo("正在设置标准表接线方式...");
        //    stdMeter.SetStdMeterWiringMode(VerificationElementType.基本误差试验, meter.WiringMode, pulse);
        //    if (IsStop) return false;
            
        //    if (IsStop) return false;
             

        //    //2.14 发送标准表常数
        //    this.OnInfo("发送标准表常数...");
        //    stdMeter.SetStdMeterConst(U, I, meter.WiringMode);
            #endregion
            if (IsStop) return false;
            
            //2.15 升电压、电流
            this.OnInfo("正在升电压、电流信息...");
            power.RaiseVotageCurrent(U, I, (LoadPhase)Enum.Parse(typeof(LoadPhase), phase), meter.WiringMode, factor, capacitive, pulse);
            if (IsStop) return false;
            //2.16 延时10秒
            this.OnInfo("延时10秒...");
            Thread.Sleep(SteadyTime * 1000 + 4000);
            ////增加校验如果检定台电流没有升起，则重新开始设置
            ////增加校验如果检定台电流大于电能表额定最大电流，则进行降电流操作
          

            #region 3.开始检定并读取误差


            //2.9 启动误差版
            this.OnInfo("正在启动误差版...");
            wcfk.StartWcfk();

            if (IsStop) return false;
            //3.1、 开始检定
            this.OnInfo("开始检定...");
            wcfk.StartVerification(VerificationElementType.基本误差试验);
            if (IsStop) return false;
            //3.2、 读取误差
            this.OnInfo("读取误差...");
            int sampleIndex = 1;
            int startTime = Environment.TickCount;
            while (true)
            {
                if (IsStop)
                {
                    break;
                }
                Thread.Sleep(100);
                if (sampleIndex <= count)                        //说明该校验点检定还未完成
                {
                    #region 循环读取各个表位误差值
                    for (int i = 0; i < MeterPositions.Length; i++)
                    {
                        if ((MeterPositions[i].IsVerify) && (!IsStop) && string.IsNullOrEmpty(errValue[i,sampleIndex -1]))
                        {
                            OnInfo("正在读取第" + (i + 1) + "表位误差……");
                            ErrorData errorData=new ErrorData ();
                            while(true )
                            {
                                errorData = wcfk.ReadErrorData(i + 1, sampleIndex);
                                
                                if (Convert.ToSingle(errorData.ErrorValue) > fMin
                                && Convert.ToSingle(errorData.ErrorValue) < fMax)
                                {
                                    break;
                                }
                                else
                                {
                                    PassCount[i] -= 1;
                                    if (PassCount[i] <= 0)
                                    {
                                        break;
                                    }
                                    System.Threading.Thread.Sleep(1500);
                                    if (IsStop)
                                    {
                                        break;
                                    }
                                }                  
                            }
                            if (errorData != null
                                && lastErrNO[i] != errorData.SampleIndex  //sampleIndex//
                                && !string.IsNullOrEmpty(errorData.ErrorValue)
                                && Convert.ToSingle(errorData.ErrorValue) > -10f
                                && Convert.ToSingle(errorData.ErrorValue) < 10f)//增加判定误差过大不计入误差检测数据内)
                            {
                                #region 正常数据处理
                                if (Convert.ToSingle(errorData.ErrorValue) > fMin
                                && Convert.ToSingle(errorData.ErrorValue) < fMax)
                                {
                                    sampleNo[i]++;
                                    lastErrNO[i] =  errorData.SampleIndex;//sampleIndex;
                                    iMeterReSult[i] = 1;
                                    errValue[i, sampleIndex - 1] = errorData.ErrorValue;
                                }
                                else if (upErrNum[i] > 0)//只要有一个不合格就不合格
                                {//若超差次数超过三个就认为该表位误差个数不合格
                                    sampleNo[i]++;
                                    lastErrNO[i] = errorData.SampleIndex; //sampleIndex;//
                                    iMeterReSult[i] = 2;
                                    errValue[i, sampleIndex - 1] = errorData.ErrorValue;
                                }
                                else
                                {
                                    upErrNum[i]++;
                                    errValue[i, sampleIndex - 1] = errorData.ErrorValue;
                                }
                                #endregion
                            }
                        }
                    }
                    #endregion 
                }
                //判断是否所有表位都读取成功，必须要将所有选中的表位读取成功，才能回传。
                readLineOver = true;
                if (((Environment.TickCount - startTime) / 1000 + 1) < timeOut + 10 + (sampleIndex == 1 ? 20 : 50))
                {
                    for (int i = 0; i < MeterPositions.Length; i++)
                    {
                        if (string.IsNullOrEmpty(errValue[i,sampleIndex -1])
                            && MeterPositions[i].IsVerify)
                        {
                            readLineOver = false;
                            break;
                        }
                    }
                }

                if (readLineOver)
                {
                    startTime = Environment.TickCount;
                    sampleIndex++;
                }
                //如果所有表位都读取到数据则进行返回，这儿也可以不用判断，每次采样进行多次返回，这样在界面上可以看到那些表位没有获取到数据
                if (sampleIndex > count)
                {
                    for (int temp = 0; temp < upErrNum.Length; temp++)
                    {
                        if (upErrNum[temp] > 0)
                        {
                            iMeterReSult[temp] = 2;
                        }
                    }
                    returnSampleDatas(errValue, iMeterReSult);
                    errValue = new string[MeterPositions.Length,count];//发送结束后，在进行重新分配。
                    startTime = Environment.TickCount;
                }

                if (sampleIndex > count)                        //表明检定已经完成，返回
                {
                    readErrOver = true;
                    break;
                }
            }
            #endregion

            #region 4.停止检定设置
            //4.1 停止误差版
            this.OnInfo("停止误差版...");
            wcfk.StopWcfk();

            //4.2 停止标准表
            this.OnInfo("停止标准表...");
            stdMeter.StopStdMeter();

            //4.3 设置接线方式  IPower
            //this.OnInfo("正在设置接线方式(正向有功)...");
            //power.SetWiringMode(meter.WiringMode, Pulse.正向有功);
            //Thread.Sleep(500);

            //4.4 停止检定 Sd2000
            //OnInfo("停止检定...");
            //power.StopVerification();
            #endregion

            return !this.IsStop;
        }
        /// 日计时试验
        /// <summary>
        /// 日计时试验
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="clockFreq">被检表时钟脉冲频率，默认1Hz</param>
        /// <param name="second">检验时间，单位：秒</param>
        /// <param name="count">检验次数</param>
        /// <param name="returnSampleDatas">误差结果的回调句柄。根据检验次数，每产生一次误差值，通过该回调句柄上上层系统返回误差值</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        public override bool ExecuteClockError(float U, float acFreq, float clockFreq, int second, int count, ReturnSampleDataErrDelegate returnSampleDatas)
        {
            bool readLineOver = false;                              //读取当前点结束。
            bool readErrOver = false;                               //校验点执行完成
            int[] lastErrNO = new int[MeterPositions.Length];       //上次误差次数。
            int[] sampleNo = new int[MeterPositions.Length];        //采样次数记录 

            int[] iMeterReSult = new int[MeterPositions.Length];   //标识表位结论。0未检定、1合格、2不合格
            int[] upErrNum = new int[MeterPositions.Length];        //超差次数
            string[,] errValue = new string[MeterPositions.Length, count];  //采样误差数据
            for (int o = 0; o < MeterPositions.Length; o++)
            {
                lastErrNO[o] = 0;
                sampleNo[o] = 0;
                upErrNum[o] = 0;
                iMeterReSult[o] = 0;
                //errValue[o] = string.Empty;
            }
            for (int j = 0; j < MeterPositions.Length; j++)
            {
                for (int k = 0; k < count; k++)
                {
                    errValue[j, k] = string.Empty;
                }
            }
            float timeOut;
            calcTime.CalcClockErrorTime(U, acFreq, clockFreq, second, count, out timeOut);//超时时间
            this.IsStop = false;

            #region 1.初始化系统通讯基本模块

            ////1.1 设置电压电流量程
            //OnInfo("设置电压电流量程......");
            //power.SetRange(U, 0);
            //Thread.Sleep(500);



            #endregion

            //this.OnInfo("升电压......");
            //bool hasUp = power.RaiseVoltage(U, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);
            //if (hasUp)
            //{
            //    this.OnInfo("延时10秒......");
            //    Thread.Sleep(SteadyTime * 1000);
            //}

            #region 2.校验参数设置
            //2.1 初始化误差板  
            this.OnInfo("初始化误差板......");
            wcfk.InitWcfk();
            if (IsStop) return false;
            //2.2 设置检定类型  
            this.OnInfo("设置检定类型......");
            wcfk.SetVerificationType(VerificationElementType.日计时误差试验);
            if (IsStop) return false;
            //2.3 设置检定脉冲方式  
            //this.OnInfo("设置检定脉冲方式...");
            //power.SetVerificationPulseType(VerificationElementType.日计时误差试验);
            ////Thread.Sleep(500);

            //2.4 设置脉冲方向  
            this.OnInfo("设置脉冲方向......");
            wcfk.SetPulseType(Pulse.正向有功);
            //Thread.Sleep(500);
            if (IsStop) return false;
            //2.6 设置脉冲通道  
            this.OnInfo("设置脉冲通道......");
            wcfk.SetPulseChannel(VerificationElementType.日计时误差试验, Pulse.正向有功);
            if (IsStop) return false;
            //2.7 设置检定时间  
            this.OnInfo("设置检定时间......");
            wcfk.SetClockErrorTime(second, VerificationElementType.日计时误差试验);
            if (IsStop) return false;
            //2.8 设置时钟通道  
            this.OnInfo("设置时钟通道......");
            timeChannel.SetTimeChannel(0);

            ////2.9 启动误差版
            //this.OnInfo("启动误差版......");
            //wcfk.StartWcfk();

            //2.10 设置标准表接线方式、电能指示
            //this.OnInfo("设置标准表接线方式、电能指示......");
            //stdMeter.SetStdMeterWiringMode(VerificationElementType.日计时误差试验, meter.WiringMode, Pulse.正向有功);

            //2.11 切换CT档位 
            //this.OnInfo("切换CT档位...");
            //currentControl.SetCurrentControl(0f);
            //Thread.Sleep(500);

            //2.12 切换多功能控制板 
            //this.OnInfo("切换多功能控制板...");
            //powerSupply.SetPowerSupplyType(VerificationElementType.日计时误差试验);
            //Thread.Sleep(500);

            //2.13 升电压
            this.OnInfo("升电压......");
            bool hasUp = power.RaiseVoltage(U, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);
            if (IsStop) return false;
            //2.15 延时10秒
            if (hasUp)
            {
                this.OnInfo("延时10秒......");
                Thread.Sleep(SteadyTime * 1000);
            }
            #endregion
            if (IsStop) return false;
            //2.9 启动误差版
            this.OnInfo("启动误差版......");
            wcfk.StartWcfk();
            #region 3.设置多功能端子
            //bool[] results;
            //string[] resultDescription;
            // 3.1 设置多功能端子
            //this.OnInfo("设置多功能端子...");
            //dlt645.SetMulTerminalOut(lstMeterIndex.ToArray(), VerificationElementType.日计时误差试验, out results, out resultDescription);
            //CheckError("设置多功能端子", results);
            #endregion
            if (IsStop) return false;
            #region 4.开始检定并读取误差
            // 4.1 开始检定
            this.OnInfo("开始检定...");
            wcfk.StartVerification(VerificationElementType.日计时误差试验);
            if (IsStop) return false;
            // 4.2 读取误差
            this.OnInfo("读取误差...");
            int sampleIndex = 1;
            int startTime = Environment.TickCount;
            while (true)
            {
                if (IsStop)
                {
                    break;
                }
                Thread.Sleep(1000);
                if (sampleIndex <= count)                        //说明该校验点检定还未完成
                {
                    for (int i = 0; i < MeterPositions.Length; i++)
                    {
                        if (!IsStop
                            && MeterPositions[i].IsVerify
                            && string.IsNullOrEmpty(errValue[i,sampleIndex-1]))
                        {
                            this.OnInfo(string.Format("正在读取第{0}表位误差……", i + 1));
                            ErrorData errorData = wcfk.ReadErrorData(i + 1, sampleIndex);
                            if (errorData != null
                                && lastErrNO[i] != errorData.SampleIndex//sampleIndex
                                && !string.IsNullOrEmpty(errorData.ErrorValue)
                                 && Convert.ToSingle(errorData.ErrorValue) > -10f
                                && Convert.ToSingle(errorData.ErrorValue) < 10f)//增加判定误差过大不计入误差检测数据内
                                
                            {
                                if (Convert.ToSingle(errorData.ErrorValue) > -0.5
                                && Convert.ToSingle(errorData.ErrorValue) < 0.5)
                                {
                                    sampleNo[i]++;
                                    lastErrNO[i] = errorData.SampleIndex;//sampleIndex;
                                    errValue[i, sampleIndex - 1] = errorData.ErrorValue;
                                    iMeterReSult[i] = 1;
                                }
                                else if (upErrNum[i] > 3)
                                {
                                    sampleNo[i]++;
                                    lastErrNO[i] = errorData.SampleIndex;// sampleIndex;
                                    errValue[i, sampleIndex - 1] = errorData.ErrorValue;
                                    iMeterReSult[i] = 2;
                                }
                                else
                                {
                                    upErrNum[i]++;
                                }
                            }
                        }
                    }
                }
                //判断是否所有表位都读取成功，必须要将所有选中的表位读取成功，才能回传。
                readLineOver = true;
                if (((Environment.TickCount - startTime) / 1000 + 1) < timeOut + (sampleIndex == 1 ? 30 : 0))
                {
                    for (int i = 0; i < MeterPositions.Length; i++)
                    {
                        if (string.IsNullOrEmpty(errValue[i,sampleIndex-1])
                            && MeterPositions[i].IsVerify)
                        {
                            readLineOver = false;
                            break;
                        }
                    }
                }
                if(readLineOver)
                {
                    startTime = Environment.TickCount;
                    sampleIndex++;
                }
                //如果所有表位都读取到数据则进行返回，这儿也可以不用判断，每次采样进行多次返回，这样在界面上可以看到那些表位没有获取到数据
                if (sampleIndex > count)
                {
                    returnSampleDatas(errValue, iMeterReSult);
                    errValue = new string[MeterPositions.Length,count];//发送结束后，在进行重新分配。
                    
                    startTime = Environment.TickCount;
                }

                if (sampleIndex > count)                        //表明检定已经完成，返回
                {
                    readErrOver = true;
                    break;
                }
            }
            #endregion

            #region 5.停止检定设置
            //5.1 停止误差版
            this.OnInfo("停止误差版...");
            wcfk.StopWcfk();

            //5.2 停止标准表
            this.OnInfo("停止标准表...");
            stdMeter.StopStdMeter();

            //5.3 停止检定
            //OnInfo("停止检定");
            //power.StopVerification();
            #endregion

            return readErrOver;
        }
        /// 表计对时
        /// <summary>
        /// 表计对时,先按南自总线式通信方式进行编码，后期与各厂商讨论后再做调整
        /// 供应商直接根据操作系统时间对时，操作系统时间准确性有上层系统保障
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="results">返回表计对时结论</param>
        /// <param name="resultDescriptions">返回表计对时结论描述</param>
        /// <param name="berforTimes">返回对时前电能表时间，格式yyyy-MM-dd HH:mm:ss，如2011-03-09 09:32:31</param>
        /// <param name="afterTimes">返回对时后电能表时间，格式yyyy-MM-dd HH:mm:ss，如2011-03-09 09:32:31</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        public override bool TimeSync(float U, float acFreq, out bool[] results, out string[] resultDescriptions, out string[] berforTimes, out string[] afterTimes)
        {
            results = new bool[MeterPositions.Length];
            resultDescriptions = new string[MeterPositions.Length];
            berforTimes = new string[MeterPositions.Length];
            afterTimes = new string[MeterPositions.Length];
            this.IsStop = false;

            #region 1.初始化系统通讯基本模块
            //1.1 发送电压电流量程信息    IPower
            this.OnInfo("设置电压电流量程......");
            power.SetRange(U, 0);
            Thread.Sleep(500);

            //1.2 切换多功能控制板
            this.OnInfo("切换多功能控制板...");
            powerSupply.SetPowerSupplyType(VerificationElementType.常数校核试验);
            Thread.Sleep(500);

            //1.3 升电压   IPower
            this.OnInfo("升电压......");
            bool hasUp = power.RaiseVoltage(U, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);

            //1.4 延时10秒 
            if (hasUp)
            {
                this.OnInfo("延时10秒......");
                Thread.Sleep(SteadyTime * 1000);
            }
            #endregion

            #region 2.读取对时前表计参数信息
            this.OnInfo("读取对时前表计参数信息......");
            DateTime[] dates;
            //2.1、读取电能表日期信息 Dlt645
            dlt645.GetDate(lstMeterIndex.ToArray(), out dates);
            DateTime[] times;
            //2.2、读取电能表时间信息 Dlt645
            dlt645.GetTime(lstMeterIndex.ToArray(), out times);
            for (int i = 0; i < MeterPositions.Length; i++)
            {
                if (MeterPositions[i].IsVerify)
                {
                    berforTimes[i] = string.Format("{0} {1}", dates[i].ToString("yyyy-MM-dd"), times[i].ToString("HH:mm:ss"));
                }
            }
            #endregion

            #region 3.写表计参数
            this.OnInfo("写表计参数信息......");
            //3.1、写电能表日期信息 Dlt645
            dlt645.SetDate(lstMeterIndex.ToArray(), DateTime.Now, out results, out resultDescriptions);
            CheckError("写电能表日期信息", results);
            //3.2、写电能表时间信息 Dlt645
            dlt645.SetTime(lstMeterIndex.ToArray(), DateTime.Now, out results, out resultDescriptions);
            CheckError("写电能表时间信息", results);
            #endregion

            #region 4.读取对时后表计参数信息
            this.OnInfo("读取对时后表计参数信息......");
            //4.1、读取电能表日期信息 Dlt645
            dlt645.GetDate(lstMeterIndex.ToArray(), out dates);
            //4.2、读取电能表时间信息 Dlt645
            dlt645.GetTime(lstMeterIndex.ToArray(), out times);
            for (int i = 0; i < MeterPositions.Length; i++)
            {
                if (MeterPositions[i].IsVerify)
                {
                    afterTimes[i] = string.Format("{0} {1}", dates[i].ToString("yyyy-MM-dd"), times[i].ToString("HH:mm:ss"));
                }
            }
            #endregion

            return !IsStop;
        }
        /// 启动试验
        /// <summary>
        /// 启动试验
        /// </summary>
        /// <param name="U">启动电压，单位V</param>
        /// <param name="I">启动电流，单位I</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="second">启动试验时间，单位：秒</param>
        /// <param name="startTimes">返回被实验电能表的实际启动（收到脉冲信号）时间，格式：浮点数字符串，如×××.××，单位：秒。如果被实验电能表未收到脉冲，相应的表位返回null</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</param>
        public override bool ExecuteStartup(float U, float I, float acFreq, int second, out string[] startTimes)
        {
            startTimes = new string[MeterPositions.Length];
            this.IsStop = false;

            #region 1.初始化系统通讯基本模块

            // 1.1 设置电压电流量程
            this.OnInfo("设置电压电流量程...");
            power.SetRange(U, I);
            Thread.Sleep(500);

            #endregion

            #region 2.校验参数设置
            // 2.1 初始化误差板
            this.OnInfo("初始化误差板...");
            wcfk.InitWcfk();

            // 2.2 设置检定类型
            this.OnInfo("设置检定类型...");
            wcfk.SetVerificationType(VerificationElementType.电表启动试验);

            //2.3 设置检定脉冲方式  
            this.OnInfo("设置检定脉冲方式...");
            power.SetVerificationPulseType(VerificationElementType.电表启动试验);
            Thread.Sleep(500);

            //2.4 设置脉冲方向  
            this.OnInfo("设置脉冲方向......");
            wcfk.SetPulseType(Pulse.正向有功);
            Thread.Sleep(500);

            //2.5 设置脉冲通道
            this.OnInfo("设置脉冲通道...");
            wcfk.SetPulseChannel(VerificationElementType.电表启动试验, Pulse.正向有功);

            //2.6 设置被检表常数
            this.OnInfo("设置被检表常数...");
            wcfk.SetMeterConst(meter.Const);

            //2.7 启动误差板
            this.OnInfo("启动误差版");
            wcfk.StartWcfk();

            // 2.8 设置标准表接线方式、电能指示
            this.OnInfo("设置标准表接线方式、电能指示...");
            stdMeter.SetStdMeterWiringMode(VerificationElementType.电表启动试验, meter.WiringMode, Pulse.正向有功);

            //2.9 切换CT档位 
            this.OnInfo("切换CT档位...");
            currentControl.SetCurrentControl(0f);
            Thread.Sleep(500);

            //2.10 切换多功能控制板 
            this.OnInfo("切换多功能控制板...");
            powerSupply.SetPowerSupplyType(VerificationElementType.电表启动试验);
            Thread.Sleep(500);

            //2.11 设置标准表常数
            this.OnInfo("设置标准表常数......");
            stdMeter.SetStdMeterConst(U, I, meter.WiringMode);

            //2.12 升电压、电流
            this.OnInfo("正在升电压、电流信息...");
            power.RaiseVotageCurrent(U, I, LoadPhase.None, meter.WiringMode, 1.0f, true, Pulse.正向有功);

            //2.13 延时10秒
            this.OnInfo("延时10秒......");
            Thread.Sleep(SteadyTime * 1000);
            #endregion

            #region 3.开始检定并读取误差
            // 3.1 开始试验
            this.OnInfo("开始试验");
            wcfk.StartVerification(VerificationElementType.电表启动试验);

            // 3.2 读取误差
            bool finish;                         // 启动试验完成标志
            int startTime = Environment.TickCount;
            while (true)
            {
                if (IsStop)
                {
                    break;
                }

                finish = true;
                Thread.Sleep(1000);
                for (int i = 0; i < MeterPositions.Length; i++)
                {
                    if (!IsStop && MeterPositions[i].IsVerify)
                    {
                        if (string.IsNullOrEmpty(startTimes[i]))
                        {
                            this.OnInfo(string.Format("正在读取第{0}表位启动脉冲……", i + 1));
                            PulseValue pulseValue = wcfk.ReadPulse(MeterPositions[i].MeterIndex);
                            if (pulseValue != null && pulseValue.Count > 0)
                            {
                                startTimes[i] = string.Format("{0:f3}", (Environment.TickCount - startTime) / 1000);
                            }
                            else
                            {
                                finish = false;
                            }
                        }
                    }
                }
                if (finish || (Environment.TickCount - startTime) / 1000 > second)
                {
                    break;
                }
            }
            #endregion

            #region 4.停止检定设置
            //4.1 停止误差版
            this.OnInfo("停止误差版...");
            wcfk.StopWcfk();

            //4.2 停止标准表
            this.OnInfo("停止标准表...");
            stdMeter.StopStdMeter();

            //4.3 停止检定 Sd2000
            OnInfo("停止检定...");
            power.StopVerification();
            #endregion

            return !IsStop;

        }
        /// 潜动试验
        /// <summary>
        /// 潜动试验
        /// 设备供应商根据试验参数，返回被实验电能表的实际潜动时间（单位：秒）。由应用系统判断试验结论
        /// </summary>
        /// <param name="U">潜动电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="second">潜动试验时间，单位：秒</param>
        /// <param name="latentTimes">返回被实验电能表的实际潜动（收到脉冲信号）时间，格式：浮点数字符串，如×××.××，单位：秒。如果被实验电能表未收到脉冲，相应的表位返回null</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        public override bool ExecuteLatent(float U, float acFreq, int second, out string[] latentTimes)
        {
            latentTimes = new string[MeterPositions.Length];
            this.IsStop = false;

            #region 1.初始化系统通讯基本模块

            // 1.1 设置电压电流量程
            this.OnInfo("设置电压电流量程...");
            power.SetRange(U, 0);
            Thread.Sleep(500);

            #endregion

            #region 2.校验参数设置
            // 2.1 初始化误差板
            this.OnInfo("初始化误差板...");
            wcfk.InitWcfk();

            // 2.2 设置检定类型
            this.OnInfo("设置检定类型...");
            wcfk.SetVerificationType(VerificationElementType.电表潜动试验);

            //2.3 设置检定脉冲方式  
            this.OnInfo("设置检定脉冲方式...");
            power.SetVerificationPulseType(VerificationElementType.电表潜动试验);
            Thread.Sleep(500);

            //2.4 设置脉冲方向  
            this.OnInfo("设置脉冲方向......");
            wcfk.SetPulseType(Pulse.正向有功);

            //2.5 设置脉冲通道
            this.OnInfo("设置脉冲通道...");
            wcfk.SetPulseChannel(VerificationElementType.电表潜动试验, Pulse.正向有功);

            //2.6 设置被检表常数
            this.OnInfo("设置被检表常数...");
            wcfk.SetMeterConst(meter.Const);

            //2.7 启动误差板
            this.OnInfo("启动误差版");
            wcfk.StartWcfk();

            // 2.8 设置标准表接线方式、电能指示
            this.OnInfo("设置标准表接线方式、电能指示...");
            stdMeter.SetStdMeterWiringMode(VerificationElementType.电表潜动试验, meter.WiringMode, Pulse.正向有功);

            //2.9 切换CT档位 
            this.OnInfo("切换CT档位...");
            currentControl.SetCurrentControl(0f);
            Thread.Sleep(500);

            //2.10 切换多功能控制板
            this.OnInfo("切换多功能控制板...");
            powerSupply.SetPowerSupplyType(VerificationElementType.电表潜动试验);
            Thread.Sleep(500);

            //2.11 设置标准表常数
            this.OnInfo("设置标准表常数......");
            stdMeter.SetStdMeterConst(U, 0, meter.WiringMode);

            //2.12 升电压、电流
            this.OnInfo("升电压......");
            bool hasUp = power.RaiseVoltage(U, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);

            //2.13 延时10秒
            if (hasUp)
            {
                this.OnInfo("延时10秒......");
                Thread.Sleep(SteadyTime * 1000);
            }
            #endregion

            #region 3.开始试验并读取误差
            // 3.1 开始试验
            OnInfo("开始试验");

            wcfk.StartVerification(VerificationElementType.电表潜动试验);

            // 3.2 读取误差
            int startTime = Environment.TickCount;
            bool finish;// 潜动试验完成标志
            while (true)
            {
                if (IsStop)
                {
                    break;
                }

                finish = true;
                Thread.Sleep(1000);
                OnInfo(string.Format("潜动试验进行中，已经进行{0}秒，一共需要{1}秒", (Environment.TickCount - startTime) * 0.001, second));
                //在进行潜动时候，只需要在最后两分钟内，读取脉冲信息，在此期间，只需要等待即可
                if ((Environment.TickCount - startTime) / 1000 > (second - 120))
                {
                    for (int i = 0; i < MeterPositions.Length; i++)
                    {
                        if (!IsStop && (MeterPositions[i].IsVerify))
                        {
                            if (string.IsNullOrEmpty(latentTimes[i]))
                            {
                                OnInfo(string.Format("正在读取第{0}表位潜动脉冲……", i + 1));
                                PulseValue pulseValue = wcfk.ReadPulse(i + 1);
                                if (pulseValue != null && pulseValue.Count > 0)
                                {
                                    latentTimes[i] = ((Environment.TickCount - startTime) / 1000).ToString("f2");
                                }
                                else
                                {
                                    finish = false;
                                }
                            }
                        }
                    }
                    if (finish || (Environment.TickCount - startTime) / 1000 > second)
                    {
                        break;
                    }
                }
            }
            #endregion

            #region 4.停止检定设置
            //4.1 停止误差版
            this.OnInfo("停止误差版...");
            wcfk.StopWcfk();

            //4.2 停止标准表
            this.OnInfo("停止标准表...");
            stdMeter.StopStdMeter();

            //4.3 停止检定
            OnInfo("停止检定");
            power.StopVerification();
            #endregion

            return !IsStop;
        }
        /// 时段投切试验
        /// <summary>
        /// 时段投切试验
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="I">试验电流，单位A，默认300%Ib</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="period">返回从电能表中读取的各时段的值，如读取到的值为峰、谷，则返回数组长度为2，period[0]为2，period[1]为4。</param>
        /// <param name="periodTime">返回电能表中各时段对应的时间，长度与返回的时段数组长度一致且与上面的时段一一对应，格式为hh:mm，
        ///  如上面描述的峰、谷时段的时间为8:00、21:00，则返回值为periodTime[0]为8:00，periodTime[1]为21:00</param>
        /// <param name="valueErrors">根据电能表返回的时段，返回各费率时段的示值误差。
        /// 如有两个时段，0:00-8:00谷，8:00-21:00峰，21:00-24:00谷，需要返回两组试验结果，其中
        ///     valueErrors[0][..]表示“谷到峰”的误差集合
        ///     valueErrors[1][..]表示“峰到谷”的误差集合
        /// 如有四个时段，0:00-8:00谷，8:00-12:00峰，12:00-17:00平，17:00-21:00峰，21:00-24:00谷，需要返回四组试验结果，其中
        ///     valueErrors[0][..]表示“谷到峰”的误差集合
        ///     valueErrors[1][..]表示“峰到平”的误差集合
        ///     valueErrors[2][..]表示“平到峰”的误差集合
        ///     valueErrors[3][..]表示“峰到谷”的误差集合
        ///     </param>
        /// <param name="combinErrors">返回组合误差</param>
        /// <param name="changeErrors">返回投切误差，二维数组定义参照valueErrors</param>
        /// <param name="changeTimes">返回投切时间，格式为HH:mm:ss.ms，如08:00:01.123，二维数组定义参照valueErrors</param>
        /// <returns>直接返回合格与不合格</returns>
        public override bool ExecuteSwitchChange(float U, float I, float acFreq, out int[] period, out string[] periodTime, out string[][] valueErrors, out string[] combinErrors, out string[][] changeErrors, out string[][] changeTimes)
        {
            string[] errValue = null;                               //采样误差数据
            float timeOut;
            calcTime.CalcSwitchChangeTime(U, I, acFreq, out timeOut);//超时时间
            this.IsStop = false;

            #region 1.初始化系统通讯基本模块
            // 1.1 设置电压电流量程
            this.OnInfo("设置电压电流量程...");
            power.SetRange(U, I);
            Thread.Sleep(500);
            #endregion

            #region 2.校验参数设定
            //2.1 初始化误差板
            this.OnInfo("初始化误差板...");
            wcfk.InitWcfk();

            //2.2 设置检定类型
            this.OnInfo("设置检定类型...");
            wcfk.SetVerificationType(VerificationElementType.时段投切);

            //2.3 设置检定脉冲方式
            this.OnInfo("设置检定脉冲方式...");
            power.SetVerificationPulseType(VerificationElementType.时段投切);
            Thread.Sleep(500);

            //2.4 设置检定脉冲方向
            this.OnInfo("设置检定脉冲方向...");
            wcfk.SetPulseType(Pulse.正向有功);

            //2.5 设置检定脉冲通道
            this.OnInfo("设置检定脉冲通道...");
            wcfk.SetPulseChannel(VerificationElementType.时段投切, Pulse.正向有功);

            //2.6 启动误差板
            this.OnInfo("启动误差板...");
            wcfk.StartWcfk();

            //2.7 切换CT档位
            this.OnInfo("切换CT档位...");
            currentControl.SetCurrentControl(I);

            //2.8 切换多功能控制板
            this.OnInfo("切换多功能控制板...");
            powerSupply.SetPowerSupplyType(VerificationElementType.时段投切);
            Thread.Sleep(500);

            //2.9 升电压
            this.OnInfo("升电压...");
            bool hasUp = power.RaiseVoltage(U, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);

            //2.10 延时10秒
            if (hasUp)
            {
                this.OnInfo("延时10秒...");
                Thread.Sleep(SteadyTime * 1000);
            }
            #endregion

            #region 3.设置多功能端子
            bool[] results;
            string[] resultDescription;
            //3.1 设置多功能端子
            this.OnInfo("设置多功能端子...");
            dlt645.SetMulTerminalOut(lstMeterIndex.ToArray(), VerificationElementType.时段投切, out results, out resultDescription);
            CheckError("设置多功能端子", results);
            #endregion

            #region  4.读费率时段
            //4.1 读费率时段
            this.OnInfo("读费率时段...");
            MeterRates[] meterRates;
            dlt645.GetMeterRates(lstMeterIndex.ToArray(), out meterRates);
            // 4.2 初始化时段投切返回数组
            this.OnInfo("初始化时段投切返回数组");
            period = new int[meterRates.Length];                //时段的值
            periodTime = new string[meterRates.Length];         //时段对应的时间
            changeErrors = new string[meterRates.Length][];     //投切误差
            valueErrors = new string[meterRates.Length][];      //费率时段的示值误差
            combinErrors = new string[MeterPositions.Length];   //组合误差
            changeTimes = new string[meterRates.Length][];      //投切时间
            if (meterRates != null && meterRates.Length > 0)
            {
                for (int j = 0; j < meterRates.Length; j++)
                {
                    period[j] = (int)meterRates[j].Period;
                    periodTime[j] = meterRates[j].PeriodTime;
                    changeErrors[j] = new string[MeterPositions.Length];
                    changeTimes[j] = new string[MeterPositions.Length];
                    valueErrors[j] = new string[MeterPositions.Length];
                }
            }
            #endregion

            #region 5、投切实验开始
            int timeDiff = 30;   //写时段投切时间，默认为少30秒。如：8：00，则写为7：59：30
            for (int i = 0; i < meterRates.Length; i++)
            {
                errValue = new string[MeterPositions.Length];

                #region 5.1 写表计时间
                // 5.1、开始时段投切
                this.OnInfo("写表计时间...");
                DateTime dt = Convert.ToDateTime(meterRates[i].PeriodTime);
                dt = dt.AddSeconds(timeDiff * -1);
                dlt645.SetTime(lstMeterIndex.ToArray(), dt, out results, out resultDescription);
                Thread.Sleep(1000);
                CheckError("写表计时间", results);
                #endregion

                // 5.2 试验开始
                this.OnInfo("试验开始");
                wcfk.StartVerification(VerificationElementType.时段投切);

                #region 5.3 等待接收投切脉冲
                this.OnInfo("等待接收投切脉冲");
                int startTime = Environment.TickCount;//记录开始读取的时间
                bool finish = false;
                while (true)
                {
                    if (IsStop)
                    {
                        break;
                    }
                    Thread.Sleep(1000);
                    finish = true;
                    for (int j = 0; j < MeterPositions.Length; j++)
                    {
                        if (!IsStop && MeterPositions[j].IsVerify)
                        {
                            if (string.IsNullOrEmpty(errValue[j]))
                            {
                                OnInfo(string.Format("正在接收第{0}表位的投切信号……", j + 1));
                                PulseValue pulseValue = wcfk.ReadPulse((byte)(j + 1));
                                if ((pulseValue != null) && (string.IsNullOrEmpty(errValue[j])) && (pulseValue.Count > 0))
                                {
                                    // 投切误差
                                    errValue[j] = ((Environment.TickCount - startTime) / 1000 - timeDiff).ToString();
                                    changeErrors[i][j] = errValue[j];
                                    // 投切时间
                                    DateTime dtime = Convert.ToDateTime(meterRates[i].PeriodTime);
                                    dtime = dtime.AddSeconds(int.Parse(errValue[j]));
                                    changeTimes[i][j] = string.Format("{0}:{1}:{2}", dtime.Hour, dtime.Minute, dtime.Second);
                                }
                                else
                                {
                                    finish = false;
                                }
                            }
                        }
                    }

                    if (finish || (Environment.TickCount - startTime) / 1000 > timeOut)//超时退出
                    {
                        break;
                    }
                }
                #endregion

                #region 5.4 计算示值误差
                this.OnInfo(string.Format("读{0}时段始电量...", meterRates[i].Period));
                string[] startEnergy;
                dlt645.GetEnergy(lstMeterIndex.ToArray(), Pulse.正向有功, out startEnergy);

                this.OnInfo("升电流...");
                power.RaiseCurrent(I, LoadPhase.None, meter.WiringMode, 1.0f, true, Pulse.正向有功);

                this.OnInfo(string.Format("电表走字{0}秒...", timeDiff));
                for (int k = 1; k < timeDiff; k++)
                {
                    this.OnInfo(string.Format("电表走字{0}秒，已经执行{1}秒...", timeDiff, k));
                    Thread.Sleep(1000);
                }

                this.OnInfo("降电流...");
                power.RaiseCurrent(0, LoadPhase.None, meter.WiringMode, 1.0f, true, Pulse.正向有功);

                // 待电流降至零位
                Thread.Sleep(4000);
                string[] endEnergy;
                dlt645.GetEnergy(lstMeterIndex.ToArray(), Pulse.正向有功, out endEnergy);
                for (int j = 0; j < MeterPositions.Length; j++)
                {
                    if (MeterPositions[j].Meter != null && MeterPositions[j].IsVerify)
                    {
                        PhaseEnergy startPhaseEnergy = new PhaseEnergy(startEnergy[j]);
                        PhaseEnergy endPhaseEnergy = new PhaseEnergy(endEnergy[j]);
                        valueErrors[i][j] = ((Convert.ToSingle(endPhaseEnergy.总) - Convert.ToSingle(startPhaseEnergy.总))
                            - (Convert.ToSingle(endPhaseEnergy.GetPhaseEnergy((Phase)period[i])) - Convert.ToSingle(startPhaseEnergy.GetPhaseEnergy((Phase)period[i])))).ToString("f4");
                    }
                }
                #endregion
            }
            #endregion

            #region 5.5 计算组合误差
            for (int i = 0; i < MeterPositions.Length; i++)
            {
                float combinError = 0;
                for (int j = 0; j < period.Length; j++)
                {
                    combinError += Convert.ToSingle(valueErrors[j][i]);
                }
                combinErrors[i] = combinError.ToString("f4");
            }
            #endregion

            #region 6.停止检定
            //6.1 停止误差版
            this.OnInfo("停止误差版...");
            wcfk.StopWcfk();

            //6.2 停止检定
            OnInfo("停止检定");
            power.StopVerification();
            #endregion

            return !IsStop;
        }
        /// 需量周期、需量示值
        /// <summary>
        /// 需量周期、需量示值
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="I">试验电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="stdDemand">返回标准表需量示值</param>
        /// <param name="checkedDemands">返回被检表需量示值</param>
        /// <param name="demandCycleErrors">返回被检表需量周期误差</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        public override bool ExecuteDemand(float U, float I, float acFreq, out string stdDemand, out string[] checkedDemands, out string[] demandCycleErrors)
        {
            //this.ExecuteDemandValue(U, I, acFreq, out stdDemand, out checkedDemands);
            //this.ExecuteDemandPeriod(U, I, acFreq, out demandCycleErrors);
            //return true;

            this.IsStop = false;

            #region 1.初始化系统通讯基本模块
            //1.1 发送电压电流量程信息    IPower
            OnInfo("设置电压电流量程......");
            power.SetRange(U, I);
            Thread.Sleep(500);

            // 1.2 发送电压
            OnInfo("发送电压......");
            bool hasUp = power.RaiseVoltage(U, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);

            //1.3 延时10秒
            if (hasUp)
            {
                OnInfo("延时10秒...");
                Thread.Sleep(SteadyTime * 1000);
            }
            #endregion

            #region 2.清除表计最大需量
            //2.1 清除表计最大需量
            OnInfo("清除表计最大需量...");
            bool[] results;
            string[] resultDescript;
            dlt645.ClearDemand(lstMeterIndex.ToArray(), out results, out resultDescript);
            CheckError("清除表计最大需量", results);
            #endregion

            #region 3.设置多功能端子
            //3.1 设置多功能端子
            OnInfo("设置多功能端子...");
            bool[] mulResults;
            string[] mulResultDescription;
            dlt645.SetMulTerminalOut(lstMeterIndex.ToArray(), VerificationElementType.需量周期误差试验, out mulResults, out mulResultDescription);
            CheckError("设置多功能端子", mulResults);
            #endregion

            #region 4.校验参数设置
            Pulse pulse = Pulse.正向有功;       // 默认正向有功能
            // 4.1 初始化误差板
            OnInfo("初始化误差板...");
            wcfk.InitWcfk();

            // 4.2 设置检定类型
            OnInfo("设置检定类型...");
            wcfk.SetVerificationType(VerificationElementType.需量周期误差试验);

            // 4.3 设置检定脉冲方式
            OnInfo("设置检定脉冲方式...");
            power.SetVerificationPulseType(VerificationElementType.需量周期误差试验);
            Thread.Sleep(500);

            // 4.4 设置脉冲方向  
            OnInfo("设置脉冲方向......");
            wcfk.SetPulseType(Pulse.正向有功);

            // 4.5 设置脉冲通道  
            OnInfo("设置脉冲通道...");
            wcfk.SetPulseChannel(VerificationElementType.需量周期误差试验, pulse);

            // 4.13 设置滑差时间 默认滑差时间为1分钟
            OnInfo("正在设置滑差时间...");
            wcfk.SetClockErrorTime(1, VerificationElementType.需量周期误差试验);

            // 4.6 启动误差版  
            OnInfo("启动误差版...");
            wcfk.StartWcfk();

            // 4.7 设置标准表接线方式、电能指示  
            OnInfo("设置标准表接线方式、电能指示...");
            stdMeter.SetStdMeterWiringMode(VerificationElementType.需量周期误差试验, meter.WiringMode, pulse);

            // 4.8 切换多功能控制板
            OnInfo("切换多功能控制板...");
            powerSupply.SetPowerSupplyType(VerificationElementType.需量周期误差试验);
            Thread.Sleep(500);

            // 4.9 设置标准表常数
            OnInfo("设置标准表常数...");
            stdMeter.SetStdMeterConst(U, I, meter.WiringMode);

            // 4.10 切换CT档位 
            this.OnInfo("切换CT档位...");
            currentControl.SetCurrentControl(I);

            // 4.11 升电流
            OnInfo("正在升电流...");
            power.RaiseCurrent(I, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);

            // 4.12 延时10秒
            OnInfo("延时10秒...");
            Thread.Sleep(SteadyTime * 1000);
            #endregion

            #region 5、获取被检表需量周期误差
            // 5.1 试验开始
            this.OnInfo("试验开始");
            wcfk.StartVerification(VerificationElementType.需量周期误差试验);
            //5.2 获取被检表需量周期误差
            OnInfo("获取被检表需量周期误差...");
            int needmaxtime = 15;                                   //最大需量周期
            float timeOut;                                          //超时时间，先设置为半小时
            calcTime.CalcDemandTime(U, I, acFreq, out timeOut);
            int StatusStartTime = Environment.TickCount;            //记录开始读取的时间
            demandCycleErrors = new string[MeterPositions.Length];  //采样误差数据
            int sampleIndex = 1;
            while (true)
            {
                if (IsStop)
                {
                    break;
                }
                Thread.Sleep(1000);
                OnInfo(string.Format("需量周期、示值试验进行中，已经进行{0}秒，最多需要{1}秒", (System.Environment.TickCount - StatusStartTime) * 0.001, (needmaxtime + 2) * 60));
                //在进行需量周期时候，只有过了15分后，才出脉冲信号。在此期间，只需要等待即可
                if ((Environment.TickCount - StatusStartTime) / 1000 > needmaxtime * 60)
                {
                    for (int i = 0; i < MeterPositions.Length; i++)
                    {
                        if (!IsStop && MeterPositions[i].IsVerify && string.IsNullOrEmpty(demandCycleErrors[i]))
                        {
                            OnInfo(string.Format("正在读取第{0}表位需量周期误差……", i + 1));
                            ErrorData errorData = wcfk.ReadErrorData(i + 1, sampleIndex);
                            if (errorData != null && !string.IsNullOrEmpty(errorData.ErrorValue))
                            {
                                demandCycleErrors[i] = errorData.ErrorValue;//误差
                            }
                        }
                    }
                    //判断是否所有表位都读取成功，必须要将所有选中的表位读取成功，才能回传。
                    bool readLineOver = true;
                    for (int i = 0; i < MeterPositions.Length; i++)
                    {
                        if ((MeterPositions[i].IsVerify)
                            && string.IsNullOrEmpty(demandCycleErrors[i]))
                        {
                            readLineOver = false;
                            break;
                        }
                    }
                    // 已经读取所有选中的表位 或者 超时，则退出循环
                    if ( (Environment.TickCount - StatusStartTime) / 1000 > timeOut)
                    {
                        break;
                    }
                }
            }
            #endregion

            #region 6、获取标准表需量示值
            //6.1 获取标准表需量示值
            OnInfo("获取标准表需量示值...");
            stdDemand = monitor.GetStdMeterPower(Pulse.正向有功);
            #endregion

            #region 7.降电流
            //7.1、降电流 Sd2000
            OnInfo("降电流...");
            power.RaiseCurrent(0f, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);
            #endregion

            #region 8、读取被检表需量示值
            //8.1 读取被检表需量示值
            OnInfo("正在读取被检表需量示值……");
            checkedDemands = new string[MeterPositions.Length];
            dlt645.GetDemand(lstMeterIndex.ToArray(), pulse, out checkedDemands);
            #endregion

            #region 9、停止检定
            //9.1 停止检定
            OnInfo("停止检定");
            power.StopVerification();
            #endregion

            return !IsStop;
        }
        /// 关闭电压电流
        /// <summary>
        /// 关闭电压电流
        /// </summary>
        /// <returns></returns>
        public override bool CloseVoltageCurrent()
        {
            return power.RaiseVotageCurrent(0, 0, LoadPhase.None, meter.WiringMode, 1.0f, true, Pulse.正向有功);
        }
        /// 需量周期试验
        /// <summary>
        /// 需量周期试验
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="demandCycleErrors">需量周期误差值</param>
        /// <returns></returns>
        public override bool ExecuteDemandPeriod(float U, float I, float acFreq, out string[] demandCycleErrors)
        {
            this.IsStop = false;

            #region 1.初始化系统通讯基本模块
            //1.1 发送电压电流量程信息    IPower
            OnInfo("设置电压电流量程......");
            power.SetRange(U, I);
            Thread.Sleep(500);

            // 1.2 发送电压
            OnInfo("发送电压......");
            bool hasUp = power.RaiseVoltage(U, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);

            //1.3 延时10秒
            if (hasUp)
            {
                OnInfo("延时10秒...");
                Thread.Sleep(SteadyTime * 1000);
            }
            #endregion

            #region 2.清除表计最大需量
            //2.1 清除表计最大需量
            OnInfo("清除表计最大需量...");
            bool[] results;
            string[] resultDescript;
            dlt645.ClearDemand(lstMeterIndex.ToArray(), out results, out resultDescript);
            CheckError("清除表计最大需量", results);
            #endregion

            #region 3.设置多功能端子
            //3.1 设置多功能端子
            OnInfo("设置多功能端子...");
            bool[] mulResults;
            string[] mulResultDescription;
            dlt645.SetMulTerminalOut(lstMeterIndex.ToArray(), VerificationElementType.需量周期误差试验, out mulResults, out mulResultDescription);
            CheckError("设置多功能端子", mulResults);
            #endregion

            #region 4.校验参数设置
            Pulse pulse = Pulse.正向有功;       // 默认正向有功能
            // 4.1 初始化误差板
            OnInfo("初始化误差板...");
            wcfk.InitWcfk();

            // 4.2 设置检定类型
            OnInfo("设置检定类型...");
            wcfk.SetVerificationType(VerificationElementType.需量周期误差试验);

            // 4.3 设置检定脉冲方式
            OnInfo("设置检定脉冲方式...");
            power.SetVerificationPulseType(VerificationElementType.需量周期误差试验);
            Thread.Sleep(500);

            // 4.4 设置脉冲方向  
            OnInfo("设置脉冲方向......");
            wcfk.SetPulseType(Pulse.正向有功);

            // 4.5 设置脉冲通道  
            OnInfo("设置脉冲通道...");
            wcfk.SetPulseChannel(VerificationElementType.需量周期误差试验, pulse);

            // 4.13 设置滑差时间 默认滑差时间为1分钟
            OnInfo("正在设置滑差时间...");
            wcfk.SetClockErrorTime(1, VerificationElementType.需量周期误差试验);

            // 4.6 启动误差版  
            OnInfo("启动误差版...");
            wcfk.StartWcfk();

            // 4.7 设置标准表接线方式、电能指示  
            OnInfo("设置标准表接线方式、电能指示...");
            stdMeter.SetStdMeterWiringMode(VerificationElementType.需量周期误差试验, meter.WiringMode, pulse);

            // 4.8 切换多功能控制板
            OnInfo("切换多功能控制板...");
            powerSupply.SetPowerSupplyType(VerificationElementType.需量周期误差试验);
            Thread.Sleep(500);

            // 4.9 设置标准表常数
            OnInfo("设置标准表常数...");
            stdMeter.SetStdMeterConst(U, I, meter.WiringMode);

            // 4.10 切换CT档位 
            this.OnInfo("切换CT档位...");
            currentControl.SetCurrentControl(I);

            // 4.11 升电流
            OnInfo("正在升电流...");
            power.RaiseCurrent(I, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);

            // 4.12 延时10秒
            OnInfo("延时10秒...");
            Thread.Sleep(SteadyTime * 1000);
            #endregion

            #region 5、获取被检表需量周期误差
            // 5.1 试验开始
            this.OnInfo("试验开始");
            wcfk.StartVerification(VerificationElementType.需量周期误差试验);
            //5.2 获取被检表需量周期误差
            OnInfo("获取被检表需量周期误差...");
            int needmaxtime = 15;                                   //最大需量周期
            float timeOut;                                          //超时时间
            calcTime.CalcDemandTime(U, I, acFreq, out timeOut);
            int StatusStartTime = Environment.TickCount;            //记录开始读取的时间
            demandCycleErrors = new string[MeterPositions.Length];  //采样误差数据
            int sampleIndex = 1;
            while (true)
            {
                if (IsStop)
                {
                    break;
                }
                Thread.Sleep(1000);
                OnInfo(string.Format("需量周期、示值试验进行中，已经进行{0}秒，最多需要{1}秒", (System.Environment.TickCount - StatusStartTime) * 0.001, (needmaxtime + 2) * 60));
                //在进行需量周期时候，只有过了15分后，才出脉冲信号。在此期间，只需要等待即可
                if ((Environment.TickCount - StatusStartTime) / 1000 > needmaxtime * 60)
                {
                    for (int i = 0; i < MeterPositions.Length; i++)
                    {
                        if (!IsStop && MeterPositions[i].IsVerify && string.IsNullOrEmpty(demandCycleErrors[i]))
                        {
                            OnInfo(string.Format("正在读取第{0}表位需量周期误差……", i + 1));
                            ErrorData errorData = wcfk.ReadErrorData(i + 1, sampleIndex);
                            if (errorData != null
                                && errorData.SampleIndex > 0)
                            {
                                demandCycleErrors[i] = errorData.ErrorValue;//误差
                            }
                        }
                    }
                    //判断是否所有表位都读取成功，必须要将所有选中的表位读取成功，才能回传。
                    bool readLineOver = true;
                    for (int i = 0; i < MeterPositions.Length; i++)
                    {
                        if ((MeterPositions[i].IsVerify)
                            && string.IsNullOrEmpty(demandCycleErrors[i]))
                        {
                            readLineOver = false;
                            break;
                        }
                    }
                    // 已经读取所有选中的表位 或者 超时，则退出循环
                    if (readLineOver || (Environment.TickCount - StatusStartTime) / 1000 > timeOut)
                    {
                        break;
                    }
                }
            }
            #endregion

            #region 6.降电流
            //6.1、降电流 Sd2000
            OnInfo("降电流...");
            power.RaiseCurrent(0f, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);
            #endregion

            #region 7、停止检定
            //8.1 停止检定
            OnInfo("停止检定");
            power.StopVerification();
            #endregion

            return !IsStop;
        }
        /// 需量示值试验
        /// <summary>
        /// 需量示值试验
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="I">试验电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="stdDemand">标准表需量</param>
        /// <param name="checkedDemands">被检表需量</param>
        /// <returns></returns>
        public override bool ExecuteDemandValue(float U, float I, float acFreq, out string stdDemand, out string[] checkedDemands)
        {
            IsStop = false;
            float timeOut;                                          //超时时间
            calcTime.CalcDemandTime(U, I, acFreq, out timeOut);

            #region 1.初始化通讯模块
            // 发送电压电流量程信息
            OnInfo("设置电压电流量程......");
            power.SetRange(U, I);
            Thread.Sleep(500);

            // 升电压
            OnInfo("升电压......");
            bool hasUp = power.RaiseVoltage(U, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);

            // 升压延时
            if (hasUp)
            {
                OnInfo("升压延时...");
                Thread.Sleep(SteadyTime * 1000);
            }
            #endregion

            #region 2.清除表计最大需量
            //2.1 清除表计最大需量
            OnInfo("清除表计最大需量...");
            bool[] results;
            string[] resultDescript;
            dlt645.ClearDemand(lstMeterIndex.ToArray(), out results, out resultDescript);
            CheckError("清除表计最大需量", results);
            #endregion

            #region 3.设置多功能端子
            //3.1 设置多功能端子
            OnInfo("设置多功能端子...");
            bool[] mulResults;
            string[] mulResultDescription;
            dlt645.SetMulTerminalOut(lstMeterIndex.ToArray(), VerificationElementType.需量周期误差试验, out mulResults, out mulResultDescription);
            CheckError("设置多功能端子", mulResults);
            #endregion

            #region 4.校验参数设置
            // 4.1 设置检定类型
            OnInfo("设置检定类型...");
            wcfk.SetVerificationType(VerificationElementType.需量周期误差试验);

            // 4.2 切换多功能控制板
            OnInfo("切换多功能控制板...");
            powerSupply.SetPowerSupplyType(VerificationElementType.需量周期误差试验);
            Thread.Sleep(500);

            // 4.3 设置标准表常数
            OnInfo("设置标准表常数...");
            stdMeter.SetStdMeterConst(U, I, meter.WiringMode);

            // 4.4 切换CT挡位
            this.OnInfo("切换CT挡位...");
            currentControl.SetCurrentControl(I);

            // 4.5 升电流
            OnInfo("升电流..");
            power.RaiseCurrent(I, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);

            // 4.6 升电流延时
            OnInfo("升电流延时...");
            Thread.Sleep(SteadyTime * 1000);
            #endregion

            #region 5.开始试验并获取需量示值
            // 5.1 开始试验
            this.OnInfo("开始试验");
            wcfk.StartVerification(VerificationElementType.需量周期误差试验);

            // 5.2 等待被检表需量周期误差
            OnInfo("等待被检表需量周期误差...");
            int statusStartTime = Environment.TickCount;
            while (true)
            {
                if (IsStop)
                {
                    break;
                }
                Thread.Sleep(1000);
                OnInfo(string.Format("需量示值试验进行中，已经进行{0}秒，最多需要{1}秒", (System.Environment.TickCount - statusStartTime) * 0.001, 17 * 60));
                if ((Environment.TickCount - statusStartTime) / 1000 > timeOut)
                {
                    break;
                }
            }

            // 5.3 获取标准表需量示值
            OnInfo("获取标准表需量示值...");
            stdDemand = monitor.GetStdMeterPower(Pulse.正向有功);

            // 5.4 降电流
            OnInfo("降电流...");
            power.RaiseCurrent(0f, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);

            // 5.5 读被检需量示值
            OnInfo("读被检需量示值...");
            dlt645.GetDemand(lstMeterIndex.ToArray(), Pulse.正向有功, out checkedDemands);
            #endregion

            #region 6、停止检定
            // 6.1 停止检定
            OnInfo("停止检定");
            power.StopVerification();
            #endregion

            return !IsStop;
        }
        /// 走字和校核计度器试验
        /// <summary>
        /// 走字和校核计度器试验
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="I">试验电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="factor">功率因数</param>
        /// <param name="capacitive">容性标志，true表示容性，false表示感性</param>
        /// <param name="pulse">脉冲类型，正向有功、反向有功、正向无功、反向无功</param>
        /// <param name="phase">费率</param>
        /// <param name="energy">走字电能(kWh)</param>
        /// <param name="stdEnergy">返回标准表走过的电能值(kWh)</param>
        /// <param name="initialTotalReading">返回总计度器的走字始度(kWh)</param>
        /// <param name="initialSubReading">返回分费率计度器的走字始度(kWh)</param>
        /// <param name="finalTotalReading">返回总计度器的走字止度(kWh)</param>
        /// <param name="finalSubReading">分费率计度器的走字止度(kWh)</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        public override bool ExecuteEnergyReading(float U, float I, float acFreq, float factor, bool capacitive, Pulse pulse, Phase phase, float energy, out string stdEnergy, out string[] initialTotalReading, out string[] initialSubReading, out string[] finalTotalReading, out string[] finalSubReading)
        {
            bool[] results;
            string[] resultDescription;
            this.IsStop = false;

            #region 1.初始化系统通讯基本模块

            // 1.1 设置电压电流量程
            this.OnInfo("设置电压电流量程...");
            power.SetRange(U, I);
            Thread.Sleep(500);

            //1.3 设置相位信息 IPower
            this.OnInfo("正在设置相位信息...");
            power.SetLoadPhase(meter.WiringMode, factor, capacitive, pulse, LoadPhase.None);
            Thread.Sleep(500);
            #endregion

            #region 2.校验参数设置
            // 2.1 初始化误差板
            this.OnInfo("初始化误差板...");
            wcfk.InitWcfk();

            // 2.2 设置检定类型
            this.OnInfo("设置检定类型...");
            wcfk.SetVerificationType(VerificationElementType.电能走字试验);

            // 2.3 设置检定脉冲方式
            this.OnInfo("设置检定脉冲方式...");
            power.SetVerificationPulseType(VerificationElementType.电能走字试验);
            Thread.Sleep(500);

            //2.4 设置脉冲方向  
            this.OnInfo("设置脉冲方向......");
            wcfk.SetPulseType(Pulse.正向有功);

            //2.5 设置脉冲通道  
            this.OnInfo("设置脉冲通道...");
            wcfk.SetPulseChannel(VerificationElementType.电能走字试验, pulse);

            //2.6 设置被检表常数 IWcfk
            this.OnInfo("正在设置被检表常数...");
            wcfk.SetMeterConst(pulse == Pulse.反向无功 || pulse == Pulse.正向无功 ? meter.Rp_Const : meter.Const,U,I);

            //2.7 启动误差版  
            this.OnInfo("启动误差版...");
            wcfk.StartWcfk();

            //2.8 设置标准表接线方式、电能指示  
            this.OnInfo("设置标准表接线方式、电能指示...");
            stdMeter.SetStdMeterWiringMode(VerificationElementType.电能走字试验, meter.WiringMode, pulse);

            //2.9 切换CT档位
            this.OnInfo("切换CT档位...");
            currentControl.SetCurrentControl(I);
            Thread.Sleep(500);

            //2.10 切换多功能控制板
            OnInfo("切换多功能控制板...");
            powerSupply.SetPowerSupplyType(VerificationElementType.电能走字试验);
            Thread.Sleep(500);

            //2.11 设置标准表常数
            this.OnInfo("设置标准表常数...");
            stdMeter.SetStdMeterConst(U, I, meter.WiringMode);

            //2.12 升电压
            OnInfo("正在升电压...");
            bool hasUp = power.RaiseVoltage(U, LoadPhase.None, meter.WiringMode, factor, capacitive, pulse);

            //2.13 延时10秒
            if (hasUp)
            {
                OnInfo("延时10秒...");
                Thread.Sleep(SteadyTime * 1000);
            }
            #endregion

            #region  3.读费率时段
            //3.1 读费率时段
            this.OnInfo("读费率时段...");
            MeterRates[] meterRates;
            dlt645.GetMeterRates(lstMeterIndex.ToArray(), out meterRates);
            string periodTime = meterRates.First(item => item.Period == phase).PeriodTime;
            #endregion

            #region 4.写表计时间
            //4.1 写表计时间
            this.OnInfo("写表计时间...");
            dlt645.SetTime(lstMeterIndex.ToArray(), Convert.ToDateTime(periodTime), out results, out resultDescription);
            CheckError("写表计时间", results);
            #endregion

            #region 5.读电能始度
            // 5.1 读电能始度
            this.OnInfo("读电能始度...");
            string[] energys;
            dlt645.GetEnergy(lstMeterIndex.ToArray(), pulse, out energys);
            initialTotalReading = new string[MeterPositions.Length];
            initialSubReading = new string[MeterPositions.Length];
            for (int i = 0; i < MeterPositions.Length; i++)
            {
                if (MeterPositions[i].IsVerify && !string.IsNullOrEmpty(energys[i]))
                {
                    string[] phaseEnergy = energys[i].Split(',');
                    initialTotalReading[i] = phaseEnergy[0];
                    initialSubReading[i] = phaseEnergy[(int)phase];
                }
            }
            #endregion

            #region 6.开始试验并读取标准表电量
            // 6.1 开始检定
            this.OnInfo("开始检定...");
            wcfk.StartVerification(VerificationElementType.电能走字试验);

            // 6.2 升电流
            OnInfo("正在升电流...");
            power.RaiseCurrent(I, LoadPhase.None, meter.WiringMode, factor, capacitive, pulse);

            // 6.3 读标准表电量
            this.OnInfo("读标准表电量...");
            stdEnergy = "0"; ;
            while (float.Parse(stdEnergy) < energy)//走字结束
            {
                if (IsStop)
                {
                    break;
                }
                Thread.Sleep(20);
                stdEnergy = stdMeter.GetStdMeterEnergy().ToString();
                OnInfo("当前标准表电量：" + stdEnergy);
            }

            // 6.4 降电流
            this.OnInfo("降电流...");
            power.RaiseCurrent(0f, LoadPhase.None, meter.WiringMode, factor, capacitive, pulse);

            // 6.5 延时10秒
            this.OnInfo("延时10秒...");
            Thread.Sleep(SteadyTime * 1000);

            // 6.6 读最终标准表电量
            this.OnInfo("读最终标准表电量...");
            stdEnergy = stdMeter.GetStdMeterEnergy().ToString();
            #endregion

            #region 7.读电能止度
            // 7.1 读电能止度
            this.OnInfo("读电能止度...");
            dlt645.GetEnergy(lstMeterIndex.ToArray(), pulse, out energys);
            finalTotalReading = new string[MeterPositions.Length];
            finalSubReading = new string[MeterPositions.Length];
            for (int i = 0; i < MeterPositions.Length; i++)
            {
                if (MeterPositions[i].IsVerify && !string.IsNullOrEmpty(energys[i]))
                {
                    string[] phaseEnergy = energys[i].Split(',');
                    finalTotalReading[i] = phaseEnergy[0];
                    finalSubReading[i] = phaseEnergy[(int)phase];
                }
            }
            #endregion

            #region 8.停止检定
            //8.1 停止误差版
            this.OnInfo("停止误差版...");
            wcfk.StopWcfk();

            //8.2 停止标准表
            this.OnInfo("停止标准表...");
            stdMeter.StopStdMeter();
            #endregion

            return !IsStop;
        }
        /// 清电量
        /// <summary>
        /// 清电量
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="results">返回清电量结论</param>
        /// <param name="resultDescriptions">返回清电量结论描述</param>
        /// <param name="berforEnergys">返回清零前正向有功总电量</param>
        /// <param name="afterEnergys">返回清零后正向有功总电量</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        public override bool ClearEnergy(float U, float acFreq, out bool[] results, out string[] resultDescriptions, out string[] berforEnergys, out string[] afterEnergys)
        {
            this.IsStop = false;

            #region 1.初始化系统通讯基本模块
            //1.1 设置电压电流量程
            OnInfo("设置电压电流量程...");
            power.SetRange(U, 0);
            Thread.Sleep(100);

            //1.2 切换多功能控制板
            OnInfo("切换多功能控制板...");
            powerSupply.SetPowerSupplyType(VerificationElementType.电量清零);
            Thread.Sleep(100);

            //1.3 升电压
            OnInfo("升电压...");
            bool hasUp = power.RaiseVoltage(U, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);

            //1.4 延时10秒
            if (hasUp)
            {
                OnInfo("延时10秒...");
                Thread.Sleep(SteadyTime * 1000);
            }
            #endregion

            #region 2.清电量前读电能底数
            //2.1 清电量前读电能底数
            OnInfo("清电量前读电能底数...");
            dlt645.GetEnergy(lstMeterIndex.ToArray(), Pulse.正向有功, out berforEnergys);
            #endregion

            #region 3.清电量
            //3.1 清电量
            OnInfo("清电量...");
            dlt645.ClearEnergy(lstMeterIndex.ToArray(), out results, out resultDescriptions);
            CheckError("清电量", results);
            #endregion

            #region 4.清电量后延时
            //4.1 清电量后延时
            OnInfo("清电量后延时...");
            Thread.Sleep(20000);
            #endregion

            #region 5.清电量后读电能底数
            //5.1 清电量后读电能底数
            OnInfo("清电量后读电能底数...");
            dlt645.GetEnergy(lstMeterIndex.ToArray(), Pulse.正向有功, out afterEnergys);
            #endregion

            return !IsStop;
        }
        /// 清需量
        /// <summary>
        /// 清需量
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="results">返回清需量结论</param>
        /// <param name="resultDescriptions">返回清需量结论描述</param>
        /// <param name="berforDemands">返回清需量前正向有功最大需量</param>
        /// <param name="afterDemands">返回清需量后正向有功最大需量</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        public override bool ClearDemand(float U, float acFreq, out bool[] results, out string[] resultDescriptions, out string[] berforDemands, out string[] afterDemands)
        {
            this.IsStop = false;

            #region 1.初始化系统通讯基本模块
            //1.1 设置电压电流量程
            OnInfo("设置电压电流量程...");
            power.SetRange(U, 0);
            Thread.Sleep(500);

            //1.2 切换多功能控制板
            OnInfo("切换多功能控制板...");
            powerSupply.SetPowerSupplyType(VerificationElementType.需量清零);
            Thread.Sleep(500);

            //1.3 升电压
            OnInfo("升电压...");
            bool hasUp = power.RaiseVoltage(U, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);

            //1.4 延时10秒
            if (hasUp)
            {
                OnInfo("延时10秒...");
                Thread.Sleep(SteadyTime * 1000);
            }
            #endregion

            #region 2.清需量前读清需量
            //2.1 清需量前读需量
            OnInfo("清需量前需量...");
            dlt645.GetDemand(lstMeterIndex.ToArray(), Pulse.正向有功, out berforDemands);
            #endregion

            #region 3.清需量
            //3.1 清需量
            OnInfo("清需量...");
            dlt645.ClearDemand(lstMeterIndex.ToArray(), out results, out resultDescriptions);
            CheckError("清需量", results);
            #endregion

            #region 4.清需量后延时
            //4.1 清需量后延时
            OnInfo("清需量后延时...");
            Thread.Sleep(10000);
            #endregion

            #region 5.清需量后读需量
            //5.1 清需量后读需量
            OnInfo("清需量后读需量...");
            dlt645.GetDemand(lstMeterIndex.ToArray(), Pulse.正向有功, out afterDemands);
            #endregion

            return !IsStop;
        }
        /// 读电能底数
        /// <summary>
        /// 读电能底数
        /// 驱动供应商自行根据电能表接线方式（单相、三相三线、三相四线）判断读数时段，如单相表，只读取总、峰、谷
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="pulse">脉冲类型，正向有功、反向有功、正向无功、反向无功</param>
        /// <param name="energyValues">返回电能读数结果</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        public override bool GetEnergyReading(float U, float acFreq, Pulse pulse, out EnergyValue[] energyValues)
        {
            this.IsStop = false;

            #region 1.初始化系统通讯基本模块
            //1.1 发送电压电流量程信息    IPower
            this.OnInfo("设置电压电流量程......");
            power.SetRange(U, 0);
            Thread.Sleep(500);

            //1.2 切换多功能控制板
            this.OnInfo("切换多功能控制板...");
            powerSupply.SetPowerSupplyType(VerificationElementType.读电能底数);
            Thread.Sleep(500);

            //1.3 升电压
            OnInfo("正在升电压...");
            bool hasUp = power.RaiseVoltage(U, LoadPhase.None, meter.WiringMode, (float)1.0, true, pulse);

            //1.4 延时10秒
            if (hasUp)
            {
                OnInfo("延时10秒...");
                Thread.Sleep(SteadyTime * 1000);
            }
            #endregion

            #region 2.读取电能底数
            // 2.1 读取电能底数
            OnInfo("读取电能底数");
            energyValues = new EnergyValue[MeterPositions.Length];
            string[] energys = null;
            dlt645.GetEnergy(lstMeterIndex.ToArray(), pulse, out energys);
            for (int i = 0; i < MeterPositions.Length; i++)
            {
                if (MeterPositions[i].IsVerify && !string.IsNullOrEmpty(energys[i]))
                {
                    energyValues[i] = new PhaseEnergy(energys[i]);
                }
            }
            #endregion

            return !IsStop;
        }
        /// 读需量底数
        /// <summary>
        /// 读需量底数
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="pulse">脉冲类型，正向有功、反向有功、正向无功、反向无功</param>
        /// <param name="demands">返回需量底数</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        public override bool GetDemandReading(float U, float acFreq, Pulse pulse, out string[] demands)
        {
            this.IsStop = false;

            #region 1.初始化系统通讯基本模块
            //1.1 发送电压电流量程信息    IPower
            this.OnInfo("设置电压电流量程......");
            power.SetRange(U, 0);
            Thread.Sleep(500);

            //1.2 切换多功能控制板
            this.OnInfo("切换多功能控制板...");
            powerSupply.SetPowerSupplyType(VerificationElementType.读需量底数);
            Thread.Sleep(500);

            //1.3 设置电压
            OnInfo("正在升电压...");
            bool hasUp = power.RaiseVoltage(U, LoadPhase.None, meter.WiringMode, (float)1.0, true, pulse);

            //1.4 延时10秒
            if (hasUp)
            {
                OnInfo("延时10秒...");
                Thread.Sleep(SteadyTime * 1000);
            }
            #endregion

            #region 2 读需量底数
            OnInfo("读需量底数");
            //2.1 读需量底数
            dlt645.GetDemand(lstMeterIndex.ToArray(), pulse, out demands);
            #endregion

            return !IsStop;
        }
        /// 读取表地址
        /// <summary>
        /// 读取表地址
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="addresses">返回表地址</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        public override bool ReadMeterAddress(float U, float acFreq, out string[] addresses)
        {
            this.IsStop = false;

            #region 1.初始化系统通讯基本模块
            //1.1 发送电压电流量程信息    IPower
            this.OnInfo("设置电压电流量程......");
            power.SetRange(U, 0);
            Thread.Sleep(100);

            //1.2 切换多功能控制板
            this.OnInfo("切换多功能控制板...");
            powerSupply.SetPowerSupplyType(VerificationElementType.读取表地址);
            Thread.Sleep(100);

            //1.3 设置电压
            OnInfo("正在升电压...");
            bool hasUp = power.RaiseVoltage(U, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);

            //1.4 延时10秒
            if (hasUp)
            {
                OnInfo("延时10秒...");
                Thread.Sleep(SteadyTime * 1000);
            }
            #endregion

            #region 2.获取电能表地址
            // 2.1 获取电能表地址
            OnInfo("获取电能表地址......");
            dlt645.GetAddress(lstMeterIndex.ToArray(), out addresses);
            #endregion

            return !IsStop;
        }
        /// <summary>
        /// 读取电能表生产编号
        /// </summary>
        /// <param name="U">电压</param>
        /// <param name="acFreq">频率</param>
        /// <param name="meterScbh">表生产编号</param>
        /// <returns></returns>
        public bool ReadMeterScbh(float U, float acFreq, out string[] meterScbhs)
        {
            this.IsStop = false;

            #region 1.初始化系统通讯基本模块
            //1.1 发送电压电流量程信息    IPower
            this.OnInfo("设置电压电流量程......");
            power.SetRange(U, 0);
            Thread.Sleep(500);

            //1.2 切换多功能控制板
            this.OnInfo("切换多功能控制板...");
            powerSupply.SetPowerSupplyType(VerificationElementType.读取表生产编号);
            Thread.Sleep(500);

            //1.3 设置电压
            OnInfo("正在升电压...");
            bool hasUp = power.RaiseVoltage(U, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);

            //1.4 延时10秒
            if (hasUp)
            {
                OnInfo("延时10秒...");
                Thread.Sleep(SteadyTime * 1000);
            }
            #endregion

            #region 2.获取电能表生产编号
            // 2.1 获取电能表地址
            OnInfo("获取电能表生产编号......");
            dlt645.GetScbh(lstMeterIndex.ToArray(), out meterScbhs);
            #endregion

            return !IsStop;
        }
        /// <summary>
        /// 打包参数下载表厂专用
        /// </summary>
        /// <param name="U">电压</param>
        /// <param name="acFreq">频率</param>
        /// <param name="Item_TxFrame">发送参数</param>
        /// <param name="Item_RxFrame">返回参数</param>
        /// <param name="meterResult">各表位结果</param>
        /// <returns></returns>
        public override bool DownParaToMeter(float U, float acFreq, string Item_TxFrame, string Item_RxFrame, out bool[] meterResult, out string[] meterRx)
        {
            this.IsStop = false;

            #region 1.初始化系统通讯基本模块
            //1.1 发送电压电流量程信息    IPower
            this.OnInfo("设置电压电流量程......");
            power.SetRange(U, 0);
            Thread.Sleep(500);

            //1.2 切换多功能控制板
            this.OnInfo("切换多功能控制板...");
            powerSupply.SetPowerSupplyType(VerificationElementType.读取表生产编号);
            Thread.Sleep(500);

            //1.3 设置电压
            OnInfo("正在升电压...");
            bool hasUp = power.RaiseVoltage(U, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);

            //1.4 延时10秒
            if (hasUp)
            {
                OnInfo("延时10秒...");
                Thread.Sleep(SteadyTime * 1000);
            }
            #endregion

            #region 2.获取电能表生产编号
            // 2.1 获取电能表地址
            OnInfo("获取电能表生产编号......");
            dlt645.DownCmdToMeter(Item_TxFrame, Item_RxFrame, lstMeterIndex.ToArray(), out meterResult, out meterRx);
            #endregion

            return !IsStop;
        }
        /// 预热
        /// <summary>
        /// 预热
        /// </summary>
        /// <param name="U">预热电压，单位V</param>
        /// <param name="I">预热电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="second">预热时间，单位秒</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        public override bool WarmUp(float U, float I, float acFreq, int second)
        {
            this.IsStop = false;

            #region 1.初始化系统通讯基本模块
            //1.1 发送电压电流量程信息    IPower
            this.OnInfo("设置电压电流量程......");
            power.SetRange(U, I);
            Thread.Sleep(500);

            //1.2 升电压
            OnInfo("正在升电压信息...");
            power.RaiseVoltage(U, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);
            Thread.Sleep(500);

            //1.3 升电流
            OnInfo("正在升电流信息...");
            power.RaiseCurrent(I, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);

            //1.3 延时10秒
            OnInfo("延时10秒...");
            Thread.Sleep(SteadyTime * 1000);
            #endregion

            #region 2.系统预热
            //2.1 系统预热
            int times = 1;
            while (true)
            {
                if (IsStop)
                {
                    break;
                }
                if (times <= second)
                {
                    OnInfo(string.Format("系统预热，需要{0}秒，已经进行{1}秒.", second, times));
                    times++;
                }
                else
                    break;
            }
            #endregion

            #region 3.停止检定
            //3.1、停止检定 Sd2000
            OnInfo("停止检定...");
            power.StopVerification();
            #endregion

            return !IsStop;
        }
        /// 点亮检验台的运行状态灯
        /// <summary>
        /// 点亮检验台的运行状态灯
        /// 由供应商根据硬件实现方式保存灯的状态；在收到下一个状态变更命令前，要维持原有状态
        /// </summary>
        /// <param name="color">状态灯颜色，目前支持，Color.Red:红，Color.Green:绿，Color.Yellow:黄</param>
        /// <param name="flicker">闪烁标志，true表示闪烁，false表示常亮</param>
        public override void Lighten(Color color, bool flicker)
        {
            // 点亮检验台的运行状态灯
            light.Light(color, flicker);
        }
        /// 读检定装置当前负载
        /// <summary>
        /// 读检定装置当前负载
        /// </summary>
        /// <param name="U">返回电压(单位：V)，长度为3的数组，U[0]表示A相，U[1]表示B相，U[2]表示C相，对于单向检定设备，U[0]表示电压，U[1],U[2]为null</param>
        /// <param name="I">返回电流(单位：A)，数组定义参考U</param>
        /// <param name="P">返回有功功率(单位：kW)，数组定义参考U</param>
        /// <param name="Q">返回无功功率(单位：kVar)，数组定义参考U</param>
        /// <param name="angle">返回电流相位角，以Ua为基准</param>
        /// <param name="acFreq">返回交流电频率(Hz)</param>
        public override void GetCurrentLoad(out string[] U, out string[] I, out string[] P, out string[] Q, out string[] angle, out string acFreq)
        {
            monitor.GetCurrentLoad(out U, out I, out P, out Q, out angle, out acFreq);
        }
        /// 表位压接
        /// <summary>
        /// 表位压接
        /// </summary>
        /// <param name="isPress">true表示进行电能表压接，false表示松开压接</param>
        /// <param name="results">每一个表位的压接情况，true表示压接成功，false表示压接失败</param>
        /// <param name="resultDescriptions">电能表压接情况描述，数组长度与result一致，对应每一个表位的压接情况描述</param>
        public override void EquipmentPress(bool isPress, out bool[] results, out string[] resultDescriptions)
        {
            controlPressMotor.EquipmentPress(isPress, out results, out resultDescriptions);
        }
        /// 表位翻转
        /// <summary>
        /// 表位翻转
        /// </summary>
        /// <param name="isReversal">true表示翻转成检定状态，false表示翻转成挂表状态</param>
        /// <param name="results">表位翻转情况，true表示翻转成功，false表示翻转失败</param>
        /// <param name="resultDescriptions">电能表表位翻转情况描述，数组长度与result一致，对应每一个表位的翻转情况</param>
        public override void EquipmentReversal(bool isReversal, out bool[] results, out string[] resultDescriptions)
        {
            controlReversalMotor.EquipmentReversal(isReversal, out results, out resultDescriptions);
        }
        /// 读检定装置表位状态
        /// <summary>
        /// 读检定装置表位状态
        /// </summary>
        /// <param name="meterNo">返回表位列表，对应检定台表位</param>
        /// <param name="status">返回表位状态，”00”为无表；”01”为有表</param>
        public override void GetCurrentMeterNoStatus(out int[] meterNo, out string[] status)
        {
            meterPositionStatus.GetCurrentMeterNoStatus(out meterNo, out status);
        }
        /// 获取检定台状态
        /// <summary>
        /// 获取检定台当前状态，表明当前检定台所处的状态，以确定是否可以执行后续相应的操作，可以在任意时刻调用该方法
        /// </summary>
        /// <returns>
        /// “00”:当前检定台处于故障状态，不允许进行操作。
        /// “01”:表示当前检定台处于待机状态，此时可以进行挂表操作。
        /// “02”:表示当前处于待压接状态，可以进行压接或下表操作
        /// “03”:表示当前处于压接状态，可以进行翻转或松开压接操作
        /// “04”:表示当前处于待检定状态，可以进行检定、或翻转操作
        /// “05”:表示当前处于检定状态，不允许进行翻转、松开压接操作
        /// </returns>
        public override string GetEquipmentStatus()
        {
            string equipStatus = "00"; // 默认故障状态
            MeterPositionPressStatus[] pressStatus;
            int[] meterIndexs;
            controlPressMotor.GetEquipmentPressStatus(MeterPositionCount, out meterIndexs, out pressStatus);
            for (int i = 0; i < pressStatus.Length; i++)
            {
                if (MeterPositions[i].Meter == null
                    || !MeterPositions[i].IsVerify)
                {
                    continue;
                }

                if (pressStatus[i] == MeterPositionPressStatus.故障
                    || pressStatus[i] == MeterPositionPressStatus.压接未到位)
                {
                    return equipStatus = "00";
                }
                else if (pressStatus[i] == MeterPositionPressStatus.未压接)
                {
                    return equipStatus = "02";
                }
                else
                {
                    equipStatus = "03";
                }
            }

            MeterPositionReverseStatus[] reverseStatus;
            controlReversalMotor.GetEquipmentReversalStatus(MeterPositionCount, out meterIndexs, out reverseStatus);
            for (int i = 0; i < reverseStatus.Length; i++)
            {
                if (MeterPositions[i].Meter == null
                    || !MeterPositions[i].IsVerify)
                {
                    continue;
                }

                if (reverseStatus[i] == MeterPositionReverseStatus.故障状态
                    || reverseStatus[i] == MeterPositionReverseStatus.翻转未到位状态)
                {
                    return equipStatus = "00";
                }
                else if (reverseStatus[i] == MeterPositionReverseStatus.倾斜状态)
                {
                    return equipStatus = "03";
                }
                else
                {
                    equipStatus = "04";
                }
            }

            return equipStatus;
        }
        /// 潜动、日计时试验
        /// <summary>
        /// 潜动、日计时试验
        /// </summary>
        /// <param name="latentClockError"></param>
        /// <param name="latentTimes"></param>
        /// <param name="returnSampleDatas"></param>
        /// <returns></returns>
        public override bool ExecuteLatentClockError(LatentClockError latentClockError, out string[] latentTimes, ReturnSampleDatasDelegate returnSampleDatas)
        {
            bool readLineOver = false;                            //读取当前点结束。
            bool readErrOver = false;                             //校验点执行完成
            int[] lastErrNO = new int[MeterPositionCount];        //上次误差次数。
            int[] sampleNo = new int[MeterPositionCount];         //采样次数记录 
            string[] errValue = new string[MeterPositionCount];   //采样误差数据
            latentTimes = new string[MeterPositions.Length];      // 潜动时间
            this.IsStop = false;
            float timeOut;
            calcTime.CalcClockErrorTime(latentClockError.U, latentClockError.AcFreq, latentClockError.clockFreq, latentClockError.SampleSecond, latentClockError.count, out timeOut);

            #region 1.初始化系统通讯基本模块

            //1.1 设置电压电流量程
            OnInfo("设置电压电流量程......");
            power.SetRange(latentClockError.U, 0);
            Thread.Sleep(500);

            #endregion

            #region 2.校验参数设置
            //2.1 初始化误差板  
            this.OnInfo("初始化误差板......");
            wcfk.InitWcfk();

            //2.2 设置检定类型  
            this.OnInfo("设置检定类型......");
            wcfk.SetVerificationType(VerificationElementType.潜动日计时试验);

            //2.3 设置检定脉冲方式  
            this.OnInfo("设置检定脉冲方式...");
            power.SetVerificationPulseType(VerificationElementType.潜动日计时试验);
            Thread.Sleep(500);

            //2.4 设置脉冲方向  
            this.OnInfo("设置脉冲方向......");
            wcfk.SetPulseType(Pulse.正向有功);
            Thread.Sleep(500);

            //2.6 设置脉冲通道  
            this.OnInfo("设置脉冲通道......");
            wcfk.SetPulseChannel(VerificationElementType.潜动日计时试验, Pulse.正向有功);

            //2.6 设置被检表常数
            this.OnInfo("设置被检表常数...");
            wcfk.SetMeterConst(meter.Const);

            //2.7 设置检定时间  
            this.OnInfo("设置检定时间......");
            wcfk.SetClockErrorTime(latentClockError.SampleSecond, VerificationElementType.潜动日计时试验);

            //2.8 设置时钟通道  
            this.OnInfo("设置时钟通道......");
            timeChannel.SetTimeChannel(0);

            //2.9 启动误差版
            this.OnInfo("启动误差版......");
            wcfk.StartWcfk();

            //2.10 设置标准表接线方式、电能指示
            this.OnInfo("设置标准表接线方式、电能指示......");
            stdMeter.SetStdMeterWiringMode(VerificationElementType.潜动日计时试验, meter.WiringMode, Pulse.正向有功);

            //2.11 切换CT档位 
            this.OnInfo("切换CT档位...");
            currentControl.SetCurrentControl(0f);
            Thread.Sleep(500);

            //2.12 切换多功能控制板 
            this.OnInfo("切换多功能控制板...");
            powerSupply.SetPowerSupplyType(VerificationElementType.潜动日计时试验);
            Thread.Sleep(500);

            //2.13 升电压
            this.OnInfo("升电压......");
            this.OnInfo("正在升电压信息...");
            bool hasUp = power.RaiseVoltage(latentClockError.U, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);
            Thread.Sleep(500);

            //2.15 延时10秒
            if (hasUp)
            {
                this.OnInfo("延时10秒......");
                Thread.Sleep(SteadyTime * 1000);
            }
            #endregion

            #region 3.设置多功能端子
            bool[] results = new bool[MeterPositionCount];
            string[] resultDescription;
            // 3.1 设置多功能端子
            this.OnInfo("设置多功能端子...");
            dlt645.SetMulTerminalOut(lstMeterIndex.ToArray(), VerificationElementType.潜动日计时试验, out results, out resultDescription);
            CheckError("设置多功能端子", results);
            #endregion

            #region 4.开始检定并读取误差
            // 4.1 开始检定
            this.OnInfo("开始检定...");
            wcfk.StartVerification(VerificationElementType.潜动日计时试验);

            this.OnInfo("开始潜动...");
            wcfk.LatentStart();

            // 4.2 读取误差
            this.OnInfo("读取误差...");
            int sampleIndex = 1;
            bool finish = false;                        // 潜动试验完成标志
            int startTime = Environment.TickCount;      // 潜动开始时间
            int clockStartTime = Environment.TickCount; // 日计时开始时间
            while (true)
            {
                if (IsStop)
                {
                    break;
                }
                Thread.Sleep(1000);

                #region 获取日计时误差
                //说明该校验点检定还未完成
                if (sampleIndex <= latentClockError.count)
                {
                    for (int i = 0; i < MeterPositions.Length; i++)
                    {
                        if (!IsStop
                            && MeterPositions[i].IsVerify
                            && string.IsNullOrEmpty(errValue[i]))
                        {
                            this.OnInfo(string.Format("正在读取第{0}表位误差……", i + 1));
                            ErrorData errorData = wcfk.ReadErrorData(i + 1, sampleIndex);
                            if (errorData != null
                                && lastErrNO[i] != errorData.SampleIndex
                                && !string.IsNullOrEmpty(errorData.ErrorValue))
                            {
                                sampleNo[i]++;
                                lastErrNO[i] = errorData.SampleIndex;
                                errValue[i] = errorData.ErrorValue;
                            }
                        }
                    }
                    //判断是否所有表位都读取成功，必须要将所有选中的表位读取成功，才能回传。
                    readLineOver = true;
                    if ((((Environment.TickCount - clockStartTime) / 1000) + 1) < timeOut + (sampleIndex == 1 ? 30 : 0))
                    {
                        for (int i = 0; i < MeterPositions.Length; i++)
                        {
                            if (string.IsNullOrEmpty(errValue[i])
                                && MeterPositions[i].IsVerify)
                            {
                                readLineOver = false;
                                break;
                            }
                        }
                    }
                    //如果所有表位都读取到数据则进行返回，这儿也可以不用判断，每次采样进行多次返回，这样在界面上可以看到那些表位没有获取到数据
                    if (readLineOver)
                    {
                        returnSampleDatas(sampleIndex,errValue);
                        errValue = new string[MeterPositions.Length];//发送结束后，在进行重新分配。
                        sampleIndex++;
                        clockStartTime = Environment.TickCount;
                    }
                }
                #endregion

                #region 获取潜动脉冲
                if ((Environment.TickCount - startTime) / 1000 > (latentClockError.LatentSecond - 120))
                {
                    finish = true;      // 潜动试验完成标志
                    for (int i = 0; i < MeterPositions.Length; i++)
                    {
                        if (!IsStop && (MeterPositions[i].IsVerify))
                        {
                            if (string.IsNullOrEmpty(latentTimes[i]))
                            {
                                OnInfo(string.Format("正在读取第{0}表位潜动脉冲……", i + 1));
                                PulseValue pulseValue = wcfk.ReadLatentPulse(i + 1);
                                if (pulseValue.Count > 0)
                                {
                                    latentTimes[i] = ((Environment.TickCount - startTime) / 1000).ToString("f2");
                                }
                                else
                                {
                                    finish = false;
                                }
                            }
                        }
                    }
                }
                #endregion

                // 潜动实验结束或者超时 且 日计时采样次数大于采样值，实验结束。
                if ((finish || ((Environment.TickCount - startTime) / 1000 > latentClockError.LatentSecond))
                        && sampleIndex > latentClockError.count)
                {
                    readErrOver = true;
                    break;
                }
            }
            #endregion

            #region 5.停止检定设置
            //5.1 停止误差版
            this.OnInfo("停止误差版...");
            wcfk.StopWcfk();

            //5.2 停止标准表
            this.OnInfo("停止标准表...");
            stdMeter.StopStdMeter();

            //5.3 停止检定
            OnInfo("停止检定");
            power.StopVerification();
            #endregion

            return readErrOver;
        }
        /// 脱机
        /// <summary>
        /// 脱机
        /// </summary>
        public override void Close()
        {
            //停止误差版
            this.OnInfo("停止误差版...");
            wcfk.StopWcfk();

            //停止标准表
            this.OnInfo("停止标准表...");
            stdMeter.StopStdMeter();

            //降电压
            OnInfo("正在降电压信息...");
            power.RaiseVoltage(0, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);
            Thread.Sleep(500);

            //降电流
            OnInfo("正在降电流信息...");
            power.RaiseCurrent(0, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);

            //延时5秒
            OnInfo("延时5秒...");
            Thread.Sleep(5000);
        }
        /// 耐压试验
        /// <summary>
        /// 设备供应商根据试验参数，执行耐压试验，并返回耐压试验结论
        /// </summary>
        /// <param name="resistanceU">耐压试验测试点电压，单位KV</param>
        /// <param name="resistanceI">漏电流档位，单位mA</param>
        /// <param name="resistanceTime">耐压试验时间，单位秒</param>
        /// <param name="resistanceType">耐压方式：”00”对外壳打耐压；”01”对辅助端子打耐压；”02”对外壳和辅助端子打耐压</param>
        /// <param name="results">返回耐压试验结果</param>
        /// <param name="resultDescriptions">返回耐压试验结果描述</param>
        /// <returns></returns>
        public override bool ExecuteResistance(float resistanceU, float resistanceI, int resistanceTime, string resistanceType, out bool[] results, out string[] resultDescriptions)
        {
            this.IsStop = false;

            #region 1.初始化电压电流
            //耐压试验时候不能有电压和电流,否则会导致设备损伤
            //1.1 降电压/降电流
            this.OnInfo("降电压、电流...");
            power.RaiseVotageCurrent(0f, 0f, LoadPhase.None, meter.WiringMode, 1f, true, Pulse.正向有功);
            Thread.Sleep(500);

            //1.2 延时10秒
            this.OnInfo("延时10秒...");
            Thread.Sleep(10000);
            #endregion

            #region 2.校验参数设置
            // 2.1 初始化误差板
            this.OnInfo("初始化误差板...");
            wcfk.InitWcfk();

            //2.2 打开耐压设备
            this.OnInfo("打开耐压设备...");
            controlResistancePower.RemoteOpenResistanceEquipment();
            Thread.Sleep(500);

            //2.3 初始化耐压设备
            this.OnInfo("初始化耐压设备...");
            resistance.InitResistancee();
            Thread.Sleep(500);

            //2.4 设置电机模块为耐压模式
            this.OnInfo("设置电机模块为耐压模式...");
            controlResistanceMoto.SetMotoContorlToResistance(true);
            Thread.Sleep(10000);

            //2.5 误差版耐压阀值
            this.OnInfo("设置耐压阀值...");
            resistanceWcfk.SetResistanceIWcfkRangle(resistanceI);

            //2.6 切换CT档位 
            this.OnInfo("切换CT档位...");
            currentControl.SetCurrentControl(0f);
            Thread.Sleep(500);

            //2.7 切换多功能控制板 
            this.OnInfo("切换多功能控制板...");
            powerSupply.SetPowerSupplyType(VerificationElementType.耐压试验);
            Thread.Sleep(500);

            //2.8 耐压仪复位状态
            this.OnInfo("耐压仪复位状态...");
            resistance.RequestReset();

            //2.9 设置漏电流
            this.OnInfo("设置漏电流...");
            resistance.SetResistanceI(resistanceI);
            Thread.Sleep(500);

            //2.10 设置耐压方式
            this.OnInfo("设置耐压方式...");
            resistance.SetResistanceType(resistanceType);
            Thread.Sleep(500);

            //2.11 设置耐压测试时间
            this.OnInfo("设置耐压测试时间...");
            resistance.SetResistanceTime(resistanceTime);
            Thread.Sleep(500);

            //2.12 设置电压
            this.OnInfo("设置电压...");
            resistance.SetResistanceU(resistanceU);
            Thread.Sleep(500);

            // 2.13 设置检定类型
            this.OnInfo("设置检定类型...");
            wcfk.SetVerificationType(VerificationElementType.耐压试验);
            #endregion

            #region 3.开始试验并读取结果
            //3.1 开始试验
            this.OnInfo("开始试验...");
            wcfk.StartVerification(VerificationElementType.耐压试验);

            //3.2 启动耐压设备
            this.OnInfo("启动耐压设备...");
            resistance.StartResistance();

            //3.3 耐压等待
            int startTime = Environment.TickCount;
            do
            {
                if (IsStop)
                {
                    break;
                }
                Thread.Sleep(1000);
                OnInfo(string.Format("耐压进行中，请稍等，已经进行{0}秒，一共需要{1}秒...", (Environment.TickCount - startTime) / 1000, resistanceTime + 5));
            } while ((Environment.TickCount - startTime) / 1000 < (resistanceTime + 5));//多给5秒，用来误差板进行内部判断

            //3.3 停止耐压
            this.OnInfo("停止耐压...");
            resistance.StopResistance();

            //3.4 设置电机模块退出耐压模式
            this.OnInfo("设置电机模块退出耐压模式...");
            controlResistanceMoto.SetMotoContorlToResistance(false);
            Thread.Sleep(5000);

            //3.5 读取结果
            this.OnInfo("读取耐压结果...");
            resistanceWcfk.ReadResistanceResult(lstMeterIndex.ToArray(), out results, out resultDescriptions);
            #endregion

            #region 4.停止检定

            //4.1 停止误差版
            this.OnInfo("停止误差版...");
            wcfk.StopWcfk();

            //4.2 关闭耐压设备
            this.OnInfo("关闭耐压设备...");
            controlResistancePower.RemoteCloseResistanceEquipment();
            Thread.Sleep(500);

            //4.3 重新打开设备
            this.OnInfo("重新打开设备...");
            controlEquipmentPower.RemoteOpenEquipment();
            #endregion

            return (!IsStop);
        }
        /// 远程拉合闸
        /// <summary>
        /// 远程拉合闸
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="openResults">返回拉闸结论</param>
        /// <param name="closeResults">返回合闸结论</param>
        /// <param name="resultDescriptions">返回远程拉合闸结果描述</param>
        /// <param name="beforeOpenStates">返回拉闸前状态字</param>
        /// <param name="afterOpenStates">返回拉闸后状态字</param>
        /// <param name="beforeCloseStates">返回合闸前状态字</param>
        /// <param name="afterCloseStates">返回合闸后状态字</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        public override bool RemoteKnifeSwitch(float U, float acFreq, out bool[] openResults, out bool[] closeResults, out string[] resultDescriptions, out string[] beforeOpenStates, out string[] afterOpenStates, out string[] beforeCloseStates, out string[] afterCloseStates)
        {
            // 变量初始化
            openResults = new bool[MeterPositions.Length];
            closeResults = new bool[MeterPositions.Length];
            resultDescriptions = new string[MeterPositions.Length];
            beforeOpenStates = new string[MeterPositions.Length];
            afterOpenStates = new string[MeterPositions.Length];
            beforeCloseStates = new string[MeterPositions.Length];
            afterCloseStates = new string[MeterPositions.Length];

            #region 1.初始化系统通讯基本模块
            //1.1 发送电压电流量程信息
            this.OnInfo("设置电压电流量程......");
            power.SetRange(U, 0);
            Thread.Sleep(500);

            //1.2 切换多功能控制板
            this.OnInfo("切换多功能控制板...");
            powerSupply.SetPowerSupplyType(VerificationElementType.常数校核试验);
            Thread.Sleep(500);

            //1.3 升电压   IPower
            this.OnInfo("升电压......");
            bool hasUp = power.RaiseVoltage(U, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);

            //1.4 延时10秒 
            if (hasUp)
            {
                this.OnInfo("延时10秒......");
                Thread.Sleep(SteadyTime * 1000);
            }
            #endregion

            #region 2.校验参数设置
            //2.1 初始化误差板  
            this.OnInfo("初始化误差板......");
            wcfk.InitWcfk();
            #endregion

            #region 3.读取跳闸前状态字、联接网络加密机、跳闸、跳闸后状态字
            // 3.1 读取跳闸前状态字
            dlt645.GetKnifeSwitchStatus(lstMeterIndex.ToArray(), out beforeOpenStates);

            // 3.2 联接网络加密机
            OnInfo(string.Format("正在连接网络加密机IP：{0},端口{1}", ESAM_IP, ESAM_PORT));
            if (!gChkESAM.ConnectEncryptionMachine(ESAM_IP, ESAM_PORT, ESAM_PWD.Length, ESAM_PWD, Netencryption)) //连接加密机
            {
                OnInfo("连接加密机失败，请检查网络。");
                return false;
            }

            // 3.3 获取随机数1和密文1
            OnInfo("正在获取随机数1和密文1...");
            StringBuilder outrand1 = new StringBuilder(16);         //随机数1
            StringBuilder outpwd1 = new StringBuilder(16);          //密文1
            if (!gChkESAM.GetRand1_Pwd1(0, putdiv, outrand1, outpwd1, Netencryption))   //获取随机数1和密文1成功
            {
                OnInfo("获取随机数1和密文1失败。");
                return false;
            }

            // 3.4 获取随机数2和esam
            OnInfo("正在进行身份认证...");
            string[] outrand2 = new string[MeterPositions.Length];  //随机数2
            string[] ESAM = new string[MeterPositions.Length];      //esam序列号
            dlt645.SetSecurityCertificate(lstMeterIndex.ToArray(), putdiv, outrand1.ToString(), outpwd1.ToString(), out outrand2, out ESAM, Netencryption);

            // 3.5 跳闸
            dlt645.UserControlOpenAlarm(lstMeterIndex.ToArray(), outrand2, ESAM, (int)Dlt645_2007OpenAlarm.跳闸, Netencryption, out openResults);
            for (int i = 0; i < MeterPositions.Length; i++)
            {
                if ((MeterPositions[i].IsVerify) && (!IsStop))
                {
                    resultDescriptions[i] = "跳闸" + (openResults[i] == true ? ("成功\r") : ("失败\r"));
                }
            }

            // 3.6 读取跳闸后状态字
            dlt645.GetKnifeSwitchStatus(lstMeterIndex.ToArray(), out afterOpenStates);
            #endregion

            #region 4.读取合闸前状态字、合闸、断开网络加密机、合闸后状态字
            // 4.1 读取合闸前状态字
            for (int i = 0; i < afterOpenStates.Length; i++)
            {
                beforeCloseStates[i] = afterOpenStates[i];
            }

            // 4.2 获取随机数1和密文1
            OnInfo("正在获取随机数1和密文1...");
            if (!gChkESAM.GetRand1_Pwd1(0, putdiv, outrand1, outpwd1, Netencryption))   //获取随机数1和密文1成功
            {
                OnInfo("获取随机数1和密文1失败。");
                return false;
            }

            // 4.3 获取随机数2和esam
            OnInfo("正在进行身份认证...");
            dlt645.SetSecurityCertificate(lstMeterIndex.ToArray(), putdiv, outrand1.ToString(), outpwd1.ToString(), out outrand2, out ESAM, Netencryption);

            // 4.4 合闸
            dlt645.UserControlOpenAlarm(lstMeterIndex.ToArray(), outrand2, ESAM, (int)Dlt645_2007OpenAlarm.合闸, Netencryption, out openResults);
            for (int i = 0; i < MeterPositions.Length; i++)
            {
                if ((MeterPositions[i].IsVerify) && (!IsStop))
                {
                    resultDescriptions[i] = "合闸" + (openResults[i] == true ? ("成功\r") : ("失败\r"));
                }
            }

            // 4.5 断开网络加密机
            gChkESAM.CloseEncryptionMachine(Netencryption);

            // 4.6 读取合闸后状态字
            dlt645.GetKnifeSwitchStatus(lstMeterIndex.ToArray(), out afterCloseStates);
            #endregion

            return true;
        }
        /// 远程密钥更新
        /// <summary>
        /// 远程密钥更新
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="isRemote">远程表或本地表，true:远程表，false:本地表</param>
        /// <param name="results">返回远程密钥更新结果</param>
        /// <param name="resultDescriptions">返回远程密钥更新结果描述</param>
        /// <param name="beforeInfos">返回密钥更新前后信息</param>
        /// <param name="afterInfos">返回密钥更新后信息</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        public override bool RemoteSecretKeyUpdate(float U, float acFreq, bool isRemote, out bool[] results, out string[] resultDescriptions, out string[] beforeInfos, out string[] afterInfos)
        {
            byte[] Arr = new byte[8];                               //参数
            resultDescriptions = new string[MeterPositions.Length];
            results = new bool[MeterPositions.Length];
            beforeInfos = new string[MeterPositions.Length];
            afterInfos = new string[MeterPositions.Length];

            #region 1.初始化系统通讯基本模块
            //1.1 发送电压电流量程信息
            this.OnInfo("设置电压电流量程......");
            power.SetRange(U, 0);
            Thread.Sleep(500);

            //1.2 切换多功能控制板
            this.OnInfo("切换多功能控制板...");
            powerSupply.SetPowerSupplyType(VerificationElementType.常数校核试验);
            Thread.Sleep(500);

            //1.3 升电压   IPower
            this.OnInfo("升电压......");
            bool hasUp = power.RaiseVoltage(U, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);

            //1.4 延时10秒 
            if (hasUp)
            {
                this.OnInfo("延时10秒......");
                Thread.Sleep(SteadyTime * 1000);
            }
            #endregion

            #region 2.校验参数设置
            //2.1 初始化误差板  
            this.OnInfo("初始化误差板......");
            wcfk.InitWcfk();
            #endregion

            #region 3.联接网络加密机、身份验证、召测密钥版本信息
            // 3.1 联接网络加密机
            OnInfo(string.Format("正在连接网络加密机IP：{0},端口{1}", ESAM_IP, ESAM_PORT));
            if (!gChkESAM.ConnectEncryptionMachine(ESAM_IP, ESAM_PORT, ESAM_PWD.Length, ESAM_PWD, Netencryption)) //连接加密机
            {
                OnInfo("连接加密机失败，请检查网络。");
                return false;
            }

            // 3.2 获取随机数1和密文1
            OnInfo("正在获取随机数1和密文1...");
            StringBuilder outrand1 = new StringBuilder(16);         //随机数1
            StringBuilder outpwd1 = new StringBuilder(16);          //密文1
            if (!gChkESAM.GetRand1_Pwd1(0, putdiv, outrand1, outpwd1, Netencryption))   //获取随机数1和密文1成功
            {
                OnInfo("获取随机数1和密文1失败。");
                return false;
            }

            // 3.3 获取随机数2和esam
            OnInfo("正在进行身份认证...");
            string[] outrand2 = new string[MeterPositions.Length];  //随机数2
            string[] ESAM = new string[MeterPositions.Length];      //esam序列号
            dlt645.SetSecurityCertificate(lstMeterIndex.ToArray(), putdiv, outrand1.ToString(), outpwd1.ToString(), out outrand2, out ESAM, Netencryption);

            // 3.4 召测密钥版本信息
            OnInfo("召测密钥版本信息...");
            int ReadLen = 4;
            int BeginAddr = 0;
            int FileID = 6;
            dlt645.GetDataBackToCopy(lstMeterIndex.ToArray(), FileID, BeginAddr, ReadLen, out beforeInfos, Netencryption);//数据回抄密钥信息。

            #endregion

            #region 4.密钥更新前准备、加密
            // 4.1 密钥更新前准备、加密
            int intConnectOk = -1;
            StringBuilder[] OutKey1 = new StringBuilder[MeterPositions.Length];//输出的主控密钥密文,字符型，长度 64
            StringBuilder[] OutKeyinfo1 = new StringBuilder[MeterPositions.Length];//输出的主控密钥信息,字符型，长度 8
            StringBuilder[] OutKey2 = new StringBuilder[MeterPositions.Length];//输出的远程控制密钥密文,字符型，长度 64
            StringBuilder[] OutKeyinfo2 = new StringBuilder[MeterPositions.Length];//输出的远程控制密钥信息,字符型，长度 8
            StringBuilder[] OutKey3 = new StringBuilder[MeterPositions.Length];//输出的参数设置密钥密文,字符型，长度 64
            StringBuilder[] OutKeyinfo3 = new StringBuilder[MeterPositions.Length];//输出的参数设置密钥信息,字符型，长度 8
            StringBuilder[] OutKey4 = new StringBuilder[MeterPositions.Length];//输出的身份认证密钥密文,字符型，长度 64
            StringBuilder[] OutKeyinfo4 = new StringBuilder[MeterPositions.Length];//输出的身份认证密钥信息,字符型，长度 8

            string KeyKind1 = "01010401";//表示输入的主控密钥密钥信息明文,字符型
            string KeyKind2 = "01010202";//表示输入的远程控制密钥信息明文,字符型
            string KeyKind3 = "01010103";//表示输入的二类参数设置密钥信息明文,字符型
            string KeyKind4 = "01010304";//表示输入的远程身份认证密钥信息明文,字符型
            for (int i = 0; i < MeterPositions.Length; i++)
            {
                try
                {
                    if (IsStop)
                    {
                        OnInfo("用户停止……");
                        return false;
                    }
                    OutKey1[i] = new StringBuilder(64);
                    OutKeyinfo1[i] = new StringBuilder(8);
                    OutKey2[i] = new StringBuilder(64);
                    OutKeyinfo2[i] = new StringBuilder(8);
                    OutKey3[i] = new StringBuilder(64);
                    OutKeyinfo3[i] = new StringBuilder(8);
                    OutKey4[i] = new StringBuilder(64);
                    OutKeyinfo4[i] = new StringBuilder(8);
                    if ((MeterPositions[i].IsVerify) && (!string.IsNullOrEmpty(outrand2[i]))
                        && (!string.IsNullOrEmpty(ESAM[i])) && (!string.IsNullOrEmpty(beforeInfos[i])))
                    {
                        if (beforeInfos[i].Substring(6, 2) != "04")
                        {
                            OnInfo(string.Format("正在对第{0}表位连接远程加密机进行密钥更新...", (i + 1)));
                            intConnectOk = gSJJ1009Server.KeyUpdate(0, outrand2[i], MeterPositions[i].Meter.Address.PadLeft(16, '0'), ESAM[i],
                                            KeyKind1, KeyKind2, KeyKind3, KeyKind4,
                                            OutKey1[i], OutKeyinfo1[i], OutKey2[i], OutKeyinfo2[i],
                                            OutKey3[i], OutKeyinfo3[i], OutKey4[i], OutKeyinfo4[i]);
                            if (intConnectOk != 0)
                            {
                                OnInfo("密钥更新不成功，错误代码：" + intConnectOk + "，KeyUpdate失败。");
                            }
                            else
                                Thread.Sleep(5);
                        }
                    }
                }
                catch
                {
                    MeterPositions[i].IsVerify = false;
                    OnInfo("密钥更新不成功，错误代码：" + intConnectOk + "，KeyUpdate失败。");
                }
            }

            #endregion

            #region 5.密钥更新
            // 5.1 主控密钥更新
            dlt645.MainSecurityUpdate(lstMeterIndex.ToArray(), beforeInfos, KeyKind1, OutKey1.ToString(),
                OutKeyinfo1.ToString(), out results, out resultDescriptions);
            CheckError("主控密钥更新", results);

            // 5.2 控制命令密钥更新
            dlt645.ControlSecurityUpdate(lstMeterIndex.ToArray(), beforeInfos, KeyKind2, OutKey2.ToString(),
                OutKeyinfo2.ToString(), out results, out resultDescriptions);
            CheckError("控制命令密钥更新", results);

            // 5.3 参数命令密钥更新
            dlt645.ArguSecurityUpdate(lstMeterIndex.ToArray(), beforeInfos, KeyKind3, OutKey3.ToString(),
                OutKeyinfo3.ToString(), out results, out resultDescriptions);
            CheckError("参数命令密钥更新", results);

            // 5.4 身份认证命令密钥更新
            dlt645.AuthSecurityUpdate(lstMeterIndex.ToArray(), beforeInfos, KeyKind4, OutKey4.ToString(),
                OutKeyinfo4.ToString(), out results, out resultDescriptions);
            CheckError("身份认证命令密钥更新", results);
            #endregion

            #region 6.更新后进行验证，断开加密机
            // 6.1 获取随机数1和密文1
            OnInfo("正在获取随机数1和密文1...");
            string[] outrand1s = new string[MeterPositions.Length];         //随机数1
            string[] outpwd1s = new string[MeterPositions.Length];          //密文1
            for (int emno = 0; emno < MeterPositions.Length; emno++)
            {
                if (MeterPositions[emno].IsVerify)
                {
                    OnInfo(string.Format("正在进行第{0}表位私钥身份认证...", emno + 1));
                    if (!gChkESAM.GetRand1_Pwd1(1, MeterPositions[emno].Meter.Address.PadLeft(16, '0'), outrand1, outpwd1, Netencryption))//获取随机数1和密文1成功
                        return false;
                    outrand1s[emno] = outrand1.ToString();
                    outpwd1s[emno] = outpwd1.ToString();
                    Thread.Sleep(80);
                }
            }

            // 6.2 获取随机数2和esam
            OnInfo("正在进行身份认证...");
            dlt645.SetSecurityCertificate(lstMeterIndex.ToArray(), outrand1s, outpwd1s, out outrand2, out ESAM, Netencryption);

            // 6.3 召测密钥版本信息
            OnInfo("召测密钥版本信息...");
            dlt645.GetDataBackToCopy(lstMeterIndex.ToArray(), FileID, BeginAddr, ReadLen, out afterInfos, Netencryption);//数据回抄密钥信息。

            // 6.4 断开加密机
            gChkESAM.CloseEncryptionMachine(Netencryption);
            #endregion

            return true;
        }
        /// 获取检定装置表位压接状态
        /// <summary>
        /// 获取检定装置表位压接状态
        /// </summary>
        /// <param name="meterNo">返回表位列表，对应检定台表位</param>
        /// <param name="pressStatus">返回表位压接状态</param>
        /// <returns></returns>
        public override void GetEquipmentPressStatus(out int[] meterNo, out MeterPositionPressStatus[] pressStatus)
        {
            controlPressMotor.GetEquipmentPressStatus(MeterPositionCount, out meterNo, out pressStatus);
        }
        /// 获取检定装置表位翻转状态
        /// <summary>
        /// 获取检定装置表位翻转状态
        /// </summary>
        /// <param name="meterNo">返回表位列表，对应检定台表位</param>
        /// <param name="reverseStatus">返回表位翻转状态</param>
        /// <returns></returns>
        public override void GetEquipmentReversalStatus(out int[] meterNo, out MeterPositionReverseStatus[] reverseStatus)
        {
            controlReversalMotor.GetEquipmentReversalStatus(MeterPositionCount, out meterNo, out reverseStatus);
        }
        /// 表位手工压接
        /// <summary>
        /// 表位手工压接
        /// </summary>
        /// <param name="isPress">true表示进行电能表压接，false表示松开压接</param>
        /// <param name="results">每一个表位的压接情况，true表示压接成功，false表示压接失败</param>
        /// <param name="resultDescriptions">电能表压接情况描述，数组长度与result一致，对应每一个表位的压接情况描述</param>
        public override void EquipmentManualPress(int meterPositionCount, bool isPress, out bool[] results, out string[] resultDescriptions)
        {
            controlPressMotor.EquipmentManualPress(MeterPositionCount, isPress, out results, out resultDescriptions);
        }
        /// 表位手工翻转
        /// <summary>
        /// 表位手工翻转
        /// </summary>
        /// <param name="isReversal">true表示翻转成检定状态，false表示翻转成挂表状态</param>
        /// <param name="results">表位翻转情况，true表示翻转成功，false表示翻转失败</param>
        /// <param name="resultDescriptions">电能表表位翻转情况描述，数组长度与result一致，对应每一个表位的翻转情况</param>
        public override void EquipmentManualReversal(int meterPositionCount, bool isReversal, out bool[] results, out string[] resultDescriptions)
        {
            controlReversalMotor.EquipmentManualReversal(MeterPositionCount, isReversal, out results, out resultDescriptions);
        }
        /// 单个表位手工压接
        /// <summary>
        /// 单个表位手工压接
        /// </summary>
        /// <param name="meterPostion">表位号</param>
        /// <param name="isPress">true表示进行电能表压接，false表示松开压接</param>
        /// <param name="results">每一个表位的压接情况，true表示压接成功，false表示压接失败</param>
        /// <param name="resultDescriptions">电能表压接情况描述，数组长度与result一致，对应每一个表位的压接情况描述</param>
        public override void EquipmentMeterPositionManualPress(int meterPositionCount, int[] meterPositions, bool isPress, out bool[] results, out string[] resultDescriptions)
        {
            controlPressMotor.EquipmentMeterPositionManualPress(meterPositionCount, meterPositions, isPress, out results, out resultDescriptions);
        }
        /// 单个表位手工翻转
        /// <summary>
        /// 单个表位手工翻转
        /// </summary>
        /// <param name="meterPostion">表位号</param>
        /// <param name="isReversal">true表示翻转成检定状态，false表示翻转成挂表状态</param>
        /// <param name="results">表位翻转情况，true表示翻转成功，false表示翻转失败</param>
        /// <param name="resultDescriptions">电能表表位翻转情况描述，数组长度与result一致，对应每一个表位的翻转情况</param>
        public override void EquipmentMeterPositionManualReversal(int meterPositionCount, int[] meterPositions, bool isReversal, out bool[] results, out string[] resultDescriptions)
        {
            controlReversalMotor.EquipmentMeterPositionManualReversal(meterPositionCount, meterPositions, isReversal, out results, out resultDescriptions);
        }
        /// 电流开路检测
        /// <summary>
        /// 电流开路检测
        /// </summary>
        /// <param name="current">电流值（基本电流）</param>
        /// <param name="results">返回电流开路检测结论</param>
        public override void ExecuteCurrentTest(float current, out bool[] resutls)
        {
            #region 1.初始化系统通讯基本模块
            //1.1 发送电压电流量程信息    IPower
            this.OnInfo("设置电压电流量程......");
            power.SetRange(220, 0);
            Thread.Sleep(500);


            this.OnInfo("初始化误差板参数......");
            wcfk.InitWcfk();
            Thread.Sleep(500);


            //1.2 切换多功能控制板
            this.OnInfo("切换多功能控制板...");
            powerSupply.SetPowerSupplyType(VerificationElementType.基本误差试验);
            Thread.Sleep(500);

            this.OnInfo("切换CT档位...");
            currentControl.SetCurrentControl(current);
            Thread.Sleep(500);
            //2.15 升电压
            this.OnInfo("正在升电压信息...");
            power.RaiseVoltage(220, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);
            Thread.Sleep(500);

            //2.16 升电流
            this.OnInfo("正在升电流信息...");
            bool hasUp = power.RaiseCurrent(current, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);
            Thread.Sleep(500);

            //1.4 延时10秒
            if (hasUp)
            {
                OnInfo("延时10秒...");
                Thread.Sleep(SteadyTime * 1000);
            }
            #endregion
            wcfk.CurrentTest(current, out resutls);
        }
        /// 重压接
        /// <summary>
        /// 重压接
        /// </summary>
        /// <param name="meterPositions">表位</param>
        /// <param name="results">结论</param>
        /// <param name="resultDescriptions">结论描述</param>
        public override void ExecuteSecondPress(int[] meterPositions, out bool[] results)
        {
            int[] meterNo;
            results = new bool[MeterPositionCount];
            MeterPositionReverseStatus[] reverseStatus;
            controlReversalMotor.GetEquipmentReversalStatus(MeterPositionCount, out meterNo, out reverseStatus);
            if (reverseStatus != null
                && reverseStatus.Where(item => item == MeterPositionReverseStatus.倾斜状态).Count() == reverseStatus.Length)
            {
                string[] resultDescriptions;
                controlPressMotor.EquipmentMeterPositionSecondPress(meterPositions, out results, out resultDescriptions);
            }
        }
        /// 表位自动短接
        /// <summary>
        /// 表位自动短接
        /// </summary>
        /// <param name="meterPositions">表位号</param>
        /// <param name="results">短接返回结果</param>
        /// <returns></returns>
        public override void CurrentShout(int[] meterPositions, out bool[] results)
        {
            wcfk.CurrentShout(meterPositions, out results);

            // 临时处理，设置MeterPositions中的值
            foreach (int i in meterPositions)
            {
                if (results[i - 1])
                    this.MeterPositions[i - 1].IsVerify = false;
            }

        }
        /// 表位手工短接
        /// <summary>
        /// 表位手工短接
        /// </summary>
        /// <param name="meterPositions">表位号</param>
        /// <param name="results">短接返回结果</param>
        /// <returns></returns>
        public void CurrentManualShout(int[] meterPositions, out bool[] results)
        {
            wcfk.CurrentShout(meterPositions, out results);

            // 临时处理，设置MeterPositions中的值
            foreach (int i in meterPositions)
            {
                if (results[i - 1])
                    this.MeterPositions[i - 1].IsVerify = false;
            }

        }
        /// 接线检查
        /// <summary>
        /// 接线检查设备供应商根据试验参数，返回各个表位电能表是否压接到位。
        /// </summary>
        /// <param name="U">预热电压，单位V</param>
        /// <param name="I">预热电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="checkResult">返回各个表位接线检查情况；”true”表示该表位接线检查正常，”false”表示该表位接线检查异常；</param>
        public override bool ExecuteConnectionCheck(float U, float I, float acFreq, out string[] checkResults)
        {
            bool readLineOver = false;      //采样结束
            Pulse pulse = Pulse.正向无功;   //脉冲方式
            int circle = 5;                //圈数 
            checkResults = new string[MeterPositions.Length];  //采样误差数据
            float timeOut;
            calcTime.CalcBasicErrorTime(U, I, 1.0f, true, acFreq, pulse.ToString(), circle, 1, out timeOut); //超时时间

            #region 验证基本误差（正向无功）
            string[] checkResult1s = new string[MeterPositionCount];

            #region 1.初始化系统通讯基本模块
            //1.1 设置接线方式  IPower
            this.OnInfo("正在设置接线方式...");
            power.SetWiringMode(meter.WiringMode, pulse);
            Thread.Sleep(500);

            //1.2 发送电压电流量程信息    IPower
            this.OnInfo("正在设置电压电流量程...");
            power.SetRange(U, I);
            Thread.Sleep(500);

            //1.3 设置相位信息 IPower
            this.OnInfo("正在设置相位信息...");
            power.SetLoadPhase(meter.WiringMode, 1.0f, true, pulse, LoadPhase.None);
            Thread.Sleep(500);
            #endregion

            #region 2.校验参数设置
            //2.1 初始化误差板状态 IWcfk
            this.OnInfo("正在初始化误差板...");
            wcfk.InitWcfk();

            //2.2 设置检定类型 IWcfk
            this.OnInfo("正在设置检定类型...");
            wcfk.SetVerificationType(VerificationElementType.基本误差试验);

            //2.3 设置检定脉冲方式 IPower
            this.OnInfo("正在设置检定脉冲方式...");
            power.SetVerificationPulseType(VerificationElementType.基本误差试验);
            Thread.Sleep(500);

            //2.4 设置脉冲方向 IWcfk
            this.OnInfo("正在设置设置脉冲方向...");
            wcfk.SetPulseType(pulse);

            //2.5 设置脉冲通道 IWcfk
            this.OnInfo("正在设置脉冲通道...");
            wcfk.SetPulseChannel(VerificationElementType.基本误差试验, pulse);
            Thread.Sleep(500);

            //2.6 设置检定圈数 IWcfk
            this.OnInfo("正在设置检定圈数...");
            wcfk.SetCircle(circle);

            //2.7 设置时钟通道
            this.OnInfo("正在设置时钟通道...");
            timeChannel.SetTimeChannel(1);

            //2.8 设置被检表常数 IWcfk
            this.OnInfo("正在设置被检表常数...");
            wcfk.SetMeterConst(pulse == Pulse.反向无功 || pulse == Pulse.正向无功 ? meter.Rp_Const : meter.Const,U,I);

            //2.9 启动误差版
            this.OnInfo("正在启动误差版...");
            wcfk.StartWcfk();

            //2.10 设置标准表接线方式
            this.OnInfo("正在设置标准表接线方式...");
            stdMeter.SetStdMeterWiringMode(VerificationElementType.基本误差试验, meter.WiringMode, pulse);

            //2.12 切换CT档位
            this.OnInfo("切换CT档位...");
            currentControl.SetCurrentControl(I);
            Thread.Sleep(500);

            //2.13 切换多功能控制板 
            this.OnInfo("切换多功能控制板...");
            powerSupply.SetPowerSupplyType(VerificationElementType.基本误差试验);
            Thread.Sleep(500);

            //2.14 发送标准表常数
            this.OnInfo("发送标准表常数...");
            stdMeter.SetStdMeterConst(U, I, meter.WiringMode);

            //2.15 升电压、电流
            this.OnInfo("正在升电压、电流信息...");
            power.RaiseVotageCurrent(U, I, LoadPhase.None, meter.WiringMode, 1.0f, true, pulse);

            //2.16 延时10秒
            this.OnInfo("延时10秒...");
            Thread.Sleep(SteadyTime * 1000);
            ////增加校验如果检定台电流没有升起，则重新开始设置
            ////增加校验如果检定台电流大于电能表额定最大电流，则进行降电流操作
            #endregion

            #region 3.开始检定并读取误差
            //3.1、 开始检定
            this.OnInfo("开始检定...");
            wcfk.StartVerification(VerificationElementType.基本误差试验);

            //3.2、 读取误差
            this.OnInfo("读取误差...");
            int sampleIndex = 1;
            int startTime = Environment.TickCount;
            while (true)
            {
                Thread.Sleep(1000);
                for (int i = 0; i < MeterPositions.Length; i++)
                {
                    if ((MeterPositions[i].IsVerify) && (!IsStop) && string.IsNullOrEmpty(checkResult1s[i]))
                    {
                        OnInfo("正在读取第" + (i + 1) + "表位误差……");
                        ErrorData errorData = wcfk.ReadErrorData(i + 1, sampleIndex);
                        if (errorData != null
                            && !string.IsNullOrEmpty(errorData.ErrorValue))
                        {
                            checkResult1s[i] = "true";
                        }
                    }
                }
                //判断是否所有表位都读取成功，必须要将所有选中的表位读取成功，才能回传。
                readLineOver = true;
                if (((Environment.TickCount - startTime) / 1000 + 1) < timeOut + (sampleIndex == 1 ? 30 : 0))
                {
                    for (int i = 0; i < MeterPositions.Length; i++)
                    {
                        if (string.IsNullOrEmpty(checkResult1s[i])
                            && MeterPositions[i].IsVerify)
                        {
                            readLineOver = false;
                            break;
                        }
                    }
                }
                //如果所有表位都读取到数据则进行返回
                if (readLineOver)
                {
                    break;
                }
            }
            #endregion
            #endregion

            #region 验证基本误差（正向有功）

            pulse = Pulse.正向有功;         //脉冲方式
            string[] checkResult2s = new string[MeterPositionCount];
            calcTime.CalcBasicErrorTime(U, I, 1.0f, true, acFreq, pulse.ToString(), circle, 1, out timeOut); //超时时间

            #region 1.初始化系统通讯基本模块
            //1.1 设置接线方式  IPower
            this.OnInfo("正在设置接线方式...");
            power.SetWiringMode(meter.WiringMode, pulse);
            Thread.Sleep(500);

            //1.2 设置相位信息 IPower
            this.OnInfo("正在设置相位信息...");
            power.SetLoadPhase(meter.WiringMode, 1.0f, true, pulse, LoadPhase.None);
            Thread.Sleep(500);
            #endregion

            #region 2.校验参数设置
            //2.1 初始化误差板状态 IWcfk
            this.OnInfo("正在初始化误差板...");
            wcfk.InitWcfk();

            //2.2 设置脉冲方向 IWcfk
            this.OnInfo("正在设置设置脉冲方向...");
            wcfk.SetPulseType(pulse);

            //2.3 设置脉冲通道 IWcfk
            this.OnInfo("正在设置脉冲通道...");
            wcfk.SetPulseChannel(VerificationElementType.基本误差试验, pulse);
            Thread.Sleep(500);

            //2.4 设置被检表常数 IWcfk
            this.OnInfo("正在设置被检表常数...");
            wcfk.SetMeterConst(pulse == Pulse.反向无功 || pulse == Pulse.正向无功 ? meter.Rp_Const : meter.Const,U,I);

            //2.5 启动误差版
            this.OnInfo("正在启动误差版...");
            wcfk.StartWcfk();

            //2.6 设置标准表接线方式
            this.OnInfo("正在设置标准表接线方式...");
            stdMeter.SetStdMeterWiringMode(VerificationElementType.基本误差试验, meter.WiringMode, pulse);

            //2.7 升电压、电流
            this.OnInfo("正在升电压、电流信息...");
            power.RaiseVotageCurrent(U, I, LoadPhase.None, meter.WiringMode, 1.0f, true, pulse);

            //2.8 延时10秒
            this.OnInfo("延时10秒...");
            Thread.Sleep(SteadyTime * 1000);
            #endregion

            #region 3.开始检定并读取误差
            //3.1、 开始检定
            this.OnInfo("开始检定...");
            wcfk.StartVerification(VerificationElementType.基本误差试验);

            //3.2、 读取误差
            this.OnInfo("读取误差...");
            startTime = Environment.TickCount;
            while (true)
            {
                Thread.Sleep(1000);
                for (int i = 0; i < MeterPositions.Length; i++)
                {
                    if ((MeterPositions[i].IsVerify) && (!IsStop) && string.IsNullOrEmpty(checkResult2s[i]))
                    {
                        OnInfo("正在读取第" + (i + 1) + "表位误差……");
                        ErrorData errorData = wcfk.ReadErrorData(i + 1, sampleIndex);
                        if (errorData != null
                            && !string.IsNullOrEmpty(errorData.ErrorValue))
                        {
                            checkResult2s[i] = "true";
                        }
                    }
                }
                //判断是否所有表位都读取成功，必须要将所有选中的表位读取成功，才能回传。
                readLineOver = true;
                if (((Environment.TickCount - startTime) / 1000 + 1) < timeOut + (sampleIndex == 1 ? 30 : 0))
                {
                    for (int i = 0; i < MeterPositions.Length; i++)
                    {
                        if (string.IsNullOrEmpty(checkResult2s[i])
                            && MeterPositions[i].IsVerify)
                        {
                            readLineOver = false;
                            break;
                        }
                    }
                }
                //如果所有表位都读取到数据则进行返回
                if (readLineOver)
                {
                    break;
                }
            }
            #endregion
            #endregion

            #region 验证日计时
            pulse = Pulse.正向有功;         //脉冲方式
            int second = 60;               //检定时间
            string[] checkResult3s = new string[MeterPositionCount];
            calcTime.CalcClockErrorTime(U, acFreq, 1, second, 1, out timeOut);//超时时间

            #region 1.校验参数设置
            //1.1 初始化误差板  
            this.OnInfo("初始化误差板......");
            wcfk.InitWcfk();

            //1.2 设置检定类型  
            this.OnInfo("设置检定类型......");
            wcfk.SetVerificationType(VerificationElementType.日计时误差试验);

            //1.3 设置检定脉冲方式  
            this.OnInfo("设置检定脉冲方式...");
            power.SetVerificationPulseType(VerificationElementType.日计时误差试验);
            Thread.Sleep(500);

            //1.4 设置脉冲方向  
            this.OnInfo("设置脉冲方向......");
            wcfk.SetPulseType(pulse);
            Thread.Sleep(500);

            //1.5 设置脉冲通道  
            this.OnInfo("设置脉冲通道......");
            wcfk.SetPulseChannel(VerificationElementType.日计时误差试验, pulse);

            //1.6 设置检定时间  
            this.OnInfo("设置检定时间......");
            wcfk.SetClockErrorTime(second, VerificationElementType.日计时误差试验);

            //1.7 启动误差版
            this.OnInfo("启动误差版......");
            wcfk.StartWcfk();

            //1.8 设置标准表接线方式、电能指示
            this.OnInfo("设置标准表接线方式、电能指示......");
            stdMeter.SetStdMeterWiringMode(VerificationElementType.日计时误差试验, meter.WiringMode, pulse);

            //1.9 切换多功能控制板 
            this.OnInfo("切换多功能控制板...");
            powerSupply.SetPowerSupplyType(VerificationElementType.日计时误差试验);
            Thread.Sleep(500);

            #endregion

            #region 2.设置多功能端子
            bool[] results;
            string[] resultDescription;
            // 2.1 设置多功能端子
            this.OnInfo("设置多功能端子...");
            dlt645.SetMulTerminalOut(lstMeterIndex.ToArray(), VerificationElementType.日计时误差试验, out results, out resultDescription);
            CheckError("设置多功能端子", results);
            #endregion

            #region 3.开始检定并读取误差
            // 3.1 开始检定
            this.OnInfo("开始检定...");
            wcfk.StartVerification(VerificationElementType.日计时误差试验);

            // 3.2 读取误差
            this.OnInfo("读取误差...");
            startTime = Environment.TickCount;
            while (true)
            {
                Thread.Sleep(1000);
                for (int i = 0; i < MeterPositions.Length; i++)
                {
                    if (!IsStop
                        && MeterPositions[i].IsVerify
                        && string.IsNullOrEmpty(checkResult3s[i]))
                    {
                        this.OnInfo(string.Format("正在读取第{0}表位误差……", i + 1));
                        ErrorData errorData = wcfk.ReadErrorData(i + 1, sampleIndex);
                        if (errorData != null
                            && !string.IsNullOrEmpty(errorData.ErrorValue))
                        {
                            checkResult3s[i] = "true";
                        }
                    }
                }
                //判断是否所有表位都读取成功，必须要将所有选中的表位读取成功，才能回传。
                readLineOver = true;
                if (((Environment.TickCount - startTime) / 1000 + 1) < timeOut + (sampleIndex == 1 ? 30 : 0))
                {
                    for (int i = 0; i < MeterPositions.Length; i++)
                    {
                        if (string.IsNullOrEmpty(checkResult3s[i])
                            && MeterPositions[i].IsVerify)
                        {
                            readLineOver = false;
                            break;
                        }
                    }
                }
                //如果所有表位都读取到数据则进行返回
                if (readLineOver)
                {
                    break;
                }
            }
            #endregion
            #endregion

            #region 返回结果整合
            for (int i = 0; i < MeterPositionCount; i++)
            {
                if (this.MeterPositions[i].IsVerify)
                {
                    if (checkResult1s[i].Equals("true")
                        && checkResult2s[i].Equals("true")
                        && checkResult3s[i].Equals("true"))
                    {
                        checkResults[i] = "true";
                    }
                }
                else
                {
                    checkResults[i] = "false";
                }
            }
            #endregion

            return !IsStop;
        }
        /// 交采集精度试验--电能表检定事业部用
        /// <summary>
        /// 交采集精度试验--电能表检定事业部用
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
        /// <returns></returns>
        public override void ExecutJcjjdItem(float UA, float IA, float Afactor, bool Acapacitive, float UB, float IB, float Bfactor, bool Bcapacitive,
            float UC, float IC, float Cfactor, bool Ccapacitive, VerificationEquipment.Commons.WiringMode wiringMode, Pulse pulse)
        {
            //分别加A、B、C三相电压、电流、功率因数。
            power.RaiseVoltageCurrentForJcjd(UA, IA, Afactor, Acapacitive, UB, IB, Bfactor, Bcapacitive, UC, IC, Cfactor, Ccapacitive, wiringMode, pulse);
        }
        
        ///分相供电试验 -- 电能表检定事业部人用
        /// <summary>
        ///  
        /// 分相供电试验 -- 电能表检定事业部人用
        /// </summary>
        /// <param name="UA"></param>
        /// <param name="UB"></param>
        /// <param name="UC"></param>
        /// <param name="wiringMode"></param>
        /// <param name="factor"></param>
        /// <param name="capacitive"></param>
        /// <param name="pulse"></param>
        public override void ExecutFenXiangPower(float UA, float UB, float UC, WiringMode wiringMode, float factor, bool capacitive, Pulse pulse)
        {

            //LoadPhase loadPhase, WiringMode wiringMode, float factor, bool capacitive, Pulse pulse)
            LoadPhase loadPhase = LoadPhase.None;
            
            power.RaiseVoltage(UA, UB, UC,loadPhase,wiringMode,factor,capacitive,pulse);
        }
        ///降源-电能表事业部人员用
        /// <summary>
        /// 降源-电能表事业部人员用
        /// </summary>
        public void ExecutDownPower()
        {
            try
            {
                power.StopVerification();
            }
            catch(Exception ex)
            { }
        }
        ///读取标准信息。
        /// <summary>
        /// 读取标准信息。
        /// </summary>
        /// <returns></returns>
        public stStdInfo ReadStdParam()
        {
            stStdInfo std = new stStdInfo();
            std = (stStdInfo)stdMeter.ReadStdMeterParam();

            return std;
        }
        ///分相供电测试试验
        /// <summary>
        /// 分相供电测试试验
        /// </summary>
        /// <param name="UA">A相电压</param>
        /// <param name="UB">B相电压</param>
        /// <param name="UC">C相电压</param>
        /// <param name="wiringMode">接线方式</param>
        /// <param name="factor">功率因数</param>
        /// <param name="capacitive">容性还是感性</param>
        /// <param name="pulse">脉冲类型</param>
        /// <param name="fOneVoltage">第一相返回值</param>
        /// <param name="fTwoVoltage">第二相返回值</param>
        public bool ExecuteFxgd(float UA, float UB, float UC, float factor, bool capacitive, Pulse pulse, out float[] fOneVoltage, out float[] fTwoVoltage)
        {
            this.IsStop = false;
            LoadPhase loadPhase = LoadPhase.None;

            power.RaiseVoltage(UA, UB, UC, loadPhase, meter.WiringMode, factor, capacitive, pulse);

            Thread.Sleep(10000);


            // 2.1 获取电能表地址
            OnInfo("读取其他相的电压值......");
            int voltIndex = 0;
            if(UA >0)
            {
                voltIndex = 1;
            }
            else if(UB > 0)
            {
                voltIndex = 2;
            }
            else if(UC > 0)
            {
                voltIndex = 3;
            }
            //读取其它两项的电压值
            dlt645.GetOtherVoltage(lstMeterIndex.ToArray(), voltIndex, out fOneVoltage, out fTwoVoltage);

            return !this.IsStop;
            //dlt645.GetAddress(lstMeterIndex.ToArray(), out fCurrentVoltage);
        }
        /// 交采集精度试验项目
        /// <summary>
        /// 交采集精度试验
        /// </summary>
        /// <param name="jc">试验参数类型</param>
        /// <param name="wiringMode">界线方式</param>
        /// <param name="pulse">脉冲类型</param>
        /// <param name="strRsInfo">返回不合格信息</param>
        /// <param name="reSult">返回该点所有表结论</param>
        /// <returns></returns>
        public bool ExecuteJcjjTest(JcjdItemPoint jc, Pulse pulse, out JcjdoPoint[] jcjdData,  out string[] strRsInfo, out int[] reSult)
        {
            this.IsStop = false;
            //1.加相应的三相电压、电流、功率引述
            float[][] MeterOurParams;

            //JcjdoPoint [] jcjdData;

            float fUA = jc.xbA.fU;//A相电压
            float fIA = jc.xbA.fI; //A相电流
            float fCaA = jc.xbA.fCa;//A相功率因数
            bool bCaA = jc.xbA.bC;//C感性OR容性、

            float fUB = jc.xbB.fU;//B相电压
            float fIB = jc.xbB.fI; //B相电流
            float fCaB = jc.xbB.fCa;//B相功率因数
            bool bCaB = jc.xbB.bC;//C感性OR容性、

            float fUC = jc.xbC.fU;//C相电压
            float fIC = jc.xbC.fI; //C相电流
            float fCaC = jc.xbC.fCa;//C相功率因数
            bool bCaC = jc.xbC.bC;//C感性OR容性、

            power.RaiseVoltageCurrentForJcjd(fUA, fIA, fCaA, bCaA, fUB, fIB, fCaB, bCaB, fUC, fIC, fCaC, bCaC, meter.WiringMode, pulse);
            //2.延时十秒后开始读数据
            Thread.Sleep(15000);
            //3.先读标准表的参数
            stStdInfo std = new stStdInfo();
            std = ReadStdParam();

            //4.再读电能表的电压参数、比对算误差判定结论
            #region 读取电压并判定结论
            dlt645.ReadMeterParam(lstMeterIndex.ToArray(), 0x0201ff00, out MeterOurParams);
            string[] strOutTestInfo = new string[MeterOurParams.Length];
            int[] bMeterResult = new int[MeterOurParams.Length];//结论0、未知|1、合格|2、不合格
            jcjdData = new JcjdoPoint[MeterOurParams.Length];
            for (int j = 0; j < MeterOurParams.Length; j++)
            {
                strOutTestInfo[j] = string.Empty;
                bMeterResult[j] = 0;
                jcjdData[j] = new JcjdoPoint();
            }

            float fError = 0f;
            //判定结论
            for (int i = 0; i < MeterOurParams.Length; i++)
            {
                //评定A相电压结论
                fError = (MeterOurParams[i][0] - std.Ua) / std.Ua* 100;
                jcjdData[i].ItemDataA.fUStd = std.Ua;
                jcjdData[i].ItemDataA.fUMeter = MeterOurParams[i][0];
                jcjdData[i].ItemDataA.fUErr = fError;
                if (fError <= jc.xbA.fUuplimit && fError >= jc.xbA.fUdownlimit)
                {
                    //合格
                    bMeterResult[i] = 1;
                }
                else
                {
                    //不合格
                    bMeterResult[i] = 2;
                    strOutTestInfo[i] += "UA:" + MeterOurParams[i][0].ToString("f2") + "|StdUA:" + std.Ua.ToString("f2") + "|UA Error:" + fError.ToString("f2");
                }

                //判定B相电压结论
                fError = (MeterOurParams[i][1] - std.Ub) / std.Ub * 100;
                jcjdData[i].ItemDataB.fUStd = std.Ub;
                jcjdData[i].ItemDataB.fUMeter = MeterOurParams[i][1];
                jcjdData[i].ItemDataB.fUErr = fError;
                if (fError <= jc.xbB.fUuplimit && fError >= jc.xbB.fUdownlimit && bMeterResult[i] == 1)
                {
                    //合格
                    bMeterResult[i] = 1;
                }
                else
                {
                    //不合格
                    bMeterResult[i] = 2;
                    strOutTestInfo[i] += "UB:" + MeterOurParams[i][1].ToString("f2") + "|StdUB:" + std.Ub.ToString("f2") + "|UB Error:" + fError.ToString("f2");
                }


                //判定C相电压结论
                fError = (MeterOurParams[i][2] - std.Uc) / std.Uc * 100;
                jcjdData[i].ItemDataC.fUStd = std.Uc;
                jcjdData[i].ItemDataC.fUMeter = MeterOurParams[i][2];
                jcjdData[i].ItemDataC.fUErr = fError;
                if (fError <= jc.xbC.fUuplimit && fError >= jc.xbC.fUdownlimit && bMeterResult[i] == 1)
                {
                    //合格
                    bMeterResult[i] = 1;
                }
                else
                {
                    //不合格
                    bMeterResult[i] = 2;
                    strOutTestInfo[i] += "UC:" + MeterOurParams[i][2].ToString("f2") + "|StdUC:" + std.Uc.ToString("f2") + "|UC Error:" + fError.ToString("f2");
                }

            }
            #endregion
            //5.再读电能表的电流参数、比对算误差判定结论

            #region 读取当前电流值并判定结论
            dlt645.ReadMeterParam(lstMeterIndex.ToArray(), 0x0202ff00, out MeterOurParams);
            //判定结论
            for (int i = 0; i < MeterOurParams.Length; i++)
            {
                //评定A相电流结论
                fError = (MeterOurParams[i][0] - std.Ia) / std.Ia * 100;
                jcjdData[i].ItemDataA.fIStd = std.Ia;
                jcjdData[i].ItemDataA.fIMeter = MeterOurParams[i][0];
                jcjdData[i].ItemDataA.fIErr = fError;
                if (fError <= jc.xbA.fIuplimit && fError >= jc.xbA.fIdownlimit && bMeterResult[i] == 1)
                {
                    //合格
                    bMeterResult[i] = 1;
                }
                else
                {
                    //不合格
                    bMeterResult[i] = 2;
                    strOutTestInfo[i] += "IA:" + MeterOurParams[i][0].ToString("f3") + "|StdIA:" + std.Ia.ToString("f3") + "|IA Error:" + fError.ToString("f2");
                }

                //判定B相电流结论
                fError = (MeterOurParams[i][1] - std.Ib) / std.Ib * 100;
                jcjdData[i].ItemDataB.fIStd = std.Ib;
                jcjdData[i].ItemDataB.fIMeter = MeterOurParams[i][1];
                jcjdData[i].ItemDataB.fIErr = fError;
                if (fError <= jc.xbB.fUuplimit && fError >= jc.xbB.fIdownlimit && bMeterResult[i] == 1)
                {
                    //合格
                    bMeterResult[i] = 1;
                }
                else
                {
                    //不合格
                    bMeterResult[i] = 2;
                    strOutTestInfo[i] += "IB:" + MeterOurParams[i][1].ToString("f3") + "|StdIB:" + std.Ib.ToString("f3") + "|IB Error:" + fError.ToString("f2");
                }


                //判定C相电流结论
                fError = (MeterOurParams[i][2] - std.Ic) / std.Ic * 100;
                jcjdData[i].ItemDataC.fIStd = std.Ic;
                jcjdData[i].ItemDataC.fIMeter = MeterOurParams[i][2];
                jcjdData[i].ItemDataC.fIErr = fError;
                if (fError <= jc.xbC.fIuplimit && fError >= jc.xbC.fIdownlimit && bMeterResult[i] == 1)
                {
                    //合格
                    bMeterResult[i] = 1;
                }
                else
                {
                    //不合格
                    bMeterResult[i] = 2;
                    strOutTestInfo[i] += "IC:" + MeterOurParams[i][2].ToString("f3") + "|StdIC:" + std.Ic.ToString("f3") + "|IC Error:" + fError.ToString("f2");
                }

            }

            #endregion

            //6.读取电能表功率参数，比对误差并判定结论

            #region 判定电能表功率并比对误差
            dlt645.ReadMeterParam(lstMeterIndex.ToArray(), 0x0203ff00, out MeterOurParams);

            //判定结论
            for (int i = 0; i < MeterOurParams.Length; i++)
            {
                //判定总有功功率
                //fError = (MeterOurParams[i][0]*1000 - std.P) / std.P * 100;
                //if (fError <= jc.xbA.fPuplimit && fError >= jc.xbA.fPdownlimit && bMeterResult[i] == 1)
                //{
                //    //合格
                //    bMeterResult[i] = 1;
                //}
                //else
                //{
                //    //不合格
                //    bMeterResult[i] = 2;
                //    strOutTestInfo[i] += "Pz:" + MeterOurParams[i][0].ToString("f4") + "|StdPz:" + std.P.ToString("f4") + "|Pz Error:" + fError.ToString("f2");
                //}

                //A相功率
                fError = (MeterOurParams[i][1]*1000 - std.Pa) / std.Pa * 100;
                jcjdData[i].ItemDataA.fPStd =std.Pa;
                jcjdData[i].ItemDataA.fPMeter = MeterOurParams[i][1];
                jcjdData[i].ItemDataA.fPErr = fError;

                if (fError <= jc.xbA.fPuplimit && fError >= jc.xbA.fPdownlimit && bMeterResult[i] == 1)
                {
                    //合格
                    bMeterResult[i] = 1;
                }
                else
                {
                    //不合格
                    bMeterResult[i] = 2;
                    strOutTestInfo[i] += "Pa:" + MeterOurParams[i][1].ToString("f4") + "|StdPa:" + std.Pa.ToString("f4") + "|Pa Error:" + fError.ToString("f2");
                }


                //判定Pb相结论
                fError = (MeterOurParams[i][2]*1000 - std.Pb) / std.Pb * 100;
                jcjdData[i].ItemDataB.fPStd = std.Pb;
                jcjdData[i].ItemDataB.fPMeter = MeterOurParams[i][2];
                jcjdData[i].ItemDataB.fPErr = fError;
                if (fError <= jc.xbB.fPuplimit && fError >= jc.xbB.fPdownlimit && bMeterResult[i] == 1)
                {
                    //合格
                    bMeterResult[i] = 1;
                }
                else
                {
                    //不合格
                    bMeterResult[i] = 2;
                    strOutTestInfo[i] += "Pb:" + MeterOurParams[i][2].ToString("f4") + "|StdPb:" + std.Pb.ToString("f4") + "|Pb Error:" + fError.ToString("f2");
                }


                //判定Pc结论
                fError = (MeterOurParams[i][3]*1000 - std.Pc) / std.Pc * 100;
                jcjdData[i].ItemDataC.fPStd = std.Pc;
                jcjdData[i].ItemDataC.fPMeter = MeterOurParams[i][3];
                jcjdData[i].ItemDataC.fPErr = fError;
                if (fError <= jc.xbC.fPuplimit && fError >= jc.xbC.fPdownlimit && bMeterResult[i] == 1)
                {
                    //合格
                    bMeterResult[i] = 1;
                }
                else
                {
                    //不合格
                    bMeterResult[i] = 2;
                    strOutTestInfo[i] += "Pc:" + MeterOurParams[i][3].ToString("f4") + "|StdPc:" + std.Pc.ToString("f4") + "|Pc Error:" + fError.ToString("f2");
                }

            }
            #endregion
            //表位检定信息
            strRsInfo = strOutTestInfo;
            //结论
            reSult = bMeterResult;

            return !this.IsStop;
        }

        /// <summary>
        /// 修改电能表倍频
        /// </summary>
        /// <param name="Beilv">倍频</param>
        /// <returns></returns>
        public override bool ExecuteMeterdoubling(byte Beilv)
        {

            //额定电压
            
            this.IsStop = false;
            float U = Convert.ToSingle(meter.Voltage);
            #region 1.初始化系统通讯基本模块
            //1.1 发送电压电流量程信息    IPower
            this.OnInfo("设置电压电流量程......");
            power.SetRange(U, 0);
            Thread.Sleep(100);

            //1.2 切换多功能控制板
            this.OnInfo("切换多功能控制板...");
            powerSupply.SetPowerSupplyType(VerificationElementType.读取表地址);
            Thread.Sleep(100);

            //1.3 设置电压
            OnInfo("正在升电压...");
            bool hasUp = power.RaiseVoltage(U, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);

            //1.4 延时10秒
            if (hasUp)
            {
                OnInfo("延时10秒...");
                Thread.Sleep(SteadyTime * 1000);
            }
            #endregion

            #region 2.设置电能表常数
            // 2.1 设置电能表常数
            OnInfo("设置电能表常数......");
            bool [] reSult;
            dlt645.SetMeterDoubling(lstMeterIndex.ToArray(), out reSult,Beilv);
            #endregion

            return !IsStop;
        }

        /// <summary>
        /// 单独升源指令
        /// </summary>
        public void ExecuteOnlyUpPower()
        {
            this.IsStop = false;
            float U = Convert.ToSingle(meter.Voltage);
            OnInfo("正在升电压...");
            bool hasUp = power.RaiseVoltage(U, LoadPhase.None, meter.WiringMode, (float)1.0, true, Pulse.正向有功);
        }


        /// <summary>
        /// 从上层传下来的要检定表
        /// </summary>
        /// <param name="meterSelect"></param>
        public void MeterSelcetForUp(bool[] meterSelect)
        {
            
            // 临时处理，设置MeterPositions中的值
            for (int i = 0; i < meterSelect.Length;i++ )
            {

                this.MeterPositions[i - 1].IsVerify = meterSelect[i];
            }

        }
        #endregion

        #region 日志输出
        /// 日志输出
        /// <summary>
        /// 日志输出
        /// </summary>
        /// <param name="message"></param>
        public override void OnInfo(string message)
        {
            base.OnInfo(message);

            //if (LOG != null)
            //{
            //    LOG.Info(message);
            //}
        }
        #endregion

        #region 表操作的错误验证
        /// 表操作的错误验证
        /// <summary>
        /// 表操作的错误验证
        /// </summary>
        /// <param name="textStr">报错类型</param>
        /// <param name="results">结果合集</param>
        private void CheckError(string textStr, bool[] results)
        {
            List<int> lstErr = new List<int>();
            
            foreach (var item in lstMeterIndex)
            {
                if (MeterPositions[item - 1].IsVerify
                    && !results[item - 1])
                {
                    lstErr.Add(item);
                }
            }
            if (lstErr.Count > 0)
            {
                int lstCount=lstErr.Count;
                string[] strErr = new string[lstCount];
                for (int i = 0; i < lstCount; i++)
                {
                    strErr[i] = lstErr[i].ToString();
                }
                OnInfo(string.Format("第{0}表位" + textStr + "不成功...", string.Join("、", strErr)));
                //throw new Exception(string.Format("第{0}表位" + textStr + "不成功...", string.Join("、", lstErr)));
            }
        }
        #endregion

        #region 未实现的校验项

        public override bool Alarm(float U, float acFreq, out bool[] result1, out bool[] result2, out string[] resultDescriptions, out string[] beforeAlarmStates, out string[] afterAlarmStates, out string[] releaseBeforeAlarmStates, out string[] releaseAfterAlarmStates)
        {
            throw new NotImplementedException();
        }

        public override bool CarrieWaveReliability(float U, float acFreq, string dataID, int times, out string[] rates)
        {
            throw new NotImplementedException();
        }

        public override bool CarrieWaveReturn(float U, float acFreq, string dataID, out bool[] results, out string[] resultDescriptions, out string[] datas)
        {
            throw new NotImplementedException();
        }

        public override bool CarrieWaveSuccessRate(float U, float acFreq, string dataID, int times, int interval, out string[] rates)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePassword02(float U, float acFreq, string oldPassword, string newPassword, out bool[] results, out string[] resultDescriptions)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePassword04(float U, float acFreq, string oldPassword, string newPassword, out bool[] results, out string[] resultDescriptions)
        {
            throw new NotImplementedException();
        }

        public override bool CommunicationTest(float U, float acFreq, out bool[] results, out string[] resultDescriptions)
        {
            throw new NotImplementedException();
        }

        public override bool EventRecord(float U, float acFreq, out bool[] results, out string[] resultDescriptions)
        {
            throw new NotImplementedException();
        }

        public override bool FreezeByDay(float U, float acFreq, out bool[] results, out string[] resultDescriptions, out string[] lastFreezenEnergys, out string[] currentEnergys, out string[] freezenEnergys)
        {
            throw new NotImplementedException();
        }

        public override bool FreezeByHour(float U, float acFreq, out bool[] results, out string[] resultDescriptions, out string[] lastFreezenEnergys, out string[] currentEnergys, out string[] freezenEnergys)
        {
            throw new NotImplementedException();
        }

        public override bool FreezeByInstantaneous(float U, float acFreq, out bool[] results, out string[] resultDescriptions, out string[] lastFreezenEnergys, out string[] currentEnergys, out string[] freezenEnergys1, out string[] freezenEnergys2, out string[] freezenEnergys3)
        {
            throw new NotImplementedException();
        }

        public override bool FreezeByTime(float U, float acFreq, out bool[] results, out string[] resultDescriptions, out string[] lastFreezenEnergys, out string[] currentEnergys, out string[] freezenEnergys)
        {
            throw new NotImplementedException();
        }

        public override bool HoldPower(float U, float acFreq, out string[] beforeStates, out string[] afterStates, out string[] releaseBeforeStates, out string[] releaseAfterStates, out bool[] results, out string[] resultDescriptions)
        {
            throw new NotImplementedException();
        }

        public override bool HoldPowerCommand(float U, float acFreq, out string[] beforeStates, out string[] afterStates, out bool[] results, out string[] resultDescriptions)
        {
            throw new NotImplementedException();
        }

        public override bool PrivateKeyAuthentication(float U, float acFreq, out bool[] results, out string[] resultDescriptions)
        {
            throw new NotImplementedException();
        }

        public override bool ProtocolConsistency(float U, float acFreq, string dataID, int length, int digital, bool readWrite, string content, out bool[] results, out string[] datas)
        {
            throw new NotImplementedException();
        }

        public override bool PublicKeyAuthentication(float U, float acFreq, out bool[] results, out string[] resultDescriptions)
        {
            throw new NotImplementedException();
        }

        public override bool ReleasePowerCommand(float U, float acFreq, out string[] releaseBeforeStates, out string[] releaseAfterStates, out bool[] results, out string[] resultDescriptions)
        {
            throw new NotImplementedException();
        }

        public override bool RemoteChangeAccount(float U, float acFreq, double money, int count, out bool[] results, out string[] resultDescriptions)
        {
            throw new NotImplementedException();
        }

        public override bool RemoteCloseSwitchCommand(float U, float acFreq, out bool[] results, out string[] resultDescriptions)
        {
            throw new NotImplementedException();
        }

        public override bool RemoteDataReturn(float U, float acFreq, out bool[] result1, out bool[] result2, out string[] resultDescriptions, out string[] returnInfo1, out string[] returnInfo2)
        {
            throw new NotImplementedException();
        }

        public override bool RemoteOpenAccount(float U, float acFreq, double money, int count, out bool[] results, out string[] resultDescriptions)
        {
            throw new NotImplementedException();
        }

        public override bool RemoteOpenSwitchCommand(float U, float acFreq, out bool[] results, out string[] resultDescriptions)
        {
            throw new NotImplementedException();
        }

        public override bool RemoteParameterUpdate(float U, float acFreq, out bool[] results, out string[] resultDescriptions)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 计算校验点执行时间
        /// 预热时间
        /// <summary>
        /// 预热时间
        /// </summary>
        /// <param name="U">预热电压，单位V</param>
        /// <param name="I">预热电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="second">预热时间，单位秒</param>
        /// <returns>预热预计时间,单位秒</returns>
        public override float CalcWarmUpTime(float U, float I, float acFreq, int second)
        {
            return calcTime.CalcWarmUpTime(U, I, acFreq, second);
        }
        /// 基本误差试验（合元）时间
        /// <summary>
        /// 基本误差试验（合元）时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="I">电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="factor">功率因数</param>
        /// <param name="capacitive">容性标志，true表示容性，false表示感性</param>
        /// <param name="pulse">脉冲类型，正向有功、反向有功、正向无功、反向无功</param>
        /// <param name="circle">检验圈数</param>
        /// <param name="count">检验次数</param>
        /// <returns>基本误差试验预计时间,单位秒</returns>
        public override float CalcBasicErrorTime(float U, float I, float factor, bool capacitive, float acFreq, string pulse, int circle, int count)
        {
            float verifyTime;
            return calcTime.CalcBasicErrorTime(U, I, factor, capacitive, acFreq, pulse, circle, count, out verifyTime);
        }
        /// 基本误差试验（分元）时间
        /// <summary>
        /// 基本误差试验（分元）时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="I">电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="phase">相别，"A"：A(B)相；"B"：B相；"C"：C(B)相。注意三相三线表不作B相试验</param>
        /// <param name="factor">功率因数</param>
        /// <param name="capacitive">容性标志，true表示容性，false表示感性</param>
        /// <param name="pulse">脉冲类型，正向有功、反向有功、正向无功、反向无功</param>
        /// <param name="circle">检验圈数</param>
        /// <param name="count">检验次数</param>
        /// <returns>基本误差试验预计时间,单位秒</returns>
        public override float CalcBasicErrorTime(float U, float I, float acFreq, string phase, float factor, bool capacitive, Pulse pulse, int circle, int count)
        {
            float verifyTime;
            return calcTime.CalcBasicErrorTime(U, I, acFreq, phase, factor, capacitive, pulse, circle, count, out verifyTime);
        }
        /// 启动试验时间
        /// <summary>
        /// 启动试验时间
        /// </summary>
        /// <param name="U">启动电压，单位V</param>
        /// <param name="I">启动电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="second">启动试验时间，单位秒</param>
        /// <returns>启动试验预计时间,单位秒</returns>
        public override float CalcStartupTime(float U, float I, float acFreq, int second)
        {
            return calcTime.CalcStartupTime(U, I, acFreq, second);
        }
        /// 潜动试验时间
        /// <summary>
        /// 潜动试验时间
        /// </summary>
        /// <param name="U">潜动电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="second">潜动试验时间，单位：秒</param>
        /// <returns>潜动试验预计时间,单位秒</returns>
        public override float CalcLatentTime(float U, float acFreq, int second)
        {
            return calcTime.CalcLatentTime(U, acFreq, second);
        }
        /// 日计时试验时间
        /// <summary>
        /// 日计时试验时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="clockFreq">被检表时钟脉冲频率，默认1Hz</param>
        /// <param name="second">检验时间，单位：秒</param>
        /// <param name="count">检验次数</param>
        /// <returns>日计时试验预计时间,单位秒</returns>
        public override float CalcClockErrorTime(float U, float acFreq, float clockFreq, int second, int count)
        {
            float verifyTime;
            return calcTime.CalcClockErrorTime(U, acFreq, clockFreq, second, count, out verifyTime);
        }
        /// 走字和校核计度器试验时间
        /// <summary>
        /// 走字和校核计度器试验时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="I">试验电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="factor">功率因数</param>
        /// <param name="capacitive">容性标志，true表示容性，false表示感性</param>
        /// <param name="pulse">脉冲类型，正向有功、反向有功、正向无功、反向无功</param>
        /// <param name="phase">费率</param>
        /// <param name="energy">走字电能(kWh)</param>
        /// <returns>走字和校核计度器试验预计时间,单位秒</returns>
        public override float CalcEnergyReadingTime(float U, float I, float acFreq, float factor, bool capacitive, Pulse pulse, Phase phase, float energy)
        {
            float verifyTime;
            return calcTime.CalcEnergyReadingTime(U, I, acFreq, factor, capacitive, pulse, phase, energy, out verifyTime);
        }
        /// 需量周期、需量示值时间
        /// <summary>
        /// 需量周期、需量示值时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="I">试验电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>需量周期、需量示值预计时间,单位秒</returns>
        public override float CalcDemandTime(float U, float I, float acFreq)
        {
            float verifyTime;
            return calcTime.CalcDemandTime(U, I, acFreq, out verifyTime);
        }
        /// 时段投切试验时间
        /// <summary>
        /// 时段投切试验时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="I">试验电流，单位A，默认300%Ib</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>时段投切试验预计时间,单位秒</returns>
        public override float CalcSwitchChangeTime(float U, float I, float acFreq)
        {
            float verifyTime;
            return calcTime.CalcSwitchChangeTime(U, I, acFreq, out verifyTime);
        }
        /// 读电能底数时间
        /// <summary>
        /// 读电能底数时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="pulse">脉冲类型，正向有功、反向有功、正向无功、反向无功</param>
        /// <returns>读电能底数预计时间,单位秒</returns>
        public override float CalcGetEnergyReadingTime(float U, float acFreq, Pulse pulse)
        {
            return calcTime.CalcGetEnergyReadingTime(U, acFreq, pulse);
        }
        /// 读需量底数时间
        /// <summary>
        /// 读需量底数时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="pulse">脉冲类型，正向有功、反向有功、正向无功、反向无功</param>
        /// <returns>读需量底数预计时间,单位秒</returns>
        public override float CalcGetDemandReadingTime(float U, float acFreq, Pulse pulse)
        {
            return calcTime.CalcGetDemandReadingTime(U, acFreq, pulse);
        }
        /// 读取表地址时间
        /// <summary>
        /// 读取表地址时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>读取表地址预计时间,单位秒</returns>
        public override float CalcReadMeterAddressTime(float U, float acFreq)
        {
            return calcTime.CalcReadMeterAddressTime(U, acFreq);
        }
        /// 保电试验时间
        /// <summary>
        /// 保电试验时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>保电试验预计时间,单位秒</returns>
        public override float CalcHoldPowerTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }
        /// 保电命令时间
        /// <summary>
        /// 保电命令时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>保电命令预计时间,单位秒</returns>
        public override float CalcHoldPowerCommandTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }
        /// 解除保电命令时间
        /// <summary>
        /// 解除保电命令时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>解除保电命令预计时间,单位秒</returns>
        public override float CalcReleasePowerCommandTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }
        /// 远程开户充值时间
        /// <summary>
        /// 远程开户充值时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="money">充值金额</param>
        /// <param name="count">充值次数</param>
        /// <returns>远程开户充值预计时间,单位秒</returns>
        public override float CalcRemoteOpenAccountTime(float U, float acFreq, double money, int count)
        {
            throw new NotImplementedException();
        }
        /// 远程充值时间
        /// <summary>
        /// 远程充值时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="money">充值金额</param>
        /// <param name="count">充值次数</param>
        /// <returns>远程充值预计时间,单位秒</returns>
        public override float CalcRemoteChangeAccountTime(float U, float acFreq, double money, int count)
        {
            throw new NotImplementedException();
        }
        /// 远程密钥更新时间
        /// <summary>
        /// 远程密钥更新时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="isRemote">远程表或本地表，true:远程表，false:本地表</param>
        /// <returns>远程密钥更新预计时间,单位秒</returns>
        public override float CalcRemoteSecretKeyUpdateTime(float U, float acFreq, bool isRemote)
        {
            return calcTime.CalcRemoteSecretKeyUpdateTime(U, acFreq, isRemote);
        }
        /// 远程参数修改时间
        /// <summary>
        /// 远程参数修改时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns> 远程参数修改预计时间,单位秒</returns>
        public override float CalcRemoteParameterUpdateTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }
        /// 远程数据回抄时间
        /// <summary>
        /// 远程数据回抄时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>远程数据回抄预计时间,单位秒</returns>
        public override float CalcRemoteDataReturnTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }
        /// 远程拉合闸时间
        /// <summary>
        /// 远程拉合闸时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>远程拉合闸预计时间,单位秒</returns>
        public override float CalcRemoteKnifeSwitchTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }
        /// 远程拉闸命令时间
        /// <summary>
        /// 远程拉闸命令时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>远程拉闸命令预计时间,单位秒</returns>
        public override float CalcRemoteOpenSwitchCommandTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }
        /// 远程合闸命令时间
        /// <summary>
        /// 远程合闸命令时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>远程合闸命令预计时间,单位秒</returns>
        public override float CalcRemoteCloseSwitchCommandTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }
        /// 报警试验时间
        /// <summary>
        /// 报警试验时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>报警试验预计时间,单位秒</returns>
        public override float CalcAlarmTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }
        /// 清电量时间
        /// <summary>
        /// 清电量时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>清电量预计时间,单位秒</returns>
        public override float CalcClearEnergyTime(float U, float acFreq)
        {
            return calcTime.CalcClearEnergyTime(U, acFreq);
        }
        /// 清需量时间
        /// <summary>
        /// 清需量时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>清需量预计时间,单位秒</returns>
        public override float CalcClearDemandTime(float U, float acFreq)
        {
            return calcTime.CalcClearDemandTime(U, acFreq);
        }
        /// 表计对时时间
        /// <summary>
        /// 表计对时时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>表计对时预计时间,单位秒</returns>
        public override float CalcTimeSyncTime(float U, float acFreq)
        {
            return calcTime.CalcTimeSyncTime(U, acFreq);
        }
        /// 修改02、04级密码时间
        /// <summary>
        /// 修改02、04级密码时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>修改密码预计时间,单位秒</returns>
        public override float CalcChangePasswordTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }
        /// 通信测试时间
        /// <summary>
        /// 通信测试时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>通信测试预计时间,单位秒</returns>
        public override float CalcCommunicationTestTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }
        /// 事件记录时间
        /// <summary>
        /// 事件记录时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>事件记录预计时间,单位秒</returns>
        public override float CalcEventRecordTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }
        /// 公钥验证时间
        /// <summary>
        /// 公钥验证时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>公钥验证预计时间,单位秒</returns>
        public override float CalcPublicKeyAuthenticationTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }
        /// 私钥验证时间
        /// <summary>
        /// 私钥验证时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>私钥验证预计时间,单位秒</returns>
        public override float CalcPrivateKeyAuthenticationTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }
        /// 定时冻结时间
        /// <summary>
        /// 定时冻结时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>定时冻结预计时间,单位秒</returns>
        public override float CalcFreezeByTimeTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }
        /// 瞬时冻结时间
        /// <summary>
        /// 瞬时冻结时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>瞬时冻结预计时间,单位秒</returns>
        public override float CalcFreezeByInstantaneousTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }
        /// 日冻结时间
        /// <summary>
        /// 日冻结时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>日冻结预计时间,单位秒</returns>
        public override float CalcFreezeByDayTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }
        /// 整点冻结时间
        /// <summary>
        /// 整点冻结时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>整点冻结预计时间,单位秒</returns>
        public override float CalcFreezeByHourTime(float U, float acFreq)
        {
            throw new NotImplementedException();
        }
        /// 载波召测时间
        /// <summary>
        /// 载波召测时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="dataID">数据项标示</param>
        /// <returns>载波召测预计时间,单位秒</returns>
        public override float CalcCarrieWaveReturnTime(float U, float acFreq, string dataID)
        {
            throw new NotImplementedException();
        }
        /// 载波可靠性时间
        /// <summary>
        /// 载波可靠性时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="dataID">数据项标示</param>
        /// <param name="times">重发次数</param>
        /// <returns>载波可靠性预计时间,单位秒</returns>
        public override float CalcCarrieWaveReliabilityTime(float U, float acFreq, string dataID, int times)
        {
            throw new NotImplementedException();
        }
        /// 载波成功率时间
        /// <summary>
        /// 载波成功率时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="dataID">数据项标示</param>
        /// <param name="times">重发次数</param>
        /// <param name="interval">抄读间隔时间</param>
        /// <returns>载波成功率预计时间,单位秒</returns>
        public override float CalcCarrieWaveSuccessRateTime(float U, float acFreq, string dataID, int times, int interval)
        {
            throw new NotImplementedException();
        }
        /// 通信规约一致性检查时间
        /// <summary>
        /// 通信规约一致性检查时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="dataID">数据项标示</param>
        /// <param name="length">长度</param>
        /// <param name="digital">小数位</param>
        /// <param name="readWrite">操作，true:写，false:读</param>
        /// <param name="content">写入数据内容</param>
        /// <returns>通信规约一致性检查预计时间,单位秒</returns>
        public override float CalcProtocolConsistencyTime(float U, float acFreq, string dataID, int length, int digital, bool readWrite, string content)
        {
            throw new NotImplementedException();
        }
        /// 接线检查时间
        /// <summary>
        /// 接线检查时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="I">电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>接线检查预计时间,单位秒</returns>
        public override float CalcConnectionCheckTime(float U, float I, float acFreq)
        {
            throw new NotImplementedException();
        }
        /// 耐压试验时间
        /// <summary>
        /// 耐压试验时间
        /// </summary>
        /// <param name="resistanceU">耐压试验测试点电压，单位KV</param>
        /// <param name="resistanceI">漏电流档位，单位mA</param>
        /// <param name="resistanceTime">耐压试验时间，单位秒</param>
        /// <param name="resistanceType">耐压方式：”00”对外壳打耐压；”01”对辅助端子打耐压；”02”对外壳和辅助端子打耐压</param>
        /// <returns>耐压试验时间,单位秒</returns>
        public override float CalcResistanceTime(float resistanceU, float resistanceI, int resistanceTime, string resistanceType)
        {
            return calcTime.CalcResistanceTime(resistanceU, resistanceI, resistanceTime, resistanceType);
        }
        #endregion

    }
}
