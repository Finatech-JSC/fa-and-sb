using MicroBase.Amazon;
using MicroBase.Service;
using MicroBase.Service.Accounts;
using MicroBase.Service.Accounts.Externals;
using MicroBase.Service.Emails;
using MicroBase.Service.Localizations;
using MicroBase.Service.Notifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicroBase.Module.IdentityApi
{
    public static class IdentityApiServiceModelue
    {
        public static void ModuleRegister(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ISocialAccountService, SocialAccountService>();
            services.AddTransient<IFacebookProviderService, FacebookProviderService>();
            services.AddTransient<IAppleProviderService, AppleProviderService>();
            
            services.AddTransient<IOtpTokenService, OtpTokenService>();
            services.AddTransient<IEmailTemplateService, EmailTemplateService>();
            services.AddTransient<ISiteSettingService, SiteSettingService>();
            services.AddTransient<IBuildEmailService, BuildEmailService>();
            services.AddTransient<IEmailFilterService, EmailFilterService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IGenericLocalizationService, GenericLocalizationService>();
            services.AddTransient<ILocalizationService, LocalizationService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<INotificationSettingService, NotificationSettingService>();
            services.AddTransient<INotificationUserService, NotificationUserService>();
            services.AddTransient<IOneSignalService, OneSignalService>();
            services.AddTransient<IFileUploadService, FileUploadService>();
            services.AddTransient<IAmazonService, AmazonService>();
        }
    }
}