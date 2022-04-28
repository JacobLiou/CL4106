
using System.IO;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using pwCollapseDataGridView;
using pwFunction.pwConst;
using pwInterface;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Linq;
namespace CL4100
{
    public partial class pw_TTTxm : Form
    {      
        public string DatabaseServer;
        public string DatabaseName;
        public string DatabasePassword;
        public string DatabaseUser;
        public pw_TTTxm()
        {
            InitializeComponent();
            txtMOName.Text = "";
            txtMOName.Focus();

        }
        private void txtMOName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)      //如果输入的是回车键
            {
                if (txtMOName.Text == "") return;

                GlobalUnit.g_DeskNo = txtMOName.Text;

                this.Close();
            }
        }
    }
}




