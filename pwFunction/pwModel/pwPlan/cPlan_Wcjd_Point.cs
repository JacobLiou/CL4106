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
    public class cPlan_Wcjd_Point 
    {


        public List<pwInterface.StWcPoint> _WcPoint;
        private string _XmlFilePath = "";
        private string _NodeName = "Plan_Wcjd_Point";
        private string _NodeDescription = "误差检定点信息";



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
        public cPlan_Wcjd_Point()
        {
            _WcPoint = new List<StWcPoint>();
            _XmlFilePath = Application.StartupPath + pwFunction.pwConst.Variable.CONST_WORKPLANPATH;

        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~cPlan_Wcjd_Point()
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
            _WcPoint.Clear();
            for (int _i = 0; _i < _XmlNodeChildNodes.ChildNodes.Count; _i++)
            {
                StWcPoint _Item = new StWcPoint();
                _Item.PrjID = _XmlNodeChildNodes.ChildNodes[_i].Attributes[0].Value;
                _Item.PrjName = _XmlNodeChildNodes.ChildNodes[_i].Attributes[1].Value;
                _Item.DLHL = _XmlNodeChildNodes.ChildNodes[_i].Attributes[2].Value;
                _Item.GLFX = _XmlNodeChildNodes.ChildNodes[_i].Attributes[3].Value;
                _Item.YJ = _XmlNodeChildNodes.ChildNodes[_i].Attributes[4].Value;
                _Item.GLYS = _XmlNodeChildNodes.ChildNodes[_i].Attributes[5].Value;
                _Item.xIb = _XmlNodeChildNodes.ChildNodes[_i].Attributes[6].Value;
                _Item.Qs = _XmlNodeChildNodes.ChildNodes[_i].Attributes[7].Value;
                _Item.fMax = _XmlNodeChildNodes.ChildNodes[_i].Attributes[8].Value;
                _Item.fMin = _XmlNodeChildNodes.ChildNodes[_i].Attributes[9].Value;
                _Item.PC = _XmlNodeChildNodes.ChildNodes[_i].Attributes[10].Value;
                _WcPoint.Add(_Item);
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
                    , "PrjName", "回路1 P+ 合元 1.0 Imax 基本误差"
                    , "DLHL", "0"
                    , "GLFX", "正向有功"
                    , "YJ", "合元"
                    , "GLYS", "1.0"
                    , "xIb", "12Ib"
                    , "Qs", "5"
                    , "fMax", "1.0"
                    , "fMin", "-1.0"
                    , "PC", "false"));

            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "PrjID", "2"
                    , "PrjName", "回路1 P+ 合元 0.5L Imax 基本误差"
                    , "DLHL", "0"
                    , "GLFX", "正向有功"
                    , "YJ", "合元"
                    , "GLYS", "0.5L"
                    , "xIb", "12Ib"
                    , "Qs", "2"
                    , "fMax", "1.0"
                    , "fMin", "-1.0"
                    , "PC", "false"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "PrjID", "3"
                    , "PrjName", "回路1 P+ 合元 1.0 1.0Ib 基本误差"
                    , "DLHL", "0"
                    , "GLFX", "正向有功"
                    , "YJ", "合元"
                    , "GLYS", "1.0"
                    , "xIb", "1.0Ib"
                    , "Qs", "1"
                    , "fMax", "1.0"
                    , "fMin", "-1.0"
                    , "PC", "false"));

            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                   , "PrjID", "4"
                   , "PrjName", "回路1 P+ 合元 0.5L 1.0Ib 基本误差"
                   , "DLHL", "0"
                   , "GLFX", "正向有功"
                   , "YJ", "合元"
                   , "GLYS", "0.5L"
                   , "xIb", "1.0Ib"
                   , "Qs", "1"
                   , "fMax", "1.0"
                   , "fMin", "-1.0"
                   , "PC", "false"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                   , "PrjID", "5"
                   , "PrjName", "回路1 P+ 合元 1.0 0.1Ib 基本误差"
                   , "DLHL", "0"
                   , "GLFX", "正向有功"
                   , "YJ", "合元"
                   , "GLYS", "1.0"
                   , "xIb", "0.1Ib"
                   , "Qs", "1"
                   , "fMax", "1.0"
                   , "fMin", "-1.0"
                   , "PC", "false"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                   , "PrjID", "6"
                   , "PrjName", "回路1 P+ 合元 1.0 0.05Ib 基本误差"
                   , "DLHL", "0"
                   , "GLFX", "正向有功"
                   , "YJ", "合元"
                   , "GLYS", "1.0"
                   , "xIb", "0.05Ib"
                   , "Qs", "1"
                   , "fMax", "1.0"
                   , "fMin", "-1.0"
                   , "PC", "false"));

            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "PrjID", "7"
                  , "PrjName", "回路1 P- A元 1.0 1.0Ib 基本误差"
                  , "DLHL", "0"
                  , "GLFX", "反向有功"
                  , "YJ", "A元"
                  , "GLYS", "1.0"
                  , "xIb", "1.0Ib"
                  , "Qs", "1"
                  , "fMax", "1.0"
                  , "fMin", "-1.0"
                  , "PC", "false"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "PrjID", "8"
                  , "PrjName", "回路1 Q+ B元 1.0 1.0Ib 基本误差"
                  , "DLHL", "0"
                  , "GLFX", "正向无功"
                  , "YJ", "B元"
                  , "GLYS", "1.0"
                  , "xIb", "1.0Ib"
                  , "Qs", "1"
                  , "fMax", "1.0"
                  , "fMin", "-1.0"
                  , "PC", "false"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "PrjID", "9"
                  , "PrjName", "回路1 Q- C元 1.0 1.0Ib 基本误差"
                  , "DLHL", "0"
                  , "GLFX", "反向无功"
                  , "YJ", "C元"
                  , "GLYS", "1.0"
                  , "xIb", "1.0Ib"
                  , "Qs", "1"
                  , "fMax", "1.0"
                  , "fMin", "-1.0"
                  , "PC", "false"));

            #endregion

        }

    }

    //public struct StWcPoint
    //{
    //    public int intNumber;
    //    public string PrjName;
    //    public string GLFX;
    //    public string YJ;
    //    public string GLYS;
    //    public string xIb;
    //    public string Qs;
    //    public string fMax;
    //    public string fMin;
    //    public string PC;
    //    public void Init()
    //    {
    //        intNumber = 0;
    //        PrjName = "";
    //        GLFX = "";
    //        YJ = "";
    //        GLYS = "";
    //        xIb = "";
    //        Qs = "";
    //        fMax = "";
    //        fMin = "";
    //        PC = "";
    //    }
    //}

    //public class cWcPoint
    //{
    //    private string _PrjName;
    //    private string _GLFX;
    //    private string _YJ;
    //    private string _GLYS;
    //    private string _xIb;
    //    private string _Qs;
    //    private string _WcMax;
    //    private string _WcMin;
    //    private bool _IsCheckPc;
    //    /// <summary>
    //    /// 项目名称
    //    /// </summary>
    //    public string PrjName
    //    {
    //        get { return this._PrjName; }
    //    }

    //    /// <summary>
    //    /// 功率方向 1=正向有功，2=反向有功，3=正向无功，4=反向无功
    //    /// </summary>
    //    public string GLFX
    //    {
    //        get { return this._GLFX; }
    //    }
    //    /// <summary>
    //    /// 元件 0=合元，1=A元，2=B元，3=C元，
    //    /// </summary>
    //    public string YJ
    //    {
    //        get { return this._YJ; }
    //    }
    //    /// <summary>
    //    /// 功率因素		1.0，0.5L，0.8C，0.25L等
    //    /// </summary>
    //    public string GLYS
    //    {
    //        get { return this._GLYS; }
    //    }
    //    /// <summary>
    //    /// 电流倍数		Imax,3.0Ib等
    //    /// </summary>
    //    public string xIb
    //    {
    //        get { return this._xIb; }
    //    }
    //    /// <summary>
    //    /// 圈数
    //    /// </summary>
    //    public int Qs
    //    {
    //        get { return this._Qs; }
    //    }
    //    /// <summary>
    //    /// 误差上限
    //    /// </summary>
    //    public string WcMax
    //    {
    //        get { return this._WcMax; }
    //    }
    //    /// <summary>
    //    /// 误差下限
    //    /// </summary>
    //    public string WcMin
    //    {
    //        get { return this._WcMin; }
    //    }
    //    /// <summary>
    //    /// 是否检定偏差
    //    /// </summary>
    //    public bool IsCheckPc
    //    {
    //        get { return this._IsCheckPc; }
    //    }


    //}
}
