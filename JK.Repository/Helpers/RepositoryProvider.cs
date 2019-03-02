using JK.Repository.RepositoryFactory;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace JK.Repository.Helpers
{
    public class RepositoryProvider : IRepositoryProvider
    {
        private RepositoryFactories _repositoryFactories;
        protected Dictionary<Type, object> Repositories { get; private set; }
        public DbContext DbContext { get; set; }


        public RepositoryProvider(RepositoryFactories repositoryFactories)
        {
            _repositoryFactories = repositoryFactories;

            Repositories = new Dictionary<Type, object>();
        }


        public T GetRepository<T>(Func<DbContext, object> factory = null) where T : class
        {
            // Look for T dictionary cache under typeof(T).
            object repoObj;
            Repositories.TryGetValue(typeof(T), out repoObj);
            if (repoObj != null)
            {
                return (T)repoObj;
            }

            // Not found or null; make one, add to dictionary cache, and return it.
            return MakeRepository<T>(factory, DbContext);
        }

        protected virtual T MakeRepository<T>(Func<DbContext, object> factory, DbContext dbContext) where T : class
        {
            var f = factory ?? _repositoryFactories.GetRepositoryFactory<T>();
            if (f == null)
            {
                throw new NotImplementedException("No factory for repository type, " + typeof(T).FullName);
            }
            var repo = (T)f(dbContext);
            Repositories[typeof(T)] = repo;
            return repo;
        }

        public void SetRepository<T>(T repository)
        {
            Repositories[typeof(T)] = repository;
        }
    }
}
