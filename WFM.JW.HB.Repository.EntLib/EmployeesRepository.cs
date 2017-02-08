using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Data;
using System.Data.Common;


namespace WFM.JW.HB.Repository.EntLib
{
    public class EmployeesRepository:BaseRepositorySql05
    {
        //Database _database;
        public EmployeesRepository()
        {
          //  _database = EnterpriseLibraryContainer.Current.GetInstance<Database>("ApplicationServices");
        }

        #region query

        String SelectSql = @"SELECT  [EmployeeID] ,
                                [EmployeeName] ,
                                A.[Enable] ,
                                A.[DepartmentID] ,
                                CMS_CSCL_Department.DepartmentName,
                                [IsUser] ,
                                [UserName] ,
                                A.[Creator] ,
                                A.[CreateDate] ,
                                A.[Modifier] ,
                                A.[ModifyDate] ,
                                cast(A.[version] as bigint) [version]
                        FROM    [CMS_CSCL_Employee] A
                                JOIN dbo.CMS_CSCL_Department ON A.DepartmentID = dbo.CMS_CSCL_Department.DepartmentID";

        public IRowMapper<Models.Employees> Mapor()
        {
            return MapBuilder<Models.Employees>.MapAllProperties()
                .Map(p => p.EmployeeID).ToColumn("EmployeeID")
                .Map(p => p.EmployeeName).ToColumn("EmployeeName")
                .Map(p => p.IsUser).ToColumn("IsUser")
                .Map(p => p.Creator).ToColumn("Creator")
                .Map(p => p.CreateDate).ToColumn("CreateDate")
                .Map(p => p.Modifier).ToColumn("Modifier")
                .Map(p => p.ModifyDate).ToColumn("ModifyDate")
                .Map(p => p.version).ToColumn("version")
                .Map(p => p.Department).WithFunc(GenerateDepartment)
                .Build();
        }

        private Models.Departments GenerateDepartment(IDataRecord record)
        {
            Models.Departments department = new Models.Departments();
            department.DepartmentID = (int)record["DepartmentID"];
            department.DepartmentName = (string)record["DepartmentName"];

            return department;
        }

        public IEnumerable<Models.Employees> GetAllData(Models.IParametersSpecification paraSpec)
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

            sql += " Order by DepartmentName";
            return _database.CreateSqlStringAccessor(sql, Mapor()).Execute();
        }
        #endregion

        //新增
        public Models.Messaging<int> InsetData(Models.Employees data)
        {

            if (data == null || data.Department == null)
                return new Models.Messaging<int> { retType = 1, Message = "SupplierType为空", Value = 0 };

            String InsertSql = @"INSERT INTO [CMS_CSCL_Employee]
                                   ([EmployeeName]
                                   ,[Enable]
                                   ,[DepartmentID]
                                   ,[IsUser]
                                   ,[UserName]
                                   ,[Creator]
                                   ,[CreateDate]
                                   ,[Modifier]
                                   ,[ModifyDate])
                             VALUES
                                   (@EmployeeName
                                   ,@Enable
                                   ,@DepartmentID
                                   ,@IsUser
                                   ,@UserName
                                   ,@Creator
                                   ,@CreateDate
                                   ,@Modifier
                                   ,@ModifyDate)";

            var insertCommand = _database.GetSqlStringCommand(InsertSql);


            _database.AddInParameter(insertCommand, "@EmployeeName", DbType.String, data.EmployeeName);
            _database.AddInParameter(insertCommand, "@DepartmentID", DbType.Int32, data.Department.DepartmentID);
            _database.AddInParameter(insertCommand, "@Enable", DbType.Boolean, data.Enable);
            _database.AddInParameter(insertCommand, "@UserName", DbType.String, data.UserName);
            _database.AddInParameter(insertCommand, "@IsUser", DbType.Boolean, data.IsUser);
            _database.AddInParameter(insertCommand, "@Creator", DbType.String, data.Creator);
            _database.AddInParameter(insertCommand, "@CreateDate", DbType.DateTime, data.CreateDate);
            _database.AddInParameter(insertCommand, "@Modifier", DbType.String, data.Creator);
            _database.AddInParameter(insertCommand, "@ModifyDate", DbType.DateTime, data.CreateDate);



            Models.Messaging<int> mes = new Models.Messaging<int>();


            using (DbConnection conn = _database.CreateConnection())
            {
                conn.Open();
                DbTransaction trans = conn.BeginTransaction();
                
                try
                {

                    mes.Value = _database.ExecuteNonQuery(insertCommand, trans);
                    mes.retType = 100;
                    mes.Message = String.Format("插入数据成功, {0} 行受影响", mes.Value);

                    if (mes.Value == 0)
                    {
                        mes.Message = "插入数据失败, 请返回重新新增!";
                        mes.retType = 10;
                    }
                    else
                    {//insert into membership
                        if (data.IsUser)
                        {
                            if (System.Web.Security.Membership.GetUser(data.UserName) == null)
                            {
                                System.Web.Security.Membership.CreateUser(data.UserName, "123456");
                                
                                trans.Commit();
                            }
                            else
                            {
                                mes.retType = 90;
                                mes.Message = "系统用户[" + data.UserName + "]已存在";
                                trans.Rollback();
                            }
                        }
                        else trans.Commit();
                    }
                }
                catch (Exception ex)
                {
                    mes.retType = 2;
                    mes.Message = "插入数据失败, Ex:" + ex.Message;

                    trans.Rollback();
                }

                conn.Close();
            }
            return mes;
        }

