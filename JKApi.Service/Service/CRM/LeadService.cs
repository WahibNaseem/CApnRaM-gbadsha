using JK.Repository.Uow;
using JKApi.Service.ServiceContract.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JKApi.Data.DAL;

namespace JKApi.Service.Service.CRM
{
    public class LeadService : BaseService, ILeadService
    {
        #region ConstructorCalls

        public LeadService(IJKEfUow uow)
        {
            Uow = uow;
        }
        #endregion
    }
}
