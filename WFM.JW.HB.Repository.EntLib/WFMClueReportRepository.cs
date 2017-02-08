using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using WFM.JW.HB.Models;
using System.Data;

namespace WFM.JW.HB.Repository.EntLib
{
    public class WFMClueReportRepository : BaseRepositorySql06<Models.WFMClueSummary>
    {
        #region 查询
        #region Sql
        String SelectSql = @"  Select * from(
                               SELECT   '1' AS ID, 'Contry' AS Contry, 'ClueType' AS ClueType, '1' AS seq, CAST(a.ParentGUID AS varchar(36)) AS RegionGuid, 
		                                '1' as DataItem,SUM(ISNULL(TotalClues, 0)) AS TotalClues, SUM(ISNULL(InputErrors, 0)) AS InputErrors, 
		                                SUM(TotalClues - ISNULL(InputErrors, 0)) AS Problems, SUM(Confirmed) AS Confirmed, SUM(IsTrue) AS IsTrue	 
                                FROM      ClueData_CountBy,  HB_NewRegion AS a
                                where ClueData_CountBy.RegionGuid = a.RegionGUID and a.DirectCity = 0
                                group by  a.ParentGUID
                                union all
                                SELECT   '1' AS ID, 'Contry' AS Contry, 'ClueType' AS ClueType, '1' AS seq, CAST(ClueData_CountBy.RegionGuid AS varchar(36)) AS RegionGuid, 
		                                '1' as DataItem,SUM(ISNULL(TotalClues, 0)) AS TotalClues, SUM(ISNULL(InputErrors, 0)) AS InputErrors, 
		                                SUM(TotalClues - ISNULL(InputErrors, 0)) AS Problems, SUM(Confirmed) AS Confirmed, SUM(IsTrue) AS IsTrue
		 
                                FROM      ClueData_CountBy,  HB_NewRegion AS a
                                where ClueData_CountBy.RegionGuid = a.RegionGUID and a.DirectCity = 1
                                group by ClueData_CountBy.RegionGuid,a.ParentGUID) a";

        //String SqlReportCity = @"SELECT '1' AS ID, 'Contry' AS Contry, 'ClueType' AS ClueType, '1' AS seq,  CAST(RegionGuid AS varchar(36)) as RegionGuid
        //                              ,'1' as DataItem
        //                              ,[TotalClues]
        //                              ,[InputErrors]
        //                              ,[Problems]
        //                              ,[Confirmed]
        //                          FROM  [dbo].[vw_wfm_ReportByCity]";        

        String SqlReportCity = @"
SELECT   '1' AS ID, 'Contry' AS Contry, 'ClueType' AS ClueType, '1' AS seq, CAST(b.RegionGuid AS varchar(36)) AS RegionGuid, 
                '1' AS DataItem, ISNULL(a.TotalClues, 0) AS TotalClues, ISNULL(a.InputErrors, 0) AS InputErrors, ISNULL(a.Problems, 0) 
                AS Problems, ISNULL(a.Confirmed, 0) AS Confirmed , ISNULL(a.IsTrue, 0) AS IsTrue 
FROM      vw_wfm_ReportByCity AS a RIGHT OUTER JOIN
                HB_NewRegion AS b ON a.RegionGuid = b.RegionGUID
 

";

        //String SqlReportCounty = @" SELECT '1' AS ID, 'Contry' AS Contry, 'ClueType' AS ClueType, '1' AS seq,  CAST(RegionGuid AS varchar(36)) as RegionGuid
        //                              ,'1' as DataItem
        //                              ,[TotalClues]
        //                              ,[InputErrors]
        //                              ,[Problems]
        //                              ,[Confirmed]
        //                          FROM  [dbo].[vw_wfm_ReportByCounty]";

        String SqlReportCounty = @"
SELECT   '1' AS ID, 'Contry' AS Contry, 'ClueType' AS ClueType, '1' AS seq, CAST(b.RegionGUID AS varchar(36)) AS RegionGuid, 
                '1' AS DataItem, ISNULL(a.TotalClues, 0) AS TotalClues, ISNULL(a.InputErrors, 0) AS InputErrors, ISNULL(a.Problems, 0) 
                AS Problems, ISNULL(a.Confirmed, 0) AS Confirmed , ISNULL(a.IsTrue, 0) AS IsTrue 
FROM      vw_wfm_ReportByCounty AS a RIGHT OUTER JOIN
                HB_NewRegion AS b ON a.RegionGuid = b.RegionGUID
";

