/****************************************************************************

    DL/T645-1997Э��
    ��ΰ 2009-10

*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using pwInterface;
using System.Collections;
using System.Globalization;
namespace pwMeterProtocol
{
    public class DLT645_1997 : DLT645,IMeterProtocol
    {

        private string m_str_Address = "AAAAAAAAAAAA";        ///���ַ
        private string m_str_Password = "000000";             ///������
        private byte m_byt_PasswordClass = 0;                 ///������ȼ�
        private string m_str_UserCode = "000000";             ///����Ա����
        private int m_int_PasswordType = 1;                   ///������֤���ͣ�0����������֤ 1����������������з�ʽ
        private bool m_bol_DataFieldPassword = false;         ///д����ʱ���������Ƿ����д����,true=Ҫ��false=����

        public DLT645_1997()
        {
            this.m_byt_RevData = new byte[0];
        }

        #region DLT645_1997��Ա

        #region ����
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

        #endregion

        #region ��������

        /// <summary>
        /// ��ȡ���ݣ��ַ��ͣ������
        /// </summary>
        /// <param name="str_OBIS">��ʶ��,2���ֽ�</param>
        /// <param name="int_Len">���ݳ���(�ֽ���)</param>
        /// <param name="str_Value">��������</param>
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
                bool bT = SendDLT645Command(0x01, this.m_str_Address, (byte)iLen, ref cDataStr, ref bE);
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
        /// ��ȡ���ݣ������ͣ������
        /// </summary>
        /// <param name="str_OBIS">��ʶ��,2���ֽ�</param>
        /// <param name="int_Len">���ݳ���(�ֽ���)</param>
        /// <param name="int_Dot">С��λ</param>
        /// <param name="sng_Value">��������</param>
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
                bool bT = SendDLT645Command(0x01, this.m_str_Address, (byte)iLen, ref cDataStr, ref bE);
                if (!bT)
                {
                    sng_Value = 0f;
                    return false;
                }
                else
                {
                    cDataStr = cDataStr.Substring(4);
                    sng_Value = (float)(double.Parse(cDataStr) / System.Math.Pow(10, int_Dot));
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
        /// д����(�ַ��ͣ�������)
        /// </summary>
        /// <param name="str_OBIS">��ʶ��,�����ֽ�</param>
        /// <param name="int_Len">���ݳ���(����ÿ���ֽ���)</param>
        /// <param name="str_Value">д������</param>
        /// <returns></returns>
        public bool WriteData(string str_OBIS, int int_Len, string str_Value)
        {
            bool bE = false;
            string cData = BackString(str_OBIS);
            str_Value = BackString(str_Value.PadLeft(2 * int_Len, '0'));

            if (m_int_PasswordType == 1)
            {
                int_Len += 4;
                cData = cData + m_byt_PasswordClass.ToString("X2") + BackString(m_str_Password.PadLeft(6, '0'));
                cData += str_Value;
            }
            else if (m_int_PasswordType == 0)
            {
                cData += str_Value;
            }

            cData = BackString(cData);
            bool bT = SendDLT645Command(0x4, m_str_Address, (byte)(int_Len + 2), ref cData, ref bE);
            return bT;

        }
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
                int_Len += 4;
                cData = cData + m_byt_PasswordClass.ToString("X2") + BackString(m_str_Password.PadLeft(6, '0')) ;
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
                int_Len += 4;
                cData = cData + m_byt_PasswordClass.ToString("X2") + BackString(m_str_Password.PadLeft(6, '0'));
                cData += str_Value;
            }
            else if (m_int_PasswordType == 0)
            {
                cData += str_Value;
            }
            cData = BackString(cData);
            bool bT = SendDLT645Command(0x4, m_str_Address, (byte)(int_Len + 2), ref cData, ref bE);
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
        public bool IEC_ReadScbh( string UseSizeCommSetting, ref string sTxFrame, ref string sRxFrame, ref string sRxData)
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
                //string tmp_str_Address = "AAAAAAAAAAAA";
                bln_Result = MakeFrame_Measure_Calibrate("AAAAAAAA", 0x60, ArrayList_APDU, ref tmpByteList, ref m_str_LostMessage);

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
                        bln_Result = this.SendFrame(byt_SendData, 4000, 2000);
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


        #endregion

        #region Ӧ�÷���
        /// <summary>
        /// ��ȡ�������
        /// </summary>
        /// <param name="str_Value">�����������</param>
        /// <returns></returns>
        public bool ReadScbh(ref string str_Value)
        {
            try
            {
                if (str_Value == "212121")
                { 
                    string str1="";
                     string str2="";
                     string str3="";
                    bool revbool=  IEC_ReadScbh("", ref str1, ref str2, ref str3);
                   
                     str_Value = str3;
                     return revbool; 
                }
                else
                {

                    //"68AAAAAAAAAAAA6803092332333333339333935216"
                    string tmp_str_Address = "AAAAAAAAAAAA";
                    string cDataStr = "";
                    int iLen = 0;
                    //cDataStr = "FFF9";
                    cDataStr = str_Value;// "233233333333933393";
                    iLen = cDataStr.Length / 2;
                    bool bE = true;
                    //bool bT = SendDLT645Command(0x01, tmp_str_Address, (byte)iLen, ref cDataStr, ref bE);
                    bool bT = SendDLT645CommandForSCBH(0x03, tmp_str_Address, (byte)iLen, ref cDataStr, ref bE);

                    #region 2013-06-13 SunBoy ����޸� ASCII ����ת��
                    string stmp = "";
                    System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                    for (int j = 0; j < cDataStr.Length / 2; j++)
                    {

                        byte[] byteArray = new byte[] { Convert.ToByte(cDataStr.Substring(2 * j, 2), 16) };
                        stmp += asciiEncoding.GetString(byteArray);

                    }
                    cDataStr = stmp.Replace("\0", "");
                    #endregion
                    if (!bT)
                    {
                        str_Value = "";
                        return false;
                    }
                    else
                    {
                        str_Value = cDataStr;//.Substring(5);
                        //str_Value = BackString(str_Value); 
                        return bT;
                    }
                }
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }


        /// <summary>
        /// �����Լ�
        /// </summary>
        /// <param name="M1">Bit0;0=�������  1=��������;  Bit1 0=������ 1=������ </param>
        /// <param name="M2">0����شű���  1���ڿشű���   2����ص籣��</param>
        /// <param name="str_Value">�������Խ����4���ֽڣ�ÿ��bitλ����һ�������Ŀ������32����Ϊ1ʱ�����й��ϣ�0ʱ����ϸ�</param>
        /// <returns></returns>
        public bool SelfCheck(int  M1,int  M2,ref string str_Value)
        {
            try
            {
                string tmp_str_Address = "AAAAAAAAAAAA";
                bool bE = false;
                int int_Len = 16;
                int intT = 6;//����Բ�ʱ�䣬hex��ʽ����λ��
                string cData = "F8FF003D7B5A";
                cData += M1.ToString("X2");
                cData += M2.ToString("X2");
                cData += intT.ToString("X2");
                cData += DateTime.Now.Second.ToString("X2");
                cData += DateTime.Now.Minute.ToString("X2");
                cData += DateTime.Now.Hour.ToString("X2");
                cData += DateTime.Now.Day.ToString("X2");
                cData += DateTime.Now.Month.ToString("X2");

                string _stryear = DateTime.Now.Year.ToString().Substring(2);
                cData += Convert.ToInt32(_stryear).ToString("X2");
                cData += Convert.ToInt32((int) DateTime.Now.DayOfWeek).ToString("X2");
                
                cData = BackString(cData);

                bool bT = SendDLT645Command(0x4, tmp_str_Address, (byte)(int_Len), ref cData, ref bE);

                if (!bT)
                {
                    str_Value = "";
                    return false;
                }
                else
                {
                    str_Value = BackString( cData); 
                    return bT;
                }
            }
            catch
            {
                str_Value = "";
                return false;
            }
        }


        /// <summary>
        /// ��ȡ������
        /// </summary>
        /// <param name="str_Value">���ص����������й���</param>
        /// <returns></returns>
        public bool ReadEnergy(ref string str_Value)
        {
            string tmp_str_Address = this.Address;
            try
            {
                this.Address="AAAAAAAAAAAA";
                float sng_Value = 0f;
                bool bT = ReadData("9010", 4, 2, ref sng_Value);
                str_Value = sng_Value.ToString();
                return bT;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
            finally
            {
                this.Address = tmp_str_Address;
            }

        }


        /// <summary>
        /// ��ȡ����汾�� 07�ݲ�֧��
        /// </summary>
        /// <param name="str_Value">��������汾��</param>
        /// <returns></returns>
        public bool ReadVer(ref string str_Value)
        {
            str_Value = "";
            return false;

        }

        /// <summary>
        /// ��ʼ������
        /// </summary>
        /// <param name="str_Value">�·�����</param>
        /// <returns></returns>
        public bool InitMeterPara(string str_Value)
        {
            bool bT = WriteData("FFFD", 0, str_Value);
            return bT;
        }

        /// <summary>
        /// ϵͳ����
        /// </summary>
        /// <returns></returns>
        public bool SysClear()
        {
            return false;
            //return WriteData("F830", 0,"00");

            //bool bE = false;
            //string cData = "";
            //return this.SendDLT645Command(Convert.ToByte("0F", 16), m_str_Address, 8, ref  cData,ref bE );
        }

        /// <summary>
        /// ���ø�Ƶ����Ŵ���(��������Ƶ�춨)
        /// </summary>
        /// <param name="BS"></param>
        /// <returns></returns>
        public bool HighFrequencyPulse(int BS)
        {
            return false;
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
                byte tmp_cmd = 0x06;
                string tmp_str_Address = "999999999999";
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
                    str_Value = cDataStr.Substring(4);
                    m_str_Address = BackString(str_Value);
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
                byte tmp_cmd = 0x0A;
                string tmp_str_Address = "999999999999";
                byte tmp_byt_iLen = 0x06;
                string cDataStr = BackString(str_Value);
                bool bE = false;
                bool bT = false;
                bT = SendDLT645Command(tmp_cmd, tmp_str_Address, tmp_byt_iLen, ref cDataStr, ref bE);
                if (bT) m_str_Address = str_Value;
                return bT;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        public bool ReadSinglePhaseTest(ref string str_Value)
        {
            try
            {
                if (str_Value == "212121")
                {
                    string str1 = "";
                    string str2 = "";
                    string str3 = "";                    
                    bool revbool = IEC_ReadScbh("", ref str1, ref str2, ref str3);                   
                    str_Value = str3;
                    return revbool;
                }
                else
                {

                    //"68AAAAAAAAAAAA6803092332333333339333935216"
                    string tmp_str_Address = "AAAAAAAAAAAA";
                    string cDataStr = "";
                    int iLen = 0;
                    //cDataStr = "FFF9";
                    cDataStr = str_Value;// "233233333333933393";
                    iLen = cDataStr.Length / 2;
                    bool bE = true;
                    //bool bT = SendDLT645Command(0x01, tmp_str_Address, (byte)iLen, ref cDataStr, ref bE);
                    bool bT = SendDLT645CommandForSCBH(0x03, tmp_str_Address, (byte)iLen, ref cDataStr, ref bE);

                    #region 2013-06-13 SunBoy ����޸� ASCII ����ת��
                    string stmp = "";
                    System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                    for (int j = 0; j < cDataStr.Length / 2; j++)
                    {

                        byte[] byteArray = new byte[] { Convert.ToByte(cDataStr.Substring(2 * j, 2), 16) };
                        stmp += asciiEncoding.GetString(byteArray);

                    }
                    cDataStr = stmp.Replace("\0", "");
                    #endregion
                    if (!bT)
                    {
                        str_Value = "";
                        return false;
                    }
                    else
                    {
                        str_Value = cDataStr;//.Substring(5);
                        //str_Value = BackString(str_Value); 
                        return bT;
                    }
                }
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        public bool ReadACSamplingTest(ref string str_Value)
        {
            str_Value = "";
            return false;
        }



        #endregion

        #region ��д�ļ�
        /// <summary>
        /// ��ȡ�ļ�
        /// </summary>
        /// <param name="bFileCode">�ļ���</param>
        /// <param name="iOffset">ƫ����</param>
        /// <param name="iFileLength">��ȡ�ļ��ĳ���(����С��128�ֽ�)</param>
        /// <param name="sFile">��������</param>
        /// <returns></returns>
        public bool ReadData(byte bFileCode, int iOffset, int iFileLength, ref string sFile)
        {///��ȡ�ļ�   bFileCode �ļ����� iOffset ƫ���� iFileLength ��ȡ�ļ��ĳ���

            try
            {
                int MeterBufferSize = 128;		///B�ͱ��ļ��Ļ����С
                bool bE = false;
                string cDataStr = "";
                int iLen = 0;


                string cdata = "";
                //--------�ļ�����
                cdata = Convert.ToString(bFileCode, 16).PadLeft(2, '0').ToUpper();
                cdata += BackString(Convert.ToString(iOffset, 16).PadLeft(4, '0').ToUpper());
                cdata += Convert.ToString(iFileLength, 16).PadLeft(2, '0').ToUpper();


                cDataStr = BackString(cdata) + "F820";

                iLen = cDataStr.Length / 2;
                bool bT = SendDLT645Command(0x01, this.m_str_Address, (byte)iLen, ref cDataStr, ref bE);
                if (!bT)
                {
                    sFile = "";
                    return false;
                }
                else
                {
                    cDataStr = cDataStr.Substring(4);
                    sFile = cDataStr;
                    return bT;
                }
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
              

        }

        public bool WriteData(byte bFileCode, int iOffset, int iFileLength, string sFile)
        {
            //----------д�ļ�
            //---------- bFileCode �ļ����� iOffset ƫ���� iFileLength ��ȡ�ļ��ĳ���

            bool bE = false;
            string cData = "";
            string cDataStr = "";
            int int_Len;

            cDataStr = BackString("F821");
            cDataStr += Convert.ToString(bFileCode, 16).PadLeft(2, '0').ToUpper();
            cDataStr = BackString(Convert.ToString(iOffset, 16).PadLeft(4, '0').ToUpper());
            cDataStr = cDataStr + Convert.ToString(iFileLength, 16).PadLeft(2, '0').ToUpper();

            int_Len=cDataStr.Length / 2;

            int_Len += 4;
            cData = cDataStr + m_byt_PasswordClass.ToString("X2") + BackString(m_str_Password.PadLeft(6, '0'));
            cData += sFile;

            cData = BackString(cData);
            bool bT = SendDLT645Command(0x4, m_str_Address, (byte)(int_Len + 2), ref cData, ref bE);
            return bT;

        }


        #endregion


        #endregion






    }



}
