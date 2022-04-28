using System;
using System.IO.Ports;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text;

namespace Frontier.MeterVerification.DeviceCommon
{
    /// <summary>
    /// 开发套件驱动接口
    /// </summary>
    public static class gTestZhuzhan
    {
        /// 身份认证取随机数和密文 
        /// <summary>
        /// 身份认证取随机数和密文 
        /// </summary>
        /// <param name="Div">输入参数，8 字节分散因子，16 进制字符串</param>
        /// <param name="RandAndEndata">输出参数，字符型，8 字节随机数+8 字节密文。</param>
        /// <returns>
        /// 0，成功  ；                 
        /// 200，连接加密机失败；                  
        /// 201，取随机数1 失败；                  
        /// 202，取随机数2 失败；                  
        /// 203，密钥分散失败；                  
        /// 204，数据加密失败；                   
        /// 205，取密文失败； 
        /// </returns>
        [DllImport("TestZhuzhan.dll")]
        public static extern int IdentityAuthentication(string Div, ref string RandAndEndata);

        /// 远程控制
        /// <summary>
        /// 远程控制,用啦拉合闸试验。
        /// </summary>
        /// <param name="RandDivEsamNumData">输入参数，字符型，4字节随机数参数说明+8 字节分散因子+8 字节ESAM 序列号+数据明文。</param>
        /// <param name="dataOut">字符型，20 字节密文</param>
        /// <returns>
        /// 0，成功  ；                  
        /// 200，连接加密机失败；                   
        /// 201，写卡失败；                   
        /// 202，读卡失败；                   
        /// 203，计算密文失败； 
        /// </returns>
        [DllImport("TestZhuzhan.dll")]
        public static extern int UserControl(string RandDivEsamNumData, ref string dataOut);

        /// 更新参数
        /// <summary>
        /// 更新参数
        /// </summary>
        /// <param name="RandDivApduData">
        /// 4 字节随机数；（字符型8 字节分散因子；更新指令10 位(04d682+起始+LC)；
        /// LC=明文数据长度+4。 其他为参数明文
        /// </param>
        /// <param name="dataOut">返回参数明文和MAC</param>
        /// <returns>
        /// 0，成功  ； 
        /// 200，连接加密机失败；
        /// 201，写卡失败；
        /// 202，读卡失败；
        /// 203，计算MAC 失败
        /// </returns>
        [DllImport("TestZhuzhan.dll")]
        public static extern int ParameterUpdate(string RandDivApduData, ref string dataOut);

        /// 密文+MAC 更新参数
        /// <summary>
        /// 密文+MAC 更新参数
        /// </summary>
        /// <param name="RandDivApduData">4字节随机数；8 字节分散因子；更新指令10 位(04d6+文件标识+00+LC)；（此处LC 长度为下发密文数据+MAC 的长度）其他为参数明文</param>
        /// <param name="EsamNum">输入参数，8 字节ESAM 序列号。</param>
        /// <param name="dataOut">
        /// 返回参数密文和MAC.
        /// 电能表接收密文+MAC 后，用04d6+文件标识+00+LC+密文+MAC 更新ESAM 文件，然后明文读取数据，该文件第一个字节为明文数据的长度（HEX ），可以根据该长度取所解密后的明文.
        /// </param>
        /// <returns>
        ///  0，成功；                        
        ///  200，连接加密机失败；                       
        ///  201，写卡失败；                       
        ///  202，读卡失败；                        
        ///  203，计算MAC失败；
        /// </returns>
        [DllImport("TestZhuzhan.dll")]
        public static extern int ParameterElseUpdate(string RandDivApduData, ref string EsamNum, ref string dataOut);