        String SqlReportTown = @"
                                    SELECT '1' AS ID, 'ClueType' AS ClueType, '1' AS seq,  CAST(RegionGuid AS varchar(36)) as RegionGuid
                                                                          ,'1' as DataItem,Town AS Contry
                                                                          ,[TotalClues]
                                                                          ,[InputErrors]
                                                                          ,[Problems]
                                                                          ,[Confirmed], ISNULL(IsTrue, 0) AS IsTrue 
                                            FROM       [dbo].[vw_wfm_ReportByCountyTown]  a
                                ";


        String SqlReportItem = @" SELECT '1' AS ID, 'Contry' AS Contry, 'ClueType' AS ClueType, '1' AS seq, 'RegionGuid' as RegionGuid
                                      ,  DataItem
                                      ,sum([TotalClues]) as TotalClues
                                      ,sum([InputErrors])as InputErrors
                                      ,sum([Problems]) as Problems
                                      ,sum([Confirmed]) as Confirmed, sum(IsTrue) AS IsTrue 
                                  FROM  [dbo].[vw_wfm_ReportByCityItem] a
                                ";
        String SqlReportItem2 = @"SELECT '1' AS ID, 'Contry' AS Contry, 'ClueType' AS ClueType, '1' AS seq, 'RegionGuid' as RegionGuid    
                                      ,  DataItem
                                      ,sum([TotalClues]) as TotalClues
                                      ,sum([InputErrors])as InputErrors
                                      ,sum([Problems]) as Problems
                                      ,sum([Confirmed]) as Confirmed, sum(IsTrue) AS IsTrue 
                                  FROM  [dbo].[vw_wfm_ReportByCountyItem] a
                                ";

        #endregion



        public override IRowMapper<WFMClueSummary> Mapor()
        {
            return MapBuilder<WFMClueSummary>.MapAllProperties()
               .Map(p => p.ID).ToColumn("Id")
               .Map(p => p.RegionGuid).ToColumn("RegionGuid")
               .Map(p => p.RegionName).ToColumn("RegionGuid")
               .Map(p => p.ContryName).ToColumn("Contry")
               .Map(p => p.ItemType).ToColumn("DataItem")
               .Map(p => p.ClueType).ToColumn("ClueType")
               .Map(p => p.TotalClues).ToColumn("TotalClues")
               .Map(p => p.InputErrors).ToColumn("InputErrors")
               .Map(p => p.Problems).ToColumn("Problems")
               .Map(p => p.Confirmed).ToColumn("Confirmed")
               .Map(p => p.Seq).ToColumn("Seq")
               .Map(p => p.IsTrue).ToColumn("IsTrue")
               .Build();
        }

        public IEnumerable<WFMClueSummary> GetAllData(Models.IParametersSpecification spec)
        {
            if (spec == null)
                spec = new CriteriaSpecification("1", CriteriaOperator.Equal, 1);

            return base.FindData(SelectSql, spec, "");
        }

