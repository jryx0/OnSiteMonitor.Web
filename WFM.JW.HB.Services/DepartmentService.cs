using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WFM.JW.HB.Repository.EntLib;

namespace WFM.JW.HB.Services
{
    public class DepartmentsService : ISerivces<Models.Departments>
    {
        public IEnumerable<Models.Departments> GetAllData()
        {
            return GetAllData(null);
        }

        public IEnumerable<Models.Departments> GetAllData(Models.IParametersSpecification paraSpec)
        {
            DepartmentRepository repository = new DepartmentRepository();
            return repository.GetAllData(paraSpec);
        }

        public Models.Messaging<int> InsertData(Models.Departments data)
        {
            DepartmentRepository repository = new DepartmentRepository();
            return repository.InsetData(data);
        }
        

        public Models.Messaging<int> UpdateData(Models.Departments data)
        {
            DepartmentRepository repository = new DepartmentRepository();
            return repository.UpdateData(data);
        }

        public Models.Messaging<int> DeleteData(Models.Departments data)
        {

            DepartmentRepository repository = new DepartmentRepository();
            return repository.DeletData(data);
        }


        public int GetPagedDataCount(Models.IParametersSpecification paraSpec)
        {
            DepartmentRepository repository = new DepartmentRepository();
            return repository.GetPagedDataCount(paraSpec);
        }

        

        public IEnumerable<Models.Departments> GetPagedData(Models.IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            DepartmentRepository repository = new DepartmentRepository();
            return repository.GetPagedData(paraSpec, pageIndex, pageSize);
        }
    }
}
