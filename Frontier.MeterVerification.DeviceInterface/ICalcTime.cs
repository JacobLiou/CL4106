using System;
using System.Collections.Generic;

using System.Text;
using VerificationEquipment.Commons;

namespace Frontier.MeterVerification.DeviceInterface
{
    /// 计算校验点执行时间接口
    /// <summary>
    /// 计算校验点执行时间接口
    /// </summary>
    public interface ICalcTime
    {
        /// 预热时间
        /// <summary>
        /// 预热时间
        /// </summary>
        /// <param name="U">预热电压，单位V</param>
        /// <param name="I">预热电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="second">预热时间，单位秒</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>预热预计时间,单位秒</returns>
        float CalcWarmUpTime(float U, float I, float acFreq, int second);
        /// 基本误差试验（合元）时间
        /// <summary>
        /// 基本误差试验（合元）时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="I">电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="factor">功率因数</param>
        /// <param name="capacitive">容性标志，true表示容性，false表示感性</param>
        /// <param name="pulse">脉冲类型，正向有功、反向有功、正向无功、反向无功</param>
        /// <param name="circle">检验圈数</param>
        /// <param name="count">检验次数</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>基本误差试验预计时间,单位秒</returns>
        float CalcBasicErrorTime(float U, float I, float factor, bool capacitive, float acFreq, string pulse,
            int circle, int count, out float verifyTime);
        /// 基本误差试验（分元）时间
        /// <summary>
        /// 基本误差试验（分元）时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="I">电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="phase">相别，"A"：A(B)相；"B"：B相；"C"：C(B)相。注意三相三线表不作B相试验</param>
        /// <param name="factor">功率因数</param>
        /// <param name="capacitive">容性标志，true表示容性，false表示感性</param>
        /// <param name="pulse">脉冲类型，正向有功、反向有功、正向无功、反向无功</param>
        /// <param name="circle">检验圈数</param>
        /// <param name="count">检验次数</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>基本误差试验预计时间,单位秒</returns>
        float CalcBasicErrorTime(float U, float I, float acFreq, string phase, float factor, bool capacitive, Pulse pulse, int circle, int count, out float verifyTime);
        /// 启动试验时间
        /// <summary>
        /// 启动试验时间
        /// </summary>
        /// <param name="U">启动电压，单位V</param>
        /// <param name="I">启动电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="second">启动试验时间，单位秒</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>启动试验预计时间,单位秒</returns>
        float CalcStartupTime(float U, float I, float acFreq, int second);
        /// 潜动试验时间
        /// <summary>
        /// 潜动试验时间
        /// </summary>
        /// <param name="U">潜动电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="second">潜动试验时间，单位：秒</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>潜动试验预计时间,单位秒</returns>
        float CalcLatentTime(float U, float acFreq, int second);
        /// 日计时试验时间
        /// <summary>
        /// 日计时试验时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="clockFreq">被检表时钟脉冲频率，默认1Hz</param>
        /// <param name="second">检验时间，单位：秒</param>
        /// <param name="count">检验次数</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>日计时试验预计时间,单位秒</returns>
        float CalcClockErrorTime(float U, float acFreq, float clockFreq, int second, int count, out float verifyTime);
        /// 走字和校核计度器试验时间
        /// <summary>
        /// 走字和校核计度器试验时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="I">试验电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="factor">功率因数</param>
        /// <param name="capacitive">容性标志，true表示容性，false表示感性</param>
        /// <param name="pulse">脉冲类型，正向有功、反向有功、正向无功、反向无功</param>
        /// <param name="phase">费率</param>
        /// <param name="energy">走字电能(kWh)</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>走字和校核计度器试验预计时间,单位秒</returns>
        float CalcEnergyReadingTime(float U, float I, float acFreq, float factor, bool capacitive, Pulse pulse, Phase phase, float energy, out float verifyTime);
        /// 需量周期、需量示值时间
        /// <summary>
        /// 需量周期、需量示值时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="I">试验电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>需量周期、需量示值预计时间,单位秒</returns>
        float CalcDemandTime(float U, float I, float acFreq, out float verifyTime);
        /// 时段投切试验时间
        /// <summary>
        /// 时段投切试验时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="I">试验电流，单位A，默认300%Ib</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>时段投切试验预计时间,单位秒</returns>
        float CalcSwitchChangeTime(float U, float I, float acFreq, out float verifyTime);
        /// 读电能底数时间
        /// <summary>
        /// 读电能底数时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="pulse">脉冲类型，正向有功、反向有功、正向无功、反向无功</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>读电能底数预计时间,单位秒</returns>
        float CalcGetEnergyReadingTime(float U, float acFreq, Pulse pulse);
        /// 读需量底数时间
        /// <summary>
        /// 读需量底数时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="pulse">脉冲类型，正向有功、反向有功、正向无功、反向无功</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>读需量底数预计时间,单位秒</returns>
        float CalcGetDemandReadingTime(float U, float acFreq, Pulse pulse);
        /// 读取表地址时间
        /// <summary>
        /// 读取表地址时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>读取表地址预计时间,单位秒</returns>
        float CalcReadMeterAddressTime(float U, float acFreq);
        /// 保电试验时间
        /// <summary>
        /// 保电试验时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>保电试验预计时间,单位秒</returns>
        float CalcHoldPowerTime(float U, float acFreq);
        /// 保电命令时间
        /// <summary>
        /// 保电命令时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>保电命令预计时间,单位秒</returns>
        float CalcHoldPowerCommandTime(float U, float acFreq);
        /// 解除保电命令时间
        /// <summary>
        /// 解除保电命令时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>解除保电命令预计时间,单位秒</returns>
        float CalcReleasePowerCommandTime(float U, float acFreq);
        /// 远程开户充值时间
        /// <summary>
        /// 远程开户充值时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="money">充值金额</param>
        /// <param name="count">充值次数</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>远程开户充值预计时间,单位秒</returns>
        float CalcRemoteOpenAccountTime(float U, float acFreq, double money, int count);
        /// 远程充值时间
        /// <summary>
        /// 远程充值时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="money">充值金额</param>
        /// <param name="count">充值次数</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>远程充值预计时间,单位秒</returns>
        float CalcRemoteChangeAccountTime(float U, float acFreq, double money, int count);
        /// 远程密钥更新时间
        /// <summary>
        /// 远程密钥更新时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="isRemote">远程表或本地表，true:远程表，false:本地表</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>远程密钥更新预计时间,单位秒</returns>
        float CalcRemoteSecretKeyUpdateTime(float U, float acFreq, bool isRemote);
        /// 远程参数修改时间
        /// <summary>
        /// 远程参数修改时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns> 远程参数修改预计时间,单位秒</returns>
        float CalcRemoteParameterUpdateTime(float U, float acFreq);
        /// 远程数据回抄时间
        /// <summary>
        /// 远程数据回抄时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>远程数据回抄预计时间,单位秒</returns>
        float CalcRemoteDataReturnTime(float U, float acFreq);
        /// 远程拉合闸时间
        /// <summary>
        /// 远程拉合闸时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>远程拉合闸预计时间,单位秒</returns>
        float CalcRemoteKnifeSwitchTime(float U, float acFreq);
        /// 远程拉闸命令时间
        /// <summary>
        /// 远程拉闸命令时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>远程拉闸命令预计时间,单位秒</returns>
        float CalcRemoteOpenSwitchCommandTime(float U, float acFreq);
        /// 远程合闸命令时间
        /// <summary>
        /// 远程合闸命令时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>远程合闸命令预计时间,单位秒</returns>
        float CalcRemoteCloseSwitchCommandTime(float U, float acFreq);
        /// 报警试验时间
        /// <summary>
        /// 报警试验时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>报警试验预计时间,单位秒</returns>
        float CalcAlarmTime(float U, float acFreq);
        /// 清电量时间
        /// <summary>
        /// 清电量时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>清电量预计时间,单位秒</returns>
        float CalcClearEnergyTime(float U, float acFreq);
        /// 清需量时间
        /// <summary>
        /// 清需量时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>清需量预计时间,单位秒</returns>
        float CalcClearDemandTime(float U, float acFreq);
        /// 表计对时时间
        /// <summary>
        /// 表计对时时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>表计对时预计时间,单位秒</returns>
        float CalcTimeSyncTime(float U, float acFreq);
        /// 修改02、04级密码时间
        /// <summary>
        /// 修改02、04级密码时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>修改密码预计时间,单位秒</returns>
        float CalcChangePasswordTime(float U, float acFreq);
        /// 通信测试时间
        /// <summary>
        /// 通信测试时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>通信测试预计时间,单位秒</returns>
        float CalcCommunicationTestTime(float U, float acFreq);
        /// 事件记录时间
        /// <summary>
        /// 事件记录时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>事件记录预计时间,单位秒</returns>
        float CalcEventRecordTime(float U, float acFreq);
        /// 公钥验证时间
        /// <summary>
        /// 公钥验证时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>公钥验证预计时间,单位秒</returns>
        float CalcPublicKeyAuthenticationTime(float U, float acFreq);
        /// 私钥验证时间
        /// <summary>
        /// 私钥验证时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>私钥验证预计时间,单位秒</returns>
        float CalcPrivateKeyAuthenticationTime(float U, float acFreq);
        /// 定时冻结时间
        /// <summary>
        /// 定时冻结时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>定时冻结预计时间,单位秒</returns>
        float CalcFreezeByTimeTime(float U, float acFreq);
        /// 瞬时冻结时间
        /// <summary>
        /// 瞬时冻结时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>瞬时冻结预计时间,单位秒</returns>
        float CalcFreezeByInstantaneousTime(float U, float acFreq);
        /// 日冻结时间
        /// <summary>
        /// 日冻结时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>日冻结预计时间,单位秒</returns>
        float CalcFreezeByDayTime(float U, float acFreq);
        /// 整点冻结时间
        /// <summary>
        /// 整点冻结时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>整点冻结预计时间,单位秒</returns>
        float CalcFreezeByHourTime(float U, float acFreq);
        /// 载波召测时间
        /// <summary>
        /// 载波召测时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="dataID">数据项标示</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>载波召测预计时间,单位秒</returns>
        float CalcCarrieWaveReturnTime(float U, float acFreq, string dataID);
        /// 载波可靠性时间
        /// <summary>
        /// 载波可靠性时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="dataID">数据项标示</param>
        /// <param name="times">重发次数</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>载波可靠性预计时间,单位秒</returns>
        float CalcCarrieWaveReliabilityTime(float U, float acFreq, string dataID, int times);
        /// 载波成功率时间
        /// <summary>
        /// 载波成功率时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="dataID">数据项标示</param>
        /// <param name="times">重发次数</param>
        /// <param name="interval">抄读间隔时间</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>载波成功率预计时间,单位秒</returns>
        float CalcCarrieWaveSuccessRateTime(float U, float acFreq, string dataID, int times, int interval);
        /// 通信规约一致性检查时间
        /// <summary>
        /// 通信规约一致性检查时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="dataID">数据项标示</param>
        /// <param name="length">长度</param>
        /// <param name="digital">小数位</param>
        /// <param name="readWrite">操作，true:写，false:读</param>
        /// <param name="content">写入数据内容</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>通信规约一致性检查预计时间,单位秒</returns>
        float CalcProtocolConsistencyTime(float U, float acFreq, string dataID, int length, int digital, bool readWrite, string content);
        /// 接线检查时间
        /// <summary>
        /// 接线检查时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="I">电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>接线检查预计时间,单位秒</returns>
        float CalcConnectionCheckTime(float U, float I, float acFreq);
        /// 耐压试验时间
        /// <summary>
        /// 耐压试验时间
        /// </summary>
        /// <param name="resistanceU">耐压试验测试点电压，单位KV</param>
        /// <param name="resistanceI">漏电流档位，单位mA</param>
        /// <param name="resistanceTime">耐压试验时间，单位秒</param>
        /// <param name="resistanceType">耐压方式：”00”对外壳打耐压；”01”对辅助端子打耐压；”02”对外壳和辅助端子打耐压</param>
        /// <param name="verifyTime">开始检定后计时时间</param>
        /// <returns>耐压试验时间,单位秒</returns>
        float CalcResistanceTime(float resistanceU, float resistanceI, int resistanceTime, string resistanceType);
    }
}
