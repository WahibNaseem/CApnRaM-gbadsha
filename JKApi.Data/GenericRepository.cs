using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Data
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private DB.UserControlEntities entities;
        IDbSet<T> _objectSet;

        public GenericRepository()
        {
            _objectSet = entities.Set<T>();
        }

        public GenericRepository(DB.UserControlEntities _entities)
        {
            entities = _entities;
            _objectSet = entities.Set<T>();

        }

        public IEnumerable<T> GetAll(Func<T, bool> predicate = null)
        {
            if (predicate != null)
            {
                return _objectSet.Where(predicate);
            }

            return _objectSet.AsEnumerable();
        }

        public T Get(Func<T, bool> predicate)
        {
            return _objectSet.First(predicate);
        }

        public bool Add(T entity)
        {
            _objectSet.Add(entity);
            return true;
        }

        public bool Attach(T entity)
        {
            _objectSet.Attach(entity);
            entities.Entry(entity).State = EntityState.Modified;
            entities.SaveChanges();
            return true;
        }

        public bool Delete(T entity)
        {
            _objectSet.Remove(entity);
            return true;
        }
    }

}
