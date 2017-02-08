using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using WFM.JW.HB.Models;

namespace WFM.JW.HB.Repository.EntLib
{

    public class WFMUploadTaskRepository : BaseRepositorySql06<WFMUploadTask>
    {
        #region 查询
        string SelectSql = @"
                                    SELECT cast( a.TaskGuid  AS VARCHAR(36))  AS TaskGuid,
                                           c.RegionName AS ParentName,
                                           cast( c.RegionGuid AS VARCHAR(36)) as ParentGuid,
                                           d.RegionName AS RegionName,
                                           cast( d.RegionGuid AS VARCHAR(36)) as RegionGuid,
                                           a.UserName AS UserName,
                                           a.Status AS DataStatus,
                                           a.TStatus AS IsDefault,
                                           a.FilePath AS FilePath,
                                           a.FileName AS FileName,
                                           a.CreateTime AS CreateTime,
                                           a.OpTime AS OpTime,
                                           a.ErrorMessage AS ErrorMsg,
                                           CAST (a.Version AS BIGINT) Version ,
            	                            a.TaskName as Taskname,
            	                            a.TaskComment as Comment,
                                            d.RegionCode as RegionCode,
                                            a.UserRegionPath as UserRegionPath,
                                            a.IsRead as IsRead
                                      FROM dbo.WFM_UploadTask a
                                           LEFT JOIN dbo.WFM_NewRegionUser b ON a.UserName = b.UserName
                                           LEFT JOIN HB_NewRegion c ON b.ParentGUID = c.RegionGUID
                                           LEFT JOIN HB_NewRegion d ON b.RegionGUID = d.RegionGUID";
        public override IRowMapper<WFMUploadTask> Mapor()
        {
            return MapBuilder<WFMUploadTask>.MapAllProperties()
                                .Map(p => p.TaskGuid).ToColumn("TaskGuid")
                                .Map(p => p.ParentName).ToColumn("ParentName")
                                .Map(p => p.RegionName).ToColumn("RegionName")
                                .Map(p => p.UserName).ToColumn("UserName")
                                .Map(p => p.Status).ToColumn("DataStatus")
                                .Map(p => p.TStatus).ToColumn("IsDefault")
                                .Map(p => p.FilePath).ToColumn("FilePath")
                                .Map(p => p.FileName).ToColumn("FileName")
                                .Map(p => p.CreateTime).ToColumn("CreateTime")
                                .Map(p => p.OpTime).ToColumn("OpTime")
                                .Map(p => p.ErrorMessage).ToColumn("ErrorMsg")
                                .Map(p => p.Version).ToColumn("Version")
                                .Map(p => p.ClientTaskName).ToColumn("Taskname")
                                .Map(p => p.ClientTaskComment).ToColumn("Comment")
                                .Map(p => p.UserRegionPath).ToColumn("UserRegionPath")
                                .Map(p => p.RegionCode).ToColumn("RegionCode")
                                .Map(p => p.TotalClues).ToColumn("DataStatus")
                                 .Map(p => p.RegionCode).ToColumn("RegionCode")
                                .Map(p => p.ParentGuid).ToColumn("ParentGuid")
                                .Map(p => p.IsRead).ToColumn("IsRead")
                                .Build();
        }

       

        public IEnumerable<Models.WFMUploadTask> GetAllData(Models.IParametersSpecification paraSpec)
        {
            return base.FindData(SelectSql, paraSpec, " ParentName, RegionName, TStatus desc,CreateTime");
        }

        public int GetPagedDataCount(Models.IParametersSpecification paraSpec)
        {
            return base.GetPagedDataCount(SelectSql, paraSpec);
        }

        public IEnumerable<Models.WFMUploadTask> GetPagedData(Models.IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            return base.GetPagedData(SelectSql, " TStatus desc, CreateTime desc", paraSpec, pageIndex, pageSize);
        }

        #endregion

 

        #region 新增
        public Models.Messaging<int> InsertData(Models.WFMUploadTask data)
        {
            String InsertSql = @"insert into [dbo].[WFM_UploadTask](TaskGuid, UserRegionPath, UserName, FilePath, FileName, Status, CreateTime, OpTime)
                                    values(@TaskGuid, @UserRegionPath, @UserName, @FilePath, @FileName, @Status, @CreateTime, @OpTime)";

            var InsertCommand = _database.GetSqlStringCommand(InsertSql);

            _database.AddInParameter(InsertCommand, "@TaskGuid", System.Data.DbType.String, data.TaskGuid);
            _database.AddInParameter(InsertCommand, "@UserRegionPath", System.Data.DbType.String, data.UserRegionPath);
            _database.AddInParameter(InsertCommand, "@UserName", System.Data.DbType.String, data.UserName);
            _database.AddInParameter(InsertCommand, "@FilePath", System.Data.DbType.String, data.FilePath);
            _database.AddInParameter(InsertCommand, "@FileName", System.Data.DbType.String, data.FileName);
            _database.AddInParameter(InsertCommand, "@Status", System.Data.DbType.Int32, data.Status);
            _database.AddInParameter(InsertCommand, "@CreateTime", System.Data.DbType.DateTime, data.CreateTime);
            _database.AddInParameter(InsertCommand, "@OpTime", System.Data.DbType.DateTime, data.OpTime);


            return base.InsertData(InsertCommand);
        }
        #endregion

