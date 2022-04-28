using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CL4101_QZ_GW
{
    /// <summary>
    /// IniFile 的摘要说明。
    /// </summary>
    public class IniFile
    {
        // 文件INI名称
        private static string filepath;

        public static string FilePath
        {
            get
            {
                return filepath;
            }

            set
            {
                filepath = value;
            }
        }

        // 声明读写INI文件的API函数 
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public IniFile()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        ///  写INI文件
        /// </summary>
        /// <param name="Section">Ini标格名</param>
        /// <param name="Key">键名</param>
        /// <param name="Value">值</param>

        public static void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, filepath);
        }

        /// <summary>
        ///  读取INI文件指定
        /// </summary>
        /// <param name="Section">Ini标格名</param>
        /// <param name="Key">键名</param>
        /// <returns>返回值</returns>
        public static string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(511);

            int i = GetPrivateProfileString(Section, Key, "", temp, 511, filepath);

            return temp.ToString();
        }
    }
}
