using System;
using System.Text;

namespace pwFunction.pwConst
{
    /// <summary>
    ///  常量壭声明：C+标记符号+使用模块(全局用G)+_常量名
    /// C:Cus缩写，用作所有常量声明的第一个字母
    /// M:Mark缩写，用作标记常量声明
    /// T:Text缩写,用作文本内容标记声明
    /// 模块简写对照：
    /// SC:SystemConfig模块
    /// </summary>
    public class Variable
    {

        /// <summary>
        /// Grid表格常态颜色
        /// </summary>
        public static System.Drawing.Color Color_Grid_Normal = System.Drawing.Color.FromArgb(250, 250, 250);

        /// <summary>
        /// Grid表格间隔行颜色
        /// </summary>
        public static System.Drawing.Color Color_Grid_Alter = System.Drawing.Color.FromArgb(235, 250, 235);

        /// <summary>
        /// 固定行（列）颜色
        /// </summary>
        public static System.Drawing.Color Color_Grid_Frone = System.Drawing.Color.FromArgb(225, 225, 225);

        /// <summary>
        /// 不合格颜色
        /// </summary>
        public static System.Drawing.Color Color_Grid_BuHeGe = System.Drawing.Color.Red;

        /// <summary>
        /// 合格颜色
        /// </summary>
        public static System.Drawing.Color Color_Grid_HeGe = System.Drawing.Color.Lime;

        /// <summary>
        /// 测试项目总数，如果增加需要改总数
        /// </summary>
        public const int CONST_ENMMETERPRJIDCOUNT = 30;//enmMeterPrjID Count

        /// <summary>
        /// 未检文本内容
        /// </summary>
        public const string CTG_WeiJian = "未检";

        /// <summary>
        /// 合格文本内容
        /// </summary>
        public const string CTG_HeGe = "合格";

        /// <summary>
        /// 不合格文本内容
        /// </summary>
        public const string CTG_BuHeGe = "不合格";

        /// <summary>
        /// 合格标记。
        /// </summary>
        public const string CMG_HeGe = "√";

        /// <summary>
        /// 不合格标志
        /// </summary>
        public const string CMG_BuHeGe = "×";
        /// <summary>
        /// 服务器断开连接消息提示文本
        /// </summary>
        public const string CTG_SERVERUNCONNECT = "服务器断开连接";
        /// <summary>
        /// 项目检定完毕提示文本
        /// </summary>
        public const string CTG_VERIFYOVER = "所有项目检定完毕";
        /// <summary>
        /// 没有出误差默认值
        /// </summary>
        public const float WUCHA_INVIADE = -999F;


        #region 系统文档，配置文档，常量文档存放路径

        /// <summary>
        /// 系统配置XML文档路径
        /// </summary>
        public const string CONST_SYSTEMPATH = "\\System\\System.xml";
        /// <summary>
        /// 工单、产品、方案XML文档路径
        /// </summary>
        public const string CONST_WORKPLANPATH = "\\WorkPlan\\WorkPlan.xml";
        /// <summary>
        /// 功率因素角度配置XML文档路径
        /// </summary>
        public const string CONST_GLYSDICTIONARY = "\\Const\\GLYS.xml";

        /// <summary>
        /// 通信数据帧保存目录
        /// </summary>
        public const string CONST_COMMUNICATIONDATA = "\\Comm\\";

        /// <summary>
        /// 登入历史文件
        /// </summary>
        public const string CONST_HISTORY_LOGINPATH = "\\History\\Login.dat";
        /// <summary>
        /// 工单、产品、方案历史文件
        /// </summary>
        public const string CONST_HISTORY_WORKPLANPATH = "\\History\\WorkPlan.dat";

        /// <summary>
        /// 检定数据本地存储目录(临时使用)
        /// </summary>
        public const string CONST_METERDATA = "\\Data\\";
        /// <summary>
        /// 检定数据XML文件
        /// </summary>
        public const string CONST_METERDATAXML = "\\Data\\Data.xml";

