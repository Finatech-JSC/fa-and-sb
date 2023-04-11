using MicroBase.Share.Extensions;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroBase.RedisProvider
{
    public interface IRedisStogare
    {
        /// <summary>
        /// Save T model to redis
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="cache"></param>
        /// <param name="ttl"></param>
        /// <returns></returns>
        Task<bool> SetAsync<T>(string key, T cache, TimeSpan? ttl = null);

        /// <summary>
        /// Get from redis return T model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// Add a row redis hashset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        Task<bool> HSetAsync<T>(string key, string field, T cache);

        /// <summary>
        /// Add multiple rows to redis hashset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="caches"></param>
        /// <returns></returns>
        Task<bool> HSetAsync<T>(string key, Dictionary<string, T> caches);

        /// <summary>
        /// Get a row from redis hashset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        Task<T> HGetAsync<T>(string key, string field);

        /// <summary>
        /// Get multiple rows from redis hashset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> HGetAsync<T>(string key, List<string> fields);

        /// <summary>
        /// Get multiple rows from redis hashset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> HGetAllAsync<T>(string key);

        /// <summary>
        /// Remove by ke
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> KeyDelAsync(string key);

        Task<bool> HDelAsync(string key, List<string> fields);

        Task<bool> HExistsAsync(string key, string value);
    }

    public class RedisStogare : IRedisStogare
    {
        private readonly IDatabase database;

        public RedisStogare()
        {
            database = RedisConnectionFactory.GetDatabase();
        }

        public async Task<bool> StringSetAsync(string key, string cache, TimeSpan? ttl)
        {
            if (database == null)
            {
                return false;
            }

            try
            {
                return await database.StringSetAsync(key, cache, ttl);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> StringGetAsync(string key)
        {
            if (database == null)
            {
                return string.Empty;
            }

            try
            {
                var val = await database.StringGetAsync(key);
                return val;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> SetAsync<T>(string key, T cache, TimeSpan? ttl = null)
        {
            if (database == null)
            {
                return false;
            }

            try
            {
                return await database.StringSetAsync(key, cache.JsonSerialize(), ttl);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T> GetAsync<T>(string key)
        {
            if (database == null)
            {
                return default(T);
            }

            try
            {
                var val = await database.StringGetAsync(key);
                if (val.IsNull)
                {
                    return default(T);
                }

                return JsonExtensions.JsonDeserialize<T>(val);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> HSetAsync<T>(string key, string field, T cache)
        {
            if (database == null)
            {
                return false;
            }

            try
            {
                return await database.HashSetAsync(key, field, cache.JsonSerialize());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> HSetAsync<T>(string key, Dictionary<string, T> caches)
        {
            if (database == null)
            {
                return false;
            }

            try
            {
                var hashEntries = caches.Select(s => new HashEntry(s.Key, s.Value.JsonSerialize())).ToArray();
                await database.HashSetAsync(key, hashEntries);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T> HGetAsync<T>(string key, string field)
        {
            if (database == null)
            {
                return default(T);
            }

            try
            {
                var val = await database.HashGetAsync(key, field);
                if (val.IsNull)
                {
                    return default(T);
                }

                return JsonExtensions.JsonDeserialize<T>(val);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<T>> HGetAsync<T>(string key, List<string> fields)
        {
            if (database == null)
            {
                return default;
            }

            try
            {
                var res = new List<T>();
                foreach (var item in fields)
                {
                    var val = await database.HashGetAsync(key, item);
                    if (!val.IsNull)
                    {
                        res.Add(JsonExtensions.JsonDeserialize<T>(val));
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<T>> HGetAllAsync<T>(string key)
        {
            if (database == null)
            {
                return default;
            }

            try
            {
                var caches = await database.HashGetAllAsync(key);
                var res = new List<T>();
                foreach (var item in caches)
                {
                    res.Add(JsonExtensions.JsonDeserialize<T>(item.Value));
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> KeyDelAsync(string key)
        {
            if (database == null)
            {
                return false;
            }

            try
            {
                return await database.KeyDeleteAsync(key);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> HExistsAsync(string key, string value)
        {
            if (database == null)
            {
                return false;
            }

            try
            {
                return await database.HashExistsAsync(key, value);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> HDelAsync(string key, List<string> fields)
        {
            if (database == null)
            {
                return false;
            }

            try
            {
                foreach (var item in fields)
                {
                    await database.HashDeleteAsync(key, item);
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
