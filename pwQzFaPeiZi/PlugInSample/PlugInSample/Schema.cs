using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;
using System.Xml.Linq;
using System.IO;



namespace CL4101_QZ_GW
{

    public class XMLFileSave
    {

      
         
        #region 数据库链接
        public string DatabaseServer1;
        public string DatabaseName1;
        public string DatabasePassword1;
        public string DatabaseUser1;
        #endregion
        #region 工单与产品信息
        public string SchemaDescription;//方案描述
        public string SchemaNameNew;//方案名称
        public string ProductName;//成品料号
        public string Model;
        public string ModelDiscription;
        public string TestType;
        public string BasicV;//基本电压
        public string BasicI;//基本电流
        public string MaxI;//最大电流
        public string Const;//常数
        public string Level;//等级
        public string HZ;//频率
        public string DuanZi;//端子类型
        public string gongyinggongyang;//共阴共阳
        public string MaiChong;//脉冲类型
        public string JiDianQi;//继电器类型
        public string RevSoftNo;//软件版本号
        #endregion

        #region 读电能量
        public string valueddny;//是否要检
        public string ProjectNameReadPower;//项目名称
        public string ProcolReadPower;//协议
        public string IdentityNOReadPower;//标识编码  
        public string DataLengthReadPower;//数据长度
        public string PointReadPower;//小数点
        public string SendNumberReadPower;//下发数 
       
        #endregion

        #region 读生产编号信息
        public string valuedscbh;//是否要检
        public string ProjectNameProductNumber;//项目名称
        public string ProductNOParamXY;//协议
        public string IdentityNO;//标识编码  
        public string DataLengthReadNO;//数据长度
        public string PointReadNO;//小数点
        public string SendNumberReadNO;//下发数 
        #endregion

        #region 误差校准
        public string valuewcjz;//是否要检
        public string wcjzProjectName;//项目名称
        public string ProcolWCJZ;//协议
        public string IdentityWCJZ;//标识编码
        public string wcjzfangfa;//校准方法
        public string wcjzdzxh;//端子型号  
        public string wcjzbzxdl;//标准小电流       
        public string wcjzinifile;//标准ini文件 
        public string wcjzbz;//步骤
        #region 误差校准参数 18个
        public string Param1;
        public string Param2;
        public string Param3;
        public string Param4;
        public string Param5;
        public string Param6;
        public string Param7;
        public string Param8;
        public string Param9;
        public string Param10;
        public string Param11;
        public string Param12;
        public string Param13;
        public string Param14;
        public string Param15;
        public string Param16;
        public string Param17;
        public string Param18;
        public string ParamName1;
        public string ParamName2;
        public string ParamName3;
        public string ParamName4;
        public string ParamName5;
        public string ParamName6;
        public string ParamName7;
        public string ParamName8;
        public string ParamName9;
        public string ParamName10;
        public string ParamName11;
        public string ParamName12;
        public string ParamName13;
        public string ParamName14;
        public string ParamName15;
        public string ParamName16;
        public string ParamName17;
        public string ParamName18;
        #endregion
        #endregion

        #region 时钟校准
        public string valueszjz;//是否要检
        public string szjzProjectName;//项目名称      
        public string TimeCheckProcol;//协议
        public string TimeIdentity;//标识编码
        public string TimeDataLength;//数据长度
        public string TimePoint;//小数点
        public string TimeSend;//下发数 
        #endregion

        #region RS485读版本号
        public string valuehw;//是否要检
        public string hwProjectName;//项目名称      
        public string hwProcol;//协议
        public string hwIdentity;//标识编码
        public string hwDataLength;//数据长度
        public string hwPoint;//小数点
        public string hwSend;//下发数 
        #endregion

        #region 读整机功耗
        public string valuegh;//是否要检ring 
        public string ghProjectName;//功耗
        public string ghPmin;//功率最小值      
        public string ghPmax;//功耗最大值
        #endregion

