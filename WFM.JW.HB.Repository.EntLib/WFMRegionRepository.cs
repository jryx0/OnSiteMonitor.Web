using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using WFM.JW.HB.Models;
using System.Data;

namespace WFM.JW.HB.Repository.EntLib
{
    public class WFMRegionRepository : BaseRepositorySql05
    {
        public WFMRegionRepository()
        {
        }

        #region 查询
        string SelectSql = @"
                            SELECT A.rowid, 
                                   A.regionname, 
                                   B.regionname                      AS ParentName, 
                                   A.regionlevel, 
                                   A.DirectCity, 
                                   A.seq, 
                                   A.status, 
                                   Cast(A.version AS BIGINT)         version, 
                                   B.rowid                           AS Expr2, 
                                   B.seq                             AS Expr3, 
                                   a.regioncode, 
                                   a.parentid, 
                                   Cast(a.regionguid AS VARCHAR(36)) RegionGUID, 
                                   Cast(a.parentguid AS VARCHAR(36)) ParentGUID 
                            FROM   hb_newregion AS A 
                                   LEFT JOIN hb_newregion AS B 
                                          ON A.ParentGUID = B.RegionGUID";

        public int GetPagedDataCount(Models.IParametersSpecification paraSpec)
        {            
            return base.GetPagedDataCount(paraSpec, SelectSql);
        }

        public IEnumerable<Models.Region> GetPagedData(Models.IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            var sql = GetParaSql(SelectSql, paraSpec);
            sql = GetPageSql(sql, pageIndex, pageSize);

            sql = sql.Replace("@order", " B.Seq, A.Seq");

            return _database.CreateSqlStringAccessor(sql, Mapor()).Execute(); 
        }

        public IRowMapper<Region> Mapor()
        {
            return MapBuilder<Region>.MapAllProperties()
                   .Map(p => p.RowID).ToColumn("RowID")
                   .Map(p => p.RegionCode).ToColumn("RegionCode")
                   .Map(p => p.RegionName).ToColumn("RegionName")
                   .Map(p => p.ParentID).ToColumn("ParentID")
                   .Map(p => p.ParentName).ToColumn("ParentName")
                   .Map(p => p.Level).ToColumn("RegionLevel")
                   .Map(p => p.DirectCity).ToColumn("DirectCity")
                   .Map(p => p.Seq).ToColumn("Seq")
                   .Map(p => p.Status).ToColumn("Status")
                   .Map(p => p.version).ToColumn("version")
                   .Map(p => p.RegionGuid).ToColumn("RegionGUID")
                   .Map(p => p.ParentGuid).ToColumn("ParentGUID")
                   .Build();

        }

        public IEnumerable<Models.Region> FindRegion(Models.IParametersSpecification paraSpec)
        {
            string sql = SelectSql;
            if (paraSpec != null)
            {
                string parastring = paraSpec.GetSpecValue();
                if (!String.IsNullOrEmpty(parastring))
                    sql += " Where " + parastring;
            }

            sql += " order by B.Seq, A.Seq";
          
            return _database.CreateSqlStringAccessor(sql, Mapor()).Execute();
        }


        #endregion

        #region 更新
        string updateSql = @"Update HB_Region set DirectCity = @DirectCity where RowID = @RowID and version = @version";



        public Models.Messaging<int> UpdateData(Models.Region data)
        {
            var updateCommand = _database.GetSqlStringCommand(updateSql);


            _database.AddInParameter(updateCommand, "@DirectCity", DbType.String, data.DirectCity);

            _database.AddInParameter(updateCommand, "@RowID", DbType.Int32, data.RowID);
            _database.AddInParameter(updateCommand, "@version", DbType.Int64, data.version);

            return base.Update(updateCommand);
        }


        #endregion












        public Models.Messaging<int> InsetData(Models.Region data)
        {
            throw new NotImplementedException();
        }

        
        public Models.Messaging<int> DeletData(Models.Region data)
        {
            throw new NotImplementedException();
        }
               

        
    }
}
