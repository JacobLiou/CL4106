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
using pwClassLibrary;
namespace ClAmMeterController.Test
{
    public partial class frmCL485_WriteSCBH : Form
    {
        private IMeterProtocol m_ccl_dlt6451997;
        //private pwMeterProtocol.DLT645_1997 m_ccl_dlt6451997;
        //private pwMeterProtocol.DLT645_2007 m_ccl_dlt6451997;
        pwComPorts.CSerialCom ccl_CL20181;

        private string m_xyname = "";

        public frmCL485_WriteSCBH()
        {
            InitializeComponent();
        }

        private void frmCL485_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            SelectProtocol("DLT645_1997");
        }

        private void SelectProtocol(string xyname)
        {
            //if (m_xyname == xyname) return;
            int intStartPort = 1;
            string _setting = comboBox3.Text;// SelectedText;// "9600,E,8,1";
            bool _bClouHw = false;
            if (comboBox2.SelectedIndex == 0)
            {
                intStartPort = 1;
                _bClouHw = false;
            }
            else
            {
                intStartPort = 23;
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

                ccl_CL20181 = new CSerialCom();
                ccl_CL20181.PortOpen(Index.ToString()+",193.168.18.1:10003:20000");
                ccl_CL20181.Setting = _setting;// "2400,E,8,1";
                m_ccl_dlt6451997.ComPort = ccl_CL20181;
                m_ccl_dlt6451997.Setting = ccl_CL20181.Setting;

                m_ccl_dlt6451997.ZendStringDel0x33 = true;
                m_ccl_dlt6451997.iRepeatTimes = 1;
                m_ccl_dlt6451997.VerifyPasswordType = 1;
                m_ccl_dlt6451997.PasswordClass = 0;
                m_ccl_dlt6451997.Password = "5A7B3D";
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

                ccl_CL20181 = new CSerialCom();
                ccl_CL20181.PortOpen(Index.ToString() + ",193.168.18.1:10003:20000");
                ccl_CL20181.Setting = _setting;// "2400,E,8,1";
                m_ccl_dlt6451997.ComPort = ccl_CL20181;
                m_ccl_dlt6451997.Setting = ccl_CL20181.Setting;

                m_ccl_dlt6451997.VerifyPasswordType = 1;
                m_ccl_dlt6451997.PasswordClass = 2;
                m_ccl_dlt6451997.Password = "000000";
                m_ccl_dlt6451997.WaitDataRevTime = 1000;
                m_ccl_dlt6451997.IntervalTime = 500;
                m_ccl_dlt6451997.ClouHw = _bClouHw;
                m_ccl_dlt6451997.OnEventTxFrame += new pwInterface.Dge_EventTxFrame(OnEventCMultiControllerBwTxFrame);
                m_ccl_dlt6451997.OnEventRxFrame += new pwInterface.Dge_EventRxFrame(OnEventCMultiControllerBwRxFrame);

            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectProtocol("DLT645_1997");
        }

        private void frmCL485_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(this.ccl_CL20181!=null ) this.ccl_CL20181.PortClose();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //SelectProtocol("DLT645_2007");
            //string sdata = "";
            //m_ccl_dlt6451997.Address = "AAAAAAAAAAAA";
            //bool bln_Result = m_ccl_dlt6451997.ReadData("04000401", 0, ref sdata);
            //if (bln_Result)
            //{
            //    textBox1.Text = m_ccl_dlt6451997.BackString(sdata);
            //    m_ccl_dlt6451997.Address = textBox1.Text;
            //}
            //label1.Text = bln_Result.ToString() + " " + this.m_ccl_dlt6451997.LostMessage;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ReadSCBH();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            WriteSCBH();
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


        private bool ReadSCBH()
        {
            try
            {
                SelectProtocol("DLT645_1997");
                textBox3.Text = "";
                string sdata = "";
                m_ccl_dlt6451997.Address = "AAAAAAAAAAAA";
                bool bln_Result = m_ccl_dlt6451997.ReadData("FFF9", 0, ref sdata);
                if (bln_Result)
                {
                    sdata = Sstring.BackString(sdata);
                    sdata = Sstring.ASCIIEncodingToString (sdata);
                    textBox3.Text = sdata;// m_ccl_dlt6451997.BackString(sdata); 
                    textBox3.BackColor = Color.Blue;
                }
                else
                {
                    textBox3.BackColor = Color.Red;
                }

                label1.Text = bln_Result.ToString() + " " + this.m_ccl_dlt6451997.LostMessage;
                return bln_Result;

            }
            catch (Exception erroreeoeee )
            {
                label1.Text = erroreeoeee.Message;
                return false;

            }
            finally
            {
                textBox2.Focus();
                textBox2.SelectAll();

            }
        }

        private bool WriteSCBH()
        {
            try
            {
                SelectProtocol("DLT645_1997");
                string sdata = "";
                m_ccl_dlt6451997.Address = "AAAAAAAAAAAA";

                sdata = textBox2.Text.Trim();
                sdata = Sstring.BackString(Sstring.StringToASCIIEncoding(sdata));
                bool bln_Result = m_ccl_dlt6451997.WriteData("FFF9", 12, sdata);

                if (bln_Result)
                {
                    textBox2.BackColor = Color.Blue;
                }
                else
                {
                    textBox2.BackColor = Color.Red;
                }

                label1.Text = bln_Result.ToString() + " " + this.m_ccl_dlt6451997.LostMessage;
                return bln_Result;
            }
            catch (Exception erroreeoeee)
            {
                label1.Text = erroreeoeee.Message;
                return false;
            }
            finally
            {
                textBox2.Focus();
                textBox2.SelectAll();
            }
        }

        private bool WriteAddR()
        {
            try
            {
                SelectProtocol("DLT645_2007");
                string sdata = "020000000000";
                m_ccl_dlt6451997.Address = "AAAAAAAAAAAA";

                sdata = Sstring.BackString(sdata);
                bool bln_Result = m_ccl_dlt6451997.WriteMeterAddress(sdata);

                if (bln_Result)
                {
                    textBox2.BackColor = Color.Blue;
                }
                else
                {
                    textBox2.BackColor = Color.Red;
                }

                label1.Text = bln_Result.ToString() + " " + this.m_ccl_dlt6451997.LostMessage;
                return bln_Result;
            }
            catch (Exception erroreeoeee)
            {
                label1.Text = erroreeoeee.Message;
                return false;
            }
            finally
            {
                textBox2.Focus();
                textBox2.SelectAll();
            }
        }

        private bool WriteBTL()
        {
            try
            {
                SelectProtocol("DLT645_2007");
                int isdataBTL =83;
                string sdata = string.Format("{0:X2}", isdataBTL);

                m_ccl_dlt6451997.Address = "000000000002";

                sdata = Sstring.BackString(sdata);
                bool bln_Result = m_ccl_dlt6451997.WriteData(0x17,1, sdata);

                if (bln_Result)
                {
                    textBox2.BackColor = Color.Blue;
                }
                else
                {
                    textBox2.BackColor = Color.Red;
                }

                label1.Text = bln_Result.ToString() + " " + this.m_ccl_dlt6451997.LostMessage;
                return bln_Result;
            }
            catch (Exception erroreeoeee)
            {
                label1.Text = erroreeoeee.Message;
                return false;
            }
            finally
            {
                textBox2.Focus();
                textBox2.SelectAll();
            }
        }
        private bool WriteBTL2()
        {
            try
            {
                SelectProtocol("DLT645_2007");
                string sdata = "";
                m_ccl_dlt6451997.Address = "000000000002";

                int idata = (int)Math.Pow(2, 5);

                sdata = string.Format("{0:X2}", idata);

                bool bln_Result = m_ccl_dlt6451997.WriteData("04000703", 1, sdata);

                if (bln_Result)
                {
                    textBox2.BackColor = Color.Blue;
                }
                else
                {
                    textBox2.BackColor = Color.Red;
                }

                label1.Text = bln_Result.ToString() + " " + this.m_ccl_dlt6451997.LostMessage;
                return bln_Result;
            }
            catch (Exception erroreeoeee)
            {
                label1.Text = erroreeoeee.Message;
                return false;
            }
            finally
            {
                textBox2.Focus();
                textBox2.SelectAll();
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            bool bolIsOK=false ;
            if (e.KeyCode == Keys.Enter)      //如果输入的是回车键
            {
                SetColor("white");
                bolIsOK = WriteSCBH();
                if (bolIsOK) bolIsOK = ReadSCBH();
                if (bolIsOK) bolIsOK = WriteAddR();
                if (bolIsOK) bolIsOK = WriteBTL2();
                if (bolIsOK)
                {
                    if (textBox2.Text.Trim() == textBox3.Text.Trim())
                    {
                        SetColor("blue");
                        return;
                    }
                }
                SetColor("red");
            }
        }

        private void SetColor(string sColor)
        {
            try
            {
                switch (sColor)
                {
                    case "white":
                        pictureBox1.Image = System.Drawing.Image.FromFile(Application.StartupPath + @"\white.png");
                        break;
                    case "blue":
                        pictureBox1.Image = System.Drawing.Image.FromFile(Application.StartupPath + @"\blue.png");
                        break;
                    case "red":
                        pictureBox1.Image = System.Drawing.Image.FromFile(Application.StartupPath + @"\red.png");
                        break;
                    default:
                        pictureBox1.Image = System.Drawing.Image.FromFile(Application.StartupPath + @"\white.png");
                        break;
                }
            }
            catch
            {
                MessageBox.Show("图标文件不存在!");
            }

        }

    }
}
