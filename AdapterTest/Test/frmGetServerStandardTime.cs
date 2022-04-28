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

namespace ClAmMeterController.Test
{
    public partial class frmGetServerStandardTime : Form
    {
        private void dfasfasd()
        {
            UdpClient udpClient = new UdpClient(11000);
            try
            {
                udpClient.Connect("www.contoso.com", 11000);

                // Sends a message to the host to which you have connected.
                Byte[] sendBytes = Encoding.ASCII.GetBytes("Is anybody there?");

                udpClient.Send(sendBytes, sendBytes.Length);

                // Sends a message to a different host using optional hostname and port parameters.
                UdpClient udpClientB = new UdpClient();
                udpClientB.Send(sendBytes, sendBytes.Length, "AlternateHostMachineName", 11000);

                //IPEndPoint object will allow us to read datagrams sent from any source.
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                // Blocks until a message returns on this socket from a remote host.
                Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes);

                // Uses the IPEndPoint object to determine which of these two hosts responded.
                Console.WriteLine("This is the message you received " +
                                             returnData.ToString());
                Console.WriteLine("This message was sent from " +
                                            RemoteIpEndPoint.Address.ToString() +
                                            " on their port number " +
                                            RemoteIpEndPoint.Port.ToString());

                udpClient.Close();
                udpClientB.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }


        }


        private UdpClient sendUdpClient;
        private UdpClient receiveUpdClient;
        public frmGetServerStandardTime()
        {
            InitializeComponent();

            IPAddress[] ips = Dns.GetHostAddresses("");
            tbxlocalip.Text = ips[0].ToString();
            int port = 51883;
            tbxlocalPort.Text = port.ToString();
            tbxSendtoIp.Text = ips[1].ToString();
            tbxSendtoport.Text = port.ToString();

        }


        // 接受消息
        private void btnReceive_Click(object sender, EventArgs e)
        {
            // 创建接收套接字
            IPAddress localIp = IPAddress.Parse(tbxlocalip.Text);
            IPEndPoint localIpEndPoint = new IPEndPoint(localIp, int.Parse(tbxlocalPort.Text));
            receiveUpdClient = new UdpClient(localIpEndPoint);


            Thread receiveThread = new Thread(ReceiveMessage);
            receiveThread.Start();
        }

        // 接收消息方法
        private void ReceiveMessage()
        {
            IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                try
                {
                    // 关闭receiveUdpClient时此时会产生异常
                    byte[] receiveBytes = receiveUpdClient.Receive(ref remoteIpEndPoint);

                    string message = Encoding.Unicode.GetString(receiveBytes);

                    // 显示消息内容
                    ShowMessageforView(lstbxMessageView, string.Format("{0}[{1}]", remoteIpEndPoint, message));
                }
                catch
                {
                    break;
                }
            }
        }

        // 利用委托回调机制实现界面上消息内容显示
        delegate void ShowMessageforViewCallBack(ListBox listbox, string text);
        private void ShowMessageforView(ListBox listbox, string text)
        {
            if (listbox.InvokeRequired)
            {
                ShowMessageforViewCallBack showMessageforViewCallback = ShowMessageforView;
                listbox.Invoke(showMessageforViewCallback, new object[] { listbox, text });
            }
            else
            {
                lstbxMessageView.Items.Add(text);
                lstbxMessageView.SelectedIndex = lstbxMessageView.Items.Count - 1;
                lstbxMessageView.ClearSelected();
            }
        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            if (tbxMessageSend.Text == string.Empty)
            {
                MessageBox.Show("发送内容不能为空","提示");
                return;
            }

            // 选择发送模式
            if (chkbxAnonymous.Checked == true)
            {
                // 匿名模式(套接字绑定的端口由系统随机分配)
                sendUdpClient = new UdpClient(0);
            }
            else
            {
                // 实名模式(套接字绑定到本地指定的端口)
                IPAddress localIp = IPAddress.Parse(tbxlocalip.Text);
                IPEndPoint localIpEndPoint = new IPEndPoint(localIp, int.Parse(tbxlocalPort.Text));
                sendUdpClient = new UdpClient(localIpEndPoint);
            }

            Thread sendThread = new Thread(SendMessage);
            sendThread.Start(tbxMessageSend.Text);
        }

        // 发送消息方法
        private void SendMessage(object obj)
        {
            string message = (string)obj;
            byte[] sendbytes = Encoding.Unicode.GetBytes(message);
            IPAddress remoteIp = IPAddress.Parse(tbxSendtoIp.Text);
            IPEndPoint remoteIpEndPoint = new IPEndPoint(remoteIp, int.Parse(tbxSendtoport.Text));
            sendUdpClient.Send(sendbytes, sendbytes.Length, remoteIpEndPoint);
          
            sendUdpClient.Close();
           
            // 清空发送消息框
            ResetMessageText(tbxMessageSend);
        }

        // 采用了回调机制
        // 使用委托实现跨线程界面的操作方式
        delegate void ResetMessageCallback(TextBox textbox);
        private void ResetMessageText(TextBox textbox)
        {
            // Control.InvokeRequired属性代表
            // 如果空间的处理与调用线程在不同线程上创建的，则为true,否则为false
            if (textbox.InvokeRequired)
            {
                ResetMessageCallback resetMessagecallback = ResetMessageText;
                textbox.Invoke(resetMessagecallback, new object[] { textbox });
            }
            else
            {
                textbox.Clear();
                textbox.Focus();
            }
        }

        // 停止接收
        private void btnStop_Click(object sender, EventArgs e)
        {
            receiveUpdClient.Close();
        }

        // 清空接受消息框
        private void btnClear_Click(object sender, EventArgs e)
        {
            this.lstbxMessageView.Items.Clear();
        }

        private void frmGetServerStandardTime_Load(object sender, EventArgs e)
        {

        }
    }
}

