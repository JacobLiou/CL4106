using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClAmMeterController
{
    /// <summary>
    /// 被检表多功能信息参数
    /// </summary>
    public class pwMeterProtocolInfo
    {
        /// <summary>
        /// 协议库名称
        /// </summary>
        public string DllFile;

        /// <summary>
        /// 协议类
        /// </summary>
        public string ClassName;

        /// <summary>
        /// 表号
        /// </summary>
        public string MeterNo;

        /// <summary>
        /// 被检表通信地址
        /// </summary>
        public string Address;

        /// <summary>
        /// 通信参数
        /// </summary>
        public string Setting;

        /// <summary>
        /// 用户代码
        /// </summary>
        public string UserID;

        /// <summary>
        /// 验证密码类型
        /// </summary>
        public int VerifyPasswordType;

        /// <summary>
        /// 写操作密码
        /// </summary>
        public string WritePassword;

        /// <summary>
        /// 写操作密码等级
        /// </summary>
        public string WritePswClass;

        /// <summary>
        /// 下发帧的唤醒符个数
        /// </summary>
        public int FECount;

        /// <summary>
        /// 当前协议名称
        /// </summary>
        public string CurXyName;

        /// <summary>
        /// 等待数据到达最大时间
        /// </summary>
        public int WaitDataRevTime;

        /// <summary>
        /// 数据间隔最大时间
        /// </summary>
        public int IntervalTime;


        /// <summary>
        /// 构造函数()
        /// </summary>
        public pwMeterProtocolInfo()
        {
            this.DllFile = "pwMeterProtocol";     //动态
            this.ClassName = "DLT645_1997";
            this.MeterNo = "000000000001";
            this.Address = "000000000000";
            //this.Setting = "2400,e,8,1";
            this.UserID = "12345678";
            this.VerifyPasswordType = 1;
            this.WritePassword = "000000";
            this.WritePswClass = "00";
            this.FECount = 1;
            this.CurXyName = "DLT645_1997";
            this.WaitDataRevTime = 3000;
            this.IntervalTime = 500;
        }

        public pwMeterProtocolInfo(string strClassName, string Setting)
        {
            //if (strClassName == CurXyName) return;
            if (strClassName == "DLT645_1997" || strClassName == "DL/T645-1997")
            {
                this.DllFile = "pwMeterProtocol";     //动态
                this.ClassName = "DLT645_1997";
                this.MeterNo = "000000000001";
                this.Address = "AAAAAAAAAAAA";
                this.Setting = Setting;// "2400,e,8,1";
                this.UserID = "12345678";
                this.VerifyPasswordType = 1;
                this.WritePassword = "000000";
                this.WritePswClass = "00";
                this.FECount = 1;
                this.CurXyName = "DLT645_1997";
                this.WaitDataRevTime = 2500;
                this.IntervalTime = 500;


            }
            else if (strClassName == "DLT645_2007" || strClassName == "DL/T645-2007")
            {

                this.DllFile = "pwMeterProtocol";     //动态
                this.ClassName = "DLT645_2007";
                this.MeterNo = "000000000001";
                this.Address = "AAAAAAAAAAAA";
                this.Setting = Setting;// "2400,e,8,1";"9600,e,8,1"; //
                this.UserID = "00000000";
                this.VerifyPasswordType = 1;
                this.WritePassword = "000000";
                this.WritePswClass = "02";
                this.FECount = 1;
                this.CurXyName = "DLT645_2007";
                this.WaitDataRevTime = 2500;
                this.IntervalTime = 500;
                
            }
            else if (strClassName == "PowerConsume")
            {

                this.DllFile = "pwMeterProtocol";     //动态
                this.ClassName = "PowerConsume";
                this.MeterNo = "000000000001";
                this.Address = "AAAAAAAAAAAA";
                this.Setting = Setting;// "2400,e,8,1";
                this.UserID = "00000000";
                this.VerifyPasswordType = 1;
                this.WritePassword = "000000";
                this.WritePswClass = "00";
                this.FECount = 1;
                this.CurXyName = "PowerConsume";
                this.WaitDataRevTime = 2500;
                this.IntervalTime = 500;
            }
            else if (strClassName == "IEC62056-21")
            {

                this.DllFile = "pwMeterProtocol";     //动态
                this.ClassName = "IEC62056_21";
                this.MeterNo = "000000000001";
                this.Address = "AAAAAAAAAAAA";
                this.Setting = Setting;// "2400,e,8,1";
                this.UserID = "00000000";
                this.VerifyPasswordType = 1;
                this.WritePassword = "000000";
                this.WritePswClass = "02";
                this.FECount = 1;
                this.CurXyName = "IEC62056_21";
                this.WaitDataRevTime = 250000;
                this.IntervalTime = 500;
            }
            else
            {

                this.DllFile = "pwMeterProtocol";     //动态
                this.ClassName = "DLT645_2007";
                this.MeterNo = "000000000001";
                this.Address = "AAAAAAAAAAAA";
                //this.Setting = Setting;// "2400,e,8,1";
                this.Setting = Setting;// "2400,e,8,1";//2015 1218 "9600,e,8,1"; // 
                this.UserID = "00000000";
                this.VerifyPasswordType = 1;
                this.WritePassword = "000000";
                this.WritePswClass = "02";
                this.FECount = 1;
                this.CurXyName = "DLT645_2007";
                this.WaitDataRevTime = 2500;
                this.IntervalTime = 500;
            }


        }

    }
}
