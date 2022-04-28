using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using pwFunction;
using System.Xml;
using System.Net;
using pwClassLibrary.DataBase;

namespace CL4100.Plan3Phase
{
    public partial class Plan_SelfCheckUserControl : UserControl
    {
        private string XmlFilePath = Application.StartupPath + pwFunction.pwConst.Variable.CONST_WORKPLANPATH;
        private string _NodeName = "Plan_SelfCheck";
        private string _NodeDescription = "整机自检";
        private bool m_Checked = false;//是否要检
        private string m_Para = "";//项目参数
        private CheckBox[] m_Checkbox = new CheckBox[32];

        public Plan_SelfCheckUserControl()
        {
            InitializeComponent();
            Init();
            pw_Main.OnEventSchemaDataSave += new pwInterface.DelegateEventSchemaDataSave(EventSchemaDataSave);
            pw_Main.OnEventSchemaDataLoad += new pwInterface.DelegateEventSchemaDataLoad(EventSchemaDataLoad);

        }

        private void EventSchemaDataSave(int intItem, bool Checked, string _XmlFilePath)//
        {
            if (intItem != 4) return;
            XmlFilePath = _XmlFilePath;
            m_Checked = Checked;
            Save();

        }
        private void EventSchemaDataLoad(int intItem, string _XmlFilePath)//
        {
            if (intItem != 4) return;
            XmlFilePath = _XmlFilePath;
            Load();
        }

        private void Init()
        {
            m_Checkbox[0] = chb1;
            m_Checkbox[1] = chb2;
            m_Checkbox[2] = chb3;
            m_Checkbox[3] = chb4;
            m_Checkbox[4] = chb5;
            m_Checkbox[5] = chb6;
            m_Checkbox[6] = chb7;
            m_Checkbox[7] = chb8;
            m_Checkbox[8] = chb9;
            m_Checkbox[9] = chb10;
            m_Checkbox[10] = chb11;
            m_Checkbox[11] = chb12;
            m_Checkbox[12] = chb13;
            m_Checkbox[13] = chb14;
            m_Checkbox[14] = chb15;
            m_Checkbox[15] = chb16;
            m_Checkbox[16] = chb17;
            m_Checkbox[17] = chb18;
            m_Checkbox[18] = chb19;
            m_Checkbox[19] = chb20;
            m_Checkbox[20] = chb21;
            m_Checkbox[21] = chb22;
            m_Checkbox[22] = chb23;
            m_Checkbox[23] = chb24;
            m_Checkbox[24] = chb25;
            m_Checkbox[25] = chb26;
            m_Checkbox[26] = chb27;
            m_Checkbox[27] = chb28;
            m_Checkbox[28] = chb29;
            m_Checkbox[29] = chb30;
            m_Checkbox[30] = chb31;
            m_Checkbox[31] = chb32;

        }
        /// <summary>
        /// 读取XML文件初始化配置信息
        /// </summary>
        public void Load()
        {
            string _ErrorString = "";
            XmlNode _XmlNodeChildNodes;
            XmlNode _XmlNode = clsXmlControl.LoadXml(XmlFilePath, out _ErrorString);
            if (_ErrorString != "")
            {
                _XmlNode = clsXmlControl.CreateXmlNode("WorkPlan");

                #region 加载系统配置默认值
                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeName, "Description", _NodeDescription);

                this.CreateDefaultData(ref _XmlNodeChildNodes);

                _XmlNode.AppendChild(_XmlNodeChildNodes);

                #endregion

                clsXmlControl.SaveXml(_XmlNode, XmlFilePath);
                this.Load();
                return;
            }

            _XmlNodeChildNodes = clsXmlControl.FindSencetion(_XmlNode, _NodeName);
            if (_XmlNodeChildNodes == null)
            {

                #region 加载系统配置默认值

                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeName, "Description", _NodeDescription);

                this.CreateDefaultData(ref _XmlNodeChildNodes);

                _XmlNode.AppendChild(_XmlNodeChildNodes);
                #endregion

                clsXmlControl.SaveXml(_XmlNode, XmlFilePath);
                this.Load();
                return;
            }


            m_Checked = Convert.ToBoolean(_XmlNodeChildNodes.ChildNodes[0].Attributes["Value"].Value);
            m_Para = _XmlNodeChildNodes.ChildNodes[2].Attributes["Value"].Value;
            string[] _Pata = m_Para.Split('|');

            cmbM1Bit0.SelectedIndex = (Convert.ToInt32(_Pata[0]) & 1) == 0 ? 0 : 1;
            cmbM1Bit1.SelectedIndex = (Convert.ToInt32(_Pata[0]) & 2) == 0 ? 0 : 1;

