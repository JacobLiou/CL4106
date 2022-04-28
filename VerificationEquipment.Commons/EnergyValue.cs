using System;
using System.Collections.Generic;
using System.Text;

namespace VerificationEquipment.Commons
{
    /// <summary>
    /// 表底数定义
    /// </summary>
    public class EnergyValue
    {
        /// <summary>
        /// 总电量，单位kWh
        /// </summary>
        public string 总 { get; set; }

        /// <summary>
        /// 峰电量，单位kWh
        /// </summary>
        public string 峰 { get; set; }

        /// <summary>
        /// 平电量，单位kWh
        /// </summary>
        public string 平 { get; set; }

        /// <summary>
        /// 谷电量，单位kWh
        /// </summary>
        public string 谷 { get; set; }

        /// <summary>
        /// 尖电量，单位kWh
        /// </summary>
        public string 尖 { get; set; }

        public override string ToString()
        {
            return string.Format("总:{0}  峰:{1}  平:{2}  谷:{3}  尖:{4}", 总, 峰, 平, 谷, 尖);
        }
    }
}
