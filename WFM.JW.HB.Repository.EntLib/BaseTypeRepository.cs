using System;
using System.Collections.Generic;
using System.Data;

using WFM.JW.HB.Models;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.SqlClient;

namespace WFM.JW.HB.Repository.EntLib
{

    public class BaseTypeRepository:BaseRepositorySql05
    {
        //Database _database;

        public BaseTypeRepository()
        {
           // _database = EnterpriseLibraryContainer.Current.GetInstance<Database>("ApplicationServices");
        }

        #region Select
        public string SelectSql = @"SELECT [BaseTypeID]
                                        ,[BaseTypeName]
                                        ,[TypeOrder]
                                        ,[Comment]
                                        ,[Enable]
                                        ,[Creator]
                                        ,[CreateDate]
                                        ,[Modifier]
                                        ,[ModifyDate]
                                        ,cast([version] as bigint) as [version] From CMS_CSCL_BaseType";

        public IRowMapper<BaseType> Mapor()
        {
            return MapBuilder<BaseType>.MapAllProperties()
                .Map(p => p.BaseTypeID).ToColumn("BaseTypeID")
                .Map(p => p.BaseTypeName).ToColumn("BaseTypeName")
                .Map(p => p.TypeOrder).ToColumn("TypeOrder")
                .Map(p => p.Comment).ToColumn("Comment")
                .Map(p => p.Enable).ToColumn("Enable")
                .Map(p => p.Creator).ToColumn("Creator")
                .Map(p => p.CreateDate).ToColumn("CreateDate")
                .Map(p => p.Modifier).ToColumn("Modifier")
                .Map(p => p.version).ToColumn("version")
                .Build();
        }

        private DataAccessor<BaseType> SqlStringAccessor(string sql)
        {
            return _database.CreateSqlStringAccessor(sql, Mapor());
        }

        public DataSet GetBaseTypeSet()
        {
            var command = _database.GetSqlStringCommand(SelectSql);
            _database.AddInParameter(command, "BaseTypeID", DbType.Int32, 1);

            return _database.ExecuteDataSet(command);
        }

        public IEnumerable<BaseType> GetBaseType(IParametersSpecification paraSpec)
        {
            //string sql;
            //if (paraSpec == null || String.IsNullOrEmpty(paraSpec.GetSpecValue()))
            //    sql = SelectSql;
            //else sql = SelectSql + " Where " + paraSpec.GetSpecValue();
            string sql = SelectSql;
            if (paraSpec != null)
            {
                string parastring = paraSpec.GetSpecValue();
                if (!String.IsNullOrEmpty(parastring))
                    sql += " Where " + parastring;
            }

            sql += " Order by TypeOrder";

            return SqlStringAccessor(sql).Execute();
        }


        public int GetPagedDataCount(IParametersSpecification paraSpec)
        {
            var sql = GetCountSql(GetParaSql(SelectSql, paraSpec));

            return (int)_database.ExecuteScalar(CommandType.Text, sql);
        }



        public IEnumerable<BaseType> GetPagedData(IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            var sql = GetParaSql(SelectSql, paraSpec);
            sql = GetPageSql(sql, pageIndex, pageSize);

            sql = sql.Replace("@order", "[TypeOrder]");

            return _database.CreateSqlStringAccessor(sql, Mapor()).Execute(); 
        }


        #endregion

