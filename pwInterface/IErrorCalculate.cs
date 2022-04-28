/****************************************************************************

    ����ӿ���
    ��ΰ 2009-10

*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace pwInterface
{   
    /// <summary>
    /// �������ͨ��Э��ӿ�
    /// </summary>
    public interface IErrorCalculate
    {
        #region ������Ա

        /// <summary>
        /// ��������ַ
        /// </summary>
        string ID { get;set;}

        /// <summary>
        /// ������崮��
        /// </summary>
        ISerialport ComPort { get;set;}


        /// <summary>
        /// ͨ��ѡ����Ҫ���ڿ���CL2011�豸�ģ���Чֵ1-4,
        /// ����PC���ڵ�RTSEnable��DTREnable��4�����
        /// CL2018-1������Ч��
        /// </summary>
        int Channel { get;set;}


        /// <summary>
        /// �ز���
        /// </summary>
        string Setting { set;}

        /// <summary>
        /// ʧ����Ϣ
        /// </summary>
        string LostMessage { get;}

        /// <summary>
        /// ���������λ��
        /// </summary>
        /// <param name="int_List">�˹���ı�λ�����б�</param>
        /// <returns></returns>
        bool SetECListTable(int[] int_List);

        /// <summary>
        /// �Զ���Ӧ��׼��˿ں�
        /// </summary>
        /// <param name="mySerialPort">�˿�����</param>
        /// <returns>true ��Ӧ�ɹ���false ��Ӧʧ��</returns>
        int AdaptCom(ISerialport[] mySerialPort);


        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="bln_Result">��������</param>
        /// <returns>true �����ɹ�,false ����ʧ��</returns>
        bool Link(ref bool[] bln_Result);

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="int_Num">��λ</param>
        /// <param name="bln_Result">��������</param>
        /// <returns></returns>
        bool Link(int int_Num);
        #endregion

        #region F1H��	���õ������춨ʱ���������������Ȧ��


        /// <summary>
        /// ���ñ�������峣���ͼ춨Ȧ��
        /// </summary>
        /// <param name="lAmMeterPulseConst">��������峣��</param>
        /// <param name="iPulseCount">�춨Ȧ��</param>
        /// <param name="lStdPulseConst">��׼��ǰ��������Ҫ�ӱ�׼���ȡ</param>
        /// <param name="fStdP">��׼��ǰ���ʣ���Ҫ�ӱ�׼���ȡ</param>
        /// <param name="iAmMeterPulseBS">���������Ŵ�ϵ����-128~127����Ĭ��Ϊ1���Ŵ�Ҳ����С1</param>
        /// <returns></returns>
        bool SetDnWcrPara(long lAmMeterPulseConst, long iPulseCount, long lStdPulseConst, float fStrandMeterP, byte iAmMeterPulseBS);
        #endregion

        #region F3H��	�����ռ�ʱ���춨ʱ��Ƶ�ʼ������������춨ʱ��

        /// <summary>
        /// �ռ�ʱ���������ã�Ƶ�ʣ�Ȧ��
        /// </summary>
        /// <param name="sglMeterHz">�����ʱ��Ƶ��</param>
        /// <param name="iPulse">������</param>
        /// <returns></returns>
        bool SetTimePara( float fMeterHz,int iPulse);

        /// <summary>
        /// �ռ�ʱ���������ã���λ�ţ�������Ƶ�ʣ�Ȧ��
        /// </summary>
        /// <param name="int_Num">��λ�ţ��㲥��־(0xFFH)��żλ(0xEEH)����λ(0xDDH)</param>
        /// <param name="lStdTimeConst">��׼ʱ�����峣��</param>
        /// <param name="flt_MeterHz">����ʱ��Ƶ��</param>
        /// <param name="int_Pulse">�����������</param>
        /// <returns></returns>
        bool SetTimePara(int iNum, long lStdTimeConst, float fMeterHz, int iPulse);
        #endregion

        #region A7H��	ѡ�񱻼�����ͨ�����춨����

        /// <summary>
        /// ѡ������ͨ��
        /// </summary>
        /// <param name="iPulseChannel">����ͨ��,0=P+,1=P-,2=Q+,3=Q-,4=����,5=ʱ��</param>
        /// <param name="iChannelType">ͨ������,0=�����,1=���ͷ</param>
        /// <param name="iPulseType">��������,0=����,1=����</param>
        /// <returns></returns>
        bool SelectPulseChannel(int[] iPulseChannel, int[] iChannelType, int[] iPulseType);

        /// <summary>
        /// ѡ������ͨ��
        /// </summary>
        /// <param name="iNum">��λ</param>
        /// <param name="iPulseChannel">����ͨ��,0=P+,1=P-,2=Q+,3=Q-,4=����,5=ʱ��</param>
        /// <param name="iChannelType">ͨ������,0=�����,1=���ͷ</param>
        /// <param name="iPulseType">��������,0=����,1=����</param>
        /// <returns></returns>
        bool SelectPulseChannel(int iNum, int iPulseChannel, int iChannelType, int iPulseType);
        #endregion

        #region ACH��	���ñ�λ����

        /// <summary>
        /// ���Ʊ�λͨ�ſڿ���
        /// </summary>
        /// <param name="int_Num">��λ��(255���б�λ)</param>
        /// <param name="bln_Open">�Ƿ�򿪣�true=�򿪣�flase=�ر�</param>
        /// <returns></returns>
        bool SetCommSwitch(int int_Num, bool bln_Open);
        #endregion

        #region ADH��	ѡ�񱻱������������
        /// <summary>
        /// ѡ�񱻱�����������ͣ�0=�������ӣ�1=��������
        /// </summary>
        /// <param name="iPulseDzType">0=�������ӣ�1=��������</param>
        /// <returns></returns>
        bool SetMeterPulseDzType(int iPulseDzType);
        #endregion


        #region AFH��	����˫��·�춨ʱ��ѡ�����е�ĳһ·��Ϊ�����������·
        /// <summary>
        /// ��·�л�
        /// </summary>
        /// <param name="int_DL_type">������·�� 0һ��· 1����·</param>
        /// <returns></returns>
        bool SetDLSwitch(int int_DL_type);
        #endregion

        #region B1H��	�������㹦��ָ��

        /// <summary>
        /// ����������㹦�� ���������Ͳ���
        /// </summary>
        /// <param name="byt_TaskType">�춨���ͣ�0x00������0x01������0x02�ռ�ʱ��0x03���ּ�����0x04�Աꡢ0x05Ԥ���ѹ��ܼ춨</param>
        /// <returns></returns>
        bool StartCalculate(int byt_TaskType);
        #endregion

        #region B2H��	ֹͣ�춨����ָ��

        /// <summary>
        /// ֹͣ������㹦��
        /// </summary>
        /// <param name="byt_TaskType">�춨���ͣ�0x00������0x01������0x02�ռ�ʱ��0x03���ּ�����0x04�Աꡢ0x05Ԥ���ѹ��ܼ춨</param>
        /// <returns></returns>
        bool StopCalculate(int byt_TaskType);
        #endregion


        #region C2H��	�����λ״̬
        /// <summary>
        /// ������߹���״̬��
        /// </summary>
        /// ״̬���ͷ�Ϊ���֣����߹���״̬01��Ԥ������բ״̬02�������ź�״̬03���Ա�״̬04
        /// <returns></returns>
        bool ClearBwState(int int_StateType);
        #endregion

        #region C0H��	��ȡ��λ������������״̬

        #region ����������

        /// <summary>
        /// ��ȡ���б�λ��������
        /// </summary>
        /// <param name="byt_TaskType">�춨���ͣ�0x00������0x01������0x02�ռ�ʱ��0x03���ּ�����0x04�Աꡢ0x05Ԥ���ѹ��ܼ춨</param>
        /// <param name="bln_Result">��Ӧ��λ�Ƿ�������</param>
        /// <param name="int_Time">��Ӧ��λ��������</param>
        /// <param name="str_Data">��Ӧ��λ�����ֵ</param>
        /// <returns></returns>
        bool ReadData(int byt_TaskType, ref bool[] bln_Result, ref int[] int_Time, ref string[] str_Data);

        /// <summary>
        /// ��ȡ��һ��λ��������
        /// </summary>
        /// <param name="byt_TaskType">�춨���ͣ�0x00������0x01������0x02�ռ�ʱ��0x03���ּ�����0x04�Աꡢ0x05Ԥ���ѹ��ܼ춨</param>
        /// <param name="int_Num">��λ��(255���б�λ)FF ȫ��λ��0xEE,ż����0xDD����</param>
        /// <param name="int_Time">������</param>
        /// <param name="str_Data">���ֵ</param>
        /// <returns></returns>
        bool ReadData(int byt_TaskType, int int_Num, ref bool bln_Result, ref int int_Times, ref string str_Data);
        #endregion

        #region ������״̬
        /// <summary>
        /// �������״̬��Ϣ ���߹���״̬��Bit0����Ԥ������բ״̬��Bit1���������ź�״̬��Bit2�����Ա�״̬��Bit3��
        /// һ���ֽڣ���λ��
        /// ���߹���״̬��Bit0����Ԥ������բ״̬��Bit1���������ź�״̬��Bit2����
        /// �Ա�״̬��Bit3���Ĳ����ֱ���һ���ֽ��е�Bit0��Bit1��Bit2��Bit3��ʾ��Ϊ1���ʾ�ñ�λ�й���/��բ/����/�Ա���ɣ�
        /// Ϊ0���ʾ����/����/����/δ��ɶԱꡣ
        /// </summary>�����λ����λ����    by Zhoujl
        /// <param name="bln_Result">�Ƿ���״̬</param>
        /// <param name="str_Data">��Ӧ��λ��״̬</param>
        /// <returns></returns>  
        bool ReadData(ref bool[] bln_Result, ref string[] str_Data);
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
        bool ReadDataTenTime(int byt_TaskType, ref bool[] bln_Result, ref int[] int_Sum, ref string[] str_Data);
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
        bool ExeOtherCmd(byte byt_Cmd, byte[][] byt_Data, ref bool[] bln_RevResult, ref byte[][] byt_RevData, int int_Scend);




        /// <summary>
        /// ִ��������չָ��(����λ,�з���ָ��)
        /// </summary>
        /// <param name="int_Num">��λ</param>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Data">����</param>
        /// <param name="byt_RevData">����ָ��</param>
        ///  <param name="int_Scend">�ȴ�ʱ��</param>
        /// <returns></returns>
        bool ExeOtherCmd(int int_Num, byte byt_Cmd, byte[] byt_Data, ref byte[] byt_RevData, int int_Scend);

        /// <summary>
        /// ִ��������չָ��(�޷���ָ��)
        /// </summary>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Data">����</param>
        /// <param name="int_Scend">���ʱ��</param>
        /// <returns></returns>
        bool ExeOtherCmd(byte byt_Cmd, byte[] byt_Data, int int_Scend);


       


        /// <summary>
        /// ִ��������չָ��(����λ,�޷���ָ��)
        /// </summary>
        /// <param name="int_Num">��λ</param>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Data">����</param>
        /// <param name="int_Scend">���ʱ��</param>
        /// <returns></returns>
        bool ExeOtherCmd(int int_Num, byte byt_Cmd, byte[] byt_Data, int int_Scend);
        #endregion




    }
}
