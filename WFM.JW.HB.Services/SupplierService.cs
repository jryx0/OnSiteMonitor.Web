using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using WFM.JW.HB.Repository.EntLib;

namespace WFM.JW.HB.Services
{
    public class SupplierService : ISerivces<Models.SupplierInfo>
    {

        public IEnumerable<Models.SupplierInfo> GetAllData()
        {
            return (null);
        }

        public IEnumerable<Models.SupplierInfo> GetAllData(Models.IParametersSpecification paraSpec)
        {
            SupplierRepository repository = new SupplierRepository();
            return repository.FindSupplier(paraSpec);
        }




        public Models.Messaging<int> InsertData(Models.SupplierInfo data)
        {
            SupplierRepository repository = new SupplierRepository();
            return repository.InsetData(data);
        }

        public Models.Messaging<int> UpdateData(Models.SupplierInfo data)
        {
            SupplierRepository repository = new SupplierRepository();
            return repository.UpdateData(data);
        }


        public Models.Messaging<int> DeleteData(Models.SupplierInfo data)
        {
            if (data == null)
                return new Models.Messaging<int> { retType = 1, Message = "删除数据不存在", Value = 0 };
           
            SupplierRepository repository = new SupplierRepository();
            return repository.DeletData(data);
        }

        

        public int GetPagedDataCount(Models.IParametersSpecification paraSpec)
        {

            SupplierRepository repository = new SupplierRepository();
            return repository.GetPagedDataCount(paraSpec);
        }

        public IEnumerable<Models.SupplierInfo> GetPagedData(Models.IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            SupplierRepository repository = new SupplierRepository();
            return repository.GetPagedData(paraSpec, pageIndex, pageSize);
        }
    }
}
