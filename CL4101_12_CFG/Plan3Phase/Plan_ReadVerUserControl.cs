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
    public partial class Plan_ReadVerUserControl : UserControl
    {
        private string XmlFilePath = Application.StartupPath + pwFunction.pwConst.Variable.CONST_WORKPLANPATH;
        private string _NodeName = "Plan_ReadVer";
        private string _NodeDescription = "RS485读版本号";
        private bool m_Checked = false;//是否要检
        private string m_Para = "";//项目参数

        public Plan_ReadVerUserControl()
        {
            InitializeComponent();
            Init();
            pw_Main.OnEventSchemaDataSave += new pwInterface.DelegateEventSchemaDataSave(EventSchemaDataSave);
            pw_Main.OnEventSchemaDataLoad += new pwInterface.DelegateEventSchemaDataLoad(EventSchemaDataLoad);

        }

        private void EventSchemaDataSave(int intItem, bool Checked, string _XmlFilePath)//
        {
            if (intItem != 2) return;
            XmlFilePath = _XmlFilePath;
            m_Checked = Checked;
            Save();

        }
        private void EventSchemaDataLoad(int intItem, string _XmlFilePath)//
        {
            if (intItem != 2) return;
            XmlFilePath = _XmlFilePath;
            Load();
        }

        private void Init()
        {

            //初始化==添加行数据"DLT645_1997|FFF9|6|0||"));//协议|标识编码|长度|小数点|下发参数
        }

        /// <summary>
        /// 读取XML文件初始化工单配置信息
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
            txt_Len.Text = _Pata[2];
            txt_Dot.Text = _Pata[3];
            txt_Data.Text = _Pata[4];


        }


        /// <summary>
        /// 创建工单默认配置文件
        /// </summary>
        /// <param name="xml"></param>
        private void CreateDefaultData(ref XmlNode xml)
        {
            #region -----------红外口读版本号(软件版本验证)-----------
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "IsCheck"
                    , "Name", "是否要检"
                    , "Value", "true"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "CustomerName"
                    , "Name", "项目名称"
                    , "Value", "RS485读版本号"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "ProductsName"
                    , "Name", "项目参数"
                    , "Value", "DLT645_2007|04CC1100|32|0||"));//协议|标识编码|长度|小数点|下发参数|软件版本号

            #endregion

        }


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


        /// <summary>
        /// 创建产品默认配置文件
        /// </summary>
        /// <param name="xml"></param>
        private void SaveCurrentData(ref XmlNode xml)
        {
            m_Para = cmb_Xieyi.Text + "|";
            m_Para += txt_Code.Text + "|";
            m_Para += txt_Len.Text + "|";
            m_Para += txt_Dot.Text + "|";
            m_Para += txt_Data.Text + "|";
            #region -----------红外口读版本号(软件版本验证)-----------
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
                    , "Value",m_Para ));//协议|标识编码|长度|小数点|下发参数|软件版本号"DLT645_2007|04CC1100|32|0||"

            #endregion

        }

    }
}
