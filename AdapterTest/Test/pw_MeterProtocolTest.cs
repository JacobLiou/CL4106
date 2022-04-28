using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using pwComPorts;
using pwInterface;

namespace ClAmMeterController.Test
{

    public partial class pw_MeterProtocolTest : Form
    {
        /// <summary>
        /// �춨��Ŀ���
        /// </summary>

        private static ClAmMeterController.CMultiController m_485Control = null;
        private static ClAmMeterController.pwMeterProtocolInfo m_ProtocolInfo = null;

        public pw_MeterProtocolTest()
        {
            InitializeComponent();
        }

        private void pw_MeterProtocolTest_Load(object sender, EventArgs e)
        {
            m_485Control = new ClAmMeterController.CMultiController(24);
            m_ProtocolInfo = new ClAmMeterController.pwMeterProtocolInfo();


        }

        private void pw_MeterProtocolTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_485Control = null;
            m_ProtocolInfo = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            m_ProtocolInfo = new ClAmMeterController.pwMeterProtocolInfo("DLT645_2007");
            m_485Control.OnEventMultiControlle += new DelegateEventMultiController(OnEventMultiControllerh);
            m_485Control.RefreshProtocol(m_ProtocolInfo);
            m_485Control.HighFrequencyPulse(5);
        }
        private void OnEventMultiControllerh(enmMeterPrjID enm_PrjID, int int_BwNo, bool bln_Result, string str_Value)
        {
            switch (enm_PrjID)
            {
                case enmMeterPrjID.���������:
                    //if (GlobalUnit.g_Meter.MData[int_BwNo].MeterResults.ContainsKey(((int)enm_PrjID).ToString()))
                    //{
                    //    GlobalUnit.g_Meter.MData[int_BwNo].MeterResults.Remove(((int)enm_PrjID).ToString());
                    //}
                    //DataResultBasic dataResule = new DataResultBasic();
                    //dataResule.Me_PrjID = Convert.ToString((int)enm_PrjID);
                    //dataResule.Me_PrjName = enm_PrjID.ToString() + "����";
                    //dataResule.Me_Result = bln_Result ? Variable.CTG_HeGe : Variable.CTG_BuHeGe;
                    //dataResule.Me_Value = str_Value;

                    //GlobalUnit.g_Meter.MData[int_BwNo].MeterResults.Add(Convert.ToString((int)enm_PrjID), dataResule);

                    //pwResultScbh.enmResult = (bln_Result ? pwInterface.enmMeterResult.�ϸ� : pwInterface.enmMeterResult.���ϸ�);
                    //GlobalUnit.g_Meter.MData[int_BwNo].chrScbh = strReadScbhReturn;

                    break;
                //case enmMeterPrjID.���춨:
                //    MeterErrors.Clear();
                //    if (MeterResults.ContainsKey(((int)enmMeterPrjID.���춨).ToString()))
                //    {
                //        MeterResults.Remove(((int)enmMeterPrjID.���춨).ToString());
                //    }

                //    break;
                //case enmMeterPrjID.�๦�ܼ춨:
                //    MeterDgns.Clear();
                //    if (MeterResults.ContainsKey(((int)enmMeterPrjID.�๦�ܼ춨).ToString()))
                //    {
                //        MeterResults.Remove(((int)enmMeterPrjID.�๦�ܼ춨).ToString());
                //    }

                //    break;
                //case enmMeterPrjID.�����������:
                //    MeterDowns.Clear();
                //    if (MeterResults.ContainsKey(((int)enmMeterPrjID.�����������).ToString()))
                //    {
                //        MeterResults.Remove(((int)enmMeterPrjID.�����������).ToString());
                //    }
                //    break;
                //case enmMeterPrjID.ϵͳ����:
                //    if (MeterResults.ContainsKey(((int)enmMeterPrjID.ϵͳ����).ToString()))
                //    {
                //        MeterResults.Remove(((int)enmMeterPrjID.ϵͳ����).ToString());
                //    }
                //    break;
                default:
                    break;

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            m_ProtocolInfo = new ClAmMeterController.pwMeterProtocolInfo("DLT645_1997");
            m_485Control.OnEventMultiControlle += new DelegateEventMultiController(OnEventMultiControllerh);
            m_485Control.RefreshProtocol(m_ProtocolInfo);
            m_485Control.ReadScbh("FFF9");
        }

    }
}