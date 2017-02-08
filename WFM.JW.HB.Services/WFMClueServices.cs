using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WFM.JW.HB.Models;
using WFM.JW.HB.Repository.EntLib;

namespace WFM.JW.HB.Services
{
    public class WFMClueServices  
    {
        public Messaging<int> DeleteData(Clues data)
        {
            throw new NotImplementedException();
        }
         

        public IEnumerable<Clues> GetAllData(String RegionCode)
        {
            return GetAllData(RegionCode, new CriteriaSpecification("1", CriteriaOperator.Equal, 1));
        }

        public IEnumerable<Clues> GetAllData(String RegionCode, IParametersSpecification paraSpec)
        {
            WFMClueRepository wcr = new WFMClueRepository();
            return wcr.GetAllData("ClueData_" + RegionCode, paraSpec);
        }

        public IEnumerable<Clues> GetPagedData(String RegionCode, IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            WFMClueRepository wcr = new WFMClueRepository();
            return wcr.GetPagedData("ClueData_" + RegionCode, paraSpec, pageIndex, pageSize);
        }

        public int GetPagedDataCount(String RegionCode,IParametersSpecification paraSpec)
        {
            WFMClueRepository wcr = new WFMClueRepository();
            return wcr.GetPagedDataCount("ClueData_" + RegionCode, paraSpec);
        }

        public Messaging<int> InsertData(Clues data)
        {
            throw new NotImplementedException();
        }

        public Messaging<int> UpdateData(Clues data)
        {
            throw new NotImplementedException();
        }
    }
}
