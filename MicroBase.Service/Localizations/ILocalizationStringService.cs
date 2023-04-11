using MicroBase.Share.Models;

namespace MicroBase.Service.Localizations
{
    public interface ILocalizationStringService
    {
        string GetLocalizationString(string localizationKey,
            List<string> msgParams = null,
            string prefix = null,
            string fallbackContent = null);

        string GetLocalizationString(string localizationKey,
            List<string> msgParams = null,
            string prefix = null,
            string fallbackContent = null,
            string cultureCode = null);

        BaseResponse<T> LocalizationBaseResponse<T>(BaseResponse<T> model, string prefix = null);
    }
}