            cmbM2.SelectedIndex = Convert.ToInt32(_Pata[1]);

            txtfourBit.Text = _Pata[2];
            for (int i = 0; i < m_Checkbox.Length; i++)
            {
                if (Convert.ToInt32(Convert.ToInt64(_Pata[2],16) & Convert.ToInt64(Math.Pow(2, i))) == 0)
                {
                    m_Checkbox[i].Checked = false;
                }
                else
                {
                    m_Checkbox[i].Checked = true ;
                }
            }

        }


        /// <summary>
        /// 创建工单默认配置文件
        /// </summary>
        /// <param name="xml"></param>
        private void CreateDefaultData(ref XmlNode xml)
        {
            #region -----------整机自检-----------
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "IsCheck"
                    , "Name", "是否要检"
                    , "Value", "true"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "CustomerName"
                    , "Name", "项目名称"
                    , "Value", "整机自检"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "ProductsName"
                    , "Name", "项目参数"
                    , "Value", "3|1|00000000"));//M1|M2---M1 : Bit0;0=单板测试  1=整机测试;  Bit1 0=三相四 1=三相三   ； M2 0：外控磁保持  1：内控磁保持   2：外控电保持 ；测试需判断项4个字节

            #endregion

        }

        /// <summary>
        /// 保存参数到当前配置文件
        /// </summary>
        public void Save()
        {
            string _ErrorString = "";
            XmlNode _XmlNodeChildNodes;
            XmlNode _XmlNode = clsXmlControl.LoadXml(XmlFilePath, out _ErrorString);
            if (_ErrorString != "")//
            {
                _XmlNode = clsXmlControl.CreateXmlNode("WorkPlan");

                #region 没找到文件则新建，加载配置默认值

                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeName, "Description", _NodeDescription);

                this.SaveCurrentData(ref _XmlNodeChildNodes);

                _XmlNode.AppendChild(_XmlNodeChildNodes);

                #endregion

                clsXmlControl.SaveXml(_XmlNode, XmlFilePath);
                //this.Load();
                return;
            }
            _XmlNodeChildNodes = clsXmlControl.FindSencetion(_XmlNode, _NodeName);
            if (_XmlNodeChildNodes == null)
            {

                #region 没找到目录则新建XmlNode

                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeName, "Description", _NodeDescription);

                this.SaveCurrentData(ref _XmlNodeChildNodes);

                _XmlNode.AppendChild(_XmlNodeChildNodes);
                #endregion

                clsXmlControl.SaveXml(_XmlNode, XmlFilePath);
                //this.Load();
                return;
            }
            else
            {
                #region 找到目录则修改XmlNode

                _XmlNode.RemoveChild(_XmlNodeChildNodes);
                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeName, "Description", _NodeDescription);

                this.SaveCurrentData(ref _XmlNodeChildNodes);


                _XmlNode.AppendChild(_XmlNodeChildNodes);
                #endregion


                clsXmlControl.SaveXml(_XmlNode, XmlFilePath);
                //this.Load();
                return;
            }

        }


        private void SaveCurrentData(ref XmlNode xml)
        {
            string sM1 = Convert.ToInt32(cmbM1Bit0.SelectedIndex + cmbM1Bit1.SelectedIndex * 2).ToString("X2");
            string sM2 = Convert.ToInt32(cmbM2.SelectedIndex).ToString("X2");

            string sM3Para = txtfourBit.Text;

            string m_Para = sM1 + "|" + sM2 + "|" + sM3Para + "|";


            #region -----------整机自检-----------
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "IsCheck"
                    , "Name", "是否要检"
                    , "Value", m_Checked.ToString()));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "CustomerName"
                    , "Name", "项目名称"
                    , "Value", _NodeDescription));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "ProductsName"
                    , "Name", "项目参数"
                    , "Value", m_Para));//M1|M2---M1 : Bit0;0=单板测试  1=整机测试;  Bit1 0=三相四 1=三相三   ； M2 0：外控磁保持  1：内控磁保持   2：外控电保持 ；测试需判断项4个字节

            #endregion

        }

        private void chb1_CheckedChanged(object sender, EventArgs e)
        {
            string sM3Para = "";

            for (int i = 0; i < m_Checkbox.Length; i++)
            {
                if (m_Checkbox[i].Checked)
                {
                    sM3Para = "1" + sM3Para;
                }
                else
                {
                    sM3Para = "0" + sM3Para;
                }
            }
            sM3Para = Convert.ToInt32(sM3Para, 2).ToString("X8");

            txtfourBit.Text = sM3Para;

        }

    }
}
