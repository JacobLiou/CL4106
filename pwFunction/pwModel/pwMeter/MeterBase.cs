using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using pwInterface;
using pwClassLibrary;
using pwFunction.pwConst;
using pwClassLibrary.DataBase;
namespace pwFunction.pwMeter
{
    [Serializable()]
    public class MeterBasic : pwSerializable
    {
        OrBitADCService.ADCService adc = new OrBitADCService.ADCService();
        private string _XmlFilePath = "";
        private Stopwatch sth_SpaceTicker = new Stopwatch();                //记时时钟
        private int m_BwCount = 12;
        private  string sfilename = System.Windows.Forms.Application.StartupPath
                + pwFunction.pwConst.Variable.CONST_METERDATA
                + pwFunction.pwConst.GlobalUnit.g_Work.WorkSN + "\\"
                + "Save_Data.txt";

        /// <summary>
        /// 24只表共有信息
        /// </summary>
        public pwFunction.pwMeter.MeterInfo MInfo = null;

        /// <summary>
        /// 24只表测试数据
        /// </summary>
        public pwFunction.pwMeter.MeterData[] MData = null;

        public MeterBasic(int BwCount)
        {
            m_BwCount = BwCount;

            //初始化共有信息
            MInfo = new MeterInfo();

            //初始化数据
            MData = new MeterData[m_BwCount];
            for (int i = 0; i < m_BwCount; i++)
            {
                MData[i] = new MeterData();
                MData[i].bolIsCheck = true;
                MData[i].bolSaveData = true;
                MData[i].bolResult = true;
                MData[i].chrResult = pwFunction.pwConst.Variable.CTG_WeiJian;
                MData[i].intBno = i;
                MData[i].chrRexplain = "";
                MData[i].sngEnergy = 0f;
            }
            if (!File.Exists(sfilename)) File.CreateText(sfilename);

        }

        ~MeterBasic()
        {
            MInfo = null;
            MData = null;
        }

        #region Init Data
        /// <summary>
        /// 根据方案初始化表结果数据
        /// </summary>
        public void InitData()
        {
            try
            {
                for (int i = 0; i < m_BwCount; i++)
                {
                    //MData[i].chrTxm = "";
                    MData[i].chrAddr = "";
                    MData[i].chrScbh = "";
                    MData[i].sngEnergy = 0f;
                    MData[i].bolResult = true;
                    MData[i].chrResult = pwConst.Variable.CTG_WeiJian;
                    MData[i].bolToServer = false;
                    MData[i].bolSaveData = true;
                    MData[i].chrRexplain = "";
                    MData[i].MeterResults.Clear();

                    MData[i].MeterErrors.Clear();



                }
            }
            catch
            {
                throw new Exception("初始化表结果数据错误");
                //GlobalUnit.g_MsgControl.OutMessage("初始化表结果数据错误", false);


            }
        }


        /// <summary>
        /// 根据方案初始化表结果数据
        /// </summary>
        /// <param name="int_BwNo">表位</param>
        /// <returns></returns>
        private Dictionary<string, DataResultBasic> InitMeterResultsData(int int_BwNo)
        {
            Dictionary<string, DataResultBasic> _CurMeterResults = new Dictionary<string, DataResultBasic>();
            enmMeterPrjID enm_PrjID;

            #region
            if (GlobalUnit.g_Plan.cReadScbh.IsCheck == true)
            {
                enm_PrjID = enmMeterPrjID.RS485读生产编号;
                AddGroupData(enm_PrjID, int_BwNo, ref  _CurMeterResults);
            }

            if (GlobalUnit.g_Plan.cWcjd.IsCheck == true)
            {
                enm_PrjID = enmMeterPrjID.误差检定;
                AddGroupData(enm_PrjID, int_BwNo, ref  _CurMeterResults);
            }


            if (GlobalUnit.g_Plan.cDgnSy.IsCheck == true)
            {
                enm_PrjID = enmMeterPrjID.日计时误差检定;
                AddGroupData(enm_PrjID, int_BwNo, ref  _CurMeterResults);
            }

            if (GlobalUnit.g_Plan.cSinglePhaseTest.IsCheck == true)
            {
                enm_PrjID = enmMeterPrjID.分相供电测试;
                AddGroupData(enm_PrjID, int_BwNo, ref  _CurMeterResults);
            }
            if (GlobalUnit.g_Plan.cACSamplingTest.IsCheck == true)
            {
                enm_PrjID = enmMeterPrjID.交流采样测试;
                AddGroupData(enm_PrjID, int_BwNo, ref  _CurMeterResults);
            }

            if (GlobalUnit.g_Plan.cReadEnergy.IsCheck == true)
            {
                enm_PrjID = enmMeterPrjID.读电能表底度;
                AddGroupData(enm_PrjID, int_BwNo, ref  _CurMeterResults);
            }


            if (GlobalUnit.g_Plan.cDownPara.IsCheck == true)
            {
                enm_PrjID = enmMeterPrjID.打包参数下载;
                AddGroupData(enm_PrjID, int_BwNo, ref  _CurMeterResults);
            }


            if (GlobalUnit.g_Plan.cSysClear.IsCheck == true)
            {
                enm_PrjID = enmMeterPrjID.系统清零;
                AddGroupData(enm_PrjID, int_BwNo, ref  _CurMeterResults);
            }


            #endregion

            return _CurMeterResults;
        }

