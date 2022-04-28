using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Net;
using System.Data;
using pwClassLibrary.DataBase;

namespace pwFunction.pwPlan
{
    public class cPlan_ReadEnergy
    {

        private bool _IsCheck;
        private string _PrjName;
        private string _PrjParameter;
        private string _XmlFilePath = "";
        private string _NodeName = "Plan_ReadEnergy";
        private string _NodeDescription = "读电能表底度";


        /// <summary>
        /// 是否要做
        /// </summary>
        public bool IsCheck
        {
            get { return this._IsCheck; }
        }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string PrjName
        {
            get { return this._PrjName; }
        }


        /// <summary>
        /// 项目参数：协议|标识编码
        /// </summary>
        public string PrjParameter
        {
            get { return this._PrjParameter; }
        }


        /// <summary>
        /// XML文件路径
        /// </summary>
        public string XmlFilePath
        {
            set { value = this._XmlFilePath; }
            get { return this._XmlFilePath; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public cPlan_ReadEnergy()
        {
            _IsCheck = false;
            _PrjName = "";
            _PrjParameter = "";
            _XmlFilePath = Application.StartupPath + pwFunction.pwConst.Variable.CONST_WORKPLANPATH;

        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~cPlan_ReadEnergy()
        {

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


            this._IsCheck = Convert.ToBoolean( _XmlNodeChildNodes.ChildNodes[0].Attributes["Value"].Value );
            this._PrjName = _XmlNodeChildNodes.ChildNodes[1].Attributes["Value"].Value;
            this._PrjParameter = _XmlNodeChildNodes.ChildNodes[2].Attributes["Value"].Value;

        }


        /// <summary>
        /// 创建工单默认配置文件
        /// </summary>
        /// <param name="xml"></param>
        private void CreateDefaultData(ref XmlNode xml)
        {
            #region -----------读电能表底度配置-----------
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "IsCheck"
                    , "Name", "是否要检"
                    , "Value", "true"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "CustomerName"
                    , "Name", "项目名称"
                    , "Value", "读电能表底度"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "ProductsName"
                    , "Name", "项目参数"
                    , "Value", "DLT645_2007|00010000|4|2||1|5"));////协议|标识编码|长度|小数点|下发参数

            #endregion

        }

    }
}
