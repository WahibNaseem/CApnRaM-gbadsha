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
   public class CRM_CallLogRepository:BaseRepository<CRM_CallLog>, ICRM_CallLogRepository
    {
        public CRM_CallLogRepository(DbContext context) : base(context) { }
    }
}
