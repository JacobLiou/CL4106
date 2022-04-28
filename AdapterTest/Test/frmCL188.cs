using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using pwErrorCalculate;
using pwComPorts;
namespace ClAmMeterController.Test
{
    //    ËùÓÐÎó²î°åRxIDÎª£º1
    public partial class frmCL188 : Form
    {
        private CCL188L m_ccl_CL188;
        private CCL20181 ccl_CL20181;
        //private CSerialCom ccl_CL20181;
        public frmCL188()
        {
            InitializeComponent();
        }

        private void frmCL188_Load(object sender, EventArgs e)
        {
            m_ccl_CL188 = new CCL188L();
            ccl_CL20181 = new CCL20181();//new CSerialCom();//
            ccl_CL20181.PortOpen("31,193.168.18.1:10003:20000");

            ccl_CL20181.Setting = "38400,n,8,1";
            m_ccl_CL188.ComPort = ccl_CL20181;
            m_ccl_CL188.SetECListTable(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 });

            //comboBox1.SelectedIndex = 0;

        }

        private void SelectBw()
        {
            int[] PortList = { 7, 8, 15, 16, 31, 32 };

            int Index =PortList[ comboBox1.SelectedIndex ];

            if (this.ccl_CL20181 != null) this.ccl_CL20181.PortClose();
            this.m_ccl_CL188 = null;
            this.ccl_CL20181 = null;

            this.m_ccl_CL188 = new CCL188L(1);
            this.ccl_CL20181 =  new pwComPorts.CCL20181();//new CSerialCom();//
            this.ccl_CL20181.PortOpen(Index.ToString() + ",193.168.18.1:10003:20000");

            this.ccl_CL20181.Setting = "38400,n,8,1";
            this.m_ccl_CL188.ComPort = ccl_CL20181;
            int[] EcListTable = new int[] { comboBox1.SelectedIndex + 1 };
            this.m_ccl_CL188.SetECListTable(EcListTable);// (new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 });


        }

        private void frmCL188_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.ccl_CL20181.PortClose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool[] b = new bool[24];
            bool bln_Result = this.m_ccl_CL188.Link(ref b);//Link(ref b);
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL188.LostMessage);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool bln_Result = this.m_ccl_CL188.SelectPulseChannel(0,0,0);
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL188.LostMessage); 
        }


        private void button3_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            long lAmeConst = Convert.ToInt64(textBox1.Text);
            long lStdConst = Convert.ToInt64(textBox2.Text);

            bool bln_Result = this.m_ccl_CL188.SetDnWcrPara(lAmeConst,1, lStdConst, 2200, 1);
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL188.LostMessage); 
        }

        private void button4_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool bln_Result = this.m_ccl_CL188.StartCalculate(0);
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL188.LostMessage); 
        }

        private void button5_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool bln_Result = this.m_ccl_CL188.StopCalculate(0);
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL188.LostMessage); 
        }

        private void button6_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool[] b = new bool[24];
            bool bln_Result = this.m_ccl_CL188.SelectPulseChannel(5,0,0);
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL188.LostMessage);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool bln_Result = this.m_ccl_CL188.StartCalculate(2);
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL188.LostMessage); 
        }

        private void button8_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool[] b = new bool[1];
            string[] s = new string[1];
            int[] i = new int[1];
            bool bln_Result = this.m_ccl_CL188.ReadData(0x02,ref b, ref i, ref s);
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL188.LostMessage); 
        }

        private void button9_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool bln_Result = this.m_ccl_CL188.SetTimePara(255,500000, 1f, 1);
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL188.LostMessage); 

        }

        private void button10_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool bln_Result = this.m_ccl_CL188.SetCommSwitch(255,true);
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL188.LostMessage);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool bln_Result = this.m_ccl_CL188.SetDLSwitch(0);
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL188.LostMessage);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool bln_Result = this.m_ccl_CL188.SetDLSwitch(1);
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL188.LostMessage);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool bln_Result = this.m_ccl_CL188.SetMeterPulseDzType(255,1);
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL188.LostMessage);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool bln_Result = this.m_ccl_CL188.SetMeterPulseDzType(255, 0);
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL188.LostMessage);

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectBw();
        }




    }
}