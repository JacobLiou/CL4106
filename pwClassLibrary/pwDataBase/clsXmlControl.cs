using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace pwClassLibrary.DataBase
{
    public class clsXmlControl
    {
        /// <summary>
        /// XML文档路径
        /// </summary>
        private static XmlDocument _XmlDom = new XmlDocument();

        private XmlNode _XmlNode;

        private string _XmlFilePath = "";

        /// <summary>
        /// 空的构造函数，不做任何操作
        /// </summary>
        public clsXmlControl()
        {
  
        }
        /// <summary>
        /// 获取XML文件主节点
        /// </summary>
        /// <param name="XmlFilePath"></param>
        public clsXmlControl(string XmlFilePath)
        {
            string _ErrString;
            _XmlFilePath = XmlFilePath;
            _XmlNode = clsXmlControl.LoadXml(XmlFilePath, out _ErrString);

        }

        ///// <summary>
        ///// 在总节点上追加节点
        ///// </summary>
        ///// <param name="NodeName"></param>
        ///// <param name="Arrlist"></param>
        ///// <returns></returns>
        //public bool appendchild(string NodeName, params string[] ArrList)
        //{
        //    if (_XmlNode == null)
        //        _XmlNode = CreateXmlNode(NodeName, ArrList);
        //    else
        //        _XmlNode.AppendChild(CreateXmlNode(NodeName, ArrList));
        //    return true;
        //}

        /// <summary>
        /// 重载追加节点，在总节点下指定子节点进行追加
        /// </summary>
        /// <param name="QueryString">查询节点表达式</param>
        /// <param name="NodeName">节点名称</param>
        /// <param name="ArrList">变体带属性或节点值</param>
        /// <returns></returns>
        public bool appendchild(string QueryString, string NodeName, params string[] ArrList)
        {
            if (_XmlNode == null)
                _XmlNode = CreateXmlNode(NodeName, ArrList);
            else
            {
                XmlNode _FindNode = FindSencetion(_XmlNode, QueryString);
                if (_FindNode == null)
                    return false;
                
                _FindNode.AppendChild(CreateXmlNode(NodeName, ArrList));
            }
            return true;
        }


        /// <summary>
        /// 追加节点
        /// </summary>
        /// <param name="NodeItem">子节点对象</param>
        /// <returns></returns>
        public bool appendchild(XmlNode NodeItem)
        {
            if (_XmlNode == null)
            {
                return false;
            }
            _XmlNode.AppendChild(NodeItem);
            return true;
        }

        public XmlNode appendchild(bool Return, string NodeName, params string[] ArrList)
        {
            if (_XmlNode == null)
            {
                _XmlNode = CreateXmlNode(NodeName, ArrList);
                return _XmlNode;
            }
            else
            {
                XmlNode _FindNode = FindSencetion(_XmlNode, "");
                if (_FindNode == null)
                    return null;

                return _FindNode.AppendChild(CreateXmlNode(NodeName, ArrList));
            }
        }

        /// <summary>
        /// 获取总节点XML字符串
        /// </summary>
        /// <returns></returns>
        public string toXmlString()
        {
            return _XmlNode.OuterXml;
        }

         
        /// <summary>
        /// 返回成XML节点对象
        /// </summary>
        /// <returns></returns>
        public XmlNode toXmlNode()
        {
            return _XmlNode;
        }

        /// <summary>
        /// 保存XML文档，覆盖打开的文档
        /// </summary>
        /// <returns></returns>
        public bool SaveXml()
        {
            if (_XmlFilePath == "")
                return false;
            SaveXml(_XmlNode, _XmlFilePath);
            return true;
        }
        /// <summary>
        /// 保存XML文档，另存为新的文档
        /// </summary>
        /// <param name="SavePath">文档存储路径</param>
        /// <returns></returns>
        public bool SaveXml(string SavePath)
        {
            if (SavePath == "")
                return false;
            
            SaveXml(_XmlNode, SavePath);
            return true;
        }
        /// <summary>
        /// 删除一个子结点
        /// </summary>
        /// <param name="QueryString">XPath查询表达式</param>
        public void RemoveChild(string QueryString)
        {
            _XmlNode = RemoveChildNode(_XmlNode, QueryString);
        }


        /// <summary>
        /// 返回子结点个数
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            
            if(_XmlNode==null)
                return (int)0;
            return _XmlNode.ChildNodes.Count;
        }
        /// <summary>
        /// 返回查询到的节点子节点个数
        /// </summary>
        /// <param name="QueryString">XPath查询表达式</param>
        /// <returns></returns>
        public int Count(string QueryString)
        {
            XmlNode _XmlFind = clsXmlControl.FindSencetion(_XmlNode, QueryString);
            if (_XmlFind == null)
                return (int)0;
            return _XmlFind.ChildNodes.Count;
        }

        /// <summary>
        /// 查询主节点值
        /// </summary>
        /// <returns></returns>
        public string Value()
        {
            return getNodeValue(_XmlNode);
        }
        /// <summary>
        /// 主节点下查询到的节点
        /// </summary>
        /// <param name="QueryString"></param>
        /// <returns></returns>
        public string Value(string QueryString)
        {
            return getNodeValue(_XmlNode, QueryString);
        }

        /// <summary>
        /// 移除主节点属性
        /// </summary>
        /// <param name="RemoveAttributeName">属性</param>
        /// <returns></returns>
        public bool RemoveAtribute(string RemoveAttributeName)
        {
            return RemoveAttribute(_XmlNode, RemoveAttributeName);
        }
        /// <summary>
        /// 移除主节点下查询到的子节点属性
        /// </summary>
        /// <param name="RemoveAttributeName">属性</param>
        /// <param name="QueryString">XPath查询表达式</param>
        /// <returns></returns>
        public bool RemoveAtribute(string RemoveAttributeName, string QueryString)
        {
            return RemoveAttribute(_XmlNode, RemoveAttributeName, QueryString);
        }


        /// <summary>
        /// 获取主节点对应子节点属性值
        /// </summary>
        /// <param name="QueryString">XPath查询表达式，为空的时候表示查询主节点</param>
        /// <param name="AttributeName">属性名称</param>
        /// <returns></returns>
        public string AttributeValue(string QueryString, string AttributeName)
        {
            XmlNode _XmlFindXml;
            if (QueryString != "")
            {
                _XmlFindXml = FindSencetion(_XmlNode, QueryString);
                if (_XmlFindXml == null)
                    return "";

            }
            else
                _XmlFindXml = _XmlNode;
            return getNodeAttributeValue(_XmlFindXml, AttributeName);
        }
        /// <summary>
        /// 修改查询到的节点的对应属性值
        /// </summary>
        /// <param name="QueryString">XPath查询表达式，为空的时候表示查询主节点</param>
        /// <param name="Values">格式：属性名称|属性值</param>
        /// <returns>修改成功或失败，失败就表示不存在该属性或节点</returns>
        public bool EditAttibuteValue(string QueryString, params string[] Values)
        {
            return clsXmlControl.EditNodeAttributeValues(QueryString, _XmlNode, Values);
        }
        /// <summary>
        /// 获取主节点对应子节点多个并列属性值
        /// </summary>
        /// <param name="QueryString">XPath查询表达式，为空的时候表示查询主节点</param>
        /// <param name="ArrList">并列的属性名称</param>
        /// <returns>属性值</returns>
        public string[] AttributeValue(string QueryString,params string[] ArrList)
        {
            XmlNode _XmlFindXml;
            string[] _ReturnValues=new string[ArrList.Length];
            if (QueryString != "")
            {
                _XmlFindXml = FindSencetion(_XmlNode, QueryString);
                if (_XmlFindXml == null)
                    return _ReturnValues;
            }
            else
                _XmlFindXml = _XmlNode;
            for (int _i = 0; _i < _ReturnValues.Length; _i++)
            {
                _ReturnValues[_i] = getNodeAttributeValue(_XmlFindXml, ArrList[_i]);
            }
            return _ReturnValues;
        }

        /// <summary>
        /// 修改节点的对应属性值 
        /// </summary>
        /// <param name="QuertString">XPath查询表达式，为空的时候表示当前节点</param>
        /// <param name="XmlInNode">需要修改属性的节点对象</param>
        /// <param name="Values">格式：属性名称|属性值</param>
        /// <returns>修改成功或失败，失败就表示不存在该属性或节点</returns>
        public static bool EditNodeAttributeValues(string QuertString, XmlNode XmlInNode, params string[] Values)
        {
            XmlNode _XmlFindNode = FindSencetion(XmlInNode, QuertString);

            if (_XmlFindNode == null)
                return false;

            for (int _i = 0; _i < Values.Length; _i++)
            {
                string[] _Value = Values[_i].Split('|');
                if (_Value.Length == 0)
                    return false;
                XmlAttribute _FindAtt = _XmlFindNode.Attributes[_Value[0]];
                if (_FindAtt == null)
                    return false;
                _FindAtt.Value=_Value[1];
            }
            return true;
        }

        /// <summary>
        /// 获取当前节点属性值
        /// </summary>
        /// <param name="XmlInNode">需要获取属性值的节点</param>
        /// <param name="AttributeName">对应节点属性名称</param>
        /// <returns></returns>
        public static string getNodeAttributeValue(XmlNode XmlInNode, string AttributeName)
        {
            XmlAttribute _FindAtt = XmlInNode.Attributes[AttributeName];
            if (_FindAtt == null)
                return "";
            _FindAtt = null;
            return XmlInNode.Attributes[AttributeName].Value;
        }
        /// <summary>
        /// 获取当前节点属性值
        /// </summary>
        /// <param name="XmlInNode">需要获取属性值的节点</param>
        /// <param name="QuertString">XPath子结点查询表达式</param>
        /// <param name="AttributeName">对应节点属性名称</param>
        /// <returns></returns>
        public static string getNodeAttributeValue(XmlNode XmlInNode, string QuertString, string AttributeName)
        {
            XmlNode _FindXmlNode = FindSencetion(XmlInNode, QuertString);
            if (_FindXmlNode == null)
                return "";
            return getNodeAttributeValue(_FindXmlNode, AttributeName);
        }

        /// <summary>
        /// 移除当前节点属性
        /// </summary>
        /// <param name="XmlInNode">需要移除属性的节点</param>
        /// <param name="RemoveAttributeName">需要移除的属性名称</param>
        /// <returns></returns>
        public static bool RemoveAttribute(XmlNode XmlInNode, string RemoveAttributeName)
        {
            if (XmlInNode.Attributes[RemoveAttributeName] == null)
                return false;
            XmlInNode.Attributes.RemoveNamedItem(RemoveAttributeName);
            return true;
        }
        /// <summary>
        /// 移除主节点下查询到的子节点的属性
        /// </summary>
        /// <param name="XmlInNode">主节点对象</param>
        /// <param name="QueryString">XPath查询表达式</param>
        /// <param name="RemoveAttributeName">需要移除的属性名称</param>
        /// <returns></returns>
        public static bool RemoveAttribute(XmlNode XmlInNode, string QueryString, string RemoveAttributeName)
        {
            XmlNode _XmlFind = FindSencetion(XmlInNode, QueryString);
            if (_XmlFind == null)
                return false;
            else
                return RemoveAttribute(_XmlFind, RemoveAttributeName);
        }

        /// <summary>
        /// 获取当前节点值
        /// </summary>
        /// <param name="XmlFindNode">当前节点对象</param>
        /// <returns></returns>
        public static string getNodeValue(XmlNode XmlFindNode)
        {
            if (XmlFindNode == null)
                return "";
            else
                return XmlFindNode.ChildNodes[0].Value;
        }
        /// <summary>
        /// 获取需要查询的子节点值
        /// </summary>
        /// <param name="XmlFatherNode">父域节点</param>
        /// <param name="QueryString">XPath查询表达式</param>
        /// <returns></returns>
        public static string getNodeValue(XmlNode XmlFatherNode, string QueryString)
        {
            return getNodeValue(FindSencetion(XmlFatherNode, QueryString));
        }

        /// <summary>
        /// 保存XML文档
        /// </summary>
        /// <param name="SaveNode">需要保存的XML节点</param>
        /// <param name="SaveFilePath">存储路径</param>
        public static void SaveXml(XmlNode SaveNode, string SaveFilePath)
        {
            System.IO.FileStream _TmpStream = pwClassLibrary.pwFile.File.Create(SaveFilePath);
            if (_TmpStream != null)
            {
                _TmpStream.Close();
            }
            
            XmlWriter _Writer = XmlWriter.Create(SaveFilePath);

            SaveNode.WriteTo(_Writer);          //写入文件
            _Writer.Flush();
            _Writer.Close();
        }

        /// <summary>
        /// 查询一个节点对象
        /// </summary>
        /// <param name="XmlFatherNode">父域节点对象</param>
        /// <param name="QueryString">XPath查询表达式</param>
        /// <returns></returns>
        public static XmlNode FindSencetion(XmlNode XmlFatherNode, string QueryString)
        {
            if (QueryString == "")
                return XmlFatherNode;
            try
            {
                return XmlFatherNode.SelectSingleNode(QueryString);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 移除子结点
        /// </summary>
        /// <param name="XmlFatherNode">父域节点</param>
        /// <param name="QueryString">XPath查询表达式</param>
        /// <returns></returns>
        public static XmlNode RemoveChildNode(XmlNode XmlFatherNode, string QueryString)
        {
            if (QueryString == "")
                return XmlFatherNode;
            XmlNode _DeleteNode = XmlFatherNode.SelectSingleNode(QueryString);
            if (_DeleteNode == null)
                return XmlFatherNode;
            return RemoveChildNode(XmlFatherNode, _DeleteNode);
            
        }
        /// <summary>
        /// 移除子结点
        /// </summary>
        /// <param name="XmlFatherNode">父域节点</param>
        /// <param name="XmlDeleteNode">需要查询的子结点</param>
        /// <returns></returns>
        public static XmlNode RemoveChildNode(XmlNode XmlFatherNode, XmlNode XmlDeleteNode)
        {
            XmlFatherNode.RemoveChild(XmlDeleteNode);
            return XmlFatherNode;
        }

        /// <summary>
        /// 创建一个XML节点
        /// </summary>
        /// <param name="NodeName">节点名称</param>
        /// <param name="ArrList">List单数为节点属性，双数为节点属性值，List长度为单数则具有节点值节点值</param>
        /// <returns></returns>
        public static XmlNode CreateXmlNode(string NodeName, params string[] ArrList)
        {

            XmlNode _XmlNewNode = _XmlDom.CreateNode(XmlNodeType.Element, NodeName, "");
            if (ArrList.Length == 0)
                return _XmlNewNode;
            if (ArrList.Length % 2 == 0)
            {
                for (int Int_i = 0; Int_i < ArrList.Length; Int_i += 2)
                {
                    XmlAttribute _XmlAtb = _XmlDom.CreateAttribute(ArrList[Int_i]);
                    _XmlAtb.Value = ArrList[Int_i + 1].ToString();
                    _XmlNewNode.Attributes.SetNamedItem(_XmlAtb);
                }
            }
            else
            {
                for (int int_i = 0; int_i < ArrList.Length - 1; int_i += 2)
                {
                    XmlAttribute _XmlAtb = _XmlDom.CreateAttribute(ArrList[int_i]);
                    _XmlAtb.Value = ArrList[int_i + 1];
                    _XmlNewNode.Attributes.SetNamedItem(_XmlAtb);
                }
                _XmlNewNode.AppendChild(_XmlDom.CreateCDataSection(ArrList[ArrList.Length - 1]));
            }
            return _XmlNewNode;
        }
        /// <summary>
        /// 加载XML文档
        /// </summary>
        /// <param name="XmlFilePath">XML文档路径</param>
        /// <param name="ErrString">错误信息返回</param>
        /// <returns></returns>
        public static XmlNode LoadXml(string XmlFilePath, out string ErrString)
        {
            if (System.IO.File.Exists(XmlFilePath) == false)
            {
                ErrString = "未找到XML文档...";
                return null;
            }
            else
                ErrString = "";
            try
            {
                _XmlDom.Load(XmlFilePath);
                return _XmlDom.ChildNodes[1];
            }
            catch (Exception e)
            {
                ErrString = e.Message;
                return null;
            }
        }
        /// <summary>
        /// 返回XPath节点查询表达式
        /// </summary>
        /// <param name="XPathString">格式为1,1-1,1-2,...,1-n|2,2-1,...,2-n，1，2为节点名称，1-1(节点属性名),1-2(属性值)类推为节点属性名,属性值</param>
        /// <returns></returns>
        public static string XPath(string XPathString)
        {
            string _XPath = "";
            string[] _TmpPathPram = XPathString.Split('|');   //分割获取需要查询嵌套几层的子节点

            for (int int_i = 0; int_i < _TmpPathPram.Length; int_i++)
            {
                string[] _TmpNodePramString = _TmpPathPram[int_i].Split(',');        //分割一个节点的表达式内容
                if (_TmpNodePramString.Length % 2 == 0)   //如果长度是2的整数倍，则表达式有错误
                    return "";          //直接返回空
                if (_XPath == "")
                {
                    _XPath += string.Format("{0}", _TmpNodePramString[0]);                  //组合表达式
                }
                else
                    _XPath += string.Format("//{0}", _TmpNodePramString[0]);                  //组合表达式
                for (int int_j = 1; int_j < _TmpNodePramString.Length; int_j += 2)
                {
                    _XPath += string.Format("[@{0}='{1}']", _TmpNodePramString[int_j], _TmpNodePramString[int_j + 1]);   //组合表达式属性

                }
            } 
            return _XPath;
        }
    }
}