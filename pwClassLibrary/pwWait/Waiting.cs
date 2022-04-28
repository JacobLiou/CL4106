using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace pwClassLibrary
{
    public partial class Waiting : UserControl
    {

        /// <summary>
        /// 显示遮盖层
        /// </summary>
        /// <param name="Parent">被遮盖区域的容器对象,可以是Form或者UserControl的子类</param>
        /// <param name="Notice">遮盖层提示文字</param>
        /// <param name="IsCover">显示遮盖?</param>
        public static void DoCover(object  parent,string Notice,bool IsCover)
        {
            if (!(parent is Form || parent is UserControl))
            {
                throw new Exception("遮盖函数使用不正确!");
            }

            if (parent is Form)
            {
                //首先清理掉原来的遮盖层控件
                for (int i = 0; i < ((Form)parent).Controls.Count; i++)
                {
                    if (((Form)parent).Controls[i] is Waiting)
                    {
                        Waiting objWaiting = (Waiting)((Form)parent).Controls[i];
                        ((Form)parent).Controls.RemoveAt(i--);
                        objWaiting.Dispose();
                        objWaiting = null;
                    }
                }
            }
            else
            {
                //首先清理掉原来的遮盖层控件
                for (int i = 0; i < ((UserControl)parent).Controls.Count; i++)
                {
                    if (((UserControl)parent).Controls[i] is Waiting)
                    {
                        Waiting objWaiting = (Waiting)((UserControl)parent).Controls[i];
                        ((UserControl)parent).Controls.RemoveAt(i--);
                        objWaiting.Dispose();
                        objWaiting = null;
                    }
                }
            }
            if (!IsCover)
            {
                //System.Threading.Thread.Sleep(2000);
                return;
            }
            //添加遮盖层
            if (parent is Form)
            {
                Waiting objWaiting = new Waiting(Notice);
                ((Form)parent).Controls.Add(objWaiting);
                ((Form)parent).Controls.SetChildIndex(objWaiting, 0);
                objWaiting.Margin = new Padding(0);
                objWaiting.Dock = DockStyle.Fill;
            }
            else
            {
                Waiting objWaiting = new Waiting(Notice);
                ((UserControl)parent).Controls.Add(objWaiting);
                ((UserControl)parent).Controls.SetChildIndex(objWaiting, 0);
                objWaiting.Margin = new Padding(0);
                objWaiting.Dock = DockStyle.Fill;
            }
        }

        
        private void thSleep(object objParams)
        {
            System.Threading.Thread.Sleep( 2000 );
            this.BeginInvoke( new EventDoCover2(DoCover2),(object[])objParams);
        }

        private delegate void EventDoCover2(object parent, string Notice, bool IsCover);

        private void DoCover2(object parent, string Notice, bool IsCover)
        {
            if (!(parent is Form || parent is UserControl))
            {
                throw new Exception("遮盖函数使用不正确!");
            }

            if (parent is Form)
            {
                //首先清理掉原来的遮盖层控件
                for (int i = 0; i < ((Form)parent).Controls.Count; i++)
                {
                    if (((Form)parent).Controls[i] is Waiting)
                    {
                        Waiting objWaiting = (Waiting)((Form)parent).Controls[i];
                        ((Form)parent).Controls.RemoveAt(i--);
                        objWaiting.Dispose();
                        objWaiting = null;
                    }
                }
            }
            else
            {
                //首先清理掉原来的遮盖层控件
                for (int i = 0; i < ((UserControl)parent).Controls.Count; i++)
                {
                    if (((UserControl)parent).Controls[i] is Waiting)
                    {
                        Waiting objWaiting = (Waiting)((UserControl)parent).Controls[i];
                        ((UserControl)parent).Controls.RemoveAt(i--);
                        objWaiting.Dispose();
                        objWaiting = null;
                    }
                }
            }
            if (!IsCover)
            {
                //System.Threading.Thread.Sleep(2000);
                return;
            }
            //添加遮盖层
            if (parent is Form)
            {
                Waiting objWaiting = new Waiting(Notice);
                ((Form)parent).Controls.Add(objWaiting);
                ((Form)parent).Controls.SetChildIndex(objWaiting, 0);
                objWaiting.Margin = new Padding(0);
                objWaiting.Dock = DockStyle.Fill;
            }
            else
            {
                Waiting objWaiting = new Waiting(Notice);
                ((UserControl)parent).Controls.Add(objWaiting);
                ((UserControl)parent).Controls.SetChildIndex(objWaiting, 0);
                objWaiting.Margin = new Padding(0);
                objWaiting.Dock = DockStyle.Fill;
            }
        }

        /// <summary>
        /// 显示遮盖层
        /// </summary>
        /// <param name="Parent">被遮盖区域的容器对象可以是Form或者UserControl的子类</param>
        /// <param name="IsCover">显示遮盖?</param>
        public static void DoCover(object parent, bool IsCover)
        {
            DoCover(parent, "系统正在处理...", IsCover);
        }

        /// <summary>
        /// 等待提示框 , 本线程提示框，如果调用此提示框的线程阻塞，本框同样处于阻塞状态 
        /// </summary>
        private Waiting()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 等待提示框, 本线程提示框，如果调用此提示框的线程阻塞，本框同样处于阻塞状态 
        /// </summary>
        /// <param name="strNotice">提示文字</param>
        private  Waiting(string strNotice)
        {
            InitializeComponent();
            SetNotice(strNotice);
        }

        /// <summary>
        /// 设置提示文字
        /// </summary>
        /// <param name="strNotice">提示文字</param>
        public void SetNotice(string strNotice)
        {
            Lab_Notice.Text = strNotice;
        }
    }
}
