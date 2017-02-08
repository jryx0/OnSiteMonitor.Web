using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace WFM.JW.HB.Models
{
    public class DataHelper
    {

        /// <summary>  
        /// 字符串压缩  
        /// </summary>  
        /// <param name="strSource"></param>  
        /// <returns></returns>  
        public static byte[] Compress(byte[] data)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true);
                zip.Write(data, 0, data.Length);
                zip.Close();
                byte[] buffer = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(buffer, 0, buffer.Length);
                ms.Close();
                return buffer;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>  
        /// 字符串解压缩  
        /// </summary>  
        /// <param name="strSource"></param>  
        /// <returns></returns>  
        public static byte[] Decompress(byte[] data)
        {
            try
            {
                MemoryStream ms = new MemoryStream(data);
                GZipStream zip = new GZipStream(ms, CompressionMode.Decompress, true);
                MemoryStream msreader = new MemoryStream();
                byte[] buffer = new byte[0x1000];
                while (true)
                {
                    int reader = zip.Read(buffer, 0, buffer.Length);
                    if (reader <= 0)
                    {
                        break;
                    }
                    msreader.Write(buffer, 0, reader);
                }
                zip.Close();
                ms.Close();
                msreader.Position = 0;
                buffer = msreader.ToArray();
                msreader.Close();
                return buffer;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static string CompressString(string str)
        {
            string compressString = "";
            byte[] compressBeforeByte = Encoding.GetEncoding("UTF-8").GetBytes(str);
            byte[] compressAfterByte = Compress(compressBeforeByte);
            //compressString = Encoding.GetEncoding("UTF-8").GetString(compressAfterByte);    
            compressString = Convert.ToBase64String(compressAfterByte);
            return compressString;
        }

        public static string DecompressString(string str)
        {
            string compressString = "";
            //byte[] compressBeforeByte = Encoding.GetEncoding("UTF-8").GetBytes(str);    
            byte[] compressBeforeByte = Convert.FromBase64String(str);
            byte[] compressAfterByte = Decompress(compressBeforeByte);
            compressString = Encoding.GetEncoding("UTF-8").GetString(compressAfterByte);
            return compressString;
        }
    }

    public class CompressHelper
    {
        public static String DeCompressFile(string inputFile, string outputFile)
        {

            if (File.Exists(outputFile))
                File.Delete(outputFile);

            FileStream inputStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read);
            FileStream outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write);

            int bufferSize = 8192;
            int bytesRead = 0;
            byte[] buffer = new byte[bufferSize];
            GZipStream decompressionStream =
                new GZipStream(inputStream, CompressionMode.Decompress);
            // 把压缩了的数据通过GZipStream解压缩后再读出来
            // 读出来的数据就存在数组里
            while ((bytesRead = decompressionStream.Read(buffer, 0, bufferSize)) > 0)
            {
                // 把解压后的数据写入到输出数据流
                outputStream.Write(buffer, 0, bytesRead);
            }
            decompressionStream.Close();

            inputStream.Close();
            outputStream.Close();

            return outputFile;
        }

        public static String CompressFile(string inputFile, string outputFile)
        {
            FileStream inputStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            FileStream outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write);

            // 决定一次读取数剧的大小，这里是8KB
            int bufferSize = 8192;
            int bytesRead = 0;
            byte[] buffer = new byte[bufferSize];

            GZipStream compressionStream =
                new GZipStream(outputStream, CompressionMode.Compress);
            // bytesRead返回每次读了多少数据，如果等于0就表示已经没有数据
            // 可以读了
            while ((bytesRead = inputStream.Read(buffer, 0, bufferSize)) > 0)
            {
                // 把读到数组中的数据通过GZipStream写入到输出数据流
                compressionStream.Write(buffer, 0, bytesRead);
            }
            compressionStream.Close();

            inputStream.Close();
            outputStream.Close();

            return outputFile;
        }
    }


}
