using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using pwInterface;
namespace VerifyAdapter
{
    public class WuChaDeal
    {
        #region----------基本误差计算----------

        /// <summary>
        /// 计算基本误差
        /// </summary>
        /// <param name="arrNumber">要参与计算的误差数组</param>
        /// <returns></returns>
        public static void SetWuCha(ref MeterErrorItem mErrPoint)
        {
            MeterErrorItem curResult = new MeterErrorItem();
            curResult = mErrPoint;
            float[] arrNumber = new float[curResult.VerifyTimes];
            Array.Copy(mErrPoint.Wcn, arrNumber, curResult.VerifyTimes);
            int AvgPriecision = getAvgPrecision();                                  //取平均值修约精度 X位小数
            float intSpace = getWuChaHzzJianJu(curResult.MeterLevel,false);         //化整间距
            float AvgWuCha = pwClassLibrary.Number.GetAvgA(arrNumber);
            float HzzWuCha = pwClassLibrary.Number.GetHzz(AvgWuCha, intSpace);

            curResult.WcPJ = AvgWuCha;
            curResult.WcHz = HzzWuCha;

            curResult.strWcPJ =AddFlag( Convert.ToString(AvgWuCha));
            curResult.strWcHz =AddFlag( Convert.ToString(HzzWuCha));

            //添加符号
            //string AvgNumber;
            //string HZNumber;
            //int hzPrecision = pwClassLibrary.Number.GetPrecision(intSpace);

            //AvgNumber = AddFlag(AvgWuCha, AvgPriecision);
            //HZNumber = AddFlag(HzzWuCha, hzPrecision);

            //curResult.strWcPJ = AvgNumber;
            //curResult.strWcHz = HZNumber;

            // 检测是否超过误差限
            if (AvgWuCha >= curResult.MinError &&
                AvgWuCha <= curResult.MaxError)
            {
                curResult.Item_Result = pwFunction.pwConst.Variable.CTG_HeGe;
            }
            else
            {
                curResult.Item_Result = pwFunction.pwConst.Variable.CTG_BuHeGe;
            }

            #region 偏差处理
            if (curResult.intPc == 1)
            {
                //计算偏差
                curResult.WcPc = pwClassLibrary.Number.GetWindage(arrNumber);
                //判断偏差结果
                if (curResult.WcPc  < curResult.MinError && curResult.WcPc  > curResult.MaxError)
                    curResult.Item_Result = pwFunction.pwConst.Variable.CTG_BuHeGe;
            }
            #endregion

            mErrPoint = curResult;
        }
        #endregion


        #region 添加符号---------AddFlag----------

        /// <summary>
        /// 加+-符号
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        static string AddFlag(string Number)
        {
            string strFlag = "";
            if (float.Parse(Number) >= 0)
            {
                strFlag = "+";
            }
            return String.Format("{0}{1}", strFlag, Number);
        }

        /// <summary>
        /// 修正数字加+-号
        /// </summary>
        /// <param name="Number">要修正的数字</param>
        /// <param name="Priecision">修正精度</param>
        /// <returns>返回指定精度的带+-号的字符串</returns>
        static string AddFlag(float Number, int Priecision)
        {
            string strValue =Number.ToString(String.Format("F{0}", Priecision));
            strValue = AddFlag(strValue);
            return strValue;
        }
        #endregion


        #region 辅助功能函数
        /// <summary>
        /// 返回修正间距
        /// </summary>
        /// <IsWindage>是否是偏差</IsWindage> 
        /// <returns></returns>
        static float getWuChaHzzJianJu(float MeterLevel, bool IsWindage)
        {
            Dictionary<string, float[]> DicJianJu = null;
            float[] JianJu = new float[] { 2, 2 };
            string Key = String.Empty;
            //根据表精度及表类型生成主键
            Key = String.Format("Level{0}", MeterLevel);

            if (DicJianJu == null)
            {
                DicJianJu = new Dictionary<string, float[]>();
                DicJianJu.Add("Level0.2", new float[] { 0.02F, 0.004F }); //0.2级普通表
                DicJianJu.Add("Level0.5", new float[] { 0.05F, 0.01F });  //0.02级表
                DicJianJu.Add("Level1", new float[] { 0.1F, 0.02F });     //0.02级表
                DicJianJu.Add("Level2", new float[] { 0.2F, 0.04F });     //0.02级表
 }

            if (DicJianJu.ContainsKey(Key))
            {
                JianJu = DicJianJu[Key];
            }
            else
            {
                JianJu = new float[] { 2, 2 };    //没有在字典中找到，则直接按2算
            }

            if (IsWindage)
            {
                //标偏差
                return (float)JianJu[1];
            }
            else
            {
                //普通误差
                return (float)JianJu[0];
            }
        }
        /// <summary>
        /// 取电能表平均值修约精度
        /// </summary>
        /// <returns>精度</returns>
        static int getAvgPrecision()
        {
            return pwFunction.pwConst.GlobalUnit.GetConfig(pwFunction.pwConst.Variable.CTC_WC_AVGPRECISION, 4);
        }
        #endregion
    }

}
