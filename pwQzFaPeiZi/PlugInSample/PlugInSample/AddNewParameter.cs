using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CL4101_QZ_GW
{
    public partial class AddNewParameter : Form
    {
        private string iniFile;        
        private string section;       
        private string key;
        private string value;
        private ComboBox cbx;

        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }
        
        public AddNewParameter(string iniFile, string section, string key,ComboBox cbm)
        {
            InitializeComponent();
            this.iniFile = iniFile;
            this.section = section;
            this.key = key;
            this.cbx = cbm;
            label1.Text += "值";
            //if (key == "方案")
            //{
            //    label1.Text = "请输入新的方案名称";
            //}
            //else
            //{
            //    label1.Text += (key + "值");
            //}
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string value = txtValue.Text.Trim();
            if (value=="")
            {
                MessageBox.Show("请输入新增的值","提示");
                txtValue.Focus();
                return;
            }
            bool flag = true;
            for (int i = 0; i < cbx.Items.Count; i++)
            {
                if (cbx.Items[i].ToString().Contains(txtValue.Text.Trim()))
                {
                    flag = false;       
                    MessageBox.Show("请不要重复添加数据", "提示");
                    break;                      
                }
            }
            bool flagNum = true;
            if (label1.Text.Contains("电流") || label1.Text.Contains("电压"))
            {
                if (!isNumeric(txtValue.Text.Trim()))
                {
                    flagNum = false;
                    MessageBox.Show("输入数据不是有效值", "提示");
                    return;
                }
            }
            if (flag == true && flagNum==true)
            {
                string oldValue = IniFile.IniReadValue(section, key);
                string newValue = oldValue == "" ? value : oldValue + "," + value;
                IniFile.IniWriteValue(section, key, newValue);
                this.DialogResult = DialogResult.Yes;
                this.Value = value;
                this.Close();
            }          
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Value = "";
            this.Close();
        }
        #region 判断是否为整数
        public bool isNumeric(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                int chr = Convert.ToInt32(str[i]);
                if (chr < 48 || chr > 57)
                    return false;
            }
            return true;
        }
        #endregion
    }
}
