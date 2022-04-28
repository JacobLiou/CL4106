using System;
using System.Collections.Generic;
using System.Text;

namespace pwInterface
{
    /// <summary>
    /// ̨�����ͨ�Žӿ�
    /// ����̨������豸����
    /// </summary>
    public interface ICommAdapter
    {
        /// <summary>
        /// ��λ��
        /// </summary>
        int BWCount { get;set;}

        /// <summary>
        /// �����Ƿ�����,���Ϊfalse����ɴ�LostMessage�ж�ȡʧ����Ϣ
        /// </summary>
        bool Enabled { get;}
        
        /// <summary>
        /// ʧ����Ϣ
        /// </summary>
        string LostMessage { get;}

        /// <summary>
        /// ֹͣ��ǰ����
        /// </summary>
        /// <returns></returns>
        bool StopTask{get;set;}

        /// <summary>
        /// ����
        /// </summary>
        /// <returns></returns>
        bool Link();

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="IsCL191BCL188L">�Ƿ����CL191B��CL188L��True�У�falseû��</param>
        /// <returns></returns>
        bool Link(bool IsCL191BCL188L);


        /// <summary>
        /// �ѻ�
        /// </summary>
        /// <returns></returns>
        bool Unlink();



        /// <summary>
        /// ���ò��Ե�(���಻ƽ���ѹ����)
        /// </summary>
        /// <param name="ecs_Clfs">������ʽ</param>
        /// <param name="sng_U">���ѹ</param>
        /// <param name="sng_xUa">A��������ѹ����</param>
        /// <param name="sng_xUb">B��������ѹ����</param>
        /// <param name="sng_xUc">C��������ѹ����</param>
        /// <param name="sng_I">�����</param>
        /// <param name="sng_Imax">������</param>
        /// <param name="eit_IType">���������׼����</param>
        /// <param name="sng_xIa">A�������������I����IMax�����t����eit_IType������</param>
        /// <param name="sng_xIb">B�������������I����IMax�����t����eit_IType������</param>
        /// <param name="sng_xIc">C�������������I����IMax�����t����eit_IType������</param>
        /// <param name="eet_Element">Ԫ��</param>
        /// <param name="str_Glys">����������������ʾ���������ݱ�ʾ����</param>
        /// <param name="sng_Freq">Ƶ��</param>
        /// <param name="bln_NXX">�Ƿ������� true=������,false=������</param>
        /// <returns></returns>
        bool SetTestPoint(enmClfs ecs_Clfs, Single sng_U, Single sng_xUa, Single sng_xUb, Single sng_xUc,
                          Single sng_I, Single sng_Imax, enmIType eit_IType, Single sng_xIa, Single sng_xIb,
                          Single sng_xIc, enmElement eet_Element, string str_Glys, Single sng_Freq, bool bln_NXX);

        /// <summary>
        /// ���ò��Ե�(���಻ƽ���ѹ����������Ƕ�)
        /// </summary>
        /// <param name="int_Clfs">������ʽ��0=PT4��1=PT3��2=QT4��3=QT3</param>
        /// <param name="sng_U">���ѹ</param>
        /// <param name="sng_xUa">A��������ѹ����</param>
        /// <param name="sng_xUb">B��������ѹ����</param>
        /// <param name="sng_xUc">C��������ѹ����</param>
        /// <param name="sng_I">�����</param>
        /// <param name="sng_Imax">������</param>
        /// <param name="eit_IType">���������׼����</param>
        /// <param name="sng_xIa">A�������������I����IMax�����t����int_IType������</param>
        /// <param name="sng_xIb">B�������������I����IMax�����t����int_IType������</param>
        /// <param name="sng_xIc">C�������������I����IMax�����t����int_IType������</param>
        /// <param name="eet_Element">Ԫ��</param>
        /// <param name="sng_UaPhi">A���ѹ�Ƕ�</param>
        /// <param name="sng_UbPhi">B���ѹ�Ƕ�</param>
        /// <param name="sng_UcPhi">C���ѹ�Ƕ�</param>
        /// <param name="sng_IaPhi">A������Ƕ�</param>
        /// <param name="sng_IbPhi">B������Ƕ�</param>
        /// <param name="sng_IcPhi">C������Ƕ�</param>
        /// <param name="sng_Freq">Ƶ��</param>
        /// <param name="bln_NXX">�Ƿ������� true=������,false=������</param>
        /// <returns></returns>
        bool SetTestPoint(enmClfs ecs_Clfs, Single sng_U, Single sng_xUa, Single sng_xUb, Single sng_xUc,
                          Single sng_I, Single sng_Imax, enmIType eit_IType, Single sng_xIa, Single sng_xIb,
                          Single sng_xIc, enmElement eet_Element, Single sng_UaPhi, Single sng_UbPhi, Single sng_UcPhi,
                           Single sng_IaPhi, Single sng_IbPhi, Single sng_IcPhi, Single sng_Freq);





