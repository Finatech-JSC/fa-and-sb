using MicroBase.Entity;
using MicroBase.Entity.Repositories;
using MicroBase.RedisProvider;
using MicroBase.Share;
using MicroBase.Share.Constants;
using MicroBase.Share.DataAccess;
using MicroBase.Share.Extensions;
using MicroBase.Share.Linqkit;
using MicroBase.Share.Models;
using MicroBase.Share.Models.MobileApps;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace MicroBase.Service
{
    public interface ISiteSettingService : IGenericService<SiteSetting, Guid>
    {
        Task<TPaging<SiteSettingResponse>> GetAvailableAsync(Expression<Func<SiteSetting, bool>> predicate,
            int pageIndex,
            int pageSize);

        Task<SiteSettingResponse> GetByKeyAsync(string key, bool isFromDb = false);

        Task<BaseResponse<object>> AddOrUpdateAsync(IList<SiteSetting> settings);

        Task<IEnumerable<MobileScreenModel>> GetMobileScreensAsync();

        Task<IEnumerable<SelectListItem>> GetMobileScreenDropdownAsync(string selected);

        Task<T> GetByKeyAsync<T>(string key, Constants.SiteSettings.Fields fieldData, bool isFromDb = false);

        Task<TPaging<SiteSettingResponse>> GetGroupKeysAsync(Expression<Func<SiteSetting, bool>> predicate,
            int pageIndex,
            int pageSize);

        Task<IEnumerable<SiteSettingResponse>> GetByGroupKeysAsync(string groupKey, bool fromCache = false);

        Task AddToCacheAsync<CacheModel>(string key, CacheModel model);

        Task RemoveFromCacheAsync(string key);

        Task BuildSiteSettingToCachAsync();
    }

    public class SiteSettingService : GenericService<SiteSetting, Guid>, ISiteSettingService
    {
        private readonly ILogger<SiteSettingService> logger;
        private readonly IRedisStogare redisStogare;

        public SiteSettingService(IRepository<SiteSetting, Guid> repository,
            ILogger<SiteSettingService> logger,
            IRedisStogare redisStogare)
            : base(repository)
        {
            this.logger = logger;
            this.redisStogare = redisStogare;
        }

        protected override void ApplyDefaultSort(FindOptions<SiteSetting> findOptions)
        {
            findOptions.SortAscending(s => s.Key);
        }

        public async Task<TPaging<SiteSettingResponse>> GetAvailableAsync(Expression<Func<SiteSetting, bool>> predicate,
            int pageIndex,
            int pageSize)
        {
            try
            {
                var findOptions = new FindOptions<SiteSetting>
                {
                    Limit = pageSize,
                    Skip = (pageIndex - 1) * pageSize
                }.SortAscending(s => s.Key);

                predicate = predicate.And(s => s.IsDelete == false && s.IsSecret == false);
                var settings = await Repository.FindAsync(predicate, findOptions);
                var rows = await Repository.CountAsync(predicate);

                var response = settings.Select(s => new SiteSettingResponse
                {
                    Id = s.Id,
                    Key = s.Key,
                    BoolValue = s.BoolValue,
                    StringValue = s.StringValue,
                    NumberValue = s.NumberValue,
                    GroupKey = s.GroupKey,
                    Order = s.Order
                });

                return new TPaging<SiteSettingResponse>
                {
                    Source = response,
                    TotalRecords = rows
                };

            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
                return new TPaging<SiteSettingResponse>
                {
                    Source = new List<SiteSettingResponse>(),
                    TotalRecords = 0
                };
            }
        }

        public async Task<SiteSettingResponse> GetByKeyAsync(string key, bool isFromDb = false)
        {
            if (!isFromDb)
            {
                var cacheValue = await redisStogare.GetAsync<SiteSettingResponse>($"{MemoryCacheConstants.CacheKeys.SITE_SETTING}:{key}");
                if (cacheValue != null)
                {
                    return cacheValue;
                }
            }

            var record = await Repository.FindOneAsync(s => s.Key.Equals(key));
            if (record == null)
            {
                return new SiteSettingResponse();
            }

            return new SiteSettingResponse
            {
                Id = record.Id,
                Key = record.Key,
                BoolValue = record.BoolValue,
                StringValue = record.StringValue,
                NumberValue = record.NumberValue
            };
        }

        public async Task<BaseResponse<object>> AddOrUpdateAsync(IList<SiteSetting> settings)
        {
            try
            {
                var message = string.Empty;
                foreach (var item in settings.Where(s => !string.IsNullOrWhiteSpace(s.Key)))
                {
                    var setting = await Repository.FindOneAsync(s => s.Key.Equals(item.Key));
                    if (setting == null)
                    {
                        await Repository.InsertAsync(item);
                        message = CommonMessage.INSERT_SUCCESS;
                    }
                    else
                    {
                        setting.BoolValue = item.BoolValue;
                        setting.StringValue = item.StringValue;
                        setting.NumberValue = item.NumberValue;
                        setting.ModelField = item.ModelField;
                        setting.ModelFieldIsArray = item.ModelFieldIsArray;

                        await Repository.UpdateAsync(setting);

                        message = CommonMessage.UPDATE_SUCCESS;
                    }

                    var cacheModel = new SiteSettingResponse
                    {
                        Id = item.Id,
                        Key = item.Key,
                        GroupKey = item.GroupKey,
                        BoolValue = item.BoolValue,
                        NumberValue = item.NumberValue,
                        StringValue = item.StringValue,
                        Order = item.Order,
                        ModelFields = item.ModelField,
                        ModelFieldIsArray = item.ModelFieldIsArray
                    };

                    if (string.IsNullOrWhiteSpace(item.GroupKey))
                    {
                        await redisStogare
                            .SetAsync($"{MemoryCacheConstants.CacheKeys.SITE_SETTING}:{item.Key}", cacheModel);
                    }
                    else
                    {
                        await redisStogare
                            .HSetAsync($"{MemoryCacheConstants.CacheKeys.SITE_SETTING}:{item.GroupKey}", item.Key, cacheModel);
                    }
                }

                return new BaseResponse<object>
                {
                    Success = true,
                    Message = message
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return new BaseResponse<object>
                {
                    Success = false,
                    Message = CommonMessage.UN_DETECTED_ERROR
                };
            }
        }

        public async override Task<SiteSetting> SoftDeleteAsync(Guid id)
        {
            var record = await Repository.GetByIdAsync(id);
            if (record == null)
            {
                return null;
            }

            record.IsDelete = true;
            record.ModifiedDate = DateTime.UtcNow;
            await Repository.UpdateAsync(record);

            if (string.IsNullOrWhiteSpace(record.GroupKey))
            {
                await redisStogare.KeyDelAsync($"{MemoryCacheConstants.CacheKeys.SITE_SETTING}:{record.Key}");
            }
            else
            {
                await redisStogare.HDelAsync($"{MemoryCacheConstants.CacheKeys.SITE_SETTING}:{record.GroupKey}", new List<string> { record.Key });
            }

            return record;
        }

        public async Task<IEnumerable<MobileScreenModel>> GetMobileScreensAsync()
        {
            var screens = await GetByKeyAsync<IEnumerable<MobileScreenModel>>(key: Constants.SiteSettings.Keys.MOBILE_SCREEN,
                fieldData: Constants.SiteSettings.Fields.StringValue,
                isFromDb: true);

            return screens;
        }

        public async Task<IEnumerable<SelectListItem>> GetMobileScreenDropdownAsync(string selected)
        {
            var screens = await GetByKeyAsync<IEnumerable<MobileScreenModel>>(key: Constants.SiteSettings.Keys.MOBILE_SCREEN,
                fieldData: Constants.SiteSettings.Fields.StringValue,
                isFromDb: true);

            if (screens == null)
            {
                return Enumerable.Empty<SelectListItem>();
            }

            return screens.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Name,
                Selected = s.Name == selected
            });
        }

        /// <summary>
        /// In case T is a collection please use List<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="fieldData"></param>
        /// <param name="isFromDb"></param>
        /// <returns></returns>
        public async Task<T> GetByKeyAsync<T>(string key, Constants.SiteSettings.Fields fieldData, bool isFromDb = false)
        {
            SiteSettingResponse record = null;
            if (!isFromDb)
            {
                record = await redisStogare.GetAsync<SiteSettingResponse>($"{MemoryCacheConstants.CacheKeys.SITE_SETTING}:{key}");
            }

            if (record == null)
            {
                //logger.LogError($"Get from DATABASE cache GetByKeyAsync key: {key}");
                var entity = await Repository.FindOneAsync(s => s.Key.Equals(key));
                if (entity == null)
                {
                    return (T)Activator.CreateInstance(typeof(T));
                }
                else
                {
                    record = new SiteSettingResponse
                    {
                        Id = entity.Id,
                        Key = entity.Key,
                        BoolValue = entity.BoolValue,
                        StringValue = entity.StringValue,
                        NumberValue = entity.NumberValue
                    };
                }
            }

            if (record == null)
            {
                return (T)Activator.CreateInstance(typeof(T));
            }

            switch (fieldData)
            {
                case Constants.SiteSettings.Fields.BoolValue:
                    if (record.BoolValue.HasValue)
                    {
                        return (T)Convert.ChangeType(record.BoolValue.Value, typeof(T));
                    }
                    else
                    {
                        record.BoolValue = false;
                        return (T)Convert.ChangeType(record.BoolValue.Value, typeof(T));
                    }

                case Constants.SiteSettings.Fields.NumberValue:
                    if (record.NumberValue.HasValue)
                    {
                        return (T)Convert.ChangeType(record.NumberValue.Value, typeof(T));
                    }
                    else
                    {
                        record.NumberValue = 0;
                        return (T)Convert.ChangeType(record.NumberValue.Value, typeof(T));
                    }

                case Constants.SiteSettings.Fields.StringValue:
                    if (!string.IsNullOrWhiteSpace(record.StringValue))
                    {
                        if (typeof(T) == typeof(string))
                        {
                            return (T)Convert.ChangeType(record.StringValue, typeof(T));
                        }
                        else
                        {
                            return JsonExtensions.JsonDeserialize<T>(record.StringValue);
                        }
                    }
                    else
                    {
                        return (T)Activator.CreateInstance(typeof(T));
                    }

                default:
                    return (T)Activator.CreateInstance(typeof(T));
            }
        }

        public async Task<TPaging<SiteSettingResponse>> GetGroupKeysAsync(Expression<Func<SiteSetting, bool>> predicate,
            int pageIndex,
            int pageSize)
        {
            try
            {
                var findOptions = new FindOptions<SiteSetting>
                {
                    Limit = int.MaxValue,
                    Skip = 0
                }.SortAscending(s => s.Order);

                predicate = predicate.And(s => !s.IsDelete && (s.GroupKey != null || s.GroupKey != ""));
                var settings = await Repository.FindAsync(predicate, findOptions);
                var response = settings.GroupBy(s => s.GroupKey)
                    .Where(s => s.Key != null)
                    .Select(s => new SiteSettingResponse
                    {
                        GroupKey = s.Key
                    });

                return new TPaging<SiteSettingResponse>
                {
                    Source = response
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
                return new TPaging<SiteSettingResponse>
                {
                    Source = new List<SiteSettingResponse>(),
                    TotalRecords = 0
                };
            }
        }

        public async Task<IEnumerable<SiteSettingResponse>> GetByGroupKeysAsync(string groupKey, bool fromCache = false)
        {
            if (string.IsNullOrWhiteSpace(groupKey))
            {
                return new List<SiteSettingResponse>();
            }

            try
            {
                if (fromCache)
                {
                    var caches = await redisStogare
                        .HGetAllAsync<SiteSettingResponse>($"{MemoryCacheConstants.CacheKeys.SITE_SETTING}:{groupKey}");
                    if (caches != null && caches.Any())
                    {
                        return caches;
                    }
                }

                var predicate = PredicateBuilder.Create<SiteSetting>(u => !u.IsDelete && u.GroupKey == groupKey.ToUpper());
                predicate = predicate.And(s => !s.IsDelete);

                var settings = await Repository.FindAsync(predicate);
                return settings.Select(s => new SiteSettingResponse
                {
                    Id = s.Id,
                    GroupKey = s.GroupKey,
                    Key = s.Key,
                    BoolValue = s.BoolValue,
                    NumberValue = s.NumberValue,
                    StringValue = s.StringValue,
                    Order = s.Order
                });
            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
                return new List<SiteSettingResponse>();
            }
        }

        public async Task AddToCacheAsync<CacheModel>(string key, CacheModel model)
        {
            await redisStogare
                .SetAsync($"{MemoryCacheConstants.CacheKeys.SITE_SETTING}:{key}", model);
        }

        public async Task RemoveFromCacheAsync(string key)
        {
            await redisStogare.HDelAsync(MemoryCacheConstants.CacheKeys.SITE_SETTING, new List<string> { key });
        }

        public async Task BuildSiteSettingToCachAsync()
        {
            var records = await Repository.FindAsync(s => !s.IsDelete);
            if (!records.Any())
            {
                return;
            }

            var groups = records.GroupBy(s => s.GroupKey);
            foreach (var gr in groups)
            {
                if (string.IsNullOrWhiteSpace(gr.Key))
                {
                    foreach (var item in gr)
                    {
                        var cacheModel = new SiteSettingResponse
                        {
                            Id = item.Id,
                            Key = item.Key,
                            GroupKey = item.GroupKey,
                            BoolValue = item.BoolValue,
                            NumberValue = item.NumberValue,
                            StringValue = item.StringValue,
                            Order = item.Order
                        };

                        var cacheKey = $"{MemoryCacheConstants.CacheKeys.SITE_SETTING}:{item.Key}";

                        await redisStogare.SetAsync(cacheKey, cacheModel);
                    }
                }
                else
                {
                    var caches = gr.Select(s => new SiteSettingResponse
                    {
                        Id = s.Id,
                        Key = s.Key,
                        GroupKey = s.GroupKey,
                        BoolValue = s.BoolValue,
                        NumberValue = s.NumberValue,
                        StringValue = s.StringValue,
                        Order = s.Order
                    });

                    var cacheModel = new SiteSettingResponse
                    {
                        GroupKey = gr.Key,
                        StringValue = caches.JsonSerialize(),
                    };

                    var cacheKey = $"{MemoryCacheConstants.CacheKeys.SITE_SETTING}:{gr.Key}";

                    await redisStogare.SetAsync(cacheKey, cacheModel);
                }
            }
        }
    }
}