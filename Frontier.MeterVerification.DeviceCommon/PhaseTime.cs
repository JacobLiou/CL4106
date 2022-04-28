using System;
using System.Collections.Generic;

using System.Text;
using VerificationEquipment.Commons;

namespace Frontier.MeterVerification.DeviceCommon
{
    /// <summary>
    /// 费率时
    /// </summary>
    public class PhaseTime
    {
        private Dictionary<Phase, List<string>> dictPhaseTime = new Dictionary<Phase, List<string>>();

        /// <summary>
        /// 费率
        /// </summary>
        private List<int> lstPeriod = new List<int>();

        /// <summary>
        /// 费率时间
        /// </summary>
        private List<string> lstPeriodTime = new List<string>();

        public PhaseTime(List<string> periodTimes)
        {
            if (periodTimes != null
                && periodTimes.Count > 0)
            {
                for (int i = 0; i < periodTimes.Count; i++)
                {
                    if (periodTimes[i].Length == 6)
                    {
                        Phase phase = (Phase)Enum.Parse(typeof(Phase), periodTimes[i].Substring(4, 2));
                        string phaseTime = string.Format("{0}:{1}", periodTimes[i].Substring(0, 2), periodTimes[i].Substring(2, 2));

                        lstPeriod.Add((int)phase);
                        lstPeriodTime.Add(phaseTime);

                        if (!dictPhaseTime.ContainsKey(phase))
                        {
                            dictPhaseTime.Add(phase, new List<string>());
                        }

                        dictPhaseTime[phase].Add(phaseTime);
                    }
                }
            }
        }

        /// <summary>
        /// 费率相应费率时间集合
        /// </summary>
        public Dictionary<Phase, List<string>> DictPhaseTime
        {
            get
            {
                return dictPhaseTime;
            }
        }

        /// <summary>
        /// 连续费率集合
        /// </summary>
        public int[] Period
        {
            get
            {
                return lstPeriod.ToArray();
            }
        }

        /// <summary>
        /// 连续费率时间集合
        /// 与费率集合相对应
        /// </summary>
        public string[] PeriodTime
        {
            get
            {
                return lstPeriodTime.ToArray();
            }
        }
    }
}