        /// <summary>
        /// 初始化分组数据(单项)
        /// </summary>
        /// <param name="enm_PrjID"></param>
        /// <param name="int_BwNo"></param>
        /// <param name="_CurMeterResults"></param>
        private void AddGroupData(enmMeterPrjID enm_PrjID, int int_BwNo, ref Dictionary<string, DataResultBasic> _CurMeterResults)
        {
            DataResultBasic dataResule = new DataResultBasic();
            dataResule.Me_Bw = int_BwNo;
            dataResule.Me_PrjID = Convert.ToString((int)enm_PrjID);
            dataResule.Me_PrjName = enm_PrjID.ToString() + "结论";
            dataResule.Me_Result = Variable.CTG_WeiJian;
            if (_CurMeterResults.ContainsKey(((int)enm_PrjID).ToString()))
            {
                _CurMeterResults.Remove(((int)enm_PrjID).ToString());
            }
            _CurMeterResults.Add(Convert.ToString((int)enm_PrjID), dataResule);

        }

        /// <summary>
        /// 根据方案初始化表误差数据
        /// </summary>
        /// <param name="int_BwNo">表位</param>
        /// <returns></returns>
        private Dictionary<string, MeterErrorItem> InitMeterErrors(int int_BwNo)
        {
            Dictionary<string, MeterErrorItem> _CurMeterErrors = new Dictionary<string, MeterErrorItem>();
            MeterErrorItem ErrorResule;
            if (GlobalUnit.g_Plan.cWcjd.IsCheck == true)
            {
                foreach (StWcPoint _wcPoint in GlobalUnit.g_Plan.cWcPoint._WcPoint)
                {
                    ErrorResule = new MeterErrorItem();
                    ErrorResule.Me_Bw = int_BwNo;
                    ErrorResule.Item_PrjID = _wcPoint.PrjID;
                    ErrorResule.Item_PrjName = _wcPoint.PrjName;
                    ErrorResule.Item_Result = Variable.CTG_WeiJian;
                    if (_CurMeterErrors.ContainsKey(Convert.ToInt32(ErrorResule.Item_PrjID).ToString()))
                    {
                        _CurMeterErrors.Remove(Convert.ToInt32(ErrorResule.Item_PrjID).ToString());
                    }
                    _CurMeterErrors.Add(Convert.ToInt32(ErrorResule.Item_PrjID).ToString(), ErrorResule);

                }
            }
            return _CurMeterErrors;
        }
        #endregion

        #region
        /// <summary>
        /// 判断所有需测试项目是否已经测试
        /// </summary>
        /// <param name="_CurMeterResults"></param>
        /// <param name="intLx">0为检定，1为前装</param>
        /// <returns></returns>
        public bool bAllPrjTestOK(Dictionary<string, DataResultBasic> _CurMeterResults, int intLx)
        {
            enmMeterPrjID enm_PrjID;

            if (intLx == 0)//检定
            {
                #region 检定大项----要检，但没有数据，就是不合格----
                if (GlobalUnit.g_Plan.cReadScbh.IsCheck == true)
                {
                    enm_PrjID = enmMeterPrjID.RS485读生产编号;

                    if (!_CurMeterResults.ContainsKey(((int)enm_PrjID).ToString()))
                        return false;
                }

                if (GlobalUnit.g_Plan.cWcjd.IsCheck == true)
                {
                    enm_PrjID = enmMeterPrjID.误差检定;
                    if (!_CurMeterResults.ContainsKey(((int)enm_PrjID).ToString()))
                        return false;
                }


                if (GlobalUnit.g_Plan.cDgnSy.IsCheck == true)
                {
                    enm_PrjID = enmMeterPrjID.日计时误差检定;
                    if (!_CurMeterResults.ContainsKey(((int)enm_PrjID).ToString()))
                        return false;
                }


                if (GlobalUnit.g_Plan.cSinglePhaseTest.IsCheck == true)
                {
                    enm_PrjID = enmMeterPrjID.分相供电测试;
                    if (!_CurMeterResults.ContainsKey(((int)enm_PrjID).ToString()))
                        return false;
                }


                if (GlobalUnit.g_Plan.cACSamplingTest.IsCheck == true)
                {
                    enm_PrjID = enmMeterPrjID.交流采样测试;
                    if (!_CurMeterResults.ContainsKey(((int)enm_PrjID).ToString()))
                        return false;
                }
                if (GlobalUnit.g_Plan.cReadEnergy.IsCheck == true)
                {
                    enm_PrjID = enmMeterPrjID.读电能表底度;
                    if (!_CurMeterResults.ContainsKey(((int)enm_PrjID).ToString()))
                        return false;
                }

                if (GlobalUnit.g_Plan.cDownPara.IsCheck == true)
                {
                    enm_PrjID = enmMeterPrjID.打包参数下载;
                    if (!_CurMeterResults.ContainsKey(((int)enm_PrjID).ToString()))
                        return false;
                }


                if (GlobalUnit.g_Plan.cSysClear.IsCheck == true)
                {
                    enm_PrjID = enmMeterPrjID.系统清零;
                    if (!_CurMeterResults.ContainsKey(((int)enm_PrjID).ToString()))
                        return false;
                }
                #endregion

            }
            else if (intLx == 1)
            {

            }


            return true;
        }