        public IEnumerable<WFMClueSummary> GetPageDataByRegion(int type, IParametersSpecification spec, int PageNo, int PageSize)
        {
            if (spec == null)
                spec = new CriteriaSpecification("1", CriteriaOperator.Equal, 1);

            if (type == 0)
                 return base.GetPagedData(SqlReportCity, "b.Seq", spec.And(new CriteriaSpecification("b.RegionLevel", CriteriaOperator.Equal, 1)), PageNo, PageSize);
  //          return base.GetPagedData(SqlReportCity, "RegionGuid", spec, PageNo, PageSize);

            if (type == 1)
                //               return base.GetPagedData(SqlReportCounty, "RegionGuid", spec, PageNo, PageSize);
                return base.GetPagedData(SqlReportCounty, "b.Seq", spec.And(new CriteriaSpecification("b.RegionLevel", CriteriaOperator.Equal, 2)), PageNo, PageSize);

            if (type == 2)
                return base.GetPagedData(SqlReportTown, "RegionGuid", spec, PageNo, PageSize);

            return null;
        }
        public int GetPageDatCountByRegion(int type, IParametersSpecification spec)
        {
            if (spec == null)
                spec = new CriteriaSpecification("1", CriteriaOperator.Equal, 1);

            if (type == 0)
                //return base.GetPagedDataCount(SqlReportCity, spec);
                return base.GetPagedDataCount(SqlReportCity, spec.And(new CriteriaSpecification("b.RegionLevel", CriteriaOperator.Equal, 1)));

            if (type == 1)
                //return base.GetPagedDataCount(SqlReportCounty, spec);
                return base.GetPagedDataCount(SqlReportCounty, spec.And(new CriteriaSpecification("b.RegionLevel", CriteriaOperator.Equal, 2)));

            if (type == 2)
                return base.GetPagedDataCount(SqlReportTown, spec);

            return 0;
        }


        public IEnumerable<WFMClueSummary> GetPageDataByItem(int type, IParametersSpecification spec, int pageNo, int pageSize)
        {
            if (spec == null)
                spec = new CriteriaSpecification("1", CriteriaOperator.Equal, 1);

            String sql = "";
            if (type <= 1)
                sql = SqlReportItem;
            else sql = SqlReportItem2;


            sql += GenerateSql(spec, "DataITem", "");                
            sql = GetPageSql(sql, pageNo, pageSize);
            sql = sql.Replace("@order", "DataITem");

            return _database.CreateSqlStringAccessor(sql, Mapor()).Execute();             
        }
        public int GetPageDatCountByItem(int type, IParametersSpecification spec)
        {
            if (spec == null)
                spec = new CriteriaSpecification("1", CriteriaOperator.Equal, 1);

            String sql;
            if (type <= 1)
                sql = SqlReportItem;
            else sql = SqlReportItem2;

            sql += GenerateSql(spec, "DataITem", "");
            sql = GetCountSql(sql);

            var o = _database.ExecuteScalar(CommandType.Text, sql);
            if (o == null) return 0;

            return (int)o;
        }

       
        #endregion
    }



//    public class WFMClueReportRepository : BaseRepositorySql06<Models.WFMClueSummary>
//    {
//        #region 查询
//        #region Sql
//        String SelectSql = @"  Select * from(
//                               SELECT   '1' AS ID, 'Contry' AS Contry, 'ClueType' AS ClueType, '1' AS seq, CAST(a.ParentGUID AS varchar(36)) AS RegionGuid, 
//		                                '1' as DataItem,SUM(ISNULL(TotalClues, 0)) AS TotalClues, SUM(ISNULL(InputErrors, 0)) AS InputErrors, 
//		                                SUM(TotalClues - ISNULL(InputErrors, 0)) AS Problems, SUM(Confirmed) AS Confirmed	 
//                                FROM      ClueData_CountBy,  HB_NewRegion AS a
//                                where ClueData_CountBy.RegionGuid = a.RegionGUID and a.DirectCity = 0
//                                group by  a.ParentGUID
//                                union all
//                                SELECT   '1' AS ID, 'Contry' AS Contry, 'ClueType' AS ClueType, '1' AS seq, CAST(ClueData_CountBy.RegionGuid AS varchar(36)) AS RegionGuid, 
//		                                '1' as DataItem,SUM(ISNULL(TotalClues, 0)) AS TotalClues, SUM(ISNULL(InputErrors, 0)) AS InputErrors, 
//		                                SUM(TotalClues - ISNULL(InputErrors, 0)) AS Problems, SUM(Confirmed) AS Confirmed
		 
//                                FROM      ClueData_CountBy,  HB_NewRegion AS a
//                                where ClueData_CountBy.RegionGuid = a.RegionGUID and a.DirectCity = 1
//                                group by ClueData_CountBy.RegionGuid,a.ParentGUID) a";

