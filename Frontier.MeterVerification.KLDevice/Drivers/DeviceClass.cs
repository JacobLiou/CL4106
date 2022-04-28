using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;

namespace Frontier.MeterVerification.KLDevice
{
    #region CL188L误差板

    #region CL188L清除表位故障
    /// <summary>
    /// 清除表位状态
    /// </summary>
    internal class CL188L_RequestClearBwStatusPacket : CL188LSendPacket
    {
        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xC2;

        /// <summary>
        /// 清除/不改变状态
        /// </summary>
        private int Clear = 0;

        /// <summary>
        /// 清除表位状态
        /// </summary>
        public CL188L_RequestClearBwStatusPacket()
            : base(true)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bwstatus">表位状态</param>
        /// <param name="seperate">True为清除，False为不改变状态</param>
        public void SetPara(bool[] bwstatus, bool isclear)
        {
            this.Clear = isclear ? 1 : 0;
            this.BwStatus = bwstatus;
        }
        public override string GetPacketName()
        {
            return "CL188L_RequestClearBwStatusPacket";
        }

        /*
         * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 状态类型（1Byte）+ 状态参数（12Byte）。
        */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put(AllFlag);
            buf.Put(0x0c);
            buf.Put(ChannelByte);
            buf.Put(Convert.ToByte(Clear));
            return buf.ToByteArray();
        }

    }
    /// <summary>
    /// 清除故障指令，返回数据包
    /// </summary>
    internal class CL188L_RequestClearBwStatusReplayPacket : ClouRecvPacket_CLT11
    {
        public CL188L_RequestClearBwStatusReplayPacket()
            : base()
        {
        }

        /// <summary>
        /// 返回路数
        /// </summary>
        public byte Pos
        {
            get;
            private set;
        }
        public override string GetPacketName()
        {
            return "CL188L_RequestLinkPacketReplay";
        }

        /// <summary>
        /// 联机结果
        /// </summary>
        public bool LinkOk { get; private set; }

        protected override void ParseBody(byte[] data)
        {
            this.LinkOk = true;

            if (data[0] != 0xc1)
            {
                this.ReciveResult = RecvResult.DataError;
                LinkOk = false;
                return;
            }
            this.Pos = data[1];
            this.ReciveResult = RecvResult.OK;
        }
    }
    #endregion

    #region CL188L CT电流档位选择控制
    /// <summary>
    /// CT电流档位选择控制
    /// </summary>
    internal class CL188L_RequestIChannelControlPacket : CL188LSendPacket
    {
        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xB5;

        /// <summary>
        /// 电流档位,01表示100A档位、02表示2A档位。
        /// </summary>
        private Cus_IChannelType iType;

        public CL188L_RequestIChannelControlPacket(bool[] bwstatus, Cus_IChannelType itype)
            : base(true)
        {
            this.BwStatus = bwstatus;
            this.iType = itype;
        }

        public override string GetPacketName()
        {
            return "CL188L_RequestIChannelControlPacket";
        }

        /*
         * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 档位选择（1Byte）
        */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put(AllFlag);
            buf.Put(0x0C);
            buf.Put(ChannelByte);
            buf.Put(Convert.ToByte((int)iType));
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// CT电流档位选择控制指令，返回数据包
    /// </summary>
    internal class CL188L_RequestIChannelControlReplayPacket : ClouRecvPacket_CLT11
    {
        public CL188L_RequestIChannelControlReplayPacket()
            : base()
        {
        }

        /// <summary>
        /// 返回路数
        /// </summary>
        public byte Pos
        {
            get;
            private set;
        }
        public override string GetPacketName()
        {
            return "CL188L_RequestIChannelControlReplayPacket";
        }

        /// <summary>
        /// 联机结果
        /// </summary>
        public bool LinkOk { get; private set; }

        protected override void ParseBody(byte[] data)
        {
            this.LinkOk = true;

            if (data[0] != 0xB6)
            {
                this.ReciveResult = RecvResult.DataError;
                LinkOk = false;
                return;
            }
            this.Pos = data[1];
            this.ReciveResult = RecvResult.OK;
        }
    }
    #endregion

    #region CL188L 联机操作指令
    /// <summary>
    /// 188联机操作请求包
    /// </summary>
    internal class CL188L_RequestLinkPacket : CL188LSendPacket
    {

        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xC0;

        public CL188L_RequestLinkPacket()
            : base(true)
        {
            this.Pos = 0;
        }

        public CL188L_RequestLinkPacket(bool[] bwstatus)
            : base(true)
        {
            this.Pos = 0;
            this.BwStatus = bwstatus;
        }

        public CL188L_RequestLinkPacket(bool[] bwstatus, int iChannelNo)
            : base(true)
        {
            this.Pos = 0;
            this.ChannelNo = iChannelNo;
            this.BwStatus = bwstatus;
        }

        public override string GetPacketName()
        {
            return "CL188L_RequestReadWcBoardVerPacket";
        }
        /*
         * Data的组织方式为：广播标志(0xFFH) + （1 byte ListLen）  + （12 bytes List）
        */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put(AllFlag);
            buf.Put(0x0C);
            buf.Put(ChannelByte);
            buf.Put(0x00);
            return buf.ToByteArray();
        }

    }
    /// <summary>
    /// 联机指令，返回数据包
    /// </summary>
    internal class CL188L_RequestLinkReplayPacket : ClouRecvPacket_CLT11
    {
        public CL188L_RequestLinkReplayPacket()
            : base()
        {
        }

        /// <summary>
        /// 返回路数
        /// </summary>
        public byte Pos
        {
            get;
            private set;
        }
        public override string GetPacketName()
        {
            return "CL188L_RequestLinkPacketReplay";
        }

        /// <summary>
        /// 联机结果
        /// </summary>
        public bool LinkOk { get; private set; }

        protected override void ParseBody(byte[] data)
        {
            this.LinkOk = true;

            if (data[4] != 0x54)
            {
                this.ReciveResult = RecvResult.DataError;
                LinkOk = false;
                return;
            }
            this.Pos = data[1];
            this.ReciveResult = RecvResult.OK;
        }
    }
    #endregion

    #region CL188L读取10次各类型误差及当前各种状态
    /// <summary>
    /// 读取表位前10次各类型误差及当前各种状态
    /// </summary>
    internal class CL188L_RequestReadBwLast10WcAndStatusPacket : CL188LSendPacket
    {
        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xC3;

        /// <summary>
        /// 误差类型
        /// </summary>
        private int wcBoardQueryType;

        /// <summary>
        /// 此指令与C0H指令的帧格式相同，区别为此指令要求误差板上报前10次误差及当前状态。
        /// </summary>
        /// <param name="bwstatus">表位状态</param>
        /// <param name="wcType">检定误差类型</param>
        public CL188L_RequestReadBwLast10WcAndStatusPacket(bool[] bwstatus, Cus_WuchaType wcType)
            : base(true)
        {
            this.BwStatus = bwstatus;
            this.wcBoardQueryType = (int)wcType;
        }

        public override string GetPacketName()
        {
            return "CL188L_RequestReadBwLast10WcAndStatusPacket";
        }
        /// <summary>
        /// Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List）+ 检定误差类型（1Byte）。
        /// </summary>
        /// <returns></returns>
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put(AllFlag);
            buf.Put(0x0C);
            buf.Put(ChannelByte);
            buf.Put(Convert.ToByte(wcBoardQueryType));
            return buf.ToByteArray();
        }

    }
    internal class CL188L_RequestReadBwLast10WcAndStatusReplyPacket : CL188LRecvPacket
    {
        /// <summary>
        /// 命令码
        /// </summary>
        public byte bCmd { get; private set; }

        /// <summary>
        /// 检定误差类型
        /// </summary>
        public Cus_CheckType CheckType { get; private set; }

        /// <summary>
        /// bytes List
        /// </summary>
        public byte[] BwChannelList { get; private set; }

        /// <summary>
        /// 当前表位编号
        /// </summary>
        public int CurBwNum { get; private set; }

        /// <summary>
        /// 误差次数,即当前误差值是计算得到的第几个误差值
        /// </summary>
        public int wcNum { get; private set; }

        /// <summary>
        /// 误差放大倍数（Bit0~Bit6）
        /// </summary>
        public int openTimes { get; private set; }

        /// <summary>
        /// 误差值
        /// </summary>
        public string[] wcData { get; private set; }

        /// <summary>
        /// 状态类型
        /// </summary>
        public int MeterConst { get; private set; }

        /// <summary>
        /// 电流回路状态,0x00表示第一个电流回路，0x01表示第二个电流回路
        /// </summary>
        public Cus_BothIRoadType iType { get; private set; }

        /// <summary>
        /// 电压回路状态,0x00表示直接接入式电表电压回路选择，0x01表示互感器接入式电表电压回路选择，0x02表示本表位无电表接入
        /// </summary>
        public Cus_BothVRoadType vType { get; private set; }

        /// <summary>
        /// 通讯口状态,0x00表示选择第一路普通485通讯；0x01表示选择第二路普通485通讯；0x02表示选择红外通讯；
        /// </summary>
        public int ConnType { get; private set; }

        /*
        * 状态类型分为四种：接线故障状态（Bit0）、预付费跳闸状态（Bit1）、报警信号状态（Bit2）、对标状态（Bit3）的参数
        * 分别由一个字节中的Bit0、Bit1、Bit2、Bit3标示，为1则表示该表位有故障/跳闸/报警/对标完成，为0则表示正常/正常/正常/未完成对标。
        */
        /// <summary>
        /// 接线故障状态,为true则表示该表位有故障,false为正常
        /// </summary>
        public bool statusTypeIsOnErr_Jxgz { get; private set; }

        /// <summary>
        /// 预付费跳闸状态,为true则表示该表位跳闸,false为正常
        /// </summary>
        public bool statusTypeIsOnErr_Yfftz { get; private set; }

        /// <summary>
        /// 报警信号状态,为true则表示该表位报警,false为正常
        /// </summary>
        public bool statusTypeIsOnErr_Bjxh { get; private set; }

        /// <summary>
        /// 对标状态,为true则表示该表位对标完成,false为未完成对标
        /// </summary>
        public bool statusTypeIsOnOver_Db { get; private set; }

        /*
         * 工作状态：电能误差(Bit0)、需量周期误差（Bit1）、日计时误差（Bit2）、脉冲个数（Bit3）、对标（Bit4）、光电脉冲个数（Bit7）
         * 1Byte中为1表示对应计算功能已启动；为0表示对应计算功能已停止。 
        */
        /// <summary>
        /// 电能误差(Bit0),True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
        /// </summary>
        public bool workStatusIsOn_Dn { get; private set; }

        /// <summary>
        /// 需量周期误差（Bit1）,True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
        /// </summary>
        public bool workStatusIsOn_Xlzq { get; private set; }

        /// <summary>
        /// 日计时误差（Bit2）,True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
        /// </summary>
        public bool workStatusIsOn_Rjs { get; private set; }

        /// <summary>
        /// 脉冲个数（Bit3）,True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
        /// </summary>
        public bool workStatusIsOn_Mcgs { get; private set; }

        /// <summary>
        /// 对标（Bit4）,True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
        /// </summary>
        public bool workStatusIsOn_Db { get; private set; }

        /// <summary>
        /// 光电脉冲个数（Bit7）,True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
        /// </summary>
        public bool workStatusIsOn_Gdmcgs { get; private set; }

        /// <summary>
        /// 系统解析帧  
        /// </summary>
        /// <param name="data"></param>
        ///         
        /*
        * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 检定误差类型（1Byte）+ 当前表位编号（1Byte）
        * +误差次数（1Byte）+ 误差值（4Bytes * 10） + 状态类型（1Byte）+ 电流回路状态（1Byte） + 电压回路状态（1Byte）
        * 通讯口状态（1Byte） + 检定状态（1Byte）+发送标志1+发送标志2。        注：与50H的区别为回复前10次的误差值。
        */
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length < 20) return;
            ByteBuffer buf = new ByteBuffer(data);
            bCmd = buf.Get();                           //命令码
            buf.Get();                                  //广播标志(0xFFH)
            buf.Get();                                  //1 byte ListLen                  
            BwChannelList = buf.GetByteArray(12);       // List
            CheckType = (Cus_CheckType)buf.Get();       //检定误差类型
            CurBwNum = buf.Get();                       //当前表位编号
            wcNum = buf.Get();
            //10次误差值包括误差数值和脉冲两种格式
            for (int i = 0; i < 10; i++)
            {
                byte tmp = buf.Get();
                byte[] wcdata = buf.GetByteArray(3);
                if (tmp == 0)                                       //脉冲
                {
                    wcData[i] = get3ByteValue(wcdata, 0).ToString();   //脉冲个数
                }
                else                                                //误差数值
                {
                    openTimes = ReplaceTargetBit(tmp, 7, false);    //误差放大倍数
                    bool isz = GetbitValue(tmp, 7) == 0;            //误差符号（Bit7）0正误差 1负误差
                    wcData[i] = get3ByteValue(wcdata, 5).ToString();
                    if (!isz) wcData[i] = "-" + wcData[i];
                }
            }

            #region 解析状态类型,为true则表示该表位有故障/跳闸/报警/对标完成，为false则表示正常/正常/正常/未完成对标
            byte statusType = buf.Get();
            //接线故障状态,为true则表示该表位有故障,false为正常
            statusTypeIsOnErr_Jxgz = GetbitValue(statusType, 0) == 1;
            //预付费跳闸状态,为true则表示该表位跳闸,false为正常
            statusTypeIsOnErr_Yfftz = GetbitValue(statusType, 0) == 1;
            //报警信号状态,为true则表示该表位报警,false为正常
            statusTypeIsOnErr_Bjxh = GetbitValue(statusType, 0) == 1;
            //对标状态,为true则表示该表位对标完成,false为未完成对标
            statusTypeIsOnOver_Db = GetbitValue(statusType, 0) == 1;
            #endregion

            iType = (Cus_BothIRoadType)buf.Get();           //电流回路状态
            vType = (Cus_BothVRoadType)buf.Get();           //电压回路状态
            ConnType = buf.Get();                           //通讯口状态

            #region 解析工作状态字节,1表示对应计算功能已启动；为0表示对应计算功能已停止。
            //工作状态（1Byte）
            byte workStatus = buf.Get();
            //电能误差
            workStatusIsOn_Dn = GetbitValue(workStatus, 0) == 1;
            //需量周期误差
            workStatusIsOn_Xlzq = GetbitValue(workStatus, 1) == 1;
            //日计时误差
            workStatusIsOn_Rjs = GetbitValue(workStatus, 2) == 1;
            //脉冲个数
            workStatusIsOn_Mcgs = GetbitValue(workStatus, 3) == 1;
            //对标
            workStatusIsOn_Db = GetbitValue(workStatus, 4) == 1;
            //光电脉冲个数
            workStatusIsOn_Gdmcgs = GetbitValue(workStatus, 7) == 1;
            #endregion
        }
    }
    #endregion

    #region CL188L读取各种类型误差及当前各种状态
    /// <summary>
    /// 读取表位各类型误差及各种状态
    /// </summary>
    internal class CL188L_RequestReadBwWcAndStatusPacket : CL188LSendPacket
    {
        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xC0;

        /// <summary>
        /// 误差类型
        /// </summary>
        private int wcBoardQueryType;

        /// <summary>
        /// 查询误差板当前误差及当前状态指令,C0指令默认查询表位状态。注：此指令只返回当前误差值。
        /// </summary>
        /// <param name="bwstatus">表位状态</param>
        /// <param name="wcType">检定误差类型</param>
        public CL188L_RequestReadBwWcAndStatusPacket(Cus_WuchaType wcType)
            : base(true)
        {
            this.wcBoardQueryType = (int)wcType;
        }

        public override string GetPacketName()
        {
            return "CL188L_RequestReadBwWcAndStatusPacket";
        }

        /// <summary>
        /// Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 检定误差类型（1Byte）。
        /// </summary>
        /// <returns></returns>
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put(AllFlag);
            buf.Put(0x0C);
            buf.Put(ChannelByte);
            buf.Put(Convert.ToByte(wcBoardQueryType));
            return buf.ToByteArray();
        }

    }
    internal class CL188L_RequestReadBwWcAndStatusReplyPacket : CL188LRecvPacket
    {
        /// <summary>
        /// 命令码
        /// </summary>
        public byte bCmd { get; private set; }
        /// <summary>
        /// 检定误差类型
        /// </summary>
        public Cus_CheckType CheckType { get; private set; }
        /// <summary>
        /// bytes List
        /// </summary>
        public byte[] BwChannelList { get; private set; }
        /// <summary>
        /// 当前表位编号
        /// </summary>
        public int CurBwNum { get; private set; }
        /// <summary>
        /// 误差次数,即当前误差值是计算得到的第几个误差值
        /// </summary>
        public int wcNum { get; private set; }

        /// <summary>
        /// 误差放大倍数（Bit0~Bit6）
        /// </summary>
        public int openTimes { get; private set; }

        /// <summary>
        /// 误差值
        /// </summary>
        public string wcData { get; private set; }

        /// <summary>
        /// 状态类型
        /// </summary>
        public int MeterConst { get; private set; }
        /// <summary>
        /// 电流回路状态,0x00表示第一个电流回路，0x01表示第二个电流回路
        /// </summary>
        public Cus_BothIRoadType iType { get; private set; }
        /// <summary>
        /// 电压回路状态,0x00表示直接接入式电表电压回路选择，0x01表示互感器接入式电表电压回路选择，0x02表示本表位无电表接入
        /// </summary>
        public Cus_BothVRoadType vType { get; private set; }
        /// <summary>
        /// 通讯口状态,0x00表示选择第一路普通485通讯；0x01表示选择第二路普通485通讯；0x02表示选择红外通讯；
        /// </summary>
        public int ConnType { get; private set; }

        /*
         * 状态类型分为四种：接线故障状态（Bit0）、预付费跳闸状态（Bit1）、报警信号状态（Bit2）、对标状态（Bit3）的参数
         * 分别由一个字节中的Bit0、Bit1、Bit2、Bit3标示，为1则表示该表位有故障/跳闸/报警/对标完成，为0则表示正常/正常/正常/未完成对标。
        */
        /// <summary>
        /// 接线故障状态,为true则表示该表位有故障,false为正常
        /// </summary>
        public bool statusTypeIsOnErr_Jxgz { get; private set; }

        /// <summary>
        /// 预付费跳闸状态,为true则表示该表位跳闸,false为正常
        /// </summary>
        public bool statusTypeIsOnErr_Yfftz { get; private set; }

        /// <summary>
        /// 报警信号状态,为true则表示该表位报警,false为正常
        /// </summary>
        public bool statusTypeIsOnErr_Bjxh { get; private set; }

        /// <summary>
        /// 对标状态,为true则表示该表位对标完成,false为未完成对标
        /// </summary>
        public bool statusTypeIsOnOver_Db { get; private set; }
        /// <summary>
        /// 温度过高故障状态（false：正常；true：故障）。温度过高时，会自动短接隔离继电器
        /// </summary>
        public bool statusTypeIsOnErr_Temp { get; private set; }
        /// <summary>
        /// 光电信号状态（false：未挂表；true：已挂表）
        /// </summary>
        public bool statusTypeIsOn_HaveMeter { get; private set; }
        /// <summary>
        /// 表位上限限位状态（false：未就位；true：就位）
        /// </summary>
        public bool statusTypeIsOn_PressUpLimit { get; private set; }
        /// <summary>
        /// 表位下限限位状态（false：未就位；true：就位）
        /// </summary>
        public bool statusTypeIsOn_PressDownLimt { get; private set; }

        /*
         * 工作状态：电能误差(Bit0)、需量周期误差（Bit1）、日计时误差（Bit2）、脉冲个数（Bit3）、对标（Bit4）、光电脉冲个数（Bit7）
         * 1Byte中为1表示对应计算功能已启动；为0表示对应计算功能已停止。 
        */
        /// <summary>
        /// 电能误差(Bit0),True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
        /// </summary>
        public bool workStatusIsOn_Dn { get; private set; }

        /// <summary>
        /// 需量周期误差（Bit1）,True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
        /// </summary>
        public bool workStatusIsOn_Xlzq { get; private set; }

        /// <summary>
        /// 日计时误差（Bit2）,True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
        /// </summary>
        public bool workStatusIsOn_Rjs { get; private set; }

        /// <summary>
        /// 脉冲个数（Bit3）,True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
        /// </summary>
        public bool workStatusIsOn_Mcgs { get; private set; }

        /// <summary>
        /// 对标（Bit4）,True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
        /// </summary>
        public bool workStatusIsOn_Db { get; private set; }

        /// <summary>
        /// 光电脉冲个数（Bit7）,True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
        /// </summary>
        public bool workStatusIsOn_Gdmcgs { get; private set; }

        /// <summary>
        /// 系统解析帧  
        /// </summary>
        /// <param name="data"></param>
        ///         
        /*
         Data的组织方式为：
         * 广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 检定误差类型（1Byte） + 当前表位编号（1Byte）
         * +误差次数（1Byte） + 误差值（4Bytes） + 状态类型（1Byte） + 电流回路状态（1Byte） + 电压回路状态（1Byte）
         * + 通讯口状态（1Byte） + 工作状态（1Byte）+发送标志1+发送标志2。
         */
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length < 20) return;
            ByteBuffer buf = new ByteBuffer(data);
            bCmd = buf.Get();                           //命令码
            buf.Get();                                  //广播标志(0xFFH)
            buf.Get();                                  //1 byte ListLen                  
            BwChannelList = buf.GetByteArray(12);       // List
            CheckType = (Cus_CheckType)buf.Get();       //检定误差类型
            CurBwNum = buf.Get();                       //当前表位编号
            wcNum = buf.Get();
            //误差值包括误差数值和脉冲两种格式
            byte tmp = buf.Get();
            byte[] wcdata = buf.GetByteArray(3);
            if (tmp == 0)                                       //脉冲
            {
                wcData = get3ByteValue(wcdata, 0).ToString();   //脉冲个数
            }
            else                                                //误差数值
            {
                openTimes = ReplaceTargetBit(tmp, 7, false);    //误差放大倍数
                bool isz = GetbitValue(tmp, 7) == 0;            //误差符号（Bit7）0正误差 1负误差
                wcData = get3ByteValue(wcdata, 5).ToString();
                if (!isz) wcData = "-" + wcData;
            }

            #region 解析状态类型,为true则表示该表位有故障/跳闸/报警/对标完成，为false则表示正常/正常/正常/未完成对标

            byte statusType = buf.Get();
            //接线故障状态,为true则表示该表位有故障,false为正常
            statusTypeIsOnErr_Jxgz = GetbitValue(statusType, 0) == 1;
            //预付费跳闸状态,为true则表示该表位跳闸,false为正常
            statusTypeIsOnErr_Yfftz = GetbitValue(statusType, 1) == 1;
            //报警信号状态,为true则表示该表位报警,false为正常
            statusTypeIsOnErr_Bjxh = GetbitValue(statusType, 2) == 1;
            //对标状态,为true则表示该表位对标完成,false为未完成对标
            statusTypeIsOnOver_Db = GetbitValue(statusType, 3) == 1;
            // 温度过高故障状态（false：正常；true：故障）。温度过高时，会自动短接隔离继电器        
            statusTypeIsOnErr_Temp = GetbitValue(statusType, 4) == 1;
            //光电信号状态（false：未挂表；true：已挂表）        
            statusTypeIsOn_HaveMeter = GetbitValue(statusType, 5) == 1;
            //表位上限限位状态（false：未就位；true：就位）
            statusTypeIsOn_PressUpLimit = GetbitValue(statusType, 6) == 1;
            //表位下限限位状态（false：未就位；true：就位）
            statusTypeIsOn_PressDownLimt = GetbitValue(statusType, 7) == 1;

            #endregion

            iType = (Cus_BothIRoadType)buf.Get();           //电流回路状态
            vType = (Cus_BothVRoadType)buf.Get();           //电压回路状态
            ConnType = buf.Get();                           //通讯口状态

            #region 解析工作状态字节,1表示对应计算功能已启动；为0表示对应计算功能已停止。
            //工作状态（1Byte）
            byte workStatus = buf.Get();
            //电能误差
            workStatusIsOn_Dn = GetbitValue(workStatus, 0) == 1;
            //需量周期误差
            workStatusIsOn_Xlzq = GetbitValue(workStatus, 1) == 1;
            //日计时误差
            workStatusIsOn_Rjs = GetbitValue(workStatus, 2) == 1;
            //脉冲个数
            workStatusIsOn_Mcgs = GetbitValue(workStatus, 3) == 1;
            //对标
            workStatusIsOn_Db = GetbitValue(workStatus, 4) == 1;
            //光电脉冲个数
            workStatusIsOn_Gdmcgs = GetbitValue(workStatus, 7) == 1;
            #endregion
        }
    }
    #endregion

    #region CL88L读取相应误差板软件版本号
    /// <summary>
    /// 读取相应误差板软件版本号
    /// </summary>
    internal class CL188L_RequestReadWcBoardVerPacket : CL188LSendPacket
    {
        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xC4;

        public CL188L_RequestReadWcBoardVerPacket(bool[] bwstatus)
            : base(true)
        {
            this.BwStatus = bwstatus;
        }

        public override string GetPacketName()
        {
            return "CL188L_RequestReadWcBoardVerPacket";
        }
        /*
         * Data的组织方式为：广播标志(0xFFH) + （1 byte ListLen）  + （12 bytes List）
        */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put(AllFlag);
            buf.Put(0x0C);
            buf.Put(ChannelByte);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 读取误差板软件版本号
    /// </summary>
    internal class CL188L_RequestReadWcBoardVerReplyPacket : CL188LRecvPacket
    {
        /// <summary>
        /// 命令码
        /// </summary>
        public byte bCmd { get; private set; }

        /// <summary>
        /// bytes List
        /// </summary>
        public byte[] BwChannelList { get; private set; }

        /// <summary>
        /// 误差板编号
        /// </summary>
        public int wcbIndex { get; private set; }

        /// <summary>
        /// 误差板软件版本号
        /// </summary>
        public string softVer { get; private set; }

        /// <summary>
        /// 系统解析帧  
        /// </summary>
        /// <param name="data"></param>
        ///         
        /*
         * Data的组织方式为：广播标志(0xFFH) + （1 byte ListLen）  + （12 bytes List）   
         * +误差板编号（1 byte）+  误差板软件版本号（1 byte）。
         * 软件版本号的表示采用BCD编码方式编码，小数点不表示。
         * 	例：如果版本号为1.1，则版本号早数据帧中被表示为  0x11。
         */
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length < 20) return;
            ByteBuffer buf = new ByteBuffer(data);
            bCmd = buf.Get();                           //命令码
            buf.Get();                                  //广播标志(0xFFH)
            buf.Get();                                  //1 byte ListLen                  
            BwChannelList = buf.GetByteArray(12);       // List
            wcbIndex = buf.Get();                       //误差板编号
            softVer = buf.Get().ToString("X2").Insert(1, ".");
        }
    }
    #endregion

    #region CL88L读取温度
    /// <summary>
    /// 读取温度
    /// </summary>
    internal class CL188L_RequestReadBwTemperaturePacket : CL188LSendPacket
    {
        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0x82;
        /// <summary>
        /// 读取类型0，A相温度；1，B相温度；2，C相温度
        /// </summary>
        private int m_intReadType = 0;
        public CL188L_RequestReadBwTemperaturePacket()
            : base(true)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bwstatus">表位状态</param>
        /// <param name="seperate">读取类型0，A相温度；1，B相温度；2，C相温度</param>
        public void SetPara(bool[] bwstatus, int intReadType)
        {
            this.m_intReadType = intReadType;
            this.BwStatus = bwstatus;
        }

        public override string GetPacketName()
        {
            return "CL188L_RequestReadBwTemperaturePacket";
        }
        /*
         * Data的组织方式为：广播标志(0xFFH) + （1 byte ListLen）  + （12 bytes List）
        */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put(AllFlag);
            buf.Put(0x0C);
            buf.Put(ChannelByte);
            buf.Put(Convert.ToByte(m_intReadType));
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 读取温度
    /// </summary>
    internal class CL188L_RequestReadBwTemperatureReplyPacket : CL188LRecvPacket
    {
        /// <summary>
        /// 命令码
        /// </summary>
        public byte bCmd { get; private set; }

        /// <summary>
        /// bytes List
        /// </summary>
        public byte[] BwChannelList { get; private set; }

        /// <summary>
        /// 误差板编号
        /// </summary>
        public int wcbIndex { get; private set; }
        /// <summary>
        /// 是否返回错误 
        /// </summary>
        public bool m_bError { get; private set; }
        /// <summary>
        /// 温度
        /// </summary>
        public string m_strTemperature { get; private set; }

        /// <summary>
        /// 系统解析帧  
        /// </summary>
        /// <param name="data"></param>
        ///         
        /*
         * Data的组织方式为：广播标志(0xFFH) + （1 byte ListLen）  + （12 bytes List）   
         * +误差板编号（1 byte）+  误差板软件版本号（1 byte）。
         * 软件版本号的表示采用BCD编码方式编码，小数点不表示。
         * 	例：如果版本号为1.1，则版本号早数据帧中被表示为  0x11。
         */
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length < 20) return;
            ByteBuffer buf = new ByteBuffer(data);
            bCmd = buf.Get();                           //命令码
            buf.Get();                                  //广播标志(0xFFH)
            buf.Get();                                  //1 byte ListLen                  
            BwChannelList = buf.GetByteArray(12);       // List
            wcbIndex = buf.Get();                       //误差板编号

            if (BitConverter.ToInt32(buf.GetByteArray(4), 0) == 0)
                m_bError = false;
            else
                m_bError = true;
            if (!m_bError)
                m_strTemperature = Convert.ToString(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 1000);
            else
                m_strTemperature = "";
        }
    }
    #endregion

    #region CL188L双回路检定时，选择其中的某一路作为电流的输出回路
    /// <summary>
    /// 用于双回路检定时，选择其中的某一路作为电流的输出回路
    /// </summary>
    internal class CL188L_RequestSelectCheckRoadPacket : CL188LSendPacket
    {
        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xAF;

        /// <summary>
        /// 电流的输出回路
        /// </summary>
        private Cus_BothIRoadType iRoad;

        /// <summary>
        /// 电压回路选择
        /// </summary>
        private Cus_BothVRoadType vRoad;

        public CL188L_RequestSelectCheckRoadPacket()
            : base(false)
        { }

        public CL188L_RequestSelectCheckRoadPacket(bool[] bwstatus)
            : base(false)
        {
            this.iRoad = Cus_BothIRoadType.第一个电流回路;
            this.vRoad = Cus_BothVRoadType.直接接入式;
        }

        /// <summary>
        /// 用于双回路检定时，选择其中的某一路作为电流的输出回路；0x00表示第一个电流回路，0x01表示第二个电流回路，系统默认为第一个电流回路。
        /// 选择电压回路时，0x00表示直接接入式电表电压回路选择，0x01表示互感器接入式电表电压回路选择，0x02表示本表位无电表接入，系统默认为直接接入式电表电压回路。
        /// </summary>
        /// <param name="bwstatus"></param>
        /// <param name="iroad"></param>
        /// <param name="vroad"></param>
        public CL188L_RequestSelectCheckRoadPacket(bool[] bwstatus, Cus_BothIRoadType iroad, Cus_BothVRoadType vroad)
            : base(true)
        {
            this.iRoad = iroad;
            this.vRoad = vroad;
        }

        public override string GetPacketName()
        {
            return "CL188L_RequestSelectCheckRoadPacket";
        }

        /// <summary>
        /// Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List）+ 电流回路路数（1Byte） + 电压回路路数（1Byte）。
        /// </summary>
        /// <returns></returns>
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put(AllFlag);
            buf.Put(0x0C);
            buf.Put(ChannelByte);
            buf.Put(Convert.ToByte((int)iRoad));
            buf.Put(Convert.ToByte((int)vRoad));
            return buf.ToByteArray();
        }
    }

    /// <summary>
    /// 用于双回路检定时，选择其中的某一路作为电流的输出回路指令，返回数据包
    /// </summary>
    internal class CL188L_RequestSelectCheckRoadReplayPacket : ClouRecvPacket_CLT11
    {
        public CL188L_RequestSelectCheckRoadReplayPacket()
            : base()
        {
        }

        /// <summary>
        /// 返回路数
        /// </summary>
        public byte Pos
        {
            get;
            private set;
        }
        public override string GetPacketName()
        {
            return "CL188L_RequestSelectCheckRoadReplayPacket";
        }

        /// <summary>
        /// 联机结果
        /// </summary>
        public bool LinkOk { get; private set; }

        protected override void ParseBody(byte[] data)
        {
            this.LinkOk = true;

            if (data[0] != 0xAE)
            {
                this.ReciveResult = RecvResult.DataError;
                LinkOk = false;
                return;
            }
            this.Pos = data[1];
            this.ReciveResult = RecvResult.OK;
        }
    }
    #endregion

    #region CL188L光点头选择命令
    /// <summary>
    /// 光电头状态选择
    /// 通讯选择： 
    /// 0x00表示选择一对一模式485通讯（默认模式）；
    /// 0X01表示选择奇数表位485通讯；
    /// 0X02表示选择偶数表位485通讯；
    /// 0x03表示选择一对一模式红外通讯；
    /// 0X04表示选择奇数表位红外通讯；
    /// 0X05表示选择偶数表位红外通讯；
    /// 0X06表示选择切换到485总线（电科院协议用）。
    /// </summary>
    internal class CL188L_RequestSelectLightStatusPacket : CL188LSendPacket
    {
        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xAC;

        /// <summary>
        /// 0x00表示选择一对一模式485通讯（默认模式）；0X01表示选择奇数表位485通讯；0X02表示选择偶数表位485通讯；
        /// 0x03表示选择一对一模式红外通讯；0X04表示选择奇数表位红外通讯；0X05表示选择偶数表位红外通讯；0X06表示选择切换到485总线（电科院协议用）。
        /// </summary>
        private Cus_LightSelect selectType;

        public CL188L_RequestSelectLightStatusPacket(bool[] bwstatus)
            : base(true)
        {
            BwStatus = bwstatus;
            this.selectType = Cus_LightSelect.一对一模式485通讯;
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="bwstatus">电表状态</param>
        /// <param name="selecttype">通讯选择</param>
        public void SetPara(bool[] bwstatus, Cus_LightSelect selecttype)
        {
            BwStatus = bwstatus;
            this.selectType = selecttype;
        }

        public override string GetPacketName()
        {
            return "CL188L_RequestSelectLightStatusPacket";
        }

        /*
         * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 通讯选择（1Byte）。
        */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put(AllFlag);
            buf.Put(0x0C);
            buf.Put(ChannelByte);
            buf.Put(Convert.ToByte((int)selectType));
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 光电头状态选择指令，返回数据包
    /// </summary>
    internal class CL188L_RequestSelectLightStatusReplayPacket : ClouRecvPacket_CLT11
    {
        public CL188L_RequestSelectLightStatusReplayPacket()
            : base()
        {
        }

        /// <summary>
        /// 返回路数
        /// </summary>
        public byte Pos
        {
            get;
            private set;
        }
        public override string GetPacketName()
        {
            return "CL188L_RequestSelectLightStatusReplayPacket";
        }

        /// <summary>
        /// 联机结果
        /// </summary>
        public bool LinkOk { get; private set; }

        protected override void ParseBody(byte[] data)
        {
            this.LinkOk = true;

            if (data[0] != 0xAD)
            {
                this.ReciveResult = RecvResult.DataError;
                LinkOk = false;
                return;
            }
            this.Pos = data[1];
            this.ReciveResult = RecvResult.OK;
        }
    }
    #endregion

    #region CL188L设置脉冲通道及检定类型
    /// <summary>
    /// 选择被检脉冲通道及检定类型
    /// </summary>
    internal class CL188L_RequestSelectPulseChannelAndCheckTypePacket : CL188LSendPacket
    {
        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xA7;

        /// <summary>
        /// 电能误差通道号,0P+ 、1P-、 2Q+、 3Q-
        /// </summary>
        private Cus_MeterWcChannelNo wcChannelNo;

        /// <summary>
        /// 光电头选择位,1为感应式脉冲输入，0为电子式脉冲输入
        /// </summary>
        private Cus_PulseType pulseType;

        /// <summary>
        /// 脉冲极性选择(共阳/共阴),0表示公共端输出低电平（共阴），1表示公共端输出高电平（共阳）
        /// </summary>
        private Cus_GyGyType GyGy;

        /// <summary>
        /// 多功能误差通道号,1为日计时脉冲、2为需量脉冲。
        /// </summary>
        private Cus_DgnWcChannelNo dgnWcChannelNo;

        /// <summary>
        /// 检定类型：0x00电能误差、0x01需量误差、0x02日计时误差、0x03走字计数、0x04对标、0x05预付费功能检定。
        /// </summary>
        private Cus_CheckType checkType;

        public CL188L_RequestSelectPulseChannelAndCheckTypePacket()
            : base(true)
        { }


        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="bwstatus">电表状态</param>
        /// <param name="wcchannelno">脉冲通道,0=P+,1=P-,2=Q+,3=Q-,4=需量,5=时钟</param>
        /// <param name="pulsetype">通道类型,0=脉冲盒,1=光电头</param>
        /// <param name="gygy">脉冲类型,0=共阳,1=共阴</param>
        /// <param name="dgnwcchannelno">多功能误差通道号,1=日计时，2=需量脉冲</param>
        /// <param name="checktype">检定类型</param>
        public void SetPara(bool[] bwstatus, Cus_MeterWcChannelNo wcchannelno, Cus_PulseType pulsetype, Cus_GyGyType gygy, Cus_DgnWcChannelNo dgnwcchannelno, Cus_CheckType checktype)
        {
            BwStatus = bwstatus;
            this.wcChannelNo = wcchannelno;
            this.pulseType = pulsetype;
            this.GyGy = gygy;
            this.dgnWcChannelNo = dgnwcchannelno;
            this.checkType = checktype;
        }

        public override string GetPacketName()
        {
            return "CL188L_RequestSelectPulseChannelAndCheckTypePacket";
        }
        /*
         * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 被检脉冲通道号（2Byte）+ 检定类型（1Byte）。
        */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put(AllFlag);
            buf.Put(0x0C);
            buf.Put(ChannelByte);
            //计算第一个字节
            string byte1 = Convert.ToString((int)wcChannelNo, 2).PadLeft(3, '0');
            byte1 = ((int)GyGy).ToString() + ((int)pulseType).ToString() + byte1;
            buf.Put(Str2ToByte(byte1));
            //计算第二个字节
            buf.Put(Convert.ToByte((int)dgnWcChannelNo));
            buf.Put(Convert.ToByte((int)checkType));
            return buf.ToByteArray();
        }
    }

    /// <summary>
    /// 选择被检脉冲通道及检定类型指令，返回数据包
    /// </summary>
    internal class CL188L_RequestSelectPulseChannelAndCheckTypeReplayPacket : ClouRecvPacket_CLT11
    {
        public CL188L_RequestSelectPulseChannelAndCheckTypeReplayPacket()
            : base()
        {
        }

        /// <summary>
        /// 返回路数
        /// </summary>
        public byte Pos
        {
            get;
            private set;
        }
        public override string GetPacketName()
        {
            return "CL188L_RequestSelectPulseChannelAndCheckTypeReplayPacket";
        }

        /// <summary>
        /// 联机结果
        /// </summary>
        public bool LinkOk { get; private set; }

        protected override void ParseBody(byte[] data)
        {
            this.LinkOk = true;

            if (data[0] != 0xA6)
            {
                this.ReciveResult = RecvResult.DataError;
                LinkOk = false;
                return;
            }
            this.Pos = data[1];
            this.ReciveResult = RecvResult.OK;
        }
    }
    #endregion

    #region CL188L隔离表位
    /// <summary>
    /// 故障表位电压电流隔离控制、次级开路试验
    /// </summary>
    internal class CL188L_RequestSeperateBwControlPacket : CL188LSendPacket
    {
        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xB4;

        /// <summary>
        /// 隔离/恢复
        /// </summary>
        private int Seperate = 0;

        /// <summary>
        /// 隔离/恢复,需要发两次指令，先隔离需要隔离的表位，再恢复掉需要恢复的表位
        /// </summary>
        /// <param name="bwstatus">表位状态</param>
        /// <param name="seperate">True为隔离，False为恢复</param>
        public CL188L_RequestSeperateBwControlPacket()
            : base(true)
        {

        }
        public void SetPara(int ErrorNo, bool[] bwstatus, bool seperate)
        {
            this.Seperate = seperate ? 1 : 0;
            ChannelByte = SeperateBwToChannelByte(ErrorNo, bwstatus, seperate);
        }
        public override string GetPacketName()
        {
            return "CL188L_RequestSeperateBwControlPacket";
        }

        /*
         * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List）+隔离控制状态（1Byte）。
        */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put(AllFlag);
            buf.Put(0x0C);
            for (int intInc = 0; intInc < ChannelByte.Length; intInc++)
            {
                //if (ChannelByte[intInc] == 0x00 && intInc == ChannelByte.Length - 1)
                //{
                //    return null;
                //}
                //else
                //    continue;
            }
            buf.Put(ChannelByte);
            buf.Put(Convert.ToByte(Seperate));
            return buf.ToByteArray();
        }

        /// <summary>
        /// 隔离故障表位,恢复正常表位
        /// </summary>
        /// <param name="bwstatus">表位状态</param>
        /// <returns></returns>
        private byte[] SeperateBwToChannelByte(int ErrorNo, bool[] bwstatus, bool seperate)
        {

            string[] str = new string[96];
            string Strtmp = "";
            for (int z = 0; z < 96; z++)
            {
                if (z < bwstatus.Length)
                {

                    if (z <= ((ErrorNo + 1) * 15 - 1) && z >= ((ErrorNo) * 15))
                    {
                        if (!seperate)
                        {
                            if (bwstatus[z])
                            {
                                str[z] = "1";
                            }
                            else
                            { str[z] = "0"; }
                        }
                        else
                        {
                            if (bwstatus[z])
                            {
                                str[z] = "0";
                            }
                            else
                            { str[z] = "1"; }
                        }
                    }
                    else
                    { str[z] = "0"; }
                }
                else
                { str[z] = "0"; }
            }

            for (int k = str.Length - 1; k >= 0; k--)
            {
                Strtmp += str[k];
            }
            byte[] Arrytmpbyte = new byte[12];


            byte tmpbyte = new byte();
            for (int i = 0; i < 12; i++)
            {
                tmpbyte = 0x00;

                for (int ii = 0; ii < 8; ii++)
                {

                    tmpbyte += Convert.ToByte(Strtmp.Substring(Strtmp.Length - 1 - 8 * i - ii, 1).Equals("1") ? (Math.Pow(2, ii)) : 0x00);

                }
                Arrytmpbyte[11 - i] = tmpbyte;
            }
            return Arrytmpbyte;
        }
    }

    /// <summary>
    /// 启动误差板指令，返回数据包
    /// </summary>
    internal class CL188L_RequestSeperateBwControlReplayPacket : ClouRecvPacket_CLT11
    {
        public CL188L_RequestSeperateBwControlReplayPacket()
            : base()
        {
        }

        /// <summary>
        /// 返回路数
        /// </summary>
        public byte Pos
        {
            get;
            private set;
        }
        public override string GetPacketName()
        {
            return "CL188L_RequestSeperateBwControlReplayPacket";
        }

        /// <summary>
        /// 联机结果
        /// </summary>
        public bool LinkOk { get; private set; }

        protected override void ParseBody(byte[] data)
        {
            this.LinkOk = true;

            if (data[0] != 0xB7)
            {
                this.ReciveResult = RecvResult.DataError;
                LinkOk = false;
                return;
            }
            this.Pos = data[1];
            this.ReciveResult = RecvResult.OK;
        }
    }
    #endregion

    #region CL188L设置误差板电机控制命令
    /// <summary>
    /// 设置误差板电机控制指令
    /// </summary>
    internal class CL188L_RequestSetElectromotorPacket : CL188LSendPacket
    {
        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xFC;
        /// <summary>
        /// 电机控制类型 0，电机伸；1，电机缩；2，电机停；
        /// </summary>
        private int StatusType;
        /// <summary>
        /// 设置误差板耐压状态功能指令
        /// </summary>
        /// <param name="bwstatus"></param>
        /// <param name="checktype"></param>
        public CL188L_RequestSetElectromotorPacket()
            : base(true)
        {

        }

        public void SetPara(bool[] bwstatus, int iStatusType)
        {
            this.Pos = 0;
            this.StatusType = iStatusType;
            this.BwStatus = bwstatus;
        }
        public override string GetPacketName()
        {
            return "CL188L_RequestSelectCheckRoadPacket";
        }

        /*
         * Data的组织方式为：广播标志(0XFF) +（1 byte ListLen） + （12 bytes List）+ 状态类型（1Byte）。
        */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put(AllFlag);
            buf.Put(0x0C);
            buf.Put(ChannelByte);
            buf.Put(Convert.ToByte(StatusType));
            return buf.ToByteArray();
        }
    }

    /// <summary>
    /// 误差板控制电机指令，返回数据包
    /// </summary>
    internal class CL188L_RequestSetElectromotorReplayPacket : ClouRecvPacket_CLT11
    {
        public CL188L_RequestSetElectromotorReplayPacket()
            : base()
        {
        }

        /// <summary>
        /// 返回路数
        /// </summary>
        public byte Pos
        {
            get;
            private set;
        }
        public override string GetPacketName()
        {
            return "CL188L_RequestSetElectromotorReplayPacket";
        }

        /// <summary>
        /// 联机结果
        /// </summary>
        public bool LinkOk { get; private set; }

        protected override void ParseBody(byte[] data)
        {
            this.LinkOk = true;

            if (data[0] != 0xFD)
            {
                this.ReciveResult = RecvResult.DataError;
                LinkOk = false;
                return;
            }
            this.Pos = data[1];
            this.ReciveResult = RecvResult.OK;
        }
    }
    #endregion

    #region CL188L设置电能误差检定时脉冲参数
    /// <summary>
    /// 设置电能误差检定时脉冲参数
    /// </summary>
    internal class CL188L_RequestSetPulseParaPacket : CL188LSendPacket
    {
        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xF1;

        /// <summary>
        /// 标准脉冲常数
        /// </summary>
        private int stdMeterConst = 0;

        /// <summary>
        /// 标准脉冲频率
        /// </summary>
        private int stdPulseFreq = 0;

        /// <summary>
        /// 标准脉冲常数缩放倍数
        /// </summary>
        private int stdMeterConstShortTime = 0;

        /// <summary>
        /// 被检脉冲常数
        /// </summary>
        private int meterConst = 0;

        /// <summary>
        /// 校验圈数
        /// </summary>
        private int meterQuans = 0;

        /// <summary>
        /// 被检脉冲常数缩放倍数
        /// </summary>
        private int meterConstShortTime = 0;

        /// <summary>
        /// 发送标志
        /// </summary>
        private byte sendFlag = 0xAA;

        public CL188L_RequestSetPulseParaPacket()
            : base(true)
        { }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="bwstatus">表位状态</param>
        /// <param name="stdmeterconst">标准脉冲常数</param>
        /// <param name="stdpulsefreq">标准脉冲频率</param>
        /// <param name="stdmeterconstshorttime">标准脉冲常数缩放倍数</param>
        /// <param name="meterconst">被检脉冲常数</param>
        /// <param name="meterquans">校验圈数</param>
        /// <param name="meterconstshorttime">被检脉冲常数缩放倍数</param>
        public void SetPara(bool[] bwstatus, int stdmeterconst, int stdpulsefreq, int stdmeterconstshorttime, int meterconst, int meterquans, int meterconstshorttime)
        {
            stdMeterConst = stdmeterconst;
            stdPulseFreq = stdpulsefreq;
            stdMeterConstShortTime = stdmeterconstshorttime;
            meterConst = meterconst;
            meterQuans = meterquans;
            meterConstShortTime = meterconstshorttime;
            this.Pos = 0;
            this.BwStatus = bwstatus;
        }

        public override string GetPacketName()
        {
            return "CL188L_RequestSetPulseParaPacket";
        }

        /*
         * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 标准脉冲常数（4Bytes）+ 
         * 标准脉冲频率（4Bytes）+ 标准脉冲常数缩放倍数（1Bytes）+ 被检脉冲常数（4Bytes） + 校验圈数（4Bytes）+ 被检脉冲常数缩放倍数(1Byte)+发送标志1（1Byte） 。
        */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put(AllFlag);
            buf.Put(0x0C);
            buf.Put(ChannelByte);
            buf.PutInt_S(stdMeterConst);
            buf.PutInt_S(stdPulseFreq);
            buf.Put(Convert.ToByte(stdMeterConstShortTime));
            buf.PutInt_S(meterConst);
            buf.PutInt_S(meterQuans);
            buf.Put(Convert.ToByte(meterConstShortTime));
            buf.Put(Convert.ToByte(sendFlag));
            return buf.ToByteArray();
        }
    }

    /// <summary>
    /// 设置电能误差检定时脉冲参数指令，返回数据包
    /// </summary>
    internal class CL188L_RequestSetPulseParaReplayPacket : ClouRecvPacket_CLT11
    {
        public CL188L_RequestSetPulseParaReplayPacket()
            : base()
        {
        }

        /// <summary>
        /// 返回路数
        /// </summary>
        public byte Pos
        {
            get;
            private set;
        }
        public override string GetPacketName()
        {
            return "CL188L_RequestSetPulseParaReplayPacket";
        }

        /// <summary>
        /// 联机结果
        /// </summary>
        public bool LinkOk { get; private set; }

        protected override void ParseBody(byte[] data)
        {
            this.LinkOk = true;

            if (data[0] != 0xF0)
            {
                this.ReciveResult = RecvResult.DataError;
                LinkOk = false;
                return;
            }
            this.Pos = data[1];
            this.ReciveResult = RecvResult.OK;
        }
    }
    #endregion

    #region CL188L设置日计时误差检定时钟频率及需量周期误差检定时间
    /// <summary>
    /// 设置日计时误差检定时钟频率及需量周期误差检定时间
    /// </summary>
    internal class CL188L_RequestSetTimePluseNumAndXLTimePacket : CL188LSendPacket
    {
        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xF3;

        /// <summary>
        /// 标准时钟频率100倍（4Bytes）
        /// </summary>
        private int stdMeterTimeFreq = 0;

        /// <summary>
        /// 被检时钟频率100倍
        /// </summary>
        private int meterTimeFreq = 0;

        /// <summary>
        /// 被检脉冲个数（4Bytes）
        /// </summary>
        private int meterPulseNum = 0;

        /// <summary>
        /// 发送标志
        /// </summary>
        private byte sendFlag = 0x55;

        public CL188L_RequestSetTimePluseNumAndXLTimePacket()
            : base(false)
        { }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="stdmetertimefreq">标准时钟频率100倍（4Bytes）</param>
        /// <param name="metertimefreq">被检时钟频率100倍</param>
        /// <param name="meterpulsenum">被检脉冲个数(4Bytes)</param>
        public void SetPara(bool[] bwstatus, int stdmetertimefreq, int metertimefreq, int meterpulsenum)
        {
            IsNeedReturn = false;
            BwStatus = bwstatus;
            stdMeterTimeFreq = stdmetertimefreq * 100;
            meterTimeFreq = metertimefreq * 100;
            meterPulseNum = meterpulsenum;
        }

        public override string GetPacketName()
        {
            return "CL188L_RequestSetPulseParaPacket";
        }

        /*
         *Data的组织方式为：广播标志(0XFF) +（1 byte ListLen） + （12 bytes List） + 标准时钟频率100倍（4Bytes）+ 被检时钟频率100倍（4Bytes）+ 被检脉冲个数（4Bytes）+发送标志2（1Byte） 
        */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put(AllFlag);
            buf.Put(0x0C);
            buf.Put(ChannelByte);

            buf.PutInt_S(stdMeterTimeFreq);
            buf.PutInt_S(meterTimeFreq);
            buf.PutInt_S(meterPulseNum);
            buf.Put(Convert.ToByte(sendFlag));
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 设置电能误差检定时脉冲参数指令，返回数据包
    /// </summary>
    internal class CL188L_RequestSetTimePluseNumAndXLTimeReplayPacket : ClouRecvPacket_CLT11
    {
        public CL188L_RequestSetTimePluseNumAndXLTimeReplayPacket()
            : base()
        {
        }

        /// <summary>
        /// 返回路数
        /// </summary>
        public byte Pos
        {
            get;
            private set;
        }
        public override string GetPacketName()
        {
            return "CL188L_RequestSetTimePluseNumAndXLTimeReplayPacket";
        }

        /// <summary>
        /// 联机结果
        /// </summary>
        public bool LinkOk { get; private set; }

        protected override void ParseBody(byte[] data)
        {
            this.LinkOk = true;

            if (data[0] != 0xF2)
            {
                this.ReciveResult = RecvResult.DataError;
                LinkOk = false;
                return;
            }
            this.Pos = data[1];
            this.ReciveResult = RecvResult.OK;
        }
    }
    #endregion

    #region CL188L设置误差板耐压漏电流阀值指令
    /// <summary>
    /// 设置误差板耐压漏电流阀值指令
    /// </summary>
    internal class CL188L_RequestSetWishStandCurrentLimitPacket : CL188LSendPacket
    {
        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xF8;

        /// <summary>
        /// 漏电流阀值
        /// </summary>
        private float CurrentLimit;


        /// <summary>
        /// 设置误差板耐压漏电流阀值指令
        /// </summary>
        /// <param name="bwstatus"></param>
        /// <param name="checktype"></param>
        public CL188L_RequestSetWishStandCurrentLimitPacket()
            : base(true)
        {

        }

        public void SetPara(bool[] bwstatus, float iCurrentLimit)
        {
            this.Pos = 0;
            this.CurrentLimit = iCurrentLimit;
            this.BwStatus = bwstatus;
        }
        public override string GetPacketName()
        {
            return "CL188L_RequestSelectCheckRoadPacket";
        }

        /*
         * Data的组织方式为：广播标志(0XFF) +（1 byte ListLen） + （12 bytes List）+ 漏电流阀值（4Byte）。
        */
        protected override byte[] GetBody()
        {
            //byte[] Blimit = GetBytes(CurrentLimit.ToString(), 4);
            byte[] Blimit = BitConverter.GetBytes(Convert.ToInt32(CurrentLimit)*1000);  
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put(AllFlag);
            buf.Put(0x0C);
            buf.Put(ChannelByte);
            buf.Put(Blimit);
            return buf.ToByteArray();
        }
        public byte[] GetBytes(string x, int index)
        {
            byte[] b = new byte[index];
            byte[] bytes = Encoding.ASCII.GetBytes(x);
            for (int i = 1; i <= bytes.Length; i++, index--)
            {
                b[i] = bytes[i - 1];
            }
            return b;
        }
    }
    /// <summary>
    /// 设置误差板耐压漏电流阀值指令，返回数据包
    /// </summary>
    internal class CL188L_RequestSetWishStandCurrentLimitReplayPacket : ClouRecvPacket_CLT11
    {
        public CL188L_RequestSetWishStandCurrentLimitReplayPacket()
            : base()
        {
        }

        /// <summary>
        /// 返回路数
        /// </summary>
        public byte Pos
        {
            get;
            private set;
        }
        public override string GetPacketName()
        {
            return "CL188L_RequestSetWishStandCurrentLimitReplayPacket";
        }

        /// <summary>
        /// 联机结果
        /// </summary>
        public bool LinkOk { get; private set; }

        protected override void ParseBody(byte[] data)
        {
            this.LinkOk = true;

            if (data[0] != 0xFB)
            {
                this.ReciveResult = RecvResult.DataError;
                LinkOk = false;
                return;
            }
            this.Pos = data[1];
            this.ReciveResult = RecvResult.OK;
        }
    }
    #endregion

    #region CL188L设置误差板耐压状态指令
    /// <summary>
    /// 设置误差板耐压状态功能指令
    /// </summary>
    internal class CL188L_RequestSetWishStandStatusPacket : CL188LSendPacket
    {
        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xBA;
        /// <summary>
        /// 状态类型 0，复位状态；1，控制耐压继电器闭合状态；
        /// </summary>
        private int StatusType;
        /// <summary>
        /// 设置误差板耐压状态功能指令
        /// </summary>
        /// <param name="bwstatus"></param>
        /// <param name="checktype"></param>
        public CL188L_RequestSetWishStandStatusPacket()
            : base(true)
        {

        }

        public void SetPara(bool[] bwstatus, int iStatusType)
        {
            this.Pos = 0;
            this.StatusType = iStatusType;
            this.BwStatus = bwstatus;
        }
        public override string GetPacketName()
        {
            return "CL188L_RequestSelectCheckRoadPacket";
        }

        /*
         * Data的组织方式为：广播标志(0XFF) +（1 byte ListLen） + （12 bytes List）+ 状态类型（1Byte）。
        */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put(AllFlag);
            buf.Put(0x0C);
            buf.Put(ChannelByte);
            buf.Put(Convert.ToByte(StatusType));
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// CL188L设置耐压状态指令，返回数据包
    /// </summary>
    internal class CL188L_RequestSetWishStandStatusReplayPacket : ClouRecvPacket_CLT11
    {
        public CL188L_RequestSetWishStandStatusReplayPacket()
            : base()
        {
        }

        /// <summary>
        /// 返回路数
        /// </summary>
        public byte Pos
        {
            get;
            private set;
        }
        public override string GetPacketName()
        {
            return "CL188L_RequestLinkPacketReplay";
        }

        /// <summary>
        /// 联机结果
        /// </summary>
        public bool LinkOk { get; private set; }

        protected override void ParseBody(byte[] data)
        {
            this.LinkOk = true;

            if (data[0] != 0xBB)
            {
                this.ReciveResult = RecvResult.DataError;
                LinkOk = false;
                return;
            }
            this.Pos = data[1];
            this.ReciveResult = RecvResult.OK;
        }
    }
    #endregion

    #region CL188L启动误差板指令
    /// <summary>
    /// 启动计算功能指令
    /// </summary>
    internal class CL188L_RequestStartPCFunctionPacket : CL188LSendPacket
    {
        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xB1;

        /// <summary>
        /// 检定类型
        /// </summary>
        private Cus_CheckType checkType;


        /// <summary>
        /// 启动计算功能指令，若表位列表中某一位置1则启动对应表位检定，为0则不启动，若List = 0x30H，则只启动第5和第6表位的检定；检定类型设置同A7指令
        /// </summary>
        /// <param name="bwstatus"></param>
        /// <param name="checktype"></param>
        public CL188L_RequestStartPCFunctionPacket()
            : base(true)
        {

        }

        public void SetPara(bool[] bwstatus, Cus_CheckType checktype)
        {
            this.Pos = 0;
            this.checkType = checktype;
            this.BwStatus = bwstatus;
        }
        public override string GetPacketName()
        {
            return "CL188L_RequestSelectCheckRoadPacket";
        }

        /*
         * Data的组织方式为：广播标志(0XFF) +（1 byte ListLen） + （12 bytes List）+ 检定类型（1Byte）。
        */
        protected override byte[] GetBody()
        {
            int iType = (int)checkType;
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put(AllFlag);
            buf.Put(0x0C);
            buf.Put(ChannelByte);
            buf.Put(Convert.ToByte(iType));
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 启动误差板指令，返回数据包
    /// </summary>
    internal class CL188L_RequestStartPCFunctionReplayPacket : ClouRecvPacket_CLT11
    {
        public CL188L_RequestStartPCFunctionReplayPacket()
            : base()
        {
        }

        /// <summary>
        /// 返回路数
        /// </summary>
        public byte Pos
        {
            get;
            private set;
        }
        public override string GetPacketName()
        {
            return "CL188L_RequestStartPCFunctionReplayPacket";
        }

        /// <summary>
        /// 联机结果
        /// </summary>
        public bool LinkOk { get; private set; }

        protected override void ParseBody(byte[] data)
        {
            this.LinkOk = true;

            if (data[0] != 0xF0)
            {
                this.ReciveResult = RecvResult.DataError;
                LinkOk = false;
                return;
            }
            this.Pos = data[1];
            this.ReciveResult = RecvResult.OK;
        }
    }
    #endregion

    #region CL188L停止误差板
    /// <summary>
    /// 停止误差板计算功能指令
    /// </summary>
    internal class CL188L_RequestStopPCFunctionPacket : CL188LSendPacket
    {
        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xB2;

        /// <summary>
        /// 检定类型
        /// </summary>
        private Cus_CheckType checkType;


        public CL188L_RequestStopPCFunctionPacket()
            : base(false)
        { }

        /// <summary>
        /// 停止计算功能指令，若表位列表中某一位置1则停止对应表位检定，为0则不改变，若List = 0x30H，
        /// 则停止第5和第6表位的检定；检定类型设置同A7指令，自动检表线上，下发07指令停止所有的检定。
        /// </summary>
        /// <param name="bwstatus"></param>
        /// <param name="checktype"></param>
        public CL188L_RequestStopPCFunctionPacket(bool[] bwstatus, Cus_CheckType checktype)
            : base(true)
        {
            this.isStart = true;
            this.Pos = 0;
            this.checkType = checktype;
            this.BwStatus = bwstatus;
        }

        public void SetParam(bool[] bwstatus, Cus_CheckType checkType)
        {
            this.checkType = checkType;
            this.BwStatus = bwstatus;
        }

        public override string GetPacketName()
        {
            return "CL188L_RequestStopPCFunctionPacket";
        }

        /*
         * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List）+ 检定类型（1Byte）。
        */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put(AllFlag);
            buf.Put(0x0C);
            buf.Put(ChannelByte);
            buf.Put(Convert.ToByte((int)checkType));
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 启动误差板指令，返回数据包
    /// </summary>
    internal class CL188L_RequestStopPCFunctionReplayPacket : ClouRecvPacket_CLT11
    {
        public CL188L_RequestStopPCFunctionReplayPacket()
            : base()
        {
        }

        /// <summary>
        /// 返回路数
        /// </summary>
        public byte Pos
        {
            get;
            private set;
        }
        public override string GetPacketName()
        {
            return "CL188L_RequestStopPCFunctionReplayPacket";
        }

        /// <summary>
        /// 联机结果
        /// </summary>
        public bool LinkOk { get; private set; }

        protected override void ParseBody(byte[] data)
        {
            this.LinkOk = true;

            if (data[0] != 0xF0)
            {
                this.ReciveResult = RecvResult.DataError;
                LinkOk = false;
                return;
            }
            this.Pos = data[1];
            this.ReciveResult = RecvResult.OK;
        }
    }
    #endregion

    /// <summary>
    /// CL2018误差板数据多线程获取误差数据
    /// 调用方法:
    /// ReadWcbManager readWcb=new ReadWcbManager();
    /// readWcb.WcbChannelCount=4;
    /// readWcb.WcbPerChannelBwCount=6;
    /// readWcb.bSelectBw=bSelectBw;
    /// readWcb.portNum=portNum;
    /// readWcb.m_curTaskType=m_curTaskType;
    /// readWcb.Start();
    /// WaitAllThreaWorkDone();
    /// tagError=readWcb.tagError;
    /// </summary>
    public class ReadWcbManager
    {
        /// <summary>
        /// 最大线程数量
        /// </summary>
        public int WcbChannelCount { get; set; }

        /// <summary>
        /// 每个线程最大任务数
        /// </summary>
        public int WcbPerChannelBwCount { get; set; }

        private bool[] bwStatus;
        /// <summary>
        /// 所有表位状态
        /// </summary>
        public bool[] BwStatus
        {
            get
            {
                return bwStatus;
            }

            set
            {
                bwStatus = value;
            }
        }

        /// <summary>
        /// 误差板端口系列
        /// </summary>
        public int[] portNum { get; set; }

        /// <summary>
        /// 所有误差板数据
        /// </summary>
        public stError[] tagError;

        /// <summary>
        /// 当前试验类型
        /// </summary>
        public enmTaskType m_curTaskType { get; set; }

        /// <summary>
        /// 工作线程数组
        /// </summary>
        private ReadWcbThread[] workThreads = new ReadWcbThread[0];

        /// <summary>
        /// 启动线程
        /// </summary>
        /// <returns>启动线程是否成功</returns>
        public bool Start()
        {
            //结束上一次的线程
            workThreads = new ReadWcbThread[WcbChannelCount];
            tagError = new stError[BwStatus.Length];
            for (int i = 0; i < WcbChannelCount; i++)
            {
                ReadWcbThread newThread = new ReadWcbThread()
                {
                    ThreadID = i + 1,                      //线程编号,用于线程自己推导起始位置
                    ThreadPerCount = WcbPerChannelBwCount,
                    bSelectBw = BwStatus,
                    PortNum = portNum[i],
                    m_curTaskType = m_curTaskType
                };

                newThread.bSelectBw = BwStatus;
                workThreads[i] = newThread;
                newThread.Start();
            }
            return true;
        }

        /// <summary>
        /// 停止所有工作线程
        /// </summary>
        public void Stop()
        {
            //首先发出停止指令
            foreach (ReadWcbThread workthread in workThreads)
            {
                workthread.Stop();
            }
        }

        public void WaitAllThreaWorkDone()
        {
            bool isAllThreaWorkDone = false;
            while (!isAllThreaWorkDone)
            {
                isAllThreaWorkDone = IsWorkDone();
            }
            for (int i = 0; i < workThreads.Length; i++)
            {
                int startpos = i * WcbPerChannelBwCount;
                Array.Copy(workThreads[i].tagError, startpos, tagError, startpos, WcbPerChannelBwCount);
            }
        }

        /// <summary>
        /// 等待所有线程工作完成
        /// </summary>
        public bool IsWorkDone()
        {
            bool isAllThreaWorkDone = true;

            foreach (ReadWcbThread workthread in workThreads)
            {
                isAllThreaWorkDone = workthread.IsWorkFinish();
                if (!isAllThreaWorkDone) break;
            }
            return isAllThreaWorkDone;
        }

    }

    public class ReadWcbThread
    {
        /// <summary>
        /// 当前线程
        /// </summary>
        Thread workThread = null;

        /// <summary>
        /// 运行标志
        /// </summary>
        private bool runFlag = false;

        /// <summary>
        /// 工作完成标志
        /// </summary>
        private bool workOverFlag = false;

        /// <summary>
        /// 每个线程个数
        /// </summary>
        public int ThreadPerCount { get; set; }

        private bool[] bSelectStatus;
        /// <summary>
        /// 所有表位状态
        /// </summary>
        public bool[] bSelectBw
        {
            get
            {
                return bSelectStatus;
            }
            set
            {
                bSelectStatus = value;
            }
        }

        /// <summary>
        /// 误差板端口号
        /// </summary>
        public int PortNum { get; set; }

        /// <summary>
        /// 当前试验类型
        /// </summary>
        public enmTaskType m_curTaskType { get; set; }

        /// <summary>
        /// 线程ID
        /// </summary>
        public int ThreadID { get; set; }

        /// <summary>
        /// 该端口下的误差板数据
        /// </summary>
        public stError[] tagError;

        /// <summary>
        /// 停止当前工作任务
        /// </summary>
        public void Stop()
        {
            runFlag = true;
            workThread.Abort();
        }

        /// <summary>
        /// 工作线程是否完成
        /// </summary>
        /// <returns></returns>
        public bool IsWorkFinish()
        {
            return workOverFlag;
        }

        /// <summary>
        /// 启动工作线程
        /// </summary>
        /// <param name="paras"></param>
        public void Start()
        {
            workThread = new Thread(StartWork);
            workThread.Start();
        }
        private void StartWork()
        {
            //初始化标志
            runFlag = false;
            workOverFlag = false;
            //调用方法
            try
            {
                //计算负载
                int startpos = (ThreadID - 1) * ThreadPerCount;
                int endpos = startpos + ThreadPerCount;
                CL188L_RequestReadBwWcAndStatusPacket rc = new CL188L_RequestReadBwWcAndStatusPacket((Cus_WuchaType)GlobalUnit.g_CurTestType);
                CL188L_RequestReadBwWcAndStatusReplyPacket rcback = new CL188L_RequestReadBwWcAndStatusReplyPacket();
                tagError = new stError[bSelectBw.Length];
                bool[] newSelectBw = new bool[bSelectBw.Length];
                for (int i = startpos; i < endpos; i++)
                {
                    ///假如停止试验,则跳出
                    if (runFlag) return;
                    ///假如不需要检表,则跳出
                    if (!bSelectBw[i]) continue;
                    //重新获取表位状态
                    rc.Pos = i + 1;
                    rc.ChannelNo = ThreadID - 1;
                    rc.ChannelNum = PortNum;
                    //rc.BwStatus = SelectOneBwChannel(bSelectBw, i); 
                    rc.BwStatus = bSelectBw;
                    tagError[i].szError = "";
                    for (int j = 0; j < 3; j++)
                    {
                        if (SendData(PortNum, rc, rcback))
                        {
                            tagError[i].szError = rcback.wcData;
                            tagError[i].Index = rcback.wcNum;
                            tagError[i].MeterConst = rcback.MeterConst;
                            tagError[i].iType = rcback.iType;
                            tagError[i].statusTypeIsOn_HaveMeter = rcback.statusTypeIsOn_HaveMeter;
                            tagError[i].statusTypeIsOn_PressDownLimt = rcback.statusTypeIsOn_PressDownLimt;
                            tagError[i].statusTypeIsOn_PressUpLimit = rcback.statusTypeIsOn_PressUpLimit;
                            tagError[i].statusTypeIsOnErr_Bjxh = rcback.statusTypeIsOnErr_Bjxh;
                            tagError[i].statusTypeIsOnErr_Jxgz = rcback.statusTypeIsOnErr_Jxgz;
                            tagError[i].statusTypeIsOnErr_Temp = rcback.statusTypeIsOnErr_Temp;
                            tagError[i].statusTypeIsOnErr_Yfftz = rcback.statusTypeIsOnErr_Yfftz;
                            tagError[i].statusTypeIsOnOver_Db = rcback.statusTypeIsOnOver_Db;
                            tagError[i].vType = rcback.vType;
                            tagError[i].ConnType = rcback.ConnType;


                            tagError[i].MeterIndex = i;
                            if (m_curTaskType == enmTaskType.需量周期)
                            {
                                tagError[i].Index = (tagError[i].Index + 1) / 10;
                            }
                            break;
                        }
                    }
                }
            }
            catch { }
            finally
            {
                //恢复标志
                runFlag = true;
                workOverFlag = true;
            }
        }

        /// <summary>
        /// 发送误差板端口数据
        /// </summary>
        /// <param name="port"></param>
        /// <param name="sendPacket"></param>
        /// <param name="recvPacket"></param>
        /// <returns></returns>
        private bool SendData(int port, SendPacket sendPacket, RecvPacket recvPacket)
        {
            string portName = GetPortNameByPortNumber(port);

            return SockPool.Instance.Send(portName, sendPacket, recvPacket);
        }

        /// <summary>
        /// 根据端口号获取端口名
        /// </summary>
        /// <param name="port">端口号</param>
        /// <returns>端口名</returns>
        private string GetPortNameByPortNumber(int port)
        {
            return string.Format("Port_{0}", port);
        }

        /// <summary>
        /// 切换到指定表位通道
        /// </summary>
        /// <param name="bwdata"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool[] SelectOneBwChannel(bool[] bwdata, int index)
        {
            for (int i = 0; i < bwdata.Length; i++)
            {
                bwdata[i] = (i == index);
            }
            return bwdata;
        }

    }

    #endregion

    #region CLReversalElectromotor 翻转电机

    #region CLReversalElectromotor 控制翻转
    /// <summary>
    /// 设置翻转电机控制指令
    /// </summary>
    internal class CLReversalElectromotor_RequestSetElectromotorPacket : CLElectromotorSendPacket
    {
        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xFC;
        /// <summary>
        /// 电机控制类型 0，电机伸；1，电机缩；2，电机停；
        /// </summary>
        private int StatusType;
        /// <summary>
        /// 设置指令
        /// </summary>
        /// <param name="bwstatus"></param>
        /// <param name="checktype"></param>
        public CLReversalElectromotor_RequestSetElectromotorPacket()
            : base(true)
        {

        }

        public void SetPara(bool[] bwstatus, int iStatusType)
        {
            this.Pos = 0;
            this.StatusType = iStatusType;
            this.BwStatus = bwstatus;
        }
        public override string GetPacketName()
        {
            return "CLReversalElectromotor_RequestSetElectromotorPacket";
        }

        /*
         * Data的组织方式为：广播标志(0XFF) +（1 byte ListLen） + （12 bytes List）+ 状态类型（1Byte）。
        */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put(AllFlag);
            buf.Put(0x0C);
            buf.Put(ChannelByte);
            buf.Put(Convert.ToByte(StatusType));
            return buf.ToByteArray();
        }
    }

    /// <summary>
    /// 误差板控制电机指令，返回数据包
    /// </summary>
    internal class CLReversalElectromotor_RequestSetElectromotorReplayPacket : ClouRecvPacket_CLT11
    {
        public CLReversalElectromotor_RequestSetElectromotorReplayPacket()
            : base()
        {
        }

        /// <summary>
        /// 返回路数
        /// </summary>
        public byte Pos
        {
            get;
            private set;
        }
        public override string GetPacketName()
        {
            return "CLReversalElectromotor_RequestSetElectromotorReplayPacket";
        }

        /// <summary>
        /// 联机结果
        /// </summary>
        public bool LinkOk { get; private set; }

        protected override void ParseBody(byte[] data)
        {
            this.LinkOk = true;
            if (data[0] != 0xFD)
            {
                this.ReciveResult = RecvResult.DataError;
                LinkOk = false;
                return;
            }
            this.Pos = data[1];
            this.ReciveResult = RecvResult.OK;
        }
    }
    #endregion

    #region CLReversalElectromotor读取各种类型误差及当前各种状态
    /// <summary>
    /// 读取表位各类型误差及各种状态
    /// </summary>
    internal class CLReversalElectromotor_RequestReadBwWcAndStatusPacket : CLElectromotorSendPacket
    {
        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xC0;

        /// <summary>
        /// 误差类型
        /// </summary>
        private int wcBoardQueryType;

        /// <summary>
        /// 查询误差板当前误差及当前状态指令,C0指令默认查询表位状态。注：此指令只返回当前误差值。
        /// </summary>
        /// <param name="bwstatus">表位状态</param>
        /// <param name="wcType">检定误差类型</param>
        public CLReversalElectromotor_RequestReadBwWcAndStatusPacket()
            : base(true)
        {
            this.wcBoardQueryType = 0;
        }

        public override string GetPacketName()
        {
            return "CLReversalElectromotor_RequestReadBwWcAndStatusPacket";
        }

        /// <summary>
        /// Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 检定误差类型（1Byte）。
        /// </summary>
        /// <returns></returns>
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put(AllFlag);
            buf.Put(0x0C);
            if (ChannelByte == null)
                return null;
            buf.Put(ChannelByte);
            buf.Put(Convert.ToByte(wcBoardQueryType));
            return buf.ToByteArray();
        }

    }
    internal class CLReversalElectromotor_RequestReadBwWcAndStatusReplyPacket : CL188LRecvPacket
    {
        /// <summary>
        /// 命令码
        /// </summary>
        public byte bCmd { get; private set; }
        /// <summary>
        /// 检定误差类型
        /// </summary>
        public Cus_CheckType CheckType { get; private set; }
        /// <summary>
        /// bytes List
        /// </summary>
        public byte[] BwChannelList { get; private set; }
        /// <summary>
        /// 当前表位编号
        /// </summary>
        public int CurBwNum { get; private set; }
        /// <summary>
        /// 误差次数,即当前误差值是计算得到的第几个误差值
        /// </summary>
        public int wcNum { get; private set; }

        /// <summary>
        /// 误差放大倍数（Bit0~Bit6）
        /// </summary>
        public int openTimes { get; private set; }

        /// <summary>
        /// 误差值
        /// </summary>
        public string wcData { get; private set; }

        /// <summary>
        /// 状态类型
        /// </summary>
        public int MeterConst { get; private set; }
        /// <summary>
        /// 电流回路状态,0x00表示第一个电流回路，0x01表示第二个电流回路
        /// </summary>
        public Cus_BothIRoadType iType { get; private set; }
        /// <summary>
        /// 电压回路状态,0x00表示直接接入式电表电压回路选择，0x01表示互感器接入式电表电压回路选择，0x02表示本表位无电表接入
        /// </summary>
        public Cus_BothVRoadType vType { get; private set; }
        /// <summary>
        /// 通讯口状态,0x00表示选择第一路普通485通讯；0x01表示选择第二路普通485通讯；0x02表示选择红外通讯；
        /// </summary>
        public int ConnType { get; private set; }

        /*
         * 状态类型分为四种：接线故障状态（Bit0）、预付费跳闸状态（Bit1）、报警信号状态（Bit2）、对标状态（Bit3）的参数
         * 分别由一个字节中的Bit0、Bit1、Bit2、Bit3标示，为1则表示该表位有故障/跳闸/报警/对标完成，为0则表示正常/正常/正常/未完成对标。
        */
        /// <summary>
        /// 接线故障状态,为true则表示该表位有故障,false为正常
        /// </summary>
        public bool statusTypeIsOnErr_Jxgz { get; private set; }

        /// <summary>
        /// 预付费跳闸状态,为true则表示该表位跳闸,false为正常
        /// </summary>
        public bool statusTypeIsOnErr_Yfftz { get; private set; }

        /// <summary>
        /// 报警信号状态,为true则表示该表位报警,false为正常
        /// </summary>
        public bool statusTypeIsOnErr_Bjxh { get; private set; }

        /// <summary>
        /// 对标状态,为true则表示该表位对标完成,false为未完成对标
        /// </summary>
        public bool statusTypeIsOnOver_Db { get; private set; }
        /// <summary>
        /// 温度过高故障状态（false：正常；true：故障）。温度过高时，会自动短接隔离继电器
        /// </summary>
        public bool statusTypeIsOnErr_Temp { get; private set; }
        /// <summary>
        /// 光电信号状态（false：未挂表；true：已挂表）
        /// </summary>
        public bool statusTypeIsOn_HaveMeter { get; private set; }
        /// <summary>
        /// 表位上限限位状态（false：未就位；true：就位）
        /// </summary>
        public bool statusTypeIsOn_PressUpLimit { get; private set; }
        /// <summary>
        /// 表位下限限位状态（false：未就位；true：就位）
        /// </summary>
        public bool statusTypeIsOn_PressDownLimt { get; private set; }

        /*
         * 工作状态：电能误差(Bit0)、需量周期误差（Bit1）、日计时误差（Bit2）、脉冲个数（Bit3）、对标（Bit4）、光电脉冲个数（Bit7）
         * 1Byte中为1表示对应计算功能已启动；为0表示对应计算功能已停止。 
        */
        /// <summary>
        /// 电能误差(Bit0),True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
        /// </summary>
        public bool workStatusIsOn_Dn { get; private set; }

        /// <summary>
        /// 需量周期误差（Bit1）,True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
        /// </summary>
        public bool workStatusIsOn_Xlzq { get; private set; }

        /// <summary>
        /// 日计时误差（Bit2）,True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
        /// </summary>
        public bool workStatusIsOn_Rjs { get; private set; }

        /// <summary>
        /// 脉冲个数（Bit3）,True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
        /// </summary>
        public bool workStatusIsOn_Mcgs { get; private set; }

        /// <summary>
        /// 对标（Bit4）,True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
        /// </summary>
        public bool workStatusIsOn_Db { get; private set; }

        /// <summary>
        /// 光电脉冲个数（Bit7）,True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
        /// </summary>
        public bool workStatusIsOn_Gdmcgs { get; private set; }

        /// <summary>
        /// 系统解析帧  
        /// </summary>
        /// <param name="data"></param>
        ///         
        /*
         Data的组织方式为：
         * 广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 检定误差类型（1Byte） + 当前表位编号（1Byte）
         * +误差次数（1Byte） + 误差值（4Bytes） + 状态类型（1Byte） + 电流回路状态（1Byte） + 电压回路状态（1Byte）
         * + 通讯口状态（1Byte） + 工作状态（1Byte）+发送标志1+发送标志2。
         */
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length < 20) return;
            ByteBuffer buf = new ByteBuffer(data);
            bCmd = buf.Get();                           //命令码
            buf.Get();                                  //广播标志(0xFFH)
            buf.Get();                                  //1 byte ListLen                  
            BwChannelList = buf.GetByteArray(12);       // List
            CheckType = (Cus_CheckType)buf.Get();       //检定误差类型
            CurBwNum = buf.Get();                       //当前表位编号
            wcNum = buf.Get();
            //误差值包括误差数值和脉冲两种格式
            byte tmp = buf.Get();
            byte[] wcdata = buf.GetByteArray(3);
            if (tmp == 0)                                       //脉冲
            {
                wcData = get3ByteValue(wcdata, 0).ToString();   //脉冲个数
            }
            else                                                //误差数值
            {
                openTimes = ReplaceTargetBit(tmp, 7, false);    //误差放大倍数
                bool isz = GetbitValue(tmp, 7) == 0;            //误差符号（Bit7）0正误差 1负误差
                wcData = get3ByteValue(wcdata, 5).ToString();
                if (!isz) wcData = "-" + wcData;
            }

            #region 解析状态类型,为true则表示该表位有故障/跳闸/报警/对标完成，为false则表示正常/正常/正常/未完成对标

            byte statusType = buf.Get();
            //接线故障状态,为true则表示该表位有故障,false为正常
            statusTypeIsOnErr_Jxgz = GetbitValue(statusType, 0) == 1;
            //预付费跳闸状态,为true则表示该表位跳闸,false为正常
            statusTypeIsOnErr_Yfftz = GetbitValue(statusType, 1) == 1;
            //报警信号状态,为true则表示该表位报警,false为正常
            statusTypeIsOnErr_Bjxh = GetbitValue(statusType, 2) == 1;
            //对标状态,为true则表示该表位对标完成,false为未完成对标
            statusTypeIsOnOver_Db = GetbitValue(statusType, 3) == 1;
            // 温度过高故障状态（false：正常；true：故障）。温度过高时，会自动短接隔离继电器        
            statusTypeIsOnErr_Temp = GetbitValue(statusType, 4) == 1;
            //光电信号状态（false：未挂表；true：已挂表）        
            statusTypeIsOn_HaveMeter = GetbitValue(statusType, 5) == 1;
            //表位上限限位状态（false：未就位；true：就位）
            statusTypeIsOn_PressUpLimit = GetbitValue(statusType, 6) == 1;
            //表位下限限位状态（false：未就位；true：就位）
            statusTypeIsOn_PressDownLimt = GetbitValue(statusType, 7) == 1;

            #endregion

            iType = (Cus_BothIRoadType)buf.Get();           //电流回路状态
            vType = (Cus_BothVRoadType)buf.Get();           //电压回路状态
            ConnType = buf.Get();                           //通讯口状态

            #region 解析工作状态字节,1表示对应计算功能已启动；为0表示对应计算功能已停止。
            //工作状态（1Byte）
            byte workStatus = buf.Get();
            //电能误差
            workStatusIsOn_Dn = GetbitValue(workStatus, 0) == 1;
            //需量周期误差
            workStatusIsOn_Xlzq = GetbitValue(workStatus, 1) == 1;
            //日计时误差
            workStatusIsOn_Rjs = GetbitValue(workStatus, 2) == 1;
            //脉冲个数
            workStatusIsOn_Mcgs = GetbitValue(workStatus, 3) == 1;
            //对标
            workStatusIsOn_Db = GetbitValue(workStatus, 4) == 1;
            //光电脉冲个数
            workStatusIsOn_Gdmcgs = GetbitValue(workStatus, 7) == 1;
            #endregion
        }
    }
    #endregion

    #endregion

    #region CL191B时基源

    #region CL191B联机指令
    /// <summary>
    /// 191联机操作请求包
    /// 回复:RequestResultReplayPacket
    /// </summary>
    internal class CL191B_RequestLinkPacket : CL191BSendPacket
    {

        public CL191B_RequestLinkPacket()
            : base()
        {
            ToID = 0xBF;
            MyID = 0x20;
        }

        public override string GetPacketName()
        {
            return "CL191B_RequestLinkPacket";
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            byte[] data = new byte[5] { 0xA3, 0x00, 0x00, 0x00, 0xFF };
            buf.Put(data);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 标准表，联机返回指令
    /// </summary>
    internal class CL191B_RequestLinkReplyPacket : CL309RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 1)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x30)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #region CL191B读取GPS时间
    /// <summary>
    /// 191读取GPS时间请求包
    /// 回复:CL191B_RequestReadGPSTimePacket
    /// </summary>
    internal class CL191B_RequestReadGPSTimePacket : CL191BSendPacket
    {

        public CL191B_RequestReadGPSTimePacket()
            : base()
        {
            ToID = 0xBF;
            MyID = 0x20;
        }

        public override string GetPacketName()
        {
            return "CL191B_RequestLinkPacket";
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            byte[] data = new byte[4] { 0xA0, 00, 00, 00 };
            buf.Put(data);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 读取GPS时间回复处理
    /// </summary>
    internal class CL191B_RequestReadGPSTimeReplayPacket : ClouRecvPacket_CLT11
    {
        /// <summary>
        /// GPS时间
        /// </summary>
        public DateTime GPSTime
        {
            get;
            private set;
        }

        public override string GetPacketName()
        {
            return "CL191B_RequestReadGPSTimeReplayPacket";
        }

        protected override void ParseBody(byte[] data)
        {
            ByteBuffer buf = new ByteBuffer(data);
            buf.GetInt();//0x50, 00 01 00
            //后面9个字节是YYYY MMMM DDDD HH MM SS
            GPSTime = new DateTime(buf.GetUShort_S(), buf.GetUShort_S(), buf.GetUShort_S(), buf.Get(), buf.Get(), buf.Get());
        }
    }
    #endregion

    #region CL191B读取GPS温度湿度请求包
    /// <summary>
    /// 读取GPS温度湿度请求包
    /// </summary>
    internal class CL191B_RequestReadTemperatureAndHumidityPacket : CL191BSendPacket
    {

        public CL191B_RequestReadTemperatureAndHumidityPacket()
            : base()
        {
            ToID = 0xBF;
            MyID = 0x20;

        }
        public override string GetPacketName()
        {
            return "CL191B_RequestReadTemperatureAndHumidityPacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            byte[] data = new byte[4] { 0xA0, 00, 03, 00 };
            buf.Put(data);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 读取GPS温度湿度回复包
    /// </summary>
    internal class CL191B_RequestReadTemperatureAndHumidityReplayPacket : ClouRecvPacket_CLT11
    {
        /// <summary>
        /// 温度
        /// </summary>
        public float Tempututer { get; private set; }
        /// <summary>
        /// 湿度
        /// </summary>
        public float Humidity { get; private set; }

        protected override void ParseBody(byte[] data)
        {
            try
            {
                this.Tempututer = BitConverter.ToInt16(data, 0) / 100;
                this.Humidity = BitConverter.ToInt16(data, 2) / 100;
            }
            catch
            {
                this.ReciveResult = RecvResult.DataError;
            }
        }
    }
    #endregion

    #region CL191B设置通道命令
    /// <summary>
    /// 设置191通道请求包
    /// </summary>
    internal class CL191B_RequestSetChannelPacket : CL191BSendPacket
    {
        public CL191B_RequestSetChannelPacket()
            : base()
        {
            ToID = 0xBF;
            MyID = 0x20;

        }
        /// <summary>
        /// 通道类型[0xFF为标准电能脉冲，00为时间脉冲]
        /// </summary>
        public enmStdPulseType channelType = enmStdPulseType.标准电能脉冲;

        public void SetPara(enmStdPulseType Type)
        {
            this.channelType = Type;
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xa3);
            buf.Put(0);
            buf.Put(0);
            buf.Put(0);
            if (channelType == enmStdPulseType.标准电能脉冲)
                buf.Put(0xFF);
            else if (channelType == enmStdPulseType.标准时钟脉冲)
                buf.Put(0x00);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// cl191b设置通道，联机返回指令
    /// </summary>
    internal class CL191B_RequestSetChannelReplyPacket : CL309RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 1)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x30)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #endregion

    #region CL2029B多功能控制板

    #region CL2029B时序板 联机指令
    /// <summary>
    /// 2029B联机/脱机请求包
    /// </summary>
    internal class CL2029B_RequestLinkPacket : CL2029BSendPacket
    {
        public bool IsLink = true;

        public CL2029B_RequestLinkPacket()
            : base(false)
        { }

        /*
         * 81 30 PCID 08 C1 01 00 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);          //命令 
            buf.Put(0x02);
            buf.Put(0x01);
            buf.Put(0x01);
            buf.Put(0x01);
            buf.Put(0x02);

            return buf.ToByteArray();
        }
    }

    /// <summary>
    /// 2029B时序板，联机返回指令
    /// </summary>
    internal class Cl2029B_RequestLinkReplyPacket : CL2029BRecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 3)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0xA0)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #region CL2029B设置警示灯命令
    /// <summary>
    /// 2029B设置警示灯请求包
    /// </summary>
    internal class CL2029B_RequestSetLightPacket : CL2029BSendPacket
    {
        public bool IsLink = true;
        /// <summary>
        /// 警示灯类型
        /// </summary>
        private int iLightType = 0;

        public CL2029B_RequestSetLightPacket()
            : base(false)
        { }

        public void SetPara(int iType)
        {
            this.iLightType = iType;
        }
        /*
         * 81 42 PCID 0B A3 02 01 01 0x xx CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);          //命令 
            buf.Put(0x02);
            buf.Put(0x01);
            buf.Put(0x01);
            buf.Put(0x01);
            buf.Put(Convert.ToByte(iLightType));

            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 2029B时序板设置警示灯返回指令
    /// </summary>
    internal class Cl2029B_RequestSetLightReplyPacket : CL2029BRecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 3)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0xA0)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #region CL2029B控制装置供电命令
    /// <summary>
    /// 2029B控制装置供电请求包
    /// </summary>
    internal class CL2029B_RequestControlPowerPacket : CL2029BSendPacket
    {
        public bool IsLink = true;
        /// <summary>
        /// 是否供电
        /// </summary>
        private bool bPowerType = false;

        public CL2029B_RequestControlPowerPacket()
            : base(false)
        { }

        public void SetPara(bool bType)
        {
            this.bPowerType = bType;
        }
        /*
         * 81 42 PCID 0B A3 01 01 02 0x xx CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);          //命令 
            buf.Put(0x01);
            buf.Put(0x01);
            buf.Put(0x01);
            buf.Put(0x02);
            if (bPowerType)
                buf.Put(0x03);
            else
                buf.Put(0x00);

            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 2029B时序板控制装置供电返回指令
    /// </summary>
    internal class CL2029B_RequestControlPowerReplyPacket : CL2029BRecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 3)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0xA0)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #endregion

    #region CL2029D继电器切换板

    #region CL2029D切换继电器命令
    /// <summary>
    /// 2029D切换继电器请求包
    /// </summary>
    internal class CL2029D_RequestSetSwitchPacket : CL2029DSendPacket
    {
        public bool IsLink = true;
        /// <summary>
        /// 继电器ID
        /// </summary>
        private int iRelayID = 0;
        /// <summary>
        /// 控制类型0,断开；1，闭合
        /// </summary>
        private int iControlType = 0;

        public CL2029D_RequestSetSwitchPacket()
            : base(true)
        { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iID">继电器ID</param>
        /// <param name="iType">继电器控制类型0，断开；1，闭合</param>
        public void SetPara(int iID, int iType)
        {
            this.iRelayID = iID;
            this.iControlType = iType;
        }
        /*
         * 81 22 01 16 84 FF 0C FF FF FF FF FF FF FF FF FF FF FF FF 00 00 CS
         */
        protected override byte[] GetBody()
        {
            byte[] byt_List = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x84);          //命令 
            buf.Put(0xFF);
            buf.Put(0x0C);
            buf.Put(byt_List);

            buf.Put(Convert.ToByte(iRelayID));
            buf.Put(Convert.ToByte(iControlType));

            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 2029D多功能控制板切换继电器返回指令
    /// </summary>
    internal class CL2029D_RequestSetSwitchReplyPacket : CL2029DRecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 19)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x85)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #endregion

    #region CL2030CT档位切换器

    #region CL2030CT档位控制器清除过载信号
    /// <summary>
    /// 2030CT档位控制器清除过载信号请求包
    /// </summary>
    internal class CL2030_RequestClearOverPacket : CL2030SendPacket
    {
        public bool IsLink = true;

        public CL2030_RequestClearOverPacket()
            : base()
        { }

        /*
         * 81 30 PCID 08 C1 01 00 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xC1);          //命令 
            buf.Put(0x01);
            buf.Put(0x00);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 2030CT档位控制器，清除过载信号返回指令
    /// </summary>
    internal class CL2030_RequestClearOverReplyPacket : CL2030RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 3)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0xA1)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #region CL2030CT档位控制器联机指令
    /// <summary>
    /// 2030CT档位控制器联机/脱机请求包
    /// </summary>
    internal class CL2030_RequestLinkPacket : CL2030SendPacket
    {
        public bool IsLink = true;

        public CL2030_RequestLinkPacket()
            : base()
        { }

        /*
         * 81 30 PCID 08 C1 01 00 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xC1);          //命令 
            buf.Put(0x01);
            buf.Put(0x00);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 2030CT档位控制器，联机返回指令
    /// </summary>
    internal class Cl2030_RequestLinkReplyPacket : CL2030RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 3)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0xA1)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #region CL2030CT档位控制器设置接线方式
    /// <summary>
    /// 2030CT档位控制器设置接线方式请求包
    /// </summary>
    internal class CL2030_RequestSetLinkTypePacket : CL2030SendPacket
    {
        public bool IsLink = true;
        public CL2030_RequestSetLinkTypePacket()
            : base()
        { }

        /*
         * 81 30 PCID 08 C0 01 00 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xC5);          //命令 
            buf.Put(0x01);
            if (GlobalUnit.Clfs == Cus_Clfs.三相三线)
                buf.Put(0x33);
            else
                buf.Put(0x34);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 2030CT档位控制器，切换档位返回指令
    /// </summary>
    internal class CL2030_RequestSetLinkTypeReplyPacket : CL2030RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 3)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0xA5)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #region CL2030CT档位控制器设置档位请求包
    /// <summary>
    /// 2030CT档位控制器设置档位请求包
    /// </summary>
    internal class CL2030_RequestSwitchPacket : CL2030SendPacket
    {
        public bool IsLink = true;

        /// <summary>
        /// CT档位类型  0x00,100A档位；0x01，2A档位
        /// </summary>
        private byte byt_CurrentType = 0;

        public CL2030_RequestSwitchPacket()
            : base()
        { }

        public void SetPara(float fCurrent)
        {
            if (fCurrent > 2)
                byt_CurrentType = 0x00;
            else
                byt_CurrentType = 0x01;
        }
        /*
         * 81 30 PCID 08 C0 01 00 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xC3);          //命令 
            buf.Put(0x01);
            buf.Put(byt_CurrentType);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 2030CT档位控制器，切换档位返回指令
    /// </summary>
    internal class CL2030_RequestSwitchReplyPacket : CL2030RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 3)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0xA3)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #endregion

    #region CL2038A耐压仪

    #region CL2038A耐压仪控制命令
    /// <summary>
    /// 2038A耐压仪控制命令请求包
    /// </summary>
    internal class CL2038A_RequestControlPacket : CL2038ASendPacket
    {
        public bool IsLink = true;
        /// <summary>
        /// 控制类型true，开源；false，关源；
        /// </summary>
        private bool _ControlType = false;
        public CL2038A_RequestControlPacket()
        //: base()
        { }

        public void SetPara(bool bControlType)
        {
            _ControlType = bControlType;
        }
        /*
         * 81 01 50 Flen A3 11 02 01 控制数据 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);          //命令 
            buf.Put(0x11);
            buf.Put(0x02);
            buf.Put(0x01);
            if (_ControlType)
                buf.Put(0x01);
            else
                buf.Put(0x00);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 2038A耐压仪控制器，控制命令返回指令
    /// </summary>
    internal class CL2038A_RequestControlReplyPacket : CL2038ARecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 3)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0xA5)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #region CL2038A 耐压仪设置阀值
    /// <summary>
    /// 2038A耐压仪设置阀值请求包
    /// </summary>
    internal class CL2038A_RequestSetThresholdValuePacket : CL2038ASendPacket
    {
        public bool IsLink = true;
        /// <summary>
        /// 阀值
        /// </summary>
        private float _ThresholdValue = 0.0f;
        public CL2038A_RequestSetThresholdValuePacket()
            : base()
        { }

        public void SetPara(float fThresholdValue)
        {
            _ThresholdValue = fThresholdValue;
        }
        /*
         * 81 30 PCID 08 C0 01 00 CS
         */
        protected override byte[] GetBody()
        {
            byte[] byt_ThresholdValue = new byte[4];
            byte[] byt_ThresholdValue1 = new byte[4];
            //byt_ThresholdValue = BitConverter.GetBytes(_VoltageValue);

            byt_ThresholdValue = BitConverter.GetBytes(_ThresholdValue);
            for (int int_Inc = 0; int_Inc < byt_ThresholdValue.Length; int_Inc++)
            {
                byt_ThresholdValue1[int_Inc] = byt_ThresholdValue[byt_ThresholdValue.Length - 1 - int_Inc];
            }


            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);          //命令 
            buf.Put(0x11);
            buf.Put(0x01);
            buf.Put(0x08);
            buf.Put(byt_ThresholdValue1);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 2038A耐压仪控制器，设置阀值返回指令
    /// </summary>
    internal class CL2038A_RequestSetThresholdValueReplyPacket : CL2038ARecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 3)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0xA3)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #region CL2038A 耐压仪设置耐压电压
    /// <summary>
    /// 2038A耐压仪设置耐压电压请求包
    /// </summary>
    internal class CL2038A_RequestSetVoltagePacket : CL2038ASendPacket
    {
        public bool IsLink = true;
        /// <summary>
        /// 电压
        /// </summary>
        private float _VoltageValue = 0.0f;
        public CL2038A_RequestSetVoltagePacket()
            : base()
        { }

        public void SetPara(float fVoltageValue)
        {
            _VoltageValue = fVoltageValue;
        }
        /*
         * 81 30 PCID 08 A3 11 01 02 电压 CS
         */
        protected override byte[] GetBody()
        {
            byte[] byt_ThresholdValue = new byte[4];
            byte[] byt_ThresholdValue1 = new byte[4];
            //byt_ThresholdValue = BitConverter.GetBytes(_VoltageValue);

            byt_ThresholdValue = BitConverter.GetBytes(_VoltageValue);
            for (int int_Inc = 0; int_Inc < byt_ThresholdValue.Length; int_Inc++)
            {
                byt_ThresholdValue1[int_Inc] = byt_ThresholdValue[byt_ThresholdValue.Length - 1 - int_Inc];
            }


            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);          //命令 
            buf.Put(0x11);
            buf.Put(0x01);
            buf.Put(0x02);
            buf.Put(byt_ThresholdValue1);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 2038A耐压仪控制器，设置电压返回指令
    /// </summary>
    internal class CL2038A_RequestSetVoltageReplyPacket : CL2038ARecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 3)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0xA3)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #region CL2038A 耐压仪设置耐压时间
    /// <summary>
    /// 2038A耐压仪设置耐压时间请求包
    /// </summary>
    internal class CL2038A_RequestSetTimePacket : CL2038ASendPacket
    {
        public bool IsLink = true;
        /// <summary>
        /// 时间
        /// </summary>
        private float _TimeValue = 0.0f;
        public CL2038A_RequestSetTimePacket()
            : base()
        { }

        public void SetPara(float fTimeValue)
        {
            _TimeValue = fTimeValue;
        }
        /*
         * 81 30 PCID 08 A3 11 02 04 时间 CS
         */
        protected override byte[] GetBody()
        {
            byte[] byt_ThresholdValue = new byte[4];
            byte[] byt_ThresholdValue1 = new byte[4];
            //byt_ThresholdValue = BitConverter.GetBytes(_VoltageValue);

            byt_ThresholdValue = BitConverter.GetBytes(Convert.ToInt32(_TimeValue));
            for (int int_Inc = 0; int_Inc < byt_ThresholdValue.Length; int_Inc++)
            {
                byt_ThresholdValue1[int_Inc] = byt_ThresholdValue[byt_ThresholdValue.Length - 1 - int_Inc];
            }



            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);          //命令 
            buf.Put(0x11);
            buf.Put(0x02);
            buf.Put(0x04);
            buf.Put(byt_ThresholdValue1);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 2038A耐压仪控制器，设置时间返回指令
    /// </summary>
    internal class CL2038A_RequestSetTimeReplyPacket : CL2038ARecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 3)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0xA3)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #region CL2038A 耐压仪复位状态
    /// <summary>
    /// 2038A耐压仪复位状态请求包
    /// </summary>
    internal class CL2038A_RequestResetStatusPacket : CL2038ASendPacket
    {
        public bool IsLink = true;

        public CL2038A_RequestResetStatusPacket()
            : base()
        { }

        /*
         * 81 30 PCID 08 A3 11 02 02 01 CS
         */
        protected override byte[] GetBody()
        {
            byte[] byt_ThresholdValue = new byte[4];
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);          //命令 
            buf.Put(0x11);
            buf.Put(0x02);
            buf.Put(0x02);
            buf.Put(0x01);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 2038A耐压仪控制器，复位状态返回指令
    /// </summary>
    internal class CL2038A_RequestResetStatusReplyPacket : CL2038ARecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 3)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0xA3)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #region CL2038A 耐压仪读取频率
    /// <summary>
    /// 2038A耐压仪读取频率请求包
    /// </summary>
    internal class CL2038A_RequestReadFreqPacket : CL2038ASendPacket
    {
        public bool IsLink = true;

        public CL2038A_RequestReadFreqPacket()
            : base()
        { }

        /*
         * 81 30 PCID 08 50 11 01 01 00 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x50);          //命令 
            buf.Put(0x11);
            buf.Put(0x01);
            buf.Put(0x01);
            buf.Put(0x00);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 2038A耐压仪控制器，读取频率返回指令
    /// </summary>
    internal class CL2038A_RequestReadFreqReplyPacket : CL2038ARecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 3)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0xA3)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #region CL2038A 耐压仪读取电流
    /// <summary>
    /// 2038A耐压仪读取电流请求包
    /// </summary>
    internal class CL2038A_RequestReadCurrentPacket : CL2038ASendPacket
    {
        public bool IsLink = true;

        public CL2038A_RequestReadCurrentPacket()
            : base()
        { }

        /*
         * 81 30 PCID 08 50 11 01 04 00 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x50);          //命令 
            buf.Put(0x11);
            buf.Put(0x01);
            buf.Put(0x04);
            buf.Put(0x00);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 2038A耐压仪控制器，读取电流返回指令
    /// </summary>
    internal class CL2038A_RequestReadCurrentReplyPacket : CL2038ARecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 3)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0xA3)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #region CL2038A 耐压仪读取状态
    /// <summary>
    /// 2038A耐压仪读取状态请求包
    /// </summary>
    internal class CL2038A_RequestReadStatusPacket : CL2038ASendPacket
    {
        public bool IsLink = true;

        public CL2038A_RequestReadStatusPacket()
            : base()
        { }

        /*
         * 81 30 PCID 08 50 11 02 02 00 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x50);          //命令 
            buf.Put(0x11);
            buf.Put(0x02);
            buf.Put(0x02);
            buf.Put(0x00);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 2038A耐压仪控制器，读取状态返回指令
    /// </summary>
    internal class CL2038A_RequestReadStatusReplyPacket : CL2038ARecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 3)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0xA3)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #region CL2038A 耐压仪读取电压
    /// <summary>
    /// 2038A耐压仪读取电压请求包
    /// </summary>
    internal class CL2038A_RequestReadVoltagePacket : CL2038ASendPacket
    {
        public bool IsLink = true;

        public CL2038A_RequestReadVoltagePacket()
            : base()
        { }

        /*
         * 81 30 PCID 08 50 11 01 02 00 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x50);          //命令 
            buf.Put(0x11);
            buf.Put(0x01);
            buf.Put(0x02);
            buf.Put(0x00);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 2038A耐压仪控制器，读取电压返回指令
    /// </summary>
    internal class CL2038A_RequestReadVoltageReplyPacket : CL2038ARecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 3)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0xA3)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #endregion

    #region CL309功率源

    #region CL309源联机指令
    /// <summary>
    /// 源联机/脱机请求包
    /// </summary>
    internal class CL309_RequestLinkPacket : CL309SendPacket
    {
        public bool IsLink = true;

        public CL309_RequestLinkPacket()
            : base()
        { }

        /*
         * 81 01 PCID 06 C9 00 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xC9);  //命令           
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 源联机 返回指令
    /// </summary>
    internal class Cl309_RequestLinkReplyPacket : CL309RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 36)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x39)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #region CL309关源指令
    /// <summary>
    /// 控制源 请求包
    /// </summary>
    internal class CL309_RequestPowerOffPacket : CL309SendPacket
    {
        public bool IsLink = true;

        //存储 电压电流
        public static float oldUa = 0.00F;
        public static float oldUb = 0.00F;
        public static float oldUc = 0.00F;
        public static float oldIa = 0.00F;
        public static float oldIb = 0.00F;
        public static float oldIc = 0.00F;


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


        private double m_UaXwValue = 0;//Ua相位角度
        private double m_UbXwValue = 240;//Ub相位角度
        private double m_UcXwValue = 120;//UC相位角度
        private double m_IaXwValue = 0;//IA相位角度
        private double m_IbXwValue = 240;
        private double m_IcXwValue = 120;

        private float _Ua;
        private float _Ub;
        private float _Uc;

        private float _Ia;
        private float _Ib;
        private float _Ic;

        private float _PhiUa;
        private float _PhiUb;
        private float _PhiUc;

        private float _PhiIa;
        private float _PhiIb;
        private float _PhiIc;

        private float _Hz;

        private int _iChange = 0x00;

        private bool _bCurrentDB;

        public CL309_RequestPowerOffPacket()
            : base()
        { }

        public void SetPara()
        {
            SetPara(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 50, 7, 0x00, false, false, false, false);
        }

        public void SetPara(float Ua, float Ub, float Uc, float Ia, float Ib, float Ic, float PhiUa, float PhiUb, float PhiUc,
            float PhiIa, float PhiIb, float PhiIc, float Hz, int iClfs, byte Xwkz, bool XieBo, bool bDBOut, bool bHuanJ, bool bBHuan)
        {
            //Ia 是否变化， 
            if (oldIa == Ia)
            {
                _iChange += 0x00;
            }
            else
            {
                _iChange += DATA5;
                oldIa = Ia;
            }
            //Ib 是否变化， 
            if (oldIb == Ib)
            {
                _iChange += 0x00;
            }
            else
            {
                _iChange += DATA4;
                oldIb = Ib;
            }
            //Ic 是否变化， 
            if (oldIc == Ic)
            {
                _iChange += 0x00;
            }
            else
            {
                _iChange += DATA3;
                oldIc = Ic;
            }

            //Ua 是否变化， 
            if (oldUa == Ua)
            {
                _iChange += 0x00;
            }
            else
            {
                _iChange += DATA2;
                oldUa = Ua;
            }
            //Ub 是否变化， 
            if (oldUb == Ub)
            {
                _iChange += 0x00;
            }
            else
            {
                _iChange += DATA1;
                oldUb = Ub;
            }
            //Uc 是否变化， 
            if (oldUc == Uc)
            {
                _iChange += 0x00;
            }
            else
            {
                _iChange += DATA0;
                oldUc = Uc;
            }

            _Ua = Ua;
            _Ub = Ub;
            _Uc = Uc;

            _Ia = Ia;
            _Ib = Ib;
            _Ic = Ic;

            _PhiIa = PhiIa;
            _PhiIb = PhiIb;
            _PhiIc = PhiIc;
            _PhiUa = PhiUa;
            _PhiUb = PhiUb;
            _PhiUc = PhiUc;
            _Hz = Hz;

            _bCurrentDB = bDBOut;
        }

        public void SetPara(float U, float I, string str_Glys, float Hz, int iClfs, byte Xwkz, bool XieBo, bool bDBOut,
            bool bHuanJ, bool bBHuan, Cus_PowerYuanJiang HABC)
        {
            float m_UaValue = 0f;
            float m_UbValue = 0f;
            float m_UcValue = 0f;
            float m_IaValue = 0f;
            float m_IbValue = 0f;
            float m_IcValue = 0f;

            //r_Glys = "-1.0";            
            switch (HABC)
            {

                case Cus_PowerYuanJiang.H:
                    m_UaValue = U;
                    m_UbValue = U;
                    m_UcValue = U;
                    m_IaValue = I;
                    m_IbValue = I;
                    m_IcValue = I;
                    break;
                case Cus_PowerYuanJiang.A:
                    m_UaValue = U;
                    m_UbValue = U;
                    m_UcValue = U;
                    m_IaValue = I;
                    m_IbValue = 0;
                    m_IcValue = 0;
                    break;
                case Cus_PowerYuanJiang.B:
                    m_UaValue = U;
                    m_UbValue = U;
                    m_UcValue = U;
                    m_IaValue = 0;
                    m_IbValue = I;
                    m_IcValue = 0;
                    break;
                case Cus_PowerYuanJiang.C:
                    m_UaValue = U;
                    m_UbValue = U;
                    m_UcValue = U;
                    m_IaValue = 0;
                    m_IbValue = 0;
                    m_IcValue = I;
                    break;
            }
            //m_UaXwValue = 0;
            //m_UbXwValue = 240;
            //m_UcXwValue = 120;
            //m_IaXwValue = 0;
            //m_IbXwValue = 240;
            //m_IcXwValue = 120;
            //m_FreqValue = 50;

            // 三相四线有功 = 0,
            //三相四线无功 = 1,
            //三相三线有功 = 2,
            //三相三线无功 = 3,
            //二元件跨相90 = 4,
            //二元件跨相60 = 5,
            //三元件跨相90 = 6,

            switch (iClfs)
            {

                case 0:  //三相四线有功表
                    m_UaXwValue = 0;
                    m_UbXwValue = 240;
                    m_UcXwValue = 120;
                    m_IaXwValue = 0;
                    m_IbXwValue = 240;
                    m_IcXwValue = 120;
                    break;
                case 2:  //三相三线有功表
                    m_UbValue = 0;
                    m_UaXwValue = 30;
                    m_UbXwValue = 0;
                    m_UcXwValue = 90;
                    m_IaXwValue = 0;
                    m_IbXwValue = 0;
                    m_IcXwValue = 120;
                    break;
                case 1: //三相四线真无功表(QT4)
                    m_UaXwValue = 0;
                    m_UbXwValue = 240;
                    m_UcXwValue = 120;
                    m_IaXwValue = 270;
                    m_IbXwValue = 150;
                    m_IcXwValue = 30;
                    break;
                case 3: //三相三线真无功表(Q32)
                    m_UbValue = 0;
                    m_UaXwValue = 30;
                    m_UbXwValue = 0;
                    m_UcXwValue = 90;

                    m_IaXwValue = 270;
                    m_IbXwValue = 0;
                    m_IcXwValue = 30;
                    break;
                case 6: //三元件跨相90无功表(Q33)
                    m_UaXwValue = 30;
                    m_UbXwValue = 270;
                    m_UcXwValue = 150;

                    m_IaXwValue = 270;
                    m_IbXwValue = 150;
                    m_IcXwValue = 30;
                    break;
                case 4: //二元件跨相90无功表(Q90)
                    m_UbValue = 0;
                    m_UaXwValue = 30;
                    m_UbXwValue = 0;
                    m_UcXwValue = 270;

                    m_IaXwValue = 270;
                    m_IbXwValue = 0;
                    m_IcXwValue = 30;
                    break;
                case 5: //二元件跨相60无功表(Q60)
                    m_UbValue = 0;
                    m_UaXwValue = 0;
                    m_UbXwValue = 0;
                    m_UcXwValue = 120;

                    m_IaXwValue = 270;
                    m_IbXwValue = 0;
                    m_IcXwValue = 30;
                    break;
                case 7: //单相表
                    m_UbValue = 0;
                    m_UcValue = 0;
                    m_UaXwValue = 0;
                    m_IaXwValue = 0;
                    m_UbXwValue = m_UaXwValue;
                    m_UcXwValue = m_UaXwValue;
                    m_IbXwValue = m_IaXwValue;
                    m_IcXwValue = m_IaXwValue;
                    break;
            }


            SetAcSourcePowerFactor(str_Glys, (byte)iClfs, false);

            SetPara(m_UaValue, m_UbValue, m_UcValue, m_IaValue, m_IbValue, m_IcValue, (float)m_UaXwValue, (float)m_UbXwValue, (float)m_UcXwValue,
            (float)m_IaXwValue, (float)m_IbXwValue, (float)m_IcXwValue, Hz, iClfs, Xwkz, XieBo, bDBOut, bHuanJ, bBHuan);

        }
        /// <summary>
        /// 设置源功率因数
        /// </summary>
        /// <param name="Glys">功率因数如(0.5L,1.0,-1,-0,0.5C)</param>
        /// <param name="jxfs">0-三相四线有功表PT4;1-三相三线有功表P32;2--三相四线真无功表(QT4);3--三相三线真无功表(Q32);4--三元件跨相90无功表(Q33);5--二元件跨相90无功表(Q90);6--二元件人工中点(60)无功表(Q60)</param>
        /// <returns></returns>
        public bool SetAcSourcePowerFactor(string Glys, byte jxfs, bool PH)
        {
            //jxfs 0-三相四线有功表；1-三相三线有功表;2--三相四线真无功表(QT4);3--三相三线真无功表(Q32);
            //4--三元件跨相90无功表(Q33);5--二元件跨相90无功表(Q90);6--二元件人工中点(60)无功表(Q60);

            double XwUa = 0;
            double XwUb = 0;
            double XwUc = 0;
            double XwIa = 0;
            double XwIb = 0;
            double XwIc = 0;
            string strGlys = "";
            string LC = "";
            double LcValue = 0;
            double Phi = 0;
            int n = 1;
            strGlys = Glys;

            if (jxfs == 0)// 三相四线有功 = 0,
            { jxfs = 0; }
            else if (jxfs == 1)//三相四线无功 = 1,
            { jxfs = 2; }
            else if (jxfs == 2)//三相三线有功 = 2,
            { jxfs = 1; }
            else if (jxfs == 3)//三相三线无功 = 3,
            { jxfs = 3; }
            else if (jxfs == 4)//二元件跨相90 = 4,
            { jxfs = 5; }
            else if (jxfs == 5)//二元件跨相60 = 5,
            { jxfs = 6; }
            else if (jxfs == 6)//三元件跨相90 = 6,
            { jxfs = 4; }
            else if (jxfs == 7)
            { jxfs = 7; }


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

            switch (jxfs)
            {
                case 0:  //三相四线有功表
                    Phi = Math.Round(Math.Acos(LcValue) * (180 / Math.PI), 5);
                    XwUa = 0;
                    XwUb = 240;
                    XwUc = 120;
                    if (LcValue > 0)
                    {
                        XwIa = 0;
                        XwIb = 240;
                        XwIc = 120;
                        Phi = 1 * Phi;

                    }
                    else if (LcValue < 0)
                    {
                        XwIa = 180;
                        XwIb = 60;
                        XwIc = 300;
                        Phi = -1 * Phi;
                    }
                    if (LC == "L")
                    {
                        Phi = 1 * Phi;
                        XwIa = XwUa - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = XwUb - Phi;
                        if (XwIb < 0) XwIb = XwIb + 360;
                        if (XwIb >= 360) XwIb = XwIb - 360;

                        XwIc = XwUc - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;

                    }
                    if (LC == "C")
                    {
                        Phi = -1 * Phi;
                        XwIa = XwUa - Phi;

                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = XwUb - Phi;
                        if (XwIb < 0) XwIb = XwIb + 360;
                        if (XwIb >= 360) XwIb = XwIb - 360;

                        XwIc = XwUc - Phi;

                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    break;
                case 1:  //三相三线有功表
                    Phi = Math.Round(Math.Acos(LcValue) * (180 / Math.PI), 5);
                    XwUa = 30;
                    XwUb = 0;
                    XwUc = 90;
                    if (LcValue > 0)
                    {
                        XwIa = 0;
                        XwIb = 0;
                        XwIc = 120;
                        Phi = 1 * Phi;
                    }
                    else if (LcValue < 0)
                    {

                        XwIa = 180;
                        XwIb = 0;
                        XwIc = 240;
                        Phi = -1 * Phi;
                    }
                    if (LC == "L")
                    {
                        Phi = 1 * Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + (360);
                        if (XwIa >= 360) XwIa = XwIa - (360);
                        XwIb = 0;
                        XwIc = 120 - Phi;

                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    if (LC == "C")
                    {
                        Phi = -1 * Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = 0;

                        XwIc = 120 - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    if (PH == false)
                    {
                        XwIa = XwIa + 30;
                        XwIc = XwIc + 30;
                    }
                    break;
                case 2: //三相四线真无功表(QT4)
                    XwUa = 0;
                    XwUb = 240;
                    XwUc = 120;
                    if (LcValue > 0)
                    {
                        XwIa = 270;
                        XwIb = 150;
                        XwIc = 30;
                        n = 1;
                    }
                    else if (LcValue < 0)
                    {

                        XwIa = 90;
                        XwIb = 330;
                        XwIc = 210;
                        n = -1;
                    }
                    if (LC == "L")
                    {
                        if (n == -1) Phi = (-180) - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        else Phi = Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = Phi;
                        XwIa = XwUa - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = XwUb - Phi;
                        if (XwIb < 0) XwIb = XwIb + 360;
                        if (XwIb >= 360) XwIb = XwIb - 360;

                        XwIc = XwUc - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    if (LC == "C")
                    {
                        if (n == -1) Phi = (-180) - (180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5));
                        else Phi = 180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = n + Phi;
                        XwIa = XwUa - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = XwUb - Phi;
                        if (XwIb < 0) XwIb = XwIb + 360;
                        if (XwIb >= 360) XwIb = XwIb - 360;

                        XwIc = XwUc - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    break;
                case 3: //三相三线真无功表(Q32)
                    XwUa = 30;
                    XwUb = 0;
                    XwUc = 90;

                    if (LcValue > 0)
                    {
                        XwIa = 270;
                        XwIb = 0;
                        XwIc = 30;
                        n = 1;
                    }
                    else if (LcValue < 0)
                    {

                        XwIa = 90;
                        XwIb = 0;
                        XwIc = 210;
                        n = -1;
                    }

                    if (LC == "L")
                    {
                        if (n == -1) Phi = (-180) - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        else Phi = Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = 0;


                        XwIc = 120 - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    if (LC == "C")
                    {
                        if (n == -1) Phi = (-180) - (180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5));
                        else Phi = 180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = n + Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = 0;


                        XwIc = 120 - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    if (PH == false)
                    {
                        XwIa = XwIa + 30;
                        XwIc = XwIc - 30;
                    }
                    break;
                case 4: //三元件跨相90无功表(Q33)
                    XwUa = 30;
                    XwUb = 270;
                    XwUc = 150;
                    if (LcValue > 0)
                    {
                        XwIa = 270;
                        XwIb = 150;
                        XwIc = 30;
                        n = 1;
                    }
                    else if (LcValue < 0)
                    {

                        XwIa = 90;
                        XwIb = 330;
                        XwIc = 210;
                        n = -1;
                    }

                    if (LC == "L")
                    {
                        if (n == -1) Phi = (-180) - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        else Phi = Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = 240 - Phi;
                        if (XwIb < 0) XwIb = XwIb + 360;
                        if (XwIb >= 360) XwIb = XwIb - 360;

                        XwIc = 120 - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    if (LC == "C")
                    {
                        if (n == -1) Phi = (-180) - (180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5));
                        else Phi = 180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = n + Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = 240 - Phi;
                        if (XwIb < 0) XwIb = XwIb + 360;
                        if (XwIb >= 360) XwIb = XwIb - 360;

                        XwIc = 120 - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    break;
                case 5: //二元件跨相90无功表(Q90)
                    XwUa = 30;
                    XwUb = 0;
                    XwUc = 270;

                    if (LcValue > 0)
                    {
                        XwIa = 270;
                        XwIb = 0;
                        XwIc = 30;
                        n = 1;
                    }
                    else if (LcValue < 0)
                    {

                        XwIa = 90;
                        XwIb = 0;
                        XwIc = 210;
                        n = -1;
                    }

                    if (LC == "L")
                    {
                        if (n == -1) Phi = (-180) - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        else Phi = Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = 0;


                        XwIc = 120 - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    if (LC == "C")
                    {
                        if (n == -1) Phi = (-180) - (180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5));
                        else Phi = 180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = n + Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = 0;


                        XwIc = 120 - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    break;
                case 6: //二元件跨相60无功表(Q60)
                    XwUa = 0;
                    XwUb = 0;
                    XwUc = 120;

                    if (LcValue > 0)
                    {
                        XwIa = 270;
                        XwIb = 0;
                        XwIc = 30;
                        n = 1;
                    }
                    else if (LcValue < 0)
                    {

                        XwIa = 90;
                        XwIb = 0;
                        XwIc = 210;
                        n = -1;
                    }
                    if (LC == "L")
                    {
                        if (n == -1) Phi = (-180) - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        else Phi = Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = 0;


                        XwIc = 120 - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    if (LC == "C")
                    {
                        if (n == -1) Phi = (-180) - (180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5));
                        else Phi = 180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = n + Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = 0;


                        XwIc = 120 - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    break;
                case 7: //单相表
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
                    XwUb = XwUa;
                    XwUc = XwUa;
                    XwIb = XwIa;
                    XwIc = XwIa;
                    break;
            }
            //tmpOk = SetAcSourcePhasic(XwUa, XwUb, XwUc, XwIa, XwIb, XwIc);
            m_UaXwValue = XwUa;
            m_UbXwValue = XwUb;
            m_UcXwValue = XwUc;

            m_IaXwValue = XwIa;
            m_IbXwValue = XwIb;
            m_IcXwValue = XwIc;

            return true;
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
        /*
         * 81 30 PCID 09 a0 02 02 40 CS
         */
        protected override byte[] GetBody()
        {
            int tmpvalue = 0;
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();

            byte[] byt_Value = new byte[67];

            byt_Value[0] = PAGE5;

            byt_Value[1] = GROUP1 + GROUP2 + GROUP6;

            //GROUP1 
            byt_Value[2] = DATA0 + DATA1 + DATA2 + DATA3 + DATA4 + DATA5; //len = 8          

            tmpvalue = (int)(_PhiUc * JIAODU_BEISHU);//电压角度 C-B-A
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 3, 4);

            tmpvalue = (int)(_PhiUb * JIAODU_BEISHU);//
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 7, 4);

            tmpvalue = (int)(_PhiUa * JIAODU_BEISHU);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 11, 4);

            tmpvalue = (int)(_PhiIc * JIAODU_BEISHU);//电流角度 C-B-A
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 15, 4);

            tmpvalue = (int)(_PhiIb * JIAODU_BEISHU);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 19, 4);

            tmpvalue = (int)(_PhiIa * JIAODU_BEISHU);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 23, 4);



            //GROUP2
            byt_Value[27] = DATA0 + DATA1 + DATA2 + DATA3 + DATA4 + DATA5 + DATA6 + DATA7; //len = 33

            tmpvalue = (int)(_Uc * 10000);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 28, 4);
            byt_Value[32] = (byte)Convert.ToSByte(-4);

            tmpvalue = (int)(_Ub * 10000);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 33, 4);
            byt_Value[37] = (byte)Convert.ToSByte(-4);

            tmpvalue = (int)(_Ua * 10000);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 38, 4);
            byt_Value[42] = (byte)Convert.ToSByte(-4);

            tmpvalue = (int)(_Ic * 1000000);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 43, 4);
            byt_Value[47] = (byte)Convert.ToSByte(-6);

            tmpvalue = (int)(_Ib * 1000000);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 48, 4);
            byt_Value[52] = (byte)Convert.ToSByte(-6);

            tmpvalue = (int)(_Ia * 1000000);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 53, 4);
            byt_Value[57] = (byte)Convert.ToSByte(-6);

            //

            tmpvalue = (int)(_Hz * 10000);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 58, 4); ; //

            byt_Value[62] = DATA0 + DATA1 + DATA2; //

            byt_Value[63] = DATA0 + DATA1 + DATA2;//

            byt_Value[64] = DATA0 + DATA1 + DATA2 + DATA3 + DATA4 + DATA5 + DATA6;
            //byt_Value[64] = //
            byt_Value[65] = Convert.ToByte(_iChange);// DATA0 + DATA1 + DATA2 + DATA3 + DATA4 + DATA5;// +DATA6; //

            if (_bCurrentDB)
            {
                byt_Value[66] = 0;//电流对标
            }
            else
            {
                byt_Value[66] = 0x0;//不对标
            }
            buf.Put(0xA3);
            buf.Put(byt_Value);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 关源 返回指令
    /// </summary>
    internal class Cl309_RequestPowerOffReplyPacket : CL309RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 1)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x30)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #region CL309升源指令
    /// <summary>
    /// 控制源 请求包
    /// </summary>
    internal class CL309_RequestPowerOnPacket : CL309SendPacket
    {
        public bool IsLink = true;

        #region //存储 电压电流
        public static double oldUa = 0.00F;
        public static double oldUb = 0.00F;
        public static double oldUc = 0.00F;
        public static double oldIa = 0.00F;
        public static double oldIb = 0.00F;
        public static double oldIc = 0.00F;
        public static double oldPhia = 0.00F;
        public static double oldPhib = 0.00F;
        public static double oldPhic = 0.00F;


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

        private UIPara m_UIpara;

        private PhiPara m_Phipara;

        private float _Hz;

        private int _iChange = 0x00;

        private bool _bCurrentDB;

        public CL309_RequestPowerOnPacket()
            : base()
        { }


        public void SetPara(UIPara uipara, PhiPara phipara, Cus_PowerYuanJiang HABC, string str_Glys, float Hz, int iClfs, byte Xwkz, 
            bool XieBo, bool bDBOut, bool bHuanJ, bool bBHuan)
        {
            m_UIpara = uipara;
            m_Phipara = phipara;
            _Hz = Hz;

            _bCurrentDB = bDBOut;

            //Ia 是否变化， 
            if (oldIa == uipara.Ia)
            {
                _iChange += 0x00;
            }
            else
            {
                _iChange += DATA5;
                oldIa = uipara.Ia;
            }
            //Ib 是否变化， 
            if (oldIb == uipara.Ib)
            {
                _iChange += 0x00;
            }
            else
            {
                _iChange += DATA4;
                oldIb = uipara.Ib;
            }
            //Ic 是否变化， 
            if (oldIc == uipara.Ic)
            {
                _iChange += 0x00;
            }
            else
            {
                _iChange += DATA3;
                oldIc = uipara.Ic;
            }

            //Ua 是否变化， 
            if (oldUa == uipara.Ua)
            {
                _iChange += 0x00;
            }
            else
            {
                _iChange += DATA2;
                oldUa = uipara.Ua;
            }
            //Ub 是否变化， 
            if (oldUb == uipara.Ub)
            {
                _iChange += 0x00;
            }
            else
            {
                _iChange += DATA1;
                oldUb = uipara.Ub;
            }
            //Uc 是否变化， 
            if (oldUc == uipara.Uc)
            {
                _iChange += 0x00;
            }
            else
            {
                _iChange += DATA0;
                oldUc = uipara.Uc;
            }
            int iYuanjian = (int)HABC - 1;
            SetAcSourcePowerFactor(str_Glys, (byte)iClfs, true, iYuanjian);
        }
        public void SetPara(float U, float I, string str_Glys, float Hz, int iClfs, byte Xwkz, bool XieBo, bool bDBOut,
            bool bHuanJ, bool bBHuan, Cus_PowerYuanJiang HABC)
        {
            //r_Glys = "-1.0";            
            switch (HABC)
            {

                case Cus_PowerYuanJiang.H:
                    m_UIpara.Ua = U;
                    m_UIpara.Ub = U;
                    m_UIpara.Uc = U;
                    m_UIpara.Ia = I;
                    m_UIpara.Ib = I;
                    m_UIpara.Ic = I;
                    break;
                case Cus_PowerYuanJiang.A:
                    m_UIpara.Ua = U;
                    m_UIpara.Ub = U;
                    m_UIpara.Uc = U;
                    m_UIpara.Ia = I;
                    m_UIpara.Ib = 0;
                    m_UIpara.Ic = 0;
                    break;
                case Cus_PowerYuanJiang.B:
                    m_UIpara.Ua = U;
                    m_UIpara.Ub = U;
                    m_UIpara.Uc = U;
                    m_UIpara.Ia = 0;
                    m_UIpara.Ib = I;
                    m_UIpara.Ic = 0;
                    break;
                case Cus_PowerYuanJiang.C:
                    m_UIpara.Ua = U;
                    m_UIpara.Ub = U;
                    m_UIpara.Uc = U;
                    m_UIpara.Ia = 0;
                    m_UIpara.Ib = 0;
                    m_UIpara.Ic = I;
                    break;
            }
            //m_UaXwValue = 0;
            //m_UbXwValue = 240;
            //m_UcXwValue = 120;
            //m_IaXwValue = 0;
            //m_IbXwValue = 240;
            //m_IcXwValue = 120;
            //m_FreqValue = 50;

            // 三相四线有功 = 0,
            //三相四线无功 = 1,
            //三相三线有功 = 2,
            //三相三线无功 = 3,
            //二元件跨相90 = 4,
            //二元件跨相60 = 5,
            //三元件跨相90 = 6,

            switch (iClfs)
            {

                case 0:  //三相四线有功表
                    m_Phipara.PhiUa = 0;
                    m_Phipara.PhiUb = 240;
                    m_Phipara.PhiUc = 120;
                    m_Phipara.PhiIa = 0;
                    m_Phipara.PhiIb = 240;
                    m_Phipara.PhiIc = 120;
                    break;
                case 2:  //三相三线有功表
                    m_UIpara.Ub = 0;
                    m_Phipara.PhiUa = 30;
                    m_Phipara.PhiUb = 0;
                    m_Phipara.PhiUc = 90;
                    m_Phipara.PhiIa = 0;
                    m_Phipara.PhiIb = 0;
                    m_Phipara.PhiIc = 120;
                    break;
                case 1: //三相四线真无功表(QT4)
                    m_Phipara.PhiUa = 0;
                    m_Phipara.PhiUb = 240;
                    m_Phipara.PhiUc = 120;
                    m_Phipara.PhiIa = 270;
                    m_Phipara.PhiIb = 150;
                    m_Phipara.PhiIc = 30;
                    break;
                case 3: //三相三线真无功表(Q32)
                    m_UIpara.Ub = 0;
                    m_Phipara.PhiUa = 30;
                    m_Phipara.PhiUb = 0;
                    m_Phipara.PhiUc = 90;

                    m_Phipara.PhiIa = 270;
                    m_Phipara.PhiIb = 0;
                    m_Phipara.PhiIc = 30;
                    break;
                case 6: //三元件跨相90无功表(Q33)
                    m_Phipara.PhiUa = 30;
                    m_Phipara.PhiUb = 270;
                    m_Phipara.PhiUc = 150;

                    m_Phipara.PhiIa = 270;
                    m_Phipara.PhiIb = 150;
                    m_Phipara.PhiIc = 30;
                    break;
                case 4: //二元件跨相90无功表(Q90)
                    m_UIpara.Ub = 0;
                    m_Phipara.PhiUa = 30;
                    m_Phipara.PhiUb = 0;
                    m_Phipara.PhiUc = 270;

                    m_Phipara.PhiIa = 270;
                    m_Phipara.PhiIb = 0;
                    m_Phipara.PhiIc = 30;
                    break;
                case 5: //二元件跨相60无功表(Q60)
                    m_UIpara.Ub = 0;
                    m_Phipara.PhiUa = 0;
                    m_Phipara.PhiUb = 0;
                    m_Phipara.PhiUc = 120;

                    m_Phipara.PhiIa = 270;
                    m_Phipara.PhiIb = 0;
                    m_Phipara.PhiIc = 30;
                    break;
                case 7: //单相表
                    m_UIpara.Ub = 0;
                    m_UIpara.Uc = 0;
                    m_Phipara.PhiUa = 0;
                    m_Phipara.PhiIa = 0;
                    m_Phipara.PhiUb = m_Phipara.PhiUa;
                    m_Phipara.PhiUc = m_Phipara.PhiUa;
                    m_Phipara.PhiIb = m_Phipara.PhiIa;
                    m_Phipara.PhiIc = m_Phipara.PhiIa;
                    break;
            }

            int iYuanjian = (int)HABC - 1;
            SetAcSourcePowerFactor(str_Glys, (byte)iClfs, false, iYuanjian);

            SetPara(m_UIpara, m_Phipara, HABC, str_Glys, Hz, iClfs, Xwkz, XieBo, bDBOut, bHuanJ, bBHuan);

        }
        /// <summary>
        /// 设置源功率因数
        /// </summary>
        /// <param name="Glys">功率因数如(0.5L,1.0,-1,-0,0.5C)</param>
        /// <param name="jxfs">0-三相四线有功表PT4;1-三相三线有功表P32;2--三相四线真无功表(QT4);3--三相三线真无功表(Q32);4--三元件跨相90无功表(Q33);5--二元件跨相90无功表(Q90);6--二元件人工中点(60)无功表(Q60)</param>
        /// <returns></returns>
        public bool SetAcSourcePowerFactor(string Glys, byte jxfs, bool PH, int iYuanjian)
        {
            //jxfs 0-三相四线有功表；1-三相三线有功表;2--三相四线真无功表(QT4);3--三相三线真无功表(Q32);
            //4--三元件跨相90无功表(Q33);5--二元件跨相90无功表(Q90);6--二元件人工中点(60)无功表(Q60);
            #region
            double XwUa = 0;
            double XwUb = 0;
            double XwUc = 0;
            double XwIa = 0;
            double XwIb = 0;
            double XwIc = 0;
            string strGlys = "";
            string LC = "";
            double LcValue = 0;
            double Phi = 0;
            int n = 1;

            strGlys = Glys;

            if (jxfs == 0)// 三相四线有功 = 0,
            { jxfs = 0; }
            else if (jxfs == 1)//三相四线无功 = 1,
            { jxfs = 2; }
            else if (jxfs == 2)//三相三线有功 = 2,
            { jxfs = 1; }
            else if (jxfs == 3)//三相三线无功 = 3,
            { jxfs = 3; }
            else if (jxfs == 4)//二元件跨相90 = 4,
            { jxfs = 5; }
            else if (jxfs == 5)//二元件跨相60 = 5,
            { jxfs = 6; }
            else if (jxfs == 6)//三元件跨相90 = 6,
            { jxfs = 4; }
            else if (jxfs == 7)
            { jxfs = 7; }


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

            switch (jxfs)
            {
                case 0:  //三相四线有功表
                    Phi = Math.Round(Math.Acos(LcValue) * (180 / Math.PI), 5);
                    XwUa = 0;
                    XwUb = 240;
                    XwUc = 120;
                    if (LcValue > 0)
                    {
                        XwIa = 0;
                        XwIb = 240;
                        XwIc = 120;
                        Phi = 1 * Phi;

                    }
                    else if (LcValue < 0)
                    {
                        XwIa = 180;
                        XwIb = 60;
                        XwIc = 300;
                        Phi = -1 * Phi;
                    }
                    if (LC == "L")
                    {
                        Phi = 1 * Phi;
                        XwIa = XwUa - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = XwUb - Phi;
                        if (XwIb < 0) XwIb = XwIb + 360;
                        if (XwIb >= 360) XwIb = XwIb - 360;

                        XwIc = XwUc - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;

                    }
                    if (LC == "C")
                    {
                        Phi = -1 * Phi;
                        XwIa = XwUa - Phi;

                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = XwUb - Phi;
                        if (XwIb < 0) XwIb = XwIb + 360;
                        if (XwIb >= 360) XwIb = XwIb - 360;

                        XwIc = XwUc - Phi;

                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    break;
                case 1:  //三相三线有功表
                    Phi = Math.Round(Math.Acos(LcValue) * (180 / Math.PI), 5);
                    XwUa = 30;
                    XwUb = 240;
                    XwUc = 90;
                    if (LcValue > 0)
                    {
                        if (iYuanjian == 0)
                        {
                            XwIa = 0;
                            XwIb = 240;
                            XwIc = 120;
                            Phi = 1 * Phi;
                        }
                        else if (iYuanjian == 1)
                        {
                            XwIa = 30;
                            XwIb = 270;
                            XwIc = 150;
                            Phi = 1 * Phi;
                        }
                        else if (iYuanjian == 3)
                        {
                            XwIa = 330;
                            XwIb = 210;
                            XwIc = 90;
                            Phi = 1 * Phi;
                        }
                    }
                    else if (LcValue < 0)
                    {

                        XwIa = 180;
                        XwIb = 0;
                        XwIc = 240;
                        Phi = -1 * Phi;
                    }
                    if (LC == "L")
                    {
                        if (iYuanjian == 0)
                        {
                            Phi = 1 * Phi;
                            XwIa = 0 - Phi;
                            if (XwIa < 0) XwIa = XwIa + (360);
                            if (XwIa >= 360) XwIa = XwIa - (360);
                            XwIb = 240 - Phi;
                            if (XwIb < 0) XwIb = XwIb + (360);
                            if (XwIb >= 360) XwIb = XwIb - (360);
                            XwIc = 120 - Phi;
                            if (XwIc < 0) XwIc = XwIc + 360;
                            if (XwIc >= 360) XwIc = XwIc - 360;
                        }
                        else if (iYuanjian == 1)
                        {
                            Phi = 1 * Phi;
                            XwIa = 330;
                            if (XwIa < 0) XwIa = XwIa + (360);
                            if (XwIa >= 360) XwIa = XwIa - (360);
                            XwIb = 0;
                            if (XwIb < 0) XwIb = XwIb + (360);
                            if (XwIb >= 360) XwIb = XwIb - (360);
                            XwIc = 0;
                            if (XwIc < 0) XwIc = XwIc + 360;
                            if (XwIc >= 360) XwIc = XwIc - 360;
                        }
                        else if (iYuanjian == 3)
                        {
                            Phi = 1 * Phi;
                            XwIa = 0;
                            if (XwIa < 0) XwIa = XwIa + (360);
                            if (XwIa >= 360) XwIa = XwIa - (360);
                            XwIb = 0;
                            if (XwIb < 0) XwIb = XwIb + (360);
                            if (XwIb >= 360) XwIb = XwIb - (360);
                            XwIc = 90 - Phi;
                            if (XwIc < 0) XwIc = XwIc + 360;
                            if (XwIc >= 360) XwIc = XwIc - 360;
                        }
                    }
                    if (LC == "C")
                    {
                        if (iYuanjian == 0)
                        {
                            Phi = -1 * Phi;
                            XwIa = 0 - Phi - 6;
                            if (XwIa < 0) XwIa = XwIa + 360;
                            if (XwIa >= 360) XwIa = XwIa - 360;

                            XwIb = 240 - Phi - 6;
                            if (XwIb < 0) XwIb = XwIb + (360);
                            if (XwIb >= 360) XwIb = XwIb - (360);

                            XwIc = 120 - Phi - 6;
                            if (XwIc < 0) XwIc = XwIc + 360;
                            if (XwIc >= 360) XwIc = XwIc - 360;
                        }
                        else if (iYuanjian == 1)
                        {
                            Phi = -1 * Phi;
                            XwIa = 60;
                            if (XwIa < 0) XwIa = XwIa + 360;
                            if (XwIa >= 360) XwIa = XwIa - 360;

                            XwIb = 240 - Phi - 6;
                            if (XwIb < 0) XwIb = XwIb + (360);
                            if (XwIb >= 360) XwIb = XwIb - (360);

                            XwIc = 120 - Phi - 6;
                            if (XwIc < 0) XwIc = XwIc + 360;
                            if (XwIc >= 360) XwIc = XwIc - 360;
                        }
                        else if (iYuanjian == 3)
                        {
                            Phi = -1 * Phi;
                            XwIa = 0 - Phi - 6;
                            if (XwIa < 0) XwIa = XwIa + 360;
                            if (XwIa >= 360) XwIa = XwIa - 360;

                            XwIb = 240 - Phi - 6;
                            if (XwIb < 0) XwIb = XwIb + (360);
                            if (XwIb >= 360) XwIb = XwIb - (360);

                            XwIc = 120;
                            if (XwIc < 0) XwIc = XwIc + 360;
                            if (XwIc >= 360) XwIc = XwIc - 360;
                        }
                    }
                    if (PH == false)
                    {
                        XwIa = XwIa + 30;
                        XwIc = XwIc + 30;
                    }
                    break;
                case 2: //三相四线真无功表(QT4)
                    XwUa = 0;
                    XwUb = 240;
                    XwUc = 120;
                    if (LcValue > 0)
                    {
                        XwIa = 270;
                        XwIb = 150;
                        XwIc = 30;
                        n = 1;
                    }
                    else if (LcValue < 0)
                    {

                        XwIa = 90;
                        XwIb = 330;
                        XwIc = 210;
                        n = -1;
                    }
                    if (LC == "L")
                    {
                        if (n == -1) Phi = (-180) - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        else Phi = Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = Phi;
                        XwIa = XwUa - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = XwUb - Phi;
                        if (XwIb < 0) XwIb = XwIb + 360;
                        if (XwIb >= 360) XwIb = XwIb - 360;

                        XwIc = XwUc - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    if (LC == "C")
                    {
                        if (n == -1) Phi = (-180) - (180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5));
                        else Phi = 180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = n + Phi;
                        XwIa = XwUa - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = XwUb - Phi;
                        if (XwIb < 0) XwIb = XwIb + 360;
                        if (XwIb >= 360) XwIb = XwIb - 360;

                        XwIc = XwUc - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    break;
                case 3: //三相三线真无功表(Q32)
                    XwUa = 30;
                    XwUb = 0;
                    XwUc = 90;

                    if (LcValue > 0)
                    {
                        if (iYuanjian == 0)
                        {
                            XwIa = 270;
                            XwIb = 0;
                            XwIc = 30;
                        }
                        else if (iYuanjian == 1)
                        {
                            XwIa = 300;
                            XwIb = 0;
                            XwIc = 0;
                        }
                        else if (iYuanjian == 3)
                        {
                            XwIa = 0;
                            XwIb = 0;
                            XwIc = 0;
                        }
                        n = 1;
                    }
                    else if (LcValue < 0)
                    {

                        XwIa = 90;
                        XwIb = 0;
                        XwIc = 210;
                        n = -1;
                    }

                    if (LC == "L")
                    {
                        switch (iYuanjian)
                        {
                            case 0:
                                if (n == -1) Phi = (-180) - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                                else Phi = Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                                //Phi = Phi;
                                XwIa = 0 - Phi;
                                if (XwIa < 0) XwIa = XwIa + 360;
                                if (XwIa >= 360) XwIa = XwIa - 360;

                                XwIb = 0;


                                XwIc = 120 - Phi;
                                if (XwIc < 0) XwIc = XwIc + 360;
                                if (XwIc >= 360) XwIc = XwIc - 360;
                                break;
                            case 1:
                                if (n == -1) Phi = (-180) - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                                else Phi = Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                                //Phi = Phi;
                                XwIa = 30 - Phi;
                                if (XwIa < 0) XwIa = XwIa + 360;
                                if (XwIa >= 360) XwIa = XwIa - 360;

                                XwIb = 0;


                                XwIc = 120 - Phi;
                                if (XwIc < 0) XwIc = XwIc + 360;
                                if (XwIc >= 360) XwIc = XwIc - 360;
                                break;
                            case 3:
                                if (n == -1) Phi = (-180) - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                                else Phi = Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                                //Phi = Phi;
                                XwIa = 0 - Phi;
                                if (XwIa < 0) XwIa = XwIa + 360;
                                if (XwIa >= 360) XwIa = XwIa - 360;

                                XwIb = 0;


                                XwIc = 90 - Phi;
                                if (XwIc < 0) XwIc = XwIc + 360;
                                if (XwIc >= 360) XwIc = XwIc - 360;
                                break;
                        }
                    }
                    if (LC == "C")
                    {
                        switch (iYuanjian)
                        {
                            case 0:
                                if (n == -1) Phi = (-180) - (180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5));
                                else Phi = 180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                                //Phi = n + Phi;
                                XwIa = 0 - Phi - 6;
                                if (XwIa < 0) XwIa = XwIa + 360;
                                if (XwIa >= 360) XwIa = XwIa - 360;

                                XwIb = 0;


                                XwIc = 120 - Phi - 6;
                                if (XwIc < 0) XwIc = XwIc + 360;
                                if (XwIc >= 360) XwIc = XwIc - 360;
                                break;
                            case 1:
                                if (n == -1) Phi = (-180) - (180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5));
                                else Phi = 180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                                //Phi = n + Phi;
                                XwIa = 330;
                                if (XwIa < 0) XwIa = XwIa + 360;
                                if (XwIa >= 360) XwIa = XwIa - 360;

                                XwIb = 0;


                                XwIc = 120 - Phi - 6;
                                if (XwIc < 0) XwIc = XwIc + 360;
                                if (XwIc >= 360) XwIc = XwIc - 360;
                                break;
                            case 3:
                                if (n == -1) Phi = (-180) - (180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5));
                                else Phi = 180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                                //Phi = n + Phi;
                                XwIa = 0 - Phi - 6;
                                if (XwIa < 0) XwIa = XwIa + 360;
                                if (XwIa >= 360) XwIa = XwIa - 360;

                                XwIb = 0;


                                XwIc = 330;
                                if (XwIc < 0) XwIc = XwIc + 360;
                                if (XwIc >= 360) XwIc = XwIc - 360;
                                break;
                        }
                    }
                    if (PH == false)
                    {
                        XwIa = XwIa + 30;
                        XwIc = XwIc - 30;
                    }
                    break;
                case 4: //三元件跨相90无功表(Q33)
                    XwUa = 30;
                    XwUb = 270;
                    XwUc = 150;
                    if (LcValue > 0)
                    {
                        XwIa = 270;
                        XwIb = 150;
                        XwIc = 30;
                        n = 1;
                    }
                    else if (LcValue < 0)
                    {

                        XwIa = 90;
                        XwIb = 330;
                        XwIc = 210;
                        n = -1;
                    }

                    if (LC == "L")
                    {
                        if (n == -1) Phi = (-180) - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        else Phi = Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = 240 - Phi;
                        if (XwIb < 0) XwIb = XwIb + 360;
                        if (XwIb >= 360) XwIb = XwIb - 360;

                        XwIc = 120 - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    if (LC == "C")
                    {
                        if (n == -1) Phi = (-180) - (180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5));
                        else Phi = 180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = n + Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = 240 - Phi;
                        if (XwIb < 0) XwIb = XwIb + 360;
                        if (XwIb >= 360) XwIb = XwIb - 360;

                        XwIc = 120 - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    break;
                case 5: //二元件跨相90无功表(Q90)
                    XwUa = 30;
                    XwUb = 0;
                    XwUc = 270;

                    if (LcValue > 0)
                    {
                        XwIa = 270;
                        XwIb = 0;
                        XwIc = 30;
                        n = 1;
                    }
                    else if (LcValue < 0)
                    {

                        XwIa = 90;
                        XwIb = 0;
                        XwIc = 210;
                        n = -1;
                    }

                    if (LC == "L")
                    {
                        if (n == -1) Phi = (-180) - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        else Phi = Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = 0;


                        XwIc = 120 - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    if (LC == "C")
                    {
                        if (n == -1) Phi = (-180) - (180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5));
                        else Phi = 180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = n + Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = 0;


                        XwIc = 120 - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    break;
                case 6: //二元件跨相60无功表(Q60)
                    XwUa = 0;
                    XwUb = 0;
                    XwUc = 120;

                    if (LcValue > 0)
                    {
                        XwIa = 270;
                        XwIb = 0;
                        XwIc = 30;
                        n = 1;
                    }
                    else if (LcValue < 0)
                    {

                        XwIa = 90;
                        XwIb = 0;
                        XwIc = 210;
                        n = -1;
                    }
                    if (LC == "L")
                    {
                        if (n == -1) Phi = (-180) - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        else Phi = Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = 0;


                        XwIc = 120 - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    if (LC == "C")
                    {
                        if (n == -1) Phi = (-180) - (180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5));
                        else Phi = 180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = n + Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = 0;


                        XwIc = 120 - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    break;
                case 7: //单相表
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
                    XwUb = XwUa;
                    XwUc = XwUa;
                    XwIb = XwIa;
                    XwIc = XwIa;
                    break;
            }
            #endregion

            //Single[] sng_PhiXX = GetPhiGlys((int)jxfs, Glys, (int)iYuanjian, PH); //根据测试方式、功率因数、元件、相序计算三相电压电流角度
            //tmpOk = SetAcSourcePhasic(XwUa, XwUb, XwUc, XwIa, XwIb, XwIc);
            m_Phipara.PhiUa = XwUa;
            m_Phipara.PhiUb = XwUb;
            m_Phipara.PhiUc = XwUc;

            m_Phipara.PhiIa = XwIa;
            m_Phipara.PhiIb = XwIb;
            m_Phipara.PhiIc = XwIc;

            return true;
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
        /*
         * 81 30 PCID 09 a0 02 02 40 CS
         */
        protected override byte[] GetBody()
        {
            int tmpvalue = 0;
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();

            byte[] byt_Value = new byte[67];

            byt_Value[0] = PAGE5;

            byt_Value[1] = GROUP1 + GROUP2 + GROUP6;

            //GROUP1 
            byt_Value[2] = DATA0 + DATA1 + DATA2 + DATA3 + DATA4 + DATA5; //len = 8          

            tmpvalue = (int)(m_Phipara.PhiUc * JIAODU_BEISHU);//电压角度 C-B-A
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 3, 4);

            tmpvalue = (int)(m_Phipara.PhiUb * JIAODU_BEISHU);//
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 7, 4);

            tmpvalue = (int)(m_Phipara.PhiUa * JIAODU_BEISHU);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 11, 4);

            tmpvalue = (int)(m_Phipara.PhiIc * JIAODU_BEISHU);//电流角度 C-B-A
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 15, 4);

            tmpvalue = (int)(m_Phipara.PhiIb * JIAODU_BEISHU);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 19, 4);

            tmpvalue = (int)(m_Phipara.PhiIa * JIAODU_BEISHU);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 23, 4);



            //GROUP2
            byt_Value[27] = DATA0 + DATA1 + DATA2 + DATA3 + DATA4 + DATA5 + DATA6 + DATA7; //len = 33

            tmpvalue = (int)(m_UIpara.Uc * 10000);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 28, 4);
            byt_Value[32] = (byte)Convert.ToSByte(-4);

            tmpvalue = (int)(m_UIpara.Ub * 10000);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 33, 4);
            byt_Value[37] = (byte)Convert.ToSByte(-4);

            tmpvalue = (int)(m_UIpara.Ua * 10000);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 38, 4);
            byt_Value[42] = (byte)Convert.ToSByte(-4);

            tmpvalue = (int)(m_UIpara.Ic * 1000000);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 43, 4);
            byt_Value[47] = (byte)Convert.ToSByte(-6);

            tmpvalue = (int)(m_UIpara.Ib * 1000000);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 48, 4);
            byt_Value[52] = (byte)Convert.ToSByte(-6);

            tmpvalue = (int)(m_UIpara.Ia * 1000000);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 53, 4);
            byt_Value[57] = (byte)Convert.ToSByte(-6);

            //

            tmpvalue = (int)(_Hz * 10000);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 58, 4); ; //

            byt_Value[63] = DATA0 + DATA1 + DATA2; //

            byt_Value[64] = DATA0 + DATA1 + DATA2;//

            byt_Value[62] = DATA0 + DATA1 + DATA2 + DATA3 + DATA4 + DATA5 + DATA6;
            //byt_Value[64] = //
            byt_Value[65] = Convert.ToByte(_iChange);// DATA0 + DATA1 + DATA2 + DATA3 + DATA4 + DATA5;// +DATA6; //

            if (_bCurrentDB)
            {
                byt_Value[66] = 0;//电流对标
            }
            else
            {
                byt_Value[66] = 0x0;//不对标
            }
            buf.Put(0xA3);
            buf.Put(byt_Value);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 升源 返回指令
    /// </summary>
    internal class Cl309_RequestPowerOnReplyPacket : CL309RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 1)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x30)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #region CL309读取过载信息
    /// <summary>
    /// 读取源过载信息请求包
    /// </summary>
    internal class CL309_RequestReadPowerOverInfoPacket : CL309SendPacket
    {
        public bool IsLink = true;

        public CL309_RequestReadPowerOverInfoPacket()
            : base()
        { }

        /*
         * 81 01 PCID 08 a0 02 00 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xC9);  //命令           
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 源联机 返回指令
    /// </summary>
    internal class CL309_RequestReadPowerOverInfoReplyPacket : CL309RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 36)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x50)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #region CL309 读取源版本号
    /// <summary>
    /// 读取源版本 请求包
    /// </summary>
    internal class CL309_RequestReadVerPacket : CL309SendPacket
    {
        public bool IsLink = true;

        public CL309_RequestReadVerPacket()
            : base()
        { }

        /*
         * 81 01 PCID 06 C9 00 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xC9);  //命令           
            return buf.ToByteArray();
        }
    }

    /// <summary>
    /// 读取CL309版本号返回包
    /// </summary>
    internal class CL309_RequestReadVersionReplayPacket : CL309RecvPacket
    {
        public CL309_RequestReadVersionReplayPacket() : base() { }

        /// <summary>
        /// 读取到的版本号
        /// 默认值为Unknown
        /// </summary>
        public string Version { get; private set; }


        protected override void ParseBody(byte[] data)
        {
            Version = ASCIIEncoding.UTF8.GetString(data);
        }
    }
    #endregion

    #region CL309更新源
    /// <summary>
    /// 更新源 请求包
    /// </summary>
    internal class CL309_RequestUpdateUIPacket : CL309SendPacket
    {
        public bool IsLink = true;

        public CL309_RequestUpdateUIPacket()
            : base()
        { }

        /*
         * 81 01 PCID 06 C9 00 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);  //命令           
            buf.Put(0x05);
            buf.Put(0x44);
            buf.Put(0x80);
            buf.Put(0x07);
            buf.Put(0x0B);
            buf.Put(0x3F);
            buf.Put(0x3F);
            buf.Put(0x01);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 更新309源返回包
    /// </summary>
    internal class CL309_RequestUpdateUIPacketReplayPacket : CL309RecvPacket
    {
        public CL309_RequestUpdateUIPacketReplayPacket() : base() { }
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 1)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x30)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #endregion

    #region CL3115标准表

    #region CL3115标准表联机指令
    /// <summary>
    /// 标准表联机/脱机请求包
    /// </summary>
    internal class CL3115_RequestLinkPacket : CL3115SendPacket
    {
        public bool IsLink = true;

        public CL3115_RequestLinkPacket()
            : base()
        { }

        /*
         * 81 30 PCID 09 a0 02 02 40 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA0);          //命令 
            buf.Put(0x02);
            buf.Put(0x02);
            buf.Put(0x40);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 标准表，联机返回指令
    /// </summary>
    internal class CL3115_RequestLinkReplyPacket : CL3115RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 8)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x50)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #region CL3115读取标准表常数
    /// <summary>
    /// 读取真实本机常数
    /// </summary>
    internal class CL3115_RequestReadStdMeterConstPacket : CL3115SendPacket
    {
        public CL3115_RequestReadStdMeterConstPacket()
            : base()
        { }

        /*
         * 81 30 PCID 09 a0 02 02 40 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA0);          //命令 
            buf.Put(0x02);
            buf.Put(0x02);
            buf.Put(0x40);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 读取真实本机常数返回包
    /// </summary>
    internal class CL3115_RequestReadStdMeterConstReplayPacket : CL3115RecvPacket
    {
        public CL3115_RequestReadStdMeterConstReplayPacket() : base() { }
        public override string GetPacketName()
        {
            return "CL3115_RequestReadStdMeterConstReplayPacket";
        }
        /// <summary>
        /// 本机常数
        /// </summary>
        /// <returns></returns>
        public int meterConst { get; private set; }

        protected override void ParseBody(byte[] data)
        {
            if (data.Length != 0x0d) return;
            ByteBuffer buf = new ByteBuffer(data);
            buf.Initialize();
            //去掉 命令字 50
            buf.Get();

            //去掉0x02
            buf.Get();
            //去掉0x02
            buf.Get();
            //去掉0x40
            buf.Get();

            //表常数
            meterConst = buf.GetInt_S();

        }
    }
    #endregion

    #region CL3115读取标准表信息
    /// <summary>
    /// 读取标准表信息
    /// </summary>
    internal class CL3115_RequestReadStdInfoPacket : CL3115SendPacket
    {
        public CL3115_RequestReadStdInfoPacket()
            : base()
        { }

        /*
         * 81 30 PCID 0e a0 02 3f ff 80 3f ff ff 0f CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA0);          //命令 
            buf.Put(0x02);
            buf.Put(0x3F);
            buf.Put(0xFF);
            buf.Put(0x80);
            buf.Put(0x3F);
            buf.Put(0xFF);
            buf.Put(0xFF);
            buf.Put(0x0F);
            return buf.ToByteArray();
        }
    }

    /// <summary>
    /// 读取标准表信息返回包
    /// </summary>
    internal class CL3115_RequestReadStdInfoReplayPacket : CL3115RecvPacket
    {
        public CL3115_RequestReadStdInfoReplayPacket() : base() { }
        public override string GetPacketName()
        {
            return "CL3115_RequestReadStdInfoReplayPacket";
        }
        /// <summary>
        /// 获取源信息
        /// </summary>
        /// <returns></returns>
        public stStdInfo PowerInfo { get; private set; }

        protected override void ParseBody(byte[] data)
        {
            stStdInfo tagInfo = new stStdInfo();
            ByteBuffer buf = new ByteBuffer(data);
            if (buf.Length != 0xA4) return;
            int[] arrDot = new int[9];

            //去掉 命令字
            buf.Get();

            //去掉0x02
            buf.Get();
            //去掉0x3f
            buf.Get();
            //去掉0xff
            buf.Get();

            //tagInfo.Clfs = (Cus_Clfs)buf.Get();
            //tagInfo.Flip_ABC = buf.Get();
            //tagInfo.Freq = buf.GetUShort_S() / 1000F;
            ////电压档位
            //tagInfo.Scale_Ua = buf.Get();
            //tagInfo.Scale_Ub = buf.Get();
            //tagInfo.Scale_Uc = buf.Get();
            ////电流档位
            //tagInfo.Scale_Ia = buf.Get();
            //tagInfo.Scale_Ib = buf.Get();
            //tagInfo.Scale_Ic = buf.Get();
            ////小数点
            //for (int i = 0; i < arrDot.Length; i++)
            //{
            //    arrDot[i] = buf.Get();
            //}
            //电压电流


            tagInfo.Uc = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Ub = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Ua = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Ic = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Ib = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Ia = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));


            //tagInfo.Ia = get3ByteValue(buf.GetByteArray(3), arrDot[3]);
            //tagInfo.Ub = get3ByteValue(buf.GetByteArray(3), arrDot[1]);
            //tagInfo.Ib = get3ByteValue(buf.GetByteArray(3), arrDot[4]);
            //tagInfo.Uc = get3ByteValue(buf.GetByteArray(3), arrDot[2]);
            //tagInfo.Ic = get3ByteValue(buf.GetByteArray(3), arrDot[5]);
            //频率
            tagInfo.Freq = BitConverter.ToInt32(buf.GetByteArray(4), 0) / 100000;
            //过载标志
            buf.Get();
            //0x80
            buf.Get();
            //相位
            //
            buf.GetByteArray(4);
            //0x3f
            buf.Get();
            tagInfo.Phi_Uc = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);
            tagInfo.Phi_Ub = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);
            tagInfo.Phi_Ua = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);
            tagInfo.Phi_Ic = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);
            tagInfo.Phi_Ib = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);
            tagInfo.Phi_Ia = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);

            if (tagInfo.Phi_Ic > 0)
                tagInfo.Phi_Ic = tagInfo.Phi_Uc - tagInfo.Phi_Ic;
            else
                tagInfo.Phi_Ic = 0;
            if (tagInfo.Phi_Ib > 0)
                tagInfo.Phi_Ib = tagInfo.Phi_Ub - tagInfo.Phi_Ib;
            else
                tagInfo.Phi_Ib = 0;
            if (tagInfo.Phi_Ia > 0)
                tagInfo.Phi_Ia = tagInfo.Phi_Ua - tagInfo.Phi_Ia;
            else
                tagInfo.Phi_Ia = 0;

            if (tagInfo.Phi_Ic < 0)
                tagInfo.Phi_Ic += 360;
            else if (tagInfo.Phi_Ic > 360)
                tagInfo.Phi_Ic -= 360;

            if (tagInfo.Phi_Ib < 0)
                tagInfo.Phi_Ib += 360;
            else if (tagInfo.Phi_Ib > 360)
                tagInfo.Phi_Ib -= 360;

            if (tagInfo.Phi_Ia < 0)
                tagInfo.Phi_Ia += 360;
            else if (tagInfo.Phi_Ia > 360)
                tagInfo.Phi_Ia -= 360;

            if (tagInfo.Ia == 0)
                tagInfo.Phi_Ia = 0;
            if (tagInfo.Ib == 0)
                tagInfo.Phi_Ib = 0;
            if (tagInfo.Ic == 0)
                tagInfo.Phi_Ic = 0;
            //0xff
            buf.Get();
            //C相 B相 A相 相角
            buf.GetByteArray(4);
            buf.GetByteArray(4);
            buf.GetByteArray(4);

            //C相 B相 A相 有功功率因素
            buf.GetByteArray(4);
            buf.GetByteArray(4);
            buf.GetByteArray(4);

            tagInfo.COS = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);
            tagInfo.SIN = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);

            //0xff
            buf.Get();

            tagInfo.Pc = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Pb = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Pa = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.P = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));

            tagInfo.Qc = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Qb = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Qa = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Q = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));

            //0x0f
            buf.Get();
            tagInfo.Sc = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Sb = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Sa = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.S = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));

            tagInfo.COS = get3ByteValue(buf.GetByteArray(3), 5);
            tagInfo.SIN = get3ByteValue(buf.GetByteArray(3), 5);


            //if (Comm.GlobalUnit.Clfs == Cus_Clfs.三相三线 || Comm.GlobalUnit.Clfs == Cus_Clfs.二元件跨相90 || Comm.GlobalUnit.Clfs == Cus_Clfs.二元件跨相60 || Comm.GlobalUnit.Clfs == Cus_Clfs.三元件跨相90)
            //{
            //    tagInfo.Phi_Ia -= 30;
            //    if (tagInfo.Phi_Ia < 0) tagInfo.Phi_Ia += 360;
            //    tagInfo.Phi_Ic -= 30;
            //    if (tagInfo.Phi_Ic < 0) tagInfo.Phi_Ic += 360;
            //    tagInfo.Phi_Ua -= 30;
            //    if (tagInfo.Phi_Ua < 0) tagInfo.Phi_Ua += 360;
            //    tagInfo.Phi_Uc -= 30;
            //    if (tagInfo.Phi_Uc < 0) tagInfo.Phi_Uc += 360;
            //}
            PowerInfo = tagInfo;
        }

        private sbyte GetByteFromByteArray(byte pArray)
        {
            string Fmt16 = Convert.ToString(pArray, 16);
            sbyte ReturnValue = (Convert.ToSByte(Fmt16, 16));
            return ReturnValue;
        }
    }
    #endregion

    #region CL3115读取标准表电能
    /// <summary>
    /// 读取标准表电能
    /// </summary>
    internal class CL3115_RequestReadStdMeterTotalNumPacket : CL3115SendPacket
    {
        public CL3115_RequestReadStdMeterTotalNumPacket()
            : base()
        { }

        /*
         * 81 30 PCID 09 a0 02 20 10 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA0);          //命令 
            buf.Put(0x02);
            buf.Put(0x20);
            buf.Put(0x10);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 读取电能
    /// </summary>
    internal class CL3115_RequestReadStdMeterTotalNumReplayPacket : CL3115RecvPacket
    {
        public CL3115_RequestReadStdMeterTotalNumReplayPacket() : base() { }
        public override string GetPacketName()
        {
            return "CL3115_RequestReadStdMeterTotalNumReplayPacket";
        }
        /// <summary>
        /// 累计电能 8字节，放大10000倍，低字节先传
        /// </summary>
        /// <returns></returns>
        public float MeterTotalNum { get; private set; }


        /// <summary>
        /// 成功返回数据: 81 PCID 30 11 50 02 20 10 llE1 CS
        /// </summary>
        /// <param name="data"></param>
        protected override void ParseBody(byte[] data)
        {
            if (data.Length != 0x0c) return;
            ByteBuffer buf = new ByteBuffer(data);

            //去掉 命令字 50
            buf.Get();
            //去掉0x02
            buf.Get();
            //去掉0x20
            buf.Get();
            //去掉0x10
            buf.Get();
            //累计电能,放大10000倍
            float fStdMeter = BitConverter.ToInt64(buf.GetByteArray(8), 0);
            MeterTotalNum = fStdMeter / 10000;
        }
    }
    #endregion

    #region CL3115读取标准表累计脉冲数
    /// <summary>
    /// 读取电能累计脉冲数
    /// </summary>
    internal class CL3115_RequestReadStdMeterTotalPulseNumPacket : CL3115SendPacket
    {
        public CL3115_RequestReadStdMeterTotalPulseNumPacket()
            : base()
        { }

        /*
         * 81 30 PCID 09 a0 02 40 80 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA0);          //命令 
            buf.Put(0x02);
            buf.Put(0x40);
            buf.Put(0x80);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 读取电能累计脉冲数
    /// </summary>
    internal class CL3115_RequestReadStdMeterTotalPulseNumReplayPacket : CL3115RecvPacket
    {
        public CL3115_RequestReadStdMeterTotalPulseNumReplayPacket() : base() { }
        public override string GetPacketName()
        {
            return "CL3115_RequestReadStdMeterTotalPulseNumReplayPacket";
        }
        /// <summary>
        /// 电能累计脉冲数8字节，低字节先传 ,CLT协议（UINT8）/变量定义SIN8
        /// </summary>
        /// <returns></returns>
        public long Pulsenum { get; private set; }


        /// <summary>
        /// 成功返回数据:81 PCID 30 11 50 02 40 80 llPulsenum1 CS
        /// </summary>
        /// <param name="data"></param>
        protected override void ParseBody(byte[] data)
        {
            if (data.Length != 0x11) return;
            ByteBuffer buf = new ByteBuffer(data);
            buf.Initialize();
            //去掉 命令字 50
            buf.Get();

            //去掉0x02
            buf.Get();
            //去掉0x40
            buf.Get();
            //去掉0x80
            buf.Get();

            //累计电能,放大10000倍
            Pulsenum = buf.GetLong_S();

        }
    }
    #endregion

    #region CL3115读取走字数据
    /// <summary>
    /// 读取电能走字数据
    /// </summary>
    internal class CL3115_RequestReadStdMeterZZDataPacket : CL3115SendPacket
    {
        public CL3115_RequestReadStdMeterZZDataPacket()
            : base()
        { }

        /*
         * 81 30 PCID 0a a0 02 60 10 80 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA0);          //命令 
            buf.Put(0x02);
            buf.Put(0x60);
            buf.Put(0x10);
            buf.Put(0x80);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 读取电能走字数据
    /// </summary>
    internal class CL3115_RequestReadStdMeterZZDataReplayPacket : CL3115RecvPacket
    {
        public CL3115_RequestReadStdMeterZZDataReplayPacket() : base() { }
        public override string GetPacketName()
        {
            return "CL3115_RequestReadStdMeterZZDataReplayPacket";
        }

        /// <summary>
        /// 累计电能 放大10000倍
        /// </summary>
        /// <returns></returns>
        public long meterTotalNum { get; private set; }

        /// <summary>
        /// 电能当前脉冲累计值
        /// </summary>
        public long meterPulseNum { get; private set; }

        /*
         * 成功返回数据
         *  81 PCID 30 1a 50
         * 02 60 
         * 10 
         * 00 00 00 00 00 00 00 00 //累计电能 放大10000倍
         * 80
         * 00 00 00 00 00 00 00 00 //电能当前脉冲累计值
         * CS
         * 失败返回Cmd 33
         * 81 PCID 30 06 33 CS 
         */
        protected override void ParseBody(byte[] data)
        {
            if (data.Length != 0x1A) return;
            ByteBuffer buf = new ByteBuffer(data);
            buf.Initialize();
            //去掉 命令字 50
            buf.Get();

            //去掉0x02
            buf.Get();
            //去掉0x60
            buf.Get();
            //去掉0x10
            buf.Get();

            //累计电能
            meterTotalNum = buf.GetInt_S() / 10000;

            //去掉0x80
            buf.Get();

            meterPulseNum = buf.GetInt_S();

        }
    }
    #endregion

    #region CL3115设置标准表常数
    /// <summary>
    /// 设置标准表常数
    /// </summary>
    internal class CL3115_RequestSetStdMeterConstPacket : CL3115SendPacket
    {
        /// <summary>
        /// 本机常数，4字节，低字节先传
        /// </summary>
        private int stdMeterConst;

        public CL3115_RequestSetStdMeterConstPacket()
            : base()
        {

        }

        /// <summary>
        /// 设置本机常数
        /// </summary>
        /// <param name="meterconst">本机常数</param>
        /// <param name="needReplay">是否需要回复</param>
        public CL3115_RequestSetStdMeterConstPacket(int meterconst, bool needReplay)
            : base(needReplay)
        {
            stdMeterConst = meterconst;
        }

        public void SetPara(int meterconst)
        {
            stdMeterConst = meterconst;
        }
        /*
         * 81 30 PCID 0d a3 00 04 01 uiLocalnum CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);          //命令 
            buf.Put(0x00);
            buf.Put(0x04);
            buf.Put(0x01);
            buf.PutInt_S(stdMeterConst);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 设置标准表常数返回包
    /// </summary>
    internal class CL3115_RequestSetStdMeterConstReplayPacket : CL3115RecvPacket
    {
        public CL3115_RequestSetStdMeterConstReplayPacket() : base() { }
        public override string GetPacketName()
        {
            return "CL3115_RequestSetStdMeterConstReplayPacket";
        }


        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 1)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x30)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }

        }
    }
    #endregion

    #region CL3115设置标准表参数
    /// <summary>
    /// 设置标准表参数
    /// </summary>
    internal class CL3115_RequestSetParaPacket : CL3115SendPacket
    {
        private byte _YouGongSetData;
        private byte _ClfsSetData;
        /// <summary>
        /// 
        /// </summary>
        public CL3115_RequestSetParaPacket()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_Clfs">测量方式</param>        
        public void SetPara(Cus_Clfs _Clfs, Cus_PowerFangXiang glfx, bool bAuto)
        {

            if (glfx == Cus_PowerFangXiang.正向有功 || glfx == Cus_PowerFangXiang.反向有功)
                _YouGongSetData = 0x00;
            else
                _YouGongSetData = 0x40;


            if (GlobalUnit.IsDan)
            {
                if (bAuto)
                    _ClfsSetData = 0x08;
                else
                    _ClfsSetData = 0x88;
            }
            else
            {
                if (bAuto)
                {
                    switch (_Clfs)
                    {
                        case Cus_Clfs.三相四线:
                            _ClfsSetData = 0x08;
                            break;
                        case Cus_Clfs.三相三线:
                            _ClfsSetData = 0x48;
                            break;
                        case Cus_Clfs.三元件跨相90:
                            _ClfsSetData = 0x44;
                            break;
                        case Cus_Clfs.二元件跨相90:
                            _ClfsSetData = 0x42;
                            break;
                        case Cus_Clfs.二元件跨相60:
                            _ClfsSetData = 0x41;
                            break;
                        default:
                            _ClfsSetData = 0x08;
                            break;
                    }
                }
                else
                {
                    switch (_Clfs)
                    {
                        case Cus_Clfs.三相四线:
                            _ClfsSetData = 0x88;
                            break;
                        case Cus_Clfs.三相三线:
                            _ClfsSetData = 0xC8;
                            break;
                        case Cus_Clfs.三元件跨相90:
                            _ClfsSetData = 0xC4;
                            break;
                        case Cus_Clfs.二元件跨相90:
                            _ClfsSetData = 0xC2;
                            break;
                        case Cus_Clfs.二元件跨相60:
                            _ClfsSetData = 0xC1;
                            break;
                        default:
                            _ClfsSetData = 0x88;
                            break;
                    }
                }
            }
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);
            buf.Put(0x00);
            buf.Put(0x09);
            buf.Put(0x20);
            buf.Put(_ClfsSetData);
            buf.Put(0x11);
            buf.Put(_YouGongSetData);
            buf.Put(0x00);
            buf.Put(0x01);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 设置标准表参数返回包
    /// </summary>
    internal class CL3115_RequestSetParaReplayPacket : CL3115RecvPacket
    {
        public CL3115_RequestSetParaReplayPacket() : base() { }
        public override string GetPacketName()
        {
            return "CL3115_RequestSetParaReplayPacket";
        }

        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 1)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x30)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }

        }
    }
    #endregion

    #region CL3115返回指令
    class CL3115_ReplyOkPacket : CL3115RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data.Length != 1)
            {
                this.ReciveResult = RecvResult.DataError;
            }
            else if (data[0] == 0x30)
            {
                this.ReciveResult = RecvResult.OK;
            }
            else
            {
                this.ReciveResult = RecvResult.DataError;
            }
        }
    }
    #endregion

    #region CL3115设置档位
    /// <summary>
    /// 设置档位
    /// </summary>
    internal class CL3115_RequestSetStdMeterDangWeiPacket : CL3115SendPacket
    {
        /// <summary>
        /// C相电压档位
        /// </summary>
        private Cus_StdMeterVDangWei ucUcRange;
        /// <summary>
        /// B相电压档位
        /// </summary>
        private Cus_StdMeterVDangWei ucUbRange;
        /// <summary>
        /// A相电压档位
        /// </summary>
        private Cus_StdMeterVDangWei ucUaRange;
        /// <summary>
        /// C相电流档位
        /// </summary>
        private Cus_StdMeterIDangWei ucIcRange;
        /// <summary>
        /// B相电流档位
        /// </summary>
        private Cus_StdMeterIDangWei ucIbRange;
        /// <summary>
        /// C相电流档位
        /// </summary>
        private Cus_StdMeterIDangWei ucIaRange;

        /// <summary>
        /// 通一设置档位,默认需要回复
        /// </summary>
        /// <param name="uRange">电压档位</param>
        /// <param name="iRange">电流档位</param>
        public CL3115_RequestSetStdMeterDangWeiPacket(Cus_StdMeterVDangWei uRange, Cus_StdMeterIDangWei iRange)
            : base()
        {
            ucUaRange = uRange;
            ucUbRange = uRange;
            ucUcRange = uRange;
            ucIaRange = iRange;
            ucIbRange = iRange;
            ucIcRange = iRange;
        }
        /// <summary>
        /// 通一设置档位
        /// </summary>
        /// <param name="uRange">电压档位</param>
        /// <param name="iRange">电流档位</param>
        /// <param name="needReplay">是否需要回复</param>
        public CL3115_RequestSetStdMeterDangWeiPacket(Cus_StdMeterVDangWei uRange, Cus_StdMeterIDangWei iRange, bool needReplay)
            : base(needReplay)
        {
            ucUaRange = uRange;
            ucUbRange = uRange;
            ucUcRange = uRange;
            ucIaRange = iRange;
            ucIbRange = iRange;
            ucIcRange = iRange;
        }

        /// <summary>
        /// 设置档位
        /// </summary>
        /// <param name="uaRange">A相电压档位</param>
        /// <param name="ubRange">B相电压档位</param>
        /// <param name="ucRange">C相电压档位</param>
        /// <param name="iaRange">A相电流档位</param>
        /// <param name="ibRange">B相电流档位</param>
        /// <param name="icRange">C相电流档位</param>
        public CL3115_RequestSetStdMeterDangWeiPacket(Cus_StdMeterVDangWei uaRange, Cus_StdMeterVDangWei ubRange, Cus_StdMeterVDangWei ucRange, Cus_StdMeterIDangWei iaRange, Cus_StdMeterIDangWei ibRange, Cus_StdMeterIDangWei icRange)
            : base()
        {
            ucUaRange = uaRange;
            ucUbRange = ubRange;
            ucUcRange = ucRange;
            ucIaRange = iaRange;
            ucIbRange = ibRange;
            ucIcRange = icRange;
        }
        /// <summary>
        /// 设置档位
        /// </summary>
        /// <param name="uaRange">A相电压档位</param>
        /// <param name="ubRange">B相电压档位</param>
        /// <param name="ucRange">C相电压档位</param>
        /// <param name="iaRange">A相电流档位</param>
        /// <param name="ibRange">B相电流档位</param>
        /// <param name="icRange">C相电流档位</param>
        /// <param name="needReplay">是否需要回复</param>
        public CL3115_RequestSetStdMeterDangWeiPacket(Cus_StdMeterVDangWei uaRange, Cus_StdMeterVDangWei ubRange, Cus_StdMeterVDangWei ucRange, Cus_StdMeterIDangWei iaRange, Cus_StdMeterIDangWei ibRange, Cus_StdMeterIDangWei icRange, bool needReplay)
            : base(needReplay)
        {
            ucUaRange = uaRange;
            ucUbRange = ubRange;
            ucUcRange = ucRange;
            ucIaRange = iaRange;
            ucIbRange = ibRange;
            ucIcRange = icRange;
        }

        /*
         * 81 30 PCID 0F A3 02 02 3F ucUcRange ucUbRange ucUaRange ucIcRange ucIbRange ucIaRange CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);          //命令 
            buf.Put(0x02);
            buf.Put(0x02);
            buf.Put(0x3F);
            buf.Put((byte)ucUcRange);
            buf.Put((byte)ucUbRange);
            buf.Put((byte)ucUaRange);
            buf.Put((byte)ucIcRange);
            buf.Put((byte)ucIbRange);
            buf.Put((byte)ucIaRange);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 设置标准表接线方式返回包
    /// </summary>
    internal class CL3115_RequestSetStdMeterDangWeiReplayPacket : CL3115RecvPacket
    {
        public CL3115_RequestSetStdMeterDangWeiReplayPacket() : base() { }
        public override string GetPacketName()
        {
            return "CL3115_RequestSetStdMeterDangWeiReplayPacket";
        }


        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 1)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x30)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }

        }
    }
    #endregion

    #region CL3115设置接线方式
    /// <summary>
    /// 设置接线方式
    /// </summary>
    internal class CL3115_RequestSetStdMeterLinkTypePacket : CL3115SendPacket
    {
        private byte _SetData;
        /// <summary>
        /// 
        /// </summary>
        public CL3115_RequestSetStdMeterLinkTypePacket()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_Clfs">测量方式</param>
        /// <param name="bAuto">自动，手动</param>
        public void SetPara(Cus_Clfs _Clfs, bool bAuto)
        {
            if (GlobalUnit.IsDan)
            {
                if (bAuto)
                    _SetData = 0x08;
                else
                    _SetData = 0x88;
            }
            else
            {
                if (bAuto)
                {
                    switch (_Clfs)
                    {
                        case Cus_Clfs.三相四线:
                            _SetData = 0x08;
                            break;
                        case Cus_Clfs.三相三线:
                            _SetData = 0x48;
                            break;
                        case Cus_Clfs.三元件跨相90:
                            _SetData = 0x44;
                            break;
                        case Cus_Clfs.二元件跨相90:
                            _SetData = 0x42;
                            break;
                        case Cus_Clfs.二元件跨相60:
                            _SetData = 0x41;
                            break;
                        default:
                            _SetData = 0x08;
                            break;
                    }
                }
                else
                {
                    switch (_Clfs)
                    {
                        case Cus_Clfs.三相四线:
                            _SetData = 0x88;
                            break;
                        case Cus_Clfs.三相三线:
                            _SetData = 0xC8;
                            break;
                        case Cus_Clfs.三元件跨相90:
                            _SetData = 0xC4;
                            break;
                        case Cus_Clfs.二元件跨相90:
                            _SetData = 0xC2;
                            break;
                        case Cus_Clfs.二元件跨相60:
                            _SetData = 0xC1;
                            break;
                        default:
                            _SetData = 0x88;
                            break;
                    }
                }
            }
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);
            buf.Put(0x00);
            buf.Put(0x01);
            buf.Put(0x20);
            buf.Put(_SetData);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 设置标准表接线方式返回包
    /// </summary>
    internal class CL3115_RequestSetStdMeterLinkTypeReplayPacket : CL3115RecvPacket
    {
        public CL3115_RequestSetStdMeterLinkTypeReplayPacket() : base() { }
        public override string GetPacketName()
        {
            return "CL3115_RequestSetStdMeterLinkTypeReplayPacket";
        }


        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 1)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x30)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }

        }
    }
    #endregion

    #region CL3115设置标准表显示
    /// <summary>
    /// 置标准表界面
    /// 由于谐波数据和波形数据仅在对应界面下获取，读取谐波数据和波形数据前必须将界面切到对应界面
    /// 界面设置命令在界面切换过程中享有最高优先级，因此为不影响上位机和使用人员的正常操作
    /// 在不需读取谐波数据和波形数据后，将界面设置为清除上位机设置。
    /// </summary>
    internal class CL3115_RequestSetStdMeterScreenPacket : CL3115SendPacket
    {
        /// <summary>
        /// 标准表界面指示
        /// </summary>
        public Cus_StdMeterScreen meterScreen;

        /// <summary>
        /// 设置标准表界面
        /// </summary>
        /// <param name="meterscreen">标准表界面指示</param>
        public CL3115_RequestSetStdMeterScreenPacket(Cus_StdMeterScreen meterscreen)
            : base()
        {
            meterScreen = meterscreen;
        }


        /*
         * 81 30 PCID 0a a3 00 10 80 ucARM_Menu CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);          //命令 
            buf.Put(0x00);
            buf.Put(0x08);
            buf.Put(0x01);
            buf.Put((byte)meterScreen);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 设置标准表显示返回包
    /// </summary>
    internal class CL3115_RequestSetStdMeterScreenReplayPacket : CL3115RecvPacket
    {
        public CL3115_RequestSetStdMeterScreenReplayPacket() : base() { }
        public override string GetPacketName()
        {
            return "CL3115_RequestSetStdMeterScreenReplayPacket";
        }

        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 8)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x50)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }

        }
    }
    #endregion

    #region CL3115设置标准表测量方式
    /// <summary>
    /// 设置3115标准表测量方式
    /// </summary>
    internal class CL3115_RequestSetStdMeterUsE1typePacket : CL3115SendPacket
    {
        private byte _SetData;
        /// <summary>
        /// 
        /// </summary>
        public CL3115_RequestSetStdMeterUsE1typePacket()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_Clfs">测量方式</param>        
        public void SetPara(Cus_PowerFangXiang glfx)
        {
            if (glfx == Cus_PowerFangXiang.正向有功 || glfx == Cus_PowerFangXiang.反向有功)
                _SetData = 0x00;
            else
                _SetData = 0x40;
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);
            buf.Put(0x00);
            buf.Put(0x08);
            buf.Put(0x01);
            buf.Put(0x11);
            buf.Put(_SetData);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 设置3115标准表测量方式返回包
    /// </summary>
    internal class CL3115_RequestSetStdMeterUsE1typeReplayPacket : CL3115RecvPacket
    {
        public CL3115_RequestSetStdMeterUsE1typeReplayPacket() : base() { }
        public override string GetPacketName()
        {
            return "CL3115_RequestSetStdMeterConstReplayPacket";
        }

        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 1)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x30)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }

        }
    }
    #endregion

    #region CL3115启动标准表
    /// <summary>
    /// 请求启动标准表指令包
    /// 返回0x4b成功
    /// </summary>
    internal class CL3115_RequestStartTaskPacket : CL3115SendPacket
    {
        /// <summary>
        /// 控制类型 
        /// </summary>
        /// <param name="iType"></param>
        public CL3115_RequestStartTaskPacket()
            : base()
        {

        }
        /// <summary>
        /// 控制类型 0，停止；1，开始计算电能误差；2，开始计算电能走字
        /// </summary>
        private int iControlType;

        public void SetPara(int iType)
        {
            this.iControlType = iType;
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);
            buf.Put(0x00);
            buf.Put(0x08);
            buf.Put(0x10);
            buf.Put(Convert.ToByte(iControlType));
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 控制标准表启动、停止、开始走字，返回指令
    /// </summary>
    internal class CL3115_RequestStartTaskReplyPacket : CL3115RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 1)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x30)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    #endregion

    #endregion

    #region CL2031B功耗板

    #region CL2031B读取功耗信息
    /// <summary>
    /// 读取功耗信息
    /// </summary>
    internal class CL2031B_RequestReadGhInfoPacket : CL2031BSendPacket
    {
        public CL2031B_RequestReadGhInfoPacket()
            : base()
        { }

        /// <summary>
        /// 读取类型
        /// </summary>
        private int _ReadType = 0;
        /*
         * 81	01	xx	19	A0	xx	01	01	18	FF	FF	FF	16	FF	FF	FF	3C	FF	FF	FF	3E	FF	FF	FF	CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA0);          //命令 
            buf.Put(Convert.ToByte(_ReadType));
            buf.Put(0x01);
            buf.Put(0x01);
            buf.Put(0x18);
            buf.Put(0xFF);
            buf.Put(0xFF);
            buf.Put(0xFF);
            buf.Put(0x16);
            buf.Put(0xFF);
            buf.Put(0xFF);
            buf.Put(0xFF);
            buf.Put(0x3C);
            buf.Put(0xFF);
            buf.Put(0xFF);
            buf.Put(0xFF);
            buf.Put(0x3E);
            buf.Put(0xFF);
            buf.Put(0xFF);
            buf.Put(0xFF);
            return buf.ToByteArray();
        }
    }

    /// <summary>
    /// 读取2031B功耗信息返回包
    /// </summary>
    internal class CL2031B_RequestReadGhInfoReplayPacket : CL2031BRecvPacket
    {
        public CL2031B_RequestReadGhInfoReplayPacket() : base() { }
        public override string GetPacketName()
        {
            return "CL2031B_RequestReadGhInfoReplayPacket";
        }


        protected override void ParseBody(byte[] data)
        {
            stStdInfo tagInfo = new stStdInfo();
            ByteBuffer buf = new ByteBuffer(data);
            if (buf.Length != 0xA4) return;
            int[] arrDot = new int[9];

            //去掉 命令字
            buf.Get();

            //去掉0x02
            buf.Get();
            //去掉0x3f
            buf.Get();
            //去掉0xff
            buf.Get();

            //tagInfo.Clfs = (Cus_Clfs)buf.Get();
            //tagInfo.Flip_ABC = buf.Get();
            //tagInfo.Freq = buf.GetUShort_S() / 1000F;
            ////电压档位
            //tagInfo.Scale_Ua = buf.Get();
            //tagInfo.Scale_Ub = buf.Get();
            //tagInfo.Scale_Uc = buf.Get();
            ////电流档位
            //tagInfo.Scale_Ia = buf.Get();
            //tagInfo.Scale_Ib = buf.Get();
            //tagInfo.Scale_Ic = buf.Get();
            ////小数点
            //for (int i = 0; i < arrDot.Length; i++)
            //{
            //    arrDot[i] = buf.Get();
            //}
            //电压电流


            tagInfo.Uc = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Ub = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Ua = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Ic = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Ib = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Ia = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));


            //tagInfo.Ia = get3ByteValue(buf.GetByteArray(3), arrDot[3]);
            //tagInfo.Ub = get3ByteValue(buf.GetByteArray(3), arrDot[1]);
            //tagInfo.Ib = get3ByteValue(buf.GetByteArray(3), arrDot[4]);
            //tagInfo.Uc = get3ByteValue(buf.GetByteArray(3), arrDot[2]);
            //tagInfo.Ic = get3ByteValue(buf.GetByteArray(3), arrDot[5]);
            //频率
            tagInfo.Freq = BitConverter.ToInt32(buf.GetByteArray(4), 0) / 100000;
            //过载标志
            buf.Get();
            //0x80
            buf.Get();
            //相位
            //
            buf.GetByteArray(4);
            //0x3f
            buf.Get();
            tagInfo.Phi_Uc = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);
            tagInfo.Phi_Ub = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);
            tagInfo.Phi_Ua = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);
            tagInfo.Phi_Ic = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);
            tagInfo.Phi_Ib = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);
            tagInfo.Phi_Ia = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);

            if (tagInfo.Phi_Ic > 0)
                tagInfo.Phi_Ic = tagInfo.Phi_Uc - tagInfo.Phi_Ic;
            else
                tagInfo.Phi_Ic = 0;
            if (tagInfo.Phi_Ib > 0)
                tagInfo.Phi_Ib = tagInfo.Phi_Ub - tagInfo.Phi_Ib;
            else
                tagInfo.Phi_Ib = 0;
            if (tagInfo.Phi_Ia > 0)
                tagInfo.Phi_Ia = tagInfo.Phi_Ua - tagInfo.Phi_Ia;
            else
                tagInfo.Phi_Ia = 0;

            if (tagInfo.Phi_Ic < 0)
                tagInfo.Phi_Ic += 360;
            else if (tagInfo.Phi_Ic > 360)
                tagInfo.Phi_Ic -= 360;

            if (tagInfo.Phi_Ib < 0)
                tagInfo.Phi_Ib += 360;
            else if (tagInfo.Phi_Ib > 360)
                tagInfo.Phi_Ib -= 360;

            if (tagInfo.Phi_Ia < 0)
                tagInfo.Phi_Ia += 360;
            else if (tagInfo.Phi_Ia > 360)
                tagInfo.Phi_Ia -= 360;

            if (tagInfo.Ia == 0)
                tagInfo.Phi_Ia = 0;
            if (tagInfo.Ib == 0)
                tagInfo.Phi_Ib = 0;
            if (tagInfo.Ic == 0)
                tagInfo.Phi_Ic = 0;
            //0xff
            buf.Get();
            //C相 B相 A相 相角
            buf.GetByteArray(4);
            buf.GetByteArray(4);
            buf.GetByteArray(4);

            //C相 B相 A相 有功功率因素
            buf.GetByteArray(4);
            buf.GetByteArray(4);
            buf.GetByteArray(4);

            tagInfo.COS = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);
            tagInfo.SIN = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);

            //0xff
            buf.Get();

            tagInfo.Pc = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Pb = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Pa = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.P = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));

            tagInfo.Qc = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Qb = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Qa = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Q = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));

            //0x0f
            buf.Get();
            tagInfo.Sc = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Sb = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Sa = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.S = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));

            tagInfo.COS = get3ByteValue(buf.GetByteArray(3), 5);
            tagInfo.SIN = get3ByteValue(buf.GetByteArray(3), 5);




        }

        private sbyte GetByteFromByteArray(byte pArray)
        {
            string Fmt16 = Convert.ToString(pArray, 16);
            sbyte ReturnValue = (Convert.ToSByte(Fmt16, 16));
            return ReturnValue;
        }
    }
    #endregion

    #endregion

    #region 485数据接收包
    /// <summary>
    /// 485数据接收包
    /// </summary>
    class Rs485RequestReplayPacket : ClouRecvPacket_CLT11
    {
        public byte[] Data
        {
            get;
            private set;
        }

        protected override void ParseBody(byte[] data)
        {
            Data = data;
        }
    }
    /// <summary>
    /// 485数据请求包
    /// </summary>
    internal class Rs485RequestPacket : SendPacket
    {
        public byte[] pata;
        public Rs485RequestPacket(bool needReturn)
        {
            IsNeedReturn = needReturn;
        }

        public override byte[] GetPacketData()
        {
            return pata;
        }

    }
    #endregion


    #region CL188E误差板

    /// <summary>
    /// 188联机操作请求包
    /// </summary>
    internal class CL188_RequestLinkPacket : CL188ESendPacket
    {

        public CL188_RequestLinkPacket()
            : base()
        {
        }
        /// <summary>
        /// 误差路数
        /// </summary>
        //public byte Pos = 0;

        public override string GetPacketName()
        {
            return "CL188_RequestLinkPacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x36);
            buf.Put(Pos);
            return buf.ToByteArray();
        }

    }
    /// <summary>
    /// 联机指令，返回数据包
    /// </summary>
    internal class CL188_RequestLinkReplayPacket : ClouRecvPacket_CLT11
    {
        public CL188_RequestLinkReplayPacket()
            : base()
        {
        }

        /// <summary>
        /// 返回路数
        /// </summary>
        public byte Pos
        {
            get;
            private set;
        }
        public override string GetPacketName()
        {
            return "CL188_RequestLinkPacketReplay";
        }

        /// <summary>
        /// 联机结果
        /// </summary>
        public bool LinkOk { get; private set; }

        protected override void ParseBody(byte[] data)
        {
            this.LinkOk = true;
            //data[0] 指令
            if (data[0] != 0x36)
            {
                this.ReciveResult = RecvResult.DataError;
                LinkOk = false;
                return;
            }
            this.Pos = data[1];
            this.ReciveResult = RecvResult.OK;
        }
    }
    /// <summary>
    /// 读取对标结果0x39
    /// </summary>
    internal class CL188_RequestReadDuiSheBiaoResultPacket : CL188ESendPacket
    {
        private byte m_pos = 0;
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="pos">表位号</param>
        public void SetPara(byte pos)
        {
            m_pos = pos;
        }

        public override string GetPacketName()
        {
            return "CL188_RequestReadDuiSheBiaoResultPacket";
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x39);
            buf.Put(m_pos);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 读取误差板对标是否成功回复包
    /// 0x39
    /// </summary>
    internal class CL188_RequestReadDuiSheBiaoResultReplayPacket : CL188ERecvPacket
    {
        public CL188_RequestReadDuiSheBiaoResultReplayPacket() : base() { }

        /// <summary>
        /// 结论
        /// </summary>
        public bool Result
        {
            get;
            set;
        }

        public override string GetPacketName()
        {
            return "CL188_RequestReadDuiSheBiaoResultReplayPacket";
        }

        protected override void ParseBody(byte[] data)
        {
            ByteBuffer buf = new ByteBuffer(data);
            buf.Get();
            Pos = buf.Get();
            Result = (buf.Get() == 1);
        }
    }

    /// <summary>
    /// 0x55指令
    /// </summary>
    internal class CL188_RequestSetStdDataPacket : CL188ESendPacket
    {
        public override string GetPacketName()
        {
            return "CL188_RequestSetStdDataPacket";
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x55);
            buf.Put(0x01);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 命令 50H
    /// 帧格式：帧头+50H+误差路数
    /// </summary>
    internal class CL188_RequestReadStatusPacket : CL188ESendPacket
    {
        protected override byte[] GetBody()
        {
            return new byte[2] { 0x50, Pos };
        }
    }
    /// <summary>
    /// 读取检定数据请求包
    /// 0x34
    /// </summary>
    internal class CL188_RequestReadVerifyDataPacket : CL188ESendPacket
    {
        /// <summary>
        /// 表位号
        /// </summary>
        public byte Pos = 0;
        public override string GetPacketName()
        {
            return "CL188_RequestReadVerifyDataPacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x34);
            buf.Put(Pos);
            return buf.ToByteArray();
        }
    }

    /// <summary>
    /// 选择脉冲通道请求包[无回复]0x46
    /// </summary>
    internal class CL188_RequestSelectPulseChannelPacket : CL188ESendPacket
    {
        private byte m_Gygy = 0;
        private byte m_PulseType = 0;
        private byte m_PulseChannelID = 0;

        public CL188_RequestSelectPulseChannelPacket()
            : base(false)
        {
        }
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="Pos">第N块误差板</param>
        /// <param name="GygyType">共阴共阳设置0:共阳,1:共阴</param>
        /// <param name="PulseType">脉冲通道口，0:1#;1:2#[光电头,脉冲盒]</param>
        /// <param name="PulseChannelID">脉冲通道号</param>
        public void SetPara(byte pos, byte GygyType, byte PulseType, byte PulseChannelID)
        {

            this.Pos = pos;
            m_Gygy = GygyType;
            m_PulseType = PulseType;
            m_PulseChannelID = PulseChannelID;
        }

        public override string GetPacketName()
        {
            return "CL188_RequestSelectPulseChannelPacket";
        }
        /// <summary>
        /// 说明：选择被检脉冲通道，Bit0、Bit1、Bit2表示通道号，如bi2bit1bit0=0自动设置为光电头，Bit4为0
        ///表示公共端输出高电平（共阳），Bit4为1输出低电平（共阴）Bit7=0:选择1号被检脉冲口；
        ///bit7=8:选择2号口。
        /// </summary>
        /// <param name="buf"></param>
        /// 
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x46);
            buf.Put(Pos);
            m_PulseChannelID = (byte)(m_PulseChannelID | (m_PulseType << 7) | (m_Gygy << 3));
            buf.Put(m_PulseChannelID);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 设置脉冲间隔时间及脉冲个数请求包
    /// 0x41
    /// </summary>
    internal class CL188_RequestSetDemandIntervalPacket : CL188ESendPacket
    {
        /// <summary>
        /// 表位号
        /// </summary>
        private byte Pos = 0;

        private int PulseSpaceTime = 0;

        private ushort PulseCount = 0;
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="pos">表位</param>
        /// <param name="psapce">脉冲间隔时间</param>
        /// <param name="pcount">脉冲个数</param>
        public void SetPara(byte pos, int psapce, ushort pcount)
        {
            Pos = pos;
            PulseSpaceTime = psapce;
            PulseCount = pcount;
        }

        public override string GetPacketName()
        {
            return "CL188_RequestSetDemandIntervalPacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x41);
            buf.Put(Pos);
            buf.PutInt(PulseSpaceTime);
            buf.PutUShort(PulseCount);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 14、	命令 44H
    ///帧格式：帧头+44H+误差路数+被检时钟频率的100倍（3Bytes）+被检脉冲个数（3Bytes）
    ///返回：无
    ///说明：时钟日误差测试，设置被检时钟，如误差路数为FFH，则表示广播指令。
    /// </summary>
    internal class CL188_RequestSetDayTimePacket : CL188ESendPacket
    {
        private byte Pos = 0;
        private int pulseCount = 0;
        //
        public void SetPara(int meterNo,int pulseSum)
        {
            Pos = Convert.ToByte(meterNo);
            pulseCount = pulseSum;
        }
        public override string GetPacketName()
        {
            return "CL188_RequestSetDayTimePacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x44);
            buf.Put(Pos);
            buf.Put(0x00);
            buf.Put(0x00);
            buf.Put(Convert.ToByte(100));
            buf.Put(0x00);
            buf.Put(0x00);
            buf.Put(Convert.ToByte(pulseCount));
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 帧格式：帧头+45H+误差路数+标准时钟频率100倍（4Bytes）
    ///返回：无
    ///说明：时钟日误差测试，设定标准时钟频率，预设为524288HZ
    /// </summary>
    internal class CL188_RequestSetclockfreqPacket : CL188ESendPacket
    {
        //private byte Pos = 0;
        private int stdClockFreq = 0;
        //
        public void SetPara(int stdClockFreq)
        {
            this.stdClockFreq = stdClockFreq;
        }
        public override string GetPacketName()
        {
            return "CL188_RequestSetDayTimePacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x45);
            buf.Put(0xFF);
            buf.PutInt(stdClockFreq);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 设置/取消对标指令
    /// </summary>
    internal class CL188_RequestSetDuiSheBiaoPacket : CL188ESendPacket
    {
        private byte m_isDsb = 0x37;
        /// <summary>
        /// 表位
        /// </summary>
        public byte Pos = 0;


        public CL188_RequestSetDuiSheBiaoPacket()
            : base(false)
        {

        }
        /// <summary>
        /// 设置参数：是否是对标T:是;F:取消对标
        /// </summary>
        /// <param name="isDuiSheBiao">操作类型</param>
        public void SetPara(bool isDuiSheBiao)
        {
            m_isDsb = (byte)(isDuiSheBiao ? 0x37 : 0x38);
            // Pos = pos;
        }

        public override string GetPacketName()
        {
            return "CL188_RequestSetDuiSheBiaoPacket";
        }

        protected override byte[] GetBody()
        {
            return new byte[] { this.m_isDsb };
        }
    }
    /// <summary>
    /// 设置被检表脉冲常数及校验圈数请求包
    /// </summary>
    internal class CL188_RequestSetMeterConstAndVerifyCirclePacket : CL188ESendPacket
    {

        private byte m_Pos = 0;
        private bool m_isSameMeterConst = true;
        private bool m_isSameCircle = true;

        private int[] m_arrMeterConst = new int[0];
        private int[] m_arrCircle = new int[0];
        private int m_MeterConst = 0;
        private int m_Circle = 0;

        public CL188_RequestSetMeterConstAndVerifyCirclePacket()
            : base(false)
        { }

        /// <summary>
        /// 统一设置被检表常数及检验圈数
        /// </summary>
        /// <param name="Pos">表位</param>
        /// <param name="meterconst">表常数</param>
        /// <param name="circle">检定圈数</param>
        public void SetPara(int meterconst, int circle)
        {
            m_isSameMeterConst = true;
            m_isSameCircle = true;
            //m_MeterConst = meterconst*100;
            m_MeterConst = meterconst;
            m_Circle = circle;
        }

        /// <summary>
        /// 设置被检表常数及检验圈数
        /// </summary>
        /// <param name="meterconst">表常数</param>
        /// <param name="circle">检定圈数</param>
        public void SetPara(byte Pos, int[] meterconst, int[] circle)
        {
            m_isSameMeterConst = false;
            m_isSameCircle = false;
            m_Pos = Pos;
            m_arrMeterConst = meterconst;
            m_arrCircle = circle;
            if (meterconst.Length != circle.Length)
                throw new Exception("meterconst 和 circle 数组长度不一致");
            //m_isSameMeterConst = isSameArray(meterconst);
            //m_isSameCircle = isSameArray(circle);
            if (m_isSameCircle && m_isSameMeterConst)
            {
                //m_MeterConst = meterconst[0]*100;
                m_MeterConst = meterconst[0];
                m_Circle = circle[0];
            }
        }

        public override string GetPacketName()
        {
            return "CL188_RequestSetMeterConstAndVerifyCirclePacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            if (m_isSameMeterConst && m_isSameCircle)
            {
                buf.Put(0x33);
                buf.PutInt(m_MeterConst);
                buf.PutInt(m_Circle);
            }
            else
            {
                buf.Put(0x32);
                buf.Put(m_Pos);
                for (int i = 0; i < m_arrMeterConst.Length; i++)
                {
                    buf.PutInt(m_arrMeterConst[i]);
                    //buf.PutInt(m_arrMeterConst[i]*100);
                    buf.PutInt(m_arrCircle[i]);
                }
            }
            return buf.ToByteArray();
        }
    }
    /// <summary>
    ///   带一个参数类型包
    ///   参数:byte 1
    ///   表号:byte 1
    ///   控制码:byte 1
    /// </summary>
    internal class CL188_RequestSetOneParaBytePacket : CL188ESendPacket
    {
        /// <summary>
        /// 数据域
        /// </summary>
        public byte Data = 0;
        /// <summary>
        /// 表位号，如果不需要请保持
        /// </summary>
        public byte Pos = 0xFE;
        /// <summary>
        /// 控制码
        /// </summary>
        public byte CmdCode = 0x00;

        public CL188_RequestSetOneParaBytePacket(bool bReturn)
            : base(bReturn)
        { }
        public override string GetPacketName()
        {
            return "CL188_RequestSetOneParaBytePacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(CmdCode);
            if (Pos != 0xFe)
                buf.Put(Pos);
            buf.Put(Data);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    ///   带一个参数类型包
    ///   参数:byte 1
    ///   表号:byte 1
    ///   控制码:byte 1
    /// </summary>
    internal class CL188_RequestSetOneParaIntPacket : CL188ESendPacket
    {

        public CL188_RequestSetOneParaIntPacket(bool bReturn)
            : base(bReturn)
        { }

        /// <summary>
        /// 数据域
        /// </summary>
        public int Data = 0;
        /// <summary>
        /// 表位号，如果不需要请保持为-1
        /// </summary>
        public byte Pos = 0xFE;
        /// <summary>
        /// 控制码
        /// </summary>
        public byte CmdCode = 0x00;
        public override string GetPacketName()
        {
            return "CL188_RequestSetOneParaBytePacket";
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(CmdCode);
            if (Pos != 0xFE)
                buf.Put(Pos);
            buf.PutInt(Data);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    ///   带一个参数类型包
    ///   参数:byte 1
    ///   表号:byte 1
    ///   控制码:byte 1
    /// </summary>
    internal class CL188_RequestSetOneParaUShortPacket : CL188ESendPacket
    {
        /// <summary>
        /// 数据域
        /// </summary>
        public ushort Data = 0;
        /// <summary>
        /// 表位号，如果不需要请保持为-1
        /// </summary>
        public byte Pos = 0xFE;
        /// <summary>
        /// 控制码
        /// </summary>
        public byte CmdCode = 0x00;
        public override string GetPacketName()
        {
            return "CL188_RequestSetOneParaBytePacket";
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(CmdCode);
            if (Pos != 0xFe)
                buf.Put(Pos);
            buf.PutUShort(Data);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 设置标准表常数0x31
    /// </summary>
    internal class CL188_RequestSetStdMeterConstPacket : CL188ESendPacket
    {

        private int m_stdMeterConst = 0;
        private ushort m_fenpingxishu = 1;

        public CL188_RequestSetStdMeterConstPacket()
            : base(false)
        {
        }

        /// <summary>
        /// 设置参数
        /// 0<分频系数<7fffH,最高位Bit15用于表示是否使用互感器：Bit15=0：未使用  Bit15=1：使用
        /// </summary>
        /// <param name="stdmeterconst">标准脉冲常数</param>
        /// <param name="fenpinxishu"> 分频系数</param>
        public void SetPara(int stdmeterconst, ushort fenpinxishu)
        {
            //m_stdMeterConst = stdmeterconst * 100;
            m_stdMeterConst = stdmeterconst;
            m_fenpingxishu = fenpinxishu;
        }
        /// <summary>
        /// 设置标准表常数 
        /// </summary>
        /// <returns></returns>
        public override string GetPacketName()
        {
            return "CL188_RequestSetStdMeterConstPacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x31);
            buf.PutInt(m_stdMeterConst);
            buf.PutUShort(m_fenpingxishu);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 设置试验功能类型请求包
    /// </summary>
    internal class CL188_RequestSetTaskTypePacket : CL188ESendPacket
    {
        private TaskType tType = TaskType.电能误差;
        /// <summary>
        /// 试验类型
        /// </summary>
        public enum TaskType
        {
            电能误差 = 0,
            需量周期 = 1,
            时钟日误差 = 2,
            脉冲计数 = 3,
            电流开路 = 4,
            电流接触电阻 = 5,
            电压短路 = 6,
        }
        public CL188_RequestSetTaskTypePacket()
            : base(false)
        {

        }
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="type"></param>
        public void SetPara(byte pos, TaskType type)
        {
            Pos = pos;
            tType = type;
        }

        public override string GetPacketName()
        {
            return "CL188_RequestSetTaskTypePacket";
        }


        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x47);
            buf.Put(Pos);
            buf.Put((byte)tType);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 设置日计时误差频率
    /// </summary>
    internal class CL188_RequestSetTimeErrorOfDayFreqPacket : CL188ESendPacket
    {
        /// <summary>
        /// 表位号
        /// </summary>
        private byte Pos = 0;
        private int Freq = 0;
        private int PulseCount = 0;


        public CL188_RequestSetTimeErrorOfDayFreqPacket()
            : base(false)
        {
            //不需要回复
        }
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="pos">表位号FF为广播</param>
        /// <param name="freq">被检表时钟频率</param>
        /// <param name="pcount">被检脉冲个数</param>
        public void SetPara(byte pos, int freq, int pcount)
        {
            Pos = pos;
            Freq = freq;
            PulseCount = pcount;
        }
        public override string GetPacketName()
        {
            return "CL188_RequestSetTimeErrorOfDayFreqPacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            //Freq = Freq * 100;//注意，这里不能这样写，值会变，fjk修改如下，这种问题找起来最头疼
            //PulseCount *= 10;
            buf.Put(0x44);
            buf.Put(Pos);
            buf.PutInt((Freq * 100) << 8);//fjk修改
            buf.Position--;
            buf.PutInt((PulseCount * 10) << 8);//fjk修改
            buf.Position--;
            //return buf.ToByteArray();

            byte[] bytReturn = new byte[buf.ToByteArray().Length - 1];
            Array.Copy(buf.ToByteArray(), 0, bytReturn, 0, bytReturn.Length);
            return bytReturn;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal class CL188_RequestSetTimeErrorOfDayStdFreqPacket : CL188ESendPacket
    {
        /// <summary>
        /// 表位号
        /// </summary>
        private byte m_Pos = 0;

        private int m_StdFreq = 0;
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="pos">表位:0XFF广播</param>
        /// <param name="freq">标准时钟频率</param>
        public void SetPara(byte pos, int freq)
        {
            m_Pos = pos;
            m_StdFreq = freq * 100;
        }
        public override string GetPacketName()
        {
            return "CL188_RequestSetTimeErrorOfDayStdFreqPacket";
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x45);
            buf.Put(m_Pos);
            buf.PutInt(m_StdFreq);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 控制误差板启动/停止/继续
    /// 48-4A指令
    /// </summary>
    internal class CL188_RequestStartStopPacket : CL188ESendPacket
    {

        public CL188_RequestStartStopPacket()
            : base(false)
        {
        }

        /// <summary>
        /// 操作类型
        /// </summary>
        public enum ControlType
        {
            启动当前功能 = 0x48,
            停止当前功能 = 0x49,
            继续当前功能 = 0x4A,
        }

        /// <summary>
        /// 误差板序号
        /// </summary>
        public byte Pos = 0;
        /// <summary>
        /// 操作类型
        /// </summary>
        public ControlType ControlTypes = ControlType.启动当前功能;

        public override string GetPacketName()
        {
            return "CL188_RequestStartStopPacket";
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put((byte)ControlTypes);
            buf.Put(Pos);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 返回：帧头+50H+误差路数+检定类型+被检脉冲通道号+标准常数（4Bytes）+被检常数（4Bytes）+标准常数标志（/100）+分频系数
    ///说明：
    ///1、	读取误差计算器工作状态、被检脉冲通道号(Bit7=1：设置通道失败)。
    ///2、	如当前工作为计算时钟日误差，则标准常数为标准时钟频率X100；被检常数为被检时钟频率X100，最后一个字节表示脉冲个数。
    ///3、	如当前工作为需量周期误差计算，则标准常数为脉冲间隔时间，被检常数为脉冲个数。
    /// </summary>
    internal class CL188_RequestReadStatusReplyPacket : CL188ERecvPacket
    {
        //50-01-00-09-23-C3-46-00-00-07-A1-20-00-00-01-00-00-00-00
        /// <summary>
        /// 当前误差板功能类型
        /// </summary>
        public enmTaskType TaskType { get; private set; }
        /// <summary>
        /// 当前脉冲通道号
        /// </summary>
        public byte PulseChannel { get; private set; }
        /// <summary>
        /// 共阴共阳类型
        /// </summary>
        public Cus_GyGyType PulseGYGYType { get; private set; }

        /// <summary>
        /// 使用脉冲盒还是光电头
        /// </summary>
        public Cus_PulseType PulseType { get; private set; }
        /// <summary>
        /// 标准表常数
        /// </summary>
        public int StdMeterConst { get; private set; }
        /// <summary>
        /// 被检表常数
        /// </summary>
        public int MeterConst { get; private set; }
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length < 19) return;
            ByteBuffer buf = new ByteBuffer(data);
            buf.Get();//0x50 控制码
            Pos = buf.Get();
            TaskType = (enmTaskType)buf.Get();
            /*
            ，如bi2bit1bit0=0自动设置为光电头，；
 

             */
            byte tmp = buf.Get();
            PulseChannel = (byte)(tmp & 7);// Bit0、Bit1、Bit2表示通道号
            PulseGYGYType = (Cus_GyGyType)(((tmp & 8) >> 3 + 1) % 2);//Bit4为0 表示公共端输出高电平（共阳），Bit4为1输出低电平（共阴）
            PulseType = (Cus_PulseType)((tmp & 0x40) >> 3);//Bit7=0:选择1号被检脉冲口 bit7=8:选择2号口
            StdMeterConst = buf.GetInt();
            MeterConst = buf.GetInt();
        }
    }
    /// <summary>
    /// 读取误差数据应答包
    /// </summary>
    internal class CL188_RequestReadVerifyDataReplayPacket : CL188ERecvPacket
    {
        private enmTaskType m_taskType = enmTaskType.电能误差;

        private WorkFlow workFlow = WorkFlow.None;

        public CL188_RequestReadVerifyDataReplayPacket(enmTaskType taskType, WorkFlow workFlow)
            : base()
        {
            m_taskType = taskType;
            this.workFlow = workFlow;
        }


        /// <summary>
        /// 误差次数
        /// </summary>
        public int WcTimes
        {
            get;
            private set;
        }
        /// <summary>
        /// 数据
        /// </summary>
        public float Data
        {
            get;

            private set;
        }
        public override string GetPacketName()
        {
            return "CL188_RequestReadVerifyDataReplayPacket";
        }

        protected override void ParseBody(byte[] data)
        {
            //if (data == null) ; return;
            //if (data.Length < 7) ; return;
            ByteBuffer buf = new ByteBuffer(data);
            buf.Get();
            int mydata = buf.GetInt();
            Data = getData(mydata);

            //启动、潜支读数 1 合格，0不合格
            if (workFlow == WorkFlow.潜动)
            {
                if (Data >= 1)
                    Data = 0;
                else
                    Data = 1;
            }
            if (workFlow == WorkFlow.启动)
            {
                if (Data >= 1)
                {
                    Data = 1;
                }
                else
                {
                    Data = 0;
                }
            }
            buf.Get();
            //buf.Get();
            WcTimes =Convert.ToInt32(data[6]);
        }

        /// <summary>
        /// 计算误差
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private float getData(int data)
        {
            byte tmp = (byte)(data >> 24); //包括符号及小数点位数
            data = data & 0xFFFFFF;       //取出其余3个字节
            byte dot = (byte)(tmp >> 4);
            if ((tmp & 0xF) > 0)
                data *= -1;
            return (float)(data / Math.Pow(10, dot));
        }
    }


    #endregion

    #region 303功率源

    /// <summary>
    /// 源联机指令
    /// 0x52是字母"R"的ASC码
    /// ox4F是O的ASC码
    /// </summary>
    internal class CL303_RequestLinkPacket : CL303SendPacket
    {

        /// <summary>
        /// 是否是联机
        /// </summary>
        public bool IsLink = true;


        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            if (IsLink)
                buf.Put(0x52);
            else
                buf.Put(0x4F);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 读取源状态
    /// </summary>
    internal class CL303_RequestReadPowerStatePacket : CL303SendPacket
    {
        protected override byte[] GetBody()
        {
            return new byte[1] { 0x42 };
        }
    }
    /// <summary>
    /// 返回源状态
    /// </summary>
    internal class CL303_RequestReadPowerStateReplyPacket : CL303RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            ;
        }
    }
    /// <summary>
    /// 读取CL303源版本号
    /// 发送字母V 的ASC码
    /// </summary>
    internal class CL303_RequestReadVersionPacket : CL303SendPacket
    {
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x56);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 读取CL303版本号返回包
    /// </summary>
    internal class CL303_RequestReadVersionReplayPacket : CL303RecvPacket
    {
        public CL303_RequestReadVersionReplayPacket() : base() { }

        /// <summary>
        /// 读取到的版本号
        /// 默认值为Unknown
        /// </summary>
        public string Version { get; private set; }


        protected override void ParseBody(byte[] data)
        {
            Version = ASCIIEncoding.UTF8.GetString(data);
        }
    }
    /// <summary>
    /// 设置频率指令
    /// ox33
    /// 返回：CLNormalRequestResultReplayPacket
    /// </summary>
    internal class CL303_RequestSetFreqPacket : CL303SendPacket
    {
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x33);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 请求关源指令
    /// 返回:CLNormalRequestResultReplayPacket
    /// </summary>
    internal class CL303_RequestSetPowerOffPacket : CL303SendPacket
    {
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x45);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// ox30指令升源
    /// </summary>
    internal class CL303_RequestSetPowerOnPacket0x30 : CL303SendPacket
    {
        /// <summary>
        /// 电压参数
        /// </summary>
        public struct UPara
        {
            public float Ua;
            public float Ub;
            public float Uc;
            public float Ia;
            public float Ib;
            public float Ic;
        }
        private UPara m_Upara;
        public struct PhiPara
        {
            public float PhiUa;
            public float PhiUb;
            public float PhiUc;
            public float PhiIa;
            public float PhiIb;
            public float PhiIc;
        }
        private PhiPara m_Phipara;
        private byte m_xwkz = 63;
        private byte m_xiebo = 0;
        private float m_freq = 50.0f;

        /// <summary>
        /// 谐波开关设置,//fjk 修改为m_xiebo =   原先m_xwkz =
        /// </summary>
        public bool OpenXieBo { set { m_xiebo = value ? (byte)0xFF : (byte)0; } }
        /// <summary>
        /// 设置频率,默认值50hz
        /// </summary>
        public float Freq { set { m_freq = value; } get { return m_freq; } }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="upara">电压参数</param>
        /// <param name="phipara"></param>
        public void SetPara(byte clfs, UPara upara, PhiPara phipara)
        {
            m_Upara = upara;
            m_Phipara = phipara;
            if (clfs == 7)
                m_xwkz &= 9;
            else if (clfs == 6)
                m_xwkz &= 45;
        }

        public override string GetPacketName()
        {
            return "CL303_RequestSetPowerOnPacket0x30";
        }

        public CL303_RequestSetPowerOnPacket0x30()
        {
            this.ToID = 0x20;
        }


        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x30);
            buf.Put(m_xwkz);                //相位控制
            buf.Put(m_xiebo);               //谐波开关
            buf.Put(get10bitData(Freq));    //频率
            //UA
            buf.Put(GetUScale(m_Upara.Ua));       //
            buf.Put(get10bitData(m_Upara.Ua));    //
            buf.Put(get10bitData(m_Phipara.PhiUa)); //
            //Ub
            buf.Put(GetUScale(m_Upara.Ub));       //
            buf.Put(get10bitData(m_Upara.Ub));    //
            buf.Put(get10bitData(m_Phipara.PhiUb)); //

            //Uc
            buf.Put(GetUScale(m_Upara.Uc));       //
            buf.Put(get10bitData(m_Upara.Uc));    //
            buf.Put(get10bitData(m_Phipara.PhiUc)); //

            //Ia
            buf.Put(GetIScale(m_Upara.Ia));       //
            buf.Put(get10bitData(m_Upara.Ia));    //
            buf.Put(get10bitData(m_Phipara.PhiIa)); //
            //Ib
            buf.Put(GetIScale(m_Upara.Ib));       //
            buf.Put(get10bitData(m_Upara.Ib));    //
            buf.Put(get10bitData(m_Phipara.PhiIb)); //
            //Ic
            buf.Put(GetIScale(m_Upara.Ic));       //樊江凯，修改--档位错误
            buf.Put(get10bitData(m_Upara.Ic));    //
            buf.Put(get10bitData(m_Phipara.PhiIc)); //
            if (buf.ToByteArray().Length != 139)
                throw new Exception(GetPacketName() + "数据包长度不对");
            return buf.ToByteArray();
        }
        private byte GetIScale(Single sngI)
        {
            //if (sngI <= 0.01) return 80;            //"50";
            //else if (sngI <= 0.025) return 81;        //"51";
            //else if (sngI <= 0.05) return 82;        // "52";
            //else if (sngI <= 0.1) return 83;       // "53";
            //else if (sngI <= 0.25) return 84;        //"54";
            //else if (sngI <= 0.5) return 85;        //"55";
            //else if (sngI <= 1) return 86;        //"56";
            //else if (sngI <= 2.5) return 87;          // "57";
            //else if (sngI <= 5) return 88;          //"58";
            //else if (sngI <= 10) return 89;         //"59";
            //else if (sngI <= 25) return 90;         //"5a";
            //else if (sngI <= 50) return 91;         //"5b";
            //else if (sngI <= 100) return 92;        // "5c";
            //else return 92;// "5c";

            if (sngI <= 0.012) return 80;            //"50";
            else if (sngI <= 0.03) return 81;        //"51";
            else if (sngI <= 0.06) return 82;        // "52";
            else if (sngI <= 0.12) return 83;       // "53";
            else if (sngI <= 0.3) return 84;        //"54";
            else if (sngI <= 0.6) return 85;        //"55";
            else if (sngI <= 1.2) return 86;        //"56";
            else if (sngI <= 3) return 87;          // "57";
            else if (sngI <= 6) return 88;          //"58";
            else if (sngI <= 12) return 89;         //"59";
            else if (sngI <= 30) return 90;         //"5a";
            else if (sngI <= 60) return 91;         //"5b";
            else if (sngI <= 120) return 92;        // "5c";
            else return 92;// "5c";
        }

        private byte GetUScale(Single sngU)
        {
            if (sngU <= 57 * 1.2) return 64;//"40";
            else if (sngU <= 120) return 65;// "41";
            else if (sngU <= 264) return 66;// "42";
            else if (sngU <= 480) return 67;//"43";
            else if (sngU <= 900) return 68;//"44";
            else return 66;// "42";
        }
    }
    /// <summary>
    /// 相同电流电压源输出指令
    /// 返回4B则成功
    /// </summary>
    internal class CL303_RequestSetPowerOnPacket0x35 : CL303SendPacket
    {
        private byte m_clfs = 0;
        private byte m_xwkz = 0;
        private bool m_xiebo = false;
        private float m_U = 0;
        private float m_I = 0;
        private bool m_isDuiSheBiao = false;
        private float m_phi = 0;
        private byte m_BuPingHeng = 0;//不平衡负载
        /// <summary>
        /// 设置频率，默认为50HZ
        /// </summary>
        public float Freq = 50;
        /// <summary>
        /// 电压百分比
        /// </summary>
        public float PercentOfU = 100F;
        /// <summary>
        /// 是否是潜动
        /// </summary>
        public bool IsCreeping = false;
        /// <summary>
        /// 元件
        /// </summary>
        public Cus_PowerYuanJiang m_YuanJian = Cus_PowerYuanJiang.H;

        /// <summary>
        /// 设置源控制参数
        /// </summary>
        /// <param name="clfs">
        /// 测量方式
        /// 0表示PT4       1表示QT4    2表示P32 
        /// 3表示Q32       4表示Q60    5表示Q90
        /// 6表示Q33       7表示P
        /// </param>
        /// <param name="xiebo"></param>
        /// <param name="U"></param>
        /// <param name="I"></param>
        /// <param name="duisebiao"></param>
        /// <param name="Phi"></param>
        public void SetPara(byte clfs, byte xwkz, bool xiebo,
                            float U, float I, bool duisebiao, float Phi)
        {
            m_clfs = clfs;
            m_isDuiSheBiao = duisebiao;
            m_xwkz = 63;
            m_xwkz = xwkz;
            m_xiebo = xiebo;
            m_U = U;
            m_I = I;
            m_phi = Phi;
            //计算不平衡负载
            updateClfs();       //更新测量方式字节 

        }

        /// <summary>
        /// 计算测量方式:
        /// </summary>
        private void updateClfs()
        {
            m_clfs = (byte)(m_isDuiSheBiao ? m_clfs ^ 128 : m_clfs & 127);  //对标标志
            m_clfs = (byte)(m_clfs & 191);                                  //缓降
            m_clfs = (byte)(m_clfs & 223);                                  //闭环
            //m_clfs = 0x00;
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            /*
            接线方式          1Byte    	
            不平衡负载        1Byte    	
            波段开关          1 Byte   	
            谐波开关          1Byte    	
            电压档位          1 Byte	2008512 20090034
            电流档位          1 Byte	
            电压幅度          4 Byte	值×6553600
            电流幅度          4 Byte	值×6553600
            φ                4 Byte	值×6553600
            频率              4 Byte	值×6553600
            负载率            4 Byte	值×6553600

             */
            byte boduan = GetBoDuan();
            buf.Put(0x35);              //CMD    
            buf.Put(m_clfs);            //接线方式
            buf.Put(boduan);                 //只让加电流开关有效
            if (m_isDuiSheBiao)
                buf.Put(0x0f);
            else
                buf.Put(m_xwkz);            //相位开关 即ABC电压ABC电流，哪相输出哪相关闭
            buf.PutInt(0xFFFFFFFF);
            buf.PutUShort(0xFFFF);
            if (m_xiebo)
                buf.Put(0xFF);
            else
                buf.Put(0);
            buf.Put(GetUScale(m_U));
            if (m_isDuiSheBiao)
                buf.Put(86);
            else
                buf.Put(GetIScale(m_I));
            buf.PutInt((int)(m_U * 65536));
            buf.PutInt((int)(m_I * 65536 * 100));
            buf.PutInt((int)(m_phi * 65536));
            buf.PutInt((int)(Freq * 65536));
            buf.Put(0);
            buf.Put(100);
            buf.PutUShort(0);
            //81:20:00:26:35:01:18:3f:ff:ff:ff:ff:ff:ff:00:42:57:00:dc:00:00:00:96:00:00:00:5a:00:00:00:32:00:00:00:64:00:00:46
            //            35-00-18-3F-FF-FF-FF-FF-FF-FF-00-42-57-00-DC-00-00-00-96-00-00-00-5A-00-00-00-32-00-00-00-64-00-00
            return buf.ToByteArray();
        }
        /// <summary>
        /// 获取波段控制
        /// </summary>
        /// <returns></returns>
        private byte GetBoDuan()
        {
            byte Tb = 0;
            if (IsCreeping)
                Tb = 0x80;
            byte Tb2 = 0;
            if (PercentOfU == 0 || PercentOfU == 100)
                Tb2 = 0x10;
            else if (PercentOfU == 110)
                Tb2 = 0x20;
            else if (PercentOfU == 115)
                Tb2 = 0x30;
            else if (PercentOfU == 120)
                Tb2 = 0x40;
            Tb += Tb2;

            if (m_I > 0F)
                Tb += 8;
            Tb += (byte)(((byte)m_YuanJian) - 1);
            return Tb;
        }

        private byte GetIScale(Single sngI)
        {
            if (sngI <= 0.012) return 80;            //"50";
            else if (sngI <= 0.03) return 81;        //"51";
            else if (sngI <= 0.06) return 82;        // "52";
            else if (sngI <= 0.12) return 83;       // "53";
            else if (sngI <= 0.3) return 84;        //"54";
            else if (sngI <= 0.6) return 85;        //"55";
            else if (sngI <= 1.2) return 86;        //"56";
            else if (sngI <= 3) return 87;          // "57";
            else if (sngI <= 6) return 88;          //"58";
            else if (sngI <= 12) return 89;         //"59";
            else if (sngI <= 30) return 90;         //"5a";
            else if (sngI <= 60) return 91;         //"5b";
            else if (sngI <= 120) return 92;        // "5c";
            else return 92;// "5c";
        }

        private byte GetUScale(Single sngU)
        {
            if (sngU <= 57 * 1.2) return 64;//"40";
            else if (sngU <= 120) return 65;// "41";
            else if (sngU <= 264) return 66;// "42";
            else if (sngU <= 480) return 67;//"43";
            else if (sngU <= 900) return 68;//"44";
            else return 66;// "42";
        }
    }
    /// <summary>
    /// 设置电压跌落试验请求包0x36
    /// 返回包:CLNormalRequestResultReplayPacket
    /// </summary>
    internal class CL303_RequestSetVoltageFallOffPacket : CL303SendPacket
    {
        /// <summary>
        /// 试验方式
        /// </summary>
        public byte VerifyType = 0;

        public override string GetPacketName()
        {
            return "CL303_RequestSetVoltageFallOffPacket";
        }

        public CL303_RequestSetVoltageFallOffPacket()
        {
            this.ToID = 0x20;
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="upara">电压参数</param>
        /// <param name="phipara"></param>
        public void SetPara(byte verifyType)
        {
            VerifyType = verifyType;
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x36);
            buf.Put(VerifyType);
            return buf.ToByteArray();
        }

        //protected override byte FrameLengthByteCount
        //{
        //    get { throw new NotImplementedException(); }
        //}
    }
    /// <summary>
    /// 设置谐波请求包[0x32]
    /// </summary>
    internal class Cl303_RequestSetXieBoPacket : CL303SendPacket
    {
        /// <summary>
        /// 是否加谐波
        /// </summary>
        public bool AddXieBo { get; set; }
        /// <summary>
        /// 当前加谐波的元件
        ///  A相电压 = 0,
        ///B相电压 = 1,
        ///C相电压 = 2,
        ///A相电流 = 3,
        ///B相电流 = 4,
        ///C相电流 = 5
        /// </summary>
        public byte YuanJian { get; set; }
        float[] content = new float[64];
        float[] phase = new float[64];
        public Cl303_RequestSetXieBoPacket()
        {
            AddXieBo = true;
        }

        public void SetPara(float[] contents, float[] phases)
        {
            content = contents;
            phase = phases;
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.SetLength(424);
            buf.Position = 0;
            buf.Put(0x32);                      //CMD
            buf.Put(YuanJian);                  //当前相位
            buf.PutUShort(0xFFFF);              //波开关
            buf.Put(0xFF);                      //含量状态
            for (int i = 0; i < 21; i++)
            {
                byte[] bytData = To10Bit(content[i]);
                buf.Put(bytData);//含量
                bytData = To10Bit(phase[i]);
                buf.Put(bytData);//相位
            }
            return buf.ToByteArray();
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

    }


    #endregion

    #region 311标准表

    /// <summary>
    /// 标准表联机/脱机请求包
    /// </summary>
    internal class CL311_RequestLinkPacket : Cl311SendPacket
    {
        public bool IsLink = true;

        public CL311_RequestLinkPacket()
        {
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            if (IsLink)
                buf.Put(0x60);
            else
                buf.Put(0x63);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 标准表，联机返回指令
    /// </summary>
    internal class Cl311_RequestLinkReplyPacket : Cl311RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 2)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[1] == 0x4b)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }
    /// <summary>
    /// 一个参数请求包
    /// </summary>
    internal class CL311_RequestReadDataOnlyCmdCodePacket : Cl311SendPacket
    {
        private byte m_CmdCode = 0;
        public CL311_RequestReadDataOnlyCmdCodePacket(byte cmd)
        {
            m_CmdCode = cmd;
            ToID = 0x16;
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_CmdCode);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 读取Harmonious请求包
    /// </summary>
    internal class CL311_RequestReadHarmoniousPacket : Cl311SendPacket
    {

        private byte[] m_readtype = new byte[0];
        public byte ReadType
        {
            set
            {
                if (value == 0)
                    m_readtype = new byte[] { 0x32, 0x80, 0x09 };
                else if (value == 1)
                    m_readtype = new byte[] { 0x32, 0xa0, 0x09 };
                else if (value == 2)
                    m_readtype = new byte[] { 0x32, 0xc0, 0x09 };
                else if (value == 3)
                    m_readtype = new byte[] { 0x32, 0xe0, 0x09 };
                else if (value == 4)
                    m_readtype = new byte[] { 0x32, 0x00, 0x0a };
                else
                    m_readtype = new byte[] { 0x32, 0x20, 0x0a };
            }
        }

        public override string GetPacketName()
        {
            return "CL311_RequestReadHarmoniousPacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x32);
            buf.Put(m_readtype);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 读取Harmonious回复包
    /// </summary>
    partial class CL311_RequestReadHarmoniousReplayPacket : Cl311RecvPacket
    {
        public override string GetPacketName()
        {
            return "CL311_RequestReadHarmoniousReplayPacket";
        }

        protected override void ParseBody(byte[] data)
        {
            throw new Exception();
        }
    }
    /// <summary>
    /// 35指令,0x12读取标准表常数 0x13读取标准表脉冲数
    /// </summary>
    class CL311_RequestReadStdMeterConstOrPulsePacket : Cl311SendPacket
    {
        private byte m_data = 0x12;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readStdConst"></param>
        public CL311_RequestReadStdMeterConstOrPulsePacket(bool readStdConst)
        {
            ToID = 0x16;
            if (readStdConst)
                m_data = 0x12;
            else
                m_data = 0x13;
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x35);
            buf.Put(m_data);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 0x12读取标准表常数 0x13读取标准表脉冲数 回复包
    /// </summary>
    class Cl311_RequestReadStdMeterConstOrPulseReplayPacket : Cl311RecvPacket
    {

        public int Data
        {
            get;
            private set;
        }

        protected override void ParseBody(byte[] data)
        {
            if (data.Length != 6)
            {
                this.ReciveResult = RecvResult.DataError;
            }
            else
            {
                byte[] datatemp = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    datatemp[i] = data[5 - i];
                }
                this.Data = DataFormart.HexStrToBin(DataFormart.byteToHexStr(datatemp));
            }
        }
    }

    /// <summary>
    /// 读取检定数据请求包
    /// </summary>
    internal class CL311_RequestReadVerifyDataPacket : Cl311SendPacket
    {
        public override string GetPacketName()
        {
            return "CL311_RequestReadVerifyDataPacket";
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x62);
            buf.Put(0x02);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 读取检定数据回复包
    /// </summary>
    internal class CL311_RequestReadVerifyDataReplayPacket : Cl311RecvPacket
    {
        public override string GetPacketName()
        {
            return "CL311_RequestReadVerifyDataReplayPacket";
        }

        //public override void ParseBody(ByteBuffer buf)
        //{
        //    byte cmd = buf.Get();


        //}
    }
    /// <summary>
    /// 请求读取标准表版本号请求包
    /// </summary>
    internal class CL311_RequestReadVersionPacket : Cl311SendPacket
    {
        public override string GetPacketName()
        {
            return "CL311_RequestReadVersionPacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x20);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 读取标准表版本号回复包
    /// </summary>
    internal class CL311_RequestReadVersionReplayPacket : Cl311RecvPacket
    {

        public string Version = "UnKnown";

        public CL311_RequestReadVersionReplayPacket() : base() { }
        //public CL311_RequestReadVersionReplayPacket(ByteBuffer buf) : base(buf) { }
        //public CL311_RequestReadVersionReplayPacket(ByteBuffer buf, int length) : base(buf, length) { }

        public override string GetPacketName()
        {
            return "CL311_RequestReadVersionReplayPacket";
        }
        //public override void ParseBody(ByteBuffer buf)
        //{
        //    byte bytTmp = buf.Get();
        //    if (bytTmp == 32)
        //    {
        //        Version = ASCIIEncoding.UTF8.GetString(buf.GetByteArray(19));
        //    }
        //}
    }
    /// <summary>
    /// 设置标准表参数请求包
    /// 返回Result包
    /// </summary>
    internal class CL311_RequestSetMeterParaPacket : Cl311SendPacket
    {
        private int m_MeterConst;
        private int m_PulseCount;
        private byte m_Lx;
        private byte m_Clfs;

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="meterconst">被检表常数</param>
        /// <param name="pulsecount">脉冲个数</param>
        /// <param name="lx"></param>
        /// <param name="clfs"></param>
        public void SetPara(int meterconst, int pulsecount, byte lx, byte clfs)
        {
            m_MeterConst = meterconst;
            m_PulseCount = pulsecount;
            m_Lx = lx;
            m_Clfs = clfs;
        }

        public override string GetPacketName()
        {
            return "CL311_RequestSetMeterParaPacket";
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x62);
            buf.Put(0x00);////62指令的类型 ，0=设置参数，1=启动，2=读数据
            buf.PutInt_S(m_MeterConst);
            buf.PutInt_S(m_PulseCount);
            buf.Put(m_Lx);
            buf.Put(m_Clfs);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 设置标准表常数
    /// </summary>
    internal class CL311_RequestSetStdMeterConstPacket : Cl311SendPacket
    {
        private byte m_auto = 0;
        private int m_meterconst = 0;

        public void SetPara(int meterconst, bool auto)
        {
            m_meterconst = meterconst;
            if (auto)
                m_auto = 0;
            else
                m_auto = 1;
        }
        public override string GetPacketName()
        {
            return "CL311_RequestSetStdMeterConstPacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x44);
            buf.Put(m_auto);
            byte[] bTmp = BitConverter.GetBytes(m_meterconst);
            //DataFormart.ByteReverse(bTmp);
            buf.Put(bTmp);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 启动标准表请求包
    /// </summary>
    internal class CL311_RequestSetStdMeterStartPacket : Cl311SendPacket
    {
        public override string GetPacketName()
        {
            return "CL311_RequestSetStdMeterStartPacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x62);
            buf.Put(0x01);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 设置电压/电流档位请求包
    /// </summary>
    internal class CL311_RequestSetUScalePacket : Cl311SendPacket
    {
        private float m_ua;
        private float m_ub;
        private float m_uc;
        /// <summary>
        /// 为TRUE时为设置电压
        /// </summary>
        public bool IsU = false;
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="ua">A相电压档位</param>
        /// <param name="ub">B相电压档位</param>
        /// <param name="uc">C相电压档位</param>
        public void SetPara(float a, float b, float c, bool needConvert)
        {
            if (needConvert)
            {
                if (IsU)
                {
                    m_ua = GetUScaleIndex(a);
                    m_ub = GetUScaleIndex(b);
                    m_uc = GetUScaleIndex(c);
                }
                else
                {
                    m_ua = GetIScaleIndex(a);
                    m_ub = GetIScaleIndex(b);
                    m_uc = GetIScaleIndex(c);

                }
            }
            else
            {
                m_ua = a;
                m_ub = b;
                m_uc = c;
            }
        }

        public override string GetPacketName()
        {
            return "CL311_RequestSetUScalePacket";
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x41);
            buf.Put((byte)m_ua);
            buf.Put((byte)m_ub);
            buf.Put((byte)m_uc);
            return buf.ToByteArray();
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
    }
    /// <summary>
    /// 请求启动标准表指令包
    /// 返回0x4b成功
    /// </summary>
    internal class Cl311_RequestStartTaskPacket : Cl311SendPacket
    {
        public Cl311_RequestStartTaskPacket()
            : base()
        {
            ToID = 0x16;
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x62);
            buf.Put(0x01);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal class CL311_ReplyOkPacket : Cl311RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {

            if (data.Length != 2)
            {
                this.ReciveResult = RecvResult.DataError;
            }
            else if (data[1] == 0x4B)
            {
                this.ReciveResult = RecvResult.OK;
            }
            else
            {
                this.ReciveResult = RecvResult.DataError;
            }
        }
    }
    /// <summary>
    /// 读取标准表信息返回包
    /// </summary>
    internal class CL311_RequestReadStdInfoReplayPacket : Cl311RecvPacket
    {
        public CL311_RequestReadStdInfoReplayPacket() : base() { }
        public override string GetPacketName()
        {
            return "CL311_RequestReadStdInfoReplayPacket";
        }
        /// <summary>
        /// 获取源信息
        /// </summary>
        /// <returns></returns>
        public stStdInfo PowerInfo { get; private set; }

        /// <summary>
        /// 3字节转换为Float
        /// </summary>
        /// <param name="bytData"></param>
        /// <param name="dotLen"></param>
        /// <returns></returns>
        private float get3ByteValue(byte[] bytData, int dotLen)
        {
            float data = 0F;

            data = bytData[2] << 16;
            data += bytData[1] << 8;
            data += bytData[0];

            // data = bytData[2]<<16 + bytData[1] << 8 + bytData[0];
            data = (float)(data / Math.Pow(10, dotLen));
            return data;
        }


        protected override void ParseBody(byte[] data)
        {

            stStdInfo tagInfo = new stStdInfo();
            //ByteBuffer buf = new ByteBuffer(data);
            //if (buf.Length != 0x62) return;
            //int[] arrDot = new int[9];

            ////去掉 命令字
            //buf.Get();

            //tagInfo.Clfs = (Cus_Clfs)buf.Get();
            //tagInfo.Flip_ABC = buf.Get();
            //tagInfo.Freq = buf.GetUShort_S() / 1000F;
            ////电压档位
            //tagInfo.Scale_Ua = buf.Get();
            //tagInfo.Scale_Ub = buf.Get();
            //tagInfo.Scale_Uc = buf.Get();
            ////电流档位
            //tagInfo.Scale_Ia = buf.Get();
            //tagInfo.Scale_Ib = buf.Get();
            //tagInfo.Scale_Ic = buf.Get();
            ////小数点
            //for (int i = 0; i < arrDot.Length; i++)
            //{
            //    arrDot[i] = buf.Get();
            //}
            //电压电流
            //tagInfo.Ua = get3ByteValue(buf.GetByteArray(3), arrDot[0]);
            //tagInfo.Ia = get3ByteValue(buf.GetByteArray(3), arrDot[3]);
            //tagInfo.Ub = get3ByteValue(buf.GetByteArray(3), arrDot[1]);
            //tagInfo.Ib = get3ByteValue(buf.GetByteArray(3), arrDot[4]);
            //tagInfo.Uc = get3ByteValue(buf.GetByteArray(3), arrDot[2]);
            //tagInfo.Ic = get3ByteValue(buf.GetByteArray(3), arrDot[5]);
            ////相位
            //tagInfo.Phi_Ua = get3ByteValue(buf.GetByteArray(3), 3);
            //tagInfo.Phi_Ib = get3ByteValue(buf.GetByteArray(3), 3);
            //tagInfo.Phi_Ub = get3ByteValue(buf.GetByteArray(3), 3);
            //tagInfo.Phi_Ib = get3ByteValue(buf.GetByteArray(3), 3);
            //tagInfo.Phi_Uc = get3ByteValue(buf.GetByteArray(3), 3);
            //tagInfo.Phi_Ic = get3ByteValue(buf.GetByteArray(3), 3);
            ///////////////////////
            //tagInfo.Pa = get3ByteValue(buf.GetByteArray(3), 5);
            //tagInfo.Pb = get3ByteValue(buf.GetByteArray(3), 5);
            //tagInfo.Pc = get3ByteValue(buf.GetByteArray(3), 5);

            //tagInfo.Qa = get3ByteValue(buf.GetByteArray(3), 5);
            //tagInfo.Qb = get3ByteValue(buf.GetByteArray(3), 5);
            //tagInfo.Qc = get3ByteValue(buf.GetByteArray(3), 5);
            //////
            ////tagInfo.Sa = get3ByteValue(buf.GetByteArray(3), 5);
            ////tagInfo.Sb = get3ByteValue(buf.GetByteArray(3), 5);
            ////tagInfo.Sc =  get3ByteValue(buf.GetByteArray(3), 5);
            ////tagInfo.P =  get3ByteValue(buf.GetByteArray(3), 5);
            ////tagInfo.Q = get3ByteValue(buf.GetByteArray(3), 5);
            ////tagInfo.S =  get3ByteValue(buf.GetByteArray(3), 5);
            ////tagInfo.COS =  get3ByteValue(buf.GetByteArray(3), 5);
            ////tagInfo.SIN =  get3ByteValue(buf.GetByteArray(3), 5);
            ////////////////////
            //tagInfo.Pa = get3ByteValue(buf.GetByteArray(3), arrDot[6]) - 16777216;
            //tagInfo.Pb = get3ByteValue(buf.GetByteArray(3), arrDot[7]) - 16777216;
            //tagInfo.Pc = get3ByteValue(buf.GetByteArray(3), arrDot[8]) - 16777216;

            //tagInfo.Qa = get3ByteValue(buf.GetByteArray(3), arrDot[6]) - 16777216;
            //tagInfo.Qb = get3ByteValue(buf.GetByteArray(3), arrDot[7]) - 16777216;
            //tagInfo.Qc = get3ByteValue(buf.GetByteArray(3), arrDot[8]) - 16777216;
            ////
            //tagInfo.Sa = get3ByteValue(buf.GetByteArray(3), arrDot[6]);
            //tagInfo.Sb = get3ByteValue(buf.GetByteArray(3), arrDot[7]);
            //tagInfo.Sc = get3ByteValue(buf.GetByteArray(3), arrDot[8]);

            //tagInfo.P = get3ByteValue(buf.GetByteArray(3), arrDot[6]);
            //tagInfo.Q = get3ByteValue(buf.GetByteArray(3), arrDot[6]);
            //tagInfo.S = get3ByteValue(buf.GetByteArray(3), arrDot[6]);
            //tagInfo.COS = get3ByteValue(buf.GetByteArray(3), 5);
            //tagInfo.SIN = get3ByteValue(buf.GetByteArray(3), 5);




            string[] str_Value = new string[25];    //35个数据
            byte[] byt_Tmp = new byte[] { 0, 0, 0, 0 };
            bool isz = true;
            int IntData=0;


            tagInfo.Clfs = (Cus_Clfs)(data[1]);    //测量方式
            tagInfo.Flip_ABC = data[2];  //相位开关,二进制表示




            Array.Copy(data, 3, byt_Tmp, 0, 2);          //频率,6、7
            tagInfo.Freq = Convert.ToSingle(BitConverter.ToInt32(byt_Tmp, 0) / 1000);  //频率

            int[] int_Dot = new int[9];         //小数点
            for (int int_Inc = 0; int_Inc < 9; int_Inc++)
            {
                int_Dot[int_Inc] = data[11 + int_Inc];
            }

            

            Array.Clear(byt_Tmp, 0, 4);
            Array.Copy(data, 20, byt_Tmp, 0, 3);
            tagInfo.Ua = Convert.ToSingle(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, int_Dot[0]));   //Ua

            Array.Copy(data, 23, byt_Tmp, 0, 3);
            tagInfo.Ia = Convert.ToSingle(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, int_Dot[3]));   //Ia

            Array.Copy(data, 26, byt_Tmp, 0, 3);
            tagInfo.Ub = Convert.ToSingle(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, int_Dot[1]));   //Ub

            Array.Copy(data, 29, byt_Tmp, 0, 3);
            tagInfo.Ib = Convert.ToSingle(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, int_Dot[4]));   //Ib

            Array.Copy(data, 32, byt_Tmp, 0, 3);
            tagInfo.Uc = Convert.ToSingle(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, int_Dot[2]));   //Uc

            Array.Copy(data, 35, byt_Tmp, 0, 3);
            tagInfo.Ic = Convert.ToSingle(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, int_Dot[5]));   //Ic

            Array.Copy(data, 38, byt_Tmp, 0, 3);
            tagInfo.Phi_Ua = Convert.ToSingle(BitConverter.ToInt32(byt_Tmp, 0) / 1000);   //Phi_Ua

            Array.Copy(data, 41, byt_Tmp, 0, 3);
            tagInfo.Phi_Ia = Convert.ToSingle(BitConverter.ToInt32(byt_Tmp, 0) / 1000);   //Phi_Ia

            Array.Copy(data, 44, byt_Tmp, 0, 3);
            tagInfo.Phi_Ub = Convert.ToSingle(BitConverter.ToInt32(byt_Tmp, 0) / 1000);   //Phi_Ub

            Array.Copy(data, 47, byt_Tmp, 0, 3);
            tagInfo.Phi_Ib = Convert.ToSingle(BitConverter.ToInt32(byt_Tmp, 0) / 1000);   //Phi_Ib

            Array.Copy(data, 50, byt_Tmp, 0, 3);
            tagInfo.Phi_Uc = Convert.ToSingle(BitConverter.ToInt32(byt_Tmp, 0) / 1000);   //Phi_Uc

            Array.Copy(data, 53, byt_Tmp, 0, 3);
            tagInfo.Phi_Ic = Convert.ToSingle(BitConverter.ToInt32(byt_Tmp, 0) / 1000);   //Phi_Ic

            Array.Copy(data, 56, byt_Tmp, 0, 3);
            int isz1 = Convert.ToInt32(data[58]) & Convert.ToInt32(0x80);
            if (isz1 > 0) { IntData = -16777216; } else { IntData = 0; }         
            tagInfo.Pa = Convert.ToSingle((BitConverter.ToInt32(byt_Tmp, 0)+IntData) / Math.Pow(10, int_Dot[6]));   //A P

            Array.Copy(data, 59, byt_Tmp, 0, 3);
            isz1 = Convert.ToInt32(data[61]) & Convert.ToInt32(0x80);
            if (isz1 > 0) { IntData = -16777216; } else { IntData = 0; }           
           
            tagInfo.Pb = Convert.ToSingle((BitConverter.ToInt32(byt_Tmp, 0)+IntData) / Math.Pow(10, int_Dot[7]));   //B P
               

            Array.Copy(data, 62, byt_Tmp, 0, 3);
            isz1 = Convert.ToInt32(data[64]) & Convert.ToInt32(0x80);
            if (isz1 > 0) { IntData = -16777216; } else { IntData = 0; }           
           
            tagInfo.Pc = Convert.ToSingle((BitConverter.ToInt32(byt_Tmp, 0)+IntData) / Math.Pow(10, int_Dot[8]));   //C P

            Array.Copy(data, 65, byt_Tmp, 0, 3);
            isz1 = Convert.ToInt32(data[67]) & Convert.ToInt32(0x80);
            if (isz1 > 0) { IntData = -16777216; } else { IntData = 0; } 
            tagInfo.Qa = Convert.ToSingle((BitConverter.ToInt32(byt_Tmp, 0)+IntData) / Math.Pow(10, int_Dot[6]));   //A Q

            Array.Copy(data, 68, byt_Tmp, 0, 3);
            isz1 = Convert.ToInt32(data[70]) & Convert.ToInt32(0x80);
            if (isz1 > 0) { IntData = -16777216; } else { IntData = 0; } 
            tagInfo.Qb = Convert.ToSingle((BitConverter.ToInt32(byt_Tmp, 0)+IntData) / Math.Pow(10, int_Dot[7]));   //B Q

            Array.Copy(data, 71, byt_Tmp, 0, 2);
            isz1 = Convert.ToInt32(data[73]) & Convert.ToInt32(0x80);
            if (isz1 > 0) { IntData = -16777216; } else { IntData = 0; }     
            tagInfo.Qc = Convert.ToSingle((BitConverter.ToInt32(byt_Tmp, 0)+IntData) / Math.Pow(10, int_Dot[8]));   //C Q

            Array.Copy(data, 74, byt_Tmp, 0, 3);
            isz1 = Convert.ToInt32(data[76]) & Convert.ToInt32(0x80);
            if (isz1 > 0) { IntData = -16777216; } else { IntData = 0; } 
            tagInfo.Sa = Convert.ToSingle((BitConverter.ToInt32(byt_Tmp, 0)+IntData) / Math.Pow(10, int_Dot[6]));   //A S

            Array.Copy(data, 77, byt_Tmp, 0, 3);
            isz1 = Convert.ToInt32(data[79]) & Convert.ToInt32(0x80);
            if (isz1 > 0) { IntData = -16777216; } else { IntData = 0; } 
            tagInfo.Sb = Convert.ToSingle(BitConverter.ToInt32(byt_Tmp, 0) / Math.Pow(10, int_Dot[7]));   //B S

            Array.Copy(data, 80, byt_Tmp, 0, 3);
            isz1 = Convert.ToInt32(data[82]) & Convert.ToInt32(0x80);
            if (isz1 > 0) { IntData = -16777216; } else { IntData = 0; } 
            tagInfo.Sc = Convert.ToSingle((BitConverter.ToInt32(byt_Tmp, 0)+IntData) / Math.Pow(10, int_Dot[8]));   //C S


            int int_MaxDot = Math.Max(int_Dot[6], Math.Max(int_Dot[7], int_Dot[8]));        //取出最大小数位

            Array.Copy(data, 83, byt_Tmp, 0, 3);
            isz1 = Convert.ToInt32(data[85]) & Convert.ToInt32(0x80);
            if (isz1 > 0) { IntData = -16777216; } else { IntData = 0; } 
            tagInfo.P = Convert.ToSingle((BitConverter.ToInt32(byt_Tmp, 0)+IntData) / Math.Pow(10, int_MaxDot));   //Sum P

            Array.Copy(data, 86, byt_Tmp, 0, 3);
            isz1 = Convert.ToInt32(data[88]) & Convert.ToInt32(0x80);
            if (isz1 > 0) { IntData = -16777216; } else { IntData = 0; } 
            tagInfo.Q = Convert.ToSingle((BitConverter.ToInt32(byt_Tmp, 0)+IntData) / Math.Pow(10, int_MaxDot));   //Sum Q

            Array.Copy(data, 89, byt_Tmp, 0, 3);
            isz1 = Convert.ToInt32(data[91]) & Convert.ToInt32(0x80);
            if (isz1 > 0) { IntData = -16777216; } else { IntData = 0; } 
            tagInfo.S = Convert.ToSingle((BitConverter.ToInt32(byt_Tmp, 0)+IntData )/ Math.Pow(10, int_MaxDot));   //Sum S
             

            PowerInfo = tagInfo;
        }

        /// <summary>
        /// 单个字节由低位向高位取值，
        /// </summary>
        /// <param name="input">单个字节</param>
        /// <param name="index">起始0,1,2..7</param>
        /// <returns></returns>
        protected int GetbitValue(byte input, int index)
        {
            int value;
            value = index > 0 ? input >> index : input;
            return value &= 1;
        }
    }

    /// <summary>
    /// 读取标准表参数信息
    /// </summary>
    internal class CL311_RequestReadStdParamPacket : Cl311SendPacket
    {
        public override string GetPacketName()
        {
            return "CL311_RequestReadStdParamPacket";
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x32);
            return buf.ToByteArray();
        }
    }


    #endregion


     

    public class DataFormart
    {
        /// <summary>
        /// 数组中的数据反转
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        public static void ByteReverse(byte[] value, int offset, int len)
        {
            for (int i = 0; i < len / 2; i++)
            {
                byte b = value[offset + i];
                value[offset + i] = value[offset + len - i - 1];
                value[offset + len - i - 1] = b;
            }
        }

        /// <summary>
        /// 数组中的数据反转
        /// </summary>
        /// <param name="value"></param>
        public static void ByteReverse(byte[] value)
        {
            ByteReverse(value, 0, value.Length);
        }

        /// <summary>
        /// 格式化 数据
        /// 返回 4个字节
        /// </summary>
        /// <param name="value">要格式化的数据</param>
        /// <param name="digit">小数位数</param>
        /// <returns></returns>
        public static byte[] Formart(double value, int digit, bool isLittleEndian)
        {
            value *= Math.Pow(10, digit);
            value = Math.Round(value, 0);
            uint uTmp = (uint)value;
            return Formart(uTmp, isLittleEndian);
        }

        public static byte[] Formart(uint value, bool isLittleEndian)
        {
            byte[] bTmp = BitConverter.GetBytes(value);

            if (isLittleEndian == false)
            {
                ByteReverse(bTmp);
            }
            return bTmp;
        }

        /// <summary>
        /// 格式化数据
        /// 返回 两个字节
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static byte[] Formart(ushort value, bool isLittleEndian)
        {
            byte[] b = BitConverter.GetBytes(value);

            if (isLittleEndian == false)
            {
                ByteReverse(b);
            }

            return b;
        }

        /// <summary>
        /// 格式化 一个整形数字
        /// 例如 int i=12345;
        /// 返回
        /// byte{49,50,51,52,53}
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isReverse">是否反顺序</param>
        /// <returns></returns>
        public static byte[] FormartToASCIIByte(int value, bool isReverse)
        {
            byte[] buf = Encoding.ASCII.GetBytes(value.ToString());

            if (isReverse == true)
            {
                ByteReverse(buf);
            }
            return buf;
        }

        /// <summary>
        /// 格式化 一个整形数字
        /// 例如 int i=12345;
        /// 返回
        /// byte{49,50,51,52,53}
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] FormartToASCIIByte(int value)
        {
            return FormartToASCIIByte(value, false);
        }

        /// <summary>
        /// 将字符串 
        /// 转换成 float 数组
        /// </summary>
        /// <param name="values"></param>
        /// <param name="spilter">分割符</param>
        /// <returns></returns>
        public static float[] ParseStringToFloat(string values, char spilter)
        {
            string[] strs = values.Split(new char[] { spilter }, StringSplitOptions.RemoveEmptyEntries);
            float[] va = new float[strs.Length];

            for (int i = 0; i < strs.Length; i++)
            {
                va[i] = float.Parse(strs[i]);
            }

            return va;
        }

        /// <summary>
        /// 将字符串 
        /// 转换成 float 数组 默认使用 空格
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float[] ParseStringToFloat(string value)
        {
            return ParseStringToFloat(value, ' ');
        }

        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }

        /// <summary>
        /// 十六进制字符串转十进制
        /// </summary>
        /// <param name="strHex"></param>
        /// <returns></returns>
        public static int HexStrToBin(string strHex)
        {
            return Convert.ToInt32(strHex, 16);
        }
    }
    /// <summary>
    /// 台体 工作流状态
    /// </summary>
    public enum WorkFlow
    {
        None,
        Unknow,
        预热,
        启动,
        潜动,
        对色标,
        基本误差,
        走字,
        需量周期误差,
        多功能
    }
}
