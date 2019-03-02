using System;
using System.Linq;
using JK.Repository.Uow;
using JKApi.Core;
using JKApi.Data.DAL;
using JKApi.Service.ServiceContract.Enums;
using JKViewModels;

namespace JKApi.Service.Service.Enums
{
    public class ContactTypeListService : BaseService, IContactTypeListService
    {
        private readonly NLogger _nLogger;
        private readonly ICacheProvider _cacheProvider;

        #region ContactTypeListService > Constructor

        public ContactTypeListService(IJKEfUow uow, ICacheProvider cacheProvider)
        {
            Uow = uow;
            _cacheProvider = cacheProvider;
            _nLogger = NLogger.Instance;
        }

        #endregion

        #region ContactTypeListService > GetAll

        public IQueryable<ContactTypeList> GetAll_ContactTypeList()
        {
            if (!_cacheProvider.Contains(CacheKeyName.ContactTypeList_GetAll))
            {
                _cacheProvider.Set(CacheKeyName.ContactTypeList_GetAll, Uow.ContactTypeList.GetAll());
            }
            return (IQueryable<ContactTypeList>)_cacheProvider.Get(CacheKeyName.ContactTypeList_GetAll);
        }

        #endregion

        #region ContactTypeListService > Get

        public ContactTypeList Get_ContactTypeList(int id)
        {
            return GetAll_ContactTypeList().FirstOrDefault(x => x.ContactTypeListId == id);
        }

        #endregion

        #region ContactTypeListService > AddOrUpdate

        public ContactTypeList AddOrUpdate_ContactTypeList(ContactTypeList contactTypeList)
        {
            var isNew = contactTypeList.ContactTypeListId == 0;
            if (isNew)
            {
                Uow.ContactTypeList.Add(contactTypeList);
            }
            else
            {
                Uow.ContactTypeList.Update(contactTypeList);
            }
            Uow.Commit();
            ClearCache();
            return contactTypeList;
        }

        #endregion

        #region ContactTypeListService > Delete

        public void Delete(int id)
        {
            Uow.ContactTypeList.Delete(id);
            Uow.Commit();
            ClearCache();
        }

        #endregion

        #region ContactTypeListService > Cache

        public void ClearCache()
        {
            _cacheProvider?.Remove(CacheKeyName.ContactTypeList_GetAll);
        }

        #endregion
    }
}
