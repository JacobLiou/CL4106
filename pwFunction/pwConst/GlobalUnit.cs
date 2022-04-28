using System;
using System.Collections.Generic;
using System.Text;
using pwInterface;
using pwFunction;
using pwClassLibrary;
namespace pwFunction.pwConst
{
    
    /// <summary>
    /// 全局变量类
    /// </summary>
    public class GlobalUnit
    {
        #region 变量声明

        /// <summary>
        /// 系统配置模型，公用
        /// </summary>
        public static pwFunction.pwSystemModel.SystemInfo g_SystemConfig = null;

        /// <summary>
        /// 工单信息
        /// </summary>
        public static pwFunction.pwWork.cWork g_Work = null;


        /// <summary>
        /// 产品信息
        /// </summary>
        public static pwFunction.pwProducts.cProducts g_Products = null;

        /// <summary>
        /// 方案信息
        /// </summary>
        public static pwFunction.pwPlan.cPlan g_Plan = null;

        /// <summary>
        /// 标准表测量数据
        /// </summary>
        public static stPower g_StdMeter = new stPower();


        /// <summary>
        /// 表基本信息
        /// </summary>
        public static pwFunction.pwMeter.MeterBasic g_Meter = null;

        /// <summary>
        ///事件队列 
        /// </summary>
        public static pwClassLibrary.VerifyMsgControl g_MsgControl = null;

        ///// <summary>
        ///// 日志队列
        ///// </summary>
        //public static pwClassLibrary.RunLog g_Log = null;


        ///// <summary>
        ///// 数据队列
        ///// </summary>
        //public static pwClassLibrary.VerifyMsgControl g_DataControl = null;
        ///// <summary>
        ///// 通讯数据队列
        ///// </summary>
        //public static VerifyMsgControl g_485DataControl = null;


        private static bool m_ApplicationIsOver = false;
        /// <summary>
        /// 应用程序是否已经退出.用于结束线程(用于系统退出控制)
        /// </summary>
        public static bool ApplicationIsOver
        {
            set
            {
                m_ApplicationIsOver=value ;
                if(g_MsgControl!=null )g_MsgControl.ApplicationIsOver = value;
                //if (g_DataControl != null) g_DataControl.ApplicationIsOver = value;
                //if (g_Log != null) g_Log.ApplicationIsOver = value;
            }
            get
            {
                return m_ApplicationIsOver;
            }
        }

        private static bool m_ForceVerifyStop = false;
        /// <summary>
        /// 强制停止(用于系统按键停止控制)
        /// </summary>
        public static bool ForceVerifyStop
        {
            set
            {
                m_ForceVerifyStop = value;
                if (m_Status == enmStatus.进行中) g_MsgControl.OutMessage("系统正在停止......",false);
            }

            get
            {
                return m_ForceVerifyStop;
            }
        }

        private static enmStatus m_Status = enmStatus.空闲;
        /// <summary>
        /// 执行状态(用于程序运行控制，正常模式)
        /// </summary>
        public static enmStatus g_Status
        {
            set
            {
                m_Status = value;
                if (g_MsgControl != null) g_MsgControl.OutMessage("系统当前状态：" + value.ToString(), false);

            }

            get
            {
                return m_Status;
            }

        }


        private static enmPowerStatus m_PowerStatus = enmPowerStatus.空闲;
        /// <summary>
        /// 功率源当前状态
        /// </summary>
        public static enmPowerStatus g_PowerStatus
        {
            set
            {
                m_PowerStatus = value;
                if (g_MsgControl != null) g_MsgControl.OutMessage("功率源当前状态：" + value.ToString(), false);

            }

            get
            {
                return m_PowerStatus;
            }

        }


        /// <summary>
        /// While循环时线程休眠时间，单位：MS
        /// </summary>
        public static int g_ThreadWaitTime = 1;

        //修改锁
        public static object objUpdateActiveIDLock = new object();

        #endregion

        public static  string TableNameaChame;

        #region 读取系统配置
        /// <summary>
        /// 取配置[文本类型]
        /// </summary>
        /// <param name="strKey">主键</param>
        /// <param name="DefaultValue">默认值</param>
        /// <returns></returns>
        public static string GetConfig(string strKey, string DefaultValue)
        {
            if (g_SystemConfig == null)
                return DefaultValue;
            string strTmp = g_SystemConfig.SystemMode.GetValue(strKey);
            if (strTmp.Length == 0 || strTmp == "" || strTmp == null || strTmp == string.Empty)
                return DefaultValue;
            return strTmp;
        }

        /// <summary>
        /// 取配置[Int类型]
        /// </summary>
        /// <param name="strKey">主键</param>
        /// <param name="DefaultValue">默认值</param>
        /// <returns></returns>
        public static int GetConfig(string strKey, int DefaultValue)
        {
            string strValue = GetConfig(strKey, DefaultValue.ToString());
            if (!pwClassLibrary.Number.IsNumeric(strValue))
                return DefaultValue;
            return int.Parse(strValue);
        }

        /// <summary>
        /// 取配置[Float类型]
        /// </summary>
        /// <param name="strKey">主键</param>
        /// <param name="DefaultValue">默认值</param>
        /// <returns></returns>
        public static float GetConfig(string strKey, float DefaultValue)
        {
            string strValue = GetConfig(strKey, DefaultValue.ToString());
            if (!pwClassLibrary.Number.IsNumeric(strValue))
                return DefaultValue;
            return float.Parse(strValue);
        }
        #endregion


