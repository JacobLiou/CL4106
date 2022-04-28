using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace pwClassLibrary
{
    /// <summary>
    /// �й����ּ���Ĺ�������
    /// </summary>
    public class Number
    {
        public const float WUCHA_INVIADE = -999F;

        /// <summary>
        /// ����һ�������ƽ��ֵ
        /// </summary>
        /// <param name="arrNumber">������������</param>
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
        /// ����һ�����ݵ�ƽ��ֵ
        /// </summary>
        /// <param name="arrNumbers">Ҫ������������(�ɱ����)</param>
        /// <returns>���ز�������ƽ��ֵ,��������������ݹ�����ʹ��GetAvgA����</returns>
        public static float GetAvg(params  float[] arrNumbers)
        {
            return GetAvgA(arrNumbers);
        }



        /// <summary>
        /// ����ֵ����
        /// </summary>
        /// <param name="Number">Ҫ����������</param>
        /// <param name="Space">�������</param>
        /// <returns>������ĸ�����</returns>
        public static float GetHzz(float Number, float Space)
        {
            float opNumber = Math.Abs(Number);           //���ڲ���
            int PartZhengShu;                           //��������
            float PartXiaoShu;                          //С������
            int intFlag = Number > 0 ? 1 : -1;          //��¼����
            if (Space != 1)
            {
                //���������಻Ϊ1,��ֱ�ӽ�Number/Space��1�ķ�������
                opNumber = (float)(opNumber / Space);
            }
            PartZhengShu = (int)opNumber;                       // ȡ����������
            PartXiaoShu = opNumber - (float)PartZhengShu;       // �õ�С������
            if (PartXiaoShu > 0.5F)                             //�ұ߲��ִ���0.5������������++
            {
                PartZhengShu++;
            }
            else if (PartXiaoShu == 0.5F)                   //==0.5,���⻯��λ
            {
                if (PartZhengShu % 2 == 1)
                {
                    PartZhengShu++;
                }
            }
            //��ԭ
            opNumber = intFlag * PartZhengShu * Space;
            return opNumber;
        }



        /// <summary>
        /// ����һ�����ݵı�׼ƫ��
        /// </summary>
        /// <param name="arrNumber">������������</param>
        /// <returns>����һ�����ݵ�ƫ��ֵ((δ����))</returns>
        public static float GetWindage(float[] arrNumber)
        {
            int intCount = 0;    //Ҫ����ƫ��ĳ�Ա����
            float Sum = 0F;                     //�ͣ����ڼ���ƽ��ֵ
            float Avg = 0F;                     //ƽ��ֵ
            float Windage = 0F;                 //�����������
           // if (intCount < 1) return 0F;
            

            //����ƽ��ֵ
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
            //����ƫ��
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
        /// ����һ�����ݵı�׼ƫ��
        /// </summary>
        /// <param name="arrNumbers">��������(�ɱ����)</param>
        /// <returns>���ؼ������ֵı�׼ƫ��(δ����)��������ݸ���������ʹ��GetWindage����</returns>
        public static float GetWindageA(params float[] arrNumbers)
        {
            return GetWindage(arrNumbers);
        }

        /// <summary>
        /// ����������ֵ[(a-b)/a]
        /// </summary>
        /// <param name="a">�Ƚϲ���</param>
        /// <param name="b">���Ƚϲ���</param>
        /// <returns>���ض����������ٷֱ�[����:(a-b)/b *100],�������������������ȼ���</returns>
        public static float GetRelativeWuCha(float a, float b)
        {
            if (a == 0) return 0F;
            return (float)Math.Round(((a - b) / a) * 100F, 2);
        }

        /// <summary>
        /// ����������ֵ(a-b)/b+r
        /// </summary>
        /// <param name="a">�Ƚϲ���</param>
        /// <param name="b">���Ƚϲ���</param>
        /// <param name="other">��׼��������</param>
        /// <returns>���ض����������ٷֱ�[����:(a-b)/b *100],�������������������ȼ���</returns>
        public static float GetRelativeWuCha(float a, float b, float other)
        {
            return GetRelativeWuCha(a, b) + other;
        }

        /// <summary>
        /// ���ص���ֵ
        /// </summary>
        /// <param name="xIb">��������Imax,1.0Ib</param>
        /// <param name="Current">��������1.5(6)</param>
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
        /// ��ȡ�й����޹�����ֵ 
        /// </summary>
        /// <param name="ConstString">���� �й����޹���</param>
        /// <param name="YouGong">�Ƿ����й�</param>
        /// <returns>[�й����޹�]</returns>
        public static int GetBcs(string ConstString,bool YouGong)
        {
            ConstString = ConstString.Replace("��", "(").Replace("��", ")");

            if (ConstString.Trim().Length < 1)
            {
                //System.Windows.Forms.MessageBox.Show("û��¼�볣��");
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
        /// ��ȡ������������ֵ
        /// </summary>
        /// <param name="xIb">���������ַ���1.5Ib</param>
        /// <param name="Current">��������1.5(6)</param>
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
        /// ��ȡ����������ֵ
        /// </summary>
        /// <param name="Glys">��������1.0,0.5L</param>
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
        /// ����Ƿ�������
        /// </summary>
        /// <param name="sNumeric">Ҫ��֤���ַ���</param>
        /// <returns>��Y��N</returns>
        public static bool IsNumeric(string sNumeric)
        {
            if (sNumeric == null || sNumeric.Length == 0) return false;
            if (IsNumeric_Reg == null)
                IsNumeric_Reg = new Regex("^[\\+\\-]?[0-9]*\\.?[0-9]+$");
            return IsNumeric_Reg.Replace(sNumeric, "").Length == 0;
        }

        private static Regex IsIntNumeric_Reg = null;
        /// <summary>
        /// ����Ƿ�Ϊ�������֡���������
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
        /// ���صȼ��������±�0=�й���1=�޹�
        /// </summary>
        /// <param name="DjString">�ȼ��ַ���1.0S(2.0)</param>
        /// <returns></returns>
        public static string[] getDj(string DjString)
        {
            DjString = DjString.ToUpper().Replace("S", "");
            DjString = DjString.ToUpper().Replace("��", "(").Replace("��", ")").Replace(")","");
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
        /// <returns>����</returns>
        ///


        /// <summary>
        /// ���1.5(6)�����Ĳ���
        /// </summary>
        /// <param name="str">Ҫ��ֵĶ���</param>
        /// <param name="bFirst">�Ƿ���ȡ��һ�����������ΪFalse��ȡ�ڶ�������</param>
        /// <returns>ָ��������</returns>

        public static long SplitKF(string str, bool bFirst)
        {
            string[] _Array = getDj(str);
            return bFirst ? long.Parse((float.Parse(_Array[0])).ToString("F0")) : long.Parse((float.Parse(_Array[1])).ToString("F0"));
        }


        //public static void PopDesc(ref long[] arrList, bool IsUp)
        //{
        //    float[] fltArray = ConvertArray.ConvertLong2Float(arrList);
        //    PopDesc(ref fltArray, IsUp);
        //   //���������
        //}

        //public static void PopDesc(ref int[] arrList,bool IsUP)
        //{
        //    float[] fltArray = ConvertArray.ConvertInt2Float(arrList);
        //    PopDesc(ref fltArray, IsUP);
        //   // Array.Copy(fltArray, arrList, fltArray.Length);
        //    // ���������
        //}
        /// <summary>
        /// ð������
        /// </summary>
        /// <param name="arrList">Ҫ���������</param>
        /// <param name="Desc">��/����</param>
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
        /// ���㹦��
        /// </summary>
        /// <param name="flt_U">��ѹ��Чֵ</param>
        /// <param name="flt_I">������Чֵ</param>
        /// <returns></returns>
        public static float GetPower(float flt_U, float flt_I)
        {
            return (float)Math.Round(Math.Sqrt((Math.Pow(flt_U, 2) + Math.Pow(flt_I, 2))), 4);
        }

        /// <summary>
        /// ��ȡָ�����ݵ�С��λ��
        /// </summary>
        /// <param name="strNumber">�����ַ���</param>
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
                //û��С���㣬����0
                hzPrecision = 0;
            }
            else
            {
                //��С����
                hzPrecision = strNumber.ToString().Length - hzPrecision - 1;
            }
            return hzPrecision;

        }
        /// <summary>
        /// ��ȡָ�����ݵ�С��λ��
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int GetPrecision(float number)
        {
            return GetPrecision(number.ToString());
        }


        /// <summary>
        /// ��ʽ�����
        /// </summary>
        /// <param name="value"></param>
        /// <param name="sFormat"></param>
        /// <param name="fromBase"></param>
        /// <returns></returns>
        private string pw_GetFloatstrFromStr(string value, string sFormat, int fromBase)
        {//BCD���HEX��ת�ɸ�������ֵ����
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
        /// ȥ������λ(���λ)
        /// </summary>
        /// <param name="sData">���ݣ�Hex�ַ���</param>
        /// <returns></returns>
        public static bool bIsNegativeString(ref string sData)
        {

            
            int iLen = sData.Length;
            string strFF = "7FFFFFFF";

            int iTmp = Convert.ToInt32(sData, 16);

            int iComRet = iTmp & (int)Math.Pow(2, iLen * 4 - 1);

            int iReturnData = Convert.ToInt32(sData, 16) & Convert.ToInt32(strFF.Substring(0, iLen), 16);

            sData = iReturnData.ToString("X" + iLen.ToString());

            if (iComRet == 0)//���λΪ0��Ϊ��
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
