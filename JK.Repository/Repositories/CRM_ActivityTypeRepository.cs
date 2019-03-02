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
    public class CRM_ActivityTypeRepository : BaseRepository<CRM_ActivityType>, ICRM_ActivityTypeRepository
    {
        public CRM_ActivityTypeRepository(DbContext context) : base(context) { }
    }
}
