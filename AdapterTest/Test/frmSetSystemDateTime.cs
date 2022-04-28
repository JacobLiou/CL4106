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
using System.Threading;
using System.Runtime.InteropServices; 
using Microsoft.Win32;
using System.IO;
using System.Net;
using System.Net.Sockets;
namespace ClAmMeterController.Test
{
    public partial class frmSetSystemDateTime : Form
    {
        private pwComPorts.CSerialCom ccl_CL20181;
        private CCL191 m_ccl_CL191;
        private string strConfigpath = Application.StartupPath;
        private string strConfigFile = "Config.cfg";
        private DateTime m_CurTime = new DateTime();                        //�ϴζ�ʱʱ��
        private Thread threadDo = null;                                     //�춨�߳�
        private bool m_Stop=false ;
        private Stopwatch sth_SpaceTicker = new Stopwatch();                //��ʱʱ��
        private int m_LoopTime = 30;                                        //���ʱ��
        private object obj =new object();

        private int m_Port = 8000;//�˿�
        UdpClient client;
        private Thread threadUdpServer=null ;

        public frmSetSystemDateTime()
        {
            InitializeComponent();
            string strTmp = "";

            strTmp=pwClassLibrary.pwFile.File.ReadInIString(strConfigFile, "System", "IsAutoDs",  "0");
            checkBox1.Checked = (strTmp == "0" ? false : true);

            strTmp = pwClassLibrary.pwFile.File.ReadInIString(strConfigFile, "System", "AutoDsSj", "1");
            textBox2.Text = strTmp;
            m_LoopTime = Convert.ToInt32(textBox2.Text);

            strTmp = pwClassLibrary.pwFile.File.ReadInIString(strConfigFile, "System", "IsAutoRun", "0");
            checkBox2.Checked = (strTmp == "0" ? false : true);


            strTmp = pwClassLibrary.pwFile.File.ReadInIString(strConfigFile, "System", "COM", "1");
            comboBox1.SelectedIndex = Convert.ToInt32(strTmp);

            strTmp = pwClassLibrary.pwFile.File.ReadInIString(strConfigFile, "System", "Port", "8000");
            textBox3.Text = strTmp;
            m_Port = Convert.ToInt32(textBox3.Text);


        }

        private void frmCL191_Load(object sender, EventArgs e)
        {
            SelectPort(comboBox1.SelectedIndex + 1);

            obj = "GPS��ʱ����������" + DateTime.Now.ToString() ;
            ThreadPool.QueueUserWorkItem(new WaitCallback(SaveRecorderData), obj);

            pwSetSystemTime();

            threadUdpServer = new Thread(new ThreadStart(InitUdpServer));
            threadUdpServer.Start();


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
            ccl_CL20181 = new pwComPorts.CSerialCom();
            ccl_CL20181.PortOpen(intPort.ToString() + ",193.168.18.1:10003:20000");
            ccl_CL20181.Setting = "2400,n,8,1";
            this.m_ccl_CL191.ComPort = ccl_CL20181;
            this.m_ccl_CL191.Setting = ccl_CL20181.Setting;
        }

