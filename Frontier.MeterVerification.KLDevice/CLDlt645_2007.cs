using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frontier.MeterVerification.DeviceInterface;
using VerificationEquipment.Commons;
using System.Diagnostics;
using Frontier.MeterVerification.DeviceCommon;

namespace Frontier.MeterVerification.KLDevice
{
    /// <summary>
    /// 电表通讯
    /// </summary>
    public class CLDlt645_2007 : Comm2018Device, IDlt645, IConnect
    {
        #region 声明

        private Meter meter = null;
        private MeterPosition[] meterPositions = null;
        private List<byte> buffer = new List<byte>();
        //运行标志
        private bool runFlag = false;
        /// <summary>
        /// 误差板端口
        /// </summary>
        private int[] m_arrMeterPort = new int[0];

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialPortSettings"></param>
        /// <param name="socketSettings"></param>
        public override void Config(List<CommSerialPortSettings> serialPortSettings, List<CommSocketSettings> socketSettings)
        {
            int intInc = 0;
            string strSetting;
            m_arrMeterPort = new int[serialPortSettings.Count];

            for (intInc = 0; intInc < serialPortSettings.Count; intInc++)
            {

                strSetting = string.Format("{0},{1},{2},{3}", serialPortSettings[intInc].BaudRate, serialPortSettings[intInc].Parity, serialPortSettings[intInc].DataBits, serialPortSettings[intInc].StopBits);
                m_arrMeterPort[intInc] = socketSettings[0].Port1 + serialPortSettings[intInc].CommPortNumber;
                RegisterPort(m_arrMeterPort[intInc], socketSettings[0].Port2, strSetting, socketSettings[0].IP, socketSettings[0].Port1, 1800, 500);
                System.Threading.Thread.Sleep(10);
            }
        }

        #region IConnect 成员

        public void Connected(int meterCount)
        {
            // to do
        }

        public void Connected(VerificationEquipment.Commons.MeterPosition[] meterPositions)
        {
            this.meterPositions = meterPositions;
            meter = meterPositions.First(item => item.Meter != null && item.IsVerify).Meter;
            MulitThreadManager.Instance.MaxThread = meterPositions.Length;
            MulitThreadManager.Instance.MaxTaskCountPerThread = 1;
        }

        public void Closed()
        {

        }

        #endregion

        protected override byte[] PretreatMessage(List<byte> dataList)
        {
            return dataList.ToArray();
        }

        #region IDlt645 成员

