using System;
using System.Collections.Generic;
using System.Text;
using pwClassLibrary.DataBase;
using System.Xml;

namespace pwFunction.pwSystemModel
{
    public class SystemInfo
    {
        /// <summary>
        /// 系统配置模型
        /// </summary>
        public SystemConfigure SystemMode;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SystemInfo()
        {
            SystemMode = new SystemConfigure();
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~SystemInfo()
        {            
            SystemMode = null;
        }

        /// <summary>
        /// 初始化系统模型
        /// </summary>
        public void Load()
        {
            SystemMode.Load();
        }

        /// <summary>
        /// 存储XML系统模型
        /// </summary>
        public void Save()
        {
            SystemMode.Save();
        }


    }
}
