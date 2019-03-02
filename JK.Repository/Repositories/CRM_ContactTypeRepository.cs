using System.Data.Entity;
using JKApi.Data.DAL;
using JK.Repository.Contracts;

namespace JK.Repository.Repositories
{
    public class CRM_ContactTypeRepository : BaseRepository<CRM_ContactType>, ICRM_ContactTypeRepository
    {
        public CRM_ContactTypeRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
