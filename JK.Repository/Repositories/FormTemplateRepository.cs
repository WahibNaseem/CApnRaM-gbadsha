using System.Data.Entity;
using JK.Repository.Contracts;
using JKApi.Data.DAL;

namespace JK.Repository.Repositories
{
    public class FormTemplateRepository : BaseRepository<FormTemplate>, IFormTemplateRepository
    {
        public FormTemplateRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
