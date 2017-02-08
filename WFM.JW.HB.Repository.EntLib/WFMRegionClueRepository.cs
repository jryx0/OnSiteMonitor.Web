using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using WFM.JW.HB.Models;
using System.Data.Common;
using System.Data;

namespace WFM.JW.HB.Repository.EntLib
{
    public class WFMRegionClueRepository : BaseRepositorySql06<Models.Clues>
    {
        #region Query
        public override IRowMapper<Clues> Mapor()
        {
            return MapBuilder<Clues>.MapAllProperties()
                                    .Map(p => p.ClueGuid).ToColumn("DataGuid")
                                    .Map(p => p.RowID).ToColumn("RowID")                                    
                                    .Map(p => p.ID).ToColumn("PersonID")
                                    .Map(p => p.Name).ToColumn("PersonName")
                                    .Map(p => p.Addr).ToColumn("PersonAddr")
                                    .Map(p => p.Region).ToColumn("PersonRegion")
                                    .Map(p => p.Type).ToColumn("ClueType")
                              //      .Map(p => p.Amount).ToColumn("ClueAmount")
                                    .Map(p => p.DateRange).ToColumn("DateRange")
                                    .Map(p => p.Table1).ToColumn("Table1")
                             //       .Map(p => p.Table2).ToColumn("Table1")
                                    .Map(p => p.IsConfirmed).ToColumn("Confirmed")
                                    .Map(p => p.IsClueTrue).ToColumn("IsClueTrue")
                                    .Map(p => p.IsCompliance).ToColumn("IsCompliance")
                                    .Map(p => p.IsCP).ToColumn("IsCP")
                                    .Map(p => p.Fact).ToColumn("Fact")
                                    .Map(p => p.IllegalMoney).ToColumn("IllegalMoney")
                                    .Map(p => p.CheckDate).ToColumn("CheckDate")
                                    .Map(p => p.CheckByName1).ToColumn("CheckByName1")
                                    .Map(p => p.CheckByName2).ToColumn("CheckByName2")
                                    .Map(p => p.ReCheckFact).ToColumn("ReCheckFact")
                                    .Map(p => p.ReCheckType).ToColumn("ReCheckType")
                                    .Map(p => p.ReCheckByName1).ToColumn("ReCheckByName1")
                                    .Map(p => p.InputError).ToColumn("InputError")
                                    .Map(p => p.Comment).ToColumn("Comment")
                                .Build();
        }

      

        public IEnumerable<Clues> GetData(string RegionCode, IParametersSpecification paraSpec)
        {
            #region Sql
            String SelectSql = @"SELECT cast([DataGuid] as varchar(36)) as DataGuid,[PersonID] ,[PersonName] ,[PersonRegion] ,[PersonAddr] ,[ClueType] ,[DateRange] ,[Table1]
                          ,[Confirmed] ,[RowID],[IsClueTrue] ,[IsCompliance],[IsCP] ,[Fact] ,[IllegalMoney],[CheckDate] ,[CheckByName1] ,[CheckByName2]
                          ,[ReCheckFact] ,[ReCheckType] ,[ReCheckByName1], [InputError],[Comment]
                      FROM [dbo].[ClueData_@RegionCode]";
            #endregion

            if (paraSpec == null)
                paraSpec = CriteriaSpecification.GetTrue();

            SelectSql = SelectSql.Replace("@RegionCode", RegionCode);
            return base.FindData(SelectSql, paraSpec, "");
        }
        #endregion

