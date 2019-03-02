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
    public class CRM_ScheduleTypeRepository : BaseRepository<CRM_ScheduleType>, ICRM_ScheduleTypeRepository
    {
        public CRM_ScheduleTypeRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
