using MicroBase.Share;
using MicroBase.Share.Extensions;
using MicroBase.Share.Models.Localizations.Localizations;

namespace MicroBase.Service.Localizations
{
    public interface IFileLocalizationService
    {
        void LoadFromFile(string languageCode);
    }

    public static class FileLocalizationService
    {
        public static void LoadFromFile(string languageCode)
        {
            var localizations = new List<LocalizationKeyModel>();
            var filePath = $"Localizations/localization.{languageCode}.json";
            if (!File.Exists(filePath))
            {
                return;
            }

            using (StreamReader r = new StreamReader(filePath))
            {
                string jsonContent = r.ReadToEnd();
                localizations = JsonExtensions.JsonDeserialize<List<LocalizationKeyModel>>(jsonContent);
            }

            var localizationDics = new Dictionary<string, LocalizationKeyModel>();
            foreach (var localizationKey in localizations)
            {
                var item = new LocalizationKeyModel
                {
                    Key = localizationKey.Key,
                    Content = localizationKey.Content
                };

                if (!localizationDics.ContainsKey(localizationKey.Key))
                {
                    localizationDics.Add(localizationKey.Key, item);
                }
                else
                {
                    localizationDics[localizationKey.Key] = item;
                }
            }

            if (!StaticModel.CountryLocalizationDics.ContainsKey(languageCode))
            {
                StaticModel.CountryLocalizationDics.Add(languageCode, localizationDics);
            }
            else
            {
                StaticModel.CountryLocalizationDics[languageCode] = localizationDics;
            }
        }

        public static string GetLocalizationString(string localizationKey,
            string languageCode = null,
            List<string> msgParams = null,
            string fallbackContent = null)
        {
            if (string.IsNullOrWhiteSpace(localizationKey))
            {
                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(languageCode))
            {
                languageCode = LocalizationConstants.LanguageCode.JAPANESE;
            }

            var localizationDics = new Dictionary<string, LocalizationKeyModel>();
            if (StaticModel.CountryLocalizationDics.ContainsKey(languageCode))
            {
                localizationDics = StaticModel.CountryLocalizationDics[languageCode];
            }

            if (!localizationDics.Any())
            {
                return localizationKey;
            }

            var translateContent = string.Empty;
            LocalizationKeyModel localizationKeyModel = null;
            if (localizationDics.ContainsKey(localizationKey))
            {
                localizationKeyModel = localizationDics[localizationKey];
            }

            if (localizationKeyModel == null || string.IsNullOrWhiteSpace(localizationKeyModel.Content))
            {
                translateContent = !string.IsNullOrWhiteSpace(fallbackContent) ? fallbackContent : localizationKey;
            }
            else
            {
                translateContent = localizationKeyModel.Content;
            }

            if (msgParams != null && msgParams.Any())
            {
                translateContent = string.Format(translateContent, msgParams);
            }

            return translateContent;
        }
    }
}