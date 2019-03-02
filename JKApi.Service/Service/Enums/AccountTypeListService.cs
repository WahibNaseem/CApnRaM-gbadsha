using System;
using System.Linq;
using JK.Repository.Uow;
using JKApi.Core;
using JKApi.Data.DAL;
using JKApi.Service.ServiceContract.Enums;
using JKViewModels;

namespace JKApi.Service.Service.Enums
{
    public class AccountTypeListService : BaseService, IAccountTypeListService
    {
        private readonly NLogger _nLogger;
        private readonly ICacheProvider _cacheProvider;

        #region AccountTypeListService > Constructor

        public AccountTypeListService(IJKEfUow uow, ICacheProvider cacheProvider)
        {
            Uow = uow;
            _cacheProvider = cacheProvider;
            _nLogger = NLogger.Instance;
        }

        #endregion

        #region AccountTypeListService > GetAll

        public IQueryable<AccountTypeList> GetAll_AccountTypeList()
        {
            if (!_cacheProvider.Contains(CacheKeyName.AccountTypeList_GetAll))
            {
                _cacheProvider.Set(CacheKeyName.AccountTypeList_GetAll, Uow.AccountTypeList.GetAll());
            }
            return (IQueryable<AccountTypeList>) _cacheProvider.Get(CacheKeyName.AccountTypeList_GetAll);
        }

        #endregion

        #region AccountTypeListService > Get

        public AccountTypeList Get_AccountTypeList(int id)
        {
            return GetAll_AccountTypeList().FirstOrDefault(x => x.AccountTypeListId == id);
        }

        #endregion

        #region AccountTypeListService > AddOrUpdate

        public AccountTypeList AddOrUpdate_AccountTypeList(AccountTypeList accountTypeList)
        {
            var isNew = accountTypeList.AccountTypeListId == 0;
            if (isNew)
            {
                Uow.AccountTypeList.Add(accountTypeList);
            }
            else
            {
                Uow.AccountTypeList.Update(accountTypeList);
            }
            Uow.Commit();
            ClearCache();
            return accountTypeList;
        }

        #endregion

        #region AccountTypeListService > Delete

        public void Delete_AccountTypeList(int id)
        {
            Uow.AccountTypeList.Delete(id);
            Uow.Commit();
            ClearCache();
        }

        #endregion

        public void ClearCache()
        {
            _cacheProvider?.Remove(CacheKeyName.AccountTypeList_GetAll);
        }
    }
}
