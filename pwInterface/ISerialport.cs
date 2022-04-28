/****************************************************************************

    ͨѶ�ӿ���
    ��ΰ 2009-10

*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace pwInterface
{

    /// <summary>
    /// �յ�����ί�У� �����յ����ݴ����¼���
    /// </summary>
    /// <param name="bData">�����յ���</param>
    public delegate void RevEventDelegete(byte[] bData);

    /// <summary>
    /// ���ڽӿ�
    /// </summary>
    public interface ISerialport
    {
        /// <summary>
        /// �˿ں�
        /// </summary>
        int ComPort { get;set;}

        /// <summary>
        /// �����յ����ݴ����¼�
        /// </summary>
        event RevEventDelegete DataReceive;

        /// <summary>
        /// ͨ��ѡ����Ҫ���ڿ���CL2011�豸�ģ���Чֵ1-4,
        /// ����PC���ڵ�RTSEnable��DTREnable��4�����
        /// CL2018-1������Ч��
        /// </summary>
        int Channel { get;set;}

        /// <summary>
        /// �˿����� 0 PC���ڣ�1 CL2018-1��Э�飬 2 CL2018-1Э���
        /// </summary>
        int ComType { get;}

        /// <summary>
        /// IP��ַ����ComType=0����Ч
        /// </summary>
        string IP { get;}

        /// <summary>
        /// ʧ����Ϣ
        /// </summary>
        string LostMessage { get;}

        /// <summary>
        /// ͨ�Ų���
        /// </summary>
        string Setting { get;set;}

        /// <summary>
        /// �˿�״̬
        /// </summary>
        bool State { get;}

        /// <summary>
        /// �򿪶˿�
        /// </summary>
        /// <param name="str_Para">�˿ڲ�������ʽ���˿ں�,IP:Զ��Port��:����ʼPort��  ע������PC�˿ں����������ʡ��</param>
        /// <returns></returns>
        bool PortOpen(string str_Para);

        /// <summary>
        /// �رն˿�
        /// </summary>
        /// <returns></returns>
        bool PortClose();

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="byt_Data">����</param>
        /// <returns></returns>
        bool SendData(byte [] byt_Data);
       
    }




}
