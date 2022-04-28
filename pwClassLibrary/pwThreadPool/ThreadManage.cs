// ***************************************************************
//  Description:
//  本类应用于VerifyAdapter的线程休眠.其它非pwVerifyAdater中的线程或
//  是延伸线程不能用
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace pwClassLibrary.pwProtocol
{
    /// <summary>
    /// 
    /// </summary>
    public class ThreadManage
    {
        /// <summary>
        /// 线程休眠状态。当前监视线程是否已经进入到休眠状态
        /// </summary>
        private static bool m_IsInSleep = false;

        /// <summary>
        /// 是否处于LOCK块中
        /// </summary>
        private static bool m_IsInLock = false;
        /// <summary>
        /// 允许进入休眠标志。为真则不允许线程继续进入休眠状态
        /// </summary>
        public static bool isStop = false;
        /// <summary>
        /// 线程休眠锁
        /// </summary>
        public static object objSleepLock = new object();


        /// <summary>
        /// 当前访问是否是检定线程
        /// </summary>
        /// <returns></returns>
        private static bool isVerifyThread()
        {
            string strCurThreadName = Thread.CurrentThread.Name;
            if (strCurThreadName != null)
            {
                if (strCurThreadName.Substring(0, 7) == "Verify_")
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 分块线程休眠。
        /// </summary>
        /// <param name="SleepTime"></param>
        public static void Sleep(int SleepTime)
        {
            if (Stop) return;
            int space = 50;
            int sleepCount = 0;
            //if (SleepTime < space)
            space = SleepTime;
            string strCurThreadName = Thread.CurrentThread.Name;

            if (isVerifyThread())
            {
                // while (true)
                //{
                if (Stop) return;
                //Console.WriteLine("{0}在临界区外",Thread.CurrentThread.Name);
                lock (objSleepLock)
                {
                    //Console.WriteLine("{0}进入临界区", strCurThreadName);
                    if (Stop) return;
                    IsInSleep = true;
                    Thread.Sleep(space);
                    IsInSleep = false;
                    // sleepCount += space;
                    // if (sleepCount >= SleepTime) break;
                    //Console.WriteLine("{0}Wait sleep");
                    // }
                    // Console.WriteLine("{0}退出临界区", strCurThreadName);
                }
                IsInSleep = false;
            }
            else
            {
                Thread.Sleep(SleepTime);
            }
            return;
        }
        /// <summary>
        /// 当前是否处于休眠中
        /// </summary>
        public static bool IsInSleep
        {
            get { return m_IsInLock; }
            set
            {
                if (isVerifyThread())
                {
                    m_IsInLock = value;
                    //Console.WriteLine("{0}{1}Sleep临界区", Thread.CurrentThread.Name, value.ToString());
                }
            }
        }

        public static void IsInLockA(bool Value,object sender)
        {
            IsInLock = Value;
            //Console.WriteLine("IsInLock Be Modifyed by{0}",sender.ToString());
        }
        /// <summary>
        /// 当前是否处于LOCK块内
        /// </summary>
        public static bool IsInLock
        {
            get { return m_IsInLock; }
            set
            {
                if (isVerifyThread())
                {
                    m_IsInLock = value;
                    //Console.WriteLine("{0}{1}LOCK临界区", Thread.CurrentThread.Name, value.ToString());
                }
            }
        }
        /// <summary>
        /// 当前线程是否可控制
        /// </summary>
        /// <returns></returns>
        public static bool IsFree()
        {
            if (!IsInSleep && !IsInLock)
                return true;
            return false;
        }



        private static bool Stop
        {
            get
            {
                return false;
                //return GlobalUnit.ForceVerifyStop ||
                //    isStop || GlobalUnit.ApplicationIsOver;
            }
        }
    }
}
