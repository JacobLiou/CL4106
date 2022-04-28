using System;
using System.Collections.Generic;

using System.Text;
using VerificationEquipment.Commons;

namespace Frontier.MeterVerification.DeviceCommon
{
    public class PhaseEnergy : EnergyValue
    {
        public PhaseEnergy(string energy)
        {
            if (!string.IsNullOrEmpty(energy))
            {
                string[] subEnergy = energy.Split(',');

                if (subEnergy.Length > 0)
                {
                    总 = string.Format("{0:f2}", CheckHelper.FormatStringToDouble(subEnergy[0]));
                    for (int i = 1; i < subEnergy.Length; i++)
                    {
                        switch (i)
                        {
                            case (int)Phase.尖:
                                尖 = string.Format("{0:f2}", CheckHelper.FormatStringToDouble(subEnergy[i]));
                                break;
                            case (int)Phase.峰:
                                峰 = string.Format("{0:f2}", CheckHelper.FormatStringToDouble(subEnergy[i]));
                                break;
                            case (int)Phase.平:
                                平 = string.Format("{0:f2}", CheckHelper.FormatStringToDouble(subEnergy[i]));
                                break;
                            case (int)Phase.谷:
                                谷 = string.Format("{0:f2}", CheckHelper.FormatStringToDouble(subEnergy[i]));
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取分时电量
        /// </summary>
        /// <param name="phase">时段</param>
        /// <returns></returns>
        public string GetPhaseEnergy(Phase? phase)
        {
            string phaseEnergy = this.总;
            if (phase.HasValue)
            {
                switch (phase.Value)
                {
                    case Phase.尖:
                        phaseEnergy = this.尖;
                        break;
                    case Phase.峰:
                        phaseEnergy = this.峰;
                        break;
                    case Phase.平:
                        phaseEnergy = this.平;
                        break;
                    case Phase.谷:
                        phaseEnergy = this.谷;
                        break;
                }
            }

            return phaseEnergy;
        }
    }
}