        /// 更新密钥
        /// <summary>
        /// 更新密钥
        /// </summary>
        /// <param name="Kid">kid=1，身份认证密钥；kid=2,远程控制密钥；Kid=3,参数更新密钥。</param>
        /// <param name="DivEsamNumRandData">输入参数，字符型，8字节分散因子+8字节ESAM 序列号+4 字节随机数+4 字节数据明文。</param>
        /// <param name="dataOut">返回32 字节密文+ 4 字节密钥信息+4字节MAC。</param>
        /// <returns>
        /// 0，成功；                        
        /// 200，连接加密机失败；                       
        /// 201，写卡失败；                       
        /// 202，读卡失败；                       
        /// 203，计算MAC失败；
        /// </returns>
        [DllImport("TestZhuzhan.dll")]
        public static extern int KeyUpdate(int Kid, string DivEsamNumRandData, ref string dataOut);

        /// 费率文件1 更新函数
        /// <summary>
        /// 费率文件1 更新函数
        /// </summary>
        /// <param name="RandDivApduData">随机数8 位；分散因子16 位；更新指令10 位(04d683+起始+LC)；LC=明文数据长度+4。其他为参数明文。</param>
        /// <param name="dataOut">返回参数明文和MAC。</param>
        /// <returns>0，成功；
        ///  200，连接加密机失败；
        ///  201，写卡失败；
        ///  202，读卡失败；
        ///  203，计算MAC 失败；
        /// </returns>
        [DllImport("TestZhuzhan.dll")]
        public static extern int Parameter1(string RandDivApduData, ref string dataOut);

        /// 费率文件2 更新函数
        /// <summary>
        /// 费率文件2 更新函数
        /// </summary>
        /// <param name="RandDivApduData">随机数8 位；分散因子16 位；更新指令10 位(04d683+起始+LC)；LC=明文数据长度+4。其他为参数明文。</param>
        /// <param name="dataOut">返回参数明文和MAC。</param>
        /// <returns>0，成功；
        ///  200，连接加密机失败；
        ///  201，写卡失败；
        ///  202，读卡失败；
        ///  203，计算MAC 失败；
        /// </returns>
        [DllImport("TestZhuzhan.dll")]
        public static extern int Parameter2(string RandDivApduData, ref string dataOut);
    }

    /// <summary>
    /// 网络加密机驱动接口
    /// </summary>
    public static class gSJJ1009Server
    {
        /// 用于获取登录服务器的权限。
        /// <summary>
        /// 用于获取登录服务器的权限。
        /// </summary>
        /// <returns>返回0:成功;其他:失败</returns>
        [DllImport("SJJ1009Server.dll")]
        public static extern int OpenUsbkey();

        /// 用于登录服务器。  
        /// <summary>
        /// 用于登录服务器。  
        /// </summary>
        /// <param name="IP">IP地址</param>
        /// <param name="port">服务器端口号，短整型</param>
        /// <param name="nPwdLen"> 密码长度，整型</param>
        /// <param name="pPwd">SBKEY 密码,无符号字符型</param>
        /// <returns>返回0:成功;其他:失败</returns>
        [DllImport("SJJ1009Server.dll")]
        public static extern int LgServer(string IP,ushort  port, int  nPwdLen, string pPwd);

        /// 断开与服务器连接函数
        /// <summary>
        /// 断开与服务器连接函数
        /// </summary>
        /// <returns>返回0:成功;其他:失败</returns>
        [DllImport("SJJ1009Server.dll")]
        public static extern int LgoutServer();

        /// 释放服务器登录权限函数
        /// <summary>
        /// 释放服务器登录权限函数
        /// </summary>
        /// <returns>返回0:成功;其他:失败</returns>
        [DllImport("SJJ1009Server.dll")]
        public static extern int ClseUsbkey();

        /// 从密码机获取随机数以及密文,用于远程身份认证
        /// <summary>
        /// 从密码机获取随机数以及密文,用于远程身份认证
        /// </summary>
        /// <param name="Flag">0: 生产密钥状态;1: 交易密钥状态 </param>
        /// <param name="PutDiv">表示输入的分散因子,字符型,长度16,“0000”+表号</param>
        /// <param name="OutRand">输出的随机数,字符型,长度16</param>
        /// <param name="OutEndata">输出的密文,字符型,长度16</param>
        /// <param name="NameId">调用者ID（推荐使用厂家或单位的名称）</param>
        /// <returns>返回0:成功;其他:失败</returns>
        [DllImport("SJJ1009Server.dll")]
        public static extern int IdentityAuthentication(int Flag, string PutDiv, StringBuilder OutRand, StringBuilder OutEndata, string NameId);