        #region 修改
        public Models.Messaging<int> UpdateData(Models.WFMUploadTask data)
        {
            String UpdateSql = @"update [dbo].[WFM_UploadTask] set 
                                           Status = @Status, 
                                           OpTime = @OpTime, 
                                           ErrorMessage=@ErrorMsg, 
                                           TaskName = @TaskName, 
                                           TaskComment=@TaskComment,
                                           TStatus = @TStatus  
                                         where TaskGuid=@TaskGuid "; // and Version = @Version";

            var UpdateCommand = _database.GetSqlStringCommand(UpdateSql);

            _database.AddInParameter(UpdateCommand, "@TaskGuid", System.Data.DbType.String, data.TaskGuid);
            _database.AddInParameter(UpdateCommand, "@Status", System.Data.DbType.Int32, data.Status);
            _database.AddInParameter(UpdateCommand, "@OpTime", System.Data.DbType.DateTime, data.OpTime);
            _database.AddInParameter(UpdateCommand, "@ErrorMsg", System.Data.DbType.String, data.ErrorMessage);
            _database.AddInParameter(UpdateCommand, "@TaskName", System.Data.DbType.String, data.ClientTaskName);
            _database.AddInParameter(UpdateCommand, "@TaskComment", System.Data.DbType.String, data.ClientTaskComment);
            _database.AddInParameter(UpdateCommand, "@TStatus", System.Data.DbType.String, data.TStatus);
            _database.AddInParameter(UpdateCommand, "@Version", System.Data.DbType.Int64, data.Version);


            return base.Update(UpdateCommand);
        }

        public void ResetTStatus(string RegionName, int value)
        {
            String UpdateSql = @"update [dbo].[WFM_UploadTask] 
                                    set TStatus = @TStatus 
                                 where UserRegionPath = @UserRegionPath
                                   and Status = 10";

            var UpdateCommand = _database.GetSqlStringCommand(UpdateSql);
            _database.AddInParameter(UpdateCommand, "@UserRegionPath", System.Data.DbType.String, RegionName);
            _database.AddInParameter(UpdateCommand, "@TStatus", System.Data.DbType.String, value);
            base.Update(UpdateCommand);
        }
        #endregion
    }

    //public class WFMUploadTaskRepository : BaseRepositorySql05 
    //{
    //    /*
    //    Status = 0  正在上传
    //    Status = 1-9 上传失败
    //    Status = 10 && TStatus = 0 上传成功

    //    Status = 10 && TStatus = 1 准备导入默认文件 -> Status = 20 && TStatus = 1  文件导入成功
    //    Status = 20 && TStatus = 0 删除默认数据 -> Status = 10 && TStatus = 0
    //    */


    //    #region 查询
    //    string SelectSql = @"
    //                            SELECT cast( a.TaskGuid  AS VARCHAR(36))  AS TaskGuid,
    //                                   c.RegionName AS ParentName,
    //                                   d.RegionName AS RegionName,
    //                                   a.UserName AS UserName,
    //                                   a.Status AS DataStatus,
    //                                   a.TStatus AS IsDefault,
    //                                   a.FilePath AS FilePath,
    //                                   a.FileName AS FileName,
    //                                   a.CreateTime AS CreateTime,
    //                                   a.OpTime AS OpTime,
    //                                   a.ErrorMessage AS ErrorMsg,
    //                                   CAST (a.Version AS BIGINT) Version ,
    //    	                            a.TaskName as Taskname,
    //    	                            a.TaskComment as Comment
    //                              FROM dbo.WFM_UploadTask a
    //                                   LEFT JOIN dbo.WFM_NewRegionUser b ON a.UserName = b.UserName
    //                                   LEFT JOIN HB_NewRegion c ON b.ParentGUID = c.RegionGUID
    //                                   LEFT JOIN HB_NewRegion d ON b.RegionGUID = d.RegionGUID";
    //    //string SelectSql = @"select  *    from  WFM_UploadTask";
    //    public   IRowMapper<WFMUploadTask> Mapor()
    //    {
    //        return MapBuilder<WFMUploadTask>.MapAllProperties()
    //              .Map(p => p.test).ToColumn("FilePath")
    //              .Map(p => p.TaskGuid).ToColumn("FilePath")

