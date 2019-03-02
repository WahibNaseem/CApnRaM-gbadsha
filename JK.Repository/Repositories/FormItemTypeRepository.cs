using System.Data.Entity;
using JK.Repository.Contracts;
using JKApi.Data.DAL;

namespace JK.Repository.Repositories
{
    public class FormItemTypeRepository : BaseRepository<FormItemType>, IFormItemTypeRepository
    {
        public FormItemTypeRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
