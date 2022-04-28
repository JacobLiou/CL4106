using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Globalization;
using pwInterface;

namespace ClAmMeterController
{
    /// <summary>
    /// 用于线程传输参数的
    /// </summary>
    public class CThreadParameter
    {
        public bool bln_Return;         //操作完成，返回了

        public int int_ChannelNo;       //程线号
        public bool bln_Selected = true;  //选中，要启动
        public int int_BwNo;            //表位号


        public int int_Parameter1;
        public int int_Parameter2;
        public int int_Parameter3;
        public string str_Parameter1;
        public string str_Parameter2;

        public float flt_Parameter1;

        public float[] flt_ParameterArry;
        public string[] str_ParameterArry;
        public byte[] byt_ParameterArry;
        public List<MeterDownParaItem> DownParaItem;

        public ArrayList ListAPDU;


    }
}
