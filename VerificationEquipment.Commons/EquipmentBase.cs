using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace VerificationEquipment.Commons
{
    public abstract class EquipmentBase : IEquipment
    {
        /// <summary>
        /// 设备上电能表的挂接情况
        /// </summary>
        private MeterPosition[] _meterPositions = null;

        /// <summary>
        /// 用户停止当前执行的检验点（切换校验点），true表示停止
        /// </summary>
        private bool _isStop = false;

        public void InstallMeter(MeterPosition[] meterPositions)
        {
            this._meterPositions = meterPositions;
        }

        public abstract void Connect();

        public abstract void Close();

        public void Abort()
        {
            _isStop = true;
        }

        public void SetMeterPositionCount(int count)
        {
            MeterPositionCount = count;
        }

        void IDisposable.Dispose()
        {

        }

        public virtual void OnInfo(string message)
        {
            if (this.Info != null)
            {
                Info(this, new EquipmentMessageEventArgs(message));
            }
        }

        public event EventHandler<EquipmentMessageEventArgs> Info;

        public MeterPosition[] MeterPositions
        {
            get
            {
                return _meterPositions;
            }
        }

        public bool IsStop
        {
            get
            {
                return _isStop;
            }
            set
            {
                _isStop = value;
            }
        }

        public MeterPosition firstGuBiao()
        {
            MeterPosition re = null;
            for (int i = 0; i < _meterPositions.Length; i++)
            {
                if (_meterPositions[i].IsVerify)
                {
                    re = _meterPositions[i];
                    break;
                }
            }
            return re;
        }

        /// <summary>
        /// 表位数量
        /// </summary>
        public int MeterPositionCount
        {
            get;
            set;
        }

        #region IEquipment 成员

        public abstract bool ExecuteBasicError(float U, float I, float acFreq, float factor, bool capacitive, Pulse pulse, int circle, int count, ReturnSampleDatasDelegate returnSampleDatas);

        public abstract bool ExecuteBasicError(float U, float I, float acFreq, float factor, bool capacitive, Pulse pulse, int meterConst, int circle, int count, float fMax, float fMin, ReturnSampleDataErrDelegate returnSampleDatas);

        public abstract bool ExecuteBasicError(float U, float I, float acFreq, string phase, float factor, bool capacitive, Pulse pulse, int circle, int count, ReturnSampleDatasDelegate returnSampleDatas);

        public abstract bool ExecuteBasicError(float U, float I, float acFreq, string phase, float factor, bool capacitive, Pulse pulse, int meterConst, int circle, int count, float fMax, float fMin, ReturnSampleDataErrDelegate returnSampleDatas);

        public abstract bool ExecuteStartup(float U, float I, float acFreq, int second, out string[] startTimes);

        public abstract bool ExecuteLatent(float U, float acFreq, int second, out string[] latentTimes);

        public abstract bool ExecuteEnergyReading(float U, float I, float acFreq, float factor, bool capacitive, Pulse pulse, Phase phase, float energy, out string stdEnergy, out string[] initialTotalReading, out string[] initialSubReading, out string[] finalTotalReading, out string[] finalSubReading);

        public abstract bool ExecuteDemand(float U, float I, float acFreq, out string stdDemand, out string[] checkedDemands, out string[] demandCycleErrors);

        public abstract bool ExecuteSwitchChange(float U, float I, float acFreq, out int[] period, out string[] periodTime, out string[][] valueErrors, out string[] combinErrors, out string[][] changeErrors, out string[][] changeTimes);

        public abstract bool GetEnergyReading(float U, float acFreq, Pulse pulse, out EnergyValue[] energyValues);

        public abstract bool GetDemandReading(float U, float acFreq, Pulse pulse, out string[] demands);

        public abstract void GetCurrentLoad(out string[] U, out string[] I, out string[] P, out string[] Q, out string[] angle, out string acFreq);

        public abstract bool HoldPower(float U, float acFreq, out string[] beforeStates, out string[] afterStates, out string[] releaseBeforeStates, out string[] releaseAfterStates, out bool[] results, out string[] resultDescriptions);

        public abstract bool HoldPowerCommand(float U, float acFreq, out string[] beforeStates, out string[] afterStates, out bool[] results, out string[] resultDescriptions);

        public abstract bool ReleasePowerCommand(float U, float acFreq, out string[] releaseBeforeStates, out string[] releaseAfterStates, out bool[] results, out string[] resultDescriptions);

        public abstract bool RemoteOpenAccount(float U, float acFreq, double money, int count, out bool[] results, out string[] resultDescriptions);

        public abstract bool RemoteChangeAccount(float U, float acFreq, double money, int count, out bool[] results, out string[] resultDescriptions);

        public abstract bool RemoteSecretKeyUpdate(float U, float acFreq, bool isRemote, out bool[] results, out string[] resultDescriptions, out string[] beforeInfos, out string[] afterInfos);

        public abstract bool RemoteParameterUpdate(float U, float acFreq, out bool[] results, out string[] resultDescriptions);

        public abstract bool RemoteDataReturn(float U, float acFreq, out bool[] result1, out bool[] result2, out string[] resultDescriptions, out string[] returnInfo1, out string[] returnInfo2);

        public abstract bool RemoteKnifeSwitch(float U, float acFreq, out bool[] openResults, out bool[] closeResults, out string[] resultDescriptions, out string[] beforeOpenStates, out string[] afterOpenStates, out string[] beforeCloseStates, out string[] afterCloseStaes);

        public abstract bool RemoteOpenSwitchCommand(float U, float acFreq, out bool[] results, out string[] resultDescriptions);

        public abstract bool RemoteCloseSwitchCommand(float U, float acFreq, out bool[] results, out string[] resultDescriptions);

        public abstract bool Alarm(float U, float acFreq, out bool[] result1, out bool[] result2, out string[] resultDescriptions, out string[] beforeAlarmStates, out string[] afterAlarmStates, out string[] releaseBeforeAlarmStates, out string[] releaseAfterAlarmStates);

        public abstract bool ClearEnergy(float U, float acFreq, out bool[] results, out string[] resultDescriptions, out string[] berforEnergys, out string[] afterEnergys);

        public abstract bool ClearDemand(float U, float acFreq, out bool[] results, out string[] resultDescriptions, out string[] berforDemands, out string[] afterDemands);

        public abstract bool TimeSync(float U, float acFreq, out bool[] results, out string[] resultDescriptions, out string[] berforTimes, out string[] afterTimes);

        public abstract bool ChangePassword02(float U, float acFreq, string oldPassword, string newPassword, out bool[] results, out string[] resultDescriptions);

        public abstract bool ChangePassword04(float U, float acFreq, string oldPassword, string newPassword, out bool[] results, out string[] resultDescriptions);

        public abstract bool CommunicationTest(float U, float acFreq, out bool[] results, out string[] resultDescriptions);

        public abstract bool EventRecord(float U, float acFreq, out bool[] results, out string[] resultDescriptions);

        public abstract bool PublicKeyAuthentication(float U, float acFreq, out bool[] results, out string[] resultDescriptions);

        public abstract bool PrivateKeyAuthentication(float U, float acFreq, out bool[] results, out string[] resultDescriptions);

        public abstract bool FreezeByTime(float U, float acFreq, out bool[] results, out string[] resultDescriptions, out string[] lastFreezenEnergys, out string[] currentEnergys, out string[] freezenEnergys);

        public abstract bool FreezeByInstantaneous(float U, float acFreq, out bool[] results, out string[] resultDescriptions, out string[] lastFreezenEnergys, out string[] currentEnergys, out string[] freezenEnergys1, out string[] freezenEnergys2, out string[] freezenEnergys3);

        public abstract bool FreezeByDay(float U, float acFreq, out bool[] results, out string[] resultDescriptions, out string[] lastFreezenEnergys, out string[] currentEnergys, out string[] freezenEnergys);

        public abstract bool FreezeByHour(float U, float acFreq, out bool[] results, out string[] resultDescriptions, out string[] lastFreezenEnergys, out string[] currentEnergys, out string[] freezenEnergys);

        public abstract bool CarrieWaveReturn(float U, float acFreq, string dataID, out bool[] results, out string[] resultDescriptions, out string[] datas);

        public abstract bool CarrieWaveReliability(float U, float acFreq, string dataID, int times, out string[] rates);

        public abstract bool CarrieWaveSuccessRate(float U, float acFreq, string dataID, int times, int interval, out string[] rates);

        public abstract bool ProtocolConsistency(float U, float acFreq, string dataID, int length, int digital, bool readWrite, string content, out bool[] results, out string[] datas);

        public abstract float CalcBasicErrorTime(float U, float I, float factor, bool capacitive, float acFreq, string pulse, int circle, int count);

        public abstract bool WarmUp(float U, float I, float acFreq, int second);

        public abstract bool ExecuteClockError(float U, float acFreq, float clockFreq, int second, int count, ReturnSampleDataErrDelegate returnSampleDatas);

        public abstract bool ReadMeterAddress(float U, float acFreq, out string[] addresses);

        public abstract bool ExecuteMeterdoubling(byte Beilv);

        public abstract bool DownParaToMeter(float U, float acFreq, string Item_TxFrame, string Item_RxFrame, out bool[] meterResult, out string[] meterRx);

        public abstract void Lighten(Color color, bool flicker);

        public abstract float CalcWarmUpTime(float U, float I, float acFreq, int second);

        public abstract float CalcBasicErrorTime(float U, float I, float acFreq, string phase, float factor, bool capacitive, Pulse pulse, int circle, int count);

        public abstract float CalcStartupTime(float U, float I, float acFreq, int second);

        public abstract float CalcLatentTime(float U, float acFreq, int second);

        public abstract float CalcClockErrorTime(float U, float acFreq, float clockFreq, int second, int count);

        public abstract float CalcEnergyReadingTime(float U, float I, float acFreq, float factor, bool capacitive, Pulse pulse, Phase phase, float energy);

        public abstract float CalcDemandTime(float U, float I, float acFreq);

        public abstract float CalcSwitchChangeTime(float U, float I, float acFreq);

        public abstract float CalcGetEnergyReadingTime(float U, float acFreq, Pulse pulse);

        public abstract float CalcGetDemandReadingTime(float U, float acFreq, Pulse pulse);

        public abstract float CalcReadMeterAddressTime(float U, float acFreq);

        public abstract float CalcHoldPowerTime(float U, float acFreq);

        public abstract float CalcHoldPowerCommandTime(float U, float acFreq);

        public abstract float CalcReleasePowerCommandTime(float U, float acFreq);

        public abstract float CalcRemoteOpenAccountTime(float U, float acFreq, double money, int count);

        public abstract float CalcRemoteChangeAccountTime(float U, float acFreq, double money, int count);

        public abstract float CalcRemoteSecretKeyUpdateTime(float U, float acFreq, bool isRemote);

        public abstract float CalcRemoteParameterUpdateTime(float U, float acFreq);

        public abstract float CalcRemoteDataReturnTime(float U, float acFreq);

        public abstract float CalcRemoteKnifeSwitchTime(float U, float acFreq);

        public abstract float CalcRemoteOpenSwitchCommandTime(float U, float acFreq);

        public abstract float CalcRemoteCloseSwitchCommandTime(float U, float acFreq);

        public abstract float CalcAlarmTime(float U, float acFreq);

        public abstract float CalcClearEnergyTime(float U, float acFreq);

        public abstract float CalcClearDemandTime(float U, float acFreq);

        public abstract float CalcTimeSyncTime(float U, float acFreq);

        public abstract float CalcChangePasswordTime(float U, float acFreq);

        public abstract float CalcCommunicationTestTime(float U, float acFreq);

        public abstract float CalcEventRecordTime(float U, float acFreq);

        public abstract float CalcPublicKeyAuthenticationTime(float U, float acFreq);

        public abstract float CalcPrivateKeyAuthenticationTime(float U, float acFreq);

        public abstract float CalcFreezeByTimeTime(float U, float acFreq);

        public abstract float CalcFreezeByInstantaneousTime(float U, float acFreq);

        public abstract float CalcFreezeByDayTime(float U, float acFreq);

        public abstract float CalcFreezeByHourTime(float U, float acFreq);

        public abstract float CalcCarrieWaveReturnTime(float U, float acFreq, string dataID);

        public abstract float CalcCarrieWaveReliabilityTime(float U, float acFreq, string dataID, int times);

        public abstract float CalcCarrieWaveSuccessRateTime(float U, float acFreq, string dataID, int times, int interval);

        public abstract float CalcProtocolConsistencyTime(float U, float acFreq, string dataID, int length, int digital, bool readWrite, string content);

        public abstract bool GetMeterTime(float U, float acFreq, out string[] times);

        public abstract void ExecutJcjjdItem(float UA, float IA, float Afactor, bool Acapacitive, float UB, float IB, float Bfactor, bool Bcapacitive,
            float UC, float IC, float Cfactor, bool Ccapacitive, VerificationEquipment.Commons.WiringMode wiringMode, Pulse pulse);

        public abstract void ExecutFenXiangPower(float UA, float UB, float UC, WiringMode wiringMode, float factor, bool capacitive, Pulse pulse);

        #endregion

        #region IEquipment 新增内容

        public virtual void InitConnect(int meterPositionCount)
        {

        }

        public virtual string GetEquipmentStatus()
        {
            return string.Empty;
        }

        public virtual void EquipmentPress(bool isPress, out bool[] results, out string[] resultDescriptions)
        {
            results = new bool[MeterPositionCount];
            resultDescriptions = new string[MeterPositionCount];
        }

        public virtual void EquipmentReversal(bool isReversal, out bool[] results, out string[] resultDescriptions)
        {
            results = new bool[MeterPositionCount];
            resultDescriptions = new string[MeterPositionCount];
        }

        public virtual void GetCurrentMeterNoStatus(out int[] meterNo, out string[] status)
        {
            meterNo = new int[MeterPositionCount];
            status = new string[MeterPositionCount];
        }

        public virtual bool ExecuteConnectionCheck(float U, float I, float acFreq, out string[] checkResults)
        {
            checkResults = new string[MeterPositionCount];
            return true;
        }

        public virtual bool ExecuteResistance(float resistanceU, float resistanceI, int resistanceTime, string resistanceType, out bool[] results, out string[] resultDescriptions)
        {
            results = new bool[MeterPositionCount];
            resultDescriptions = new string[MeterPositionCount];
            return true;
        }

        public virtual float CalcConnectionCheckTime(float U, float I, float acFreq)
        {
            return 1.0f;
        }

        public virtual float CalcResistanceTime(float resistanceU, float resistanceI, int resistanceTime, string resistanceType)
        {
            return 1.0f;
        }

        /// <summary>
        /// 获取检定装置表位压接状态
        /// </summary>
        /// <param name="meterNo">返回表位列表，对应检定台表位</param>
        /// <param name="pressStatus">返回表位压接状态</param>
        /// <returns></returns>
        public virtual void GetEquipmentPressStatus(out int[] meterNo, out MeterPositionPressStatus[] pressStatus)
        {
            meterNo = new int[MeterPositionCount];
            pressStatus = new MeterPositionPressStatus[MeterPositionCount];
        }

        /// <summary>
        /// 获取检定装置表位翻转状态
        /// </summary>
        /// <param name="meterNo">返回表位列表，对应检定台表位</param>
        /// <param name="reverseStatus">返回表位翻转状态</param>
        /// <returns></returns>
        public virtual void GetEquipmentReversalStatus(out int[] meterNo, out MeterPositionReverseStatus[] reverseStatus)
        {
            meterNo = new int[MeterPositionCount];
            reverseStatus = new MeterPositionReverseStatus[MeterPositionCount];
        }

        /// <summary>
        /// 设备手工压接
        /// </summary>
        /// <param name="isPress">true：压紧 false：松开</param>
        /// <param name="results">结论</param>
        /// <param name="resultDescriptions">结论描述</param>
        public virtual void EquipmentManualPress(int meterPositionCount, bool isPress, out bool[] results, out string[] resultDescriptions)
        {
            results = new bool[MeterPositionCount];
            resultDescriptions = new string[MeterPositionCount];
        }

        /// <summary>
        /// 设置手工翻转
        /// </summary>
        /// <param name="isReversal">true：正转 false：反转</param>
        /// <param name="results">结论</param>
        /// <param name="resultDescriptions">结论描述</param>
        public virtual void EquipmentManualReversal(int meterPositionCount, bool isReversal, out bool[] results, out string[] resultDescriptions)
        {
            results = new bool[MeterPositionCount];
            resultDescriptions = new string[MeterPositionCount];
        }

        /// <summary>
        /// 设置单表位手工压接
        /// </summary>
        /// <param name="meterPosition">表位</param>
        /// <param name="isPress">true：压紧 false：松开</param>
        /// <param name="results">结论</param>
        /// <param name="resultDescriptions">结论描述</param>
        public virtual void EquipmentMeterPositionManualPress(int meterPositionCount, int[] meterPosition, bool isPress, out bool[] results, out string[] resultDescriptions)
        {
            results = new bool[20];
            resultDescriptions = new string[20];
        }

        /// <summary>
        /// 设置单表位手工翻转
        /// 说明：由于翻转板是一控多状态，因此翻转表位事实上是翻转表位所在翻转板
        /// </summary>
        /// <param name="meterPosition">表位</param>
        /// <param name="isReversal">true：正转 false：反转</param>
        /// <param name="results">结论</param>
        /// <param name="resultDescriptions">结论描述</param>
        public virtual void EquipmentMeterPositionManualReversal(int meterPositionCount, int[] meterPosition, bool isReversal, out bool[] results, out string[] resultDescriptions)
        {
            results = new bool[20];
            resultDescriptions = new string[20];
        }
        /// <summary>
        /// 电流开路检测
        /// </summary>
        /// <param name="current"></param>
        /// <param name="resutls"></param>
        public virtual void ExecuteCurrentTest(float current, out bool[] results)
        {
            results = new bool[MeterPositionCount];
        }

        public virtual void ExecuteSecondPress(int[] meterPositions, out bool[] results)
        {
            results = new bool[MeterPositionCount];
        }

        public virtual bool CloseVoltageCurrent()
        {
            return true;
        }

        public virtual bool ExecuteDemandValue(float U, float I, float acFreq, out string stdDemand, out string[] checkedDemands)
        {
            stdDemand = string.Empty;
            checkedDemands = new string[MeterPositionCount];
            return true;
        }

        public virtual bool ExecuteDemandPeriod(float U, float I, float acFreq, out string[] demandCycleErrors)
        {
            demandCycleErrors = new string[MeterPositionCount];
            return true;
        }

        public virtual void CurrentShout(int[] meterPositions, out bool[] results)
        {
            results = new bool[MeterPositionCount];
        }
        #endregion

        #region IEquipment 检定项合并执行

        public virtual bool ExecuteLatentClockError(LatentClockError latentClockError, out string[] latentTimes, ReturnSampleDatasDelegate returnSampleDatas)
        {
            latentTimes = new string[MeterPositionCount];
            return true;
        }

        #endregion
    }
}
