using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using pwClassLibrary.DataBase;
using pwInterface;

namespace pwFunction.pwGlysModel
{
    /// <summary>
    /// 功率因素字典
    /// </summary>
    public class csGlys
    {
        /// <summary>
        /// 功率因素字典
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> _GlysDic;
        
        /// <summary>
        /// 功率因素ID对照字典
        /// </summary>
        private Dictionary<string, string> _GlysTable;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public csGlys()
        {
            _GlysDic = new Dictionary<string, Dictionary<string, string>>();
            _GlysTable = new Dictionary<string, string>();
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~csGlys()
        {
            _GlysDic = null;
            _GlysTable = null;
        }

        /// <summary>
        /// 加载功率因素字典
        /// </summary>
        public void Load()
        {
            string _ErrorString = "";
            _GlysDic.Clear();    ///清除字典信息集合
            _GlysTable.Clear();
            XmlNode _XmlNode = clsXmlControl.LoadXml(Application.StartupPath + pwFunction.pwConst.Variable.CONST_GLYSDICTIONARY, out _ErrorString);
            if (_ErrorString != "")
            {
                #region 初始化功率因素字典表
                _XmlNode = clsXmlControl.CreateXmlNode("GLYSGroup","Count","7");
                XmlNode _XmlChildNode = clsXmlControl.CreateXmlNode("R", "Name", "1.0","ID","01");  //功率因素1.0
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "001", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "002", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "003", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "004", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "011", "90"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "012", "90"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "013", "90"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "014", "90"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "101", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "102", "330"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "103", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "104", "30"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "111", "90"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "112", "60"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "113", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "114", "120"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "211", "90"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "212", "60"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "213", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "214", "120"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "311", "90"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "312", "60"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "313", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "314", "120"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "411", "90"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "412", "60"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "413", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "414", "120"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "501", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "502", "90"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "503", "90"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "504", "90"));
                _XmlNode.AppendChild(_XmlChildNode);

                _XmlChildNode = clsXmlControl.CreateXmlNode("R", "Name", "0.5L", "ID", "02");  //功率因素0.5L
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "001", "60"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "002", "60"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "003", "60"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "004", "60"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "011", "30"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "012", "30"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "013", "30"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "014", "30"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "101", "60"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "102", "30"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "103", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "104", "90"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "111", "30"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "112", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "113", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "114", "60"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "211", "30"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "212", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "213", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "214", "60"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "311", "30"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "312", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "313", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "314", "60"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "411", "30"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "412", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "413", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "414", "60"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "501", "60"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "502", "30"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "503", "30"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "504", "30"));
                _XmlNode.AppendChild(_XmlChildNode);

