using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using WFM.JW.HB.Models;
using System.Data;
using System.Data.Common;


namespace WFM.JW.HB.Repository.EntLib
{
    public class WFMRegionUserRepository : BaseRepositorySql06<Models.RegionUser>
    {
        #region 查询
        string SelectSql = @"select 
                              a.RowID, 
                              CAST(a.RegionUserID AS VARCHAR(36)) RegionUserID,
                              a.RegionID,
                              CAST(a.RegionGuid AS VARCHAR(36)) RegionGUID,
                              a.ParentID,
                              CAST(a.ParentGuid AS VARCHAR(36)) ParentGuid,
                              a.UserName, 
                              a.Password,
                              a.Createby,
                              d.LastActivityDate,                             
                              CAST(a.version as bigint) as version
                            from  
	                            dbo.WFM_newRegionUser a 	                         
	                            join dbo.aspnet_Users d on a.UserName = d.UserName                            
                           ";


        public override IRowMapper<RegionUser> Mapor()
        {
            return MapBuilder<RegionUser>.MapAllProperties()
                   .Map(p => p.RowID).ToColumn("RowID")
                   .Map(p => p.RegionUserID).ToColumn("RegionUserID")
                   .Map(p => p.RegionID).ToColumn("RegionID")
                   .Map(p => p.ParentID).ToColumn("ParentID")     
                   .Map(p => p.UserName).ToColumn("UserName")
                   .Map(p => p.LastActivityDate).ToColumn("LastActivityDate")
                   .Map(p => p.version).ToColumn("version")
                   .Map(p => p.Password).ToColumn("Password")
                   .Map(p => p.Createby).ToColumn("Createby")
                   .Map(p => p.RegionGuid).ToColumn("RegionGuid")
                   .Map(p => p.ParentGuid).ToColumn("ParentGuid")
                   .Build();            
        }
        
        public IEnumerable<Models.RegionUser> FindRegion(Models.IParametersSpecification paraSpec)
        {
            return base.FindData(SelectSql, paraSpec, " RowID");
        }

        public int GetPagedDataCount(Models.IParametersSpecification paraSpec)
        {
            return base.GetPagedDataCount(SelectSql, paraSpec);
        }

        public IEnumerable<Models.RegionUser> GetPagedData(Models.IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            return base.GetPagedData(SelectSql, " RowID", paraSpec, pageIndex, pageSize);
        }



        #endregion


        public Models.Messaging<int> InsertData(Models.RegionUser data)
        {
            if (data == null)
                return new Models.Messaging<int> { retType = 1, Message = "RegionUser为空", Value = 0 };


            String InsertSql = @"
                                INSERT INTO [dbo].[WFM_NewRegionUser]
                                       ([RegionGuid],
                                        [ParentGuid],
                                        [UserName],
                                        [Password],
                                        [Createby]
                                       )
                                 VALUES
                                       (@RegionGuid, 
                                        @ParentGuid,
                                       @UserName ,
                                        @Password,
                                        @Createby
                                       )";


            var insertCommand = _database.GetSqlStringCommand(InsertSql);

            
                
            _database.AddInParameter(insertCommand, "@RegionGuid", DbType.String, data.RegionGuid);
            _database.AddInParameter(insertCommand, "@ParentGuid", DbType.String, data.ParentGuid);
            _database.AddInParameter(insertCommand, "@UserName", DbType.String, data.UserName.ToLower());
            _database.AddInParameter(insertCommand, "@Password", DbType.String, data.Password);
            _database.AddInParameter(insertCommand, "@Createby", DbType.String, data.Createby.ToLower());


            Models.Messaging<int> mes = new Messaging<int>();
            try
            {
                if (System.Web.Security.Membership.GetUser(data.UserName) == null)
                {
                    System.Web.Security.Membership.CreateUser(data.UserName.ToLower(), data.Password);

                    mes = null;
                    mes = base.InsertData(insertCommand);

                    if (mes.retType != 100)
                    {
                        System.Web.Security.Membership.DeleteUser(data.UserName);
                        mes.Message += "创建用户[" + data.UserName + "]失败。";
                    }else 
                    mes.Message = "创建用户[" + data.UserName + "]成功！";
                }
                else
                {
                    mes.retType = 90;
                    mes.Message = "用户[" + data.UserName + "]已存在";
                }
            }
            catch
            {
                mes.retType = 95;
                mes.Message = "创建用户[" + data.UserName + "]失败";
            }
            return mes;
        }


        public Models.Messaging<int> UpdateData(Models.RegionUser data)
        {
            Models.Messaging<int> mes = new Messaging<int>();
            mes.retType = 100;

            try
            {

                var user = System.Web.Security.Membership.GetUser(data.UserName);
                user.ChangePassword(user.ResetPassword(), data.Password);
                mes.Message = "密码修改成功！";


                String UpdateSql = @"update [dbo].[WFM_NewRegionUser] set password = @password where RegionUserID = @RegionUserID and version = @version";

                var updateCommand = _database.GetSqlStringCommand(UpdateSql);
                _database.AddInParameter(updateCommand, "@RegionUserID", DbType.String, data.RegionUserID);
                _database.AddInParameter(updateCommand, "@Version", DbType.Int64, data.version);
                _database.AddInParameter(updateCommand, "@password", DbType.String, data.Password);

                mes = base.ModifyData(updateCommand);
            }
            catch (Exception ex)
            {
                mes.retType = 20;
                mes.Message = "修改密码错误"+ ex.Message;
            }

            return mes;
        }


        public Messaging<int> DeleteData(RegionUser data)
        {
            Models.Messaging<int> mes = new Messaging<int>();
            mes.retType = 1;
            mes.Message = "RegionUser为空";
            mes.Value = 0;

            if (data == null)
                return mes;

            String DeleteSql = @"Delete from WFM_NewRegionUser where RegionUserID = @RegionUserID and version = @version";

            var deleteCommand = _database.GetSqlStringCommand(DeleteSql);
            _database.AddInParameter(deleteCommand, "@RegionUserID", DbType.String, data.RegionUserID);
            _database.AddInParameter(deleteCommand, "@Version", DbType.Int64, data.version);

            try
            {
                var list = FindRegion(new Models.CriteriaSpecification("RegionUserID", Models.CriteriaOperator.Equal, data.RegionUserID));
                var ur = list.First();
                if(ur == null)
                    return mes;

                if (System.Web.Security.Membership.GetUser(ur.UserName) != null)
                {
                    System.Web.Security.Membership.DeleteUser(ur.UserName);
                    mes = base.ModifyData(deleteCommand);
                    mes.Message = mes.Message.Replace("?", "删除");
                    mes.retType = 100;
                }
            }
            catch (Exception ex)
            {
                mes.retType = -1;
                mes.Message = ex.Message;
            }

            return mes;
        }
    }
}
