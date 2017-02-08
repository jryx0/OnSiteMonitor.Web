using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using WFM.JW.HB.Models;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Data;
using System.Data.Common;

namespace WFM.JW.HB.Repository.EntLib
{
    public class ContractInfoRepository: BaseRepositorySql05
    {
        //Database _database; 
        public ContractInfoRepository()
        {
           // _database = EnterpriseLibraryContainer.Current.GetInstance<Database>("ApplicationServices");
        }

//        string selectSql = @"SELECT  [ContractID] ,
//                                A.[ProjectID] , B.ProjectName ,
//                                A.[SupplierID] , E.SupplierName ,
//                                A.[DepartmentID] , F.DepartmentName ,
//                                [ContractCode] ,
//                                [ContractName] ,
//                                [ContractValue] ,
//                                [ContractSettle] ,
//                                [ContractTypeID] ,  C.[ItemName] AS ContractType, 
//                                [ContractStatusID] ,  D.[ItemName] AS StatusName,
//                                A.[Comment] ,
//                               
//                                A.[Creator] ,
//                                A.[CreateDate] ,
//                                A.[Modifier] ,
//                                A.[ModifyDate] ,
//                                A.[ContractDate],
//                                cast(A.[version] as bigint) as [version]  
//                        FROM    [CMS_CSCL_ContractInfo] A
//                                JOIN dbo.CMS_CSCL_ProjectInfo B ON A.ProjectID = b.ProjectID
//                                JOIN dbo.CMS_CSCL_BaseTypeItem C ON A.ContractTypeID = C.BaseTypeItemID
//                                JOIN dbo.CMS_CSCL_BaseTypeItem D ON A.ContractStatusID = D.BaseTypeItemID
//                                JOIN dbo.CMS_CSCL_Supplier E ON A.SupplierID = E.SupplierID
//                                JOIN dbo.CMS_CSCL_Department F ON A.DepartmentID = F.DepartmentID";
         
        #region Query
        string selectSql = @"SELECT  [ContractID] ,
                                A.[ProjectID] , B.ProjectName ,
                                A.SupplierName ,
                                A.[DepartmentID] , F.DepartmentName ,
                                [ContractCode] ,
                                [ContractName] ,
                                [ContractValue] ,
                                [ContractSettle] ,
                                [ContractTypeID] ,  C.[ItemName] AS ContractType, 
                                [ContractStatusID] ,  D.[ItemName] AS StatusName,
                                A.[Comment] ,
                                A.[Filing] ,
                                A.[Creator] ,
                                A.[CreateDate] ,
                                A.[Modifier] ,
                                A.[ModifyDate] ,
                                A.[ContractDate],
                                cast(A.[version] as bigint) as [version]  
                        FROM    [CMS_CSCL_ContractInfo] A
                                JOIN dbo.CMS_CSCL_ProjectInfo B ON A.ProjectID = b.ProjectID
                                JOIN dbo.CMS_CSCL_BaseTypeItem C ON A.ContractTypeID = C.BaseTypeItemID
                                JOIN dbo.CMS_CSCL_BaseTypeItem D ON A.ContractStatusID = D.BaseTypeItemID
                                
                                JOIN dbo.CMS_CSCL_Department F ON A.DepartmentID = F.DepartmentID";


        public IRowMapper<ContractInfo> Mapor()
        {
            return MapBuilder<ContractInfo>.MapAllProperties()
                .Map(p => p.ContractID).ToColumn("ContractID")
                .Map(p => p.ContractCode).ToColumn("ContractCode")
                .Map(p => p.ContractName).ToColumn("ContractName")
                .Map(p => p.ContractValue).ToColumn("ContractValue")
                .Map(p => p.ContractSettle).ToColumn("ContractSettle")
                .Map(p => p.Comment).ToColumn("Comment")
                .Map(p => p.ContractDate).ToColumn("ContractDate")
                .Map(p => p.CreateDate).ToColumn("CreateDate")
                .Map(p => p.Creator).ToColumn("Creator")
                .Map(p => p.ModifyDate).ToColumn("ModifyDate")
                .Map(p =>p.Modifier).ToColumn("Modifier")
                .Map(p => p.version).ToColumn("version")
                .Map(p => p.Project).WithFunc(GenerateProject)
                .Map(p => p.SupplierName).ToColumn("SupplierName")
                .Map(p => p.ContractStatus).WithFunc(GenerateStatus)
                .Map(p => p.ContractType).WithFunc(GenerateType)
                .Map(p => p.Department).WithFunc(GenerateDepartment)
                .Build();
        }

