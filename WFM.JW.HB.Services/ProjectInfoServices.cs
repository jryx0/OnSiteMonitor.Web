using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WFM.JW.HB.Repository.EntLib;

namespace WFM.JW.HB.Services
{
    public class ProjectInfoServices : ISerivces<Models.ProjectInfo>
    {
        public IEnumerable<Models.ProjectInfo> GetAllData()
        {
            return GetAllData(null);
        }

        public IEnumerable<Models.ProjectInfo> GetAllData(Models.IParametersSpecification paraSpec)
        {
            ProjectInfoRepository repository = new ProjectInfoRepository();
            return repository.GetAllData(paraSpec);
        }

        public Models.Messaging<int> InsertData(Models.ProjectInfo data)
        {
            ProjectInfoRepository repository = new ProjectInfoRepository();
            return repository.InsetData(data);
        }

        public Models.Messaging<int> UpdateData(Models.ProjectInfo data)
        {
            ProjectInfoRepository repository = new ProjectInfoRepository();
            return repository.UpdateData(data);
        }

        public Models.Messaging<int> DeleteData(Models.ProjectInfo data)
        {
            ProjectInfoRepository repository = new ProjectInfoRepository();
            return repository.DeletData(data);
        }


        public int GetPagedDataCount(Models.IParametersSpecification paraSpec)
        {
            ProjectInfoRepository repository = new ProjectInfoRepository();
            return repository.GetPagedDataCount(paraSpec);
        }

        public IEnumerable<Models.ProjectInfo> GetPagedData(Models.IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            ProjectInfoRepository repository = new ProjectInfoRepository();
            return repository.GetPagedData(paraSpec, pageIndex, pageSize);
        }
    }
}
