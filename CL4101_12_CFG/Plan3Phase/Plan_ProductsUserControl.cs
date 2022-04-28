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
using System.Xml.Linq;
namespace CL4100.Plan3Phase
{
    public partial class Plan_ProductsUserControl : UserControl
    {

        private string XmlFilePath = Application.StartupPath + pwFunction.pwConst.Variable.CONST_WORKPLANPATH;
        private string _NodeName = "Products";
        private string _NodeDescription = "产品信息";

        public Plan_ProductsUserControl()
        {
            InitializeComponent();
            Init();
            pw_Main.OnEventSchemaDataSave += new pwInterface.DelegateEventSchemaDataSave(EventSchemaDataSave);
            pw_Main.OnEventProductChange+=new pwInterface.DelegateEventProductChange(EventProductChange);
            pw_Main.OnEventSchemaDataLoad += new pwInterface.DelegateEventSchemaDataLoad(EventSchemaDataLoad);

        }

        private void EventSchemaDataSave(int intItem, bool Checked,string _XmlFilePath)//
        {
            if (intItem != 0) return;
            XmlFilePath =  _XmlFilePath;
            Save();
        }
        private void EventSchemaDataLoad(int intItem, string _XmlFilePath)//
        {
            if (intItem != 0) return;
            XmlFilePath = _XmlFilePath;
            Load();
        }

        private void EventProductChange(string ProductPara)
        {
            string[] _Pata = ProductPara.Split('|');
            txtTestType.Text = _Pata[0];
            txtBasicV.Text = _Pata[1];
            txtBasicI.Text = _Pata[2];
            txtMaxI.Text = _Pata[3];
            txtConst.Text = _Pata[4];
            txtLevel.Text = _Pata[5];
            txtHZ.Text = _Pata[6];
            txtDuanZi.Text = _Pata[7];
            txtgongyinggongyang.Text = _Pata[8];
            txtMaiChong.Text = _Pata[9];
            txtJiDianQi.Text = _Pata[10];
            //txtRevSoftNo.Text = _Pata[11];
            txt_MeterType.Text = _Pata[11];

        }

        private void Init()
        {

            //初始化==添加行数据

            //object[][] row ={
            //    new object[]  { "测量方式", "单相" },
            //    new object[]  { "额定电压", "220V" },
            //    new object[]  { "额定电流", "5A" },
            //    new object[]  { "最大电流", "60A" },
            //    new object[]  { "常数", "1000" },
            //    new object[]  { "等级", "1.0" },
            //    new object[]  { "频率", "50Hz" },
            //    new object[]  { "端子类型", "国网端子" },
            //    new object[]  { "共阴共阳", "共阴" },
            //    new object[]  { "脉冲类型", "脉冲盒" },
            //    new object[]  { "继电器类型", "内置继电器" },
            //    new object[]  { "软件版本号", "" },
            //};
            //DataGridViewRowCollection rows = this.Data_Products.Rows;
            //for (int i = 0; i < row.Length; i++)
            //{
            //    rows.Add(row[i]);
            //}
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


            try
            {
                txtTestType.Text = _XmlNodeChildNodes.ChildNodes[0].Attributes["Value"].Value;
                txtBasicV.Text  = _XmlNodeChildNodes.ChildNodes[1].Attributes["Value"].Value;
                txtBasicI.Text = _XmlNodeChildNodes.ChildNodes[2].Attributes["Value"].Value;
                txtMaxI.Text = _XmlNodeChildNodes.ChildNodes[3].Attributes["Value"].Value;
                txtConst.Text = _XmlNodeChildNodes.ChildNodes[4].Attributes["Value"].Value;
                txtLevel.Text = _XmlNodeChildNodes.ChildNodes[5].Attributes["Value"].Value;
                txtHZ.Text = _XmlNodeChildNodes.ChildNodes[6].Attributes["Value"].Value;
                txtDuanZi.Text = _XmlNodeChildNodes.ChildNodes[7].Attributes["Value"].Value;
                txtgongyinggongyang.Text = _XmlNodeChildNodes.ChildNodes[8].Attributes["Value"].Value;
                txtMaiChong.Text = _XmlNodeChildNodes.ChildNodes[9].Attributes["Value"].Value;
                txtJiDianQi.Text = _XmlNodeChildNodes.ChildNodes[10].Attributes["Value"].Value;
              //txtRevSoftNo.Text = _XmlNodeChildNodes.ChildNodes[11].Attributes["Value"].Value;
            }
            catch
            {
                return;
            }

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

                #region 加载产品配置默认值
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

                SaverWorkInfo();
                //this.Load();
                return;
            }


        }

        private void SaverWorkInfo()
        {
            try
            {
                XDocument xmldoc = XDocument.Load(XmlFilePath);
                XElement WorkInformation = xmldoc.Root.Elements("Work").Where(el => el.Attribute("Description").Value == "工单信息").First();
                WorkInformation.Elements("Work").Where(el => el.Attribute("ID").Value == "ProductsModel").First().Attribute("Value").Value = txt_MeterType.Text;
                xmldoc.Save(XmlFilePath);
            }
            catch
            {

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
                  , "Value", "1.5A"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "IMax"
                  , "Name", "电流"
                  , "Value", "6A"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "Constant"
                  , "Name", "常数"
                  , "Value", "5000"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "DJ"
                  , "Name", "等级"
                  , "Value", "0.5"));
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
            //xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
            //      , "ID", "SoftVer"
            //      , "Name", "软件版本号"
            //      , "Value", ""));//与RS485读版本号比对用1E0091050400630020130115

            #endregion
        }


        /// <summary>
        /// 创建产品默认配置文件
        /// </summary>
        /// <param name="xml"></param>
        private void SaveCurrentData(ref XmlNode xml)
        {
            #region -----------产品信息配置-----------
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "Clfs"
                  , "Name", "测量方式"
                  , "Value", txtTestType.Text));

            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "Ub"
                  , "Name", "电压"
                  , "Value", txtBasicV.Text));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "Ib"
                  , "Name", "电流"
                  , "Value", txtBasicI.Text));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "IMax"
                  , "Name", "电流"
                  , "Value", txtMaxI.Text));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "Constant"
                  , "Name", "常数"
                  , "Value", txtConst.Text));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "DJ"
                  , "Name", "等级"
                  , "Value", txtLevel.Text));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "PL"
                  , "Name", "频率"
                  , "Value", txtHZ.Text));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "DzType"
                  , "Name", "端子类型"
                  , "Value", txtDuanZi.Text));//国网端子\南网端子
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "GYGY"
                  , "Name", "共阴共阳"
                  , "Value", txtgongyinggongyang.Text));//共阴\共阳
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "PulseType"
                  , "Name", "脉冲类型"
                  , "Value", txtMaiChong.Text));//脉冲盒\光电头，
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                  , "ID", "JDQType"
                  , "Name", "继电器类型"
                  , "Value", txtJiDianQi.Text));//00时表示选择测试内置继电器，为01时表示选择测试外置继电器，为02时表示外置隔离继电器
            //xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
            //      , "ID", "SoftVer"
            //      , "Name", "软件版本号"
            //      , "Value", txtRevSoftNo.Text));//与RS485读版本号比对用

            #endregion
        }

        private void txtBasicI_TextChanged(object sender, EventArgs e)
        {
            CParam.IBasic = Convert.ToDouble(txtBasicI.Text.ToUpper().Replace("A", ""));
        }

        private void txtMaxI_TextChanged(object sender, EventArgs e)
        {
            CParam.IMax = Convert.ToDouble(txtMaxI.Text.ToUpper().Replace("A", ""));
        }


    }
}
