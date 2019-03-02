using System.Data.Entity;
using JK.Repository.Contracts;
using JKApi.Data.DAL;

namespace JK.Repository.Repositories
{
    public class InspectionStatusRepository : BaseRepository<InspectionStatu>, IInspectionStatusRepository
    {
        public InspectionStatusRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
