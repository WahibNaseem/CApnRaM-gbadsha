using System.Linq;
using JKApi.Data.DAL;
using JKApi.Service.Service;

namespace JKApi.Service.ServiceContract.Enums
{
    public interface IServiceTypeListService : ICachedService
    {
        #region IServiceTypeListService > GetAll

        IQueryable<ServiceTypeList> GetAll_ServiceTypeList();

        #endregion

        #region IServiceTypeListService > Get

        ServiceTypeList Get_ServiceTypeList(int id);

        #endregion

        #region IServiceTypeListService > AddOrUpdate

        ServiceTypeList AddOrUpdate_ServiceTypeList(ServiceTypeList serviceTypeList);

        #endregion

        #region IServiceTypeListService > Delete

        void Delete(int id);

        #endregion
    }
}
