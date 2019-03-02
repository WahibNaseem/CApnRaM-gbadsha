using JKApi.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using JK.Repository.Contracts;

namespace JK.Repository.Repositories
{
    public class CRM_AccountFranchiseDetailRepository : BaseRepository<CRM_AccountFranchiseDetail> , ICRM_AccountFranchiseDetailRepository
    {
        public CRM_AccountFranchiseDetailRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
