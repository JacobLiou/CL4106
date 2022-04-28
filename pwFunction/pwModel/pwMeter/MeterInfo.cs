using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pwInterface;
using pwClassLibrary;
namespace pwFunction.pwMeter
{
    [Serializable()]
    public class MeterInfo : pwSerializable 
    {
        /// <summary>
        /// 台体编号
        /// </summary>
        public readonly int TaiID = 0;

        /// <summary>
        /// 成品编码
        /// </summary>
        public string ProductsSN = "";

        /// <summary>
        /// 表名称	
        /// </summary>
        public string ProductsName = "";

        /// <summary>
        /// 表型号
        /// </summary>
        public string ProductsModel = "";

        /// <summary>
        /// 测量方式
        /// </summary>
        public enmClfs Clfs = enmClfs.单相;

        /// <summary>
        /// 共阴共阳，需要改放到方案
        /// </summary>
        public enmGyGyType GYGY = enmGyGyType.共阴;

        /// <summary>
        /// 脉冲类型0:脉冲盒，1：光电头，需要改放到系统配置
        /// </summary>
        public enmPulseComType PulseType = enmPulseComType.脉冲盒;

        private enmPulseDzType _enmDzType;// = enmPulseDzType.国网端子;
        /// <summary>
        /// 脉冲端子类型
        /// </summary>
        public enmPulseDzType enmDzType
        {
            get { return _enmDzType; }
        }

        public enmJDQType enmJdqType = enmJDQType.内置继电器;

        /// <summary>
        /// 基本电压
        /// </summary>
        public float Ub = 220f;

        /// <summary>
        /// 基本电流
        /// </summary>
        public float Ib = 10f;

        /// <summary>
        /// 最大电流
        /// </summary>
        public float IMax = 60f;

        /// <summary>
        /// 常数
        /// </summary>
        public int Constant = 1200;

        /// <summary>
        /// 无功常数
        /// </summary>
        public int Constant_wg = 1200;

        /// <summary>
        /// 等级
        /// </summary>
        public float DJ = 0.5f;

        /// <summary>
        /// 无功等级
        /// </summary>
        public float DJ_wg = 1.0f;


        /// <summary>
        /// 频率
        /// </summary>
        public float PL = 50f;


        /// <summary>
        /// 检定日期		YYYY-MM-DD HH:NN:SS
        /// </summary>
        public string Jdrq
        {
            get { return DateTime.Now.ToString(); }
        }

        /// <summary>
        /// 温度		XX（不带单位）
        /// </summary>
        public string Wd = "";

        /// <summary>
        /// 湿度		XX（不带单位）
        /// </summary>
        public string Sd = "";

        /// <summary>
        /// 检验员
        /// </summary>
        public string Jyy = "";//从MES传入

        /// <summary>
        /// 串接字
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string strReturn = "";
            strReturn = Clfs.ToString() + " | "
                + Clfs.ToString() + " | "
                + Ub.ToString() + " | "
                + IMax.ToString() + " | "
                + Constant.ToString() + " | "
                + PL.ToString() + " | "
                + _enmDzType.ToString() + " | "
                + enmJdqType.ToString() + " | ";

