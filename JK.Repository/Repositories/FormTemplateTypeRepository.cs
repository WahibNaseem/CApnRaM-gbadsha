using System.Data.Entity;
using JK.Repository.Contracts;
using JKApi.Data.DAL;

namespace JK.Repository.Repositories
{
    public class FormTemplateTypeRepository : BaseRepository<FormTemplateType>, IFormTemplateTypeRepository
    {
        public FormTemplateTypeRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
