using JK.Repository.Contracts;
using JKApi.Data.DAL;
using System.Data.Entity;

namespace JK.Repository.Repositories
{
    public class CRM_CloseTypeRepository:BaseRepository<CRM_CloseType>,ICRM_CloseTypeRepository
    {
        public CRM_CloseTypeRepository(DbContext context) : base(context) { }
    }
}
