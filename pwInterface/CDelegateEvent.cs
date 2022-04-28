using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pwInterface
{
    /// <summary>
    /// 表操作作业委托
    /// </summary>
    /// <param name="enm_PrjID">项目ID</param>
    /// <param name="int_Index">表位</param>
    /// <param name="bln_Result">返回结果</param>
    /// <param name="str_Value">返回值</param>
    public delegate void DelegateEventMultiController(enmMeterPrjID enm_PrjID, int int_Index, bool bln_Result, string str_Value);

    /// <summary>
    /// 状态改变事件
    /// </summary>
    /// <param name="?"></param>
    public delegate void DelegateEventStatusChange(int Bw, enmStatus Status);//状态改变事件--->灯炮改变

    /// <summary>
    /// 结果改变
    /// </summary>
    /// <param name="Bw">表位</param>
    /// <param name="Checked">是否合格</param>
    public delegate void DelegateEventResuleChange(int Bw, bool Resule);

    /// <summary>
    /// 要检改变
    /// </summary>
    /// <param name="Bw">表位</param>
    /// <param name="Checked">要检</param>
    public delegate void DelegateEventChenkedChange(int Bw, bool Checked);

    /// <summary>
    /// 输入框条形码改变
    /// </summary>
    /// <param name="Bw"></param>
    /// <param name="strTxm"></param>
    public delegate void DelegateEventTxmChange(int Bw, string strTxm);

    /// <summary>
    /// 输入框条形码改变
    /// </summary>
    /// <param name="Bw"></param>
    /// <param name="strTxm"></param>
    public delegate void DelegateEventTxmtextSet(int Bw, string strTxm);

    /// <summary>
    /// 检定项目改变
    /// </summary>
    /// <param name="strPrjName">项目名称</param>
    public delegate void DelegateEventItemChange(string strPrjName);

    /// <summary>
    /// 显示表详细数据
    /// </summary>
    /// <param name="Bw">表位</param>
    public delegate void DelegateEventBwChange(int Bw);

    /// <summary>
    /// 显示监视器事件
    /// </summary>
    /// <param name="tagPower"></param>
    public delegate void OnShowMonitor(stPower tagPower);

    /// <summary>
    /// 标准表读取事件
    /// </summary>
    /// <param name="strInfo">标准表数据信息</param>
    public delegate void OnEventReadStdInfo(stPower strInfo);


    /// <summary>
    /// 主控板启动、复位停止事件
    /// </summary>
    /// <param name="Typ_Cmd"></param>
    public delegate void DelegateEventMainControl(int Typ_Cmd);


    /// <summary>
    /// 方案重新载入事件
    /// </summary>
    public delegate void DelegateEventRepeatLoadPlan();

    /// <summary>
    /// 表数据保存事件
    /// </summary>
    /// <param name="intGroup">0为A组，1为B组</param>
    public delegate void DelegateEventMeterDataSave(int intGroup);

    /// <summary>
    /// 料号改变事件
    /// </summary>
    /// <param name="strProductPara">料号参数</strProductPara>
    public delegate void DelegateEventProductChange(string strProductPara);


    /// <summary>
    /// 方案数据保存事件
    /// </summary>
    /// <param name="intItem">项目序号</param>
    /// <param name="Checked">是否要检</param>
    /// <param name="XmlFilePath">XML文件路径</param>
    public delegate void DelegateEventSchemaDataSave(int intItem, bool Checked, string XmlFilePath);//


    /// <summary>
    /// 方案数据调用事件
    /// </summary>
    /// <param name="intItem">项目序号</param>
    public delegate void DelegateEventSchemaDataLoad(int intItem, string XmlFilePath);
    /// <summary>
    /// 
    /// </summary>
    public delegate void DelegateEventTxtChange(int Bw, string strTxm);
}
