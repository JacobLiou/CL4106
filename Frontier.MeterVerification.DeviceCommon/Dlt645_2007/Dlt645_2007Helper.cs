using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VerificationEquipment.Commons;

namespace Frontier.MeterVerification.DeviceCommon
{
    /// <summary>
    /// Dlt645_2007规约合成与解析帮助类
    /// </summary>
    public class Dlt645_2007Helper
    {
        #region 组装报文
        /// <summary>
        /// 获取读取表地址的报文
        /// </summary>
        /// <returns></returns>
        public static List<byte> GetAddressProtocol()
        {
            Dlt645_2007Protocol protocol = new Dlt645_2007Protocol();
            protocol.Address = "AAAAAAAAAAAA";
            protocol.Cmd = (byte)Dlt645_2007ControlCode.读通信地址;
            protocol.Data = new List<byte>(0);
            return protocol.GetProtocol();
        }

        /// <summary>
        /// 获取读取生产编号报文
        /// </summary>
        /// <returns></returns>
        public static List<byte> GetScbhProtocol()
        {
            Dlt645_2007Protocol protocol = new Dlt645_2007Protocol();
            return protocol.OrgFrame(0x01,"AAAAAAAAAAAA",2,"FFF9");
        }

        /// <summary>
        /// 获取电能表当前 电压指令
        /// </summary>
        /// <param name="voltIndex">1代表A、2代表B、3代表C</param>
        /// <returns></returns>
        public static List<byte> GetVoltageProtocol(int voltIndex)
        {
            Dlt645_2007Protocol protocol = new Dlt645_2007Protocol();
            protocol.Address = "AAAAAAAAAAAA";
            protocol.Cmd = (byte)Dlt645_2007ControlCode.读数据;
            uint uvolt = new uint();
            if(voltIndex == 1)
            {
                uvolt = (uint)Dlt645_2007Cmd.当前A相电压;
            }
            else if(voltIndex == 2)
            {
                uvolt = (uint)Dlt645_2007Cmd.当前B相电压;
            }
            else if (voltIndex == 3)
            {
                uvolt = (uint)Dlt645_2007Cmd.当前C相电压;
            }

            protocol.Data = EncodeDataIndex(uvolt);
            return protocol.GetProtocol();
        }
        /// <summary>
        /// 合成发送的报文
        /// </summary>
        /// <param name="sendParam">输入胡数据标识</param>
        /// <returns></returns>
        public static List<byte> GetSendProtocol(uint sendParam)
        {
            Dlt645_2007Protocol protocol = new Dlt645_2007Protocol();
            protocol.Address = "AAAAAAAAAAAA";
            protocol.Cmd = (byte)Dlt645_2007ControlCode.读数据;

            protocol.Data = EncodeDataIndex(sendParam);
            return protocol.GetProtocol();
        }

        /// <summary>
        /// 写表地址
        /// </summary>
        /// <param name="address">表地址</param>
        /// <returns></returns>
        public static List<byte> SetAddressProtocol(string address)
        {
            Dlt645_2007Protocol protocol = new Dlt645_2007Protocol();
            protocol.Address = "AAAAAAAAAAAA";
            protocol.Cmd = (byte)Dlt645_2007ControlCode.写通信地址;
            protocol.Data = ResolveAddress(address);
            return protocol.GetProtocol();
        }

        /// <summary>
        /// 获取读取电能表日期的报文
        /// </summary>
        /// <param name="address">电能表表地址</param>
        /// <returns></returns>
        public static List<byte> GetDateProtocol(string address)
        {
            Dlt645_2007Protocol protocol = new Dlt645_2007Protocol();
            protocol.Address = address;
            protocol.Cmd = (byte)Dlt645_2007ControlCode.读数据;
            protocol.Data = EncodeDataIndex((uint)Dlt645_2007Cmd.日期);
            return protocol.GetProtocol();
        }

        /// <summary>
        /// 获取读取电能表时间的报文
        /// </summary>
        /// <param name="address">电能表表地址</param>
        /// <returns></returns>
        public static List<byte> GetTimeProtocol(string address)
        {
            Dlt645_2007Protocol protocol = new Dlt645_2007Protocol();
            protocol.Address = address;
            protocol.Cmd = (byte)Dlt645_2007ControlCode.读数据;
            protocol.Data = EncodeDataIndex((uint)Dlt645_2007Cmd.时间);
            return protocol.GetProtocol();
        }
        /// <summary>
        /// 合成设置电表高频检定报文
        /// </summary>
        /// <param name="address">表地址</param>
        /// <param name="password">密码</param>
        /// <param name="bBeilv">要设定的倍率</param>
        /// <returns></returns>
        public static List<byte> SetMeterBeilv(string address, string password,byte bBeilv)
        {
            Dlt645_2007Protocol protocol = new Dlt645_2007Protocol();
            protocol.Address = address;
            protocol.Cmd = (byte)Dlt645_2007ControlCode.写数据;
            List<byte> data = new List<byte>();

            data.AddRange(EncodeDataIndex((uint)Dlt645_2007Cmd.脉冲常数放大));
            data.AddRange(CheckHelper.TransDataToBytes(password, false));
            data.AddRange(CheckHelper.TransDataToBytes(true));//操作者代码，默认为00000000
            data.Add(bBeilv);
            protocol.Data = data;
            return protocol.GetProtocol();
        }

