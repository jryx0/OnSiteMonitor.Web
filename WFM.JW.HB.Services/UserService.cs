using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WFM.JW.HB.Services
{
    public class UserService : ISerivces<Models.User>
    {
        public IEnumerable<Models.User> GetAllData()
        {
           return GetAllData(null);
        }

        public IEnumerable<Models.User> GetAllData(Models.IParametersSpecification paraSpec)
        {
            Repository.EntLib.UserRepository repository = new Repository.EntLib.UserRepository();
            return repository.GetUser(paraSpec);
        }

        public Models.Messaging<int> InsertData(Models.User data)
        {
            throw new NotImplementedException();
        }

        public Models.Messaging<int> UpdateData(Models.User data)
        {
            throw new NotImplementedException();
        }


        public Models.Messaging<int> DeleteData(Models.User data)
        {
            throw new NotImplementedException();
        }


        public int GetPagedDataCount(Models.IParametersSpecification paraSpec)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Models.User> GetPagedData(Models.IParametersSpecification paraSpec, int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
