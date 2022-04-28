/****************************************************************************

    Com����
    ��ΰ 2009-10

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
        private string m_str_LostMessage = "";  //ʧ����ʾ��Ϣ
        private bool m_bln_ComState = false;
        private int m_int_Com;
        private string m_str_Setting = "38400,n,8,1";
        private int m_int_Channel = 1;      //����RTSEnable��DTREnable��4�����

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
                //�˿ڲ����ڣ���˿���ʹ��ʱ�������pseilwe2010-01-27

            }
        }

       
        #region ISerialport ��Ա

        public event RevEventDelegete DataReceive;

        /// <summary>
        /// ͨ��ѡ����Ҫ���ڿ���CL2011�豸�ģ���Чֵ1-4,
        /// ����PC���ڵ�RTSEnable��DTREnable��4�����
        /// CL2018-1������Ч��
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
            get { return 0; }   //��������
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
        /// �򿪶˿�
        /// </summary>
        /// <param name="str_Para">�˿ڲ�������ʽ���˿ں�,IP:Զ��Port��:����ʼPort��  ע������PC�˿ں����������0.0.0.0:0:0��ʾ</param>
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
                    this.m_str_LostMessage = "�˿����ò���������Ҫ��";
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