        /// <summary>
        /// 设置电能表日期
        /// </summary>
        /// <param name="address">表地址</param>
        /// <param name="password">密码</param>
        /// <param name="dateTime">设置的时间</param>
        /// <returns></returns>
        public static List<byte> SetDateProtocol(string address, string password, DateTime dateTime)
        {
            Dlt645_2007Protocol protocol = new Dlt645_2007Protocol();
            protocol.Address = address;
            protocol.Cmd = (byte)Dlt645_2007ControlCode.写数据;
            List<byte> data = new List<byte>();
            data.AddRange(EncodeDataIndex((uint)Dlt645_2007Cmd.日期));
            data.AddRange(CheckHelper.TransDataToBytes(password, false));
            data.AddRange(CheckHelper.TransDataToBytes(true));//操作者代码，默认为00000000
            data.Add((byte)dateTime.DayOfWeek);
            data.Add(CheckHelper.EncodeUnsignedBcd(dateTime.Day, true)[0]);
            data.Add(CheckHelper.EncodeUnsignedBcd(dateTime.Month, true)[0]);
            data.Add(CheckHelper.EncodeUnsignedBcd(dateTime.Year % 100, true)[0]);
            protocol.Data = data;
            return protocol.GetProtocol();
        }

        /// <summary>
        /// 设置电能表时间
        /// </summary>
        /// <param name="address">表地址</param>
        /// <param name="password">密码</param>
        /// <param name="dateTime">设置的时间</param>
        /// <returns></returns>
        public static List<byte> SetTimeProtocol(string address, string password, DateTime dateTime)
        {
            Dlt645_2007Protocol protocol = new Dlt645_2007Protocol();
            protocol.Address = address;
            protocol.Cmd = (byte)Dlt645_2007ControlCode.写数据;
            List<byte> data = new List<byte>();
            data.AddRange(EncodeDataIndex((uint)Dlt645_2007Cmd.时间));
            data.AddRange(CheckHelper.TransDataToBytes(password, false));
            data.AddRange(CheckHelper.TransDataToBytes(true));//操作者代码，默认为00000000
            data.Add(CheckHelper.EncodeUnsignedBcd(dateTime.Second, true)[0]);
            data.Add(CheckHelper.EncodeUnsignedBcd(dateTime.Minute, true)[0]);
            data.Add(CheckHelper.EncodeUnsignedBcd(dateTime.Hour, true)[0]);
            protocol.Data = data;
            return protocol.GetProtocol();
        }

        /// <summary>
        /// 获取第三运行状态字报文
        /// </summary>
        /// <param name="address">表地址</param>
        /// <returns></returns>
        public static List<byte> GetThirdStatusProtocol(string address)
        {
            Dlt645_2007Protocol protocol = new Dlt645_2007Protocol();
            protocol.Address = address;
            protocol.Cmd = (byte)Dlt645_2007ControlCode.读数据;
            protocol.Data = EncodeDataIndex((uint)Dlt645_2007Cmd.运行状态字3);
            return protocol.GetProtocol();
        }

        /// <summary>
        /// 电表清零
        /// </summary>
        /// <param name="address"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static List<byte> ClearMeterProtocol(string address, string password)
        {
            Dlt645_2007Protocol protocol = new Dlt645_2007Protocol();
            protocol.Address = address;
            protocol.Cmd = (byte)Dlt645_2007ControlCode.电表清零;
            List<byte> data = new List<byte>();
            data.AddRange(CheckHelper.TransDataToBytes(password, true));
            data.AddRange(CheckHelper.TransDataToBytes(true));//操作者代码，默认为00000000            
            protocol.Data = data;
            return protocol.GetProtocol();
        }

        /// <summary>
        /// 最大需量清零
        /// </summary>
        /// <param name="address"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static List<byte> ClearMaxDemandProtocol(string address, string password)
        {
            Dlt645_2007Protocol protocol = new Dlt645_2007Protocol();
            protocol.Address = address;
            protocol.Cmd = (byte)Dlt645_2007ControlCode.最大需量清零;
            List<byte> data = new List<byte>();
            data.AddRange(CheckHelper.TransDataToBytes(password, false));
            data.AddRange(CheckHelper.TransDataToBytes(true));//操作者代码，默认为00000000            
            protocol.Data = data;
            return protocol.GetProtocol();
        }

        /// <summary>
        /// 设置多功能端子
        /// </summary>
        /// <param name="address">表地址</param>
        /// <returns></returns>
        public static List<byte> SetMulTerminalOutProtocol(string address, VerificationElementType type)
        {
            Dlt645_2007Protocol protocol = new Dlt645_2007Protocol();
            protocol.Address = address;
            protocol.Cmd = (byte)Dlt645_2007ControlCode.多功能端子输出控制命令;
            protocol.Data = new List<byte>(new byte[] { GetMulTerminalOutCmd(type) });

            return protocol.GetProtocol();
        }

        /// <summary>
        /// 获取费率时间
        /// </summary>
        /// <param name="address">表地址</param>
        /// <returns></returns>
        public static List<byte> GetPhaseTimeProtocol(string address, Dlt645_2007Cmd cmd)
        {
            Dlt645_2007Protocol protocol = new Dlt645_2007Protocol();
            protocol.Address = address;
            protocol.Cmd = (byte)Dlt645_2007ControlCode.读数据;
            protocol.Data = EncodeDataIndex((uint)cmd);
            return protocol.GetProtocol();
        }

        /// <summary>
        /// 读费率电能
        /// </summary>
        /// <param name="address">表地址</param>
        /// <param name="pulse">脉冲</param>
        /// <returns></returns>
        public static List<byte> GetEnergyProtocol(string address, Pulse pulse)
        {
            Dlt645_2007Protocol protocol = new Dlt645_2007Protocol();
            protocol.Address = address;
            protocol.Cmd = (byte)Dlt645_2007ControlCode.读数据;
            protocol.Data = EncodeDataIndex(GetPeriodCmd(pulse));
            return protocol.GetProtocol();
        }

