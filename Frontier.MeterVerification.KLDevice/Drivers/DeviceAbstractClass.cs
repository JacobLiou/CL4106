using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.KLDevice
{
    /// <summary>
    /// 数据包基类，提供最基本的数据包操作
    /// </summary>
    public abstract class Packet
    {
        /// <summary>
        /// 默认实现 ，返回类命名空间名称
        /// </summary>
        /// <returns></returns>
        public virtual string GetPacketName()
        {
            return this.GetType().FullName;
        }
    }
    /// <summary>
    /// 接收数据包基类
    /// </summary>
    public abstract class RecvPacket : Packet
    {
        /// <summary>
        /// 侦解析结果
        /// </summary>
        public RecvResult ReciveResult
        {
            get;
            set;
        }

        /// <summary>
        /// 解析一个接收数据包。
        /// </summary>
        /// <param name="buf">接收到的缓冲区数据</param>
        /// <returns>解析是否成功，如果失败，当前交互无效</returns>
        public abstract bool ParsePacket(byte[] buf);
    }
    /// <summary>
    /// 初始化2018数据包
    /// </summary>
    public class RequestInit2018PortPacket : SendPacket
    {
        private string m_strSetting = "";
        public RequestInit2018PortPacket(string strSetting)
        {
            m_strSetting = strSetting;
        }

        public override byte[] GetPacketData()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            string str_Data = "init " + m_strSetting.Replace(',', ' ');
            byte[] byt_Data = ASCIIEncoding.ASCII.GetBytes(str_Data);
            buf.Put(byt_Data);
            return buf.ToByteArray();
        }

    }
    /// <summary>
    /// 发送数据包基类
    /// </summary>
    public abstract class SendPacket : Packet
    {
        /// <summary>
        /// 是否需要回复
        /// </summary>
        public bool IsNeedReturn { get; set; }

        /// <summary>
        /// 获取数据包二进制内容
        /// </summary>
        /// <returns></returns>
        public abstract byte[] GetPacketData();
    }

    /// <summary>
    /// 科陆CLT11协议接收数据包基类
    /// </summary>
    internal abstract class ClouRecvPacket_CLT11 : RecvPacket
    {
        /// <summary>
        /// 包头
        /// </summary>
        protected byte PacketHead = 0x81;
        /// <summary>
        /// 发信节点
        /// </summary>
        protected byte MyID = 0x80;
        /// <summary>
        /// 受信节点
        /// </summary>
        protected byte ToID = 0x10;
        /// <summary>
        /// 解析数据包
        /// </summary>
        /// <param name="buf">缓冲区接收到的数据包内容</param>
        /// <returns>解析是否成功</returns>
        public override bool ParsePacket(byte[] buf)
        {
            //第一步，验证包长度
            //第二步，验证包结构
            //第三步，拆帧
            ByteBuffer pack = new ByteBuffer(buf);
            int iLocalSum = 1;
            PacketHead = pack.Get();
            ToID = pack.Get();
            MyID = pack.Get();
            byte dataLength = pack.Get();
            if (buf.Length < dataLength) return false;
            byte[] data = pack.GetByteArray(dataLength - 5);
            byte chkCode = pack.Get();

            while (buf[dataLength - iLocalSum] == 0)
            {
                iLocalSum++;
            }
            //计算校验码
            byte chkSum = GetChkSum(buf, 1, dataLength - iLocalSum);
            //zzg soinlove@126.com
            //if (chkCode != chkSum) return false;
            ParseBody(data);
            return true;
        }
        /// <summary>
        /// 计算检验码[帧头不进入检验范围]
        /// </summary>
        /// <param name="bytData"></param>
        /// <returns></returns>
        protected byte GetChkSum(byte[] bytData, int startPos, int length)
        {
            byte bytChkSum = 0;
            for (int int_Inc = startPos; int_Inc < length; int_Inc++)
            {
                bytChkSum ^= bytData[int_Inc];
            }
            return bytChkSum;
        }
        /// <summary>
        /// 解析数据域
        /// </summary>
        /// <param name="data">数据域</param>
        protected abstract void ParseBody(byte[] data);


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

        /// <summary>
        /// 3字节转换为Float
        /// </summary>
        /// <param name="bytData"></param>
        /// <param name="dotLen"></param>
        /// <returns></returns>
        protected float get3ByteValue(byte[] bytData, int dotLen)
        {
            float data = 0F;

            data = bytData[0] << 16;
            data += bytData[1] << 8;
            data += bytData[2];

            data = (float)(data / Math.Pow(10, dotLen));
            return data;
        }

        ///<summary>
        /// 替换byteSource目标位的值
        ///</summary>
        ///<param name="byteSource">源字节</param>
        ///<param name="location">替换位置(0-7)</param>
        ///<param name="value">替换的值(1-true,0-false)</param>
        ///<returns>替换后的值</returns>
        protected byte ReplaceTargetBit(byte byteSource, short location, bool value)
        {
            Byte baseNum = (byte)(Math.Pow(2, location + 1) / 2);
            return ReplaceTargetBit(location, value, byteSource, baseNum);
        }

        ///<summary>
        /// 替换byteSource目标位的值
        ///</summary>
        ///<param name="location"></param>
        ///<param name="value">替换的值(1-true,0-false)</param>
        ///<param name="byteSource"></param>
        ///<param name="baseNum">与 基数(1,2,4,8,16,32,64,128)</param>
        ///<returns></returns>
        private byte ReplaceTargetBit(short location, bool value, byte byteSource, byte baseNum)
        {
            bool locationValue = GetbitValue(byteSource, location) == 1 ? true : false;
            if (locationValue != value)
            {
                return (byte)(value ? byteSource + baseNum : byteSource - baseNum);
            }
            return byteSource;
        }
    }

    /// <summary>
    /// 科陆CLT1.1协议发送包基类
    /// </summary>
    internal abstract class ClouSendPacket_CLT11 : SendPacket
    {

        /// <summary>
        /// 包头
        /// </summary>
        protected byte PacketHead = 0x81;
        /// <summary>
        /// 发信节点
        /// </summary>
        protected byte MyID = 0x80;
        /// <summary>
        /// 受信节点
        /// </summary>
        protected byte ToID = 0x10;

        public ClouSendPacket_CLT11() { IsNeedReturn = true; }
        public ClouSendPacket_CLT11(bool needReplay) { IsNeedReturn = needReplay; }

        /// <summary>
        /// 组帧
        /// </summary>
        /// <returns>完整的数据包内容</returns>
        public override byte[] GetPacketData()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(PacketHead);        //帧头0x81
            buf.Put(ToID);              //发信节点0x10
            buf.Put(MyID);              //受信节点0x80
            byte[] body = GetBody();
            if (body == null)
                return null;
            byte packetLength = (byte)(body.Length + 5);//帧头4字节+CS一字节
            buf.Put(packetLength);      //帧长度
            buf.Put(body);              //数据域 
            byte chkSum = GetChkSum(buf.ToByteArray());
            buf.Put(chkSum);
            return buf.ToByteArray();
        }

        protected abstract byte[] GetBody();


        /// <summary>
        /// 计算检验码[帧头不进入检验范围]
        /// </summary>
        /// <param name="bytData"></param>
        /// <returns></returns>
        protected byte GetChkSum(byte[] bytData)
        {
            byte bytChkSum = 0;
            for (int int_Inc = 1; int_Inc < bytData.Length; int_Inc++)
            {
                bytChkSum ^= bytData[int_Inc];
            }
            return bytChkSum;
        }

        /// <summary>
        /// 二进制字符串转换成16进制Hex
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte Str2ToByte(string str2)
        {
            int num = Convert.ToInt32(str2, 2);
            return Convert.ToByte(num);
        }
    }
    /// <summary>
    /// 非DLT协议基类，
    /// 其数据长度为 1 字节
    /// </summary>
    internal abstract class ClouRecvPacket_NotCltOne : ClouRecvPacket_NotCtl
    {
        protected override byte FrameLengthByteCount
        {
            get { return 1; }
        }
    }
    /// <summary>
    /// 科际非 CLT协议结构基类，
    /// 使用数据长度 1 个字节
    /// 默认需要回复
    /// </summary>
    internal abstract class ClouSendPacket_NotCltOne : ClouSendPacket_NotClt
    {
        /// <summary>
        /// 已重写，
        /// 设置IsNeedReturn 为true,
        /// ToID 为0x00
        /// </summary>
        public ClouSendPacket_NotCltOne()
            : this(true, 0x00)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isNeedReturn"></param>
        public ClouSendPacket_NotCltOne(bool isNeedReturn)
            : this(isNeedReturn, 0x00)
        {
        }

        /// <summary>
        /// 已重写，
        /// ToID 为0x00
        /// </summary>
        /// <param name="isNeedReturn">该包是否需要返回</param>
        /// <param name="toID">目标ID</param>
        public ClouSendPacket_NotCltOne(bool isNeedReturn, byte toID)
        {
            this.IsNeedReturn = isNeedReturn;
            this.ToID = toID;
        }

        protected override byte FrameLengthByteCount
        {
            get { return 1; }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal abstract class ClouRecvPacket_NotCltTwo : ClouRecvPacket_NotCtl
    {
        protected override byte FrameLengthByteCount
        {
            get { return 2; }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal abstract class ClouSendPacket_NotCltTwo : ClouSendPacket_NotClt
    {
        /// <summary>
        /// 已重写，返回 2
        /// </summary>
        protected override byte FrameLengthByteCount
        {
            get { return 2; }
        }

        /// <summary>
        /// 已重写，
        /// 设置IsNeedReturn 为true,
        /// ToID 为0x00
        /// </summary>
        public ClouSendPacket_NotCltTwo()
            : this(true, 0x00)
        {
        }

        /// <summary>
        /// 已重写
        /// 设置 ToID为0
        /// </summary>
        /// <param name="isNeedReturn"></param>
        public ClouSendPacket_NotCltTwo(bool isNeedReturn)
            : this(isNeedReturn, 0x00)
        {
        }

        /// <summary>
        /// 已重写，
        /// ToID 为0x00
        /// </summary>
        /// <param name="isNeedReturn">该包是否需要返回</param>
        /// <param name="toID">目标ID</param>
        public ClouSendPacket_NotCltTwo(bool isNeedReturn, byte toID)
        {
            this.IsNeedReturn = isNeedReturn;
            this.ToID = toID;
        }
    }
    /// <summary>
    /// 非CLT1.1
    /// </summary>
    internal abstract class ClouRecvPacket_NotCtl : RecvPacket
    {
        /// <summary>
        /// 包头
        /// </summary>
        protected byte PacketHead = 0x81;

        /// <summary>
        /// 受信节点
        /// </summary>
        protected byte ToID = 0x10;

        /// <summary>
        /// 整个帧的长度
        /// </summary>
        public int Length
        {
            get;
            set;
        }

        /// <summary>
        /// 表示 帧长度用多少个字节来表示
        /// </summary>
        protected abstract byte FrameLengthByteCount
        {
            get;
        }

        public override bool ParsePacket(byte[] buf)
        {
            //第一步，验证包长度
            //第二步，验证包结构
            //第三步，拆帧
            ByteBuffer pack = new ByteBuffer(buf);
            PacketHead = pack.Get();
            ToID = pack.Get();
            //处理 帧长度 
            int frameLen = 0;
            if (FrameLengthByteCount == 1)
            {
                frameLen = pack.Get();
            }
            else if (FrameLengthByteCount == 2)
            {
                frameLen = pack.GetUShort();
            }
            else
            {
                throw new Exception("无法支持的数据");
            }

            Length = frameLen;
            if (buf.Length < frameLen)
            {
                this.ReciveResult = RecvResult.FrameError;
                return false;
            }
            int bytecount = frameLen - 3 - FrameLengthByteCount;
            if (bytecount < 0) return false;
            byte[] data = pack.GetByteArray(bytecount);
            byte chkCode = pack.Get();
            //计算校验码
            byte chkSum = ChkSum.GetChkSumXor(buf, 1, frameLen - 2);
            if (chkCode != chkSum)
            {
                this.ReciveResult = RecvResult.CSError;
                return false;
            }
            ParseBody(data);
            if (this.ReciveResult == RecvResult.None)
            {
                this.ReciveResult = RecvResult.OK;
            }
            return true;
        }

        /// <summary>
        /// 解析数据，可能包括返回的 命令
        /// </summary>
        /// <param name="data"></param>
        protected abstract void ParseBody(byte[] data);
    }
    /// <summary>
    /// 非CLT1.1协议，适用于CL303,CL311
    /// 与CLT协议区别为帧结构中没有发信节点号，只有收信节点号
    /// 默认需要回复
    /// </summary>
    internal abstract class ClouSendPacket_NotClt : SendPacket
    {
        /// <summary>
        /// 包头
        /// </summary>
        protected byte PacketHead = 0x81;

        /// <summary>
        /// 受信节点
        /// </summary>
        protected byte ToID = 0x10;

        /// <summary>
        /// 返回表示 帧长度的 字节数
        /// </summary>
        protected abstract byte FrameLengthByteCount
        {
            get;
        }

        /// <summary>
        /// 已重写
        /// </summary>
        /// <returns></returns>
        public override byte[] GetPacketData()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(PacketHead);        //帧头
            buf.Put(ToID);              //受信节点
            byte[] body = GetBody();
            if (FrameLengthByteCount == 1)
            {
                byte packetLen = (byte)(body.Length + 3 + FrameLengthByteCount);
                buf.Put(packetLen);
            }
            else if (FrameLengthByteCount == 2)
            {
                ushort packetLen = (ushort)(body.Length + 3 + FrameLengthByteCount);
                buf.PutUShort(packetLen);
            }
            else
            {
                throw new Exception("无法支持的数据");
            }
            buf.Put(body);              //数据域 
            byte chkSum = ChkSum.GetChkSumXor(buf.ToByteArray(), 1, buf.Length - 1);
            buf.Put(chkSum);
            return buf.ToByteArray();
        }

        protected abstract byte[] GetBody();

        /// <summary>
        /// 将数字转换成10字节数据
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        protected static byte[] get10bitData(Single Data)
        {
            //float fdata =float.Parse( string.Format("{0:0.0}", Data));
            string strdata = Data.ToString("0.0").PadRight(10, '0');
            if (strdata.Length > 10)
                strdata = strdata.Substring(0, 10);
            byte[] d = ASCIIEncoding.ASCII.GetBytes(strdata);
            d[9] = 0x2E;
            return d;
        }
    }
    /// <summary>
    /// 效验码计算
    /// </summary>
    class ChkSum
    {
        /// <summary>
        /// 计算异或效验码
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static byte GetChkSumXor(byte[] buff, int offset, int len)
        {
            byte chkSum = 0;

            for (int i = 0; i < len; i++)
            {
                chkSum ^= buff[i + offset];
            }

            return chkSum;
        }

        /// <summary>
        /// 计算异或效验码
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        public static byte GetChkSumXor(byte[] buff)
        {
            return GetChkSumXor(buff, 0, buff.Length);
        }
    }
}
