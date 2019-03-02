using System.Linq;
using JK.Repository.Uow;
using JKApi.Core;
using JKApi.Data.DAL;
using JKApi.Service.ServiceContract.Contract;

namespace JKApi.Service.Service.Contract
{
    public class ContractDetailService : BaseService, IContractDetailService
    {
        private readonly NLogger _nLogger;
        private readonly ICacheProvider _cacheProvider;

        #region ContractDetailService > Constructor

        public ContractDetailService(IJKEfUow uow, ICacheProvider cacheProvider)
        {
            Uow = uow;
            _cacheProvider = cacheProvider;
            _nLogger = NLogger.Instance;
        }

        #endregion

        #region ContractDetailService > GetAll

        public IQueryable<ContractDetail> GetAll_ContractDetails()
        {
            return Uow.ContractDetail.GetAll();
        }

        #endregion

        #region ContractDetailService > Get

        public ContractDetail Get_ContractDetail(int id)
        {
            return Uow.ContractDetail.GetById(id);
        }

        #endregion

        #region ContractDetailService > AddOrUpdate

        public ContractDetail AddOrUpdate_ContractDetail(ContractDetail contractDetail)
        {
            var isNew = contractDetail.ContractDetailId == 0;
            if (isNew)
            {
                Uow.ContractDetail.Add(contractDetail);
            }
            else
            {
                Uow.ContractDetail.Update(contractDetail);
            }
            Uow.Commit();
            return contractDetail;
        }

        #endregion

        #region ContractDetailService > Delete

        public void Delete_ContractDetail(int id)
        {
            Uow.ContractDetail.Delete(id);
            Uow.Commit();
        }

        #endregion
    }
}
