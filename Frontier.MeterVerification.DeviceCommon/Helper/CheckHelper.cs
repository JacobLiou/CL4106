using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Frontier.MeterVerification.DeviceCommon
{
    public static class CheckHelper
    {
        public static ushort Crc16(int N, byte[] ArryB)
        {
            ushort CRC = 0xFFFF;
            ushort temp = 0xA001;
            for (int k = 0; k < N; k++)
            {
                CRC ^= ArryB[k];
                for (int i = 0; i < 8; i++)
                {
                    int j = CRC & 1;
                    CRC >>= 1;
                    CRC &= 0x7FFF;
                    if (j == 1)
                        CRC ^= temp;
                }
            }
            return CRC;
        }

        /// <summary>
        /// 计算异或值
        /// </summary>
        /// <param name="ArrB">字节数组</param>
        /// <param name="IndexStart">开始位置</param>
        /// <param name="ByteCount">计算长度</param>
        /// <returns>异或结果</returns>
        public static byte CheckXor(List<byte> arr, int offset, int count)
        {
            Debug.Assert(arr != null && offset >= 0 && count >= 0 && offset + count <= arr.Count);
            byte ret = 0;
            if (count == 0) count = arr.Count - offset;
            for (int i = offset; i < offset + count; i++) ret ^= arr[i];
            return ret;
        }

        /// <summary>
        /// 计算校验和
        /// </summary>
        /// <param name="ArrB">字节数组</param>
        /// <param name="IndexStart">开始位置</param>
        /// <param name="ByteCount">计算长度</param>
        /// <returns>校验和结果</returns>
        public static byte CheckSum(List<byte> arr, int offset, int count)
        {
            Debug.Assert(arr != null && offset >= 0 && count > 0 && offset + count <= arr.Count);
            byte ret = 0;
            for (int i = offset; i < offset + count; i++) ret += arr[i];
            return ret;
        }

        /// <summary>
        /// 对数组的部分元素求最大值
        /// </summary>
        /// <typeparam name="T">数组元素类型</typeparam>
        /// <param name="arr">数组</param>
        /// <param name="offset">起始位置</param>
        /// <param name="count">元素个数</param>
        /// <returns>最大值</returns>
        public static T Max<T>(T[] arr, int offset, int count) where T : IComparable<T>
        {
            Debug.Assert(arr != null && offset >= 0 && count > 0 && offset + count <= arr.Length);
            T ret = arr[offset];
            for (int i = offset; i < offset + count; i++)
                if (arr[i].CompareTo(ret) > 0) ret = arr[i];
            return ret;
        }

        /// <summary>
        /// 对列举的同类型变量求最大值
        /// </summary>
        /// <typeparam name="T">变量类型</typeparam>
        /// <param name="arr">列举的变量</param>
        /// <returns>最大值</returns>
        public static T Max<T>(params T[] arr) where T : IComparable<T>
        {
            return Max<T>(arr, 0, arr.Length);
        }

        /// <summary>
        /// 对数组的部分元素求最小值
        /// </summary>
        /// <typeparam name="T">数组元素类型</typeparam>
        /// <param name="arr">数组</param>
        /// <param name="offset">起始位置</param>
        /// <param name="count">元素个数</param>
        /// <returns>最小值</returns>
        public static T Min<T>(T[] arr, int offset, int count) where T : IComparable<T>
        {
            Debug.Assert(arr != null && offset >= 0 && count > 0 && offset + count <= arr.Length);
            T ret = arr[offset];
            for (int i = offset; i < offset + count; i++)
                if (arr[i].CompareTo(ret) < 0) ret = arr[i];
            return ret;
        }

        /// <summary>
        /// 对列举的同类型变量求最小值
        /// </summary>
        /// <typeparam name="T">变量类型</typeparam>
        /// <param name="arr">列举的变量</param>
        /// <returns>最小值</returns>
        public static T Min<T>(params T[] arr) where T : IComparable<T>
        {
            return Min<T>(arr, 0, arr.Length);
        }

        /// <summary>
        /// 动态数组的扩展函数，寻找第一次出现后续元素的位置
        /// </summary>
        /// <typeparam name="T">数组元素类型</typeparam>
        /// <param name="list"></param>
        /// <param name="arr">所寻找的元素列表</param>
        /// <returns>第一次出现后续列举元素的位置</returns>
        public static int IndexOf<T>(this List<T> list, params T[] arr)
        {
            Debug.Assert(arr != null && arr.Length > 0);
            int ret = int.MaxValue, temp = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                temp = list.IndexOf(arr[i]);
                if (temp != -1 && temp < ret) ret = temp;
            }
            if (ret == int.MaxValue) ret = -1;
            return ret;
        }

        /// <summary>
        /// 返回arrB在arrA中的位置
        /// </summary>
        /// <param name="arrA"></param>
        /// <param name="arrB"></param>
        /// <returns></returns>
        public static int IndexOf(List<byte> arrA, List<byte> arrB)
        {
            if (arrB.Count <= 0 || arrA.Count <= 0)
                return -1;
            int index = -1;
            for (int i = 0; i < arrA.Count; i++)
            {
                if (index < arrB.Count && arrA[i] == arrB[index + 1])
                {
                    index++;
                }
                else
                {
                    index = -1;
                }
                if (index > arrB.Count)
                {
                    break;
                }
            }
            return index;
        }

        public static byte[] FloatToByteArr(float val, int len)
        {
            Debug.Assert(len > 0);
            string s = val.ToString();
            Debug.Assert(s.Length <= len);
            if (s.Length < len && !s.Contains('.')) s += '.';
            return Encoding.ASCII.GetBytes(s.PadRight(len, '0'));
        }

        /// <summary>
        /// 按照南自误差板标准，编码带符号BCD码
        /// </summary>
        /// <param name="val">数值</param>
        /// <param name="len">码值字节数，不可小于所必需的长度</param>
        /// <param name="littleEndian">true表示低字节在前，false表示高字节在前</param>
        /// <returns>编码生成的字节数组</returns>
        /// <remarks>最高字节的bit6为符号，0表示正，1表示负。多余字节补0。n字节最多只能表示2n-1位十进制数。</remarks>
        public static List<byte> EncodeSignedBcd(long val, int len, bool littleEndian)
        {
            Debug.Assert(len > 0);
            List<byte> ret = new List<byte>(len);
            long remain = 0, temp = 0, sign = 0;
            if (val >= 0)
            {
                remain = val;
                sign = 1;
            }
            else
            {
                remain = -val;
                sign = -1;
            }
            while (remain > 0)
            {
                temp = remain % 100;
                ret.Add((byte)(temp % 10 | temp / 10 << 4));
                remain /= 100;
            }
            Debug.Assert(ret.Count < len || ret.Count == len && (ret[ret.Count - 1] & 0xF0) == 0);
            while (ret.Count < len) ret.Add(0);
            if (sign == -1) ret[ret.Count - 1] |= 0x40;
            if (!littleEndian) ret.Reverse();
            return ret;
        }

        /// <summary>
        /// 编码无符号BCD码
        /// </summary>
        /// <param name="val">数值</param>
        /// <param name="littleEndian">true表示低字节在前，false表示高字节在前</param>
        /// <returns>编码生成的字节数组</returns>
        public static List<byte> EncodeUnsignedBcd(long val, bool littleEndian)
        {
            Debug.Assert(val >= 0);
            List<byte> ret = new List<byte>();
            long remain = val, temp = 0;
            while (remain > 0)
            {
                temp = remain % 100;
                ret.Add((byte)(temp % 10 | temp / 10 << 4));
                remain /= 100;
            }
            if (ret.Count == 0) ret.Add(0);
            if (!littleEndian) ret.Reverse();
            return ret;
        }

        /// <summary>
        /// 编码无符号BCD码
        /// </summary>
        /// <param name="val">数值</param>
        /// <param name="len">码值字节数，不可小于所必需的长度</param>
        /// <param name="littleEndian">true表示低字节在前，false表示高字节在前</param>
        /// <returns>编码生成的字节数组</returns>
        /// <remarks>n字节最多只能表示2n位十进制数。</remarks>
        public static List<byte> EncodeUnsignedBcd(long val, int len, bool littleEndian)
        {
            List<byte> ret = EncodeUnsignedBcd(val, true);
            Debug.Assert(ret.Count <= len);
            while (ret.Count < len) ret.Add(0);
            if (!littleEndian) ret.Reverse();
            return ret;
        }

        /// <summary>
        /// 按照南自误差板标准，解码带符号BCD码
        /// </summary>
        /// <param name="arr">字节数组</param>
        /// <param name="offset">起始位置</param>
        /// <param name="count">码值字节数</param>
        /// <param name="littleEndian">true表示低位在前，false表示高位在前</param>
        /// <returns>解码后的整数</returns>
        /// <remarks>最高字节的bit6为符号，0表示正，1表示负。</remarks>
        public static long DecodeSignedBcd(List<byte> arr, int offset, int count, bool littleEndian)
        {
            Debug.Assert(arr != null && offset >= 0 && count > 0 && offset + count <= arr.Count);
            List<byte> temp = arr.GetRange(offset, count);
            if (littleEndian) temp.Reverse();
            int sign = (temp[0] & 0x40) == 0 ? 1 : -1;
            long ret = temp[0] & 0x0F;
            for (int i = 1; i < count; i++)
            {
                ret *= 100;
                ret += ((temp[i] & 0xF0) >> 4) * 10 + (temp[i] & 0x0F);
            }
            ret *= sign;
            return ret;
        }

        /// <summary>
        /// 解码无符号BCD码
        /// </summary>
        /// <param name="arr">字节数组</param>
        /// <param name="offset">起始位置</param>
        /// <param name="count">码值字节数</param>
        /// <param name="littleEndian">true表示低位在前，false表示高位在前</param>
        /// <returns>解码后的整数</returns>
        public static long DecodeUnsignedBcd(List<byte> arr, int offset, int count, bool littleEndian)
        {
            Debug.Assert(arr != null && offset >= 0 && count > 0 && offset + count <= arr.Count);
            List<byte> temp = arr.GetRange(offset, count);
            if (littleEndian) temp.Reverse();
            long ret = 0;
            for (int i = 0; i < count; i++)
            {
                ret *= 100;
                ret += ((temp[i] & 0xF0) >> 4) * 10 + (temp[i] & 0x0F);
            }
            return ret;
        }

        /// <summary>
        /// 十进制转换为16进制数
        /// </summary>
        /// <param name="val"></param>
        /// <param name="littleEndian"></param>
        /// <returns></returns>
        public static List<byte> EncodeDecToHex(long val, bool isLowFront)
        {
            Debug.Assert(val >= 0);
            List<byte> ret = new List<byte>();
            long remain = val, temp = 0;
            //低位先增加入列表
            while (remain > 0)
            {
                temp = remain % 256;
                ret.Add((byte)(temp % 16 | temp / 16 << 4));
                remain /= 256;
            }
            if (ret.Count == 0) ret.Add(0);
            if (ret.Count == 1) ret.Add(0);
            //是否反序
            if (!isLowFront) ret.Reverse();
            return ret;
        }

        /// <summary>
        /// 16进制转换为十进制
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="littleEndian"></param>
        /// <returns></returns>
        public static long DecodeHexToDec(List<byte> arr, int offset, int count, bool littleEndian)
        {
            Debug.Assert(arr != null && offset >= 0 && count > 0 && offset + count <= arr.Count);
            List<byte> temp = arr.GetRange(offset, count);
            if (littleEndian) temp.Reverse();
            long ret = 0;
            for (int i = 0; i < count; i++)
            {
                ret *= 256;
                ret += ((temp[i] & 0xF0) >> 4) * 16 + (temp[i] & 0x0F);
            }
            return ret;
        }

        /// <summary>
        /// 按无符号BCD码计算校验和
        /// </summary>
        /// <param name="arr">字节数组</param>
        /// <param name="offset">计算起始位置</param>
        /// <param name="count">计算长度</param>
        /// <returns>校验和</returns>
        public static byte BcdCheckSum(List<byte> arr, int offset, int count)
        {
            Debug.Assert(arr != null && offset >= 0 && count >= 0 && offset + count <= arr.Count);
            int ret = 0;
            if (count == 0) count = arr.Count - offset;
            for (int i = offset; i < offset + count; i++) ret += (int)DecodeUnsignedBcd(arr, i, 1, false);
            return EncodeUnsignedBcd(ret % 100, false)[0];
        }

        /// <summary>
        /// 将字节流转换成可供显示的16进制字符串
        /// </summary>
        /// <param name="arr">字节流</param>
        /// <param name="offset">转换起始位置</param>
        /// <param name="count">转换长度</param>
        /// <returns>16进制字符串</returns>
        public static string ByteArrToHex(byte[] arr, int offset, int count)
        {
            Debug.Assert(arr != null && offset >= 0 && count > 0 && offset + count <= arr.Length);
            StringBuilder bdr = new StringBuilder(count * 3);
            for (int i = offset; i < offset + count; i++)
            {
                bdr.Append(Convert.ToString(arr[i], 16).PadLeft(2, '0').ToUpper());
                bdr.Append(' ');
            }
            return bdr.ToString();
        }

        /// <summary>
        /// 判断一个浮点数是否整数的倒数
        /// </summary>
        /// <param name="x">待判断的浮点数</param>
        /// <param name="reciprocal">倒数</param>
        /// <returns>true表示x是一个整数的倒数，false表示不是</returns>
        public static bool IsReciprocalInteger(float x, out int reciprocal)
        {
            const float THRESHOLD = .001f;
            float temp = 1 / x;
            if (x >= 0) reciprocal = (int)(temp + .5f);
            else reciprocal = (int)(temp - .5f);
            return Math.Abs(temp - reciprocal) <= THRESHOLD;
        }

        /// <summary>
        /// 转换串口校验
        /// </summary>
        /// <param name="parity">数据库中的校验方式代码</param>
        /// <returns>.net定义的校验方式</returns>
        public static System.IO.Ports.Parity TranslateParity(string parity)
        {
            Debug.Assert(parity != null);
            System.IO.Ports.Parity ret = System.IO.Ports.Parity.None;
            switch (parity)
            {
                case CommPortParity.None:
                    ret = System.IO.Ports.Parity.None;
                    break;
                case CommPortParity.Odd:
                    ret = System.IO.Ports.Parity.Odd;
                    break;
                case CommPortParity.Even:
                    ret = System.IO.Ports.Parity.Even;
                    break;
                case CommPortParity.Mark:
                    ret = System.IO.Ports.Parity.Mark;
                    break;
                case CommPortParity.Space:
                    ret = System.IO.Ports.Parity.Space;
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
            return ret;
        }

        /// <summary>
        /// 转换串口停止位
        /// </summary>
        /// <param name="parity">数据库中的停止位</param>
        /// <returns>.net定义的停止位</returns>
        public static System.IO.Ports.StopBits TranslateStopBits(int stopBits)
        {
            System.IO.Ports.StopBits ret = System.IO.Ports.StopBits.One;
            switch (stopBits)
            {
                case 1:
                    ret = System.IO.Ports.StopBits.One;
                    break;
                case 2:
                    ret = System.IO.Ports.StopBits.Two;
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
            return ret;
        }

        /// <summary>
        /// 将字符串转换为byte
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<byte> StringToByte(string str)
        {
            List<byte> ret = new List<byte>();
            foreach (char ch in str)
            {
                ret.Add(Convert.ToByte(ch));
            }
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="Upper">true:高至低 false:低至高</param>
        /// <returns></returns>
        public static byte[] StringToByte(string value, bool Upper)
        {
            List<byte> lstData = new List<byte>();
            try
            {
                for (int i = 0; i < (value.Length / 2); i++)
                {
                    lstData.Add(Convert.ToByte(string.Format("0x{0}", value.Substring(i * 2, 2)), 16));
                }

                if (!Upper)
                {
                    lstData.Reverse();
                }
            }
            catch
            {
                // to do
            }

            return lstData.ToArray();
        }

        /// <summary>
        /// 将List对象转换成字符串
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ByteListToString(List<byte> list)
        {
            string ret = string.Empty;
            if (list != null && list.Count > 0)
            {
                ret = Encoding.ASCII.GetString(list.ToArray());
            }
            return ret;
        }

        /// <summary>
        /// 格式化数值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="digit">精度位数（包含整数与小数部分总位数，不含小数点）</param>
        /// <returns></returns>
        public static string FormatPrecision(decimal value, int precision)
        {
            string ret = value.ToString();
            if (ret.Contains(".") == false) ret += ".";

            int decPoint = ret.IndexOf(".");    // 小数点位置 = 整数部分长度 
            if (ret.StartsWith("-"))
            {
                decPoint -= 1;
                precision -= 1;
            }
            if (decPoint >= precision)
                ret = value.ToString("0");
            else
            {
                ret = FormatDigit(value, precision - decPoint);
            }
            return ret;
        }

        /// <summary>
        /// 格式化数值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="digit">小数位</param>
        /// <returns></returns>
        public static string FormatDigit(decimal value, int digit)
        {
            if (digit > 0)
            {
                return value.ToString("F" + digit.ToString());
            }
            else
                return value.ToString("0");
        }

        /// <summary>
        /// 多线程同步等待
        /// </summary>
        /// <param name="ar"></param>
        public static void SyncWait(IAsyncResult ar)
        {
            try
            {
                while (ar.IsCompleted == false) System.Threading.Thread.Sleep(100);
            }
            catch { }
        }

        /// <summary>
        /// 数据修约
        /// </summary>
        /// <param name="dec"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string CheckFormatModify(decimal basicDec, decimal data)
        {
            if (decimal.Floor(basicDec) == basicDec) return decimal.Floor(data).ToString();//无小数位数，返回
            if (basicDec == 0) basicDec = new decimal(0.00001);
            string basic = basicDec.ToString().TrimEnd('0');

            string result = string.Empty;
            //数据修约
            decimal y = decimal.Parse(basic.Substring(basic.Length - 1));

            //除于数据修约位
            decimal dataDiv = data / y;

            //按照1间隔修约

            //小数部分
            int basicDecimal = basic.ToString().Length - basic.ToString().IndexOf(".") - 1;
            int dataDecimal = dataDiv.ToString().Length - dataDiv.ToString().IndexOf(".") - 1;
            //整数部分
            int basicInteger = basic.ToString().IndexOf(".");
            int dataInteger = dataDiv.ToString().IndexOf(".");

            if (dataDecimal > basicDecimal)
            {
                decimal compareData = decimal.Parse(dataDiv.ToString().Substring(0, basicDecimal + basicInteger) + "5");

                if (dataDiv < compareData)
                {
                    //舍去
                    result = CheckHelper.FormatDigit(dataDiv, basicDecimal);
                }
                else if (dataDiv > compareData)
                {
                    //进1
                    decimal basicFormat = decimal.Parse(CheckHelper.FormatDigit(dataDiv, basicDecimal));
                    decimal add = decimal.Parse(Math.Pow(0.1, (double)basicDecimal).ToString("F9").TrimEnd('0'));
                    result = (basicFormat + add).ToString();
                }
                else
                {
                    //保留位为奇数吗，进1
                    if (decimal.Parse(dataDiv.ToString().Substring(basicDecimal + basicInteger)) / 2 == 1)
                    {
                        decimal basicFormat = decimal.Parse(CheckHelper.FormatDigit(dataDiv, basicDecimal));
                        decimal add = decimal.Parse(Math.Pow(0.1, (double)basicDecimal).ToString("F9").TrimEnd('0'));
                        result = (basicFormat + add).ToString();
                    }
                    //保留位为偶数，舍去
                    else if (decimal.Parse(dataDiv.ToString().Substring(basicDecimal + basicInteger)) / 2 == 0)
                    {
                        result = CheckHelper.FormatDigit(dataDiv, basicDecimal);
                    }
                }
            }

            //乘于数据修约位
            if (!string.IsNullOrEmpty(result))
            {
                result = (decimal.Parse(result) * y).ToString();
            }
            else
            {
                result = data.ToString("F" + basicDecimal.ToString());
            }

            return result;
        }

        /// <summary>
        /// 浮点数转成IEEE754表示法（4个字节）
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<byte> FloatToIEEE754(double value)
        {
            Int64 base16 = 0x00000000;
            int m_s = 0; //数符
            int m_e = 0; //介码
            double m_x = 0; //小数部分
            double absFloatValue = Math.Abs(value);
            m_s = value < 0 ? 1 : 0;
            base16 += Convert.ToInt64(m_s * Math.Pow(2, 31)); //首位左移31位到31位处
            m_e = (int)Math.Log(absFloatValue, 2) + 127;
            m_x = absFloatValue / Math.Pow(2, m_e - 127) - 1;
            base16 += Convert.ToInt64(m_e * Math.Pow(2, 23));
            m_x = m_x * Math.Pow(2, 23);
            base16 += Convert.ToInt64(m_x);
            List<byte> datas = new List<byte>();
            for (int i = 3; i >= 0; i--)
            {
                datas.Add((byte)(base16 / (Math.Pow(256, i))));
            }
            return datas;
        }

        /// <summary>
        /// IEEE754（4个字节）表示法转成浮点数
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static double IEEE754ToFloat(byte[] values)
        {
            if (values.Length != 4)
            {
                throw new Exception("数据长度存在问题!");
            }

            string strBase16 = string.Empty;
            foreach (byte data in values)
            {
                strBase16 += data;
            }
            string strTemp = " ";
            double temp = 0;
            int m_s = 0;  //   数符 
            int m_e = 0;  //   阶 
            double m_x = 0; //   小数部分 
            double m_re = 0; //   计算结果 
            strTemp = strBase16.Substring(0, 2);
            temp = Convert.ToInt32(strTemp, 16) & 0x80;
            if (temp == 128) m_s = 1;
            strTemp = strBase16.Substring(0, 3);
            temp = Convert.ToInt32(strTemp, 16) & 0x7f8;
            m_e = Convert.ToInt32(temp / Math.Pow(2, 3));
            strTemp = strBase16.Substring(2, 6);
            temp = Convert.ToInt32(strTemp, 16) & 0x7fffff;
            m_x = temp / Math.Pow(2, 23);
            m_re = Math.Pow(-1, m_s) * (1 + m_x) * Math.Pow(2, m_e - 127);
            return m_re;
        }

        /// 返回在字符串中指定位置中第几项
        /// <summary>
        /// 返回在字符串中指定索引值,GetItemNo
        /// </summary>
        /// <param name="msg">取指定项的源值</param>
        /// <param name="split">分隔符</param>
        /// <param name="item">item,需要找出的项目</param>
        /// <example> GetItem("1A,5A,10A,20A",",",5A) = 1</example>
        /// <returns></returns>
        public static int GetItem(string msg, string split, string item)
        {
            //'取指定项的序号(0..N),找不到返回-1
            int Splitlen = 0;
            int S = 0;
            int N = 0;
            int Count = 0;
            int returnItemNo = -1;
            Splitlen = split.Length;
            if (string.IsNullOrEmpty(msg))
                return -1;
            if (Splitlen > 0)
            {
                S = 0;
                do
                {
                    N = msg.IndexOf(split, S);
                    if (N == -1)
                    {
                        if (msg.Substring(S) == item)
                            returnItemNo = Count;
                        break;
                    }
                    else
                    {
                        if (msg.Substring(S, N - S).Trim() == item)
                        {
                            returnItemNo = Count;
                            break;
                        }
                        S = N + Splitlen;
                        Count += 1;
                    }
                } while (N != -1);
            }
            return returnItemNo;
        }

        /// 返回在字符串中指定索引值
        /// <summary>
        /// 返回在字符串中指定索引值
        /// </summary>
        /// <param name="msg">取指定项的源值</param>
        /// <param name="split">分隔符</param>
        /// <param name="gindex">第几项，若=-1时候，则为所有</param>
        /// <example> GetItem("1A,5A,10A,20A",",",2) = "10A"</example>
        /// <returns></returns>
        public static object GetItem(string msg, string split, int gindex)
        {
            int splitLen = -1;
            int s = 0;
            int n = 0;
            int count = 0;
            string item = "";
            //return -1;
            if (string.IsNullOrEmpty(msg))
                return -1;
            splitLen = split.Length;
            try
            {
                if ((msg.Length) * splitLen > 0)
                {
                    s = 0;
                    if (gindex < 0)
                    {
                        do
                        {
                            n = msg.IndexOf(split, s);
                            count += 1;
                            if (n > 0)
                            {
                                s = n + splitLen;
                            }
                        } while (n != -1);
                        return count;
                    }
                    else
                    {
                        do
                        {
                            n = msg.IndexOf(split, s);
                            //if (n != -1)
                            //    count += 1;
                            if (count == gindex)
                            {
                                if (n != -1)
                                {
                                    item = msg.Substring(s, (n - s));
                                }
                                else
                                {
                                    item = msg.Substring(s);
                                }
                                break;
                            }
                            else
                            {
                                count += 1;
                                if (n > 0)
                                    s = n + splitLen;
                                continue;
                            }
                        } while (n != -1);
                        return item;
                    }
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                //不做任何处理
                return "";
            }
        }

        /// <summary>
        /// 选择数组第一次批配字符后的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Select<T>(ref T[] datas, T value)
        {
            int index = Array.IndexOf(datas, value);

            if (index == -1)
            {
                return false;
            }

            List<T> listData = datas.ToList();
            listData.RemoveRange(0, index);
            datas = listData.ToArray();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Double FormatStringToDouble(string value)
        {
            double returnValue = 0;
            try
            {
                //将字符串中的第一个匹配数字过滤出来
                string regularString = @"-?\d+(\.\d+)?";
                string temp = System.Text.RegularExpressions.Regex.Match(value, regularString).ToString();
                if (temp.Length > 0)
                {
                    returnValue = Convert.ToDouble(temp);
                }
            }
            catch (Exception)
            {
            }
            return returnValue;
        }

        public static List<byte> TransDataToBytes(bool drift)
        {
            return TransDataToBytes("00000000", drift);
        }

        /// <summary>
        /// 将Uint数据转换为BYTE数组
        /// </summary>
        /// <param name="value">待转值</param>
        /// <param name="drift">true 低至高 false 高至低</param>
        /// <returns></returns>
        public static List<byte> TransDataToBytes(string value, bool drift)
        {
            if (string.IsNullOrEmpty(value))
            {
                value = string.Empty;
            }

            value = value.PadLeft(8, '0');
            uint data = Convert.ToUInt32(value, 16);
            List<byte> lstData = new List<byte>();
            for (int i = 0; i < 4; i++)
            {
                lstData.Add((byte)(data >> (i * 8)));
            }

            if (!drift)
            {
                lstData.Reverse();
            }
            return lstData;
        }
    }
}