        ///// <summary>
        ///// 系统字典配置XML文档路径
        ///// </summary>
        //public const string CONST_DICTIONARY = "\\Const\\ZiDian.xml";
        ///// <summary>
        ///// 电流倍数字典表
        ///// </summary>
        //public const string CONST_XIBDICTIONARY = "\\Const\\xIb.xml";
        ///// <summary>
        ///// 多功能项目字典
        ///// </summary>
        //public const string CONST_DGNDICTIONARY = "\\Const\\DngConfig.xml";
        ///// <summary>
        ///// 项目标识符字典
        ///// </summary>
        //public const string CONST_PROJIDENTIFIE= "\\Const\\ProjIdentifie.xml";
        ///// <summary>
        ///// 误差限字典文件
        ///// </summary>
        //public const string CONST_WCLIMIT = "\\Const\\WcLimit.Mdb";
        ///// <summary>
        ///// 多功能协议配置文件
        ///// </summary>
        //public const string CONST_DGNPROTOCOL = "\\Const\\DgnProtocol.xml";
        ///// <summary>
        ///// 脉冲通道配置文件
        ///// </summary>
        //public const string CONST_PULSETYPE ="\\Tmp\\Pulse.xml";



        //public const string CONST_WAITUPDATE = "\\WAITUPDATE";

        #endregion

        #region 系统配置键值

        #region 台体信息配置
        /// <summary>
        /// 台体编号
        /// </summary>
        public const string CTC_DESKNO = "DESKNO";
        /// <summary>
        /// 台体类型：单相台，三相台
        /// </summary>
        public const string CTC_DESKTYPE = "DESKTYPE";
        /// <summary>
        /// 表位数
        /// </summary>
        public const string CTC_BWCOUNT = "BWCOUNT";
        /// <summary>
        /// 显示灯泡每行个数
        /// </summary>
        public const string CTC_DESBWCOUNT = "CTC_DESBWCOUNT";

        /// <summary>
        /// 是否有CL191B与CL188L
        /// </summary>
        public const string CTC_ISCL191BCL188L = "CTC_ISCL191BCL188L";


        #endregion

        #region CL2018-1端口分配
        /// <summary>
        /// 2018IP
        /// </summary>
        public const string CTC_2018IP = "2018IP";
        /// <summary>
        /// 源端口
        /// </summary>
        public const string CTC_COMY = "COMY";
        /// <summary>
        /// 表端口
        /// </summary>
        public const string CTC_COMB = "COMB";
        /// <summary>
        /// 误差板端口
        /// </summary>
        public const string CTC_COMW = "COMW";
        /// <summary>
        /// 时基源端口
        /// </summary>
        public const string CTC_COMT = "COMT";

        #endregion

        #region 网络数据库
        /// <summary>
        /// SQL数据库服务器IP
        /// </summary>
        public const string CTC_SQL_SERVERIP = "SQL_SERVERIP";
        /// <summary>
        /// SQL数据库用户名
        /// </summary>
        public const string CTC_SQL_USERID = "SQL_USERID";
        /// <summary>
        /// SQL登录密码
        /// </summary>
        public const string CTC_SQL_PASSWORD = "SQL_PASSWORD";
        /// <summary>
        /// SQL数据库名
        /// </summary>
        public const string CTC_SQL_DATABASENAME = "SQL_DATABASENAME";
        #endregion

