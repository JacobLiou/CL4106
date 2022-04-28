using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.DeviceCommon
{
    public class MonitorData
    {
        //监视仪表
        private string[] u = new string[3];//ABC电压

        private string[] i = new string[3];//电流 

        private string[] p = new string[3];//有功功率

        private string[] q = new string[3];//无功功率

        private string[] angle = new string[3];//相位

        private string[] angleLinePhase = new string[3];//线相位 


        private string freq;//频率

        private string pTotal = "";
        //No.194-257 ΣP,ΣPs,ΣQ,ΣCoSΦ,Φ12,Φ23,Φ31,Fr
        private string psTotal = "";

        private string qTotal = "";

        private bool haveU = false;

        private int stemConst = 0;

        /// <summary>
        /// 当前电压电流状态下，标准表常数值
        /// </summary>
        public int StemConst
        {
            get { return stemConst; }
            set { stemConst = value; }
        }

        /// <summary>
        /// 是否有电压的标志
        /// </summary>
        public bool HaveU
        {
            get { return haveU; }
            set { haveU = value; }
        }

        /// <summary>
        /// 线相位。分别对应：Φ12,Φ23,Φ31
        /// </summary>
        public string[] AngleLinePhase
        {
            get { return angleLinePhase; }
            set { angleLinePhase = value; }
        }

        /// <summary>
        /// 无功总功率
        /// </summary>
        public string QTotal
        {
            get { return qTotal; }
            set { qTotal = value; }
        }

        /// <summary>
        /// 视在功率
        /// </summary>
        public string PsTotal
        {
            get { return psTotal; }
            set { psTotal = value; }
        }

        /// <summary>
        /// 总有功功率
        /// </summary>
        public string PTotal
        {
            get { return pTotal; }
            set { pTotal = value; }
        }

        //以上字段的属性。
        /// <summary>
        /// 电压，单位V，数组，0-2分别对应ABC
        /// </summary>
        public string[] U
        {
            get { return u; }
            set { u = value; }
        }

        /// <summary>
        /// 电流，单位A，数组，0-2分别对应ABC
        /// </summary>
        public string[] I
        {
            get { return i; }
            set { i = value; }
        }

        /// <summary>
        /// 有功功率，单位kw，数组，0-2分别对应ABC
        /// </summary>
        public string[] P
        {
            get { return p; }
            set { p = value; }
        }

        /// <summary>
        /// 无功功率，单位kw，数组，0-2分别对应ABC
        /// </summary>
        public string[] Q
        {
            get { return q; }
            set { q = value; }
        }

        /// <summary>
        /// 相位，单位°，数组，0-2分别对应ABC
        /// </summary>
        public string[] Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        /// <summary>
        /// 频率，单位Hz
        /// </summary>
        public string Freq
        {
            get { return freq; }
            set { freq = value; }
        }
    }
}
