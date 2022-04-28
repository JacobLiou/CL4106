using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;
using pwClassLibrary.pwWin32Api;

namespace pwClassLibrary
{

    /// <summary>
    /// 顶层等待框，和Waiting不同的是：这个等待框不会因为调用线程阻塞而阻塞
    /// 系统关闭以前如果还有等待框没有释放，请调用HideWaiting()以关闭对话框。
    /// </summary>
    public partial class TopWaiting : Form
    {

        #region =============非静态成员================
        public object owner = null;
        public TopWaiting()
        {
            InitializeComponent();
            Table_Main_2.MouseDown += new MouseEventHandler(FormMove);
            Lab_Notice.MouseDown += new MouseEventHandler(FormMove);
            pictureBox1.MouseDown += new MouseEventHandler(FormMove);

            ////设置阴影效果
            //const int CS_DropSHADOW = 0x20000;
            //const int GCL_STYLE = (-26);
            //Win32Api.SetClassLong(this.Handle, GCL_STYLE, Win32Api.GetClassLong(this.Handle, GCL_STYLE) | CS_DropSHADOW);

            //SetWindowShadow(60);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Parent = Win32Api.GetWindow();
                return cp;
            }
        }

        private void SetWindowShadow(byte bAlpha)
        {
            Win32Api.SetWindowLong(this.Handle, (int)Win32Api.WindowStyle.GWL_EXSTYLE,
            Win32Api.GetWindowLong(this.Handle, (int)Win32Api.WindowStyle.GWL_EXSTYLE) | (uint)Win32Api.ExWindowStyle.WS_EX_LAYERED);
            Win32Api.SetLayeredWindowAttributes(this.Handle, 0, bAlpha, Win32Api.LWA_COLORKEY | Win32Api.LWA_ALPHA);
        }


        public TopWaiting(string Notice)
            : this()
        {
            if (Notice.Length > 0)
                this.Lab_Notice.Text = Notice;
        }

        public TopWaiting(string Notice, string Title)
            : this()
        {
            if (Notice.Length > 0)
                this.Lab_Notice.Text = Notice;
            if (Title.Length > 0)
                this.Text = Title;
        }
        #region 窗体拖曳
        [DllImport("user32.dll")]
        public extern static long SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        [DllImport("user32.dll")]
        public extern static bool ReleaseCapture();
        private void FormMove(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            Win32Api.SendMessage(this.Handle.ToInt32(), WM_SYSCOMMAND, 0xF017, 0);
        }
        #endregion

        private void Lab_Close_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch
            {
            }
            try
            {
                this.Dispose(true);
            }
            catch { }
        }

        #endregion =============非静态成员=================



        #region =============静态成员=================

        //private static TopWaiting hTopWaiting = null;
        //private static bool bTopWaitingShow = false;
        //private static string strNotice;
        //private static string strTitle;
        //private static object objLock = new object();

        private static List<TopWaiting> LstHTopWaiting = new List<TopWaiting>();    //用于保存所有

        private static Semaphore SeampLock = new Semaphore(1, 1);

        private static void thShowWaiting(object hObj)
        {
            TopWaiting hTopWaiting = (TopWaiting)hObj;

            Win32Api.POINT CurPos = new Win32Api.POINT();
            if (Win32Api.GetCursorPos(out CurPos))
            {
                //计算当前正在第几个屏幕
                System.Windows.Forms.Screen[] ArScreen = System.Windows.Forms.Screen.AllScreens;
                int index =pwClassLibrary. Screen.GetScreenIndex(CurPos.X, CurPos.Y);
                //设置坐标
                Point PosLoc = new Point((ArScreen[index].WorkingArea.Width - hTopWaiting.Width) / 2, (ArScreen[index].WorkingArea.Height - hTopWaiting.Height) / 2);
                PosLoc.X += ArScreen[index].WorkingArea.Left;
                PosLoc.Y += ArScreen[index].WorkingArea.Top;
                hTopWaiting.Location = PosLoc;
            }
            hTopWaiting.ShowDialog();
        }

        public static void ShowWaiting(string Notice, string Title)
        {
            SeampLock.WaitOne();
            TopWaiting hTopWaiting = new TopWaiting(Notice, Title);
            LstHTopWaiting.Add(hTopWaiting);
            pwClassLibrary.pwProtocol.ThreadPool.QueueUserWorkItem(new WaitCallback(thShowWaiting), hTopWaiting);
            while (!hTopWaiting.IsHandleCreated && !hTopWaiting.IsDisposed)
            {
                Thread.Sleep(1);
            }
            SeampLock.Release();
        }

        public static void ShowWaiting(string Notice)
        {
            ShowWaiting(Notice, "");
        }

        public static void ShowWaiting()
        {
            ShowWaiting("", "");
        }

        /// <summary>
        /// 隐藏等待框
        /// </summary>
        /// <returns></returns>
        public static void HideWaiting()
        {
            SeampLock.WaitOne();
            for (int i = 0; i < LstHTopWaiting.Count; i++)
            {
                try
                {
                    TopWaiting hTopWaiting = LstHTopWaiting[i];
                    try
                    {
                        hTopWaiting.Invoke(new MethodInvoker(hTopWaiting.Close));
                    }
                    catch { }
                }
                catch { }
            }
            LstHTopWaiting.Clear();
            SeampLock.Release();
        }


        #endregion =============静态成员=================



    }
}