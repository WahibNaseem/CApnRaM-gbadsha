using System.Linq;
using JKApi.Service.Service;

namespace JKApi.Service.ServiceContract.Distribution
{
    public interface IDistributionService : ICachedService
    {
        #region IDistributionService > GetAll

        IQueryable<Data.DAL.Distribution> GetAll_Distributions();
        IQueryable<Data.DAL.Distribution> GetAll_Distributions_ByContractDetailId(int id);
        IQueryable<Data.DAL.Distribution> GetAll_Distributions_ByCustomerDetailId(int id);
        IQueryable<Data.DAL.Distribution> GetAll_Distributions_ByFranchiseeId(int id);

        #endregion

        #region IDistributionService > Get

        Data.DAL.Distribution Get_Distribution(int id);

        #endregion

        #region IDistributionService > AddOrUpdate

        Data.DAL.Distribution AddOrUpdate_Distribution(Data.DAL.Distribution distribution);

        #endregion

        #region IDistributionService > Delete

        void Delete_Distribution(int id);

        #endregion
    }
}
