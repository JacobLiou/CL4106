using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
namespace pwClassLibrary.pwFile
{
    /// <summary>
    /// 文件操作类
    /// </summary>
    public class File
    {




        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, byte[] retVal, int size, string filePath);

        #region 写INI文件void WriteInIString(string inifile, string Section, string Ident, string Value)
        /// <summary>
        /// 写INI文件
        /// </summary>
        /// <param name="inifile"></param>
        /// <param name="Section"></param>
        /// <param name="Ident"></param>
        /// <param name="Value"></param>
        public static void WriteInIString(string inifile, string Section, string Ident, string Value)
        {
            try
            {
                inifile = GetPhyPath(inifile);
                if (System.IO.File.Exists(inifile) == false)
                {
                    System.IO.File.Create(inifile).Close();
                }
                WritePrivateProfileString(Section, Ident, Value, inifile);
            }
            catch { }
        }
        #endregion

        #region 读取INI文件string ReadInIString(string inifile, string Section, string Ident, string Default)
        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="inifile"></param>
        /// <param name="Section"></param>
        /// <param name="Ident"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static string ReadInIString(string inifile, string Section, string Ident, string Default)
        {
            try
            {
                inifile = GetPhyPath(inifile);
                if (System.IO.File.Exists(inifile) == false)
                {
                    System.IO.File.Create(inifile).Close();
                }

                Byte[] Buffer = new Byte[65535];
                int bufLen = GetPrivateProfileString(Section, Ident, Default, Buffer, Buffer.GetUpperBound(0), inifile);
                //必须设定0（系统默认的代码页）的编码方式，否则无法支持中文
                string s = Encoding.GetEncoding(0).GetString(Buffer);
                s = s.Substring(0, bufLen);
                return s.Trim();
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion


        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="FileName">文件路径</param>
        public static void RemoveFile(string FilePath)
        {
            System.IO.File.Delete(FilePath);
        }

        /// <summary>
        /// 根据相对路径获取文件、文件夹绝对路径
        /// </summary>
        /// <param name="FileName">相对路径</param>   
        /// <returns></returns>
        public static string GetPhyPath(string FileName)
        {
            FileName = FileName.Replace('/', '\\');             //规范路径写法
            if (FileName.IndexOf(':') != -1) return FileName;   //已经是绝对路径了
            if (FileName.Length > 0 && FileName[0] == '\\') FileName = FileName.Substring(1);
            return string.Format("{0}\\{1}", System.Windows.Forms.Application.StartupPath, FileName);
        }

        /// <summary>
        /// 获取文件后缀名（带.）
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static string GetExtName(string FileName)
        {
            int index = FileName.LastIndexOf(".");
            if (index == -1)
            {
                return string.Empty;
            }
            return FileName.Substring(index);
        }

        /// <summary>
        /// 创建文件、如果目录不存在则自动创建、路径既可以是绝对路径也可以是相对路径
        /// 返回文件数据流，如果创建失败在返回null、如果文件存在则打开它
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static FileStream Create(string FileName)
        {
            FileName = GetPhyPath(FileName);
            string folder = FileName.Substring(0, FileName.LastIndexOf('\\') + 1);

            string tmpFolder = folder.Substring(0, FileName.IndexOf('\\')); //磁盘跟目录
            //逐层创建文件夹
            try
            {
                while (tmpFolder != folder)
                {
                    tmpFolder = folder.Substring(0, FileName.IndexOf('\\', tmpFolder.Length) + 1);
                    if (!System.IO.Directory.Exists(tmpFolder))
                        System.IO.Directory.CreateDirectory(tmpFolder);
                }
            }
            catch { return null; }

            if (System.IO.File.Exists(FileName))
            {
                return System.IO.File.Open(FileName, FileMode.Open, FileAccess.ReadWrite);
                //return null;
            }
            else
            {
                try
                {
                    return System.IO.File.Create(FileName);
                }
                catch { return null; }
            }
        }

        private static object objWriteFileDataLock = new object();

        /// <summary>
        /// 写入二进制数据到文件
        /// </summary>
        /// <param name="strFilePath">要写入的本地文件名路径[相对/绝对路径]</param>
        /// <param name="byData">要写入的数据</param>
        /// <returns>返回写入结论[成功Y/N]</returns>
        public static bool WriteFileData(string strFilePath, byte[] byData)
        {
            lock (objWriteFileDataLock)
            {
                FileStream _FS;
                bool _Result = false;
                //路径转化
                string _FilePath = GetPhyPath(strFilePath);
                //首先查找,不存在则建立一个新文件
                if (!System.IO.File.Exists(_FilePath))
                {
                    _FS = Create(_FilePath);
                }
                else
                {
                    try
                    {
                        _FS = new FileStream(_FilePath, FileMode.Truncate, FileAccess.Write, FileShare.None);
                    }
                    catch {
                        return false;
                    }
                }
                //开始写
                try
                {
                    try
                    {
                        _FS.Write(byData, 0, byData.Length);
                        _FS.Flush();
                        _Result = true;
                    }
                    catch { }
                }
                catch
                {
                    _Result = false;
                }
                _FS.Close();
                _FS.Dispose();
                return _Result;
            }
        }

        /// <summary>
        /// 二进制方式读取文件到缓冲区
        /// </summary>
        /// <param name="strFilePath">要读取的文件路径</param>
        /// <param name="byData">读入缓冲区</param>
        /// <returns>返回读取结果[成功Y/N ]</returns>
        public static byte[] ReadFileData(string strFilePath)
        {
            string _FilePath = GetPhyPath(strFilePath);
            FileStream _FS;
            
            if (!System.IO.File.Exists(_FilePath))
            {
                try
                {

                    System.IO.File.Create(_FilePath).Close();
                }
                catch { }
                return new byte[] { 0 };
            }
            try
            {
                _FS = new FileStream(_FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                byte[] _TmpByte = new byte[_FS.Length];
                _FS.Read(_TmpByte, 0, _TmpByte.Length);
                _FS.Close();
                return _TmpByte;
            }
            catch
            {
                return new byte[] { 0 };
            }
        }

        /// <summary>
        /// 获取指定目录下的文件，
        /// </summary>
        /// <param name="FolderPath"></param>
        /// <param name="bIncludeChildFolder"></param>
        /// <returns></returns>
        public static string[] ListFolder(string FolderPath, bool bIncludeChildFolder)
        {
            System.Collections.Generic.Stack<string> lstFiles = new Stack<string>();
            string sourceDir = GetPhyPath(FolderPath);
            if (!System.IO.Directory.Exists(sourceDir))
                return new string[] { };
            if (sourceDir[sourceDir.Length -1] != '\\')
                sourceDir = sourceDir + "\\";

            FileInfo[] FileInfo;
            DirectoryInfo rootDicrctoty = new DirectoryInfo(sourceDir);
            FileInfo = rootDicrctoty.GetFiles("*.*", SearchOption.AllDirectories);
            for (int i = 0; i < FileInfo.Length; i++)
            {
                lstFiles.Push(FileInfo[i].FullName.Substring(sourceDir.Length));
            }
            return lstFiles.ToArray();
        }

        /// <summary>
        /// 运行外部EXE程序
        /// </summary>
        /// <param name="strFilePath">执行文件路径</param>
        /// <param name="processName">进程名字</param>
        public static void RunOtherExe(string strFilePath, string processName)
        {
            strFilePath = pwClassLibrary.pwFile.File.GetPhyPath(strFilePath);
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length > 0)
            {
                pwClassLibrary.pwWin32Api.Win32Api.ShowWindow((int)processes[0].MainWindowHandle, 1);
                pwClassLibrary.pwWin32Api.Win32Api.SetForegroundWindow((int)processes[0].MainWindowHandle);
                return;
            }
            System.Diagnostics.Process pHand = null;
            pHand = new System.Diagnostics.Process();
            pHand.StartInfo.FileName = strFilePath;
            pHand.StartInfo.UseShellExecute = false;
            //pHand.StartInfo.UserName = processName;
            try
            {
                pHand.Start();
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
            }
        }
    }
}
