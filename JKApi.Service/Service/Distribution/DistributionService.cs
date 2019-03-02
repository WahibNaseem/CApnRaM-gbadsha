using System.Linq;
using JK.Repository.Uow;
using JKApi.Core;
using JKApi.Service.ServiceContract.Distribution;
using JKViewModels;

namespace JKApi.Service.Service.Distribution
{
    public class DistributionService : BaseService, IDistributionService
    {
        private readonly NLogger _nLogger;
        private readonly ICacheProvider _cacheProvider;

        #region DistributionService > Constructor

        public DistributionService(IJKEfUow uow, ICacheProvider cacheProvider)
        {
            Uow = uow;
            _cacheProvider = cacheProvider;
            _nLogger = NLogger.Instance;
        }

        #endregion

        #region DistributionService > GetAll

        public IQueryable<Data.DAL.Distribution> GetAll_Distributions()
        {
            return Uow.DistributionRepository.GetAll();
        }

        public IQueryable<Data.DAL.Distribution> GetAll_Distributions_ByContractDetailId(int id)
        {
            return GetAll_Distributions().Where(x => x.ContractDetailId == id);
        }

        public IQueryable<Data.DAL.Distribution> GetAll_Distributions_ByCustomerDetailId(int id)
        {
            return GetAll_Distributions().Where(x => x.CustomerId == id);
        }

        public IQueryable<Data.DAL.Distribution> GetAll_Distributions_ByFranchiseeId(int id)
        {
            return GetAll_Distributions().Where(x => x.FranchiseeId == id);
        }

        #endregion

        #region DistributionService > Get

        public Data.DAL.Distribution Get_Distribution(int id)
        {
            return GetAll_Distributions().FirstOrDefault(x => x.DistributionId == id);
        }

        #endregion

        #region DistributionService > AddOrUpdate

        public Data.DAL.Distribution AddOrUpdate_Distribution(Data.DAL.Distribution distribution)
        {
            var isNew = distribution.DistributionId == 0;
            if (isNew)
            {
                Uow.DistributionRepository.Add(distribution);
            }
            else
            {
                Uow.DistributionRepository.Update(distribution);
            }
            Uow.Commit();
            ClearCache();
            return distribution;
        }

        #endregion

        #region DistributionService > Delete

        public void Delete_Distribution(int id)
        {
            Uow.DistributionRepository.Delete(id);
            Uow.Commit();
            ClearCache();
        }

        #endregion

        public void ClearCache()
        {
            _cacheProvider?.Remove(CacheKeyName.Distribution_GetAll);
            _cacheProvider?.Remove(CacheKeyName.Distribution_GetAll_ByContractDetailId);
            _cacheProvider?.Remove(CacheKeyName.Distribution_GetAll_ByCustomerId);
            _cacheProvider?.Remove(CacheKeyName.Distribution_GetAll_ByFranchiseeId);
        }
    }
}