        /// <summary>
        /// 读需量
        /// </summary>
        /// <param name="address">表地址</param>
        /// <param name="pulse">脉冲</param>
        /// <returns></returns>
        public static List<byte> GetDemandProtocol(string address, Pulse pulse)
        {
            Dlt645_2007Protocol protocol = new Dlt645_2007Protocol();
            protocol.Address = address;
            protocol.Cmd = (byte)Dlt645_2007ControlCode.读数据;
            protocol.Data = EncodeDataIndex(GetDemandCmd(pulse));
            return protocol.GetProtocol();
        }

        /// <summary>
        /// 身份认证
        /// </summary>
        /// <param name="address">表地址</param>
        /// <param name="putDiv">分散因子</param>
        /// <param name="putRand">随机数</param>
        /// <param name="putPwd">随机密码</param>
        /// <returns>安全认证相关操作报文</returns>
        public static List<byte> SecurityCertificateProtocol(string address, string putDiv, string putRand, string putPwd)
        {
            Dlt645_2007Protocol protocol = new Dlt645_2007Protocol();
            protocol.Address = address;
            protocol.Cmd = (byte)Dlt645_2007ControlCode.安全认证相关;
            List<byte> data = new List<byte>();
            data.AddRange(EncodeDataIndex((uint)Dlt645_2007Cmd.身份认证));
            data.AddRange(CheckHelper.TransDataToBytes(true));//操作者代码，默认为00000000
            data.AddRange(CheckHelper.TransDataToBytes(putPwd, true));
            data.AddRange(CheckHelper.TransDataToBytes(putRand, true));
            data.AddRange(CheckHelper.TransDataToBytes(putDiv, true));
            protocol.Data = data;
            return protocol.GetProtocol();
        }

        /// <summary>
        /// 身份认证(私钥)
        /// </summary>
        /// <param name="address">表地址</param>
        /// <param name="putRand">随机数</param>
        /// <param name="putPwd">随机密码</param>
        /// <returns>安全认证相关操作报文</returns>
        public static List<byte> SecurityCertificateProtocol(string address, string putRand, string putPwd)
        {
            Dlt645_2007Protocol protocol = new Dlt645_2007Protocol();
            protocol.Address = address;
            protocol.Cmd = (byte)Dlt645_2007ControlCode.安全认证相关;
            List<byte> data = new List<byte>();
            data.AddRange(EncodeDataIndex((uint)Dlt645_2007Cmd.身份认证));
            data.AddRange(CheckHelper.TransDataToBytes(true));//操作者代码，默认为00000000
            data.AddRange(CheckHelper.TransDataToBytes(putPwd, true));
            data.AddRange(CheckHelper.TransDataToBytes(putRand, true));
            data.AddRange(ResolveAddress(address));
            protocol.Data = data;
            return protocol.GetProtocol();
        }

        /// <summary>
        /// 数据回抄
        /// </summary>
        /// <param name="address">表地址</param>
        /// <param name="fileID">文件标识</param>
        /// <param name="beginAddress">读取数据的相对起始地址</param>
        /// <param name="readLen">要读取的数据长度</param>
        /// <returns>安全认证相关操作报文</returns>
        public static List<byte> DataBackToCopyProtocol(string address, int fileID, int beginAddress, int readLen)
        {
            Dlt645_2007Protocol protocol = new Dlt645_2007Protocol();
            protocol.Address = address;
            protocol.Cmd = (byte)Dlt645_2007ControlCode.安全认证相关;
            List<byte> data = new List<byte>();
            data.AddRange(EncodeDataIndex((uint)Dlt645_2007Cmd.数据回抄));
            data.AddRange(CheckHelper.TransDataToBytes(true));//操作者代码，默认为00000000
            byte[] Arr = new byte[8];
            string strparas = readLen.ToString("x2") + "00" + beginAddress.ToString("x2") + "00" + fileID.ToString("x2") + "0001DF";
            strparas = "04000000060001DF";
            Arr = CheckHelper.StringToByte(strparas, true);
            data.AddRange(Arr);
            protocol.Data = data;
            return protocol.GetProtocol();
        }

        /// <summary>
        /// 拉合闸报警保电
        /// </summary>
        /// <param name="address">表地址</param>
        /// <returns>安全认证相关操作报文</returns>
        public static List<byte> KnifeSwitchAlarmProtocol(string address, string secretTxt)
        {
            Dlt645_2007Protocol protocol = new Dlt645_2007Protocol();
            protocol.Address = address;
            protocol.Cmd = (byte)Dlt645_2007ControlCode.拉合闸报警保电;
            List<byte> data = new List<byte>();
            data.AddRange(EncodeDataIndex(0));                 //空
            data.AddRange(CheckHelper.TransDataToBytes(true));//操作者代码，默认为00000000
            data.AddRange(CheckHelper.StringToByte(secretTxt, false));
            protocol.Data = data;
            return protocol.GetProtocol();
        }

        /// <summary>
        /// 主控密钥更新
        /// </summary>
        /// <param name="address">表地址</param>
        /// <param name="KeyKind">输入的密钥信息明文</param>
        /// <param name="OutKey">输出的密钥密文</param>
        /// <param name="OutKeyinfo">输出的密钥信息</param>
        /// <returns>安全认证相关操作报文</returns>
        public static List<byte> MainSecurityUpdateProtocol(string address, string keyKind, string outKey, string outKeyinfo)
        {
            return SecretKeyUpdateOfSecurity(Dlt645_2007Cmd.主控密钥更新, address, keyKind, outKey, outKeyinfo);
        }

        /// <summary>
        /// 控制命令密钥更新
        /// </summary>
        /// <param name="address">表地址</param>
        /// <param name="KeyKind">输入的密钥信息明文</param>
        /// <param name="OutKey">输出的密钥密文</param>
        /// <param name="OutKeyinfo">输出的密钥信息</param>
        /// <returns>安全认证相关操作报文</returns>
        public static List<byte> ControlSecurityUpdateProtocol(string address, string keyKind, string outKey, string outKeyinfo)
        {
            return SecretKeyUpdateOfSecurity(Dlt645_2007Cmd.控制命令密钥更新, address, keyKind, outKey, outKeyinfo);
        }

