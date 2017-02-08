using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using WFM.JW.HB.Models;
using WFM.JW.HB.Repository.EntLib;

namespace WFM.JW.HB.Services
{
    public class BaseTypeServices : ISerivces<BaseType>
    {
        
        public BaseTypeServices()
        {
            
        }

        #region Query
        public DataSet GetDataSet()
        {
            BaseTypeRepository repository = new BaseTypeRepository();
            return repository.GetBaseTypeSet();
        }

        public IEnumerable<BaseType> GetAllData(IParametersSpecification paraSpec)
        {
            BaseTypeRepository repository = new BaseTypeRepository();
            return repository.GetBaseType(paraSpec);
        }

        public IEnumerable<BaseType> GetAllData()
        {
            return GetAllData(null);
        }
        #endregion


        #region Insert
        public Messaging<int> InsertData(BaseType data)
        {
            if (data == null)
            {
                return new Messaging<int> { Message ="传入数据data为空", retType =1, Value = 0 };
            }

            BaseTypeRepository repository = new BaseTypeRepository();
            return repository.InsertBaseType(data);
        }
        #endregion

        #region UpdateData
        public Messaging<int> UpdateData(BaseType data)
        {
            if (data == null)
                return new Messaging<int> { Message = "传入数据data为空", retType = 1, Value = 0 };

            BaseTypeRepository repository = new BaseTypeRepository();
            return repository.UpdateBaseType(data);
        }
        #endregion

        public Messaging<int> DeleteData(BaseType data)
        {
            BaseTypeRepository repository = new BaseTypeRepository();
            return repository.DeletData(data);
        }


        public int GetPagedDataCount(IParametersSpecification paraSpec)
        {
            BaseTypeRepository repository = new BaseTypeRepository();
            return repository.GetPagedDataCount(paraSpec);
        }

       

        public IEnumerable<BaseType> GetPagedData(IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            BaseTypeRepository repository = new BaseTypeRepository();
            return repository.GetPagedData(paraSpec, pageIndex, pageSize);
        }
    }
}
