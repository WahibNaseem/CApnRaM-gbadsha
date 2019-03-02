using System.Data.Entity;
using JK.Repository.Contracts;
using JKApi.Data.DAL;

namespace JK.Repository.Repositories
{
    public class CRM_ContactRepository : BaseRepository<CRM_Contact>, ICRM_ContactRepository
    {
        public CRM_ContactRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
