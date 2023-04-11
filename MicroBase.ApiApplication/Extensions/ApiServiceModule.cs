using MicroBase.Service;
using MicroBase.Service.Accounts;
using MicroBase.Service.Emails;
using MicroBase.Service.Localizations;
using MicroBase.Service.Notifications;

namespace MicroBase.ApiApplication.Extensions
{
    public static class ApiServiceModule
    {
        public static void ModuleRegister(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddTransient<IOtpTokenService, OtpTokenService>();
            services.AddTransient<IEmailTemplateService, EmailTemplateService>();
            services.AddTransient<ISiteSettingService, SiteSettingService>();
            services.AddTransient<IEmailFilterService, EmailFilterService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IGenericLocalizationService, GenericLocalizationService>();
            services.AddTransient<ILocalizationService, LocalizationService>();
            services.AddTransient<INotificationSettingService, NotificationSettingService>();
            services.AddTransient<INotificationUserService, NotificationUserService>();
            services.AddTransient<IOneSignalService, OneSignalService>();
        }
    }
}