using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Data;
using pwCommAdapter;
using pwFunction;
using pwFunction.pwConst;
using pwFunction.pwEnum;

namespace VerifyAdapter
{
    /// <summary>
    /// 检定控制器
    /// 根据当前检定项目创建对应检定器，启动检定线程完成检定
    /// </summary>
    public class Adapter
    {
        #region ------------静态成员-------------

        private static EquipUnit m_ComAdpater = null;
        private static ClAmMeterController.CMultiController m_485Control = null;
        private static ClAmMeterController.pwMeterProtocolInfo m_ProtocolInfo = null;
        private static int m_BwCount = 24;
        private static int m_ChancelCount = 24;
        private static int m_reTryTime = 3;


        /// <summary>
        /// 设备控制单元
        /// 静态设计，所有检定器共享
        /// </summary>   
        public static EquipUnit g_ComAdpater
        {
            set { m_ComAdpater = value; }
            get { return m_ComAdpater; }
        }

        /// <summary>
        /// 485控制单元，
        /// </summary>
        public static ClAmMeterController.CMultiController g_485Control
        {
            set { m_485Control = value; }
            get { return m_485Control; }
        }

        /// <summary>
        /// 协议信息
        /// </summary>
        public static ClAmMeterController.pwMeterProtocolInfo g_ProtocolInfo
        {
            set { m_ProtocolInfo = value; }
            get { return m_ProtocolInfo; }
        }


        /// <summary>
        /// 挂表架表位数量
        /// </summary>
        public static int BwCount
        {
            set { m_BwCount = value; }
            get { return m_BwCount; }
        }

        /// <summary>
        /// 485通道数量
        /// </summary>
        public static int ChancelCount
        {
            get { return Adapter.m_ChancelCount; }
            set { Adapter.m_ChancelCount = value; }
        }

        /// <summary>
        /// 当前检定状态
        /// </summary>
        /// <returns></returns>
        public static bool IsStop()
        {
            return GlobalUnit.g_Status == Enum_Status.IsStop;
        }

        /// <summary>
        /// 操作重试次数
        /// </summary>
        public static int RetryTime
        {
            get { return m_reTryTime; }
        }


        #endregion




        #region----------构造函数----------
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bwCount">挂表架表位数量</param>
        /// <param name="ReTeyTime">操作失败重试次数</param>
        public Adapter(int bwCount, int ReTeyTime)
        {
            //初始化设备控制单元
            g_ComAdpater = new EquipUnit(bwCount, ReTeyTime);
            g_485Control = new ClAmMeterController.CMultiController(pwFunction.pwConst.GlobalUnit.g_BW);
            g_ProtocolInfo = new ClAmMeterController.pwMeterProtocolInfo();
        }
        #endregion

        #region 读生产编号
        public void  ReadScbh()
        {
            g_ProtocolInfo = new ClAmMeterController.pwMeterProtocolInfo("DLT645_1997");
            g_485Control.RefreshProtocol(g_ProtocolInfo);
            g_485Control.ReadScbh("FFF9");
        }

        #endregion

        #region 误差检定


        #endregion

        #region 多功能测试


        #endregion

        #region 打包参数下载
        public void DownPara()
        {
            Adapter.g_ProtocolInfo = new ClAmMeterController.pwMeterProtocolInfo("DLT645_2007");
            Adapter.g_485Control.RefreshProtocol(Adapter.g_ProtocolInfo);
            Adapter.g_485Control.DownPara(GlobalUnit.g_Plan.cDownParaItem._DownParaItem);
        }

        #endregion

        #region 系统清零


        #endregion



    }
}
