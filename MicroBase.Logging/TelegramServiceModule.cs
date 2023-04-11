using MicroBase.Share.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace MicroBase.Logging
{
    public static class TelegramServiceModule
    {
        public static void ModuleRegister(this IServiceCollection services, IConfiguration Configuration)
        {
            BotConnectorModel botConnectors = new BotConnectorModel();
            var config = Configuration.GetSection("TelegramBot");
            if (config != null && !string.IsNullOrWhiteSpace(config.Value))
            {
                botConnectors = JsonExtensions.JsonDeserialize<BotConnectorModel>(config.Value);
            }

            services.AddSingleton<ITelegramBotService>(x => new TelegramBotService(x.GetRequiredService<IHttpClientFactory>(), botConnectors));
            services.AddSingleton<IExceptionMonitorService, ExceptionMonitorService>();
        }
    }
}