using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WFM.JW.HB.Models;
using WFM.JW.HB.Repository.EntLib;

namespace WFM.JW.HB.Services
{
    public class WFMClueSummaryServices : ISerivces<Models.WFMClueSummary>
    {
        public Messaging<int> DeleteData(WFMClueSummary data)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<WFMClueSummary> GetAllData()
        {
            return GetAllData(null);
        }

        public IEnumerable<WFMClueSummary> GetAllData(IParametersSpecification paraSpec)
        {
            WFMClueSummaryRepository csr = new WFMClueSummaryRepository();
            return csr.GetAllData(paraSpec); 
        }

        public IEnumerable<WFMClueSummary> GetPagedData(IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            WFMClueSummaryRepository csr = new WFMClueSummaryRepository();
            return csr.GetPageData(paraSpec, pageIndex, pageSize);
        }


        public IEnumerable<WFMClueSummary> GetGroupData2(IParametersSpecification paraSpec)
        {
            WFMClueSummaryRepository csr = new WFMClueSummaryRepository();
              return csr.GetGroupData2(paraSpec);
        }

        public IEnumerable<WFMClueSummary> GetGroupData1(IParametersSpecification paraSpec)
        {
            WFMClueSummaryRepository csr = new WFMClueSummaryRepository();
            return csr.GetGroupData1(paraSpec);
        }

        public int GetPagedDataCount(IParametersSpecification paraSpec)
        {
            WFMClueSummaryRepository csr = new WFMClueSummaryRepository();
            return csr.GetPagedDataCount(paraSpec );
        }

        public Messaging<int> InsertData(WFMClueSummary data)
        {
            throw new NotImplementedException();
        }

        public Messaging<int> UpdateData(WFMClueSummary data)
        {
            throw new NotImplementedException();
        }
    }
}
