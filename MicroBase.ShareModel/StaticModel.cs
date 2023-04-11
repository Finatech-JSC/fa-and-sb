using MicroBase.Share.Models.Localizations.Localizations;
using System.Collections.Generic;

namespace MicroBase.Share
{
    public static class StaticModel
    {
        public static class Application
        {
            public static string Domain = "";
            public static string ShareRoom = "";
        }

        /// <summary>
        /// key: key to translate
        /// </summary>
        public static Dictionary<string, LocalizationKeyModel> LocalizationDics = new Dictionary<string, LocalizationKeyModel>();

        /// <summary>
        /// key: key is language code
        /// </summary>
        public static Dictionary<string, Dictionary<string, LocalizationKeyModel>> CountryLocalizationDics = new Dictionary<string, Dictionary<string, LocalizationKeyModel>>();

        public static string CMS_DEFAULT_LANGUAGE = LocalizationConstants.LanguageCode.JAPANESE;

        /// <summary>
        /// Set default is yyyy-MM-dd HH:mm:ss
        /// </summary>
        public static string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
    }
}