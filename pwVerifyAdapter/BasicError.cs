using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using pwFunction.pwConst;
using System.Diagnostics;
using pwInterface;
using pwFunction.pwMeter;
using System.Windows.Forms;
namespace VerifyAdapter
{

    /// <summary>
    /// 基本误差检定控制器
    /// </summary>
    public class BasicError
    {


        #region -----------变量----------
        private Stopwatch sth_SpaceTicker = new Stopwatch();        //记时时钟
        private VerifyAdapter.EquipUnit m_EquipUnit = null;         //装置
        private CheckPoint m_CheckPoint = new CheckPoint();         //检定点(检定设置用)
        private MeterErrorItem[] m_Error = new MeterErrorItem[24];  //误差数据(计算及存储用)
        //=============
        private int m_BwCount = 24;                                 //表位数
        private bool m_Stop = false;                                //是否停止
        protected bool m_CheckOver = false;                         //是否已经完成检定
        private bool[] m_IsCheck = new bool[24];                    //要检
        private stPrjParameter m_stPrjParameter;                    //项目参数结构
        //private int m_DLHL = 0;                                     //电流回路

        #region -------误差控制属性，从系统配置可读取----------
        /// <summary>
        /// 每一个误差点取几次误差参与计算(计算次数)
        /// </summary>
        private int m_WCTimesPerPoint = GlobalUnit.WCTimesPerPoint;

        /// <summary>
        /// 标准偏差点取几次误差参与计算
        /// </summary>
        private int m_WindageTimesPerPoint = GlobalUnit.WindageTimesPerPoint;

        /// <summary>
        /// 每一个误差点最多读取多少次误差(最大次数)
        /// </summary>
        private int m_WCMaxTimes = GlobalUnit.WCMaxTimes;

        /// <summary>
        /// 每一个误差点最多读多少秒(最长时间)
        /// </summary>
        private int m_WCMaxSeconds = GlobalUnit.WCMaxSeconds;

        /// <summary>
        /// 跳差判定标准
        /// </summary>
        private float m_WCJump = GlobalUnit.WCJump;

        /// <summary>
        /// 高频检定时常数倍数
        /// </summary>
        private int m_WcConstantBs = 8;//GlobalUnit.WcConstantBs;

        /// <summary>
        ///标准脉冲分频系数
        /// </summary>
        private float m_WcDriverF = GlobalUnit.WcDriverF;

        #endregion
        #endregion

        #region -----------属性----------

        public CheckPoint CheckPoint
        {
            set
            {
                m_CheckPoint = value;
            }
            get
            {
                return m_CheckPoint;
            }
        }

        /// <summary>
        /// 停止检定
        /// </summary>
        public bool IsStop
        {
            set
            {
                m_Stop = value;
                m_CheckOver = value;
                m_EquipUnit.StopTask(enmTaskType.电能误差);
            }
            get { return m_Stop; }
        }

        /// <summary>
        /// 是否要检
        /// </summary>
        public bool[] blnSelected
        {
            get { return m_IsCheck; }
            set { m_IsCheck = value; }
        }

        /// <summary>
        /// 项目参数结构
        /// </summary>
        public stPrjParameter stPrjParameter
        {
            set
            {
                m_stPrjParameter = value;
                m_WcConstantBs = m_stPrjParameter.int_HighFrequencyPulseBS;
            }
        }

        public event DelegateEventMultiController OnEventMultiControlle;

        #endregion


        #region -----------构造----------

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="BwCount"></param>
        /// <param name="ComAdpater"></param>
        public BasicError(int BwCount, VerifyAdapter.EquipUnit ComAdpater)
        {
            m_BwCount = BwCount;
            m_EquipUnit = ComAdpater;
            m_IsCheck = new bool[m_BwCount];

        }
        #endregion 

        #region -----------方法----------

