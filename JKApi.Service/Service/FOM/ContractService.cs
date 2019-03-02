using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using JK.Repository.Uow;
using JKApi.Core;
using JKViewModels.Common;
using JKViewModels.Contract;
using JKViewModels.Customer;

namespace JKApi.Service.Service.FOM
{

    public interface IContractService
    {
        #region IContractService > GetAll

        IQueryable<Data.DAL.Contract> GetAll_Contracts();

        #endregion

        #region IContractService > Get

        Data.DAL.Contract Get_Contract(int id);

        #endregion

        #region IContractService > AddOrUpdate

        Data.DAL.Contract AddOrUpdate_Contract(Data.DAL.Contract contract);

        #endregion

        #region IContractService > Delete

        void Delete_Contract(int id);

        #endregion

        List<ContractModel> GetAllContracts();
        List<ContractModel> GetContractsByFranchisee(int franchiseeId);
    }

    public class ContractService : BaseService, IContractService
    {
        private readonly NLogger _nLogger;
        private readonly ICacheProvider _cacheProvider;

        #region ContractService > Constructor

        public ContractService(IJKEfUow uow, ICacheProvider cacheProvider)
        {
            Uow = uow;
            _cacheProvider = cacheProvider;
            _nLogger = NLogger.Instance;
        }

        #endregion

        #region ContractService > GetAll

        public IQueryable<Data.DAL.Contract> GetAll_Contracts()
        {
            return Uow.Contract.GetAll();
        }

        #endregion

        #region ContractService > Get

        public Data.DAL.Contract Get_Contract(int id)
        {
            return Uow.Contract.GetById(id);
        }

        #endregion

        #region ContractService > AddOrUpdate

        public Data.DAL.Contract AddOrUpdate_Contract(Data.DAL.Contract contract)
        {
            var isNew = contract.ContractId == 0;
            if (isNew)
            {
                Uow.Contract.Add(contract);
            }
            else
            {
                Uow.Contract.Update(contract);
            }
            Uow.Commit();
            return contract;
        }

        #endregion

        #region ContractService > Delete

        public void Delete_Contract(int id)
        {
            Uow.Contract.Delete(id);
            Uow.Commit();
        }

        #endregion

        public List<ContractModel> GetAllContracts()
        {
            var parameters = new DynamicParameters();

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetAllContracts, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return multipleResult?.Read<ContractModel, AddressModel, ContractModel>((contract, address) =>
                {
                    contract.Address = address;
                    return contract;
                }, "AddressId").ToList();
            }
        }

        public List<ContractModel> GetContractsByFranchisee(int franchiseeId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FranchiseeId", franchiseeId);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetAllContractsByFranchisee, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return multipleResult?.Read<ContractModel, AddressModel, ContractModel>((contract, address) =>
                {
                    contract.Address = address;
                    return contract;
                }, "AddressId").ToList();
            }
        }
    }
}
