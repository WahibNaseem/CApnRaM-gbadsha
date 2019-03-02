using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Data
{
    public class GenericUnitOfWork : IDisposable
    {
        private DB.UserControlEntities entities = null;
        private bool disposed = false;

        public GenericUnitOfWork()
        {
            entities = new DB.UserControlEntities();
        }

        public Dictionary<Type, object> repositories = new Dictionary<Type, object>();

        public IRepository<T> Repository<T>() where T : class
        {
            if (repositories.Keys.Contains(typeof(T)) == true)
            {
                return repositories[typeof(T)] as IRepository<T>;
            }
            IRepository<T> repo = new GenericRepository<T>(entities);
            repositories.Add(typeof(T), repo);
            return repo;
        }

        public void SaveChanges()
        {
            entities.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    entities.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