//        //String SqlReportCity = @"SELECT '1' AS ID, 'Contry' AS Contry, 'ClueType' AS ClueType, '1' AS seq,  CAST(RegionGuid AS varchar(36)) as RegionGuid
//        //                              ,'1' as DataItem
//        //                              ,[TotalClues]
//        //                              ,[InputErrors]
//        //                              ,[Problems]
//        //                              ,[Confirmed]
//        //                          FROM  [dbo].[vw_wfm_ReportByCity]";        

//        String SqlReportCity = @"
//SELECT   '1' AS ID, 'Contry' AS Contry, 'ClueType' AS ClueType, '1' AS seq, CAST(b.RegionGuid AS varchar(36)) AS RegionGuid, 
//                '1' AS DataItem, ISNULL(a.TotalClues, 0) AS TotalClues, ISNULL(a.InputErrors, 0) AS InputErrors, ISNULL(a.Problems, 0) 
//                AS Problems, ISNULL(a.Confirmed, 0) AS Confirmed 
//FROM      vw_wfm_ReportByCity AS a RIGHT OUTER JOIN
//                HB_NewRegion AS b ON a.RegionGuid = b.RegionGUID
 

//";

//        //String SqlReportCounty = @" SELECT '1' AS ID, 'Contry' AS Contry, 'ClueType' AS ClueType, '1' AS seq,  CAST(RegionGuid AS varchar(36)) as RegionGuid
//        //                              ,'1' as DataItem
//        //                              ,[TotalClues]
//        //                              ,[InputErrors]
//        //                              ,[Problems]
//        //                              ,[Confirmed]
//        //                          FROM  [dbo].[vw_wfm_ReportByCounty]";

//        String SqlReportCounty = @"
//SELECT   '1' AS ID, 'Contry' AS Contry, 'ClueType' AS ClueType, '1' AS seq, CAST(b.RegionGUID AS varchar(36)) AS RegionGuid, 
//                '1' AS DataItem, ISNULL(a.TotalClues, 0) AS TotalClues, ISNULL(a.InputErrors, 0) AS InputErrors, ISNULL(a.Problems, 0) 
//                AS Problems, ISNULL(a.Confirmed, 0) AS Confirmed 
//FROM      vw_wfm_ReportByCounty AS a RIGHT OUTER JOIN
//                HB_NewRegion AS b ON a.RegionGuid = b.RegionGUID
//";

//        String SqlReportTown = @"
//                                    SELECT '1' AS ID, 'ClueType' AS ClueType, '1' AS seq,  CAST(RegionGuid AS varchar(36)) as RegionGuid
//                                                                          ,'1' as DataItem,Town AS Contry
//                                                                          ,[TotalClues]
//                                                                          ,[InputErrors]
//                                                                          ,[Problems]
//                                                                          ,[Confirmed]
//                                            FROM       [dbo].[vw_wfm_ReportByCountyTown]  
//                                ";


//        String SqlReportItem = @" SELECT '1' AS ID, 'Contry' AS Contry, 'ClueType' AS ClueType, '1' AS seq, 'RegionGuid' as RegionGuid
//                                      ,  DataItem
//                                      ,sum([TotalClues]) as TotalClues
//                                      ,sum([InputErrors])as InputErrors
//                                      ,sum([Problems]) as Problems
//                                      ,sum([Confirmed]) as Confirmed
//                                  FROM  [dbo].[vw_wfm_ReportByCityItem]
//                                ";
//        String SqlReportItem2 = @"SELECT '1' AS ID, 'Contry' AS Contry, 'ClueType' AS ClueType, '1' AS seq, 'RegionGuid' as RegionGuid    
//                                      ,  DataItem
//                                      ,sum([TotalClues]) as TotalClues
//                                      ,sum([InputErrors])as InputErrors
//                                      ,sum([Problems]) as Problems
//                                      ,sum([Confirmed]) as Confirmed
//                                  FROM  [dbo].[vw_wfm_ReportByCountyItem]
//                                ";

//        #endregion



