using System.Collections.Generic;

namespace MicroBase.Share.Constants
{
    public class Constants
    {
        public static class Authentication
        {
            public static string COOKIES_NAME = "Identity.Application";
        }

        public static class HttpHeaderKey
        {
            public static string IP_ADDRESS = "X-IP-ADDRESS";
            public static string LOCATION = "X-LOCATION";
            public static string USER_AGENT = "X-USER-AGENT";
            public static string VIA = "X-VIA";
            public static string CULTURE_CODE = "X-CULTURE-CODE";
            public static string TIMESTAMP = "X-TIMESTAMP";
            public static string CSRF_TOKEN = "X-CSRF-TOKEN";
            public static string X_APP_VERSION = "X-APP-VERSION";
            public static string X_OS_PLATFORM = "X-OS-PLATFORM";
        }

        public class Jwt
        {
            public static class ClaimKeys
            {
                public static string Id = "UserId";

                public static string UserName = "Username";

                public static string AccountType = "AccountType";

                public static string Via = "Via";

                public static string IsConfirmed = "IsConfirmed";
            }

            public static class ApiClaimKeys
            {
                public static string Id = "UserId";

                public static string Api = "Api";

                public static string NftId = "NftId";
            }
        }

        public enum OtpType
        {
            REGISTER_OTP = 1,
            RESET_PASSWORD = 2,
            TWO_FA_CODE = 3
        }

        public static class Account
        {
            public enum Type
            {
                Operator = 1,
                Saler = 2,
                Banker = 3,
                Normal = 4
            }

            public enum RegisterAction
            {
                /// <summary>
                /// Required confirm Email or Phone
                /// </summary>
                MustConfirmEmailOrPhone = 1,

                /// <summary>
                /// Send notification via Email or Phone after user registered
                /// </summary>
                SendEmailAfterRegister = 2
            }

            public static class ActivityAction
            {
                public static string Login = "LOGIN";
                public static string Web3Login = "WEB3_LOGIN";
                public static string SocialLogin = "SOCIAL_LOGIN";
                public static string Register = "REGISTER";
                public static string ChangePassword = "CHANGE_PASSWORD";
                public static string ChangeDefaultPassword = "CHANGE_DEFAULT_PASSWORD";
                public static string ForgotPassword = "FORGOT_PASSWORD";
                public static string ResetPassword = "RESET_PASSWORD";
                public static string ConfirmResetPassword = "CONFIRM_RESET_PASSWORD";
                public static string UpdateProfile = "UPDATE_USER_PROFILE";
                public static string GetOtpFor = "GET_OTP_FOR_{0}";
                public static string VerifyOtpFor = "VERIFY_OTP_FOR_{0}";
            }

            public enum Gender
            {
                Male = 1,
                Female = 2,
                Other = 3
            }

            public static Dictionary<int, string> KeyMaps = new Dictionary<int, string>
            {
                { (int)Gender.Male, "Male" },
                { (int)Gender.Female, "Female" },
                { (int)Gender.Other, "Other" }
            };

            public enum TwoFAService
            {
                EMAIL,
                GOOGLE_AUTHENTICATOR,
                SMS
            }

            public static Dictionary<string, string> TwoFAServiceMaps = new Dictionary<string, string>
            {
                { TwoFAService.EMAIL.ToString(), "Email Authenticator" },
                { TwoFAService.GOOGLE_AUTHENTICATOR.ToString(), "Google Authenticator" },
                { TwoFAService.SMS.ToString(), "SMS" }
            };

            public enum ResponseCode
            {
                /// <summary>
                /// Required confirm email or phone number after user registered
                /// </summary>
                MustConfirmUsername = 1001,

                /// <summary>
                /// Required 2FA to complete login process
                /// </summary>
                Required2FA = 1002,

                NotAllow = 1003,
                LockedOut = 1004,
                TwoFASessionExpired = 1005,
            }
        }

        public static class CultureCode
        {
            public const string UnitedStates = "EN";
            public const string UnitedStatesLower = "en";

            public const string Japanese = "JA";
            public const string JapaneseLower = "ja";

            public static Dictionary<string, string> Maps = new Dictionary<string, string>
            {
                { UnitedStates, "English" }
            };
        }

