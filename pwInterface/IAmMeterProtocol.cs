using System;
using System.Collections.Generic;
using System.Text;

namespace ClInterface
{



    public delegate void Dge_EventRxFrame(string str_Frame);
    public delegate void Dge_EventTxFrame(string str_Frame);

    public delegate void DisposeRxEvent(string str_Frame);
    public delegate void DisposeTxEvent(string str_Frame);


    /// <summary>
    /// �๦�ܱ�Э��ӿ�
    /// </summary>
    public interface IAmMeterProtocol
    {

        /// <summary>
        /// ����485�����¼�
        /// </summary>
        event Dge_EventRxFrame OnEventRxFrame;
        /// <summary>
        /// ����485�����¼�
        /// </summary>
        event Dge_EventTxFrame OnEventTxFrame;


        /// <summary>
        /// ͨ�Ŵ���
        /// </summary>
        ISerialport ComPort { get;set;}

        /// <summary>
        /// ������
        /// </summary>
        string Setting { get;set;}

        /// <summary>
        /// ���ַ
        /// </summary>
        string Address { get;set;}

        /// <summary>
        /// д��������
        /// </summary>
        string WritePassword { get;set;}

        /// <summary>
        /// ������������
        /// </summary>
        string ClearDemandPassword { get;set;}

        /// <summary>
        /// ������������
        /// </summary>
        string ClearEnergyPassword { get;set;}

        
        /// <summary>
        /// ����Ա����
        /// </summary>
        string UserID { get;set;}

        /// <summary>
        /// ������֤����
        /// </summary>
        int VerifyPasswordType { get;set;}

        /// <summary>
        /// д����ʱ���������Ƿ����д����,true=Ҫ��false=����
        /// </summary>
        bool DataFieldPassword { get;set;}

        /// <summary>
        /// ��д����ʱ��������Ƿ��AA
        /// </summary>
        bool BlockAddAA { get;set;}

        /// <summary>
        /// ������������  �磺��ƽ�ȼ�=1234 ���ƽ��=4123
        /// </summary>
        string TariffOrderType { get;set;}

        /// <summary>
        /// ���ڸ�ʽ,Ĭ��:YYMMDDHHFFSS
        /// </summary>
        string DateTimeFormat { get;set;}

        /// <summary>
        /// ���������ţ�Ĭ��:0
        /// </summary>
        int SundayIndex { get;set;}

        /// <summary>
        /// �����ļ�
        /// </summary>
        string ConfigFile { get;set;}



        /// <summary>
        /// ����ʧ����Ϣ
        /// </summary>
        string LostMessage { get;}

        /// <summary>
        /// ����֡
        /// </summary>
        string RxFrame { get;}

        /// <summary>
        /// �·�֡
        /// </summary>
        string TxFrame { get;}

        /// <summary>
        /// �·�֡�Ļ��ѷ�����
        /// </summary>
        int FECount { get;set;}
            


        /// <summary>
        /// ͨ�Ų���
        /// </summary>
        /// <param name="int_Type"></param>
        /// <returns></returns>
        bool ComTest(int int_Type);



        /// <summary>
        /// �㲥Уʱ
        /// </summary>
        /// <param name="int_Type">�㲥Уʱ����</param>
        /// <param name="str_DateTime">����ʱ��</param>
        /// <returns></returns>
        bool BroadcastTime(int int_Type, string str_DateTime);



        /// <summary>
        /// ��ȡ����(�ַ��ʶ�ȡ)
        /// </summary>
        /// <param name="int_Type">��ȡ����</param>
        /// <param name="ept_DirectType">�������ͣ�0=P+ 1=P- 2=Q+ 3=Q- 4=Q1 5=Q2 6=Q3 6=Q4</param>
        /// <param name="ett_TariffType">�������ͣ�0=�ܣ�1=�壬2=ƽ��3=�ȣ�4=��</param>
        /// <param name="sng_Energy">���ص���</param>
        /// <returns></returns>
        bool ReadEnergy(int int_Type, enmPDirectType ept_DirectType, enmTariffType ett_TariffType, ref Single sng_Energy);

        /// <summary>
        /// ��ȡ����(���з��ʶ�ȡ)
        /// </summary>
        /// <param name="int_Type">��ȡ����</param>
        /// <param name="ept_DirectType">�������ͣ�0=P+ 1=P- 2=Q+ 3=Q- 4=Q1 5=Q2 6=Q3 6=Q4</param>
        /// <param name="sng_Energy">���ص���</param>
        /// <returns></returns>
        bool ReadEnergy(int int_Type, enmPDirectType ept_DirectType, ref Single[] sng_Energy);

