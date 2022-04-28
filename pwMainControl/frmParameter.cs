using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace pwMainControl
{
    public partial class frmParameter : Form
    {

        private string[] str_ComDll = new string[0];
        private string[][] str_ComClass = new string[0][];
        private int m_int_BwCount = 1;                     //表位数
        private string m_str_files = "ClMainControlComPort.xml";

        public frmParameter()
        {
            InitializeComponent();
        }

        //public frmParameter(int p_int_BwCount)
        //{
        //    InitializeComponent();
        //    m_int_BwCount = p_int_BwCount;
        //}


        private void frmParameter_Load(object sender, EventArgs e)
        {

            string str_files = Directory.GetCurrentDirectory() + "\\" + m_str_files;
            if (!System.IO.File.Exists(str_files))      //没有文件
                CreateXMLFile(str_files);

            RefreshParameter(str_files);


            string[] str_Dll = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.dll");
            GetDllFiles(str_Dll, ref str_ComDll, ref str_ComClass);

            for (int int_Inc = 0; int_Inc < str_ComDll.Length; int_Inc++)
                this.cmbComLib.Items.Add(str_ComDll[int_Inc]);

            if (this.cmbComLib.Items.Count > 0)
                this.cmbComLib.SelectedIndex = 0;

        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="str_Files">文件</param>
        private void RefreshParameter(string str_Files)
        {
            DataSet dst_XmlConfig = new DataSet();
            dst_XmlConfig.ReadXml(str_Files);
            lvwConfig.Items.Clear();
            DataRow[] daw_cRowArry = dst_XmlConfig.Tables["extension"].Select("UserID='COM'");
            if (daw_cRowArry != null)
            {
                foreach (DataRow daw_cRow in daw_cRowArry)
                {
                    ListViewItem lvi_LstItem = new ListViewItem(daw_cRow["Index"].ToString());
                    lvi_LstItem.SubItems.Add(daw_cRow["Dllfile"].ToString() + "/" + daw_cRow["Class"].ToString());
                    lvi_LstItem.SubItems.Add(daw_cRow["Parameter"].ToString());
                    lvi_LstItem.SubItems.Add(daw_cRow["setting"].ToString());
                    lvwConfig.Items.Add(lvi_LstItem);
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
            string[] str_Name = new string[] { "UserID", "Index", "Interface", "Dllfile", "Class", "Parameter", "setting", "CParameter", "Channel" };
            for (int int_Inc = 0; int_Inc < str_Name.Length; int_Inc++)
            {
                DataColumn dac_Clum = new DataColumn(str_Name[int_Inc]);
                dac_Clum.DataType = System.Type.GetType("System.String");
                dac_Clum.DefaultValue = "";
                dat_XML.Columns.Add(dac_Clum);
            }

            for (int int_Inc = 0; int_Inc < m_int_BwCount; int_Inc++)
            {
                DataRow dar_Row = dat_XML.NewRow();
                dar_Row[str_Name[0]] = "COM";
                dar_Row[str_Name[1]] = Convert.ToString(int_Inc + 1);
                dar_Row[str_Name[2]] = "ISerialport";
                dar_Row[str_Name[3]] = "pwComPorts";
                dar_Row[str_Name[4]] = "CCL20181";
                dar_Row[str_Name[5]] = Convert.ToString(int_Inc + 1) + ",193.168.18.1:10003:20000";
                dar_Row[str_Name[6]] = "9600,e,8,1";
                dar_Row[str_Name[7]] = Convert.ToString(int_Inc + 1) + "/" + m_int_BwCount.ToString();
                dar_Row[str_Name[8]] = "";
                dat_XML.Rows.Add(dar_Row);
            }
            dat_XML.AcceptChanges();
            dat_XML.WriteXml(str_File);
        }


        private void lvwConfig_DoubleClick(object sender, EventArgs e)
        {

            if (lvwConfig.SelectedItems.Count <= 0)
                return;
            pnlEditPara.Visible = true;

            txtChannelNo.Text = lvwConfig.SelectedItems[0].Text;
            string str_Para = lvwConfig.SelectedItems[0].SubItems[1].Text;
            string[] str_ParaArry = str_Para.Split(new char[] { '/' });
            this.cmbComLib.Text = str_ParaArry[0];
            this.cmbComClass.Text = str_ParaArry[1];

            str_Para = lvwConfig.SelectedItems[0].SubItems[2].Text;
            str_ParaArry = str_Para.Split(new char[] { ',' });
            this.cmbComNo.Text = str_ParaArry[0];
            str_Para = str_ParaArry[1];
            str_ParaArry = str_Para.Split(new char[] { ':' });
            this.txtIpAddr.Text = str_ParaArry[0];
            this.txtRomtPort.Text = str_ParaArry[1];
            this.txtBindPort.Text = str_ParaArry[2];

            this.cmb_Setting.Text = lvwConfig.SelectedItems[0].SubItems[3].Text;

        }

        private void btnCancelEdit_Click(object sender, EventArgs e)
        {
            pnlEditPara.Visible = false;

        }

        private void lvwConfig_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnOkEdit_Click(object sender, EventArgs e)
        {
            string[] str_Para = this.txtIpAddr.Text.Split(new char[] { '.' });

            int int_Index = int.Parse(this.txtChannelNo.Text) - 1;
            lvwConfig.Items[int_Index].SubItems[1].Text = this.cmbComLib.Text + "/" + this.cmbComClass.Text;
            lvwConfig.Items[int_Index].SubItems[2].Text = this.cmbComNo.Text + "," + this.txtIpAddr.Text + ":" + this.txtRomtPort.Text + ":" + this.txtBindPort.Text;
            for (int int_Inc = 0; int_Inc < m_int_BwCount; int_Inc++)
            {
                lvwConfig.Items[int_Inc].SubItems[3].Text = this.cmb_Setting.Text;
            }
            pnlEditPara.Visible = false;
        }


        private void GetDllFiles(string[] str_Files, ref string[] str_Dll, ref string[][] str_Class)
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
                                if (tpe_ITypes[int_Ina].Name == "ISerialport")     //是继承于str_Interface接口,才是要找的
                                {
                                    bln_Have = true;
                                    Array.Resize(ref str_TmpClass, str_TmpClass.Length + 1);
                                    str_TmpClass[str_TmpClass.Length - 1] = tpe_Types[int_Inb].Name;
                                    break;
                                }
                            }
                            //if (bln_Have)
                            //    break;
                        }
                    }
                    catch
                    {
                    }
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

        private void cmbComLib_SelectedValueChanged(object sender, EventArgs e)
        {
            for (int int_Inc = 0; int_Inc < this.str_ComClass[cmbComLib.SelectedIndex].Length; int_Inc++)
            {
                this.cmbComClass.Items.Add(this.str_ComClass[cmbComLib.SelectedIndex][int_Inc]);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string[] str_files = Directory.GetFiles(Directory.GetCurrentDirectory(), m_str_files);
            if (str_files != null)
            {
                if (str_files.Length >= 1)
                {
                    DataSet dst_XmlConfig = new DataSet();
                    dst_XmlConfig.ReadXml(str_files[0]);
                    DataRow[] daw_cRowArry = dst_XmlConfig.Tables["extension"].Select("UserID='COM'");
                    if (daw_cRowArry != null)
                    {
                        for (int int_Inc = 0; int_Inc < lvwConfig.Items.Count; int_Inc++)
                        {
                            daw_cRowArry[int_Inc].BeginEdit();
                            string str_Para = lvwConfig.Items[int_Inc].SubItems[1].Text;
                            string[] str_ParaArry = str_Para.Split(new char[] { '/' });
                            daw_cRowArry[int_Inc]["Dllfile"] = str_ParaArry[0];
                            daw_cRowArry[int_Inc]["Class"] = str_ParaArry[1];
                            daw_cRowArry[int_Inc]["Parameter"] = lvwConfig.Items[int_Inc].SubItems[2].Text;
                            daw_cRowArry[int_Inc]["setting"] = lvwConfig.Items[int_Inc].SubItems[3].Text;
                            daw_cRowArry[int_Inc].EndEdit();
                        }
                        dst_XmlConfig.AcceptChanges();
                        dst_XmlConfig.WriteXml(str_files[0]);
                        //RefreshParameter(str_files[0]);
                        this.Close();
                    }
                }
            }
        }




    }
}