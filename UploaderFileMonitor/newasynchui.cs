using System;
using System.Threading;
using WFM.JW.HB.Services;
using WFM.JW.HB.Models;
using WFM.JW.HB.Repository.EntLib;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Data;
using System.Linq;
using System.Data.OleDb;
using System.Diagnostics;
using UploaderFileMonitor.DAL;
using System.Text.RegularExpressions;

namespace UploaderFileMonitor
{
    /// <summary>
    /// newasynchui 的摘要说明。
    /// </summary>
    public class newasynchui : Task
    {
        public List<Region> RegionList;
        public newasynchui()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        /// <summary>
        /// 用于触发异常
        /// </summary>
        public int errorkey = 1;
        override public object Work(params object[] args)
        {
            

            DAL.MyDataBase sqlserver = new DAL.MyDataBase();
            sqlserver.DBConnectionString = Properties.Settings.Default.ConnectionStr; 

            //read db for report sql
            //1.get all regiion
            WFMRegionServices services = new WFMRegionServices();
            RegionList = services.GetAllData().ToList();

            WFMRegionUserServices rus = new WFMRegionUserServices();
            var uList = rus.GetAllData().ToList();

            WFMUploadTaskServices uts = new WFMUploadTaskServices();
            var tlist = uts.GetAllData(new CriteriaSpecification("IsRead", CriteriaOperator.Equal, "0")).ToList();

            foreach (var t in tlist)
            {
                var dataFile = GetImportFileName(t);
                using (DAL.MySqlite resultDB = new DAL.MySqlite(dataFile))
                {
                    var ds = resultDB.ExecuteDataset("Select count() totalnum, sum(inputerror) inputerrors, sum(case when inputerror = 1 then 0 else 1 end) clues from clue_report");


                    var list = from dr in ds.Tables[0].AsEnumerable()
                               select new
                               {
                                   totalnum = (Int32)dr.Field<Int32>("totalnum"),
                                   inputerrors = (Int32)dr.Field<Int32>("inputerrors"),
                                   clues = (Int32)dr.Field<Int32>("clues")
                               };


                    

                }
            }
            sqlserver.CloseConnection();
            return 100;
        }

        private void GenerateReport(WFMUploadTask task)
        {
            #region Sql
            String Sql1 = @"Insert into [dbo].[ClueData_CountBy](RegionGuid, Contry, DataItem,  ClueType, Totalclues, Confirmed) 
                            SELECT '@RegionGuid' AS Guid,
                                        PersonRegion as Contry,
                                        Table1 AS ItemType,
                                        Table2 AS ClueType,
                                        COUNT(DataGuid) AS totalclues,
                                        SUM(CASE WHEN confirmed = 1 THEN 1 ELSE 0 END) AS Confirmed
                                    FROM @TableName
                                    GROUP BY PersonRegion,
                                            Table1,
                                            Table2";


            #endregion
            MyDataBase sqlserver = new MyDataBase();
            sqlserver.DBConnectionString = Properties.Settings.Default.ConnectionStr;

            String TableName = "ClueData_" + task.RegionCode;
            //检查上传表是否存在
            if (!TableExist(sqlserver, TableName))
                return;

            //总表
            try
            {
                Sql1 = Sql1.Replace("@TableName", TableName);

                sqlserver.BeginTran();
                sqlserver.ExecuteNonQuery("delete from ClueData_CountBy where RegionGuid = '" + task.RegionGuid.Trim() + "'");
                sqlserver.ExecuteNonQuery(Sql1.Replace("@RegionGuid", task.RegionGuid));

                sqlserver.Commit();
                this.FireStatusChangedEvent(TaskStatus.Running, "-导入报表成功！");

            }
            catch (Exception ex)
            {
                sqlserver.RollBack();
                throw new Exception("-导入报表出错！" + ex.Message);
            }

            //每日进度表




        }
        private bool TableExist(MyDataBase sqlserver, string tableName)
        {
            String Sql = @"select * from sys.tables t join sys.schemas s on (t.schema_id = s.schema_id) 
              where s.name = 'dbo' and t.name = '" + tableName + "'";

            bool bRet = false;
            try
            {
                var o = sqlserver.ExecuteScalar(Sql);
                //drop table
                bRet = (o != null);
            }
            catch
            {

            }
            return bRet;

        }

