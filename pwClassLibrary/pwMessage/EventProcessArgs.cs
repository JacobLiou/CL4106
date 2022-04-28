using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pwClassLibrary
{
    [Serializable()]
    public class EventProcessArgs : EventMessageArgs
    {
        private float _TotalMinute;//总共需要分钟
        private float _PastMinute;//当前已经PASS时间
        private int _CurPos;//当前进度
        private string _OtherMessage;//其它信息
        public EventProcessArgs()
            : base()
        {

        }


        /// <summary>
        /// 需要处理总时间(分)
        /// </summary>
        public float TotalTime
        {
            set { _TotalMinute = value; }
            get { return _TotalMinute; }
        }
        /// <summary>
        /// 当前已经进行时间
        /// </summary>
        public float PastTime
        {
            set
            {
                _PastMinute = value;
                //设置进度
                if (_TotalMinute == 0)
                    _CurPos = 0;
                else
                {
                    _CurPos = (int)(_PastMinute / _TotalMinute) * 100;
                }
            }
            get { return _PastMinute; }
        }
        /// <summary>
        /// 当前进度百分数
        /// </summary>
        public int Process
        {
            get
            {
                return _CurPos;
            }

        }
        /// <summary>
        /// 其它信息，主要由应用的检定项目确定数据功能.目前约定如下：
        /// 预热试验：无数据
        /// 起动试验：如果被检表已经收到脉冲，则此字段记录第一个脉冲出来时间
        /// 潜动试验：同起动试验
        /// 走字试验：标准表电量/误差板脉冲计数
        /// 多功能试验：待定
        /// </summary>
        public string OtherMessage
        {
            set { _OtherMessage = value; }
            get { return _OtherMessage; }
        }

        /// <summary>
        /// 检定完毕
        /// </summary>
        public override bool VerifyOver
        {
            get
            {
                return base.VerifyOver;
            }
            set
            {
                base.VerifyOver = value;
                if (value == true)
                {
                    PastTime = 0;
                }
            }
        }

    }
}