        public Messaging<int> UpdateReport(string regionCode)
        {
            String CountbySql = @"update ClueData_CountBy set confirmed = 
                                    (select sum( isnull(iscluetrue, 0)) from ClueData_@regionCode 
                                    where ClueData_@regionCode.Table1 = ClueData_CountBy.DataItem
                                    and ClueData_@regionCode.PersonRegion = ClueData_CountBy.Contry)
                                    where ClueData_CountBy.RegionGuid =
                                    (select distinct regionguid from HB_NewRegion where RegionCode = '@regionCode')";

            
            CountbySql = CountbySql.Replace("@regionCode", regionCode);
            Models.Messaging<int> mes = new Models.Messaging<int>();

            try
            {
                mes.Value = _database.ExecuteNonQuery(CountbySql);
                mes.retType = 100;
                mes.Message = "更新成功！";
            }
            catch(Exception ex)
            {
                mes.retType = -1;
                mes.Value = 0;
                mes.Message = ex.Message;
            }

            return mes;
        }

        public Messaging<int> UpdateData(string regionCode, List<Clues> data)
        {
            String UpdateSql = @"UPDATE [dbo].[ClueData_@regionCode]
                                   SET  
                                       [PersonRegion] = @PersonRegion 
                                      ,[PersonAddr] = @PersonAddr      
                                      ,[Confirmed] = 1
                                      ,[IsClueTrue] = @IsClueTrue 
                                       
                                      ,[IsCP] = @IsCP 
                                      ,[Fact] = @Fact 
                                      ,[IllegalMoney] = @IllegalMoney 
                                      ,[CheckDate] = @CheckDate 
                                      ,[CheckByName1] = @CheckByName1 
                                      ,[CheckByName2] = @CheckByName2 
                                      ,[ReCheckFact] = @ReCheckFact 
                                      ,[ReCheckType] = @ReCheckType 
                                      ,[ReCheckByName1] = @ReCheckByName1 
                                      ,[InputError] = @InputError
                                      ,[Comment] = @Comment
                                 WHERE  [PersonID] = @PersonID and [RowID] = @RowID";

            String CountbySql = @"update ClueData_CountBy set confirmed = 
                                    (select sum( isnull(confirmed, 0)) from ClueData_@regionCode 
                                    where ClueData_@regionCode.Table1 = ClueData_CountBy.DataItem
                                    and ClueData_@regionCode.PersonRegion = ClueData_CountBy.Contry),
                                    IsTrue = 
                                    (select sum( isnull(iscluetrue, 0)) from ClueData_@regionCode 
                                    where ClueData_@regionCode.Table1 = ClueData_CountBy.DataItem
                                    and ClueData_@regionCode.PersonRegion = ClueData_CountBy.Contry)
                                    where ClueData_CountBy.RegionGuid =
                                    (select distinct regionguid from HB_NewRegion where RegionCode = '@regionCode')";


            UpdateSql = UpdateSql.Replace("@regionCode", regionCode);
            CountbySql = CountbySql.Replace("@regionCode", regionCode);
            Models.Messaging<int> mes = new Models.Messaging<int>();

            Clues cl;
            using (DbConnection conn = _database.CreateConnection())
            {
                conn.Open();
                DbTransaction trans = conn.BeginTransaction();
                try
                {
                    foreach (var c in data)
                    {
                        cl = c;
                        c.IsConfirmed = 10;
                        var UpdateCommand = _database.GetSqlStringCommand(UpdateSql);
                        _database.AddInParameter(UpdateCommand, "@RowID", System.Data.DbType.Int32, c.RowID);
                        _database.AddInParameter(UpdateCommand, "@PersonID", System.Data.DbType.String, c.ID);
                        _database.AddInParameter(UpdateCommand, "@PersonRegion", System.Data.DbType.String, c.Region);
                        _database.AddInParameter(UpdateCommand, "@PersonAddr", System.Data.DbType.String, c.Addr);
                        _database.AddInParameter(UpdateCommand, "@IsClueTrue", System.Data.DbType.Int32, c.IsClueTrue);
                        _database.AddInParameter(UpdateCommand, "@IsCP", System.Data.DbType.Int32, c.IsCP);


                        if (c.Fact.Length > 140)
                            c.Fact = c.Fact.Substring(0, 140);

                        _database.AddInParameter(UpdateCommand, "@Fact", System.Data.DbType.String, c.Fact);
                        _database.AddInParameter(UpdateCommand, "@IllegalMoney", System.Data.DbType.Double, c.IllegalMoney);
                        if (c.CheckDate.Year >= 2016)
                            _database.AddInParameter(UpdateCommand, "@CheckDate", System.Data.DbType.DateTime, c.CheckDate);
                        else _database.AddInParameter(UpdateCommand, "@CheckDate", System.Data.DbType.DateTime, DBNull.Value);

                        _database.AddInParameter(UpdateCommand, "@CheckByName1", System.Data.DbType.String, c.CheckByName1);
                        _database.AddInParameter(UpdateCommand, "@CheckByName2", System.Data.DbType.String, c.CheckByName2);

                        _database.AddInParameter(UpdateCommand, "@ReCheckFact", System.Data.DbType.String, c.ReCheckFact);
                        _database.AddInParameter(UpdateCommand, "@ReCheckType", System.Data.DbType.Int32, c.ReCheckType);
                        _database.AddInParameter(UpdateCommand, "@ReCheckByName1", System.Data.DbType.String, c.ReCheckByName1);
                        _database.AddInParameter(UpdateCommand, "@InputError", System.Data.DbType.Int32, c.InputError);


                        if (c.Comment !=null && c.Comment.Length > 140)
                            c.Comment = c.Comment.Substring(0, 140);
                        _database.AddInParameter(UpdateCommand, "@Comment", System.Data.DbType.String, c.Comment);

                        _database.ExecuteNonQuery(UpdateCommand, trans);

                        c.Clear();
                        c.IsConfirmed = 1;
                    }

                   
                    trans.Commit();

                    //update report
                    // _database.ExecuteNonQuery(CommandType.Text, CountbySql);
                  


                    mes.retType = 100;                   
                    mes.Value = data.Count;
                    mes.Message = String.Format("插入数据成功, {0} 行受影响", mes.Value);
                }
                catch (Exception ex)
                {
                    trans.Rollback();

                    mes.retType = 1;
                    mes.Message = ex.Message;

                    return mes;
                }
            }


            var o = _database.ExecuteScalar(CommandType.Text, @"select distinct regionguid from HB_NewRegion where RegionCode = '" + regionCode + "'");
            mes = GenerateReport(regionCode, o.ToString());

            return mes;
        }

