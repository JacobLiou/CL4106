
/****************************************************************************

    ProtocolBase协议基类
    刘伟 2009-10

*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using pwInterface;
using System.Threading;


namespace pwMeterProtocol
{
    /// <summary>
    /// 功能描述：协议积累
    /// 作    者：
    /// 编写日期：
    /// 修改记录：
    ///         修改日期		     修改人	            修改内容
    ///
    /// </summary>
    public class ProtocolBase
    {
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
        /// <summary>
        /// 等待时间控制倍率.默认值为1F
        /// </summary>
        private float SendFrameWaitTime =1f;
        private bool m_bol_ClouHw = false;

        #endregion

        #region ----------公共事件----------
        public event Dge_EventRxFrame OnEventRxFrame;
        public event Dge_EventTxFrame OnEventTxFrame;

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

        public bool ClouHw
        {
            set { this.m_bol_ClouHw = value; }
        }
        #endregion


        #region----------虚方法----------

        /// <summary>
        /// 通讯协议类名
        /// </summary>
        public virtual string GetClassName()
        {
            return null;
        }

        #endregion----------------------------

        #region----------发送指令----------
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
                DspTxFrame(m_Ispt_com.ComPort ,BitConverter.ToString(byt_Frame));
                
                Thread.Sleep(2000);
                this.m_Ispt_com.SendData(byt_Frame);
                //Thread.Sleep(10000);//A到B 不稳定 
                if (m_bol_ClouHw)
                {
                    Waiting(int_MinSecond, int_SpaceMSecond, byt_Frame);
                }
                else
                {
                    Waiting(int_MinSecond, int_SpaceMSecond);
                }
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

        #endregion


        #region----------上/下行数据记录----------
        private void AcyDspRxFrame(int intCom, string str_Frame)
        {
            this.m_str_RxFrame = str_Frame;
            if (this.OnEventRxFrame != null) this.OnEventRxFrame(intCom,str_Frame);
        }

        private void AcyDspTxFrame(int intCom,string str_Frame)
        {
            this.m_str_TxFrame = str_Frame;
            if (this.OnEventTxFrame != null) this.OnEventTxFrame(intCom,str_Frame);
        }
        #endregion

        #region----------发送等待----------
        /// <summary>
        /// 等待数据返回
        /// </summary>
        /// <param name="int_MinSecond">等待返回时间</param>
        /// <param name="int_SpaceMSecond">等待返回帧字节间隔时间</param>
        private void Waiting(int int_MinSecond, int int_SpaceMSecond)
        {
            try
            {
                int_MinSecond = (int)(int_MinSecond * SendFrameWaitTime);
                int_SpaceMSecond = (int)(int_SpaceMSecond * SendFrameWaitTime);

                int int_OldLen = 0;
                Stopwatch sth_Ticker = new Stopwatch();                     //等待计时，
                Stopwatch sth_SpaceTicker = new Stopwatch();                //
                sth_SpaceTicker.Start();
                sth_Ticker.Start();
                while (this.m_bln_Enabled)
                {
                    System.Windows.Forms.Application.DoEvents();
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
        #endregion

        #region----------发送等待(处理CLOU红外反射)----------
        /// <summary>
        /// 科陆红外头等待数据返回，去掉反射数据
        /// </summary>
        /// <param name="int_MinSecond">等待返回时间</param>
        /// <param name="int_SpaceMSecond">等待返回帧字节间隔时间</param>
        /// <param name="byt_Data_OldSend">发送帧</param>
        private  void Waiting(int int_MinSecond, int int_SpaceMSecond, byte[] byt_Data_OldSend)
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
                    System.Windows.Forms.Application.DoEvents();
                    System.Threading.Thread.Sleep(5);
                    if (sth_Ticker.ElapsedMilliseconds >= int_MinSecond)        //总计时
                        break;

                    #region 接收
                    if (this.m_byt_RevData.Length > int_OldLen)     //长度有改变
                    {
                        #region 处理CLOU红外反射问题
                        int int_Len_old = byt_Data_OldSend.Length;
                        if (this.m_byt_RevData.Length >= int_Len_old + 12)
                        {
                            byte[] byt_RevData1 = new byte[int_Len_old];
                            Array.Copy(this.m_byt_RevData, 0, byt_RevData1, 0, int_Len_old);
                            if (CheckArray(byt_RevData1, byt_Data_OldSend))
                            {
                                byte[] byt_RevData2 = new byte[m_byt_RevData.Length - int_Len_old];
                                Array.Copy(this.m_byt_RevData, int_Len_old, byt_RevData2, 0, this.m_byt_RevData.Length - int_Len_old);
                                m_byt_RevData = byt_RevData2;

                            }
                        }
                        #endregion

                        if (CheckFrame(m_byt_RevData))
                            break;

                        sth_SpaceTicker.Reset();
                        int_OldLen = this.m_byt_RevData.Length;
                        sth_SpaceTicker.Start();                    //字节间计时重新开始
                    }
                    else        //如果长度有没有增加，与前次收到数据时间隔500毫秒则退出
                    {
                        if (this.m_byt_RevData.Length > 0)      //已经收到一部分，则按字节间计时
                        {
                            #region 处理CLOU红外反射问题
                            int int_Len_old = byt_Data_OldSend.Length;
                            if (this.m_byt_RevData.Length >= int_Len_old + 12)
                            {
                                byte[] byt_RevData1 = new byte[int_Len_old];
                                Array.Copy(this.m_byt_RevData, 0, byt_RevData1, 0, int_Len_old);
                                if (CheckArray(byt_RevData1, byt_Data_OldSend))
                                {
                                    byte[] byt_RevData2 = new byte[m_byt_RevData.Length - int_Len_old];
                                    Array.Copy(this.m_byt_RevData, int_Len_old, byt_RevData2, 0, this.m_byt_RevData.Length - int_Len_old);
                                    m_byt_RevData = byt_RevData2;

                                }
                            }
                            #endregion

                            if (CheckFrame(m_byt_RevData))
                                break;
                            if (sth_SpaceTicker.ElapsedMilliseconds >= int_SpaceMSecond)
                                break;
                        }
                    }
                    #endregion
                }
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.Message;
            }
            finally
            {
                //if (m_bol_ClosComm)
                //{
                //    if (m_Ispt_com.State) m_Ispt_com.PortClose();
                //}
            }
        }

        /// <summary>
        /// 判断收到的数据是否完整的帧  
        /// </summary>
        /// <param name="cWrap">接收到的数据包</param>
        /// <returns></returns>
        private bool CheckFrame(byte[] byt_Value)
        {
            /// 解析数据  cWrap需要解析的包	

            try
            {
                if (byt_Value.Length < 11) //帧格式，至少12个字节
                {
                    if (byt_Value.Length == 0)
                        this.m_str_LostMessage = "没有返回数据!";
                    else
                        this.m_str_LostMessage = "返回数据不完整！";
                    return false;
                }

                //0x68
                int int_Start = 0;
                int_Start = Array.IndexOf(byt_Value, (byte)0x68);
                if (int_Start < 0 || int_Start > byt_Value.Length)
                {
                    this.m_str_LostMessage = "返回帧不符合要求，找不到0x68!";
                    return false;
                }

                //0x68
                if (byt_Value[int_Start + 7] != (byte)0x68)
                {
                    this.m_str_LostMessage = "返回帧不符合要求，找不到第2个0x68!";
                    return false;

                }

                //C　控制码，高位为1时是丛站应答标志
                if ((byt_Value[int_Start + 8] & 0x80) != (byte)0x80)
                {
                    this.m_str_LostMessage = "不是从站的应答帧!";
                    return false;

                }


                //长度
                int int_Len = byt_Value[int_Start + 9];
                if (int_Len + int_Start + 12 > byt_Value.Length)
                {
                    this.m_str_LostMessage = "实际长度与帧长度不相符!";
                    return false;
                }

                //0x16
                byte byt_End = byt_Value[int_Start + 11 + int_Len];
                if (byt_End != (byte)0x16)
                {
                    this.m_str_LostMessage = "返回帧不符合要求!，找不到帧尾0x16!";
                    return false;
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
        /// 接收数据中是否包括CLOU红外反射数据
        /// </summary>
        /// <param name="byt_Send"></param>
        /// <param name="Rev"></param>
        /// <returns></returns>
        private bool CheckArray(byte[] byt_Send, byte[] Rev)
        {
            for (int i = 0; i < byt_Send.Length; i++)
            {
                if (byt_Send[i] != Rev[i])
                {
                    return false;
                }
            }
            return true;
        }

        #endregion


        #region----------数据接收----------

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
            }
        }
        #endregion
    }
}
