using System;
using System.Data.Entity;

namespace JK.Repository.Helpers
{
    public interface IRepositoryProvider
    {
        /// <summary>
        /// Interface for a class that can provide repositories by type.
        /// The class may create the repositories dynamically if it is unable
        /// to find one in its cache of repositories.
        /// </summary>
        /// <remarks>
        /// Repositories created by this provider tend to require a <see cref="DbContext"/>
        /// to retrieve data.
        /// </remarks>
        DbContext DbContext { get; set; }

        T GetRepository<T>(Func<DbContext, object> factory = null) where T : class;
        void SetRepository<T>(T repository);
    }
}
