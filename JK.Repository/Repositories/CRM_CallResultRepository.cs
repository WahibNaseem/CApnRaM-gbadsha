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
    public class CRM_CallResultRepository : BaseRepository<CRM_CallResult>, ICRM_CallResultRepository
    {
        public CRM_CallResultRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