//        public override IRowMapper<WFMClueSummary> Mapor()
//        {
//            return MapBuilder<WFMClueSummary>.MapAllProperties()
//               .Map(p => p.ID).ToColumn("Id")
//               .Map(p => p.RegionGuid).ToColumn("RegionGuid")
//               .Map(p => p.RegionName).ToColumn("RegionGuid")
//               .Map(p => p.ContryName).ToColumn("Contry")
//               .Map(p => p.ItemType).ToColumn("DataItem")
//               .Map(p => p.ClueType).ToColumn("ClueType")
//               .Map(p => p.TotalClues).ToColumn("TotalClues")
//               .Map(p => p.InputErrors).ToColumn("InputErrors")
//               .Map(p => p.Problems).ToColumn("Problems")
//               .Map(p => p.Confirmed).ToColumn("Confirmed")
//               .Map(p => p.Seq).ToColumn("Seq")
//               .Build();
//        }

//        public IEnumerable<WFMClueSummary> GetAllData(Models.IParametersSpecification spec)
//        {
//            if (spec == null)
//                spec = new CriteriaSpecification("1", CriteriaOperator.Equal, 1);

//            return base.FindData(SelectSql, spec, "");
//        }

//        public IEnumerable<WFMClueSummary> GetPageDataByRegion(int type, IParametersSpecification spec, int PageNo, int PageSize)
//        {
//            if (spec == null)
//                spec = new CriteriaSpecification("1", CriteriaOperator.Equal, 1);

//            if (type == 0)
//                return base.GetPagedData(SqlReportCity, "b.Seq", spec.And(new CriteriaSpecification("b.RegionLevel", CriteriaOperator.Equal, 1)), PageNo, PageSize);
//            //          return base.GetPagedData(SqlReportCity, "RegionGuid", spec, PageNo, PageSize);

//            if (type == 1)
//                //               return base.GetPagedData(SqlReportCounty, "RegionGuid", spec, PageNo, PageSize);
//                return base.GetPagedData(SqlReportCounty, "b.Seq", spec.And(new CriteriaSpecification("b.RegionLevel", CriteriaOperator.Equal, 2)), PageNo, PageSize);

//            if (type == 2)
//                return base.GetPagedData(SqlReportTown, "RegionGuid", spec, PageNo, PageSize);

//            return null;
//        }
//        public int GetPageDatCountByRegion(int type, IParametersSpecification spec)
//        {
//            if (spec == null)
//                spec = new CriteriaSpecification("1", CriteriaOperator.Equal, 1);

//            if (type == 0)
//                //return base.GetPagedDataCount(SqlReportCity, spec);
//                return base.GetPagedDataCount(SqlReportCity, spec.And(new CriteriaSpecification("b.RegionLevel", CriteriaOperator.Equal, 1)));

//            if (type == 1)
//                //return base.GetPagedDataCount(SqlReportCounty, spec);
//                return base.GetPagedDataCount(SqlReportCounty, spec.And(new CriteriaSpecification("b.RegionLevel", CriteriaOperator.Equal, 2)));

//            if (type == 2)
//                return base.GetPagedDataCount(SqlReportTown, spec);

//            return 0;
//        }


//        public IEnumerable<WFMClueSummary> GetPageDataByItem(int type, IParametersSpecification spec, int pageNo, int pageSize)
//        {
//            if (spec == null)
//                spec = new CriteriaSpecification("1", CriteriaOperator.Equal, 1);

//            String sql = "";
//            if (type <= 1)
//                sql = SqlReportItem;
//            else sql = SqlReportItem2;


//            sql += GenerateSql(spec, "DataITem", "");
//            sql = GetPageSql(sql, pageNo, pageSize);
//            sql = sql.Replace("@order", "DataITem");

//            return _database.CreateSqlStringAccessor(sql, Mapor()).Execute();
//        }
//        public int GetPageDatCountByItem(int type, IParametersSpecification spec)
//        {
//            if (spec == null)
//                spec = new CriteriaSpecification("1", CriteriaOperator.Equal, 1);

//            String sql;
//            if (type <= 1)
//                sql = SqlReportItem;
//            else sql = SqlReportItem2;

//            sql += GenerateSql(spec, "DataITem", "");
//            sql = GetCountSql(sql);

//            var o = _database.ExecuteScalar(CommandType.Text, sql);
//            if (o == null) return 0;

//            return (int)o;
//        }


//        #endregion
//    }
}
