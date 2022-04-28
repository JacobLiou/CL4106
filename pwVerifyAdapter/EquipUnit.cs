// ***************************************************************
//  设备控制单元
//  刘伟2012-07:
// ***************************************************************
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Text;
using System.IO;
using pwInterface;
using pwFunction;
using pwFunction.pwConst;
using pwClassLibrary;
using VerificationEquipment.Commons;
namespace VerifyAdapter
{
    /// <summary>
    /// 设备控制单元
    /// </summary>
    public class EquipUnit
    {

        #region----------私有变量----------

        private int m_BwCount = 24;
        /// <summary>
        /// 操作失败重试次数
        /// </summary>
        public int m_ReTryTimes = 3;

        private bool m_Stop = false;                            //是否停止

        #endregion


        #region----------公有变量----------
        /// <summary>
        /// 台体控制器接口
        /// </summary>
        public Frontier.MeterVerification.Communication.MeterEquipment m_IComAdpater = null;

        /// <summary>
        /// 当前是否是有功
        /// </summary>
        public bool IsYouGong = true;

        /// <summary>
        /// 是否已经联机可读标准表更新UI
        /// </summary>
        public bool isConnected = false;


        /// <summary>
        /// 停止检定
        /// </summary>
        public bool IsStop
        {
            set
            {
                m_Stop = value;
            }
            get { return m_Stop; }
        }


        /// <summary>
        /// 当前功能是否已经启动
        /// </summary>
        private bool isControlTask = false;
        #endregion


        #region----------构造函数----------

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="BwCount">表位数量</param>
        /// <param name="ReTryTime">失败重试次数</param>
        public EquipUnit(int BwCount, int ReTryTime)
        {
            m_BwCount = BwCount;
            m_ReTryTimes = ReTryTime;
            try
            {
                pwClassLibrary.TopWaiting.ShowWaiting("开始创建检定器...");

                m_IComAdpater = new Frontier.MeterVerification.Communication.MeterEquipment();

                //m_IComAdpater.InitConnect(BwCount);

            }
            catch
            {
                pwFunction.pwConst.GlobalUnit.g_MsgControl.OutMessage("创建台体控制器失败！请确认当前系统控制器类型" +  "\r\n如果当前控制器类型不符合，请到系统配置中更改后重新启动系统！");
            }

            pwClassLibrary.TopWaiting.ShowWaiting("初始化多功能控制器...");

        }


        #endregion


        #region ----------公有方法----------

        #region-----------延时----------
        /// <summary>
        /// 延时多少毫秒钟
        /// </summary>
        /// <param name="iTime">毫秒</param>
        public void DelayTime(long iTime)
        {
            Stopwatch sth_SpaceTicker = new Stopwatch();                        //记时时钟
            sth_SpaceTicker.Reset();
            sth_SpaceTicker.Start();       //开始记时             
            long TimeE = 0;
            while (true)
            {
                if (GlobalUnit.ApplicationIsOver 
                    || GlobalUnit.ForceVerifyStop 
                    || GlobalUnit.g_Status==enmStatus.停止
                    ) break;
                TimeE = sth_SpaceTicker.ElapsedMilliseconds;
                Thread.Sleep(5);
                if (TimeE > iTime) break;
                Application.DoEvents();
            }

        }
        #endregion

        #region ----------联机:public bool Link(bool bLink) ----------


        /// <summary>
        /// 联机/脱机操作
        /// </summary>
        /// <param name="bLink">是否联机</param>
        /// <returns>操作结果</returns>
        public bool Link(bool bLink)
        {
            if (GlobalUnit.IsDemo) return true;
            string strAction = bLink ? "联机" : "脱机";
            bool[] Result = new bool[] { false, false };

            if (GlobalUnit.g_Status == enmStatus.进行中)
            {
                GlobalUnit.g_MsgControl.OutMessage("不能执行 [" + strAction.ToString() + "] 操作，系统正在进行作业项目...");
                return false;
            }
            pwClassLibrary.TopWaiting.ShowWaiting(string.Format("正在{0}......", strAction));
            for (int i = 0; i < m_ReTryTimes; i++)
            {
                if (bLink)
                {
                    m_IComAdpater.InitConnect(m_BwCount);
                    GlobalUnit.g_Status = enmStatus.联机;
                    GlobalUnit.g_PowerStatus = enmPowerStatus.联机;
                    isConnected = true;
                }
                else
                {
                    m_IComAdpater.ExecutDownPower();
                    GlobalUnit.g_Status = enmStatus.脱机;
                    GlobalUnit.g_PowerStatus = enmPowerStatus.脱机;
                    isConnected = true;
                }

            }
            string strDes = string.Format("{0}", strAction) + "操作" + (Result[0] ? "成功" : "失败" );
            pwClassLibrary.TopWaiting.ShowWaiting(strDes);
            GlobalUnit.g_MsgControl.OutMessage(strDes,false );
            this.DelayTime(50);
            pwClassLibrary.TopWaiting.HideWaiting();
            return true;
        }
        #endregion

