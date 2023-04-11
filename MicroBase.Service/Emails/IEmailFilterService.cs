using MicroBase.RedisProvider;
using MicroBase.Share;
using MicroBase.Share.Extensions;
using MicroBase.Share.Models.Emails;

namespace MicroBase.Service.Emails
{
    public interface IEmailFilterService
    {
        Task<bool> IsAcceptEmailAsync(string emailAddress);

        Task<EmailFilterCacheModel> GetEmailFilterAsync();
    }

    public class EmailFilterService : IEmailFilterService
    {
        private readonly IRedisStogare redisStogare;

        public EmailFilterService(IRedisStogare redisStogare)
        {
            this.redisStogare = redisStogare;
        }

        public async Task<bool> IsAcceptEmailAsync(string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                return false;
            }

            var isEmailAddress = emailAddress.IsEmailAddress();
            if (!isEmailAddress)
            {
                return false;
            }

            var mailFilters = await GetEmailFilterAsync();
            if (mailFilters == null)
            {
                return true;
            }

            var mailParts = emailAddress.Split('@');
            if (!mailParts.Any() || mailParts.Length != 2)
            {
                return false;
            }

            var isInBlackList = IsInBlackList(emailAddress, mailFilters.BlackList, mailParts[1]);
            if (isInBlackList)
            {
                return false;
            }

            var isInWhiteListAsync = IsInWhiteListAsync(emailAddress, mailFilters.WhiteList, mailParts[1]);
            if (!isInWhiteListAsync)
            {
                return false;
            }

            return true;
        }

        private bool IsInBlackList(string emailAddress, IEnumerable<string> blacklistEmails, string emailExtension)
        {
            if (blacklistEmails == null || !blacklistEmails.Any())
            {
                return false;
            }

            if (blacklistEmails.ContainsAny(emailExtension))
            {
                return true;
            }

            return false;
        }

        private bool IsInWhiteListAsync(string emailAddress, IEnumerable<string> whiteList, string emailExtension)
        {
            if (whiteList == null || !whiteList.Any() || whiteList.ContainsAny(emailExtension))
            {
                return true;
            }

            if (whiteList.ContainsAny(emailExtension))
            {
                return true;
            }

            return false;
        }

        public async Task<EmailFilterCacheModel> GetEmailFilterAsync()
        {
            var cacheVal = await redisStogare.GetAsync<EmailFilterCacheModel>(MemoryCacheConstants.CacheKeys.EMAIL_FILTER);
            return cacheVal;
        }
    }
}