        public Messaging<int> InsertData(string regionCode, List<Clues> data)
        {
            String InsertSql = @"INSERT INTO [dbo].[ClueData_@regionCode] (    [RowID],                         
                                    [PersonID], 
                                    [PersonName], 
                                    [PersonRegion], 
                                    [PersonAddr], 
                                    [ClueType], 
                                    [Comment], 
                                    [DateRange], 
                                    [Table1], 
                                    [InputError]) 
                                  VALUES (@RowID, @PersonID, 
                                    @PersonName, 
                                    @PersonRegion, 
                                    @PersonAddr, 
                                    @ClueType, 
                                    @Comment, 
                                    @DateRange, 
                                    @Table1, 
                                    @InputError)";

            InsertSql = InsertSql.Replace("@regionCode", regionCode);
            Models.Messaging<int> mes = new Models.Messaging<int>();
            
            using (DbConnection conn = _database.CreateConnection())
            {
                conn.Open();
                DbTransaction trans = conn.BeginTransaction();
                try
                {
                    foreach (var c in data)
                    {
                        var InsertCommand = _database.GetSqlStringCommand(InsertSql);
                        _database.AddInParameter(InsertCommand, "@RowID", System.Data.DbType.Int32, c.RowID);
                        _database.AddInParameter(InsertCommand, "@PersonID", System.Data.DbType.String, c.ID);
                        _database.AddInParameter(InsertCommand, "@PersonName", System.Data.DbType.String, c.Name);
                        _database.AddInParameter(InsertCommand, "@PersonRegion", System.Data.DbType.String, c.Region);
                        _database.AddInParameter(InsertCommand, "@PersonAddr", System.Data.DbType.String, c.Addr);
                        _database.AddInParameter(InsertCommand, "@ClueType", System.Data.DbType.String, c.Type);


                        _database.AddInParameter(InsertCommand, "@Comment", System.Data.DbType.String, c.Comment);
                        _database.AddInParameter(InsertCommand, "@DateRange", System.Data.DbType.String, c.DateRange);
                        _database.AddInParameter(InsertCommand, "@Table1", System.Data.DbType.Int32, c.Table1);
                        _database.AddInParameter(InsertCommand, "@InputError", System.Data.DbType.Int32, c.InputError);

                        _database.ExecuteNonQuery(InsertCommand, trans);
                    }

                    trans.Commit();
                    mes.retType = 100;
                    mes.Message = String.Format("插入数据成功, {0} 行受影响", mes.Value);
                    mes.Value = data.Count;                   
                }
                catch (Exception ex)
                {
                    trans.Rollback();

                    mes.retType = 1;
                    mes.Message = "InsertDB 错误：" + ex.Message;
                }
            }

