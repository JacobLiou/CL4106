using System;
using System.Collections.Generic;

using System.Text;
using VerificationEquipment.Commons;
using Frontier.MeterVerification.DeviceCommon;

namespace Frontier.MeterVerification.DeviceInterface
{
    /// 电能表645接口
    /// <summary>
    /// 电能表645接口
    /// </summary>
    public interface IDlt645
    {
        /// 获取电能表表地址
        /// <summary>
        /// 获取电能表表地址
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <returns></returns>
        string GetAddress(int meterIndex);
        /// <summary>
        /// 获取电能表生产编号
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <returns></returns>
        string GetScbh(int meterIndex);
        /// 获取电能表日期
        /// <summary>
        /// 获取电能表日期
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <returns></returns>
        string GetDate(int meterIndex);
        /// 获取电能表时间
        /// <summary>
        /// 获取电能表时间
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <returns></returns>
        string GetTime(int meterIndex);
        /// 设置电能表日期
        /// <summary>
        /// 设置电能表日期
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        bool SetDate(int meterIndex, DateTime dateTime);
        /// 设置电能表时间
        /// <summary>
        /// 设置电能表时间
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        bool SetTime(int meterIndex, DateTime dateTime);
        /// 获取电能表电能
        /// <summary>
        /// 获取电能表电能
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <returns></returns>
        string GetEnergy(int meterIndex, Pulse pulse);
        /// 清电量
        /// <summary>
        /// 清电量
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <returns></returns>
        bool ClearEnergy(int meterIndex);
        /// 获取电能表需量
        /// <summary>
        /// 获取电能表需量
        /// </summary>
        /// <param name="meterInde"></param>
        /// <param name="pulse"></param>
        /// <returns></returns>
        string GetDemand(int meterIndex);
        /// 清需量
        /// <summary>
        /// 清需量
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <returns></returns>
        bool ClearDemand(int meterIndex);
        /// 设定单表位高频常数
        /// <summary>
        /// 设定单表位高频常数
        /// </summary>
        /// <param name="meterIndex">表位号</param>
        /// <param name="iBeilv">设置的倍率</param>
        /// <returns>返回设置成功与否</returns>
        bool SetMeterDoubling(int meterIndex, byte iBeilv);

        /// 设置多功能端子输出
        /// <summary>
        /// 设置多功能端子输出
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="mulTerminalOut"></param>
        /// <returns></returns>
        bool SetMulTerminalOut(int meterIndex, VerificationElementType elementType);
        /// 获取电能表表地址
        /// <summary>
        /// 获取电能表表地址
        /// </summary>
        /// <param name="meterIndex">表位数组</param>
        /// <param name="address">表地址数组</param>
        void GetAddress(int[] meterIndex, out string[] address);
        /// <summary>
        /// 设置电能表高频倍率
        /// </summary>
        /// <param name="meterIndex">表位数组</param>
        /// <param name="bResult">返回表位结论</param>
        /// <param name="bBeilv">设置的倍率</param>
        void SetMeterDoubling(int[] meterIndex, out bool[] bResult, byte bBeilv);
        /// <summary>
        /// 打包参数下载并进行比对
        /// </summary>
        /// <param name="Item_TxFrame">下发数据</param>
        /// <param name="Item_RxFrame">比对返回数据</param>
        /// <param name="meterIndex">表位号</param>
        /// <returns></returns>
        bool DownCmdToMeterTxAndRx(string Item_TxFrame, string Item_RxFrame, out string strMeterRx, int meterIndex);
        /// <summary>
        ///  打包参数下载并进行比对
        /// </summary>
        /// <param name="Item_TxFrame"></param>
        /// <param name="Item_RxFrame"></param>
        /// <param name="meterIndex"></param>
        /// <param name="meterreSult"></param>
        void DownCmdToMeter(string Item_TxFrame, string Item_RxFrame, int[] meterIndex, out bool[] meterreSult, out string[] meterRx);

