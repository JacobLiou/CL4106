using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using pwFunction.pwConst;
using System.Diagnostics;
using pwInterface;
using pwFunction.pwMeter;
using System.Windows.Forms;
namespace VerifyAdapter
{

    /// <summary>
    /// �������춨������
    /// </summary>
    public class BasicError
    {


        #region -----------����----------
        private Stopwatch sth_SpaceTicker = new Stopwatch();        //��ʱʱ��
        private VerifyAdapter.EquipUnit m_EquipUnit = null;         //װ��
        private CheckPoint m_CheckPoint = new CheckPoint();         //�춨��(�춨������)
        private MeterErrorItem[] m_Error = new MeterErrorItem[24];  //�������(���㼰�洢��)
        //=============
        private int m_BwCount = 24;                                 //��λ��
        private bool m_Stop = false;                                //�Ƿ�ֹͣ
        protected bool m_CheckOver = false;                         //�Ƿ��Ѿ���ɼ춨
        private bool[] m_IsCheck = new bool[24];                    //Ҫ��
        private stPrjParameter m_stPrjParameter;                    //��Ŀ�����ṹ
        //private int m_DLHL = 0;                                     //������·

        #region -------���������ԣ���ϵͳ���ÿɶ�ȡ----------
        /// <summary>
        /// ÿһ������ȡ�������������(�������)
        /// </summary>
        private int m_WCTimesPerPoint = GlobalUnit.WCTimesPerPoint;

        /// <summary>
        /// ��׼ƫ���ȡ�������������
        /// </summary>
        private int m_WindageTimesPerPoint = GlobalUnit.WindageTimesPerPoint;

        /// <summary>
        /// ÿһ����������ȡ���ٴ����(������)
        /// </summary>
        private int m_WCMaxTimes = GlobalUnit.WCMaxTimes;

        /// <summary>
        /// ÿһ����������������(�ʱ��)
        /// </summary>
        private int m_WCMaxSeconds = GlobalUnit.WCMaxSeconds;

        /// <summary>
        /// �����ж���׼
        /// </summary>
        private float m_WCJump = GlobalUnit.WCJump;

        /// <summary>
        /// ��Ƶ�춨ʱ��������
        /// </summary>
        private int m_WcConstantBs = 8;//GlobalUnit.WcConstantBs;

        /// <summary>
        ///��׼�����Ƶϵ��
        /// </summary>
        private float m_WcDriverF = GlobalUnit.WcDriverF;

        #endregion
        #endregion

        #region -----------����----------

        public CheckPoint CheckPoint
        {
            set
            {
                m_CheckPoint = value;
            }
            get
            {
                return m_CheckPoint;
            }
        }

        /// <summary>
        /// ֹͣ�춨
        /// </summary>
        public bool IsStop
        {
            set
            {
                m_Stop = value;
                m_CheckOver = value;
                m_EquipUnit.StopTask(enmTaskType.�������);
            }
            get { return m_Stop; }
        }

        /// <summary>
        /// �Ƿ�Ҫ��
        /// </summary>
        public bool[] blnSelected
        {
            get { return m_IsCheck; }
            set { m_IsCheck = value; }
        }

        /// <summary>
        /// ��Ŀ�����ṹ
        /// </summary>
        public stPrjParameter stPrjParameter
        {
            set
            {
                m_stPrjParameter = value;
                m_WcConstantBs = m_stPrjParameter.int_HighFrequencyPulseBS;
            }
        }

        public event DelegateEventMultiController OnEventMultiControlle;

        #endregion


        #region -----------����----------

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="BwCount"></param>
        /// <param name="ComAdpater"></param>
        public BasicError(int BwCount, VerifyAdapter.EquipUnit ComAdpater)
        {
            m_BwCount = BwCount;
            m_EquipUnit = ComAdpater;
            m_IsCheck = new bool[m_BwCount];

        }
        #endregion 

        #region -----------����----------

