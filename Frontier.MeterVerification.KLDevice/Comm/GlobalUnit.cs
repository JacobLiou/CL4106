
#region FileHeader And Descriptions
// ***************************************************************
//  GlobalUnit   date: 11/25/2009 By Niaoked
//  -------------------------------------------------------------
//  Description:
//  全局模块。
//  -------------------------------------------------------------
//  Copyright (C) 2009 -CdClou All Rights Reserved
// ***************************************************************
// Modify Log:
// 11/25/2009 13-43-13    Created
// ***************************************************************
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Frontier.MeterVerification.KLDevice
{
    /// <summary>
    /// 全局变量类
    /// </summary>
    public class GlobalUnit
    {
        #region 变量声明   
     
        /// <summary>
        /// 当前标准表升源信息
        /// </summary>
        private static stStdInfo _StdMeterInfo;
        /// <summary>
        /// 当前标准表升源信息
        /// </summary>
        public static stStdInfo g_StdMeterInfo
        {
            get { return _StdMeterInfo; }
            set
            {
                _StdMeterInfo = value;
            }
        }        
        /// <summary>
        /// 当前检定任务类型
        /// </summary>
        private static int _CurTestType = 00;

        /// <summary>
        /// 获取设置当前检定任务类型
        /// //任务类型,0=电能误差,1=需量周期,2=日计时误差,3=计数,4=对标,5=预付费功能检定,6=耐压实验,7=多功能脉冲计数试验
        /// </summary>
        public static int g_CurTestType
        {
            get { return _CurTestType; }
            set
            {
                _CurTestType = value;
            }
        }        

        /// <summary>
        /// While循环时线程休眠时间，单位：MS
        /// </summary>
        public static int g_ThreadWaitTime = 1;

        /// <summary>
        /// 标准表功率[0:有功功率1:无功功率:2:视在功率]
        /// </summary>
        public static float[] g_StrandMeterP = new float[3];
        
        #endregion
        /// <summary>
        /// 台体硬件配置
        /// 标准表（0-3115,2-311V2）功率源（0-309,1-303）精密时基源（0-191B）误差板（0-L，1-E，2-G，3-H）
        /// 默认“0000”：3115,309,191B，188L
        /// </summary>
        public static string DriverTypes = "0101";

        /// <summary>
        /// 0= 脉冲盒，1=其他
        /// </summary>
        public static int isPushBox = 0;
        /// <summary>
        /// 测量方式
        /// </summary>
        public static Cus_Clfs Clfs
        {
            get
            {
                //MeterBasicInfo meterinfo = Meter(FirstYaoJianMeter);
                //if (meterinfo == null) return Cus_Clfs.三相四线;
                //return (Comm.Enum.Cus_Clfs)meterinfo.Mb_intClfs;
                return Cus_Clfs.三相四线;
            }
        }

        
        
        /// <summary>
        /// 是否是单相台
        /// </summary>
        public static bool IsDan
        {
            get
            {
                return false;
            }
        }        
    }
}