        /// <summary>
        /// 检定
        /// </summary>
        public void Verify()
        {

            #region ----------检定准备----------
            GlobalUnit.g_MsgControl.OutMessage("开始初始化当前误差检定点参数...", false);
            int _TableHeader = m_WCTimesPerPoint;           //记录误差数据，推箱子用，用来计算的有效误差次数
            int _MaxWCnum = m_WCMaxTimes;                   //最大误差次数
            m_CheckOver = false;                            //还原检定标识，没有检定完成
            m_Stop = false;
            #endregion

            #region ----------初始化设备参数：[基本误差]:InitEquipment----------
            if (!InitEquipment())
            {
                #region 升源失败
                for (int i = 0; i < m_BwCount; i++)
                {
                    if (!m_IsCheck[i]) continue;
                    m_Error[i] = new MeterErrorItem();
                    m_Error[i].Me_Bw = i;
                    m_Error[i].Item_PrjID = m_CheckPoint.PrjID;
                    m_Error[i].Item_PrjName = m_CheckPoint.PrjName;
                    m_Error[i].Item_Result = Variable.CTG_BuHeGe;
                    if (!m_IsCheck[i]) continue;
                    if (GlobalUnit.g_Meter.MData[i].MeterErrors.ContainsKey(m_CheckPoint.PrjID))
                    {
                        GlobalUnit.g_Meter.MData[i].MeterErrors.Remove(m_CheckPoint.PrjID);
                    }
                    GlobalUnit.g_Meter.MData[i].MeterErrors.Add(m_CheckPoint.PrjID, m_Error[i]);

                    if (this.OnEventMultiControlle != null) this.OnEventMultiControlle(enmMeterPrjID.误差检定, i, false, "升源失败");

                }
                #endregion
                return;
            }
            #endregion

            #region ----------变量----------
            string[] _CurWC = new string[m_BwCount];          //当前误差
            int[] _CurrentWcNum = new int[m_BwCount];         //当前累计检定次数
            int[] _MeterWcNum = new int[m_BwCount];           //表位取误差次数，处进超过255次
            int[] _VerifyTimes = new int[m_BwCount];          //有效误差次数
            bool[] _CheckOver = new bool[m_BwCount];          //是否已经完成本次检定

            GlobalUnit.g_MsgControl.OutMessage(pwClassLibrary.enmMessageType.清空消息队列);
            int ReadWCNum = 0;
            sth_SpaceTicker.Reset();
            sth_SpaceTicker.Start();       //开始记时             
            #endregion

            #region ----------循环检定----------
            while (true)
            {
                #region 读误差
                GlobalUnit.g_MsgControl.OutMessage("正在检定当前误差点：" + m_CheckPoint.PrjName, false);
                Application.DoEvents();
                if (m_CheckOver || GlobalUnit.ForceVerifyStop || m_Stop)//全部表检定完成，外部中止＝＝退出
                {
                    GlobalUnit.g_MsgControl.OutMessage("当前点被检定停止...", false);
                    break;
                }
                if (Convert.ToInt32(sth_SpaceTicker.ElapsedMilliseconds / 1000) > m_WCMaxSeconds)
                {
                    GlobalUnit.g_MsgControl.OutMessage("当前点检定已经超过最大检定时间" + m_WCMaxSeconds + "秒！", false);
                    break;
                }

                _CurWC = new string[m_BwCount];               //重新初始化本次误差
                _CurrentWcNum = new int[m_BwCount];           //重新初始化本次误差次数

                Thread readstdinfo = new Thread(new ThreadStart(m_EquipUnit.ReadStdInfo));
                readstdinfo.Start();
                Application.DoEvents();

                if (m_CheckPoint.xIb <= 0.1f)
                {
                    m_EquipUnit.DelayTime(m_CheckPoint.OneTime);
                }

                if (!ReadData(ref _CurWC, ref _CurrentWcNum))
                {
                    continue;
                }
                ReadWCNum++;
                GlobalUnit.g_MsgControl.OutMessage("第" + ReadWCNum.ToString() + "次读取误差完成，开始分析数据...", false);
                #endregion

                #region 处理数据
                for (int i = 0; i < m_BwCount; i++)
                {
                    #region
                    Application.DoEvents();

                    #region 退出判断
                    if (!m_IsCheck[i])//要检
                    {
                        continue;
                    }
                    if (m_Stop || GlobalUnit.ForceVerifyStop)//停止
                    {
                        GlobalUnit.g_MsgControl.OutMessage("检定停止，操作完毕", false);
                        break;
                    }
                    if (Convert.ToInt32(sth_SpaceTicker.ElapsedMilliseconds / 1000) > m_WCMaxSeconds)//超时
                    {
                        GlobalUnit.g_MsgControl.OutMessage("当前点检定已经超过最大检定时间" + m_WCMaxSeconds + "秒！", false);
                        break;
                    }
                    #endregion

                    //去掉第一次大误差
                    if (!_CheckOver[i] && _CurrentWcNum[i] > 1)
                    {
                        #region ----------数据合法性检测----------
                        /*
                        处理超过255次的情况
                        */
                        if (_MeterWcNum[i] > 0 && _CurrentWcNum[i] < _MeterWcNum[i])
                        {
                            if (_MeterWcNum[i] > _CurrentWcNum[i])
                            {
                                _CurrentWcNum[i] += 255;
                            }
                        }
                        if (_MeterWcNum[i] < _CurrentWcNum[i])
                        {
                            _MeterWcNum[i] = _CurrentWcNum[i] % 255;
                            _VerifyTimes[i]++;
                        }
                        if (_CurrentWcNum[i] == 0 || _CurrentWcNum[i] == 255)//如果本表位没有出误差，换下一表
                        {
                            _CheckOver[i] = false;
                            continue;            
                        }
                        #endregion

                        #region 推箱子,最后一次误差排列在最前面
                        if (_VerifyTimes[i] > 1)
                        {
                            //if (_VerifyTimes[i] > m_Error[i].Wcn.Length) _VerifyTimes[i] = m_Error[i].Wcn.Length;
                            //for (int dtPos = _VerifyTimes[i]; dtPos > 0; dtPos--)
                            for (int dtPos = m_WCTimesPerPoint - 1; dtPos > 0; dtPos--)
                            {
                                m_Error[i].Wcn[dtPos] = m_Error[i].Wcn[dtPos - 1];
                            }
                        }
                        m_Error[i].Wcn[0] = Convert.ToSingle(_CurWC[i]);     //最后一次误差始终放在第一位
                        #endregion

                        #region 处理误差数据
                        if (_VerifyTimes[i] < m_WCTimesPerPoint)
                            m_Error[i].VerifyTimes = _VerifyTimes[i];
                        else
                            m_Error[i].VerifyTimes = m_WCTimesPerPoint;

                        WuChaDeal.SetWuCha(ref m_Error[i]);
                        Application.DoEvents();


                        if (m_Error[i].Item_Result == Variable.CTG_HeGe)
                        {
                            _CheckOver[i] = true;
                        }


                        #endregion

                        #region 完成判断
                        /*
                     * 检测是否所有表都合格
                     * 如果检定次数已经超过每个点检定的误差次数。则检测此点是否已经合格。如果不合格则
                     * 先检测当前误差次数是否已经达到每个点的最大误差差数或当前点检定时间是否已经超过
                     * 预定最大时间。是：则认为此表不合格。否：则清理掉当前点数据重检。
                     * _VerifyTimes[i] > _MaxWCnum 此时会把第一次误差数据清除掉。
                     */
                        if (_VerifyTimes[i] >= m_WCTimesPerPoint)//做完最小次数
                        {
                            
                            if (m_Error[i].Item_Result == Variable.CTG_HeGe)//合格，做完了
                            {
                                _CheckOver[i] = true;
                            }
                            else //不合格
                            {
                                if (_VerifyTimes[i] >= _MaxWCnum)//做完最大次数
                                {
                                    _CheckOver[i] = true;
                                }
                                else
                                {
                                    _CheckOver[i] = false ;
                                }
                            }
                        }
                        else
                        {
                            _CheckOver[i] = false;
                        }
                        #endregion
                    }
                    else if (_CurrentWcNum[i] < 1)//没有误差出，但有一块合格，则判断其它表为不合格，退出检定完成
                    {
                        #region 没有误差出
                        int intJdOverCount = 0;
                        for (int k = 0; k < GlobalUnit.g_BW; k++)
                        {
                            if (_CheckOver[k] && (_CurrentWcNum[k] > _TableHeader)) intJdOverCount++;
                        }
                        if (intJdOverCount > 1)
                        {
                            _CheckOver[i] = true;
                            m_Error[i].Item_Result = Variable.CTG_BuHeGe;
                        }
                        Application.DoEvents();
                        #endregion
                    }
                    #endregion
                }
                #endregion

                #region 处理结果
                m_CheckOver = true;
                for (int j = 0; j < m_BwCount; j++)
                {
                    if (!m_IsCheck[j]) continue;
                    if (!_CheckOver[j])
                    {
                        GlobalUnit.g_MsgControl.OutMessage(string.Format("第{0}块表还没有通过", j + 1));
                        m_CheckOver = false;
                        break;
                    }
                }
                Application.DoEvents();

                #endregion
            }
            #endregion

            #region ----------处理数据----------
            for (int i = 0; i < m_BwCount; i++)
            {
                if (!m_IsCheck[i]) continue;
                if (GlobalUnit.g_Meter.MData[i].MeterErrors.ContainsKey(m_CheckPoint.PrjID))
                {
                    GlobalUnit.g_Meter.MData[i].MeterErrors.Remove(m_CheckPoint.PrjID);
                }
                GlobalUnit.g_Meter.MData[i].MeterErrors.Add(m_CheckPoint.PrjID, m_Error[i]);

                bool IsAllHeGe = true;
                IsAllHeGe = isHeGeOtherWcPoint(GlobalUnit.g_Meter.MData[i].MeterErrors);
                if (this.OnEventMultiControlle != null) this.OnEventMultiControlle(enmMeterPrjID.误差检定, i, IsAllHeGe, IsAllHeGe ? "" : m_CheckPoint.PrjName + "：" + m_Error[i].ToString());

            }
            m_EquipUnit.StopTask(enmTaskType.电能误差);
            GlobalUnit.g_MsgControl.OutMessage("当前检定点检定完毕");
            Application.DoEvents();
            #endregion
        }
        #endregion