            return mes;
        }


        String SqlFindTable = @"select * from sys.tables t join sys.schemas s on (t.schema_id = s.schema_id) 
              where s.name = 'dbo' and t.name = 'ClueData_@RegionCode'";

        public Messaging<int> InitTable(String RegionCode)
        {
            #region InitSql
            //String SqlFindTable = @"select * from sys.tables t join sys.schemas s on (t.schema_id = s.schema_id) 
            //  where s.name = 'dbo' and t.name = 'ClueData_@RegionCode'";
            String SqlClearTable = @"DELETE FROM [dbo].[ClueData_@RegionCode]";
            String SqlCreateTable = @"             
                            CREATE TABLE [dbo].[ClueData_@RegionCode] (    
                                                [DataGuid]     UNIQUEIDENTIFIER  ROWGUIDCOL NOT NULL, 
                                                [PersonID]     NVARCHAR (20)       NOT NULL,
                                                [PersonName]   NVARCHAR(50)       NULL,
                                                [PersonRegion] NVARCHAR(50)       NULL,
                                                [PersonAddr]   NVARCHAR(200)       NULL,
                                                [ClueType]     NVARCHAR (500)       NOT NULL,
                                                [Comment]      NVARCHAR(300)       NULL,
                                                [DateRange]    NVARCHAR(300)       NULL,
                                                [Table1]        int       NOT NULL,
                                                [InputError]       int       NULL DEFAULT 0, 
                                                [Confirmed]     INT  NULL DEFAULT 0, 
                                                [RowID]    int null,
                                                [IsClueTrue]     int null,
                                                [IsCompliance]    int null,
                                                [IsCP]           int null,
                                                [Fact]           NVARCHAR (450),
                                                [IllegalMoney]   real null ,
                                                [CheckDate]      DATETIME,
                                                [CheckByName1]   NVARCHAR (10),
                                                [CheckByName2]   NVARCHAR (10),
                                                [ReCheckFact]    NVARCHAR(100),
                                                [ReCheckType]    int null,
                                                [ReCheckByName1] NVARCHAR (10)
                            CONSTRAINT [PK_ClueData_@RegionCode] PRIMARY KEY CLUSTERED 
                            (
	                            [DataGuid] ASC
                            )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                            ) ON [PRIMARY] ";

            string SqlCreatKey = @"ALTER TABLE [dbo].[ClueData_@RegionCode]  ADD  CONSTRAINT [DF_ClueData_@RegionCode_DataGuid]  DEFAULT (newid()) FOR [DataGuid]";

            string SqlCreateIndex1 = @"CREATE NONCLUSTERED INDEX [NonClusteredIndex-@RegionCode-RowID] ON [dbo].[ClueData_@RegionCode]
                            (
                                [RowID] ASC
                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]";
            string SqlCreateIndex2 = @"CREATE NONCLUSTERED INDEX [NonClusteredIndex-@RegionCode-PersonID] ON [dbo].[ClueData_@RegionCode]
                            (
                                [PersonID] ASC
                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]";

            #endregion


            Messaging<int> mes = new Messaging<int>();
            mes.retType = 1;
            mes.Message = "初始化表失败！";

            var o = _database.ExecuteScalar(CommandType.Text, SqlFindTable.Replace("@RegionCode", RegionCode.Trim()));
            //drop table
            if (o == null)
            {
                //create table
                using (DbConnection conn = _database.CreateConnection())
                {
                    conn.Open();
                    DbTransaction trans = conn.BeginTransaction();
                    try
                    {
                        _database.ExecuteNonQuery(CommandType.Text, SqlCreateTable.Replace("@RegionCode", RegionCode.Trim()));
                        _database.ExecuteNonQuery(CommandType.Text, SqlCreatKey.Replace("@RegionCode", RegionCode.Trim()));
                        _database.ExecuteNonQuery(CommandType.Text, SqlCreateIndex1.Replace("@RegionCode", RegionCode.Trim()));
                        _database.ExecuteNonQuery(CommandType.Text, SqlCreateIndex2.Replace("@RegionCode", RegionCode.Trim()));
                        trans.Commit();

                        mes.retType = 100;
                        mes.Message = "初始化表成功！";
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                    }
                }
            }
            else
            { 
                _database.ExecuteNonQuery(CommandType.Text, SqlClearTable.Replace("@RegionCode", RegionCode.Trim()));

                mes.retType = 100;
                mes.Message = "初始化表成功！";
            }
           
            return mes;
        }

