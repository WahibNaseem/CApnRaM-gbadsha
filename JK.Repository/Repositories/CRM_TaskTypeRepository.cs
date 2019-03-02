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
    public class CRM_TaskTypeRepository : BaseRepository<CRM_TaskType>, ICRM_TaskTypeRepository
    {
        public CRM_TaskTypeRepository(DbContext context) : base(context) { }
    }
}
