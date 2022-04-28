using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Net;
using System.Data;
using pwClassLibrary.DataBase;
using pwClassLibrary;
namespace pwFunction.pwPlan
{
    /// <summary>
    /// ��������
    /// </summary>
    [Serializable()]
    public class cPlanBase : pwSerializable
    {
        private bool _IsCheck;
        private string _PrjName;
        private string _PrjParameter;
        private string _XmlFilePath = "";

        /// <summary>
        /// �Ƿ�Ҫ��
        /// </summary>
        public bool IsCheck
        {
            get { return this._IsCheck; }
        }

        /// <summary>
        /// ��Ŀ����
        /// </summary>
        public string PrjName
        {
            get { return this._PrjName; }
        }


        /// <summary>
        /// ��Ŀ����
        /// </summary>
        public string PrjParameter
        {
            get { return this._PrjParameter; }
        }


        /// <summary>
        /// XML�ļ�·��
        /// </summary>
        public string XmlFilePath
        {
            set { value = this._XmlFilePath; }
            get { return this._XmlFilePath; }
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        public cPlanBase()
        {
            _IsCheck =false;
            _PrjName = "";
            _PrjParameter = "";
            _XmlFilePath = Application.StartupPath + pwFunction.pwConst.Variable.CONST_WORKPLANPATH;
        }

        /// <summary>
        /// ��������
        /// </summary>
        ~cPlanBase()
        {

        }


        /// <summary>
        /// ��ȡXML�ļ���ʼ������������Ϣ
        /// </summary>
        public virtual void Load()
        {

            string _ErrorString = "";
            XmlNode _XmlNodeChildNodes;
            XmlNode _XmlNode = clsXmlControl.LoadXml(XmlFilePath, out _ErrorString);
            if (_ErrorString != "")
            {
                _XmlNode = clsXmlControl.CreateXmlNode("WorkPlan");

                #region ����ϵͳ����Ĭ��ֵ
                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode("Plan");

                this.CreateDefaultData(ref _XmlNodeChildNodes);

                _XmlNode.AppendChild(_XmlNodeChildNodes);

                #endregion

                clsXmlControl.SaveXml(_XmlNode, XmlFilePath);
                this.Load();
                return;
            }

            _XmlNodeChildNodes = clsXmlControl.FindSencetion(_XmlNode, "Plan");
            if (_XmlNodeChildNodes == null)
            {

                #region ����ϵͳ����Ĭ��ֵ

                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode("Plan");

                this.CreateDefaultData(ref _XmlNodeChildNodes);

                _XmlNode.AppendChild(_XmlNodeChildNodes);
                #endregion

                clsXmlControl.SaveXml(_XmlNode, XmlFilePath);
                this.Load();
                return;
            }


            this._IsCheck = Convert.ToBoolean(_XmlNodeChildNodes.ChildNodes[0].Attributes["Value"].Value);
            this._PrjName = _XmlNodeChildNodes.ChildNodes[1].Attributes["Value"].Value;
            this._PrjParameter = _XmlNodeChildNodes.ChildNodes[2].Attributes["Value"].Value;

        }


        /// <summary>
        /// ��������Ĭ�������ļ�
        /// </summary>
        /// <param name="xml"></param>
        private void CreateDefaultData(ref XmlNode xml)
        {
            #region -----------����-----------
            xml.AppendChild(clsXmlControl.CreateXmlNode("Plan"
                    , "ID", "IsCheck"
                    , "Name", "�Ƿ�Ҫ��"
                    , "Value", "flase"));
            xml.AppendChild(clsXmlControl.CreateXmlNode("Plan"
                    , "ID", "CustomerName"
                    , "Name", "��Ŀ����"
                    , "Value", "Ĭ����Ŀ"));
            xml.AppendChild(clsXmlControl.CreateXmlNode("Plan"
                    , "ID", "ProductsName"
                    , "Name", "��Ŀ����"
                    , "Value", "Ĭ�ϲ���"));

            #endregion

        }
    }

}
