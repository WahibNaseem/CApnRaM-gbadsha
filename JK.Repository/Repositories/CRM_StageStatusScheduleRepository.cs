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
    public class CRM_StageStatusScheduleRepository : BaseRepository<CRM_StageStatusSchedule>, ICRM_StageStatusScheduleRepository
    {
        public CRM_StageStatusScheduleRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
