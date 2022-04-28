/****************************************************************************

    CL101标准源控制
    刘伟 2012-05

*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using pwInterface;
using System.Threading;
using System.Diagnostics;
using pwFunction.pwGlysModel;
namespace pwPower
{
    public class CCL109 : IPower
    {


        private string m_str_ID = "01";
        private ISerialport m_Ispt_com;
        private string m_str_LostMessage = "";
        private byte[] m_byt_RevData;
        private bool m_bln_Enabled = true;

        private int m_int_Channel = 1;          //通道

        #region Const
        private const int AskDat = 0xA0;  //
        private const int WrtDat = 0xA3; //
        private const int WrtAry = 0xA6;//
        private const int EchOk = 0x30;	//
        private const int EchErr = 0x33;	//
        private const int EchBsy = 0x35; //
        private const int EchInh = 0x36;	//

        private const int HeadPos = 0;
        private const int RxIDPos = 1;
        private const int TxIDPos = 2;
        private const int FlenPos = 3;
        private const int ComdPos = 4;
        private const int FRAME_ID = 0x81;

        private const int PagePos = 5;
        private const int GrpPos = 6;
        private const int AryPos = 6;
        private const int Start0Pos = 7;
        private const int Start1Pos = 8;
        private const int LenPos = 9;

        private const int Grp0Pos = 7;
        private const int Grp1Pos = 8;
        private const int Grp2Pos = 9;
        private const int Grp3Pos = 10;
        private const int Grp4Pos = 11;
        private const int Grp5Pos = 12;
        private const int Grp6Pos = 13;
        private const int Grp7Pos = 14;

        private const int LOCAL_ID = 0x05;
        private const int OBJ_ID = 0x01;


        private const int PAGE0 = 0;
        private const int PAGE1 = 1;
        private const int PAGE2 = 2;
        private const int PAGE3 = 3;
        private const int PAGE4 = 4;
        private const int PAGE5 = 5;
        private const int PAGE6 = 6;

        private const int GROUP0 = 0x01;
        private const int GROUP1 = 0x02;
        private const int GROUP2 = 0x04;
        private const int GROUP3 = 0x08;
        private const int GROUP4 = 0x10;
        private const int GROUP5 = 0x20;
        private const int GROUP6 = 0x40;
        private const int GROUP7 = 0x80;

        private const int DATA0 = 0x01;
        private const int DATA1 = 0x02;
        private const int DATA2 = 0x04;
        private const int DATA3 = 0x08;
        private const int DATA4 = 0x10;
        private const int DATA5 = 0x20;
        private const int DATA6 = 0x40;
        private const int DATA7 = 0x80;

        private const int COS_SIN_BEISHU = 10000;//COSIN放大倍数
        private const int JIAODU_BEISHU = 10000;//角度放大倍数
        private const int PINLV_BEISHU = 10000;//频率放大倍数
        #endregion

        private double m_UaXwValue = 0;//Ua相位角度
        private double m_UbXwValue = 240;//Ub相位角度
        private double m_UcXwValue = 120;//UC相位角度
        private double m_IaXwValue = 0;//IA相位角度
        private double m_IbXwValue = 240;
        private double m_IcXwValue = 120;



        public CCL109()
        {

        }



        #region IPower 成员

        private string m_str_Setting = "38400,n,8,1";
        /// <summary>
        /// 特波率
        /// </summary>
        public string Setting
        {
            set { this.m_str_Setting = value; }
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
        /// 通道选择，主要用于控制CL2011设备的，有效值1-4,
        /// 控制PC串口的RTSEnable和DTREnable的4种组合
        /// CL2018-1串口无效。
        /// </summary>
        public int Channel
        {
            get { return this.m_int_Channel; }
            set { this.m_int_Channel = value; }
        }


        public string LostMessage
        {
            get { return this.m_str_LostMessage; }
        }

        public int AdaptCom(ISerialport[] mySerialPort)
        {
            try
            {
                int int_AddEventNo = -1;
                for (int int_Inc = 0; int_Inc < mySerialPort.Length; int_Inc++)
                {
                    if (this.m_bln_Enabled == false)
                    {
                        this.m_str_LostMessage = "被中止搜索！";
                        return -1;
                    }
                    if (mySerialPort[int_Inc] != null)
                    {
                        if (int_AddEventNo == -1)      //是否要注册事件
                        {
                            mySerialPort[int_Inc].DataReceive += new RevEventDelegete(AdaptCom_DataReceive);
                            int_AddEventNo = int_Inc;
                        }
                        byte[] byt_SendData = this.OrgFrame(0x52);
                        this.m_byt_RevData = new byte[0];
                        mySerialPort[int_Inc].SendData(byt_SendData);
                        Waiting(900, 300);
                        byte[] byt_RevData = new byte[0];
                        if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                        {
                            if (byt_RevData[4] == 0x4b)           //返回4b
                            {
                                mySerialPort[int_AddEventNo].DataReceive -= new RevEventDelegete(AdaptCom_DataReceive);
                                return int_Inc;
                            }
                        }
                    }
                }
                if (int_AddEventNo != -1) mySerialPort[int_AddEventNo].DataReceive -= new RevEventDelegete(AdaptCom_DataReceive);
                this.m_str_LostMessage = "没有搜索到端口！";
                return -1;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return -1;
            }
        }

        /// <summary>
        /// 联机
        /// </summary>
        /// <returns></returns>
        public bool link()
        {
            this.m_str_LostMessage = "";
            try
            {

                if (!this.m_Ispt_com.State)
                {
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                    this.m_str_LostMessage = m_Ispt_com.LostMessage;
                    return false;
                }

                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                byte[] byt_SendData = this.OrgFrame(0xC9);
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1000, 500);
                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x39)
                        return true;
                    else
                    {
                        this.m_str_LostMessage = "返回失败指令！";
                        return false;
                    }
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 读取版本
        /// </summary>
        /// <param name="str_Ver">版本</param>
        /// <returns></returns>
        public bool ReadVer(ref string str_Ver)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                byte[] byt_SendData = this.OrgFrame(0xC9);
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);
                byte[] byt_RevData = new byte[0];

                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x39)
                    {
                        str_Ver = ASCIIEncoding.UTF8.GetString(byt_RevData, 6, 22);
                        str_Ver = str_Ver.Replace("\0", "");
                        return true;
                    }
                    else
                    {
                        this.m_str_LostMessage = "返回失败指令！";
                        return false;
                    }

                }
                else
                    return false;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 读源过载信息
        /// </summary>
        /// <param name="str_OutPower">源过载</param>
        /// <returns></returns>
        public bool ReadOutPower(ref string str_OutPower)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                byte[] byt_SendData = this.OrgFrame1(0xA0);
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);
                byte[] byt_RevData = new byte[0];

                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x50)
                    {
                        str_OutPower = byt_RevData[8].ToString();
                        return true;
                    }
                    else
                    {
                        this.m_str_LostMessage = "返回失败指令！";
                        return false;
                    }

                }
                else
                    return false;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 按各项升源 
        /// </summary>
        /// <param name="Ua">A电压值</param>
        /// <param name="Ub"></param>
        /// <param name="Uc"></param>
        /// <param name="Ia"></param>
        /// <param name="Ib"></param>
        /// <param name="Ic"></param>
        /// <param name="PhiUa"></param>
        /// <param name="PhiUb"></param>
        /// <param name="PhiUc"></param>
        /// <param name="PhiIa"></param>
        /// <param name="PhiIb"></param>
        /// <param name="PhiIc"></param>
        /// <param name="Hz">频率</param>
        /// <param name="iClfs">接线方式</param>
        /// <param name="Xwkz">相位控制</param>
        /// <returns></returns>
        public bool PowerOn(float Ua, float Ub, float Uc, float Ia, float Ib, float Ic, float PhiUa, float PhiUb, float PhiUc,
            float PhiIa, float PhiIb, float PhiIc, float Hz, int iClfs, byte Xwkz)
        {
            this.m_str_LostMessage = "";

            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                byte[] byt_Value = new byte[67];

                byt_Value[0] = PAGE5;

                byt_Value[1] = GROUP1 + GROUP2 + GROUP6;

                //GROUP1 
                byt_Value[2] = DATA0 + DATA1 + DATA2 + DATA3 + DATA4 + DATA5; //len = 8

                int tmpvalue = 0;

                tmpvalue = (int)(PhiUc * JIAODU_BEISHU);//电压角度 C-B-A
                Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 3, 4);

                tmpvalue = (int)(PhiUb * JIAODU_BEISHU);//
                Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 7, 4);

                tmpvalue = (int)(PhiUa * JIAODU_BEISHU);
                Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 11, 4);

                tmpvalue = (int)(PhiIc * JIAODU_BEISHU);//电流角度 C-B-A
                Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 15, 4);

                tmpvalue = (int)(PhiIb * JIAODU_BEISHU);
                Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 19, 4);

                tmpvalue = (int)(PhiIa * JIAODU_BEISHU);
                Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 23, 4);



                //GROUP2
                byt_Value[27] = DATA0 + DATA1 + DATA2 + DATA3 + DATA4 + DATA5 + DATA6 + DATA7; //len = 33

                tmpvalue = (int)(Uc * 10000);
                Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 28, 4);
                byt_Value[32] = (byte)Convert.ToSByte(-4);

                tmpvalue = (int)(Ub * 10000);
                Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 33, 4);
                byt_Value[37] = (byte)Convert.ToSByte(-4);

                tmpvalue = (int)(Ua * 10000);
                Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 38, 4);
                byt_Value[42] = (byte)Convert.ToSByte(-4);

                tmpvalue = (int)(Ic * 1000000);
                Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 43, 4);
                byt_Value[47] = (byte)Convert.ToSByte(-6);

                tmpvalue = (int)(Ib * 1000000);
                Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 48, 4);
                byt_Value[52] = (byte)Convert.ToSByte(-6);

                tmpvalue = (int)(Ia * 1000000);
                Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 53, 4);
                byt_Value[57] = (byte)Convert.ToSByte(-6);

                //

                tmpvalue = (int)(Hz * 10000);
                Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 58, 4); ; //

                byt_Value[62] = DATA0 + DATA1 + DATA2; //

                byt_Value[63] = DATA0 + DATA1 + DATA2;//
                byt_Value[64] = DATA0 + DATA1 + DATA2 + DATA3 + DATA4 + DATA5 + DATA6; //
                byt_Value[65] = DATA0 + DATA1 + DATA2 + DATA3 + DATA4 + DATA5 + DATA6; //

                byt_Value[66] = 0x0;//不对标

                byte[] byt_SendData = this.OrgFrame(WrtDat, byt_Value);
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);

                string m_str_TxFrame = BitConverter.ToString(byt_SendData);//接收帧
                Waiting(800, 300);
                string m_str_RxFrame = BitConverter.ToString(this.m_byt_RevData);//返回帧

                byte[] byt_RevData = new byte[0];

                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {

                    if (byt_RevData[4] == 0x30)
                        return true;
                    else
                    {
                        this.m_str_LostMessage = "返回失败指令！";
                        return false;
                    }
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }


        /// <summary>
        /// 启动功率源
        /// </summary>
        /// <returns></returns>
        public bool RunPower()
        {
            this.m_str_LostMessage = "";

            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                int int_Len = 13;                                       //81、ID、len、CMD和Chksum各占一位 +数据域位
                byte[] byt_Value = new byte[int_Len];

                byt_Value[0] = 0x81;                                    //81
                byt_Value[1] = Convert.ToByte(this.m_str_ID, 16);       //ID
                byt_Value[2] = 0x05;                                    //SD
                byt_Value[3] = Convert.ToByte(int_Len % 256);           //Len

                byt_Value[4] = 0xA3;                                    //Cmd
                byt_Value[5] = 0x05;                                    //标示码
                byt_Value[6] = 0x44;                                    //标示码
                byt_Value[7] = 0x80;                                    //标示码
                byt_Value[8] = 0x01;                                    //频率更新标志,0不更新，1更新
                byt_Value[9] = 0x03;                                    //标示码
                byt_Value[10] = 0x38;                                   //幅值更新标志,XXIaIbIcUaUbUc,0不更新，1更新
                byt_Value[11] = 0x3F;                                   //相位更新标志,,XXIaIbIcUaUbUc,0不更新，1更新

                for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //校验码    校验码从81后面开始计算
                {
                    byt_Value[int_Len - 1] ^= byt_Value[int_Inc];               //Chksum
                }

                byte[] byt_SendData = this.OrgFrame(WrtDat, byt_Value);
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);

                string m_str_TxFrame = BitConverter.ToString(byt_SendData);//接收帧
                Waiting(800, 300);
                string m_str_RxFrame = BitConverter.ToString(this.m_byt_RevData);//返回帧

                byte[] byt_RevData = new byte[0];

                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {

                    if (byt_RevData[4] == 0x30)
                        return true;
                    else
                    {
                        this.m_str_LostMessage = "返回失败指令！";
                        return false;
                    }
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }


        /// <summary>
        /// 控制源输出，统一输出
        /// </summary>
        /// <param name="U">电压</param>
        /// <param name="I">电流</param>
        /// <param name="Phi">角度(电压电流夹角)</param>
        /// <param name="Hz">频率</param>
        /// <param name="iClfs">测试方式 0=PT4,1=QT4,2=P32,3=Q32,4=Q60,5=Q90,6=Q33,7=P</param>
        /// <param name="Xwkz">相位控制 XXIcIbIaUcUbUa</param>
        /// <returns>true 输出成功，false 输出失败</returns>
        public bool PowerOn(Single U, Single I, Single Phi, Single Hz, int iClfs, byte Xwkz)
        {
            Phi = 360 - Phi;
            m_IaXwValue = (m_UaXwValue + Phi) % 360;
            m_IbXwValue = (m_UbXwValue + Phi) % 360;
            m_IcXwValue = (m_UcXwValue + Phi) % 360;
            return PowerOn(U, U, U, I, I, I, (float)m_UaXwValue, (float)m_UbXwValue, (float)m_UcXwValue,
            (float)m_IaXwValue , (float)m_IbXwValue , (float)m_IcXwValue, Hz, iClfs, Xwkz);
        }


        /// <summary>
        /// 控源按功率因数输出
        /// </summary>
        /// <param name="U">电压</param>
        /// <param name="I">电流</param>
        /// <param name="Phi">功率因数(+表示正向，－表示反向；L表示感性,C表示容性)</param>
        /// <param name="Hz">频率</param>
        /// <param name="iClfs">测试方式 0=PT4,1=QT4,2=P32,3=Q32,4=Q60,5=Q90,6=Q33,7=P</param>
        /// <param name="HABC">元件：0=合元，1＝A元，2＝B元，3＝C元</param>
        /// <returns></returns>
        public bool PowerOn(float U, float I, string str_Glys, float Hz, int iClfs, enmElement emt_Element)
        {
            //把角度计算、相位控制转到底层
            byte Xwkz = 0x3F;
            //SetAcSourcePowerFactor(str_Glys);

            float phitmp = GetPhiGlys(iClfs, str_Glys);
            m_IaXwValue =( m_UaXwValue + phitmp) % 360;
            m_IbXwValue = ( m_UbXwValue + phitmp) % 360;
            m_IcXwValue = (m_UcXwValue + phitmp) % 360;
            return PowerOn(U, U, U, I, I, I, (float)m_UaXwValue, (float)m_UbXwValue, (float)m_UcXwValue,
            (float)m_IaXwValue, (float)m_IbXwValue, (float)m_IcXwValue, Hz, iClfs,Xwkz);
        }

        public bool PowerOff()
        {
            this.m_str_LostMessage = "";
            try
            {
                return PowerOn(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 50, 7, 0x00);
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 计算角度
        /// </summary>
        /// <param name="int_Clfs">测量方式</param>
        /// <param name="str_Glys">功率因数</param>
        /// <returns></returns>
        private Single GetPhiGlys(int int_Clfs, string str_Glys)
        {
            string str_CL = str_Glys.ToUpper().Substring(str_Glys.Length - 1, 1);
            Double dbl_XS = 0;
            if (str_CL == "C" || str_CL == "L")
                dbl_XS = Convert.ToDouble(str_Glys.Substring(0, str_Glys.Length - 1));
            else
                dbl_XS = Convert.ToDouble(str_Glys);
            Double dbl_Phase;
            if (int_Clfs == 1 || int_Clfs == 3 || int_Clfs == 6)
                dbl_Phase = Math.Asin(Math.Abs(dbl_XS));                              //无功计算
            else
                dbl_Phase = Math.Acos(Math.Abs(dbl_XS));                              //有功计算

            dbl_Phase = dbl_Phase * 180 / Math.PI;      //角度换算
            if (dbl_XS < 0) dbl_Phase = 180 + dbl_Phase;         //反向
            if (str_CL == "C") dbl_Phase = 360 - dbl_Phase;
            if (dbl_Phase < 0) dbl_Phase = 360 + dbl_Phase;
            dbl_Phase = 360 - dbl_Phase;
            dbl_Phase %= 360;
            return Convert.ToSingle(dbl_Phase);
        }

        #region 三相用
        /// <summary>
        /// 计算角度
        /// </summary>
        /// <param name="p_int_Clfs">测量方式</param>
        /// <param name="p_str_Glys">功率因素名</param>
        /// <param name="p_eet_Element">合分元</param>
        /// <returns></returns>
        private float GetPhiGlys(int p_int_Clfs, string p_str_Glys, enmElement p_eet_Element)
        {
            string str_GlysName = p_str_Glys.Replace("-", "").Replace("+", "");
            string str_GlysF = p_str_Glys.Contains("-") ? "-" : "+";
            string str_ClfsCode = "";
            string str_ElementCode = "";

            switch (p_int_Clfs)
            {
                case 0:
                    str_ClfsCode = "00";
                    break;
                case 1:
                    str_ClfsCode = "01";
                    break;
                case 2:
                    str_ClfsCode = "10";
                    break;
                case 3:
                    str_ClfsCode = "11";
                    break;
                case 4:
                    str_ClfsCode = "21";
                    break;
                case 5:
                    str_ClfsCode = "31";
                    break;
                case 6:
                    str_ClfsCode = "41";
                    break;
                case 7:
                    str_ClfsCode = "51";
                    break;
                default:
                    str_ClfsCode = "00";
                    break;
            }
            switch (p_eet_Element)
            {
                case enmElement.H:
                    str_ElementCode = "1";
                    break;
                case enmElement.A:
                    str_ElementCode = "2";
                    break;
                case enmElement.B:
                    str_ElementCode = "3";
                    break;
                case enmElement.C:
                    str_ElementCode = "4";
                    break;
                default:
                    str_ElementCode = "1";
                    break;
            }

            csGlys gly_Instance = new csGlys();
            gly_Instance.Load();
            string str_JD = gly_Instance.getJiaoDu(str_GlysName)[str_ClfsCode + str_ElementCode];
            Single sng_JD = Single.Parse(str_JD);
            if (str_GlysF.Equals("-"))
            {
                sng_JD += 180;
                sng_JD %= 360;
            }
            return sng_JD;
        }

        /// <summary>
        /// 计算角度 分相计算
        /// </summary>
        /// <param name="int_Clfs">测量方式</param>
        /// <param name="str_Glys">功率因数</param>
        /// <param name="bln_NXX">逆相序</param>
        /// <returns>返回数组，数组元素为各相ABC相电压电流角度</returns>
        private Single[] GetPhiGlys(int int_Clfs, string str_Glys, int int_Element, bool bln_NXX)
        {
            string str_CL = str_Glys.ToUpper().Substring(str_Glys.Length - 1, 1);
            Double dbl_XS = 0;
            if (str_CL == "C" || str_CL == "L")
                dbl_XS = Convert.ToDouble(str_Glys.Substring(0, str_Glys.Length - 1));
            else
                dbl_XS = Convert.ToDouble(str_Glys);
            Double dbl_Phase;

            if (int_Clfs == 1 || int_Clfs == 3 || int_Clfs == 6)
                dbl_Phase = Math.Asin(Math.Abs(dbl_XS));                              //无功计算
            else
                dbl_Phase = Math.Acos(Math.Abs(dbl_XS));                              //有功计算

            dbl_Phase = dbl_Phase * 180 / Math.PI;      //角度换算
            if (dbl_XS < 0) dbl_Phase = 180 + dbl_Phase;         //反向
            if (str_CL == "C") dbl_Phase = 360 - dbl_Phase;
            if (dbl_Phase < 0) dbl_Phase = 360 + dbl_Phase;

            Single sng_UIPhi = Convert.ToSingle(dbl_Phase);
            Single[] sng_Phi = new Single[6];

            if (bln_NXX)
            {
                sng_Phi[0] = 0;         //Ua
                sng_Phi[1] = 240;       //Ub
                sng_Phi[2] = 120;       //Uc
            }
            else
            {
                sng_Phi[0] = 0;         //Ua
                sng_Phi[1] = 120;       //Ub
                sng_Phi[2] = 240;       //Uc
            }


            sng_Phi[3] = sng_Phi[0] + sng_UIPhi;       //Ia
            sng_Phi[4] = sng_Phi[1] + sng_UIPhi;       //Ib
            sng_Phi[5] = sng_Phi[2] + sng_UIPhi;       //Ic

            if (int_Clfs == 2 || int_Clfs == 3)
            {
                sng_Phi[2] += 60;       //Uc
                sng_Phi[3] += 30;       //Ia
                sng_Phi[4] += 30;       //Ib
                sng_Phi[5] += 30;       //Ic
            }

            sng_Phi[3] %= 360;       //Ia
            sng_Phi[4] %= 360;       //Ib
            sng_Phi[5] %= 360;



            //0, 240, 120, 0, 240, 120
            //0, 240, 120, 180, 60, 300
            //0, 240, 120, 30, 270, 150
            //0, 240, 120, 210, 90, 330,

            return sng_Phi;
        }

        /// <summary>
        /// 设置源功率因数
        /// </summary>
        /// <param name="Glys">功率因数如(0.5L,1.0,-1,-0,0.5C)</param>
        /// <param name="jxfs">0-三相四线有功表PT4;1-三相三线有功表P32;2--三相四线真无功表(QT4);3--三相三线真无功表(Q32);4--三元件跨相90无功表(Q33);5--二元件跨相90无功表(Q90);6--二元件人工中点(60)无功表(Q60)</param>
        /// <returns></returns>
        public bool SetAcSourcePowerFactor(string Glys)
        {

            double XwUa = 0;
            double XwIa = 0;
            //double XwIb = 0;
            //double XwIc = 0;
            string strGlys = "";
            string LC = "";
            double LcValue = 0;
            double Phi = 0;
            int n = 1;
            strGlys = Glys;



            if (Glys == "0") strGlys = "0L";
            if (Glys == "-0") strGlys = "0C";
            LC = GetUnit(strGlys);
            if (LC.Length > 0)
            {
                LcValue = Convert.ToDouble(strGlys.Replace(LC, ""));
            }
            else
            {
                LcValue = Convert.ToDouble(strGlys);
            }


            #region
            Phi = Math.Round(Math.Acos(LcValue) * (180 / Math.PI), 5);
            XwUa = 0;
            if (LcValue > 0)
            {
                XwIa = 0;
                Phi = 1 * Phi;

            }
            else if (LcValue < 0)
            {
                XwIa = 180;
                Phi = -1 * Phi;
            }
            if (LC == "L")
            {
                Phi = 1 * Phi;
                XwIa = XwUa - Phi;
                if (XwIa < 0) XwIa = XwIa + 360;
                if (XwIa >= 360) XwIa = XwIa - 360;

            }
            if (LC == "C")
            {
                Phi = -1 * Phi;
                XwIa = XwUa - Phi;
                if (XwIa < 0) XwIa = XwIa + 360;
                if (XwIa >= 360) XwIa = XwIa - 360;
            }
            #endregion

            m_IaXwValue = XwIa;
            m_IbXwValue %= (XwIa + m_UbXwValue) ;
            m_IcXwValue %= (XwIa + m_UcXwValue) ;

            return true;
        }
        #endregion

        public bool ExtendCommand(byte byt_Cmd, byte[] byt_Data, ref byte[] byt_RevValue)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                byte[] byt_SendData = this.OrgFrame(byt_Cmd, byt_Data);
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1500, 400);
                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    byt_RevValue = byt_RevData;
                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        #endregion





        #region---私有-------------------------
        private void m_Ispt_com_DataReceive(byte[] bData)
        {
            int int_Len = bData.Length;
            int int_OldLen = this.m_byt_RevData.Length;
            Array.Resize(ref this.m_byt_RevData, int_Len + int_OldLen);
            Array.Copy(bData, 0, this.m_byt_RevData, int_OldLen, int_Len);
        }

        /// <summary>
        /// 用于搜索端口
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
        /// 等待数据返回
        /// </summary>
        /// <param name="int_MinSecond">等待返回时间</param>
        /// <param name="int_SpaceMSecond">等待返回帧字节间隔时间</param>
        private void Waiting(int int_MinSecond, int int_SpaceMSecond)
        {
            try
            {
                int int_OldLen = 0;
                Stopwatch sth_Ticker = new Stopwatch();                     //等待计时，
                Stopwatch sth_SpaceTicker = new Stopwatch();                //
                sth_SpaceTicker.Start();
                sth_Ticker.Start();
                while (this.m_bln_Enabled)
                {
                    System.Windows.Forms.Application.DoEvents();
                    if (this.m_byt_RevData.Length > int_OldLen)     //长度有改变
                    {
                        sth_SpaceTicker.Reset();
                        int_OldLen = this.m_byt_RevData.Length;
                        sth_SpaceTicker.Start();                    //字节间计时重新开始
                    }
                    else        //如果长度有没有增加，与前次收到数据时间隔500毫秒则退出
                    {
                        if (this.m_byt_RevData.Length > 0)      //已经收到一部分，则按字节间计时
                        {
                            if (sth_SpaceTicker.ElapsedMilliseconds >= int_SpaceMSecond)
                                break;
                        }
                    }
                    if (sth_Ticker.ElapsedMilliseconds >= int_MinSecond)        //总计时
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
        ///  组织帧
        /// </summary>
        /// <param name="byt_Cmd">控制码</param>
        /// <returns>返回组好的帧</returns>
        private byte[] OrgFrame(byte byt_Cmd)
        {

            int int_Len = 6;                  //81、ID、len、CMD和Chksum各占一位 +数据域位
            byte[] byt_Data = new byte[int_Len];
            byt_Data[0] = 129;                                  //81
            byt_Data[1] = Convert.ToByte(this.m_str_ID, 16);    //ID

            byt_Data[2] = 0x05;// Convert.ToByte(int_Len / 256);                                 //H  Len
            byt_Data[3] = Convert.ToByte(int_Len % 256);                                 //L  Len

            byt_Data[4] = byt_Cmd;    //Cmd

            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //校验码    校验码从81后面开始计算
            {
                byt_Data[int_Len - 1] ^= byt_Data[int_Inc];               //Chksum
            }
            return byt_Data;


        }
        /// <summary>
        ///  组织帧
        /// </summary>
        /// <param name="byt_Cmd">控制码</param>
        /// <returns>返回组好的帧</returns>
        private byte[] OrgFrame1(byte byt_Cmd)
        {

            int int_Len = 9;                  //81、ID、len、CMD和Chksum各占一位 +数据域位
            byte[] byt_Data = new byte[int_Len];
            byt_Data[0] = 129;                                  //81
            byt_Data[1] = Convert.ToByte(this.m_str_ID, 16);    //ID

            byt_Data[2] = 0x05;// Convert.ToByte(int_Len / 256);                                 //H  Len
            byt_Data[3] = Convert.ToByte(int_Len % 256);                                 //L  Len

            byt_Data[4] = byt_Cmd;    //Cmd
            byt_Data[5] = 0x02;    //Cmd
            byt_Data[6] = 0x01;    //Cmd
            byt_Data[7] = 0x80;    //Cmd

            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //校验码    校验码从81后面开始计算
            {
                byt_Data[int_Len - 1] ^= byt_Data[int_Inc];               //Chksum
            }
            return byt_Data;


        }


        /// <summary>
        ///  组织帧
        /// </summary>
        /// <param name="byt_Cmd">控制码</param>
        /// <param name="byt_Value">数据域</param>
        /// <returns>返回组好的帧</returns>
        private byte[] OrgFrame(byte byt_Cmd, byte[] byt_Value)
        {

            int int_Len = 6 + byt_Value.Length;                  //81、ID、len、CMD和Chksum各占一位 +数据域位
            byte[] byt_Data = new byte[int_Len];
            byt_Data[0] = 0x81;                                  //81
            byt_Data[1] = Convert.ToByte(this.m_str_ID, 16);    //ID

            byt_Data[2] = 0x05;//Convert.ToByte(int_Len / 256);                                 //H  Len
            byt_Data[3] = Convert.ToByte(int_Len % 256);                                 //L  Len

            byt_Data[4] = byt_Cmd;    //Cmd

            Array.Copy(byt_Value, 0, byt_Data, 5, byt_Value.Length);    //数据域数据

            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //校验码    校验码从81后面开始计算
            {
                byt_Data[int_Len - 1] ^= byt_Data[int_Inc];               //Chksum
            }
            return byt_Data;


        }


        /// <summary>
        /// 检查返回帧合法性
        /// </summary>
        /// <param name="byt_Data">返回帧</param>
        /// <returns></returns>
        private bool CheckFrame(byte[] byt_Value, ref byte[] byt_Data)
        {
            int int_Start = 0;
            int_Start = Array.IndexOf(byt_Value, (byte)0x81);
            if (int_Start < 0 || int_Start > byt_Value.Length) return false;    //没有81开头
            if (int_Start + 3 >= byt_Value.Length) return false;                //没有帧长度字节
            int int_Len = byt_Value[int_Start + 3];
            if (int_Len + int_Start > byt_Value.Length) return false;           //实际长度与帧长度不相符
            byte byt_ChkSum = 0;
            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)
            {
                byt_ChkSum ^= byt_Value[int_Start + int_Inc];
            }
            if (byt_ChkSum != byt_Value[int_Start + int_Len - 1]) return false; //校验码不正常
            Array.Resize(ref byt_Data, int_Len);    //数据域长度
            Array.Copy(byt_Value, int_Start, byt_Data, 0, int_Len);
            return true;
        }


        /// <summary>
        /// 转换成10个Bit的值
        /// </summary>
        /// <param name="sValue">转换值</param>
        /// <returns></returns>
        private byte[] To10Bit(Single sng_Value)
        {
            string sData = Convert.ToString(sng_Value);
            if (sData.IndexOf('.') <= 0) sData += ".";
            sData += "0000000000";
            sData = sData.Substring(0, 9);
            byte[] bPara = ASCIIEncoding.ASCII.GetBytes(sData);
            Array.Resize(ref bPara, bPara.Length + 1);
            bPara[9] = 48;          //Convert.ToByte((sng_Value - Math.Floor(sng_Value)) == 0 ? 46 : 48);
            return bPara;
        }

        private string GetUnit(string chrVal)  //得到量程的单位 //chrVal带单位的值如 15A
        {
            int i = 0;
            string cUnit = "";
            byte[] chrbytes = new byte[256];
            ASCIIEncoding ascii = new ASCIIEncoding();
            chrbytes = ascii.GetBytes(chrVal);
            for (i = 0; i < chrbytes.Length; ++i)
            {
                if (chrbytes[i] > 57)
                {
                    cUnit = chrVal.Substring(i);
                    break;
                }

            }
            return cUnit;
        }


        //private byte GetIScale(Single sngI)
        //{

        //    if (sngI <= 0.01) return 80;            //"50";
        //    else if (sngI <= 0.025) return 81;        //"51";
        //    else if (sngI <= 0.05) return 82;        // "52";
        //    else if (sngI <= 0.1) return 83;       // "53";
        //    else if (sngI <= 0.25) return 84;        //"54";
        //    else if (sngI <= 0.5) return 85;        //"55";
        //    else if (sngI <= 1) return 86;        //"56";
        //    else if (sngI <= 2.5) return 87;          // "57";
        //    else if (sngI <= 5) return 88;          //"58";
        //    else if (sngI <= 10) return 89;         //"59";
        //    else if (sngI <= 25) return 90;         //"5a";
        //    else if (sngI <= 50) return 91;         //"5b";
        //    else if (sngI <= 100) return 92;        // "5c";
        //    else return 92;// "5c";

        //    //if (sngI <= 0.12) return 80;            //"50";
        //    //else if (sngI <= 0.3) return 81;        //"51";
        //    //else if (sngI <= 0.6) return 82;        // "52";
        //    //else if (sngI <= 0.12) return 83;       // "53";
        //    //else if (sngI <= 0.3) return 84;        //"54";
        //    //else if (sngI <= 0.6) return 85;        //"55";
        //    //else if (sngI <= 1.2) return 86;        //"56";
        //    //else if (sngI <= 3) return 87;          // "57";
        //    //else if (sngI <= 6) return 88;          //"58";
        //    //else if (sngI <= 12) return 89;         //"59";
        //    //else if (sngI <= 30) return 90;         //"5a";
        //    //else if (sngI <= 60) return 91;         //"5b";
        //    //else if (sngI <= 120) return 92;        // "5c";
        //    //else return 92;// "5c";
        //}

        //private byte GetUScale(Single sngU)
        //{
        //    if (sngU <= 57 * 1.2) return 64;//"40";
        //    else if (sngU <= 120) return 65;// "41";
        //    else if (sngU <= 264) return 66;// "42";
        //    else if (sngU <= 480) return 67;//"43";
        //    else if (sngU <= 900) return 68;//"44";
        //    else return 66;// "42";
        //}

        ///// <summary>
        ///// 获得不平衡负载控制字
        ///// </summary>
        ///// <param name="IsYuan">合分元H,A,B,C</param>
        ///// <param name="IsAddI">是否加电流</param>
        ///// <param name="IsQd">是否是潜动</param>
        ///// <param name="QdRate">潜动电压比率</param>
        ///// <returns>控制字</returns>
        //private byte GetBlanceCmdstr(enmElement IsYuan, bool IsAddI, bool IsQd, int QdRate)
        //{
        //    int tmpByte;
        //    tmpByte = IsQd ? 0x80 : 0;//是否潜动
        //    switch (QdRate)
        //    {
        //        case 0:
        //            tmpByte = tmpByte + 0x10;//100%
        //            break;
        //        case 80:
        //            tmpByte = tmpByte + 0x00;//80%潜动
        //            break;
        //        case 100:
        //            tmpByte = tmpByte + 0x10;
        //            break;
        //        case 110:
        //            tmpByte = tmpByte + 0x20;
        //            break;
        //        case 115:
        //            tmpByte = tmpByte + 0x30;
        //            break;
        //        case 120:
        //            tmpByte = tmpByte + 0x40;
        //            break;
        //        default:
        //            tmpByte = tmpByte + 0x10;
        //            break;
        //    }
        //    tmpByte = IsQd ? tmpByte + 0x08 : tmpByte + 0x00;//是否潜动

        //    switch (IsYuan)//合分元
        //    {
        //        case enmElement.H:
        //            tmpByte = tmpByte + 0;

        //            break;
        //        case enmElement.A:
        //            tmpByte = tmpByte + 1;
        //            break;
        //        case enmElement.B:
        //            tmpByte = tmpByte + 2;
        //            break;
        //        case enmElement.C:
        //            tmpByte = tmpByte + 3;
        //            break;
        //        default:
        //            tmpByte = tmpByte + 0;
        //            break;
        //    }
        //    return (byte)tmpByte;

        //}


        #endregion












    }


}
