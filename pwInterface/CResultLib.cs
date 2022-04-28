using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pwInterface
{

    /// <summary>
    /// 数据基类
    /// </summary>
    [Serializable()]
    public class DataResultBasic
    {

        /// <summary>
        /// 表位号
        /// </summary>
        public int Me_Bw = 0;

        /// <summary>
        /// 项目ID
        /// </summary>
        public string Me_PrjID = "";
        /// <summary>
        /// 项目名称		描述
        /// </summary>
        public string Me_PrjName = "";

        /// <summary>
        /// 项目结论		合格/不合格
        /// </summary>
        private string m_Result = "";
        public string Me_Result
        {
            set
            {
                m_Result = value;
            }
            get { return m_Result; }
        }

        /// <summary>
        /// 项目值
        /// </summary>
        public string Me_Value="";

        public DataResultBasic()
        {
        }

    }


    /// <summary>
    /// 误差点数据
    /// </summary>
    [Serializable()]
    public class MeterErrorItem
    {

        /// <summary>
        /// 表位号
        /// </summary>
        public int Me_Bw = 0;

        /// <summary>
        /// 是否要检
        /// </summary>
        public bool IsCheck=true ;

        /// <summary>
        /// 误差子项目ID	
        /// </summary>
        public string Item_PrjID="";

        /// <summary>
        /// 项目名称		描述
        /// </summary>
        public string Item_PrjName="";

        /// <summary>
        /// 项目结论		合格/不合格
        /// </summary>
        public string Item_Result = "";

        /// <summary>
        /// 表等级
        /// </summary>
        public float  MeterLevel=1.0f;
        /// <summary>
        /// 误差上限
        /// </summary>
        public float  MaxError=1.0f;
        /// <summary>
        /// 误差下限
        /// </summary>
        public float  MinError=-1.0f;
        /// <summary>
        /// 误差平均
        /// </summary>
        public string strWcPJ = "";
        /// <summary>
        /// 误差化整
        /// </summary>
        public string strWcHz = "";
        /// <summary>
        /// 误差化整
        /// </summary>
        public float WcHz = 0f;
        /// <summary>
        /// 误差平均
        /// </summary>
        public float WcPJ = 0f;
        /// <summary>
        /// 误差一|误差二
        /// </summary>
        public float[] Wcn;

        /// <summary>
        /// 误差有效次数，每一个误差点取几次误差参与计算
        /// </summary>
        public int VerifyTimes = 2;

        /// <summary>
        /// 偏差值
        /// </summary>
        public float WcPc = 0f;

        /// <summary>
        /// 是否要在WuChaDeal类里计算偏差
        /// </summary>
        public int intPc = 0 ;//0=不做　1=做

        public MeterErrorItem()
        {
            Wcn = new float[10];
            for (int i = 0; i < Wcn.Length; i++)
                Wcn[i] = 0f;

        }

        /// <summary>
        /// 计算平均值的化整值
        /// </summary>
        /// <param name="averageValue">平均值</param>
        /// <param name="intDalt">化整间距</param>
        /// <param name="errorInt">化整误差</param>
        /// <returns></returns>
        public static string ErrorInt(float averageValue, string intDalt, ref float errorInt)
        {
            try
            {
                string value = string.Empty;
                //判断是否是负值
                bool negative = false;
                if (averageValue < 0)
                    negative = true;
                //取绝对值
                averageValue = System.Math.Abs(averageValue);
                //首先判断化整间距的小数位数
                if (intDalt.Length < 1)
                    return "";
                float intdaltfloat = float.Parse(intDalt);
                int wei = intDalt.Length - intDalt.IndexOf('.') - 1;
                if (wei < 1)
                    wei = 1;
                //取整
                double quotientValue = averageValue / intdaltfloat;
                quotientValue = System.Math.Round(quotientValue, 6, MidpointRounding.AwayFromZero);
                double tt = System.Math.Round((float)quotientValue);
                errorInt = (float)(tt * intdaltfloat);
                //化整后的值的格式
                //string format = string.Format("0:F{0}", wei);
                string format = "0.";//= string.Format("0.", wei);
                format = format.PadRight(wei + 2, '0');
                //if (errorInt > -0.00001)
                //{
                //    if (SysCommon.TestItemParam.ResultZeroSign)
                //    {
                //        if (negative)
                //            format = "-{" + format + "}";
                //        else
                //            format = "+{" + format + "}";
                //    }
                //    else
                //    {
                //        format = "{" + format + "}";
                //    }
                //}
                //else
                //{
                //    if (negative)
                //        format = "-{" + format + "}";
                //    else
                //        format = "+{" + format + "}";

                //}
                // value = string.Format(format, errorInt);
                value = errorInt.ToString(format);

                return value;
            }
            catch (Exception ex)
            {
                return "";
            }
        }


        /// <summary>
        ///  误差平均|误差化整|误差一|误差二|...|
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string strReturn = "";
            strReturn = WcPJ.ToString("F4") + "|";
            strReturn += WcHz.ToString() + "|";
            for (int i = 0; i < Wcn.Length; i++)
            {
                //if (i > VerifyTimes)
                //    strReturn +=  "|";
                //else
                    strReturn += Wcn[i].ToString("F4") + "|";
            }
            strReturn += WcPc.ToString("F4") + "|";
            return strReturn;
        }


    }
    
    /// <summary>
    /// 多功能项数据
    /// </summary>
    [Serializable()]
    public class MeterDgnItem
    {


        /// <summary>
        /// 表位号
        /// </summary>
        public int Me_Bw = 0;

        /// <summary>
        /// 多功能子项目ID	
        /// </summary>
        public string Item_PrjID;

        /// <summary>
        /// 项目名称		描述
        /// </summary>
        public string Item_PrjName;

        /// <summary>
        /// 项目结论		合格/不合格
        /// </summary>
        private string m_Result = "";
        public string Item_Result
        {
            set
            {
                m_Result = value;
            }
            get { return m_Result; }
        }

        /// <summary>
        /// 项目值
        /// </summary>
        public string Item_Value="";

    }

    /// <summary>
    /// 打包参数下载方案项目
    /// </summary>
    [Serializable()]
    public class MeterDownParaItem
    {

        /// <summary>
        /// 表位号
        /// </summary>
        public int Me_Bw = 0;

        /// <summary>
        ///下载子项目ID
        /// </summary>
        public string Item_PrjID;

        /// <summary>
        /// 子项目名称		描述
        /// </summary>
        public string Item_PrjName;

        /// <summary>
        /// 项目结论		合格/不合格
        /// </summary>
        private string m_Result = "";
        public string Item_Result
        {
            set
            {
                m_Result = value;
            }
            get { return m_Result; }
        }

        /// <summary>
        /// 下载帧
        /// </summary>
        public string Item_TxFrame;
        /// <summary>
        /// 返回帧
        /// </summary>
        public string Item_RxFrame;
        /// <summary>
        /// 返回数据域
        /// </summary>
        public string Item_Value;

        public MeterDownParaItem()
        {
            Item_PrjID = "";
            Item_TxFrame = "";
            Item_RxFrame = "";
            Item_Value = "";

        }
    }

}
