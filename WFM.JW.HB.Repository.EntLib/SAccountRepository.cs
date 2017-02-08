using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Data;


namespace WFM.JW.HB.Repository.EntLib
{
    public class SAccountRepository : BaseRepositorySql05
    {
        //Database _database;
        public SAccountRepository()
        {
          //  _database = EnterpriseLibraryContainer.Current.GetInstance<Database>("ApplicationServices");
        }

        #region Query String
        string selectSql1 = @"SELECT  A.ContractID AS [1], A.ProjectID AS [2], B.ProjectName AS [3], A.SupplierName AS [4], F.DepartmentName AS [5], 
                                               A.ContractCode AS [6], A.ContractName AS [7], A.ContractValue AS [8], A.ContractSettle AS [9], C.ItemName AS [10], 
                                               D.ItemName AS [11], A.Comment AS [12], A.ContractDate AS [13], G.SettlementCode AS [14], G.SettlementDate AS [15], 
                                               G.AmountChanged AS [16], G.AmountDeclared AS [17], G.AmountAudited AS [18], G.AuditDeduction AS [19], 
                                               G.Filing AS [20], G.Comment AS [21], G.Auditor AS [22], G.Statementor AS [23], G.SettlementInfoID AS [24], 
                                               G.SettlementStatusID AS [25], H.ItemName AS [26], A.Filing AS[27]
                            FROM      CMS_CSCL_ContractInfo AS A INNER JOIN
                                               CMS_CSCL_ProjectInfo AS B ON A.ProjectID = B.ProjectID INNER JOIN
                                               CMS_CSCL_BaseTypeItem AS C ON A.ContractTypeID = C.BaseTypeItemID INNER JOIN
                                               CMS_CSCL_BaseTypeItem AS D ON A.ContractStatusID = D.BaseTypeItemID INNER JOIN
                                               
                                               CMS_CSCL_Department AS F ON A.DepartmentID = F.DepartmentID LEFT OUTER JOIN
                                               CMS_CSCL_SettlementInfo AS G ON A.ContractID = G.ContractID LEFT OUTER JOIN
                                               CMS_CSCL_BaseTypeItem AS H ON G.SettlementStatusID = H.BaseTypeItemID";
        string selectSql2 = @"SELECT  A.ContractID AS [1], A.ProjectID AS [2], B.ProjectName AS [3], A.SupplierName AS [4], F.DepartmentName AS [5], 
                                               A.ContractCode AS [6], A.ContractName AS [7], A.ContractValue AS [8], A.ContractSettle AS [9], C.ItemName AS [10], 
                                               D.ItemName AS [11], A.Comment AS [12], A.ContractDate AS [13], G.SettlementCode AS [14], G.SettlementDate AS [15], 
                                               G.AmountChanged AS [16], G.AmountDeclared AS [17], G.AmountAudited AS [18], G.AuditDeduction AS [19], 
                                               G.Filing AS [20], G.Comment AS [21], G.Auditor AS [22], G.Statementor AS [23], G.SettlementInfoID AS [24], 
                                               G.SettlementStatusID AS [25], H.ItemName AS [26], A.Filing AS[27]
                            FROM      CMS_CSCL_ContractInfo AS A INNER JOIN
                                               CMS_CSCL_ProjectInfo AS B ON A.ProjectID = B.ProjectID INNER JOIN
                                               CMS_CSCL_BaseTypeItem AS C ON A.ContractTypeID = C.BaseTypeItemID INNER JOIN
                                               CMS_CSCL_BaseTypeItem AS D ON A.ContractStatusID = D.BaseTypeItemID INNER JOIN
                                               
                                               CMS_CSCL_Department AS F ON A.DepartmentID = F.DepartmentID RIGHT OUTER JOIN
                                               CMS_CSCL_SettlementInfo AS G ON A.ContractID = G.ContractID INNER JOIN
                                               CMS_CSCL_BaseTypeItem AS H ON G.SettlementStatusID = H.BaseTypeItemID";


        #endregion


        public IRowMapper<Models.SettlementRecord> Mapor()
        {
            return MapBuilder<Models.SettlementRecord>.MapNoProperties()
                .Map(p => p.SettlementInfoID).ToColumn("24")
                .Map(p => p.SettlementCode).ToColumn("14")
                .Map(p => p.SettlementDate).ToColumn("15")
                .Map(p => p.AmountChanged).ToColumn("16")
                .Map(p => p.AmountDeclared).ToColumn("17")
                .Map(p => p.AmountAudited).ToColumn("18")
                .Map(p => p.AuditDeduction).ToColumn("19")
                .Map(p => p.Filing).ToColumn("20")
                .Map(p => p.Comment).ToColumn("21")
                .Map(p => p.Auditor).ToColumn("22")
                .Map(p => p.Statementor).ToColumn("23")
                .Map(p => p.SettlementStatus).WithFunc(GenerateStatus)
                .Map(p => p.Contract).WithFunc(GenerateContract)
                .Build();
        }

