using System.Linq;
using JKApi.Data.DAL;

namespace JKApi.Service.ServiceContract.Contract
{
    public interface IContractDetailService
    {
        #region IContractDetailService > GetAll

        IQueryable<ContractDetail> GetAll_ContractDetails();

        #endregion

        #region IContractDetailService > Get

        ContractDetail Get_ContractDetail(int id);

        #endregion

        #region IContractDetailService > AddOrUpdate

        ContractDetail AddOrUpdate_ContractDetail(ContractDetail contractDetail);

        #endregion

        #region IContractDetailService > Delete

        void Delete_ContractDetail(int id);

        #endregion
    }
}
