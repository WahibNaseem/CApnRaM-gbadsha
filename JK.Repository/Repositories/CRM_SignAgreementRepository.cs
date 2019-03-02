using JK.Repository.Contracts;
using JKApi.Data.DAL;
using System.Data.Entity;

namespace JK.Repository.Repositories
{
    public class CRM_SignAgreementRepository:BaseRepository<CRM_SignAgreement>,ICRM_SignAgreementRepository
    {
        public CRM_SignAgreementRepository(DbContext context) : base(context) { }
    }
}
