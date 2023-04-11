using MicroBase.RedisProvider;
using MicroBase.Share;
using MicroBase.Share.Constants;
using MicroBase.Share.Models;

namespace MicroBase.Service
{
    public interface ILanguageFilterService
    {
        Task<string> ValidateLanguageAsync(string str);

        Task BuildLanguageFilterAsync();
    }

    public class LanguageFilterService : ILanguageFilterService
    {
        private readonly IRedisStogare redisStogare;
        private readonly ISiteSettingService siteSettingService;

        public LanguageFilterService(IRedisStogare redisStogare, 
            ISiteSettingService siteSettingService)
        {
            this.redisStogare = redisStogare;
            this.siteSettingService = siteSettingService;
        }

        public async Task<string> ValidateLanguageAsync(string str)
        {

            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }
           
            var languageFilters = await GetLanguageFilterAsync();
            if (languageFilters == null)
            {
                await BuildLanguageFilterAsync();
                languageFilters = await GetLanguageFilterAsync();
            }
          
            var inBlackList = InBlackList(str, languageFilters.BlackList);
            return inBlackList;
        }

        private string InBlackList(string str, IEnumerable<string> blacklistLanguages)
        {
            var q = blacklistLanguages.FirstOrDefault(w => str.Contains(w));
            return q;
        }

        public async Task BuildLanguageFilterAsync()
        {
            var languageFilters = await siteSettingService.GetByKeyAsync<SwearLanguageFilterModel>(key: Constants.SiteSettings.Keys.SWEAR_LANGUAGE_FILTER,
                fieldData: Constants.SiteSettings.Fields.StringValue,
                isFromDb: true);

            if (languageFilters == null)
            {
                return;
            }

            var languageFilterCacheModel = new SwearLanguageCacheModel();
            if (!string.IsNullOrWhiteSpace(languageFilters.BlackList))
            {
                languageFilterCacheModel.BlackList = languageFilters.BlackList.Split("\r\n");
            }

            await redisStogare.SetAsync(MemoryCacheConstants.CacheKeys.LANGUAGE_FILTER, languageFilterCacheModel);
        }

        public async Task<SwearLanguageCacheModel> GetLanguageFilterAsync()
        {
            var cacheVal = await redisStogare
                .GetAsync<SwearLanguageCacheModel>(MemoryCacheConstants.CacheKeys.LANGUAGE_FILTER);

            return cacheVal;
        }
    }
}