        #region ----------合元升源SetTestPoint----------
        public bool SetTestPoint(enmClfs Clfs ,float U, float I, string str_Glys,enmElement emt_Element,float Hz)
        {
            if (GlobalUnit.IsDemo) return true;
            bool Result = false;
            int _RetryTimes = 0;
            while (true)
            {
                pwFunction.pwConst.GlobalUnit.g_MsgControl.OutMessage("功率源正在输出...", false);
                Result = false; // m_IComAdpater.SetTestPoint(Clfs, U, I, emt_Element, str_Glys, Hz);
                _RetryTimes++;
                if (Result)
                    break;
                if (_RetryTimes > m_ReTryTimes)
                    break;
                if (GlobalUnit.ForceVerifyStop)
                    break;
                this.DelayTime(GlobalUnit.g_ThreadWaitTime*1000);
                if (GlobalUnit.ForceVerifyStop || GlobalUnit.g_Status == enmStatus.停止)//外部中止＝＝退出
                {
                    GlobalUnit.g_MsgControl.OutMessage("源输出被停止，操作完毕", false);
                    break;
                }
            }
            if (Result)
            {
                GlobalUnit.g_PowerStatus = enmPowerStatus.输出电压电流;
                this.DelayTime(GlobalUnit.g_PowerWaitTime * 1000);
            }
            else
            {
                GlobalUnit.g_PowerStatus = enmPowerStatus.升源失败;
                pwClassLibrary.Check.Require(false, "控制源输出失败" );
            }
            //再读取一次标准数据。让UI层更新
            ReadStdInfo();
            return Result;
        }
        #endregion

        #region ----------三相用----------
        /// <summary>
        /// 控制源输出
        /// </summary>
        /// <param name="Clfs">测量方式</param>
        /// <param name="U">电压</param>
        /// <param name="I">电流</param>
        /// <param name="Glys">功率因素</param>
        /// <param name="YJ">元件</param>
        /// <param name="isYouGong">是否是有功</param>
        /// <returns>源输出是否成功</returns>
        public bool SetTestPoint(enmClfs Clfs, Single U, Single I, string Glys, enmElement YJ, bool isYouGong)
        {
            return false;
        }

