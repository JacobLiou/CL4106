using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace pwClassLibrary.pwProtocol
{

    /// <summary>
    /// 线程池：线程数量不限、可以任意调用、线程不够时自动增加。
    /// 经测试使用本类的静态函数QueueUserWorkItem比直接申明线程对象快600倍
    /// </summary>
    public class ThreadPool
    {
        private static List<ClouThreadPoolItem> LstThreadPoolItem = new List<ClouThreadPoolItem>();

        private static object LstThreadPoolItemLock = new object();

        private static int intCursor = 0; //游标

        private ThreadPool()
        {
            //不让实例化
        }
        ~ThreadPool()
        {
        }
        #region static bool QueueUserWorkItem(WaitCallback callBack)
        /// <summary>
        /// 获取一个线程调用
        /// </summary>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public static bool QueueUserWorkItem(WaitCallback callBack)
        {
            return QueueUserWorkItem(callBack, null);
        }
        #endregion

        #region static bool QueueUserWorkItem(WaitCallback callBack,object objParam)
        /// <summary>
        /// 获取一个线程调用
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="objParam"></param>
        /// <returns></returns>
        public static bool QueueUserWorkItem(WaitCallback callBack, object objParam)
        {
            lock (LstThreadPoolItemLock)
            {
                int lastCuror = intCursor;
                int poolItemCount = LstThreadPoolItem.Count;

                while (++intCursor < poolItemCount)
                {
                    if (LstThreadPoolItem[intCursor].QueueUserWorkItem(callBack, objParam))
                    {
                        return true;
                    }
                }

                intCursor = -1;

                while (++intCursor <= lastCuror && poolItemCount > 0)
                {
                    if (LstThreadPoolItem[intCursor].QueueUserWorkItem(callBack, objParam))
                    {
                        return true;
                    }
                }

                //如果没有空闲线程、则增加一个线程到线程池
                LstThreadPoolItem.Add(new ClouThreadPoolItem());

                return QueueUserWorkItem(callBack, objParam);
            }
        }
        #endregion

        #region 线程池中线程的数量static int ThreadCount
        /// <summary>
        /// 线程池中线程的数量
        /// </summary>
        public static int ThreadCount
        {
            get
            {
                lock (LstThreadPoolItemLock)
                {
                    return LstThreadPoolItem.Count;
                }
            }
        }
        #endregion


        #region 清理多余线程序void ClearUnUsedThread()
        /// <summary>
        /// 清理多余线程序
        /// </summary>
        private static void ClearUnUsedThread()
        {
        }
        #endregion

    }


    

    class ClouThreadPoolItem
    {
        Thread thread;
        WaitCallback CallBack;     //回调
        Object CallBackParameters;  //回调参
        Semaphore Smaph;            //信号量
        bool IsFree;                //当先是否空闲
        bool IsKeepAlive;           //是否需要保持存或状态
        object objIsFreeLock;
        TimeSpan TsLastFree;        //上次空闲时间

        public ClouThreadPoolItem()
        {
            IsKeepAlive = true;
            TsLastFree = new TimeSpan(DateTime.Now.Ticks);
            objIsFreeLock = new object();
            Smaph = new Semaphore(1, 1);
            Smaph.WaitOne();
            thread = new Thread(new ThreadStart(DoLoopWork));
            thread.Priority = ThreadPriority.Normal;
            thread.IsBackground = true;
            thread.Start();
        }

        public bool QueueUserWorkItem(WaitCallback callBack)
        {
            lock (objIsFreeLock)
            {
                if (!IsFree) return false;
                IsFree = false;
                this.CallBack = callBack;
                this.CallBackParameters = null;
                Smaph.Release();
                return true;
            }
        }

        public bool QueueUserWorkItem(WaitCallback callBack, object objParam)
        {
            lock (objIsFreeLock)
            {
                if (!IsFree) return false;
                IsFree = false;
                this.CallBack = callBack;
                this.CallBackParameters = objParam;
                Smaph.Release();
                return true;
            }
        }

        /// <summary>
        /// 待最后一次任务执行完成以后，结束本线程
        /// </summary>
        public void Abort()
        {
            lock (objIsFreeLock)
            {
                IsFree = false;
                IsKeepAlive = false;
            }
        }

        /// <summary>
        /// 本线程已经空闲的秒数(若当前在非空闲状态,则返回-1)
        /// </summary>
        /// <returns></returns>
        public long FreeSecond
        {
            get
            {
                lock (objIsFreeLock)
                {
                    if (!IsFree) return -1;
                    TimeSpan tsSub = new TimeSpan(DateTime.Now.Ticks).Subtract(TsLastFree);
                    return tsSub.Hours * 3600 + tsSub.Minutes * 60 + tsSub.Seconds;
                }
            }
        }

        /// <summary>
        /// 线程是否存在（false = 已经消亡）
        /// </summary>
        public bool IsAlive
        {
            get
            {
                return thread.IsAlive;
            }
        }


        #region 循环等待接受任务protected void DoLoopWork()
        /// <summary>
        /// 循环等待接受任务
        /// </summary>
        protected void DoLoopWork()
        {
            while (IsKeepAlive)
            {
                lock (objIsFreeLock)
                {
                    IsFree = true;
                }
                TsLastFree = new TimeSpan(DateTime.Now.Ticks);
                Smaph.WaitOne();

                try
                {
                    CallBack(CallBackParameters);
                }
                catch (Exception ex)
                {
                    //写日志
#if DEBUG
                    System.Windows.Forms.MessageBox.Show(string.Format("线程池发生错误!\r\n{0}\r\n具体情况请查看日志文件", ex.Message), "CTP线程池发生错误!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
#endif
                }
            }
        }
        #endregion

    }
}
