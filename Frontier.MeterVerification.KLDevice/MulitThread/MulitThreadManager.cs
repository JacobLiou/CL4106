using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Frontier.MeterVerification.KLDevice
{
    class MulitThreadManager : SingletonBase<MulitThreadManager>
    {
        /// <summary>
        /// 最大线程数量
        /// </summary>
        public int MaxThread { get; set; }
        public int[] MeterIndex { get; set; }
        /// <summary>
        /// 每个线程最大任务数
        /// </summary>
        public int MaxTaskCountPerThread { get; set; }

        /// <summary>
        /// 工作线程数组
        /// </summary>
        private List<WorkThread> lstWorkThread = new List<WorkThread>();

        public Action<int> DoWork
        {
            private get;
            set;
        }
        /// <summary>
        /// 启动线程
        /// </summary>
        /// <returns>启动线程是否成功</returns>
        public bool Start()
        {
            //结束上一次的线程

            for (int i = 0; i < MeterIndex.Length && i < MaxThread; i++)
            {
                if (MeterIndex.Length > 30)
                {
                    if (i == MeterIndex.Length / 2 - 1)
                    {
                        System.Threading.Thread.Sleep(5000);
                    }
                }
                WorkThread newThread = new WorkThread()
                {
                    ThreadID = MeterIndex[i],                      //线程编号,用于线程自己推导起始位置
                    TaskCount = MaxTaskCountPerThread,

                    DoWork = this.DoWork
                };
                lstWorkThread.Add(newThread);
                newThread.Start();
            }
            return true;
        }

        /// <summary>
        /// 停止所有工作线程
        /// </summary>
        public void Stop()
        {
            //首先发出停止指令
            foreach (WorkThread workthread in lstWorkThread)
            {
                workthread.Stop();
            }
            //等待所有工作线程都完成
            bool isAllThreaWorkDone = false;
            while (!isAllThreaWorkDone)
            {
                isAllThreaWorkDone = IsWorkDone();
            }

        }

        /// <summary>
        /// 等待所有线程工作完成
        /// </summary>
        public bool IsWorkDone()
        {
            bool isAllThreaWorkDone = true;

            foreach (WorkThread workthread in lstWorkThread)
            {
                isAllThreaWorkDone = workthread.IsWorkFinish();
                if (!isAllThreaWorkDone) break;
            }
            if (isAllThreaWorkDone)
            {
                Debug.Print("当前操作已经完成!");

            }
            return isAllThreaWorkDone;
        }
    }
}
