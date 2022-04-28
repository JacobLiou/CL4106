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
        /// ��ʾ�ڸǲ�
        /// </summary>
        /// <param name="Parent">���ڸ��������������,������Form����UserControl������</param>
        /// <param name="Notice">�ڸǲ���ʾ����</param>
        /// <param name="IsCover">��ʾ�ڸ�?</param>
        public static void DoCover(object  parent,string Notice,bool IsCover)
        {
            if (!(parent is Form || parent is UserControl))
            {
                throw new Exception("�ڸǺ���ʹ�ò���ȷ!");
            }

            if (parent is Form)
            {
                //���������ԭ�����ڸǲ�ؼ�
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
                //���������ԭ�����ڸǲ�ؼ�
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
            //����ڸǲ�
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
                throw new Exception("�ڸǺ���ʹ�ò���ȷ!");
            }

            if (parent is Form)
            {
                //���������ԭ�����ڸǲ�ؼ�
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
                //���������ԭ�����ڸǲ�ؼ�
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
            //����ڸǲ�
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
        /// ��ʾ�ڸǲ�
        /// </summary>
        /// <param name="Parent">���ڸ�������������������Form����UserControl������</param>
        /// <param name="IsCover">��ʾ�ڸ�?</param>
        public static void DoCover(object parent, bool IsCover)
        {
            DoCover(parent, "ϵͳ���ڴ���...", IsCover);
        }

        /// <summary>
        /// �ȴ���ʾ�� , ���߳���ʾ��������ô���ʾ����߳�����������ͬ����������״̬ 
        /// </summary>
        private Waiting()
        {
            InitializeComponent();
        }

        /// <summary>
        /// �ȴ���ʾ��, ���߳���ʾ��������ô���ʾ����߳�����������ͬ����������״̬ 
        /// </summary>
        /// <param name="strNotice">��ʾ����</param>
        private  Waiting(string strNotice)
        {
            InitializeComponent();
            SetNotice(strNotice);
        }

        /// <summary>
        /// ������ʾ����
        /// </summary>
        /// <param name="strNotice">��ʾ����</param>
        public void SetNotice(string strNotice)
        {
            Lab_Notice.Text = strNotice;
        }
    }
}
