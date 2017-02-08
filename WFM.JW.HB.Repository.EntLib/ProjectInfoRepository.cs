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
    public class ProjectInfoRepository : BaseRepositorySql05
    {
       // Database _database;
        public ProjectInfoRepository()
        {
        //    _database = EnterpriseLibraryContainer.Current.GetInstance<Database>("ApplicationServices");
        }

        #region 查询
        String SelectSql = @"SELECT [ProjectID]
                              ,[ProjectName]
                              ,[ProjectShortName]            
                              ,[ProjectTypeID]
                              ,[ProjectCode]
                              ,[ProjectStartDate]
                              ,[ProjectEndDate]
                              ,B.ItemName as ProjectTypeName
                              ,[ContractsNumbers]
                              ,[ContractsTotalValue]
                              ,[ContractsTotalSettle]
                              ,A.[Comment]
                              ,[StatusID]
                              ,C.ItemName as StatusName
                              ,A.[Creator]
                              ,A.[CreateDate]
                              ,A.[Modifier]
                              ,A.[ModifyDate]
                              ,CAST(A.[version] AS BIGINT) AS [version]
                          FROM [CMS_CSCL_ProjectInfo] A 
                              JOIN [CMS_CSCL_BaseTypeItem] B ON A.ProjectTypeID = B.BaseTypeItemID
                              JOIN [CMS_CSCL_BaseTypeItem] C ON A.StatusID = C.BaseTypeItemID";


        public IRowMapper<ProjectInfo> Mapor()
        {
            return MapBuilder<ProjectInfo>.MapAllProperties()
                .Map(p => p.ProjectID).ToColumn("ProjectID")
                .Map(p => p.ProjectName).ToColumn("ProjectName")
                .Map(p => p.ProjectShortName).ToColumn("ProjectShortName")
                .Map(p => p.ProjectCode).ToColumn("ProjectCode")
                .Map(p => p.ProjectStartDate).ToColumn("ProjectStartDate")
                .Map(p => p.ProjectEndDate).ToColumn("ProjectEndDate")
                .Map(p => p.ContractsTotalValue).ToColumn("ContractsTotalValue")
                .Map(p => p.ContractsTotalSettle).ToColumn("ContractsTotalSettle")
                .Map(p => p.ContractsNumbers).ToColumn("ContractsNumbers")
                .Map(p => p.Comment).ToColumn("Comment")
                .Map(p => p.Creator).ToColumn("Creator")
                .Map(p => p.CreateDate).ToColumn("CreateDate")
                .Map(p => p.Modifier).ToColumn("Modifier")
                .Map(p => p.ModifyDate).ToColumn("ModifyDate")
                .Map(p => p.version).ToColumn("version")
                .Map(p => p.ProjectType).WithFunc(GenerateProjectInfo)
                .Map(p => p.Status).WithFunc(GenerateStatus)
                .Build();
        }
        private TypeItem GenerateStatus(IDataRecord record)
        {
            TypeItem statustype = new TypeItem();
            statustype.BaseTypeItemID = (int)record["StatusID"];
            statustype.ItemName = (string)record["StatusName"];

            return statustype;
        }

        private TypeItem GenerateProjectInfo(IDataRecord record)
        {
            TypeItem projecttype = new TypeItem();
            projecttype.BaseTypeItemID = (int)record["ProjectTypeID"];
            projecttype.ItemName = (string)record["ProjectTypeName"];

            return projecttype;
        }


        public IEnumerable<Models.ProjectInfo> GetAllData(Models.IParametersSpecification paraSpec)
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

        //新增Insert
        public Models.Messaging<int> InsetData(Models.ProjectInfo data)
        {
            if (data == null || data.ProjectType == null || data.Status == null)
                return new Messaging<int> { retType = 1, Message = "项目类型或项目状态为空", Value = 0 };

            String InsertSql = @"INSERT  INTO [CMS_CSCL_ProjectInfo]
                                            ( [ProjectName] ,
                                              [ProjectShortName] ,
                                              [ProjectStartDate] ,
                                              [ProjectEndDate] ,
                                              [ProjectTypeID] ,
                                              [Comment] ,
                                              [StatusID] ,
                                              [Creator] ,
                                              [CreateDate] ,
                                              [Modifier] ,
                                              [ModifyDate])
                                    VALUES  ( @ProjectName ,
                                              @ProjectShortName ,
                                              @ProjectStartDate ,
                                              @ProjectEndDate ,
                                              @ProjectTypeID ,
                                              @Comment ,
                                              @StatusID ,
                                              @Creator ,
                                              @CreateDate ,
                                              @Modifier ,
                                              @ModifyDate)";

            var insertCommand = _database.GetSqlStringCommand(InsertSql);


            _database.AddInParameter(insertCommand, "@ProjectName", DbType.String, data.ProjectName);
            _database.AddInParameter(insertCommand, "@ProjectShortName", DbType.String, data.ProjectShortName);
            _database.AddInParameter(insertCommand, "@ProjectStartDate", DbType.DateTime, data.ProjectStartDate);
            _database.AddInParameter(insertCommand, "@ProjectEndDate", DbType.DateTime, data.ProjectEndDate);
            _database.AddInParameter(insertCommand, "@ProjectTypeID", DbType.Int32, data.ProjectType.BaseTypeItemID);
            _database.AddInParameter(insertCommand, "@Comment", DbType.String, data.ProjectType.Comment);
            _database.AddInParameter(insertCommand, "@StatusID", DbType.Int32, data.Status.BaseTypeItemID);
            _database.AddInParameter(insertCommand, "@Creator", DbType.String, data.Creator);
            _database.AddInParameter(insertCommand, "@CreateDate", DbType.DateTime, data.CreateDate);
            _database.AddInParameter(insertCommand, "@Modifier", DbType.String, data.Creator);
            _database.AddInParameter(insertCommand, "@ModifyDate", DbType.DateTime, data.CreateDate);


            Messaging<int> mes = new Messaging<int>();

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

        //删除Delete
        public Models.Messaging<int> UpdateData(Models.ProjectInfo data)
        {
            if (data == null || data.ProjectType == null || data.Status == null)
                return new Messaging<int> { Message = "传入数据data为空", retType = 1, Value = 0 };

            string updateSql = @" UPDATE [CMS_CSCL_ProjectInfo]
                                       SET [ProjectName] = @ProjectName  
                                          ,[ProjectShortName] = @ProjectShortName                                         
                                          ,[ProjectStartDate] = @ProjectStartDate
                                          ,[ProjectEndDate] = @ProjectEndDate
                                          ,[ProjectTypeID] = @ProjectTypeID
                                          ,[Comment] = @Comment
                                          ,[StatusID] = @StatusID
                                          ,[Modifier] = @Modifier
                                          ,[ModifyDate] = @ModifyDate
                                     WHERE ProjectID=@ProjectID AND [version]=@version";

            var updateCommand = _database.GetSqlStringCommand(updateSql);

            _database.AddInParameter(updateCommand, "@ProjectName", DbType.String, data.ProjectName);
            _database.AddInParameter(updateCommand, "@ProjectShortName", DbType.String, data.ProjectShortName);
            _database.AddInParameter(updateCommand, "@ProjectStartDate", DbType.DateTime, data.ProjectStartDate);
            _database.AddInParameter(updateCommand, "@ProjectEndDate", DbType.DateTime, data.ProjectEndDate);
            _database.AddInParameter(updateCommand, "@ProjectTypeID", DbType.Int32, data.ProjectType.BaseTypeItemID);
            _database.AddInParameter(updateCommand, "@Comment", DbType.String, data.Comment);
            _database.AddInParameter(updateCommand, "@StatusID", DbType.String, data.Status.BaseTypeItemID);
            _database.AddInParameter(updateCommand, "@Modifier", DbType.String, data.Modifier);
            _database.AddInParameter(updateCommand, "@ModifyDate", DbType.DateTime, data.ModifyDate);
            _database.AddInParameter(updateCommand, "@ProjectID", DbType.Int32, data.ProjectID);
            _database.AddInParameter(updateCommand, "@version", DbType.Int64, data.version);


            Messaging<int> mes = new Messaging<int>();
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

        public Models.Messaging<int> DeletData(Models.ProjectInfo data)
        {
            if (data == null)
                return new Models.Messaging<int> { retType = 1, Message = "删除数据不存在", Value = 0 };

            string deleteSql = @"DELETE FROM [CMS_CSCL_ProjectInfo] WHERE ProjectID=@ProjectID";

            var deleteCommand = _database.GetSqlStringCommand(deleteSql);

            _database.AddInParameter(deleteCommand, "@ProjectID", DbType.Int32, data.ProjectID);


            Messaging<int> mes = new Messaging<int>();

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

        public IEnumerable<Models.ProjectInfo> GetPagedData(Models.IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            var sql = GetParaSql(SelectSql, paraSpec);
            sql = GetPageSql(sql, pageIndex, pageSize);

            sql = sql.Replace("@order", "ProjectName");

            return _database.CreateSqlStringAccessor(sql, Mapor()).Execute();  
        }

        #endregion

    }
}
