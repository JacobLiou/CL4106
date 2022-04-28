using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using pwMeterProtocol;
using pwComPorts;
using pwInterface;
using System.IO;
using System.Threading;
namespace ClAmMeterController.Test
{
    public partial class frmCL485 : Form
    {
        private IMeterProtocol m_ccl_dlt6451997;
        //private pwMeterProtocol.DLT645_1997 m_ccl_dlt6451997;
        //private pwMeterProtocol.DLT645_2007 m_ccl_dlt6451997;
        pwComPorts.CCL20181 ccl_CL20181;

        private string m_xyname = "";

        public frmCL485()
        {
            InitializeComponent();
        }

        private void frmCL485_Load(object sender, EventArgs e)
        {
            //    m_ccl_dlt6451997 = new DLT645_1997();

            //    ccl_CL20181 = new CCL20181();
            //    ccl_CL20181.PortOpen("1,193.168.18.1:10003:20000");
            //    ccl_CL20181.Setting = "2400,E,8,1";

            //    m_ccl_dlt6451997.ComPort = ccl_CL20181;
            //    m_ccl_dlt6451997.Setting = ccl_CL20181.Setting;
            //    m_ccl_dlt6451997.VerifyPasswordType = 1;
            //    m_ccl_dlt6451997.PasswordClass = 0;
            //    m_ccl_dlt6451997.Password = "000000";
            //    m_ccl_dlt6451997.WaitDataRevTime = 1000;
            //    m_ccl_dlt6451997.IntervalTime = 500;
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            SelectProtocol("DLT645_1997");
        }
        private void SelectProtocol(string xyname)
        {
            //if (m_xyname == xyname) return;
            int intStartPort = 1;
            string _setting= "2400,E,8,1";
            bool _bClouHw = false;
            if (comboBox2.SelectedIndex == 0)
            {
                intStartPort = 1;
                _setting = "2400,E,8,1";
                _bClouHw = false;
            }
            else
            {
                intStartPort = 23;
                _setting = "1200,E,8,1";
                _bClouHw = true;
            }

            int Index = comboBox1.SelectedIndex + intStartPort;

            if (this.ccl_CL20181 != null)
            {
                this.ccl_CL20181.PortClose();
                m_ccl_dlt6451997.OnEventTxFrame -= new pwInterface.Dge_EventTxFrame(OnEventCMultiControllerBwTxFrame);
                m_ccl_dlt6451997.OnEventRxFrame -= new pwInterface.Dge_EventRxFrame(OnEventCMultiControllerBwRxFrame);
            }
            m_ccl_dlt6451997 = null;
            ccl_CL20181 = null;

            if (xyname == "DLT645_1997")
            {
                m_xyname = "DLT645_1997";
                m_ccl_dlt6451997 = new DLT645_1997();

                ccl_CL20181 = new CCL20181();
                ccl_CL20181.PortOpen(Index.ToString()+",193.168.18.1:10003:20000");
                ccl_CL20181.Setting = _setting;// "2400,E,8,1";
                m_ccl_dlt6451997.ComPort = ccl_CL20181;
                m_ccl_dlt6451997.Setting = ccl_CL20181.Setting;

                m_ccl_dlt6451997.VerifyPasswordType = 1;
                m_ccl_dlt6451997.PasswordClass = 0;
                m_ccl_dlt6451997.Password = "000000";
                m_ccl_dlt6451997.WaitDataRevTime = 1000;
                m_ccl_dlt6451997.IntervalTime = 500;
                m_ccl_dlt6451997.ClouHw = _bClouHw;
                m_ccl_dlt6451997.OnEventTxFrame += new pwInterface.Dge_EventTxFrame(OnEventCMultiControllerBwTxFrame);
                m_ccl_dlt6451997.OnEventRxFrame += new pwInterface.Dge_EventRxFrame(OnEventCMultiControllerBwRxFrame);

            }
            else if (xyname == "DLT645_2007")
            {
                m_xyname = "DLT645_2007";

                m_ccl_dlt6451997 = new DLT645_2007();

                ccl_CL20181 = new CCL20181();
                ccl_CL20181.PortOpen(Index.ToString() + ",193.168.18.1:10003:20000");
                ccl_CL20181.Setting = _setting;// "2400,E,8,1";
                m_ccl_dlt6451997.ComPort = ccl_CL20181;
                m_ccl_dlt6451997.Setting = ccl_CL20181.Setting;

                m_ccl_dlt6451997.VerifyPasswordType = 1;
                m_ccl_dlt6451997.PasswordClass = 0;
                m_ccl_dlt6451997.Password = "000000";
                m_ccl_dlt6451997.WaitDataRevTime = 1000;
                m_ccl_dlt6451997.IntervalTime = 500;
                m_ccl_dlt6451997.ClouHw = _bClouHw;
                m_ccl_dlt6451997.OnEventTxFrame += new pwInterface.Dge_EventTxFrame(OnEventCMultiControllerBwTxFrame);
                m_ccl_dlt6451997.OnEventRxFrame += new pwInterface.Dge_EventRxFrame(OnEventCMultiControllerBwRxFrame);

            }

        }