        public object Work2(params object[] args)
        {
            base.Work(args);

            //DateTime dt = System.DateTime.Now;
            //if (dt.Hour == 3) //凌晨3点
            //    Work(args);



            this.FireStatusChangedEvent(TaskStatus.Running, "-读数据库，查询是否有新上传的问题线索文件");
            WFMUploadTaskServices wts = new WFMUploadTaskServices();             

            IParametersSpecification spec = new CriteriaSpecification("TStatus", CriteriaOperator.Equal, 1);
            spec = spec.And(new CriteriaSpecification("a.Status", CriteriaOperator.Equal, 10));
            List<WFMUploadTask> wList =  wts.GetAllData(spec).ToList();  

            if(wList == null || wList.Count == 0)
            {
                this.FireStatusChangedEvent(TaskStatus.Running, "-没有新上传的问题线索文件");
                this.StopTask();
            }

            //DAL.MyDataBase sqlserver = new DAL.MyDataBase();
            //sqlserver.DBConnectionString = Properties.Settings.Default.ConnectionStr;           


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

                    this.FireStatusChangedEvent(TaskStatus.Running, "-完成文件：" + ut.RegionName + "文件导入！");
                }
                catch (Exception ex)
                {
                    this.FireStatusChangedEvent(TaskStatus.Running, "-导入数据库出错！" + ex.Message);
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
            this.FireProgressChangedEvent(100, "-完成导入！");
            return "";
        }

        private void ImportData(WFMRegionClueSeverices rcs, String dataFile, WFMUploadTask uTask)
        {
            using (DAL.MySqlite resultDB = new DAL.MySqlite(dataFile))
            {

                var o = resultDB.ExecuteScalar("Select count() from  Clue_report");
                int total = int.Parse(o.ToString());
                if (o != null)
                    this.FireStatusChangedEvent(TaskStatus.Running, "*总共问题线索：" + total.ToString());
                else
                {
                    throw new Exception("上传的数据库没有数据");
                }

                int iProgress = 0;
                this.FireProgressChangedEvent(0, "导入 0 / " + total.ToString() + "条数据！");

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
                    if(mes.retType != 100)                   
                        throw new Exception("插入数据库失败：" + mes.Message);

                    if(total != 0)
                        this.FireProgressChangedEvent((int)(iProgress * 100 / total), "导入 " + iProgress.ToString() + " / " + total.ToString() + "条数据！");

                    cList.Clear();
                }

                this.FireProgressChangedEvent(100, "数据导入完成");
                resultDB.CloseConnection();
            }

            this.FireStatusChangedEvent(TaskStatus.Running, "*导入文件" + dataFile + "成功！");
        }

