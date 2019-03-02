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
    public class CRM_CloseRepository : BaseRepository<CRM_Close>, ICRM_CloseRepository
    {
        public CRM_CloseRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
