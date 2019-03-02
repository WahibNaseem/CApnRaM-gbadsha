using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;


namespace JK.Repository
{
    /// <summary>
    /// The EF-dependent, generic repository for data access
    /// </summary>
    /// <typeparam name="T">Type of entity for this Repository.</typeparam>
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        public BaseRepository(DbContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException("dbContext");
            DbContext = dbContext;
            DbSet = DbContext.Set<T>();
        }

        protected DbContext DbContext { get; set; }

        protected DbSet<T> DbSet { get; set; }

        public virtual IQueryable<T> GetAll(Boolean noTrack = false)
        {
            return noTrack ? DbSet.AsNoTracking() : DbSet;
        }

        public virtual T GetById(int id)
        {
            return DbSet.Find(id);
        }

        public virtual T GetById(long id)
        {
            return DbSet.Find(id);
        }

        public virtual T GetByStringKey(string key)
        {
            return DbSet.Find(key);
        }

        public ObjectResult<T> StoredProc<T>(string pName, params ObjectParameter[] pParams)
        {
            return ((IObjectContextAdapter)this.DbContext).ObjectContext.ExecuteFunction<T>(pName, pParams);

        }
        public ObjectResult<T> StoredQuery<T>(string pName, params ObjectParameter[] pParams)
        {
            return ((IObjectContextAdapter)this.DbContext).ObjectContext.ExecuteStoreQuery<T>(pName, pParams);

        }

        public virtual void Add(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                DbSet.Add(entity);
            }
        }

        public virtual void Update(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State == EntityState.Modified)
            {
                DbSet.Attach(entity);
            }
            dbEntityEntry.State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Deleted)
            {
                dbEntityEntry.State = EntityState.Deleted;
            }
            else
            {
                DbSet.Attach(entity);
                DbSet.Remove(entity);
            }
        }

        public virtual void Delete(int id)
        {
            var entity = GetById(id);
            if (entity == null) return; // not found; assume already deleted.
            Delete(entity);
        }

        public virtual void DeleteByStringKey(string key)
        {
            var entity = GetByStringKey(key);
            if (entity == null) return;
            Delete(entity);
        }
    }
}
