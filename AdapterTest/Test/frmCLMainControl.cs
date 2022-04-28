using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using pwErrorCalculate;
using pwComPorts;
using pwMainControl;
using System.IO;
using System.Threading;
namespace ClAmMeterController.Test
{
    public partial class frmCLMainControl : Form
    {
//CL188(2) -------- CL2018-1新模式 COM27  [成功]
//CL188(1) -------- CL2018-1新模式 COM27  [成功]
//CL191 ----------- CL2018-1新模式 COM13  [成功]
//CL311 ----------- CL2018-1新模式 COM32  [成功]
//CL303 ----------- CL2018-1新模式 COM14  [成功]
        private CMainControl m_ccl_CLMainControl;
        private pwComPorts.CCL20181 ccl_CL20181;
        //private pwComPorts.CSerialCom ccl_CL20181;

        public frmCLMainControl()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;
            comboBox6.SelectedIndex = 0;
            comboBox7.SelectedIndex = 28;
        }

        private void frmCL303_Load(object sender, EventArgs e)
        {
            //this.m_ccl_CLMainControl = new CMainControl();
            //ccl_CL20181 = new pwComPorts.CCL20181();
            //ccl_CL20181.PortOpen("29,193.168.18.1:10003:20000");
            //ccl_CL20181.Setting = "9600,n,8,1";
            //m_ccl_CLMainControl.OnEventTxFrame += new pwInterface.Dge_EventTxFrame(OnEventCMultiControllerBwTxFrame);
            //m_ccl_CLMainControl.OnEventRxFrame += new pwInterface.Dge_EventRxFrame(OnEventCMultiControllerBwRxFrame);
            //m_ccl_CLMainControl.OnEventMainControl += new pwInterface.DelegateEventMainControl(OnEventMainControler);
            //this.m_ccl_CLMainControl.ComPort = ccl_CL20181;
            SelectPort(comboBox7.SelectedIndex + 1);
           
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectPort(comboBox7.SelectedIndex + 1);
        }


        private void SelectPort(int intPort)
        {
            if (this.ccl_CL20181 != null)
            {
                this.ccl_CL20181.PortClose();
            }
            m_ccl_CLMainControl = null;
            ccl_CL20181 = null;

            this.m_ccl_CLMainControl = new CMainControl();
            ccl_CL20181 = new pwComPorts.CCL20181();
            ccl_CL20181.PortOpen(intPort.ToString() + ",193.168.18.1:10003:20000");
            ccl_CL20181.Setting = "9600,n,8,1";
            m_ccl_CLMainControl.OnEventTxFrame += new pwInterface.Dge_EventTxFrame(OnEventCMultiControllerBwTxFrame);
            m_ccl_CLMainControl.OnEventRxFrame += new pwInterface.Dge_EventRxFrame(OnEventCMultiControllerBwRxFrame);
            m_ccl_CLMainControl.OnEventMainControl += new pwInterface.DelegateEventMainControl(OnEventMainControler);
            this.m_ccl_CLMainControl.ComPort = ccl_CL20181;
        }



        public void OnEventMainControler(int Typ_Cmd)
        {
            textBox1.Text = textBox1.Text + Typ_Cmd.ToString("X2");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool bln_Result = this.m_ccl_CLMainControl.SelectJDQ(comboBox6.SelectedIndex);
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CLMainControl.LostMessage);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            string s = "";
            bool bln_Result = this.m_ccl_CLMainControl.AdjustCmdA(comboBox1.SelectedIndex);
            label1.Text = bln_Result.ToString() + " " + s + " " + (bln_Result ? "" : this.m_ccl_CLMainControl.LostMessage);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool bln_Result = this.m_ccl_CLMainControl.SetGzdengA(comboBox2.SelectedIndex);
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CLMainControl.LostMessage);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool bln_Result = this.m_ccl_CLMainControl.SetTestStatusCmd(comboBox3.SelectedIndex);
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CLMainControl.LostMessage);
        }

        private void frmCL303_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.ccl_CL20181.PortClose();

           
        }

        /// <summary>
        /// 数据发送事件
        /// </summary>
        /// <param name="str_Frame"></param>
        public  void OnEventCMultiControllerBwTxFrame(int intCom, string str_Frame)
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
        public  void OnEventCMultiControllerBwRxFrame(int intCom, string str_Frame)
        {
            //写文件处理
            object obj = intCom.ToString() + "|" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ":"
                        + DateTime.Now.Millisecond.ToString("D3") + "<-" + str_Frame;
            ThreadPool.QueueUserWorkItem(new WaitCallback(SaveCommunicationData), obj);

        }

        private  object Locked = new object();
        private  string m_DelDirectory = Application.StartupPath      //写文件目录(数据帧)
            + "\\Comm\\"
            + DateTime.Now.ToShortDateString();

        /// <summary>
        /// 保存数据帧到文件
        /// </summary>
        /// <param name="obj"></param>
        /// 
        private  void SaveCommunicationData(object obj)
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

        private void button6_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            string s = "";
            bool bln_Result = this.m_ccl_CLMainControl.AdjustCmdB(comboBox5.SelectedIndex);
            label1.Text = bln_Result.ToString() + " " + s + " " + (bln_Result ? "" : this.m_ccl_CLMainControl.LostMessage);

        }

        private void button5_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            bool bln_Result = this.m_ccl_CLMainControl.SetGzdengB(Convert.ToInt32(comboBox4.Text.Substring(0,2),16));
            label1.Text = bln_Result.ToString() + " " + (bln_Result ? "" : this.m_ccl_CLMainControl.LostMessage);

        }


    }
}