/****************************************************************************

    ʱ��Դ�ӿ���
    ��ΰ 2009-10

*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace pwInterface
{
    public interface IStdTime
    {
        //-----------����--------------------
        
        string ID { get;set;}               //���ID��
        ISerialport ComPort { get;set;}     //����


        /// <summary>
        /// ͨ��ѡ����Ҫ���ڿ���CL2011�豸�ģ���Чֵ1-4,
        /// ����PC���ڵ�RTSEnable��DTREnable��4�����
        /// CL2018-1������Ч��
        /// </summary>
        int Channel { get;set;}
        
        /// <summary>
        /// ͨ�Ų�������,�ж���豸,��ͬ���ʿ�����/����
        /// </summary>
        string Setting { set;}          //

        /// <summary>
        /// ʧ����Ϣ
        /// </summary>
        string LostMessage { get;}


        //----------����---------------------
        

        /// <summary>
        /// ����
        /// </summary>
        /// <returns></returns>
        bool Link();                                                    //����

        /// <summary>
        /// ����ͨ��
        /// </summary>
        /// <param name="iType">0=��׼ʱ�����塢1=��׼��������</param>
        /// <returns></returns>
        bool SetChannel(int iType);                                     //

        /// <summary>
        /// ��GPSʱ��
        /// </summary>
        /// <param name="sDateTime"></param>
        /// <returns></returns>
        bool ReadGPSTime(ref string sDateTime);                         //

        /// <summary>
        /// ����ʪ��
        /// </summary>
        /// <param name="Temperature"></param>
        /// <param name="Humidity"></param>
        /// <returns></returns>
        bool ReadTempHum(ref float Temperature, ref float Humidity);
        

        ///// <summary>
        /////����GPSʱ��,ֻ���ڶ�ȡ����GPSʱ���ʱ��
        ///// </summary>
        ///// <param name="sDateTime">����ʱ��</param>
        ///// <returns></returns>
        //bool SetGPSTime(string sDateTime);

        ///// <summary>
        ///// ��ȡ���Ĵ��źŵ�ѹС�źŵ��������ڹ���
        ///// </summary>
        ///// <param name="sValue">��\A��\B��\C��</param>
        ///// <returns></returns>
        //bool ReadGhBuSiS(ref string[] sValue);

        ///// <summary>
        ///// ��ȡ���Ĵ��źŵ�ѹС�źŵ������й�����
        ///// </summary>
        ///// <param name="sValue">��\A��\B��\C��</param>
        ///// <returns></returns>
        //bool ReadGhBuSiP(ref string[] sValue);

        ///// <summary>
        ///// ��ȡ���Ĵ��źŵ���С�źŵ�ѹ�����ڹ���
        ///// </summary>
        ///// <param name="sValue">��\A��\B��\C��</param>
        ///// <returns></returns>
        //bool ReadGhBiSuS(ref string[] sValue);

        ///// <summary>
        ///// ��ȡ����С�źŵ�ѹ
        ///// </summary>
        ///// <param name="sValue">A��\B��\C��</param>
        ///// <returns></returns>
        //bool ReadSmallU(ref string[] sValue);

        ///// <summary>
        ///// ��ȡ����С�źŵ���
        ///// </summary>
        ///// <param name="sValue">A��\B��\C��</param>
        ///// <returns></returns>
        //bool ReadSmallI(ref string[] sValue);
        






    }




}
