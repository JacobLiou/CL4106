using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.KLDevice
{
    #region CL188L误差板数据包基类
    /// <summary>
    /// CL188L误差板数据包基类
    /// </summary>
    internal class CL188LSendPacket : ClouSendPacket_CLT11
    {
        /// <summary>
        /// 广播标识
        /// </summary>
        public byte AllFlag = 0xFF;

        /// <summary>
        /// 表位数
        /// </summary>
        public int BwNum = 0;

        /// <summary>
        /// 误差版通道
        /// </summary>
        public byte[] ChannelByte;

        /// <summary>
        /// 启动/停止当前功能
        /// </summary>
        public bool isStart = true;

        /// <summary>
        /// 表位状态
        /// </summary>
        private bool[] bwStatus;
        public bool[] BwStatus
        {
            get
            {
                return bwStatus;
            }
            set
            {
                bwStatus = value;
                BwNum = bwStatus.Length;
                ChannelByte = ConvertBwStatusToChannelByte(bwStatus, isStart);
            }
        }
        /// <summary>
        /// 通道数
        /// </summary>
        private int iChannelNum;
        /// <summary>
        /// 通道表位数
        /// </summary>
        public int ChannelNum
        {
            get
            {
                return iChannelNum;
            }
            set
            {
                iChannelNum = value;
            }
        }
        /// <summary>
        /// 当前通讯通道
        /// </summary>
        private int iChannelNo;
        /// <summary>
        /// 通讯通道
        /// </summary>
        public int ChannelNo
        {
            get
            {
                return iChannelNo;
            }
            set
            {
                iChannelNo = value;
            }
        }
        public CL188LSendPacket()
            : base()
        {
            ToID = 0x40;
            MyID = 0x05;
        }
        public CL188LSendPacket(bool bReplay)
            : base(bReplay)
        {
            ToID = 0x40;
            MyID = 0x05;
        }

        public int Pos { get; set; }

        protected override byte[] GetBody()
        {
            return new byte[0];
        }

        /// <summary>
        /// 启动停止功能
        /// </summary>
        /// <param name="bwstatus">表位状态</param>
        /// <param name="start">true为启动,false为停止</param>
        /// <returns></returns>
        private byte[] ConvertBwStatusToChannelByte(bool[] bwstatus, bool start)
        {
            byte[] ChannelByte = new byte[12];
            for (int i = 0; i < ChannelByte.Length; i++)
            {
                string hex2 = "";
                for (int j = (i + 1) * 8 - 1; j >= i * 8; j--)
                {
                    if (Pos == 0)
                    {
                        int tmp = iChannelNo * (BwNum / iChannelNum);
                        if (j < tmp + BwNum / iChannelNum && j >= tmp)
                            hex2 += bwstatus[j] ? (start ? "1" : "0") : "0";
                        else
                            hex2 += "0";
                    }
                    else
                    {
                        if (j == Pos - 1)
                            hex2 += "1";
                        else
                            hex2 += "0";
                    }
                }
                ChannelByte[ChannelByte.Length - 1 - i] = Str2ToByte(hex2);
            }

            return ChannelByte;
        }
    }
    /// <summary>
    /// CL188L接收数据包基类
    /// </summary>
    internal class CL188LRecvPacket : ClouRecvPacket_CLT11
    {
        public byte Pos { get; set; }

        protected override void ParseBody(byte[] data)
        {
        }
    }
    #endregion

    #region CL191B时基源
    /// <summary>
    /// 时基源发送包基类
    /// </summary>
    internal class CL191BSendPacket : ClouSendPacket_CLT11
    {
        public CL191BSendPacket()
            : base()
        {
            ToID = 0x01;
            MyID = 0x07;
        }

        public CL191BSendPacket(bool needReplay)
            : base(needReplay)
        {
            ToID = 0x01;
            MyID = 0x07;
        }

        protected override byte[] GetBody()
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 191B 时基源接收基类
    /// </summary>
    internal class CL191BRecvPacket : ClouRecvPacket_CLT11
    {
        protected override void ParseBody(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region CL2029B
    /// <summary>
    /// 2029B发送包基类
    /// </summary>
    internal class CL2029BSendPacket : ClouSendPacket_CLT11
    {
        public CL2029BSendPacket()
            : base()
        {
            ToID = 0x42;
            MyID = 0x01;
        }

        public CL2029BSendPacket(bool needReplay)
            : base(needReplay)
        {
            ToID = 0x42;
            MyID = 0x01;
        }

        protected override byte[] GetBody()
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 2029B 接收基类
    /// </summary>
    internal class CL2029BRecvPacket : ClouRecvPacket_CLT11
    {
        protected override void ParseBody(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region CL2029D

    /// <summary>
    /// 2029D 多功能控制板接收基类
    /// </summary>
    internal class CL2029DRecvPacket : ClouRecvPacket_CLT11
    {
        protected override void ParseBody(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 2029D多功能控制板发送包基类
    /// </summary>
    internal class CL2029DSendPacket : ClouSendPacket_CLT11
    {
        public CL2029DSendPacket()
            : base()
        {
            ToID = 0x22;
            MyID = 0x01;
        }

        public CL2029DSendPacket(bool needReplay)
            : base(needReplay)
        {
            ToID = 0x22;
            MyID = 0x01;
        }

        protected override byte[] GetBody()
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region CL2030 CT
    /// <summary>
    /// 2030 CT档位控制器 接收基类
    /// </summary>
    internal class CL2030RecvPacket : ClouRecvPacket_CLT11
    {
        protected override void ParseBody(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 2030 CT档位控制器发送包基类
    /// </summary>
    internal class CL2030SendPacket : ClouSendPacket_CLT11
    {
        public CL2030SendPacket()
            : base()
        {
            ToID = 0x30;
            MyID = 0x01;
        }

        public CL2030SendPacket(bool needReplay)
            : base(needReplay)
        {
            ToID = 0x30;
            MyID = 0x01;
        }

        protected override byte[] GetBody()
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region CL2031B
    /// <summary>
    /// 2031B 功耗板 接收基类
    /// </summary>
    internal class CL2031BRecvPacket : ClouRecvPacket_CLT11
    {
        protected override void ParseBody(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 2031B 功耗板发送包基类
    /// </summary>
    internal class CL2031BSendPacket : ClouSendPacket_CLT11
    {
        public CL2031BSendPacket()
            : base()
        {
            ToID = 0x30;
            MyID = 0x01;
        }

        public CL2031BSendPacket(bool needReplay)
            : base(needReplay)
        {
            ToID = 0x30;
            MyID = 0x01;
        }

        protected override byte[] GetBody()
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region CL2038A
    /// <summary>
    /// 2038A 耐压仪控制器 接收基类
    /// </summary>
    internal class CL2038ARecvPacket : ClouRecvPacket_CLT11
    {
        protected override void ParseBody(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 2038A 耐压仪控制器发送包基类
    /// </summary>
    internal class CL2038ASendPacket : ClouSendPacket_CLT11
    {
        public CL2038ASendPacket()
            : base()
        {
            ToID = 0x01;
            MyID = 0x05;
        }

        public CL2038ASendPacket(bool needReplay)
            : base(needReplay)
        {
            ToID = 0x01;
            MyID = 0x05;
        }

        protected override byte[] GetBody()
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region CL309
    /// <summary>
    /// 309 功率源接收基类
    /// </summary>
    internal class CL309RecvPacket : ClouRecvPacket_CLT11
    {
        protected override void ParseBody(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 309功率源发送包基类
    /// </summary>
    internal class CL309SendPacket : ClouSendPacket_CLT11
    {
        public CL309SendPacket()
            : base()
        {
            ToID = 0x01;
            MyID = 0x07;
        }

        public CL309SendPacket(bool needReplay)
            : base(needReplay)
        {
            ToID = 0x01;
            MyID = 0x07;
        }

        protected override byte[] GetBody()
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region CL3115
    /// <summary>
    /// 3115 标准表接收基类
    /// </summary>
    internal class CL3115RecvPacket : ClouRecvPacket_CLT11
    {
        protected override void ParseBody(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 标准表发送包基类
    /// </summary>
    internal class CL3115SendPacket : ClouSendPacket_CLT11
    {
        public CL3115SendPacket()
            : base()
        {
            ToID = 0x30;
            MyID = 0x05;
        }

        public CL3115SendPacket(bool needReplay)
            : base(needReplay)
        {
            ToID = 0x30;
            MyID = 0x05;
        }

        protected override byte[] GetBody()
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region CLElectromotor
    /// <summary>
    /// 翻转电机接收数据包基类
    /// </summary>
    internal class CLElectromotorRecvPacket : ClouRecvPacket_CLT11
    {
        public byte Pos { get; set; }

        protected override void ParseBody(byte[] data)
        {
        }
    }
    /// <summary>
    /// 翻转电机发送数据包基类
    /// </summary>
    internal class CLElectromotorSendPacket : ClouSendPacket_CLT11
    {
        /// <summary>
        /// 广播标识
        /// </summary>
        public byte AllFlag = 0xFF;

        /// <summary>
        /// 表位数
        /// </summary>
        public int BwNum = 0;

        /// <summary>
        /// 误差版通道
        /// </summary>
        public byte[] ChannelByte;

        /// <summary>
        /// 启动/停止当前功能
        /// </summary>
        public bool isStart = true;

        /// <summary>
        /// 表位状态
        /// </summary>
        private bool[] bwStatus;
        public bool[] BwStatus
        {
            get
            {
                return bwStatus;
            }
            set
            {
                bwStatus = value;
                BwNum = bwStatus.Length;
                ChannelByte = ConvertBwStatusToChannelByte(bwStatus, isStart);
            }
        }
        /// <summary>
        /// 通道数
        /// </summary>
        private int iChannelNum;
        /// <summary>
        /// 通道表位数
        /// </summary>
        public int ChannelNum
        {
            get
            {
                return iChannelNum;
            }
            set
            {
                iChannelNum = value;
            }
        }
        /// <summary>
        /// 当前通讯通道
        /// </summary>
        private int iChannelNo;
        /// <summary>
        /// 通讯通道
        /// </summary>
        public int ChannelNo
        {
            get
            {
                return iChannelNo;
            }
            set
            {
                iChannelNo = value;
            }
        }
        public CLElectromotorSendPacket()
            : base()
        {
            ToID = 0x40;
            MyID = 0x05;
        }
        public CLElectromotorSendPacket(bool bReplay)
            : base(bReplay)
        {
            ToID = 0x40;
            MyID = 0x05;
        }

        public int Pos { get; set; }

        protected override byte[] GetBody()
        {
            return new byte[0];
        }

        /// <summary>
        /// 启动停止功能
        /// </summary>
        /// <param name="bwstatus">表位状态</param>
        /// <param name="start">true为启动,false为停止</param>
        /// <returns></returns>
        private byte[] ConvertBwStatusToChannelByte(bool[] bwstatus, bool start)
        {
            byte[] ChannelByte = new byte[12];
            for (int i = 0; i < ChannelByte.Length; i++)
            {
                string hex2 = "";
                for (int j = (i + 1) * 8 - 1; j >= i * 8; j--)
                {
                    if (Pos == 0)
                    {
                        int tmp = iChannelNo * (BwNum / iChannelNum);
                        if (j < tmp + BwNum / iChannelNum && j >= tmp)
                            hex2 += bwstatus[j] ? (start ? "1" : "0") : "0";
                        else
                            hex2 += "0";
                    }
                    else
                    {
                        if (j == Pos - 1)
                            hex2 += "1";
                        else
                            hex2 += "0";
                    }
                }
                ChannelByte[ChannelByte.Length - 1 - i] = Str2ToByte(hex2);
            }

            return ChannelByte;
        }    
    }
    #endregion


    #region CL188E误差板数据包基类
    /// <summary>
    /// CL188E误差板数据包基类
    /// </summary>
    internal class CL188ESendPacket : ClouSendPacket_CLT11
    {
        public CL188ESendPacket()
            : base()
        {
            ToID = 0x10;
            MyID = 0x20;
        }
        public CL188ESendPacket(bool bReplay)
            : base(bReplay)
        {
            ToID = 0x10;
            MyID = 0x20;
        }

        public byte Pos { get; set; }

        protected override byte[] GetBody()
        {
            return new byte[0];
        }
    }
    /// <summary>
    /// CL188E接收数据包基类
    /// </summary>
    internal class CL188ERecvPacket : ClouRecvPacket_CLT11
    {
        public byte Pos { get; set; }

        protected override void ParseBody(byte[] data)
        {
        }
    }
    #endregion

    #region  303
    /// <summary>
    /// 303 控源设备 接收包基类
    /// </summary>
    abstract class CL303RecvPacket : ClouRecvPacket_NotCltTwo
    {
    }
    /// <summary>
    /// 
    /// </summary>
    abstract class CL303SendPacket : ClouSendPacket_NotCltTwo
    {
        public CL303SendPacket()
            : base(true, 0x20)
        {

        }
    }
    #endregion

    #region 311
    /// <summary>
    /// 311 标准表接收基类
    /// </summary>
    internal class Cl311RecvPacket : ClouRecvPacket_NotCltOne
    {
        protected override void ParseBody(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 标准表发送包基类
    /// </summary>
    internal class Cl311SendPacket : ClouSendPacket_NotCltOne
    {
        public Cl311SendPacket()
            : base(true, 0x16)
        {
        }

        protected override byte[] GetBody()
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    /// <summary>
    /// 结论返回
    /// 0x4b:成功
    /// </summary>
    internal class CLNormalRequestResultReplayPacket : ClouRecvPacket_NotCltTwo
    {
        public CLNormalRequestResultReplayPacket()
            : base()
        {
        }
        /// <summary>
        /// 结论
        /// </summary>
        public virtual ReplayCode ReplayResult
        {
            get;
            private set;
        }

        public override string GetPacketName()
        {
            return "CLNormalRequestResultReplayPacket";
        }
        protected override void ParseBody(byte[] data)
        {
            if (data.Length == 2)
                ReplayResult = (ReplayCode)data[1];
            else
                ReplayResult = (ReplayCode)data[0];
        }

        public enum ReplayCode
        {
            /// <summary>
            /// CLT11返回
            /// </summary>
            CLT11OK = 0x30,
            /// <summary>
            /// 响应命令，表示“OK”
            /// </summary>
            Ok = 0x4b,
            /// <summary>
            /// 响应命令，表示出错
            /// </summary>
            Error = 0x33,
            /// <summary>
            /// 响应命令，表示系统估计还要忙若干mS
            /// </summary>
            Busy = 0x35,
            /// <summary>
            /// 误差板联机成功
            /// </summary>
            CL188LinkOk = 0x36,
            /// <summary>
            /// 标准表脱机成功
            /// </summary>
            Cl311UnLinkOk = 0x37
        }
    }

}
