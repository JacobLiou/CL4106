using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Net;
using pwClassLibrary.DataBase;
using pwInterface;
namespace pwFunction.pwSystemModel
{
    /// <summary>
    /// 
    /// </summary>
    public class SystemConfigure
    {
        /// <summary>
        /// 系统设置信息配置
        /// </summary>
        private Dictionary<string, StSystemInfo> _SystemMode;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SystemConfigure()
        {
            _SystemMode = new Dictionary<string, StSystemInfo>();
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~SystemConfigure()
        {
            _SystemMode = null;
        }

        /// <summary>
        /// 初始化系统配置信息
        /// </summary>
        public void Load()
        {
            string _ErrorString = "";
            _SystemMode.Clear();            ///清空系统配置集合
            XmlNode _XmlNode = clsXmlControl.LoadXml(Application.StartupPath + pwFunction.pwConst.Variable.CONST_SYSTEMPATH, out _ErrorString);
            if (_ErrorString != "")
            {
                _XmlNode = clsXmlControl.CreateXmlNode("SystemInfo");
                #region 加载系统配置默认值

                this.CreateDefaultData(ref _XmlNode);

                #endregion

                clsXmlControl.SaveXml(_XmlNode, Application.StartupPath + pwFunction.pwConst.Variable.CONST_SYSTEMPATH);
            }

            if (_XmlNode.ChildNodes.Count > 0)
            {
                if (_XmlNode.ChildNodes[0].Attributes.Count < 6)
                {
                    pwClassLibrary.pwFile.File.RemoveFile(Application.StartupPath + pwFunction.pwConst.Variable.CONST_SYSTEMPATH);   //如果发现旧的系统配置文件就要删除掉，再重新创建
                    this.Load();
                    return;
                }
            }
            for (int _i = 0; _i < _XmlNode.ChildNodes.Count; _i++)
            {
                StSystemInfo _Item = new StSystemInfo();

                _Item.Value = _XmlNode.ChildNodes[_i].Attributes[1].Value;       //项目值
                _Item.Name = _XmlNode.ChildNodes[_i].Attributes[2].Value;       //项目中文名称
                _Item.Description = _XmlNode.ChildNodes[_i].Attributes[3].Value;      //项目描述
                _Item.ClassName = _XmlNode.ChildNodes[_i].Attributes[4].Value;  //项目分类名称
                _Item.DataSource = _XmlNode.ChildNodes[_i].Attributes[5].Value; //数据源
                if (_SystemMode.ContainsKey(_XmlNode.ChildNodes[_i].Attributes[0].Value))
                    _SystemMode.Remove(_XmlNode.ChildNodes[_i].Attributes[0].Value);
                _SystemMode.Add(_XmlNode.ChildNodes[_i].Attributes[0].Value, _Item);
            }
        }
        /// <summary>
        /// 读取系统配置项目值
        /// </summary>
        /// <param name="Tkey">系统项目ID</param>
        /// <returns>系统项目配置值</returns>
        public string GetValue(string Tkey)
        {
            if (_SystemMode.Count == 0)
                return "";
            if (_SystemMode.ContainsKey(Tkey))
                return _SystemMode[Tkey].Value;
            else
                return "";
        }
        /// <summary>
        /// 获取系统配置项目结构体
        /// </summary>
        /// <param name="Tkey">系统项目ID</param>
        /// <returns></returns>
        public StSystemInfo getItem(string Tkey)
        {
            if (_SystemMode.Count == 0)
                return new StSystemInfo();
            if (_SystemMode.ContainsKey(Tkey))
                return _SystemMode[Tkey];
            else
                return new StSystemInfo();
        }

        /// <summary>
        /// 添加系统配置项目
        /// </summary>
        /// <param name="Tkey">系统项目名称</param>
        /// <param name="TValue">系统项目配置值</param>
        public void Add(string Tkey, StSystemInfo Item)
        {
            if (_SystemMode.ContainsKey(Tkey))
            {
                _SystemMode.Remove(Tkey);
                _SystemMode.Add(Tkey, Item);
            }
            else
                _SystemMode.Add(Tkey, Item);
            return;
        }

        /// <summary>
        /// 修改键值
        /// </summary>
        /// <param name="Tkey">关键字</param>
        /// <param name="TValue">修改的值</param>
        public bool EditValue(string Tkey, string TValue)
        {
            if (_SystemMode.ContainsKey(Tkey))
            {
                StSystemInfo _Item = _SystemMode[Tkey];
                _Item.Value = TValue;
                _SystemMode.Remove(Tkey);
                _SystemMode.Add(Tkey, _Item);
                return true;
            }
            else { return false; }
        }

        /// <summary>
        /// 系统配置项目个数
        /// </summary>
        public int Count
        {
            get
            {
                return _SystemMode.Count;
            }
        }

        /// <summary>
        /// 获取关键字列表
        /// </summary>
        /// <returns></returns>
        public List<string> getKeyNames()
        {
            List<string> _Keys = new List<string>();
            foreach (string _name in _SystemMode.Keys)
            {
                _Keys.Add(_name);
            }
            return _Keys;
        }
        /// <summary>
        /// 清空列表
        /// </summary>
        public void Clear()
        {
            _SystemMode.Clear();
        }


        /// <summary>
        /// 存储系统配置XML文档
        /// </summary>
        public void Save()
        {
            clsXmlControl _Xml = new clsXmlControl();
            _Xml.appendchild("", "SystemInfo");
            foreach (string _Key in _SystemMode.Keys)
            {
                _Xml.appendchild(""
                                , "R"
                                , "Item", _Key
                                , "Value", _SystemMode[_Key].Value
                                , "Name", _SystemMode[_Key].Name
                                , "Description", _SystemMode[_Key].Description
                                , "ClassName", _SystemMode[_Key].ClassName
                                , "DataSource", _SystemMode[_Key].DataSource);
            }
            _Xml.SaveXml(Application.StartupPath + pwFunction.pwConst.Variable.CONST_SYSTEMPATH);
        }

        /// <summary>
        /// 创建系统默认配置文件
        /// </summary>
        /// <param name="xml"></param>
        private void CreateDefaultData(ref XmlNode xml)
        {

            #region 作废
            /*
            #region ------------CL2018-1端口分配-----------
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                                        , "Item", pwFunction.pwConst.Variable.CTC_2018IP
                                        , "Value", "193.168.18.1"
                                        , "Name", "CL2018-1网络IP地址"
                                        , "Description", "CL2018-1网络IP地址，与主机在同一网段上，请勿擅自更改，默认193.168.18.1"
                                        , "ClassName", "B端口信息配置"
                                        , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                                        , "Item", pwFunction.pwConst.Variable.CTC_COMY
                                        , "Value", "29"
                                        , "Name", "标准源通讯端口"
                                        , "Description", "标准源通讯端口,默认29"
                                        , "ClassName", "B端口信息配置"
                                        , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                                        , "Item", pwFunction.pwConst.Variable.CTC_COMB
                                        , "Value", "30"
                                        , "Name", "标准表通讯端口"
                                        , "Description", "标准表通讯端口,默认30"
                                        , "ClassName", "B端口信息配置"
                                        , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                                        , "Item", pwFunction.pwConst.Variable.CTC_COMW
                                        , "Value", "31"
                                        , "Name", "误差板通讯端口"
                                        , "Description", "误差板通讯端口,默认31"
                                        , "ClassName", "B端口信息配置"
                                        , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                                        , "Item", pwFunction.pwConst.Variable.CTC_COMT
                                        , "Value", "32"
                                        , "Name", "时基源通讯端口"
                                        , "Description", "时基源通讯端口,默认32"
                                        , "ClassName", "B端口信息配置"
                                        , "DataSource", ""));

            #endregion


            #region ----------表位RS485端口参数配置---------

            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                   , "Item", pwFunction.pwConst.Variable.CTC_RS485_COM1
                   , "Value", "1,193.168.18.1:10003:20000"
                   , "Name", "表位01"
                   , "Description", "通讯参数（格式为：串口号+IP地址和端口号）默认通讯IP为（1,193.168.18.1:10003:20000）。"
                   , "ClassName", "表位RS485端口"
                   , "DataSource", ""));
                
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                  , "Item", pwFunction.pwConst.Variable.CTC_RS485_COM2
                  , "Value", "2,193.168.18.1:10003:20000"
                  , "Name", "表位02"
                  , "Description", "通讯参数（格式为：串口号+IP地址和端口号）默认通讯IP为（2,193.168.18.1:10003:20000）。"
                  , "ClassName", "表位RS485端口"
                  , "DataSource", ""));

            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
               , "Item", pwFunction.pwConst.Variable.CTC_RS485_COM3
               , "Value", "3,193.168.18.1:10003:20000"
               , "Name", "表位03"
               , "Description", "通讯参数（格式为：串口号+IP地址和端口号）默认通讯IP为（3,193.168.18.1:10003:20000）。"
               , "ClassName", "表位RS485端口"
               , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
             , "Item", pwFunction.pwConst.Variable.CTC_RS485_COM4
             , "Value", "4,193.168.18.1:10003:20000"
             , "Name", "表位04"
             , "Description", "通讯参数（格式为：串口号+IP地址和端口号）默认通讯IP为（4,193.168.18.1:10003:20000）。"
             , "ClassName", "表位RS485端口"
             , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
               , "Item", pwFunction.pwConst.Variable.CTC_RS485_COM5
               , "Value", "5,193.168.18.1:10003:20000"
               , "Name", "表位05"
               , "Description", "通讯参数（格式为：串口号+IP地址和端口号）默认通讯IP为（5,193.168.18.1:10003:20000）。"
               , "ClassName", "表位RS485端口"
               , "DataSource", ""));

            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
               , "Item", pwFunction.pwConst.Variable.CTC_RS485_COM6
               , "Value", "6,193.168.18.1:10003:20000"
               , "Name", "表位06"
               , "Description", "通讯参数（格式为：串口号+IP地址和端口号）默认通讯IP为（6,193.168.18.1:10003:20000）。"
               , "ClassName", "表位RS485端口"
               , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
               , "Item", pwFunction.pwConst.Variable.CTC_RS485_COM7
               , "Value", "7,193.168.18.1:10003:20000"
               , "Name", "表位07"
               , "Description", "通讯参数（格式为：串口号+IP地址和端口号）默认通讯IP为（7,193.168.18.1:10003:20000）。"
               , "ClassName", "表位RS485端口"
               , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
               , "Item", pwFunction.pwConst.Variable.CTC_RS485_COM8
               , "Value", "8,193.168.18.1:10003:20000"
               , "Name", "表位08"
               , "Description", "通讯参数（格式为：串口号+IP地址和端口号）默认通讯IP为（8,193.168.18.1:10003:20000）。"
               , "ClassName", "表位RS485端口"
               , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
               , "Item", pwFunction.pwConst.Variable.CTC_RS485_COM9
               , "Value", "9,193.168.18.1:10003:20000"
               , "Name", "表位09"
               , "Description", "通讯参数（格式为：串口号+IP地址和端口号）默认通讯IP为（9,193.168.18.1:10003:20000）。"
               , "ClassName", "表位RS485端口"
               , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
               , "Item", pwFunction.pwConst.Variable.CTC_RS485_COM10
               , "Value", "10,193.168.18.1:10003:20000"
               , "Name", "表位10"
               , "Description", "通讯参数（格式为：串口号+IP地址和端口号）默认通讯IP为（10,193.168.18.1:10003:20000）。"
               , "ClassName", "表位RS485端口"
               , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
               , "Item", pwFunction.pwConst.Variable.CTC_RS485_COM11
               , "Value", "11,193.168.18.1:10003:20000"
               , "Name", "表位11"
               , "Description", "通讯参数（格式为：串口号+IP地址和端口号）默认通讯IP为（11,193.168.18.1:10003:20000）。"
               , "ClassName", "表位RS485端口"
               , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
               , "Item", pwFunction.pwConst.Variable.CTC_RS485_COM12
               , "Value", "12,193.168.18.1:10003:20000"
               , "Name", "表位12"
               , "Description", "通讯参数（格式为：串口号+IP地址和端口号）默认通讯IP为（12,193.168.18.1:10003:20000）。"
               , "ClassName", "表位RS485端口"
               , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
               , "Item", pwFunction.pwConst.Variable.CTC_RS485_COM13
               , "Value", "13,193.168.18.1:10003:20000"
               , "Name", "表位13"
               , "Description", "通讯参数（格式为：串口号+IP地址和端口号）默认通讯IP为（13,193.168.18.1:10003:20000）。"
               , "ClassName", "表位RS485端口"
               , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
               , "Item", pwFunction.pwConst.Variable.CTC_RS485_COM14
               , "Value", "14,193.168.18.1:10003:20000"
               , "Name", "表位14"
               , "Description", "通讯参数（格式为：串口号+IP地址和端口号）默认通讯IP为（14,193.168.18.1:10003:20000）。"
               , "ClassName", "表位RS485端口"
               , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
               , "Item", pwFunction.pwConst.Variable.CTC_RS485_COM15
               , "Value", "15,193.168.18.1:10003:20000"
               , "Name", "表位15"
               , "Description", "通讯参数（格式为：串口号+IP地址和端口号）默认通讯IP为（15,193.168.18.1:10003:20000）。"
               , "ClassName", "表位RS485端口"
               , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
               , "Item", pwFunction.pwConst.Variable.CTC_RS485_COM16
               , "Value", "16,193.168.18.1:10003:20000"
               , "Name", "表位16"
               , "Description", "通讯参数（格式为：串口号+IP地址和端口号）默认通讯IP为（16,193.168.18.1:10003:20000）。"
               , "ClassName", "表位RS485端口"
               , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
               , "Item", pwFunction.pwConst.Variable.CTC_RS485_COM17
               , "Value", "17,193.168.18.1:10003:20000"
               , "Name", "表位17"
               , "Description", "通讯参数（格式为：串口号+IP地址和端口号）默认通讯IP为（17,193.168.18.1:10003:20000）。"
               , "ClassName", "表位RS485端口"
               , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
               , "Item", pwFunction.pwConst.Variable.CTC_RS485_COM18
               , "Value", "18,193.168.18.1:10003:20000"
               , "Name", "表位18"
               , "Description", "通讯参数（格式为：串口号+IP地址和端口号）默认通讯IP为（18,193.168.18.1:10003:20000）。"
               , "ClassName", "表位RS485端口"
               , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
               , "Item", pwFunction.pwConst.Variable.CTC_RS485_COM19
               , "Value", "19,193.168.18.1:10003:20000"
               , "Name", "表位19"
               , "Description", "通讯参数（格式为：串口号+IP地址和端口号）默认通讯IP为（19,193.168.18.1:10003:20000）。"
               , "ClassName", "表位RS485端口"
               , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
               , "Item", pwFunction.pwConst.Variable.CTC_RS485_COM20
               , "Value", "20,193.168.18.1:10003:20000"
               , "Name", "表位20"
               , "Description", "通讯参数（格式为：串口号+IP地址和端口号）默认通讯IP为（20,193.168.18.1:10003:20000）。"
               , "ClassName", "表位RS485端口"
               , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
               , "Item", pwFunction.pwConst.Variable.CTC_RS485_COM21
               , "Value", "21,193.168.18.1:10003:20000"
               , "Name", "表位21"
               , "Description", "通讯参数（格式为：串口号+IP地址和端口号）默认通讯IP为（21,193.168.18.1:10003:20000）。"
               , "ClassName", "表位RS485端口"
               , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
               , "Item", pwFunction.pwConst.Variable.CTC_RS485_COM22
               , "Value", "22,193.168.18.1:10003:20000"
               , "Name", "表位22"
               , "Description", "通讯参数（格式为：串口号+IP地址和端口号）默认通讯IP为（22,193.168.18.1:10003:20000）。"
               , "ClassName", "表位RS485端口"
               , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
               , "Item", pwFunction.pwConst.Variable.CTC_RS485_COM23
               , "Value", "23,193.168.18.1:10003:20000"
               , "Name", "表位23"
               , "Description", "通讯参数（格式为：串口号+IP地址和端口号）默认通讯IP为（23,193.168.18.1:10003:20000）。"
               , "ClassName", "表位RS485端口"
               , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
               , "Item", pwFunction.pwConst.Variable.CTC_RS485_COM24
               , "Value", "24,193.168.18.1:10003:20000"
               , "Name", "表位24"
               , "Description", "通讯参数（格式为：串口号+IP地址和端口号）默认通讯IP为（24,193.168.18.1:10003:20000）。"
               , "ClassName", "表位RS485端口"
               , "DataSource", ""));








            #endregion
            */
            #endregion

            #region -----------台体信息配置-----------
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                                            , "Item", pwFunction.pwConst.Variable.CTC_DESKNO
                                            , "Value", "1"
                                            , "Name", "台体编号"
                                            , "Description", "台体编号，该编号直接影响数据存储及统计分析报表。"
                                            , "ClassName", "A台体信息配置"
                                            , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                                           , "Item", pwFunction.pwConst.Variable.CTC_DESKTYPE
                                           , "Value", "三相相检定台"
                                           , "Name", "台体类型"
                                           , "Description", "当前检定装置类型，是为生产全自动综合检定台体 \r\n三相台：(CL2018-1,CL309,CL3115,CL188L,CL191B)。"
                                           , "ClassName", "A台体信息配置"
                                           , "DataSource", "单相检定台|单相前装台|单相后装台|三相检定台|三相前装台|三相后装台"));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                                            , "Item", pwFunction.pwConst.Variable.CTC_BWCOUNT
                                            , "Value", "12"
                                            , "Name", "台体表位数"
                                            , "Description", "台体表位数，该数量应尽量和台架上表位数量一致。默认：检定台24表位，前装台3表位"
                                            , "ClassName", "A台体信息配置"
                                            , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                                            , "Item", pwFunction.pwConst.Variable.CTC_DESBWCOUNT
                                            , "Value", "4"
                                            , "Name", "每行显示灯泡个数"
                                            , "Description", "主界面灯泡每行显示灯泡个数，该数量应和挂台架上每行表位数量一致。"
                                            , "ClassName", "A台体信息配置"
                                            , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                                           , "Item", pwFunction.pwConst.Variable.CTC_ISCL191BCL188L
                                           , "Value", "是"
                                           , "Name", "台体是否有CL191B及CL188L"
                                           , "Description", "国网前装终合台体专用,默认是有CL191B及CL188L"
                                           , "ClassName", "A台体信息配置"
                                           , "DataSource", "是|否"));


            #endregion

            #region ----------误差检定配置------------
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                   , "Item", pwFunction.pwConst.Variable.CTC_WC_TIMES_BASICERROR
                   , "Value", "2"
                   , "Name", "误差计算取值数"
                   , "Description", "每个误差点取几次误差参与计算。也就是最小检定次数"
                   , "ClassName", "检定配置"
                   , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                      , "Item", pwFunction.pwConst.Variable.CTC_WC_TIMES_WINDAGE
                      , "Value", "5"
                      , "Name", "标准偏差计算取值数"
                      , "Description", "每个标准偏差取几次误差参与计算。"
                      , "ClassName", "检定配置"
                      , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                        , "Item", pwFunction.pwConst.Variable.CTC_WC_MAXTIMES
                        , "Value", "3"
                        , "Name", "最大处理次数"
                        , "Description", "每个误差点最大处理次数，如果需检偏差请确保此数值大于标准偏差次数。"
                        , "ClassName", "检定配置"
                        , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                        , "Item", pwFunction.pwConst.Variable.CTC_WC_MAXSECONDS
                        , "Value", "120"
                        , "Name", "最大处理时间(秒)"
                        , "Description", "每个误差点最大处理时间。"
                        , "ClassName", "检定配置"
                        , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                       , "Item", pwFunction.pwConst.Variable.CTC_WC_JUMP
                       , "Value", "1"
                       , "Name", "跳差判定倍数"
                       , "Description", "当二次误差值相差大于此倍数乘以表等级时，此点误差时此点不合格。"
                       , "ClassName", "检定配置"
                       , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                      , "Item", pwFunction.pwConst.Variable.CTC_WC_AVGPRECISION
                      , "Value", "4"
                      , "Name", "平均值小数位"
                      , "Description", "检定时误差平均值保留小数位数。"
                      , "ClassName", "检定配置"
                      , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                      , "Item", pwFunction.pwConst.Variable.CTC_WC_SYSTEMQS
                      , "Value", "否"
                      , "Name", "是否系统自动计算误差圈数"
                      , "Description", "是：系统自动计算圈数；否：使用方案圈数"
                      , "ClassName", "检定配置"
                      , "DataSource", "是|否"));

            #endregion


            #region ----------其它配置-----------
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                   , "Item", pwFunction.pwConst.Variable.CTC_OTHER_POWERON_ATTERTIME
                   , "Value", "5"
                   , "Name", "故障表数报警阀值(块)"
                   , "Description", "当故障表达超过限值后弹出报警界面。"
                   , "ClassName", "其它配置"
                   , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                   , "Item", pwFunction.pwConst.Variable.CTC_OTHER_POWERON_ATTERTIME
                   , "Value", "3"
                   , "Name", "标准源稳定时间(秒)"
                   , "Description", "升源稳定时间，一般根据升源指令结束后源能在几秒内达到源等级稳定度,单位（秒）。"
                   , "ClassName", "其它配置"
                   , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                    , "Item", pwFunction.pwConst.Variable.CTC_OTHER_MAXWAITDATABACKTIME
                    , "Value", "45"
                    , "Name", "读写表操作最大等待返回时间(秒)"
                    , "Description", "读写表操作后等待表返回的时间。建议不要太长。"
                    , "ClassName", "其它配置"
                    , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                    , "Item", pwFunction.pwConst.Variable.CTC_OTHER_EQUIPSET_WAITTIME
                    , "Value", "100"
                    , "Name", "参数设置时间间隔(毫秒)"
                    , "Description", "系统每次设置台体参数后的时间间隔，可根据硬件实际情况调整。太小可能会影响正常检定，太大则会影响检定速度。"
                    , "ClassName", "其它配置"
                    , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                    , "Item", pwFunction.pwConst.Variable.CTC_OTHER_RETRY
                    , "Value", "3"
                    , "Name", "操作重试次数(次)"
                    , "Description", "命令发送返回失败后操作重试次数，默认为3次。"
                    , "ClassName", "其它配置"
                    , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                   , "Item", pwFunction.pwConst.Variable.CTC_OTHER_DRIVERF
                   , "Value", "1"
                   , "Name", "标准分频系数"
                   , "Description", "标准表标准分频系统，请查阅标准表相关说明。"
                   , "ClassName", "其它配置"
                   , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                     , "Item", pwFunction.pwConst.Variable.CTC_READTABLENO
                     , "Value", "否"
                     , "Name", "是否扫描输入表号"
                     , "Description", "是：扫描输入；否：通信读取"
                     , "ClassName", "其它配置"
                     , "DataSource", "是|否"));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                              , "Item", pwFunction.pwConst.Variable.CTC_READTABLELENGTH
                              , "Value", "12"
                              , "Name", "电表表号长度"
                              , "Description", "固定电表长度，给与扫描判断是否调至下一个输入框"
                              , "ClassName", "其它配置"
                              , "DataSource", ""));
            #endregion

            #region ------------网络数据服务器信息配置-----------
            /*网络数据库服务器配置
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                                      , "Item", pwFunction.pwConst.Variable.CTC_SQL_SERVERIP
                                      , "Value", "10.98.99.6"
                                      , "Name", "网络数据库服务IP"
                                      , "Description", "SQL数据库服务器IP地址或是主机名，请勿擅自更改。"
                                      , "ClassName", "网络数据服器信息配置"
                                      , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                                     , "Item", pwFunction.pwConst.Variable.CTC_SQL_DATABASENAME
                                     , "Value", "OrBitXI"
                                     , "Name", "数据库名"
                                     , "Description", "SQL数据库名，请勿擅自更改。"
                                     , "ClassName", "网络数据服器信息配置"
                                     , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                                      , "Item", pwFunction.pwConst.Variable.CTC_SQL_USERID
                                      , "Value", "sa"
                                      , "Name", "网络数据库用户ID"
                                      , "Description", "登录SQL数据库的用户名，一般由系统管理员分配，默认为SA，请勿擅自更改！"
                                      , "ClassName", "网络数据服器信息配置"
                                      , "DataSource", ""));
            xml.AppendChild(clsXmlControl.CreateXmlNode("R"
                                                , "Item", pwFunction.pwConst.Variable.CTC_SQL_PASSWORD
                                                , "Value", "mes123"
                                                , "Name", "网络数据库登录密码"
                                                , "Description", "登录SQL数据库的密码，请勿擅自更改！"
                                                , "ClassName", "网络数据服器信息配置"
                                                , "DataSource", ""));


            /**/
            #endregion

        }

    }
}
