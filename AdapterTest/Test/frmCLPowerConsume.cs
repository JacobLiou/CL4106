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
    public partial class frmCLPowerConsume : Form
    {
        //    功耗板RxID为：1,2,3,4,5,6
        private PowerConsume m_ccl_dlt6451997;
        //private pwMeterProtocol.DLT645_1997 m_ccl_dlt6451997;
        //private pwMeterProtocol.DLT645_2007 m_ccl_dlt6451997;
        pwComPorts.CCL20181 ccl_CL20181;

        private string m_xyname = "";

        public frmCLPowerConsume()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        private void frmCL485_Load(object sender, EventArgs e)
        {
            //m_ccl_dlt6451997 = new PowerConsume();

            //ccl_CL20181 = new CCL20181();
            //ccl_CL20181.PortOpen("9,193.168.18.1:10003:20000");
            //ccl_CL20181.Setting = "9600,n,8,1";

            //m_ccl_dlt6451997.ComPort = ccl_CL20181;
            //m_ccl_dlt6451997.Setting = ccl_CL20181.Setting;
            //m_ccl_dlt6451997.VerifyPasswordType = 1;
            //m_ccl_dlt6451997.PasswordClass = 0;
            //m_ccl_dlt6451997.Password = "000000";
            //m_ccl_dlt6451997.WaitDataRevTime = 1000;
            //m_ccl_dlt6451997.IntervalTime = 500;
        }
        private void SelectProtocol(int Index)
        {
            if (this.ccl_CL20181 != null) this.ccl_CL20181.PortClose();

            m_ccl_dlt6451997 = new PowerConsume();

            ccl_CL20181 = new CCL20181();
            ccl_CL20181.PortOpen(Convert.ToInt32( Index+8).ToString()+ ",193.168.18.1:10003:20000");
            ccl_CL20181.Setting = "9600,n,8,1";
            m_ccl_dlt6451997.ComPort = ccl_CL20181;
            m_ccl_dlt6451997.Setting = ccl_CL20181.Setting;

            m_ccl_dlt6451997.VerifyPasswordType = 1;
            m_ccl_dlt6451997.PasswordClass = 0;
            m_ccl_dlt6451997.Password = "000000";
            m_ccl_dlt6451997.WaitDataRevTime = 2000;
            m_ccl_dlt6451997.IntervalTime = 500;
            m_ccl_dlt6451997.OnEventTxFrame += new pwInterface.Dge_EventTxFrame(OnEventCMultiControllerBwTxFrame);
            m_ccl_dlt6451997.OnEventRxFrame += new pwInterface.Dge_EventRxFrame(OnEventCMultiControllerBwRxFrame);

        }

        private void frmCL485_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(this.ccl_CL20181!=null ) this.ccl_CL20181.PortClose();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int Index=comboBox1.SelectedIndex+1;
            SelectProtocol( Index);
            float sngP=0f;
            m_ccl_dlt6451997.RxID = Index.ToString("X2");
            bool bln_Result = m_ccl_dlt6451997.ReadPower( ref sngP);
            if (bln_Result)
            {
                textBox1.Text = sngP.ToString();
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
    }
}
