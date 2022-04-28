using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace pwInterface
{
    /// <summary>
    /// 标准表参数结构体
    /// </summary>
    /// 
    [Serializable()]
    public struct stPower
    {
        /// <summary>
        /// A相电压
        /// </summary>
        public float Ua;
        /// <summary>
        /// B相电压
        /// </summary>
        public float Ub;
        /// <summary>
        /// C相电压
        /// </summary>
        public float Uc;

        /// <summary>
        /// A相电流
        /// </summary>
        public float Ia;
        /// <summary>
        /// B相电流
        /// </summary>
        public float Ib;
        /// <summary>
        /// C相电流
        /// </summary>
        public float Ic;

        public float P;
        public float Pa;
        public float Pb;
        public float Pc;
        public float Phi_Ia;
        public float Phi_Ib;
        public float Phi_Ic;
        public float Phi_Ua;
        public float Phi_Ub;
        public float Phi_Uc;
        public float Q;
        public float Qa;
        public float Qb;
        public float Qc;
        public float S;
        public float Sa;
        public float Sb;
        public float Sc;



    }



    /// <summary>
    /// 系统参数
    /// </summary>
    [Serializable()]
    public struct StSystemInfo
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 项目值
        /// </summary>
        public string Value;
        /// <summary>
        /// 项目描述
        /// </summary>
        public string Description;
        /// <summary>
        /// 分类名称
        /// </summary>
        public string ClassName;
        /// <summary>
        /// 数据源，存在多个数据用管道符号分隔(是|否)（三相台|单相台）
        /// </summary>
        public string DataSource;
    }



    
    /// <summary>
    /// 误差检定
    /// </summary>
    [Serializable()]
    public struct StWcPoint
    {
        public string PrjID;
        public string DLHL;//电流回路
        public string PrjName;
        public string GLFX;//功率方向
        public string YJ;//功率元件
        public string GLYS;//功率因数
        public string xIb;//电流
        public string Qs;//圈数
        public string fMax;//误差上限
        public string fMin;//误差下限
        public string PC;//偏差
        public void Init()
        {
            PrjID = "";
            DLHL = "";
            PrjName = "";
            GLFX = "";
            YJ = "";
            GLYS = "";
            xIb = "";
            Qs = "";
            fMax = "";
            fMin = "";
            PC = "";            
        }

        public CheckPoint SetCheckPoint()
        {
            try
            {
                CheckPoint m_CheckPoint = new CheckPoint();
                m_CheckPoint.PrjID = this.PrjID;
                m_CheckPoint.PrjName = this.PrjName;
                m_CheckPoint.DLHL = Convert.ToInt32(this.DLHL);
                m_CheckPoint.YJ = SetCurYJ(this.YJ);
                m_CheckPoint.GLFX = SetCurGLFX(this.GLFX);
                m_CheckPoint.GLYS = this.GLYS;
                m_CheckPoint.xIb = Convert.ToSingle(this.xIb.ToUpper().Replace("IB", ""));
                m_CheckPoint.QS = Convert.ToInt32(this.Qs);
                m_CheckPoint.MaxError = Convert.ToSingle(fMax);
                m_CheckPoint.MinError = Convert.ToSingle(fMin); ;
                m_CheckPoint.PC = Convert.ToBoolean(this.PC) ? 1 : 0;
                return m_CheckPoint;
            }
            catch
            {
                throw new Exception("错误的误差检定点参数列表");

            }

        }

        /// <summary>
        /// 获取功率元件
        /// </summary>
        /// <param name="strYJ"></param>
        /// <returns></returns>
        private   enmElement SetCurYJ(string strYJ)
        {
            enmElement _YJ = enmElement.H;
            switch (strYJ)
            {
                case "合元":
                    _YJ = enmElement.H;
                    break;
                case "A元":
                    _YJ = enmElement.A;
                    break;
                case "B元":
                    _YJ = enmElement.B;
                    break;
                case "C元":
                    _YJ = enmElement.C;
                    break;

            }
            return _YJ;
        }
        /// <summary>
        /// 获取功率方向
        /// </summary>
        /// <param name="strGLFX"></param>
        /// <returns></returns>
        private  enmsPowerFangXiang SetCurGLFX(string strGLFX)
        {
            enmsPowerFangXiang _GLFX = enmsPowerFangXiang.正向有功;
            switch (strGLFX)
            {
                case "正有向有功":
                    _GLFX = enmsPowerFangXiang.正向有功;
                    break;
                case "反向有功":
                    _GLFX = enmsPowerFangXiang.反向有功;
                    break;
                case "正向无功":
                    _GLFX = enmsPowerFangXiang.正向无功;
                    break;
                case "反向无功":
                    _GLFX = enmsPowerFangXiang.反向无功;
                    break;

            }
            return _GLFX;
        }


    }

    /// <summary>
    /// 多功能方案项目
    /// </summary>
    [Serializable()]
    public struct StDgnItem
    {
        /// <summary>
        /// 多功能项目ID
        /// </summary>
        public string PrjID;
        /// <summary>
        /// 多功能项目名称  
        /// </summary>
        public string PrjName;
        /// <summary>
        /// 源输出参数
        /// </summary>
        public string OutPramerter;
        /// <summary>
        /// 项目检定参数
        /// </summary>
        public string PrjParameter;

        public void Init()
        {
            PrjID = "";
            PrjName = "";
            OutPramerter = "";
            PrjParameter = "";
        }

        public override string ToString()
        {
            return PrjName;
        }
    }

    /// <summary>
    /// 校准误差项目参数
    /// </summary>
    [Serializable()]
    public class AdjustErrorParaItem
    {

        public string ParaID;
        public string ParaName;
        public string ParaValue;
        public AdjustErrorParaItem()
        {
            ParaID = "";
            ParaName = "";
            ParaValue = "";

        }
    }

    /// <summary>
    /// 源输出参数
    /// </summary>
    [Serializable()]
    public struct stPowerPramerter
    {
        /// <summary>
        /// 功率方向
        /// </summary>
        public enmsPowerFangXiang GLFX;
        /// <summary>
        /// 元件
        /// </summary>
        public enmElement YJ;
        /// <summary>
        /// 电压倍数
        /// </summary>
        public float xU;
        /// <summary>
        /// 电流倍数
        /// </summary>
        public string xIb;
        /// <summary>
        /// 功率因素
        /// </summary>
        public string GLYS;
        /// <summary>
        /// 组合结构体参数
        /// </summary>
        /// <returns></returns>
        public string Jion()
        {
            return ((int)GLFX).ToString() + "|" + ((int)YJ).ToString() + "|" + xU.ToString() + "|" + xIb + "|" + GLYS;
        }

        /// <summary>
        /// 分解结构体参数组合字符串
        /// </summary>
        /// <param name="PramValue"></param>
        public void Split(string PramValue)
        {
            string[] _TmpPram = PramValue.Split('|');
            if (_TmpPram.Length != 5)
            {
                GLFX = enmsPowerFangXiang.正向有功;
                YJ = enmElement.H;
                xU = 1F;
                xIb = "0Ib";
                GLYS = "1.0";
                return;
            }
            GLFX = (enmsPowerFangXiang)int.Parse(_TmpPram[0]);
            YJ = (enmElement)int.Parse(_TmpPram[1]);
            xU = float.Parse(_TmpPram[2]);
            xIb = _TmpPram[3];
            GLYS = _TmpPram[4];
            return;
        }
    }

    /// <summary>
    /// 作业项目参数
    /// </summary>
    public class  stPrjParameter
    {

        public string str_XyName;           //协议
        public string str_Code;             //标识编码
        public int int_Len;                 //长度
        public int int_Dot;                 //小数点
        public string str_SendParameter;    //下发参数

        public int int_AdjustModel;         //校准模式，0=功率校准，1=误差校准
        public float flt_Ib5OrIb2;          //小电流校准点，0.22Ib,0.25Ib
        public int int_AdjustStep;          //校准步数字节
        public List<AdjustErrorParaItem> AdjustParaItem;//校准参数文件

        public int int_HighFrequencyPulseBS;//高频检定倍数



        public long lng_TimePulseConst;     //基准时钟常数
        public int int_TimePulsePL ;        //时钟频率
        public int int_TimePulseQs;         //圈数

        public string str_Address;          //统一地址
        public int int_Count ;              //记数器

        public int M1 = 1;                  //Bit0;0=单板测试  1=整机测试;  Bit1 0=三相四 1=三相三
        public int M2 = 1;                  //0：外控磁保持  1：内控磁保持   2：外控电保持
        public string str_SelfCheckItem;    //自检测试子项目，4个字节32项

        public string str_SoftVer;          //软件版本号

        public float flt_PowerMin = 0f;     //整机耗最小值
        public float flt_PowerMax = 2f;     //整机耗最大值


        public float flt_EnergyMin = 1f;     //底度最小值
        public float flt_EnergyMax = 5f;     //底度最大值

        public float flt_UMaxSinglePhaseTest = 5f;//其他相电压的上限设定值--分相供电测试

        public float []flt_WcACSamplingTest =new float[]{0.4f,0.4f,.04f};//电压、电流、有功功率的误差限%

        //======以下备用=========
        public int int_Parameter1;
        public int int_Parameter2;
        public int int_Parameter3;

        public string str_Parameter1;
        public string str_Parameter2;
        public string str_Parameter3;

        public float flt_Parameter1;
        public float flt_Parameter2;
        public float flt_Parameter3;

        public float[] flt_ParameterArry;
        public string[] str_ParameterArry;
        public byte[] byt_ParameterArry;

        public stPrjParameter()
        {
            str_XyName = "DLT645_1997";           //协议
            str_Code="";                          //标识编码
            int_Len=0;                            //长度
            int_Dot=0;                            //小数点
            str_SendParameter="";                 //下发参数

            int_AdjustModel=0;                  //校准模式，0=功率校准，1=误差校准
            flt_Ib5OrIb2 = 0.05f;               //小电流校准点，0.02Ib,0.05Ib
            int_AdjustStep=0x7C;                //校准步数字节
            AdjustParaItem = new List<AdjustErrorParaItem>();//校准误差参数文件

            int_HighFrequencyPulseBS= 1;        //高频检定倍数
            flt_UMaxSinglePhaseTest = 5;

            lng_TimePulseConst = 5000000;       //基准时钟常数
            int_TimePulsePL = 1;                //时钟频率
            int_TimePulseQs = 1;

            str_Address = "111111111111";       //统一地址
            int_Count = 0;                      //记数器

            M1 = 1;                             //Bit0;0=单板测试  1=整机测试;  Bit1 0=三相四 1=三相三
            M2 = 1;                             //0：外控磁保持  1：内控磁保持   2：外控电保持
            str_SelfCheckItem = "00003DA67F";   //自检测试子项目，4个字节32项 (XY6表测试项)

        }

    }






    /// <summary>
    /// 详细误差数据、第一次误差、第二次误差...
    /// </summary>
    [Serializable()]
    public struct StError
    {
        private List<float> LstError;
        /// <summary>
        /// 添加一个误差
        /// </summary>
        /// <param name="fValue"></param>
        public void Add(float fValue)
        {
            if (LstError == null) LstError = new List<float>();
            LstError.Add(fValue);
        }
        /// <summary>
        /// 误差查询
        /// </summary>
        /// <param name="index">误差次数</param>
        /// <returns>误差值</returns>
        public float this[int index]
        {
            get
            {
                if (LstError == null) LstError = new List<float>();
                return LstError[index];
            }
            set
            {
                if (LstError == null) LstError = new List<float>();
                LstError[index] = value;
            }
        }
        /// <summary>
        /// 误差总数量
        /// </summary>
        public int Count
        {
            get
            {
                if (LstError == null) LstError = new List<float>();
                return LstError.Count;
            }
        }
        /// <summary>
        /// 清空误差
        /// </summary>
        public void Clear()
        {
            if (LstError == null) LstError = new List<float>();
            LstError.Clear();
        }

    }


    /// <summary>
    /// 误差点
    /// </summary>
    [Serializable()]
    public struct CheckPoint
    {

        /// <summary>
        /// 项目ID
        /// </summary>
        public string PrjID;

        /// <summary>
        /// 项目名称
        /// </summary>
        public string PrjName;

        /// <summary>
        /// 电流回路
        /// </summary>
        public int DLHL ;

        /// <summary>
        /// 功率方向
        /// </summary>
        public enmsPowerFangXiang GLFX ;

        /// <summary>
        /// 功率元件
        /// </summary>
        public enmElement YJ ;


        /// <summary>
        /// 功率因数
        /// </summary>
        public string GLYS ;

        /// <summary>
        /// 负载电流xIb 
        /// </summary>
        public float xIb ;

        /// <summary>
        /// 检测圈数
        /// </summary>
        public int QS ;

        /// <summary>
        /// 计算1个脉冲所需时间
        /// </summary>
        public int OneTime;


        /// <summary>
        /// 误差上限(默认值-999)
        /// </summary>
        public float MaxError;

        /// <summary>
        /// 误差下限（默认值-999）
        /// </summary>
        public float MinError;

        /// <summary>
        /// 是否是偏差0不是1是
        /// </summary>
        public int PC ;



        /// <summary>
        /// 计算获取需要检定圈数
        /// </summary>
        /// <param name="_CurClfs">测量方式</param>
        /// <param name="_CurUb">电压</param>
        /// <param name="_CurConst">常数</param>
        public void SetLapCount(enmClfs _CurClfs, float _CurUb, long _CurConst)
        {//(p*t*k)/(1000*3600) '30
            try
            {
                enmElement _CurYJ = this.YJ;//元件
                float _CurIb = this.xIb;//电流
                string _CurGLYS = this.GLYS;//功率因数
                int _CutT = 5;//5秒钟出一个误差

                string str_CL = _CurGLYS.ToUpper().Substring(_CurGLYS.Length - 1, 1);
                Double glys_XS = 1;
                if (str_CL == "C" || str_CL == "L")
                    glys_XS = Convert.ToDouble(_CurGLYS.Substring(0, _CurGLYS.Length - 1));
                else
                    glys_XS = Convert.ToDouble(_CurGLYS);

                double _yj_Xs = 1;
                if (_CurYJ == enmElement.H)
                {
                    if (_CurClfs == enmClfs.单相)
                        _yj_Xs = 1;
                    else if (_CurClfs == enmClfs.三相四线有功 || _CurClfs == enmClfs.三相四线无功)
                        _yj_Xs = 3;
                    else if (_CurClfs == enmClfs.三相三线有功 || _CurClfs == enmClfs.三相三线无功)
                        _yj_Xs = 1.732;
                }
                else if (_CurYJ == enmElement.A)
                    _yj_Xs = 1;
                else if (_CurYJ == enmElement.B)
                    _yj_Xs = 1;
                else if (_CurYJ == enmElement.C)
                    _yj_Xs = 1;

                this.QS = Convert.ToInt32((_yj_Xs * _CurUb * _CurIb * glys_XS * _CutT * _CurConst) / (1000 * 3600));

                if (this.QS < 1) this.QS = 1;
            }
            catch 
            {
                throw new Exception("错误的检定点参数列表");

            }

        }

        /// <summary>
        /// 计算1圈所需时间
        /// </summary>
        /// <param name="_CurClfs">测量方式</param>
        /// <param name="_CurUb">电压</param>
        /// <param name="_CurConst">电流</param>
        /// <param name="intHighFrequencyPulseBS">高频倍数</param>
        /// <returns>秒</returns>
        public void SetOneQsTime(enmClfs _CurClfs, float _CurUb, long _CurConst, int intHighFrequencyPulseBS)
        {// Time= 1 / ( (p*k) / (3600*1000) )
            try
            {
                enmElement _CurYJ = this.YJ;//元件
                float _CurIb = this.xIb;//电流
                string _CurGLYS = this.GLYS;//功率因数

                string str_CL = _CurGLYS.ToUpper().Substring(_CurGLYS.Length - 1, 1);
                Double glys_XS = 1;
                if (str_CL == "C" || str_CL == "L")
                    glys_XS = Convert.ToDouble(_CurGLYS.Substring(0, _CurGLYS.Length - 1));
                else
                    glys_XS = Convert.ToDouble(_CurGLYS);

                double _yj_Xs = 1;
                if (_CurYJ == enmElement.H)
                {
                    if (_CurClfs == enmClfs.单相)
                        _yj_Xs = 1;
                    else if (_CurClfs == enmClfs.三相四线有功 || _CurClfs == enmClfs.三相四线无功)
                        _yj_Xs = 3;
                    else if (_CurClfs == enmClfs.三相三线有功 || _CurClfs == enmClfs.三相三线无功)
                        _yj_Xs = 1.732;
                }
                else if (_CurYJ == enmElement.A)
                    _yj_Xs = 1;
                else if (_CurYJ == enmElement.B)
                    _yj_Xs = 1;
                else if (_CurYJ == enmElement.C)
                    _yj_Xs = 1;

                Single  _CurTime =Convert .ToSingle( 1 / ((_yj_Xs * _CurUb * _CurIb * glys_XS * _CurConst) / (3600 *1000)) ) ;//秒


                this.OneTime = Convert.ToInt32((_CurTime * 1000) / ( intHighFrequencyPulseBS * 2 ) );// 豪秒/2

                if (this.OneTime < 50) this.QS = 50;
            }
            catch
            {
                this.QS = 50; 
                throw new Exception("错误的检定点参数列表");

            }

        }



        public override string ToString()
        {
            return PrjName;
        }


    }


}