        /// <summary>
        /// �춨
        /// </summary>
        public void Verify()
        {

            #region ----------�춨׼��----------
            GlobalUnit.g_MsgControl.OutMessage("��ʼ��ʼ����ǰ���춨�����...", false);
            int _TableHeader = m_WCTimesPerPoint;           //��¼������ݣ��������ã������������Ч������
            int _MaxWCnum = m_WCMaxTimes;                   //���������
            m_CheckOver = false;                            //��ԭ�춨��ʶ��û�м춨���
            m_Stop = false;
            #endregion

            #region ----------��ʼ���豸������[�������]:InitEquipment----------
            if (!InitEquipment())
            {
                #region ��Դʧ��
                for (int i = 0; i < m_BwCount; i++)
                {
                    if (!m_IsCheck[i]) continue;
                    m_Error[i] = new MeterErrorItem();
                    m_Error[i].Me_Bw = i;
                    m_Error[i].Item_PrjID = m_CheckPoint.PrjID;
                    m_Error[i].Item_PrjName = m_CheckPoint.PrjName;
                    m_Error[i].Item_Result = Variable.CTG_BuHeGe;
                    if (!m_IsCheck[i]) continue;
                    if (GlobalUnit.g_Meter.MData[i].MeterErrors.ContainsKey(m_CheckPoint.PrjID))
                    {
                        GlobalUnit.g_Meter.MData[i].MeterErrors.Remove(m_CheckPoint.PrjID);
                    }
                    GlobalUnit.g_Meter.MData[i].MeterErrors.Add(m_CheckPoint.PrjID, m_Error[i]);

                    if (this.OnEventMultiControlle != null) this.OnEventMultiControlle(enmMeterPrjID.���춨, i, false, "��Դʧ��");

                }
                #endregion
                return;
            }
            #endregion

            #region ----------����----------
            string[] _CurWC = new string[m_BwCount];          //��ǰ���
            int[] _CurrentWcNum = new int[m_BwCount];         //��ǰ�ۼƼ춨����
            int[] _MeterWcNum = new int[m_BwCount];           //��λȡ����������������255��
            int[] _VerifyTimes = new int[m_BwCount];          //��Ч������
            bool[] _CheckOver = new bool[m_BwCount];          //�Ƿ��Ѿ���ɱ��μ춨

            GlobalUnit.g_MsgControl.OutMessage(pwClassLibrary.enmMessageType.�����Ϣ����);
            int ReadWCNum = 0;
            sth_SpaceTicker.Reset();
            sth_SpaceTicker.Start();       //��ʼ��ʱ             
            #endregion

            #region ----------ѭ���춨----------
            while (true)
            {
                #region �����
                GlobalUnit.g_MsgControl.OutMessage("���ڼ춨��ǰ���㣺" + m_CheckPoint.PrjName, false);
                Application.DoEvents();
                if (m_CheckOver || GlobalUnit.ForceVerifyStop || m_Stop)//ȫ����춨��ɣ��ⲿ��ֹ�����˳�
                {
                    GlobalUnit.g_MsgControl.OutMessage("��ǰ�㱻�춨ֹͣ...", false);
                    break;
                }
                if (Convert.ToInt32(sth_SpaceTicker.ElapsedMilliseconds / 1000) > m_WCMaxSeconds)
                {
                    GlobalUnit.g_MsgControl.OutMessage("��ǰ��춨�Ѿ��������춨ʱ��" + m_WCMaxSeconds + "�룡", false);
                    break;
                }

                _CurWC = new string[m_BwCount];               //���³�ʼ���������
                _CurrentWcNum = new int[m_BwCount];           //���³�ʼ������������

                Thread readstdinfo = new Thread(new ThreadStart(m_EquipUnit.ReadStdInfo));
                readstdinfo.Start();
                Application.DoEvents();

                if (m_CheckPoint.xIb <= 0.1f)
                {
                    m_EquipUnit.DelayTime(m_CheckPoint.OneTime);
                }

                if (!ReadData(ref _CurWC, ref _CurrentWcNum))
                {
                    continue;
                }
                ReadWCNum++;
                GlobalUnit.g_MsgControl.OutMessage("��" + ReadWCNum.ToString() + "�ζ�ȡ�����ɣ���ʼ��������...", false);
                #endregion

                #region ��������
                for (int i = 0; i < m_BwCount; i++)
                {
                    #region
                    Application.DoEvents();

                    #region �˳��ж�
                    if (!m_IsCheck[i])//Ҫ��
                    {
                        continue;
                    }
                    if (m_Stop || GlobalUnit.ForceVerifyStop)//ֹͣ
                    {
                        GlobalUnit.g_MsgControl.OutMessage("�춨ֹͣ���������", false);
                        break;
                    }
                    if (Convert.ToInt32(sth_SpaceTicker.ElapsedMilliseconds / 1000) > m_WCMaxSeconds)//��ʱ
                    {
                        GlobalUnit.g_MsgControl.OutMessage("��ǰ��춨�Ѿ��������춨ʱ��" + m_WCMaxSeconds + "�룡", false);
                        break;
                    }
                    #endregion

                    //ȥ����һ�δ����
                    if (!_CheckOver[i] && _CurrentWcNum[i] > 1)
                    {
                        #region ----------���ݺϷ��Լ��----------
                        /*
                        ������255�ε����
                        */
                        if (_MeterWcNum[i] > 0 && _CurrentWcNum[i] < _MeterWcNum[i])
                        {
                            if (_MeterWcNum[i] > _CurrentWcNum[i])
                            {
                                _CurrentWcNum[i] += 255;
                            }
                        }
                        if (_MeterWcNum[i] < _CurrentWcNum[i])
                        {
                            _MeterWcNum[i] = _CurrentWcNum[i] % 255;
                            _VerifyTimes[i]++;
                        }
                        if (_CurrentWcNum[i] == 0 || _CurrentWcNum[i] == 255)//�������λû�г�������һ��
                        {
                            _CheckOver[i] = false;
                            continue;            
                        }
                        #endregion

                        #region ������,���һ�������������ǰ��
                        if (_VerifyTimes[i] > 1)
                        {
                            //if (_VerifyTimes[i] > m_Error[i].Wcn.Length) _VerifyTimes[i] = m_Error[i].Wcn.Length;
                            //for (int dtPos = _VerifyTimes[i]; dtPos > 0; dtPos--)
                            for (int dtPos = m_WCTimesPerPoint - 1; dtPos > 0; dtPos--)
                            {
                                m_Error[i].Wcn[dtPos] = m_Error[i].Wcn[dtPos - 1];
                            }
                        }
                        m_Error[i].Wcn[0] = Convert.ToSingle(_CurWC[i]);     //���һ�����ʼ�շ��ڵ�һλ
                        #endregion

                        #region �����������
                        if (_VerifyTimes[i] < m_WCTimesPerPoint)
                            m_Error[i].VerifyTimes = _VerifyTimes[i];
                        else
                            m_Error[i].VerifyTimes = m_WCTimesPerPoint;

                        WuChaDeal.SetWuCha(ref m_Error[i]);
                        Application.DoEvents();


                        if (m_Error[i].Item_Result == Variable.CTG_HeGe)
                        {
                            _CheckOver[i] = true;
                        }


                        #endregion

                        #region ����ж�
                        /*
                     * ����Ƿ����б��ϸ�
                     * ����춨�����Ѿ�����ÿ����춨��������������˵��Ƿ��Ѿ��ϸ�������ϸ���
                     * �ȼ�⵱ǰ�������Ƿ��Ѿ��ﵽÿ����������������ǰ��춨ʱ���Ƿ��Ѿ�����
                     * Ԥ�����ʱ�䡣�ǣ�����Ϊ�˱��ϸ񡣷����������ǰ�������ؼ졣
                     * _VerifyTimes[i] > _MaxWCnum ��ʱ��ѵ�һ����������������
                     */
                        if (_VerifyTimes[i] >= m_WCTimesPerPoint)//������С����
                        {
                            
                            if (m_Error[i].Item_Result == Variable.CTG_HeGe)//�ϸ�������
                            {
                                _CheckOver[i] = true;
                            }
                            else //���ϸ�
                            {
                                if (_VerifyTimes[i] >= _MaxWCnum)//����������
                                {
                                    _CheckOver[i] = true;
                                }
                                else
                                {
                                    _CheckOver[i] = false ;
                                }
                            }
                        }
                        else
                        {
                            _CheckOver[i] = false;
                        }
                        #endregion
                    }
                    else if (_CurrentWcNum[i] < 1)//û������������һ��ϸ����ж�������Ϊ���ϸ��˳��춨���
                    {
                        #region û������
                        int intJdOverCount = 0;
                        for (int k = 0; k < GlobalUnit.g_BW; k++)
                        {
                            if (_CheckOver[k] && (_CurrentWcNum[k] > _TableHeader)) intJdOverCount++;
                        }
                        if (intJdOverCount > 1)
                        {
                            _CheckOver[i] = true;
                            m_Error[i].Item_Result = Variable.CTG_BuHeGe;
                        }
                        Application.DoEvents();
                        #endregion
                    }
                    #endregion
                }
                #endregion

                #region ������
                m_CheckOver = true;
                for (int j = 0; j < m_BwCount; j++)
                {
                    if (!m_IsCheck[j]) continue;
                    if (!_CheckOver[j])
                    {
                        GlobalUnit.g_MsgControl.OutMessage(string.Format("��{0}���û��ͨ��", j + 1));
                        m_CheckOver = false;
                        break;
                    }
                }
                Application.DoEvents();

                #endregion
            }
            #endregion

            #region ----------��������----------
            for (int i = 0; i < m_BwCount; i++)
            {
                if (!m_IsCheck[i]) continue;
                if (GlobalUnit.g_Meter.MData[i].MeterErrors.ContainsKey(m_CheckPoint.PrjID))
                {
                    GlobalUnit.g_Meter.MData[i].MeterErrors.Remove(m_CheckPoint.PrjID);
                }
                GlobalUnit.g_Meter.MData[i].MeterErrors.Add(m_CheckPoint.PrjID, m_Error[i]);

                bool IsAllHeGe = true;
                IsAllHeGe = isHeGeOtherWcPoint(GlobalUnit.g_Meter.MData[i].MeterErrors);
                if (this.OnEventMultiControlle != null) this.OnEventMultiControlle(enmMeterPrjID.���춨, i, IsAllHeGe, IsAllHeGe ? "" : m_CheckPoint.PrjName + "��" + m_Error[i].ToString());

            }
            m_EquipUnit.StopTask(enmTaskType.�������);
            GlobalUnit.g_MsgControl.OutMessage("��ǰ�춨��춨���");
            Application.DoEvents();
            #endregion
        }
        #endregion

