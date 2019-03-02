using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Core
{
    public interface ICacheProvider
    {
        //T Get<T>(string cacheKey);

        //void Set<T>(string cacheKey, T cacheValue);
      

        /// <summary>
        /// Get cache by key.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <returns></returns>
        object Get(string cacheKey);
        /// <summary>
        /// Set cache based on key
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="cacheValue">The cache value.</param>
        void Set(string cacheKey, object cacheValue);
        /// <summary>
        /// Determines whether [contains] [the specified cache key].
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <returns></returns>
        bool Contains(string cacheKey);
        void Remove(string cacheKey);

        string CachingExpireType { get; set; }
    }
     
}