using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using pwErrorCalculate;
using pwComPorts;
using pwPower;
using pwMainControl;
namespace ClAmMeterController.Test
{
    public partial class frmCL303 : Form
    {
//CL188(2) -------- CL2018-1新模式 COM27  [成功]
//CL188(1) -------- CL2018-1新模式 COM27  [成功]
//CL191 ----------- CL2018-1新模式 COM13  [成功]
//CL311 ----------- CL2018-1新模式 COM32  [成功]
//CL303 ----------- CL2018-1新模式 COM14  [成功]

        private CMainControl m_ccl_CLMainControl;
        private pwComPorts.CCL20181 ccl_CL20181M;

        private CCL109 m_ccl_CL303;
        private pwComPorts.CCL20181 ccl_CL20181;
        public frmCL303()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 18;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 28;
        }

        private void frmCL303_Load(object sender, EventArgs e)
        {
            //this.m_ccl_CLMainControl = new CMainControl();
            //ccl_CL20181M = new pwComPorts.CCL20181();
            //ccl_CL20181M.PortOpen("29,193.168.18.1:10003:20000");
            //ccl_CL20181M.Setting = "9600,n,8,1";
            ////m_ccl_CLMainControl.OnEventTxFrame += new pwInterface.Dge_EventTxFrame(OnEventCMultiControllerBwTxFrame);
            ////m_ccl_CLMainControl.OnEventRxFrame += new pwInterface.Dge_EventRxFrame(OnEventCMultiControllerBwRxFrame);
            ////m_ccl_CLMainControl.OnEventMainControl += new pwInterface.DelegateEventMainControl(OnEventMainControler);
            //this.m_ccl_CLMainControl.ComPort = ccl_CL20181M;

            //==============
            SelectPort(comboBox1.SelectedIndex + 1);

            SelectPort2(comboBox3.SelectedIndex + 1);

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
            m_ccl_CL303 = null;
            ccl_CL20181 = null;

            this.m_ccl_CL303 = new CCL109();
            ccl_CL20181 = new pwComPorts.CCL20181();
            ccl_CL20181.PortOpen(intPort.ToString() + ",193.168.18.1:10003:20000");//33,19,20
            ccl_CL20181.Setting = "38400,n,8,1";
            this.m_ccl_CL303.ComPort = ccl_CL20181;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectPort2(comboBox3.SelectedIndex + 1);
        }

        private void SelectPort2(int intPort)
        {
            if (this.ccl_CL20181M != null)
            {
                this.ccl_CL20181M.PortClose();
            }
            m_ccl_CLMainControl = null;
            ccl_CL20181M = null;


            this.m_ccl_CLMainControl = new CMainControl();
            ccl_CL20181M = new pwComPorts.CCL20181();
            ccl_CL20181M.PortOpen(intPort.ToString() + ",193.168.18.1:10003:20000");
            ccl_CL20181M.Setting = "9600,n,8,1";
            //m_ccl_CLMainControl.OnEventTxFrame += new pwInterface.Dge_EventTxFrame(OnEventCMultiControllerBwTxFrame);
            //m_ccl_CLMainControl.OnEventRxFrame += new pwInterface.Dge_EventRxFrame(OnEventCMultiControllerBwRxFrame);
            //m_ccl_CLMainControl.OnEventMainControl += new pwInterface.DelegateEventMainControl(OnEventMainControler);
            this.m_ccl_CLMainControl.ComPort = ccl_CL20181M;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool bln_Result = this.m_ccl_CL303.link();
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL303.LostMessage);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            string s = "";
            bool bln_Result = this.m_ccl_CL303.ReadVer(ref s);
            label1.Text = bln_Result.ToString() + " " + s + " " + (bln_Result ? "" : this.m_ccl_CL303.LostMessage);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool bln_Result = this.m_ccl_CL303.PowerOff();
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL303.LostMessage);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            bool bln_Result = false;
            label1.Text = "";
            //bool bln_Result = this.m_ccl_CL303.PowerOn(220, 220, 220, 5, 5, 5, 0, 240, 120, 300, 180, 60, 50, 0, 63);
            //bool bln_Result = this.m_ccl_CL303.PowerOn(220, 10f, 300f, 50, 0, 0x3f);
            Single xIb = Convert.ToSingle(textBox1.Text);
            string strGlys = comboBox2.Text;

            if (checkBox1.Checked==true)
            {
                if (xIb > 2f)//大电流档位
                {
                    label1.Text = "";
                    string s = "";
                    if (radioButton1.Checked)
                    {
                        bln_Result = this.m_ccl_CLMainControl.AdjustCmdA(0x00);
                    }
                    else
                    {
                        bln_Result = this.m_ccl_CLMainControl.AdjustCmdB(0x00);
                    }
                    label1.Text = bln_Result.ToString() + " " + s + " " + (bln_Result ? "" : this.m_ccl_CLMainControl.LostMessage);

                }
                else//小电流档位
                {
                    label1.Text = "";
                    string s = "";
                    if (radioButton1.Checked)
                    {
                        bln_Result = this.m_ccl_CLMainControl.AdjustCmdA(0x01);
                    }
                    else
                    {
                        bln_Result = this.m_ccl_CLMainControl.AdjustCmdB(0x01);
                    }
                    label1.Text = bln_Result.ToString() + " " + s + " " + (bln_Result ? "" : this.m_ccl_CLMainControl.LostMessage);

                }

                if (bln_Result == false)
                {
                    label1.Text = "继电器档们切换不成功";

                    return;

                }
            }

            bln_Result = this.m_ccl_CL303.PowerOn(220, xIb, strGlys, 50, 0, pwInterface.enmElement.H);

            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL303.LostMessage);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool bln_Result = this.m_ccl_CL303.PowerOn(220, 220, 220, 0f, 0f, 0f, 0, 120, 240, 0, 120, 240, 50, 0, 63);
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CL303.LostMessage);
        }

        private void frmCL303_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.ccl_CL20181.PortClose();

           
        }



      
    }
}