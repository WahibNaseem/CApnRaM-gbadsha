using System.Data.Entity;
using JK.Repository.Contracts;
using JKApi.Data.DAL;

namespace JK.Repository.Repositories
{
    public class CRMTerritoryAssignmentRepository : BaseRepository<CRM_Territory_Assignment>, ICRMTerritoryAssignmentRepository
    {
        public CRMTerritoryAssignmentRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
