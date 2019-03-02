using System.Data.Entity;
using JKApi.Data.DAL;
using JK.Repository.Contracts;


namespace JK.Repository.Repositories
{
    public class CRMTerritoryRepository : BaseRepository<CRM_Territory>, ICRMTerritoryRepository
    {
        public CRMTerritoryRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
