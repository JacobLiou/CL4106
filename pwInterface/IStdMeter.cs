/****************************************************************************

    ��׼��ӿ���
    ��ΰ 2009-10

*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace pwInterface
{

    /// <summary>
    /// ��׼��ͨ�Žӿ�
    /// </summary>
    public interface IStdMeter
    {
        /// <summary>
        /// ��׼���ַ
        /// </summary>
        string ID { get;set;}


        /// <summary>
        /// �ز���
        /// </summary>
        string Setting { set;}


        /// <summary>
        /// ʧ����Ϣ
        /// </summary>
        string LostMessage { get;}

        /// <summary>
        /// ��׼����
        /// </summary>
        ISerialport ComPort { get;set;}

        /// <summary>
        /// ͨ��ѡ����Ҫ���ڿ���CL2011�豸�ģ���Чֵ1-4,
        /// ����PC���ڵ�RTSEnable��DTREnable��4�����
        /// CL2018-1������Ч��
        /// </summary>
        int Channel { get;set;}


        /// <summary>
        /// 
        /// </summary>
        bool Enabled { get;set;}


        /// <summary>
        /// �Զ���Ӧ��׼��˿ں�
        /// </summary>
        /// <param name="mySerialPort">�˿�����</param>
        /// <returns>��Ӧ�ɹ��򷵻ض˿ںţ�ʧ���򷵻�-1</returns>
        int AdaptCom(ISerialport[] mySerialPort);


        /// <summary>
        /// ��׼������
        /// </summary>
        /// <returns>true �����ɹ�,false ����ʧ��</returns>
        bool link();

        /// <summary>
        /// ��ȡ��׼��汾
        /// </summary>
        /// <param name="strVer">���ذ汾��</param>
        /// <returns>true ��ȡ�ɹ���false ��ȡʧ��</returns>
        bool ReadVer(ref string strVer);

        /// <summary>
        /// 1.	���ý��߷�ʽ 81 30 PCID 0a a3 00 01 20 uclinemode CS
        /// </summary>
        /// <param name="Uclinemode">���߷�ʽ��CL1115���Զ����̣�08H �ֶ����̣�88H</param>
        /// <returns></returns>
        bool SetStdMeterUclinemode(byte Uclinemode);


        /// <summary>
        /// 2.	���ñ������� 81 30 PCID 0d a3 00 04 01 uiLocalnum CS
        /// </summary>
        /// <param name="lStdConst">��׼����</param>
        /// <param name="bAuto">������Ч����Ӧ�ӿ�</param>
        /// <returns></returns>
        bool SetStdMeterConst(long lStdConst, bool bAuto);

        /// <summary>
        /// 3.	���õ���ָʾ 81 30 PCID 0b a3 00 08 01 usE1type CS
        /// </summary>
        /// <param name="int_EngeType">CL1115�����й�����00H  ��CL3115�����й�����00H  ���޹�����40H  </param>
        /// <returns></returns>
        bool SetStdMeterUsE1type(int int_EngeType);

        /// <summary>
        /// 4.	���õ��ܼ��������������(������׼��)
        /// </summary>
        /// <param name="UcE1switch">����ָ�� 0��ֹͣ����  1����ʼ����������  2����ʼ�����������</param>
        /// <returns></returns>
        bool SetStdMeterUcE1switch(byte UcE1switch);

        /// <summary>
        /// 5.	���õ��ܲ��� 81 30 PCID 0e a3 00 09 20 uclinemode 11 usE1type ucE1switch CS
        /// </summary>
        /// <param name="Uclinemode">���߷�ʽ��CL1115���Զ����̣�08H �ֶ����̣�88H</param>
        /// <param name="UsE1type">����ָʾ��CL1115�����й�����00H    CL3115�����й�����00H  ���޹�����40H  </param>
        /// <param name="UcE1switch">����ָ� 0��ֹͣ����  1����ʼ����������  2����ʼ�����������</param>
        /// <returns>[true-�ɹ�,false-ʧ��]</returns>
        /// <returns></returns>
        bool SetAmMeterParameter(byte Uclinemode, byte UsE1type, byte UcE1switch);


        /// <summary>
        /// ��ȡ��׼���������
        /// </summary>
        /// <param name="sPara">���ز������ݣ��Ⱥ�˳��Э���ʽ</param>
        /// <returns>true ��ȡ�ɹ���false ��ȡʧ��</returns>
        bool ReadStdMeterInfo(ref string []sPara);

        /// <summary>
        /// ��ȡ��׼���׼����
        /// </summary>
        /// <param name="lngConst">���ر�׼����</param>
        /// <returns>true ��ȡ�ɹ���false ��ȡʧ��</returns>
        bool ReadStdMeterConst(ref long lngConst);


        /// <summary>
        /// 12.	���ñ�׼��λ
        /// </summary>
        /// <param name="sngIa"></param>
        /// <param name="sngIb"></param>
        /// <param name="sngIc"></param>
        /// <param name="sngUa"></param>
        /// <param name="sngUb"></param>
        /// <param name="sngUc"></param>
        /// <returns></returns>
        bool SetUIScale(float sngIa, float sngIb, float sngIc, float sngUa, float sngUb, float sngUc);

        /// <summary>
        /// ��ѯ��׼����
        /// </summary>
        /// <param name="sng_I">�������</param>
        /// <returns></returns>
        long SelectStdMeterConst(Single sng_I);

        /// <summary>
        /// ���ñ�׼�����
        /// </summary>
        /// <param name="int_Type"></param>
        /// <returns></returns>
        bool SetLCDMenu(byte int_Type);

        /// <summary>
        /// �˳���׼�����,���س�ʼ����
        /// </summary>
        /// <returns></returns>
        bool ExitLCDMenu();

        /// <summary>
        /// ������׼��
        /// </summary>
        /// <returns>true �����ɹ���false ����ʧ��</returns>
        bool RunStdMeter();

        /// <summary>
        /// ��չָ��
        /// </summary>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Data">������</param>
        /// <param name="byt_RevData">��������</param>
        /// <returns>true �����ɹ���false ����ʧ��</returns>
        bool ExtendCommand(byte byt_Cmd, byte[] byt_Data, ref byte[] byt_RevData);


        ///// <summary>
        ///// ��ȡ��׼�����λ
        ///// </summary>
        ///// <param name="int_UaRng">A���ѹ��λ</param>
        ///// <param name="int_UbRng">B���ѹ��λ</param>
        ///// <param name="int_UcRng">C���ѹ��λ</param>
        ///// <param name="int_IaRng">A�������λ</param>
        ///// <param name="int_IbRng">B�������λ</param>
        ///// <param name="int_IcRng">C�������λ</param>
        ///// <returns></returns>
        //bool ReadOutRange(ref int int_UaRng, ref int int_UbRng, ref int int_UcRng,
        //                  ref int int_IaRng, ref int int_IbRng, ref int int_IcRng);



        ///// <summary>
        ///// ���ñ�׼���ѹ��λ
        ///// </summary>
        ///// <param name="iUaScale">A���ѹ��λ</param>
        ///// <param name="iUbScale">B���ѹ��λ</param>
        ///// <param name="iUcScale">C���ѹ��λ</param>
        ///// <returns>true ���óɹ���false ����ʧ��</returns>
        //bool SetUScale(int iUaScale,int iUbScale,int iUcScale);

        ///// <summary>
        ///// ���ñ�׼���ѹ��λ
        ///// </summary>
        ///// <param name="sngUa">A���ѹ��ֵ</param>
        ///// <param name="sngUb">B���ѹ��ֵ</param>
        ///// <param name="sngUc">C���ѹ��ֵ</param>
        ///// <returns>true ���óɹ���false ����ʧ��</returns>
        //bool SetUScale(Single sngUa, Single sngUb, Single sngUc);

        ///// <summary>
        ///// ���ñ�׼�������λ
        ///// </summary>
        ///// <param name="iIaScale">A�������λ</param>
        ///// <param name="iIbScale">B�������λ</param>
        ///// <param name="iIcScale">C�������λ</param>
        ///// <returns>true ���óɹ���false ����ʧ��</returns>
        //bool SetIScale(int iIaScale, int iIbScale, int iIcScale);

        ///// <summary>
        ///// ���ñ�׼�������λ
        ///// </summary>
        ///// <param name="sngIa">A�������ֵ</param>
        ///// <param name="sngIb">B�������ֵ</param>
        ///// <param name="sngIc">C�������ֵ</param>
        ///// <returns>true ���óɹ���false ����ʧ��</returns>
        //bool SetIScale(Single sngIa, Single sngIb, Single sngIc);

        ///// <summary>
        ///// ��ȡ��׼���ۼ���������ע����������λ����Ч��
        ///// </summary>
        ///// <param name="lngPulse">����������</param>
        ///// <returns>true ��ȡ�ɹ���false ��ȡʧ��</returns>
        //bool ReadStdMeterPulses(ref long lngPulse);

        ///// <summary>
        ///// ��ȡ��׼���׼���� ���ٽ絵λֱ�Ӳ��
        ///// </summary>
        ///// <param name="lngConst">����</param>
        ///// <param name="sng_U">��ѹ</param>
        ///// <param name="sng_I">����</param>
        ///// <param name="bln_SetXieBo">�Ƿ�����г��</param>
        ///// <returns></returns>
        //bool ReadStdMeterConst(ref long lngConst, Single sng_U, Single sng_I, bool bln_SetXieBo);


        ///// <summary>
        ///// ���ñ�������
        ///// </summary>
        ///// <param name="lMeterConst">�������</param>
        ///// <param name="lPulseTimes">У��Ȧ��</param>
        ///// <param name="iLX">���ޣ�0=25A,1=100A</param>
        ///// <param name="iClfs">������ʽ��0=PT4,1=QT4,2=P32,3=Q32,4=Q90,5=Q60,6=Q33</param>
        ///// <returns>true ���óɹ���false ����ʧ��</returns>
        //bool SetAmMeterParameter(long lMeterConst, long lPulseTimes, int iLX, int iClfs);

    }
}