        private void frmCL191_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                e.Cancel = true;
                m_Stop = true;
                client.Close();
                threadUdpServer.Abort();
                Thread.Sleep(100);
                this.ccl_CL20181.PortClose();
                pwClassLibrary.pwFile.File.WriteInIString(strConfigFile, "System", "IsAutoDs", checkBox1.Checked ? "1" : "0");
                pwClassLibrary.pwFile.File.WriteInIString(strConfigFile, "System", "AutoDsSj", textBox2.Text);
                pwClassLibrary.pwFile.File.WriteInIString(strConfigFile, "System", "IsAutoRun", checkBox2.Checked ? "1" : "0");
                pwClassLibrary.pwFile.File.WriteInIString(strConfigFile, "System", "COM", comboBox1.SelectedIndex.ToString());
                pwClassLibrary.pwFile.File.WriteInIString(strConfigFile, "System", "Port", textBox3.Text);
                obj = "GPS��ʱ�����˳���" + DateTime.Now.ToString();
                ThreadPool.QueueUserWorkItem(new WaitCallback(SaveRecorderData), obj);
            }
            catch
            {
            }
            finally 
            {
                e.Cancel = false   ;
            }

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
            label1.Text = bln_Result.ToString() + "����ϵͳʱ��" + (bln_Result ? "�ɹ�" : "ʧ��");

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkBox2.Checked) //���ÿ���������
                {
                    //MessageBox.Show ("���ÿ�����������Ҫ�޸�ע���","��ʾ");
                    string path = Application.ExecutablePath;
                    RegistryKey rk = Registry.LocalMachine;
                    RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                    rk2.SetValue("AdapterTest", path);
                    rk2.Close();
                    rk.Close();
                }
                else //ȡ������������
                {
                    //MessageBox.Show ("ȡ��������������Ҫ�޸�ע���","��ʾ");
                    string path = Application.ExecutablePath;
                    RegistryKey rk = Registry.LocalMachine;
                    RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                    rk2.DeleteValue("AdapterTest", false);
                    rk2.Close();
                    rk.Close();
                }
            }
            catch
            {
                return;
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = checkBox1.Checked;
            if (checkBox1.Checked)
            {
                m_Stop = false;
                start();
            }
            else
            {
                m_Stop = true;
                threadDo.Abort();
            }
        }

        private void start()
        {

            Control.CheckForIllegalCrossThreadCalls = false;
            threadDo = new Thread(new ThreadStart(pwExecute));
            threadDo.Start();
        }
        private void pwExecute()
        {
            sth_SpaceTicker.Reset();
            sth_SpaceTicker.Start();
            int _CurTime = 0;
            while (true)
            {
                Thread.Sleep(100);

                Application.DoEvents();

                if (m_Stop) break;//�ⲿ��ֹ�����˳�

                _CurTime = Convert.ToInt32(sth_SpaceTicker.ElapsedMilliseconds / (1000*60));

                if (_CurTime < m_LoopTime)
                    continue;// ��С�� �ȴ�Դ�ȶ���ʱ��
                else
                {
                    pwSetSystemTime();
                    sth_SpaceTicker.Reset();
                    sth_SpaceTicker.Start();
                }

            }
        }

        private void pwSetSystemTime()
        {
            bool bln_Result = false;
            string strtime = "";
            DateTime _CurTime = DateTime.Now;
            object _info = "����ʱ��";
            try
            {
                bln_Result = this.m_ccl_CL191.Link();
                if (bln_Result)
                {
                    _CurTime = DateTime.Now;
                    bln_Result = this.m_ccl_CL191.ReadGPSTime(ref strtime);

                    if (bln_Result)
                    {
                        obj = "��GPSʱ��ɹ���"+strtime.ToString();
                        ThreadPool.QueueUserWorkItem(new WaitCallback(ViewRecorderData), obj);

                        bln_Result = pwClassLibrary.pwWin32Api.Win32Api.SetSystemDateTime(strtime);

                        if (bln_Result)
                        {
                            _info = "����ϵͳʱ�䣺" + _CurTime.ToString() + "GPSʱ�䣺" + strtime + "��ִ��GPS��ʱ�ɹ�!";

                            obj = _info;
                            ThreadPool.QueueUserWorkItem(new WaitCallback(SaveRecorderData), obj);
                            ThreadPool.QueueUserWorkItem(new WaitCallback(ViewRecorderData), obj);
                            //listBox1.Items.Add(_info);
                            return ;
                        }
                    }
                }
                _info = "����ϵͳʱ�䣺" + _CurTime.ToString() + "��ִ��GPS��ʱʧ��!";
                obj = _info;
                ThreadPool.QueueUserWorkItem(new WaitCallback(SaveRecorderData), obj);
                ThreadPool.QueueUserWorkItem(new WaitCallback(ViewRecorderData), obj);
                return;
            }
            catch (System.Exception err)
            {
                return;
            }
            finally
            {

            }
        }
        private static object Locked2 = new object();
        private void ViewRecorderData(object obj)
        {
            try
            {
                lock (Locked2)
                {
                    string str_Frame = obj.ToString();
                    listBox1.Items.Add(str_Frame);
                }
            }
            catch (Exception e)
            {
                return;
            }
        }

        private static object Locked = new object();

        private void SaveRecorderData(object obj)
        {
            try
            {
                lock (Locked)
                {
                    string str_Frame = obj.ToString();
                    #region д���ļ�
                    string sfilename = strConfigpath + "\\" + "RecorderData.txt";
                    StreamWriter sw = new StreamWriter(@sfilename, true, System.Text.Encoding.Unicode);
                    sw.WriteLine(str_Frame);
                    sw.Close();
                    #endregion
                }
            }
            catch (Exception e)
            {
                return;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            m_LoopTime = Convert.ToInt32(textBox2.Text);

        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            listBox1.Items.Clear();
        }


        private void InitUdpServer()
        {
            object Udp_info = "";

            client = new UdpClient(8000);

            //server�����Ķ˿� 

            IPEndPoint ep = null;

            //Զ�˵��ս�㣬��receive��ʱ���õ�����˭�����ң��Ҿͷ���˭
            byte[] data = new byte[0];
            while (true)
            {
                if (m_Stop) break;

                try
                {
                    data = client.Receive(ref ep);
                }
                catch
                {
                }

                string strIP = Encoding.UTF8.GetString(data);
                IPAddress outTmpIp;
                if (IPAddress.TryParse(strIP, out outTmpIp))
                {
                    Udp_info = "����ϵͳʱ�䣺" + DateTime.Now.ToString() + "�յ�IPΪ��" + outTmpIp + "�Ķ�ʱ����!";

                    ThreadPool.QueueUserWorkItem(new WaitCallback(SaveRecorderData), Udp_info);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ViewRecorderData), Udp_info);
                }

                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(DateTime.Now.ToString());

                client.Send(bytes, bytes.Length, ep);

            } 
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            m_Port = Convert.ToInt32(textBox3.Text);
            Thread.Sleep(50);
            client.Close();
            threadUdpServer.Abort();
            threadUdpServer = null;
            threadUdpServer = new Thread(new ThreadStart(InitUdpServer));
            threadUdpServer.Start();

        }




 
    }
}