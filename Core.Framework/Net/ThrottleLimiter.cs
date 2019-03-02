using Core.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core.Net
{
    public static class ThrottleLimiter
    {
        static object locker = new object();

        /// <summary>
        /// This method is Called with every request.
        /// </summary>
        /// <param name="userToken">Unique key for the user</param>
        /// <param name="allowedHits">Number of allowed hits for this user</param>
        /// <returns>True, if hits not exceeded. otherwise, it will be false.</returns>
        public static bool AllowedHit(string userToken, int allowedHits)
        {
            lock (locker)
            {
                int hitsCount = 0;
                DateTime cacheTime = DateTime.UtcNow;
                userToken = "ThrottleLimiter_" + userToken;
                cacheTime = cacheTime.AddSeconds(1);

                // Cache key doesn't exist
                if (!CacheManager.Get(userToken, out hitsCount))
                {
                    hitsCount++;
                    CacheManager.Add(userToken, hitsCount, cacheTime);
                    return true;
                }
                else
                {
                    if (hitsCount < allowedHits)
                    {
                        hitsCount++;
                        CacheManager.Add(userToken, hitsCount, cacheTime);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
    }
}