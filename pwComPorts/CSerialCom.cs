/****************************************************************************

    Com控制
    刘伟 2009-10

*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using pwInterface;
using System.IO.Ports;

namespace pwComPorts
{
    public class CSerialCom : ISerialport
    {
        private SerialPort m_spt_Com;
        private string m_str_LostMessage = "";  //失败提示信息
        private bool m_bln_ComState = false;
        private int m_int_Com;
        private string m_str_Setting = "38400,n,8,1";
        private int m_int_Channel = 1;      //设置RTSEnable和DTREnable的4种组合

        public CSerialCom()
        {

        }
       
        public void  IniParameter(int int_Com)
        {
            try
            {
                if (int_Com < 1 || int_Com > 255)
                {
                    this.m_bln_ComState = false;
                    return;
                }

                if (this.m_spt_Com == null)
                {
                    this.m_spt_Com = new SerialPort("COM" + int_Com.ToString(), 9600, Parity.None, 8, StopBits.One);
                    this.m_spt_Com.DataReceived += new SerialDataReceivedEventHandler(m_spt_Com_DataReceived);
                }
                    this.m_spt_Com.PortName = "COM" + int_Com.ToString();

                this.m_int_Com = int_Com;
                if (this.m_spt_Com.IsOpen == false)
                {
                    this.m_spt_Com.Open();
                }
                this.m_bln_ComState = true;
            }
            catch (Exception e)
            {
                this.m_bln_ComState = false;
                this.m_str_LostMessage = e.ToString();
                //端口不存在，或端口在使用时，会出错，pseilwe2010-01-27

            }
        }

       
        #region ISerialport 成员

        public event RevEventDelegete DataReceive;

        /// <summary>
        /// 通道选择，主要用于控制CL2011设备的，有效值1-4,
        /// 控制PC串口的RTSEnable和DTREnable的4种组合
        /// CL2018-1串口无效。
        /// </summary>
        public int Channel
        {
            get { return this.m_int_Channel; }
            set
            {
                if (value != this.m_int_Channel)
                {
                    this.m_int_Channel = value;
                    switch (this.m_int_Channel)
                    {
                        case 1:
                            this.m_spt_Com.RtsEnable = true;
                            this.m_spt_Com.DtrEnable = true;
                            break;

                        case 2:
                            this.m_spt_Com.RtsEnable = false;
                            this.m_spt_Com.DtrEnable = true;
                            break;

                        case 3:
                            this.m_spt_Com.RtsEnable = true;
                            this.m_spt_Com.DtrEnable = false;
                            break;

                        case 4:
                            this.m_spt_Com.RtsEnable = false;
                            this.m_spt_Com.DtrEnable = false;
                            break;
                        default:
                            break;
                    }
                }
            }
        }


        public int ComPort
        {
            get { return this.m_int_Com; }
            set
            {
                int int_OldCom = this.m_int_Com;
                try
                {
                    if (this.m_int_Com != value)
                    {
                        this.m_spt_Com.PortName = "COM" + value.ToString();
                        this.m_int_Com = value;
                        if (this.m_spt_Com.IsOpen == false)
                        {
                            this.m_spt_Com.Open();
                        }
                        this.m_spt_Com.RtsEnable = true;
                        this.m_spt_Com.DtrEnable = true;
                    }
                }
                catch (Exception e)
                {
                    this.m_spt_Com.PortName = "COM" + int_OldCom;
                    this.m_str_LostMessage = e.ToString();
                }
            }
        }

        public int ComType
        {
            get { return 0; }   //返回类型
        }

        public string IP
        {
            get { return "0.0.0.0:0:0"; }
        }

        public string LostMessage
        {
            get { return this.m_str_LostMessage; }
        }

        public string Setting
        {
            get
            {
                return this.m_str_Setting;
            }
            set
            {
                try
                {
                    if (value != this.m_str_Setting)
                    {
                        this.m_str_Setting = value;
                        string[] str_Para = this.m_str_Setting.Split(new char[] { ',' });
                        if (str_Para.Length < 4)
                        {
                            str_Para = this.m_str_Setting.Split(new char[] { '-' });
                        }
                        if (str_Para.Length >= 4)
                        {
                            this.m_spt_Com.BaudRate = Convert.ToInt16(str_Para[0]);
                            this.m_spt_Com.DataBits = Convert.ToInt16(str_Para[2]);
                   
                            if (str_Para[1].ToLower() == "n")
                                this.m_spt_Com.Parity = Parity.None;
                            else if (str_Para[1].ToLower() == "e")
                                this.m_spt_Com.Parity = Parity.Even;
                            else
                                this.m_spt_Com.Parity = Parity.Odd;

                            if (str_Para[3] == "1")
                                this.m_spt_Com.StopBits = StopBits.One;
                            else if (str_Para[3] == "2")
                                this.m_spt_Com.StopBits = StopBits.Two;
                            else if (str_Para[3] == "0")
                                this.m_spt_Com.StopBits = StopBits.None;
                            else
                                this.m_spt_Com.StopBits = StopBits.OnePointFive;
                        }
                    }
                }
                catch (Exception e)
                {
                    this.m_bln_ComState = false;
                    this.m_str_LostMessage = e.ToString();
                }
            }
        }

        public bool State
        {
            get { return this.m_bln_ComState; }
        }


        /// <summary>
        /// 打开端口
        /// </summary>
        /// <param name="str_Para">端口参数，格式：端口号,IP:远程Port号:绑定起始Port号  注：如是PC端口后面参数可以0.0.0.0:0:0表示</param>
        /// <returns></returns>
        public bool PortOpen(string str_Para)
        {
            try
            {
                string[] str_Data = str_Para.Split(new char[] { ',' });
                if (str_Data.Length >= 1)
                {
                    IniParameter(Convert.ToInt16(str_Data[0]));
                    return this.m_bln_ComState;
                }
                else
                {
                    this.m_str_LostMessage = "端口设置参数不符合要求。";
                    return false;
                }
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                this.m_bln_ComState = false;
                return false;
            }
        }

        public bool PortClose()
        {
            try
            {
                this.m_spt_Com.Close();
                this.m_bln_ComState = false;
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                this.m_bln_ComState = false;
                return false;
            }
        }

        public bool SendData(byte[] byt_Data)
        {
            try
            {
                if ( !m_bln_ComState)
                {
                    this.PortOpen(this.ComPort.ToString() + ",0.0.0.0:0:0");
                    this.Setting = m_str_Setting;
                }
                this.m_spt_Com.Write(byt_Data, 0, byt_Data.Length);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                this.m_bln_ComState = false;
                return false;
            }
        }

        #endregion



        void m_spt_Com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (this.State == false || m_bln_ComState == false) return;
                byte[] readBuffer = new byte[this.m_spt_Com.ReadBufferSize];
                if (!this.m_spt_Com.IsOpen) return;
                int iLen = this.m_spt_Com.Read(readBuffer, 0, readBuffer.Length);
                Array.Resize(ref readBuffer, iLen);
                if (DataReceive != null) DataReceive(readBuffer);
            }
            catch
            {
                return;
            }

        }



    }
}
