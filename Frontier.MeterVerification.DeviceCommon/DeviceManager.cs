using System;
using System.Collections.Generic;

using System.Text;

namespace Frontier.MeterVerification.DeviceCommon
{
    /// <summary>
    /// 设备管理器
    /// </summary>
    public class DeviceManager
    {
        private static Dictionary<Type, CommPortDevice> dictDevice = new Dictionary<Type, CommPortDevice>();
        private static Dictionary<Type, object> dictDeviceInterface = new Dictionary<Type, object>();

        public static Dictionary<Type, CommPortDevice> DictDevice
        {
            get
            {
                return dictDevice;
            }
        }
        /// <summary>
        /// 注册设备
        /// </summary>
        /// <param name="type"></param>
        /// <param name="device"></param>
        public static void RegisterDevice(Type type, CommPortDevice device)
        {
            dictDevice.Add(type, device);
        }
        /// <summary>
        /// 根据接口获取单个设备
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetDevice<T>() where T : class
        {
            T ret = default(T);
            if (dictDeviceInterface.ContainsKey(typeof(T)))
            {
                ret = dictDeviceInterface[typeof(T)] as T;
            }
            else
            {
                foreach (CommPortDevice acc in dictDevice.Values)
                {
                    ret = acc as T;
                    if (ret != null)
                    {
                        dictDeviceInterface.Add(typeof(T), acc);
                        break;
                    }
                }
            }
            return ret;
        }

        public static List<T> GetDevices<T>() where T : class
        {
            T ret = default(T);
            List<T> rets = new List<T>();

            foreach (CommPortDevice acc in dictDevice.Values)
            {
                ret = acc as T;
                if (ret != null)
                {
                    rets.Add(ret);
                }
            }
            return rets;
        }
    }
}