        #region 表位RS485端口参数配置
        public const string CTC_RS485_COM1 = "RS485_COM1s";
        public const string CTC_RS485_COM2 = "RS485_COM2s";
        public const string CTC_RS485_COM3 = "RS485_COM3s";
        public const string CTC_RS485_COM4 = "RS485_COM4s";
        public const string CTC_RS485_COM5 = "RS485_COM5s";
        public const string CTC_RS485_COM6 = "RS485_COM6s";
        public const string CTC_RS485_COM7 = "RS485_COM7s";
        public const string CTC_RS485_COM8 = "RS485_COM8s";
        public const string CTC_RS485_COM9 = "RS485_COM9s";
        public const string CTC_RS485_COM10 = "RS485_COM10s";
        public const string CTC_RS485_COM11 = "RS485_COM11s";
        public const string CTC_RS485_COM12 = "RS485_COM12s";
        public const string CTC_RS485_COM13 = "RS485_COM13s";
        public const string CTC_RS485_COM14 = "RS485_COM14s";
        public const string CTC_RS485_COM15 = "RS485_COM15s";
        public const string CTC_RS485_COM16 = "RS485_COM16s";
        public const string CTC_RS485_COM17 = "RS485_COM17s";
        public const string CTC_RS485_COM18 = "RS485_COM18s";
        public const string CTC_RS485_COM19 = "RS485_COM19s";
        public const string CTC_RS485_COM20 = "RS485_COM20s";
        public const string CTC_RS485_COM21 = "RS485_COM21s";
        public const string CTC_RS485_COM22 = "RS485_COM22s";
        public const string CTC_RS485_COM23 = "RS485_COM23s";
        public const string CTC_RS485_COM24 = "RS485_COM24s";
        #endregion

        #region 误差检定相关设置
        /// <summary>
        /// 每个误差点取几次误差参与计算
        /// </summary>
        public const string CTC_WC_TIMES_BASICERROR = "TIMES_BASICERROR";
        /// <summary>
        /// 标准偏差取几次误差参与计算
        /// </summary>
        public const string CTC_WC_TIMES_WINDAGE = "TIMES_WINDAGE";
        /// <summary>
        /// 每个点误差最大处理次数
        /// </summary>
        public const string CTC_WC_MAXTIMES = "WC_MAXTIMES";
        /// <summary>
        /// 每个点最大检定时间
        /// </summary>
        public const string CTC_WC_MAXSECONDS = "WC_MAXSECONDS";
        /// <summary>
        /// 跳差倍数判定
        /// </summary>
        public const string CTC_WC_JUMP = "WC_JUMP";
        /// <summary>
        /// IN电流
        /// </summary>
        public const string CTC_WC_IN = "WC_IN";
        /// <summary>
        /// 平均值保留小数位
        /// </summary>
        public const string CTC_WC_AVGPRECISION = "AVGPRECISION";
        /// <summary>
        /// 是否系统自动计算圈数
        /// </summary>
        public const string CTC_WC_SYSTEMQS = "SYSTEMQS";

        /// <summary>
        /// 误差获取方式
        /// </summary>
        public const string CTC_WC_BY = "WC_BY";
        #endregion

        #region 其它配置
        /// <summary>
        /// 源稳定时间
        /// </summary>
        public const string CTC_OTHER_POWERON_ATTERTIME = "POWERON_ATTERTIME";
        /// <summary>
        /// 写表最大等待时间
        /// </summary>
        public const string CTC_OTHER_MAXWAITDATABACKTIME = "MAXWAITDATABACKTIME";
        /// <summary>
        /// 写表参数间隔
        /// </summary>
        public const string CTC_OTHER_EQUIPSET_WAITTIME = "EQUIPSET_WAITTIME";
        /// <summary>
        /// 重试次数
        /// </summary>
        public const string CTC_OTHER_RETRY = "RETRY";
        /// <summary>
        /// 标准表分频系数
        /// </summary>
        public const string CTC_OTHER_DRIVERF = "DRIVERF";
        /// <summary>
        /// 电表是否扫描
        /// </summary>
        public const string CTC_READTABLENO = "READTABLENO";
        /// <summary>
        /// 电表长度
        /// </summary>
        public const string CTC_READTABLELENGTH = "READTABLELENGTH";
        #endregion

        #endregion


    }
}
