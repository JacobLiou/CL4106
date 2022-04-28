using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using pwFunction;
using System.Xml;
using System.Net;
using pwClassLibrary.DataBase;

namespace CL4100.Plan3Phase
{
    public partial class Plan_AdjustErrorUserControl : UserControl
    {
        private string XmlFilePath = Application.StartupPath + pwFunction.pwConst.Variable.CONST_WORKPLANPATH;
        private string _NodeName = "Plan_AdjustError";
        private string _NodeDescription = "校准误差";
        private bool m_Checked = false;//是否要检
        private string m_Para = "";//项目参数
        private string m_sFilePaht = Application.StartupPath + @"\AdjustConfig_MES.ini";
        #region
        private string[] _ParaName = new string[]{
                    "FractionI",
                    "VolChnGain",
                    "CTSampleRes",
                    "CTRatio",
                    "IbSampleValue",
                    "HFConstK",
                    "Region1",
                    "Region2",
                    "Region3",
                    "Region4",
                    "U_PGA",
                    "I_PGA",
                    "Coef_Ki",
                    "AngleOffset1A",
                    "AngleOffset1B",
                    "AngleOffset1C",
                    "AngleOffset2A",
                    "AngleOffset2B",
                    "AngleOffset2C",
                    "AngleOffset3A",
                    "AngleOffset3B",
                    "AngleOffset3C",
                    "AngleOffset4A",
                    "AngleOffset4B",
                    "AngleOffset4C",
                    "AngleOffset5A",
                    "AngleOffset5B",
                    "AngleOffset5C",

                    "AmpliOffset1A",
                    "AmpliOffset1B",
                    "AmpliOffset1C",
                    "AmpliOffset2A",
                    "AmpliOffset2B",
                    "AmpliOffset2C",
                    "AmpliOffset3A",
                    "AmpliOffset3B",
                    "AmpliOffset3C",
                    "AmpliOffset4A",
                    "AmpliOffset4B",
                    "AmpliOffset4C",
                    "AmpliOffset5A",
                    "AmpliOffset5B",
                    "AmpliOffset5C"};
        #endregion
        private string _NodeNamePara = "Plan_AdjustErrorPara_Item";
        private string _NodeDescriptionPara = "校准误差参数文件";

        public Plan_AdjustErrorUserControl()
        {
            InitializeComponent();
            Init();
            pw_Main.OnEventSchemaDataSave += new pwInterface.DelegateEventSchemaDataSave(EventSchemaDataSave);
            pw_Main.OnEventSchemaDataLoad += new pwInterface.DelegateEventSchemaDataLoad(EventSchemaDataLoad);

        }

        private void EventSchemaDataSave(int intItem, bool Checked, string _XmlFilePath)//
        {
            if (intItem != 7) return;
            XmlFilePath = _XmlFilePath;
            m_Checked = Checked;
            Save();
            SaveAdjustPara();
        }
        private void EventSchemaDataLoad(int intItem, string _XmlFilePath)//
        {
            if (intItem != 7) return;
            XmlFilePath = _XmlFilePath;
            Load();
        }