        /// <summary>
        /// 获取电表生产编号
        /// </summary>
        /// <param name="meterIndex">表位数组</param>
        /// <param name="meterScbh">表地址数组</param>
        void GetScbh(int[] meterIndex, out string[] meterScbh);

        void GetDate(int[] meterIndex, out DateTime[] dateTime);

        void GetTime(int[] meterIndex, out DateTime[] dateTime);

        float[] ReadMeterParam(int meterIndex, uint uSendParam);

        void ReadMeterParam(int[] meterIndex,uint uSendParam, out float[][] foutMeterParam);
        /// <summary>
        /// 获取分相供电测试数据
        /// </summary>
        /// <param name="meterIndex">表位-1</param>
        /// <param name="MeterOneVolt">第一相电压值，依次类推A、B、C(加A相电压，就读B、C相</param>
        /// <param name="mererTwoVolt">第二相电压值，依次类推A、B、C(加A相电压，就读B、C相</param>
        void GetOtherVoltage(int[] meterIndex, int VoltageIndex, out float[] MeterOneVolt, out float[] mererTwoVolt);
        /// <summary>
        /// 获取分相供电数据单表位
        /// </summary>
        /// <param name="mererIndex">表位-1</param>
        /// <param name="meterOneVolt">第一相电压</param>
        /// <param name="meterTwoVolt">第二相电压</param>
        void GetOtherVoltOnePosition(int mererIndex, int VoltageIndex,out float meterOneVolt, out float meterTwoVolt);

        void SetDate(int[] meterIndex, DateTime dateTime, out bool[] results, out string[] resultDescript);

        void SetTime(int[] meterIndex, DateTime dateTime, out bool[] results, out string[] resultDescript);

        void SetTime(int[] meterIndex, out bool[] results, out string[] resultDescript);

        void GetEnergy(int[] meterIndex, Pulse pulse, out string[] energy);

        void ClearEnergy(int[] meterIndex, out bool[] results, out string[] resultDescript);

        void GetDemand(int[] meterIndex, Pulse pulse, out string[] demand);

        void ClearDemand(int[] meterIndex, out bool[] results, out string[] resultDescript);

        void SetMulTerminalOut(int[] meterIndex, VerificationElementType elementType, out bool[] results, out string[] resultDescript);
        /// <summary>
        /// 1个表位的费率
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="meterRates"></param>
        void GetMeterRates(int[] meterIndex, out MeterRates[] meterRates);

        void GetDemandInterval(int[] meterIndex, out int[] demandPeriod);
        /// <summary>
        /// 获取滑差时间
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="HCTime"></param>
        void GetSlidingWindowTime(int[] meterIndex, out int[] slidingWindowTime);

        /// 获取拉合闸状态字
        /// <summary>
        /// 获取拉合闸状态字
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="states"></param>
        void GetKnifeSwitchStatus(int[] meterIndex, out String[] states);

        /// 身份验证
        /// <summary>
        /// 身份验证
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="putdiv">分散因子</param>
        /// <param name="putrand1">随机数1</param>
        /// <param name="putpwd1">密文1</param>
        /// <param name="outrand2">输出随机数2</param>
        /// <param name="outesam">输出easm</param>
        /// <param name="Netencryption">true：网络加密机；false：开发套件</param>
        void SetSecurityCertificate(int[] meterIndex, string putDiv, string putRand1, string putPwd1, out string[] outRand2, out string[] outEsam, bool netEncryption);

        /// 身份验证
        /// <summary>
        /// 身份验证
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="putrand1">随机数1</param>
        /// <param name="putpwd1">密文1</param>
        /// <param name="outrand2">输出随机数2</param>
        /// <param name="outesam">输出easm</param>
        /// <param name="Netencryption">true：网络加密机；false：开发套件</param>
        void SetSecurityCertificate(int[] meterIndex, string[] putRand1, string[] putPwd1, out string[] outRand2, out string[] outEsam, bool netEncryption);

