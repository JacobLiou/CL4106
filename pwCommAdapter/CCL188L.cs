/****************************************************************************

    CL188L����������
    ��ΰ 2012-05

*******************************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using pwInterface;
using System.Diagnostics;
namespace pwErrorCalculate
{ 
    public class CCL188L: IErrorCalculate
    {

        private string m_str_ID = "40";         //����ID
        private int m_int_ListCount = 0;        //����һ����������,���������м�������
        private int[] m_int_ListTable;          //�����б�
        private ISerialport m_Ispt_com;         //Щ���ߵĿ��ƶ˿�
        private string m_str_BwSelect = "000000000000000000000000000000000000000000000000"+ "111111111111111111111111111111111111111111111111"   ;//��λ��>��λ��1��ʾ��Ч��Ҫ��
        private int m_int_Bws = 24;


        private string m_str_LostMessage = "";  //ʧ����ʾ��Ϣ
        private byte[] m_byt_RevData;           //��������
        private bool m_bln_Enabled = true;      //��ǰ״̬
        private int m_int_Channel = 1;          //ͨ��
        private string m_str_Setting = "38400,n,8,1";

        public CCL188L()
        {

        }
        public CCL188L(int int_bws)
        {
            m_int_Bws = int_bws;
        }

        #region IErrorCalculate ��Ա
        
        #region ������Ա
        /// <summary>
        /// ��������ַ
        /// </summary>
        public string ID
        {
            get
            {
                return this.m_str_ID;
            }
            set
            {
                this.m_str_ID = value;
            }
        }


        /// <summary>
        /// �ز���
        /// </summary>
        public string Setting
        {
            set { this.m_str_Setting = value; }
        }


        /// <summary>
        /// ʧ����Ϣ
        /// </summary>
        public string LostMessage
        {
            get
            {
                return this.m_str_LostMessage;
            }
        }

        /// <summary>
        /// ������崮��
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
        /// ͨ��ѡ����Ҫ���ڿ���CL2011�豸�ģ���Чֵ1-4,
        /// ����PC���ڵ�RTSEnable��DTREnable��4�����
        /// CL2018-1������Ч��
        /// </summary>
        public int Channel
        {
            get { return this.m_int_Channel; }
            set { this.m_int_Channel = value; }
        }


        /// <summary>
        /// ���������λ��
        /// </summary>
        /// <param name="int_List">�˹���ı�λ�����б�</param>
        /// <returns></returns>
        public bool SetECListTable(int[] int_List)
        {
            try
            {
                this.m_int_ListTable = int_List;
                Array.Sort(this.m_int_ListTable);
                this.m_int_ListCount = int_List.Length;
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        public int AdaptCom(ISerialport[] mySerialPort)
        {
            throw new Exception("The method or operation is not implemented.");


        }

        public bool Link(ref bool[] bln_Result)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (!this.m_Ispt_com.State)
                {
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                    this.m_str_LostMessage = m_Ispt_com.LostMessage;
                    return false;
                }

                if (this.m_int_ListTable == null || this.m_int_ListCount <= 0)
                {
                    this.m_str_LostMessage = "������������û�б�λ�б�";
                    return false;
                }

                bool bln_IsOK = false;       //ֻҪ��һ����OK��OK
                for (int int_Inc = 0; int_Inc < this.m_int_ListCount; int_Inc++)
                {
                    bln_Result[this.m_int_ListTable[int_Inc] - 1] = this.Link(this.m_int_ListTable[int_Inc]);
                    if (bln_Result[this.m_int_ListTable[int_Inc] - 1])
                    {
                        bln_IsOK = true;
                        break;
                    }
                }
                return bln_IsOK;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        public bool Link(int int_Num)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;

                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                if (Array.IndexOf(this.m_int_ListTable, int_Num) == -1)
                {
                    this.m_str_LostMessage = "�˱�λ�Ų��������ϣ�";
                    return false;
                }

                byte[] byt_SendData = this.OrgFrame(0xC0, 0x00, Convert.ToByte(int_Num));//ͨ���������������ݴӶ��ж��Ƿ��з���
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(400, 200);
                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //��鷵��֡�Ƿ�����
                {
                    if (byt_RevData[4] == 0x50)//����0x50

                        return true;
                    else
                    {
                        this.m_str_LostMessage = "����ʧ��ָ�";
                        return false ;
                    }
                }
                else
                    return false ;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }
        #endregion

        #region F1H��	���õ������춨ʱ������������������Ȧ������׼��ǰ��������׼��ǰ���ʡ����������Ŵ�ϵ��
        /// <summary>
        /// ͳһ�����������
        /// </summary>
        /// <param name="lAmMeterPulseConst">�������</param>
        /// <param name="iPulseCount">�����Ȧ��</param>
        /// <param name="lStdPulseConst">��׼��ǰ��������Ҫ�ӱ�׼���ȡ</param>
        /// <param name="fStdP">��׼��ǰ���ʣ���Ҫ�ӱ�׼���ȡ</param>
        /// <param name="iAmMeterPulseBS">���������Ŵ�ϵ����-128~127����Ĭ��Ϊ1���Ŵ�Ҳ����С1</param>
        /// <returns></returns>
        public bool SetDnWcrPara(long lAmMeterPulseConst, long iPulseCount, long lStdPulseConst, float fStrandMeterP, byte iAmMeterPulseBS)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                return SetDnWcrPara(255, lAmMeterPulseConst, iPulseCount, lStdPulseConst, fStrandMeterP, iAmMeterPulseBS);

            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// ���õ��ܱ�����Ȧ��
        /// </summary>
        /// <param name="int_ChNo">��λ�ţ��㲥��־(0xFFH)��żλ(0xEEH)����λ(0xDDH)</param>
        /// <param name="lng_PulseConst1">�������</param>
        /// <param name="lng_PulseQs">���춨Ȧ��</param>
        /// <param name="lStdPulseConst">��׼��ǰ��������Ҫ�ӱ�׼���ȡ</param>
        /// <param name="fStdP">��׼��ǰ���ʣ���Ҫ�ӱ�׼���ȡ</param>
        /// <param name="iAmMeterPulseBS">���������Ŵ�ϵ����-128~127����Ĭ��Ϊ1���Ŵ�Ҳ����С1</param>
        /// <returns></returns>
        public bool SetDnWcrPara(int int_ChNo, long lng_PulseConst1, long lng_PulseQs, long lStdPulseConst, float fStrandMeterP, byte iAmMeterPulseBS)
        {
            //Data����֯��ʽΪ���㲥��־(0xFFH) +��1 byte ListLen�� + ��12 bytes List�� + ��׼���峣����4Bytes��+ ��׼����Ƶ�ʣ�4Bytes��+ ��׼���峣�����ű�����1Bytes��+ �������峣����4Bytes�� + У��Ȧ����4Bytes��+ �������峣�����ű���(1Byte)+���ͱ�־1��1Byte��
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                long lngtmpStdPulsePl =  GetStdPulsePl(lStdPulseConst, fStrandMeterP);//�ɷ��ù̶�ֵ�������������������û������1000;//

                byte[] byt_Value = new byte[19];
                byte[] byt_Tmp = BitConverter.GetBytes(Convert.ToInt64(lStdPulseConst));//��׼���峣��

                Array.Copy(byt_Tmp, 0, byt_Value, 0, 4);

                byt_Tmp = BitConverter.GetBytes(Convert.ToInt64(lngtmpStdPulsePl));//��׼����Ƶ��
                Array.Copy(byt_Tmp, 0, byt_Value, 4, 4);

                byt_Value[8] = 0x01;//�Ŵ�ϵ��

                byt_Tmp = BitConverter.GetBytes(Convert.ToInt32(lng_PulseConst1));//���������
                Array.Copy(byt_Tmp, 0, byt_Value, 9, 4);


                byt_Tmp = BitConverter.GetBytes(Convert.ToInt32(lng_PulseQs));//У��Ȧ��
                Array.Copy(byt_Tmp, 0, byt_Value, 13, 4);

                byt_Value[17] = iAmMeterPulseBS;//�Ŵ�ϵ��

                byt_Value[18] = 0xAA;//���ͱ�־
                byte[] byt_SendData;

                if (int_ChNo == 255)
                {
                    byt_SendData = this.OrgFrame(0xF1, byt_Value);    //ͳһ����
                }
                else
                {
                    byt_SendData = this.OrgFrame(0xF1, byt_Value, Convert.ToByte(int_ChNo));    //������������
                }

                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(300, 200);
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(300, 200);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }
        #endregion


        #region F3H��	�����ռ�ʱ���춨ʱ��Ƶ�ʼ������������춨ʱ��
        /// <summary>
        /// �����ռ�ʱ���춨ʱ��Ƶ�ʣ�Ȧ��
        /// </summary>
        /// <param name="flt_MeterHz">����ʱ��Ƶ��</param>
        /// <param name="int_Pulse">�����������</param>
        /// <returns></returns>
        public bool SetTimePara(float flt_MeterHz,int int_Pulse)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                if (this.m_int_ListTable == null || this.m_int_ListCount <= 0)
                {
                    this.m_str_LostMessage = "������������û�б�λ�б�";
                    return false;
                }
                return this.SetTimePara(255,500000, flt_MeterHz, int_Pulse);
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// �����ռ�ʱ���춨ʱ��Ƶ�ʣ�Ȧ��
        /// </summary>
        /// <param name="flt_MeterHz">����ʱ��Ƶ��</param>
        /// <param name="int_Pulse">�����������</param>
        /// <returns></returns>
        public bool SetTimePara(float[] flt_MeterHz,int[] int_Pulse)
        {
            //Data����֯��ʽΪ���㲥��־(0XFF) +��1 byte ListLen�� + ��12 bytes List�� + ��׼ʱ��Ƶ��100����4Bytes��+ ����ʱ��Ƶ��100����4Bytes��+ �������������4Bytes��+���ͱ�־2��1Byte��
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                if (this.m_int_ListTable == null || this.m_int_ListCount <= 0)
                {
                    this.m_str_LostMessage = "������������û�б�λ�б�";
                    return false;
                }
                bool bln_AllSame = false;
                int[] int_Tmp = int_Pulse;
                Array.Sort(int_Tmp);
                if (int_Tmp[0] == int_Tmp[int_Tmp.Length - 1])          //�ж��Ƿ����
                {
                    float[] flt_Tmp = flt_MeterHz;
                    Array.Sort(flt_Tmp);
                    if (flt_Tmp[0] == flt_Tmp[flt_Tmp.Length - 1])
                        bln_AllSame = true;
                }

                if (bln_AllSame)
                    return this.SetTimePara(255,500000, flt_MeterHz[0],int_Pulse[0]);
                else
                {
                    for (int int_Int = 0; int_Int < this.m_int_ListCount; int_Int++)
                    {
                        this.SetTimePara(flt_MeterHz[this.m_int_ListTable[int_Int] - 1],int_Pulse[this.m_int_ListTable[int_Int] - 1]);
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// �ռ�ʱ���������ã���λ�ţ�������Ƶ�ʣ�Ȧ��
        /// </summary>
        /// <param name="int_Num">��λ�ţ��㲥��־(0xFFH)��żλ(0xEEH)����λ(0xDDH)</param>
        /// <param name="flt_MeterHz">��׼ʱ�����峣��</param>
        /// <param name="flt_MeterHz">����ʱ��Ƶ��</param>
        /// <param name="int_Pulse">�����������</param>
        /// <returns></returns>
        public bool SetTimePara(int int_Num,long lStdTimeConst, float flt_MeterHz, int int_Pulse)
        {

            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                if (Array.IndexOf(this.m_int_ListTable, int_Num) == -1 && int_Num != 255)
                {
                    this.m_str_LostMessage = "�˱�λ�Ų��������ϣ�";
                    return false;
                }



                //byte[] byt_Value = new byte[14]; ; //������
                ////frameData = new byte[0];
                ////InitDataRegion(out dataRegion, 28);
                ////��׼ʱ��Ƶ��
                //Array.Copy(BitConverter.GetBytes((uint)(500000 * 100)), 0, byt_Value, 0, 4);
                ////����ʱ��Ƶ��
                //Array.Copy(BitConverter.GetBytes((uint)(1 * 100)), 0, byt_Value, 4, 4);
                ////�����������
                //Array.Copy(BitConverter.GetBytes(int_Pulse/10), 0, byt_Value, 8, 4);
                //byt_Value[12] = 0xFF;
                //byt_Value[13] = 0xFF;
                //��׼ʱ��Ƶ��100����4Bytes��
                //+ ����ʱ��Ƶ��100����4Bytes��
                // + �������������4Bytes��
                // +���ͱ�־2��1Byte��

                byte[] byt_Value = new byte[13];
                byte[] byt_Tmp;
                byt_Tmp = BitConverter.GetBytes(Convert.ToInt32(lStdTimeConst * 100));//��׼ʱ��Ƶ��100��
                Array.Copy(byt_Tmp, 0, byt_Value, 0, 4);

                byt_Tmp = BitConverter.GetBytes(Convert.ToInt32(flt_MeterHz * 100));//����ʱ��Ƶ��100����4Bytes��
                Array.Copy(byt_Tmp, 0, byt_Value, 4, 4);

                byt_Tmp = BitConverter.GetBytes(Convert.ToInt32(int_Pulse));//�����������
                Array.Copy(byt_Tmp, 0, byt_Value, 8, 4);

                byt_Value[12] = 0x55;//���ͱ�־


                byte[] byt_SendData;

                if (int_Num == 255)
                {

                    byt_SendData = this.OrgFrameForTimePulse(0xF3, byt_Value); //41H

                }
                else
                {
                    byt_SendData = this.OrgFrame(0xF3, byt_Value, Convert.ToByte(int_Num)); //41H

                }

                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(300, 300);
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(300, 300);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        #endregion


        #region A7H��	ѡ�񱻼�����ͨ�����춨����
        /// <summary>
        /// ��������ͨ��,0=P+,1=P-,2=Q+,3=Q-,4=����,5=ʱ�ӣ�������������,0=����,1=����������ͨ������,0=�����,1=���ͷ
        /// </summary>
        /// <param name="iPulseChannel">����ͨ��,0=P+,1=P-,2=Q+,3=Q-,4=����,5=ʱ��</param>
        /// <param name="iChannelType">ͨ������,0=�����,1=���ͷ</param>
        /// <param name="iPulseType">��������,0=����,1=����</param>
        /// <returns></returns>
        public bool SelectPulseChannel(int iPulseChannel, int iChannelType, int iPulseType)
        {
            //Data����֯��ʽΪ���㲥��־(0xFFH) +��1 byte ListLen�� + ��12 bytes List�� + ��������ͨ���ţ�2Byte��+ �춨���ͣ�1Byte��
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_int_ListTable == null || this.m_int_ListCount <= 0)
                {
                    this.m_str_LostMessage = "������������û�б�λ�б�";
                    return false;
                }
                return this.SelectPulseChannel(255, iPulseChannel, iChannelType, iPulseType);

            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }


        }

        /// <summary>
        /// ��������ͨ��,0=P+,1=P-,2=Q+,3=Q-,4=����,5=ʱ�ӣ�������������,0=����,1=����������ͨ������,0=�����,1=���ͷ
        /// </summary>
        /// <param name="iPulseChannel">����ͨ��,0=P+,1=P-,2=Q+,3=Q-,4=����,5=ʱ��</param>
        /// <param name="iChannelType">ͨ������,0=�����,1=���ͷ</param>
        /// <param name="iPulseType">��������,0=����,1=����</param>
        /// <returns></returns>
        public bool SelectPulseChannel(int[] iPulseChannel, int[] iChannelType, int[] iPulseType)
        {
            //Data����֯��ʽΪ���㲥��־(0xFFH) +��1 byte ListLen�� + ��12 bytes List�� + ��������ͨ���ţ�2Byte��+ �춨���ͣ�1Byte��
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_int_ListTable == null || this.m_int_ListCount <= 0)
                {
                    this.m_str_LostMessage = "������������û�б�λ�б�";
                    return false;
                }

                bool bln_AllSame = false;
                int[] int_Tmp = iPulseType;
                Array.Sort(int_Tmp);                    //����
                if (int_Tmp[0] == int_Tmp[int_Tmp.Length - 1])  //��һ�������һ�������һ���Ļ�����ȫһ��
                {
                    int_Tmp = iChannelType;
                    Array.Sort(int_Tmp);
                    if (int_Tmp[0] == int_Tmp[int_Tmp.Length - 1])   //��һ�������һ�������һ���Ļ�����ȫһ��
                    {
                        int_Tmp = iPulseChannel;
                        Array.Sort(int_Tmp);
                        if (int_Tmp[0] == int_Tmp[int_Tmp.Length - 1])   //��һ�������һ�������һ���Ļ�����ȫһ��
                            bln_AllSame = true;
                    }
                }
                if (bln_AllSame)
                    return this.SelectPulseChannel(255, iPulseChannel[0], iChannelType[0], iPulseType[0]);
                else
                {
                    for (int int_Int = 0; int_Int < this.m_int_ListCount; int_Int++)
                    {
                        this.SelectPulseChannel(this.m_int_ListTable[int_Int], iPulseChannel[this.m_int_ListTable[int_Int] - 1], iChannelType[this.m_int_ListTable[int_Int] - 1], iPulseType[this.m_int_ListTable[int_Int] - 1]);
                    }
                    return true;
                }

            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }


        }

        /// <summary>
        /// ��������ͨ��
        /// </summary>
        /// <param name="iNum">��λ��</param>
        /// <param name="iPulseChannel">����ͨ��,0=P+,1=P-,2=Q+,3=Q-,4=����,5=ʱ��</param>
        /// <param name="iChannelType">ͨ������,0=�����,1=���ͷ</param>
        /// <param name="iPulseType">��������,0=����,1=����</param>
        /// <returns></returns>
        public bool SelectPulseChannel(int iNum, int iPulseChannel, int iChannelType, int iPulseType)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                if (Array.IndexOf(this.m_int_ListTable, iNum) == -1 && iNum != 255)
                {
                    this.m_str_LostMessage = "�˱�λ�Ų��������ϣ�";
                    return false;
                }
                if (iPulseChannel < 0 || iPulseChannel > 5 ||
                    iChannelType < 0 || iChannelType > 1 ||
                    iPulseType < 0 || iPulseType > 1)
                {
                    this.m_str_LostMessage = "���ò������ԣ�";
                    return false;
                }
                byte[] byt_Value = new byte[3];



                #region byt_Value[0]�� Bit0��Bit1��Bit2��ʾ�������ͨ���ţ�Bit2Bit1Bit0ֵ�뱻ѡͨ����ϵΪ��0P+ ��1P-�� 2Q+�� 3Q-����
                //Bit2Bit1Bit0ֵ�뱻ѡͨ����ϵΪ��0P+ ��1P-�� 2Q+�� 3Q-����
                //Bit3��ʾ���ͷѡ��λ��1Ϊ��Ӧʽ�������룬0Ϊ����ʽ�������룻
                //Bit4Ϊ���弫��ѡ��Bit4Ϊ0��ʾ����������͵�ƽ����������Bit4Ϊ1��ʾ����������ߵ�ƽ����������
                if (iPulseChannel > 3)//5=ʱ�����壬4=������������
                {
                    byt_Value[0] = Convert.ToByte( iChannelType * 8 + iPulseType * 16);//����\��е �� ����\������
                }
                else//��������P+,P-,Q+,Q-
                {
                    byt_Value[0] = Convert.ToByte(iPulseChannel + iChannelType  * 8 + iPulseType * 16);
                }
                #endregion

                #region byt_Value[1]�е� Bit1Bit0ֵ��ʾ�๦�����ͨ���ţ�1Ϊ�ռ�ʱ���塢2Ϊ��������
                if (iPulseChannel == 5)
                {
                    byt_Value[1] = 0x01;//1Ϊ�ռ�ʱ����
                }
                else if (iPulseChannel == 4)
                {
                    byt_Value[1] = 0x02;//2Ϊ�������塣
                }
                else
                {
                    byt_Value[1] = 0x00;//����չ
                }

                #endregion

                #region byt_Value[2] �춨�������ͣ�0x00������0x01������0x02�ռ�ʱ��0x03���ּ�����0x04�Աꡢ0x05Ԥ���ѹ��ܼ춨
                if (iPulseChannel == 5)
                {
                    byt_Value[2] = 0x02;// ʱ������� = 2,
                }
                else if (iPulseChannel == 4)
                {
                    byt_Value[2] = 0x01;//�������� = 1,  
                }
                else //if (iPulseChannel < 4)
                {
                    byt_Value[2] = 0x00; 
                }
                #endregion

                #region �·�֡
                byte[] byt_SendData;
                if (iNum == 255)
                {
                    byt_SendData = this.OrgFrame(0xA7, byt_Value);  //������ȫ��λ����
                }
                else
                {
                    //����λ����
                    byt_SendData = this.OrgFrame(0xA7, byt_Value, Convert.ToByte(iNum));
                }

                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(200, 200);
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(200, 200);
                return true;
                #endregion
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }
        #endregion

        #region ADH��	ѡ�񱻱������������
        /// <summary>
        /// ѡ�񱻱�����������ͣ�0=�������ӣ�1=��������
        /// </summary>
        /// <param name="iPulseDzType">0=�������ӣ�1=��������</param>
        /// <returns></returns>
        public bool SetMeterPulseDzType(int iNum, int iPulseDzType)
        {
            //Data����֯��ʽΪ���㲥��־(0xFFH) +��1 byte ListLen�� + ��12 bytes List�� + �������ͣ�1Byte��
            //����������ͣ� 0x00��ʾ�������ӣ�0X01��ʾ�������ӣ�
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                byte bytCmd;

                bytCmd = Convert.ToByte(iPulseDzType);

                byte[] byt_SendData = this.OrgFrameForComm(0xAD, Convert.ToByte(iNum), bytCmd);

                this.m_byt_RevData = new byte[0];

                this.m_Ispt_com.SendData(byt_SendData);

                Waiting(300, 300);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }

        public bool SetMeterPulseDzType(int iPulseDzType)
        {
            return SetMeterPulseDzType(255, iPulseDzType);
        }

        #endregion

        #region ACH��	ͨѶѡ��
        /// <summary>
        /// ���Ʊ�λͨ�ſڿ��أ�ͨѶѡ�� 0x00��ʾѡ��һ��һģʽ485ͨѶ��Ĭ��ģʽ����0X01��ʾѡ��������λ485ͨѶ��0X02��ʾѡ��ż����λ485ͨѶ��0x03��ʾѡ��һ��һģʽ����ͨѶ��0X04��ʾѡ��������λ����ͨѶ��0X05��ʾѡ��ż����λ����ͨѶ��0X06��ʾѡ���л���485���ߣ����ԺЭ���ã���
        /// </summary>
        /// <param name="int_Num">��λ��(255���б�λ)FF ȫ��λ��0xEE,ż����0xDD����</param>
        /// <param name="bln_Open">�Ƿ�򿪣�true=�򿪣�flase=�ر�</param>
        /// <returns></returns>
        public bool SetCommSwitch(int int_Num, bool bln_Open)
        {
            //Data����֯��ʽΪ���㲥��־(0xFFH) +��1 byte ListLen�� + ��12 bytes List�� + ͨѶѡ��1Byte��
            //ͨѶѡ�� 0x00��ʾѡ��һ��һģʽ485ͨѶ��Ĭ��ģʽ����0X01��ʾѡ��������λ485ͨѶ��0X02��ʾѡ��ż����λ485ͨѶ��
            //ͨѶѡ�� 0x03��ʾѡ��һ��һģʽ����ͨѶ��0X04��ʾѡ��������λ����ͨѶ��0X05��ʾѡ��ż����λ����ͨѶ��
            //ͨѶѡ�� 0X06��ʾѡ���л���485���ߣ����ԺЭ���ã���
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                byte bytCmd;
                if (int_Num == 0xFF)
                {
                    bytCmd = Convert.ToByte(bln_Open ? 0x00 : 0x03);
                }
                else if (int_Num == 0xEE)
                {
                    bytCmd = Convert.ToByte(bln_Open ? 0x02 : 0x03);
                }
                else if (int_Num == 0xDD)
                {
                    bytCmd = Convert.ToByte(bln_Open ? 0x01 : 0x03);
                }
                else
                {
                    if (int_Num % 2 == 0)
                    {
                        bytCmd = Convert.ToByte(bln_Open ? 0x02 : 0x03);
                    }
                    else
                    {
                        bytCmd = Convert.ToByte(bln_Open ? 0x01 : 0x03);

                    }
                }

                byte[] byt_SendData = this.OrgFrameForComm(0xAC, Convert.ToByte(int_Num), bytCmd);

                this.m_byt_RevData = new byte[0];

                this.m_Ispt_com.SendData(byt_SendData);

                Waiting(300, 300);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }
        #endregion


        #region AFH��	����˫��·�춨ʱ��ѡ�����е�ĳһ·��Ϊ�����������·
        /// <summary>
        /// ��·�л�
        /// </summary>
        /// <param name="int_DL_type">������·�� 0һ��· 1����·</param>
        /// <param name="int_DY_type">��ѹ��·��0x00ֱ�ӽ���ʽ��0x01����������0x02��ʾ����λ�޵�����</param>
        /// <returns></returns>
        public bool SetDLSwitch(int int_DL_type, int int_DY_type)
        {
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                    this.m_Ispt_com.Channel = this.m_int_Channel;


                byte[] byt_tmpdata = new byte[2];
                byt_tmpdata[0] = Convert.ToByte(int_DL_type);//������·
                byt_tmpdata[1] = Convert.ToByte(int_DY_type);//��ѹ��·
                byte[] byt_SendData = this.OrgFrameForDLSwich(0xAF, byt_tmpdata);              //B4H�����λ
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(200, 200);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// ��·�л�
        /// </summary>
        /// <param name="int_DL_type">������·�� 0һ��· 1����·</param>
        /// <returns></returns>
        public bool SetDLSwitch(int int_DL_type)
        {
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                    this.m_Ispt_com.Channel = this.m_int_Channel;


                byte[] byt_tmpdata = new byte[2];
                byt_tmpdata[0] = Convert.ToByte(int_DL_type);
                byt_tmpdata[1] = 0x00;
                byte[] byt_SendData = this.OrgFrameForDLSwich(0xAF, byt_tmpdata);              //AFH��·�л���λ
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(200, 200);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }
        #endregion


        #region B1H��	�������㹦��ָ��
        /// <summary>
        /// ����������㹦�� ���������Ͳ���
        /// </summary>
        /// <param name="byt_TaskType">�춨���ͣ�0x00������0x01������0x02�ռ�ʱ��0x03���ּ�����0x04�Աꡢ0x05Ԥ���ѹ��ܼ춨</param>
        /// <returns></returns>
        public bool StartCalculate(int byt_TaskType)
        {
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                byte[] byt_SendData = this.OrgFrame(0xB1, Convert.ToByte(byt_TaskType));   //B1H��������
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(200, 200);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// ����������㹦��
        /// </summary>
        ///<param name="int_Num">��λ��(255���б�λ)FF ȫ��λ��0xEE,ż����0xDD����</param>
        /// <param name="byt_TaskType">�춨���ͣ�0x00������0x01������0x02�ռ�ʱ��0x03���ּ�����0x04�Աꡢ0x05Ԥ���ѹ��ܼ춨</param>
        /// <returns></returns>
        private bool StartCalculate(int int_Num, int byt_TaskType)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                this.m_Ispt_com.Setting = this.m_str_Setting;
                //��������,0=�������,1=��������,2=�ռ�ʱ���,3=����,4=�Ա�
                if (byt_TaskType < 0 || byt_TaskType > 4)
                {
                    this.m_str_LostMessage = "�������ͳ���ָ����Χ0-3!";
                    return false;
                }
                if (Array.IndexOf(this.m_int_ListTable, int_Num) == -1 && int_Num != 255)
                {
                    this.m_str_LostMessage = "�˱�λ�Ų��������ϣ�";
                    return false;
                }
                byte[] byt_Value = new byte[2];
                byt_Value[0] = Convert.ToByte(byt_TaskType);//��������
                byte[] byt_SendData;
                if (int_Num == 255)
                {
                    byt_SendData = this.OrgFrame(0xB1, byt_Value[0]);  //������ȫ��λ����
                }
                else
                {
                    byt_Value[1] = Convert.ToByte(int_Num);//����λ����
                    byt_SendData = this.OrgFrame(0xB1, byt_Value[0], byt_Value[1]);
                }

                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(200, 200);
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(200, 200);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        #endregion


        #region B2H��	ֹͣ�춨����ָ��
        /// <summary>
        /// ֹͣ������㹦��
        /// </summary>
        /// <param name="byt_TaskType">�춨���ͣ�0x00������0x01������0x02�ռ�ʱ��0x03���ּ�����0x04�Աꡢ0x05Ԥ���ѹ��ܼ춨</param>
        /// <returns></returns>
        public bool StopCalculate(int byt_TaskType)
        {
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                byte[] byt_SendData = this.OrgFrame(0xB2, Convert.ToByte(byt_TaskType));   //B2Hֹͣ����
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(200, 200);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// ֹͣ������㹦��
        /// </summary>
        ///<param name="int_Num">��λ��(255���б�λ)FF ȫ��λ��0xEE,ż����0xDD����</param>
        /// <param name="byt_TaskType">�춨���ͣ�0x00������0x01������0x02�ռ�ʱ��0x03���ּ�����0x04�Աꡢ0x05Ԥ���ѹ��ܼ춨</param>
        /// <returns></returns>
        private bool StopCalculate(int int_Num, int byt_TaskType)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                //��������,0=�������,1=��������,2=�ռ�ʱ���,3=����,4=�Ա�
                this.m_Ispt_com.Setting = this.m_str_Setting;
                if (byt_TaskType < 0 || byt_TaskType > 4)
                {
                    this.m_str_LostMessage = "�������ͳ���ָ����Χ0-3!";
                    return false;
                }
                if (Array.IndexOf(this.m_int_ListTable, int_Num) == -1 && int_Num != 255)
                {
                    this.m_str_LostMessage = "�˱�λ�Ų��������ϣ�";
                    return false;
                }
                byte[] byt_Value = new byte[2];
                byt_Value[0] = Convert.ToByte(byt_TaskType);//��������
                byte[] byt_SendData;
                if (int_Num == 255)
                {
                    byt_SendData = this.OrgFrame(0xB2, byt_Value[0]);  //������ȫ��λ����
                }
                else
                {
                    byt_Value[1] = Convert.ToByte(int_Num);//����λ����
                    byt_SendData = this.OrgFrame(0xB2, byt_Value[0], byt_Value[1]);
                }

                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(200, 200);
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(200, 200);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }


        #endregion


        #region B4H��	���ϱ�λ��ѹ�����������
        /// <summary>
        /// ���ϱ�λ��ѹ����������ƣ��޲��������Ʒ��������б��У�12 bytes List��
        /// </summary>
        /// <returns></returns>
        public bool SetBwGLSwitch()
        {
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                byte[] byt_SendData = this.OrgFrameForGL(0xB4);   //B4H�����λ
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(200, 200);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }


        #endregion


        #region C2H��	�����λ״̬
        /// <summary>
        /// ������߹���״̬��
        /// </summary>
        /// ״̬���ͷ�Ϊ���֣����߹���״̬01��Ԥ������բ״̬02�������ź�״̬03���Ա�״̬04
        /// <returns></returns>
        public bool ClearBwState(int int_StateType)
        {
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                byte[] byt_SendData = this.OrgFrameForClearState(0xC2, Convert.ToByte(int_StateType));
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(200, 200);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }


        #endregion


        #region C0H��	��ȡ��λ������������״̬

        #region ����������

        /// <summary>
        /// ��ȡ���б�λһ��������Ϣ
        /// </summary>
        /// <param name="bln_Result">�Ƿ�������</param>
        /// <param name="byt_TaskType">�춨���ͣ�0x00������0x01������0x02�ռ�ʱ��0x03���ּ�����0x04�Աꡢ0x05Ԥ���ѹ��ܼ춨</param>
        /// <param name="int_Sum">��Ӧ��λ��������</param>
        /// <param name="str_Data">��Ӧ��λ�����ֵ</param>
        /// <returns></returns>
        public bool ReadData(int byt_TaskType,ref bool[] bln_Result, ref int[] int_Sum, ref string[] str_Data)
        {

            this.m_str_LostMessage = "";
            try
            {
                if (this.m_int_ListTable == null || this.m_int_ListCount <= 0)
                {
                    this.m_str_LostMessage = "������������û�б�λ�б�";
                    return false;
                }

                bool bln_IsOK = false;       //ֻҪ��һ����OK��OK

                byte[] byt_RevData = new byte[0];
                for (int int_Inc = 0; int_Inc < this.m_int_ListCount; int_Inc++)
                {


                    if (m_str_BwSelect.Substring(m_str_BwSelect.Length - this.m_int_ListTable[int_Inc], 1) == "1")
                    {

                        ReadData188L(this.m_int_ListTable[int_Inc], byt_TaskType);//
                        Waiting(300, 50);

                        if (this.m_byt_RevData.Length > 0)
                        {

                            if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //��鷵��֡�Ƿ�����
                            {


                                if (GetData188L(byt_RevData, ref bln_Result, ref int_Sum, ref str_Data))
                                {

                                    bln_IsOK = true;
                                }

                            }

                        }
                    }

                }


                return bln_IsOK;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// ��ȡ��λ������������״̬
        /// </summary>
        /// <param name="int_Num">��λ��(255���б�λ)FF ȫ��λ��0xEE,ż����0xDD����</param>
        /// <param name="byt_TaskType">�춨���ͣ�0x00������0x01������0x02�ռ�ʱ��0x03���ּ�����0x04�Աꡢ0x05Ԥ���ѹ��ܼ춨</param>
        /// <param name="bln_Result">��ȡ�Ƿ�ɹ�</param>
        /// <param name="int_Sum">������</param>
        /// <param name="str_Data">���ֵ</param>
        /// <returns></returns>
        public bool ReadData(int byt_TaskType, int int_Num, ref bool bln_Result, ref int int_Sum, ref string str_Data)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                if (Array.IndexOf(this.m_int_ListTable, int_Num) == -1)
                {
                    this.m_str_LostMessage = "�˱�λ�Ų��������ϣ�";
                    return false;
                }
                byte[] byt_SendData;
                if (int_Num == 255)
                {

                    byt_SendData = this.OrgFrame(0xC0, Convert.ToByte(byt_TaskType)); //34H
                }
                else
                {
                    byt_SendData = this.OrgFrameForReadWcb(0xC0, Convert.ToByte(byt_TaskType), Convert.ToByte(int_Num)); //34H
                }
                bln_Result = false;
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(500, 300);
                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //��鷵��֡�Ƿ�����
                {
                    //len=6
                    //Data����֯��ʽΪ���㲥��־(0xFFH) +��1 byte ListLen�� + ��12 bytes List��20 
                    //+ �춨������ͣ�1Byte�� 21
                    //+ ��ǰ��λ��ţ�1Byte��22
                    //+��������1Byte�� 23
                    //+ ���ֵ��4Bytes�� 27
                    //+ ״̬���ͣ�1Byte�� 28
                    //+ ������·״̬��1Byte��29 
                    //+ ��ѹ��·״̬��1Byte�� 30
                    //+ ͨѶ��״̬��1Byte�� 31
                    //+ ����״̬��1Byte��32
                    //+ ���ͱ�־1+���ͱ�־2��
                    //

                    if (byt_RevData[4] == 0x50 && byt_RevData[20] == Convert.ToByte(int_Num))//����36H
                    {
                        str_Data = Convert.ToString(byt_RevData[6]);

                        int int_Dot = 0;//= byt_RevData[23] >> 7;//�Ŵ���
                        byte[] byt_Tmp = new byte[4];
                        Array.Copy(byt_RevData, 23, byt_Tmp, 1, 3);
                        Array.Reverse(byt_Tmp);
                        if ((byt_RevData[22] >> 8 & 1) == 1)//f
                        {
                            int_Dot = byt_RevData[22] - 0x80;
                            str_Data = Convert.ToString(0 - BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, int_Dot));
                        }
                        else//z
                        {
                            int_Dot = byt_RevData[22];
                            str_Data = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, int_Dot));
                        }
                        int_Sum = byt_RevData[20];
                        bln_Result = true;
                        return true;
                    }
                    else
                    {
                        this.m_str_LostMessage = "����ʧ��ָ�";
                        return false;
                    }
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }
        #endregion

        #region ������״̬
        /// <summary>
        /// ���������Ϣ ���߹���״̬��Bit0����Ԥ������բ״̬��Bit1���������ź�״̬��Bit2�����Ա�״̬��Bit3��
        /// һ���ֽڣ���λ��
        /// ���߹���״̬��Bit0����Ԥ������բ״̬��Bit1���������ź�״̬��Bit2����
        /// �Ա�״̬��Bit3���Ĳ����ֱ���һ���ֽ��е�Bit0��Bit1��Bit2��Bit3��ʾ��Ϊ1���ʾ�ñ�λ�й���/��բ/����/�Ա���ɣ�
        /// Ϊ0���ʾ����/����/����/δ��ɶԱꡣ
        /// </summary>�����λ����λ����    by Zhoujl
        /// <param name="bln_Result">�Ƿ���״̬</param>
        /// <param name="str_Data">��Ӧ��λ��״̬</param>
        /// <returns></returns>  
        public bool ReadData(ref bool[] bln_Result, ref string[] str_Data)
        {

            this.m_str_LostMessage = "";
            try
            {
                if (this.m_int_ListTable == null || this.m_int_ListCount <= 0)
                {
                    this.m_str_LostMessage = "������������û�б�λ�б�";
                    return false;
                }

                bool bln_IsOK = false;       //ֻҪ��һ����OK��OK

                byte[] byt_RevData = new byte[0];

                for (int int_Inc = 0; int_Inc < this.m_int_ListCount; int_Inc++)
                {


                    if (m_str_BwSelect.Substring(m_str_BwSelect.Length - this.m_int_ListTable[int_Inc], 1) == "1")
                    {
                        ReadData188L(this.m_int_ListTable[int_Inc], 0x00);//��״̬ʱ���������õ������
                        Waiting(100, 50);

                        if (this.m_byt_RevData.Length > 0)
                        {
                            if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //��鷵��֡�Ƿ�����
                            {
                                if (GetData188LState(byt_RevData, ref bln_Result, ref str_Data))
                                    bln_IsOK = true;
                            }
                        }
                    }

                }


                return bln_IsOK;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }
        #endregion

        #region ˽������֡����������
        /// <summary>
        /// ���Ͷ�ȡ������Ϣָ�A0��
        /// </summary>
        /// <param name="int_Num">��λ��(255���б�λ)FF ȫ��λ��0xEE,ż����0xDD����</param>
        /// <param name="byt_TaskType">�춨���ͣ�0x00������0x01������0x02�ռ�ʱ��0x03���ּ�����0x04�Աꡢ0x05Ԥ���ѹ��ܼ춨</param>
        /// <returns></returns>
        private bool ReadData188L(int int_Num, int byt_TaskType)
        {
            if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                this.m_Ispt_com.Setting = this.m_str_Setting;
            if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                this.m_Ispt_com.Channel = this.m_int_Channel;

            this.m_Ispt_com.Setting = this.m_str_Setting;
            byte[] byt_SendData;
            if (int_Num == 255)
                byt_SendData = this.OrgFrame(0xC0, Convert.ToByte(byt_TaskType)); //C0H �㲥
            else
                byt_SendData = this.OrgFrameForReadWcb(0xC0, Convert.ToByte(byt_TaskType), Convert.ToByte(int_Num)); //C0H �ǹ㲥

            this.m_byt_RevData = new byte[0];
            this.m_Ispt_com.SendData(byt_SendData);

            return true;
        }

        /// <summary>
        /// �������巵�ص��������
        /// </summary>
        /// <param name="byt_RevData">���巵�ص�����</param>
        /// <param name="bln_Result">�Ƿ�������</param>       
        /// <param name="str_Data">��Ӧ��λ������״ֵ̬</param>
        /// <returns></returns>
        private bool GetData188L(byte[] byt_RevData, ref bool[] bln_Result, ref int[] int_Sum, ref string[] str_Data)
        {
            int meterSt = byt_RevData[20];//��ǰ�������ݵı�λ��
            int errorCount = byt_RevData[21];//������
            string errorMassage = null;//���ֵ ��4Bytes��

            int int_Dot = 0;//= byt_RevData[22] >> 7;//�Ŵ���
            byte[] byt_Tmp = new byte[4];
            Array.Copy(byt_RevData, 23, byt_Tmp, 1, 3);
            Array.Reverse(byt_Tmp);
            if (byt_RevData[22] >= 0x80)//����
            {
                int_Dot = byt_RevData[22] - 0x80;
                errorMassage = Convert.ToString(0 - BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, int_Dot));
                string s = Convert.ToString(0 - BitConverter.ToInt32(byt_Tmp, 0));
            }
            else//����
            {
                int_Dot = byt_RevData[22];
                errorMassage = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, int_Dot));
                string s = Convert.ToString(0 - BitConverter.ToInt32(byt_Tmp, 0));
            }

            bln_Result[meterSt - 1] = true;
            int_Sum[meterSt - 1] = errorCount;
            str_Data[meterSt - 1] = errorMassage;
            return true;
        }

        /// <summary>
        /// �������巵�ص�״̬����  ���߹���״̬��Bit0����Ԥ������բ״̬��Bit1���������ź�״̬��Bit2�����Ա�״̬��Bit3��
        /// </summary> by Zhoujl
        /// <param name="byt_RevData">���巵�ص�����</param>
        /// <param name="bln_Result">�Ƿ�������</param>       
        /// <param name="str_Data">��Ӧ��λ������״ֵ̬</param>
        /// <returns></returns>
        private bool GetData188LState(byte[] byt_RevData, ref bool[] bln_Result, ref string[] str_Data)
        {
            int meterSt = byt_RevData[20];//��ǰ�������ݵı�λ��
            int errorCount = byt_RevData[21];//������
            string errorMassage = null;// ��״ֵ̬��

            //��16����ת��Ϊ8λ�����ƣ������λ����
            errorMassage = Convert.ToString(byt_RevData[26], 2).PadLeft(8, '0');


            bln_Result[meterSt - 1] = true;
            str_Data[meterSt - 1] = errorMassage;
            return true;
        }
        #endregion

        #endregion


        #region C3H��	��ȡ��λǰ10�θ���������ǰ����״̬
        /// <summary>
        /// ��ȡ10����������(��ʮ��)
        /// </summary>
        /// <param name="byt_TaskType">�춨���ͣ�0x00������0x01������0x02�ռ�ʱ��0x03���ּ�����0x04�Աꡢ0x05Ԥ���ѹ��ܼ춨</param>
        /// <param name="bln_Result">�Ƿ�������</param>
        /// <param name="int_Sum">��Ӧ��λ��������</param>
        /// <param name="str_Data">��Ӧ��λ�����ֵ</param>
        /// <returns></returns>
        public bool ReadDataTenTime(int byt_TaskType,ref bool[] bln_Result, ref int[] int_Sum, ref string[] str_Data)
        {

            this.m_str_LostMessage = "";
            try
            {
                if (this.m_int_ListTable == null || this.m_int_ListCount <= 0)
                {
                    this.m_str_LostMessage = "������������û�б�λ�б�";
                    return false;
                }

                bool bln_IsOK = false;       //ֻҪ��һ����OK��OK

                byte[] byt_RevData = new byte[0];
                for (int int_Inc = 0; int_Inc < this.m_int_ListCount; int_Inc++)
                {
                    if (m_str_BwSelect.Substring(m_str_BwSelect.Length - this.m_int_ListTable[int_Inc], 1) == "1")
                    {
                        ReadData188L10Times(this.m_int_ListTable[int_Inc], byt_TaskType);
                        Waiting(300, 50);

                        if (this.m_byt_RevData.Length > 0)
                        {
                            if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //��鷵��֡�Ƿ�����
                            {
                                if (GetData188L10Times(byt_RevData, ref bln_Result, ref int_Sum, ref str_Data))
                                    bln_IsOK = true;
                            }
                        }

                    }

                }

                return bln_IsOK;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        #region ˽������֡����������

        /// <summary>
        /// ���Ͷ�ȡ������Ϣָ�C3H,����10����
        /// </summary>
        /// <param name="int_Num">ָ���Ƿ�㲥��־��FF ��255�� ��ʾ�㲥</param>
        /// <param name="byt_TaskType">�춨���ͣ�0x00������0x01������0x02�ռ�ʱ��0x03���ּ�����0x04�Աꡢ0x05Ԥ���ѹ��ܼ춨</param>
        /// <returns></returns>
        private bool ReadData188L10Times(int int_Num,int byt_TaskType)
        {
            if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                this.m_Ispt_com.Setting = this.m_str_Setting;
            if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                this.m_Ispt_com.Channel = this.m_int_Channel;
            byte[] byt_SendData;
            if (int_Num == 255)
                byt_SendData = this.OrgFrame(0xC3, Convert.ToByte(byt_TaskType)); //C0H �㲥
            else
                byt_SendData = this.OrgFrameForReadWcb(0xC3, Convert.ToByte(byt_TaskType), Convert.ToByte(int_Num)); //C0H �ǹ㲥

            this.m_byt_RevData = new byte[0];
            this.m_Ispt_com.SendData(byt_SendData);

            return true;
        }
        /// <summary>
        /// ��������10�����ص�����
        /// </summary>
        /// <param name="byt_RevData">���巵�ص�����</param>
        /// <param name="bln_Result">�Ƿ�������</param>
        /// <param name="int_Sum">��Ӧ��λ��������</param>
        /// <param name="str_Data">��Ӧ��λ�����ֵ</param>
        /// <returns></returns>
        private bool GetData188L10Times(byte[] byt_RevData, ref bool[] bln_Result, ref int[] int_Sum, ref string[] str_Data)
        {
            try
            {
                int meterSt = byt_RevData[20];//��ǰ�������ݵı�λ��
                int errorCount = byt_RevData[21];//������
                string errorMassage = null;//���ֵ ��4Bytes��
                string strError = "";

                for (int i = 0; i < 10; i++)
                {

                    int int_Dot = 0;//= byt_RevData[22] >> 7;//�Ŵ���
                    byte[] byt_Tmp = new byte[4];
                    Array.Copy(byt_RevData, (23 + i * 4), byt_Tmp, 1, 3);

                    Array.Reverse(byt_Tmp);
                    if (byt_RevData[22 + (i * 4)] >= 0x80)//����
                    {
                        int_Dot = byt_RevData[22 + (i * 4)] - 0x80;
                        errorMassage = string.Format("{0:F5}", (0 - BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, int_Dot)));
                    }
                    else//����
                    {
                        int_Dot = byt_RevData[22 + (i * 4)];
                        errorMassage = string.Format("{0:F5}", BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, int_Dot));
                    }


                    strError += errorMassage + "|";
                }
                bln_Result[meterSt - 1] = true;
                int_Sum[meterSt - 1] = errorCount;
                str_Data[meterSt - 1] = strError;
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #endregion


        #region ��չ����

        /// <summary>
        /// ִ��������չָ��(�з���ָ��)
        /// </summary>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Data">����</param>
        /// <param name="bln_RevResult">���ؽ���</param>
        /// <param name="byt_RevData">��������</param>
        /// <param name="int_Scend">�ȴ�ʱ��</param>
        /// <returns></returns>
        public bool ExeOtherCmd(byte byt_Cmd, byte[][] byt_Data, ref bool[] bln_RevResult, ref byte[][] byt_RevData, int int_Scend)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                if (this.m_int_ListTable == null || this.m_int_ListCount <= 0)
                {
                    this.m_str_LostMessage = "������������û�б�λ�б�";
                    return false;
                }

                bool bln_IsOK = false;       //ֻҪ��һ����OK��OK
                for (int int_Inc = 0; int_Inc < this.m_int_ListCount; int_Inc++)
                {
                    bln_RevResult[this.m_int_ListTable[int_Inc] - 1] = this.ExeOtherCmd(this.m_int_ListTable[int_Inc], byt_Cmd, byt_Data[this.m_int_ListTable[int_Inc] - 1], ref byt_RevData[this.m_int_ListTable[int_Inc] - 1], int_Scend);
                    if (bln_RevResult[this.m_int_ListTable[int_Inc] - 1])
                        bln_IsOK = true;
                }
                return bln_IsOK;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }


        }




        /// <summary>
        /// ִ��������չָ��(����λ,�з���ָ��)
        /// </summary>
        /// <param name="int_Num">��λ</param>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Data">����</param>
        /// <param name="byt_RevData">����ָ��</param>
        ///  <param name="int_Scend">�ȴ�ʱ��</param>
        /// <returns></returns>
        public bool ExeOtherCmd(int int_Num, byte byt_Cmd, byte[] byt_Data, ref byte[] byt_RevData, int int_Scend)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                if (Array.IndexOf(this.m_int_ListTable, int_Num) == -1)
                {
                    this.m_str_LostMessage = "�˱�λ�Ų��������ϣ�";
                    return false;
                }
                byte[] byt_SendData = this.OrgFrame(byt_Cmd, byt_Data);
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(int_Scend, 200);
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //��鷵��֡�Ƿ�����
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// ִ��������չָ��(�޷���ָ��)
        /// </summary>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Data">����</param>
        /// <param name="int_Scend">���ʱ��</param>
        /// <returns></returns>
        public bool ExeOtherCmd(byte byt_Cmd, byte[] byt_Data, int int_Scend)
        {

            for (int int_Int = 0; int_Int < this.m_int_ListCount; int_Int++)
            {
                this.ExeOtherCmd(this.m_int_ListTable[int_Int], byt_Cmd, byt_Data, int_Scend);
            }
            return true;
        }

        /// <summary>
        /// ִ��������չָ��(����λ,�޷���ָ��)
        /// </summary>
        /// <param name="int_Num">��λ</param>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Data">����</param>
        /// <param name="int_Scend">���ʱ��</param>
        /// <returns></returns>
        public bool ExeOtherCmd(int int_Num, byte byt_Cmd, byte[] byt_Data, int int_Scend)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                if (Array.IndexOf(this.m_int_ListTable, int_Num) == -1)
                {
                    this.m_str_LostMessage = "�˱�λ�Ų��������ϣ�";
                    return false;
                }
                byte[] byt_SendData = this.OrgFrame(byt_Cmd, byt_Data);
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(int_Scend, 200);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        #endregion

        #endregion


        #region---˽��-------------------------

        private void m_Ispt_com_DataReceive(byte[] bData)
        {
            int int_Len = bData.Length;
            int int_OldLen = this.m_byt_RevData.Length;
            Array.Resize(ref this.m_byt_RevData, int_Len + int_OldLen);
            Array.Copy(bData, 0, this.m_byt_RevData, int_OldLen, int_Len);
        }

        /// <summary>
        /// ���������˿�
        /// </summary>
        /// <param name="bData"></param>
        private void AdaptCom_DataReceive(byte[] bData)
        {
            int int_Len = bData.Length;
            int int_OldLen = this.m_byt_RevData.Length;
            Array.Resize(ref this.m_byt_RevData, int_Len + int_OldLen);
            Array.Copy(bData, 0, this.m_byt_RevData, int_OldLen, int_Len);
        }

        /// <summary>
        /// �ȴ����ݷ���
        /// </summary>
        /// <param name="int_MinSecond">�ȴ�����ʱ��</param>
        /// <param name="int_SpaceMSecond">�ȴ�����֡�ֽڼ��ʱ��</param>
        private void Waiting(int int_MinSecond, int int_SpaceMSecond)
        {
            try
            {
                int int_OldLen = 0;
                Stopwatch sth_Ticker = new Stopwatch();                     //�ȴ���ʱ��
                Stopwatch sth_SpaceTicker = new Stopwatch();                //
                sth_SpaceTicker.Start();
                sth_Ticker.Start();
                while (this.m_bln_Enabled)
                {
                    System.Windows.Forms.Application.DoEvents();
                    if (this.m_byt_RevData.Length > int_OldLen)     //�����иı�
                    {
                        sth_SpaceTicker.Reset();
                        int_OldLen = this.m_byt_RevData.Length;
                        sth_SpaceTicker.Start();                    //�ֽڼ��ʱ���¿�ʼ
                    }
                    else        //���������û�����ӣ���ǰ���յ�����ʱ���500�������˳�
                    {
                        if (this.m_byt_RevData.Length > 0)      //�Ѿ��յ�һ���֣����ֽڼ��ʱ
                        {
                            if (sth_SpaceTicker.ElapsedMilliseconds >= int_SpaceMSecond)
                                break;
                        }
                    }
                    if (sth_Ticker.ElapsedMilliseconds >= int_MinSecond)        //�ܼ�ʱ
                        break;
                    System.Threading.Thread.Sleep(1);
                }
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.Message;
            }
        }


        /// <summary>
        /// ������λͨ���ͱ�λѡ���ַ�����õ�ǰͨ���ı�λѡ���ַ���
        /// </summary>
        /// <param name="intChannel">ͨ����</param>
        /// <param name="BwSelectString">ȫ����λѡ����ַ���"1111110000....."</param>
        /// <returns>��ǰͨ���ϵı�λѡ���ַ�����������λ��0</returns>
        private string GetBwListstring(int intChannel, string BwSelectString)
        {
            string strtmp1 = "";
            string strtmp2 = BwSelectString;

            for (int i = 0; i < this.m_int_Bws; i++)
            {
                if (this.m_int_Bws - 1 - i < this.m_int_ListTable[0] - 1 || this.m_int_Bws - 1 - i > m_int_ListTable[m_int_ListTable.Length - 1] - 1)
                {
                    strtmp1 = strtmp1 + "0";

                }
                else
                {
                    strtmp1 = strtmp1 + strtmp2.Substring(i, 1); //strtmp2.Substring(0, strtmp2.Length - i - 1) + "0" + strtmp2.Substring(strtmp2.Length - 1 - i, i);
                }
            }
            return strtmp1;

        }

        /// <summary>
        /// ������λͨ���ͱ�λѡ���ַ�����õ�ǰͨ������ż��λѡ���ַ���0=������1=ż��
        /// </summary>
        /// <param name="intChannel">ͨ����</param>
        /// <param name="BwSelectString">ȫ����λѡ����ַ���"1111110000....."</param>
        /// <returns>��ǰͨ���ϵı�λѡ���ַ�����������λ��0</returns>
        private string GetBwListstring(int intChannel, int int_type, string BwSelectString)
        {
            string strtmp1 = "";
            string strtmp2 = BwSelectString;
            if (int_type == 0)
            {
                for (int i = 0; i < this.m_int_Bws; i++)
                {
                    if (i % 2 == 0)
                    {
                        strtmp1 = strtmp1 + "1";

                    }
                    else
                    {

                        strtmp1 = strtmp1 + "0";
                    }

                }
            }
            else
            {
                for (int i = 0; i < this.m_int_Bws; i++)
                {
                    if (i % 2 == 0)
                    {
                        strtmp1 = strtmp1 + "0";

                    }
                    else
                    {

                        strtmp1 = strtmp1 + "1";
                    }

                }

            }
            return strtmp1;

        }

        /// <summary>
        /// ������λ������
        /// </summary>
        /// <param name="intChannel"></param>
        /// <param name="byt_bwnum"></param>
        /// <param name="BwSelectString"></param>
        /// <returns></returns>
        private string GetBwListstring(int intChannel, byte byt_bwnum, string BwSelectString)
        {
            string strtmp = BwSelectString;
            string str_tmp1 = "";
            string str_tmp2 = "";
            int int_tmp = 0;
            try
            {
                for (int i = 0; i < this.m_int_Bws; i++)
                {
                    if (i == (byt_bwnum - 1))
                    {
                        int_tmp = strtmp.Length - i - 1;

                        str_tmp1 = strtmp.Remove(int_tmp, 1);

                        strtmp = str_tmp1.Insert(int_tmp, "1");
                    }
                    else
                    {

                        int_tmp = strtmp.Length - i - 1;

                        str_tmp1 = strtmp.Remove(int_tmp, 1);

                        strtmp = str_tmp1.Insert(int_tmp, "0");

                    }
                }
                return strtmp;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return "";
            }


        }

        /// <summary>
        /// ��"01011001011"��01���з�ת0��1 ��1��0
        /// </summary>
        /// <param name="str01"></param>
        /// <returns></returns>
        private string GetChange01(string str01)
        {
            string strtmp = "";
            for (int i = 0; i < str01.Length; i++)
            {

                if (str01.Substring(i, 1) == "0")
                {
                    strtmp = strtmp + "1";
                }
                else
                {
                    strtmp = strtmp + "0";
                }

            }
            return strtmp;

        }

        /// <summary>
        /// ת���ַ�����ʽ01010����ֽ�
        /// </summary>
        /// <param name="strtmp">96λ��"0001100010101......"</param>
        /// <returns></returns>
        private byte[] ChangeStringToByte(string strtmp)
        {
            byte[] Arrytmpbyte = new byte[12];

            byte tmpbyte = new byte();
            for (int i = 0; i < 12; i++)
            {
                tmpbyte = 0x00;
                for (int k = 0; k < 8; k++)
                {
                    tmpbyte += Convert.ToByte(strtmp.Substring(strtmp.Length - 1 - 8 * i - k, 1).Equals("1") ? (Math.Pow(2, k)) : 0x00);
                }
                Arrytmpbyte[11 - i] = tmpbyte;
            }
            return Arrytmpbyte;

        }
        #region ��֡


        /// <summary>
        ///  ��֯֡
        /// </summary>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Data">������</param>
        /// <returns>������õ�֡</returns>
        private byte[] OrgFrame(byte byt_Cmd, byte[] byt_Data)
        {
            int int_Len = 20 + byt_Data.Length;                   //81��ID��len��CMD��Chksum��ռһλ +������λ
            byte[] byt_Frame = new byte[int_Len];
            byt_Frame[0] = 0x81;                                    //81
            byt_Frame[1] = Convert.ToByte(this.m_str_ID, 16);       //ID
            byt_Frame[2] = 05;                                      //SD
            byt_Frame[3] = Convert.ToByte(int_Len);                 // Len
            byt_Frame[4] = byt_Cmd;                                 //Cmd
            byt_Frame[5] = 0xFF;                                    //�㲥ָ���־
            byt_Frame[6] = Convert.ToByte(this.m_int_Bws);          //��λ��

            byte[] tmparrbyte;
            tmparrbyte = ChangeStringToByte(m_str_BwSelect);        //Ҫ��
            Array.Copy(tmparrbyte, 0, byt_Frame, 7, tmparrbyte.Length);

            if (byt_Data.Length > 0) Array.Copy(byt_Data, 0, byt_Frame, 19, byt_Data.Length);

            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //У����    У�����81���濪ʼ����
            {
                byt_Frame[int_Len - 1] ^= byt_Frame[int_Inc];               //Chksum
            }
            return byt_Frame;

        }
        /// <summary>
        ///  ��֯֡ ����λ��
        /// </summary>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Data">������</param>
        /// <returns>������õ�֡</returns>
        private byte[] OrgFrame(byte byt_Cmd, byte[] byt_Data, byte byt_BwNum)
        {
            int int_Len = 20 + byt_Data.Length;                   //81��ID��len��CMD��Chksum��ռһλ +������λ
            byte[] byt_Frame = new byte[int_Len];
            byt_Frame[0] = 0x81;                                    //81
            byt_Frame[1] = Convert.ToByte(this.m_str_ID, 16);       //ID
            byt_Frame[2] = 05;                                      //SD
            byt_Frame[3] = Convert.ToByte(int_Len);                 // Len
            byt_Frame[4] = byt_Cmd;                                 //Cmd
            byt_Frame[5] = 0xFF;                                    //�㲥ָ���־
            byt_Frame[6] = Convert.ToByte(this.m_int_Bws);          //ListLen

            byte[] tmparrbyte;
            tmparrbyte = ChangeStringToByte(m_str_BwSelect);
            Array.Copy(tmparrbyte, 0, byt_Frame, 7, tmparrbyte.Length);

            if (byt_Data.Length > 0) Array.Copy(byt_Data, 0, byt_Frame, 19, byt_Data.Length);

            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //У����    У�����81���濪ʼ����
            {
                byt_Frame[int_Len - 1] ^= byt_Frame[int_Inc];               //Chksum
            }
            return byt_Frame;

        }


        /// <summary>
        /// ��֯֡
        /// </summary>
        /// <param name="byt_Cmd">������</param>
        /// <returns>������õ�֡</returns>
        private byte[] OrgFrame(byte byt_Cmd)
        {
            int int_Len = 20;// + byt_Data.Length;                   //81��ID��SD��len��CMD��Chksum+14��ռһλ +������λ
            byte[] byt_Data = new byte[20];
            byt_Data[0] = 0x81;                                  //81
            byt_Data[1] = Convert.ToByte(this.m_str_ID, 16);    //ID
            byt_Data[2] = 0x05;                                   //SD
            byt_Data[3] = Convert.ToByte(int_Len);                     //Len
            byt_Data[4] = byt_Cmd;                          //Cmd
            byt_Data[5] = 0xFF;                            //�㲥ָ���־
            byt_Data[6] = Convert.ToByte(this.m_int_Bws);//ListLen

            byte[] tmparrbyte;

            //tmparrbyte = ChangeStringToByte(GetBwListstring(m_int_Channel, m_str_BwSelect));
            tmparrbyte = ChangeStringToByte(m_str_BwSelect);
            Array.Copy(tmparrbyte, 0, byt_Data, 7, tmparrbyte.Length);

            for (int int_Inc = 1; int_Inc < 19; int_Inc++)
            {
                byt_Data[19] ^= byt_Data[int_Inc];               //Chksum
            }
            return byt_Data;



        }

        /// <summary>
        ///  ��֯֡
        /// </summary>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Value">������</param>
        /// <returns>������õ�֡</returns>
        private byte[] OrgFrame(byte byt_Cmd, byte byt_Value)
        {

            int int_Len = 21;                  //81��ID��SD \len��CMD��Chksum��ռһλ +������λ
            byte[] byt_Data = new byte[int_Len];
            byt_Data[0] = 0x81;                                  //81
            byt_Data[1] = Convert.ToByte(this.m_str_ID, 16);     //ID

            byt_Data[2] = 0x05;                                  //SD
            byt_Data[3] = Convert.ToByte(int_Len);               //Len

            byt_Data[4] = byt_Cmd;                              //Cmd

            byt_Data[5] = 0xFF;                                 //�㲥ָ���־

            byt_Data[6] = Convert.ToByte(this.m_int_Bws);       //ListLen

            byte[] tmparrbyte;

            tmparrbyte = ChangeStringToByte(m_str_BwSelect);
            Array.Copy(tmparrbyte, 0, byt_Data, 7, tmparrbyte.Length);

            byt_Data[19] = byt_Value;

            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //У����    У�����81���濪ʼ����
            {
                byt_Data[int_Len - 1] ^= byt_Data[int_Inc];               //Chksum
            }
            return byt_Data;

        }

        /// <summary>
        ///  ��֯֡For������
        /// </summary>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Value">������</param>
        ///  <param name="byt_BwNum">��λ��</param>
        /// <returns>������õ�֡</returns>
        private byte[] OrgFrame(byte byt_Cmd, byte byt_Value, byte byt_BwNum)
        {

            int int_Len = 21;                   //81��ID��SD \len��CMD��Chksum��ռһλ +������λ
            byte[] byt_Data = new byte[int_Len];
            byt_Data[0] = 0x81;                                 //81
            byt_Data[1] = Convert.ToByte(this.m_str_ID, 16);    //ID

            byt_Data[2] = 0x05;                                 //SD
            byt_Data[3] = Convert.ToByte(int_Len);              //Len

            byt_Data[4] = byt_Cmd;                              //Cmd

            byt_Data[5] = 0xFF;                                 //�㲥ָ���־

            byt_Data[6] = Convert.ToByte(this.m_int_Bws);       //ListLen

            byte[] tmparrbyte;

            //tmparrbyte = ChangeStringToByte(GetBwListstring(m_int_Channel, byt_BwNum, m_str_BwSelect));
            tmparrbyte = ChangeStringToByte(m_str_BwSelect);
            Array.Copy(tmparrbyte, 0, byt_Data, 7, tmparrbyte.Length);

            byt_Data[19] = byt_Value;

            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //У����    У�����81���濪ʼ����
            {
                byt_Data[int_Len - 1] ^= byt_Data[int_Inc];               //Chksum
            }
            return byt_Data;

        }


        //*****************************************

        /// <summary>
        /// ��֯֡Ϊ����
        /// </summary>
        /// <param name="byt_Cmd">������</param>
        /// <returns>������õ�֡</returns>
        private byte[] OrgFrameForGL(byte byt_Cmd)
        {
            int int_Len = 20;// + byt_Data.Length;               //81��ID��SD��len��CMD��Chksum+14��ռһλ +������λ
            byte[] byt_Data = new byte[20];
            byt_Data[0] = 0x81;                                  //81
            byt_Data[1] = Convert.ToByte(this.m_str_ID, 16);     //ID
            byt_Data[2] = 0x05;                                  //SD
            byt_Data[3] = Convert.ToByte(int_Len);               //Len
            byt_Data[4] = byt_Cmd;                          //Cmd
            byt_Data[5] = 0xFF;                            //�㲥ָ���־
            byt_Data[6] = Convert.ToByte(this.m_int_Bws);//ListLen

            byte[] tmparrbyte;
            string str_BwGL;//
            //str_BwGL = GetBwListstring(m_int_Channel, m_str_BwSelect);
            str_BwGL = GetChange01(m_str_BwSelect);
            tmparrbyte = ChangeStringToByte(str_BwGL);
            Array.Copy(tmparrbyte, 0, byt_Data, 7, tmparrbyte.Length);

            for (int int_Inc = 1; int_Inc < 19; int_Inc++)
            {
                byt_Data[19] ^= byt_Data[int_Inc];               //Chksum
            }
            return byt_Data;



        }

        /// <summary>
        ///  ��֯֡
        /// </summary>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Data">������</param>
        /// <returns>������õ�֡</returns>
        private byte[] OrgFrameForTimePulse(byte byt_Cmd, byte[] byt_Data)
        {
            int int_Len = 20 + byt_Data.Length;                   //81��ID��len��CMD��Chksum��ռһλ +������λ
            byte[] byt_Frame = new byte[int_Len];
            byt_Frame[0] = 0x81;                                  //81
            byt_Frame[1] = Convert.ToByte(this.m_str_ID, 16);    //ID
            byt_Frame[2] = 05;                                 //SD
            byt_Frame[3] = Convert.ToByte(int_Len);          // Len
            byt_Frame[4] = byt_Cmd;                         //Cmd

            byt_Frame[5] = 0xFF;                            //�㲥ָ���־
            byt_Frame[6] = Convert.ToByte(this.m_int_Bws);//ListLen

            byte[] tmparrbyte = { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };
            //tmparrbyte = ChangeStringToByte(m_str_BwSelect);
            //tmparrbyte = ChangeStringToByte(m_str_BwSelect);
            //tmparrbyte = ChangeStringToByte(GetBwListstring(m_int_Channel, m_str_BwSelect));

            Array.Copy(tmparrbyte, 0, byt_Frame, 7, tmparrbyte.Length);

            if (byt_Data.Length > 0) Array.Copy(byt_Data, 0, byt_Frame, 19, byt_Data.Length);

            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //У����    У�����81���濪ʼ����
            {
                byt_Frame[int_Len - 1] ^= byt_Frame[int_Inc];               //Chksum
            }
            return byt_Frame;

        }

        /// <summary>
        ///  ��֯֡
        /// </summary>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Data">������</param>
        /// <returns>������õ�֡</returns>
        private byte[] OrgFrameForDLSwich(byte byt_Cmd, byte[] byt_Data)
        {
            int int_Len = 20 + byt_Data.Length;                   //81��ID��len��CMD��Chksum��ռһλ +������λ
            byte[] byt_Frame = new byte[int_Len];
            byt_Frame[0] = 0x81;                                  //81
            byt_Frame[1] = Convert.ToByte(this.m_str_ID, 16);    //ID
            byt_Frame[2] = 05;                                 //SD
            byt_Frame[3] = Convert.ToByte(int_Len);          // Len
            byt_Frame[4] = byt_Cmd;                         //Cmd

            byt_Frame[5] = 0xFF;                            //�㲥ָ���־
            byt_Frame[6] = Convert.ToByte(this.m_int_Bws);//ListLen

            byte[] tmparrbyte = { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };
            //tmparrbyte = ChangeStringToByte(m_str_BwSelect);
            //tmparrbyte = ChangeStringToByte(m_str_BwSelect);
            //tmparrbyte = ChangeStringToByte(GetBwListstring(m_int_Channel, m_str_BwSelect));

            Array.Copy(tmparrbyte, 0, byt_Frame, 7, tmparrbyte.Length);

            if (byt_Data.Length > 0) Array.Copy(byt_Data, 0, byt_Frame, 19, byt_Data.Length);

            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //У����    У�����81���濪ʼ����
            {
                byt_Frame[int_Len - 1] ^= byt_Frame[int_Inc];               //Chksum
            }
            return byt_Frame;

        }

        /// <summary>
        ///  ��֯֡
        /// </summary>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Data">������</param>
        /// <returns>������õ�֡</returns>
        private byte[] OrgFrameForClear(byte byt_Cmd, byte[] byt_Data)
        {
            int int_Len = 32 + byt_Data.Length;                   //81��ID��len��CMD��Chksum��ռһλ +������λ
            byte[] byt_Frame = new byte[int_Len];
            byt_Frame[0] = 0x81;                                  //81
            byt_Frame[1] = Convert.ToByte(this.m_str_ID, 16);    //ID
            byt_Frame[2] = 05;                                 //SD
            byt_Frame[3] = Convert.ToByte(int_Len);          // Len
            byt_Frame[4] = byt_Cmd;                         //Cmd

            byt_Frame[5] = 0xFF;                            //�㲥ָ���־
            byt_Frame[6] = Convert.ToByte(this.m_int_Bws);//ListLen

            byte[] tmparrbyte;

            //tmparrbyte = ChangeStringToByte(GetBwListstring(m_int_Channel, m_str_BwSelect));
            tmparrbyte = ChangeStringToByte(m_str_BwSelect);
            Array.Copy(tmparrbyte, 0, byt_Frame, 7, tmparrbyte.Length);

            if (byt_Data.Length > 0) Array.Copy(byt_Data, 0, byt_Frame, 19, byt_Data.Length);

            Array.Copy(tmparrbyte, 0, byt_Frame, 19 + byt_Data.Length, tmparrbyte.Length);


            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //У����    У�����81���濪ʼ����
            {
                byt_Frame[int_Len - 1] ^= byt_Frame[int_Inc];               //Chksum
            }
            return byt_Frame;

        }

        /// <summary>
        ///  ��֯֡ ����λ��
        /// </summary>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Data">������</param>
        /// <returns>������õ�֡</returns>
        private byte[] OrgFrameForReadWcb(byte byt_Cmd, byte[] byt_Data, byte byt_BwNum)
        {
            int int_Len = 20 + byt_Data.Length;                   //81��ID��len��CMD��Chksum��ռһλ +������λ
            byte[] byt_Frame = new byte[int_Len];
            byt_Frame[0] = 0x81;                                  //81
            byt_Frame[1] = Convert.ToByte(this.m_str_ID, 16);    //ID
            byt_Frame[2] = 05;                                 //SD
            byt_Frame[3] = Convert.ToByte(int_Len);          // Len
            byt_Frame[4] = byt_Cmd;                         //Cmd

            byt_Frame[5] = 0xFF;                            //�㲥ָ���־
            byt_Frame[6] = Convert.ToByte(this.m_int_Bws);//ListLen

            byte[] tmparrbyte;

            tmparrbyte = ChangeStringToByte(GetBwListstring(m_int_Channel, byt_BwNum, m_str_BwSelect));
            // tmparrbyte = ChangeStringToByte(m_str_BwSelect);
            Array.Copy(tmparrbyte, 0, byt_Frame, 7, tmparrbyte.Length);

            if (byt_Data.Length > 0) Array.Copy(byt_Data, 0, byt_Frame, 19, byt_Data.Length);

            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //У����    У�����81���濪ʼ����
            {
                byt_Frame[int_Len - 1] ^= byt_Frame[int_Inc];               //Chksum
            }
            return byt_Frame;

        }

        /// <summary>
        ///  ��֯֡����״̬
        /// </summary>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Value">������</param>
        /// <returns>������õ�֡</returns>
        private byte[] OrgFrameForClearState(byte byt_Cmd, byte byt_Value)
        {

            int int_Len = 33;                  //81��ID��SD \len��CMD��Chksum��ռһλ +������λ
            byte[] byt_Data = new byte[int_Len];
            byt_Data[0] = 0x81;                                  //81
            byt_Data[1] = Convert.ToByte(this.m_str_ID, 16);     //ID

            byt_Data[2] = 0x05;                                  //SD
            byt_Data[3] = Convert.ToByte(int_Len);               //Len

            byt_Data[4] = byt_Cmd;    //Cmd

            byt_Data[5] = 0xFF;                            //�㲥ָ���־

            byt_Data[6] = Convert.ToByte(this.m_int_Bws);//ListLen

            byte[] tmparrbyte = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };


            Array.Copy(tmparrbyte, 0, byt_Data, 7, tmparrbyte.Length);
            byt_Data[19] = byt_Value;
            byte[] tmparrbyte1 = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            Array.Copy(tmparrbyte1, 0, byt_Data, 20, tmparrbyte.Length);

            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //У����    У�����81���濪ʼ����
            {
                byt_Data[int_Len - 1] ^= byt_Data[int_Inc];               //Chksum
            }
            return byt_Data;

        }

        /// <summary>
        ///  ��֯֡ Ϊ������ϵ� 
        /// </summary>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Value">������</param>
        /// <returns>������õ�֡</returns>
        private byte[] OrgFrameForClear(byte byt_Cmd, byte byt_Value)
        {

            int int_Len = 21 + 12;                  //81��ID��SD \len��CMD��Chksum��ռһλ +������λ
            byte[] byt_Data = new byte[int_Len];
            byt_Data[0] = 0x81;                                  //81
            byt_Data[1] = Convert.ToByte(this.m_str_ID, 16);     //ID

            byt_Data[2] = 0x05;                                  //SD
            byt_Data[3] = Convert.ToByte(int_Len);               //Len

            byt_Data[4] = byt_Cmd;    //Cmd

            byt_Data[5] = 0xFF;                            //�㲥ָ���־

            byt_Data[6] = Convert.ToByte(this.m_int_Bws);//ListLen

            byte[] tmparrbyte;

            //tmparrbyte = ChangeStringToByte(GetBwListstring(m_int_Channel, m_str_BwSelect));
            tmparrbyte = ChangeStringToByte(m_str_BwSelect);
            Array.Copy(tmparrbyte, 0, byt_Data, 7, tmparrbyte.Length);

            byt_Data[19] = byt_Value;

            Array.Copy(tmparrbyte, 0, byt_Data, 20, tmparrbyte.Length);

            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //У����    У�����81���濪ʼ����
            {
                byt_Data[int_Len - 1] ^= byt_Data[int_Inc];               //Chksum
            }
            return byt_Data;

        }

        /// <summary>
        ///  ��֯֡
        /// </summary>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Value">������</param>
        /// <returns>������õ�֡</returns>
        private byte[] OrgFrameForComm(byte byt_Cmd, byte byt_Num, byte byt_Value)
        {

            int int_Len = 21;                  //81��ID��SD \len��CMD��Chksum��ռһλ +������λ
            byte[] byt_Data = new byte[int_Len];
            byt_Data[0] = 0x81;                                  //81
            byt_Data[1] = Convert.ToByte(this.m_str_ID, 16);     //ID

            byt_Data[2] = 0x05;                                  //SD
            byt_Data[3] = Convert.ToByte(int_Len);               //Len

            byt_Data[4] = byt_Cmd;    //Cmd

            byt_Data[5] = 0xFF;                            //�㲥ָ���־

            byt_Data[6] = Convert.ToByte(this.m_int_Bws);//ListLen

            byte[] tmparrbyte = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            if (byt_Num == 0xFF)//ȫ��λ
            {
                if (byt_Value != 0x03)
                {

                    tmparrbyte = ChangeStringToByte(m_str_BwSelect);
                }
            }
            else if (byt_Num == 0xEE)//ż��
            {
                tmparrbyte = ChangeStringToByte(m_str_BwSelect); //ChangeStringToByte(GetBwListstring(m_int_Channel, 0, Comm.GlobalUnit.strYaoJianMeter));

            }
            else if (byt_Num == 0xDD)//����
            {
                tmparrbyte = ChangeStringToByte(m_str_BwSelect); //ChangeStringToByte(GetBwListstring(m_int_Channel, 1, Comm.GlobalUnit.strYaoJianMeter));
            }
            else//����λ
            {
                tmparrbyte = ChangeStringToByte(GetBwListstring(m_int_Channel, byt_Num, m_str_BwSelect));
            }
            Array.Copy(tmparrbyte, 0, byt_Data, 7, tmparrbyte.Length);
            byt_Data[19] = byt_Value;

            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //У����    У�����81���濪ʼ����
            {
                byt_Data[int_Len - 1] ^= byt_Data[int_Inc];               //Chksum
            }
            return byt_Data;

        }


        /// <summary>
        ///  ��֯֡For������
        /// </summary>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Value">������</param>
        ///  <param name="byt_BwNum">��λ��</param>
        /// <returns>������õ�֡</returns>
        private byte[] OrgFrameForReadWcb(byte byt_Cmd, byte byt_Value, byte byt_BwNum)
        {

            int int_Len = 21;                  //81��ID��SD \len��CMD��Chksum��ռһλ +������λ
            byte[] byt_Data = new byte[int_Len];
            byt_Data[0] = 0x81;                                  //81
            byt_Data[1] = Convert.ToByte(this.m_str_ID, 16);    //ID

            byt_Data[2] = 0x05;                                 //SD
            byt_Data[3] = Convert.ToByte(int_Len);             //  Len

            byt_Data[4] = byt_Cmd;    //Cmd

            byt_Data[5] = 0xFF;                            //�㲥ָ���־

            byt_Data[6] = Convert.ToByte(this.m_int_Bws);//ListLen

            byte[] tmparrbyte;

            tmparrbyte = ChangeStringToByte(GetBwListstring(m_int_Channel, byt_BwNum, m_str_BwSelect));
            // tmparrbyte = ChangeStringToByte(m_str_BwSelect);
            Array.Copy(tmparrbyte, 0, byt_Data, 7, tmparrbyte.Length);

            byt_Data[19] = byt_Value;

            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //У����    У�����81���濪ʼ����
            {
                byt_Data[int_Len - 1] ^= byt_Data[int_Inc];               //Chksum
            }
            return byt_Data;

        }
        #endregion

        /// <summary>
        /// ��鷵��֡�Ϸ���
        /// </summary>
        /// <param name="byt_Data">����֡</param>
        /// <returns></returns>
        private bool CheckFrame(byte[] byt_Value, ref byte[] byt_Data)
        {
            int int_Start = 0;
            int_Start = Array.IndexOf(byt_Value, (byte)0x81);
            if (int_Start < 0 || int_Start > byt_Value.Length) return false;    //û��81��ͷ
            if (int_Start + 3 >= byt_Value.Length) return false;                //û��֡�����ֽ�
            int int_Len = byt_Value[int_Start + 3];
            if (int_Len + int_Start > byt_Value.Length) return false;           //ʵ�ʳ�����֡���Ȳ����
            byte byt_ChkSum = 0;
            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)
            {
                byt_ChkSum ^= byt_Value[int_Start + int_Inc];
            }
            if (byt_ChkSum != byt_Value[int_Start + int_Len - 1]) return false; //У���벻����
            Array.Resize(ref byt_Data, int_Len);    //�����򳤶�
            Array.Copy(byt_Value, int_Start, byt_Data, 0, int_Len);
            return true;
        }


        /// <summary>
        /// �����׼����Ƶ��
        /// </summary>
        /// <param name="StdPulse"></param>
        /// <param name="_CurP"></param>
        /// <returns></returns>
        private long GetStdPulsePl(long StdPulse, float _CurP)
        {//��׼���峣��/��3600*1000/P��
            long lngTmp;
            //lngTmp = (StdPulse / (3600 * 1000)) * (long)_CurP;
            lngTmp = Convert.ToInt64(StdPulse / (3600 * 1000 / _CurP));

            return lngTmp;
        }

        #endregion




    }


}
