

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using pwFunction.pwConst;
using System.Diagnostics;
using System.Windows.Forms;
using pwInterface;
using VerificationEquipment.Commons;
namespace VerifyAdapter
{

    public class Dgn_ClockError 
    {
        #region -------变量声明----------
        private Stopwatch sth_SpaceTicker = new Stopwatch();        //记时时钟
        private VerifyAdapter.EquipUnit m_EquipUnit = null;         //装置
        private MeterDgnItem[] m_Dgn = new MeterDgnItem[24];        //多功能检定子项目结果

        private MeterErrorItem[] m_Error = new MeterErrorItem[24];  //误差数据，检定生成
        private int m_BwCount = 24;                                 //表位
        private bool m_Stop = false;                                //是否停止
        protected bool m_CheckOver = false;                         //是否已经完成检定
        private bool[] m_IsCheck = new bool[24];                    //要检
        private stPrjParameter m_stPrjParameter;                    //项目参数结构

        #endregion

        /// <summary>
        /// 每一个误差点取几次误差参与计算(计算次数)
        /// </summary>
        private int m_WCTimesPerPoint = GlobalUnit.WCTimesPerPoint;

        /// <summary>
        /// 每一个误差点最多读取多少次误差(最大次数)
        /// </summary>
        private int m_WCMaxTimes = GlobalUnit.WCMaxTimes;

        /// <summary>
        /// 每一个误差点最多读多少秒(最长时间)
        /// </summary>
        private int m_WCMaxSeconds = GlobalUnit.WCMaxSeconds;

        /// <summary>
        /// 检定圈数
        /// </summary>
        private int m_intTimePulseQs = 10;

        #region -----------属性------
        private pwInterface.StDgnItem m_DngItem = new pwInterface.StDgnItem();
        /// <summary>
        /// 多功能子项目
        /// </summary>
        public pwInterface.StDgnItem DngItem
        {
            set
            {
                m_DngItem = value;
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
                m_EquipUnit.StopTask(enmTaskType.时钟日误差);
            }
            get { return m_Stop; }
        }

        /// <summary>
        /// 是否要检
        /// </summary>
        public bool[] blnSelected
        {
            set
            {
                m_IsCheck = value;
            }
            get { return m_IsCheck; }
        }

        /// <summary>
        /// 项目参数结构
        /// </summary>
        public stPrjParameter stPrjParameter
        {
            set
            {
                m_stPrjParameter = value;
                m_intTimePulseQs = m_stPrjParameter.int_TimePulseQs;
            }
        }

        public event DelegateEventMultiController OnEventMultiControlle;
        #endregion


        #region -------方法----------

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="BwCount"></param>
        /// <param name="ComAdpater"></param>
        public Dgn_ClockError(int BwCount, VerifyAdapter.EquipUnit ComAdpater)
        {
            m_BwCount = BwCount;
            m_EquipUnit = ComAdpater;

            m_Error = new MeterErrorItem[m_BwCount];
            m_IsCheck = new bool[m_BwCount];
            m_Dgn = new MeterDgnItem[m_BwCount];
        }
        
