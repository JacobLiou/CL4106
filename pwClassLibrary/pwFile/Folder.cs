using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace pwClassLibrary.pwFile
{
    public class Folder
    {
        /// <summary>
        /// 获取指定目录下的所有文件名称
        /// </summary>
        /// <param name="FolderPath">文件夹路径</param>
        /// <param name="Searchpattern">文件类型条件(*.xml|*.mdb)</param>
        /// <returns></returns>
        public static List<string> getFileNames(string FolderPath,string Searchpattern)
        {
            List<string> _Files = new List<string>();
            if (!Directory.Exists(FolderPath))
                return _Files;
            foreach (string _Name in Directory.GetFiles(FolderPath, Searchpattern))
            {
                string _Tmp=_Name.ToLower().Replace(FolderPath.ToLower() + "\\", "");
                _Tmp = _Tmp.ToLower().Replace(Searchpattern.Substring(1).ToLower(),"");
                _Files.Add(_Tmp);
            }
            return _Files;
        }
        /// <summary>
        /// 获取指定目录下的所有文件名称
        /// </summary>
        /// <param name="FolderPath">文件夹路径</param>
        /// <returns></returns>
        public static List<string> getFileNames(string FolderPath)
        {
            List<string> _Files = new List<string>();
            if (!Directory.Exists(FolderPath))
                return _Files;
            foreach (string _Name in Directory.GetFiles(FolderPath))
            {
                string _Tmp = _Name.ToLower().Replace(FolderPath.ToLower() + "\\", "");
                _Tmp = _Tmp.Substring(0, _Tmp.LastIndexOf('.'));
                _Files.Add(_Tmp);
            }
            return _Files;
        }
        /// <summary>
        /// 获取指定目录下的所有文件路径
        /// </summary>
        /// <param name="FolderPath">文件夹路径</param>
        /// <param name="Searchpattern">文件类型条件(*.xml|*.mdb)</param>
        /// <returns></returns>
        public static List<string> getFilePaths(string FolderPath, string Searchpattern)
        {
            List<string> _Files = new List<string>();
            if (!Directory.Exists(FolderPath))
                return _Files;
            foreach (string _Name in Directory.GetFiles(FolderPath, Searchpattern))
            {
                _Files.Add(_Name);
            }
            return _Files;
        }
        /// <summary>
        /// 获取指定目录下的所有文件路径
        /// </summary>
        /// <param name="FolderPath">文件夹路径</param>
        /// <returns></returns>
        public static List<string> getFilePaths(string FolderPath)
        {
            List<string> _Files = new List<string>();
            if (!Directory.Exists(FolderPath))
                return _Files;
            foreach (string _Name in Directory.GetFiles(FolderPath))
            {
                _Files.Add(_Name);
            }
            return _Files;
        }
    }
}
