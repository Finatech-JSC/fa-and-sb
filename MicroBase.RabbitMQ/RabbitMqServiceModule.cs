using MicroBase.RabbitMQ.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicroBase.RabbitMQ
{
    public static class RabbitMqServiceModule
    {
        public static void ModuleRegister(this IServiceCollection services, IConfiguration Configuration)
        {
            var rabbitMqSetting = new RabbitMqSetting
            {
                HostName = Configuration.GetValue<string>("RabbitMq:HostName"),
                VirtualHost = Configuration.GetValue<string>("RabbitMq:VirtualHost"),
                UserName = Configuration.GetValue<string>("RabbitMq:UserName"),
                Password = Configuration.GetValue<string>("RabbitMq:Password")
            };

            services.AddSingleton<IRabbitConnectionProvider>(new RabbitConnectionProvider(rabbitMqSetting));
            services.AddSingleton<IMessageBusService, MessageBusService>();
            services.AddSingleton<IRabbitMqService>(new RabbitMqService(rabbitMqSetting));
        }
    }
}