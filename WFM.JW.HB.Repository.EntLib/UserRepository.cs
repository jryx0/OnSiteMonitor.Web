using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace WFM.JW.HB.Repository.EntLib
{
    public class UserRepository 
    {
        Database _database;

       public UserRepository()
       {
           _database = EnterpriseLibraryContainer.Current.GetInstance<Database>("ApplicationServices");
       }

       String SelectSql = @"SELECT  [UserID], [UserName], [Password] from sys_user";


       public IRowMapper<Models.User> Mapor()
       {
           return MapBuilder<Models.User>.MapAllProperties()
               .Map(p => p.UserName).ToColumn("UserName")
               .Map(p => p.PassWord).ToColumn("Password")              
               .Build();
       }

       public IEnumerable<Models.User> GetUser(Models.IParametersSpecification spec)
       {

           string sql;
           if (spec == null || String.IsNullOrEmpty(spec.GetSpecValue()))
               sql = SelectSql;
           else sql = SelectSql + " Where " + spec.GetSpecValue();


           return _database.CreateSqlStringAccessor(sql, Mapor()).Execute();
       }
    }
}