        #region 整机自检
        private string Valuezj;//是否要检
        private string ZjProjectName;//项目名称
        private string m1Bit0;//Bit0;0=单板测试  1=整机测试;     
        private string m1Bit1;//Bit1 0=三相四 1=三相三   ；5
        private string m1Bit;//M1Bit0+M1Bit1
        private string m2;//0：外控磁保持  1：内控磁保持   2：外控电保持 ；
        private string FourBit;//测试需判断项4个字节
        /// <summary>
        /// 是否要检
        /// </summary>
        public string valuezj
        {
            get
            {
                return Valuezj;
            }
            set
            {
                Valuezj = value;
            }
        }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string zjProjectName
        {
            get
            {
                return ZjProjectName;
            }
            set
            {
                ZjProjectName = value;
            }
        }

        /// <summary>
        /// Bit0;0=单板测试  1=整机测试;
        /// </summary>
        public string M1Bit0
        {
            get
            {
                return m1Bit0;
            }
            set
            {
                m1Bit0 = value;
            }
        }

        /// <summary>
        /// Bit1 0=三相四 1=三相三
        /// </summary>
        public string M1Bit1
        {
            get
            {
                return m1Bit1;
            }
            set
            {
                m1Bit1 = value;
            }
        }

        /// <summary>
        /// M1Bit0+M1Bit1
        /// </summary>
        public string M1Bit
        {
            get
            {
                return m1Bit;
            }
            set
            {
                m1Bit = value;
            }
        }
        /// <summary>
        /// 0：外控磁保持  1：内控磁保持   2：外控电保持 
        /// </summary>
        public string M2
        {
            get
            {
                return m2;
            }
            set
            {
                m2 = value;
            }
        }

        /// <summary>
        /// 测试需判断项4个字节
        /// </summary>
        public string fourBit
        {
            get
            {
                return FourBit;
            }
            set
            {
                FourBit = value;
            }
        }
        #endregion

        #region 初始化参数
        public string valueIniParam;//是否要检
        public string ProjectNameIniParam;//项目名称
        public string ProcolIniParam;//协议
        public string IdentityNOIniParam;//标识编码  
        public string DataLengthIniParam;//数据长度
        public string PointIniParam;//小数点
        public string SendNumberIniParam;//下发数 
        #endregion

