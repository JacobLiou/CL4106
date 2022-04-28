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
using System.IO;
using pwClassLibrary.DataBase;
namespace CL4100.Plan3Phase
{
    public partial class Plan_DownParaUserControl : UserControl
    {
        private string XmlFilePath = Application.StartupPath + pwFunction.pwConst.Variable.CONST_WORKPLANPATH;
        private string _NodeName = "Plan_DownPara";
        private string _NodeDescription = "打包参数下载信息";
        private string _NodeNameItem = "Plan_DownPara_Item";
        private string _NodeDescriptionItem = "打包参数下载项信息";
        private bool m_Checked = false;//是否要检
        private string m_Para = "";//项目参数

        public Plan_DownParaUserControl()
        {
            InitializeComponent();
            Init();
            pw_Main.OnEventSchemaDataSave += new pwInterface.DelegateEventSchemaDataSave(EventSchemaDataSave);
            pw_Main.OnEventSchemaDataLoad += new pwInterface.DelegateEventSchemaDataLoad(EventSchemaDataLoad);
        }

        private void EventSchemaDataSave(int intItem, bool Checked, string _XmlFilePath)//
        {
            if (intItem != 7) return;
            XmlFilePath = _XmlFilePath;
            m_Checked = Checked;
            Save();
        }
        private void EventSchemaDataLoad(int intItem,  string _XmlFilePath)//
        {
            if (intItem != 7) return;
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
            XmlNode _XmlNodeChildNodesItem;
            XmlNode _XmlNode = clsXmlControl.LoadXml(XmlFilePath, out _ErrorString);
            if (_ErrorString != "")
            {
                _XmlNode = clsXmlControl.CreateXmlNode("WorkPlan");

                #region 打包参数下载信息
                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeName, "Description", _NodeDescription);

                this.CreateDefaultData(ref _XmlNodeChildNodes);

                _XmlNode.AppendChild(_XmlNodeChildNodes);

                #endregion

                #region 打包参数下载项信息
                _XmlNodeChildNodesItem = clsXmlControl.CreateXmlNode(_NodeNameItem, "Description", _NodeDescriptionItem);

                this.CreateDefaultDataItem(ref _XmlNodeChildNodesItem);

                _XmlNode.AppendChild(_XmlNodeChildNodesItem);

                #endregion

                clsXmlControl.SaveXml(_XmlNode, XmlFilePath);
                this.Load();
                return;
            }

            _XmlNodeChildNodes = clsXmlControl.FindSencetion(_XmlNode, _NodeName);
            _XmlNodeChildNodesItem = clsXmlControl.FindSencetion(_XmlNode, _NodeNameItem);
            if (_XmlNodeChildNodes == null || _XmlNodeChildNodesItem==null)
            {

                #region 打包参数下载信息

                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeName, "Description", _NodeDescription);

                this.CreateDefaultData(ref _XmlNodeChildNodes);

                _XmlNode.AppendChild(_XmlNodeChildNodes);
                #endregion

                #region 打包参数下载项信息

                _XmlNodeChildNodesItem = clsXmlControl.CreateXmlNode(_NodeNameItem, "Description", _NodeDescriptionItem);

                this.CreateDefaultDataItem(ref _XmlNodeChildNodesItem);

