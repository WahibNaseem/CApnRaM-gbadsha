using System;
using System.Web;
using System.Web.Caching;
using System.Collections;
using System.IO;
using System.Configuration;

namespace Core.Cache
{
    public static class CacheManager
    {
        /// <summary>
        /// Insert value into the cache using
        /// appropriate name/value pairs
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="o">Item to be cached</param>
        /// <param name="key">Name of item</param>
        public static void Add<T>(string key,T o)
        {
            CacheItemRemovedCallback dep = new CacheItemRemovedCallback(OnRemove);
            HttpRuntime.Cache.Insert(key, o, null, DateTime.Now.AddYears(1)
                //new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,/*ConfigurationData.CacheExpireAtHour*/4,0,0)
               , System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, dep);

        }

        public static void Add<T>(string key, T o,DateTime cacheTime)
        {
            CacheItemRemovedCallback dep = new CacheItemRemovedCallback(OnRemove);
            HttpRuntime.Cache.Insert(key, o, null, cacheTime
                //new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,/*ConfigurationData.CacheExpireAtHour*/4,0,0)
               , System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, dep);

        }

        // delegate method must have this input parameters
        public static void OnRemove(String k, Object v, CacheItemRemovedReason r)
        {

        }

        /// <summary>
        /// Remove item from cache
        /// </summary>
        /// <param name="key">Name of cached item</param>
        public static void Clear(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }

        /// <summary>
        /// Check for item in cache
        /// </summary>
        /// <param name="key">Name of cached item</param>
        /// <returns></returns>
        public static bool Exists(string key)
        {
            //return HttpContext.Current.Cache[key] != null;
            return HttpRuntime.Cache[key] != null;
        }

        /// <summary>
        /// Retrieve cached item
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Name of cached item</param>
        /// <param name="value">Cached value. Default(T) if item doesn't exist.</param>
        /// <returns>Cached item as type</returns>
        public static bool Get<T>(string key, out T value)
        {
            try
            {
                if (!Exists(key))
                {
                    value = default(T);
                    return false;
                }

                value = (T)HttpRuntime.Cache[key];
            }
            catch
            {
                value = default(T);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Remove all cached Item.
        /// </summary>
        public static void ClearAll()
        {
            IDictionaryEnumerator en = HttpRuntime.Cache.GetEnumerator();
            while (en.MoveNext())
            {
                HttpRuntime.Cache.Remove(en.Key.ToString());
            }
        }
    }

}