        #endregion


        #region Save Lacat
        public void Save()
        {
            try
            {
                //GlobalUnit.g_MsgControl.OutMessage("正在保存当前工单测试数据，请稍候......", false);

                //_XmlFilePath = System.Windows.Forms.Application.StartupPath + pwFunction.pwConst.Variable.CONST_METERDATAXML;

                _XmlFilePath = System.Windows.Forms.Application.StartupPath
                    + pwFunction.pwConst.Variable.CONST_METERDATA
                    + pwFunction.pwConst.GlobalUnit.g_Work.WorkSN + "\\"
                    + DateTime.Now.ToShortDateString() + "_Data.xml";

                if (System.IO.File.Exists(_XmlFilePath))
                {
                    System.IO.File.Delete(_XmlFilePath);
                }
                XmlNode _XmlNode = clsXmlControl.CreateXmlNode("Data");

                #region 加载工单、产品，检定信息
                XmlNode _XmlNodeChildNodes_JDInfo = clsXmlControl.CreateXmlNode("JDInfo");

                CreateChildNodes_Info(ref _XmlNodeChildNodes_JDInfo);

                _XmlNode.AppendChild(_XmlNodeChildNodes_JDInfo);

                #endregion

                #region 加载检定数据
                for (int i = 0; i < GlobalUnit.g_BW; i++)
                {
                    XmlNode _XmlNodeChildNodes_Meter = clsXmlControl.CreateXmlNode("Meter" + (i + 1).ToString());
                    _XmlNode.AppendChild(_XmlNodeChildNodes_Meter);


                    XmlNode _XmlNodeChildNodes_Base = clsXmlControl.CreateXmlNode("Base");
                    CreateChildNodes_BaseInfo(i, ref _XmlNodeChildNodes_Base);
                    _XmlNodeChildNodes_Meter.AppendChild(_XmlNodeChildNodes_Base);


                    XmlNode _XmlNodeChildNodes_Group = clsXmlControl.CreateXmlNode("Group");
                    CreateChildNodes_GroupInfo(i, ref _XmlNodeChildNodes_Group);
                    _XmlNodeChildNodes_Meter.AppendChild(_XmlNodeChildNodes_Group);


                    XmlNode _XmlNodeChildNodes_ErrorItem = clsXmlControl.CreateXmlNode("ErrorItem");
                    CreateChildNodes_ErrorItemInfo(i, ref _XmlNodeChildNodes_ErrorItem);
                    _XmlNodeChildNodes_Meter.AppendChild(_XmlNodeChildNodes_ErrorItem);

                    XmlNode _XmlNodeChildNodes_DgnItem = clsXmlControl.CreateXmlNode("DgnItem");
                    CreateChildNodes_DgnItemInfo(i, ref _XmlNodeChildNodes_DgnItem);
                    _XmlNodeChildNodes_Meter.AppendChild(_XmlNodeChildNodes_DgnItem);

                }
                #endregion

                clsXmlControl.SaveXml(_XmlNode, _XmlFilePath);
                //GlobalUnit.g_MsgControl.OutMessage("保存测试数据，成功！", false);

            }
            catch (System.Exception errorr)
            {
                throw new Exception("保存测试数据，错误");
                //GlobalUnit.g_MsgControl.OutMessage("保存测试数据，错误", false);

            }

        }