        #region -----------˽��----------

        /// <summary>
        /// ���������Ƿ�ϸ�
        /// </summary>
        /// <param name="curMeter"></param>
        /// <returns></returns>
        private bool isHeGeOtherWcPoint(Dictionary<string, MeterErrorItem> curMeter)
        {
            bool isAllItemOk = true;
            foreach (string strKey in curMeter.Keys)
            {
                //��ǰ���ʷ���
                MeterErrorItem _Item = curMeter[strKey];
                if (_Item.Item_Result != Variable.CTG_HeGe)
                {
                    isAllItemOk = false;
                    break;
                }
            }
            return isAllItemOk;
        }

        /// <summary>
        /// ��ȡ�춨����,��������Ϊ��ǰ�춨����
        /// </summary>
        /// <param name="arrData">�������飬ÿ��λ��Ӧ</param>
        /// <param name="arrWcCount">���������飬ÿ��λ��Ӧ</param>
        /// <returns>�Ƿ��ȡ�ɹ�</returns>
        private bool ReadData(ref string[] arrData, ref int[] arrWcCount)
        {
            bool[] bln_Result = new bool[24];
            if (!m_EquipUnit.ReadTaskData(0x00,ref bln_Result , ref arrData, ref arrWcCount))
            {
                Thread.Sleep(10);
                return false;
            }
            /*
            ��ȡ������������������������һ����������Ϊ0�Ļ�����Ϊȫ����û�г����
            */
            int[] _CurrentWcNumCopy = (int[])arrWcCount.Clone();
            Array.Sort(_CurrentWcNumCopy);
            if (_CurrentWcNumCopy[_CurrentWcNumCopy.Length - 1] == 0)
            {
                Thread.Sleep(1);
                return false;
            }
            return true;
        }

