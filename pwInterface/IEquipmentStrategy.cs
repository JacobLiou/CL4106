using System;
using System.Collections.Generic;
using System.Text;

namespace ClInterface
{
    /// <summary>
    /// 设备控制策略
    /// </summary>
    public interface IEquipmentStrategy
    {
        /// <summary>
        /// 执行操作
        /// </summary>
        /// <returns>操作是否成功.如果不成功则抛出Comm.Exceptions.EquipmentControlException异常</returns>
        bool Action();
        /// <summary>
        /// 取消当前操作
        /// </summary>
        /// <returns></returns>
        void Cancel();
    }
}
