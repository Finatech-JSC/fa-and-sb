using Microsoft.Extensions.Logging;
using System.Text;
using MicroBase.Share.Models.Notifications;
using MicroBase.Share.Models.Notifications.OneSignal;
using MicroBase.Share.Models;
using MicroBase.Share.Extensions;
using MicroBase.Share;
using MicroBase.Service.Localizations;
using MicroBase.Share.Constants;

namespace MicroBase.Service.Notifications
{
    public interface IOneSignalService
    {
        Task<BaseResponse<object>> PushNotificationsAsync(OneSignalSettingModel setting,
            NotificationModel notification,
            IEnumerable<string> connectionIds);

        Task<BaseResponse<object>> PushNotificationsToAllAsync(OneSignalSettingModel setting,
            NotificationModel notification);
    }

    public class OneSignalService : IOneSignalService
    {
        private readonly ILogger<OneSignalService> logger;
        private readonly HttpClient httpClient;
        private readonly ILocalizationService localizationService;

        public OneSignalService(ILogger<OneSignalService> logger,
            IHttpClientFactory httpClientFactory,
            ILocalizationService localizationService)
        {
            this.logger = logger;
            this.localizationService = localizationService;
            
            httpClient = httpClientFactory.CreateClient();
        }

        public async Task<BaseResponse<object>> PushNotificationsAsync(OneSignalSettingModel setting,
            NotificationModel notification,
            IEnumerable<string> connectionIds)
        {
            try
            {
                if (!connectionIds.Any())
                {
                    logger.LogError($"PushNotificationsAsync notification: {notification.JsonSerialize()} no connectionIds to push");
                    return new BaseResponse<object>
                    {
                        Success = false
                    };
                }

                return await SendToOneSignalAsync(setting, notification, connectionIds);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return new BaseResponse<object>
                {
                    Success = false
                };
            }
        }

        public async Task<BaseResponse<object>> PushNotificationsToAllAsync(OneSignalSettingModel setting,
            NotificationModel notification)
        {
            try
            {
                return await SendToOneSignalAsync(setting, notification, new List<string>());
            }
            catch (Exception)
            {
                logger.LogError($"PushNotificationsToAllAsync notification: {notification.JsonSerialize()}");
                throw;
            }
        }

        private async Task<BaseResponse<object>> SendToOneSignalAsync(OneSignalSettingModel setting,
            NotificationModel notification,
            IEnumerable<string> connectionIds)
        {
            try
            {
                var prefix = $"{LocalizationConstants.Prefix.NOTIFICATION_SETTING}:{notification.Id}";
                var engCode = Constants.CultureCode.UnitedStates;

                var engTitle = localizationService
                    .GetLocalizationString(LocalizationConstants.MessageModelKey.TITLE, null, prefix, notification.Title, engCode);
                var engSubTitle = localizationService
                    .GetLocalizationString(LocalizationConstants.MessageModelKey.SUB_CONTENT, null, prefix, notification.Description, engCode);

                var reqBody = new NotificationRequest
                {
                    AppId = setting.AppId,
                    Contents = new Dictionary<string, string>
                    {
                        { Constants.CultureCode.UnitedStatesLower, engSubTitle }
                    },
                    Data = new NotificationRequestData
                    {
                        Id = notification.Id.ToString(),
                        ActionTo = notification.RedirectTo,
                        ActionType = notification.RedirectType,
                        Params = notification.ExtraParams,
                    },
                    Headings = new Dictionary<string, string>
                    {
                        { Constants.CultureCode.UnitedStatesLower, engTitle }
                    }
                };

                if (!string.IsNullOrWhiteSpace(notification.Image))
                {
                    reqBody.BigPicture = notification.Image;
                    reqBody.IosAttachments = new NotificationRequestIosAttachments
                    {
                        MediaId = notification.Image
                    };
                }

                if (connectionIds != null && connectionIds.Any())
                {
                    reqBody.IncludePlayerIds = connectionIds.ToList();
                }
                else
                {
                    reqBody.IncludedSegments = new List<string>() { "Active Users", "Inactive Users" };
                }

                if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", string.Format("Basic " + setting.Authentication));
                }

                if (!setting.ApiEndpoint.EndsWith("/"))
                {
                    setting.ApiEndpoint = $"{setting.ApiEndpoint}/";
                }

                var bodyContent = JsonExtensions.JsonSerialize(reqBody, true);
                var request = new StringContent(bodyContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync($"{setting.ApiEndpoint}api/v1/notifications", request);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    var resContent = await response.Content.ReadAsStringAsync();
                    logger.LogError($"OneSignal PushNotificationsAsync Error {resContent}");
                    return new BaseResponse<object>
                    {
                        Success = false
                    };
                }

                return new BaseResponse<object>
                {
                    Success = true
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}