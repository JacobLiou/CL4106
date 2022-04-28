using System;
using System.Collections.Generic;

using System.Text;
using System.ComponentModel;

namespace Frontier.MeterVerification.DeviceCommon
{
    /// <summary>
    /// 网口设置
    /// </summary>
    public class CommSocketSettings
    {
        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}",
                    this.IP,
                    this.Port1,
                    this.Port2);
        }

        [Description("IP地址"), DefaultValue("192.168.0.1")]
        public string IP { get; set; }

        [Description("端口号1"), DefaultValue(1000)]
        public int Port1 { get; set; }

        [Description("端口号1"), DefaultValue(1001)]
        public int Port2 { get; set; }
    }
}
