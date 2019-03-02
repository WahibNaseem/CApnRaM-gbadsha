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
    public class CRM_ProviderSourceRepository : BaseRepository<CRM_ProviderSource>, ICRM_ProviderSourceRepository
    {
        public CRM_ProviderSourceRepository(DbContext context) : base(context) { }
    }
}