        public Models.Messaging<int> UpdateData(Models.Employees data)
        {
            if (data == null || data.Department == null)
                return new Models.Messaging<int> { Message = "传入数据data为空", retType = 1, Value = 0 };

            string updateSql = @"UPDATE [CMS_CSCL_Employee]
                                    SET [EmployeeName] = @EmployeeName
                                        ,[Enable] = @Enable
                                        ,[DepartmentID] = @DepartmentID
                                        ,[IsUser] = @IsUser
                                        ,[UserName] = @UserName                                       
                                        ,[Modifier] = @Modifier
                                        ,[ModifyDate] = @ModifyDate
                                WHERE  [EmployeeID]=@EmployeeID AND [version]=@version";

            var updateCommand = _database.GetSqlStringCommand(updateSql);

            _database.AddInParameter(updateCommand, "@EmployeeName", DbType.String, data.EmployeeName);
            _database.AddInParameter(updateCommand, "@DepartmentID", DbType.Int32, data.Department.DepartmentID);
            _database.AddInParameter(updateCommand, "@Enable", DbType.Boolean, data.Enable);
            _database.AddInParameter(updateCommand, "@Modifier", DbType.String, data.Modifier);
            _database.AddInParameter(updateCommand, "@ModifyDate", DbType.DateTime, data.ModifyDate);
            _database.AddInParameter(updateCommand, "@IsUser", DbType.Boolean, data.IsUser);
            _database.AddInParameter(updateCommand, "@UserName", DbType.String, data.UserName);

            _database.AddInParameter(updateCommand, "@EmployeeID", DbType.String, data.EmployeeID);
            _database.AddInParameter(updateCommand, "@version", DbType.Int64, data.version);


            Models.Messaging<int> mes = new Models.Messaging<int>();

            //ToDo: 事务， isuser ＝ false 删除membership  isenble = false membership 禁用
            //ToDo: 取消 settlement同employee的关联， settlement中记录名称，不记录id
            using (DbConnection conn = _database.CreateConnection())
            {
                conn.Open();
                DbTransaction trans = conn.BeginTransaction();

                try
                {
                    mes.Value = _database.ExecuteNonQuery(updateCommand, trans);
                    mes.retType = 100;
                    mes.Message = String.Format("更新数据成功, {0} 行受影响", mes.Value);
                    if (mes.Value == 0)
                    {
                        mes.Message = "更新数据失败, 用户数据已被其它用户修改。请返回重新修改!";
                        mes.retType = 10;
                    }
                    else
                    {
                                                   
                        var user = System.Web.Security.Membership.GetUser(data.UserName);
                        if (user != null)
                        {//是membership 
                            user.IsApproved = data.Enable;
                            System.Web.Security.Membership.UpdateUser(user);
                        }

                        //是系统用户
                        if (data.IsUser)
                        {//user 不存在创建
                            if (user == null)
                                System.Web.Security.Membership.CreateUser(data.UserName, "123456");
                        }
                        else
                        {//不是系统用户， 存在删除
                            if (user != null)
                                System.Web.Security.Membership.DeleteUser(data.UserName);
                        }

                        trans.Commit();
                    }
                }
                catch (Exception ex)
                {
                    mes.retType = 2;
                    mes.Message = "更新数据失败.Ex message:" + ex.Message;
                    trans.Rollback();
                }
            }
            return mes;
        }

        public Models.Messaging<int> DeletData(Models.Employees data)
        {
            if (data == null)
                return new Models.Messaging<int> { Message = "传入数据data为空", retType = 1, Value = 0 };

            string updateSql = @"DELETE FROM  [CMS_CSCL_Employee]
                                WHERE EmployeeID=@EmployeeID";

            var deleteCommand = _database.GetSqlStringCommand(updateSql);

            _database.AddInParameter(deleteCommand, "@EmployeeID", DbType.Int32, data.EmployeeID);

            Models.Messaging<int> mes = new Models.Messaging<int>();

            //ToDo: 事务， isuser ＝ false 删除membership  isenble = false membership 禁用
            //ToDo: 取消 settlement同employee的关联， settlement中记录名称，不记录id
            using (DbConnection conn = _database.CreateConnection())
            {
                conn.Open();
                DbTransaction trans = conn.BeginTransaction();

                try
                {
                    mes.Value = _database.ExecuteNonQuery(deleteCommand, trans);
                    mes.retType = 100;
                    mes.Message = String.Format("更新数据成功, {0} 行受影响", mes.Value);
                    if (mes.Value == 0)
                    {
                        mes.Message = "删除数据失败, 用户数据已被其它用户修改。请返回重新修改!";
                        mes.retType = 10;
                    }
                    else
                    {

                        var user = System.Web.Security.Membership.GetUser(data.UserName);
                        if (user != null)
                        {//是membership 
                            System.Web.Security.Membership.DeleteUser(data.UserName);
                        }                      
                        trans.Commit();
                    }
                }
                catch (Exception ex)
                {
                    mes.retType = 2;
                    mes.Message = "删除数据失败.Ex message:" + ex.Message;
                    trans.Rollback();
                }
            }
            return mes;
        }


        public int GetPagedDataCount(Models.IParametersSpecification paraSpec)
        {

            var sql = GetCountSql(GetParaSql(SelectSql, paraSpec));

            return (int)_database.ExecuteScalar(CommandType.Text, sql);

        }

        public IEnumerable<Models.Employees> GetPagedData(Models.IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            var sql = GetParaSql(SelectSql, paraSpec);
            sql = GetPageSql(sql, pageIndex, pageSize);

            sql = sql.Replace("@order", "A.[DepartmentID]");

            return _database.CreateSqlStringAccessor(sql, Mapor()).Execute();
        }

    }

}