        #region ----------自由升源SetTestPoint----------
        /// <summary>
        /// 设置检测点参数[带重载]
        /// </summary>
        /// <param name="int_Clfs">测量方式</param>
        /// <param name="sng_U">电压</param>
        /// <param name="sng_xUa">A相电压倍数</param>
        /// <param name="sng_xUb">B相电压倍数</param>
        /// <param name="sng_xUc">C相电压倍数</param>
        /// <param name="sng_I">IB电流</param>
        /// <param name="sng_Imax">Imax电流</param>
        /// <param name="int_IType">采用IB还是IMax作为电流倍数基数</param>
        /// <param name="sng_xIa"></param>
        /// <param name="sng_xIb"></param>
        /// <param name="sng_xIc"></param>
        /// <param name="int_Element"></param>
        /// <param name="str_Glys"></param>
        /// <param name="sng_Freq"></param>
        /// <param name="bln_NXX"></param>
        /// <returns></returns>
        public bool SetTestPoint(enmClfs int_Clfs, Single sng_U, Single sng_xUa, Single sng_xUb, Single sng_xUc,
                          Single sng_I, Single sng_Imax, enmIType int_IType, Single sng_xIa, Single sng_xIb,
                          Single sng_xIc, enmElement int_Element, string str_Glys, Single sng_Freq, bool bln_NXX)
        {
            if (GlobalUnit.IsDemo) return true;
            bool Result = false;
            int _RetryTimes = 0;
            while (true)
            {
                Result = false;// m_IComAdpater.SetTestPoint(int_Clfs, sng_U, sng_xUa, sng_xUb, sng_xUc,
                           //sng_I, sng_Imax, int_IType, sng_xIa, sng_xIb,
                           //sng_xIc, int_Element, str_Glys, sng_Freq, bln_NXX);
                _RetryTimes++;
                if (Result)
                    break;
                if (_RetryTimes > m_ReTryTimes)
                    break;
                if (GlobalUnit.ForceVerifyStop)
                    break;
                //ThreadManage.Sleep(GlobalUnit.g_ThreadWaitTime);
            }
            //this.DelayTime(100);
            //读取一次标准信息
            ReadStdInfo();
            // if (!Result)        //如果没有控制成功，则直接返回
            return Result;
            //等待源稳定
            DateTime dateStartTime = DateTime.Now;
            GlobalUnit.g_MsgControl.OutMessage("正在等待源稳定...", false);
            //显示源信息

            #region 验证升源是否成功
            string[] strStdInfo = new string[0];

            DateTime sTime = DateTime.Now;
            //从配置文件中读取等待源稳定的时间
            int intMaxWaitTim = 20;// GlobalUnit.GetConfig(pwFunction.pwConst.Variable.CTC_OTHER_WAITTIME_POWERSTATE, 20);
            bool bOutOfTime = false;
            bool bSuccess = false;      //成功标志
            while (true)
            {
                if (ReadStdInfo(ref strStdInfo))
                {
                    //判断A相电压或是视在功能是否大于0
                    if (float.Parse(strStdInfo[0]) > 0F ||
                        float.Parse(strStdInfo[1]) > 0F ||
                        float.Parse(strStdInfo[2]) > 0F)
                    {
                        bSuccess = true;
                    }

                    //增加电流检测
                    if (sng_xIa > 0 || sng_xIb > 0 || sng_xIc > 0)
                    {
                        if (float.Parse(strStdInfo[15]) > 0F)
                            bSuccess = true;
                        else
                            bSuccess = false;
                    }
                    if (bSuccess)
                        break;
                }
            }
            #endregion
            return Result;
        }
        #endregion

        #region ----------获取当前要升源的测量方式-getClfs--------
        /// <summary>
        /// 转换当前要升源的测量方式
        /// ClinterFace中的测试方式定义与检定器定义不一致。
        /// </summary>
        /// <param name="isYouGong">是否是有功</param>
        /// <returns>测量方式</returns>
        private enmClfs getClfs(enmClfs Clfs, bool isYouGong)
        {
            /*   三相四线有功 = 0,
         三相四线无功 = 1,
         三相三线有功 = 2,
         三相三线无功 = 3,
         二元件跨相90 = 4,
         二元件跨相60 = 5,
         三元件跨相90 = 6,
             
        三相四线=0,
        三相三线=1,
        二元件跨相90=2,
        二元件跨相60=3,
        三元件跨相90=4,
        单相=5
             
             */

            int clfs = (int)Clfs;
            if (clfs == 5)                            //单相台统一划分为三相四线
            {
                clfs = 0;
            }
            clfs += 2;                              //先保证后面对齐
            if (clfs < 4)                             //处理前面没有对齐部分
            {
                if (clfs == 3)
                {
                    if (IsYouGong)
                    {
                        clfs--;
                    }
                }
                else
                {
                    clfs--;
                    if (IsYouGong)
                    {
                        clfs--;
                    }
                }
            }
            return (enmClfs)clfs;
        }
        #endregion
        #endregion

        #region ----------只升电压PowerOnOnlyU----------
        /// <summary>
        /// 只输出电压[默认输出正向有功1.0合元]
        /// </summary>
        /// <returns></returns>
        public bool PowerOnOnlyU()
        {
            bool Result = false;
            pwFunction.pwConst.GlobalUnit.g_MsgControl.OutMessage("功率源正在输出电压...", false);
            WiringMode _clfs;
            Pulse _glfx;
            if (GlobalUnit.g_Meter.MInfo.Clfs == enmClfs.三相四线有功)
            {
                _clfs = WiringMode.三相四线;
                _glfx = Pulse.正向有功;
            }
            else
            {
                _clfs = WiringMode.三相三线;
                _glfx = Pulse.正向有功;
            }
            

            m_IComAdpater.ExecutFenXiangPower(GlobalUnit.g_Meter.MInfo.Ub, 
                GlobalUnit.g_Meter.MInfo.Ub, 
                GlobalUnit.g_Meter.MInfo.Ub,
                _clfs,1.0f, true, _glfx);

                GlobalUnit.g_PowerStatus = enmPowerStatus.只输出电压;
                this.DelayTime(GlobalUnit.g_PowerWaitTime * 1000);

            ////再读取一次标准数据。让UI层更新
            ReadStdInfo();
            return Result;
        }
        #endregion

