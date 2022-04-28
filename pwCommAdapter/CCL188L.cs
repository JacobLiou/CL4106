/****************************************************************************

    CL188L误差计算板控制
    刘伟 2012-05

*******************************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using pwInterface;
using System.Diagnostics;
namespace pwErrorCalculate
{ 
    public class CCL188L: IErrorCalculate
    {

        private string m_str_ID = "40";         //误差板ID
        private int m_int_ListCount = 0;        //串在一起的误差板个数,即本总线有几个误差板
        private int[] m_int_ListTable;          //误差板列表
        private ISerialport m_Ispt_com;         //些总线的控制端口
        private string m_str_BwSelect = "000000000000000000000000000000000000000000000000"+ "111111111111111111111111111111111111111111111111"   ;//低位－>高位，1表示有效，要检
        private int m_int_Bws = 24;


        private string m_str_LostMessage = "";  //失败提示信息
        private byte[] m_byt_RevData;           //返回数据
        private bool m_bln_Enabled = true;      //当前状态
        private int m_int_Channel = 1;          //通道
        private string m_str_Setting = "38400,n,8,1";

        public CCL188L()
        {

        }
        public CCL188L(int int_bws)
        {
            m_int_Bws = int_bws;
        }

        #region IErrorCalculate 成员
        
        #region 基本成员
        /// <summary>
        /// 误差计算板地址
        /// </summary>
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


        /// <summary>
        /// 失败信息
        /// </summary>
        public string LostMessage
        {
            get
            {
                return this.m_str_LostMessage;
            }
        }

        /// <summary>
        /// 误差计算板串口
        /// </summary>
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


        /// <summary>
        /// 设置误差板表位号
        /// </summary>
        /// <param name="int_List">此管理的表位误差板列表</param>
        /// <returns></returns>
        public bool SetECListTable(int[] int_List)
        {
            try
            {
                this.m_int_ListTable = int_List;
                Array.Sort(this.m_int_ListTable);
                this.m_int_ListCount = int_List.Length;
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        public int AdaptCom(ISerialport[] mySerialPort)
        {
            throw new Exception("The method or operation is not implemented.");


        }

        public bool Link(ref bool[] bln_Result)
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

                if (this.m_int_ListTable == null || this.m_int_ListCount <= 0)
                {
                    this.m_str_LostMessage = "此误差板总线上没有表位列表！";
                    return false;
                }

                bool bln_IsOK = false;       //只要有一个表OK就OK
                for (int int_Inc = 0; int_Inc < this.m_int_ListCount; int_Inc++)
                {
                    bln_Result[this.m_int_ListTable[int_Inc] - 1] = this.Link(this.m_int_ListTable[int_Inc]);
                    if (bln_Result[this.m_int_ListTable[int_Inc] - 1])
                    {
                        bln_IsOK = true;
                        break;
                    }
                }
                return bln_IsOK;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        public bool Link(int int_Num)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;

                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                if (Array.IndexOf(this.m_int_ListTable, int_Num) == -1)
                {
                    this.m_str_LostMessage = "此表位号不在总线上！";
                    return false;
                }

                byte[] byt_SendData = this.OrgFrame(0xC0, 0x00, Convert.ToByte(int_Num));//通过读误差板的误差数据从而判断是否有返回
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(400, 200);
                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    if (byt_RevData[4] == 0x50)//等于0x50

                        return true;
                    else
                    {
                        this.m_str_LostMessage = "返回失败指令！";
                        return false ;
                    }
                }
                else
                    return false ;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }
        #endregion

        #region F1H：	设置电能误差检定时脉冲参数：被检表常数、圈数、标准表当前常数、标准表当前功率、被检表脉冲放大系数
        /// <summary>
        /// 统一设置误差板参数
        /// </summary>
        /// <param name="lAmMeterPulseConst">被检表常数</param>
        /// <param name="iPulseCount">被检表圈数</param>
        /// <param name="lStdPulseConst">标准表当前常数，需要从标准表读取</param>
        /// <param name="fStdP">标准表当前功率，需要从标准表读取</param>
        /// <param name="iAmMeterPulseBS">被检表脉冲放大系数（-128~127），默认为1不放大也不缩小1</param>
        /// <returns></returns>
        public bool SetDnWcrPara(long lAmMeterPulseConst, long iPulseCount, long lStdPulseConst, float fStrandMeterP, byte iAmMeterPulseBS)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                return SetDnWcrPara(255, lAmMeterPulseConst, iPulseCount, lStdPulseConst, fStrandMeterP, iAmMeterPulseBS);

            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 设置电能表常数、圈数
        /// </summary>
        /// <param name="int_ChNo">表位号：广播标志(0xFFH)，偶位(0xEEH)，奇位(0xDDH)</param>
        /// <param name="lng_PulseConst1">被检表常数</param>
        /// <param name="lng_PulseQs">误差检定圈数</param>
        /// <param name="lStdPulseConst">标准表当前常数，需要从标准表读取</param>
        /// <param name="fStdP">标准表当前功率，需要从标准表读取</param>
        /// <param name="iAmMeterPulseBS">被检表脉冲放大系数（-128~127），默认为1不放大也不缩小1</param>
        /// <returns></returns>
        public bool SetDnWcrPara(int int_ChNo, long lng_PulseConst1, long lng_PulseQs, long lStdPulseConst, float fStrandMeterP, byte iAmMeterPulseBS)
        {
            //Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 标准脉冲常数（4Bytes）+ 标准脉冲频率（4Bytes）+ 标准脉冲常数缩放倍数（1Bytes）+ 被检脉冲常数（4Bytes） + 校验圈数（4Bytes）+ 被检脉冲常数缩放倍数(1Byte)+发送标志1（1Byte）
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                long lngtmpStdPulsePl =  GetStdPulsePl(lStdPulseConst, fStrandMeterP);//可否用固定值？经测试这个参数好像没有作用1000;//

                byte[] byt_Value = new byte[19];
                byte[] byt_Tmp = BitConverter.GetBytes(Convert.ToInt64(lStdPulseConst));//标准脉冲常数

                Array.Copy(byt_Tmp, 0, byt_Value, 0, 4);

                byt_Tmp = BitConverter.GetBytes(Convert.ToInt64(lngtmpStdPulsePl));//标准脉冲频率
                Array.Copy(byt_Tmp, 0, byt_Value, 4, 4);

                byt_Value[8] = 0x01;//放大系数

                byt_Tmp = BitConverter.GetBytes(Convert.ToInt32(lng_PulseConst1));//被检表脉冲
                Array.Copy(byt_Tmp, 0, byt_Value, 9, 4);


                byt_Tmp = BitConverter.GetBytes(Convert.ToInt32(lng_PulseQs));//校验圈数
                Array.Copy(byt_Tmp, 0, byt_Value, 13, 4);

                byt_Value[17] = iAmMeterPulseBS;//放大系数

                byt_Value[18] = 0xAA;//发送标志
                byte[] byt_SendData;

                if (int_ChNo == 255)
                {
                    byt_SendData = this.OrgFrame(0xF1, byt_Value);    //统一设置
                }
                else
                {
                    byt_SendData = this.OrgFrame(0xF1, byt_Value, Convert.ToByte(int_ChNo));    //单个设置误差板
                }

                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(300, 200);
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(300, 200);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }
        #endregion


        #region F3H：	设置日计时误差检定时钟频率及需量周期误差检定时间
        /// <summary>
        /// 设置日计时误差检定时钟频率，圈数
        /// </summary>
        /// <param name="flt_MeterHz">被检时钟频率</param>
        /// <param name="int_Pulse">被检脉冲个数</param>
        /// <returns></returns>
        public bool SetTimePara(float flt_MeterHz,int int_Pulse)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                if (this.m_int_ListTable == null || this.m_int_ListCount <= 0)
                {
                    this.m_str_LostMessage = "此误差板总线上没有表位列表！";
                    return false;
                }
                return this.SetTimePara(255,500000, flt_MeterHz, int_Pulse);
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 设置日计时误差检定时钟频率，圈数
        /// </summary>
        /// <param name="flt_MeterHz">被检时钟频率</param>
        /// <param name="int_Pulse">被检脉冲个数</param>
        /// <returns></returns>
        public bool SetTimePara(float[] flt_MeterHz,int[] int_Pulse)
        {
            //Data的组织方式为：广播标志(0XFF) +（1 byte ListLen） + （12 bytes List） + 标准时钟频率100倍（4Bytes）+ 被检时钟频率100倍（4Bytes）+ 被检脉冲个数（4Bytes）+发送标志2（1Byte）
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                if (this.m_int_ListTable == null || this.m_int_ListCount <= 0)
                {
                    this.m_str_LostMessage = "此误差板总线上没有表位列表！";
                    return false;
                }
                bool bln_AllSame = false;
                int[] int_Tmp = int_Pulse;
                Array.Sort(int_Tmp);
                if (int_Tmp[0] == int_Tmp[int_Tmp.Length - 1])          //判断是否相等
                {
                    float[] flt_Tmp = flt_MeterHz;
                    Array.Sort(flt_Tmp);
                    if (flt_Tmp[0] == flt_Tmp[flt_Tmp.Length - 1])
                        bln_AllSame = true;
                }

                if (bln_AllSame)
                    return this.SetTimePara(255,500000, flt_MeterHz[0],int_Pulse[0]);
                else
                {
                    for (int int_Int = 0; int_Int < this.m_int_ListCount; int_Int++)
                    {
                        this.SetTimePara(flt_MeterHz[this.m_int_ListTable[int_Int] - 1],int_Pulse[this.m_int_ListTable[int_Int] - 1]);
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 日计时误差参数设置：表位号，常数，频率，圈数
        /// </summary>
        /// <param name="int_Num">表位号：广播标志(0xFFH)，偶位(0xEEH)，奇位(0xDDH)</param>
        /// <param name="flt_MeterHz">标准时钟脉冲常数</param>
        /// <param name="flt_MeterHz">被检时钟频率</param>
        /// <param name="int_Pulse">被检脉冲个数</param>
        /// <returns></returns>
        public bool SetTimePara(int int_Num,long lStdTimeConst, float flt_MeterHz, int int_Pulse)
        {

            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                if (Array.IndexOf(this.m_int_ListTable, int_Num) == -1 && int_Num != 255)
                {
                    this.m_str_LostMessage = "此表位号不在总线上！";
                    return false;
                }



                //byte[] byt_Value = new byte[14]; ; //数据域
                ////frameData = new byte[0];
                ////InitDataRegion(out dataRegion, 28);
                ////标准时钟频率
                //Array.Copy(BitConverter.GetBytes((uint)(500000 * 100)), 0, byt_Value, 0, 4);
                ////被检时钟频率
                //Array.Copy(BitConverter.GetBytes((uint)(1 * 100)), 0, byt_Value, 4, 4);
                ////被检脉冲个数
                //Array.Copy(BitConverter.GetBytes(int_Pulse/10), 0, byt_Value, 8, 4);
                //byt_Value[12] = 0xFF;
                //byt_Value[13] = 0xFF;
                //标准时钟频率100倍（4Bytes）
                //+ 被检时钟频率100倍（4Bytes）
                // + 被检脉冲个数（4Bytes）
                // +发送标志2（1Byte）

                byte[] byt_Value = new byte[13];
                byte[] byt_Tmp;
                byt_Tmp = BitConverter.GetBytes(Convert.ToInt32(lStdTimeConst * 100));//标准时钟频率100倍
                Array.Copy(byt_Tmp, 0, byt_Value, 0, 4);

                byt_Tmp = BitConverter.GetBytes(Convert.ToInt32(flt_MeterHz * 100));//被检时钟频率100倍（4Bytes）
                Array.Copy(byt_Tmp, 0, byt_Value, 4, 4);

                byt_Tmp = BitConverter.GetBytes(Convert.ToInt32(int_Pulse));//被检脉冲个数
                Array.Copy(byt_Tmp, 0, byt_Value, 8, 4);

                byt_Value[12] = 0x55;//发送标志


                byte[] byt_SendData;

                if (int_Num == 255)
                {

                    byt_SendData = this.OrgFrameForTimePulse(0xF3, byt_Value); //41H

                }
                else
                {
                    byt_SendData = this.OrgFrame(0xF3, byt_Value, Convert.ToByte(int_Num)); //41H

                }

                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(300, 300);
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(300, 300);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        #endregion


        #region A7H：	选择被检脉冲通道及检定类型
        /// <summary>
        /// 设置脉冲通道,0=P+,1=P-,2=Q+,3=Q-,4=需量,5=时钟；　　脉冲类型,0=共阳,1=共阴；　　通道类型,0=脉冲盒,1=光电头
        /// </summary>
        /// <param name="iPulseChannel">脉冲通道,0=P+,1=P-,2=Q+,3=Q-,4=需量,5=时钟</param>
        /// <param name="iChannelType">通道类型,0=脉冲盒,1=光电头</param>
        /// <param name="iPulseType">脉冲类型,0=共阴,1=共阳</param>
        /// <returns></returns>
        public bool SelectPulseChannel(int iPulseChannel, int iChannelType, int iPulseType)
        {
            //Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 被检脉冲通道号（2Byte）+ 检定类型（1Byte）
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_int_ListTable == null || this.m_int_ListCount <= 0)
                {
                    this.m_str_LostMessage = "此误差板总线上没有表位列表！";
                    return false;
                }
                return this.SelectPulseChannel(255, iPulseChannel, iChannelType, iPulseType);

            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }


        }

        /// <summary>
        /// 设置脉冲通道,0=P+,1=P-,2=Q+,3=Q-,4=需量,5=时钟；　　脉冲类型,0=共阳,1=共阴；　　通道类型,0=脉冲盒,1=光电头
        /// </summary>
        /// <param name="iPulseChannel">脉冲通道,0=P+,1=P-,2=Q+,3=Q-,4=需量,5=时钟</param>
        /// <param name="iChannelType">通道类型,0=脉冲盒,1=光电头</param>
        /// <param name="iPulseType">脉冲类型,0=共阴,1=共阳</param>
        /// <returns></returns>
        public bool SelectPulseChannel(int[] iPulseChannel, int[] iChannelType, int[] iPulseType)
        {
            //Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 被检脉冲通道号（2Byte）+ 检定类型（1Byte）
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_int_ListTable == null || this.m_int_ListCount <= 0)
                {
                    this.m_str_LostMessage = "此误差板总线上没有表位列表！";
                    return false;
                }

                bool bln_AllSame = false;
                int[] int_Tmp = iPulseType;
                Array.Sort(int_Tmp);                    //排序，
                if (int_Tmp[0] == int_Tmp[int_Tmp.Length - 1])  //第一个跟最后一个如果是一样的话，则全一样
                {
                    int_Tmp = iChannelType;
                    Array.Sort(int_Tmp);
                    if (int_Tmp[0] == int_Tmp[int_Tmp.Length - 1])   //第一个跟最后一个如果是一样的话，则全一样
                    {
                        int_Tmp = iPulseChannel;
                        Array.Sort(int_Tmp);
                        if (int_Tmp[0] == int_Tmp[int_Tmp.Length - 1])   //第一个跟最后一个如果是一样的话，则全一样
                            bln_AllSame = true;
                    }
                }
                if (bln_AllSame)
                    return this.SelectPulseChannel(255, iPulseChannel[0], iChannelType[0], iPulseType[0]);
                else
                {
                    for (int int_Int = 0; int_Int < this.m_int_ListCount; int_Int++)
                    {
                        this.SelectPulseChannel(this.m_int_ListTable[int_Int], iPulseChannel[this.m_int_ListTable[int_Int] - 1], iChannelType[this.m_int_ListTable[int_Int] - 1], iPulseType[this.m_int_ListTable[int_Int] - 1]);
                    }
                    return true;
                }

            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }


        }

        /// <summary>
        /// 设置脉冲通道
        /// </summary>
        /// <param name="iNum">表位号</param>
        /// <param name="iPulseChannel">脉冲通道,0=P+,1=P-,2=Q+,3=Q-,4=需量,5=时钟</param>
        /// <param name="iChannelType">通道类型,0=脉冲盒,1=光电头</param>
        /// <param name="iPulseType">脉冲类型,0=共阴,1=共阳</param>
        /// <returns></returns>
        public bool SelectPulseChannel(int iNum, int iPulseChannel, int iChannelType, int iPulseType)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                if (Array.IndexOf(this.m_int_ListTable, iNum) == -1 && iNum != 255)
                {
                    this.m_str_LostMessage = "此表位号不在总线上！";
                    return false;
                }
                if (iPulseChannel < 0 || iPulseChannel > 5 ||
                    iChannelType < 0 || iChannelType > 1 ||
                    iPulseType < 0 || iPulseType > 1)
                {
                    this.m_str_LostMessage = "设置参数不对！";
                    return false;
                }
                byte[] byt_Value = new byte[3];



                #region byt_Value[0]中 Bit0、Bit1、Bit2表示电能误差通道号，Bit2Bit1Bit0值与被选通道关系为：0P+ 、1P-、 2Q+、 3Q-、；
                //Bit2Bit1Bit0值与被选通道关系为：0P+ 、1P-、 2Q+、 3Q-、；
                //Bit3表示光电头选择位，1为感应式脉冲输入，0为电子式脉冲输入；
                //Bit4为脉冲极性选择，Bit4为0表示公共端输出低电平（共阴），Bit4为1表示公共端输出高电平（共阳）。
                if (iPulseChannel > 3)//5=时钟脉冲，4=需量周期脉冲
                {
                    byt_Value[0] = Convert.ToByte( iChannelType * 8 + iPulseType * 16);//电子\机械 ； 共阴\共阳，
                }
                else//电能脉冲P+,P-,Q+,Q-
                {
                    byt_Value[0] = Convert.ToByte(iPulseChannel + iChannelType  * 8 + iPulseType * 16);
                }
                #endregion

                #region byt_Value[1]中的 Bit1Bit0值表示多功能误差通道号：1为日计时脉冲、2为需量脉冲
                if (iPulseChannel == 5)
                {
                    byt_Value[1] = 0x01;//1为日计时脉冲
                }
                else if (iPulseChannel == 4)
                {
                    byt_Value[1] = 0x02;//2为需量脉冲。
                }
                else
                {
                    byt_Value[1] = 0x00;//待扩展
                }

                #endregion

                #region byt_Value[2] 检定任务类型：0x00电能误差、0x01需量误差、0x02日计时误差、0x03走字计数、0x04对标、0x05预付费功能检定
                if (iPulseChannel == 5)
                {
                    byt_Value[2] = 0x02;// 时钟日误差 = 2,
                }
                else if (iPulseChannel == 4)
                {
                    byt_Value[2] = 0x01;//需量周期 = 1,  
                }
                else //if (iPulseChannel < 4)
                {
                    byt_Value[2] = 0x00; 
                }
                #endregion

                #region 下发帧
                byte[] byt_SendData;
                if (iNum == 255)
                {
                    byt_SendData = this.OrgFrame(0xA7, byt_Value);  //总线上全表位命令
                }
                else
                {
                    //单表位命令
                    byt_SendData = this.OrgFrame(0xA7, byt_Value, Convert.ToByte(iNum));
                }

                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(200, 200);
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(200, 200);
                return true;
                #endregion
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }
        #endregion

        #region ADH：	选择被表脉冲端子类型
        /// <summary>
        /// 选择被表脉冲端子类型：0=国网端子，1=南网端子
        /// </summary>
        /// <param name="iPulseDzType">0=国网端子，1=南网端子</param>
        /// <returns></returns>
        public bool SetMeterPulseDzType(int iNum, int iPulseDzType)
        {
            //Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 端子类型（1Byte）
            //脉冲端子类型： 0x00表示国网端子；0X01表示南网端子；
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                byte bytCmd;

                bytCmd = Convert.ToByte(iPulseDzType);

                byte[] byt_SendData = this.OrgFrameForComm(0xAD, Convert.ToByte(iNum), bytCmd);

                this.m_byt_RevData = new byte[0];

                this.m_Ispt_com.SendData(byt_SendData);

                Waiting(300, 300);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }

        }

        public bool SetMeterPulseDzType(int iPulseDzType)
        {
            return SetMeterPulseDzType(255, iPulseDzType);
        }

        #endregion

        #region ACH：	通讯选择
        /// <summary>
        /// 控制表位通信口开关：通讯选择： 0x00表示选择一对一模式485通讯（默认模式）；0X01表示选择奇数表位485通讯；0X02表示选择偶数表位485通讯；0x03表示选择一对一模式红外通讯；0X04表示选择奇数表位红外通讯；0X05表示选择偶数表位红外通讯；0X06表示选择切换到485总线（电科院协议用）。
        /// </summary>
        /// <param name="int_Num">表位号(255所有表位)FF 全表位，0xEE,偶数，0xDD奇数</param>
        /// <param name="bln_Open">是否打开，true=打开，flase=关闭</param>
        /// <returns></returns>
        public bool SetCommSwitch(int int_Num, bool bln_Open)
        {
            //Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 通讯选择（1Byte）
            //通讯选择： 0x00表示选择一对一模式485通讯（默认模式）；0X01表示选择奇数表位485通讯；0X02表示选择偶数表位485通讯；
            //通讯选择： 0x03表示选择一对一模式红外通讯；0X04表示选择奇数表位红外通讯；0X05表示选择偶数表位红外通讯；
            //通讯选择： 0X06表示选择切换到485总线（电科院协议用）。
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                byte bytCmd;
                if (int_Num == 0xFF)
                {
                    bytCmd = Convert.ToByte(bln_Open ? 0x00 : 0x03);
                }
                else if (int_Num == 0xEE)
                {
                    bytCmd = Convert.ToByte(bln_Open ? 0x02 : 0x03);
                }
                else if (int_Num == 0xDD)
                {
                    bytCmd = Convert.ToByte(bln_Open ? 0x01 : 0x03);
                }
                else
                {
                    if (int_Num % 2 == 0)
                    {
                        bytCmd = Convert.ToByte(bln_Open ? 0x02 : 0x03);
                    }
                    else
                    {
                        bytCmd = Convert.ToByte(bln_Open ? 0x01 : 0x03);

                    }
                }

                byte[] byt_SendData = this.OrgFrameForComm(0xAC, Convert.ToByte(int_Num), bytCmd);

                this.m_byt_RevData = new byte[0];

                this.m_Ispt_com.SendData(byt_SendData);

                Waiting(300, 300);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }
        #endregion


        #region AFH：	用于双回路检定时，选择其中的某一路作为电流的输出回路
        /// <summary>
        /// 回路切换
        /// </summary>
        /// <param name="int_DL_type">电流回路： 0一回路 1二回路</param>
        /// <param name="int_DY_type">电压回路：0x00直接接入式，0x01经互感器，0x02表示本表位无电表接入</param>
        /// <returns></returns>
        public bool SetDLSwitch(int int_DL_type, int int_DY_type)
        {
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;


                byte[] byt_tmpdata = new byte[2];
                byt_tmpdata[0] = Convert.ToByte(int_DL_type);//电流回路
                byt_tmpdata[1] = Convert.ToByte(int_DY_type);//电压回路
                byte[] byt_SendData = this.OrgFrameForDLSwich(0xAF, byt_tmpdata);              //B4H隔离表位
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(200, 200);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 回路切换
        /// </summary>
        /// <param name="int_DL_type">电流回路： 0一回路 1二回路</param>
        /// <returns></returns>
        public bool SetDLSwitch(int int_DL_type)
        {
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;


                byte[] byt_tmpdata = new byte[2];
                byt_tmpdata[0] = Convert.ToByte(int_DL_type);
                byt_tmpdata[1] = 0x00;
                byte[] byt_SendData = this.OrgFrameForDLSwich(0xAF, byt_tmpdata);              //AFH回路切换表位
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(200, 200);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }
        #endregion


        #region B1H：	启动计算功能指令
        /// <summary>
        /// 启动误差板计算功能 带任务类型参数
        /// </summary>
        /// <param name="byt_TaskType">检定类型：0x00电能误差、0x01需量误差、0x02日计时误差、0x03走字计数、0x04对标、0x05预付费功能检定</param>
        /// <returns></returns>
        public bool StartCalculate(int byt_TaskType)
        {
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                byte[] byt_SendData = this.OrgFrame(0xB1, Convert.ToByte(byt_TaskType));   //B1H启动误差板
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(200, 200);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 启动误差板计算功能
        /// </summary>
        ///<param name="int_Num">表位号(255所有表位)FF 全表位，0xEE,偶数，0xDD奇数</param>
        /// <param name="byt_TaskType">检定类型：0x00电能误差、0x01需量误差、0x02日计时误差、0x03走字计数、0x04对标、0x05预付费功能检定</param>
        /// <returns></returns>
        private bool StartCalculate(int int_Num, int byt_TaskType)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                this.m_Ispt_com.Setting = this.m_str_Setting;
                //任务类型,0=电能误差,1=需量周期,2=日计时误差,3=计数,4=对标
                if (byt_TaskType < 0 || byt_TaskType > 4)
                {
                    this.m_str_LostMessage = "任务类型超出指定范围0-3!";
                    return false;
                }
                if (Array.IndexOf(this.m_int_ListTable, int_Num) == -1 && int_Num != 255)
                {
                    this.m_str_LostMessage = "此表位号不在总线上！";
                    return false;
                }
                byte[] byt_Value = new byte[2];
                byt_Value[0] = Convert.ToByte(byt_TaskType);//任务类型
                byte[] byt_SendData;
                if (int_Num == 255)
                {
                    byt_SendData = this.OrgFrame(0xB1, byt_Value[0]);  //总线上全表位命令
                }
                else
                {
                    byt_Value[1] = Convert.ToByte(int_Num);//单表位命令
                    byt_SendData = this.OrgFrame(0xB1, byt_Value[0], byt_Value[1]);
                }

                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(200, 200);
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(200, 200);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        #endregion


        #region B2H：	停止检定功能指令
        /// <summary>
        /// 停止误差板计算功能
        /// </summary>
        /// <param name="byt_TaskType">检定类型：0x00电能误差、0x01需量误差、0x02日计时误差、0x03走字计数、0x04对标、0x05预付费功能检定</param>
        /// <returns></returns>
        public bool StopCalculate(int byt_TaskType)
        {
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                byte[] byt_SendData = this.OrgFrame(0xB2, Convert.ToByte(byt_TaskType));   //B2H停止误差板
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(200, 200);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 停止误差板计算功能
        /// </summary>
        ///<param name="int_Num">表位号(255所有表位)FF 全表位，0xEE,偶数，0xDD奇数</param>
        /// <param name="byt_TaskType">检定类型：0x00电能误差、0x01需量误差、0x02日计时误差、0x03走字计数、0x04对标、0x05预付费功能检定</param>
        /// <returns></returns>
        private bool StopCalculate(int int_Num, int byt_TaskType)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                //任务类型,0=电能误差,1=需量周期,2=日计时误差,3=计数,4=对标
                this.m_Ispt_com.Setting = this.m_str_Setting;
                if (byt_TaskType < 0 || byt_TaskType > 4)
                {
                    this.m_str_LostMessage = "任务类型超出指定范围0-3!";
                    return false;
                }
                if (Array.IndexOf(this.m_int_ListTable, int_Num) == -1 && int_Num != 255)
                {
                    this.m_str_LostMessage = "此表位号不在总线上！";
                    return false;
                }
                byte[] byt_Value = new byte[2];
                byt_Value[0] = Convert.ToByte(byt_TaskType);//任务类型
                byte[] byt_SendData;
                if (int_Num == 255)
                {
                    byt_SendData = this.OrgFrame(0xB2, byt_Value[0]);  //总线上全表位命令
                }
                else
                {
                    byt_Value[1] = Convert.ToByte(int_Num);//单表位命令
                    byt_SendData = this.OrgFrame(0xB2, byt_Value[0], byt_Value[1]);
                }

                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(200, 200);
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(200, 200);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }


        #endregion


        #region B4H：	故障表位电压电流隔离控制
        /// <summary>
        /// 故障表位电压电流隔离控制，无参数，控制放在误差板列表中（12 bytes List）
        /// </summary>
        /// <returns></returns>
        public bool SetBwGLSwitch()
        {
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                byte[] byt_SendData = this.OrgFrameForGL(0xB4);   //B4H隔离表位
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(200, 200);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }


        #endregion


        #region C2H：	清除表位状态
        /// <summary>
        /// 清理接线故障状态，
        /// </summary>
        /// 状态类型分为四种：接线故障状态01、预付费跳闸状态02、报警信号状态03、对标状态04
        /// <returns></returns>
        public bool ClearBwState(int int_StateType)
        {
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;

                byte[] byt_SendData = this.OrgFrameForClearState(0xC2, Convert.ToByte(int_StateType));
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(200, 200);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }


        #endregion


        #region C0H：	读取表位各类型误差及各种状态

        #region 读误差板数据

        /// <summary>
        /// 读取所有表位一次误差板信息
        /// </summary>
        /// <param name="bln_Result">是否获得数据</param>
        /// <param name="byt_TaskType">检定类型：0x00电能误差、0x01需量误差、0x02日计时误差、0x03走字计数、0x04对标、0x05预付费功能检定</param>
        /// <param name="int_Sum">对应表位的误差次数</param>
        /// <param name="str_Data">对应表位的误差值</param>
        /// <returns></returns>
        public bool ReadData(int byt_TaskType,ref bool[] bln_Result, ref int[] int_Sum, ref string[] str_Data)
        {

            this.m_str_LostMessage = "";
            try
            {
                if (this.m_int_ListTable == null || this.m_int_ListCount <= 0)
                {
                    this.m_str_LostMessage = "此误差板总线上没有表位列表！";
                    return false;
                }

                bool bln_IsOK = false;       //只要有一个表OK就OK

                byte[] byt_RevData = new byte[0];
                for (int int_Inc = 0; int_Inc < this.m_int_ListCount; int_Inc++)
                {


                    if (m_str_BwSelect.Substring(m_str_BwSelect.Length - this.m_int_ListTable[int_Inc], 1) == "1")
                    {

                        ReadData188L(this.m_int_ListTable[int_Inc], byt_TaskType);//
                        Waiting(300, 50);

                        if (this.m_byt_RevData.Length > 0)
                        {

                            if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                            {


                                if (GetData188L(byt_RevData, ref bln_Result, ref int_Sum, ref str_Data))
                                {

                                    bln_IsOK = true;
                                }

                            }

                        }
                    }

                }


                return bln_IsOK;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 读取表位各类型误差及各种状态
        /// </summary>
        /// <param name="int_Num">表位号(255所有表位)FF 全表位，0xEE,偶数，0xDD奇数</param>
        /// <param name="byt_TaskType">检定类型：0x00电能误差、0x01需量误差、0x02日计时误差、0x03走字计数、0x04对标、0x05预付费功能检定</param>
        /// <param name="bln_Result">读取是否成功</param>
        /// <param name="int_Sum">误差次数</param>
        /// <param name="str_Data">误差值</param>
        /// <returns></returns>
        public bool ReadData(int byt_TaskType, int int_Num, ref bool bln_Result, ref int int_Sum, ref string str_Data)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                if (Array.IndexOf(this.m_int_ListTable, int_Num) == -1)
                {
                    this.m_str_LostMessage = "此表位号不在总线上！";
                    return false;
                }
                byte[] byt_SendData;
                if (int_Num == 255)
                {

                    byt_SendData = this.OrgFrame(0xC0, Convert.ToByte(byt_TaskType)); //34H
                }
                else
                {
                    byt_SendData = this.OrgFrameForReadWcb(0xC0, Convert.ToByte(byt_TaskType), Convert.ToByte(int_Num)); //34H
                }
                bln_Result = false;
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(500, 300);
                byte[] byt_RevData = new byte[0];
                if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                {
                    //len=6
                    //Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List）20 
                    //+ 检定误差类型（1Byte） 21
                    //+ 当前表位编号（1Byte）22
                    //+误差次数（1Byte） 23
                    //+ 误差值（4Bytes） 27
                    //+ 状态类型（1Byte） 28
                    //+ 电流回路状态（1Byte）29 
                    //+ 电压回路状态（1Byte） 30
                    //+ 通讯口状态（1Byte） 31
                    //+ 工作状态（1Byte）32
                    //+ 发送标志1+发送标志2。
                    //

                    if (byt_RevData[4] == 0x50 && byt_RevData[20] == Convert.ToByte(int_Num))//等于36H
                    {
                        str_Data = Convert.ToString(byt_RevData[6]);

                        int int_Dot = 0;//= byt_RevData[23] >> 7;//放大倍数
                        byte[] byt_Tmp = new byte[4];
                        Array.Copy(byt_RevData, 23, byt_Tmp, 1, 3);
                        Array.Reverse(byt_Tmp);
                        if ((byt_RevData[22] >> 8 & 1) == 1)//f
                        {
                            int_Dot = byt_RevData[22] - 0x80;
                            str_Data = Convert.ToString(0 - BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, int_Dot));
                        }
                        else//z
                        {
                            int_Dot = byt_RevData[22];
                            str_Data = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, int_Dot));
                        }
                        int_Sum = byt_RevData[20];
                        bln_Result = true;
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

        #region 读误差板状态
        /// <summary>
        /// 获得误差板信息 接线故障状态（Bit0）、预付费跳闸状态（Bit1）、报警信号状态（Bit2）、对标状态（Bit3）
        /// 一个字节（八位）
        /// 接线故障状态（Bit0）、预付费跳闸状态（Bit1）、报警信号状态（Bit2）、
        /// 对标状态（Bit3）的参数分别由一个字节中的Bit0、Bit1、Bit2、Bit3标示，为1则表示该表位有故障/跳闸/报警/对标完成，
        /// 为0则表示正常/正常/正常/未完成对标。
        /// </summary>不足八位，高位补零    by Zhoujl
        /// <param name="bln_Result">是否获得状态</param>
        /// <param name="str_Data">对应表位的状态</param>
        /// <returns></returns>  
        public bool ReadData(ref bool[] bln_Result, ref string[] str_Data)
        {

            this.m_str_LostMessage = "";
            try
            {
                if (this.m_int_ListTable == null || this.m_int_ListCount <= 0)
                {
                    this.m_str_LostMessage = "此误差板总线上没有表位列表！";
                    return false;
                }

                bool bln_IsOK = false;       //只要有一个表OK就OK

                byte[] byt_RevData = new byte[0];

                for (int int_Inc = 0; int_Inc < this.m_int_ListCount; int_Inc++)
                {


                    if (m_str_BwSelect.Substring(m_str_BwSelect.Length - this.m_int_ListTable[int_Inc], 1) == "1")
                    {
                        ReadData188L(this.m_int_ListTable[int_Inc], 0x00);//读状态时任务类型用电能误差
                        Waiting(100, 50);

                        if (this.m_byt_RevData.Length > 0)
                        {
                            if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                            {
                                if (GetData188LState(byt_RevData, ref bln_Result, ref str_Data))
                                    bln_IsOK = true;
                            }
                        }
                    }

                }


                return bln_IsOK;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }
        #endregion

        #region 私有命令帧及处理数据
        /// <summary>
        /// 发送读取误差板信息指令，A0。
        /// </summary>
        /// <param name="int_Num">表位号(255所有表位)FF 全表位，0xEE,偶数，0xDD奇数</param>
        /// <param name="byt_TaskType">检定类型：0x00电能误差、0x01需量误差、0x02日计时误差、0x03走字计数、0x04对标、0x05预付费功能检定</param>
        /// <returns></returns>
        private bool ReadData188L(int int_Num, int byt_TaskType)
        {
            if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                this.m_Ispt_com.Setting = this.m_str_Setting;
            if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                this.m_Ispt_com.Channel = this.m_int_Channel;

            this.m_Ispt_com.Setting = this.m_str_Setting;
            byte[] byt_SendData;
            if (int_Num == 255)
                byt_SendData = this.OrgFrame(0xC0, Convert.ToByte(byt_TaskType)); //C0H 广播
            else
                byt_SendData = this.OrgFrameForReadWcb(0xC0, Convert.ToByte(byt_TaskType), Convert.ToByte(int_Num)); //C0H 非广播

            this.m_byt_RevData = new byte[0];
            this.m_Ispt_com.SendData(byt_SendData);

            return true;
        }

        /// <summary>
        /// 解析误差板返回的误差数据
        /// </summary>
        /// <param name="byt_RevData">误差板返回的数据</param>
        /// <param name="bln_Result">是否获得数据</param>       
        /// <param name="str_Data">对应表位的四种状态值</param>
        /// <returns></returns>
        private bool GetData188L(byte[] byt_RevData, ref bool[] bln_Result, ref int[] int_Sum, ref string[] str_Data)
        {
            int meterSt = byt_RevData[20];//当前返回数据的表位数
            int errorCount = byt_RevData[21];//误差次数
            string errorMassage = null;//误差值 （4Bytes）

            int int_Dot = 0;//= byt_RevData[22] >> 7;//放大倍数
            byte[] byt_Tmp = new byte[4];
            Array.Copy(byt_RevData, 23, byt_Tmp, 1, 3);
            Array.Reverse(byt_Tmp);
            if (byt_RevData[22] >= 0x80)//负数
            {
                int_Dot = byt_RevData[22] - 0x80;
                errorMassage = Convert.ToString(0 - BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, int_Dot));
                string s = Convert.ToString(0 - BitConverter.ToInt32(byt_Tmp, 0));
            }
            else//正数
            {
                int_Dot = byt_RevData[22];
                errorMassage = Convert.ToString(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, int_Dot));
                string s = Convert.ToString(0 - BitConverter.ToInt32(byt_Tmp, 0));
            }

            bln_Result[meterSt - 1] = true;
            int_Sum[meterSt - 1] = errorCount;
            str_Data[meterSt - 1] = errorMassage;
            return true;
        }

        /// <summary>
        /// 解析误差板返回的状态数据  接线故障状态（Bit0）、预付费跳闸状态（Bit1）、报警信号状态（Bit2）、对标状态（Bit3）
        /// </summary> by Zhoujl
        /// <param name="byt_RevData">误差板返回的数据</param>
        /// <param name="bln_Result">是否获得数据</param>       
        /// <param name="str_Data">对应表位的四种状态值</param>
        /// <returns></returns>
        private bool GetData188LState(byte[] byt_RevData, ref bool[] bln_Result, ref string[] str_Data)
        {
            int meterSt = byt_RevData[20];//当前返回数据的表位数
            int errorCount = byt_RevData[21];//误差次数
            string errorMassage = null;// （状态值）

            //将16进制转化为8位二进制，不足高位补零
            errorMassage = Convert.ToString(byt_RevData[26], 2).PadLeft(8, '0');


            bln_Result[meterSt - 1] = true;
            str_Data[meterSt - 1] = errorMassage;
            return true;
        }
        #endregion

        #endregion


        #region C3H：	读取表位前10次各类型误差及当前各种状态
        /// <summary>
        /// 读取10次误差板数据(读十个)
        /// </summary>
        /// <param name="byt_TaskType">检定类型：0x00电能误差、0x01需量误差、0x02日计时误差、0x03走字计数、0x04对标、0x05预付费功能检定</param>
        /// <param name="bln_Result">是否获得数据</param>
        /// <param name="int_Sum">对应表位的误差次数</param>
        /// <param name="str_Data">对应表位的误差值</param>
        /// <returns></returns>
        public bool ReadDataTenTime(int byt_TaskType,ref bool[] bln_Result, ref int[] int_Sum, ref string[] str_Data)
        {

            this.m_str_LostMessage = "";
            try
            {
                if (this.m_int_ListTable == null || this.m_int_ListCount <= 0)
                {
                    this.m_str_LostMessage = "此误差板总线上没有表位列表！";
                    return false;
                }

                bool bln_IsOK = false;       //只要有一个表OK就OK

                byte[] byt_RevData = new byte[0];
                for (int int_Inc = 0; int_Inc < this.m_int_ListCount; int_Inc++)
                {
                    if (m_str_BwSelect.Substring(m_str_BwSelect.Length - this.m_int_ListTable[int_Inc], 1) == "1")
                    {
                        ReadData188L10Times(this.m_int_ListTable[int_Inc], byt_TaskType);
                        Waiting(300, 50);

                        if (this.m_byt_RevData.Length > 0)
                        {
                            if (CheckFrame(this.m_byt_RevData, ref byt_RevData))            //检查返回帧是否正常
                            {
                                if (GetData188L10Times(byt_RevData, ref bln_Result, ref int_Sum, ref str_Data))
                                    bln_IsOK = true;
                            }
                        }

                    }

                }

                return bln_IsOK;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        #region 私有命令帧及处理数据

        /// <summary>
        /// 发送读取误差板信息指令，C3H,读起10次误差。
        /// </summary>
        /// <param name="int_Num">指令是否广播标志，FF （255） 表示广播</param>
        /// <param name="byt_TaskType">检定类型：0x00电能误差、0x01需量误差、0x02日计时误差、0x03走字计数、0x04对标、0x05预付费功能检定</param>
        /// <returns></returns>
        private bool ReadData188L10Times(int int_Num,int byt_TaskType)
        {
            if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                this.m_Ispt_com.Setting = this.m_str_Setting;
            if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                this.m_Ispt_com.Channel = this.m_int_Channel;
            byte[] byt_SendData;
            if (int_Num == 255)
                byt_SendData = this.OrgFrame(0xC3, Convert.ToByte(byt_TaskType)); //C0H 广播
            else
                byt_SendData = this.OrgFrameForReadWcb(0xC3, Convert.ToByte(byt_TaskType), Convert.ToByte(int_Num)); //C0H 非广播

            this.m_byt_RevData = new byte[0];
            this.m_Ispt_com.SendData(byt_SendData);

            return true;
        }
        /// <summary>
        /// 解析误差板10次误差返回的数据
        /// </summary>
        /// <param name="byt_RevData">误差板返回的数据</param>
        /// <param name="bln_Result">是否获得数据</param>
        /// <param name="int_Sum">对应表位的误差次数</param>
        /// <param name="str_Data">对应表位的误差值</param>
        /// <returns></returns>
        private bool GetData188L10Times(byte[] byt_RevData, ref bool[] bln_Result, ref int[] int_Sum, ref string[] str_Data)
        {
            try
            {
                int meterSt = byt_RevData[20];//当前返回数据的表位数
                int errorCount = byt_RevData[21];//误差次数
                string errorMassage = null;//误差值 （4Bytes）
                string strError = "";

                for (int i = 0; i < 10; i++)
                {

                    int int_Dot = 0;//= byt_RevData[22] >> 7;//放大倍数
                    byte[] byt_Tmp = new byte[4];
                    Array.Copy(byt_RevData, (23 + i * 4), byt_Tmp, 1, 3);

                    Array.Reverse(byt_Tmp);
                    if (byt_RevData[22 + (i * 4)] >= 0x80)//负数
                    {
                        int_Dot = byt_RevData[22 + (i * 4)] - 0x80;
                        errorMassage = string.Format("{0:F5}", (0 - BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, int_Dot)));
                    }
                    else//正数
                    {
                        int_Dot = byt_RevData[22 + (i * 4)];
                        errorMassage = string.Format("{0:F5}", BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, int_Dot));
                    }


                    strError += errorMassage + "|";
                }
                bln_Result[meterSt - 1] = true;
                int_Sum[meterSt - 1] = errorCount;
                str_Data[meterSt - 1] = strError;
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #endregion


        #region 扩展命令

        /// <summary>
        /// 执行其它扩展指令(有返回指令)
        /// </summary>
        /// <param name="byt_Cmd">命令字</param>
        /// <param name="byt_Data">数据</param>
        /// <param name="bln_RevResult">返回结论</param>
        /// <param name="byt_RevData">返回数据</param>
        /// <param name="int_Scend">等待时间</param>
        /// <returns></returns>
        public bool ExeOtherCmd(byte byt_Cmd, byte[][] byt_Data, ref bool[] bln_RevResult, ref byte[][] byt_RevData, int int_Scend)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                if (this.m_int_ListTable == null || this.m_int_ListCount <= 0)
                {
                    this.m_str_LostMessage = "此误差板总线上没有表位列表！";
                    return false;
                }

                bool bln_IsOK = false;       //只要有一个表OK就OK
                for (int int_Inc = 0; int_Inc < this.m_int_ListCount; int_Inc++)
                {
                    bln_RevResult[this.m_int_ListTable[int_Inc] - 1] = this.ExeOtherCmd(this.m_int_ListTable[int_Inc], byt_Cmd, byt_Data[this.m_int_ListTable[int_Inc] - 1], ref byt_RevData[this.m_int_ListTable[int_Inc] - 1], int_Scend);
                    if (bln_RevResult[this.m_int_ListTable[int_Inc] - 1])
                        bln_IsOK = true;
                }
                return bln_IsOK;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }


        }




        /// <summary>
        /// 执行其它扩展指令(单表位,有返回指令)
        /// </summary>
        /// <param name="int_Num">表位</param>
        /// <param name="byt_Cmd">命令字</param>
        /// <param name="byt_Data">数据</param>
        /// <param name="byt_RevData">返回指令</param>
        ///  <param name="int_Scend">等待时间</param>
        /// <returns></returns>
        public bool ExeOtherCmd(int int_Num, byte byt_Cmd, byte[] byt_Data, ref byte[] byt_RevData, int int_Scend)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                if (Array.IndexOf(this.m_int_ListTable, int_Num) == -1)
                {
                    this.m_str_LostMessage = "此表位号不在总线上！";
                    return false;
                }
                byte[] byt_SendData = this.OrgFrame(byt_Cmd, byt_Data);
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(int_Scend, 200);
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

        /// <summary>
        /// 执行其它扩展指令(无返回指令)
        /// </summary>
        /// <param name="byt_Cmd">命令字</param>
        /// <param name="byt_Data">数据</param>
        /// <param name="int_Scend">间隔时间</param>
        /// <returns></returns>
        public bool ExeOtherCmd(byte byt_Cmd, byte[] byt_Data, int int_Scend)
        {

            for (int int_Int = 0; int_Int < this.m_int_ListCount; int_Int++)
            {
                this.ExeOtherCmd(this.m_int_ListTable[int_Int], byt_Cmd, byt_Data, int_Scend);
            }
            return true;
        }

        /// <summary>
        /// 执行其它扩展指令(单表位,无返回指令)
        /// </summary>
        /// <param name="int_Num">表位</param>
        /// <param name="byt_Cmd">命令字</param>
        /// <param name="byt_Data">数据</param>
        /// <param name="int_Scend">间隔时间</param>
        /// <returns></returns>
        public bool ExeOtherCmd(int int_Num, byte byt_Cmd, byte[] byt_Data, int int_Scend)
        {
            this.m_str_LostMessage = "";
            try
            {
                if (this.m_Ispt_com.Setting != this.m_str_Setting)      //主要防止是同一个RS485多个设备
                    this.m_Ispt_com.Setting = this.m_str_Setting;
                if (this.m_Ispt_com.Channel != this.m_int_Channel)      //主要防止同一个RS485不同一个通道上，主要是在CL2011设置上
                    this.m_Ispt_com.Channel = this.m_int_Channel;
                if (Array.IndexOf(this.m_int_ListTable, int_Num) == -1)
                {
                    this.m_str_LostMessage = "此表位号不在总线上！";
                    return false;
                }
                byte[] byt_SendData = this.OrgFrame(byt_Cmd, byt_Data);
                this.m_byt_RevData = new byte[0];
                this.m_Ispt_com.SendData(byt_SendData);
                Waiting(int_Scend, 200);
                return true;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        #endregion

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
        /// 从误差表位通道和表位选择字符串获得当前通道的表位选择字符串
        /// </summary>
        /// <param name="intChannel">通道号</param>
        /// <param name="BwSelectString">全部表位选择的字符串"1111110000....."</param>
        /// <returns>当前通道上的表位选择字符串，其他表位置0</returns>
        private string GetBwListstring(int intChannel, string BwSelectString)
        {
            string strtmp1 = "";
            string strtmp2 = BwSelectString;

            for (int i = 0; i < this.m_int_Bws; i++)
            {
                if (this.m_int_Bws - 1 - i < this.m_int_ListTable[0] - 1 || this.m_int_Bws - 1 - i > m_int_ListTable[m_int_ListTable.Length - 1] - 1)
                {
                    strtmp1 = strtmp1 + "0";

                }
                else
                {
                    strtmp1 = strtmp1 + strtmp2.Substring(i, 1); //strtmp2.Substring(0, strtmp2.Length - i - 1) + "0" + strtmp2.Substring(strtmp2.Length - 1 - i, i);
                }
            }
            return strtmp1;

        }

        /// <summary>
        /// 从误差表位通道和表位选择字符串获得当前通道的奇偶表位选择字符串0=奇数，1=偶数
        /// </summary>
        /// <param name="intChannel">通道号</param>
        /// <param name="BwSelectString">全部表位选择的字符串"1111110000....."</param>
        /// <returns>当前通道上的表位选择字符串，其他表位置0</returns>
        private string GetBwListstring(int intChannel, int int_type, string BwSelectString)
        {
            string strtmp1 = "";
            string strtmp2 = BwSelectString;
            if (int_type == 0)
            {
                for (int i = 0; i < this.m_int_Bws; i++)
                {
                    if (i % 2 == 0)
                    {
                        strtmp1 = strtmp1 + "1";

                    }
                    else
                    {

                        strtmp1 = strtmp1 + "0";
                    }

                }
            }
            else
            {
                for (int i = 0; i < this.m_int_Bws; i++)
                {
                    if (i % 2 == 0)
                    {
                        strtmp1 = strtmp1 + "0";

                    }
                    else
                    {

                        strtmp1 = strtmp1 + "1";
                    }

                }

            }
            return strtmp1;

        }

        /// <summary>
        /// 单个表位的数据
        /// </summary>
        /// <param name="intChannel"></param>
        /// <param name="byt_bwnum"></param>
        /// <param name="BwSelectString"></param>
        /// <returns></returns>
        private string GetBwListstring(int intChannel, byte byt_bwnum, string BwSelectString)
        {
            string strtmp = BwSelectString;
            string str_tmp1 = "";
            string str_tmp2 = "";
            int int_tmp = 0;
            try
            {
                for (int i = 0; i < this.m_int_Bws; i++)
                {
                    if (i == (byt_bwnum - 1))
                    {
                        int_tmp = strtmp.Length - i - 1;

                        str_tmp1 = strtmp.Remove(int_tmp, 1);

                        strtmp = str_tmp1.Insert(int_tmp, "1");
                    }
                    else
                    {

                        int_tmp = strtmp.Length - i - 1;

                        str_tmp1 = strtmp.Remove(int_tmp, 1);

                        strtmp = str_tmp1.Insert(int_tmp, "0");

                    }
                }
                return strtmp;
            }
            catch (Exception e)
            {
                this.m_str_LostMessage = e.ToString();
                return "";
            }


        }

        /// <summary>
        /// 将"01011001011"将01序列反转0变1 ，1变0
        /// </summary>
        /// <param name="str01"></param>
        /// <returns></returns>
        private string GetChange01(string str01)
        {
            string strtmp = "";
            for (int i = 0; i < str01.Length; i++)
            {

                if (str01.Substring(i, 1) == "0")
                {
                    strtmp = strtmp + "1";
                }
                else
                {
                    strtmp = strtmp + "0";
                }

            }
            return strtmp;

        }

        /// <summary>
        /// 转换字符串形式01010变成字节
        /// </summary>
        /// <param name="strtmp">96位的"0001100010101......"</param>
        /// <returns></returns>
        private byte[] ChangeStringToByte(string strtmp)
        {
            byte[] Arrytmpbyte = new byte[12];

            byte tmpbyte = new byte();
            for (int i = 0; i < 12; i++)
            {
                tmpbyte = 0x00;
                for (int k = 0; k < 8; k++)
                {
                    tmpbyte += Convert.ToByte(strtmp.Substring(strtmp.Length - 1 - 8 * i - k, 1).Equals("1") ? (Math.Pow(2, k)) : 0x00);
                }
                Arrytmpbyte[11 - i] = tmpbyte;
            }
            return Arrytmpbyte;

        }
        #region 组帧


        /// <summary>
        ///  组织帧
        /// </summary>
        /// <param name="byt_Cmd">控制码</param>
        /// <param name="byt_Data">数据域</param>
        /// <returns>返回组好的帧</returns>
        private byte[] OrgFrame(byte byt_Cmd, byte[] byt_Data)
        {
            int int_Len = 20 + byt_Data.Length;                   //81、ID、len、CMD和Chksum各占一位 +数据域位
            byte[] byt_Frame = new byte[int_Len];
            byt_Frame[0] = 0x81;                                    //81
            byt_Frame[1] = Convert.ToByte(this.m_str_ID, 16);       //ID
            byt_Frame[2] = 05;                                      //SD
            byt_Frame[3] = Convert.ToByte(int_Len);                 // Len
            byt_Frame[4] = byt_Cmd;                                 //Cmd
            byt_Frame[5] = 0xFF;                                    //广播指令标志
            byt_Frame[6] = Convert.ToByte(this.m_int_Bws);          //表位数

            byte[] tmparrbyte;
            tmparrbyte = ChangeStringToByte(m_str_BwSelect);        //要检
            Array.Copy(tmparrbyte, 0, byt_Frame, 7, tmparrbyte.Length);

            if (byt_Data.Length > 0) Array.Copy(byt_Data, 0, byt_Frame, 19, byt_Data.Length);

            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //校验码    校验码从81后面开始计算
            {
                byt_Frame[int_Len - 1] ^= byt_Frame[int_Inc];               //Chksum
            }
            return byt_Frame;

        }
        /// <summary>
        ///  组织帧 带表位号
        /// </summary>
        /// <param name="byt_Cmd">控制码</param>
        /// <param name="byt_Data">数据域</param>
        /// <returns>返回组好的帧</returns>
        private byte[] OrgFrame(byte byt_Cmd, byte[] byt_Data, byte byt_BwNum)
        {
            int int_Len = 20 + byt_Data.Length;                   //81、ID、len、CMD和Chksum各占一位 +数据域位
            byte[] byt_Frame = new byte[int_Len];
            byt_Frame[0] = 0x81;                                    //81
            byt_Frame[1] = Convert.ToByte(this.m_str_ID, 16);       //ID
            byt_Frame[2] = 05;                                      //SD
            byt_Frame[3] = Convert.ToByte(int_Len);                 // Len
            byt_Frame[4] = byt_Cmd;                                 //Cmd
            byt_Frame[5] = 0xFF;                                    //广播指令标志
            byt_Frame[6] = Convert.ToByte(this.m_int_Bws);          //ListLen

            byte[] tmparrbyte;
            tmparrbyte = ChangeStringToByte(m_str_BwSelect);
            Array.Copy(tmparrbyte, 0, byt_Frame, 7, tmparrbyte.Length);

            if (byt_Data.Length > 0) Array.Copy(byt_Data, 0, byt_Frame, 19, byt_Data.Length);

            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //校验码    校验码从81后面开始计算
            {
                byt_Frame[int_Len - 1] ^= byt_Frame[int_Inc];               //Chksum
            }
            return byt_Frame;

        }


        /// <summary>
        /// 组织帧
        /// </summary>
        /// <param name="byt_Cmd">控制码</param>
        /// <returns>返回组好的帧</returns>
        private byte[] OrgFrame(byte byt_Cmd)
        {
            int int_Len = 20;// + byt_Data.Length;                   //81、ID、SD、len、CMD和Chksum+14各占一位 +数据域位
            byte[] byt_Data = new byte[20];
            byt_Data[0] = 0x81;                                  //81
            byt_Data[1] = Convert.ToByte(this.m_str_ID, 16);    //ID
            byt_Data[2] = 0x05;                                   //SD
            byt_Data[3] = Convert.ToByte(int_Len);                     //Len
            byt_Data[4] = byt_Cmd;                          //Cmd
            byt_Data[5] = 0xFF;                            //广播指令标志
            byt_Data[6] = Convert.ToByte(this.m_int_Bws);//ListLen

            byte[] tmparrbyte;

            //tmparrbyte = ChangeStringToByte(GetBwListstring(m_int_Channel, m_str_BwSelect));
            tmparrbyte = ChangeStringToByte(m_str_BwSelect);
            Array.Copy(tmparrbyte, 0, byt_Data, 7, tmparrbyte.Length);

            for (int int_Inc = 1; int_Inc < 19; int_Inc++)
            {
                byt_Data[19] ^= byt_Data[int_Inc];               //Chksum
            }
            return byt_Data;



        }

        /// <summary>
        ///  组织帧
        /// </summary>
        /// <param name="byt_Cmd">控制码</param>
        /// <param name="byt_Value">数据域</param>
        /// <returns>返回组好的帧</returns>
        private byte[] OrgFrame(byte byt_Cmd, byte byt_Value)
        {

            int int_Len = 21;                  //81、ID、SD \len、CMD和Chksum各占一位 +数据域位
            byte[] byt_Data = new byte[int_Len];
            byt_Data[0] = 0x81;                                  //81
            byt_Data[1] = Convert.ToByte(this.m_str_ID, 16);     //ID

            byt_Data[2] = 0x05;                                  //SD
            byt_Data[3] = Convert.ToByte(int_Len);               //Len

            byt_Data[4] = byt_Cmd;                              //Cmd

            byt_Data[5] = 0xFF;                                 //广播指令标志

            byt_Data[6] = Convert.ToByte(this.m_int_Bws);       //ListLen

            byte[] tmparrbyte;

            tmparrbyte = ChangeStringToByte(m_str_BwSelect);
            Array.Copy(tmparrbyte, 0, byt_Data, 7, tmparrbyte.Length);

            byt_Data[19] = byt_Value;

            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //校验码    校验码从81后面开始计算
            {
                byt_Data[int_Len - 1] ^= byt_Data[int_Inc];               //Chksum
            }
            return byt_Data;

        }

        /// <summary>
        ///  组织帧For读数据
        /// </summary>
        /// <param name="byt_Cmd">控制码</param>
        /// <param name="byt_Value">数据域</param>
        ///  <param name="byt_BwNum">表位号</param>
        /// <returns>返回组好的帧</returns>
        private byte[] OrgFrame(byte byt_Cmd, byte byt_Value, byte byt_BwNum)
        {

            int int_Len = 21;                   //81、ID、SD \len、CMD和Chksum各占一位 +数据域位
            byte[] byt_Data = new byte[int_Len];
            byt_Data[0] = 0x81;                                 //81
            byt_Data[1] = Convert.ToByte(this.m_str_ID, 16);    //ID

            byt_Data[2] = 0x05;                                 //SD
            byt_Data[3] = Convert.ToByte(int_Len);              //Len

            byt_Data[4] = byt_Cmd;                              //Cmd

            byt_Data[5] = 0xFF;                                 //广播指令标志

            byt_Data[6] = Convert.ToByte(this.m_int_Bws);       //ListLen

            byte[] tmparrbyte;

            //tmparrbyte = ChangeStringToByte(GetBwListstring(m_int_Channel, byt_BwNum, m_str_BwSelect));
            tmparrbyte = ChangeStringToByte(m_str_BwSelect);
            Array.Copy(tmparrbyte, 0, byt_Data, 7, tmparrbyte.Length);

            byt_Data[19] = byt_Value;

            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //校验码    校验码从81后面开始计算
            {
                byt_Data[int_Len - 1] ^= byt_Data[int_Inc];               //Chksum
            }
            return byt_Data;

        }


        //*****************************************

        /// <summary>
        /// 组织帧为隔离
        /// </summary>
        /// <param name="byt_Cmd">控制码</param>
        /// <returns>返回组好的帧</returns>
        private byte[] OrgFrameForGL(byte byt_Cmd)
        {
            int int_Len = 20;// + byt_Data.Length;               //81、ID、SD、len、CMD和Chksum+14各占一位 +数据域位
            byte[] byt_Data = new byte[20];
            byt_Data[0] = 0x81;                                  //81
            byt_Data[1] = Convert.ToByte(this.m_str_ID, 16);     //ID
            byt_Data[2] = 0x05;                                  //SD
            byt_Data[3] = Convert.ToByte(int_Len);               //Len
            byt_Data[4] = byt_Cmd;                          //Cmd
            byt_Data[5] = 0xFF;                            //广播指令标志
            byt_Data[6] = Convert.ToByte(this.m_int_Bws);//ListLen

            byte[] tmparrbyte;
            string str_BwGL;//
            //str_BwGL = GetBwListstring(m_int_Channel, m_str_BwSelect);
            str_BwGL = GetChange01(m_str_BwSelect);
            tmparrbyte = ChangeStringToByte(str_BwGL);
            Array.Copy(tmparrbyte, 0, byt_Data, 7, tmparrbyte.Length);

            for (int int_Inc = 1; int_Inc < 19; int_Inc++)
            {
                byt_Data[19] ^= byt_Data[int_Inc];               //Chksum
            }
            return byt_Data;



        }

        /// <summary>
        ///  组织帧
        /// </summary>
        /// <param name="byt_Cmd">控制码</param>
        /// <param name="byt_Data">数据域</param>
        /// <returns>返回组好的帧</returns>
        private byte[] OrgFrameForTimePulse(byte byt_Cmd, byte[] byt_Data)
        {
            int int_Len = 20 + byt_Data.Length;                   //81、ID、len、CMD和Chksum各占一位 +数据域位
            byte[] byt_Frame = new byte[int_Len];
            byt_Frame[0] = 0x81;                                  //81
            byt_Frame[1] = Convert.ToByte(this.m_str_ID, 16);    //ID
            byt_Frame[2] = 05;                                 //SD
            byt_Frame[3] = Convert.ToByte(int_Len);          // Len
            byt_Frame[4] = byt_Cmd;                         //Cmd

            byt_Frame[5] = 0xFF;                            //广播指令标志
            byt_Frame[6] = Convert.ToByte(this.m_int_Bws);//ListLen

            byte[] tmparrbyte = { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };
            //tmparrbyte = ChangeStringToByte(m_str_BwSelect);
            //tmparrbyte = ChangeStringToByte(m_str_BwSelect);
            //tmparrbyte = ChangeStringToByte(GetBwListstring(m_int_Channel, m_str_BwSelect));

            Array.Copy(tmparrbyte, 0, byt_Frame, 7, tmparrbyte.Length);

            if (byt_Data.Length > 0) Array.Copy(byt_Data, 0, byt_Frame, 19, byt_Data.Length);

            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //校验码    校验码从81后面开始计算
            {
                byt_Frame[int_Len - 1] ^= byt_Frame[int_Inc];               //Chksum
            }
            return byt_Frame;

        }

        /// <summary>
        ///  组织帧
        /// </summary>
        /// <param name="byt_Cmd">控制码</param>
        /// <param name="byt_Data">数据域</param>
        /// <returns>返回组好的帧</returns>
        private byte[] OrgFrameForDLSwich(byte byt_Cmd, byte[] byt_Data)
        {
            int int_Len = 20 + byt_Data.Length;                   //81、ID、len、CMD和Chksum各占一位 +数据域位
            byte[] byt_Frame = new byte[int_Len];
            byt_Frame[0] = 0x81;                                  //81
            byt_Frame[1] = Convert.ToByte(this.m_str_ID, 16);    //ID
            byt_Frame[2] = 05;                                 //SD
            byt_Frame[3] = Convert.ToByte(int_Len);          // Len
            byt_Frame[4] = byt_Cmd;                         //Cmd

            byt_Frame[5] = 0xFF;                            //广播指令标志
            byt_Frame[6] = Convert.ToByte(this.m_int_Bws);//ListLen

            byte[] tmparrbyte = { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };
            //tmparrbyte = ChangeStringToByte(m_str_BwSelect);
            //tmparrbyte = ChangeStringToByte(m_str_BwSelect);
            //tmparrbyte = ChangeStringToByte(GetBwListstring(m_int_Channel, m_str_BwSelect));

            Array.Copy(tmparrbyte, 0, byt_Frame, 7, tmparrbyte.Length);

            if (byt_Data.Length > 0) Array.Copy(byt_Data, 0, byt_Frame, 19, byt_Data.Length);

            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //校验码    校验码从81后面开始计算
            {
                byt_Frame[int_Len - 1] ^= byt_Frame[int_Inc];               //Chksum
            }
            return byt_Frame;

        }

        /// <summary>
        ///  组织帧
        /// </summary>
        /// <param name="byt_Cmd">控制码</param>
        /// <param name="byt_Data">数据域</param>
        /// <returns>返回组好的帧</returns>
        private byte[] OrgFrameForClear(byte byt_Cmd, byte[] byt_Data)
        {
            int int_Len = 32 + byt_Data.Length;                   //81、ID、len、CMD和Chksum各占一位 +数据域位
            byte[] byt_Frame = new byte[int_Len];
            byt_Frame[0] = 0x81;                                  //81
            byt_Frame[1] = Convert.ToByte(this.m_str_ID, 16);    //ID
            byt_Frame[2] = 05;                                 //SD
            byt_Frame[3] = Convert.ToByte(int_Len);          // Len
            byt_Frame[4] = byt_Cmd;                         //Cmd

            byt_Frame[5] = 0xFF;                            //广播指令标志
            byt_Frame[6] = Convert.ToByte(this.m_int_Bws);//ListLen

            byte[] tmparrbyte;

            //tmparrbyte = ChangeStringToByte(GetBwListstring(m_int_Channel, m_str_BwSelect));
            tmparrbyte = ChangeStringToByte(m_str_BwSelect);
            Array.Copy(tmparrbyte, 0, byt_Frame, 7, tmparrbyte.Length);

            if (byt_Data.Length > 0) Array.Copy(byt_Data, 0, byt_Frame, 19, byt_Data.Length);

            Array.Copy(tmparrbyte, 0, byt_Frame, 19 + byt_Data.Length, tmparrbyte.Length);


            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //校验码    校验码从81后面开始计算
            {
                byt_Frame[int_Len - 1] ^= byt_Frame[int_Inc];               //Chksum
            }
            return byt_Frame;

        }

        /// <summary>
        ///  组织帧 带表位号
        /// </summary>
        /// <param name="byt_Cmd">控制码</param>
        /// <param name="byt_Data">数据域</param>
        /// <returns>返回组好的帧</returns>
        private byte[] OrgFrameForReadWcb(byte byt_Cmd, byte[] byt_Data, byte byt_BwNum)
        {
            int int_Len = 20 + byt_Data.Length;                   //81、ID、len、CMD和Chksum各占一位 +数据域位
            byte[] byt_Frame = new byte[int_Len];
            byt_Frame[0] = 0x81;                                  //81
            byt_Frame[1] = Convert.ToByte(this.m_str_ID, 16);    //ID
            byt_Frame[2] = 05;                                 //SD
            byt_Frame[3] = Convert.ToByte(int_Len);          // Len
            byt_Frame[4] = byt_Cmd;                         //Cmd

            byt_Frame[5] = 0xFF;                            //广播指令标志
            byt_Frame[6] = Convert.ToByte(this.m_int_Bws);//ListLen

            byte[] tmparrbyte;

            tmparrbyte = ChangeStringToByte(GetBwListstring(m_int_Channel, byt_BwNum, m_str_BwSelect));
            // tmparrbyte = ChangeStringToByte(m_str_BwSelect);
            Array.Copy(tmparrbyte, 0, byt_Frame, 7, tmparrbyte.Length);

            if (byt_Data.Length > 0) Array.Copy(byt_Data, 0, byt_Frame, 19, byt_Data.Length);

            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //校验码    校验码从81后面开始计算
            {
                byt_Frame[int_Len - 1] ^= byt_Frame[int_Inc];               //Chksum
            }
            return byt_Frame;

        }

        /// <summary>
        ///  组织帧清理状态
        /// </summary>
        /// <param name="byt_Cmd">控制码</param>
        /// <param name="byt_Value">数据域</param>
        /// <returns>返回组好的帧</returns>
        private byte[] OrgFrameForClearState(byte byt_Cmd, byte byt_Value)
        {

            int int_Len = 33;                  //81、ID、SD \len、CMD和Chksum各占一位 +数据域位
            byte[] byt_Data = new byte[int_Len];
            byt_Data[0] = 0x81;                                  //81
            byt_Data[1] = Convert.ToByte(this.m_str_ID, 16);     //ID

            byt_Data[2] = 0x05;                                  //SD
            byt_Data[3] = Convert.ToByte(int_Len);               //Len

            byt_Data[4] = byt_Cmd;    //Cmd

            byt_Data[5] = 0xFF;                            //广播指令标志

            byt_Data[6] = Convert.ToByte(this.m_int_Bws);//ListLen

            byte[] tmparrbyte = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };


            Array.Copy(tmparrbyte, 0, byt_Data, 7, tmparrbyte.Length);
            byt_Data[19] = byt_Value;
            byte[] tmparrbyte1 = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            Array.Copy(tmparrbyte1, 0, byt_Data, 20, tmparrbyte.Length);

            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //校验码    校验码从81后面开始计算
            {
                byt_Data[int_Len - 1] ^= byt_Data[int_Inc];               //Chksum
            }
            return byt_Data;

        }

        /// <summary>
        ///  组织帧 为清理故障灯 
        /// </summary>
        /// <param name="byt_Cmd">控制码</param>
        /// <param name="byt_Value">数据域</param>
        /// <returns>返回组好的帧</returns>
        private byte[] OrgFrameForClear(byte byt_Cmd, byte byt_Value)
        {

            int int_Len = 21 + 12;                  //81、ID、SD \len、CMD和Chksum各占一位 +数据域位
            byte[] byt_Data = new byte[int_Len];
            byt_Data[0] = 0x81;                                  //81
            byt_Data[1] = Convert.ToByte(this.m_str_ID, 16);     //ID

            byt_Data[2] = 0x05;                                  //SD
            byt_Data[3] = Convert.ToByte(int_Len);               //Len

            byt_Data[4] = byt_Cmd;    //Cmd

            byt_Data[5] = 0xFF;                            //广播指令标志

            byt_Data[6] = Convert.ToByte(this.m_int_Bws);//ListLen

            byte[] tmparrbyte;

            //tmparrbyte = ChangeStringToByte(GetBwListstring(m_int_Channel, m_str_BwSelect));
            tmparrbyte = ChangeStringToByte(m_str_BwSelect);
            Array.Copy(tmparrbyte, 0, byt_Data, 7, tmparrbyte.Length);

            byt_Data[19] = byt_Value;

            Array.Copy(tmparrbyte, 0, byt_Data, 20, tmparrbyte.Length);

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
        private byte[] OrgFrameForComm(byte byt_Cmd, byte byt_Num, byte byt_Value)
        {

            int int_Len = 21;                  //81、ID、SD \len、CMD和Chksum各占一位 +数据域位
            byte[] byt_Data = new byte[int_Len];
            byt_Data[0] = 0x81;                                  //81
            byt_Data[1] = Convert.ToByte(this.m_str_ID, 16);     //ID

            byt_Data[2] = 0x05;                                  //SD
            byt_Data[3] = Convert.ToByte(int_Len);               //Len

            byt_Data[4] = byt_Cmd;    //Cmd

            byt_Data[5] = 0xFF;                            //广播指令标志

            byt_Data[6] = Convert.ToByte(this.m_int_Bws);//ListLen

            byte[] tmparrbyte = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            if (byt_Num == 0xFF)//全表位
            {
                if (byt_Value != 0x03)
                {

                    tmparrbyte = ChangeStringToByte(m_str_BwSelect);
                }
            }
            else if (byt_Num == 0xEE)//偶数
            {
                tmparrbyte = ChangeStringToByte(m_str_BwSelect); //ChangeStringToByte(GetBwListstring(m_int_Channel, 0, Comm.GlobalUnit.strYaoJianMeter));

            }
            else if (byt_Num == 0xDD)//奇数
            {
                tmparrbyte = ChangeStringToByte(m_str_BwSelect); //ChangeStringToByte(GetBwListstring(m_int_Channel, 1, Comm.GlobalUnit.strYaoJianMeter));
            }
            else//单表位
            {
                tmparrbyte = ChangeStringToByte(GetBwListstring(m_int_Channel, byt_Num, m_str_BwSelect));
            }
            Array.Copy(tmparrbyte, 0, byt_Data, 7, tmparrbyte.Length);
            byt_Data[19] = byt_Value;

            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //校验码    校验码从81后面开始计算
            {
                byt_Data[int_Len - 1] ^= byt_Data[int_Inc];               //Chksum
            }
            return byt_Data;

        }


        /// <summary>
        ///  组织帧For读数据
        /// </summary>
        /// <param name="byt_Cmd">控制码</param>
        /// <param name="byt_Value">数据域</param>
        ///  <param name="byt_BwNum">表位号</param>
        /// <returns>返回组好的帧</returns>
        private byte[] OrgFrameForReadWcb(byte byt_Cmd, byte byt_Value, byte byt_BwNum)
        {

            int int_Len = 21;                  //81、ID、SD \len、CMD和Chksum各占一位 +数据域位
            byte[] byt_Data = new byte[int_Len];
            byt_Data[0] = 0x81;                                  //81
            byt_Data[1] = Convert.ToByte(this.m_str_ID, 16);    //ID

            byt_Data[2] = 0x05;                                 //SD
            byt_Data[3] = Convert.ToByte(int_Len);             //  Len

            byt_Data[4] = byt_Cmd;    //Cmd

            byt_Data[5] = 0xFF;                            //广播指令标志

            byt_Data[6] = Convert.ToByte(this.m_int_Bws);//ListLen

            byte[] tmparrbyte;

            tmparrbyte = ChangeStringToByte(GetBwListstring(m_int_Channel, byt_BwNum, m_str_BwSelect));
            // tmparrbyte = ChangeStringToByte(m_str_BwSelect);
            Array.Copy(tmparrbyte, 0, byt_Data, 7, tmparrbyte.Length);

            byt_Data[19] = byt_Value;

            for (int int_Inc = 1; int_Inc < int_Len - 1; int_Inc++)        //校验码    校验码从81后面开始计算
            {
                byt_Data[int_Len - 1] ^= byt_Data[int_Inc];               //Chksum
            }
            return byt_Data;

        }
        #endregion

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
        /// 计算标准脉冲频率
        /// </summary>
        /// <param name="StdPulse"></param>
        /// <param name="_CurP"></param>
        /// <returns></returns>
        private long GetStdPulsePl(long StdPulse, float _CurP)
        {//标准脉冲常数/（3600*1000/P）
            long lngTmp;
            //lngTmp = (StdPulse / (3600 * 1000)) * (long)_CurP;
            lngTmp = Convert.ToInt64(StdPulse / (3600 * 1000 / _CurP));

            return lngTmp;
        }

        #endregion




    }


}
