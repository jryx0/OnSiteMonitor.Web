using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WFM.JW.HB.Models;

namespace WFM.JW.HB.Services
{
    public class WFMUploadTaskServices : ISerivces<Models.WFMUploadTask>
    {
        Messaging<int> ISerivces<WFMUploadTask>.DeleteData(WFMUploadTask data)
        {
            throw new NotImplementedException();
        }

        public WFMUploadTask GetDatabyGuid(string Guid)
        {
            var list = GetAllData(new Models.CriteriaSpecification("TaskGuid", Models.CriteriaOperator.Equal, Guid));
            if (list == null || list.ToList().Count == 0) return null;
            var t = list.First();
            return t;
        }

        public IEnumerable<WFMUploadTask> GetAllData()
        {
            return GetAllData(new CriteriaSpecification("1", CriteriaOperator.Equal, 1));
        }

        public IEnumerable<WFMUploadTask> GetAllData(IParametersSpecification paraSpec)
        {
            Repository.EntLib.WFMUploadTaskRepository utr = new Repository.EntLib.WFMUploadTaskRepository();
            return utr.GetAllData(paraSpec);
        }

        public IEnumerable<WFMUploadTask> GetPagedData(IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            Repository.EntLib.WFMUploadTaskRepository utr = new Repository.EntLib.WFMUploadTaskRepository();
            return utr.GetPagedData(paraSpec, pageIndex, pageSize);
        }

        public int GetPagedDataCount(IParametersSpecification paraSpec)
        {
            Repository.EntLib.WFMUploadTaskRepository utr = new Repository.EntLib.WFMUploadTaskRepository();
            return utr.GetPagedDataCount(paraSpec);
        }

        public Messaging<int> InsertData(WFMUploadTask data)
        {
            Repository.EntLib.WFMUploadTaskRepository utr = new Repository.EntLib.WFMUploadTaskRepository();
            return utr.InsertData(data);
        }

        public Messaging<int> UpdateData(WFMUploadTask data)
        {

            Repository.EntLib.WFMUploadTaskRepository utr = new Repository.EntLib.WFMUploadTaskRepository();
            return utr.UpdateData(data);
        }
        /// <summary>
        //    Status = 0  正在上传
        //    Status = 1-9 上传失败
        //    Status = 10 && TStatus = 0 上传成功
        //    Status = 10 && TStatus = 1 准备导入默认文件 -> Status = 15 && TStatus = 1  正在导入 -> Status = 20 && TStatus = 1 文件导入成功
        //                                                                                    -> Status = 16  &&TStatus = 1 导入失败
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Messaging<int> SetDefaultData(WFMUploadTask value)
        {
            Messaging<int> mes = new Messaging<int>();
            mes.retType = 10;
            mes.Message = "没有发现文件！";

            var tList = GetAllData();
            if (tList == null || tList.Count() == 0)
                return mes;

            var t = tList.ToList().Find(x => (x.TaskGuid == value.TaskGuid && x.Version == value.Version &&
                                              x.TStatus != value.TStatus &&  //值未改变
                                              x.Status >= 10));  //非正在处理中
            if (t == null)
            {
                mes.retType = 1;
                mes.Message = "数据已经被修改或数据正在导入数据，请稍后再操作！";
                return mes;
            }

            if (t.TStatus == 0) //from 0 -> 1
            {
                var preDefault = tList.ToList().Find(x => (x.RegionCode == t.RegionCode && x.TStatus == 1));
                if (preDefault != null)
                {
                    preDefault.TStatus = 0;
                    preDefault.Status = 10;
                    preDefault.OpTime = System.DateTime.Now;
                    preDefault.ErrorMessage = "用户" + t.UserName.Trim() + "修改为非上报数据";
                    mes = UpdateData(preDefault);
                    if (mes.retType != 100)
                        return mes;
                }
            }

            t.OpTime = System.DateTime.Now;
            t.ErrorMessage = "用户" + t.UserName.Trim() + "修改";
            t.Status = 10;
            t.TStatus = value.TStatus;

            mes = UpdateData(t);
            return mes;
        }
        
        public Messaging<int> SetDefaultData(String UserName, String dbInfo)
        {
            Messaging<int> mes = new Messaging<int>();
            mes.retType = 10;
            mes.Message = "没有发现文件！";

            WFMUploadTaskServices wts = new WFMUploadTaskServices();
            IParametersSpecification spec = new CriteriaSpecification("a.UserName", CriteriaOperator.Equal, UserName);
            //spec = spec.And(new CriteriaSpecification("a.FileName", CriteriaOperator.Equal, dbInfo));
            //spec = spec.And(new CriteriaSpecification("a.status", CriteriaOperator.GreaterThanOrEqual, 10));

            var tList = GetAllData(spec);
            if (tList == null || tList.Count() == 0)
                return mes;


            var t = tList.ToList().Find(x => (x.FileName == dbInfo) && (x.Status >= 10));
            if (t == null)
                return mes;

             
            foreach (var T in tList)
            {
                if (T.TStatus == 0)
                    continue;

                T.TStatus = 0;
                T.Status = 10;
                T.OpTime = System.DateTime.Now;
                T.ErrorMessage = "用户" + T.UserName.Trim() + "修改为非上报数据";
                mes = UpdateData(T);
            }

            if(t.TStatus == 0)
            { //1->

                t.OpTime = System.DateTime.Now;
                t.ErrorMessage = "用户" + t.UserName.Trim() + "修改";
                t.Status = 10;
                t.TStatus = 1;

                mes = UpdateData(t);
            }
            
           
            return mes;
        }
    }
}
