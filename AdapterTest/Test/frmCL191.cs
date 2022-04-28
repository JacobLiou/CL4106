using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using pwStdTime;
using System.Diagnostics;
using pwClassLibrary;
namespace ClAmMeterController.Test
{
    public partial class frmCL191 : Form
    {
        private pwComPorts.CCL20181 ccl_CL20181;
        private CCL191 m_ccl_CL191;

        private Stopwatch sth_SpaceTicker = new Stopwatch();                //记时时钟

        public frmCL191()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 20;

        }
        private void frmCL191_Load(object sender, EventArgs e)
        {
            //this.m_ccl_CL191 = new CCL191();
            //ccl_CL20181 = new pwComPorts.CCL20181();
            //ccl_CL20181.PortOpen("21,193.168.18.1:10003:20000");//32,21
            //ccl_CL20181.Setting = "2400,n,8,1";
            //this.m_ccl_CL191.ComPort = ccl_CL20181;
            //this.m_ccl_CL191.Setting = ccl_CL20181.Setting;

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
            m_ccl_CL191 = null;
            ccl_CL20181 = null;

            this.m_ccl_CL191 = new CCL191();
            ccl_CL20181 = new pwComPorts.CCL20181();
            ccl_CL20181.PortOpen(intPort.ToString() + ",,193.168.18.1:10003:20000");//32,21
            ccl_CL20181.Setting = "2400,n,8,1";
            this.m_ccl_CL191.ComPort = ccl_CL20181;
            this.m_ccl_CL191.Setting = ccl_CL20181.Setting;
        }

        private void frmCL191_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.ccl_CL20181.PortClose();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool bln_Result = this.m_ccl_CL191.Link();
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL191.LostMessage);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool bln_Result = this.m_ccl_CL191.SetChannel(1);
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL191.LostMessage);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool bln_Result = this.m_ccl_CL191.SetChannel(0);
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL191.LostMessage);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string strtime = "";
            label1.Text = "";
            bool bln_Result = this.m_ccl_CL191.ReadGPSTime(ref strtime);
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL191.LostMessage);
            if(bln_Result) textBox1.Text = strtime;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            bool bln_Result = pwClassLibrary.pwWin32Api.Win32Api.SetSystemDateTime(textBox1.Text);
            label1.Text = bln_Result.ToString() + "更改系统时间" + (bln_Result ? "成功" : "失败");

        }



    }
}