        /// <summary>
        /// 获取地址
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <returns></returns>
        public string GetAddress(int meterIndex)
        {
            bool bResult = false;
            string strResultDecription;
            string strAddr = string.Empty;
            Debug.Assert(meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null);
            if (meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null)
            {
                //合成发送报文
                byte[] byt_Cmd = Dlt645_2007Helper.GetAddressProtocol().ToArray();
                SendCmd(meterIndex, ref byt_Cmd);

                strAddr = Dlt645_2007Helper.AnalyseGetAddress(byt_Cmd.ToList(), out bResult, out strResultDecription);

                meterPositions[meterIndex - 1].Meter.Address = strAddr;
            }

            return strAddr;
        }
        /// <summary>
        /// 获取生产编号
        /// </summary>
        /// <param name="meterIndex">获取表位生产编号</param>
        /// <returns></returns>
        public string GetScbh(int meterIndex)
        {
            bool bResult = false;
            string strScbh = string.Empty;
            Debug.Assert(meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null);
            if (meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null)
            {
                byte[] byt_Cmd = Dlt645_2007Helper.GetScbhProtocol().ToArray();
                SendCmd(meterIndex, ref byt_Cmd);

                 Dlt645_2007Helper.CheckFrame(byt_Cmd.ToList(),  ref strScbh,ref bResult);
                 meterPositions[meterIndex - 1].Meter.AssetNo = strScbh;
            }

            return strScbh;
        }
        /// <summary>
        /// 获取其他两相电压值，测试分相供电专用
        /// </summary>
        /// <param name="mererIndex">表位-1</param>
        /// <param name="VoltageIndex">加第几相电压1、代表A相，2、代表B相，3、代表C相</param>
        /// <param name="meterOneVolt">表位1电压</param>
        /// <param name="meterTwoVolt">表位2电压</param>
        public void GetOtherVoltOnePosition(int meterIndex, int VoltageIndex, out float meterOneVolt, out float meterTwoVolt)
        {

            string result = string.Empty;
            bool bsult = false;
            meterOneVolt = 0.0f;
            meterTwoVolt = 0.0F;
            Debug.Assert(meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null);
            if (meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null)
            {
                byte[] byt_Cmd1 = Dlt645_2007Helper.GetVoltageProtocol(2).ToArray();
                byte[] byt_Cmd2 = Dlt645_2007Helper.GetVoltageProtocol(3).ToArray(); 
                if (VoltageIndex == 1)
                {
                    byt_Cmd1 = Dlt645_2007Helper.GetVoltageProtocol(2).ToArray();//B相
                    byt_Cmd2 = Dlt645_2007Helper.GetVoltageProtocol(3).ToArray();//C相
                }
                else if (VoltageIndex == 2)
                {
                    byt_Cmd1 = Dlt645_2007Helper.GetVoltageProtocol(1).ToArray();//A相
                    byt_Cmd2 = Dlt645_2007Helper.GetVoltageProtocol(3).ToArray();//C相
                }
                else if (VoltageIndex == 3)
                {
                    byt_Cmd1 = Dlt645_2007Helper.GetVoltageProtocol(1).ToArray();//A相
                    byt_Cmd2 = Dlt645_2007Helper.GetVoltageProtocol(2).ToArray();//B相
                }
                //先发第一条指令
                SendCmd(meterIndex, ref byt_Cmd1);
                //接到数据域数据
                string strOneVolt = Dlt645_2007Helper.AnalyseGetBankCmd(byt_Cmd1.ToList(), out bsult, out result);

                strOneVolt = strOneVolt.Substring(0, 4);
                 meterOneVolt = Convert.ToSingle(strOneVolt.Substring(0, 3)) + Convert.ToSingle(strOneVolt.Substring(3,1))/10;

                SendCmd(meterIndex, ref byt_Cmd2);

                string strTwoVolt = Dlt645_2007Helper.AnalyseGetBankCmd(byt_Cmd1.ToList(), out bsult, out result);
                strTwoVolt = strTwoVolt.Substring(0, 4);
                meterTwoVolt = Convert.ToSingle(strTwoVolt.Substring(0, 3)) + Convert.ToSingle(strTwoVolt.Substring(3, 1)) / 10;

                //meterPositions[meterIndex - 1].Meter.AssetNo = strScbh;
            }


        }
        /// <summary>
        /// 设定单个表位常数
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <returns></returns>
        public bool SetMeterDoubling(int meterIndex,byte bBeilv)
        {
            bool bResult = false;
            string strResultDecription;
            Debug.Assert(meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null);
            if (meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null)
            {
                //获取表地址
                byte[] byt_Cmd1 = Dlt645_2007Helper.GetAddressProtocol().ToArray();
                SendCmd(meterIndex, ref byt_Cmd1);
                string strAddr = string.Empty;
                strAddr = Dlt645_2007Helper.AnalyseGetAddress(byt_Cmd1.ToList(), out bResult, out strResultDecription);
                meterPositions[meterIndex - 1].Meter.Address = strAddr;
                //合成发送报文
                byte[] byt_Cmd2 = Dlt645_2007Helper.SetMeterBeilv(meterPositions[meterIndex - 1].Meter.Address, meterPositions[meterIndex - 1].Meter.Protocal.WriteTimePassword, bBeilv).ToArray();
                SendCmd(meterIndex, ref byt_Cmd2);

                bResult = Dlt645_2007Helper.AnalyseSetResutData(byt_Cmd2.ToList(), out strResultDecription);

            }

            return bResult;
        }
        /// <summary>
        /// 读取单个表位参数
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="uSendParam"></param>
        /// <returns></returns>
        public float [] ReadMeterParam(int meterIndex, uint uSendParam)
        {
            float [] fResult = new float[4];
            string result = string.Empty;
            bool bsult = false;
            Debug.Assert(meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null);
            if (meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null)
            {
                byte[] byt_Cmd = Dlt645_2007Helper.GetSendProtocol(uSendParam).ToArray();
                SendCmd(meterIndex, ref byt_Cmd);

                //接到数据域数据
                string strBackData = Dlt645_2007Helper.AnalyseGetBankCmd(byt_Cmd.ToList(), out bsult, out result);
                strBackData = strBackData.Substring(0, strBackData.Length - 4);
                switch (uSendParam)
                {
                    case 0x0201ff00:
                        {
                            //Uc
                            fResult[2] = Convert.ToSingle(strBackData.Substring(0, 3)) + Convert.ToSingle(strBackData.Substring(3, 1)) / 10;
                            //Ub
                            fResult[1] = Convert.ToSingle(strBackData.Substring(4, 3)) + Convert.ToSingle(strBackData.Substring(7, 1)) / 10;
                            //Ua
                            fResult[0] = Convert.ToSingle(strBackData.Substring(8, 3)) + Convert.ToSingle(strBackData.Substring(11, 1)) / 10;
                        }
                        break;
                    case 0x0202ff00:
                        {
                            //Ic
                            fResult[2] = Convert.ToSingle(strBackData.Substring(0, 3)) + Convert.ToSingle(strBackData.Substring(3, 3)) / 1000;
                            //Ib
                            fResult[1] = Convert.ToSingle(strBackData.Substring(6, 3)) + Convert.ToSingle(strBackData.Substring(9, 3)) / 1000;
                            //Ia
                            fResult[0] = Convert.ToSingle(strBackData.Substring(12, 3)) + Convert.ToSingle(strBackData.Substring(15, 1)) / 10000;
                        }
                        break;
                    case 0x0203ff00:
                        {

                            //PC
                            fResult[3] = Convert.ToSingle(strBackData.Substring(0, 2)) + Convert.ToSingle(strBackData.Substring(2, 4)) / 10000;
                            //PB
                            fResult[2] = Convert.ToSingle(strBackData.Substring(6, 2)) + Convert.ToSingle(strBackData.Substring(8, 4)) / 10000;
                            //PA
                            fResult[1] = Convert.ToSingle(strBackData.Substring(12, 2)) + Convert.ToSingle(strBackData.Substring(14, 4)) / 10000;
                            //PZ
                            fResult[0] = Convert.ToSingle(strBackData.Substring(18, 2)) + Convert.ToSingle(strBackData.Substring(20, 4)) / 10000;
                        }
                        break;
                    default:
                        break;
                }
            }
            return fResult;
        }

        /// <summary>
        /// 打包参数下载
        /// </summary>
        /// <param name="Item_TxFrame">下发参数，格式必须正确</param>
        /// <param name="Item_RxFrame">下发参数，格式 必须正确</param>
        /// <param name="meterIndex">表位号</param>
        /// <returns></returns>
        public bool DownCmdToMeterTxAndRx(string Item_TxFrame, string Item_RxFrame, out string strMeterRx, int meterIndex)
        {
            bool bResult = false;

            //Item_TxFrame = Item_TxFrame.Replace(" ", "");
            List<byte> data = new List<byte>();
            //data = 
            strMeterRx = string.Empty;
            Debug.Assert(meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null);
            if (meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null)
            {
                //string strfs = "68 02 00 00 00 00 00 68 14 0D 35 3B 33 37 35 33 33 33 33 33 33 33 34 9B 16";
                Item_TxFrame = "fe fe fe " + Item_TxFrame;
                byte[] byt_Cmd = HexCon.StringToByte(Item_TxFrame);
                SendCmd(meterIndex, ref byt_Cmd);

                string strRxCmd = HexCon.ByteToString(byt_Cmd);
                strRxCmd = strRxCmd.Replace("fe", "");
                strRxCmd = strRxCmd.Replace("FE", "");
                strMeterRx = strRxCmd;
                strRxCmd = strRxCmd.Replace(" ", "");
                Item_RxFrame = Item_RxFrame.Replace(" ", "");
                if (Item_RxFrame == strRxCmd)
                {
                    bResult = true;
                }
            }


            return bResult;
        }


       public class HexCon
        {
            //converter hex string to byte and byte to hex string
            public static string ByteToString(byte[] InBytes)
            {
                string StringOut = "";
                foreach (byte InByte in InBytes)
                {
                    StringOut = StringOut + String.Format("{0:X2} ", InByte);
                }
                return StringOut;
            }