        private void Init()
        {
            if (!File.Exists(m_sFilePaht))       //检查INI文件是否存在
            {
                MessageBox.Show("AdjustConfig_MES File lost!", "info", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            cmb_ParaFile.Items.Clear();
            string[] sTmp1 = new string[100];            
            for (int i = 0; i < sTmp1.Length; i++)   //增加ini文件条目时，应该相应增加下列表的数目
            {
                sTmp1[i] = pwClassLibrary.pwFile.File.ReadInIString(m_sFilePaht, "ParaFile", i.ToString(), "");
                if (sTmp1[i].Trim() == "") continue;
                cmb_ParaFile.Items.Add(sTmp1[i]);
            }

        }

        private void cmb_ParaFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgv_Para.Rows.Clear();
            DataGridViewRowCollection rows= this.dgv_Para.Rows;

            for (int i = 0; i < _ParaName.Length; i++)
            {
                string sTmp0 = pwClassLibrary.pwFile.File.ReadInIString(m_sFilePaht, cmb_ParaFile.Text, _ParaName[i], "");

                rows.Add(new object[] { i, _ParaName[i], sTmp0 });

            }
            dgv_Para.ReadOnly = true;
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
            cmb_Xieyi.Text = _Pata[0];
            txt_Code.Text = _Pata[1];
            txt_Len.Text = "0"; //_Pata[2];
            txt_Dot.Text = "0"; //_Pata[3];
            txt_Data.Text = ""; //_Pata[4];
            cmb_ParaFile.Text = _Pata[5];

        }


        /// <summary>
        /// 创建工单默认配置文件
        /// </summary>
        /// <param name="xml"></param>
        private void CreateDefaultData(ref XmlNode xml)
        {
            #region -----------校准误差配置-----------
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "IsCheck"
                    , "Name", "是否要检"
                    , "Value", "true"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "CustomerName"
                    , "Name", "项目名称"
                    , "Value", "校准误差"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "ProductsName"
                    , "Name", "项目参数"
                    , "Value", "DLT645_1997|FFF0|0|0|0|0|"));//协议|标识编码|长度|小数点|下发参数

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
            if (_ErrorString != "")
            {
                _XmlNode = clsXmlControl.CreateXmlNode("WorkPlan");

                #region 加载产品配置默认值

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

                #region 加载系统配置默认值

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
            m_Para = cmb_Xieyi.Text + "|";
            m_Para += txt_Code.Text + "|";
            m_Para += txt_Len.Text + "|";
            m_Para += txt_Dot.Text + "|";
            m_Para += txt_Data.Text + "|";
            m_Para += cmb_ParaFile.Text.Trim()  + "|";

            #region -----------读生产编号配置-----------
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
                    , "Value", m_Para));//协议|标识编码|长度|小数点|下发参数"DLT645_1997|FFF9|6|0||"

            #endregion
        }


        public void SaveAdjustPara()
        {
            string _ErrorString = "";
            XmlNode _XmlNodeChildNodes;
            XmlNode _XmlNode = clsXmlControl.LoadXml(XmlFilePath, out _ErrorString);
            if (_ErrorString != "")
            {
                _XmlNode = clsXmlControl.CreateXmlNode("WorkPlan");

                #region 加载产品配置默认值

                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeNamePara, "Description", _NodeDescriptionPara);

                this.SaveCurrentDataAdjustPara(ref _XmlNodeChildNodes);

                _XmlNode.AppendChild(_XmlNodeChildNodes);

                #endregion

                clsXmlControl.SaveXml(_XmlNode, XmlFilePath);
                //this.Load();
                return;
            }
            _XmlNodeChildNodes = clsXmlControl.FindSencetion(_XmlNode, "Plan_AdjustErrorPara_Item");
            if (_XmlNodeChildNodes == null)
            {

                #region 加载系统配置默认值

                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeNamePara, "Description", _NodeDescriptionPara);

                this.SaveCurrentDataAdjustPara(ref _XmlNodeChildNodes);

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
                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeNamePara, "Description", _NodeDescriptionPara);

                this.SaveCurrentDataAdjustPara(ref _XmlNodeChildNodes);


                _XmlNode.AppendChild(_XmlNodeChildNodes);
                #endregion


                clsXmlControl.SaveXml(_XmlNode, XmlFilePath);
                //this.Load();
                return;
            }


        }

        private void SaveCurrentDataAdjustPara(ref XmlNode xml)
        {
            #region -----------保存配置-----------

            DataGridViewRowCollection rows = this.dgv_Para.Rows;

            for (int i = 0; i < _ParaName.Length; i++)
            {
                xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeNamePara
                , "ParaID", Convert.ToInt32 (i+1).ToString()
                , "ParaName", _ParaName[i]
                , "ParaValue", rows[i].Cells[2].Value.ToString()));
            }

            #endregion
        }


    }
}
