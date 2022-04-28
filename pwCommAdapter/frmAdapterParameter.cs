using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace pwCommAdapter
{
    public partial class frmAdapterParameter : Form
    {
        string[] str_ComDll = new string[0];
        string[][] str_ComClass = new string[0][];

        string[] str_PDll = new string[0];
        string[][] str_PClass = new string[0][];


        int m_int_ShowType = 1;
        string m_str_files = Directory.GetCurrentDirectory() + "\\ClPlugins.xml";
        
        public frmAdapterParameter()
        {
            InitializeComponent();
        }
        public frmAdapterParameter(string str_files)
        {
            InitializeComponent();
            switch (str_files)
            {
                case "ClPlugins.xml"://V7
                    //this.Text += "（检定台）";
                    break;

                case "ClPlugins_V7A.xml"://V7A
                    this.Text += "（A组 台架）";
                    break;

                case "ClPlugins_V7B.xml"://V7B
                    this.Text += "（B组 台架）";
                    break;
                default :
                    m_str_files = Directory.GetCurrentDirectory() + "\\ClPlugins.xml";
                    break;

            }
            m_str_files = Directory.GetCurrentDirectory() + "\\" + str_files;

        }

        public frmAdapterParameter(int int_ShowType)
        {
            InitializeComponent();
            this.m_int_ShowType = int_ShowType;
        }

        private void frmAdapterParameter_Load(object sender, EventArgs e)
        {
            if (!System.IO.File.Exists(m_str_files))
                CreateXMLFile(m_str_files);
            string[] str_Dll = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.dll");
            GetDllFiles(str_Dll, "ISerialport", ref str_ComDll, ref str_ComClass);
            if (m_int_ShowType == 1)
            {
                for (int int_Inc = 0; int_Inc < str_ComDll.Length; int_Inc++)
                    this.cmb_3000_ComLib.Items.Add(str_ComDll[int_Inc]);
                if (this.cmb_3000_ComLib.Items.Count > 0)
                    this.cmb_3000_ComLib.SelectedIndex = 0;
            }
            RefreshParameter(m_str_files, m_int_ShowType);
        }


        private void GetDllFiles(string[] str_Files,string str_InterfaceName, ref string[] str_Dll, ref string[][] str_Class)
        {
            string[] str_DllName = new string[0];
            string[][] str_ClassName = new string[0][];
            Assembly aby_DllFile = null;
            for (int int_Inc = 0; int_Inc < str_Files.Length; int_Inc++)
            {
                try
                {
                    aby_DllFile = Assembly.LoadFile(str_Files[int_Inc]);      //动态加载文件
                }
                catch
                {
                }
                if (aby_DllFile != null)                            //
                {
                    bool bln_Have = false;
                    string[] str_TmpClass = new string[0];
                    try
                    {
                        Type[] tpe_Types = aby_DllFile.GetTypes();        //取出当前.DLL所有类
                        for (int int_Inb = 0; int_Inb < tpe_Types.Length; int_Inb++)     //遍历当前.DLL文件中的所有类是存在为ClassName的名称
                        {
                            Type[] tpe_ITypes = tpe_Types[int_Inb].GetInterfaces();    //取出当前类的所有继承的接口
                            for (int int_Ina = 0; int_Ina < tpe_ITypes.Length; int_Ina++)        //判断这个类是否继承str_Interface这个接口
                            {
                                if (tpe_ITypes[int_Ina].Name == str_InterfaceName)     //是继承于str_Interface接口,才是要找的
                                {
                                    bln_Have = true;
                                    Array.Resize(ref str_TmpClass, str_TmpClass.Length + 1);
                                    str_TmpClass[str_TmpClass.Length - 1] = tpe_Types[int_Inb].Name;
                                    break;
                                }
                            }
                        }
                    }
                    catch { }
                    if (bln_Have)
                    {
                        Array.Resize(ref str_DllName, str_DllName.Length + 1);
                        str_DllName[str_DllName.Length - 1] = aby_DllFile.GetName().Name;
                        Array.Resize(ref str_ClassName, str_ClassName.Length + 1);
                        str_ClassName[str_ClassName.Length - 1] = str_TmpClass;
                    }
                }
            }

            str_Dll = str_DllName;
            str_Class = str_ClassName;
        }

        private void RefreshParameter(string str_Files, int int_ShowType)
        {

            if (!System.IO.File.Exists(str_Files))      //找不到文件
                return;
            
            DataSet dst_XmlConfig = new DataSet();
            dst_XmlConfig.ReadXml(str_Files);
            this.lvw_Cl3000DPara.Items.Clear();
            if (m_int_ShowType == 1)
            {
                DataRow[] daw_cRowArry = dst_XmlConfig.Tables["extension"].Select("UserID='ClCommV7'");
                if (daw_cRowArry != null)
                {
                    foreach (DataRow daw_cRow in daw_cRowArry)
                    {
                        string str_Interface = daw_cRow["Interface"].ToString();
                        string str_Name = "";
                        if (str_Interface == "IPower")
                            str_Name = "程控源";
                        else if (str_Interface == "IStdMeter")
                            str_Name = "标准表";
                        else if (str_Interface == "IErrorCalculate")
                            str_Name = "误差计算板";
                        else if (str_Interface == "IStdTime")
                            str_Name = "时钟基准源";

                        ListViewItem lvi_LstItem = new ListViewItem(str_Name);
                        lvi_LstItem.SubItems.Add(daw_cRow["Dllfile"].ToString() + "/" + daw_cRow["Class"].ToString());
                        lvi_LstItem.SubItems.Add(daw_cRow["ComDllfile"].ToString() + "/" + daw_cRow["ComClass"].ToString());
                        lvi_LstItem.SubItems.Add(daw_cRow["Parameter"].ToString());
                        lvi_LstItem.SubItems.Add(daw_cRow["setting"].ToString());
                        lvi_LstItem.SubItems.Add(daw_cRow["CParameter"].ToString());
                        lvi_LstItem.SubItems.Add(str_Interface);
                        lvi_LstItem.Tag = str_Interface;
                        this.lvw_Cl3000DPara.Items.Add(lvi_LstItem);
                    }
                }
            }
        }

        /// <summary>
        /// 创建XML文件
        /// </summary>
        /// <param name="str_File"></param>
        private void CreateXMLFile(string str_File)
        {
            DataTable dat_XML = new DataTable("extension");
            string[] str_Name = new string[] { "UserID", "Interface", "Dllfile", "Class", "ComDllfile",
                                               "ComClass", "Parameter","setting" ,"CParameter","Channel"};
            for (int int_Inc = 0; int_Inc < 10; int_Inc++)
            {
                DataColumn dac_Clum = new DataColumn(str_Name[int_Inc]);
                dac_Clum.DataType = System.Type.GetType("System.String");
                dac_Clum.DefaultValue = "";
                dat_XML.Columns.Add(dac_Clum);
            }

            string[][] str_Value = new string[][] { 
            new string []{"ClCommV7","IPower","pwCommAdapter","CCL109","pwComPorts","CCL20181","29,193.168.18.1:10003:20000","9600,n,8,1","",""},
            new string []{"ClCommV7","IStdMeter","pwCommAdapter","CCL1115","pwComPorts","CCL20181","30,193.168.18.1:10003:20000","9600,n,8,1","",""},
            new string []{"ClCommV7","IErrorCalculate","pwCommAdapter","CCl188E","pwComPorts","CCL20181","31,193.168.18.1:10003:20000","19200,n,8,1","1/1",""},
            new string []{"ClCommV7","IStdTime","pwCommAdapter","CCL191","pwComPorts","CCL20181","32,193.168.18.1:10003:20000","2400,n,8,1","",""}
            };
            if (str_File== (Directory.GetCurrentDirectory() + "\\" + "ClPlugins_V7A.xml" ))//A组
            {
                    str_Value = new string[][] { 
                        new string []{"ClCommV7","IPower","pwCommAdapter","CCL109","pwComPorts","CCL20181","19,193.168.18.1:10003:20000","38400,n,8,1","",""},
                        new string []{"ClCommV7","IStdMeter","pwCommAdapter","CCL1115","pwComPorts","CCL20181","17,193.168.18.1:10003:20000","38400,n,8,1","",""},
                        new string []{"ClCommV7","IErrorCalculate","pwCommAdapter","CCL188L","pwComPorts","CCL20181","7,193.168.18.1:10003:20000","38400,n,8,1","1/3",""},
                        new string []{"ClCommV7","IErrorCalculate","pwCommAdapter","CCL188L","pwComPorts","CCL20181","8,193.168.18.1:10003:20000","38400,n,8,1","2/3",""},
                        new string []{"ClCommV7","IErrorCalculate","pwCommAdapter","CCL188L","pwComPorts","CCL20181","15,193.168.18.1:10003:20000","38400,n,8,1","3/3",""},
                        new string []{"ClCommV7","IStdTime","pwCommAdapter","CCL191","pwComPorts","CCL20181","21,193.168.18.1:10003:20000","2400,n,8,1","",""},
                        };
            }
            else if (str_File ==( Directory.GetCurrentDirectory() + "\\" + "ClPlugins_V7B.xml" ))//B组
            {
                str_Value = new string[][] { 
                        new string []{"ClCommV7","IPower","pwCommAdapter","CCL109","pwComPorts","CCL20181","20,193.168.18.1:10003:20000","38400,n,8,1","",""},
                        new string []{"ClCommV7","IStdMeter","pwCommAdapter","CCL1115","pwComPorts","CCL20181","18,193.168.18.1:10003:20000","38400,n,8,1","",""},
                        new string []{"ClCommV7","IErrorCalculate","pwCommAdapter","CCL188L","pwComPorts","CCL20181","16,193.168.18.1:10003:20000","38400,n,8,1","1/3",""},
                        new string []{"ClCommV7","IErrorCalculate","pwCommAdapter","CCL188L","pwComPorts","CCL20181","31,193.168.18.1:10003:20000","38400,n,8,1","2/3",""},
                        new string []{"ClCommV7","IErrorCalculate","pwCommAdapter","CCL188L","pwComPorts","CCL20181","32,193.168.18.1:10003:20000","38400,n,8,1","3/3",""},
                        new string []{"ClCommV7","IStdTime","pwCommAdapter","CCL191","pwComPorts","CCL20181","21,193.168.18.1:10003:20000","2400,n,8,1","",""},
                        };
            }
            for (int int_Inc = 0; int_Inc < str_Value.Length ; int_Inc++)
            {
                DataRow dar_Row = dat_XML.NewRow();
                for (int int_Inb = 0; int_Inb < str_Value[int_Inc ].Length ; int_Inb++)
                    dar_Row[str_Name[int_Inb]] = str_Value[int_Inc][int_Inb];
                dat_XML.Rows.Add(dar_Row);
            }
            
            dat_XML.AcceptChanges();
            dat_XML.WriteXml(str_File);
        }


        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_str_files == "")
                    return;
                DataTable dat_XML = new DataTable("extension");
                string[] str_Name = new string[] { "UserID", "Interface", "Dllfile", "Class", "ComDllfile",
                                               "ComClass", "Parameter","setting" ,"CParameter","Channel"};
                for (int int_Inc = 0; int_Inc < 10; int_Inc++)
                {
                    DataColumn dac_Clum = new DataColumn(str_Name[int_Inc]);
                    dac_Clum.DataType = System.Type.GetType("System.String");
                    dac_Clum.DefaultValue = "";
                    dat_XML.Columns.Add(dac_Clum);
                }
                if (m_int_ShowType == 1)
                {
                    for (int int_Inc = 0; int_Inc < this.lvw_Cl3000DPara.Items.Count; int_Inc++)
                    {
                        DataRow dar_Row = dat_XML.NewRow();
                        dar_Row[str_Name[0]] = "ClCommV7";
                        dar_Row[str_Name[1]] = this.lvw_Cl3000DPara.Items[int_Inc].Tag.ToString();
                        string str_Tmp = this.lvw_Cl3000DPara.Items[int_Inc].SubItems[1].Text;
                        string[] str_Para = str_Tmp.Split(new char[] { '/' });
                        dar_Row[str_Name[2]] = str_Para[0];
                        dar_Row[str_Name[3]] = str_Para[1];
                        str_Tmp = this.lvw_Cl3000DPara.Items[int_Inc].SubItems[2].Text;
                        str_Para = str_Tmp.Split(new char[] { '/' });
                        dar_Row[str_Name[4]] = str_Para[0];
                        dar_Row[str_Name[5]] = str_Para[1];
                        dar_Row[str_Name[6]] = this.lvw_Cl3000DPara.Items[int_Inc].SubItems[3].Text;
                        dar_Row[str_Name[7]] = this.lvw_Cl3000DPara.Items[int_Inc].SubItems[4].Text;
                        dar_Row[str_Name[8]] = this.lvw_Cl3000DPara.Items[int_Inc].SubItems[5].Text;
                        dar_Row[str_Name[9]] = "";
                        dat_XML.Rows.Add(dar_Row);
                    }
                }

                dat_XML.AcceptChanges();
                dat_XML.WriteXml(m_str_files);
                RefreshParameter(m_str_files, m_int_ShowType);

                MessageBox.Show("保存成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存失败!"+ ex.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btn_3000_OkEdit_Click(object sender, EventArgs e)
        {
            int int_Index = int.Parse(this.pnl_3000_EditPara.Tag.ToString());
            if (int_Index < this.lvw_Cl3000DPara.Items.Count)
            {
                this.lvw_Cl3000DPara.Items[int_Index].SubItems[1].Text = cmb_3000_PLib.Text + "/" + this.cmb_3000_PClass.Text;
                this.lvw_Cl3000DPara.Items[int_Index].SubItems[2].Text = this.cmb_3000_ComLib.Text + "/" + this.cmb_3000_ComClass.Text;
                this.lvw_Cl3000DPara.Items[int_Index].SubItems[3].Text = this.cmb_3000_ComNo.Text + "," + this.txt_3000_IP.Text + ":" + this.txt_3000_RPort.Text + ":" + this.txt_3000_BPort.Text;
                this.lvw_Cl3000DPara.Items[int_Index].SubItems[4].Text = this.cmb_3000_Setting.Text;
                if (this.lvw_Cl3000DPara.Items[int_Index].Tag.ToString() == "IErrorCalculate")
                    this.lvw_Cl3000DPara.Items[int_Index].SubItems[5].Text = txt_3000_Channel.Text;
                else
                    this.lvw_Cl3000DPara.Items[int_Index].SubItems[5].Text = "";
                pnl_3000_EditPara.Visible = false;
            }
        }

        private void lvw_Cl3000DPara_DoubleClick(object sender, EventArgs e)
        {
            if (this.lvw_Cl3000DPara.SelectedItems.Count <= 0)
                return;

            lab_Channel.Visible = false;
            txt_3000_Channel.Visible = false;
            pnl_3000_EditPara.Visible = true;
            pnl_3000_EditPara.Tag = this.lvw_Cl3000DPara.SelectedItems[0].Index;
            string[] str_Dll = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.dll");
            
            GetDllFiles(str_Dll, this.lvw_Cl3000DPara.SelectedItems[0].Tag.ToString(), ref str_PDll, ref str_PClass);

            this.cmb_3000_PLib.Items.Clear();
            for (int int_Inc = 0; int_Inc < str_PDll.Length; int_Inc++)  
                this.cmb_3000_PLib.Items.Add(str_PDll[int_Inc]);
            if (this.cmb_3000_PLib.Items.Count > 0)
                this.cmb_3000_PLib.SelectedIndex = 0;

          


            string str_Para = this.lvw_Cl3000DPara.SelectedItems[0].SubItems[1].Text;
            string[] str_ParaArry = str_Para.Split(new char[] { '/' });
            this.cmb_3000_PLib.Text = str_ParaArry[0]; 
            this.cmb_3000_PClass.Text = str_ParaArry[1];
            
            str_Para = this.lvw_Cl3000DPara.SelectedItems[0].SubItems[2].Text;
            str_ParaArry = str_Para.Split(new char[] { '/' });
            this.cmb_3000_ComLib .Text = str_ParaArry[0];
            this.cmb_3000_ComClass.Text = str_ParaArry[1];

            str_Para = lvw_Cl3000DPara.SelectedItems[0].SubItems[3].Text;
            str_ParaArry = str_Para.Split(new char[] { ',' });
            this.cmb_3000_ComNo .Text = str_ParaArry[0];
            str_Para = str_ParaArry[1];
            str_ParaArry = str_Para.Split(new char[] { ':' });
            this.txt_3000_IP.Text = str_ParaArry[0];
            this.txt_3000_RPort.Text = str_ParaArry[1];
            this.txt_3000_BPort.Text = str_ParaArry[2];
            this.cmb_3000_Setting.Text = lvw_Cl3000DPara.SelectedItems[0].SubItems[4].Text;
            if (this.lvw_Cl3000DPara.SelectedItems[0].Tag.ToString() == "IErrorCalculate")
            {
                lab_Channel.Visible = true;
                txt_3000_Channel.Visible = true;
                txt_3000_Channel.Text = lvw_Cl3000DPara.SelectedItems[0].SubItems[5].Text;
            }

        }

        private void cmb_3000_PLib_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.cmb_3000_PClass.Items.Clear();
            for (int int_Inc = 0; int_Inc < this.str_PClass [this.cmb_3000_PLib.SelectedIndex].Length; int_Inc++)
            {
                this.cmb_3000_PClass .Items.Add(this.str_PClass[cmb_3000_PLib.SelectedIndex][int_Inc]);
            }
        }

        private void cmb_3000_ComLib_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.cmb_3000_ComClass.Items.Clear();
            for (int int_Inc = 0; int_Inc < this.str_ComClass[this.cmb_3000_ComLib.SelectedIndex].Length; int_Inc++)
            {
                this.cmb_3000_ComClass.Items.Add(this.str_ComClass[this.cmb_3000_ComLib.SelectedIndex][int_Inc]);
            }
        }

        private void btn_3000_Cancel_Click(object sender, EventArgs e)
        {
            if (this.lvw_Cl3000DPara.SelectedItems.Count > 0)
            {
                for (int int_Inc = 0; int_Inc < this.lvw_Cl3000DPara.SelectedItems.Count; int_Inc++)
                {
                    if (this.lvw_Cl3000DPara.SelectedItems[int_Inc].Tag.ToString() == "IErrorCalculate")
                    {
                        if (MessageBox.Show("真的要取消吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            this.lvw_Cl3000DPara.Items.Remove(this.lvw_Cl3000DPara.SelectedItems[int_Inc]);
                        return;
                    }
                }
            }
        }

        private void btn_Add188Channel_Click(object sender, EventArgs e)
        {
            ListViewItem lvi_LstItem = new ListViewItem("误差计算板");
            lvi_LstItem.SubItems.Add("pwCommAdapter/CCL188L");
            lvi_LstItem.SubItems.Add("pwComPorts/CCL20181");
            lvi_LstItem.SubItems.Add("7,193.168.18.1:10003:20000");
            lvi_LstItem.SubItems.Add("38400,n,8,1");
            lvi_LstItem.SubItems.Add("1/3");
            lvi_LstItem.SubItems.Add("IErrorCalculate");
            lvi_LstItem.Tag = "IErrorCalculate";
            this.lvw_Cl3000DPara.Items.Add(lvi_LstItem);
            this.lvw_Cl3000DPara.SelectedItems.Clear();
            this.lvw_Cl3000DPara.Items[this.lvw_Cl3000DPara.Items.Count - 1].Selected = true;
            lvw_Cl3000DPara_DoubleClick(sender, e);
            btn_Add188Channel.Enabled = false;

        }

    }
}