        /// <summary>
        /// ���ò��Ե�(ͳһ��ѹ�����Ƕ�)
        /// </summary>
        /// <param name="int_Clfs">������ʽ��0=PT4��1=PT3��2=QT4��3=QT3</param>
        /// <param name="sng_U">���ѹ</param>
        /// <param name="sng_xU">������ѹ����</param>
        /// <param name="sng_I">�����</param>
        /// <param name="sng_Imax">������</param>
        /// <param name="eit_IType">���������׼���ͣ�0=�������1=������</param>
        /// <param name="sng_xI">�����������I����IMax�����t����int_IType������</param>
        /// <param name="eet_Element">Ԫ��</param>
        /// <param name="str_Glys">����������������ʾ���������ݱ�ʾ����</param>
        /// <param name="sng_Freq">Ƶ��</param>
        /// <param name="bln_NXX">�Ƿ������� true=������,false=������</param>
        /// <returns></returns>
        bool SetTestPoint(enmClfs ecs_Clfs, Single sng_U, Single sng_xU, Single sng_I, Single sng_Imax,
                          enmIType eit_IType, Single sng_xI, enmElement eet_Element, string str_Glys, Single sng_Freq, bool bln_NXX);



        /// <summary>
        /// ���ò��Ե�(����ͳһ��ѹ����������Ƕ�)
        /// </summary>
        /// <param name="int_Clfs">������ʽ��0=PT4��1=PT3��2=QT4��3=QT3</param>
        /// <param name="sng_U">���ѹ</param>
        /// <param name="sng_xU">������ѹ����</param>
        /// <param name="sng_I">�����</param>
        /// <param name="sng_Imax">������</param>
        /// <param name="eit_IType">���������׼���ͣ�0=�������1=������</param>
        /// <param name="sng_xI">�����������I����IMax�����t����int_IType������</param>
        /// <param name="eet_Element">Ԫ��</param>
        /// <param name="sng_UaPhi">A���ѹ�Ƕ�</param>
        /// <param name="sng_UbPhi">B���ѹ�Ƕ�</param>
        /// <param name="sng_UcPhi">C���ѹ�Ƕ�</param>
        /// <param name="sng_IaPhi">A������Ƕ�</param>
        /// <param name="sng_IbPhi">B������Ƕ�</param>
        /// <param name="sng_IcPhi">C������Ƕ�</param>
        /// <param name="sng_Freq">Ƶ��</param>
        /// <param name="bln_NXX">�Ƿ������� true=������,false=������</param>
        /// <returns></returns>
        bool SetTestPoint(enmClfs ecs_Clfs, Single sng_U, Single sng_xU, Single sng_I, Single sng_Imax,
                          enmIType eit_IType, Single sng_xI, enmElement eet_Element, Single sng_UaPhi, Single sng_UbPhi, Single sng_UcPhi,
                           Single sng_IaPhi, Single sng_IbPhi, Single sng_IcPhi, Single sng_Freq);


        /// <summary>
        /// ���ò��Ե�
        /// </summary>
        /// <param name="int_Clfs">������ʽ</param>
        /// <param name="sng_U">�����ѹ</param>
        /// <param name="sng_I">�������</param>
        /// <param name="eet_Element">Ԫ��</param>
        /// <param name="str_Glys">����������������ʾ���������ݱ�ʾ����</param>
        /// <param name="sng_Freq">Ƶ��</param>
        /// <returns></returns>
        bool SetTestPoint(enmClfs ecs_Clfs, Single sng_U, Single sng_I, enmElement eet_Element, string str_Glys, Single sng_Freq);





        /// <summary>
        /// ��Դ
        /// </summary>
        /// <returns></returns>
        bool PowerOff();



