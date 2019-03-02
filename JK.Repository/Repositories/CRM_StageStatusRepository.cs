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
    public class CRM_StageStatusRepository : BaseRepository<CRM_StageStatus>, ICRM_StageStatusRepository
    {
        public CRM_StageStatusRepository(DbContext context) : base(context) { }
    }
}
