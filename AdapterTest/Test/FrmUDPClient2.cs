using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
namespace ClAmMeterController.Test
{
    public partial class FrmUDPClient2 : Form
    {
        private Thread thread1;
        private string strConfigpath = Application.StartupPath;
        private string strConfigFile = "Config.cfg";
        private DateTime m_CurTime = new DateTime();                        //上次对时时间
        private Thread threadDo = null;                                     //检定线程
        private bool m_Stop = false;
        private Stopwatch sth_SpaceTicker = new Stopwatch();                //记时时钟
        private int m_LoopTime = 30;                                        //间隔时间
        private object obj = new object();

        private string m_ServerIP = "10.98.99.4";//"192.168.1.105"
        private int m_ServerPort = 8000;//端口
        private int m_ClientPort = 8080;//端口

        UdpClient client;

        public FrmUDPClient2()
        {
            InitializeComponent();
            string strTmp = "";

            strTmp =  pwClassLibrary.pwFile.File.ReadInIString(strConfigFile, "System", "IsAutoDs", "0");
            checkBox1.Checked = (strTmp == "0" ? false : true);

            strTmp =  pwClassLibrary.pwFile.File.ReadInIString(strConfigFile, "System", "AutoDsSj", "1");
            textBox2.Text = strTmp;
            m_LoopTime = Convert.ToInt32(textBox2.Text);

            strTmp =  pwClassLibrary.pwFile.File.ReadInIString(strConfigFile, "System", "ServerIP", "10.98.99.4");
            textBox1.Text = strTmp;
            m_ServerIP = textBox2.Text;


            strTmp =  pwClassLibrary.pwFile.File.ReadInIString(strConfigFile, "System", "ServerPort", "8000");
            textBox3.Text = strTmp;
            m_ServerPort = Convert.ToInt32(textBox3.Text);

            strTmp =  pwClassLibrary.pwFile.File.ReadInIString(strConfigFile, "System", "LoactPort", "8080");
            textBox4.Text = strTmp;
            m_ClientPort = Convert.ToInt32(textBox4.Text);

        }
        private void FrmUDPServer_Load(object sender, EventArgs e)
        {

            obj = "GPS对时客户端启动：" + DateTime.Now.ToString();
            ThreadPool.QueueUserWorkItem(new WaitCallback(SaveRecorderData), obj);

        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                m_Stop = false;
                start();
            }
            else
            {
                m_Stop = true;
                if (client != null) client.Close();
                if (threadDo != null) threadDo.Abort();
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

                if (m_Stop) break;//外部中止＝＝退出

                _CurTime = Convert.ToInt32(sth_SpaceTicker.ElapsedMilliseconds / (1000 * 60));

                if (_CurTime < m_LoopTime)
                    continue;// 最小的 等待源稳定的时间
                else
                {
                    InitClient();
                    sth_SpaceTicker.Reset();
                    sth_SpaceTicker.Start();
                }

            }
        }

        private void InitClient()
        {
            UdpClient client = new UdpClient(8080);


            IPHostEntry ipe = Dns.GetHostEntry(Dns.GetHostName()); 

            IPAddress ipa = ipe.AddressList[0];

            //client端的发送监听
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(ipa.ToString());


            client.Send(bytes, bytes.Length, new IPEndPoint(IPAddress.Parse(m_ServerIP), 8000));

            IPEndPoint ep= null;

            //server端，这里只是一个引用，返回数据时会把远端终结点发过来 
            byte[] data=new byte[0];
            try
            {
                data = client.Receive(ref ep);
            }
            catch
            {
            }

            string strtime=Encoding.Default.GetString(data);
            DateTime datetime = DateTime.Now;

            bool bln_Result = DateTime.TryParse(strtime, out datetime);
            if (bln_Result)
            {
                bln_Result = pwClassLibrary.pwWin32Api.Win32Api.SetSystemDateTime(strtime);

                if (bln_Result)
                {

                }
            }
            obj = "本机系统时间：" + datetime.ToString() + "，GPS服务器时间：" + datetime + "，执行GPS对时成功!";
            ThreadPool.QueueUserWorkItem(new WaitCallback(SaveRecorderData), obj);
            ThreadPool.QueueUserWorkItem(new WaitCallback(ViewRecorderData), obj);

            client.Close();
            return;


        }


        private void BtnSend_Click(object sender, EventArgs e)
        {
            InitClient();
        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            m_LoopTime = Convert.ToInt32(textBox2.Text);

        }

        private void FrmUDPClient2_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                e.Cancel = true;
                m_Stop = true;
                if(client!=null )client.Close();
                if(threadDo!=null)threadDo.Abort();
                Thread.Sleep(100);
                 pwClassLibrary.pwFile.File.WriteInIString(strConfigFile, "System", "IsAutoDs", checkBox1.Checked ? "1" : "0");
                 pwClassLibrary.pwFile.File.WriteInIString(strConfigFile, "System", "AutoDsSj", textBox2.Text);
                 pwClassLibrary.pwFile.File.WriteInIString(strConfigFile, "System", "ServerIP", textBox1.Text);
                 pwClassLibrary.pwFile.File.WriteInIString(strConfigFile, "System", "ServerPort", textBox3.Text);
                 pwClassLibrary.pwFile.File.WriteInIString(strConfigFile, "System", "LoactPort", textBox4.Text);
                obj = "GPS对时客户端退出：" + DateTime.Now.ToString();
                ThreadPool.QueueUserWorkItem(new WaitCallback(SaveRecorderData), obj);
            }
            catch
            {
            }
            finally
            {
                e.Cancel = false;
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
                    #region 写入文件
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



    }
    
}
