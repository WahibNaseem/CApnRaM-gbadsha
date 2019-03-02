using JK.Repository.Contracts;
using JKApi.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace JK.Repository.Repositories
{
    public class CRM_FranchiseContractRepository : BaseRepository<CRM_FranchiseContract>, ICRM_FranchiseContractRepository
    {
        public CRM_FranchiseContractRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
