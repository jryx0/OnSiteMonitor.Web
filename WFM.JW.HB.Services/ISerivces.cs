using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using WFM.JW.HB.Models;

namespace WFM.JW.HB.Services
{
    public interface ISerivces<T> where T: new()
    {
        IEnumerable<T> GetAllData();
        IEnumerable<T> GetAllData(IParametersSpecification paraSpec);

        int GetPagedDataCount(IParametersSpecification paraSpec);
        IEnumerable<T> GetPagedData(IParametersSpecification paraSpec, int pageIndex, int pageSize);
        


        Messaging<int> InsertData(T data);
        Messaging<int> UpdateData(T data);

        Messaging<int> DeleteData(T data);
    }
}