        /// 拉合闸、保电、告警等操作
        /// <summary>
        /// 拉合闸、保电、告警等操作
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="outrand">随机数2</param>
        /// <param name="ESAM">ESAM序列号</param>
        /// <param name="OpenAlarm">N1=1AH代表跳闸，N1=1BH代表合闸允许，N1=2AH代表报警，N1=2BH代表报警解除，N1=3AH代表保电，N1=3BH代表保电解除</param>
        /// <param name="Netencryption">true：网络加密机；false：开发套件</param>
        /// <param name="results">结果</param>
        /// <returns></returns>
        void UserControlOpenAlarm(int[] meterIndex, string[] outRand, string[] esam, int openAlarm, bool netEncryption, out bool[] results);

        /// 数据回抄
        /// <summary>
        /// 数据回抄
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="fileID">文件标识</param>
        /// <param name="beginAddress">读取数据的相对起始地址</param>
        /// <param name="readLen">要读取的数据长度</param>
        /// <param name="outdata">输出的抄表记录</param>
        /// <param name="netencryption">true：网络加密机；false：开发套件</param>
        /// <returns></returns>
        void GetDataBackToCopy(int[] meterIndex, int fileID, int beginAddress, int readLen, out string[] outData, bool netEncryption);

        /// 主控密钥更新
        /// <summary>
        /// 主控密钥更新
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="beforeInfos">更新前密钥版本信息</param>
        /// <param name="address">表地址</param>
        /// <param name="KeyKind">输入的密钥信息明文</param>
        /// <param name="OutKey">输出的密钥密文</param>
        /// <param name="OutKeyinfo">输出的密钥信息</param>
        /// <param name="results">返回远程密钥更新结果</param>
        /// <param name="resultDescriptions">返回远程密钥更新结果描述</param>
        /// <returns></returns>
        void MainSecurityUpdate(int[] meterIndex, string[] beforeInfos, string keyKind, string outKey, string outKeyinfo, out bool[] results, out string[] resultDescript);

        /// 控制命令密钥更新
        /// <summary>
        /// 控制命令密钥更新
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="beforeInfos">更新前密钥版本信息</param>
        /// <param name="KeyKind">输入的密钥信息明文</param>
        /// <param name="OutKey">输出的密钥密文</param>
        /// <param name="OutKeyinfo">输出的密钥信息</param>
        /// <param name="results">返回远程密钥更新结果</param>
        /// <param name="resultDescriptions">返回远程密钥更新结果描述</param>
        /// <returns></returns>
        void ControlSecurityUpdate(int[] meterIndex, string[] beforeInfos, string keyKind, string outKey, string outKeyinfo, out bool[] results, out string[] resultDescript);

        /// 参数命令密钥更新
        /// <summary>
        /// 参数命令密钥更新
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="beforeInfos">更新前密钥版本信息</param>
        /// <param name="KeyKind">输入的密钥信息明文</param>
        /// <param name="OutKey">输出的密钥密文</param>
        /// <param name="OutKeyinfo">输出的密钥信息</param>
        /// <param name="results">返回远程密钥更新结果</param>
        /// <param name="resultDescriptions">返回远程密钥更新结果描述</param>
        /// <returns></returns>
        void ArguSecurityUpdate(int[] meterIndex, string[] beforeInfos, string keyKind, string outKey, string outKeyinfo, out bool[] results, out string[] resultDescript);

        /// 身份认证密钥更新
        /// <summary>
        /// 身份认证密钥更新
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="beforeInfos">更新前密钥版本信息</param>
        /// <param name="KeyKind">输入的密钥信息明文</param>
        /// <param name="OutKey">输出的密钥密文</param>
        /// <param name="OutKeyinfo">输出的密钥信息</param>
        /// <param name="results">返回远程密钥更新结果</param>
        /// <param name="resultDescriptions">返回远程密钥更新结果描述</param>
        /// <returns></returns>
        void AuthSecurityUpdate(int[] meterIndex, string[] beforeInfos, string keyKind, string outKey, string outKeyinfo, out bool[] results, out string[] resultDescript);
    }
}