            return strReturn;
        }

        
        public MeterInfo()
        {
            try
            {
                if (pwFunction.pwConst.GlobalUnit.g_Work != null)
                {
                    this.ProductsSN = pwFunction.pwConst.GlobalUnit.g_Work.ProductsSN;
                    this.ProductsName = pwFunction.pwConst.GlobalUnit.g_Work.ProductsName;
                    this.ProductsModel = pwFunction.pwConst.GlobalUnit.g_Work.ProductsModel;
                }
                if (pwFunction.pwConst.GlobalUnit.g_Products != null)
                {
                    if (pwFunction.pwConst.GlobalUnit.g_Products.Clfs == "单相")
                    {
                        this.Clfs = enmClfs.单相;
                    }
                    else if (pwFunction.pwConst.GlobalUnit.g_Products.Clfs == "三相四线" || pwFunction.pwConst.GlobalUnit.g_Products.Clfs == "三相四")
                    {
                        this.Clfs = enmClfs.三相四线有功;
                    }
                    else if (pwFunction.pwConst.GlobalUnit.g_Products.Clfs == "三相三线" || pwFunction.pwConst.GlobalUnit.g_Products.Clfs == "三相三")
                    {
                        this.Clfs = enmClfs.三相三线有功;
                    }
                    else if (pwFunction.pwConst.GlobalUnit.g_Products.Clfs == "二相三线")
                    {
                        this.Clfs = enmClfs.二相三线;
                    }
                    this.Ub = Convert.ToSingle(pwFunction.pwConst.GlobalUnit.g_Products.Ub.ToUpper().Replace("V", ""));
                    this.Ib = Convert.ToSingle(pwFunction.pwConst.GlobalUnit.g_Products.Ib.ToUpper().Replace("A", ""));
                    this.IMax = Convert.ToSingle(pwFunction.pwConst.GlobalUnit.g_Products.IMax.ToUpper().Replace("A", ""));

                    string[] _strConstant = pwFunction.pwConst.GlobalUnit.g_Products.Constant.Split(new[] { "(" },StringSplitOptions.RemoveEmptyEntries);
                    this.Constant = Convert.ToInt32(_strConstant[0]);
                    if (_strConstant.Length > 1)
                    {
                        this.Constant_wg = Convert.ToInt32(_strConstant[1].Replace(")", ""));
                    }
                    else
                    {
                        this.Constant_wg = this.Constant;
                    }

                    string[] _strDJ = pwFunction.pwConst.GlobalUnit.g_Products.DJ.ToUpper().Replace("S", "").Split(new[] { "(" }, StringSplitOptions.RemoveEmptyEntries);
                    this.DJ = Convert.ToSingle(_strDJ[0]);
                    if (_strDJ.Length > 1)
                    {
                        this.DJ_wg = Convert.ToSingle(_strDJ[1].Replace(")", "").ToString());
                    }
                    else
                    {
                        this.DJ_wg = this.DJ;
                    }

                    this.PL = Convert.ToSingle(pwFunction.pwConst.GlobalUnit.g_Products.PL.ToUpper().Replace("HZ", ""));
                    if (pwFunction.pwConst.GlobalUnit.g_Products.DzType == "国网端子")
                    {
                        this._enmDzType = enmPulseDzType.国网端子;
                    }
                    else if (pwFunction.pwConst.GlobalUnit.g_Products.DzType == "南网端子")
                    {
                        this._enmDzType = enmPulseDzType.南网端子;
                    }

                    this.GYGY =(pwFunction.pwConst.GlobalUnit.g_Products.GYGY=="共阴"? enmGyGyType.共阴:enmGyGyType.共阳);
                    this.PulseType = (pwFunction.pwConst.GlobalUnit.g_Products.PulseType == "脉冲盒" ? enmPulseComType.脉冲盒 : enmPulseComType.光电头);

                    #region 继电器
                    /*
                    if (pwFunction.pwConst.GlobalUnit.g_Products.JDQType == "内置继电器")
                    {
                        this.enmJdqType = enmJDQType.内置继电器;
                    }
                    else if (pwFunction.pwConst.GlobalUnit.g_Products.JDQType == "外置继电器")
                    {
                        this.enmJdqType = enmJDQType.外置继电器;
                    }
                    else if (pwFunction.pwConst.GlobalUnit.g_Products.JDQType == "外置隔离继电器")
                    {
                        this.enmJdqType = enmJDQType.外置隔离继电器;
                    }
                    */
                    #endregion
                }
            }
            catch
            {
                throw new Exception("工单及产品配置信息出错！");
            }
        }

        ~MeterInfo()
        {

        }

    }
}
