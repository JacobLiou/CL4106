using System;
using System.Text;

namespace pwFunction.pwConst
{
    /// <summary>
    ///  ������������C+��Ƿ���+ʹ��ģ��(ȫ����G)+_������
    /// C:Cus��д���������г��������ĵ�һ����ĸ
    /// M:Mark��д��������ǳ�������
    /// T:Text��д,�����ı����ݱ������
    /// ģ���д���գ�
    /// SC:SystemConfigģ��
    /// </summary>
    public class Variable
    {

        /// <summary>
        /// Grid���̬��ɫ
        /// </summary>
        public static System.Drawing.Color Color_Grid_Normal = System.Drawing.Color.FromArgb(250, 250, 250);

        /// <summary>
        /// Grid���������ɫ
        /// </summary>
        public static System.Drawing.Color Color_Grid_Alter = System.Drawing.Color.FromArgb(235, 250, 235);

        /// <summary>
        /// �̶��У��У���ɫ
        /// </summary>
        public static System.Drawing.Color Color_Grid_Frone = System.Drawing.Color.FromArgb(225, 225, 225);

        /// <summary>
        /// ���ϸ���ɫ
        /// </summary>
        public static System.Drawing.Color Color_Grid_BuHeGe = System.Drawing.Color.Red;

        /// <summary>
        /// �ϸ���ɫ
        /// </summary>
        public static System.Drawing.Color Color_Grid_HeGe = System.Drawing.Color.Lime;

        /// <summary>
        /// ������Ŀ���������������Ҫ������
        /// </summary>
        public const int CONST_ENMMETERPRJIDCOUNT = 30;//enmMeterPrjID Count

        /// <summary>
        /// δ���ı�����
        /// </summary>
        public const string CTG_WeiJian = "δ��";

        /// <summary>
        /// �ϸ��ı�����
        /// </summary>
        public const string CTG_HeGe = "�ϸ�";

        /// <summary>
        /// ���ϸ��ı�����
        /// </summary>
        public const string CTG_BuHeGe = "���ϸ�";

        /// <summary>
        /// �ϸ��ǡ�
        /// </summary>
        public const string CMG_HeGe = "��";

        /// <summary>
        /// ���ϸ��־
        /// </summary>
        public const string CMG_BuHeGe = "��";
        /// <summary>
        /// �������Ͽ�������Ϣ��ʾ�ı�
        /// </summary>
        public const string CTG_SERVERUNCONNECT = "�������Ͽ�����";
        /// <summary>
        /// ��Ŀ�춨�����ʾ�ı�
        /// </summary>
        public const string CTG_VERIFYOVER = "������Ŀ�춨���";
        /// <summary>
        /// û�г����Ĭ��ֵ
        /// </summary>
        public const float WUCHA_INVIADE = -999F;


        #region ϵͳ�ĵ��������ĵ��������ĵ����·��

        /// <summary>
        /// ϵͳ����XML�ĵ�·��
        /// </summary>
        public const string CONST_SYSTEMPATH = "\\System\\System.xml";
        /// <summary>
        /// ��������Ʒ������XML�ĵ�·��
        /// </summary>
        public const string CONST_WORKPLANPATH = "\\WorkPlan\\WorkPlan.xml";
        /// <summary>
        /// �������ؽǶ�����XML�ĵ�·��
        /// </summary>
        public const string CONST_GLYSDICTIONARY = "\\Const\\GLYS.xml";

        /// <summary>
        /// ͨ������֡����Ŀ¼
        /// </summary>
        public const string CONST_COMMUNICATIONDATA = "\\Comm\\";

        /// <summary>
        /// ������ʷ�ļ�
        /// </summary>
        public const string CONST_HISTORY_LOGINPATH = "\\History\\Login.dat";
        /// <summary>
        /// ��������Ʒ��������ʷ�ļ�
        /// </summary>
        public const string CONST_HISTORY_WORKPLANPATH = "\\History\\WorkPlan.dat";

