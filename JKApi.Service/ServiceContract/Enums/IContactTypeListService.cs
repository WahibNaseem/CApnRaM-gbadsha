using System.Linq;
using JKApi.Service.Service;
using JKApi.Data.DAL;

namespace JKApi.Service.ServiceContract.Enums
{
    public interface IContactTypeListService : ICachedService
    {
        #region IContactTypeListService > GetAll

        IQueryable<ContactTypeList> GetAll_ContactTypeList();

        #endregion

        #region IContactTypeListService > Get

        ContactTypeList Get_ContactTypeList(int id);

        #endregion

        #region IContactTypeListService > AddOrUpdate

        ContactTypeList AddOrUpdate_ContactTypeList(ContactTypeList contactTypeList);

        #endregion

        #region IContactTypeListService > Delete

        void Delete(int id);

        #endregion
    }
}