        /// <summary>
        /// 加载检定信息
        /// </summary>
        /// <param name="xml"></param>
        private void CreateChildNodes_Info(ref XmlNode xml)
        {
            xml.AppendChild(clsXmlControl.CreateXmlNode("Info"
                            , "WorkSN", GlobalUnit.g_Work.WorkSN
                            , "CustomerName", GlobalUnit.g_Work.CustomerName
                            , "ProductsName", GlobalUnit.g_Work.ProductsName
                            , "ProductsSN", GlobalUnit.g_Work.ProductsSN
                            , "ProductsModel", GlobalUnit.g_Work.ProductsModel


                            , "Clfs", GlobalUnit.g_Products.Clfs
                            , "Ub", GlobalUnit.g_Products.Ub
                            , "Ib", GlobalUnit.g_Products.Ib
                            , "IMax", GlobalUnit.g_Products.IMax
                            , "Constant", GlobalUnit.g_Products.Constant
                            , "DJ", GlobalUnit.g_Products.DJ
                            , "PL", GlobalUnit.g_Products.PL


                            , "TaiID", MInfo.TaiID.ToString()
                            , "Jdrq", MInfo.Jdrq
                            , "Wd", MInfo.Wd
                            , "Sd", MInfo.Sd
                            , "Jyy", MInfo.Jyy));

        }

        /// <summary>
        /// 加载总结论
        /// </summary>
        /// <param name="intBw"></param>
        /// <param name="xml"></param>
        private void CreateChildNodes_BaseInfo(int intBw, ref XmlNode xml)
        {
            //总结论
            bool _bResult = MData[intBw].bolAlreadyTest || MData[intBw].bolResult;
            xml.AppendChild(clsXmlControl.CreateXmlNode("BaseInfo"
                            , "Bw", MData[intBw].intBno.ToString()
                            , "bolIsCheck", _bResult.ToString()     //MData[intBw].bolIsCheck.ToString()
                            , "chrTxm", MData[intBw].chrTxm
                            , "chrScbh", MData[intBw].chrScbh
                            , "chrAddr", MData[intBw].chrAddr
                            , "sngEnergy", MData[intBw].sngEnergy.ToString()
                            , "PrjName", MData[intBw].Me_PrjName
                            , "chrResult", MData[intBw].chrResult
                            , "chrRexplain", MData[intBw].chrRexplain
                            , "bolAlreadyTest", MData[intBw].bolAlreadyTest.ToString()
                //, "bolToServer", MData[intBw].bolToServer.ToString()
                            ));
        }

        /// <summary>
        /// 加载分组结论
        /// </summary>
        /// <param name="intBw"></param>
        /// <param name="xml"></param>
        private void CreateChildNodes_GroupInfo(int intBw, ref XmlNode xml)
        {

            //分项结论
            foreach (string strKey in MData[intBw].MeterResults.Keys)
            {
                DataResultBasic _Item = MData[intBw].MeterResults[strKey];
                xml.AppendChild(clsXmlControl.CreateXmlNode("GroupInfo"
                , "Bw", _Item.Me_Bw.ToString()
                , "Me_PrjID", _Item.Me_PrjID
                , "Me_PrjName", _Item.Me_PrjName
                , "Me_Result", _Item.Me_Result
                , "Me_Value", _Item.Me_Value));
            }
        }

        /// <summary>
        /// 加载误差结论
        /// </summary>
        /// <param name="intBw"></param>
        /// <param name="xml"></param>
        private void CreateChildNodes_ErrorItemInfo(int intBw, ref XmlNode xml)
        {

            //误差：子项数据
            foreach (MeterErrorItem _ErrorItem in MData[intBw].MeterErrors.Values)
            {
                xml.AppendChild(clsXmlControl.CreateXmlNode("ErrorItemInfo"
                , "Bw", _ErrorItem.Me_Bw.ToString()
                , "Item_PrjID", _ErrorItem.Item_PrjID
                , "Item_PrjName", _ErrorItem.Item_PrjName
                , "Item_Result", _ErrorItem.Item_Result
                , "Item_Value", _ErrorItem.ToString()));
            }
        }

        /// <summary>
        /// 加载多功能结论
        /// </summary>
        /// <param name="intBw"></param>
        /// <param name="xml"></param>
        private void CreateChildNodes_DgnItemInfo(int intBw, ref XmlNode xml)
        {
            //多功能：子项数据
            foreach (MeterDgnItem _DgnItem in MData[intBw].MeterDgns.Values)
            {
                xml.AppendChild(clsXmlControl.CreateXmlNode("DgnItemInfo"
                , "Bw", _DgnItem.Me_Bw.ToString()
                , "Item_PrjID", _DgnItem.Item_PrjID
                , "Item_PrjName", _DgnItem.Item_PrjName
                , "Item_Result", _DgnItem.Item_Result
                , "Item_Value", _DgnItem.Item_Value));
            }


        }
        #endregion

