using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using System.Configuration;
using JK.Resources;

namespace JKApi.Core
{
    public class CacheProvider : ICacheProvider
    {
        private static readonly Lazy<CacheProvider> Lazy = new Lazy<CacheProvider>(() => new CacheProvider());
        public static CacheProvider Instance => Lazy.Value;

        /// <summary>
        /// Object of MemoryCache
        /// </summary>
        private MemoryCache _memcache;
        private string _cachingExpireType;
        string ICacheProvider.CachingExpireType
        {
            get
            {
                return _cachingExpireType;
            }
            set
            {
                _cachingExpireType = value;
            }
        }
        /// <summary>
        /// Instance of MemoryCache .
        /// </summary>

        private MemoryCache _CachingInstance
        {
            get
            {
                if (_memcache == null)
                    _memcache = MemoryCache.Default;

                return _memcache;
            }
        }



        /// <summary>
        /// Get expire time from configuration.
        /// </summary>

        private static double _ExpiresIn { get { return Convert.ToDouble(WebConfigResource.CacheExpiresIn_seconds); } }

        /// <summary>
        /// Get cache by key.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <returns></returns>
        public object Get(string cacheKey)
        {
            return _CachingInstance.Get(cacheKey);
        }

        /// <summary>
        /// Set cache based on key
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="cacheValue">The cache value.</param>
        public void Set(string cacheKey, object cacheValue)
        {
            _CachingInstance.Set(cacheKey, cacheValue, DateTimeOffset.UtcNow.AddSeconds(_ExpiresIn));
        }


        /// <summary>
        /// Determines whether [contains] [the specified cache key].
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <returns></returns>
        public bool Contains(string cacheKey)
        {
            return _CachingInstance.Contains(cacheKey);
        }


        /// <summary>
        /// Removes specific cache by key.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        public void Remove(string cacheKey)
        {
            if (_CachingInstance.Contains(cacheKey))
                _CachingInstance.Remove(cacheKey);
        }

        public void RemoveAll()
        {
            List<string> cacheKeys = _CachingInstance.Select(kvp => kvp.Key).ToList();
            foreach (string cacheKey in cacheKeys)
            {
                _CachingInstance.Remove(cacheKey);
            }
        }

    }

}
