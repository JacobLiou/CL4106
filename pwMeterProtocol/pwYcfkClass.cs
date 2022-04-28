using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace pwMeterProtocol
{
    public class pwYcfkClass2
    {
        public static string sMeterSerial = "";       //分散因子
        public static string sRnad1 = "";             //随机数1
        public static string sRnad2 = "";             //随机数2
        public static string sESAMSerial = "";        //ESAM序列号
        public static bool bLogin = false;            //身份证认状态

        public static string sRnad1_WoQi = "";         //随机数1,握奇使用

        public pwYcfkClass2()
        {
            sMeterSerial = "0000000000000001";
        }

        #region 身份认证函数
        [DllImport("TestZhuzhan.dll")]
        private static extern byte IdentityAuthentication(string Div, byte[] RandAndEndata);
        /// <summary>
        /// 1．身份认证函数:函数功能 身份认证取随机数和密文
        /// </summary>
        /// <param name="Div">输入参数，8 字节分散因子，16 进制字符串。</param>
        /// <param name="RandAndEndata">输出参数，字符型，8 字节随机数+8 字节密文。</param>
        /// <returns></returns>
        public static byte pwIdentityAuthentication(string Div, ref string RandAndEndata, ref string str_LostMessage)
        {
            byte bReturn = 0;
            byte[] tRand = new byte[32];

            bReturn = IdentityAuthentication(Div, tRand);
            //RandAndEndata = Encoding.ASCII.GetString(tRand);
            RandAndEndata = Encoding.ASCII.GetString(tRand);
            #region
            switch (bReturn)
            {
                case 0:
                    str_LostMessage = "成功标志";
                    break;
                case 200:
                    str_LostMessage = "连接加密机失败";
                    break;
                case 201:
                    str_LostMessage = "取随机数1 失败";
                    break;
                case 202:
                    str_LostMessage = "取随机数2 失败";
                    break;
                case 203:
                    str_LostMessage = "密钥分散失败";
                    break;
                case 204:
                    str_LostMessage = "数据加密失败";
                    break;
                case 205:
                    str_LostMessage = "取密文失败";
                    break;
                default:
                    str_LostMessage = "失败，未知错误类型";
                    break;
            }
            #endregion
            return bReturn;
        }
        #endregion


        #region 远程控制
        [DllImport("TestZhuzhan.dll")]
        private static extern byte UserControl(string RandDivEsamNumData, byte[] dataOut);
        /// <summary>
        /// 2．远程控制函数
        /// </summary>
        /// <param name="RandDivEsamNumData">输入参数，字符型，4 字节随机数+8 字节分散因子+8 字节ESAM 序列号+数据明文。</param>
        /// <param name="dataOut">字符型，20 字节密文</param>
        /// <returns></returns>
        public static byte pwUserControl(string RandDivEsamNumData, ref string dataOut, ref string str_LostMessage)
        {
            byte bReturn = 0;
            byte[] tRand = new byte[40];

            bReturn = UserControl(RandDivEsamNumData, tRand);
            dataOut = Encoding.ASCII.GetString(tRand);
            #region
            switch (bReturn)
            {
                case 0:
                    str_LostMessage = "成功标志";
                    break;
                case 200:
                    str_LostMessage = "连接加密机失败";
                    break;
                case 201:
                    str_LostMessage = "写卡失败";
                    break;
                case 202:
                    str_LostMessage = "读卡失败";
                    break;
                case 203:
                    str_LostMessage = "计算密文失败";
                    break;
                default:
                    str_LostMessage = "失败，未知错误类型";
                    break;
            }
            #endregion
            return bReturn;

        }

        #endregion


        #region 开户充值

        [DllImport("TestZhuzhan.dll")]
        private static extern byte InCreasePurse(string RandDivData, byte[] dataout);
        /// <summary>
        /// 3．充值函数
        /// </summary>
        /// <param name="RandDivData">4 字节随机数；（字符型）8 字节分散因子；电量和次数，8 字节；首次充值时，6 字节户号。</param>
        /// <param name="dataout">返回电量，次数和4 字节MAC。</param>
        /// <returns></returns>
        public static byte pwInCreasePurse(string RandDivData, ref string dataOut, ref string str_LostMessage)
        {
            byte bReturn = 0;
            byte[] tRand = new byte[24];

            bReturn = InCreasePurse(RandDivData, tRand);
            dataOut = Encoding.ASCII.GetString(tRand);
            #region
            switch (bReturn)
            {
                case 0:
                    str_LostMessage = "成功标志";
                    break;
                case 200:
                    str_LostMessage = "连接加密机失败";
                    break;
                case 201:
                    str_LostMessage = "写卡失败";
                    break;
                case 202:
                    str_LostMessage = "读卡失败";
                    break;
                case 203:
                    str_LostMessage = "计算MAC 失败";
                    break;
                default:
                    str_LostMessage = "失败，未知错误类型";
                    break;
            }
            #endregion
            return bReturn;

        }

        #endregion


        #region 校验MAC
        [DllImport("TestZhuzhan.dll")]
        private static extern byte Maccheck(string RandDivData, byte[] dataout);
        /// <summary>
        /// 7．校验MAC 函数
        /// </summary>
        /// <param name="RandDivData">输入参数,4 字节随机数+8 字节分散因子+5 字节指令(04d68600+LC)+数据明文+4 字节MAC。LC=明文长度+0x0C； </param>
        /// <param name="dataout">空</param>
        /// <returns></returns>
        public static byte pwMaccheck(string RandDivData, ref string dataOut, ref string str_LostMessage)
        {
            byte bReturn = 0;
            byte[] tRand = new byte[1];

            bReturn = Maccheck(RandDivData, tRand);
            dataOut = Encoding.ASCII.GetString(tRand);
            #region
            switch (bReturn)
            {
                case 0:
                    str_LostMessage = "成功标志";
                    break;
                default:
                    str_LostMessage = "MAC错误";
                    break;
            }
            #endregion
            return bReturn;

        }
        #endregion


        #region 密钥更新-----返回　密文  //byte-->int
        [DllImport("TestZhuzhan.dll")]
        private static extern int KeyUpdate(int kid, string DivEsamNumRandData, byte[] dataOut);  //byte-->int
        /// <summary>
        /// 6．密钥更新函数
        /// </summary>
        /// <param name="kid">kid=1 ，身份认证密钥；kid=2, 远程控制密钥；Kid=3, 参数更新密钥。</param>
        /// <param name="DivEsamNumRandData">输入参数，字符型，8 字节分散因子+8 字节ESAM 序列号+4 字节随机数+4 字节数据明文。</param>
        /// <param name="dataOut">返回32 字节密文+ 4 字节密钥信息+4 字节MAC。</param>
        /// <returns></returns>
        public static int pwKeyUpdate(int kid, string DivEsamNumRandData, ref string dataOut, ref string str_LostMessage)
        {
            int bReturn = 0;
            byte[] tRand = new byte[80];
            bReturn = KeyUpdate(kid, DivEsamNumRandData, tRand);
            dataOut = Encoding.ASCII.GetString(tRand);
            #region
            switch (bReturn)
            {
                case 0:
                    str_LostMessage = "成功标志";
                    break;
                case 200:
                    str_LostMessage = "连接加密机失败";
                    break;
                case 201:
                    str_LostMessage = "写卡失败";
                    break;
                case 202:
                    str_LostMessage = "读卡失败";
                    break;
                case 203:
                    str_LostMessage = "计算MAC 失败";
                    break;
                default:
                    str_LostMessage = "失败，未知错误类型";
                    break;
            }
            #endregion
            return bReturn;

        }
        #endregion


        #region 一类参数更新-----返回 明文＋MAC

        [DllImport("TestZhuzhan.dll")]
        private static extern byte ParameterUpdate(string RandDivApduData, byte[] dataout);
        /// <summary>
        /// 4．参数更新函数
        /// </summary>
        /// <param name="RandDivApduData">4 字节随机数；（字符型）8 字节分散因子；更新指令10 位(04d682+起始+LC)； LC=明文数据长度+4。其他为参数明文。</param>
        /// <param name="dataout">返回参数明文和MAC。</param>
        /// <returns></returns>
        public static byte pwParameterUpdate(string RandDivApduData, ref string dataOut, ref string str_LostMessage)
        {
            byte bLen = Convert.ToByte(RandDivApduData.Length / 2);
            byte bReturn = 0;
            byte bLen2 = Convert.ToByte((bLen - 17 + 4) * 2);//变长bLen-17 + 4 
            byte[] tRand = new byte[bLen2];

            bReturn = ParameterUpdate(RandDivApduData, tRand);
            dataOut = Encoding.UTF8.GetString(tRand);

            //char chrEnd = '\0';
            //dataOut = dataOut.Substring(0, dataOut.IndexOf(chrEnd));

            #region
            switch (bReturn)
            {
                case 0:
                    str_LostMessage = "成功标志";
                    break;
                case 200:
                    str_LostMessage = "连接加密机失败";
                    break;
                case 201:
                    str_LostMessage = "写卡失败";
                    break;
                case 202:
                    str_LostMessage = "读卡失败";
                    break;
                case 203:
                    str_LostMessage = "计算MAC 失败";
                    break;
                default:
                    str_LostMessage = "失败，未知错误类型";
                    break;
            }
            #endregion
            return bReturn;

        }



        [DllImport("TestZhuzhan.dll")]
        private static extern byte Parameter1(string RandDivApduData, byte[] dataout);
        /// <summary>
        /// 8．费率文件1 更新函数
        /// </summary>
        /// <param name="RandDivApduData">随机数8 位；（字符型）分散因子16 位；更新指令10 位(04d683+起始+LC)； LC=明文数据长度+4。其他为参数明文。</param>
        /// <param name="dataout">返回参数明文和MAC。</param>
        /// <returns></returns>
        public static byte pwParameter1(string RandDivApduData, ref string dataOut, ref string str_LostMessage)
        {
            byte bLen = Convert.ToByte(RandDivApduData.Length / 2);
            byte bLen2 = Convert.ToByte((bLen - 17 + 4) * 2);//变长bLen-17 + 4 ==密文长度＋4MAC
            byte[] tRand = new byte[bLen2];
            byte bReturn = 0;

            bReturn = Parameter1(RandDivApduData, tRand);
            dataOut = Encoding.ASCII.GetString(tRand);
            #region
            switch (bReturn)
            {
                case 0:
                    str_LostMessage = "成功标志";
                    break;
                case 200:
                    str_LostMessage = "连接加密机失败";
                    break;
                case 201:
                    str_LostMessage = "写卡失败";
                    break;
                case 202:
                    str_LostMessage = "读卡失败";
                    break;
                case 203:
                    str_LostMessage = "计算MAC 失败";
                    break;
                default:
                    str_LostMessage = "失败，未知错误类型";
                    break;
            }
            #endregion
            return bReturn;

        }



        [DllImport("TestZhuzhan.dll")]
        private static extern byte Parameter2(string RandDivApduData, byte[] dataout);
        /// <summary>
        /// 9．费率文件2 更新函数
        /// </summary>
        /// <param name="RandDivApduData">随机数8 位；（字符型）分散因子16 位；更新指令10 位(04d684+起始+LC)； LC=明文数据长度+4。其他为参数明文。</param>
        /// <param name="dataout">返回参数明文和MAC。</param>
        /// <returns></returns>
        public static byte pwParameter2(string RandDivApduData, ref string dataOut, ref string str_LostMessage)
        {
            byte bLen = Convert.ToByte(RandDivApduData.Length / 2);
            byte bLen2 = Convert.ToByte((bLen - 17 + 4) * 2);//变长bLen-17 + 4 ==密文长度＋4MAC
            byte[] tRand = new byte[bLen2];
            byte bReturn = 0;

            bReturn = Parameter2(RandDivApduData, tRand);
            dataOut = Encoding.ASCII.GetString(tRand);
            #region
            switch (bReturn)
            {
                case 0:
                    str_LostMessage = "成功标志";
                    break;
                case 200:
                    str_LostMessage = "连接加密机失败";
                    break;
                case 201:
                    str_LostMessage = "写卡失败";
                    break;
                case 202:
                    str_LostMessage = "读卡失败";
                    break;
                case 203:
                    str_LostMessage = "计算MAC 失败";
                    break;
                default:
                    str_LostMessage = "失败，未知错误类型";
                    break;
            }
            #endregion
            return bReturn;

        }

        #endregion


        #region 二类参数更新　-----返回  密文＋MAC

        [DllImport("TestZhuzhan.dll")]
        private static extern byte ParameterElseUpdate(string RandDivApduData, string EsamNum, byte[] dataout);
        /// <summary>
        /// 5．密文+MAC 参数更新函数
        /// </summary>
        /// <param name="RandDivApduData">4 字节随机数；（字符型）8 字节分散因子；更新指令10 位(04d6+文件标识+00+LC)； （此处LC 长度为下发密文数据+MAC 的长度）其他为参数明文。</param>
        /// <param name="EsamNum">输入参数，8 字节ESAM 序列号。</param>
        /// <param name="dataout">返回参数密文和MAC。</param>
        /// <returns></returns>
        public static byte pwParameterElseUpdate(string RandDivApduData, string EsamNum, ref string dataOut, ref string str_LostMessage)
        {
            byte bLen = Convert.ToByte(RandDivApduData.Length / 2);
            byte bLen2 = Convert.ToByte((bLen - 17 + 4 + 8) * 2);//变长bLen-17 + 4 ==密文长度＋4MAC
            byte[] tRand = new byte[255];//bLen2
            byte bReturn = 0;

            bReturn = ParameterElseUpdate(RandDivApduData, EsamNum, tRand);
            dataOut = ASCIIEncoding.UTF8.GetString(tRand);
            char chrEnd = '\0';
            dataOut = dataOut.Substring(0, dataOut.IndexOf(chrEnd));
            #region
            switch (bReturn)
            {
                case 0:
                    str_LostMessage = "成功标志";
                    break;
                case 200:
                    str_LostMessage = "连接加密机失败";
                    break;
                case 201:
                    str_LostMessage = "写卡失败";
                    break;
                case 202:
                    str_LostMessage = "读卡失败";
                    break;
                default:
                    str_LostMessage = "失败，未知错误类型";
                    break;
            }
            #endregion
            return bReturn;

        }

        #endregion


    }

    public class pwYcfkClass
    {
        public  string sMeterSerial = "";       //分散因子
        public  string sRnad1 = "";             //随机数1
        public  string sRnad2 = "";             //随机数2
        public  string sESAMSerial = "";        //ESAM序列号
        public  bool bLogin = false;            //身份证认状态

        public  string sRnad1_WoQi = "";         //随机数1,握奇使用

        public pwYcfkClass()
        {
            sMeterSerial = "0000000000000001";
        }

        #region 身份认证函数
        [DllImport("TestZhuzhan.dll")]
        private static extern byte IdentityAuthentication(string Div, byte[] RandAndEndata);
        /// <summary>
        /// 1．身份认证函数:函数功能 身份认证取随机数和密文
        /// </summary>
        /// <param name="Div">输入参数，8 字节分散因子，16 进制字符串。</param>
        /// <param name="RandAndEndata">输出参数，字符型，8 字节随机数+8 字节密文。</param>
        /// <returns></returns>
        public  byte pwIdentityAuthentication(string Div, ref string RandAndEndata, ref string str_LostMessage)
        {
            byte bReturn = 0;
            byte[] tRand = new byte[32];

            bReturn = IdentityAuthentication(Div, tRand);
            //RandAndEndata = Encoding.ASCII.GetString(tRand);
            RandAndEndata = Encoding.ASCII.GetString(tRand);
            #region
            switch (bReturn)
            {
                case 0:
                    str_LostMessage = "成功标志";
                    break;
                case 200:
                    str_LostMessage = "连接加密机失败";
                    break;
                case 201:
                    str_LostMessage = "取随机数1 失败";
                    break;
                case 202:
                    str_LostMessage = "取随机数2 失败";
                    break;
                case 203:
                    str_LostMessage = "密钥分散失败";
                    break;
                case 204:
                    str_LostMessage = "数据加密失败";
                    break;
                case 205:
                    str_LostMessage = "取密文失败";
                    break;
                default:
                    str_LostMessage = "失败，未知错误类型";
                    break;
            }
            #endregion
            return bReturn;
        }
        #endregion


        #region 远程控制
        [DllImport("TestZhuzhan.dll")]
        private static extern byte UserControl(string RandDivEsamNumData, byte[] dataOut);
        /// <summary>
        /// 2．远程控制函数
        /// </summary>
        /// <param name="RandDivEsamNumData">输入参数，字符型，4 字节随机数+8 字节分散因子+8 字节ESAM 序列号+数据明文。</param>
        /// <param name="dataOut">字符型，20 字节密文</param>
        /// <returns></returns>
        public  byte pwUserControl(string RandDivEsamNumData, ref string dataOut, ref string str_LostMessage)
        {
            byte bReturn = 0;
            byte[] tRand = new byte[40];

            bReturn = UserControl(RandDivEsamNumData, tRand);
            dataOut = Encoding.ASCII.GetString(tRand);
            #region
            switch (bReturn)
            {
                case 0:
                    str_LostMessage = "成功标志";
                    break;
                case 200:
                    str_LostMessage = "连接加密机失败";
                    break;
                case 201:
                    str_LostMessage = "写卡失败";
                    break;
                case 202:
                    str_LostMessage = "读卡失败";
                    break;
                case 203:
                    str_LostMessage = "计算密文失败";
                    break;
                default:
                    str_LostMessage = "失败，未知错误类型";
                    break;
            }
            #endregion
            return bReturn;

        }

        #endregion


        #region 开户充值

        [DllImport("TestZhuzhan.dll")]
        private static extern byte InCreasePurse(string RandDivData, byte[] dataout);
        /// <summary>
        /// 3．充值函数
        /// </summary>
        /// <param name="RandDivData">4 字节随机数；（字符型）8 字节分散因子；电量和次数，8 字节；首次充值时，6 字节户号。</param>
        /// <param name="dataout">返回电量，次数和4 字节MAC。</param>
        /// <returns></returns>
        public  byte pwInCreasePurse(string RandDivData, ref string dataOut, ref string str_LostMessage)
        {
            byte bReturn = 0;
            byte[] tRand = new byte[24];

            bReturn = InCreasePurse(RandDivData, tRand);
            dataOut = Encoding.ASCII.GetString(tRand);
            #region
            switch (bReturn)
            {
                case 0:
                    str_LostMessage = "成功标志";
                    break;
                case 200:
                    str_LostMessage = "连接加密机失败";
                    break;
                case 201:
                    str_LostMessage = "写卡失败";
                    break;
                case 202:
                    str_LostMessage = "读卡失败";
                    break;
                case 203:
                    str_LostMessage = "计算MAC 失败";
                    break;
                default:
                    str_LostMessage = "失败，未知错误类型";
                    break;
            }
            #endregion
            return bReturn;

        }

        #endregion


        #region 校验MAC  
        [DllImport("TestZhuzhan.dll")]
        private static extern byte Maccheck(string RandDivData, byte[] dataout);
        /// <summary>
        /// 7．校验MAC 函数
        /// </summary>
        /// <param name="RandDivData">输入参数,4 字节随机数+8 字节分散因子+5 字节指令(04d68600+LC)+数据明文+4 字节MAC。LC=明文长度+0x0C； </param>
        /// <param name="dataout">空</param>
        /// <returns></returns>
        public  byte pwMaccheck(string RandDivData, ref string dataOut, ref string str_LostMessage)
        {
            byte bReturn = 0;
            byte[] tRand = new byte[1];

            bReturn = Maccheck(RandDivData, tRand);
            dataOut = Encoding.ASCII.GetString(tRand);
            #region
            switch (bReturn)
            {
                case 0:
                    str_LostMessage = "成功标志";
                    break;
                default:
                    str_LostMessage = "MAC错误";
                    break;
            }
            #endregion
            return bReturn;

        }
        #endregion


        #region 密钥更新-----返回　密文  //byte-->int
        [DllImport("TestZhuzhan.dll")]
        private static extern int KeyUpdate(int kid, string DivEsamNumRandData, byte[] dataOut);  //byte-->int
        /// <summary>
        /// 6．密钥更新函数
        /// </summary>
        /// <param name="kid">kid=1 ，身份认证密钥；kid=2, 远程控制密钥；Kid=3, 参数更新密钥。</param>
        /// <param name="DivEsamNumRandData">输入参数，字符型，8 字节分散因子+8 字节ESAM 序列号+4 字节随机数+4 字节数据明文。</param>
        /// <param name="dataOut">返回32 字节密文+ 4 字节密钥信息+4 字节MAC。</param>
        /// <returns></returns>
        public  int pwKeyUpdate(int kid, string DivEsamNumRandData, ref string dataOut, ref string str_LostMessage)
        {
            int bReturn = 0;
            byte[] tRand = new byte[80];
            bReturn = KeyUpdate(kid, DivEsamNumRandData,tRand);
            dataOut = Encoding.ASCII.GetString(tRand);
            #region
            switch (bReturn)
            {
                case 0:
                    str_LostMessage = "成功标志";
                    break;
                case 200:
                    str_LostMessage = "连接加密机失败";
                    break;
                case 201:
                    str_LostMessage = "写卡失败";
                    break;
                case 202:
                    str_LostMessage = "读卡失败";
                    break;
                case 203:
                    str_LostMessage = "计算MAC 失败";
                    break;
                default:
                    str_LostMessage = "失败，未知错误类型";
                    break;
            }
            #endregion
            return bReturn;

        }
        #endregion


        #region 一类参数更新-----返回 明文＋MAC

        [DllImport("TestZhuzhan.dll")]
        private static extern byte ParameterUpdate(string RandDivApduData, byte[] dataout);
        /// <summary>
        /// 4．参数更新函数
        /// </summary>
        /// <param name="RandDivApduData">4 字节随机数；（字符型）8 字节分散因子；更新指令10 位(04d682+起始+LC)； LC=明文数据长度+4。其他为参数明文。</param>
        /// <param name="dataout">返回参数明文和MAC。</param>
        /// <returns></returns>
        public  byte pwParameterUpdate(string RandDivApduData,ref string dataOut, ref string str_LostMessage)
        {
            byte bLen = Convert.ToByte(RandDivApduData.Length / 2);
            byte bReturn = 0;
            byte bLen2 =Convert.ToByte ((bLen - 17 + 4)*2);//变长bLen-17 + 4 
            byte[] tRand = new byte[bLen2];

            bReturn = ParameterUpdate(RandDivApduData, tRand);
            dataOut = Encoding.UTF8.GetString(tRand);

            //char chrEnd = '\0';
            //dataOut = dataOut.Substring(0, dataOut.IndexOf(chrEnd));

            #region
            switch (bReturn)
            {
                case 0:
                    str_LostMessage = "成功标志";
                    break;
                case 200:
                    str_LostMessage = "连接加密机失败";
                    break;
                case 201:
                    str_LostMessage = "写卡失败";
                    break;
                case 202:
                    str_LostMessage = "读卡失败";
                    break;
                case 203:
                    str_LostMessage = "计算MAC 失败";
                    break;
                default:
                    str_LostMessage = "失败，未知错误类型";
                    break;
            }
            #endregion
            return bReturn;

        }



        [DllImport("TestZhuzhan.dll")]
        private static extern byte Parameter1(string RandDivApduData, byte[] dataout);
        /// <summary>
        /// 8．费率文件1 更新函数
        /// </summary>
        /// <param name="RandDivApduData">随机数8 位；（字符型）分散因子16 位；更新指令10 位(04d683+起始+LC)； LC=明文数据长度+4。其他为参数明文。</param>
        /// <param name="dataout">返回参数明文和MAC。</param>
        /// <returns></returns>
        public  byte pwParameter1(string RandDivApduData, ref string dataOut, ref string str_LostMessage)
        {
            byte bLen = Convert.ToByte(RandDivApduData.Length / 2);
            byte bLen2 = Convert.ToByte((bLen - 17 + 4) * 2);//变长bLen-17 + 4 ==密文长度＋4MAC
            byte[] tRand = new byte[bLen2];
            byte bReturn = 0;

            bReturn = Parameter1(RandDivApduData, tRand);
            dataOut = Encoding.ASCII.GetString(tRand);
            #region
            switch (bReturn)
            {
                case 0:
                    str_LostMessage = "成功标志";
                    break;
                case 200:
                    str_LostMessage = "连接加密机失败";
                    break;
                case 201:
                    str_LostMessage = "写卡失败";
                    break;
                case 202:
                    str_LostMessage = "读卡失败";
                    break;
                case 203:
                    str_LostMessage = "计算MAC 失败";
                    break;
                default:
                    str_LostMessage = "失败，未知错误类型";
                    break;
            }
            #endregion
            return bReturn;

        }
        


        [DllImport("TestZhuzhan.dll")]
        private static extern byte Parameter2(string RandDivApduData, byte[] dataout);
        /// <summary>
        /// 9．费率文件2 更新函数
        /// </summary>
        /// <param name="RandDivApduData">随机数8 位；（字符型）分散因子16 位；更新指令10 位(04d684+起始+LC)； LC=明文数据长度+4。其他为参数明文。</param>
        /// <param name="dataout">返回参数明文和MAC。</param>
        /// <returns></returns>
        public  byte pwParameter2(string RandDivApduData, ref string dataOut, ref string str_LostMessage)
        {
            byte bLen = Convert.ToByte(RandDivApduData.Length / 2);
            byte bLen2 = Convert.ToByte((bLen - 17 + 4) * 2);//变长bLen-17 + 4 ==密文长度＋4MAC
            byte[] tRand = new byte[bLen2];
            byte bReturn = 0;

            bReturn = Parameter2(RandDivApduData, tRand);
            dataOut = Encoding.ASCII.GetString(tRand);
            #region
            switch (bReturn)
            {
                case 0:
                    str_LostMessage = "成功标志";
                    break;
                case 200:
                    str_LostMessage = "连接加密机失败";
                    break;
                case 201:
                    str_LostMessage = "写卡失败";
                    break;
                case 202:
                    str_LostMessage = "读卡失败";
                    break;
                case 203:
                    str_LostMessage = "计算MAC 失败";
                    break;
                default:
                    str_LostMessage = "失败，未知错误类型";
                    break;
            }
            #endregion
            return bReturn;

        }

        #endregion
        

        #region 二类参数更新　-----返回  密文＋MAC

        [DllImport("TestZhuzhan.dll")]
        private static extern byte ParameterElseUpdate(string RandDivApduData, string EsamNum, byte[] dataout);
        /// <summary>
        /// 5．密文+MAC 参数更新函数
        /// </summary>
        /// <param name="RandDivApduData">4 字节随机数；（字符型）8 字节分散因子；更新指令10 位(04d6+文件标识+00+LC)； （此处LC 长度为下发密文数据+MAC 的长度）其他为参数明文。</param>
        /// <param name="EsamNum">输入参数，8 字节ESAM 序列号。</param>
        /// <param name="dataout">返回参数密文和MAC。</param>
        /// <returns></returns>
        public  byte pwParameterElseUpdate(string RandDivApduData, string EsamNum, ref string dataOut, ref string str_LostMessage)
        {
            byte bLen = Convert.ToByte(RandDivApduData.Length / 2);
            byte bLen2 = Convert.ToByte((bLen - 17 + 4 + 8 ) * 2);//变长bLen-17 + 4 ==密文长度＋4MAC
            byte[] tRand = new byte[255];//bLen2
            byte bReturn = 0;

            bReturn = ParameterElseUpdate(RandDivApduData, EsamNum ,tRand);
            dataOut = ASCIIEncoding.UTF8.GetString(tRand);
            char chrEnd = '\0';
            dataOut = dataOut.Substring(0, dataOut.IndexOf(chrEnd));
            #region
            switch (bReturn)
            {
                case 0:
                    str_LostMessage = "成功标志";
                    break;
                case 200:
                    str_LostMessage = "连接加密机失败";
                    break;
                case 201:
                    str_LostMessage = "写卡失败";
                    break;
                case 202:
                    str_LostMessage = "读卡失败";
                    break;
                default:
                    str_LostMessage = "失败，未知错误类型";
                    break;
            }
            #endregion
            return bReturn;

        }

        #endregion


   }

}