        public static class EmailTemplates
        {
            public const string REGISTER_OTP_CONFIRM = "REGISTER_OTP_CONFIRM";
            public const string RESET_PASSWORD_OTP = "RESET_PASSWORD_OTP";
            public const string TERM_AND_CONDITION = "TERM_AND_CONDITION";
            public const string REGISTER_NOTIFICATION = "REGISTER_NOTIFICATION";
            public const string KYC_FAILED = "KYC_FAILED";
            public const string KYC_VERIFIED = "KYC_VERIFIED";
            public const string FOR_TEST = "FOR_TEST";
            public const string EMAIL_TWO_FA_CODE = "EMAIL_TWO_FA_CODE";
            public const string ACCOUNT_HAS_BEEN_LOCKED = "ACCOUNT_HAS_BEEN_LOCKED";
            public const string ACCOUNT_HAS_BEEN_LOCKED_FOREVER = "ACCOUNT_HAS_BEEN_LOCKED_FOREVER";

            public static Dictionary<string, string> KeyMaps = new Dictionary<string, string>
            {
                { REGISTER_OTP_CONFIRM, "Send OTP after account registration" },
                { RESET_PASSWORD_OTP, "Send OTP to request password reset" },
                { TERM_AND_CONDITION, "Terms of use" },
                { REGISTER_NOTIFICATION, "Notice after account registration" },
                { KYC_FAILED, "KYC notification failed" },
                { KYC_VERIFIED, "Successful KYC notification" },
                { FOR_TEST, "Draft email for testing" },
                { EMAIL_TWO_FA_CODE, "Notice of 2-factor authentication code" },
                { ACCOUNT_HAS_BEEN_LOCKED, "Your account has been locked for a limited time" },
                { ACCOUNT_HAS_BEEN_LOCKED_FOREVER, "Your account has been locked indefinitely" }
            };

            public static class EmailTokens
            {
                public const string USERNAME = "USERNAME";

                public const string EMAIL_ADDRESS = "EMAIL_ADDRESS";

                public const string PHONE_NUMBER = "PHONE_NUMBER";

                public const string OPT_CODE = "OPT_CODE";

                public const string ID_CODE = "ID_CODE";

                public const string BIRTH_DATE = "BIRTH_DATE";

                public const string USER_HEIGHT = "USER_HEIGHT";

                public const string LOCK_DATE = "LOCK_DATE";
            }
        }

        public static Dictionary<OtpType, string> OtpEmailMappings = new Dictionary<OtpType, string>
        {
            { OtpType.REGISTER_OTP, EmailTemplates.REGISTER_OTP_CONFIRM },
            { OtpType.RESET_PASSWORD, EmailTemplates.RESET_PASSWORD_OTP },
            { OtpType.TWO_FA_CODE, EmailTemplates.EMAIL_TWO_FA_CODE }
        };

        public enum Via
        {
            Unknow = 0,
            Internal = 1,
            Web = 2,
            Android = 3,
            IOS = 4,
            ApiIntergation = 5
        }

        public static string GetViaString(int? via)
        {
            if (via.HasValue)
            {
                return Via.Unknow.ToString();
            }

            if (via.Value == (int)Via.Internal)
            {
                return Via.Internal.ToString();
            }
            else if (via.Value == (int)Via.Web)
            {
                return Via.Web.ToString();
            }
            else if (via.Value == (int)Via.Android)
            {
                return Via.Android.ToString();
            }
            else if (via.Value == (int)Via.IOS)
            {
                return Via.IOS.ToString();
            }
            else if (via.Value == (int)Via.ApiIntergation)
            {
                return Via.ApiIntergation.ToString();
            }

            return Via.Unknow.ToString();
        }

        public static class ExportCSV
        {
            public const string HIDE_COLUMN = "HIDE_COLUMN";
        }

        public static class SiteSettings
        {
            public static class Keys
            {
                public const string SMTP_SETTING = "SMTP_SETTING";

                public const string MOBILE_SLIDE = "MOBILE_SLIDE";

                public const string NOTIFICATION_PROVIDER_SETTING = "NOTIFICATION_PROVIDER_SETTING";

                public const string MOBILE_SCREEN = "MOBILE_SCREEN";

                public const string MOBILE_APP_VERSION = "MOBILE_APP_VERSION";

                public const string REVIEW_MOBILE_APP_VERSION = "REVIEW_MOBILE_APP_VERSION";

                public const string EMAIL_FILTER = "EMAIL_FILTER";

                public const string MOBILE_MENU = "MOBILE_MENU";

                public const string SHARE_MEDIA = "SHARE_MEDIA";

