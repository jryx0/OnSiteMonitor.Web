using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WFM.JW.HB.Repository.EntLib;

namespace WFM.JW.HB.Services
{
    public class WFMRegionServices :ISerivces<Models.Region>
    {
        public IEnumerable<Models.Region> GetAllData()
        {
            Models.IParametersSpecification spec = new Models.CriteriaSpecification("1", Models.CriteriaOperator.Equal, 1);
            return this.GetAllData(spec);
        }

        public IEnumerable<Models.Region> GetAllData(Models.IParametersSpecification paraSpec)
        {
            WFMRegionRepository repository = new WFMRegionRepository();
            return repository.FindRegion(paraSpec);
        }

        public Models.Messaging<int> InsertData(Models.Region data)
        {            
            return null;
        }

        public Models.Messaging<int> UpdateData(Models.Region data)
        {
            WFMRegionRepository repository = new WFMRegionRepository();
            return repository.UpdateData(data);
        }

        public Models.Messaging<int> DeleteData(Models.Region data)
        {
            return null;
        }

        public int GetPagedDataCount(Models.IParametersSpecification paraSpec)
        {

            WFMRegionRepository repository = new WFMRegionRepository();
            return repository.GetPagedDataCount(paraSpec);
        }

        public IEnumerable<Models.Region> GetPagedData(Models.IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            WFMRegionRepository repository = new WFMRegionRepository();
            return repository.GetPagedData(paraSpec, pageIndex, pageSize);
        }
    }
}