        /// <summary>
        /// ��ȡ����(�ַ��ʶ�ȡ)
        /// </summary>
        /// <param name="int_Type">��ȡ����</param>
        /// <param name="ept_DirectType">�������ͣ�0=P+ 1=P- 2=Q+ 3=Q- 4=Q1 5=Q2 6=Q3 6=Q4</param>
        /// <param name="ett_TariffType">�������ͣ�0=�ܣ�1=�壬2=ƽ��3=�ȣ�4=��</param>
        /// <param name="sng_Demand">��������</param>
        /// <returns></returns>
        bool ReadDemand(int int_Type, enmPDirectType ept_DirectType, enmTariffType ett_TariffType, ref Single sng_Demand);

        /// <summary>
        /// ��ȡ����(���з��ʶ�ȡ)
        /// </summary>
        /// <param name="int_Type">��ȡ����</param>
        /// <param name="ept_DirectType">�������ͣ�0=P+ 1=P- 2=Q+ 3=Q- 4=Q1 5=Q2 6=Q3 6=Q4</param>
        /// <param name="sng_Demand">��������</param>
        /// <returns></returns>
        bool ReadDemand(int int_Type, enmPDirectType ept_DirectType, ref Single[] sng_Demand);

        /// <summary>
        /// ������ʱ��
        /// </summary>
        /// <param name="int_Type">��ȡ����</param>
        /// <param name="str_DateTime">����ʱ��</param>
        /// <returns></returns>
        bool ReadDateTime(int int_Type, ref string str_DateTime);


        /// <summary>
        /// ����ַ
        /// </summary>
        /// <param name="int_Type">����</param>
        /// <param name="str_Address">���ص�ַ</param>
        /// <returns></returns>
        bool ReadAddress(int int_Type, ref string str_Address);

        /// <summary>
        /// ��ȡʱ��
        /// </summary>
        /// <param name="int_Type">����</param>
        /// <param name="str_PTime">����ʱ��</param>
        /// <returns></returns>
        bool ReadPeriodTime(int int_Type,ref string [] str_PTime);


        /// <summary>
        /// ��ȡ���ݣ������ͣ������
        /// </summary>
        /// <param name="int_Type">����</param>
        /// <param name="str_ID">��ʶ��,2���ֽ�</param>
        /// <param name="int_Len">���ݳ���(�ֽ���)</param>
        /// <param name="int_Dot">С��λ</param>
        /// <param name="sng_Value">��������</param>
        /// <returns></returns>
        bool ReadData(int int_Type, string str_ID, int int_Len, int int_Dot, ref Single sng_Value);

        /// <summary>
        /// ��ȡ���ݣ��ַ��ͣ������
        /// </summary>
        /// <param name="int_Type">����</param>
        /// <param name="str_ID">��ʶ��,2���ֽ�</param>
        /// <param name="int_Len">���ݳ���(�ֽ���)</param>
        /// <param name="str_Value">��������</param>
        /// <returns></returns>
        bool ReadData(int int_Type, string str_ID, int int_Len, ref string str_Value);


        /// <summary>
        /// ��ȡ���ݣ������ͣ����ݿ飩
        /// </summary>
        /// <param name="int_Type">����</param>
        /// <param name="str_ID">��ʶ��,2���ֽ�</param>
        /// <param name="int_Len">���ݳ���(�ֽ���)</param>
        /// <param name="int_Dot">С��λ</param>
        /// <param name="sng_Value">��������</param>
        /// <returns></returns>
        bool ReadData(int int_Type, string str_ID, int int_Len, int int_Dot, ref Single[] sng_Value);

        /// <summary>
        /// ��ȡ���ݣ��ַ��ͣ����ݿ飩
        /// </summary>
        /// <param name="int_Type">����</param>
        /// <param name="str_ID">��ʶ��,2���ֽ�</param>
        /// <param name="int_Len">���ݳ���(�ֽ���)</param>
        /// <param name="int_Dot">С��λ</param>
        /// <param name="sng_Value">��������</param>
        /// <returns></returns>
        bool ReadData(int int_Type, string str_ID, int int_Len, ref string[] str_Value);


        

        /// <summary>
        /// д��ַ
        /// </summary>
        /// <param name="int_Type">����</param>
        /// <param name="str_Address">д���ַ</param>
        /// <returns></returns>
        bool WriteAddress(int int_Type, string str_Address);

        /// <summary>
        /// д����ʱ��
        /// </summary>
        /// <param name="int_Type">����</param>
        /// <param name="str_DateTime">����ʱ��</param>
        /// <returns></returns>
        bool WriteDateTime(int int_Type, string str_DateTime);