        #region Save To DB


        #region 检定数据保存到数据库
        private object Locked = new object();
        private void SaveCommunicationData(object obj)
        {
            //string sfilename = System.Windows.Forms.Application.StartupPath
            //    + pwFunction.pwConst.Variable.CONST_METERDATA
            //    + pwFunction.pwConst.GlobalUnit.g_Work.WorkSN + "\\"
            //    + "Save_Data.txt";
            //if (!File.Exists(sfilename)) File.CreateText(sfilename);
            
            try
            {
                lock (Locked)
                {
                    string objStr = obj.ToString();

                    #region 写入文件
                    //File.AppendAllText(@sfilename, objStr);
                    StreamWriter sw = new StreamWriter(@sfilename, true, System.Text.Encoding.Unicode);
                    sw.WriteLine(objStr);
                    sw.Close();
                    #endregion
                }
            }
            catch (Exception e)
            {
                return;
            }
        }

        /// <summary>
        /// 保存检定数据到数据库
        /// </summary>
        /// <param name="PathServer"></param>
        /// <param name="OrBitUserName"></param>
        public void SaveToDB(string WCLPathServer, string OrBitUserName, string chr_Products_SN)
        {
            sth_SpaceTicker.Reset();
            sth_SpaceTicker.Start();       //开始记时             
            string PathServer = "";
            long intTicker = 0;
            object obj=new object();
            try
            {
                PathServer = WCLPathServer.Substring(0, WCLPathServer.LastIndexOf("/")) + "/DataService.svc";
                //GlobalUnit.g_MsgControl.OutMessage("正在保存当前工单测试数据，请稍候......", false);
                #region 加载检定数据
                //DateTime _curTime = System.DateTime.Now;//274201927

                string StrSQL = "exec GetNowSystemTime";
                DataTable NowSystemTimeDT = adc.GetDataSetWithSQLString(PathServer, StrSQL).Tables[0];
                System.DateTime _curTime = Convert.ToDateTime(NowSystemTimeDT.Rows[0][0].ToString());

                obj = "开始**保存" + _curTime.ToShortDateString() + " " + _curTime.ToLongTimeString() + ":"
                        + _curTime.Millisecond.ToString("D3")+":";

                for (int i = 0; i < GlobalUnit.g_BW; i++)
                {
                    obj += "表" + i.ToString() + ":生产编号" + MData[i].chrScbh + ":";

                    if (MData[i].chrScbh == "" || MData[i].chrScbh == null)
                    {
                        obj +="continue" + ";";
                        continue;
                    }
                    if (MData[i].bolSaveData)
                    {
                        SaveTestFormstring(OrBitUserName, PathServer, i, chr_Products_SN, _curTime);
                        SaveTestFormDetail(PathServer, i, _curTime);
                        SaveMultiFunctionTestDetail(PathServer, i, _curTime);
                        obj += "true" + ";";
                    }
                    else
                    {
                        obj += "false" + ";";
                    }
                }
                #endregion
                //GlobalUnit.g_MsgControl.OutMessage("保存测试数据，成功！", false);
            }
            catch (Exception e)
            {
                //GlobalUnit.g_MsgControl.OutMessage("保存测试数据，错误", false);               
                MessageBox.Show(e.Message);
                obj += "出错!";
            }
            finally
            {
                intTicker = Convert.ToInt32(sth_SpaceTicker.ElapsedMilliseconds / 1000);
                obj += "耗时" + intTicker.ToString() + "秒" + ";结束";
                ThreadPool.QueueUserWorkItem(new WaitCallback(SaveCommunicationData), obj);

            }
        }

