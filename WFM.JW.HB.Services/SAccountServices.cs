using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WFM.JW.HB.Models;
using WFM.JW.HB.Repository.EntLib;

namespace WFM.JW.HB.Services
{
    public class SAccountServices 
    {

        public IEnumerable<SettlementRecord> GetAllData(bool allData)
        {
            return GetAllData(null, allData);
        }

        public IEnumerable<SettlementRecord> GetAllData(IParametersSpecification paraSpec, bool allData)
        {
            SAccountRepository repository = new SAccountRepository();
            return repository.GetAllAccount(paraSpec, allData);
        }


        public IEnumerable<SettlementRecord> GetSettlementData()
        {
            
            return GetSettlementData(null);
        }

        public IEnumerable<SettlementRecord> GetSettlementData(IParametersSpecification paraSpec)
        {
            SAccountRepository repository = new SAccountRepository();
            return repository.GetSettlementAccount(paraSpec);
        }


        public int AccountCount(Models.IParametersSpecification paraSpec, bool allData)
        {
            SAccountRepository repository = new SAccountRepository();
            return repository.AccountCount(paraSpec, allData);
        }

        public System.Data.DataSet AcountGroupData(Models.IParametersSpecification paraSpec, bool allData, string groupField)
        {
            SAccountRepository repository = new SAccountRepository();
            return repository.AcountGroupData(paraSpec, allData, groupField);
        }

        public IEnumerable<Models.SettlementRecord> GetPagedAccount(Models.IParametersSpecification paraSpec, int pageIndex, int pageSize, bool allData)
        {
            SAccountRepository reposity = new SAccountRepository();
            return reposity.GetPagedAccount(paraSpec, pageIndex, pageSize, allData);
        }

        public IEnumerable<Models.SettlementRecord> GetPagedAccount2(Models.IParametersSpecification paraSpec, int pageIndex, int pageSize, bool allData)
        {
            SAccountRepository reposity = new SAccountRepository();
            return reposity.GetPagedAccount2(paraSpec, pageIndex, pageSize, allData);
        }

    }
}