        /// <summary>
        /// ���õ���������
        /// </summary>
        /// <param name="epd_DzType">��������</param>
        /// <param name="ett_TaskType">��������</param>
        /// <param name="ect_ChannelNo">ͨ��</param>
        /// <param name="ept_PulseType">����ӿ�</param>
        /// <param name="egt_GyG">��������</param>
        /// <param name="lng_AmConst">���峣��</param>
        /// <param name="lng_PulseTimes">Ȧ��</param>
        /// <param name="iAmMeterPulseBS">����</param>
        /// <returns></returns>
        bool SetTaskParameter(enmPulseDzType epd_DzType, enmTaskType ett_TaskType, enmChannelType ect_ChannelNo, enmPulseComType ept_PulseType, enmGyGyType egt_GyG, long lng_AmConst, long lng_PulseTimes, byte iAmMeterPulseBS);

        /// <summary>
        /// �����ռ�ʱ������
        /// </summary>
        /// <param name="epd_DzType">��������</param>
        /// <param name="ett_TaskType">��������</param>
        /// <param name="ect_ChannelNo">ͨ��</param>
        /// <param name="ept_PulseType">����ӿ�</param>
        /// <param name="egt_GyG">��������</param>
        /// <param name="lng_AmConst">ʱ��Ƶ��</param>
        /// <param name="lng_PulseTimes">Ȧ��</param>
        /// <returns></returns>
        bool SetTaskParameter(enmPulseDzType epd_DzType, enmTaskType ett_TaskType, enmChannelType ect_ChannelNo, enmPulseComType ept_PulseType, enmGyGyType egt_GyG, float sng_TimePL, int lng_PulseTimes);


        /// <summary>
        /// ���õ�����·
        /// </summary>
        /// <param name="int_LoopType">������·���� 0=��һ��·(Ĭ��)��1=�ڶ���·</param>
        /// <returns></returns>
        bool SetCurrentLoop(int int_LoopType);

        /// <summary>
        /// ���ñ��������������ͣ�0=�������ӣ�1=��������
        /// </summary>
        /// <param name="iPulseDzType">0=�������ӣ�1=��������</param>
        /// <returns></returns>
        bool SetMeterPulseDzType(int iPulseDzType);


        /// <summary>
        /// ���ø���λ��ͨ�ſ���
        /// </summary>
        /// <param name="int_NO">λ���(1-200)������ţ�0xFF(255)=�㲥��ַ��0xEE(238)=ż����ַ��0xDD(221)=������ַ</param>
        /// <param name="bln_Open">�Ƿ�򿪣�ture=�򿪣�false=�ر�</param>
        /// <returns></returns>
        bool SetAmmeterCmmSwitch(int int_NO, bool bln_Open);


        /// <summary>
        /// ��ȡ��׼������Ϣ
        /// </summary>
        /// <param name="str_Value">���ر�׼������Ϣ</param>
        /// <returns></returns>
        bool ReadStdInfo(ref string[] str_Value);

        /// <summary>
        /// ��ȡ����춨���ݣ��������ݸ����������������Ͳ�ͬ����ͬ  ָ��������
        /// </summary>
        /// <param name="int_FunNo">�������ݻ������ţ��춨���ͣ�0x00������0x01������0x02�ռ�ʱ��0x03���ּ�����0x04�Աꡢ0x05Ԥ���ѹ��ܼ춨</param>
        /// <param name="bln_Result">���ض�ȡ���</param>
        /// <param name="str_Data">��������</param>
        /// <param name="int_Times">���ش���</param>
        /// <returns></returns>
        bool ReadTaskData(int int_FunNo,ref bool[] bln_Result, ref string[] str_Data, ref int[] int_Times);

        bool StopCalculate(enmTaskType ett_TaskType);

        /// <summary>
        /// ����CL191ͨ��
        /// </summary>
        /// <param name="iType">0=��׼ʱ�����塢1=��׼��������</param>
        /// <returns></returns>
        bool SetStdTimeChannel(int iType);