            /// <summary>
            /// 字符串转化为字节数组
            /// </summary>
            /// <param name="InString">帧</param>
            /// <returns></returns>
            public static byte[] StringToByte(string InString)
            {
                string[] ByteStrings;
                int iLen;

                InString = InString.Trim();
                ByteStrings = InString.Split(" ".ToCharArray());
                byte[] ByteOut;
                ByteOut = new byte[ByteStrings.Length];
                iLen = ByteStrings.Length - 1;
                string hexstr;
                if (InString.Length <= 0)
                {
                    return ByteOut;
                }
                for (int i = 0; i <= iLen; i++)
                {
                    hexstr = "0x" + ByteStrings[i];
                    ByteOut[i] = System.Convert.ToByte(hexstr, 16);
                }
                return ByteOut;
            }
        }
        /// <summary>
        /// 获取日期
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <returns></returns>
        public string GetDate(int meterIndex)
        {
            bool bResult = false;
            string strResultDecription;
            string strDate = "";
            Debug.Assert(meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null);
            if (meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null)
            {
                byte[] byt_Cmd = Dlt645_2007Helper.GetDateProtocol(meterPositions[meterIndex - 1].Meter.Address).ToArray();
                SendCmd(meterIndex, ref byt_Cmd);

                strDate = Dlt645_2007Helper.AnalyseGetDate(byt_Cmd.ToList(), out bResult, out strResultDecription);
            }

            return strDate;
        }

        /// <summary>
        /// 获取时间
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <returns></returns>
        public string GetTime(int meterIndex)
        {
            bool bResult = false;
            string strResultDecription;
            string strTime = "";
            Debug.Assert(meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null);
            if (meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null)
            {
                byte[] byt_Cmd = Dlt645_2007Helper.GetTimeProtocol(meterPositions[meterIndex - 1].Meter.Address).ToArray();
                SendCmd(meterIndex, ref byt_Cmd);
                strTime = Dlt645_2007Helper.AnalyseGetTime(byt_Cmd.ToList(), out bResult, out strResultDecription);
            }

            return strTime;
        }

        /// <summary>
        /// 设置单表位日期
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="dateTime"></param>
        /// <param name="resultDescript"></param>
        /// <returns></returns>
        private bool SetDate(int meterIndex, DateTime dateTime, out string resultDescript)
        {
            bool bResult = false;
            resultDescript = string.Empty;
            Debug.Assert(meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null);
            if (meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null)
            {
                byte[] byt_Cmd = Dlt645_2007Helper.SetDateProtocol(meterPositions[meterIndex - 1].Meter.Address, meterPositions[meterIndex - 1].Meter.Protocal.WriteTimePassword, dateTime).ToArray();
                SendCmd(meterIndex, ref byt_Cmd);

                bResult = Dlt645_2007Helper.AnalyseSetDate(byt_Cmd.ToList(), out resultDescript);
            }
            return bResult;
        }

        /// <summary>
        /// 设置单表位时间
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="dateTime"></param>
        /// <param name="resultDescript"></param>
        /// <returns></returns>
        private bool SetTime(int meterIndex, DateTime dateTime, out string resultDescript)
        {
            bool bResult = false;
            resultDescript = string.Empty;
            Debug.Assert(meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null);
            if (meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null)
            {
                byte[] byt_Cmd = Dlt645_2007Helper.SetTimeProtocol(meterPositions[meterIndex - 1].Meter.Address, meterPositions[meterIndex - 1].Meter.Protocal.WriteTimePassword, dateTime).ToArray();
                SendCmd(meterIndex, ref byt_Cmd);
                bResult = Dlt645_2007Helper.AnalyseSetTime(byt_Cmd.ToList(), out resultDescript);
            }
            return bResult;
        }

        /// <summary>
        /// 设置日期
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public bool SetDate(int meterIndex, DateTime dateTime)
        {
            string resultDescript;
            return SetDate(meterIndex, dateTime, out resultDescript);
        }

        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public bool SetTime(int meterIndex, DateTime dateTime)
        {
            string resultDescript;
            return SetTime(meterIndex, dateTime, out resultDescript);
        }

        /// <summary>
        /// 获取电量
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="pulse"></param>
        /// <returns></returns>
        public string GetEnergy(int meterIndex, VerificationEquipment.Commons.Pulse pulse)
        {
            string strEnergy = string.Empty;
            bool bResult = false;
            string strResultDecription;
            Debug.Assert(meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null);
            if (meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null)
            {
                byte[] byt_Cmd = Dlt645_2007Helper.GetEnergyProtocol(meterPositions[meterIndex - 1].Meter.Address, pulse).ToArray();
                SendCmd(meterIndex, ref byt_Cmd);

                strEnergy = Dlt645_2007Helper.AnalyzeGetEnergy(byt_Cmd.ToList(), out bResult, out strResultDecription);
            }

            return strEnergy;
        }

        /// <summary>
        /// 清空电量
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <returns></returns>
        public bool ClearEnergy(int meterIndex)
        {
            bool bResult = false;
            string strResultDecription;
            Debug.Assert(meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null);
            if (meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null)
            {
                //获取表地址
                byte[] byt_Cmd = Dlt645_2007Helper.GetAddressProtocol().ToArray();
                SendCmd(meterIndex, ref byt_Cmd);
                string strAddr = string.Empty;
                strAddr = Dlt645_2007Helper.AnalyseGetAddress(byt_Cmd.ToList(), out bResult, out strResultDecription);
                meterPositions[meterIndex - 1].Meter.Address = strAddr;
                //清除表底度
                byt_Cmd = Dlt645_2007Helper.ClearMeterProtocol(meterPositions[meterIndex - 1].Meter.Address, meterPositions[meterIndex - 1].Meter.Protocal.WriteTimePassword).ToArray();
                SendCmd(meterIndex, ref byt_Cmd);//这个Pulse没作用

                bResult = Dlt645_2007Helper.AnalyseClearMeter(byt_Cmd.ToList(), out strResultDecription);
            }

            return bResult;
        }

        /// <summary>
        /// 获取需量
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <returns></returns>
        public string GetDemand(int meterIndex)
        {
            string strDemand = string.Empty;
            bool bResult = false;
            string strResultDecription;
            Debug.Assert(meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null);
            if (meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null)
            {
                byte[] byt_Cmd = Dlt645_2007Helper.GetDemandProtocol(meterPositions[meterIndex - 1].Meter.Address, Pulse.正向有功).ToArray();
                SendCmd(meterIndex, ref byt_Cmd);

                strDemand = Dlt645_2007Helper.AnalyzeGetDemand(byt_Cmd.ToList(), out bResult, out strResultDecription);
            }

            return strDemand;
        }

