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
    public class CRM_AccountRepository : BaseRepository<CRM_Account>, ICRM_AccountRepository
    {
        public CRM_AccountRepository(DbContext context) : base(context) { }
    }
}