        public Messaging<int> GenerateReport(String RegionCode, String RegionGuid)
        {
            #region Sql
            String SqlReport1 = @"Insert into [dbo].[ClueData_CountBy](RegionGuid, Contry, DataItem,  Totalclues, InputErrors, Problems, Confirmed, IsTrue) 
                                        select '@RegionGuid' AS Guid, LEFT(PersonRegion, 50) AS Expr1, Table1, COUNT(DataGuid) AS totalclues, 
                                          SUM(CASE WHEN (ClueType LIKE '%不一致%') AND (ClueType NOT LIKE '%+%') THEN 1 ELSE 0 END) AS 录入错误, 
                                          SUM(CASE WHEN (ClueType NOT LIKE '%不一致%') OR
                                          (ClueType LIKE '%+%') THEN 1 ELSE 0 END) AS 问题数, SUM(CASE WHEN confirmed = 1 THEN 1 ELSE 0 END) AS Confirmed, 
                                          SUM(CASE WHEN IsClueTrue = 1 THEN 1 ELSE 0 END) AS IsTrue
                                         from [dbo].[ClueData_@RegionCode]
                                        group by PersonRegion, Table1";

            #endregion
            Models.Messaging<int> mes = new Models.Messaging<int>();
            mes.retType = 100;

            var o = _database.ExecuteScalar(CommandType.Text, SqlFindTable.Replace("@RegionCode", RegionCode));
            if (o == null)
                return mes;

            SqlReport1 = SqlReport1.Replace("@RegionCode", RegionCode);
           
            
            using (DbConnection conn = _database.CreateConnection())
            {
                conn.Open();
                DbTransaction trans = conn.BeginTransaction();
                try
                {
                    _database.ExecuteNonQuery(trans, CommandType.Text, "delete from ClueData_CountBy where RegionGuid = '" + RegionGuid + "'");
                    _database.ExecuteNonQuery(trans, CommandType.Text, SqlReport1.Replace("@RegionGuid", RegionGuid));

                    trans.Commit();

                    mes.retType = 100;
                    mes.Message = "报表生成成功！";
                }
                catch (Exception ex)
                {
                    trans.Rollback();

                    mes.retType = 1;
                    mes.Message = "报表生成失败！";
                }
            }

            return mes;
        }
    }
}
