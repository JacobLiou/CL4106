using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Net;
using System.Data;
using pwClassLibrary.DataBase;
using pwClassLibrary;
namespace pwFunction.pwWork
{

    /// <summary>
    /// 工单类
    /// </summary>
    [Serializable()]
    public class cWork : pwSerializable 
    {
        private string _WorkSN="";
        private string _CustomerName="";
        private string _ProductsName="";
        private string _ProductsSN="";
        private string _ProductsModel="";
        private string _XmlFilePath ="";
        private string _NodeName = "Work";
        private string _NodeDescription = "工单信息";


        /// <summary>
        /// 工单号
        /// </summary>
        public string WorkSN
        {
            get { return this._WorkSN; }
        }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName
        {
            get { return this._CustomerName; }
        }


        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductsName
        {
            get { return this._ProductsName; }
        }

        /// <summary>
        /// 成品编号
        /// </summary>
        public string ProductsSN
        {
            get { return this._ProductsSN; }
        }


        /// <summary>
        /// 产品型号
        /// </summary>
        public string ProductsModel
        {
            get { return this._ProductsModel; }
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
        public cWork()
        {
            _WorkSN = "";
            _CustomerName = "";
            _ProductsName = "";
            _ProductsSN = "";
            _ProductsModel = "";
            _XmlFilePath = Application.StartupPath + pwFunction.pwConst.Variable.CONST_WORKPLANPATH;
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~cWork()
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

                #region 加载工单配置默认值
                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeName, "Description", _NodeDescription);

                this.CreateDefaultData(ref _XmlNodeChildNodes);

                _XmlNode.AppendChild(_XmlNodeChildNodes);

                #endregion

                clsXmlControl.SaveXml(_XmlNode, XmlFilePath);
                this.Load();
                return;
            }
            #region 删除
            //XmlNode _XmlNodeChildNodes = clsXmlControl.FindSencetion(_XmlNode, "Work");

            //if (_XmlNodeChildNodes == null)
            //{
            //    pwFunction.pwFile.File.RemoveFile(XmlFilePath);   //如果发现旧的系统配置文件就要删除掉，再重新创建
            //    this.Load();
            //    return;
            //}
            #endregion
            _XmlNodeChildNodes = clsXmlControl.FindSencetion(_XmlNode, _NodeName);
            if (_XmlNodeChildNodes == null)
            {

                #region 加载工单配置默认值

                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeName, "Description", _NodeDescription);

                this.CreateDefaultData(ref _XmlNodeChildNodes);

                _XmlNode.AppendChild(_XmlNodeChildNodes);
                #endregion

                clsXmlControl.SaveXml(_XmlNode, XmlFilePath);
                this.Load();
                return;
            }


            try
            {
                _WorkSN = _XmlNodeChildNodes.ChildNodes[0].Attributes["Value"].Value;
                _CustomerName = _XmlNodeChildNodes.ChildNodes[1].Attributes["Value"].Value;
                _ProductsName = _XmlNodeChildNodes.ChildNodes[2].Attributes["Value"].Value;
                _ProductsSN = _XmlNodeChildNodes.ChildNodes[3].Attributes["Value"].Value;
                _ProductsModel = _XmlNodeChildNodes.ChildNodes[4].Attributes["Value"].Value;
            }
            catch
            {
                return;
            }

            #region 简易模式==作废==

            //_WorkSN = _XmlNodeChildNodes.ChildNodes[0].Attributes["WorkSN"].Value;
            //_CustomerName = _XmlNodeChildNodes.ChildNodes[0].Attributes["CustomerName"].Value;
            //_ProductsName = _XmlNodeChildNodes.ChildNodes[0].Attributes["ProductsName"].Value;
            //_ProductsSN = _XmlNodeChildNodes.ChildNodes[0].Attributes["ProductsSN"].Value;
            //_ProductsModel = _XmlNodeChildNodes.ChildNodes[0].Attributes["ProductsModel"].Value; 
            #endregion
        }


        /// <summary>
        /// 创建工单默认配置文件
        /// </summary>
        /// <param name="xml"></param>
        private void CreateDefaultData(ref XmlNode xml)
        {
            #region -----------工单信息配置-----------
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "WorkSN"
                    , "Name", "工单号"
                    , "Value", "默认工单号"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "CustomerName"
                    , "Name", "客户名称"
                    , "Value", "默认客户名称"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "ProductsName"
                    , "Name", "产品名称"
                    , "Value", "默认产品名称"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "ProductsSN"
                    , "Name", "产品编号"
                    , "Value", "默认产品编号"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "ProductsModel"
                    , "Name", "产品型号"
                    , "Value", "默号产品型号"));
            #endregion

            #region 简易模式==作废==

            //xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
            //                , "WorkSN", "默认工单号"
            //                , "CustomerName", "默认客户名称"
            //                , "ProductsName", "默认产品名称"
            //                , "ProductsSN", "默认产品编号"
            //                , "ProductsModel", "默号产品型号"));
            #endregion
        }

    }

}