        #region ----------关源:public bool PowerOff()----------
        /// <summary>
        /// 关源
        /// </summary>
        /// <returns></returns>
        public bool PowerOff()
        {
            bool Result = false;
            if (GlobalUnit.IsDemo) return true;
            if (GlobalUnit.g_PowerStatus == enmPowerStatus.功率源关闭) return true;//检测源有没有输出
            pwFunction.pwConst.GlobalUnit.g_MsgControl.OutMessage("功率源正在关闭", false);
            m_IComAdpater.ExecutDownPower();

            GlobalUnit.g_PowerStatus = enmPowerStatus.功率源关闭;
            pwFunction.pwConst.GlobalUnit.g_MsgControl.OutMessage("关闭功率源成功!", false);
            this.DelayTime(GlobalUnit.g_PowerWaitTime * 1000);
            //再读取一次标准数据。让UI层更新
            ReadStdInfo();
            return Result;
        }
        #endregion

        #region ----------设置误差参数、并启动计算:public bool SetTaskParameter
        /// <summary>
        /// 设置电能误差参数、并启动计算
        /// </summary>
        /// <param name="int_TaskType">测试类型</param>
        /// <param name="lng_ErrTimes">误差次数</param>
        /// <param name="int_StdPulseType">标准脉冲类型</param>
        /// <param name="int_DivideF">分频系数</param>
        /// <param name="int_PulseSpace">脉冲修正系数</param>
        /// <param name="int_CFilter">过滤系数</param>
        /// <param name="lng_StdTimeConst"></param>
        /// <returns></returns>
        public bool SetTaskParameter(enmPulseDzType epd_DzType,enmTaskType ett_TaskType, enmChannelType ect_ChannelNo, enmPulseComType ept_PulseType, enmGyGyType egt_GyG, long lng_AmConst, long lng_PulseTimes, byte iAmMeterPulseBS)
        {
            if (GlobalUnit.IsDemo) return true;
            bool Result = false;
            int _RetryTimes = 0;

            while (true)
            {
                Result = false; //
                //m_IComAdpater.SetTaskParameter(
                //    epd_DzType,
                //    ett_TaskType,
                //    ect_ChannelNo,
                //    ept_PulseType,
                //    egt_GyG,
                //    lng_AmConst,
                //    lng_PulseTimes,
                //    iAmMeterPulseBS);


                _RetryTimes++;
                if (Result)
                {
                    break;
                }
                if (_RetryTimes > m_ReTryTimes)
                {
                    break;
                } if (GlobalUnit.ForceVerifyStop )
                {
                    break;
                }
                //ThreadManage.Sleep(GlobalUnit.g_ThreadWaitTime);
            }

            return Result;
        }

        /// <summary>
        /// 设置日计时误差参数
        /// </summary>
        /// <param name="ett_TaskType">任务类型</param>
        /// <param name="ect_ChannelNo">通道</param>
        /// <param name="ept_PulseType">脉冲接口</param>
        /// <param name="egt_GyG">共阴共阳</param>
        /// <param name="lng_AmConst">时钟频率</param>
        /// <param name="lng_PulseTimes">圈数</param>
        /// <returns></returns>
        public bool SetTaskParameter(enmPulseDzType epd_DzType,enmTaskType ett_TaskType, enmChannelType ect_ChannelNo, enmPulseComType ept_PulseType, enmGyGyType egt_GyG, float sng_TimePL, int lng_PulseTimes)
        {
            if (GlobalUnit.IsDemo) return true;
            bool Result = false;
            int _RetryTimes = 0;

            while (true)
            {
                Result =false; //
                //m_IComAdpater.SetTaskParameter(
                //    epd_DzType,
                //    ett_TaskType,
                //    ect_ChannelNo,
                //    ept_PulseType,
                //    egt_GyG,
                //    sng_TimePL,
                //    lng_PulseTimes);


                _RetryTimes++;
                if (Result)
                {
                    break;
                }
                if (_RetryTimes > m_ReTryTimes)
                {
                    break;
                } if (GlobalUnit.ForceVerifyStop)
                {
                    break;
                }
                //ThreadManage.Sleep(GlobalUnit.g_ThreadWaitTime);
            }

            return Result;
        }
        #endregion