        /* ����


        /// <summary>
        /// ����׼����
        /// </summary>
        /// <param name="lng_StdConst">��׼����</param>
        /// <returns></returns>
        bool ReadStdConst(ref long lng_StdConst);

        /// <summary>
        /// ͳһѡ������ͨ��
        /// </summary>
        /// <param name="ect_ChannelNo">ͨ����</param>
        /// <param name="ept_PulseType">����ӿ�</param>
        /// <param name="egt_GyGy">��������</param>
        /// <returns></returns>
        bool SelectPulseChannel(enmChannelType ect_ChannelNo, enmPulseComType ept_PulseType, enmGyGyType egt_GyGy);

        /// <summary>
        /// ͳһ���ñ��������Ȧ��
        /// </summary>
        /// <param name="lng_AmConst">�����</param>
        /// <param name="lng_PulseTimes">Ȧ��</param>
        /// <returns></returns>
        bool SetDnWcParameter(long lng_AmConst, long lng_PulseTimes, long lStdPulseConst, float fStrandMeterP, byte iAmMeterPulseBS);

        /// <summary>
        /// ͳһ�����ռ�ʱ����
        /// </summary>
        /// <param name="sng_TimePL">�����ʱ��Ƶ��</param>
        /// <param name="int_Times">�춨Ȧ��</param>
        /// <returns></returns>
        bool SetRjsParameter(Single sng_TimePL, int int_Times);


        /// <summary>
        /// ���Ƶ�ǰ����״̬
        /// </summary>
        /// <param name="elt_Type">����״̬,0=ֹͣ��1=����</param>
        /// <returns></returns>
        bool ControlTask(enmControlTaskType  elt_Type);

        /// <summary>
        /// ������ǰ��������
        /// </summary>
        /// <returns></returns>
        bool StartTask();



        /// <summary>
        /// ��ȡ��ѹ�����Ĺ���״̬
        /// </summary>
        /// <param name="str_Result">���ظ����ѹ�������Ͻ��,����Ԫ�ض�Ӧ��λ�����ݣ�bit0��ʾA���ѹ bit1��ʾA����� bit2��ʾB���ѹ 
        ///                          bit3��ʾB����� bit4��ʾC���ѹ bit5��ʾC�����,</param>
        /// <param name="int_WaitingTime">�ȴ�ʱ��</param>
        /// <returns></returns>
        bool ReadIUHitch(ref string [] str_Result,int int_WaitingTime);


        /// <summary>
        /// ����̨��״̬��
        /// </summary>
        /// <param name="esl_LightType">״̬������</param>
        /// <returns></returns>
        bool SetStateLight(enmStateLightType esl_LightType);


        /// <summary>
        /// ���ø���λ��ͨ�ŵ��Կ���
        /// </summary>
        /// <param name="int_NO">λ���(1-200)</param>
        /// <param name="bln_Open">�Ƿ�򿪣�ture=�򿪣�false=�ر�</param>
        /// <returns></returns>
        bool SetAmmeterCmmDebug(int int_NO, bool bln_Open);

        /// <summary>
        /// ��λ��ʾ�춨�������״̬��(����2036)
        /// </summary>
        /// <param name="srt_Result">�춨���</param>
        /// <returns></returns>
        bool SetAmMeterResult(enmShowResultType[] srt_Result);

        /// <summary>
        /// ��λ��ʾ�춨�������״̬��(����2036)
        /// </summary>
        /// <param name="int_BwNo"></param>
        /// <param name="srt_Result"></param>
        /// <returns></returns>
        bool SetAmMeterResult(int int_BwNo, enmShowResultType srt_Result);

        /// <summary>
        /// ������״̬(����2036)
        /// </summary>
        /// <param name="int_State">����״̬��</param>
        /// <param name="bln_ErrCalResult">��������״̬</param>
        /// <returns></returns>
        bool ReadState(ref int[] int_State ,ref bool [] bln_ErrCalResult);

        /// <summary>
        /// �����ư汾(����2036)
        /// </summary>
        /// <param name="str_Ver">�汾��</param>
        /// <returns></returns>
        bool ReadVer(ref string str_Ver);

        /// <summary>
        /// ����ֹͣ��
        /// </summary>
        /// <param name="int_Pulses">���������</param>
        /// <returns></returns>
        bool SetStopPulses(int[] int_Pulses);
        /// <summary>
        /// ��GPS����ʱ��
        /// </summary>
        /// <param name="str_DateTime">����ʱ��,ע����ʽΪyyyy-mm-dd hh:mm:ss</param>
        /// <returns></returns>
        bool ReadGPSDateTime(ref string str_DateTime);

        /// <summary>
        /// ��ȡ�¶�ʪ��
        /// </summary>
        /// <param name="sng_Temp">�����¶�ֵ</param>
        /// <param name="sng_Hum">����ʪ��ֵ</param>
        /// <returns></returns>
        bool ReadTempHum(ref Single sng_Temp, ref Single sng_Hum);

        /// <summary>
        /// ������������
        /// </summary>
        /// <param name="lng_SpaceTime">����������ʱ��</param>
        /// <param name="int_Pulses">��������������</param>
        /// <returns></returns>
        //bool SetXLParameter(long[] lng_SpaceTime, int[] int_Pulses);

        ///// <summary>
        ///// ͳһ������������
        ///// </summary>
        ///// <param name="lng_SpaceTime">����������ʱ��</param>
        ///// <param name="int_Pulses">��������������</param>
        ///// <returns></returns>
        //bool SetXLParameter(long lng_SpaceTime, int int_Pulses);

        /// <summary>
        /// �������г������
        /// </summary>
        /// <param name="int_XSwitch">���࿪�أ�����Ԫ��ֵ��0=����г����1=��г���������Ԫ�أ�0=A���ѹ��1=B���ѹ��2=C���ѹ��3=A�������4=B�������5=C�����</param>
        /// <param name="int_XTSwitch">������ο���,����Ԫ��ֵ��0=����г����1=��г��,�����Ԫ�أ����ࣨ0-5�������Σ�0-64��</param>
        /// <returns></returns>
        bool SetHarmSwitch(int[] int_XSwitch, int[][] int_XTSwitch);

        /// <summary>
        /// ���ø���г������
        /// </summary>
        /// <param name="eui_Type">���</param>
        /// <param name="sng_Value">64��г������</param>
        /// <param name="sng_Phase">64��г����λ</param>
        /// <returns></returns>
        bool SetHarmValuePhase(enmUIXXType eui_Type, Single[] sng_Value, Single[] sng_Phase);
        /// <summary>
        /// ��ѹ����\��ʱ�ж�\�𽥱仯\�𽥹ػ�������
        /// </summary>
        /// <param name="evo_Type">��ѹ��������</param>
        /// <returns></returns>
        bool SetVotFalloff(enmVolFallOff  evo_Type);






        /// <summary>
        /// ��ȡ��������(��Ҫ�����ڱ�׼��������ĵ���̨)
        /// </summary>
        /// <param name="strErr">���ص����ֵ</param>
        /// <param name="lngTimes">���ص�������</param>
        /// <returns>true ��ȡ�ɹ���false ��ȡʧ��</returns>
        bool ReadStdMeterErrorAndPulse(ref string[] strErr, ref long[] lngTimes);

        /// <summary>
        /// ��ȡ��׼���ۼ���������ע����������λ����Ч��
        /// </summary>
        /// <param name="lngPulse">����������</param>
        /// <returns>true ��ȡ�ɹ���false ��ȡʧ��</returns>
        bool ReadStdMeterPulses(ref long lngPulse);

        /// <summary>
        /// ��ȡ��λ�¶ȹ��ϣ���λ�������¶ȹ��ߣ�
        /// </summary>
        /// <param name="tht_Result">���ع���</param>
        /// <returns></returns>
        bool ReadBwTempHitch(ref enmTempHitchType[] tht_Result);

        /// <summary>
        /// ��ȡ��λ�¶ȹ��ϣ���λ�������¶ȹ��ߣ�
        /// </summary>
        /// <param name="int_BwIndex">��λ��</param>
        /// <param name="int_Result">���ع���</param>
        /// <returns></returns>
        bool ReadBwTempHitch(int int_BwIndex, ref enmTempHitchType int_Result);

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
        bool ReadPower(int int_BwIndex, int int_Chancel, ref float flt_U, ref float flt_I, ref float flt_ActiveP, ref float flt_ApparentP);

        ///// <summary>
        ///// ��ȡ��ɫ��״̬
        ///// </summary>
        ///// <param name="str_Data"></param>
        ///// <returns></returns>
        //bool ReadStateDSB(ref string[] str_Data);

        ///// <summary>
        ///// ��ȡ��׼���ۼƵ��������֣�
        ///// </summary>
        ///// <param name="sng_StdEnergy">���ر�׼�����</param>
        ///// <returns></returns>
        //bool ReadStdEnergy(ref Single sng_StdEnergy);



        /// <summary>
        /// ����80A���ϴ��������
        /// </summary>
        /// <param name="int_SwitchID">��������(С��80A 0������80A1-48��λ�� 1������80A1-48��λ�� 2)</param>
        /// <re turns></returns>
        bool SetPower(int int_SwitchID);
        */


    }
}
