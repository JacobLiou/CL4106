
/****************************************************************************

    DLT645协议类
    刘伟 2009-10

*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using pwInterface;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Collections;
using System.Globalization;
using System.Globalization;
namespace pwMeterProtocol
{


    /// <summary>
    /// 国产电能表645协议基类，适用1997、2007版本,均可继承于它
    /// </summary>
    public class DLT645:ProtocolBase
    {
        private int m_int_WaitDataRevTime = 2000;           ///等待数据到达最大时间ms
        private int m_int_IntervalTime = 500;               ///数据间隔最大时间ms
        private bool m_bol_ZendStringDel0x33 = false;		///发送接收的数据，数据域是否减0x33
        private byte m_byt_iRepeatTimes = 3;                ///通讯失败重试次数
        private bool m_bol_ClosComm = false;                ///通讯完成后是否关闭端口
        private bool m_bol_BreakDown = false;               ///被外部中断，主要用于下载打包参数时
        private string m_str_RxID = "01";                   ///R——受信节点ID码 10H，地址根据表位来1,2,3,4,5,6...

        public DLT645()
        {
            this.m_byt_RevData = new byte[0];
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
        /// 发送接收的数据帧，数据域减0x33
        /// </summary>
        public bool ZendStringDel0x33
        {
            set
            {
                this.m_bol_ZendStringDel0x33 = value;
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

        public string RxID
        {
            get
            {
                return this.m_str_RxID;
            }
            set
            {
                this.m_str_RxID = value;
            }
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
        public bool SendDLT645CommandForSCBH(byte bCmd, string cAddr, byte bDataLen, ref string cData, ref bool bExtend)
        {	/// cCmd 命令字  cAddr 地址 bLen 数据长度 cData 数据域数据 bExtend 是否有后续数据
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                Stopwatch sth_SpaceTicker = new Stopwatch();                //
                sth_SpaceTicker.Start();

                byte[] byt_SendData = this.OrgFrame(bCmd, cAddr, bDataLen, cData);

                //处理显示发送帧
                if (m_bol_ZendStringDel0x33) this.m_str_TxFrame = GetDel0x33(byt_SendData);

                if (byt_SendData.Length > 0)
                {
                    byte bRepeat = 0;
                    bool bln_Result = false;
                    while (bRepeat < m_byt_iRepeatTimes)
                    {
                        cData = "";
                        bln_Result = this.SendFrame(byt_SendData, m_int_WaitDataRevTime, m_int_IntervalTime);
                        if (!bln_Result) break;

                        //bln_Result = this.CheckFrameSCBH(this.m_byt_RevData, ref cData, ref bExtend);
                        bln_Result = this.CheckFrame_IEC_SCBH(this.m_byt_RevData, ref cData, ref bExtend);
                        if (bln_Result)
                        {
                            //处理显示接收帧
                            if (m_bol_ZendStringDel0x33) this.m_str_RxFrame = GetDel0x33(m_byt_RevData);
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
                if (m_bol_ClosComm)
                {
                    if (this.m_Ispt_com.State) this.m_Ispt_com.PortClose();
                }
            }


        }
        /// <summary>
        /// 检查返回帧合法性
        /// </summary>
        /// <param name="bWrap">返回帧</param>
        /// <param name="cData">返回数据</param>
        /// <param name="bExtend">是否有后续帧</param>
        /// <returns></returns>
        public bool CheckFrame_IEC_SCBH(byte[] byt_RevFrame, ref string cData, ref bool bExtend)
        {/// 解析数据  cWrap 需要解析的包 cData 返回的数据	bExtend 是否有后续数据
            try
            {
                //" FE 68 AA AA AA AA AA AA 68 83 05 F0 FF 81 01 82 16"
                #region 判断是否有返回数据
                if (byt_RevFrame.Length <= 0)
                {
                    this.m_str_LostMessage = "没有返回数据！";// "没有返回数据！";
                    return false;
                }
                #endregion
                #region 查找第一个 0x68  （int_Start  0x68 出现的位置）
                int int_Start = 0; //0x68 出现的位置
                int_Start = Array.IndexOf(byt_RevFrame, (byte)0x68);
                if (int_Start < 0 || int_Start > byt_RevFrame.Length || int_Start + 12 > byt_RevFrame.Length) //没有68开头 长度是否足够一帧 是否完整
                {
                    this.m_str_LostMessage ="返回帧不完整！没有帧头" //  "返回帧不完整！没有帧头。
                    + "[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }
                #endregion
                #region 查找第二个 0x68
                if (byt_RevFrame[int_Start + 7] != 0x68)        //找不到第二个68
                {
                    this.m_str_LostMessage = "返回帧不完整！" //"返回帧不完整！
                        + "[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }
                #endregion
                #region 判断返回数据 是否为 0x83-安全认证应答
                if (byt_RevFrame[int_Start + 8] != 0x83)        // 0x83-安全认证应答
                {
                    this.m_str_LostMessage ="返回帧 不是0x83-安全认证应答！" // "返回帧 不是0x83-安全认证应答！
                        + "[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }
                #endregion
                #region 判断数据长度是否合格    int_Len
                int int_Len = byt_RevFrame[int_Start + 9];
                if (int_Start + 12 + int_Len != byt_RevFrame.Length)
                {
                    this.m_str_LostMessage = "数据长度与实际长度不一致！" //"数据长度与实际长度不一致！
                        + "[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;                //帧的长度是否与实际长度一样
                }
                #endregion
                #region 判断返回校验码是否正确
                byte byt_Chksum = 0;
                for (int int_Inc = int_Start; int_Inc < int_Start + int_Len + 10; int_Inc++)
                    byt_Chksum += byt_RevFrame[int_Inc];
                if (byt_RevFrame[int_Start + int_Len + 10] != byt_Chksum)       //校验码不正确
                {
                    this.m_str_LostMessage ="返回校验码不正确！"// "返回校验码不正确！
                        + "[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }
                #endregion
                #region 判断返回帧 是否完整
                if (byt_RevFrame[int_Start + int_Len + 11] != 0x16)       //没有16结束
                {
                    this.m_str_LostMessage = "返回帧不完整！" //"返回帧不完整！
                        + "[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }
                #endregion

                string s_RxFrame = BytesToString(byt_RevFrame);

                //////读生产编号返回帧：
                //////" FE 68 AA AA AA AA AA AA 68 83 10 23 32 B3 36 33 33 33 33 33 33 33 33 33 33 33 B6 84 16"
                //////校表返回帧
                //////" FE 68 AA AA AA AA AA AA 68 83 05 23 32 B3 33 B3 42 16"
                ////byte[] byt_RevData = new byte[0];
                ////Array.Resize(ref byt_RevData, int_Len);    //数据域长度
                ////string sTempRxFrame = "";
                ////if (int_Len > 0)
                ////{
                ////    //Array.Copy(byt_RevFrame, int_Start + 10, byt_RevData, 0, int_Len);
                ////    Array.Copy(byt_RevFrame, int_Start + 10 + 3, byt_RevData, 0, int_Len - 4);
                ////    for (int int_Inc = 0; int_Inc < byt_RevData.Length - 1; int_Inc++)
                ////        byt_RevData[int_Inc] -= 0x33;

                ////    //"F0 FF 80 FF FF FF FF FF FF FF FF FF FF FF FF 74"

                ////    cData = BytesToString1(byt_RevData);
                ////    cData = BackString(cData);
                ////    //"F0FF8103B7"
                ////    int ilen = cData.Length; //调试使用 计算返回数据 长度
                ////}

                byte sRxOF = byt_RevFrame[int_Start + 10];
                byte sRxFF = byt_RevFrame[int_Start + 11];
                byte sRx80 = byt_RevFrame[int_Start + 12];
                if ((sRxOF -= 0x33) != 0x0F &&
                    (sRxFF -= 0x33) != 0xFF)
                {
                    this.m_str_LostMessage ="返回帧不符合海外校表协议，无 0F FF " // "返回帧不符合海外校表协议，无 0F FF 
                        + "！[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }
                //" FE 68 AA AA AA AA AA AA 68 83 05 23 32 B3 33 B3 42 16"
                if ((sRx80 -= 0x33) == 0x80)
                {
                    byte[] byt_RevData = new byte[0];
                    Array.Resize(ref byt_RevData, int_Len - 4);    //数据域长度
                    if (int_Len > 0)
                    {
                        //Array.Copy(byt_RevFrame, int_Start + 10, byt_RevData, 0, int_Len);
                        Array.Copy(byt_RevFrame, int_Start + 10 + 3, byt_RevData, 0, int_Len - 4);
                        for (int int_Inc = 0; int_Inc < byt_RevData.Length; int_Inc++)
                            byt_RevData[int_Inc] -= 0x33;

                        cData = BytesToString1(byt_RevData);
                        if (cData.Length > 24)
                        {
                            cData = cData.Substring(0, 24);
                        }
                        cData = BackString(cData);
                        ////int ilen = cData.Length; //调试使用 计算返回数据 长度
                    }
                    //"313330333030303034354E4D"

                    return true;
                }
                else if ((sRx80 -= 0x33) == 0x81)
                {
                    byte[] byt_RevData = new byte[0];
                    Array.Resize(ref byt_RevData, int_Len);    //数据域长度
                    if (int_Len > 0)
                    {
                        Array.Copy(byt_RevFrame, int_Start + 10, byt_RevData, 0, int_Len);
                        ////Array.Copy(byt_RevFrame, int_Start + 10 + 3, byt_RevData, 0, int_Len - 4);
                    }

                    if (byt_RevData != null && byt_RevData.Length > 4)
                        this.m_str_LostMessage =  "[" + BitConverter.ToString(byt_RevFrame) + "]";
                    else
                        this.m_str_LostMessage = "返回操作失败" // "返回操作失败
                            + "！[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }
                else
                {
                    this.m_str_LostMessage =  "返回操作失败" // "返回操作失败
                          + "！[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        #region BytesToString1 无空格显示方式
        private string BytesToString1(byte[] byt_Values)
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (byte bt in byt_Values)
            {
                strBuilder.AppendFormat("{0:X2}", bt);
            }
            return strBuilder.ToString();
        }
        #endregion
        /// <summary>
        /// 命令链路
        /// </summary>
        /// <param name="bCmd">控制码</param>
        /// <param name="cAddr">表地址</param>
        /// <param name="bDataLen">数据长度</param>
        /// <param name="cData">数据域数据</param>
        /// <param name="bExtend">是否有后续数据</param>
        /// <returns></returns>
        public bool SendDLT645Command(byte bCmd, string cAddr, byte bDataLen, ref string cData, ref bool bExtend)
        {	/// cCmd 命令字  cAddr 地址 bLen 数据长度 cData 数据域数据 bExtend 是否有后续数据
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                Stopwatch sth_SpaceTicker = new Stopwatch();                //
                sth_SpaceTicker.Start();

                byte[] byt_SendData = this.OrgFrame(bCmd, cAddr, bDataLen, cData);

                //处理显示发送帧
                if(m_bol_ZendStringDel0x33) this.m_str_TxFrame = GetDel0x33(byt_SendData);

                if (byt_SendData.Length > 0)
                {
                    byte bRepeat = 0;
                    bool bln_Result = false;
                    while (bRepeat < m_byt_iRepeatTimes)
                    {
                        cData = "";
                        bln_Result = this.SendFrame(byt_SendData, m_int_WaitDataRevTime, m_int_IntervalTime);
                        if (!bln_Result) break;

                        bln_Result = this.CheckFrame(this.m_byt_RevData, ref cData, ref bExtend);
                        if (bln_Result)
                        { 
                            //处理显示接收帧
                            if (m_bol_ZendStringDel0x33)  this.m_str_RxFrame = GetDel0x33(m_byt_RevData);
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
                if(m_bol_ClosComm)
                {
                    if (this.m_Ispt_com.State) this.m_Ispt_com.PortClose();
                }
            }


        }

        /// <summary>
        /// 命令链路
        /// </summary>
        /// <param name="bCmd">控制码</param>
        /// <param name="cAddr">表地址</param>
        /// <param name="bDataLen">数据长度</param>
        /// <param name="cData">数据域数据</param>
        /// <param name="byt_RevDataF">数据帧</param>
        /// <param name="bExtend">是否有后续数据</param>
        /// <returns></returns>
        public bool SendDLT645Command(byte bCmd, string cAddr, byte bDataLen, ref string cData, ref byte[] byt_RevDataF, ref bool bExtend)
        {	/// cCmd 命令字  cAddr 地址 bLen 数据长度 cData 数据域数据 bExtend 是否有后续数据
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                Stopwatch sth_SpaceTicker = new Stopwatch();                //
                sth_SpaceTicker.Start();

                byte[] byt_SendData = this.OrgFrame(bCmd, cAddr, bDataLen, cData);

                //处理显示发送帧
                if (m_bol_ZendStringDel0x33) this.m_str_TxFrame = GetDel0x33(byt_SendData);

                if (byt_SendData.Length > 0)
                {
                    byte bRepeat = 0;
                    bool bln_Result = false;
                    while (bRepeat < m_byt_iRepeatTimes)
                    {
                        cData = "";
                        bln_Result = this.SendFrame(byt_SendData, m_int_WaitDataRevTime, m_int_IntervalTime);
                        if (!bln_Result) break;

                        bln_Result = this.CheckFrame(this.m_byt_RevData, ref cData, ref bExtend);
                        byt_RevDataF = this.m_byt_RevData;
                        if (bln_Result)
                        {
                            //处理显示接收帧
                            if (m_bol_ZendStringDel0x33) this.m_str_RxFrame = GetDel0x33(m_byt_RevData);
                            break;
                        }


                        Thread.Sleep(1);
                        bRepeat++;
                        if (m_bol_BreakDown) return false;

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
                if (m_bol_ClosComm)
                {
                    if (this.m_Ispt_com.State) this.m_Ispt_com.PortClose();
                }
            }

        }


        /// <summary>
        /// 发送数据帧，返回接收数据帧，用于下载打包参数文件
        /// </summary>
        /// <param name="RxFrame">发送帧</param>
        /// <param name="TxFrame">接收帧</param>
        /// <param name="cData">接收数据域</param>
        /// <returns></returns>
        public bool SendDLT645RxFrame(string RxFrame, ref string TxFrame, ref string cData)
        {	/// RxFrame 发送帧  TxFrame 接收帧 cData 接收数据域
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                Stopwatch sth_SpaceTicker = new Stopwatch();                //
                sth_SpaceTicker.Start();

                bool bExtend = false;
                byte[] byt_SendData = HexCon.StringToByte(RxFrame);
                if (byt_SendData.Length > 0)
                {
                    byte bRepeat = 0;
                    bool bT = false;
                    while (bRepeat < m_byt_iRepeatTimes)
                    {

                        cData = "";
                        byte[] cWrap = Message(byt_SendData);
                        TxFrame = HexCon.ByteToString(cWrap).Trim();
                        bT = CheckFrame(cWrap, ref cData, ref bExtend);
                        if (bT) break;
                        Application.DoEvents();
                        Thread.Sleep(500);
                        bRepeat++;
                        if (m_bol_BreakDown) return false; 
                    }
                    this.m_str_LostMessage = @"sum time :" + sth_SpaceTicker.ElapsedMilliseconds.ToString();
                    return bT;
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

        }

        /// <summary>
        /// 打包参数下载
        /// </summary>
        /// <param name="_DownParaItemOne">下载帧列表</param>
        /// <param name="AllAddress">统一通信地址</param>
        /// <returns></returns>
        public bool DownPara(List<MeterDownParaItem> _DownParaItemOne)
        {

            try
            {
                //List<StDownParaItem> _DownParaItemOne = _DownParaItem;
                bool bRet = false;

                #region 取得总项数
                int i_DownParaSum = _DownParaItemOne.Count;
                #endregion

                #region 下载设置项

                string sdata = "";
                string cdata = "";

                for (int i = 0; i < i_DownParaSum ; i++)
                {
                    //需要处理外部中断
                    Thread.Sleep(1);//等待时间，好浪费哟
                    Application.DoEvents();
                    bRet = SendDLT645RxFrame(_DownParaItemOne[i].Item_TxFrame, ref sdata, ref cdata);
                    if(! bRet)  break  ;
                    if (m_bol_BreakDown) break;

                    //if (! (_DownParaItemOne[i].Item_RxFrame.Contains(sdata) || sdata.Contains(_DownParaItemOne[i].Item_RxFrame)) ) break;

                }
                #endregion


                return bRet;

            }
            catch (System.Exception Error)
            {
                return false;
            }
        }

        /// <summary>
        /// 读功耗
        /// </summary>
        /// <param name="int_BwIndex"></param>
        /// <param name="int_Chancel"></param>
        /// <param name="flt_U"></param>
        /// <param name="flt_I"></param>
        /// <param name="flt_ActiveP"></param>
        /// <param name="flt_ApparentP"></param>
        /// <returns></returns>
        public bool ReadPower(int int_BwIndex, int int_Chancel, ref float flt_U, ref float flt_I, ref float flt_ActiveP, ref float flt_ApparentP)
        {
            flt_U = 0f;
            flt_I = 0f;
            flt_ActiveP = 0f;
            flt_ApparentP = 0f;
            return false;
        }

        public bool ReadPower(ref float flt_ActiveP)
        {
            this.m_str_LostMessage = "本协议不技持";
            flt_ActiveP = 0f;
            return false;
        }

        #region 
        /// <summary>
        /// 海外校表协议使用的---组帧通用框架---协议框架DLT645_1997
        /// </summary>
        /// <param name="Addr"></param>
        /// <param name="i_APCU"></param>
        /// <param name="ArrayList_APDU"></param>
        /// <param name="fpByteList"></param>
        /// <param name="str_LostMessage"></param>
        /// <returns></returns>
        public bool MakeFrame_Measure_Calibrate(string Addr, int i_APCU, ArrayList ArrayList_APDU, ref ArrayList fpByteList, ref string str_LostMessage)
        {
            str_LostMessage = "";
            bool bResult = false;
            string sData = "";
            string sTemp = "";
            int iStartByte = 0;
            int j = 0;
            //byte bTemp = 0;
            int iTemp = 0;
            byte[] fpByte = new byte[10];
            byte[] bSend = new byte[10];
            ArrayList arlist = new ArrayList();
            fpByteList.Clear();
            try
            {
                #region  DLT645_1997 组帧  协议框架
                #region   0、DLT645_1997 组帧   协议框架 组帧说明
                //发送帧： 0x68	地址	0x68	控制码	数据长度	数据标识	操作者代码	APCU APDU 	APKU	校验码	0x16
                //APKU为 APCU与APDU的校验和，长度为一个字节。
                //应答帧： Addr	Func	RegisterAddr 	RegisterNum 	RegisterLen	APCU	APRU	APKU	CRC
                //APKU为 APCU与APRU的校验和，长度为一个字节。
                #endregion
                #region 1、APCU-- OBISID 命令码 1字节 放入 ArrayList arlist
                sTemp = i_APCU.ToString("X2").PadLeft(2, '0').Substring(0, 2);
                for (int i = sTemp.Length / 2; i > 0; i--)
                {
                    arlist.Add(byte.Parse(sTemp.Substring(i * 2 - 2, 2), NumberStyles.HexNumber));
                }
                #endregion
                #region 2、APDU (ArrayList_APDU 校验数据 放入 ArrayList arlist)

                #region APDU nBytes 如n+2为奇数，则APKU后补1个字节0x00.
                if (ArrayList_APDU.Count % 2 != 0)
                {
                    ArrayList_APDU.Add(0x00);
                }
                #endregion

                for (int i = 0; i < ArrayList_APDU.Count; i++)
                {
                    arlist.Add(ArrayList_APDU[i]);
                }
                #endregion
                #region 3、APKU 校验码 ( 校验码=APCU+APDU)
                iStartByte = 0;
                arlist.Add(Get_CheckSum(arlist, iStartByte));
                #endregion
                #region 4、0x68	   地址	  0x68	 控制码	 数据长度	数据标识	操作者代码	APCU 	APDU 	APKU  校验码	0x16
                //         1字节  6字节   1字节  1字节   1字节      2字节       4字节                             1字节     1字节

                bSend = new byte[1 + 6 + 1 + 1 + 1 + 2 + 4 + arlist.Count + 1 + 1];

                #region  0x68 1字节
                bSend[0] = (byte)0x68;
                #endregion
                #region 电表地址 6字节-- MeterAddress  放入  byte[] bSend
                j = 1;
                Addr = Addr.PadLeft(12, '0').Substring(0, 12);
                for (int i = Addr.Length / 2; i > 0; i--)
                {
                    bSend[j] = byte.Parse(Addr.Substring(i * 2 - 2, 2), NumberStyles.HexNumber);
                    j++;
                }
                #endregion
                #region  0x68 1字节
                bSend[7] = (byte)0x68;
                #endregion
                #region  控制码 1字节--  0x03-安全认证
                bSend[8] = (byte)0x03;
                #endregion
                #region  数据长度 1字节-- 数据域包括( 数据标识 + 操作者代码	+ APCU + APDU + APKU)
                bSend[9] = (byte)(2 + 4 + arlist.Count);
                #endregion

                #region 数据域数据  +0x33

                #region  数据标识 2字节--  0xF0, 0xFF-用于校表
                iTemp = 0xF0 + 0x33;
                bSend[10] = (byte)iTemp;
                iTemp = 0xFF + 0x33;
                bSend[11] = (byte)iTemp;
                #endregion
                #region  操作者代码 4字节--  固定为0x00,0x00,0x00,0x00
                iTemp = 0x00 + 0x33;
                bSend[12] = (byte)iTemp;
                bSend[13] = (byte)iTemp;
                bSend[14] = (byte)iTemp;
                bSend[15] = (byte)iTemp;
                #endregion
                #region  APCU + APDU + APKU

                j = 16;
                fpByte = new byte[arlist.Count];
                arlist.CopyTo(fpByte, 0);

                for (int i = 0; i < fpByte.Length; i++)
                {
                    iTemp = fpByte[i] + 0x33;
                    bSend[j + i] = (byte)iTemp;
                    //bSend[j + i] = Convert.ToByte(arlist[i].ToString(), NumberStyles.HexNumber);
                }
                #endregion

                #endregion

                #region 校验码 CS 1字节
                CRC8 CRC8 = new CRC8();
                byte CRC = CRC8.CRC(bSend);
                bSend[bSend.Length - 2] = CRC;
                #endregion
                #region 结束符 16H 1字节
                bSend[bSend.Length - 1] = 0x16;
                #endregion


                #endregion DLT645_1997 组帧  协议框架
                #region 5、将完整的帧数据 赋值 到 fpByteList 返回
                fpByteList.AddRange(bSend);
                #endregion
                #endregion
                str_LostMessage = "OK";
                bResult = true;

                return bResult;
            }
            catch (Exception)
            {
                return bResult;
            }
        }

        public bool SendAdjustData(int i_APCU, ArrayList ArrayList_APDU)
        {
            ArrayList arlist = new ArrayList();
            ArrayList_APDU.Clear();
            return false;
        }

        #endregion



        #region---私有-------------------------



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

            //处理显示发送帧
            this.m_str_TxFrame = GetDel0x33(byt_SendData);
            //发送帧
            this.SendFrame(byt_SendData, m_int_WaitDataRevTime, m_int_IntervalTime);
            //处理显示接收帧
            this.m_str_RxFrame = GetDel0x33(m_byt_RevData);
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
        private bool CheckFrameSCBH(byte[] byt_RevFrame, ref string cData, ref bool bExtend)
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
                if (int_Start + 12 + int_Len != byt_RevFrame.Length)
                {
                    this.m_str_LostMessage = "数据长度与实际长度不一致！[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;                //帧的长度是否与实际长度一样
                }
                byte byt_Chksum = 0;
                for (int int_Inc = int_Start; int_Inc < int_Start + int_Len + 10; int_Inc++)
                    byt_Chksum += byt_RevFrame[int_Inc];
                if (byt_RevFrame[int_Start + int_Len + 10] != byt_Chksum)       //校验码不正确
                {
                    this.m_str_LostMessage = "返回校验码不正确！[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }
                if (byt_RevFrame[int_Start + int_Len + 11] != 0x16)       //没有16结束
                {
                    this.m_str_LostMessage = "返回帧不完整！[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }

                //Array.Resize(ref byt_Addr, 6);
                //Array.Copy(byt_RevFrame, int_Start + 1, byt_Addr, 0, 6);
                //cmd
                byte[] byt_RevData = new byte[0];
                Array.Resize(ref byt_RevData, 6);    //数据域长度
                if (int_Len > 0)
                {
                    Array.Copy(byt_RevFrame, int_Start + 1, byt_RevData, 0, 6);
                    //for (int int_Inc = 0; int_Inc < byt_RevData.Length; int_Inc++)
                    //    byt_RevData[int_Inc] -= 0x33;

                    cData = BytesToString(byt_RevData);
                }


                //是否有后续帧
                if ((byt_RevFrame[int_Start + 8] & 0x20) == 0x20)
                    bExtend = true;
                else
                    bExtend = false;

                //是否返回操作成功     第7Bit是1则是返回，第6bit是0=成功，1=失败
                if ((byt_RevFrame[int_Start + 7] ) == 0x68 )
                    return true;
                else
                {
                    if (byt_RevData != null && byt_RevData.Length > 0)
                        this.m_str_LostMessage = GetErrorMsg645(byt_RevData[0]) + "[" + BitConverter.ToString(byt_RevFrame) + "]";
                    else
                        this.m_str_LostMessage = "返回操作失败！[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }

            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 检查返回帧合法性
        /// </summary>
        /// <param name="bWrap">返回帧</param>
        /// <param name="cData">返回数据</param>
        /// <param name="bExtend">是否有后续帧</param>
        /// <returns></returns>
        private bool CheckFrame(byte[] byt_RevFrame, ref string cData, ref bool bExtend)
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
                if (int_Start + 12 + int_Len != byt_RevFrame.Length)
                {
                    this.m_str_LostMessage = "数据长度与实际长度不一致！[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;                //帧的长度是否与实际长度一样
                }
                byte byt_Chksum = 0;
                for (int int_Inc = int_Start; int_Inc < int_Start + int_Len + 10; int_Inc++)
                    byt_Chksum += byt_RevFrame[int_Inc];
                if (byt_RevFrame[int_Start + int_Len + 10] != byt_Chksum)       //校验码不正确
                {
                    this.m_str_LostMessage = "返回校验码不正确！[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }
                if (byt_RevFrame[int_Start + int_Len + 11] != 0x16)       //没有16结束
                {
                    this.m_str_LostMessage = "返回帧不完整！[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }

                //Array.Resize(ref byt_Addr, 6);
                //Array.Copy(byt_RevFrame, int_Start + 1, byt_Addr, 0, 6);
                //cmd
                byte[] byt_RevData = new byte[0];
                Array.Resize(ref byt_RevData, int_Len);    //数据域长度
                if (int_Len > 0)
                {
                    Array.Copy(byt_RevFrame, int_Start + 10, byt_RevData, 0, int_Len);
                    for (int int_Inc = 0; int_Inc < byt_RevData.Length; int_Inc++)
                        byt_RevData[int_Inc] -= 0x33;

                    cData=BytesToString(byt_RevData);
                }


                //是否有后续帧
                if ((byt_RevFrame[int_Start + 8] & 0x20) == 0x20)
                    bExtend = true;
                else
                    bExtend = false;

                //是否返回操作成功     第7Bit是1则是返回，第6bit是0=成功，1=失败
                if ((byt_RevFrame[int_Start + 8] & 0x80) == 0x80 && (byt_RevFrame[int_Start + 8] & 0x40) == 0x00)
                    return true;
                else
                {
                    if (byt_RevData != null && byt_RevData.Length > 0)
                        this.m_str_LostMessage = GetErrorMsg645(byt_RevData[0]) + "[" + BitConverter.ToString(byt_RevFrame) + "]";
                    else
                        this.m_str_LostMessage = "返回操作失败！[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }

            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }




        /// <summary>
        /// 解析数据域减0x33后的数据帧
        /// </summary>
        /// <param name="cWrap"></param>
        /// <returns></returns>
        private string GetDel0x33(byte[] byt_Rec)
        {	///解析数据域减0x33后的数据帧

            int int_Len = byt_Rec.Length;
            byte[] byt_Value = new byte[int_Len];
            Array.Copy(byt_Rec, 0, byt_Value, 0, int_Len);

            try
            {
                if (m_bol_ZendStringDel0x33)//是否数据域减0x33
                {
                    int iTn = 0, iTi = 0, iLen = 0;

                    if (byt_Value.Length <= 10) return HexCon.ByteToString(byt_Value);
                    /// 查找帧头
                    int int_Start = 0;
                    int_Start = Array.IndexOf(byt_Value, (byte)0x68);

                    iLen = byt_Value.Length;
                    /// 解析数据域的数据
                    if ((byt_Value[int_Start + 9] + 12) == byt_Value.Length)
                    {
                        iTi = byt_Value[int_Start + 9];
                    }

                    for (iTn = 0; iTn < iTi; iTn++)
                    {
                        byt_Value[iTn + int_Start + 10] -= 0x33;
                    }
                }
                ///解析数据域减0x33后的数据帧
                return HexCon.ByteToString(byt_Value);
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return "";
            }
        }



        /// <summary>
        /// 返回操作失败，错误代码
        /// </summary>
        /// <param name="byt_ErrCode"></param>
        /// <returns></returns>
        private string GetErrorMsg645(byte byt_ErrCode)
        {
            //保留	费率数超	日时段数超	年时区数超	通信速率不能更改	密码错/未授权	无请求数据	其他错误
            string str_Msg = "";
            if ((byt_ErrCode & 1) == 1)
                str_Msg = "发送非法数据";
            else if ((byt_ErrCode & 2) == 2)
                str_Msg = "标识符错误";
            else if ((byt_ErrCode & 4) == 4)
                str_Msg = "密码错误";
            else if((byt_ErrCode & 8) == 8)
                str_Msg = "通信速率不能更改";
            else if ((byt_ErrCode & 16) == 16)
                str_Msg = "年时区数超出";
            else if ((byt_ErrCode & 32) == 32)
                str_Msg = "日时段数超出";
            else if ((byt_ErrCode & 64) == 64)
                str_Msg = "费率数超出";
            return str_Msg;
        }

        public static byte Get_CheckSum(ArrayList ar, int iStartByte)
        {
            byte b = 0;
            try
            {
                for (int i = iStartByte; i < ar.Count; i++)
                {
                    b += Convert.ToByte(ar[i].ToString());
                }
                return b;
            }
            catch (Exception)
            {
                return b;
            }
        }

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

    #region DLT645_1997 组帧--CRC8校验码 1字节
    public class CRC8
    {
        public byte CRC(byte[] bLen)
        {
            byte CRC8 = 0;
            /// 校验码
            for (int i = 0; i < bLen.Length - 2; i++)
            {
                CRC8 += bLen[i];
            }
            return CRC8;
        }

    }
    #endregion

    class HexCon
    {
        public static byte[] ASCIIStringToByte(string InString)//将一组字符编码为一个字节序列。
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetBytes(InString);
        }
        public static string ASCIIByteToString(byte[] InBytes)// 将一个字节序列解码为一个字符串。 
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetString(InBytes);
        }
        public static string HexStringToASCIIString(string InString)// 将一个字节序列解码为一个字符串。 
        {
            if (InString == "") return "";
            byte[] Inbytestmp = StringToByte(InString);
            return ASCIIByteToString(Inbytestmp);
        }
        public static string ASCIIStringToHexString(string InString)//  
        {
            if (InString == "") return "";
            byte[] Inbytestmp = ASCIIStringToByte(InString);
            return ByteToString(Inbytestmp);
        }
        //converter hex string to byte and byte to hex string
        public static string ByteToString(byte[] InBytes)
        {
            string StringOut = "";
            foreach (byte InByte in InBytes)
            {
                StringOut = StringOut + String.Format("{0:X2} ", InByte);
            }
            return StringOut;
        }

        /// <summary>
        /// 字符串转化为字节数组
        /// </summary>
        /// <param name="InString">帧</param>
        /// <returns></returns>
        public static byte[] StringToByte(string InString)
        {
            string[] ByteStrings;
            int iLen;

            InString = InString.Trim();
            ByteStrings = InString.Split(" ".ToCharArray());
            byte[] ByteOut;
            ByteOut = new byte[ByteStrings.Length];
            iLen = ByteStrings.Length - 1;
            string hexstr;
            if (InString.Length <= 0)
            {
                return ByteOut;
            }
            for (int i = 0; i <= iLen; i++)
            {
                hexstr = "0x" + ByteStrings[i];
                ByteOut[i] = System.Convert.ToByte(hexstr, 16);
            }
            return ByteOut;
        }


    //    public static string getCodeById(int id)
    //    {
    //        string strVal = "";
    //        switch (id)
    //        {
    //            case 48:
    //                strVal = "1";
    //                break;
    //            default:
    //                break;
    //        }
    //        return strVal;

    }
}
