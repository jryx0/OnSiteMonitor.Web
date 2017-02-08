using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

using System.Text.RegularExpressions;
using WFM.JW.HB.Models;
using System.Data;

namespace WFM.JW.HB.Repository.EntLib
{
    public class BaseRepositorySql05
    {
        protected Database _database;
        public BaseRepositorySql05()
        {
            _database = EnterpriseLibraryContainer.Current.GetInstance<Database>("ApplicationServices");
        }
        
        protected string GetPageSql(string orginalSql, int pageIndex, int pageSize)
        {
            //Regex.Replace(字符串,要替换的子串,替换子串的字符,RegexOptions.IgnoreCase)
            string pagesql = Regex.Replace(orginalSql, "Select", "SELECT  ROW_NUMBER() OVER ( ORDER BY @order ) AS Row ,", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                
           //orginalSql.Replace("Select", "SELECT  ROW_NUMBER() OVER ( ORDER BY A.ContractName DESC ) AS Row ,");
            pagesql = "WITH   temptbl AS(" + pagesql + 
                                            @") SELECT  *  
                                                FROM    temptbl   
                                                WHERE   Row  BETWEEN ( @pageindex - 1 ) * @pagesize + 1  AND 
                                                                     ( @pageindex - 1 ) * @pagesize + @pagesize";
           
            pagesql = pagesql.Replace("@pageindex", pageIndex.ToString());
            pagesql = pagesql.Replace("@pagesize", pageSize.ToString());
                        
            return pagesql;
        }

        protected string GetCountSql(string orginalSql)
        {
            string countsql = Regex.Replace(orginalSql, @"select[\s|\S]*?from", "Select count(*) from ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            //var b = Regex.IsMatch(orginalSql, @"select[\s|\S]*?from", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            return countsql;
        }
        /**
         * groupField = "count(*), sum(), sum() ..."
         * 
         * */
        protected string GetGroupSql(string orginalSql, string groupField)
        {
           
            string countsql = Regex.Replace(orginalSql, @"select[\s|\S]*?from", "Select "+ groupField + "   from ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            //var b = Regex.IsMatch(orginalSql, @"select[\s|\S]*?from", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            return countsql;
        }

        protected string GetParaSql(string orginalSql, Models.IParametersSpecification paraSpec)
        {
            string parasql = orginalSql;
            if (paraSpec != null)
            {
                string parastring = paraSpec.GetSpecValue();
                if (!String.IsNullOrEmpty(parastring))
                    parasql += " Where " + parastring;
            }

            return parasql;
        }

        protected int GetPagedDataCount(Models.IParametersSpecification paraSpec, string SelectSql)
        {
            var sql = GetCountSql(GetParaSql(SelectSql, paraSpec));


            return (int)_database.ExecuteScalar(CommandType.Text, sql);
        }
        protected Messaging<int> Update(System.Data.Common.DbCommand updateCommander)
        {
            Messaging<int> mes = new Messaging<int>();
            try
            {
                mes.Value = _database.ExecuteNonQuery(updateCommander);
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


        public static DataSet ExecuteDataSet(string sql)
        {
            Database DB = EnterpriseLibraryContainer.Current.GetInstance<Database>("ApplicationServices");
            return DB.ExecuteDataSet(sql);
        }

        public static int ExecuteNonQuery(string sql)
        {
            Database DB = EnterpriseLibraryContainer.Current.GetInstance<Database>("ApplicationServices");
            return DB.ExecuteNonQuery(sql);
        }

        public static object ExecuteScalar(string sql)
        {
            Database DB = EnterpriseLibraryContainer.Current.GetInstance<Database>("ApplicationServices");
            return DB.ExecuteScalar(sql);
        }
    }

    public abstract class BaseRepositorySql06<T>
    {
        protected Database _database;

        public abstract IRowMapper<T> Mapor();
       

        public BaseRepositorySql06()
        {
            _database = EnterpriseLibraryContainer.Current.GetInstance<Database>("ApplicationServices");

            
        }




        #region 分页操作
        protected string GetPageSql(string orginalSql, int pageIndex, int pageSize)
        {
            //Regex.Replace(字符串,要替换的子串,替换子串的字符,RegexOptions.IgnoreCase)
            string pagesql = Regex.Replace(orginalSql, "Select", "SELECT  ROW_NUMBER() OVER ( ORDER BY @order ) AS Row ,", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            //orginalSql.Replace("Select", "SELECT  ROW_NUMBER() OVER ( ORDER BY A.ContractName DESC ) AS Row ,");
            pagesql = "WITH   temptbl AS(" + pagesql +
                                            @") SELECT  *  
                                                FROM    temptbl   
                                                WHERE   Row  BETWEEN ( @pageindex - 1 ) * @pagesize + 1  AND 
                                                                     ( @pageindex - 1 ) * @pagesize + @pagesize";

            pagesql = pagesql.Replace("@pageindex", pageIndex.ToString());
            pagesql = pagesql.Replace("@pagesize", pageSize.ToString());

            return pagesql;
        }

        protected string GetCountSql(string orginalSql)
        {
            string countsql = Regex.Replace(orginalSql, @"select[\s|\S]*?from", "Select count(*) from ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            //var b = Regex.IsMatch(orginalSql, @"select[\s|\S]*?from", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            return countsql;
        }
        /**
         * groupField = "count(*), sum(), sum() ..."
         * 
         * */
        protected string GetGroupSql(string orginalSql, string groupField)
        {

            string countsql = Regex.Replace(orginalSql, @"select[\s|\S]*?from", "Select " + groupField + "   from ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            //var b = Regex.IsMatch(orginalSql, @"select[\s|\S]*?from", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            return countsql;
        }

        protected string GetParaSql(string orginalSql, Models.IParametersSpecification paraSpec)
        {
            string parasql = orginalSql;
            if (paraSpec != null)
            {
                string parastring = paraSpec.GetSpecValue();
                if (!String.IsNullOrEmpty(parastring))
                    parasql += " Where " + parastring;
            }

            return parasql;
        }

        protected int GetPagedDataCount(string SelectSql, Models.IParametersSpecification paraSpec)
        {
            var sql = GetCountSql(GetParaSql(SelectSql, paraSpec));


            return (int)_database.ExecuteScalar(CommandType.Text, sql);
        }

        internal IEnumerable<T> GetPagedData(string SelectSql, string orderby, Models.IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            var sql = GetParaSql(SelectSql, paraSpec);
            sql = GetPageSql(sql, pageIndex, pageSize);

            sql = sql.Replace("@order", " " + orderby);
            return _database.CreateSqlStringAccessor(sql, Mapor()).Execute();
        }

        #endregion

        public virtual string GenerateSql(Models.IParametersSpecification paraSpec, string Groupby , string orderby)
        {
            string clause = " ";
            if (paraSpec != null)
            {
                string parastring = paraSpec.GetSpecValue();
                if (!String.IsNullOrEmpty(parastring))
                    clause += " Where " + parastring;
            }

            if (!String.IsNullOrEmpty(Groupby))
                clause += " group by " + Groupby;

            if (!String.IsNullOrEmpty(orderby))
                clause += " order by " + orderby;

            return clause;
        }

        protected IEnumerable<T> FindData(string sql )
        {
            return _database.CreateSqlStringAccessor(sql  , Mapor()).Execute();
        }


        protected IEnumerable<T> FindData(string sql, Models.IParametersSpecification paraSpec, string orderby)
        {
            if (paraSpec != null)
            {
                string parastring = paraSpec.GetSpecValue();
                if (!String.IsNullOrEmpty(parastring))
                    sql += " Where " + parastring;
            }

            if(orderby.Length != 0)
                sql += " order by " + orderby;

            return _database.CreateSqlStringAccessor(sql, Mapor()).Execute();
        }

        public static DataSet ExecuteDataSet(string sql)
        {
            Database DB = EnterpriseLibraryContainer.Current.GetInstance<Database>("ApplicationServices");
            return DB.ExecuteDataSet(CommandType.Text, sql);
        }

        public int ExecuteNonQuery(string sql)
        {           
            return _database.ExecuteNonQuery(CommandType.Text, sql);
        }

        protected Messaging<int> InsertData(System.Data.Common.DbCommand insertCommand)
        {
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
        protected Messaging<int> Update(System.Data.Common.DbCommand updateCommander)
        {
            Messaging<int> mes = new Messaging<int>();
            try
            {
                mes.Value = _database.ExecuteNonQuery(updateCommander);
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
        protected Messaging<int> ModifyData(System.Data.Common.DbCommand comm)
        {
            Models.Messaging<int> mes = new Models.Messaging<int>();

            try
            {
                mes.Value = _database.ExecuteNonQuery(comm);
                mes.retType = 100;
                mes.Message = String.Format("?成功, {0} 行受影响", mes.Value);

                if (mes.Value == 0)
                {
                    mes.Message = "?失败, 请返回!";
                    mes.retType = 10;
                }
            }
            catch (Exception ex)
            {
                mes.retType = 2;
                mes.Message = "?失败, Ex:" + ex.Message;
            }
            return mes;
        }
    }

}
