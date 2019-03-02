using JK.Repository.Contracts;
using JKApi.Data.DAL;
using System.Data.Entity;


namespace JK.Repository.Repositories
{
    public class CRM_PurposeTypeRepository : BaseRepository<CRM_PurposeType>, ICRM_PurposeTypeRepository
    {
        public CRM_PurposeTypeRepository(DbContext dbContext) : base(dbContext) { }
       
    }
}
