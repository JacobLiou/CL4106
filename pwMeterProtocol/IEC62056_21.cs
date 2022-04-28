/****************************************************************************

    DL/T645-2007Э��
    ��ΰ 2009-10

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


        private string m_str_Address = "000000000000";      ///���ַ
        private string m_str_Password = "000000";           ///������
        private byte m_byt_PasswordClass = 2;               ///������ȼ�
        private string m_str_UserCode = "00000000";         ///����Ա����
        private int m_int_PasswordType = 1;                 ///������֤���ͣ�0����������֤ 1����������������з�ʽ
        private bool m_bol_DataFieldPassword = false;       ///д����ʱ���������Ƿ����д����,true=Ҫ��false=����

        public IEC62056_21()
        {
            this.m_byt_RevData = new byte[0];
        }

        #region DLT645_2007��Ա


        #region ����
        /// <summary>
        /// ����ʧ����Ϣ
        /// </summary>
        public string LostMessage
        {
            get
            {
                return this.m_str_LostMessage;
            }
        }


        /// <summary>
        /// ���ַ
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
        /// д��������
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
        /// ����ȼ�
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
        /// ����Ա����
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
        /// ������֤���ͣ�0����������֤ 1����������������з�ʽ 2��A�ͱ�������֤��ʽ 3��B�ͱ�������֤��ʽ
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
        /// д����ʱ���������Ƿ����д����,true=Ҫ��false=����
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

        #region ��������
        /// <summary>
        /// 0x11��ȡ���ݣ��ַ��ͣ������
        /// </summary>
        /// <param name="str_OBIS">��ʶ��,4���ֽ�</param>
        /// <param name="int_Len">�������ݳ���(�ֽ���)</param>
        /// <param name="str_Value">�������ݣ��ɴ�������ȡ</param>
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
        /// 0x14д����(�ַ��ͣ�������)
        /// </summary>
        /// <param name="str_OBIS">��ʶ��,�����ֽ�</param>
        /// <param name="int_Len">д��������ݳ���(����ÿ���ֽ���)</param>
        /// <param name="str_Value">д������</param>
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
        #region ����--1��IEC���--��������� ReadScbh_IEC
        /// <summary>
        /// ����У��Э����֡ͨ��-- Э���� --  ��ȡ������� IEC_ReadScbh
        /// </summary>
        /// <param name="iBW">��λ</param>
        /// <param name="UseSizeCommSetting">ϵͳ���õĵ���������ͨѶ������</param>
        /// <param name="sTxFrame">����֡</param>
        /// <param name="sRxFrame">����֡</param>
        /// <param name="sRxData">��������</param>
        /// <returns></returns>
        /// 
        public bool IEC_ReadScbh(string UseSizeCommSetting, ref string sTxFrame, ref string sRxFrame, ref string sRxData)
        {
            ////8.1	���������
            ////����֡��
            ////APCUΪMeasure_CalibrateCmd_RdProductId(0x60)��
            ////APDUΪ0x00

            ////Ӧ��֡��
            ////��ȷʱAPCUΪMeasure_CalibrateCmd_Ok (0x80)��APRUΪ6���ֽڵ�������ţ����ֽ��ȴ���
            ////����ʱAPCUΪMeasure_CalibrateCmd_Err (0x81)��APRUΪ0x01~0x03��0x01����У��ͳ���,0x02�����ȳ��� ,0x03����APCU����


            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
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
                //������ʾ����֡
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
                            //������ʾ����֡
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

        #region ��鷵��֡�Ϸ��� CheckFrame_IEC_Read_SCBH
        /// <summary>
        /// ��鷵��֡�Ϸ���
        /// </summary>
        /// <param name="bWrap">����֡</param>
        /// <param name="cData">��������</param>
        /// <param name="bExtend">�Ƿ��к���֡</param>
        /// <returns></returns>
        public bool CheckFrame_IEC_Read_SCBH(byte[] byt_RevFrame, ref string cData, ref bool bExtend)
        {/// ��������  cWrap ��Ҫ�����İ� cData ���ص�����	bExtend �Ƿ��к�������
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
                    this.m_str_LostMessage = "û�з������ݣ�"; // Language.GetWord(SystemID.SYS_NoReturnData);// "û�з������ݣ�";
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


                #region ========= Sunboy 2013-05-07 �޸� ��ʱʹ�õ� Ӧ�� CL2018 �������������
                if (byt_RevFrame[int_Start] != 0x2F)
                {
                    int_Start = Array.IndexOf(byt_RevFrame, (byte)0x2D);
                }
                #endregion==================================================================


                if (int_Start < 0 || int_Start > byt_RevFrame.Length)// || int_Start + 4 > byt_RevFrame.Length) //û��0x2F��ͷ �����Ƿ��㹻һ֡ �Ƿ�����
                {
                    this.m_str_LostMessage = "����֡��������û��֡ͷ��"   //Language.GetWord(SystemID.SYS_Info_400179) //  "����֡��������û��֡ͷ��
                    + "[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }

                if (byt_RevFrame[byt_RevFrame.Length - int_Start - 2] != 0x0D &&
                    byt_RevFrame[byt_RevFrame.Length - int_Start - 1] != 0x0A)
                {
                    this.m_str_LostMessage = "����֡��������"  // Language.GetWord(SystemID.SYS_ReturnData_halfbaked) //"����֡��������
                        + "[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }

                #region �ж� ����ֵ�Ƿ���ȷ
                bool bResult = false;
                string sRefValue = "";
                string sRef = "";
                if (byt_RevFrame.Length >= 27)
                {
                    if (byt_RevFrame[int_Start] == 0x2D)
                    {
                        #region ========= Sunboy 2013-05-07 �޸� ��ʱʹ�õ� Ӧ�� CL2018 �������������
                        #region ������ص� �� 5bit ��Ϊ 0x5C �� ȡ 21��22 λ�����ж� �Ƿ�Ϊ 80  �� 23 λ��ʼȡ �������
                        ////sRef = ((byt_RevFrame[int_Start + 21] - 0x30).ToString()) + ((byt_RevFrame[int_Start + 22] - 0x30).ToString());
                        ////sRefValue = ((byt_RevFrame[int_Start + 23] - 0x30).ToString()) + ((byt_RevFrame[int_Start + 24] - 0x30).ToString());

                        byte[] byt_APCU = new byte[0];
                        Array.Resize(ref byt_APCU, 2);    //�����򳤶�
                        //�� 21 λ��ʼȡ APCU  80 
                        Array.Copy(byt_RevFrame, int_Start + 21, byt_APCU, 0, 2);
                        sRef = HexCon.ASCIIByteToString(byt_APCU);


                        byte[] byt_APRU = new byte[0];
                        Array.Resize(ref byt_APRU, 2);    //�����򳤶�
                        //�� 23 λ��ʼȡ APRU 00 
                        Array.Copy(byt_RevFrame, int_Start + 23, byt_APRU, 0, 2);
                        sRefValue = HexCon.ASCIIByteToString(byt_APRU);

                        if (sRef == "80")
                        {
                            cData = "";
                            ////"313330383030303033333256"
                            #region ������� ������
                            //////////"313330383030303033333256"
                            //////for (int i = 0; i < 24; i++)
                            //////{  
                            //////     //�� 23 λ��ʼȡ ������� 
                            //////    cData = cData + ((byt_RevFrame[int_Start + 23 + i] - 0x30).ToString());
                            //////}
                            //////if (cData.Length > 24)
                            //////{
                            //////    cData = cData.Substring(0, 24);
                            //////}
                            #endregion
                            #region �µĴ����� 2013-09-13 SunBoy �޸�
                            byte[] byt_RevData = new byte[0];
                            Array.Resize(ref byt_RevData, 24);    //�����򳤶�
                            //�� 23 λ��ʼȡ ������� 
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
                                ////LabelChkValue[iNum].Text = "0x01 У��ʹ���";
                                ////RefAdjustValue[iNum] = "0x01 У��ʹ���";
                            }
                            else if (sRefValue == "02")
                            {
                                bResult = false;
                                ////LabelChkValue[iNum].Text = "0x02 ���ȳ���";
                                ////RefAdjustValue[iNum] = "0x02 ���ȳ���";

                            }
                            else if (sRefValue == "03")
                            {
                                bResult = false;
                                ////LabelChkValue[iNum].Text = "0x03 APCU����";
                                ////RefAdjustValue[iNum] = "0x03 APCU����";
                            }
                        }
                        else
                        {
                            bResult = false;
                            ////LabelChkValue[iNum].Text = "0x03 APCU����";
                            ////RefAdjustValue[iNum] = "δ֪����";
                        }
                        #endregion
                        #endregion==================================================================
                    }
                    else if (byt_RevFrame[int_Start] == 0x2F)
                    {
                        if (byt_RevFrame[int_Start + 5] == 0x5C)
                        {
                            #region ������ص� �� 5bit Ϊ 0x5C �� ȡ 23��24 �� 25��26λ�����ж�

                            ////sRef = ((byt_RevFrame[int_Start + 23] - 0x30).ToString()) + ((byt_RevFrame[int_Start + 24] - 0x30).ToString());
                            ////sRefValue = ((byt_RevFrame[int_Start + 25] - 0x30).ToString()) + ((byt_RevFrame[int_Start + 26] - 0x30).ToString());


                            byte[] byt_APCU = new byte[0];
                            Array.Resize(ref byt_APCU, 2);    //�����򳤶�
                            //�� 23 λ��ʼȡ APCU  80 
                            Array.Copy(byt_RevFrame, int_Start + 23, byt_APCU, 0, 2);
                            sRef = HexCon.ASCIIByteToString(byt_APCU);


                            byte[] byt_APRU = new byte[0];
                            Array.Resize(ref byt_APRU, 2);    //�����򳤶�
                            //�� 25 λ��ʼȡ APRU 00 
                            Array.Copy(byt_RevFrame, int_Start + 25, byt_APRU, 0, 2);
                            sRefValue = HexCon.ASCIIByteToString(byt_APRU);

                            //if (sRef == "80" && sRefValue == "00")
                            if (sRef == "80")
                            {
                                cData = "";
                                #region ������� ������
                                //////////"313330383030303033333256"
                                //////for (int i = 0; i < 24; i++)
                                //////{  
                                //////    //�� 25 λ��ʼȡ ������� 
                                //////    cData = cData + ((byt_RevFrame[int_Start + 25 + i] - 0x30).ToString());
                                //////}
                                //////if (cData.Length > 24)
                                //////{
                                //////    cData = cData.Substring(0, 24);
                                //////}
                                #endregion
                                #region �µĴ����� 2013-09-13 SunBoy �޸�
                                byte[] byt_RevData = new byte[0];
                                Array.Resize(ref byt_RevData, 24);    //�����򳤶�

                                //�� 25 λ��ʼȡ ������� 
                                Array.Copy(byt_RevFrame, int_Start + 25, byt_RevData, 0, 24);

                                //////"313330393030303031554A31"
                                cData = HexCon.ASCIIByteToString(byt_RevData);

                                if (cData.Length > 24)
                                {
                                    cData = cData.Substring(0, 24);
                                }
                                #endregion
                                bResult = true;
                                //this.ToolBar_RuningInfo.Text = iNum+ " �������� OK  ";
                            }
                            else
                            {
                                bResult = false;
                                //RefAdjustValue[iNum] = "����ֵ����:" + sRef + " " + sRefValue;
                            }
                            #endregion
                        }
                        else
                        {
                            #region ������ص� �� 5bit ��Ϊ 0x5C �� ȡ 21��22 �� 23��24λ�����ж�
                            ////sRef = ((byt_RevFrame[int_Start + 21] - 0x30).ToString()) + ((byt_RevFrame[int_Start + 22] - 0x30).ToString());
                            ////sRefValue = ((byt_RevFrame[int_Start + 23] - 0x30).ToString()) + ((byt_RevFrame[int_Start + 24] - 0x30).ToString());

                            byte[] byt_APCU = new byte[0];
                            Array.Resize(ref byt_APCU, 2);    //�����򳤶�
                            //�� 21 λ��ʼȡ APCU  80 
                            Array.Copy(byt_RevFrame, int_Start + 21, byt_APCU, 0, 2);
                            sRef = HexCon.ASCIIByteToString(byt_APCU);


                            byte[] byt_APRU = new byte[0];
                            Array.Resize(ref byt_APRU, 2);    //�����򳤶�
                            //�� 23 λ��ʼȡ APRU 00 
                            Array.Copy(byt_RevFrame, int_Start + 23, byt_APRU, 0, 2);
                            sRefValue = HexCon.ASCIIByteToString(byt_APRU);

                            if (sRef == "80")
                            {
                                //if (sRefValue == "00")
                                //{
                                cData = "";

                                #region ������� ������
                                //////////"313330383030303033333256"
                                //////for (int i = 0; i < 24; i++)
                                //////{  
                                //////     //�� 23 λ��ʼȡ ������� 
                                //////    cData = cData + ((byt_RevFrame[int_Start + 23 + i] - 0x30).ToString());
                                //////}
                                //////if (cData.Length > 24)
                                //////{
                                //////    cData = cData.Substring(0, 24);
                                //////}
                                #endregion
                                #region �µĴ����� 2013-09-13 SunBoy �޸�
                                byte[] byt_RevData = new byte[0];
                                Array.Resize(ref byt_RevData, 24);    //�����򳤶�
                                //�� 23 λ��ʼȡ ������� 
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
                                    ////LabelChkValue[iNum].Text = "0x01 У��ʹ���";
                                    ////RefAdjustValue[iNum] = "0x01 У��ʹ���";
                                }
                                else if (sRefValue == "02")
                                {
                                    bResult = false;
                                    ////LabelChkValue[iNum].Text = "0x02 ���ȳ���";
                                    ////RefAdjustValue[iNum] = "0x02 ���ȳ���";

                                }
                                else if (sRefValue == "03")
                                {
                                    bResult = false;
                                    ////LabelChkValue[iNum].Text = "0x03 APCU����";
                                    ////RefAdjustValue[iNum] = "0x03 APCU����";
                                }
                            }
                            else
                            {
                                bResult = false;
                                ////LabelChkValue[iNum].Text = "0x03 APCU����";
                                ////RefAdjustValue[iNum] = "δ֪����";
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
        #region  MakeFrame_Measure_Calibrate ����У��Э��ʹ�õ�---��֡ͨ�ÿ��---Э���� IEC62056-21
        /// <summary>
        /// У��Э��ʹ�õ�---��֡ͨ�ÿ��---Э����IEC62056-21
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
                #region IEC62056_21��֡  Э����
                #region   0��IEC62056_21��֡  Э���� ��֡˵��
                //����֡��0x2F	0x3F	Addr	APCU	APDU	APKU	0x21	0x0D	0x0A
                //APKUΪ APCU��APDU��У��ͣ�����Ϊһ���ֽڡ�
                //Ӧ��֡�� 0x2F	Manufaturer	Baud_Rate	0x5C(��ѡ)	Mode(��ѡ)	Identfication	APCU	APRU	APKU	0x0D	0x0A
                //APKUΪ APCU��APRU��У��ͣ�����Ϊһ���ֽڡ�
                #endregion
                #region 1��ȡ APCU ��ArrayList_APDU ֵ ���㲢 ����� APKU У���� ���� ArrayList arlist

                #region APCU-- OBISID ������ 1�ֽ�
                sTemp = i_APCU.ToString("X2").PadLeft(2, '0').Substring(0, 2);
                for (int i = sTemp.Length / 2; i > 0; i--)
                {
                    arlist.Add(byte.Parse(sTemp.Substring(i * 2 - 2, 2), NumberStyles.HexNumber));
                }
                #endregion

                #region ArrayList_APDU У������ ���� ArrayList arlist
                //==============================================================================================
                // �û����������޸ĵ� �̶�������
                for (int i = 0; i < ArrayList_APDU.Count; i++)
                {
                    arlist.Add(ArrayList_APDU[i]);
                }
                #endregion

                #region ���� APKU У���� 1�ֽ�

                // У����=APCU+APDU   
                iStartByte = 0;
                arlist.Add(Get_CheckSum(arlist, iStartByte));
                #endregion
                #endregion
                #region 2���� MeterAddress �� APCU ��APDU��APKU ���� ArrayList ar �� ��ֵ ��  byte[] fpByte
                ArrayList ar = new ArrayList();
                #region MeterAddress ����ַ 2n�ֽ�
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
                #region  3����� һ�� ������ ֡ ����
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
                #region 4����������֡���� ��ֵ �� fpByteList ����
                fpByteList.AddRange(bSend);
                #endregion
                #endregion
                bResult = true;
                str_LostMessage = "OK";
                return bResult;

                #region ���β��õ�----MODBUS ��֡  Э����
                /*
                #region  MODBUS ��֡  Э����
                #region   0��MODBUS ��֡   Э���� ��֡˵��
                //����֡�� Addr	Func	RegisterAddr 	RegisterNum 	RegisterLen	APCU	APDU	APKU	CRC
                //APKUΪ APCU��APDU��У��ͣ�����Ϊһ���ֽڡ�
                //Ӧ��֡�� Addr	Func	RegisterAddr 	RegisterNum 	RegisterLen	APCU	APRU	APKU	CRC
                //APKUΪ APCU��APRU��У��ͣ�����Ϊһ���ֽڡ�
                #endregion
                #region 1��APCU-- OBISID ������ 1�ֽ� ���� ArrayList arlist
                sTemp = i_APCU.ToString("X2").PadLeft(2, '0').Substring(0, 2);
                for (int i = sTemp.Length / 2; i > 0; i--)
                {
                    arlist.Add(byte.Parse(sTemp.Substring(i * 2 - 2, 2), NumberStyles.HexNumber));
                }
                #endregion
                #region 2��APDU (ArrayList_APDU У������ ���� ArrayList arlist)

                #region APDU nBytes ��n+2Ϊ��������APKU��1���ֽ�0x00.
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
                #region 3��APKU У���� ( У����=APCU+APDU)
                iStartByte = 0;
                arlist.Add(Get_CheckSum(arlist, iStartByte));
                #endregion
                #region 4��ȡ���ݳ���  iRegisterLen
                int iRegisterLen = arlist.Count;
                #endregion
                #region  5����� һ�� ������ ֡ ���� ���� bSend[]
                fpByte = new byte[arlist.Count];
                arlist.CopyTo(fpByte, 0);

                bSend = new byte[7 + fpByte.Length + 2]; //* 2

                // MODBUS�㲥��ַ-0x00 �ڷ�����ʱ�� ��Ҫ ʹ�� string Addr  ��ȡֵ Addr ǰһ���ֽ� ����ֵ
                // ����ʹ�� ͨ�õ� ϵͳ�������� 
                // ���� ���� ʹ�� MODBUS�㲥��ַ-0x00 ����Ҫ�޸ĵ� ʱ�� ���Խ����ʵ����޸�
                //     �޸ķ���
                ////   sTemp = Addr.PadLeft(2, '0').Substring(0, 2);
                ////   bSend[0] = byte.Parse(sTemp, NumberStyles.HexNumber); 

                bSend[0] = (byte)0x00;                  // Addr :1Byte   MODBUS�㲥��ַ-0x00
                bSend[1] = (byte)0x10;                  // Func :1Byte   MODBUSд����  -0x10

                //RegisterAddr 2Bytes; 0xFF,0xF0-��Ӧ�� 0xFF,0xF1-����Ӧ��
                bSend[2] = (byte)0xFF;
                bSend[3] = (byte)0xF0;

                //RegisterNum 2Bytes; ΪRegisterLen/2; ����ʱ���ֽ���ǰ�����ֽ��ں�;
                bSend[4] = (byte)(iRegisterLen / 2 / 256);
                bSend[5] = (byte)(iRegisterLen / 2 % 256);

                //RegisterLen 1Byte; ΪAPCU+APDU+APKU�����ݳ��ȣ���ó���Ϊ���������1����Ϊż����
                bSend[6] = (byte)iRegisterLen;
                j = 7;
                for (int i = 0; i < fpByte.Length; i++)
                {
                    bSend[i + 7] = fpByte[i];
                    j++;
                }
                #endregion
                #region 6����� CRC У����  2�ֽ� ���� bSend[]
                //CRC�Ǵ��������ݵĵ�һ���ֽ���CRCǰһ���ֽ�

                CRC16 CRC16now = new CRC16();
                int ikuCRC = CRC16now.CalculateCrc16(bSend, bSend.Length - 2);
                sTemp = ikuCRC.ToString("X4").PadLeft(4, '0').Substring(0, 4);
                for (int i = 0; i < sTemp.Length / 2; i++)
                {
                    bSend[j] = byte.Parse(sTemp.Substring(i * 2, 2), NumberStyles.HexNumber);
                    j++;
                }
                #endregion
                #region 7����������֡���� ��ֵ �� fpByteList ����
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
        /// д����(BYTE�ͣ�������)
        /// </summary>
        /// <param name="byt_Cmd">������,1���ֽ�</param>
        /// <param name="int_Len">д��������ݳ���(����ÿ���ֽ���)</param>
        /// <param name="str_Value">д������</param>
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
        /// 0x12�������������
        /// </summary>
        /// <param name="str_OBIS">��ʶ��,4���ֽ�</param>
        /// <param name="str_Value">�������ݣ��ɴ�������ȡ</param>
        /// <param name="int_SEQ">֡���</param>
        /// <param name="bE">�Ƿ��к���֡</param>
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
        /// ��ͨ�ŵ�ַ
        /// </summary>
        /// <param name="str_Value">���ر��ַ</param>
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
        /// дͨ�ŵ�ַ
        /// </summary>
        /// <param name="str_Value">д����ַ</param>
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

        #region Ӧ�÷���

        /// <summary>
        /// ��ȡ���ݣ������ͣ������
        /// </summary>
        /// <param name="str_OBIS">��ʶ��,4���ֽ�</param>
        /// <param name="int_Len">���ݳ���(�ֽ���)</param>
        /// <param name="int_Dot">С��λ</param>
        /// <param name="sng_Value">�������ݣ����ɴ�������ȡ</param>
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
        /// д����(�����ͣ�������)
        /// </summary>
        /// <param name="str_OBIS">��ʶ��</param>
        /// <param name="int_Len">���ݳ���(�ֽ���)</param>
        /// <param name="int_Dot">С��λ</param>
        /// <param name="sng_Value">д������</param>
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
        /// ��ȡ������ţ�07�ݲ�֧��
        /// </summary>
        /// <param name="str_Value">�����������</param>
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
        /// ��ȡ���׶�
        /// </summary>
        /// <param name="str_Value">���׶�</param>
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
        /// �����ߵ���
        /// </summary>
        /// <param name="str_Value">���ߵ���(��λΪ����λ)</param>
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
        /// ��ȡ����汾��
        /// </summary>
        /// <param name="str_Value">��������汾��</param>
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
        /// ��ʼ�������
        /// </summary>
        /// <param name="str_Value">�·�����</param>
        /// <returns></returns>
        public bool InitMeterPara(string str_Value)
        {
            return false;
        }


        /// <summary>
        /// ϵͳ����
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
        /// ���ø�Ƶ����Ŵ���(��������Ƶ�춨)
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
            //bT=WriteData("04CC0509", 1, BS.ToString("D2"));//�Ŵ���5��
            string str="";
            string str1 = "";
            string str2 = "0009";
            
            return bT;
        }

        /// <summary>
        /// �����Լ� 07��֧�֡�
        /// </summary>
        /// <param name="M1">Bit0;0=�������  1=��������;  Bit1 0=������ 1=������ </param>
        /// <param name="M2">0����شű���  1���ڿشű���   2����ص籣��</param>
        /// <param name="str_Value">�������Խ����4���ֽڣ�ÿ��bitλ����һ�������Ŀ������32����Ϊ1ʱ�����й��ϣ�0ʱ����ϸ�</param>
        /// <returns></returns>
        public bool SelfCheck(int M1, int M2, ref string str_Value)
        {
            str_Value = "";
            return false;
        }

        /// <summary>
        /// ���๩�����:��ȡ�����ѹ���ݿ�
        /// </summary>
        /// <param name="str_Value">���������ѹ���ݿ�</param>
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
        /// ������������:����ѹ0201FF00������0202FF00���й�����0203FF00���ݿ�
        /// </summary>
        /// <param name="str_Value">���ص�ѹ���������й��������ݿ�</param>
        /// <returns></returns>
        public bool ReadACSamplingTest(ref string str_Value)
        {
            
            try
            {
                string[] strCode = new string[] { "0201FF00", "0202FF00", "0203FF00" };
                int[] Len = new int[] { 2, 3, 3 };
                int[] Dot = new int[] { 1, 3, 4 };
                int[] Count = new int[] { 3, 3, 4 };//��10������
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
                                continue;//ȥ�����й�����
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
        /// ȥ������λ(���λ)
        /// </summary>
        /// <param name="sData">���ݣ�Hex�ַ���</param>
        /// <returns></returns>
        private  bool bIsNegativeString(ref string sData)
        {
            int iTmp = Convert.ToInt32(sData.Substring(0, 2), 16);

            int iComRet = iTmp & (int)Math.Pow(2, 7);

            int iTmp2 = iTmp & (int)127;

            string sTmpsData = iTmp2.ToString("X2") + sData.Substring(2);

            sData = sTmpsData;

            if (iComRet == 0)//���λΪ0��Ϊ��
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        #endregion

        #region  �ѿ�����

        /// <summary>
        /// �ѿ�д��(�ַ��ͣ�������)
        /// </summary>
        /// <param name="bCmd">��������</param>
        /// <param name="int_Len">д�����ݳ���</param>
        /// <param name="str_Value">д�����ݣ���������</param>
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
        /// �����ָ֤��
        /// </summary>
        /// <param name="RandAndEndata">�����1������1</param>
        /// <param name="Div">��ɢ����</param>
        /// <param name="Rand2">���أ������2</param>
        /// <param name="ESAMSerial">���أ�ESAM���к�</param>
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
                    MessageBox.Show("���������2��ESAM���кŴ���");
                    return false;
                }
            }
            return tb;

        }

        #region

        /// <summary>
        /// �����֤��Ч,��Ч����
        /// </summary>
        /// <param name="RandAndEndata">��Чʱ����MAC,��Ч��Ϊ��</param>
        /// <returns></returns>
        public bool Fk_IdentityYxWx(string RandAndEndata, ref string Info)
        {

            
            string cData1 = "";
            if (RandAndEndata.Length == 12)//��Ч
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
                MessageBox.Show("�������:" + sData);
            }
            return tb;

        }

        #endregion


        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="sMoney">������</param>
        /// <param name="Count">�������</param>
        /// <param name="MAC1">MAC1</param>
        /// <param name="UserSerial">�ͻ����</param>
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
                MessageBox.Show("�������:" + sData);
            }
            return tb;

        }

        /// <summary>
        /// ��ֵ����
        /// </summary>
        /// <param name="sMoney">������</param>
        /// <param name="Count">�������</param>
        /// <param name="MAC1">MAC1</param>
        /// <param name="UserSerial">�ͻ����</param>
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
                MessageBox.Show("�������:" + sData);
            }
            return tb;

        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="sMoneyInfoAndMAC">��MAC�ĳ�ֵ��Ϣ</param>
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
                MessageBox.Show("�������:" + sData);
            }
            return tb;

        }
        /// <summary>
        /// ��ֵ����
        /// </summary>
        /// <param name="sMoneyInfoAndMAC">��MAC�ĳ�ֵ��Ϣ</param>
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
                MessageBox.Show("�������:" + sData);
            }
            return tb;

        }


        #region  ��Կ���º���:������Կ��������Կ��Զ�������֤��Կ

        /// <summary>
        /// ����������Կ���º���070201FF
        /// </summary>
        /// <param name="KeyMAC">��Կ��Ϣ+MAC 8byte</param>
        /// <param name="DivEsamNumRandData">���������ļ���·������Կ 32byte</param>
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
                MessageBox.Show("�������:" + sData);
            }
            return tb;

        }


        /// <summary>
        /// ������Կ���º���070202FF
        /// </summary>
        /// <param name="KeyMAC">��Կ��Ϣ+MAC 8byte</param>
        /// <param name="DivEsamNumRandData">���������ļ���·������Կ 32byte</param>
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
                MessageBox.Show("�������:" + sData);
            }
            return tb;

        }


        /// <summary>
        /// Զ���������֤��Կ���º���070203FF
        /// </summary>
        /// <param name="KeyMAC">��Կ��Ϣ+MAC 8byte</param>
        /// <param name="DivEsamNumRandData">���������ļ���·������Կ 32byte</param>
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
                MessageBox.Show("�������:" + sData);
            }
            return tb;

        }

        /// <summary>
        /// Զ��������Կ���º���070204FF
        /// </summary>
        /// <param name="KeyMAC">��Կ��Ϣ+MAC 8byte</param>
        /// <param name="DivEsamNumRandData">���������ļ���·������Կ 32byte</param>
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
                MessageBox.Show("�������:" + sData);
            }
            return tb;

        }


        #endregion

        /// <summary>
        /// ���ݻس�(�޺���֡)
        /// </summary>
        /// <param name="sReturnCode">���ݻس���ʶ</param>
        /// <param name="sReturnData">���أ��س�������</param>
        /// <returns></returns>
        public bool Fk_ReturnData(string sReturnCode, ref string sReturnData)
        {

            
            string cData1 = BackString("078001FF");
            if (sReturnCode == "") cData1 = BackString("078102FF");//״̬��ѯ
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
        /// ���ݻس�(�к���֡)
        /// </summary>
        /// <param name="sReturnCode">���ݻس���ʶ</param>
        /// <param name="sReturnData">���أ��س�������</param>
        /// <param name="bExtend">���أ��Ƿ��к���֡</param>
        /// <returns></returns>
        public bool Fk_ReturnData(string sReturnCode, ref string sReturnData, ref  bool bExtend)
        {

            
            string cData1 = BackString("078001FF");
            if (sReturnCode == "") cData1 = BackString("078102FF");//״̬��ѯ
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
        /// Զ�̿��ƺ���
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
                MessageBox.Show("�������:" + sData);
            }
            return tb;

        }

        /// <summary>
        /// �ѿ�д����
        /// </summary>
        /// <param name="sFlag">��ʶ����</param>
        /// <param name="sData">����:���ģ�MAC�����ģ�MAC������</param>
        /// <param name="sClass">����ȼ���99Ϊ�������ݣ�MAC��98Ϊ�������ݣ�MAC����02��or��04Ϊ����</param>
        /// <returns></returns>
        public bool Fk_WriteData_Str(string sFlag, string sData, string sClass)
        {
            /// cFlag ��ʶ����  b_DataLen ���ݳ���   b_Dot С����λ�� sData ���õ�����
            
            

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