        /// 远程跳/合闸命令加密函数，用于形成跳/合闸命令的 645 命令报文。
        /// <summary>
        /// 远程跳/合闸命令加密函数，用于形成跳/合闸命令的 645 命令报文。
        /// </summary>
        /// <param name="Flag">恒为0</param>
        /// <param name="PutRand">表示输入的随机数,字符型,长度8</param>
        /// <param name="PutDiv">表示输入的分散因子,字符型,长度16 ,“0000”+表号</param>
        /// <param name="PutEsamNo">表示输入的 ESAM 序列号,复位信息的后 8 字节,字符型,长度16</param>
        /// <param name="PutData">表示跳闸或合闸控制命令明文,字符型</param>
        /// <param name="OutEndata">输出的密文,字符型</param>
        /// <returns>返回0:成功;其他:失败</returns>
        [DllImport("SJJ1009Server.dll")]
        public static extern int UserControl(int Flag, string PutRand, string PutDiv, string PutEsamNo, string PutData, StringBuilder OutEndata);

        /// 远程一类参数MAC 计算函数，用于形成一类参数设置645 命令的报文.
        /// <summary>
        /// 远程一类参数MAC 计算函数，用于形成一类参数设置645 命令的报文.
        /// </summary>
        /// <param name="Flag">恒为0 </param>
        /// <param name="PutRand">表示输入的随机数,字符型,长度8 </param>
        /// <param name="PutDiv">表示输入的分散因子,字符型,长度16,“0000”+表号</param>
        /// <param name="PutApdu">一类参数设置的写 Esam 命令头，字符型，长度10</param>
        /// <param name="PutData">表示输入的一类参数明文,字符型 </param>
        /// <param name="OutEndata">输出的MAC 数据，字符型，长度8</param>
        /// <returns>返回0:成功;其他:失败</returns>
        [DllImport("SJJ1009Server.dll")]
        public static extern int ParameterUpdate(int Flag, string PutRand, string PutDiv, string PutApdu, string PutData, StringBuilder OutEndata);

        /// 远程二类参数设置加密函数。用于形成二类参数设置645 命令的报文.
        /// <summary>
        /// 远程二类参数设置加密函数。用于形成二类参数设置645 命令的报文.
        /// </summary>
        /// <param name="Flag">恒为0 </param>
        /// <param name="PutRand">表示输入的随机数,字符型,长度8 </param>
        /// <param name="PutDiv">表示输入的分散因子,字符型,长度16,“0000”+表号</param>
        /// <param name="PutApdu">一类参数设置的写 Esam 命令头，字符型，长度10</param>
        /// <param name="PutData">表示输入的二类参数明文,字符型 </param>
        /// <param name="OutEndata">输出的MAC 数据，字符型，长度8</param>
        /// <returns>返回0:成功;其他:失败</returns>
        [DllImport("SJJ1009Server.dll")]
        public static extern int ParameterElseUpdate(int Flag, string PutRand, string PutDiv, string PutApdu, string PutData, StringBuilder OutEndata);

        /// 电能表远程密钥信息清零。
        /// <summary>
        /// 电能表远程密钥信息清零。
        /// </summary>
        /// <param name="Flag">0,公钥状态下清零,1，私钥状态下清零</param>
        /// <param name="rand">表示输入的随机数,字符型,长度8</param>
        /// <param name="div">表示输入的分散因子,字符型,长度16,“0000”+表号</param>
        /// <param name="EsamNo">表示输入的 ESAM 序列号,复位信息的后 8 字节,字符型,长度16  </param>
        /// <param name="PutKeyinfo1">表示输入的远程控制密钥密钥信息明文,字符型</param>
        /// <param name="Outkey1">输出的远程控制密钥密文,字符型，长度64</param>
        /// <param name="Outinfo1">输出的远程控制密钥信息,字符型，长度8</param>
        /// <returns>返回0:成功;其他:失败</returns>
        [DllImport("SJJ1009Server.dll")]
        public static extern int ClearKeyInfo(int Flag, string rand, string div, string EsamNo, string PutKeyinfo1, StringBuilder Outkey1, StringBuilder Outinfo1);

