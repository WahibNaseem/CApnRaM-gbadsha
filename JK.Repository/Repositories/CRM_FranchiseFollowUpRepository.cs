using JK.Repository.Contracts;
using JKApi.Data.DAL;
using System.Data.Entity;


namespace JK.Repository.Repositories
{
   public class CRM_FranchiseFollowUpRepository:BaseRepository<CRM_FranchiseFollowUp>,ICRM_FranchiseFollowUpRepository
    {
        public CRM_FranchiseFollowUpRepository(DbContext context) : base(context) { }
    }
}