        #region Insert
        public Models.Messaging<int> InsertBaseType(BaseType basetype)
        {
            if (basetype == null)
                return new Messaging<int> { Message = "传入数据data为空", retType = 1, Value = 0 };
            string InsertSql = @"INSERT INTO [CMS_CSCL_BaseType]
                                   ([BaseTypeName]
                                   ,[TypeOrder]
                                   ,[Comment]
                                   ,[Enable]
                                   ,[Creator]
                                   ,[CreateDate])
                             VALUES
                                   (@BaseTypeName
                                   ,@TypeOrder
                                   ,@Comment
                                   ,@Enable
                                   ,@Creator
                                   ,@CreateDate)";

            var insertCommand = _database.GetSqlStringCommand(InsertSql);
           

            _database.AddInParameter(insertCommand, "@BaseTypeName", DbType.String, basetype.BaseTypeName);

            if (basetype.TypeOrder == 0)
            {
                object order = _database.ExecuteScalar(CommandType.Text, "Select max(TypeOrder) from [CMS_CSCL_BaseType]");
                if (order == null)
                    basetype.TypeOrder = 10;
                else
                    basetype.TypeOrder = Int32.Parse(order.ToString()) + 10;
            }

            _database.AddInParameter(insertCommand, "@TypeOrder", DbType.Int32, basetype.TypeOrder);
            _database.AddInParameter(insertCommand, "@Comment", DbType.String, basetype.Comment);
            _database.AddInParameter(insertCommand, "@Enable", DbType.Boolean, basetype.Enable);
            _database.AddInParameter(insertCommand, "@Creator", DbType.String, basetype.Creator);
            _database.AddInParameter(insertCommand, "@CreateDate", DbType.DateTime, basetype.CreateDate);
        
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
                mes.Message = "插入数据失败.Ex message:" + ex.Message;
            }
            return mes;
        }
        #endregion

        #region Update
        public Models.Messaging<int> UpdateBaseType(BaseType basetype)
        {
            if (basetype == null)
                return new Messaging<int> { Message = "传入数据data为空", retType = 1, Value = 0 };

            string updateSql = @"UPDATE [CMS_CSCL_BaseType]
                                   SET [BaseTypeName] = @BaseTypeName
                                      ,[TypeOrder] = @TypeOrder
                                      ,[Comment] = @Comment
                                      ,[Enable] = @Enable
                                      ,[Modifier] = @Modifier
                                      ,[ModifyDate] = @ModifyDate
                                 WHERE BaseTypeID=@BaseTypeID AND [version]=@version";

            var updateCommand = _database.GetSqlStringCommand(updateSql);

            _database.AddInParameter(updateCommand, "@BaseTypeName", DbType.String, basetype.BaseTypeName);
            _database.AddInParameter(updateCommand, "@TypeOrder", DbType.Int32, basetype.TypeOrder);
            _database.AddInParameter(updateCommand, "@Comment", DbType.String, basetype.Comment);
            _database.AddInParameter(updateCommand, "@Enable", DbType.Boolean, basetype.Enable);
            _database.AddInParameter(updateCommand, "@Modifier", DbType.String, basetype.Modifier);
            _database.AddInParameter(updateCommand, "@ModifyDate", DbType.DateTime, basetype.ModifyDate);
            _database.AddInParameter(updateCommand, "@BaseTypeID", DbType.Int32, basetype.BaseTypeID);
            _database.AddInParameter(updateCommand, "@version", DbType.Int64, basetype.version);

            Messaging<int> mes = new Messaging<int>();
            try
            {
                mes.Value = _database.ExecuteNonQuery(updateCommand);
                mes.retType = 100;
                mes.Message = String.Format("插入数据成功, {0} 行受影响", mes.Value); 

            }
            catch (Exception ex)
            {
                mes.retType = 2;
                mes.Message = "插入数据失败.Ex message:" + ex.Message;
            }
            return mes;


        }
        #endregion


        #region Delete
        public Messaging<int> DeletData(BaseType data)
        {
            if (data == null)
                return new Models.Messaging<int> { retType = 1, Message = "删除数据不存在", Value = 0 };

            string deleteSql = @"DELETE FROM [CMS_CSCL_BaseType] WHERE BaseTypeID=@BaseTypeID";

            var deleteCommand = _database.GetSqlStringCommand(deleteSql);

            _database.AddInParameter(deleteCommand, "@BaseTypeID", DbType.String, data.BaseTypeID);


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

    }
}
