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
    public class CRM_FollowUpRepository : BaseRepository<CRM_FollowUp>, ICRM_FollowUpRepository
    {
        public CRM_FollowUpRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
