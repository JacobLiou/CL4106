using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using pwComPorts;
using pwStdMeter;
using pwStdTime;
namespace ClAmMeterController.Test
{
    public partial class frmCL311 : Form
    {

        private CCL1115 m_ccl_CL311V2;
        private pwComPorts.CCL20181 ccl_CL20181;
        //private CCl191 m_ccl_CL191;

        public frmCL311()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 30;
        }

        private void frmCL311_Load(object sender, EventArgs e)
        {
            //this.m_ccl_CL311V2 = new CCL1115();
            //ccl_CL20181 = new pwComPorts.CCL20181();
            //ccl_CL20181.PortOpen("30,193.168.18.1:10003:20000");//30,17,18
            //ccl_CL20181.Setting = "38400,n,8,1";
            //this.m_ccl_CL311V2.ComPort = ccl_CL20181;
            //this.m_ccl_CL311V2.Setting = "38400,n,8,1";

            SelectPort(comboBox1.SelectedIndex + 1);

        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectPort(comboBox1.SelectedIndex + 1);
        }

        private void SelectPort(int intPort)
        {
            if (this.ccl_CL20181 != null)
            {
                this.ccl_CL20181.PortClose();
            }
            m_ccl_CL311V2 = null;
            ccl_CL20181 = null;


            this.m_ccl_CL311V2 = new CCL1115();
            ccl_CL20181 = new pwComPorts.CCL20181();
            ccl_CL20181.PortOpen(intPort.ToString() + ",193.168.18.1:10003:20000");//30,17,18
            ccl_CL20181.Setting = "38400,n,8,1";
            this.m_ccl_CL311V2.ComPort = ccl_CL20181;
            this.m_ccl_CL311V2.Setting = "38400,n,8,1";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool bln_Result = this.m_ccl_CL311V2.link();
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL311V2.LostMessage); 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            string s = "";
            bool bln_Result = this.m_ccl_CL311V2.SetStdMeterUclinemode(0x08);
            label1.Text = bln_Result.ToString() + " " + s + " " + (bln_Result ? "" : this.m_ccl_CL311V2.LostMessage); 
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            string[] s = new string[50];
            bool bln_Result = this.m_ccl_CL311V2.ReadStdMeterInfo(ref s);
            label1.Text = bln_Result.ToString()  + " " + (bln_Result ? "" : this.m_ccl_CL311V2.LostMessage);
            textBox1.Text = s[2];
            textBox2.Text = s[1];
            textBox3.Text = s[0];
            textBox4.Text = s[5];
            textBox5.Text = s[4];
            textBox6.Text = s[3];

        }

        private void button4_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            string[] s = new string[50];
            bool bln_Result = this.m_ccl_CL311V2.SetStdMeterUsE1type(0x00);
            label1.Text = bln_Result.ToString()  + " " + (bln_Result ? "" : this.m_ccl_CL311V2.LostMessage); 
        }

        private void frmCL311_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.ccl_CL20181.PortClose();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            string[] s = new string[50];
            bool bln_Result = this.m_ccl_CL311V2.SetStdMeterUcE1switch(0x01);
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL311V2.LostMessage); 
        }

        private void button6_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            string[] s = new string[50];
            bool bln_Result = this.m_ccl_CL311V2.SetAmMeterParameter(0x08, 0x00,0x01);
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL311V2.LostMessage); 
        }

        private void button7_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            string[] s = new string[50];
            bool bln_Result = this.m_ccl_CL311V2.RunStdMeter();
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL311V2.LostMessage); 
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //label1.Text = "";
            //string[] s = new string[50];
            //long[] t = new long[50];
            //bool bln_Result = this.m_ccl_CL311V2.ReadErrorAndPulse(ref s, ref t);
           
            //label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL311V2.LostMessage); 
        }

        private void button9_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool bln_Result = this.m_ccl_CL311V2.SetStdMeterConst(160000000, false );
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL311V2.LostMessage); 
        }

        private void button10_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool bln_Result = this.m_ccl_CL311V2.SetUIScale(10f,10f,10f,220f,220f,220f);
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL311V2.LostMessage); 
        }

        private void button11_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            textBox8.Text = "";
            long s = 0;
            bool bln_Result = this.m_ccl_CL311V2.ReadStdMeterConst(ref s);
            label1.Text = bln_Result.ToString() + " "+s.ToString() + (bln_Result ? "" : this.m_ccl_CL311V2.LostMessage);
            textBox8.Text = s.ToString();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            //label1.Text = "";
            //long s = 0;
            //bool bln_Result = this.m_ccl_CL311V2.ReadStdMeterPulses (ref s);
            //label1.Text = bln_Result.ToString() + " " + s.ToString() + (bln_Result ? "" : this.m_ccl_CL311V2.LostMessage); 
        }

        private void button13_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool bln_Result = this.m_ccl_CL311V2.SetUIScale(Convert.ToSingle(textBox4.Text), Convert.ToSingle(textBox5.Text), Convert.ToSingle(textBox6.Text), Convert.ToSingle(textBox1.Text), Convert.ToSingle(textBox2.Text), Convert.ToSingle(textBox3.Text));
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL311V2.LostMessage); 
        }

        private void button14_Click(object sender, EventArgs e)
        {
            //this.m_ccl_CL191.Link();

            // 105 251 255 0
            byte[] byt_Tmp ={ 0x69, 0xFB, 0xFF,0x00 };
            if (Convert.ToString( byt_Tmp[2],2).PadLeft(8,'0').Substring(0,1) =="1" )
                byt_Tmp[3] = 0xFF;
            else
                byt_Tmp[3] = 0x0;

            textBox7.Text = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 3));   //B Q
        }
        
    }
}