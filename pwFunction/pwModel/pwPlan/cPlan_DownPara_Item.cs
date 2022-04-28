using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Net;
using System.Data;
using pwClassLibrary.DataBase;
using pwInterface;
namespace pwFunction.pwPlan
{
    public class cPlan_DownPara_Item 
    {


        public List<MeterDownParaItem> _DownParaItem;
        private string _XmlFilePath = "";
        private string _NodeName = "Plan_DownPara_Item";
        private string _NodeDescription = "打包参数下载项信息";



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
        public cPlan_DownPara_Item()
        {
            _DownParaItem = new List<MeterDownParaItem>();
            _XmlFilePath = Application.StartupPath + pwFunction.pwConst.Variable.CONST_WORKPLANPATH;

        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~cPlan_DownPara_Item()
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
            _DownParaItem.Clear();
            for (int _i = 0; _i < _XmlNodeChildNodes.ChildNodes.Count; _i++)
            {
                MeterDownParaItem _Item = new MeterDownParaItem();
                _Item.Item_PrjID = _XmlNodeChildNodes.ChildNodes[_i].Attributes[0].Value;
                _Item.Item_TxFrame = _XmlNodeChildNodes.ChildNodes[_i].Attributes[1].Value;
                _Item.Item_RxFrame = _XmlNodeChildNodes.ChildNodes[_i].Attributes[2].Value;
                _DownParaItem.Add(_Item);
            }

        }


        /// <summary>
        /// 创建默认配置文件
        /// </summary>
        /// <param name="xml"></param>
        private void CreateDefaultData(ref XmlNode xml)
        {
            #region -----------创建默认配置-----------
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "PrjID", "1"
                    , "TxFrame", "68 11 11 11 11 11 11 68 14 0D 34 39 33 37 35 33 33 33 33 33 33 33 38 00 16"
                    , "RxFrame", "68 11 11 11 11 11 11 68 94 00 CA 16"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                   , "PrjID", "2"
                    , "TxFrame", "68 11 11 11 11 11 11 68 14 0D 34 39 33 37 35 33 33 33 33 33 33 33 38 00 16"
                    , "RxFrame", "68 11 11 11 11 11 11 68 94 00 CA 16"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                   , "PrjID", "3"
                    , "TxFrame", "68 11 11 11 11 11 11 68 14 0D 34 39 33 37 35 33 33 33 33 33 33 33 38 00 16"
                    , "RxFrame", "68 11 11 11 11 11 11 68 94 00 CA 16"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "PrjID", "4"
                    , "TxFrame", "68 11 11 11 11 11 11 68 14 0D 34 39 33 37 35 33 33 33 33 33 33 33 38 00 16"
                    , "RxFrame", "68 11 11 11 11 11 11 68 94 00 CA 16"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                   , "PrjID", "5"
                    , "TxFrame", "68 11 11 11 11 11 11 68 14 0D 34 39 33 37 35 33 33 33 33 33 33 33 38 00 16"
                    , "RxFrame", "68 11 11 11 11 11 11 68 94 00 CA 16"));
            #endregion

        }

    }
}
