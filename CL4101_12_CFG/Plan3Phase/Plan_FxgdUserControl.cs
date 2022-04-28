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
    public partial class Plan_FxgdUserControl : UserControl
    {
        private string XmlFilePath = Application.StartupPath + pwFunction.pwConst.Variable.CONST_WORKPLANPATH;
        private string _NodeName = "Plan_SinglePhaseTest";
        private string _NodeDescription = "分相供电测试";
        private bool m_Checked = false;//是否要检
        private string m_Para = "";//项目参数

        public Plan_FxgdUserControl()
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
        private void EventSchemaDataLoad(int intItem,  string _XmlFilePath)//
        {
            if (intItem != 4) return;
            XmlFilePath = _XmlFilePath;
            Load();
        }

        private void Init()
        {

            //初始化==添加行数据"DLT645_1997|FFF9|6|0||"));//协议|标识编码|长度|小数点|下发参数
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
            //string[] _Pata = m_Para.Split('|');
            //txtA.Text = _Pata[0];
            //txtB.Text = _Pata[1];
            //txtC.Text = _Pata[2];
            txtV.Text = m_Para;
        }


        /// <summary>
        /// 创建工单默认配置文件
        /// </summary>
        /// <param name="xml"></param>
        private void CreateDefaultData(ref XmlNode xml)
        {
            #region -----------分相供电测试-----------
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "IsCheck"
                    , "Name", "是否要检"
                    , "Value", "true"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "CustomerName"
                    , "Name", "项目名称"
                    , "Value", "分相供电测试"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "ProductsName"
                    , "Name", "项目参数"
                    , "Value", ""));//协议|标识编码|长度|小数点|下发参数

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
                //_XmlNodeChildNodes.RemoveAll();
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
            //m_Para = txtA.Text + "|";
            //m_Para += txtB.Text + "|";
            //m_Para += txtC.Text + "|";
            m_Para = txtV.Text ;

            #region -----------分相供电测试-----------
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
                    , "Value", m_Para));//A|B|C"

            #endregion
        }


    }
}
