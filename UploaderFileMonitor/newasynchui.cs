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
    /// newasynchui ��ժҪ˵����
    /// </summary>
    public class newasynchui : Task
    {
        public List<Region> RegionList;
        public newasynchui()
        {
            //
            // TODO: �ڴ˴���ӹ��캯���߼�
            //
        }
        /// <summary>
        /// ���ڴ����쳣
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
            //����ϴ����Ƿ����
            if (!TableExist(sqlserver, TableName))
                return;

            //�ܱ�
            try
            {
                Sql1 = Sql1.Replace("@TableName", TableName);

                sqlserver.BeginTran();
                sqlserver.ExecuteNonQuery("delete from ClueData_CountBy where RegionGuid = '" + task.RegionGuid.Trim() + "'");
                sqlserver.ExecuteNonQuery(Sql1.Replace("@RegionGuid", task.RegionGuid));

                sqlserver.Commit();
                this.FireStatusChangedEvent(TaskStatus.Running, "-���뱨��ɹ���");

            }
            catch (Exception ex)
            {
                sqlserver.RollBack();
                throw new Exception("-���뱨�����" + ex.Message);
            }

            //ÿ�ս��ȱ�




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
            //if (dt.Hour == 3) //�賿3��
            //    Work(args);



            this.FireStatusChangedEvent(TaskStatus.Running, "-�����ݿ⣬��ѯ�Ƿ������ϴ������������ļ�");
            WFMUploadTaskServices wts = new WFMUploadTaskServices();             

            IParametersSpecification spec = new CriteriaSpecification("TStatus", CriteriaOperator.Equal, 1);
            spec = spec.And(new CriteriaSpecification("a.Status", CriteriaOperator.Equal, 10));
            List<WFMUploadTask> wList =  wts.GetAllData(spec).ToList();  

            if(wList == null || wList.Count == 0)
            {
                this.FireStatusChangedEvent(TaskStatus.Running, "-û�����ϴ������������ļ�");
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
                    ut.ErrorMessage = "�������ݳɹ�";
                    ut.OpTime = System.DateTime.Now;

                    wts.UpdateData(ut);
                    //sqlserver.ExecuteNonQuery(@"Update [dbo].[WFM_UploadTask] set 
                    //                       Status = 20, ErrorMessage = '�������ݳɹ�' where Status = 15 and TaskGuid = '"
                    //                       + ut.TaskGuid.ToString() + "'");


                    mes = rcs.GenerateReport(ut.RegionCode, ut.RegionGuid);
                    if (mes.retType != 100)
                        throw new Exception(mes.Message);

                    this.FireStatusChangedEvent(TaskStatus.Running, "-����ļ���" + ut.RegionName + "�ļ����룡");
                }
                catch (Exception ex)
                {
                    this.FireStatusChangedEvent(TaskStatus.Running, "-�������ݿ����" + ex.Message);
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
            this.FireProgressChangedEvent(100, "-��ɵ��룡");
            return "";
        }

        private void ImportData(WFMRegionClueSeverices rcs, String dataFile, WFMUploadTask uTask)
        {
            using (DAL.MySqlite resultDB = new DAL.MySqlite(dataFile))
            {

                var o = resultDB.ExecuteScalar("Select count() from  Clue_report");
                int total = int.Parse(o.ToString());
                if (o != null)
                    this.FireStatusChangedEvent(TaskStatus.Running, "*�ܹ�����������" + total.ToString());
                else
                {
                    throw new Exception("�ϴ������ݿ�û������");
                }

                int iProgress = 0;
                this.FireProgressChangedEvent(0, "���� 0 / " + total.ToString() + "�����ݣ�");

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
                            if (c.Type.IndexOf("��һ��") != -1)
                                c.InputError = 1;


                        int RowID = 0;
                        int.TryParse(r[9].ToString(), out RowID);
                        c.RowID = RowID;

                        cList.Add(c);
                    }

                    var mes = rcs.InsertData(cList);
                    if(mes.retType != 100)                   
                        throw new Exception("�������ݿ�ʧ�ܣ�" + mes.Message);

                    if(total != 0)
                        this.FireProgressChangedEvent((int)(iProgress * 100 / total), "���� " + iProgress.ToString() + " / " + total.ToString() + "�����ݣ�");

                    cList.Clear();
                }

                this.FireProgressChangedEvent(100, "���ݵ������");
                resultDB.CloseConnection();
            }

            this.FireStatusChangedEvent(TaskStatus.Running, "*�����ļ�" + dataFile + "�ɹ���");
        }

        private string GetImportFileName(WFMUploadTask uTask)
        {            
            this.FireStatusChangedEvent(TaskStatus.Running, "-����" + uTask.UserRegionPath.Trim() + "�ϴ����ļ���" +
               uTask.FileName.Trim() + "����ʼ��ѹ");

            //depress data
            string datafile = DepressData(uTask.FilePath.Trim() + "\\" + uTask.FileName.Trim());
            if (datafile.Length == 0)
            {              
                throw new Exception("- δ�����ļ�" + uTask.FileName.Trim()); 
            }           

            this.FireProgressChangedEvent(50, "-���" + uTask.UserRegionPath.Trim() + "��ѹ");            
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
                    this.FireStatusChangedEvent(TaskStatus.Running, "*�ܹ�����������" + total.ToString());
                else
                {
                    throw new Exception("�ϴ������ݿ�û������");
                }

                int iProgress = 0;
                this.FireProgressChangedEvent(0, "���� 0 / " + total.ToString() + "�����ݣ�");

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
                        this.FireStatusChangedEvent(TaskStatus.Running, "�������ݳ���" + ex.Message);
                        this.StopTask();
                    }
                    this.FireProgressChangedEvent((int)(iProgress * 100 / total), "���� " + iProgress.ToString() + " / " + total.ToString() + "�����ݣ�");
                    ds.Tables[0].Clear();
                    ds = null;
                }
                this.FireProgressChangedEvent(100, "���ݵ������");
                resultDB.CloseConnection();                
            }


            this.FireStatusChangedEvent(TaskStatus.Running, "*�����ļ�" + datafile + "�ɹ���");
        }
        private string DepressData(string compressfile)
        {
            if(!File.Exists(compressfile)) throw new Exception(compressfile + "*�ļ�" + compressfile + "�����ڣ�");  

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
                // ��ѹ���˵�����ͨ��GZipStream��ѹ�����ٶ�����
                // �����������ݾʹ���������
                while ((bytesRead = decompressionStream.Read(buffer, 0, bufferSize)) > 0)
                {
                    // �ѽ�ѹ�������д�뵽���������
                    outputStream.Write(buffer, 0, bytesRead);
                }
                decompressionStream.Close();

                inputStream.Close();
                outputStream.Close();

            }
            catch (Exception ex)
            {
                throw new Exception(compressfile + "*��ѹ�ļ�" + outputFileName + "ʧ�ܣ�" + ex.Message); 
            }

            this.FireProgressChangedEvent(20, "-���" + compressfile + "���ļ�");
            this.FireStatusChangedEvent(TaskStatus.Running, "*��ѹ�ļ�" + outputFileName + "�ɹ���");
            return outputFileName;
        }

        public static string StripSQLInjection(string sql)
        {
            if (!string.IsNullOrEmpty(sql))
            {
                //���� ' --    
                string pattern1 = @"(\%27)|(\-\-)";

                //��ִֹ�� ' or    
                string pattern2 = @"((\%27)|(\'))\s*((\%6F)|o|(\%4F))((\%72)|r|(\%52))";

                //��ִֹ��sql server �ڲ��洢���̻���չ�洢����    
                string pattern3 = @"\s+exec(\s|\+)+(s|x)p\w+";

                sql = Regex.Replace(sql, pattern1, string.Empty, RegexOptions.IgnoreCase);
                sql = Regex.Replace(sql, pattern2, string.Empty, RegexOptions.IgnoreCase);
                sql = Regex.Replace(sql, pattern3, string.Empty, RegexOptions.IgnoreCase);
            }
            return sql;
        }


        /// <summary>
        /// ����cmd����
        /// ����ʾ�����
        /// </summary>
        /// <param name="cmdExe">ָ��Ӧ�ó��������·��</param>
        /// <param name="cmdStr">ִ�������в���</param>
        static bool RunCmd2(string cmdExe, string cmdStr)
        {
            bool result = false;
            try
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;    //�Ƿ�ʹ�ò���ϵͳshell����
                p.StartInfo.RedirectStandardInput = true;//�������Ե��ó����������Ϣ
                p.StartInfo.RedirectStandardOutput = true;//�ɵ��ó����ȡ�����Ϣ
                p.StartInfo.RedirectStandardError = true;//�ض����׼�������
                p.StartInfo.CreateNoWindow = true;//����ʾ���򴰿�
                
                p.Start();//��������

                //��cmd���ڷ���������Ϣ
                p.StandardInput.WriteLine(cmdExe + cmdStr + "&exit");
               // p.StandardInput.WriteLine("&exit");
                p.StandardInput.AutoFlush = true;
                //p.StandardInput.WriteLine("exit");
                //���׼����д��Ҫִ�е��������ʹ��&������������ķ��ţ���ʾǰ��һ��������Ƿ�ִ�гɹ���ִ�к���(exit)��������ִ��exit����������ReadToEnd()���������
                //ͬ��ķ��Ż���&&��||ǰ�߱�ʾ����ǰһ������ִ�гɹ��Ż�ִ�к����������߱�ʾ����ǰһ������ִ��ʧ�ܲŻ�ִ�к��������



                //��ȡcmd���ڵ������Ϣ
                string output = p.StandardOutput.ReadToEnd();

                //StreamReader reader = p.StandardOutput;
                //string line=reader.ReadLine();
                //while (!reader.EndOfStream)
                //{
                //    str += line + "  ";
                //    line = reader.ReadLine();
                //}

                p.WaitForExit();//�ȴ�����ִ�����˳�����
                p.Close();


            }
            catch (Exception ex)
            {

            }
            return result;
        }

        #region sqlite odbc
        //string connString = @"Driver=SQLite3 ODBC Driver;Database=C:\Users\admin\Desktop\������\Ǳ��\���\�������ݿ�\34253145446.741.db";
        //System.Data.Odbc.OdbcConnection conn = new System.Data.Odbc.OdbcConnection(connString);

        //conn.Open();
        //System.Data.Odbc.OdbcDataAdapter sad = new System.Data.Odbc.OdbcDataAdapter("select * from tbCompareCivilInfo", conn);//������ѯ��
        //System.Data.DataSet ds = new System.Data.DataSet();//���������
        //sad.Fill(ds);//�����������
        //conn.Close();//�ر�����

        //if(ds != null && ds.Tables[0] != null)
        //    this.FireStatusChangedEvent(TaskStatus.Running, "-test sqlite odbc sucess" + ds.Tables[0].Rows.Count.ToString());

        #endregion
    }
}
