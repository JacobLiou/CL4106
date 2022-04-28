/****************************************************************************

    DL/T645-2007协议
    刘伟 2009-10

*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using pwInterface;
using System.Collections;
using System.Globalization;
namespace pwMeterProtocol
{
    public class IEC62056_21 : DLT645, IMeterProtocol
    {


        private string m_str_Address = "000000000000";      ///表地址
        private string m_str_Password = "000000";           ///表密码
        private byte m_byt_PasswordClass = 2;               ///表密码等级
        private string m_str_UserCode = "00000000";         ///操作员代码
        private int m_int_PasswordType = 1;                 ///密码验证类型，0＝无密码认证 1＝密码放在数据域中方式
        private bool m_bol_DataFieldPassword = false;       ///写操作时，数据域是否包含写密码,true=要，false=不用

        public IEC62056_21()
        {
            this.m_byt_RevData = new byte[0];
        }

        #region DLT645_2007成员


        #region 属性
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
        /// 表地址
        /// </summary>
        public string Address
        {
            get
            {
                return this.m_str_Address;
            }
            set
            {
                this.m_str_Address = value;
            }
        }

        /// <summary>
        /// 写操作密码
        /// </summary>
        public string Password
        {
            get
            {
                return this.m_str_Password;
            }
            set
            {
                this.m_str_Password = value;
            }
        }

        /// <summary>
        /// 密码等级
        /// </summary>
        public byte PasswordClass
        {
            get
            {
                return this.m_byt_PasswordClass;
            }
            set
            {
                this.m_byt_PasswordClass = value;
                
            }
        }

        /// <summary>
        /// 操作员代码
        /// </summary>
        public string UserID
        {
            get
            {
                return this.m_str_UserCode;
            }
            set
            {
                this.m_str_UserCode = value;
            }
        }

        /// <summary>
        /// 密码验证类型，0＝无密码认证 1＝密码放在数据域中方式 2＝A型表密码认证方式 3＝B型表密码认证方式
        /// </summary>
        public int VerifyPasswordType
        {
            get
            {
                return this.m_int_PasswordType;
            }
            set
            {
                this.m_int_PasswordType = value;
            }
        }

        /// <summary>
        /// 写操作时，数据域是否包含写密码,true=要，false=不用
        /// </summary>
        public bool DataFieldPassword
        {
            get
            {
                return this.m_bol_DataFieldPassword;
            }
            set
            {
                this.m_bol_DataFieldPassword = value;
            }
        }


        #endregion

        #region 基本方法
        /// <summary>
        /// 0x11读取数据（字符型，数据项）
        /// </summary>
        /// <param name="str_OBIS">标识符,4个字节</param>
        /// <param name="int_Len">返回数据长度(字节数)</param>
        /// <param name="str_Value">返回数据，可带参数读取</param>
        /// <returns></returns>
        public bool ReadData(string str_OBIS, int int_Len, ref string str_Value)
        {
            try
            {
                bool bE = false;
                string cDataStr = "";
                int iLen = 0;
                cDataStr = str_Value + str_OBIS;
                iLen = cDataStr.Length / 2;
                bool bT = SendDLT645Command(0x11, this.m_str_Address, (byte)iLen, ref cDataStr, ref bE);
                if (!bT)
                {
                    str_Value = "";
                    return false;
                }
                else
                {
                    str_Value = cDataStr.Substring(8);
                    str_Value = BackString(str_Value);
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
        /// 0x14写数据(字符型，数据项)
        /// </summary>
        /// <param name="str_OBIS">标识符,两个字节</param>
        /// <param name="int_Len">写入参数数据长度(块中每项字节数)</param>
        /// <param name="str_Value">写入数据</param>
        /// <returns></returns>
        public bool WriteData(string str_OBIS, int int_Len, string str_Value)
        {
            bool bE = false;
            string cData = BackString(str_OBIS);
            str_Value = BackString(str_Value.PadLeft(2 * int_Len, '0'));
            if (m_int_PasswordType == 1)
            {
                int_Len += 8;
                cData = cData + m_byt_PasswordClass.ToString("X2") + BackString(m_str_Password.PadLeft(6, '0')) + BackString(m_str_UserCode.PadLeft(8, '0'));
                cData += str_Value;
            }
            else if (m_int_PasswordType == 0)
            {
                cData += str_Value;
            }
            cData = BackString(cData);
            bool bT = SendDLT645Command(0x14, m_str_Address, (byte)(int_Len + 4), ref cData, ref bE);
            return bT;

        }
        #region 海外--1、IEC框架--读生产编号 ReadScbh_IEC
        /// <summary>
        /// 海外校表协议组帧通用-- 协议框架 --  读取生产编号 IEC_ReadScbh
        /// </summary>
        /// <param name="iBW">表位</param>
        /// <param name="UseSizeCommSetting">系统设置的电表联机后的通讯波特率</param>
        /// <param name="sTxFrame">发送帧</param>
        /// <param name="sRxFrame">返回帧</param>
        /// <param name="sRxData">返回数据</param>
        /// <returns></returns>
        /// 
        public bool IEC_ReadScbh(string UseSizeCommSetting, ref string sTxFrame, ref string sRxFrame, ref string sRxData)
        {
            ////8.1	读生产编号
            ////命令帧：
            ////APCU为Measure_CalibrateCmd_RdProductId(0x60)；
            ////APDU为0x00

            ////应答帧：
            ////正确时APCU为Measure_CalibrateCmd_Ok (0x80)；APRU为6个字节的生产编号，低字节先传；
            ////错误时APCU为Measure_CalibrateCmd_Err (0x81)；APRU为0x01~0x03（0x01代表校验和出错,0x02代表长度出错 ,0x03代表APCU出错）


            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;

                Stopwatch sth_SpaceTicker = new Stopwatch();                //
                sth_SpaceTicker.Start();

                bool bExtend = false;
                byte bRepeat = 0;
                bool bln_Result = false;

                ArrayList tmpByteList = new ArrayList();
                ArrayList ArrayList_APDU = new ArrayList();
                ArrayList_APDU.Add(byte.Parse("00", NumberStyles.HexNumber));
                bln_Result = MakeFrame_Measure_Calibrate("AAAAAAAAAAAAAA", 0x60, ArrayList_APDU, ref tmpByteList, ref m_str_LostMessage);

                if (!bln_Result) return false;

                byte[] byt_SendData = new byte[tmpByteList.Count];
                tmpByteList.CopyTo(byt_SendData, 0);

                //byte[] byt_SendData = OrigWrap(sCmd, cAddr, bDataLen, cData);
                //处理显示发送帧
                this.m_str_TxFrame = BytesToString(byt_SendData);
                //m_str_TxFrame :  "68AAAAAAAAAAAA6803092332333333339333935216"
                sTxFrame = m_str_TxFrame;

                if (byt_SendData.Length > 0)
                {
                    while (bRepeat < 1)
                    {
                        sRxData = "";
                        bln_Result = this.SendFrame(byt_SendData, 2000, 500);
                        if (!bln_Result) break;

                        bln_Result = this.CheckFrame_IEC_Read_SCBH(this.m_byt_RevData, ref sRxData, ref bExtend);
                        this.m_str_RxFrame = BytesToString(m_byt_RevData);
                        sRxFrame = m_str_RxFrame;


                        //"2F-43-4C-55-35-44-44-53-46-37-32-30-2D-4E-31-30-20-20-20-20-20-38-30-33-31-33-33-33-30-33-38-33-30-33-30-33-30-33-30-33-33-33-33-33-32-35-36-46-41-0D-0A"
                        //"2F434C5535444453463732302D4E31302020202020383033313333333033383330333033303330333333333331353446370D0A"


                        //"FE-68-AA-AA-AA-AA-AA-AA-68-83-0A-F0-FF-80-FF-FF-FF-FF-FF-FF-7A-3A-16"
                        ////"FE68AAAAAAAAAAAA6883052332B434B54616"
                        if (bln_Result)
                        {
                            //处理显示接收帧
                            this.m_str_RxFrame = BytesToString(m_byt_RevData);

                            break;
                        }
                        //if (PubVariable.m_bln_Selected[iBW] != true)
                        //{
                        //    bRepeat = m_byt_iRepeatTimes;
                        //    break;
                        //}

                        Thread.Sleep(1);
                        bRepeat++;


                        //if (m_bol_BreakDown) return false;

                    }
                    sth_SpaceTicker.Stop();
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
                //if (m_bol_ClosComm)
                //{
                //    if (this.m_Ispt_com.State) this.m_Ispt_com.PortClose();
                //}
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

        #region 检查返回帧合法性 CheckFrame_IEC_Read_SCBH
        /// <summary>
        /// 检查返回帧合法性
        /// </summary>
        /// <param name="bWrap">返回帧</param>
        /// <param name="cData">返回数据</param>
        /// <param name="bExtend">是否有后续帧</param>
        /// <returns></returns>
        public bool CheckFrame_IEC_Read_SCBH(byte[] byt_RevFrame, ref string cData, ref bool bExtend)
        {/// 解析数据  cWrap 需要解析的包 cData 返回的数据	bExtend 是否有后续数据
            try
            {

                //         public const byte backslash = 0x2F;// char /
                //public const byte SOH = 0x01;
                //public const byte STX = 0x02;
                //public const byte ETX = 0x03;
                //public const byte EOT = 0x04;
                //public const byte LF = 0x0A;
                //public const byte CR = 0x0D;
                //public const byte ACK = 0X06;
                //public const byte NAK = 0X15;



                if (byt_RevFrame.Length <= 0)
                {
                    this.m_str_LostMessage = "没有返回数据！"; // Language.GetWord(SystemID.SYS_NoReturnData);// "没有返回数据！";
                    return false;
                }
                Int32 int_Start = 0;
                int_Start = Array.IndexOf(byt_RevFrame, (byte)0x2F);
                int_Start = int.Parse(int_Start.ToString());

                string ss = int_Start.ToString();
                if (ss == "-1")
                {
                    int_Start = 0;
                }
                //byte bFrameTou = byt_RevFrame[int_Start];


                #region ========= Sunboy 2013-05-07 修改 临时使用的 应对 CL2018 返回乱码的问题
                if (byt_RevFrame[int_Start] != 0x2F)
                {
                    int_Start = Array.IndexOf(byt_RevFrame, (byte)0x2D);
                }
                #endregion==================================================================


                if (int_Start < 0 || int_Start > byt_RevFrame.Length)// || int_Start + 4 > byt_RevFrame.Length) //没有0x2F开头 长度是否足够一帧 是否完整
                {
                    this.m_str_LostMessage = "返回帧不完整！没有帧头。"   //Language.GetWord(SystemID.SYS_Info_400179) //  "返回帧不完整！没有帧头。
                    + "[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }

                if (byt_RevFrame[byt_RevFrame.Length - int_Start - 2] != 0x0D &&
                    byt_RevFrame[byt_RevFrame.Length - int_Start - 1] != 0x0A)
                {
                    this.m_str_LostMessage = "返回帧不完整！"  // Language.GetWord(SystemID.SYS_ReturnData_halfbaked) //"返回帧不完整！
                        + "[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }

                #region 判断 返回值是否正确
                bool bResult = false;
                string sRefValue = "";
                string sRef = "";
                if (byt_RevFrame.Length >= 27)
                {
                    if (byt_RevFrame[int_Start] == 0x2D)
                    {
                        #region ========= Sunboy 2013-05-07 修改 临时使用的 应对 CL2018 返回乱码的问题
                        #region 如果返回的 第 5bit 不为 0x5C 则 取 21、22 位进行判断 是否为 80  从 23 位开始取 生产编号
                        ////sRef = ((byt_RevFrame[int_Start + 21] - 0x30).ToString()) + ((byt_RevFrame[int_Start + 22] - 0x30).ToString());
                        ////sRefValue = ((byt_RevFrame[int_Start + 23] - 0x30).ToString()) + ((byt_RevFrame[int_Start + 24] - 0x30).ToString());

                        byte[] byt_APCU = new byte[0];
                        Array.Resize(ref byt_APCU, 2);    //数据域长度
                        //从 21 位开始取 APCU  80 
                        Array.Copy(byt_RevFrame, int_Start + 21, byt_APCU, 0, 2);
                        sRef = HexCon.ASCIIByteToString(byt_APCU);


                        byte[] byt_APRU = new byte[0];
                        Array.Resize(ref byt_APRU, 2);    //数据域长度
                        //从 23 位开始取 APRU 00 
                        Array.Copy(byt_RevFrame, int_Start + 23, byt_APRU, 0, 2);
                        sRefValue = HexCon.ASCIIByteToString(byt_APRU);

                        if (sRef == "80")
                        {
                            cData = "";
                            ////"313330383030303033333256"
                            #region 有问题的 处理方法
                            //////////"313330383030303033333256"
                            //////for (int i = 0; i < 24; i++)
                            //////{  
                            //////     //从 23 位开始取 生产编号 
                            //////    cData = cData + ((byt_RevFrame[int_Start + 23 + i] - 0x30).ToString());
                            //////}
                            //////if (cData.Length > 24)
                            //////{
                            //////    cData = cData.Substring(0, 24);
                            //////}
                            #endregion
                            #region 新的处理方法 2013-09-13 SunBoy 修改
                            byte[] byt_RevData = new byte[0];
                            Array.Resize(ref byt_RevData, 24);    //数据域长度
                            //从 23 位开始取 生产编号 
                            Array.Copy(byt_RevFrame, int_Start + 23, byt_RevData, 0, 24);

                            //////"313330393030303031554A31"
                            cData = HexCon.ASCIIByteToString(byt_RevData);

                            if (cData.Length > 24)
                            {
                                cData = cData.Substring(0, 24);
                            }
                            #endregion
                            //if (sRefValue == "00")
                            //{
                            bResult = true;
                            //}
                        }
                        else if (sRef == "81")
                        {
                            if (sRefValue == "01")
                            {
                                bResult = false;
                                ////LabelChkValue[iNum].Text = "0x01 校验和错误";
                                ////RefAdjustValue[iNum] = "0x01 校验和错误";
                            }
                            else if (sRefValue == "02")
                            {
                                bResult = false;
                                ////LabelChkValue[iNum].Text = "0x02 长度出错";
                                ////RefAdjustValue[iNum] = "0x02 长度出错";

                            }
                            else if (sRefValue == "03")
                            {
                                bResult = false;
                                ////LabelChkValue[iNum].Text = "0x03 APCU出错";
                                ////RefAdjustValue[iNum] = "0x03 APCU出错";
                            }
                        }
                        else
                        {
                            bResult = false;
                            ////LabelChkValue[iNum].Text = "0x03 APCU出错";
                            ////RefAdjustValue[iNum] = "未知错误";
                        }
                        #endregion
                        #endregion==================================================================
                    }
                    else if (byt_RevFrame[int_Start] == 0x2F)
                    {
                        if (byt_RevFrame[int_Start + 5] == 0x5C)
                        {
                            #region 如果返回的 第 5bit 为 0x5C 则 取 23、24 和 25、26位进行判断

                            ////sRef = ((byt_RevFrame[int_Start + 23] - 0x30).ToString()) + ((byt_RevFrame[int_Start + 24] - 0x30).ToString());
                            ////sRefValue = ((byt_RevFrame[int_Start + 25] - 0x30).ToString()) + ((byt_RevFrame[int_Start + 26] - 0x30).ToString());


                            byte[] byt_APCU = new byte[0];
                            Array.Resize(ref byt_APCU, 2);    //数据域长度
                            //从 23 位开始取 APCU  80 
                            Array.Copy(byt_RevFrame, int_Start + 23, byt_APCU, 0, 2);
                            sRef = HexCon.ASCIIByteToString(byt_APCU);


                            byte[] byt_APRU = new byte[0];
                            Array.Resize(ref byt_APRU, 2);    //数据域长度
                            //从 25 位开始取 APRU 00 
                            Array.Copy(byt_RevFrame, int_Start + 25, byt_APRU, 0, 2);
                            sRefValue = HexCon.ASCIIByteToString(byt_APRU);

                            //if (sRef == "80" && sRefValue == "00")
                            if (sRef == "80")
                            {
                                cData = "";
                                #region 有问题的 处理方法
                                //////////"313330383030303033333256"
                                //////for (int i = 0; i < 24; i++)
                                //////{  
                                //////    //从 25 位开始取 生产编号 
                                //////    cData = cData + ((byt_RevFrame[int_Start + 25 + i] - 0x30).ToString());
                                //////}
                                //////if (cData.Length > 24)
                                //////{
                                //////    cData = cData.Substring(0, 24);
                                //////}
                                #endregion
                                #region 新的处理方法 2013-09-13 SunBoy 修改
                                byte[] byt_RevData = new byte[0];
                                Array.Resize(ref byt_RevData, 24);    //数据域长度

                                //从 25 位开始取 生产编号 
                                Array.Copy(byt_RevFrame, int_Start + 25, byt_RevData, 0, 24);

                                //////"313330393030303031554A31"
                                cData = HexCon.ASCIIByteToString(byt_RevData);

                                if (cData.Length > 24)
                                {
                                    cData = cData.Substring(0, 24);
                                }
                                #endregion
                                bResult = true;
                                //this.ToolBar_RuningInfo.Text = iNum+ " 返回数据 OK  ";
                            }
                            else
                            {
                                bResult = false;
                                //RefAdjustValue[iNum] = "返回值错误:" + sRef + " " + sRefValue;
                            }
                            #endregion
                        }
                        else
                        {
                            #region 如果返回的 第 5bit 不为 0x5C 则 取 21、22 和 23、24位进行判断
                            ////sRef = ((byt_RevFrame[int_Start + 21] - 0x30).ToString()) + ((byt_RevFrame[int_Start + 22] - 0x30).ToString());
                            ////sRefValue = ((byt_RevFrame[int_Start + 23] - 0x30).ToString()) + ((byt_RevFrame[int_Start + 24] - 0x30).ToString());

                            byte[] byt_APCU = new byte[0];
                            Array.Resize(ref byt_APCU, 2);    //数据域长度
                            //从 21 位开始取 APCU  80 
                            Array.Copy(byt_RevFrame, int_Start + 21, byt_APCU, 0, 2);
                            sRef = HexCon.ASCIIByteToString(byt_APCU);


                            byte[] byt_APRU = new byte[0];
                            Array.Resize(ref byt_APRU, 2);    //数据域长度
                            //从 23 位开始取 APRU 00 
                            Array.Copy(byt_RevFrame, int_Start + 23, byt_APRU, 0, 2);
                            sRefValue = HexCon.ASCIIByteToString(byt_APRU);

                            if (sRef == "80")
                            {
                                //if (sRefValue == "00")
                                //{
                                cData = "";

                                #region 有问题的 处理方法
                                //////////"313330383030303033333256"
                                //////for (int i = 0; i < 24; i++)
                                //////{  
                                //////     //从 23 位开始取 生产编号 
                                //////    cData = cData + ((byt_RevFrame[int_Start + 23 + i] - 0x30).ToString());
                                //////}
                                //////if (cData.Length > 24)
                                //////{
                                //////    cData = cData.Substring(0, 24);
                                //////}
                                #endregion
                                #region 新的处理方法 2013-09-13 SunBoy 修改
                                byte[] byt_RevData = new byte[0];
                                Array.Resize(ref byt_RevData, 24);    //数据域长度
                                //从 23 位开始取 生产编号 
                                Array.Copy(byt_RevFrame, int_Start + 23, byt_RevData, 0, 24);

                                //////"313330393030303031554A31"
                                cData = HexCon.ASCIIByteToString(byt_RevData);

                                if (cData.Length > 24)
                                {
                                    cData = cData.Substring(0, 24);
                                }
                                #endregion
                                bResult = true;
                                //}
                            }
                            else if (sRef == "81")
                            {
                                if (sRefValue == "01")
                                {
                                    bResult = false;
                                    ////LabelChkValue[iNum].Text = "0x01 校验和错误";
                                    ////RefAdjustValue[iNum] = "0x01 校验和错误";
                                }
                                else if (sRefValue == "02")
                                {
                                    bResult = false;
                                    ////LabelChkValue[iNum].Text = "0x02 长度出错";
                                    ////RefAdjustValue[iNum] = "0x02 长度出错";

                                }
                                else if (sRefValue == "03")
                                {
                                    bResult = false;
                                    ////LabelChkValue[iNum].Text = "0x03 APCU出错";
                                    ////RefAdjustValue[iNum] = "0x03 APCU出错";
                                }
                            }
                            else
                            {
                                bResult = false;
                                ////LabelChkValue[iNum].Text = "0x03 APCU出错";
                                ////RefAdjustValue[iNum] = "未知错误";
                            }
                            #endregion
                        }
                    }
                }
                #endregion

                return bResult;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }
        #endregion
        #region  MakeFrame_Measure_Calibrate 海外校表协议使用的---组帧通用框架---协议框架 IEC62056-21
        /// <summary>
        /// 校表协议使用的---组帧通用框架---协议框架IEC62056-21
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
            //int iTemp = 0;
            byte[] fpByte = new byte[10];
            byte[] bSend = new byte[10];
            ArrayList arlist = new ArrayList();
            fpByteList.Clear();
            try
            {
                #region IEC62056_21组帧  协议框架
                #region   0、IEC62056_21组帧  协议框架 组帧说明
                //发送帧：0x2F	0x3F	Addr	APCU	APDU	APKU	0x21	0x0D	0x0A
                //APKU为 APCU与APDU的校验和，长度为一个字节。
                //应答帧： 0x2F	Manufaturer	Baud_Rate	0x5C(可选)	Mode(可选)	Identfication	APCU	APRU	APKU	0x0D	0x0A
                //APKU为 APCU与APRU的校验和，长度为一个字节。
                #endregion
                #region 1、取 APCU 、ArrayList_APDU 值 计算并 计算出 APKU 校验码 放入 ArrayList arlist

                #region APCU-- OBISID 命令码 1字节
                sTemp = i_APCU.ToString("X2").PadLeft(2, '0').Substring(0, 2);
                for (int i = sTemp.Length / 2; i > 0; i--)
                {
                    arlist.Add(byte.Parse(sTemp.Substring(i * 2 - 2, 2), NumberStyles.HexNumber));
                }
                #endregion

                #region ArrayList_APDU 校验数据 放入 ArrayList arlist
                //==============================================================================================
                // 用户可以输入修改的 固定参数类
                for (int i = 0; i < ArrayList_APDU.Count; i++)
                {
                    arlist.Add(ArrayList_APDU[i]);
                }
                #endregion

                #region 计算 APKU 校验码 1字节

                // 校验码=APCU+APDU   
                iStartByte = 0;
                arlist.Add(Get_CheckSum(arlist, iStartByte));
                #endregion
                #endregion
                #region 2、将 MeterAddress 和 APCU 、APDU、APKU 放入 ArrayList ar 并 赋值 到  byte[] fpByte
                ArrayList ar = new ArrayList();
                #region MeterAddress 电表地址 2n字节
                for (int i = Addr.Length / 2; i > 0; i--)
                {
                    ar.Add(byte.Parse(Addr.Substring(i * 2 - 2, 2), NumberStyles.HexNumber));
                }
                for (int i = 0; i < arlist.Count; i++)
                {
                    ar.Add(arlist[i]);
                }
                fpByte = new byte[ar.Count];
                ar.CopyTo(fpByte, 0);
                #endregion
                //==============================================================================================
                #endregion
                #region  3、组成 一个 完整的 帧 数据
                bSend = new byte[2 + fpByte.Length * 2 + 3];

                bSend[0] = (byte)0x2F;
                bSend[1] = (byte)0x3F;

                j = 2;
                for (int i = 0; i < fpByte.Length; i++)
                {
                    sData = fpByte[i].ToString("X2");
                    bSend[j] = (byte)sData[0];
                    bSend[j + 1] = (byte)sData[1];
                    j = j + 2;
                }

                bSend[j] = (byte)0x21;
                bSend[j + 1] = (byte)0x0D;
                bSend[j + 2] = (byte)0x0A;
                #endregion
                #region 4、将完整的帧数据 赋值 到 fpByteList 返回
                fpByteList.AddRange(bSend);
                #endregion
                #endregion
                bResult = true;
                str_LostMessage = "OK";
                return bResult;

                #region 屏蔽不用的----MODBUS 组帧  协议框架
                /*
                #region  MODBUS 组帧  协议框架
                #region   0、MODBUS 组帧   协议框架 组帧说明
                //发送帧： Addr	Func	RegisterAddr 	RegisterNum 	RegisterLen	APCU	APDU	APKU	CRC
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
                #region 4、取数据长度  iRegisterLen
                int iRegisterLen = arlist.Count;
                #endregion
                #region  5、组成 一个 完整的 帧 数据 放入 bSend[]
                fpByte = new byte[arlist.Count];
                arlist.CopyTo(fpByte, 0);

                bSend = new byte[7 + fpByte.Length + 2]; //* 2

                // MODBUS广播地址-0x00 在发布的时候 需要 使用 string Addr  并取值 Addr 前一个字节 来赋值
                // 可以使用 通用的 系统参数配置 
                // 现在 这里 使用 MODBUS广播地址-0x00 在需要修改的 时候 可以进行适当的修改
                //     修改方法
                ////   sTemp = Addr.PadLeft(2, '0').Substring(0, 2);
                ////   bSend[0] = byte.Parse(sTemp, NumberStyles.HexNumber); 

                bSend[0] = (byte)0x00;                  // Addr :1Byte   MODBUS广播地址-0x00
                bSend[1] = (byte)0x10;                  // Func :1Byte   MODBUS写数据  -0x10

                //RegisterAddr 2Bytes; 0xFF,0xF0-须应答 0xFF,0xF1-无须应答
                bSend[2] = (byte)0xFF;
                bSend[3] = (byte)0xF0;

                //RegisterNum 2Bytes; 为RegisterLen/2; 传输时高字节在前，低字节在后;
                bSend[4] = (byte)(iRegisterLen / 2 / 256);
                bSend[5] = (byte)(iRegisterLen / 2 % 256);

                //RegisterLen 1Byte; 为APCU+APDU+APKU的数据长度，如该长度为奇数，则加1，补为偶数；
                bSend[6] = (byte)iRegisterLen;
                j = 7;
                for (int i = 0; i < fpByte.Length; i++)
                {
                    bSend[i + 7] = fpByte[i];
                    j++;
                }
                #endregion
                #region 6、添加 CRC 校验码  2字节 放入 bSend[]
                //CRC是从整个数据的第一个字节至CRC前一个字节

                CRC16 CRC16now = new CRC16();
                int ikuCRC = CRC16now.CalculateCrc16(bSend, bSend.Length - 2);
                sTemp = ikuCRC.ToString("X4").PadLeft(4, '0').Substring(0, 4);
                for (int i = 0; i < sTemp.Length / 2; i++)
                {
                    bSend[j] = byte.Parse(sTemp.Substring(i * 2, 2), NumberStyles.HexNumber);
                    j++;
                }
                #endregion
                #region 7、将完整的帧数据 赋值 到 fpByteList 返回
                fpByteList.AddRange(bSend);
                #endregion
                #endregion
                str_LostMessage = "OK";
                bResult = true;
                */
                #endregion
            }
            catch (Exception)
            {
                return bResult;
            }
        }
        #endregion
        #endregion

        /// <summary>
        /// 写命令(BYTE型，数据项)
        /// </summary>
        /// <param name="byt_Cmd">命令字,1个字节</param>
        /// <param name="int_Len">写入参数数据长度(块中每项字节数)</param>
        /// <param name="str_Value">写入数据</param>
        /// <returns></returns>
        public bool WriteData(byte byte_Cmd, int int_Len, string str_Value)
        {
            bool bE = false;
            string cData = "";
            str_Value = BackString(str_Value.PadLeft(2 * int_Len, '0'));
            if (m_int_PasswordType == 1)
            {
                int_Len += 8;
                cData = cData + m_byt_PasswordClass.ToString("X2") + BackString(m_str_Password.PadLeft(6, '0')) + BackString(m_str_UserCode.PadLeft(8, '0'));
                cData += str_Value;
            }
            else if (m_int_PasswordType == 0)
            {
                cData += str_Value;
            }
            cData = BackString(cData);
            bool bT = SendDLT645Command(byte_Cmd, m_str_Address, (byte)(int_Len), ref cData, ref bE);
            return bT;

        }


        /// <summary>
        /// 0x12请求读后续数据
        /// </summary>
        /// <param name="str_OBIS">标识符,4个字节</param>
        /// <param name="str_Value">返回数据，可带参数读取</param>
        /// <param name="int_SEQ">帧序号</param>
        /// <param name="bE">是否还有后续帧</param>
        /// <returns></returns>
        public bool ReadDataExtend(string str_OBIS, ref string str_Value,byte int_SEQ, ref bool bE)
        {
            try
            {
                string cDataStr = "";
                int iLen = 0;
                cDataStr =int_SEQ.ToString("X2")+ str_Value + str_OBIS;
                iLen = cDataStr.Length / 2;
                bool bT = SendDLT645Command(0x12, this.m_str_Address, (byte)iLen, ref cDataStr, ref bE);
                if (!bT)
                {
                    str_Value = "";
                    return false;
                }
                else
                {
                    str_Value = cDataStr.Substring(8);
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
        /// 读通信地址
        /// </summary>
        /// <param name="str_Value">返回表地址</param>
        /// <returns></returns>
        public bool ReadMeterAddress(ref string str_Value)
        {
            try
            {
                byte tmp_cmd = 0x13;
                string tmp_str_Address = "AAAAAAAAAAAA";
                byte tmp_byt_iLen = 0x0;
                string cDataStr = "";
                bool bE = false;
                bool bT = false;
                bT = SendDLT645Command(tmp_cmd, tmp_str_Address, tmp_byt_iLen, ref cDataStr, ref bE);
                if (!bT)
                {
                    str_Value = "";
                    return false;
                }
                else
                {
                    str_Value = BackString(cDataStr);
                    m_str_Address = str_Value;

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
        /// 写通信地址
        /// </summary>
        /// <param name="str_Value">写入表地址</param>
        /// <returns></returns>
        public bool WriteMeterAddress(string str_Value)
        {
            try
            {
                byte tmp_cmd = 0x15;
                string tmp_str_Address = "AAAAAAAAAAAA";
                byte tmp_byt_iLen = 0x06;
                string cDataStr = BackString(str_Value);
                bool bE = false;
                bool bT = false;
                bT = SendDLT645Command(tmp_cmd, tmp_str_Address, tmp_byt_iLen, ref cDataStr, ref bE);
                if(bT) m_str_Address =BackString( str_Value);

                return bT;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }




        #endregion

        #region 应用方法

        /// <summary>
        /// 读取数据（数据型，数据项）
        /// </summary>
        /// <param name="str_OBIS">标识符,4个字节</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="int_Dot">小数位</param>
        /// <param name="sng_Value">返回数据，不可带参数读取</param>
        /// <returns></returns>
        public bool ReadData(string str_OBIS, int int_Len, int int_Dot, ref Single sng_Value)
        {
            try
            {
                bool bE = false;
                string cDataStr = "";
                int iLen = 0;
                cDataStr = str_OBIS;
                iLen = cDataStr.Length / 2;
                bool bT = SendDLT645Command(0x11, this.m_str_Address, (byte)iLen, ref cDataStr, ref bE);
                if (!bT)
                {
                    sng_Value = 0f;
                    return false;
                }
                else
                {
                    cDataStr = cDataStr.Substring(8);
                    cDataStr = BackString(cDataStr);
                    sng_Value = (float)(Convert.ToSingle(cDataStr) / System.Math.Pow(10, int_Dot));
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
        /// 写数据(数据型，数据项)
        /// </summary>
        /// <param name="str_OBIS">标识符</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="int_Dot">小数位</param>
        /// <param name="sng_Value">写入数据</param>
        /// <returns></returns>
        public bool WriteData(string str_OBIS, int int_Len, int int_Dot, Single sng_Value)
        {
            bool bE = false;
            string cData = BackString(str_OBIS);
            string str_Value = (Convert.ToInt64((sng_Value * System.Math.Pow(10, int_Dot)))).ToString();
            str_Value = BackString(str_Value.PadLeft(2 * int_Len, '0'));
            if (m_int_PasswordType == 1)
            {
                int_Len += 8;
                cData = cData + m_byt_PasswordClass.ToString("X2") + BackString(m_str_Password.PadLeft(6, '0')) + BackString(m_str_UserCode.PadLeft(8, '0'));
                cData += str_Value;
            }
            else if (m_int_PasswordType == 0)
            {
                cData += str_Value;
            }
            cData = BackString(cData);
            bool bT = SendDLT645Command(0x14, m_str_Address, (byte)(int_Len + 4), ref cData, ref bE);
            return bT;

        }


        /// <summary>
        /// 读取生产编号，07暂不支持
        /// </summary>
        /// <param name="str_Value">返回生产编号</param>
        /// <returns></returns>
        public bool ReadScbh(ref string str_Value)
        {
            string str1="";
            string str2="";
            string str3="";
            IEC_ReadScbh("", ref str1, ref str2, ref str3);
            str_Value = str3;
            return false;
            

        }

        /// <summary>
        /// 读取电表底度
        /// </summary>
        /// <param name="str_Value">电表底度</param>
        /// <returns></returns>
        public bool ReadEnergy(ref string str_Value)
        {
            str_Value = "";
            try
            {
                float _fData = 0f;
                bool bT = ReadData("00010000", 4, 2, ref _fData);
                if (bT)
                {
                    str_Value = _fData.ToString();
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
        /// 读零线电流
        /// </summary>
        /// <param name="str_Value">零线电流(高位为符号位)</param>
        /// <returns></returns>
        public bool ReadLxDL(ref string str_Value)
        {
            str_Value = "";
            try
            {
                float _fData = 0f;
                bool bT = ReadData("02800001", 3, 3, ref _fData);
                if (bT)
                {
                    str_Value = _fData.ToString();
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
        /// 读取软件版本号
        /// </summary>
        /// <param name="str_Value">返回软件版本号</param>
        /// <returns></returns>
        public bool ReadVer(ref string str_Value)
        {
            try
            {
                bool bE = false;
                string str_OBIS = "04CC1100";
                int iLen = 32;
                string cDataStr ="";
                str_Value = "";
                bool bT = ReadData(str_OBIS, (byte)iLen, ref cDataStr);
                if (!bT)
                {
                    str_Value = "";
                    return false;
                }
                else
                {

                    string stmp = "";
                    System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                    for (int j = 0; j < cDataStr.Length / 2; j++)
                    {
                        byte[] byteArray = new byte[] { Convert.ToByte(cDataStr.Substring(2 * j, 2), 16) };
                        stmp += asciiEncoding.GetString(byteArray);
                    }

                    str_Value = stmp.Replace("\0", "");

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
        /// 初始化表参数
        /// </summary>
        /// <param name="str_Value">下发参数</param>
        /// <returns></returns>
        public bool InitMeterPara(string str_Value)
        {
            return false;
        }


        /// <summary>
        /// 系统清零
        /// </summary>
        /// <returns></returns>
        public bool SysClear()
        {
            int _WaitDataRevTime = this.WaitDataRevTime;
            this.WaitDataRevTime = 8000;
            try
            {
                bool bT = false;
                string str_Value = "";
                bT = ReadMeterAddress(ref str_Value);
                if (!bT) return false;
                return WriteData("04CC1000", 1, "55");
            }
            finally
            {
                this.WaitDataRevTime = _WaitDataRevTime;
            }
        }

        /// <summary>
        /// 设置高频脉冲放大倍数(用于误差高频检定)
        /// </summary>
        /// <param name="BS"></param>
        /// <returns></returns>
        public bool HighFrequencyPulse(int BS)
        {
            bool bT = false;
            string str_Value = "";
            bT = ReadMeterAddress(ref str_Value);
            if (!bT) return false;
            if (BS <= 1) BS = 1;
            //bT=WriteData("04CC0509", 1, BS.ToString("D2"));//放大常数5倍
            string str="";
            string str1 = "";
            string str2 = "0009";
            
            return bT;
        }

        /// <summary>
        /// 整机自检 07不支持　
        /// </summary>
        /// <param name="M1">Bit0;0=单板测试  1=整机测试;  Bit1 0=三相四 1=三相三 </param>
        /// <param name="M2">0：外控磁保持  1：内控磁保持   2：外控电保持</param>
        /// <param name="str_Value">整机测试结果，4个字节，每个bit位代表一个检测项目，包含32个，为1时代表有故障，0时代表合格</param>
        /// <returns></returns>
        public bool SelfCheck(int M1, int M2, ref string str_Value)
        {
            str_Value = "";
            return false;
        }

        /// <summary>
        /// 分相供电测试:读取三相电压数据块
        /// </summary>
        /// <param name="str_Value">返回三相电压数据块</param>
        /// <returns></returns>
        public bool ReadSinglePhaseTest(ref string str_Value)
        {
            try
            {
                string _strValue = "";
                float _fData = 0f;
                str_Value="";
                bool bT = ReadData("0201FF00", 0, ref _strValue);
                if (bT) 
                {
                    _strValue = BackString(_strValue);
                    for (int i = 0; i < 3; i++)
                    {
                        _fData =Convert.ToSingle ( Convert.ToDouble(BackString( _strValue.Substring(i * 4, 4) )) / Math.Pow(10,1)) ;

                        str_Value += _fData.ToString() + "|";

                    }


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
        /// 交流采样测试:读电压0201FF00，电流0202FF00，有功功率0203FF00数据块
        /// </summary>
        /// <param name="str_Value">返回电压，电流，有功功率数据块</param>
        /// <returns></returns>
        public bool ReadACSamplingTest(ref string str_Value)
        {
            
            try
            {
                string[] strCode = new string[] { "0201FF00", "0202FF00", "0203FF00" };
                int[] Len = new int[] { 2, 3, 3 };
                int[] Dot = new int[] { 1, 3, 4 };
                int[] Count = new int[] { 3, 3, 4 };//共10个数据
                string _strValue = "";
                float _fData = 0f;
                str_Value = "";
                bool bT =false ;
                for (int k = 0; k < strCode.Length; k++)
                {
                    bT = ReadData(strCode[k], 0, ref _strValue);
                    if (!bT) bT = ReadData(strCode[k], 0, ref _strValue);
                    if (!bT) bT = ReadData(strCode[k], 0, ref _strValue);

                    if (bT)
                    {
                        _strValue = BackString(_strValue);
                        for (int i = 0; i < Count[k]; i++)
                        {
                            string _strData = BackString(_strValue.Substring(i * Len[k] * 2, Len[k] * 2));

                            bool bRet = bIsNegativeString(ref _strData);
                            _fData = Convert.ToSingle(Convert.ToDouble(_strData) / Math.Pow(10, Dot[k]));
                            if (!bRet) _fData = -_fData;

                            if (k == 2 && i == 0)
                                continue;//去掉总有功功率
                            else
                                str_Value += _fData.ToString() + "|";
                        }
                    }
                    else break;
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
        /// 去掉符号位(最高位)
        /// </summary>
        /// <param name="sData">数据：Hex字符串</param>
        /// <returns></returns>
        private  bool bIsNegativeString(ref string sData)
        {
            int iTmp = Convert.ToInt32(sData.Substring(0, 2), 16);

            int iComRet = iTmp & (int)Math.Pow(2, 7);

            int iTmp2 = iTmp & (int)127;

            string sTmpsData = iTmp2.ToString("X2") + sData.Substring(2);

            sData = sTmpsData;

            if (iComRet == 0)//最高位为0，为正
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        #endregion

        #region  费控命令

        /// <summary>
        /// 费控写入(字符型，数据项)
        /// </summary>
        /// <param name="bCmd">控制命令</param>
        /// <param name="int_Len">写入数据长度</param>
        /// <param name="str_Value">写入数据，返回数据</param>
        /// <returns></returns>
        public bool WriteData(byte bCmd, byte int_Len, ref string str_Value)
        {

            bool bE = false;
            string cData = "";
            str_Value = BackString(str_Value.PadLeft(2 * int_Len, '0'));
            if (m_int_PasswordType == 1)
            {
                //int_Len += 8;
                //cData = m_byt_PasswordClass.ToString("X2") + BackString(m_str_Password.PadLeft(6, '0')) + BackString(m_str_UserCode.PadLeft(8, '0'));
                cData += str_Value;
            }
            else if (m_int_PasswordType == 0)
            {
                cData += str_Value;
            }
            cData = BackString(cData);
            bool bT = SendDLT645Command(0x03, m_str_Address, int_Len , ref cData, ref bE);
            if (bT)
            {
                if (cData.Length >= 8) 
                    str_Value = cData.Substring(0, cData.Length - 8);
            }
            else
            {
                str_Value = cData;
            }

            return bT;


        }

        /// <summary>
        /// 身份认证指令
        /// </summary>
        /// <param name="RandAndEndata">随机数1和密文1</param>
        /// <param name="Div">分散因子</param>
        /// <param name="Rand2">返回：随机数2</param>
        /// <param name="ESAMSerial">返回：ESAM序列号</param>
        /// <returns></returns>
        public bool Fk_IdentityAuthentication(string RandAndEndata, string Div, ref string Rand2, ref string ESAMSerial)
        {

            
            string cData1 = BackString("070000FF");
            string cData2 = BackString(m_str_UserCode.PadLeft(8, '0'));
            string cData3 = BackString(RandAndEndata) + BackString(Div);

            string sData = cData1 + cData2 + cData3;

            byte bDataLen = Convert.ToByte(sData.Length / 2);

            sData = BackString(sData);

            bool tb = WriteData(0x3, bDataLen, ref sData);
            if (tb)
            {
                if (sData.Length == 24)
                {
                    Rand2 = sData.Substring(16, 8);
                    ESAMSerial = sData.Substring(0, 16);
                }
                else
                {
                    MessageBox.Show("返回随机数2、ESAM序列号错误");
                    return false;
                }
            }
            return tb;

        }

        #region

        /// <summary>
        /// 身份认证有效,无效设置
        /// </summary>
        /// <param name="RandAndEndata">有效时长＋MAC,无效是为空</param>
        /// <returns></returns>
        public bool Fk_IdentityYxWx(string RandAndEndata, ref string Info)
        {

            
            string cData1 = "";
            if (RandAndEndata.Length == 12)//有效
                cData1 = BackString("070001FF");
            else
                cData1 = BackString("070002FF");
            string cData2 = BackString(m_str_UserCode.PadLeft(8, '0'));
            string cData3 = BackString(RandAndEndata);

            string sData = cData1 + cData2 + cData3;

            byte bDataLen = Convert.ToByte(sData.Length / 2);

            sData = BackString(sData);

            
            bool tb = WriteData(0x3,  bDataLen, ref sData);
            Info = sData;
            if (!tb)
            {
                MessageBox.Show("错误代码:" + sData);
            }
            return tb;

        }

        #endregion


        /// <summary>
        /// 开户函数
        /// </summary>
        /// <param name="sMoney">购电金额</param>
        /// <param name="Count">购电次数</param>
        /// <param name="MAC1">MAC1</param>
        /// <param name="UserSerial">客户编号</param>
        /// <param name="MAC2">MAC2</param>
        /// <returns></returns>
        public bool Fk_InCreasePurseFirst(float sMoney, int Count, string MAC1, string UserSerial, string MAC2)
        {

            
            string cData1 = BackString("070101FF");
            string cData2 = BackString(m_str_UserCode.PadLeft(8, '0'));
            string cData3 = BackString(Convert.ToInt32(sMoney * 100).ToString("X8"));
            cData3 += BackString(Count.ToString("X8"));
            cData3 += BackString(MAC1);
            cData3 += BackString(UserSerial);
            cData3 += BackString(MAC2);

            string sData = cData1 + cData2 + cData3;

            byte bDataLen = Convert.ToByte(sData.Length / 2);

            sData = BackString(sData);

            
            bool tb = WriteData(0x3,  bDataLen, ref  sData);
            if (!tb)
            {
                MessageBox.Show("错误代码:" + sData);
            }
            return tb;

        }

        /// <summary>
        /// 充值函数
        /// </summary>
        /// <param name="sMoney">购电金额</param>
        /// <param name="Count">购电次数</param>
        /// <param name="MAC1">MAC1</param>
        /// <param name="UserSerial">客户编号</param>
        /// <param name="MAC2">MAC2</param>
        /// <returns></returns>
        public bool Fk_InCreasePurse(float sMoney, int Count, string MAC1, string UserSerial, string MAC2)
        {

            
            string cData1 = BackString("070102FF");
            string cData2 = BackString(m_str_UserCode.PadLeft(8, '0'));
            string cData3 = BackString(Convert.ToInt32(sMoney * 100).ToString("X8"));
            cData3 += BackString(Count.ToString("X8"));
            cData3 += BackString(MAC1);
            cData3 += BackString(UserSerial);
            cData3 += BackString(MAC2);

            string sData = cData1 + cData2 + cData3;

            byte bDataLen = Convert.ToByte(sData.Length / 2);

            sData = BackString(sData);

            
            bool tb = WriteData(0x3,  bDataLen, ref sData);
            if (!tb)
            {
                MessageBox.Show("错误代码:" + sData);
            }
            return tb;

        }

        /// <summary>
        /// 开户函数
        /// </summary>
        /// <param name="sMoneyInfoAndMAC">带MAC的充值信息</param>
        /// <returns></returns>
        public bool Fk_InCreasePurseFirst(string sMoneyInfoAndMAC)
        {

            
            string cData1 = BackString("070101FF");
            string cData2 = BackString(m_str_UserCode.PadLeft(8, '0'));
            string cData3 = BackString(sMoneyInfoAndMAC);

            string sData = cData1 + cData2 + cData3;

            byte bDataLen = Convert.ToByte(sData.Length / 2);

            sData = BackString(sData);

            
            bool tb = WriteData(0x3,  bDataLen, ref  sData);
            if (!tb)
            {
                MessageBox.Show("错误代码:" + sData);
            }
            return tb;

        }
        /// <summary>
        /// 充值函数
        /// </summary>
        /// <param name="sMoneyInfoAndMAC">带MAC的充值信息</param>
        /// <returns></returns>
        public bool Fk_InCreasePurse(string sMoneyInfoAndMAC)
        {

            
            string cData1 = BackString("070102FF");
            string cData2 = BackString(m_str_UserCode.PadLeft(8, '0'));
            string cData3 = BackString(sMoneyInfoAndMAC);

            string sData = cData1 + cData2 + cData3;

            byte bDataLen = Convert.ToByte(sData.Length / 2);

            sData = BackString(sData);

            
            bool tb = WriteData(0x3,  bDataLen, ref  sData);
            if (!tb)
            {
                MessageBox.Show("错误代码:" + sData);
            }
            return tb;

        }


        #region  密钥更新函数:控制密钥，参数密钥，远程身份认证密钥

        /// <summary>
        /// 控制命令密钥更新函数070201FF
        /// </summary>
        /// <param name="KeyMAC">密钥信息+MAC 8byte</param>
        /// <param name="DivEsamNumRandData">控制命令文件线路保护密钥 32byte</param>
        /// <returns></returns>
        public bool Fk_ControlKeyUpdate(string KeyMACAndDivEsamNumRandData)
        {

            
            string cData1 = BackString("070201FF");
            string cData2 = BackString(m_str_UserCode.PadLeft(8, '0'));
            string cData3 = BackString(KeyMACAndDivEsamNumRandData);

            string sData = cData1 + cData2 + cData3;

            byte bDataLen = Convert.ToByte(sData.Length / 2);

            sData = BackString(sData);

            
            bool tb = WriteData(0x3,  bDataLen, ref  sData);
            if (!tb)
            {
                MessageBox.Show("错误代码:" + sData);
            }
            return tb;

        }


        /// <summary>
        /// 参数密钥更新函数070202FF
        /// </summary>
        /// <param name="KeyMAC">密钥信息+MAC 8byte</param>
        /// <param name="DivEsamNumRandData">控制命令文件线路保护密钥 32byte</param>
        /// <returns></returns>
        public bool Fk_ParameterKeyUpdate(string KeyMACAndDivEsamNumRandData)
        {

            
            string cData1 = BackString("070202FF");
            string cData2 = BackString(m_str_UserCode.PadLeft(8, '0'));
            string cData3 = BackString(KeyMACAndDivEsamNumRandData);

            string sData = cData1 + cData2 + cData3;

            byte bDataLen = Convert.ToByte(sData.Length / 2);

            sData = BackString(sData);

            
            bool tb = WriteData(0x3,  bDataLen, ref  sData);
            if (!tb)
            {
                MessageBox.Show("错误代码:" + sData);
            }
            return tb;

        }


        /// <summary>
        /// 远程序身份认证密钥更新函数070203FF
        /// </summary>
        /// <param name="KeyMAC">密钥信息+MAC 8byte</param>
        /// <param name="DivEsamNumRandData">控制命令文件线路保护密钥 32byte</param>
        /// <returns></returns>
        public bool Fk_UserControlKeyUpdate(string KeyMACAndDivEsamNumRandData)
        {

            
            string cData1 = BackString("070203FF");
            string cData2 = BackString(m_str_UserCode.PadLeft(8, '0'));
            string cData3 = BackString(KeyMACAndDivEsamNumRandData);

            string sData = cData1 + cData2 + cData3;

            byte bDataLen = Convert.ToByte(sData.Length / 2);

            sData = BackString(sData);

            
            bool tb = WriteData(0x3,  bDataLen, ref sData);
            if (!tb)
            {
                MessageBox.Show("错误代码:" + sData);
            }
            return tb;

        }

        /// <summary>
        /// 远程主控密钥更新函数070204FF
        /// </summary>
        /// <param name="KeyMAC">密钥信息+MAC 8byte</param>
        /// <param name="DivEsamNumRandData">控制命令文件线路保护密钥 32byte</param>
        /// <returns></returns>
        public bool Fk_MainControlKeyUpdate(string KeyMACAndDivEsamNumRandData)
        {

            
            string cData1 = BackString("070204FF");
            string cData2 = BackString(m_str_UserCode.PadLeft(8, '0'));
            string cData3 = BackString(KeyMACAndDivEsamNumRandData);

            string sData = cData1 + cData2 + cData3;

            byte bDataLen = Convert.ToByte(sData.Length / 2);

            sData = BackString(sData);

            
            bool tb = WriteData(0x3,  bDataLen, ref sData);
            if (!tb)
            {
                MessageBox.Show("错误代码:" + sData);
            }
            return tb;

        }


        #endregion

        /// <summary>
        /// 数据回抄(无后续帧)
        /// </summary>
        /// <param name="sReturnCode">数据回抄标识</param>
        /// <param name="sReturnData">返回：回抄的数据</param>
        /// <returns></returns>
        public bool Fk_ReturnData(string sReturnCode, ref string sReturnData)
        {

            
            string cData1 = BackString("078001FF");
            if (sReturnCode == "") cData1 = BackString("078102FF");//状态查询
            string cData2 = BackString(m_str_UserCode.PadLeft(8, '0'));
            string cData3 = BackString(sReturnCode);

            string sData = cData1 + cData2 + cData3;

            byte bDataLen = Convert.ToByte(sData.Length / 2);

            sData = BackString(sData);

            
            bool tb = WriteData(0x3,  bDataLen, ref sData);
            if (tb)
            {
                sReturnData = sData;
            }
            return tb;

        }

        /// <summary>
        /// 数据回抄(有后续帧)
        /// </summary>
        /// <param name="sReturnCode">数据回抄标识</param>
        /// <param name="sReturnData">返回：回抄的数据</param>
        /// <param name="bExtend">返回：是否有后续帧</param>
        /// <returns></returns>
        public bool Fk_ReturnData(string sReturnCode, ref string sReturnData, ref  bool bExtend)
        {

            
            string cData1 = BackString("078001FF");
            if (sReturnCode == "") cData1 = BackString("078102FF");//状态查询
            string cData2 = BackString(m_str_UserCode.PadLeft(8, '0'));
            string cData3 = BackString(sReturnCode);

            string sData = cData1 + cData2 + cData3;

            byte bDataLen = Convert.ToByte(sData.Length / 2);

            sData = BackString(sData);

            
            bool tb = WriteData(0x3,  bDataLen, ref sData);
            if (tb)
            {
                sReturnData = sData;
            }
            return tb;

        }

        /// <summary>
        /// 远程控制函数
        /// </summary>
        /// <param name="RandDivEsamNumData"></param>
        /// <returns></returns>
        public bool Fk_UserControl(string RandDivEsamNumData)
        {

            
            string cData1 = "98" + m_str_Password.PadLeft(6, '0').Trim();
            string cData2 = BackString(m_str_UserCode.PadLeft(8, '0'));
            string cData3 = BackString(RandDivEsamNumData);

            string sData = cData1 + cData2 + cData3;

            byte bDataLen = Convert.ToByte(sData.Length / 2);

            sData = BackString(sData);

            
            bool tb = WriteData(0x1C,  bDataLen, ref  sData);
            if (!tb)
            {
                MessageBox.Show("错误代码:" + sData);
            }
            return tb;

        }

        /// <summary>
        /// 费控写参数
        /// </summary>
        /// <param name="sFlag">标识编码</param>
        /// <param name="sData">数据:明文＋MAC；密文＋MAC；明文</param>
        /// <param name="sClass">密码等级，99为明文数据＋MAC；98为密文数据＋MAC；　02　or　04为明文</param>
        /// <returns></returns>
        public bool Fk_WriteData_Str(string sFlag, string sData, string sClass)
        {
            /// cFlag 标识编码  b_DataLen 数据长度   b_Dot 小数点位数 sData 设置的数据
            
            

            string cData = "";
            cData = sFlag.Substring(6, 2) + sFlag.Substring(4, 2) + sFlag.Substring(2, 2) + sFlag.Substring(0, 2);
            cData += sClass.PadLeft(2, '0') + m_str_Password.PadLeft(6, '0').Trim() + BackString(m_str_UserCode.PadLeft(8, '0'));
            cData += sData;

            byte bDataLen = 0;
            bDataLen = Convert.ToByte(sData.Length / 2 + 12);

            cData = BackString(cData);
            bool bT = WriteData(0x14, (byte)(bDataLen), ref cData);
            return bT;

        }

        #endregion

        
 
        #endregion



    }

}