        /// <summary>
        /// 保存检定表单数据
        /// </summary>
        /// <param name="OrBitUserName"></param>
        /// <param name="PathServer"></param>
        /// <param name="intBw"></param>
        private void SaveTestFormstring(string OrBitUserName, string PathServer, int intBw, string chr_Products_SN,DateTime CurTime)
        {
            try
            {
                #region 变量初始化
                string zhengjitiaomaSN = ""; //整机条型码SN 
                string shengchangbianhao = "";//生产编号SN
                string gongdanhao = "";//工单号SN
                string wuliaobianma = "";//物料编码
                string cheshifanganbianhao = "";//测试方案编号
                string ceshitaijiabianhao = "";//测试台架编号
                string ceshibiaoweihao = "";//测试表位号
                string chaozuoren = "";//操作人
                string ceshijielun = "";//测试结论
                string zhiguangjielun = "";//直观检查结论
                string dushengchangbianhao = "";//读生产编号结论
                string dudianbiaodidujielun = "";//读电表底度结论
                string jiaozhunwuchajielun = "";//校准误差结论
                string jiaozhunshizhongjielun = "";//校准时钟结论
                string wuchajiandingjielun = "";//误差检定结论
                string duogongnengjiandingjielun = "";//多功能检定结论
                string dabaocshuxiazaijielun = "";//打包参数下载结论
                string xitongqinglingjielun = "";//系统清零结论
                string laohuajielun = "";//老化结论
                string diaobiaodid = "";//电表底度
                string buliangdaima = "";//不良代码
                string fanggongzhongcebiaoji = "";//返工重测标记
                string fanganceshibiaoji = "";//方案测试标记
                string zhiliangbuchoujianbiaoji = "";//质量部抽检标记
                string dulingxiandljielun = "";//读零线电流结论
                string lingxiandljielun = "";//零线电流
                string chr_Conclusion_SinglePhaseTest = "";//分相供电测试结论
                string chr_Conclusion_ACSamplingTest = "";//交流采样测试结论
                string chr_Data_SinglePhaseTest = "";//分相供电测试数据
                string chr_Data_ACSamplingTest = "";//交流采样测试数据


                #endregion

                #region 变量赋值
                string StrSQL = @"select ISNull(c.WorkcenterName,'') as WorkcenterName,
                                Isnull(b.DivisionName,'') as DivisionName
                                from Resource a 
                                left outer join Division b on a.DivisionId=b.DivisionId 
                                left outer join Workcenter c on a.WorkcenterId=c.WorkcenterId
                                where a.ResourceName='" + GlobalUnit.g_DeskNo + "'";
                DataTable dt = adc.GetDataSetWithSQLString(PathServer, StrSQL).Tables[0];
                string zhidaozhongxing = "龙岗制造中心";
                string chejian = "";
                string changxian = "";
                if (dt.Rows.Count != 0)
                {
                    chejian = dt.Rows[0]["DivisionName"].ToString();
                    changxian = dt.Rows[0]["WorkcenterName"].ToString();
                }
                zhengjitiaomaSN = ""; //整机条型码SN 

                #region 生产编号SN
                try
                {
                    shengchangbianhao = MData[intBw].chrScbh;//生产编号SN

                }
                catch
                { }
                #endregion

                gongdanhao = GlobalUnit.g_Work.WorkSN; //工单号SN
                wuliaobianma = chr_Products_SN;//物料编码
                cheshifanganbianhao = "";//测试方案编号
                #region 测试台架编号
                try
                {
                    ceshitaijiabianhao = GlobalUnit.g_DeskNo;//测试台架编号
                }
                catch
                {

                }
                #endregion
                ceshibiaoweihao = intBw.ToString();//测试表位号
                chaozuoren = OrBitUserName;//操作人
                #region 测试结论
                try
                {
                    ceshijielun = MData[intBw].chrResult;//测试结论
                }
                catch
                { }
                #endregion
                zhiguangjielun = "";//直观检查结论
                #region 读生产编号结论
                try
                {
                    dushengchangbianhao = MData[intBw].MeterResults["100"].Me_Result.Trim();//读生产编号结论
                }
                catch
                {
                    if (GlobalUnit.g_Plan.cReadScbh.IsCheck == true) dushengchangbianhao = Variable.CTG_WeiJian;
                }
                #endregion

                #region 误差检定结论
                try
                {
                    wuchajiandingjielun = MData[intBw].MeterResults["200"].Me_Result.Trim();//误差检定结论
                }
                catch
                {
                    if (GlobalUnit.g_Plan.cWcjd.IsCheck == true) wuchajiandingjielun = Variable.CTG_WeiJian;
                }
                #endregion

                #region 日计时误差结论
                try
                {
                    duogongnengjiandingjielun = MData[intBw].MeterResults["300"].Me_Result.Trim();//日计时误差结论
                }
                catch
                {
                    if (GlobalUnit.g_Plan.cDgnSy.IsCheck == true) duogongnengjiandingjielun = Variable.CTG_WeiJian;
                }
                #endregion

                #region 分相供电测试结论
                try
                {
                    chr_Conclusion_SinglePhaseTest = MData[intBw].MeterResults["400"].Me_Result.Trim();
                }
                catch
                {
                    if (GlobalUnit.g_Plan.cSinglePhaseTest.IsCheck == true) chr_Conclusion_SinglePhaseTest = Variable.CTG_WeiJian;
                }
                #endregion

                #region 流采样测试结论
                try
                {
                    chr_Conclusion_ACSamplingTest = MData[intBw].MeterResults["500"].Me_Result.Trim();
                }
                catch
                {
                    if (GlobalUnit.g_Plan.cACSamplingTest.IsCheck == true) chr_Conclusion_ACSamplingTest = Variable.CTG_WeiJian;
                }
                #endregion

                #region 读电表底度结论
                try
                {
                    dudianbiaodidujielun = MData[intBw].MeterResults["600"].Me_Result.Trim();//读电表底度结论
                }
                catch
                {
                    if (GlobalUnit.g_Plan.cReadEnergy.IsCheck == true) dudianbiaodidujielun = Variable.CTG_WeiJian;
                }
                #endregion

                #region 打包参数下载结论
                try
                {
                    dabaocshuxiazaijielun = MData[intBw].MeterResults["700"].Me_Result.Trim();//打包参数下载结论
                }
                catch
                {
                    if (GlobalUnit.g_Plan.cDownPara.IsCheck == true) dabaocshuxiazaijielun = Variable.CTG_WeiJian;
                }
                #endregion

                #region 系统清零结论
                try
                {
                    xitongqinglingjielun = MData[intBw].MeterResults["800"].Me_Result.Trim();//系统清零结论
                }
                catch
                {
                    if (GlobalUnit.g_Plan.cSysClear.IsCheck == true) xitongqinglingjielun = Variable.CTG_WeiJian;
                }
                #endregion

                #region 老化结论
                try
                {
                    laohuajielun = MData[intBw].MeterResults["600"].Me_Result.Trim();//读电表底度结论
                }
                catch
                {
                    if (GlobalUnit.g_Plan.cReadEnergy.IsCheck == true) laohuajielun = Variable.CTG_WeiJian;
                }
                #endregion

                #region 分相供电测试数据
                try
                {
                    chr_Data_SinglePhaseTest = MData[intBw].MeterResults["400"].Me_Value.ToString();// 零线电流
                }
                catch
                { }
                #endregion

                #region 交流采样测试数据
                try
                {
                    chr_Data_ACSamplingTest = MData[intBw].MeterResults["500"].Me_Value.ToString();// 零线电流
                }
                catch
                { }
                #endregion

                #region 电表底度
                try
                {
                    diaobiaodid = MData[intBw].sngEnergy.ToString();// MData[intBw].MeterResults["200"].Me_Value.Trim();//电表底度
                }
                catch
                { }
                #endregion

                buliangdaima = "";//不良代码
                fanggongzhongcebiaoji = "";//返工重测标记
                fanganceshibiaoji = "";//方案测试标记
                zhiliangbuchoujianbiaoji = "";//质量部抽检标记
                #endregion

                #region StrSQL
                StrSQL = @"exec SaveTestForm_Overseas_ThreePhase @chr_Meter_SN='" + zhengjitiaomaSN 
                   + "',@chr_Scbh_SN='"+ shengchangbianhao 
                   + "',@chr_Work_SN='"+ gongdanhao 
                   + "',@chr_Products_SN='"+ wuliaobianma 
                   + "',@chr_Factory ='"+ zhidaozhongxing 
                   + "',@chr_Shop='"+ chejian 
                   + "',@chr_Line='"+ changxian 
                   + "',@chr_Operator='"+ chaozuoren
                   + "',@dat_DateTime='" + CurTime 
                   + "',@chr_Conclusion='"+ ceshijielun 
                   + "',@chr_Conclusion_Zgjc='"+ zhiguangjielun 
                   + "',@chr_Conclusion_RScbh='"+ dushengchangbianhao 
                   + "',@chr_Conclusion_REnergy='"+ dudianbiaodidujielun 
                   + "',@chr_Conclusion_AdClock='"+ jiaozhunwuchajielun 
                   + "',@chr_Conclusion_AdError='"+ jiaozhunshizhongjielun 
                   + "',@chr_Conclusion_Error='"+ wuchajiandingjielun 
                   + "',@chr_Conclusion_Dgn='"+ duogongnengjiandingjielun 
                   + "',@chr_Conclusion_Down='"+ dabaocshuxiazaijielun 
                   + "',@chr_Conclusion_SysClear='"+ xitongqinglingjielun 
                   + "',@chr_Conclusion_Ageing='"+ laohuajielun 
                   + "',@chr_Conclusion_LxDL='"+ dulingxiandljielun 
                   + "',@Sng_LxDL='"+ lingxiandljielun 
                   + "',@Sng_Energy='"+ diaobiaodid 
                   + "',@chr_ErrorCode='"+ buliangdaima 
                   + "',@B_Mark_Retest='"+ fanggongzhongcebiaoji 
                   + "',@B_Mark_Test='"+ fanganceshibiaoji 
                   + "',@B_Mark_QC='"+ zhiliangbuchoujianbiaoji 
                   + "',@chr_FA_ID='"+ cheshifanganbianhao 
                   + "',@chr_TaiTi_ID='"+ ceshitaijiabianhao 
                   + "',@chr_Bw='"+ ceshibiaoweihao
                   + "',@chr_Conclusion_SinglePhaseTest='" + chr_Conclusion_SinglePhaseTest
                   + "',@chr_Conclusion_ACSamplingTest='" + chr_Conclusion_ACSamplingTest
                   + "',@chr_Data_SinglePhaseTest='" + chr_Data_SinglePhaseTest
                   + "',@chr_Data_ACSamplingTest='" + chr_Data_ACSamplingTest + "'"; 

                #endregion
                adc.GetDataSetWithSQLString(PathServer, StrSQL);
                
            }
            catch (Exception e)
            {
                GlobalUnit.g_MsgControl.OutMessage("保存测试数据，错误", false);

                //MessageBox.Show(e.Message);
            }
        }

