using System;
using System.Collections.Generic;

using System.Text;
using VerificationEquipment.Commons;

namespace Frontier.MeterVerification.DeviceCommon
{
    public class VerificationHelper
    {
        /// <summary>
        /// 获取电能表的电压
        /// </summary>
        /// <param name="meter"></param>
        /// <returns></returns>
        public static float GetU(Meter meter)
        {
            float U = 0;
            if (meter != null && !string.IsNullOrEmpty(meter.Voltage))
            {
                string voltage = meter.Voltage.Trim().ToUpper().TrimEnd('V');
                if (voltage.Contains("220"))
                {
                    voltage = "220";
                }
                else if (voltage.Contains("×"))
                {
                    //截取×号后面的字符串
                    voltage = voltage.Substring(voltage.IndexOf("×") + 1);

                    //部分含有/则截取/号前面的字符串
                    if (voltage.Contains("/"))
                    {
                        voltage = voltage.Substring(0, voltage.IndexOf("/"));
                    }

                }
                else if (voltage.Contains("*"))
                {
                    //截取×号后面的字符串
                    voltage = voltage.Substring(voltage.IndexOf("*") + 1);

                    //部分含有/则截取/号前面的字符串
                    if (voltage.Contains("/"))
                    {
                        voltage = voltage.Substring(0, voltage.IndexOf("/"));
                    }
                }
                try
                {
                    U = float.Parse(voltage);
                }
                catch { }
            }
            return U;
        }

        /// <summary>
        /// 获取电能表的基本电流
        /// </summary>
        /// <param name="meter"></param>
        /// <returns></returns>
        public static double GetIb(Meter meter)
        {
            double Ib = 0;
            if (meter != null && !string.IsNullOrEmpty(meter.Current))
            {
                string current = meter.Current.Trim().ToUpper().TrimEnd('A');
                if (current.Contains("×"))
                {
                    //截取×号后面的字符串,处理成Ib(Imax)
                    current = current.Substring(current.IndexOf("×") + 1);
                }
                if (current.Contains("*"))
                {
                    //截取*号后面的字符串,处理成Ib(Imax)
                    current = current.Substring(current.IndexOf("*") + 1);
                }
                if (current.Contains("("))
                {
                    current = current.Substring(0, current.IndexOf("("));
                }
                //修改电流中不包括×和（）的情况，当电流格式为5A，则该转换后为0A
                try
                {
                    Ib = double.Parse(current);
                }
                catch { }
            }
            return Ib;
        }

        /// <summary>
        /// 获取电能表的最大电流
        /// </summary>
        /// <param name="meter"></param>
        /// <returns></returns>
        public static double GetImax(Meter meter)
        {
            double Imax = 0;
            if (meter != null && !string.IsNullOrEmpty(meter.Current))
            {
                string strCurrent = meter.Current.Trim().ToUpper().TrimEnd('A');
                if (strCurrent.Contains("("))
                {
                    strCurrent = strCurrent.Substring(strCurrent.IndexOf("(") + 1, strCurrent.IndexOf(")") - strCurrent.IndexOf("(") - 1);
                }
                //修改电流中不包括×和（）的情况，当电流格式为5A，则该转换后为0A
                try
                {
                    Imax = double.Parse(strCurrent);
                }
                catch { }
            }
            return Imax;
        }
    }
}
