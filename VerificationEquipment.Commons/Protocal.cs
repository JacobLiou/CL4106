using System;
using System.Collections.Generic;
using System.Text;

namespace VerificationEquipment.Commons
{
    /// <summary>
    /// 规约
    /// </summary>
    public class Protocal
    {
        private string _clearPowerPassword;
        private string _clearDemandPassword;
        private string _writeTimePassword;
        private string _clearEventPassword;
        private int _fe;                    //唤醒FE数目
        private int _returnFE;              //返回FE数目

        /// <summary>
        /// 清电量密码
        /// </summary>
        public string ClearPowerPassword
        {
            get { return _clearPowerPassword; }
            set { _clearPowerPassword = value; }
        }

        /// <summary>
        /// 清需量密码
        /// </summary>
        public string ClearDemandPassword
        {
            get { return _clearDemandPassword; }
            set { _clearDemandPassword = value; }
        }

        /// <summary>
        /// 写时间密码
        /// </summary>
        public string WriteTimePassword
        {
            get { return _writeTimePassword; }
            set { _writeTimePassword = value; }
        }

        /// <summary>
        /// 清事件密码
        /// </summary>
        public string ClearEventPassword
        {
            get { return _clearEventPassword; }
            set { _clearEventPassword = value; }
        }

        /// <summary>
        /// 唤醒FE数目
        /// </summary>
        public int FECount
        {
            get { return _fe; }
            set { _fe = value; }
        }

        /// <summary>
        /// 返回FE数目
        /// </summary>
        public int ReturnFECount
        {
            get { return _returnFE; }
            set { _returnFE = value; }
        }

    }
}