        /// <summary>
        /// 参数命令密钥更新
        /// </summary>
        /// <param name="address">表地址</param>
        /// <param name="KeyKind">输入的密钥信息明文</param>
        /// <param name="OutKey">输出的密钥密文</param>
        /// <param name="OutKeyinfo">输出的密钥信息</param>
        /// <returns>安全认证相关操作报文</returns>
        public static List<byte> ArguSecurityUpdateProtocol(string address, string keyKind, string outKey, string outKeyinfo)
        {
            return SecretKeyUpdateOfSecurity(Dlt645_2007Cmd.参数密钥更新, address, keyKind, outKey, outKeyinfo);
        }

        /// <summary>
        /// 身份认证命令密钥更新
        /// </summary>
        /// <param name="address">表地址</param>
        /// <param name="KeyKind">输入的密钥信息明文</param>
        /// <param name="OutKey">输出的密钥密文</param>
        /// <param name="OutKeyinfo">输出的密钥信息</param>
        /// <returns>安全认证相关操作报文</returns>
        public static List<byte> AuthSecurityUpdateProtocol(string address, string keyKind, string outKey, string outKeyinfo)
        {
            return SecretKeyUpdateOfSecurity(Dlt645_2007Cmd.身份认证密钥更新, address, keyKind, outKey, outKeyinfo);
        }

        /// <summary>
        /// 安全认证相关中的密钥更新
        /// </summary>
        /// <param name="code"></param>
        /// <param name="address"></param>
        /// <param name="keyKind"></param>
        /// <param name="outKey"></param>
        /// <param name="outKeyInfo"></param>
        /// <returns></returns>
        public static List<byte> SecretKeyUpdateOfSecurity(Dlt645_2007Cmd cmd, string address, string keyKind, string outKey, string outKeyInfo)
        {
            Dlt645_2007Protocol protocol = new Dlt645_2007Protocol();
            protocol.Address = address;
            protocol.Cmd = (byte)Dlt645_2007ControlCode.安全认证相关;
            List<byte> data = new List<byte>();
            data.AddRange(EncodeDataIndex((uint)cmd));
            data.AddRange(CheckHelper.TransDataToBytes(true));//操作者代码，默认为00000000
            data.AddRange(CheckHelper.StringToByte((outKey.ToString().Substring(0, 64) + keyKind.ToString() + outKeyInfo.ToString().Substring(0, 8)), false));
            protocol.Data = data;
            return protocol.GetProtocol();
        }

        ///解析645_2007协议控制码
        /// <summary>
        /// 解析645_2007协议控制码
        /// </summary>
        /// <param name="di"></param>
        /// <returns></returns>
        public static List<byte> EncodeDataIndex(uint di)
        {
            List<byte> ret = new List<byte>(4);
            ushort di1 = (ushort)(di % (256 * 256));
            ushort di2 = (ushort)(di / (256 * 256));
            ret.Add((byte)(di1 % 256));
            ret.Add((byte)(di1 / 256));
            ret.Add((byte)(di2 % 256));
            ret.Add((byte)(di2 / 256));
            return ret;
        }

        /// <summary>
        /// 获取多功能端子输出命令
        /// </summary>
        /// <param name="type">检定类型</param>
        /// <returns></returns>
        private static byte GetMulTerminalOutCmd(VerificationElementType type)
        {
            switch (type)
            {
                case VerificationElementType.时段投切:
                    return 0x02;
                case VerificationElementType.需量周期误差试验:
                    return 0x01;
                default:
                    return 0x0;
            }
        }

        /// <summary>
        /// 获取需量指令
        /// </summary>
        /// <param name="pulse"></param>
        /// <returns></returns>
        private static uint GetDemandCmd(Pulse pulse)
        {
            switch (pulse)
            {
                case Pulse.正向有功:
                    return (uint)Dlt645_2007Cmd.正向有功需量;
                case Pulse.正向无功:
                    return (uint)Dlt645_2007Cmd.正向无功需量;
                case Pulse.反向有功:
                    return (uint)Dlt645_2007Cmd.反向有功需量;
                default:
                    return (uint)Dlt645_2007Cmd.反向无功需量;
            }
        }

        /// <summary>
        /// 获取费控指令
        /// </summary>
        /// <param name="pulse">脉冲</param>
        /// <returns></returns>
        private static uint GetPeriodCmd(Pulse pulse)
        {
            switch (pulse)
            {
                case Pulse.正向有功:
                    return (uint)Dlt645_2007Cmd.正向有功电能;
                case Pulse.正向无功:
                    return (uint)Dlt645_2007Cmd.正向无功电能;
                case Pulse.反向有功:
                    return (uint)Dlt645_2007Cmd.反向有功电能;
                default:
                    return (uint)Dlt645_2007Cmd.反向无功电能;
            }
        }

        /// <summary>
        /// 拆分表地址
        /// </summary>
        /// <param name="address">表地址</param>
        /// <returns></returns>
        public static List<byte> ResolveAddress(string address)
        {
            List<byte> lstAddress = new List<byte>();
            if (string.IsNullOrEmpty(address))
            {
                address = "AAAAAAAAAAAA";
            }

            for (int i = 5; i >= 0; i--)
            {
                lstAddress.Add((byte)Convert.ToInt64(address.Substring(i * 2, 2), 16));
            }

            return lstAddress;
        }

        #endregion

        #region 解析报文

