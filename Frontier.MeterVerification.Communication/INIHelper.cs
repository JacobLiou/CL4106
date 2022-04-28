using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Frontier.MeterVerification.Communication
{
    public class INIHelper
    {
        public string inipath;

        /// <summary>
        /// 修改INI文件中内容
        /// </summary>
        /// <param name="section">欲在其中写入的节点名称</param>
        /// <param name="key">欲设置的项名</param>
        /// <param name="val">要写入的新字符串</param>
        /// <param name="filePath">INI文件名</param>
        /// <returns>非零表示成功，零表示失败</returns>
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern long WritePrivateProfileString(
            string section,
            string key,
            string val,
            string filePath);

        /// <summary>
        /// 为INI文件中指定的节点取得字符串
        /// </summary>
        /// <param name="section">欲在其中查找关键字的节点名称</param>
        /// <param name="key">欲获取的项名</param>
        /// <param name="def">指定的项没有找到时返回的默认值</param>
        /// <param name="retVal">指定一个字串缓冲区，长度至少为nSize</param>
        /// <param name="size">指定装载到lpReturnedString缓冲区的最大字符数量</param>
        /// <param name="filePath">INI文件名</param>
        /// <returns>复制到lpReturnedString缓冲区的字节数量，其中不包括那些NULL中止字符</returns>
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern int GetPrivateProfileString(
            string section,
            string key,
            string def,
            StringBuilder retVal,
            int size,
            string filePath);

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="INIPath">文件路径</param>
        public INIHelper(string iniName)
        {
            inipath = Application.StartupPath + "\\" + iniName;
        }

        /// <summary>
        /// 写入INI文件
        /// </summary>
        /// <param name="Section">INI节点项(如 [TypeName] )</param>
        /// <param name="Key">节点下的项</param>
        /// <param name="Value">值</param>
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.inipath);
        }

        /// <summary>
        /// 读出INI文件
        /// </summary>
        /// <param name="Section">节点项(如 [TypeName] )</param>
        /// <param name="Key">键</param>
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(5000);
            int i = GetPrivateProfileString(Section, Key, "", temp, 5000, this.inipath);
            return temp.ToString();
        }

        /// <summary>
        /// 验证文件是否存在
        /// </summary>
        /// <returns>布尔值</returns>
        public bool ExistINIFile()
        {
            return System.IO.File.Exists(inipath);
        }
    }
}
