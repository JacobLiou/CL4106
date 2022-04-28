using System;
using System.Collections.Generic;
using System.Text;

namespace Frontier.MeterVerification.KLDevice
{
    /// <summary>
    /// 电压参数
    /// </summary>
    public struct UIPara
    {
        public double Ua;
        public double Ub;
        public double Uc;
        public double Ia;
        public double Ib;
        public double Ic;
    }
    public struct PhiPara
    {
        public double PhiUa;
        public double PhiUb;
        public double PhiUc;
        public double PhiIa;
        public double PhiIb;
        public double PhiIc;
    }
    /// <summary>
    /// 读取的电源信息
    /// </summary>
    public struct stStdInfo
    {
        public Cus_Clfs Clfs;  //	 接线方式	
        public byte Flip_ABC;     //   	'相位开关控制	
        public float Freq;//	'频率	
        public byte Scale_Ua;// 	'Ua档位 	
        public byte Scale_Ub;// 	'Ub档位 	
        public byte Scale_Uc;// 	'Uc档位 	
        public byte Scale_Ia;// 	'Ia档位 	
        public byte Scale_Ib;// 	'Ib档位 	
        public byte Scale_Ic;// 	'Ic档位 	
        public float Ua;//	'UA 
        public float Ia;//	'Ia 	
        public float Ub;//	'UB  	
        public float Ib;// Ib 	
        public float Uc;// 	'UC 	
        public float Ic;// 'Ic 	
        public float Phi_Ua;// 	'Ua相位 	
        public float Phi_Ia;// 	'Ia相位 	
        public float Phi_Ub;//	'UB相位 	
        public float Phi_Ib;// 	'Ib相位 	
        public float Phi_Uc;// 	'UC相位 	
        public float Phi_Ic;// 	'Ic相位 	
        public float Pa;// 	'A相有功功率 	
        public float Pb;// 	'B相有功功率	
        public float Pc;// 	'C相有功功率	
        public float Qa;//	'A相无功功率	
        public float Qb;//	'B相无功功率	
        public float Qc;//	'C相无功功率	
        public float Sa;//	'A相视在功率	
        public float Sb;// 	'B相视在功率	
        public float Sc;// 	'C相视在功率	
        public float P;//	总有功功率	
        public float Q;//	总无功功率	
        public float S;//	总视在功功率	
        public float COS;//	有功功率因数	
        public float SIN;//无功功率因数	
    }

    /// <summary>
    /// 读取的误差数据
    /// </summary>
    public struct stError
    {
        /// <summary>
        /// 误差值
        /// </summary>
        public string szError;

        /// <summary>
        /// 标识当前属于第几次误差
        /// </summary>
        public int Index;

        /// <summary>
        /// 表位号
        /// </summary>
        public int MeterIndex;

        /// <summary>
        /// 状态类型
        /// </summary>
        public int MeterConst;
        /// <summary>
        /// 电流回路状态,0x00表示第一个电流回路，0x01表示第二个电流回路
        /// </summary>
        public Cus_BothIRoadType iType;
        /// <summary>
        /// 电压回路状态,0x00表示直接接入式电表电压回路选择，0x01表示互感器接入式电表电压回路选择，0x02表示本表位无电表接入
        /// </summary>
        public Cus_BothVRoadType vType;
        /// <summary>
        /// 通讯口状态,0x00表示选择第一路普通485通讯；0x01表示选择第二路普通485通讯；0x02表示选择红外通讯；
        /// </summary>
        public int ConnType;

        /*
         * 状态类型分为四种：接线故障状态（Bit0）、预付费跳闸状态（Bit1）、报警信号状态（Bit2）、对标状态（Bit3）的参数
         * 分别由一个字节中的Bit0、Bit1、Bit2、Bit3标示，为1则表示该表位有故障/跳闸/报警/对标完成，为0则表示正常/正常/正常/未完成对标。
        */
        /// <summary>
        /// 接线故障状态,为true则表示该表位有故障,false为正常
        /// </summary>
        public bool statusTypeIsOnErr_Jxgz;

        /// <summary>
        /// 预付费跳闸状态,为true则表示该表位跳闸,false为正常
        /// </summary>
        public bool statusTypeIsOnErr_Yfftz;

        /// <summary>
        /// 报警信号状态,为true则表示该表位报警,false为正常
        /// </summary>
        public bool statusTypeIsOnErr_Bjxh;

        /// <summary>
        /// 对标状态,为true则表示该表位对标完成,false为未完成对标
        /// </summary>
        public bool statusTypeIsOnOver_Db;
        /// <summary>
        /// 温度过高故障状态（false：正常；true：故障）。温度过高时，会自动短接隔离继电器
        /// </summary>
        public bool statusTypeIsOnErr_Temp;
        /// <summary>
        /// 光电信号状态（false：未挂表；true：已挂表）
        /// </summary>
        public bool statusTypeIsOn_HaveMeter;
        /// <summary>
        /// 表位上限限位状态（false：未就位；true：就位）
        /// </summary>
        public bool statusTypeIsOn_PressUpLimit;
        /// <summary>
        /// 表位下限限位状态（false：未就位；true：就位）
        /// </summary>
        public bool statusTypeIsOn_PressDownLimt;
    }
   