        #region -----------私有----------

        /// <summary>
        /// 其它误差点是否合格
        /// </summary>
        /// <param name="curMeter"></param>
        /// <returns></returns>
        private bool isHeGeOtherWcPoint(Dictionary<string, MeterErrorItem> curMeter)
        {
            bool isAllItemOk = true;
            foreach (string strKey in curMeter.Keys)
            {
                //当前功率方向
                MeterErrorItem _Item = curMeter[strKey];
                if (_Item.Item_Result != Variable.CTG_HeGe)
                {
                    isAllItemOk = false;
                    break;
                }
            }
            return isAllItemOk;
        }

        /// <summary>
        /// 读取检定数据,数据类型为当前检定数据
        /// </summary>
        /// <param name="arrData">功能数组，每表位对应</param>
        /// <param name="arrWcCount">误差次数数组，每表位对应</param>
        /// <returns>是否读取成功</returns>
        private bool ReadData(ref string[] arrData, ref int[] arrWcCount)
        {
            bool[] bln_Result = new bool[24];
            if (!m_EquipUnit.ReadTaskData(0x00,ref bln_Result , ref arrData, ref arrWcCount))
            {
                Thread.Sleep(10);
                return false;
            }
            /*
            对取到的误差次数进行排序，如果最后一个次数还是为0的话则认为全部都没有出误差
            */
            int[] _CurrentWcNumCopy = (int[])arrWcCount.Clone();
            Array.Sort(_CurrentWcNumCopy);
            if (_CurrentWcNumCopy[_CurrentWcNumCopy.Length - 1] == 0)
            {
                Thread.Sleep(1);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 初始化设备参数：[基本误差]
        /// </summary>
        /// <returns></returns>
        private bool InitEquipment()
        {
            /*
            选通道--》设置被检表参数--》设置功能参数-->设置工作点-->启动当前功能
            */
            //第一次默认为采用脉冲盒共阳。
            //if (GlobalUnit.IsDemo) return true;

            bool SetTestPointResult = false;

            float PowerIb = m_CheckPoint.xIb * GlobalUnit.g_Meter.MInfo.Ib;

            float PowerUb = GlobalUnit.g_Meter.MInfo.Ub;

            #region ----------------设置回路----------------
            //if (m_DLHL != m_CheckPoint.DLHL)
            //{
            //    m_EquipUnit.PowerOff();
            //    m_EquipUnit.DelayTime(200);

            //    SetTestPointResult = m_EquipUnit.SetCurrentLoop(m_CheckPoint.DLHL);
            //    if (!SetTestPointResult)
            //    {
            //        if (!m_Stop)
            //            pwClassLibrary.Check.Require(false, "设置电流回路失败" + m_EquipUnit.m_IComAdpater.LostMessage);

            //    }
            //    //等待源稳定
            //    m_EquipUnit.DelayTime(200);
            //}
            //m_DLHL = m_CheckPoint.DLHL;
            //if (GlobalUnit.ForceVerifyStop) return false;

            #endregion

            #region ----------------输出电压电流----------------

            pwFunction.pwConst.GlobalUnit.g_MsgControl.OutMessage("开始设置检定点参数："
                + GlobalUnit.g_Meter.MInfo.Clfs.ToString() + " | "
                + PowerUb.ToString() + " | "
                + PowerIb.ToString() + " | "
                + m_CheckPoint.GLYS + " | "
                + m_CheckPoint.YJ+ " | "
                , false);
            SetTestPointResult = m_EquipUnit.SetTestPoint(GlobalUnit.g_Meter.MInfo.Clfs, PowerUb, PowerIb, m_CheckPoint.GLYS, m_CheckPoint.YJ, GlobalUnit.g_Meter.MInfo.PL);
            if (!SetTestPointResult)
            {
                if (!m_Stop)
                    pwClassLibrary.Check.Require(false, "设置检定点失败" );

            }
            if (m_CheckPoint.xIb <= 0.05f)//小电流时标准表常数不稳，需要增加等待时间
            {
                m_EquipUnit.DelayTime(6000);//等待源稳定
            }
            else
            {
                m_EquipUnit.DelayTime(2000);//等待源稳定
            }
            if (GlobalUnit.ForceVerifyStop) return false;
            #endregion

            #region ----------------设置功能参数----------------
            byte intBs = 1;//常数倍数
            long lngConstant = Convert.ToInt64(GlobalUnit.g_Meter.MInfo.Constant);
            if (PowerIb < GlobalUnit.g_Meter.MInfo.Ib)
            {
                //intBs = (byte)m_WcConstantBs;
                lngConstant = lngConstant * m_WcConstantBs;
            }

            enmChannelType _CurTdType = enmChannelType.正向有功;
            switch (m_CheckPoint.GLFX)
            {
                case enmsPowerFangXiang.正向有功:
                    _CurTdType = enmChannelType.正向有功;
                    break;
                case enmsPowerFangXiang.反向有功:
                    _CurTdType = enmChannelType.反向有功;
                    break;
                case enmsPowerFangXiang.正向无功:
                    _CurTdType = enmChannelType.正向无功;
                    break;
                case enmsPowerFangXiang.反向无功:
                    _CurTdType = enmChannelType.反向无功;
                    break;
            }

            pwFunction.pwConst.GlobalUnit.g_MsgControl.OutMessage("开始设置功能参数："
                + GlobalUnit.g_Meter.MInfo.enmDzType.ToString() + " | "
                + enmTaskType.电能误差.ToString() + " | "
                + _CurTdType.ToString() + " | "
                + GlobalUnit.g_Meter.MInfo.PulseType.ToString() + " | "
                + GlobalUnit.g_Meter.MInfo.GYGY.ToString() + " | "
                + lngConstant.ToString() + " | "
                + m_CheckPoint.QS.ToString() + " | "
                + intBs.ToString() + " | "
               , false);
            if (!m_EquipUnit.SetTaskParameter(
                                            GlobalUnit.g_Meter.MInfo.enmDzType
                                            , enmTaskType.电能误差
                                            , _CurTdType
                                            , GlobalUnit.g_Meter.MInfo.PulseType// enmPulseComType.脉冲盒
                                            , GlobalUnit.g_Meter.MInfo.GYGY //enmGyGyType.共阴
                                            , lngConstant
                                            , Convert.ToInt64(m_CheckPoint.QS)
                                            , intBs)
                )
            {
                m_Stop = true;
                pwFunction.pwConst.GlobalUnit.g_MsgControl.OutMessage("开始设置功能参数：失败");
                return false;
                //pwClassLibrary.Check.Require(false, "设置功能参数失败!" + m_EquipUnit.m_IComAdpater.LostMessage);

            }
            if (GlobalUnit.ForceVerifyStop) return false;
            #endregion






            #region ----------------初始化详参数----------------

            if (!InitErrorData())
                return false ;
            if (GlobalUnit.ForceVerifyStop) return false;
            #endregion

            return true;
        }

        /// <summary>
        /// 初始化误差检定数据
        /// </summary>
        /// <returns></returns>
        private bool InitErrorData()
        {
            try
            {
                m_Error = new MeterErrorItem[m_BwCount];

                for (int i = 0; i < m_Error.Length; i++)
                {
                    m_Error[i] = new MeterErrorItem();
                    m_Error[i].Me_Bw = i;
                    m_Error[i].Item_PrjID = m_CheckPoint.PrjID;
                    m_Error[i].Item_PrjName = m_CheckPoint.PrjName;
                    m_Error[i].MeterLevel = GlobalUnit.g_Meter.MInfo.DJ;
                    m_Error[i].MaxError = m_CheckPoint.MaxError;
                    m_Error[i].MinError = m_CheckPoint.MinError;
                    m_Error[i].intPc = m_CheckPoint.PC;

                    if (m_CheckPoint.PC == 1)//偏差次数
                    {
                        m_WCTimesPerPoint = m_WindageTimesPerPoint;// GlobalUnit.WCTimesPerPoint;//最小次数
                        m_WCMaxTimes = m_WindageTimesPerPoint + 2;// GlobalUnit.WCMaxTimes;//最大次数
                        m_WCMaxSeconds = GlobalUnit.WCMaxSeconds * 2 ;//时间
                        m_Error[i].VerifyTimes = m_WindageTimesPerPoint;
                    }
                    else
                    {
                        m_WCTimesPerPoint = GlobalUnit.WCTimesPerPoint;//最小次数
                        m_WCMaxTimes = GlobalUnit.WCMaxTimes;//最大次数
                        m_WCMaxSeconds = GlobalUnit.WCMaxSeconds;//时间
                        m_Error[i].VerifyTimes = m_WCTimesPerPoint;

                    }

                }
                return true;
            }
            catch
            {
                return false;
            }
        }
  
        #endregion


    }


}