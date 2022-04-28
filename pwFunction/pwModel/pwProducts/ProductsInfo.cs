using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Net;
using System.Data;
using pwClassLibrary.DataBase;

namespace pwFunction.pwProducts
{
    /// <summary>
    /// 产品属性类(表)
    /// </summary>
    [Serializable()]
    public class cProducts
    {

        private string _Clfs = "";
        private string _Ub = "";
        private string _Ib = "";
        private string _IMax = "";
        private string _Constant = "";
        private string _DJ = "";
        private string _PL = "";
        private string _DzType = "";
        private string _GYGY = "";
        private string _PulseType = "";
        private string _JDQType = "";
        private string _SoftVer = "";
        private string _SelfCheckItem = "";
        private string _XmlFilePath = "";
        private string _NodeName = "Products";
        private string _NodeDescription = "产品信息";


        /// <summary>
        /// 测量方式
        /// </summary>
        public string Clfs
        {
            get { return this._Clfs; }
        }

        /// <summary>
        /// 基本电压
        /// </summary>
        public string Ub        
        {
            get { return this._Ub; }
        }

        /// <summary>
        /// 基本电流
        /// </summary>
        public string Ib
        {
            get { return this._Ib; }
        }

        /// <summary>
        /// 最大电流
        /// </summary>
        public string IMax
        {
            get { return this._IMax; }
        }

        /// <summary>
        /// 常数
        /// </summary>
        public string Constant
        {
            get { return this._Constant; }
        }

        /// <summary>
        /// 等级
        /// </summary>
        public string DJ
        {
            get { return this._DJ; }
        }

        /// <summary>
        /// 频率
        /// </summary>
        public string PL
        {
            get { return this._PL; }
        }

        /// <summary>
        /// 端子类型
        /// </summary>
        public string DzType
        {
            get { return this._DzType; }
        }

        /// <summary>
        /// 共阴共阳，
        /// </summary>
        public string GYGY
        {
            get { return this._GYGY; }
        }

        /// <summary>
        /// 脉冲类型0:脉冲盒，1：光电头，
        /// </summary>
        public string PulseType 
        {
            get { return this._PulseType; }
        }
        
        /// <summary>
        /// 继电器类型：00时表示选择测试内置继电器，01表示选择测试外置继电器，02时表示外置隔离继电器
        /// </summary>
        public string JDQType 
        {
            get { return this._JDQType; }
        }

        /// <summary>
        /// 软件版本号
        /// </summary>
        public string SoftVer
        {
            get { return this._SoftVer; }
        }

        /// <summary>
        /// 自检项目4字节 32位，0为不检，1为要检
        /// </summary>
        public string SelfCheckItem
        {
            get { return this._SelfCheckItem; }
            set { this._SelfCheckItem=value ; }

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
        public cProducts()
        {
            _Clfs = "三相四线";
            _Ub = "220V";
            _Ib = "5A";
            _IMax = "60A";
            _Constant = "1600";
            _DJ = "1.0";
            _PL = "50";
            _DzType = "国网端子";
            _GYGY = "共阴";
            _PulseType = "脉冲盒";
            _JDQType = "内置继电器";
            _SoftVer = "";
            _SelfCheckItem = "00000000";
            _XmlFilePath = Application.StartupPath + pwFunction.pwConst.Variable.CONST_WORKPLANPATH;

        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~cProducts()
        {
            
        }

        /// <summary>
        /// 组合结构体参数
        /// </summary>
        /// <returns></returns>
        public string Jion()
        {
            return (_Clfs.ToString() 
                + " | 电压：" + _Ub.ToString() 
                + " | 电流：" + _Ib.ToUpper().Replace("A", "") 
                + "(" + _IMax.ToUpper().Replace("A", "") + ")A" 
                + " | 常数：" + _Constant.ToString() 
                + " | 等级：" + _DJ.ToString() 
                + " | 频率：" + _PL.ToString()
                + " | " + _DzType.ToString()
                + " | " + _JDQType.ToString());

       }

        /// <summary>
        /// 读取XML文件初始化产品配置信息
        /// </summary>
        public void Load()
        {

            string _ErrorString = "";
            XmlNode _XmlNodeChildNodes;
            XmlNode _XmlNode = clsXmlControl.LoadXml(XmlFilePath, out _ErrorString);
            if (_ErrorString != "")
            {
                _XmlNode = clsXmlControl.CreateXmlNode("WorkPlan");

                #region 加载产品配置默认值

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

                #region 加载产品配置默认值
                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeName, "Description", _NodeDescription);


                this.CreateDefaultData(ref _XmlNodeChildNodes);

                _XmlNode.AppendChild(_XmlNodeChildNodes);
                #endregion

                clsXmlControl.SaveXml(_XmlNode, XmlFilePath);
                this.Load();
                return;
            }



            _Clfs = _XmlNodeChildNodes.ChildNodes[0].Attributes["Value"].Value;
            _Ub = _XmlNodeChildNodes.ChildNodes[1].Attributes["Value"].Value;
            _Ib = _XmlNodeChildNodes.ChildNodes[2].Attributes["Value"].Value;
            _IMax = _XmlNodeChildNodes.ChildNodes[3].Attributes["Value"].Value;
            _Constant = _XmlNodeChildNodes.ChildNodes[4].Attributes["Value"].Value;
            _DJ = _XmlNodeChildNodes.ChildNodes[5].Attributes["Value"].Value;
            _PL = _XmlNodeChildNodes.ChildNodes[6].Attributes["Value"].Value;
            _DzType =_XmlNodeChildNodes.ChildNodes[7].Attributes["Value"].Value;
            _GYGY = _XmlNodeChildNodes.ChildNodes[8].Attributes["Value"].Value;
            _PulseType = _XmlNodeChildNodes.ChildNodes[9].Attributes["Value"].Value;
            try
            {
                _JDQType = _XmlNodeChildNodes.ChildNodes[10].Attributes["Value"].Value;
                _SoftVer = _XmlNodeChildNodes.ChildNodes[11].Attributes["Value"].Value;
            }
            catch
            {
                return;
            }

        }

        /// <summary>
        /// 创建产品默认配置文件
        /// </summary>
        /// <param name="xml"></param>
        private void CreateDefaultData(ref XmlNode xml)
        {
            #region -----------产品信息配置-----------
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "Clfs"
                  , "Name", "测量方式"
                  , "Value", "三相四线"));

            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "Ub"
                  , "Name", "电压"
                  , "Value", "220V"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "Ib"
                  , "Name", "电流"
                  , "Value", "5A"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "IMax"
                  , "Name", "电流"
                  , "Value", "60A"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "Constant"
                  , "Name", "常数"
                  , "Value", "400(400)"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "DJ"
                  , "Name", "等级"
                  , "Value", "0.5(1.0)"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "PL"
                  , "Name", "频率"
                  , "Value", "50"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "DzType"
                  , "Name", "端子类型"
                  , "Value", "国网端子"));//国网端子\南网端子
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "GYGY"
                  , "Name", "共阴共阳"
                  , "Value", "共阴"));//共阴\共阳
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "PulseType"
                  , "Name", "脉冲类型"
                  , "Value", "脉冲盒"));//脉冲盒\光电头，
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "JDQType"
                  , "Name", "继电器类型"
                  , "Value", "内置继电器"));//00时表示选择测试内置继电器，为01时表示选择测试外置继电器，为02时表示外置隔离继电器
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "SoftVer"
                  , "Name", "软件版本号"
                  , "Value", "3E0091050400630020130115"));//与RS485读版本号比对用

            #endregion
        }
    }
}
