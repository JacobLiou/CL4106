using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace pwClassLibrary
{
    /// <summary>
    /// 消息管理器。
    /// 说明：
    ///      负责消息的添加，清空管理。提供线程处理入口及消息事件委托。 系统在运行过程中采用消息驱动机制更新数据。刷新UI。
    /// </summary>
    /// <example>以下示例展示如何使用消息队列
    /// <code>
    /// //实例化一个消息队列
    /// VerifyMsgControl myMsg= new VerifyMsgControl();
    /// //消息队列类型为:消息队列
    /// myMsg.IsMsg=true;
    /// //消息线程轮循时间为20ms
    /// myMsg.SleepTime=20;
    /// //指定消息到达时处理方法
    /// myMsg.ShowMsg+=new OnShowMsg(myShowMsg);
    /// //实例化一个消息线程
    /// Thread myMsgThread= new Thread(myMsg.DoWork);
    /// myMsgThread.Start();
    /// 
    /// ///消息处理函数
    /// private void myShowMsg(object sender, object E)
    /// {
    ///     Comm.MessageArgs.EventMessageArgs _Message 
    ///         = E as Comm.MessageArgs.EventMessageArgs;
    ///     /*
    ///         消息处理过程
    ///     */
    /// }
    /// </code>
    /// </example>
    public class VerifyMsgControl
    {
        #region ----------公共成员----------
        /// <summary>
        /// 消息事件委托，
        /// </summary>
        /// <param name="sender">发生者</param>
        /// <param name="E">消息参数</param>
        public delegate void OnShowMsg(object sender, object E);
        /// <summary>
        /// 消息委托，当有消息到达时触发
        /// </summary>
        public OnShowMsg ShowMsg;

        /// <summary>
        /// 消息轮循间隔(ms)，此越小处理消息速度越快
        /// </summary>
        public int SleepTime = 50;

        /// <summary>
        /// 是否是消息队列，为TRUE时为消息队列，为FALSE时为数据队列
        /// </summary>
        public bool IsMsg = true;

        /// <summary>
        /// 队列最大成员数量。多余部分则删除掉
        /// </summary>
        public int MaxItem = 50;
        #endregion

        #region ----------私有成员----------
        /// <summary>
        /// 队列对象
        /// </summary>
        private Queue<pwClassLibrary.StVerifyMsg> lstMsg = new Queue<pwClassLibrary.StVerifyMsg>();
        /// <summary>
        /// 线程读取锁
        /// </summary>
        private object objLock = new object();
        /// <summary>
        /// 线程写锁
        /// </summary>
        private object objAddLock = new object();
        #endregion

        /// <summary>
        /// 应该用程序退出
        /// </summary>
        public bool ApplicationIsOver = false;

        /// <summary>
        /// 清除当前所有没处理的消息
        /// </summary>
        public void ClearCache()
        {
            lstMsg.Clear();
        }

        /// <summary>
        /// 取当前消息队列中的消息数量
        /// </summary>
        public int Count
        {
            get
            {
                return lstMsg.Count;
            }
        }

        #region ---------消息队列添加---------

        /// <summary>
        /// 添加消息/数据到队列中
        /// </summary>
        /// <param name="sender">消息发出者</param>
        /// <param name="e">消息参数</param>
        public void AddMsg(object sender, object e)
        {
            //if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
            //    return;
            try
            {
                StVerifyMsg _Msg = new StVerifyMsg();
                _Msg.objSender = sender;
                //移除已经过期的消息
                while (lstMsg.Count > MaxItem)
                {
                    StVerifyMsg m = lstMsg.Dequeue();
                    Console.WriteLine("move one message");
                }
                if (IsMsg)
                {
                    _Msg.objEventArgs = (EventArgs)e;
                    //进度消息不重复添加
                    if (e is EventProcessArgs)
                    {
                        ClearCache();
                    }
                    else if (e is EventMessageArgs)
                    {
                        //清空队列
                        if (((EventMessageArgs)e).MessageType == enmMessageType.清空消息队列)
                        {
                            ClearCache();
                            return;
                        }
                        else if (((EventMessageArgs)e).MessageType == enmMessageType.提示消息)
                        {
                            //线程消息过虑
                            if (((EventMessageArgs)e).Message.IndexOf("线程") != -1 ||
                                ((EventMessageArgs)e).Message.IndexOf("Thread was") != -1)
                            {
                                return;
                            }
                        }
                    }

                }
                else
                {
                    _Msg.cmdData = (pwSerializable)e;
                }
                lstMsg.Enqueue(_Msg);

            }
            catch (Exception ex)
            {
                if (!(ex is ThreadAbortException))
                {
                    string logPath = "/Log/Thread/MsgThread-" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                    //pwFunction.pwConst.GlobalUnit.g_Log.WriteLog(logPath, this, "AddMsg", ex.Message + "\r\n" + ex.StackTrace);
                }
#if DEBUG
                throw ex;
#endif
            }

        }

        #endregion

        #region----------消息/数据队列处理-DoWork()---------
        /// <summary>
        /// 消息处理线程，确保只有一个线程调用
        /// </summary>
        public void DoWork()
        {
            while (true)
            {
                if (ApplicationIsOver)
                    break;
                if (lstMsg.Count > 0)
                {
                    try
                    {
                        StVerifyMsg _Msg = lstMsg.Dequeue();

                        if (ShowMsg != null)
                        {
                            if (IsMsg)
                            {
                                //消息队列处理
                                ShowMsg(_Msg.objSender, _Msg.objEventArgs);
                            }
                            else
                            {
                                //数据队列处理
                                ShowMsg(_Msg.objSender, _Msg.cmdData);
                            }
                        }
                    }
                    catch (InvalidOperationException e)
                    {
                        //消息队列为空时的意外处理.
                    }
                    catch (Exception ex)
                    {
                        string logPath = "/Log/Thread/MsgThread-" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                        //RunLog.WriteLog(logPath, this, "DoWork", ex.Message + "\r\n" + ex.StackTrace);
#if DEBUG
                        //throw ex;
#endif
                    }
                }
                Thread.Sleep(SleepTime);
            }
            // Console.WriteLine("消息线程已经退出");
            string logThreadPath = "/Log/Thread/MsgThreadInfo.log";
            //pwFunction.pwConst.GlobalUnit.g_Log.WriteLog(logThreadPath, this, "DoWork", Thread.CurrentThread.Name + "退出");

        }
        #endregion

        #region----------消息泵----------
        /// <summary>
        /// 外发消息:只刷新数据
        /// </summary>
        public void OutMessage()
        {
            OutMessage("null");
        }

        /// <summary>
        /// 外发检定消息[默认为运行时消息，需要刷新数据]
        /// </summary>
        /// <param name="strMessage"></param>
        public void OutMessage(string strMessage)
        {
            EventMessageArgs _Message = new EventMessageArgs();
            _Message.MessageType = enmMessageType.运行时消息;
            _Message.Message = strMessage;
            OutMessage(_Message);
        }

        /// <summary>
        /// 外发检定消息[默认为运行时消息，可设置是否需要刷新数据]
        /// </summary>
        /// <param name="strMessage"></param>
        /// <param name="RefreshData"></param>
        public void OutMessage(string strMessage, bool RefreshData)
        {
            EventMessageArgs _Message = new EventMessageArgs();
            _Message.MessageType = enmMessageType.运行时消息;
            _Message.Message = strMessage;
            _Message.RefreshData = RefreshData;
            OutMessage(_Message);

        }

        /// <summary>
        /// 外发检定消息[可设置是否刷新数据及消息类型]
        /// </summary>
        /// <param name="strMessage"></param>
        /// <param name="RefreshData"></param>
        /// <param name="eType"></param>
        public void OutMessage(string strMessage, bool RefreshData, enmMessageType eType)
        {
            EventMessageArgs _Message = new EventMessageArgs();
            _Message.MessageType = eType;
            _Message.Message = strMessage;
            _Message.RefreshData = RefreshData;
            OutMessage(_Message);
        }

        /// <summary>
        /// 外发检定消息
        /// </summary>
        /// <param name="MessageType"></param>
        public void OutMessage(enmMessageType MessageType)
        {
            EventMessageArgs _E = new EventMessageArgs();
            _E.MessageType = MessageType;
            _E.RefreshData = false;
            OutMessage(_E);
        }

        /// <summary>
        /// 外发检定消息
        /// </summary>
        /// <param name="e"></param>
        public void OutMessage(EventMessageArgs e)
        {
            if (IsMsg)
            {
                AddMsg(this, e);
            }
        }

        ///// <summary>
        ///// 上报局部检定数据
        ///// </summary>
        ///// <param name="BW">表位号，如果为999则为所有表</param>
        ///// <param name="arrStrKey">更新的键值</param>
        ///// <param name="objValue">对应的数据</param>
        ///// <param name="dataType">数据类型</param>
        ///// <param name="isDelete">为True时删除掉键值为strKey的数据</param>
        //public void OutUpdateData(int BW, string[] arrStrKey, object[] objValue, enmMeterDataType dataType, bool isDelete)
        //{
        //    /*上报数据*/
        //    UpdateData_Ask Cmd_UpdateData = new UpdateData_Ask();
        //    Cmd_UpdateData.BW = BW;
        //    Cmd_UpdateData.isDelete = isDelete;
        //    Cmd_UpdateData.strKey = arrStrKey;
        //    Cmd_UpdateData.objData = objValue;
        //    Cmd_UpdateData.DataType = dataType;
        //    //添加到消息队列
        //    AddMsg(this, Cmd_UpdateData);
        //    //RaiseVerifyData(Cmd_UpdateData);
        //    //RaiseUpdateData(this, Cmd_UpdateData);
        //    /*上报数据完毕*/
        //}

        ///// <summary>
        ///// 更新局部数据[如果存在则删除后添加，不存在则直接添加]
        ///// </summary>
        ///// <param name="BW"></param>
        ///// <param name="arrStrKey"></param>
        ///// <param name="objValue"></param>
        ///// <param name="dataType"></param>
        //public void OutUpdateData(int BW, string[] arrStrKey, object[] objValue, enmMeterDataType dataType)
        //{
        //    OutUpdateData(BW, arrStrKey, objValue, dataType, false);
        //}
        #endregion
    }
    ///// <summary>
    ///// 局部数据更新[添加/修改/删除]
    ///// </summary>
    //[Serializable()]
    //public class UpdateData_Ask : pwSerializable
    //{
    //    /// <summary>
    //    /// 操作数据类型
    //    /// </summary>
    //    public enmMeterDataType DataType;
    //    public string[] strKey;  //要更新的键值
    //    public object[] objData; //对应键值的数据
    //    public int BW;           //表位，如果为999则为所有表
    //    public bool isDelete = false;    //是否是删除当前数据
    //    public UpdateData_Ask()
    //    {
    //    }

    //}
}
