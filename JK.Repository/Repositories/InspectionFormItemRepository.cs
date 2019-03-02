using System.Data.Entity;
using JK.Repository.Contracts;
using JKApi.Data.DAL;

namespace JK.Repository.Repositories
{
    public class InspectionFormItemRepository : BaseRepository<InspectionFormItem>, IInspectionFormItemRepository
    {
        public InspectionFormItemRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