        private void frmCL485_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(this.ccl_CL20181!=null ) this.ccl_CL20181.PortClose();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelectProtocol("DLT645_2007");
            string sdata = "";
            m_ccl_dlt6451997.Address = "AAAAAAAAAAAA";
            bool bln_Result = m_ccl_dlt6451997.ReadData("04000401", 0, ref sdata);
            if (bln_Result)
            {
                textBox1.Text = m_ccl_dlt6451997.BackString(sdata);
                m_ccl_dlt6451997.Address = textBox1.Text;
            }
            label1.Text = bln_Result.ToString() + " " + this.m_ccl_dlt6451997.LostMessage;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SelectProtocol("DLT645_1997");
            string sdata = "";
            m_ccl_dlt6451997.Address = "AAAAAAAAAAAA";
            bool bln_Result = m_ccl_dlt6451997.ReadData("FFF9", 0, ref sdata);
            if (bln_Result)
            {
                textBox2.Text = sdata;// m_ccl_dlt6451997.BackString(sdata); ;
            }
            label1.Text = bln_Result.ToString() + " " + this.m_ccl_dlt6451997.LostMessage;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SelectProtocol("DLT645_2007");
            string sdata = "";
            bool bln_Result = m_ccl_dlt6451997.SysClear();
            label1.Text = bln_Result.ToString() + " " + this.m_ccl_dlt6451997.LostMessage;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SelectProtocol("DLT645_2007");
            string sdata = "";
            bool bln_Result = m_ccl_dlt6451997.WriteData("04CC0509", 1, "8");//放大常数5倍
            label1.Text = bln_Result.ToString() + " " + this.m_ccl_dlt6451997.LostMessage;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string sdata = "";
            string cdata = "";
            bool bln_Result = m_ccl_dlt6451997.SendDLT645RxFrame(textBox3.Text, ref sdata, ref cdata);
            label1.Text = bln_Result.ToString() + " " + this.m_ccl_dlt6451997.LostMessage;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SelectProtocol("DLT645_2007");
            string sdata = "";
            m_ccl_dlt6451997.Address = "AAAAAAAAAAAA";
            bool bln_Result = m_ccl_dlt6451997.ReadData("04CC0509", 1, ref sdata);
            if (bln_Result)
            {
                textBox4.Text = sdata;
            }
            label1.Text = bln_Result.ToString() + " " + this.m_ccl_dlt6451997.LostMessage;

        }

        private void button7_Click(object sender, EventArgs e)
        {
            SelectProtocol("DLT645_2007");
            string sdata = "";
            m_ccl_dlt6451997.Address = "AAAAAAAAAAAA";
            bool bln_Result = m_ccl_dlt6451997.ReadVer( ref sdata);
            if (bln_Result)
            {
                textBox5.Text = sdata;
            }
            label1.Text = bln_Result.ToString() + " " + this.m_ccl_dlt6451997.LostMessage;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SelectProtocol("DLT645_1997");
            string sdata = "";
            m_ccl_dlt6451997.Address = "AAAAAAAAAAAA";
            m_ccl_dlt6451997.WaitDataRevTime = 10000;
            bool bln_Result = m_ccl_dlt6451997.SelfCheck(0x01, 0x00, ref sdata);
            if (bln_Result)
            {
                textBox6.Text = sdata;
            }

            label1.Text = bln_Result.ToString() + " " + this.m_ccl_dlt6451997.LostMessage;
        }
        /// <summary>
        /// 数据发送事件
        /// </summary>
        /// <param name="str_Frame"></param>
        public void OnEventCMultiControllerBwTxFrame(int intCom, string str_Frame)
        {
            //写文件处理
            object obj = intCom.ToString() + "|" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ":"
                        + DateTime.Now.Millisecond.ToString("D3") + "->" + str_Frame;
            ThreadPool.QueueUserWorkItem(new WaitCallback(SaveCommunicationData), obj);


        }

        /// <summary>
        /// 数据接收事件
        /// </summary>
        /// <param name="str_Frame"></param>
        public void OnEventCMultiControllerBwRxFrame(int intCom, string str_Frame)
        {
            //写文件处理
            object obj = intCom.ToString() + "|" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ":"
                        + DateTime.Now.Millisecond.ToString("D3") + "<-" + str_Frame;
            ThreadPool.QueueUserWorkItem(new WaitCallback(SaveCommunicationData), obj);

        }

        private object Locked = new object();
        private string m_DelDirectory = Application.StartupPath      //写文件目录(数据帧)
            + "\\Comm\\"
            + DateTime.Now.ToShortDateString();

        /// <summary>
        /// 保存数据帧到文件
        /// </summary>
        /// <param name="obj"></param>
        /// 
        private void SaveCommunicationData(object obj)
        {
            try
            {
                Control.CheckForIllegalCrossThreadCalls = false;

                //lock (Locked)
                //{
                //    string[] objArray = obj.ToString().Split('|');
                //    int intCom = Convert.ToInt32(objArray[0].ToString());
                //    string str_Frame = objArray[1].ToString();

                //    #region 写入文件
                //    string sfilename = m_DelDirectory + "\\" + intCom.ToString("D2") + "_CommunicationData.txt";
                //    StreamWriter sw = new StreamWriter(@sfilename, true, System.Text.Encoding.Unicode);
                //    sw.WriteLine(str_Frame);
                //    sw.Close();
                //    #endregion
                //}
                lock (Locked)
                {
                    string[] objArray = obj.ToString().Split('|');
                    int intCom = Convert.ToInt32(objArray[0].ToString());
                    string str_Frame = objArray[1].ToString();

                    int intCount = listView1.Items.Count;
                    ListViewItem lv = new ListViewItem(Convert.ToString(intCount + 1));
                    lv.SubItems.Add(DateTime.Now.ToLongTimeString());
                    lv.SubItems.Add(str_Frame);
                    listView1.Items.Add(lv);
                    listView1.Items[intCount].EnsureVisible();
                    listView1.Refresh();
                }


            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