        /// <summary>
        /// ��ʼ���豸������[�������]
        /// </summary>
        /// <returns></returns>
        private bool InitEquipment()
        {
            /*
            ѡͨ��--�����ñ�������--�����ù��ܲ���-->���ù�����-->������ǰ����
            */
            //��һ��Ĭ��Ϊ��������й�����
            //if (GlobalUnit.IsDemo) return true;

            bool SetTestPointResult = false;

            float PowerIb = m_CheckPoint.xIb * GlobalUnit.g_Meter.MInfo.Ib;

            float PowerUb = GlobalUnit.g_Meter.MInfo.Ub;

            #region ----------------���û�·----------------
            //if (m_DLHL != m_CheckPoint.DLHL)
            //{
            //    m_EquipUnit.PowerOff();
            //    m_EquipUnit.DelayTime(200);

            //    SetTestPointResult = m_EquipUnit.SetCurrentLoop(m_CheckPoint.DLHL);
            //    if (!SetTestPointResult)
            //    {
            //        if (!m_Stop)
            //            pwClassLibrary.Check.Require(false, "���õ�����·ʧ��" + m_EquipUnit.m_IComAdpater.LostMessage);

            //    }
            //    //�ȴ�Դ�ȶ�
            //    m_EquipUnit.DelayTime(200);
            //}
            //m_DLHL = m_CheckPoint.DLHL;
            //if (GlobalUnit.ForceVerifyStop) return false;

            #endregion

            #region ----------------�����ѹ����----------------

            pwFunction.pwConst.GlobalUnit.g_MsgControl.OutMessage("��ʼ���ü춨�������"
                + GlobalUnit.g_Meter.MInfo.Clfs.ToString() + " | "
                + PowerUb.ToString() + " | "
                + PowerIb.ToString() + " | "
                + m_CheckPoint.GLYS + " | "
                + m_CheckPoint.YJ+ " | "
                , false);
            SetTestPointResult = m_EquipUnit.SetTestPoint(GlobalUnit.g_Meter.MInfo.Clfs, PowerUb, PowerIb, m_CheckPoint.GLYS, m_CheckPoint.YJ, GlobalUnit.g_Meter.MInfo.PL);
            if (!SetTestPointResult)
            {
                if (!m_Stop)
                    pwClassLibrary.Check.Require(false, "���ü춨��ʧ��" );

            }
            if (m_CheckPoint.xIb <= 0.05f)//С����ʱ��׼�������ȣ���Ҫ���ӵȴ�ʱ��
            {
                m_EquipUnit.DelayTime(6000);//�ȴ�Դ�ȶ�
            }
            else
            {
                m_EquipUnit.DelayTime(2000);//�ȴ�Դ�ȶ�
            }
            if (GlobalUnit.ForceVerifyStop) return false;
            #endregion

            #region ----------------���ù��ܲ���----------------
            byte intBs = 1;//��������
            long lngConstant = Convert.ToInt64(GlobalUnit.g_Meter.MInfo.Constant);
            if (PowerIb < GlobalUnit.g_Meter.MInfo.Ib)
            {
                //intBs = (byte)m_WcConstantBs;
                lngConstant = lngConstant * m_WcConstantBs;
            }

            enmChannelType _CurTdType = enmChannelType.�����й�;
            switch (m_CheckPoint.GLFX)
            {
                case enmsPowerFangXiang.�����й�:
                    _CurTdType = enmChannelType.�����й�;
                    break;
                case enmsPowerFangXiang.�����й�:
                    _CurTdType = enmChannelType.�����й�;
                    break;
                case enmsPowerFangXiang.�����޹�:
                    _CurTdType = enmChannelType.�����޹�;
                    break;
                case enmsPowerFangXiang.�����޹�:
                    _CurTdType = enmChannelType.�����޹�;
                    break;
            }

            pwFunction.pwConst.GlobalUnit.g_MsgControl.OutMessage("��ʼ���ù��ܲ�����"
                + GlobalUnit.g_Meter.MInfo.enmDzType.ToString() + " | "
                + enmTaskType.�������.ToString() + " | "
                + _CurTdType.ToString() + " | "
                + GlobalUnit.g_Meter.MInfo.PulseType.ToString() + " | "
                + GlobalUnit.g_Meter.MInfo.GYGY.ToString() + " | "
                + lngConstant.ToString() + " | "
                + m_CheckPoint.QS.ToString() + " | "
                + intBs.ToString() + " | "
               , false);
            if (!m_EquipUnit.SetTaskParameter(
                                            GlobalUnit.g_Meter.MInfo.enmDzType
                                            , enmTaskType.�������
                                            , _CurTdType
                                            , GlobalUnit.g_Meter.MInfo.PulseType// enmPulseComType.�����
                                            , GlobalUnit.g_Meter.MInfo.GYGY //enmGyGyType.����
                                            , lngConstant
                                            , Convert.ToInt64(m_CheckPoint.QS)
                                            , intBs)
                )
            {
                m_Stop = true;
                pwFunction.pwConst.GlobalUnit.g_MsgControl.OutMessage("��ʼ���ù��ܲ�����ʧ��");
                return false;
                //pwClassLibrary.Check.Require(false, "���ù��ܲ���ʧ��!" + m_EquipUnit.m_IComAdpater.LostMessage);

            }
            if (GlobalUnit.ForceVerifyStop) return false;
            #endregion






            #region ----------------��ʼ�������----------------

            if (!InitErrorData())
                return false ;
            if (GlobalUnit.ForceVerifyStop) return false;
            #endregion

            return true;
        }

