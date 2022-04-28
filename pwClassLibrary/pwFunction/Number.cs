using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace pwClassLibrary
{
    /// <summary>
    /// 有关数字计算的公共函数
    /// </summary>
    public class Number
    {
        public const float WUCHA_INVIADE = -999F;

        /// <summary>
        /// 计算一个数组的平均值
        /// </summary>
        /// <param name="arrNumber">参与计算的数字</param>
        /// <returns></returns>
        public static float GetAvgA(float[] arrNumber)
        {
            int intCount = arrNumber.Length;
            float Sum = 0;
            if (0 == intCount) return 0F;
            for (int i = 0; i < intCount; i++)
            {
                if (arrNumber[i] != WUCHA_INVIADE)
                {
                    Sum += arrNumber[i];
                }
            }

            float fAv = (float)Math.Round(Sum / (float)intCount, 4);
            return fAv;
        }




        /// <summary>
        /// 计算一组数据的平均值
        /// </summary>
        /// <param name="arrNumbers">要参与计算的数字(可变参数)</param>
        /// <returns>返回参与计算的平均值,如果参数计算数据过多请使用GetAvgA函数</returns>
        public static float GetAvg(params  float[] arrNumbers)
        {
            return GetAvgA(arrNumbers);
        }



        /// <summary>
        /// 化整值计算
        /// </summary>
        /// <param name="Number">要化整的数字</param>
        /// <param name="Space">化整间距</param>
        /// <returns>化整后的浮点数</returns>
        public static float GetHzz(float Number, float Space)
        {
            float opNumber = Math.Abs(Number);           //用于操作
            int PartZhengShu;                           //整数部分
            float PartXiaoShu;                          //小数部分
            int intFlag = Number > 0 ? 1 : -1;          //记录符号
            if (Space != 1)
            {
                //如果化整间距不为1,则直接将Number/Space后按1的方法化整
                opNumber = (float)(opNumber / Space);
            }
            PartZhengShu = (int)opNumber;                       // 取得整数部分
            PartXiaoShu = opNumber - (float)PartZhengShu;       // 得到小数部分
            if (PartXiaoShu > 0.5F)                             //右边部分大于0.5，则整数部分++
            {
                PartZhengShu++;
            }
            else if (PartXiaoShu == 0.5F)                   //==0.5,则检测化整位
            {
                if (PartZhengShu % 2 == 1)
                {
                    PartZhengShu++;
                }
            }
            //还原
            opNumber = intFlag * PartZhengShu * Space;
            return opNumber;
        }



        /// <summary>
        /// 计算一组数据的标准偏差
        /// </summary>
        /// <param name="arrNumber">输入数据数组</param>
        /// <returns>返回一组数据的偏差值((未化整))</returns>
        public static float GetWindage(float[] arrNumber)
        {
            int intCount = 0;    //要计算偏差的成员个数
            float Sum = 0F;                     //和，用于计算平均值
            float Avg = 0F;                     //平均值
            float Windage = 0F;                 //辅助计算变量
           // if (intCount < 1) return 0F;
            

            //计算平均值
            for (int i = 0; i < arrNumber.Length; i++)
            {
                if (arrNumber[i] !=WUCHA_INVIADE)
                {
                    Sum += arrNumber[i];
                    intCount++;
                }
            }
            if (intCount == 1)
            {
                return 0F;
            }
            Avg = Sum / (float)intCount;
            //计算偏差
            for (int i = 0; i < intCount; i++)
            {
                if (arrNumber[i] !=WUCHA_INVIADE)
                {
                    Windage += (float)Math.Pow((arrNumber[i] - Avg), 2);
                }
            }
            Windage = Windage / (float)(intCount - 1);
            return (float)Math.Sqrt((double)Windage);
        }



        /// <summary>
        /// 计算一组数据的标准偏差
        /// </summary>
        /// <param name="arrNumbers">输入数据(可变参数)</param>
        /// <returns>返回几个数字的标准偏差(未化整)，如果数据个数过多请使用GetWindage函数</returns>
        public static float GetWindageA(params float[] arrNumbers)
        {
            return GetWindage(arrNumbers);
        }

        /// <summary>
        /// 计算相对误差值[(a-b)/a]
        /// </summary>
        /// <param name="a">比较参数</param>
        /// <param name="b">被比较参数</param>
        /// <returns>返回二个数字相差百分比[形如:(a-b)/b *100],适用于走字误差，需量误差等计算</returns>
        public static float GetRelativeWuCha(float a, float b)
        {
            if (a == 0) return 0F;
            return (float)Math.Round(((a - b) / a) * 100F, 2);
        }

        /// <summary>
        /// 计算相对误差值(a-b)/b+r
        /// </summary>
        /// <param name="a">比较参数</param>
        /// <param name="b">被比较参数</param>
        /// <param name="other">标准表基本误差</param>
        /// <returns>返回二个数字相差百分比[形如:(a-b)/b *100],适用于走字误差，需量误差等计算</returns>
        public static float GetRelativeWuCha(float a, float b, float other)
        {
            return GetRelativeWuCha(a, b) + other;
        }

        /// <summary>
        /// 返回电流值
        /// </summary>
        /// <param name="xIb">电流倍数Imax,1.0Ib</param>
        /// <param name="Current">电流参数1.5(6)</param>
        /// <returns></returns>
        public static float getI(string xIb, string Current)
        {
            //if (Current.IndexOf("(") >= 0 && Current.IndexOf(")") >= 0)
            float _Ib = 0F;
            float _Imax = 0F;
            Regex _Reg = new Regex("(?<ib>[\\d\\.]+)\\((?<imax>[\\d\\.]+)\\)");
            Match _Match = _Reg.Match(Current);
            if (_Match.Groups["ib"].Value.Length < 1)
                return 0F;

            _Ib = float.Parse(_Match.Groups["ib"].Value);
            _Imax = float.Parse(_Match.Groups["imax"].Value);

            if (xIb.ToLower() == "imax")
                return _Imax;
            else if (xIb.ToLower().IndexOf("imax") >= 0 && xIb.ToLower().IndexOf("ib") == -1)
                return _Imax * float.Parse(xIb.ToLower().Replace("imax", ""));
            else if (xIb.ToLower() == "ib")
                return _Ib;
            else if (xIb.ToLower().IndexOf("ib") >= 0 && xIb.ToLower().IndexOf("imax") == -1)
                return _Ib * float.Parse(xIb.ToLower().Replace("ib", ""));
            else if (xIb.ToLower().IndexOf("(imax-ib)") >= 0)
                if (xIb.ToLower().IndexOf("/") >= 0)
                    return (_Imax - _Ib) / float.Parse(xIb.ToLower().Replace("(imax-ib)/", ""));
                else
                    return (_Imax - _Ib) * float.Parse(xIb.ToLower().Replace("(imax-ib)", ""));
            else
                return 0F;
        }

        /// <summary>
        /// 获取有功或无功常数值 
        /// </summary>
        /// <param name="ConstString">表常数 有功（无功）</param>
        /// <param name="YouGong">是否是有功</param>
        /// <returns>[有功，无功]</returns>
        public static int GetBcs(string ConstString,bool YouGong)
        {
            ConstString = ConstString.Replace("（", "(").Replace("）", ")");

            if (ConstString.Trim().Length < 1)
            {
                //System.Windows.Forms.MessageBox.Show("没有录入常数");
                return 1;
            }

            string[] arTmp = ConstString.Trim().Replace(")", "").Split('(');

            if (arTmp.Length == 1)
            {
                if (pwClassLibrary.Number.IsNumeric(arTmp[0]))
                    return int.Parse(arTmp[0]);
                else
                    return 1;
            }
            else
            {
                if (pwClassLibrary.Number.IsNumeric(arTmp[0]) && pwClassLibrary.Number.IsNumeric(arTmp[1]))
                {
                    if (YouGong)
                        return int.Parse(arTmp[0]);
                    else
                        return int.Parse(arTmp[1]);
                }
                else
                    return 1;
            }
        }
        /// <summary>
        /// 获取电流倍数算术值
        /// </summary>
        /// <param name="xIb">电流倍数字符串1.5Ib</param>
        /// <param name="Current">电流参数1.5(6)</param>
        /// <returns></returns>
        public static float getxIb(string xIb, string Current)
        {
            float _Ib = 0F;
            float _Imax = 0F;
            Regex _Reg = new Regex("(?<ib>[\\d\\.]+)\\((?<imax>[\\d\\.]+)\\)");
            Match _Match = _Reg.Match(Current);
            if (_Match.Groups["ib"].Value.Length < 1)
                return 0F;

            _Ib = float.Parse(_Match.Groups["ib"].Value);
            _Imax = float.Parse(_Match.Groups["imax"].Value);
            float _BeiShu = _Imax / _Ib;
            if (xIb.ToLower() == "imax")
                return _BeiShu;
            else if (xIb.ToLower().IndexOf("imax") >= 0 && xIb.ToLower().IndexOf("ib") == -1)
                return _BeiShu * float.Parse(xIb.ToLower().Replace("imax", ""));
            else if (xIb.ToLower() == "ib")
                return 1F;
            else if (xIb.ToLower().IndexOf("ib") >= 0 && xIb.ToLower().IndexOf("imax") == -1)
                return 1F * float.Parse(xIb.ToLower().Replace("ib", ""));
            else if (xIb.ToLower().IndexOf("(imax-ib)") >= 0)
                if (xIb.ToLower().IndexOf("/") >= 0)
                    return ((_Imax - _Ib) / float.Parse(xIb.ToLower().Replace("(imax-ib)/", ""))) / _Ib;
                else
                    return ((_Imax - _Ib) * float.Parse(xIb.ToLower().Replace("(imax-ib)", ""))) / _Ib;
            else
                return 1F;
        }
        /// <summary>
        /// 获取功率因素数值
        /// </summary>
        /// <param name="Glys">功率因素1.0,0.5L</param>
        /// <returns></returns>
        public static float getGlysValue(string Glys)
        {
            try
            {
                return float.Parse(Glys);
            }
            catch
            {
                return float.Parse(Glys.Substring(0, Glys.Length - 1));
            }
        }
        private static Regex IsNumeric_Reg = null;
        /// <summary>
        /// 检测是否是数字
        /// </summary>
        /// <param name="sNumeric">要验证的字符串</param>
        /// <returns>是Y否N</returns>
        public static bool IsNumeric(string sNumeric)
        {
            if (sNumeric == null || sNumeric.Length == 0) return false;
            if (IsNumeric_Reg == null)
                IsNumeric_Reg = new Regex("^[\\+\\-]?[0-9]*\\.?[0-9]+$");
            return IsNumeric_Reg.Replace(sNumeric, "").Length == 0;
        }

        private static Regex IsIntNumeric_Reg = null;
        /// <summary>
        /// 检查是否为整型数字、包括负数
        /// </summary>
        /// <param name="sNumeric"></param>
        /// <returns></returns>
        public static bool IsIntNumber(string sNumeric)
        {
            if (sNumeric == null || sNumeric.Length == 0) return false;
            if (IsIntNumeric_Reg == null)
                IsIntNumeric_Reg = new Regex("-?[0-9]+");
            return IsIntNumeric_Reg.Replace(sNumeric, "").Length == 0;
        }

        /// <summary>
        /// 返回等级，数组下标0=有功，1=无功
        /// </summary>
        /// <param name="DjString">等级字符串1.0S(2.0)</param>
        /// <returns></returns>
        public static string[] getDj(string DjString)
        {
            DjString = DjString.ToUpper().Replace("S", "");
            DjString = DjString.ToUpper().Replace("（", "(").Replace("）", ")").Replace(")","");
            string[] _Dj = DjString.Split('(');
            if (_Dj.Length == 1)
                return new string[] { float.Parse(_Dj[0]).ToString("F1"), float.Parse(_Dj[0]).ToString("F1") };
            else
                return new string[] { float.Parse(_Dj[0]).ToString("F1"), float.Parse(_Dj[1]).ToString("F1") };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bFirst"></param>
        /// <returns>数组</returns>
        ///


        /// <summary>
        /// 拆分1.5(6)字样的参数
        /// </summary>
        /// <param name="str">要拆分的对象</param>
        /// <param name="bFirst">是否是取第一个参数，如果为False则取第二个参数</param>
        /// <returns>指定的数据</returns>

        public static long SplitKF(string str, bool bFirst)
        {
            string[] _Array = getDj(str);
            return bFirst ? long.Parse((float.Parse(_Array[0])).ToString("F0")) : long.Parse((float.Parse(_Array[1])).ToString("F0"));
        }


        //public static void PopDesc(ref long[] arrList, bool IsUp)
        //{
        //    float[] fltArray = ConvertArray.ConvertLong2Float(arrList);
        //    PopDesc(ref fltArray, IsUp);
        //   //这儿有问题
        //}

        //public static void PopDesc(ref int[] arrList,bool IsUP)
        //{
        //    float[] fltArray = ConvertArray.ConvertInt2Float(arrList);
        //    PopDesc(ref fltArray, IsUP);
        //   // Array.Copy(fltArray, arrList, fltArray.Length);
        //    // 这儿有问题
        //}
        /// <summary>
        /// 冒泡排序法
        /// </summary>
        /// <param name="arrList">要排序的数驵</param>
        /// <param name="Desc">升/降序</param>
        public static void PopDesc(ref float[] arrList, bool IsUp)
        {

            for (int i = 0; i < arrList.Length; i++)
            {
                for (int j = i; j < arrList.Length; j++)
                {
                    if (IsUp) {
                        if (arrList[i] > arrList[j])
                        {
                            float temp = arrList[i];
                            arrList[i] = arrList[j];
                            arrList[j] = temp;
                        } 
                    }
                    else
                    {
                        if (arrList[i] < arrList[j])
                        {
                            float temp = arrList[i];
                            arrList[i] = arrList[j];
                            arrList[j] = temp;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 计算功率
        /// </summary>
        /// <param name="flt_U">电压有效值</param>
        /// <param name="flt_I">电流有效值</param>
        /// <returns></returns>
        public static float GetPower(float flt_U, float flt_I)
        {
            return (float)Math.Round(Math.Sqrt((Math.Pow(flt_U, 2) + Math.Pow(flt_I, 2))), 4);
        }

        /// <summary>
        /// 获取指定数据的小数位数
        /// </summary>
        /// <param name="strNumber">数字字符串</param>
        /// <returns></returns>
        public static int GetPrecision(string strNumber)
        {
            if (!pwClassLibrary.Number.IsNumeric(strNumber))
            {
                return 0;
            }
            int hzPrecision = strNumber.ToString().LastIndexOf('.');
            if (hzPrecision == -1)
            {
                //没有小数点，返回0
                hzPrecision = 0;
            }
            else
            {
                //有小数点
                hzPrecision = strNumber.ToString().Length - hzPrecision - 1;
            }
            return hzPrecision;

        }
        /// <summary>
        /// 获取指定数据的小数位数
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int GetPrecision(float number)
        {
            return GetPrecision(number.ToString());
        }


        /// <summary>
        /// 格式化输出
        /// </summary>
        /// <param name="value"></param>
        /// <param name="sFormat"></param>
        /// <param name="fromBase"></param>
        /// <returns></returns>
        private string pw_GetFloatstrFromStr(string value, string sFormat, int fromBase)
        {//BCD码或HEX码转成浮点型数值数据
            try
            {
                int iDot = 0;
                string[] sTmpFormat = sFormat.Split('.');
                if (sTmpFormat.Length > 1)
                {
                    iDot = sTmpFormat[1].Length;
                }
                string sz = "00000000";
                double ff = 0;
                value =Sstring.BackString(value);
                ff = Convert.ToInt64(value, fromBase) / Math.Pow(10, iDot);
                value = string.Format("{0:#0." + sz.Substring(0, iDot) + "}", ff);
                return value;

            }
            catch (Exception ee)
            {
                return "0";
            }
        }


        /// <summary>
        /// 去掉符号位(最高位)
        /// </summary>
        /// <param name="sData">数据：Hex字符串</param>
        /// <returns></returns>
        public static bool bIsNegativeString(ref string sData)
        {

            
            int iLen = sData.Length;
            string strFF = "7FFFFFFF";

            int iTmp = Convert.ToInt32(sData, 16);

            int iComRet = iTmp & (int)Math.Pow(2, iLen * 4 - 1);

            int iReturnData = Convert.ToInt32(sData, 16) & Convert.ToInt32(strFF.Substring(0, iLen), 16);

            sData = iReturnData.ToString("X" + iLen.ToString());

            if (iComRet == 0)//最高位为0，为正
            {
                return true;
            }
            else
            {
                return false;
            }


        }

    }
}
