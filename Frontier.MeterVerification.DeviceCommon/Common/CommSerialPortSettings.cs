using System;
using System.Collections.Generic;

using System.Text;
using System.ComponentModel;

namespace Frontier.MeterVerification.DeviceCommon
{
    /// <summary>
    /// 串口通信参数设置
    /// </summary>
    public class CommSerialPortSettings
    {
        public override string ToString()
        {
            return string.Format("Com{0} {1},{2},{3},{4}",
                    this.CommPortNumber,
                    this.BaudRate,
                    this.DataBits,
                    this.Parity,
                    this.StopBits);
        }

        [Description("端口号"), DefaultValue(1)]
        public int CommPortNumber { get; set; }

        [Description("波特率"), DefaultValue(9600)]
        public int BaudRate { get; set; }

        [Description("数据位数"), DefaultValue(8)]
        public int DataBits { get; set; }

        [Description("校验位：N,O,E,M,S"), DefaultValue("N")]
        public string Parity { get; set; }

        [Description("停止位：0,1,2,3"), DefaultValue(1)]
        public int StopBits { get; set; }
    }
}
