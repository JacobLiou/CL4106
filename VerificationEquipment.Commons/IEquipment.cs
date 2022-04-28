using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace VerificationEquipment.Commons
{
    public interface IEquipment : IDisposable
    {
        #region 设备相关
        /// <summary>
        /// 表位装表
        /// 在每次试验开始前，定义设备上电能表的挂接情况
        /// 每个MeterPosition对象对应一个表位
        ///     meterPositions数组长度与检定装置表位数量一致
        ///     meterPositions元素均不为null
        ///     meterPosition.Index属性表示表位，从1开始编码
        ///     meterPosition.Meter属性表示挂接的电能表参数信息，如果表位未挂表，该属性为null
        ///     meterPosition.IsVerify属性表示该表位是否检定，true：检定；false：不检定
        /// </summary>
        /// <param name="meterPositions">检定装置挂接的表位参数定义</param>
        void InstallMeter(MeterPosition[] meterPositions);

        /// <summary>
        /// 设备联机
        /// 设备供应商根据检定装置结构组成，分别对各设备部件进行初始化操作，使其具备检定控制条件
        /// </summary>
        /// <exception cref="ConnectException">联机异常</exception>
        /// <exception cref="Open485Exception">打开485端口异常，上层调用可以选择继续试验</exception>
        void Connect();

        /// <summary>
        /// 初始化串口连接
        /// </summary>
        /// <exception cref="ConnectException">联机异常</exception>
        /// <exception cref="Open485Exception">打开485端口异常，上层调用可以选择继续试验</exception>
        void InitConnect(int meterPositionCount);

        /// <summary>
        /// 用户停止检定试验，调用该函数后要经过一定延时才能真正将硬件设备停下，在以下情况时会调用该方法：
        ///     1.用户切换校验点时发出该命令停止当前正在检定的校验点，执行用户选择的校验点
        ///     2.用户停止检定试验时发出该命令停止当前正在检定的校验点，然后执行脱机命令
        ///     3.设备层发生异常时调用该方法，停止当前正在检定的校验点，然后执行脱机命令
        /// </summary>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        void Abort();

        /// <summary>
        /// 脱机
        /// </summary>
        void Close();

        /// <summary>
        /// 输出日志信息
        /// </summary>
        /// <param name="message">信息内容</param>
        void OnInfo(string message);

        /// <summary>
        /// 获取检定台状态
        /// </summary>
        /// <returns>
        /// “00”:当前检定台处于故障状态，不允许进行操作。
        /// “01”:表示当前检定台处于待机状态，此时可以进行挂表操作。
        /// “02”:表示当前处于待压接状态，可以进行压接或下表操作。
        /// “03”:表示当前处于压接状态，可以进行翻转或松开压接操作。
        /// “04”:表示当前处于待检定状态，可以进行检定、或翻转操作。
        /// “05”:表示当前处于检定状态，不允许进行翻转、松开压接操作。
        /// </returns>
        string GetEquipmentStatus();

        /// <summary>
        /// 表位压接
        /// </summary>
        /// <param name="isPress">true表示进行电能表压接，false表示松开压接</param>
        /// <param name="results">每一个表位的压接情况，true表示压接成功，false表示压接失败</param>
        /// <param name="resultDescriptions">电能表压接情况描述，数组长度与result一致，对应每一个表位的压接情况描述</param>
        void EquipmentPress(bool isPress, out bool[] results, out string[] resultDescriptions);

        /// <summary>
        /// 表位翻转
        /// </summary>
        /// <param name="isReversal">true表示翻转成检定状态，false表示翻转成挂表状态</param>
        /// <param name="results">表位翻转情况，true表示翻转成功，false表示翻转失败</param>
        /// <param name="resultDescriptions">电能表表位翻转情况描述，数组长度与result一致，对应每一个表位的翻转情况</param>
        void EquipmentReversal(bool isReversal, out bool[] results, out string[] resultDescriptions);

        /// <summary>
        /// 读检定装置表位状态
        /// </summary>
        /// <param name="meterNo">返回表位列表，对应检定台表位</param>
        /// <param name="status">返回表位状态，”00”为无表；”01”为有表</param>
        void GetCurrentMeterNoStatus(out int[] meterNo, out string[] status);

        event EventHandler<EquipmentMessageEventArgs> Info;

        #endregion

        #region 自动检定新增
        /// <summary>
        /// 获取检定装置表位压接状态
        /// </summary>
        /// <param name="meterNo">返回表位列表，对应检定台表位</param>
        /// <param name="pressStatus">返回表位压接状态</param>
        /// <returns></returns>
        void GetEquipmentPressStatus(out int[] meterNo, out MeterPositionPressStatus[] pressStatus);

        /// <summary>
        /// 获取检定装置表位翻转状态
        /// </summary>
        /// <param name="meterNo">返回表位列表，对应检定台表位</param>
        /// <param name="reverseStatus">返回表位翻转状态</param>
        /// <returns></returns>
        void GetEquipmentReversalStatus(out int[] meterNo, out MeterPositionReverseStatus[] reverseStatus);

        /// <summary>
        /// 表位手工压接
        /// </summary>
        /// <param name="isPress">true表示进行电能表压接，false表示松开压接</param>
        /// <param name="results">每一个表位的压接情况，true表示压接成功，false表示压接失败</param>
        /// <param name="resultDescriptions">电能表压接情况描述，数组长度与result一致，对应每一个表位的压接情况描述</param>
        void EquipmentManualPress(int meterPositionCount, bool isPress, out bool[] results, out string[] resultDescriptions);

        /// <summary>
        /// 表位手工翻转
        /// </summary>
        /// <param name="isReversal">true表示翻转成检定状态，false表示翻转成挂表状态</param>
        /// <param name="results">表位翻转情况，true表示翻转成功，false表示翻转失败</param>
        /// <param name="resultDescriptions">电能表表位翻转情况描述，数组长度与result一致，对应每一个表位的翻转情况</param>
        void EquipmentManualReversal(int meterPositionCount, bool isReversal, out bool[] results, out string[] resultDescriptions);

        /// <summary>
        /// 单个表位手工压接
        /// </summary>
        /// <param name="meterPostion">表位号</param>
        /// <param name="isPress">true表示进行电能表压接，false表示松开压接</param>
        /// <param name="results">每一个表位的压接情况，true表示压接成功，false表示压接失败</param>
        /// <param name="resultDescriptions">电能表压接情况描述，数组长度与result一致，对应每一个表位的压接情况描述</param>
        void EquipmentMeterPositionManualPress(int meterPositionCount, int[] meterPosition, bool isPress, out bool[] results, out string[] resultDescriptions);

        /// <summary>
        /// 单个表位手工翻转
        /// </summary>
        /// <param name="meterPostion">表位号</param>
        /// <param name="isReversal">true表示翻转成检定状态，false表示翻转成挂表状态</param>
        /// <param name="results">表位翻转情况，true表示翻转成功，false表示翻转失败</param>
        /// <param name="resultDescriptions">电能表表位翻转情况描述，数组长度与result一致，对应每一个表位的翻转情况</param>
        void EquipmentMeterPositionManualReversal(int meterPositionCount, int[] meterPosition, bool isReversal, out bool[] results, out string[] resultDescriptions);

        /// <summary>
        /// 设置检定台体表位数
        /// </summary>
        /// <param name="count"></param>
        void SetMeterPositionCount(int count);

        /// <summary>
        /// 电流开路检测
        /// </summary>
        /// <param name="current"></param>
        /// <param name="resutls"></param>
        void ExecuteCurrentTest(float current, out bool[] results);

        /// <summary>
        /// 重压接
        /// </summary>
        /// <param name="meterPositions"></param>
        /// <param name="results"></param>
        void ExecuteSecondPress(int[] meterPositions, out bool[] results);
        #endregion

        #region 检定项目

        /// <summary>
        /// 关闭电压电流
        /// </summary>
        /// <returns></returns>
        bool CloseVoltageCurrent();

        /// <summary>
        /// 预热
        /// </summary>
        /// <param name="U">预热电压，单位V</param>
        /// <param name="I">预热电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="second">预热时间，单位秒</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool WarmUp(float U, float I, float acFreq, int second);

        /// <summary>
        /// 基本误差试验，合元
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="I">电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="factor">功率因数</param>
        /// <param name="capacitive">容性标志，true表示容性，false表示感性</param>
        /// <param name="pulse">脉冲类型，正向有功、反向有功、正向无功、反向无功</param>
        /// <param name="circle">检验圈数</param>
        /// <param name="count">检验次数</param>
        /// <param name="returnSampleDatas">误差结果的回调句柄。根据检验次数，每产生一次误差值，通过该回调句柄上上层系统返回误差值</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool ExecuteBasicError(float U, float I, float acFreq, float factor, bool capacitive, Pulse pulse,
            int circle, int count, ReturnSampleDatasDelegate returnSampleDatas);

        /// <summary>
        /// 基本误差试验，合元 带误差限
        /// </summary>
        /// <param name="U"></param>
        /// <param name="I"></param>
        /// <param name="acFreq"></param>
        /// <param name="factor"></param>
        /// <param name="capacitive"></param>
        /// <param name="pulse"></param>
        /// <param name="circle"></param>
        /// <param name="count"></param>
        /// <param name="fMax"></param>
        /// <param name="fMin"></param>
        /// <param name="returnSampleDatas"></param>
        /// <returns></returns>
        bool ExecuteBasicError(float U, float I, float acFreq, float factor, bool capacitive, Pulse pulse,int meterConst,
            int circle, int count, float fMax, float fMin, ReturnSampleDataErrDelegate returnSampleDatas);

        /// <summary>
        /// 基本误差试验，分元
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
        /// <param name="returnSampleDatas">误差结果的回调句柄。根据检验次数，每产生一次误差值，通过该回调句柄上上层系统返回误差值</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool ExecuteBasicError(float U, float I, float acFreq, string phase, float factor, bool capacitive,
            Pulse pulse, int circle, int count, ReturnSampleDatasDelegate returnSampleDatas);



        /// <summary>
        /// 分元带误差限
        /// </summary>
        /// <param name="U"></param>
        /// <param name="I"></param>
        /// <param name="acFreq"></param>
        /// <param name="phase"></param>
        /// <param name="factor"></param>
        /// <param name="capacitive"></param>
        /// <param name="pulse"></param>
        /// <param name="circle"></param>
        /// <param name="count"></param>
        /// <param name="returnSampleDatas"></param>
        /// <returns></returns>
        bool ExecuteBasicError(float U, float I, float acFreq, string phase, float factor, bool capacitive,
            Pulse pulse, int meterConst, int circle, int count, float fMax, float fMin, ReturnSampleDataErrDelegate returnSampleDatas);
        /// <summary>
        /// 启动试验
        /// 设备供应商根据试验参数，返回被实验电能表的实际启动时间（单位：秒）。由应用系统判断试验结论
        /// </summary>
        /// <param name="U">启动电压，单位V</param>
        /// <param name="I">启动电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="second">启动试验时间，单位秒</param>
        /// <param name="startTime">返回被实验电能表的实际启动时间，格式：浮点数字符串，如×××.××，单位：秒。如果被实验电能表未启动，相应的表位返回null</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool ExecuteStartup(float U, float I, float acFreq, int second, out string[] startTimes);

        /// <summary>
        /// 潜动试验
        /// 设备供应商根据试验参数，返回被实验电能表的实际潜动时间（单位：秒）。由应用系统判断试验结论
        /// </summary>
        /// <param name="U">潜动电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="second">潜动试验时间，单位：秒</param>
        /// <param name="latentTimes">返回被实验电能表的实际潜动（收到脉冲信号）时间，格式：浮点数字符串，如×××.××，单位：秒。如果被实验电能表未收到脉冲，相应的表位返回null</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool ExecuteLatent(float U, float acFreq, int second, out string[] latentTimes);

        /// <summary>
        /// 日计时试验
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="clockFreq">被检表时钟脉冲频率，默认1Hz</param>
        /// <param name="second">检验时间，单位：秒</param>
        /// <param name="count">检验次数</param>
        /// <param name="returnSampleDatas">误差结果的回调句柄。根据检验次数，每产生一次误差值，通过该回调句柄上上层系统返回误差值</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool ExecuteClockError(float U, float acFreq, float clockFreq, int second, int count,
            ReturnSampleDataErrDelegate returnSampleDatas);

        /// <summary>
        /// 走字和校核计度器试验
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="I">试验电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="factor">功率因数</param>
        /// <param name="capacitive">容性标志，true表示容性，false表示感性</param>
        /// <param name="pulse">脉冲类型，正向有功、反向有功、正向无功、反向无功</param>
        /// <param name="phase">费率</param>
        /// <param name="energy">走字电能(kWh)</param>
        /// <param name="stdEnergy">返回标准表走过的电能值(kWh)</param>
        /// <param name="initialTotalReading">返回总计度器的走字始度(kWh)</param>
        /// <param name="initialSubReading">返回分费率计度器的走字始度(kWh)</param>
        /// <param name="finalTotalReading">返回总计度器的走字止度(kWh)</param>
        /// <param name="finalSubReading">分费率计度器的走字止度(kWh)</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        /// <exception cref="NotInstalledException">所操作的表位未挂表</exception>
        bool ExecuteEnergyReading(float U, float I, float acFreq, float factor, bool capacitive,
            Pulse pulse, Phase phase, float energy, out string stdEnergy,
            out string[] initialTotalReading, out string[] initialSubReading,
            out string[] finalTotalReading, out string[] finalSubReading);

        /// <summary>
        /// 需量周期、需量示值
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="I">试验电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="stdDemand">返回标准表需量示值</param>
        /// <param name="checkedDemands">返回被检表需量示值</param>
        /// <param name="demandCycleError">返回被检表需量周期误差</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool ExecuteDemand(float U, float I, float acFreq, out string stdDemand, out string[] checkedDemands,
            out string[] demandCycleErrors);

        /// <summary>
        /// 需量示值
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="I">试验电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="stdDemand">返回标准表需量示值</param>
        /// <param name="checkedDemands">返回被检表需量示值</param>
        /// <returns></returns>
        bool ExecuteDemandValue(float U, float I, float acFreq, out string stdDemand, out string[] checkedDemands);

        /// <summary>
        /// 需量周期试验
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="I">试验电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="demandCycleError">返回被检表需量周期误差</param>
        /// <returns></returns>
        bool ExecuteDemandPeriod(float U, float I, float acFreq, out string[] demandCycleErrors);

        /// <summary>
        /// 时段投切试验
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="I">试验电流，单位A，默认300%Ib</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="period">返回从电能表中读取的各时段的值，如读取到的值为峰、谷，则返回数组长度为2，period[0]为2，period[1]为4。</param>
        /// <param name="periodTime">返回电能表中各时段对应的时间，长度与返回的时段数组长度一致且与上面的时段一一对应，格式为hh:mm，
        ///  如上面描述的峰、谷时段的时间为8:00、21:00，则返回值为periodTime[0]为8:00，periodTime[1]为21:00</param>
        /// <param name="valueErrors">根据电能表返回的时段，返回各费率时段的示值误差。
        /// 如有两个时段，0:00-8:00谷，8:00-21:00峰，21:00-24:00谷，需要返回两组试验结果，其中
        ///     valueErrors[0][..]表示“谷到峰”的误差集合
        ///     valueErrors[1][..]表示“峰到谷”的误差集合
        /// 如有四个时段，0:00-8:00谷，8:00-12:00峰，12:00-17:00平，17:00-21:00峰，21:00-24:00谷，需要返回四组试验结果，其中
        ///     valueErrors[0][..]表示“谷到峰”的误差集合
        ///     valueErrors[1][..]表示“峰到平”的误差集合
        ///     valueErrors[2][..]表示“平到峰”的误差集合
        ///     valueErrors[3][..]表示“峰到谷”的误差集合
        /// <param name="combinErrors">返回组合误差</param>
        /// <param name="changeErrors">返回投切误差，二维数组定义参照valueErrors</param>
        /// <param name="changeTimes">返回投切时间，格式为HH:mm:ss.ms，如08:00:01.123，二维数组定义参照valueErrors</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool ExecuteSwitchChange(float U, float I, float acFreq, out int[] period, out string[] periodTime, out string[][] valueErrors, out string[] combinErrors,
            out string[][] changeErrors, out string[][] changeTimes);

        /// <summary>
        /// 读电能底数
        /// 驱动供应商自行根据电能表接线方式（单相、三相三线、三相四线）判断读数时段，如单相表，只读取总、峰、谷
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="pulse">脉冲类型，正向有功、反向有功、正向无功、反向无功</param>
        /// <param name="energyValues">返回电能读数结果</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool GetEnergyReading(float U, float acFreq, Pulse pulse, out EnergyValue[] energyValues);

        /// <summary>
        /// 读需量底数
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="pulse">脉冲类型，正向有功、反向有功、正向无功、反向无功</param>
        /// <param name="demands">返回需量底数</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool GetDemandReading(float U, float acFreq, Pulse pulse, out string[] demands);

        /// <summary>
        /// 读取表地址
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="addresses">返回表地址</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool ReadMeterAddress(float U, float acFreq, out string[] addresses);
        /// <summary>
        /// 打包参数下载
        /// </summary>
        /// <param name="U">电压</param>
        /// <param name="acFreq">频率</param>
        /// <param name="Item_TxFrame">发送数据</param>
        /// <param name="Item_RxFrame">返回数据</param>
        /// <param name="meterResult">返回结论</param>
        /// <returns></returns>
        bool DownParaToMeter(float U, float acFreq, string Item_TxFrame, string Item_RxFrame, out bool[] meterResult, out string [] meterRx);

        /// <summary>
        /// 读检定装置当前负载
        /// </summary>
        /// <param name="U">返回电压(单位：V)，长度为3的数组，U[0]表示A相，U[1]表示B相，U[2]表示C相，对于单向检定设备，U[0]表示电压，U[1],U[2]为null</param>
        /// <param name="I">返回电流(单位：A)，数组定义参考U</param>
        /// <param name="P">返回有功功率(单位：kW)，数组定义参考U</param>
        /// <param name="Q">返回无功功率(单位：kVar)，数组定义参考U</param>
        /// <param name="angle">返回电流相位角，以Ua为基准</param>
        /// <param name="acFreq">返回交流电频率(Hz)</param>
        void GetCurrentLoad(out string[] U, out string[] I, out string[] P, out string[] Q,
            out string[] angle, out string acFreq);

        /// <summary>
        /// 表位自动短接
        /// </summary>
        /// <param name="meterPositions"></param>
        /// <param name="results"></param>
        void CurrentShout(int[] meterPositions, out bool[] results);

        /// <summary>
        /// 点亮检验台的运行状态灯
        /// 由供应商根据硬件实现方式保存灯的状态；在收到下一个状态变更命令前，要维持原有状态
        /// </summary>
        /// <param name="color">状态灯颜色，目前支持，Color.Red:红，Color.Green:绿，Color.Yellow:黄</param>
        /// <param name="flicker">闪烁标志，true表示闪烁，false表示常亮</param>
        void Lighten(Color color, bool flicker);

        /// <summary>
        /// 保电试验
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="beforeStates">返回保电前状态字</param>
        /// <param name="afterStates">返回保电后状态字</param>
        /// <param name="releaseBeforeStates">返回解除保电前状态字</param>
        /// <param name="releaseAfterStates">返回解除保电后状态字</param>
        /// <param name="results">返回保电试验结果</param>
        /// <param name="resultDescriptions">返回保电试验结果描述</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool HoldPower(float U, float acFreq, out string[] beforeStates, out string[] afterStates,
            out string[] releaseBeforeStates, out string[] releaseAfterStates,
            out bool[] results, out string[] resultDescriptions);

        /// <summary>
        /// 保电命令
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="beforeStates">返回保电前状态字</param>
        /// <param name="afterStates">返回保电后状态字</param>
        /// <param name="results">返回保电命令结果</param>
        /// <param name="resultDescriptions">返回保电命令结果描述</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool HoldPowerCommand(float U, float acFreq, out string[] beforeStates,
            out string[] afterStates, out bool[] results, out string[] resultDescriptions);

        /// <summary>
        /// 解除保电命令
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="releaseBeforeStates">返回解除保电前状态字</param>
        /// <param name="releaseAfterStates">返回解除保电后状态字</param>
        /// <param name="results">返回解除保电命令结果</param>
        /// <param name="resultDescriptions">返回解除保电命令结果描述</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool ReleasePowerCommand(float U, float acFreq, out string[] releaseBeforeStates,
            out string[] releaseAfterStates, out bool[] results, out string[] resultDescriptions);

        /// <summary>
        /// 远程开户充值
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="money">充值金额</param>
        /// <param name="count">充值次数</param>
        /// <param name="results">返回远程开户充值结果</param>
        /// <param name="resultDescriptions">返回远程开户充值结果描述</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool RemoteOpenAccount(float U, float acFreq, double money, int count,
            out bool[] results, out string[] resultDescriptions);

        /// <summary>
        /// 远程充值
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="money">充值金额</param>
        /// <param name="count">充值次数</param>
        /// <param name="results">返回远程充值结果</param>
        /// <param name="resultDescriptions">返回远程充值结果描述</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool RemoteChangeAccount(float U, float acFreq, double money, int count,
            out bool[] results, out string[] resultDescriptions);

        /// <summary>
        /// 远程密钥更新
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="isRemote">远程表或本地表，true:远程表，false:本地表</param>
        /// <param name="results">返回远程密钥更新结果</param>
        /// <param name="resultDescriptions">返回远程密钥更新结果描述</param>
        /// <param name="beforeInfos">返回密钥更新后信息</param>
        /// <param name="afterInfos">返回密钥更新前信息</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool RemoteSecretKeyUpdate(float U, float acFreq, bool isRemote, out bool[] results,
            out string[] resultDescriptions, out string[] beforeInfos, out string[] afterInfos);

        /// <summary>
        /// 远程参数修改，参数由供应商自行设置
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="results">返回远程参数修改结果</param>
        /// <param name="resultDescriptions">返回远程参数修改结果描述</param>
        /// <returns> true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool RemoteParameterUpdate(float U, float acFreq, out bool[] results, out string[] resultDescriptions);

        /// <summary>
        /// 远程数据回抄，回抄信息，参数由供应商自行设置
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="result1">返回本地密钥信息结论</param>
        /// <param name="result2">返回运行信息文件结论</param>
        /// <param name="resultDescriptions">返回远程数据回抄结果描述</param>
        /// <param name="returnInfo1">返回本地密钥信息回抄数据信息</param>
        /// <param name="returnInfo2">返回运行信息文件回抄数据信息</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool RemoteDataReturn(float U, float acFreq, out bool[] result1, out bool[] result2,
            out string[] resultDescriptions, out string[] returnInfo1, out string[] returnInfo2);

        /// <summary>
        /// 远程拉合闸
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="openResults">返回拉闸结论</param>
        /// <param name="closeResults">返回合闸结论</param>
        /// <param name="resultDescriptions">返回远程拉合闸结果描述</param>
        /// <param name="beforeOpenStates">返回拉闸前状态字</param>
        /// <param name="afterOpenStates">返回拉闸后状态字</param>
        /// <param name="beforeCloseStates">返回合闸前状态字</param>
        /// <param name="afterCloseStaes">返回合闸后状态字</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool RemoteKnifeSwitch(float U, float acFreq, out bool[] openResults, out bool[] closeResults, out string[] resultDescriptions,
            out string[] beforeOpenStates, out string[] afterOpenStates,
            out string[] beforeCloseStates, out string[] afterCloseStaes);

        /// <summary>
        /// 远程拉闸命令
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="results">返回远程拉闸结论</param>
        /// <param name="resultDescriptions">返回远程拉闸结论描述</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool RemoteOpenSwitchCommand(float U, float acFreq, out bool[] results, out string[] resultDescriptions);

        /// <summary>
        /// 远程合闸命令
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="results">返回远程合闸结论</param>
        /// <param name="resultDescriptions">返回远程合闸结论描述</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool RemoteCloseSwitchCommand(float U, float acFreq, out bool[] results, out string[] resultDescriptions);

        /// <summary>
        /// 报警试验
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="result1">返回报警结论</param>
        /// <param name="result2">返回解除报警结论</param>
        /// <param name="resultDescriptions">返回报警试验结论描述</param>
        /// <param name="beforeAlarmStates">返回报警前状态字</param>
        /// <param name="afterAlarmStates">返回报警后状态字</param>
        /// <param name="releaseBeforeAlarmStates">返回解除报警前状态字</param>
        /// <param name="releaseAfterAlarmStates">返回解除报警前状态字</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool Alarm(float U, float acFreq, out bool[] result1, out bool[] result2, out string[] resultDescriptions,
            out string[] beforeAlarmStates, out string[] afterAlarmStates,
            out string[] releaseBeforeAlarmStates, out string[] releaseAfterAlarmStates);

        /// <summary>
        /// 清电量
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="results">返回清电量结论</param>
        /// <param name="resultDescriptions">返回清电量结论描述</param>
        /// <param name="berforEnergys">返回清零前正向有功总电量</param>
        /// <param name="afterEnergys">返回清零后正向有功总电量</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool ClearEnergy(float U, float acFreq, out bool[] results, out string[] resultDescriptions,
            out string[] berforEnergys, out string[] afterEnergys);

        /// <summary>
        /// 清需量
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="results">返回清需量结论</param>
        /// <param name="resultDescriptions">返回清需量结论描述</param>
        /// <param name="berforDemands">返回清需量前正向有功最大需量</param>
        /// <param name="afterDemands">返回清需量后正向有功最大需量</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool ClearDemand(float U, float acFreq, out bool[] results, out string[] resultDescriptions,
            out string[] berforDemands, out string[] afterDemands);

        /// <summary>
        /// 表计对时
        /// 供应商直接根据操作系统时间对时，操作系统时间准确性有上层系统保障
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="results">返回表计对时结论</param>
        /// <param name="resultDescriptions">返回表计对时结论描述</param>
        /// <param name="berforTimes">返回对时前电能表时间，格式yyyy-MM-dd HH:mm:ss，如2011-03-09 09:32:31</param>
        /// <param name="afterTimes">返回对时后电能表时间，格式yyyy-MM-dd HH:mm:ss，如2011-03-09 09:32:31</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool TimeSync(float U, float acFreq, out bool[] results, out string[] resultDescriptions,
            out string[] berforTimes, out string[] afterTimes);

        /// <summary>
        /// 修改02级密码
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="oldPassword">原密码</param>
        /// <param name="newPassword">新密码</param>
        /// <param name="results">返回修改密码结论</param>
        /// <param name="resultDescriptions">返回修改密码结论描述</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool ChangePassword02(float U, float acFreq, string oldPassword, string newPassword, out bool[] results, out string[] resultDescriptions);

        /// <summary>
        /// 修改04级密码
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="oldPassword">原密码</param>
        /// <param name="newPassword">新密码</param>
        /// <param name="results">返回修改密码结论</param>
        /// <param name="resultDescriptions">返回修改密码结论描述</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool ChangePassword04(float U, float acFreq, string oldPassword, string newPassword, out bool[] results, out string[] resultDescriptions);

        /// <summary>
        /// 通信测试
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="results">返回通信测试结论</param>
        /// <param name="resultDescriptions">返回通信测试结论描述</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool CommunicationTest(float U, float acFreq, out bool[] results, out string[] resultDescriptions);

        /// <summary>
        /// 事件记录
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="results">返回事件记录结论</param>
        /// <param name="resultDescriptions">返回事件记录结论描述</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool EventRecord(float U, float acFreq, out bool[] results, out string[] resultDescriptions);

        /// <summary>
        /// 公钥验证
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="results">返回公钥验证结论</param>
        /// <param name="resultDescriptions">返回公钥验证结论描述</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool PublicKeyAuthentication(float U, float acFreq, out bool[] results, out string[] resultDescriptions);

        /// <summary>
        /// 私钥验证
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="results">返回私钥验证结论</param>
        /// <param name="resultDescriptions">返回私钥验证结论描述</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool PrivateKeyAuthentication(float U, float acFreq, out bool[] results, out string[] resultDescriptions);

        /// <summary>
        /// 定时冻结
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="results">返回定时冻结结论</param>
        /// <param name="resultDescriptions">返回定时冻结结论描述</param>
        /// <param name="lastFreezenEnergys">返回上次定时冻结电量</param>
        /// <param name="currentEnergys">返回冻结时电量</param>
        /// <param name="freezenEnergys">返回冻结后电量</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool FreezeByTime(float U, float acFreq, out bool[] results, out string[] resultDescriptions,
            out string[] lastFreezenEnergys, out string[] currentEnergys, out string[] freezenEnergys);

        /// <summary>
        /// 瞬时冻结
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="results">返回瞬时冻结结论</param>
        /// <param name="resultDescriptions">返回瞬时冻结结论描述</param>
        /// <param name="lastFreezenEnergys">返回上一次瞬时冻结电量</param>
        /// <param name="currentEnergys">返回冻结时电量</param>
        /// <param name="freezenEnergys1">返回第一次瞬时冻结电量</param>
        /// <param name="freezenEnergys2">返回第二次瞬时冻结电量</param>
        /// <param name="freezenEnergys3">返回第三次瞬时冻结电量</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool FreezeByInstantaneous(float U, float acFreq, out bool[] results, out string[] resultDescriptions,
            out string[] lastFreezenEnergys, out string[] currentEnergys, out string[] freezenEnergys1,
            out string[] freezenEnergys2, out string[] freezenEnergys3);

        /// <summary>
        /// 日冻结
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="results">返回日冻结结论</param>
        /// <param name="resultDescriptions">返回日冻结结论描述</param>
        /// <param name="lastFreezenEnergys">返回上次日冻结电量</param>
        /// <param name="currentEnergys">返回冻结时电量</param>
        /// <param name="freezenEnergys">返回冻结后电量</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool FreezeByDay(float U, float acFreq, out bool[] results, out string[] resultDescriptions,
            out string[] lastFreezenEnergys, out string[] currentEnergys, out string[] freezenEnergys);

        /// <summary>
        /// 整点冻结
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="results">返回整点冻结结论</param>
        /// <param name="resultDescriptions">返回整点冻结结论描述</param>
        /// <param name="lastFreezenEnergys">返回上次整点冻结电量</param>
        /// <param name="currentEnergys">返回冻结时电量</param>
        /// <param name="freezenEnergys">返回冻结后电量</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool FreezeByHour(float U, float acFreq, out bool[] results, out string[] resultDescriptions,
            out string[] lastFreezenEnergys, out string[] currentEnergys, out string[] freezenEnergys);

        /// <summary>
        /// 载波召测
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="dataID">数据项标示</param>
        /// <param name="results">返回载波召测结论</param>
        /// <param name="resultDescriptions">返回载波召测结论描述</param>
        /// <param name="datas">返回载波召测数据</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool CarrieWaveReturn(float U, float acFreq, string dataID, out bool[] results,
            out string[] resultDescriptions, out string[] datas);

        /// <summary>
        /// 载波可靠性
        /// 由上层应用根据载波成功率判断试验结论
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="dataID">数据项标示</param>
        /// <param name="times">重发次数</param>
        /// <param name="rates">返回载波成功率</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool CarrieWaveReliability(float U, float acFreq, string dataID, int times, out string[] rates);

        /// <summary>
        /// 载波成功率
        /// 由上层应用根据载波成功率判断试验结论
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="dataID">数据项标示</param>
        /// <param name="times">重发次数</param>
        /// <param name="interval">抄读间隔时间</param>
        /// <param name="rates">返回载波成功率</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool CarrieWaveSuccessRate(float U, float acFreq, string dataID, int times, int interval, out string[] rates);

        /// <summary>
        /// 通信规约一致性检查
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="dataID">数据项标示</param>
        /// <param name="length">长度</param>
        /// <param name="digital">小数位</param>
        /// <param name="readWrite">操作，true:写，false:读</param>
        /// <param name="content">写入数据内容</param>
        /// <param name="results">返回检查结论</param>
        /// <param name="datas">返回数据</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool ProtocolConsistency(float U, float acFreq, string dataID, int length, int digital,
            bool readWrite, string content, out bool[] results, out string[] datas);

        /// <summary>
        /// 接线检查
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="I">电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="checkResults">返回每一个表位的压接情况,"00"表示压接正常，"01"表示压接异常</param>
        /// <returns></returns>
        bool ExecuteConnectionCheck(float U, float I, float acFreq, out string[] checkResults);

        /// <summary>
        /// 耐压试验
        /// </summary>
        /// <param name="resistanceU">耐压试验测试点电压，单位KV</param>
        /// <param name="resistanceI">漏电流档位，单位mA</param>
        /// <param name="resistanceTime">耐压试验时间，单位秒</param>
        /// <param name="resistanceType">耐压方式：”00”对外壳打耐压；”01”对辅助端子打耐压；”02”对外壳和辅助端子打耐压</param>
        /// <param name="results">返回耐压试验结果</param>
        /// <param name="resultDescriptions">返回耐压试验结果描述</param>
        /// <returns></returns>
        bool ExecuteResistance(float resistanceU, float resistanceI, int resistanceTime, string resistanceType, out bool[] results, out string[] resultDescriptions);
        #endregion

        #region 表厂增加试验项目
        /// <summary>
        /// 交采集精度试验
        /// </summary>
        /// <param name="UA">A电压</param>
        /// <param name="IA">A电流</param>
        /// <param name="Afactor">A功率因数</param>
        /// <param name="Acapacitive">A感性OR容性</param>
        /// <param name="UB"></param>
        /// <param name="IB"></param>
        /// <param name="Bfactor"></param>
        /// <param name="Bcapacitive"></param>
        /// <param name="UC"></param>
        /// <param name="IC"></param>
        /// <param name="Cfactor"></param>
        /// <param name="Ccapacitive"></param>
        void ExecutJcjjdItem(float UA, float IA, float Afactor, bool Acapacitive, float UB, float IB, float Bfactor, bool Bcapacitive,
            float UC, float IC, float Cfactor, bool Ccapacitive, VerificationEquipment.Commons.WiringMode wiringMode, Pulse pulse);
        /// <summary>
        /// 分项供电试验，通常为供一相电压，读取其他两相电压与标准值比对。
        /// </summary>
        /// <param name="UA"></param>
        /// <param name="UB"></param>
        /// <param name="UC"></param>
        void ExecutFenXiangPower(float UA, float UB, float UC, WiringMode wiringMode, float factor, bool capacitive, Pulse pulse);
        /// <summary>
        /// 设置电能表高频倍率
        /// </summary>
        /// <param name="Beilv">要设置的高频倍率</param>
        /// <returns></returns>
        bool ExecuteMeterdoubling(byte Beilv);
        #endregion

        #region 检定项目时间估算

        /// <summary>
        /// 预热时间
        /// </summary>
        /// <param name="U">预热电压，单位V</param>
        /// <param name="I">预热电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="second">预热时间，单位秒</param>
        /// <returns>预热预计时间,单位秒</returns>
        float CalcWarmUpTime(float U, float I, float acFreq, int second);

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
        /// <returns>基本误差试验预计时间,单位秒</returns>
        float CalcBasicErrorTime(float U, float I, float factor, bool capacitive, float acFreq, string pulse,
            int circle, int count);

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
        /// <returns>基本误差试验预计时间,单位秒</returns>
        float CalcBasicErrorTime(float U, float I, float acFreq, string phase, float factor, bool capacitive, Pulse pulse, int circle, int count);

        /// <summary>
        /// 启动试验时间
        /// </summary>
        /// <param name="U">启动电压，单位V</param>
        /// <param name="I">启动电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="second">启动试验时间，单位秒</param>
        /// <returns>启动试验预计时间,单位秒</returns>
        float CalcStartupTime(float U, float I, float acFreq, int second);


        /// <summary>
        /// 潜动试验时间
        /// </summary>
        /// <param name="U">潜动电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="second">潜动试验时间，单位：秒</param>
        /// <returns>潜动试验预计时间,单位秒</returns>
        float CalcLatentTime(float U, float acFreq, int second);

        /// <summary>
        /// 日计时试验时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="clockFreq">被检表时钟脉冲频率，默认1Hz</param>
        /// <param name="second">检验时间，单位：秒</param>
        /// <param name="count">检验次数</param>
        /// <returns>日计时试验预计时间,单位秒</returns>
        float CalcClockErrorTime(float U, float acFreq, float clockFreq, int second, int count);

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
        /// <returns>走字和校核计度器试验预计时间,单位秒</returns>
        float CalcEnergyReadingTime(float U, float I, float acFreq, float factor, bool capacitive, Pulse pulse, Phase phase, float energy);

        /// <summary>
        /// 需量周期、需量示值时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="I">试验电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>需量周期、需量示值预计时间,单位秒</returns>
        float CalcDemandTime(float U, float I, float acFreq);

        /// <summary>
        /// 时段投切试验时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="I">试验电流，单位A，默认300%Ib</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>时段投切试验预计时间,单位秒</returns>
        float CalcSwitchChangeTime(float U, float I, float acFreq);

        /// <summary>
        /// 读电能底数时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="pulse">脉冲类型，正向有功、反向有功、正向无功、反向无功</param>
        /// <returns>读电能底数预计时间,单位秒</returns>
        float CalcGetEnergyReadingTime(float U, float acFreq, Pulse pulse);

        /// <summary>
        /// 读需量底数时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="pulse">脉冲类型，正向有功、反向有功、正向无功、反向无功</param>
        /// <returns>读需量底数预计时间,单位秒</returns>
        float CalcGetDemandReadingTime(float U, float acFreq, Pulse pulse);

        /// <summary>
        /// 读取表地址时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>读取表地址预计时间,单位秒</returns>
        float CalcReadMeterAddressTime(float U, float acFreq);

        /// <summary>
        /// 保电试验时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>保电试验预计时间,单位秒</returns>
        float CalcHoldPowerTime(float U, float acFreq);

        /// <summary>
        /// 保电命令时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>保电命令预计时间,单位秒</returns>
        float CalcHoldPowerCommandTime(float U, float acFreq);

        /// <summary>
        /// 解除保电命令时间
        /// </summary>
        /// <param name="U">试验电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>解除保电命令预计时间,单位秒</returns>
        float CalcReleasePowerCommandTime(float U, float acFreq);

        /// <summary>
        /// 远程开户充值时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="money">充值金额</param>
        /// <param name="count">充值次数</param>
        /// <returns>远程开户充值预计时间,单位秒</returns>
        float CalcRemoteOpenAccountTime(float U, float acFreq, double money, int count);

        /// <summary>
        /// 远程充值时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="money">充值金额</param>
        /// <param name="count">充值次数</param>
        /// <returns>远程充值预计时间,单位秒</returns>
        float CalcRemoteChangeAccountTime(float U, float acFreq, double money, int count);

        /// <summary>
        /// 远程密钥更新时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="isRemote">远程表或本地表，true:远程表，false:本地表</param>
        /// <returns>远程密钥更新预计时间,单位秒</returns>
        float CalcRemoteSecretKeyUpdateTime(float U, float acFreq, bool isRemote);

        /// <summary>
        /// 远程参数修改时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns> 远程参数修改预计时间,单位秒</returns>
        float CalcRemoteParameterUpdateTime(float U, float acFreq);

        /// <summary>
        /// 远程数据回抄时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>远程数据回抄预计时间,单位秒</returns>
        float CalcRemoteDataReturnTime(float U, float acFreq);

        /// <summary>
        /// 远程拉合闸时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>远程拉合闸预计时间,单位秒</returns>
        float CalcRemoteKnifeSwitchTime(float U, float acFreq);

        /// <summary>
        /// 远程拉闸命令时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>远程拉闸命令预计时间,单位秒</returns>
        float CalcRemoteOpenSwitchCommandTime(float U, float acFreq);

        /// <summary>
        /// 远程合闸命令时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>远程合闸命令预计时间,单位秒</returns>
        float CalcRemoteCloseSwitchCommandTime(float U, float acFreq);

        /// <summary>
        /// 报警试验时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>报警试验预计时间,单位秒</returns>
        float CalcAlarmTime(float U, float acFreq);

        /// <summary>
        /// 清电量时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>清电量预计时间,单位秒</returns>
        float CalcClearEnergyTime(float U, float acFreq);

        /// <summary>
        /// 清需量时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>清需量预计时间,单位秒</returns>
        float CalcClearDemandTime(float U, float acFreq);

        /// <summary>
        /// 表计对时时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>表计对时预计时间,单位秒</returns>
        float CalcTimeSyncTime(float U, float acFreq);

        /// <summary>
        /// 修改02、04级密码时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>修改密码预计时间,单位秒</returns>
        float CalcChangePasswordTime(float U, float acFreq);

        /// <summary>
        /// 通信测试时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>通信测试预计时间,单位秒</returns>
        float CalcCommunicationTestTime(float U, float acFreq);

        /// <summary>
        /// 事件记录时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>事件记录预计时间,单位秒</returns>
        float CalcEventRecordTime(float U, float acFreq);

        /// <summary>
        /// 公钥验证时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>公钥验证预计时间,单位秒</returns>
        float CalcPublicKeyAuthenticationTime(float U, float acFreq);

        /// <summary>
        /// 私钥验证时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>私钥验证预计时间,单位秒</returns>
        float CalcPrivateKeyAuthenticationTime(float U, float acFreq);

        /// <summary>
        /// 定时冻结时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>定时冻结预计时间,单位秒</returns>
        float CalcFreezeByTimeTime(float U, float acFreq);

        /// <summary>
        /// 瞬时冻结时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>瞬时冻结预计时间,单位秒</returns>
        float CalcFreezeByInstantaneousTime(float U, float acFreq);

        /// <summary>
        /// 日冻结时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>日冻结预计时间,单位秒</returns>
        float CalcFreezeByDayTime(float U, float acFreq);

        /// <summary>
        /// 整点冻结时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>整点冻结预计时间,单位秒</returns>
        float CalcFreezeByHourTime(float U, float acFreq);

        /// <summary>
        /// 载波召测时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="dataID">数据项标示</param>
        /// <returns>载波召测预计时间,单位秒</returns>
        float CalcCarrieWaveReturnTime(float U, float acFreq, string dataID);

        /// <summary>
        /// 载波可靠性时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="dataID">数据项标示</param>
        /// <param name="times">重发次数</param>
        /// <returns>载波可靠性预计时间,单位秒</returns>
        float CalcCarrieWaveReliabilityTime(float U, float acFreq, string dataID, int times);

        /// <summary>
        /// 载波成功率时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="dataID">数据项标示</param>
        /// <param name="times">重发次数</param>
        /// <param name="interval">抄读间隔时间</param>
        /// <returns>载波成功率预计时间,单位秒</returns>
        float CalcCarrieWaveSuccessRateTime(float U, float acFreq, string dataID, int times, int interval);

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
        /// <returns>通信规约一致性检查预计时间,单位秒</returns>
        float CalcProtocolConsistencyTime(float U, float acFreq, string dataID, int length, int digital, bool readWrite, string content);

        /// <summary>
        /// 接线检查时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="I">电流，单位A</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <returns>接线检查预计时间,单位秒</returns>
        float CalcConnectionCheckTime(float U, float I, float acFreq);

        /// <summary>
        /// 耐压试验时间
        /// </summary>
        /// <param name="resistanceU">耐压试验测试点电压，单位KV</param>
        /// <param name="resistanceI">漏电流档位，单位mA</param>
        /// <param name="resistanceTime">耐压试验时间，单位秒</param>
        /// <param name="resistanceType">耐压方式：”00”对外壳打耐压；”01”对辅助端子打耐压；”02”对外壳和辅助端子打耐压</param>
        /// <returns>耐压试验时间,单位秒</returns>
        float CalcResistanceTime(float resistanceU, float resistanceI, int resistanceTime, string resistanceType);


        /// <summary>
        /// 电能表时间
        /// </summary>
        /// <param name="U">电压，单位V</param>
        /// <param name="acFreq">交流电频率(Hz)</param>
        /// <param name="times">返回电能表时间</param>
        /// <returns></returns>
        bool GetMeterTime(float U, float acFreq, out string[] times);

        #endregion

        #region 检定项合并
        /// <summary>
        /// 潜动试验、日计时试验合并执行
        /// 设备供应商根据试验参数，返回潜动试验、日计时试验结论。由应用系统判断试验结论
        /// </summary>
        /// <param name="LatentClockError">潜动试验，日计时试验参数</param>        
        /// <param name="latentTimes">返回被实验电能表的实际潜动（收到脉冲信号）时间，格式：浮点数字符串，如×××.××，单位：秒。如果被实验电能表未收到脉冲，相应的表位返回null</param>        
        /// <param name="returnSampleDatas">误差结果的回调句柄。根据检验次数，每产生一次误差值，通过该回调句柄上上层系统返回误差值</param>
        /// <returns>true表示正常执行完毕，false表示用户中止</returns>
        /// <exception cref="PortException">端口操作失败</exception>
        /// <exception cref="NotConnectedException">硬件设备未联机</exception>"
        /// <exception cref="NoResponseException">硬件设备无应答</exception>
        bool ExecuteLatentClockError(LatentClockError latentClockError, out string[] latentTimes, ReturnSampleDatasDelegate returnSampleDatas);
        #endregion
    }
}