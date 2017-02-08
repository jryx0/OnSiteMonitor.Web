using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WFM.JW.HB.Repository;

namespace WFM.JW.HB.Services
{
    public class EmployeeServices : ISerivces<Models.Employees>
    {
        public IEnumerable<Models.Employees> GetAllData()
        {
            return GetAllData(null);
        }

        public IEnumerable<Models.Employees> GetAllData(Models.IParametersSpecification paraSpec)
        {
            Repository.EntLib.EmployeesRepository service = new Repository.EntLib.EmployeesRepository();
            return service.GetAllData(paraSpec);
        }

        public Models.Messaging<int> InsertData(Models.Employees data)
        {
            Repository.EntLib.EmployeesRepository service = new Repository.EntLib.EmployeesRepository();
            return service.InsetData(data);
        }

        public Models.Messaging<int> UpdateData(Models.Employees data)
        {
            Repository.EntLib.EmployeesRepository service = new Repository.EntLib.EmployeesRepository();
            return service.UpdateData(data);
        }

        public Models.Messaging<int> DeleteData(Models.Employees data)
        {
            Repository.EntLib.EmployeesRepository service = new Repository.EntLib.EmployeesRepository();
            var list = service.GetAllData(new Models.CriteriaSpecification("EmployeeID", Models.CriteriaOperator.Equal, data.EmployeeID));

            data.IsUser = list.First().IsUser;
            data.UserName = list.First().UserName;

            return service.DeletData(data);
        }


        public int GetPagedDataCount(Models.IParametersSpecification paraSpec)
        {
            Repository.EntLib.EmployeesRepository service = new Repository.EntLib.EmployeesRepository();
            return service.GetPagedDataCount(paraSpec);
        }

        public IEnumerable<Models.Employees> GetPagedData(Models.IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            Repository.EntLib.EmployeesRepository service = new Repository.EntLib.EmployeesRepository();
            return service.GetPagedData(paraSpec, pageIndex, pageSize);
        }
    }
}
