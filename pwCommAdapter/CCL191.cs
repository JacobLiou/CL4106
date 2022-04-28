/****************************************************************************

    CL191����ʱ��Դ����
    ��ΰ 2009-10

*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using pwInterface;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace pwStdTime
{
    public class CCL191   :IStdTime 
    {

        private string m_str_ID = "80";
        private ISerialport m_Ispt_com;
        private string m_str_LostMessage = "";
        private byte[] m_byt_RevData;
        private bool m_bln_Enabled = true;
        private string m_str_Setting = "38400,n,8,1";
        private int m_int_Channel = 1;          //ͨ��

        public CCL191()
        {
            this.m_byt_RevData = new byte[0];
           
        }



        #region IStdTime ��Ա

        public string LostMessage
        {
            get { return this.m_str_LostMessage; }
        }

       
        
        public string ID
        {
            get
            {
                return this.m_str_ID;
            }
            set
            {
                this.m_str_ID = value;
            }
        }

        public ISerialport ComPort
        {
            get
            {
                return this.m_Ispt_com;
            }
            set
            {
                if (!value.Equals(this.m_Ispt_com))
                {
                    if (this.m_Ispt_com != null)
                    {
                        this.m_Ispt_com.DataReceive -= new RevEventDelegete(m_Ispt_com_DataReceive);
                    }
                    this.m_Ispt_com = value;
                    this.m_Ispt_com.DataReceive += new RevEventDelegete(m_Ispt_com_DataReceive);
                }
            }
        }

        /// <summary>
        /// ͨ��ѡ����Ҫ���ڿ���CL2011�豸�ģ���Чֵ1-4,
        /// ����PC���ڵ�RTSEnable��DTREnable��4�����
        /// CL2018-1������Ч��
        /// </summary>
        public int Channel
        {
            get { return this.m_int_Channel; }
            set { this.m_int_Channel = value; }
        }

        public string Setting
        {
            set
            {
                this.m_str_Setting = value;
            }
        }

        public bool Link()
        {
            try
            {
                if (!this.m_Ispt_com.State)
                {
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                    this.m_str_LostMessage = m_Ispt_com.LostMessage;
                    return false;
                }
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                //81 BF 20 0A A3 00 00 00 FF C9
                byte[] byt_SendData = new byte[] { 129, 191, 32, 10, 163, 0, 0, 0, 255, 201 };
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(500, 300);
                byte[] byt_RevData = this.m_byt_RevData;
                if (CheckFrame(byt_RevData))            //��鷵��֡�Ƿ�����
                {
                    if (byt_RevData[4] == 48)       //30H
                        return true;
                    else
                    {
                        this.m_str_LostMessage = "����ʧ��ָ�";
                        return false;
                    }
                }
                else
                    return false;
                
            }
            catch(Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                throw new Exception(e.ToString());
            }

        }
        /// <summary>
        /// ����ͨ��
        /// </summary>
        /// <param name="iType">0=��׼ʱ�����塢1=��׼��������</param>
        /// <returns></returns>
        public bool SetChannel(int iType)
        {
            try
            {

                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                //gstr191 = "81BF200AA30000000036"

                //0=��׼ʱ�����塢1=��׼��������

                //81 BF 20 0A A3 00 00 00 FF C9
                //81 BF 20 0A A3 00 00 00 00 36
                byte[] byt_SendData;
                if (iType == 0)
                    byt_SendData = new byte[] { 129, 191, 32, 10, 163, 0, 0, 0, 0, 54 };
                else
                    byt_SendData = new byte[] { 129, 191, 32, 10, 163, 0, 0, 0, 255, 201 };



                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(500, 300);
                byte[] byt_RevData = this.m_byt_RevData;
                if (CheckFrame(byt_RevData))            //��鷵��֡�Ƿ�����
                {
                    if (byt_RevData[4] == 48)       //30H
                        return true;
                    else
                    {
                        this.m_str_LostMessage = "����ʧ��ָ�";
                        return false;
                    }
                }
                else
                    return false;

            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                throw new Exception(e.ToString());
            }
        }


        public bool ReadGPSTime(ref string sDateTime)
        {
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                //81 BF 00 09 A0 00 00 00 16
                byte[] byt_SendData = new byte[] { 129, 191, 0, 9, 160, 0, 0, 0, 22 };
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1500, 500);
                byte[] byt_RevData = this.m_byt_RevData;
                if (CheckFrame(byt_RevData))            //��鷵��֡�Ƿ�����
                {
                    if (byt_RevData[4] == 80)    //50H
                    {

                        string str_Value = BitConverter.ToString(byt_RevData, 8, 9 ).Replace("-","");

                        sDateTime = Convert.ToInt32(str_Value.Substring(2, 2) + str_Value.Substring(0, 2), 16).ToString() + "-";
                        sDateTime += Convert.ToInt32(str_Value.Substring(6, 2) + str_Value.Substring(4, 2), 16).ToString() + "-";
                        sDateTime += Convert.ToInt32(str_Value.Substring(10, 2) + str_Value.Substring(8, 2), 16).ToString() + " ";
                        sDateTime += Convert.ToInt32(str_Value.Substring(12, 2), 16).ToString() + ":";
                        sDateTime += Convert.ToInt32(str_Value.Substring(14, 2), 16).ToString() + ":";
                        sDateTime += Convert.ToInt32(str_Value.Substring(16, 2), 16).ToString();

                        return true;
                    }
                    else
                    {
                        this.m_str_LostMessage = "����ʧ��ָ�";
                        return false;
                    }
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                throw new Exception(e.ToString());
            }
        }

        public bool ReadTempHum(ref float Temperature, ref float Humidity)
        {
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //��Ҫ��ֹ��ͬһ��RS485����豸
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //��Ҫ��ֹͬһ��RS485��ͬһ��ͨ���ϣ���Ҫ����CL2011������
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                //81 BF 00 09 A0 00 03 00 15
                byte[] byt_SendData = new byte[] { 129, 191, 0, 9, 160, 0, 3, 0, 21 };
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1500, 500);
                byte[] byt_RevData = this.m_byt_RevData;
                if (CheckFrame(byt_RevData))            //��鷵��֡�Ƿ�����
                {
                    if (byt_RevData[4] == 80)    //50H
                    {


                    }
                }

                //01 03 00 00 00 02 c4 0b
                return true;


            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                throw new Exception(e.ToString());
            }
        }






        #endregion



        private void m_Ispt_com_DataReceive(byte[] bData)
        {

            int int_Len = bData.Length;
            int int_OldLen = this.m_byt_RevData.Length;
            Array.Resize(ref this.m_byt_RevData, int_Len + int_OldLen);
            Array.Copy(bData, 0, this.m_byt_RevData, int_OldLen, int_Len);

        }

        /// <summary>
        /// ���������˿�
        /// </summary>
        /// <param name="bData"></param>
        private void AdaptCom_DataReceive(byte[] bData)
        {
            int int_Len = bData.Length;
            int int_OldLen = this.m_byt_RevData.Length;
            Array.Resize(ref this.m_byt_RevData, int_Len + int_OldLen);
            Array.Copy(bData, 0, this.m_byt_RevData, int_OldLen, int_Len);
        }


        /// <summary>
        /// �ȴ����ݷ���
        /// </summary>
        /// <param name="int_MinSecond">�ȴ�����ʱ��</param>
        /// <param name="int_SpaceMSecond">�ȴ�����֡�ֽڼ��ʱ��</param>
        private void Waiting(int int_MinSecond, int int_SpaceMSecond)
        {
            try
            {
                int int_OldLen = 0;
                Stopwatch sth_Ticker = new Stopwatch();                     //�ȴ���ʱ��
                Stopwatch sth_SpaceTicker = new Stopwatch();                //
                sth_SpaceTicker.Start();
                sth_Ticker.Start();
                while (this.m_bln_Enabled)
                {
                    System.Windows.Forms.Application.DoEvents();
                    if (this.m_byt_RevData.Length > int_OldLen)     //�����иı�
                    {
                        sth_SpaceTicker.Reset();
                        int_OldLen = this.m_byt_RevData.Length;
                        sth_SpaceTicker.Start();                    //�ֽڼ��ʱ���¿�ʼ
                    }
                    else        //���������û�����ӣ���ǰ���յ�����ʱ���500�������˳�
                    {
                        if (this.m_byt_RevData.Length > 0)      //�Ѿ��յ�һ���֣����ֽڼ��ʱ
                        {
                            if (sth_SpaceTicker.ElapsedMilliseconds >= int_SpaceMSecond)
                                break;
                        }
                    }
                    if (sth_Ticker.ElapsedMilliseconds >= int_MinSecond)        //�ܼ�ʱ
                        break;
                    System.Threading.Thread.Sleep(1);
                }
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.Message;
            }
        }

       
        /// <summary>
        /// ��鷵��֡�Ϸ���
        /// </summary>
        /// <param name="byt_Data">����֡</param>
        /// <returns></returns>
        private bool CheckFrame(byte[] byt_Data)
        {
            if (byt_Data != null)
            {
                if (byt_Data.Length < 6) //֡��ʽ������5���ֽ�
                {
                    if (byt_Data.Length == 0)
                        this.m_str_LostMessage = "û�з�������!";
                    else
                        this.m_str_LostMessage = "�������ݲ�������";
                    return false;
                }
                else
                {
                    if (byt_Data.Length == byt_Data[3])    //֡�ĳ�����֡��˵���ĳ����Ƿ�һ��
                    {
                        byte byt_ChkSum = 0;
                        for (int int_Inc = 1; int_Inc < byt_Data.Length - 1; int_Inc++)           //У����ӵڶ����ֽڿ�ʼ���������ڶ����ֽڽ���
                        {
                            byt_ChkSum ^= byt_Data[int_Inc];
                        }
                        if (byt_ChkSum == byt_Data[byt_Data.Length - 1])
                            return true;
                        else
                        {
                            this.m_str_LostMessage = "���س��������ݳ��Ȳ�����";
                            return false;
                        }
                    }
                    else
                    {
                        this.m_str_LostMessage = "���س��������ݳ��Ȳ�����";
                        return false;
                    }
                }
            }
            else
            {
                this.m_str_LostMessage = "û�з������ݣ�";
                return false;
            }


        }

    }
}