        /// <summary>
        /// 分析电表清零回发报文
        /// </summary>
        /// <param name="datas">回传数据</param>
        /// <returns></returns>
        public static bool AnalyseClearMeter(List<byte> datas, out string resultDescription)
        {
            List<byte> lstData;
            resultDescription = string.Empty;
            bool result = DataAnalyzer(datas, Dlt645_2007ControlCode.电表清零, out lstData);
            if (!result)
            {
                resultDescription = AnalyzeErrorData(lstData);
            }
            return result;
        }

        /// <summary>
        /// 分析读表地址回发报文
        /// </summary>
        /// <param name="buff">回传数据</param>
        /// <returns></returns>
        public static string AnalyseGetAddress(List<byte> datas, out bool result, out string resultDescription)
        {
            List<byte> lstData;
            string address = string.Empty;
            result = true;
            resultDescription = string.Empty;
            if (DataAnalyzer(datas, Dlt645_2007ControlCode.读通信地址, out lstData))
            {
                lstData.Reverse();
                for (int i = 0; i < lstData.Count; i++)
                {
                    address += string.Format("{0:X2}", lstData[i]);
                }
            }
            else
            {
                result = false;
                resultDescription = AnalyzeErrorData(lstData);
            }

            return address;
        }

        /// <summary>
        /// 分析读数据回发报文
        /// </summary>
        /// <param name="buff">回传数据</param>
        /// <returns></returns>
        public static string AnalyseGetBankCmd(List<byte> datas, out bool result, out string resultDescription)
        {
            List<byte> lstData;
            string volt = string.Empty;
            result = true;
            resultDescription = string.Empty;
            if (DataAnalyzer(datas, Dlt645_2007ControlCode.读数据, out lstData))
            {
                lstData.Reverse();
                for (int i = 0; i < lstData.Count; i++)
                {
                    volt += string.Format("{0:X2}", lstData[i]);
                }
            }
            else
            {
                result = false;
                resultDescription = AnalyzeErrorData(lstData);
            }

            return volt;
        }


