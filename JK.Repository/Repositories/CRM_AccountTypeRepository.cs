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
    public class CRM_AccountTypeRepository : BaseRepository<CRM_AccountType>, ICRM_AccountTypeRepository
    {
        public CRM_AccountTypeRepository(DbContext context) : base(context) { }
    }
}