        /// <summary>
        /// 清空需量
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <returns></returns>
        public bool ClearDemand(int meterIndex)
        {
            bool bResult = false;
            string strResultDecription;
            Debug.Assert(meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null);
            if (meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null)
            {
                byte[] byt_Cmd = Dlt645_2007Helper.ClearMaxDemandProtocol(meterPositions[meterIndex - 1].Meter.Address, meterPositions[meterIndex - 1].Meter.Protocal.WriteTimePassword).ToArray();
                SendCmd(meterIndex, ref byt_Cmd);//这个Pulse没作用

                bResult = Dlt645_2007Helper.AnalyzeClearMaxDemand(byt_Cmd.ToList(), out strResultDecription);
            }

            return bResult;
        }

        /// <summary>
        /// 获取费率信息
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <returns></returns>
        private MeterRates[] GetMeterRates(int meterIndex, int iSeries, Dlt645_2007Cmd dltCmd)
        {
            MeterRates[] meterRates = null;
            Debug.Assert(meterPositions != null
                && meterPositions[meterIndex - 1] != null
                && meterPositions[meterIndex - 1].Meter != null);
            if (meterPositions != null
                && meterPositions[meterIndex - 1] != null
                && meterPositions[meterIndex - 1].Meter != null)
            {
                byte[] byt_Cmd = Dlt645_2007Helper.GetPhaseTimeProtocol(meterPositions[meterIndex - 1].Meter.Address, dltCmd).ToArray();
                SendCmd(meterIndex, ref byt_Cmd);

                bool result;
                string resultDescription;
                meterRates = Dlt645_2007Helper.AnalyzeGetPhaseTime(byt_Cmd.ToList(), iSeries, out result, out resultDescription);
            }

            return meterRates;
        }

        /// <summary>
        /// 获取第三状态字
        /// </summary>
        /// <param name="meterIndex">表位</param>
        /// <returns></returns>
        private string GetThirdStatus(int meterIndex)
        {
            string status = string.Empty;
            Debug.Assert(meterPositions != null
                && meterPositions[meterIndex - 1] != null
                && meterPositions[meterIndex - 1].Meter != null);
            if (meterPositions != null
                && meterPositions[meterIndex - 1] != null
                && meterPositions[meterIndex - 1].Meter != null)
            {
                byte[] byt_Cmd = Dlt645_2007Helper.GetThirdStatusProtocol(meterPositions[meterIndex - 1].Meter.Address).ToArray();
                SendCmd(meterIndex, ref byt_Cmd);
                bool result;
                string resultDescription;
                status = Dlt645_2007Helper.AnalyzeGetThirdStatus(byt_Cmd.ToList(), out result, out resultDescription);
            }

            return status;
        }

        /// <summary>
        /// 设置多功能端子
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="elementType"></param>
        /// <returns></returns>
        public bool SetMulTerminalOut(int meterIndex, DeviceCommon.VerificationElementType elementType)
        {
            bool bResult = false;
            string strResultDecription;
            Debug.Assert(meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null);
            if (meterPositions != null && meterPositions[meterIndex - 1] != null && meterPositions[meterIndex - 1].Meter != null)
            {
                byte[] byt_Cmd = Dlt645_2007Helper.SetMulTerminalOutProtocol(meterPositions[meterIndex - 1].Meter.Address, elementType).ToArray();
                SendCmd(meterIndex, ref byt_Cmd);

                bResult = Dlt645_2007Helper.AnalyzeSetMulTerminalOut(byt_Cmd.ToList(), out strResultDecription);
            }

            return bResult;
        }

