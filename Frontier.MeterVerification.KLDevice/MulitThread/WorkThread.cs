using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Frontier.MeterVerification.KLDevice
{
    class WorkThread
    {
        Thread workThread = null;

        /// <summary>
        /// 运行标志
        /// </summary>
        private bool runFlag = false;

        /// <summary>
        /// 工作完成标志
        /// </summary>
        private bool workOverFlag = false;

        /// <summary>
        /// 线程编号
        /// </summary>
        public int ThreadID { get; set; }
        /// <summary>
        /// 任务数量
        /// </summary>
        public int TaskCount { get; set; }

        public int DoIndex { get; set; }
        public Action<int> DoWork { get; set; }

        /// <summary>
        /// 停止当前工作任务
        /// </summary>
        public void Stop()
        {
            runFlag = true;
        }

        /// <summary>
        /// 工作线程是否完成
        /// </summary>
        /// <returns></returns>
        public bool IsWorkFinish()
        {
            return workOverFlag;
        }

        /// <summary>
        /// 启动工作线程
        /// </summary>
        /// <param name="paras"></param>
        public void Start()
        {
            workThread = new Thread(StartWork);
            workThread.Start();
        }

        private void StartWork()
        {
            //初始化标志
            runFlag = true;
            workOverFlag = false;
            //计算负载
            int startpos = ThreadID * TaskCount;
            int endpos = startpos + TaskCount;
            //调用方法
            try
            {

                for (int i = startpos; i < endpos; i++)
                {
                    if (DoWork != null)
                    {
                        DateTime startTime = DateTime.Now;

                        Debug.Print(String.Format("开始进行第{0}项工作任务", i + 1));
                        DoWork(i);
                        Debug.Print(String.Format("已经完成第{0}项工作任务", i + 1));

                        TimeSpan ts = DateTime.Now - startTime;
                        double rettime = ts.TotalMilliseconds;
                        Console.WriteLine("单次工作使用时间{0}ms", rettime);
                    }

                    if (!runFlag)
                        break;
                }
            }
            catch { }
            finally
            {
                //恢复标志
                runFlag = false;
                workOverFlag = true;
            }
        }


    }
}