                public const string SEO_CONFIGURATION = "SEO_CONFIGURATION";

                public const string GOOGLE_AUTHENTICATOR = "GOOGLE_AUTHENTICATOR";

                public const string SWEAR_LANGUAGE_FILTER = "SWEAR_LANGUAGE_FILTER";

                public const string ADS_MANAGEMENT = "ADS_MANAGEMENT";

                public const string EMAIL_SETTING_KEY = "EMAIL_SETTING_KEY";
            }

            public enum Fields
            {
                StringValue,
                BoolValue,
                NumberValue
            }

            public static Dictionary<string, string> KeyMaps = new Dictionary<string, string>
            {
                { "SMTP_SETTING", "SMTP Setting"},
                { "MOBILE_SLIDE", "Mobile Slide"},
                { "NOTIFICATION_PROVIDER_SETTING", "Notication Provider Setting"},
                { "MOBILE_SCREEN", "Mobile Screen"},
                { "MOBILE_APP_VERSION", "Mobile App Version"},
                { "REVIEW_MOBILE_APP_VERSION", "In-Review Mode"},
                { "EMAIL_FILTER", "Email Filter"},
                { "MOBILE_MENU", "Mobile Menu"},
                { "SEO_CONFIGURATION", "SEO Configuration"},
                { "GOOGLE_AUTHENTICATOR", "Google Authenticator"},
                { "SWEAR_LANGUAGE_FILTER", "Swear Language Filter"},
                { "ADS_MANAGEMENT", "Ads Management"},
            };
        }

        public static IEnumerable<SiteSettingModel> SiteSettingCollections = new List<SiteSettingModel>
        {
            new SiteSettingModel
            {
                 Key = SiteSettings.Keys.SMTP_SETTING,
                 Name = "SMTP Configuration"
            },
            new SiteSettingModel
            {
                 Key = SiteSettings.Keys.NOTIFICATION_PROVIDER_SETTING,
                 Name = "Notification providers"
            },
            new SiteSettingModel
            {
                 Key = SiteSettings.Keys.EMAIL_FILTER,
                 Name = "Email Filter"
            },
            new SiteSettingModel
            {
                 Key = SiteSettings.Keys.GOOGLE_AUTHENTICATOR,
                 Name = "Google Authenticator Configuration"
            }
        };

        public class SiteSettingModel
        {
            public string Key { get; set; }

            public string Name { get; set; }
        }

        public static class Mobile
        {
            public static class MenuType
            {
                public static class Type
                {
                    public const string HOME = "HOME";

                    public const string LEFT_MENU = "LEFT_MENU";

                    public const string HEADER = "HEADER";
                }

                public static Dictionary<string, string> TypeMaps = new Dictionary<string, string>
                {
                    { Type.HOME, "Home" },
                    { Type.LEFT_MENU , "Left Menu" },
                    { Type.HEADER , "Header" }
                };
            }

            public static class ActionType
            {
                public static class Action
                {
                    public const string NAVIGATE_IN_APP = "NAVIGATE_IN_APP";

                    public const string REDIRECT_TO_LINK = "REDIRECT_TO_LINK";

                    public const string READ_A_POST = "READ_A_POST";
                }

                public static Dictionary<string, string> ActionMaps = new Dictionary<string, string>
                {
                    { Action.NAVIGATE_IN_APP, "Navigate in app" },
                    { Action.REDIRECT_TO_LINK, "Open a link" },
                    { Action.READ_A_POST, "Read a post" },
                };
            }

            public static class OsPlatfrom
            {
                public static string Android = "ANDROID";

                public static string Ios = "IOS";
            }

            public static class InReviewSetting
            {
                public static class InReviewMode
                {
                    public const string SHOW_IN_REVIEW_MODE = "SHOW_IN_REVIEW_MODE";

                    public const string HIDE_IN_REVIEW_MODE = "HIDE_IN_REVIEW_MODE";
                }

                public static Dictionary<string, string> ActionMaps = new Dictionary<string, string>
                {
                    { InReviewMode.SHOW_IN_REVIEW_MODE, "Show In Review Mode" },
                    { InReviewMode.HIDE_IN_REVIEW_MODE, "Hide In Review Mode" }
                };
            }
        }

        public static class ExternalAccountProvider
        {
            public const string GOOGLE = "GOOGLE";
            public const string FACEBOOK = "FACEBOOK";
            public const string APPLE = "APPLE";
        }
    }
}