using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WFM.JW.HB.Models;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.SqlClient;
using System.Data;

namespace WFM.JW.HB.Repository.EntLib
{
    public class BaseTypeItemRepository : BaseRepositorySql05
    {
        //Database _database;

        public BaseTypeItemRepository()
        {
          //  _database = EnterpriseLibraryContainer.Current.GetInstance<Database>("ApplicationServices");
        }

        
        //新增Insert
        public Messaging<int> InsertItemType(TypeItem item)
        { 
            String InsertSql = @"INSERT INTO [CMS_CSCL_BaseTypeItem]
                               ([BaseTypeID]
                               ,[ItemName]
                               ,[ItemValue]
                               ,[ItemValueType]
                               ,[ItemOrder]
                               ,[Enable]
                               ,[Creator]
                               ,[CreateDate]                               
                               ,[Comment])
                         VALUES
                               (@BaseTypeID
                               ,@ItemName
                               ,@ItemValue
                               ,@ItemValueType
                               ,@ItemOrder
                               ,@Enable
                               ,@Creator
                               ,@CreateDate                              
                               ,@Comment)";
            if (item == null || item.ItemParent == null)
                return new Messaging<int> { retType = 1, Message = "TypeItem为空", Value = 0 };

            var insertCommand = _database.GetSqlStringCommand(InsertSql);


            _database.AddInParameter(insertCommand, "@BaseTypeID", DbType.Int32, item.ItemParent.BaseTypeID);
            _database.AddInParameter(insertCommand, "@ItemName", DbType.String, item.ItemName);
            _database.AddInParameter(insertCommand, "@ItemValue", DbType.String, item.ItemValue);
            _database.AddInParameter(insertCommand, "@ItemValueType", DbType.String, item.ItemValueType);
            if (item.ItemOrder == 0)
            {
                object order = _database.ExecuteScalar(CommandType.Text, "Select max(ItemOrder) from [CMS_CSCL_BaseTypeItem] WHERE BaseTypeID=" + item.ItemParent.BaseTypeID.ToString());

                if (order == null || order == DBNull.Value)
                    item.ItemOrder = 10;
                else
                    item.ItemOrder = Int32.Parse(order.ToString()) + 10;

                _database.AddInParameter(insertCommand, "@ItemOrder", DbType.Int32, item.ItemOrder);
            }
            _database.AddInParameter(insertCommand, "@Enable", DbType.Boolean, true);
            _database.AddInParameter(insertCommand, "@Creator", DbType.String, item.Creator);
            _database.AddInParameter(insertCommand, "@CreateDate", DbType.DateTime, item.CreateDate);
            _database.AddInParameter(insertCommand, "@Comment", DbType.String, item.Comment);


            Messaging<int> mes = new Messaging<int>();

            try
            {
                mes.Value = _database.ExecuteNonQuery(insertCommand);
                mes.retType = 100;
                mes.Message = String.Format("插入数据成功, {0} 行受影响", mes.Value);
            }
            catch (Exception ex)
            {
                mes.retType = 2;
                mes.Message = "插入数据失败, Ex:" + ex.Message;
            }
            return mes;
        }

        //更新Update
        public Messaging<int> UpdateItemType(TypeItem item)
        {
            if (item == null || item.ItemParent == null)
                return new Messaging<int> { Message = "传入数据data为空", retType = 1, Value = 0 };

            string updateSql = @"UPDATE [CMS_CSCL_BaseTypeItem]
                                   SET [ItemName] = @ItemName
                                      ,[ItemValue] = @ItemValue
                                      ,[ItemValueType] = @ItemValueType
                                      ,[ItemOrder] = @ItemOrder
                                      ,[Enable] = @Enable     
                                      ,[Modifier] = @Modifier
                                      ,[ModifyDate] = @ModifyDate
                                      ,[Comment] = @Comment
                                 WHERE  BaseTypeID=@BaseTypeID AND [version]=@version And BaseTypeItemID=@BaseTypeItemID";

            var updateCommand = _database.GetSqlStringCommand(updateSql);

            _database.AddInParameter(updateCommand, "@ItemName", DbType.String, item.ItemName);
            _database.AddInParameter(updateCommand, "@ItemValue", DbType.Int32, item.ItemValue);
            _database.AddInParameter(updateCommand, "@ItemValueType", DbType.String, item.ItemValueType);
            _database.AddInParameter(updateCommand, "@ItemOrder", DbType.Int32, item.ItemOrder);
            _database.AddInParameter(updateCommand, "@Enable", DbType.Boolean, item.Enable);
            _database.AddInParameter(updateCommand, "@Modifier", DbType.String, item.Modifier);
            _database.AddInParameter(updateCommand, "@ModifyDate", DbType.DateTime, item.ModifyDate);
            _database.AddInParameter(updateCommand, "@Comment", DbType.String, item.Comment);
            _database.AddInParameter(updateCommand, "@BaseTypeID", DbType.Int32, item.ItemParent.BaseTypeID);
            _database.AddInParameter(updateCommand, "@version", DbType.Int64, item.version);
            _database.AddInParameter(updateCommand, "@BaseTypeItemID", DbType.Int32, item.BaseTypeItemID);

            Messaging<int> mes = new Messaging<int>();
            try
            {
                mes.Value = _database.ExecuteNonQuery(updateCommand);

                mes.retType = 100;
                mes.Message = String.Format("插入数据成功, {0} 行受影响", mes.Value);
                if (mes.Value == 0)
                {
                    mes.Message = "插入数据失败, 用户数据已被其它用户修改。请返回重新修改!";
                    mes.retType = 10;
                }

            }
            catch (Exception ex)
            {
                mes.retType = 2;
                mes.Message = "插入数据失败.Ex message:" + ex.Message;
            }
            return mes;
        }


