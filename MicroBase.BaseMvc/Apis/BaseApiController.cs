using MicroBase.Service.Localizations;
using MicroBase.Share.Extensions;
using MicroBase.Share.Models;
using MicroBase.Share.Models.Accounts;
using MicroBase.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using MicroBase.Share.Constants;

namespace MicroBase.BaseMvc.Apis
{
    public abstract class BaseApiController
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly ILocalizationService localizationService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IExceptionMonitorService exceptionMonitorService;

        protected BaseApiController(ILoggerFactory loggerFactory,
            ILocalizationService localizationService,
            IHttpContextAccessor httpContextAccessor,
            IExceptionMonitorService exceptionMonitorService)
        {
            this.loggerFactory = loggerFactory;
            this.localizationService = localizationService;
            this.httpContextAccessor = httpContextAccessor;
            this.exceptionMonitorService = exceptionMonitorService;
        }

        protected string GetLocalizationString(string localizationKey,
            List<string> msgParams = null,
            string prefix = null,
            string fallbackContent = null)
        {
            return localizationService.GetLocalizationString(localizationKey, msgParams, prefix, fallbackContent);
        }

        protected BaseResponse<T> LocalizationBaseResponse<T>(BaseResponse<T> model, string prefix = null)
        {
            var message = string.Empty;
            if (!string.IsNullOrWhiteSpace(model.MessageCode))
            {
                message = GetLocalizationString(model.MessageCode, model.MsgParams, prefix, fallbackContent: null);
            }

            if ((string.IsNullOrWhiteSpace(message) || message.Equals(model.MessageCode)) && !string.IsNullOrWhiteSpace(model.Message))
            {
                message = GetLocalizationString(model.Message, model.MsgParams, prefix, model.Message);
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                message = model.MessageCode;
            }

            model.Message = message;
            return model;
        }

        protected async Task<BaseResponse<T>> ReturnExceptionMessageAsync<T>(Exception ex)
        {
            var actions = new List<string>();
            var routeDatas = httpContextAccessor.HttpContext.GetRouteData().Values;
            foreach (var item in routeDatas)
            {
                actions.Add(item.Value.ToString());
            }

            actions.Reverse();
            await exceptionMonitorService.SendExceptionNotiAsync(string.Join(" > ", actions.ToArray()), ex);
            return new BaseResponse<T>
            {
                Success = false,
                Message = GetLocalizationString(CommonMessage.UN_DETECTED_ERROR),
                MessageCode = nameof(CommonMessage.UN_DETECTED_ERROR)
            };
        }

        protected BaseActivityTrackingModel GetTracking(string action, string description)
        {
            var tracking = new BaseActivityTrackingModel
            {
                Via = httpContextAccessor.HttpContext.GetVia(),
                IpAddress = httpContextAccessor.HttpContext.GetIpAddress(),
                Location = httpContextAccessor.HttpContext.GetLocation(),
                UserAgent = httpContextAccessor.HttpContext.GetUserAgent(),
                Action = action,
                Description = description
            };

            return tracking;
        }
    }
}