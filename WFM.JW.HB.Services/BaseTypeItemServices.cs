using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WFM.JW.HB.Models;
using WFM.JW.HB.Repository.EntLib;

namespace WFM.JW.HB.Services
{
    public class BaseTypeItemServices : ISerivces<Models.TypeItem>
    {
        #region Query
        public IEnumerable<TypeItem> GetAllData()
        {
            return GetAllData(null);
        }

        public IEnumerable<TypeItem> GetAllData(IParametersSpecification paraSpec)
        {
            BaseTypeItemRepository repository = new BaseTypeItemRepository();
            return repository.GetBaseTypeItem(paraSpec);            
        }
        #endregion

        public  Models.Messaging<int> InsertData(TypeItem item)
        {
            BaseTypeItemRepository repository = new BaseTypeItemRepository();

            return repository.InsertItemType(item);
        }

        public Models.Messaging<int> UpdateData(TypeItem data)
        {
            BaseTypeItemRepository repository = new BaseTypeItemRepository();
            return repository.UpdateItemType(data);
        }


        public Messaging<int> DeleteData(TypeItem data)
        {
            BaseTypeItemRepository repository = new BaseTypeItemRepository();
            return repository.DeletData(data);
        }


        public int GetPagedDataCount(IParametersSpecification paraSpec)
        {
            BaseTypeItemRepository repository = new BaseTypeItemRepository();
            return repository.GetPagedDataCount(paraSpec);
        }

       
        public IEnumerable<TypeItem> GetPagedData(IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            BaseTypeItemRepository repository = new BaseTypeItemRepository();
            return repository.GetPagedData(paraSpec, pageIndex, pageSize);
        }
    }
}
