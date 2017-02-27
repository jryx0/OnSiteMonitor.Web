using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using WFM.JW.HB.Services;
using WFM.JW.HB.Models;
using System.IO;
using System.IO.Compression;
using System.Data;

namespace TMPA.FileMonitor
{
    public class Importor
    {
        BackgroundWorker _backWorker;
        public Importor(BackgroundWorker bw)
        {
            _backWorker = bw;
        }

        public void DataImportor()
        {
            _backWorker.ReportProgress(-1, "-读数据库，查询是否有新上传的问题线索文件");
            WFMUploadTaskServices wts = new WFMUploadTaskServices();

            IParametersSpecification spec = new CriteriaSpecification("TStatus", CriteriaOperator.Equal, 1);
            spec = spec.And(new CriteriaSpecification("a.Status", CriteriaOperator.Equal, 10));
            List<WFMUploadTask> wList = wts.GetAllData(spec).ToList();

            if (wList == null || wList.Count == 0)
            {
                _backWorker.ReportProgress(-1, "-没有新上传的问题线索文件");
                return;
            }

            foreach (var ut in wList)
            {
                WFMRegionClueSeverices rcs = new WFMRegionClueSeverices(ut.RegionCode);
                try
                {
                    //update status
                    ut.Status = 15;
                    var mes = wts.UpdateData(ut);

                    //int Affect  = sqlserver.ExecuteNonQuery(@"Update [dbo].[WFM_UploadTask] set 
                    //                       Status = 15 where Status = 10 and version = " + ut.Version.ToString() 
                    //                       + " and TaskGuid = '" + ut.TaskGuid.ToString()+"'");
                    if (mes.retType != 100)
                        continue;

                    //Decompress
                    string datafile = GetImportFileName(ut);
                    if (datafile.Length == 0)
                        continue;

                    //PreImportData(sqlserver, ut);
                    rcs.InitTable();

                    //import data to table
                    //ImportData(sqlserver, datafile, ut);
                    ImportData(rcs, datafile, ut);
                    File.Delete(datafile);


                    ut.Status = 20;
                    ut.ErrorMessage = "导入数据成功";
                    ut.OpTime = System.DateTime.Now;

                    wts.UpdateData(ut);
                    //sqlserver.ExecuteNonQuery(@"Update [dbo].[WFM_UploadTask] set 
                    //                       Status = 20, ErrorMessage = '导入数据成功' where Status = 15 and TaskGuid = '"
                    //                       + ut.TaskGuid.ToString() + "'");


                    mes = rcs.GenerateReport(ut.RegionCode, ut.RegionGuid);
                    if (mes.retType != 100)
                        throw new Exception(mes.Message);

                    _backWorker.ReportProgress(-1, "-完成文件：" + ut.RegionName + "文件导入！");
                }
                catch (Exception ex)
                {
                    _backWorker.ReportProgress(-1, "-导入数据库出错！" + ex.Message);
                    try
                    {
                        string str = ex.Message;
                        if (str.Length > 499)
                            str = str.Substring(0, 498);
                        ut.ErrorMessage = str;
                        ut.Status = 16;
                        ut.OpTime = System.DateTime.Now;

                        wts.UpdateData(ut);
                    }
                    catch (Exception ex1)
                    {
                    }
                }
            }

            // sqlserver.CloseConnection();
            _backWorker.ReportProgress(100, "-完成导入！");
        }


        private string GetImportFileName(WFMUploadTask uTask)
        {
            _backWorker.ReportProgress(-1, "-发现" + uTask.UserRegionPath.Trim() + "上传的文件，" +
               uTask.FileName.Trim() + "！开始解压");

            //depress data
            string datafile = DepressData(uTask.FilePath.Trim() + "\\" + uTask.FileName.Trim());
            if (datafile.Length == 0)
            {
                throw new Exception("- 未发现文件" + uTask.FileName.Trim());
            }
            else

                _backWorker.ReportProgress(-1, "-完成" + uTask.UserRegionPath.Trim() + "解压");
            return datafile;
        }

