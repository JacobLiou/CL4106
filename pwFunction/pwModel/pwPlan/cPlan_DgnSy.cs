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
    public class cPlan_DgnSy 
    {

        private bool _IsCheck;
        private string _PrjName;
        private string _PrjParameter;
        private string _XmlFilePath = "";
        private string _NodeName = "Plan_DgnSy";
        private string _NodeDescription = "日计时误差检定";


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
        /// 项目参数
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
        public cPlan_DgnSy()
        {
            _IsCheck = false;
            _PrjName = "";
            _PrjParameter = "";
            _XmlFilePath = Application.StartupPath + pwFunction.pwConst.Variable.CONST_WORKPLANPATH;

        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~cPlan_DgnSy()
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


            this._IsCheck =Convert.ToBoolean( _XmlNodeChildNodes.ChildNodes[0].Attributes["Value"].Value );
            this._PrjName = _XmlNodeChildNodes.ChildNodes[1].Attributes["Value"].Value;
            this._PrjParameter = _XmlNodeChildNodes.ChildNodes[2].Attributes["Value"].Value;

        }


        /// <summary>
        /// 创建默认配置文件
        /// </summary>
        /// <param name="xml"></param>
        private void CreateDefaultData(ref XmlNode xml)
        {
            #region -----------创建默认配置-----------
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "IsCheck"
                    , "Name", "是否要检"
                    , "Value", "true"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "CustomerName"
                    , "Name", "项目名称"
                    , "Value", "日计时误差检定"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "ProductsName"
                    , "Name", "项目参数"
                    , "Value", "5000000|1|1"));//标准时钟脉冲常数|被检表时钟频率|圈数

            #endregion

        }

    }
}