        private string GetImportFileName(WFMUploadTask uTask)
        {            
            this.FireStatusChangedEvent(TaskStatus.Running, "-发现" + uTask.UserRegionPath.Trim() + "上传的文件，" +
               uTask.FileName.Trim() + "！开始解压");

            //depress data
            string datafile = DepressData(uTask.FilePath.Trim() + "\\" + uTask.FileName.Trim());
            if (datafile.Length == 0)
            {              
                throw new Exception("- 未发现文件" + uTask.FileName.Trim()); 
            }           

            this.FireProgressChangedEvent(50, "-完成" + uTask.UserRegionPath.Trim() + "解压");            
            return datafile;
        }
        //private bool PreImportData(DAL.MyDataBase sqlserver, WFMUploadTask uTask)
        //{
        //    #region sql
        //    String Sql = @"select * from sys.tables t join sys.schemas s on (t.schema_id = s.schema_id) 
        //      where s.name = 'dbo' and t.name = 'ClueData_@tablename'";
        //    String Sql1 = @"DELETE FROM [dbo].[ClueData_@tablename]";
        //    String Sql2 = @"             
        //                    CREATE TABLE [dbo].[ClueData_@tablename] (    
        //                                        [DataGuid]     UNIQUEIDENTIFIER  ROWGUIDCOL NOT NULL, 
        //                                        [PersonID]     NVARCHAR (20)       NOT NULL,
        //                                        [PersonName]   NVARCHAR(50)       NULL,
        //                                        [PersonRegion] NVARCHAR(50)       NULL,
        //                                        [PersonAddr]   NVARCHAR(200)       NULL,
        //                                        [ClueType]     NVARCHAR (500)       NOT NULL,                                                 
        //                                        [DateRange]    NVARCHAR(300)       NULL,
        //                                        [Comment]       NVARCHAR(300)       NULL,
        //                                        [Table1]        int       NOT NULL,                                                 
        //                                        [InputError]    int       NULL DEFAULT 0, 
        //                                        [Confirmed]     INT  NULL DEFAULT 0, 
        //                                        [RowID]    int null,
        //                                        [IsClueTrue]     int null,
        //                                        [IsCompliance]    int null,
        //                                        [IsCP]           int null,
        //                                        [Fact]           NVARCHAR (450),
        //                                        [IllegalMoney]   real null ,
        //                                        [CheckDate]      DATETIME,
        //                                        [CheckByName1]   NVARCHAR (10),
        //                                        [CheckByName2]   NVARCHAR (10),
        //                                        [ReCheckFact]    NVARCHAR(100),
        //                                        [ReCheckType]    int null,
        //                                        [ReCheckByName1] NVARCHAR (10)
        //                    CONSTRAINT [PK_ClueData_@tablename] PRIMARY KEY CLUSTERED 
        //                    (
	       //                     [DataGuid] ASC
        //                    )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
        //                    ) ON [PRIMARY] ";

        //    string Sql3 = @"ALTER TABLE [dbo].[ClueData_@tablename]  ADD  CONSTRAINT [DF_ClueData_@tablename_DataGuid]  DEFAULT (newid()) FOR [DataGuid]";
        //    string Sql4 = @"delete from [dbo].[ClueData_Report] where RegionName = @Name";
        //    string Sql4 = @"CREATE NONCLUSTERED INDEX [NonClusteredIndex-@tablename-RowID] ON [dbo].[ClueData_@tablename]
        //                    (
        //                        [RowID] ASC
        //                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]";
        //    string Sql5 = @"CREATE NONCLUSTERED INDEX [NonClusteredIndex-@tablename-PersonID] ON [dbo].[ClueData_@tablename]
        //                    (
        //                        [PersonID] ASC
        //                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]";
        //    #endregion
        //    bool bRet = false;

