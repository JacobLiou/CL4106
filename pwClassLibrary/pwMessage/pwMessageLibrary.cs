using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pwClassLibrary
{
    /// <summary>
    /// 检定器消息结构
    /// </summary>
    public struct StVerifyMsg
    {
        /// <summary>
        /// 消息发送者
        /// </summary>
        public object objSender;
        /// <summary>
        /// 消息参数
        /// </summary>
        public EventArgs objEventArgs;

        /// <summary>
        /// 数据参数
        /// </summary>
        public pwClassLibrary.pwSerializable cmdData;
    }


    /// <summary>
    /// 检定器消息提示类型
    /// </summary>
    public enum enmMessageType
    {
        /// <summary>
        /// 需要UI用消息框提示给用户[不需要交互]
        /// </summary>
        提示消息 = 0,
        /// <summary>
        /// 检定方案或是操作中出现错误。提示此消息后所有检定停止
        /// </summary>
        错误消息 = 1,
        /// <summary>
        /// 运行时的提示信息。提供给操作者参考
        /// </summary>
        运行时消息 = 2,
        /// <summary>
        /// 项目检定点改变,报告给服务器
        /// </summary>
        运行中断 = 3,
        /// <summary>
        /// 所有项目检定完毕
        /// </summary>
        运行完毕 = 4,
        /// <summary>
        /// 内部消息:清空消息队列
        /// </summary>
        清空消息队列 = 5,
        /// <summary>
        /// 内部消息:清空数据队列
        /// </summary>
        清空数据队列 = 6,
    }

    ///// <summary>
    ///// 电能表数据分类
    ///// </summary>
    //public enum enmMeterDataType2
    //{
    //    非法信息 = 0,
    //    工单信息 = 1,
    //    产品信息 = 2,
    //    检定方案 = 3,

    //    读生产编号 = 100,

    //    读电能表底度 = 200,

    //    校准时钟 = 300,

    //    校准误差 = 400,

    //    误差检定 = 500,

    //    多功能检定 = 600,

    //    打包参数下载 = 700,

    //    系统清零 = 800,

    //    插预置卡 = 900,

    //    手动测试 = 1000,

    //    //=========
    //    读表地址 = 101,

    //    写表地址 = 102,

    //    //========
    //    高频检定 = 501,

    //    偏差检定 = 502,

    //    误差检定2回路 = 503,

    //}

}
