using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WFM.JW.HB.Services
{
    public class SettlementServices : ISerivces<Models.SettlementRecord>
    {
        public IEnumerable<Models.SettlementRecord> GetAllData()
        {
            return GetAllData(null);
        }

        public IEnumerable<Models.SettlementRecord> GetAllData(Models.IParametersSpecification paraSpec)
        {
            Repository.EntLib.SettlementRepository settlement = new Repository.EntLib.SettlementRepository();
            return settlement.GetAllData(paraSpec);
        }

        public Models.Messaging<int> InsertData(Models.SettlementRecord data)
        {
            Repository.EntLib.SettlementRepository settlement = new Repository.EntLib.SettlementRepository();
            return settlement.InsetData(data);
        }

        public Models.Messaging<int> UpdateData(Models.SettlementRecord data)
        {
            Repository.EntLib.SettlementRepository settlement = new Repository.EntLib.SettlementRepository();
            return settlement.UpdateData(data);
        }

        public Models.Messaging<int> DeleteData(Models.SettlementRecord data)
        {
            Repository.EntLib.SettlementRepository settlement = new Repository.EntLib.SettlementRepository();
            return settlement.DeletData(data);
        }


        public int GetPagedDataCount(Models.IParametersSpecification paraSpec)
        {
            Repository.EntLib.SettlementRepository repository = new Repository.EntLib.SettlementRepository();
            return repository.GetPagedDataCount(paraSpec);
        }

        public System.Data.DataSet GetGroupData(Models.IParametersSpecification paraSpec, string groupField)
        {
            Repository.EntLib.SettlementRepository repository = new Repository.EntLib.SettlementRepository();
            return repository.GetGroupData(paraSpec, groupField);
        }
       

        public IEnumerable<Models.SettlementRecord> GetPagedData(Models.IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            Repository.EntLib.SettlementRepository repository = new Repository.EntLib.SettlementRepository();
            return repository.GetPagedData(paraSpec, pageIndex, pageSize);
        }
    }
}
