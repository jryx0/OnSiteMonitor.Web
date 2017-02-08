using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WFM.JW.HB.Services
{
    public class ContractServices : ISerivces<Models.ContractInfo>
    {
        public IEnumerable<Models.ContractInfo> GetAllData()
        {
            return GetAllData(null);
        }

        public IEnumerable<Models.ContractInfo> GetAllData(Models.IParametersSpecification paraSpec)
        {
            Repository.EntLib.ContractInfoRepository repository = new Repository.EntLib.ContractInfoRepository();
            return repository.GetAllData(paraSpec);
        }

        public Models.Messaging<int> InsertData(Models.ContractInfo data)
        {
            Repository.EntLib.ContractInfoRepository repository = new Repository.EntLib.ContractInfoRepository();
            return repository.InsetData(data);
        }

        public Models.Messaging<int> UpdateData(Models.ContractInfo data)
        {
            Repository.EntLib.ContractInfoRepository repository = new Repository.EntLib.ContractInfoRepository();
            return repository.UpdateData(data);
        }

        public Models.Messaging<int> DeleteData(Models.ContractInfo data)
        {
            Repository.EntLib.ContractInfoRepository repository = new Repository.EntLib.ContractInfoRepository();
            return repository.DeletData(data);
        }




        public int GetPagedDataCount(Models.IParametersSpecification paraSpec)
        {
            Repository.EntLib.ContractInfoRepository repository = new Repository.EntLib.ContractInfoRepository();
            return repository.GetPagedDataCount(paraSpec); 
        }

        public System.Data.DataSet GetGroupData(Models.IParametersSpecification paraSpec, string groupField)
        {
            Repository.EntLib.ContractInfoRepository repository = new Repository.EntLib.ContractInfoRepository();
            return repository.GetGroupData(paraSpec, groupField);
        }
        


        public IEnumerable<Models.ContractInfo> GetPagedData(Models.IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            Repository.EntLib.ContractInfoRepository repository = new Repository.EntLib.ContractInfoRepository();
            return repository.GetPagedData(paraSpec, pageIndex, pageSize);
        }
    }
}
