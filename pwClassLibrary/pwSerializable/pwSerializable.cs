using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
namespace pwClassLibrary
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    [Serializable()]
    public class pwSerializable
    {
        public pwSerializable()
        {

        }

        #region  序列化、反序列化 XmlSerializer
        /// <summary>
        /// 序列化到XML文件
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public Object XMLSerialize(string FileName)
        {
            try
            {
                Stream stream = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);

                XmlSerializer xs = new XmlSerializer(this.GetType());

                xs.Serialize(stream, this);

                stream.Close();
                return this;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 从XML反序列化到object
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static Object XMLDeserialize(string FileName)
        {
            if (!File.Exists(FileName)) return null;
            try
            {
                Stream stream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                Object obj;
                XmlSerializer xs = new XmlSerializer(typeof(Object));
                obj = xs.Deserialize(stream) as Object;
                stream.Close();
                return obj;
            }
            catch
            {
                return null;
            }
        }
        #endregion




        #region 序列化、反序列化 BinaryFormatter
        /// <summary>
        /// 序列化
        /// </summary>
        public byte[] GetBytes()
        {
            byte[] binaryDataResult = null;
            MemoryStream memStream = new MemoryStream();
            IFormatter brFormatter = new BinaryFormatter();

            brFormatter.Serialize(memStream, this);
            binaryDataResult = memStream.ToArray();
            memStream.Close();
            memStream.Dispose();
            return binaryDataResult;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="byPacket"></param>
        /// <returns></returns>
        public static pwSerializable GetObject(byte[] byPacket)
        {
            MemoryStream memStream = new MemoryStream(byPacket);
            IFormatter brFormatter = new BinaryFormatter();
            pwSerializable obj;
            try
            {
                obj = (pwSerializable)brFormatter.Deserialize(memStream);
            }
            catch
            {
                throw new InvalidCastException("序列化对象结构发生变化，请删除缓存文件后再操作");
            }
            memStream.Close();
            memStream.Dispose();
            return obj;
        }
        #endregion

        #region 保存到文件
        /// <summary>
        /// 保存到文件
        /// </summary>
        /// <param name="FileName">文件绝对路径</param>
        /// <returns></returns>
        public bool SaveToFile(string FileName)
        {
            FileStream tmpFs = null;
            try
            {
                tmpFs = File.Open(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            }
            catch
            {
                return false;
            }
            byte[] byWrite = this.GetBytes();
            tmpFs.Position = 0;
            tmpFs.Write(byWrite, 0, byWrite.Length);
            tmpFs.Flush();
            tmpFs.Close();
            tmpFs.Dispose();
            return true;
        }
        #endregion

        #region 从文件读取
        /// <summary>
        /// 从文件读取
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns>失败返回 null</returns>
        public static pwSerializable ReadFromFile(string FileName)
        {
            if (!File.Exists(FileName)) return null;
            FileStream tmpFs = null;
            try
            {
                tmpFs = File.Open(FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch
            {
                return null;
            }
            byte[] byRead = new byte[tmpFs.Length];
            tmpFs.Read(byRead, 0, byRead.Length);
            tmpFs.Flush();
            tmpFs.Close();
            tmpFs.Dispose();
            try
            {
                return GetObject(byRead);
            }
            catch { }
            return null;
        }
        #endregion




    }
}
