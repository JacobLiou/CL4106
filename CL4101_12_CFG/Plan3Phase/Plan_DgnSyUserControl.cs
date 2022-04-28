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
    public partial class Plan_DgnSyUserControl : UserControl
    {
        private string XmlFilePath = Application.StartupPath + pwFunction.pwConst.Variable.CONST_WORKPLANPATH;
        private string _NodeName = "Plan_DgnSy";
        private string _NodeDescription = "日计时误差检定";
        //private string _NodeNameItem = "Plan_DgnSy_Item";
        //private string _NodeDescriptionItem = "多功能试验项信息";
        private bool m_Checked = false;//是否要检
        private string m_Para = "";//项目参数
        private string OutPramerter = "";
        private string PrjParm = "";

        public Plan_DgnSyUserControl()
        {
            InitializeComponent();
            Init();
            pw_Main.OnEventSchemaDataSave += new pwInterface.DelegateEventSchemaDataSave(EventSchemaDataSave);
            pw_Main.OnEventSchemaDataLoad += new pwInterface.DelegateEventSchemaDataLoad(EventSchemaDataLoad);
        }

        private void EventSchemaDataSave(int intItem, bool Checked, string _XmlFilePath)//
        {
            if (intItem != 3) return;
            XmlFilePath = _XmlFilePath;
            m_Checked = Checked;
            Save();
        }
        private void EventSchemaDataLoad(int intItem,  string _XmlFilePath)//
        {
            if (intItem != 3) return;
            XmlFilePath = _XmlFilePath;
            Load();
        }

        private void Init()
        {
            //cmbGLFXdgn.SelectedIndex = 0;
            //cmbGLYSdgn.SelectedIndex = 0;
            //cmbYJdgn.SelectedIndex = 0;
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

                #region 加载系统配置默认值
                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeName, "Description", _NodeDescription);

                this.CreateDefaultData(ref _XmlNodeChildNodes);

                _XmlNode.AppendChild(_XmlNodeChildNodes);

                #endregion

                #region 加载系统配置默认值Item
                //_XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeNameItem, "Description", _NodeDescriptionItem);

                //this.CreateDefaultDataItem(ref _XmlNodeChildNodes);

                //_XmlNode.AppendChild(_XmlNodeChildNodes);

                #endregion

                clsXmlControl.SaveXml(_XmlNode, XmlFilePath);
                this.Load();
                return;
            }

            _XmlNodeChildNodes = clsXmlControl.FindSencetion(_XmlNode, _NodeName);
           // _XmlNodeChildNodesItem = clsXmlControl.FindSencetion(_XmlNode, _NodeNameItem);
            if (_XmlNodeChildNodes == null )
            {

                #region 加载系统配置默认值

                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeName, "Description", _NodeDescription);

                this.CreateDefaultData(ref _XmlNodeChildNodes);

                _XmlNode.AppendChild(_XmlNodeChildNodes);
                #endregion

                //#region 加载系统配置默认值Item
                //_XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeNameItem, "Description", _NodeDescriptionItem);

                //this.CreateDefaultDataItem(ref _XmlNodeChildNodes);

                //_XmlNode.AppendChild(_XmlNodeChildNodes);

                //#endregion
                clsXmlControl.SaveXml(_XmlNode, XmlFilePath);
                this.Load();
                return;
            }


            m_Checked = Convert.ToBoolean(_XmlNodeChildNodes.ChildNodes[0].Attributes["Value"].Value);
            m_Para = _XmlNodeChildNodes.ChildNodes[2].Attributes["Value"].Value;

            string[] _Pata = m_Para.Split('|');
            txtParameterFuntion.Text = _Pata[0];//标准时钟脉冲常数
            txtTimePLFunction.Text = _Pata[1]; //时钟频率
            txtTurnNOFunction.Text = _Pata[2];//圈数


            //cmb_Xieyi.Text = _Pata[0];
            //txt_Code.Text = _Pata[1];
            //txt_Len.Text = _Pata[2];
            //txt_Dot.Text = _Pata[3];
            //txt_Data.Text = _Pata[4];
            //OutPramerter = _XmlNodeChildNodesItem.ChildNodes[0].Attributes["OutPramerter"].Value;
            //PrjParm = _XmlNodeChildNodesItem.ChildNodes[0].Attributes["PrjParm"].Value; 
            //string[] _OutPramerter= OutPramerter.Split('|');
            ////cmbGLFXdgn.Text = _OutPramerter[0];//功率方向
            ////cmbYJdgn.Text = _OutPramerter[1];//元件
            ////txtVmultiple.Text = _OutPramerter[2];//电压倍数
            ////txtImultiple.Text = _OutPramerter[3];//电流倍数
            ////cmbGLYSdgn.Text = _OutPramerter[4];//功率因素

            //string[] _PrjParm = PrjParm.Split('|');
            //txtParameterFuntion.Text = _PrjParm[0];//标准时钟脉冲常数
            //txtTimePLFunction.Text = _PrjParm[1]; //时钟频率
            //txtTurnNOFunction.Text = _PrjParm[2];//圈数
        }
        

        /// <summary>
        /// 多功能试验信息
        /// </summary>
        /// <param name="xml"></param>
        private void CreateDefaultData(ref XmlNode xml)
        {
            #region -----------多功能试验信息-----------
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
                    , "Value", "")); 

            #endregion

        }

        /// <summary>
        /// 多功能试验项信息
        /// </summary>
        /// <param name="xml"></param>
        private void CreateDefaultDataItem(ref XmlNode xml)
        {
            //#region -----------多功能试验项信息-----------
            //xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
            //        , "ID", "1"
            //        , "PrjName", "日计时误差"
            //        , "OutPramerter", "正向有功|合元|1|1|"
            //        , "PrjParm", "5000000|1|10"));
            //xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
            //        , "ID", "CustomerName"
            //        , "Name", "项目名称"
            //        , "Value", "多功能试验信息"));
            //xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
            //        , "ID", "ProductsName"
            //        , "Name", "项目参数"
            //        , "Value", "")); 

          //  #endregion

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

                #region 加载产品配置默认值

                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeName, "Description", _NodeDescription);

                this.SaveCurrentData(ref _XmlNodeChildNodes);

                _XmlNode.AppendChild(_XmlNodeChildNodes);

                #endregion

                #region 加载系统配置默认值Item
                //_XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeNameItem, "Description", _NodeDescriptionItem);

                //this.CreateDefaultDataItem(ref _XmlNodeChildNodes);

                //_XmlNode.AppendChild(_XmlNodeChildNodes);

                #endregion
                clsXmlControl.SaveXml(_XmlNode, XmlFilePath);
                //this.Load();
                return;
            }
            _XmlNodeChildNodes = clsXmlControl.FindSencetion(_XmlNode, _NodeName);
            //_XmlNodeChildNodesItem = clsXmlControl.FindSencetion(_XmlNode, _NodeNameItem);
            if (_XmlNodeChildNodes == null  )
            {

                #region 加载系统配置默认值

                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeName, "Description", _NodeDescription);

                this.CreateDefaultData(ref _XmlNodeChildNodes);

                _XmlNode.AppendChild(_XmlNodeChildNodes);
                #endregion

                #region 加载系统配置默认值Item
                //_XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeNameItem, "Description", _NodeDescriptionItem);

                //this.CreateDefaultDataItem(ref _XmlNodeChildNodes);

                //_XmlNode.AppendChild(_XmlNodeChildNodes);

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


                #region 找到目录则修改XmlNodeItem

                //_XmlNode.RemoveChild(_XmlNodeChildNodesItem);
                ////_XmlNodeChildNodes.RemoveAll();
                //_XmlNodeChildNodesItem = clsXmlControl.CreateXmlNode(_NodeNameItem, "Description", _NodeDescriptionItem);

                //this.SaveCurrentDataItem(ref _XmlNodeChildNodesItem);


                //_XmlNode.AppendChild(_XmlNodeChildNodesItem);
                #endregion

                clsXmlControl.SaveXml(_XmlNode, XmlFilePath);
                //this.Load();
                return;
            }

        }


        private void SaveCurrentDataItem(ref XmlNode xml)
        {
            //OutPramerter = cmbGLFXdgn.Text + "|";
            //OutPramerter += cmbYJdgn.Text + "|";
            //OutPramerter += txtVmultiple.Text + "|";
            //OutPramerter += txtImultiple.Text + "|";
            //OutPramerter += cmbGLYSdgn.Text + "|";

            //PrjParm = txtParameterFuntion.Text + "|";
            //PrjParm += txtTimePLFunction.Text + "|";
            //PrjParm += txtTurnNOFunction.Text + "|"; 

            //#region -----------加载页面配置的日计算误差值-----------
            //xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeNameItem
            //        , "ID", "1"
            //        , "PrjName", "日计时误差"
            //        , "OutPramerter", OutPramerter
            //        , "PrjParm", PrjParm));

            //#endregion
        }

        private void SaveCurrentData(ref XmlNode xml)
        {
            PrjParm = txtParameterFuntion.Text + "|";
            PrjParm += txtTimePLFunction.Text + "|";
            PrjParm += txtTurnNOFunction.Text + "|";

            #region -----------加载多功能试验信息-----------
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
                    , "Value", PrjParm));

            #endregion 
        } 
    }
}