        private Departments GenerateDepartment(IDataRecord record)
        {
            Departments department = new Departments();
            department.DepartmentID = (int)record["DepartmentID"];//"A.DepartmentID"
            department.DepartmentName = (string)record["DepartmentName"];//"DepartmentName"

            return department;
        }


        private TypeItem GenerateType(IDataRecord record)
        {
            TypeItem item = new TypeItem();
            item.BaseTypeItemID = (int)record["ContractTypeID"];//"A.ContractTypeID"
            item.ItemName = (string)record["ContractType"];

            return item;
        }

        private TypeItem GenerateStatus(IDataRecord record)
        {
            TypeItem item = new TypeItem();
            item.BaseTypeItemID = (int)record["ContractStatusID"];
            item.ItemName = (string)record["StatusName"];

            return item;
        }
     

        //private SupplierInfo GenerateSupplier(IDataRecord record)
        //{
        //    SupplierInfo supplier = new SupplierInfo();
        //    supplier.SupplierID = (int)record["SupplierID"];//"A.SupplierID"
        //    supplier.SupplierName = (string)record["SupplierName"];//"SupplierName"

        //    return supplier;
        //}

        private ProjectInfo GenerateProject(IDataRecord record)
        {
            ProjectInfo project = new ProjectInfo();
            project.ProjectID = (int)record["ProjectID"];//"A.ProjectID"
            project.ProjectName = (string)record["ProjectName"];//"ProjectName"

            return project;
        }

        public IEnumerable<Models.ContractInfo> GetAllData(Models.IParametersSpecification paraSpec)
        {
            //string sql;
            //if (paraSpec == null || String.IsNullOrEmpty(paraSpec.GetSpecValue()))
            //    sql = selectSql;
            //else sql = selectSql + " Where " + paraSpec.GetSpecValue();

            string sql = selectSql;
            if (paraSpec != null)
            {
                string parastring = paraSpec.GetSpecValue();
                if (!String.IsNullOrEmpty(parastring))
                    sql += " Where " + parastring;
            }

            sql += " Order by YEAR(A.[ContractDate]) ASC, MONTH(A.[ContractDate]) ASC, A.[ProjectID]";

            return _database.CreateSqlStringAccessor(sql, Mapor()).Execute();
        }


        public int GetPagedDataCount(Models.IParametersSpecification paraSpec)
        {
            var sql = GetCountSql(GetParaSql(selectSql, paraSpec));

            return (int)_database.ExecuteScalar(CommandType.Text, sql);
        }

        public DataSet GetGroupData(Models.IParametersSpecification paraSpec, string groupField)
        {
            var sql = GetGroupSql(GetParaSql(selectSql, paraSpec), groupField);

            return _database.ExecuteDataSet(CommandType.Text, sql);
        }

        public IEnumerable<Models.ContractInfo> GetPagedData(Models.IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            var sql = GetParaSql(selectSql, paraSpec);
            sql = GetPageSql(sql, pageIndex, pageSize);

            sql = sql.Replace("@order", "YEAR(A.[ContractDate]) ASC, MONTH(A.[ContractDate]) ASC, A.[ProjectID]");

            return _database.CreateSqlStringAccessor(sql, Mapor()).Execute();   
        }
        #endregion 

