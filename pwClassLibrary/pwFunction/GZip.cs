using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;


namespace pwClassLibrary
{
    /// <summary>
    /// 压缩/解要所数据
    /// </summary>
    public class Gzip
    {

        /// <summary>
        /// 压缩数据
        /// </summary>
        /// <param name="bySrc"></param>
        /// <returns></returns>
        public static bool GZIPCompress(ref byte[] bySrc)
        {
            MemoryStream ms = new MemoryStream();
            GZipStream gzipStream = new GZipStream(ms, CompressionMode.Compress, true);
            gzipStream.Write(bySrc, 0, bySrc.Length);
            gzipStream.Close();
            ms.Position = 0;
            bySrc = new byte[ms.Length];
            ms.Read(bySrc, 0, bySrc.Length);
            ms.Close();
            ms.Dispose();
            gzipStream.Dispose();
            return true;
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="bySrc"></param>
        /// <returns></returns>
        public static bool GZIPDecompress(ref byte[] bySrc)
        {
            MemoryStream ms = new MemoryStream();
            ms.Write(bySrc, 0, bySrc.Length);
            ms.Seek(0, SeekOrigin.Begin);
            GZipStream gzipStream = new GZipStream(ms, CompressionMode.Decompress);

            //开始解压数据
            int totallen = 0;
            List<byte[]> LstBuffer = new List<byte[]>();

            while (true)
            {
                byte[] buffer = new byte[bySrc.Length];
                int readlen = gzipStream.Read(buffer, 0, buffer.Length);
                if (readlen == 0)
                {
                    break;
                }
                totallen += readlen;
                LstBuffer.Add(buffer);
            }

            //把解压的数据串在一起
            bySrc = new byte[totallen];
            for (int i = 0; i < LstBuffer.Count; i++)
            {
                if (LstBuffer.Count - 1 == i)
                {
                    Array.Copy(LstBuffer[i], 0, bySrc, LstBuffer[i].Length * i, totallen - LstBuffer[i].Length * i);
                }
                else
                {
                    Array.Copy(LstBuffer[i], 0, bySrc, LstBuffer[i].Length * i, LstBuffer[i].Length);
                }
            }

            ms.Close();
            ms.Dispose();
            gzipStream.Close();
            gzipStream.Dispose();
            return true;
        }


    }
}
