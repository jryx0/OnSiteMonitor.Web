using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using WFM.JW.HB.Models;

namespace WFM.JW.HB.Repository.EntLib
{
    public class WFMClueSummaryRepository : BaseRepositorySql06<Models.WFMClueSummary>
    {
        #region Sql
        String SelectSql = @" SELECT Id,
                           Cast(RegionGuid as varchar(36)) as RegionGuid,
                           Contry,
                           DataItem,
                           ClueType,
                           TotalClues,
                           InputErrors,
                           Problems,
                           Confirmed,
                           Seq
                      FROM ClueData_CountBy";

        String GroupSelectSql1 = @"SELECT  '1' AS ID, 'Contry' AS Contry, 'ClueType' AS ClueType, '1' AS seq, CAST(RegionGuid AS varchar(36)) AS RegionGuid, 
                                            DataItem, SUM(ISNULL(TotalClues, 0)) AS TotalClues, SUM(ISNULL(InputErrors, 0)) AS InputErrors, 
                                            SUM(TotalClues - ISNULL(InputErrors, 0)) AS Problems, SUM(Confirmed) AS Confirmed
                                    FROM      ClueData_CountBy
                                    ";

        String GroupSelectSql2 = @"SELECT   '1' AS ID, 'Contry' AS Contry, 'ClueType' AS ClueType, '1' AS seq, CAST(RegionGuid AS varchar(36)) AS RegionGuid, 
                                            '1' as DataItem,SUM(ISNULL(TotalClues, 0)) AS TotalClues, SUM(ISNULL(InputErrors, 0)) AS InputErrors, 
                                            SUM(TotalClues - ISNULL(InputErrors, 0)) AS Problems, SUM(Confirmed) AS Confirmed
                                    FROM      ClueData_CountBy
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
                .Build();
        }

        public int GetPagedDataCount(IParametersSpecification paraSpec)
        {
            if (paraSpec == null)
                paraSpec = new Models.CriteriaSpecification("1", CriteriaOperator.Equal, 1);
            return base.GetPagedDataCount(SelectSql, paraSpec);
        }

        public IEnumerable<WFMClueSummary> GetPageData(IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            if (paraSpec == null)
                paraSpec = new Models.CriteriaSpecification("1", CriteriaOperator.Equal, 1);
            return base.GetPagedData(SelectSql, "  Contry", paraSpec, pageIndex, pageSize);
        }

        public IEnumerable<WFMClueSummary> GetAllData(IParametersSpecification paraSpec)
        {
            if (paraSpec == null)
                paraSpec = new Models.CriteriaSpecification("1", CriteriaOperator.Equal, 1);

            return base.FindData(SelectSql, paraSpec, " Contry");
        }


        public IEnumerable<WFMClueSummary> GetGroupData1(IParametersSpecification paraSpec)
        {
            if (paraSpec == null)
                paraSpec = new Models.CriteriaSpecification("1", CriteriaOperator.Equal, 1);

            string Clause = GenerateSql(paraSpec, "RegionGuid, DataItem", "");

            return base.FindData(GroupSelectSql1);
        }

        public IEnumerable<WFMClueSummary> GetGroupData2(IParametersSpecification paraSpec)
        {
            if (paraSpec == null)
                paraSpec = new Models.CriteriaSpecification("1", CriteriaOperator.Equal, 1);

            string Clause = GenerateSql(paraSpec, "RegionGuid", "");

            return base.FindData(GroupSelectSql2 + Clause);
        }



        public IEnumerable<WFMClueSummary> GetGroupData3(IParametersSpecification paraSpec)
        {
            if (paraSpec == null)
                paraSpec = new Models.CriteriaSpecification("1", CriteriaOperator.Equal, 1);

            string Clause = GenerateSql(paraSpec, "RegionGuid", "");

            return base.FindData(GroupSelectSql2 + Clause);
        }
    }

}
