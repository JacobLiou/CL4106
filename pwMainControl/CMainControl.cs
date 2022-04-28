/****************************************************************************

    CMainControl主控板
    刘伟 2012-05

*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using pwInterface;
using System.Threading;
using System.Diagnostics;
using System.Collections;
namespace pwMainControl
{
    public class CMainControl : IMainControlProtocol
    {
        #region   单相后装智能台控制板通讯协议

        #endregion

        #region ----------变量声明----------
        protected string m_str_Setting = "9600,n,8,1";        //波特率
        protected ISerialport m_Ispt_com;                     //控制端口
        protected byte[] m_byt_RevData;                       //返回数据
        public string m_str_LostMessage = "";                 //操作失败信息
        public bool m_bln_Enabled = true;                     //
        protected string m_str_RxFrame = "";
        protected string m_str_TxFrame = "";
        protected int m_int_FECount = 1;
        private object m_obj_LockRev = new object();          //用于锁定互斥 
        private object m_obj_LockRev2 = new object();         //用于锁定互斥 


        private int m_int_WaitDataRevTime = 2000;           ///等待数据到达最大时间ms
        private int m_int_IntervalTime = 500;               ///数据间隔最大时间ms
        private byte m_byt_iRepeatTimes = 1;                ///通讯失败重试次数
        private bool m_bol_ClosComm = false;                ///通讯完成后是否关闭端口
        private bool m_bol_BreakDown = false;               ///被外部中断，主要用于下载打包参数时
        private string m_str_Address = "AAAAAAAAAAAA";      ///表地址

        #endregion


        #region ----------公共事件----------
        public event Dge_EventRxFrame OnEventRxFrame;
        public event Dge_EventTxFrame OnEventTxFrame;

        public event DelegateEventMainControl OnEventMainControl;
        #endregion


        #region----------公共属性----------
        /// <summary>
        /// 通信串口
        /// </summary>
        public ISerialport ComPort
        {
            get
            {
                return this.m_Ispt_com;
            }
            set
            {
                if (!value.Equals(this.m_Ispt_com))
                {
                    if (this.m_Ispt_com != null)
                    {
                        this.m_Ispt_com.DataReceive -= new RevEventDelegete(m_Ispt_com_DataReceive);
                    }
                    this.m_Ispt_com = value;
                    this.m_Ispt_com.DataReceive += new RevEventDelegete(m_Ispt_com_DataReceive);
                }
            }
        }


        /// <summary>
        /// 波特率
        /// </summary>
        public string Setting
        {
            get
            {
                return this.m_str_Setting;
            }
            set
            {
                this.m_str_Setting = value;
            }
        }

        /// <summary>
        /// 返回帧
        /// </summary>
        public string RxFrame
        {
            get { return this.m_str_RxFrame; }
        }

        /// <summary>
        /// 下发帧
        /// </summary>
        public string TxFrame
        {
            get { return this.m_str_TxFrame; }
        }


        /// <summary>
        /// 下发帧的唤醒符个数
        /// </summary>
        public int FECount
        {
            get { return this.m_int_FECount; }
            set { this.m_int_FECount = value; }
        }

        /// <summary>
        /// 等待数据到达最大时间
        /// </summary>
        public int WaitDataRevTime
        {
            get
            {
                return this.m_int_WaitDataRevTime;
            }
            set
            {
                this.m_int_WaitDataRevTime = value;
            }
        }

        /// <summary>
        /// 数据间隔最大时间
        /// </summary>
        public int IntervalTime
        {
            get
            {
                return this.m_int_IntervalTime;
            }
            set
            {
                this.m_int_IntervalTime = value;
            }
        }

        /// <summary>
        /// 操作失败信息
        /// </summary>
        public string LostMessage
        {
            get
            {
                return this.m_str_LostMessage;
            }
        }


        /// <summary>
        /// 通讯失败重试次数
        /// </summary>
        public byte iRepeatTimes
        {
            set
            {
                this.m_byt_iRepeatTimes = value;
            }
        }


        /// <summary>
        /// 通讯完成后是否关闭端口
        /// </summary>
        public bool bClosComm
        {
            set
            {
                this.m_bol_ClosComm = value;
            }
        }

        /// <summary>
        /// 下载打包参数时是否被外部中断
        /// </summary>
        public bool BreakDown
        {
            set
            {
                this.m_bol_BreakDown = value;
            }
        }


        #endregion


        #region----------公有方法----------

        /// <summary>
        /// 读取数据（字符型，数据项）
        /// </summary>
        /// <param name="str_OBIS">标识符,2个字节</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="str_Value">返回数据</param>
        /// <returns></returns>
        public bool ReadData(string str_OBIS, int int_Len, ref string str_Value)
        {
            try
            {
                string cDataStr = "";
                int iLen = 0;
                cDataStr = str_Value + str_OBIS;
                iLen = cDataStr.Length / 2;
                bool bT = SendDLT645Command(0x01, this.m_str_Address, (byte)iLen, ref cDataStr);
                if (!bT)
                {
                    str_Value = "";
                    return false;
                }
                else
                {
                    cDataStr = cDataStr.Substring(4);
                    str_Value = cDataStr;
                    return bT;
                }
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }

        /// <summary>
        /// 写数据(字符型，数据项)
        /// </summary>
        /// <param name="str_OBIS">标识符,两个字节</param>
        /// <param name="int_Len">数据长度(块中每项字节数)</param>
        /// <param name="str_Value">写入数据</param>
        /// <returns></returns>
        public bool WriteData(string str_OBIS, int int_Len, string str_Value)
        {
            string cData = BackString(str_OBIS);
            str_Value = BackString(str_Value.PadLeft(2 * int_Len, '0'));

            cData += str_Value;

            cData = BackString(cData);

            bool bT = SendDLT645Command(0x4, m_str_Address, (byte)(int_Len + 2), ref cData);

            if (!bT)
            {
                str_Value = "";
            }
            else
            {
                str_Value = cData.Substring(4);
            }
            return bT;

        }


        /// <summary>
        /// 命令链路
        /// </summary>
        /// <param name="bCmd">控制码</param>
        /// <param name="cAddr">表地址</param>
        /// <param name="bDataLen">数据长度</param>
        /// <param name="cData">数据域数据</param>
        /// <param name="bExtend">是否有后续数据</param>
        /// <returns></returns>
        public bool SendDLT645Command(byte bCmd, string cAddr, byte bDataLen, ref string cData)
        {	/// cCmd 命令字  cAddr 地址 bLen 数据长度 cData 数据域数据 bExtend 是否有后续数据
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                Stopwatch sth_SpaceTicker = new Stopwatch();                //
                sth_SpaceTicker.Start();

                byte[] byt_SendData = this.OrgFrame(bCmd, cAddr, bDataLen, cData);


                if (byt_SendData.Length > 0)
                {
                    byte bRepeat = 0;
                    bool bln_Result = false;
                    while (bRepeat < m_byt_iRepeatTimes)
                    {
                        cData = "";
                        bln_Result = this.SendFrame(byt_SendData, m_int_WaitDataRevTime, m_int_IntervalTime);
                        if (!bln_Result) break;

                        bln_Result = this.CheckFrame(this.m_byt_RevData, ref cData);
                        if (bln_Result)
                        {
                            break;
                        }


                        Thread.Sleep(1);
                        bRepeat++;
                        if (m_bol_BreakDown)
                            return false;

                    }
                    this.m_str_LostMessage = @"sum time :" + sth_SpaceTicker.ElapsedMilliseconds.ToString();
                    return bln_Result;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
            finally
            {
                this.m_byt_RevData = new byte[0];
            }


        }
        #endregion

        #region ---------后装应用方法----------
        /// <summary>
        /// 升源(F930,F950)
        /// </summary>
        /// <param name="Typ_Cmd">Typ_Cmd为00=表示220V给A组电表供电上电，01=表示154V给A组电表供电上电，02=表示220V给B组电表供电上电，03=表示154V给B组电表供电上电</param>
        /// <returns></returns>
        public bool SetPowerOn(int Typ_Cmd)
        {   //其中XX为00时表示220V给A组电表供电，
            //      为01时表示154V给A组电表供电，
            //      为02时表示220V给B组电表供电，
            //      为03时表示154V给B组电表供电

            try
            {
                int iLen = 3;
                string[] cDataStrF930 = { "00F930", "01F930", "02F930", "03F930"};
                string[] cDataStrF950 = { "01F950", "01F950", "03F950", "03F950" };

                string cDataStr = cDataStrF930[Typ_Cmd]; 
                bool bT = SendDLT645Command(0x81, this.m_str_Address, (byte)iLen, ref cDataStr);
                if (bT)
                {
                    cDataStr = cDataStrF950[Typ_Cmd]; 
                    bT = SendDLT645Command(0x81, this.m_str_Address, (byte)iLen, ref cDataStr);

                }
                return bT;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }

        /// <summary>
        /// 继电器切换电源给电表供电(F930)
        /// </summary>
        /// <param name="Typ_Cmd">Typ_Cmd为00=表示220V给A组电表供电，01=表示154V给A组电表供电，02=表示220V给B组电表供电，03=表示154V给B组电表供电</param>
        /// <returns></returns>
        public bool SetJDQChange(int Typ_Cmd)
        {   //其中XX为00时表示220V给A组电表供电，
            //      为01时表示154V给A组电表供电，
            //      为02时表示220V给B组电表供电，
            //      为03时表示154V给B组电表供电

            try
            {
                string str_OBIS = "F930";
                string str_Value = Typ_Cmd.ToString("X2");
                int iLen = 0;
                string cDataStr = str_Value + str_OBIS;
                iLen = cDataStr.Length / 2;
                bool bT = SendDLT645Command(0x81, this.m_str_Address, (byte)iLen, ref cDataStr);
                return bT;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }

        /// <summary>
        /// 给表位上电断电(F950)
        /// </summary>
        /// <param name="Typ_Cmd">Typ_Cmd为00=表示给A组表位断电，01=表示给A组表位上电，02=表示给B组表位断电，03=表示给B组表位上电</param>
        /// <returns></returns>
        public bool SetPowerOnOff(int Typ_Cmd)
        {   //其中XX为00时表示给A组表位断电，
            //      为01时表示给A组表位上电，
            //      为02时表示给B组表位断电，
            //      为03时表示给B组表位上电
            try
            {
                string str_OBIS = "F950";
                string str_Value = Typ_Cmd.ToString("X2");
                int iLen = 0;
                string cDataStr = str_Value + str_OBIS;
                iLen = cDataStr.Length / 2;
                bool bT = SendDLT645Command(0x81, this.m_str_Address, (byte)iLen, ref cDataStr);
                return bT;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }


        #endregion

        #region ---------前装应用方法----------

        /// <summary>
        /// 启动、复位停止(F980)
        /// </summary>
        /// <param name="Typ_Cmd">Typ_Cmd为00=表示A组复位停止，01=表示A组启动，02=表示B组复位停止，03=表示B组启动</param>
        /// <returns></returns>
        public bool SetStartStopCmd(int Typ_Cmd)
        {
            try
            {
                string str_OBIS = "F980";
                string str_Value = Typ_Cmd.ToString("X2");
                int iLen = 0;
                string cDataStr = str_Value + str_OBIS;
                iLen = cDataStr.Length / 2;
                bool bT = SendDLT645Command(0x81, this.m_str_Address, (byte)iLen, ref cDataStr);
                return bT;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }

        /// <summary>
        /// 气缸控制 --上、下(F913)  SunBoy Add
        /// </summary>
        /// <param name="Typ_Cmd">Typ_Cmd为00=表示A组气缸下降，01=表示A组气缸上升，04=表示A组气缸停止，02=表示B组气缸下降，03=表示B组气缸上升 05=表示B组气缸停止，</param>
        /// <returns></returns>
        public bool SetStartStopQG(int Typ_Cmd)
        {
            try
            {
                string str_OBIS = "F913";
                string str_Value = Typ_Cmd.ToString("X2");
                int iLen = 0;
                string cDataStr = str_Value + str_OBIS;
                iLen = cDataStr.Length / 2;
                bool bT = SendDLT645Command(0x04, this.m_str_Address, (byte)iLen, ref cDataStr);
                return bT;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }

        /// <summary>
        /// 设置测试状态(F90F)
        /// </summary>
        /// <param name="Typ_Cmd">XX为00时表示A组测试完毕，为01时表示A组测试中，为02时表示B组测试完毕，为03时表示B组测试中</param>
        /// <returns></returns>
        public bool SetTestStatusCmd(int Typ_Cmd)
        {
            try
            {
                string str_OBIS = "F90F";
                string str_Value = Typ_Cmd.ToString("X2");
                int iLen = 0;
                string cDataStr = str_Value + str_OBIS;
                iLen = cDataStr.Length / 2;
                bool bT = SendDLT645Command(0x04, this.m_str_Address, (byte)iLen, ref cDataStr);
                return bT;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }


        /// <summary>
        /// 选择测试继电器(F907)
        /// </summary>
        /// <param name="Typ_Cmd">Typ_Cmd为00时表示选择测试内置继电器，为01时表示选择测试外置继电器，为02时表示外置隔离继电器</param>
        /// <returns></returns>
        public bool SelectJDQ(int Typ_Cmd)
        {
            try
            {
                string str_OBIS = "F907";
                string str_Value = Typ_Cmd.ToString("X2");
                int iLen = 0;
                string cDataStr = str_Value + str_OBIS;
                iLen = cDataStr.Length / 2;
                bool bT = SendDLT645Command(0x04, this.m_str_Address, (byte)iLen, ref cDataStr);
                this.m_bln_Enabled = bT;
                return bT;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }

        /// <summary>
        /// A组电流校淮切换(F931)
        /// </summary>
        /// <param name="Typ_Cmd">Typ_Cmd为为00=表示大电流校准，01=表示小电流校准</param>
        /// <returns></returns>
        public bool AdjustCmdA(int Typ_Cmd)
        {
            try
            {
                string str_OBIS = "F931";
                string str_Value = Typ_Cmd.ToString("X2");
                int iLen = 0;
                string cDataStr = str_Value + str_OBIS;
                iLen = cDataStr.Length / 2;
                bool bT = SendDLT645Command(0x04, this.m_str_Address, (byte)iLen, ref cDataStr);
                return bT;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }

        /// <summary>
        /// B组电流校淮切换(F931)
        /// </summary>
        /// <param name="Typ_Cmd">Typ_Cmd为为00=表示大电流校准，01=表示小电流校准</param>
        /// <returns></returns>
        public bool AdjustCmdB(int Typ_Cmd)
        {
            try
            {
                string str_OBIS = "F932";
                string str_Value = Typ_Cmd.ToString("X2");
                int iLen = 0;
                string cDataStr = str_Value + str_OBIS;
                iLen = cDataStr.Length / 2;
                bool bT = SendDLT645Command(0x04, this.m_str_Address, (byte)iLen, ref cDataStr);
                return bT;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }

        
        /// <summary>
        /// 读A组继电器状态(F908)
        /// </summary>
        /// <param name="str_Valu">返回值XX表示读到的A组继电器状态，01表示1#表位拉闸故障，02表示2#表位拉闸故障，04表示3#表位拉闸故障， 03表示1#2#表位拉闸故障，05表示1#3#表位拉闸故障，06表示2#3#表位拉闸故障，07表示1#2#3#表位拉闸故障，00表示无故障</param>
        /// <returns></returns>
        public bool ReadJDQA(ref string str_Value)
        {
            try
            {
                string str_OBIS = "F908";
                int iLen = 0;
                string cDataStr =str_OBIS;
                iLen = cDataStr.Length / 2;
                bool bT = SendDLT645Command(0x01, this.m_str_Address, (byte)iLen, ref cDataStr);
                if (!bT)
                {
                    str_Value = "";
                    return false;
                }
                else
                {
                    cDataStr = cDataStr.Substring(4);
                    str_Value = cDataStr;
                    return bT;
                }
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }

        /// <summary>
        /// 读B组继电器状态(F909)
        /// </summary>
        /// <param name="str_Valu">返回值XX表示读到的B组继电器状态，08表示4#表位拉闸故障，10表示5#表位拉闸故障，20表示6#表位拉闸故障，18表示4#、5#表位拉闸故障，28表示4#、6#表位拉闸故障，30表示5#、6#表位拉闸故障，38表示4#、5#、6#表位拉闸故障，00表示无故障</param>
        /// <returns></returns>
        public bool ReadJDQB(ref string str_Value)
        {
            try
            {
                string str_OBIS = "F909";
                int iLen = 0;
                string cDataStr = str_OBIS;
                iLen = cDataStr.Length / 2;
                bool bT = SendDLT645Command(0x01, this.m_str_Address, (byte)iLen, ref cDataStr);
                if (!bT)
                {
                    str_Value = "";
                    return false;
                }
                else
                {
                    cDataStr = cDataStr.Substring(4);
                    str_Value = cDataStr;
                    return bT;
                }
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }


        /// <summary>
        /// 设置A组表位故障灯(F90D)
        /// </summary>
        /// <param name="Typ_Cmd">XX表示A组故障灯状态，01表示1#表位故障灯亮，02表示2#表位故障灯亮，04表示3#表位故障灯亮， 03表示1#2#表位故障灯亮，05表示1#3#表位故障灯亮，06表示2#3#表位故障灯亮，07表示1#2#3#表位故障灯亮，00表示无故障</param>
        /// <returns></returns>
        public bool SetGzdengA(int Typ_Cmd)
        {
            try
            {
                string str_OBIS = "F90D";
                string str_Value = Typ_Cmd.ToString("X2");
                int iLen = 0;
                string cDataStr = str_Value + str_OBIS;
                iLen = cDataStr.Length / 2;
                bool bT = SendDLT645Command(0x04, this.m_str_Address, (byte)iLen, ref cDataStr);
                return bT;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }

        /// <summary>
        /// 设置B组表位故障灯(F90E)
        /// </summary>
        /// <param name="Typ_Cmd">XX表示B组故障灯状态，08表示4#表位故障灯亮，10表示5#表位故障灯亮，20表示6#表位故障灯亮，18表示4#、5#表位故障灯亮，28表示4#、6#表位故障灯亮，30表示5#、6#表位故障灯亮，38表示4#、5#、6#表位故障灯亮，00表示无故障</param>
        /// <returns></returns>
        public bool SetGzdengB(int Typ_Cmd)
        {
            try
            {
                string str_OBIS = "F90E";
                string str_Value = Typ_Cmd.ToString("X2");
                int iLen = 0;
                string cDataStr = str_Value + str_OBIS;
                iLen = cDataStr.Length / 2;
                bool bT = SendDLT645Command(0x04, this.m_str_Address, (byte)iLen, ref cDataStr);
                return bT;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }


        /// <summary>
        /// 整机自检开始命令
        /// </summary>
        /// <param name="Typ_Cmd">0为A组，1为B组</param>
        /// <returns></returns>
        public bool SetSelfCheckStart(int Typ_Cmd)
        {
            try
            {
                string str_OBIS = "F910";
                string str_Value = Typ_Cmd.ToString("X2");
                int iLen = 0;
                string cDataStr = str_Value + str_OBIS;
                iLen = cDataStr.Length / 2;
                bool bT = SendDLT645Command(0x04, this.m_str_Address, (byte)iLen, ref cDataStr);
                return bT;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }

        /// <summary>
        /// 整机自检结束命令
        /// </summary>
        /// <param name="Typ_Cmd">0为A组，1为B组</param>
        /// <returns></returns>
        public bool SetSelfCheckEnd(int Typ_Cmd, ref string cDataStr)
        {
            try
            {
                string str_OBIS = "F911";
                string str_Value = Typ_Cmd.ToString("X2");
                int iLen = 0;
                cDataStr = str_Value + str_OBIS;
                iLen = cDataStr.Length / 2;
                bool bT = SendDLT645Command(0x04, this.m_str_Address, (byte)iLen, ref cDataStr);
                return bT;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }

        #endregion


        #region----------私有方法----------

        #region 数据发送

        /// <summary>
        /// 发送指帧
        /// </summary>
        /// <param name="byt_Frame">发送数据</param>
        /// <param name="int_MinSecond">等待返回时间</param>
        /// <param name="int_SpaceMSecond">等待返回帧字节间隔时间</param>
        /// <returns></returns>
        protected bool SendFrame(byte[] byt_Frame, int int_MinSecond, int int_SpaceMSecond)
        {
            try
            {
                this.m_byt_RevData = new byte[0];
                DisposeTxEvent DspTxFrame = new DisposeTxEvent(AcyDspTxFrame);
                DspTxFrame(m_Ispt_com.ComPort, BitConverter.ToString(byt_Frame));


                this.m_Ispt_com.SendData(byt_Frame);
                Waiting(int_MinSecond, int_SpaceMSecond);
                DisposeRxEvent DspRxFrame = new DisposeRxEvent(AcyDspRxFrame);
                DspRxFrame(m_Ispt_com.ComPort, BitConverter.ToString(m_byt_RevData));
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 等待数据返回
        /// </summary>
        /// <param name="int_MinSecond">等待返回时间</param>
        /// <param name="int_SpaceMSecond">等待返回帧字节间隔时间</param>
        private void Waiting(int int_MinSecond, int int_SpaceMSecond)
        {
            try
            {
                int int_OldLen = 0;
                Stopwatch sth_Ticker = new Stopwatch();                     //等待计时，
                Stopwatch sth_SpaceTicker = new Stopwatch();                //
                sth_SpaceTicker.Start();
                sth_Ticker.Start();
                while (this.m_bln_Enabled)
                {
                    if (this.m_byt_RevData.Length > int_OldLen)     //长度有改变
                    {
                        sth_SpaceTicker.Reset();
                        int_OldLen = this.m_byt_RevData.Length;
                        sth_SpaceTicker.Start();                    //字节间计时重新开始
                    }
                    else        //如果长度有没有增加，与前次收到数据时间隔500毫秒则退出
                    {
                        if (this.m_byt_RevData.Length > 0)      //已经收到一部分，则按字节间计时
                        {
                            if (sth_SpaceTicker.ElapsedMilliseconds >= int_SpaceMSecond)
                                break;
                        }
                    }
                    if (sth_Ticker.ElapsedMilliseconds >= int_MinSecond)        //总计时
                        break;
                    System.Threading.Thread.Sleep(1);
                }
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.Message;
            }
        }

        private void AcyDspRxFrame(int intCom, string str_Frame)
        {
            this.m_str_RxFrame = str_Frame;
            if (this.OnEventRxFrame != null) this.OnEventRxFrame(intCom, str_Frame);
        }

        private void AcyDspTxFrame(int intCom, string str_Frame)
        {
            this.m_str_TxFrame = str_Frame;
            if (this.OnEventTxFrame != null) this.OnEventTxFrame(intCom, str_Frame);
        }

        #endregion


        /// <summary>
        /// 数据接收处理
        /// </summary>
        /// <param name="bData">接收到的数据</param>
        private void m_Ispt_com_DataReceive(byte[] bData)
        {
            lock (m_obj_LockRev)
            {
                int int_Len = bData.Length;
                int int_OldLen = this.m_byt_RevData.Length;
                Array.Resize(ref this.m_byt_RevData, int_Len + int_OldLen);
                Array.Copy(bData, 0, this.m_byt_RevData, int_OldLen, int_Len);

                if (this.m_byt_RevData.Length >= 17)
                {
                    Thread ThreadMonitor = new Thread(Monitor);
                    ThreadMonitor.Start();
                }

                
            }
        }

        private  void Monitor()
        {
            ///需要处理上行下行同时发生情况
            ///FE FE 68 AAAAAAAAAAAA 68 81 03 80 F9 XX YY 16
            this.m_str_LostMessage = "";
            try
            {
                int int_Len = 0;

                if (CheckFrame(this.m_byt_RevData, ref int_Len))
                {
                    Array.Resize(ref this.m_byt_RevData, 0);
                }
                else
                {

                    Array.Resize(ref this.m_byt_RevData, 0);

                    //处理乱码
                    //byte[] _byt_RevData = new byte[0];

                    //Array.Resize(ref _byt_RevData, this.m_byt_RevData.Length - int_Len);
                    //Array.Copy(this.m_byt_RevData, int_Len, _byt_RevData, 0, this.m_byt_RevData.Length - int_Len);

                    //Array.Resize(ref this.m_byt_RevData, _byt_RevData.Length);
                    //Array.Copy(_byt_RevData, 0, this.m_byt_RevData, 0, _byt_RevData.Length);


                }

            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
            }
            finally
            {
            }
        }

        /// <summary>
        /// 验证帧是否符合要求，是否为主动上报帧
        /// </summary>
        /// <param name="byt_Value">需验证帧内容</param>
        /// <param name="byt_Cmd">返回帧控制码</param>
        /// <param name="byt_Data">返回帧数据域内容</param>
        /// <returns></returns>
        private bool CheckFrame(byte[] byt_RevFrame, ref int int_CurZenLen)
        {
            try
            {

                if (byt_RevFrame.Length <= 0)
                {
                    this.m_str_LostMessage = "没有返回数据！";
                    return false;
                }
                int_CurZenLen = 0;
                int int_Start = 0;
                int_Start = Array.IndexOf(byt_RevFrame, (byte)0x68);
                if (int_Start < 0 || int_Start > byt_RevFrame.Length || int_Start + 12 > byt_RevFrame.Length) //没有68开头 长度是否足够一帧 是否完整
                {
                    int_CurZenLen = int_Start;
                    this.m_str_LostMessage = "返回帧不完整！没有帧头。[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }

                if (byt_RevFrame[int_Start + 7] != 0x68)        //找不到第二个68
                {
                    int_CurZenLen = int_Start+7;
                    this.m_str_LostMessage = "返回帧不完整！[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }

                int int_Len = byt_RevFrame[int_Start + 9];
                if (int_Start + 12 + int_Len > byt_RevFrame.Length)
                {
                    this.m_str_LostMessage = "数据长度与实际长度不一致！[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;                //帧的长度是否与实际长度一样
                }
                if (byt_RevFrame[int_Start + int_Len + 11] != 0x16)       //没有16结束
                {
                    this.m_str_LostMessage = "返回帧不完整！[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }
                int_CurZenLen = int_Start + int_Len + 12;

                if (byt_RevFrame[int_Start + 8] == 0x81
                    && byt_RevFrame[int_Start + 9] == 0x03
                    && byt_RevFrame[int_Start + 10] == 0xB3//F980
                    && byt_RevFrame[int_Start + 11] == 0x2C)//表示主控板启动、复位停止帧
                {
                    #region 主控事件触发
                    int Typ_Cmd = Convert.ToInt32(byt_RevFrame[int_Start + 12]-0x33);
                    if (OnEventMainControl != null) OnEventMainControl(Typ_Cmd);
                    #endregion 

                    #region 主控上报数据帧
                    byte[] _byt_RevData = new byte[0];

                    Array.Resize(ref _byt_RevData, int_CurZenLen);
                    Array.Copy(this.m_byt_RevData, 0, _byt_RevData, 0, int_CurZenLen);

                    DisposeRxEvent DspRxFrame = new DisposeRxEvent(AcyDspRxFrame);
                    DspRxFrame(m_Ispt_com.ComPort, BitConverter.ToString(_byt_RevData));
                    #endregion

                    this.m_str_LostMessage = "主控板启动、复位停止帧！[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return true ;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                int_CurZenLen = 0;
                return false;
            }
        }



        /// <summary>
        /// 发送接收函数
        /// </summary>
        /// <param name="byt_SendData">发送数据包</param>
        /// <returns>接收数据包</returns>
        private byte[] Message(byte[] byt_SendData)
        {	/// sSendString 发送的数据   返回接收的数据


            //清空
            this.m_str_TxFrame = "";
            this.m_str_RxFrame = "";
            this.m_byt_RevData = new byte[0];
            //发送帧
            this.SendFrame(byt_SendData, m_int_WaitDataRevTime, m_int_IntervalTime);
            //返回帧
            return m_byt_RevData;

        }

        /// <summary>
        /// 组织帧
        /// </summary>
        /// <param name="cCmd">控制码</param>
        /// <param name="cAddr">表地址</param>
        /// <param name="bLen">长度</param>
        /// <param name="cData">数据</param>
        /// <returns>返回组好的帧</returns>
        /// 
        private byte[] OrgFrame(byte cCmd, string cAddr, byte bLen, string cData)
        {	/// cCmd 命令字  cAddr 地址 bLen 数据长度 cData 数据域数据 返回组好的帧
            byte[] bSend;
            int iTn;
            string cStr = "";

            bSend = new byte[12 + bLen];

            bSend[0] = 0x68;
            /// 地址域
            cAddr = cAddr.PadLeft(12, '0');
            for (iTn = 0; iTn <= 5; iTn++)
            {
                cStr = "0x" + cAddr.Substring(2 * (5 - iTn), 2);
                bSend[iTn + 1] = System.Convert.ToByte(cStr, 16);
            }
            bSend[7] = 0x68;
            bSend[8] = cCmd; //System.Convert.ToByte("0x" + cCmd, 16);
            bSend[9] = bLen;
            /// 数据域
            cData = cData.PadLeft(2 * bLen, '0');
            if (bLen > 0)
            {
                for (iTn = 1; iTn <= bLen; iTn++)
                {
                    cStr = cData.Substring(2 * (bLen - iTn), 2);
                    bSend[9 + iTn] = System.Convert.ToByte("0x" + cStr, 16);
                    //bSend[9 + iTn] += 0x33;

                }
            }
            /// 校验码
            for (iTn = 0; iTn <= 9 + bLen; iTn++)
            {
                bSend[10 + bLen] += bSend[iTn];
            }

            /// 结束码
            bSend[11 + bLen] = 0x16;
            return bSend;
        }

        /// <summary>
        /// 检查返回帧合法性
        /// </summary>
        /// <param name="bWrap">返回帧</param>
        /// <param name="cData">返回数据</param>
        /// <param name="bExtend">是否有后续帧</param>
        /// <returns></returns>
        private bool CheckFrame(byte[] byt_RevFrame, ref string cData)
        {/// 解析数据  cWrap 需要解析的包 cData 返回的数据	bExtend 是否有后续数据
            try
            {

                if (byt_RevFrame.Length <= 0)
                {
                    this.m_str_LostMessage = "没有返回数据！";
                    return false;
                }
                int int_Start = 0;
                int_Start = Array.IndexOf(byt_RevFrame, (byte)0x68);
                if (int_Start < 0 || int_Start > byt_RevFrame.Length || int_Start + 12 > byt_RevFrame.Length) //没有68开头 长度是否足够一帧 是否完整
                {
                    this.m_str_LostMessage = "返回帧不完整！没有帧头。[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }

                if (byt_RevFrame[int_Start + 7] != 0x68)        //找不到第二个68
                {
                    this.m_str_LostMessage = "返回帧不完整！[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }

                int int_Len = byt_RevFrame[int_Start + 9];
                if (int_Start + 12 + int_Len > byt_RevFrame.Length)
                {
                    this.m_str_LostMessage = "数据长度与实际长度不一致！[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;                //帧的长度是否与实际长度一样
                }
                //byte byt_Chksum = 0;
                //for (int int_Inc = int_Start; int_Inc < int_Start + int_Len + 10; int_Inc++)
                //    byt_Chksum += byt_RevFrame[int_Inc];
                //if (byt_RevFrame[int_Start + int_Len + 10] != byt_Chksum)       //校验码不正确
                //{
                //    this.m_str_LostMessage = "返回校验码不正确！[" + BitConverter.ToString(byt_RevFrame) + "]";
                //    return false;
                //}
                if (byt_RevFrame[int_Start + int_Len + 11] != 0x16)       //没有16结束
                {
                    this.m_str_LostMessage = "返回帧不完整！[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }

                ////是否返回操作成功     第7Bit是1则是返回，第6bit是0=成功，1=失败
                //if ((byt_RevFrame[int_Start + 8] & 0xC0) == 0xC0)
                //{
                //    this.m_str_LostMessage = "返回操作失败帧！[" + BitConverter.ToString(byt_RevFrame) + "]";
                //    return false;
                //}


                //Array.Resize(ref byt_Addr, 6);
                //Array.Copy(byt_RevFrame, int_Start + 1, byt_Addr, 0, 6);
                //cmd
                byte[] byt_RevData = new byte[0];
                Array.Resize(ref byt_RevData, int_Len);    //数据域长度
                if (int_Len > 0)
                {
                    Array.Copy(byt_RevFrame, int_Start + 10, byt_RevData, 0, int_Len);
                    //for (int int_Inc = 0; int_Inc < byt_RevData.Length; int_Inc++)
                    //    byt_RevData[int_Inc] -= 0x33;

                    cData = BytesToString(byt_RevData);
                }

                return true;

            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 字节串转字符串
        /// </summary>
        /// <param name="byt_Values"></param>
        /// <returns></returns>
        private string BytesToString(byte[] byt_Values)
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (byte bt in byt_Values)
            {
                strBuilder.AppendFormat("{0:X2}", bt);
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// 反转字节字符串
        /// </summary>
        /// <param name="sData"></param>
        /// <returns></returns>
        public string BackString(string sData)
        {		//字符重新排序
            int ilen = sData.Length;
            string stemp = "";
            if (ilen <= 0) return "";
            if (ilen % 2 != 0) return "";
            for (int tn = 0; tn < ilen / 2; tn++)
            {
                stemp = sData.Substring(2 * tn, 2) + stemp;
            }
            return stemp;
        }

        #endregion
    }


}
