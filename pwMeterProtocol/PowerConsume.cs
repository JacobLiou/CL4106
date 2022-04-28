
/****************************************************************************

    DLT645Э����
    ��ΰ 2009-10

*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using pwInterface;
using System.Diagnostics;
using System.Collections;
using System.Threading;
using System.Windows.Forms;
namespace pwMeterProtocol
{


    /// <summary>
    /// �������ܱ�645Э����࣬����1997��2007�汾,���ɼ̳�����
    /// </summary>
    public class PowerConsume : ProtocolBase, IMeterProtocol
    {
        private string m_str_RxID = "01";                   ///R�������Žڵ�ID�� 10H����ַ���ݱ�λ��1,2,3,4,5,6...
        private string m_str_TxID = "01";                   ///T�������Žڵ�ID�루��ַ�룩  ��ʱ��ֵΪ 01H ��
        private int m_int_WaitDataRevTime = 2000;           ///�ȴ����ݵ������ʱ��ms
        private int m_int_IntervalTime = 500;               ///���ݼ�����ʱ��ms
        private bool m_bol_ZendStringDel0x33 = false;		///���ͽ��յ����ݣ��������Ƿ��0x33
        private byte m_byt_iRepeatTimes = 3;                ///ͨѶʧ�����Դ���
        private bool m_bol_ClosComm = false;                ///ͨѶ��ɺ��Ƿ�رն˿�
        private bool m_bol_BreakDown = false;               ///���ⲿ�жϣ���Ҫ�������ش������ʱ

        public PowerConsume()
        {
            this.m_byt_RevData = new byte[0];
        }


        #region---����-------------------------
        //======================

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

        public string TxID
        {
            get
            {
                return this.m_str_TxID;
            }
            set
            {
                this.m_str_TxID = value;
            }
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


        #region---����-------------------------

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <param name="int_BwIndex">��λ��</param>
        /// <param name="int_Chancel">ͨ��</param>
        /// <param name="flt_U">��ѹ��Чֵ</param>
        /// <param name="flt_I">������Чֵ</param>
        /// <param name="flt_ActiveP">�й�����</param>
        /// <param name="flt_ApparentP">���ڹ���</param>
        /// <returns></returns>
        public bool ReadPower2(int int_BwIndex, int int_Chancel, ref float flt_U, ref float flt_I, ref float flt_ActiveP, ref float flt_ApparentP)
        {
            //0x81 + 0x10 + 0x80 + 0x1B + 0xA0 + 0x04 +��λ��+ͨ��+CS
            //��λ�ţ�����Ҫ��ȡ���ĵı�λ�ţ���������UINT1��
            //ChannelSwitch ����������UINT1����
            //����λ������Ե�Ԫģ���ţ�
            //ȡֵ1������9~24�ֽڷ��͵�A���ѹ��·���Ե�Ԫ��Ҳ��������ͨ��1��
            //ȡֵ2������9~24�ֽڷ��͵�A�������·���Ե�Ԫ��Ҳ��������ͨ��2��
            //ȡֵ3������9~24�ֽڷ��͵�B���ѹ��·���Ե�Ԫ��Ҳ��������ͨ��3��
            //ȡֵ4������9~24�ֽڷ��͵�B�������·���Ե�Ԫ��Ҳ��������ͨ��4��
            //ȡֵ5������9~24�ֽڷ��͵�C���ѹ��·���Ե�Ԫ��Ҳ��������ͨ��5��
            //ȡֵ6������9~24�ֽڷ��͵�C�������·���Ե�Ԫ��Ҳ��������ͨ��6��
            //����λ�á�1������


            this.m_str_LostMessage = "";
            byte[] byt_temp = new byte[4];
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;

                byte[] byt_Frame = ReadDataPage(0x04, (byte)int_BwIndex, (byte)int_Chancel);

                this.SendFrame(byt_Frame, 1200, 900);

                byte[] byt_Data = new byte[0];
                if (this.CheckFrameData(this.m_byt_RevData, 0x50, ref byt_Data))
                {

                    if (byt_Data[0] == (byte)int_BwIndex && byt_Data[1] == (byte)int_Chancel && byt_Data[2] == 0x01 && byt_Data[3] == 0x01)
                    {
                        Array.Copy(byt_Data, 4, byt_temp, 0, 4);
                        flt_U = BitConverter.ToSingle(byt_temp, 0);

                        Array.Copy(byt_Data, 8, byt_temp, 0, 4);
                        flt_I = BitConverter.ToSingle(byt_temp, 0);

                        Array.Copy(byt_Data, 12, byt_temp, 0, 4);
                        flt_ActiveP = BitConverter.ToSingle(byt_temp, 0);

                        Array.Copy(byt_Data, 16, byt_temp, 0, 4);
                        flt_ApparentP = BitConverter.ToSingle(byt_temp, 0);
                        return true;
                    }
                    else
                    {
                        this.m_str_LostMessage = "����֡��Page��Group��Data���·�֡��һ��";
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.Message;
                return false;
            }
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <param name="iCType">iCType     ���Ե�λ    ȡֵ��ΧΪ1-15����Ӧ15����ͬ�Ĳ��Ե�λ</param>
        /// <param name="iRType">iRType     ��������    1��A���ѹ��·���Ե�Ԫ��2��A�������·���Ե�Ԫ��3��B���ѹ��4��B�������5��C���ѹ��6��C�����</param>
        /// <param name="byt_RevData"></param>
        /// <param name="byt_SendFrame"></param>
        /// <returns></returns>
        public bool ReadPower(int iCType, int iRType, ref float flt_U, ref float flt_I, ref float flt_ActiveP, ref float flt_ApparentP)
        {
            this.m_str_LostMessage = "";
            byte[] byt_temp = new byte[4];
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                #region
                //H	 R	T	L	C	CLT1.0Э���������	CS
                //H����CLT1.0Э��ͷ���̶��ֽڣ�ֵ�� 81H
                //R�������Žڵ�ID�루��ַ�룩��ģ��IDʼ����10H
                //T�������Žڵ�ID�루��ַ�룩
                //L��������H��CS�����������ֽ���
                //CS����У��ͣ���ȥH��CS������������ֽڵİ�λ���ֵ��
                //C����CLT1.0��Э������֣�0xA3����д���ݣ�0xA0��������ݣ�0x50����ش����ݡ�

                //���Ĳ�ѯ����
                //81	01	xx	19	A0	xx	01	01	18	FF	FF	FF	16	FF	FF	FF	3C	FF	FF	FF	3E	FF	FF	FF	CS
                //1	    2	3	4	5	6	7	8	9	10	11	12	13	14	15	16	17	18	19	20	21	22	23	24	25
                //H	    R	T	L	C	CLT1.0Э���������	                                                        CS
                //                      PG	GP	G0	���ܱ��Ĳ���ģ���������
                //�ֽ�3�Ƿ��Žڵ�ID�롣�ֽ�2�ǹ��Ĳ��԰��ID�롣
                //�ֽ�6��ȡֵ�ǣ���4λȡֵ��1~6����4λȡֵ1~15��



                ////'iRType     ��������    1��A���ѹ��·���Ե�Ԫ��2��A�������·���Ե�Ԫ��3��B���ѹ��4��B�������5��C���ѹ��6��C�����
                ////'iCType     ���Ե�λ    ȡֵ��ΧΪ1-15����Ӧ15����ͬ�Ĳ��Ե�λ

                ////'''0x81 + ��λ�� + ���Žڵ�ID + 0x19 + 0xA0 + ChannelSwitch + 0x01 + 0x01 + 0x18FFFFFF16FFFFFF3CFFFFFF3EFFFFFF + ParityBit(У��λ)
                ////'''��λ��: ����Ҫ��ȡ���ĵı�λ�� (��������UINT1)
                ////'''ChannelSwitch (��������UINT1):
                ////'''����λ������Ե�Ԫģ����:
                ////'''ȡֵ1������9~24�ֽڷ��͵�A���ѹ��·���Ե�Ԫ��Ҳ��������ͨ��1��
                ////'''ȡֵ2������9~24�ֽڷ��͵�A�������·���Ե�Ԫ��Ҳ��������ͨ��2��
                ////'''ȡֵ3������9~24�ֽڷ��͵�B���ѹ��·���Ե�Ԫ��Ҳ��������ͨ��3��
                ////'''ȡֵ4������9~24�ֽڷ��͵�B�������·���Ե�Ԫ��Ҳ��������ͨ��4��
                ////'''ȡֵ5������9~24�ֽڷ��͵�C���ѹ��·���Ե�Ԫ��Ҳ��������ͨ��5��
                ////'''ȡֵ6������9~24�ֽڷ��͵�C�������·���Ե�Ԫ��Ҳ��������ͨ��6��
                ////'''����λ��"1"����
                ////'''
                ////'''����:
                ////'''0x81 + ��λ�� + ���Žڵ�id + 0x19 + 0x50 + ChannelSwitch + 0x01 +0x01 + ��ѹ��Чֵ+ ������Чֵ + �����й����� + �����޹����� + ParityBit(У��λ)
                ////'''
                ////'''��λ��: ��Ӧ��Ҫ��ȡ�ı�λ (��������UINT1)
                ////'''��ѹ��Чֵ����λ����32λ�����ʽ�����ֽڣ�
                ////'''������Чֵ����λ����32λ�����ʽ
                ////'''�����й����ʣ���λ�ߣ�32λ�����ʽ
                ////'''�����޹����ʣ���λ����32λ�����ʽ
                #endregion

                byte[] byt_Value = new byte[19];
                string sChannelSwitch = iCType.ToString() + iRType.ToString();
                byt_Value[0] = Convert.ToByte(sChannelSwitch, 16);     // XX ---ChannelSwitch (��������UINT1):
                #region
                byt_Value[1] = 0x01;
                byt_Value[2] = 0x01;
                byt_Value[3] = 0x18;
                byt_Value[4] = 0xFF;
                byt_Value[5] = 0xFF;
                byt_Value[6] = 0xFF;
                byt_Value[7] = 0x16;
                byt_Value[8] = 0xFF;
                byt_Value[9] = 0xFF;
                byt_Value[10] = 0xFF;
                byt_Value[11] = 0x3C;
                byt_Value[12] = 0xFF;
                byt_Value[13] = 0xFF;
                byt_Value[14] = 0xFF;
                byt_Value[15] = 0x3E;
                byt_Value[16] = 0xFF;
                byt_Value[17] = 0xFF;
                byt_Value[18] = 0xFF;
                #endregion
                //C����CLT1.0��Э������֣�0xA3����д���ݣ�0xA0��������ݣ�0x50����ش����ݡ�
                byte[] byt_SendData = this.OrgFrame(byt_Value, 0xA0);

                this.m_byt_RevData = new byte[0];
                this.SendFrame(byt_SendData, 1200, 900);

                byte[] byt_Data = new byte[0];
                string strflt = "";
                if (CheckFrame(this.m_byt_RevData, ref byt_Data))            //��鷵��֡�Ƿ�����
                {
                    Array.Copy(byt_Data, 3, byt_temp, 0, 4);
                    strflt = HexCon.ByteToString(byt_temp).Replace(" ","");
                    flt_U = pw_SingleHex.pwHexToSingle(strflt);


                    Array.Copy(byt_Data, 7, byt_temp, 0, 4);
                    strflt = HexCon.ByteToString(byt_temp).Replace(" ", "");
                    flt_I = pw_SingleHex.pwHexToSingle(strflt);


                    Array.Copy(byt_Data, 11, byt_temp, 0, 4);
                    strflt = HexCon.ByteToString(byt_temp).Replace(" ", "");
                    flt_ActiveP = pw_SingleHex.pwHexToSingle(strflt);

                    Array.Copy(byt_Data, 15, byt_temp, 0, 4);
                    strflt = HexCon.ByteToString(byt_temp).Replace(" ", "");
                    flt_ApparentP = pw_SingleHex.pwHexToSingle(strflt);
                    return true;
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

        public bool ReadPower(ref float flt_ActiveP)
        {
            float flt_U = 0f;
            float flt_I = 0f;
            //float flt_ActiveP = 0f;
            float flt_ApparentP = 0f;
            this.m_str_LostMessage = "";
            byte[] byt_temp = new byte[4];
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                #region
                //H	 R	T	L	C	CLT1.0Э���������	CS
                //H����CLT1.0Э��ͷ���̶��ֽڣ�ֵ�� 81H
                //R�������Žڵ�ID�루��ַ�룩��ģ��IDʼ����10H
                //T�������Žڵ�ID�루��ַ�룩
                //L��������H��CS�����������ֽ���
                //CS����У��ͣ���ȥH��CS������������ֽڵİ�λ���ֵ��
                //C����CLT1.0��Э������֣�0xA3����д���ݣ�0xA0��������ݣ�0x50����ش����ݡ�

                //���Ĳ�ѯ����
                //81	01	xx	19	A0	xx	01	01	18	FF	FF	FF	16	FF	FF	FF	3C	FF	FF	FF	3E	FF	FF	FF	CS
                //1	    2	3	4	5	6	7	8	9	10	11	12	13	14	15	16	17	18	19	20	21	22	23	24	25
                //H	    R	T	L	C	CLT1.0Э���������	                                                        CS
                //                      PG	GP	G0	���ܱ��Ĳ���ģ���������
                //�ֽ�3�Ƿ��Žڵ�ID�롣�ֽ�2�ǹ��Ĳ��԰��ID�롣
                //�ֽ�6��ȡֵ�ǣ���4λȡֵ��1~6����4λȡֵ1~15��



                ////'iRType     ��������    1��A���ѹ��·���Ե�Ԫ��2��A�������·���Ե�Ԫ��3��B���ѹ��4��B�������5��C���ѹ��6��C�����
                ////'iCType     ���Ե�λ    ȡֵ��ΧΪ1-15����Ӧ15����ͬ�Ĳ��Ե�λ

                ////'''0x81 + ��λ�� + ���Žڵ�ID + 0x19 + 0xA0 + ChannelSwitch + 0x01 + 0x01 + 0x18FFFFFF16FFFFFF3CFFFFFF3EFFFFFF + ParityBit(У��λ)
                ////'''��λ��: ����Ҫ��ȡ���ĵı�λ�� (��������UINT1)
                ////'''ChannelSwitch (��������UINT1):
                ////'''����λ������Ե�Ԫģ����:
                ////'''ȡֵ1������9~24�ֽڷ��͵�A���ѹ��·���Ե�Ԫ��Ҳ��������ͨ��1��
                ////'''ȡֵ2������9~24�ֽڷ��͵�A�������·���Ե�Ԫ��Ҳ��������ͨ��2��
                ////'''ȡֵ3������9~24�ֽڷ��͵�B���ѹ��·���Ե�Ԫ��Ҳ��������ͨ��3��
                ////'''ȡֵ4������9~24�ֽڷ��͵�B�������·���Ե�Ԫ��Ҳ��������ͨ��4��
                ////'''ȡֵ5������9~24�ֽڷ��͵�C���ѹ��·���Ե�Ԫ��Ҳ��������ͨ��5��
                ////'''ȡֵ6������9~24�ֽڷ��͵�C�������·���Ե�Ԫ��Ҳ��������ͨ��6��
                ////'''����λ��"1"����
                ////'''
                ////'''����:
                ////'''0x81 + ��λ�� + ���Žڵ�id + 0x19 + 0x50 + ChannelSwitch + 0x01 +0x01 + ��ѹ��Чֵ+ ������Чֵ + �����й����� + �����޹����� + ParityBit(У��λ)
                ////'''
                ////'''��λ��: ��Ӧ��Ҫ��ȡ�ı�λ (��������UINT1)
                ////'''��ѹ��Чֵ����λ����32λ�����ʽ�����ֽڣ�
                ////'''������Чֵ����λ����32λ�����ʽ
                ////'''�����й����ʣ���λ�ߣ�32λ�����ʽ
                ////'''�����޹����ʣ���λ����32λ�����ʽ
                #endregion

                byte[] byt_Value = new byte[19];
                //string sChannelSwitch = iCType.ToString() + iRType.ToString();
                //byt_Value[0] = Convert.ToByte(sChannelSwitch, 16);     // XX ---ChannelSwitch (��������UINT1):
                #region
                byt_Value[0] = 0x11;
                byt_Value[1] = 0x01;
                byt_Value[2] = 0x01;
                byt_Value[3] = 0x18;
                byt_Value[4] = 0xFF;
                byt_Value[5] = 0xFF;
                byt_Value[6] = 0xFF;
                byt_Value[7] = 0x16;
                byt_Value[8] = 0xFF;
                byt_Value[9] = 0xFF;
                byt_Value[10] = 0xFF;
                byt_Value[11] = 0x3C;
                byt_Value[12] = 0xFF;
                byt_Value[13] = 0xFF;
                byt_Value[14] = 0xFF;
                byt_Value[15] = 0x3E;
                byt_Value[16] = 0xFF;
                byt_Value[17] = 0xFF;
                byt_Value[18] = 0xFF;
                #endregion
                //C����CLT1.0��Э������֣�0xA3����д���ݣ�0xA0��������ݣ�0x50����ش����ݡ�
                byte[] byt_SendData = this.OrgFrame(byt_Value, 0xA0);

                this.m_byt_RevData = new byte[0];
                this.SendFrame(byt_SendData, 1200, 900);

                byte[] byt_Data = new byte[0];
                string strflt = "";
                if (CheckFrame(this.m_byt_RevData, ref byt_Data))            //��鷵��֡�Ƿ�����
                {
                    //Array.Copy(byt_Data, 3, byt_temp, 0, 4);
                    //strflt = HexCon.ByteToString(byt_temp).Replace(" ", "");
                    //flt_U = pw_SingleHex.pwHexToSingle(strflt);


                    //Array.Copy(byt_Data, 7, byt_temp, 0, 4);
                    //strflt = HexCon.ByteToString(byt_temp).Replace(" ", "");
                    //flt_I = pw_SingleHex.pwHexToSingle(strflt);



                    Array.Copy(byt_Data, 11, byt_temp, 0, 4);
                    strflt = HexCon.ByteToString(byt_temp).Replace(" ", "");
                    flt_ActiveP = pw_SingleHex.pwHexToSingle(strflt);

                    //Array.Copy(byt_Data, 15, byt_temp, 0, 4);
                    //strflt = HexCon.ByteToString(byt_temp).Replace(" ", "");
                    //flt_ApparentP = pw_SingleHex.pwHexToSingle(strflt);
                    return true;
                }
                else
                {
                    flt_ActiveP = 0f;
                    return false;
                }

            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        #endregion

        #region---˽��-------------------------

        /// <summary>
        /// ����������A0��ֻ��һ��Data��
        /// </summary>
        /// <param name="byt_Page">ҳ�ţ�ȡֵ��Χ0x00-0xFF</param>
        /// <param name="byt_Group">������֣�</param>
        /// <param name="byt_Grps">��������������֣�</param>
        /// <returns></returns>
        private byte[] ReadDataPage(byte byt_Page, byte byt_Group, byte byt_Grps)
        {
            //����Page��ʾҳ�ţ�ȡֵ��Χ00..ffH��Grp��Grp0..7�Ŀ����֣�bit0..7�ֱ��ʾGrp0..7�Ƿ���ڡ�Grp0..7�Ƿ�������֣�bit0..7�ֱ��ʾ�Ƿ���ҪData[0..7]����0����ʾ�޻���Ҫ����1����ʾ�л���Ҫ
            int int_Len = 3;
            byte[] byt_Data = new byte[int_Len];
            byt_Data[0] = byt_Page;
            byt_Data[1] = byt_Group;
            byt_Data[2] = byt_Grps;
            return OrgCLTFrame(0xA0, byt_Data);
        }

        /// <summary>
        /// ����CLT��֡
        /// </summary>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Data">������</param>
        /// <returns></returns>
        private byte[] OrgCLTFrame(byte byt_Cmd, byte[] byt_Data)
        {
            int int_Len = byt_Data.Length + 6;  //81��sRxID��sTxID��len��CMD��Chksum��ռһλ + ������
            byte[] byt_Frame = new byte[int_Len];
            //|֡��ʼ��ʶ|���Žڵ�ID��|���Žڵ�ID��|֡��|�����ֽ�|����|֡У����|
            byt_Frame[0] = 0x81;
            byt_Frame[1] = Convert.ToByte(m_str_RxID);
            byt_Frame[2] = Convert.ToByte(m_str_TxID);
            byt_Frame[3] = Convert.ToByte(int_Len);
            byt_Frame[4] = byt_Cmd;
            if (byt_Data.Length > 0)
                Array.Copy(byt_Data, 0, byt_Frame, 5, byt_Data.Length);
            byte byt_ChkSum = 0;
            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)
            {
                byt_ChkSum ^= byt_Frame[int_Inc];
            }
            byt_Frame[int_Len - 1] = byt_ChkSum;
            return byt_Frame;
        }

        /// <summary>
        /// ���鷵��֡��ʽ
        /// </summary>
        /// <param name="byt_RevValue"></param>
        /// <param name="byt_RevCmd"></param>
        /// <param name="byt_Data"></param>
        /// <returns></returns>
        private bool CheckFrameData(byte[] byt_RevValue, byte byt_RevCmd, ref byte[] byt_Data)
        {

            try
            {
                if (byt_RevValue.Length <= 0)
                {
                    this.m_str_LostMessage = "û���յ�����֡��";
                    return false;
                }
                byte byt_Cmd = 0;

                // Console.WriteLine(BitConverter.ToString(byt_RevValue));

                if (CheckFrame(byt_RevValue, ref byt_Cmd, ref byt_Data))
                {
                    if (byt_Cmd == byt_RevCmd)
                        return true;
                    else if (byt_Cmd == 0x33)
                    {
                        this.m_str_LostMessage = "�豸���ز���ʧ��ָ�";
                        return false;              //�Ƿ񷵻�OK
                    }
                    else if (byt_Cmd == 0x35)
                    {
                        this.m_str_LostMessage = "�豸������æָ����Ժ��ٲ���";
                        return false;              //�Ƿ񷵻�OK
                    }
                    else if (byt_Cmd == 0x36)
                    {
                        this.m_str_LostMessage = "�·���ָ����������ָ�";
                        return false;              //�Ƿ񷵻�OK
                    }
                    else
                    {
                        this.m_str_LostMessage = "����ָ���ȷ��";
                        return false;              //�Ƿ񷵻�OK
                    }
                }
                this.m_str_LostMessage = "����֡������Ҫ��";
                return false;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.Message;
                return false;
            }

        }

        /// <summary>
        /// ��֤֡�Ƿ����Ҫ��
        /// </summary>
        /// <param name="byt_Value">����֤֡����</param>
        /// <param name="byt_Cmd">����֡������</param>
        /// <param name="byt_Data">����֡����������</param>
        /// <returns></returns>
        private bool CheckFrame(byte[] byt_Value, ref byte byt_Cmd, ref byte[] byt_Data)
        {
            int int_Start = 0;
            int_Start = Array.IndexOf(byt_Value, (byte)0x81);
            if (int_Start < 0 || int_Start > byt_Value.Length) return false;    //û��81��ͷ
            if (int_Start + 3 >= byt_Value.Length) return false;                //û��֡�����ֽ�
            int int_Len = byt_Value[int_Start + 3];
            if (int_Len + int_Start > byt_Value.Length) return false;           //ʵ�ʳ�����֡���Ȳ����

            byte byt_ChkSum = 0;
            for (int int_Inc = 0; int_Inc < int_Len - 1; int_Inc++)
            {
                byt_ChkSum ^= byt_Value[int_Start + int_Inc];
            }
            if (byt_ChkSum != byt_Value[int_Start + int_Len - 1]) return false; //У���벻����


            byt_Cmd = byt_Value[int_Start + 4];     //������
            Array.Resize(ref byt_Data, int_Len - 6);    //�����򳤶�
            Array.Copy(byt_Value, int_Start + 5, byt_Data, 0, int_Len - 6);
            return true;
        }


        private byte[] OrgFrame(byte[] byt_Value, byte byt_Cmd)
        {

            //--֡��ʽ:-----------------------------------------------------------------------------
            //-----|֡��ʼ��ʶ|���Žڵ�ID��|���Žڵ�ID��|֡��|�����ֽ�|����|֡У����|
            //--------------------------------------------------------------------------------------
            //ILen = 6  '81��sRxID��sTxID��len��CMD��Chksum��ռһλ                    '��û�����ݿ����������ٵĳ���

            //H	 R	T	L	C	CLT1.0Э���������	CS
            //H����CLT1.0Э��ͷ���̶��ֽڣ�ֵ�� 81H
            //R�������Žڵ�ID�루��ַ�룩��ģ��IDʼ����10H
            //T�������Žڵ�ID�루��ַ�룩
            //L��������H��CS�����������ֽ���
            //CS����У��ͣ���ȥH��CS������������ֽڵİ�λ���ֵ��
            //C����CLT1.0��Э������֣�0xA3����д���ݣ�0xA0��������ݣ�0x50����ش����ݡ�
            //���Ĳ�ѯ����
            //81	01	xx	19	A0	xx	01	01	18	FF	FF	FF	16	FF	FF	FF	3C	FF	FF	FF	3E	FF	FF	FF	CS
            //1	    2	3	4	5	6	7	8	9	10	11	12	13	14	15	16	17	18	19	20	21	22	23	24	25
            //H	    R	T	L	C	CLT1.0Э���������	                                                        CS
            //                      PG	GP	G0	���ܱ��Ĳ���ģ���������

            int int_Len = 6 + byt_Value.Length;                 //81��RxID��TxID��len��CMD��Chksum��ռһλ
            byte[] byt_Data = new byte[int_Len];
            byt_Data[0] = 0x81;                                 //81
            byt_Data[1] = Convert.ToByte(this.m_str_RxID, 16);  //R�������Žڵ�ID�루��ַ�룩��ģ��IDʼ����10H
            byt_Data[2] = Convert.ToByte(this.m_str_TxID, 16);  //T�������Žڵ�ID�루��ַ�룩
            byt_Data[3] = (byte)int_Len;                              //֡��--L��������H��CS�����������ֽ���
            byt_Data[4] = byt_Cmd;    //Cmd

            for (int i = 0; i < byt_Value.Length; i++)              //data
            {
                byt_Data[5 + i] = byt_Value[i];
            }

            for (int int_Inc = 1; int_Inc < (byt_Data.Length - 1); int_Inc++)
            {
                byt_Data[byt_Data.Length - 1] ^= byt_Data[int_Inc];               //Chksum
            }
            return byt_Data;


        }
        private bool CheckFrame(byte[] byt_Value, ref byte[] byt_Data)
        {

            if (byt_Value.Length < 6) //֡��ʽ������5���ֽ�
            {
                if (byt_Value.Length == 0)
                    this.m_str_LostMessage = "û�з�������!";
                else
                    this.m_str_LostMessage = "�������ݲ�������";
                return false;
            }

            int int_Start = 0;
            int_Start = Array.IndexOf(byt_Value, (byte)0x81);
            if (int_Start < 0 || int_Start > byt_Value.Length)
            {
                this.m_str_LostMessage = "����֡������Ҫ���Ҳ���0x81!";
                return false;    //û��81��ͷ
            }

            if (int_Start + 3 >= byt_Value.Length)
            {
                this.m_str_LostMessage = "û�з�������!";
                return false;                //û��֡�����ֽ�
            }

            byte byt_Tmp = byt_Value[int_Start + 3];
            ////Array.Copy(byt_Value, int_Start + 3, byt_Tmp, 0, 1);
            ////Array.Reverse(byt_Tmp);             //���ڸ�λ��ǰ����λ�ں�����Ҫ�Ե�
            int int_Len = byt_Tmp;
            if (int_Len + int_Start > byt_Value.Length)
            {
                this.m_str_LostMessage = "ʵ�ʳ�����֡���Ȳ����!";
                return false;           //ʵ�ʳ�����֡���Ȳ����
            }
            byte byt_ChkSum = 0;
            for (int int_Inc = 0; int_Inc < int_Len - 1; int_Inc++)
            {
                byt_ChkSum ^= byt_Value[int_Start + int_Inc];
            }
            if (byt_ChkSum != byt_Value[int_Start + int_Len - 1])
            {
                this.m_str_LostMessage = "У���벻һ��!";
                return false; //У���벻����
            }
            Array.Resize(ref byt_Data, int_Len-6);    //�����򳤶�
            Array.Copy(byt_Value, int_Start+5, byt_Data, 0, int_Len-6);
            return true;
        }

        #endregion



        #region---������-------------------------

        private string m_str_Address = "000000000000";        ///���ַ
        private string m_str_Password = "000000";             ///������
        private byte m_byt_PasswordClass = 0;                 ///������ȼ�
        private string m_str_UserCode = "000000";             ///����Ա����
        private int m_int_PasswordType = 1;                   ///������֤���ͣ�0����������֤ 1����������������з�ʽ
        private bool m_bol_DataFieldPassword = false;         ///д����ʱ���������Ƿ����д����,true=Ҫ��false=����
        //===================


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

        #region ---�սӿ�-------------------------

        #region ��������
        public bool SendDLT645Command(byte bCmd, string cAddr, byte bDataLen, ref string cData, ref bool bExtend)
        {	/// cCmd ������  cAddr ��ַ bLen ���ݳ��� cData ���������� bExtend �Ƿ��к�������

            cData = "";
            bExtend = false;
            return false;

        }


        public bool SendDLT645Command(byte bCmd, string cAddr, byte bDataLen, ref string cData, ref byte[] byt_RevDataF, ref bool bExtend)
        {	/// cCmd ������  cAddr ��ַ bLen ���ݳ��� cData ���������� bExtend �Ƿ��к�������
            byt_RevDataF = new byte[0];
            cData = "";
            bExtend = false;
            return false;


        }


        
        public bool SendDLT645RxFrame(string RxFrame, ref string TxFrame, ref string cData)
        {	/// RxFrame ����֡  TxFrame ����֡ cData ����������
            TxFrame="";
            cData = "";
            return false;

        }

        public bool DownPara(List<MeterDownParaItem> _DownParaItemOne)
        {
                return false;
        }


        public bool ReadData(string str_OBIS, int int_Len, ref string str_Value)
        {
            str_Value = "";
            return false ;

        }

        public bool ReadData(string str_OBIS, int int_Len, int int_Dot, ref Single sng_Value)
        {
            sng_Value = 0;
            return false;
        }


        public bool WriteData(string str_OBIS, int int_Len, string str_Value)
        {
            return false ;

        }

        public bool WriteData(byte byte_Cmd, int int_Len, string str_Value)
        {
            return false;

        }


        public bool WriteData(string str_OBIS, int int_Len, int int_Dot, Single sng_Value)
        {
            return false ;

        }

        public bool ReadMeterAddress(ref string str_Value)
        {
            str_Value = "";
            return false;
        }

        public bool WriteMeterAddress(string str_Value)
        {
            return false;
        }

        public bool MakeFrame_Measure_Calibrate(string Addr, int i_APCU, ArrayList ArrayList_APDU, ref ArrayList fpByteList, ref string str_LostMessage)
        {
            str_LostMessage = "";
            ArrayList_APDU.Clear();
            fpByteList.Clear();
            return false ;
        }
        public bool SendAdjustData(int i_APCU, ArrayList ArrayList_APDU)
        {
            ArrayList arlist = new ArrayList();
            ArrayList_APDU.Clear();
            return false;
        }

        #endregion

        #region Ӧ�÷���
        public bool ReadScbh(ref string str_Value)
        {

            return false;
        }


        public bool SelfCheck(int M1, int M2, ref string str_Value)
        {

                str_Value = "";
                return false;
        }

        public bool ReadEnergy(ref string str_Value)
        {
            str_Value = "";
            return false;

        }

        public bool ReadSinglePhaseTest(ref string str_Value)
        {
            str_Value = "";
            return false;
        }

        public bool ReadACSamplingTest(ref string str_Value)
        {
            str_Value = "";
            return false;
        }

        public bool ReadVer(ref string str_Value)
        {
            str_Value = "";
            return false;

        }

        public bool InitMeterPara(string str_Value)
        {
            return false;
        }

        public bool SysClear()
        {
            return false;
        }

        public bool HighFrequencyPulse(int BS)
        {
            return false;
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


        #endregion



    }

}
