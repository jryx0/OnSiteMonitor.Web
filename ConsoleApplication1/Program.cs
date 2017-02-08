using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Compression;
using System.IO;
using System.Diagnostics;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFileName = @"format.sql";
            string outputFileName = @"format.sql.gz";




            

            FileStream inputStream =
                new FileStream(inputFileName, FileMode.Open, FileAccess.Read);
            FileStream outputStream =
                new FileStream(outputFileName, FileMode.Create, FileAccess.Write);

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

          



            //FileStream inputStream =
            //      new FileStream(inputFileName, FileMode.Open, FileAccess.Read);
            //FileStream outputStream =
            //    new FileStream(outputFileName, FileMode.Create, FileAccess.Write);

            //// 决定一次读取数剧的大小，这里是8KB
            //int bufferSize = 8192;
            //int bytesRead = 0;
            //byte[] buffer = new byte[bufferSize];

            //GZipStream compressionStream =
            //    new GZipStream(outputStream, CompressionMode.Compress);
            //// bytesRead返回每次读了多少数据，如果等于0就表示已经没有数据
            //// 可以读了
            //while ((bytesRead = inputStream.Read(buffer, 0, bufferSize)) > 0)
            //{
            //    // 把读到数组中的数据通过GZipStream写入到输出数据流
            //    compressionStream.Write(buffer, 0, bytesRead);
            //}
            //compressionStream.Close();

            //inputStream.Close();
            //outputStream.Close();


            //Console.WriteLine("Finished");
            //Console.ReadLine();
        }




        public static string ExeCommand(string WorkingDirectory, string commandText)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WorkingDirectory = WorkingDirectory;
            string strOutput = null;
            try
            {
                p.Start();
                p.StandardInput.WriteLine(commandText);
                p.StandardInput.WriteLine("exit");
                strOutput = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                p.Close();
            }
            catch (Exception e)
            {
                strOutput = e.Message;
            }
            return strOutput;
        }
    }
}
