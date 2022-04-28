
/****************************************************************************

    DLT645协议类
    刘伟 2009-10

*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using pwInterface;
using System.Diagnostics;
using System.Collections;
using System.Threading;
using System.Windows.Forms;
namespace pwMeterProtocol
{


    /// <summary>
    /// 国产电能表645协议基类，适用1997、2007版本,均可继承于它
    /// </summary>
    public class PowerConsume : ProtocolBase, IMeterProtocol
    {
        private string m_str_RxID = "01";                   ///R——受信节点ID码 10H，地址根据表位来1,2,3,4,5,6...
        private string m_str_TxID = "01";                   ///T——发信节点ID码（地址码）  暂时赋值为 01H 、
        private int m_int_WaitDataRevTime = 2000;           ///等待数据到达最大时间ms
        private int m_int_IntervalTime = 500;               ///数据间隔最大时间ms
        private bool m_bol_ZendStringDel0x33 = false;		///发送接收的数据，数据域是否减0x33
        private byte m_byt_iRepeatTimes = 3;                ///通讯失败重试次数
        private bool m_bol_ClosComm = false;                ///通讯完成后是否关闭端口
        private bool m_bol_BreakDown = false;               ///被外部中断，主要用于下载打包参数时

        public PowerConsume()
        {
            this.m_byt_RevData = new byte[0];
        }


        #region---属性-------------------------
        //======================

        public string RxID
        {
            get
            {
                return this.m_str_RxID;
            }
            set
            {
                this.m_str_RxID = value;
            }
        }

        public string TxID
        {
            get
            {
                return this.m_str_TxID;
            }
            set
            {
                this.m_str_TxID = value;
            }
        }
 
        /// <summary>
        /// 等待数据到达最大时间
        /// </summary>
        public int WaitDataRevTime
        {
            get
            {
                return this.m_int_WaitDataRevTime;
            }
            set
            {
                this.m_int_WaitDataRevTime = value;
            }
        }

        /// <summary>
        /// 数据间隔最大时间
        /// </summary>
        public int IntervalTime
        {
            get
            {
                return this.m_int_IntervalTime;
            }
            set
            {
                this.m_int_IntervalTime = value;
            }
        }


        /// <summary>
        /// 发送接收的数据帧，数据域减0x33
        /// </summary>
        public bool ZendStringDel0x33
        {
            set
            {
                this.m_bol_ZendStringDel0x33 = value;
            }
        }

        /// <summary>
        /// 通讯失败重试次数
        /// </summary>
        public byte iRepeatTimes
        {
            set
            {
                this.m_byt_iRepeatTimes = value;
            }
        }


        /// <summary>
        /// 通讯完成后是否关闭端口
        /// </summary>
        public bool bClosComm
        {
            set
            {
                this.m_bol_ClosComm = value;
            }
        }

        /// <summary>
        /// 下载打包参数时是否被外部中断
        /// </summary>
        public bool BreakDown
        {
            set
            {
                this.m_bol_BreakDown = value;
            }
        }

        /// <summary>
        /// 操作失败信息
        /// </summary>
        public string LostMessage
        {
            get
            {
                return this.m_str_LostMessage;
            }
        }

        #endregion


        #region---公有-------------------------

        /// <summary>
        /// 读取功耗
        /// </summary>
        /// <param name="int_BwIndex">表位号</param>
        /// <param name="int_Chancel">通道</param>
        /// <param name="flt_U">电压有效值</param>
        /// <param name="flt_I">电流有效值</param>
        /// <param name="flt_ActiveP">有功功率</param>
        /// <param name="flt_ApparentP">视在功率</param>
        /// <returns></returns>
        public bool ReadPower2(int int_BwIndex, int int_Chancel, ref float flt_U, ref float flt_I, ref float flt_ActiveP, ref float flt_ApparentP)
        {
            //0x81 + 0x10 + 0x80 + 0x1B + 0xA0 + 0x04 +表位号+通道+CS
            //表位号：即需要读取功耗的表位号（数据类型UINT1）
            //ChannelSwitch （数据类型UINT1）：
            //低四位代表测试单元模块编号：
            //取值1，代表9~24字节发送到A相电压回路测试单元。也即：发往通道1。
            //取值2，代表9~24字节发送到A相电流回路测试单元。也即：发往通道2。
            //取值3，代表9~24字节发送到B相电压回路测试单元。也即：发往通道3。
            //取值4，代表9~24字节发送到B相电流回路测试单元。也即：发往通道4。
            //取值5，代表9~24字节发送到C相电压回路测试单元。也即：发往通道5。
            //取值6，代表9~24字节发送到C相电流回路测试单元。也即：发往通道6。
            //高四位置“1”即可


            this.m_str_LostMessage = "";
            byte[] byt_temp = new byte[4];
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;

                byte[] byt_Frame = ReadDataPage(0x04, (byte)int_BwIndex, (byte)int_Chancel);

                this.SendFrame(byt_Frame, 1200, 900);

                byte[] byt_Data = new byte[0];
                if (this.CheckFrameData(this.m_byt_RevData, 0x50, ref byt_Data))
                {

                    if (byt_Data[0] == (byte)int_BwIndex && byt_Data[1] == (byte)int_Chancel && byt_Data[2] == 0x01 && byt_Data[3] == 0x01)
                    {
                        Array.Copy(byt_Data, 4, byt_temp, 0, 4);
                        flt_U = BitConverter.ToSingle(byt_temp, 0);

                        Array.Copy(byt_Data, 8, byt_temp, 0, 4);
                        flt_I = BitConverter.ToSingle(byt_temp, 0);

                        Array.Copy(byt_Data, 12, byt_temp, 0, 4);
                        flt_ActiveP = BitConverter.ToSingle(byt_temp, 0);

                        Array.Copy(byt_Data, 16, byt_temp, 0, 4);
                        flt_ApparentP = BitConverter.ToSingle(byt_temp, 0);
                        return true;
                    }
                    else
                    {
                        this.m_str_LostMessage = "返回帧的Page、Group、Data与下发帧不一致";
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.Message;
                return false;
            }
        }

        /// <summary>
        /// 读取功耗
        /// </summary>
        /// <param name="iCType">iCType     测试档位    取值范围为1-15，对应15个不同的测试档位</param>
        /// <param name="iRType">iRType     测试类型    1，A相电压回路测试单元；2，A相电流回路测试单元；3，B相电压；4，B相电流；5，C相电压；6，C相电流</param>
        /// <param name="byt_RevData"></param>
        /// <param name="byt_SendFrame"></param>
        /// <returns></returns>
        public bool ReadPower(int iCType, int iRType, ref float flt_U, ref float flt_I, ref float flt_ActiveP, ref float flt_ApparentP)
        {
            this.m_str_LostMessage = "";
            byte[] byt_temp = new byte[4];
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                #region
                //H	 R	T	L	C	CLT1.0协议的数据体	CS
                //H——CLT1.0协议头。固定字节，值： 81H
                //R——受信节点ID码（地址码），模块ID始终是10H
                //T——发信节点ID码（地址码）
                //L——包含H和CS的所有数据字节数
                //CS——校验和，除去H和CS后的所有数据字节的按位异或值。
                //C——CLT1.0的协议控制字，0xA3代表写数据，0xA0代表查数据，0x50代表回答数据。

                //功耗查询命令
                //81	01	xx	19	A0	xx	01	01	18	FF	FF	FF	16	FF	FF	FF	3C	FF	FF	FF	3E	FF	FF	FF	CS
                //1	    2	3	4	5	6	7	8	9	10	11	12	13	14	15	16	17	18	19	20	21	22	23	24	25
                //H	    R	T	L	C	CLT1.0协议的数据体	                                                        CS
                //                      PG	GP	G0	电能表功耗测试模块的数据体
                //字节3是发信节点ID码。字节2是功耗测试板的ID码。
                //字节6的取值是：低4位取值：1~6；高4位取值1~15。



                ////'iRType     测试类型    1，A相电压回路测试单元；2，A相电流回路测试单元；3，B相电压；4，B相电流；5，C相电压；6，C相电流
                ////'iCType     测试档位    取值范围为1-15，对应15个不同的测试档位

                ////'''0x81 + 表位号 + 发信节点ID + 0x19 + 0xA0 + ChannelSwitch + 0x01 + 0x01 + 0x18FFFFFF16FFFFFF3CFFFFFF3EFFFFFF + ParityBit(校验位)
                ////'''表位号: 即需要读取功耗的表位号 (数据类型UINT1)
                ////'''ChannelSwitch (数据类型UINT1):
                ////'''低四位代表测试单元模块编号:
                ////'''取值1，代表9~24字节发送到A相电压回路测试单元。也即：发往通道1。
                ////'''取值2，代表9~24字节发送到A相电流回路测试单元。也即：发往通道2。
                ////'''取值3，代表9~24字节发送到B相电压回路测试单元。也即：发往通道3。
                ////'''取值4，代表9~24字节发送到B相电流回路测试单元。也即：发往通道4。
                ////'''取值5，代表9~24字节发送到C相电压回路测试单元。也即：发往通道5。
                ////'''取值6，代表9~24字节发送到C相电流回路测试单元。也即：发往通道6。
                ////'''高四位置"1"即可
                ////'''
                ////'''返回:
                ////'''0x81 + 表位号 + 发信节点id + 0x19 + 0x50 + ChannelSwitch + 0x01 +0x01 + 电压有效值+ 电流有效值 + 基波有功功率 + 基波无功功率 + ParityBit(校验位)
                ////'''
                ////'''表位号: 对应需要读取的表位 (数据类型UINT1)
                ////'''电压有效值：单位伏，32位浮点格式（四字节）
                ////'''电流有效值：单位安，32位浮点格式
                ////'''基波有功功率：单位瓦，32位浮点格式
                ////'''基波无功功率：单位乏，32位浮点格式
                #endregion

                byte[] byt_Value = new byte[19];
                string sChannelSwitch = iCType.ToString() + iRType.ToString();
                byt_Value[0] = Convert.ToByte(sChannelSwitch, 16);     // XX ---ChannelSwitch (数据类型UINT1):
                #region
                byt_Value[1] = 0x01;
                byt_Value[2] = 0x01;
                byt_Value[3] = 0x18;
                byt_Value[4] = 0xFF;
                byt_Value[5] = 0xFF;
                byt_Value[6] = 0xFF;
                byt_Value[7] = 0x16;
                byt_Value[8] = 0xFF;
                byt_Value[9] = 0xFF;
                byt_Value[10] = 0xFF;
                byt_Value[11] = 0x3C;
                byt_Value[12] = 0xFF;
                byt_Value[13] = 0xFF;
                byt_Value[14] = 0xFF;
                byt_Value[15] = 0x3E;
                byt_Value[16] = 0xFF;
                byt_Value[17] = 0xFF;
                byt_Value[18] = 0xFF;
                #endregion
                //C——CLT1.0的协议控制字，0xA3代表写数据，0xA0代表查数据，0x50代表回答数据。
                byte[] byt_SendData = this.OrgFrame(byt_Value, 0xA0);

                this.m_byt_RevData = new byte[0];
                this.SendFrame(byt_SendData, 1200, 900);

                byte[] byt_Data = new byte[0];
                string strflt = "";
                if (CheckFrame(this.m_byt_RevData, ref byt_Data))            //检查返回帧是否正常
                {
                    Array.Copy(byt_Data, 3, byt_temp, 0, 4);
                    strflt = HexCon.ByteToString(byt_temp).Replace(" ","");
                    flt_U = pw_SingleHex.pwHexToSingle(strflt);


                    Array.Copy(byt_Data, 7, byt_temp, 0, 4);
                    strflt = HexCon.ByteToString(byt_temp).Replace(" ", "");
                    flt_I = pw_SingleHex.pwHexToSingle(strflt);


                    Array.Copy(byt_Data, 11, byt_temp, 0, 4);
                    strflt = HexCon.ByteToString(byt_temp).Replace(" ", "");
                    flt_ActiveP = pw_SingleHex.pwHexToSingle(strflt);

                    Array.Copy(byt_Data, 15, byt_temp, 0, 4);
                    strflt = HexCon.ByteToString(byt_temp).Replace(" ", "");
                    flt_ApparentP = pw_SingleHex.pwHexToSingle(strflt);
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        public bool ReadPower(ref float flt_ActiveP)
        {
            float flt_U = 0f;
            float flt_I = 0f;
            //float flt_ActiveP = 0f;
            float flt_ApparentP = 0f;
            this.m_str_LostMessage = "";
            byte[] byt_temp = new byte[4];
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                #region
                //H	 R	T	L	C	CLT1.0协议的数据体	CS
                //H——CLT1.0协议头。固定字节，值： 81H
                //R——受信节点ID码（地址码），模块ID始终是10H
                //T——发信节点ID码（地址码）
                //L——包含H和CS的所有数据字节数
                //CS——校验和，除去H和CS后的所有数据字节的按位异或值。
                //C——CLT1.0的协议控制字，0xA3代表写数据，0xA0代表查数据，0x50代表回答数据。

                //功耗查询命令
                //81	01	xx	19	A0	xx	01	01	18	FF	FF	FF	16	FF	FF	FF	3C	FF	FF	FF	3E	FF	FF	FF	CS
                //1	    2	3	4	5	6	7	8	9	10	11	12	13	14	15	16	17	18	19	20	21	22	23	24	25
                //H	    R	T	L	C	CLT1.0协议的数据体	                                                        CS
                //                      PG	GP	G0	电能表功耗测试模块的数据体
                //字节3是发信节点ID码。字节2是功耗测试板的ID码。
                //字节6的取值是：低4位取值：1~6；高4位取值1~15。



                ////'iRType     测试类型    1，A相电压回路测试单元；2，A相电流回路测试单元；3，B相电压；4，B相电流；5，C相电压；6，C相电流
                ////'iCType     测试档位    取值范围为1-15，对应15个不同的测试档位

                ////'''0x81 + 表位号 + 发信节点ID + 0x19 + 0xA0 + ChannelSwitch + 0x01 + 0x01 + 0x18FFFFFF16FFFFFF3CFFFFFF3EFFFFFF + ParityBit(校验位)
                ////'''表位号: 即需要读取功耗的表位号 (数据类型UINT1)
                ////'''ChannelSwitch (数据类型UINT1):
                ////'''低四位代表测试单元模块编号:
                ////'''取值1，代表9~24字节发送到A相电压回路测试单元。也即：发往通道1。
                ////'''取值2，代表9~24字节发送到A相电流回路测试单元。也即：发往通道2。
                ////'''取值3，代表9~24字节发送到B相电压回路测试单元。也即：发往通道3。
                ////'''取值4，代表9~24字节发送到B相电流回路测试单元。也即：发往通道4。
                ////'''取值5，代表9~24字节发送到C相电压回路测试单元。也即：发往通道5。
                ////'''取值6，代表9~24字节发送到C相电流回路测试单元。也即：发往通道6。
                ////'''高四位置"1"即可
                ////'''
                ////'''返回:
                ////'''0x81 + 表位号 + 发信节点id + 0x19 + 0x50 + ChannelSwitch + 0x01 +0x01 + 电压有效值+ 电流有效值 + 基波有功功率 + 基波无功功率 + ParityBit(校验位)
                ////'''
                ////'''表位号: 对应需要读取的表位 (数据类型UINT1)
                ////'''电压有效值：单位伏，32位浮点格式（四字节）
                ////'''电流有效值：单位安，32位浮点格式
                ////'''基波有功功率：单位瓦，32位浮点格式
                ////'''基波无功功率：单位乏，32位浮点格式
                #endregion

                byte[] byt_Value = new byte[19];
                //string sChannelSwitch = iCType.ToString() + iRType.ToString();
                //byt_Value[0] = Convert.ToByte(sChannelSwitch, 16);     // XX ---ChannelSwitch (数据类型UINT1):
                #region
                byt_Value[0] = 0x11;
                byt_Value[1] = 0x01;
                byt_Value[2] = 0x01;
                byt_Value[3] = 0x18;
                byt_Value[4] = 0xFF;
                byt_Value[5] = 0xFF;
                byt_Value[6] = 0xFF;
                byt_Value[7] = 0x16;
                byt_Value[8] = 0xFF;
                byt_Value[9] = 0xFF;
                byt_Value[10] = 0xFF;
                byt_Value[11] = 0x3C;
                byt_Value[12] = 0xFF;
                byt_Value[13] = 0xFF;
                byt_Value[14] = 0xFF;
                byt_Value[15] = 0x3E;
                byt_Value[16] = 0xFF;
                byt_Value[17] = 0xFF;
                byt_Value[18] = 0xFF;
                #endregion
                //C——CLT1.0的协议控制字，0xA3代表写数据，0xA0代表查数据，0x50代表回答数据。
                byte[] byt_SendData = this.OrgFrame(byt_Value, 0xA0);

                this.m_byt_RevData = new byte[0];
                this.SendFrame(byt_SendData, 1200, 900);

                byte[] byt_Data = new byte[0];
                string strflt = "";
                if (CheckFrame(this.m_byt_RevData, ref byt_Data))            //检查返回帧是否正常
                {
                    //Array.Copy(byt_Data, 3, byt_temp, 0, 4);
                    //strflt = HexCon.ByteToString(byt_temp).Replace(" ", "");
                    //flt_U = pw_SingleHex.pwHexToSingle(strflt);


                    //Array.Copy(byt_Data, 7, byt_temp, 0, 4);
                    //strflt = HexCon.ByteToString(byt_temp).Replace(" ", "");
                    //flt_I = pw_SingleHex.pwHexToSingle(strflt);



                    Array.Copy(byt_Data, 11, byt_temp, 0, 4);
                    strflt = HexCon.ByteToString(byt_temp).Replace(" ", "");
                    flt_ActiveP = pw_SingleHex.pwHexToSingle(strflt);

                    //Array.Copy(byt_Data, 15, byt_temp, 0, 4);
                    //strflt = HexCon.ByteToString(byt_temp).Replace(" ", "");
                    //flt_ApparentP = pw_SingleHex.pwHexToSingle(strflt);
                    return true;
                }
                else
                {
                    flt_ActiveP = 0f;
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

        #region---私有-------------------------

        /// <summary>
        /// 读数据命令A0（只读一个Data）
        /// </summary>
        /// <param name="byt_Page">页号，取值范围0x00-0xFF</param>
        /// <param name="byt_Group">组控制字，</param>
        /// <param name="byt_Grps">各组数据项控制字，</param>
        /// <returns></returns>
        private byte[] ReadDataPage(byte byt_Page, byte byt_Group, byte byt_Grps)
        {
            //其中Page表示页号，取值范围00..ffH；Grp是Grp0..7的控制字，bit0..7分别表示Grp0..7是否存在。Grp0..7是分组控制字，bit0..7分别表示是否需要Data[0..7]，“0”表示无或不需要，“1”表示有或需要
            int int_Len = 3;
            byte[] byt_Data = new byte[int_Len];
            byt_Data[0] = byt_Page;
            byt_Data[1] = byt_Group;
            byt_Data[2] = byt_Grps;
            return OrgCLTFrame(0xA0, byt_Data);
        }

        /// <summary>
        /// 根据CLT组帧
        /// </summary>
        /// <param name="byt_Cmd">命令字</param>
        /// <param name="byt_Data">数据域</param>
        /// <returns></returns>
        private byte[] OrgCLTFrame(byte byt_Cmd, byte[] byt_Data)
        {
            int int_Len = byt_Data.Length + 6;  //81、sRxID、sTxID、len、CMD和Chksum各占一位 + 数据域
            byte[] byt_Frame = new byte[int_Len];
            //|帧起始标识|受信节点ID码|发信节点ID码|帧长|命令字节|数据|帧校验码|
            byt_Frame[0] = 0x81;
            byt_Frame[1] = Convert.ToByte(m_str_RxID);
            byt_Frame[2] = Convert.ToByte(m_str_TxID);
            byt_Frame[3] = Convert.ToByte(int_Len);
            byt_Frame[4] = byt_Cmd;
            if (byt_Data.Length > 0)
                Array.Copy(byt_Data, 0, byt_Frame, 5, byt_Data.Length);
            byte byt_ChkSum = 0;
            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)
            {
                byt_ChkSum ^= byt_Frame[int_Inc];
            }
            byt_Frame[int_Len - 1] = byt_ChkSum;
            return byt_Frame;
        }

        /// <summary>
        /// 检验返回帧格式
        /// </summary>
        /// <param name="byt_RevValue"></param>
        /// <param name="byt_RevCmd"></param>
        /// <param name="byt_Data"></param>
        /// <returns></returns>
        private bool CheckFrameData(byte[] byt_RevValue, byte byt_RevCmd, ref byte[] byt_Data)
        {

            try
            {
                if (byt_RevValue.Length <= 0)
                {
                    this.m_str_LostMessage = "没有收到返回帧！";
                    return false;
                }
                byte byt_Cmd = 0;

                // Console.WriteLine(BitConverter.ToString(byt_RevValue));

                if (CheckFrame(byt_RevValue, ref byt_Cmd, ref byt_Data))
                {
                    if (byt_Cmd == byt_RevCmd)
                        return true;
                    else if (byt_Cmd == 0x33)
                    {
                        this.m_str_LostMessage = "设备返回操作失败指令！";
                        return false;              //是否返回OK
                    }
                    else if (byt_Cmd == 0x35)
                    {
                        this.m_str_LostMessage = "设备返回正忙指令！请稍候再操作";
                        return false;              //是否返回OK
                    }
                    else if (byt_Cmd == 0x36)
                    {
                        this.m_str_LostMessage = "下发的指令是受限制指令！";
                        return false;              //是否返回OK
                    }
                    else
                    {
                        this.m_str_LostMessage = "返回指令不正确！";
                        return false;              //是否返回OK
                    }
                }
                this.m_str_LostMessage = "返回帧不符合要求！";
                return false;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.Message;
                return false;
            }

        }

        /// <summary>
        /// 验证帧是否符合要求
        /// </summary>
        /// <param name="byt_Value">需验证帧内容</param>
        /// <param name="byt_Cmd">返回帧控制码</param>
        /// <param name="byt_Data">返回帧数据域内容</param>
        /// <returns></returns>
        private bool CheckFrame(byte[] byt_Value, ref byte byt_Cmd, ref byte[] byt_Data)
        {
            int int_Start = 0;
            int_Start = Array.IndexOf(byt_Value, (byte)0x81);
            if (int_Start < 0 || int_Start > byt_Value.Length) return false;    //没有81开头
            if (int_Start + 3 >= byt_Value.Length) return false;                //没有帧长度字节
            int int_Len = byt_Value[int_Start + 3];
            if (int_Len + int_Start > byt_Value.Length) return false;           //实际长度与帧长度不相符

            byte byt_ChkSum = 0;
            for (int int_Inc = 0; int_Inc < int_Len - 1; int_Inc++)
            {
                byt_ChkSum ^= byt_Value[int_Start + int_Inc];
            }
            if (byt_ChkSum != byt_Value[int_Start + int_Len - 1]) return false; //校验码不正常


            byt_Cmd = byt_Value[int_Start + 4];     //控制码
            Array.Resize(ref byt_Data, int_Len - 6);    //数据域长度
            Array.Copy(byt_Value, int_Start + 5, byt_Data, 0, int_Len - 6);
            return true;
        }


        private byte[] OrgFrame(byte[] byt_Value, byte byt_Cmd)
        {

            //--帧格式:-----------------------------------------------------------------------------
            //-----|帧起始标识|受信节点ID码|发信节点ID码|帧长|命令字节|数据|帧校验码|
            //--------------------------------------------------------------------------------------
            //ILen = 6  '81、sRxID、sTxID、len、CMD和Chksum各占一位                    '在没有数据块的情况下至少的长度

            //H	 R	T	L	C	CLT1.0协议的数据体	CS
            //H——CLT1.0协议头。固定字节，值： 81H
            //R——受信节点ID码（地址码），模块ID始终是10H
            //T——发信节点ID码（地址码）
            //L——包含H和CS的所有数据字节数
            //CS——校验和，除去H和CS后的所有数据字节的按位异或值。
            //C——CLT1.0的协议控制字，0xA3代表写数据，0xA0代表查数据，0x50代表回答数据。
            //功耗查询命令
            //81	01	xx	19	A0	xx	01	01	18	FF	FF	FF	16	FF	FF	FF	3C	FF	FF	FF	3E	FF	FF	FF	CS
            //1	    2	3	4	5	6	7	8	9	10	11	12	13	14	15	16	17	18	19	20	21	22	23	24	25
            //H	    R	T	L	C	CLT1.0协议的数据体	                                                        CS
            //                      PG	GP	G0	电能表功耗测试模块的数据体

            int int_Len = 6 + byt_Value.Length;                 //81、RxID、TxID、len、CMD和Chksum各占一位
            byte[] byt_Data = new byte[int_Len];
            byt_Data[0] = 0x81;                                 //81
            byt_Data[1] = Convert.ToByte(this.m_str_RxID, 16);  //R——受信节点ID码（地址码），模块ID始终是10H
            byt_Data[2] = Convert.ToByte(this.m_str_TxID, 16);  //T——发信节点ID码（地址码）
            byt_Data[3] = (byte)int_Len;                              //帧长--L——包含H和CS的所有数据字节数
            byt_Data[4] = byt_Cmd;    //Cmd

            for (int i = 0; i < byt_Value.Length; i++)              //data
            {
                byt_Data[5 + i] = byt_Value[i];
            }

            for (int int_Inc = 1; int_Inc < (byt_Data.Length - 1); int_Inc++)
            {
                byt_Data[byt_Data.Length - 1] ^= byt_Data[int_Inc];               //Chksum
            }
            return byt_Data;


        }
        private bool CheckFrame(byte[] byt_Value, ref byte[] byt_Data)
        {

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

            if (int_Start + 3 >= byt_Value.Length)
            {
                this.m_str_LostMessage = "没有返回数据!";
                return false;                //没有帧长度字节
            }

            byte byt_Tmp = byt_Value[int_Start + 3];
            ////Array.Copy(byt_Value, int_Start + 3, byt_Tmp, 0, 1);
            ////Array.Reverse(byt_Tmp);             //由于高位在前，低位在后，所以要对调
            int int_Len = byt_Tmp;
            if (int_Len + int_Start > byt_Value.Length)
            {
                this.m_str_LostMessage = "实际长度与帧长度不相符!";
                return false;           //实际长度与帧长度不相符
            }
            byte byt_ChkSum = 0;
            for (int int_Inc = 0; int_Inc < int_Len - 1; int_Inc++)
            {
                byt_ChkSum ^= byt_Value[int_Start + int_Inc];
            }
            if (byt_ChkSum != byt_Value[int_Start + int_Len - 1])
            {
                this.m_str_LostMessage = "校验码不一致!";
                return false; //校验码不正常
            }
            Array.Resize(ref byt_Data, int_Len-6);    //数据域长度
            Array.Copy(byt_Value, int_Start+5, byt_Data, 0, int_Len-6);
            return true;
        }

        #endregion



        #region---空属性-------------------------

        private string m_str_Address = "000000000000";        ///表地址
        private string m_str_Password = "000000";             ///表密码
        private byte m_byt_PasswordClass = 0;                 ///表密码等级
        private string m_str_UserCode = "000000";             ///操作员代码
        private int m_int_PasswordType = 1;                   ///密码验证类型，0＝无密码认证 1＝密码放在数据域中方式
        private bool m_bol_DataFieldPassword = false;         ///写操作时，数据域是否包含写密码,true=要，false=不用
        //===================


        /// <summary>
        /// 表地址
        /// </summary>
        public string Address
        {
            get
            {
                return this.m_str_Address;
            }
            set
            {
                this.m_str_Address = value;
            }
        }

        /// <summary>
        /// 写操作密码
        /// </summary>
        public string Password
        {
            get
            {
                return this.m_str_Password;
            }
            set
            {
                this.m_str_Password = value;
            }
        }

        /// <summary>
        /// 密码等级
        /// </summary>
        public byte PasswordClass
        {
            get
            {
                return this.m_byt_PasswordClass;
            }
            set
            {
                this.m_byt_PasswordClass = value;
            }
        }

        /// <summary>
        /// 操作员代码
        /// </summary>
        public string UserID
        {
            get
            {
                return this.m_str_UserCode;
            }
            set
            {
                this.m_str_UserCode = value;
            }
        }

        /// <summary>
        /// 密码验证类型，0＝无密码认证 1＝密码放在数据域中方式 2＝A型表密码认证方式 3＝B型表密码认证方式
        /// </summary>
        public int VerifyPasswordType
        {
            get
            {
                return this.m_int_PasswordType;
            }
            set
            {
                this.m_int_PasswordType = value;
            }
        }

        /// <summary>
        /// 写操作时，数据域是否包含写密码,true=要，false=不用
        /// </summary>
        public bool DataFieldPassword
        {
            get
            {
                return this.m_bol_DataFieldPassword;
            }
            set
            {
                this.m_bol_DataFieldPassword = value;
            }
        }

        #endregion

        #region ---空接口-------------------------

        #region 基本方法
        public bool SendDLT645Command(byte bCmd, string cAddr, byte bDataLen, ref string cData, ref bool bExtend)
        {	/// cCmd 命令字  cAddr 地址 bLen 数据长度 cData 数据域数据 bExtend 是否有后续数据

            cData = "";
            bExtend = false;
            return false;

        }


        public bool SendDLT645Command(byte bCmd, string cAddr, byte bDataLen, ref string cData, ref byte[] byt_RevDataF, ref bool bExtend)
        {	/// cCmd 命令字  cAddr 地址 bLen 数据长度 cData 数据域数据 bExtend 是否有后续数据
            byt_RevDataF = new byte[0];
            cData = "";
            bExtend = false;
            return false;


        }


        
        public bool SendDLT645RxFrame(string RxFrame, ref string TxFrame, ref string cData)
        {	/// RxFrame 发送帧  TxFrame 接收帧 cData 接收数据域
            TxFrame="";
            cData = "";
            return false;

        }

        public bool DownPara(List<MeterDownParaItem> _DownParaItemOne)
        {
                return false;
        }


        public bool ReadData(string str_OBIS, int int_Len, ref string str_Value)
        {
            str_Value = "";
            return false ;

        }

        public bool ReadData(string str_OBIS, int int_Len, int int_Dot, ref Single sng_Value)
        {
            sng_Value = 0;
            return false;
        }


        public bool WriteData(string str_OBIS, int int_Len, string str_Value)
        {
            return false ;

        }

        public bool WriteData(byte byte_Cmd, int int_Len, string str_Value)
        {
            return false;

        }


        public bool WriteData(string str_OBIS, int int_Len, int int_Dot, Single sng_Value)
        {
            return false ;

        }

        public bool ReadMeterAddress(ref string str_Value)
        {
            str_Value = "";
            return false;
        }

        public bool WriteMeterAddress(string str_Value)
        {
            return false;
        }

        public bool MakeFrame_Measure_Calibrate(string Addr, int i_APCU, ArrayList ArrayList_APDU, ref ArrayList fpByteList, ref string str_LostMessage)
        {
            str_LostMessage = "";
            ArrayList_APDU.Clear();
            fpByteList.Clear();
            return false ;
        }
        public bool SendAdjustData(int i_APCU, ArrayList ArrayList_APDU)
        {
            ArrayList arlist = new ArrayList();
            ArrayList_APDU.Clear();
            return false;
        }

        #endregion

        #region 应用方法
        public bool ReadScbh(ref string str_Value)
        {

            return false;
        }


        public bool SelfCheck(int M1, int M2, ref string str_Value)
        {

                str_Value = "";
                return false;
        }

        public bool ReadEnergy(ref string str_Value)
        {
            str_Value = "";
            return false;

        }

        public bool ReadSinglePhaseTest(ref string str_Value)
        {
            str_Value = "";
            return false;
        }

        public bool ReadACSamplingTest(ref string str_Value)
        {
            str_Value = "";
            return false;
        }

        public bool ReadVer(ref string str_Value)
        {
            str_Value = "";
            return false;

        }

        public bool InitMeterPara(string str_Value)
        {
            return false;
        }

        public bool SysClear()
        {
            return false;
        }

        public bool HighFrequencyPulse(int BS)
        {
            return false;
        }

        /// <summary>
        /// 反转字节字符串
        /// </summary>
        /// <param name="sData"></param>
        /// <returns></returns>
        public string BackString(string sData)
        {		//字符重新排序
            int ilen = sData.Length;
            string stemp = "";
            if (ilen <= 0) return "";
            if (ilen % 2 != 0) return "";
            for (int tn = 0; tn < ilen / 2; tn++)
            {
                stemp = sData.Substring(2 * tn, 2) + stemp;
            }
            return stemp;
        }


        #endregion


        #endregion



    }

}