        /// <summary>
        /// 检查返回帧合法性
        /// </summary>
        /// <param name="bWrap">返回帧</param>
        /// <param name="cData">返回数据</param>
        /// <param name="bExtend">是否有后续帧</param>
        /// <returns></returns>
        public static bool CheckFrame(List<byte> datas, ref string cData, ref bool bExtend)
        {/// 解析数据  cWrap 需要解析的包 cData 返回的数据	bExtend 是否有后续数据
            try
            {
                byte[] byt_RevFrame = datas.ToArray();
                if (byt_RevFrame.Length <= 0)
                {
                    //m_str_LostMessage = "没有返回数据！";
                    return false;
                }
                int int_Start = 0;
                int_Start = Array.IndexOf(byt_RevFrame, (byte)0x68);
                if (int_Start < 0 || int_Start > byt_RevFrame.Length || int_Start + 12 > byt_RevFrame.Length) //没有68开头 长度是否足够一帧 是否完整
                {
                    ///this.m_str_LostMessage = "返回帧不完整！没有帧头。[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }

                if (byt_RevFrame[int_Start + 7] != 0x68)        //找不到第二个68
                {
                    //this.m_str_LostMessage = "返回帧不完整！[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }

                int int_Len = byt_RevFrame[int_Start + 9];
                if (int_Start + 12 + int_Len != byt_RevFrame.Length)
                {
                    //this.m_str_LostMessage = "数据长度与实际长度不一致！[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;                //帧的长度是否与实际长度一样
                }
                byte byt_Chksum = 0;
                for (int int_Inc = int_Start; int_Inc < int_Start + int_Len + 10; int_Inc++)
                    byt_Chksum += byt_RevFrame[int_Inc];
                if (byt_RevFrame[int_Start + int_Len + 10] != byt_Chksum)       //校验码不正确
                {
                    //this.m_str_LostMessage = "返回校验码不正确！[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }
                if (byt_RevFrame[int_Start + int_Len + 11] != 0x16)       //没有16结束
                {
                    //this.m_str_LostMessage = "返回帧不完整！[" + BitConverter.ToString(byt_RevFrame) + "]";
                    return false;
                }

                //Array.Resize(ref byt_Addr, 6);
                //Array.Copy(byt_RevFrame, int_Start + 1, byt_Addr, 0, 6);
                //cmd
                byte[] byt_RevData = new byte[0];
                Array.Resize(ref byt_RevData, int_Len);    //数据域长度
                if (int_Len > 0)
                {
                    Array.Copy(byt_RevFrame, int_Start + 10, byt_RevData, 0, int_Len);
                    for (int int_Inc = 0; int_Inc < byt_RevData.Length; int_Inc++)
                        byt_RevData[int_Inc] -= 0x33;

                    cData = BytesToString(byt_RevData);
                }


                //是否有后续帧
                if ((byt_RevFrame[int_Start + 8] & 0x20) == 0x20)
                    bExtend = true;
                else
                    bExtend = false;

                //
                string str_Value;
                str_Value = cData.Substring(2);
                str_Value = BackString(str_Value);
                cData = str_Value;
                //是否返回操作成功     第7Bit是1则是返回，第6bit是0=成功，1=失败
                if ((byt_RevFrame[int_Start + 8] & 0x80) == 0x80 && (byt_RevFrame[int_Start + 8] & 0x40) == 0x00)
                    return true;
                else
                {
                    string m_str_LostMessage;
                    if (byt_RevData != null && byt_RevData.Length > 0)
                    {
                        //m_str_LostMessage = GetErrorMsg645(byt_RevData[0]) + "[" + BitConverter.ToString(byt_RevFrame) + "]";
                    }
                    
                    else
                    {
                        //this.m_str_LostMessage = "返回操作失败！[" + BitConverter.ToString(byt_RevFrame) + "]";
                    }
                    return false;
                }

            }
            catch (Exception e)
            {
                //this.m_str_LostMessage = e.ToString();
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="byt_Values"></param>
        /// <returns></returns>
        private static string BytesToString(byte[] byt_Values)
        {
            StringBuilder strBuilder = new StringBuilder();
            string strData = string.Empty;
            foreach (byte bt in byt_Values)
            {
                strData = Convert.ToChar(bt).ToString() ;
                strBuilder.AppendFormat("{0}", strData);
            }
            return strBuilder.ToString();
        }
        /// <summary>
        /// 反转字节字符串
        /// </summary>
        /// <param name="sData"></param>
        /// <returns></returns>
        public static string BackString(string sData)
        {		//字符重新排序
            int ilen = sData.Length;
            string stemp = "";
            if (ilen <= 0) return "";
            for (int tn = 0; tn < ilen; tn++)
            {
                stemp = sData.Substring(tn, 1) + stemp;
            }
            return stemp;
        }
        /// <summary>
        /// 分析写表地址回发报文
        /// </summary>
        /// <param name="datas">回传数据</param>
        /// <returns></returns>
        public static bool AnalyseSetAddress(List<byte> datas, out string resultDescription)
        {
            List<byte> lstData;
            bool result = DataAnalyzer(datas, Dlt645_2007ControlCode.写通信地址, out lstData);
            resultDescription = string.Empty;
            if (!result)
            {
                resultDescription = AnalyzeErrorData(lstData);
            }

            return result;
        }

        /// <summary>
        /// 分析读日期回发报文
        /// </summary>
        /// <param name="datas">回传数据</param>
        /// <returns></returns>
        public static string AnalyseGetDate(List<byte> datas, out bool result, out string resultDescription)
        {
            List<byte> lstData;
            result = true;
            resultDescription = string.Empty;
            if (DataAnalyzer(datas, Dlt645_2007ControlCode.读数据, out lstData))
            {
                if (lstData.Count > 4)
                {
                    lstData.RemoveRange(0, 4);
                    lstData.Reverse();
                    if (lstData.Count > 2)
                    {
                        return string.Format("{0:X2}-{1:X2}-{2:X2}", lstData[0], lstData[1], lstData[2]);
                    }
                }
            }
            else
            {
                result = true;
                resultDescription = AnalyzeErrorData(lstData);
            }
            return null;
        }

        /// <summary>
        /// 分析读时间回发报文
        /// </summary>
        /// <param name="datas">回传数据</param>
        /// <returns></returns>
        public static string AnalyseGetTime(List<byte> datas, out bool result, out string resultDescription)
        {
            List<byte> lstData;
            result = true;
            resultDescription = string.Empty;
            if (DataAnalyzer(datas, Dlt645_2007ControlCode.读数据, out lstData))
            {
                if (lstData.Count > 4)
                {
                    lstData.RemoveRange(0, 4);
                    lstData.Reverse();
                    if (lstData.Count > 2)
                    {
                        return string.Format("{0:X2}:{1:X2}:{2:X2}", lstData[0], lstData[1], lstData[2]);
                    }
                }
            }
            else
            {
                result = true;
                resultDescription = AnalyzeErrorData(lstData);
            }
            return null;
        }

        /// <summary>
        /// 分析写日期回发报文
        /// </summary>
        /// <param name="datas">回传数据</param>
        /// <returns></returns>
        public static bool AnalyseSetDate(List<byte> datas, out string resultDescription)
        {
            List<byte> lstData;
            resultDescription = string.Empty;

            bool result = DataAnalyzer(datas, Dlt645_2007ControlCode.写数据, out lstData);
            if (!result)
            {
                resultDescription = AnalyzeErrorData(lstData);
            }

            return result;
        }
        /// <summary>
        /// 解析写数据结论
        /// </summary>
        /// <param name="datas">返回报文</param>
        /// <param name="resultDescription"></param>
        /// <returns></returns>
        public static bool AnalyseSetResutData(List<byte> datas, out string resultDescription)
        {
            List<byte> lstData;
            resultDescription = string.Empty;

            bool result = DataAnalyzer(datas, Dlt645_2007ControlCode.写数据, out lstData);
            if (!result)
            {
                resultDescription = AnalyzeErrorData(lstData);//异常信息
            }

            return result;
        }

        /// <summary>
        /// 分析写时间回发报文
        /// </summary>
        /// <param name="datas">回传数据</param>
        /// <returns></returns>
        public static bool AnalyseSetTime(List<byte> datas, out string resultDescription)
        {
            List<byte> lstData;
            resultDescription = string.Empty;

            bool result = DataAnalyzer(datas, Dlt645_2007ControlCode.写数据, out lstData);
            if (!result)
            {
                resultDescription = AnalyzeErrorData(lstData);
            }

            return result;
        }

        /// <summary>
        /// 分析读费时电量回发报文
        /// </summary>
        /// <param name="datas">回传数据</param>
        /// <returns></returns>
        public static string AnalyzeGetEnergy(List<byte> datas, out bool result, out string resultDescription)
        {
            string energy = string.Empty;
            List<byte> lstData;
            result = true;
            resultDescription = string.Empty;

            if (DataAnalyzer(datas, Dlt645_2007ControlCode.读数据, out lstData))
            {
                if (lstData.Count > 4)
                {
                    lstData.RemoveRange(0, 4);
                    lstData.Reverse();
                    List<float> lstPhaseEnergy = new List<float>();
                    for (int i = 0; i < lstData.Count; i = i + 4)
                    {
                        if (i + 4 <= lstData.Count)
                        {
                            string phaseEnergy = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", lstData[i], lstData[i + 1], lstData[i + 2], lstData[i + 3]);
                            lstPhaseEnergy.Add(Convert.ToSingle(string.Format("{0:f2}", CheckHelper.FormatStringToDouble(phaseEnergy) / 100)));
                        }
                    }
                    lstPhaseEnergy.Reverse();
                    int lstCount=lstPhaseEnergy.Count;
                    string[] strPE = new string[lstCount];
                    for (int i = 0; i < lstCount; i++)
                    {
                        strPE[i] = lstPhaseEnergy[i].ToString();
                    }
                    energy = string.Join(",", strPE);
                }
            }
            else
            {
                result = false;
                resultDescription = AnalyzeErrorData(lstData);
            }

            return energy;
        }

        /// <summary>
        /// 分析需量示值回发报文
        /// </summary>
        /// <param name="datas">回传数据</param>
        /// <returns></returns>
        public static string AnalyzeGetDemand(List<byte> datas, out bool result, out string resultDescription)
        {
            string demand = string.Empty;
            List<byte> lstData;
            result = true;
            resultDescription = string.Empty;

            if (DataAnalyzer(datas, Dlt645_2007ControlCode.读数据, out lstData))
            {
                if (lstData.Count > 4)
                {
                    lstData.RemoveRange(0, 4);
                    lstData.Reverse();
                    lstData.RemoveRange(0, 5);

                    if (lstData.Count > 2)
                    {
                        demand = string.Format("{0:f4}", CheckHelper.FormatStringToDouble(string.Format("{0:X2}{1:X2}{2:X2}", lstData[0], lstData[1], lstData[2])) / 10000);
                    }
                }
            }
            else
            {
                result = false;
                resultDescription = AnalyzeErrorData(lstData);
            }

            return demand;
        }

        /// <summary>
        /// 分析清最大需量回发报文
        /// </summary>
        /// <param name="datas">回传数据</param>
        /// <returns></returns>
        public static bool AnalyzeClearMaxDemand(List<byte> datas, out string resultDescription)
        {
            List<byte> lstData;
            resultDescription = string.Empty;
            bool result = DataAnalyzer(datas, Dlt645_2007ControlCode.最大需量清零, out lstData);
            if (!result)
            {
                resultDescription = AnalyzeErrorData(lstData);
            }
            return result;
        }

        /// <summary>
        /// 分析多功能端子输出回发报文
        /// </summary>
        /// <param name="datas">回传数据</param>
        /// <returns></returns>
        public static bool AnalyzeSetMulTerminalOut(List<byte> datas, out string resultDescription)
        {
            List<byte> lstData;
            resultDescription = string.Empty;
            bool result = DataAnalyzer(datas, Dlt645_2007ControlCode.多功能端子输出控制命令, out lstData);
            if (!result)
            {
                resultDescription = AnalyzeErrorData(lstData);
            }
            return result;
        }

        /// <summary>
        /// 获取第三运行状态字回发报文
        /// </summary>
        /// <param name="datas">回传数据</param>
        /// <returns></returns>
        public static string AnalyzeGetThirdStatus(List<byte> datas, out bool result, out string resultDescription)
        {
            string data = string.Empty;
            List<byte> lstData;
            result = true;
            resultDescription = string.Empty;

            if (DataAnalyzer(datas, Dlt645_2007ControlCode.读数据, out lstData))
            {
                lstData.RemoveRange(0, 4);
                lstData.Reverse();
                for (int i = 0; i < lstData.Count(); i++)
                {
                    data += Convert.ToString(lstData[i], 2).PadLeft(4, '0');
                }
            }
            else
            {
                result = false;
                resultDescription = AnalyzeErrorData(lstData);
            }
            return data;
        }

        /// 分析身份验证回发报文
        /// <summary>
        /// 分析身份验证回发报文
        /// </summary>
        /// <param name="datas">回传数据</param>
        /// <returns></returns>
        public static bool AnalyzeSecurityCertificate(List<byte> datas, out string outRandNum, out string outEsam, out string resultDescription)
        {
            List<byte> lstData;
            bool result = true;
            resultDescription = string.Empty;
            outRandNum = string.Empty;
            outEsam = string.Empty;
            if (DataAnalyzer(datas, Dlt645_2007ControlCode.安全认证相关, out lstData))
            {
                List<byte> lstRandNum = lstData.GetRange(0, 4);

                // 分析随机数
                lstRandNum.Reverse();
                for (int i = 0; i < lstRandNum.Count; i++)
                {
                    outRandNum += Convert.ToString(lstRandNum[i], 16).PadLeft('0');
                }

                lstData.RemoveRange(0, 4);
                lstData.Reverse();
                for (int i = 0; i < lstData.Count; i++)
                {
                    outEsam += Convert.ToString(lstData[i], 16).PadLeft('0');
                }
            }
            else
            {
                result = false;
                resultDescription = AnalyzeErrorData(lstData);
            }
            return result;
        }

        /// 分析数据回抄回发报文
        /// <summary>
        /// 分析数据回抄回发报文
        /// </summary>
        /// <param name="datas">回传数据</param>
        /// <param name="result">结论</param>
        /// <param name="resultDescription">结论描述</param>
        /// <returns></returns>
        public static string AnalyzeDataBackToCopy(List<byte> datas, out bool result, out string resultDescription)
        {
            List<byte> lstData;
            string data = "01010304"; // 默认为私密状态
            result = true;
            resultDescription = string.Empty;
            if (DataAnalyzer(datas, Dlt645_2007ControlCode.安全认证相关, out lstData))
            {
                if (lstData.Count >= 25)
                {
                    lstData.RemoveRange(24, lstData.Count - 25);
                    lstData.Reverse();
                    for (int i = 0; i < 4; i++)
                    {
                        data += lstData[i].ToString("X2");
                    }
                }
                else
                {
                    data = "00000000";
                }
            }
            else
            {
                result = false;
                resultDescription = AnalyzeErrorData(lstData);
            }
            return data;
        }

        /// 分析拉合闸报警保电回发报文
        /// <summary>
        /// 分析拉合闸报警保电回发报文
        /// </summary>
        /// <param name="datas">回传数据</param>
        /// <returns></returns>
        public static bool AnalyzeKnifeSwitchAlarm(List<byte> datas, out string resultDescription)
        {
            List<byte> lstData;
            resultDescription = string.Empty;
            bool result = DataAnalyzer(datas, Dlt645_2007ControlCode.拉合闸报警保电, out lstData);
            if (result)
            {
                if (lstData != null && lstData.Count > 0 && String.IsNullOrEmpty(resultDescription))
                {
                    result = lstData.ToString().Contains("9C"); //9C为合格标志
                }
            }
            else
            {
                result = false;
                resultDescription = AnalyzeErrorData(lstData);
            }
            return result;
        }

        /// 分析密钥更新回发报文
        /// <summary>
        /// 分析密钥更新回发报文
        /// </summary>
        /// <param name="datas">回传数据</param>
        /// <returns></returns>
        public static bool AnalyzeSecurityUpdate(List<byte> datas, out string resultDescription)
        {
            List<byte> lstData;
            resultDescription = string.Empty;
            bool result = DataAnalyzer(datas, Dlt645_2007ControlCode.安全认证相关, out lstData);
            if (!result)
            {
                resultDescription = AnalyzeErrorData(lstData);
            }
            return result;
        }

        /// <summary>
        /// 分析读费率时间回发报文
        /// </summary>
        /// <param name="datas">回传数据</param>
        /// <param name="Series">第几套</param>
        /// <returns></returns>
        public static MeterRates[] AnalyzeGetPhaseTime(List<byte> datas, int Series, out bool result, out string resultDescription)
        {
            List<byte> lstData;
            result = true;
            resultDescription = string.Empty;

            if (DataAnalyzer(datas, Dlt645_2007ControlCode.读数据, out lstData))
            {
                if (lstData.Count > 4)
                {
                    lstData.RemoveRange(0, 4);
                    lstData.Reverse();
                    if (lstData.Count > 3)
                    {
                        Dictionary<string, MeterRates> dictMeterRates = new Dictionary<string, MeterRates>();
                        for (int i = 0; i < lstData.Count; i = i + 3)
                        {
                            MeterRates mr = new MeterRates();
                            if (i + 3 <= lstData.Count)
                            {
                                mr.DefaultTime = "8:00";
                                mr.PeriodTime = string.Format("{0:X2}:{1:X2}", lstData[i], lstData[i + 1]);
                                mr.Period = (Phase)Enum.Parse(typeof(Phase), string.Format("{0:X2}", lstData[i + 2]));
                                mr.Series = Series;

                                if (!dictMeterRates.ContainsKey(mr.PeriodTime))
                                {
                                    dictMeterRates.Add(mr.PeriodTime, mr);
                                }
                            }
                        }

                        return dictMeterRates.Values.ToArray();
                    }
                }
            }
            else
            {
                result = false;
                resultDescription = AnalyzeErrorData(lstData);
            }
            return null;
        }

        /// <summary>
        /// 回发错误数据分析
        /// </summary>
        /// <param name="errDatas">错误数据代码</param>
        /// <returns></returns>
        private static string AnalyzeErrorData(List<byte> errDatas)
        {
            string errMsg = "未知异常!";
            if (errDatas.Count > 0)
            {
                IList<char> lstErrCode = Convert.ToString(errDatas[0], 2).PadLeft(8, '0').Reverse().ToList();
                for (int i = 0; i < lstErrCode.Count; i++)
                {
                    if (lstErrCode[i] == '1')
                    {
                        errMsg = ((ErrMsg)Enum.Parse(typeof(ErrMsg), i.ToString())).ToString();
                        break;
                    }
                }
            }
            return errMsg;
        }

        /// <summary>
        /// 分析回发报文
        /// </summary>
        /// <param name="datas">回传数据</param>
        /// <returns></returns>
        private static bool DataAnalyzer(List<byte> datas, Dlt645_2007ControlCode controlCode, out List<byte> analyzerData)
        {
            analyzerData = new List<byte>();
            int index = -1;
            if (datas != null && datas.Count > 0)
            {
                index = CheckHelper.IndexOf<byte>(datas, 0x68);
            }
            if (index > -1)
            {
                if (datas.Count - index > 11
                    && datas.Count - index > (datas[index + 9] + 11)
                    && datas[index + 7] == 0x68
                    && datas[index + datas[index + 9] + 10] == CheckHelper.CheckSum(datas, index, datas[index + 9] + 10)
                    && datas[index + datas[index + 9] + 11] == 0x16)
                {
                    if ((datas[index + 8] - 0x80) == (byte)controlCode)
                    {
                        byte[] arr = new byte[datas[index + 9]];
                        Array.Copy(datas.ToArray(), index + 10, arr, 0, arr.Length);
                        for (int i = 0; i < arr.Length; i++)
                        {
                            analyzerData.Add((byte)(arr[i] - 0x33));
                        }

                        return true;
                    }
                    else if ((datas[index + 8] - 0xC0) == (byte)controlCode)
                    {
                        analyzerData.Add(datas[index + 10]);// 错误应答帧
                        return false;
                    }
                }
            }
            return false;
        }

        #endregion
    }

    /// <summary>
    /// 错误信息
    /// </summary>
    public enum ErrMsg : int
    {
        其它错误 = 0,
        无请求数据 = 1,
        密码错或未授权 = 2,
        通信速率不能更新,
        年时区数超,
        日时段数超,
        费率数超,
        保留
    }
}
