using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using pwCommAdapter;
namespace ClAmMeterController.Test
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }
        private void frmMain_Load(object sender, EventArgs e)
        { 
            //long lngTmp2= GetStdPulsePl(4000000, 2200);
            double[] UiPhi = new double[6];
            bool rt = SetAcSourcePowerFactor("0.5L", ref UiPhi);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmCL303 frmcl303 = new frmCL303();
            frmcl303.ShowDialog(); 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmCL311 frmcl311 = new frmCL311();
            frmcl311.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            frmCL188 frmcl188 = new frmCL188();
            frmcl188.ShowDialog();
        }


        private void button4_Click(object sender, EventArgs e)
        {
            frmCL191 frmcl191 = new frmCL191();
            frmcl191.ShowDialog();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            frmCL485 frmcl485 = new frmCL485();
            frmcl485.ShowDialog();
        }

        /// <summary>
        /// 设置源功率因数
        /// </summary>
        /// <param name="Glys">功率因数如(0.5L,1.0,-1,-0,0.5C)</param>
        /// <param name="jxfs">0-三相四线有功表PT4;1-三相三线有功表P32;2--三相四线真无功表(QT4);3--三相三线真无功表(Q32);4--三元件跨相90无功表(Q33);5--二元件跨相90无功表(Q90);6--二元件人工中点(60)无功表(Q60)</param>
        /// <returns></returns>
        public bool SetAcSourcePowerFactor(string Glys, ref double  [] UiPhi)
        {

            double XwUa = 0;
            double XwUb = 0;
            double XwUc = 0;
            double XwIa = 0;
            double XwIb = 0;
            double XwIc = 0;
            string strGlys = "";
            string LC = "";
            double LcValue = 0;
            double Phi = 0;
            int n = 1;
            strGlys = Glys;



            if (Glys == "0") strGlys = "0L";
            if (Glys == "-0") strGlys = "0C";
            LC = GetUnit(strGlys);
            if (LC.Length > 0)
            {
                LcValue = Convert.ToDouble(strGlys.Replace(LC, ""));
            }
            else
            {
                LcValue = Convert.ToDouble(strGlys);
            }


            #region
            Phi = Math.Round(Math.Acos(LcValue) * (180 / Math.PI), 5);
            XwUa = 0;
            if (LcValue > 0)
            {
                XwIa = 0;
                Phi = 1 * Phi;

            }
            else if (LcValue < 0)
            {
                XwIa = 180;
                Phi = -1 * Phi;
            }
            if (LC == "L")
            {
                Phi = 1 * Phi;
                XwIa = XwUa - Phi;
                if (XwIa < 0) XwIa = XwIa + 360;
                if (XwIa >= 360) XwIa = XwIa - 360;

            }
            if (LC == "C")
            {
                Phi = -1 * Phi;
                XwIa = XwUa - Phi;
                if (XwIa < 0) XwIa = XwIa + 360;
                if (XwIa >= 360) XwIa = XwIa - 360;
            }
            XwUb = XwUa;
            XwUc = XwUa;
            XwIb = XwIa;
            XwIc = XwIa;
            #endregion

            UiPhi[0] = XwUa;
            UiPhi[1] = XwUb;
            UiPhi[2] = XwUc;
            UiPhi[3] = XwIa;
            UiPhi[4] = XwIb;
            UiPhi[5] = XwIc;

            //m_UaXwValue = XwUa;
            //m_UbXwValue = XwUb;
            //m_UcXwValue = XwUc;

            //m_IaXwValue = XwIa;
            //m_IbXwValue = XwIb;
            //m_IcXwValue = XwIc;

            return true;
        }

        private string GetUnit(string chrVal)  //得到量程的单位 //chrVal带单位的值如 15A
        {
            int i = 0;
            string cUnit = "";
            byte[] chrbytes = new byte[256];
            ASCIIEncoding ascii = new ASCIIEncoding();
            chrbytes = ascii.GetBytes(chrVal);
            for (i = 0; i < chrbytes.Length; ++i)
            {
                if (chrbytes[i] > 57)
                {
                    cUnit = chrVal.Substring(i);
                    break;
                }

            }
            return cUnit;
        }
        private long GetStdPulsePl(long StdPulse, float _CurP)
        {//标准脉冲常数/（3600*1000/P）
            long lngTmp;
            lngTmp = (StdPulse / (3600 * 1000)) * (long)_CurP;

            lngTmp = Convert.ToInt64(StdPulse / (3600 * 1000 / _CurP)) ;

            return lngTmp;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            pwFkTest frmFk = new pwFkTest();
            frmFk.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            frmCLMainControl frmMc = new frmCLMainControl();
            frmMc.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            frmCLPowerConsume frmMc = new frmCLPowerConsume();
            frmMc.ShowDialog();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            frmCL485Adjust frmMc = new frmCL485Adjust();
            frmMc.ShowDialog();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            frmCL485_HW_Test frmMc = new frmCL485_HW_Test();
            frmMc.ShowDialog();
        }


 
    }
}