using System.Data.Entity;
using JK.Repository.Contracts;
using JKApi.Data.DAL;

namespace JK.Repository.Repositories
{
    public class InspectionFormRepository : BaseRepository<InspectionForm>, IInspectionFormRepository
    {
        public InspectionFormRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