        /// <summary>
        /// дʱ��
        /// </summary>
        /// <param name="int_Type">����</param>
        /// <param name="str_PTime">ʱ����</param>
        /// <returns></returns>
        bool WritePeriodTime(int int_Type, string[] str_PTime);



        /// <summary>
        /// д����
        /// </summary>
        /// <param name="int_Type">����</param>
        /// <param name="str_ID">��ʶ��</param>
        /// <param name="bln_Reverse">���ͷ�ʽ��true=�ߵ�λ�Ե���false=�ߵ�λ����</param>
        /// <param name="byt_Value">д������</param>
        /// <returns></returns>
        bool WriteData(int int_Type, string str_ID, byte[] byt_Value);

        /// <summary>
        /// д����(�ַ��ͣ�������)
        /// </summary>
        /// <param name="int_Type">����</param>
        /// <param name="str_ID">��ʶ��,�����ֽ�</param>
        /// <param name="int_Len">���ݳ���(����ÿ���ֽ���)</param>
        /// <param name="str_Value">д������</param>
        /// <returns></returns>
        bool WriteData(int int_Type, string str_ID, int int_Len, string str_Value);


        /// <summary>
        /// д����(�����ͣ�������)
        /// </summary>
        /// <param name="int_Type">����</param>
        /// <param name="str_ID">��ʶ��</param>
        /// <param name="int_Len">���ݳ���(�ֽ���)</param>
        /// <param name="int_Dot">С��λ</param>
        /// <param name="bln_Reverse">���ͷ�ʽ��true=�ߵ�λ�Ե���false=�ߵ�λ����</param>
        /// <param name="sng_Value">д������</param>
        /// <returns></returns>
        bool WriteData(int int_Type, string str_ID, int int_Len, int int_Dot, Single sng_Value);


        /// <summary>
        /// д����(�ַ��ͣ����ݿ�)
        /// </summary>
        /// <param name="int_Type">����</param>
        /// <param name="str_ID">��ʶ��</param>
        /// <param name="int_Len">���ݳ���(�ֽ���)</param>
        /// <param name="bln_Reverse">���ͷ�ʽ��true=�ߵ�λ�Ե���false=�ߵ�λ����</param>
        /// <param name="str_Value">д������</param>
        /// <returns></returns>
        bool WriteData(int int_Type, string str_ID, int int_Len, string[] str_Value);


        /// <summary>
        /// д����(�����ͣ����ݿ�)
        /// </summary>
        /// <param name="int_Type">����</param>
        /// <param name="str_ID">��ʶ��</param>
        /// <param name="int_Len">���ݳ���(�ֽ���)</param>
        /// <param name="int_Dot">С��λ</param>
        /// <param name="bln_Reverse">���ͷ�ʽ��true=�ߵ�λ�Ե���false=�ߵ�λ����</param>
        /// <param name="str_Value">д������</param>
        /// <returns></returns>
        bool WriteData(int int_Type, string str_ID, int int_Len, int int_Dot, Single[] sng_Value);


        /// <summary>
        /// �������
        /// </summary>
        /// <returns></returns>
        bool ClearDemand(int int_Type);

        /// <summary>
        /// ��յ���
        /// </summary>
        /// <param name="int_Type">����</param>
        /// <returns></returns>
        bool ClearEnergy(int int_Type);

        /// <summary>
        /// ����¼���¼
        /// </summary>
        /// <param name="int_Type">����</param>
        /// <param name="str_ID">�¼���������</param>
        /// <returns></returns>
        bool ClearEventLog(int int_Type, string str_ID);

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="int_Type">����</param>
        /// <param name="ecp_PulseType">���������������</param>
        /// <returns></returns>
        bool SetPulseCom(int int_Type, enmComPulseType ecp_PulseType);


        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="int_Type">����</param>
        /// <param name="str_DateHour">����ʱ�䣬MMDDhhmm(��.��.ʱ.��)</param>
        /// <returns></returns>
        bool FreezeCmd(int int_Type, string str_DateHour);


        /// <summary>
        /// ���Ĳ�����
        /// </summary>
        /// <param name="int_Type">����</param>
        /// <param name="str_Setting">������</param>
        /// <returns></returns>
        bool ChangeSetting(int int_Type, string str_Setting);


        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="int_Type">����</param>
        /// <param name="int_Class">����ȼ������޸ļ���������</param>
        /// <param name="str_OldPws">������,����Ǹ��ߵȼ��޸ı��ȼ���������Ӹ��ߵȼ���ԭ�����򲻰����ȼ�</param>
        /// <param name="str_NewPsw">������,�������ȼ�</param>
        /// <returns></returns>
        bool ChangePassword(int int_Type,int int_Class, string str_OldPws, string str_NewPsw);

        



    }
}