        #region ----------设置电流回路:public bool SetCurrentLoop
        /// <summary>
        /// 设置电流回路
        /// </summary>
        /// <param name="int_LoopType"></param>
        /// <returns></returns>
        public bool SetCurrentLoop(int int_LoopType)
        {
            if (GlobalUnit.IsDemo) return true;
            return false ;
        }
        #endregion

        #region----------设置表位开关SetBWSwitch--------
        public bool SetBWSwitch(bool bln_Open)
        {
            if (GlobalUnit.IsDemo) return true;
            return false; //m_IComAdpater.SetAmmeterCmmSwitch(0xff, bln_Open);
        }
        #endregion

        #region ----------读取标准数据信息:ReadStdInfo----------
        /// <summary>
        /// 读取标准数据事件.读取标准数据可以采用直接返回和事件返回二种
        /// 方式。
        /// </summary>
        public OnEventReadStdInfo OnReadStaInfo;

        /// <summary>
        /// 读取标准数据信息[直接返回]
        /// </summary>
        /// <param name="strData">读取到的数据</param>
        /// <returns>是否成功</returns>
        public bool ReadStdInfo(ref string[] strData)
        {
            bool Result = false;
            if (!isConnected) return true;
            for (int i = 0; i < m_ReTryTimes; i++)
            {
                Frontier.MeterVerification.KLDevice.stStdInfo std = new Frontier.MeterVerification.KLDevice.stStdInfo();
                std = m_IComAdpater.ReadStdParam();

                //strData = std;

                
                if (Result)
                    break;
            }
            return Result;
        }
        /// <summary>
        /// 读取标准表数据[事件通知]
        /// </summary>
        public void ReadStdInfo()
        {
            //return ;                //
            bool Result = false;
            string[] strData = new string[0];
            if (!isConnected) return;
            Result = ReadStdInfo(ref strData);
            if (Result)
            {
                if (OnReadStaInfo != null)
                {
                    OnReadStaInfo(strData);
                }
            }
        }
        #endregion


        #region-----------读取数据ReadTaskData----------
        /// <summary>
        /// 读取功能数据
        /// </summary>
        /// <param name="arrData">[数据数组]</param>
        /// <param name="intTimes">次数数组</param>
        /// <returns></returns>
        public bool ReadTaskData(int byt_TaskType, ref bool[] bln_Result, ref string[] arrData, ref int[] intTimes)
        {
            bool Result = false;
            string strReadType = "误差板";//预留，方便以后扩展

            for (int i = 0; i < m_ReTryTimes; i++)
            {
                if (strReadType == "误差板")
                {
                    Result = false;// m_IComAdpater.ReadTaskData(byt_TaskType, ref bln_Result, ref arrData, ref intTimes);
                }
                else if (strReadType == "标准表")
                {
                    //Result = m_IComAdpater.ReadStdMeterErrorAndPulse(ref arrData, ref (long)intTimes);
                }
                if (Result)
                    break;
            }
            return Result;
        }

        #endregion

        #region ----------停止当前任务:public bool ControlTask(bool bStart)----------
        /// <summary>
        /// 启动/停止当前任务[除走字试验以外，本操作必须放在升源命令以后，否则标准表常数会出错]
        /// </summary>
        /// <param name="bStart">启动(True)/停止(False)</param>
        /// <returns>是否操作成功</returns>
        public bool StopTask(enmTaskType ett_TaskType)
        {
            if (GlobalUnit.IsDemo) return true;
            GlobalUnit.g_MsgControl.OutMessage("正在停止当前误差板任务", false);
            bool Result = false;
            if (m_IComAdpater == null) return false;
            for (int i = 0; i < m_ReTryTimes; i++)
            {

                Result = false; //m_IComAdpater.StopCalculate(ett_TaskType);
                if (Result)
                {
                    break;
                }
                if (GlobalUnit.ForceVerifyStop)
                {
                    break;
                }
                this.DelayTime(50);
            }
            return Result;
        }
        #endregion




        #endregion


    }
}
