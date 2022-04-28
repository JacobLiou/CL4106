using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace CL4100
{

    public partial class Monitor : Form
    {
        public Monitor()
        {
            InitializeComponent();
            this.TopLevel = false;

            label1.MouseDown += new MouseEventHandler(FormMove);
            label3.MouseDown += new MouseEventHandler(FormMove);
            label5.MouseDown += new MouseEventHandler(FormMove);
            label7.MouseDown += new MouseEventHandler(FormMove);
            label9.MouseDown += new MouseEventHandler(FormMove);
            label11.MouseDown += new MouseEventHandler(FormMove);
            label13.MouseDown += new MouseEventHandler(FormMove);
            label15.MouseDown += new MouseEventHandler(FormMove);
            label17.MouseDown += new MouseEventHandler(FormMove);
            Lab_Ua.MouseDown += new MouseEventHandler(FormMove);
            Lab_Ia.MouseDown += new MouseEventHandler(FormMove);
            Lab_Pa.MouseDown += new MouseEventHandler(FormMove);
            Lab_Ub.MouseDown += new MouseEventHandler(FormMove);
            Lab_Ib.MouseDown += new MouseEventHandler(FormMove);
            Lab_Pb.MouseDown += new MouseEventHandler(FormMove);
            Lab_Uc.MouseDown += new MouseEventHandler(FormMove);
            Lab_Ic.MouseDown += new MouseEventHandler(FormMove);
            Lab_Pc.MouseDown += new MouseEventHandler(FormMove);


            this.Load += new EventHandler(Monitor_Load);
        }


        void Monitor_Load(object sender, EventArgs e)
        {
            Btn_Expend_Click(sender, e);
        }


        #region 窗体拖曳
        public const int WM_SYSCOMMAND = 0x0112;
        [DllImport("user32.dll")]
        public extern static bool ReleaseCapture();
        private void FormMove(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            pwClassLibrary.pwWin32Api.Win32Api.SendMessage(this.Handle.ToInt32(), WM_SYSCOMMAND, 0xF017, 0);
        }
        #endregion

        /// <summary>
        /// A相电压，如果没有则传入一个空字符串
        /// </summary>
        public string Ua
        {
            get
            {
                if (Lab_Ua.Text == "")
                    return "";
                else
                    return Lab_Ua.Text;
            }
            set
            {
                if (value == null || value == "")
                    Lab_Ua.Text = "";
                else
                    Lab_Ua.Text = value;
            }
        }
        /// <summary>
        /// A相电压，如果没有则传入一个空字符串
        /// </summary>
        public string Ub
        {
            get
            {
                if (Lab_Ub.Text == "")
                    return "";
                else
                    return Lab_Ub.Text;
            }
            set
            {
                if (value == null || value == "")
                    Lab_Ub.Text = "";
                else
                    Lab_Ub.Text = value;
            }
        }
        /// <summary>
        /// B相电压，如果没有则传入一个空字符串
        /// </summary>
        public string Uc
        {
            get
            {
                if (Lab_Uc.Text == "")
                    return "";
                else
                    return Lab_Uc.Text;
            }
            /// <summary>
            /// C相电压，如果没有则传入一个空字符串
            /// </summary>
            set
            {
                if (value == null || value == "")
                    Lab_Uc.Text = "";
                else
                    Lab_Uc.Text = value;
            }
        }

        /// <summary>
        /// A相电流，如果没有则传入一个空字符串
        /// </summary>
        public string Ia
        {
            get
            {
                if (Lab_Ia.Text == "")
                    return "";
                else
                    return Lab_Ia.Text;
            }
            set
            {
                if (value == null || value == "")
                    Lab_Ia.Text = "";
                else
                    Lab_Ia.Text = value;
            }
        }
        /// <summary>
        /// B相电流，如果没有则传入一个空字符串
        /// </summary>
        public string Ib
        {
            get
            {
                if (Lab_Ib.Text == "")
                    return "";
                else
                    return Lab_Ib.Text;
            }
            set
            {
                if (value == null || value == "")
                    Lab_Ib.Text = "";
                else
                    Lab_Ib.Text = value;
            }
        }
        /// <summary>
        /// C相电流，如果没有则传入一个空字符串
        /// </summary>
        public string Ic
        {
            get
            {
                if (Lab_Ic.Text == "")
                    return "";
                else
                    return Lab_Ic.Text;
            }
            /// <summary>
            /// C相电压，如果没有则传入一个空字符串
            /// </summary>
            set
            {
                if (value == null || value == "")
                    Lab_Ic.Text = "";
                else
                    Lab_Ic.Text = value;
            }
        }
        /// <summary>
        /// A相角度，如果没有则传入一个空字符串
        /// </summary>
        public string Phia
        {
            get
            {
                if (Lab_Pa.Text == "")
                    return "";
                else
                    return Lab_Pa.Text;
            }
            set
            {
                if (value == null || value == "")
                    Lab_Pa.Text = "";
                else
                    Lab_Pa.Text = value;
            }
        }
        /// <summary>
        /// B相角度，如果没有则传入一个空字符串
        /// </summary>
        public string Phib
        {

            get
            {
                if (Lab_Pb.Text == "")
                    return "";
                else
                    return Lab_Pb.Text;
            }
            set
            {
                if (value == null || value == "")
                    Lab_Pb.Text = "";
                else
                    Lab_Pb.Text = value;
            }
        }
        /// <summary>
        /// C相角度，如果没有则传入一个空字符串
        /// </summary>
        public string Phic
        {
            get
            {
                if (Lab_Pc.Text == "")
                    return "";
                else
                    return Lab_Pc.Text;
            }
            /// <summary>
            /// C相电压，如果没有则传入一个空字符串
            /// </summary>
            set
            {
                if (value == null || value == "")
                    Lab_Pc.Text = "";
                else
                    Lab_Pc.Text = value;
            }
        }
        /// <summary>
        /// 设置显示数据
        /// </summary>
        /// <param name="tagPower"></param>
        public void SetMonitorData(pwInterface.stPower tagPower)
        {
            MonitorConfig = tagPower;
        }


        /// <summary>
        /// 根据Power结构体获取和设置监视器
        /// </summary>
        /// 
        private pwInterface.stPower MonitorConfig
        {

            get
            {
                pwInterface.stPower _Power = new pwInterface.stPower();
                _Power.Ua = (Ua == "" ? 0F : float.Parse(Ua));
                _Power.Ub = (Ub == "" ? 0F : float.Parse(Ub));
                _Power.Uc = (Uc == "" ? 0F : float.Parse(Uc));
                _Power.Ia = (Ia == "" ? 0F : float.Parse(Ia));
                _Power.Ib = (Ib == "" ? 0F : float.Parse(Ib));
                _Power.Ic = (Ic == "" ? 0F : float.Parse(Ic));
                _Power.Phi_Ia = (Phia == "" ? 0F : float.Parse(Phia));
                _Power.Phi_Ib = (Phib == "" ? 0F : float.Parse(Phib));
                _Power.Phi_Ic = (Phic == "" ? 0F : float.Parse(Phic));
                return _Power;
            }
            set
            {
                Ua = value.Ua.ToString("F4");
                Ub = value.Ub.ToString("F4");
                Uc = value.Uc.ToString("F4");
                Ia = value.Ia.ToString("F4");
                Ib = value.Ib.ToString("F4");
                Ic = value.Ic.ToString("F4");
                Phia = value.Phi_Ia.ToString("F3");
                Phib = value.Phi_Ib.ToString("F3");
                Phic = value.Phi_Ic.ToString("F3");
            }
        }

        /// <summary>
        /// 设置单相、三相模式
        ///  DanXiangTai = true 为单相模式，否则为三相模式
        /// </summary>
        public bool DanXiangTai
        {
            set
            {
                if (value)
                {
                    //设置到单相模式
                    label1.Text = "U(V)";
                    label3.Text = "I(A)";
                    label5.Text = "Phi";
                    this.Height = 25;
                }
                else
                {
                    label1.Text = "Ua(V)";
                    label7.Text = "Ub(V)";
                    label13.Text = "Uc(V)";
                    label3.Text = "Ia(A)";
                    label9.Text = "Ib(A)";
                    label15.Text = "Ic(A)";
                    label5.Text = "Phia";
                    label11.Text = "Phib";
                    label17.Text = "Phic";
                    this.Height = 70;
                }

            }
        }

        public void Btn_Expend_Click(object sender, EventArgs e)
        {
            if (this.Parent == null) return;
            Control CtrlParent = this.Parent;

            /* 只要不是在 收缩 状态、就收缩*/
            int top =0;// 28;// 
            if (CtrlParent is pw_Main)//UI.UI_Client
            {
                top = 0;//6;//  28;//
            }

            if (this.Location.X == CtrlParent.Width - 30
                && this.Location.Y == top )
            {
                //当前在收缩 状态 -> 展开
                this.Location = new Point(CtrlParent.Width - this.Width, top);
            }
            else
            {
                //非 收缩 状态  -> 收缩
                this.Location = new Point(CtrlParent.Width - 30, top);
            }

        }


    }
}
