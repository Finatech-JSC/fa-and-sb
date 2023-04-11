using MicroBase.Entity;
using MicroBase.Entity.Repositories;
using MicroBase.Share.Extensions;
using MicroBase.RedisProvider;
using MicroBase.Share;
using MicroBase.Share.DataAccess;
using MicroBase.Share.Linqkit;
using MicroBase.Share.Models;
using MicroBase.Share.Models.Emails;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Text;
using MicroBase.Share.Constants;

namespace MicroBase.Service.Emails
{
    public interface IEmailTemplateService : IGenericService<EmailTemplate, Guid>
    {
        Task<TPaging<EmailTemplateResponse>> GetAvailableAsync(Expression<Func<EmailTemplate, bool>> predicate,
            int pageIndex,
            int pageSize);

        Task<EmailTemplateResponse> GetByKeyAsync(string templateKey, string cultureCode);

        Task BuildToMemoryCacheAsync();

        string BuildMailContent(string orginContent, Dictionary<string, string> tokens);
    }

    public class EmailTemplateService : GenericService<EmailTemplate, Guid>, IEmailTemplateService
    {
        private readonly ILogger<EmailTemplateService> logger;
        private readonly IRedisStogare redisStogare;
        private readonly ISiteSettingService siteSettingService;

        public EmailTemplateService(IRepository<EmailTemplate, Guid> repository,
            ILogger<EmailTemplateService> logger,
            IRedisStogare redisStogare,
            ISiteSettingService siteSettingService) : base(repository)
        {
            this.logger = logger;
            this.redisStogare = redisStogare;
            this.siteSettingService = siteSettingService;
        }

        public EmailTemplateService(IRepository<EmailTemplate, Guid> repository) : base(repository)
        {
        }

        protected override void ApplyDefaultSort(FindOptions<EmailTemplate> findOptions)
        {
            findOptions.SortDescending(s => s.Key);
        }

        public async Task<TPaging<EmailTemplateResponse>> GetAvailableAsync(Expression<Func<EmailTemplate, bool>> predicate,
            int pageIndex,
            int pageSize)
        {
            try
            {
                var findOptions = new FindOptions<EmailTemplate>
                {
                    Limit = pageSize,
                    Skip = (pageIndex - 1) * pageSize
                }.SortAscending(s => s.Key);

                predicate = predicate.And(s => s.IsDelete == false);
                var settings = await Repository.FindAsync(predicate, findOptions);
                var rows = await Repository.CountAsync(predicate);

                var notificationKeys = await siteSettingService.GetByKeyAsync<Dictionary<string, string>>(key: Constants.SiteSettings.Keys.EMAIL_SETTING_KEY,
                    fieldData: Constants.SiteSettings.Fields.StringValue,
                    isFromDb: true);

                var templateRes = new List<EmailTemplateResponse>();
                foreach (var temp in settings)
                {
                    var keyLabel = notificationKeys.ContainsKey(temp.Key)
                        ? notificationKeys[temp.Key]
                        : string.Empty;

                    templateRes.Add(new EmailTemplateResponse
                    {
                        Id = temp.Id,
                        Key = temp.Key,
                        KeyLabel = keyLabel,
                        CultureCode = temp.CultureCode,
                        Subject = temp.Subject
                    });
                }

                return new TPaging<EmailTemplateResponse>
                {
                    Source = templateRes,
                    TotalRecords = rows
                };

            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
                return new TPaging<EmailTemplateResponse>
                {
                    Source = new List<EmailTemplateResponse>(),
                    TotalRecords = 0
                };
            }
        }

        public async Task<EmailTemplateResponse> GetByKeyAsync(string templateKey, string cultureCode)
        {
            if (string.IsNullOrWhiteSpace(cultureCode))
            {
                cultureCode = Constants.CultureCode.Japanese;
            }

            var temp = await Repository.FindOneAsync(s => s.Key == templateKey 
                && s.CultureCode == cultureCode
                && !s.IsDelete);

            if (temp == null)
            {
                logger.LogError($"Email template not found, emailTemplate: {templateKey} cultureCode: {cultureCode}");
                return null;
            }

            return new EmailTemplateResponse
            {
                Id = temp.Id,
                Key = temp.Key,
                KeyLabel = ConstantsService.GetEmailKeyLabel(temp.Key),
                Subject = temp.Subject,
                Body = temp.Body,
                CultureCode = temp.CultureCode
            };
        }

        public async Task BuildToMemoryCacheAsync()
        {
            var templates = await Repository.FindAsync(s => true);
            if (templates == null && !templates.Any())
            {
                return;
            }

            var emailDics = new Dictionary<string, EmailTemplateResponse>();
            foreach (var item in templates)
            {
                var key = $"{item.CultureCode}:{item.Key}";
                if (!emailDics.ContainsKey(key))
                {
                    emailDics.Add($"{item.CultureCode}:{item.Key}", new EmailTemplateResponse
                    {
                        Id = item.Id,
                        Key = item.Key,
                        KeyLabel = ConstantsService.GetEmailKeyLabel(item.Key),
                        Subject = item.Subject,
                        Body = item.Body,
                        CultureCode = item.CultureCode
                    });
                }
            }

            await redisStogare.HSetAsync(MemoryCacheConstants.CacheKeys.EMAIL_TEMPLATE, emailDics);
        }

        public string BuildMailContent(string orginContent, Dictionary<string, string> tokens)
        {
            var strBuilder = new StringBuilder(orginContent);
            foreach (var item in tokens)
            {
                strBuilder = strBuilder.Replace(item.Key, item.Value);
            }

            return strBuilder.ToString();
        }
    }
}