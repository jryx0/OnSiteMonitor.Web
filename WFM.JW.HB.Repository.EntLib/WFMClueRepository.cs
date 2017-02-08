using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using WFM.JW.HB.Models;
using System.Data;

namespace WFM.JW.HB.Repository.EntLib
{
    public class WFMClueRepository : BaseRepositorySql06<Models.Clues>
    {
        public override IRowMapper<Clues> Mapor()
        {
            return MapBuilder<Clues>.MapAllProperties()
                .Map(p => p.RowID).ToColumn("RowID")
                .Map(p => p.ClueGuid).ToColumn("DataGuid")
                .Map(p => p.ID).ToColumn("PersonID")
                .Map(p => p.Name).ToColumn("PersonName")
                .Map(p => p.Region).ToColumn("PersonRegion")
                .Map(p => p.Addr).ToColumn("PersonAddr")
                .Map(p => p.Type).ToColumn("ClueType")               
                .Map(p => p.DateRange).ToColumn("DateRange")
                .Map(p => p.Table1).ToColumn("Table1")
                .Map(p => p.Comment).ToColumn("Comment")
                .Map(p => p.InputError).ToColumn("InputError")
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
                .Build();
        }

        string Sql = @"SELECT cast(DataGuid AS VARCHAR(36)) as DataGuid,[PersonID],[PersonName],[PersonRegion],[PersonAddr],[ClueType]
                      ,[Comment],[DateRange],[Table1],[InputError],[Confirmed],[RowID],[IsClueTrue],[IsCompliance]
                      ,[IsCP],[Fact],[IllegalMoney] ,[CheckDate],[CheckByName1],[CheckByName2]
                      ,[ReCheckFact],[ReCheckType],[ReCheckByName1]  FROM   @Table";

        String SqlTable = @"select* from sys.tables t join sys.schemas s on (t.schema_id = s.schema_id)
             where s.name = 'dbo' and t.name = '@tablename'";

        public IEnumerable<Clues> GetAllData(String tablename, IParametersSpecification paraSpec)
        {
            try
            {                
                var o = _database.ExecuteScalar(CommandType.Text, SqlTable.Replace("@tablename", tablename));

                if (o != null)
                {
                    Sql = Sql.Replace("@Table", tablename);
                    return base.FindData(Sql, paraSpec, " PersonRegion, Table1, ClueType");
                }
            }
            catch(Exception ex)
            {

            }
             

            return null;
        }

        public new int GetPagedDataCount(string tablename, Models.IParametersSpecification paraSpec)
        {

            try
            {
                var o = _database.ExecuteScalar(CommandType.Text, SqlTable.Replace("@tablename", tablename));

                if (o != null)
                {
                    Sql = Sql.Replace("@Table", tablename);
                    return base.GetPagedDataCount(Sql, paraSpec);
                }
            }
            catch (Exception ex)
            {

            }
            
            return 0;
           
        }

        public IEnumerable<Models.Clues> GetPagedData(string tablename, Models.IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            try
            {
                var o = _database.ExecuteScalar(CommandType.Text, SqlTable.Replace("@tablename", tablename));

                if (o != null)
                {
                    Sql = Sql.Replace("@Table", tablename);
                    return base.GetPagedData(Sql, " PersonRegion, Table1, ClueType", paraSpec, pageIndex, pageSize);
                }
            }
            catch (Exception ex)
            {

            }
            
            return null;
        }
    }
}