    /// <summary>
    /// 电源参数
    /// </summary>
    /// 
    [Serializable()]
    public struct stPower
    {
        public Cus_Clfs Clfs;  //	 接线方式	
        public byte Flip_ABC;     //   	'相位开关控制	
        public float Freq;//	'频率	
        public byte Scale_Ua;// 	'Ua档位 	
        public byte Scale_Ub;// 	'Ub档位 	
        public byte Scale_Uc;// 	'Uc档位 	
        public byte Scale_Ia;// 	'Ia档位 	
        public byte Scale_Ib;// 	'Ib档位 	
        public byte Scale_Ic;// 	'Ic档位 	
        public float Ua;//	'UA 
        public float Ia;//	'Ia 	
        public float Ub;//	'UB  	
        public float Ib;// Ib 	
        public float Uc;// 	'UC 	
        public float Ic;// 'Ic 	
        public float Phi_Ua;// 	'Ua相位 	
        public float Phi_Ia;// 	'Ia相位 	
        public float Phi_Ub;//	'UB相位 	
        public float Phi_Ib;// 	'Ib相位 	
        public float Phi_Uc;// 	'UC相位 	
        public float Phi_Ic;// 	'Ic相位 	
        public float Pa;// 	'A相有功功率 	
        public float Pb;// 	'B相有功功率	
        public float Pc;// 	'C相有功功率	
        public float Qa;//	'A相无功功率	
        public float Qb;//	'B相无功功率	
        public float Qc;//	'C相无功功率	
        public float Sa;//	'A相视在功率	
        public float Sb;// 	'B相视在功率	
        public float Sc;// 	'C相视在功率	
        public float P;//	总有功功率	
        public float Q;//	总无功功率	
        public float S;//	总视在功功率	
        public float COS;//	有功功率因数	
        public float SIN;//无功功率因数	
    }

   
    /// <summary>
    /// 标准表常数查表
    /// 查表原则:
    /// 如果在非临界电压电流区域直接按常数表查询返回
    /// 如果在临界电压或临界电流区域，首先查询本次电压和上次电压是否相同，如果相同则返回上
    /// 次查询结果，如果不同则返回0，提示客户端需要发送指令给标准表读取标准表常数
    /// </summary>
    internal class StdMeterConst
    {
        /// <summary>
        /// 第一维为电流，第二维为电压
        /// </summary>
        /// 
        private static Dictionary<string, int> dicStdConstSheet = new Dictionary<string, int>();
        private static int[] arrU = new int[5] { 60, 100, 220, 380, 1000 };
        private static int[] arrI = new int[5] { 1, 5, 10, 50, 100 };
        public static float m_LastSearchU = 0F;        //上一次查询的电压
        public static float m_LastSearchI = 0F;        //上一次查询的电流
        public static int m_StdMeterConst = 0;         //上一次查询标准表常数
        static StdMeterConst()
        {
            int[] consts = new int[25]{
            (int)1.2*100000000,(int)2.4*10000000,(int)1.2*10000000,(int)2.4*1000000,(int)1.2*1000000,
            6*10000000,(int)1.2*10000000,6*1000000,(int)1.2*1000000,6*100000,
                3*10000000,6*1000000,3*1000000,6*100000,3*100000,
                (int)1.5*10000000,3*1000000,(int)1.5*1000000,3*100000,(int)1.5*100000,
                6*1000000,(int)1.2*1000000,6*100000,(int)1.2*100000,6*10000
            };
            string strKey = "";
            for (int i = 0; i < arrU.Length; i++)
            {
                for (int j = 0; j < arrI.Length; j++)
                {
                    strKey = string.Format("{0}{1}", arrU[i], arrI[j]);
                    dicStdConstSheet.Add(strKey, consts[i * 5 + j]);
                }
            }
        }
        /// <summary>
        /// 查表
        /// </summary>
        /// <param name="u"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static int SearchStdMeterConst(float sngU, float sngI)
        {
            bool bFound = false;
            int meterconst = 0;
            //查询是否是临界点
            if (IsCriticalValue(sngU, sngI))
            {
                //如果是临界点，则查询本次是否和上一次的电压电流一样
                if (sngU == m_LastSearchU && sngI == m_LastSearchI && m_StdMeterConst != 0)
                    meterconst = m_StdMeterConst;
                return meterconst;
            }
            for (int i = 0; i < arrU.Length; i++)
            {
                if (sngU < arrU[i] * 1.2F)
                {
                    for (int j = 0; j < arrI.Length; j++)
                    {
                        if (sngI < arrI[j] * 1.2F)
                        {
                            string strKey = string.Format("{0}{1}", arrU[i], arrI[j]);
                            if (dicStdConstSheet.ContainsKey(strKey))
                            {
                                meterconst = dicStdConstSheet[strKey];
                                bFound = true;
                                break;
                            }
                            else
                            {
                                throw new Exception("the key is not found");
                            }
                        }
                    }
                }
                if (bFound) break;
            }
            return meterconst;
        }

        public static bool IsCriticalValue(float sngU, float sngI)
        {
            //首先检测电压是否临界
            bool isCritical = false;
            float tmp = 0;
            for (int i = 0; i < arrU.Length; i++)
            {
                tmp = arrU[i] * 1.2F;
                if (sngU == tmp)
                {
                    isCritical = true;
                    break;
                }
            }
            if (isCritical) return true;
            //检测电流是否临界
            for (int i = 0; i < arrI.Length; i++)
            {
                tmp = arrI[i] * 1.2F;
                if (sngI * 1000000F == tmp * 1000000F)
                {
                    isCritical = true;
                    break;
                }
            }
            return isCritical;
        }
    }
}
