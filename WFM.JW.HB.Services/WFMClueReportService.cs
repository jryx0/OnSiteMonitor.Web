using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WFM.JW.HB.Models;
using WFM.JW.HB.Repository.EntLib;

namespace WFM.JW.HB.Services
{
    public class WFMClueReportService  : ISerivces<WFMClueSummary>
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
            WFMClueReportRepository crr = new WFMClueReportRepository();
            return crr.GetAllData(paraSpec);
        }

        public IEnumerable<WFMClueSummary> GetPagedData(IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public int GetPagedDataCount(IParametersSpecification paraSpec)
        {
            throw new NotImplementedException();
        }

        public Messaging<int> InsertData(WFMClueSummary data)
        {
            throw new NotImplementedException();
        }

        public Messaging<int> UpdateData(WFMClueSummary data)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<WFMClueSummary> GetPageDataByRegion(int type, IParametersSpecification spec, int PageNo, int PageSize)
        {
            WFMClueReportRepository crr = new WFMClueReportRepository();
            return crr.GetPageDataByRegion(type, spec, PageNo, PageSize);
        }

        public int GetPagedDataCountByRegion(int type, IParametersSpecification paraSpec)
        {
            WFMClueReportRepository crr = new WFMClueReportRepository();
            return crr.GetPageDatCountByRegion(type, paraSpec);
        }


        public IEnumerable<WFMClueSummary> GetPageDataByItem(int type, IParametersSpecification spec, int PageNo, int PageSize)
        {
            WFMClueReportRepository crr = new WFMClueReportRepository();
            return crr.GetPageDataByItem(type, spec, PageNo, PageSize);
        }

        public int GetPagedDataCountByItem(int type, IParametersSpecification paraSpec)
        {
            WFMClueReportRepository crr = new WFMClueReportRepository();
            return crr.GetPageDatCountByItem(type, paraSpec);
        }

    }
}
