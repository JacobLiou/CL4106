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
    public partial class frmCL485Adjust : Form
    {
        private IMeterProtocol m_ccl_dlt6451997;
        //private pwMeterProtocol.DLT645_1997 m_ccl_dlt6451997;
        //private pwMeterProtocol.DLT645_2007 m_ccl_dlt6451997;
        pwComPorts.CCL20181 ccl_CL20181;

        private string m_xyname = "";

        public frmCL485Adjust()
        {
            InitializeComponent();
            InitRowdataGridView1();
        }
        private void InitRowdataGridView1()
        {
            object[][] sNameText ={
                new object[]  { "1：有功脉冲常数（4bytes，HEX）", "1600" },
                new object[]  { "2：校准步骤标识（2bytes，HEX）", "0000" },
                new object[]  { "3：HFConst：（4bytes，HEX）", "00000000" },
                new object[]  { "4：基本电压V：（2bytes，HEX格式：xxx.xV含1位小数）", "220" },
                new object[]  { "5：基本电流V：（2bytes，HEX，格式：x.xxxA含3位小数）", "10" },
                new object[]  { "6：标准源电压V：（4bytes，HEX格式：xx.xxxV含3位小数）","220"},
                new object[]  { "7：标准源1路电流A：（4bytes，HEX，格式：x.xxxxA含4位小数）","10"},
                new object[]  { "7：标准源2路电流A：（4bytes，HEX，格式：x.xxxxA含4位小数）","0"},
                new object[]  { "8：标准源1路功率kW：（4bytes，HEX，格式：x.xxxxxA含5位小数）","1.1"},
                new object[]  { "10：标准源2路功率kW：（4bytes，HEX，格式：x.xxxxxA含5位小数）","0"},
                new object[]  { "11：电流1偏移；（4bytes，HEX）", "0"},
                new object[]  { "12：电流2偏移；（4bytes，HEX）", "0" },
                new object[]  { "13：功率1偏移；（4bytes，HEX）", "0" },
                new object[]  { "14：功率2偏移；（4bytes，HEX）", "0" },
                new object[]  { "15：相位1补偿；（4byte，HEX）", "0" },
                new object[]  { "16：相位2补偿；（4byte，HEX）", "0" },
                new object[]  { "17：回路1小电流误差，第5步时下发使用；（补码4bytes，HEX，格式：xx.xxxxxx 含6位小数）", "0" },
                new object[]  { "18：回路2小电流误差，第3步时下发使用；（补码4bytes，HEX，格式：xx.xxxxxx 含6位小数）", "0" },
             };
            DataGridViewRowCollection rows1 = this.dataGridView1.Rows;

            for (int i = 0; i < sNameText.Length; i++)
            {
                rows1.Add(sNameText[i]);
            }
        }
        private string setSendData()
        {
            string Tstr = "";
            string s_Data = "";
            string sDgvValue = "";

            dataGridView1.EndEdit();


            Tstr = "";
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                sDgvValue = dataGridView1.Rows[i].Cells[1].Value.ToString();
                switch (i)
                {

                    case 0: //有功脉冲常数
                        s_Data = Convert.ToInt32(sDgvValue).ToString("X8");
                        break;
                    case 1: //无功脉冲常数
                        s_Data = sDgvValue.PadLeft(4,'0');
                        break;
                    case 2: //HFConst
                        s_Data = Convert.ToInt32(sDgvValue).ToString("X8");
                        break;
                    case 3: //基本电压
                        s_Data = Convert.ToInt32(Single.Parse(sDgvValue) * Math.Pow(10, 1)).ToString("X4");
                        break;
                    case 4: //基本电流
                        s_Data = Convert.ToInt32(Single.Parse(sDgvValue) * Math.Pow(10, 3)).ToString("X4");
                        break;
                    case 5: //标准源电压
                        s_Data = Convert.ToInt32(Single.Parse(sDgvValue) * Math.Pow(10, 3)).ToString("X8");
                        break;
                    case 6: //标准源1路电流
                    case 7: //标准源2路电流
                        s_Data = Convert.ToInt32(Single.Parse(sDgvValue) * Math.Pow(10, 4)).ToString("X8");
                        break;
                    case 8: //标准源1路功率
                    case 9: //标准源2路功率
                        s_Data = Convert.ToInt32(Single.Parse(sDgvValue) * Math.Pow(10, 2)).ToString("X8");
                        break;
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                        s_Data = sDgvValue.PadLeft(8, '0');
                        break;
                    default:
                        s_Data = sDgvValue.PadLeft(8, '0');
                        break;
                }


                Tstr = s_Data + Tstr;
            }
            return Tstr;

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

        private void button1_Click(object sender, EventArgs e)
        {
            SelectProtocol("DLT645_1997");
            string sdata = "";
            m_ccl_dlt6451997.Address = "AAAAAAAAAAAA";
            //int int_Step = 0;
            //int[] intStepWaitDataRevTime = new int[] { 10, 35, 10, 35, 10, 35, 10, };//每步校准等待时间
            m_ccl_dlt6451997.WaitDataRevTime = 35000;// intStepWaitDataRevTime[int_Step] * 1000;
            sdata = setSendData();
            bool bln_Result = m_ccl_dlt6451997.WriteData("FFF0", sdata.Length / 2, sdata);
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


    }
}
