using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.DeviceCommon
{
    public static class CommPortParity
    {
        /// <summary>
        /// 无校验
        /// </summary>
        public const string None = "N";

        /// <summary>
        /// 奇校验
        /// </summary>
        public const string Odd = "O";

        /// <summary>
        /// 偶校验
        /// </summary>
        public const string Even = "E";

        /// <summary>
        /// 保留为1
        /// </summary>
        public const string Mark = "M";

        /// <summary>
        /// 保留为0
        /// </summary>
        public const string Space = "S";
    }
}