    //              .Build();
    //    }

    //    public IEnumerable<WFMUploadTask> GetAllData(IParametersSpecification paraSpec)
    //    {
    //        return  FindRegion( paraSpec );
    //    }



    //    public int GetPagedDataCount(Models.IParametersSpecification paraSpec)
    //    {
    //        return base.GetPagedDataCount(paraSpec, SelectSql);
    //    }


    //    public IEnumerable<Models.WFMUploadTask> GetPagedData(Models.IParametersSpecification paraSpec, int pageIndex, int pageSize)
    //    {
    //        var sql = GetParaSql(SelectSql, paraSpec);
    //        sql = GetPageSql(sql, pageIndex, pageSize);

    //        sql = sql.Replace("@order", " FilePath");

    //        var ds = _database.ExecuteDataSet(System.Data.CommandType.Text, sql);



    //        return null;
    //    }

    //    public IEnumerable<Models.WFMUploadTask> FindRegion(Models.IParametersSpecification paraSpec)
    //    {
    //        string sql = SelectSql;
    //        if (paraSpec != null)
    //        {
    //            string parastring = paraSpec.GetSpecValue();
    //            if (!String.IsNullOrEmpty(parastring))
    //                sql += " Where " + parastring;
    //        }

    //        sql += " order by  FilePath";

    //        return _database.CreateSqlStringAccessor(sql, Mapor()).Execute();
    //    }


    //    #endregion

    //    //#region 新增
    //    //public Models.Messaging<int> InsertData(Models.WFMUploadTask data)
    //    //{
    //    //    String InsertSql = @"insert into [dbo].[WFM_UploadTask](TaskGuid, UserRegionPath, UserName, FilePath, FileName, Status, CreateTime, OpTime)
    //    //                            values(@TaskGuid, @UserRegionPath, @UserName, @FilePath, @FileName, @Status, @CreateTime, @OpTime)";

    //    //    var InsertCommand = _database.GetSqlStringCommand(InsertSql);

    //    //    _database.AddInParameter(InsertCommand, "@TaskGuid", System.Data.DbType.String, data.TaskGuid);
    //    //    //_database.AddInParameter(InsertCommand, "@UserRegionPath", System.Data.DbType.String, data.UserRegionPath);
    //    //    //_database.AddInParameter(InsertCommand, "@UserName", System.Data.DbType.String, data.UserName);
    //    //    //_database.AddInParameter(InsertCommand, "@FilePath", System.Data.DbType.String, data.FilePath);
    //    //    //_database.AddInParameter(InsertCommand, "@FileName", System.Data.DbType.String, data.FileName);
    //    //    //_database.AddInParameter(InsertCommand, "@Status", System.Data.DbType.Int32, data.Status);
    //    //    //_database.AddInParameter(InsertCommand, "@CreateTime", System.Data.DbType.DateTime, data.CreateTime);
    //    //    //_database.AddInParameter(InsertCommand, "@OpTime", System.Data.DbType.DateTime, data.OpTime);


    //    //    return base.InsertData(InsertCommand);
    //    //}
    //    //#endregion

    //    //#region 修改
    //    //public Models.Messaging<int> UpdateData(Models.WFMUploadTask data)
    //    //{
    //    //    String UpdateSql = @"update [dbo].[WFM_UploadTask] set 
    //    //                                   Status = @Status, 
    //    //                                   OpTime = @OpTime, 
    //    //                                   ErrorMessage=@ErrorMsg, 
    //    //                                   TaskName = @TaskName, 
    //    //                                   TaskComment=@TaskComment  
    //    //                                 where TaskGuid=@TaskGuid";

    //    //    var UpdateCommand = _database.GetSqlStringCommand(UpdateSql);

    //    //    _database.AddInParameter(UpdateCommand, "@TaskGuid", System.Data.DbType.String, data.TaskGuid);
    //    //    //_database.AddInParameter(UpdateCommand, "@Status", System.Data.DbType.Int32, data.Status);
    //    //    //_database.AddInParameter(UpdateCommand, "@OpTime", System.Data.DbType.DateTime, data.OpTime);
    //    //    //_database.AddInParameter(UpdateCommand, "@ErrorMsg", System.Data.DbType.String, data.ErrorMsg);
    //    //    //_database.AddInParameter(UpdateCommand, "@TaskName", System.Data.DbType.String, data.ClientTaskName);
    //    //    //_database.AddInParameter(UpdateCommand, "@TaskComment", System.Data.DbType.String, data.ClientTaskComment);


    //    //    return base.Update(UpdateCommand);
    //    //}
    //    //#endregion

    //}
}