        /// <summary>
        /// �춨���ݱ��ش洢Ŀ¼(��ʱʹ��)
        /// </summary>
        public const string CONST_METERDATA = "\\Data\\";
        /// <summary>
        /// �춨����XML�ļ�
        /// </summary>
        public const string CONST_METERDATAXML = "\\Data\\Data.xml";

        ///// <summary>
        ///// ϵͳ�ֵ�����XML�ĵ�·��
        ///// </summary>
        //public const string CONST_DICTIONARY = "\\Const\\ZiDian.xml";
        ///// <summary>
        ///// ���������ֵ��
        ///// </summary>
        //public const string CONST_XIBDICTIONARY = "\\Const\\xIb.xml";
        ///// <summary>
        ///// �๦����Ŀ�ֵ�
        ///// </summary>
        //public const string CONST_DGNDICTIONARY = "\\Const\\DngConfig.xml";
        ///// <summary>
        ///// ��Ŀ��ʶ���ֵ�
        ///// </summary>
        //public const string CONST_PROJIDENTIFIE= "\\Const\\ProjIdentifie.xml";
        ///// <summary>
        ///// ������ֵ��ļ�
        ///// </summary>
        //public const string CONST_WCLIMIT = "\\Const\\WcLimit.Mdb";
        ///// <summary>
        ///// �๦��Э�������ļ�
        ///// </summary>
        //public const string CONST_DGNPROTOCOL = "\\Const\\DgnProtocol.xml";
        ///// <summary>
        ///// ����ͨ�������ļ�
        ///// </summary>
        //public const string CONST_PULSETYPE ="\\Tmp\\Pulse.xml";



        //public const string CONST_WAITUPDATE = "\\WAITUPDATE";

        #endregion

        #region ϵͳ���ü�ֵ

        #region ̨����Ϣ����
        /// <summary>
        /// ̨����
        /// </summary>
        public const string CTC_DESKNO = "DESKNO";
        /// <summary>
        /// ̨�����ͣ�����̨������̨
        /// </summary>
        public const string CTC_DESKTYPE = "DESKTYPE";
        /// <summary>
        /// ��λ��
        /// </summary>
        public const string CTC_BWCOUNT = "BWCOUNT";
        /// <summary>
        /// ��ʾ����ÿ�и���
        /// </summary>
        public const string CTC_DESBWCOUNT = "CTC_DESBWCOUNT";

        /// <summary>
        /// �Ƿ���CL191B��CL188L
        /// </summary>
        public const string CTC_ISCL191BCL188L = "CTC_ISCL191BCL188L";


        #endregion

        #region CL2018-1�˿ڷ���
        /// <summary>
        /// 2018IP
        /// </summary>
        public const string CTC_2018IP = "2018IP";
        /// <summary>
        /// Դ�˿�
        /// </summary>
        public const string CTC_COMY = "COMY";
        /// <summary>
        /// ��˿�
        /// </summary>
        public const string CTC_COMB = "COMB";
        /// <summary>
        /// ����˿�
        /// </summary>
        public const string CTC_COMW = "COMW";
        /// <summary>
        /// ʱ��Դ�˿�
        /// </summary>
        public const string CTC_COMT = "COMT";

        #endregion

        #region �������ݿ�
        /// <summary>
        /// SQL���ݿ������IP
        /// </summary>
        public const string CTC_SQL_SERVERIP = "SQL_SERVERIP";
        /// <summary>
        /// SQL���ݿ��û���
        /// </summary>
        public const string CTC_SQL_USERID = "SQL_USERID";
        /// <summary>
        /// SQL��¼����
        /// </summary>
        public const string CTC_SQL_PASSWORD = "SQL_PASSWORD";
        /// <summary>
        /// SQL���ݿ���
        /// </summary>
        public const string CTC_SQL_DATABASENAME = "SQL_DATABASENAME";
        #endregion

