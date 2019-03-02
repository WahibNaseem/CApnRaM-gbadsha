using JK.Repository.Contracts;
using JKApi.Data.DAL;
using System.Data.Entity;


namespace JK.Repository.Repositories
{
   public class CRM_ReasonTypeRepository:BaseRepository<CRM_ReasonType>,ICRM_ReasonTypeRepository
    {
        public CRM_ReasonTypeRepository(DbContext context) : base(context) { }
    }
}
