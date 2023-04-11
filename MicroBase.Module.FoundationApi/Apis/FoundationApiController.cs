using MicroBase.Service;
using MicroBase.Service.Localizations;
using MicroBase.Share.Constants;
using MicroBase.Share.Extensions;
using MicroBase.Share.Models;
using MicroBase.Share.Models.MobileApps;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MicroBase.Module.FoundationApi.Apis
{
    [ApiController]
    [Route("foundation")]
    public class FoundationApiController : ControllerBase
    {
        private readonly ISiteSettingService siteSettingService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILocalizationService localizationService;

        public FoundationApiController(ISiteSettingService siteSettingService,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService)
        {
            this.siteSettingService = siteSettingService;
            this.httpContextAccessor = httpContextAccessor;
            this.localizationService = localizationService;
        }

        [HttpGet("ads")]
        public async Task<BaseResponse<List<AdsModel>>> GetAds(string type)
        {
            var setting = await siteSettingService.GetByKeyAsync(Constants.SiteSettings.Keys.ADS_MANAGEMENT, true);
            var res = JsonExtensions.JsonDeserialize<List<AdsModel>>(setting.StringValue);

            return new BaseResponse<List<AdsModel>>
            {
                Success = true,
                Data = res
            };
        }

        [HttpGet("site-setting/by-key")]
        public async Task<BaseResponse<SiteSettingResponse>> GetSiteSettingByKey(string key)
        {
            var cultureCode = httpContextAccessor.HttpContext.GetRequestCultureCode();
            if (string.IsNullOrWhiteSpace(cultureCode))
            {
                cultureCode = Constants.CultureCode.UnitedStates;
            }

            var setting = await siteSettingService.GetByKeyAsync(key, false);
            if (setting != null)
            {
                setting.StringValue = localizationService.GetLocalizationString(setting.Id.ToString(),
                    msgParams: null,
                    prefix: string.Empty,
                    fallbackContent: setting.StringValue,
                    cultureCode: cultureCode);
            }

            return new BaseResponse<SiteSettingResponse>
            {
                Success = true,
                Data = setting
            };
        }
    }
}