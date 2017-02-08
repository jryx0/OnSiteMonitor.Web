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
    public class SupplierRepository : BaseRepositorySql05
    {
       // Database _database;
        public SupplierRepository()
        {
           // _database = EnterpriseLibraryContainer.Current.GetInstance<Database>("ApplicationServices");
        }

        #region 新增Insert
        public Models.Messaging<int> InsetData(Models.SupplierInfo data)
        {
            if (data == null || data.SupplierType == null)
                return new Messaging<int> { retType = 1, Message = "SupplierType为空", Value = 0 };
           
            String InsertSql = @"INSERT INTO [CMS_CSCL_Supplier]
                                   ([SupplierName]
                                   ,[SupplierTypeID]
                                   ,[Enable]
                                   ,[Comment]
                                   ,[Creator]
                                   ,[CreateDate])
                             VALUES
                                   (@SupplierName
                                   ,@SupplierTypeID 
                                   ,@Enable 
                                   ,@Comment
                                   ,@Creator
                                   ,@CreateDate)";

            var insertCommand = _database.GetSqlStringCommand(InsertSql);


            _database.AddInParameter(insertCommand, "@SupplierName", DbType.String, data.SupplierName);
            _database.AddInParameter(insertCommand, "@SupplierTypeID", DbType.Int32, data.SupplierType.BaseTypeItemID);
            _database.AddInParameter(insertCommand, "@Enable", DbType.Boolean, data.Enable);
            _database.AddInParameter(insertCommand, "@Comment", DbType.String, data.Comment);

            _database.AddInParameter(insertCommand, "@Creator", DbType.String, data.Creator);
            _database.AddInParameter(insertCommand, "@CreateDate", DbType.DateTime, data.CreateDate);



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
        #endregion

        #region 修改Update
        public Models.Messaging<int> UpdateData(Models.SupplierInfo data)
        {
            if (data == null || data.SupplierType == null)
                return new Messaging<int> { Message = "传入数据data为空", retType = 1, Value = 0 };

            string updateSql = @"UPDATE  [CMS_CSCL_Supplier]
                                SET [SupplierName] = @SupplierName
                                    ,[SupplierTypeID] = @SupplierTypeID
                                    ,[Enable] = @Enable
                                    ,[Comment] = @Comment
                                    ,[Modifier] = @Modifier
                                    ,[ModifyDate] = @ModifyDate
                                WHERE  SupplierID=@SupplierID AND [version]=@version";

            var updateCommand = _database.GetSqlStringCommand(updateSql);

            _database.AddInParameter(updateCommand, "@SupplierName", DbType.String, data.SupplierName);
            _database.AddInParameter(updateCommand, "@SupplierTypeID", DbType.Int32, data.SupplierType.BaseTypeItemID);
            _database.AddInParameter(updateCommand, "@Enable", DbType.Boolean, data.Enable);
            _database.AddInParameter(updateCommand, "@Comment", DbType.String, data.Comment);
            _database.AddInParameter(updateCommand, "@Modifier", DbType.String, data.Modifier);
            _database.AddInParameter(updateCommand, "@ModifyDate", DbType.DateTime, data.ModifyDate);


            _database.AddInParameter(updateCommand, "@SupplierID", DbType.Int32, data.SupplierID);
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
        #endregion


        #region Delete
        public Models.Messaging<int> DeletData(Models.SupplierInfo data)
        {
            if (data == null)
                return new Models.Messaging<int> { retType = 1, Message = "删除数据不存在", Value = 0 };

            string deleteSql = @"DELETE FROM [CMS_CSCL_Supplier] WHERE SupplierID=@SupplierID";

            var deleteCommand = _database.GetSqlStringCommand(deleteSql);

            _database.AddInParameter(deleteCommand, "@SupplierID", DbType.Int32, data.SupplierID);


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
        #endregion

        #region 查询Query
        String SelectSql = @"SELECT  A.[SupplierID] ,
                                    A.[SupplierName] ,
                                    A.[SupplierTypeID] ,
                                    [CMS_CSCL_BaseTypeItem].ItemName ,
                                    A.[Enable] ,
                                    A.[Comment] ,
                                    A.[Creator] ,
                                    A.[CreateDate] ,
                                    A.[Modifier] ,
                                    A.[ModifyDate] ,
                                    CAST(A.[version] AS BIGINT) AS [version]
                            FROM    [CMS_CSCL_Supplier] A
                                    JOIN [CMS_CSCL_BaseTypeItem] ON A.[SupplierTypeID] = [CMS_CSCL_BaseTypeItem].BaseTypeItemID";
        public IEnumerable<Models.SupplierInfo> FindSupplier(Models.IParametersSpecification spec)
        {
            string sql = SelectSql;
            if (spec != null)
            {
                string parastring = spec.GetSpecValue();
                if (!String.IsNullOrEmpty(parastring))
                    sql += " Where " + parastring;
            }

            //string sql;
            //if (spec == null || String.IsNullOrEmpty(spec.GetSpecValue()))
            //    sql = SelectSql;
            //else sql = SelectSql + " Where " + spec.GetSpecValue();

            return _database.CreateSqlStringAccessor(sql, Mapor()).Execute();
        }

        public IRowMapper<SupplierInfo> Mapor()
        {
            return MapBuilder<SupplierInfo>.MapAllProperties()
                .Map(p => p.SupplierID).ToColumn("SupplierID")
                .Map(p => p.SupplierName).ToColumn("SupplierName")
                .Map(p => p.Enable).ToColumn("Enable")
                .Map(p => p.Comment).ToColumn("Comment")
                .Map(p => p.Creator).ToColumn("Creator")
                .Map(p => p.CreateDate).ToColumn("CreateDate")
                .Map(p => p.Modifier).ToColumn("Modifier")
                .Map(p => p.ModifyDate).ToColumn("ModifyDate")
                .Map(p => p.version).ToColumn("version")
                .Map(p => p.SupplierType).WithFunc(GenerateObject)
                .Build();
        }

        private TypeItem GenerateObject(IDataRecord record)
        {
            TypeItem suppliertype = new TypeItem();
            suppliertype.BaseTypeItemID = (int)record["SupplierTypeID"];
            suppliertype.ItemName = (string)record["ItemName"];

            return suppliertype;
        }

        #endregion


        public int GetPagedDataCount(Models.IParametersSpecification paraSpec)
        {

            var sql = GetCountSql(GetParaSql(SelectSql, paraSpec));

            return (int)_database.ExecuteScalar(CommandType.Text, sql);

        }

        public IEnumerable<Models.SupplierInfo> GetPagedData(Models.IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            var sql = GetParaSql(SelectSql, paraSpec);
            sql = GetPageSql(sql, pageIndex, pageSize);

            sql = sql.Replace("@order", "A.[SupplierName]");

            return _database.CreateSqlStringAccessor(sql, Mapor()).Execute();            
        }
    }
}
