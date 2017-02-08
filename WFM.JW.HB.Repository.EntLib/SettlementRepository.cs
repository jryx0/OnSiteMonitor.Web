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
    public class SettlementRepository:BaseRepositorySql05
    {
        //Database _database;
        public SettlementRepository()
        {
          //  _database = EnterpriseLibraryContainer.Current.GetInstance<Database>("ApplicationServices");
        }

        #region Query

        String SelectSql = @"SELECT [SettlementInfoID]
                                ,[SettlementStatusID]                        ,CMS_CSCL_BaseTypeItem.ItemName
                                ,CMS_CSCL_SettlementInfo.[ContractID]       ,CMS_CSCL_ContractInfo.ContractName
                                ,CMS_CSCL_ContractInfo.[ContractCode]     ,CMS_CSCL_ContractInfo.ProjectID
                                ,[SettlementCode]
                                ,[SettlementInfo]
                                ,[SettlementDate]
                                ,[AmountChanged]
                                ,[AmountDeclared]
                                ,[AmountAudited]
                                ,[AuditDeduction]
                                ,CMS_CSCL_SettlementInfo.[Filing]
                                ,CMS_CSCL_SettlementInfo.[Comment]
                                ,[Auditor]
                                ,[Statementor]
                                ,CMS_CSCL_SettlementInfo.[Creator]
                                ,CMS_CSCL_SettlementInfo.[CreateDate]
                                ,CMS_CSCL_SettlementInfo.[Modifier]
                                ,CMS_CSCL_SettlementInfo.[modifyDate]
                                ,cast(CMS_CSCL_SettlementInfo.[version] as bigint) as [version]
                              FROM [CMS_CSCL_SettlementInfo]
                            JOIN dbo.CMS_CSCL_ContractInfo ON dbo.CMS_CSCL_SettlementInfo.ContractID = dbo.CMS_CSCL_ContractInfo.ContractID
                            JOIN dbo.CMS_CSCL_BaseTypeItem ON CMS_CSCL_SettlementInfo.SettlementStatusID = BaseTypeItemID";

        public IRowMapper<Models.SettlementRecord> Mapor()
        {
            return MapBuilder<Models.SettlementRecord>.MapAllProperties()
                .Map(p => p.SettlementInfoID).ToColumn("SettlementInfoID")
                .Map(p => p.SettlementCode).ToColumn("SettlementCode")
                .Map(p => p.SettlementInfo).ToColumn("SettlementInfo")
                .Map(p => p.SettlementDate).ToColumn("SettlementDate")
                .Map(p => p.Filing).ToColumn("Filing")
                .Map(p => p.Comment).ToColumn("Comment")
                .Map(p => p.Creator).ToColumn("Creator")
                .Map(p => p.CreateDate).ToColumn("CreateDate")
                .Map(p => p.Modifier).ToColumn("Modifier")
                .Map(p => p.modifyDate).ToColumn("modifyDate")
                .Map(p => p.version).ToColumn("version")
                .Map(p => p.AmountAudited).ToColumn("AmountAudited")
                .Map(p => p.AmountChanged).ToColumn("AmountChanged")
                .Map(p => p.AmountDeclared).ToColumn("AmountDeclared")
                .Map(p => p.AuditDeduction).ToColumn("AuditDeduction")
                .Map(p => p.Auditor).ToColumn("Auditor")
                .Map(p => p.Statementor).ToColumn("Statementor")
                .Map(p => p.SettlementStatus).WithFunc(GenerateStatus)
                .Map(p => p.Contract).WithFunc(GenerateContract)
                .Build();
        }

        private Models.TypeItem GenerateStatus(IDataRecord record)
        {
            Models.TypeItem item = new Models.TypeItem();
            item.BaseTypeItemID = (int)record["SettlementStatusID"];
            item.ItemName = (string)record["ItemName"];

            return item;
        }


        private Models.ContractInfo GenerateContract(IDataRecord record)
        {
            Models.ContractInfo contract = new Models.ContractInfo();
            contract.ContractID = (int)record["ContractID"];
            contract.ContractName = (string)record["ContractName"];
            contract.ContractCode = (string)record["ContractCode"];
            contract.Project = new Models.ProjectInfo();
            contract.Project.ProjectID = (int)record["ProjectID"];

            return contract;
        }


        public IEnumerable<Models.SettlementRecord> GetAllData(Models.IParametersSpecification paraSpec)
        {
            string sql = SelectSql;
            if (paraSpec != null)
            {
                string parastring = paraSpec.GetSpecValue();
                if (!String.IsNullOrEmpty(parastring))
                    sql += " Where " + parastring;
            }

            sql += " Order by SettlementDate DESC";

            return _database.CreateSqlStringAccessor(sql, Mapor()).Execute();

        }


        public int GetPagedDataCount(Models.IParametersSpecification paraSpec)
        {
            var sql = GetCountSql(GetParaSql(SelectSql, paraSpec));

            return (int)_database.ExecuteScalar(CommandType.Text, sql);
        }


        public DataSet GetGroupData(Models.IParametersSpecification paraSpec, string groupField)
        {
            var sql = GetGroupSql(GetParaSql(SelectSql, paraSpec), groupField);

            return _database.ExecuteDataSet(CommandType.Text, sql);
        }


        public IEnumerable<Models.SettlementRecord> GetPagedData(Models.IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            var sql = GetParaSql(SelectSql, paraSpec);
            sql = GetPageSql(sql, pageIndex, pageSize);

            sql = sql.Replace("@order", "YEAR(SettlementDate) ASC, MONTH(SettlementDate) ASC");

            return _database.CreateSqlStringAccessor(sql, Mapor()).Execute();         
        }

        #endregion

        #region 新增
        public Models.Messaging<int> InsetData(Models.SettlementRecord data)
        {
            String InsertSql =
                            @"INSERT  [CMS_CSCL_SettlementInfo]
                                    ( [SettlementStatusID] , [ContractID] ,       [SettlementCode] ,    [SettlementInfo] ,
                                      [SettlementDate] ,     [AmountChanged] ,    [AmountDeclared] ,    [AmountAudited] ,
                                      [AuditDeduction] ,    [Filing] ,           [Comment] ,           [Auditor] ,
                                      [Statementor] ,       [Creator] ,          [CreateDate] ,        [Modifier] ,
                                      [modifyDate])
                            VALUES  ( @SettlementStatusID ,  @ContractID ,        @SettlementCode ,     @SettlementInfo ,     
                                      @SettlementDate ,      @AmountChanged ,     @AmountDeclared ,     @AmountAudited ,
                                      @AuditDeduction ,     @Filing ,            @Comment ,            @Auditor ,
                                      @Statementor ,        @Creator ,           @CreateDate ,         @Modifier ,
                                      @modifyDate)";



            if (data == null || data.SettlementStatus == null || data.Contract == null)
                return new Models.Messaging<int> { retType = 1, Message = "TypeItem为空", Value = 0 };

            var insertCommand = _database.GetSqlStringCommand(InsertSql);

            _database.AddInParameter(insertCommand, "@SettlementStatusID", DbType.Int32, data.SettlementStatus.BaseTypeItemID);
            _database.AddInParameter(insertCommand, "@ContractID", DbType.String, data.Contract.ContractID);
            _database.AddInParameter(insertCommand, "@SettlementCode", DbType.String, data.SettlementCode);
            _database.AddInParameter(insertCommand, "@SettlementInfo", DbType.String, data.SettlementInfo);

            _database.AddInParameter(insertCommand, "@SettlementDate", DbType.DateTime, data.SettlementDate);
            _database.AddInParameter(insertCommand, "@AmountChanged", DbType.Currency, data.AmountChanged);
            _database.AddInParameter(insertCommand, "@AmountDeclared", DbType.Currency, data.AmountDeclared);
            _database.AddInParameter(insertCommand, "@AmountAudited", DbType.Currency, data.AmountAudited);

            //data.AuditDeduction = data.AmountDeclared - data.AmountAudited;
            _database.AddInParameter(insertCommand, "@AuditDeduction", DbType.Currency, data.AuditDeduction);
            _database.AddInParameter(insertCommand, "@Filing", DbType.String, data.Filing);
            _database.AddInParameter(insertCommand, "@Comment", DbType.String, data.Comment);
            _database.AddInParameter(insertCommand, "@Auditor", DbType.String, data.Auditor);

            _database.AddInParameter(insertCommand, "@Statementor", DbType.String, data.Statementor);
            _database.AddInParameter(insertCommand, "@Creator", DbType.String, data.Creator);
            _database.AddInParameter(insertCommand, "@CreateDate", DbType.DateTime, data.CreateDate);
            _database.AddInParameter(insertCommand, "@Modifier", DbType.String, data.Modifier);

            _database.AddInParameter(insertCommand, "@modifyDate", DbType.DateTime, data.CreateDate);


            var updateContractCommand = _database.GetSqlStringCommand(updateContractAudit);
            _database.AddInParameter(updateContractCommand, "@ContractID", DbType.Int32, data.Contract.ContractID);

            var updateProjectCommand = _database.GetSqlStringCommand(updateProjectAudit);
            _database.AddInParameter(updateProjectCommand, "@ContractID", DbType.Int32, data.Contract.ContractID);


            Models.Messaging<int> mes = new Models.Messaging<int>();


            using (DbConnection conn = _database.CreateConnection())
            {
                conn.Open();
                DbTransaction trans = conn.BeginTransaction();

                try
                {//updata settlement
                    mes.Value = _database.ExecuteNonQuery(insertCommand, trans);
                    mes.retType = 100;
                    mes.Message = String.Format("插入数据成功, {0} 行受影响", mes.Value);
                    if (mes.Value == 0)
                    {
                        mes.Message = "插入数据失败, 用户数据已被其它用户修改。请返回重新修改!";
                        mes.retType = 10;
                        throw new Exception(mes.Message);
                    }
                    else
                    {
                        //contract
                        _database.ExecuteNonQuery(updateContractCommand, trans);                       
                        //project
                        _database.ExecuteNonQuery(updateProjectCommand, trans);
                        trans.Commit();
                    }

                   
                }
                catch (Exception ex)
                {
                    mes.retType = 2;
                    mes.Message += "插入数据失败, Ex:" + ex.Message;
                    trans.Rollback();
                }
            }
            return mes;
        }
        #endregion

        #region 修改Update
        public Models.Messaging<int> UpdateData(Models.SettlementRecord data)
        {//ToDo: updata contract value, project value
            if (data == null || data.SettlementStatus == null | data.Contract == null)
                return new Models.Messaging<int> { Message = "传入数据data为空", retType = 1, Value = 0 };

            string updateSql = @"UPDATE  [CMS_CSCL_SettlementInfo]
                                   SET [SettlementStatusID] = @SettlementStatusID
                                      ,[ContractID] = @ContractID
                                      ,[SettlementCode] = @SettlementCode
                                      ,[SettlementInfo] = @SettlementInfo
                                      ,[SettlementDate] = @SettlementDate
                                      ,[AmountChanged] = @AmountChanged
                                      ,[AmountDeclared] = @AmountDeclared
                                      ,[AmountAudited] = @AmountAudited
                                      ,[AuditDeduction] = @AuditDeduction
                                      ,[Filing] = @Filing
                                      ,[Comment] = @Comment
                                      ,[Auditor] = @Auditor
                                      ,[Statementor] = @Statementor
                                     
                                      ,[Modifier] = @Modifier
                                      ,[modifyDate] = @modifyDate
                                WHERE  [SettlementInfoID]=@SettlementInfoID AND [version]=@version";

            var updateCommand = _database.GetSqlStringCommand(updateSql);

            _database.AddInParameter(updateCommand, "@SettlementStatusID", DbType.Int32, data.SettlementStatus.BaseTypeItemID);
            _database.AddInParameter(updateCommand, "@ContractID", DbType.Int32, data.Contract.ContractID);
            _database.AddInParameter(updateCommand, "@SettlementCode", DbType.String, data.SettlementCode);
            _database.AddInParameter(updateCommand, "@SettlementInfo", DbType.String, data.SettlementInfo);
            _database.AddInParameter(updateCommand, "@SettlementDate", DbType.DateTime, data.SettlementDate);
            _database.AddInParameter(updateCommand, "@AmountChanged", DbType.Currency, data.AmountChanged);
            _database.AddInParameter(updateCommand, "@AmountDeclared", DbType.Currency, data.AmountDeclared);
            _database.AddInParameter(updateCommand, "@AmountAudited", DbType.Currency, data.AmountAudited);
            
           //data.AuditDeduction = data.AmountDeclared - data.AmountAudited;
            _database.AddInParameter(updateCommand, "@AuditDeduction", DbType.Currency, data.AuditDeduction);
            _database.AddInParameter(updateCommand, "@Filing", DbType.String, data.Filing);
            _database.AddInParameter(updateCommand, "@Comment", DbType.String, data.Comment);
            _database.AddInParameter(updateCommand, "@Auditor", DbType.String, data.Auditor);
            _database.AddInParameter(updateCommand, "@Statementor", DbType.String, data.Statementor);

            _database.AddInParameter(updateCommand, "@Modifier", DbType.String, data.Modifier);
            _database.AddInParameter(updateCommand, "@modifyDate", DbType.DateTime, data.modifyDate);


            _database.AddInParameter(updateCommand, "@SettlementInfoID", DbType.Int32, data.SettlementInfoID);
            _database.AddInParameter(updateCommand, "@version", DbType.Int64, data.version);


            var updateContractCommand = _database.GetSqlStringCommand(updateContractAudit);
            _database.AddInParameter(updateContractCommand, "@ContractID", DbType.Int32, data.Contract.ContractID);

            var updateProjectCommand = _database.GetSqlStringCommand(updateProjectAudit);
            _database.AddInParameter(updateProjectCommand, "@ContractID", DbType.Int32, data.Contract.ContractID);




            Models.Messaging<int> mes = new Models.Messaging<int>();

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
                        throw new Exception(mes.Message);
                    }
                    else
                    {
                        //contract
                        _database.ExecuteNonQuery(updateContractCommand, trans);
                        //project
                        _database.ExecuteNonQuery(updateProjectCommand, trans);
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
        #endregion

        #region Delete
        public Models.Messaging<int> DeletData(Models.SettlementRecord data)
        {
            if (data == null)
                return new Models.Messaging<int> { retType = 1, Message = "删除数据不存在", Value = 0 };

            string deleteSql = @"DELETE FROM  [CMS_CSCL_SettlementInfo] WHERE SettlementInfoID=@SettlementInfoID";

            var deleteCommand = _database.GetSqlStringCommand(deleteSql);

            _database.AddInParameter(deleteCommand, "@SettlementInfoID", DbType.Int32, data.SettlementInfoID);

            var updateContractCommand = _database.GetSqlStringCommand(updateContractAudit);
            _database.AddInParameter(updateContractCommand, "@ContractID", DbType.Int32, data.Contract.ContractID);

            var updateProjectCommand = _database.GetSqlStringCommand(updateProjectAudit);
            _database.AddInParameter(updateProjectCommand, "@ContractID", DbType.Int32, data.Contract.ContractID);

            Models.Messaging<int> mes = new Models.Messaging<int>();
            using (DbConnection conn = _database.CreateConnection())
            {
                conn.Open();
                DbTransaction trans = conn.BeginTransaction();
                try
                {
                    mes.Value = _database.ExecuteNonQuery(deleteCommand, trans);
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

                        //contract
                        _database.ExecuteNonQuery(updateContractCommand, trans);
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
        #endregion


        #region UpdateSettlements Sql
        String updateContractAudit = @"UPDATE [CMS_CSCL_ContractInfo]
                                    SET     [ContractSettle] = ( SELECT SUM([AmountAudited]) AS totalAudited
                                                                 FROM    [CMS_CSCL_SettlementInfo]
                                                                 WHERE  [CMS_CSCL_SettlementInfo].[ContractID] = @ContractID
                                                               )
                                    WHERE   [CMS_CSCL_ContractInfo].[ContractID] = @ContractID";
        String updateProjectAudit1 = @"UPDATE  [CMS_CSCL_ProjectInfo]
                                    SET     [ContractsTotalSettle] = B.settle 
                                    FROM    ( SELECT    ProjectID ,
                                                        COUNT(ContractID) AS cNumber ,
                                                        SUM(ContractValue) AS value ,
                                                        SUM(ContractSettle) AS settle
                                              FROM      dbo.CMS_CSCL_ContractInfo
                                              WHERE     [ProjectID] = @ProjectID
                                              GROUP BY  ProjectID
                                            ) B
                                    WHERE   [CMS_CSCL_ProjectInfo].ProjectID = B.ProjectID";

        String updateProjectAudit = @"UPDATE  [CMS_CSCL_ProjectInfo]
                                    SET     [ContractsTotalSettle] = B.settle 
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
