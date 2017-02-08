using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Data;

namespace WFM.JW.HB.Repository.EntLib
{
    public class DepartmentRepository:BaseRepositorySql05
    {
        //Database _database;
        public DepartmentRepository()
        {
           // _database = EnterpriseLibraryContainer.Current.GetInstance<Database>("ApplicationServices");
        }

        #region 查询
        String SelectSql = @"SELECT [DepartmentID]
                              ,[DepartmentName]
                              ,[Enable]
                              ,[Creator]
                              ,[CreateDate]
                              ,[Modifier]
                              ,[ModifyDate]
                              ,cast([version] as bigint) as [version]
                          FROM [CMS_CSCL_Department] ";        


        public IRowMapper<Models.Departments> Mapor()
        {
            return MapBuilder<Models.Departments>.MapAllProperties()
                .Map(p => p.DepartmentID).ToColumn("DepartmentID")
                .Map(p => p.DepartmentName).ToColumn("DepartmentName")
                .Map(p => p.Enable).ToColumn("Enable")
                .Map(p => p.Creator).ToColumn("Creator")
                .Map(p => p.CreateDate).ToColumn("CreateDate")
                .Map(p => p.Modifier).ToColumn("Modifier")
                .Map(p => p.version).ToColumn("version")
                .Build();
        }

       
        public IEnumerable<Models.Departments> GetAllData(Models.IParametersSpecification paraSpec)
        {
            string sql = SelectSql;
            if (paraSpec != null)
            {
                string parastring = paraSpec.GetSpecValue();
                if (!String.IsNullOrEmpty(parastring))
                    sql += " Where " + parastring;
            }
            //string sql;
            //if (paraSpec == null || String.IsNullOrEmpty(paraSpec.GetSpecValue()))
            //    sql = SelectSql;
            //else sql = SelectSql + " Where " + paraSpec.GetSpecValue();

            return _database.CreateSqlStringAccessor(sql, Mapor()).Execute();
        }
        #endregion

        public Models.Messaging<int> InsetData(Models.Departments data)
        {
            if (data == null)
                return new Models.Messaging<int> { retType = 1, Message = "SupplierType为空", Value = 0 };

            String InsertSql = @"INSERT INTO [CMS_CSCL_Department]
                                   ([DepartmentName]
                                   ,[Enable]
                                   ,[Creator]
                                   ,[CreateDate]
                                   ,[Modifier]
                                   ,[ModifyDate])
                             VALUES
                                   (@DepartmentName
                                   ,@Enable
                                   ,@Creator
                                   ,@CreateDate
                                   ,@Modifier
                                   ,@ModifyDate)";

            var insertCommand = _database.GetSqlStringCommand(InsertSql);


            _database.AddInParameter(insertCommand, "@DepartmentName", DbType.String, data.DepartmentName);
            _database.AddInParameter(insertCommand, "@Enable", DbType.Boolean, true);

            _database.AddInParameter(insertCommand, "@Creator", DbType.String, data.Creator);
            _database.AddInParameter(insertCommand, "@CreateDate", DbType.DateTime, data.CreateDate);

            _database.AddInParameter(insertCommand, "@Modifier", DbType.String, data.Creator);
            _database.AddInParameter(insertCommand, "@ModifyDate", DbType.DateTime, data.CreateDate);



            Models.Messaging<int> mes = new Models.Messaging<int>();

            try
            {
                mes.Value = _database.ExecuteNonQuery(insertCommand);
                mes.retType = 100;
                mes.Message = String.Format("插入数据成功, {0} 行受影响", mes.Value);

                if (mes.Value == 0)
                {
                    mes.Message = "插入数据失败, 请返回重新新增!";
                    mes.retType = 10;
                }
            }
            catch (Exception ex)
            {
                mes.retType = 2;
                mes.Message = "插入数据失败, Ex:" + ex.Message;
            }
            return mes;
        }

        public Models.Messaging<int> UpdateData(Models.Departments data)
        {
           if (data == null)
                return new Models.Messaging<int> { Message = "传入数据data为空", retType = 1, Value = 0 };

            string updateSql = @"UPDATE [CMS_CSCL_Department]
                                   SET [DepartmentName] = @DepartmentName
                                      ,[Enable] = @ENABLE
                                      ,[Modifier] = @Modifier
                                      ,[ModifyDate] = @ModifyDate
                                 where  DepartmentID=@DepartmentID AND [version]=@version";


            var updateCommand = _database.GetSqlStringCommand(updateSql);

            _database.AddInParameter(updateCommand, "@DepartmentName", DbType.String, data.DepartmentName);
            _database.AddInParameter(updateCommand, "@ENABLE", DbType.Boolean, data.Enable);
            _database.AddInParameter(updateCommand, "@Modifier", DbType.String, data.Modifier);
            _database.AddInParameter(updateCommand, "@ModifyDate", DbType.DateTime, data.ModifyDate);
            _database.AddInParameter(updateCommand, "@DepartmentID", DbType.Int32, data.DepartmentID);
            _database.AddInParameter(updateCommand, "@version", DbType.Int64, data.version);


            Models.Messaging<int> mes = new Models.Messaging<int>();
            try
            {
                mes.Value = _database.ExecuteNonQuery(updateCommand);
                mes.retType = 100;
                mes.Message = String.Format("更新数据成功, {0} 行受影响", mes.Value);
                if (mes.Value == 0)
                {
                    mes.Message = "更新数据失败, 用户数据已被其它用户修改。请返回重新修改!";
                    mes.retType = 10;
                }
            }
            catch (Exception ex)
            {
                mes.retType = 2;
                mes.Message = "更新数据失败.Ex message:" + ex.Message;
            }
            return mes;


        }

        public Models.Messaging<int> DeletData(Models.Departments data)
        {
            if (data == null)
                return new Models.Messaging<int> { retType = 1, Message = "删除数据不存在", Value = 0 };

            string deleteSql = @"DELETE FROM [CMS_CSCL_Department] WHERE DepartmentID=@DepartmentID";

            var deleteCommand = _database.GetSqlStringCommand(deleteSql);

            _database.AddInParameter(deleteCommand, "@DepartmentID", DbType.Int32, data.DepartmentID);


            Models.Messaging<int> mes = new Models.Messaging<int>();

            try
            {
                mes.Value = _database.ExecuteNonQuery(deleteCommand);
                mes.retType = 100;
                mes.Message = String.Format("删除数据成功, {0} 行受影响", mes.Value);

                if (mes.Value == 0)
                {
                    mes.Message = "删除数据失败, 用户数据已被其它用户修改。请返回重新修改!";
                    mes.retType = 10;
                }
            }
            catch (Exception ex)
            {
                mes.retType = 2;
                mes.Message = "删除数据失败, Ex:" + ex.Message;
            }

            return mes;


        }


        #region 分页
        public int GetPagedDataCount(Models.IParametersSpecification paraSpec)
        {
            var sql = GetCountSql(GetParaSql(SelectSql, paraSpec));

            return (int)_database.ExecuteScalar(CommandType.Text, sql);
        }

        public IEnumerable<Models.Departments> GetPagedData(Models.IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            var sql = GetParaSql(SelectSql, paraSpec);
            sql = GetPageSql(sql, pageIndex, pageSize);

            sql = sql.Replace("@order", "DepartmentName");

            return _database.CreateSqlStringAccessor(sql, Mapor()).Execute();
        }

        #endregion

    }
}