        #region 保存XML文档
        public void Save(string path,  string OrBitUserName)
        {
            try
            {
                XDocument document = new XDocument(new XElement("WorkPlan",
                new XElement("Work", new XAttribute("Description", "工单信息"),
                   new XElement("Work", new XAttribute("ID", "WorkSN"), new XAttribute("Name", "工单号"), new XAttribute("Value", "")),
                   new XElement("Work", new XAttribute("ID", "CustomerName"), new XAttribute("Name", "客户名称"), new XAttribute("Value", "")),
                   new XElement("Work", new XAttribute("ID", "ProductsName"), new XAttribute("Name", "产品名称"), new XAttribute("Value", "")),
                   new XElement("Work", new XAttribute("ID", "ProductsSN"), new XAttribute("Name", "产品编号"), new XAttribute("Value", "")),
                   new XElement("Work", new XAttribute("ID", "ProductsModel"), new XAttribute("Name", "产品型号"), new XAttribute("Value", Model))),

                new XElement("Products", new XAttribute("Description", "产品信息"),
                    //new XElement("Products", new XAttribute("ID", "Model"), new XAttribute("Name", "表型号"), new XAttribute("Value", Model)),
                    //new XElement("Products", new XAttribute("ID", "ModelDiscription"), new XAttribute("Name", "型号描述"), new XAttribute("Value", ModelDiscription)),
                   new XElement("Products", new XAttribute("ID", "Clfs"), new XAttribute("Name", "测量方式"), new XAttribute("Value", TestType)),
                   new XElement("Products", new XAttribute("ID", "Ub"), new XAttribute("Name", "电压"), new XAttribute("Value", BasicV + "V")),
                   new XElement("Products", new XAttribute("ID", "Ib"), new XAttribute("Name", "电流"), new XAttribute("Value", BasicI + "A")),
                   new XElement("Products", new XAttribute("ID", "IMax"), new XAttribute("Name", "电流"), new XAttribute("Value", MaxI + "A")),
                   new XElement("Products", new XAttribute("ID", "Constant"), new XAttribute("Name", "常数"), new XAttribute("Value", Const)),
                   new XElement("Products", new XAttribute("ID", "DJ"), new XAttribute("Name", "等级"), new XAttribute("Value", Level)),
                   new XElement("Products", new XAttribute("ID", "PL"), new XAttribute("Name", "频率"), new XAttribute("Value", HZ)),
                   new XElement("Products", new XAttribute("ID", "DzType"), new XAttribute("Name", "端子类型"), new XAttribute("Value", DuanZi)),
                    new XElement("Products", new XAttribute("ID", "GYGY"), new XAttribute("Name", "共阴共阳"), new XAttribute("Value", gongyinggongyang)),
                   new XElement("Products", new XAttribute("ID", "PulseType"), new XAttribute("Name", "脉冲类型"), new XAttribute("Value", MaiChong)),
                   new XElement("Products", new XAttribute("ID", "JDQType"), new XAttribute("Name", "继电器类型"), new XAttribute("Value", JiDianQi)),
                   new XElement("Products", new XAttribute("ID", "SoftVer"), new XAttribute("Name", "软件版本号"), new XAttribute("Value", RevSoftNo))),

               new XElement("Plan_ReadEnergy", new XAttribute("Description", "读电能量"),
                   new XElement("Plan_ReadEnergy", new XAttribute("ID", "IsCheck"), new XAttribute("Name", "是否要检"), new XAttribute("Value", valueddny)),
                   new XElement("Plan_ReadEnergy", new XAttribute("ID", "CustomerName"), new XAttribute("Name", "项目名称"), new XAttribute("Value", ProjectNameReadPower)),
                   new XElement("Plan_ReadEnergy", new XAttribute("ID", "ProductsName"), new XAttribute("Name", "项目参数"), new XAttribute("Value", ProcolReadPower + "|" + IdentityNOReadPower + "|" + DataLengthReadPower + "|" + PointReadPower + "|" + SendNumberReadPower))),

               new XElement("Plan_ReadScbh", new XAttribute("Description", "读生产编号信息"),
                   new XElement("Plan_ReadScbh", new XAttribute("ID", "IsCheck"), new XAttribute("Name", "是否要检"), new XAttribute("Value", valuedscbh)),
                   new XElement("Plan_ReadScbh", new XAttribute("ID", "CustomerName"), new XAttribute("Name", "项目名称"), new XAttribute("Value", ProjectNameProductNumber)),
                   new XElement("Plan_ReadScbh", new XAttribute("ID", "ProductsName"), new XAttribute("Name", "项目参数"), new XAttribute("Value", ProductNOParamXY + "|" + IdentityNO + "|" + DataLengthReadNO + "|" + PointReadNO + "|" + SendNumberReadNO))),

               new XElement("Plan_AdjustError", new XAttribute("Description", "校准误差"),
                   new XElement("Plan_AdjustError", new XAttribute("ID", "IsCheck"), new XAttribute("Name", "是否要检"), new XAttribute("Value", valuewcjz)),
                   new XElement("Plan_AdjustError", new XAttribute("ID", "CustomerName"), new XAttribute("Name", "项目名称"), new XAttribute("Value", wcjzProjectName)),
                   new XElement("Plan_AdjustError", new XAttribute("ID", "ProductsName"), new XAttribute("Name", "项目参数"), new XAttribute("Value", ProcolWCJZ + "|" + IdentityWCJZ + "|" + wcjzfangfa + "|" + wcjzbzxdl + "|" + wcjzbz))),

               new XElement("Plan_AdjustClock", new XAttribute("Description", "校准时钟"),
                    new XElement("Plan_AdjustClock", new XAttribute("ID", "IsCheck"), new XAttribute("Name", "是否要检"), new XAttribute("Value", valueszjz)),
                    new XElement("Plan_AdjustClock", new XAttribute("ID", "CustomerName"), new XAttribute("Name", "项目名称"), new XAttribute("Value", szjzProjectName)),
                    new XElement("Plan_AdjustClock", new XAttribute("ID", "ProductsName"), new XAttribute("Name", "项目参数"), new XAttribute("Value", TimeCheckProcol + "|" + TimeIdentity + "|" + TimeDataLength + "|" + TimePoint + "|" + TimeSend))),

                new XElement("Plan_ReadVer", new XAttribute("Description", "RS485读版本号"),
                    new XElement("Plan_ReadVer", new XAttribute("ID", "IsCheck"), new XAttribute("Name", "是否要检"), new XAttribute("Value", valuehw)),
                    new XElement("Plan_ReadVer", new XAttribute("ID", "CustomerName"), new XAttribute("Name", "项目名称"), new XAttribute("Value", hwProjectName)),
                    new XElement("Plan_ReadVer", new XAttribute("ID", "ProductsName"), new XAttribute("Name", "项目参数"), new XAttribute("Value", hwProcol + "|" + hwIdentity + "|" + hwDataLength + "|" + hwPoint + "|" + hwSend))),

                new XElement("Plan_ReadPower", new XAttribute("Description", "读整机功耗"),
                    new XElement("Plan_ReadPower", new XAttribute("ID", "IsCheck"), new XAttribute("Name", "是否要检"), new XAttribute("Value", valuegh)),
                    new XElement("Plan_ReadPower", new XAttribute("ID", "CustomerName"), new XAttribute("Name", "项目名称"), new XAttribute("Value", ghProjectName)),
                    new XElement("Plan_ReadPower", new XAttribute("ID", "ProductsName"), new XAttribute("Name", "项目参数"), new XAttribute("Value", ghPmin + "|" + ghPmax))),
                new XElement("Plan_SelfCheck", new XAttribute("Description", "整机自检"),
                    new XElement("Plan_SelfCheck", new XAttribute("ID", "IsCheck"), new XAttribute("Name", "是否要检"), new XAttribute("Value", valuezj)),
                    new XElement("Plan_SelfCheck", new XAttribute("ID", "CustomerName"), new XAttribute("Name", "项目名称"), new XAttribute("Value", zjProjectName)),
                    new XElement("Plan_SelfCheck", new XAttribute("ID", "ProductsName"), new XAttribute("Name", "项目参数"), new XAttribute("Value", M1Bit + "|" + M2 + "|" + fourBit))),
              new XElement("Plan_AdjustErrorPara_Item", new XAttribute("Description", "校准误差参数文件")),

               new XElement("Plan_InitPara", new XAttribute("Description", "初始化参数"),
                   new XElement("Plan_InitPara", new XAttribute("ID", "IsCheck"), new XAttribute("Name", "是否要检"), new XAttribute("Value", valueIniParam)),
                   new XElement("Plan_InitPara", new XAttribute("ID", "CustomerName"), new XAttribute("Name", "项目名称"), new XAttribute("Value", ProjectNameIniParam)),
                   new XElement("Plan_InitPara", new XAttribute("ID", "ProductsName"), new XAttribute("Name", "项目参数"), new XAttribute("Value", ProcolIniParam + "|" + IdentityNOIniParam + "|" + DataLengthIniParam + "|" + PointIniParam + "|" + SendNumberIniParam))))); 

                #region 误差校准信息
                if (valuewcjz == "true")
                {
                    XElement xele2 = document.Element("WorkPlan").Element("Plan_AdjustErrorPara_Item");
                    xele2.Add(new XElement("Plan_AdjustErrorPara_Item", new XAttribute("ParaID", "1"), new XAttribute("ParaName", ParamName1), new XAttribute("ParaValue", Param1)));
                    xele2.Add(new XElement("Plan_AdjustErrorPara_Item", new XAttribute("ParaID", "2"), new XAttribute("ParaName", ParamName2), new XAttribute("ParaValue", Param2)));
                    xele2.Add(new XElement("Plan_AdjustErrorPara_Item", new XAttribute("ParaID", "3"), new XAttribute("ParaName", ParamName3), new XAttribute("ParaValue", Param3)));
                    xele2.Add(new XElement("Plan_AdjustErrorPara_Item", new XAttribute("ParaID", "4"), new XAttribute("ParaName", ParamName4), new XAttribute("ParaValue", Param4)));
                    xele2.Add(new XElement("Plan_AdjustErrorPara_Item", new XAttribute("ParaID", "5"), new XAttribute("ParaName", ParamName5), new XAttribute("ParaValue", Param5)));
                    xele2.Add(new XElement("Plan_AdjustErrorPara_Item", new XAttribute("ParaID", "6"), new XAttribute("ParaName", ParamName6), new XAttribute("ParaValue", Param6)));
                    xele2.Add(new XElement("Plan_AdjustErrorPara_Item", new XAttribute("ParaID", "7"), new XAttribute("ParaName", ParamName7), new XAttribute("ParaValue", Param7)));
                    xele2.Add(new XElement("Plan_AdjustErrorPara_Item", new XAttribute("ParaID", "8"), new XAttribute("ParaName", ParamName8), new XAttribute("ParaValue", Param8)));
                    xele2.Add(new XElement("Plan_AdjustErrorPara_Item", new XAttribute("ParaID", "9"), new XAttribute("ParaName", ParamName9), new XAttribute("ParaValue", Param9)));
                    xele2.Add(new XElement("Plan_AdjustErrorPara_Item", new XAttribute("ParaID", "10"), new XAttribute("ParaName", ParamName10), new XAttribute("ParaValue", Param10)));
                    xele2.Add(new XElement("Plan_AdjustErrorPara_Item", new XAttribute("ParaID", "11"), new XAttribute("ParaName", ParamName11), new XAttribute("ParaValue", Param11)));
                    xele2.Add(new XElement("Plan_AdjustErrorPara_Item", new XAttribute("ParaID", "12"), new XAttribute("ParaName", ParamName12), new XAttribute("ParaValue", Param12)));
                    xele2.Add(new XElement("Plan_AdjustErrorPara_Item", new XAttribute("ParaID", "13"), new XAttribute("ParaName", ParamName13), new XAttribute("ParaValue", Param13)));
                    xele2.Add(new XElement("Plan_AdjustErrorPara_Item", new XAttribute("ParaID", "14"), new XAttribute("ParaName", ParamName14), new XAttribute("ParaValue", Param14)));
                    xele2.Add(new XElement("Plan_AdjustErrorPara_Item", new XAttribute("ParaID", "15"), new XAttribute("ParaName", ParamName15), new XAttribute("ParaValue", Param15)));
                    xele2.Add(new XElement("Plan_AdjustErrorPara_Item", new XAttribute("ParaID", "16"), new XAttribute("ParaName", ParamName16), new XAttribute("ParaValue", Param16)));
                    xele2.Add(new XElement("Plan_AdjustErrorPara_Item", new XAttribute("ParaID", "17"), new XAttribute("ParaName", ParamName17), new XAttribute("ParaValue", Param17)));
                    xele2.Add(new XElement("Plan_AdjustErrorPara_Item", new XAttribute("ParaID", "18"), new XAttribute("ParaName", ParamName18), new XAttribute("ParaValue", Param18)));
                }
                #endregion

                if (path == "0" || path == "1")//新增方案 修改方案
                {
                    if (File.Exists(ProductName + ".xml"))
                    {
                        File.Delete(ProductName + ".xml");
                    }
                    document.Save(ProductName + ".xml");
                }
                else//另存为方案
                {
                    StreamWriter sw = new StreamWriter(path);
                    sw.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" + document.ToString());
                    sw.Close();
                    sw.Dispose();
                }

                #region 数据库操作
                if (path == "0")//新增方案
                {
                    #region 保存数据
                    string SQLString = @"exec SaveCLTestSchemaNew @SchemaNOParam='" + Model + "-20" + "',@SchemaTypeParam='20',@MONameParam='" + Model + "',@SchemaNameParam='" + ProductName + "',@FileXMLParam='" + document.ToString() + "',@SchemaConfigureUserParam='" + OrBitUserName + "',@SchemaConfigureDateParam='" + System.DateTime.Now + "',@SchemaDescriptionParam='" + SchemaDescription + "',@SchemaNameNew='" + SchemaNameNew + "',@SchemaArea='国网'";
                    UserControl1.PUC.GetDataSetWithSQLString(SQLString);
                    #endregion

                    #region 保存历史数据
                    SQLString = @"exec SaveCLTestSchemaHistoricalRecords @SchemaNOParam='" + Model + "-20" + "',@SchemaTypeParam='20',@MONameParam='" + Model + "',@SchemaNameParam='" + ProductName + "',@FileXMLParam='" + document.ToString() + "',@SchemaConfigureUserParam='" + OrBitUserName + "',@SchemaConfigureDateParam='" + System.DateTime.Now + "',@SchemaDescriptionParam='" + SchemaDescription + "',@SchemaNameNew='" + SchemaNameNew + "',@SchemaArea='国网'";
                    UserControl1.PUC.GetDataSetWithSQLString(SQLString);
                    #endregion
                }
                else  //修改方案 另成为方案
                {
                    //#region //修改方案 另成为方案
                    //string SQLString = "select top 1 SchemaNO from CLTestSchema where SchemaType='20' and MOName='" + Model + "'";
                    //DataSet ds = UserControl1.PUC.GetDataSetWithSQLString(SQLString);
                    //if (ds.Tables[0].Rows.Count == 0)//如果另成为方案是新增的数据
                    //{
                        //#region 保存数据
                        //SQLString = @"exec SaveCLTestSchemaNew @SchemaNOParam='" + Model + "-20" + "',@SchemaTypeParam='20',@MONameParam='" + Model + "',@SchemaNameParam='" + ProductName + "',@FileXMLParam='" + document.ToString() + "',@SchemaConfigureUserParam='" + OrBitUserName + "',@SchemaConfigureDateParam='" + System.DateTime.Now + "'";
                        //UserControl1.PUC.GetDataSetWithSQLString(SQLString);
                        //#region 保存历史数据
                        //SQLString = @"exec SaveCLTestSchemaHistoricalRecords @SchemaNOParam='" + Model + "-20" + "',@SchemaTypeParam='20',@MONameParam='" + Model + "',@SchemaNameParam='" + ProductName + "',@FileXMLParam='" + document.ToString() + "',@SchemaConfigureUserParam='" + OrBitUserName + "',@SchemaConfigureDateParam='" + System.DateTime.Now + "'";
                        //UserControl1.PUC.GetDataSetWithSQLString(SQLString);
                        //#endregion
                      //  #endregion
                   // }
                    //else//修改方案和另成为方案是修改数据
                    //{
                        #region 保存历史数据
                    string SQLString = @"exec SaveCLTestSchemaHistoricalRecords @SchemaNOParam='" + Model + "-20" + "',@SchemaTypeParam='20',@MONameParam='" + Model + "',@SchemaNameParam='" + ProductName + "',@FileXMLParam='" + document.ToString() + "',@SchemaConfigureUserParam='" + OrBitUserName + "',@SchemaConfigureDateParam='" + System.DateTime.Now + "',@SchemaDescriptionParam='" + SchemaDescription + "',@SchemaNameNew='" + SchemaNameNew + "',@SchemaArea='国网'";
                       UserControl1.PUC.GetDataSetWithSQLString( SQLString);
                        #endregion

                        #region 修改数据
                       SQLString = @"update CLTestSchema set FileXML='" + document.ToString() + "',SchemaConfigureUser='" + OrBitUserName + "',SchemaConfigureDate='" + System.DateTime.Now + "',SchemaDescription='" + SchemaDescription + "' where MOName='" + Model + "' and SchemaType='20' and SchemaNameNew='" + SchemaNameNew + "' and SchemaName='" + ProductName + "'and SchemaArea='国网'";
                        UserControl1.PUC.GetDataSetWithSQLString(SQLString);
                        #endregion
                   // }
                    #endregion
                }
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
       // #endregion

        
    }
}

