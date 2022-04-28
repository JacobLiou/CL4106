using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
namespace pwClassLibrary.pwFile
{
    /// <summary>
    /// �ļ�������
    /// </summary>
    public class File
    {




        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, byte[] retVal, int size, string filePath);

        #region дINI�ļ�void WriteInIString(string inifile, string Section, string Ident, string Value)
        /// <summary>
        /// дINI�ļ�
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

        #region ��ȡINI�ļ�string ReadInIString(string inifile, string Section, string Ident, string Default)
        /// <summary>
        /// ��ȡINI�ļ�
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
                //�����趨0��ϵͳĬ�ϵĴ���ҳ���ı��뷽ʽ�������޷�֧������
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
        /// ɾ���ļ�
        /// </summary>
        /// <param name="FileName">�ļ�·��</param>
        public static void RemoveFile(string FilePath)
        {
            System.IO.File.Delete(FilePath);
        }

        /// <summary>
        /// �������·����ȡ�ļ����ļ��о���·��
        /// </summary>
        /// <param name="FileName">���·��</param>   
        /// <returns></returns>
        public static string GetPhyPath(string FileName)
        {
            FileName = FileName.Replace('/', '\\');             //�淶·��д��
            if (FileName.IndexOf(':') != -1) return FileName;   //�Ѿ��Ǿ���·����
            if (FileName.Length > 0 && FileName[0] == '\\') FileName = FileName.Substring(1);
            return string.Format("{0}\\{1}", System.Windows.Forms.Application.StartupPath, FileName);
        }

        /// <summary>
        /// ��ȡ�ļ���׺������.��
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
        /// �����ļ������Ŀ¼���������Զ�������·���ȿ����Ǿ���·��Ҳ���������·��
        /// �����ļ����������������ʧ���ڷ���null������ļ����������
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static FileStream Create(string FileName)
        {
            FileName = GetPhyPath(FileName);
            string folder = FileName.Substring(0, FileName.LastIndexOf('\\') + 1);

            string tmpFolder = folder.Substring(0, FileName.IndexOf('\\')); //���̸�Ŀ¼
            //��㴴���ļ���
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
        /// д����������ݵ��ļ�
        /// </summary>
        /// <param name="strFilePath">Ҫд��ı����ļ���·��[���/����·��]</param>
        /// <param name="byData">Ҫд�������</param>
        /// <returns>����д�����[�ɹ�Y/N]</returns>
        public static bool WriteFileData(string strFilePath, byte[] byData)
        {
            lock (objWriteFileDataLock)
            {
                FileStream _FS;
                bool _Result = false;
                //·��ת��
                string _FilePath = GetPhyPath(strFilePath);
                //���Ȳ���,����������һ�����ļ�
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
                //��ʼд
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
        /// �����Ʒ�ʽ��ȡ�ļ���������
        /// </summary>
        /// <param name="strFilePath">Ҫ��ȡ���ļ�·��</param>
        /// <param name="byData">���뻺����</param>
        /// <returns>���ض�ȡ���[�ɹ�Y/N ]</returns>
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
        /// ��ȡָ��Ŀ¼�µ��ļ���
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
        /// �����ⲿEXE����
        /// </summary>
        /// <param name="strFilePath">ִ���ļ�·��</param>
        /// <param name="processName">��������</param>
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
