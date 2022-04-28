using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace pwClassLibrary.pwFile
{
    public class Folder
    {
        /// <summary>
        /// ��ȡָ��Ŀ¼�µ������ļ�����
        /// </summary>
        /// <param name="FolderPath">�ļ���·��</param>
        /// <param name="Searchpattern">�ļ���������(*.xml|*.mdb)</param>
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
        /// ��ȡָ��Ŀ¼�µ������ļ�����
        /// </summary>
        /// <param name="FolderPath">�ļ���·��</param>
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
        /// ��ȡָ��Ŀ¼�µ������ļ�·��
        /// </summary>
        /// <param name="FolderPath">�ļ���·��</param>
        /// <param name="Searchpattern">�ļ���������(*.xml|*.mdb)</param>
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
        /// ��ȡָ��Ŀ¼�µ������ļ�·��
        /// </summary>
        /// <param name="FolderPath">�ļ���·��</param>
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
