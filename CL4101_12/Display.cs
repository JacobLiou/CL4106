using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using pwInterface;
using pwFunction.pwConst;
using System.Threading;

namespace CL4100
{

    public partial class Display : UserControl
    {
        private int m_MeterIndex;
        private string m_MeterColor;
        public event DelegateEventChenkedChange OnEventCheckedChange;
        public event DelegateEventBwChange OnEventBwChange;
        public event DelegateEventTxtChange OnEvenTxtChange;

        public Display(int Index, bool bChecked,bool IsShaomiao)
        {
            InitializeComponent();
            m_MeterIndex = Index;
            checkBox1.Checked = bChecked;
            checkBox1.Text = Convert.ToInt32(m_MeterIndex + 1).ToString("D2") + "表";
            SetColor("white");

            Adapter.OnEventResuleChange += new DelegateEventResuleChange(OnEventResuleChanged);
            pw_Main.OnEventResuleChange += new DelegateEventResuleChange(OnEventResuleChanged);
            pw_Main.OnEventStatusChange += new DelegateEventStatusChange(OnEventStatusChanged);

            pw_Main.OnEventTxmtextSet += new DelegateEventTxmtextSet(OnEventTextChanged);



            pictureBox1.Click += new EventHandler(pictureBox1_Click);
            textBox1.Enabled = IsShaomiao;

        }
        public void Doing()
        {
            if (checkBox1.Checked)
                SetColor("wait");
            else
                SetColor("white");

        }
        private void OnEventResuleChanged(int Bw, bool Resule)
        {
            if (m_MeterIndex != Bw) return;
            if (checkBox1.Checked)
            {
                if (!Resule)
                {
                    SetColor("red");
                }
                else
                {
                    if (pwFunction.pwConst.GlobalUnit.g_Status == pwInterface.enmStatus.结束)
                        SetColor("blue");
                }
            }
            if (!Resule) checkBox1.Checked = false;

        }


        private void OnEventTextChanged(int Bw, string   Resule)
        {
            if (m_MeterIndex != Bw) return;
            if (checkBox1.Checked)
            {
                textBox1.Text = Resule;
                textBox1.ForeColor = Color.Red;
            }
           

        }

        private void OnEventStatusChanged(int Bw, enmStatus Status)
        {
            if (m_MeterIndex != Bw) return;
            if (Status == enmStatus.进行中)
            {
                if (checkBox1.Checked)
                    SetColor("wait");
                else
                    SetColor("white");

            }
            //else if (Status == enmStatus.结束)
            //{
            //    checkBox1.Checked = true;
            //    SetColor("white");

            //}
            else if (Status == enmStatus.空闲)
            {
                checkBox1.Checked = true;
                SetColor("white");
            }



        }
        private void SetColor(string sColor)
        {
            try
            {
                m_MeterColor = sColor;
                switch (sColor)
                {
                    case "white":
                        //pictureBox1.Image = System.Drawing.Image.FromFile(Application.StartupPath + @"\white.gif"); 
                        pictureBox1.Image = imageList1.Images[0];
                        break;
                    case "wait"://闪烁等待
                        pictureBox1.Image = System.Drawing.Image.FromFile(Application.StartupPath + @"\wait2.gif");
                        //pictureBox1.Image = imageList1.Images[1];
                        break;
                    case "blue":
                        //pictureBox1.Image = System.Drawing.Image.FromFile(Application.StartupPath + @"\blue.gif");
                        pictureBox1.Image = imageList1.Images[2];
                        break;
                    case "red":
                        //pictureBox1.Image = System.Drawing.Image.FromFile(Application.StartupPath + @"\red.gif");
                        pictureBox1.Image = imageList1.Images[3];
                        break;
                    default:
                        //pictureBox1.Image = System.Drawing.Image.FromFile(Application.StartupPath + @"\white.png");
                        pictureBox1.Image = imageList1.Images[0];
                        break;
                }
            }
            catch
            {
                //MessageBox.Show("图标文件不存在!");
                return;
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.OnEventCheckedChange != null) this.OnEventCheckedChange(m_MeterIndex, checkBox1.Checked);
            checkBox1.ForeColor = checkBox1.Checked ? Color.Black : Color.Gray;
            if (pwFunction.pwConst.GlobalUnit.g_Status == pwInterface.enmStatus.进行中)
            {
                if (checkBox1.Checked)
                {
                    SetColor("wait");
                }
                else if (m_MeterColor == "wait")
                {
                    SetColor("white");
                }

            }
            this.textBox1.Enabled = this.checkBox1.Checked;
            this.textBox1.Text = "";
        }

        private void Display_Resize(object sender, EventArgs e)
        {
            pictureBox1.Left = (this.Width - pictureBox1.Width) / 2;
            checkBox1.Left = (this.Width - checkBox1.Width) / 2;
        }

        /// <summary>
        /// 显示详细数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (this.OnEventBwChange != null) this.OnEventBwChange(m_MeterIndex);
        }



        //public   void   GetTxtFocus
        //{
        //    get 
        //    {
        //        this.ActiveControl = this.textBox1;
        //        this.textBox1.Select((this.textBox1.Text.Trim().Length), 0);
                
        //    }
        //    set { }
        //}

        private string textvalue;
        public string GetTextValue
        {
            get
            {
                return textvalue;
            }
            set
            {
                textvalue = this.textBox1.Text.Trim();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            if (this.OnEvenTxtChange != null) this.OnEvenTxtChange(m_MeterIndex, textBox1.Text.Trim());
            textvalue = this.textBox1.Text.Trim();

        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            this.textBox1.Text = "";
            textBox1.ForeColor = Color.Black;
        }



        internal void GetTxtFocus()
        {
            textBox1.ForeColor = Color.Black;
            this.textBox1.Text = "";
            this.ActiveControl = this.textBox1;
            this.textBox1.Select((this.textBox1.Text.Trim().Length), 0);
        }

        internal void GetEnblan(bool  IsShaomiao)
        {
            this.textBox1.Enabled = IsShaomiao;
            
        }

        internal void setText(string  text)
        {
            this.textBox1.Text = text;

        }

    }
}