        /// 密钥更新
        /// <summary>
        /// 获取3 条远程密钥和主控密钥的密钥信息和密钥密文，用于产生远程更新密钥的 645 命令
        /// </summary>
        /// <param name="Counter">恒为0 </param>
        /// <param name="PutRand">表示输入的随机数,字符型,长度16</param>
        /// <param name="PutDiv">表示输入的分散因子,字符型,长度16 ,“0000”+表号</param>
        /// <param name="PutEsamNo">表示输入的 ESAM 序列号,复位信息的后 8 字节,字符型,长度16</param>
        /// <param name="PutKeyinfo1">表示输入的主控密钥密钥信息明文,字符型 </param>
        /// <param name="PutKeyinfo2">表示输入的远程控制密钥信息明文,字符型</param>
        /// <param name="PutKeyinfo3">表示输入的二类参数设置密钥信息明文,字符型</param>
        /// <param name="PutKeyinfo4">表示输入的远程身份认证密钥信息明文,字符型 </param>
        /// <param name="OutKey1">输出的主控密钥密文,字符型，长度 64</param>
        /// <param name="OutKeyinfo1">输出的主控密钥信息,字符型，长度 8</param>
        /// <param name="OutKey2">输出的远程控制密钥密文,字符型，长度64</param>
        /// <param name="OutKeyinfo2">输出的远程控制密钥信息,字符型，长度8</param>
        /// <param name="OutKey3">输出的二类参数设置密钥密文,字符型，长度 64</param>
        /// <param name="OutKeyinfo3">输出的二类参数设置密钥信息,字符型，长度 8</param>
        /// <param name="OutKey4">输出的远程身份认证密钥密文,字符型，长度 64</param>
        /// <param name="OutKeyinfo4">输出的远程身份认证密钥信息,字符型，长度 8</param>
        /// <returns>返回0:成功;其他:失败</returns>
        [DllImport("SJJ1009Server.dll")]
        public static extern int KeyUpdate(int Counter, string PutRand, string PutDiv, string PutEsamNo,
                                         string PutKeyinfo1, string PutKeyinfo2, string PutKeyinfo3, string PutKeyinfo4,
                                         StringBuilder OutKey1, StringBuilder OutKeyinfo1, StringBuilder OutKey2, StringBuilder OutKeyinfo2,
                                         StringBuilder OutKey3, StringBuilder OutKeyinfo3, StringBuilder OutKey4, StringBuilder OutKeyinfo4);


        #region [开发套件]
        /// 身份认证取随机数和密文 
        /// <summary>
        /// 身份认证取随机数和密文 
        /// </summary>
        /// <param name="Div">输入参数，8 字节分散因子，16 进制字符串</param>
        /// <param name="RandAndEndata">输出参数，字符型，8 字节随机数+8 字节密文。</param>
        /// <returns>
        /// 0，成功  ；                 
        /// 200，连接加密机失败；                  
        /// 201，取随机数1 失败；                  
        /// 202，取随机数2 失败；                  
        /// 203，密钥分散失败；                  
        /// 204，数据加密失败；                   
        /// 205，取密文失败； 
        /// </returns>
        [DllImport("TestZhuzhan.dll")]
        public static extern int IdentityAuthentication(string Div, StringBuilder RandAndEndata);

        /// 远程控制
        /// <summary>
        /// 远程控制,用啦拉合闸试验。
        /// </summary>
        /// <param name="RandDivEsamNumData">输入参数，字符型，4字节随机数参数说明+8 字节分散因子+8 字节ESAM 序列号+数据明文。</param>
        /// <param name="dataOut">字符型，20 字节密文</param>
        /// <returns>
        /// 0，成功  ；                  
        /// 200，连接加密机失败；                   
        /// 201，写卡失败；                   
        /// 202，读卡失败；                   
        /// 203，计算密文失败； 
        /// </returns>
        [DllImport("TestZhuzhan.dll")]
        public static extern int UserControl(string RandDivEsamNumData, StringBuilder dataOut);

