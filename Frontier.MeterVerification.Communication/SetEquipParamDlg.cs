using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;



namespace Frontier.MeterVerification.Communication
{
 

    public partial class SetEquipParamDlg : Form
    {
        #region 字段
        private INIHelper iniRw; //读写对象
        #endregion
        public SetEquipParamDlg()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 初始化端口信息
        /// </summary>
        private void InitEquipParm()
        {
           StringBuilder equip = new StringBuilder();
            iniRw = new INIHelper("DeviceConfig.ini");
             string[] devices = iniRw.IniReadValue("Main", "Devices").Split(',');
             dgw_Equip.Rows.Clear();
             dgt_Meter.Rows.Clear();
             for (int i = 0; i < devices.Length; i++)
             {
                 //实例化设备
                 string devicePortSetting = iniRw.IniReadValue("Main", string.Format("{0}_{1}", devices[i], "PortSetting"));
                 
                 if (devices[i] == "CLDlt645_2007")
                 {
                     string[] devicePorts = devicePortSetting.Split('|');
                     for (int j = 0; j < devicePorts.Length; j++)
                     {
                         string[] deviePortSettings = devicePorts[j].Split(',');
                         if (deviePortSettings[0].ToUpper() == "SerialPort".ToUpper())
                         {
                             DataGridViewRow GridRow = dgt_Meter.Rows[dgt_Meter.Rows.Add()];
                             GridRow.Cells["_表位号"].Value = j;
                             GridRow.Cells["_表位号端口参数"].Value = devicePorts[j];

                         }
                     }
                 }
                 else
                 {   //获取参数

                     DataGridViewRow GridRow = dgw_Equip.Rows[dgw_Equip.Rows.Add()];
                     GridRow.Cells["_仪器仪表"].Value = devices[i];
                     GridRow.Cells["_仪器仪表参数"].Value = devicePortSetting;
                 }



             }

             //标准表
            comBox_Std.Text =  iniRw.IniReadValue("EquipSet", comBox_Std.Name);
             //功率源
            comBox_Power.Text = iniRw.IniReadValue("EquipSet", comBox_Power.Name);
             //时基源
            comBox_TimeBs.Text = iniRw.IniReadValue("EquipSet", comBox_TimeBs.Name);
             //误差板
            comBox_Error.Text = iniRw.IniReadValue("EquipSet", comBox_Error.Name);
            //R
            cbx_Rs485.Text = iniRw.IniReadValue("EquipSet", cbx_Rs485.Name);
            string strIsPush = iniRw.IniReadValue("EquipSet", IsPushBox.Name);
            if (String.IsNullOrEmpty(strIsPush))
            { strIsPush = "true"; }
            IsPushBox.Checked = Convert.ToBoolean(strIsPush);
            updataEquipSet();

        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Save_Click(object sender, EventArgs e)
        {
            iniRw = new INIHelper("DeviceConfig.ini");
            string[] devices = iniRw.IniReadValue("Main", "Devices").Split(',');

            foreach (DataGridViewRow dt in dgw_Equip.Rows)
            {
               string strKey = dt.Cells["_仪器仪表"].Value.ToString();
               strKey = string.Format("{0}_{1}", strKey, "PortSetting");
               string strValue = dt.Cells["_仪器仪表参数"].Value.ToString();
               iniRw.IniWriteValue("Main", strKey, strValue);
            }

            string sKey = "CLDlt645_2007_PortSetting";
            //
            string sValue = "Socket,193.168.18.1,20000,10003";
            foreach (DataGridViewRow dtw in dgt_Meter.Rows)
            {
                sValue  += "|"+ dtw.Cells["_表位号端口参数"].Value.ToString();
                //dtw.Cells["_表位号"].Value.ToString();
            }
            iniRw.IniWriteValue("Main", sKey, sValue);

            //标准表
            iniRw.IniWriteValue("EquipSet", comBox_Std.Name, comBox_Std.Text);
            //功率源
            iniRw.IniWriteValue("EquipSet", comBox_Power.Name, comBox_Power.Text);
            //时基源
            iniRw.IniWriteValue("EquipSet", comBox_TimeBs.Name, comBox_TimeBs.Text);
            //误差板
            iniRw.IniWriteValue("EquipSet", comBox_Error.Name, comBox_Error.Text);
            //
            iniRw.IniWriteValue("EquipSet", cbx_Rs485.Name, cbx_Rs485.Text);

            iniRw.IniWriteValue("EquipSet", IsPushBox.Name, (IsPushBox.Checked).ToString());
            

            updataEquipSet();
        }

        //载入INI文件
        private void SetEquipParamDlg_Load(object sender, EventArgs e)
        {
            InitEquipParm();
        }


        private void updataEquipSet()
        {
            string a = string.Empty;
            string b = string.Empty;
            string c = string.Empty;
            string d = string.Empty;
            //标准表
            if (comBox_Std.Text == "CL3115")
            {
                a = "0";
            }
            else if (comBox_Power.Text == "CL311V2")
            {
                a = "2";
            }
            else
            {
                a = "2";
            }
            //功率源
            if (comBox_Power.Text == "CL309")
            {
                b = "0";
            }
            else if (comBox_Power.Text == "CL303")
            {
                b = "1";
            }
            else
            {
                b = "1";
            }
            //时基源
            c = "0";
            //误差板
            if (comBox_Error.Text == "188L")
            {
                d = "0";
            }
            else
            {
                d = "1";
            }
            Frontier.MeterVerification.KLDevice.GlobalUnit.DriverTypes = a + b + c + d;
            if (IsPushBox.Checked)
            {
                Frontier.MeterVerification.KLDevice.GlobalUnit.isPushBox = 0;
            }
            else
            { Frontier.MeterVerification.KLDevice.GlobalUnit.isPushBox = 1; }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //string strPath = System.Windows.Forms.Application.StartupPath + "\\DeviceConfig.ini";
            //string section = "Main";
            //string key = "CLPower_PortSetting";
            //StringBuilder temp = new StringBuilder(255);
            //int i = GetPrivateProfileString(section, key, "无法读取对应数值！", temp, 255, strPath);
            ////显示读取的数值 
            ////textBox4.Text = temp.ToString(); 
            //string strTemp = temp.ToString();
            //MessageBox.Show(strTemp);



        }
        private void button2_Click(object sender, EventArgs e)
        {
            //string strPath = System.Windows.Forms.Application.StartupPath + "\\DeviceConfig.ini";
            //string section = "Main";
            //string key = "test311v2";
            //StringBuilder temp = new StringBuilder(255);
            // long ltest =  WritePrivateProfileString(section, key, "ssniszai243092-p2&&**343@#$$%%^^&&*(())$#@@", strPath); 
            ////显示读取的数值 
            ////textBox4.Text = temp.ToString(); 
            // string strTemp = ltest.ToString();
            //MessageBox.Show(strTemp);
        }
    }




}