        public Messaging<int> DeletData(TypeItem item)
        {
            if (item == null)
                return new Models.Messaging<int> { retType = 1, Message = "删除数据不存在", Value = 0 };

            string deleteSql = @"DELETE FROM [CMS_CSCL_BaseTypeItem] WHERE BaseTypeItemID=@BaseTypeItemID";

            var deleteCommand = _database.GetSqlStringCommand(deleteSql);

            _database.AddInParameter(deleteCommand, "@BaseTypeItemID", DbType.String, item.BaseTypeItemID);


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


        #region 查询Query
        String SelectSql = @"SELECT  A.[BaseTypeItemID] ,
                                    CMS_CSCL_BaseType.[BaseTypeID] ,
                                    CMS_CSCL_BaseType.[BaseTypeName] ,
                                    A.[ItemName] ,
                                    A.[ItemValue] ,
                                    A.[ItemValueType] ,
                                    A.[ItemOrder] ,
                                    A.[Enable] ,
                                    A.[Creator] ,
                                    A.[CreateDate] ,
                                    A.[Modifier] ,
                                    A.[ModifyDate] ,
                                    A.[Comment],
                                    cast(A.[version] as bigint) as [version]                                   
                            FROM    [CMS_CSCL_BaseTypeItem] A
                                     JOIN [CMS_CSCL_BaseType] ON A.[BaseTypeID] = CMS_CSCL_BaseType.[BaseTypeID]";


        public IRowMapper<TypeItem> Mapor()
        {
            return MapBuilder<TypeItem>.MapAllProperties()
                .Map(p => p.BaseTypeItemID).ToColumn("BaseTypeItemID")
                .Map(p => p.ItemName).ToColumn("ItemName")
                .Map(p => p.ItemOrder).ToColumn("ItemOrder")
                .Map(p => p.ItemValue).ToColumn("ItemValue")
                .Map(p => p.ItemValueType).ToColumn("ItemValueType")
                .Map(p => p.Enable).ToColumn("Enable")
                .Map(p => p.Creator).ToColumn("Creator")
                .Map(p => p.CreateDate).ToColumn("CreateDate")
                .Map(p => p.Modifier).ToColumn("Modifier")
                .Map(p => p.version).ToColumn("version")
                .Map(p => p.ItemParent).WithFunc(GenerateObject)
                .Build();
        }

        private BaseType GenerateObject(IDataRecord record)
        {
            BaseType basetype = new BaseType();
            basetype.BaseTypeID = (int)record["BaseTypeID"];
            basetype.BaseTypeName = (string)record["BaseTypeName"];

            return basetype;
        }

        public IEnumerable<TypeItem> GetBaseTypeItem(IParametersSpecification spec)
        {
            var sql = base.GetParaSql(SelectSql, spec);
            //string sql = SelectSql;
            //if (spec != null)
            //{
            //    string parastring = spec.GetSpecValue();
            //    if (!String.IsNullOrEmpty(parastring))
            //        sql += " Where " + parastring;
            //}
            //string sql;
            //if (spec == null || String.IsNullOrEmpty(spec.GetSpecValue()))
            //    sql = SelectSql;
            //else sql = SelectSql + " Where " + spec.GetSpecValue();

            sql += " Order by BaseTypeID, ItemOrder";

            return _database.CreateSqlStringAccessor(sql, Mapor()).Execute();
        }

        #endregion

        public int GetPagedDataCount(IParametersSpecification paraSpec)
        {
            var sql = GetCountSql(GetParaSql(SelectSql, paraSpec));

            return (int)_database.ExecuteScalar(CommandType.Text, sql);
        }

        public IEnumerable<TypeItem> GetPagedData(IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            var sql = GetParaSql(SelectSql, paraSpec);
            sql = GetPageSql(sql, pageIndex, pageSize);

            sql = sql.Replace("@order", "A.[ItemOrder]");

            return _database.CreateSqlStringAccessor(sql, Mapor()).Execute(); 
        }
    }
}
