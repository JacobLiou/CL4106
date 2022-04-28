using System;
using System.Collections.Generic;
using System.Text;

namespace VerificationEquipment.Commons
{
    /// <summary>
    /// 设备消息事件参数
    /// </summary>
    public class EquipmentMessageEventArgs : EventArgs
    {
        private string message;

        public EquipmentMessageEventArgs(string message)
        {
            this.message = message;
        }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
            }
        }
    }
}