        private Models.TypeItem GenerateStatus(IDataRecord record)
        {
            Models.TypeItem item = new Models.TypeItem();
            if (record["25"] == DBNull.Value)
                return item;            

            item.BaseTypeItemID = (int)record["25"];
            item.ItemName = (string)record["26"];
            
            return item;
        }

        private Models.ContractInfo GenerateContract(IDataRecord record)
        {
            Models.ContractInfo contract = new Models.ContractInfo();

            contract.ContractStatus = new Models.TypeItem();
            contract.ContractType = new Models.TypeItem();
            contract.Department = new Models.Departments();
            contract.Project = new Models.ProjectInfo();
            //contract.Supplier = new Models.SupplierInfo();


            contract.ContractID = (int)record["1"];

            contract.Project.ProjectID = (int)record["2"];
            contract.Project.ProjectName = (string)record["3"];

            contract.SupplierName = (string)record["4"];
            contract.Department.DepartmentName = (string)record["5"];


            contract.ContractCode = (string)record["6"];
            contract.ContractName = (string)record["7"];

            if (record["8"] != DBNull.Value)
            {
                double contractvalue = 0;
                Double.TryParse(record["8"].ToString(), out contractvalue);
                contract.ContractValue = contractvalue;
                
            }
            if (record["9"] != DBNull.Value)
            {
                double contractsettle = 0;
                Double.TryParse(record["9"].ToString(), out contractsettle);
                contract.ContractSettle = contractsettle;
                
            }

            contract.ContractType.ItemName = (string)record["10"];
            contract.ContractStatus.ItemName = (string)record["11"];

            contract.Comment = record["12"].ToString();
            if (record["13"] != DBNull.Value)
            {
                contract.ContractDate = (System.DateTime)record["13"];
            }


            contract.Filing = record["27"].ToString();
           

            return contract;
        }

        public IEnumerable<Models.SettlementRecord> GetAllAccount(Models.IParametersSpecification paraSpec, bool allData)
        {

            var sql = base.GetParaSql(allData ? selectSql1 : selectSql2, paraSpec);

            sql += " ORDER BY a.ContractDate, a.ContractCode, g.SettlementDate, g.SettlementCode";

            return _database.CreateSqlStringAccessor(sql, Mapor()).Execute();
        }

        public IEnumerable<Models.SettlementRecord> GetSettlementAccount(Models.IParametersSpecification paraSpec)
        {           
            var sql = base.GetParaSql(selectSql2, paraSpec); 

            return _database.CreateSqlStringAccessor(sql, Mapor()).Execute();
        }


        public int AccountCount(Models.IParametersSpecification paraSpec, bool allData)
        {
            var sql = GetCountSql(GetParaSql(allData ? selectSql1 : selectSql2, paraSpec));

            return (int) _database.ExecuteScalar(CommandType.Text, sql);
        }


        public DataSet AcountGroupData(Models.IParametersSpecification paraSpec, bool allData, string groupField)
        {
            var sql = GetGroupSql(GetParaSql(allData ? selectSql1 : selectSql2, paraSpec), groupField);

            return _database.ExecuteDataSet(CommandType.Text, sql);
        }

        public IEnumerable<Models.SettlementRecord> GetPagedAccount(Models.IParametersSpecification paraSpec, int pageIndex, int pageSize, bool allData)
        {
            var sql = GetParaSql(allData ? selectSql1 : selectSql2, paraSpec);
            sql = GetPageSql(sql, pageIndex, pageSize);

            sql = sql.Replace("@order", "YEAR(A.[ContractDate]) ASC, MONTH(A.[ContractDate]) ASC, A.[ContractCode], YEAR(G.SettlementDate) ASC, MONTH(G.SettlementDate) ASC,A.[ProjectID]");

            return _database.CreateSqlStringAccessor(sql, Mapor()).Execute();
        }

        public IEnumerable<Models.SettlementRecord> GetPagedAccount2(Models.IParametersSpecification paraSpec, int pageIndex, int pageSize, bool allData)
        {
            var sql = GetParaSql(allData ? selectSql1 : selectSql2, paraSpec);
            sql = GetPageSql(sql, pageIndex, pageSize);

            sql = sql.Replace("@order", "YEAR(A.[ContractDate]) ASC, MONTH(A.[ContractDate]) ASC, A.[ContractName], YEAR(G.SettlementDate) ASC, MONTH(G.SettlementDate) ASC");

            return _database.CreateSqlStringAccessor(sql, Mapor()).Execute();
        }
    }
}