        /// 更新参数
        /// <summary>
        /// 更新参数
        /// </summary>
        /// <param name="RandDivApduData">
        /// 4 字节随机数；（字符型8 字节分散因子；更新指令10 位(04d682+起始+LC)；
        /// LC=明文数据长度+4。 其他为参数明文
        /// </param>
        /// <param name="dataOut">返回参数明文和MAC</param>
        /// <returns>
        /// 0，成功  ； 
        /// 200，连接加密机失败；
        /// 201，写卡失败；
        /// 202，读卡失败；
        /// 203，计算MAC 失败
        /// </returns>
        [DllImport("TestZhuzhan.dll")]
        public static extern int ParameterUpdate(string RandDivApduData, StringBuilder dataOut);

        /// 密文+MAC 更新参数
        /// <summary>
        /// 密文+MAC 更新参数
        /// </summary>
        /// <param name="RandDivApduData">4字节随机数；8 字节分散因子；更新指令10 位(04d6+文件标识+00+LC)；（此处LC 长度为下发密文数据+MAC 的长度）其他为参数明文</param>
        /// <param name="EsamNum">输入参数，8 字节ESAM 序列号。</param>
        /// <param name="dataOut">
        /// 返回参数密文和MAC.
        /// 电能表接收密文+MAC 后，用04d6+文件标识+00+LC+密文+MAC 更新ESAM 文件，然后明文读取数据，该文件第一个字节为明文数据的长度（HEX ），可以根据该长度取所解密后的明文.
        /// </param>
        /// <returns>
        ///  0，成功；                        
        ///  200，连接加密机失败；                       
        ///  201，写卡失败；                       
        ///  202，读卡失败；                        
        ///  203，计算MAC失败；
        /// </returns>
        [DllImport("TestZhuzhan.dll")]
        public static extern int ParameterElseUpdate(string RandDivApduData, ref string EsamNum, ref string dataOut);

        /// 更新密钥
        /// <summary>
        /// 更新密钥
        /// </summary>
        /// <param name="Kid">kid=1，身份认证密钥；kid=2,远程控制密钥；Kid=3,参数更新密钥。</param>
        /// <param name="DivEsamNumRandData">输入参数，字符型，8字节分散因子+8字节ESAM 序列号+4 字节随机数+4 字节数据明文。</param>
        /// <param name="dataOut">返回32 字节密文+ 4 字节密钥信息+4字节MAC。</param>
        /// <returns>
        /// 0，成功；                        
        /// 200，连接加密机失败；                       
        /// 201，写卡失败；                       
        /// 202，读卡失败；                       
        /// 203，计算MAC失败；
        /// </returns>
        [DllImport("TestZhuzhan.dll")]
        public static extern int KeyUpdate(int Kid, string DivEsamNumRandData, StringBuilder dataOut);

        /// 费率文件1 更新函数
        /// <summary>
        /// 费率文件1 更新函数
        /// </summary>
        /// <param name="RandDivApduData">随机数8 位；分散因子16 位；更新指令10 位(04d683+起始+LC)；LC=明文数据长度+4。其他为参数明文。</param>
        /// <param name="dataOut">返回参数明文和MAC。</param>
        /// <returns>0，成功；
        ///  200，连接加密机失败；
        ///  201，写卡失败；
        ///  202，读卡失败；
        ///  203，计算MAC 失败；
        /// </returns>
        [DllImport("TestZhuzhan.dll")]
        public static extern int Parameter1(string RandDivApduData, StringBuilder dataOut);

        /// 费率文件2 更新函数
        /// <summary>
        /// 费率文件2 更新函数
        /// </summary>
        /// <param name="RandDivApduData">随机数8 位；分散因子16 位；更新指令10 位(04d683+起始+LC)；LC=明文数据长度+4。其他为参数明文。</param>
        /// <param name="dataOut">返回参数明文和MAC。</param>
        /// <returns>0，成功；
        ///  200，连接加密机失败；
        ///  201，写卡失败；
        ///  202，读卡失败；
        ///  203，计算MAC 失败；
        /// </returns>
        [DllImport("TestZhuzhan.dll")]
        public static extern int Parameter2(string RandDivApduData, StringBuilder dataOut);