        /// <summary>
        /// 检定
        /// </summary>
        public void Verify()
        {

            #region ----------检定准备----------
            GlobalUnit.g_MsgControl.OutMessage("开始初始化检定参数...", false);

            int _TableHeader = m_WCTimesPerPoint;           //记录误差数据，推箱子用，用来计算的有效误差次数
            int _MaxWCnum = m_WCMaxTimes;                   //最大误差次数
            m_CheckOver = false;                            //还原检定标识，没有检定完成
            m_Stop = false;
            #endregion

            #region ----------初始化设备参数：[日计时误差]:InitEquipment----------

            if (!InitEquipment())
            {
                #region 升源失败
                for (int i = 0; i < m_BwCount; i++)
                {
                    if (!m_IsCheck[i]) continue;
                    m_Dgn[i] = new MeterDgnItem();
                    m_Dgn[i].Me_Bw = i;
                    m_Dgn[i].Item_PrjID = m_DngItem.PrjID;
                    m_Dgn[i].Item_PrjName = m_DngItem.PrjName;
                    m_Dgn[i].Item_Result = Variable.CTG_BuHeGe;
                    m_Dgn[i].Item_Value = "";
                    if (GlobalUnit.g_Meter.MData[i].MeterDgns.ContainsKey(m_DngItem.PrjID))
                    {
                        GlobalUnit.g_Meter.MData[i].MeterDgns.Remove(m_DngItem.PrjID);
                    }
                    GlobalUnit.g_Meter.MData[i].MeterDgns.Add(m_DngItem.PrjID, m_Dgn[i]);

                    if (this.OnEventMultiControlle != null) this.OnEventMultiControlle(enmMeterPrjID.日计时误差检定, i, false ,  "升源失败" );

                }
                #endregion

                return;

            }
            Application.DoEvents();
            #endregion

            #region ----------初始化变量参数----------

            m_Error = new MeterErrorItem[m_BwCount];
            for (int i = 0; i < m_Error.Length; i++)
            {
                m_Error[i] = new MeterErrorItem();
                m_Error[i].Item_PrjID = m_DngItem.PrjID;
                m_Error[i].Item_PrjName = m_DngItem.PrjName;
                m_Error[i].MeterLevel = GlobalUnit.g_Meter.MInfo.DJ;
                m_Error[i].MaxError = 0.4f;
                m_Error[i].MinError = -0.4f;
            }


            string[] _CurWC = new string[m_BwCount];
            int[] _CurrentWcNum = new int[m_BwCount];         //当前累计检定次数
            int[] _MeterWcNum = new int[m_BwCount];           //表位取误差次数，处进超过255次
            int[] _VerifyTimes = new int[m_BwCount];          //有效误差次数
            bool[] _CheckOver = new bool[m_BwCount];          //是否已经完成本次检定
            GlobalUnit.g_MsgControl.OutMessage("正在检定......", false);
            int ReadWCNum = 0;
            int m_WCMaxSeconds = 300;                       //最大做300秒
            sth_SpaceTicker.Reset();
            sth_SpaceTicker.Start();       //开始记时             
            #endregion 

            #region ----------循环检定----------
            m_EquipUnit.m_IComAdpater.ExecuteClockError(GlobalUnit.g_Meter.MInfo.Ub, 50, 50, 10, 5, dgtMeterDayErr);
            #endregion 

        }

        private ReturnSampleDataErrDelegate dgtMeterDayErr = new ReturnSampleDataErrDelegate(Up_MeterDayErr); 
        private static void Up_MeterDayErr(string[,] val, int[] reSult)
        {

            MeterDgnItem meterDgnErr;
            int count = val.Length / reSult.Length;//误差个数
            for (int i = 0; i < reSult.Length; i++)
            {

                if (!m_IsCheck[i]) continue;
                #region
                m_Dgn[i] = new MeterDgnItem();
                m_Dgn[i].Item_PrjID = m_DngItem.PrjID;
                m_Dgn[i].Item_PrjName = m_DngItem.PrjName;
                m_Dgn[i].Me_Bw = i + 1;
                m_Dgn[i].Item_Result = reSult[i] == 1 ? Variable.CTG_HeGe : Variable.CTG_BuHeGe;
                for (int j = 0; j < count; j++)
                {
                    m_Dgn[i].Item_Value += val[i, j] + "|";
                }

                #endregion
                if (GlobalUnit.g_Meter.MData[i].MeterDgns.ContainsKey(m_DngItem.PrjID))
                {
                    GlobalUnit.g_Meter.MData[i].MeterDgns.Remove(m_DngItem.PrjID);
                }
                GlobalUnit.g_Meter.MData[i].MeterDgns.Add(m_DngItem.PrjID, m_Dgn[i]);

                bool IsHeGe = reSult[i] == 1 ? true : false;
                if (this.OnEventMultiControlle != null) this.OnEventMultiControlle(enmMeterPrjID.日计时误差检定, i, IsHeGe, IsHeGe ? "" : m_DngItem.PrjName + "：" + m_Dgn[i].Item_Value);

            }
            GlobalUnit.g_MsgControl.OutMessage( "日计时误差 检定完毕");
            Application.DoEvents();

        }

        #endregion

        #region -------私有----------

        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <returns></returns>
        private bool InitEquipment()
        {
            if (GlobalUnit.IsDemo) return false ;
            try
            {

                if (!m_EquipUnit.PowerOnOnlyU())
                {
                    throw new Exception("控制源输出失败！");
                }
                m_EquipUnit.DelayTime(100);
            }
            catch (System.Exception e)
            {
                return false;
            }
            return true;
        }


        #endregion


    }



}
