/****************************************************************************

    CL3115标准表控制
    SunBoy 2011-08

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
    public class CCL3115 : IStdMeter
    {
        #region  用户自定义

        private byte[] m_byt_RevData;

        #region 标准表地址 ID :CL3115本机地址 为 30H
        private string m_str_ID = "30";
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
        #endregion

        

        #region CL3115 PCID :PC/上位机通讯地址 01H(本设备对PCID不作要求 1字节)
        private string m_str_PCID = "01";
        public string PCID
        {
            get
            {
                return this.m_str_PCID;
            }
            set
            {
                this.m_str_PCID = value;
            }
        }
        #endregion

        #region 特波率 CL3115 与PC/上位机通讯接口 ---38400 与源通讯接口--9600
        private string m_str_Setting = "38400,n,8,1";
        public string Setting
        {
            get { return this.m_str_Setting; }
            set { this.m_str_Setting = value; }
        }
        #endregion

        #region   标准表串口
        private ISerialport m_Ispt_com;
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
        private void m_Ispt_com_DataReceive(byte[] bData)
        {
            int int_Len = bData.Length;
            int int_OldLen = this.m_byt_RevData.Length;
            Array.Resize(ref this.m_byt_RevData, int_Len + int_OldLen);
            Array.Copy(bData, 0, this.m_byt_RevData, int_OldLen, int_Len);
        }

        #endregion

        #region 失败信息
        private string m_str_LostMessage = "";
        public string LostMessage
        {
            get { return this.m_str_LostMessage; }
        }
        #endregion

        private bool m_bln_Enabled = true;
        public bool Enabled
        {
            get { return this.m_bln_Enabled; }
            set { this.m_bln_Enabled = true; }
        }

        #region CL2011设备 通道设置
        private int m_int_Channel = 1;
        /// 通道选择，主要用于控制CL2011设备的，有效值1-4,
        /// 控制PC串口的RTSEnable和DTREnable的4种组合
        /// CL2018-1串口无效。
        public int Channel
        {
            get { return this.m_int_Channel; }
            set { this.m_int_Channel = value; }
        }
        #endregion

        /// 用于搜索端口
        /// <param name="bData"></param>
        private void AdaptCom_DataReceive(byte[] bData)
        {
            int int_Len = bData.Length;
            int int_OldLen = this.m_byt_RevData.Length;
            Array.Resize(ref this.m_byt_RevData, int_Len + int_OldLen);
            Array.Copy(bData, 0, this.m_byt_RevData, int_OldLen, int_Len);
        }
        /// 自动适应标准表端口号
        /// <param name="mySerialPort">端口数组</param>
        /// <returns>适应成功则返回端口号，失败则返回-1</returns>
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

        #endregion

        public CCL3115()
        {

        }

        //#region ------ 接口成员------------------

        //谐波柱图界面  09H
        //谐波列表界面  0aH
        //波形界面      0bH
        //清除设置界面  feH

        //#region ------CCL3115 使用到的接口成员------------------

        #region 联机命令 使用07-读取真实本机常数做为联机命令  link()
        public bool link()
        {
            long uiActualLocalnum = 0;
            this.m_str_LostMessage = "";
            //try
            //{
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                byte[] byt_Data = new byte[3];

                byt_Data[0] = 0x02;
                byt_Data[1] = 0x02;
                byt_Data[2] = 0x40;

                byte[] byt_SendData = this.OrgFrame(0xA0, byt_Data);     //读取真实本机常数 0xA0
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);

                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x50)  //返回0x50，OK
                    {

                        #region     //-------------解析数据---------------

                        string[] str_Value = new string[35];    //35个数据

                        //// 81 PCID 30 0d 50
                        //// 02 02 40 
                        ////从 byte 8 开始 是 数据 4字节 低字节 先传
                        HexCon hexCon = new HexCon();

                        byte[] byt_Tmp = new byte[5];

                        Array.Copy(byt_RevData, 8, byt_Tmp, 0, 4);
                        str_Value[0] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0));   // 读取真实本机常数

                        uiActualLocalnum = long.Parse(Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0))); ;   // 读取真实本机常数

                        //str_Para = str_Value;
                        #endregion

                        return true;
                    }
                    else if (byt_RevData[4] == 0x33)  //返回0x33，失败返回
                    {
                        this.m_str_LostMessage = "失败返回！";
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

            //}
            //catch (Exception e)
            //{
            //    this.m_str_LostMessage = e.ToString();
            //    return false;
            //}
        }
        #endregion
        #region SetLCDMenu(int int_Type)---CL3115 暂时不适应这个 直接返回 ture
        public bool SetLCDMenu(int int_Type)
        {
            //CL3115 暂时不适应这个 直接返回 ture
            try
            {
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }
           #endregion

        #region CL3115 01-设置接线方式 bool SetUcLineMode( byte UcLineMode)
        /// <param name="UcLineMode">1字节</param>
        /// UcLineMode --接线方式
        ///    BIT7  0--自动量程   1--手动量程
        ///    BIT6  0--三相四线   1--三相三线
        ///    BIT5  0--功率       1--A相小电压信号
        ///    BIT4  
        ///    BIT3  1--PQ
        ///    BIT2  1--Q33
        ///    BIT1  1--Q90
        ///    BIT0  1--Q60
        /// 其中 BIT0--BIT3 只能有一位为 1，并与 BIT6一起使用
        /// 目前有效设置如下（BITX 表示 X 位置 1）
        /// CL3115
        ///    自动量程下：
        ///        WIRE3P4L (BIT3)                    08H
        ///        WIRE3P3L (BIT6+BIT3)               48H
        ///        WIREQ390 (BIT6+BIT2)               44H
        ///        WIREQ290 (BIT6+BIT1)               42H
        ///        WIREQ260 (BIT6+BIT0)               41H
        ///    手动量程下：
        ///        WIRE3P4L (BIT7+BIT3)               88H
        ///        WIRE3P3L (BIT7+BIT6+BIT3)          C8H
        ///        WIREQ390 (BIT7+BIT6+BIT2)          C4H
        ///        WIREQ290 (BIT7+BIT6+BIT1)          C2H
        ///        WIREQ260 (BIT7+BIT6+BIT0)          C1H
        /// CL1115
        ///   自动量程：
        ///        WIRE3P4L (BIT3)                    08H
        ///   手动量程：
        ///        WIRE3P4L (BIT7+BIT3)               88H
        /// 
        /// <returns>成功 返回 Cmd 30H 失败返回 Cmd 33H</returns>
        public bool SetUcLineMode(byte UcLineMode)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                byte[] byt_Data = new byte[4];

                byt_Data[0] = 0x00;
                byt_Data[1] = 0x01;
                byt_Data[2] = 0x20;
                byt_Data[3] = UcLineMode;

                byte[] byt_SendData = this.OrgFrame(0xA3, byt_Data);     //设置接线方式 0xA3
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1800, 300);

                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x30)  //返回0x30，OK
                    {
                        return true;
                    }
                    else if (byt_RevData[4] == 0x33)  //返回0x33，失败返回
                    {
                        this.m_str_LostMessage = "失败返回！";
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
        #endregion
        #region CL3115 02-设置本机常数 bool SetUiLocalnum(  int int_LocalNum)
        /// <param name="int_LocalNum">本机常数 4字节 低字节先传</param>
        /// <returns>成功 返回 Cmd 30H 失败返回 Cmd 33H</returns>
        public bool SetUiLocalnum(int int_LocalNum)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                byte[] byt_Data = new byte[7];

                byt_Data[0] = 0x00;
                byt_Data[1] = 0x04;
                byt_Data[2] = 0x01;

                string sLocaNum = int_LocalNum.ToString("X2").PadLeft(8, '0').Substring(0, 8);

                int j = 0;
                // 本机常数 4字节 低字节先传
                for (int i = sLocaNum.Length / 2; i > 0; i--)
                {
                    byt_Data[3 + j] = byte.Parse(sLocaNum.Substring(i * 2 - 2, 2), System.Globalization.NumberStyles.HexNumber);
                    j++;
                }

                byte[] byt_SendData = this.OrgFrame(0xA3, byt_Data);     //设置本机常数 0xA3
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);

                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x30)  //返回0x30，OK
                    {
                        return true;
                    }
                    else if (byt_RevData[4] == 0x33)  //返回0x33，失败返回
                    {
                        this.m_str_LostMessage = "失败返回！";
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
         #endregion
        #region CL3115 03-设置电能指示 bool SetUsE1type(int int_UsE1type);
        /// <param name="int_UsE1type">2字节 低字节先传</param>
        /// CL3115 总有功电能 00H
        ///        总无功电能 40H
        /// CL1115 总有功电能 00H
        /// 
        /// <returns>成功 返回 Cmd 30H 失败返回 Cmd 33H</returns>
        public bool SetUsE1type(int int_UsE1type)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                byte[] byt_Data = new byte[5];

                byt_Data[0] = 0x00;
                byt_Data[1] = 0x08;
                byt_Data[2] = 0x01;

                string sUsE1type = int_UsE1type.ToString("X2").PadLeft(4, '0').Substring(0, 4);

                int j = 0;
                // 设置电能指示 2字节 低字节先传
                for (int i = sUsE1type.Length / 2; i > 0; i--)
                {
                    byt_Data[3 + j] = byte.Parse(sUsE1type.Substring(i * 2 - 2, 2), System.Globalization.NumberStyles.HexNumber);
                    j++;
                }

                byte[] byt_SendData = this.OrgFrame(0xA3, byt_Data);     //设置电能指示 0xA3
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);

                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x30)  //返回0x30，OK
                    {
                        return true;
                    }
                    else if (byt_RevData[4] == 0x33)  //返回0x33，失败返回
                    {
                        this.m_str_LostMessage = "失败返回！";
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
              #endregion
        #region CL3115 04-设置电能计算误差启动开关  bool SetUcE1Swithch(byte byte_E1Swithch)
        /// <param name="int_UsE1type">1字节</param>
        /// E1Swithch  启动指令：
        ///     0--停止计算
        ///     1--开始计算电能误差
        ///     2--开始计算电能走字
        /// 
        /// <returns>成功 返回 Cmd 30H 失败返回 Cmd 33H</returns>
        public bool SetUcE1Swithch(byte byte_E1Swithch)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                byte[] byt_Data = new byte[4];

                byt_Data[0] = 0x00;
                byt_Data[1] = 0x08;
                byt_Data[2] = 0x10;
                byt_Data[3] = byte_E1Swithch;

                byte[] byt_SendData = this.OrgFrame(0xA3, byt_Data);     //设置电能计算误差启动开关 0xA3
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);

                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x30)  //返回0x30，OK
                    {
                        return true;
                    }
                    else if (byt_RevData[4] == 0x33)  //返回0x33，失败返回
                    {
                        this.m_str_LostMessage = "失败返回！";
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
          #endregion
        #region CL3115 05-设置电能参数  bool SetEPara(byte UcLineMode,int int_UsE1type,byte E1Swithch)
        /// <param name="UcLineMode">1字节</param>
        /// <param name="int_UsE1type">2字节 低字节先传</param>
        /// <param name="E1Swithch">1字节</param>
        ///===UcLineMode --接线方式===1字节=============================
        ///    BIT7  0--自动量程   1--手动量程
        ///    BIT6  0--三相四线   1--三相三线
        ///    BIT5  0--功率       1--A相小电压信号
        ///    BIT4  
        ///    BIT3  1--PQ
        ///    BIT2  1--Q33
        ///    BIT1  1--Q90
        ///    BIT0  1--Q60
        ///    其中 BIT0--BIT3 只能有一位为 1，并与 BIT6一起使用
        ///    目前有效设置如下（BITX 表示 X 位置 1）
        ///    CL3115
        ///        自动量程下：
        ///           WIRE3P4L (BIT3)                    08H
        ///           WIRE3P3L (BIT6+BIT3)               48H
        ///           WIREQ390 (BIT6+BIT2)               44H
        ///           WIREQ290 (BIT6+BIT1)               42H
        ///           WIREQ260 (BIT6+BIT0)               41H
        ///        手动量程下：
        ///           WIRE3P4L (BIT7+BIT3)               88H
        ///           WIRE3P3L (BIT7+BIT6+BIT3)          48H
        ///           WIREQ390 (BIT7+BIT6+BIT2)          44H
        ///           WIREQ290 (BIT7+BIT6+BIT1)          42H
        ///           WIREQ260 (BIT7+BIT6+BIT0)          41H
        ///    CL1115
        ///        自动量程：
        ///           WIRE3P4L (BIT3)                    08H
        ///        手动量程：
        ///           WIRE3P4L (BIT7+BIT3)               88H
        /// 
        /// 
        ///===UsE1type 电能指示==============2字节 低字节先传======================
        ///       CL3115 总有功电能 00H
        ///              总无功电能 40H
        ///       CL1115 总有功电能 00H
        /// 
        ///===E1Swithch  启动指令：===========1字节=====================
        ///     0--停止计算
        ///     1--开始计算电能误差
        ///     2--开始计算电能走字
        /// 
        /// <returns>成功 返回 Cmd 30H 失败返回 Cmd 33H</returns>
        public bool SetEPara(byte UcLineMode, int int_UsE1type, byte E1Swithch)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                byte[] byt_Data = new byte[8];

                byt_Data[0] = 0x00;
                byt_Data[1] = 0x09;
                byt_Data[2] = 0x20;
                byt_Data[3] = UcLineMode; //接线方式===1字节
                byt_Data[4] = 0x11;


                //UsE1type 电能指示==============2字节 低字节先传

                string sUsE1type = int_UsE1type.ToString("X2").PadLeft(4, '0').Substring(0, 4);
                int j = 0;
                // 设置电能指示 2字节 低字节先传
                for (int i = sUsE1type.Length / 2; i > 0; i--)
                {
                    byt_Data[5 + j] = byte.Parse(sUsE1type.Substring(i * 2 - 2, 2), System.Globalization.NumberStyles.HexNumber);
                    j++;
                }
                byt_Data[7] = E1Swithch; //启动指令：===========1字节



                byte[] byt_SendData = this.OrgFrame(0xA3, byt_Data);     //设置电能参数 0xA3
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);

                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x30)  //返回0x30，OK
                    {
                        return true;
                    }
                    else if (byt_RevData[4] == 0x33)  //返回0x33，失败返回
                    {
                        this.m_str_LostMessage = "失败返回！";
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

        #endregion
        #region CL3115 06-读取测量数据 ReadStdMeterInfo(ref string[] str_Para)
        public bool ReadStdMeterInfo(ref string[] str_Para)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                byte[] byt_Data = new byte[8];

                byt_Data[0] = 0x02;
                byt_Data[1] = 0x3F;
                byt_Data[2] = 0xFF;
                byt_Data[3] = 0x80;
                byt_Data[4] = 0x3F;
                byt_Data[5] = 0xFF;
                byt_Data[6] = 0xFF;
                byt_Data[7] = 0x0F;

                byte[] byt_SendData = this.OrgFrame(0xA0, byt_Data);     //读取测量数据 0xA0
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);

                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x50)  //返回0x50，OK
                    {

                        #region     //-------------解析数据---------------

                        string[] str_Value = new string[35];    //35个数据
                        str_Para = new string[35];    //35个数据
                        //// 81 PCID 30 a9 50
                        //// 02 3f
                        //// ff
                        ////从 byte 8 开始 是 数据
                        HexCon hexCon = new HexCon();

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

                        ////CS



                        str_Para = str_Value;
                        #endregion

                        return true;
                    }
                    else if (byt_RevData[4] == 0x33)  //返回0x33，失败返回
                    {
                        this.m_str_LostMessage = "失败返回！";
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
        private byte get_byt_Tmp3(byte byt_Tmp2)
        {
            byte byt_Tmp3 = 0x0;
            try
            {
                if (Convert.ToString(byt_Tmp2, 2).PadLeft(8, '0').Substring(0, 1) == "1")
                    byt_Tmp3 = 0xFF;
                else
                    byt_Tmp3 = 0x0;
                return byt_Tmp3;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return byt_Tmp3;
            }
        }
        #endregion
        #region CL3115 07-读取真实本机常数 bool GetuiActualLocalnum(ref long uiActualLocalnum)
        /// <param name="int_LocalNum">本机常数 4字节 低字节先传</param>
        /// <returns>成功 返回 81 PCID 30 0D 50 02 02 40 uiActualLocalnum CS</returns>
        /// <returns>失败返回 Cmd 33H</returns>
        public bool GetuiActualLocalnum(ref long uiActualLocalnum)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                byte[] byt_Data = new byte[3];

                byt_Data[0] = 0x02;
                byt_Data[1] = 0x02;
                byt_Data[2] = 0x40;

                byte[] byt_SendData = this.OrgFrame(0xA0, byt_Data);     //读取真实本机常数 0xA0
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);

                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x50)  //返回0x50，OK
                    {

                        #region     //-------------解析数据---------------

                        string[] str_Value = new string[35];    //35个数据

                        //// 81 PCID 30 0d 50
                        //// 02 02 40 
                        ////从 byte 8 开始 是 数据 4字节 低字节 先传
                        HexCon hexCon = new HexCon();

                        byte[] byt_Tmp = new byte[5];

                        Array.Copy(byt_RevData, 8, byt_Tmp, 0, 4);
                        str_Value[0] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0));   // 读取真实本机常数

                        uiActualLocalnum = long.Parse(Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0))); ;   // 读取真实本机常数

                        //str_Para = str_Value;
                        #endregion

                        return true;
                    }
                    else if (byt_RevData[4] == 0x33)  //返回0x33，失败返回
                    {
                        this.m_str_LostMessage = "失败返回！";
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
        #endregion

        #region CL3115 08-读取电能 bool ReadllE1(ref float float_llE1)
        /// <param name="int_llE1">累计电能 8字节 低字节先传</param>
        /// <returns>成功 返回 81 PCID 30 11 50 02 20 10 llE1 CS</returns>
        /// <returns>失败返回 Cmd 33H</returns>
        public bool ReadllE1(ref float float_llE1)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                byte[] byt_Data = new byte[3];

                byt_Data[0] = 0x02;
                byt_Data[1] = 0x20;
                byt_Data[2] = 0x10;

                byte[] byt_SendData = this.OrgFrame(0xA0, byt_Data);     //读取电能 0xA0
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);

                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x50)  //返回0x50，OK
                    {

                        #region     //-------------解析数据---------------

                        //// 81 PCID 30 11 50
                        //// 02 20 10 
                        ////从 byte 8 开始 是 数据 4字节 低字节 先传
                        HexCon hexCon = new HexCon();

                        byte[] byt_Tmp = new byte[8];

                        Array.Copy(byt_RevData, 8, byt_Tmp, 0, 8);
                        float_llE1 = float.Parse(Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 5)));  // 累计电能  放大10000倍
                        #endregion

                        return true;
                    }
                    else if (byt_RevData[4] == 0x33)  //返回0x33，失败返回
                    {
                        this.m_str_LostMessage = "失败返回！";
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
        #endregion
        #region CL3115 09-读取电能累计脉冲数 bool ReadllPulsenum1(ref long long_llPulsenum1)
        /// <param name="long_llPulsenum1">电能累计脉冲数 8字节 低字节先传</param>
        ///                                CLT协议(UINT8)/变量定义SIN8
        /// <returns>成功 返回 81 PCID 30 11 50 02 40 80 llPulsenum1 CS</returns>
        /// <returns>失败返回 Cmd 33H</returns>
        public bool ReadllPulsenum1(ref long long_llPulsenum1)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                byte[] byt_Data = new byte[3];

                byt_Data[0] = 0x02;
                byt_Data[1] = 0x40;
                byt_Data[2] = 0x80;

                byte[] byt_SendData = this.OrgFrame(0xA0, byt_Data);     //读取电能累计脉冲数 0xA0
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);

                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x50)  //返回0x50，OK
                    {

                        #region     //-------------解析数据---------------

                        //// 81 PCID 30 09 50
                        //// 02 40 80 
                        ////从 byte 8 开始 是 数据 4字节 低字节 先传
                        HexCon hexCon = new HexCon();

                        byte[] byt_Tmp = new byte[8];

                        Array.Copy(byt_RevData, 8, byt_Tmp, 0, 8);

                        long_llPulsenum1 = long.Parse(Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0))); ;   // 读取电能累计脉冲数

                        //str_Para = str_Value;
                        #endregion

                        return true;
                    }
                    else if (byt_RevData[4] == 0x33)  //返回0x33，失败返回
                    {
                        this.m_str_LostMessage = "失败返回！";
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
        #endregion
        #region CL3115 10-读取电能走字数据  bool ReadE1num(ref  float float_llE1, ref long long_llPulsenum1);
        /// <param name="float_llE1">累计电能 8字节 低字节先传</param>
        /// <param name="long_llPulsenum1">电能累计脉冲数 8字节 低字节先传</param>
        ///                                CLT协议(UINT8)/变量定义SIN8
        /// <returns>成功 返回 数据  </returns>
        /// <returns>失败返回 Cmd 33H</returns>
        public bool ReadE1num(ref  float float_llE1, ref long long_llPulsenum1)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                byte[] byt_Data = new byte[4];

                byt_Data[0] = 0x02;
                byt_Data[1] = 0x60;
                byt_Data[2] = 0x10;
                byt_Data[2] = 0x80;

                byte[] byt_SendData = this.OrgFrame(0xA0, byt_Data);     //读取电能走字数据 0xA0
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);

                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x50)  //返回0x50，OK
                    {

                        #region     //-------------解析数据---------------

                        //// 81 PCID 30 1a 50
                        //// 02 60 10 
                        ////从 byte 8 开始 是 数据 4字节 低字节 先传
                        HexCon hexCon = new HexCon();

                        byte[] byt_Tmp = new byte[8];

                        Array.Copy(byt_RevData, 8, byt_Tmp, 0, 8);
                        float_llE1 = float.Parse(Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 5)));  // 累计电能  放大10000倍

                        // 80

                        Array.Copy(byt_RevData, 17, byt_Tmp, 0, 8);
                        long_llPulsenum1 = long.Parse(Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0))); ;   // 累计电能

                        #endregion

                        return true;
                    }
                    else if (byt_RevData[4] == 0x33)  //返回0x33，失败返回
                    {
                        this.m_str_LostMessage = "失败返回！";
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
        #endregion
        #region CL3115 11-读取测量数据(2036自用)  bool ReadStdMeterInfo2036(ref string[] sPara);
        /// 读取标准表测量数据
        /// <param name="sPara">返回测量数据，先后顺序按协议格式</param>
        /// <returns>失败返回 Cmd 33H</returns>
        public bool ReadStdMeterInfo2036(ref string[] str_Para)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                byte[] byt_Data = new byte[8];

                byt_Data[0] = 0x02;
                byt_Data[1] = 0x3F;
                byt_Data[2] = 0xFF;
                byt_Data[3] = 0xFF;
                byt_Data[4] = 0x3F;
                byt_Data[5] = 0xFF;
                byt_Data[6] = 0xFF;
                byt_Data[7] = 0x0F;

                byte[] byt_SendData = this.OrgFrame(0xA0, byt_Data);     //读取测量数据(2036自用) 0xA0
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);

                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x50)  //返回0x50，OK
                    {

                        #region     //-------------解析数据---------------

                        string[] str_Value = new string[42];    //42个数据
                        str_Para = new string[42];//42个数据
                        //// 81 PCID 30 B3 50
                        //// 02 3f
                        //// ff
                        ////从 byte 8 开始 是 数据
                        HexCon hexCon = new HexCon();

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
                        byt_Tmp = new byte[1];
                        Array.Copy(byt_RevData, 42, byt_Tmp, 0, 1);
                        str_Value[7] = Convert.ToString(byt_RevData[42], 2).PadLeft(8, '0').Substring(0, 8);

                        //// FF

                        /// 电压档位： 值1:480V档位    值3:120V档位
                        ///            值2:240V档位    值4:60V 档位
                        /// 
                        /// 电流档位： 值0: 100A档位    值1:  50A档位  值2:  20A档位  值3: 10A档位  值4:  5A档位 
                        ///            值5:   2A档位    值6:   1A档位  值7: 0.5A档位  值8:0.2A档位  值9:0.1A档位
                        ///            值a:0.05A档位    值b:0.02A档位  值c:0.01A档位   
                        ///            
                        str_Value[8] = byt_RevData[44].ToString("X");// Uc档位
                        str_Value[9] = byt_RevData[45].ToString("X");// Ub档位
                        str_Value[10] = byt_RevData[46].ToString("X");// Ua档位
                        str_Value[11] = byt_RevData[47].ToString("X");// Ic档位
                        str_Value[12] = byt_RevData[48].ToString("X");// Ib档位
                        str_Value[13] = byt_RevData[49].ToString("X");// Ia档位

                        byt_Tmp = new byte[4];
                        Array.Copy(byt_RevData, 50, byt_Tmp, 0, 4);
                        str_Value[14] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0));   //真实本机常数
                        Array.Copy(byt_RevData, 54, byt_Tmp, 0, 4);
                        str_Value[15] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //功率相角 放大10000倍

                        //// 3f 

                        byt_Tmp = new byte[4];
                        Array.Copy(byt_RevData, 59, byt_Tmp, 0, 4);
                        str_Value[16] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //C 相电压相位 放大10000倍
                        Array.Copy(byt_RevData, 63, byt_Tmp, 0, 4);
                        str_Value[17] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //B 相电压相位 放大10000倍
                        Array.Copy(byt_RevData, 67, byt_Tmp, 0, 4);
                        str_Value[18] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //A 相电压相位 放大10000倍
                        Array.Copy(byt_RevData, 71, byt_Tmp, 0, 4);
                        str_Value[19] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //C 相电流相位 放大10000倍
                        Array.Copy(byt_RevData, 75, byt_Tmp, 0, 4);
                        str_Value[20] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //B 相电流相位 放大10000倍
                        Array.Copy(byt_RevData, 79, byt_Tmp, 0, 4);
                        str_Value[21] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //A 相电流相位 放大10000倍

                        //// ff

                        byt_Tmp = new byte[4];

                        Array.Copy(byt_RevData, 81, byt_Tmp, 0, 4);
                        str_Value[22] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 5));   //C 相相角 放大10000倍
                        Array.Copy(byt_RevData, 85, byt_Tmp, 0, 4);
                        str_Value[23] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 5));   //B 相相角 放大10000倍
                        Array.Copy(byt_RevData, 89, byt_Tmp, 0, 4);
                        str_Value[24] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 5));   //A 相相角 放大10000倍
                        Array.Copy(byt_RevData, 93, byt_Tmp, 0, 4);
                        str_Value[25] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //C 相有功功率因素 放大10000倍
                        Array.Copy(byt_RevData, 97, byt_Tmp, 0, 4);
                        str_Value[26] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //B 相有功功率因素 放大10000倍
                        Array.Copy(byt_RevData, 101, byt_Tmp, 0, 4);
                        str_Value[27] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //A 相有功功率因素 放大10000倍
                        Array.Copy(byt_RevData, 105, byt_Tmp, 0, 4);
                        str_Value[28] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //总有功功率因素 放大10000倍
                        Array.Copy(byt_RevData, 109, byt_Tmp, 0, 4);
                        str_Value[29] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //总无功功率因素 放大10000倍

                        //// ff

                        //struct _DATA{
                        //    volatile signed int iData;
                        //    volatile signed char cPower;
                        //}_attribute_((packed));
                        //功率 数据类型为 _DATA

                        byt_Tmp = new byte[4];
                        Array.Copy(byt_RevData, 111, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[115]);
                        str_Value[30] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   //C 相有功功率
                        Array.Copy(byt_RevData, 116, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[120]);
                        str_Value[31] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   //B 相有功功率
                        Array.Copy(byt_RevData, 121, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[125]);
                        str_Value[32] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   //A 相有功功率
                        Array.Copy(byt_RevData, 126, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[130]);
                        str_Value[33] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   //总有功功率

                        Array.Copy(byt_RevData, 131, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[135]);
                        str_Value[34] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   //C 相无功功率
                        Array.Copy(byt_RevData, 136, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[140]);
                        str_Value[35] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   //B 相无功功率
                        Array.Copy(byt_RevData, 141, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[145]);
                        str_Value[36] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   //A 相无功功率
                        Array.Copy(byt_RevData, 146, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[150]);
                        str_Value[37] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   //总无功功率

                        //// 0F

                        byt_Tmp = new byte[4];
                        Array.Copy(byt_RevData, 148, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[152]);
                        str_Value[38] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   //C 相视在功率
                        Array.Copy(byt_RevData, 153, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[157]);
                        str_Value[39] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   //B 相视在功率
                        Array.Copy(byt_RevData, 158, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[162]);
                        str_Value[40] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, d_DecimalPoint));   //A 相视在功率
                        Array.Copy(byt_RevData, 163, byt_Tmp, 0, 4);
                        d_DecimalPoint = Convert.ToDouble(256 - byt_RevData[167]);
                        str_Value[41] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 3));   //总视在功率

                        ////CS

                        str_Para = str_Value;
                        #endregion

                        return true;
                    }
                    else if (byt_RevData[4] == 0x33)  //返回0x33，失败返回
                    {
                        this.m_str_LostMessage = "失败返回！";
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
        #endregion
        #region CL3115 13-设置标准表界面   bool SetucARM_Menu(byte byte_ucARM_Menu)
        /// <param name="byte_ucARM_Menu">1字节</param>
        /// ucARM_Menu  标准表界面指示
        ///     目前有效设置如下(BITX 表示 X 位置 1)
        ///    
        ///     谐波柱图界面  09H
        ///     谐波列表界面  0aH
        ///     波形界面      0bH
        ///     清除设置界面  feH
        /// 
        /// <returns>成功 返回 Cmd 30H 失败返回 Cmd 33H</returns>
        public bool SetucARM_Menu(byte byte_ucARM_Menu)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                byte[] byt_Data = new byte[4];

                byt_Data[0] = 0x00;
                byt_Data[1] = 0x10;
                byt_Data[2] = 0x80;
                byt_Data[3] = byte_ucARM_Menu;

                byte[] byt_SendData = this.OrgFrame(0xA3, byt_Data);     //设置标准表界面 0xA3
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1800, 300);

                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x30)  //返回0x30，OK
                    {
                        return true;
                    }
                    else if (byt_RevData[4] == 0x33)  //返回0x33，失败返回
                    {
                        this.m_str_LostMessage = "失败返回！";
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
        #endregion
        #region CL3115 12-设置标准表输出档位
        /// <param name="int_ucUaRange">A相电压档位 1字节</param>
        /// <param name="int_ucUbRange">B相电压档位 1字节</param>
        /// <param name="int_ucUcRange">C相电压档位 1字节</param>
        /// <param name="int_ucIaRange">A相电流档位 1字节</param>
        /// <param name="int_ucIbRange">B相电流档位 1字节</param>
        /// <param name="int_ucIcRange">C相电流档位 1字节</param>
        /// <returns>成功 返回 Cmd 30H 失败返回 Cmd 33H</returns>
        ///===有效设置：档位设置无论CL3115 或 CL1115=================
        /// 电压档位： 值1:480V档位    值3:120V档位
        ///            值2:240V档位    值4:60V 档位
        /// 
        /// 电流档位： 值0: 100A档位    值1:  50A档位  值2:  20A档位  值3: 10A档位  值4:  5A档位 
        ///            值5:   2A档位    值6:   1A档位  值7: 0.5A档位  值8:0.2A档位  值9:0.1A档位
        ///            值a:0.05A档位    值b:0.02A档位  值c:0.01A档位   
        public bool SetOutRange(int int_ucUaRange, int int_ucUbRange, int int_ucUcRange,
                                int int_ucIaRange, int int_ucIbRange, int int_ucIcRange)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                byte[] byt_Data = new byte[9];

                byt_Data[0] = 0x02;
                byt_Data[1] = 0x02;
                byt_Data[2] = 0x3F;
                byt_Data[3] = byte.Parse(int_ucUcRange.ToString());
                byt_Data[4] = byte.Parse(int_ucUbRange.ToString());
                byt_Data[5] = byte.Parse(int_ucUaRange.ToString());

                byt_Data[6] = byte.Parse(int_ucIcRange.ToString());
                byt_Data[7] = byte.Parse(int_ucIbRange.ToString());
                byt_Data[8] = byte.Parse(int_ucIaRange.ToString());


                byte[] byt_SendData = this.OrgFrame(0xA3, byt_Data);     //设置标准表输出档位 0xA3
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1800, 300);

                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x30)  //返回0x30，OK
                    {
                        return true;
                    }
                    else if (byt_RevData[4] == 0x33)  //返回0x33，失败返回
                    {
                        this.m_str_LostMessage = "失败返回！";
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
        #endregion

        #region CL3115 B1-与源通讯指令 读取测量数据(源)   bool ReadStdMeterInfoSource(ref string[] sPara)
        /// 读取标准表测量数据
        /// <param name="sPara">返回测量数据，先后顺序按协议格式</param>
        /// <returns>失败返回 Cmd 33H</returns>
        public bool ReadStdMeterInfoSource(ref string[] str_Para)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                byte[] byt_Data = new byte[4];

                byt_Data[0] = 0x02;
                byt_Data[1] = 0x05;
                byt_Data[2] = 0x3F;
                byt_Data[3] = 0x3F;


                byte[] byt_SendData = this.OrgFrame(0xA0, byt_Data);     //读取测量数据(源) 0xA0
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);

                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x50)  //返回0x50，OK
                    {

                        #region     //-------------解析数据---------------

                        string[] str_Value = new string[12];    //12个数据
                        str_Para = new string[12];    //12个数据
                        //// 81 PCID 30 40 50
                        //// 02 05 
                        //// 3f
                        ////从 byte 8 开始 是 数据
                        HexCon hexCon = new HexCon();

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

                        //// 3f 

                        //相位数据类型为 signed int (4字节) 放大10000倍
                        byt_Tmp = new byte[4];
                        Array.Copy(byt_RevData, 39, byt_Tmp, 0, 4);
                        str_Value[6] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //C 相电压相位  放大10000倍
                        Array.Copy(byt_RevData, 43, byt_Tmp, 0, 4);
                        str_Value[7] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //B 相电压相位  放大10000倍
                        Array.Copy(byt_RevData, 47, byt_Tmp, 0, 4);
                        str_Value[8] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //A 相电压相位  放大10000倍
                        Array.Copy(byt_RevData, 51, byt_Tmp, 0, 4);
                        str_Value[9] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //C 相电流相位  放大10000倍
                        Array.Copy(byt_RevData, 55, byt_Tmp, 0, 4);
                        str_Value[10] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //B 相电流相位  放大10000倍
                        Array.Copy(byt_RevData, 59, byt_Tmp, 0, 4);
                        str_Value[11] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, 4));   //A 相电流相位  放大10000倍

                        ////CS

                        str_Para = str_Value;
                        #endregion

                        return true;
                    }
                    else if (byt_RevData[4] == 0x33)  //返回0x33，失败返回
                    {
                        this.m_str_LostMessage = "失败返回！";
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
        #endregion


        //#endregion ------CCL3115 使用到的接口成员------------------

        //#endregion ------ 接口成员------------------

        #region ------私有成员 ----------------


        #region 等待数据返回  Waiting(int int_MinSecond, int int_SpaceMSecond)
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
            finally
            {
                if (ComPort.State) ComPort.PortClose();
            }
        }
        #endregion

        #region 组织帧 byte[] OrgFrame(byte bCmd, byte[] Data)
        /// <param name="byt_Cmd">控制码</param>
        /// <param name="Data">数据域数组</param>
        /// <returns>返回组好的帧</returns>
        private byte[] OrgFrame(byte bCmd, byte[] Data)
        {
            byte[] byt_Data = new byte[6 + Data.Length];
            byte bLen = Convert.ToByte(byt_Data.Length);
            try
            {

                byt_Data[0] = 0x81;                                //81
                byt_Data[1] = Convert.ToByte(this.m_str_ID, 16);   //ID
                byt_Data[2] = Convert.ToByte(this.m_str_PCID, 16); //PCID
                byt_Data[3] = bLen;                                //Len  0x0a
                byt_Data[4] = bCmd;                                //Cmd  0xa3

                for (int i = 0; i < Data.Length; i++)              //data
                {
                    byt_Data[5 + i] = Data[i];
                }

                for (int int_Inc = 1; int_Inc < (byt_Data.Length - 1); int_Inc++)
                {
                    byt_Data[byt_Data.Length - 1] ^= byt_Data[int_Inc];               //Chksum
                }
                return byt_Data;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return byt_Data;
            }
        }
        #endregion

        #region  组织帧 OrgFrame(byte byt_Cmd, byte byt_Data)
        /// <param name="byt_Cmd">控制码</param>
        /// <param name="byt_Data">数据域</param>
        /// <returns>返回组好的帧</returns>
        private byte[] OrgFrame(byte byt_Cmd, byte byt_Data)
        {
            int iLen = 6;                   //81、ID、len、CMD和Chksum各占一位 +数据域位
            byte[] byt_Frame = new byte[iLen];
            try
            {
                byt_Frame[0] = 129;                                  //81
                byt_Frame[1] = Convert.ToByte(this.m_str_ID, 16);    //ID
                byt_Frame[2] = Convert.ToByte(iLen);                 //Len
                byt_Frame[3] = byt_Cmd;                              //Cmd
                byt_Frame[4] = byt_Data;                             //Data

                for (int int_Inc = 1; int_Inc < iLen - 1; int_Inc++) //校验码    校验码从81后面开始计算
                {
                    byt_Frame[iLen - 1] ^= byt_Frame[int_Inc];       //Chksum
                }
                return byt_Frame;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return byt_Frame;
            }
        }
        #endregion

        #region 组织帧 OrgFrame(byte byt_Cmd)
        /// <param name="byt_Cmd">控制码</param>
        /// <returns>返回组好的帧</returns>
        private byte[] OrgFrame(byte byt_Cmd)
        {
            byte[] byt_Data = new byte[5];
            try
            {

                byt_Data[0] = 0x81;                                  //81
                byt_Data[1] = Convert.ToByte(this.m_str_ID, 16);    //ID
                byt_Data[2] = 5;                                 //Len
                byt_Data[3] = byt_Cmd;    //Cmd
                for (int int_Inc = 1; int_Inc < 4; int_Inc++)
                {
                    byt_Data[4] ^= byt_Data[int_Inc];               //Chksum
                }
                return byt_Data;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return byt_Data;
            }
        }
        #endregion

        #region 检查返回帧合法性  CheckFrame(byte[] byt_Value, ref byte[] byt_Data)
        /// <param name="byt_Data">返回帧</param>
        /// <returns></returns>
        private bool CheckFrame(byte[] byt_Value, ref byte[] byt_Data)
        {
            try
            {

                HexCon hexcon = new HexCon();
                string sRxFrame = hexcon.ByteToString(byt_Value);
                //"81 01 30 06 30 07 "

                if (byt_Value.Length < 6) //帧格式，至少5个字节
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

                if (int_Start + 4 >= byt_Value.Length)
                {
                    this.m_str_LostMessage = "没有返回数据!";
                    return false;                //没有帧长度字节
                }
                int int_Len = byt_Value[3];
                if (int_Len > byt_Value.Length)
                {
                    this.m_str_LostMessage = "实际长度与帧长度不相符!";
                    return false;           //实际长度与帧长度不相符
                }
                byte byt_ChkSum = 0;
                for (int int_Inc = 1; int_Inc < byt_Value.Length - 1; int_Inc++)
                {
                    byt_ChkSum ^= byt_Value[int_Start + int_Inc];
                }

                if (byt_ChkSum != byt_Value[byt_Value.Length - 1])
                {
                    this.m_str_LostMessage = "校验码不一致!";
                    return false; //校验码不正常
                }
                Array.Resize(ref byt_Data, int_Len);    //数据域长度
                Array.Copy(byt_Value, int_Start, byt_Data, 0, int_Len);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false; ;
            }
        }
        #endregion

        #region------未使用的 私有成员--Private----------------

        #region 命令控制码：0x64 PC控制接线方式：0-YPQ 1-PQ 2-Q90 3-Q3 4-Q60° 适用画面：功率测量、伏安测量、相频测量 SetConnection(int int_Type)
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
        #endregion

        #endregion ------未使用的 私有成员--Private----------------


        #region  //暂时屏蔽


        #region  组织帧 OrgFrame(byte byt_Cmd, byte[] byt_Data)
        ///// <param name="byt_Cmd">控制码</param>
        ///// <param name="byt_Data">数据域</param>
        ///// <returns>返回组好的帧</returns>
        //private byte[] OrgFrame(byte byt_Cmd, byte[] byt_Data)
        //{
        //    int iLen = 5 + byt_Data.Length;                   //81、ID、len、CMD和Chksum各占一位 +数据域位
        //    byte[] byt_Frame = new byte[iLen];
        //    try
        //    {
        //        byt_Frame[0] = 129;                                  //81
        //        byt_Frame[1] = Convert.ToByte(this.m_str_ID, 16);    //ID
        //        byt_Frame[2] = Convert.ToByte(iLen);                                 //Len
        //        byt_Frame[3] = byt_Cmd;    //Cmd

        //        if (byt_Data.Length > 0) Array.Copy(byt_Data, 0, byt_Frame, 4, byt_Data.Length);

        //        for (int int_Inc = 1; int_Inc < iLen - 1; int_Inc++)        //校验码    校验码从81后面开始计算
        //        {
        //            byt_Frame[iLen - 1] ^= byt_Frame[int_Inc];               //Chksum
        //        }
        //        return byt_Frame;
        //    }
        //    catch (Exception e)
        //    {
        //        this.m_str_LostMessage = e.ToString();
        //        return byt_Frame;
        //    }
        //}
        #endregion
        #endregion 暂时屏蔽
        #endregion ------私有成员 ----------------

        #region ------CCL3115 未使用到的接口成员------------------


        #region 命令控制码：0x60 联机命令 LCD复位，PC联机控制LCD   ExitLCDMenu()
        public bool ExitLCDMenu()
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                byte[] byt_SendData = this.OrgFrame(0x60);          //复位
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(900, 300);

                byte[] byt_RevData = new byte[0];

                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 75)           //返回4b，OK
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
        #endregion


     

        #region 命令控制码：0x20 读版本号   ReadVer(ref string str_Ver)
        public bool ReadVer(ref string str_Ver)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                byte[] byt_SendData = this.OrgFrame(0x20);
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);
                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[3] == 32)
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
        #endregion

        #region 命令控制码：0x32  读取标准输出档位 ReadOutRange
        /// <param name="int_UaRng">A相电压档位</param>
        /// <param name="int_UbRng">B相电压档位</param>
        /// <param name="int_UcRng">C相电压档位</param>
        /// <param name="int_IaRng">A相电流档位</param>
        /// <param name="int_IbRng">B相电流档位</param>
        /// <param name="int_IcRng">C相电流档位</param>
        /// <returns></returns>
        public bool ReadOutRange(ref int int_UaRng, ref int int_UbRng, ref int int_UcRng,
                          ref int int_IaRng, ref int int_IbRng, ref int int_IcRng)
        {

            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                byte[] byt_SendData = this.OrgFrame(0x32);
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1500, 400);
                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[3] == 0x32)           //返回的是32指令
                    {
                        //-------------解析数据---------------
                        int_UaRng = byt_RevData[8];
                        int_UbRng = byt_RevData[9];
                        int_UcRng = byt_RevData[10];
                        int_IaRng = byt_RevData[11];
                        int_IbRng = byt_RevData[12];
                        int_IcRng = byt_RevData[13];
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
        #endregion

        #region 命令控制码：0x31  读地址命令:CL311返回指定地址起的指定个WORD的数据(低字节在前,高字节在后) ReadHarmonious(int int_Type, ref string[] str_Para)
        public bool ReadHarmonious(int int_Type, ref string[] str_Para)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (int_Type >= 0 && int_Type <= 5)
                {
                    byte[] byt_Value;
                    if (int_Type == 0)
                        byt_Value = new byte[] { 0x32, 0x80, 0x09 };
                    else if (int_Type == 1)
                        byt_Value = new byte[] { 0x32, 0xa0, 0x09 };
                    else if (int_Type == 2)
                        byt_Value = new byte[] { 0x32, 0xc0, 0x09 };
                    else if (int_Type == 3)
                        byt_Value = new byte[] { 0x32, 0xe0, 0x09 };
                    else if (int_Type == 4)
                        byt_Value = new byte[] { 0x32, 0x00, 0x0a };
                    else
                        byt_Value = new byte[] { 0x32, 0x20, 0x0a };
                    if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                        this.m_Ispt_com.Setting = this.m_str_Setting;
                    if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                        this.m_Ispt_com.Channel = this.m_int_Channel;

                    byte[] byt_SendData = this.OrgFrame(0x31, byt_Value);
                    this.m_byt_RevData = new byte[0];
                    this.m_Ispt_com.SendData(byt_SendData);
                    Waiting(1500, 300);

                    byte[] byt_RevData = new byte[0];

                    if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                    {
                        if (byt_RevData[3] == 49)
                        {
                            //----------解析数据----------------------

                            if (byt_RevData[4] == 66)
                            {
                                this.m_str_LostMessage = "返回标准表正忙，无法响应读谐波！";
                                return false;
                            }


                            string[] str_MyData = new string[21];

                            byte[] byt_Tmp = new byte[] { 0, 0, 0, 0 };

                            for (int int_Inc = 0; int_Inc < 21; int_Inc++)
                            {
                                Array.Copy(byt_RevData, 5 + int_Inc * 2, byt_Tmp, 0, 2);

                                str_MyData[int_Inc] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / 100);
                            }
                            str_Para = str_MyData;
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
                else
                {
                    this.m_str_LostMessage = "读取谐波含量相别序号不正确！";
                    return false;
                }
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }
        #endregion

        //命令控制码： 0x62指令的类型 ，0=设置参数，1=启动，2=读数据
        #region   命令控制码：0x62, 0x01 电能表控制命令 序号0x01 启动命令  RunStdMeter()
        public bool RunStdMeter()
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                byte[] byt_SendData = this.OrgFrame(0x62, 0x01);
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);

                byte[] byt_RevData = new byte[0];

                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 75)           //返回4b，OK
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
        #endregion
        #region 命令控制码：0x62, 0x02 电能表控制命令 序号0x02 读误差及脉冲数 ReadErrorAndPulse(ref string[] sErr, ref long[] lTimes)
        public bool ReadErrorAndPulse(ref string[] sErr, ref long[] lTimes)
        {

            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                byte[] byt_SendData = this.OrgFrame(0x62, 0x02);
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);

                byte[] byt_RevData = new byte[0];

                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[3] == 98)
                    {
                        string[] str_Value = new string[8];
                        long[] lng_Times = new long[8];
                        byte[] byt_Tmp = new byte[] { 0, 0, 0, 0 };
                        int int_Start = BitConverter.ToInt16(byt_RevData, 4);
                        for (int int_Inc = 0; int_Inc < 8; int_Inc++)
                        {
                            Array.Copy(byt_RevData, 38 + int_Inc * 4, byt_Tmp, 0, 4);
                            if (byt_Tmp[0] == 0 && byt_Tmp[1] == 0 &&
                                byt_Tmp[2] == 0 && byt_Tmp[3] == 128)
                            {
                                str_Value[int_Inc] = "999.000";     //标准表没有计算
                            }
                            else
                                str_Value[int_Inc] = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / 1000);
                        }

                        for (int int_Inc = 0; int_Inc < 8; int_Inc++)
                        {
                            Array.Copy(byt_RevData, 6 + int_Inc * 4, byt_Tmp, 0, 4);
                            if (int_Start == 0)
                                lng_Times[int_Inc] = 0;
                            else
                                lng_Times[int_Inc] = BitConverter.ToInt32(byt_Tmp, 0);
                        }

                        sErr = str_Value;
                        lTimes = lng_Times;
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
        #endregion
        #region 命令控制码：0x62, byt_Value  SetAmMeterParameter(long lMeterConst, long lPulseTimes, int iLX, int iClfs)
        //lMeterConst 电表常数
        //lPulseTimes 圈数
        //iLX         表量限 （备用）
        //iClfs  测量方式：表类型   0-PT4 三相四线有功电能表    1-QT4三相四线真无功电能表   2-P32 三相三线有功电能表
        //                          3-Q32 三相三线真无功电能表  4-Q90二元件90°无功电能表   5-Q60 二元件60°无功电能表
        //                          6-Q33 三元件无功电能表
        public bool SetAmMeterParameter(long lMeterConst, long lPulseTimes, int iLX, int iClfs)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                byte[] byt_Value = new byte[11];
                byt_Value[0] = 0x00;        //62指令的类型 ，0=设置参数，1=启动，2=读数据
                Array.Copy(BitConverter.GetBytes(Convert.ToInt32(lMeterConst)), 0, byt_Value, 1, 4);
                Array.Copy(BitConverter.GetBytes(Convert.ToInt32(lPulseTimes)), 0, byt_Value, 5, 4);
                byt_Value[9] = (byte)iLX;
                byt_Value[10] = (byte)iClfs;
                byte[] byt_SendData = this.OrgFrame(0x62, byt_Value);
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(900, 300);
                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x4b)           //返回4b，OK
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
        #endregion

        #region 命令控制码： 0x44 模式：0：自动  1：手动  设置输出电能脉冲常数  SetStdMeterConst(long lStdConst, bool bAuto)
        public bool SetStdMeterConst(long lStdConst, bool bAuto)
        {
            try
            {

                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                byte[] byt_Value = new byte[5];
                byt_Value[0] = (byte)(bAuto ? 0 : 1);
                Array.Copy(BitConverter.GetBytes(Convert.ToInt32(lStdConst)), 0, byt_Value, 1, 4);
                byte[] byt_SendData = this.OrgFrame(0x44, byt_Value);
                this.m_byt_RevData = new byte[0];           //返回数据
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(900, 300);

                byte[] byt_RevData = new byte[0];

                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 75)
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
        #endregion

        #region 命令控制码：0x42  设置ABC电压测量档位,返回OK  SetUScale(int iUaScale, int iUbScale, int iUcScale)
        public bool SetUScale(int iUaScale, int iUbScale, int iUcScale)
        {
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                byte[] byt_Value = new byte[] { (byte)iUaScale, (byte)iUbScale, (byte)iUcScale };
                byte[] byt_SendData = this.OrgFrame(0x42, byt_Value);
                this.m_byt_RevData = new byte[0];           //返回数据
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(900, 300);
                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 75)
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
        public bool SetUScale(float sngUa, float sngUb, float sngUc)
        {
            int int_UaScale = GetUScaleIndex(sngUa);
            int int_UbScale = GetUScaleIndex(sngUb);
            int int_UcScale = GetUScaleIndex(sngUc);
            return SetUScale(int_UaScale, int_UbScale, int_UcScale);
        }
        #endregion

        #region 命令控制码：0x41 设置ABC电流测量档位,返回OK。 SetIScale(int iIaScale, int iIbScale, int iIcScale)
        public bool SetIScale(int iIaScale, int iIbScale, int iIcScale)
        {
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                byte[] byt_Value = new byte[] { (byte)iIaScale, (byte)iIbScale, (byte)iIcScale };

                byte[] byt_SendData = this.OrgFrame(0x41, byt_Value);
                this.m_byt_RevData = new byte[0];           //返回数据
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(900, 300);
                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 75)
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
        public bool SetIScale(float sngIa, float sngIb, float sngIc)
        {
            //电流档位,0=100A  1=50A  2=25A  3=10A 4=5A  5=2.5A  6=1A  7=0.5A 8=0.25A  9=0.1A  10=0.05A  11=0.025A  15=自动档
            int int_IaScale = GetIScaleIndex(sngIa);
            int int_IbScale = GetIScaleIndex(sngIb);
            int int_IcScale = GetIScaleIndex(sngIc);
            return SetIScale(int_IaScale, int_IbScale, int_IcScale);
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

        #endregion

        #region 命令控制码：0x35 读数据 0x12 输出电能脉冲常数 ReadStdMeterConst(ref long lngConst)
        public bool ReadStdMeterConst(ref long lngConst)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                byte[] byt_SendData = this.OrgFrame(0x35, 0x12);
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);

                byte[] byt_RevData = new byte[0];

                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[3] == 53)
                    {
                        //-----解析数据-----
                        lngConst = BitConverter.ToInt32(byt_RevData, 5);        //第五个字节开始，4字节计算
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
        #endregion

        #region 命令控制码：0x35 读数据 0x13	输出电能脉冲数 ReadStdMeterPulses(ref long lngPulse)
        public bool ReadStdMeterPulses(ref long lngPulse)
        {

            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                byte[] byt_SendData = this.OrgFrame(0x35, 0x13);
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(1200, 300);

                byte[] byt_RevData = new byte[0];

                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[3] == 53)
                    {
                        //-----解析数据-----
                        lngPulse = BitConverter.ToInt32(byt_RevData, 5);        //第五个字节开始，4字节计算
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
        #endregion

        #region 命令控制码：byt_Cmd, byt_Data   ExtendCommand(byte byt_Cmd, byte[] byt_Data, ref byte[] byt_RevData)
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




        #endregion


        public class HexCon
        {
            public byte[] ASCIIStringToByte(string InString)//将一组字符编码为一个字节序列。
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                return encoding.GetBytes(InString);
            }
            public string ASCIIByteToString(byte[] InBytes)// 将一个字节序列解码为一个字符串。 
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                return encoding.GetString(InBytes);
            }
            public string HexStringToASCIIString(string InString)// 将一个字节序列解码为一个字符串。 
            {
                if (InString == "") return "";
                byte[] Inbytestmp = StringToByte(InString);
                return ASCIIByteToString(Inbytestmp);
            }
            public string ASCIIStringToHexString(string InString)//  
            {
                if (InString == "") return "";
                byte[] Inbytestmp = ASCIIStringToByte(InString);
                return ByteToString(Inbytestmp);
            }

            //converter hex string to byte and byte to hex string
            public string ByteToString(byte[] InBytes)
            {
                string StringOut = "";

                foreach (byte InByte in InBytes)
                {
                    StringOut = StringOut + String.Format("{0:X2} ", InByte);
                }
                return StringOut;
            }
            public byte[] StringToByte(string InString)
            {
                string[] ByteStrings;
                int iLen;
                InString = InString.Trim();
                ByteStrings = InString.Split(" ".ToCharArray());
                byte[] ByteOut;
                ByteOut = new byte[ByteStrings.Length];
                iLen = ByteStrings.Length - 1;
                string hexstr;
                if (InString.Length <= 0) return ByteOut;
                for (int i = 0; i <= iLen; i++)
                {
                    hexstr = "0x" + ByteStrings[i];
                    ByteOut[i] = System.Convert.ToByte(hexstr, 16);

                }
                return ByteOut;
            }
        }
    }
}
