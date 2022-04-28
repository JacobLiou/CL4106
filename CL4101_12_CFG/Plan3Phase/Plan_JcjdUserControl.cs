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
    public partial class Plan_JcjdUserControl : UserControl
    {
        private string XmlFilePath = Application.StartupPath + pwFunction.pwConst.Variable.CONST_WORKPLANPATH;
        private string _NodeName = "Plan_ACSamplingTest";
        private string _NodeDescription = "交流采样测试";
        private bool m_Checked = false;//是否要检 
        //private string m_Para1A = "";//A相1
        //private string m_Para1B = "";//B相1
        //private string m_Para1C = "";//C相1
        //private string m_Para2A = "";//A相2
        //private string m_Para2B = "";//B相2
        //private string m_Para2C = "";//C相2
        //private string m_Para3A = "";//A相3
        //private string m_Para3B = "";//B相3
        //private string m_Para3C = "";//C相3
        //private string[] m_Para = new string[9];
        private string m_Para = "";
        private DataTable DTdgv = new DataTable();
        private DataTable DTdgvDataBase = new DataTable();
        public Plan_JcjdUserControl()
        {
            InitializeComponent();
           // Init();
           // InitDataTable();
            pw_Main.OnEventSchemaDataSave += new pwInterface.DelegateEventSchemaDataSave(EventSchemaDataSave);
            pw_Main.OnEventSchemaDataLoad += new pwInterface.DelegateEventSchemaDataLoad(EventSchemaDataLoad);
        }

        private void EventSchemaDataSave(int intItem, bool Checked, string _XmlFilePath)//
        {
            if (intItem != 5) return;
            XmlFilePath = _XmlFilePath;
            m_Checked = Checked;
            Save();
        }
        private void EventSchemaDataLoad(int intItem, string _XmlFilePath)//
        {
            if (intItem != 5) return;
            XmlFilePath = _XmlFilePath;
            Load();
        }

        private void Init()
        {
            //m_Para[0] = m_Para1A;
            //m_Para[1] = m_Para1B;
            //m_Para[2] = m_Para1C;
            //m_Para[3] = m_Para2A;
            //m_Para[4] = m_Para2B;
            //m_Para[5] = m_Para2C;
            //m_Para[6] = m_Para3A;
            //m_Para[7] = m_Para3B;
            //m_Para[8] = m_Para3C;  
        }

        public void InitDataTable()
        {
            if (DTdgv.Columns.Count == 0)
            {
                DTdgv.Columns.Add("AllPoint", System.Type.GetType("System.String"));
                DTdgv.Columns.Add("Un", System.Type.GetType("System.String"));
                DTdgv.Columns.Add("I", System.Type.GetType("System.String"));
                DTdgv.Columns.Add("GLYS", System.Type.GetType("System.String"));
                DTdgv.Columns.Add("UnU", System.Type.GetType("System.String"));
                DTdgv.Columns.Add("UnD", System.Type.GetType("System.String"));
                DTdgv.Columns.Add("IU", System.Type.GetType("System.String"));
                DTdgv.Columns.Add("ID", System.Type.GetType("System.String"));
                DTdgv.Columns.Add("PU", System.Type.GetType("System.String"));
                DTdgv.Columns.Add("PD", System.Type.GetType("System.String"));
            }
            #region 第1行
            DataRow dr1 = DTdgv.NewRow();
            dr1[0] = "1(A相)";
            dr1[1] = "120%Un";
            dr1[2] = "Imax";
            dr1[3] = "0.5L";
            dr1[4] = "0.4";
            dr1[5] = "-0.4";
            dr1[6] = "0.4";
            dr1[7] = "-0.4";
            dr1[8] = "0.4";
            dr1[9] = "-0.4";
            DTdgv.Rows.Add(dr1);
            #endregion

            #region 第2行
            DataRow dr2 = DTdgv.NewRow();
            dr2[0] = "1(B相)";
            dr2[1] = "Un";
            dr2[2] = "1.0Ib";
            dr2[3] = "0.5L";
            dr2[4] = "0.4";
            dr2[5] = "-0.4";
            dr2[6] = "0.4";
            dr2[7] = "-0.4";
            dr2[8] = "0.4";
            dr2[9] = "-0.4";
            DTdgv.Rows.Add(dr2);
            #endregion

            #region 第3行
            DataRow dr3 = DTdgv.NewRow();
            dr3[0] = "1(C相)";
            dr3[1] = "80%Un";
            dr3[2] = "0.1Ib";
            dr3[3] = "0.5L";
            dr3[4] = "0.4";
            dr3[5] = "-0.4";
            dr3[6] = "0.4";
            dr3[7] = "-0.4";
            dr3[8] = "0.4";
            dr3[9] = "-0.4";
            DTdgv.Rows.Add(dr3);
            #endregion

            #region 第4行(空格)
            //DataRow dr4 = DTdgv.NewRow();
            //dr4[0] = "";
            //dr4[1] = "";
            //dr4[2] = "";
            //dr4[3] = "";
            //dr4[4] = "";
            //dr4[5] = "";
            //dr4[6] = "";
            //dr4[7] = "";
            //dr4[8] = "";
            //dr4[9] = "";
            //DTdgv.Rows.Add(dr4);
            #endregion

            #region 第5行
            DataRow dr5 = DTdgv.NewRow();
            dr5[0] = "2(A相)";
            dr5[1] = "Un";
            dr5[2] = "1.0Ib";
            dr5[3] = "0.5L";
            dr5[4] = "0.4";
            dr5[5] = "-0.4";
            dr5[6] = "0.4";
            dr5[7] = "-0.4";
            dr5[8] = "0.4";
            dr5[9] = "-0.4";
            DTdgv.Rows.Add(dr5);
            #endregion

            #region 第6行
            DataRow dr6 = DTdgv.NewRow();
            dr6[0] = "2(B相)";
            dr6[1] = "80%Un";
            dr6[2] = "0.1Ib";
            dr6[3] = "0.5L";
            dr6[4] = "0.4";
            dr6[5] = "-0.4";
            dr6[6] = "0.4";
            dr6[7] = "-0.4";
            dr6[8] = "0.4";
            dr6[9] = "-0.4";
            DTdgv.Rows.Add(dr6);
            #endregion

            #region 第7行
            DataRow dr7 = DTdgv.NewRow();
            dr7[0] = "2(C相)";
            dr7[1] = "120%Un";
            dr7[2] = "Imax";
            dr7[3] = "0.5L";
            dr7[4] = "0.4";
            dr7[5] = "-0.4";
            dr7[6] = "0.4";
            dr7[7] = "-0.4";
            dr7[8] = "0.4";
            dr7[9] = "-0.4";
            DTdgv.Rows.Add(dr7);
            #endregion

            #region 第8行(空格)
            //DataRow dr8 = DTdgv.NewRow();
            //dr8[0] = "";
            //dr8[1] = "";
            //dr8[2] = "";
            //dr8[3] = "";
            //dr8[4] = "";
            //dr8[5] = "";
            //dr8[6] = "";
            //dr8[7] = "";
            //dr8[8] = "";
            //dr8[9] = "";
            //DTdgv.Rows.Add(dr8);
            #endregion

            #region 第9行
            DataRow dr9 = DTdgv.NewRow();
            dr9[0] = "3(A相)";
            dr9[1] = "80%Un";
            dr9[2] = "0.1Ib";
            dr9[3] = "0.5L";
            dr9[4] = "0.4";
            dr9[5] = "-0.4";
            dr9[6] = "0.4";
            dr9[7] = "-0.4";
            dr9[8] = "0.4";
            dr9[9] = "-0.4";
            DTdgv.Rows.Add(dr9);
            #endregion

            #region 第10行
            DataRow dr10 = DTdgv.NewRow();
            dr10[0] = "3(B相)";
            dr10[1] = "120%Un";
            dr10[2] = "Imax";
            dr10[3] = "0.5L";
            dr10[4] = "0.4";
            dr10[5] = "-0.4";
            dr10[6] = "0.4";
            dr10[7] = "-0.4";
            dr10[8] = "0.4";
            dr10[9] = "-0.4";
            DTdgv.Rows.Add(dr10);
            #endregion

            #region 第11行
            DataRow dr11 = DTdgv.NewRow();
            dr11[0] = "3(C相)";
            dr11[1] = "Un";
            dr11[2] = "1.0Ib";
            dr11[3] = "0.5L";
            dr11[4] = "0.4";
            dr11[5] = "-0.4";
            dr11[6] = "0.4";
            dr11[7] = "-0.4";
            dr11[8] = "0.4";
            dr11[9] = "-0.4";
            DTdgv.Rows.Add(dr11);
            #endregion
            dgvAllPoint.DataSource = DTdgv;
            //dgvAllPoint.Rows[2].Height = 60;
        }


        /// <summary>
        /// 读取XML文件初始化配置信息
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


            m_Checked = Convert.ToBoolean(_XmlNodeChildNodes.ChildNodes[0].Attributes["Value"].Value);
             m_Para = _XmlNodeChildNodes.ChildNodes[2].Attributes["Value"].Value;
             string[] m_ParamItem = m_Para.Split('|');
            txtV.Text = m_ParamItem[0];
            txtI.Text=m_ParamItem[1];
            txtP.Text=m_ParamItem[2];
            //m_Para[0] = "1(A相)|" + _XmlNodeChildNodes.ChildNodes[3].Attributes["ValueA"].Value;
            //m_Para[1] = "1(B相)|" + _XmlNodeChildNodes.ChildNodes[3].Attributes["ValueB"].Value;
            //m_Para[2] = "1(C相)|" + _XmlNodeChildNodes.ChildNodes[3].Attributes["ValueC"].Value;
            //m_Para[3] = "2(A相)|" + _XmlNodeChildNodes.ChildNodes[4].Attributes["ValueA"].Value;
            //m_Para[4] = "2(B相)|" + _XmlNodeChildNodes.ChildNodes[4].Attributes["ValueB"].Value;
            //m_Para[5] = "2(C相)|" + _XmlNodeChildNodes.ChildNodes[4].Attributes["ValueC"].Value;
            //m_Para[6] = "3(A相)|" + _XmlNodeChildNodes.ChildNodes[5].Attributes["ValueA"].Value;
            //m_Para[7] = "3(B相)|" + _XmlNodeChildNodes.ChildNodes[5].Attributes["ValueB"].Value;
            //m_Para[8] = "3(C相)|" + _XmlNodeChildNodes.ChildNodes[5].Attributes["ValueC"].Value;




            //if (DTdgvDataBase.Columns.Count == 0)
            //{
            //    DTdgvDataBase.Columns.Add("AllPoint", System.Type.GetType("System.String"));
            //    DTdgvDataBase.Columns.Add("Un", System.Type.GetType("System.String"));
            //    DTdgvDataBase.Columns.Add("I", System.Type.GetType("System.String"));
            //    DTdgvDataBase.Columns.Add("GLYS", System.Type.GetType("System.String"));
            //    DTdgvDataBase.Columns.Add("UnU", System.Type.GetType("System.String"));
            //    DTdgvDataBase.Columns.Add("UnD", System.Type.GetType("System.String"));
            //    DTdgvDataBase.Columns.Add("IU", System.Type.GetType("System.String"));
            //    DTdgvDataBase.Columns.Add("ID", System.Type.GetType("System.String"));
            //    DTdgvDataBase.Columns.Add("PU", System.Type.GetType("System.String"));
            //    DTdgvDataBase.Columns.Add("PD", System.Type.GetType("System.String"));
            //}
            //if (DTdgvDataBase.Rows.Count != 0)
            //{
            //    DTdgvDataBase.Rows.Clear();
            //}
            //for (int i = 0; i < 9; i++)
            //{
            //    string[] m_ParaCell = m_Para[i].Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            //    DataRow dr = DTdgvDataBase.NewRow();
            //    for (int j = 0; j < DTdgvDataBase.Columns.Count; j++)
            //    {
            //        dr[j] = m_ParaCell[j];
            //    }
            //    DTdgvDataBase.Rows.Add(dr);
            //}
            //dgvAllPoint.DataSource = DTdgvDataBase; 
        }


        /// <summary>
        /// 创建工单默认配置文件
        /// </summary>
        /// <param name="xml"></param>
        private void CreateDefaultData(ref XmlNode xml)
        {
            #region -----------交采精度检定-----------
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "IsCheck"
                    , "Name", "是否要检"
                    , "Value", "true"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "CustomerName"
                    , "Name", "项目名称"
                    , "Value", "交采精度检定"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "ProductsName"
                    , "Name", "项目参数"
                    , "Value", "0.4|0.4|0.4"));
            //xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
            //        , "ID", "Param1"
            //        , "ValueA", "120%Un|Imax|0.5L|0.4|-0.4|0.4|-0.4|0.4|-0.4"
            //        , "ValueB", "Un|1.0Ib|0.5L|0.4|-0.4|0.4|-0.4|0.4|-0.4"
            //        , "ValueC", "80%Un|0.1Ib|0.5L|0.4|-0.4|0.4|-0.4|0.4|-0.4"));
            //xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
            //        , "ID", "Param2"
            //        , "ValueA", "Un|1.0Ib|0.5L|0.4|-0.4|0.4|-0.4|0.4|-0.4"
            //        , "ValueB", "80%Un|0.1Ib|0.5L|0.4|-0.4|0.4|-0.4|0.4|-0.4"
            //        , "ValueC", "120%Un|Imax|0.5L|0.4|-0.4|0.4|-0.4|0.4|-0.4"));
            //xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
            //        , "ID", "Param3"
            //        , "ValueA", "80%Un|80%Un|0.5L|0.4|-0.4|0.4|-0.4|0.4|-0.4"
            //        , "ValueB", "120%Un|120%Un|0.5L|0.4|-0.4|0.4|-0.4|0.4|-0.4"
            //        , "ValueC", "Un|Un|0.5L|0.4|-0.4|0.4|-0.4|0.4|-0.4"));
            #endregion

 
        }

        /// <summary>
        /// 保存参数到当前配置文件
        /// </summary>
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

                #region 加载系统配置默认值

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
                //_XmlNodeChildNodes.RemoveAll();
                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeName, "Description", _NodeDescription);

                this.SaveCurrentData(ref _XmlNodeChildNodes);


                _XmlNode.AppendChild(_XmlNodeChildNodes);
                #endregion


                clsXmlControl.SaveXml(_XmlNode, XmlFilePath);
                //this.Load();
                return;
            }

        }


        private void SaveCurrentData(ref XmlNode xml)
        {
            m_Para = txtV.Text.Trim();
            m_Para += ("|" + txtI.Text.Trim());
            m_Para += ("|" + txtP.Text.Trim());
            //for (int i = 0; i < 9; i++)
            //{
            //    m_Para[i] = String.Empty;
            //}
            //for (int i = 0; i < m_Para.Length; i++)
            //{
            //    for (int j = 1; j < dgvAllPoint.Columns.Count; j++)
            //    {
            //        m_Para[i] += (dgvAllPoint.Rows[i].Cells[j].Value.ToString() + "|");
            //    }
            //}
            #region -----------交采检定检验-----------
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
                    , "Value", m_Para));
            //xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
            //         , "ID", "Param1"
            //         , "ValueA", m_Para[0]
            //         , "ValueB", m_Para[1]
            //         , "ValueC", m_Para[2]));
            //xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
            //         , "ID", "Param2"
            //        , "ValueA", m_Para[3]
            //        , "ValueB", m_Para[4]
            //        , "ValueC", m_Para[5]));
            //xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
            //         , "ID", "Param3"
            //        , "ValueA", m_Para[6]
            //        , "ValueB", m_Para[7]
            //        , "ValueC", m_Para[8]));
            #endregion
        }
    }
}
