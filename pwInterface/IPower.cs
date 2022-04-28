/****************************************************************************

    �̿�Դ�ӿ���
    ��ΰ 2009-10

*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace pwInterface
{
    /// <summary>
    /// Դͨ��Э��ӿ�
    /// </summary>
    public interface IPower
    {
        /// <summary>
        /// Դ��ַ
        /// </summary>
        string ID { get;set;}

        /// <summary>
        /// Դ����
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
        /// �Զ���Ӧ��׼��˿ں�
        /// </summary>
        /// <param name="mySerialPort">�˿�����</param>
        /// <returns>��Ӧ�ɹ��򷵻ض˿ںţ�ʧ���򷵻�-1</returns>
        int AdaptCom(ISerialport[] mySerialPort);

        /// <summary>
        /// Դ����
        /// </summary>
        /// <returns>true �����ɹ�,false ����ʧ��</returns>
        bool link();

        /// <summary>
        /// ��ȡԴ�汾
        /// </summary>
        /// <param name="strVer">���ذ汾��</param>
        /// <returns>true ��ȡ�ɹ���false ��ȡʧ��</returns>
        bool ReadVer(ref string strVer);

        /// <summary>
        /// ����Դ������������
        /// </summary>
        /// <param name="Ua">A���ѹ</param>
        /// <param name="Ub">B���ѹ</param>
        /// <param name="Uc">C���ѹ</param>
        /// <param name="Ia">A�����</param>
        /// <param name="Ib">B�����</param>
        /// <param name="Ic">C�����</param>
        /// <param name="PhiUa">A���ѹ�Ƕ�</param>
        /// <param name="PhiUb">B���ѹ�Ƕ�</param>
        /// <param name="PhiUc">C���ѹ�Ƕ�</param>
        /// <param name="PhiIa">A������Ƕ�</param>
        /// <param name="PhiIb">B������Ƕ�</param>
        /// <param name="PhiIc">C������Ƕ�</param>
        /// <param name="Hz">Ƶ��</param>
        /// <param name="iClfs">���Է�ʽ 0=PT4,1=QT4,2=P32,3=Q32,4=Q60,5=Q90,6=Q33</param>
        /// <param name="Xwkz">��λ���� XXIcIbIaUcUbUa</param>
        /// <returns>true ����ɹ���false ���ʧ��</returns>
        bool PowerOn(Single Ua, Single Ub, Single Uc,
                     Single Ia, Single Ib, Single Ic,
                     Single PhiUa, Single PhiUb, Single PhiUc,
                     Single PhiIa, Single PhiIb, Single PhiIc,
                     Single Hz, int iClfs, byte Xwkz);

        /// <summary>
        /// ����Դ�����ͳһ���
        /// </summary>
        /// <param name="U">��ѹ</param>
        /// <param name="I">����</param>
        /// <param name="Phi">�Ƕ�(��ѹ�����н�)</param>
        /// <param name="Hz">Ƶ��</param>
        /// <param name="iClfs">���Է�ʽ 0=PT4,1=QT4,2=P32,3=Q32,4=Q60,5=Q90,6=Q33,7=P</param>
        /// <param name="Xwkz">��λ���� XXIcIbIaUcUbUa</param>
        /// <returns>true ����ɹ���false ���ʧ��</returns>
        bool PowerOn(Single U, Single I, Single Phi, Single Hz, int iClfs, byte Xwkz);

        /// <summary>
        /// ��Դ�������������
        /// </summary>
        /// <param name="U">��ѹ</param>
        /// <param name="I">����</param>
        /// <param name="Phi">��������</param>
        /// <param name="Hz">Ƶ��</param>
        /// <param name="iClfs">���Է�ʽ 0=PT4,1=QT4,2=P32,3=Q32,4=Q60,5=Q90,6=Q33,7=P</param>
        /// <param name="HABC">Ԫ����0=��Ԫ��1��AԪ��2��BԪ��3��CԪ</param>
        /// <returns></returns>
        bool PowerOn(float U, float I, string str_Glys, float Hz, int iClfs, enmElement emt_Element);

        
        /// <summary>
        /// ��Դ����
        /// </summary>
        /// <returns>true ��Դ�ɹ���false ��Դʧ��</returns>
        bool PowerOff();

    }




}