        private string DepressData(string compressfile)
        {
            if (!File.Exists(compressfile)) throw new Exception(compressfile + "*文件" + compressfile + "不存在！");

            string outputFileName = compressfile.Substring(0, compressfile.Length - 7);
            try
            {
                if (File.Exists(outputFileName))
                    File.Delete(outputFileName);


                FileStream inputStream = new FileStream(compressfile, FileMode.Open, FileAccess.Read);
                FileStream outputStream = new FileStream(outputFileName, FileMode.Create, FileAccess.Write);

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

            }
            catch (Exception ex)
            {
                throw new Exception(compressfile + "*解压文件" + outputFileName + "失败！" + ex.Message);
            }

            _backWorker.ReportProgress(-1, "-完成" + compressfile + "的文件");
            _backWorker.ReportProgress(-1, "*解压文件" + outputFileName + "成功！");
            return outputFileName;
        }

        private void ImportData(WFMRegionClueSeverices rcs, String dataFile, WFMUploadTask uTask)
        {
            using (DAL.MySqlite resultDB = new DAL.MySqlite(dataFile, ""))
            {

                var o = resultDB.ExecuteScalar("Select count() from  Clue_report");
                int total = int.Parse(o.ToString());
                if (o != null)
                    _backWorker.ReportProgress(-1, "*总共问题线索：" + total.ToString());
                else
                {
                    throw new Exception("上传的数据库没有数据");
                }

                int iProgress = 0;
                _backWorker.ReportProgress(0, "导入 0 / " + total.ToString() + "条数据！");

                for (int i = 0; i <= total / 10000; i++)
                {
                    var ds = resultDB.ExecuteDataset(@"SELECT 
                                                       subStr(ID, 0, 20) AS ID,
                                                       subStr(Name, 0, 50) AS Name,
                                                       subStr(Addr, 0, 200) AS Addr,
                                                       subStr(Region, 0, 50) AS Region,
                                                       subStr(Type, 0, 400) AS Type,                                                       
                                                       subStr(DateRange, 0, 250) AS DataRange,
                                                       subStr(Comment, 0, 250) AS Comment,
                                                       Table1,
                                                       InputError,
                                                       RowID
                                                  FROM Clue_report limit " + (i * 10000).ToString() + ",10000");

                    if (ds == null)
                        throw new Exception("DB Error!");

                    iProgress += ds.Tables[0].Rows.Count;

                    List<Clues> cList = new List<Clues>();
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        Clues c = new Clues();
                        c.ID = r[0].ToString();
                        c.Name = r[1].ToString();
                        c.Addr = r[2].ToString();
                        if (c.Addr.Length > 180)
                            c.Addr = c.Addr.Substring(0, 180);

                        c.Region = r[3].ToString();
                        c.Type = r[4].ToString();
                        c.DateRange = r[5].ToString();
                        c.Comment = r[6].ToString();

                        int Table1 = 0;
                        int.TryParse(r[7].ToString(), out Table1);
                        c.Table1 = Table1;

                        int InputError = 0;
                        int.TryParse(r[8].ToString(), out InputError);
                        c.InputError = InputError;

                        if (c.Type.IndexOf("+") == -1)
                            if (c.Type.IndexOf("不一致") != -1)
                                c.InputError = 1;


                        int RowID = 0;
                        int.TryParse(r[9].ToString(), out RowID);
                        c.RowID = RowID;

                        cList.Add(c);
                    }

                    var mes = rcs.InsertData(cList);
                    if (mes.retType != 100)
                        throw new Exception("插入数据库失败：" + mes.Message);

                    if (total != 0)
                        _backWorker.ReportProgress((int)(iProgress * 100 / total), "导入 " + iProgress.ToString() + " / " + total.ToString() + "条数据！");

                    cList.Clear();
                }

                _backWorker.ReportProgress(100, "数据导入完成");
                resultDB.CloseConnection();
            }

            _backWorker.ReportProgress(-1, "*导入文件" + dataFile + "成功！");
        }
    }
}