        //    var o = sqlserver.ExecuteScalar(Sql.Replace("@tablename", uTask.RegionCode.Trim()));
        //    drop table
        //    if (o == null)
        //    {
        //        create table
        //        sqlserver.ExecuteNonQuery(Sql2.Replace("@tablename", uTask.RegionCode.Trim()));
        //        sqlserver.ExecuteNonQuery(Sql3.Replace("@tablename", uTask.RegionCode.Trim()));
        //        sqlserver.ExecuteNonQuery(Sql4.Replace("@tablename", uTask.RegionCode.Trim()));
        //        sqlserver.ExecuteNonQuery(Sql5.Replace("@tablename", uTask.RegionCode.Trim()));
        //    }
        //    else
        //    {//delete date
        //        sqlserver.ExecuteNonQuery(Sql1.Replace("@tablename", uTask.RegionCode.Trim()));
        //        sqlserver.ExecuteNonQuery(Sql4.Replace("@Name", uTask.RegionName));
        //    }
        //    bRet = true;
        //    return bRet;
        //}
        private void ImportData(DAL.MyDataBase sqlserver, string datafile, WFMUploadTask uTask)
        {
            string insertSql = @"
                               INSERT INTO [dbo].[ClueData_@table] (    [RowID],                         
                                    [PersonID], 
                                    [PersonName], 
                                    [PersonRegion], 
                                    [PersonAddr], 
                                    [ClueType], 
                                    [ClueAmount], 
                                    [DateRange], 
                                    [Table1], 
                                    [Table2]) 
                                  VALUES (  @RowID, '@PersonID', 
                                    '@PersonName', 
                                    '@PersonRegion', 
                                    '@PersonAddr', 
                                    '@ClueType', 
                                    '@ClueAmount', 
                                    '@DateRange', 
                                    @Table1, 
                                    @Table2)";

            insertSql = insertSql.Replace("@table", uTask.RegionCode);


            using (DAL.MySqlite resultDB = new DAL.MySqlite(datafile))
            {

                var o = resultDB.ExecuteScalar("Select count() from  report");
                int total = int.Parse(o.ToString());
                if (o != null)
                    this.FireStatusChangedEvent(TaskStatus.Running, "*总共问题线索：" + total.ToString());
                else
                {
                    throw new Exception("上传的数据库没有数据");
                }

                int iProgress = 0;
                this.FireProgressChangedEvent(0, "导入 0 / " + total.ToString() + "条数据！");

                for (int i = 0; i <= total / 10000; i++)
                {
                    var ds = resultDB.ExecuteDataset(@"SELECT 
                                                       subStr(ID, 0, 20) AS ID,
                                                       subStr(Name, 0, 50) AS Name,
                                                       subStr(Addr, 0, 200) AS Addr,
                                                       subStr(Region, 0, 50) AS Region,
                                                       subStr(Type, 0, 20) ASType,
                                                       subStr(Amount, 0, 10) AS Amount,
                                                       subStr(DateRange, 0, 100) AS DataRange,
                                                       Table1,
                                                       Table2,
                                                       RowID
                                                  FROM report limit " + (i * 10000).ToString() + ",10000");

                    if (ds == null)
                        throw new Exception("DB Error!");

                    iProgress += ds.Tables[0].Rows.Count;
                    try
                    {
                        sqlserver.BeginTran();
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {

                            string ID = r[0].ToString();
                            string Name = r[1].ToString();
                            string Addr = r[2].ToString();
                            string Region = r[3].ToString();
                            string Type = r[4].ToString();
                            string Amount = r[5].ToString();
                            string DateRange = r[6].ToString();


                            int Table1 = 0;
                            int.TryParse(r[7].ToString(), out Table1);
                            int Table2 = 0;
                            int.TryParse(r[8].ToString(), out Table2);

                            int RowID = 0;
                            int.TryParse(r[9].ToString(), out RowID);

                            //insertSql = insertSql.Replace("@DataGuid", System.Guid.NewGuid().ToString());
                            String Sql = insertSql.Replace("@PersonID", ID);
                            Sql = Sql.Replace("@PersonName", Name);
                            Sql = Sql.Replace("@PersonRegion", Region);
                            Sql = Sql.Replace("@PersonAddr", Addr);
                            Sql = Sql.Replace("@ClueType", Type);
                            Sql = Sql.Replace("@ClueAmount", Amount);
                            Sql = Sql.Replace("@DateRange", DateRange);
                            Sql = Sql.Replace("@Table1", Table1.ToString());
                            Sql = Sql.Replace("@Table2", Table2.ToString());
                            Sql = Sql.Replace("@RowID", RowID.ToString());
                            
                            //Sql = StripSQLInjection(Sql); 
                            sqlserver.ExecuteNonQuery(Sql);
                        }

                        sqlserver.Commit();
                    }
                    catch(Exception ex)
                    {
                        sqlserver.RollBack();
                        this.FireStatusChangedEvent(TaskStatus.Running, "插入数据出错：" + ex.Message);
                        this.StopTask();
                    }
                    this.FireProgressChangedEvent((int)(iProgress * 100 / total), "导入 " + iProgress.ToString() + " / " + total.ToString() + "条数据！");
                    ds.Tables[0].Clear();
                    ds = null;
                }
                this.FireProgressChangedEvent(100, "数据导入完成");
                resultDB.CloseConnection();                
            }


            this.FireStatusChangedEvent(TaskStatus.Running, "*导入文件" + datafile + "成功！");
        }
        private string DepressData(string compressfile)
        {
            if(!File.Exists(compressfile)) throw new Exception(compressfile + "*文件" + compressfile + "不存在！");  

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

            this.FireProgressChangedEvent(20, "-完成" + compressfile + "的文件");
            this.FireStatusChangedEvent(TaskStatus.Running, "*解压文件" + outputFileName + "成功！");
            return outputFileName;
        }

        public static string StripSQLInjection(string sql)
        {
            if (!string.IsNullOrEmpty(sql))
            {
                //过滤 ' --    
                string pattern1 = @"(\%27)|(\-\-)";

                //防止执行 ' or    
                string pattern2 = @"((\%27)|(\'))\s*((\%6F)|o|(\%4F))((\%72)|r|(\%52))";

                //防止执行sql server 内部存储过程或扩展存储过程    
                string pattern3 = @"\s+exec(\s|\+)+(s|x)p\w+";

                sql = Regex.Replace(sql, pattern1, string.Empty, RegexOptions.IgnoreCase);
                sql = Regex.Replace(sql, pattern2, string.Empty, RegexOptions.IgnoreCase);
                sql = Regex.Replace(sql, pattern3, string.Empty, RegexOptions.IgnoreCase);
            }
            return sql;
        }


        /// <summary>
        /// 运行cmd命令
        /// 不显示命令窗口
        /// </summary>
        /// <param name="cmdExe">指定应用程序的完整路径</param>
        /// <param name="cmdStr">执行命令行参数</param>
        static bool RunCmd2(string cmdExe, string cmdStr)
        {
            bool result = false;
            try
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;//不显示程序窗口
                
                p.Start();//启动程序

                //向cmd窗口发送输入信息
                p.StandardInput.WriteLine(cmdExe + cmdStr + "&exit");
               // p.StandardInput.WriteLine("&exit");
                p.StandardInput.AutoFlush = true;
                //p.StandardInput.WriteLine("exit");
                //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
                //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令



                //获取cmd窗口的输出信息
                string output = p.StandardOutput.ReadToEnd();

                //StreamReader reader = p.StandardOutput;
                //string line=reader.ReadLine();
                //while (!reader.EndOfStream)
                //{
                //    str += line + "  ";
                //    line = reader.ReadLine();
                //}

                p.WaitForExit();//等待程序执行完退出进程
                p.Close();


            }
            catch (Exception ex)
            {

            }
            return result;
        }

        #region sqlite odbc
        //string connString = @"Driver=SQLite3 ODBC Driver;Database=C:\Users\admin\Desktop\大数据\潜江\输出\导入数据库\34253145446.741.db";
        //System.Data.Odbc.OdbcConnection conn = new System.Data.Odbc.OdbcConnection(connString);

        //conn.Open();
        //System.Data.Odbc.OdbcDataAdapter sad = new System.Data.Odbc.OdbcDataAdapter("select * from tbCompareCivilInfo", conn);//创建查询器
        //System.Data.DataSet ds = new System.Data.DataSet();//创建结果集
        //sad.Fill(ds);//将结果集填入
        //conn.Close();//关闭连接

        //if(ds != null && ds.Tables[0] != null)
        //    this.FireStatusChangedEvent(TaskStatus.Running, "-test sqlite odbc sucess" + ds.Tables[0].Rows.Count.ToString());

        #endregion
    }
}