                _XmlChildNode = clsXmlControl.CreateXmlNode("R", "Name", "0.8C", "ID", "03");  //功率因素0.8C
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "001", "330"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "002", "330"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "003", "330"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "004", "330"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "011", "120"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "012", "120"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "013", "120"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "014", "120"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "101", "330"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "102", "300"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "103", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "104", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "111", "120"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "112", "90"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "113", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "114", "150"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "211", "120"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "212", "90"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "213", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "214", "150"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "311", "120"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "312", "90"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "313", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "314", "150"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "411", "120"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "412", "90"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "413", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "414", "150"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "501", "330"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "502", "120"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "503", "120"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "504", "120"));
                _XmlNode.AppendChild(_XmlChildNode);

                _XmlChildNode = clsXmlControl.CreateXmlNode("R", "Name", "0.5C", "ID", "04");  //功率因素0.5C
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "001", "300"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "002", "300"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "003", "300"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "004", "300"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "011", "150"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "012", "150"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "013", "150"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "014", "150"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "101", "300"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "102", "270"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "103", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "104", "330"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "111", "150"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "112", "120"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "113", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "114", "180"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "211", "150"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "212", "120"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "213", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "214", "180"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "311", "150"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "312", "120"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "313", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "314", "180"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "411", "150"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "412", "120"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "413", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "414", "180"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "501", "300"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "502", "150"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "503", "150"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "504", "150"));
                _XmlNode.AppendChild(_XmlChildNode);

                _XmlChildNode = clsXmlControl.CreateXmlNode("R", "Name", "0.8L", "ID", "05");  //功率因素0.8L
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "001", "30"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "002", "30"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "003", "30"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "004", "30"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "011", "60"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "012", "60"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "013", "60"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "014", "60"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "101", "30"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "102", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "103", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "104", "60"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "111", "60"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "112", "30"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "113", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "114", "90"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "211", "60"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "212", "30"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "213", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "214", "90"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "311", "60"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "312", "30"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "313", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "314", "90"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "411", "60"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "412", "30"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "413", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "414", "90"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "501", "30"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "502", "30"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "503", "90"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "504", "90"));
                _XmlNode.AppendChild(_XmlChildNode);

                _XmlChildNode = clsXmlControl.CreateXmlNode("R", "Name", "0.25L", "ID", "06");  //功率因素0.25L
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "001", "75.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "002", "75.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "003", "75.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "004", "75.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "011", "14.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "012", "14.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "013", "14.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "014", "14.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "101", "75.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "102", "45.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "103", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "104", "105.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "111", "14.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "112", "334.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "113", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "114", "44.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "211", "14.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "212", "334.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "213", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "214", "44.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "311", "14.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "312", "334.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "313", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "314", "44.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "411", "14.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "412", "334.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "413", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "414", "44.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "501", "75.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "502", "14.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "503", "14.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "504", "14.5"));
                _XmlNode.AppendChild(_XmlChildNode);


                _XmlChildNode = clsXmlControl.CreateXmlNode("R", "Name", "0.25C", "ID", "07");  //功率因素0.25C
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "001", "284.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "002", "284.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "003", "284.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "004", "284.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "011", "165.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "012", "165.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "013", "165.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "014", "165.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "101", "284.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "102", "254.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "103", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "104", "314.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "111", "165.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "112", "135.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "113", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "114", "195.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "211", "165.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "212", "135.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "213", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "214", "195.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "311", "165.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "312", "135.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "313", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "314", "195.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "411", "165.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "412", "135.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "413", "0"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "414", "195.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "501", "284.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "502", "284.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "503", "14.5"));
                _XmlChildNode.AppendChild(clsXmlControl.CreateXmlNode("C", "ID", "504", "14.5"));
                _XmlNode.AppendChild(_XmlChildNode);

                clsXmlControl.SaveXml(_XmlNode, Application.StartupPath + pwFunction.pwConst.Variable.CONST_GLYSDICTIONARY);
                #endregion
            }
            for (int _i = 0; _i < _XmlNode.ChildNodes.Count; _i++)
            {
                Dictionary<string,string> _Values = new Dictionary<string,string>();
                for (int _j = 0; _j < _XmlNode.ChildNodes[_i].ChildNodes.Count; _j++)
                {
                    _Values.Add(_XmlNode.ChildNodes[_i].ChildNodes[_j].Attributes["ID"].Value,_XmlNode.ChildNodes[_i].ChildNodes[_j].InnerText);          ///获取字典值
                }
                _GlysDic.Add(_XmlNode.ChildNodes[_i].Attributes["Name"].Value, _Values);       ///获取功率因素字典属性值
                _GlysTable.Add(_XmlNode.ChildNodes[_i].Attributes["Name"].Value, _XmlNode.ChildNodes[_i].Attributes["ID"].Value);
            }
            return;
        }
        /// <summary>
        /// 存储功率因素字典
        /// </summary>
        public void Save()
        {
            clsXmlControl _Xml = new clsXmlControl();
            if(_GlysDic.Count==0)
                return;
            _Xml.appendchild("","GlYSGroup");
            foreach (string _Name in _GlysDic.Keys)
            {
                _Xml.appendchild("","R", "Name", _Name, "ID" ,_GlysTable[_Name]);           //<GLYSGroup Count=_GlysDic.Keys><R Name="_Name" ID ="_GlysTable[_Name]"/></GLYSGroup>
                foreach (string _ID in _GlysDic[_Name].Keys)
                {
                    _Xml.appendchild(clsXmlControl.XPath("R,Name," + _Name), "C", "ID", _ID, _GlysDic[_Name][_ID]); //<GLYSGroup Count=_GlysDic.Keys>
                                                                                                                    //<R Name="_Name" ID ="_GlysTable[_Name]"/>
                                                                                                                    //<C ID ="_ID" >_GlysDic[_Name][_ID]</C>
                                                                                                                    //</GLYSGroup>
                }
            }
            _Xml.SaveXml(Application.StartupPath + pwFunction.pwConst.Variable.CONST_GLYSDICTIONARY);
        
        }

        /// <summary>
        /// 增加功率因素
        /// </summary>
        /// <param name="GlysName">功率因素名称</param>
        /// <returns></returns>
        public bool Add(string GlysName)
        {
            if (_GlysDic.Count == 0)
                return false;
            if (_GlysDic.ContainsKey(GlysName))
                return false;
            int _GlysID=0;
            for(int Int_I=1;Int_I<=99;Int_I++)         //获取功率因素对应的唯一ID值
            {
                if(!_GlysTable.ContainsValue(Int_I.ToString("d2")))
                {    
                    _GlysID=Int_I;
                    break;
                }
            }
            _GlysTable.Add(GlysName, _GlysID.ToString("d2"));         //功率因素ID对照字典

            Dictionary<string, string> _GlysPram = new Dictionary<string, string>();
            for (int _i = 0; _i <= 5; _i++)    //测量方式
            {
                for (int _j = 0; _j <= 1; _j++)     //有无功
                {
                    if ((_i > 1 && _i <= 4 && _j==0) || (_i==5 && _j==1))      //2元件，3元件没有有功，单相没有无功
                        continue;
                    for (int _z = 1; _z <= 4; _z++)     //元件
                    { 
                        string _ID=_i.ToString()+_j.ToString()+_z.ToString();
                        _GlysPram.Add(_ID, "0"); 
                    }
                }
            }

            _GlysDic.Add(GlysName, _GlysPram);
            return true;
        }
        /// <summary>
        /// 获取角度值
        /// </summary>
        /// <param name="GLys">功率因素名称</param>
        /// <param name="XID">标志ID组合值，调用静态函数XID获取</param>
        /// <returns>角度</returns>
        public string getJiaoDu(string GLys,string XID)
        {
            if (!_GlysDic.ContainsKey(GLys))
                return "";
            if (!_GlysDic[GLys].ContainsKey(XID))
                return "";
            return _GlysDic[GLys][XID];
        }
        /// <summary>
        /// 获取功率因素对应角度集合
        /// </summary>
        /// <param name="Glys">功率因素名称</param>
        /// <returns></returns>
        public Dictionary<string, string> getJiaoDu(string Glys)
        {
            if (!_GlysDic.ContainsKey(Glys))
                return new Dictionary<string,string>();
            return _GlysDic[Glys];
        }

        /// <summary>
        /// 返回功率因素名称列表
        /// </summary>
        /// <returns></returns>
        public List<string> getGlysName()
        {
            List<string> _Names = new List<string>();
            foreach (string _n in _GlysDic.Keys)
                _Names.Add(_n);
            return _Names;
        }

        /// <summary>
        /// 修改一个角度值
        /// </summary>
        /// <param name="Glys">要修改的角度值的功率因素</param>
        /// <param name="XID">XID</param>
        /// <param name="sValue">角度值</param>
        public void EditJiaoDu(string Glys,string XID,string sValue)
        {
            if (!_GlysDic.ContainsKey(Glys))
                return ;
            if (!_GlysDic[Glys].ContainsKey(XID))
                return ;
            _GlysDic[Glys][XID]=sValue;
            return;
        }

        /// <summary>
        /// 修改一个角度值
        /// </summary>
        /// <param name="Glys">要修改的角度值的功率因素</param>
        /// <param name="XID">XID</param>
        /// <param name="sValue">角度值</param>
        /// <param name="SaveNow">是否及时存档</param>
        public void EditJiaoDu(string Glys, string XID, string sValue, bool SaveNow)
        {
            this.EditJiaoDu(Glys, XID, sValue);
            if (SaveNow)
                this.Save();
            return;
        }
        /// <summary>
        /// 修改一个功率因素的所有角度值
        /// </summary>
        /// <param name="Glys">功率因素</param>
        /// <param name="sValues">角度集合</param>
        public void EditJiaoDu(string Glys, Dictionary<string, string> sValues)
        {
            if (!_GlysDic.ContainsKey(Glys))
                return;
            _GlysDic[Glys] = sValues;
            return;
        }
        /// <summary>
        /// 修改一个功率因素的所有角度值
        /// </summary>
        /// <param name="Glys">功率因素</param>
        /// <param name="sValues">角度集合</param>
        /// <param name="SaveNow">是否立即存档</param>
        public void EditJiaoDu(string Glys, Dictionary<string, string> sValues, bool SaveNow)
        {
            this.EditJiaoDu(Glys, sValues);
            if(SaveNow)
                this.Save();
            return;
        }

        /// <summary>
        /// 获取功率因素ID值
        /// </summary>
        /// <param name="Glys">功率因素名称</param>
        /// <returns></returns>
        public string getGlysID(string Glys)
        {
            foreach (string Key in _GlysTable.Keys)
            {
                if (Key.ToLower() == Glys.ToLower())
                {
                    return _GlysTable[Key];
                }

            }
            return "";
        }


        /// <summary>
        /// 移除一个功率因素
        /// </summary>
        /// <param name="Glys">功率因素名称</param>
        /// <returns>成功或失败</returns>
        public bool Remove(string Glys)
        {
            if (!_GlysDic.ContainsKey(Glys) || int.Parse(_GlysTable[Glys]) <= 7)
                return false;
            _GlysTable.Remove(Glys);
            _GlysDic.Remove(Glys);
            return true;
        }
        /// <summary>
        /// 移除一个功率因素
        /// </summary>
        /// <param name="Glys">功率因素名称</param>
        /// <param name="SaveNow">是否马上存档</param>
        /// <returns></returns>
        public bool Remove(string Glys, bool SaveNow)
        {
            bool _Save=this.Remove(Glys);
            if (_Save == false)
                return false;
            if (SaveNow)
                this.Save();
            return true;
        }

        /// <summary>
        /// 获取XID的值
        /// </summary>
        /// <param name="clfs">测量方式</param>
        /// <param name="Ywg">有无功</param>
        /// <param name="Yj">元件</param>
        /// <returns></returns>
        public static string XID(enmClfs clfs, enmYwg Ywg, enmElement Yj)
        {
            return ((int)clfs).ToString() + ((int)Ywg).ToString() + ((int)Yj).ToString();
        }
    }


}