        #region ��λRS485�˿ڲ�������
        public const string CTC_RS485_COM1 = "RS485_COM1s";
        public const string CTC_RS485_COM2 = "RS485_COM2s";
        public const string CTC_RS485_COM3 = "RS485_COM3s";
        public const string CTC_RS485_COM4 = "RS485_COM4s";
        public const string CTC_RS485_COM5 = "RS485_COM5s";
        public const string CTC_RS485_COM6 = "RS485_COM6s";
        public const string CTC_RS485_COM7 = "RS485_COM7s";
        public const string CTC_RS485_COM8 = "RS485_COM8s";
        public const string CTC_RS485_COM9 = "RS485_COM9s";
        public const string CTC_RS485_COM10 = "RS485_COM10s";
        public const string CTC_RS485_COM11 = "RS485_COM11s";
        public const string CTC_RS485_COM12 = "RS485_COM12s";
        public const string CTC_RS485_COM13 = "RS485_COM13s";
        public const string CTC_RS485_COM14 = "RS485_COM14s";
        public const string CTC_RS485_COM15 = "RS485_COM15s";
        public const string CTC_RS485_COM16 = "RS485_COM16s";
        public const string CTC_RS485_COM17 = "RS485_COM17s";
        public const string CTC_RS485_COM18 = "RS485_COM18s";
        public const string CTC_RS485_COM19 = "RS485_COM19s";
        public const string CTC_RS485_COM20 = "RS485_COM20s";
        public const string CTC_RS485_COM21 = "RS485_COM21s";
        public const string CTC_RS485_COM22 = "RS485_COM22s";
        public const string CTC_RS485_COM23 = "RS485_COM23s";
        public const string CTC_RS485_COM24 = "RS485_COM24s";
        #endregion

        #region ���춨�������
        /// <summary>
        /// ÿ������ȡ�������������
        /// </summary>
        public const string CTC_WC_TIMES_BASICERROR = "TIMES_BASICERROR";
        /// <summary>
        /// ��׼ƫ��ȡ�������������
        /// </summary>
        public const string CTC_WC_TIMES_WINDAGE = "TIMES_WINDAGE";
        /// <summary>
        /// ÿ���������������
        /// </summary>
        public const string CTC_WC_MAXTIMES = "WC_MAXTIMES";
        /// <summary>
        /// ÿ�������춨ʱ��
        /// </summary>
        public const string CTC_WC_MAXSECONDS = "WC_MAXSECONDS";
        /// <summary>
        /// ������ж�
        /// </summary>
        public const string CTC_WC_JUMP = "WC_JUMP";
        /// <summary>
        /// IN����
        /// </summary>
        public const string CTC_WC_IN = "WC_IN";
        /// <summary>
        /// ƽ��ֵ����С��λ
        /// </summary>
        public const string CTC_WC_AVGPRECISION = "AVGPRECISION";
        /// <summary>
        /// �Ƿ�ϵͳ�Զ�����Ȧ��
        /// </summary>
        public const string CTC_WC_SYSTEMQS = "SYSTEMQS";

        /// <summary>
        /// ����ȡ��ʽ
        /// </summary>
        public const string CTC_WC_BY = "WC_BY";
        #endregion

        #region ��������
        /// <summary>
        /// Դ�ȶ�ʱ��
        /// </summary>
        public const string CTC_OTHER_POWERON_ATTERTIME = "POWERON_ATTERTIME";
        /// <summary>
        /// д�����ȴ�ʱ��
        /// </summary>
        public const string CTC_OTHER_MAXWAITDATABACKTIME = "MAXWAITDATABACKTIME";
        /// <summary>
        /// д��������
        /// </summary>
        public const string CTC_OTHER_EQUIPSET_WAITTIME = "EQUIPSET_WAITTIME";
        /// <summary>
        /// ���Դ���
        /// </summary>
        public const string CTC_OTHER_RETRY = "RETRY";
        /// <summary>
        /// ��׼���Ƶϵ��
        /// </summary>
        public const string CTC_OTHER_DRIVERF = "DRIVERF";
        /// <summary>
        /// ����Ƿ�ɨ��
        /// </summary>
        public const string CTC_READTABLENO = "READTABLENO";
        /// <summary>
        /// �����
        /// </summary>
        public const string CTC_READTABLELENGTH = "READTABLELENGTH";
        #endregion

        #endregion


    }
}
