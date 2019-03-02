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
    public class CRM_SalePossibilityTypeRespository : BaseRepository<CRM_SalePossibilityType>, ICRM_SalePossibilityTypeRepository
    {
        public CRM_SalePossibilityTypeRespository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
