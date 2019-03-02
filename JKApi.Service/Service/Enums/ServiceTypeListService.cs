using System;
using System.Linq;
using JK.Repository.Uow;
using JKApi.Core;
using JKApi.Data.DAL;
using JKApi.Service.ServiceContract.Enums;
using JKViewModels;

namespace JKApi.Service.Service.Enums
{
    public class ServiceTypeListService : BaseService, IServiceTypeListService
    {
        private readonly NLogger _nLogger;
        private readonly ICacheProvider _cacheProvider;

        #region ServiceTypeListService > Constructor

        public ServiceTypeListService(IJKEfUow uow, ICacheProvider cacheProvider)
        {
            Uow = uow;
            _cacheProvider = cacheProvider;
            _nLogger = NLogger.Instance;
        }

        #endregion

        #region ServiceTypeListService > GetAll

        public IQueryable<ServiceTypeList> GetAll_ServiceTypeList()
        {
            if (!_cacheProvider.Contains(CacheKeyName.ServiceTypeList_GetAll))
            {
                _cacheProvider.Set(CacheKeyName.ServiceTypeList_GetAll, Uow.ServiceTypeList.GetAll());
            }
            return (IQueryable<ServiceTypeList>)_cacheProvider.Get(CacheKeyName.ServiceTypeList_GetAll);
        }

        #endregion

        #region ServiceTypeListService > Get

        public ServiceTypeList Get_ServiceTypeList(int id)
        {
            return GetAll_ServiceTypeList().FirstOrDefault(x => x.ServiceTypeListid == id);
        }

        #endregion

        #region ServiceTypeListService > AddOrUpdate

        public ServiceTypeList AddOrUpdate_ServiceTypeList(ServiceTypeList serviceTypeList)
        {
            var isNew = serviceTypeList.ServiceTypeListid == 0;
            if (isNew)
            {
                Uow.ServiceTypeList.Add(serviceTypeList);
            }
            else
            {
                Uow.ServiceTypeList.Update(serviceTypeList);
            }
            Uow.Commit();
            ClearCache();
            return serviceTypeList;
        }

        #endregion

        #region ServiceTypeListService > Delete

        public void Delete(int id)
        {
            Uow.ServiceTypeList.Delete(id);
            Uow.Commit();
            ClearCache();
        }

        #endregion

        #region ServiceTypeListService > Cache

        public void ClearCache()
        {
            _cacheProvider.Remove(CacheKeyName.ServiceTypeList_GetAll);
        }

        #endregion
    }
}
