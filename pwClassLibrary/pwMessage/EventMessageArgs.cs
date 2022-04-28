using System;
using System.Collections.Generic;
using System.Text;

namespace pwClassLibrary
{

    /// <summary>
    /// 检定控制器消息
    /// </summary>
    [Serializable()]
    public class EventMessageArgs : EventArgs
    {
        public bool RefreshData;        //是否需要刷新数据
        private string _Message;        //提示信息
        private bool _VerifyOver;       //检定完成
        private enmMessageType _MessageType;//消息类型
        public int ActiveItemID;        //报告当前ActiveID
        public EventMessageArgs()
            : base()
        {
            RefreshData = true;         //默认数据需要刷新
        }

        /// <summary>
        /// 运行时消息
        /// </summary>
        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
            
        }
        /// <summary>
        /// 消息类型
        /// </summary>
        public enmMessageType MessageType
        {
            get { return _MessageType; }
            set { _MessageType = (enmMessageType)value; }
        }

        /// <summary>
        /// 检定是否完成
        /// </summary>
        public virtual bool VerifyOver
        {
            set { _VerifyOver = value; }
            get { return _VerifyOver; }
        }
    }
}
