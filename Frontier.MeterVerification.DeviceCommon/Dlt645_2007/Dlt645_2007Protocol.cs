using System;
using System.Collections.Generic;

using System.Text;
using VerificationEquipment.Commons;

namespace Frontier.MeterVerification.DeviceCommon
{

    //帧起始符	68H
    //地址域	A0
    //    A1
    //    A2
    //    A3
    //    A4
    //    A5
    //帧起始符	68H
    //控制码	C
    //数据域长度	L
    //数据域	DATA
    //校验码	CS
    //结束符	16H
    /// <summary>
    /// 电能表645_2007规约
    /// </summary>
    public class Dlt645_2007Protocol
    {
        private byte _wakening = 0xfe;              //唤醒字符
        private byte _head = 0x68;                  //报文头
        private byte[] _address = new byte[6];       //表地址
        private byte _cmd = (byte)Dlt645_2007ControlCode.读数据;       //控制指令，默认为读取数据
        private List<byte> _data = new List<byte>();       //数据
        private byte _checkSum = Byte.MinValue;                   //校验码(异或和)
        private byte _end = 0x16;

        /// <summary>
        /// 设备地址
        /// </summary>
        public string Address
        {
            set
            {
                string address = value;
                if (string.IsNullOrEmpty(value))
                {
                    address = "AAAAAAAAAAAA";
                }
                for (int i = 0; i < 6; i++)
                {
                    _address[5 - i] = (byte)Convert.ToInt64(address.Substring(i * 2, 2), 16);
                }
            }
        }

        /// <summary>
        /// 指令
        /// </summary>
        public byte Cmd
        {
            set
            {
                this._cmd = value;
            }
        }

        /// <summary>
        /// 数据
        /// </summary>
        public List<byte> Data
        {
            set
            {
                for (int i = 0; i < value.Count; i++)
                {
                    this._data.Add((byte)(value[i] + 0x33));
                }
            }
        }

        /// <summary>
        /// 取得报文
        /// </summary>
        /// <returns></returns>
        public List<byte> GetProtocol()
        {
            if (_data == null || _data.Count < 0 || _cmd == Byte.MinValue)
            {
                throw new PortException("报文格式存在问题");
            }

            List<byte> protocol = new List<byte>();      //完整报文
            for (int i = 0; i < 4; i++)
            {
                protocol.Add(this._wakening);
            }
            protocol.Add(this._head);
            protocol.AddRange(this._address);
            protocol.Add(this._head);
            protocol.Add(this._cmd);
            protocol.Add((byte)this._data.Count);
            protocol.AddRange(this._data);

            _checkSum = CheckHelper.CheckSum(protocol, 4, protocol.Count - 4);
            protocol.Add(this._checkSum);
            protocol.Add(this._end);
            return protocol;
        }

        /// <summary>
        /// 97协议组织帧
        /// </summary>
        /// <param name="cCmd">控制码</param>
        /// <param name="cAddr">表地址</param>
        /// <param name="bLen">长度</param>
        /// <param name="cData">数据</param>
        /// <returns>返回组好的帧</returns>
        /// 
        public List<byte> OrgFrame(byte cCmd, string cAddr, byte bLen, string cData)
        {	/// cCmd 命令字  cAddr 地址 bLen 数据长度 cData 数据域数据 返回组好的帧
            byte[] bSend;
            int iTn;
            string cStr = "";

            bSend = new byte[12 + bLen];

            bSend[0] = 0x68;
            /// 地址域
            cAddr = cAddr.PadLeft(12, '0');
            for (iTn = 0; iTn <= 5; iTn++)
            {
                cStr = "0x" + cAddr.Substring(2 * (5 - iTn), 2);
                bSend[iTn + 1] = System.Convert.ToByte(cStr, 16);
            }
            bSend[7] = 0x68;
            bSend[8] = cCmd; //System.Convert.ToByte("0x" + cCmd, 16);
            bSend[9] = bLen;
            /// 数据域
            cData = cData.PadLeft(2 * bLen, '0');
            if (bLen > 0)
            {
                for (iTn = 1; iTn <= bLen; iTn++)
                {
                    cStr = cData.Substring(2 * (bLen - iTn), 2);
                    bSend[9 + iTn] = System.Convert.ToByte("0x" + cStr, 16);
                    bSend[9 + iTn] += 0x33;
                }
            }
            /// 校验码
            for (iTn = 0; iTn <= 9 + bLen; iTn++)
            {
                bSend[10 + bLen] += bSend[iTn];
            }
            /// 结束码
            bSend[11 + bLen] = 0x16;

             List<byte> protocol = new List<byte>();

             for (int i = 0; i < bSend.Length; i++)
             {
                 protocol.Add(bSend[i]);
             }
             return protocol;
        }


    }
}
