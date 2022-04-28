using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace CL4100
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();

            Application.SetCompatibleTextRenderingDefault(false);
            
            System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            IntPtr hMutex;
            hMutex = CreateMutex(null, false, "CL4101_12");//自己修改名字
            if (GetLastError() == ERROR_ALREADY_EXISTS)
            {
                //MessageBox.Show("App Runing!", "info", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                ReleaseMutex(hMutex);
                return;
            }


            /*进程清理*/
            //【该部分功能:避免重复启动多次程序】
            Process curPro = Process.GetCurrentProcess();
            string proc = curPro.ProcessName;
            Process[] processes = Process.GetProcessesByName(proc);
            if (processes.Length > 1)
            {
                for (int i = 0; i < processes.Length; i++)
                {
                    if (processes[i].Id != curPro.Id)
                    {
                        processes[i].Kill();
                    }
                }
            }

            #region 启动主界面
            Application.Run(new pw_Main());
            
            #endregion
        }

        #region 调用API

        [StructLayout(LayoutKind.Sequential)]//只允许启动一个API函数
        public class SECURITY_ATTRIBUTES
        {
            public int nLength;
            public int lpSecurityDescriptor;
            public int bInheritHandle;
        }
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern int GetLastError();
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern IntPtr CreateMutex(SECURITY_ATTRIBUTES lpMutexAttributes, bool bInitialOwner, string lpName);

        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern int ReleaseMutex(IntPtr hMutex);
        const int ERROR_ALREADY_EXISTS = 0183;
        #endregion


    }
}