        public Models.Messaging<int> InsetData(Models.ContractInfo data)
        {
            if (data == null || data.Project == null || data.Department == null || data.ContractType == null || data.ContractStatus == null)
                return new Messaging<int> { retType = 1, Message = "下拉框选择不能有数据为全部！", Value = 0 };

            String InsertSql = @"INSERT INTO [CMS_CSCL_ContractInfo]
                               ([ProjectID] ,[SupplierName] ,[DepartmentID] ,[ContractCode]
                               ,[ContractName]  ,[ContractValue] 
                               ,[ContractTypeID] ,[ContractStatusID],[Comment],[Filing]
                               ,[Creator] ,[CreateDate] ,[Modifier],[ModifyDate], [ContractDate])
                         VALUES
                               (@ProjectID ,@SupplierName ,@DepartmentID
                               ,@ContractCode ,@ContractName  ,@ContractValue
                               ,@ContractTypeID ,@ContractStatusID
                               ,@Comment,@Filing,@Creator,@CreateDate ,@Modifier ,@ModifyDate, @ContractDate)";


            var insertCommand = _database.GetSqlStringCommand(InsertSql);


            _database.AddInParameter(insertCommand, "@ProjectID", DbType.Int32, data.Project.ProjectID);
            _database.AddInParameter(insertCommand, "@SupplierName", DbType.String, data.SupplierName);
            _database.AddInParameter(insertCommand, "@DepartmentID", DbType.Int32, data.Department.DepartmentID);
            _database.AddInParameter(insertCommand, "@ContractCode", DbType.String, data.ContractCode);

            _database.AddInParameter(insertCommand, "@ContractName", DbType.String, data.ContractName);
            _database.AddInParameter(insertCommand, "@ContractValue", DbType.Double, data.ContractValue);
 
            _database.AddInParameter(insertCommand, "@ContractTypeID", DbType.Int32, data.ContractType.BaseTypeItemID);
            _database.AddInParameter(insertCommand, "@ContractStatusID", DbType.Int32, data.ContractStatus.BaseTypeItemID);
            _database.AddInParameter(insertCommand, "@Comment", DbType.String, data.Comment);
            _database.AddInParameter(insertCommand, "@Filing", DbType.String, data.Filing);

            _database.AddInParameter(insertCommand, "@Creator", DbType.String, data.Creator);
            _database.AddInParameter(insertCommand, "@CreateDate", DbType.DateTime, data.CreateDate);
            _database.AddInParameter(insertCommand, "@Modifier", DbType.String, data.Creator);
            _database.AddInParameter(insertCommand, "@ModifyDate", DbType.DateTime, data.CreateDate);
            _database.AddInParameter(insertCommand, "@ContractDate", DbType.DateTime, data.ContractDate);


            var updateProjectCommand = _database.GetSqlStringCommand(updateProjectValue1);
            _database.AddInParameter(updateProjectCommand, "@ProjectID", DbType.Int32, data.Project.ProjectID);


            Messaging<int> mes = new Messaging<int>();
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
                        throw new Exception(mes.Message);
                    }
                    else
                    {
                        //project
                        _database.ExecuteNonQuery(updateProjectCommand, trans);
                        trans.Commit();
                    }
                }
                catch (Exception ex)
                {
                    mes.retType = 2;
                    mes.Message = "插入数据失败, Ex:" + ex.Message;
                    trans.Rollback();
                }
            }
            return mes;



        }

        public Models.Messaging<int> UpdateData(Models.ContractInfo data)
        {
            if (data == null ||  data.Project == null || data.Department == null || data.ContractType == null || data.ContractStatus == null)
                return new Messaging<int> { retType = 1, Message = "下拉框选择不能有数据为全部！", Value = 0 };

            String updateSql = @"UPDATE [CMS_CSCL_ContractInfo]
                                   SET [ProjectID] = @ProjectID
                                      ,[SupplierName] = @SupplierName
                                      ,[DepartmentID] = @DepartmentID
                                      ,[ContractCode] = @ContractCode
                                      ,[ContractName] = @ContractName
                                      ,[ContractValue] = @ContractValue
                                     
                                      ,[ContractTypeID] = @ContractTypeID
                                      ,[ContractStatusID] = @ContractStatusID
                                      ,[Comment] = @Comment
                                        ,[Filing] = @Filing
                                      ,[ContractDate] = @ContractDate
                                      ,[Modifier] = @Modifier
                                      ,[ModifyDate] = @ModifyDate
                                 WHERE version=@version and ContractID=@ContractID";


            var updateCommand = _database.GetSqlStringCommand(updateSql);


            _database.AddInParameter(updateCommand, "@ProjectID", DbType.Int32, data.Project.ProjectID);
            _database.AddInParameter(updateCommand, "@SupplierName", DbType.String, data.SupplierName);
            _database.AddInParameter(updateCommand, "@DepartmentID", DbType.Int32, data.Department.DepartmentID);
            _database.AddInParameter(updateCommand, "@ContractCode", DbType.String, data.ContractCode);

            _database.AddInParameter(updateCommand, "@ContractName", DbType.String, data.ContractName);
            _database.AddInParameter(updateCommand, "@ContractValue", DbType.Double, data.ContractValue);

            _database.AddInParameter(updateCommand, "@ContractTypeID", DbType.Int32, data.ContractType.BaseTypeItemID);
            _database.AddInParameter(updateCommand, "@ContractStatusID", DbType.Int32, data.ContractStatus.BaseTypeItemID);
            _database.AddInParameter(updateCommand, "@Comment", DbType.String, data.Comment);
            _database.AddInParameter(updateCommand, "@Filing", DbType.String, data.Filing);

            _database.AddInParameter(updateCommand, "@Modifier", DbType.String, data.Modifier);
            _database.AddInParameter(updateCommand, "@ModifyDate", DbType.DateTime, data.ModifyDate);
            _database.AddInParameter(updateCommand, "@ContractDate", DbType.DateTime, data.ContractDate);

            _database.AddInParameter(updateCommand, "@version", DbType.Int64, data.version);
            _database.AddInParameter(updateCommand, "@ContractID", DbType.Int32, data.ContractID);
  
            var updateProjectCommand = _database.GetSqlStringCommand(updateProjectValue1);
            _database.AddInParameter(updateProjectCommand, "@ProjectID", DbType.Int32, data.Project.ProjectID);


            Messaging<int> mes = new Messaging<int>();
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
                        mes.Message = "更新数据失败, 请返回重新新增!";
                        mes.retType = 10;
                        throw new Exception(mes.Message);
                    }
                    else
                    {
                        //project
                        _database.ExecuteNonQuery(updateProjectCommand, trans);
                        trans.Commit();
                    }
                }
                catch (Exception ex)
                {
                    mes.retType = 2;
                    mes.Message = "插入数据失败, Ex:" + ex.Message;
                    trans.Rollback();
                }
            }
            return mes;


        }

        public Models.Messaging<int> DeletData(Models.ContractInfo data)
        {
            if (data == null)
                return new Models.Messaging<int> { retType = 1, Message = "删除数据不存在", Value = 0 };

            string deleteSql = @"DELETE FROM [CMS_CSCL_ContractInfo] WHERE ContractID=@ContractID";

            var deleteCommand = _database.GetSqlStringCommand(deleteSql);
            _database.AddInParameter(deleteCommand, "@ContractID", DbType.Int32, data.ContractID);


            var updateProjectCommand = _database.GetSqlStringCommand(updateProjectValue1);
           


            Messaging<int> mes = new Messaging<int>();
            using (DbConnection conn = _database.CreateConnection())
            {
                conn.Open();
                DbTransaction trans = conn.BeginTransaction();

                try
                {
                    //get contracts projectid
                    ContractInfoRepository contract = new ContractInfoRepository();
                    Models.CriteriaSpecification spec = new CriteriaSpecification("contractid", CriteriaOperator.Equal, data.ContractID);
                    var list = contract.GetAllData(spec);
                    if (list.Count() != 0)
                    {
                        _database.AddInParameter(updateProjectCommand, "@ProjectID", DbType.Int32, list.First().Project.ProjectID);
                    }
                    else
                    {
                        mes.Message = "非法合同！";
                        throw new Exception(mes.Message);
                    }

                    mes.Value = _database.ExecuteNonQuery(deleteCommand,trans);
                    mes.retType = 100;
                    mes.Message = String.Format("删除数据成功, {0} 行受影响", mes.Value);

                    if (mes.Value == 0)
                    {
                        mes.Message = "删除数据失败, 用户数据已被其它用户修改。请返回重新修改!";
                        mes.retType = 10;
                        throw new Exception(mes.Message);
                    }
                    else
                    {
                        //project
                        _database.ExecuteNonQuery(updateProjectCommand, trans);
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    mes.retType = 2;
                    mes.Message = "删除数据失败, Ex:" + ex.Message; 
                    trans.Rollback();
                }
            }

            return mes;
        }





        #region UpdateProjectValue Sql
        String updateProjectValue1 = @"UPDATE  [CMS_CSCL_ProjectInfo]
                                    SET     [ContractsTotalValue] = B.value ,
                                            [ContractsNumbers] = B.cNumber
                                    FROM    ( SELECT    ProjectID ,
                                                        COUNT(ContractID) AS cNumber ,
                                                        SUM(ContractValue) AS value ,
                                                        SUM(ContractSettle) AS settle
                                              FROM      dbo.CMS_CSCL_ContractInfo
                                              WHERE     [ProjectID] = @ProjectID
                                              GROUP BY  ProjectID
                                            ) B
                                    WHERE   [CMS_CSCL_ProjectInfo].ProjectID = B.ProjectID";

        String updateProjectValue2 = @"UPDATE  [CMS_CSCL_ProjectInfo]
                                    SET     [ContractsTotalValue] = B.value ,
                                            [ContractsNumbers] = B.cNumber
                                    FROM    ( SELECT    ProjectID ,
                                                        COUNT(ContractID) AS cNumber ,
                                                        SUM(ContractValue) AS value ,
                                                        SUM(ContractSettle) AS settle
                                              FROM      dbo.CMS_CSCL_ContractInfo
                                              WHERE     [ProjectID] = ( SELECT  projectid
                                                                        FROM    [CMS_CSCL_ContractInfo]
                                                                        WHERE   ContractID = @ContractID
                                                                      )
                                              GROUP BY  ProjectID
                                            ) B
                                    WHERE   [CMS_CSCL_ProjectInfo].ProjectID = B.ProjectID";
        #endregion
    }
}
