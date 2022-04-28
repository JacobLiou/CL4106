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
    public partial class Plan_WcjdUserControl : UserControl
    {
        private string XmlFilePath = Application.StartupPath + pwFunction.pwConst.Variable.CONST_WORKPLANPATH;
        private string _NodeName = "Plan_Wcjd";
        private string _NodeDescription = "误差检定信息";
        private string _NodeNameItem = "Plan_Wcjd_Point";
        private string _NodeDescriptionItem = "误差检定点信息";
        private bool m_Checked = false;//是否要检
        private string m_Para = "";//项目参数

        string model = CParam.ModelName;//表型号
        string product = CParam.ProductName;//成品料号
        string faname = CParam.FanName;//方案名称
        string INow = "";
        string IHp = "";
        Double Imax = CParam.IMax;
        Double IBasic = CParam.IBasic;
        DataTable dtWC = new DataTable();
        public Plan_WcjdUserControl()
        {
            InitializeComponent();
            Init();
            pw_Main.OnEventSchemaDataSave += new pwInterface.DelegateEventSchemaDataSave(EventSchemaDataSave);
            pw_Main.OnEventSchemaDataLoad += new pwInterface.DelegateEventSchemaDataLoad(EventSchemaDataLoad); 
        }

        private void EventSchemaDataSave(int intItem, bool Checked, string _XmlFilePath)//
        {
            if (intItem != 2) return;
            XmlFilePath = _XmlFilePath;
            m_Checked = Checked;
            Save();
        }
        private void EventSchemaDataLoad(int intItem,  string _XmlFilePath)//
        {
            if (intItem != 2) return;
            XmlFilePath = _XmlFilePath;
            Load();
        }

        private void Init()
        {
            cmbGPBS.SelectedIndex = 3;
            cmbBLS.SelectedIndex = 7;
            cmbBLX.SelectedIndex = 7;
            cmbHL.SelectedIndex = 0;
            cmbGLFX.SelectedIndex = 0;
            cmbYJ.SelectedIndex = 0;
            cmbGLYS.SelectedIndex = 0;
            cmbDL.SelectedIndex = 0;
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

                #region 加载默认误差信息
                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeName, "Description", _NodeDescription);

                this.CreateDefaultData(ref _XmlNodeChildNodes);

                _XmlNode.AppendChild(_XmlNodeChildNodes);

                #endregion

                #region 加载默认误差点信息（没有选择任何点）
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

                #region 加载默认误差信息

                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeName, "Description", _NodeDescription);

                this.CreateDefaultData(ref _XmlNodeChildNodes);

                _XmlNode.AppendChild(_XmlNodeChildNodes);
                #endregion

                #region 加载默认误差点信息（没有选择任何点）
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
            //string[] _Pata = m_Para.Split('|');

            cmbGPBS.Text = m_Para;//高频检定倍数    

           // m_Para = _XmlNodeChildNodes.ChildNodes[2].Attributes["Value"].Value;

            //string WCPointString = _XmlNodeChildNodesItem.ToString();

            LoadDgv(ref _XmlNodeChildNodesItem);

        }

        private void LoadDgv(ref XmlNode xml)
        {
            object WCPoint;
            object TurnsNO;
            object WCUpperLimit;
            object WCLowerLimit;
            object HL;
            object GLFX;
            object YJ;
            object GLYS;
            object DL;
            object DLHp;
            model = CParam.ModelName;//表型号
            product = CParam.ProductName;//成品料号
            faname = CParam.FanName;//方案名称 
            string sql = @"DELETE FROM JDPointTempNew where Model='"
                     + model + "' and ProductName='"
                     + product + "' and SchemaName='"
                     + faname + "' and SchemaType='30' and SchemaArea='海外'";
            pw_Main.PUC.GetDataSetWithSQLString(sql);
            if (dtWC.Rows.Count != 0)
            {
                dtWC.Rows.Clear();
            }
            if (dtWC.Columns.Count == 0)
            {
                dtWC.Columns.Add("WCPoint", System.Type.GetType("System.String"));
                dtWC.Columns.Add("TurnsNO", System.Type.GetType("System.Int32"));
                dtWC.Columns.Add("WCUpperLimit", System.Type.GetType("System.Decimal"));
                dtWC.Columns.Add("WCLowerLimit", System.Type.GetType("System.Decimal"));
                dtWC.Columns.Add("HL", System.Type.GetType("System.String"));
                dtWC.Columns.Add("GLFX", System.Type.GetType("System.String"));
                dtWC.Columns.Add("YJ", System.Type.GetType("System.String"));
                dtWC.Columns.Add("GLYS", System.Type.GetType("System.String"));
                dtWC.Columns.Add("DL", System.Type.GetType("System.String"));
                dtWC.Columns.Add("DLHp", System.Type.GetType("System.String"));
            }
            for (int i = 0; i < 1000; i++)
            {
                try
                {
                    WCPoint = xml.ChildNodes[i].Attributes["PrjName"].Value;//检校点 
                    TurnsNO = xml.ChildNodes[i].Attributes["Qs"].Value;//圈数
                    WCUpperLimit = xml.ChildNodes[i].Attributes["fMax"].Value;//误差上限
                    WCLowerLimit = xml.ChildNodes[i].Attributes["fMin"].Value;//误差下限
                    HL = xml.ChildNodes[i].Attributes["DLHL"].Value;//回路 [0|1]
                    GLFX = xml.ChildNodes[i].Attributes["GLFX"].Value;//功率方向
                    YJ = xml.ChildNodes[i].Attributes["YJ"].Value;//元件
                    GLYS = xml.ChildNodes[i].Attributes["GLYS"].Value;//功率因素
                    DL = xml.ChildNodes[i].Attributes["xIb"].Value;//电流
                    string[] Point = WCPoint.ToString().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    DLHp = Point[4]; //电流隐藏
                    DataRow dr = dtWC.NewRow();
                    dr[0] = WCPoint;//检校点
                    dr[1] = TurnsNO;//圈数  
                    dr[2] = WCUpperLimit;//误差上限
                    dr[3] = WCLowerLimit;//误差下限 
                    dr[4] = (HL.ToString() == "0" ? "回路1" : "回路2");//回路
                    dr[5] = GLFX; //功率方向   
                    dr[6] = YJ;//元件
                    dr[7] = GLYS;//功率因素
                    dr[8] = DL;//电流
                    dr[9] = Point[4]; //电流隐藏
                    dtWC.Rows.Add(dr);
                    #region 将误差点保存到临时表中
                    sql = @"exec SaveJDPointTempNew @WCPoint='"
                    + WCPoint.ToString() + "',@TurnsNO='"
                    + TurnsNO.ToString() + "',@UpperLimitProportion='1',@WCUpperLimit='"
                    + WCUpperLimit.ToString() + "',@LowerLimitProportion='1',@WCLowerLimit='"
                    + WCLowerLimit.ToString() + "',@HL='"
                    + dr[4].ToString() + "',@GLFX='"
                    + GLFX.ToString() + "',@YJ ='"
                    + YJ.ToString() + "',@GLYS='"
                    + GLYS.ToString() + "',@DL='"
                    + DL.ToString() + "',@DLHp='"
                    + Point[4].ToString() + "',@HLIndex='"
                    + (HL.ToString() == "0" ? "A" : "B") + "',@GLFXIndex='"
                    + GetXHGLFX(GLFX.ToString()) + "',@YJIndex ='"
                    + GetXHYJ(YJ.ToString()) + "',@GLYSIndex='"
                    + GetXHGLYS(GLYS.ToString()) + "',@DLIndex='"
                    + GetXHDL(Point[4].ToString()) + "',@Model='"
                    + model + "',@ProductName='"
                    + product + "',@SchemaName='"
                    + faname + "',@SchemaType='30',@SchemaArea='海外'";
                    pw_Main.PUC.GetDataSetWithSQLString(sql);
                    #endregion
                }
                catch
                {
                    break;
                }
            }
            dgvPoint.DataSource = dtWC;
        }
        #region 通过值获取序号
        private string GetXHGLFX(string Value)
        {
            string ReSTR = "A";
            switch (Value)
            {
                case "正向有功":
                    ReSTR = "A";
                    break;
                case "反向有功":
                    ReSTR = "B";
                    break;
                case "正向无功":
                    ReSTR = "C";
                    break;
                case "反向无功":
                    ReSTR = "D";
                    break;
                default:
                    break;

            }
            return ReSTR;
        }
        private string GetXHYJ(string Value)
        {
            string ReSTR = "A";
            switch (Value)
            {
                case "合元":
                    ReSTR = "A";
                    break;
                case "A元":
                    ReSTR = "B";
                    break;
                case "B元":
                    ReSTR = "C";
                    break;
                case "C元":
                    ReSTR = "D";
                    break;
                default:
                    break;

            }
            return ReSTR;
        }
        private string GetXHGLYS(string Value)
        {
            string ReSTR = "A";
            switch (Value)
            {
                case "1.0":
                    ReSTR = "A";
                    break;
                case "0.5L":
                    ReSTR = "B";
                    break;
                case "0.8C":
                    ReSTR = "C";
                    break;
                case "0.5C":
                    ReSTR = "D";
                    break;
                case "0.25L":
                    ReSTR = "E";
                    break;
                default:
                    break;
            }
            return ReSTR;
        }
        private string GetXHDL(string Value)
        {
            string ReSTR = "A";
            switch (Value)
            {
                case "Imax":
                    ReSTR = "A";
                    break;
                case "0.5Imax":
                    ReSTR = "B";
                    break;
                case "3.0Ib":
                    ReSTR = "C";
                    break;
                case "2.0Ib":
                    ReSTR = "D";
                    break;
                case "(M-I)/2":
                    ReSTR = "E";
                    break;
                case "1.0Ib":
                    ReSTR = "F";
                    break;
                case "0.5Ib":
                    ReSTR = "G";
                    break;
                case "0.2Ib":
                    ReSTR = "H";
                    break;
                case "0.1Ib":
                    ReSTR = "I";
                    break;
                case "0.05Ib":
                    ReSTR = "J";
                    break;
                case "0.02Ib":
                    ReSTR = "K";
                    break;
                case "0.01Ib":
                    ReSTR = "L";
                    break;
                default:
                    break;
            }
            return ReSTR;
        }
        #endregion


        /// <summary>
        /// 创建工单默认配置文件
        /// </summary>
        /// <param name="xml"></param>
        private void CreateDefaultData(ref XmlNode xml)
        {
            #region -----------读生产编号配置-----------
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "IsCheck"
                    , "Name", "是否要检"
                    , "Value", "true"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "CustomerName"
                    , "Name", "项目名称"
                    , "Value", "误差检定信息"));
            xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
                    , "ID", "ProductsName"
                    , "Name", "项目参数"
                    , "Value", "4"));//高频检定倍数

            #endregion

        }

        /// <summary>
        /// 创建工单默认配置文件
        /// </summary>
        /// <param name="xml"></param>
        private void CreateDefaultDataItem(ref XmlNode xml)
        {
            //#region -----------读生产编号配置-----------
            //xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
            //        , "ID", "IsCheck"
            //        , "Name", "是否要检"
            //        , "Value", "true"));
            //xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
            //        , "ID", "CustomerName"
            //        , "Name", "项目名称"
            //        , "Value", "误差检定信息"));
            //xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeName
            //        , "ID", "ProductsName"
            //        , "Name", "项目参数"
            //        , "Value", "4"));//高频检定倍数

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

                #region 保存误差检定默认值

                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeName, "Description", _NodeDescription);

                this.SaveCurrentData(ref _XmlNodeChildNodes);

                _XmlNode.AppendChild(_XmlNodeChildNodes);

                #endregion

                #region 加载误差检定点信息

                _XmlNodeChildNodesItem = clsXmlControl.CreateXmlNode(_NodeNameItem, "Description", _NodeDescriptionItem);

                this.SaveCurrentDataItem(ref _XmlNodeChildNodes);

                _XmlNode.AppendChild(_XmlNodeChildNodes);

                #endregion

                clsXmlControl.SaveXml(_XmlNode, XmlFilePath);
                //this.Load();
                return;
            }
            _XmlNodeChildNodes = clsXmlControl.FindSencetion(_XmlNode, _NodeName);
            _XmlNodeChildNodesItem = clsXmlControl.FindSencetion(_XmlNode, _NodeNameItem);
            if (_XmlNodeChildNodes == null|| _XmlNodeChildNodes == null)
            {

                #region 加载误差信息

                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeName, "Description", _NodeDescription);

                this.SaveCurrentData(ref _XmlNodeChildNodes);

                _XmlNode.AppendChild(_XmlNodeChildNodes);
                #endregion 

                #region 加载系误差点信息

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
                //_XmlNodeChildNodes.RemoveAll();
                _XmlNodeChildNodes = clsXmlControl.CreateXmlNode(_NodeName, "Description", _NodeDescription);

                this.SaveCurrentData(ref _XmlNodeChildNodes);


                _XmlNode.AppendChild(_XmlNodeChildNodes);
                #endregion

                #region 找到目录则修改XmlNodeItem

                _XmlNode.RemoveChild(_XmlNodeChildNodesItem);
                //_XmlNodeChildNodes.RemoveAll();
                _XmlNodeChildNodesItem = clsXmlControl.CreateXmlNode(_NodeNameItem, "Description", _NodeDescriptionItem);

                this.SaveCurrentDataItem(ref _XmlNodeChildNodesItem);


                _XmlNode.AppendChild(_XmlNodeChildNodesItem);
                #endregion
                clsXmlControl.SaveXml(_XmlNode, XmlFilePath);
                //this.Load();
                return;
            }

        }


        private void SaveCurrentData(ref XmlNode xml)
        {
            ////////m_Para = cmb_Xieyi.Text + "|";
            ////////m_Para += txt_Code.Text + "|";
            ////////m_Para += txt_Len.Text + "|";
            ////////m_Para += txt_Dot.Text + "|";
            ////////m_Para += txt_Data.Text + "|";
            m_Para = cmbGPBS.Text.Trim();

            #region -----------误差检定信息-----------
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
                    , "Value", m_Para));//协议|标识编码|长度|小数点|下发参数"DLT645_1997|FFF9|6|0||"

            #endregion
        }

        private void SaveCurrentDataItem(ref XmlNode xml)
        { 
            DataTable PointAllDT = (DataTable)dgvPoint.DataSource;
            for (int i = 0; i < dgvPoint.Rows.Count; i++)
            {
                xml.AppendChild(clsXmlControl.CreateXmlNode(_NodeNameItem
                       , "ID", (i + 1).ToString()
                       , "PrjName", PointAllDT.Rows[i]["WCPoint"].ToString()
                       , "DLHL", PointAllDT.Rows[i]["HL"].ToString() == "回路1" ? "0" : "1"
                       , "GLFX", PointAllDT.Rows[i]["GLFX"].ToString()
                       , "YJ", PointAllDT.Rows[i]["YJ"].ToString()
                       , "GLYS", PointAllDT.Rows[i]["GLYS"].ToString()
                       , "xIb", PointAllDT.Rows[i]["DL"].ToString()
                       , "Qs", PointAllDT.Rows[i]["TurnsNO"].ToString()
                       , "fMax", PointAllDT.Rows[i]["WCUpperLimit"].ToString()
                       , "fMin", PointAllDT.Rows[i]["WCLowerLimit"].ToString()
                       , "PC", "false"));
            }
        }
        

        private void btnAddPoint_Click(object sender, EventArgs e)
        {
            model = CParam.ModelName;//表型号
            product = CParam.ProductName;//成品料号
            faname = CParam.FanName;//方案名称
            Imax = CParam.IMax;
            IBasic = CParam.IBasic;
            IHp = cmbDL.Text.Trim();
            if (model == "")
            {
                MessageBox.Show("请选择表型号！", "提示");
                return;
            }
            if (product == "")
            {
                MessageBox.Show("请选择成品料号！", "提示");
                return;
            }
            if (faname == null||faname =="")
            {
                MessageBox.Show("请填写方案名称！", "提示");
                return;
            }
            if (!IsNumber(Imax.ToString()))
            {
                MessageBox.Show("请确保产品信息中填入正确的电流值！", "提示");
                return;
            }
            if (!IsNumber(IBasic.ToString()))
            {
                MessageBox.Show("请确保产品信息中填入正确的电流值！", "提示");
                return;
            }
            if (cmbHL.SelectedIndex == -1)
            {
                MessageBox.Show("请选择回路！", "提示");
                return;
            }
            if (cmbGLFX.SelectedIndex == -1)
            {
                MessageBox.Show("请选择功率方向！", "提示");
                return;
            }
            if (cmbYJ.SelectedIndex == -1)
            {
                MessageBox.Show("请选择元件！", "提示");
                return;
            }
            if (cmbGLYS.SelectedIndex == -1)
            {
                MessageBox.Show("请选择功率因素！", "提示");
                return;
            }
            if (cmbDL.SelectedIndex == -1)
            {
                MessageBox.Show("请选择电流！", "提示");
                return;
            }
           
            if (IHp.Contains("Imax"))
            {
                if (IHp.Contains("0.5Imax"))
                {
                    INow = Convert.ToString(Imax / (2 * IBasic)) + "Ib";
                }
                else
                {
                    INow = Convert.ToString(Imax / IBasic) + "Ib";
                }
            }
            else if (IHp.Contains("(M-I)/2"))
            {
                INow = Convert.ToString((Imax - IBasic) / 2) + "Ib";
            }
            else
            {
                INow = IHp;
            }
            string sql = @"SELECT * FROM JDPointTempNew where Model='"
                + model + "' and ProductName='"
                + product + "' and SchemaName='"
                + faname + "' and SchemaType='30' and SchemaArea='海外' and HL='"
                + cmbHL.Text.Trim() + "' and GLFX='"
                + cmbGLFX.Text.Trim() + "' and YJ='"
                + cmbYJ.Text.Trim() + "' and GLYS='"
                + cmbGLYS.Text.Trim() + "' and DLHp='"
                + cmbDL.Text.Trim() + "' and HLIndex='"
                + GetZM(cmbHL.SelectedIndex) + "' and GLFXIndex='"
                + GetZM(cmbGLFX.SelectedIndex) + "' and YJIndex='"
                + GetZM(cmbYJ.SelectedIndex) + "' and GLYSIndex='"
                + GetZM(cmbGLYS.SelectedIndex) + "' and DLIndex='"
                + GetZM(cmbDL.SelectedIndex) + "'";
            DataTable PointDT = pw_Main.PUC.GetDataSetWithSQLString(sql).Tables[0];
            if (PointDT.Rows.Count != 0)
            {
                MessageBox.Show("该误差点已经存在！", "提示");
                return;
            }
            string glfx = "P+";
            switch (cmbGLFX.Text.Trim())
            { 
                case "正向有功":
                    glfx = "P+";
                    break;
                case "反向有功":
                    glfx = "P-";
                    break;
                case "正向无功":
                    glfx = "Q+";
                    break;
                case "反向无功":
                    glfx = "Q-";
                    break;
                default:
                    break;
            }
            string WCPoint = cmbHL.Text.Trim() + " " + glfx + " " + cmbYJ.Text.Trim() + " " + cmbGLYS.Text.Trim() + " " + cmbDL.Text.Trim() + " 基本误差";
            //误差点组成方式【回路】【功率方向】【元件】【功率因素】【电流】
            sql = @"exec SaveJDPointTempNew @WCPoint='"
                + WCPoint + "',@TurnsNO='1',@UpperLimitProportion='1',@WCUpperLimit='0.25',@LowerLimitProportion='1',@WCLowerLimit='-0.25',@HL='"
                + cmbHL.Text.Trim() + "',@GLFX='"
                + cmbGLFX.Text.Trim() + "',@YJ ='"
                + cmbYJ.Text.Trim() + "',@GLYS='"
                + cmbGLYS.Text.Trim() + "',@DL='"
                + INow + "',@DLHp='"
                + cmbDL.Text.Trim() + "',@HLIndex='"
                + GetZM(cmbHL.SelectedIndex) + "',@GLFXIndex='"
                + GetZM(cmbGLFX.SelectedIndex) + "',@YJIndex ='"
                + GetZM(cmbYJ.SelectedIndex) + "',@GLYSIndex='"
                + GetZM(cmbGLYS.SelectedIndex) + "',@DLIndex='"
                + GetZM(cmbDL.SelectedIndex) + "',@Model='"
                + model + "',@ProductName='"
                + product + "',@SchemaName='"
                + faname + "',@SchemaType='30',@SchemaArea='海外'";
            pw_Main.PUC.GetDataSetWithSQLString(sql);
            sql = @"SELECT WCPoint,TurnsNO,WCUpperLimit,WCLowerLimit,HL,GLFX,YJ,GLYS,DL,DLHp FROM JDPointTempNew where Model='"
                + model + "' and ProductName='"
                + product + "' and SchemaName='"
                + faname + "' and SchemaType='30' and SchemaArea='海外' order by HLIndex,GLFXIndex,YJIndex,DLIndex,GLYSIndex";
            DataTable PointAllDT = pw_Main.PUC.GetDataSetWithSQLString(sql).Tables[0];
            dgvPoint.DataSource = PointAllDT;
            //排序依次按照【回路】【功率方向】【元件】【电流】【功率因素】
        }

        private void btnDeletePoint_Click(object sender, EventArgs e)
        {
            model = CParam.ModelName;//表型号
            product = CParam.ProductName;//成品料号
            faname = CParam.FanName;//方案名称 
            for (int i = 0; i < dgvPoint.Rows.Count; i++)
            {
                if (dgvPoint.Rows[i].Cells[0].Selected == true)
                {
                    string sql = @"DELETE FROM JDPointTempNew where Model='"
                     + model + "' and ProductName='"
                     + product + "' and SchemaName='"
                     + faname+ "' and SchemaType='30' and SchemaArea='海外' and HL='"
                     + dgvPoint.Rows[i].Cells[4].Value.ToString() + "' and GLFX='"
                     + dgvPoint.Rows[i].Cells[5].Value.ToString() + "' and YJ='"
                     + dgvPoint.Rows[i].Cells[6].Value.ToString() + "' and GLYS='"
                     + dgvPoint.Rows[i].Cells[7].Value.ToString() + "' and DLHp='"
                     + dgvPoint.Rows[i].Cells[9].Value.ToString() + "'";
                    pw_Main.PUC.GetDataSetWithSQLString(sql);
                    sql = @"SELECT WCPoint,TurnsNO,WCUpperLimit,WCLowerLimit,HL,GLFX,YJ,GLYS,DL,DLHp FROM JDPointTempNew where Model='"
               + model + "' and ProductName='"
               + product + "' and SchemaName='"
               + faname + "' and SchemaType='30' and SchemaArea='海外' order by HLIndex,GLFXIndex,YJIndex,DLIndex,GLYSIndex";
                    DataTable PointAllDT = pw_Main.PUC.GetDataSetWithSQLString(sql).Tables[0];
                    dgvPoint.DataSource = PointAllDT;
                    return;
                }
            }
        }
        #region 判断是否为数字
        public bool IsNumber(string strNum)
        {
            bool flag = true;
            int flgcount = 0;
            string NumAll = "0123456789.";
            for (int i = 0; i < strNum.Length; i++)
            {
                if (strNum[i].ToString() == ".")
                {
                    flgcount++;
                }
                if (!NumAll.Contains(strNum[i].ToString()))
                {
                    flag = false;
                    break;
                }
            }
            if (strNum == "")
            {
                flag = false;
            }
            if (flgcount > 1)
            {
                flag = false;
            }
            return flag;
        }
        #endregion

        private string GetZM(int x)
        {
            char Startchar = 'A';
            int Startint = Convert.ToInt32(Startchar);
            char EndAchar = Convert.ToChar(Startint + x);
            return Convert.ToString(EndAchar);
        }

        private void cmbBLS_SelectedIndexChanged(object sender, EventArgs e)
        {
            double bls = 1;
            if (cmbBLS.Text.Trim().Contains("/") == true)
            {
                try
                {
                    bls = Convert.ToDouble(cmbBLS.Text.Trim().Substring(0, 1)) / Convert.ToDouble(cmbBLS.Text.Trim().Substring(2));
                }
                catch
                {

                }
            }
            else
            {
                bls = Convert.ToDouble(cmbBLS.Text.Trim());
            }

            for (int i = 0; i < dgvPoint.Rows.Count; i++)
            {
                try
                {
                    dgvPoint.Rows[i].Cells[2].Value = (object)(bls * Convert.ToDouble(dgvPoint.Rows[i].Cells[2].Value.ToString()));
                }
                catch
                {

                }
            }
        }

        private void cmbBLX_SelectedIndexChanged(object sender, EventArgs e)
        {
            double blx = 1;
            if (cmbBLX.Text.Trim().Contains("/") == true)
            {
                try
                {
                    blx = Convert.ToDouble(cmbBLX.Text.Trim().Substring(0, 1)) / Convert.ToDouble(cmbBLX.Text.Trim().Substring(2));
                }
                catch
                {

                }
            }
            else
            {
                blx = Convert.ToDouble(cmbBLX.Text.Trim());
            }
            for (int i = 0; i < dgvPoint.Rows.Count; i++)
            {
                try
                {
                    dgvPoint.Rows[i].Cells[3].Value = (object)(blx * Convert.ToDouble(dgvPoint.Rows[i].Cells[3].Value.ToString()));
                }
                catch
                {

                }
            }
        }
    }
}