        #region----------系统信息----------

        /// <summary>
        /// 是否演模式
        /// </summary>
        public static bool IsDemo = false;

        private static string  m_DeskNo ="1";

        /// <summary>
        /// 台体编号
        /// </summary>
        public static string g_DeskNo
        {
            set
            {
                m_DeskNo = value;
            }
            get { return GetConfig(pwFunction.pwConst.Variable.CTC_DESKNO, "1"); }
        }

        /// <summary>
        /// 台体类型
        /// </summary>
        public static enmDesktype g_Desktype
        {
            get 
            { 
                string strDesktype=GetConfig(pwFunction.pwConst.Variable.CTC_DESKTYPE, "单相检定台");
                enmDesktype _Desktype;
                switch(strDesktype)
                {
                    case "单相检定台":
                        _Desktype=enmDesktype.单相检定台;
                        break ;
                    case "单相前装台":
                        _Desktype=enmDesktype.单相前装台;
                        break ;
                    case "单相后装台":
                        _Desktype=enmDesktype.单相后装台;
                        break ;
                    case "三相检定台":
                        _Desktype=enmDesktype.三相检定台;
                        break ;
                    case "三相前装台":
                        _Desktype=enmDesktype.三相前装台;
                        break ;
                    case "三相后装台":
                        _Desktype=enmDesktype.三相后装台;
                        break ;
                    default :
                        _Desktype = enmDesktype.单相检定台;
                        break;

                }
                return _Desktype;
            }
        }

        //<summary>
        // 表位数
        //</summary>
        public static int g_BW
        {
            get { return GetConfig(pwFunction.pwConst.Variable.CTC_BWCOUNT, 12); }
        }

        /// <summary>
        /// 源稳定时间
        /// </summary>
        public static int g_PowerWaitTime
        {
            get { return GetConfig(pwFunction.pwConst.Variable.CTC_OTHER_POWERON_ATTERTIME, 3); }
        }

        /// <summary>
        /// 读写表最大等待时间
        /// </summary>
        public static int g_MaxWriteMeterWaitTime
        {
            get { return GetConfig(pwFunction.pwConst.Variable.CTC_OTHER_MAXWAITDATABACKTIME, 45); }
        }

        /// <summary>
        /// 台体是否有CL191B及误差计算板   是：有；否：没有
        /// </summary>
        public static bool IsCL191BCL188L
        {
            get { return pwFunction.pwConst.GlobalUnit.GetConfig(pwFunction.pwConst.Variable.CTC_ISCL191BCL188L, "否") == "否" ? false  : true ; }
        }


        #region -------误差控制属性，从系统配置可读取----------
        /// <summary>
        /// 每一个误差点取几次误差参与计算(计算次数)
        /// </summary>
        public static int WCTimesPerPoint
        {
            get { return pwFunction.pwConst.GlobalUnit.GetConfig(pwFunction.pwConst.Variable.CTC_WC_TIMES_BASICERROR, 2); }
        }
        /// <summary>
        /// 标准偏差点取几次误差参与计算
        /// </summary>
        public static int WindageTimesPerPoint
        {
            get { return pwFunction.pwConst.GlobalUnit.GetConfig(pwFunction.pwConst.Variable.CTC_WC_TIMES_WINDAGE, 5); }
        }

        /// <summary>
        /// 每一个误差点最多读取多少次误差(最大次数)
        /// </summary>
        public static int WCMaxTimes
        {
            get { return pwFunction.pwConst.GlobalUnit.GetConfig(pwFunction.pwConst.Variable.CTC_WC_MAXTIMES, 15); }
        }
        /// <summary>
        /// 每一个误差点最多读多少秒(最长时间)
        /// </summary>
        public static int WCMaxSeconds
        {
            get { return pwFunction.pwConst.GlobalUnit.GetConfig(pwFunction.pwConst.Variable.CTC_WC_MAXSECONDS, 300); }
        }

        /// <summary>
        /// 跳差判定标准
        /// </summary>
        public static float WCJump
        {
            get { return pwFunction.pwConst.GlobalUnit.GetConfig(pwFunction.pwConst.Variable.CTC_WC_JUMP, 2f); }
        }

        /// <summary>
        /// 是否系统自动计算误差圈数   是：系统自动计算圈数；否：使用方案圈数
        /// </summary>
        public static bool WcSystemQs
        {
            get { return pwFunction.pwConst.GlobalUnit.GetConfig(pwFunction.pwConst.Variable.CTC_WC_SYSTEMQS, "是")=="是"?true :false ; }
        }

        /// <summary>
        ///标准脉冲分频系数
        /// </summary>
        public static float WcDriverF
        {
            get { return pwFunction.pwConst.GlobalUnit.GetConfig(pwFunction.pwConst.Variable.CTC_OTHER_DRIVERF, 1f); }
        }
        /// <summary>
        /// 电表长度
        /// </summary>
        public static int WcTableNoLenth {
            get {
                return pwFunction.pwConst.GlobalUnit.GetConfig(pwFunction.pwConst.Variable.CTC_READTABLELENGTH, 12);
            }
        }
        /// <summary>
        /// 电表是否扫描输入
        /// </summary>
        public static bool WcReadTableNo
        {
            get { return pwFunction.pwConst.GlobalUnit.GetConfig(pwFunction.pwConst.Variable.CTC_READTABLENO, "是") == "是" ? true : false; }
        }

        #endregion

        #endregion
    }
    

}
