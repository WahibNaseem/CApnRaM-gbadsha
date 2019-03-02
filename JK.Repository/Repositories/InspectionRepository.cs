using System.Data.Entity;
using JK.Repository.Contracts;
using JKApi.Data.DAL;

namespace JK.Repository.Repositories
{
    public class InspectionRepository : BaseRepository<Inspection> , IInspectionRepository
    {
        public InspectionRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
