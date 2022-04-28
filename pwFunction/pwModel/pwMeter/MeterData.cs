using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pwInterface;
using pwClassLibrary;
namespace pwFunction.pwMeter
{

    [Serializable()]
    public class MeterData : pwSerializable 
    {
        public string  _readdatecheck = "";

        public string  readdatecheck
        {
            set
            {
                _readdatecheck = value;
            }
            get
            {
                return _readdatecheck;
            }
        }
        /// <summary>
        /// 是否要检(检定用，作业过程可更改)
        /// </summary>
        public bool bolIsCheck = true;

        /// <summary>
        /// 表位号		在表架上所挂位置
        /// </summary>
        public int intBno = 0;

        /// <summary>
        /// 条形码	
        /// </summary>
        public string chrTxm = "";
    
        /// <summary>
        /// 生产编号	
        /// </summary>
        public string chrScbh = "";

        /// <summary>
        /// 表通信地址
        /// </summary>
        public string chrAddr = "";

        /// <summary>
        /// 电能量(当前正向有功总)
        /// </summary>
        public float sngEnergy = 0f;

        /// <summary>
        /// 软件版本号	
        /// </summary>
        public string chrVer = "";

        /// <summary>
        /// 功耗
        /// </summary>
        public float sngPower = 0f;

        /// <summary>
        /// 总结论  名称
        /// </summary>
        public string Me_PrjName = "总结论";

        /// <summary>
        /// 总结论		false/true
        /// </summary>
        public bool  bolResult = true  ;

        private string _chrResult= pwConst.Variable.CTG_WeiJian;
        /// <summary>
        /// 总结论		合格/不合格
        /// </summary>
        public string chrResult
        {
            set
            {
                if (value != pwConst.Variable.CTG_WeiJian) _bolAlreadyTest = true;
                _chrResult = value;
            }
            get
            {
                return _chrResult;
            }
        }

        private bool _bolAlreadyTest = false;
        /// <summary>
        /// 是否已经测试   true=已检 / false=未检
        /// </summary>
        public bool bolAlreadyTest
        {
            set
            {
                _bolAlreadyTest = value;
            }
            get
            {
                return _bolAlreadyTest;
            }
        }


        private bool _bolSaveData = true;
        /// <summary>
        /// 是否需要保存数据，默认保存   true=保存 / false=不保存
        /// </summary>
        public bool bolSaveData
        {
            set
            {
                _bolSaveData = value;
            }
            get
            {
                return _bolSaveData;
            }
        }


        /// <summary>
        /// 是否上传到服务器
        /// </summary>
        public bool bolToServer = false;

        /// <summary>
        /// 不合明细
        /// </summary>
        public string chrRexplain=""; 

        /// <summary>
        /// 电能表检定项目结论集；Key值为检定项目ID编号格式化字符串。格式为[检定项目ID号]参照enmMeterPrjID
        /// </summary>
        public Dictionary<string, DataResultBasic> MeterResults = new Dictionary<string, DataResultBasic>();


        /// <summary>
        /// 电能表误差集；Key值为项目Prj_ID值， 偏差怎么描述呢？
        /// </summary>
        public Dictionary<string, MeterErrorItem> MeterErrors = new Dictionary<string, MeterErrorItem>();

        /// <summary>
        /// 电能表多功能数据集； Key值为项目Prj_ID值
        /// </summary>
        public Dictionary<string, MeterDgnItem> MeterDgns = new Dictionary<string, MeterDgnItem>();



        public MeterData()
        {
            this.chrTxm = "";
            this.chrAddr = "";
            this.chrScbh = "";
            this.sngEnergy = 0f;
            this.chrVer = "";
            this.sngPower = 0f;
            this.bolResult = true;
            this._chrResult = pwConst.Variable.CTG_WeiJian;
            this._bolAlreadyTest = false;
            this._bolSaveData = true;
            this._readdatecheck = "1";
            MeterResults.Clear();
            MeterErrors.Clear();
            MeterDgns.Clear();
        }
       
      
    }

}
