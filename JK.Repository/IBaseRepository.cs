using System;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace JK.Repository
{
    public interface IBaseRepository<T> where T : class
    {
        IQueryable<T> GetAll(Boolean noTrack = false);
        T GetById(int id);
        T GetById(long id);
        T GetByStringKey(string key);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(int id);
        void DeleteByStringKey(string key);
        ObjectResult<T> StoredProc<T>(string pName, params ObjectParameter[] pParams);
        ObjectResult<T> StoredQuery<T>(string pName, params ObjectParameter[] pParams);
    }
}
