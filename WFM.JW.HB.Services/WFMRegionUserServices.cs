using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WFM.JW.HB.Repository.EntLib;

namespace WFM.JW.HB.Services
{
    public class WFMRegionUserServices : ISerivces<Models.RegionUser>
    {

        public IEnumerable<Models.RegionUser> GetAllData()
        {
            Models.IParametersSpecification spec = new Models.CriteriaSpecification("1", Models.CriteriaOperator.Equal, 1);
            return this.GetAllData(spec);
        }

        public IEnumerable<Models.RegionUser> GetAllData(Models.IParametersSpecification paraSpec)
        {
            WFMRegionUserRepository repository = new WFMRegionUserRepository();
            return repository.FindRegion(paraSpec);
        }

        public int GetPagedDataCount(Models.IParametersSpecification paraSpec)
        {
            WFMRegionUserRepository repository = new WFMRegionUserRepository();
            return repository.GetPagedDataCount(paraSpec);
        }

        public IEnumerable<Models.RegionUser> GetPagedData(Models.IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            WFMRegionUserRepository repository = new WFMRegionUserRepository();
            return repository.GetPagedData(paraSpec, pageIndex, pageSize);
        }

        public Models.Messaging<int> InsertData(Models.RegionUser data)
        {
            WFMRegionUserRepository repository = new WFMRegionUserRepository();
            return repository.InsertData(data);
        }

        public Models.Messaging<int> UpdateData(Models.RegionUser data)
        {
            WFMRegionUserRepository repository = new WFMRegionUserRepository();
            return repository.UpdateData(data);
        }

        public Models.Messaging<int> DeleteData(Models.RegionUser data)
        {
            WFMRegionUserRepository repository = new WFMRegionUserRepository();
            return repository.DeleteData(data);
        }
    }
}
