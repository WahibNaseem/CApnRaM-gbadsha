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
    public class CRM_ProviderTypeRepository : BaseRepository<CRM_ProviderType>, ICRM_ProviderTypeRepository
    {
        public CRM_ProviderTypeRepository(DbContext context) : base(context) { }
    }
}
