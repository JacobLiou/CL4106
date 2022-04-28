
/****************************************************************************

    DLT645Э����
    ��ΰ 2009-10

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
    /// �������ܱ�645Э����࣬����1997��2007�汾,���ɼ̳�����
    /// </summary>
    public class DLT645:ProtocolBase
    {
        private int m_int_WaitDataRevTime = 2000;           ///�ȴ����ݵ������ʱ��ms
        private int m_int_IntervalTime = 500;               ///���ݼ�����ʱ��ms
        private bool m_bol_ZendStringDel0x33 = false;		///���ͽ��յ����ݣ��������Ƿ��0x33
        private byte m_byt_iRepeatTimes = 3;                ///ͨѶʧ�����Դ���
        private bool m_bol_ClosComm = false;                ///ͨѶ��ɺ��Ƿ�رն˿�
        private bool m_bol_BreakDown = false;               ///���ⲿ�жϣ���Ҫ�������ش������ʱ
        private string m_str_RxID = "01";                   ///R�������Žڵ�ID�� 10H����ַ���ݱ�λ��1,2,3,4,5,6...

        public DLT645()
        {
            this.m_byt_RevData = new byte[0];
        }


        /// <summary>
        /// �ȴ����ݵ������ʱ��
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
        /// ���ݼ�����ʱ��
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
        /// ���ͽ��յ�����֡���������0x33
        /// </summary>
        public bool ZendStringDel0x33
        {
            set
            {
                this.m_bol_ZendStringDel0x33 = value;
            }
        }

        /// <summary>
        /// ͨѶʧ�����Դ���
        /// </summary>
        public byte iRepeatTimes
        {
            set
            {
                this.m_byt_iRepeatTimes = value;
            }
        }


        /// <summary>
        /// ͨѶ��ɺ��Ƿ�رն˿�
        /// </summary>
        public bool bClosComm
        {
            set
            {
                this.m_bol_ClosComm = value;
            }
        }

        /// <summary>
        /// ���ش������ʱ�Ƿ��ⲿ�ж�
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
        /// ������·
        /// </summary>
        /// <param name="bCmd">������</param>
        /// <param name="cAddr">���ַ</param>
        /// <param name="bDataLen">���ݳ���</param>
        /// <param name="cData">����������</param>
        /// <param name="bExtend">�Ƿ��к�������</param>
        /// <returns></returns>
        public bool SendDLT645CommandForSCBH(byte bCmd, string cAddr, byte bDataLen, ref string cData, ref bool bExtend)
        {	/// cCmd ������  cAddr ��ַ bLen ���ݳ��� cData ���������� bExtend �Ƿ��к�������
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                Stopwatch sth_SpaceTicker = new Stopwatch();                //
                sth_SpaceTicker.Start();

                byte[] byt_SendData = this.OrgFrame(bCmd, cAddr, bDataLen, cData);

                //������ʾ����֡
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
                            //������ʾ����֡
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
        /// ��鷵��֡�Ϸ���
        /// </summary>
        /// <param name="bWrap">����֡</param>
        /// <param name="cData">��������</param>
        /// <param name="bExtend">�Ƿ��к���֡</param>
        /// <returns></returns>
        public bool CheckFrame_IEC_SCBH(byte[] byt_RevFrame, ref string cData, ref bool bExtend)
        {/// ��������  cWrap ��Ҫ�����İ� cData ���ص�����	bExtend �Ƿ��к�������
            try
            {
                //" FE 68 AA AA AA AA AA AA 68 83 05 F0 FF 81 01 82 16"
                #region �ж��Ƿ��з�������
                if (byt_RevFrame.Length <= 0)
                {
                    this.m_str_LostMessage = "û�з������ݣ�";// "û�з������ݣ�";
                    return false;
                }
                #endregion
                #region ���ҵ�һ�� 0x68  ��int_Start  0x68 ���ֵ�λ�ã�
                int int_Start = 0; //0x68 ���ֵ�λ��
                int_Start = Array.IndexOf(byt_RevFrame, (byte)0x68);
                if (int_Start < 0 || int_Start > byt_RevFrame.Length || int_Start + 12 > byt_RevFrame.Length) //û��68��ͷ �����Ƿ��㹻һ֡ �Ƿ�����
                {
                    this.m_str_LostMessage ="����֡��������û��֡ͷ" //  "����֡��������û��֡ͷ��
                    + "[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }
                #endregion
                #region ���ҵڶ��� 0x68
                if (byt_RevFrame[int_Start + 7] != 0x68)        //�Ҳ����ڶ���68
                {
                    this.m_str_LostMessage = "����֡��������" //"����֡��������
                        + "[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }
                #endregion
                #region �жϷ������� �Ƿ�Ϊ 0x83-��ȫ��֤Ӧ��
                if (byt_RevFrame[int_Start + 8] != 0x83)        // 0x83-��ȫ��֤Ӧ��
                {
                    this.m_str_LostMessage ="����֡ ����0x83-��ȫ��֤Ӧ��" // "����֡ ����0x83-��ȫ��֤Ӧ��
                        + "[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }
                #endregion
                #region �ж����ݳ����Ƿ�ϸ�    int_Len
                int int_Len = byt_RevFrame[int_Start + 9];
                if (int_Start + 12 + int_Len != byt_RevFrame.Length)
                {
                    this.m_str_LostMessage = "���ݳ�����ʵ�ʳ��Ȳ�һ�£�" //"���ݳ�����ʵ�ʳ��Ȳ�һ�£�
                        + "[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;                //֡�ĳ����Ƿ���ʵ�ʳ���һ��
                }
                #endregion
                #region �жϷ���У�����Ƿ���ȷ
                byte byt_Chksum = 0;
                for (int int_Inc = int_Start; int_Inc < int_Start + int_Len + 10; int_Inc++)
                    byt_Chksum += byt_RevFrame[int_Inc];
                if (byt_RevFrame[int_Start + int_Len + 10] != byt_Chksum)       //У���벻��ȷ
                {
                    this.m_str_LostMessage ="����У���벻��ȷ��"// "����У���벻��ȷ��
                        + "[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }
                #endregion
                #region �жϷ���֡ �Ƿ�����
                if (byt_RevFrame[int_Start + int_Len + 11] != 0x16)       //û��16����
                {
                    this.m_str_LostMessage = "����֡��������" //"����֡��������
                        + "[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }
                #endregion

                string s_RxFrame = BytesToString(byt_RevFrame);

                //////��������ŷ���֡��
                //////" FE 68 AA AA AA AA AA AA 68 83 10 23 32 B3 36 33 33 33 33 33 33 33 33 33 33 33 B6 84 16"
                //////У����֡
                //////" FE 68 AA AA AA AA AA AA 68 83 05 23 32 B3 33 B3 42 16"
                ////byte[] byt_RevData = new byte[0];
                ////Array.Resize(ref byt_RevData, int_Len);    //�����򳤶�
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
                ////    int ilen = cData.Length; //����ʹ�� ���㷵������ ����
                ////}

                byte sRxOF = byt_RevFrame[int_Start + 10];
                byte sRxFF = byt_RevFrame[int_Start + 11];
                byte sRx80 = byt_RevFrame[int_Start + 12];
                if ((sRxOF -= 0x33) != 0x0F &&
                    (sRxFF -= 0x33) != 0xFF)
                {
                    this.m_str_LostMessage ="����֡�����Ϻ���У��Э�飬�� 0F FF " // "����֡�����Ϻ���У��Э�飬�� 0F FF 
                        + "��[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }
                //" FE 68 AA AA AA AA AA AA 68 83 05 23 32 B3 33 B3 42 16"
                if ((sRx80 -= 0x33) == 0x80)
                {
                    byte[] byt_RevData = new byte[0];
                    Array.Resize(ref byt_RevData, int_Len - 4);    //�����򳤶�
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
                        ////int ilen = cData.Length; //����ʹ�� ���㷵������ ����
                    }
                    //"313330333030303034354E4D"

                    return true;
                }
                else if ((sRx80 -= 0x33) == 0x81)
                {
                    byte[] byt_RevData = new byte[0];
                    Array.Resize(ref byt_RevData, int_Len);    //�����򳤶�
                    if (int_Len > 0)
                    {
                        Array.Copy(byt_RevFrame, int_Start + 10, byt_RevData, 0, int_Len);
                        ////Array.Copy(byt_RevFrame, int_Start + 10 + 3, byt_RevData, 0, int_Len - 4);
                    }

                    if (byt_RevData != null && byt_RevData.Length > 4)
                        this.m_str_LostMessage =  "[" + BitConverter.ToString(byt_RevFrame) + "]";
                    else
                        this.m_str_LostMessage = "���ز���ʧ��" // "���ز���ʧ��
                            + "��[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }
                else
                {
                    this.m_str_LostMessage =  "���ز���ʧ��" // "���ز���ʧ��
                          + "��[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        #region BytesToString1 �޿ո���ʾ��ʽ
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
        /// ������·
        /// </summary>
        /// <param name="bCmd">������</param>
        /// <param name="cAddr">���ַ</param>
        /// <param name="bDataLen">���ݳ���</param>
        /// <param name="cData">����������</param>
        /// <param name="bExtend">�Ƿ��к�������</param>
        /// <returns></returns>
        public bool SendDLT645Command(byte bCmd, string cAddr, byte bDataLen, ref string cData, ref bool bExtend)
        {	/// cCmd ������  cAddr ��ַ bLen ���ݳ��� cData ���������� bExtend �Ƿ��к�������
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                Stopwatch sth_SpaceTicker = new Stopwatch();                //
                sth_SpaceTicker.Start();

                byte[] byt_SendData = this.OrgFrame(bCmd, cAddr, bDataLen, cData);

                //������ʾ����֡
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
                            //������ʾ����֡
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
        /// ������·
        /// </summary>
        /// <param name="bCmd">������</param>
        /// <param name="cAddr">���ַ</param>
        /// <param name="bDataLen">���ݳ���</param>
        /// <param name="cData">����������</param>
        /// <param name="byt_RevDataF">����֡</param>
        /// <param name="bExtend">�Ƿ��к�������</param>
        /// <returns></returns>
        public bool SendDLT645Command(byte bCmd, string cAddr, byte bDataLen, ref string cData, ref byte[] byt_RevDataF, ref bool bExtend)
        {	/// cCmd ������  cAddr ��ַ bLen ���ݳ��� cData ���������� bExtend �Ƿ��к�������
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                Stopwatch sth_SpaceTicker = new Stopwatch();                //
                sth_SpaceTicker.Start();

                byte[] byt_SendData = this.OrgFrame(bCmd, cAddr, bDataLen, cData);

                //������ʾ����֡
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
                            //������ʾ����֡
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
        /// ��������֡�����ؽ�������֡���������ش�������ļ�
        /// </summary>
        /// <param name="RxFrame">����֡</param>
        /// <param name="TxFrame">����֡</param>
        /// <param name="cData">����������</param>
        /// <returns></returns>
        public bool SendDLT645RxFrame(string RxFrame, ref string TxFrame, ref string cData)
        {	/// RxFrame ����֡  TxFrame ����֡ cData ����������
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
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
        /// �����������
        /// </summary>
        /// <param name="_DownParaItemOne">����֡�б�</param>
        /// <param name="AllAddress">ͳһͨ�ŵ�ַ</param>
        /// <returns></returns>
        public bool DownPara(List<MeterDownParaItem> _DownParaItemOne)
        {

            try
            {
                //List<StDownParaItem> _DownParaItemOne = _DownParaItem;
                bool bRet = false;

                #region ȡ��������
                int i_DownParaSum = _DownParaItemOne.Count;
                #endregion

                #region ����������

                string sdata = "";
                string cdata = "";

                for (int i = 0; i < i_DownParaSum ; i++)
                {
                    //��Ҫ�����ⲿ�ж�
                    Thread.Sleep(1);//�ȴ�ʱ�䣬���˷�Ӵ
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
        /// ������
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
            this.m_str_LostMessage = "��Э�鲻����";
            flt_ActiveP = 0f;
            return false;
        }

        #region 
        /// <summary>
        /// ����У��Э��ʹ�õ�---��֡ͨ�ÿ��---Э����DLT645_1997
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
                #region  DLT645_1997 ��֡  Э����
                #region   0��DLT645_1997 ��֡   Э���� ��֡˵��
                //����֡�� 0x68	��ַ	0x68	������	���ݳ���	���ݱ�ʶ	�����ߴ���	APCU APDU 	APKU	У����	0x16
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
                #region 4��0x68	   ��ַ	  0x68	 ������	 ���ݳ���	���ݱ�ʶ	�����ߴ���	APCU 	APDU 	APKU  У����	0x16
                //         1�ֽ�  6�ֽ�   1�ֽ�  1�ֽ�   1�ֽ�      2�ֽ�       4�ֽ�                             1�ֽ�     1�ֽ�

                bSend = new byte[1 + 6 + 1 + 1 + 1 + 2 + 4 + arlist.Count + 1 + 1];

                #region  0x68 1�ֽ�
                bSend[0] = (byte)0x68;
                #endregion
                #region ����ַ 6�ֽ�-- MeterAddress  ����  byte[] bSend
                j = 1;
                Addr = Addr.PadLeft(12, '0').Substring(0, 12);
                for (int i = Addr.Length / 2; i > 0; i--)
                {
                    bSend[j] = byte.Parse(Addr.Substring(i * 2 - 2, 2), NumberStyles.HexNumber);
                    j++;
                }
                #endregion
                #region  0x68 1�ֽ�
                bSend[7] = (byte)0x68;
                #endregion
                #region  ������ 1�ֽ�--  0x03-��ȫ��֤
                bSend[8] = (byte)0x03;
                #endregion
                #region  ���ݳ��� 1�ֽ�-- ���������( ���ݱ�ʶ + �����ߴ���	+ APCU + APDU + APKU)
                bSend[9] = (byte)(2 + 4 + arlist.Count);
                #endregion

                #region ����������  +0x33

                #region  ���ݱ�ʶ 2�ֽ�--  0xF0, 0xFF-����У��
                iTemp = 0xF0 + 0x33;
                bSend[10] = (byte)iTemp;
                iTemp = 0xFF + 0x33;
                bSend[11] = (byte)iTemp;
                #endregion
                #region  �����ߴ��� 4�ֽ�--  �̶�Ϊ0x00,0x00,0x00,0x00
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

                #region У���� CS 1�ֽ�
                CRC8 CRC8 = new CRC8();
                byte CRC = CRC8.CRC(bSend);
                bSend[bSend.Length - 2] = CRC;
                #endregion
                #region ������ 16H 1�ֽ�
                bSend[bSend.Length - 1] = 0x16;
                #endregion


                #endregion DLT645_1997 ��֡  Э����
                #region 5����������֡���� ��ֵ �� fpByteList ����
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



        #region---˽��-------------------------



        /// <summary>
        /// ���ͽ��պ���
        /// </summary>
        /// <param name="byt_SendData">�������ݰ�</param>
        /// <returns>�������ݰ�</returns>
        private byte[] Message(byte[] byt_SendData)
        {	/// sSendString ���͵�����   ���ؽ��յ�����


            //���
            this.m_str_TxFrame = "";
            this.m_str_RxFrame = "";
            this.m_byt_RevData = new byte[0];

            //������ʾ����֡
            this.m_str_TxFrame = GetDel0x33(byt_SendData);
            //����֡
            this.SendFrame(byt_SendData, m_int_WaitDataRevTime, m_int_IntervalTime);
            //������ʾ����֡
            this.m_str_RxFrame = GetDel0x33(m_byt_RevData);
            //����֡
            return m_byt_RevData;

        }



        /// <summary>
        /// ��֯֡
        /// </summary>
        /// <param name="cCmd">������</param>
        /// <param name="cAddr">���ַ</param>
        /// <param name="bLen">����</param>
        /// <param name="cData">����</param>
        /// <returns>������õ�֡</returns>
        /// 
        private byte[] OrgFrame(byte cCmd, string cAddr, byte bLen, string cData)
        {	/// cCmd ������  cAddr ��ַ bLen ���ݳ��� cData ���������� ������õ�֡
            byte[] bSend;
            int iTn;
            string cStr = "";

            bSend = new byte[12 + bLen];

            bSend[0] = 0x68;
            /// ��ַ��
            cAddr = cAddr.PadLeft(12, '0');
            for (iTn = 0; iTn <= 5; iTn++)
            {
                cStr = "0x" + cAddr.Substring(2 * (5 - iTn), 2);
                bSend[iTn + 1] = System.Convert.ToByte(cStr, 16);
            }
            bSend[7] = 0x68;
            bSend[8] = cCmd; //System.Convert.ToByte("0x" + cCmd, 16);
            bSend[9] = bLen;
            /// ������
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
            /// У����
            for (iTn = 0; iTn <= 9 + bLen; iTn++)
            {
                bSend[10 + bLen] += bSend[iTn];
            }
            /// ������
            bSend[11 + bLen] = 0x16;
            return bSend;
        }




        /// <summary>
        /// ��鷵��֡�Ϸ���
        /// </summary>
        /// <param name="bWrap">����֡</param>
        /// <param name="cData">��������</param>
        /// <param name="bExtend">�Ƿ��к���֡</param>
        /// <returns></returns>
        private bool CheckFrameSCBH(byte[] byt_RevFrame, ref string cData, ref bool bExtend)
        {/// ��������  cWrap ��Ҫ�����İ� cData ���ص�����	bExtend �Ƿ��к�������
            try
            {

                if (byt_RevFrame.Length <= 0)
                {
                    this.m_str_LostMessage = "û�з������ݣ�";
                    return false;
                }
                int int_Start = 0;
                int_Start = Array.IndexOf(byt_RevFrame, (byte)0x68);
                if (int_Start < 0 || int_Start > byt_RevFrame.Length || int_Start + 12 > byt_RevFrame.Length) //û��68��ͷ �����Ƿ��㹻һ֡ �Ƿ�����
                {
                    this.m_str_LostMessage = "����֡��������û��֡ͷ��[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }

                if (byt_RevFrame[int_Start + 7] != 0x68)        //�Ҳ����ڶ���68
                {
                    this.m_str_LostMessage = "����֡��������[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }

                int int_Len = byt_RevFrame[int_Start + 9];
                if (int_Start + 12 + int_Len != byt_RevFrame.Length)
                {
                    this.m_str_LostMessage = "���ݳ�����ʵ�ʳ��Ȳ�һ�£�[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;                //֡�ĳ����Ƿ���ʵ�ʳ���һ��
                }
                byte byt_Chksum = 0;
                for (int int_Inc = int_Start; int_Inc < int_Start + int_Len + 10; int_Inc++)
                    byt_Chksum += byt_RevFrame[int_Inc];
                if (byt_RevFrame[int_Start + int_Len + 10] != byt_Chksum)       //У���벻��ȷ
                {
                    this.m_str_LostMessage = "����У���벻��ȷ��[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }
                if (byt_RevFrame[int_Start + int_Len + 11] != 0x16)       //û��16����
                {
                    this.m_str_LostMessage = "����֡��������[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }

                //Array.Resize(ref byt_Addr, 6);
                //Array.Copy(byt_RevFrame, int_Start + 1, byt_Addr, 0, 6);
                //cmd
                byte[] byt_RevData = new byte[0];
                Array.Resize(ref byt_RevData, 6);    //�����򳤶�
                if (int_Len > 0)
                {
                    Array.Copy(byt_RevFrame, int_Start + 1, byt_RevData, 0, 6);
                    //for (int int_Inc = 0; int_Inc < byt_RevData.Length; int_Inc++)
                    //    byt_RevData[int_Inc] -= 0x33;

                    cData = BytesToString(byt_RevData);
                }


                //�Ƿ��к���֡
                if ((byt_RevFrame[int_Start + 8] & 0x20) == 0x20)
                    bExtend = true;
                else
                    bExtend = false;

                //�Ƿ񷵻ز����ɹ�     ��7Bit��1���Ƿ��أ���6bit��0=�ɹ���1=ʧ��
                if ((byt_RevFrame[int_Start + 7] ) == 0x68 )
                    return true;
                else
                {
                    if (byt_RevData != null && byt_RevData.Length > 0)
                        this.m_str_LostMessage = GetErrorMsg645(byt_RevData[0]) + "[" + BitConverter.ToString(byt_RevFrame) + "]";
                    else
                        this.m_str_LostMessage = "���ز���ʧ�ܣ�[" + BitConverter.ToString(byt_RevFrame) + "]";
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
        /// ��鷵��֡�Ϸ���
        /// </summary>
        /// <param name="bWrap">����֡</param>
        /// <param name="cData">��������</param>
        /// <param name="bExtend">�Ƿ��к���֡</param>
        /// <returns></returns>
        private bool CheckFrame(byte[] byt_RevFrame, ref string cData, ref bool bExtend)
        {/// ��������  cWrap ��Ҫ�����İ� cData ���ص�����	bExtend �Ƿ��к�������
            try
            {

                if (byt_RevFrame.Length <= 0)
                {
                    this.m_str_LostMessage = "û�з������ݣ�";
                    return false;
                }
                int int_Start = 0;
                int_Start = Array.IndexOf(byt_RevFrame, (byte)0x68);
                if (int_Start < 0 || int_Start > byt_RevFrame.Length || int_Start + 12 > byt_RevFrame.Length) //û��68��ͷ �����Ƿ��㹻һ֡ �Ƿ�����
                {
                    this.m_str_LostMessage = "����֡��������û��֡ͷ��[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }

                if (byt_RevFrame[int_Start + 7] != 0x68)        //�Ҳ����ڶ���68
                {
                    this.m_str_LostMessage = "����֡��������[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }

                int int_Len = byt_RevFrame[int_Start + 9];
                if (int_Start + 12 + int_Len != byt_RevFrame.Length)
                {
                    this.m_str_LostMessage = "���ݳ�����ʵ�ʳ��Ȳ�һ�£�[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;                //֡�ĳ����Ƿ���ʵ�ʳ���һ��
                }
                byte byt_Chksum = 0;
                for (int int_Inc = int_Start; int_Inc < int_Start + int_Len + 10; int_Inc++)
                    byt_Chksum += byt_RevFrame[int_Inc];
                if (byt_RevFrame[int_Start + int_Len + 10] != byt_Chksum)       //У���벻��ȷ
                {
                    this.m_str_LostMessage = "����У���벻��ȷ��[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }
                if (byt_RevFrame[int_Start + int_Len + 11] != 0x16)       //û��16����
                {
                    this.m_str_LostMessage = "����֡��������[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }

                //Array.Resize(ref byt_Addr, 6);
                //Array.Copy(byt_RevFrame, int_Start + 1, byt_Addr, 0, 6);
                //cmd
                byte[] byt_RevData = new byte[0];
                Array.Resize(ref byt_RevData, int_Len);    //�����򳤶�
                if (int_Len > 0)
                {
                    Array.Copy(byt_RevFrame, int_Start + 10, byt_RevData, 0, int_Len);
                    for (int int_Inc = 0; int_Inc < byt_RevData.Length; int_Inc++)
                        byt_RevData[int_Inc] -= 0x33;

                    cData=BytesToString(byt_RevData);
                }


                //�Ƿ��к���֡
                if ((byt_RevFrame[int_Start + 8] & 0x20) == 0x20)
                    bExtend = true;
                else
                    bExtend = false;

                //�Ƿ񷵻ز����ɹ�     ��7Bit��1���Ƿ��أ���6bit��0=�ɹ���1=ʧ��
                if ((byt_RevFrame[int_Start + 8] & 0x80) == 0x80 && (byt_RevFrame[int_Start + 8] & 0x40) == 0x00)
                    return true;
                else
                {
                    if (byt_RevData != null && byt_RevData.Length > 0)
                        this.m_str_LostMessage = GetErrorMsg645(byt_RevData[0]) + "[" + BitConverter.ToString(byt_RevFrame) + "]";
                    else
                        this.m_str_LostMessage = "���ز���ʧ�ܣ�[" + BitConverter.ToString(byt_RevFrame) + "]";
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
        /// �����������0x33�������֡
        /// </summary>
        /// <param name="cWrap"></param>
        /// <returns></returns>
        private string GetDel0x33(byte[] byt_Rec)
        {	///�����������0x33�������֡

            int int_Len = byt_Rec.Length;
            byte[] byt_Value = new byte[int_Len];
            Array.Copy(byt_Rec, 0, byt_Value, 0, int_Len);

            try
            {
                if (m_bol_ZendStringDel0x33)//�Ƿ��������0x33
                {
                    int iTn = 0, iTi = 0, iLen = 0;

                    if (byt_Value.Length <= 10) return HexCon.ByteToString(byt_Value);
                    /// ����֡ͷ
                    int int_Start = 0;
                    int_Start = Array.IndexOf(byt_Value, (byte)0x68);

                    iLen = byt_Value.Length;
                    /// ���������������
                    if ((byt_Value[int_Start + 9] + 12) == byt_Value.Length)
                    {
                        iTi = byt_Value[int_Start + 9];
                    }

                    for (iTn = 0; iTn < iTi; iTn++)
                    {
                        byt_Value[iTn + int_Start + 10] -= 0x33;
                    }
                }
                ///�����������0x33�������֡
                return HexCon.ByteToString(byt_Value);
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return "";
            }
        }



        /// <summary>
        /// ���ز���ʧ�ܣ��������
        /// </summary>
        /// <param name="byt_ErrCode"></param>
        /// <returns></returns>
        private string GetErrorMsg645(byte byt_ErrCode)
        {
            //����	��������	��ʱ������	��ʱ������	ͨ�����ʲ��ܸ���	�����/δ��Ȩ	����������	��������
            string str_Msg = "";
            if ((byt_ErrCode & 1) == 1)
                str_Msg = "���ͷǷ�����";
            else if ((byt_ErrCode & 2) == 2)
                str_Msg = "��ʶ������";
            else if ((byt_ErrCode & 4) == 4)
                str_Msg = "�������";
            else if((byt_ErrCode & 8) == 8)
                str_Msg = "ͨ�����ʲ��ܸ���";
            else if ((byt_ErrCode & 16) == 16)
                str_Msg = "��ʱ��������";
            else if ((byt_ErrCode & 32) == 32)
                str_Msg = "��ʱ��������";
            else if ((byt_ErrCode & 64) == 64)
                str_Msg = "����������";
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
        /// ��ת�ֽ��ַ���
        /// </summary>
        /// <param name="sData"></param>
        /// <returns></returns>
        public string BackString(string sData)
        {		//�ַ���������
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

    #region DLT645_1997 ��֡--CRC8У���� 1�ֽ�
    public class CRC8
    {
        public byte CRC(byte[] bLen)
        {
            byte CRC8 = 0;
            /// У����
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
        public static byte[] ASCIIStringToByte(string InString)//��һ���ַ�����Ϊһ���ֽ����С�
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetBytes(InString);
        }
        public static string ASCIIByteToString(byte[] InBytes)// ��һ���ֽ����н���Ϊһ���ַ����� 
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetString(InBytes);
        }
        public static string HexStringToASCIIString(string InString)// ��һ���ֽ����н���Ϊһ���ַ����� 
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
        /// �ַ���ת��Ϊ�ֽ�����
        /// </summary>
        /// <param name="InString">֡</param>
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
