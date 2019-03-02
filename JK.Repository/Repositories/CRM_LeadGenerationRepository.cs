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
    public class CRM_LeadGenerationRepository : BaseRepository<CRM_LeadGeneration>, ICRM_LeadGenerationRepository
    {
        public CRM_LeadGenerationRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