        /// <summary>
        /// 获取表地址
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="address"></param>
        public void GetAddress(int[] meterIndex, out string[] address)
        {
            string[] addr = new string[meterPositions.Length];
            runFlag = true;
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag) return;
                addr[bw - 1] = GetAddress(bw);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            address = addr;
        }
        /// <summary>
        /// 读取表生产编号
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="meterScbh"></param>
        public void GetScbh(int[] meterIndex, out string[] meterScbh)
        {
            string[] addr = new string[meterPositions.Length];
            runFlag = true;
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag) return;
                addr[bw - 1] = GetScbh(bw);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            meterScbh = addr;
        }

        /// <summary>
        /// 设置电能表倍率
        /// </summary>
        /// <param name="meterIndex">表位数组</param>
        /// <param name="bResult">返回结果</param>
        /// <param name="bBeilv">设置的倍率</param>
        public void SetMeterDoubling(int[] meterIndex, out bool[] bResult,byte bBeilv)
        {
            bool[] Setsult = new bool[meterPositions.Length];
            runFlag = true;
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag) return;
                Setsult[bw - 1] = SetMeterDoubling(bw, bBeilv);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            bResult = Setsult;
        }
        /// <summary>
        /// 读取表当前内部参数
        /// </summary>
        /// <param name="meterIndex">表位号</param>
        /// <param name="uSendParam">要读取的数据标识</param>
        /// <param name="foutMeterParam">出参所有表位的返回数据</param>
        public void ReadMeterParam(int[] meterIndex, uint uSendParam, out float[][] foutMeterParam)
        {
            float[][] foutP = new float[meterPositions.Length][];
            runFlag = true;
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag) return;
                foutP[bw - 1] = ReadMeterParam(bw, uSendParam);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            foutMeterParam = foutP;
        }

        /// <summary>
        /// 获取分相供电测试数据
        /// </summary>
        /// <param name="meterIndex">表位-1</param>
        /// <param name="MeterOneVolt">第一相电压值，依次类推A、B、C(加A相电压，就读B、C相</param>
        /// <param name="mererTwoVolt">第二相电压值，依次类推A、B、C(加A相电压，就读B、C相</param>
        public void GetOtherVoltage(int[] meterIndex, int VoltageIndex, out float[] meterOneVolt, out float[] meterTwoVolt)
        {
            float[] OneVolt = new float[meterPositions.Length];
            float[] TwoVolt = new float[meterPositions.Length];
            float oneVolt;
            float twoVolt;
            runFlag = true;
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag) return;
                 GetOtherVoltOnePosition(bw, VoltageIndex, out oneVolt,out twoVolt);
                 OneVolt[bw - 1] = oneVolt;
                 TwoVolt[bw - 1] = twoVolt;
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            meterOneVolt = OneVolt;
            meterTwoVolt = TwoVolt;
        }

        /// <summary>
        /// 打包参数下载
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="meterScbh"></param>
        public void DownCmdToMeter(string Item_TxFrame, string Item_RxFrame, int[] meterIndex, out bool[] meterreSult, out string [] meterRx)
        {
            bool [] bResults = new bool[meterPositions.Length];
            string[] meterRxFrame = new string[meterPositions.Length];
            runFlag = true;
            string strRX = string.Empty;
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag) return;
                bResults[bw - 1] = DownCmdToMeterTxAndRx(Item_TxFrame, Item_RxFrame, out strRX, bw);
                meterRxFrame[bw - 1] = strRX;
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            meterreSult = bResults;
            meterRx = meterRxFrame;
        }

        /// <summary>
        /// 获取日期
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="dateTime"></param>
        public void GetDate(int[] meterIndex, out DateTime[] dateTime)
        {
            DateTime[] addr = new DateTime[meterPositions.Length];
            runFlag = true;
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag) return;
                addr[bw - 1] = Convert.ToDateTime(GetDate(bw));
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            dateTime = addr;
        }

        /// <summary>
        /// 获取时间
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="dateTime"></param>
        public void GetTime(int[] meterIndex, out DateTime[] dateTime)
        {
            DateTime[] addr = new DateTime[meterPositions.Length];
            runFlag = true;
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag) return;
                addr[bw - 1] = Convert.ToDateTime(GetTime(bw));
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            dateTime = addr;
        }

        /// <summary>
        /// 设置日期
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="dateTime"></param>
        /// <param name="results"></param>
        /// <param name="resultDescript"></param>
        public void SetDate(int[] meterIndex, DateTime dateTime, out bool[] results, out string[] resultDescript)
        {
            bool[] ret = new bool[meterPositions.Length];
            string[] retdes = new string[meterPositions.Length];
            runFlag = true;
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag) return;
                ret[bw - 1] = SetDate(bw, dateTime, out retdes[bw - 1]);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            results = ret;
            resultDescript = retdes;
        }

        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="dateTime"></param>
        /// <param name="results"></param>
        /// <param name="resultDescript"></param>
        public void SetTime(int[] meterIndex, DateTime dateTime, out bool[] results, out string[] resultDescript)
        {
            bool[] ret = new bool[meterPositions.Length];
            string[] retdes = new string[meterPositions.Length];
            runFlag = true;
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag) return;
                ret[bw - 1] = SetTime(bw, dateTime, out retdes[bw - 1]);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            results = ret;
            resultDescript = retdes;
        }

        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="results"></param>
        /// <param name="resultDescript"></param>
        public void SetTime(int[] meterIndex, out bool[] results, out string[] resultDescript)
        {
            SetTime(meterIndex, DateTime.Now, out results, out resultDescript);
        }

        /// <summary>
        /// 获取电量
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="pulse"></param>
        /// <param name="energy"></param>
        public void GetEnergy(int[] meterIndex, VerificationEquipment.Commons.Pulse pulse, out string[] energy)
        {
            string[] retdes = new string[meterPositions.Length];
            runFlag = true;
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag) return;
                retdes[bw - 1] = GetEnergy(bw, pulse);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            energy = retdes;
        }

        /// <summary>
        /// 清空电量
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="results"></param>
        /// <param name="resultDescript"></param>
        public void ClearEnergy(int[] meterIndex, out bool[] results, out string[] resultDescript)
        {
            bool[] ret = new bool[meterPositions.Length];
            string[] retdes = new string[meterPositions.Length];
            runFlag = true;
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag) return;
                ret[bw - 1] = ClearEnergy(bw);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            results = ret;
            resultDescript = retdes;
        }

        /// <summary>
        /// 获取需量
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="pulse"></param>
        /// <param name="demand"></param>
        public void GetDemand(int[] meterIndex, VerificationEquipment.Commons.Pulse pulse, out string[] demand)
        {
            string[] retdes = new string[meterPositions.Length];
            runFlag = true;
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag) return;
                retdes[bw - 1] = GetDemand(bw);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            demand = retdes;
        }

        /// <summary>
        /// 清空需量
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="results"></param>
        /// <param name="resultDescript"></param>
        public void ClearDemand(int[] meterIndex, out bool[] results, out string[] resultDescript)
        {
            bool[] ret = new bool[meterPositions.Length];
            string[] retdes = new string[meterPositions.Length];
            runFlag = true;
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag) return;
                ret[bw - 1] = ClearDemand(bw);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            results = ret;
            resultDescript = retdes;
        }

        /// <summary>
        /// 设置多功能端子
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="elementType"></param>
        /// <param name="results"></param>
        /// <param name="resultDescript"></param>
        public void SetMulTerminalOut(int[] meterIndex, DeviceCommon.VerificationElementType elementType, out bool[] results, out string[] resultDescript)
        {
            bool[] ret = new bool[meterPositions.Length];
            string[] retdes = new string[meterPositions.Length];
            runFlag = true;
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag) return;
                ret[bw - 1] = SetMulTerminalOut(bw, elementType);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            results = ret;
            resultDescript = retdes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="meterRates"></param>
        public void GetMeterRates(int[] meterIndex, out MeterRates[] meterRates)
        {
            // 获取第三状态字
            string[] status = new string[meterPositions.Length];
            runFlag = true;
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag) return;
                status[bw - 1] = GetThirdStatus(bw);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            runFlag = true;
            MeterRates[][] myRates = new MeterRates[meterPositions.Length][];
            meterRates = new MeterRates[meterPositions.Length];
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag
                    || string.IsNullOrEmpty(status[bw - 1]))
                {
                    return;
                }
            
                myRates[bw - 1] = GetMeterRates(bw, 1,
                    (status[bw - 1][0] == 1
                    ? Dlt645_2007Cmd.第一套日时段
                    : Dlt645_2007Cmd.第二套日时段));
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            for (int intInc = 0; intInc < meterPositions.Length; intInc++)
            {
                if (myRates[intInc] != null)
                {
                    meterRates = myRates[intInc];
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="demandPeriod"></param>
        public void GetDemandInterval(int[] meterIndex, out int[] demandPeriod)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="SlidingWindowTime"></param>
        public void GetSlidingWindowTime(int[] meterIndex, out int[] SlidingWindowTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取拉合闸状态
        /// </summary>
        /// <param name="meterIndex">表位</param>
        /// <param name="states">状态</param>
        public void GetKnifeSwitchStatus(int[] meterIndex, out string[] states)
        {
            // 获取第三状态字
            string[] status = new string[meterPositions.Length];
            runFlag = true;
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag) return;
                status[bw] = GetThirdStatus(meterIndex[bw]);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            states = status;
        }

        /// <summary>
        /// 身份认证
        /// </summary>
        /// <param name="address"></param>
        /// <param name="putdiv"></param>
        /// <param name="putrand1"></param>
        /// <param name="putpwd"></param>
        /// <returns></returns>
        private bool SetSecurityCertificate(int meterIndex, string putdiv, string putrand1, string putpwd, out string outRandNum, out string outEsam)
        {
            bool result = false;
            outRandNum = string.Empty;
            outEsam = string.Empty;
            string resultDescription;
            Debug.Assert(meterPositions != null
                && meterPositions[meterIndex - 1] != null
                && meterPositions[meterIndex - 1].Meter != null);
            if (meterPositions != null
                && meterPositions[meterIndex - 1] != null
                && meterPositions[meterIndex - 1].Meter != null)
            {
                byte[] byt_Cmd = Dlt645_2007Helper.SecurityCertificateProtocol(meterPositions[meterIndex - 1].Meter.Address, putdiv, putrand1, putpwd).ToArray();
                SendCmd(meterIndex, ref byt_Cmd);
                result = Dlt645_2007Helper.AnalyzeSecurityCertificate(byt_Cmd.ToList(), out outRandNum, out outEsam, out resultDescription);
            }

            return result;
        }

        /// <summary>
        /// 设置身份认证
        /// </summary>
        /// <param name="meterIndex">表位</param>
        /// <param name="putdiv">分散因子</param>
        /// <param name="putrand1">随机数1</param>
        /// <param name="putpwd">密钥</param>
        /// <param name="outrand2">随机数2</param>
        /// <param name="outesam"></param>
        /// <param name="Netencryption"></param>
        /// <returns></returns>
        public void SetSecurityCertificate(int[] meterIndex, string putDiv, string putRand1, string putPwd1, out string[] outRand2, out string[] outEsam, bool netEncryption)
        {
            runFlag = true;
            string[] randNums = new string[meterPositions.Length];
            string[] esams = new string[meterPositions.Length];
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag) return;
                SetSecurityCertificate(meterIndex[bw], putDiv, putRand1, putPwd1, out randNums[meterIndex[bw] - 1], out esams[meterIndex[bw] - 1]);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

            outRand2 = randNums;
            outEsam = esams;
        }

        /// <summary>
        /// 设置身份认证
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="putRand1"></param>
        /// <param name="putPwd1"></param>
        /// <param name="outRandNum"></param>
        /// <param name="outEsam"></param>
        /// <param name="netEncryption"></param>
        /// <returns></returns>
        private bool SetSecurityCertificate(int meterIndex, string putRand1, string putPwd1, out string outRandNum, out string outEsam, bool netEncryption)
        {
            bool result = false;
            outRandNum = string.Empty;
            outEsam = string.Empty;
            string resultDescription;
            Debug.Assert(meterPositions != null
                && meterPositions[meterIndex - 1] != null
                && meterPositions[meterIndex - 1].Meter != null);
            if (meterPositions != null
                && meterPositions[meterIndex - 1] != null
                && meterPositions[meterIndex - 1].Meter != null)
            {
                byte[] byt_Cmd = Dlt645_2007Helper.SecurityCertificateProtocol(meterPositions[meterIndex - 1].Meter.Address, putRand1, putPwd1).ToArray();
                SendCmd(meterIndex, ref byt_Cmd);
                result = Dlt645_2007Helper.AnalyzeSecurityCertificate(byt_Cmd.ToList(), out outRandNum, out outEsam, out resultDescription);
            }

            return result;
        }

        /// <summary>
        /// 设置身份认证
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="putRand1"></param>
        /// <param name="putPwd1"></param>
        /// <param name="outRand2"></param>
        /// <param name="outEsam"></param>
        /// <param name="netEncryption"></param>
        public void SetSecurityCertificate(int[] meterIndex, string[] putRand1, string[] putPwd1, out string[] outRand2, out string[] outEsam, bool netEncryption)
        {
            runFlag = true;
            string[] randNums = new string[meterPositions.Length];
            string[] esams = new string[meterPositions.Length];
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag) return;
                SetSecurityCertificate(meterIndex[bw], putRand1[meterIndex[bw] - 1], putPwd1[meterIndex[bw] - 1], out randNums[meterIndex[bw] - 1], out esams[meterIndex[bw] - 1], netEncryption);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

            outRand2 = randNums;
            outEsam = esams;
        }

        /// <summary>
        /// 拉合闸、保电、告警等操作
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="outRand"></param>
        /// <param name="esam"></param>
        /// <param name="openAlarm"></param>
        /// <param name="netEncryption"></param>
        /// <returns></returns>
        private bool UserControlOpenAlarm(int meterIndex, string outRand, string esam, int openAlarm, bool netEncryption)
        {
            bool result = false;
            Debug.Assert(meterPositions != null
               && meterPositions[meterIndex - 1] != null
               && meterPositions[meterIndex - 1].Meter != null);
            if (meterPositions != null
                && meterPositions[meterIndex - 1] != null
                && meterPositions[meterIndex - 1].Meter != null)
            {
                string putDiv = "0000000000000001";
                StringBuilder outEndData = new StringBuilder();
                string putData = string.Format("{0}00{1}", "", DateTime.Now.AddHours(1).ToString("yyMMddHHmmss")); // 送入至加密机参数由状态码加有效时间组成
                if (netEncryption)
                {
                    gSJJ1009Server.UserControl(0, outRand, putDiv, esam, putData, outEndData);
                }
                else
                {
                    gSJJ1009Server.UserControl(outRand + putDiv + esam + putData, outEndData);
                }

                byte[] cmdDatas = Dlt645_2007Helper.KnifeSwitchAlarmProtocol(meterPositions[meterIndex - 1].Meter.Address, outEndData.ToString()).ToArray();
                SendCmd(meterIndex, ref cmdDatas);

                string resultDescription;
                result = Dlt645_2007Helper.AnalyzeKnifeSwitchAlarm(cmdDatas.ToList(), out resultDescription);
            }
            return result;
        }

        /// <summary>
        /// 拉合闸、保电、告警等操作
        /// </summary>
        /// <param name="meterIndex">需要设置的表位号数组，内容从表位1开始</param>
        /// <param name="outrand2">随机数2</param>
        /// <param name="ESAM">ESAM序列号</param>
        /// <param name="OpenAlarm">N1=1AH代表跳闸，N1=1BH代表合闸允许，N1=2AH代表报警，N1=2BH代表报警解除，N1=3AH代表保电，N1=3BH代表保电解除</param>
        /// <param name="Netencryption">true：网络加密机；false：开发套件</param>
        /// <param name="results">结果</param>
        /// <returns></returns>
        public void UserControlOpenAlarm(int[] meterIndex, string[] outRand, string[] esam, int openAlarm, bool netEncryption, out bool[] results)
        {
            bool[] outResults = new bool[meterPositions.Length];
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag) return;
                outResults[bw] = UserControlOpenAlarm(meterIndex[bw], outRand[meterIndex[bw]], esam[meterIndex[bw]], openAlarm, netEncryption);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

            results = outResults;
        }

        /// <summary>
        /// 获取数据回抄数据
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="fileId"></param>
        /// <param name="beginAddress"></param>
        /// <param name="readLen"></param>
        /// <param name="netencryption"></param>
        /// <returns></returns>
        private string GetDataBackToCopy(int meterIndex, int fileId, int beginAddress, int readLen, bool netencryption)
        {
            string data = string.Empty;
            bool result;
            string resultDescription;
            Debug.Assert(meterPositions != null
                && meterPositions[meterIndex - 1] != null
                && meterPositions[meterIndex - 1].Meter != null);
            if (meterPositions != null
                && meterPositions[meterIndex - 1] != null
                && meterPositions[meterIndex - 1].Meter != null)
            {
                byte[] bytCmd = Dlt645_2007Helper.DataBackToCopyProtocol(meterPositions[meterIndex - 1].Meter.Address, fileId, beginAddress, readLen).ToArray();
                SendCmd(meterIndex, ref bytCmd);
                data = Dlt645_2007Helper.AnalyzeDataBackToCopy(bytCmd.ToList(), out result, out resultDescription);
            }

            return data;
        }

        /// <summary>
        /// 获取数据回抄数据
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="fileID"></param>
        /// <param name="beginAddress"></param>
        /// <param name="readLen"></param>
        /// <param name="outdata"></param>
        /// <param name="Netencryption"></param>
        public void GetDataBackToCopy(int[] meterIndex, int fileID, int beginAddress, int readLen, out string[] outdata, bool Netencryption)
        {
            string[] datas = new string[meterPositions.Length];
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag) return;
                datas[bw] = GetDataBackToCopy(meterIndex[bw], fileID, beginAddress, readLen, Netencryption);
            };

            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

            outdata = datas;
        }

        /// <summary>
        /// 主控密钥更新
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="beforeInfo"></param>
        /// <param name="keyKind"></param>
        /// <param name="outKey"></param>
        /// <param name="outKeyInfo"></param>
        /// <param name="resultDescript"></param>
        /// <returns></returns>
        private bool MainSecurityUpdate(int meterIndex, string keyKind, string outKey, string outKeyInfo, out string resultDescript)
        {
            bool result = false;
            resultDescript = string.Empty;
            Debug.Assert(meterPositions != null
               && meterPositions[meterIndex - 1] != null
               && meterPositions[meterIndex - 1].Meter != null);
            if (meterPositions != null
                && meterPositions[meterIndex - 1] != null
                && meterPositions[meterIndex - 1].Meter != null)
            {
                byte[] cmdDatas = Dlt645_2007Helper.MainSecurityUpdateProtocol(meterPositions[meterIndex - 1].Meter.Address, keyKind, outKey, outKeyInfo).ToArray();
                SendCmd(meterIndex, ref cmdDatas);
                result = Dlt645_2007Helper.AnalyzeSecurityUpdate(cmdDatas.ToList(), out resultDescript);
            }

            return result;
        }

        /// <summary>
        /// 主控密钥更新
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="beforeInfos">更新前密钥版本信息</param>
        /// <param name="KeyKind">输入的密钥信息明文</param>
        /// <param name="OutKey">输出的密钥密文</param>
        /// <param name="OutKeyinfo">输出的密钥信息</param>
        /// <param name="results">返回远程密钥更新结果</param>
        /// <param name="resultDescriptions">返回远程密钥更新结果描述</param>
        /// <returns></returns>
        public void MainSecurityUpdate(int[] meterIndex, string[] beforeInfos, string KeyKind, string OutKey, string OutKeyinfo, out bool[] results, out string[] resultDescript)
        {
            bool[] outResults = new bool[meterPositions.Length];
            string[] outResultDescription = new string[meterPositions.Length];
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag) return;

                int keyCode = 0;
                if (!string.IsNullOrEmpty(beforeInfos[meterIndex[bw]]) &&
                    beforeInfos[meterIndex[bw]].Length > 7)
                {
                    keyCode = Convert.ToInt32(beforeInfos[meterIndex[bw]].Substring(6, 2));
                }

                if (keyCode < 1)
                {
                    outResults[meterIndex[bw]] = MainSecurityUpdate(meterIndex[bw], KeyKind, OutKey, OutKeyinfo, out outResultDescription[meterIndex[bw]]);
                }
            };

            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

            results = outResults;
            resultDescript = outResultDescription;
        }

        /// <summary>
        /// 控制命令密钥更新
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="keyKind"></param>
        /// <param name="outKey"></param>
        /// <param name="outKeyInfo"></param>
        /// <param name="resultDescript"></param>
        /// <returns></returns>
        private bool ControlSecurityUpdate(int meterIndex, string keyKind, string outKey, string outKeyInfo, out string resultDescript)
        {
            bool result = false;
            resultDescript = string.Empty;
            Debug.Assert(meterPositions != null
               && meterPositions[meterIndex - 1] != null
               && meterPositions[meterIndex - 1].Meter != null);
            if (meterPositions != null
                && meterPositions[meterIndex - 1] != null
                && meterPositions[meterIndex - 1].Meter != null)
            {
                byte[] cmdDatas = Dlt645_2007Helper.ControlSecurityUpdateProtocol(meterPositions[meterIndex - 1].Meter.Address, keyKind, outKey, outKeyInfo).ToArray();
                SendCmd(meterIndex, ref cmdDatas);
                result = Dlt645_2007Helper.AnalyzeSecurityUpdate(cmdDatas.ToList(), out resultDescript);
            }

            return result;
        }

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
        public void ControlSecurityUpdate(int[] meterIndex, string[] beforeInfos, string KeyKind, string OutKey, string OutKeyinfo, out bool[] results, out string[] resultDescript)
        {

            bool[] outResults = new bool[meterPositions.Length];
            string[] outResultDescription = new string[meterPositions.Length];
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag) return;

                int keyCode = 0;
                if (!string.IsNullOrEmpty(beforeInfos[meterIndex[bw]]) &&
                    beforeInfos[meterIndex[bw]].Length > 7)
                {
                    keyCode = Convert.ToInt32(beforeInfos[meterIndex[bw]].Substring(6, 2));
                }

                if (keyCode < 1)
                {
                    outResults[meterIndex[bw]] = MainSecurityUpdate(meterIndex[bw], KeyKind, OutKey, OutKeyinfo, out outResultDescription[meterIndex[bw]]);
                }
            };

            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

            results = outResults;
            resultDescript = outResultDescription;
        }

        /// <summary>
        /// 参数命令密钥更新
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="keyKind"></param>
        /// <param name="outKey"></param>
        /// <param name="outKeyInfo"></param>
        /// <param name="resultDescript"></param>
        /// <returns></returns>
        private bool ArguSecurityUpdate(int meterIndex, string keyKind, string outKey, string outKeyInfo, out string resultDescript)
        {
            bool result = false;
            resultDescript = string.Empty;
            Debug.Assert(meterPositions != null
               && meterPositions[meterIndex - 1] != null
               && meterPositions[meterIndex - 1].Meter != null);
            if (meterPositions != null
                && meterPositions[meterIndex - 1] != null
                && meterPositions[meterIndex - 1].Meter != null)
            {
                byte[] cmdDatas = Dlt645_2007Helper.ArguSecurityUpdateProtocol(meterPositions[meterIndex - 1].Meter.Address, keyKind, outKey, outKeyInfo).ToArray();
                SendCmd(meterIndex, ref cmdDatas);
                result = Dlt645_2007Helper.AnalyzeSecurityUpdate(cmdDatas.ToList(), out resultDescript);
            }

            return result;
        }

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
        public void ArguSecurityUpdate(int[] meterIndex, string[] beforeInfos, string KeyKind, string OutKey, string OutKeyinfo, out bool[] results, out string[] resultDescript)
        {

            bool[] outResults = new bool[meterPositions.Length];
            string[] outResultDescription = new string[meterPositions.Length];
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag) return;

                int keyCode = 0;
                if (!string.IsNullOrEmpty(beforeInfos[meterIndex[bw]]) &&
                    beforeInfos[meterIndex[bw]].Length > 7)
                {
                    keyCode = Convert.ToInt32(beforeInfos[meterIndex[bw]].Substring(6, 2));
                }

                if (keyCode < 1)
                {
                    outResults[meterIndex[bw]] = MainSecurityUpdate(meterIndex[bw], KeyKind, OutKey, OutKeyinfo, out outResultDescription[meterIndex[bw]]);
                }
            };

            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

            results = outResults;
            resultDescript = outResultDescription;
        }

        /// <summary>
        /// 身份认证密钥更新
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="keyKind"></param>
        /// <param name="outKey"></param>
        /// <param name="outKeyInfo"></param>
        /// <param name="resultDescript"></param>
        /// <returns></returns>
        private bool AuthSecurityUpdate(int meterIndex, string keyKind, string outKey, string outKeyInfo, out string resultDescript)
        {
            bool result = false;
            resultDescript = string.Empty;
            Debug.Assert(meterPositions != null
               && meterPositions[meterIndex - 1] != null
               && meterPositions[meterIndex - 1].Meter != null);
            if (meterPositions != null
                && meterPositions[meterIndex - 1] != null
                && meterPositions[meterIndex - 1].Meter != null)
            {
                byte[] cmdDatas = Dlt645_2007Helper.AuthSecurityUpdateProtocol(meterPositions[meterIndex - 1].Meter.Address, keyKind, outKey, outKeyInfo).ToArray();
                SendCmd(meterIndex, ref cmdDatas);
                result = Dlt645_2007Helper.AnalyzeSecurityUpdate(cmdDatas.ToList(), out resultDescript);
            }

            return result;
        }

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
        public void AuthSecurityUpdate(int[] meterIndex, string[] beforeInfos, string KeyKind, string OutKey, string OutKeyinfo, out bool[] results, out string[] resultDescript)
        {

            bool[] outResults = new bool[meterPositions.Length];
            string[] outResultDescription = new string[meterPositions.Length];
            MulitThreadManager.Instance.MeterIndex = meterIndex;
            MulitThreadManager.Instance.DoWork = delegate(int bw)
            {
                if (!runFlag) return;

                int keyCode = 0;
                if (!string.IsNullOrEmpty(beforeInfos[meterIndex[bw]]) &&
                    beforeInfos[meterIndex[bw]].Length > 7)
                {
                    keyCode = Convert.ToInt32(beforeInfos[meterIndex[bw]].Substring(6, 2));
                }

                if (keyCode < 1)
                {
                    outResults[meterIndex[bw]] = MainSecurityUpdate(meterIndex[bw], KeyKind, OutKey, OutKeyinfo, out outResultDescription[meterIndex[bw]]);
                }
            };

            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

            results = outResults;
            resultDescript = outResultDescription;
        }

        #endregion

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="MeterIndex"></param>
        /// <param name="cmd"></param>
        private void SendCmd(int MeterIndex, ref byte[] cmd)
        {
            SockPool.Instance.Send(GetPortNameByPortNumber(m_arrMeterPort[MeterIndex - 1]), ref cmd);
        }

        /// <summary>
        /// 等待所有线程完成
        /// </summary>
        private void WaitWorkDone()
        {
            while (true)
            {
                if (!runFlag) break;
                if (MulitThreadManager.Instance.IsWorkDone())
                {
                    runFlag = false;
                    break;
                }
                System.Threading.Thread.Sleep(20);
            }
        }
    }
}
