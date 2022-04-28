/****************************************************************************

    CL111标准表控制
    刘伟 2012-05

*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;
using pwInterface;
using System.Diagnostics;
namespace pwStdMeter
{
    public class CCL1115 : IStdMeter
    {

        private string m_str_ID = "30";
        private ISerialport m_Ispt_com;
        private string m_str_LostMessage = "";
        private byte[] m_byt_RevData;
        private bool m_bln_Enabled = true;
        private int m_int_Channel = 1;          //通道
        private string m_str_Setting = "38400,n,8,1";

        public CCL1115()
        {

        }


        #region ------接口成员------------------

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


        /// <summary>
        /// 特波率
        /// </summary>
        public string Setting
        {
            set { this.m_str_Setting = value; }
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

        public bool Enabled
        {
            get { return this.m_bln_Enabled; }
            set { this.m_bln_Enabled = true; }
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
                        byte[] byt_SendData = this.OrgFrame(0x20);
                        this.m_byt_RevData = new byte[0];
                        mySerialPort[int_Inc].SendData(byt_SendData);
                        Waiting(900, 300);
                        byte[] byt_RevData = new byte[0];
                        if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                        {
                            if (byt_RevData[3] == 32)           //
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
        /// 联机。
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

                byte[] byt_Data = new byte[3];

                byt_Data[0] = 0x02;
                byt_Data[1] = 0x02;
                byt_Data[2] = 0x40;

                byte[] byt_SendData = this.OrgFrame(0xA0, byt_Data);//读常数
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);
                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x50)//返回50
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
        /// 读版本号
        /// </summary>
        /// <param name="str_Ver"></param>
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
                    if (byt_RevData[4] == 39)
                    {
                        str_Ver = ASCIIEncoding.UTF8.GetString(byt_RevData, 4, 19);
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
        /// 1.	设置接线方式 81 30 PCID 0a a3 00 01 20 uclinemode CS
        /// </summary>
        /// <param name="Uclinemode">接线方式：CL1115：自动量程：08H 手动量程：88H</param>
        /// <returns></returns>
        public bool SetStdMeterUclinemode(byte Uclinemode)
        {

            #region uclinemode —— 接线方式 		
            //    BIT7   0——自动量程； 1——手动量程
            //    BIT6   0——三相四线； 1——三相三线
            //    BIT5   0——功率	； 1——A相小电压信号
            //    BIT3   1——PQ		；
            //    BIT2   1——Q33		；
            //BIT1   1——Q90		；
            //    BIT0   1——Q60		；
            //    其中BIT0~BIT3只能有一位为1，并与BIT6一起使用
            //目前有效设置如下 （BITX表示X位置1）
            //CL3115：
            //自动量程下：
            //    WIRE3P4L    (BIT3)					08H
            //    WIRE3P3L    (BIT6+ BIT3)			48H
            //    WIREQ390    (BIT6+ BIT2)			44H
            //    WIREQ290    (BIT6+ BIT1)			42H
            //    WIREQ260    (BIT6+ BIT0)			41H
            // 手动量程下：
            //    WIRE3P4L    (BIT7+BIT3)				88H
            //    WIRE3P3L    (BIT7+ BIT6+ BIT3)		C8H
            //    WIREQ390    (BIT7+ BIT6+ BIT2)		C4H
            //    WIREQ290    (BIT7+ BIT6+ BIT1)		C2H
            //    WIREQ260    (BIT7+ BIT6+ BIT0)		C1H
            //CL1115：
            //自动量程  （BIT3）						08H
                        //手动量程  （BIT7+BIT3）					88H
            #endregion
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                byte[] Energy_Mode = new byte[4];//设置标准表

                Energy_Mode[0] = 0x00;//PAGE
                Energy_Mode[1] = 0x01;//GROUP
                Energy_Mode[2] = 0x20;//DATA
                Energy_Mode[3] = Uclinemode;//DATA接线方式

                byte[] byt_SendData = this.OrgFrame(0xA3, Energy_Mode);

                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);

                byte[] byt_RevData = new byte[0];

                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x30)           //返回30，OK
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
        /// 2.	设置本机常数 81 30 PCID 0d a3 00 04 01 uiLocalnum CS
        /// </summary>
        /// <param name="lStdConst">标准表常数</param>
        /// <param name="bAuto">参数无效，适应接口</param>
        /// <returns></returns>
        public bool SetStdMeterConst(long lStdConst, bool bAuto)
        {
            try
            {

                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                byte[] byt_Value = new byte[7];
                byt_Value[0] = 0x00;
                byt_Value[1] = 0x04;
                byt_Value[2] = 0x01;


                Array.Copy(BitConverter.GetBytes(Convert.ToInt32(lStdConst)), 0, byt_Value, 3, 4);
                byte[] byt_SendData = this.OrgFrame(0xA3, byt_Value);
                this.m_byt_RevData = new byte[0];           //返回数据
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(900, 300);

                byte[] byt_RevData = new byte[0];

                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x30)//返回 成功 0x30
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
        /// 3.	设置电能指示 81 30 PCID 0b a3 00 08 01 usE1type CS
        /// </summary>
        /// <param name="int_EngeType">CL1115：总有功电能00H  ；CL3115：总有功电能00H  总无功电能40H  </param>
        /// <returns></returns>
        public bool SetStdMeterUsE1type(int int_EngeType)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                byte[] Energy_Mode = new byte[4];//设置标准表

                Energy_Mode[0] = 0x00;//PAGE
                Energy_Mode[1] = 0x08;// 0x04;//GROUP
                Energy_Mode[2] = 0x10;//DATA
                Energy_Mode[3] = Convert.ToByte(int_EngeType);//类型

                byte[] byt_SendData = this.OrgFrame(0xA3, Energy_Mode);

                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);

                byte[] byt_RevData = new byte[0];

                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x30)           //返回0x30，OK
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
        /// 4.	设置电能计算误差启动开关(启动标准表)
        /// </summary>
        /// <param name="UcE1switch">启动指令 0：停止计算  1：开始计算电能误差  2：开始计算电能走字</param>
        /// <returns></returns>
        public bool SetStdMeterUcE1switch(byte UcE1switch)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                //byte[] Energy_Mode = { 0x00, 0x04, 0x10, 0x01 };//设置标准表
                byte[] Energy_Mode = { 0x00, 0x08, 0x10, 0x01 };//设置标准表
                Energy_Mode[3] = 0x01;//DATA




                byte[] byt_SendData = this.OrgFrame(0xA3, Energy_Mode);

                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);

                byte[] byt_RevData = new byte[0];

                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x30)           //返回0x30，OK
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
        /// 5.	设置电能参数 81 30 PCID 0e a3 00 09 20 uclinemode 11 usE1type ucE1switch CS
        /// </summary>
        /// <param name="Uclinemode">接线方式：CL1115：自动量程：08H 手动量程：88H</param>
        /// <param name="UsE1type">电能指示：CL1115：总有功电能00H    CL3115：总有功电能00H  总无功电能40H  </param>
        /// <param name="UcE1switch">启动指令： 0：停止计算  1：开始计算电能误差  2：开始计算电能走字</param>
        /// <returns>[true-成功,false-失败]</returns>
        /// <returns></returns>
        public bool SetAmMeterParameter(byte Uclinemode, byte UsE1type, byte UcE1switch)
        {
            #region
            //uclinemode —— 接线方式 ，设置方式见设置接线方式命令
            //usE1type —— 电能指示，设置方式见设置电能指示命令 
            //ucE1switch —— 启动指令 0：停止计算  1：开始计算电能误差  2：开始计算电能走字
            #endregion

            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                byte[] Energy_Mode = new byte[8];//设置标准表

                Energy_Mode[0] = 0x00;//PAGE
                Energy_Mode[1] = 0x09;//GROUP
                Energy_Mode[2] = 0x20;//DATA
                Energy_Mode[3] = Uclinemode;//DATA接线方式
                Energy_Mode[4] = 0x11;//DATA电能指示
                Energy_Mode[5] = UsE1type;//DATA电能指示
                Energy_Mode[6] = 0x00;//DATA                
                Energy_Mode[7] = UcE1switch;//启动类型

                byte[] byt_SendData = this.OrgFrame(0xA3, Energy_Mode);

                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);
                System.Threading.Thread.Sleep(1200);
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);
                byte[] byt_RevData = new byte[0];

                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x30)           //返回30，OK
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
        /// 6.	读取测量数据81 30 PCID 0e a0 02 3f ff 80 3f ff ff 0f CS 
        /// </summary>
        /// <param name="str_Para"></param>
        /// <returns></returns>
        public bool ReadStdMeterInfo(bool bIsCL3115, ref string[] str_Para)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                this.m_Ispt_com.Setting = this.m_str_Setting;

                byte[] ReadAllStdMeterData = { 0x02, 0x3f, 0xff, 0xff, 0x3f, 0xff, 0xff, 0x0f };//读所有标准表数据的Group ,
                byte[] byt_SendData = this.OrgFrame(0xA0, ReadAllStdMeterData);
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1500, 400);
                if (this.m_byt_RevData.Length > 0)
                { ;}
                else
                {
                    this.m_Ispt_com.SendData(byt_SendData);
                }
                Waiting(1500, 400);
                //发多一遍
                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {

                    if (byt_RevData[4] == 0x50)           //返回的是32指令
                    {
                        #region     //-------------解析数据---------------

                        string[] str_Value = new string[35];    //35个数据
                        str_Para = new string[35];    //35个数据
                        //// 81 PCID 30 a9 50
                        //// 02 3f
                        //// ff
                        ////从 byte 8 开始 是 数据

                        byte[] byt_Tmp = new byte[5];

                        //struct _DATA{
                        //    volatile signed int iData;
                        //    volatile signed char cPower;
                        //}_attribute_((packed));
                        //电流电压幅值 数据类型为 _DATA
                        double d_DecimalPoint = 0;

                        Array.Copy(byt_RevData, 8, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[12]);
                        str_Value[0] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   // C 相电压幅值 

                        Array.Copy(byt_RevData, 13, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[17]);
                        str_Value[1] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   // B 相电压幅值
                        Array.Copy(byt_RevData, 18, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[22]);
                        str_Value[2] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   // A 相电压幅值
                        Array.Copy(byt_RevData, 23, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[27]);
                        str_Value[3] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   // C 相电流幅值 
                        Array.Copy(byt_RevData, 28, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[32]);
                        str_Value[4] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   // B 相电流幅值
                        Array.Copy(byt_RevData, 33, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[37]);
                        str_Value[5] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   // A 相电流幅值

                        byt_Tmp = new byte[4];
                        Array.Copy(byt_RevData, 38, byt_Tmp, 0, 4);
                        str_Value[6] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 5));   //频率 放大100000倍

                        //档位过载标志
                        //BIT5:Ia
                        //BIT4:Ib
                        //BIT3:Ic
                        //BIT2:Ua
                        //BIT1:Ub
                        //BIT0:Uc 1--过载 0--无过载
                        byt_Tmp = new byte[5];
                        Array.Copy(byt_RevData, 42, byt_Tmp, 0, 1);
                        str_Value[7] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0), 2).PadLeft(8, '0').Substring(0, 8);

                        //// 80

                        byt_Tmp = new byte[5];
                        Array.Copy(byt_RevData, 44, byt_Tmp, 0, 4);
                        str_Value[8] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //功率相角  放大10000倍

                        //// 3f 

                        byt_Tmp = new byte[5];
                        Array.Copy(byt_RevData, 49, byt_Tmp, 0, 4);
                        str_Value[9] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //C 相电压相位 放大10000倍
                        Array.Copy(byt_RevData, 53, byt_Tmp, 0, 4);
                        str_Value[10] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //B 相电压相位 放大10000倍 
                        Array.Copy(byt_RevData, 57, byt_Tmp, 0, 4);
                        str_Value[11] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //A 相电压相位 放大10000倍 
                        Array.Copy(byt_RevData, 61, byt_Tmp, 0, 4);
                        str_Value[12] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //C 相电流相位 放大10000倍 
                        Array.Copy(byt_RevData, 65, byt_Tmp, 0, 4);
                        str_Value[13] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //B 相电流相位 放大10000倍 
                        Array.Copy(byt_RevData, 69, byt_Tmp, 0, 4);
                        str_Value[14] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //A 相电流相位 放大10000倍 

                        //// ff

                        byt_Tmp = new byte[5];

                        Array.Copy(byt_RevData, 74, byt_Tmp, 0, 4);
                        str_Value[15] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //C 相相角 放大10000倍
                        Array.Copy(byt_RevData, 78, byt_Tmp, 0, 4);
                        str_Value[16] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //B 相相角 放大10000倍
                        Array.Copy(byt_RevData, 82, byt_Tmp, 0, 4);
                        str_Value[17] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //A 相相角 放大10000倍
                        Array.Copy(byt_RevData, 86, byt_Tmp, 0, 4);
                        str_Value[18] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //C 相有功功率因素 放大10000倍
                        Array.Copy(byt_RevData, 90, byt_Tmp, 0, 4);
                        str_Value[19] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //B 相有功功率因素 放大10000倍
                        Array.Copy(byt_RevData, 94, byt_Tmp, 0, 4);
                        str_Value[20] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //A 相有功功率因素 放大10000倍
                        Array.Copy(byt_RevData, 98, byt_Tmp, 0, 4);
                        str_Value[21] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //总有功功率因素 放大10000倍
                        Array.Copy(byt_RevData, 102, byt_Tmp, 0, 4);
                        str_Value[22] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //总无功功率因素 放大10000倍

                        //// ff

                        byt_Tmp = new byte[5];

                        //struct _DATA{
                        //    volatile signed int iData;
                        //    volatile signed char cPower;
                        //}_attribute_((packed));
                        //功率 数据类型为 _DATA


                        Array.Copy(byt_RevData, 107, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[111]);
                        str_Value[23] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   //C 相有功功率
                        Array.Copy(byt_RevData, 112, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[116]);
                        str_Value[24] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   //B 相有功功率
                        Array.Copy(byt_RevData, 117, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[121]);
                        str_Value[25] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   //A 相有功功率
                        Array.Copy(byt_RevData, 122, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[126]);
                        str_Value[26] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   //总有功功率

                        Array.Copy(byt_RevData, 127, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[131]);
                        str_Value[27] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   //C 相无功功率
                        Array.Copy(byt_RevData, 132, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[136]);
                        str_Value[28] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   //B 相无功功率
                        Array.Copy(byt_RevData, 137, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[141]);
                        str_Value[29] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   //A 相无功功率
                        Array.Copy(byt_RevData, 142, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[146]);
                        str_Value[30] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   //总无功功率

                        //// 0F

                        byt_Tmp = new byte[4];
                        Array.Copy(byt_RevData, 148, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[152]);
                        str_Value[31] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   //C 相视在功率
                        Array.Copy(byt_RevData, 153, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[157]);
                        str_Value[32] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   //B 相视在功率
                        Array.Copy(byt_RevData, 158, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[162]);
                        str_Value[33] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   //A 相视在功率
                        Array.Copy(byt_RevData, 163, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[167]);
                        str_Value[34] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   //总视在功率

                        str_Para = str_Value;
                        return true;
                        #endregion

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
        /// 读标准表数据
        /// 81 30 PCID 0e a0 02 3f ff 80 3f ff ff 0f CS 
        /// </summary>
        /// <param name="str_Para"></param>
        /// <returns></returns>
        public bool ReadStdMeterInfo(ref string[] str_Para)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                this.m_Ispt_com.Setting = this.m_str_Setting;

                byte[] ReadAllStdMeterData = { 0x02, 0x3f, 0xff, 0xff, 0x3f, 0xff, 0xff, 0x0f };//读所有标准表数据的Group ,
                byte[] byt_SendData = this.OrgFrame(0xA0, ReadAllStdMeterData);
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1500, 400);
                if (this.m_byt_RevData.Length > 0)
                { ;}
                else
                {
                    this.m_Ispt_com.SendData(byt_SendData);
                }
                Waiting(1500, 400);
                //发多一遍
                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {

                    if (byt_RevData[4] == 0x50)           //返回的是32指令
                    {
                        //-------------解析数据---------------
                        if (this.m_byt_RevData.Length == 179)
                        {
                            string[] str_Value = new string[26];    //35个数据
                            byte[] byt_Tmp = new byte[] { 0, 0, 0, 0 };




                            //str_Value[0] = Convert.ToString(byt_RevData[4]);    //测量方式
                            //str_Value[1] = Convert.ToString(byt_RevData[5], 2);  //相位开关,二进制表示




                            Array.Copy(byt_RevData, 38, byt_Tmp, 0, 4);          //频率,6、7
                            str_Value[6] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / 100000);  //频率

                            //int[] int_Dot = new int[9];         //小数点
                            //for (int int_Inc = 0; int_Inc < 9; int_Inc++)
                            //{
                            //    int_Dot[int_Inc] = byt_RevData[14 + int_Inc];
                            //}

                            //Ua(0),Ub(1),Uc(2),Ia(3),Ib(4),Ic(5), Freq(6), Phi_Ua(7), Phi_Ub(8), Phi_Uc(9),
                            //Phi_Ia(10),Phi_Ib(11), Phi_Ic(12),Psum(13),Qsum(14),Ssum(15), 
                            //Pa(16), Qa(17), Sa(18) ,Pb(19), Qb(20) ,Sb(21),Pc(22), 
                            //Qc(23), Sc(24),

                            Array.Clear(byt_Tmp, 0, 4);
                            Array.Copy(byt_RevData, 18, byt_Tmp, 0, 4);
                            str_Value[0] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 0 - GetByteFromByteArray(byt_RevData, 22)));   //Uc
                            //str_Value[0] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0));
                            Array.Copy(byt_RevData, 13, byt_Tmp, 0, 4);

                            str_Value[1] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 0 - GetByteFromByteArray(byt_RevData, 17)));   //Ib

                            Array.Copy(byt_RevData, 8, byt_Tmp, 0, 4);
                            str_Value[2] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 0 - GetByteFromByteArray(byt_RevData, 22)));   //Ub

                            Array.Copy(byt_RevData, 33, byt_Tmp, 0, 4);
                            str_Value[3] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 0 - GetByteFromByteArray(byt_RevData, 37)));   //Ia

                            Array.Copy(byt_RevData, 28, byt_Tmp, 0, 4);
                            str_Value[4] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 0 - GetByteFromByteArray(byt_RevData, 32)));   //ib

                            Array.Copy(byt_RevData, 23, byt_Tmp, 0, 4);
                            str_Value[5] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 0 - GetByteFromByteArray(byt_RevData, 27)));   //Ic

                            Array.Copy(byt_RevData, 67, byt_Tmp, 0, 4);
                            str_Value[7] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / 10000);   //Phi_Ua

                            Array.Copy(byt_RevData, 63, byt_Tmp, 0, 4);
                            str_Value[8] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / 10000);   //Phi_Ia

                            Array.Copy(byt_RevData, 59, byt_Tmp, 0, 4);
                            str_Value[9] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / 10000);   //Phi_Ub

                            Array.Copy(byt_RevData, 79, byt_Tmp, 0, 4);
                            str_Value[10] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / 10000);   //Phi_Ib

                            Array.Copy(byt_RevData, 75, byt_Tmp, 0, 4);
                            str_Value[11] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / 10000);   //Phi_Uc

                            Array.Copy(byt_RevData, 71, byt_Tmp, 0, 4);
                            str_Value[12] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / 10000);   //Phi_Ic

                            Array.Copy(byt_RevData, 127, byt_Tmp, 0, 4);
                            str_Value[13] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 0 - GetByteFromByteArray(byt_RevData, 131)));   //A P

                            Array.Copy(byt_RevData, 122, byt_Tmp, 0, 4);
                            str_Value[14] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 0 - GetByteFromByteArray(byt_RevData, 126)));   //B P

                            Array.Copy(byt_RevData, 117, byt_Tmp, 0, 4);
                            str_Value[15] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 0 - GetByteFromByteArray(byt_RevData, 121)));   //C P

                            Array.Copy(byt_RevData, 147, byt_Tmp, 0, 4);
                            str_Value[16] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 0 - GetByteFromByteArray(byt_RevData, 151)));   //A Q

                            Array.Copy(byt_RevData, 142, byt_Tmp, 0, 4);
                            str_Value[17] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 0 - GetByteFromByteArray(byt_RevData, 146)));   //B Q

                            Array.Copy(byt_RevData, 137, byt_Tmp, 0, 4);
                            str_Value[18] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 0 - GetByteFromByteArray(byt_RevData, 141)));   //C Q
                            //------------
                            Array.Copy(byt_RevData, 168, byt_Tmp, 0, 4);
                            str_Value[19] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 0 - GetByteFromByteArray(byt_RevData, 172)));   //A S

                            Array.Copy(byt_RevData, 163, byt_Tmp, 0, 4);
                            str_Value[20] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 0 - GetByteFromByteArray(byt_RevData, 167)));   //B S

                            Array.Copy(byt_RevData, 158, byt_Tmp, 0, 4);
                            str_Value[21] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 0 - GetByteFromByteArray(byt_RevData, 162)));   //C S


                            //int int_MaxDot = Math.Max(int_Dot[6], Math.Max(int_Dot[7], int_Dot[8]));        //表常数

                            Array.Copy(byt_RevData, 132, byt_Tmp, 0, 4);
                            str_Value[22] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 0 - GetByteFromByteArray(byt_RevData, 136)));   //Sum P

                            Array.Copy(byt_RevData, 152, byt_Tmp, 0, 4);
                            str_Value[23] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 0 - GetByteFromByteArray(byt_RevData, 156)));   //Sum Q

                            Array.Copy(byt_RevData, 173, byt_Tmp, 0, 4);
                            str_Value[24] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 0 - GetByteFromByteArray(byt_RevData, 177)));   //Sum S

                            Array.Copy(byt_RevData, 50, byt_Tmp, 0, 4);
                            str_Value[25] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0));// / Math.Pow(10, byt_RevData[177]));   //表常数

                            //Array.Copy(byt_RevData, 54, byt_Tmp, 0, 4);
                            //str_Value[17] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, byt_RevData[177]));   //表常数


                            str_Para = str_Value;
                            return true;
                        }
                        else
                            return false;
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
        /// 7.	读取真实本机常数  81 30 PCID 09 a0 02 02 40 CS
        /// </summary>
        /// <param name="lngConst"></param>
        /// <returns></returns>
        public bool ReadStdMeterConst(ref long lngConst)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;


                byte[] MeterConst = { 0x02, 0x02, 0x40 };

                byte[] byt_SendData = this.OrgFrame(0xA0, MeterConst);

                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);

                byte[] byt_RevData = new byte[0];

                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x50)
                    {
                        //-----解析数据-----
                        byte[] byt_Tmp = new byte[5];

                        Array.Copy(byt_RevData, 8, byt_Tmp, 0, 4);

                        lngConst = long.Parse(Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0))); ;   // 读取真实本机常数

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
        /// 12.	设置标准表档位
        /// </summary>
        /// <param name="sngIa"></param>
        /// <param name="sngIb"></param>
        /// <param name="sngIc"></param>
        /// <param name="sngUa"></param>
        /// <param name="sngUb"></param>
        /// <param name="sngUc"></param>
        /// <returns></returns>
        public bool SetUIScale(float sngIa, float sngIb, float sngIc, float sngUa, float sngUb, float sngUc)
        {
            //电流档位,0=100A  1=50A  2=25A  3=10A 4=5A  5=2.5A  6=1A  7=0.5A 8=0.25A  9=0.1A  10=0.05A  11=0.025A  15=自动档
            int int_IaScale = GetIScaleIndex(sngIa);
            int int_IbScale = GetIScaleIndex(sngIb);
            int int_IcScale = GetIScaleIndex(sngIc);
            //电压档位
            int int_UaScale = GetUScaleIndex(sngUa);
            int int_UbScale = GetUScaleIndex(sngUb);
            int int_UcScale = GetUScaleIndex(sngUc);
            return SetUIScale(int_IaScale, int_IbScale, int_IcScale, int_UaScale, int_UbScale, int_UcScale);
        }

        /// <summary>
        /// 13.	设置标准表界面
        /// </summary>
        /// <param name="int_Type">谐波柱图界面：09H；谐波列表界面：0aH；波形界面：0bH；清除设置界面：feH</param>
        /// <returns></returns>
        public bool SetLCDMenu(byte int_Type)
        {
            this.m_str_LostMessage = "";
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                byte[] Energy_Mode = new byte[4];//设置标准表界面

                Energy_Mode[0] = 0x00;//PAGE
                Energy_Mode[1] = 0x10;//GROUP
                Energy_Mode[2] = 0x80;//DATA
                Energy_Mode[3] = int_Type;//清除设置界面：feH

                byte[] byt_SendData = this.OrgFrame(0xA3, Energy_Mode);

                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);

                byte[] byt_RevData = new byte[0];

                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x30)           //返回30，OK
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
        /// 清除设置界面
        /// </summary>
        /// <returns></returns>
        public bool ExitLCDMenu()
        {
            this.m_str_LostMessage = "";
            return SetLCDMenu(0xFE);
        }

        /// <summary>
        /// 启动标准表
        /// </summary>
        /// <returns></returns>
        public bool RunStdMeter()
        {
            return SetStdMeterUcE1switch(0x01);

        }

        public bool ExtendCommand(byte byt_Cmd, byte[] byt_Data, ref byte[] byt_RevData)
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
                Waiting(2000, 300);
                byt_RevData = new byte[0];

                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                    return true;
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

        #region------私有成员------------------

        private bool SetConnection(int int_Type)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                byte[] byt_SendData = this.OrgFrame(0x64, (byte)int_Type);     //时入界面
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);

                byte[] byt_RevData = new byte[0];

                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 75)           //返回4b指令，则OK
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

        #region 设置档位
        private bool SetUIScale(int int_IaScale, int int_IbScale, int int_IcScale, int int_UaScale, int int_UbScale, int int_UcScale)
        {
            #region Ib
            //值0: 100A档位
            //值1: 50A档位
            //值2: 20A档位
            //值3: 10A档位
            //值4: 5A档位
            //值5: 2A档位
            //值6: 1A档位
            //值7: 0.5A档位
            //值8 : 0.2A档位
            //值9 : 0.1A档位
            //值a : 0.05A档位
            //值b : 0.02A档位
            //值c : 0.01A档位
            #endregion

            #region Ub
            //值1: 480V档位		
            //值2: 240V档位		
            //值3: 120V档位		
            //值4: 60V档位
            #endregion
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                byte[] byt_Value = new byte[] { 0x02, 0x02, 0x3F, (byte)int_UaScale, (byte)int_UbScale, (byte)int_UcScale, (byte)int_IaScale, (byte)int_IbScale, (byte)int_IcScale };

                byte[] byt_SendData = this.OrgFrame(0xA3, byt_Value);
                this.m_byt_RevData = new byte[0];           //返回数据
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(900, 300);
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

        private int GetIScaleIndex(Single sng_I)
        {
            //0=100A  1=50A  2=25A  3=10A 4=5A  5=2.5A  6=1A  7=0.5A 8=0.25A  9=0.1A  10=0.05A  11=0.025A  15=自动档
            if (sng_I > 120)                        //超过100A档，为自动档
                return 15;
            else if (120 >= sng_I && sng_I > 60)    //100A档范围内 120%   120---60
                return 0;
            else if (60 >= sng_I && sng_I > 30)     //50A档范围内 120%   60---30
                return 1;
            else if (30 >= sng_I && sng_I > 12)     //25A档范围内 120%   30---12
                return 2;
            else if (12 >= sng_I && sng_I > 6)      //10A档范围内 120%   12---6
                return 3;
            else if (6 >= sng_I && sng_I > 3)       //5A档范围内 120%   6---3
                return 4;
            else if (3 >= sng_I && sng_I > 1.2)       //2.5A档范围内 120%   3---1.2
                return 5;
            else if (1.2 >= sng_I && sng_I > 0.6)       //1A档范围内 120%   1.2---0.6
                return 6;
            else if (0.6 >= sng_I && sng_I > 0.3)       //0.5A档范围内 120%   0.6---0.3
                return 7;
            else if (0.3 >= sng_I && sng_I > 0.12)       //0.25A档范围内 120%   0.3---0.12
                return 8;
            else if (0.12 >= sng_I && sng_I > 0.06)       //0.1A档范围内 120%   0.12---0.06
                return 9;
            else if (0.06 >= sng_I && sng_I > 0.03)       //0.05A档范围内 120%   0.06---0.03
                return 10;
            else if (0.03 >= sng_I)                    //0.025A档范围内 120%   0.03---0
                return 11;
            else
                return 15;
        }

        private int GetUScaleIndex(Single sng_U)
        {
            //0=1000V  1=600V 2=380V 3=220V 4=100V  5=60V 6=30V  15=自动档

            if (sng_U > 1000)           //超过1000V 则自动档
                return 15;
            else if (1000 >= sng_U && sng_U > 600)          // 1000V 档  1000---600V
                return 0;
            else if (600 >= sng_U && sng_U > 380)           // 600V 档  600---380V
                return 1;
            else if (380 >= sng_U && sng_U > 220 * 1.2)           // 380V 档  380---220V
                return 2;
            else if (220 * 1.2 >= sng_U && sng_U > 100 * 1.2)           // 220V 档  220---100V
                return 3;
            else if (100 * 1.2 >= sng_U && sng_U > 60 * 1.2)            // 100V 档  100---60V
                return 4;
            else if (60 * 1.2 >= sng_U && sng_U > 30 * 1.2)             // 60V 档  100---60V
                return 5;
            else if (30 * 1.2 >= sng_U)                           // 30V 档  100---60V
                return 6;
            else
                return 15;
        }

        /// <summary>
        /// 查询标准表常数
        /// </summary>
        /// <param name="sng_I">输出电流</param>
        /// <returns></returns>
        public long SelectStdMeterConst(Single sng_I)
        {
            #region CL1115 脉冲常数
            long[] G_ConstPulse3115 = new long[13];              ///CL1115标准表脉冲常数表
            G_ConstPulse3115[0] = 8000000;  //	240V	100A 档位	8.00e6
            G_ConstPulse3115[1] = 16000000;//	240V	50A  档位	1.60e7
            G_ConstPulse3115[2] = 40000000;//	240V	20A  档位	4.00e7
            G_ConstPulse3115[3] = 80000000;//	240V	10A  档位	8.00e7
            G_ConstPulse3115[4] = 160000000;//	240V	5A   档位	1.60e8
            G_ConstPulse3115[5] = 400000000;//	240V	2A   档位	4.00e8
            G_ConstPulse3115[6] = 800000000;//	240V	1A   档位	8.00e8
            G_ConstPulse3115[7] = 1600000000;//	240V	0.5A 档位	1.60e9
            G_ConstPulse3115[8] = 2000000000;//	240V	0.2A 档位	2.00e9
            G_ConstPulse3115[9] = 2000000000;//	240V	0.1A 档位	2.00e9
            G_ConstPulse3115[10] = 2000000000;//	240V	0.05A档位	2.00e9
            G_ConstPulse3115[11] = 2000000000;//	240V	0.02A档位	2.00e9
            G_ConstPulse3115[12] = 2000000000;//	240V	0.01A档位	2.00e9
            #endregion
            int iXb = GetIScaleIndex(sng_I);
            if (iXb == 15) iXb = 12;
            return G_ConstPulse3115[iXb];
        }

        #endregion


        /// <summary>
        /// 组织帧
        /// </summary>
        /// <param name="byt_Cmd">控制码</param>
        /// <returns>返回组好的帧</returns>
        private byte[] OrgFrame(byte byt_Cmd)
        {

            byte[] byt_Data = new byte[6];
            byt_Data[0] = 0x81;                                  //81
            byt_Data[1] = Convert.ToByte(this.m_str_ID, 16);    //RID
            byt_Data[2] = 0x05;                                 //TID
            byt_Data[3] = 0x06;                                 //Len
            byt_Data[4] = byt_Cmd;    //Cmd
            for (int int_Inc = 1; int_Inc < 5; int_Inc++)
            {
                byt_Data[5] ^= byt_Data[int_Inc];               //Chksum
            }
            return byt_Data;

        }



        /// <summary>
        ///  组织帧
        /// </summary>
        /// <param name="byt_Cmd">控制码</param>
        /// <param name="byt_Data">数据域</param>
        /// <returns>返回组好的帧</returns>
        private byte[] OrgFrame(byte byt_Cmd, byte[] byt_Data)
        {
            int iLen = 6 + byt_Data.Length;                   //81、ID、len、CMD和Chksum各占一位 +数据域位
            byte[] byt_Frame = new byte[iLen];
            byt_Frame[0] = 129;                                  //81
            byt_Frame[1] = Convert.ToByte(this.m_str_ID, 16);    //ID
            byt_Frame[2] = 0x05;                                 //Pc机固定为05
            byt_Frame[3] = Convert.ToByte(iLen);                                 //Len
            byt_Frame[4] = byt_Cmd;    //Cmd

            if (byt_Data.Length > 0) Array.Copy(byt_Data, 0, byt_Frame, 5, byt_Data.Length);

            for (int int_Inc = 1; int_Inc < iLen - 1; int_Inc++)        //校验码    校验码从81后面开始计算
            {
                byt_Frame[iLen - 1] ^= byt_Frame[int_Inc];               //Chksum
            }
            return byt_Frame;
        }

        /// <summary>
        ///  组织帧
        /// </summary>
        /// <param name="byt_Cmd">控制码</param>
        /// <param name="byt_Data">数据域</param>
        /// <returns>返回组好的帧</returns>
        private byte[] OrgFrame(byte byt_Cmd, byte byt_Data)
        {
            int iLen = 7;                   //81、ID、len、CMD和Chksum各占一位 +数据域位
            byte[] byt_Frame = new byte[iLen];
            byt_Frame[0] = 0x81;                                  //81
            byt_Frame[1] = Convert.ToByte(this.m_str_ID, 16);    //ID
            byt_Frame[2] = 0x05;
            byt_Frame[3] = Convert.ToByte(iLen);                                 //Len
            byt_Frame[4] = byt_Cmd;    //Cmd
            byt_Frame[5] = byt_Data;

            for (int int_Inc = 1; int_Inc < iLen - 1; int_Inc++)        //校验码    校验码从81后面开始计算
            {
                byt_Frame[iLen - 1] ^= byt_Frame[int_Inc];               //Chksum
            }
            return byt_Frame;
        }



        /// <summary>
        /// 检查返回帧合法性
        /// </summary>
        /// <param name="byt_Data">返回帧</param>
        /// <returns></returns>
        private bool CheckFrame(byte[] byt_Value, ref byte[] byt_Data)
        {
            if (byt_Value.Length < 5) //帧格式，至少5个字节
            {
                if (byt_Value.Length == 0)
                    this.m_str_LostMessage = "没有返回数据!";
                else
                    this.m_str_LostMessage = "返回数据不完整！";
                return false;
            }

            int int_Start = 0;
            int_Start = Array.IndexOf(byt_Value, (byte)0x81);
            if (int_Start < 0 || int_Start > byt_Value.Length)
            {
                this.m_str_LostMessage = "返回帧不符合要求，找不到0x81!";
                return false;    //没有81开头
            }

            if (int_Start + 2 >= byt_Value.Length)
            {
                this.m_str_LostMessage = "没有返回数据!";
                return false;                //没有帧长度字节
            }
            int int_Len = byt_Value[int_Start + 3];
            if (int_Len + int_Start > byt_Value.Length)
            {
                this.m_str_LostMessage = "实际长度与帧长度不相符!";
                return false;           //实际长度与帧长度不相符
            }
            byte byt_ChkSum = 0;
            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)
            {
                byt_ChkSum ^= byt_Value[int_Start + int_Inc];
            }
            if (byt_ChkSum != byt_Value[int_Start + int_Len - 1])
            {
                this.m_str_LostMessage = "校验码不一致!";
                return false; //校验码不正常
            }
            Array.Resize(ref byt_Data, int_Len);    //数据域长度
            Array.Copy(byt_Value, int_Start, byt_Data, 0, int_Len);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pArray"></param>
        /// <param name="pLocStart"></param>
        /// <returns></returns>
        private sbyte GetByteFromByteArray(byte[] pArray, int pLocStart)
        {
            string Fmt16 = Convert.ToString(pArray[pLocStart], 16);
            sbyte ReturnValue = (Convert.ToSByte(Fmt16, 16));
            return ReturnValue;
        }
        #endregion
    }



}