        #endregion
    }

    /// <summary>
   /// 将开发套件中的接口，单独生成相应的接口函数
   /// </summary>
    public static class gChkESAM
    {
        /// <summary>
        /// 连接加密机
        /// </summary>
        /// <param name="IP">IP地址</param>
        /// <param name="port">服务器端口号，短整型</param>
        /// <param name="nPwdLen"> 密码长度，整型</param>
        /// <param name="pPwd">SBKEY 密码,无符号字符型</param>
        /// <param name="Netencryption">true：网络加密机；false：开发套件</param>
        /// <returns></returns>
        public static bool ConnectEncryptionMachine(string IP,ushort  port, int  nPwdLen, string pPwd,bool Netencryption)
        {
            int intConnectOk = -1;
            if (!Netencryption)
                return true;
            try
            {
                intConnectOk = gSJJ1009Server.OpenUsbkey();
                CheckSMResult(intConnectOk);
                intConnectOk = gSJJ1009Server.LgServer(IP, port, nPwdLen, pPwd);
                CheckSMResult(intConnectOk); 
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message + " " + ex.TargetSite);
            }
            return true;
        }

        /// <summary>
        /// 获取随机数和密文1
        /// </summary>
        /// <param name="Flag">0: 生产密钥状态;1: 交易密钥状态</param>
        /// <param name="putdiv">分散因子</param>
        /// <param name="outrand">输出随机数1</param>
        /// <param name="outpwd">输出密文1</param>
        /// <param name="Netencryption">true：网络加密机；false：开发套件</param>
        /// <returns></returns>
        public static bool GetRand1_Pwd1(int Flag, string putdiv, StringBuilder outrand, StringBuilder outpwd, bool Netencryption)
        {
            int intConnectOK = -1;
            if (!Netencryption)
            {
                //intConnectOK = gTestZhuzhan.IdentityAuthentication(putdiv, ref RandAndEndData);
                //CheckSMResult(intConnectOK);
                //outrand = RandAndEndData.Substring(1, 16);
                //outpwd = RandAndEndData.Substring(17, 16);
            }
            else
            {
                intConnectOK = gSJJ1009Server.IdentityAuthentication(Flag, putdiv, outrand, outpwd, "nzsc");
                CheckSMResult(intConnectOK);
            }
            return true;
        }

        /// <summary>
        /// 断开与加密机的连接
        /// </summary>
        /// <param name="Netencryption">true：网络加密机；false：开发套件</param>
        /// <returns></returns>
        public static bool CloseEncryptionMachine(bool Netencryption)
        {
            int intConnectOk = -1;
            if (!Netencryption)
                return true;
            else
            {
                intConnectOk = gSJJ1009Server.ClseUsbkey();
                CheckSMResult(intConnectOk);
                //intConnectOk = gSJJ1009Server.LgoutServer();
                //CheckSMResult(intConnectOk);
            }
            return true;
        }

        /// <summary>
        /// 返回出错的内容
        /// </summary>
        /// <param name="result">错误代码</param>
        public static void CheckSMResult(int result)
        {
            if (result !=0)
            {
                string msg = "";
                msg = "操作网络加密机失败，请按照以下步骤检查:\r";
                msg += "1、Usbkey是否插入计算机中，没有重新插入UsbKey即可；\r";
                msg += "2、Usbkey插入计算机时，是否在右下角弹出小对话框，提示有两张证书；\r";
                msg += "3、在命令窗口，ping网络加密机的IP地址是否成功；\r";
                msg += "4、检查表计是否具备【费控】功能；\r";
                msg += "5、若前面4步均正确，请用总线式读取485功能，若不成功，设备在总线485通讯上出故障，联系硬件售后人员。\r";
                throw new Exception(msg);
            }
        }

    }
}
