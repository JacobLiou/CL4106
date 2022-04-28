using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pwClassLibrary
{
    /// <summary>
    /// 字符串相关处理
    /// </summary>
    public class Sstring
    {

        /// <summary>
        /// 反转字符串
        /// </summary>
        /// <param name="sData"></param>
        /// <returns></returns>
        public static string BackString(string sData)
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


        /// <summary>
        /// ASICC码串转成字符串，单个字节转成单个字符,读取时用
        /// </summary>
        /// <param name="sData">ASICC码串</param>
        /// <returns></returns>
        public static string ASCIIEncodingToString(string sData)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            string StrData = "";
            for (int j = 0; j < sData.Length / 2; j++)
            {
                byte[] byteArray = new byte[] { Convert.ToByte(sData.Substring(2 * j, 2), 16) };
                StrData += asciiEncoding.GetString(byteArray);
            }
            StrData = StrData.Replace("\0", "");
            return StrData;

        }

        /// <summary>
        /// 字符串转成ASCII码下发串，写表时用
        /// </summary>
        /// <param name="sData"></param>
        /// <returns></returns>
        public static string StringToASCIIEncoding(string sDataInput)
        {

            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();

            byte[] ASCIIStringToByte = asciiEncoding.GetBytes(sDataInput);

            string StringOut = "";

            foreach (byte InByte in ASCIIStringToByte)
            {
                StringOut = StringOut + String.Format("{0:X2} ", InByte);
            }


            StringOut = StringOut.Replace(" ", "");

            StringOut = BackString(StringOut);

            return StringOut;


        }

    }
}