        /// <summary>
        /// ��ʼ�����춨����
        /// </summary>
        /// <returns></returns>
        private bool InitErrorData()
        {
            try
            {
                m_Error = new MeterErrorItem[m_BwCount];

                for (int i = 0; i < m_Error.Length; i++)
                {
                    m_Error[i] = new MeterErrorItem();
                    m_Error[i].Me_Bw = i;
                    m_Error[i].Item_PrjID = m_CheckPoint.PrjID;
                    m_Error[i].Item_PrjName = m_CheckPoint.PrjName;
                    m_Error[i].MeterLevel = GlobalUnit.g_Meter.MInfo.DJ;
                    m_Error[i].MaxError = m_CheckPoint.MaxError;
                    m_Error[i].MinError = m_CheckPoint.MinError;
                    m_Error[i].intPc = m_CheckPoint.PC;

                    if (m_CheckPoint.PC == 1)//ƫ�����
                    {
                        m_WCTimesPerPoint = m_WindageTimesPerPoint;// GlobalUnit.WCTimesPerPoint;//��С����
                        m_WCMaxTimes = m_WindageTimesPerPoint + 2;// GlobalUnit.WCMaxTimes;//������
                        m_WCMaxSeconds = GlobalUnit.WCMaxSeconds * 2 ;//ʱ��
                        m_Error[i].VerifyTimes = m_WindageTimesPerPoint;
                    }
                    else
                    {
                        m_WCTimesPerPoint = GlobalUnit.WCTimesPerPoint;//��С����
                        m_WCMaxTimes = GlobalUnit.WCMaxTimes;//������
                        m_WCMaxSeconds = GlobalUnit.WCMaxSeconds;//ʱ��
                        m_Error[i].VerifyTimes = m_WCTimesPerPoint;

                    }

                }
                return true;
            }
            catch
            {
                return false;
            }
        }
  
        #endregion


    }


}