using System.Linq;
using JKApi.Data.DAL;
using JKApi.Service.Service;

namespace JKApi.Service.ServiceContract.Enums
{
    public interface IAccountTypeListService : ICachedService
    {
        #region IAccountTypeListService > GetAll

        IQueryable<AccountTypeList> GetAll_AccountTypeList();

        #endregion

        #region IAccountTypeListService > Get

        AccountTypeList Get_AccountTypeList(int id);

        #endregion

        #region IAccountTypeListService > AddOrUpdate

        AccountTypeList AddOrUpdate_AccountTypeList(AccountTypeList accountTypeList);

        #endregion

        #region IAccountTypeListService > Delete

        void Delete_AccountTypeList(int id);

        #endregion

    }
}