                _XmlNode.AppendChild(_XmlNodeChildNodesItem);
                #endregion
                clsXmlControl.SaveXml(_XmlNode, XmlFilePath);
                this.Load();
                return;
            }
            m_Checked = Convert.ToBoolean(_XmlNodeChildNodes.ChildNodes[0].Attributes["Value"].Value);
            m_Para = _XmlNodeChildNodes.ChildNodes[2].Attributes["Value"].Value;
            string[] _Pata = m_Para.Split('|');
            txtNameParamDown.Text = _Pata[0];
            cmbProtcolParam.Text = _Pata[1];
            txtAddressParamSchema.Text = _Pata[2];
            txtProjectCount.Text = _Pata[3];        
        }


        /// <summary>
        /// 创建工单默认配置文件
        /// </summary>
        /// <param name="xml"></param>
        private void CreateDefaultData(ref XmlNode xml)
        {
             #region -----------打包参数下载-----------
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "IsCheck"
                    , "Name", "是否要检"
                    , "Value", "true"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "CustomerName"
                    , "Name", "项目名称"
                    , "Value", "打包参数下载信息"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "ProductsName"
                    , "Name", "项目参数"
                    , "Value", "|||"));//名称|协议|方案地址|项目总数

            #endregion

        }
        private void CreateDefaultDataItem(ref XmlNode xml)
        {
            //#region -----------打包参数下载项信息-----------
            //xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
            //        , "ID", "IsCheck"
            //        , "Name", "是否要检"
            //        , "Value", "true"));
            //xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
            //        , "ID", "CustomerName"
            //        , "Name", "项目名称"
            //        , "Value", "打包参数下载信息"));
            //xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
            //        , "ID", "ProductsName"
            //        , "Name", "项目参数"
            //        , "Value", "|||"));//名称|协议|方案地址|项目总数

            //#endregion

        }

        /// <summary>
        /// 保存参数到当前配置文件
        /// </summary>
        public void Save()
        {
            string _ErrorString = "";
            XmlNode _XmlNodeChildNodes;
            XmlNode _XmlNodeChildNodesItem;
            XmlNode _XmlNode = clsXmlControl.LoadXml(XmlFilePath, out _ErrorString);
            if (_ErrorString != "")
            {
                _XmlNode = clsXmlControl.CreateXmlNode("WorkPlan");

                #region 打包参数下载信息

                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeName, "Description", _NodeDescription);

                this.SaveCurrentData(ref _XmlNodeChildNodes);

                _XmlNode.AppendChild(_XmlNodeChildNodes);

                #endregion

                #region 打包参数下载项信息

                _XmlNodeChildNodesItem = clsXmlControl.CreateXmlNode(_NodeNameItem, "Description", _NodeDescriptionItem);

                this.SaveCurrentDataItem(ref _XmlNodeChildNodesItem);

                _XmlNode.AppendChild(_XmlNodeChildNodesItem);

                #endregion

                clsXmlControl.SaveXml(_XmlNode, XmlFilePath);
                //this.Load();
                return;
            }
            _XmlNodeChildNodes = clsXmlControl.FindSencetion(_XmlNode, _NodeName);
            _XmlNodeChildNodesItem = clsXmlControl.FindSencetion(_XmlNode, _NodeNameItem);
            if (_XmlNodeChildNodes == null ||_XmlNodeChildNodesItem == null)
            {

                #region 加载系统配置默认值

                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeName, "Description", _NodeDescription);

                this.SaveCurrentData(ref _XmlNodeChildNodes);

                _XmlNode.AppendChild(_XmlNodeChildNodes);
                #endregion

                #region 打包参数下载项信息

                _XmlNodeChildNodesItem = clsXmlControl.CreateXmlNode(_NodeNameItem, "Description", _NodeDescriptionItem);

                this.SaveCurrentDataItem(ref _XmlNodeChildNodesItem);

                _XmlNode.AppendChild(_XmlNodeChildNodesItem);

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

                #region 找到目录则修改XmlNodeItem

                if (File.Exists(txtAddressParam.Text)) 
                {

                    _XmlNode.RemoveChild(_XmlNodeChildNodesItem);
                    _XmlNodeChildNodesItem = clsXmlControl.CreateXmlNode(_NodeNameItem, "Description", _NodeDescriptionItem);

                    this.SaveCurrentDataItem(ref _XmlNodeChildNodesItem);


                    _XmlNode.AppendChild(_XmlNodeChildNodesItem);
                }
                #endregion
                clsXmlControl.SaveXml(_XmlNode, XmlFilePath);
                //this.Load();
                return;
            }
        }


        private void SaveCurrentData(ref XmlNode xml)
        {
            m_Para = txtNameParamDown.Text + "|";
            m_Para += cmbProtcolParam.Text + "|";
            m_Para += txtAddressParamSchema.Text + "|";
            m_Para += txtProjectCount.Text + "|"; 
            #region -----------打包参数下载-----------
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
                    , "Value", m_Para));//名称|协议|方案地址|项目总数

            #endregion
        }


        private void SaveCurrentDataItem(ref XmlNode xml)
        {
            try
            {
                for (int i = 0; i < Convert.ToInt32(txtProjectCount.Text); i++)
                {

                   
                    //string ParamDown = IniFile.IniReadValue("Commnunication", "Message" + (i + 1).ToString());
                    string ParamDown = pwClassLibrary.pwFile.File.ReadInIString(txtAddressParam.Text, "Commnunication", "Message" + (i + 1).ToString(), ""); 
                    int indexleft = ParamDown.IndexOf(">");
                    int indexright = ParamDown.IndexOf("<");
                    try
                    {
                        string TxFramestr = ParamDown.Substring(indexleft + 1, indexright - indexleft - 1);
                        string RxFramestr = ParamDown.Substring(indexright + 2);
                     //   xele2.Add(new XElement("Plan_DownPara_Item", new XAttribute("PrjID", i + 1), new XAttribute("TxFrame", TxFramestr), new XAttribute("RxFrame", RxFramestr)));
                        xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeNameItem
                        , "PrjID", (i+1).ToString()
                        , "TxFrame", TxFramestr
                        , "RxFrame", RxFramestr));
                    }
                    catch
                    {

                    }
                }
            }
            catch
            { 
            
            }
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.Filter = "Config Files|*.ini|txt Files|*.txt|All Files|*.*";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                txtAddressParam.Text = fileDialog.FileName;
                //IniFile.FilePath = fileDialog.FileName;
                try
                {
                    //string strAllMessage3 = IniFile.IniReadValue("Commnunication", "Message3");
                    string strAllMessage3 = pwClassLibrary.pwFile.File.ReadInIString(txtAddressParam.Text, "Commnunication", "Message3", ""); 
                    string[] AddressParamSchema = strAllMessage3.Substring(strAllMessage3.IndexOf("68") + 3, 17).Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    txtAddressParamSchema.Text = AddressParamSchema[5] + AddressParamSchema[4] + AddressParamSchema[3] + AddressParamSchema[2] + AddressParamSchema[1] + AddressParamSchema[0];
                    txtNameParamDown.Text = txtAddressParam.Text.Substring(txtAddressParam.Text.LastIndexOf("\\") + 1);
                    //txtProjectCount.Text = IniFile.IniReadValue("Commnunication", "NUM");
                    //cmbProtcolParam.Text = IniFile.IniReadValue("XieYi", "Type");
                    txtProjectCount.Text = pwClassLibrary.pwFile.File.ReadInIString(txtAddressParam.Text, "Commnunication", "NUM", "");
                    cmbProtcolParam.Text = pwClassLibrary.pwFile.File.ReadInIString(txtAddressParam.Text, "XieYi", "Type", "");


                    string _ErrorString = "";
                    XmlNode _XmlNode = clsXmlControl.LoadXml(XmlFilePath, out _ErrorString);
                    XmlNode _XmlNodeChildNodesItem = clsXmlControl.FindSencetion(_XmlNode, _NodeNameItem);
                    #region 找到目录则修改XmlNodeItem
                    if (_XmlNodeChildNodesItem != null)
                    {
                        _XmlNode.RemoveChild(_XmlNodeChildNodesItem);
                        _XmlNodeChildNodesItem = clsXmlControl.CreateXmlNode(_NodeNameItem, "Description", _NodeDescriptionItem);
                        this.SaveCurrentDataItem(ref _XmlNodeChildNodesItem);
                        _XmlNode.AppendChild(_XmlNodeChildNodesItem);
                        clsXmlControl.SaveXml(_XmlNode, XmlFilePath);
                    #endregion
                    }




                }
                catch
                {
                    txtAddressParamSchema.Text = txtNameParamDown.Text = txtProjectCount.Text = cmbProtcolParam.Text = string.Empty;
                }
            }
        }
    }
}