        #region 误差详细数据
        /// <summary>
        /// 误差详细数据
        /// </summary>
        /// <param name="OrBitUserName"></param>
        /// <param name="PathServer"></param>
        /// <param name="intBw"></param>
        private void SaveTestFormDetail(string PathServer, int intBw, DateTime CurTime)
        {
            try
            {
                string _glfx = "";
                foreach (MeterErrorItem _ErrorItem in MData[intBw].MeterErrors.Values)
                {
                    string[] Item_Value = _ErrorItem.ToString().Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    string[] Item_PrjName = _ErrorItem.Item_PrjName.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    string Int_DLHL = Item_PrjName[0];
                    if (Item_PrjName[1] == "P+")
                        _glfx = "正向有功";
                    else if (Item_PrjName[1] == "P-")
                        _glfx = "反向有功";
                    else if (Item_PrjName[1] == "Q+")
                        _glfx = "正向无功";
                    else if (Item_PrjName[1] == "Q-")
                        _glfx = "反向无功";
                    string chr_GLFX = _glfx;
                    string Chr_YJ = Item_PrjName[2];
                    string Chr_GLYS = Item_PrjName[3];
                    string Sng_xIb = Item_PrjName[4];
                    string StrSQL = @"exec SaveTestFormDetail_Overseas_ThreePhase @chr_Meter_SN='',@chr_Scbh_SN='" 
                    //string StrSQL = @"exec SaveTestForm_SXDetail @chr_Meter_SN='',@chr_Scbh_SN='" 
                        + MData[intBw].chrScbh + "',@Int_DLHL='" + Int_DLHL + "',@chr_GLFX='" + chr_GLFX + "',@Chr_YJ='" 
                        + Chr_YJ + "',@Chr_GLYS='" + Chr_GLYS + "',@Sng_xIb='" + Sng_xIb + "',@Chr_JL='" 
                        + _ErrorItem.Item_Result + "',@Sng_Error_PJ='" + Item_Value[0] + "',@Sng_Error_HZ='" + Item_Value[1] + "',@Sng_Error_1='" 
                        + Item_Value[2] + "',@Sng_Error_2='" + Item_Value[3] + "',@Sng_Error_3='" + Item_Value[4] + "',@Sng_Error_4='" + Item_Value[5] + "',@Sng_Error_5='" 
                        + Item_Value[6] + "',@Sng_Error_6='" + Item_Value[7] + "',@Sng_Error_7='" + Item_Value[8] + "',@Sng_Error_8='" + Item_Value[9] + "',@Sng_Error_9='"
                        + Item_Value[10] + "',@Sng_Error_10='" + Item_Value[11] + "',@Sng_Error_PC='" + Item_Value[12] + "',@dat_DateTime='" + CurTime + "'";
                    adc.GetDataSetWithSQLString(PathServer, StrSQL);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

            }
        }
        #endregion

        #region 多功能检定详细数据
        /// <summary>
        /// 多功能检定详细数据
        /// </summary>
        /// <param name="PathServer"></param>
        /// <param name="intBw"></param>
        private void SaveMultiFunctionTestDetail(string PathServer, int intBw, DateTime CurTime)
        {
            //多功能：子项数据
            try
            {
                foreach (MeterDgnItem _DgnItem in MData[intBw].MeterDgns.Values)
                {
                    string StrSQL = @"exec SaveMultiFunction_Overseas_ThreePhase @chr_Meter_SN='',@chr_Scbh_SN='"
                        + MData[intBw].chrScbh + "',@Chr_ItemCode='" + _DgnItem.Item_PrjName + "',@Chr_JLMultiFunction='" 
                        + _DgnItem.Item_Result + "',@Chr_Data='" + _DgnItem.Item_Value + "',@dat_DateTime='" + CurTime + "'";
                    adc.GetDataSetWithSQLString(PathServer, StrSQL);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        #endregion

        #endregion



        #endregion
    }
}
