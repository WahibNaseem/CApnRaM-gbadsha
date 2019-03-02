using JK.Repository.Contracts;
using JKApi.Data.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JK.Repository.Repositories
{
    public class CRM_AccountCustomerDetailRepository : BaseRepository<CRM_AccountCustomerDetail>, ICRM_AccountCustomerDetailRepository
    {
        public CRM_AccountCustomerDetailRepository(DbContext context) : base(context) { }
    }
}
