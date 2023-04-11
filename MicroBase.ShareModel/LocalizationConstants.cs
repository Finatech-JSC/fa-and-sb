using System.Collections.Generic;

namespace MicroBase.Share
{
    public static class LocalizationConstants
    {
        public static class LanguageCode
        {
            /// <summary>
            /// ja
            /// </summary>
            public static string JAPANESE = "ja";
        }

        public static class Prefix
        {
            public static string APP_MENU = "APP_MENU";
            public static string APP_BANNER = "APP_BANNER";
            public static string EMAIL_TEMPLATE = "EMAIL_TEMPLATE";
            public static string SITE_SETTING = "SITE_SETTING";
            public static string NOTIFICATION_SETTING = "NOTIFICATION_SETTING";
        }

        public static Dictionary<string, string> PrefixDisc = new Dictionary<string, string>
        {
            { Prefix.APP_MENU, "App menu" },
            { Prefix.APP_BANNER, "App banner" },
            { Prefix.EMAIL_TEMPLATE, "Email template" }
        };

        public static class MessageModelKey
        {
            public const string CONTENT = "BODY_DESCRIPTION";
            public const string SUB_CONTENT = "SUB_CONTENT";
            public const string TITLE = "TITLE";
            public const string LINK = "LINK";
            public const string REDIRECT_TO_LINK = "REDIRECT_TO_LINK";
            public const string REDIRECT_TO_SCREEN = "REDIRECT_TO_SCREEN";
            public const string VIEW_DETAIL = "VIEW_DETAIL";
        }

        public static Dictionary<string, string> RedirectTypeDics = new Dictionary<string, string>
        {
            { MessageModelKey.REDIRECT_TO_LINK, "Open a link" },
            { MessageModelKey.REDIRECT_TO_SCREEN, "Navigate in app" },
            { MessageModelKey.VIEW_DETAIL, "Goto details" }
        };

        public static class Key
        {
            public const string DEFAULT_AFTER_REGISTER = "DEFAULT_AFTER_REGISTER";
            public const string DEFAULT_REMIND_UPDATE_APP_VERSION = "DEFAULT_REMIND_UPDATE_APP_VERSION";
            public const string DEFAULT_POP_UP_WHEN_USER_OPEN_APP = "DEFAULT_POP_UP_WHEN_USER_OPEN_APP";
            public const string ACTIVE_ACCOUNT = "ACTIVE_ACCOUNT";
            public const string REGISTER_ACCOUNT = "REGISTER_ACCOUNT";

            // KYC
            public const string KYC_FAILED = "KYC_FAILED";
            public const string KYC_VERIFIED = "KYC_VERIFIED";
        }

        public static Dictionary<string, string> KeyMaps = new Dictionary<string, string>
        {
            { Key.DEFAULT_AFTER_REGISTER, "Notification after registration success" },
            { Key.DEFAULT_REMIND_UPDATE_APP_VERSION, "Notification update app version" },
            { Key.DEFAULT_POP_UP_WHEN_USER_OPEN_APP, "Default popup when open app" },
            { Key.ACTIVE_ACCOUNT, "Active Account" },
            { Key.REGISTER_ACCOUNT, "Register Account" },
            { Key.KYC_FAILED, "KYC Failed" },
            { Key.KYC_VERIFIED, "KYC Verified" }
        };
    }
}