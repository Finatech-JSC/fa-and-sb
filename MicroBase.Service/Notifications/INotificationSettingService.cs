using MicroBase.Entity;
using MicroBase.Entity.Notifications;
using MicroBase.Entity.Repositories;
using MicroBase.RedisProvider;
using MicroBase.Service.Localizations;
using MicroBase.Share;
using MicroBase.Share.Constants;
using MicroBase.Share.DataAccess;
using MicroBase.Share.Linqkit;
using MicroBase.Share.Models;
using MicroBase.Share.Models.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace MicroBase.Service.Notifications
{
    public interface INotificationSettingService : IGenericService<NotificationSetting, Guid>
    {
        Task<TPaging<NotificationSettingResponse>> GetAvailableAsync(Expression<Func<NotificationSetting, bool>> predicate,
            int pageIndex,
            int pageSize);

        Task<BaseResponse<NotificationSettingResponse>> GetByKeyAsync(string key);

        Task<NotificationMessage> GetByKeyAsync(string key, string cultureCode, bool fromCache = false);

        Task<NotificationMessage> GetNotificationAsync(string key, string cultureCode);
    }

    public class NotificationSettingService : GenericService<NotificationSetting, Guid>, INotificationSettingService
    {
        private readonly ILogger<NotificationSettingService> logger;
        private readonly MicroDbContext microDbContext;
        private readonly ILocalizationService localizationService;
        private readonly IRedisStogare redisStogare;

        public NotificationSettingService(IRepository<NotificationSetting, Guid> repository,
            ILogger<NotificationSettingService> logger,
            MicroDbContext microDbContext,
            ILocalizationService localizationService,
            IRedisStogare redisStogare) : base(repository)
        {
            this.logger = logger;
            this.microDbContext = microDbContext;
            this.localizationService = localizationService;
            this.redisStogare = redisStogare;
        }

        protected override void ApplyDefaultSort(FindOptions<NotificationSetting> findOptions)
        {
            findOptions.SortAscending(s => s.CreatedDate);
        }

        public async Task<TPaging<NotificationSettingResponse>> GetAvailableAsync(Expression<Func<NotificationSetting, bool>> predicate,
            int pageIndex,
            int pageSize)
        {
            try
            {
                predicate = predicate.And(s => s.IsDelete == false);

                var rows = await Repository.CountAsync(predicate);
                var records = await microDbContext.Set<NotificationSetting>()
                    .Where(predicate)
                    .OrderByDescending(s => s.CreatedDate)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var response = records.Select(s => new NotificationSettingResponse
                {
                    Id = s.Id,
                    Image = s.Image,
                    Content = s.Content,
                    CreatedBy = s.CreatedBy,
                    PushToMailBox = s.PushToMailBox,
                    RedirectTo = s.RedirectTo,
                    StartDate = s.StartDate,
                    ModifiedBy = s.ModifiedBy,
                    SubContent = s.SubContent,
                    Title = s.Title,
                    CreatedDate = s.CreatedDate,
                    ModifiedDate = s.ModifiedDate,
                    Enabled = s.Enabled,
                    IsSend = s.IsSend,
                    Key = s.Key,
                    RepeatDay = s.RepeatDay,
                    LatestSend = s.LatestSend,
                    ExtraParams = s.ExtraParams,
                    SendToAll = !s.SendManually,
                    RedirectType = s.RedirectType,
                    Link = s.Link
                });

                return new TPaging<NotificationSettingResponse>
                {
                    Source = response,
                    TotalRecords = rows
                };

            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
                return new TPaging<NotificationSettingResponse>
                {
                    Source = new List<NotificationSettingResponse>(),
                    TotalRecords = 0
                };
            }
        }

        public async Task<BaseResponse<NotificationSettingResponse>> GetByKeyAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return new BaseResponse<NotificationSettingResponse>
                {
                    Success = false,
                    Message = CommonMessage.RECORD_NOT_FOUND
                };
            }

            var entity = await Repository.FindOneAsync(s => s.Key != null && s.Key == key.ToUpper());
            if (entity == null)
            {
                return new BaseResponse<NotificationSettingResponse>
                {
                    Success = false,
                    Message = CommonMessage.RECORD_NOT_FOUND
                };
            }

            var prefix = $"{LocalizationConstants.Prefix.NOTIFICATION_SETTING}:{entity.Id}";
            return new BaseResponse<NotificationSettingResponse>
            {
                Success = true,
                Data = new NotificationSettingResponse
                {
                    Id = entity.Id,
                    Image = entity.Image,
                    Title = localizationService.GetLocalizationString(LocalizationConstants.MessageModelKey.TITLE, null, prefix, entity.Title),
                    SubContent = localizationService.GetLocalizationString(LocalizationConstants.MessageModelKey.SUB_CONTENT, null, prefix, entity.SubContent),
                    Content = localizationService.GetLocalizationString(LocalizationConstants.MessageModelKey.CONTENT, null, prefix, entity.Content),
                    CreatedBy = entity.CreatedBy,
                    PushToMailBox = entity.PushToMailBox,
                    RedirectTo = entity.RedirectTo,
                    StartDate = entity.StartDate,
                    ModifiedBy = entity.ModifiedBy,
                    CreatedDate = entity.CreatedDate,
                    ModifiedDate = entity.ModifiedDate,
                    Enabled = entity.Enabled,
                    IsSend = entity.IsSend,
                    RedirectType = entity.RedirectType,
                    Link = entity.Link
                }
            };
        }

        public async Task<NotificationMessage> GetByKeyAsync(string key, string cultureCode, bool fromCache)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            if (fromCache)
            {
                var notification = await GetNotificationAsync(key, cultureCode);
                if (notification != null)
                {
                    return notification;
                }
            }

            var entity = await Repository.FindOneAsync(s => s.Key != null && s.Key == key.ToUpper());
            if (entity == null)
            {
                return null;
            }

            var prefix = $"{LocalizationConstants.Prefix.NOTIFICATION_SETTING}:{entity.Id}";
            return new NotificationMessage
            {
                Id = entity.Id,
                Image = entity.Image,
                Title = localizationService.GetLocalizationString(LocalizationConstants.MessageModelKey.TITLE, null, prefix, entity.Title, cultureCode),
                Description = localizationService.GetLocalizationString(LocalizationConstants.MessageModelKey.SUB_CONTENT, null, prefix, entity.SubContent, cultureCode),
                Content = localizationService.GetLocalizationString(LocalizationConstants.MessageModelKey.CONTENT, null, prefix, entity.Content, cultureCode),
                PushToMailBox = entity.PushToMailBox,
                RedirectTo = entity.RedirectTo,
                RedirectType = entity.RedirectType,
                ExtraParams = entity.ExtraParams,
                Link = entity.Link
            };
        }

        public async Task<NotificationMessage> GetNotificationAsync(string key, string cultureCode)
        {
            var notification = await redisStogare.HGetAsync<NotificationMessage>(MemoryCacheConstants.CacheKeys.NOTIFICATIONS, key);
            if (notification == null)
            {
                return null;
            }

            var prefix = $"{LocalizationConstants.Prefix.NOTIFICATION_SETTING}:{notification.Id}";

            return new NotificationMessage
            {
                Id = notification.Id,
                Content = localizationService.GetLocalizationString(LocalizationConstants.MessageModelKey.CONTENT, null, prefix, notification.Content, cultureCode),
                Description = localizationService.GetLocalizationString(LocalizationConstants.MessageModelKey.SUB_CONTENT, null, prefix, notification.Description, cultureCode),
                Title = localizationService.GetLocalizationString(LocalizationConstants.MessageModelKey.TITLE, null, prefix, notification.Title, cultureCode),
                ExtraParams = notification.ExtraParams,
                Image = notification.Image,
                Key = notification.Key,
                PushToMailBox = notification.PushToMailBox,
                RedirectTo = notification.RedirectTo,
                RedirectType = notification.RedirectType
            };
        }
    }
}