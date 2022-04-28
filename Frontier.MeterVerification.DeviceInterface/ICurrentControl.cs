using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.DeviceInterface
{
    /// <summary>
    /// 科陆档位控制器
    /// </summary>
    public interface ICurrentControl
    {
        /// <summary>
        /// 三相装置需设置CT档位控制器
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        bool SetCurrentControl(float current);
    }
}
