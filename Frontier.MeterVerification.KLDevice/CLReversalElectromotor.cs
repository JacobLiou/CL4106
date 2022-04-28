using System;
using System.Collections.Generic;

using System.Text;
using VerificationEquipment.Commons;
using Frontier.MeterVerification.DeviceCommon;
using Frontier.MeterVerification.DeviceInterface;

namespace Frontier.MeterVerification.KLDevice
{
    /// <summary>
    /// 翻转电机控制
    /// </summary>
    public class CLReversalElectromotor : Comm2018Device, IConnect, IControlReversalMotor
    {
        private object objLock = new object();
        private MeterPosition[] _meterPositions;
        /// <summary>
        /// 
        /// </summary>
        private bool[] bSelectBw;


        private bool[] bDefaultSelectBw;

        /// <summary>
        /// 重试次数
        /// </summary>
        public static int RETRYTIEMS = 3;

        /// <summary>
        /// 翻转电机控制端口
        /// </summary>
        private int m_ElectromotorPort = 0;

        public override void Config(List<CommSerialPortSettings> serialPortSettings, List<CommSocketSettings> socketSettings)
        {
            string strSetting = string.Format("{0},{1},{2},{3}", serialPortSettings[0].BaudRate, serialPortSettings[0].Parity, serialPortSettings[0].DataBits, serialPortSettings[0].StopBits);
            m_ElectromotorPort = serialPortSettings[0].CommPortNumber + socketSettings[0].Port1;
            RegisterPort(m_ElectromotorPort, socketSettings[0].Port2, strSetting, socketSettings[0].IP, socketSettings[0].Port1, 1000, 200);

        }

        #region IConnect 成员

        public void Connected(int meterCount)
        {
            this.Open();
        }

        public void Connected(VerificationEquipment.Commons.MeterPosition[] meterPositions)
        {
            _meterPositions = meterPositions;
            bSelectBw = new bool[_meterPositions.Length];
            bDefaultSelectBw = new bool[_meterPositions.Length];
            for (int i = 0; i < bSelectBw.Length; i++)
            {
                bSelectBw[i] = _meterPositions[i].IsVerify;
                bDefaultSelectBw[i] = true;
            }
        }

        public void Closed()
        {
            this.Close();
        }

        #endregion

        #region IControlMotor 成员

