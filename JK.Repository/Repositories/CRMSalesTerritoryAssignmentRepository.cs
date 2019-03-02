using JK.Repository.Contracts;
using JKApi.Data.DAL;
using System.Data.Entity;

namespace JK.Repository.Repositories
{
    public class CRMSalesTerritoryAssignmentRepository : BaseRepository<CRM_SalesTerritory_Assignment>, ICRMSalesTerritoryAssignmentRepository
    {
        public CRMSalesTerritoryAssignmentRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