        /// 控制设备翻转操作
        /// <summary>
        /// 控制设备翻转操作
        /// </summary>
        /// <param name="isReversal">true表示正转为检定状态，false表示反转</param>
        /// <param name="results">翻转结论</param>
        /// <param name="resultDescriptions">翻转结论描述</param>
        public void EquipmentReversal(bool isReversal, out bool[] results, out string[] resultDescriptions)
        {

            results = new bool[_meterPositions.Length];
            resultDescriptions = new string[_meterPositions.Length];
            int intPress = 0;
            string strPress = string.Empty;
            bool bResult = true;
            if (isReversal)
            {
                intPress = 1;
                strPress = "翻转";
            }
            else
            {
                intPress = 0;
                strPress = "竖直";
            }
            //翻转电机
            CLReversalElectromotor_RequestSetElectromotorPacket clelectromotror = new CLReversalElectromotor_RequestSetElectromotorPacket();
            CLReversalElectromotor_RequestSetElectromotorReplayPacket clelectromotrorrec = new CLReversalElectromotor_RequestSetElectromotorReplayPacket();

            clelectromotror.Pos = 0;
            clelectromotror.ChannelNo = 0;
            clelectromotror.ChannelNum = 1;
            clelectromotror.SetPara(bDefaultSelectBw, intPress);
            if (!SendData(m_ElectromotorPort, clelectromotror, clelectromotrorrec))
            {
                bResult = false;
            }

            for (int i = 0; i < results.Length; i++)
            {
                results[i] = bResult;
                if (bResult)
                {
                    resultDescriptions[i] = string.Format("{0}成功", strPress);
                }
                else
                {
                    resultDescriptions[i] = string.Format("{0}失败", strPress);
                }
            }
        }
        /// 单独某一行进行翻转过程
        /// <summary>
        /// 单独某一行进行翻转过程
        /// </summary>
        /// <param name="rowIndex">行索引，从1开始（1：最上一行）</param>
        /// <param name="IsReversal">true表示正转为检定状态，false表示反转</param>
        /// <returns></returns>
        public bool EquipmentReversal(int rowIndex, bool isReversal)
        {
            CLReversalElectromotor_RequestReadBwWcAndStatusPacket clElectromotor = new CLReversalElectromotor_RequestReadBwWcAndStatusPacket();

            CLReversalElectromotor_RequestReadBwWcAndStatusReplyPacket clElectromotorRec = new CLReversalElectromotor_RequestReadBwWcAndStatusReplyPacket();
            clElectromotor.Pos = rowIndex;
            clElectromotor.BwStatus = bDefaultSelectBw;
            return SendPacketWithRetry(m_ElectromotorPort, clElectromotor, clElectromotorRec);
        }
        /// <summary>
        /// 表位翻转状态
        /// </summary>
        /// <param name="meterIndex">表位数组</param>
        /// <param name="pressStatus">压接状态</param>
        public void GetEquipmentReversalStatus(int[] rowIndex, out MeterPositionReverseStatus[] pressStatus)
        {
            int intRowNum = 0;
            if (GlobalUnit.Clfs == Cus_Clfs.单相)
            {
                intRowNum = 4;
            }
            else
            {
                intRowNum = 2;
            }
            rowIndex = new int[intRowNum];
            pressStatus = new MeterPositionReverseStatus[intRowNum];

            CLReversalElectromotor_RequestReadBwWcAndStatusPacket clElectromotor = new CLReversalElectromotor_RequestReadBwWcAndStatusPacket();

            CLReversalElectromotor_RequestReadBwWcAndStatusReplyPacket clElectromotorRec = new CLReversalElectromotor_RequestReadBwWcAndStatusReplyPacket();

            for (int intInc = 0; intInc < intRowNum; intInc++)
            {
                clElectromotor.Pos = intInc + 1;
                clElectromotor.BwStatus = bDefaultSelectBw;
                if (SendPacketWithRetry(m_ElectromotorPort, clElectromotor, clElectromotorRec))
                {
                    rowIndex[intInc] = intInc + 1;
                    if (clElectromotorRec.statusTypeIsOn_PressDownLimt)
                    {
                        pressStatus[intInc] = MeterPositionReverseStatus.倾斜状态;
                    }
                    else if (clElectromotorRec.statusTypeIsOn_PressUpLimit)
                    {
                        pressStatus[intInc] = MeterPositionReverseStatus.直立状态;
                    }
                    else
                    {
                        pressStatus[intInc] = MeterPositionReverseStatus.翻转未到位状态;
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// 发送报文
        /// </summary>
        /// <param name="port"></param>
        /// <param name="sp"></param>
        /// <param name="rp"></param>
        /// <returns></returns>
        private bool SendPacketWithRetry(int port, SendPacket sp, RecvPacket rp)
        {
            lock (objLock)
            {
                for (int i = 0; i < RETRYTIEMS; i++)
                {
                    if (this.SendData(port, sp, rp) == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #region IControlReversalMotor 成员


        public void EquipmentManualReversal(int meterPositionCount, bool isReversal, out bool[] results, out string[] resultDescriptions)
        {

            results = new bool[meterPositionCount];
            resultDescriptions = new string[meterPositionCount];
            int intPress = 0;
            string strPress = string.Empty;
            bool bResult = true;
            if (isReversal)
            {
                intPress = 1;
                strPress = "翻转";
            }
            else
            {
                intPress = 0;
                strPress = "竖直";
            }
            //翻转电机
            CLReversalElectromotor_RequestSetElectromotorPacket clelectromotror = new CLReversalElectromotor_RequestSetElectromotorPacket();
            CLReversalElectromotor_RequestSetElectromotorReplayPacket clelectromotrorrec = new CLReversalElectromotor_RequestSetElectromotorReplayPacket();

            clelectromotror.Pos = 0;
            clelectromotror.ChannelNo = 0;
            clelectromotror.ChannelNum = 1;
            bool[] defaultReversal = new bool[meterPositionCount];
            for (int i = 0; i < meterPositionCount; i++)
            {
                defaultReversal[i] = true;
            }
            clelectromotror.SetPara(defaultReversal, intPress);
            if (!SendData(m_ElectromotorPort, clelectromotror, clelectromotrorrec))
            {
                bResult = false;
            }

            for (int i = 0; i < results.Length; i++)
            {
                results[i] = bResult;
                if (bResult)
                {
                    resultDescriptions[i] = string.Format("{0}成功", strPress);
                }
                else
                {
                    resultDescriptions[i] = string.Format("{0}失败", strPress);
                }
            }
            System.Threading.Thread.Sleep(5000);
        }

        public void EquipmentMeterPositionManualReversal(int meterPositionCount, int meterPosition, bool isReversal, out bool results, out string resultDescriptions)
        {
            int rowNum = 0;
            if (GlobalUnit.Clfs == Cus_Clfs.单相)
            {
                rowNum = 4;
            }
            else
            {
                rowNum = 2;
            }
            int colNum = meterPositionCount / rowNum;
            int currRowNum = meterPosition / colNum + (meterPosition % colNum) > 0 ? 1 : 0;
            results = EquipmentReversal(currRowNum, isReversal);
            resultDescriptions = results ? string.Empty : string.Format("表位{0}所在行{1}翻转失败!", meterPosition, currRowNum);
        }

        public void GetEquipmentReversalStatus(int meterPositionCount, out int[] meterNo, out MeterPositionReverseStatus[] reverseStatus)
        {
            meterNo = new int[meterPositionCount];
            int rowNum = 0;
            if (GlobalUnit.Clfs == Cus_Clfs.单相)
            {
                rowNum = 4;
            }
            else
            {
                rowNum = 2;
            }
            int colNum = meterPositionCount / rowNum;

            reverseStatus = new MeterPositionReverseStatus[meterPositionCount];

            CLReversalElectromotor_RequestReadBwWcAndStatusPacket clElectromotor = new CLReversalElectromotor_RequestReadBwWcAndStatusPacket();

            CLReversalElectromotor_RequestReadBwWcAndStatusReplyPacket clElectromotorRec = new CLReversalElectromotor_RequestReadBwWcAndStatusReplyPacket();

            for (int i = 0; i < rowNum; i++)
            {
                clElectromotor.Pos = i + 1;
                clElectromotor.BwStatus = new bool[colNum];
                for (int col = 0; col < colNum; col++)
                {
                    clElectromotor.BwStatus[col] = true;
                }

                if (SendPacketWithRetry(m_ElectromotorPort, clElectromotor, clElectromotorRec))
                {
                    for (int j = 0; j < colNum; j++)
                    {
                        meterNo[((i + 1) * (j + 1)) - 1] = (i + 1) * (j + 1);
                        if (clElectromotorRec.statusTypeIsOn_PressDownLimt)
                        {
                            reverseStatus[((i + 1) * (j + 1)) - 1] = MeterPositionReverseStatus.倾斜状态;
                        }
                        else if (clElectromotorRec.statusTypeIsOn_PressUpLimit)
                        {
                            reverseStatus[((i + 1) * (j + 1)) - 1] = MeterPositionReverseStatus.直立状态;
                        }
                        else
                        {
                            reverseStatus[((i + 1) * (j + 1)) - 1] = MeterPositionReverseStatus.翻转未到位状态;
                        }
                    }
                }
            }
        }

        #endregion


        public void EquipmentMeterPositionManualReversal(int meterPositionCount, int[] meterPosition, bool isReversal, out bool[] results, out string[] resultDescriptions)
        {
            throw new NotImplementedException();
        }
    }
}
